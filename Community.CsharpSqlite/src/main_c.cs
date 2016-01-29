using System;
using System.Diagnostics;
using System.Text;
using System.Linq;
using sqlite_int64=System.Int64;
using unsigned=System.Int32;
using i16=System.Int16;
using u8=System.Byte;
using u16=System.UInt16;
using u32=System.UInt32;
using u64=System.UInt64;
using Pgno=System.UInt32;
using sqlite3_int64=System.Int64;
namespace Community.CsharpSqlite {
    using sqlite3_value = Engine.Mem;
    using Community.CsharpSqlite.Os;
    using Community.CsharpSqlite.Metadata;
    using Community.CsharpSqlite.Engine;
    using Community.CsharpSqlite.tree;
    using Community.CsharpSqlite.Utils;
    using Community.CsharpSqlite.Paging;
    using Community.CsharpSqlite.Metadata.Traverse;
    using Cache;
    using Ast;
    public partial class Sqlite3 {
		///
		///<summary>
		///2001 September 15
		///
		///The author disclaims copyright to this source code.  In place of
		///a legal notice, here is a blessing:
		///
		///May you do good and not evil.
		///May you find forgiveness for yourself and forgive others.
		///May you share freely, never taking more than you give.
		///
		///
		///Main file for the SQLite library.  The routines in this file
		///implement the programmer interface to the library.  Routines in
		///other files are for internal use by SQLite and should not be
		///accessed by users of the library.
		///
		///</summary>
		///<param name="Included in SQLite3 port to C#">SQLite;  2008 Noah B Hart</param>
		///<param name="C#">SQLite is an independent reimplementation of the SQLite software library</param>
		///<param name=""></param>
		///<param name="SQLITE_SOURCE_ID: 2011">23 19:49:22 4374b7e83ea0a3fbc3691f9c0c936272862f32f2</param>
		///<param name=""></param>
		///<param name=""></param>
		///<param name=""></param>
		//#include "sqliteInt.h"
		#if SQLITE_ENABLE_FTS3
																																														// include "fts3.h"
#endif
		#if SQLITE_ENABLE_RTREE
																																														// include "rtree.h"
#endif
		#if SQLITE_ENABLE_ICU
																																														// include "sqliteicu.h"
#endif
		#if !SQLITE_AMALGAMATION
		///<summary>
		///IMPLEMENTATION-OF: R-46656-45156 The sqlite3_version[] string constant
		/// contains the text of SQLITE_VERSION macro.
		///</summary>
		public static string sqlite3_version=SQLITE_VERSION;
		#endif
		///<summary>
		///IMPLEMENTATION-OF: R-53536-42575 The sqlite3_libversion() function returns
		/// a pointer to the to the sqlite3_version[] string constant.
		///</summary>
		public static string sqlite3_libversion() {
			return sqlite3_version;
		}
		///<summary>
		///IMPLEMENTATION-OF: R-63124-39300 The sqlite3_sourceid() function returns a
		/// pointer to a string constant whose value is the same as the
		/// SQLITE_SOURCE_ID C preprocessor macro.
		///
		///</summary>
		public static string sqlite3_sourceid() {
			return SQLITE_SOURCE_ID;
		}
		///<summary>
		///IMPLEMENTATION-OF: R-35210-63508 The sqlite3_libversion_number() function
		/// returns an integer equal to SQLITE_VERSION_NUMBER.
		///
		///</summary>
		public static int sqlite3_libversion_number() {
			return SQLITE_VERSION_NUMBER;
		}
		///
		///<summary>
		///</summary>
		///<param name="IMPLEMENTATION">41343 The sqlite3_threadsafe() function returns</param>
		///<param name="zero if and only if SQLite was compiled mutexing code omitted due to">zero if and only if SQLite was compiled mutexing code omitted due to</param>
		///<param name="the SQLITE_THREADSAFE compile">time option being set to 0.</param>
		///<param name=""></param>
		public static int sqlite3_threadsafe() {
			return Globals.SQLITE_THREADSAFE;
		}
		#if !SQLITE_OMIT_TRACE && SQLITE_ENABLE_IOTRACE
																																														/*
** If the following function pointer is not NULL and if
** SQLITE_ENABLE_IOTRACE is enabled, then messages describing
** I/O active are written using this function.  These messages
** are intended for debugging activity only.
*/
//void (*sqlite3IoTrace)(const char*, ...) = 0;
static void sqlite3IoTrace( string X, params object[] ap ) {  }
#endif
		///<summary>
		/// If the following global variable points to a string which is the
		/// name of a directory, then that directory will be used to store
		/// temporary files.
		///
		/// See also the "PRAGMA temp_store_directory" SQL command.
		///</summary>
		static string sqlite3_temp_directory="";
		//string sqlite3_temp_directory = 0;
		///<summary>
		/// Initialize SQLite.
		///
		/// This routine must be called to initialize the memory allocation,
		/// VFS, and mutex subsystems prior to doing any serious work with
		/// SQLite.  But as long as you do not compile with SQLITE_OMIT_AUTOINIT
		/// this routine will be called automatically by key routines such as
		/// sqlite3_open().
		///
		/// This routine is a no-op except on its very first call for the process,
		/// or for the first call after a call to sqlite3_shutdown.
		///
		/// The first thread to call this routine runs the initialization to
		/// completion.  If subsequent threads call this routine before the first
		/// thread has finished the initialization process, then the subsequent
		/// threads must block until the first thread finishes with the initialization.
		///
		/// The first thread might call this routine recursively.  Recursive
		/// calls to this routine should not block, of course.  Otherwise the
		/// initialization process would never complete.
		///
		/// Let X be the first thread to enter this routine.  Let Y be some other
		/// thread.  Then while the initial invocation of this routine by X is
		/// incomplete, it is required that:
		///
		///    *  Calls to this routine from Y must block until the outer-most
		///       call by X completes.
		///
		///    *  Recursive calls to this routine from thread X return immediately
		///       without blocking.
		///
		///</summary>
		public static SqlResult sqlite3_initialize() {
			//--------------------------------------------------------------------
			// Under C#, Need to initialize some static variables
			//
			if(sqlite3_version==null)
				sqlite3_version=SQLITE_VERSION;
			if(sqlite3OpcodeProperty==null)
				sqlite3OpcodeProperty=Array.ConvertAll(Sqlite3.OPFLG_INITIALIZER,(i)=>(OpFlag)i);
			if(sqliteinth.sqlite3GlobalConfig==null)
				sqliteinth.sqlite3GlobalConfig=sqlite3Config;
			//--------------------------------------------------------------------
			sqlite3_mutex pMaster;
			///
			///<summary>
			///The main static mutex 
			///</summary>
			SqlResult rc;
			///
			///<summary>
			///Result code 
			///</summary>
			#if SQLITE_OMIT_WSD
																																																																					rc = sqlite3_wsd_init(4096, 24);
if( rc!=SqlResult.SQLITE_OK ){
return rc;
}
#endif
			///
			///<summary>
			///If SQLite is already completely initialized, then this call
			///</summary>
			///<param name="to sqlite3_initialize() should be a no">op.  But the initialization</param>
			///<param name="must be complete.  So isInit must not be set until the very end">must be complete.  So isInit must not be set until the very end</param>
			///<param name="of this routine.">of this routine.</param>
			if(sqliteinth.sqlite3GlobalConfig.isInit!=0)
				return SqlResult.SQLITE_OK;
			///
			///<summary>
			///Make sure the mutex subsystem is initialized.  If unable to
			///initialize the mutex subsystem, return early with the error.
			///If the system is so sick that we are unable to allocate a mutex,
			///there is not much SQLite is going to be able to do.
			///
			///The mutex subsystem must take care of serializing its own
			///initialization.
			///
			///</summary>
			rc=sqlite3MutexInit();
			if(rc!=0)
				return rc;
			///
			///<summary>
			///Initialize the malloc() system and the recursive pInitMutex mutex.
			///This operation is protected by the STATIC_MASTER mutex.  Note that
			///MutexAlloc() is called for a static mutex prior to initializing the
			///</summary>
			///<param name="malloc subsystem "> this implies that the allocation of a static</param>
			///<param name="mutex must not require support from the malloc subsystem.">mutex must not require support from the malloc subsystem.</param>
			///<param name=""></param>
			pMaster=sqlite3MutexAlloc(SQLITE_MUTEX_STATIC_MASTER);
			//sqlite3_mutex_enter( pMaster );
			lock(pMaster) {
				sqliteinth.sqlite3GlobalConfig.isMutexInit=1;
				if(sqliteinth.sqlite3GlobalConfig.isMallocInit==0) {
					rc=malloc_cs.sqlite3MallocInit();
				}
				if(rc==SqlResult.SQLITE_OK) {
					sqliteinth.sqlite3GlobalConfig.isMallocInit=1;
					if(sqliteinth.sqlite3GlobalConfig.pInitMutex==null) {
						sqliteinth.sqlite3GlobalConfig.pInitMutex=sqlite3MutexAlloc(SQLITE_MUTEX_RECURSIVE);
						if(sqliteinth.sqlite3GlobalConfig.bCoreMutex&&sqliteinth.sqlite3GlobalConfig.pInitMutex==null) {
							rc=SqlResult.SQLITE_NOMEM;
						}
					}
				}
				if(rc==SqlResult.SQLITE_OK) {
					sqliteinth.sqlite3GlobalConfig.nRefInitMutex++;
				}
			}
			//sqlite3_mutex_leave( pMaster );
			///
			///<summary>
			///If rc is not SqlResult.SQLITE_OK at this point, then either the malloc
			///subsystem could not be initialized or the system failed to allocate
			///the pInitMutex mutex. Return an error in either case.  
			///</summary>
			if(rc!=SqlResult.SQLITE_OK) {
				return rc;
			}
			///
			///<summary>
			///Do the rest of the initialization under the recursive mutex so
			///that we will be able to handle recursive calls into
			///sqlite3_initialize().  The recursive calls normally come through
			///sqlite3_os_init() when it invokes sqlite3_vfs_register(), but other
			///recursive calls might also be possible.
			///
			///</summary>
			///<param name="IMPLEMENTATION">37445 SQLite automatically serializes calls</param>
			///<param name="to the xInit method, so the xInit method need not be threadsafe.">to the xInit method, so the xInit method need not be threadsafe.</param>
			///<param name=""></param>
			///<param name="The following mutex is what serializes access to the appdef pcache xInit">The following mutex is what serializes access to the appdef pcache xInit</param>
			///<param name="methods.  The sqlite3_pcache_methods.xInit() all is embedded in the">methods.  The sqlite3_pcache_methods.xInit() all is embedded in the</param>
			///<param name="call to PCacheMethods.sqlite3PcacheInitialize().">call to PCacheMethods.sqlite3PcacheInitialize().</param>
			///<param name=""></param>
			//sqlite3_mutex_enter( sqliteinth.sqlite3GlobalConfig.pInitMutex );
			lock(sqliteinth.sqlite3GlobalConfig.pInitMutex) {
				if(sqliteinth.sqlite3GlobalConfig.isInit==0&&sqliteinth.sqlite3GlobalConfig.inProgress==0) {
					sqliteinth.sqlite3GlobalConfig.inProgress=1;
					#if SQLITE_OMIT_WSD
																																																																																																																			FuncDefHash *pHash = GLOBAL(FuncDefHash, sqlite3GlobalFunctions);
memset( pHash, 0, sizeof( sqlite3GlobalFunctions ) );
#else
					sqlite3GlobalFunctions=new FuncDefHash();
					FuncDefHash pHash=sqlite3GlobalFunctions;
					#endif
					PredefinedFunctions.sqlite3RegisterGlobalFunctions();
					if(sqliteinth.sqlite3GlobalConfig.isPCacheInit==0) {
						rc=PCacheMethods.sqlite3PcacheInitialize();
					}
					if(rc==SqlResult.SQLITE_OK) {
						sqliteinth.sqlite3GlobalConfig.isPCacheInit=1;
						rc=sqlite3_os_init();
					}
					if(rc==SqlResult.SQLITE_OK) {
                        CacheMethods.sqlite3PCacheBufferSetup(sqliteinth.sqlite3GlobalConfig.pPage,sqliteinth.sqlite3GlobalConfig.szPage,sqliteinth.sqlite3GlobalConfig.nPage);
						sqliteinth.sqlite3GlobalConfig.isInit=1;
					}
					sqliteinth.sqlite3GlobalConfig.inProgress=0;
				}
			}
			//sqlite3_mutex_leave( sqliteinth.sqlite3GlobalConfig.pInitMutex );
			///
			///<summary>
			///Go back under the static mutex and clean up the recursive
			///mutex to prevent a resource leak.
			///
			///</summary>
			//sqlite3_mutex_enter( pMaster );
			lock(pMaster) {
				sqliteinth.sqlite3GlobalConfig.nRefInitMutex--;
				if(sqliteinth.sqlite3GlobalConfig.nRefInitMutex<=0) {
					Debug.Assert(sqliteinth.sqlite3GlobalConfig.nRefInitMutex==0);
					//sqlite3_mutex_free( ref sqliteinth.sqlite3GlobalConfig.pInitMutex );
					sqliteinth.sqlite3GlobalConfig.pInitMutex=null;
				}
			}
			//sqlite3_mutex_leave( pMaster );
			///
			///<summary>
			///The following is just a sanity check to make sure SQLite has
			///been compiled correctly.  It is important to run this code, but
			///we don't want to run it too often and soak up CPU cycles for no
			///reason.  So we run it once during initialization.
			///
			///</summary>
#if !NDEBUG
#if !SQLITE_OMIT_FLOATING_POINT
      /* This section of code's only "output" is via Debug.Assert() statements. */
      if ( rc == SqlResult.SQLITE_OK )
      {
        //u64 x = ( ( (u64)1 ) << 63 ) - 1;
        //double y;
        //Debug.Assert( sizeof( u64 ) == 8 );
        //Debug.Assert( sizeof( u64 ) == sizeof( double ) );
        //memcpy( &y, x, 8 );
        //Debug.Assert( MathExtensions.sqlite3IsNaN( y ) );
      }
#endif
#endif
		
			return rc;
		}
		///
		///<summary>
		///Undo the effects of sqlite3_initialize().  Must not be called while
		///there are outstanding database connections or memory allocations or
		///while any part of SQLite is otherwise in use in any thread.  This
		///routine is not threadsafe.  But it is safe to invoke this routine
		///on when SQLite is already shut down.  If SQLite is already shut down
		///</summary>
		///<param name="when this routine is invoked, then this routine is a harmless no">op.</param>
		///<param name=""></param>
		public static SqlResult sqlite3_shutdown() {
			if(sqliteinth.sqlite3GlobalConfig.isInit!=0) {
				sqlite3_os_end();
				Sqlite3ExtensionModule.sqlite3_reset_auto_extension();
				sqliteinth.sqlite3GlobalConfig.isInit=0;
			}
			if(sqliteinth.sqlite3GlobalConfig.isPCacheInit!=0) {
				PCacheMethods.sqlite3PcacheShutdown();
				sqliteinth.sqlite3GlobalConfig.isPCacheInit=0;
			}
			if(sqliteinth.sqlite3GlobalConfig.isMallocInit!=0) {
				malloc_cs.sqlite3MallocEnd();
				sqliteinth.sqlite3GlobalConfig.isMallocInit=0;
			}
			if(sqliteinth.sqlite3GlobalConfig.isMutexInit!=0) {
				sqlite3MutexEnd();
				sqliteinth.sqlite3GlobalConfig.isMutexInit=0;
			}
			return SqlResult.SQLITE_OK;
		}
		
        
    ///<summary>
/// This API allows applications to modify the global configuration of
/// the SQLite library at run-time.
///
/// This routine should only be called when there are no outstanding
/// database connections or memory allocations.  This routine is not
/// threadsafe.  Failure to heed these warnings can lead to unpredictable
/// behavior.
///
///</summary>
    // Overloads for ap assignments
        public static SqlResult sqlite3_config(SqliteConfig op, sqlite3_pcache_methods ap)
    {      //  va_list ap;
      SqlResult rc = SqlResult.SQLITE_OK;
      switch ( op )
      {
          case SqliteConfig.PCACHE:
          {
            /* Specify an alternative malloc implementation */
            sqliteinth.sqlite3GlobalConfig.pcache = ap; //sqlite3GlobalConfig.pcache = (sqlite3_pcache_methods)va_arg(ap, "sqlite3_pcache_methods");
            break;
          }
      }
      return rc;
    }

        public static SqlResult sqlite3_config(SqliteConfig op, ref sqlite3_pcache_methods ap)
    {      //  va_list ap;
      var rc = SqlResult.SQLITE_OK;
      switch ( op )
      {
          case SqliteConfig.GETPCACHE:
          {
            if ( sqliteinth.sqlite3GlobalConfig.pcache.xInit == null )
            {
                            CacheMethods.sqlite3PCacheSetDefault();
            }
            ap = sqliteinth.sqlite3GlobalConfig.pcache;//va_arg(ap, sqlite3_pcache_methods) = sqlite3GlobalConfig.pcache;
            break;
          }
      }
      return rc;
    }

