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
                resultAddress:  pOp.p3,
                cursorIndex:    pOp.p1
            );
        }

        ///
        ///<summary>
        ///Some compilers complain about constants of the form 0x7fffffffffffffff.
        ///Others complain about 0x7ffffffffffffffffLL.  The following macro seems
        ///to provide the constant while making all compilers happy.
        ///</summary>
        const long MAX_ROWID = i64.MaxValue;
        
        internal RuntimeException NewRowId(int resultAddress,int cursorIndex)
        {
            var vdbe = cpu.vdbe;
            ///<param name="out2">prerelease </param>
            i64 newRowId = 0;///The new rowid
            
            Debug.Assert(cursorIndex >= 0 && cursorIndex < vdbe.nCursor);
            var vdbeCursor = vdbe.OpenCursors[cursorIndex];///Cursor of table to get the new rowid 
            Debug.Assert(vdbeCursor != null);
            
            RuntimeException exp=vdbeCursor.generateNewRowId(cpu.lastRowid,getMemForNewRowId(resultAddress), ref newRowId,ref cpu.rc);
            if (exp == RuntimeException.OK)
            {
                cpu.pOut.u.AsInteger = (long)newRowId;
            }
            return RuntimeException.OK;
        }

        

        private Mem getMemForNewRowId(int resultAddress) {
            var vdbe = cpu.vdbe;
        
            VdbeFrame rootFrame;///Root frame of VDBE 
            Mem pMem=null;///Register holding largest rowid for AUTOINCREMENT 
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
    }
}
