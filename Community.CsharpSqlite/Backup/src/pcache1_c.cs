using System.Diagnostics;
using u32 = System.UInt32;
using Pgno = System.UInt32;

namespace Community.CsharpSqlite
{
	using sqlite3_pcache = Sqlite3.PCache1;

	public partial class Sqlite3
	{
		///<summary>
		/// 2008 November 05
		///
		/// The author disclaims copyright to this source code.  In place of
		/// a legal notice, here is a blessing:
		///
		///    May you do good and not evil.
		///    May you find forgiveness for yourself and forgive others.
		///    May you share freely, never taking more than you give.
		///
		///
		///
		/// This file implements the default page cache implementation (the
		/// sqlite3_pcache interface). It also contains part of the implementation
		/// of the SQLITE_CONFIG_PAGECACHE and sqlite3_release_memory() features.
		/// If the default page cache implementation is overriden, then neither of
		/// these two features are available.
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
		//typedef struct PCache1 PCache1;
		//typedef struct PgHdr1 PgHdr1;
		//typedef struct PgFreeslot PgFreeslot;
		//typedef struct PGroup PGroup;
		///<summary>
		///Each page cache (or PCache) belongs to a PGroup.  A PGroup is a set
		/// of one or more PCaches that are able to recycle each others unpinned
		/// pages when they are under memory pressure.  A PGroup is an instance of
		/// the following object.
		///
		/// This page cache implementation works in one of two modes:
		///
		///   (1)  Every PCache is the sole member of its own PGroup.  There is
		///        one PGroup per PCache.
		///
		///   (2)  There is a single global PGroup that all PCaches are a member
		///        of.
		///
		/// Mode 1 uses more memory (since PCache instances are not able to rob
		/// unused pages from other PCaches) but it also operates without a mutex,
		/// and is therefore often faster.  Mode 2 requires a mutex in order to be
		/// threadsafe, but is able recycle pages more efficient.
		///
		/// For mode (1), PGroup.mutex is NULL.  For mode (2) there is only a single
		/// PGroup which is the pcache1.grp global variable and its mutex is
		/// SQLITE_MUTEX_STATIC_LRU.
		///
		///</summary>
		public class PGroup
		{
			public sqlite3_mutex mutex;

			///
///<summary>
///MUTEX_STATIC_LRU or NULL 
///</summary>

			public int nMaxPage;

			///
///<summary>
///Sum of nMax for purgeable caches 
///</summary>

			public int nMinPage;

			///
///<summary>
///Sum of nMin for purgeable caches 
///</summary>

			public int mxPinned;

			///
///<summary>
///</summary>
///<param name="nMaxpage + 10 "> nMinPage </param>

			public int nCurrentPage;

			///
///<summary>
///Number of purgeable pages allocated 
///</summary>

			public PgHdr1 pLruHead, pLruTail;

			///
///<summary>
///LRU list of unpinned pages 
///</summary>

			// C#
			public PGroup ()
			{
				mutex = new sqlite3_mutex ();
			}
		};


		///<summary>
		///Each page cache is an instance of the following object.  Every
		/// open database file (including each in-memory database and each
		/// temporary or transient database) has a single page cache which
		/// is an instance of this object.
		///
		/// Pointers to structures of this type are cast and returned as
		/// opaque sqlite3_pcache* handles.
		///
		///</summary>
		public class PCache1
		{
			///
///<summary>
///Cache configuration parameters. Page size (szPage) and the purgeable
///flag (bPurgeable) are set when the cache is created. nMax may be 
///modified at any time by a call to the pcache1CacheSize() method.
///The PGroup mutex must be held when accessing nMax.
///
///</summary>

			public PGroup pGroup;

			///
///<summary>
///PGroup this cache belongs to 
///</summary>

			public int szPage;

			///
///<summary>
///Size of allocated pages in bytes 
///</summary>

			public bool bPurgeable;

			///
///<summary>
///True if cache is purgeable 
///</summary>

			public int nMin;

			///
///<summary>
///Minimum number of pages reserved 
///</summary>

			public int nMax;

			///
///<summary>
///Configured "cache_size" value 
///</summary>

			public int n90pct;

			///
///<summary>
///nMax*9/10 
///</summary>

			///
///<summary>
///Hash table of all pages. The following variables may only be accessed
///when the accessor is holding the PGroup mutex.
///
///</summary>

			public int nRecyclable;

			///
///<summary>
///Number of pages in the LRU list 
///</summary>

			public int nPage;

			///
///<summary>
///Total number of pages in apHash 
///</summary>

			public int nHash;

			///
///<summary>
///Number of slots in apHash[] 
///</summary>

			public PgHdr1[] apHash;

			///<summary>
			///Hash table for fast lookup by key
			///</summary>
			public Pgno iMaxKey;

			///
///<summary>
///Largest key seen since xTruncate() 
///</summary>

			public void Clear ()
			{
				nRecyclable = 0;
				nPage = 0;
				nHash = 0;
				apHash = null;
				iMaxKey = 0;
			}
		};


		///<summary>
		/// Each cache entry is represented by an instance of the following
		/// structure. A buffer of PgHdr1.pCache.szPage bytes is allocated
		/// directly before this structure in memory (see the PGHDR1_TO_PAGE()
		/// macro below).
		///
		///</summary>
		public class PgHdr1
		{
			public Pgno iKey;

			///
///<summary>
///Key value (page number) 
///</summary>

			public PgHdr1 pNext;

			///
///<summary>
///Next in hash table chain 
///</summary>

			public PCache1 pCache;

			///
///<summary>
///Cache that currently owns this page 
///</summary>

			public PgHdr1 pLruNext;

			///
///<summary>
///Next in LRU list of unpinned pages 
///</summary>