        public static SqlResult sqlite3_config(SqliteConfig op, sqlite3_mem_methods ap)
    {      //  va_list ap;
      var rc = SqlResult.SQLITE_OK;
      switch ( op )
      {
          case SqliteConfig.MALLOC:
          {
            /* Specify an alternative malloc implementation */
            sqliteinth.sqlite3GlobalConfig.m = ap;// (sqlite3_mem_methods)va_arg( ap, "sqlite3_mem_methods" );
            break;
          }
      }
      return rc;
    }

        public static SqlResult sqlite3_config(SqliteConfig op, ref sqlite3_mem_methods ap)
    {      //  va_list ap;
      var rc = SqlResult.SQLITE_OK;
      switch ( op )
      {
          case SqliteConfig.GETMALLOC:
          {
            /* Retrieve the current malloc() implementation */
            //if ( sqlite3GlobalConfig.m.xMalloc == null ) sqlite3MemSetDefault();
            ap = sqliteinth.sqlite3GlobalConfig.m;//va_arg(ap, sqlite3_mem_methods) =  sqlite3GlobalConfig.m;
            break;
          }
      }
      return rc;
    }

		#if SQLITE_THREADSAFE
																																														    static int sqlite3_config( int op, sqlite3_mutex_methods ap )
    {
      //  va_list ap;
      var rc = SqlResult.SQLITE_OK;
      switch ( op )
      {
        case SQLITE_CONFIG_MUTEX:
          {
            /* Specify an alternative mutex implementation */
            sqliteinth.sqlite3GlobalConfig.mutex = ap;// (sqlite3_mutex_methods)_Custom.va_arg( ap, "sqlite3_mutex_methods" );
            break;
          }
      }
      return rc;
    }

    static int sqlite3_config( int op, ref sqlite3_mutex_methods ap )
    {
      //  va_list ap;
      var rc = SqlResult.SQLITE_OK;
      switch ( op )
      {
        case SQLITE_CONFIG_GETMUTEX:
          {
            /* Retrieve the current mutex implementation */
            ap = sqliteinth.sqlite3GlobalConfig.mutex;// *_Custom.va_arg(ap, sqlite3_mutex_methods) =  sqliteinth.sqlite3GlobalConfig.mutex;
            break;
          }
      }
      return rc;
    }
#endif
        public static SqlResult sqlite3_config(SqliteConfig op, params object[] ap)
        {
			//  va_list ap;
			var rc=SqlResult.SQLITE_OK;
			///
			///<summary>
			///sqlite3_config() shall return SQLITE_MISUSE if it is invoked while
			///the SQLite library is in use. 
			///</summary>
			if(sqliteinth.sqlite3GlobalConfig.isInit!=0)
				return sqliteinth.SQLITE_MISUSE_BKPT();
			lock(_Custom.lock_va_list) {
				_Custom.va_start(ap,null);

                switch (op)
                {
				///
				///<summary>
				///Mutex configuration options are only available in a threadsafe
				///compile.
				///
				///</summary>
				#if SQLITE_THREADSAFE
																																																																																												          case SQLITE_CONFIG_SINGLETHREAD:
            {
              /* Disable all mutexing */
              sqliteinth.sqlite3GlobalConfig.bCoreMutex = false;
              sqliteinth.sqlite3GlobalConfig.bFullMutex = false;
              break;
            }
          case SQLITE_CONFIG_MULTITHREAD:
            {
              /* Disable mutexing of database connections */
              /* Enable mutexing of core data structures */
              sqliteinth.sqlite3GlobalConfig.bCoreMutex = true;
              sqliteinth.sqlite3GlobalConfig.bFullMutex = false;
              break;
            }
          case SQLITE_CONFIG_SERIALIZED:
            {
              /* Enable all mutexing */
              sqliteinth.sqlite3GlobalConfig.bCoreMutex = true;
              sqliteinth.sqlite3GlobalConfig.bFullMutex = true;
              break;
            }
          case SQLITE_CONFIG_MUTEX:
            {
              /* Specify an alternative mutex implementation */
              sqliteinth.sqlite3GlobalConfig.mutex = _Custom.va_arg( ap, (sqlite3_mutex_methods)null );
              break;
            }
          case SQLITE_CONFIG_GETMUTEX:
            {
              /* Retrieve the current mutex implementation */
              Debugger.Break(); // TODO -- *_Custom.va_arg(ap, sqlite3_mutex_methods) = sqliteinth.sqlite3GlobalConfig.mutex;
              break;
            }
#endif
				case SqliteConfig.MALLOC: {
					Debugger.Break();
					// TODO --
					///
					///<summary>
					///Specify an alternative malloc implementation 
					///</summary>
					sqliteinth.sqlite3GlobalConfig.m=_Custom.va_arg(ap,(sqlite3_mem_methods)null);
					break;
				}
				case SqliteConfig.GETMALLOC: {
					///
					///<summary>
					///Retrieve the current malloc() implementation 
					///</summary>
					//if ( sqliteinth.sqlite3GlobalConfig.m.xMalloc == null ) sqlite3MemSetDefault();
					//Debugger.Break(); // TODO --//_Custom.va_arg(ap, sqlite3_mem_methods) =  sqliteinth.sqlite3GlobalConfig.m;
					break;
				}
				case SqliteConfig.MEMSTATUS: {
					///
					///<summary>
					///Enable or disable the malloc status collection 
					///</summary>
					sqliteinth.sqlite3GlobalConfig.bMemstat=_Custom.va_arg(ap,(Int32)0)!=0;
					break;
				}
				case SqliteConfig.SCRATCH: {
					///
					///<summary>
					///Designate a buffer for scratch memory space 
					///</summary>
					sqliteinth.sqlite3GlobalConfig.pScratch=_Custom.va_arg(ap,(Byte[][])null);
					sqliteinth.sqlite3GlobalConfig.szScratch=_Custom.va_arg(ap,(Int32)0);
					sqliteinth.sqlite3GlobalConfig.nScratch=_Custom.va_arg(ap,(Int32)0);
					break;
				}
				case SqliteConfig.PAGECACHE: {
					///
					///<summary>
					///Designate a buffer for page cache memory space 
					///</summary>
					sqliteinth.sqlite3GlobalConfig.pPage=_Custom.va_arg(ap,(MemPage)null);
					sqliteinth.sqlite3GlobalConfig.szPage=_Custom.va_arg(ap,(Int32)0);
					sqliteinth.sqlite3GlobalConfig.nPage=_Custom.va_arg(ap,(Int32)0);
					break;
				}
				case SqliteConfig.PCACHE: {
					///
					///<summary>
					///Specify an alternative page cache implementation 
					///</summary>
					Debugger.Break();
					// TODO --sqliteinth.sqlite3GlobalConfig.pcache = (sqlite3_pcache_methods)_Custom.va_arg(ap, "sqlite3_pcache_methods");
					break;
				}
				case SqliteConfig.GETPCACHE: {
					if(sqliteinth.sqlite3GlobalConfig.pcache.xInit==null) {
                                CacheMethods.sqlite3PCacheSetDefault();
					}
					Debugger.Break();
					// TODO -- *_Custom.va_arg(ap, sqlite3_pcache_methods) = sqliteinth.sqlite3GlobalConfig.pcache;
					break;
				}
				#if SQLITE_ENABLE_MEMSYS3 || SQLITE_ENABLE_MEMSYS5
																																																																																												case SQLITE_CONFIG_HEAP: {
/* Designate a buffer for heap memory space */
sqliteinth.sqlite3GlobalConfig.pHeap = _Custom.va_arg(ap, void);
sqliteinth.sqlite3GlobalConfig.nHeap = _Custom.va_arg(ap, int);
sqliteinth.sqlite3GlobalConfig.mnReq = _Custom.va_arg(ap, int);

if( sqliteinth.sqlite3GlobalConfig.mnReq<1 ){
  sqliteinth.sqlite3GlobalConfig.mnReq = 1;
}else if( sqliteinth.sqlite3GlobalConfig.mnReq>(1<<12) ){
  /* cap min request size at 2^12 */
  sqliteinth.sqlite3GlobalConfig.mnReq = (1<<12);
}

if(  sqliteinth.sqlite3GlobalConfig.pHeap==0 ){
/* If the heap pointer is NULL, then restore the malloc implementation
** back to NULL pointers too.  This will cause the malloc to go
** back to its default implementation when sqlite3_initialize() is
** run.
*/
memset(& sqliteinth.sqlite3GlobalConfig.m, 0, sizeof( sqliteinth.sqlite3GlobalConfig.m));
}else{
/* The heap pointer is not NULL, then install one of the
** mem5.c/mem3.c methods. If neither ENABLE_MEMSYS3 nor
** ENABLE_MEMSYS5 is defined, return an error.
*/
#if SQLITE_ENABLE_MEMSYS3
																																																																																												sqliteinth.sqlite3GlobalConfig.m = *sqlite3MemGetMemsys3();
#endif
																																																																																												#if SQLITE_ENABLE_MEMSYS5
																																																																																												sqliteinth.sqlite3GlobalConfig.m = *sqlite3MemGetMemsys5();
#endif
																																																																																												}
break;
}
#endif
				case SqliteConfig.LOOKASIDE: {
					sqliteinth.sqlite3GlobalConfig.szLookaside=_Custom.va_arg(ap,(Int32)0);
					sqliteinth.sqlite3GlobalConfig.nLookaside=_Custom.va_arg(ap,(Int32)0);
					break;
				}
				///
				///<summary>
				///Record a pointer to the logger funcction and its first argument.
				///The default is NULL.  Logging is disabled if the function pointer is
				///NULL.
				///
				///</summary>
				case SqliteConfig.LOG: {
					///
					///<summary>
					///MSVC is picky about pulling func ptrs from va lists.
					///http://support.microsoft.com/kb/47961
					///sqliteinth.sqlite3GlobalConfig.xLog = _Custom.va_arg(ap, void()(void*,int,const char));
					///
					///</summary>
					//typedef void(*LOGFUNC_t)(void*,int,const char);
					sqliteinth.sqlite3GlobalConfig.xLog=_Custom.va_arg(ap,(dxLog)null);
					//"LOGFUNC_t" );
					sqliteinth.sqlite3GlobalConfig.pLogArg=_Custom.va_arg(ap,(Object)null);
					break;
				}
				case SqliteConfig.URI: {
					sqliteinth.sqlite3GlobalConfig.bOpenUri=_Custom.va_arg(ap,(Boolean)true);
					break;
				}
				default: {
					rc=SqlResult.SQLITE_ERROR;
					break;
				}
				}
				_Custom.va_end(ref ap);
			}
			return rc;
		}
		///<summary>
		/// Set up the lookaside buffers for a database connection.
		/// Return SqlResult.SQLITE_OK on success.
		/// If lookaside is already active, return SQLITE_BUSY.
		///
		/// The sz parameter is the number of bytes in each lookaside slot.
		/// The cnt parameter is the number of slots.  If pStart is NULL the
		/// space for the lookaside memory is obtained from sqlite3_malloc().
		/// If pStart is not NULL then it is sz*cnt bytes of memory to use for
		/// the lookaside memory.
		///
		///</summary>
		static SqlResult setupLookaside(Connection db,byte[] pBuf,int sz,int cnt) {
			//void* pStart;
			//if ( db.lookaside.nOut )
			//{
			//  return SQLITE_BUSY;
			//}
			///* Free any existing lookaside buffer for this handle before
			//** allocating a new one so we don't have to have space for
			//** both at the same time.
			//*/
			//if ( db.lookaside.bMalloced )
			//{
			//  //malloc_cs.sqlite3_free( db.lookaside.pStart );
			//}
			///* The size of a lookaside slot needs to be larger than a pointer
			//** to be useful.
			//*/
			//if ( sz <= (int)sizeof( LookasideSlot* ) ) sz = 0;
			//if ( cnt < 0 ) cnt = 0;
			//if ( sz == 0 || cnt == 0 )
			//{
			//  sz = 0;
			//  pStart = 0;
			//}
			//else if ( pBuf == 0 )
			//{
			//  sz = ROUNDDOWN8(sz); /* IMP: R-33038-09382 */
			//  sqlite3BeginBenignMalloc();
			//  pStart = malloc_cs.sqlite3Malloc( sz*cnt );  /* IMP: R-61949-35727 */
			//  sqlite3EndBenignMalloc();
			//}else{
			//  sz = ROUNDDOWN8(sz); /* IMP: R-33038-09382 */
			//  pStart = pBuf;
			//}
			//db.lookaside.pStart = pStart;
			//db.lookaside.pFree = 0;
			//db.lookaside.sz = (u16)sz;
			//if ( pStart )
			//{
			//  int i;
			//  LookasideSlot* p;
			//  Debug.Assert( sz > sizeof( LookasideSlot* ) );
			//  p = (LookasideSlot)pStart;
			//  for ( i = cnt - 1 ; i >= 0 ; i-- )
			//  {
			//    p.pNext = db.lookaside.pFree;
			//    db.lookaside.pFree = p;
			//    p = (LookasideSlot)&( (u8)p )[sz];
			//  }
			//  db.lookaside.pEnd = p;
			//  db.lookaside.bEnabled = 1;
			//  db.lookaside.bMalloced = pBuf == 0 ? 1 : 0;
			//}
			//else
			//{
			//  db.lookaside.pEnd = 0;
			//  db.lookaside.bEnabled = 0;
			//  db.lookaside.bMalloced = 0;
			//}
			return SqlResult.SQLITE_OK;
		}
		///<summary>
		/// Return the mutex associated with a database connection.
		///
		///</summary>
		static sqlite3_mutex sqlite3_db_mutex(Connection db) {
			return db.mutex;
		}
		///<summary>
		/// Configuration settings for an individual database connection
		///
		///</summary>
		static SqlResult sqlite3_db_config(Connection db,int op,params object[] ap) {
			SqlResult rc;
			//va_list ap;
			lock(_Custom.lock_va_list) {
				_Custom.va_start(ap,"");
				switch(op) {
				case SQLITE_DBCONFIG_LOOKASIDE: {
					byte[] pBuf=_Custom.va_arg(ap,(byte[])null);
					///
					///<summary>
					///</summary>
					///<param name="IMP: R">10964 </param>
					int sz=_Custom.va_arg(ap,(Int32)0);
					///
					///<summary>
					///</summary>
					///<param name="IMP: R">25994 </param>
					int cnt=_Custom.va_arg(ap,(Int32)0);
					///
					///<summary>
					///</summary>
					///<param name="IMP: R">53386 </param>
					rc=setupLookaside(db,pBuf,sz,cnt);
					break;
				}
				default: {
					_aFlagOp[] aFlagOp=new _aFlagOp[] {
						new _aFlagOp(SQLITE_DBCONFIG_ENABLE_FKEY,SqliteFlags.SQLITE_ForeignKeys),
						new _aFlagOp(SQLITE_DBCONFIG_ENABLE_TRIGGER,SqliteFlags.SQLITE_EnableTrigger),
					};
					uint i;
					rc=SqlResult.SQLITE_ERROR;
					///
					///<summary>
					///</summary>
					///<param name="IMP: R">23372 </param>
					for(i=0;i<Sqlite3.ArraySize(aFlagOp);i++) {
						if(aFlagOp[i].op==op) {
							int onoff=_Custom.va_arg(ap,(Int32)0);
							int pRes=_Custom.va_arg(ap,(Int32)0);
                            SqliteFlags oldFlags = db.flags;
							if(onoff>0) {
								db.flags=(db.flags|aFlagOp[i].mask);
							}
							else
								if(onoff==0) {
									db.flags=(db.flags&~aFlagOp[i].mask);
								}
							if(oldFlags!=db.flags) {
                                vdbeaux.sqlite3ExpirePreparedStatements(db);
							}
							if(pRes!=0) {
								pRes=(db.flags&aFlagOp[i].mask)!=0?1:0;
							}
							rc=SqlResult.SQLITE_OK;
							break;
						}
					}
					break;
				}
				}
				_Custom.va_end(ref ap);
			}
			return rc;
		}
		///<summary>
		/// Return true if the buffer z[0..n-1] contains all spaces.
		///
		///</summary>
		static bool allSpaces(string z,int iStart,int n) {
			while(n>0&&z[iStart+n-1]==' ') {
				n--;
			}
			return n==0;
		}
		///<summary>
		/// This is the default collating function named "BINARY" which is always
		/// available.
		///
		/// If the padFlag argument is not NULL then space padding at the end
		/// of strings is ignored.  This implements the RTRIM collation.
		///
		///</summary>
		static int binCollFunc(object padFlag,int nKey1,string pKey1,int nKey2,string pKey2) {
			int rc,n;
			n=nKey1<nKey2?nKey1:nKey2;
			rc=_Custom.memcmp(pKey1,pKey2,n);
			if(rc==0) {
				if((int)padFlag!=0&&allSpaces(pKey1,n,nKey1-n)&&allSpaces(pKey2,n,nKey2-n)) {
					///
					///<summary>
					///Leave rc unchanged at 0 
					///</summary>
				}
				else {
					rc=nKey1-nKey2;
				}
			}
			return rc;
		}
		///<summary>
		/// Another built-in collating sequence: NOCASE.
		///
		/// This collating sequence is intended to be used for "case independant
		/// comparison". SQLite's knowledge of upper and lower case equivalents
		/// extends only to the 26 characters used in the English language.
		///
		/// At the moment there is only a UTF-8 implementation.
		///
		///</summary>
		static int nocaseCollatingFunc(object NotUsed,int nKey1,string pKey1,int nKey2,string pKey2) {
			int n=(nKey1<nKey2)?nKey1:nKey2;
			int r=StringExtensions.sqlite3StrNICmp(pKey1,pKey2,(nKey1<nKey2)?nKey1:nKey2);
			sqliteinth.UNUSED_PARAMETER(NotUsed);
			if(0==r) {
				r=nKey1-nKey2;
			}
			return r;
		}
		///<summary>
		/// Return the ROWID of the most recent insert
		///
		///</summary>
		static public sqlite_int64 sqlite3_last_insert_rowid(Connection db) {
			return db.lastRowid;
		}
		///<summary>
		/// Return the number of changes in the most recent call to sqlite3_exec().
		///
		///</summary>
		static public int sqlite3_changes(Connection db) {
			return db.nChange;
		}
		///<summary>
		/// Return the number of changes since the database handle was opened.
		///
		///</summary>
		static public int sqlite3_total_changes(Connection db) {
			return db.nTotalChange;
		}
		///<summary>
		/// Close all open savepoints. This function only manipulates fields of the
		/// database handle object, it does not close any savepoints that may be open
		/// at the b-tree/pager level.
		///
		///</summary>
		public static void sqlite3CloseSavepoints(Connection db) {
			while(db.pSavepoint!=null) {
				Savepoint pTmp=db.pSavepoint;
				db.pSavepoint=pTmp.pNext;
				db.DbFree(ref pTmp);
			}
			db.nSavepoint=0;
			db.nStatement=0;
			db.isTransactionSavepoint=0;
		}
		///<summary>
		/// Invoke the destructor function associated with FuncDef p, if any. Except,
		/// if this is not the last copy of the function, do not invoke it. Multiple
		/// copies of a single function are created when create_function() is called
		/// with SqliteEncoding.ANY as the encoding.
		///
		///</summary>
		static void functionDestroy(Connection db,FuncDef p) {
			FuncDestructor pDestructor=p.pDestructor;
			if(pDestructor!=null) {
				pDestructor.nRef--;
				if(pDestructor.nRef==0) {
					//pDestructor.xDestroy( pDestructor.pUserData );
					db.DbFree(ref pDestructor);
				}
			}
		}
		///<summary>
		/// Close an existing SQLite database
		///
		///</summary>
		public static SqlResult sqlite3_close(Connection db) {
			HashElem i;
			///
			///<summary>
			///Hash table iterator 
			///</summary>
			int j;
			if(db==null) {
				return SqlResult.SQLITE_OK;
			}
			if(!utilc.sqlite3SafetyCheckSickOrOk(db)) {
				return sqliteinth.SQLITE_MISUSE_BKPT();
			}
			db.mutex.Enter();
			///
			///<summary>
			///Force xDestroy calls on all virtual tables 
			///</summary>
			build.sqlite3ResetInternalSchema(db,-1);
			///
			///<summary>
			///If a transaction is open, the ResetInternalSchema() call above
			///will not have called the xDisconnect() method on any virtual
			///</summary>
			///<param name="tables in the db">>aVTrans[] array. The following sqlite3VtabRollback()</param>
			///<param name="call will do so. We need to do this before the check for active">call will do so. We need to do this before the check for active</param>
			///<param name="SQL statements below, as the v">table implementation may be storing</param>
			///<param name="some prepared statements internally.">some prepared statements internally.</param>
			///<param name=""></param>
            VTableMethodsExtensions.sqlite3VtabRollback(db);
			///
			///<summary>
			///If there are any outstanding VMs, return SQLITE_BUSY. 
			///</summary>
			if(db.pVdbe!=null) {
				utilc.sqlite3Error(db,SqlResult.SQLITE_BUSY,"unable to close due to unfinalised statements");
				db.mutex.Exit();
				return SqlResult.SQLITE_BUSY;
			}
			Debug.Assert(utilc.sqlite3SafetyCheckSickOrOk(db));


            var backends = db.Backends.Take(db.BackendCount);

            
			foreach(var backend in backends) {
				Btree pBt=backend.BTree;
				if(pBt!=null&&pBt.sqlite3BtreeIsInBackup()) {
					utilc.sqlite3Error(db, SqlResult.SQLITE_BUSY, "unable to close due to unfinished backup operation");
					db.mutex.Exit();
					return SqlResult.SQLITE_BUSY;
				}
			}
			///
			///<summary>
			///Free any outstanding Savepoint structures. 
			///</summary>
			sqlite3CloseSavepoints(db);
			for(j=0;j<db.BackendCount;j++) {
				DbBackend pDb=db.Backends[j];
				if(pDb.BTree!=null) {
                    BTreeMethods.sqlite3BtreeClose(ref pDb.BTree);
					pDb.BTree=null;
					if(j!=1) {
						pDb.pSchema=null;
					}
				}
			}
			build.sqlite3ResetInternalSchema(db,-1);
			///
			///<summary>
			///Tell the code in notify.c that the connection no longer holds any
			///</summary>
			///<param name="locks and does not require any further unlock">notify callbacks.</param>
			///<param name=""></param>
			sqliteinth.sqlite3ConnectionClosed(db);
			Debug.Assert(db.BackendCount<=2);
			Debug.Assert(db.Backends[0].Equals(db.aDbStatic[0]));
			for(j=0;j<Sqlite3.ArraySize(db.aFunc.a);j++) {
				FuncDef pNext,pHash,p;
				for(p=db.aFunc.a[j];p!=null;p=pHash) {
					pHash=p.pHash;
					while(p!=null) {
						functionDestroy(db,p);
						pNext=p.pNext;
						db.DbFree(ref p);
						p=pNext;
					}
				}
			}
			for(i=db.aCollSeq.first;i!=null;i=i.pNext) {
				//sqliteHashFirst(db.aCollSeq); i!=null; i=sqliteHashNext(i)){
				CollSeq[] pColl=(CollSeq[])i.data;
				// sqliteHashData(i);
				///
				///<summary>
				///Invoke any destructors registered for collation sequence user data. 
				///</summary>
				for(j=0;j<3;j++) {
					if(pColl[j].xDel!=null) {
						pColl[j].xDel(ref pColl[j].pUser);
					}
				}
				db.DbFree(ref pColl);
			}
            db.aCollSeq.sqlite3HashClear();
			#if !SQLITE_OMIT_VIRTUALTABLE
			for(i= db.aModule.sqliteHashFirst();i!=null;i= i.sqliteHashNext()) {
				Module pMod=(Module)i.sqliteHashData();
				if(pMod.xDestroy!=null) {
					pMod.xDestroy(ref pMod.pAux);
				}
				db.DbFree(ref pMod);
			}
            db.aModule.sqlite3HashClear();
			#endif
			utilc.sqlite3Error(db,SqlResult.SQLITE_OK,0);
			///
			///<summary>
			///Deallocates any cached error strings. 
			///</summary>
			if(db.pErr!=null) {
                vdbemem_cs.sqlite3ValueFree(ref db.pErr);
			}
			#if !SQLITE_OMIT_LOAD_EXTENSION
			Sqlite3ExtensionModule.sqlite3CloseExtensions(db);
			#endif
            db.magic = Sqlite3.SQLITE_MAGIC_ERROR;
			///
			///<summary>
			///The temp.database schema is allocated differently from the other schema
			///objects (using sqliteMalloc() directly, instead of sqlite3BtreeSchema()).
			///So it needs to be freed here. Todo: Why not roll the temp schema into
			///the same sqliteMalloc() as the one that allocates the database
			///structure?
			///
			///</summary>
			db.DbFree(ref db.Backends[1].pSchema);
			db.mutex.Exit();
            db.magic = Sqlite3.SQLITE_MAGIC_CLOSED;
			sqlite3_mutex_free(db.mutex);
			Debug.Assert(db.lookaside.nOut==0);
			///
			///<summary>
			///Fails on a lookaside memory leak 
			///</summary>
			//if ( db.lookaside.bMalloced )
			//{
			//  malloc_cs.sqlite3_free( ref db.lookaside.pStart );
			//}
			//malloc_cs.sqlite3_free( ref db );
			return SqlResult.SQLITE_OK;
		}
        ///<summary>
        /// Rollback all database files.
        ///
        ///</summary>
        public static void sqlite3RollbackAll(Connection db) {
            int i;
            int inTrans = 0;
            Debug.Assert(db.mutex.sqlite3_mutex_held());
            sqlite3BeginBenignMalloc();

            db.Backends.Take(db.BackendCount).ForEach(
                    dbFile => {
                        if (dbFile.BTree != null)
                        {
                            if (dbFile.BTree.sqlite3BtreeIsInTrans())
                            {
                                inTrans = 1;
                            }
                            dbFile.BTree.sqlite3BtreeRollback();
                            dbFile.inTrans = DbBackendState.NotWritable;
                        }
                    }
                );
			
            VTableMethodsExtensions.sqlite3VtabRollback(db);
			sqlite3EndBenignMalloc();
            if ((db.flags & SqliteFlags.SQLITE_InternChanges) != 0)
            {
                vdbeaux.sqlite3ExpirePreparedStatements(db);
				build.sqlite3ResetInternalSchema(db,-1);
			}
			///Any deferred constraint violations have now been resolved. 
			db.nDeferredCons=0;
			///If one has been configured, invoke the rollback-hook callback 
			if(db.xRollbackCallback!=null&&(inTrans!=0||0==db.autoCommit)) {
				db.xRollbackCallback(db.pRollbackArg);
			}
		}
		
