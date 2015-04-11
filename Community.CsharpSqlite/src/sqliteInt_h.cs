#define SQLITE_MAX_EXPR_DEPTH
using System;
using System.Diagnostics;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Text;
using Bitmask=System.UInt64;
using i16=System.Int16;
using i64=System.Int64;
using sqlite3_int64=System.Int64;
using u8=System.Byte;
using u16=System.UInt16;
using u32=System.UInt32;
using u64=System.UInt64;
using unsigned=System.UInt64;
using Pgno=System.UInt32;
#if !SQLITE_MAX_VARIABLE_NUMBER
using ynVar=System.Int16;
#else
using ynVar = System.Int32; 
#endif
///
///<summary>
///The yDbMask datatype for the bitmask of all attached databases.
///</summary>
#if SQLITE_MAX_ATTACHED
//  typedef sqlite3_uint64 yDbMask;
using yDbMask = System.Int64; 
#else
//  typedef unsigned int yDbMask;
using yDbMask=System.Int32;
#endif
namespace Community.CsharpSqlite
{
    using sqlite3_value = Mem;
    using Parse = Sqlite3.Parse;
    using Vdbe = Sqlite3.Vdbe;
    ///<summary>
    /// SQLite supports many different ways to resolve a constraint
    /// error.  ROLLBACK processing means that a constraint violation
    /// causes the operation in process to fail and for the current transaction
    /// to be rolled back.  ABORT processing means the operation in process
    /// fails and any prior changes from that one operation are backed out,
    /// but the transaction is not rolled back.  FAIL processing means that
    /// the operation in progress stops and returns an error code.  But prior
    /// changes due to the same operation are not backed out and no rollback
    /// occurs.  IGNORE means that the particular row that caused the constraint
    /// error is not inserted or updated.  Processing continues and no error
    /// is returned.  REPLACE means that preexisting database rows that caused
    /// a UNIQUE constraint violation are removed so that the new insert or
    /// update can proceed.  Processing continues and no error is reported.
    ///
    /// RESTRICT, SETNULL, and CASCADE actions apply only to foreign keys.
    /// RESTRICT is the same as ABORT for IMMEDIATE foreign keys and the
    /// same as ROLLBACK for DEFERRED keys.  SETNULL means that the foreign
    /// key is set to NULL.  CASCADE means that a DELETE or UPDATE of the
    /// referenced table row is propagated into the row that holds the
    /// foreign key.
    ///
    /// The following symbolic values are used to record which type
    /// of action to take.
    ///
    ///</summary>
    public enum OnConstraintError : byte
    {
        OE_None = 0, //#define OnConstraintError.OE_None     0   /* There is no constraint to check */

        OE_Rollback = 1,
        //#define OnConstraintError.OE_Rollback 1   /* Fail the operation and rollback the transaction */

        OE_Abort = 2, //#define OnConstraintError.OE_Abort    2   /* Back out changes but do no rollback transaction */
        OE_Fail = 3, //#define OnConstraintError.OE_Fail     3   /* Stop the operation but leave all prior changes */

        OE_Ignore = 4,
        //#define OnConstraintError.OE_Ignore   4   /* Ignore the error. Do not do the INSERT or UPDATE */

        OE_Replace = 5,
        //#define OnConstraintError.OE_Replace  5   /* Delete existing record, then do INSERT or UPDATE */

        OE_Restrict = 6,
        //#define OnConstraintError.OE_Restrict 6   /* OnConstraintError.OE_Abort for IMMEDIATE, OnConstraintError.OE_Rollback for DEFERRED */

        OE_SetNull = 7, //#define OnConstraintError.OE_SetNull  7   /* Set the foreign key value to NULL */
        OE_SetDflt = 8, //#define OnConstraintError.OE_SetDflt  8   /* Set the foreign key value to its default */
        OE_Cascade = 9, //#define OnConstraintError.OE_Cascade  9   /* Cascade the changes */

        OE_Default = 99 //#define OnConstraintError.OE_Default  99  /* Do whatever the default action is */
    }



    ///
    ///<summary>
    ///Possible values for the sqlite3.flags.
    ///
    ///</summary>
    public enum SqliteFlags:uint
    {

        //#define SQLITE_VdbeTrace      0x00000100  /* True to trace VDBE execution */
        //#define SQLITE_InternChanges  0x00000200  /* Uncommitted Hash table changes */
        //#define SQLITE_FullColNames   0x00000400  /* Show full column names on SELECT */
        //#define SQLITE_ShortColNames  0x00000800  /* Show short columns names */
        //#define SQLITE_CountRows      0x00001000  /* Count rows changed by INSERT, */
        //                                          /*   DELETE, or UPDATE and return */
        //                                          /*   the count using a callback. */
        //#define SQLITE_NullCallback   0x00002000  /* Invoke the callback once if the */
        //                                          /*   result set is empty */
        //#define SQLITE_SqlTrace       0x00004000  /* Debug print SQL as it executes */
        //#define SQLITE_VdbeListing    0x00008000  /* Debug listings of VDBE programs */
        //#define SQLITE_WriteSchema    0x00010000  /* OK to update SQLITE_MASTER */
        //#define SQLITE_NoReadlock     0x00020000  /* Readlocks are omitted when 
        //                                          ** accessing read-only databases */
        //#define SQLITE_IgnoreChecks   0x00040000  /* Do not enforce check constraints */
        //#define SQLITE_ReadUncommitted 0x0080000  /* For shared-cache mode */
        //#define SQLITE_LegacyFileFmt  0x00100000  /* Create new databases in format 1 */
        //#define SQLITE_FullFSync      0x00200000  /* Use full fsync on the backend */
        //#define SQLITE_CkptFullFSync  0x00400000  /* Use full fsync for checkpoint */
        //#define SQLITE_RecoveryMode   0x00800000  /* Ignore schema errors */
        //#define SQLITE_ReverseOrder   0x01000000  /* Reverse unordered SELECTs */
        //#define SQLITE_RecTriggers    0x02000000  /* Enable recursive triggers */
        //#define SQLITE_ForeignKeys    0x04000000  /* Enforce foreign key constraints  */
        //#define SQLITE_AutoIndex      0x08000000  /* Enable automatic indexes */
        //#define SQLITE_PreferBuiltin  0x10000000  /* Preference to built-in funcs */
        //#define SQLITE_LoadExtension  0x20000000  /* Enable load_extension */
        //define SQLITE_EnableTrigger  0x40000000  /* True to enable triggers */
        SQLITE_VdbeTrace = 0x00000100,
        SQLITE_InternChanges = 0x00000200,
        SQLITE_FullColNames = 0x00000400,
        SQLITE_ShortColNames = 0x00000800,
        SQLITE_CountRows = 0x00001000,
        SQLITE_NullCallback = 0x00002000,
        SQLITE_SqlTrace = 0x00004000,
        SQLITE_VdbeListing = 0x00008000,
        SQLITE_WriteSchema = 0x00010000,
        SQLITE_NoReadlock = 0x00020000,
        SQLITE_IgnoreChecks = 0x00040000,
        SQLITE_ReadUncommitted = 0x0080000,
        SQLITE_LegacyFileFmt = 0x00100000,
        SQLITE_FullFSync = 0x00200000,
        SQLITE_CkptFullFSync = 0x00400000,
        SQLITE_RecoveryMode = 0x00800000,
        SQLITE_ReverseOrder = 0x01000000,
        SQLITE_RecTriggers = 0x02000000,
        SQLITE_ForeignKeys = 0x04000000,
        SQLITE_AutoIndex = 0x08000000,
        SQLITE_PreferBuiltin = 0x10000000,
        SQLITE_LoadExtension = 0x20000000,
        SQLITE_EnableTrigger = 0x40000000,
        ///
        ///<summary>
        ///Bits of the sqlite3.flags field that are used by the
        ///sqlite3_test_control(SQLITE_TESTCTRL_OPTIMIZATIONS,...) interface.
        ///</summary>
        ///<param name="These must be the low">order bits of the flags field.</param>
        ///<param name=""></param>
        //#define SQLITE_QueryFlattener 0x01        /* Disable query flattening */
        //#define SQLITE_ColumnCache    0x02        /* Disable the column cache */
        //#define SQLITE_IndexSort      0x04        /* Disable indexes for sorting */
        //#define SQLITE_IndexSearch    0x08        /* Disable indexes for searching */
        //#define SQLITE_IndexCover     0x10        /* Disable index covering table */
        //#define SQLITE_GroupByOrder   0x20        /* Disable GROUPBY cover of ORDERBY */
        //#define SQLITE_FactorOutConst 0x40        /* Disable factoring out constants */
        //#define SQLITE_IdxRealAsInt   0x80        /* Store REAL as INT in indices */
        //#define SQLITE_OptMask        0xff        /* Mask of all disablable opts */
        SQLITE_QueryFlattener = 0x01,
        SQLITE_ColumnCache = 0x02,
        SQLITE_IndexSort = 0x04,
        SQLITE_IndexSearch = 0x08,
        SQLITE_IndexCover = 0x10,
        SQLITE_GroupByOrder = 0x20,
        SQLITE_FactorOutConst = 0x40,
        SQLITE_IdxRealAsInt = 0x80,
        SQLITE_OptMask = 0xff,
        




    }

    public partial class Sqlite3{

    ///<summary>
        /// Possible values for the sqlite.magic field.
        /// The numbers are obtained at random and have no special meaning, other
        /// than being distinct from one another.
        ///
        ///</summary>
       public const int SQLITE_MAGIC_OPEN = 0x1029a697;
        //#define SQLITE_MAGIC_OPEN     0xa029a697  /* Database is open */

        public const int SQLITE_MAGIC_CLOSED = 0x2f3c2d33;
        //#define SQLITE_MAGIC_CLOSED   0x9f3c2d33  /* Database is closed */

        public const int SQLITE_MAGIC_SICK = 0x3b771290;
            //#define SQLITE_MAGIC_SICK     0x4b771290  /* Error and awaiting close */

        public const int SQLITE_MAGIC_BUSY = 0x403b7906;
        //#define SQLITE_MAGIC_BUSY     0xf03b7906  /* Database currently in use */

        public const int SQLITE_MAGIC_ERROR = 0x55357930;
            //#define SQLITE_MAGIC_ERROR    0xb5357930  /* An SQLITE_MISUSE error occurred */

    }

    ///
    ///<summary>
    ///A sort order can be either ASC or DESC.
    ///
    ///</summary>
    public enum SortOrder:byte
    {
        SQLITE_SO_ASC = 0,
        SQLITE_SO_DESC = 1
    }

    ///<summary>
    /// Bitfield flags for P5 value in OP_Insert and OP_Delete
    ///
    ///</summary>
    //#define OPFLAG_NCHANGE       0x01    /* Set to update db->nChange */
    //#define OPFLAG_LASTROWID     0x02    /* Set to update db->lastRowid */
    //#define OPFLAG_ISUPDATE      0x04    /* This OP_Insert is an sql UPDATE */
    //#define OPFLAG_APPEND        0x08    /* This is likely to be an append */
    //#define OPFLAG_USESEEKRESULT 0x10    /* Try to avoid a seek in BtreeInsert() */
    //#define OPFLAG_CLEARCACHE    0x20    /* Clear pseudo-table cache in OP_Column */

    public enum OpFlag : byte { OPFLAG_NCHANGE = 0x01, OPFLAG_LASTROWID = 0x02, OPFLAG_ISUPDATE = 0x04, OPFLAG_APPEND = 0x08, OPFLAG_USESEEKRESULT = 0x10, OPFLAG_CLEARCACHE = 0x20 }


    public partial class Sqlite3
    {



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
        /// A convenience macro that returns the number of elements in
        /// an array.
        ///
        ///</summary>
        //#define Sqlite3.ArraySize(X)    ((int)(sizeof(X)/sizeof(X[0])))
        public static int ArraySize<T>(T[] x)
        {
            return x.Length;
        }

