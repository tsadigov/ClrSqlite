#define SQLITE_OS_WIN
using System.Diagnostics;
namespace Community.CsharpSqlite
{

    	#if SQLITE_MUTEX_OMIT
		///
///<summary>
        ///If this is a no-op implementation, implement everything as macros.
///</summary>


		public class sqlite3_mutex
		{
            
		}
        public static class sqlite3_mutex_Extensions {
            //#define sqlite3_mutex_free(X)
            public static void sqlite3_mutex_enter(this sqlite3_mutex p)
            {
            }

            //#define sqlite3_mutex_try(X)      Sqlite3.SQLITE_OK
            public static void sqlite3_mutex_leave(this sqlite3_mutex p)
            {
            }

            //#define X.sqlite3_mutex_leave()
            public static bool sqlite3_mutex_held(this sqlite3_mutex p)
            {
                return true;
            }

            //#define Sqlite3.X.sqlite3_mutex_held()     ((void)(X),1)
            public static bool sqlite3_mutex_notheld(this sqlite3_mutex p)
            {
                return true;
            }
        }
#endif
	public partial class Sqlite3
	{
		///
///<summary>
///2007 August 28
///
///The author disclaims copyright to this source code.  In place of
///a legal notice, here is a blessing:
///
///May you do good and not evil.
///May you find forgiveness for yourself and forgive others.
///May you share freely, never taking more than you give.
///
///
///
///This file contains the common header for all mutex implementations.
///The sqliteInt.h header #includes this file so that it is available
///to all source files.  We break it out in an effort to keep the code
///better organized.
///
///NOTE:  source files should *not* #include this header file directly.
///Source files should #include the sqliteInt.h file and let that file
///include this one indirectly.
///
///</summary>
///<param name="Included in SQLite3 port to C#">SQLite;  2008 Noah B Hart</param>
///<param name="C#">SQLite is an independent reimplementation of the SQLite software library</param>
///<param name=""></param>
///<param name="SQLITE_SOURCE_ID: 2010">07 20:14:09 a586a4deeb25330037a49df295b36aaf624d0f45</param>
///<param name=""></param>
///<param name=""></param>
///<param name=""></param>

		///
///<summary>
///Figure out what version of the code to use.  The choices are
///
///SQLITE_MUTEX_OMIT         No mutex logic.  Not even stubs.  The
///mutexes implemention cannot be overridden
///</summary>
///<param name="at start">time.</param>
///<param name=""></param>
///<param name="SQLITE_MUTEX_NOOP         For single">threaded applications.  No</param>
///<param name="mutual exclusion is provided.  But this">mutual exclusion is provided.  But this</param>
///<param name="implementation can be overridden at">implementation can be overridden at</param>
///<param name="start">time.</param>
///<param name=""></param>
///<param name="SQLITE_MUTEX_PTHREADS     For multi">threaded applications on Unix.</param>
///<param name=""></param>
///<param name="SQLITE_MUTEX_W32          For multi">threaded applications on Win32.</param>
///<param name=""></param>
///<param name="SQLITE_MUTEX_OS2          For multi">threaded applications on OS/2.</param>
///<param name=""></param>