		///<summary>
		/// This routine implements a busy callback that sleeps and tries
		/// again until a timeout value is reached.  The timeout value is
		/// an integer number of milliseconds passed in as the first
		/// argument.
		///
		///</summary>
		static int sqliteDefaultBusyCallback(object ptr,///
		///<summary>
		///Database connection 
		///</summary>
		int count///
		///<summary>
		///Number of times table has been busy 
		///</summary>
		) {
			#if SQLITE_OS_WIN || HAVE_USLEEP
			u8[] delays=new u8[] {
				1,
				2,
				5,
				10,
				15,
				20,
				25,
				25,
				25,
				50,
				50,
				100
			};
			u8[] totals=new u8[] {
				0,
				1,
				3,
				8,
				18,
				33,
				53,
				78,
				103,
				128,
				178,
				228
			};
			//# define NDELAY Sqlite3.ArraySize(delays)
			int NDELAY=Sqlite3.ArraySize(delays);
			Connection db=(Connection)ptr;
			int timeout=db.busyTimeout;
			int delay,prior;
			Debug.Assert(count>=0);
			if(count<NDELAY) {
				delay=delays[count];
				prior=totals[count];
			}
			else {
				delay=delays[NDELAY-1];
				prior=totals[NDELAY-1]+delay*(count-(NDELAY-1));
			}
			if(prior+delay>timeout) {
				delay=timeout-prior;
				if(delay<=0)
					return 0;
			}
			os.sqlite3OsSleep(db.pVfs,delay*1000);
			return 1;
			#else
																																																																					sqlite3 db = (sqlite3)ptr;
int timeout = ( (sqlite3)ptr ).busyTimeout;
if ( ( count + 1 ) * 1000 > timeout )
{
return 0;
}
sqlite3OsSleep( db.pVfs, 1000000 );
return 1;
#endif
		}
		///<summary>
		/// Invoke the given busy handler.
		///
		/// This routine is called when an operation failed with a lock.
		/// If this routine returns non-zero, the lock is retried.  If it
		/// returns 0, the operation aborts with an SQLITE_BUSY error.
		///
		///</summary>
		public static int sqlite3InvokeBusyHandler(BusyHandler p) {
			int rc;
			if(NEVER(p==null)||p.xFunc==null||p.nBusy<0)
				return 0;
			rc=p.xFunc(p.pArg,p.nBusy);
			if(rc==0) {
				p.nBusy=-1;
			}
			else {
				p.nBusy++;
			}
			return rc;
		}
		///<summary>
		/// This routine sets the busy callback for an Sqlite database to the
		/// given callback function with the given argument.
		///
		///</summary>
		static SqlResult sqlite3_busy_handler(Connection db,dxBusy xBusy,object pArg) {
			db.mutex.Enter();
			db.busyHandler.xFunc=xBusy;
			db.busyHandler.pArg=pArg;
			db.busyHandler.nBusy=0;
			db.mutex.Exit();
			return SqlResult.SQLITE_OK;
		}
		#if !SQLITE_OMIT_PROGRESS_CALLBACK
		///<summary>
		/// This routine sets the progress callback for an Sqlite database to the
		/// given callback function with the given argument. The progress callback will
		/// be invoked every nOps opcodes.
		///</summary>
		static void sqlite3_progress_handler(Connection db,int nOps,dxProgress xProgress,//int (xProgress)(void),
		object pArg) {
			db.mutex.Enter();
			if(nOps>0) {
				db.xProgress=xProgress;
				db.nProgressOps=nOps;
				db.pProgressArg=pArg;
			}
			else {
				db.xProgress=null;
				db.nProgressOps=0;
				db.pProgressArg=null;
			}
			db.mutex.Exit();
		}
		#endif
		///<summary>
		/// This routine installs a default busy handler that waits for the
		/// specified number of milliseconds before returning 0.
		///</summary>
		static public SqlResult sqlite3_busy_timeout(Connection db,int ms) {
			if(ms>0) {
				db.busyTimeout=ms;
				sqlite3_busy_handler(db,sqliteDefaultBusyCallback,db);
			}
			else {
				sqlite3_busy_handler(db,null,null);
			}
			return SqlResult.SQLITE_OK;
		}
		///<summary>
		/// Cause any pending operation to stop at its earliest opportunity.
		///
		///</summary>
		static void sqlite3_interrupt(Connection db) {
			db.u1.isInterrupted=true;
		}
		///<summary>
		/// This function is exactly the same as sqlite3_create_function(), except
		/// that it is designed to be called by internal code. The difference is
		/// that if a malloc() fails in sqlite3_create_function(), an error code
		/// is returned and the mallocFailed flag cleared.
		///
		///</summary>
		public static SqlResult sqlite3CreateFunc(Connection db,string zFunctionName,int nArg,SqliteEncoding enc,object pUserData,dxFunc xFunc,//)(sqlite3_context*,int,sqlite3_value *),
		dxStep xStep,//)(sqlite3_context*,int,sqlite3_value *),
		dxFinal xFinal,//)(sqlite3_context),
		FuncDestructor pDestructor) {
			FuncDef p;
			int nName;
			Debug.Assert(db.mutex.sqlite3_mutex_held());
            if (zFunctionName == null || (xFunc != null && (xFinal != null || xStep != null)) || (xFunc == null && (xFinal != null && xStep == null)) || (xFunc == null && (xFinal == null && xStep != null)) || (nArg < -1 || nArg > Limits.SQLITE_MAX_FUNCTION_ARG) || (255 < (nName = StringExtensions.Strlen30(zFunctionName))))
            {
				return sqliteinth.SQLITE_MISUSE_BKPT();
			}
			#if !SQLITE_OMIT_UTF16
																																																																					/* If SqliteEncoding.UTF16 is specified as the encoding type, transform this
** to one of SqliteEncoding.UTF16LE or SqliteEncoding.UTF16BE using the
** SQLITE_UTF16NATIVE macro. SqliteEncoding.UTF16 is not used internally.
**
** If SqliteEncoding.ANY is specified, add three versions of the function
** to the hash table.
*/
if( enc==SqliteEncoding.UTF16 ){
enc = SqliteEncoding.UTF16NATIVE;
}else if( enc==SqliteEncoding.ANY ){
int rc;
rc = sqlite3CreateFunc(db, zFunctionName, nArg, SqliteEncoding.UTF8,
pUserData, xFunc, xStep, xFinal, pDestructor);
if( rc==SqlResult.SQLITE_OK ){
rc = sqlite3CreateFunc(db, zFunctionName, nArg, SqliteEncoding.UTF16LE,
pUserData, xFunc, xStep, xFinal, pDestructor);
}
if( rc!=SqlResult.SQLITE_OK ){
return rc;
}
enc = SqliteEncoding.UTF16BE;
}
#else
			enc=SqliteEncoding.UTF8;
			#endif
			///
			///<summary>
			///Check if an existing function is being overridden or deleted. If so,
			///and there are active VMs, then return SQLITE_BUSY. If a function
			///is being overridden/deleted but there are no active VMs, allow the
			///operation to continue but invalidate all precompiled statements.
			///</summary>
            p = FuncDefTraverse.FindFunction(db, zFunctionName, nName, nArg, enc, 0);
			if(p!=null&&p.iPrefEnc==enc&&p.nArg==nArg) {
				if(db.activeVdbeCnt!=0) {
					utilc.sqlite3Error(db, SqlResult.SQLITE_BUSY, "unable to delete/modify user-function due to active statements");
					//Debug.Assert( 0 == db.mallocFailed );
					return SqlResult.SQLITE_BUSY;
				}
				else {
                    vdbeaux.sqlite3ExpirePreparedStatements(db);
				}
			}
            p = FuncDefTraverse.FindFunction(db, zFunctionName, nName, nArg, enc, 1);
			Debug.Assert(p!=null///
			///<summary>
			///|| db.mallocFailed != 0 
			///</summary>
			);
			//if ( p == null )
			//{
			//  return SQLITE_NOMEM;
			//}
			///
			///<summary>
			///If an older version of the function with a configured destructor is
			///being replaced invoke the destructor function here. 
			///</summary>
			functionDestroy(db,p);
			if(pDestructor!=null) {
				pDestructor.nRef++;
			}
			p.pDestructor=pDestructor;
			p.flags=0;
			p.xFunc=xFunc;
			p.xStep=xStep;
			p.xFinalize=xFinal;
			p.pUserData=pUserData;
			p.nArg=(i16)nArg;
			return SqlResult.SQLITE_OK;
		}
		///<summary>
		/// Create new user functions.
		///
		///</summary>
		static public SqlResult sqlite3_create_function(Connection db,string zFunc,int nArg,SqliteEncoding enc,object p,dxFunc xFunc,//)(sqlite3_context*,int,sqlite3_value *),
		dxStep xStep,//)(sqlite3_context*,int,sqlite3_value *),
		dxFinal xFinal//)(sqlite3_context)
		) {
			return sqlite3_create_function_v2(db,zFunc,nArg,enc,p,xFunc,xStep,xFinal,null);
		}
		static SqlResult sqlite3_create_function_v2(Connection db,string zFunc,int nArg,SqliteEncoding enc,object p,dxFunc xFunc,//)(sqlite3_context*,int,sqlite3_value *),
		dxStep xStep,//)(sqlite3_context*,int,sqlite3_value *),
		dxFinal xFinal,//)(sqlite3_context)
		dxFDestroy xDestroy//)(void )
		) {
			var rc=SqlResult.SQLITE_ERROR;
			FuncDestructor pArg=null;
			db.mutex.Enter();
			if(xDestroy!=null) {
				pArg=new FuncDestructor();
				//(FuncDestructor )sqlite3DbMallocZero(db, sizeof(FuncDestructor));
				//if( null==pArg ){
				//  xDestroy(p);
				//  goto out;
				//}
				pArg.xDestroy=xDestroy;
				pArg.pUserData=p;
			}
			rc=sqlite3CreateFunc(db,zFunc,nArg,enc,p,xFunc,xStep,xFinal,pArg);
			if(pArg!=null&&pArg.nRef==0) {
				Debug.Assert(rc!=SqlResult.SQLITE_OK);
				//xDestroy(p);
				db.DbFree(ref pArg);
			}
			//_out:
			rc=malloc_cs.sqlite3ApiExit(db,rc);
			db.mutex.Exit();
			return rc;
		}
		#if !SQLITE_OMIT_UTF16
																																														static int sqlite3_create_function16(
sqlite3 db,
string zFunctionName,
int nArg,
int eTextRep,
object p,
dxFunc xFunc,   //)(sqlite3_context*,int,sqlite3_value*),
dxStep xStep,   //)(sqlite3_context*,int,sqlite3_value*),
dxFinal xFinal  //)(sqlite3_context)
){
int rc;
string zFunc8;
db.mutex.sqlite3_mutex_enter();
Debug.Assert( 0==db.mallocFailed );
zFunc8 = sqlite3Utf16to8(db, zFunctionName, -1, SqliteEncoding.UTF16NATIVE);
rc = sqlite3CreateFunc(db, zFunc8, nArg, eTextRep, p, xFunc, xStep, xFinal, null);
sqlite3DbFree(db,ref zFunc8);
rc = malloc_cs.sqlite3ApiExit(db, rc);
db.mutex.sqlite3_mutex_leave();
return rc;
}
#endif
		///<summary>
		/// Declare that a function has been overloaded by a virtual table.
		///
		/// If the function already exists as a regular global function, then
		/// this routine is a no-op.  If the function does not exist, then create
		/// a new one that always throws a run-time error.
		///
		/// When virtual tables intend to provide an overloaded function, they
		/// should call this routine to make sure the global function exists.
		/// A global function must exist in order for name resolution to work
		/// properly.
		///</summary>
		public static SqlResult sqlite3_overload_function(Connection db,string zName,int nArg) {
			int nName=StringExtensions.Strlen30(zName);
            SqlResult rc;
			db.mutex.Enter();
            if (FuncDefTraverse.FindFunction(db, zName, nName, nArg, SqliteEncoding.UTF8, 0) == null)
            {
                sqlite3CreateFunc(db, zName, nArg, SqliteEncoding.UTF8, 0, (dxFunc)vdbeapi.sqlite3InvalidFunction, null, null, null);
			}
			rc=malloc_cs.sqlite3ApiExit(db,SqlResult.SQLITE_OK);
			db.mutex.Exit();
			return rc;
		}
		#if !SQLITE_OMIT_TRACE
		///<summary>
		/// Register a trace function.  The pArg from the previously registered trace
		/// is returned.
		///
		/// A NULL trace function means that no tracing is executes.  A non-NULL
		/// trace is a pointer to a function that is invoked at the start of each
		/// SQL statement.
		///</summary>
		static object sqlite3_trace(Connection db,dxTrace xTrace,object pArg) {
			// (*xTrace)(void*,const char), object pArg){
			object pOld;
			db.mutex.Enter();
			pOld=db.pTraceArg;
			db.xTrace=xTrace;
			db.pTraceArg=pArg;
			db.mutex.Exit();
			return pOld;
		}
		///<summary>
		/// Register a profile function.  The pArg from the previously registered
		/// profile function is returned.
		///
		/// A NULL profile function means that no profiling is executes.  A non-NULL
		/// profile is a pointer to a function that is invoked at the conclusion of
		/// each SQL statement that is run.
		///
		///</summary>
		static object sqlite3_profile(Connection db,dxProfile xProfile,//void (*xProfile)(void*,const char*,sqlite_u3264),
		object pArg) {
			object pOld;
			db.mutex.Enter();
			pOld=db.pProfileArg;
			db.xProfile=xProfile;
			db.pProfileArg=pArg;
			db.mutex.Exit();
			return pOld;
		}
		#endif
		///<summary>
		/// EXPERIMENTAL 
		///
		/// Register a function to be invoked when a transaction comments.
		/// If the invoked function returns non-zero, then the commit becomes a
		/// rollback.
		///</summary>
		static object sqlite3_commit_hook(Connection db,///
		///<summary>
		///Attach the hook to this database 
		///</summary>
		dxCommitCallback xCallback,//int (*xCallback)(void),  /* Function to invoke on each commit */
		object pArg///
		///<summary>
		///Argument to the function 
		///</summary>
		) {
			object pOld;
			db.mutex.Enter();
			pOld=db.pCommitArg;
			db.xCommitCallback=xCallback;
			db.pCommitArg=pArg;
			db.mutex.Exit();
			return pOld;
		}
		///<summary>
		/// Register a callback to be invoked each time a row is updated,
		/// inserted or deleted using this database connection.
		///
		///</summary>
		static object sqlite3_update_hook(Connection db,///
		///<summary>
		///Attach the hook to this database 
		///</summary>
		dxUpdateCallback xCallback,//void (*xCallback)(void*,int,char const *,char const *,sqlite_int64),
		object pArg///
		///<summary>
		///Argument to the function 
		///</summary>
		) {
			object pRet;
			db.mutex.Enter();
			pRet=db.pUpdateArg;
			db.xUpdateCallback=xCallback;
			db.pUpdateArg=pArg;
			db.mutex.Exit();
			return pRet;
		}
		///<summary>
		/// Register a callback to be invoked each time a transaction is rolled
		/// back by this database connection.
		///
		///</summary>
		static object sqlite3_rollback_hook(Connection db,///
		///<summary>
		///Attach the hook to this database 
		///</summary>
		dxRollbackCallback xCallback,//void (*xCallback)(void), /* Callback function */
		object pArg///
		///<summary>
		///Argument to the function 
		///</summary>
		) {
			object pRet;
			db.mutex.Enter();
			pRet=db.pRollbackArg;
			db.xRollbackCallback=xCallback;
			db.pRollbackArg=pArg;
			db.mutex.Exit();
			return pRet;
		}
		#if !SQLITE_OMIT_WAL
																																														///<summary>
/// The sqlite3_wal_hook() callback registered by sqlite3_wal_autocheckpoint().
/// Invoke sqlite3_wal_checkpoint if the number of frames in the log file
/// is greater than sqlite3.pWalArg cast to an integer (the value configured by
/// wal_autocheckpoint()).
///</summary> 
int sqlite3WalDefaultHook(
void *pClientData,     /* Argument */
sqlite3 db,           /* Connection */
const string zDb,       /* Database */
int nFrame             /* Size of WAL */
){
if( nFrame>=SQLITE_PTR_TO_INT(pClientData) ){
sqlite3BeginBenignMalloc();
sqlite3_wal_checkpoint(db, zDb);
sqlite3EndBenignMalloc();
}
return SqlResult.SQLITE_OK;
}
#endif
		///<summary>
		/// Configure an sqlite3_wal_hook() callback to automatically checkpoint
		/// a database after committing a transaction if there are nFrame or
		/// more frames in the log file. Passing zero or a negative value as the
		/// nFrame parameter disables automatic checkpoints entirely.
		///
		/// The callback registered by this function replaces any existing callback
		/// registered using sqlite3_wal_hook(). Likewise, registering a callback
		/// using sqlite3_wal_hook() disables the automatic checkpoint mechanism
		/// configured by this function.
		///</summary>
		static SqlResult sqlite3_wal_autocheckpoint(Connection db,int nFrame) {
			#if SQLITE_OMIT_WAL
			sqliteinth.UNUSED_PARAMETER(db);
			sqliteinth.UNUSED_PARAMETER(nFrame);
			#else
																																																																					if( nFrame>0 ){
sqlite3_wal_hook(db, sqlite3WalDefaultHook, SQLITE_INT_TO_PTR(nFrame));
}else{
sqlite3_wal_hook(db, 0, 0);
}
#endif
			return SqlResult.SQLITE_OK;
		}
		///<summary>
		/// Register a callback to be invoked each time a transaction is written
		/// into the write-ahead-log by this database connection.
		///
		///</summary>
		static object sqlite3_wal_hook(Connection db,///
		///<summary>
		///Attach the hook to this db handle 
		///</summary>
		dxWalCallback xCallback,//int(*xCallback)(void *, sqlite3*, const char*, int),
		object pArg///
		///<summary>
		///First argument passed to xCallback() 
		///</summary>
		) {
			#if !SQLITE_OMIT_WAL
																																																																					void *pRet;
db.mutex.sqlite3_mutex_enter();
pRet = db.pWalArg;
db.xWalCallback = xCallback;
db.pWalArg = pArg;
db.mutex.sqlite3_mutex_leave();
return pRet;
#else
			return null;
			#endif
		}
		///<summary>
		/// Checkpoint database zDb.
		///
		///</summary>
		static SqlResult sqlite3_wal_checkpoint_v2(Connection db,///
		///<summary>
		///Database handle 
		///</summary>
		string zDb,///
		///<summary>
		///Name of attached database (or NULL) 
		///</summary>
		int eMode,///
		///<summary>
		///SQLITE_CHECKPOINT_* value 
		///</summary>
		out int pnLog,///
		///<summary>
		///OUT: Size of WAL log in frames 
		///</summary>
		out int pnCkpt///
		///<summary>
		///OUT: Total number of frames checkpointed 
		///</summary>
		) {
			#if SQLITE_OMIT_WAL
			pnLog=0;
			pnCkpt=0;
			return SqlResult.SQLITE_OK;
			#else
																																																																					  int rc;                         /* Return code */
  int iDb = SQLITE_MAX_ATTACHED;  /* sqlite3.aDb[] index of db to checkpoint */

  /* Initialize the output variables to -1 in case an error occurs. */
  if( pnLog ) *pnLog = -1;
  if( pnCkpt ) *pnCkpt = -1;

  Debug.Assert( SQLITE_CHECKPOINT_FULL>SQLITE_CHECKPOINT_PASSIVE );
  Debug.Assert( SQLITE_CHECKPOINT_FULL<SQLITE_CHECKPOINT_RESTART );
  Debug.Assert( SQLITE_CHECKPOINT_PASSIVE+2==SQLITE_CHECKPOINT_RESTART );
  if( eMode<SQLITE_CHECKPOINT_PASSIVE || eMode>SQLITE_CHECKPOINT_RESTART ){
    return SQLITE_MISUSE;
  }

  sqlite3_mutex_enter(db->mutex);
  if( zDb && zDb[0] ){
    iDb = build.sqlite3FindDbName(db, zDb);
  }
  if( iDb<0 ){
    rc = SqlResult.SQLITE_ERROR;
    utilc.sqlite3Error(db, SqlResult.SQLITE_ERROR, "unknown database: %s", zDb);
  }else{
    rc = sqlite3Checkpoint(db, iDb, eMode, pnLog, pnCkpt);
    utilc.sqlite3Error(db, rc, 0);
  }
  rc = malloc_cs.sqlite3ApiExit(db, rc);
  sqlite3_mutex_leave(db->mutex);
  return rc;
#endif
		}
		///<summary>
		/// Checkpoint database zDb. If zDb is NULL, or if the buffer zDb points
		/// to contains a zero-length string, all attached databases are
		/// checkpointed.
		///
		///</summary>
		static SqlResult sqlite3_wal_checkpoint(Connection db,string zDb) {
			int dummy;
			return sqlite3_wal_checkpoint_v2(db,zDb,SQLITE_CHECKPOINT_PASSIVE,out dummy,out dummy);
		}
		#if !SQLITE_OMIT_WAL
																																														///<summary>
/// Run a checkpoint on database iDb. This is a no-op if database iDb is
/// not currently open in WAL mode.
///
/// If a transaction is open on the database being checkpointed, this
/// function returns SQLITE_LOCKED and a checkpoint is not attempted. If
/// an error occurs while running the checkpoint, an SQLite error code is
/// returned (i.e. SQLITE_IOERR). Otherwise, SqlResult.SQLITE_OK.
///
/// The mutex on database handle db should be held by the caller. The mutex
/// associated with the specific b-tree being checkpointed is taken by
/// this function while the checkpoint is running.
///
/// If iDb is passed SQLITE_MAX_ATTACHED, then all attached databases are
/// checkpointed. If an error is encountered it is returned immediately -
/// no attempt is made to checkpoint any remaining databases.
///
/// Parameter eMode is one of SQLITE_CHECKPOINT_PASSIVE, FULL or RESTART.
///</summary>
int sqlite3Checkpoint(sqlite3 db, int iDb, int eMode, int *pnLog, int *pnCkpt){
  var rc = SqlResult.SQLITE_OK;             /* Return code */
  int i;                          /* Used to iterate through attached dbs */
  int bBusy = 0;                  /* True if SQLITE_BUSY has been encountered */

  Debug.Assert( Sqlite3.sqlite3_mutex_held(db->mutex) );
  Debug.Assert( !pnLog || *pnLog==-1 );
  Debug.Assert( !pnCkpt || *pnCkpt==-1 );

  for(i=0; i<db->nDb && rc==SqlResult.SQLITE_OK; i++){
    if( i==iDb || iDb==SQLITE_MAX_ATTACHED ){
      rc = sqlite3BtreeCheckpoint(db->aDb[i].pBt, eMode, pnLog, pnCkpt);
      pnLog = 0;
      pnCkpt = 0;
      if( rc==SQLITE_BUSY ){
        bBusy = 1;
        rc = SqlResult.SQLITE_OK;
      }
    }
  }

  return (rc==SqlResult.SQLITE_OK && bBusy) ? SQLITE_BUSY : rc;
}
#endif
		///<summary>
		////
		/// This function returns true if main-memory should be used instead of
		/// a temporary file for transient pager files and statement journals.
		/// The value returned depends on the value of db->temp_store (runtime
		/// parameter) and the compile time value of SQLITE_TEMP_STORE. The
		/// following table describes the relationship between these two values
		/// and this functions return value.
		///
		///   SQLITE_TEMP_STORE     db->temp_store     Location of temporary database
		///   -----------------     --------------     ------------------------------
		///   0                     any                file      (return 0)
		///   1                     1                  file      (return 0)
		///   1                     2                  memory    (return 1)
		///   1                     0                  file      (return 0)
		///   2                     1                  file      (return 0)
		///   2                     2                  memory    (return 1)
		///   2                     0                  memory    (return 1)
		///   3                     any                memory    (return 1)
		///
		///</summary>
        public  static bool sqlite3TempInMemory(Connection db)
        {
			//#if SQLITE_TEMP_STORE==1
            if (sqliteinth.SQLITE_TEMP_STORE == 1)
				return (db.temp_store==2);
			//#endif
			//#if SQLITE_TEMP_STORE==2
            if (sqliteinth.SQLITE_TEMP_STORE == 2)
				return (db.temp_store!=1);
			//#endif
			//#if SQLITE_TEMP_STORE==3
            if (sqliteinth.SQLITE_TEMP_STORE == 3)
				return true;
			//#endif
			//#if SQLITE_TEMP_STORE<1 || SQLITE_TEMP_STORE>3
            if (sqliteinth.SQLITE_TEMP_STORE < 1 || sqliteinth.SQLITE_TEMP_STORE > 3)
				return false;
			//#endif
			return false;
		}
		///
		///<summary>
		///</summary>
		///<param name="Return UTF">8 encoded English language explanation of the most recent</param>
		///<param name="error.">error.</param>
		///<param name=""></param>
		public static string sqlite3_errmsg(Connection db) {
			string z;
			if(db==null) {
				return SqlResult.SQLITE_NOMEM.sqlite3ErrStr();
			}
			if(!utilc.sqlite3SafetyCheckSickOrOk(db)) {
				return sqliteinth.SQLITE_MISUSE_BKPT().sqlite3ErrStr();
			}
			db.mutex.Enter();
			//if ( db.mallocFailed != 0 )
			//{
			//  z = sqlite3ErrStr( SQLITE_NOMEM );
			//}
			//else
			{
				z=vdbeapi.sqlite3_value_text(db.pErr);
				//Debug.Assert( 0 == db.mallocFailed );
				if(String.IsNullOrEmpty(z)) {
					z= db.errCode.sqlite3ErrStr();
				}
			}
			db.mutex.Exit();
			return z;
		}
		#if !SQLITE_OMIT_UTF16
																																														/*
** Return UTF-16 encoded English language explanation of the most recent
** error.
*/
const void *sqlite3_errmsg16(sqlite3 db){
static const u16 outOfMem[] = {
'o', 'u', 't', ' ', 'o', 'f', ' ', 'm', 'e', 'm', 'o', 'r', 'y', 0
};
static const u16 misuse[] = {
'l', 'i', 'b', 'r', 'a', 'r', 'y', ' ',
'r', 'o', 'u', 't', 'i', 'n', 'e', ' ',
'c', 'a', 'l', 'l', 'e', 'd', ' ',
'o', 'u', 't', ' ',
'o', 'f', ' ',
's', 'e', 'q', 'u', 'e', 'n', 'c', 'e', 0
};

string z;
if( null==db ){
return (void )outOfMem;
}
if( null==utilc.sqlite3SafetyCheckSickOrOk(db) ){
return (void )misuse;
}
sqlite3_mutex_enter(db->mutex);
if( db->mallocFailed ){
z = (void )outOfMem;
}else{
z = vdbeapi.sqlite3_value_text16(db->pErr);
if( z==0 ){
sqlite3ValueSetStr(db->pErr, -1, sqlite3ErrStr(db->errCode),
SqliteEncoding.UTF8, SQLITE_STATIC);
z = vdbeapi.sqlite3_value_text16(db->pErr);
}
/* A malloc() may have failed within the call to vdbeapi.sqlite3_value_text16()
** above. If this is the case, then the db->mallocFailed flag needs to
** be cleared before returning. Do this directly, instead of via
** malloc_cs.sqlite3ApiExit(), to avoid setting the database handle error message.
*/
db->mallocFailed = 0;
}
sqlite3_mutex_leave(db->mutex);
return z;
}
#endif
		///
		///<summary>
		///Return the most recent error code generated by an SQLite routine. If NULL is
		///passed to this function, we assume a malloc() failed during sqlite3_open().
		///</summary>
		static public SqlResult sqlite3_errcode(Connection db) {
			if(db!=null&&!utilc.sqlite3SafetyCheckSickOrOk(db)) {
				return sqliteinth.SQLITE_MISUSE_BKPT();
			}
			if(null==db///
			///<summary>
			///|| db.mallocFailed != 0 
			///</summary>
			) {
				return SqlResult.SQLITE_NOMEM;
			}
			return db.errCode&db.errMask;
		}
		static SqlResult sqlite3_extended_errcode(Connection db) {
			if(db!=null&&!utilc.sqlite3SafetyCheckSickOrOk(db)) {
				return sqliteinth.SQLITE_MISUSE_BKPT();
			}
			if(null==db///
			///<summary>
			///|| db.mallocFailed != 0 
			///</summary>
			) {
				return SqlResult.SQLITE_NOMEM;
			}
			return db.errCode;
		}
		///
		///<summary>
		///Create a new collating function for database "db".  The name is zName
		///and the encoding is enc.
		///
		///</summary>
		static SqlResult createCollation(Connection db,string zName,SqliteEncoding enc,CollationType collType,object pCtx,dxCompare xCompare,//)(void*,int,const void*,int,const void),
		dxDelCollSeq xDel//)(void)
		) {
			CollSeq pColl;
			SqliteEncoding enc2;
			int nName=StringExtensions.Strlen30(zName);
			Debug.Assert(db.mutex.sqlite3_mutex_held());
			///If SqliteEncoding.UTF16 is specified as the encoding type, transform this
			///to one of SqliteEncoding.UTF16LE or SqliteEncoding.UTF16BE using the
			///SqliteEncoding.UTF16NATIVE macro. SqliteEncoding.UTF16 is not used internally.
			enc2=enc;
			sqliteinth.testcase(enc2==SqliteEncoding.UTF16);
			sqliteinth.testcase(enc2==SqliteEncoding.UTF16_ALIGNED);
			if(enc2==SqliteEncoding.UTF16||enc2==SqliteEncoding.UTF16_ALIGNED) {
				enc2=SqliteEncoding.UTF16NATIVE;
			}
			if(enc2<SqliteEncoding.UTF8||enc2>SqliteEncoding.UTF16BE) {
				return sqliteinth.SQLITE_MISUSE_BKPT();
			}
			///Check if this call is removing or replacing an existing collation
			///sequence. If so, and there are active VMs, return busy. If there
			///are no active VMs, invalidate any pre-compiled statements.
			pColl=db.sqlite3FindCollSeq(enc2,zName,0);
			if(pColl!=null&&pColl.xCmp!=null) {
				if(db.activeVdbeCnt!=0) {
					utilc.sqlite3Error(db, SqlResult.SQLITE_BUSY, "unable to delete/modify collation sequence due to active statements");
					return SqlResult.SQLITE_BUSY;
				}
                vdbeaux.sqlite3ExpirePreparedStatements(db);
				///
				///<summary>
				///If collation sequence pColl was created directly by a call to
				///sqlite3_create_collation, and not generated by synthCollSeq(),
				///then any copies made by synthCollSeq() need to be invalidated.
				///</summary>
				///<param name="Also, collation destructor "> function may need</param>
				///<param name="to be called.">to be called.</param>
				///<param name=""></param>
				if((pColl.enc&~SqliteEncoding.UTF16_ALIGNED)==enc2) {
					CollSeq[] aColl=db.aCollSeq.Find(zName,nName,(CollSeq[])null);
					int j;
					for(j=0;j<3;j++) {
						CollSeq p=aColl[j];
						if(p.enc==pColl.enc) {
							if(p.xDel!=null) {
								p.xDel(ref p.pUser);
							}
							p.xCmp=null;
						}
					}
				}
			}
			pColl=db.sqlite3FindCollSeq(enc2,zName,1);
			//if ( pColl == null )
			//  return SQLITE_NOMEM;
			pColl.xCmp=xCompare;
			pColl.pUser=pCtx;
			pColl.xDel=xDel;
			pColl.enc=(enc2|(enc&SqliteEncoding.UTF16_ALIGNED));
			pColl.type=collType;
			utilc.sqlite3Error(db,SqlResult.SQLITE_OK,0);
			return SqlResult.SQLITE_OK;
		}
		///
		///<summary>
		///This array defines hard upper bounds on limit values.  The
		///initializer must be kept in sync with the SQLITE_LIMIT_*
		///#defines in sqlite3.h.
		///
		///</summary>
		static int[] aHardLimit=new int[] {
			Limits.SQLITE_MAX_LENGTH,
			Limits.SQLITE_MAX_SQL_LENGTH,
			Limits.SQLITE_MAX_COLUMN,
			Limits.SQLITE_MAX_EXPR_DEPTH,
			Limits.SQLITE_MAX_COMPOUND_SELECT,
			Limits.SQLITE_MAX_VDBE_OP,
			Limits.SQLITE_MAX_FUNCTION_ARG,
			Limits.SQLITE_MAX_ATTACHED,
			Limits.SQLITE_MAX_LIKE_PATTERN_LENGTH,
			Limits.SQLITE_MAX_VARIABLE_NUMBER,
			Limits.SQLITE_MAX_TRIGGER_DEPTH,
		};
		///
		///<summary>
		///Make sure the hard limits are set to reasonable values
		///
		///</summary>
		//#if SQLITE_MAX_LENGTH<100
		//# error SQLITE_MAX_LENGTH must be at least 100
		//#endif
		//#if SQLITE_MAX_SQL_LENGTH<100
		//# error SQLITE_MAX_SQL_LENGTH must be at least 100
		//#endif
		//#if SQLITE_MAX_SQL_LENGTH>SQLITE_MAX_LENGTH
		//# error SQLITE_MAX_SQL_LENGTH must not be greater than SQLITE_MAX_LENGTH
		//#endif
		//#if SQLITE_MAX_COMPOUND_SELECT<2
		//# error SQLITE_MAX_COMPOUND_SELECT must be at least 2
		//#endif
		//#if SQLITE_MAX_VDBE_OP<40
		//# error SQLITE_MAX_VDBE_OP must be at least 40
		//#endif
		//#if SQLITE_MAX_FUNCTION_ARG<0 || SQLITE_MAX_FUNCTION_ARG>1000
		//# error SQLITE_MAX_FUNCTION_ARG must be between 0 and 1000
		//#endif
		//#if SQLITE_MAX_ATTACHED<0 || SQLITE_MAX_ATTACHED>62
		//# error SQLITE_MAX_ATTACHED must be between 0 and 62
		//#endif
		//#if SQLITE_MAX_LIKE_PATTERN_LENGTH<1
		//# error SQLITE_MAX_LIKE_PATTERN_LENGTH must be at least 1
		//#endif
		//#if SQLITE_MAX_COLUMN>32767
		//# error SQLITE_MAX_COLUMN must not exceed 32767
		//#endif
		//#if SQLITE_MAX_TRIGGER_DEPTH<1
		//# error SQLITE_MAX_TRIGGER_DEPTH must be at least 1
		//#endif
		///
		///<summary>
		///Change the value of a limit.  Report the old value.
		///</summary>
		///<param name="If an invalid limit index is supplied, report ">1.</param>
		///<param name="Make no changes but still report the old value if the">Make no changes but still report the old value if the</param>
		///<param name="new limit is negative.">new limit is negative.</param>
		///<param name=""></param>
		///<param name="A new lower limit does not shrink existing constructs.">A new lower limit does not shrink existing constructs.</param>
		///<param name="It merely prevents new constructs that exceed the limit">It merely prevents new constructs that exceed the limit</param>
		///<param name="from forming.">from forming.</param>
		///<param name=""></param>
		static int sqlite3_limit(Connection db,int limitId,int newLimit) {
			int oldLimit;
			///
			///<summary>
			///</summary>
			///<param name="EVIDENCE">54097 For each limit category SQLITE_LIMIT_NAME</param>
			///<param name="there is a hard upper bound set at compile">time by a C preprocessor</param>
			///<param name="macro called SQLITE_MAX_NAME. (The "_LIMIT_" in the name is changed to">macro called SQLITE_MAX_NAME. (The "_LIMIT_" in the name is changed to</param>
			///<param name=""_MAX_".)">"_MAX_".)</param>
			///<param name=""></param>
			Debug.Assert(aHardLimit[Globals.SQLITE_LIMIT_LENGTH]==Limits.SQLITE_MAX_LENGTH);
            Debug.Assert(aHardLimit[Globals.SQLITE_LIMIT_SQL_LENGTH] == Limits.SQLITE_MAX_SQL_LENGTH);
			Debug.Assert(aHardLimit[Globals.SQLITE_LIMIT_COLUMN]==Limits.SQLITE_MAX_COLUMN);
            Debug.Assert(aHardLimit[Globals.SQLITE_LIMIT_EXPR_DEPTH] == Limits.SQLITE_MAX_EXPR_DEPTH);
            Debug.Assert(aHardLimit[Globals.SQLITE_LIMIT_COMPOUND_SELECT] == Limits.SQLITE_MAX_COMPOUND_SELECT);
            Debug.Assert(aHardLimit[Globals.SQLITE_LIMIT_VDBE_OP] == Limits.SQLITE_MAX_VDBE_OP);
            Debug.Assert(aHardLimit[Globals.SQLITE_LIMIT_FUNCTION_ARG] == Limits.SQLITE_MAX_FUNCTION_ARG);
            Debug.Assert(aHardLimit[Globals.SQLITE_LIMIT_ATTACHED] == Limits.SQLITE_MAX_ATTACHED);
            Debug.Assert(aHardLimit[Globals.SQLITE_LIMIT_LIKE_PATTERN_LENGTH] == Limits.SQLITE_MAX_LIKE_PATTERN_LENGTH);
            Debug.Assert(aHardLimit[Globals.SQLITE_LIMIT_VARIABLE_NUMBER] == Limits.SQLITE_MAX_VARIABLE_NUMBER);
            Debug.Assert(aHardLimit[Globals.SQLITE_LIMIT_TRIGGER_DEPTH] == Limits.SQLITE_MAX_TRIGGER_DEPTH);
            Debug.Assert(Globals.SQLITE_LIMIT_TRIGGER_DEPTH == (sqliteinth.SQLITE_N_LIMIT - 1));
			if(limitId<0||limitId>=sqliteinth.SQLITE_N_LIMIT) {
				return -1;
			}
			oldLimit=db.aLimit[limitId];
			if(newLimit>=0)///
			///<summary>
			///</summary>
			///<param name="IMP: R">28732 </param>
			 {
				if(newLimit>aHardLimit[limitId]) {
					newLimit=aHardLimit[limitId];
					///
					///<summary>
					///</summary>
					///<param name="IMP: R">25634 </param>
				}
				db.aLimit[limitId]=newLimit;
			}
			return oldLimit;
			///
			///<summary>
			///</summary>
			///<param name="IMP: R">35419 </param>
		}
		