        ///<summary>
        /// The ALWAYS and NEVER macros surround boolean expressions which
        /// are intended to always be true or false, respectively.  Such
        /// expressions could be omitted from the code completely.  But they
        /// are included in a few cases in order to enhance the resilience
        /// of SQLite to unexpected behavior - to make the code "self-healing"
        /// or "ductile" rather than being "brittle" and crashing at the first
        /// hint of unplanned behavior.
        ///
        /// In other words, ALWAYS and NEVER are added for defensive code.
        ///
        /// When doing coverage testing ALWAYS and NEVER are hard-coded to
        /// be true and false so that the unreachable code then specify will
        /// not be counted as untested code.
        ///</summary>
#if SQLITE_COVERAGE_TEST
																																																												// define Sqlite3.ALWAYS(X)      (1)
// define NEVER(X)       (0)
#elif !NDEBUG
																																																												    // define Sqlite3.ALWAYS(X)      ((X)?1:(Debug.Assert(0),0))
    static bool Sqlite3.ALWAYS( bool X )
    {
      if ( X != true )
        Debug.Assert( false );
      return true;
    }
    static int Sqlite3.ALWAYS( int X )
    {
      if ( X == 0 )
        Debug.Assert( false );
      return 1;
    }
    static bool ALWAYS<T>( T X )
    {
      if ( X == null )
        Debug.Assert( false );
      return true;
    }

    // define NEVER(X)       ((X)?(Debug.Assert(0),1):0)
    static bool NEVER( bool X )
    {
      if ( X == true )
        Debug.Assert( false );
      return false;
    }
    static byte NEVER( byte X )
    {
      if ( X != 0 )
        Debug.Assert( false );
      return 0;
    }
    static int NEVER( int X )
    {
      if ( X != 0 )
        Debug.Assert( false );
      return 0;
    }
    static bool NEVER<T>( T X )
    {
      if ( X != null )
        Debug.Assert( false );
      return false;
    }
#else
        //# define Sqlite3.ALWAYS(X)      (X)
        public static bool ALWAYS(bool X)
        {
            return X;
        }
        public static byte ALWAYS(byte X)
        {
            return X;
        }
        public static int ALWAYS(int X)
        {
            return X;
        }
        public static bool ALWAYS<T>(T X)
        {
            return true;
        }
        //# define NEVER(X)       (X)
        public static bool NEVER(bool X)
        {
            return X;
        }
        public static byte NEVER(byte X)
        {
            return X;
        }
        public static int NEVER(int X)
        {
            return X;
        }
        public static bool NEVER<T>(T X)
        {
            return false;
        }
#endif



        

    }




    public class sqliteinth
    {
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
        ///Internal interface definitions for SQLite.
        ///
        ///
        ///</summary>
        ///<param name="Included in SQLite3 port to C#">SQLite;  2008 Noah B Hart</param>
        ///<param name="C#">SQLite is an independent reimplementation of the SQLite software library</param>
        ///<param name=""></param>
        ///<param name="SQLITE_SOURCE_ID: 2011">23 19:49:22 4374b7e83ea0a3fbc3691f9c0c936272862f32f2</param>
        ///<param name=""></param>
        ///<param name=""></param>
        ///<param name=""></param>
        //#if !_SQLITEINT_H_
        //#define _SQLITEINT_H_
        ///
        ///<summary>
        ///These #defines should enable >2GB file support on POSIX if the
        ///underlying operating system supports it.  If the OS lacks
        ///</summary>
        ///<param name="large file support, or if the OS is windows, these should be no">ops.</param>
        ///<param name=""></param>
        ///<param name="Ticket #2739:  The _LARGEFILE_SOURCE macro must appear before any">Ticket #2739:  The _LARGEFILE_SOURCE macro must appear before any</param>
        ///<param name="system #includes.  Hence, this block of code must be the very first">system #includes.  Hence, this block of code must be the very first</param>
        ///<param name="code in all source files.">code in all source files.</param>
        ///<param name=""></param>
        ///<param name="Large file support can be disabled using the ">DSQLITE_DISABLE_LFS switch</param>
        ///<param name="on the compiler command line.  This is necessary if you are compiling">on the compiler command line.  This is necessary if you are compiling</param>
        ///<param name="on a recent machine (ex: Red Hat 7.2) but you want your code to work">on a recent machine (ex: Red Hat 7.2) but you want your code to work</param>
        ///<param name="on an older machine (ex: Red Hat 6.0).  If you compile on Red Hat 7.2">on an older machine (ex: Red Hat 6.0).  If you compile on Red Hat 7.2</param>
        ///<param name="without this option, LFS is enable.  But LFS does not exist in the kernel">without this option, LFS is enable.  But LFS does not exist in the kernel</param>
        ///<param name="in Red Hat 6.0, so the code won't work.  Hence, for maximum binary">in Red Hat 6.0, so the code won't work.  Hence, for maximum binary</param>
        ///<param name="portability you should omit LFS.">portability you should omit LFS.</param>
        ///<param name=""></param>
        ///<param name="Similar is true for Mac OS X.  LFS is only supported on Mac OS X 9 and later.">Similar is true for Mac OS X.  LFS is only supported on Mac OS X 9 and later.</param>
        ///<param name=""></param>
        //#if !SQLITE_DISABLE_LFS
        //# define _LARGE_FILE       1
        //# ifndef _FILE_OFFSET_BITS
        //#   define _FILE_OFFSET_BITS 64
        //# endif
        //# define _LARGEFILE_SOURCE 1
        //#endif
        ///
        ///<summary>
        ///Include the configuration header output by 'configure' if we're using the
        ///</summary>
        ///<param name="autoconf">based build</param>
        ///<param name=""></param>
#if _HAVE_SQLITE_CONFIG_H
																																																												//include "config.h"
#endif
        //#include "sqliteLimit.h"
        ///
        ///<summary>
        ///Disable nuisance warnings on Borland compilers 
        ///</summary>
        //#if (__BORLANDC__)
        //#pragma warn -rch /* unreachable code */
        //#pragma warn -ccc /* Condition is always true or false */
        //#pragma warn -aus /* Assigned value is never used */
        //#pragma warn -csu /* Comparing signed and unsigned */
        //#pragma warn -spa /* Suspicious pointer arithmetic */
        //#endif
        ///
        ///<summary>
        ///Needed for various definitions... 
        ///</summary>
        //#if !_GNU_SOURCE
        //#define _GNU_SOURCE
        //#endif
        ///
        ///<summary>
        ///Include standard header files as necessary
        ///
        ///</summary>
#if HAVE_STDINT_H
																																																												//include <stdint.h>
#endif
#if HAVE_INTTYPES_H
																																																												//include <inttypes.h>
#endif
        ///

        ///
        ///<summary>
        ///The following macros are used to cast pointers to integers and
        ///integers to pointers.  The way you do this varies from one compiler
        ///to the next, so we have developed the following set of #if statements
        ///to generate appropriate macros for a wide range of compilers.
        ///
        ///The correct "ANSI" way to do this is to use the intptr_t type. 
        ///Unfortunately, that typedef is not available on all compilers, or
        ///if it is available, it requires an #include of specific headers
        ///that vary from one machine to the next.
        ///
        ///</summary>
        ///<param name="Ticket #3860:  The llvm">4.2 compiler from Apple chokes on</param>
        ///<param name="the ((void)&((char)0)[X]) construct.  But MSVC chokes on ((void)(X)).">the ((void)&((char)0)[X]) construct.  But MSVC chokes on ((void)(X)).</param>
        ///<param name="So we have to define the macros in different ways depending on the">So we have to define the macros in different ways depending on the</param>
        ///<param name="compiler.">compiler.</param>
        ///<param name=""></param>
        //#if (__PTRDIFF_TYPE__)  /* This case should work for GCC */
        //# define SQLITE_INT_TO_PTR(X)  ((void)(__PTRDIFF_TYPE__)(X))
        //# define SQLITE_PTR_TO_INT(X)  ((int)(__PTRDIFF_TYPE__)(X))
        //#elif !defined(__GNUC__)       /* Works for compilers other than LLVM */
        //# define SQLITE_INT_TO_PTR(X)  ((void)&((char)0)[X])
        //# define SQLITE_PTR_TO_INT(X)  ((int)(((char)X)-(char)0))
        //#elif defined(HAVE_STDINT_H)   /* Use this case if we have ANSI headers */
        //# define SQLITE_INT_TO_PTR(X)  ((void)(intptr_t)(X))
        //# define SQLITE_PTR_TO_INT(X)  ((int)(intptr_t)(X))
        //#else                          /* Generates a warning - but it always works */
        //# define SQLITE_INT_TO_PTR(X)  ((void)(X))
        //# define SQLITE_PTR_TO_INT(X)  ((int)(X))
        //#endif
        ///

        ///
        ///<summary>
        ///We need to define _XOPEN_SOURCE as follows in order to enable
        ///recursive mutexes on most Unix systems.  But Mac OS X is different.
        ///The _XOPEN_SOURCE define causes problems for Mac OS X we are told,
        ///so it is omitted there.  See ticket #2673.
        ///
        ///Later we learn that _XOPEN_SOURCE is poorly or incorrectly
        ///implemented on some systems.  So we avoid defining it at all
        ///if it is already defined or if it is unneeded because we are
        ///not doing a threadsafe build.  Ticket #2681.
        ///
        ///See also ticket #2741.
        ///</summary>
#if !_XOPEN_SOURCE && !__DARWIN__ && !__APPLE__ && SQLITE_THREADSAFE
																																																												    const int _XOPEN_SOURCE = 500;//define _XOPEN_SOURCE 500  /* Needed to enable pthread recursive mutexes */
#endif
        ///
        ///<summary>
        ///The TCL headers are only needed when compiling the TCL bindings.
        ///</summary>
#if SQLITE_TCL || TCLSH
																																																												    // include <tcl.h>
#endif
        ///
        ///<summary>
        ///</summary>
        ///<param name="Many people are failing to set ">DNDEBUG=1 when compiling SQLite.</param>
        ///<param name="Setting NDEBUG makes the code smaller and run faster.  So the following">Setting NDEBUG makes the code smaller and run faster.  So the following</param>
        ///<param name="lines are added to automatically set NDEBUG unless the ">DSQLITE_DEBUG=1</param>
        ///<param name="option is set.  Thus NDEBUG becomes an opt">out</param>
        ///<param name="feature.">feature.</param>
#if !NDEBUG && !SQLITE_DEBUG
																																																												const int NDEBUG = 1;// define NDEBUG 1
#endif
        ///<summary>
        /// The sqliteinth.testcase() macro is used to aid in coverage testing.  When
        /// doing coverage testing, the condition inside the argument to
        /// sqliteinth.testcase() must be evaluated both true and false in order to
        /// get full branch coverage.  The sqliteinth.testcase() macro is inserted
        /// to help ensure adequate test coverage in places where simple
        /// condition/decision coverage is inadequate.  For example, sqliteinth.testcase()
        /// can be used to make sure boundary values are tested.  For
        /// bitmask tests, sqliteinth.testcase() can be used to make sure each bit
        /// is significant and used at least once.  On switch statements
        /// where multiple cases go to the same block of code, sqliteinth.testcase()
        /// can insure that all cases are evaluated.
        ///
        ///</summary>
#if SQLITE_COVERAGE_TEST
																																																												void sqlite3Coverage(int);
// define sqliteinth.testcase(X)  if( X ){ sqlite3Coverage(__LINE__); }
#else
        //# define sqliteinth.testcase(X)
        public static void testcase<T>(T X)
        {
        }
#endif
        ///
        ///<summary>
        ///The TESTONLY macro is used to enclose variable declarations or
        ///other bits of code that are needed to support the arguments
        ///within sqliteinth.testcase() and Debug.Assert() macros.
        ///</summary>
#if !NDEBUG || SQLITE_COVERAGE_TEST
																																																												    // define TESTONLY(X)  X
    // -- Need workaround for C, since inline macros don't exist
#else
        //# define TESTONLY(X)
#endif
        ///
        ///<summary>
        ///Sometimes we need a small amount of code such as a variable initialization
        ///to setup for a later Debug.Assert() statement.  We do not want this code to
        ///appear when Debug.Assert() is disabled.  The following macro is therefore
        ///used to contain that setup code.  The "VVA" acronym stands for
        ///"Verification, Validation, and Accreditation".  In other words, the
        ///code within VVA_ONLY() will only run during verification processes.
        ///</summary>
#if !NDEBUG
																																																												    // define VVA_ONLY(X)  X
#else
        //# define VVA_ONLY(X)
#endif

