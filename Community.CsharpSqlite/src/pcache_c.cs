using System;
using System.Diagnostics;
using System.Text;
using u32 = System.UInt32;
using Pgno = System.UInt32;
using System.Linq;

namespace Community.CsharpSqlite.Cache
{
    using System.Collections.Generic;
    using Utils;
    using sqlite3_value = Engine.Mem;


    ///
    ///<summary>
    ///A complete page cache is an instance of this structure.
    ///
    ///</summary>

    public class PCache: Community.CsharpSqlite.Utils.LinkedList<PgHdr>
    {
        /// <summary>
        /// DirtyListHead
        /// pDirtyTail
        /// </summary>
        

        ///
        ///<summary>
        ///List of dirty pages in LRU order 
        ///</summary>

        public PgHdr pSynced;

        ///
        ///<summary>
        ///Last synced page in dirty page list 
        ///</summary>

        public int _nRef;

        ///
        ///<summary>
        ///Number of referenced pages 
        ///</summary>

        public int nMax;

        ///
        ///<summary>
        ///Configured cache size 
        ///</summary>

        public int szPage;

        ///
        ///<summary>
        ///Size of every page in this cache 
        ///</summary>

        public int szExtra;

        ///
        ///<summary>
        ///Size of extra space for each page 
        ///</summary>

        public bool bPurgeable;

        ///
        ///<summary>
        ///True if pages are on backing store 
        ///</summary>

        public dxStress xStress;

        //int (*xStress)(void*,PgHdr*);       /* Call to try make a page clean */
        public object pStress;

        ///
        ///<summary>
        ///Argument to xStress 
        ///</summary>

        public PCache1 pCache;

        ///
        ///<summary>
        ///Pluggable cache module 
        ///</summary>

        ///<summary>
        ///pPage1
        ///Reference to page 1
        ///</summary>
        public PgHdr FirstPage;

        
        public int nRef ///
        ///<summary>
        ///Number of referenced pages 
        ///</summary>
        {
            get
            {
                return _nRef;
            }
            set
            {
                _nRef = value;
            }
        }

        public void Clear()
        {
            Head = null;
            Tail = null;
            pSynced = null;
            nRef = 0;
        }
    };

   
        ///<summary>
        /// 2008 August 05
        ///
        /// The author disclaims copyright to this source code.  In place of
        /// a legal notice, here is a blessing:
        ///
        ///    May you do good and not evil.
        ///    May you find forgiveness for yourself and forgive others.
        ///    May you share freely, never taking more than you give.
        ///
        ///
        /// This file implements that page cache.
        ///
        ///  Included in SQLite3 port to C#-SQLite;  2008 Noah B Hart
        ///  C#-SQLite is an independent reimplementation of the SQLite software library
        ///
        ///  SQLITE_SOURCE_ID: 2011-06-23 19:49:22 4374b7e83ea0a3fbc3691f9c0c936272862f32f2
        ///
        ///
        ///
        ///</summary>
        //#include "sqliteInt.h"
        