		///
		///<summary>
		///This function is used to parse both URIs and non-URI filenames passed by the
		///user to API functions sqlite3_open() or sqlite3_open_v2(), and for database
		///URIs specified as part of ATTACH statements.
        ///
		///The first argument to this function is the name of the VFS to use (or
		///a NULL to signify the default VFS) if the URI does not contain a "vfs=xxx""
		///query parameter. The second argument contains the URI (or non-URI filename)
		///itself. When this function is called the *pFlags variable should contain
		///the default flags to open the database handle with. The value stored in
		///pFlags may be updated before returning if the URI filename contains 
		///"cache=xxx" or "mode=xxx" query parameters.
        ///
		///If successful, SqlResult.SQLITE_OK is returned. In this case *ppVfs is set to point to
		///the VFS that should be used to open the database file. *pzFile is set to
		///point to a buffer containing the name of the file to open. It is the 
		///responsibility of the caller to eventually call malloc_cs.sqlite3_free() to release
		///this buffer.
        ///
		///If an error occurs, then an SQLite error code is returned and *pzErrMsg">If an error occurs, then an SQLite error code is returned and *pzErrMsg</param>
		///<param name="may be set to point to a buffer containing an English language error ">may be set to point to a buffer containing an English language error </param>
		///<param name="message. It is the responsibility of the caller to eventually release">message. It is the responsibility of the caller to eventually release</param>
		///<param name="this buffer by calling malloc_cs.sqlite3_free().">this buffer by calling malloc_cs.sqlite3_free().</param>
        ///</summary>
        static SqlResult sqlite3ParseUri(
            string zDefaultVfs,///VFS to use if no "vfs=xxx" query option 
		    string zUri,///Nul-terminated URI to parse
		    ref int pFlags,///IN/OUT: SQLITE_OPEN_XXX flags 
		    ref sqlite3_vfs ppVfs,///OUT: VFS to use 
		    ref string pzFile,///OUT: Filename component of URI 
		    ref string pzErrMsg///OUT: Error message (if rc!=SqlResult.SQLITE_OK) 
		) {
			var rc=SqlResult.SQLITE_OK;
			int flags=pFlags;
			string zVfs=zDefaultVfs;
			StringBuilder zFile=null;
			char c;
			int nUri=StringExtensions.Strlen30(zUri);
			pzErrMsg=null;
			ppVfs=null;
			if(((flags&SQLITE_OPEN_URI)!=0||sqliteinth.sqlite3GlobalConfig.bOpenUri)&&nUri>=5&&_Custom.memcmp(zUri,"file:",5)==0) {
				string zOpt;
				int eState;
				///Parser state when parsing URI 
				int iIn;
				///Input character index 
				//int iOut = 0;                 /* Output character index */
				int nByte=nUri+2;
				///Bytes of space to allocate 
				///Make sure the SQLITE_OPEN_URI flag is set to indicate to the VFS xOpen 
				///method that there may be extra parameters following the file-name.
				flags|=SQLITE_OPEN_URI;
				for(iIn=0;iIn<nUri;iIn++)
					nByte+=(zUri[iIn]=='&')?1:0;
				//zFile = sqlite3_malloc(nByte);
				//if( null==zFile ) return SQLITE_NOMEM;
				zFile=new StringBuilder(nByte);
				///
				///<summary>
				///Discard the scheme and authority segments of the URI. 
				///</summary>
				if(zUri[5]=='/'&&zUri[6]=='/') {
					iIn=7;
					while(iIn<nUri&&zUri[iIn]!='/')
						iIn++;
					if(iIn!=7&&(iIn!=16||String.Compare("localhost",zUri.Substring(7,9),StringComparison.InvariantCultureIgnoreCase)!=0))//_Custom.memcmp("localhost", &zUri[7], 9)) )
					 {
						pzErrMsg=io.sqlite3_mprintf("invalid uri authority: %.*s",iIn-7,zUri.Substring(7));
						rc=SqlResult.SQLITE_ERROR;
						goto parse_uri_out;
					}
				}
				else {
					iIn=5;
				}
				///
				///<summary>
				///Copy the filename and any query parameters into the zFile buffer. 
				///Decode %HH escape codes along the way. 
				///
				///Within this loop, variable eState may be set to 0, 1 or 2, depending
				///on the parsing context. As follows:
				///
				///</summary>
				///<param name="0: Parsing file">name.</param>
				///<param name="1: Parsing name section of a name=value query parameter.">1: Parsing name section of a name=value query parameter.</param>
				///<param name="2: Parsing value section of a name=value query parameter.">2: Parsing value section of a name=value query parameter.</param>
				///<param name=""></param>
				eState=0;
				while(iIn<nUri&&(c=zUri[iIn])!=0&&c!='#') {
					iIn++;
					if(c=='%'&&CharExtensions.sqlite3Isxdigit(zUri[iIn])&&CharExtensions.sqlite3Isxdigit(zUri[iIn+1])) {
						int octet=(Converter.sqlite3HexToInt(zUri[iIn++])<<4);
						octet+=Converter.sqlite3HexToInt(zUri[iIn++]);
						Debug.Assert(octet>=0&&octet<256);
						if(octet==0) {
							///
							///<summary>
							///This branch is taken when "%00" appears within the URI. In this
							///case we ignore all text in the remainder of the path, name or
							///value currently being parsed. So ignore the current character
							///and skip to the next "?", "=" or "&", as appropriate. 
							///</summary>
							while(iIn<nUri&&(c=zUri[iIn])!=0&&c!='#'&&(eState!=0||c!='?')&&(eState!=1||(c!='='&&c!='&'))&&(eState!=2||c!='&')) {
								iIn++;
							}
							continue;
						}
						c=(char)octet;
					}
					else
						if(eState==1&&(c=='&'||c=='=')) {
							if(zFile[zFile.Length-1]=='\0') {
								///
								///<summary>
								///An empty option name. Ignore this option altogether. 
								///</summary>
								while(zUri[iIn]!='\0'&&zUri[iIn]!='#'&&zUri[iIn-1]!='&')
									iIn++;
								continue;
							}
							if(c=='&') {
								zFile.Append('\0');
								//[iOut++] = '\0';
							}
							else {
								eState=2;
							}
							c='\0';
						}
						else
							if((eState==0&&c=='?')||(eState==2&&c=='&')) {
								c='\0';
								eState=1;
							}
					zFile.Append(c);
					//      zFile[iOut++] = c;
				}
				if(eState==1)
					zFile.Append('\0');
				//[iOut++] = '\0';
				zFile.Append('\0');
				//[iOut++] = '\0';
				zFile.Append('\0');
				//[iOut++] = '\0';
				///
				///<summary>
				///Check if there were any options specified that should be interpreted 
				///here. Options that are interpreted here include "vfs" and those that
				///correspond to flags that may be passed to the sqlite3_open_v2()
				///method. 
				///</summary>
				zOpt=zFile.ToString().Substring(StringExtensions.sqlite3Strlen30(zFile)+1);
				while(zOpt.Length>0) {
					int nOpt=StringExtensions.Strlen30(zOpt);
					string zVal=zOpt.Substring(nOpt);
					//zOpt[nOpt + 1];
					int nVal=StringExtensions.Strlen30(zVal);
					if(nOpt==3&&_Custom.memcmp("vfs",zOpt,3)==0) {
						zVfs=zVal;
					}
					else {
						OpenMode[] aMode=null;
						string zModeType="";
						int mask=0;
						int limit=0;
						if(nOpt==5&&_Custom.memcmp("cache",zOpt,5)==0) {
							OpenMode[] aCacheMode=new OpenMode[] {
								new OpenMode("shared",SQLITE_OPEN_SHAREDCACHE),
								new OpenMode("private",SQLITE_OPEN_PRIVATECACHE),
								new OpenMode(null,0)
							};
							mask=SQLITE_OPEN_SHAREDCACHE|SQLITE_OPEN_PRIVATECACHE;
							aMode=aCacheMode;
							limit=mask;
							zModeType="cache";
						}
						if(nOpt==4&&_Custom.memcmp("mode",zOpt,4)==0) {
							OpenMode[] aOpenMode=new OpenMode[] {
								new OpenMode("ro",SQLITE_OPEN_READONLY),
								new OpenMode("rw",SQLITE_OPEN_READWRITE),
								new OpenMode("rwc",SQLITE_OPEN_READWRITE|SQLITE_OPEN_CREATE),
								new OpenMode(null,0)
							};
							mask=SQLITE_OPEN_READONLY|SQLITE_OPEN_READWRITE|SQLITE_OPEN_CREATE;
							aMode=aOpenMode;
							limit=mask&flags;
							zModeType="access";
						}
						if(aMode!=null) {
							int i;
							int mode=0;
							for(i=0;aMode[i].z!=null;i++) {
								string z=aMode[i].z;
								if(nVal==StringExtensions.Strlen30(z)&&0==_Custom.memcmp(zVal,z,nVal)) {
									mode=aMode[i].mode;
									break;
								}
							}
							if(mode==0) {
								pzErrMsg=io.sqlite3_mprintf("no such %s mode: %s",zModeType,zVal);
								rc=SqlResult.SQLITE_ERROR;
								goto parse_uri_out;
							}
							if(mode>limit) {
								pzErrMsg=io.sqlite3_mprintf("%s mode not allowed: %s",zModeType,zVal);
								rc=SqlResult.SQLITE_PERM;
								goto parse_uri_out;
							}
							flags=((flags&~mask)|mode);
						}
					}
					zOpt=zVal.Substring(nVal+1);
				}
			}
			else {
				//zFile = sqlite3_malloc(nUri+2);
				//if( null==zFile ) return SQLITE_NOMEM;
				//memcpy(zFile, zUri, nUri);
				zFile=zUri==null?new StringBuilder():new StringBuilder(zUri.Substring(0,nUri));
				zFile.Append('\0');
				//[iOut++] = '\0';
				zFile.Append('\0');
				//[iOut++] = '\0';
			}
			ppVfs=os.sqlite3_vfs_find(zVfs);
			if(ppVfs==null) {
				pzErrMsg=io.sqlite3_mprintf("no such vfs: %s",zVfs);
				rc=SqlResult.SQLITE_ERROR;
			}
			parse_uri_out:
			if(rc!=SqlResult.SQLITE_OK) {
				//malloc_cs.sqlite3_free(zFile);
				zFile=null;
			}
			pFlags=flags;
			pzFile=zFile==null?null:zFile.ToString().Substring(0,StringExtensions.Strlen30(zFile.ToString()));
			return rc;
		}
		///
		///<summary>
		///This routine does the work of opening a database on behalf of
		///sqlite3_open() and sqlite3_open16(). The database filename "zFilename" is UTF-8 encoded.
        ///</summary>
		static SqlResult openDatabase(
            string zFilename,///Database filename UTF-8 encoded </param>
		    out Connection ppDb,///OUT: Returned database handle 		
		    int flags,///Operational flags 
		    string zVfs///Name of the VFS to use 
		) {
			Connection connection;
			///Store allocated handle here 
			SqlResult rc;
			///Return code 
			int isThreadsafe;
			///True for threadsafe connections 
			string zOpen="";
			///Filename argument to pass to BtreeOpen() 
			string zErrMsg="";
			///Error message from sqlite3ParseUri() 
			ppDb=null;
			#if !SQLITE_OMIT_AUTOINIT
			rc=sqlite3_initialize();
			if(rc!=0)
				return rc;
			#endif
			///
			///<summary>
			///Only allow sensible combinations of bits in the flags argument.
			///Throw an error if any non-sense combination is used.  If we
			///do not block illegal combinations here, it could trigger
			///Debug.Assert() statements in deeper layers.  Sensible combinations
			///are:
			///
			///1:  SQLITE_OPEN_READONLY
			///2:  SQLITE_OPEN_READWRITE
			///6:  SQLITE_OPEN_READWRITE | SQLITE_OPEN_CREATE
			Debug.Assert(SQLITE_OPEN_READONLY==0x01);
			Debug.Assert(SQLITE_OPEN_READWRITE==0x02);
			Debug.Assert(SQLITE_OPEN_CREATE==0x04);
			sqliteinth.testcase((1<<(flags&7))==0x02);
			///READONLY 
			sqliteinth.testcase((1<<(flags&7))==0x04);
			///READWRITE 
			sqliteinth.testcase((1<<(flags&7))==0x40);
			///READWRITE | CREATE 
			if(((1<<(flags&7))&0x46)==0)
				return sqliteinth.SQLITE_MISUSE_BKPT();
			if(sqliteinth.sqlite3GlobalConfig.bCoreMutex==false) {
				isThreadsafe=0;
			}
			else
				if((flags&SQLITE_OPEN_NOMUTEX)!=0) {
					isThreadsafe=0;
				}
				else
					if((flags&SQLITE_OPEN_FULLMUTEX)!=0) {
						isThreadsafe=1;
					}
					else {
						isThreadsafe=sqliteinth.sqlite3GlobalConfig.bFullMutex?1:0;
					}
			if((flags&SQLITE_OPEN_PRIVATECACHE)!=0) {
				flags&=~SQLITE_OPEN_SHAREDCACHE;
			}
			else
				if(sqliteinth.sqlite3GlobalConfig.sharedCacheEnabled) {
					flags|=SQLITE_OPEN_SHAREDCACHE;
				}
			///
			///<summary>
			///Remove harmful bits from the flags parameter
			///
			///The SQLITE_OPEN_NOMUTEX and SQLITE_OPEN_FULLMUTEX flags were
			///dealt with in the previous code block.  Besides these, the only
			///valid input flags for sqlite3_open_v2() are SQLITE_OPEN_READONLY,
			///SQLITE_OPEN_READWRITE, SQLITE_OPEN_CREATE, SQLITE_OPEN_SHAREDCACHE,
			///SQLITE_OPEN_PRIVATECACHE, and some reserved bits.  Silently mask
			///off all other flags.
			///
			///</summary>
			flags&=~(SQLITE_OPEN_DELETEONCLOSE|SQLITE_OPEN_EXCLUSIVE|SQLITE_OPEN_MAIN_DB|SQLITE_OPEN_TEMP_DB|SQLITE_OPEN_TRANSIENT_DB|SQLITE_OPEN_MAIN_JOURNAL|SQLITE_OPEN_TEMP_JOURNAL|SQLITE_OPEN_SUBJOURNAL|SQLITE_OPEN_MASTER_JOURNAL|SQLITE_OPEN_NOMUTEX|SQLITE_OPEN_FULLMUTEX|SQLITE_OPEN_WAL);
			///
			///<summary>
			///Allocate the sqlite data structure 
			///</summary>
			connection=new Connection();
			//malloc_cs.sqlite3MallocZero( sqlite3.Length );
			if(connection==null)
				goto opendb_out;
			if(sqliteinth.sqlite3GlobalConfig.bFullMutex&&isThreadsafe!=0) {
				connection.mutex=sqlite3MutexAlloc(SQLITE_MUTEX_RECURSIVE);
				if(connection.mutex==null) {
					//malloc_cs.sqlite3_free( ref db );
					goto opendb_out;
				}
			}
			connection.mutex.Enter();
			connection.errMask=(SqlResult)0xff;
			connection.BackendCount=2;
            connection.magic = Sqlite3.SQLITE_MAGIC_BUSY;
			Array.Copy(connection.aDbStatic,connection.Backends,connection.aDbStatic.Length);
			// db.aDb = db.aDbStatic;
			Debug.Assert(connection.aLimit.Length==aHardLimit.Length);
			Buffer.BlockCopy(aHardLimit,0,connection.aLimit,0,aHardLimit.Length*sizeof(int));
			//memcpy(db.aLimit, aHardLimit, sizeof(db.aLimit));
			connection.autoCommit=1;
			connection.nextAutovac=-1;
			connection.nextPagesize=0;
            connection.flags |= SqliteFlags.SQLITE_ShortColNames | SqliteFlags.SQLITE_AutoIndex | SqliteFlags.SQLITE_EnableTrigger;
			if(sqliteinth.SQLITE_DEFAULT_FILE_FORMAT<4)
                connection.flags |= SqliteFlags.SQLITE_LegacyFileFmt
				#if SQLITE_ENABLE_LOAD_EXTENSION
																																																																																												| SQLITE_LoadExtension
#endif
				#if SQLITE_DEFAULT_RECURSIVE_TRIGGERS
																																																																																												   | SQLITE_RecTriggers
#endif
				#if (SQLITE_DEFAULT_FOREIGN_KEYS)
																																																																																												   | SQLITE_ForeignKeys
#endif
				;
			connection.aCollSeq.sqlite3HashInit();
			#if !SQLITE_OMIT_VIRTUALTABLE
			connection.aModule=new Hash<Module>();
			connection.aModule.sqlite3HashInit();
			#endif
			///<param name="Add the default collation sequence BINARY. BINARY works for both UTF">8</param>
			///<param name="and UTF">16, so add a version for each to avoid any unnecessary</param>
			///<param name="conversions. The only error that can occur here is a malloc() failure.">conversions. The only error that can occur here is a malloc() failure.</param>
			///<param name=""></param>
			createCollation(connection,"BINARY",SqliteEncoding.UTF8,CollationType.BINARY,0,binCollFunc,null);
			createCollation(connection,"BINARY",SqliteEncoding.UTF16BE,CollationType.BINARY,0,binCollFunc,null);
			createCollation(connection,"BINARY",SqliteEncoding.UTF16LE,CollationType.BINARY,0,binCollFunc,null);
			createCollation(connection,"RTRIM",SqliteEncoding.UTF8,CollationType.USER,1,binCollFunc,null);
			//if ( db.mallocFailed != 0 )
			//{
			//  goto opendb_out;
			//}
			connection.pDfltColl=connection.sqlite3FindCollSeq(SqliteEncoding.UTF8,"BINARY",0);
			Debug.Assert(connection.pDfltColl!=null);
			///<param name="Also add a UTF">insensitive collation sequence. </param>
			createCollation(connection,"NOCASE",SqliteEncoding.UTF8,CollationType.NOCASE,0,nocaseCollatingFunc,null);
			///Parse the filename/URI argument. 
			connection.openFlags=flags;
			rc=sqlite3ParseUri(zVfs,zFilename,ref flags,ref connection.pVfs,ref zOpen,ref zErrMsg);
			if(rc!=SqlResult.SQLITE_OK) {
				//if( rc==SQLITE_NOMEM ) db.mallocFailed = 1;
				utilc.sqlite3Error(connection,rc,zErrMsg.Length>0?"%s":"",zErrMsg);
				//malloc_cs.sqlite3_free(zErrMsg);
				goto opendb_out;
			}
			///
			///<summary>
			///Open the backend database driver 
			///</summary>
			rc=Btree.Open(connection.pVfs,zOpen,connection,ref connection.Backends[0].BTree,0,flags|SQLITE_OPEN_MAIN_DB);
			if(rc!=SqlResult.SQLITE_OK) {
				if(rc== SqlResult.SQLITE_IOERR_NOMEM) {
					rc=SqlResult.SQLITE_NOMEM;
				}
				utilc.sqlite3Error(connection,rc,0);
				goto opendb_out;
			}

            ///The default safety_level for the main database is 'full'; for the temp
            ///database it is 'NONE'. This matches the pager layer defaults.
            {
                var main = connection.Backends[0];
                main.pSchema=SchemaExtensions.GetSchema(main.BTree, connection);
                main.Name = "main";
                main.safety_level = 3;
            }
            {
                var temp = connection.Backends[1];
                temp.pSchema = SchemaExtensions.GetSchema(null, connection);
                temp.Name = "temp";
                temp.safety_level = 1;
            }

            connection.magic = Sqlite3.SQLITE_MAGIC_OPEN;
			//if ( db.mallocFailed != 0 )
			//{
			//  goto opendb_out;
			//}
			///
			///Register all built-in functions, but do not attempt to read the
			///database schema yet. This is delayed until the first time the database
			///is accessed.
			utilc.sqlite3Error(connection,SqlResult.SQLITE_OK,0);
			PredefinedFunctions.sqlite3RegisterBuiltinFunctions(connection);

			///Load automatic extensions - extensions that have been registered</param>
			///using the sqlite3_automatic_extension() API.
			Sqlite3ExtensionModule.sqlite3AutoLoadExtensions(connection);
			rc=sqlite3_errcode(connection);
			if(rc!=SqlResult.SQLITE_OK) {
				goto opendb_out;
			}
			#if SQLITE_ENABLE_FTS1
																																																																					if( 0==db.mallocFailed ){
extern int sqlite3Fts1Init(sqlite3);
rc = sqlite3Fts1Init(db);
}
#endif
			#if SQLITE_ENABLE_FTS2
																																																																					if( 0==db.mallocFailed && rc==SqlResult.SQLITE_OK ){
extern int sqlite3Fts2Init(sqlite3);
rc = sqlite3Fts2Init(db);
}
#endif
			#if SQLITE_ENABLE_FTS3
																																																																					if( 0==db.mallocFailed && rc==SqlResult.SQLITE_OK ){
rc = sqlite3Fts3Init(db);
}
#endif
			#if SQLITE_ENABLE_ICU
																																																																					if( 0==db.mallocFailed && rc==SqlResult.SQLITE_OK ){
extern int sqlite3IcuInit(sqlite3);
rc = sqlite3IcuInit(db);
}
#endif
			#if SQLITE_ENABLE_RTREE
																																																																					if( 0==db.mallocFailed && rc==SqlResult.SQLITE_OK){
rc = sqlite3RtreeInit(db);
}
#endif
			utilc.sqlite3Error(connection,rc,0);
			///-DSQLITE_DEFAULT_LOCKING_MODE=1 makes EXCLUSIVE the default locking mode.  
            ///-DSQLITE_DEFAULT_LOCKING_MODE=0 make NORMAL the default locking mode.  
            ///-Doing nothing at all also makes NORMAL the default mode.
			#if SQLITE_DEFAULT_LOCKING_MODE
																																																																					db.dfltLockMode = SQLITE_DEFAULT_LOCKING_MODE;
sqlite3PagerLockingMode(sqlite3BtreePager(db.aDb[0].pBt),
SQLITE_DEFAULT_LOCKING_MODE);
#endif
			///<param name="Enable the lookaside">malloc subsystem </param>
			setupLookaside(connection,null,sqliteinth.sqlite3GlobalConfig.szLookaside,sqliteinth.sqlite3GlobalConfig.nLookaside);
			sqlite3_wal_autocheckpoint(connection,Limits.SQLITE_DEFAULT_WAL_AUTOCHECKPOINT);
			opendb_out:
			//malloc_cs.sqlite3_free(zOpen);
			if(connection!=null) {
				Debug.Assert(connection.mutex!=null||isThreadsafe==0||!sqliteinth.sqlite3GlobalConfig.bFullMutex);
				connection.mutex.Exit();
			}
			rc=sqlite3_errcode(connection);
			if(rc==SqlResult.SQLITE_NOMEM) {
				sqlite3_close(connection);
				connection=null;
			}
			else
				if(rc!=SqlResult.SQLITE_OK) {
                    connection.magic = Sqlite3.SQLITE_MAGIC_SICK;
				}
			ppDb=connection;
			return malloc_cs.sqlite3ApiExit(0,rc);
		}
		
        
        ///<summary>
		///Open a new database handle.
		///</summary>
		static public SqlResult sqlite3_open(string zFilename,out Connection ppDb) {
			return openDatabase(zFilename,out ppDb,SQLITE_OPEN_READWRITE|SQLITE_OPEN_CREATE,null);
		}