			public PgHdr1 pLruPrev;

			///<summary>
			///Previous in LRU list of unpinned pages
			///</summary>
			// For C#
			public PgHdr pPgHdr = new PgHdr ();

			///
///<summary>
///Pointer to Actual Page Header 
///</summary>

			public void Clear ()
			{
				this.iKey = 0;
				this.pNext = null;
				this.pCache = null;
				this.pPgHdr.Clear ();
			}
		};


		///<summary>
		/// Free slots in the allocator used to divide up the buffer provided using
		/// the SQLITE_CONFIG_PAGECACHE mechanism.
		///
		///</summary>
		public class PgFreeslot
		{
			public PgFreeslot pNext;

			///
///<summary>
///Next free slot 
///</summary>

			public PgHdr _PgHdr;
		///
///<summary>
///Next Free Header 
///</summary>

		};


		///
///<summary>
///Global data used by this cache.
///
///</summary>

		public class PCacheGlobal
		{
			public PGroup grp;

			///
///<summary>
///The global PGroup for mode (2) 
///</summary>

			///
///<summary>
///Variables related to SQLITE_CONFIG_PAGECACHE settings.  The
///szSlot, nSlot, pStart, pEnd, nReserve, and isInit values are all
///fixed at sqlite3_initialize() time and do not require mutex protection.
///The nFreeSlot and pFree values do require mutex protection.
///
///</summary>

			public bool isInit;

			///
///<summary>
///True if initialized 
///</summary>

			public int szSlot;

			///
///<summary>
///Size of each free slot 
///</summary>

			public int nSlot;

			///
///<summary>
///The number of pcache slots 
///</summary>

			public int nReserve;

			///
///<summary>
///Try to keep nFreeSlot above this 
///</summary>

			public object pStart, pEnd;

			///
///<summary>
///Bounds of pagecache malloc range 
///</summary>

			///
///<summary>
///Above requires no mutex.  Use mutex below for variable that follow. 
///</summary>

			public sqlite3_mutex mutex;

			///
///<summary>
///Mutex for accessing the following: 
///</summary>

			public int nFreeSlot;

			///
///<summary>
///Number of unused pcache slots 
///</summary>

			public PgFreeslot pFree;

			///
///<summary>
///Free page blocks 
///</summary>

			///
///<summary>
///The following value requires a mutex to change.  We skip the mutex on
///</summary>
///<param name="reading because (1) most platforms read a 32">bit integer atomically and</param>
///<param name="(2) even if an incorrect value is read, no great harm is done since this">(2) even if an incorrect value is read, no great harm is done since this</param>
///<param name="is really just an optimization. ">is really just an optimization. </param>

			public bool bUnderPressure;

			///
///<summary>
///True if low on PAGECACHE memory 
///</summary>

			// C#
			public PCacheGlobal ()
			{
				grp = new PGroup ();
			}
		}

		static PCacheGlobal pcache = new PCacheGlobal ();

		///
///<summary>
///All code in this file should access the global structure above via the
///alias "pcache1". This ensures that the WSD emulation is used when
///compiling for systems that do not support real WSD.
///
///</summary>

		//#define pcache1 (GLOBAL(struct PCacheGlobal, pcache1_g))
		static PCacheGlobal pcache1 = pcache;

		///<summary>
		/// When a PgHdr1 structure is allocated, the associated PCache1.szPage
		/// bytes of data are located directly before it in memory (i.e. the total
		/// size of the allocation is sizeof(PgHdr1)+PCache1.szPage byte). The
		/// PGHDR1_TO_PAGE() macro takes a pointer to a PgHdr1 structure as
		/// an argument and returns a pointer to the associated block of szPage
		/// bytes. The PAGE_TO_PGHDR1() macro does the opposite: its argument is
		/// a pointer to a block of szPage bytes of data and the return value is
		/// a pointer to the associated PgHdr1 structure.
		///
		///   Debug.Assert( PGHDR1_TO_PAGE(PAGE_TO_PGHDR1(pCache, X))==X );
		///
		///</summary>
		//#define PGHDR1_TO_PAGE(p)    (void)(((char)p) - p.pCache.szPage)
		static PgHdr PGHDR1_TO_PAGE (PgHdr1 p)
		{
			return p.pPgHdr;
		}

		//#define PAGE_TO_PGHDR1(c, p) (PgHdr1)(((char)p) + c.szPage)
		static PgHdr1 PAGE_TO_PGHDR1 (PCache1 c, PgHdr p)
		{
			return p.pPgHdr1;
		}

		///<summary>
		/// Macros to enter and leave the PCache LRU mutex.
		///
		///</summary>
		//#define pcache1EnterMutex(X) sqlite3_mutex_enter((X).mutex)
		static void pcache1EnterMutex (PGroup X)
		{
			sqlite3_mutex_enter (X.mutex);
		}

		//#define pcache1LeaveMutex(X) sqlite3_mutex_leave((X).mutex)
		static void pcache1LeaveMutex (PGroup X)
		{
			sqlite3_mutex_leave (X.mutex);
		}

		///
///<summary>
///
///</summary>

