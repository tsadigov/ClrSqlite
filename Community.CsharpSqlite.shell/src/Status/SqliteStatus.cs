using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Community.CsharpSqlite
{

    ///
    ///<summary>
    ///CAPI3REF: SQLite Runtime Status
    ///
    ///^This interface is used to retrieve runtime status information
    ///about the performance of SQLite, and optionally to reset various
    ///highwater marks.  ^The first argument is an integer code for
    ///the specific parameter to measure.  ^(Recognized integer codes
    ///are of the form [status parameters | SQLITE_STATUS_...].)^
    ///^The current value of the parameter is returned into *pCurrent.
    ///^The highest recorded value is returned in *pHighwater.  ^If the
    ///resetFlag is true, then the highest record value is reset after
    ///pHighwater is written.  ^(Some parameters do not record the highest
    ///value.  For those parameters
    ///nothing is written into *pHighwater and the resetFlag is ignored.)^
    ///^(Other parameters record only the highwater mark and not the current
    ///value.  For these latter parameters nothing is written into *pCurrent.)^
    ///
    ///^The sqlite3_status() routine returns SqlResult.SQLITE_OK on success and a
    ///</summary>
    ///<param name="non">zero [error code] on failure.</param>
    ///<param name=""></param>
    ///<param name="This routine is threadsafe but is not atomic.  This routine can be">This routine is threadsafe but is not atomic.  This routine can be</param>
    ///<param name="called while other threads are running the same or different SQLite">called while other threads are running the same or different SQLite</param>
    ///<param name="interfaces.  However the values returned in *pCurrent and">interfaces.  However the values returned in *pCurrent and</param>
    ///<param name="pHighwater reflect the status of SQLite at different points in time">pHighwater reflect the status of SQLite at different points in time</param>
    ///<param name="and it is possible that another thread might change the parameter">and it is possible that another thread might change the parameter</param>
    ///<param name="in between the times when *pCurrent and *pHighwater are written.">in between the times when *pCurrent and *pHighwater are written.</param>
    ///<param name=""></param>
    ///<param name="See also: [sqlite3_db_status()]">See also: [sqlite3_db_status()]</param>
    ///<param name=""></param>

    //SQLITE_API int sqlite3_status(int op, int *pCurrent, int *pHighwater, int resetFlag);
    ///
    ///<summary>
    ///CAPI3REF: Status Parameters
    ///KEYWORDS: {status parameters}
    ///
    ///</summary>
    ///<param name="These integer constants designate various run">time status parameters</param>
    ///<param name="that can be returned by [sqlite3_status()].">that can be returned by [sqlite3_status()].</param>
    ///<param name=""></param>
    ///<param name="<dl>"><dl></param>
    ///<param name="[[SQLITE_STATUS_MEMORY_USED]] ^(<dt>SQLITE_STATUS_MEMORY_USED</dt>">[[SQLITE_STATUS_MEMORY_USED]] ^(<dt>SQLITE_STATUS_MEMORY_USED</dt></param>
    ///<param name="<dd>This parameter is the current amount of memory checked out"><dd>This parameter is the current amount of memory checked out</param>
    ///<param name="using [sqlite3_malloc()], either directly or indirectly.  The">using [sqlite3_malloc()], either directly or indirectly.  The</param>
    ///<param name="figure includes calls made to [sqlite3_malloc()] by the application">figure includes calls made to [sqlite3_malloc()] by the application</param>
    ///<param name="and internal memory usage by the SQLite library.  Scratch memory">and internal memory usage by the SQLite library.  Scratch memory</param>
    ///<param name="controlled by [SQLITE_CONFIG_SCRATCH] and auxiliary page">cache</param>
    ///<param name="memory controlled by [SQLITE_CONFIG_PAGECACHE] is not included in">memory controlled by [SQLITE_CONFIG_PAGECACHE] is not included in</param>
    ///<param name="this parameter.  The amount returned is the sum of the allocation">this parameter.  The amount returned is the sum of the allocation</param>
    ///<param name="sizes as reported by the xSize method in [sqlite3_mem_methods].</dd>)^">sizes as reported by the xSize method in [sqlite3_mem_methods].</dd>)^</param>
    ///<param name=""></param>
    ///<param name="[[SQLITE_STATUS_MALLOC_SIZE]] ^(<dt>SQLITE_STATUS_MALLOC_SIZE</dt>">[[SQLITE_STATUS_MALLOC_SIZE]] ^(<dt>SQLITE_STATUS_MALLOC_SIZE</dt></param>
    ///<param name="<dd>This parameter records the largest memory allocation request"><dd>This parameter records the largest memory allocation request</param>
    ///<param name="handed to [sqlite3_malloc()] or [sqlite3_realloc()] (or their">handed to [sqlite3_malloc()] or [sqlite3_realloc()] (or their</param>
    ///<param name="internal equivalents).  Only the value returned in the">internal equivalents).  Only the value returned in the</param>
    ///<param name="pHighwater parameter to [sqlite3_status()] is of interest.  ">pHighwater parameter to [sqlite3_status()] is of interest.  </param>
    ///<param name="The value written into the *pCurrent parameter is undefined.</dd>)^">The value written into the *pCurrent parameter is undefined.</dd>)^</param>
    ///<param name=""></param>
    ///<param name="[[SQLITE_STATUS_MALLOC_COUNT]] ^(<dt>SQLITE_STATUS_MALLOC_COUNT</dt>">[[SQLITE_STATUS_MALLOC_COUNT]] ^(<dt>SQLITE_STATUS_MALLOC_COUNT</dt></param>
    ///<param name="<dd>This parameter records the number of separate memory allocations"><dd>This parameter records the number of separate memory allocations</param>
    ///<param name="currently checked out.</dd>)^">currently checked out.</dd>)^</param>
    ///<param name=""></param>
    ///<param name="[[SQLITE_STATUS_PAGECACHE_USED]] ^(<dt>SQLITE_STATUS_PAGECACHE_USED</dt>">[[SQLITE_STATUS_PAGECACHE_USED]] ^(<dt>SQLITE_STATUS_PAGECACHE_USED</dt></param>
    ///<param name="<dd>This parameter returns the number of pages used out of the"><dd>This parameter returns the number of pages used out of the</param>
    ///<param name="[pagecache memory allocator] that was configured using ">[pagecache memory allocator] that was configured using </param>
    ///<param name="[SQLITE_CONFIG_PAGECACHE].  The">[SQLITE_CONFIG_PAGECACHE].  The</param>
    ///<param name="value returned is in pages, not in bytes.</dd>)^">value returned is in pages, not in bytes.</dd>)^</param>
    ///<param name=""></param>
    ///<param name="[[SQLITE_STATUS_PAGECACHE_OVERFLOW]] ">[[SQLITE_STATUS_PAGECACHE_OVERFLOW]] </param>
    ///<param name="^(<dt>SQLITE_STATUS_PAGECACHE_OVERFLOW</dt>">^(<dt>SQLITE_STATUS_PAGECACHE_OVERFLOW</dt></param>
    ///<param name="<dd>This parameter returns the number of bytes of page cache"><dd>This parameter returns the number of bytes of page cache</param>
    ///<param name="allocation which could not be satisfied by the [SQLITE_CONFIG_PAGECACHE]">allocation which could not be satisfied by the [SQLITE_CONFIG_PAGECACHE]</param>
    ///<param name="buffer and where forced to overflow to [sqlite3_malloc()].  The">buffer and where forced to overflow to [sqlite3_malloc()].  The</param>
    ///<param name="returned value includes allocations that overflowed because they">returned value includes allocations that overflowed because they</param>
    ///<param name="where too large (they were larger than the "sz" parameter to">where too large (they were larger than the "sz" parameter to</param>
    ///<param name="[SQLITE_CONFIG_PAGECACHE]) and allocations that overflowed because">[SQLITE_CONFIG_PAGECACHE]) and allocations that overflowed because</param>
    ///<param name="no space was left in the page cache.</dd>)^">no space was left in the page cache.</dd>)^</param>
    ///<param name=""></param>
    ///<param name="[[SQLITE_STATUS_PAGECACHE_SIZE]] ^(<dt>SQLITE_STATUS_PAGECACHE_SIZE</dt>">[[SQLITE_STATUS_PAGECACHE_SIZE]] ^(<dt>SQLITE_STATUS_PAGECACHE_SIZE</dt></param>
    ///<param name="<dd>This parameter records the largest memory allocation request"><dd>This parameter records the largest memory allocation request</param>
    ///<param name="handed to [pagecache memory allocator].  Only the value returned in the">handed to [pagecache memory allocator].  Only the value returned in the</param>
    ///<param name="pHighwater parameter to [sqlite3_status()] is of interest.  ">pHighwater parameter to [sqlite3_status()] is of interest.  </param>
    ///<param name="The value written into the *pCurrent parameter is undefined.</dd>)^">The value written into the *pCurrent parameter is undefined.</dd>)^</param>
    ///<param name=""></param>
    ///<param name="[[SQLITE_STATUS_SCRATCH_USED]] ^(<dt>SQLITE_STATUS_SCRATCH_USED</dt>">[[SQLITE_STATUS_SCRATCH_USED]] ^(<dt>SQLITE_STATUS_SCRATCH_USED</dt></param>
    ///<param name="<dd>This parameter returns the number of allocations used out of the"><dd>This parameter returns the number of allocations used out of the</param>
    ///<param name="[scratch memory allocator] configured using">[scratch memory allocator] configured using</param>
    ///<param name="[SQLITE_CONFIG_SCRATCH].  The value returned is in allocations, not">[SQLITE_CONFIG_SCRATCH].  The value returned is in allocations, not</param>
    ///<param name="in bytes.  Since a single thread may only have one scratch allocation">in bytes.  Since a single thread may only have one scratch allocation</param>
    ///<param name="outstanding at time, this parameter also reports the number of threads">outstanding at time, this parameter also reports the number of threads</param>
    ///<param name="using scratch memory at the same time.</dd>)^">using scratch memory at the same time.</dd>)^</param>
    ///<param name=""></param>
    ///<param name="[[SQLITE_STATUS_SCRATCH_OVERFLOW]] ^(<dt>SQLITE_STATUS_SCRATCH_OVERFLOW</dt>">[[SQLITE_STATUS_SCRATCH_OVERFLOW]] ^(<dt>SQLITE_STATUS_SCRATCH_OVERFLOW</dt></param>
    ///<param name="<dd>This parameter returns the number of bytes of scratch memory"><dd>This parameter returns the number of bytes of scratch memory</param>
    ///<param name="allocation which could not be satisfied by the [SQLITE_CONFIG_SCRATCH]">allocation which could not be satisfied by the [SQLITE_CONFIG_SCRATCH]</param>
    ///<param name="buffer and where forced to overflow to [sqlite3_malloc()].  The values">buffer and where forced to overflow to [sqlite3_malloc()].  The values</param>
    ///<param name="returned include overflows because the requested allocation was too">returned include overflows because the requested allocation was too</param>
    ///<param name="larger (that is, because the requested allocation was larger than the">larger (that is, because the requested allocation was larger than the</param>
    ///<param name=""sz" parameter to [SQLITE_CONFIG_SCRATCH]) and because no scratch buffer">"sz" parameter to [SQLITE_CONFIG_SCRATCH]) and because no scratch buffer</param>
    ///<param name="slots were available.">slots were available.</param>
    ///<param name="</dd>)^"></dd>)^</param>
    ///<param name=""></param>
    ///<param name="[[SQLITE_STATUS_SCRATCH_SIZE]] ^(<dt>SQLITE_STATUS_SCRATCH_SIZE</dt>">[[SQLITE_STATUS_SCRATCH_SIZE]] ^(<dt>SQLITE_STATUS_SCRATCH_SIZE</dt></param>
    ///<param name="<dd>This parameter records the largest memory allocation request"><dd>This parameter records the largest memory allocation request</param>
    ///<param name="handed to [scratch memory allocator].  Only the value returned in the">handed to [scratch memory allocator].  Only the value returned in the</param>
    ///<param name="pHighwater parameter to [sqlite3_status()] is of interest.  ">pHighwater parameter to [sqlite3_status()] is of interest.  </param>
    ///<param name="The value written into the *pCurrent parameter is undefined.</dd>)^">The value written into the *pCurrent parameter is undefined.</dd>)^</param>
    ///<param name=""></param>
    ///<param name="[[SQLITE_STATUS_PARSER_STACK]] ^(<dt>SQLITE_STATUS_PARSER_STACK</dt>">[[SQLITE_STATUS_PARSER_STACK]] ^(<dt>SQLITE_STATUS_PARSER_STACK</dt></param>
    ///<param name="<dd>This parameter records the deepest parser stack.  It is only"><dd>This parameter records the deepest parser stack.  It is only</param>
    ///<param name="meaningful if SQLite is compiled with [YYTRACKMAXSTACKDEPTH].</dd>)^">meaningful if SQLite is compiled with [YYTRACKMAXSTACKDEPTH].</dd>)^</param>
    ///<param name="</dl>"></dl></param>
    ///<param name=""></param>
    ///<param name="New status parameters may be added from time to time.">New status parameters may be added from time to time.</param>
    ///<param name=""></param>

    //#define SQLITE_STATUS_MEMORY_USED          0
    //#define SQLITE_STATUS_PAGECACHE_USED       1
    //#define SQLITE_STATUS_PAGECACHE_OVERFLOW   2
    //#define SQLITE_STATUS_SCRATCH_USED         3
    //#define SQLITE_STATUS_SCRATCH_OVERFLOW     4
    //#define SQLITE_STATUS_MALLOC_SIZE          5
    //#define SQLITE_STATUS_PARSER_STACK         6
    //#define SQLITE_STATUS_PAGECACHE_SIZE       7
    //#define SQLITE_STATUS_SCRATCH_SIZE         8
    //#define SQLITE_STATUS_MALLOC_COUNT         9
    public enum SqliteStatus {
    SQLITE_STATUS_MEMORY_USED = 0,

    SQLITE_STATUS_PAGECACHE_USED = 1,

    SQLITE_STATUS_PAGECACHE_OVERFLOW = 2,

    SQLITE_STATUS_SCRATCH_USED = 3,

    SQLITE_STATUS_SCRATCH_OVERFLOW = 4,

    SQLITE_STATUS_MALLOC_SIZE = 5,

    SQLITE_STATUS_PARSER_STACK = 6,

    SQLITE_STATUS_PAGECACHE_SIZE = 7,

    SQLITE_STATUS_SCRATCH_SIZE = 8,

    SQLITE_STATUS_MALLOC_COUNT = 9
}
    ///
    ///<summary>
    ///CAPI3REF: Database Connection Status
    ///
    ///^This interface is used to retrieve runtime status information 
    ///about a single [database connection].  ^The first argument is the
    ///database connection object to be interrogated.  ^The second argument
    ///is an integer constant, taken from the set of
    ///[SQLITE_DBSTATUS options], that
    ///determines the parameter to interrogate.  The set of 
    ///[SQLITE_DBSTATUS options] is likely
    ///to grow in future releases of SQLite.
    ///
    ///^The current value of the requested parameter is written into *pCur
    ///and the highest instantaneous value is written into *pHiwtr.  ^If
    ///the resetFlg is true, then the highest instantaneous value is
    ///reset back down to the current value.
    ///
    ///^The sqlite3_db_status() routine returns SqlResult.SQLITE_OK on success and a
    ///</summary>
    ///<param name="non">zero [error code] on failure.</param>
    ///<param name=""></param>
    ///<param name="See also: [sqlite3_status()] and [sqlite3_stmt_status()].">See also: [sqlite3_status()] and [sqlite3_stmt_status()].</param>
    ///<param name=""></param>

    //SQLITE_API int sqlite3_db_status(sqlite3*, int op, int *pCur, int *pHiwtr, int resetFlg);
    ///
    ///<summary>
    ///CAPI3REF: Status Parameters for database connections
    ///KEYWORDS: {SQLITE_DBSTATUS options}
    ///
    ///These constants are the available integer "verbs" that can be passed as
    ///the second argument to the [sqlite3_db_status()] interface.
    ///
    ///New verbs may be added in future releases of SQLite. Existing verbs
    ///might be discontinued. Applications should check the return code from
    ///[sqlite3_db_status()] to make sure that the call worked.
    ///</summary>
    ///<param name="The [sqlite3_db_status()] interface will return a non">zero error code</param>
    ///<param name="if a discontinued or unsupported verb is invoked.">if a discontinued or unsupported verb is invoked.</param>
    ///<param name=""></param>
    ///<param name="<dl>"><dl></param>
    ///<param name="[[SQLITE_DBSTATUS_LOOKASIDE_USED]] ^(<dt>SQLITE_DBSTATUS_LOOKASIDE_USED</dt>">[[SQLITE_DBSTATUS_LOOKASIDE_USED]] ^(<dt>SQLITE_DBSTATUS_LOOKASIDE_USED</dt></param>
    ///<param name="<dd>This parameter returns the number of lookaside memory slots currently"><dd>This parameter returns the number of lookaside memory slots currently</param>
    ///<param name="checked out.</dd>)^">checked out.</dd>)^</param>
    ///<param name=""></param>
    ///<param name="[[SQLITE_DBSTATUS_LOOKASIDE_HIT]] ^(<dt>SQLITE_DBSTATUS_LOOKASIDE_HIT</dt>">[[SQLITE_DBSTATUS_LOOKASIDE_HIT]] ^(<dt>SQLITE_DBSTATUS_LOOKASIDE_HIT</dt></param>
    ///<param name="<dd>This parameter returns the number malloc attempts that were "><dd>This parameter returns the number malloc attempts that were </param>
    ///<param name="satisfied using lookaside memory. Only the high">water value is meaningful;</param>
    ///<param name="the current value is always zero.)^">the current value is always zero.)^</param>
    ///<param name=""></param>
    ///<param name="[[SQLITE_DBSTATUS_LOOKASIDE_MISS_SIZE]]">[[SQLITE_DBSTATUS_LOOKASIDE_MISS_SIZE]]</param>
    ///<param name="^(<dt>SQLITE_DBSTATUS_LOOKASIDE_MISS_SIZE</dt>">^(<dt>SQLITE_DBSTATUS_LOOKASIDE_MISS_SIZE</dt></param>
    ///<param name="<dd>This parameter returns the number malloc attempts that might have"><dd>This parameter returns the number malloc attempts that might have</param>
    ///<param name="been satisfied using lookaside memory but failed due to the amount of">been satisfied using lookaside memory but failed due to the amount of</param>
    ///<param name="memory requested being larger than the lookaside slot size.">memory requested being larger than the lookaside slot size.</param>
    ///<param name="Only the high">water value is meaningful;</param>
    ///<param name="the current value is always zero.)^">the current value is always zero.)^</param>
    ///<param name=""></param>
    ///<param name="[[SQLITE_DBSTATUS_LOOKASIDE_MISS_FULL]]">[[SQLITE_DBSTATUS_LOOKASIDE_MISS_FULL]]</param>
    ///<param name="^(<dt>SQLITE_DBSTATUS_LOOKASIDE_MISS_FULL</dt>">^(<dt>SQLITE_DBSTATUS_LOOKASIDE_MISS_FULL</dt></param>
    ///<param name="<dd>This parameter returns the number malloc attempts that might have"><dd>This parameter returns the number malloc attempts that might have</param>
    ///<param name="been satisfied using lookaside memory but failed due to all lookaside">been satisfied using lookaside memory but failed due to all lookaside</param>
    ///<param name="memory already being in use.">memory already being in use.</param>
    ///<param name="Only the high">water value is meaningful;</param>
    ///<param name="the current value is always zero.)^">the current value is always zero.)^</param>
    ///<param name=""></param>
    ///<param name="[[SQLITE_DBSTATUS_CACHE_USED]] ^(<dt>SQLITE_DBSTATUS_CACHE_USED</dt>">[[SQLITE_DBSTATUS_CACHE_USED]] ^(<dt>SQLITE_DBSTATUS_CACHE_USED</dt></param>
    ///<param name="<dd>This parameter returns the approximate number of of bytes of heap"><dd>This parameter returns the approximate number of of bytes of heap</param>
    ///<param name="memory used by all pager caches associated with the database connection.)^">memory used by all pager caches associated with the database connection.)^</param>
    ///<param name="^The highwater mark associated with SQLITE_DBSTATUS_CACHE_USED is always 0.">^The highwater mark associated with SQLITE_DBSTATUS_CACHE_USED is always 0.</param>
    ///<param name=""></param>
    ///<param name="[[SQLITE_DBSTATUS_SCHEMA_USED]] ^(<dt>SQLITE_DBSTATUS_SCHEMA_USED</dt>">[[SQLITE_DBSTATUS_SCHEMA_USED]] ^(<dt>SQLITE_DBSTATUS_SCHEMA_USED</dt></param>
    ///<param name="<dd>This parameter returns the approximate number of of bytes of heap"><dd>This parameter returns the approximate number of of bytes of heap</param>
    ///<param name="memory used to store the schema for all databases associated">memory used to store the schema for all databases associated</param>
    ///<param name="with the connection ">ed databases.)^ </param>
    ///<param name="^The full amount of memory used by the schemas is reported, even if the">^The full amount of memory used by the schemas is reported, even if the</param>
    ///<param name="schema memory is shared with other database connections due to">schema memory is shared with other database connections due to</param>
    ///<param name="[shared cache mode] being enabled.">[shared cache mode] being enabled.</param>
    ///<param name="^The highwater mark associated with SQLITE_DBSTATUS_SCHEMA_USED is always 0.">^The highwater mark associated with SQLITE_DBSTATUS_SCHEMA_USED is always 0.</param>
    ///<param name=""></param>
    ///<param name="[[SQLITE_DBSTATUS_STMT_USED]] ^(<dt>SQLITE_DBSTATUS_STMT_USED</dt>">[[SQLITE_DBSTATUS_STMT_USED]] ^(<dt>SQLITE_DBSTATUS_STMT_USED</dt></param>
    ///<param name="<dd>This parameter returns the approximate number of of bytes of heap"><dd>This parameter returns the approximate number of of bytes of heap</param>
    ///<param name="and lookaside memory used by all prepared statements associated with">and lookaside memory used by all prepared statements associated with</param>
    ///<param name="the database connection.)^">the database connection.)^</param>
    ///<param name="^The highwater mark associated with SQLITE_DBSTATUS_STMT_USED is always 0.">^The highwater mark associated with SQLITE_DBSTATUS_STMT_USED is always 0.</param>
    ///<param name="</dd>"></dd></param>
    ///<param name="</dl>"></dl></param>
    ///<param name=""></param>

    //#define SQLITE_DBSTATUS_LOOKASIDE_USED     0
    //#define SQLITE_DBSTATUS_CACHE_USED         1
    //#define SQLITE_DBSTATUS_SCHEMA_USED        2
    //#define SQLITE_DBSTATUS_STMT_USED          3
    //#define SQLITE_DBSTATUS_LOOKASIDE_HIT        4
    //#define SQLITE_DBSTATUS_LOOKASIDE_MISS_SIZE  5
    //#define SQLITE_DBSTATUS_LOOKASIDE_MISS_FULL  6
    //#define SQLITE_DBSTATUS_MAX                  6   /* Largest defined DBSTATUS */
    public enum SqliteDbStatus { 
        SQLITE_DBSTATUS_LOOKASIDE_USED = 0,

        SQLITE_DBSTATUS_CACHE_USED = 1,

        SQLITE_DBSTATUS_SCHEMA_USED = 2,

        SQLITE_DBSTATUS_STMT_USED = 3,

        SQLITE_DBSTATUS_LOOKASIDE_HIT = 4,

        SQLITE_DBSTATUS_LOOKASIDE_MISS_SIZE = 5,

        SQLITE_DBSTATUS_LOOKASIDE_MISS_FULL = 6,

        SQLITE_DBSTATUS_MAX = 6
    }
}