		//#if !SQLITE_THREADSAFE
		//# define SQLITE_MUTEX_OMIT
		//#endif
		//#if SQLITE_THREADSAFE && !defined(SQLITE_MUTEX_NOOP)
		//#  if SQLITE_OS_UNIX
		//#    define SQLITE_MUTEX_PTHREADS
		//#  elif SQLITE_OS_WIN
		//#    define SQLITE_MUTEX_W32
		//#  elif SQLITE_OS_OS2
		//#    define SQLITE_MUTEX_OS2
		//#  else
		//#    define SQLITE_MUTEX_NOOP
		//#  endif
		//#endif
		#if WINDOWS_PHONE && SQLITE_THREADSAFE
																																						#error Cannot compile with both WINDOWS_PHONE and SQLITE_THREADSAFE
																																						#endif
		#if SQLITE_SILVERLIGHT && SQLITE_THREADSAFE
																																						#error Cannot compile with both SQLITE_SILVERLIGHT and SQLITE_THREADSAFE
																																						#endif
		#if SQLITE_THREADSAFE && SQLITE_MUTEX_NOOP
																																						#error Cannot compile with both SQLITE_THREADSAFE and SQLITE_MUTEX_NOOP
																																						#endif
		#if SQLITE_THREADSAFE && SQLITE_MUTEX_OMIT
																																						#error Cannot compile with both SQLITE_THREADSAFE and SQLITE_MUTEX_OMIT
																																						#endif
		#if SQLITE_MUTEX_OMIT && SQLITE_MUTEX_NOOP
																																						#error Cannot compile with both SQLITE_MUTEX_OMIT and SQLITE_MUTEX_NOOP
																																						#endif
		#if SQLITE_MUTEX_OMIT && SQLITE_MUTEX_W32
																																						#error Cannot compile with both SQLITE_MUTEX_OMIT and SQLITE_MUTEX_W32
																																						#endif
		#if SQLITE_MUTEX_NOOP && SQLITE_MUTEX_W32
																																						#error Cannot compile with both SQLITE_MUTEX_NOOP and SQLITE_MUTEX_W32
																																						#endif
		#if SQLITE_MUTEX_OMIT
	

		static sqlite3_mutex mutex = null;

		//sqlite3_mutex sqlite3_mutex;
		public static sqlite3_mutex sqlite3MutexAlloc (int iType)
		{
			return new sqlite3_mutex ();
		}

		//#define sqlite3MutexAlloc(X)      ((sqlite3_mutex*)8)
		static sqlite3_mutex sqlite3_mutex_alloc (int iType)
		{
			return new sqlite3_mutex ();
		}

		//#define sqlite3_mutex_alloc(X)    ((sqlite3_mutex*)8)
		static void sqlite3_mutex_free (sqlite3_mutex m)
		{
		}

		

		//#define $.sqlite3_mutex_enter()
		static int sqlite3_mutex_try (int iType)
		{
			return Sqlite3.SQLITE_OK;
		}

		

		//#define sqlite3_mutex_notheld(X)  ((void)(X),1)
		static int sqlite3MutexInit ()
		{
			return Sqlite3.SQLITE_OK;
		}

		//#define sqlite3MutexInit()        Sqlite3.SQLITE_OK
		static void sqlite3MutexEnd ()
		{
		}
	//#define sqlite3MutexEnd()
	#endif



















