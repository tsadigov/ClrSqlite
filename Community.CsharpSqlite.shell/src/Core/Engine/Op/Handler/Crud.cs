using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using i64 = System.Int64;
using u8 = System.Byte;
using u16 = System.UInt16;
using u32 = System.UInt32;
using u64 = System.UInt64;
using unsigned = System.UIntPtr;
using Pgno = System.UInt32;
using i32 = System.Int32;
using sqlite_int64 = System.Int64;
using Community.CsharpSqlite.Engine;
using Community.CsharpSqlite.Utils;
using Community.CsharpSqlite.Metadata;

namespace Community.CsharpSqlite.Engine.Op
{
    using System.Text;
    using sqlite3_value = Engine.Mem;
    using System.Collections.Generic;
    using Community.CsharpSqlite.Engine;
    using Community.CsharpSqlite.Metadata;
    using Community.CsharpSqlite.Os;
    using Vdbe = Engine.Vdbe;
    using Core.Runtime;
    using CsharpSqlite.Core.Runtime;
    public class Crud
    {
        CPU cpu;
        public Crud(CPU cpu)
        {
            this.cpu = cpu;
            runtime.cpu = cpu;
        }

        static CrudRuntime s_runtime = new CrudRuntime();
        private static CrudRuntime runtime
        {
            get { return s_runtime; }
        }

        public static RuntimeException Exec(CPU cpu, OpCode opcode, VdbeOp pOp)
        {
            Crud c = new Crud(cpu);
            return c.Exec(opcode, pOp);
        }

