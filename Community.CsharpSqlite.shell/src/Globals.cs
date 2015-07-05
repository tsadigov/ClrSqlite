using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bitmask = System.UInt64;

namespace Community.CsharpSqlite
{
    //SQLITE_API int sqlite3_limit(sqlite3*, int id, int newVal);
    ///
    ///<summary>
    ///</summary>
    ///<param name="CAPI3REF: Run">Time Limit Categories</param>
    ///<param name="KEYWORDS: {limit category} {*limit categories}">KEYWORDS: {limit category} {*limit categories}</param>
    ///<param name=""></param>
    ///<param name="These constants define various performance limits">These constants define various performance limits</param>
    ///<param name="that can be lowered at run">time using [sqlite3_limit()].</param>
    ///<param name="The synopsis of the meanings of the various limits is shown below.">The synopsis of the meanings of the various limits is shown below.</param>
    ///<param name="Additional information is available at [limits | Limits in SQLite].">Additional information is available at [limits | Limits in SQLite].</param>
    ///<param name=""></param>
    ///<param name="<dl>"><dl></param>
    ///<param name="[[SQLITE_LIMIT_LENGTH]] ^(<dt>SQLITE_LIMIT_LENGTH</dt>">[[SQLITE_LIMIT_LENGTH]] ^(<dt>SQLITE_LIMIT_LENGTH</dt></param>
    ///<param name="<dd>The maximum size of any string or BLOB or table row, in bytes.<dd>)^"><dd>The maximum size of any string or BLOB or table row, in bytes.<dd>)^</param>
    ///<param name=""></param>
    ///<param name="[[SQLITE_LIMIT_SQL_LENGTH]] ^(<dt>SQLITE_LIMIT_SQL_LENGTH</dt>">[[SQLITE_LIMIT_SQL_LENGTH]] ^(<dt>SQLITE_LIMIT_SQL_LENGTH</dt></param>
    ///<param name="<dd>The maximum length of an SQL statement, in bytes.</dd>)^"><dd>The maximum length of an SQL statement, in bytes.</dd>)^</param>
    ///<param name=""></param>
    ///<param name="[[SQLITE_LIMIT_COLUMN]] ^(<dt>SQLITE_LIMIT_COLUMN</dt>">[[SQLITE_LIMIT_COLUMN]] ^(<dt>SQLITE_LIMIT_COLUMN</dt></param>
    ///<param name="<dd>The maximum number of columns in a table definition or in the"><dd>The maximum number of columns in a table definition or in the</param>
    ///<param name="result set of a [SELECT] or the maximum number of columns in an index">result set of a [SELECT] or the maximum number of columns in an index</param>
    ///<param name="or in an ORDER BY or GROUP BY clause.</dd>)^">or in an ORDER BY or GROUP BY clause.</dd>)^</param>
    ///<param name=""></param>
    ///<param name="[[SQLITE_LIMIT_EXPR_DEPTH]] ^(<dt>SQLITE_LIMIT_EXPR_DEPTH</dt>">[[SQLITE_LIMIT_EXPR_DEPTH]] ^(<dt>SQLITE_LIMIT_EXPR_DEPTH</dt></param>
    ///<param name="<dd>The maximum depth of the parse tree on any expression.</dd>)^"><dd>The maximum depth of the parse tree on any expression.</dd>)^</param>
    ///<param name=""></param>
    ///<param name="[[SQLITE_LIMIT_COMPOUND_SELECT]] ^(<dt>SQLITE_LIMIT_COMPOUND_SELECT</dt>">[[SQLITE_LIMIT_COMPOUND_SELECT]] ^(<dt>SQLITE_LIMIT_COMPOUND_SELECT</dt></param>
    ///<param name="<dd>The maximum number of terms in a compound SELECT statement.</dd>)^"><dd>The maximum number of terms in a compound SELECT statement.</dd>)^</param>
    ///<param name=""></param>
    ///<param name="[[SQLITE_LIMIT_VDBE_OP]] ^(<dt>SQLITE_LIMIT_VDBE_OP</dt>">[[SQLITE_LIMIT_VDBE_OP]] ^(<dt>SQLITE_LIMIT_VDBE_OP</dt></param>
    ///<param name="<dd>The maximum number of instructions in a virtual machine program"><dd>The maximum number of instructions in a virtual machine program</param>
    ///<param name="used to implement an SQL statement.  This limit is not currently">used to implement an SQL statement.  This limit is not currently</param>
    ///<param name="enforced, though that might be added in some future release of">enforced, though that might be added in some future release of</param>
    ///<param name="SQLite.</dd>)^">SQLite.</dd>)^</param>
    ///<param name=""></param>
    ///<param name="[[SQLITE_LIMIT_FUNCTION_ARG]] ^(<dt>SQLITE_LIMIT_FUNCTION_ARG</dt>">[[SQLITE_LIMIT_FUNCTION_ARG]] ^(<dt>SQLITE_LIMIT_FUNCTION_ARG</dt></param>
    ///<param name="<dd>The maximum number of arguments on a function.</dd>)^"><dd>The maximum number of arguments on a function.</dd>)^</param>
    ///<param name=""></param>
    ///<param name="[[SQLITE_LIMIT_ATTACHED]] ^(<dt>SQLITE_LIMIT_ATTACHED</dt>">[[SQLITE_LIMIT_ATTACHED]] ^(<dt>SQLITE_LIMIT_ATTACHED</dt></param>
    ///<param name="<dd>The maximum number of [ATTACH | attached databases].)^</dd>"><dd>The maximum number of [ATTACH | attached databases].)^</dd></param>
    ///<param name=""></param>
    ///<param name="[[SQLITE_LIMIT_LIKE_PATTERN_LENGTH]]">[[SQLITE_LIMIT_LIKE_PATTERN_LENGTH]]</param>
    ///<param name="^(<dt>SQLITE_LIMIT_LIKE_PATTERN_LENGTH</dt>">^(<dt>SQLITE_LIMIT_LIKE_PATTERN_LENGTH</dt></param>
    ///<param name="<dd>The maximum length of the pattern argument to the [LIKE] or"><dd>The maximum length of the pattern argument to the [LIKE] or</param>
    ///<param name="[GLOB] operators.</dd>)^">[GLOB] operators.</dd>)^</param>
    ///<param name=""></param>
    ///<param name="[[SQLITE_LIMIT_VARIABLE_NUMBER]]">[[SQLITE_LIMIT_VARIABLE_NUMBER]]</param>
    ///<param name="^(<dt>SQLITE_LIMIT_VARIABLE_NUMBER</dt>">^(<dt>SQLITE_LIMIT_VARIABLE_NUMBER</dt></param>
    ///<param name="<dd>The maximum index number of any [parameter] in an SQL statement.)^"><dd>The maximum index number of any [parameter] in an SQL statement.)^</param>
    ///<param name=""></param>
    ///<param name="[[SQLITE_LIMIT_TRIGGER_DEPTH]] ^(<dt>SQLITE_LIMIT_TRIGGER_DEPTH</dt>">[[SQLITE_LIMIT_TRIGGER_DEPTH]] ^(<dt>SQLITE_LIMIT_TRIGGER_DEPTH</dt></param>
    ///<param name="<dd>The maximum depth of recursion for triggers.</dd>)^"><dd>The maximum depth of recursion for triggers.</dd>)^</param>
    ///<param name="</dl>"></dl></param>
    ///<param name=""></param>