        //SQLITE_API sqlite3_vfs *sqlite3_vfs_find(string zVfsName);
        //SQLITE_API int sqlite3_vfs_register(sqlite3_vfs*, int makeDflt);
        //SQLITE_API int sqlite3_vfs_unregister(sqlite3_vfs);
        ///
        ///<summary>
        ///CAPI3REF: Mutexes
        ///
        ///The SQLite core uses these routines for thread
        ///synchronization. Though they are intended for internal
        ///use by SQLite, code that links against SQLite is
        ///permitted to use any of these routines.
        ///
        ///The SQLite source code contains multiple implementations
        ///of these mutex routines.  An appropriate implementation
        ///</summary>
        ///<param name="is selected automatically at compile">time.  ^(The following</param>
        ///<param name="implementations are available in the SQLite core:">implementations are available in the SQLite core:</param>
        ///<param name=""></param>
        ///<param name="<ul>"><ul></param>
        ///<param name="<li>   SQLITE_MUTEX_OS2"><li>   SQLITE_MUTEX_OS2</param>
        ///<param name="<li>   SQLITE_MUTEX_PTHREAD"><li>   SQLITE_MUTEX_PTHREAD</param>
        ///<param name="<li>   SQLITE_MUTEX_W32"><li>   SQLITE_MUTEX_W32</param>
        ///<param name="<li>   SQLITE_MUTEX_NOOP"><li>   SQLITE_MUTEX_NOOP</param>
        ///<param name="</ul>)^"></ul>)^</param>
        ///<param name=""></param>
        ///<param name="^The SQLITE_MUTEX_NOOP implementation is a set of routines">^The SQLITE_MUTEX_NOOP implementation is a set of routines</param>
        ///<param name="that does no real locking and is appropriate for use in">that does no real locking and is appropriate for use in</param>
        ///<param name="a single">threaded application.  ^The SQLITE_MUTEX_OS2,</param>
        ///<param name="SQLITE_MUTEX_PTHREAD, and SQLITE_MUTEX_W32 implementations">SQLITE_MUTEX_PTHREAD, and SQLITE_MUTEX_W32 implementations</param>
        ///<param name="are appropriate for use on OS/2, Unix, and Windows.">are appropriate for use on OS/2, Unix, and Windows.</param>
        ///<param name=""></param>
        ///<param name="^(If SQLite is compiled with the SQLITE_MUTEX_APPDEF preprocessor">^(If SQLite is compiled with the SQLITE_MUTEX_APPDEF preprocessor</param>
        ///<param name="macro defined (with "">DSQLITE_MUTEX_APPDEF=1"), then no mutex</param>
        ///<param name="implementation is included with the library. In this case the">implementation is included with the library. In this case the</param>
        ///<param name="application must supply a custom mutex implementation using the">application must supply a custom mutex implementation using the</param>
        ///<param name="[SQLITE_CONFIG_MUTEX] option of the sqlite3_config() function">[SQLITE_CONFIG_MUTEX] option of the sqlite3_config() function</param>
        ///<param name="before calling sqlite3_initialize() or any other public sqlite3_">before calling sqlite3_initialize() or any other public sqlite3_</param>
        ///<param name="function that calls sqlite3_initialize().)^">function that calls sqlite3_initialize().)^</param>
        ///<param name=""></param>
        ///<param name="^The sqlite3_mutex_alloc() routine allocates a new">^The sqlite3_mutex_alloc() routine allocates a new</param>
        ///<param name="mutex and returns a pointer to it. ^If it returns NULL">mutex and returns a pointer to it. ^If it returns NULL</param>
        ///<param name="that means that a mutex could not be allocated.  ^SQLite">that means that a mutex could not be allocated.  ^SQLite</param>
        ///<param name="will unwind its stack and return an error.  ^(The argument">will unwind its stack and return an error.  ^(The argument</param>
        ///<param name="to sqlite3_mutex_alloc() is one of these integer constants:">to sqlite3_mutex_alloc() is one of these integer constants:</param>
        ///<param name=""></param>
        ///<param name="<ul>"><ul></param>
        ///<param name="<li>  SQLITE_MUTEX_FAST"><li>  SQLITE_MUTEX_FAST</param>
        ///<param name="<li>  SQLITE_MUTEX_RECURSIVE"><li>  SQLITE_MUTEX_RECURSIVE</param>
        ///<param name="<li>  SQLITE_MUTEX_STATIC_MASTER"><li>  SQLITE_MUTEX_STATIC_MASTER</param>
        ///<param name="<li>  SQLITE_MUTEX_STATIC_MEM"><li>  SQLITE_MUTEX_STATIC_MEM</param>
        ///<param name="<li>  SQLITE_MUTEX_STATIC_MEM2"><li>  SQLITE_MUTEX_STATIC_MEM2</param>
        ///<param name="<li>  SQLITE_MUTEX_STATIC_PRNG"><li>  SQLITE_MUTEX_STATIC_PRNG</param>
        ///<param name="<li>  SQLITE_MUTEX_STATIC_LRU"><li>  SQLITE_MUTEX_STATIC_LRU</param>
        ///<param name="<li>  SQLITE_MUTEX_STATIC_LRU2"><li>  SQLITE_MUTEX_STATIC_LRU2</param>
        ///<param name="</ul>)^"></ul>)^</param>
        ///<param name=""></param>
        ///<param name="^The first two constants (SQLITE_MUTEX_FAST and SQLITE_MUTEX_RECURSIVE)">^The first two constants (SQLITE_MUTEX_FAST and SQLITE_MUTEX_RECURSIVE)</param>
        ///<param name="cause sqlite3_mutex_alloc() to create">cause sqlite3_mutex_alloc() to create</param>
        ///<param name="a new mutex.  ^The new mutex is recursive when SQLITE_MUTEX_RECURSIVE">a new mutex.  ^The new mutex is recursive when SQLITE_MUTEX_RECURSIVE</param>
        ///<param name="is used but not necessarily so when SQLITE_MUTEX_FAST is used.">is used but not necessarily so when SQLITE_MUTEX_FAST is used.</param>
        ///<param name="The mutex implementation does not need to make a distinction">The mutex implementation does not need to make a distinction</param>
        ///<param name="between SQLITE_MUTEX_RECURSIVE and SQLITE_MUTEX_FAST if it does">between SQLITE_MUTEX_RECURSIVE and SQLITE_MUTEX_FAST if it does</param>
        ///<param name="not want to.  ^SQLite will only request a recursive mutex in">not want to.  ^SQLite will only request a recursive mutex in</param>
        ///<param name="cases where it really needs one.  ^If a faster non">recursive mutex</param>
        ///<param name="implementation is available on the host platform, the mutex subsystem">implementation is available on the host platform, the mutex subsystem</param>
        ///<param name="might return such a mutex in response to SQLITE_MUTEX_FAST.">might return such a mutex in response to SQLITE_MUTEX_FAST.</param>
        ///<param name=""></param>
        ///<param name="^The other allowed parameters to sqlite3_mutex_alloc() (anything other">^The other allowed parameters to sqlite3_mutex_alloc() (anything other</param>
        ///<param name="than SQLITE_MUTEX_FAST and SQLITE_MUTEX_RECURSIVE) each return">than SQLITE_MUTEX_FAST and SQLITE_MUTEX_RECURSIVE) each return</param>
        ///<param name="a pointer to a static preexisting mutex.  ^Six static mutexes are">a pointer to a static preexisting mutex.  ^Six static mutexes are</param>
        ///<param name="used by the current version of SQLite.  Future versions of SQLite">used by the current version of SQLite.  Future versions of SQLite</param>
        ///<param name="may add additional static mutexes.  Static mutexes are for internal">may add additional static mutexes.  Static mutexes are for internal</param>
        ///<param name="use by SQLite only.  Applications that use SQLite mutexes should">use by SQLite only.  Applications that use SQLite mutexes should</param>
        ///<param name="use only the dynamic mutexes returned by SQLITE_MUTEX_FAST or">use only the dynamic mutexes returned by SQLITE_MUTEX_FAST or</param>
        ///<param name="SQLITE_MUTEX_RECURSIVE.">SQLITE_MUTEX_RECURSIVE.</param>
        ///<param name=""></param>
        ///<param name="^Note that if one of the dynamic mutex parameters (SQLITE_MUTEX_FAST">^Note that if one of the dynamic mutex parameters (SQLITE_MUTEX_FAST</param>
        ///<param name="or SQLITE_MUTEX_RECURSIVE) is used then sqlite3_mutex_alloc()">or SQLITE_MUTEX_RECURSIVE) is used then sqlite3_mutex_alloc()</param>
        ///<param name="returns a different mutex on every call.  ^But for the static">returns a different mutex on every call.  ^But for the static</param>
        ///<param name="mutex types, the same mutex is returned on every call that has">mutex types, the same mutex is returned on every call that has</param>
        ///<param name="the same type number.">the same type number.</param>
        ///<param name=""></param>
        ///<param name="^The sqlite3_mutex_free() routine deallocates a previously">^The sqlite3_mutex_free() routine deallocates a previously</param>
        ///<param name="allocated dynamic mutex.  ^SQLite is careful to deallocate every">allocated dynamic mutex.  ^SQLite is careful to deallocate every</param>
        ///<param name="dynamic mutex that it allocates.  The dynamic mutexes must not be in">dynamic mutex that it allocates.  The dynamic mutexes must not be in</param>
        ///<param name="use when they are deallocated.  Attempting to deallocate a static">use when they are deallocated.  Attempting to deallocate a static</param>
        ///<param name="mutex results in undefined behavior.  ^SQLite never deallocates">mutex results in undefined behavior.  ^SQLite never deallocates</param>
        ///<param name="a static mutex.">a static mutex.</param>
        ///<param name=""></param>
        ///<param name="^The sqlite3_mutex_enter() and sqlite3_mutex_try() routines attempt">^The sqlite3_mutex_enter() and sqlite3_mutex_try() routines attempt</param>
        ///<param name="to enter a mutex.  ^If another thread is already within the mutex,">to enter a mutex.  ^If another thread is already within the mutex,</param>
        ///<param name="sqlite3_mutex_enter() will block and sqlite3_mutex_try() will return">sqlite3_mutex_enter() will block and sqlite3_mutex_try() will return</param>
        ///<param name="SQLITE_BUSY.  ^The sqlite3_mutex_try() interface returns [Sqlite3.SQLITE_OK]">SQLITE_BUSY.  ^The sqlite3_mutex_try() interface returns [Sqlite3.SQLITE_OK]</param>
        ///<param name="upon successful entry.  ^(Mutexes created using">upon successful entry.  ^(Mutexes created using</param>
        ///<param name="SQLITE_MUTEX_RECURSIVE can be entered multiple times by the same thread.">SQLITE_MUTEX_RECURSIVE can be entered multiple times by the same thread.</param>
        ///<param name="In such cases the,">In such cases the,</param>
        ///<param name="mutex must be exited an equal number of times before another thread">mutex must be exited an equal number of times before another thread</param>
        ///<param name="can enter.)^  ^(If the same thread tries to enter any other">can enter.)^  ^(If the same thread tries to enter any other</param>
        ///<param name="kind of mutex more than once, the behavior is undefined.">kind of mutex more than once, the behavior is undefined.</param>
        ///<param name="SQLite will never exhibit">SQLite will never exhibit</param>
        ///<param name="such behavior in its own use of mutexes.)^">such behavior in its own use of mutexes.)^</param>
        ///<param name=""></param>
        ///<param name="^(Some systems (for example, Windows 95) do not support the operation">^(Some systems (for example, Windows 95) do not support the operation</param>
        ///<param name="implemented by sqlite3_mutex_try().  On those systems, sqlite3_mutex_try()">implemented by sqlite3_mutex_try().  On those systems, sqlite3_mutex_try()</param>
        ///<param name="will always return SQLITE_BUSY.  The SQLite core only ever uses">will always return SQLITE_BUSY.  The SQLite core only ever uses</param>
        ///<param name="sqlite3_mutex_try() as an optimization so this is acceptable behavior.)^">sqlite3_mutex_try() as an optimization so this is acceptable behavior.)^</param>
        ///<param name=""></param>
        ///<param name="^The sqlite3_mutex_leave() routine exits a mutex that was">^The sqlite3_mutex_leave() routine exits a mutex that was</param>
        ///<param name="previously entered by the same thread.   ^(The behavior">previously entered by the same thread.   ^(The behavior</param>
        ///<param name="is undefined if the mutex is not currently entered by the">is undefined if the mutex is not currently entered by the</param>
        ///<param name="calling thread or is not currently allocated.  SQLite will">calling thread or is not currently allocated.  SQLite will</param>
        ///<param name="never do either.)^">never do either.)^</param>
        ///<param name=""></param>
        ///<param name="^If the argument to sqlite3_mutex_enter(), sqlite3_mutex_try(), or">^If the argument to sqlite3_mutex_enter(), sqlite3_mutex_try(), or</param>
        ///<param name="sqlite3_mutex_leave() is a NULL pointer, then all three routines">sqlite3_mutex_leave() is a NULL pointer, then all three routines</param>
        ///<param name="behave as no">ops.</param>
        ///<param name=""></param>
        ///<param name="See also: [Sqlite3.sqlite3_mutex_held()] and [sqlite3_mutex_notheld()].">See also: [Sqlite3.sqlite3_mutex_held()] and [sqlite3_mutex_notheld()].</param>
        ///<param name=""></param>