		///<summary>
		/// Page Allocation/SQLITE_CONFIG_PCACHE Related Functions 
		///</summary>
		///<summary>
		/// This function is called during initialization if a static buffer is
		/// supplied to use for the page-cache by passing the SQLITE_CONFIG_PAGECACHE
		/// verb to sqlite3_config(). Parameter pBuf points to an allocation large
		/// enough to contain 'n' buffers of 'sz' bytes each.
		///
		/// This routine is called from sqlite3_initialize() and so it is guaranteed
		/// to be serialized already.  There is no need for further mutexing.
		///
		///</summary>
		static void sqlite3PCacheBufferSetup (object pBuf, int sz, int n)
		{
			if (pcache1.isInit) {
				PgFreeslot p;
				sz = ROUNDDOWN8 (sz);
				pcache1.szSlot = sz;
				pcache1.nSlot = pcache1.nFreeSlot = n;
				pcache1.nReserve = n > 90 ? 10 : (n / 10 + 1);
				pcache1.pStart = null;
				pcache1.pEnd = null;
				pcache1.pFree = null;
				pcache1.bUnderPressure = false;
				while (n-- > 0) {
					p = new PgFreeslot ();
					// (PgFreeslot)pBuf;
					p._PgHdr = new PgHdr ();
					p.pNext = pcache1.pFree;
					pcache1.pFree = p;
					//pBuf = (void)&((char)pBuf)[sz];
				}
				pcache1.pEnd = pBuf;
			}
		}

		///<summary>
		/// Malloc function used within this file to allocate space from the buffer
		/// configured using sqlite3_config(SQLITE_CONFIG_PAGECACHE) option. If no
		/// such buffer exists or there is no space left in it, this function falls
		/// back to sqlite3Malloc().
		///
		/// Multiple threads can run this routine at the same time.  Global variables
		/// in pcache1 need to be protected via mutex.
		///
		///</summary>
		static PgHdr pcache1Alloc (int nByte)
		{
			PgHdr p = null;
			Debug.Assert (sqlite3_mutex_notheld (pcache1.grp.mutex));
			sqlite3StatusSet (SQLITE_STATUS_PAGECACHE_SIZE, nByte);
			if (nByte <= pcache1.szSlot) {
				sqlite3_mutex_enter (pcache1.mutex);
				p = pcache1.pFree._PgHdr;
				if (p != null) {
					pcache1.pFree = pcache1.pFree.pNext;
					pcache1.nFreeSlot--;
					pcache1.bUnderPressure = pcache1.nFreeSlot < pcache1.nReserve;
					Debug.Assert (pcache1.nFreeSlot >= 0);
					sqlite3StatusAdd (SQLITE_STATUS_PAGECACHE_USED, 1);
				}
				sqlite3_mutex_leave (pcache1.mutex);
			}
			if (p == null) {
				///
///<summary>
///Memory is not available in the SQLITE_CONFIG_PAGECACHE pool.  Get
///it from sqlite3Malloc instead.
///
///</summary>

				p = new PgHdr ();
				// sqlite3Malloc( nByte );
				//if ( p != null )
				{
					int sz = nByte;
					//sqlite3MallocSize( p );
					sqlite3_mutex_enter (pcache1.mutex);
					sqlite3StatusAdd (SQLITE_STATUS_PAGECACHE_OVERFLOW, sz);
					sqlite3_mutex_leave (pcache1.mutex);
				}
				sqlite3MemdebugSetType (p, MemType.PCACHE);
			}
			return p;
		}

		///<summary>
		/// Free an allocated buffer obtained from pcache1Alloc().
		///
		///</summary>
		static void pcache1Free (ref PgHdr p)
		{
			if (p == null)
				return;
			if (p.CacheAllocated)//if ( p >= pcache1.pStart && p < pcache1.pEnd )
			 {
				PgFreeslot pSlot = new PgFreeslot ();
				sqlite3_mutex_enter (pcache1.mutex);
				sqlite3StatusAdd (SQLITE_STATUS_PAGECACHE_USED, -1);
				pSlot._PgHdr = p;
				// pSlot = (PgFreeslot)p;
				pSlot.pNext = pcache1.pFree;
				pcache1.pFree = pSlot;
				pcache1.nFreeSlot++;
				pcache1.bUnderPressure = pcache1.nFreeSlot < pcache1.nReserve;
				Debug.Assert (pcache1.nFreeSlot <= pcache1.nSlot);
				sqlite3_mutex_leave (pcache1.mutex);
			}
			else {
				int iSize;
				Debug.Assert (sqlite3MemdebugHasType (p, MemType.PCACHE));
				sqlite3MemdebugSetType (p, MemType.HEAP);
				iSize = sqlite3MallocSize (p.pData);
				sqlite3_mutex_enter (pcache1.mutex);
				sqlite3StatusAdd (SQLITE_STATUS_PAGECACHE_OVERFLOW, -iSize);
				sqlite3_mutex_leave (pcache1.mutex);
				sqlite3_free (ref p.pData);
			}
		}

		#if SQLITE_ENABLE_MEMORY_MANAGEMENT
																																						/*
** Return the size of a pcache allocation
*/
static int pcache1MemSize(object p){
  if( p>=pcache1.pStart && p<pcache1.pEnd ){
    return pcache1.szSlot;
  }else{
    int iSize;
    Debug.Assert( sqlite3MemdebugHasType(p, MemType.PCACHE) );
    sqlite3MemdebugSetType(p, MemType.HEAP);
    iSize = sqlite3MallocSize(p);
    sqlite3MemdebugSetType(p, MemType.PCACHE);
    return iSize;
  }
}
#endif
		///<summary>
		/// Allocate a new page object initially associated with cache pCache.
		///</summary>
		static PgHdr1 pcache1AllocPage (PCache1 pCache)
		{
			//int nByte = sizeof( PgHdr1 ) + pCache.szPage;
			PgHdr pPg = pcache1Alloc (pCache.szPage);
			//nByte );
			PgHdr1 p = null;
			//if ( pPg !=null)
			{
				//PAGE_TO_PGHDR1( pCache, pPg );
				p = new PgHdr1 ();
				p.pCache = pCache;
				p.pPgHdr = pPg;
				if (pCache.bPurgeable) {
					pCache.pGroup.nCurrentPage++;
				}
				//else
				//{
				//  p = 0;
				//}
			}
			return p;
		}