        public RuntimeException Exec(OpCode opcode, VdbeOp pOp)
        //(Community.CsharpSqlite.Vdbe vdbe, OpCode opcode, ref int opcodeIndex,Mem [] aMem,VdbeOp pOp,ref SqlResult rc)
        {
            var vdbe = cpu.vdbe;
            var aMem = vdbe.aMem;
            var db = cpu.db;

            SqlResult rc;
            switch (opcode)
            {



                ///
                ///<summary>
                ///Opcode: Insert P1 P2 P3 P4 P5
                ///
                ///Write an entry into the table of cursor P1.  A new entry is
                ///created if it doesn't already exist or the data for an existing
                ///entry is overwritten.  The data is the value MEM.MEM_Blob stored in register
                ///number P2. The key is stored in register P3. The key must
                ///be a MEM.MEM_Int.
                ///
                ///If the OPFLAG_NCHANGE flag of P5 is set, then the row change count is
                ///incremented (otherwise not).  If the OPFLAG_LASTROWID flag of P5 is set,
                ///then rowid is stored for subsequent return by the
                ///sqlite3_last_insert_rowid() function (otherwise it is unmodified).
                ///
                ///If the OPFLAG_USESEEKRESULT flag of P5 is set and if the result of
                ///the last seek operation ( OpCode.OP_NotExists) was a success, then this
                ///operation will not attempt to find the appropriate row before doing
                ///the insert but will instead overwrite the row that the cursor is
                ///currently pointing to.  Presumably, the prior  OpCode.OP_NotExists opcode
                ///has already positioned the cursor correctly.  This is an optimization
                ///that boosts performance by avoiding redundant seeks.
                ///
                ///If the OPFLAG_ISUPDATE flag is set, then this opcode is part of an
                ///UPDATE operation.  Otherwise (if the flag is clear) then this opcode
                ///is part of an INSERT operation.  The difference is only important to
                ///the update hook.
                ///
                ///</summary>
                ///<param name="Parameter P4 may point to a string containing the table">name, or</param>
                ///<param name="may be NULL. If it is not NULL, then the update">hook </param>
                ///<param name="(sqlite3.xUpdateCallback) is invoked following a successful insert.">(sqlite3.xUpdateCallback) is invoked following a successful insert.</param>
                ///<param name=""></param>
                ///<param name="(WARNING/TODO: If P1 is a pseudo">cursor and P2 is dynamically</param>
                ///<param name="allocated, then ownership of P2 is transferred to the pseudo">cursor</param>
                ///<param name="and register P2 becomes ephemeral.  If the cursor is changed, the">and register P2 becomes ephemeral.  If the cursor is changed, the</param>
                ///<param name="value of register P2 will then change.  Make sure this does not">value of register P2 will then change.  Make sure this does not</param>
                ///<param name="cause any problems.)">cause any problems.)</param>
                ///<param name=""></param>
                ///<param name="This instruction only works on tables.  The equivalent instruction">This instruction only works on tables.  The equivalent instruction</param>
                ///<param name="for indices is  OpCode.OP_IdxInsert.">for indices is  OpCode.OP_IdxInsert.</param>
                ///<param name=""></param>
                ///
                ///<summary>
                ///Opcode: InsertInt P1 P2 P3 P4 P5
                ///
                ///This works exactly like  OpCode.OP_Insert except that the key is the
                ///integer value P3, not the value of the integer stored in register P3.
                ///
                ///</summary>
                case OpCode.OP_Insert:
                case OpCode.OP_InsertInt:
                    {
                        runtime.Insert(pOp);                        
                        break;
                    }

                ///
                ///<summary>
                ///Opcode: Delete P1 P2 * P4 *
                ///
                ///Delete the record at which the P1 cursor is currently pointing.
                ///
                ///The cursor will be left pointing at either the next or the previous
                ///record in the table. If it is left pointing at the next record, then
                ///</summary>
                ///<param name="the next Next instruction will be a no">op.  Hence it is OK to delete</param>
                ///<param name="a record from within an Next loop.">a record from within an Next loop.</param>
                ///<param name=""></param>
                ///<param name="If the OPFLAG_NCHANGE flag of P2 is set, then the row change count is">If the OPFLAG_NCHANGE flag of P2 is set, then the row change count is</param>
                ///<param name="incremented (otherwise not).">incremented (otherwise not).</param>
                ///<param name=""></param>
                ///<param name="P1 must not be pseudo">table.  It has to be a real table with</param>
                ///<param name="multiple rows.">multiple rows.</param>
                ///<param name=""></param>
                ///<param name="If P4 is not NULL, then it is the name of the table that P1 is">If P4 is not NULL, then it is the name of the table that P1 is</param>
                ///<param name="pointing to.  The update hook will be invoked, if it exists.">pointing to.  The update hook will be invoked, if it exists.</param>
                ///<param name="If P4 is not NULL then the P1 cursor must have been positioned">If P4 is not NULL then the P1 cursor must have been positioned</param>
                ///<param name="using  OpCode.OP_NotFound prior to invoking this opcode.">using  OpCode.OP_NotFound prior to invoking this opcode.</param>
                ///<param name=""></param>
                case OpCode.OP_Delete:
                    {
                        i64 iKey;
                        VdbeCursor pC;
                        iKey = 0;
                        Debug.Assert(pOp.p1 >= 0 && pOp.p1 < vdbe.nCursor);
                        pC = vdbe.OpenCursors[pOp.p1];
                        Debug.Assert(pC != null);
                        Debug.Assert(pC.pCursor != null);
                        ///
                        ///<summary>
                        ///Only valid for real tables, no pseudotables 
                        ///</summary>
                        ///
                        ///<summary>
                        ///</summary>
                        ///<param name="If the update">hook will be invoked, set iKey to the rowid of the</param>
                        ///<param name="row being deleted.">row being deleted.</param>
                        if (db.xUpdateCallback != null && pOp.p4.z != null)
                        {
                            Debug.Assert(pC.isTable);
                            Debug.Assert(pC.rowidIsValid);
                            ///
                            ///<summary>
                            ///lastRowid set by previous  OpCode.OP_NotFound 
                            ///</summary>
                            iKey = pC.lastRowid;
                        }
                        ///
                        ///<summary>
                        ///The  OpCode.OP_Delete opcode always follows an  OpCode.OP_NotExists or  OpCode.OP_Last or
                        ///OP_Column on the same table without any intervening operations that
                        ///might move or invalidate the cursor.  Hence cursor pC is always pointing
                        ///to the row to be deleted and the sqlite3VdbeCursorMoveto() operation
                        ///</summary>
                        ///<param name="below is always a no">op and cannot fail.  We will run it anyhow, though,</param>
                        ///<param name="to guard against future changes to the code generator.">to guard against future changes to the code generator.</param>
                        ///<param name=""></param>
                        Debug.Assert(pC.deferredMoveto == false);
                        rc = vdbeaux.sqlite3VdbeCursorMoveto(pC);
                        if (Sqlite3.NEVER(rc != SqlResult.SQLITE_OK))
                            return RuntimeException.abort_due_to_error;
                        pC.pCursor.sqlite3BtreeSetCachedRowid(0);
                        rc = pC.pCursor.sqlite3BtreeDelete();
                        pC.cacheStatus = Sqlite3.CACHE_STALE;
                        ///
                        ///<summary>
                        ///</summary>
                        ///<param name="Invoke the update">hook if required. </param>
                        if (rc == SqlResult.SQLITE_OK && db.xUpdateCallback != null && pOp.p4.z != null)
                        {
                            string zDb = db.Backends[pC.iDb].Name;
                            string zTbl = pOp.p4.z;
                            db.xUpdateCallback(db.pUpdateArg, AuthTarget.SQLITE_DELETE, zDb, zTbl, iKey);
                            Debug.Assert(pC.iDb >= 0);
                        }
                        if ((pOp.p2 & (int)OpFlag.OPFLAG_NCHANGE) != 0)
                            vdbe.nChange++;
                        break;
                    }


                ///
                ///<summary>
                ///Opcode: NewRowid P1 P2 P3 * *
                ///
                ///Get a new integer record number (a.k.a "rowid") used as the key to a table.
                ///The record number is not previously used as a key in the database
                ///table that cursor P1 points to.  The new record number is written
                ///written to register P2.
                ///
                ///If P3>0 then P3 is a register in the root frame of this VDBE that holds 
                ///the largest previously generated record number. No new record numbers are
                ///allowed to be less than this value. When this value reaches its maximum, 
                ///an SQLITE_FULL error is generated. The P3 register is updated with the '
                ///generated record number. This P3 mechanism is used to help implement the
                ///AUTOINCREMENT feature.
                ///
                ///</summary>
                case OpCode.OP_NewRowid:
                    #region generate rowid
                    {
                        RuntimeException r= runtime.NewRowId(pOp);
                        if (r != RuntimeException.OK)
                            return r;
                        break;
                    }
#endregion

                default: return RuntimeException.noop;
            }

            
            return RuntimeException.OK;
        }
    }
}
