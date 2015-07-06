using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Community.CsharpSqlite
{

    public class config_cs
    {

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
        static SqlResult sqlite3_config(SqliteConfig op, sqlite3_pcache_methods ap)
        {
            //  va_list ap;
            var rc = SqlResult.SQLITE_OK;
            switch (op)
            {
                case SqliteConfig.PCACHE:
                    {
                        ///
                        ///<summary>
                        ///Specify an alternative malloc implementation 
                        ///</summary>
                        sqliteinth.sqlite3GlobalConfig.pcache = ap;
                        //sqliteinth.sqlite3GlobalConfig.pcache = (sqlite3_pcache_methods)_Custom.va_arg(ap, "sqlite3_pcache_methods");
                        break;
                    }
            }
            return rc;
        }
        static SqlResult sqlite3_config(SqliteConfig op, ref sqlite3_pcache_methods ap)
        {
            //  va_list ap;
            var rc = SqlResult.SQLITE_OK;
            switch (op)
            {
                case SqliteConfig.GETPCACHE:
                    {
                        if (sqliteinth.sqlite3GlobalConfig.pcache.xInit == null)
                        {
                            Sqlite3.sqlite3PCacheSetDefault();
                        }
                        ap = sqliteinth.sqlite3GlobalConfig.pcache;
                        //_Custom.va_arg(ap, sqlite3_pcache_methods) = sqliteinth.sqlite3GlobalConfig.pcache;
                        break;
                    }
            }
            return rc;
        }
        static SqlResult sqlite3_config(int op, sqlite3_mem_methods ap)
        {
            //  va_list ap;
            var rc = SqlResult.SQLITE_OK;
            switch ((SqliteConfig)op)
            {
                case SqliteConfig.MALLOC:
                    {
                        ///
                        ///<summary>
                        ///Specify an alternative malloc implementation 
                        ///</summary>
                        sqliteinth.sqlite3GlobalConfig.m = ap;
                        // (sqlite3_mem_methods)_Custom.va_arg( ap, "sqlite3_mem_methods" );
                        break;
                    }
            }
            return rc;
        }
        static SqlResult sqlite3_config(SqliteConfig op, ref sqlite3_mem_methods ap)
        {
            //  va_list ap;
            var rc = SqlResult.SQLITE_OK;
            switch (op)
            {
                case SqliteConfig.GETMALLOC:
                    {
                        ///
                        ///<summary>
                        ///Retrieve the current malloc() implementation 
                        ///</summary>
                        //if ( sqliteinth.sqlite3GlobalConfig.m.xMalloc == null ) sqlite3MemSetDefault();
                        ap = sqliteinth.sqlite3GlobalConfig.m;
                        //_Custom.va_arg(ap, sqlite3_mem_methods) =  sqliteinth.sqlite3GlobalConfig.m;
                        break;
                    }
            }
            return rc;
        }
    }
      
    
    public class Sqlite3Config
        {
            public bool bMemstat; /* True to enable memory status */
            public bool bCoreMutex; /* True to enable core mutexing */
            public bool bFullMutex; /* True to enable full mutexing */
            public bool bOpenUri; /* True to interpret filenames as URIs */
            public int mxStrlen; /* Maximum string length */
            public int szLookaside; /* Default lookaside buffer size */
            public int nLookaside; /* Default lookaside buffer count */
            public sqlite3_mem_methods m; /* Low-level memory allocation interface */
            public sqlite3_mutex_methods mutex; /* Low-level mutex interface */

            sqlite3_pcache_methods m_pcache;
            public sqlite3_pcache_methods pcache {
                get {
                    return m_pcache;
                }
                set {
                    m_pcache = value;
                }
            } 
            
            /* Low-level page-cache interface */
            public byte[] pHeap; /* Heap storage space */
            public int nHeap; /* Size of pHeap[] */
            public int mnReq, mxReq; /* Min and max heap requests sizes */
            public byte[][] pScratch2; /* Scratch memory */
            public byte[][] pScratch; /* Scratch memory */
            public int szScratch; /* Size of each scratch buffer */
            public int nScratch; /* Number of scratch buffers */
            public MemPage pPage; /* Page cache memory */
            public int szPage; /* Size of each page in pPage[] */
            public int nPage; /* Number of pages in pPage[] */
            public int mxParserStack; /* maximum depth of the parser stack */
            public bool sharedCacheEnabled; /* true if shared-cache mode enabled */
            /* The above might be initialized to non-zero.  The following need to always
      ** initially be zero, however. */
            public int isInit; /* True after initialization has finished */
            public int inProgress; /* True while initialization in progress */
            public int isMutexInit; /* True after mutexes are initialized */
            public int isMallocInit; /* True after malloc is initialized */
            public int isPCacheInit; /* True after malloc is initialized */
            public sqlite3_mutex pInitMutex; /* Mutex used by sqlite3_initialize() */
            public int nRefInitMutex; /* Number of users of pInitMutex */
            public dxLog xLog; //void (*xLog)(void*,int,const char); /* Function for logging */
            public object pLogArg; /* First argument to xLog() */
            public bool bLocaltimeFault; /* True to fail localtime() calls */

            public Sqlite3Config(
                int bMemstat
                , int bCoreMutex
                , bool bFullMutex
                , bool bOpenUri
                , int mxStrlen
                , int szLookaside
                , int nLookaside
                , sqlite3_mem_methods m
                , sqlite3_mutex_methods mutex
                , sqlite3_pcache_methods pcache
                , byte[] pHeap
                , int nHeap
                , int mnReq
                , int mxReq
                , byte[][] pScratch
                , int szScratch
                , int nScratch
                , MemPage pPage
                , int szPage
                , int nPage
                , int mxParserStack
                , bool sharedCacheEnabled
                , int isInit
                , int inProgress
                , int isMutexInit
                , int isMallocInit
                , int isPCacheInit
                , sqlite3_mutex pInitMutex
                , int nRefInitMutex
                , dxLog xLog
                , object pLogArg
                , bool bLocaltimeFault
                )
            {
                this.bMemstat = bMemstat != 0;
                this.bCoreMutex = bCoreMutex != 0;
                this.bOpenUri = bOpenUri;
                this.bFullMutex = bFullMutex;
                this.mxStrlen = mxStrlen;
                this.szLookaside = szLookaside;
                this.nLookaside = nLookaside;
                this.m = m;
                this.mutex = mutex;
                this.pcache = pcache;
                this.pHeap = pHeap;
                this.nHeap = nHeap;
                this.mnReq = mnReq;
                this.mxReq = mxReq;
                this.pScratch = pScratch;
                this.szScratch = szScratch;
                this.nScratch = nScratch;
                this.pPage = pPage;
                this.szPage = szPage;
                this.nPage = nPage;
                this.mxParserStack = mxParserStack;
                this.sharedCacheEnabled = sharedCacheEnabled;
                this.isInit = isInit;
                this.inProgress = inProgress;
                this.isMutexInit = isMutexInit;
                this.isMallocInit = isMallocInit;
                this.isPCacheInit = isPCacheInit;
                this.pInitMutex = pInitMutex;
                this.nRefInitMutex = nRefInitMutex;
                this.xLog = xLog;
                this.pLogArg = pLogArg;
                this.bLocaltimeFault = bLocaltimeFault;
            }
        };




        ///
        ///<summary>
        ///CAPI3REF: Configuration Options
        ///KEYWORDS: {configuration option}
        ///
        ///These constants are the available integer configuration options that
        ///can be passed as the first argument to the [sqlite3_config()] interface.
        ///
        ///New configuration options may be added in future releases of SQLite.
        ///Existing configuration options might be discontinued.  Applications
        ///should check the return code from [sqlite3_config()] to make sure that
        ///the call worked.  The [sqlite3_config()] interface will return a
        ///</summary>
        ///<param name="non">zero [error code] if a discontinued or unsupported configuration option</param>
        ///<param name="is invoked.">is invoked.</param>
        ///<param name=""></param>
        ///<param name="<dl>"><dl></param>
        ///<param name="[[SQLITE_CONFIG_SINGLETHREAD]] <dt>SQLITE_CONFIG_SINGLETHREAD</dt>">[[SQLITE_CONFIG_SINGLETHREAD]] <dt>SQLITE_CONFIG_SINGLETHREAD</dt></param>
        ///<param name="<dd>There are no arguments to this option.  ^This option sets the"><dd>There are no arguments to this option.  ^This option sets the</param>
        ///<param name="[threading mode] to Single">thread.  In other words, it disables</param>
        ///<param name="all mutexing and puts SQLite into a mode where it can only be used">all mutexing and puts SQLite into a mode where it can only be used</param>
        ///<param name="by a single thread.   ^If SQLite is compiled with">by a single thread.   ^If SQLite is compiled with</param>
        ///<param name="the [SQLITE_THREADSAFE | SQLITE_THREADSAFE=0] compile">time option then</param>
        ///<param name="it is not possible to change the [threading mode] from its default">it is not possible to change the [threading mode] from its default</param>
        ///<param name="value of Single">thread and so [sqlite3_config()] will return </param>
        ///<param name="[SqlResult.SQLITE_ERROR] if called with the SQLITE_CONFIG_SINGLETHREAD">[SqlResult.SQLITE_ERROR] if called with the SQLITE_CONFIG_SINGLETHREAD</param>
        ///<param name="configuration option.</dd>">configuration option.</dd></param>
        ///<param name=""></param>
        ///<param name="[[SQLITE_CONFIG_MULTITHREAD]] <dt>SQLITE_CONFIG_MULTITHREAD</dt>">[[SQLITE_CONFIG_MULTITHREAD]] <dt>SQLITE_CONFIG_MULTITHREAD</dt></param>
        ///<param name="<dd>There are no arguments to this option.  ^This option sets the"><dd>There are no arguments to this option.  ^This option sets the</param>
        ///<param name="[threading mode] to Multi">thread.  In other words, it disables</param>
        ///<param name="mutexing on [database connection] and [prepared statement] objects.">mutexing on [database connection] and [prepared statement] objects.</param>
        ///<param name="The application is responsible for serializing access to">The application is responsible for serializing access to</param>
        ///<param name="[database connections] and [prepared statements].  But other mutexes">[database connections] and [prepared statements].  But other mutexes</param>
        ///<param name="are enabled so that SQLite will be safe to use in a multi">threaded</param>
        ///<param name="environment as long as no two threads attempt to use the same">environment as long as no two threads attempt to use the same</param>
        ///<param name="[database connection] at the same time.  ^If SQLite is compiled with">[database connection] at the same time.  ^If SQLite is compiled with</param>
        ///<param name="the [SQLITE_THREADSAFE | SQLITE_THREADSAFE=0] compile">time option then</param>
        ///<param name="it is not possible to set the Multi">thread [threading mode] and</param>
        ///<param name="[sqlite3_config()] will return [SqlResult.SQLITE_ERROR] if called with the">[sqlite3_config()] will return [SqlResult.SQLITE_ERROR] if called with the</param>
        ///<param name="SQLITE_CONFIG_MULTITHREAD configuration option.</dd>">SQLITE_CONFIG_MULTITHREAD configuration option.</dd></param>
        ///<param name=""></param>
        ///<param name="[[SQLITE_CONFIG_SERIALIZED]] <dt>SQLITE_CONFIG_SERIALIZED</dt>">[[SQLITE_CONFIG_SERIALIZED]] <dt>SQLITE_CONFIG_SERIALIZED</dt></param>
        ///<param name="<dd>There are no arguments to this option.  ^This option sets the"><dd>There are no arguments to this option.  ^This option sets the</param>
        ///<param name="[threading mode] to Serialized. In other words, this option enables">[threading mode] to Serialized. In other words, this option enables</param>
        ///<param name="all mutexes including the recursive">all mutexes including the recursive</param>
        ///<param name="mutexes on [database connection] and [prepared statement] objects.">mutexes on [database connection] and [prepared statement] objects.</param>
        ///<param name="In this mode (which is the default when SQLite is compiled with">In this mode (which is the default when SQLite is compiled with</param>
        ///<param name="[SQLITE_THREADSAFE=1]) the SQLite library will itself serialize access">[SQLITE_THREADSAFE=1]) the SQLite library will itself serialize access</param>
        ///<param name="to [database connections] and [prepared statements] so that the">to [database connections] and [prepared statements] so that the</param>
        ///<param name="application is free to use the same [database connection] or the">application is free to use the same [database connection] or the</param>
        ///<param name="same [prepared statement] in different threads at the same time.">same [prepared statement] in different threads at the same time.</param>
        ///<param name="^If SQLite is compiled with">^If SQLite is compiled with</param>
        ///<param name="the [SQLITE_THREADSAFE | SQLITE_THREADSAFE=0] compile">time option then</param>
        ///<param name="it is not possible to set the Serialized [threading mode] and">it is not possible to set the Serialized [threading mode] and</param>
        ///<param name="[sqlite3_config()] will return [SqlResult.SQLITE_ERROR] if called with the">[sqlite3_config()] will return [SqlResult.SQLITE_ERROR] if called with the</param>
        ///<param name="SQLITE_CONFIG_SERIALIZED configuration option.</dd>">SQLITE_CONFIG_SERIALIZED configuration option.</dd></param>
        ///<param name=""></param>
        ///<param name="[[SQLITE_CONFIG_MALLOC]] <dt>SQLITE_CONFIG_MALLOC</dt>">[[SQLITE_CONFIG_MALLOC]] <dt>SQLITE_CONFIG_MALLOC</dt></param>
        ///<param name="<dd> ^(This option takes a single argument which is a pointer to an"><dd> ^(This option takes a single argument which is a pointer to an</param>
        ///<param name="instance of the [sqlite3_mem_methods] structure.  The argument specifies">instance of the [sqlite3_mem_methods] structure.  The argument specifies</param>
        ///<param name="alternative low">level memory allocation routines to be used in place of</param>
        ///<param name="the memory allocation routines built into SQLite.)^ ^SQLite makes">the memory allocation routines built into SQLite.)^ ^SQLite makes</param>
        ///<param name="its own private copy of the content of the [sqlite3_mem_methods] structure">its own private copy of the content of the [sqlite3_mem_methods] structure</param>
        ///<param name="before the [sqlite3_config()] call returns.</dd>">before the [sqlite3_config()] call returns.</dd></param>
        ///<param name=""></param>
        ///<param name="[[SQLITE_CONFIG_GETMALLOC]] <dt>SQLITE_CONFIG_GETMALLOC</dt>">[[SQLITE_CONFIG_GETMALLOC]] <dt>SQLITE_CONFIG_GETMALLOC</dt></param>
        ///<param name="<dd> ^(This option takes a single argument which is a pointer to an"><dd> ^(This option takes a single argument which is a pointer to an</param>
        ///<param name="instance of the [sqlite3_mem_methods] structure.  The [sqlite3_mem_methods]">instance of the [sqlite3_mem_methods] structure.  The [sqlite3_mem_methods]</param>
        ///<param name="structure is filled with the currently defined memory allocation routines.)^">structure is filled with the currently defined memory allocation routines.)^</param>
        ///<param name="This option can be used to overload the default memory allocation">This option can be used to overload the default memory allocation</param>
        ///<param name="routines with a wrapper that simulations memory allocation failure or">routines with a wrapper that simulations memory allocation failure or</param>
        ///<param name="tracks memory usage, for example. </dd>">tracks memory usage, for example. </dd></param>
        ///<param name=""></param>
        ///<param name="[[SQLITE_CONFIG_MEMSTATUS]] <dt>SQLITE_CONFIG_MEMSTATUS</dt>">[[SQLITE_CONFIG_MEMSTATUS]] <dt>SQLITE_CONFIG_MEMSTATUS</dt></param>
        ///<param name="<dd> ^This option takes single argument of type int, interpreted as a "><dd> ^This option takes single argument of type int, interpreted as a </param>
        ///<param name="boolean, which enables or disables the collection of memory allocation ">boolean, which enables or disables the collection of memory allocation </param>
        ///<param name="statistics. ^(When memory allocation statistics are disabled, the ">statistics. ^(When memory allocation statistics are disabled, the </param>
        ///<param name="following SQLite interfaces become non">operational:</param>
        ///<param name="<ul>"><ul></param>
        ///<param name="<li> [sqlite3_memory_used()]"><li> [sqlite3_memory_used()]</param>
        ///<param name="<li> [sqlite3_memory_highwater()]"><li> [sqlite3_memory_highwater()]</param>
        ///<param name="<li> [sqlite3_soft_heap_limit64()]"><li> [sqlite3_soft_heap_limit64()]</param>
        ///<param name="<li> [sqlite3_status()]"><li> [sqlite3_status()]</param>
        ///<param name="</ul>)^"></ul>)^</param>
        ///<param name="^Memory allocation statistics are enabled by default unless SQLite is">^Memory allocation statistics are enabled by default unless SQLite is</param>
        ///<param name="compiled with [SQLITE_DEFAULT_MEMSTATUS]=0 in which case memory">compiled with [SQLITE_DEFAULT_MEMSTATUS]=0 in which case memory</param>
        ///<param name="allocation statistics are disabled by default.">allocation statistics are disabled by default.</param>
        ///<param name="</dd>"></dd></param>
        ///<param name=""></param>
        ///<param name="[[SQLITE_CONFIG_SCRATCH]] <dt>SQLITE_CONFIG_SCRATCH</dt>">[[SQLITE_CONFIG_SCRATCH]] <dt>SQLITE_CONFIG_SCRATCH</dt></param>
        ///<param name="<dd> ^This option specifies a static memory buffer that SQLite can use for"><dd> ^This option specifies a static memory buffer that SQLite can use for</param>
        ///<param name="scratch memory.  There are three arguments:  A pointer an 8">byte</param>
        ///<param name="aligned memory buffer from which the scratch allocations will be">aligned memory buffer from which the scratch allocations will be</param>
        ///<param name="drawn, the size of each scratch allocation (sz),">drawn, the size of each scratch allocation (sz),</param>
        ///<param name="and the maximum number of scratch allocations (N).  The sz">and the maximum number of scratch allocations (N).  The sz</param>
        ///<param name="argument must be a multiple of 16.">argument must be a multiple of 16.</param>
        ///<param name="The first argument must be a pointer to an 8">byte aligned buffer</param>
        ///<param name="of at least sz*N bytes of memory.">of at least sz*N bytes of memory.</param>
        ///<param name="^SQLite will use no more than two scratch buffers per thread.  So">^SQLite will use no more than two scratch buffers per thread.  So</param>
        ///<param name="N should be set to twice the expected maximum number of threads.">N should be set to twice the expected maximum number of threads.</param>
        ///<param name="^SQLite will never require a scratch buffer that is more than 6">^SQLite will never require a scratch buffer that is more than 6</param>
        ///<param name="times the database page size. ^If SQLite needs needs additional">times the database page size. ^If SQLite needs needs additional</param>
        ///<param name="scratch memory beyond what is provided by this configuration option, then ">scratch memory beyond what is provided by this configuration option, then </param>
        ///<param name="[sqlite3_malloc()] will be used to obtain the memory needed.</dd>">[sqlite3_malloc()] will be used to obtain the memory needed.</dd></param>
        ///<param name=""></param>
        ///<param name="[[SQLITE_CONFIG_PAGECACHE]] <dt>SQLITE_CONFIG_PAGECACHE</dt>">[[SQLITE_CONFIG_PAGECACHE]] <dt>SQLITE_CONFIG_PAGECACHE</dt></param>
        ///<param name="<dd> ^This option specifies a static memory buffer that SQLite can use for"><dd> ^This option specifies a static memory buffer that SQLite can use for</param>
        ///<param name="the database page cache with the default page cache implementation.  ">the database page cache with the default page cache implementation.  </param>
        ///<param name="This configuration should not be used if an application">define page</param>
        ///<param name="cache implementation is loaded using the SQLITE_CONFIG_PCACHE option.">cache implementation is loaded using the SQLITE_CONFIG_PCACHE option.</param>
        ///<param name="There are three arguments to this option: A pointer to 8">byte aligned</param>
        ///<param name="memory, the size of each page buffer (sz), and the number of pages (N).">memory, the size of each page buffer (sz), and the number of pages (N).</param>
        ///<param name="The sz argument should be the size of the largest database page">The sz argument should be the size of the largest database page</param>
        ///<param name="(a power of two between 512 and 32768) plus a little extra for each">(a power of two between 512 and 32768) plus a little extra for each</param>
        ///<param name="page header.  ^The page header size is 20 to 40 bytes depending on">page header.  ^The page header size is 20 to 40 bytes depending on</param>
        ///<param name="the host architecture.  ^It is harmless, apart from the wasted memory,">the host architecture.  ^It is harmless, apart from the wasted memory,</param>
        ///<param name="to make sz a little too large.  The first">to make sz a little too large.  The first</param>
        ///<param name="argument should point to an allocation of at least sz*N bytes of memory.">argument should point to an allocation of at least sz*N bytes of memory.</param>
        ///<param name="^SQLite will use the memory provided by the first argument to satisfy its">^SQLite will use the memory provided by the first argument to satisfy its</param>
        ///<param name="memory needs for the first N pages that it adds to cache.  ^If additional">memory needs for the first N pages that it adds to cache.  ^If additional</param>
        ///<param name="page cache memory is needed beyond what is provided by this option, then">page cache memory is needed beyond what is provided by this option, then</param>
        ///<param name="SQLite goes to [sqlite3_malloc()] for the additional storage space.">SQLite goes to [sqlite3_malloc()] for the additional storage space.</param>
        ///<param name="The pointer in the first argument must">The pointer in the first argument must</param>
        ///<param name="be aligned to an 8">byte boundary or subsequent behavior of SQLite</param>
        ///<param name="will be undefined.</dd>">will be undefined.</dd></param>
        ///<param name=""></param>
        ///<param name="[[SQLITE_CONFIG_HEAP]] <dt>SQLITE_CONFIG_HEAP</dt>">[[SQLITE_CONFIG_HEAP]] <dt>SQLITE_CONFIG_HEAP</dt></param>
        ///<param name="<dd> ^This option specifies a static memory buffer that SQLite will use"><dd> ^This option specifies a static memory buffer that SQLite will use</param>
        ///<param name="for all of its dynamic memory allocation needs beyond those provided">for all of its dynamic memory allocation needs beyond those provided</param>
        ///<param name="for by [SQLITE_CONFIG_SCRATCH] and [SQLITE_CONFIG_PAGECACHE].">for by [SQLITE_CONFIG_SCRATCH] and [SQLITE_CONFIG_PAGECACHE].</param>
        ///<param name="There are three arguments: An 8">byte aligned pointer to the memory,</param>
        ///<param name="the number of bytes in the memory buffer, and the minimum allocation size.">the number of bytes in the memory buffer, and the minimum allocation size.</param>
        ///<param name="^If the first pointer (the memory pointer) is NULL, then SQLite reverts">^If the first pointer (the memory pointer) is NULL, then SQLite reverts</param>
        ///<param name="to using its default memory allocator (the system malloc() implementation),">to using its default memory allocator (the system malloc() implementation),</param>
        ///<param name="undoing any prior invocation of [SQLITE_CONFIG_MALLOC].  ^If the">undoing any prior invocation of [SQLITE_CONFIG_MALLOC].  ^If the</param>
        ///<param name="memory pointer is not NULL and either [SQLITE_ENABLE_MEMSYS3] or">memory pointer is not NULL and either [SQLITE_ENABLE_MEMSYS3] or</param>
        ///<param name="[SQLITE_ENABLE_MEMSYS5] are defined, then the alternative memory">[SQLITE_ENABLE_MEMSYS5] are defined, then the alternative memory</param>
        ///<param name="allocator is engaged to handle all of SQLites memory allocation needs.">allocator is engaged to handle all of SQLites memory allocation needs.</param>
        ///<param name="The first pointer (the memory pointer) must be aligned to an 8">byte</param>
        ///<param name="boundary or subsequent behavior of SQLite will be undefined.">boundary or subsequent behavior of SQLite will be undefined.</param>
        ///<param name="The minimum allocation size is capped at 2^12. Reasonable values">The minimum allocation size is capped at 2^12. Reasonable values</param>
        ///<param name="for the minimum allocation size are 2^5 through 2^8.</dd>">for the minimum allocation size are 2^5 through 2^8.</dd></param>
        ///<param name=""></param>
        ///<param name="[[SQLITE_CONFIG_MUTEX]] <dt>SQLITE_CONFIG_MUTEX</dt>">[[SQLITE_CONFIG_MUTEX]] <dt>SQLITE_CONFIG_MUTEX</dt></param>
        ///<param name="<dd> ^(This option takes a single argument which is a pointer to an"><dd> ^(This option takes a single argument which is a pointer to an</param>
        ///<param name="instance of the [sqlite3_mutex_methods] structure.  The argument specifies">instance of the [sqlite3_mutex_methods] structure.  The argument specifies</param>
        ///<param name="alternative low">level mutex routines to be used in place</param>
        ///<param name="the mutex routines built into SQLite.)^  ^SQLite makes a copy of the">the mutex routines built into SQLite.)^  ^SQLite makes a copy of the</param>
        ///<param name="content of the [sqlite3_mutex_methods] structure before the call to">content of the [sqlite3_mutex_methods] structure before the call to</param>
        ///<param name="[sqlite3_config()] returns. ^If SQLite is compiled with">[sqlite3_config()] returns. ^If SQLite is compiled with</param>
        ///<param name="the [SQLITE_THREADSAFE | SQLITE_THREADSAFE=0] compile">time option then</param>
        ///<param name="the entire mutexing subsystem is omitted from the build and hence calls to">the entire mutexing subsystem is omitted from the build and hence calls to</param>
        ///<param name="[sqlite3_config()] with the SQLITE_CONFIG_MUTEX configuration option will">[sqlite3_config()] with the SQLITE_CONFIG_MUTEX configuration option will</param>
        ///<param name="return [SqlResult.SQLITE_ERROR].</dd>">return [SqlResult.SQLITE_ERROR].</dd></param>
        ///<param name=""></param>
        ///<param name="[[SQLITE_CONFIG_GETMUTEX]] <dt>SQLITE_CONFIG_GETMUTEX</dt>">[[SQLITE_CONFIG_GETMUTEX]] <dt>SQLITE_CONFIG_GETMUTEX</dt></param>
        ///<param name="<dd> ^(This option takes a single argument which is a pointer to an"><dd> ^(This option takes a single argument which is a pointer to an</param>
        ///<param name="instance of the [sqlite3_mutex_methods] structure.  The">instance of the [sqlite3_mutex_methods] structure.  The</param>
        ///<param name="[sqlite3_mutex_methods]">[sqlite3_mutex_methods]</param>
        ///<param name="structure is filled with the currently defined mutex routines.)^">structure is filled with the currently defined mutex routines.)^</param>
        ///<param name="This option can be used to overload the default mutex allocation">This option can be used to overload the default mutex allocation</param>
        ///<param name="routines with a wrapper used to track mutex usage for performance">routines with a wrapper used to track mutex usage for performance</param>
        ///<param name="profiling or testing, for example.   ^If SQLite is compiled with">profiling or testing, for example.   ^If SQLite is compiled with</param>
        ///<param name="the [SQLITE_THREADSAFE | SQLITE_THREADSAFE=0] compile">time option then</param>
        ///<param name="the entire mutexing subsystem is omitted from the build and hence calls to">the entire mutexing subsystem is omitted from the build and hence calls to</param>
        ///<param name="[sqlite3_config()] with the SQLITE_CONFIG_GETMUTEX configuration option will">[sqlite3_config()] with the SQLITE_CONFIG_GETMUTEX configuration option will</param>
        ///<param name="return [SqlResult.SQLITE_ERROR].</dd>">return [SqlResult.SQLITE_ERROR].</dd></param>
        ///<param name=""></param>
        ///<param name="[[SQLITE_CONFIG_LOOKASIDE]] <dt>SQLITE_CONFIG_LOOKASIDE</dt>">[[SQLITE_CONFIG_LOOKASIDE]] <dt>SQLITE_CONFIG_LOOKASIDE</dt></param>
        ///<param name="<dd> ^(This option takes two arguments that determine the default"><dd> ^(This option takes two arguments that determine the default</param>
        ///<param name="memory allocation for the lookaside memory allocator on each">memory allocation for the lookaside memory allocator on each</param>
        ///<param name="[database connection].  The first argument is the">[database connection].  The first argument is the</param>
        ///<param name="size of each lookaside buffer slot and the second is the number of">size of each lookaside buffer slot and the second is the number of</param>
        ///<param name="slots allocated to each database connection.)^  ^(This option sets the">slots allocated to each database connection.)^  ^(This option sets the</param>
        ///<param name="<i>default</i> lookaside size. The [SQLITE_DBCONFIG_LOOKASIDE]"><i>default</i> lookaside size. The [SQLITE_DBCONFIG_LOOKASIDE]</param>
        ///<param name="verb to [sqlite3_db_config()] can be used to change the lookaside">verb to [sqlite3_db_config()] can be used to change the lookaside</param>
        ///<param name="configuration on individual connections.)^ </dd>">configuration on individual connections.)^ </dd></param>
        ///<param name=""></param>
        ///<param name="[[SQLITE_CONFIG_PCACHE]] <dt>SQLITE_CONFIG_PCACHE</dt>">[[SQLITE_CONFIG_PCACHE]] <dt>SQLITE_CONFIG_PCACHE</dt></param>
        ///<param name="<dd> ^(This option takes a single argument which is a pointer to"><dd> ^(This option takes a single argument which is a pointer to</param>
        ///<param name="an [sqlite3_pcache_methods] object.  This object specifies the interface">an [sqlite3_pcache_methods] object.  This object specifies the interface</param>
        ///<param name="to a custom page cache implementation.)^  ^SQLite makes a copy of the">to a custom page cache implementation.)^  ^SQLite makes a copy of the</param>
        ///<param name="object and uses it for page cache memory allocations.</dd>">object and uses it for page cache memory allocations.</dd></param>
        ///<param name=""></param>
        ///<param name="[[SQLITE_CONFIG_GETPCACHE]] <dt>SQLITE_CONFIG_GETPCACHE</dt>">[[SQLITE_CONFIG_GETPCACHE]] <dt>SQLITE_CONFIG_GETPCACHE</dt></param>
        ///<param name="<dd> ^(This option takes a single argument which is a pointer to an"><dd> ^(This option takes a single argument which is a pointer to an</param>
        ///<param name="[sqlite3_pcache_methods] object.  SQLite copies of the current">[sqlite3_pcache_methods] object.  SQLite copies of the current</param>
        ///<param name="page cache implementation into that object.)^ </dd>">page cache implementation into that object.)^ </dd></param>
        ///<param name=""></param>
        ///<param name="[[SQLITE_CONFIG_LOG]] <dt>SQLITE_CONFIG_LOG</dt>">[[SQLITE_CONFIG_LOG]] <dt>SQLITE_CONFIG_LOG</dt></param>
        ///<param name="<dd> ^The SQLITE_CONFIG_LOG option takes two arguments: a pointer to a"><dd> ^The SQLITE_CONFIG_LOG option takes two arguments: a pointer to a</param>
        ///<param name="function with a call signature of void()(void*,int,const char), ">function with a call signature of void()(void*,int,const char), </param>
        ///<param name="and a pointer to void. ^If the function pointer is not NULL, it is">and a pointer to void. ^If the function pointer is not NULL, it is</param>
        ///<param name="invoked by [io.sqlite3_log()] to process each logging event.  ^If the">invoked by [io.sqlite3_log()] to process each logging event.  ^If the</param>
        ///<param name="function pointer is NULL, the [io.sqlite3_log()] interface becomes a no">op.</param>
        ///<param name="^The void pointer that is the second argument to SQLITE_CONFIG_LOG is">^The void pointer that is the second argument to SQLITE_CONFIG_LOG is</param>
        ///<param name="passed through as the first parameter to the application">defined logger</param>
        ///<param name="function whenever that function is invoked.  ^The second parameter to">function whenever that function is invoked.  ^The second parameter to</param>
        ///<param name="the logger function is a copy of the first parameter to the corresponding">the logger function is a copy of the first parameter to the corresponding</param>
        ///<param name="[io.sqlite3_log()] call and is intended to be a [result code] or an">[io.sqlite3_log()] call and is intended to be a [result code] or an</param>
        ///<param name="[extended result code].  ^The third parameter passed to the logger is">[extended result code].  ^The third parameter passed to the logger is</param>
        ///<param name="log message after formatting via [sqlite3_snprintf()].">log message after formatting via [sqlite3_snprintf()].</param>
        ///<param name="The SQLite logging interface is not reentrant; the logger function">The SQLite logging interface is not reentrant; the logger function</param>
        ///<param name="supplied by the application must not invoke any SQLite interface.">supplied by the application must not invoke any SQLite interface.</param>
        ///<param name="In a multi">defined logger</param>
        ///<param name="function must be threadsafe. </dd>">function must be threadsafe. </dd></param>
        ///<param name=""></param>
        ///<param name="[[SQLITE_CONFIG_URI]] <dt>SQLITE_CONFIG_URI">[[SQLITE_CONFIG_URI]] <dt>SQLITE_CONFIG_URI</param>
        ///<param name="<dd> This option takes a single argument of type int. If non">zero, then</param>
        ///<param name="URI handling is globally enabled. If the parameter is zero, then URI handling">URI handling is globally enabled. If the parameter is zero, then URI handling</param>
        ///<param name="is globally disabled. If URI handling is globally enabled, all filenames">is globally disabled. If URI handling is globally enabled, all filenames</param>
        ///<param name="passed to [sqlite3_open()], [sqlite3_open_v2()], [sqlite3_open16()] or">passed to [sqlite3_open()], [sqlite3_open_v2()], [sqlite3_open16()] or</param>
        ///<param name="specified as part of [ATTACH] commands are interpreted as URIs, regardless">specified as part of [ATTACH] commands are interpreted as URIs, regardless</param>
        ///<param name="of whether or not the [SQLITE_OPEN_URI] flag is set when the database">of whether or not the [SQLITE_OPEN_URI] flag is set when the database</param>
        ///<param name="connection is opened. If it is globally disabled, filenames are">connection is opened. If it is globally disabled, filenames are</param>
        ///<param name="only interpreted as URIs if the SQLITE_OPEN_URI flag is set when the">only interpreted as URIs if the SQLITE_OPEN_URI flag is set when the</param>
        ///<param name="database connection is opened. By default, URI handling is globally">database connection is opened. By default, URI handling is globally</param>
        ///<param name="disabled. The default value may be changed by compiling with the">disabled. The default value may be changed by compiling with the</param>
        ///<param name="[SQLITE_USE_URI] symbol defined.">[SQLITE_USE_URI] symbol defined.</param>
        ///<param name="</dl>"></dl></param>
        ///<param name=""></param>

        //#define SQLITE_CONFIG_SINGLETHREAD  1  /* nil */
        //#define SQLITE_CONFIG_MULTITHREAD   2  /* nil */
        //#define SQLITE_CONFIG_SERIALIZED    3  /* nil */
        //#define SQLITE_CONFIG_MALLOC        4  /* sqlite3_mem_methods* */
        //#define SQLITE_CONFIG_GETMALLOC     5  /* sqlite3_mem_methods* */
        //#define SQLITE_CONFIG_SCRATCH       6  /* void*, int sz, int N */
        //#define SQLITE_CONFIG_PAGECACHE     7  /* void*, int sz, int N */
        //#define SQLITE_CONFIG_HEAP          8  /* void*, int nByte, int min */
        //#define SQLITE_CONFIG_MEMSTATUS     9  /* boolean */
        //#define SQLITE_CONFIG_MUTEX        10  /* sqlite3_mutex_methods* */
        //#define SQLITE_CONFIG_GETMUTEX     11  /* sqlite3_mutex_methods* */
        ///* previously SQLITE_CONFIG_CHUNKALLOC 12 which is now unused. */ 
        //#define SQLITE_CONFIG_LOOKASIDE    13  /* int int */
        //#define SQLITE_CONFIG_PCACHE       14  /* sqlite3_pcache_methods* */
        //#define SQLITE_CONFIG_GETPCACHE    15  /* sqlite3_pcache_methods* */
        //#define SQLITE_CONFIG_LOG          16  /* xFunc, void* */
        //#define SQLITE_CONFIG_URI          17  /* int */


        public enum SqliteConfig
        {
            SINGLETHREAD = 1,

            MULTITHREAD = 2,

            SERIALIZED = 3,

            MALLOC = 4,

            GETMALLOC = 5,

            SCRATCH = 6,

            PAGECACHE = 7,

            HEAP = 8,

            MEMSTATUS = 9,

            MUTEX = 10,

            GETMUTEX = 11,

            LOOKASIDE = 13,

            PCACHE = 14,

            GETPCACHE = 15,

            LOG = 16,

            URI = 17
        }
    }