		///<summary>
		/// Free a page object allocated by pcache1AllocPage().
		///
		/// The pointer is allowed to be NULL, which is prudent.  But it turns out
		/// that the current implementation happens to never call this routine
		/// with a NULL pointer, so we mark the NULL test with ALWAYS().
		///
		///</summary>
		static void pcache1FreePage (ref PgHdr1 p)
		{
			if (ALWAYS (p)) {
				PCache1 pCache = p.pCache;
				if (pCache.bPurgeable) {
					pCache.pGroup.nCurrentPage--;
				}
				pcache1Free (ref p.pPgHdr);
				//PGHDR1_TO_PAGE( p );
			}
		}

		///<summary>
		/// Malloc function used by SQLite to obtain space from the buffer configured
		/// using sqlite3_config(SQLITE_CONFIG_PAGECACHE) option. If no such buffer
		/// exists, this function falls back to sqlite3Malloc().
		///
		///</summary>
		static PgHdr sqlite3PageMalloc (int sz)
		{
			return pcache1Alloc (sz);
		}

		///<summary>
		/// Free an allocated buffer obtained from sqlite3PageMalloc().
		///
		///</summary>
		static void sqlite3PageFree (ref byte[] p)
		{
			if (p != null) {
				sqlite3_free (ref p);
				p = null;
			}
		}

		static void sqlite3PageFree (ref PgHdr p)
		{
			pcache1Free (ref p);
		}

		///
///<summary>
///Return true if it desirable to avoid allocating a new page cache
///entry.
///
///If memory was allocated specifically to the page cache using
///SQLITE_CONFIG_PAGECACHE but that memory has all been used, then
///it is desirable to avoid allocating a new page cache entry because
///presumably SQLITE_CONFIG_PAGECACHE was suppose to be sufficient
///for all page cache needs and we should not need to spill the
///allocation onto the heap.
///
///Or, the heap is used for all page cache memory put the heap is
///under memory pressure, then again it is desirable to avoid
///allocating a new page cache entry in order to avoid stressing
///the heap even further.
///
///</summary>

		static bool pcache1UnderMemoryPressure (PCache1 pCache)
		{
			if (pcache1.nSlot != 0 && pCache.szPage <= pcache1.szSlot) {
				return pcache1.bUnderPressure;
			}
			else {
				return sqlite3HeapNearlyFull ();
			}
		}

		///
///<summary>
///
///</summary>

		///<summary>
		/// General Implementation Functions 
		///</summary>
		///<summary>
		/// This function is used to resize the hash table used by the cache passed
		/// as the first argument.
		///
		/// The PCache mutex must be held when this function is called.
		///
		///</summary>
		static int pcache1ResizeHash (PCache1 p)
		{
			PgHdr1[] apNew;
			int nNew;
			int i;
			Debug.Assert (sqlite3_mutex_held (p.pGroup.mutex));
			nNew = p.nHash * 2;
			if (nNew < 256) {
				nNew = 256;
			}
			pcache1LeaveMutex (p.pGroup);
			if (p.nHash != 0) {
				sqlite3BeginBenignMalloc ();
			}
			apNew = new PgHdr1[nNew];
			//(PgHdr1 *)sqlite3_malloc(sizeof(PgHdr1 )*nNew);
			if (p.nHash != 0) {
				sqlite3EndBenignMalloc ();
			}
			pcache1EnterMutex (p.pGroup);
			if (apNew != null) {
				//memset(apNew, 0, sizeof(PgHdr1 )*nNew);
				for (i = 0; i < p.nHash; i++) {
					PgHdr1 pPage;
					PgHdr1 pNext = p.apHash [i];
					while ((pPage = pNext) != null) {
						Pgno h = (Pgno)(pPage.iKey % nNew);
						pNext = pPage.pNext;
						pPage.pNext = apNew [h];
						apNew [h] = pPage;
					}
				}
				//sqlite3_free( p.apHash );
				p.apHash = apNew;
				p.nHash = nNew;
			}
			return (p.apHash != null ? SQLITE_OK : SQLITE_NOMEM);
		}

		///<summary>
		/// This function is used internally to remove the page pPage from the
		/// PGroup LRU list, if is part of it. If pPage is not part of the PGroup
		/// LRU list, then this function is a no-op.
		///
		/// The PGroup mutex must be held when this function is called.
		///
		/// If pPage is NULL then this routine is a no-op.
		///
		///</summary>
		static void pcache1PinPage (PgHdr1 pPage)
		{
			PCache1 pCache;
			PGroup pGroup;
			if (pPage == null)
				return;
			pCache = pPage.pCache;
			pGroup = pCache.pGroup;
			Debug.Assert (sqlite3_mutex_held (pGroup.mutex));
			if (pPage.pLruNext != null || pPage == pGroup.pLruTail) {
				if (pPage.pLruPrev != null) {
					pPage.pLruPrev.pLruNext = pPage.pLruNext;
				}
				if (pPage.pLruNext != null) {
					pPage.pLruNext.pLruPrev = pPage.pLruPrev;
				}
				if (pGroup.pLruHead == pPage) {
					pGroup.pLruHead = pPage.pLruNext;
				}
				if (pGroup.pLruTail == pPage) {
					pGroup.pLruTail = pPage.pLruPrev;
				}
				pPage.pLruNext = null;
				pPage.pLruPrev = null;
				pPage.pCache.nRecyclable--;
			}
		}