        //SQLITE_API sqlite3_mutex *sqlite3_mutex_alloc(int);
        //SQLITE_API void sqlite3_mutex_free(sqlite3_mutex);
        //SQLITE_API void $.sqlite3_mutex_enter();
        //SQLITE_API int sqlite3_mutex_try(sqlite3_mutex);
        //SQLITE_API void sqlite3_mutex.sqlite3_mutex_leave();
        ///<summary>
        /// CAPI3REF: Mutex Methods Object
        ///
        /// An instance of this structure defines the low-level routines
        /// used to allocate and use mutexes.
        ///
        /// Usually, the default mutex implementations provided by SQLite are
        /// sufficient, however the user has the option of substituting a custom
        /// implementation for specialized deployments or systems for which SQLite
        /// does not provide a suitable implementation. In this case, the user
        /// creates and populates an instance of this structure to pass
        /// to sqlite3_config() along with the [SQLITE_CONFIG_MUTEX] option.
        /// Additionally, an instance of this structure can be used as an
        /// output variable when querying the system for the current mutex
        /// implementation, using the [SQLITE_CONFIG_GETMUTEX] option.
        ///
        /// ^The xMutexInit method defined by this structure is invoked as
        /// part of system initialization by the sqlite3_initialize() function.
        /// ^The xMutexInit routine is called by SQLite exactly once for each
        /// effective call to [sqlite3_initialize()].
        ///
        /// ^The xMutexEnd method defined by this structure is invoked as
        /// part of system shutdown by the sqlite3_shutdown() function. The
        /// implementation of this method is expected to release all outstanding
        /// resources obtained by the mutex methods implementation, especially
        /// those obtained by the xMutexInit method.  ^The xMutexEnd()
        /// interface is invoked exactly once for each call to [sqlite3_shutdown()].
        ///
        /// ^(The remaining seven methods defined by this structure (xMutexAlloc,
        /// xMutexFree, xMutexEnter, xMutexTry, xMutexLeave, xMutexHeld and
        /// xMutexNotheld) implement the following interfaces (respectively):
        ///
        /// <ul>
        ///   <li>  [sqlite3_mutex_alloc()] </li>
        ///   <li>  [sqlite3_mutex_free()] </li>
        ///   <li>  [sqlite3_mutex_enter()] </li>
        ///   <li>  [sqlite3_mutex_try()] </li>
        ///   <li>  [sqlite3_mutex_leave()] </li>
        ///   <li>  [Sqlite3.sqlite3_mutex_held()] </li>
        ///   <li>  [sqlite3_mutex_notheld()] </li>
        /// </ul>)^
        ///
        /// The only difference is that the public sqlite3_XXX functions enumerated
        /// above silently ignore any invocations that pass a NULL pointer instead
        /// of a valid mutex handle. The implementations of the methods defined
        /// by this structure are not required to handle this case, the results
        /// of passing a NULL pointer instead of a valid mutex handle are undefined
        /// (i.e. it is acceptable to provide an implementation that segfaults if
        /// it is passed a NULL pointer).
        ///
        /// The xMutexInit() method must be threadsafe.  ^It must be harmless to
        /// invoke xMutexInit() multiple times within the same process and without
        /// intervening calls to xMutexEnd().  Second and subsequent calls to
        /// xMutexInit() must be no-ops.
        ///
        /// ^xMutexInit() must not use SQLite memory allocation ([sqlite3_malloc()]
        /// and its associates).  ^Similarly, xMutexAlloc() must not use SQLite memory
        /// allocation for a static mutex.  ^However xMutexAlloc() may use SQLite
        /// memory allocation for a fast or recursive mutex.
        ///
        /// ^SQLite will invoke the xMutexEnd() method when [sqlite3_shutdown()] is
        /// called, but only if the prior call to xMutexInit returned Sqlite3.SQLITE_OK.
        /// If xMutexInit fails in any way, it is expected to clean up after itself
        /// prior to returning.
        ///
        ///</summary>
        //typedef struct sqlite3_mutex_methods sqlite3_mutex_methods;
        //struct sqlite3_mutex_methods {
        //  int (*xMutexInit)(void);
        //  int (*xMutexEnd)(void);
        //  sqlite3_mutex *(*xMutexAlloc)(int);
        //  void (*xMutexFree)(sqlite3_mutex );
        //  void (*xMutexEnter)(sqlite3_mutex );
        //  int (*xMutexTry)(sqlite3_mutex );
        //  void (*xMutexLeave)(sqlite3_mutex );
        //  int (*xMutexHeld)(sqlite3_mutex );
        //  int (*xMutexNotheld)(sqlite3_mutex );
        //};
        public class sqlite3_mutex_methods
        {
            public dxMutexInit xMutexInit;

