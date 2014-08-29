using System;
using System.Diagnostics;
using System.Text;
using u32=System.UInt32;
using Pgno=System.UInt32;
namespace Community.CsharpSqlite {
	using sqlite3_value=Sqlite3.Mem;
	using sqlite3_pcache=Sqlite3.PCache1;
	public partial class Sqlite3 {
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
		/*
    ** A complete page cache is an instance of this structure.
    */public class PCache {
			public PgHdr pDirty,pDirtyTail;
			/* List of dirty pages in LRU order */public PgHdr pSynced;
			/* Last synced page in dirty page list */public int _nRef;
			/* Number of referenced pages */public int nMax;
			/* Configured cache size */public int szPage;
			/* Size of every page in this cache */public int szExtra;
			/* Size of extra space for each page */public bool bPurgeable;
			/* True if pages are on backing store */public dxStress xStress;
			//int (*xStress)(void*,PgHdr*);       /* Call to try make a page clean */
			public object pStress;
			/* Argument to xStress */public sqlite3_pcache pCache;
			/* Pluggable cache module */public PgHdr pPage1;
			///<summary>
			///Reference to page 1
			///</summary>
			public int nRef /* Number of referenced pages */{
				get {
					return _nRef;
				}
				set {
					_nRef=value;
				}
			}
			public void Clear() {
				pDirty=null;
				pDirtyTail=null;
				pSynced=null;
				nRef=0;
			}
		};