		///<summary>
		/// Remove the page supplied as an argument from the hash table
		/// (PCache1.apHash structure) that it is currently stored in.
		///
		/// The PGroup mutex must be held when this function is called.
		///
		///</summary>
		static void pcache1RemoveFromHash (PgHdr1 pPage)
		{
			int h;
			PCache1 pCache = pPage.pCache;
			PgHdr1 pp;
			PgHdr1 pPrev = null;
			Debug.Assert (sqlite3_mutex_held (pCache.pGroup.mutex));
			h = (int)(pPage.iKey % pCache.nHash);
			for (pp = pCache.apHash [h]; pp != pPage; pPrev = pp, pp = pp.pNext)
				;
			if (pPrev == null)
				pCache.apHash [h] = pp.pNext;
			else
				pPrev.pNext = pp.pNext;
			// pCache.apHash[h] = pp.pNext;
			pCache.nPage--;
		}

		///<summary>
		/// If there are currently more than nMaxPage pages allocated, try
		/// to recycle pages to reduce the number allocated to nMaxPage.
		///
		///</summary>
		static void pcache1EnforceMaxPage (PGroup pGroup)
		{
			Debug.Assert (sqlite3_mutex_held (pGroup.mutex));
			while (pGroup.nCurrentPage > pGroup.nMaxPage && pGroup.pLruTail != null) {
				PgHdr1 p = pGroup.pLruTail;
				Debug.Assert (p.pCache.pGroup == pGroup);
				pcache1PinPage (p);
				pcache1RemoveFromHash (p);
				pcache1FreePage (ref p);
			}
		}

		///
///<summary>
///Discard all pages from cache pCache with a page number (key value) 
///greater than or equal to iLimit. Any pinned pages that meet this 
///criteria are unpinned before they are discarded.
///
///The PCache mutex must be held when this function is called.
///
///</summary>

		static void pcache1TruncateUnsafe (PCache1 pCache, ///
///<summary>
///The cache to truncate 
///</summary>

		uint iLimit///
///<summary>
///Drop pages with this pgno or larger 
///</summary>

		)
		{
			#if !NDEBUG || SQLITE_COVERAGE_TEST
																																																									      uint nPage = 0;
#endif
			uint h;
			Debug.Assert (sqlite3_mutex_held (pCache.pGroup.mutex));
			for (h = 0; h < pCache.nHash; h++) {
				PgHdr1 pPrev = null;
				PgHdr1 pp = pCache.apHash [h];
				PgHdr1 pPage;
				while ((pPage = pp) != null) {
					if (pPage.iKey >= iLimit) {
						pCache.nPage--;
						pp = pPage.pNext;
						pcache1PinPage (pPage);
						if (pCache.apHash [h] == pPage)
							pCache.apHash [h] = pPage.pNext;
						else
							pPrev.pNext = pp;
						pcache1FreePage (ref pPage);
					}
					else {
						pp = pPage.pNext;
						#if !NDEBUG || SQLITE_COVERAGE_TEST
																																																																																																																		            nPage++;
#endif
					}
					pPrev = pPage;
				}
			}
			#if !NDEBUG || SQLITE_COVERAGE_TEST
																																																									      Debug.Assert( pCache.nPage == nPage );
#endif
		}

		///
///<summary>
///
///</summary>

		///<summary>
		/// sqlite3_pcache Methods 
		///</summary>
		///<summary>
		/// Implementation of the sqlite3_pcache.xInit method.
		///
		///</summary>
		static int pcache1Init<T> (T NotUsed)
		{
			UNUSED_PARAMETER (NotUsed);
			Debug.Assert (pcache1.isInit == false);
			pcache1 = new PCacheGlobal ();
			//memset(&pcache1, 0, sizeof(pcache1));
			if (sqlite3GlobalConfig.bCoreMutex) {
				pcache1.grp.mutex = sqlite3_mutex_alloc (SQLITE_MUTEX_STATIC_LRU);
				pcache1.mutex = sqlite3_mutex_alloc (SQLITE_MUTEX_STATIC_PMEM);
			}
			pcache1.grp.mxPinned = 10;
			pcache1.isInit = true;
			return SQLITE_OK;
		}

		///<summary>
		/// Implementation of the sqlite3_pcache.xShutdown method.
		/// Note that the static mutex allocated in xInit does
		/// not need to be freed.
		///
		///</summary>
		static void pcache1Shutdown<T> (T NotUsed)
		{
			UNUSED_PARAMETER (NotUsed);
			Debug.Assert (pcache1.isInit);
			pcache1 = new PCacheGlobal ();
			//;memset( &pcache1, 0, sizeof( pcache1 ) );
		}

		///<summary>
		/// Implementation of the sqlite3_pcache.xCreate method.
		///
		/// Allocate a new cache.
		///
		///</summary>
		static sqlite3_pcache pcache1Create (int szPage, bool bPurgeable)
		{
			PCache1 pCache;
			///
///<summary>
///The newly created page cache 
///</summary>

			PGroup pGroup;
			///
///<summary>
///The group the new page cache will belong to 
///</summary>

			int sz;
			///
///<summary>
///Bytes of memory required to allocate the new cache 
///</summary>

			///
///<summary>
///The seperateCache variable is true if each PCache has its own private
///PGroup.  In other words, separateCache is true for mode (1) where no
///mutexing is required.
///
///</summary>
///<param name="Always use a unified cache (mode">2) if ENABLE_MEMORY_MANAGEMENT</param>
///<param name=""></param>
///<param name="Always use a unified cache in single">threaded applications</param>
///<param name=""></param>
///<param name="Otherwise (if multi">threaded and ENABLE_MEMORY_MANAGEMENT is off)</param>
///<param name="use separate caches (mode">1)</param>
///<param name=""></param>

			#if (SQLITE_ENABLE_MEMORY_MANAGEMENT) || !SQLITE_THREADSAF
			const int separateCache = 0;
			#else
																																																									  int separateCache = sqlite3GlobalConfig.bCoreMutex>0;
#endif
			//sz = sizeof( PCache1 ) + sizeof( PGroup ) * separateCache;
			pCache = new PCache1 ();
			//(PCache1)sqlite3_malloc( sz );
			//if ( pCache != null )
			{
				//memset( pCache, 0, sz );
				if (separateCache != 0) {
					//pGroup = new PGroup();//(PGroup)pCache[1];
					//pGroup.mxPinned = 10;
				}
				else {
					pGroup = pcache1.grp;
				}
				pCache.pGroup = pGroup;
				pCache.szPage = szPage;
				pCache.bPurgeable = bPurgeable;
				//( bPurgeable ? 1 : 0 );
				if (bPurgeable) {
					pCache.nMin = 10;
					pcache1EnterMutex (pGroup);
					pGroup.nMinPage += (int)pCache.nMin;
					pGroup.mxPinned = pGroup.nMaxPage + 10 - pGroup.nMinPage;
					pcache1LeaveMutex (pGroup);
				}
			}
			return (sqlite3_pcache)pCache;
		}