        ///
        ///<summary>
        ///</summary>
        ///<param name="Return true (non">zero) if the input is a integer that is too large</param>
        ///<param name="to fit in 32">bits.  This macro is used inside of various sqliteinth.testcase()</param>
        ///<param name="macros to verify that we have tested SQLite for large">file support.</param>
        private static bool IS_BIG_INT(i64 X)
        {
            return (((X) & ~(i64)0xffffffff) != 0);
        }
        //#define IS_BIG_INT(X)  (((X)&~(i64)0xffffffff)!=0)
        ///<summary>
        /// The macro unlikely() is a hint that surrounds a boolean
        /// expression that is usually false.  Macro likely() surrounds
        /// a boolean expression that is usually true.  GCC is able to
        /// use these hints to generate better code, sometimes.
        ///
        ///</summary>
#if (__GNUC__) && FALSE
																																																												// define likely(X)    __builtin_expect((X),1)
// define unlikely(X)  __builtin_expect((X),0)
#else
        //# define likely(X)    !!(X)
        public static bool likely(bool X)
        {
            return !!X;
        }
        //# define unlikely(X)  !!(X)
        public static bool unlikely(bool X)
        {
            return !!X;
        }
#endif
        //#include "sqlite3.h"
        //#include "hash.h"
        //#include "parse.h"
        //#include <stdio.h>
        //#include <stdlib.h>
        //#include <string.h>
        //#include <assert.h>
        //#include <stddef.h>
        ///
        ///<summary>
        ///If compiling for a processor that lacks floating point support,
        ///</summary>
        ///<param name="substitute integer for floating">point</param>
        ///<param name=""></param>
#if SQLITE_OMIT_FLOATING_POINT
																																																												// define double sqlite_int64
// define float sqlite_int64
// define LONGDOUBLE_TYPE sqlite_int64
//if !SQLITE_BIG_DBL
//   define SQLITE_BIG_DBL (((sqlite3_int64)1)<<50)
// endif
// define SQLITE_OMIT_DATETIME_FUNCS 1
// define SQLITE_OMIT_TRACE 1
// undef SQLITE_MIXED_ENDIAN_64BIT_FLOAT
// undef SQLITE_HAVE_ISNAN
#endif
#if !SQLITE_BIG_DBL
        public const double SQLITE_BIG_DBL = (((sqlite3_int64)1) << 60);
        //# define SQLITE_BIG_DBL (1e99)
#endif
        ///
        ///<summary>
        ///OMIT_TEMPDB is set to 1 if SQLITE_OMIT_TEMPDB is defined, or 0
        ///afterward. Having this macro allows us to cause the C compiler
        ///to omit code used by TEMP tables without messy #if !statements.
        ///</summary>
#if SQLITE_OMIT_TEMPDB
																																																												//define OMIT_TEMPDB 1
#else
        public static int OMIT_TEMPDB = 0;
#endif
        ///
        ///<summary>
        ///The "file format" number is an integer that is incremented whenever
        ///</summary>
        ///<param name="the VDBE">level file format changes.  The following macros define the</param>
        ///<param name="the default file format for new databases and the maximum file format">the default file format for new databases and the maximum file format</param>
        ///<param name="that the library can read.">that the library can read.</param>
        public static int SQLITE_MAX_FILE_FORMAT = 4;
        //#define SQLITE_MAX_FILE_FORMAT 4
        //#if !SQLITE_DEFAULT_FILE_FORMAT
        public static int SQLITE_DEFAULT_FILE_FORMAT = 1;
        //# define SQLITE_DEFAULT_FILE_FORMAT 1
        //#endif
        ///
        ///<summary>
        ///Determine whether triggers are recursive by default.  This can be
        ///</summary>
        ///<param name="changed at run">time using a pragma.</param>
        ///<param name=""></param>
#if !SQLITE_DEFAULT_RECURSIVE_TRIGGERS
        //# define SQLITE_DEFAULT_RECURSIVE_TRIGGERS 0
        public static bool SQLITE_DEFAULT_RECURSIVE_TRIGGERS = false;
#else
																																																												static public bool SQLITE_DEFAULT_RECURSIVE_TRIGGERS = true;
#endif
        ///
        ///<summary>
        ///Provide a default value for SQLITE_TEMP_STORE in case it is not specified
        ///</summary>
        ///<param name="on the command">line</param>
        //#if !SQLITE_TEMP_STORE
        public static int SQLITE_TEMP_STORE = 1;
        //#define SQLITE_TEMP_STORE 1
        //#endif
        ///
        ///<summary>
        ///GCC does not define the offsetof() macro so we'll have to do it
        ///ourselves.
        ///</summary>
#if !offsetof
        //#define offsetof(STRUCTURE,FIELD) ((int)((char)&((STRUCTURE)0)->FIELD))
#endif
        ///
        ///<summary>
        ///Check to see if this machine uses EBCDIC.  (Yes, believe it or
        ///not, there are still machines out there that use EBCDIC.)
        ///</summary>
#if FALSE
																																																												// define SQLITE_EBCDIC 1
#else
        private const int SQLITE_ASCII = 1;
        //#define SQLITE_ASCII 1
#endif
        ///
        ///<summary>
        ///Integers of known sizes.  These typedefs might change for architectures
        ///where the sizes very.  Preprocessor macros are available so that the
        ///</summary>
        ///<param name="types can be conveniently redefined at compile">type.  Like this:</param>
        ///<param name=""></param>
        ///<param name="cc '">Du32PTR_TYPE=long long int' ...</param>
        //#if !u32_TYPE
        //# ifdef HAVE_u32_T
        //#  define u32_TYPE u32_t
        //# else
        //#  define u32_TYPE unsigned int
        //# endif
        //#endif
        //#if !u3216_TYPE
        //# ifdef HAVE_u3216_T
        //#  define u3216_TYPE u3216_t
        //# else
        //#  define u3216_TYPE unsigned short int
        //# endif
        //#endif
        //#if !INT16_TYPE
        //# ifdef HAVE_INT16_T
        //#  define INT16_TYPE int16_t
        //# else
        //#  define INT16_TYPE short int
        //# endif
        //#endif
        //#if !u328_TYPE
        //# ifdef HAVE_u328_T
        //#  define u328_TYPE u328_t
        //# else
        //#  define u328_TYPE unsigned char
        //# endif
        //#endif
        //#if !INT8_TYPE
        //# ifdef HAVE_INT8_T
        //#  define INT8_TYPE int8_t
        //# else
        //#  define INT8_TYPE signed char
        //# endif
        //#endif
        //#if !LONGDOUBLE_TYPE
        //# define LONGDOUBLE_TYPE long double
        //#endif
        //typedef sqlite_int64 i64;          /* 8-byte signed integer */
        //typedef sqlite_u3264 u64;         /* 8-byte unsigned integer */
        //typedef u32_TYPE u32;           /* 4-byte unsigned integer */
        //typedef u3216_TYPE u16;           /* 2-byte unsigned integer */
        //typedef INT16_TYPE i16;            /* 2-byte signed integer */
        //typedef u328_TYPE u8;             /* 1-byte unsigned integer */
        //typedef INT8_TYPE i8;              /* 1-byte signed integer */
        ///
        ///<summary>
        ///sqliteinth.SQLITE_MAX_U32 is a u64 constant that is the maximum u64 value
        ///that can be stored in a u32 without loss of data.  The value
        ///is 0x00000000ffffffff.  But because of quirks of some compilers, we
        ///have to specify the value in the less intuitive manner shown:
        ///
        ///</summary>
        //#define sqliteinth.SQLITE_MAX_U32  ((((u64)1)<<32)-1)
        public const u32 SQLITE_MAX_U32 = (u32)((((u64)1) << 32) - 1);
        ///
        ///<summary>
        ///Macros to determine whether the machine is big or little endian,
        ///evaluated at runtime.
        ///
        ///</summary>
#if SQLITE_AMALGAMATION
																																																												//const int sqlite3one = 1;
#else
        private const bool sqlite3one = true;
#endif
#if i386 || __i386__ || _M_IX86
																																																												const int ;//define SQLITE_BIGENDIAN    0
const int ;//define SQLITE_LITTLEENDIAN 1
const int ;//define SQLITE_UTF16NATIVE  SqliteEncoding.UTF16LE
#else
        public static u8 SQLITE_BIGENDIAN = 0;
        //#define SQLITE_BIGENDIAN    (*(char )(&sqlite3one)==0)
        public static u8 SQLITE_LITTLEENDIAN = 1;
        //#define SQLITE_LITTLEENDIAN (*(char )(&sqlite3one)==1)
        //#define SQLITE_UTF16NATIVE (SQLITE_BIGENDIAN?SqliteEncoding.UTF16BE:SqliteEncoding.UTF16LE)
#endif
        ///<summary>

        ///
        ///<summary>
        ///</summary>
        ///<param name="Assert that the pointer X is aligned to an 8">byte boundary.  This</param>
        ///<param name="macro is used only within Debug.Assert() to verify that the code gets">macro is used only within Debug.Assert() to verify that the code gets</param>
        ///<param name="all alignment restrictions correct.">all alignment restrictions correct.</param>
        ///<param name=""></param>
        ///<param name="Except, if SQLITE_4_BYTE_ALIGNED_MALLOC is defined, then the">Except, if SQLITE_4_BYTE_ALIGNED_MALLOC is defined, then the</param>
        ///<param name="underlying malloc() implemention might return us 4">byte aligned</param>
        ///<param name="pointers.  In that case, only verify 4">byte alignment.</param>
        ///<param name=""></param>
        //#if SQLITE_4_BYTE_ALIGNED_MALLOC
        //# define EIGHT_BYTE_ALIGNMENT(X)   ((((char)(X) - (char)0)&3)==0)
        //#else
        //# define EIGHT_BYTE_ALIGNMENT(X)   ((((char)(X) - (char)0)&7)==0)
        //#endif
        //////////////---------------------BusyHandler
        ///
        ///<summary>
        ///Name of the master database table.  The master database table
        ///is a special table that holds the names and attributes of all
        ///user tables and indices.
        ///
        ///</summary>
        private const string MASTER_NAME = "sqlite_master";
        //#define MASTER_NAME       "sqlite_master"
        private const string TEMP_MASTER_NAME = "sqlite_temp_master";
        //#define TEMP_MASTER_NAME  "sqlite_temp_master"
        ///<summary>
        /// The root-page of the master database table.
        ///
        ///</summary>
        public const int MASTER_ROOT = 1;
        //#define MASTER_ROOT       1
        ///
        ///<summary>
        ///The name of the schema table.
        ///
        ///</summary>
        public static string SCHEMA_TABLE(int x)//#define SCHEMA_TABLE(x)  ((!OMIT_TEMPDB)&&(x==1)?TEMP_MASTER_NAME:MASTER_NAME)
        {
            return ((OMIT_TEMPDB == 0) && (x == 1) ? TEMP_MASTER_NAME : MASTER_NAME);
        }

        ///
        ///<summary>
        ///The following value as a destructor means to use sqlite3DbFree().
        ///This is an internal extension to SQLITE_STATIC and SQLITE_TRANSIENT.
        ///
        ///</summary>
        //#define SQLITE_DYNAMIC   ((sqlite3_destructor_type)sqlite3DbFree)
        public static dxDel SQLITE_DYNAMIC;
        ///
        ///<summary>
        ///When SQLITE_OMIT_WSD is defined, it means that the target platform does
        ///not support Writable Static Data (WSD) such as global and static variables.
        ///All variables must either be on the stack or dynamically allocated from
        ///the heap.  When WSD is unsupported, the variable declarations scattered
        ///throughout the SQLite code must become constants instead.  The SQLITE_WSD
        ///macro is used for this purpose.  And instead of referencing the variable
        ///</summary>
        ///<param name="directly, we use its constant as a key to lookup the run">time allocated</param>
        ///<param name="buffer that holds real variable.  The constant is also the initializer">buffer that holds real variable.  The constant is also the initializer</param>
        ///<param name="for the run">time allocated buffer.</param>
        ///<param name=""></param>
        ///<param name="In the usual case where WSD is supported, the SQLITE_WSD and GLOBAL">In the usual case where WSD is supported, the SQLITE_WSD and GLOBAL</param>
        ///<param name="macros become no">ops and have zero performance impact.</param>
        ///<param name=""></param>
#if SQLITE_OMIT_WSD
																																																												//define SQLITE_WSD const
//define GLOBAL(t,v) (*(t)sqlite3_wsd_find((void)&(v), sizeof(v)))
//define sqliteinth.sqlite3GlobalConfig GLOBAL(struct Sqlite3Config, sqlite3Config)
int sqlite3_wsd_init(int N, int J);
void *sqlite3_wsd_find(void *K, int L);
#else
        //#define SQLITE_WSD
        //#define GLOBAL(t,v) v
        //#define sqliteinth.sqlite3GlobalConfig sqlite3Config
        public static Sqlite3Config sqlite3GlobalConfig;
#endif
        ///<summary>
        /// The following macros are used to suppress compiler warnings and to
        /// make it clear to human readers when a function parameter is deliberately
        /// left unused within the body of a function. This usually happens when
        /// a function is called via a function pointer. For example the
        /// implementation of an SQL aggregate step callback may not use the
        /// parameter indicating the number of arguments passed to the aggregate,
        /// if it knows that this is enforced elsewhere.
        ///
        /// When a function parameter is not used at all within the body of a function,
        /// it is generally named "NotUsed" or "NotUsed2" to make things even clearer.
        /// However, these macros may also be used to suppress warnings related to
        /// parameters that may or may not be used depending on compilation options.
        /// For example those parameters only used in Debug.Assert() statements. In these
        /// cases the parameters are named as per the usual conventions.
        ///</summary>
        //#define sqliteinth.UNUSED_PARAMETER(x) (void)(x)
        public static void UNUSED_PARAMETER<T>(T x)
        {
        }
        //#define sqliteinth.UNUSED_PARAMETER2(x,y) sqliteinth.UNUSED_PARAMETER(x),sqliteinth.UNUSED_PARAMETER(y)
        public static void UNUSED_PARAMETER2<T1, T2>(T1 x, T2 y)
        {
            sqliteinth.UNUSED_PARAMETER(x);
            sqliteinth.UNUSED_PARAMETER(y);
        }
        ///
        ///<summary>
        ///Forward references to structures
        ///
        ///</summary>

