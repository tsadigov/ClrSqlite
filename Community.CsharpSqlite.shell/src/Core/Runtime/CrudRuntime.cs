using Community.CsharpSqlite.Engine;
using System;
using System.Diagnostics;
using System.Linq;
using i64 = System.Int64;
using Community.CsharpSqlite.Utils;
using Community.CsharpSqlite.Engine.Core.Runtime;
using Community.CsharpSqlite.Runtime;

namespace Community.CsharpSqlite.Core.Runtime
{
    public class CrudRuntime: IRuntime
    {
        public CPU cpu { get; set; }
        public CrudRuntime()
        { }
        public CrudRuntime(CPU p)
        {
            this.cpu = p;
        }

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
        internal RuntimeException NewRowId(VdbeOp pOp) {
            return NewRowId(
                resultAddress: pOp.p3,
                cursorIndex: pOp.p1
            );
        }

        ///
        ///<summary>
        ///Some compilers complain about constants of the form 0x7fffffffffffffff.
        ///Others complain about 0x7ffffffffffffffffLL.  The following macro seems
        ///to provide the constant while making all compilers happy.
        ///</summary>
        const long MAX_ROWID = i64.MaxValue;

        internal RuntimeException NewRowId(int resultAddress, int cursorIndex)
        {
            var vdbe = cpu.vdbe;
            ///<param name="out2">prerelease </param>
            i64 newRowId = 0;///The new rowid

            Debug.Assert(cursorIndex >= 0 && cursorIndex < vdbe.nCursor);
            var vdbeCursor = vdbe.OpenCursors[cursorIndex];///Cursor of table to get the new rowid 
            Debug.Assert(vdbeCursor != null);

            RuntimeException exp = vdbeCursor.generateNewRowId(cpu.lastRowid, getMemForNewRowId(resultAddress), ref newRowId, ref cpu.rc);
            if (exp == RuntimeException.OK)
            {
                cpu.pOut.u.AsInteger = (long)newRowId;
            }
            return RuntimeException.OK;
        }