		///<summary>
		/// Implementation of the sqlite3_pcache.xCachesize method.
		///
		/// Configure the cache_size limit for a cache.
		///
		///</summary>
		static void pcache1Cachesize (sqlite3_pcache p, int nMax)
		{
			PCache1 pCache = (PCache1)p;
			if (pCache.bPurgeable) {
				PGroup pGroup = pCache.pGroup;
				pcache1EnterMutex (pGroup);
				pGroup.nMaxPage += nMax - pCache.nMax;
				pGroup.mxPinned = pGroup.nMaxPage + 10 - pGroup.nMinPage;
				pCache.nMax = nMax;
				pCache.n90pct = pCache.nMax * 9 / 10;
				pcache1EnforceMaxPage (pGroup);
				pcache1LeaveMutex (pGroup);
			}
		}

		///<summary>
		/// Implementation of the sqlite3_pcache.xPagecount method.
		///
		///</summary>
		static int pcache1Pagecount (sqlite3_pcache p)
		{
			int n;
			PCache1 pCache = (PCache1)p;
			pcache1EnterMutex (pCache.pGroup);
			n = (int)pCache.nPage;
			pcache1LeaveMutex (pCache.pGroup);
			return n;
		}

		///<summary>
		/// Implementation of the sqlite3_pcache.xFetch method.
		///
		/// Fetch a page by key value.
		///
		/// Whether or not a new page may be allocated by this function depends on
		/// the value of the createFlag argument.  0 means do not allocate a new
		/// page.  1 means allocate a new page if space is easily available.  2
		/// means to try really hard to allocate a new page.
		///
		/// For a non-purgeable cache (a cache used as the storage for an in-memory
		/// database) there is really no difference between createFlag 1 and 2.  So
		/// the calling function (pcache.c) will never have a createFlag of 1 on
		/// a non-purgable cache.
		///
		/// There are three different approaches to obtaining space for a page,
		/// depending on the value of parameter createFlag (which may be 0, 1 or 2).
		///
		///   1. Regardless of the value of createFlag, the cache is searched for a
		///      copy of the requested page. If one is found, it is returned.
		///
		///   2. If createFlag==0 and the page is not already in the cache, NULL is
		///      returned.
		///
		///   3. If createFlag is 1, and the page is not already in the cache, then
		///      return NULL (do not allocate a new page) if any of the following
		///      conditions are true:
		///
		///       (a) the number of pages pinned by the cache is greater than
		///           PCache1.nMax, or
		///
		///       (b) the number of pages pinned by the cache is greater than
		///           the sum of nMax for all purgeable caches, less the sum of
		///           nMin for all other purgeable caches, or
		///
		///   4. If none of the first three conditions apply and the cache is marked
		///      as purgeable, and if one of the following is true:
		///
		///       (a) The number of pages allocated for the cache is already
		///           PCache1.nMax, or
		///
		///       (b) The number of pages allocated for all purgeable caches is
		///           already equal to or greater than the sum of nMax for all
		///           purgeable caches,
		///
		///       (c) The system is under memory pressure and wants to avoid
		///           unnecessary pages cache entry allocations
		///
		///      then attempt to recycle a page from the LRU list. If it is the right
		///      size, return the recycled buffer. Otherwise, free the buffer and
		///      proceed to step 5.
		///
		///   5. Otherwise, allocate and return a new page buffer.
		///
		///</summary>
		static PgHdr pcache1Fetch (sqlite3_pcache p, Pgno iKey, int createFlag)
		{
			int nPinned;
			PCache1 pCache = (PCache1)p;
			PGroup pGroup;
			PgHdr1 pPage = null;
			Debug.Assert (pCache.bPurgeable || createFlag != 1);
			Debug.Assert (pCache.bPurgeable || pCache.nMin == 0);
			Debug.Assert (pCache.bPurgeable == false || pCache.nMin == 10);
			Debug.Assert (pCache.nMin == 0 || pCache.bPurgeable);
			pcache1EnterMutex (pGroup = pCache.pGroup);
			///
///<summary>
///Step 1: Search the hash table for an existing entry. 
///</summary>

			if (pCache.nHash > 0) {
				int h = (int)(iKey % pCache.nHash);
				for (pPage = pCache.apHash [h]; pPage != null && pPage.iKey != iKey; pPage = pPage.pNext)
					;
			}
			///
///<summary>
///Step 2: Abort if no existing page is found and createFlag is 0 
///</summary>

			if (pPage != null || createFlag == 0) {
				pcache1PinPage (pPage);
				goto fetch_out;
			}
			///
///<summary>
///The pGroup local variable will normally be initialized by the
///pcache1EnterMutex() macro above.  But if SQLITE_MUTEX_OMIT is defined,
///</summary>
///<param name="then pcache1EnterMutex() is a no">op, so we have to initialize the</param>
///<param name="local variable here.  Delaying the initialization of pGroup is an">local variable here.  Delaying the initialization of pGroup is an</param>
///<param name="optimization:  The common case is to exit the module before reaching">optimization:  The common case is to exit the module before reaching</param>
///<param name="this point.">this point.</param>
///<param name=""></param>

			#if SQLITE_MUTEX_OMIT
			pGroup = pCache.pGroup;
			#endif
			///
///<summary>
///Step 3: Abort if createFlag is 1 but the cache is nearly full 
///</summary>

			nPinned = pCache.nPage - pCache.nRecyclable;
			Debug.Assert (nPinned >= 0);
			Debug.Assert (pGroup.mxPinned == pGroup.nMaxPage + 10 - pGroup.nMinPage);
			Debug.Assert (pCache.n90pct == pCache.nMax * 9 / 10);
			if (createFlag == 1 && (nPinned >= pGroup.mxPinned || nPinned >= (int)pCache.n90pct || pcache1UnderMemoryPressure (pCache))) {
				goto fetch_out;
			}
			if (pCache.nPage >= pCache.nHash && pcache1ResizeHash (pCache) != 0) {
				goto fetch_out;
			}
			///
///<summary>
///Step 4. Try to recycle a page. 
///</summary>

			if (pCache.bPurgeable && pGroup.pLruTail != null && ((pCache.nPage + 1 >= pCache.nMax) || pGroup.nCurrentPage >= pGroup.nMaxPage || pcache1UnderMemoryPressure (pCache))) {
				PCache1 pOtherCache;
				pPage = pGroup.pLruTail;
				pcache1RemoveFromHash (pPage);
				pcache1PinPage (pPage);
				if ((pOtherCache = pPage.pCache).szPage != pCache.szPage) {
					pcache1FreePage (ref pPage);
					pPage = null;
				}
				else {
					pGroup.nCurrentPage -= (pOtherCache.bPurgeable ? 1 : 0) - (pCache.bPurgeable ? 1 : 0);
				}
			}
			///
///<summary>
///Step 5. If a usable page buffer has still not been found, 
///attempt to allocate a new one. 
///
///</summary>

			if (null == pPage) {
				if (createFlag == 1)
					sqlite3BeginBenignMalloc ();
				pcache1LeaveMutex (pGroup);
				pPage = pcache1AllocPage (pCache);
				pcache1EnterMutex (pGroup);
				if (createFlag == 1)
					sqlite3EndBenignMalloc ();
			}
			if (pPage != null) {
				int h = (int)(iKey % pCache.nHash);
				pCache.nPage++;
				pPage.iKey = iKey;
				pPage.pNext = pCache.apHash [h];
				pPage.pCache = pCache;
				pPage.pLruPrev = null;
				pPage.pLruNext = null;
				PGHDR1_TO_PAGE (pPage).Clear ();
				// *(void **)(PGHDR1_TO_PAGE(pPage)) = 0;
				pPage.pPgHdr.pPgHdr1 = pPage;
				pCache.apHash [h] = pPage;
			}
			fetch_out:
			if (pPage != null && iKey > pCache.iMaxKey) {
				pCache.iMaxKey = iKey;
			}
			pcache1LeaveMutex (pGroup);
			return (pPage != null ? PGHDR1_TO_PAGE (pPage) : null);
		}