        public static class PCacheMethods
        {
            ///
            ///<summary>
            ///Some of the Debug.Assert() macros in this code are too expensive to run
            ///</summary>
            ///<param name="even during normal debugging.  Use them only rarely on long">running</param>
            ///<param name="tests.  Enable the expensive asserts using the">tests.  Enable the expensive asserts using the</param>
            ///<param name="">time option.</param>
            ///<param name=""></param>

#if SQLITE_ENABLE_EXPENSIVE_ASSERT
																																						// define expensive_assert(X)  Debug.Assert(X)
static void expensive_assert( bool x ) { Debug.Assert( x ); }
#else
            //# define expensive_assert(X)
#endif
            ///<summary>
            /// Linked List Management 
            ///</summary>
#if !NDEBUG &&  SQLITE_ENABLE_EXPENSIVE_ASSERT
																																						/*
** Check that the pCache.pSynced variable is set correctly. If it
** is not, either fail an Debug.Assert or return zero. Otherwise, return
** non-zero. This is only used in debugging builds, as follows:
**
**   expensive_assert( pcacheCheckSynced(pCache) );
*/
static int pcacheCheckSynced(PCache pCache){
PgHdr p ;
for(p=pCache.pDirtyTail; p!=pCache.pSynced; p=p.pDirtyPrev){
Debug.Assert( p.nRef !=0|| (p.flags&PGHDR.NEED_SYNC) !=0);
}
return (p==null || p.nRef!=0 || (p.flags&PGHDR.NEED_SYNC)==0)?1:0;
}
#endif
            ///<summary>
            /// Remove page pPage from the list of dirty pages.
            ///</summary>
            static void pcacheRemoveFromDirtyList(PgHdr pPage)
            {
                PCache p = pPage.pCache;
                Debug.Assert(pPage.pNext != null || pPage == p.Tail);
                Debug.Assert(pPage.pPrev != null || pPage == p.Head);
                ///
                ///<summary>
                ///Update the PCache1.pSynced variable if necessary. 
                ///</summary>

                if (p.pSynced == pPage)
                {
                    PgHdr pSynced = pPage.pPrev;
                    while (pSynced != null && (pSynced.flags & PGHDR.NEED_SYNC) != 0)
                    {
                        pSynced = pSynced.pPrev;
                    }
                    p.pSynced = pSynced;
                }
                if (pPage.pNext != null)
                {
                    pPage.pNext.pPrev = pPage.pPrev;
                }
                else
                {
                    Debug.Assert(pPage == p.Tail);
                    p.Tail = pPage.pPrev;
                }
                if (pPage.pPrev != null)
                {
                    pPage.pPrev.pNext = pPage.pNext;
                }
                else
                {
                    Debug.Assert(pPage == p.Head);
                    p.Head = pPage.pNext;
                }
                pPage.pNext = null;
                pPage.pPrev = null;
#if SQLITE_ENABLE_EXPENSIVE_ASSERT
																																																									expensive_assert( pcacheCheckSynced(p) );
#endif
            }

            ///<summary>
            /// Add page pPage to the head of the dirty list (PCache1.pDirty is set to
            /// pPage).
            ///
            ///</summary>
            static void pcacheAddToDirtyList(PgHdr pPage)
            {
                PCache pCache = pPage.pCache;
            
                pCache.Push(pPage);
           
                if (null == pCache.pSynced && 0 == (pPage.flags & PGHDR.NEED_SYNC))
                {
                    pCache.pSynced = pPage;
                }
#if SQLITE_ENABLE_EXPENSIVE_ASSERT
																																																									expensive_assert( pcacheCheckSynced(p) );
#endif
            }

            ///<summary>
            /// Wrapper around the pluggable caches xUnpin method. If the cache is
            /// being used for an in-memory database, this function is a no-op.
            ///
            ///</summary>
            static void pcacheUnpin(PgHdr p)
            {
                PCache pCache = p.pCache;
                if (pCache.bPurgeable)
                {
                    if (p.pgno == 1)
                    {
                        pCache.FirstPage = null;
                    }
                    sqliteinth.sqlite3GlobalConfig.CacheController.xUnpin(pCache.pCache, p, false);
                }
            }

            ///<summary>
            /// General Interfaces 
            ///
            /// Initialize and shutdown the page cache subsystem. Neither of these
            /// functions are threadsafe.
            ///
            ///</summary>
            public static SqlResult sqlite3PcacheInitialize()
            {
                if (sqliteinth.sqlite3GlobalConfig.CacheController.xInit == null)
                {
                ///<param name="IMPLEMENTATION">64137 If the xInit() method is NULL, then the</param>
                ///<param name="built">in default page cache is used instead of the application defined</param>
                ///<param name="page cache. ">page cache. </param>

                CacheMethods.sqlite3PCacheSetDefault();
                }
                return sqliteinth.sqlite3GlobalConfig.CacheController.xInit(sqliteinth.sqlite3GlobalConfig.CacheController.pArg);
            }

            public static void sqlite3PcacheShutdown()
            {
                if (sqliteinth.sqlite3GlobalConfig.CacheController.xShutdown != null)
                {
                    ///<param name="IMPLEMENTATION">56589 The xShutdown() method may be NULL. </param>

                    sqliteinth.sqlite3GlobalConfig.CacheController.xShutdown(sqliteinth.sqlite3GlobalConfig.CacheController.pArg);
                }
            }

            ///<summary>
            /// Return the size in bytes of a PCache object.
            ///
            ///</summary>
            public static int sqlite3PcacheSize()
            {
                return 4;
            }

