using Community.CsharpSqlite.Engine;
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
using Community.CsharpSqlite.Utils;
using Community.CsharpSqlite.Engine.Core.Runtime;

namespace Community.CsharpSqlite.Core.Runtime
{
    public class CrudRuntime
    {
        public CPU cpu { get; set; }
        public CrudRuntime()
        { }
        public CrudRuntime(CPU p)
        {
            this.cpu = p;
        }

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

        public void Insert(VdbeOp pOp)
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