            public dxMutexEnd xMutexEnd;

            public dxMutexAlloc xMutexAlloc;

            public dxMutexFree xMutexFree;

            public dxMutexEnter xMutexEnter;

            public dxMutexTry xMutexTry;

            public dxMutexLeave xMutexLeave;

            public dxMutexHeld xMutexHeld;

            public dxMutexNotheld xMutexNotheld;

            public sqlite3_mutex_methods()
            {
            }

            public sqlite3_mutex_methods(dxMutexInit xMutexInit, dxMutexEnd xMutexEnd, dxMutexAlloc xMutexAlloc, dxMutexFree xMutexFree, dxMutexEnter xMutexEnter, dxMutexTry xMutexTry, dxMutexLeave xMutexLeave, dxMutexHeld xMutexHeld, dxMutexNotheld xMutexNotheld)
            {
                this.xMutexInit = xMutexInit;
                this.xMutexEnd = xMutexEnd;
                this.xMutexAlloc = xMutexAlloc;
                this.xMutexFree = xMutexFree;
                this.xMutexEnter = xMutexEnter;
                this.xMutexTry = xMutexTry;
                this.xMutexLeave = xMutexLeave;
                this.xMutexHeld = xMutexHeld;
                this.xMutexNotheld = xMutexNotheld;
            }

