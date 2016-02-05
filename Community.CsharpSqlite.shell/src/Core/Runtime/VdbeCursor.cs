using Community.CsharpSqlite.Metadata;
using Community.CsharpSqlite.tree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using i64 = System.Int64;
using u8 = System.Byte;
using u16 = System.UInt16;
using u32 = System.UInt32;
using u64 = System.UInt64;
using Pgno = System.UInt32;
using Community.CsharpSqlite.Utils;
using System.Diagnostics;

namespace Community.CsharpSqlite.Engine.Core.Runtime
{
    public class VdbeCursor
    {
        public VdbeCursor()
        {
        }

        public BtCursor pCursor;

        ///
        ///<summary>
        ///The cursor structure of the backend 
        ///</summary>

        public Btree pBt;

        ///
        ///<summary>
        ///Separate file holding temporary table 
        ///</summary>

        public KeyInfo pKeyInfo;

        ///
        ///<summary>
        ///Info about index keys needed by index cursors 
        ///</summary>

        public int iDb;

        ///
        ///<summary>
        ///</summary>
        ///<param name="Index of cursor database in db">1) </param>

        public int pseudoTableReg;

        ///
        ///<summary>
        ///Register holding pseudotable content. 
        ///</summary>

        public int FieldCount;

        ///
        ///<summary>
        ///Number of fields in the header 
        ///</summary>

        public bool zeroed;

        ///
        ///<summary>
        ///True if zeroed out and ready for reuse 
        ///</summary>

        public bool rowidIsValid;

        ///
        ///<summary>
        ///True if lastRowid is valid 
        ///</summary>

        public bool atFirst;

        internal RuntimeException generateNewRowId(i64 lastRowId,Mem pMem, ref long newRowId,ref SqlResult rc)
        {
            var vdbeCursor = this;
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
                    var r = vdbeCursor.getNonRandomNewRowId(out newRowId, ref rc);
                    if (r != RuntimeException.OK)
                        return r;
#if !SQLITE_OMIT_AUTOINCREMENT

                    

                    if (null != pMem)
                    {
                        ///mem(P3) holds an integer 
                        if (pMem.u.AsInteger == MAX_ROWID || vdbeCursor.useRandomRowid)
                        {
                            rc = SqlResult.SQLITE_FULL;///IMP: R-61338
                            return RuntimeException.abort_due_to_error;
                        }
                        pMem.u.AsInteger = newRowId = Math.Max((pMem.u.AsInteger + 1), newRowId);
                    }
#endif
                    vdbeCursor.pCursor.sqlite3BtreeSetCachedRowid(newRowId < MAX_ROWID ? newRowId + 1 : 0);
                }
                if (vdbeCursor.useRandomRowid)//else  --- useRandomRowid may become true 
                {
                    ///IMPLEMENTATION-41881 If the largest ROWID is equal to the
                    ///largest possible integer (9223372036854775807) then the database
                    ///engine starts picking positive candidate ROWIDs at random until
                    ///it finds one that is not previously used. 
                    //Debug.Assert(resultAddress == 0);TODO:out of context
                    pMem = null;
                    ///We cannot be in random rowid mode if this is
                    ///an AUTOINCREMENT table. 
                    ///on the first attempt, simply do one more than previous 
                    newRowId = lastRowid;
                    var r = vdbeCursor.getRandomNewRowId(ref newRowId, ref rc);
                    if (r != RuntimeException.OK)
                        return r;

                }
                vdbeCursor.rowidIsValid = false;
                vdbeCursor.deferredMoveto = false;
                vdbeCursor.cacheStatus = Sqlite3.CACHE_STALE;
            }
            return RuntimeException.OK;
        }

        ///
        ///<summary>
        ///True if pointing to first entry 
        ///</summary>

        public bool useRandomRowid;

        ///
        ///<summary>
        ///</summary>
        ///<param name="Generate new record numbers semi">randomly </param>

        public bool nullRow;

        ///
        ///<summary>
        ///True if pointing to a row with no data 
        ///</summary>

        public bool deferredMoveto;

        ///
        ///<summary>
        ///A call to sqlite3BtreeMoveto() is needed 
        ///</summary>

        public bool isTable;

        ///
        ///<summary>
        ///True if a table requiring integer keys 
        ///</summary>

        public bool isIndex;

        ///
        ///<summary>
        ///</summary>
        ///<param name="True if an index containing keys only "> no data </param>

        public bool isOrdered;

        ///
        ///<summary>
        ///True if the underlying table is BTREE_UNORDERED 
        ///</summary>