        ///<summary>
        /// These macros can be used to test, set, or clear bits in the
        /// Db.pSchema->flags field.
        ///
        ///</summary>
        //#define DbHasProperty(D,I,P)     (((D)->aDb[I].pSchema->flags&(P))==(P))
        private static bool DbHasProperty(sqlite3 D, int I, ushort P)
        {
            return (D.aDb[I].pSchema.flags & P) == P;
        }
        //#define DbHasAnyProperty(D,I,P)  (((D)->aDb[I].pSchema->flags&(P))!=0)
        //#define DbSetProperty(D,I,P)     (D)->aDb[I].pSchema->flags|=(P)
        private static void DbSetProperty(sqlite3 D, int I, ushort P)
        {
            D.aDb[I].pSchema.flags = (u16)(D.aDb[I].pSchema.flags | P);
        }
        //#define DbClearProperty(D,I,P)   (D)->aDb[I].pSchema->flags&=~(P)
        public static void DbClearProperty(sqlite3 D, int I, ushort P)
        {
            D.aDb[I].pSchema.flags = (u16)(D.aDb[I].pSchema.flags & ~P);
        }
        ///
        ///<summary>
        ///</summary>
        ///<param name="Allowed values for the DB.pSchema">>flags field.</param>
        ///<param name=""></param>
        ///<param name="The DB_SchemaLoaded flag is set after the database schema has been">The DB_SchemaLoaded flag is set after the database schema has been</param>
        ///<param name="read into internal hash tables.">read into internal hash tables.</param>
        ///<param name=""></param>
        ///<param name="DB_UnresetViews means that one or more views have column names that">DB_UnresetViews means that one or more views have column names that</param>
        ///<param name="have been filled out.  If the schema changes, these column names might">have been filled out.  If the schema changes, these column names might</param>
        ///<param name="changes and so the view will need to be reset.">changes and so the view will need to be reset.</param>
        ///<param name=""></param>
        //#define DB_SchemaLoaded    0x0001  /* The schema has been loaded */
        //#define DB_UnresetViews    0x0002  /* Some views have defined column names */
        //#define DB_Empty           0x0004  /* The file is empty (length 0 bytes) */
        public const u16 DB_SchemaLoaded = 0x0001;
        public const u16 DB_UnresetViews = 0x0002;
        public const u16 DB_Empty = 0x0004;
        ///<summary>
        /// The number of different kinds of things that can be limited
        /// using the sqlite3_limit() interface.
        ///
        ///</summary>
        //#define SQLITE_N_LIMIT (SQLITE_LIMIT_TRIGGER_DEPTH+1)
        public const int SQLITE_N_LIMIT = Globals.SQLITE_LIMIT_TRIGGER_DEPTH + 1;




        ///<summary>
        /// A macro to discover the encoding of a database.
        ///
        ///</summary>
        //#define ENC(db) ((db)->aDb[0].pSchema->enc)
        public static SqliteEncoding ENC(sqlite3 db)
        {
            return db.aDb[0].pSchema.enc;
        }
        ///

        ///////////---------------------------------savepoint

        ///<summary>
        /// The following are used as the second parameter to build.sqlite3Savepoint(),
        /// and as the P1 argument to the OP_Savepoint instruction.
        ///
        ///</summary>
        public const int SAVEPOINT_BEGIN = 0;
        //#define SAVEPOINT_BEGIN      0
        public const int SAVEPOINT_RELEASE = 1;
        //#define SAVEPOINT_RELEASE    1
        public const int SAVEPOINT_ROLLBACK = 2;
        //#define sqliteinth.SAVEPOINT_ROLLBACK   2


        /////////////////----------------------------Module




        ///
        ///<summary>
        ///Column affinity types.
        ///
        ///These used to have mnemonic name like 'i' for SQLITE_AFF_INTEGER and
        ///'t' for sqliteinth.SQLITE_AFF_TEXT.  But we can save a little space and improve
        ///the speed a little by numbering the values consecutively.
        ///
        ///But rather than start with 0 or 1, we begin with 'a'.  That way,
        ///when multiple affinity types are concatenated into a string and
        ///used as the P4 operand, they will be more readable.
        ///
        ///Note also that the numeric types are grouped together so that testing
        ///for a numeric type is a single comparison.
        ///
        ///</summary>
        public const char SQLITE_AFF_TEXT = 'a';
        //#define sqliteinth.SQLITE_AFF_TEXT     'a'
        public const char SQLITE_AFF_NONE = 'b';
        //#define SQLITE_AFF_NONE     'b'
        public const char SQLITE_AFF_NUMERIC = 'c';
        //#define SQLITE_AFF_NUMERIC  'c'
        public const char SQLITE_AFF_INTEGER = 'd';
        //#define SQLITE_AFF_INTEGER  'd'
        public const char SQLITE_AFF_REAL = 'e';
        //#define SQLITE_AFF_REAL     'e'
        //#define sqlite3IsNumericAffinity(X)  ((X)>=SQLITE_AFF_NUMERIC)
        ///
        ///<summary>
        ///The SQLITE_AFF_MASK values masks off the significant bits of an
        ///affinity value.
        ///
        ///</summary>
        public const int SQLITE_AFF_MASK = 0x67;
        //#define SQLITE_AFF_MASK     0x67
        ///<summary>
        /// Additional bit values that can be ORed with an affinity without
        /// changing the affinity.
        ///
        ///</summary>
        public const int SQLITE_JUMPIFNULL = 0x08;
        //#define sqliteinth.SQLITE_JUMPIFNULL   0x08  /* jumps if either operand is NULL */
        public const int SQLITE_STOREP2 = 0x10;
        //#define SQLITE_STOREP2      0x10  /* Store result in reg[P2] rather than jump */
        public const int SQLITE_NULLEQ = 0x80;
        //#define sqliteinth.SQLITE_NULLEQ       0x80  /* NULL=NULL */

        ///////////////////////////////////////--------------------------------VTABLE





        ///<summary>
        /// The datatype ynVar is a signed integer, either 16-bit or 32-bit.
        /// Usually it is 16-bits.  But if SQLITE_MAX_VARIABLE_NUMBER is greater
        /// than 32767 we have to make it 32-bit.  16-bit is preferred because
        /// it uses less memory in the Expr object, which is a big memory user
        /// in systems with lots of prepared statements.  And few applications
        /// need more than about 10 or 20 variables.  But some extreme users want
        /// to have prepared statements with over 32767 variables, and for them
        /// the option is available (at compile-time).
        ///
        ///</summary>
        //#if SQLITE_MAX_VARIABLE_NUMBER<=32767
        //typedef i16 ynVar;
        //#else
        //typedef int ynVar;
        //#endif

        ///<summary>
        /// The pseudo-routine exprc.sqlite3ExprSetIrreducible sets the EP2_Irreducible
        /// flag on an expression structure.  This flag is used for VV&A only.  The
        /// routine is implemented as a macro that only works when in debugging mode,
        /// so as not to burden production code.
        ///
        ///</summary>
#if SQLITE_DEBUG
																																																												    // define ExprSetIrreducible(X)  (X)->flags2 |= EP2_Irreducible
    static void ExprSetIrreducible( Expr X )
    {
      X.flags2 |= EP2_Irreducible;
    }
#else
        //# define ExprSetIrreducible(X)
#endif
        ///<summary>
        /// These macros can be used to test, set, or clear bits in the
        /// Expr.flags field.
        ///</summary>
        //#define ExprHasProperty(E,P)     (((E)->flags&(P))==(P))
        //#define ExprHasAnyProperty(E,P)  (((E)->flags&(P))!=0)
        //#define ExprSetProperty(E,P)     (E)->flags|=(P)
        //#define ExprClearProperty(E,P)   (E)->flags&=~(P)
        ///






        ///<summary>
        /// Size of the column cache
        ///
        ///</summary>
#if !SQLITE_N_COLCACHE
        //# define SQLITE_N_COLCACHE 10
        public const int SQLITE_N_COLCACHE = 10;
#endif
        ///<summary>
        /// The yDbMask datatype for the bitmask of all attached databases.
        ///
        ///</summary>
        //#if SQLITE_MAX_ATTACHED>30
        //  typedef sqlite3_uint64 yDbMask;
        //#else
        //  typedef unsigned int yDbMask;
        //#endif
        ///<summary>
        /// An SQL parser context.  A copy of this structure is passed through
        /// the parser and down into all the parser action routine in order to
        /// carry around information that is global to the entire parse.
        ///
        /// The structure is divided into two parts.  When the parser and code
        /// generate call themselves recursively, the first part of the structure
        /// is constant but the second part is reset at the beginning and end of
        /// each recursion.
        ///
        /// The nTableLock and aTableLock variables are only used if the shared-cache
        /// feature is enabled (if sqlite3Tsd()->useSharedData is true). They are
        /// used to store the set of table-locks required by the statement being
        /// compiled. Function sqliteinth.sqlite3TableLock() is used to add entries to the
        /// list.
        ///
        ///</summary>
        public class yColCache
        {
            public int iTable;
            ///
            ///<summary>
            ///Table cursor number 
            ///</summary>
            public int iColumn;
            ///
            ///<summary>
            ///Table column number 
            ///</summary>
            public u8 tempReg;
            ///
            ///<summary>
            ///iReg is a temp register that needs to be freed 
            ///</summary>
            public int iLevel;
            ///
            ///<summary>
            ///Nesting level 
            ///</summary>
            public int iReg;
            ///
            ///<summary>
            ///Reg with value of this column. 0 means none. 
            ///</summary>
            public int lru;
            ///
            ///<summary>
            ///Least recently used entry has the smallest value 
            ///</summary>
        }
#if SQLITE_OMIT_VIRTUALTABLE
																																																												//define IN_DECLARE_VTAB 0
    static bool IN_DECLARE_VTAB( Parse pParse )
    {
      return false;
    }
#else
        //#define IN_DECLARE_VTAB (pParse.declareVtab)
        public static bool IN_DECLARE_VTAB(Parse pParse)
        {
            return pParse.declareVtab != 0;
        }
#endif
        /////------------------------------------------------



        //-------------------------DbFixer
        ///////////////------------------initdata

        ///<summary>
        /// Assuming zIn points to the first byte of a UTF-8 character,
        /// advance zIn to point to the first byte of the next UTF-8 character.
        ///
        ///</summary>
        //#define sqliteinth.SQLITE_SKIP_UTF8(zIn) {                        \
        //  if( (*(zIn++))>=0xc0 ){                              \
        //    while( (*zIn & 0xc0)==0x80 ){ zIn++; }             \
        //  }                                                    \
        //}
        public static void SQLITE_SKIP_UTF8(string zIn, ref int iz)
        {
            iz++;
            if (iz < zIn.Length && zIn[iz - 1] >= 0xC0)
            {
                while (iz < zIn.Length && (zIn[iz] & 0xC0) == 0x80)
                {
                    iz++;
                }
            }
        }
        public static void SQLITE_SKIP_UTF8(byte[] zIn, ref int iz)
        {
            iz++;
            if (iz < zIn.Length && zIn[iz - 1] >= 0xC0)
            {
                while (iz < zIn.Length && (zIn[iz] & 0xC0) == 0x80)
                {
                    iz++;
                }
            }
        }
        ///<summary>
        /// The SQLITE_*_BKPT macros are substitutes for the error codes with
        /// the same name but without the _BKPT suffix.  These macros invoke
        /// routines that report the line-number on which the error originated
        /// using io.sqlite3_log().  The routines also provide a convenient place
        /// to set a debugger breakpoint.
        ///
        ///</summary>
        //int sqlite3CorruptError(int);
        //int sqlite3MisuseError(int);
        //int sqlite3CantopenError(int);
#if DEBUG
																																																												  
