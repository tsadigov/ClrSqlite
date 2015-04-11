﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using u8 = System.Byte;

namespace Community.CsharpSqlite.Engine.Op
{
    public class Schema
    {
        public static RuntimeException Exec(CPU cpu, OpCode opcode, VdbeOp pOp) 
        //public static bool Exec(ref OnConstraintError errorAction, OpCode opcode, VdbeOp pOp, sqlite3 db, Mem pOut, ref u8 resetSchemaOnFault, ref SqlResult rc)
        {
            Mem pOut=cpu.pOut;
            var db = cpu.db;
            switch (opcode)
            {
                ///<summary>
                ///Opcode: Destroy P1 P2 P3 * *
                ///
                ///Delete an entire database table or index whose root page in the database
                ///file is given by P1.
                ///
                ///The table being destroyed is in the main database file if P3==0.  If
                ///P3==1 then the table to be clear is in the auxiliary database file
                ///that is used to store tables create using CREATE TEMPORARY TABLE.
                ///
                ///If AUTOVACUUM is enabled then it is possible that another root page
                ///might be moved into the newly deleted root page in order to keep all
                ///root pages contiguous at the beginning of the database.  The former
                ///</summary>
                ///<param name="value of the root page that moved "></param>
                ///<param name="is stored in register P2.  If no page">is stored in register P2.  If no page</param>
                ///<param name="movement was required (because the table being dropped was already">movement was required (because the table being dropped was already</param>
                ///<param name="the last one in the database) then a zero is stored in register P2.">the last one in the database) then a zero is stored in register P2.</param>
                ///<param name="If AUTOVACUUM is disabled then a zero is stored in register P2.">If AUTOVACUUM is disabled then a zero is stored in register P2.</param>
                ///<param name=""></param>
                ///<param name="See also: Clear">See also: Clear</param>
                ///<param name=""></param>
                case OpCode.OP_Destroy:
                    {
                        ///<param name="out2">prerelease </param>
                        int iMoved = 0;
                        int iDb;
#if !SQLITE_OMIT_VIRTUALTABLE
                        var iCnt = db.pVdbe.linkedList()
                            .Where(pVdbe=>pVdbe.magic == VdbeMagic.VDBE_MAGIC_RUN && pVdbe.inVtabMethod < 2 && pVdbe.currentOpCodeIndex >= 0)
                            .Count();
#else
																																																																																																																																				              iCnt = db.activeVdbeCnt;
#endif
                        pOut.flags = MemFlags.MEM_Null;
                        if (iCnt > 1)
                        {
                            cpu.rc = SqlResult.SQLITE_LOCKED;
                            cpu.errorAction = OnConstraintError.OE_Abort;
                        }
                        else
                        {
                            iDb = pOp.p3;
                            Debug.Assert(iCnt == 1);
                            //Debug.Assert((vdbe.btreeMask & (((yDbMask)1) << iDb)) != 0);//TODO:ERROR
                            cpu.rc = db.aDb[iDb].pBt.sqlite3BtreeDropTable(pOp.p1, ref iMoved);
                            pOut.flags = MemFlags.MEM_Int;
                            pOut.u.i = iMoved;
#if !SQLITE_OMIT_AUTOVACUUM
                            if (cpu.rc == SqlResult.SQLITE_OK && iMoved != 0)
                            {
                                build.sqlite3RootPageMoved(db, iDb, iMoved, pOp.p1);
                                ///
                                ///<summary>
                                ///All  OpCode.OP_Destroy operations occur on the same btree 
                                ///</summary>
                                Debug.Assert(cpu.resetSchemaOnFault == 0 || cpu.resetSchemaOnFault == iDb + 1);
                                cpu.resetSchemaOnFault = (u8)(iDb + 1);
                            }
#endif
                        }
                        break;
                    }
                ///<summary>
                ///Opcode: DropTable P1 * * P4 *
                ///</summary>
                ///Remove the internal (in">memory) data structures that describe</param>
                ///the table named P4 in database P1.  This is called after a table</param>
                ///is dropped in order to keep the internal representation of the</param>
                ///schema consistent with what is on disk.</param>
                case OpCode.OP_DropTable:
                    {
                        TableBuilder.sqlite3UnlinkAndDeleteTable(db, pOp.p1, pOp.p4.z);
                        break;
                    }
                ///
                ///<summary>
                ///Opcode: DropIndex P1 * * P4 *
                ///
                ///Remove the internal (in-memory) data structures that describe
                ///the index named P4 in database P1.  This is called after an index
                ///is dropped in order to keep the internal representation of the
                ///schema consistent with what is on disk.
                case OpCode.OP_DropIndex:
                    {
                        IndexBuilder.sqlite3UnlinkAndDeleteIndex(db, pOp.p1, pOp.p4.z);
                        break;
                    }
                ///
                ///<summary>
                ///Opcode: DropTrigger P1 * * P4 *
                ///
                ///</summary>
                ///<param name="Remove the internal (in">memory) data structures that describe</param>
                ///<param name="the trigger named P4 in database P1.  This is called after a trigger">the trigger named P4 in database P1.  This is called after a trigger</param>
                ///<param name="is dropped in order to keep the internal representation of the">is dropped in order to keep the internal representation of the</param>
                ///<param name="schema consistent with what is on disk.">schema consistent with what is on disk.</param>
                ///<param name=""></param>
                case OpCode.OP_DropTrigger:
                    {
                        TriggerParser.sqlite3UnlinkAndDeleteTrigger(db, pOp.p1, pOp.p4.z);
                        break;
                    }


                //
                ///<summary>
                ///Opcode: CreateTable P1 P2 * * *
                ///
                ///Allocate a new table in the main database file if P1==0 or in the
                ///auxiliary database file if P1==1 or in an attached database if
                ///P1>1.  Write the root page number of the new table into
                ///register P2
                ///
                ///The difference between a table and an index is this:  A table must
                ///</summary>
                ///<param name="have a 4">byte integer key and can have arbitrary data.  An index</param>
                ///<param name="has an arbitrary key but no data.">has an arbitrary key but no data.</param>
                ///<param name=""></param>
                ///<param name="See also: CreateIndex">See also: CreateIndex</param>
                ///<param name=""></param>
                ///
                ///<summary>
                ///Opcode: CreateIndex P1 P2 * * *
                ///
                ///Allocate a new index in the main database file if P1==0 or in the
                ///auxiliary database file if P1==1 or in an attached database if
                ///P1>1.  Write the root page number of the new table into
                ///register P2.
                ///
                ///See documentation on  OpCode.OP_CreateTable for additional information.
                ///
                ///</summary>
                case OpCode.OP_CreateIndex:
                ///
                ///<summary>
                ///</summary>
                ///<param name="out2">prerelease </param>
                case OpCode.OP_CreateTable:
                    {
                        ///<param name="out2">prerelease </param>
                        int flags;
                        var pgno = 0;
                        Debug.Assert(pOp.p1 >= 0 && pOp.p1 < db.nDb);
                        //Debug.Assert((this.btreeMask & (((yDbMask)1) << pOp.p1)) != 0);//TODO:ERROR
                        var pDb = db.aDb[pOp.p1];
                        Debug.Assert(pDb.pBt != null);
                        if (pOp.OpCode == OpCode.OP_CreateTable)
                        {
                            ///flags = BTREE_INTKEY; 
                            flags = Sqlite3.BTREE_INTKEY;
                        }
                        else
                        {
                            flags = Sqlite3.BTREE_BLOBKEY;
                        }
                        cpu.rc = pDb.pBt.sqlite3BtreeCreateTable(ref pgno, flags);
                        pOut.u.i = pgno;
                        break;
                    }
                default: return RuntimeException.noop;
            }

            return RuntimeException.OK;
        }
    }
}