		static public SqlResult sqlite3_open_v2(
            string filename,///<param name="Database filename (UTF">8) </param>
		    out Connection ppDb,///OUT: SQLite db handle 
		    int flags,///Flags 
		    string zVfs///Name of VFS module to use 
		) {
			return openDatabase(filename,out ppDb,flags,zVfs);
		}
		#if !SQLITE_OMIT_UTF16
																																														
/*
** Open a new database handle.
*/
int sqlite3_open16(
string zFilename,
sqlite3 **ppDb
){
char const *zFilename8;   /* zFilename encoded in UTF-8 instead of UTF-16 */
sqlite3_value pVal;
int rc;

Debug.Assert(zFilename );
Debug.Assert(ppDb );
*ppDb = 0;
#if !SQLITE_OMIT_AUTOINIT
																																														rc = sqlite3_initialize();
if( rc !=0) return rc;
#endif
																																														pVal = sqlite3ValueNew(0);
sqlite3ValueSetStr(pVal, -1, zFilename, SqliteEncoding.UTF16NATIVE, SQLITE_STATIC);
zFilename8 = sqlite3ValueText(pVal, SqliteEncoding.UTF8);
if( zFilename8 ){
rc = openDatabase(zFilename8, ppDb,
SQLITE_OPEN_READWRITE | SQLITE_OPEN_CREATE, 0);
Debug.Assert(*ppDb || rc==SQLITE_NOMEM );
if( rc==SqlResult.SQLITE_OK && !DbHasProperty(*ppDb, 0, DB_SchemaLoaded) ){
ENC(*ppDb) = SqliteEncoding.UTF16NATIVE;
}
}else{
rc = SQLITE_NOMEM;
}
sqlite3ValueFree(pVal);

return malloc_cs.sqlite3ApiExit(0, rc);
}
#endif
		///<summary>
		///Register a new collation sequence with the database handle db.
		///</summary>
		static SqlResult sqlite3_create_collation(Connection db,string zName,SqliteEncoding enc,object pCtx,dxCompare xCompare) {
            SqlResult rc;
			db.mutex.Enter();
			//Debug.Assert( 0 == db.mallocFailed );
			rc=createCollation(db,zName,enc,CollationType.USER,pCtx,xCompare,null);
			rc=malloc_cs.sqlite3ApiExit(db,rc);
			db.mutex.Exit();
			return rc;
		}
		
