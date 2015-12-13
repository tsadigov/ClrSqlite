using System;
using System.Diagnostics;
using System.IO;
using i16 = System.Int16;
using u32 = System.UInt32;
using Pgno = System.UInt32;
using u8 = System.Byte;


namespace Community.CsharpSqlite
{
    using Utils;

    namespace Cache
    {

        
   
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
        /// This header file defines the interface that the sqlite page cache
        /// subsystem.
        ///
        ///  Included in SQLite3 port to C#-SQLite;  2008 Noah B Hart
        ///  C#-SQLite is an independent reimplementation of the SQLite software library
        ///
        ///  SQLITE_SOURCE_ID: 2010-08-23 18:52:01 42537b60566f288167f1b5864a5435986838e3a3
        ///
        ///
        ///
        ///</summary>
#if !_PCACHE_H_
        //typedef struct PgHdr PgHdr;
        //typedef struct PCache PCache;
        ///
        ///<summary>
        ///Every page in the cache is controlled by an instance of the following
        ///structure.
        ///
        ///</summary>



        ///
        ///<summary>
        ///Initialize and shutdown the page cache subsystem 
        ///</summary>

        //int sqlite3PcacheInitialize(void);
        //void sqlite3PcacheShutdown(void);
        ///
        ///<summary>
        ///Page cache buffer management:
        ///These routines implement SQLITE_CONFIG_PAGECACHE.
        ///
        ///</summary>

        //void sqlite3PCacheBufferSetup(void *, int sz, int n);
        ///
        ///<summary>
        ///Create a new pager cache.
        ///Under memory stress, invoke xStress to try to make pages clean.
        ///Only clean and unpinned pages can be reclaimed.
        ///
        ///</summary>

        //void sqlite3PcacheOpen(
        //  int szPage,                    /* Size of every page */
        //  int szExtra,                   /* Extra space associated with each page */
        //  int bPurgeable,                /* True if pages are on backing store */
        //  int (*xStress)(void*, PgHdr*), /* Call to try to make pages clean */
        //  void pStress,                 /* Argument to xStress */
        //  PCache pToInit                /* Preallocated space for the PCache */
        //);
        ///
        ///<summary>
        ///</summary>
        ///<param name="Modify the page">size after the cache has been created. </param>

        //void sqlite3PcacheSetPageSize(PCache *, int);
        ///
        ///<summary>
        ///Return the size in bytes of a PCache object.  Used to preallocate
        ///storage space.
        ///
        ///</summary>

        //int sqlite3PcacheSize(void);
        ///
        ///<summary>
        ///One release per successful fetch.  Page is pinned until released.
        ///Reference counted.
        ///
        ///</summary>

        //int sqlite3PcacheFetch(PCache*, Pgno, int createFlag, PgHdr**);
        //void sqlite3PcacheRelease(PgHdr*);
        //void sqlite3PcacheDrop(PgHdr*);         /* Remove page from cache */
        //void sqlite3PcacheMakeDirty(PgHdr*);    /* Make sure page is marked dirty */
        //void sqlite3PcacheMakeClean(PgHdr*);    /* Mark a single page as clean */
        //void sqlite3PcacheCleanAll(PCache*);    /* Mark all dirty list pages as clean */
        ///
        ///<summary>
        ///</summary>
        ///<param name="Change a page number.  Used by incr">vacuum. </param>

        //void sqlite3PcacheMove(PgHdr*, Pgno);
        ///
        ///<summary>
        ///Remove all pages with pgno>x.  Reset the cache if x==0 
        ///</summary>

        //void sqlite3PcacheTruncate(PCache*, Pgno x);
        ///
        ///<summary>
        ///Get a list of all dirty pages in the cache, sorted by page number 
        ///</summary>

        //PgHdr *sqlite3PcacheDirtyList(PCache*);
        ///
        ///<summary>
        ///Reset and close the cache object 
        ///</summary>

        //void sqlite3PcacheClose(PCache*);
        ///
        ///<summary>
        ///Clear flags from pages of the page cache 
        ///</summary>

        //void sqlite3PcacheClearSyncFlags(PCache *);
        ///
        ///<summary>
        ///Discard the contents of the cache 
        ///</summary>

        //void sqlite3PcacheClear(PCache*);
        ///
        ///<summary>
        ///Return the total number of outstanding page references 
        ///</summary>

        //int sqlite3PcacheRefCount(PCache*);
        ///
        ///<summary>
        ///Increment the reference count of an existing page 
        ///</summary>

        //void sqlite3PcacheRef(PgHdr*);
        //int sqlite3PcachePageRefcount(PgHdr*);
        ///
        ///<summary>
        ///Return the total number of pages stored in the cache 
        ///</summary>

        //int sqlite3PcachePagecount(PCache*);
#if SQLITE_CHECK_PAGES
																				/* Iterate through all dirty pages currently stored in the cache. This
** interface is only available if SQLITE_CHECK_PAGES is defined when the
** library is built.
*/

//void sqlite3PcacheIterateDirty(PCache pCache, void (*xIter)(PgHdr *));
#endif
        ///
        ///<summary>
        ///</summary>
        ///<param name="Set and get the suggested cache">cache.</param>
        ///<param name=""></param>
        ///<param name="If no global maximum is configured, then the system attempts to limit">If no global maximum is configured, then the system attempts to limit</param>
        ///<param name="the total number of pages cached by purgeable pager">caches to the sum</param>
        ///<param name="of the suggested cache">sizes.</param>

        //void sqlite3PcacheSetCachesize(PCache *, int);
#if SQLITE_TEST
																				    //int sqlite3PcacheGetCachesize(PCache *);
#endif
#if SQLITE_ENABLE_MEMORY_MANAGEMENT
																				/* Try to return memory used by the pcache module to the main memory heap */
//int sqlite3PcacheReleaseMemory(int);
#endif
#if SQLITE_TEST
																				    //void sqlite3PcacheStats(int*,int*,int*,int*);
#endif
        //void sqlite3PCacheSetDefault(void);
#endif


        
    }
}
