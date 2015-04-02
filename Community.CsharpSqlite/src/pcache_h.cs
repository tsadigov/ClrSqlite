using System;
using System.Diagnostics;
using System.IO;
using i16 = System.Int16;
using u32 = System.UInt32;
using Pgno = System.UInt32;

namespace Community.CsharpSqlite
{
    using MemPage = Sqlite3.MemPage;
    using Pager = Sqlite3.Pager;
    using u8 = Byte;
    using malloc_cs = Sqlite3.malloc_cs;
    using PCache = Sqlite3.PCache;
    using PagerMethods = Sqlite3.PagerMethods;
    public partial class Sqlite3
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
///Bit values for PgHdr.flags 
///</summary>

		//#define PGHDR_DIRTY             0x002  /* Page has changed */
		//#define PGHDR_NEED_SYNC         0x004  /* Fsync the rollback journal before
		//                                       ** writing this page to the database */
		//#define PGHDR_NEED_READ         0x008  /* Content is unread */
		//#define PGHDR_REUSE_UNLIKELY    0x010  /* A hint that reuse is unlikely */
		//#define PGHDR_DONT_WRITE        0x020  /* Do not write content to disk */
		const int PGHDR_DIRTY = 0x002;

		///
///<summary>
///Page has changed 
///</summary>

		const int PGHDR_NEED_SYNC = 0x004;

		///
///<summary>
///Fsync the rollback journal before
///writing this page to the database 
///</summary>

		const int PGHDR_NEED_READ = 0x008;

		///
///<summary>
///Content is unread 
///</summary>

		const int PGHDR_REUSE_UNLIKELY = 0x010;

		///
///<summary>
///A hint that reuse is unlikely 
///</summary>

		const int PGHDR_DONT_WRITE = 0x020;
	///
///<summary>
///Do not write content to disk 
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

		public class PgHdr
		{
			public byte[] pData;

			///
///<summary>
///Content of this page 
///</summary>

			public MemPage pExtra;

			///
///<summary>
///Extra content 
///</summary>

			public PgHdr pDirty;

			///
///<summary>
///Transient list of dirty pages 
///</summary>

			public Pgno pgno;

			///
///<summary>
///The page number for this page 
///</summary>

			public Pager pPager;

			///
///<summary>
///The pager to which this page belongs 
///</summary>

			#if SQLITE_CHECK_PAGES || (SQLITE_DEBUG)
																																																												      public int pageHash;          /* Hash of page content */
#endif
			public int flags;

			///
///<summary>
///PGHDR flags defined below 
///</summary>

			///
///<summary>
///
///Elements above are public.  All that follows is private to pcache.c
///and should not be accessed by other modules.
///
///</summary>

			public int nRef;

			///
///<summary>
///Number of users of this page 
///</summary>

			public PCache pCache;

			///
///<summary>
///Cache that owns this page 
///</summary>

			public bool CacheAllocated;

			///
///<summary>
///True, if allocated from cache 
///</summary>

			public PgHdr pDirtyNext;

			///
///<summary>
///Next element in list of dirty pages 
///</summary>

			public PgHdr pDirtyPrev;

			///
///<summary>
///Previous element in list of dirty pages 
///</summary>

			public Sqlite3.PgHdr1 pPgHdr1;

			///<summary>
			///Cache page header this this page
			///</summary>
			public static implicit operator bool (PgHdr b) {
				return (b != null);
			}

			public void Clear ()
			{
				malloc_cs.sqlite3_free (ref this.pData);
				this.pData = null;
				this.pExtra = null;
				this.pDirty = null;
				this.pgno = 0;
				this.pPager = null;
				#if SQLITE_CHECK_PAGES
																																																																																this.pageHash=0;
#endif
				this.flags = 0;
				this.nRef = 0;
				this.CacheAllocated = false;
				this.pCache = null;
				this.pDirtyNext = null;
				this.pDirtyPrev = null;
				this.pPgHdr1 = null;
			}

			public///<summary>
			/// Return true if it is necessary to write page *pPg into the sub-journal.
			/// A page needs to be written into the sub-journal if there exists one
			/// or more open savepoints for which:
			///
			///   * The page-number is less than or equal to PagerSavepoint.nOrig, and
			///   * The bit corresponding to the page-number is not set in
			///     PagerSavepoint.pInSavepoint.
			///</summary>
			bool subjRequiresPage ()
			{
				u32 pgno = this.pgno;
				Pager pPager = this.pPager;
				int i;
				for (i = 0; i < pPager.nSavepoint; i++) {
					PagerSavepoint p = pPager.aSavepoint [i];
					if (p.nOrig >= pgno && 0 == p.pInSavepoint.sqlite3BitvecTest (pgno)) {
						return true;
					}
				}
				return false;
			}

			public///<summary>
			/// Return true if the page is already in the journal file.
			///
			///</summary>
			bool pageInJournal ()
			{
				return this.pPager.pInJournal.sqlite3BitvecTest(this.pgno) != 0;
			}

			public int pager_pagehash ()
			{
				return 0;
			}

			public void pager_set_pagehash ()
			{
			}

			public MemPage btreePageFromDbPage (Pgno pgno, BtShared pBt)
			{
				MemPage pPage = (MemPage) PagerMethods.sqlite3PagerGetExtra(this);
                pPage.aData = this.sqlite3PagerGetData();
				pPage.pDbPage = this;
				pPage.pBt = pBt;
				pPage.pgno = pgno;
				pPage.hdrOffset = (u8)(pPage.pgno == 1 ? 100 : 0);
				return pPage;
			}

            ///<summary>
            /// Return a pointer to the data for the specified page.
            ///</summary>
            public byte[] sqlite3PagerGetData()
            {
                PgHdr pPg = this;
                Debug.Assert(pPg.nRef > 0 || pPg.pPager.memDb != 0);
                return pPg.pData;
            }


            ///<summary>
            /// Increase the reference count of a supplied page by 1.
            ///
            ///</summary>
            public void sqlite3PcacheRef()
            {
                Debug.Assert(this.nRef > 0);
                this.nRef++;
            }

            ///<summary>
            /// Update the value of the change-counter at offsets 24 and 92 in
            /// the header and the sqlite version number at offset 96.
            ///
            /// This is an unconditional update.  See also the pager_incr_changecounter()
            /// routine which only updates the change-counter if the update is actually
            /// needed, as determined by the pPager.changeCountDone state variable.
            ///
            ///</summary>
            public void pager_write_changecounter()
            {
                PgHdr pPg = this;
                u32 change_counter;
                ///Increment the value just read and write it back to byte 24. 

                change_counter = Converter.sqlite3Get4byte(pPg.pPager.dbFileVers, 0) + 1;
                Converter.put32bits(pPg.pData, 24, change_counter);

                ///Also store the SQLite version number in bytes 96..99 and in
                ///bytes 92..95 store the change counter for which the version number
                Converter.put32bits(pPg.pData, 92, change_counter);
                Converter.put32bits(pPg.pData, 96, Sqlite3.SQLITE_VERSION_NUMBER);
            }

            ///<summary>
            /// Return the number of references to the page supplied as an argument.
            ///
            ///</summary>
            public int sqlite3PcachePageRefcount()
            {
                return this.nRef;
            }

            ///<summary>
            /// Return the number of references to the specified page.
            ///
            ///</summary>
            public int sqlite3PagerPageRefcount()
            {
                return this.sqlite3PcachePageRefcount();
            }

		}
        #endif
}