		///<summary>
		/// Implementation of the sqlite3_pcache.xUnpin method.
		///
		/// Mark a page as unpinned (eligible for asynchronous recycling).
		///
		///</summary>
		static void pcache1Unpin (sqlite3_pcache p, PgHdr pPg, bool reuseUnlikely)
		{
			PCache1 pCache = (PCache1)p;
			PgHdr1 pPage = PAGE_TO_PGHDR1 (pCache, pPg);
			PGroup pGroup = pCache.pGroup;
			Debug.Assert (pPage.pCache == pCache);
			pcache1EnterMutex (pGroup);
			///
///<summary>
///It is an error to call this function if the page is already 
///part of the PGroup LRU list.
///
///</summary>

			Debug.Assert (pPage.pLruPrev == null && pPage.pLruNext == null);
			Debug.Assert (pGroup.pLruHead != pPage && pGroup.pLruTail != pPage);
			if (reuseUnlikely || pGroup.nCurrentPage > pGroup.nMaxPage) {
				pcache1RemoveFromHash (pPage);
				pcache1FreePage (ref pPage);
			}
			else {
				///
///<summary>
///Add the page to the PGroup LRU list. 
///</summary>

				if (pGroup.pLruHead != null) {
					pGroup.pLruHead.pLruPrev = pPage;
					pPage.pLruNext = pGroup.pLruHead;
					pGroup.pLruHead = pPage;
				}
				else {
					pGroup.pLruTail = pPage;
					pGroup.pLruHead = pPage;
				}
				pCache.nRecyclable++;
			}
			pcache1LeaveMutex (pCache.pGroup);
		}

		///<summary>
		/// Implementation of the sqlite3_pcache.xRekey method.
		///
		///</summary>
		static void pcache1Rekey (sqlite3_pcache p, PgHdr pPg, Pgno iOld, Pgno iNew)
		{
			PCache1 pCache = (PCache1)p;
			PgHdr1 pPage = PAGE_TO_PGHDR1 (pCache, pPg);
			PgHdr1 pp;
			int h;
			Debug.Assert (pPage.iKey == iOld);
			Debug.Assert (pPage.pCache == pCache);
			pcache1EnterMutex (pCache.pGroup);
			h = (int)(iOld % pCache.nHash);
			pp = pCache.apHash [h];
			while ((pp) != pPage) {
				pp = (pp).pNext;
			}
			if (pp == pCache.apHash [h])
				pCache.apHash [h] = pp.pNext;
			else
				pp.pNext = pPage.pNext;
			h = (int)(iNew % pCache.nHash);
			pPage.iKey = iNew;
			pPage.pNext = pCache.apHash [h];
			pCache.apHash [h] = pPage;
			if (iNew > pCache.iMaxKey) {
				pCache.iMaxKey = iNew;
			}
			pcache1LeaveMutex (pCache.pGroup);
		}

