using Community.CsharpSqlite.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using u8 = System.Byte;
using u32 = System.UInt32;

namespace Community.CsharpSqlite.Compiler.CodeGeneration
{
    using Utils;
    using Ast;
    using Metadata;
    using System.Diagnostics;
    using ParseState = Sqlite3.ParseState;
    using CsharpSqlite.Parsing;
    public class TriggerBuilder
    {

        ///<summary>
        /// Generate VDBE code for the statements inside the body of a single
        /// trigger.
        ///</summary>
        public static int codeTriggerProgram(
            ParseState pParse,///The parser context 
            TriggerStep pStepList,///List of statements inside the trigger body 
            OnConstraintError orconf///Conflict algorithm. (OnConstraintError.OE_Abort, etc) 
        )
        {
            Vdbe v = pParse.pVdbe;
            Connection db = pParse.db;
            Debug.Assert(pParse.pTriggerTab != null && pParse.pToplevel != null);
            Debug.Assert(pStepList != null);
            Debug.Assert(v != null);
            pStepList.linkedList().ForEach(
                pStep => {
                    ///Figure out the ON CONFLICT policy that will be used for this step
                    ///of the trigger program. If the statement that caused this trigger
                    ///to fire had an explicit ON CONFLICT, then use it. Otherwise, use
                    ///the ON CONFLICT policy that was specified as part of the trigger
                    ///step statement. Example:
                    ///
                    ///CREATE TRIGGER AFTER INSERT ON t1 BEGIN;
                    ///INSERT OR REPLACE INTO t2 VALUES(new.a, new.b);
                    ///END;
                    ///
                    ///<param name="INSERT INTO t1 ... ;            "> insert into t2 uses REPLACE policy</param>
                    ///<param name="INSERT OR IGNORE INTO t1 ... ;  "> insert into t2 uses IGNORE policy</param>
                    ///<param name=""></param>
                    pParse.eOrconf = orconf.Filter(OnConstraintError.OE_Default, pStep.orconf);
                    switch (pStep.Operator)
                    {
                        case TokenType.TK_UPDATE:
                            {
                                pParse.sqlite3Update(TriggerParser.targetSrcList(pParse, pStep), exprc.Duplicate(db, pStep.pExprList, 0), exprc.Duplicate(db, pStep.pWhere, 0), pParse.eOrconf);
                                break;
                            }
                        case TokenType.TK_INSERT:
                            {
                                pParse.sqlite3Insert(TriggerParser.targetSrcList(pParse, pStep), exprc.Duplicate(db, pStep.pExprList, 0), exprc.Clone(db, pStep.pSelect, 0), exprc.sqlite3IdListDup(db, pStep.pIdList), pParse.eOrconf);
                                break;
                            }
                        case TokenType.TK_DELETE:
                            {
                                pParse.sqlite3DeleteFrom(TriggerParser.targetSrcList(pParse, pStep), exprc.Duplicate(db, pStep.pWhere, 0));
                                break;
                            }
                        default:
                            Debug.Assert(pStep.Operator == TokenType.TK_SELECT);
                            {
                                SelectDest sDest = new SelectDest();
                                Select pSelect = exprc.Clone(db, pStep.pSelect, 0);
                                sDest.Init(SelectResultType.Discard, 0);
                                Compiler.CodeGeneration.ForSelect.codegenSelect(pParse, pSelect, ref sDest);
                                SelectMethods.SelectDestructor(db, ref pSelect);
                                break;
                            }
                    }
                    if (pStep.Operator != TokenType.TK_SELECT)
                    {
                        v.sqlite3VdbeAddOp0(OpCode.OP_ResetCount);
                    }
                }
            );

            return 0;
        }