    //#define SQLITE_LIMIT_LENGTH                    0
    //#define SQLITE_LIMIT_SQL_LENGTH                1
    //#define SQLITE_LIMIT_COLUMN                    2
    //#define SQLITE_LIMIT_EXPR_DEPTH                3
    //#define SQLITE_LIMIT_COMPOUND_SELECT           4
    //#define SQLITE_LIMIT_VDBE_OP                   5
    //#define SQLITE_LIMIT_FUNCTION_ARG              6
    //#define SQLITE_LIMIT_ATTACHED                  7
    //#define SQLITE_LIMIT_LIKE_PATTERN_LENGTH       8
    //#define SQLITE_LIMIT_VARIABLE_NUMBER           9
    //#define SQLITE_LIMIT_TRIGGER_DEPTH            10

    public class Globals
    {
        public const int SQLITE_LIMIT_LENGTH = 0;

        public const int SQLITE_LIMIT_SQL_LENGTH = 1;

        public const int SQLITE_LIMIT_COLUMN = 2;

        public const int SQLITE_LIMIT_EXPR_DEPTH = 3;

        public const int SQLITE_LIMIT_COMPOUND_SELECT = 4;

        public const int SQLITE_LIMIT_VDBE_OP = 5;

        public const int SQLITE_LIMIT_FUNCTION_ARG = 6;

        public const int SQLITE_LIMIT_ATTACHED = 7;

        public const int SQLITE_LIMIT_LIKE_PATTERN_LENGTH = 8;

        public const int SQLITE_LIMIT_VARIABLE_NUMBER = 9;

        public const int SQLITE_LIMIT_TRIGGER_DEPTH = 10;