            //Copy sqlite3_mutex_methods from existing 
            public void Copy(sqlite3_mutex_methods cp)
            {
                Debug.Assert(cp != null);
                this.xMutexInit = cp.xMutexInit;
                this.xMutexEnd = cp.xMutexEnd;
                this.xMutexAlloc = cp.xMutexAlloc;
                this.xMutexFree = cp.xMutexFree;
                this.xMutexEnter = cp.xMutexEnter;
                this.xMutexTry = cp.xMutexTry;
                this.xMutexLeave = cp.xMutexLeave;
                this.xMutexHeld = cp.xMutexHeld;
                this.xMutexNotheld = cp.xMutexNotheld;
            }
        }

        //Mutex Methods
        public delegate int dxMutexInit();

        public delegate int dxMutexEnd();

        public delegate sqlite3_mutex dxMutexAlloc(int iNumber);

        public delegate void dxMutexFree(sqlite3_mutex sm);

        public delegate void dxMutexEnter(sqlite3_mutex sm);

        public delegate int dxMutexTry(sqlite3_mutex sm);

        public delegate void dxMutexLeave(sqlite3_mutex sm);

        public delegate bool dxMutexHeld(sqlite3_mutex sm);

        public delegate bool dxMutexNotheld(sqlite3_mutex sm);


	}
}