        private Mem getMemForNewRowId(int resultAddress) {
            var vdbe = cpu.vdbe;

            VdbeFrame rootFrame;///Root frame of VDBE 
            Mem pMem = null;///Register holding largest rowid for AUTOINCREMENT 
            if (resultAddress != 0)
            {
                ///Assert that P3 is a valid memory cell. 

                Debug.Assert(resultAddress > 0);
                if (vdbe.pFrame != null)
                {
                    rootFrame = vdbe.pFrame.GetRoot();
                    ///Assert that P3 is a valid memory cell. 
                    Debug.Assert(resultAddress <= rootFrame.aMem.Count());
                    pMem = rootFrame.aMem[resultAddress];
                }
                else
                {
                    ///Assert that P3 is a valid memory cell. 
                    Debug.Assert(resultAddress <= vdbe.aMem.Count());
                    pMem = cpu.aMem[resultAddress];
                    vdbe.memAboutToChange(pMem);
                }
                Debug.Assert(pMem.memIsValid());
                Sqlite3.REGISTER_TRACE(vdbe, resultAddress, pMem);
                pMem.Integerify();
                Debug.Assert((pMem.flags & MemFlags.MEM_Int) != 0);
            }
            return pMem;
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
        public RuntimeException Delete(VdbeOp pOp) {
            return Delete(pOp.p1,pOp.p2, pOp.p4.z,ref cpu.rc);
        }
        public RuntimeException Delete(int p1,int p2,String p4,ref SqlResult rc)
        {
            var vdbe =cpu.vdbe;
            var db = cpu.db;

            Debug.Assert(p1 >= 0 && p1 < vdbe.nCursor);
            var pC = vdbe.OpenCursors[p1];
            Debug.Assert(pC != null);
            Debug.Assert(pC.pCursor != null);

            ///Only valid for real tables, no pseudotables 
            ///If the update-hook will be invoked, set iKey to the rowid of the
            ///row being deleted.
            i64 iKey = 0;
            if (db.xUpdateCallback != null && p4 != null)
            {
                Debug.Assert(pC.isTable);
                Debug.Assert(pC.rowidIsValid);
                ///lastRowid set by previous  OpCode.OP_NotFound 
                iKey = pC.lastRowid;
            }
            ///The  OpCode.OP_Delete opcode always follows an  OpCode.OP_NotExists or  OpCode.OP_Last or
            ///OP_Column on the same table without any intervening operations that
            ///might move or invalidate the cursor.  Hence cursor pC is always pointing
            ///to the row to be deleted and the sqlite3VdbeCursorMoveto() operation
            ///<param name="below is always a no">op and cannot fail.  We will run it anyhow, though,</param>
            ///<param name="to guard against future changes to the code generator.">to guard against future changes to the code generator.</param>
            Debug.Assert(pC.deferredMoveto == false);
            rc = vdbeaux.sqlite3VdbeCursorMoveto(pC);
            if (Sqlite3.NEVER(rc != SqlResult.SQLITE_OK))
                return RuntimeException.abort_due_to_error;
            pC.pCursor.sqlite3BtreeSetCachedRowid(0);
            rc = pC.pCursor.sqlite3BtreeDelete();
            pC.cacheStatus = Sqlite3.CACHE_STALE;
            ///Invoke the update-hook if required.
            if (rc == SqlResult.SQLITE_OK && db.xUpdateCallback != null && p4 != null)
            {
                string databaseName = db.Backends[pC.iDb].Name;
                string tableName = p4;
                db.xUpdateCallback(db.pUpdateArg, AuthTarget.SQLITE_DELETE, databaseName, tableName, iKey);
                Debug.Assert(pC.iDb >= 0);
            }
            if ((p2 & (int)OpFlag.OPFLAG_NCHANGE) != 0)
                vdbe.nChange++;
            return RuntimeException.OK;
        }

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
        public RuntimeException Insert(VdbeOp pOp)
        {
            Debug.Assert(pOp.OpCode == OpCode.OP_InsertInt|| pOp.OpCode == OpCode.OP_Insert);

            i64 iKey;///The integer ROWID or key for the record to be inserted 
            if (pOp.OpCode == OpCode.OP_Insert)
            {
                var pKey = cpu.aMem[pOp.p3];///MEM cell holding key  for the record 
                Debug.Assert((pKey.flags & MemFlags.MEM_Int) != 0);
                Debug.Assert(pKey.memIsValid());
                Sqlite3.REGISTER_TRACE(cpu.vdbe, pOp.p3, pKey);
                iKey = pKey.u.AsInteger;
            }
            else
            {
                iKey = pOp.p3;
            }
            Insert(pOp.p1, pOp.p2, iKey, pOp.p4.z,(OpFlag)pOp.p5);

            return RuntimeException.OK;
        }

        public void Insert(int cursorIndex,int recordAddress, i64 iKey, string tableName, OpFlag opflags)
        {
            var vdbe = cpu.vdbe;
            Mem pData = cpu.aMem[recordAddress];///MEM cell holding data for the record to be inserted 
            Debug.Assert(cursorIndex >= 0 && cursorIndex < vdbe.nCursor);
            Debug.Assert(pData.memIsValid());

            VdbeCursor vdbeCursor = vdbe.OpenCursors[cursorIndex];///Cursor to table into which insert is written             
            Debug.Assert(vdbeCursor != null);
            Debug.Assert(vdbeCursor.pCursor != null);
            Debug.Assert(vdbeCursor.pseudoTableReg == 0);
            Debug.Assert(vdbeCursor.isTable);
            Sqlite3.REGISTER_TRACE(vdbe, recordAddress, pData);

            if (opflags.Has(OpFlag.OPFLAG_NCHANGE))
                vdbe.nChange++;
            if (opflags.Has(OpFlag.OPFLAG_LASTROWID))
                cpu.db.lastRowid = cpu.lastRowid = iKey;

            if (pData.flags.HasFlag(MemFlags.MEM_Null))
            {
                malloc_cs.sqlite3_free(ref pData.zBLOB);
                pData.AsString = null;
                pData.CharacterCount = 0;
            }
            else
            {
                Debug.Assert((pData.flags & (MemFlags.MEM_Blob | MemFlags.MEM_Str)) != 0);
            }


            Insert(iKey, tableName, opflags, vdbe, pData, vdbeCursor);
        }

        private void Insert(long iKey, string tableName, OpFlag opflags, Vdbe vdbe, Mem pData, VdbeCursor vdbeCursor)
        {
            
            var seekResult = opflags.Has(OpFlag.OPFLAG_USESEEKRESULT) ? vdbeCursor.seekResult : ThreeState.Neutral;///Result of prior seek or 0 if no USESEEKRESULT flag 

            int nZero = pData.flags.HasFlag(MemFlags.MEM_Zero) ? pData.u.nZero : 0;///<param name="Number of zero">bytes to append </param>


            var rc = vdbeCursor.pCursor.sqlite3BtreeInsert(null, iKey, pData.zBLOB, pData.CharacterCount, nZero, opflags.Has(OpFlag.OPFLAG_APPEND) ? 1 : 0, seekResult);

            vdbeCursor.rowidIsValid = false;
            vdbeCursor.deferredMoveto = false;
            vdbeCursor.cacheStatus = Sqlite3.CACHE_STALE;

            ///<param name="Invoke the update">hook if required. </param>
            if (rc == SqlResult.SQLITE_OK && cpu.db.xUpdateCallback != null && tableName != null)
            {
                var zDb = cpu.db.Backends[vdbeCursor.iDb].Name;///<param name="database name "> used by the update hook </param>
                var op = ((
                    opflags.Has(OpFlag.OPFLAG_ISUPDATE)
                    ? AuthTarget.SQLITE_UPDATE : AuthTarget.SQLITE_INSERT
                    ));///Opcode for update hook: SQLITE_UPDATE or SQLITE_INSERT 
                Debug.Assert(vdbeCursor.isTable);
                cpu.db.xUpdateCallback(cpu.db.pUpdateArg, op, zDb, tableName, iKey);
                Debug.Assert(vdbeCursor.iDb >= 0);
            }
        }
    }
}
