using Community.CsharpSqlite.Os;
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
        public static class MemJournal {
            ///<summary>
            ///Forward references to internal structures 
            ///</summary>

            //typedef struct MemJournal MemJournal;
            //typedef struct FilePoint FilePoint;
            //typedef struct FileChunk FileChunk;
            ///<summary>
            ///Space to hold the rollback journal is allocated in increments of
            /// this many bytes.
            ///
            /// The size chosen is a little less than a power of two.  That way,
            /// the FileChunk object will have a size that almost exactly fills
            /// a power-of-two allocation.  This mimimizes wasted space in power-of-two
            /// memory allocators.
            ///
            ///</summary>
            //#define JOURNAL_CHUNKSIZE ((int)(1024-sizeof(FileChunk*)))
            public const int JOURNAL_CHUNKSIZE = 4096;
        }
        public static class BTree {
            ///<summary>
            /// The in-memory image of a disk page has the auxiliary information appended
            /// to the end.  EXTRA_SIZE is the number of bytes of space needed to hold
            /// that extra information.
            ///
            ///</summary>
            public const int EXTRA_SIZE = 0;

        }

        public static class Paging {
            ///
            ///<summary>
            ///Journal files begin with the following magic string.  The data
            ///was obtained from /dev/random.  It is used only as a sanity check.
            ///
            ///Since version 2.8.0, the journal format contains additional sanity
            ///checking information.  If the power fails while the journal is being
            ///</summary>
            ///<param name="written, semi">random garbage data might appear in the journal</param>
            ///<param name="file after power is restored.  If an attempt is then made">file after power is restored.  If an attempt is then made</param>
            ///<param name="to roll the journal back, the database could be corrupted.  The additional">to roll the journal back, the database could be corrupted.  The additional</param>
            ///<param name="sanity checking data is an attempt to discover the garbage in the">sanity checking data is an attempt to discover the garbage in the</param>
            ///<param name="journal and ignore it.">journal and ignore it.</param>
            ///<param name=""></param>
            ///<param name="The sanity checking information for the new journal format consists">The sanity checking information for the new journal format consists</param>
            ///<param name="of a 32">bit checksum on each page of data.  The checksum covers both</param>
            ///<param name="the page number and the pPager.pageSize bytes of data for the page.">the page number and the pPager.pageSize bytes of data for the page.</param>
            ///<param name="This cksum is initialized to a 32">bit random value that appears in the</param>
            ///<param name="journal file right after the header.  The random initializer is important,">journal file right after the header.  The random initializer is important,</param>
            ///<param name="because garbage data that appears at the end of a journal is likely">because garbage data that appears at the end of a journal is likely</param>
            ///<param name="data that was once in other files that have now been deleted.  If the">data that was once in other files that have now been deleted.  If the</param>
            ///<param name="garbage data came from an obsolete journal file, the checksums might">garbage data came from an obsolete journal file, the checksums might</param>
            ///<param name="be correct.  But by initializing the checksum to random value which">be correct.  But by initializing the checksum to random value which</param>
            ///<param name="is different for every journal, we minimize that risk.">is different for every journal, we minimize that risk.</param>

            public static byte[] aJournalMagic = new byte[] {
			0xd9,
			0xd5,
			0x05,
			0xf9,
			0x20,
			0xa1,
			0x63,
			0xd7,
		};


            ///
            ///<summary>
            ///</summary>
            ///<param name="The macro MEMDB is true if we are dealing with an in">memory database.</param>
            ///<param name="We do this as a macro so that if the SQLITE_OMIT_MEMORYDB macro is set,">We do this as a macro so that if the SQLITE_OMIT_MEMORYDB macro is set,</param>
            ///<param name="the value of MEMDB will be a constant and the compiler will optimize">the value of MEMDB will be a constant and the compiler will optimize</param>
            ///<param name="out code that would never execute.">out code that would never execute.</param>
            ///<param name=""></param>

#if !SQLITE_OMIT_MEMORYDB
            // define MEMDB 0
            public const int MEMDB = 0;
#else
        //# define MEMDB pPager.memDb
#endif



            ///
            ///<summary>
            ///The Pager.eLock variable is almost always set to one of the 
            ///</summary>
            ///<param name="following locking">states, according to the lock currently held on</param>
            ///<param name="the database file: NO_LOCK, SHARED_LOCK, RESERVED_LOCK or EXCLUSIVE_LOCK.">the database file: NO_LOCK, SHARED_LOCK, RESERVED_LOCK or EXCLUSIVE_LOCK.</param>
            ///<param name="This variable is kept up to date as locks are taken and released by">This variable is kept up to date as locks are taken and released by</param>
            ///<param name="the pagerLockDb() and pagerUnlockDb() wrappers.">the pagerLockDb() and pagerUnlockDb() wrappers.</param>
            ///<param name=""></param>
            ///<param name="If the VFS xLock() or xUnlock() returns an error other than SQLITE_BUSY">If the VFS xLock() or xUnlock() returns an error other than SQLITE_BUSY</param>
            ///<param name="(i.e. one of the SQLITE_IOERR subtypes), it is not clear whether or not">(i.e. one of the SQLITE_IOERR subtypes), it is not clear whether or not</param>
            ///<param name="the operation was successful. In these circumstances pagerLockDb() and">the operation was successful. In these circumstances pagerLockDb() and</param>
            ///<param name="pagerUnlockDb() take a conservative approach "> eLock is always updated</param>
            ///<param name="when unlocking the file, and only updated when locking the file if the">when unlocking the file, and only updated when locking the file if the</param>
            ///<param name="VFS call is successful. This way, the Pager.eLock variable may be set">VFS call is successful. This way, the Pager.eLock variable may be set</param>
            ///<param name="to a less exclusive (lower) value than the lock that is actually held">to a less exclusive (lower) value than the lock that is actually held</param>
            ///<param name="at the system level, but it is never set to a more exclusive value.">at the system level, but it is never set to a more exclusive value.</param>
            ///<param name=""></param>
            ///<param name="This is usually safe. If an xUnlock fails or appears to fail, there may ">This is usually safe. If an xUnlock fails or appears to fail, there may </param>
            ///<param name="be a few redundant xLock() calls or a lock may be held for longer than">be a few redundant xLock() calls or a lock may be held for longer than</param>
            ///<param name="required, but nothing really goes wrong.">required, but nothing really goes wrong.</param>
            ///<param name=""></param>
            ///<param name="The exception is when the database file is unlocked as the pager moves">The exception is when the database file is unlocked as the pager moves</param>
            ///<param name="from ERROR to OPEN state. At this point there may be a hot">journal file </param>
            ///<param name="in the file">>SHARED</param>
            ///<param name="transition, by the same pager or any other). If the call to xUnlock()">transition, by the same pager or any other). If the call to xUnlock()</param>
            ///<param name="fails at this point and the pager is left holding an EXCLUSIVE lock, this">fails at this point and the pager is left holding an EXCLUSIVE lock, this</param>
            ///<param name="can confuse the call to xCheckReservedLock() call made later as part">can confuse the call to xCheckReservedLock() call made later as part</param>
            ///<param name="of hot">journal detection.</param>
            ///<param name=""></param>
            ///<param name="xCheckReservedLock() is defined as returning true "if there is a RESERVED ">xCheckReservedLock() is defined as returning true "if there is a RESERVED </param>
            ///<param name="lock held by this process or any others". So xCheckReservedLock may ">lock held by this process or any others". So xCheckReservedLock may </param>
            ///<param name="return true because the caller itself is holding an EXCLUSIVE lock (but">return true because the caller itself is holding an EXCLUSIVE lock (but</param>
            ///<param name="doesn't know it because of a previous error in xUnlock). If this happens">doesn't know it because of a previous error in xUnlock). If this happens</param>
            ///<param name="a hot">journal may be mistaken for a journal being created by an active</param>
            ///<param name="transaction in another process, causing SQLite to read from the database">transaction in another process, causing SQLite to read from the database</param>
            ///<param name="without rolling it back.">without rolling it back.</param>
            ///<param name=""></param>
            ///<param name="To work around this, if a call to xUnlock() fails when unlocking the">To work around this, if a call to xUnlock() fails when unlocking the</param>
            ///<param name="database in the ERROR state, Pager.eLock is set to UNKNOWN_LOCK. It">database in the ERROR state, Pager.eLock is set to UNKNOWN_LOCK. It</param>
            ///<param name="is only changed back to a real locking state after a successful call">is only changed back to a real locking state after a successful call</param>
            ///<param name="to xLock(EXCLUSIVE). Also, the code to do the OPEN">>SHARED state transition</param>
            ///<param name="omits the check for a hot">journal if Pager.eLock is set to UNKNOWN_LOCK </param>
            ///<param name="lock. Instead, it assumes a hot">journal exists and obtains an EXCLUSIVE</param>
            ///<param name="lock on the database file before attempting to roll it back. See function">lock on the database file before attempting to roll it back. See function</param>
            ///<param name="PagerSharedLock() for more detail.">PagerSharedLock() for more detail.</param>
            ///<param name=""></param>
            ///<param name="Pager.eLock may only be set to UNKNOWN_LOCK when the pager is in ">Pager.eLock may only be set to UNKNOWN_LOCK when the pager is in </param>
            ///<param name="PagerState.PAGER_OPEN state.">PagerState.PAGER_OPEN state.</param>
            ///<param name=""></param>

            //#define UNKNOWN_LOCK                (EXCLUSIVE_LOCK+1)
            public const LockType UNKNOWN_LOCK = (LockType.EXCLUSIVE_LOCK + 1);

            ///<summary>
            ///Allowed values for the flags parameter to sqlite3PagerOpen().
            ///
            ///NOTE: These values must match the corresponding BTREE_ values in btree.h.
            ///
            ///</summary>

            //#define PAGER_OMIT_JOURNAL  0x0001    /* Do not use a rollback journal */
            //#define PAGER_NO_READLOCK   0x0002    /* Omit readlocks on readonly files */
            //#define PAGER_MEMORY        0x0004    /* In-memory database */
            public const int PAGER_OMIT_JOURNAL = 0x0001;

            public const int PAGER_NO_READLOCK = 0x0002;

            public const int PAGER_MEMORY = 0x0004;

            ///
            ///<summary>
            ///Valid values for the second argument to sqlite3PagerLockingMode().
            ///
            ///</summary>

            //#define PAGER_LOCKINGMODE_QUERY      -1
            //#define PAGER_LOCKINGMODE_NORMAL      0
            //#define PAGER_LOCKINGMODE_EXCLUSIVE   1
            public static int PAGER_LOCKINGMODE_QUERY = -1;

            public static int PAGER_LOCKINGMODE_NORMAL = 0;

            public static int PAGER_LOCKINGMODE_EXCLUSIVE = 1;

            
               
        }
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





        ///<summary>
        ///This is a magic string that appears at the beginning of every
        ///SQLite database in order to identify the file as a real database.
        ///
        ///</summary>
        ///<param name="You can change this value at compile">time by specifying a</param>
        ///<param name="">line.  The</param>
        ///<param name="header must be exactly 16 bytes including the zero">terminator so</param>
        ///<param name="the string itself should be 15 characters long.  If you change">the string itself should be 15 characters long.  If you change</param>
        ///<param name="the header, then your custom library will not be able to read">the header, then your custom library will not be able to read</param>
        ///<param name="databases generated by the standard tools and the standard tools">databases generated by the standard tools and the standard tools</param>
        ///<param name="will not be able to read databases created by your custom library.">will not be able to read databases created by your custom library.</param>
        ///<param name=""></param>

#if !SQLITE_FILE_HEADER
        public const string SQLITE_FILE_HEADER = "SQLite format 3\0";

#endif

        ///<summary>
        ///The header string that appears at the beginning of every
        ///SQLite database.
        ///</summary>
        public static byte[] zMagicHeader = Encoding.UTF8.GetBytes(Globals.SQLITE_FILE_HEADER);


    }
}