            // sizeof( PCache ); }
            ///<summary>
            /// Create a new PCache object. Storage space to hold the object
            /// has already been allocated and is passed in as the p pointer.
            /// The caller discovers how much space needs to be allocated by
            /// calling sqlite3PcacheSize().
            ///
            ///</summary>
            public static void sqlite3PcacheOpen(int szPage, ///
                ///<summary>
                ///Size of every page 
                ///</summary>

            int szExtra, ///
                ///<summary>
                ///Extra space associated with each page 
                ///</summary>

            bool bPurgeable, ///
                ///<summary>
                ///True if pages are on backing store 
                ///</summary>

            dxStress xStress, //int (*xStress)(void*,PgHdr*),/* Call to try to make pages clean */
            object pStress, ///
                ///<summary>
                ///Argument to xStress 
                ///</summary>

            PCache p///
                ///<summary>
                ///Preallocated space for the PCache 
                ///</summary>

            )
            {
                p.Clear();
                //memset(p, 0, sizeof(PCache));
                p.szPage = szPage;
                p.szExtra = szExtra;
                p.bPurgeable = bPurgeable;
                p.xStress = xStress;
                p.pStress = pStress;
                p.nMax = 100;
            }

            ///<summary>
            /// Change the page size for PCache object. The caller must ensure that there
            /// are no outstanding page references when this function is called.
            ///
            ///</summary>
            public static void sqlite3PcacheSetPageSize(PCache pCache, int szPage)
            {
                Debug.Assert(pCache.nRef == 0 && pCache.Head == null);
                if (pCache.pCache != null)
                {
                    sqliteinth.sqlite3GlobalConfig.CacheController.xDestroy(ref pCache.pCache);
                    pCache.pCache = null;
                }
                pCache.szPage = szPage;
            }

            ///<summary>
            /// Try to obtain a page from the cache.
            ///
            ///</summary>
            public static SqlResult sqlite3PcacheFetch(this PCache pCache, ///
                ///<summary>
                ///Obtain the page from this cache 
                ///</summary>

            u32 pgno, ///
                ///<summary>
                ///Page number to obtain 
                ///</summary>

            int createFlag, ///
                ///<summary>
                ///If true, create page if it does not exist already 
                ///</summary>

            ref PgHdr ppPage///
                ///<summary>
                ///Write the page here 
                ///</summary>

            )
            {
                PgHdr pPage = null;
               
                Debug.Assert(pCache != null);
                Debug.Assert(createFlag == 1 || createFlag == 0);
                Debug.Assert(pgno > 0);
                ///If the pluggable cache (sqlite3_pcache*) has not been allocated,
                ///allocate it now.

                if (null == pCache.pCache && createFlag != 0)
                {                    
                    int nByte = pCache.szPage + pCache.szExtra + 0;
                // sizeof( PgHdr );
                    PCache1 p = sqliteinth.sqlite3GlobalConfig.CacheController.xCreate(nByte, pCache.bPurgeable);
                    //if ( null == p )
                    //{
                    //  return SQLITE_NOMEM;
                    //}
                    sqliteinth.sqlite3GlobalConfig.CacheController.xCachesize(p, pCache.nMax);
                    pCache.pCache = p;
                }
                int eCreate = createFlag * (1 + ((!pCache.bPurgeable || null == pCache.Head) ? 1 : 0));
                if (pCache.pCache != null)
                {
                    pPage = sqliteinth.sqlite3GlobalConfig.CacheController.xFetch(pCache.pCache, pgno, eCreate);
                }
                if (null == pPage && eCreate == 1)
                {
                    PgHdr pPg;
                ///Find a dirty page to write">out and recycle. First try to find a</param>
                ///page that does not require a journal">sync (one with PGHDR.NEED_SYNC</param>
                ///cleared), but if that is not possible settle for any other
                ///unreferenced dirty page.

#if SQLITE_ENABLE_EXPENSIVE_ASSERT
																																																																												expensive_assert( pcacheCheckSynced(pCache) );
#endif
                    var dirty_pages = Utils.MyLinqExtensions.path(pCache.pSynced,x=>x.pPrev);
                    pPg = dirty_pages.FirstOrDefault(p=> (p.ReferenceCount != 0 || (p.flags & PGHDR.NEED_SYNC) != 0));
                    
                    pCache.pSynced = pPg;
                    if (null == pPg)
                    {
                        pPg = dirty_pages.FirstOrDefault(p => (p.ReferenceCount != 0 ));
                    }
                    if (pPg != null)
                    {
                        
#if SQLITE_LOG_CACHE_SPILL
																																																																																															      io.sqlite3_log(SQLITE_FULL, 
                  "spill page %d making room for %d - cache used: %d/%d",
                  pPg->pgno, pgno,
                  sqliteinth.sqlite3GlobalConfig.pcache.xPagecount(pCache->pCache),
                  pCache->nMax);
#endif
                        var rc = pCache.xStress(pCache.pStress, pPg);
                        if (rc != SqlResult.SQLITE_OK && rc != SqlResult.SQLITE_BUSY)
                        {
                            return rc;
                        }
                    }
                    pPage = sqliteinth.sqlite3GlobalConfig.CacheController.xFetch(pCache.pCache, pgno, 2);
                }
                if (pPage != null)
                {
                    if (null == pPage.buffer)
                    {
                        //          memset(pPage, 0, sizeof(PgHdr));
                        pPage.buffer = malloc_cs.sqlite3Malloc(pCache.szPage);
                        //          pPage->pData = (void*)&pPage[1];
                        //pPage->pExtra = (void*)&((char*)pPage->pData)[pCache->szPage];
                        //memset(pPage->pExtra, 0, pCache->szExtra);
                        pPage.pCache = pCache;
                        pPage.pgno = pgno;
                    }
                    Debug.Assert(pPage.pCache == pCache);
                    Debug.Assert(pPage.pgno == pgno);
                    //assert(pPage->pData == (void*)&pPage[1]);
                    //assert(pPage->pExtra == (void*)&((char*)&pPage[1])[pCache->szPage]);
                    if (0 == pPage.ReferenceCount)
                    {
                        pCache.nRef++;
                    }
                    pPage.ReferenceCount++;
                    if (pgno == 1)
                    {
                        pCache.FirstPage = pPage;
                    }
                }
                ppPage = pPage;
                return (pPage == null && eCreate != 0) ? SqlResult.SQLITE_NOMEM : SqlResult.SQLITE_OK;
            }