        ///<summary>
		///Register a new collation sequence with the database handle db.
		///</summary>
		static SqlResult sqlite3_create_collation_v2(Connection db,string zName,SqliteEncoding enc,object pCtx,dxCompare xCompare,//int(*xCompare)(void*,int,const void*,int,const void),
		dxDelCollSeq xDel//void(*xDel)(void)
		) {
            SqlResult rc;
			db.mutex.Enter();
			//Debug.Assert( 0 == db.mallocFailed );
			rc=createCollation(db,zName,enc,CollationType.USER,pCtx,xCompare,xDel);
			rc=malloc_cs.sqlite3ApiExit(db,rc);
			db.mutex.Exit();
			return rc;
		}
		#if !SQLITE_OMIT_UTF16
																																														/*
** Register a new collation sequence with the database handle db.
*/
//int sqlite3_create_collation16(
//  sqlite3* db,
//  string zName,
//  int enc,
//  void* pCtx,
//  int(*xCompare)(void*,int,const void*,int,const void)
//){
//  var rc = SqlResult.SQLITE_OK;
//  string zName8;
//  db.mutex.sqlite3_mutex_enter();
//  Debug.Assert( 0==db.mallocFailed );
//  zName8 = sqlite3Utf16to8(db, zName, -1, SqliteEncoding.UTF16NATIVE);
//  if( zName8 ){
//    rc = createCollation(db, zName8, (u8)enc, CollationType.USER, pCtx, xCompare, 0);
//    sqlite3DbFree(db,ref zName8);
//  }
//  rc = malloc_cs.sqlite3ApiExit(db, rc);
//  db.mutex.sqlite3_mutex_leave();
//  return rc;
//}
#endif
		///<summary>
		///Register a collation sequence factory callback with the database handle
		///db. Replace any previously installed collation sequence factory.
		///</summary>
		static SqlResult sqlite3_collation_needed(Connection db,object pCollNeededArg,dxCollNeeded xCollNeeded) {
			db.mutex.Enter();
			db.xCollNeeded=xCollNeeded;
			db.xCollNeeded16=null;
			db.pCollNeededArg=pCollNeededArg;
			db.mutex.Exit();
			return SqlResult.SQLITE_OK;
		}
		#if !SQLITE_OMIT_UTF16
																																														/*
** Register a collation sequence factory callback with the database handle
** db. Replace any previously installed collation sequence factory.
*/
//int sqlite3_collation_needed16(
//  sqlite3 db,
//  void pCollNeededArg,
//  void(*xCollNeeded16)(void*,sqlite3*,int eTextRep,const void)
//){
//  db.mutex.sqlite3_mutex_enter();
//  db.xCollNeeded = 0;
//  db.xCollNeeded16 = xCollNeeded16;
//  db.pCollNeededArg = pCollNeededArg;
//  db.mutex.sqlite3_mutex_leave();
//  return SqlResult.SQLITE_OK;
//}
#endif
		#if !SQLITE_OMIT_DEPRECATED
																																														/*
** This function is now an anachronism. It used to be used to recover from a
** malloc() failure, but SQLite now does this automatically.
*/
static int sqlite3_global_recover()
{
return SqlResult.SQLITE_OK;
}
#endif
		///
		///<summary>
		///Test to see whether or not the database connection is in autocommit
		///mode.  Return TRUE if it is and FALSE if not.  Autocommit mode is on
		///by default.  Autocommit is disabled by a BEGIN statement and reenabled
		///by the next COMMIT or ROLLBACK.
		///
		///THIS IS AN EXPERIMENTAL API AND IS SUBJECT TO CHANGE ******
		///</summary>
		static u8 sqlite3_get_autocommit(Connection db) {
			return db.autoCommit;
		}


