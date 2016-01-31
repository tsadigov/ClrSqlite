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
            var res = ThreeState.Neutral;///Result of an sqlite3BtreeLast() 
            Debug.Assert(cursorIndex >= 0 && cursorIndex < vdbe.nCursor);
            var vdbeCursor = vdbe.OpenCursors[cursorIndex];///Cursor of table to get the new rowid 
            Debug.Assert(vdbeCursor != null);
            if (Sqlite3.NEVER(vdbeCursor.pCursor == null))
            {
                ///The zero initialization above is all that is needed 
            }
            else
            {
                ///The next rowid or record number (different terms for the same
                ///<param name="thing) is obtained in a two">step algorithm.</param>
                ///<param name=""></param>
                ///<param name="First we attempt to find the largest existing rowid and add one">First we attempt to find the largest existing rowid and add one</param>
                ///<param name="to that.  But if the largest existing rowid is already the maximum">to that.  But if the largest existing rowid is already the maximum</param>
                ///<param name="positive integer, we have to fall through to the second">positive integer, we have to fall through to the second</param>
                ///<param name="probabilistic algorithm">probabilistic algorithm</param>
                ///<param name=""></param>
                ///<param name="The second algorithm is to select a rowid at random and see if">The second algorithm is to select a rowid at random and see if</param>
                ///<param name="it already exists in the table.  If it does not exist, we have">it already exists in the table.  If it does not exist, we have</param>
                ///<param name="succeeded.  If the random rowid does exist, we select a new one">succeeded.  If the random rowid does exist, we select a new one</param>
                ///<param name="and try again, up to 100 times.">and try again, up to 100 times.</param>
                ///<param name=""></param>
                Debug.Assert(vdbeCursor.isTable);
#if SQLITE_32BIT_ROWID
																																																																																																																																																													const int MAX_ROWID = i32.MaxValue;//   define MAX_ROWID 0x7fffffff
#else
                
                // (i64)( (((u64)0x7fffffff)<<32) | (u64)0xffffffff )
#endif
                if (!vdbeCursor.useRandomRowid)
                {
                    var r = getNonRandomNewRowId(vdbeCursor,out newRowId);
                    if (r != RuntimeException.OK)
                        return r;
#if !SQLITE_OMIT_AUTOINCREMENT
                    
                    Mem pMem = getMemForNewRowId(resultAddress);

                    if (null!=pMem)
                    {
                        ///mem(P3) holds an integer 
                        if (pMem.u.AsInteger == MAX_ROWID || vdbeCursor.useRandomRowid)
                        {
                            cpu.rc = SqlResult.SQLITE_FULL;///IMP: R-61338
                            return RuntimeException.abort_due_to_error;
                        }                        
                        pMem.u.AsInteger = newRowId = Math.Max((pMem.u.AsInteger + 1), newRowId) ;
                    }
#endif
                    vdbeCursor.pCursor.sqlite3BtreeSetCachedRowid(newRowId < MAX_ROWID ? newRowId + 1 : 0);
                }
                if (vdbeCursor.useRandomRowid)//else
                {
                    ///IMPLEMENTATION-41881 If the largest ROWID is equal to the
                    ///largest possible integer (9223372036854775807) then the database
                    ///engine starts picking positive candidate ROWIDs at random until
                    ///it finds one that is not previously used. 
                    Debug.Assert(resultAddress == 0);
                    ///We cannot be in random rowid mode if this is
                    ///an AUTOINCREMENT table. 
                    ///on the first attempt, simply do one more than previous 
                    newRowId = cpu.lastRowid;
                    newRowId &= (MAX_ROWID >> 1);
                    ///ensure doesn't go negative 
                    newRowId++;///ensure non-zero 
                    var cnt = 0;///Counter to limit the number of searches 
                    while (((cpu.rc = vdbeCursor.pCursor.sqlite3BtreeMovetoUnpacked(null, newRowId, 0, ref res)) == SqlResult.SQLITE_OK) && (res == ThreeState.Neutral) && (++cnt < Globals.TryCountForRandomRowId))
                    {
                        ///collision- try another random rowid
                        Sqlite3.sqlite3_randomness(sizeof(i64), ref newRowId);
                        if (cnt < Globals.TryCountForSmallRandomRowId)
                        {
                            ///try "small" random rowids for the initial attempts 
                            newRowId &= 0xffffff;
                        }
                        else
                        {
                            newRowId &= (MAX_ROWID >> 1);///ensure doesn't go negative 
                        }
                        newRowId++;///ensure non-zero
                    }
                    if (cpu.rc == SqlResult.SQLITE_OK && res == ThreeState.Neutral)
                    {
                        cpu.rc = SqlResult.SQLITE_FULL;///IMP: R-53002
                        return RuntimeException.abort_due_to_error;
                    }
                    Debug.Assert(newRowId > 0);///EV: R-03570 
                }
                vdbeCursor.rowidIsValid = false;
                vdbeCursor.deferredMoveto = false;
                vdbeCursor.cacheStatus = Sqlite3.CACHE_STALE;
            }
            cpu.pOut.u.AsInteger = (long)newRowId;
            return RuntimeException.OK;
        }

        private RuntimeException getNonRandomNewRowId(VdbeCursor vdbeCursor,out i64 newRowId)
        {
            newRowId = vdbeCursor.pCursor.sqlite3BtreeGetCachedRowid();
            ThreeState res=ThreeState.Neutral;
            if (newRowId == 0)
            {
                cpu.rc = vdbeCursor.pCursor.sqlite3BtreeLast(ref res);
                if (cpu.rc != SqlResult.SQLITE_OK)
                {
                    return RuntimeException.abort_due_to_error;
                }
                if (res != ThreeState.Neutral)
                {
                    newRowId = 1;
                    ///IMP: R-48074 
                }
                else
                {
                    Debug.Assert(vdbeCursor.pCursor.sqlite3BtreeCursorIsValid());
                    cpu.rc = vdbeCursor.pCursor.sqlite3BtreeKeySize(ref newRowId);
                    Debug.Assert(cpu.rc == SqlResult.SQLITE_OK);
                    ///Cannot fail following BtreeLast() 
                    if (newRowId == MAX_ROWID)
                        vdbeCursor.useRandomRowid = true;
                    else
                        newRowId++;///IMP: R-34987
                }
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