            ///<summary>
            /// Decrement the reference count on a page. If the page is clean and the
            /// reference count drops to 0, then it is made elible for recycling.
            ///</summary>
            public static void sqlite3PcacheRelease(PgHdr p)
            {
                Debug.Assert(p.ReferenceCount > 0);
                p.ReferenceCount--;
                if (p.ReferenceCount == 0)
                {
                    PCache pCache = p.pCache;
                    pCache.nRef--;
                    if ((p.flags & PGHDR.DIRTY) == 0)
                    {
                        pcacheUnpin(p);
                    }
                    else
                    {
                        ///Move the page to the head of the dirty list. 
                        pcacheRemoveFromDirtyList(p);
                        pcacheAddToDirtyList(p);
                    }
                }
            }



            ///<summary>
            /// Drop a page from the cache. There must be exactly one reference to the
            /// page. This function deletes that reference, so after it returns the
            /// page pointed to by p is invalid.
            ///
            ///</summary>
            public static void sqlite3PcacheDrop(PgHdr p)
            {
                PCache pCache;
                Debug.Assert(p.ReferenceCount == 1);
                if ((p.flags & PGHDR.DIRTY) != 0)
                {
                    pcacheRemoveFromDirtyList(p);
                }
                pCache = p.pCache;
                pCache.nRef--;
                if (p.pgno == 1)
                {
                    pCache.FirstPage = null;
                }
                sqliteinth.sqlite3GlobalConfig.CacheController.xUnpin(pCache.pCache, p, true);
            }

            ///<summary>
            /// Make sure the page is marked as dirty. If it isn't dirty already,
            /// make it so.
            ///
            ///</summary>
            public static void sqlite3PcacheMakeDirty(PgHdr p)
            {
                p.flags &= ~PGHDR.DONT_WRITE;
                Debug.Assert(p.ReferenceCount > 0);
                if (0 == (p.flags & PGHDR.DIRTY))
                {
                    p.flags |= PGHDR.DIRTY;
                    pcacheAddToDirtyList(p);
                }
            }

            ///<summary>
            /// Make sure the page is marked as clean. If it isn't clean already,
            /// make it so.
            ///
            ///</summary>
            public static void sqlite3PcacheMakeClean(PgHdr p)
            {
                if ((p.flags & PGHDR.DIRTY) != 0)
                {
                    pcacheRemoveFromDirtyList(p);
                    p.flags &= ~(PGHDR.DIRTY | PGHDR.NEED_SYNC);
                    if (p.ReferenceCount == 0)
                    {
                        pcacheUnpin(p);
                    }
                }
            }