		///
		///<summary>
		///The following routines are subtitutes for constants SQLITE_CORRUPT,
		///SQLITE_MISUSE, SQLITE_CANTOPEN, SQLITE_IOERR and possibly other error
		///constants.  They server two purposes:
		///
		///1.  Serve as a convenient place to set a breakpoint in a debugger
		///to detect when version error conditions occurs.
		///
		///2.  Invoke io.sqlite3_log() to provide the source code location where
		///a low-level error is first detected.
        ///</summary>		
        static SqlResult sqlite3CorruptError(int lineno) {
			sqliteinth.testcase(sqliteinth.sqlite3GlobalConfig.xLog!=null);
			io.sqlite3_log(SqlResult.SQLITE_CORRUPT,"database corruption at line %d of [%.10s]",lineno,20+sqlite3_sourceid());
			return SqlResult.SQLITE_CORRUPT;
		}
		static SqlResult sqlite3MisuseError(int lineno) {
			sqliteinth.testcase(sqliteinth.sqlite3GlobalConfig.xLog!=null);
			io.sqlite3_log(SqlResult.SQLITE_MISUSE,"misuse at line %d of [%.10s]",lineno,20+sqlite3_sourceid());
			return SqlResult.SQLITE_MISUSE;
		}
		static SqlResult sqlite3CantopenError(int lineno) {
			sqliteinth.testcase(sqliteinth.sqlite3GlobalConfig.xLog!=null);
			io.sqlite3_log(SqlResult.SQLITE_CANTOPEN,"cannot open file at line %d of [%.10s]",lineno,20+sqlite3_sourceid());
			return SqlResult.SQLITE_CANTOPEN;
		}
		#if !SQLITE_OMIT_DEPRECATED
																																														/*
** This is a convenience routine that makes sure that all thread-specific
** data for this thread has been deallocated.
**
** SQLite no longer uses thread-specific data so this routine is now a
** no-op.  It is retained for historical compatibility.
*/
void sqlite3_thread_cleanup()
{
}
#endif
		///
		///<summary>
		///Return meta information about a specific column of a database table.
		///See comment in sqlite3.h (sqlite.h.in) for details.
		///</summary>
		#if SQLITE_ENABLE_COLUMN_METADATA
																																														