		/*
    ** Some of the Debug.Assert() macros in this code are too expensive to run
    ** even during normal debugging.  Use them only rarely on long-running
    ** tests.  Enable the expensive asserts using the
    ** -DSQLITE_ENABLE_EXPENSIVE_ASSERT=1 compile-time option.
    */
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
Debug.Assert( p.nRef !=0|| (p.flags&PGHDR_NEED_SYNC) !=0);
}
return (p==null || p.nRef!=0 || (p.flags&PGHDR_NEED_SYNC)==0)?1:0;
}
#endif
		///<summary>
		/// Remove page pPage from the list of dirty pages.
		///</summary>
		static void pcacheRemoveFromDirtyList(PgHdr pPage) {
			PCache p=pPage.pCache;
			Debug.Assert(pPage.pDirtyNext!=null||pPage==p.pDirtyTail);
			Debug.Assert(pPage.pDirtyPrev!=null||pPage==p.pDirty);
			/* Update the PCache1.pSynced variable if necessary. */if(p.pSynced==pPage) {
				PgHdr pSynced=pPage.pDirtyPrev;
				while(pSynced!=null&&(pSynced.flags&PGHDR_NEED_SYNC)!=0) {
					pSynced=pSynced.pDirtyPrev;
				}
				p.pSynced=pSynced;
			}
			if(pPage.pDirtyNext!=null) {
				pPage.pDirtyNext.pDirtyPrev=pPage.pDirtyPrev;
			}
			else {
				Debug.Assert(pPage==p.pDirtyTail);
				p.pDirtyTail=pPage.pDirtyPrev;
			}
			if(pPage.pDirtyPrev!=null) {
				pPage.pDirtyPrev.pDirtyNext=pPage.pDirtyNext;
			}
			else {
				Debug.Assert(pPage==p.pDirty);
				p.pDirty=pPage.pDirtyNext;
			}
			pPage.pDirtyNext=null;
			pPage.pDirtyPrev=null;
			#if SQLITE_ENABLE_EXPENSIVE_ASSERT
															expensive_assert( pcacheCheckSynced(p) );
#endif
		}
		///<summary>
		/// Add page pPage to the head of the dirty list (PCache1.pDirty is set to
		/// pPage).
		///
		///</summary>
		static void pcacheAddToDirtyList(PgHdr pPage) {
			PCache p=pPage.pCache;
			Debug.Assert(pPage.pDirtyNext==null&&pPage.pDirtyPrev==null&&p.pDirty!=pPage);
			pPage.pDirtyNext=p.pDirty;
			if(pPage.pDirtyNext!=null) {
				Debug.Assert(pPage.pDirtyNext.pDirtyPrev==null);
				pPage.pDirtyNext.pDirtyPrev=pPage;
			}
			p.pDirty=pPage;
			if(null==p.pDirtyTail) {
				p.pDirtyTail=pPage;
			}
			if(null==p.pSynced&&0==(pPage.flags&PGHDR_NEED_SYNC)) {
				p.pSynced=pPage;
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
		static void pcacheUnpin(PgHdr p) {
			PCache pCache=p.pCache;
			if(pCache.bPurgeable) {
				if(p.pgno==1) {
					pCache.pPage1=null;
				}
				sqlite3GlobalConfig.pcache.xUnpin(pCache.pCache,p,false);
			}
		}
		///<summary>
		/// General Interfaces 
		///
		/// Initialize and shutdown the page cache subsystem. Neither of these
		/// functions are threadsafe.
		///
		///</summary>
		static int sqlite3PcacheInitialize() {
			if(sqlite3GlobalConfig.pcache.xInit==null) {
				/* IMPLEMENTATION-OF: R-26801-64137 If the xInit() method is NULL, then the
        ** built-in default page cache is used instead of the application defined
        ** page cache. */sqlite3PCacheSetDefault();
			}
			return sqlite3GlobalConfig.pcache.xInit(sqlite3GlobalConfig.pcache.pArg);
		}
		static void sqlite3PcacheShutdown() {
			if(sqlite3GlobalConfig.pcache.xShutdown!=null) {
				/* IMPLEMENTATION-OF: R-26000-56589 The xShutdown() method may be NULL. */sqlite3GlobalConfig.pcache.xShutdown(sqlite3GlobalConfig.pcache.pArg);
			}
		}
		///<summary>
		/// Return the size in bytes of a PCache object.
		///
		///</summary>
		static int sqlite3PcacheSize() {
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
		static void sqlite3PcacheOpen(int szPage,/* Size of every page */int szExtra,/* Extra space associated with each page */bool bPurgeable,/* True if pages are on backing store */dxStress xStress,//int (*xStress)(void*,PgHdr*),/* Call to try to make pages clean */
		object pStress,/* Argument to xStress */PCache p/* Preallocated space for the PCache */) {
			p.Clear();
			//memset(p, 0, sizeof(PCache));
			p.szPage=szPage;
			p.szExtra=szExtra;
			p.bPurgeable=bPurgeable;
			p.xStress=xStress;
			p.pStress=pStress;
			p.nMax=100;
		}
		///<summary>
		/// Change the page size for PCache object. The caller must ensure that there
		/// are no outstanding page references when this function is called.
		///
		///</summary>
		static void sqlite3PcacheSetPageSize(PCache pCache,int szPage) {
			Debug.Assert(pCache.nRef==0&&pCache.pDirty==null);
			if(pCache.pCache!=null) {
				sqlite3GlobalConfig.pcache.xDestroy(ref pCache.pCache);
				pCache.pCache=null;
			}
			pCache.szPage=szPage;
		}
		///<summary>
		/// Try to obtain a page from the cache.
		///
		///</summary>
		static int sqlite3PcacheFetch(PCache pCache,/* Obtain the page from this cache */u32 pgno,/* Page number to obtain */int createFlag,/* If true, create page if it does not exist already */ref PgHdr ppPage/* Write the page here */) {
			PgHdr pPage=null;
			int eCreate;
			Debug.Assert(pCache!=null);
			Debug.Assert(createFlag==1||createFlag==0);
			Debug.Assert(pgno>0);
			/* If the pluggable cache (sqlite3_pcache*) has not been allocated,
      ** allocate it now.
      */if(null==pCache.pCache&&createFlag!=0) {
				sqlite3_pcache p;
				int nByte;
				nByte=pCache.szPage+pCache.szExtra+0;
				// sizeof( PgHdr );
				p=sqlite3GlobalConfig.pcache.xCreate(nByte,pCache.bPurgeable);
				//if ( null == p )
				//{
				//  return SQLITE_NOMEM;
				//}
				sqlite3GlobalConfig.pcache.xCachesize(p,pCache.nMax);
				pCache.pCache=p;
			}
			eCreate=createFlag*(1+((!pCache.bPurgeable||null==pCache.pDirty)?1:0));
			if(pCache.pCache!=null) {
				pPage=sqlite3GlobalConfig.pcache.xFetch(pCache.pCache,pgno,eCreate);
			}
			if(null==pPage&&eCreate==1) {
				PgHdr pPg;
				/* Find a dirty page to write-out and recycle. First try to find a
        ** page that does not require a journal-sync (one with PGHDR_NEED_SYNC
        ** cleared), but if that is not possible settle for any other
        ** unreferenced dirty page.
        */
				#if SQLITE_ENABLE_EXPENSIVE_ASSERT
																				expensive_assert( pcacheCheckSynced(pCache) );
#endif
				for(pPg=pCache.pSynced;pPg!=null&&(pPg.nRef!=0||(pPg.flags&PGHDR_NEED_SYNC)!=0);pPg=pPg.pDirtyPrev)
					;
				pCache.pSynced=pPg;
				if(null==pPg) {
					for(pPg=pCache.pDirtyTail;pPg!=null&&pPg.nRef!=0;pPg=pPg.pDirtyPrev)
						;
				}
				if(pPg!=null) {
					int rc;
					#if SQLITE_LOG_CACHE_SPILL
																									      sqlite3_log(SQLITE_FULL, 
                  "spill page %d making room for %d - cache used: %d/%d",
                  pPg->pgno, pgno,
                  sqlite3GlobalConfig.pcache.xPagecount(pCache->pCache),
                  pCache->nMax);
#endif
					rc=pCache.xStress(pCache.pStress,pPg);
					if(rc!=SQLITE_OK&&rc!=SQLITE_BUSY) {
						return rc;
					}
				}
				pPage=sqlite3GlobalConfig.pcache.xFetch(pCache.pCache,pgno,2);
			}
			if(pPage!=null) {
				if(null==pPage.pData) {
					//          memset(pPage, 0, sizeof(PgHdr));
					pPage.pData=sqlite3Malloc(pCache.szPage);
					//          pPage->pData = (void*)&pPage[1];
					//pPage->pExtra = (void*)&((char*)pPage->pData)[pCache->szPage];
					//memset(pPage->pExtra, 0, pCache->szExtra);
					pPage.pCache=pCache;
					pPage.pgno=pgno;
				}
				Debug.Assert(pPage.pCache==pCache);
				Debug.Assert(pPage.pgno==pgno);
				//assert(pPage->pData == (void*)&pPage[1]);
				//assert(pPage->pExtra == (void*)&((char*)&pPage[1])[pCache->szPage]);
				if(0==pPage.nRef) {
					pCache.nRef++;
				}
				pPage.nRef++;
				if(pgno==1) {
					pCache.pPage1=pPage;
				}
			}
			ppPage=pPage;
			return (pPage==null&&eCreate!=0)?SQLITE_NOMEM:SQLITE_OK;
		}
		///<summary>
		/// Decrement the reference count on a page. If the page is clean and the
		/// reference count drops to 0, then it is made elible for recycling.
		///
		///</summary>
		static void sqlite3PcacheRelease(PgHdr p) {
			Debug.Assert(p.nRef>0);
			p.nRef--;
			if(p.nRef==0) {
				PCache pCache=p.pCache;
				pCache.nRef--;
				if((p.flags&PGHDR_DIRTY)==0) {
					pcacheUnpin(p);
				}
				else {
					/* Move the page to the head of the dirty list. */pcacheRemoveFromDirtyList(p);
					pcacheAddToDirtyList(p);
				}
			}
		}
		///<summary>
		/// Increase the reference count of a supplied page by 1.
		///
		///</summary>
		static void sqlite3PcacheRef(PgHdr p) {
			Debug.Assert(p.nRef>0);
			p.nRef++;
		}
		///<summary>
		/// Drop a page from the cache. There must be exactly one reference to the
		/// page. This function deletes that reference, so after it returns the
		/// page pointed to by p is invalid.
		///
		///</summary>
		static void sqlite3PcacheDrop(PgHdr p) {
			PCache pCache;
			Debug.Assert(p.nRef==1);
			if((p.flags&PGHDR_DIRTY)!=0) {
				pcacheRemoveFromDirtyList(p);
			}
			pCache=p.pCache;
			pCache.nRef--;
			if(p.pgno==1) {
				pCache.pPage1=null;
			}
			sqlite3GlobalConfig.pcache.xUnpin(pCache.pCache,p,true);
		}
		///<summary>
		/// Make sure the page is marked as dirty. If it isn't dirty already,
		/// make it so.
		///
		///</summary>
		static void sqlite3PcacheMakeDirty(PgHdr p) {
			p.flags&=~PGHDR_DONT_WRITE;
			Debug.Assert(p.nRef>0);
			if(0==(p.flags&PGHDR_DIRTY)) {
				p.flags|=PGHDR_DIRTY;
				pcacheAddToDirtyList(p);
			}
		}
		///<summary>
		/// Make sure the page is marked as clean. If it isn't clean already,
		/// make it so.
		///
		///</summary>
		static void sqlite3PcacheMakeClean(PgHdr p) {
			if((p.flags&PGHDR_DIRTY)!=0) {
				pcacheRemoveFromDirtyList(p);
				p.flags&=~(PGHDR_DIRTY|PGHDR_NEED_SYNC);
				if(p.nRef==0) {
					pcacheUnpin(p);
				}
			}
		}
		///<summary>
		/// Make every page in the cache clean.
		///
		///</summary>
		static void sqlite3PcacheCleanAll(PCache pCache) {
			PgHdr p;
			while((p=pCache.pDirty)!=null) {
				sqlite3PcacheMakeClean(p);
			}
		}
		///<summary>
		/// Clear the PGHDR_NEED_SYNC flag from all dirty pages.
		///
		///</summary>
		static void sqlite3PcacheClearSyncFlags(PCache pCache) {
			PgHdr p;
			for(p=pCache.pDirty;p!=null;p=p.pDirtyNext) {
				p.flags&=~PGHDR_NEED_SYNC;
			}
			pCache.pSynced=pCache.pDirtyTail;
		}
		///<summary>
		/// Change the page number of page p to newPgno.
		///
		///</summary>
		static void sqlite3PcacheMove(PgHdr p,Pgno newPgno) {
			PCache pCache=p.pCache;
			Debug.Assert(p.nRef>0);
			Debug.Assert(newPgno>0);
			sqlite3GlobalConfig.pcache.xRekey(pCache.pCache,p,p.pgno,newPgno);
			p.pgno=newPgno;
			if((p.flags&PGHDR_DIRTY)!=0&&(p.flags&PGHDR_NEED_SYNC)!=0) {
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
		static void sqlite3PcacheTruncate(PCache pCache,u32 pgno) {
			if(pCache.pCache!=null) {
				PgHdr p;
				PgHdr pNext;
				for(p=pCache.pDirty;p!=null;p=pNext) {
					pNext=p.pDirtyNext;
					/* This routine never gets call with a positive pgno except right
          ** after sqlite3PcacheCleanAll().  So if there are dirty pages,
          ** it must be that pgno==0.
          */Debug.Assert(p.pgno>0);
					if(ALWAYS(p.pgno>pgno)) {
						Debug.Assert((p.flags&PGHDR_DIRTY)!=0);
						sqlite3PcacheMakeClean(p);
					}
				}
				if(pgno==0&&pCache.pPage1!=null) {
					// memset( pCache.pPage1.pData, 0, pCache.szPage );
					pCache.pPage1.pData=sqlite3Malloc(pCache.szPage);
					pgno=1;
				}
				sqlite3GlobalConfig.pcache.xTruncate(pCache.pCache,pgno+1);
			}
		}
		///<summary>
		/// Close a cache.
		///
		///</summary>
		static void sqlite3PcacheClose(PCache pCache) {
			if(pCache.pCache!=null) {
				sqlite3GlobalConfig.pcache.xDestroy(ref pCache.pCache);
			}
		}
		///<summary>
		/// Discard the contents of the cache.
		///
		///</summary>
		static void sqlite3PcacheClear(PCache pCache) {
			sqlite3PcacheTruncate(pCache,0);
		}
		/*
    ** Merge two lists of pages connected by pDirty and in pgno order.
    ** Do not both fixing the pDirtyPrev pointers.
    */static PgHdr pcacheMergeDirtyList(PgHdr pA,PgHdr pB) {
			PgHdr result=new PgHdr();
			PgHdr pTail=result;
			while(pA!=null&&pB!=null) {
				if(pA.pgno<pB.pgno) {
					pTail.pDirty=pA;
					pTail=pA;
					pA=pA.pDirty;
				}
				else {
					pTail.pDirty=pB;
					pTail=pB;
					pB=pB.pDirty;
				}
			}
			if(pA!=null) {
				pTail.pDirty=pA;
			}
			else
				if(pB!=null) {
					pTail.pDirty=pB;
				}
				else {
					pTail.pDirty=null;
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
		const int N_SORT_BUCKET=32;
		static PgHdr pcacheSortDirtyList(PgHdr pIn) {
			PgHdr[] a;
			PgHdr p;
			//a[N_SORT_BUCKET], p;
			int i;
			a=new PgHdr[N_SORT_BUCKET];
			//memset(a, 0, sizeof(a));
			while(pIn!=null) {
				p=pIn;
				pIn=p.pDirty;
				p.pDirty=null;
				for(i=0;ALWAYS(i<N_SORT_BUCKET-1);i++) {
					if(a[i]==null) {
						a[i]=p;
						break;
					}
					else {
						p=pcacheMergeDirtyList(a[i],p);
						a[i]=null;
					}
				}
				if(NEVER(i==N_SORT_BUCKET-1)) {
					/* To get here, there need to be 2^(N_SORT_BUCKET) elements in
          ** the input list.  But that is impossible.
          */a[i]=pcacheMergeDirtyList(a[i],p);
				}
			}
			p=a[0];
			for(i=1;i<N_SORT_BUCKET;i++) {
				p=pcacheMergeDirtyList(p,a[i]);
			}
			return p;
		}
		///<summary>
		/// Return a list of all dirty pages in the cache, sorted by page number.
		///
		///</summary>
		static PgHdr sqlite3PcacheDirtyList(PCache pCache) {
			PgHdr p;
			for(p=pCache.pDirty;p!=null;p=p.pDirtyNext) {
				p.pDirty=p.pDirtyNext;
			}
			return pcacheSortDirtyList(pCache.pDirty);
		}
		///<summary>
		/// Return the total number of referenced pages held by the cache.
		///
		///</summary>
		static int sqlite3PcacheRefCount(PCache pCache) {
			return pCache.nRef;
		}
		///<summary>
		/// Return the number of references to the page supplied as an argument.
		///
		///</summary>
		static int sqlite3PcachePageRefcount(PgHdr p) {
			return p.nRef;
		}
		///<summary>
		/// Return the total number of pages in the cache.
		///
		///</summary>
		static int sqlite3PcachePagecount(PCache pCache) {
			int nPage=0;
			if(pCache.pCache!=null) {
				nPage=sqlite3GlobalConfig.pcache.xPagecount(pCache.pCache);
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
		/*
** Set the suggested cache-size value.
*/static void sqlite3PcacheSetCachesize(PCache pCache,int mxPage) {
			pCache.nMax=mxPage;
			if(pCache.pCache!=null) {
				sqlite3GlobalConfig.pcache.xCachesize(pCache.pCache,mxPage);
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