		///<summary>
		/// Implementation of the sqlite3_pcache.xTruncate method.
		///
		/// Discard all unpinned pages in the cache with a page number equal to
		/// or greater than parameter iLimit. Any pinned pages with a page number
		/// equal to or greater than iLimit are implicitly unpinned.
		///
		///</summary>
		static void pcache1Truncate (sqlite3_pcache p, Pgno iLimit)
		{
			PCache1 pCache = (PCache1)p;
			pcache1EnterMutex (pCache.pGroup);
			if (iLimit <= pCache.iMaxKey) {
				pcache1TruncateUnsafe (pCache, iLimit);
				pCache.iMaxKey = iLimit - 1;
			}
			pcache1LeaveMutex (pCache.pGroup);
		}

		///<summary>
		/// Implementation of the sqlite3_pcache.xDestroy method.
		///
		/// Destroy a cache allocated using pcache1Create().
		///
		///</summary>
		static void pcache1Destroy (ref sqlite3_pcache p)
		{
			PCache1 pCache = (PCache1)p;
			PGroup pGroup = pCache.pGroup;
			Debug.Assert (pCache.bPurgeable || (pCache.nMax == 0 && pCache.nMin == 0));
			pcache1EnterMutex (pGroup);
			pcache1TruncateUnsafe (pCache, 0);
			pGroup.nMaxPage -= pCache.nMax;
			pGroup.nMinPage -= pCache.nMin;
			pGroup.mxPinned = pGroup.nMaxPage + 10 - pGroup.nMinPage;
			pcache1EnforceMaxPage (pGroup);
			pcache1LeaveMutex (pGroup);
			//sqlite3_free(  pCache.apHash );
			//sqlite3_free( pCache );
			p = null;
		}

		///
///<summary>
///This function is called during initialization (sqlite3_initialize()) to
///install the default pluggable cache module, assuming the user has not
///already provided an alternative.
///
///</summary>

		static void sqlite3PCacheSetDefault ()
		{
			sqlite3_pcache_methods defaultMethods = new sqlite3_pcache_methods (0, ///
///<summary>
///pArg 
///</summary>

			(dxPC_Init)pcache1Init, ///
///<summary>
///xInit 
///</summary>

			(dxPC_Shutdown)pcache1Shutdown, ///
///<summary>
///xShutdown 
///</summary>

			(dxPC_Create)pcache1Create, ///
///<summary>
///xCreate 
///</summary>

			(dxPC_Cachesize)pcache1Cachesize, ///
///<summary>
///xCachesize 
///</summary>

			(dxPC_Pagecount)pcache1Pagecount, ///
///<summary>
///xPagecount 
///</summary>

			(dxPC_Fetch)pcache1Fetch, ///
///<summary>
///xFetch 
///</summary>

			(dxPC_Unpin)pcache1Unpin, ///
///<summary>
///xUnpin 
///</summary>

			(dxPC_Rekey)pcache1Rekey, ///
///<summary>
///xRekey 
///</summary>

			(dxPC_Truncate)pcache1Truncate, ///
///<summary>
///xTruncate 
///</summary>

			(dxPC_Destroy)pcache1Destroy///
///<summary>
///xDestroy 
///</summary>

			);
			sqlite3_config (SQLITE_CONFIG_PCACHE, defaultMethods);
		}
	#if SQLITE_ENABLE_MEMORY_MANAGEMENT
																			/*
** This function is called to free superfluous dynamically allocated memory
** held by the pager system. Memory in use by any SQLite pager allocated
** by the current thread may be sqlite3_free()ed.
**
** nReq is the number of bytes of memory required. Once this much has
** been released, the function returns. The return value is the total number 
** of bytes of memory released.
*/
int sqlite3PcacheReleaseMemory(int nReq){
  int nFree = 0;
  Debug.Assert( sqlite3_mutex_notheld(pcache1.grp.mutex) );
  Debug.Assert( sqlite3_mutex_notheld(pcache1.mutex) );
  if( pcache1.pStart==0 ){
    PgHdr1 p;
    pcache1EnterMutex(&pcache1.grp);
    while( (nReq<0 || nFree<nReq) && ((p=pcache1.grp.pLruTail)!=0) ){
      nFree += pcache1MemSize(PGHDR1_TO_PAGE(p));
      PCache1pinPage(p);
      pcache1RemoveFromHash(p);
      pcache1FreePage(p);
    }
    pcache1LeaveMutex(&pcache1.grp);
  }
  return nFree;
}
#endif
	#if SQLITE_TEST
																			    /*
** This function is used by test procedures to inspect the internal state
** of the global cache.
*/
    static void sqlite3PcacheStats(
      out int pnCurrent,      /* OUT: Total number of pages cached */
      out int pnMax,          /* OUT: Global maximum cache size */
      out int pnMin,          /* OUT: Sum of PCache1.nMin for purgeable caches */
      out int pnRecyclable    /* OUT: Total number of pages available for recycling */
    )
    {
      PgHdr1 p;
      int nRecyclable = 0;
      for ( p = pcache1.grp.pLruHead; p != null; p = p.pLruNext )
      {
        nRecyclable++;
      }
      pnCurrent = pcache1.grp.nCurrentPage;
      pnMax = pcache1.grp.nMaxPage;
      pnMin = pcache1.grp.nMinPage;
      pnRecyclable = nRecyclable;
    }
#endif
	}
}