    static int sqlite3_table_column_metadata(
    sqlite3 db,            /* Connection handle */
    string zDbName,        /* Database name or NULL */
    string zTableName,     /* Table name */
    string zColumnName,    /* Column name */
    ref string pzDataType, /* OUTPUT: Declared data type */
    ref string pzCollSeq,  /* OUTPUT: Collation sequence name */
    ref int pNotNull,      /* OUTPUT: True if NOT NULL constraint exists */
    ref int pPrimaryKey,   /* OUTPUT: True if column part of PK */
    ref int pAutoinc       /* OUTPUT: True if column is auto-increment */
    )
    {
      int rc;
      string zErrMsg = "";
      Table pTab = null;
      Column pCol = null;
      int iCol;

      string zDataType = null;
      string zCollSeq = null;
      int notnull = 0;
      int primarykey = 0;
      int autoinc = 0;

      /* Ensure the database schema has been loaded */
      sqlite3_mutex_enter( db.mutex );
      sqlite3BtreeEnterAll( db );
      rc = sqlite3Init( db, ref zErrMsg );
      if ( SqlResult.SQLITE_OK != rc )
      {
        goto error_out;
      }

      /* Locate the table in question */
      pTab = build.sqlite3FindTable( db, zTableName, zDbName );
      if ( null == pTab || pTab.pSelect != null )
      {
        pTab = null;
        goto error_out;
      }

      /* Find the column for which info is requested */
      if ( exprc.sqlite3IsRowid( zColumnName ) )
      {
        iCol = pTab.iPKey;
        if ( iCol >= 0 )
        {
          pCol = pTab.aCol[iCol];
        }
      }
      else
      {
        for ( iCol = 0; iCol < pTab.nCol; iCol++ )
        {
          pCol = pTab.aCol[iCol];
          if ( pCol.zName.Equals( zColumnName, StringComparison.InvariantCultureIgnoreCase ) )
          {
            break;
          }
        }
        if ( iCol == pTab.nCol )
        {
          pTab = null;
          goto error_out;
        }
      }

      /* The following block stores the meta information that will be returned
      ** to the caller in local variables zDataType, zCollSeq, notnull, primarykey
      ** and autoinc. At this point there are two possibilities:
      **
      **     1. The specified column name was rowid", "oid" or "_rowid_"
      **        and there is no explicitly declared IPK column.
      **
      **     2. The table is not a view and the column name identified an
      **        explicitly declared column. Copy meta information from pCol.
      */
      if ( pCol != null )
      {
        zDataType = pCol.zType;
        zCollSeq = pCol.zColl;
        notnull = pCol.notNull != 0 ? 1 : 0;
        primarykey = pCol.isPrimKey != 0 ? 1 : 0;
        autoinc = ( pTab.iPKey == iCol && ( pTab.tabFlags & TableFlags.TF_Autoincrement ) != 0 ) ? 1 : 0;
      }
      else
      {
        zDataType = "INTEGER";
        primarykey = 1;
      }
      if ( String.IsNullOrEmpty( zCollSeq ) )
      {
        zCollSeq = "BINARY";
      }

error_out:
      sqlite3BtreeLeaveAll( db );

      /* Whether the function call succeeded or failed, set the output parameters
      ** to whatever their local counterparts contain. If an error did occur,
      ** this has the effect of zeroing all output parameters.
      */
      //if ( pzDataType )
      pzDataType = zDataType;
      //if ( pzCollSeq )
      pzCollSeq = zCollSeq;
      //if ( pNotNull )
      pNotNull = notnull;
      //if ( pPrimaryKey )
      pPrimaryKey = primarykey;
      //if ( pAutoinc )
      pAutoinc = autoinc;

      if ( SqlResult.SQLITE_OK == rc && null == pTab )
      {
        sqlite3DbFree( db, ref zErrMsg );
        zErrMsg = io.sqlite3MPrintf( db, "no such table column: %s.%s", zTableName,
        zColumnName );
        rc = SqlResult.SQLITE_ERROR;
      }
      utilc.sqlite3Error( db, rc, ( !String.IsNullOrEmpty( zErrMsg ) ? "%s" : null ), zErrMsg );
      sqlite3DbFree( db, ref zErrMsg );
      rc = malloc_cs.sqlite3ApiExit( db, rc );
      sqlite3_mutex_leave( db.mutex );
      return rc;
    }
#endif
		///
		///<summary>
		///Sleep for a little while.  Return the amount of time slept.
		///</summary>
		static public int sqlite3_sleep(int ms) {
			sqlite3_vfs pVfs;
			int rc;
			pVfs=os.sqlite3_vfs_find(null);
			if(pVfs==null)
				return 0;
			///
			///<summary>
			///This function works in milliseconds, but the underlying OsSleep()
			///API uses microseconds. Hence the 1000's.
			///
			///</summary>
			rc=(os.sqlite3OsSleep(pVfs,1000*ms)/1000);
			return rc;
		}
		///
		///<summary>
		///Enable or disable the extended result codes.
		///
		///</summary>
		static SqlResult sqlite3_extended_result_codes(Connection db,bool onoff) {
			db.mutex.Enter();
			db.errMask=(SqlResult)(onoff?0xffffffff:0xff);
			db.mutex.Exit();
			return SqlResult.SQLITE_OK;
		}
		///
		///<summary>
		///Invoke the xFileControl method on a particular database.
		///
		///</summary>
		static SqlResult sqlite3_file_control(Connection db,string zDbName,int op,ref sqlite3_int64 pArg) {
			var rc=SqlResult.SQLITE_ERROR;
			int iDb;
			db.mutex.Enter();
			if(zDbName==null) {
				iDb=0;
			}
			else {
				for(iDb=0;iDb<db.BackendCount;iDb++) {
					if(db.Backends[iDb].Name==zDbName)
						break;
				}
			}
			if(iDb<db.BackendCount) {
				Btree pBtree=db.Backends[iDb].BTree;
				if(pBtree!=null) {
					Pager pPager;
					sqlite3_file fd;
                    pBtree.Enter();
					pPager=pBtree.sqlite3BtreePager();
					Debug.Assert(pPager!=null);
					fd=pPager.sqlite3PagerFile();
					Debug.Assert(fd!=null);
					if(op==SQLITE_FCNTL_FILE_POINTER) {
						#if (SQLITE_SILVERLIGHT || WINDOWS_MOBILE)
																																																																																																																																										              pArg = (long)-1; // not supported
#else
						pArg=(long)fd.fs.Handle;
						#endif
						rc=SqlResult.SQLITE_OK;
					}
					else
						if(fd.pMethods!=null) {
							rc=os.sqlite3OsFileControl(fd,(u32)op,ref pArg);
						}
						else {
							rc=SqlResult.SQLITE_NOTFOUND;
						}
                    pBtree.Exit();
				}
			}
			db.mutex.Exit();
			return rc;
		}
		///
		///<summary>
		///Interface to the testing logic.
		///
		///</summary>
		static int sqlite3_test_control(int op,params object[] ap) {
			var rc=0;
			#if !SQLITE_OMIT_BUILTIN_TEST
			//  va_list ap;
			lock(_Custom.lock_va_list) {
				_Custom.va_start(ap,"op");
				switch(op) {
				///
				///<summary>
				///Save the current state of the PRNG.
				///
				///</summary>
				case SQLITE_TESTCTRL_PRNG_SAVE: {
					sqlite3PrngSaveState();
					break;
				}
				///
				///<summary>
				///Restore the state of the PRNG to the last state saved using
				///PRNG_SAVE.  If PRNG_SAVE has never before been called, then
				///this verb acts like PRNG_RESET.
				///
				///</summary>
				case SQLITE_TESTCTRL_PRNG_RESTORE: {
					sqlite3PrngRestoreState();
					break;
				}
				///
				///<summary>
				///Reset the PRNG back to its uninitialized state.  The next call
				///to sqlite3_randomness() will reseed the PRNG using a single call
				///to the xRandomness method of the default VFS.
				///
				///</summary>
				case SQLITE_TESTCTRL_PRNG_RESET: {
					sqlite3PrngResetState();
					break;
				}
				///
				///<summary>
				///sqlite3_test_control(BITVEC_TEST, size, program)
				///
				///Run a test against a Bitvec object of size.  The program argument
				///</summary>
				///<param name="is an array of integers that defines the test.  Return ">1 on a</param>
				///<param name="memory allocation error, 0 on success, or non">zero for an error.</param>
				///<param name="See the sqlite3BitvecBuiltinTest() for additional information.">See the sqlite3BitvecBuiltinTest() for additional information.</param>
				///<param name=""></param>
				case SQLITE_TESTCTRL_BITVEC_TEST: {
					int sz=_Custom.va_arg(ap,(Int32)0);
					int[] aProg=_Custom.va_arg(ap,(Int32[])null);
					rc=BitvecExtensions.sqlite3BitvecBuiltinTest((u32)sz,aProg);
					break;
				}
				///
				///<summary>
				///sqlite3_test_control(BENIGN_MALLOC_HOOKS, xBegin, xEnd)
				///
				///Register hooks to call to indicate which malloc() failures
				///are benign.
				///
				///</summary>
				case SQLITE_TESTCTRL_BENIGN_MALLOC_HOOKS: {
					//typedef void (*void_function)(void);
					void_function xBenignBegin;
					void_function xBenignEnd;
					xBenignBegin=_Custom.va_arg(ap,(void_function)null);
					xBenignEnd=_Custom.va_arg(ap,(void_function)null);
					sqlite3BenignMallocHooks(xBenignBegin,xBenignEnd);
					break;
				}
				///
				///<summary>
				///sqlite3_test_control(SQLITE_TESTCTRL_PENDING_BYTE, unsigned int X)
				///
				///Set the PENDING byte to the value in the argument, if X>0.
				///Make no changes if X==0.  Return the value of the pending byte
				///as it existing before this routine was called.
				///
				///IMPORTANT:  Changing the PENDING byte from 0x40000000 results in
				///an incompatible database file format.  Changing the PENDING byte
				///while any database connection is open results in undefined and
				///dileterious behavior.
				///
				///</summary>
				case SQLITE_TESTCTRL_PENDING_BYTE: {
					rc=PENDING_BYTE;
					#if !SQLITE_OMIT_WSD
					{
						u32 newVal=_Custom.va_arg(ap,(UInt32)0);
						if(newVal!=0) {
							if(sqlite3PendingByte!=newVal)
								sqlite3PendingByte=(int)newVal;
							#if DEBUG && TCLSH
																																																																																																																																																																	                  TCLsqlite3PendingByte.iValue = sqlite3PendingByte;
#endif
							PENDING_BYTE=sqlite3PendingByte;
						}
						#endif
					}
					break;
				}
				///
				///<summary>
				///sqlite3_test_control(SQLITE_TESTCTRL_ASSERT, int X)
				///
				///</summary>
				///<param name="This action provides a run">time test to see whether or not</param>
				///<param name="Debug.Assert() was enabled at compile">time.  If X is true and Debug.Assert()</param>
				///<param name="is enabled, then the return value is true.  If X is true and">is enabled, then the return value is true.  If X is true and</param>
				///<param name="Debug.Assert() is disabled, then the return value is zero.  If X is">Debug.Assert() is disabled, then the return value is zero.  If X is</param>
				///<param name="false and Debug.Assert() is enabled, then the assertion fires and the">false and Debug.Assert() is enabled, then the assertion fires and the</param>
				///<param name="process aborts.  If X is false and Debug.Assert() is disabled, then the">process aborts.  If X is false and Debug.Assert() is disabled, then the</param>
				///<param name="return value is zero.">return value is zero.</param>
				///<param name=""></param>
				case SQLITE_TESTCTRL_ASSERT: {
					int x=0;
					Debug.Assert((x=_Custom.va_arg(ap,(Int32)0))!=0);
					rc=x;
					break;
				}
				///
				///<summary>
				///sqlite3_test_control(SQLITE_TESTCTRL_ALWAYS, int X)
				///
				///</summary>
				///<param name="This action provides a run">time test to see how the ALWAYS and</param>
				///<param name="NEVER macros were defined at compile">time.</param>
				///<param name=""></param>
				///<param name="The return value is Sqlite3.ALWAYS(X).">The return value is Sqlite3.ALWAYS(X).</param>
				///<param name=""></param>
				///<param name="The recommended test is X==2.  If the return value is 2, that means">The recommended test is X==2.  If the return value is 2, that means</param>
				///<param name="Sqlite3.ALWAYS() and NEVER() are both no">through macros, which is the</param>
				///<param name="default setting.  If the return value is 1, then Sqlite3.ALWAYS() is either">default setting.  If the return value is 1, then Sqlite3.ALWAYS() is either</param>
				///<param name="hard">coded to true or else it asserts if its argument is false.</param>
				///<param name="The first behavior (hard">coded to true) is the case if</param>
				///<param name="SQLITE_TESTCTRL_ASSERT shows that Debug.Assert() is disabled and the second">SQLITE_TESTCTRL_ASSERT shows that Debug.Assert() is disabled and the second</param>
				///<param name="behavior (assert if the argument to Sqlite3.ALWAYS() is false) is the case if">behavior (assert if the argument to Sqlite3.ALWAYS() is false) is the case if</param>
				///<param name="SQLITE_TESTCTRL_ASSERT shows that Debug.Assert() is enabled.">SQLITE_TESTCTRL_ASSERT shows that Debug.Assert() is enabled.</param>
				///<param name=""></param>
				///<param name="The run">time test procedure might look something like this:</param>
				///<param name=""></param>
				///<param name="if( sqlite3_test_control(SQLITE_TESTCTRL_ALWAYS, 2)==2 ){">if( sqlite3_test_control(SQLITE_TESTCTRL_ALWAYS, 2)==2 ){</param>
				///<param name="// Sqlite3.ALWAYS() and NEVER() are no">through macros</param>
				///<param name="}else if( sqlite3_test_control(SQLITE_TESTCTRL_ASSERT, 1) ){">}else if( sqlite3_test_control(SQLITE_TESTCTRL_ASSERT, 1) ){</param>
				///<param name="// Sqlite3.ALWAYS(x) asserts that x is true. NEVER(x) asserts x is false.">// Sqlite3.ALWAYS(x) asserts that x is true. NEVER(x) asserts x is false.</param>
				///<param name="}else{">}else{</param>
				///<param name="// Sqlite3.ALWAYS(x) is a constant 1.  NEVER(x) is a constant 0.">// Sqlite3.ALWAYS(x) is a constant 1.  NEVER(x) is a constant 0.</param>
				///<param name="}">}</param>
				///<param name=""></param>
				case SQLITE_TESTCTRL_ALWAYS: {
					int x=_Custom.va_arg(ap,(Int32)0);
					rc=Sqlite3.ALWAYS(x);
					break;
				}
				///
				///<summary>
				///sqlite3_test_control(SQLITE_TESTCTRL_RESERVE, sqlite3 db, int N)
				///
				///Set the nReserve size to N for the main database on the database
				///connection db.
				///
				///</summary>
				case SQLITE_TESTCTRL_RESERVE: {
					Connection db=_Custom.va_arg(ap,(Connection)null);
					int x=_Custom.va_arg(ap,(Int32)0);
					db.mutex.Enter();
					db.Backends[0].BTree.sqlite3BtreeSetPageSize(0,x,0);
					db.mutex.Exit();
					break;
				}
				///
				///<summary>
				///sqlite3_test_control(SQLITE_TESTCTRL_OPTIMIZATIONS, sqlite3 db, int N)
				///
				///Enable or disable various optimizations for testing purposes.  The 
				///argument N is a bitmask of optimizations to be disabled.  For normal
				///operation N should be 0.  The idea is that a test program (like the
				///SQL Logic Test or SLT test module) can run the same SQL multiple times
				///with various optimizations disabled to verify that the same answer
				///is obtained in every case.
				///
				///</summary>
				case SQLITE_TESTCTRL_OPTIMIZATIONS: {
					Connection db=_Custom.va_arg(ap,(Connection)null);
					//sqlite3 db = _Custom.va_arg(ap, sqlite3);
					int x=_Custom.va_arg(ap,(Int32)0);
					//int x = _Custom.va_arg(ap,int);
                    db.flags = ((SqliteFlags)x & SqliteFlags.SQLITE_OptMask) | (db.flags & ~SqliteFlags.SQLITE_OptMask);
					break;
				}
				//#if SQLITE_N_KEYWORD
				///
				///<summary>
				///sqlite3_test_control(SQLITE_TESTCTRL_ISKEYWORD, const string zWord)
				///
				///If zWord is a keyword recognized by the parser, then return the
				///number of keywords.  Or if zWord is not a keyword, return 0.
				///
				///This test feature is only available in the amalgamation since
				///the SQLITE_N_KEYWORD macro is not defined in this file if SQLite
				///is built using separate source files.
				///
				///</summary>
				case SQLITE_TESTCTRL_ISKEYWORD: {
					string zWord=(string)_Custom.va_arg(ap,"char*");
					int n=StringExtensions.Strlen30(zWord);
					rc=((TokenType)sqlite3KeywordCode(zWord,n)!=TokenType.TK_ID)?SQLITE_N_KEYWORD:0;
					break;
				}
				//#endif 
				///
				///<summary>
				///sqlite3_test_control(SQLITE_TESTCTRL_PGHDRSZ)
				///
				///Return the size of a pcache header in bytes.
				///
				///</summary>
				case SQLITE_TESTCTRL_PGHDRSZ: {
					rc=-1;
					// sizeof(PgHdr);
					break;
				}
				///
				///<summary>
				///sqlite3_test_control(SQLITE_TESTCTRL_SCRATCHMALLOC, sz, &pNew, pFree);
				///
				///Pass pFree into sqlite3ScratchFree(). 
				///If sz>0 then allocate a scratch buffer into pNew.  
				///
				///</summary>
				case SQLITE_TESTCTRL_SCRATCHMALLOC: {
					//void pFree, *ppNew;
					//int sz;
					//sz = _Custom.va_arg(ap, int);
					//ppNew = _Custom.va_arg(ap, void*);
					//pFree = _Custom.va_arg(ap, void);
					//if( sz ) *ppNew = sqlite3ScratchMalloc(sz);
					//sqlite3ScratchFree(pFree);
					break;
				}
				///
				///<summary>
				///sqlite3_test_control(SQLITE_TESTCTRL_LOCALTIME_FAULT, int onoff);
				///
				///</summary>
				///<param name="If parameter onoff is non">zero, configure the wrappers so that all</param>
				///<param name="subsequent calls to localtime() and variants fail. If onoff is zero,">subsequent calls to localtime() and variants fail. If onoff is zero,</param>
				///<param name="undo this setting.">undo this setting.</param>
				///<param name=""></param>
				case SQLITE_TESTCTRL_LOCALTIME_FAULT: {
					sqliteinth.sqlite3GlobalConfig.bLocaltimeFault=_Custom.va_arg(ap,(Boolean)true);
					break;
				}
				}
				_Custom.va_end(ref ap);
			}
			#endif
			return rc;
		}
		///
		///<summary>
		///This is a utility routine, useful to VFS implementations, that checks
		///to see if a database file was a URI that contained a specific query 
		///parameter, and if so obtains the value of the query parameter.
		///
		///The zFilename argument is the filename pointer passed into the xOpen()
		///method of a VFS implementation.  The zParam argument is the name of the
		///query parameter we seek.  This routine returns the value of the zParam
		///parameter if it exists.  If the parameter does not exist, this routine
		///returns a NULL pointer.
		///</summary>
		static string sqlite3_uri_parameter(string zFilename,string zParam) {
			Debugger.Break();
			//zFilename += StringExtensions.sqlite3Strlen30(zFilename) + 1;
			//while( zFilename[0] ){
			//  int x = strcmp(zFilename, zParam);
			//  zFilename += StringExtensions.sqlite3Strlen30(zFilename) + 1;
			//  if( x==0 ) return zFilename;
			//  zFilename += StringExtensions.sqlite3Strlen30(zFilename) + 1;
			//}
			return null;
		}
	}
    class OpenMode
    {
        public string z;
        public int mode;
        public OpenMode(string z, int mode)
        {
            this.z = z;
            this.mode = mode;
        }
    }
    public class _aFlagOp
    {
        public int op;
        ///
        ///<summary>
        ///The opcode 
        ///</summary>
        public SqliteFlags mask;
        ///
        ///<summary>
        ///Mask of the bit in sqlite3.flags to set/clear 
        ///</summary>
        public _aFlagOp(int op, SqliteFlags mask)
        {
            this.op = op;
            this.mask = mask;
        }
    }
		
}