    //define SQLITE_CORRUPT_BKPT sqlite3CorruptError(__LINE__)
    static int SQLITE_CORRUPT_BKPT()
    {
      return sqlite3CorruptError( 0 );
    }

    //define sqliteinth.SQLITE_MISUSE_BKPT sqlite3MisuseError(__LINE__)
    static int sqliteinth.SQLITE_MISUSE_BKPT()
    {
      return sqlite3MisuseError( 0 );
    }

    //define  sqliteinth.SQLITE_CANTOPEN_BKPT sqlite3CantopenError(__LINE__)
    static int  sqliteinth.SQLITE_CANTOPEN_BKPT()
    {
      return sqlite3CantopenError( 0 );
    }
#else
        public static SqlResult SQLITE_CORRUPT_BKPT()
        {
            return SqlResult.SQLITE_CORRUPT;
        }
        public static SqlResult SQLITE_MISUSE_BKPT()
        {
            return SqlResult.SQLITE_MISUSE;
        }
        public static SqlResult SQLITE_CANTOPEN_BKPT()
        {
            return SqlResult.SQLITE_CANTOPEN;
        }
#endif
        ///
        ///<summary>
        ///Internal function prototypes
        ///</summary>
        //int sqlite3StrICmp(string , string );
        //int StringExtensions.sqlite3Strlen30(const char);
        //#define StringExtensions.sqlite3StrNICmp sqlite3_strnicmp
        //int malloc_cs.sqlite3MallocInit(void);
        //void malloc_cs.sqlite3MallocEnd(void);
        //void *malloc_cs.sqlite3Malloc(int);
        //void *malloc_cs.sqlite3MallocZero(int);
        //void *sqlite3DbMallocZero(sqlite3*, int);
        //void *sqlite3DbMallocRaw(sqlite3*, int);
        //char *sqlite3DbStrDup(sqlite3*,const char);
        //char *sqlite3DbStrNDup(sqlite3*,const char*, int);
        //void *sqlite3Realloc(void*, int);
        //void *sqlite3DbReallocOrFree(sqlite3 *, object  *, int);
        //void *sqlite3DbRealloc(sqlite3 *, object  *, int);
        //void sqlite3DbFree(sqlite3*, void);
        //int malloc_cs.sqlite3MallocSize(void);
        //int sqlite3DbMallocSize(sqlite3*, void);
        //void *sqlite3ScratchMalloc(int);
        //void //sqlite3ScratchFree(void);
        //void *sqlite3PageMalloc(int);
        //void sqlite3PageFree(void);
        //void sqlite3MemSetDefault(void);
        //void sqlite3BenignMallocHooks(void ()(void), object  ()(void));
        //int sqlite3HeapNearlyFull(void);
        ///<summary>
        /// On systems with ample stack space and that support alloca(), make
        /// use of alloca() to obtain space for large automatic objects.  By default,
        /// obtain space from malloc().
        ///
        /// The alloca() routine never returns NULL.  This will cause code paths
        /// that deal with sqlite3StackAlloc() failures to be unreachable.
        ///
        ///</summary>
#if SQLITE_USE_ALLOCA
																																																												// define sqlite3StackAllocRaw(D,N)   alloca(N)
// define sqlite3StackAllocZero(D,N)  memset(alloca(N), 0, N)
// define sqlite3StackFree(D,P)
#else
#if FALSE
																																																												// define sqlite3StackAllocRaw(D,N)   sqlite3DbMallocRaw(D,N)
static void sqlite3StackAllocRaw( sqlite3 D, int N ) { sqlite3DbMallocRaw( D, N ); }
// define sqlite3StackAllocZero(D,N)  sqlite3DbMallocZero(D,N)
static void sqlite3StackAllocZero( sqlite3 D, int N ) { sqlite3DbMallocZero( D, N ); }
// define sqlite3StackFree(D,P)       sqlite3DbFree(D,P)
static void sqlite3StackFree( sqlite3 D, object P ) {sqlite3DbFree( D, P ); }
#endif
#endif
#if SQLITE_ENABLE_MEMSYS3
																																																												const sqlite3_mem_methods *sqlite3MemGetMemsys3(void);
#endif
#if SQLITE_ENABLE_MEMSYS5
																																																												const sqlite3_mem_methods *sqlite3MemGetMemsys5(void);
#endif
#if !SQLITE_MUTEX_OMIT
																																																												    //  sqlite3_mutex_methods const *sqlite3DefaultMutex(void);
    //  sqlite3_mutex_methods const *sqlite3NoopMutex(void);
    //  sqlite3_mutex *sqlite3MutexAlloc(int);
    //  int sqlite3MutexInit(void);
    //  int sqlite3MutexEnd(void);
#endif
        //int sqlite3StatusValue(int);
        //void sqlite3StatusAdd(int, int);
        //void sqlite3StatusSet(int, int);
        //#if !SQLITE_OMIT_FLOATING_POINT
        //  int MathExtensions.sqlite3IsNaN(double);
        //#else
        //# define MathExtensions.sqlite3IsNaN(X)  0
        //#endif
        //void sqlite3VXPrintf(StrAccum*, int, const char*, va_list);
#if !SQLITE_OMIT_TRACE
        //void io.sqlite3XPrintf(StrAccum*, const char*, ...);
#endif
        //char *io.sqlite3MPrintf(sqlite3*,const char*, ...);
        //char *sqlite3VMPrintf(sqlite3*,const char*, va_list);
        //char *sqlite3MAppendf(sqlite3*,char*,const char*,...);
#if SQLITE_TEST || SQLITE_DEBUG
																																																												    //  void sqlite3DebugPrintf(const char*, ...);
#endif
#if SQLITE_TEST
																																																												    //  void *sqlite3TestTextToPtr(const char);
#endif
        //void malloc_cs.sqlite3SetString(char **, sqlite3*, const char*, ...);
        //void utilc.sqlite3ErrorMsg(Parse*, const char*, ...);
        //int StringExtensions.sqlite3Dequote(char);
        //int sqlite3KeywordCode(const unsigned char*, int);
        //int sqlite3RunParser(Parse*, const char*, char *);
        //void sqlite3FinishCoding(Parse);
        //int sqlite3GetTempReg(Parse);
        //void sqlite3ReleaseTempReg(Parse*,int);
        //int sqlite3GetTempRange(Parse*,int);
        //void sqlite3ReleaseTempRange(Parse*,int,int);
        //Expr *exprc.sqlite3ExprAlloc(sqlite3*,int,const Token*,int);
        //Expr *exprc.sqlite3Expr(sqlite3*,int,const char);
        //void exprc.sqlite3ExprAttachSubtrees(sqlite3*,Expr*,Expr*,Expr);
        //Expr *sqlite3PExpr(Parse*, int, Expr*, Expr*, const Token);
        //Expr *exprc.sqlite3ExprAnd(sqlite3*,Expr*, Expr);
        //Expr *exprc.sqlite3ExprFunction(Parse*,ExprList*, Token);
        //void exprc.sqlite3ExprAssignVarNumber(Parse*, Expr);
        //void exprc.sqlite3ExprDelete(sqlite3*, Expr);
        //ExprList *exprc.sqlite3ExprListAppend(Parse*,ExprList*,Expr);
        //void exprc.sqlite3ExprListSetName(Parse*,ExprList*,Token*,int);
        //void exprc.sqlite3ExprListSetSpan(Parse*,ExprList*,ExprSpan);
        //void exprc.sqlite3ExprListDelete(sqlite3*, ExprList);
        //int sqlite3Init(sqlite3*, char*);
        //int sqlite3InitCallback(void*, int, char**, char*);
        //void sqlite3Pragma(Parse*,Token*,Token*,Token*,int);
        //void build.sqlite3ResetInternalSchema(sqlite3*, int);
        //void sqlite3BeginParse(Parse*,int);
        //void sqlite3CommitInternalChanges(sqlite3);
        //Table *SelectMethods.sqlite3ResultSetOfSelect(Parse*,Select);
        //void sqlite3OpenMasterTable(Parse *, int);
        //void build.sqlite3StartTable(Parse*,Token*,Token*,int,int,int,int);
        //void build.sqlite3AddColumn(Parse*,Token);
        //void build.sqlite3AddNotNull(Parse*, int);
        //void build.sqlite3AddPrimaryKey(Parse*, ExprList*, int, int, int);
        //void build.sqlite3AddCheckConstraint(Parse*, Expr);
        //void build.sqlite3AddColumnType(Parse*,Token);
        //void build.sqlite3AddDefaultValue(Parse*,ExprSpan);
        //void build.sqlite3AddCollateType(Parse*, Token);
        //void sqlite3EndTable(Parse*,Token*,Token*,Select);
        //int sqlite3ParseUri(const char*,const char*,unsigned int*,
        //                sqlite3_vfs**,char**,char *);
        //Bitvec *sqlite3BitvecCreate(u32);
        //int sqlite3BitvecTest(Bitvec*, u32);
        //int sqlite3BitvecSet(Bitvec*, u32);
        //void sqlite3BitvecClear(Bitvec*, u32, void);
        //void sqlite3BitvecDestroy(Bitvec);
        //u32 sqlite3BitvecSize(Bitvec);
        //int sqlite3BitvecBuiltinTest(int,int);
        //RowSet *sqlite3RowSetInit(sqlite3*, void*, unsigned int);
        //void sqlite3RowSetClear(RowSet);
        //void sqlite3RowSetInsert(RowSet*, i64);
        //int sqlite3RowSetTest(RowSet*, u8 iBatch, i64);
        //int sqlite3RowSetNext(RowSet*, i64);
        //void sqlite3CreateView(Parse*,Token*,Token*,Token*,Select*,int,int);
#if !SQLITE_OMIT_VIEW || !SQLITE_OMIT_VIRTUALTABLE
        //int build.sqlite3ViewGetColumnNames(Parse*,Table);
#else
																																																												    // define build.sqlite3ViewGetColumnNames(A,B) 0
    static int build.sqlite3ViewGetColumnNames( Parse A, Table B )
    {
      return 0;
    }
#endif
        //void sqlite3DropTable(Parse*, SrcList*, int, int);
        //void build.sqlite3DeleteTable(sqlite3*, Table);
        //#if !SQLITE_OMIT_AUTOINCREMENT
        //  void sqlite3AutoincrementBegin(Parse *pParse);
        //  void sqlite3AutoincrementEnd(Parse *pParse);
        //#else
        //# define sqlite3AutoincrementBegin(X)
        //# define sqlite3AutoincrementEnd(X)
        //#endif
        //void sqlite3Insert(Parse*, SrcList*, ExprList*, Select*, IdList*, int);
        //void *sqlite3ArrayAllocate(sqlite3*,void*,int,int,int*,int*,int);
        //IdList *build.sqlite3IdListAppend(sqlite3*, IdList*, Token);
        //int sqlite3IdListIndex(IdList*,const char);
        //SrcList *build.sqlite3SrcListEnlarge(sqlite3*, SrcList*, int, int);
        //SrcList *build.sqlite3SrcListAppend(sqlite3*, SrcList*, Token*, Token);
        //SrcList *build.sqlite3SrcListAppendFromTerm(Parse*, SrcList*, Token*, Token*,
        //                                      Token*, Select*, Expr*, IdList);
        //void build.sqlite3SrcListIndexedBy(Parse *, SrcList *, Token );
        //int sqlite3IndexedByLookup(Parse *, struct SrcList_item );
        //void build.sqlite3SrcListShiftJoinType(SrcList);
        //void sqlite3SrcListAssignCursors(Parse*, SrcList);
        //void build.sqlite3IdListDelete(sqlite3*, IdList);
        //void build.sqlite3SrcListDelete(sqlite3*, SrcList);
        //Index *build.sqlite3CreateIndex(Parse*,Token*,Token*,SrcList*,ExprList*,int,Token*,
        //                        Token*, int, int);
        //void sqlite3DropIndex(Parse*, SrcList*, int);
        //int sqlite3Select(Parse*, Select*, SelectDest);
        //Select *sqlite3SelectNew(Parse*,ExprList*,SrcList*,Expr*,ExprList*,
        //                         Expr*,ExprList*,int,Expr*,Expr);
        //void SelectMethods.sqlite3SelectDelete(sqlite3*, Select);
        //Table *sqlite3SrcListLookup(Parse*, SrcList);
        //int sqlite3IsReadOnly(Parse*, Table*, int);
        //void sqlite3OpenTable(Parse*, int iCur, int iDb, Table*, int);
#if (SQLITE_ENABLE_UPDATE_DELETE_LIMIT) && !(SQLITE_OMIT_SUBQUERY)
																																																												//Expr *sqlite3LimitWhere(Parse *, SrcList *, Expr *, ExprList *, Expr *, Expr *, char );
#endif
        //void sqlite3DeleteFrom(Parse*, SrcList*, Expr);
        //void sqlite3Update(Parse*, SrcList*, ExprList*, Expr*, int);
        //WhereInfo *sqlite3WhereBegin(Parse*, SrcList*, Expr*, ExprList**, u16);
        //void sqlite3WhereEnd(WhereInfo);
        //int exprc.sqlite3ExprCodeGetColumn(Parse*, Table*, int, int, int);
        //void exprc.sqlite3ExprCodeGetColumnOfTable(Vdbe*, Table*, int, int, int);
        //void exprc.sqlite3ExprCodeMove(Parse*, int, int, int);
        //void exprc.sqlite3ExprCodeCopy(Parse*, int, int, int);
        //void exprc.sqlite3ExprCacheStore(Parse*, int, int, int);
        //void exprc.sqlite3ExprCachePush(Parse);
        //void exprc.sqlite3ExprCachePop(Parse*, int);
        //void exprc.sqlite3ExprCacheRemove(Parse*, int, int);
        //void exprc.sqlite3ExprCacheClear(Parse);
        //void exprc.sqlite3ExprCacheAffinityChange(Parse*, int, int);
        //int exprc.sqlite3ExprCode(Parse*, Expr*, int);
        //int exprc.sqlite3ExprCodeTemp(Parse*, Expr*, int);
        //int exprc.sqlite3ExprCodeTarget(Parse*, Expr*, int);
        //int exprc.sqlite3ExprCodeAndCache(Parse*, Expr*, int);
        //void exprc.sqlite3ExprCodeConstants(Parse*, Expr);
        //int exprc.sqlite3ExprCodeExprList(Parse*, ExprList*, int, int);
        //void exprc.sqlite3ExprIfTrue(Parse*, Expr*, int, int);
        //void exprc.sqlite3ExprIfFalse(Parse*, Expr*, int, int);
        //Table *build.sqlite3FindTable(sqlite3*,const char*, const char);
        //Table *build.sqlite3LocateTable(Parse*,int isView,const char*, const char);
        //Index *build.sqlite3FindIndex(sqlite3*,const char*, const char);
        //void build.sqlite3UnlinkAndDeleteTable(sqlite3*,int,const char);
        //void build.sqlite3UnlinkAndDeleteIndex(sqlite3*,int,const char);
        //void sqlite3Vacuum(Parse);
        //int sqlite3RunVacuum(char**, sqlite3);
        //char *build.sqlite3NameFromToken(sqlite3*, Token);
        //int exprc.sqlite3ExprCompare(Expr*, Expr);
        //int exprc.sqlite3ExprListCompare(ExprList*, ExprList);
        //void exprc.sqlite3ExprAnalyzeAggregates(NameContext*, Expr);
        //void exprc.sqlite3ExprAnalyzeAggList(NameContext*,ExprList);
        //Vdbe *sqlite3GetVdbe(Parse);
        //void sqlite3PrngSaveState(void);
        //void sqlite3PrngRestoreState(void);
        //void sqlite3PrngResetState(void);
        //void sqlite3RollbackAll(sqlite3);
        //void sqlite3CodeVerifySchema(Parse*, int);
        //void sqlite3CodeVerifyNamedSchema(Parse*, string zDb);
        //void sqlite3BeginTransaction(Parse*, int);
        //void build.sqlite3CommitTransaction(Parse);
        //void sqlite3RollbackTransaction(Parse);
        //void build.sqlite3Savepoint(Parse*, int, Token);
        //void sqlite3CloseSavepoints(sqlite3 );
        //int sqlite3ExprIsConstant(Expr);
        //int sqlite3ExprIsConstantNotJoin(Expr);
        //int sqlite3ExprIsConstantOrFunction(Expr);
        //int sqlite3ExprIsInteger(Expr*, int);
        //int exprc.sqlite3ExprCanBeNull(const Expr);
        //void exprc.sqlite3ExprCodeIsNullJump(Vdbe*, const Expr*, int, int);
        //int exprc.sqlite3ExprNeedsNoAffinityChange(const Expr*, char);
        //int exprc.sqlite3IsRowid(const char);
        //void sqlite3GenerateRowDelete(Parse*, Table*, int, int, int, Trigger *, int);
        //void sqlite3GenerateRowIndexDelete(Parse*, Table*, int, int);
        //int sqlite3GenerateIndexKey(Parse*, Index*, int, int, int);
        //void sqlite3GenerateConstraintChecks(Parse*,Table*,int,int,
        //                                     int*,int,int,int,int,int);
        //void sqlite3CompleteInsertion(Parse*, Table*, int, int, int*, int, int, int);
        //int sqlite3OpenTableAndIndices(Parse*, Table*, int, int);
        //void sqlite3BeginWriteOperation(Parse*, int, int);
        //void build.sqlite3MultiWrite(Parse);
        //void build.sqlite3MayAbort(Parse );
        //void build.sqlite3HaltConstraint(Parse*, int, char*, int);
        //Expr *exprc.sqlite3ExprDup(sqlite3*,Expr*,int);
        //ExprList *exprc.sqlite3ExprListDup(sqlite3*,ExprList*,int);
        //SrcList *exprc.sqlite3SrcListDup(sqlite3*,SrcList*,int);
        //IdList *exprc.sqlite3IdListDup(sqlite3*,IdList);
        //Select *exprc.sqlite3SelectDup(sqlite3*,Select*,int);
        //void sqlite3FuncDefInsert(FuncDefHash*, FuncDef);
        //FuncDef *sqlite3FindFunction(sqlite3*,const char*,int,int,u8,int);
        //void sqlite3RegisterBuiltinFunctions(sqlite3);
        //void sqlite3RegisterDateTimeFunctions(void);
        //void sqlite3RegisterGlobalFunctions(void);
        //int sqlite3SafetyCheckOk(sqlite3);
        //int utilc.sqlite3SafetyCheckSickOrOk(sqlite3);
        //void sqlite3ChangeCookie(Parse*, int);
#if !(SQLITE_OMIT_VIEW) && !(SQLITE_OMIT_TRIGGER)
        //void sqlite3MaterializeView(Parse*, Table*, Expr*, int);
#endif
#if !SQLITE_OMIT_TRIGGER
        //void sqlite3BeginTrigger(Parse*, Token*,Token*,int,int,IdList*,SrcList*,
        //                         Expr*,int, int);
        //void sqlite3FinishTrigger(Parse*, TriggerStep*, Token);
        //void sqlite3DropTrigger(Parse*, SrcList*, int);
        //Trigger *sqlite3TriggersExist(Parse *, Table*, int, ExprList*, int *pMask);
        //Trigger *sqlite3TriggerList(Parse *, Table );
        //  void sqlite3CodeRowTrigger(Parse*, Trigger *, int, ExprList*, int, Table *,
        //                        int, int, int);
        //void sqliteViewTriggers(Parse*, Table*, Expr*, int, ExprList);
        //void sqlite3DeleteTriggerStep(sqlite3*, TriggerStep);
        //TriggerStep *sqlite3TriggerSelectStep(sqlite3*,Select);
        //TriggerStep *sqlite3TriggerInsertStep(sqlite3*,Token*, IdList*,
        //                                      ExprList*,Select*,u8);
        //TriggerStep *sqlite3TriggerUpdateStep(sqlite3*,Token*,ExprList*, Expr*, u8);
        //TriggerStep *sqlite3TriggerDeleteStep(sqlite3*,Token*, Expr);
        //void sqlite3DeleteTrigger(sqlite3*, Trigger);
        //void sqlite3UnlinkAndDeleteTrigger(sqlite3*,int,const char);
        //  u32  sqlite3TriggerColmask(Parse*,Trigger*,ExprList*,int,int,Table*,int);
        //# define sqliteinth.sqlite3ParseToplevel(p) ((p)->pToplevel ? (p)->pToplevel : (p))
        public static Parse sqlite3ParseToplevel(Parse p)
        {
            return p.pToplevel != null ? p.pToplevel : p;
        }
#else
																																																												    static void sqlite3BeginTrigger( Parse A, Token B, Token C, int D, int E, IdList F, SrcList G, Expr H, int I, int J )
    {
    }
    static void sqlite3FinishTrigger( Parse P, TriggerStep TS, Token T )
    {
    }
    static TriggerStep sqlite3TriggerSelectStep( sqlite3 A, Select B )
    {
      return null;
    }
    static TriggerStep sqlite3TriggerInsertStep( sqlite3 A, Token B, IdList C, ExprList D, Select E, u8 F )
    {
      return null;
    }
    static TriggerStep sqlite3TriggerInsertStep( sqlite3 A, Token B, IdList C, int D, Select E, u8 F )
    {
      return null;
    }
    static TriggerStep sqlite3TriggerInsertStep( sqlite3 A, Token B, IdList C, ExprList D, int E, u8 F )
    {
      return null;
    }
    static TriggerStep sqlite3TriggerUpdateStep( sqlite3 A, Token B, ExprList C, Expr D, u8 E )
    {
      return null;
    }
    static TriggerStep sqlite3TriggerDeleteStep( sqlite3 A, Token B, Expr C )
    {
      return null;
    }
    static u32 sqlite3TriggerColmask( Parse A, Trigger B, ExprList C, int D, int E, Table F, int G )
    {
      return 0;
    }