            ///<summary>
            /// Make every page in the cache clean.
            ///
            ///</summary>
            public static void sqlite3PcacheCleanAll(PCache pCache)
            {
                PgHdr p;
                while ((p = pCache.Head) != null)
                {
                    sqlite3PcacheMakeClean(p);
                }
            }

            ///<summary>
            /// Clear the PGHDR.NEED_SYNC flag from all dirty pages.
            ///
            ///</summary>
            public static void sqlite3PcacheClearSyncFlags(PCache pCache)
            {
                pCache.Head.linkedList().ForEach(p=> p.flags &= ~PGHDR.NEED_SYNC);
                
                pCache.pSynced = pCache.Tail;
            }

            ///<summary>
            /// Change the page number of page p to newPgno.
            ///
            ///</summary>
            public static void sqlite3PcacheMove(PgHdr p, Pgno newPgno)
            {
                PCache pCache = p.pCache;
                Debug.Assert(p.ReferenceCount > 0);
                Debug.Assert(newPgno > 0);
                sqliteinth.sqlite3GlobalConfig.CacheController.xRekey(pCache.pCache, p, p.pgno, newPgno);
                p.pgno = newPgno;
                if ((p.flags & PGHDR.DIRTY) != 0 && (p.flags & PGHDR.NEED_SYNC) != 0)
                {
                    pcacheRemoveFromDirtyList(p);
                    pcacheAddToDirtyList(p);
                }
            }

            ///<summary>
            /// Drop every cache entry whose page number is greater than "pgno". The
            /// caller must ensure that there are no outstanding references to any pages
            /// other than page 1 with a page number greater than pgno.
            ///
            /// If there is a reference to page 1 and the pgno parameter passed to this
            /// function is 0, then the data area associated with page 1 is zeroed, but
            /// the page object is not dropped.
            ///
            ///</summary>
            public static void sqlite3PcacheTruncate(PCache pCache, u32 pgno)
            {
                if (pCache.pCache != null)
                {
                    PgHdr p;
                    PgHdr pNext;
                    for (p = pCache.Head; p != null; p = pNext)
                    {
                        pNext = p.pNext;
                        ///This routine never gets call with a positive pgno except right
                        ///after sqlite3PcacheCleanAll().  So if there are dirty pages,
                        ///it must be that pgno==0.

                        Debug.Assert(p.pgno > 0);
                        if (Sqlite3.ALWAYS(p.pgno > pgno))
                        {
                            Debug.Assert((p.flags & PGHDR.DIRTY) != 0);
                            sqlite3PcacheMakeClean(p);
                        }
                    }
                    if (pgno == 0 && pCache.FirstPage != null)
                    {
                        // memset( pCache.pPage1.pData, 0, pCache.szPage );
                        pCache.FirstPage.buffer = malloc_cs.sqlite3Malloc(pCache.szPage);
                        pgno = 1;
                    }
                    sqliteinth.sqlite3GlobalConfig.CacheController.xTruncate(pCache.pCache, pgno + 1);
                }
            }

            ///<summary>
            /// Close a cache.
            ///
            ///</summary>
            public static void sqlite3PcacheClose(PCache pCache)
            {
                if (pCache.pCache != null)
                {
                    sqliteinth.sqlite3GlobalConfig.CacheController.xDestroy(ref pCache.pCache);
                }
            }

            ///<summary>
            /// Discard the contents of the cache.
            ///
            ///</summary>
            public static void sqlite3PcacheClear(PCache pCache)
            {
                sqlite3PcacheTruncate(pCache, 0);
            }

            ///
            ///<summary>
            ///Merge two lists of pages connected by pDirty and in pgno order.
            ///Do not both fixing the pDirtyPrev pointers.
            ///
            ///</summary>

            static PgHdr pcacheMergeDirtyList(PgHdr pA, PgHdr pB)
            {
                PgHdr result = new PgHdr();
                PgHdr pTail = result;
                while (pA != null && pB != null)
                {
                    if (pA.pgno < pB.pgno)
                    {
                        pTail.pDirty = pA;
                        pTail = pA;
                        pA = pA.pDirty;
                    }
                    else
                    {
                        pTail.pDirty = pB;
                        pTail = pB;
                        pB = pB.pDirty;
                    }
                }
                if (pA != null)
                {
                    pTail.pDirty = pA;
                }
                else
                    if (pB != null)
                    {
                        pTail.pDirty = pB;
                    }
                    else
                    {
                        pTail.pDirty = null;
                    }
                return result.pDirty;
            }