#if !SQLITE_OMIT_VIRTUALTABLE
        public sqlite3_vtab_cursor pVtabCursor;

        ///
        ///<summary>
        ///The cursor for a virtual table 
        ///</summary>

        public sqlite3_module pModule;

        ///
        ///<summary>
        ///Module for cursor pVtabCursor 
        ///</summary>

#endif

        ///
        ///<summary>
        ///Sequence counter 
        ///</summary>
        public i64 seqCount;


        ///
        ///<summary>
        ///Argument to the deferred sqlite3BtreeMoveto() 
        ///</summary>
        public i64 movetoTarget;


        ///
        ///<summary>
        ///Last rowid from a Next or NextIdx operation 
        ///</summary>
        public i64 lastRowid;

        ///
        ///<summary>
        ///Result of last sqlite3BtreeMoveto() done by an OP_NotExists or
        ///OP_IsUnique opcode on this cursor. 
        ///</summary>

        public ThreeState seekResult;

        ///
        ///<summary>
        ///Cached information about the header for the data record that the
        ///cursor is currently pointing to.  Only valid if cacheStatus matches
        ///Vdbe.cacheCtr.  Vdbe.cacheCtr will never take on the value of
        ///CACHE_STALE and so setting cacheStatus=CACHE_STALE guarantees that
        ///the cache is out of date.
        ///
        ///aRow might point to (ephemeral) data for the current row, or it might
        ///be NULL.
        ///
        ///</summary>

        public u32 cacheStatus;

        ///
        ///<summary>
        ///Cache is valid if this matches Vdbe.cacheCtr 
        ///</summary>

        public Pgno payloadSize;

        ///
        ///<summary>
        ///Total number of bytes in the record 
        ///</summary>

        public u32[] aType;

        ///
        ///<summary>
        ///Type values for all entries in the record 
        ///</summary>

        public u32[] aOffset;

        ///<summary>
        ///Cached offsets to the start of each columns data
        ///</summary>
        public int aRow;

        ///
        ///<summary>
        ///Pointer to Data for the current row, if all on one page 
        ///</summary>

        public VdbeCursor Copy()
        {
            return (VdbeCursor)MemberwiseClone();
        }

        internal RuntimeException getRandomNewRowId(ref long newRowId, ref SqlResult rc)
        {
            var vdbeCursor = this;
            
            
            newRowId &= (MAX_ROWID >> 1);
            ///ensure doesn't go negative 
            newRowId++;///ensure non-zero 
            var cnt = 0;///Counter to limit the number of searches 
            ThreeState res=ThreeState.Neutral;
            while (((rc = vdbeCursor.pCursor.sqlite3BtreeMovetoUnpacked(null, newRowId, 0, ref res)) == SqlResult.SQLITE_OK) && (res == ThreeState.Neutral) && (++cnt < Globals.TryCountForRandomRowId))
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
            if (rc == SqlResult.SQLITE_OK && res == ThreeState.Neutral)
            {
                rc = SqlResult.SQLITE_FULL;///IMP: R-53002
                return RuntimeException.abort_due_to_error;
            }
            Debug.Assert(newRowId > 0);///EV: R-03570 
            return RuntimeException.OK;
        }



        //---------------
        ///
        ///<summary>
        ///Some compilers complain about constants of the form 0x7fffffffffffffff.
        ///Others complain about 0x7ffffffffffffffffLL.  The following macro seems
        ///to provide the constant while making all compilers happy.
        ///</summary>
        const long MAX_ROWID = i64.MaxValue;

        public RuntimeException getNonRandomNewRowId( out i64 newRowId, ref SqlResult rc)
        {
            VdbeCursor vdbeCursor = this;
            newRowId = vdbeCursor.pCursor.sqlite3BtreeGetCachedRowid();
            ThreeState res = ThreeState.Neutral;
            if (newRowId == 0)
            {
                rc = vdbeCursor.pCursor.sqlite3BtreeLast(ref res);
                if (rc != SqlResult.SQLITE_OK)
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
                    rc = vdbeCursor.pCursor.sqlite3BtreeKeySize(ref newRowId);
                    Debug.Assert(rc == SqlResult.SQLITE_OK);
                    ///Cannot fail following BtreeLast() 
                    if (newRowId == MAX_ROWID)
                        vdbeCursor.useRandomRowid = true;
                    else
                        newRowId++;///IMP: R-34987
                }
            }
            return RuntimeException.OK;
        }
    };

}