    // define sqlite3TriggersExist(B,C,D,E,F) 0
    static Trigger sqlite3TriggersExist( Parse B, Table C, int D, ExprList E, ref int F )
    {
      return null;
    }

    // define sqlite3DeleteTrigger(A,B)
    static void sqlite3DeleteTrigger( sqlite3 A, ref Trigger B )
    {
    }
    static void sqlite3DeleteTriggerStep( sqlite3 A, ref TriggerStep B )
    {
    }

    // define sqlite3DropTriggerPtr(A,B)
    static void sqlite3DropTriggerPtr( Parse A, Trigger B )
    {
    }
    static void sqlite3DropTrigger( Parse A, SrcList B, int C )
    {
    }

    // define sqlite3UnlinkAndDeleteTrigger(A,B,C)
    static void sqlite3UnlinkAndDeleteTrigger( sqlite3 A, int B, string C )
    {
    }

    // define sqlite3CodeRowTrigger(A,B,C,D,E,F,G,H,I)
    static void sqlite3CodeRowTrigger( Parse A, Trigger B, int C, ExprList D, int E, Table F, int G, int H, int I )
    {
    }

    // define sqlite3CodeRowTriggerDirect(A,B,C,D,E,F)
    static Trigger sqlite3TriggerList( Parse pParse, Table pTab )
    {
      return null;
    } // define sqlite3TriggerList(X, Y) 0

    // define sqliteinth.sqlite3ParseToplevel(p) p
    static Parse sqliteinth.sqlite3ParseToplevel( Parse p )
    {
      return p;
    }

