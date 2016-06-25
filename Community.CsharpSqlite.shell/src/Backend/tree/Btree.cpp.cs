using System;
using System.Diagnostics;
using i16 = System.Int16;
using i64 = System.Int64;
using u8 = System.Byte;
using u16 = System.UInt16;
using u32 = System.UInt32;
using u64 = System.UInt64;
using sqlite3_int64 = System.Int64;
using Pgno = System.UInt32;
namespace Community.CsharpSqlite
{
    using DbPage = Cache.PgHdr;
    using System.Text;
    using Metadata;
    using Community.CsharpSqlite.Os;
    using Community.CsharpSqlite.Paging;
    using Utils;


    namespace Tree
    {
        public partial class Btree  
        {

            ///<summary>
            ///Close an open database and invalidate all cursors.
            ///</summary>
            public SqlResult Close()
            {
                BtShared pBt = this.pBt;
                ///Close all cursors opened via this handle.  
                Debug.Assert(this.db.mutex.sqlite3_mutex_held());
                using (this.scope())
                {
                    pBt.pCursor.linkedList().ForEach(
                        pCur => pCur.Close()
                        );
                    
                    ///Rollback any active transaction and free the handle structure.
                    ///The call to sqlite3BtreeRollback() drops any table-locks held by
                    ///this handle.
                    this.sqlite3BtreeRollback();
                }
                ///If there are still other outstanding references to the shared">btree</param>
                ///structure, return now. The remainder of this procedure cleans
                ///btree.
                Debug.Assert(this.wantToLock == 0 && !this.locked);
                if (!this.sharable || pBt.removeFromSharingList())
                {
                    ///The pBt is no longer on the sharing list, so we can access
                    ///it without having to hold the mutex.
                    ///Clean out and delete the BtShared object.
                    Debug.Assert(null == pBt.pCursor);
                    pBt.pPager.sqlite3PagerClose();
                    if (pBt.xFreeSchema != null && pBt.pSchema != null)
                    {
                        pBt.xFreeSchema(pBt.pSchema);
                    }
                    pBt.pSchema = null;
                    // sqlite3DbFree(0, pBt->pSchema);
                    //freeTempSpace(pBt);
                    pBt = null;
                    //malloc_cs.sqlite3_free(ref pBt);
                }
#if !SQLITE_OMIT_SHARED_CACHE
																																																																											Debug.Assert( p.wantToLock==null );
Debug.Assert( p.locked==null );
if( p.pPrev ) p.pPrev.pNext = p.pNext;
if( p.pNext ) p.pNext.pPrev = p.pPrev;
#endif
                //malloc_cs.sqlite3_free(ref p);
                return SqlResult.SQLITE_OK;
            }
        }
    }
}