            ///<summary>
            /// Sort the list of pages in accending order by pgno.  Pages are
            /// connected by pDirty pointers.  The pDirtyPrev pointers are
            /// corrupted by this sort.
            ///
            /// Since there cannot be more than 2^31 distinct pages in a database,
            /// there cannot be more than 31 buckets required by the merge sorter.
            /// One extra bucket is added to catch overflow in case something
            /// ever changes to make the previous sentence incorrect.
            ///
            ///</summary>
            //#define N_SORT_BUCKET  32
            const int N_SORT_BUCKET = 32;

            static PgHdr pcacheSortDirtyList(PgHdr pIn)
            {
                PgHdr[] a;
                PgHdr p;
                //a[N_SORT_BUCKET], p;
                int i;
                a = new PgHdr[N_SORT_BUCKET];
                //memset(a, 0, sizeof(a));
                while (pIn != null)
                {
                    p = pIn;
                    pIn = p.pDirty;
                    p.pDirty = null;
                    for (i = 0; Sqlite3.ALWAYS(i < N_SORT_BUCKET - 1); i++)
                    {
                        if (a[i] == null)
                        {
                            a[i] = p;
                            break;
                        }
                        else
                        {
                            p = pcacheMergeDirtyList(a[i], p);
                            a[i] = null;
                        }
                    }
                    if (Sqlite3.NEVER(i == N_SORT_BUCKET - 1))
                    {
                        ///
                        ///<summary>
                        ///To get here, there need to be 2^(N_SORT_BUCKET) elements in
                        ///the input list.  But that is impossible.
                        ///
                        ///</summary>

                        a[i] = pcacheMergeDirtyList(a[i], p);
                    }
                }
                p = a[0];
                for (i = 1; i < N_SORT_BUCKET; i++)
                {
                    p = pcacheMergeDirtyList(p, a[i]);
                }
                return p;
            }

            ///<summary>
            /// Return a list of all dirty pages in the cache, sorted by page number.
            ///
            ///</summary>
            public static PgHdr sqlite3PcacheDirtyList(PCache pCache)
            {
                PgHdr p;
                for (p = pCache.Head; p != null; p = p.pNext)
                {
                    p.pDirty = p.pNext;
                }
                return pcacheSortDirtyList(pCache.Head);
            }

            ///<summary>
            /// Return the total number of referenced pages held by the cache.
            ///
            ///</summary>
            public static int sqlite3PcacheRefCount(PCache pCache)
            {
                return pCache.nRef;
            }



            ///<summary>
            /// Return the total number of pages in the cache.
            ///
            ///</summary>
            public static int sqlite3PcachePagecount(PCache pCache)
            {
                int nPage = 0;
                if (pCache.pCache != null)
                {
                    nPage = sqliteinth.sqlite3GlobalConfig.CacheController.xPagecount(pCache.pCache);
                }
                return nPage;
            }

#if SQLITE_TEST
																																						    /*
** Get the suggested cache-size value.
*/
    static int sqlite3PcacheGetCachesize( PCache pCache )
    {
      return pCache.nMax;
    }
#endif
            ///
            ///<summary>
            ///</summary>
            ///<param name="Set the suggested cache">size value.</param>

            public static void sqlite3PcacheSetCachesize(PCache pCache, int mxPage)
            {
                pCache.nMax = mxPage;
                if (pCache.pCache != null)
                {
                    sqliteinth.sqlite3GlobalConfig.CacheController.xCachesize(pCache.pCache, mxPage);
                }
            }
#if SQLITE_CHECK_PAGES  || (SQLITE_DEBUG)
																			    /*
** For all dirty pages currently in the cache, invoke the specified
** callback. This is only used if the SQLITE_CHECK_PAGES macro is
** defined.
*/
    static void sqlite3PcacheIterateDirty( PCache pCache, dxIter xIter )
    {
      PgHdr pDirty;
      for ( pDirty = pCache.pDirty; pDirty != null; pDirty = pDirty.pDirtyNext )
      {
        xIter( pDirty );
      }
    }
#endif
        }
    
}