    // define sqlite3TriggerOldmask(A,B,C,D,E,F) 0
    static u32 sqlite3TriggerOldmask( Parse A, Trigger B, int C, ExprList D, Table E, int F )
    {
      return 0;
    }
#endif
        //int sqlite3JoinType(Parse*, Token*, Token*, Token);
        //void build.sqlite3CreateForeignKey(Parse*, ExprList*, Token*, ExprList*, int);
        //void sqlite3DeferForeignKey(Parse*, int);
#if !SQLITE_OMIT_AUTHORIZATION
																																																												void sqlite3AuthRead(Parse*,Expr*,Schema*,SrcList);
int sqlite3AuthCheck(Parse*,int, const char*, const char*, const char);
void sqlite3AuthContextPush(Parse*, AuthContext*, const char);
void sqlite3AuthContextPop(AuthContext);
int sqlite3AuthReadCol(Parse*, string , string , int);
#else
        //# define sqlite3AuthRead(a,b,c,d)
        public static void sqlite3AuthRead(Parse a, Expr b, Schema c, SrcList d)
        {
        }
        //# define sqlite3AuthCheck(a,b,c,d,e)    SqlResult.SQLITE_OK
        public static SqlResult sqlite3AuthCheck(Parse a, int b, string c, byte[] d, byte[] e)
        {
            return SqlResult.SQLITE_OK;
        }
        //# define sqlite3AuthContextPush(a,b,c)
        public static void sqlite3AuthContextPush(Parse a, AuthContext b, string c)
        {
        }
        //# define sqlite3AuthContextPop(a)  ((void)(a))
        public static Parse sqlite3AuthContextPop(Parse a)
        {
            return a;
        }
#endif
        //void sqlite3Attach(Parse*, Expr*, Expr*, Expr);
        //void sqlite3Detach(Parse*, Expr);
        //int sqlite3FixInit(DbFixer*, Parse*, int, const char*, const Token);
        //int sqlite3FixSrcList(DbFixer*, SrcList);
        //int sqlite3FixSelect(DbFixer*, Select);
        //int sqlite3FixExpr(DbFixer*, Expr);
        //int sqlite3FixExprList(DbFixer*, ExprList);
        //int sqlite3FixTriggerStep(DbFixer*, TriggerStep);
        //Converter.sqlite3AtoF(string z, double*, int, u8)
        //int sqlite3GetInt32(string , int);
        //int refaactorrConverter__sqlite3Atoi(string );
        //int sqlite3Utf16ByteLen(const void pData, int nChar);
        //int sqlite3Utf8CharLen(const char pData, int nByte);
        //u32 sqlite3Utf8Read(const u8*, const u8*);
        ///
        ///<summary>
        ///</summary>
        ///<param name="Routines to read and write variable">length integers.  These used to</param>
        ///<param name="be defined locally, but now we use the varint routines in the util.c">be defined locally, but now we use the varint routines in the util.c</param>
        ///<param name="file.  Code should use the MACRO forms below, as the Varint32 versions">file.  Code should use the MACRO forms below, as the Varint32 versions</param>
        ///<param name="are coded to assume the single byte case is already handled (which">are coded to assume the single byte case is already handled (which</param>
        ///<param name="the MACRO form does).">the MACRO form does).</param>
        ///<param name=""></param>
        //int sqlite3PutVarint(unsigned char*, u64);
        //int putVarint32(unsigned char*, u32);
        //u8 sqlite3GetVarint(const unsigned char *, u64 );
        //u8 sqlite3GetVarint32(const unsigned char *, u32 );
        //int utilc.sqlite3VarintLen(u64 v);
        ///<summary>
        /// The header of a record consists of a sequence variable-length integers.
        /// These integers are almost always small and are encoded as a single byte.
        /// The following macros take advantage this fact to provide a fast encode
        /// and decode of the integers in a record header.  It is faster for the common
        /// case where the integer is a single byte.  It is a little slower when the
        /// integer is two or more bytes.  But overall it is faster.
        ///
        /// The following expressions are equivalent:
        ///
        ///     x = sqlite3GetVarint32( A, B );
        ///     x = putVarint32( A, B );
        ///
        ///     x = utilc.getVarint32( A, B );
        ///     x = putVarint32( A, B );
        ///
        ///
        ///</summary>
        //#define utilc.getVarint32(A,B)  (u8)((*(A)<(u8)0x80) ? ((B) = (u32)*(A)),1 : sqlite3GetVarint32((A), (u32 )&(B)))
        //#define putVarint32(A,B)  (u8)(((u32)(B)<(u32)0x80) ? (*(A) = (unsigned char)(B)),1 : sqlite3PutVarint32((A), (B)))
        //#define getVarint    sqlite3GetVarint
        //#define putVarint    sqlite3PutVarint
        //string sqlite3IndexAffinityStr(Vdbe *, Index );
        //void sqlite3TableAffinityStr(Vdbe *, Table );
        //char sqlite3CompareAffinity(Expr pExpr, char aff2);
        //int sqlite3IndexAffinityOk(Expr pExpr, char idx_affinity);
        //char exprc.sqlite3ExprAffinity(Expr pExpr);
        //int Converter.sqlite3Atoi64(const char*, i64*, int, u8);
        //void utilc.sqlite3Error(sqlite3*, int, const char*,...);
        //void *Converter.sqlite3HexToBlob(sqlite3*, string z, int n);
        //u8 Converter.sqlite3HexToInt(int h);
        //int build.sqlite3TwoPartName(Parse *, Token *, Token *, Token *);
        //string sqlite3ErrStr(int);
        //int sqlite3ReadSchema(Parse pParse);
        //CollSeq *sqlite3FindCollSeq(sqlite3*,u8 enc, const char*,int);
        //CollSeq *build.sqlite3LocateCollSeq(Parse *pParse, const char*zName);
        //CollSeq *exprc.sqlite3ExprCollSeq(Parse pParse, Expr pExpr);
        //Expr *exprc.sqlite3ExprSetColl(Expr*, CollSeq);
        //Expr *exprc.sqlite3ExprSetCollByToken(Parse *pParse, Expr*, Token);
        //int sqlite3CheckCollSeq(Parse *, CollSeq );
        //int sqlite3CheckObjectName(Parse *, string );
        //void sqlite3VdbeSetChanges(sqlite3 *, int);
        //int sqlite3AddInt64(i64*,i64);
        //int utilc.sqlite3SubInt64(i64*,i64);
        //int sqlite3MulInt64(i64*,i64);
        //int sqlite3AbsInt32(int);
#if SQLITE_ENABLE_8_3_NAMES
																																																												    //void sqlite3FileSuffix3(const char*, char);
#else
        //# define sqlite3FileSuffix3(X,Y)
        public static void sqlite3FileSuffix3(string X, string Y)
        {
        }
#endif
        //u8 sqlite3GetBoolean(string z);
        //const void *sqlite3ValueText(sqlite3_value*, u8);
        //int sqlite3ValueBytes(sqlite3_value*, u8);
        //void sqlite3ValueSetStr(sqlite3_value*, int, const void *,u8,
        //                      //  void()(void));
        //void sqlite3ValueFree(sqlite3_value);
        //sqlite3_value *sqlite3ValueNew(sqlite3 );
        //char *sqlite3Utf16to8(sqlite3 *, const void*, int, u8);
        //#if SQLITE_ENABLE_STAT2
        //char *sqlite3Utf8to16(sqlite3 *, u8, char *, int, int );
        //#endif
        //int sqlite3ValueFromExpr(sqlite3 *, Expr *, u8, u8, sqlite3_value *);
        //void sqlite3ValueApplyAffinity(sqlite3_value *, u8, u8);
        //#if !SQLITE_AMALGAMATION
        //extern const unsigned char sqlite3OpcodeProperty[];
        //extern const unsigned char _Custom.sqlite3UpperToLower[];
        //extern const unsigned char sqlite3CtypeMap[];
        //extern const Token sqlite3IntTokens[];
        //extern SQLITE_WSD struct Sqlite3Config sqlite3Config;
        //extern SQLITE_WSD FuncDefHash sqlite3GlobalFunctions;
        //#if !SQLITE_OMIT_WSD
        //extern int sqlite3PendingByte;
        //#endif
        //#endif
        //void sqlite3RootPageMoved(sqlite3*, int, int, int);
        //void build.sqlite3Reindex(Parse*, Token*, Token);
        //void sqlite3AlterFunctions(void);
        //void sqlite3AlterRenameTable(Parse*, SrcList*, Token);
        //int sqlite3GetToken(const unsigned char *, int );
        //void build.sqlite3NestedParse(Parse*, const char*, ...);
        //void sqlite3ExpirePreparedStatements(sqlite3);
        //int sqlite3CodeSubselect(Parse *, Expr *, int, int);
        //void sqlite3SelectPrep(Parse*, Select*, NameContext);
        //int sqlite3ResolveExprNames(NameContext*, Expr);
        //void sqlite3ResolveSelectNames(Parse*, Select*, NameContext);
        //int sqlite3ResolveOrderGroupBy(Parse*, Select*, ExprList*, const char);
        //void sqlite3ColumnDefault(Vdbe *, Table *, int, int);
        //void sqlite3AlterFinishAddColumn(Parse *, Token );
        //void sqlite3AlterBeginAddColumn(Parse *, SrcList );
        //CollSeq *sqlite3GetCollSeq(sqlite3*, u8, CollSeq *, const char);
        //char sqlite3AffinityType(const char);
        //void sqlite3Analyze(Parse*, Token*, Token);
        //int sqlite3InvokeBusyHandler(BusyHandler);
        //int build.sqlite3FindDb(sqlite3*, Token);
        //int build.sqlite3FindDbName(sqlite3 *, string );
        //int sqlite3AnalysisLoad(sqlite3*,int iDB);
        //void sqlite3DeleteIndexSamples(sqlite3*,Index);
        //void sqlite3DefaultRowEst(Index);
        //void sqlite3RegisterLikeFunctions(sqlite3*, int);
        //int sqlite3IsLikeFunction(sqlite3*,Expr*,int*,char);
        //void sqlite3MinimumFileFormat(Parse*, int, int);
        //void sqlite3SchemaClear(void );
        //Schema *sqlite3SchemaGet(sqlite3 *, Btree );
        //int sqlite3SchemaToIndex(sqlite3 db, Schema );
        //KeyInfo *build.sqlite3IndexKeyinfo(Parse *, Index );
        //int sqlite3CreateFunc(sqlite3 *, string , int, int, object  *, 
        //  void ()(sqlite3_context*,int,sqlite3_value *),
        //  void ()(sqlite3_context*,int,sqlite3_value *), object  ()(sqlite3_context),
        //  FuncDestructor *pDestructor
        //);
        //int malloc_cs.sqlite3ApiExit(sqlite3 db, int);
        //int sqlite3OpenTempDatabase(Parse );
        //void sqlite3StrAccumAppend(StrAccum*,const char*,int);
        //char *sqlite3StrAccumFinish(StrAccum);
        //void sqlite3StrAccumReset(StrAccum);
        //void sqlite3SelectDestInit(SelectDest*,int,int);
        //Expr *sqlite3CreateColumnExpr(sqlite3 *, SrcList *, int, int);
        //void sqlite3BackupRestart(sqlite3_backup );
        //void sqlite3BackupUpdate(sqlite3_backup *, Pgno, const u8 );
        ///<summary>
        /// The interface to the LEMON-generated parser
        ///
        ///</summary>
        //void *sqlite3ParserAlloc(void*()(size_t));
        //void sqlite3ParserFree(void*, void()(void));
        //void sqlite3Parser(void*, int, Token, Parse);
#if YYTRACKMAXSTACKDEPTH
																																																												int sqlite3ParserStackPeak(void);
#endif
        //void sqlite3AutoLoadExtensions(sqlite3);
#if !SQLITE_OMIT_LOAD_EXTENSION
        //void sqlite3CloseExtensions(sqlite3);
#else
																																																												// define sqlite3CloseExtensions(X)
#endif
#if !SQLITE_OMIT_SHARED_CACHE
																																																												//void sqliteinth.sqlite3TableLock(Parse *, int, int, u8, string );
#else
        //#define sqliteinth.sqlite3TableLock(v,w,x,y,z)
        public static void sqlite3TableLock(Parse p, int p1, int p2, u8 p3, byte[] p4)
        {
        }
        public static void sqlite3TableLock(Parse p, int p1, int p2, u8 p3, string p4)
        {
        }
#endif
#if SQLITE_TEST
																																																												    ///int sqlite3Utf8To8(unsigned char);
#endif
#if SQLITE_OMIT_VIRTUALTABLE
																																																												    //  define sqlite3VtabClear(D, Y)
    static void sqlite3VtabClear( sqlite3 db, Table Y )
    {
    }

    //  define sqlite3VtabSync(X,Y) SqlResult.SQLITE_OK
    static int sqlite3VtabSync( sqlite3 X, ref string Y )
    {
      return SqlResult.SQLITE_OK;
    }