        ///<summary>
        /// This is called to code the required FOR EACH ROW triggers for an operation
        /// on table pTab. The operation to code triggers for (INSERT, UPDATE or DELETE)
        /// is given by the op paramater. The tr_tm parameter determines whether the
        /// BEFORE or AFTER triggers are coded. If the operation is an UPDATE, then
        /// parameter pChanges is passed the list of columns being modified.
        ///
        /// If there are no triggers that fire at the specified time for the specified
        /// operation on pTab, this function is a no-op.
        ///
        /// The reg argument is the address of the first in an array of registers
        /// that contain the values substituted for the new.* and old.* references
        /// in the trigger program. If N is the number of columns in table pTab
        /// (a copy of pTab.nCol), then registers are populated as follows:
        ///
        ///   Register       Contains
        ///   ------------------------------------------------------
        ///   reg+0          OLD.rowid
        ///   reg+1          OLD.* value of left-most column of pTab
        ///   ...            ...
        ///   reg+N          OLD.* value of right-most column of pTab
        ///   reg+N+1        NEW.rowid
        ///   reg+N+2        OLD.* value of left-most column of pTab
        ///   ...            ...
        ///   reg+N+N+1      NEW.* value of right-most column of pTab
        ///
        /// For ON DELETE triggers, the registers containing the NEW.* values will
        /// never be accessed by the trigger program, so they are not allocated or
        /// populated by the caller (there is no data to populate them with anyway).
        /// Similarly, for ON INSERT triggers the values stored in the OLD.* registers
        /// are never accessed, and so are not allocated by the caller. So, for an
        /// ON INSERT trigger, the value passed to this function as parameter reg
        /// is not a readable register, although registers (reg+N) through
        /// (reg+N+N+1) are.
        ///
        /// Parameter orconf is the default conflict resolution algorithm for the
        /// trigger program to use (REPLACE, IGNORE etc.). Parameter ignoreJump
        /// is the instruction that control should jump to if a trigger program
        /// raises an IGNORE exception.
        ///
        ///</summary>
        public static void sqlite3CodeRowTrigger(
            ParseState pParse,///Parse context 
            Trigger pTrigger,///List of triggers on table pTab 
            TokenType op,///One of TokenType.TK_UPDATE, TokenType.TK_INSERT, TokenType.TK_DELETE 
            ExprList pChanges,///Changes list for any UPDATE OF triggers 
            TriggerType tr_tm,///One of TriggerType.TRIGGER_BEFORE, TriggerType.TRIGGER_AFTER 		
            Table pTab,///The table to code triggers from 
            int reg,///The first in an array of registers (see above) 
            OnConstraintError orconf,///ON CONFLICT policy 
            int ignoreJump///Instruction to jump to for RAISE(IGNORE) 
        )
        {
            Trigger p;
            ///Used to iterate through pTrigger list 
            Debug.Assert(op == TokenType.TK_UPDATE || op == TokenType.TK_INSERT || op == TokenType.TK_DELETE);
            Debug.Assert(tr_tm == TriggerType.TRIGGER_BEFORE || tr_tm == TriggerType.TRIGGER_AFTER);
            Debug.Assert((op == TokenType.TK_UPDATE) == (pChanges != null));
            pTrigger.linkedList().ForEach(itr =>
            {
                p = itr;
                ///Sanity checking:  The schema for the trigger and for the table are
                ///always defined.  The trigger must be in the same schema as the table
                ///or else it must be a TEMP trigger. 
                Debug.Assert(p.pSchema != null);
                Debug.Assert(p.pTabSchema != null);
                Debug.Assert(p.pSchema == p.pTabSchema || p.pSchema == pParse.db.Backends[1].pSchema);
                ///Determine whether we should code this trigger 
                if (p.Operator == op && p.tr_tm == tr_tm && TriggerParser.checkColumnOverlap(p.pColumns, pChanges) != 0)
                {
                    sqlite3CodeRowTriggerDirect(pParse, p, pTab, reg, orconf, ignoreJump);
                }
            }
            );
        }

        ///<summary>
        /// Generate code for the trigger program associated with trigger p on
        /// table pTab. The reg, orconf and ignoreJump parameters passed to this
        /// function are the same as those described in the header function for
        /// sqlite3CodeRowTrigger()
        ///
        ///</summary>
        public static void sqlite3CodeRowTriggerDirect(
            ParseState pParse,///Parse context 
            Trigger p,///Trigger to code 
            Table pTab,///The table to code triggers from 
            int reg,///Reg array containing OLD.* and NEW.* values 
            OnConstraintError orconf,///ON CONFLICT policy 
            int ignoreJump///Instruction to jump to for RAISE(IGNORE) 
        )
        {
            Vdbe v = pParse.sqlite3GetVdbe();
            ///Main VM 
            var pPrg = TriggerParser.getRowTrigger(pParse, p, pTab, orconf);
            Debug.Assert(pPrg != null || pParse.nErr != 0);
            //|| pParse.db.mallocFailed );
            ///Code the  OpCode.OP_Program opcode in the parent VDBE. P4 of the  OpCode.OP_Program 
            ///is a pointer to the sub-vdbe containing the trigger program.
            if (pPrg != null)
            {
                bool bRecursive = (!String.IsNullOrEmpty(p.zName) && 0 == (pParse.db.flags & SqliteFlags.SQLITE_RecTriggers));
                v.AddOpp3(OpCode.OP_Program, reg, ignoreJump, ++pParse.UsedCellCount);
                v.sqlite3VdbeChangeP4(-1, pPrg.pProgram, P4Usage.P4_SUBPROGRAM);
#if SQLITE_DEBUG
																																																																																																        VdbeComment
            ( v, "Call: %s.%s", ( !String.IsNullOrEmpty( p.zName ) ? p.zName : "fkey" ), onErrorText( orconf ) );
#endif
                ///Set the P5 operand of the  OpCode.OP_Program instruction to non-zero if
                ///recursive invocation of this trigger program is disallowed. Recursive
                ///invocation is disallowed if (a) the sub-program is really a trigger,
                ///not a foreign key action, and (b) the flag to enable recursive triggers
                ///is clear.
                v.ChangeP5((u8)(bRecursive ? 1 : 0));
            }
        }
    }
}