        ///
        ///<summary>
        ///</summary>
        ///<param name="The maximum number of in">memory pages to use for the main database</param>
        ///<param name="table and for temporary tables.  The SQLITE_DEFAULT_CACHE_SIZE">table and for temporary tables.  The SQLITE_DEFAULT_CACHE_SIZE</param>

#if !SQLITE_DEFAULT_CACHE_SIZE
        public const int SQLITE_DEFAULT_CACHE_SIZE = 2000;

#endif












        ///<summary>
        ///The number of samples of an index that SQLite takes in order to 
        ///construct a histogram of the table content when running ANALYZE
        ///and with SQLITE_ENABLE_STAT2
        ///</summary>
        //#define SQLITE_INDEX_SAMPLES 10
        public const int SQLITE_INDEX_SAMPLES = 10;


        ///<summary>
        ///The SQLITE_THREADSAFE macro must be defined as 0, 1, or 2.
        ///0 means mutexes are permanently disable and the library is never
        ///threadsafe.  1 means the library is serialized which is the highest
        ///</summary>
        ///<param name="level of threadsafety.  2 means the libary is multithreaded "> multiple</param>
        ///<param name="threads can use SQLite as long as no two threads try to use the same">threads can use SQLite as long as no two threads try to use the same</param>
        ///<param name="database connection at the same time.">database connection at the same time.</param>
        ///<param name=""></param>
        ///<param name="Older versions of SQLite used an optional THREADSAFE macro.">Older versions of SQLite used an optional THREADSAFE macro.</param>
        ///<param name="We support that for legacy.">We support that for legacy.</param>
        ///<param name=""></param>
#if !SQLITE_THREADSAFE
        //# define SQLITE_THREADSAFE 2
        public const int SQLITE_THREADSAFE = 2;
#else
																																																												    const int SQLITE_THREADSAFE = 2; /* IMP: R-07272-22309 */
#endif
        ///
        ///<summary>
        ///The SQLITE_DEFAULT_MEMSTATUS macro must be defined as either 0 or 1.
        ///It determines whether or not the features related to
        ///SQLITE_CONFIG_MEMSTATUS are available by default or not. This value can
        ///be overridden at runtime using the sqlite3_config() API.
        ///</summary>
#if !(SQLITE_DEFAULT_MEMSTATUS)
        //# define SQLITE_DEFAULT_MEMSTATUS 1
        public const int SQLITE_DEFAULT_MEMSTATUS = 0;
#else
																																																												const int SQLITE_DEFAULT_MEMSTATUS = 1;
#endif
        ///
        ///<summary>
        ///Exactly one of the following macros must be defined in order to
        ///specify which memory allocation subsystem to use.
        ///
        ///SQLITE_SYSTEM_MALLOC          // Use normal system malloc()
        ///SQLITE_MEMDEBUG               // Debugging version of system malloc()
        ///
        ///(Historical note:  There used to be several other options, but we've
        ///pared it down to just these two.)
        ///
        ///If none of the above are defined, then set SQLITE_SYSTEM_MALLOC as
        ///the default.
        ///</summary>
        //#if (SQLITE_SYSTEM_MALLOC)+defined(SQLITE_MEMDEBUG)+\
        //# error "At most one of the following compile-time configuration options\
        // is allows: SQLITE_SYSTEM_MALLOC, SQLITE_MEMDEBUG"
        //#endif
        //#if (SQLITE_SYSTEM_MALLOC)+defined(SQLITE_MEMDEBUG)+\
        //# define SQLITE_SYSTEM_MALLOC 1
        //#endif
        ///
        ///<summary>
        ///If SQLITE_MALLOC_SOFT_LIMIT is not zero, then try to keep the
        ///sizes of memory allocations below this value where possible.
        ///
        ///</summary>
#if !(SQLITE_MALLOC_SOFT_LIMIT)
        public const int SQLITE_MALLOC_SOFT_LIMIT = 1024;
#endif

























        ///
        ///<summary>
        ///The bitmask datatype defined below is used for various optimizations.
        ///
        ///</summary>
        ///<param name="Changing this from a 64">bit type limits the number of</param>
        ///<param name="tables in a join to 32 instead of 64.  But it also reduces the size">tables in a join to 32 instead of 64.  But it also reduces the size</param>
        ///<param name="of the library by 738 bytes on ix86.">of the library by 738 bytes on ix86.</param>
        ///<param name=""></param>
        //typedef u64 Bitmask;
        ///<summary>
        /// The number of bits in a Bitmask.  "BMS" means "BitMask Size".
        ///
        ///</summary>
        //#define BMS  ((int)(sizeof(Bitmask)*8))
        public const int BMS = ((int)(sizeof(Bitmask) * 8));


    }
}