    //  define sqlite3VtabRollback(X)
    static void sqlite3VtabRollback( sqlite3 X )
    {
    }

    //  define sqlite3VtabCommit(X)
    static void sqlite3VtabCommit( sqlite3 X )
    {
    }

    //  define sqlite3VtabLock(X) 
    static void sqlite3VtabLock( VTable X )
    {
    }

    //  define sqlite3VtabUnlock(X)
    static void sqlite3VtabUnlock( VTable X )
    {
    }

    //  define sqlite3VtabUnlockList(X)
    static void sqlite3VtabUnlockList( sqlite3 X )
    {
    }
    //  define sqlite3VtabSavepoint(X, Y, Z) SqlResult.SQLITE_OK
    static int sqlite3VtabSavepoint( sqlite3 X, int Y, int Z )
    {
      return SqlResult.SQLITE_OK;
    }
    //  define sqlite3VtabInSync(db) ((db)->nVTrans>0 && (db)->aVTrans==0)
    static bool sqlite3VtabInSync( sqlite3 db )
    {
      return false;
    }

    //  define sqlite3VtabArgExtend(P, T)
    static void sqlite3VtabArgExtend( Parse P, Token T )
    {
    }

    //  define sqlite3VtabArgInit(P)
    static void sqlite3VtabArgInit( Parse P )
    {
    }

    //  define sqlite3VtabBeginParse(P, T, T1, T2);
    static void sqlite3VtabBeginParse( Parse P, Token T, Token T1, Token T2 )
    {
    }

    //  define sqlite3VtabFinishParse(P, T)
    static void sqlite3VtabFinishParse<T>( Parse P, T t )
    {
    }

    static VTable sqlite3GetVTable( sqlite3 db, Table T )
    {
      return null;
    }
#else
        //void sqlite3VtabClear(sqlite3 db, Table);
        //int sqlite3VtabSync(sqlite3 db, int rc);
        //int sqlite3VtabRollback(sqlite3 db);
        //int sqlite3VtabCommit(sqlite3 db);
        //void sqlite3VtabLock(VTable );
        //void sqlite3VtabUnlock(VTable );
        //void sqlite3VtabUnlockList(sqlite3);
        //int sqlite3VtabSavepoint(sqlite3 *, int, int);
        //#  define sqlite3VtabInSync(db) ((db)->nVTrans>0 && (db)->aVTrans==0)
        public static bool sqlite3VtabInSync(sqlite3 db)
        {
            return (db.nVTrans > 0 && db.aVTrans == null);
        }
#endif
        //void sqlite3VtabMakeWritable(Parse*,Table);
        //void sqlite3VtabBeginParse(Parse*, Token*, Token*, Token);
        //void sqlite3VtabFinishParse(Parse*, Token);
        //void sqlite3VtabArgInit(Parse);
        //void sqlite3VtabArgExtend(Parse*, Token);
        //int sqlite3VtabCallCreate(sqlite3*, int, string , char *);
        //int sqlite3VtabCallConnect(Parse*, Table);
        //int sqlite3VtabCallDestroy(sqlite3*, int, string );
        //int sqlite3VtabBegin(sqlite3 *, VTable );
        //FuncDef *sqlite3VtabOverloadFunction(sqlite3 *,FuncDef*, int nArg, Expr);
        //void sqlite3InvalidFunction(sqlite3_context*,int,sqlite3_value*);
        //int vdbeapi.sqlite3VdbeParameterIndex(Vdbe*, const char*, int);
        //int sqlite3TransferBindings(sqlite3_stmt *, sqlite3_stmt );
        //int sqlite3Reprepare(Vdbe);
        //void exprc.sqlite3ExprListCheckLength(Parse*, ExprList*, const char);
        //CollSeq *sqlite3BinaryCompareCollSeq(Parse *, Expr *, Expr );
        //int sqlite3TempInMemory(const sqlite3);
        //VTable *sqlite3GetVTable(sqlite3*, Table);
        //string sqlite3JournalModename(int);
        //int sqlite3Checkpoint(sqlite3*, int, int, int*, int);
        //int sqlite3WalDefaultHook(void*,sqlite3*,const char*,int);
        ///
        ///<summary>
        ///Declarations for functions in fkey.c. All of these are replaced by
        ///</summary>
        ///<param name="no">op macros if OMIT_FOREIGN_KEY is defined. In this case no foreign</param>
        ///<param name="key functionality is available. If OMIT_TRIGGER is defined but">key functionality is available. If OMIT_TRIGGER is defined but</param>
        ///<param name="OMIT_FOREIGN_KEY is not, only some of the functions are no">oped. In</param>
        ///<param name="this case foreign keys are parsed, but no other functionality is ">this case foreign keys are parsed, but no other functionality is </param>
        ///<param name="provided (enforcement of FK constraints requires the triggers sub">system).</param>
        ///<param name=""></param>
#if !(SQLITE_OMIT_FOREIGN_KEY) && !(SQLITE_OMIT_TRIGGER)
        //void sqlite3FkCheck(Parse*, Table*, int, int);
        //void sqlite3FkDropTable(Parse*, SrcList *, Table);
        //void sqlite3FkActions(Parse*, Table*, ExprList*, int);
        //int sqlite3FkRequired(Parse*, Table*, int*, int);
        //u32 sqlite3FkOldmask(Parse*, Table);
        //FKey *sqlite3FkReferences(vtable );
#else
																																																												//define sqlite3FkActions(a,b,c,d)
static void sqlite3FkActions( Parse a, Table b, ExprList c, int d ) { }

//define sqlite3FkCheck(a,b,c,d)
static void sqlite3FkCheck( Parse a, Table b, int c, int d ) { }

//define sqlite3FkDropTable(a,b,c)
static void sqlite3FkDropTable( Parse a, SrcList b, Table c ) { }

//define sqlite3FkOldmask(a,b)      0
static u32 sqlite3FkOldmask( Parse a, Table b ) { return 0; }

//define sqlite3FkRequired(a,b,c,d) 0
static int sqlite3FkRequired( Parse a, Table b, int[] c, int d ) { return 0; }
#endif
#if !SQLITE_OMIT_FOREIGN_KEY
        //void sqlite3FkDelete(sqlite3 *, Table);
#else
																																																												//define sqlite3FkDelete(a, b)
static void sqlite3FkDelete(sqlite3 a, Table b) {}                 
#endif
        ///
        ///<summary>
        ///Available fault injectors.  Should be numbered beginning with 0.
        ///</summary>
        private const int SQLITE_FAULTINJECTOR_MALLOC = 0;
        //#define SQLITE_FAULTINJECTOR_MALLOC     0
        private const int SQLITE_FAULTINJECTOR_COUNT = 1;
        //#define SQLITE_FAULTINJECTOR_COUNT      1
        ///<summary>
        /// The interface to the code in fault.c used for identifying "benign"
        /// malloc failures. This is only present if SQLITE_OMIT_BUILTIN_TEST
        /// is not defined.
        ///
        ///</summary>
#if !SQLITE_OMIT_BUILTIN_TEST
        //void sqlite3BeginBenignMalloc(void);
        //void sqlite3EndBenignMalloc(void);
#else
																																																												//define sqlite3BeginBenignMalloc()
//define sqlite3EndBenignMalloc()
#endif
        public const int IN_INDEX_ROWID = 1;
        //#define IN_INDEX_ROWID           1
        public const int IN_INDEX_EPH = 2;
        //#define sqliteinth.IN_INDEX_EPH             2
        public const int IN_INDEX_INDEX = 3;
        //#define IN_INDEX_INDEX           3
        //int sqlite3FindInIndex(Parse *, Expr *, int);

        //void sqlite3MemJournalOpen(sqlite3_file );
        //int sqlite3MemJournalSize(void);
        //int sqlite3IsMemJournal(sqlite3_file );
#if SQLITE_MAX_EXPR_DEPTH
        //  void exprc.sqlite3ExprSetHeight(Parse pParse, Expr p);
        //  int sqlite3SelectExprHeight(Select );
        //int exprc.sqlite3ExprCheckHeight(Parse*, int);
#else
																																																												//define exprc.sqlite3ExprSetHeight(x,y)
//define sqlite3SelectExprHeight(x) 0
//define exprc.sqlite3ExprCheckHeight(x,y)
#endif
        //u32 sqlite3Get4byte(const u8);
        //void sqlite3sqlite3Put4byte(u8*, u32);
#if SQLITE_ENABLE_UNLOCK_NOTIFY
																																																												void sqlite3ConnectionBlocked(sqlite3 *, sqlite3 );
void sqlite3ConnectionUnlocked(sqlite3 db);
void sqlite3ConnectionClosed(sqlite3 db);
#else
        public static void sqlite3ConnectionBlocked(sqlite3 x, sqlite3 y)
        {
        }
        //#define sqlite3ConnectionBlocked(x,y)
        public static void sqlite3ConnectionUnlocked(sqlite3 x)
        {
        }
        //#define sqlite3ConnectionUnlocked(x)
        public static void sqlite3ConnectionClosed(sqlite3 x)
        {
        }
        //#define sqlite3ConnectionClosed(x)
#endif
#if SQLITE_DEBUG
																																																												    //  void sqlite3ParserTrace(FILE*, char );
#endif
        ///<summary>
        /// If the SQLITE_ENABLE sqliteinth.IOTRACE exists then the global variable
        /// sqlite3IoTrace is a pointer to a printf-like routine used to
        /// print I/O tracing messages.
        ///</summary>
#if SQLITE_ENABLE_IOTRACE
																																																												static bool SQLite3IoTrace = false;
//define sqliteinth.IOTRACE(A)  if( sqlite3IoTrace ){ sqlite3IoTrace A; }
static void sqliteinth.IOTRACE( string X, params object[] ap ) { if ( SQLite3IoTrace ) { printf( X, ap ); } }

//  void sqlite3VdbeIOTraceSql(Vdbe);
//SQLITE_EXTERN void (*sqlite3IoTrace)(const char*,...);
#else
        //#define sqliteinth.IOTRACE(A)
        public static void IOTRACE(string F, params object[] ap)
        {
        }
        //#define sqlite3VdbeIOTraceSql(X)
        public static void sqlite3VdbeIOTraceSql(Vdbe X)
        {
        }
#endif
        ///<summary>
        /// These routines are available for the mem2.c debugging memory allocator
        /// only.  They are used to verify that different "types" of memory
        /// allocations are properly tracked by the system.
        ///
        /// sqlite3MemdebugSetType() sets the "type" of an allocation to one of
        /// the MEMTYPE_* macros defined below.  The type must be a bitmask with
        /// a single bit set.
        ///
        /// sqlite3MemdebugHasType() returns true if any of the bits in its second
        /// argument match the type set by the previous sqlite3MemdebugSetType().
        /// sqlite3MemdebugHasType() is intended for use inside Debug.Assert() statements.
        ///
        /// sqliteinth.sqlite3MemdebugNoType() returns true if none of the bits in its second
        /// argument match the type set by the previous sqlite3MemdebugSetType().
        ///
        /// Perhaps the most important point is the difference between MEMTYPE_HEAP
        /// and MEMTYPE_LOOKASIDE.  If an allocation is MEMTYPE_LOOKASIDE, that means
        /// it might have been allocated by lookaside, except the allocation was
        /// too large or lookaside was already full.  It is important to verify
        /// that allocations that might have been satisfied by lookaside are not
        /// passed back to non-lookaside free() routines.  Asserts such as the
        /// example above are placed on the non-lookaside free() routines to verify
        /// this constraint.
        ///
        /// All of this is no-op for a production build.  It only comes into
        /// play when the SQLITE_MEMDEBUG compile-time option is used.
        ///</summary>
#if SQLITE_MEMDEBUG
																																																												//  void sqlite3MemdebugSetType(void*,u8);
//  int sqlite3MemdebugHasType(void*,u8);
//  int sqliteinth.sqlite3MemdebugNoType(void*,u8);
#else
        //# define sqlite3MemdebugSetType(X,Y)  /* no-op */
        public static void sqlite3MemdebugSetType<T>(T X, MemType Y)
        {
        }
        //# define sqlite3MemdebugHasType(X,Y)  1
        public static bool sqlite3MemdebugHasType<T>(T X, MemType Y)
        {
            return true;
        }
        //# define sqliteinth.sqlite3MemdebugNoType(X,Y)   1
        public static bool sqlite3MemdebugNoType<T>(T X, MemType Y)
        {
            return true;
        }
#endif
        //#endif //* _SQLITEINT_H_ */
    }

}