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
/*
** The yDbMask datatype for the bitmask of all attached databases.
*/
#if SQLITE_MAX_ATTACHED
//  typedef sqlite3_uint64 yDbMask;
using yDbMask = System.Int64; 
#else
//  typedef unsigned int yDbMask;
using yDbMask=System.Int32;
#endif
namespace Community.CsharpSqlite {
	using sqlite3_value=Sqlite3.Mem;



    public enum TokenType : byte
    {
        TK_SEMI = 1,
        TK_EXPLAIN = 2,
        TK_QUERY = 3,
        TK_PLAN = 4,
        TK_BEGIN = 5,
        TK_TRANSACTION = 6,
        TK_DEFERRED = 7,
        TK_IMMEDIATE = 8,
        TK_EXCLUSIVE = 9,
        TK_COMMIT = 10,
        TK_END = 11,
        TK_ROLLBACK = 12,
        TK_SAVEPOINT = 13,
        TK_RELEASE = 14,
        TK_TO = 15,
        TK_TABLE = 16,
        TK_CREATE = 17,
        TK_IF = 18,
        TK_NOT = 19,
        TK_EXISTS = 20,
        TK_TEMP = 21,
        TK_LP = 22,
        TK_RP = 23,
        TK_AS = 24,
        TK_COMMA = 25,
        TK_ID = 26,
        TK_INDEXED = 27,
        TK_ABORT = 28,
        TK_ACTION = 29,
        TK_AFTER = 30,
        TK_ANALYZE = 31,
        TK_ASC = 32,
        TK_ATTACH = 33,
        TK_BEFORE = 34,
        TK_BY = 35,
        TK_CASCADE = 36,
        TK_CAST = 37,
        TK_COLUMNKW = 38,
        TK_CONFLICT = 39,
        TK_DATABASE = 40,
        TK_DESC = 41,
        TK_DETACH = 42,
        TK_EACH = 43,
        TK_FAIL = 44,
        TK_FOR = 45,
        TK_IGNORE = 46,
        TK_INITIALLY = 47,
        TK_INSTEAD = 48,
        TK_LIKE_KW = 49,
        TK_MATCH = 50,
        TK_NO = 51,
        TK_KEY = 52,
        TK_OF = 53,
        TK_OFFSET = 54,
        TK_PRAGMA = 55,
        TK_RAISE = 56,
        TK_REPLACE = 57,
        TK_RESTRICT = 58,
        TK_ROW = 59,
        TK_TRIGGER = 60,
        TK_VACUUM = 61,
        TK_VIEW = 62,
        TK_VIRTUAL = 63,
        TK_REINDEX = 64,
        TK_RENAME = 65,
        TK_CTIME_KW = 66,
        TK_ANY = 67,
        TK_OR = 68,
        TK_AND = 69,
        TK_IS = 70,
        TK_BETWEEN = 71,
        TK_IN = 72,
        TK_ISNULL = 73,
        TK_NOTNULL = 74,
        TK_NE = 75,
        TK_EQ = 76,
        TK_GT = 77,
        TK_LE = 78,
        TK_LT = 79,
        TK_GE = 80,
        TK_ESCAPE = 81,
        TK_BITAND = 82,
        TK_BITOR = 83,
        TK_LSHIFT = 84,
        TK_RSHIFT = 85,
        TK_PLUS = 86,
        TK_MINUS = 87,
        TK_STAR = 88,
        TK_SLASH = 89,
        TK_REM = 90,
        TK_CONCAT = 91,
        TK_COLLATE = 92,
        TK_BITNOT = 93,
        TK_STRING = 94,
        TK_JOIN_KW = 95,
        TK_CONSTRAINT = 96,
        TK_DEFAULT = 97,
        TK_NULL = 98,
        TK_PRIMARY = 99,
        TK_UNIQUE = 100,
        TK_CHECK = 101,
        TK_REFERENCES = 102,
        TK_AUTOINCR = 103,
        TK_ON = 104,
        TK_INSERT = 105,
        TK_DELETE = 106,
        TK_UPDATE = 107,
        TK_SET = 108,
        TK_DEFERRABLE = 109,
        TK_FOREIGN = 110,
        TK_DROP = 111,
        TK_UNION = 112,
        TK_ALL = 113,
        TK_EXCEPT = 114,
        TK_INTERSECT = 115,
        TK_SELECT = 116,
        TK_DISTINCT = 117,
        TK_DOT = 118,
        TK_FROM = 119,
        TK_JOIN = 120,
        TK_USING = 121,
        TK_ORDER = 122,
        TK_GROUP = 123,
        TK_HAVING = 124,
        TK_LIMIT = 125,
        TK_WHERE = 126,
        TK_INTO = 127,
        TK_VALUES = 128,
        TK_INTEGER = 129,
        TK_FLOAT = 130,
        TK_BLOB = 131,
        TK_REGISTER = 132,
        TK_VARIABLE = 133,
        TK_CASE = 134,
        TK_WHEN = 135,
        TK_THEN = 136,
        TK_ELSE = 137,
        TK_INDEX = 138,
        TK_ALTER = 139,
        TK_ADD = 140,
        TK_TO_TEXT = 141,
        TK_TO_BLOB = 142,
        TK_TO_NUMERIC = 143,
        TK_TO_INT = 144,
        TK_TO_REAL = 145,
        TK_ISNOT = 146,
        TK_END_OF_FILE = 147,
        TK_ILLEGAL = 148,
        TK_SPACE = 149,
        TK_UNCLOSED_STRING = 150,
        TK_FUNCTION = 151,
        TK_COLUMN = 152,
        TK_AGG_FUNCTION = 153,
        TK_AGG_COLUMN = 154,
        TK_CONST_FUNC = 155,
        TK_UMINUS = 156,
        TK_UPLUS = 157
    }




	public partial class Sqlite3 {
		/*
    ** 2001 September 15
    **
    ** The author disclaims copyright to this source code.  In place of
    ** a legal notice, here is a blessing:
    **
    **    May you do good and not evil.
    **    May you find forgiveness for yourself and forgive others.
    **    May you share freely, never taking more than you give.
    **
    *************************************************************************
    ** Internal interface definitions for SQLite.
    **
    *************************************************************************
    **  Included in SQLite3 port to C#-SQLite;  2008 Noah B Hart
    **  C#-SQLite is an independent reimplementation of the SQLite software library
    **
    **  SQLITE_SOURCE_ID: 2011-06-23 19:49:22 4374b7e83ea0a3fbc3691f9c0c936272862f32f2
    **
    *************************************************************************
    *///#if !_SQLITEINT_H_
		//#define _SQLITEINT_H_
		/*
    ** These #defines should enable >2GB file support on POSIX if the
    ** underlying operating system supports it.  If the OS lacks
    ** large file support, or if the OS is windows, these should be no-ops.
    **
    ** Ticket #2739:  The _LARGEFILE_SOURCE macro must appear before any
    ** system #includes.  Hence, this block of code must be the very first
    ** code in all source files.
    **
    ** Large file support can be disabled using the -DSQLITE_DISABLE_LFS switch
    ** on the compiler command line.  This is necessary if you are compiling
    ** on a recent machine (ex: Red Hat 7.2) but you want your code to work
    ** on an older machine (ex: Red Hat 6.0).  If you compile on Red Hat 7.2
    ** without this option, LFS is enable.  But LFS does not exist in the kernel
    ** in Red Hat 6.0, so the code won't work.  Hence, for maximum binary
    ** portability you should omit LFS.
    **
    ** Similar is true for Mac OS X.  LFS is only supported on Mac OS X 9 and later.
    *///#if !SQLITE_DISABLE_LFS
		//# define _LARGE_FILE       1
		//# ifndef _FILE_OFFSET_BITS
		//#   define _FILE_OFFSET_BITS 64
		//# endif
		//# define _LARGEFILE_SOURCE 1
		//#endif
		/*
    ** Include the configuration header output by 'configure' if we're using the
    ** autoconf-based build
    */
		#if _HAVE_SQLITE_CONFIG_H
																																																								//include "config.h"
#endif
		//#include "sqliteLimit.h"
		/* Disable nuisance warnings on Borland compilers *///#if (__BORLANDC__)
		//#pragma warn -rch /* unreachable code */
		//#pragma warn -ccc /* Condition is always true or false */
		//#pragma warn -aus /* Assigned value is never used */
		//#pragma warn -csu /* Comparing signed and unsigned */
		//#pragma warn -spa /* Suspicious pointer arithmetic */
		//#endif
		/* Needed for various definitions... *///#if !_GNU_SOURCE
		//#define _GNU_SOURCE
		//#endif
		/*
    ** Include standard header files as necessary
    */
		#if HAVE_STDINT_H
																																																								//include <stdint.h>
#endif
		#if HAVE_INTTYPES_H
																																																								//include <inttypes.h>
#endif
		/*
** The number of samples of an index that SQLite takes in order to 
** construct a histogram of the table content when running ANALYZE
** and with SQLITE_ENABLE_STAT2
*///#define SQLITE_INDEX_SAMPLES 10
		public const int SQLITE_INDEX_SAMPLES=10;
		/*
    ** The following macros are used to cast pointers to integers and
    ** integers to pointers.  The way you do this varies from one compiler
    ** to the next, so we have developed the following set of #if statements
    ** to generate appropriate macros for a wide range of compilers.
    **
    ** The correct "ANSI" way to do this is to use the intptr_t type. 
    ** Unfortunately, that typedef is not available on all compilers, or
    ** if it is available, it requires an #include of specific headers
    ** that vary from one machine to the next.
    **
    ** Ticket #3860:  The llvm-gcc-4.2 compiler from Apple chokes on
    ** the ((void)&((char)0)[X]) construct.  But MSVC chokes on ((void)(X)).
    ** So we have to define the macros in different ways depending on the
    ** compiler.
    *///#if (__PTRDIFF_TYPE__)  /* This case should work for GCC */
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
		/*
    ** The SQLITE_THREADSAFE macro must be defined as 0, 1, or 2.
    ** 0 means mutexes are permanently disable and the library is never
    ** threadsafe.  1 means the library is serialized which is the highest
    ** level of threadsafety.  2 means the libary is multithreaded - multiple
    ** threads can use SQLite as long as no two threads try to use the same
    ** database connection at the same time.
    **
    ** Older versions of SQLite used an optional THREADSAFE macro.
    ** We support that for legacy.
    */
		#if !SQLITE_THREADSAFE
		//# define SQLITE_THREADSAFE 2
		private const int SQLITE_THREADSAFE=2;
		#else
																																																								    const int SQLITE_THREADSAFE = 2; /* IMP: R-07272-22309 */
#endif
		/*
** The SQLITE_DEFAULT_MEMSTATUS macro must be defined as either 0 or 1.
** It determines whether or not the features related to
** SQLITE_CONFIG_MEMSTATUS are available by default or not. This value can
** be overridden at runtime using the sqlite3_config() API.
*/
		#if !(SQLITE_DEFAULT_MEMSTATUS)
		//# define SQLITE_DEFAULT_MEMSTATUS 1
		private const int SQLITE_DEFAULT_MEMSTATUS=0;
		#else
																																																								const int SQLITE_DEFAULT_MEMSTATUS = 1;
#endif
		/*
** Exactly one of the following macros must be defined in order to
** specify which memory allocation subsystem to use.
**
**     SQLITE_SYSTEM_MALLOC          // Use normal system malloc()
**     SQLITE_MEMDEBUG               // Debugging version of system malloc()
**
** (Historical note:  There used to be several other options, but we've
** pared it down to just these two.)
**
** If none of the above are defined, then set SQLITE_SYSTEM_MALLOC as
** the default.
*///#if (SQLITE_SYSTEM_MALLOC)+defined(SQLITE_MEMDEBUG)+\
		//# error "At most one of the following compile-time configuration options\
		// is allows: SQLITE_SYSTEM_MALLOC, SQLITE_MEMDEBUG"
		//#endif
		//#if (SQLITE_SYSTEM_MALLOC)+defined(SQLITE_MEMDEBUG)+\
		//# define SQLITE_SYSTEM_MALLOC 1
		//#endif
		/*
    ** If SQLITE_MALLOC_SOFT_LIMIT is not zero, then try to keep the
    ** sizes of memory allocations below this value where possible.
    */
		#if !(SQLITE_MALLOC_SOFT_LIMIT)
		private const int SQLITE_MALLOC_SOFT_LIMIT=1024;
		#endif
		/*
** We need to define _XOPEN_SOURCE as follows in order to enable
** recursive mutexes on most Unix systems.  But Mac OS X is different.
** The _XOPEN_SOURCE define causes problems for Mac OS X we are told,
** so it is omitted there.  See ticket #2673.
**
** Later we learn that _XOPEN_SOURCE is poorly or incorrectly
** implemented on some systems.  So we avoid defining it at all
** if it is already defined or if it is unneeded because we are
** not doing a threadsafe build.  Ticket #2681.
**
** See also ticket #2741.
*/
		#if !_XOPEN_SOURCE && !__DARWIN__ && !__APPLE__ && SQLITE_THREADSAFE
																																																								    const int _XOPEN_SOURCE = 500;//define _XOPEN_SOURCE 500  /* Needed to enable pthread recursive mutexes */
#endif
		/*
** The TCL headers are only needed when compiling the TCL bindings.
*/
		#if SQLITE_TCL || TCLSH
																																																								    // include <tcl.h>
#endif
		/*
** Many people are failing to set -DNDEBUG=1 when compiling SQLite.
** Setting NDEBUG makes the code smaller and run faster.  So the following
** lines are added to automatically set NDEBUG unless the -DSQLITE_DEBUG=1
** option is set.  Thus NDEBUG becomes an opt-in rather than an opt-out
** feature.
*/
		#if !NDEBUG && !SQLITE_DEBUG
																																																								const int NDEBUG = 1;// define NDEBUG 1
#endif
		///<summary>
		/// The testcase() macro is used to aid in coverage testing.  When
		/// doing coverage testing, the condition inside the argument to
		/// testcase() must be evaluated both true and false in order to
		/// get full branch coverage.  The testcase() macro is inserted
		/// to help ensure adequate test coverage in places where simple
		/// condition/decision coverage is inadequate.  For example, testcase()
		/// can be used to make sure boundary values are tested.  For
		/// bitmask tests, testcase() can be used to make sure each bit
		/// is significant and used at least once.  On switch statements
		/// where multiple cases go to the same block of code, testcase()
		/// can insure that all cases are evaluated.
		///
		///</summary>
		#if SQLITE_COVERAGE_TEST
																																																								void sqlite3Coverage(int);
// define testcase(X)  if( X ){ sqlite3Coverage(__LINE__); }
#else
		//# define testcase(X)
		private static void testcase<T>(T X) {
		}
		#endif
		/*
** The TESTONLY macro is used to enclose variable declarations or
** other bits of code that are needed to support the arguments
** within testcase() and Debug.Assert() macros.
*/
		#if !NDEBUG || SQLITE_COVERAGE_TEST
																																																								    // define TESTONLY(X)  X
    // -- Need workaround for C, since inline macros don't exist
#else
		//# define TESTONLY(X)
		#endif
		/*
** Sometimes we need a small amount of code such as a variable initialization
** to setup for a later Debug.Assert() statement.  We do not want this code to
** appear when Debug.Assert() is disabled.  The following macro is therefore
** used to contain that setup code.  The "VVA" acronym stands for
** "Verification, Validation, and Accreditation".  In other words, the
** code within VVA_ONLY() will only run during verification processes.
*/
		#if !NDEBUG
																																																								    // define VVA_ONLY(X)  X
#else
		//# define VVA_ONLY(X)
		#endif
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
																																																								// define ALWAYS(X)      (1)
// define NEVER(X)       (0)
#elif !NDEBUG
																																																								    // define ALWAYS(X)      ((X)?1:(Debug.Assert(0),0))
    static bool ALWAYS( bool X )
    {
      if ( X != true )
        Debug.Assert( false );
      return true;
    }
    static int ALWAYS( int X )
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
		//# define ALWAYS(X)      (X)
		private static bool ALWAYS(bool X) {
			return X;
		}
		private static byte ALWAYS(byte X) {
			return X;
		}
		private static int ALWAYS(int X) {
			return X;
		}
		private static bool ALWAYS<T>(T X) {
			return true;
		}
		//# define NEVER(X)       (X)
		private static bool NEVER(bool X) {
			return X;
		}
		private static byte NEVER(byte X) {
			return X;
		}
		private static int NEVER(int X) {
			return X;
		}
		private static bool NEVER<T>(T X) {
			return false;
		}
		#endif
		/*
** Return true (non-zero) if the input is a integer that is too large
** to fit in 32-bits.  This macro is used inside of various testcase()
** macros to verify that we have tested SQLite for large-file support.
*/private static bool IS_BIG_INT(i64 X) {
			return (((X)&~(i64)0xffffffff)!=0);
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
		private static bool likely(bool X) {
			return !!X;
		}
		//# define unlikely(X)  !!(X)
		private static bool unlikely(bool X) {
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
		/*
    ** If compiling for a processor that lacks floating point support,
    ** substitute integer for floating-point
    */
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
		private const double SQLITE_BIG_DBL=(((sqlite3_int64)1)<<60);
		//# define SQLITE_BIG_DBL (1e99)
		#endif
		/*
** OMIT_TEMPDB is set to 1 if SQLITE_OMIT_TEMPDB is defined, or 0
** afterward. Having this macro allows us to cause the C compiler
** to omit code used by TEMP tables without messy #if !statements.
*/
		#if SQLITE_OMIT_TEMPDB
																																																								//define OMIT_TEMPDB 1
#else
		private static int OMIT_TEMPDB=0;
		#endif
		/*
** The "file format" number is an integer that is incremented whenever
** the VDBE-level file format changes.  The following macros define the
** the default file format for new databases and the maximum file format
** that the library can read.
*/public static int SQLITE_MAX_FILE_FORMAT=4;
		//#define SQLITE_MAX_FILE_FORMAT 4
		//#if !SQLITE_DEFAULT_FILE_FORMAT
		private static int SQLITE_DEFAULT_FILE_FORMAT=1;
		//# define SQLITE_DEFAULT_FILE_FORMAT 1
		//#endif
		/*
    ** Determine whether triggers are recursive by default.  This can be
    ** changed at run-time using a pragma.
    */
		#if !SQLITE_DEFAULT_RECURSIVE_TRIGGERS
		//# define SQLITE_DEFAULT_RECURSIVE_TRIGGERS 0
		public static bool SQLITE_DEFAULT_RECURSIVE_TRIGGERS=false;
		#else
																																																								static public bool SQLITE_DEFAULT_RECURSIVE_TRIGGERS = true;
#endif
		/*
** Provide a default value for SQLITE_TEMP_STORE in case it is not specified
** on the command-line
*///#if !SQLITE_TEMP_STORE
		private static int SQLITE_TEMP_STORE=1;
		//#define SQLITE_TEMP_STORE 1
		//#endif
		/*
** GCC does not define the offsetof() macro so we'll have to do it
** ourselves.
*/
		#if !offsetof
		//#define offsetof(STRUCTURE,FIELD) ((int)((char)&((STRUCTURE)0)->FIELD))
		#endif
		/*
** Check to see if this machine uses EBCDIC.  (Yes, believe it or
** not, there are still machines out there that use EBCDIC.)
*/
		#if FALSE
																																																								// define SQLITE_EBCDIC 1
#else
		private const int SQLITE_ASCII=1;
		//#define SQLITE_ASCII 1
		#endif
		/*
** Integers of known sizes.  These typedefs might change for architectures
** where the sizes very.  Preprocessor macros are available so that the
** types can be conveniently redefined at compile-type.  Like this:
**
**         cc '-Du32PTR_TYPE=long long int' ...
*///#if !u32_TYPE
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
		/*
    ** SQLITE_MAX_U32 is a u64 constant that is the maximum u64 value
    ** that can be stored in a u32 without loss of data.  The value
    ** is 0x00000000ffffffff.  But because of quirks of some compilers, we
    ** have to specify the value in the less intuitive manner shown:
    *///#define SQLITE_MAX_U32  ((((u64)1)<<32)-1)
		private const u32 SQLITE_MAX_U32=(u32)((((u64)1)<<32)-1);
		/*
    ** Macros to determine whether the machine is big or little endian,
    ** evaluated at runtime.
    */
		#if SQLITE_AMALGAMATION
																																																								//const int sqlite3one = 1;
#else
		private const bool sqlite3one=true;
		#endif
		#if i386 || __i386__ || _M_IX86
																																																								const int ;//define SQLITE_BIGENDIAN    0
const int ;//define SQLITE_LITTLEENDIAN 1
const int ;//define SQLITE_UTF16NATIVE  SqliteEncoding.UTF16LE
#else
		public static u8 SQLITE_BIGENDIAN=0;
		//#define SQLITE_BIGENDIAN    (*(char )(&sqlite3one)==0)
		public static u8 SQLITE_LITTLEENDIAN=1;
		//#define SQLITE_LITTLEENDIAN (*(char )(&sqlite3one)==1)
		//#define SQLITE_UTF16NATIVE (SQLITE_BIGENDIAN?SqliteEncoding.UTF16BE:SqliteEncoding.UTF16LE)
		#endif
		///<summary>
		/// Round up a number to the next larger multiple of 8.  This is used
		/// to force 8-byte alignment on 64-bit architectures.
		///
		///</summary>
		//#define ROUND8(x)     (((x)+7)&~7)
		private static int ROUND8(int x) {
			return (x+7)&~7;
		}
		///<summary>
		/// Round down to the nearest multiple of 8
		///
		///</summary>
		//#define ROUNDDOWN8(x) ((x)&~7)
		private static int ROUNDDOWN8(int x) {
			return x&~7;
		}
		/*
    ** Assert that the pointer X is aligned to an 8-byte boundary.  This
    ** macro is used only within Debug.Assert() to verify that the code gets
    ** all alignment restrictions correct.
    **
    ** Except, if SQLITE_4_BYTE_ALIGNED_MALLOC is defined, then the
    ** underlying malloc() implemention might return us 4-byte aligned
    ** pointers.  In that case, only verify 4-byte alignment.
    *///#if SQLITE_4_BYTE_ALIGNED_MALLOC
		//# define EIGHT_BYTE_ALIGNMENT(X)   ((((char)(X) - (char)0)&3)==0)
		//#else
		//# define EIGHT_BYTE_ALIGNMENT(X)   ((((char)(X) - (char)0)&7)==0)
		//#endif
		///<summary>
		/// An instance of the following structure is used to store the busy-handler
		/// callback for a given sqlite handle.
		///
		/// The sqlite.busyHandler member of the sqlite struct contains the busy
		/// callback for the database handle. Each pager opened via the sqlite
		/// handle is passed a pointer to sqlite.busyHandler. The busy-handler
		/// callback is currently invoked only from within pager.c.
		///
		///</summary>
		//typedef struct BusyHandler BusyHandler;
		public class BusyHandler {
			public dxBusy xFunc;
			//)(void *,int);  /* The busy callback */
			public object pArg;
			/* First arg to busy callback */public int nBusy;
		/* Incremented with each busy call */};

		/*
    ** Name of the master database table.  The master database table
    ** is a special table that holds the names and attributes of all
    ** user tables and indices.
    */private const string MASTER_NAME="sqlite_master";
		//#define MASTER_NAME       "sqlite_master"
		private const string TEMP_MASTER_NAME="sqlite_temp_master";
		//#define TEMP_MASTER_NAME  "sqlite_temp_master"
		///<summary>
		/// The root-page of the master database table.
		///
		///</summary>
		private const int MASTER_ROOT=1;
		//#define MASTER_ROOT       1
		/*
    ** The name of the schema table.
    */private static string SCHEMA_TABLE(int x)//#define SCHEMA_TABLE(x)  ((!OMIT_TEMPDB)&&(x==1)?TEMP_MASTER_NAME:MASTER_NAME)
		 {
			return ((OMIT_TEMPDB==0)&&(x==1)?TEMP_MASTER_NAME:MASTER_NAME);
		}
		///<summary>
		/// A convenience macro that returns the number of elements in
		/// an array.
		///
		///</summary>
		//#define ArraySize(X)    ((int)(sizeof(X)/sizeof(X[0])))
		private static int ArraySize<T>(T[] x) {
			return x.Length;
		}
		/*
    ** The following value as a destructor means to use sqlite3DbFree().
    ** This is an internal extension to SQLITE_STATIC and SQLITE_TRANSIENT.
    *///#define SQLITE_DYNAMIC   ((sqlite3_destructor_type)sqlite3DbFree)
		private static dxDel SQLITE_DYNAMIC;
		/*
    ** When SQLITE_OMIT_WSD is defined, it means that the target platform does
    ** not support Writable Static Data (WSD) such as global and static variables.
    ** All variables must either be on the stack or dynamically allocated from
    ** the heap.  When WSD is unsupported, the variable declarations scattered
    ** throughout the SQLite code must become constants instead.  The SQLITE_WSD
    ** macro is used for this purpose.  And instead of referencing the variable
    ** directly, we use its constant as a key to lookup the run-time allocated
    ** buffer that holds real variable.  The constant is also the initializer
    ** for the run-time allocated buffer.
    **
    ** In the usual case where WSD is supported, the SQLITE_WSD and GLOBAL
    ** macros become no-ops and have zero performance impact.
    */
		#if SQLITE_OMIT_WSD
																																																								//define SQLITE_WSD const
//define GLOBAL(t,v) (*(t)sqlite3_wsd_find((void)&(v), sizeof(v)))
//define sqlite3GlobalConfig GLOBAL(struct Sqlite3Config, sqlite3Config)
int sqlite3_wsd_init(int N, int J);
void *sqlite3_wsd_find(void *K, int L);
#else
		//#define SQLITE_WSD
		//#define GLOBAL(t,v) v
		//#define sqlite3GlobalConfig sqlite3Config
		private static Sqlite3Config sqlite3GlobalConfig;
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
		//#define UNUSED_PARAMETER(x) (void)(x)
		private static void UNUSED_PARAMETER<T>(T x) {
		}
		//#define UNUSED_PARAMETER2(x,y) UNUSED_PARAMETER(x),UNUSED_PARAMETER(y)
		private static void UNUSED_PARAMETER2<T1,T2>(T1 x,T2 y) {
			UNUSED_PARAMETER(x);
			UNUSED_PARAMETER(y);
		}
		/*
    ** Forward references to structures
    *///typedef struct AggInfo AggInfo;
		//typedef struct AuthContext AuthContext;
		//typedef struct AutoincInfo AutoincInfo;
		//typedef struct Bitvec Bitvec;
		//typedef struct CollSeq CollSeq;
		//typedef struct Column Column;
		//typedef struct Db Db;
		//typedef struct Schema Schema;
		//typedef struct Expr Expr;
		//typedef struct ExprList ExprList;
		//typedef struct ExprSpan ExprSpan;
		//typedef struct FKey FKey;
		//typedef struct FuncDestructor FuncDestructor;
		//typedef struct FuncDef FuncDef;
		//typedef struct IdList IdList;
		//typedef struct Index Index;
		//typedef struct IndexSample IndexSample;
		//typedef struct KeyClass KeyClass;
		//typedef struct KeyInfo KeyInfo;
		//typedef struct Lookaside Lookaside;
		//typedef struct LookasideSlot LookasideSlot;
		//typedef struct Module Module;
		//typedef struct NameContext NameContext;
		//typedef struct Parse Parse;
		//typedef struct RowSet RowSet;
		//typedef struct Savepoint Savepoint;
		//typedef struct Select Select;
		//typedef struct SrcList SrcList;
		//typedef struct StrAccum StrAccum;
		//typedef struct Table Table;
		//typedef struct TableLock TableLock;
		//typedef struct Token Token;
		//typedef struct Trigger Trigger;
		//typedef struct TriggerPrg TriggerPrg;
		//typedef struct TriggerStep TriggerStep;
		//typedef struct UnpackedRecord UnpackedRecord;
		//typedef struct VTable VTable;
		//typedef struct VtabCtx VtabCtx;
		//typedef struct Walker Walker;
		//typedef struct WherePlan WherePlan;
		//typedef struct WhereInfo WhereInfo;
		//typedef struct WhereLevel WhereLevel;
		///<summary>
		/// Defer sourcing vdbe.h and btree.h until after the "u8" and
		/// "BusyHandler" typedefs. vdbe.h also requires a few of the opaque
		/// pointer types (i.e. FuncDef) defined above.
		///
		///</summary>
		//#include "btree.h"
		//#include "vdbe.h"
		//#include "pager.h"
		//#include "pcache_g.h"
		//#include "os.h"
		//#include "mutex.h"
		///<summary>
		/// Each database file to be accessed by the system is an instance
		/// of the following structure.  There are normally two of these structures
		/// in the sqlite.aDb[] array.  aDb[0] is the main database file and
		/// aDb[1] is the database file used to hold temporary tables.  Additional
		/// databases may be attached.
		///
		///</summary>
		public class Db {
			public string zName;
			/*  Name of this database  */public Btree pBt;
			/*  The B Tree structure for this database file  */public u8 inTrans;
			/*  0: not writable.  1: Transaction.  2: Checkpoint  */public u8 safety_level;
			/*  How aggressive at syncing data to disk  */public Schema pSchema;
		/* Pointer to database schema (possibly shared)  */};

		/*
    ** An instance of the following structure stores a database schema.
    **
    ** Most Schema objects are associated with a Btree.  The exception is
    ** the Schema for the TEMP databaes (sqlite3.aDb[1]) which is free-standing.
    ** In shared cache mode, a single Schema object can be shared by multiple
    ** Btrees that refer to the same underlying BtShared object.
    ** 
    ** Schema objects are automatically deallocated when the last Btree that
    ** references them is destroyed.   The TEMP Schema is manually freed by
    ** sqlite3_close().
    *
    ** A thread must be holding a mutex on the corresponding Btree in order
    ** to access Schema content.  This implies that the thread must also be
    ** holding a mutex on the sqlite3 connection pointer that owns the Btree.
    ** For a TEMP Schema, only the connection mutex is required.
    */public class Schema {
			public int schema_cookie;
			/* Database schema version number for this file */public u32 iGeneration;
			/* Generation counter.  Incremented with each change */public Hash tblHash=new Hash();
			/* All tables indexed by name */public Hash idxHash=new Hash();
			/* All (named) indices indexed by name */public Hash trigHash=new Hash();
			/* All triggers indexed by name */public Hash fkeyHash=new Hash();
			/* All foreign keys by referenced table name */public Table pSeqTab;
			/* The sqlite_sequence table used by AUTOINCREMENT */public u8 file_format;
			/* Schema format version for this file */public SqliteEncoding enc;
			/* Text encoding used by this database */public u16 flags;
			///<summary>
			///Flags associated with this schema
			///</summary>
			public int cache_size;
			///<summary>
			///Number of pages to use in the cache
			///</summary>
			public Schema Copy() {
				if(this==null)
					return null;
				else {
					Schema cp=(Schema)MemberwiseClone();
					return cp;
				}
			}
			public void Clear() {
				if(this!=null) {
					schema_cookie=0;
					tblHash=new Hash();
					idxHash=new Hash();
					trigHash=new Hash();
					fkeyHash=new Hash();
					pSeqTab=null;
				}
			}
		};

		///<summary>
		/// These macros can be used to test, set, or clear bits in the
		/// Db.pSchema->flags field.
		///
		///</summary>
		//#define DbHasProperty(D,I,P)     (((D)->aDb[I].pSchema->flags&(P))==(P))
		private static bool DbHasProperty(sqlite3 D,int I,ushort P) {
			return (D.aDb[I].pSchema.flags&P)==P;
		}
		//#define DbHasAnyProperty(D,I,P)  (((D)->aDb[I].pSchema->flags&(P))!=0)
		//#define DbSetProperty(D,I,P)     (D)->aDb[I].pSchema->flags|=(P)
		private static void DbSetProperty(sqlite3 D,int I,ushort P) {
			D.aDb[I].pSchema.flags=(u16)(D.aDb[I].pSchema.flags|P);
		}
		//#define DbClearProperty(D,I,P)   (D)->aDb[I].pSchema->flags&=~(P)
		private static void DbClearProperty(sqlite3 D,int I,ushort P) {
			D.aDb[I].pSchema.flags=(u16)(D.aDb[I].pSchema.flags&~P);
		}
		/*
    ** Allowed values for the DB.pSchema->flags field.
    **
    ** The DB_SchemaLoaded flag is set after the database schema has been
    ** read into internal hash tables.
    **
    ** DB_UnresetViews means that one or more views have column names that
    ** have been filled out.  If the schema changes, these column names might
    ** changes and so the view will need to be reset.
    *///#define DB_SchemaLoaded    0x0001  /* The schema has been loaded */
		//#define DB_UnresetViews    0x0002  /* Some views have defined column names */
		//#define DB_Empty           0x0004  /* The file is empty (length 0 bytes) */
		private const u16 DB_SchemaLoaded=0x0001;
		private const u16 DB_UnresetViews=0x0002;
		private const u16 DB_Empty=0x0004;
		///<summary>
		/// The number of different kinds of things that can be limited
		/// using the sqlite3_limit() interface.
		///
		///</summary>
		//#define SQLITE_N_LIMIT (SQLITE_LIMIT_TRIGGER_DEPTH+1)
		private const int SQLITE_N_LIMIT=SQLITE_LIMIT_TRIGGER_DEPTH+1;
		///<summary>
		/// Lookaside malloc is a set of fixed-size buffers that can be used
		/// to satisfy small transient memory allocation requests for objects
		/// associated with a particular database connection.  The use of
		/// lookaside malloc provides a significant performance enhancement
		/// (approx 10%) by avoiding numerous malloc/free requests while parsing
		/// SQL statements.
		///
		/// The Lookaside structure holds configuration information about the
		/// lookaside malloc subsystem.  Each available memory allocation in
		/// the lookaside subsystem is stored on a linked list of LookasideSlot
		/// objects.
		///
		/// Lookaside allocations are only allowed for objects that are associated
		/// with a particular database connection.  Hence, schema information cannot
		/// be stored in lookaside because in shared cache mode the schema information
		/// is shared by multiple database connections.  Therefore, while parsing
		/// schema information, the Lookaside.bEnabled flag is cleared so that
		/// lookaside allocations are not used to construct the schema objects.
		///
		///</summary>
		public class Lookaside {
			public int sz;
			/* Size of each buffer in bytes */public u8 bEnabled;
			/* False to disable new lookaside allocations */public bool bMalloced;
			/* True if pStart obtained from sqlite3_malloc() */public int nOut;
			/* Number of buffers currently checked out */public int mxOut;
			/* Highwater mark for nOut */public int[] anStat=new int[3];
			/* 0: hits.  1: size misses.  2: full misses */public LookasideSlot pFree;
			/* List of available buffers */public int pStart;
			/* First byte of available memory space */public int pEnd;
		/* First byte past end of available space */};

		public class LookasideSlot {
			public LookasideSlot pNext;
		/* Next buffer in the list of free buffers */};

		///<summary>
		/// A hash table for function definitions.
		///
		/// Hash each FuncDef structure into one of the FuncDefHash.a[] slots.
		/// Collisions are on the FuncDef.pHash chain.
		///
		///</summary>
		public class FuncDefHash {
			public FuncDef[] a=new FuncDef[23];
		/* Hash table for functions */};

		/*
    ** Each database connection is an instance of the following structure.
    **
    ** The sqlite.lastRowid records the last insert rowid generated by an
    ** insert statement.  Inserts on views do not affect its value.  Each
    ** trigger has its own context, so that lastRowid can be updated inside
    ** triggers as usual.  The previous value will be restored once the trigger
    ** exits.  Upon entering a before or instead of trigger, lastRowid is no
    ** longer (since after version 2.8.12) reset to -1.
    **
    ** The sqlite.nChange does not count changes within triggers and keeps no
    ** context.  It is reset at start of sqlite3_exec.
    ** The sqlite.lsChange represents the number of changes made by the last
    ** insert, update, or delete statement.  It remains constant throughout the
    ** length of a statement and is then updated by OP_SetCounts.  It keeps a
    ** context stack just like lastRowid so that the count of changes
    ** within a trigger is not seen outside the trigger.  Changes to views do not
    ** affect the value of lsChange.
    ** The sqlite.csChange keeps track of the number of current changes (since
    ** the last statement) and is used to update sqlite_lsChange.
    **
    ** The member variables sqlite.errCode, sqlite.zErrMsg and sqlite.zErrMsg16
    ** store the most recent error code and, if applicable, string. The
    ** internal function sqlite3Error() is used to set these variables
    ** consistently.
    */public class sqlite3 {
          public sqlite3()
          {

          }
			public sqlite3_vfs pVfs;
			/* OS Interface */public int nDb;
			/* Number of backends currently in use */public Db[] aDb=new Db[SQLITE_MAX_ATTACHED];
			/* All backends */public int flags;
			/* Miscellaneous flags. See below */public int openFlags;
			/* Flags passed to sqlite3_vfs.xOpen() */public int errCode;
			/* Most recent error code (SQLITE_) */public int errMask;
			/* & result codes with this before returning */public u8 autoCommit;
			/* The auto-commit flag. */public u8 temp_store;
			/* 1: file 2: memory 0: default */// Cannot happen under C#
			// public u8 mallocFailed;           /* True if we have seen a malloc failure */
			public u8 dfltLockMode;
			/* Default locking-mode for attached dbs */public int nextAutovac;
			/* Autovac setting after VACUUM if >=0 */public u8 suppressErr;
			/* Do not issue error messages if true */public u8 vtabOnConflict;
			/* Value to return for s3_vtab_on_conflict() */public int nextPagesize;
			/* Pagesize after VACUUM if >0 */public int nTable;
			/* Number of tables in the database */public CollSeq pDfltColl;
			/* The default collating sequence (BINARY) */public i64 lastRowid;
			/* ROWID of most recent insert (see above) */public u32 magic;
			/* Magic number for detect library misuse */public int nChange;
			/* Value returned by sqlite3_changes() */public int nTotalChange;
			/* Value returned by sqlite3_total_changes() */public sqlite3_mutex mutex;
			///<summary>
			///Connection mutex
			///</summary>
			public int[] aLimit=new int[SQLITE_N_LIMIT];
			/* Limits */public class sqlite3InitInfo {
				/* Information used during initialization */public int iDb;
				/* When back is being initialized */public int newTnum;
				/* Rootpage of table being initialized */public u8 busy;
				/* TRUE if currently initializing */public u8 orphanTrigger;
			/* Last statement is orphaned TEMP trigger */};

			public sqlite3InitInfo init=new sqlite3InitInfo();
			public int nExtension;
			/* Number of loaded extensions */public object[] aExtension;
			/* Array of shared library handles */public Vdbe pVdbe;
			/* List of active virtual machines */public int activeVdbeCnt;
			/* Number of VDBEs currently executing */public int writeVdbeCnt;
			/* Number of active VDBEs that are writing */
          /// <summary>
            /// Number of nested calls to VdbeExec()
          /// </summary>
            public int callStackDepth;
			public dxTrace xTrace;
			//)(void*,const char);        /* Trace function */
			public object pTraceArg;
			/* Argument to the trace function */public dxProfile xProfile;
			//)(void*,const char*,u64);  /* Profiling function */
			public object pProfileArg;
			/* Argument to profile function */public object pCommitArg;
			/* Argument to xCommitCallback() */public dxCommitCallback xCommitCallback;
			//)(void);    /* Invoked at every commit. */
			public object pRollbackArg;
			/* Argument to xRollbackCallback() */public dxRollbackCallback xRollbackCallback;
			//)(void); /* Invoked at every commit. */
			public object pUpdateArg;
			public dxUpdateCallback xUpdateCallback;
			//)(void*,int, const char*,const char*,sqlite_int64);
			#if !SQLITE_OMIT_WAL
																																																																																				//int (*xWalCallback)(void *, sqlite3 *, string , int);
//void *pWalArg;
#endif
			public dxCollNeeded xCollNeeded;
			//)(void*,sqlite3*,int eTextRep,const char);
			public dxCollNeeded xCollNeeded16;
			//)(void*,sqlite3*,int eTextRep,const void);
			public object pCollNeededArg;
			public sqlite3_value pErr;
			/* Most recent error message */public string zErrMsg;
			///<summary>
			///Most recent error message (UTF-8 encoded)
			///</summary>
			public string zErrMsg16;
			/* Most recent error message (UTF-16 encoded) */public struct _u1 {
				public bool isInterrupted;
				/* True if sqlite3_interrupt has been called */public double notUsed1;
			/* Spacer */}
			public _u1 u1;
			public Lookaside lookaside=new Lookaside();
			/* Lookaside malloc configuration */
			#if !SQLITE_OMIT_AUTHORIZATION
																																																																																				public dxAuth xAuth;//)(void*,int,const char*,const char*,const char*,const char);
/* Access authorization function */
public object pAuthArg;               /* 1st argument to the access auth function */
#endif
			#if !SQLITE_OMIT_PROGRESS_CALLBACK
			public dxProgress xProgress;
			//)(void );  /* The progress callback */
			public object pProgressArg;
			/* Argument to the progress callback */public int nProgressOps;
			/* Number of opcodes for progress callback */
			#endif
			#if !SQLITE_OMIT_VIRTUALTABLE
			public Hash aModule;
			/* populated by sqlite3_create_module() */public VtabCtx pVtabCtx;
			/* Context for active vtab connect/create */public VTable[] aVTrans;
			/* Virtual tables with open transactions */public int nVTrans;
			/* Allocated size of aVTrans */public VTable pDisconnect;
			/* Disconnect these in next sqlite3_prepare() */
			#endif
			public FuncDefHash aFunc=new FuncDefHash();
			/* Hash table of connection functions */public Hash aCollSeq=new Hash();
			/* All collating sequences */public BusyHandler busyHandler=new BusyHandler();
			/* Busy callback */public int busyTimeout;
			/* Busy handler timeout, in msec */public Db[] aDbStatic=new Db[] {
				new Db(),
				new Db()
			};
			/* Static space for the 2 default backends */public Savepoint pSavepoint;
			/* List of active savepoints */public int nSavepoint;
			/* Number of non-transaction savepoints */public int nStatement;
			/* Number of nested statement-transactions  */public u8 isTransactionSavepoint;
			/* True if the outermost savepoint is a TS */public i64 nDeferredCons;
			/* Net deferred constraints this transaction. */public int pnBytesFreed;
			/* If not NULL, increment this in DbFree() */
			#if SQLITE_ENABLE_UNLOCK_NOTIFY
																																																														/* The following variables are all protected by the STATIC_MASTER
** mutex, not by sqlite3.mutex. They are used by code in notify.c.
**
** When X.pUnlockConnection==Y, that means that X is waiting for Y to
** unlock so that it can proceed.
**
** When X.pBlockingConnection==Y, that means that something that X tried
** tried to do recently failed with an SQLITE_LOCKED error due to locks
** held by Y.
*/
sqlite3 *pBlockingConnection; /* Connection that caused SQLITE_LOCKED */
sqlite3 *pUnlockConnection;           /* Connection to watch for unlock */
void *pUnlockArg;                     /* Argument to xUnlockNotify */
void (*xUnlockNotify)(void **, int);  /* Unlock notify callback */
sqlite3 *pNextBlocked;        /* Next in list of all blocked connections */
#endif
			public void whereOrInfoDelete(WhereOrInfo p) {
				p.wc.whereClauseClear();
				this.sqlite3DbFree(ref p);
			}
			public void whereAndInfoDelete(WhereAndInfo p) {
				p.wc.whereClauseClear();
				this.sqlite3DbFree(ref p);
			}
			public string explainIndexRange(WhereLevel pLevel,Table pTab) {
				WherePlan pPlan=pLevel.plan;
				Index pIndex=pPlan.u.pIdx;
				uint nEq=pPlan.nEq;
				int i,j;
				Column[] aCol=pTab.aCol;
				int[] aiColumn=pIndex.aiColumn;
				StrAccum txt=new StrAccum(100);
				if(nEq==0&&(pPlan.wsFlags&(WHERE_BTM_LIMIT|WHERE_TOP_LIMIT))==0) {
					return null;
				}
				sqlite3StrAccumInit(txt,null,0,SQLITE_MAX_LENGTH);
				txt.db=this;
				sqlite3StrAccumAppend(txt," (",2);
				for(i=0;i<nEq;i++) {
					txt.explainAppendTerm(i,aCol[aiColumn[i]].zName,"=");
				}
				j=i;
				if((pPlan.wsFlags&WHERE_BTM_LIMIT)!=0) {
					txt.explainAppendTerm(i++,aCol[aiColumn[j]].zName,">");
				}
				if((pPlan.wsFlags&WHERE_TOP_LIMIT)!=0) {
					txt.explainAppendTerm(i,aCol[aiColumn[j]].zName,"<");
				}
				sqlite3StrAccumAppend(txt,")",1);
				return sqlite3StrAccumFinish(txt);
			}
			public void whereInfoFree(WhereInfo pWInfo) {
				if(ALWAYS(pWInfo!=null)) {
					int i;
					for(i=0;i<pWInfo.nLevel;i++) {
						sqlite3_index_info pInfo=pWInfo.a[i]!=null?pWInfo.a[i].pIdxInfo:null;
						if(pInfo!=null) {
							/* Debug.Assert( pInfo.needToFreeIdxStr==0 || db.mallocFailed ); */if(pInfo.needToFreeIdxStr!=0) {
								//sqlite3_free( ref pInfo.idxStr );
							}
							this.sqlite3DbFree(ref pInfo);
						}
						if(pWInfo.a[i]!=null&&(pWInfo.a[i].plan.wsFlags&WHERE_TEMP_INDEX)!=0) {
							Index pIdx=pWInfo.a[i].plan.u.pIdx;
							if(pIdx!=null) {
								this.sqlite3DbFree(ref pIdx.zColAff);
								this.sqlite3DbFree(ref pIdx);
							}
						}
					}
					pWInfo.pWC.whereClauseClear();
					this.sqlite3DbFree(ref pWInfo);
				}
			}
			public void sqlite3DbFree(ref string pString) {
			}
			public void sqlite3DbFree<T>(ref T pT) where T : class {
			}
			public void sqlite3DbFree(ref Mem[] pPrior) {
				if(pPrior!=null)
					for(int i=0;i<pPrior.Length;i++)
						sqlite3MemFreeMem(ref pPrior[i]);
			}
			public void sqlite3DbFree(ref Mem pPrior) {
				if(pPrior!=null)
					sqlite3MemFreeMem(ref pPrior);
			}
			public void sqlite3DbFree(ref int[] pPrior) {
				if(pPrior!=null)
					sqlite3MemFreeInt(ref pPrior);
			}
		}
		///<summary>
		/// A macro to discover the encoding of a database.
		///
		///</summary>
		//#define ENC(db) ((db)->aDb[0].pSchema->enc)
		private static SqliteEncoding ENC(sqlite3 db) {
			return db.aDb[0].pSchema.enc;
		}
		/*
    ** Possible values for the sqlite3.flags.
    *///#define SQLITE_VdbeTrace      0x00000100  /* True to trace VDBE execution */
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
		private const int SQLITE_VdbeTrace=0x00000100;
		private const int SQLITE_InternChanges=0x00000200;
		private const int SQLITE_FullColNames=0x00000400;
		private const int SQLITE_ShortColNames=0x00000800;
		private const int SQLITE_CountRows=0x00001000;
		private const int SQLITE_NullCallback=0x00002000;
		private const int SQLITE_SqlTrace=0x00004000;
		private const int SQLITE_VdbeListing=0x00008000;
		private const int SQLITE_WriteSchema=0x00010000;
		private const int SQLITE_NoReadlock=0x00020000;
		private const int SQLITE_IgnoreChecks=0x00040000;
		private const int SQLITE_ReadUncommitted=0x0080000;
		private const int SQLITE_LegacyFileFmt=0x00100000;
		private const int SQLITE_FullFSync=0x00200000;
		private const int SQLITE_CkptFullFSync=0x00400000;
		private const int SQLITE_RecoveryMode=0x00800000;
		private const int SQLITE_ReverseOrder=0x01000000;
		private const int SQLITE_RecTriggers=0x02000000;
		private const int SQLITE_ForeignKeys=0x04000000;
		private const int SQLITE_AutoIndex=0x08000000;
		private const int SQLITE_PreferBuiltin=0x10000000;
		private const int SQLITE_LoadExtension=0x20000000;
		private const int SQLITE_EnableTrigger=0x40000000;
		/*
    ** Bits of the sqlite3.flags field that are used by the
    ** sqlite3_test_control(SQLITE_TESTCTRL_OPTIMIZATIONS,...) interface.
    ** These must be the low-order bits of the flags field.
    *///#define SQLITE_QueryFlattener 0x01        /* Disable query flattening */
		//#define SQLITE_ColumnCache    0x02        /* Disable the column cache */
		//#define SQLITE_IndexSort      0x04        /* Disable indexes for sorting */
		//#define SQLITE_IndexSearch    0x08        /* Disable indexes for searching */
		//#define SQLITE_IndexCover     0x10        /* Disable index covering table */
		//#define SQLITE_GroupByOrder   0x20        /* Disable GROUPBY cover of ORDERBY */
		//#define SQLITE_FactorOutConst 0x40        /* Disable factoring out constants */
		//#define SQLITE_IdxRealAsInt   0x80        /* Store REAL as INT in indices */
		//#define SQLITE_OptMask        0xff        /* Mask of all disablable opts */
		private const int SQLITE_QueryFlattener=0x01;
		private const int SQLITE_ColumnCache=0x02;
		private const int SQLITE_IndexSort=0x04;
		private const int SQLITE_IndexSearch=0x08;
		private const int SQLITE_IndexCover=0x10;
		private const int SQLITE_GroupByOrder=0x20;
		private const int SQLITE_FactorOutConst=0x40;
		private const int SQLITE_IdxRealAsInt=0x80;
		private const int SQLITE_OptMask=0xff;
		///<summary>
		/// Possible values for the sqlite.magic field.
		/// The numbers are obtained at random and have no special meaning, other
		/// than being distinct from one another.
		///
		///</summary>
		private const int SQLITE_MAGIC_OPEN=0x1029a697;
		//#define SQLITE_MAGIC_OPEN     0xa029a697  /* Database is open */
		private const int SQLITE_MAGIC_CLOSED=0x2f3c2d33;
		//#define SQLITE_MAGIC_CLOSED   0x9f3c2d33  /* Database is closed */
		private const int SQLITE_MAGIC_SICK=0x3b771290;
		//#define SQLITE_MAGIC_SICK     0x4b771290  /* Error and awaiting close */
		private const int SQLITE_MAGIC_BUSY=0x403b7906;
		//#define SQLITE_MAGIC_BUSY     0xf03b7906  /* Database currently in use */
		private const int SQLITE_MAGIC_ERROR=0x55357930;
		//#define SQLITE_MAGIC_ERROR    0xb5357930  /* An SQLITE_MISUSE error occurred */
		/*
    ** Each SQL function is defined by an instance of the following
    ** structure.  A pointer to this structure is stored in the sqlite.aFunc
    ** hash table.  When multiple functions have the same name, the hash table
    ** points to a linked list of these structures.
    */public class FuncDef {
			public i16 nArg;
			/* Number of arguments.  -1 means unlimited */public SqliteEncoding iPrefEnc;
			/* Preferred text encoding (SqliteEncoding.UTF8, 16LE, 16BE) */public u8 flags;
			/* Some combination of SQLITE_FUNC_* */public object pUserData;
			/* User data parameter */public FuncDef pNext;
			/* Next function with same name */public dxFunc xFunc;
			//)(sqlite3_context*,int,sqlite3_value*); /* Regular function */
			public dxStep xStep;
			//)(sqlite3_context*,int,sqlite3_value*); /* Aggregate step */
			public dxFinal xFinalize;
			//)(sqlite3_context);                /* Aggregate finalizer */
			public string zName;
			/* SQL name of the function. */public FuncDef pHash;
			/* Next with a different name but the same hash */public FuncDestructor pDestructor;
			///<summary>
			///Reference counted destructor function
			///</summary>
			public FuncDef() {
			}
			public FuncDef(i16 nArg,SqliteEncoding iPrefEnc,u8 iflags,object pUserData,FuncDef pNext,dxFunc xFunc,dxStep xStep,dxFinal xFinalize,string zName,FuncDef pHash,FuncDestructor pDestructor) {
				this.nArg=nArg;
				this.iPrefEnc=iPrefEnc;
				this.flags=iflags;
				this.pUserData=pUserData;
				this.pNext=pNext;
				this.xFunc=xFunc;
				this.xStep=xStep;
				this.xFinalize=xFinalize;
				this.zName=zName;
				this.pHash=pHash;
				this.pDestructor=pDestructor;
			}
			public FuncDef(string zName,SqliteEncoding iPrefEnc,i16 nArg,int iArg,u8 iflags,dxFunc xFunc) {
				this.nArg=nArg;
				this.iPrefEnc=iPrefEnc;
				this.flags=iflags;
				this.pUserData=iArg;
				this.pNext=null;
				this.xFunc=xFunc;
				this.xStep=null;
				this.xFinalize=null;
				this.zName=zName;
			}
			public FuncDef(string zName,SqliteEncoding iPrefEnc,i16 nArg,int iArg,u8 iflags,dxStep xStep,dxFinal xFinal) {
				this.nArg=nArg;
				this.iPrefEnc=iPrefEnc;
				this.flags=iflags;
				this.pUserData=iArg;
				this.pNext=null;
				this.xFunc=null;
				this.xStep=xStep;
				this.xFinalize=xFinal;
				this.zName=zName;
			}
			public FuncDef(string zName,SqliteEncoding iPrefEnc,i16 nArg,object arg,dxFunc xFunc,u8 flags) {
				this.nArg=nArg;
				this.iPrefEnc=iPrefEnc;
				this.flags=flags;
				this.pUserData=arg;
				this.pNext=null;
				this.xFunc=xFunc;
				this.xStep=null;
				this.xFinalize=null;
				this.zName=zName;
			}
			public FuncDef Copy() {
				FuncDef c=new FuncDef();
				c.nArg=nArg;
				c.iPrefEnc=iPrefEnc;
				c.flags=flags;
				c.pUserData=pUserData;
				c.pNext=pNext;
				c.xFunc=xFunc;
				c.xStep=xStep;
				c.xFinalize=xFinalize;
				c.zName=zName;
				c.pHash=pHash;
				c.pDestructor=pDestructor;
				return c;
			}
		};

		///<summary>
		/// This structure encapsulates a user-function destructor callback (as
		/// configured using create_function_v2()) and a reference counter. When
		/// create_function_v2() is called to create a function with a destructor,
		/// a single object of this type is allocated. FuncDestructor.nRef is set to
		/// the number of FuncDef objects created (either 1 or 3, depending on whether
		/// or not the specified encoding is SqliteEncoding.ANY). The FuncDef.pDestructor
		/// member of each of the new FuncDef objects is set to point to the allocated
		/// FuncDestructor.
		///
		/// Thereafter, when one of the FuncDef objects is deleted, the reference
		/// count on this object is decremented. When it reaches 0, the destructor
		/// is invoked and the FuncDestructor structure freed.
		///
		///</summary>
		//struct FuncDestructor {
		//  int nRef;
		//  void (*xDestroy)(void );
		//  void *pUserData;
		//};
		public class FuncDestructor {
			public int nRef;
			public dxFDestroy xDestroy;
			// (*xDestroy)(void );
			public object pUserData;
		};

		/*
    ** Possible values for FuncDef.flags
    *///#define SQLITE_FUNC_LIKE     0x01  /* Candidate for the LIKE optimization */
		//#define SQLITE_FUNC_CASE     0x02  /* Case-sensitive LIKE-type function */
		//#define SQLITE_FUNC_EPHEM    0x04  /* Ephemeral.  Delete with VDBE */
		//#define SQLITE_FUNC_NEEDCOLL 0x08 /* sqlite3GetFuncCollSeq() might be called */
		//#define SQLITE_FUNC_PRIVATE  0x10 /* Allowed for internal use only */
		//#define SQLITE_FUNC_COUNT    0x20 /* Built-in count() aggregate */
		//#define SQLITE_FUNC_COALESCE 0x40 /* Built-in coalesce() or ifnull() function */
		private const int SQLITE_FUNC_LIKE=0x01;
		/* Candidate for the LIKE optimization */private const int SQLITE_FUNC_CASE=0x02;
		/* Case-sensitive LIKE-type function */private const int SQLITE_FUNC_EPHEM=0x04;
		/* Ephermeral.  Delete with VDBE */private const int SQLITE_FUNC_NEEDCOLL=0x08;
		/* sqlite3GetFuncCollSeq() might be called */private const int SQLITE_FUNC_PRIVATE=0x10;
		/* Allowed for internal use only */private const int SQLITE_FUNC_COUNT=0x20;
		/* Built-in count() aggregate */private const int SQLITE_FUNC_COALESCE=0x40;
		/* Built-in coalesce() or ifnull() function *////<summary>
		/// The following three macros, FUNCTION(), LIKEFUNC() and AGGREGATE() are
		/// used to create the initializers for the FuncDef structures.
		///
		///   FUNCTION(zName, nArg, iArg, bNC, xFunc)
		///     Used to create a scalar function definition of a function zName
		///     implemented by C function xFunc that accepts nArg arguments. The
		///     value passed as iArg is cast to a (void) and made available
		///     as the user-data (sqlite3_user_data()) for the function. If
		///     argument bNC is true, then the SQLITE_FUNC_NEEDCOLL flag is set.
		///
		///   AGGREGATE(zName, nArg, iArg, bNC, xStep, xFinal)
		///     Used to create an aggregate function definition implemented by
		///     the C functions xStep and xFinal. The first four parameters
		///     are interpreted in the same way as the first 4 parameters to
		///     FUNCTION().
		///
		///   LIKEFUNC(zName, nArg, pArg, flags)
		///     Used to create a scalar function definition of a function zName
		///     that accepts nArg arguments and is implemented by a call to C
		///     function likeFunc. Argument pArg is cast to a (void ) and made
		///     available as the function user-data (sqlite3_user_data()). The
		///     FuncDef.flags variable is set to the value passed as the flags
		///     parameter.
		///
		///</summary>
		//#define FUNCTION(zName, nArg, iArg, bNC, xFunc) \
		//  {nArg, SqliteEncoding.UTF8, bNC*SQLITE_FUNC_NEEDCOLL, \
		//SQLITE_INT_TO_PTR(iArg), 0, xFunc, 0, 0, #zName, 0, 0}
		private static FuncDef FUNCTION(string zName,i16 nArg,int iArg,u8 bNC,dxFunc xFunc) {
			return new FuncDef(zName,SqliteEncoding.UTF8,nArg,iArg,(u8)(bNC*SQLITE_FUNC_NEEDCOLL),xFunc);
		}
		//#define STR_FUNCTION(zName, nArg, pArg, bNC, xFunc) \
		//  {nArg, SqliteEncoding.UTF8, bNC*SQLITE_FUNC_NEEDCOLL, \
		//pArg, 0, xFunc, 0, 0, #zName, 0, 0}
		//#define LIKEFUNC(zName, nArg, arg, flags) \
		//  {nArg, SqliteEncoding.UTF8, flags, (void )arg, 0, likeFunc, 0, 0, #zName, 0, 0}
		private static FuncDef LIKEFUNC(string zName,i16 nArg,object arg,u8 flags) {
			return new FuncDef(zName,SqliteEncoding.UTF8,nArg,arg,likeFunc,flags);
		}
		//#define AGGREGATE(zName, nArg, arg, nc, xStep, xFinal) \
		//  {nArg, SqliteEncoding.UTF8, nc*SQLITE_FUNC_NEEDCOLL, \
		//SQLITE_INT_TO_PTR(arg), 0, 0, xStep,xFinal,#zName,0,0}
		private static FuncDef AGGREGATE(string zName,i16 nArg,int arg,u8 nc,dxStep xStep,dxFinal xFinal) {
			return new FuncDef(zName,SqliteEncoding.UTF8,nArg,arg,(u8)(nc*SQLITE_FUNC_NEEDCOLL),xStep,xFinal);
		}
		///<summary>
		/// All current savepoints are stored in a linked list starting at
		/// sqlite3.pSavepoint. The first element in the list is the most recently
		/// opened savepoint. Savepoints are added to the list by the vdbe
		/// OP_Savepoint instruction.
		///
		///</summary>
		//struct Savepoint {
		//  string zName;                        /* Savepoint name (nul-terminated) */
		//  i64 nDeferredCons;                  /* Number of deferred fk violations */
		//  Savepoint *pNext;                   /* Parent savepoint (if any) */
		//};
		public class Savepoint {
			public string zName;
			/* Savepoint name (nul-terminated) */public i64 nDeferredCons;
			/* Number of deferred fk violations */public Savepoint pNext;
		/* Parent savepoint (if any) */};

		///<summary>
		/// The following are used as the second parameter to sqlite3Savepoint(),
		/// and as the P1 argument to the OP_Savepoint instruction.
		///
		///</summary>
		private const int SAVEPOINT_BEGIN=0;
		//#define SAVEPOINT_BEGIN      0
		private const int SAVEPOINT_RELEASE=1;
		//#define SAVEPOINT_RELEASE    1
		private const int SAVEPOINT_ROLLBACK=2;
		//#define SAVEPOINT_ROLLBACK   2
		///<summary>
		/// Each SQLite module (virtual table definition) is defined by an
		/// instance of the following structure, stored in the sqlite3.aModule
		/// hash table.
		///
		///</summary>
		public class Module {
			public sqlite3_module pModule;
			/* Callback pointers */public string zName;
			/* Name passed to create_module() */public object pAux;
			/* pAux passed to create_module() */public smdxDestroy xDestroy;
		//)(void );/* Module destructor function */
		};

		///<summary>
		/// information about each column of an SQL table is held in an instance
		/// of this structure.
		///
		///</summary>
		public class Column {
			public string zName;
			/* Name of this column */public Expr pDflt;
			/* Default value of this column */public string zDflt;
			/* Original text of the default value */public string zType;
			/* Data type for this column */public string zColl;
			/* Collating sequence.  If NULL, use the default */public u8 notNull;
			/* True if there is a NOT NULL constraint */public u8 isPrimKey;
			/* True if this column is part of the PRIMARY KEY */public char affinity;
			/* One of the SQLITE_AFF_... values */
			#if !SQLITE_OMIT_VIRTUALTABLE
			public u8 isHidden;
			///<summary>
			///True if this column is 'hidden'
			///</summary>
			#endif
			public Column Copy() {
				Column cp=(Column)MemberwiseClone();
				if(cp.pDflt!=null)
					cp.pDflt=pDflt.Copy();
				return cp;
			}
		};

		///<summary>
		/// A "Collating Sequence" is defined by an instance of the following
		/// structure. Conceptually, a collating sequence consists of a name and
		/// a comparison routine that defines the order of that sequence.
		///
		/// There may two separate implementations of the collation function, one
		/// that processes text in UTF-8 encoding (CollSeq.xCmp) and another that
		/// processes text encoded in UTF-16 (CollSeq.xCmp16), using the machine
		/// native byte order. When a collation sequence is invoked, SQLite selects
		/// the version that will require the least expensive encoding
		/// translations, if any.
		///
		/// The CollSeq.pUser member variable is an extra parameter that passed in
		/// as the first argument to the UTF-8 comparison function, xCmp.
		/// CollSeq.pUser16 is the equivalent for the UTF-16 comparison function,
		/// xCmp16.
		///
		/// If both CollSeq.xCmp and CollSeq.xCmp16 are NULL, it means that the
		/// collating sequence is undefined.  Indices built on an undefined
		/// collating sequence may not be read or written.
		///
		///</summary>
		public class CollSeq {
			public string zName;
			/* Name of the collating sequence, UTF-8 encoded */public SqliteEncoding enc;
			/* Text encoding handled by xCmp() */public CollationType type;
			/* One of the CollationType.... values below */public object pUser;
			///<summary>
			///First argument to xCmp()
			///</summary>
			public dxCompare xCmp;
			//)(void*,int, const void*, int, const void);
			public dxDelCollSeq xDel;
			//)(void);  /* Destructor for pUser */
			public CollSeq Copy() {
				if(this==null)
					return null;
				else {
					CollSeq cp=(CollSeq)MemberwiseClone();
					return cp;
				}
			}
		};

		/*
    ** Allowed values of CollSeq.type:
    */public enum CollationType {
			BINARY=1,
			//#define SQLITE_COLL_BINARY  1  /* The default memcmp() collating sequence */
			NOCASE=2,
			//#define SQLITE_COLL_NOCASE  2  /* The built-in NOCASE collating sequence */
			REVERSE=3,
			//#define SQLITE_COLL_REVERSE 3  /* The built-in REVERSE collating sequence */
			USER=0
		//#define SQLITE_COLL_USER    0  /* Any other user-defined collating sequence */
		}
		/*
    ** A sort order can be either ASC or DESC.
    */private const int SQLITE_SO_ASC=0;
		//#define SQLITE_SO_ASC       0  /* Sort in ascending order */
		private const int SQLITE_SO_DESC=1;
		//#define SQLITE_SO_DESC     1  /* Sort in ascending order */
		/*
    ** Column affinity types.
    **
    ** These used to have mnemonic name like 'i' for SQLITE_AFF_INTEGER and
    ** 't' for SQLITE_AFF_TEXT.  But we can save a little space and improve
    ** the speed a little by numbering the values consecutively.
    **
    ** But rather than start with 0 or 1, we begin with 'a'.  That way,
    ** when multiple affinity types are concatenated into a string and
    ** used as the P4 operand, they will be more readable.
    **
    ** Note also that the numeric types are grouped together so that testing
    ** for a numeric type is a single comparison.
    */private const char SQLITE_AFF_TEXT='a';
		//#define SQLITE_AFF_TEXT     'a'
		private const char SQLITE_AFF_NONE='b';
		//#define SQLITE_AFF_NONE     'b'
		private const char SQLITE_AFF_NUMERIC='c';
		//#define SQLITE_AFF_NUMERIC  'c'
		private const char SQLITE_AFF_INTEGER='d';
		//#define SQLITE_AFF_INTEGER  'd'
		private const char SQLITE_AFF_REAL='e';
		//#define SQLITE_AFF_REAL     'e'
		//#define sqlite3IsNumericAffinity(X)  ((X)>=SQLITE_AFF_NUMERIC)
		/*
    ** The SQLITE_AFF_MASK values masks off the significant bits of an
    ** affinity value.
    */private const int SQLITE_AFF_MASK=0x67;
		//#define SQLITE_AFF_MASK     0x67
		///<summary>
		/// Additional bit values that can be ORed with an affinity without
		/// changing the affinity.
		///
		///</summary>
		private const int SQLITE_JUMPIFNULL=0x08;
		//#define SQLITE_JUMPIFNULL   0x08  /* jumps if either operand is NULL */
		private const int SQLITE_STOREP2=0x10;
		//#define SQLITE_STOREP2      0x10  /* Store result in reg[P2] rather than jump */
		private const int SQLITE_NULLEQ=0x80;
		//#define SQLITE_NULLEQ       0x80  /* NULL=NULL */
		///<summary>
		/// An object of this type is created for each virtual table present in
		/// the database schema.
		///
		/// If the database schema is shared, then there is one instance of this
		/// structure for each database connection (sqlite3) that uses the shared
		/// schema. This is because each database connection requires its own unique
		/// instance of the sqlite3_vtab* handle used to access the virtual table
		/// implementation. sqlite3_vtab* handles can not be shared between
		/// database connections, even when the rest of the in-memory database
		/// schema is shared, as the implementation often stores the database
		/// connection handle passed to it via the xConnect() or xCreate() method
		/// during initialization internally. This database connection handle may
		/// then be used by the virtual table implementation to access real tables
		/// within the database. So that they appear as part of the callers
		/// transaction, these accesses need to be made via the same database
		/// connection as that used to execute SQL operations on the virtual table.
		///
		/// All VTable objects that correspond to a single table in a shared
		/// database schema are initially stored in a linked-list pointed to by
		/// the Table.pVTable member variable of the corresponding Table object.
		/// When an sqlite3_prepare() operation is required to access the virtual
		/// table, it searches the list for the VTable that corresponds to the
		/// database connection doing the preparing so as to use the correct
		/// sqlite3_vtab* handle in the compiled query.
		///
		/// When an in-memory Table object is deleted (for example when the
		/// schema is being reloaded for some reason), the VTable objects are not
		/// deleted and the sqlite3_vtab* handles are not xDisconnect()ed
		/// immediately. Instead, they are moved from the Table.pVTable list to
		/// another linked list headed by the sqlite3.pDisconnect member of the
		/// corresponding sqlite3 structure. They are then deleted/xDisconnected
		/// next time a statement is prepared using said sqlite3*. This is done
		/// to avoid deadlock issues involving multiple sqlite3.mutex mutexes.
		/// Refer to comments above function sqlite3VtabUnlockList() for an
		/// explanation as to why it is safe to add an entry to an sqlite3.pDisconnect
		/// list without holding the corresponding sqlite3.mutex mutex.
		///
		/// The memory for objects of this type is always allocated by
		/// sqlite3DbMalloc(), using the connection handle stored in VTable.db as
		/// the first argument.
		///
		///</summary>
		public class VTable {
			public sqlite3 db;
			/* Database connection associated with this table */public Module pMod;
			/* Pointer to module implementation */public sqlite3_vtab pVtab;
			/* Pointer to vtab instance */public int nRef;
			/* Number of pointers to this structure */public u8 bConstraint;
			/* True if constraints are supported */public int iSavepoint;
			/* Depth of the SAVEPOINT stack */public VTable pNext;
		/* Next in linked list (see above) */};

		/*
    ** Each SQL table is represented in memory by an instance of the
    ** following structure.
    **
    ** Table.zName is the name of the table.  The case of the original
    ** CREATE TABLE statement is stored, but case is not significant for
    ** comparisons.
    **
    ** Table.nCol is the number of columns in this table.  Table.aCol is a
    ** pointer to an array of Column structures, one for each column.
    **
    ** If the table has an INTEGER PRIMARY KEY, then Table.iPKey is the index of
    ** the column that is that key.   Otherwise Table.iPKey is negative.  Note
    ** that the datatype of the PRIMARY KEY must be INTEGER for this field to
    ** be set.  An INTEGER PRIMARY KEY is used as the rowid for each row of
    ** the table.  If a table has no INTEGER PRIMARY KEY, then a random rowid
    ** is generated for each row of the table.  TF_HasPrimaryKey is set if
    ** the table has any PRIMARY KEY, INTEGER or otherwise.
    **
    ** Table.tnum is the page number for the root BTree page of the table in the
    ** database file.  If Table.iDb is the index of the database table backend
    ** in sqlite.aDb[].  0 is for the main database and 1 is for the file that
    ** holds temporary tables and indices.  If TF_Ephemeral is set
    ** then the table is stored in a file that is automatically deleted
    ** when the VDBE cursor to the table is closed.  In this case Table.tnum
    ** refers VDBE cursor number that holds the table open, not to the root
    ** page number.  Transient tables are used to hold the results of a
    ** sub-query that appears instead of a real table name in the FROM clause
    ** of a SELECT statement.
    */public class Table {
			public string zName;
			/* Name of the table or view */public int iPKey;
			/* If not negative, use aCol[iPKey] as the primary key */public int nCol;
			/* Number of columns in this table */public Column[] aCol;
			/* Information about each column */public Index pIndex;
			/* List of SQL indexes on this table. */public int tnum;
			/* Root BTree node for this table (see note above) */public u32 nRowEst;
			/* Estimated rows in table - from sqlite_stat1 table */public Select pSelect;
			/* NULL for tables.  Points to definition if a view. */public u16 nRef;
			/* Number of pointers to this Table */public u8 tabFlags;
			/* Mask of TF_* values */public u8 keyConf;
			/* What to do in case of uniqueness conflict on iPKey */public FKey pFKey;
			/* Linked list of all foreign keys in this table */public string zColAff;
			/* String defining the affinity of each column */
			#if !SQLITE_OMIT_CHECK
			public Expr pCheck;
			/* The AND of all CHECK constraints */
			#endif
			#if !SQLITE_OMIT_ALTERTABLE
			public int addColOffset;
			/* Offset in CREATE TABLE stmt to add a new column */
			#endif
			#if !SQLITE_OMIT_VIRTUALTABLE
			public VTable pVTable;
			/* List of VTable objects. */public int nModuleArg;
			/* Number of arguments to the module */public string[] azModuleArg;
			/* Text of all module args. [0] is module name */
			#endif
			public Trigger pTrigger;
			/* List of SQL triggers on this table */public Schema pSchema;
			///<summary>
			///Schema that contains this table
			///</summary>
			public Table pNextZombie;
			/* Next on the Parse.pZombieTab list */public Table Copy() {
				if(this==null)
					return null;
				else {
					Table cp=(Table)MemberwiseClone();
					if(pIndex!=null)
						cp.pIndex=pIndex.Copy();
					if(pSelect!=null)
						cp.pSelect=pSelect.Copy();
					if(pTrigger!=null)
						cp.pTrigger=pTrigger.Copy();
					if(pFKey!=null)
						cp.pFKey=pFKey.Copy();
					#if !SQLITE_OMIT_CHECK
					// Don't Clone Checks, only copy reference via Memberwise Clone above --
					//if ( pCheck != null ) cp.pCheck = pCheck.Copy();
					#endif
					// Don't Clone Schema, only copy reference via Memberwise Clone above --
					// if ( pSchema != null ) cp.pSchema=pSchema.Copy();
					// Don't Clone pNextZombie, only copy reference via Memberwise Clone above --
					// if ( pNextZombie != null ) cp.pNextZombie=pNextZombie.Copy();
					return cp;
				}
			}
		};

		/*
    ** Allowed values for Tabe.tabFlags.
    *///#define TF_Readonly        0x01    /* Read-only system table */
		//#define TF_Ephemeral       0x02    /* An ephemeral table */
		//#define TF_HasPrimaryKey   0x04    /* Table has a primary key */
		//#define TF_Autoincrement   0x08    /* Integer primary key is autoincrement */
		//#define TF_Virtual         0x10    /* Is a virtual table */
		//#define TF_NeedMetadata    0x20    /* aCol[].zType and aCol[].pColl missing */
		/*
    ** Allowed values for Tabe.tabFlags.
    */private const int TF_Readonly=0x01;
		/* Read-only system table */private const int TF_Ephemeral=0x02;
		/* An ephemeral table */private const int TF_HasPrimaryKey=0x04;
		/* Table has a primary key */private const int TF_Autoincrement=0x08;
		/* Integer primary key is autoincrement */private const int TF_Virtual=0x10;
		/* Is a virtual table */private const int TF_NeedMetadata=0x20;
		///<summary>
		///aCol[].zType and aCol[].pColl missing
		///</summary>
		///<summary>
		/// Test to see whether or not a table is a virtual table.  This is
		/// done as a macro so that it will be optimized out when virtual
		/// table support is omitted from the build.
		///
		///</summary>
		#if !SQLITE_OMIT_VIRTUALTABLE
		//#  define IsVirtual(X)      (((X)->tabFlags & TF_Virtual)!=0)
		private static bool IsVirtual(Table X) {
			return (X.tabFlags&TF_Virtual)!=0;
		}
		//#  define IsHiddenColumn(X) ((X)->isHidden)
		private static bool IsHiddenColumn(Column X) {
			return X.isHidden!=0;
		}
		#else
																																																								    //  define IsVirtual(X)      0
    static bool IsVirtual( Table T )
    {
      return false;
    }
    //  define IsHiddenColumn(X) 0
    static bool IsHiddenColumn( Column C )
    {
      return false;
    }
#endif
		/*
** Each foreign key constraint is an instance of the following structure.
**
** A foreign key is associated with two tables.  The "from" table is
** the table that contains the REFERENCES clause that creates the foreign
** key.  The "to" table is the table that is named in the REFERENCES clause.
** Consider this example:
**
**     CREATE TABLE ex1(
**       a INTEGER PRIMARY KEY,
**       b INTEGER CONSTRAINT fk1 REFERENCES ex2(x)
**     );
**
** For foreign key "fk1", the from-table is "ex1" and the to-table is "ex2".
**
** Each REFERENCES clause generates an instance of the following structure
** which is attached to the from-table.  The to-table need not exist when
** the from-table is created.  The existence of the to-table is not checked.
*/public class FKey {
			public Table pFrom;
			/* Table containing the REFERENCES clause (aka: Child) */public FKey pNextFrom;
			/* Next foreign key in pFrom */public string zTo;
			/* Name of table that the key points to (aka: Parent) */public FKey pNextTo;
			/* Next foreign key on table named zTo */public FKey pPrevTo;
			/* Previous foreign key on table named zTo */public int nCol;
			/* Number of columns in this key *//* EV: R-30323-21917 */public u8 isDeferred;
			/* True if constraint checking is deferred till COMMIT */public u8[] aAction=new u8[2];
			///<summary>
			///ON DELETE and ON UPDATE actions, respectively
			///</summary>
			public Trigger[] apTrigger=new Trigger[2];
			///<summary>
			///Triggers for aAction[] actions
			///</summary>
			public class sColMap {
				/* Mapping of columns in pFrom to columns in zTo */public int iFrom;
				/* Index of column in pFrom */public string zCol;
			/* Name of column in zTo.  If 0 use PRIMARY KEY */};

			public sColMap[] aCol;
			/* One entry for each of nCol column s */public FKey Copy() {
				if(this==null)
					return null;
				else {
					FKey cp=(FKey)MemberwiseClone();
					return cp;
				}
			}
		};

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
		private const int OE_None=0;
		//#define OE_None     0   /* There is no constraint to check */
		private const int OE_Rollback=1;
		//#define OE_Rollback 1   /* Fail the operation and rollback the transaction */
		private const int OE_Abort=2;
		//#define OE_Abort    2   /* Back out changes but do no rollback transaction */
		private const int OE_Fail=3;
		//#define OE_Fail     3   /* Stop the operation but leave all prior changes */
		private const int OE_Ignore=4;
		//#define OE_Ignore   4   /* Ignore the error. Do not do the INSERT or UPDATE */
		private const int OE_Replace=5;
		//#define OE_Replace  5   /* Delete existing record, then do INSERT or UPDATE */
		private const int OE_Restrict=6;
		//#define OE_Restrict 6   /* OE_Abort for IMMEDIATE, OE_Rollback for DEFERRED */
		private const int OE_SetNull=7;
		//#define OE_SetNull  7   /* Set the foreign key value to NULL */
		private const int OE_SetDflt=8;
		//#define OE_SetDflt  8   /* Set the foreign key value to its default */
		private const int OE_Cascade=9;
		//#define OE_Cascade  9   /* Cascade the changes */
		private const int OE_Default=99;
		//#define OE_Default  99  /* Do whatever the default action is */
		///<summary>
		/// An instance of the following structure is passed as the first
		/// argument to sqlite3VdbeKeyCompare and is used to control the
		/// comparison of the two index keys.
		///
		///</summary>
		public class KeyInfo {
			public sqlite3 db;
			/* The database connection */public SqliteEncoding enc;
			/* Text encoding - one of the SQLITE_UTF* values */public u16 nField;
			/* Number of entries in aColl[] */public u8[] aSortOrder;
			///<summary>
			///Sort order for each column.  May be NULL
			///</summary>
			public CollSeq[] aColl=new CollSeq[1];
			/* Collating sequence for each term of the key */public KeyInfo Copy() {
				return (KeyInfo)MemberwiseClone();
			}
		};

		/*
    ** An instance of the following structure holds information about a
    ** single index record that has already been parsed out into individual
    ** values.
    **
    ** A record is an object that contains one or more fields of data.
    ** Records are used to store the content of a table row and to store
    ** the key of an index.  A blob encoding of a record is created by
    ** the OP_MakeRecord opcode of the VDBE and is disassembled by the
    ** OP_Column opcode.
    **
    ** This structure holds a record that has already been disassembled
    ** into its constituent fields.
    */public class UnpackedRecord {
			public KeyInfo pKeyInfo;
			/* Collation and sort-order information */public u16 nField;
			/* Number of entries in apMem[] */public u16 flags;
			/* Boolean settings.  UNPACKED_... below */public i64 rowid;
			/* Used by UNPACKED_PREFIX_SEARCH */public Mem[] aMem;
		/* Values */};

		/*
    ** Allowed values of UnpackedRecord.flags
    *///#define UNPACKED_NEED_FREE     0x0001  /* Memory is from sqlite3Malloc() */
		//#define UNPACKED_NEED_DESTROY  0x0002  /* apMem[]s should all be destroyed */
		//#define UNPACKED_IGNORE_ROWID  0x0004  /* Ignore trailing rowid on key1 */
		//#define UNPACKED_INCRKEY       0x0008  /* Make this key an epsilon larger */
		//#define UNPACKED_PREFIX_MATCH  0x0010  /* A prefix match is considered OK */
		//#define UNPACKED_PREFIX_SEARCH 0x0020  /* A prefix match is considered OK */
		private const int UNPACKED_NEED_FREE=0x0001;
		/* Memory is from sqlite3Malloc() */private const int UNPACKED_NEED_DESTROY=0x0002;
		/* apMem[]s should all be destroyed */private const int UNPACKED_IGNORE_ROWID=0x0004;
		/* Ignore trailing rowid on key1 */private const int UNPACKED_INCRKEY=0x0008;
		/* Make this key an epsilon larger */private const int UNPACKED_PREFIX_MATCH=0x0010;
		/* A prefix match is considered OK */private const int UNPACKED_PREFIX_SEARCH=0x0020;
		///<summary>
		///A prefix match is considered OK
		///</summary>
		///<summary>
		/// Each SQL index is represented in memory by an
		/// instance of the following structure.
		///
		/// The columns of the table that are to be indexed are described
		/// by the aiColumn[] field of this structure.  For example, suppose
		/// we have the following table and index:
		///
		///     CREATE TABLE Ex1(c1 int, c2 int, c3 text);
		///     CREATE INDEX Ex2 ON Ex1(c3,c1);
		///
		/// In the Table structure describing Ex1, nCol==3 because there are
		/// three columns in the table.  In the Index structure describing
		/// Ex2, nColumn==2 since 2 of the 3 columns of Ex1 are indexed.
		/// The value of aiColumn is {2, 0}.  aiColumn[0]==2 because the
		/// first column to be indexed (c3) has an index of 2 in Ex1.aCol[].
		/// The second column to be indexed (c1) has an index of 0 in
		/// Ex1.aCol[], hence Ex2.aiColumn[1]==0.
		///
		/// The Index.onError field determines whether or not the indexed columns
		/// must be unique and what to do if they are not.  When Index.onError=OE_None,
		/// it means this is not a unique index.  Otherwise it is a unique index
		/// and the value of Index.onError indicate the which conflict resolution
		/// algorithm to employ whenever an attempt is made to insert a non-unique
		/// element.
		///
		///</summary>
		public class Index {
			public string zName;
			/* Name of this index */public int nColumn;
			/* Number of columns in the table used by this index */public int[] aiColumn;
			/* Which columns are used by this index.  1st is 0 */public int[] aiRowEst;
			/* Result of ANALYZE: Est. rows selected by each column */public Table pTable;
			/* The SQL table being indexed */public int tnum;
			/* Page containing root of this index in database file */public u8 onError;
			/* OE_Abort, OE_Ignore, OE_Replace, or OE_None */public u8 autoIndex;
			/* True if is automatically created (ex: by UNIQUE) */public u8 bUnordered;
			/* Use this index for == or IN queries only */public string zColAff;
			/* String defining the affinity of each column */public Index pNext;
			/* The next index associated with the same table */public Schema pSchema;
			/* Schema containing this index */public u8[] aSortOrder;
			/* Array of size Index.nColumn. True==DESC, False==ASC */public string[] azColl;
			///<summary>
			///Array of collation sequence names for index
			///</summary>
			public IndexSample[] aSample;
			/* Array of SQLITE_INDEX_SAMPLES samples */public Index Copy() {
				if(this==null)
					return null;
				else {
					Index cp=(Index)MemberwiseClone();
					return cp;
				}
			}
		};

		///<summary>
		/// Each sample stored in the sqlite_stat2 table is represented in memory
		/// using a structure of this type.
		///
		///</summary>
		public class IndexSample {
			public struct _u {
				//union {
				public string z;
				/* Value if eType is SQLITE_TEXT */public byte[] zBLOB;
				/* Value if eType is SQLITE_BLOB */public double r;
			/* Value if eType is SQLITE_FLOAT or SQLITE_INTEGER */}
			public _u u;
			public u8 eType;
			/* SQLITE_NULL, SQLITE_INTEGER ... etc. */public u8 nByte;
		/* Size in byte of text or blob. */};

		///<summary>
		/// An instance of this structure contains information needed to generate
		/// code for a SELECT that contains aggregate functions.
		///
		/// If Expr.op==TK_AGG_COLUMN or TK_AGG_FUNCTION then Expr.pAggInfo is a
		/// pointer to this structure.  The Expr.iColumn field is the index in
		/// AggInfo.aCol[] or AggInfo.aFunc[] of information needed to generate
		/// code for that node.
		///
		/// AggInfo.pGroupBy and AggInfo.aFunc.pExpr point to fields within the
		/// original Select structure that describes the SELECT statement.  These
		/// fields do not need to be freed when deallocating the AggInfo structure.
		///
		///</summary>
		public class AggInfo_col {
			/* For each column used in source tables */public Table pTab;
			/* Source table */public int iTable;
			/* VdbeCursor number of the source table */public int iColumn;
			/* Column number within the source table */public int iSorterColumn;
			/* Column number in the sorting index */public int iMem;
			/* Memory location that acts as accumulator */public Expr pExpr;
		/* The original expression */};

		public class AggInfo_func {
			/* For each aggregate function */public Expr pExpr;
			/* Expression encoding the function */public FuncDef pFunc;
			/* The aggregate function implementation */public int iMem;
			/* Memory location that acts as accumulator */public int iDistinct;
		/* Ephemeral table used to enforce DISTINCT */}
		public class AggInfo {
			public u8 directMode;
			/* Direct rendering mode means take data directly
** from source tables rather than from accumulators */public u8 useSortingIdx;
			/* In direct mode, reference the sorting index rather
** than the source table */public int sortingIdx;
			/* VdbeCursor number of the sorting index */public ExprList pGroupBy;
			/* The group by clause */public int nSortingColumn;
			/* Number of columns in the sorting index */public AggInfo_col[] aCol;
			public int nColumn;
			/* Number of used entries in aCol[] */public int nColumnAlloc;
			/* Number of slots allocated for aCol[] */public int nAccumulator;
			/* Number of columns that show through to the output.
** Additional columns are used only as parameters to
** aggregate functions */public AggInfo_func[] aFunc;
			public int nFunc;
			///<summary>
			///Number of entries in aFunc[]
			///</summary>
			public int nFuncAlloc;
			/* Number of slots allocated for aFunc[] */public AggInfo Copy() {
				if(this==null)
					return null;
				else {
					AggInfo cp=(AggInfo)MemberwiseClone();
					if(pGroupBy!=null)
						cp.pGroupBy=pGroupBy.Copy();
					return cp;
				}
			}
		};

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

		/**
    ** Each node of an expression in the parse tree is an instance
    ** of this structure.
    **
    ** Expr.op is the opcode.  The integer parser token codes are reused
    ** as opcodes here.  For example, the parser defines TK_GE to be an integer
    ** code representing the ">=" operator.  This same integer code is reused
    ** to represent the greater-than-or-equal-to operator in the expression
    ** tree.
    **
    ** If the expression is an SQL literal (TK_INTEGER, TK_FLOAT, TK_BLOB,
    ** or TK_STRING), then Expr.token contains the text of the SQL literal. If
    ** the expression is a variable (TK_VARIABLE), then Expr.token contains the
    ** variable name. Finally, if the expression is an SQL function (TK_FUNCTION),
    ** then Expr.token contains the name of the function.
    **
    ** Expr.pRight and Expr.pLeft are the left and right subexpressions of a
    ** binary operator. Either or both may be NULL.
    **
    ** Expr.x.pList is a list of arguments if the expression is an SQL function,
    ** a CASE expression or an IN expression of the form "<lhs> IN (<y>, <z>...)".
    ** Expr.x.pSelect is used if the expression is a sub-select or an expression of
    ** the form "<lhs> IN (SELECT ...)". If the EP_xIsSelect bit is set in the
    ** Expr.flags mask, then Expr.x.pSelect is valid. Otherwise, Expr.x.pList is
    ** valid.
    **
    ** An expression of the form ID or ID.ID refers to a column in a table.
    ** For such expressions, Expr.op is set to TK_COLUMN and Expr.iTable is
    ** the integer cursor number of a VDBE cursor pointing to that table and
    ** Expr.iColumn is the column number for the specific column.  If the
    ** expression is used as a result in an aggregate SELECT, then the
    ** value is also stored in the Expr.iAgg column in the aggregate so that
    ** it can be accessed after all aggregates are computed.
    **
    ** If the expression is an unbound variable marker (a question mark
    ** character '?' in the original SQL) then the Expr.iTable holds the index
    ** number for that variable.
    **
    ** If the expression is a subquery then Expr.iColumn holds an integer
    ** register number containing the result of the subquery.  If the
    ** subquery gives a constant result, then iTable is -1.  If the subquery
    ** gives a different answer at different times during statement processing
    ** then iTable is the address of a subroutine that computes the subquery.
    **
    ** If the Expr is of type OP_Column, and the table it is selecting from
    ** is a disk table or the "old.*" pseudo-table, then pTab points to the
    ** corresponding table definition.
    **
    ** ALLOCATION NOTES:
    **
    ** Expr objects can use a lot of memory space in database schema.  To
    ** help reduce memory requirements, sometimes an Expr object will be
    ** truncated.  And to reduce the number of memory allocations, sometimes
    ** two or more Expr objects will be stored in a single memory allocation,
    ** together with Expr.zToken strings.
    **
    ** If the EP_Reduced and EP_TokenOnly flags are set when
    ** an Expr object is truncated.  When EP_Reduced is set, then all
    ** the child Expr objects in the Expr.pLeft and Expr.pRight subtrees
    ** are contained within the same memory allocation.  Note, however, that
    ** the subtrees in Expr.x.pList or Expr.x.pSelect are always separately
    ** allocated, regardless of whether or not EP_Reduced is set.
    */public class Expr {
          public Expr()
          {

          }
			#if DEBUG_CLASS_EXPR || DEBUG_CLASS_ALL
																																																																																				public u8 _op;                      /* Operation performed by this node */
public u8 op
{
get { return _op; }
set { _op = value; }
}
#else
          TokenType _op;

          public TokenType Operator
          {
              get { return _op; }
              set { _op = value; }
          }
          public u8 op
          {
              get { return(u8) _op; }
              set { _op = (TokenType)value; }
          }
			/* Operation performed by this node */
			#endif
			public char affinity;
			///<summary>
			///The affinity of the column or 0 if not a column
			///</summary>
			#if DEBUG_CLASS_EXPR || DEBUG_CLASS_ALL
																																																																																				public u16 _flags;                            /* Various flags.  EP_* See below */
public u16 flags
{
get { return _flags; }
set { _flags = value; }
}
public struct _u
{
public string _zToken;         /* Token value. Zero terminated and dequoted */
public string zToken
{
get { return _zToken; }
set { _zToken = value; }
}
public int iValue;            /* Non-negative integer value if EP_IntValue */
}

#else
			public struct _u {
				public string zToken;
				/* Token value. Zero terminated and dequoted */public int iValue;
			/* Non-negative integer value if EP_IntValue */}
			public u16 flags;
			/* Various flags.  EP_* See below */
			#endif
			public _u u;
			/* If the EP_TokenOnly flag is set in the Expr.flags mask, then no
      ** space is allocated for the fields below this point. An attempt to
      ** access them will result in a segfault or malfunction.
      *********************************************************************/public Expr pLeft;
			///<summary>
			///Left subnode
			///</summary>
			public Expr pRight;
			/* Right subnode */public struct _x {
				public ExprList pList;
				/* Function arguments or in "<expr> IN (<expr-list)" */public Select pSelect;
			/* Used for sub-selects and "<expr> IN (<select>)" */}
			public _x x;
			public CollSeq pColl;
			/* The collation type of the column or 0 *//* If the EP_Reduced flag is set in the Expr.flags mask, then no
      ** space is allocated for the fields below this point. An attempt to
      ** access them will result in a segfault or malfunction.
      *********************************************************************/public int iTable;
			/* TK_COLUMN: cursor number of table holding column
  ** TK_REGISTER: register number
  ** TK_TRIGGER: 1 -> new, 0 -> old */public ynVar iColumn;
			/* TK_COLUMN: column index.  -1 for rowid.
  ** TK_VARIABLE: variable number (always >= 1). */public i16 iAgg;
			/* Which entry in pAggInfo->aCol[] or ->aFunc[] */public i16 iRightJoinTable;
			/* If EP_FromJoin, the right table of the join */public u8 flags2;
			/* Second set of flags.  EP2_... */public u8 op2;
			/* If a TK_REGISTER, the original value of Expr.op */public AggInfo pAggInfo;
			/* Used by TK_AGG_COLUMN and TK_AGG_FUNCTION */public Table pTab;
			/* Table for TK_COLUMN expressions. */
			#if SQLITE_MAX_EXPR_DEPTH
			public int nHeight;
			/* Height of the tree headed by this node */public Table pZombieTab;
			///<summary>
			///List of Table objects to delete after code gen
			///</summary>
			#endif
			#if DEBUG_CLASS
																																																																																				public int op
{
get { return _op; }
set { _op = value; }
}
#endif
			public void CopyFrom(Expr cf) {
				op=cf.op;
				affinity=cf.affinity;
				flags=cf.flags;
				u=cf.u;
				pColl=cf.pColl==null?null:cf.pColl.Copy();
				iTable=cf.iTable;
				iColumn=cf.iColumn;
				pAggInfo=cf.pAggInfo==null?null:cf.pAggInfo.Copy();
				iAgg=cf.iAgg;
				iRightJoinTable=cf.iRightJoinTable;
				flags2=cf.flags2;
				pTab=cf.pTab==null?null:cf.pTab;
				#if SQLITE_TEST || SQLITE_MAX_EXPR_DEPTH
				nHeight=cf.nHeight;
				pZombieTab=cf.pZombieTab;
				#endif
				pLeft=cf.pLeft==null?null:cf.pLeft.Copy();
				pRight=cf.pRight==null?null:cf.pRight.Copy();
				x.pList=cf.x.pList==null?null:cf.x.pList.Copy();
				x.pSelect=cf.x.pSelect==null?null:cf.x.pSelect.Copy();
			}
			public Expr Copy() {
				if(this==null)
					return null;
				else
					return Copy(flags);
			}
			public Expr Copy(int flag) {
				Expr cp=new Expr();
				cp.op=op;
				cp.affinity=affinity;
				cp.flags=flags;
				cp.u=u;
				if((flag&EP_TokenOnly)!=0)
					return cp;
				if(pLeft!=null)
					cp.pLeft=pLeft.Copy();
				if(pRight!=null)
					cp.pRight=pRight.Copy();
				cp.x=x;
				cp.pColl=pColl;
				if((flag&EP_Reduced)!=0)
					return cp;
				cp.iTable=iTable;
				cp.iColumn=iColumn;
				cp.iAgg=iAgg;
				cp.iRightJoinTable=iRightJoinTable;
				cp.flags2=flags2;
				cp.op2=op2;
				cp.pAggInfo=pAggInfo;
				cp.pTab=pTab;
				#if SQLITE_MAX_EXPR_DEPTH
				cp.nHeight=nHeight;
				cp.pZombieTab=pZombieTab;
				#endif
				return cp;
			}
			public///<summary>
			/// pExpr is an operand of a comparison operator.  aff2 is the
			/// type affinity of the other operand.  This routine returns the
			/// type affinity that should be used for the comparison operator.
			///
			///</summary>
			char sqlite3CompareAffinity(char aff2) {
				char aff1=this.sqlite3ExprAffinity();
				if(aff1!='\0'&&aff2!='\0') {
					/* Both sides of the comparison are columns. If one has numeric
        ** affinity, use that. Otherwise use no affinity.
        */if(aff1>=SQLITE_AFF_NUMERIC||aff2>=SQLITE_AFF_NUMERIC)//        if (sqlite3IsNumericAffinity(aff1) || sqlite3IsNumericAffinity(aff2))
					 {
						return SQLITE_AFF_NUMERIC;
					}
					else {
						return SQLITE_AFF_NONE;
					}
				}
				else
					if(aff1=='\0'&&aff2=='\0') {
						/* Neither side of the comparison is a column.  Compare the
        ** results directly.
        */return SQLITE_AFF_NONE;
					}
					else {
						/* One side is a column, the other is not. Use the columns affinity. */Debug.Assert(aff1==0||aff2==0);
						return (aff1!='\0'?aff1:aff2);
					}
			}
			public///<summary>
			/// pExpr is a comparison operator.  Return the type affinity that should
			/// be applied to both operands prior to doing the comparison.
			///
			///</summary>
			char comparisonAffinity() {
				char aff;
				Debug.Assert(this.op==TK_EQ||this.op==TK_IN||this.op==TK_LT||this.op==TK_GT||this.op==TK_GE||this.op==TK_LE||this.op==TK_NE||this.op==TK_IS||this.op==TK_ISNOT);
				Debug.Assert(this.pLeft!=null);
				aff=this.pLeft.sqlite3ExprAffinity();
				if(this.pRight!=null) {
					aff=this.pRight.sqlite3CompareAffinity(aff);
				}
				else
					if(ExprHasProperty(this,EP_xIsSelect)) {
						aff=this.x.pSelect.pEList.a[0].pExpr.sqlite3CompareAffinity(aff);
					}
					else
						if(aff=='\0') {
							aff=SQLITE_AFF_NONE;
						}
				return aff;
			}
			public///<summary>
			/// pExpr is a comparison expression, eg. '=', '<', IN(...) etc.
			/// idx_affinity is the affinity of an indexed column. Return true
			/// if the index with affinity idx_affinity may be used to implement
			/// the comparison in pExpr.
			///
			///</summary>
			bool sqlite3IndexAffinityOk(char idx_affinity) {
				char aff=this.comparisonAffinity();
				switch(aff) {
				case SQLITE_AFF_NONE:
				return true;
				case SQLITE_AFF_TEXT:
				return idx_affinity==SQLITE_AFF_TEXT;
				default:
				return idx_affinity>=SQLITE_AFF_NUMERIC;
				// sqlite3IsNumericAffinity(idx_affinity);
				}
			}
			public///<summary>
			/// The dupedExpr*Size() routines each return the number of bytes required
			/// to store a copy of an expression or expression tree.  They differ in
			/// how much of the tree is measured.
			///
			///     dupedExprStructSize()     Size of only the Expr structure
			///     dupedExprNodeSize()       Size of Expr + space for token
			///     dupedExprSize()           Expr + token + subtree components
			///
			///
			///
			/// The dupedExprStructSize() function returns two values OR-ed together:
			/// (1) the space required for a copy of the Expr structure only and
			/// (2) the EP_xxx flags that indicate what the structure size should be.
			/// The return values is always one of:
			///
			///      EXPR_FULLSIZE
			///      EXPR_REDUCEDSIZE   | EP_Reduced
			///      EXPR_TOKENONLYSIZE | EP_TokenOnly
			///
			/// The size of the structure can be found by masking the return value
			/// of this routine with 0xfff.  The flags can be found by masking the
			/// return value with EP_Reduced|EP_TokenOnly.
			///
			/// Note that with flags==EXPRDUP_REDUCE, this routines works on full-size
			/// (unreduced) Expr objects as they or originally constructed by the parser.
			/// During expression analysis, extra information is computed and moved into
			/// later parts of teh Expr object and that extra information might get chopped
			/// off if the expression is reduced.  Note also that it does not work to
			/// make a EXPRDUP_REDUCE copy of a reduced expression.  It is only legal
			/// to reduce a pristine expression tree from the parser.  The implementation
			/// of dupedExprStructSize() contain multiple Debug.Assert() statements that attempt
			/// to enforce this constraint.
			///
			///</summary>
			int dupedExprStructSize(int flags) {
				int nSize;
				Debug.Assert(flags==EXPRDUP_REDUCE||flags==0);
				/* Only one flag value allowed */if(0==(flags&EXPRDUP_REDUCE)) {
					nSize=EXPR_FULLSIZE;
				}
				else {
					Debug.Assert(!ExprHasAnyProperty(this,EP_TokenOnly|EP_Reduced));
					Debug.Assert(!ExprHasProperty(this,EP_FromJoin));
					Debug.Assert((this.flags2&EP2_MallocedToken)==0);
					Debug.Assert((this.flags2&EP2_Irreducible)==0);
					if(this.pLeft!=null||this.pRight!=null||this.pColl!=null||this.x.pList!=null||this.x.pSelect!=null) {
						nSize=EXPR_REDUCEDSIZE|EP_Reduced;
					}
					else {
						nSize=EXPR_TOKENONLYSIZE|EP_TokenOnly;
					}
				}
				return nSize;
			}
			public///<summary>
			/// Return the number of bytes required to create a duplicate of the
			/// expression passed as the first argument. The second argument is a
			/// mask containing EXPRDUP_XXX flags.
			///
			/// The value returned includes space to create a copy of the Expr struct
			/// itself and the buffer referred to by Expr.u.zToken, if any.
			///
			/// If the EXPRDUP_REDUCE flag is set, then the return value includes
			/// space to duplicate all Expr nodes in the tree formed by Expr.pLeft
			/// and Expr.pRight variables (but not for any structures pointed to or
			/// descended from the Expr.x.pList or Expr.x.pSelect variables).
			///
			///</summary>
			int dupedExprSize(int flags) {
				int nByte=0;
				if(this!=null) {
					nByte=this.dupedExprNodeSize(flags);
					if((flags&EXPRDUP_REDUCE)!=0) {
						nByte+=this.pLeft.dupedExprSize(flags)+this.pRight.dupedExprSize(flags);
					}
				}
				return nByte;
			}
			public///<summary>
			/// This function returns the space in bytes required to store the copy
			/// of the Expr structure and a copy of the Expr.u.zToken string (if that
			/// string is defined.)
			///
			///</summary>
			int dupedExprNodeSize(int flags) {
				int nByte=this.dupedExprStructSize(flags)&0xfff;
				if(!ExprHasProperty(this,EP_IntValue)&&this.u.zToken!=null) {
					nByte+=StringExtensions.sqlite3Strlen30(this.u.zToken)+1;
				}
				return ROUND8(nByte);
			}
			public int exprIsConst(int initFlag) {
				Walker w=new Walker();
				w.u.i=initFlag;
				w.xExprCallback=exprNodeIsConstant;
				w.xSelectCallback=selectNodeIsConstant;
				Expr _this=this;
				w.sqlite3WalkExpr(ref _this);
				return w.u.i;
			}
			public///<summary>
			/// Walk an expression tree.  Return 1 if the expression is constant
			/// and 0 if it involves variables or function calls.
			///
			/// For the purposes of this function, a double-quoted string (ex: "abc")
			/// is considered a variable but a single-quoted string (ex: 'abc') is
			/// a constant.
			///
			///</summary>
			int sqlite3ExprIsConstant() {
				return this.exprIsConst(1);
			}
			public///<summary>
			/// Walk an expression tree.  Return 1 if the expression is constant
			/// that does no originate from the ON or USING clauses of a join.
			/// Return 0 if it involves variables or function calls or terms from
			/// an ON or USING clause.
			///
			///</summary>
			int sqlite3ExprIsConstantNotJoin() {
				return this.exprIsConst(3);
			}
			public///<summary>
			/// Walk an expression tree.  Return 1 if the expression is constant
			/// or a function call with constant arguments.  Return and 0 if there
			/// are any variables.
			///
			/// For the purposes of this function, a double-quoted string (ex: "abc")
			/// is considered a variable but a single-quoted string (ex: 'abc') is
			/// a constant.
			///
			///</summary>
			int sqlite3ExprIsConstantOrFunction() {
				return this.exprIsConst(2);
			}
			public///<summary>
			/// If the expression p codes a constant integer that is small enough
			/// to fit in a 32-bit integer, return 1 and put the value of the integer
			/// in pValue.  If the expression is not an integer or if it is too big
			/// to fit in a signed 32-bit integer, return 0 and leave pValue unchanged.
			///
			///</summary>
			int sqlite3ExprIsInteger(ref int pValue) {
				int rc=0;
				/* If an expression is an integer literal that fits in a signed 32-bit
      ** integer, then the EP_IntValue flag will have already been set */Debug.Assert(this.op!=TK_INTEGER||(this.flags&EP_IntValue)!=0||!Converter.sqlite3GetInt32(this.u.zToken,ref rc));
				if((this.flags&EP_IntValue)!=0) {
					pValue=(int)this.u.iValue;
					return 1;
				}
				switch(this.op) {
				case TK_UPLUS: {
					rc=this.pLeft.sqlite3ExprIsInteger(ref pValue);
					break;
				}
				case TK_UMINUS: {
					int v=0;
					if(this.pLeft.sqlite3ExprIsInteger(ref v)!=0) {
						pValue=-v;
						rc=1;
					}
					break;
				}
				default:
				break;
				}
				return rc;
			}
			public///<summary>
			/// Return FALSE if there is no chance that the expression can be NULL.
			///
			/// If the expression might be NULL or if the expression is too complex
			/// to tell return TRUE.
			///
			/// This routine is used as an optimization, to skip OP_IsNull opcodes
			/// when we know that a value cannot be NULL.  Hence, a false positive
			/// (returning TRUE when in fact the expression can never be NULL) might
			/// be a small performance hit but is otherwise harmless.  On the other
			/// hand, a false negative (returning FALSE when the result could be NULL)
			/// will likely result in an incorrect answer.  So when in doubt, return
			/// TRUE.
			///
			///</summary>
			int sqlite3ExprCanBeNull() {
				u8 op;
				Expr expr=this;
				while(expr.op==TK_UPLUS||expr.op==TK_UMINUS) {
					expr=expr.pLeft;
				}
				op=expr.op;
				if(op==TK_REGISTER)
					op=expr.op2;
				switch(op) {
				case TK_INTEGER:
				case TK_STRING:
				case TK_FLOAT:
				case TK_BLOB:
				return 0;
				default:
				return 1;
				}
			}
			public///<summary>
			/// Return TRUE if pExpr is an constant expression that is appropriate
			/// for factoring out of a loop.  Appropriate expressions are:
			///
			///    *  Any expression that evaluates to two or more opcodes.
			///
			///    *  Any OP_Integer, OP_Real, OP_String, OP_Blob, OP_Null,
			///       or OP_Variable that does not need to be placed in a
			///       specific register.
			///
			/// There is no point in factoring out single-instruction constant
			/// expressions that need to be placed in a particular register.
			/// We could factor them out, but then we would end up adding an
			/// OP_SCopy instruction to move the value into the correct register
			/// later.  We might as well just use the original instruction and
			/// avoid the OP_SCopy.
			///
			///</summary>
			int isAppropriateForFactoring() {
				Expr expr=this;
				if(expr.sqlite3ExprIsConstantNotJoin()==0) {
					return 0;
					/* Only constant expressions are appropriate for factoring */}
				if((expr.flags&EP_FixedDest)==0) {
					return 1;
					/* Any constant without a fixed destination is appropriate */}
				while(expr.op==TK_UPLUS)
					expr=expr.pLeft;
				switch(expr.op) {
				#if !SQLITE_OMIT_BLOB_LITERAL
				case TK_BLOB:
				#endif
				case TK_VARIABLE:
				case TK_INTEGER:
				case TK_FLOAT:
				case TK_NULL:
				case TK_STRING: {
					testcase(expr.op==TK_BLOB);
					testcase(expr.op==TK_VARIABLE);
					testcase(expr.op==TK_INTEGER);
					testcase(expr.op==TK_FLOAT);
					testcase(expr.op==TK_NULL);
					testcase(expr.op==TK_STRING);
					/* Single-instruction constants with a fixed destination are
            ** better done in-line.  If we factor them, they will just end
            ** up generating an OP_SCopy to move the value to the destination
            ** register. */return 0;
				}
				case TK_UMINUS: {
					if(expr.pLeft.op==TK_FLOAT||expr.pLeft.op==TK_INTEGER) {
						return 0;
					}
					break;
				}
				default: {
					break;
				}
				}
				return 1;
			}
			public char sqlite3ExprAffinity() {
				int op=this.op;
				if(op==TK_SELECT) {
					Debug.Assert((this.flags&EP_xIsSelect)!=0);
					return this.x.pSelect.pEList.a[0].pExpr.sqlite3ExprAffinity();
				}
				#if !SQLITE_OMIT_CAST
				if(op==TK_CAST) {
					Debug.Assert(!ExprHasProperty(this,EP_IntValue));
					return sqlite3AffinityType(this.u.zToken);
				}
				#endif
				if((op==TK_AGG_COLUMN||op==TK_COLUMN||op==TK_REGISTER)&&this.pTab!=null) {
					/* op==TK_REGISTER && pExpr.pTab!=0 happens when pExpr was originally
        ** a TK_COLUMN but was previously evaluated and cached in a register */int j=this.iColumn;
					if(j<0)
						return SQLITE_AFF_INTEGER;
					Debug.Assert(this.pTab!=null&&j<this.pTab.nCol);
					return this.pTab.aCol[j].affinity;
				}
				return this.affinity;
			}
			public Expr sqlite3ExprSetColl(CollSeq pColl) {
				if(this!=null&&pColl!=null) {
					this.pColl=pColl;
					this.flags|=EP_ExpCollate;
				}
				return this;
			}
			public u8 binaryCompareP5(Expr pExpr2,int jumpIfNull) {
				u8 aff=(u8)pExpr2.sqlite3ExprAffinity();
				aff=(u8)((u8)this.sqlite3CompareAffinity((char)aff)|(u8)jumpIfNull);
				return aff;
			}
			public void exprSetHeight() {
				int nHeight=0;
				this.pLeft.heightOfExpr(ref nHeight);
				this.pRight.heightOfExpr(ref nHeight);
				if(ExprHasProperty(this,EP_xIsSelect)) {
					this.x.pSelect.heightOfSelect(ref nHeight);
				}
				else {
					this.x.pList.heightOfExprList(ref nHeight);
				}
				this.nHeight=nHeight+1;
			}
			public int isMatchOfColumn(/* Test this expression */) {
				ExprList pList;
				if(this.op!=TK_FUNCTION) {
					return 0;
				}
				if(!this.u.zToken.Equals("match",StringComparison.InvariantCultureIgnoreCase)) {
					return 0;
				}
				pList=this.x.pList;
				if(pList.nExpr!=2) {
					return 0;
				}
				if(pList.a[1].pExpr.op!=TK_COLUMN) {
					return 0;
				}
				return 1;
			}
			public void transferJoinMarkings(Expr pBase) {
				this.flags=(u16)(this.flags|pBase.flags&EP_FromJoin);
				this.iRightJoinTable=pBase.iRightJoinTable;
			}

            public TokenType Operator2 { get { return (TokenType)op2; } set { op2 = (u8)value; } }
      }
		/*
    ** The following are the meanings of bits in the Expr.flags field.
    *///#define EP_FromJoin   0x0001  /* Originated in ON or USING clause of a join */
		//#define EP_Agg        0x0002  /* Contains one or more aggregate functions */
		//#define EP_Resolved   0x0004  /* IDs have been resolved to COLUMNs */
		//#define EP_Error      0x0008  /* Expression contains one or more errors */
		//#define EP_Distinct   0x0010  /* Aggregate function with DISTINCT keyword */
		//#define EP_VarSelect  0x0020  /* pSelect is correlated, not constant */
		//#define EP_DblQuoted  0x0040  /* token.z was originally in "..." */
		//#define EP_InfixFunc  0x0080  /* True for an infix function: LIKE, GLOB, etc */
		//#define EP_ExpCollate 0x0100  /* Collating sequence specified explicitly */
		//#define EP_FixedDest  0x0200  /* Result needed in a specific register */
		//#define EP_IntValue   0x0400  /* Integer value contained in u.iValue */
		//#define EP_xIsSelect  0x0800  /* x.pSelect is valid (otherwise x.pList is) */
		//#define EP_Reduced    0x1000  /* Expr struct is EXPR_REDUCEDSIZE bytes only */
		//#define EP_TokenOnly  0x2000  /* Expr struct is EXPR_TOKENONLYSIZE bytes only */
		//#define EP_Static     0x4000  /* Held in memory not obtained from malloc() */
		private const ushort EP_FromJoin=0x0001;
		private const ushort EP_Agg=0x0002;
		private const ushort EP_Resolved=0x0004;
		private const ushort EP_Error=0x0008;
		private const ushort EP_Distinct=0x0010;
		private const ushort EP_VarSelect=0x0020;
		private const ushort EP_DblQuoted=0x0040;
		private const ushort EP_InfixFunc=0x0080;
		private const ushort EP_ExpCollate=0x0100;
		private const ushort EP_FixedDest=0x0200;
		private const ushort EP_IntValue=0x0400;
		private const ushort EP_xIsSelect=0x0800;
		private const ushort EP_Reduced=0x1000;
		private const ushort EP_TokenOnly=0x2000;
		private const ushort EP_Static=0x4000;
		/*
    ** The following are the meanings of bits in the Expr.flags2 field.
    *///#define EP2_MallocedToken  0x0001  /* Need to sqlite3DbFree() Expr.zToken */
		//#define EP2_Irreducible    0x0002  /* Cannot EXPRDUP_REDUCE this Expr */
		private const u8 EP2_MallocedToken=0x0001;
		private const u8 EP2_Irreducible=0x0002;
		///<summary>
		/// The pseudo-routine sqlite3ExprSetIrreducible sets the EP2_Irreducible
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
		private static void ExprSetIrreducible(Expr X) {
		}
		#endif
		///<summary>
		/// These macros can be used to test, set, or clear bits in the
		/// Expr.flags field.
		///</summary>
		//#define ExprHasProperty(E,P)     (((E)->flags&(P))==(P))
		private static bool ExprHasProperty(Expr E,int P) {
			return (E.flags&P)==P;
		}
		//#define ExprHasAnyProperty(E,P)  (((E)->flags&(P))!=0)
		private static bool ExprHasAnyProperty(Expr E,int P) {
			return (E.flags&P)!=0;
		}
		//#define ExprSetProperty(E,P)     (E)->flags|=(P)
		private static void ExprSetProperty(Expr E,int P) {
			E.flags=(ushort)(E.flags|P);
		}
		//#define ExprClearProperty(E,P)   (E)->flags&=~(P)
		private static void ExprClearProperty(Expr E,int P) {
			E.flags=(ushort)(E.flags&~P);
		}
		/*
    ** Macros to determine the number of bytes required by a normal Expr
    ** struct, an Expr struct with the EP_Reduced flag set in Expr.flags
    ** and an Expr struct with the EP_TokenOnly flag set.
    *///#define EXPR_FULLSIZE           sizeof(Expr)           /* Full size */
		//#define EXPR_REDUCEDSIZE        offsetof(Expr,iTable)  /* Common features */
		//#define EXPR_TOKENONLYSIZE      offsetof(Expr,pLeft)   /* Fewer features */
		// We don't use these in C#, but define them anyway,
		private const int EXPR_FULLSIZE=48;
		private const int EXPR_REDUCEDSIZE=24;
		private const int EXPR_TOKENONLYSIZE=8;
		///<summary>
		/// Flags passed to the sqlite3ExprDup() function. See the header comment
		/// above sqlite3ExprDup() for details.
		///
		///</summary>
		//#define EXPRDUP_REDUCE         0x0001  /* Used reduced-size Expr nodes */
		private const int EXPRDUP_REDUCE=0x0001;
		///<summary>
		/// A list of expressions.  Each expression may optionally have a
		/// name.  An expr/name combination can be used in several ways, such
		/// as the list of "expr AS ID" fields following a "SELECT" or in the
		/// list of "ID = expr" items in an UPDATE.  A list of expressions can
		/// also be used as the argument to a function, in which case the a.zName
		/// field is not used.
		///
		///</summary>
		public class ExprList_item {
			public Expr pExpr;
			/* The list of expressions */public string zName;
			/* Token associated with this expression */public string zSpan;
			/*  Original text of the expression */public u8 sortOrder;
			/* 1 for DESC or 0 for ASC */public u8 done;
			/* A flag to indicate when processing is finished */public u16 iCol;
			/* For ORDER BY, column number in result set */public u16 iAlias;
		/* Index into Parse.aAlias[] for zName */}
		public class ExprList {
			public int nExpr;
			/* Number of expressions on the list */public int nAlloc;
			/* Number of entries allocated below */public int iECursor;
			///<summary>
			///VDBE VdbeCursor associated with this ExprList
			///</summary>
			public ExprList_item[] a;
			/* One entry for each expression */public ExprList Copy() {
				if(this==null)
					return null;
				else {
					ExprList cp=(ExprList)MemberwiseClone();
					a.CopyTo(cp.a,0);
					return cp;
				}
			}
			public bool referencesOtherTables(/* Search expressions in ths list */WhereMaskSet pMaskSet,/* Mapping from tables to bitmaps */int iFirst,/* Be searching with the iFirst-th expression */int iBase/* Ignore references to this table */) {
				Bitmask allowed=~pMaskSet.getMask(iBase);
				while(iFirst<this.nExpr) {
					if((pMaskSet.exprTableUsage(this.a[iFirst++].pExpr)&allowed)!=0) {
						return true;
					}
				}
				return false;
			}
		}
		///<summary>
		/// An instance of this structure is used by the parser to record both
		/// the parse tree for an expression and the span of input text for an
		/// expression.
		///
		///</summary>
		public class ExprSpan {
			public Expr pExpr;
			/* The expression parse tree */public string zStart;
			/* First character of input text */public string zEnd;
			/* One character past the end of input text */public void spanSet(Token pStart,Token pEnd) {
				this.zStart=pStart.zRestSql;
				this.zEnd=pEnd.zRestSql.Substring(pEnd.Length);
			}
			public void spanExpr(Parse pParse,int op,Token pValue) {
                Log.WriteLine(String.Empty);
                Log.WriteHeader("spanExpr");
                Log.WriteLine(pValue.zRestSql);
                Log.Indent();
				this.pExpr=pParse.sqlite3PExpr(op,0,0,pValue);
				this.zStart=pValue.zRestSql;
				this.zEnd=pValue.zRestSql.Substring(pValue.Length);
                Log.Unindent();
			}
			public void spanBinaryExpr(/* Write the result here */Parse pParse,/* The parsing context.  Errors accumulate here */int op,/* The binary operation */ExprSpan pLeft,/* The left operand */ExprSpan pRight/* The right operand */) {
				this.pExpr=pParse.sqlite3PExpr(op,pLeft.pExpr,pRight.pExpr,0);
				this.zStart=pLeft.zStart;
				this.zEnd=pRight.zEnd;
			}
			public void spanUnaryPostfix(/* Write the new expression node here */Parse pParse,/* Parsing context to record errors */int op,/* The operator */ExprSpan pOperand,/* The operand */Token pPostOp/* The operand token for setting the span */) {
				this.pExpr=pParse.sqlite3PExpr(op,pOperand.pExpr,0,0);
				this.zStart=pOperand.zStart;
				this.zEnd=pPostOp.zRestSql.Substring(pPostOp.Length);
			}
			public void spanUnaryPrefix(/* Write the new expression node here */Parse pParse,/* Parsing context to record errors */int op,/* The operator */ExprSpan pOperand,/* The operand */Token pPreOp/* The operand token for setting the span */) {
				this.pExpr=pParse.sqlite3PExpr(op,pOperand.pExpr,0,0);
				this.zStart=pPreOp.zRestSql;
				this.zEnd=pOperand.zEnd;
			}
		}
		///<summary>
		/// An instance of this structure can hold a simple list of identifiers,
		/// such as the list "a,b,c" in the following statements:
		///
		///      INSERT INTO t(a,b,c) VALUES ...;
		///      CREATE INDEX idx ON t(a,b,c);
		///      CREATE TRIGGER trig BEFORE UPDATE ON t(a,b,c) ...;
		///
		/// The IdList.a.idx field is used when the IdList represents the list of
		/// column names after a table name in an INSERT statement.  In the statement
		///
		///     INSERT INTO t(a,b,c) ...
		///
		/// If "a" is the k-th column of table "t", then IdList.a[0].idx==k.
		///
		///</summary>
		public class IdList_item {
			public string zName;
			/* Name of the identifier */public int idx;
		/* Index in some Table.aCol[] of a column named zName */}
		public class IdList {
			public IdList_item[] a;
			public int nId;
			///<summary>
			///Number of identifiers on the list
			///</summary>
			public int nAlloc;
			/* Number of entries allocated for a[] below */public IdList Copy() {
				if(this==null)
					return null;
				else {
					IdList cp=(IdList)MemberwiseClone();
					a.CopyTo(cp.a,0);
					return cp;
				}
			}
		};

		/*
    ** The bitmask datatype defined below is used for various optimizations.
    **
    ** Changing this from a 64-bit to a 32-bit type limits the number of
    ** tables in a join to 32 instead of 64.  But it also reduces the size
    ** of the library by 738 bytes on ix86.
    *///typedef u64 Bitmask;
		///<summary>
		/// The number of bits in a Bitmask.  "BMS" means "BitMask Size".
		///
		///</summary>
		//#define BMS  ((int)(sizeof(Bitmask)*8))
		private const int BMS=((int)(sizeof(Bitmask)*8));
		///<summary>
		/// The following structure describes the FROM clause of a SELECT statement.
		/// Each table or subquery in the FROM clause is a separate element of
		/// the SrcList.a[] array.
		///
		/// With the addition of multiple database support, the following structure
		/// can also be used to describe a particular table such as the table that
		/// is modified by an INSERT, DELETE, or UPDATE statement.  In standard SQL,
		/// such a table must be a simple name: ID.  But in SQLite, the table can
		/// now be identified by a database name, a dot, then the table name: ID.ID.
		///
		/// The jointype starts out showing the join type between the current table
		/// and the next table on the list.  The parser builds the list this way.
		/// But sqlite3SrcListShiftJoinType() later shifts the jointypes so that each
		/// jointype expresses the join between the table and the previous table.
		///
		/// In the colUsed field, the high-order bit (bit 63) is set if the table
		/// contains more than 63 columns and the 64-th or later column is used.
		///
		///</summary>
		public class SrcList_item {
			public string zDatabase;
			/* Name of database holding this table */public string zName;
			/* Name of the table */public string zAlias;
			/* The "B" part of a "A AS B" phrase.  zName is the "A" */public Table pTab;
			/* An SQL table corresponding to zName */public Select pSelect;
			/* A SELECT statement used in place of a table name */public u8 isPopulated;
			/* Temporary table associated with SELECT is populated */public u8 jointype;
			/* Type of join between this able and the previous */public u8 notIndexed;
			/* True if there is a NOT INDEXED clause */
			#if !SQLITE_OMIT_EXPLAIN
			public u8 iSelectId;
			/* If pSelect!=0, the id of the sub-select in EQP */
			#endif
			public int iCursor;
			/* The VDBE cursor number used to access this table */public Expr pOn;
			/* The ON clause of a join */public IdList pUsing;
			/* The USING clause of a join */public Bitmask colUsed;
			/* Bit N (1<<N) set if column N of pTab is used */public string zIndex;
			/* Identifier from "INDEXED BY <zIndex>" clause */public Index pIndex;
		/* Index structure corresponding to zIndex, if any */}
		public class SrcList {
			public i16 nSrc;
			/* Number of tables or subqueries in the FROM clause */public i16 nAlloc;
			///<summary>
			///Number of entries allocated in a[] below
			///</summary>
			public SrcList_item[] a;
			/* One entry for each identifier on the list */public SrcList Copy() {
				if(this==null)
					return null;
				else {
					SrcList cp=(SrcList)MemberwiseClone();
					if(a!=null)
						a.CopyTo(cp.a,0);
					return cp;
				}
			}
			public void exprAnalyzeAll(/* the FROM clause */WhereClause pWC/* the WHERE clause to be analyzed */) {
				int i;
				for(i=pWC.nTerm-1;i>=0;i--) {
					this.exprAnalyze(pWC,i);
				}
			}
			public void exprAnalyzeOrTerm(/* the FROM clause */WhereClause pWC,/* the complete WHERE clause */int idxTerm/* Index of the OR-term to be analyzed */) {
				Parse pParse=pWC.pParse;
				/* Parser context */sqlite3 db=pParse.db;
				/* Data_base connection */WhereTerm pTerm=pWC.a[idxTerm];
				/* The term to be analyzed */Expr pExpr=pTerm.pExpr;
				/* The expression of the term */WhereMaskSet pMaskSet=pWC.pMaskSet;
				/* Table use masks */int i;
				/* Loop counters */WhereClause pOrWc;
				/* Breakup of pTerm into subterms */WhereTerm pOrTerm;
				/* A Sub-term within the pOrWc */WhereOrInfo pOrInfo;
				/* Additional information Debug.Associated with pTerm */Bitmask chngToIN;
				/* Tables that might satisfy case 1 */Bitmask indexable;
				/* Tables that are indexable, satisfying case 2 *//*
      ** Break the OR clause into its separate subterms.  The subterms are
      ** stored in a WhereClause structure containing within the WhereOrInfo
      ** object that is attached to the original OR clause term.
      */Debug.Assert((pTerm.wtFlags&(TERM_DYNAMIC|TERM_ORINFO|TERM_ANDINFO))==0);
				Debug.Assert(pExpr.op==TK_OR);
				pTerm.u.pOrInfo=pOrInfo=new WhereOrInfo();
				//sqlite3DbMallocZero(db, sizeof(*pOrInfo));
				if(pOrInfo==null)
					return;
				pTerm.wtFlags|=TERM_ORINFO;
				pOrWc=pOrInfo.wc;
				pOrWc.whereClauseInit(pWC.pParse,pMaskSet);
                pOrWc.whereSplit(pExpr, TokenType.TK_OR);
				this.exprAnalyzeAll(pOrWc);
				//      if ( db.mallocFailed != 0 ) return;
				Debug.Assert(pOrWc.nTerm>=2);
				/*
      ** Compute the set of tables that might satisfy cases 1 or 2.
      */indexable=~(Bitmask)0;
				chngToIN=~(pWC.vmask);
				for(i=pOrWc.nTerm-1;i>=0&&indexable!=0;i--)//, pOrTerm++ )
				 {
					pOrTerm=pOrWc.a[i];
					if((pOrTerm.eOperator&WO_SINGLE)==0) {
						WhereAndInfo pAndInfo;
						Debug.Assert(pOrTerm.eOperator==0);
						Debug.Assert((pOrTerm.wtFlags&(TERM_ANDINFO|TERM_ORINFO))==0);
						chngToIN=0;
						pAndInfo=new WhereAndInfo();
						//sqlite3DbMallocRaw(db, sizeof(*pAndInfo));
						if(pAndInfo!=null) {
							WhereClause pAndWC;
							WhereTerm pAndTerm;
							int j;
							Bitmask b=0;
							pOrTerm.u.pAndInfo=pAndInfo;
							pOrTerm.wtFlags|=TERM_ANDINFO;
							pOrTerm.eOperator=WO_AND;
							pAndWC=pAndInfo.wc;
							pAndWC.whereClauseInit(pWC.pParse,pMaskSet);
                            pAndWC.whereSplit(pOrTerm.pExpr, TokenType.TK_AND);
							this.exprAnalyzeAll(pAndWC);
							//testcase( db.mallocFailed );
							////if ( 0 == db.mallocFailed )
							{
								for(j=0;j<pAndWC.nTerm;j++)//, pAndTerm++ )
								 {
									pAndTerm=pAndWC.a[j];
									Debug.Assert(pAndTerm.pExpr!=null);
									if(allowedOp(pAndTerm.pExpr.op)) {
										b|=pMaskSet.getMask(pAndTerm.leftCursor);
									}
								}
							}
							indexable&=b;
						}
					}
					else
						if((pOrTerm.wtFlags&TERM_COPIED)!=0) {
							/* Skip this term for now.  We revisit it when we process the
          ** corresponding TERM_VIRTUAL term */}
						else {
							Bitmask b;
							b=pMaskSet.getMask(pOrTerm.leftCursor);
							if((pOrTerm.wtFlags&TERM_VIRTUAL)!=0) {
								WhereTerm pOther=pOrWc.a[pOrTerm.iParent];
								b|=pMaskSet.getMask(pOther.leftCursor);
							}
							indexable&=b;
							if(pOrTerm.eOperator!=WO_EQ) {
								chngToIN=0;
							}
							else {
								chngToIN&=b;
							}
						}
				}
				/*
      ** Record the set of tables that satisfy case 2.  The set might be
      ** empty.
      */pOrInfo.indexable=indexable;
				pTerm.eOperator=(u16)(indexable==0?0:WO_OR);
				/*
      ** chngToIN holds a set of tables that *might* satisfy case 1.  But
      ** we have to do some additional checking to see if case 1 really
      ** is satisfied.
      **
      ** chngToIN will hold either 0, 1, or 2 bits.  The 0-bit case means
      ** that there is no possibility of transforming the OR clause into an
      ** IN operator because one or more terms in the OR clause contain
      ** something other than == on a column in the single table.  The 1-bit
      ** case means that every term of the OR clause is of the form
      ** "table.column=expr" for some single table.  The one bit that is set
      ** will correspond to the common table.  We still need to check to make
      ** sure the same column is used on all terms.  The 2-bit case is when
      ** the all terms are of the form "table1.column=table2.column".  It
      ** might be possible to form an IN operator with either table1.column
      ** or table2.column as the LHS if either is common to every term of
      ** the OR clause.
      **
      ** Note that terms of the form "table.column1=table.column2" (the
      ** same table on both sizes of the ==) cannot be optimized.
      */if(chngToIN!=0) {
					int okToChngToIN=0;
					/* True if the conversion to IN is valid */int iColumn=-1;
					/* Column index on lhs of IN operator */int iCursor=-1;
					/* Table cursor common to all terms */int j=0;
					/* Loop counter *//* Search for a table and column that appears on one side or the
        ** other of the == operator in every subterm.  That table and column
        ** will be recorded in iCursor and iColumn.  There might not be any
        ** such table and column.  Set okToChngToIN if an appropriate table
        ** and column is found but leave okToChngToIN false if not found.
        */for(j=0;j<2&&0==okToChngToIN;j++) {
						//pOrTerm = pOrWc.a;
						for(i=pOrWc.nTerm-1;i>=0;i--)//, pOrTerm++)
						 {
							pOrTerm=pOrWc.a[pOrWc.nTerm-1-i];
							Debug.Assert(pOrTerm.eOperator==WO_EQ);
							pOrTerm.wtFlags=(u8)(pOrTerm.wtFlags&~TERM_OR_OK);
							if(pOrTerm.leftCursor==iCursor) {
								/* This is the 2-bit case and we are on the second iteration and
              ** current term is from the first iteration.  So skip this term. */Debug.Assert(j==1);
								continue;
							}
							if((chngToIN&pMaskSet.getMask(pOrTerm.leftCursor))==0) {
								/* This term must be of the form t1.a==t2.b where t2 is in the
              ** chngToIN set but t1 is not.  This term will be either preceeded
              ** or follwed by an inverted copy (t2.b==t1.a).  Skip this term
              ** and use its inversion. */testcase(pOrTerm.wtFlags&TERM_COPIED);
								testcase(pOrTerm.wtFlags&TERM_VIRTUAL);
								Debug.Assert((pOrTerm.wtFlags&(TERM_COPIED|TERM_VIRTUAL))!=0);
								continue;
							}
							iColumn=pOrTerm.u.leftColumn;
							iCursor=pOrTerm.leftCursor;
							break;
						}
						if(i<0) {
							/* No candidate table+column was found.  This can only occur
            ** on the second iteration */Debug.Assert(j==1);
							Debug.Assert((chngToIN&(chngToIN-1))==0);
							Debug.Assert(chngToIN==pMaskSet.getMask(iCursor));
							break;
						}
						testcase(j==1);
						/* We have found a candidate table and column.  Check to see if that
          ** table and column is common to every term in the OR clause */okToChngToIN=1;
						for(;i>=0&&okToChngToIN!=0;i--)//, pOrTerm++)
						 {
							pOrTerm=pOrWc.a[pOrWc.nTerm-1-i];
							Debug.Assert(pOrTerm.eOperator==WO_EQ);
							if(pOrTerm.leftCursor!=iCursor) {
								pOrTerm.wtFlags=(u8)(pOrTerm.wtFlags&~TERM_OR_OK);
							}
							else
								if(pOrTerm.u.leftColumn!=iColumn) {
									okToChngToIN=0;
								}
								else {
									int affLeft,affRight;
									/* If the right-hand side is also a column, then the affinities
              ** of both right and left sides must be such that no type
              ** conversions are required on the right.  (Ticket #2249)
              */affRight=pOrTerm.pExpr.pRight.sqlite3ExprAffinity();
									affLeft=pOrTerm.pExpr.pLeft.sqlite3ExprAffinity();
									if(affRight!=0&&affRight!=affLeft) {
										okToChngToIN=0;
									}
									else {
										pOrTerm.wtFlags|=TERM_OR_OK;
									}
								}
						}
					}
					/* At this point, okToChngToIN is true if original pTerm satisfies
        ** case 1.  In that case, construct a new virtual term that is
        ** pTerm converted into an IN operator.
        **
        ** EV: R-00211-15100
        */if(okToChngToIN!=0) {
						Expr pDup;
						/* A transient duplicate expression */ExprList pList=null;
						/* The RHS of the IN operator */Expr pLeft=null;
						/* The LHS of the IN operator */Expr pNew;
						/* The complete IN operator */for(i=pOrWc.nTerm-1;i>=0;i--)//, pOrTerm++)
						 {
							pOrTerm=pOrWc.a[pOrWc.nTerm-1-i];
							if((pOrTerm.wtFlags&TERM_OR_OK)==0)
								continue;
							Debug.Assert(pOrTerm.eOperator==WO_EQ);
							Debug.Assert(pOrTerm.leftCursor==iCursor);
							Debug.Assert(pOrTerm.u.leftColumn==iColumn);
							pDup=sqlite3ExprDup(db,pOrTerm.pExpr.pRight,0);
							pList=pWC.pParse.sqlite3ExprListAppend(pList,pDup);
							pLeft=pOrTerm.pExpr.pLeft;
						}
						Debug.Assert(pLeft!=null);
						pDup=sqlite3ExprDup(db,pLeft,0);
						pNew=pParse.sqlite3PExpr(TK_IN,pDup,null,null);
						if(pNew!=null) {
							int idxNew;
							pNew.transferJoinMarkings(pExpr);
							Debug.Assert(!ExprHasProperty(pNew,EP_xIsSelect));
							pNew.x.pList=pList;
							idxNew=pWC.whereClauseInsert(pNew,TERM_VIRTUAL|TERM_DYNAMIC);
							testcase(idxNew==0);
							this.exprAnalyze(pWC,idxNew);
							pTerm=pWC.a[idxTerm];
							pWC.a[idxNew].iParent=idxTerm;
							pTerm.nChild=1;
						}
						else {
							sqlite3ExprListDelete(db,ref pList);
						}
						pTerm.eOperator=WO_NOOP;
						/* case 1 trumps case 2 */}
				}
			}
			public void exprAnalyze(/* the FROM clause */WhereClause pWC,/* the WHERE clause */int idxTerm/* Index of the term to be analyzed */) {
				WhereTerm pTerm;
				/* The term to be analyzed */WhereMaskSet pMaskSet;
				/* Set of table index masks */Expr pExpr;
				/* The expression to be analyzed */Bitmask prereqLeft;
				/* Prerequesites of the pExpr.pLeft */Bitmask prereqAll;
				/* Prerequesites of pExpr */Bitmask extraRight=0;
				/* Extra dependencies on LEFT JOIN */Expr pStr1=null;
				/* RHS of LIKE/GLOB operator */bool isComplete=false;
				/* RHS of LIKE/GLOB ends with wildcard */bool noCase=false;
				/* LIKE/GLOB distinguishes case */int op;
				/* Top-level operator.  pExpr.op */Parse pParse=pWC.pParse;
				/* Parsing context */sqlite3 db=pParse.db;
				/* Data_base connection *///if ( db.mallocFailed != 0 )
				//{
				//  return;
				//}
				pTerm=pWC.a[idxTerm];
				pMaskSet=pWC.pMaskSet;
				pExpr=pTerm.pExpr;
				prereqLeft=pMaskSet.exprTableUsage(pExpr.pLeft);
				op=pExpr.op;
				if(op==TK_IN) {
					Debug.Assert(pExpr.pRight==null);
					if(ExprHasProperty(pExpr,EP_xIsSelect)) {
						pTerm.prereqRight=pMaskSet.exprSelectTableUsage(pExpr.x.pSelect);
					}
					else {
						pTerm.prereqRight=pMaskSet.exprListTableUsage(pExpr.x.pList);
					}
				}
				else
					if(op==TK_ISNULL) {
						pTerm.prereqRight=0;
					}
					else {
						pTerm.prereqRight=pMaskSet.exprTableUsage(pExpr.pRight);
					}
				prereqAll=pMaskSet.exprTableUsage(pExpr);
				if(ExprHasProperty(pExpr,EP_FromJoin)) {
					Bitmask x=pMaskSet.getMask(pExpr.iRightJoinTable);
					prereqAll|=x;
					extraRight=x-1;
					/* ON clause terms may not be used with an index
** on left table of a LEFT JOIN.  Ticket #3015 */}
				pTerm.prereqAll=prereqAll;
				pTerm.leftCursor=-1;
				pTerm.iParent=-1;
				pTerm.eOperator=0;
				if(allowedOp(op)&&(pTerm.prereqRight&prereqLeft)==0) {
					Expr pLeft=pExpr.pLeft;
					Expr pRight=pExpr.pRight;
					if(pLeft.op==TK_COLUMN) {
						pTerm.leftCursor=pLeft.iTable;
						pTerm.u.leftColumn=pLeft.iColumn;
						pTerm.eOperator=operatorMask(op);
					}
					if(pRight!=null&&pRight.op==TK_COLUMN) {
						WhereTerm pNew;
						Expr pDup;
						if(pTerm.leftCursor>=0) {
							int idxNew;
							pDup=sqlite3ExprDup(db,pExpr,0);
							//if ( db.mallocFailed != 0 )
							//{
							//  sqlite3ExprDelete( db, ref pDup );
							//  return;
							//}
							idxNew=pWC.whereClauseInsert(pDup,TERM_VIRTUAL|TERM_DYNAMIC);
							if(idxNew==0)
								return;
							pNew=pWC.a[idxNew];
							pNew.iParent=idxTerm;
							pTerm=pWC.a[idxTerm];
							pTerm.nChild=1;
							pTerm.wtFlags|=TERM_COPIED;
						}
						else {
							pDup=pExpr;
							pNew=pTerm;
						}
						pParse.exprCommute(pDup);
						pLeft=pDup.pLeft;
						pNew.leftCursor=pLeft.iTable;
						pNew.u.leftColumn=pLeft.iColumn;
						testcase((prereqLeft|extraRight)!=prereqLeft);
						pNew.prereqRight=prereqLeft|extraRight;
						pNew.prereqAll=prereqAll;
						pNew.eOperator=operatorMask(pDup.op);
					}
				}
				#if !SQLITE_OMIT_BETWEEN_OPTIMIZATION
				/* If a term is the BETWEEN operator, create two new virtual terms
** that define the range that the BETWEEN implements.  For example:
**
**      a BETWEEN b AND c
**
** is converted into:
**
**      (a BETWEEN b AND c) AND (a>=b) AND (a<=c)
**
** The two new terms are added onto the end of the WhereClause object.
** The new terms are "dynamic" and are children of the original BETWEEN
** term.  That means that if the BETWEEN term is coded, the children are
** skipped.  Or, if the children are satisfied by an index, the original
** BETWEEN term is skipped.
*/else
                    if (pExpr.Operator == TokenType.TK_BETWEEN && pWC.Operator == TokenType.TK_AND)
                    {
						ExprList pList=pExpr.x.pList;
						int i;
						u8[] ops=new u8[] {
							TK_GE,
							TK_LE
						};
						Debug.Assert(pList!=null);
						Debug.Assert(pList.nExpr==2);
						for(i=0;i<2;i++) {
							Expr pNewExpr;
							int idxNew;
							pNewExpr=pParse.sqlite3PExpr(ops[i],sqlite3ExprDup(db,pExpr.pLeft,0),sqlite3ExprDup(db,pList.a[i].pExpr,0),null);
							idxNew=pWC.whereClauseInsert(pNewExpr,TERM_VIRTUAL|TERM_DYNAMIC);
							testcase(idxNew==0);
							this.exprAnalyze(pWC,idxNew);
							pTerm=pWC.a[idxTerm];
							pWC.a[idxNew].iParent=idxTerm;
						}
						pTerm.nChild=2;
					}
					#endif
					#if !(SQLITE_OMIT_OR_OPTIMIZATION) && !(SQLITE_OMIT_SUBQUERY)
					/* Analyze a term that is composed of two or more subterms connected by
** an OR operator.
*/else
                        if (pExpr.Operator == TokenType.TK_OR)
                        {
							Debug.Assert(pWC.op==TK_AND);
							this.exprAnalyzeOrTerm(pWC,idxTerm);
							pTerm=pWC.a[idxTerm];
						}
				#endif
				#if !SQLITE_OMIT_LIKE_OPTIMIZATION
				/* Add constraints to reduce the search space on a LIKE or GLOB
** operator.
**
** A like pattern of the form "x LIKE 'abc%'" is changed into constraints
**
**          x>='abc' AND x<'abd' AND x LIKE 'abc%'
**
** The last character of the prefix "abc" is incremented to form the
** termination condition "abd".
*/
                if (pWC.Operator == TokenType.TK_AND && pParse.isLikeOrGlob(pExpr, ref pStr1, ref isComplete, ref noCase) != 0)
                {
					Expr pLeft;
					/* LHS of LIKE/GLOB operator */Expr pStr2;
					/* Copy of pStr1 - RHS of LIKE/GLOB operator */Expr pNewExpr1;
					Expr pNewExpr2;
					int idxNew1;
					int idxNew2;
					CollSeq pColl;
					/* Collating sequence to use */pLeft=pExpr.x.pList.a[1].pExpr;
					pStr2=sqlite3ExprDup(db,pStr1,0);
					////if ( 0 == db.mallocFailed )
					{
						int c,pC;
						/* Last character before the first wildcard */pC=pStr2.u.zToken[StringExtensions.sqlite3Strlen30(pStr2.u.zToken)-1];
						c=pC;
						if(noCase) {
							/* The point is to increment the last character before the first
            ** wildcard.  But if we increment '@', that will push it into the
            ** alphabetic range where case conversions will mess up the
            ** inequality.  To avoid this, make sure to also run the full
            ** LIKE on all candidate expressions by clearing the isComplete flag
            */if(c=='A'-1)
								isComplete=false;
							/* EV: R-64339-08207 */c=sqlite3UpperToLower[c];
						}
						pStr2.u.zToken=pStr2.u.zToken.Substring(0,StringExtensions.sqlite3Strlen30(pStr2.u.zToken)-1)+(char)(c+1);
						// pC = c + 1;
					}
					pColl=sqlite3FindCollSeq(db,SqliteEncoding.UTF8,noCase?"NOCASE":"BINARY",0);
					pNewExpr1=pParse.sqlite3PExpr(TK_GE,sqlite3ExprDup(db,pLeft,0).sqlite3ExprSetColl(pColl),pStr1,0);
					idxNew1=pWC.whereClauseInsert(pNewExpr1,TERM_VIRTUAL|TERM_DYNAMIC);
					testcase(idxNew1==0);
					this.exprAnalyze(pWC,idxNew1);
					pNewExpr2=pParse.sqlite3PExpr(TK_LT,sqlite3ExprDup(db,pLeft,0).sqlite3ExprSetColl(pColl),pStr2,null);
					idxNew2=pWC.whereClauseInsert(pNewExpr2,TERM_VIRTUAL|TERM_DYNAMIC);
					testcase(idxNew2==0);
					this.exprAnalyze(pWC,idxNew2);
					pTerm=pWC.a[idxTerm];
					if(isComplete) {
						pWC.a[idxNew1].iParent=idxTerm;
						pWC.a[idxNew2].iParent=idxTerm;
						pTerm.nChild=2;
					}
				}
				#endif
				#if !SQLITE_OMIT_VIRTUALTABLE
				/* Add a WO_MATCH auxiliary term to the constraint set if the
** current expression is of the form:  column MATCH expr.
** This information is used by the xBestIndex methods of
** virtual tables.  The native query optimizer does not attempt
** to do anything with MATCH functions.
*/if(pExpr.isMatchOfColumn()!=0) {
					int idxNew;
					Expr pRight,pLeft;
					WhereTerm pNewTerm;
					Bitmask prereqColumn,prereqExpr;
					pRight=pExpr.x.pList.a[0].pExpr;
					pLeft=pExpr.x.pList.a[1].pExpr;
					prereqExpr=pMaskSet.exprTableUsage(pRight);
					prereqColumn=pMaskSet.exprTableUsage(pLeft);
					if((prereqExpr&prereqColumn)==0) {
						Expr pNewExpr;
						pNewExpr=pParse.sqlite3PExpr(TK_MATCH,null,sqlite3ExprDup(db,pRight,0),null);
						idxNew=pWC.whereClauseInsert(pNewExpr,TERM_VIRTUAL|TERM_DYNAMIC);
						testcase(idxNew==0);
						pNewTerm=pWC.a[idxNew];
						pNewTerm.prereqRight=prereqExpr;
						pNewTerm.leftCursor=pLeft.iTable;
						pNewTerm.u.leftColumn=pLeft.iColumn;
						pNewTerm.eOperator=WO_MATCH;
						pNewTerm.iParent=idxTerm;
						pTerm=pWC.a[idxTerm];
						pTerm.nChild=1;
						pTerm.wtFlags|=TERM_COPIED;
						pNewTerm.prereqAll=pTerm.prereqAll;
					}
				}
				#endif
				#if SQLITE_ENABLE_STAT2
																																																																																				      /* When sqlite_stat2 histogram data is available an operator of the
  ** form "x IS NOT NULL" can sometimes be evaluated more efficiently
  ** as "x>NULL" if x is not an INTEGER PRIMARY KEY.  So construct a
  ** virtual term of that form.
  **
  ** Note that the virtual term must be tagged with TERM_VNULL.  This
  ** TERM_VNULL tag will suppress the not-null check at the beginning
  ** of the loop.  Without the TERM_VNULL flag, the not-null check at
  ** the start of the loop will prevent any results from being returned.
  */
      if ( pExpr.op == TK_NOTNULL
       && pExpr.pLeft.op == TK_COLUMN
       && pExpr.pLeft.iColumn >= 0
      )
      {
        Expr pNewExpr;
        Expr pLeft = pExpr.pLeft;
        int idxNew;
        WhereTerm pNewTerm;

        pNewExpr = sqlite3PExpr( pParse, TK_GT,
                                sqlite3ExprDup( db, pLeft, 0 ),
                                sqlite3PExpr( pParse, TK_NULL, 0, 0, 0 ), 0 );

        idxNew = whereClauseInsert( pWC, pNewExpr,
                                  TERM_VIRTUAL | TERM_DYNAMIC | TERM_VNULL );
        if ( idxNew != 0 )
        {
          pNewTerm = pWC.a[idxNew];
          pNewTerm.prereqRight = 0;
          pNewTerm.leftCursor = pLeft.iTable;
          pNewTerm.u.leftColumn = pLeft.iColumn;
          pNewTerm.eOperator = WO_GT;
          pNewTerm.iParent = idxTerm;
          pTerm = pWC.a[idxTerm];
          pTerm.nChild = 1;
          pTerm.wtFlags |= TERM_COPIED;
          pNewTerm.prereqAll = pTerm.prereqAll;
        }
      }
#endif
				/* Prevent ON clause terms of a LEFT JOIN from being used to drive
** an index for tables to the left of the join.
*/pTerm.prereqRight|=extraRight;
			}
		}
		///<summary>
		/// Permitted values of the SrcList.a.jointype field
		///
		///</summary>
		private const int JT_INNER=0x0001;
		//#define JT_INNER     0x0001    /* Any kind of inner or cross join */
		private const int JT_CROSS=0x0002;
		//#define JT_CROSS     0x0002    /* Explicit use of the CROSS keyword */
		private const int JT_NATURAL=0x0004;
		//#define JT_NATURAL   0x0004    /* True for a "natural" join */
		private const int JT_LEFT=0x0008;
		//#define JT_LEFT      0x0008    /* Left outer join */
		private const int JT_RIGHT=0x0010;
		//#define JT_RIGHT     0x0010    /* Right outer join */
		private const int JT_OUTER=0x0020;
		//#define JT_OUTER     0x0020    /* The "OUTER" keyword is present */
		private const int JT_ERROR=0x0040;
		//#define JT_ERROR     0x0040    /* unknown or unsupported join type */
		///<summary>
		/// A WherePlan object holds information that describes a lookup
		/// strategy.
		///
		/// This object is intended to be opaque outside of the where.c module.
		/// It is included here only so that that compiler will know how big it
		/// is.  None of the fields in this object should be used outside of
		/// the where.c module.
		///
		/// Within the union, pIdx is only used when wsFlags&WHERE_INDEXED is true.
		/// pTerm is only used when wsFlags&WHERE_MULTI_OR is true.  And pVtabIdx
		/// is only used when wsFlags&WHERE_VIRTUALTABLE is true.  It is never the
		/// case that more than one of these conditions is true.
		///
		///</summary>
		public class WherePlan {
			public u32 wsFlags;
			/* WHERE_* flags that describe the strategy */public u32 nEq;
			///<summary>
			///Number of == constraints
			///</summary>
			public double nRow;
			///<summary>
			///Estimated number of rows (for EQP)
			///</summary>
			public class _u {
				public Index pIdx;
				/* Index when WHERE_INDEXED is true */public WhereTerm pTerm;
				/* WHERE clause term for OR-search */public sqlite3_index_info pVtabIdx;
			/* Virtual table index to use */}
			public _u u=new _u();
			public void Clear() {
				wsFlags=0;
				nEq=0;
				nRow=0;
				u.pIdx=null;
				u.pTerm=null;
				u.pVtabIdx=null;
			}
		};

		///<summary>
		/// For each nested loop in a WHERE clause implementation, the WhereInfo
		/// structure contains a single instance of this structure.  This structure
		/// is intended to be private the the where.c module and should not be
		/// access or modified by other modules.
		///
		/// The pIdxInfo field is used to help pick the best index on a
		/// virtual table.  The pIdxInfo pointer contains indexing
		/// information for the i-th table in the FROM clause before reordering.
		/// All the pIdxInfo pointers are freed by whereInfoFree() in where.c.
		/// All other information in the i-th WhereLevel object for the i-th table
		/// after FROM clause ordering.
		///
		///</summary>
		public class InLoop {
			public int iCur;
			/* The VDBE cursor used by this IN operator */public int addrInTop;
		/* Top of the IN loop */}
		public class WhereLevel {
			public WherePlan plan=new WherePlan();
			/** query plan for this element of the FROM clause */
            public int iLeftJoin;
			/** Memory cell used to implement LEFT OUTER JOIN */public int iTabCur;
			/** The VDBE cursor used to access the table */public int iIdxCur;
			/** The VDBE cursor used to access pIdx */public int addrBrk;
			/** Jump here to break out of the loop */public int addrNxt;
			/** Jump here to start the next IN combination */public int addrCont;
			/** Jump here to continue with the next loop cycle */public int addrFirst;
			/** First instruction of interior of the loop */public u8 iFrom;
			/** Which entry in the FROM clause */public u8 op,p5;
			///<summary>
			///Opcode and P5 of the opcode that ends the loop
			///</summary>
			public int p1,p2;
			/* Operands of the opcode used to ends the loop */public class _u {
				public class __in /* Information that depends on plan.wsFlags */{
					public int nIn;
					/* Number of entries in aInLoop[] */public InLoop[] aInLoop;
				/* Information about each nested IN operator */}
				public __in _in=new __in();
			/* Used when plan.wsFlags&WHERE_IN_ABLE */}
			public _u u=new _u();
			/* The following field is really not part of the current level.  But
      ** we need a place to cache virtual table index information for each
      ** virtual table in the FROM clause and the WhereLevel structure is
      ** a convenient place since there is one WhereLevel for each FROM clause
      ** element.
      */public sqlite3_index_info pIdxInfo;
			/* Index info for n-th source table */public void disableTerm(WhereTerm pTerm) {
				if(pTerm!=null&&(pTerm.wtFlags&TERM_CODED)==0&&(this.iLeftJoin==0||ExprHasProperty(pTerm.pExpr,EP_FromJoin))) {
					pTerm.wtFlags|=TERM_CODED;
					if(pTerm.iParent>=0) {
						WhereTerm pOther=pTerm.pWC.a[pTerm.iParent];
						if((--pOther.nChild)==0) {
							this.disableTerm(pOther);
						}
					}
				}
			}
		}
		///<summary>
		/// Flags appropriate for the wctrlFlags parameter of sqlite3WhereBegin()
		/// and the WhereInfo.wctrlFlags member.
		///
		///</summary>
		//#define WHERE_ORDERBY_NORMAL   0x0000 /* No-op */
		//#define WHERE_ORDERBY_MIN      0x0001 /* ORDER BY processing for min() func */
		//#define WHERE_ORDERBY_MAX      0x0002 /* ORDER BY processing for max() func */
		//#define WHERE_ONEPASS_DESIRED  0x0004 /* Want to do one-pass UPDATE/DELETE */
		//#define WHERE_DUPLICATES_OK    0x0008 /* Ok to return a row more than once */
		//#define WHERE_OMIT_OPEN        0x0010  /* Table cursors are already open */
		//#define WHERE_OMIT_CLOSE       0x0020  /* Omit close of table & index cursors */
		//#define WHERE_FORCE_TABLE      0x0040 /* Do not use an index-only search */
		//#define WHERE_ONETABLE_ONLY    0x0080 /* Only code the 1st table in pTabList */
		private const int WHERE_ORDERBY_NORMAL=0x0000;
		private const int WHERE_ORDERBY_MIN=0x0001;
		private const int WHERE_ORDERBY_MAX=0x0002;
		private const int WHERE_ONEPASS_DESIRED=0x0004;
		private const int WHERE_DUPLICATES_OK=0x0008;
		private const int WHERE_OMIT_OPEN=0x0010;
		private const int WHERE_OMIT_CLOSE=0x0020;
		private const int WHERE_FORCE_TABLE=0x0040;
		private const int WHERE_ONETABLE_ONLY=0x0080;
		///<summary>
		/// The WHERE clause processing routine has two halves.  The
		/// first part does the start of the WHERE loop and the second
		/// half does the tail of the WHERE loop.  An instance of
		/// this structure is returned by the first half and passed
		/// into the second half to give some continuity.
		///
		///</summary>
		public class WhereInfo {
			public Parse pParse;
			/* Parsing and code generating context */public u16 wctrlFlags;
			/* Flags originally passed to sqlite3WhereBegin() */public u8 okOnePass;
			/* Ok to use one-pass algorithm for UPDATE or DELETE */public u8 untestedTerms;
			/* Not all WHERE terms resolved by outer loop */public SrcList pTabList;
			/* List of tables in the join */public int iTop;
			/* The very beginning of the WHERE loop */public int iContinue;
			/* Jump here to continue with next record */public int iBreak;
			/* Jump here to break out of the loop */public int nLevel;
			/* Number of nested loop */public WhereClause pWC;
			/* Decomposition of the WHERE clause */public double savedNQueryLoop;
			/* pParse->nQueryLoop outside the WHERE loop */public double nRowOut;
			/* Estimated number of output rows */public WhereLevel[] a=new WhereLevel[] {
				new WhereLevel()
			};
			/* Information about each nest loop in the WHERE */public void sqlite3WhereEnd() {
				Parse pParse=this.pParse;
				Vdbe v=pParse.pVdbe;
				int i;
				WhereLevel pLevel;
				SrcList pTabList=this.pTabList;
				sqlite3 db=pParse.db;
				/* Generate loop termination code.
      */pParse.sqlite3ExprCacheClear();
				for(i=this.nLevel-1;i>=0;i--) {
					pLevel=this.a[i];
					v.sqlite3VdbeResolveLabel(pLevel.addrCont);
					if(pLevel.op!=OP_Noop) {
						v.sqlite3VdbeAddOp2(pLevel.op,pLevel.p1,pLevel.p2);
						v.sqlite3VdbeChangeP5(pLevel.p5);
					}
					if((pLevel.plan.wsFlags&WHERE_IN_ABLE)!=0&&pLevel.u._in.nIn>0) {
						InLoop pIn;
						int j;
						v.sqlite3VdbeResolveLabel(pLevel.addrNxt);
						for(j=pLevel.u._in.nIn;j>0;j--)//, pIn--)
						 {
							pIn=pLevel.u._in.aInLoop[j-1];
							v.sqlite3VdbeJumpHere(pIn.addrInTop+1);
							v.sqlite3VdbeAddOp2(OP_Next,pIn.iCur,pIn.addrInTop);
							v.sqlite3VdbeJumpHere(pIn.addrInTop-1);
						}
						db.sqlite3DbFree(ref pLevel.u._in.aInLoop);
					}
					v.sqlite3VdbeResolveLabel(pLevel.addrBrk);
					if(pLevel.iLeftJoin!=0) {
						int addr;
						addr=v.sqlite3VdbeAddOp1(OP_IfPos,pLevel.iLeftJoin);
						Debug.Assert((pLevel.plan.wsFlags&WHERE_IDX_ONLY)==0||(pLevel.plan.wsFlags&WHERE_INDEXED)!=0);
						if((pLevel.plan.wsFlags&WHERE_IDX_ONLY)==0) {
							v.sqlite3VdbeAddOp1(OP_NullRow,pTabList.a[i].iCursor);
						}
						if(pLevel.iIdxCur>=0) {
							v.sqlite3VdbeAddOp1(OP_NullRow,pLevel.iIdxCur);
						}
						if(pLevel.op==OP_Return) {
							v.sqlite3VdbeAddOp2(OP_Gosub,pLevel.p1,pLevel.addrFirst);
						}
						else {
							v.sqlite3VdbeAddOp2(OP_Goto,0,pLevel.addrFirst);
						}
						v.sqlite3VdbeJumpHere(addr);
					}
				}
				/* The "break" point is here, just past the end of the outer loop.
      ** Set it.
      */v.sqlite3VdbeResolveLabel(this.iBreak);
				/* Close all of the cursors that were opened by sqlite3WhereBegin.
      */Debug.Assert(this.nLevel==1||this.nLevel==pTabList.nSrc);
				for(i=0;i<this.nLevel;i++)//  for(i=0, pLevel=pWInfo.a; i<pWInfo.nLevel; i++, pLevel++){
				 {
					pLevel=this.a[i];
					SrcList_item pTabItem=pTabList.a[pLevel.iFrom];
					Table pTab=pTabItem.pTab;
					Debug.Assert(pTab!=null);
					if((pTab.tabFlags&TF_Ephemeral)==0&&pTab.pSelect==null&&(this.wctrlFlags&WHERE_OMIT_CLOSE)==0) {
						u32 ws=pLevel.plan.wsFlags;
						if(0==this.okOnePass&&(ws&WHERE_IDX_ONLY)==0) {
							v.sqlite3VdbeAddOp1(OP_Close,pTabItem.iCursor);
						}
						if((ws&WHERE_INDEXED)!=0&&(ws&WHERE_TEMP_INDEX)==0) {
							v.sqlite3VdbeAddOp1(OP_Close,pLevel.iIdxCur);
						}
					}
					/* If this scan uses an index, make code substitutions to read data
        ** from the index in preference to the table. Sometimes, this means
        ** the table need never be read from. This is a performance boost,
        ** as the vdbe level waits until the table is read before actually
        ** seeking the table cursor to the record corresponding to the current
        ** position in the index.
        **
        ** Calls to the code generator in between sqlite3WhereBegin and
        ** sqlite3WhereEnd will have created code that references the table
        ** directly.  This loop scans all that code looking for opcodes
        ** that reference the table and converts them into opcodes that
        ** reference the index.
        */if((pLevel.plan.wsFlags&WHERE_INDEXED)!=0)///* && 0 == db.mallocFailed */ )
					 {
						int k,j,last;
						VdbeOp pOp;
						Index pIdx=pLevel.plan.u.pIdx;
						Debug.Assert(pIdx!=null);
						//pOp = sqlite3VdbeGetOp( v, pWInfo.iTop );
						last=v.sqlite3VdbeCurrentAddr();
						for(k=this.iTop;k<last;k++)//, pOp++ )
						 {
							pOp=v.sqlite3VdbeGetOp(k);
							if(pOp.p1!=pLevel.iTabCur)
								continue;
                            if (pOp.OpCode == OpCode.OP_Column)
                            {
								for(j=0;j<pIdx.nColumn;j++) {
									if(pOp.p2==pIdx.aiColumn[j]) {
										pOp.p2=j;
										pOp.p1=pLevel.iIdxCur;
										break;
									}
								}
								Debug.Assert((pLevel.plan.wsFlags&WHERE_IDX_ONLY)==0||j<pIdx.nColumn);
							}
							else
                                if (pOp.OpCode == OpCode.OP_Rowid)
                                {
									pOp.p1=pLevel.iIdxCur;
                                    pOp.OpCode = OpCode.OP_IdxRowid;
								}
						}
					}
				}
				/* Final cleanup
      */pParse.nQueryLoop=this.savedNQueryLoop;
				db.whereInfoFree(this);
				return;
			}
			public Bitmask codeOneLoopStart(/* Complete information about the WHERE clause */int iLevel,/* Which level of pWInfo.a[] should be coded */u16 wctrlFlags,/* One of the WHERE_* flags defined in sqliteInt.h */Bitmask notReady/* Which tables are currently available */) {
				int j,k;
				/* Loop counters */int iCur;
				/* The VDBE cursor for the table */int addrNxt;
				/* Where to jump to continue with the next IN case */int omitTable;
				/* True if we use the index only */int bRev;
				/* True if we need to scan in reverse order */WhereLevel pLevel;
				/* The where level to be coded */WhereClause pWC;
				/* Decomposition of the entire WHERE clause */WhereTerm pTerm;
				/* A WHERE clause term */Parse pParse;
				/* Parsing context */Vdbe v;
				/* The prepared stmt under constructions */SrcList_item pTabItem;
				/* FROM clause term being coded */int addrBrk;
				/* Jump here to break out of the loop */int addrCont;
				/* Jump here to continue with next cycle */int iRowidReg=0;
				/* Rowid is stored in this register, if not zero */int iReleaseReg=0;
				/* Temp register to free before returning */pParse=this.pParse;
				v=pParse.pVdbe;
				pWC=this.pWC;
				pLevel=this.a[iLevel];
				pTabItem=this.pTabList.a[pLevel.iFrom];
				iCur=pTabItem.iCursor;
				bRev=(pLevel.plan.wsFlags&WHERE_REVERSE)!=0?1:0;
				omitTable=((pLevel.plan.wsFlags&WHERE_IDX_ONLY)!=0&&(wctrlFlags&WHERE_FORCE_TABLE)==0)?1:0;
				/* Create labels for the "break" and "continue" instructions
      ** for the current loop.  Jump to addrBrk to break out of a loop.
      ** Jump to cont to go immediately to the next iteration of the
      ** loop.
      **
      ** When there is an IN operator, we also have a "addrNxt" label that
      ** means to continue with the next IN value combination.  When
      ** there are no IN operators in the constraints, the "addrNxt" label
      ** is the same as "addrBrk".
      */addrBrk=pLevel.addrBrk=pLevel.addrNxt=v.sqlite3VdbeMakeLabel();
				addrCont=pLevel.addrCont=v.sqlite3VdbeMakeLabel();
				/* If this is the right table of a LEFT OUTER JOIN, allocate and
      ** initialize a memory cell that records if this table matches any
      ** row of the left table of the join.
      */if(pLevel.iFrom>0&&(pTabItem.jointype&JT_LEFT)!=0)// Check value of pTabItem[0].jointype
				 {
					pLevel.iLeftJoin=++pParse.nMem;
					v.sqlite3VdbeAddOp2(OP_Integer,0,pLevel.iLeftJoin);
					#if SQLITE_DEBUG
																																																																																																														        VdbeComment( v, "init LEFT JOIN no-match flag" );
#endif
				}
				#if !SQLITE_OMIT_VIRTUALTABLE
				if((pLevel.plan.wsFlags&WHERE_VIRTUALTABLE)!=0) {
					/* Case 0:  The table is a virtual-table.  Use the VFilter and VNext
        **          to access the data.
        */int iReg;
					/* P3 Value for OP_VFilter */sqlite3_index_info pVtabIdx=pLevel.plan.u.pVtabIdx;
					int nConstraint=pVtabIdx.nConstraint;
					sqlite3_index_constraint_usage[] aUsage=pVtabIdx.aConstraintUsage;
					sqlite3_index_constraint[] aConstraint=pVtabIdx.aConstraint;
					pParse.sqlite3ExprCachePush();
					iReg=pParse.sqlite3GetTempRange(nConstraint+2);
					for(j=1;j<=nConstraint;j++) {
						for(k=0;k<nConstraint;k++) {
							if(aUsage[k].argvIndex==j) {
								int iTerm=aConstraint[k].iTermOffset;
								pParse.sqlite3ExprCode(pWC.a[iTerm].pExpr.pRight,iReg+j+1);
								break;
							}
						}
						if(k==nConstraint)
							break;
					}
					v.sqlite3VdbeAddOp2(OP_Integer,pVtabIdx.idxNum,iReg);
					v.sqlite3VdbeAddOp2(OP_Integer,j-1,iReg+1);
					v.sqlite3VdbeAddOp4(OP_VFilter,iCur,addrBrk,iReg,pVtabIdx.idxStr,pVtabIdx.needToFreeIdxStr!=0?P4_MPRINTF:P4_STATIC);
					pVtabIdx.needToFreeIdxStr=0;
					for(j=0;j<nConstraint;j++) {
						if(aUsage[j].omit!=false) {
							int iTerm=aConstraint[j].iTermOffset;
							pLevel.disableTerm(pWC.a[iTerm]);
						}
					}
					pLevel.op=OP_VNext;
					pLevel.p1=iCur;
					pLevel.p2=v.sqlite3VdbeCurrentAddr();
					pParse.sqlite3ReleaseTempRange(iReg,nConstraint+2);
					pParse.sqlite3ExprCachePop(1);
				}
				else
					#endif
					if((pLevel.plan.wsFlags&WHERE_ROWID_EQ)!=0) {
						/* Case 1:  We can directly reference a single row using an
          **          equality comparison against the ROWID field.  Or
          **          we reference multiple rows using a "rowid IN (...)"
          **          construct.
          */iReleaseReg=pParse.sqlite3GetTempReg();
						pTerm=pWC.findTerm(iCur,-1,notReady,WO_EQ|WO_IN,null);
						Debug.Assert(pTerm!=null);
						Debug.Assert(pTerm.pExpr!=null);
						Debug.Assert(pTerm.leftCursor==iCur);
						Debug.Assert(omitTable==0);
						testcase(pTerm.wtFlags&TERM_VIRTUAL);
						/* EV: R-30575-11662 */iRowidReg=pParse.codeEqualityTerm(pTerm,pLevel,iReleaseReg);
						addrNxt=pLevel.addrNxt;
						v.sqlite3VdbeAddOp2(OP_MustBeInt,iRowidReg,addrNxt);
						v.sqlite3VdbeAddOp3(OP_NotExists,iCur,addrNxt,iRowidReg);
						pParse.sqlite3ExprCacheStore(iCur,-1,iRowidReg);
						#if SQLITE_DEBUG
																																																																																																																																								          VdbeComment( v, "pk" );
#endif
						pLevel.op=OP_Noop;
					}
					else
						if((pLevel.plan.wsFlags&WHERE_ROWID_RANGE)!=0) {
							/* Case 2:  We have an inequality comparison against the ROWID field.
          */int testOp=OP_Noop;
							int start;
							int memEndValue=0;
							WhereTerm pStart,pEnd;
							Debug.Assert(omitTable==0);
							pStart=pWC.findTerm(iCur,-1,notReady,WO_GT|WO_GE,null);
							pEnd=pWC.findTerm(iCur,-1,notReady,WO_LT|WO_LE,null);
							if(bRev!=0) {
								pTerm=pStart;
								pStart=pEnd;
								pEnd=pTerm;
							}
							if(pStart!=null) {
								Expr pX;
								/* The expression that defines the start bound */int r1,rTemp=0;
								/* Registers for holding the start boundary *//* The following constant maps TK_xx codes into corresponding
            ** seek opcodes.  It depends on a particular ordering of TK_xx
            */u8[] aMoveOp=new u8[] {
									/* TK_GT */OP_SeekGt,
									/* TK_LE */OP_SeekLe,
									/* TK_LT */OP_SeekLt,
									/* TK_GE */OP_SeekGe
								};
								Debug.Assert(TK_LE==TK_GT+1);
								/* Make sure the ordering.. */Debug.Assert(TK_LT==TK_GT+2);
								/*  ... of the TK_xx values... */Debug.Assert(TK_GE==TK_GT+3);
								/*  ... is correcct. */testcase(pStart.wtFlags&TERM_VIRTUAL);
								/* EV: R-30575-11662 */pX=pStart.pExpr;
								Debug.Assert(pX!=null);
								Debug.Assert(pStart.leftCursor==iCur);
								r1=pParse.sqlite3ExprCodeTemp(pX.pRight,ref rTemp);
								v.sqlite3VdbeAddOp3(aMoveOp[pX.op-TK_GT],iCur,addrBrk,r1);
								#if SQLITE_DEBUG
																																																																																																																																																																																												            VdbeComment( v, "pk" );
#endif
								pParse.sqlite3ExprCacheAffinityChange(r1,1);
								pParse.sqlite3ReleaseTempReg(rTemp);
								pLevel.disableTerm(pStart);
							}
							else {
								v.sqlite3VdbeAddOp2(bRev!=0?OP_Last:OP_Rewind,iCur,addrBrk);
							}
							if(pEnd!=null) {
								Expr pX;
								pX=pEnd.pExpr;
								Debug.Assert(pX!=null);
								Debug.Assert(pEnd.leftCursor==iCur);
								testcase(pEnd.wtFlags&TERM_VIRTUAL);
								/* EV: R-30575-11662 */memEndValue=++pParse.nMem;
								pParse.sqlite3ExprCode(pX.pRight,memEndValue);
								if(pX.op==TK_LT||pX.op==TK_GT) {
									testOp=bRev!=0?OP_Le:OP_Ge;
								}
								else {
									testOp=bRev!=0?OP_Lt:OP_Gt;
								}
								pLevel.disableTerm(pEnd);
							}
							start=v.sqlite3VdbeCurrentAddr();
							pLevel.op=(u8)(bRev!=0?OP_Prev:OP_Next);
							pLevel.p1=iCur;
							pLevel.p2=start;
							if(pStart==null&&pEnd==null) {
								pLevel.p5=SQLITE_STMTSTATUS_FULLSCAN_STEP;
							}
							else {
								Debug.Assert(pLevel.p5==0);
							}
							if(testOp!=OP_Noop) {
								iRowidReg=iReleaseReg=pParse.sqlite3GetTempReg();
								v.sqlite3VdbeAddOp2(OP_Rowid,iCur,iRowidReg);
								pParse.sqlite3ExprCacheStore(iCur,-1,iRowidReg);
								v.sqlite3VdbeAddOp3(testOp,memEndValue,addrBrk,iRowidReg);
								v.sqlite3VdbeChangeP5(SQLITE_AFF_NUMERIC|SQLITE_JUMPIFNULL);
							}
						}
						else
							if((pLevel.plan.wsFlags&(WHERE_COLUMN_RANGE|WHERE_COLUMN_EQ))!=0) {
								/* Case 3: A scan using an index.
          **
          **         The WHERE clause may contain zero or more equality
          **         terms ("==" or "IN" operators) that refer to the N
          **         left-most columns of the index. It may also contain
          **         inequality constraints (>, <, >= or <=) on the indexed
          **         column that immediately follows the N equalities. Only
          **         the right-most column can be an inequality - the rest must
          **         use the "==" and "IN" operators. For example, if the
          **         index is on (x,y,z), then the following clauses are all
          **         optimized:
          **
          **            x=5
          **            x=5 AND y=10
          **            x=5 AND y<10
          **            x=5 AND y>5 AND y<10
          **            x=5 AND y=5 AND z<=10
          **
          **         The z<10 term of the following cannot be used, only
          **         the x=5 term:
          **
          **            x=5 AND z<10
          **
          **         N may be zero if there are inequality constraints.
          **         If there are no inequality constraints, then N is at
          **         least one.
          **
          **         This case is also used when there are no WHERE clause
          **         constraints but an index is selected anyway, in order
          **         to force the output order to conform to an ORDER BY.
          */u8[] aStartOp=new u8[] {
									0,
									0,
									OP_Rewind,
									/* 2: (!start_constraints && startEq &&  !bRev) */OP_Last,
									/* 3: (!start_constraints && startEq &&   bRev) */OP_SeekGt,
									/* 4: (start_constraints  && !startEq && !bRev) */OP_SeekLt,
									/* 5: (start_constraints  && !startEq &&  bRev) */OP_SeekGe,
									/* 6: (start_constraints  &&  startEq && !bRev) */OP_SeekLe
								/* 7: (start_constraints  &&  startEq &&  bRev) */};
								u8[] aEndOp=new u8[] {
									OP_Noop,
									/* 0: (!end_constraints) */OP_IdxGE,
									/* 1: (end_constraints && !bRev) */OP_IdxLT
								/* 2: (end_constraints && bRev) */};
								int nEq=(int)pLevel.plan.nEq;
								/* Number of == or IN terms */int isMinQuery=0;
								/* If this is an optimized SELECT min(x).. */int regBase;
								/* Base register holding constraint values */int r1;
								/* Temp register */WhereTerm pRangeStart=null;
								/* Inequality constraint at range start */WhereTerm pRangeEnd=null;
								/* Inequality constraint at range end */int startEq;
								/* True if range start uses ==, >= or <= */int endEq;
								/* True if range end uses ==, >= or <= */int start_constraints;
								/* Start of range is constrained */int nConstraint;
								/* Number of constraint terms */Index pIdx;
								/* The index we will be using */int iIdxCur;
								/* The VDBE cursor for the index */int nExtraReg=0;
								/* Number of extra registers needed */int op;
								/* Instruction opcode */StringBuilder zStartAff=new StringBuilder("");
								;
								/* Affinity for start of range constraint */StringBuilder zEndAff;
								/* Affinity for end of range constraint */pIdx=pLevel.plan.u.pIdx;
								iIdxCur=pLevel.iIdxCur;
								k=pIdx.aiColumn[nEq];
								/* Column for inequality constraints *//* If this loop satisfies a sort order (pOrderBy) request that
          ** was pDebug.Assed to this function to implement a "SELECT min(x) ..."
          ** query, then the caller will only allow the loop to run for
          ** a single iteration. This means that the first row returned
          ** should not have a NULL value stored in 'x'. If column 'x' is
          ** the first one after the nEq equality constraints in the index,
          ** this requires some special handling.
          */if((wctrlFlags&WHERE_ORDERBY_MIN)!=0&&((pLevel.plan.wsFlags&WHERE_ORDERBY)!=0)&&(pIdx.nColumn>nEq)) {
									/* Debug.Assert( pOrderBy.nExpr==1 ); *//* Debug.Assert( pOrderBy.a[0].pExpr.iColumn==pIdx.aiColumn[nEq] ); */isMinQuery=1;
									nExtraReg=1;
								}
								/* Find any inequality constraint terms for the start and end
          ** of the range.
          */if((pLevel.plan.wsFlags&WHERE_TOP_LIMIT)!=0) {
									pRangeEnd=pWC.findTerm(iCur,k,notReady,(WO_LT|WO_LE),pIdx);
									nExtraReg=1;
								}
								if((pLevel.plan.wsFlags&WHERE_BTM_LIMIT)!=0) {
									pRangeStart=pWC.findTerm(iCur,k,notReady,(WO_GT|WO_GE),pIdx);
									nExtraReg=1;
								}
								/* Generate code to evaluate all constraint terms using == or IN
          ** and store the values of those terms in an array of registers
          ** starting at regBase.
          */regBase=pParse.codeAllEqualityTerms(pLevel,pWC,notReady,nExtraReg,out zStartAff);
								zEndAff=new StringBuilder(zStartAff.ToString());
								//sqlite3DbStrDup(pParse.db, zStartAff);
								addrNxt=pLevel.addrNxt;
								/* If we are doing a reverse order scan on an ascending index, or
          ** a forward order scan on a descending index, interchange the
          ** start and end terms (pRangeStart and pRangeEnd).
          */if(nEq<pIdx.nColumn&&bRev==(pIdx.aSortOrder[nEq]==SQLITE_SO_ASC?1:0)) {
									SWAP(ref pRangeEnd,ref pRangeStart);
								}
								testcase(pRangeStart!=null&&(pRangeStart.eOperator&WO_LE)!=0);
								testcase(pRangeStart!=null&&(pRangeStart.eOperator&WO_GE)!=0);
								testcase(pRangeEnd!=null&&(pRangeEnd.eOperator&WO_LE)!=0);
								testcase(pRangeEnd!=null&&(pRangeEnd.eOperator&WO_GE)!=0);
								startEq=(null==pRangeStart||(pRangeStart.eOperator&(WO_LE|WO_GE))!=0)?1:0;
								endEq=(null==pRangeEnd||(pRangeEnd.eOperator&(WO_LE|WO_GE))!=0)?1:0;
								start_constraints=(pRangeStart!=null||nEq>0)?1:0;
								/* Seek the index cursor to the start of the range. */nConstraint=nEq;
								if(pRangeStart!=null) {
									Expr pRight=pRangeStart.pExpr.pRight;
									pParse.sqlite3ExprCode(pRight,regBase+nEq);
									if((pRangeStart.wtFlags&TERM_VNULL)==0) {
										sqlite3ExprCodeIsNullJump(v,pRight,regBase+nEq,addrNxt);
									}
									if(zStartAff.Length!=0) {
										if(pRight.sqlite3CompareAffinity(zStartAff[nEq])==SQLITE_AFF_NONE) {
											/* Since the comparison is to be performed with no conversions
                ** applied to the operands, set the affinity to apply to pRight to 
                ** SQLITE_AFF_NONE.  */zStartAff[nEq]=SQLITE_AFF_NONE;
										}
										if((sqlite3ExprNeedsNoAffinityChange(pRight,zStartAff[nEq]))!=0) {
											zStartAff[nEq]=SQLITE_AFF_NONE;
										}
									}
									nConstraint++;
									testcase(pRangeStart.wtFlags&TERM_VIRTUAL);
									/* EV: R-30575-11662 */}
								else
									if(isMinQuery!=0) {
										v.sqlite3VdbeAddOp2(OP_Null,0,regBase+nEq);
										nConstraint++;
										startEq=0;
										start_constraints=1;
									}
								pParse.codeApplyAffinity(regBase,nConstraint,zStartAff.ToString());
								op=aStartOp[(start_constraints<<2)+(startEq<<1)+bRev];
								Debug.Assert(op!=0);
								testcase(op==OP_Rewind);
								testcase(op==OP_Last);
								testcase(op==OP_SeekGt);
								testcase(op==OP_SeekGe);
								testcase(op==OP_SeekLe);
								testcase(op==OP_SeekLt);
								v.sqlite3VdbeAddOp4Int(op,iIdxCur,addrNxt,regBase,nConstraint);
								/* Load the value for the inequality constraint at the end of the
          ** range (if any).
          */nConstraint=nEq;
								if(pRangeEnd!=null) {
									Expr pRight=pRangeEnd.pExpr.pRight;
									pParse.sqlite3ExprCacheRemove(regBase+nEq,1);
									pParse.sqlite3ExprCode(pRight,regBase+nEq);
									if((pRangeEnd.wtFlags&TERM_VNULL)==0) {
										sqlite3ExprCodeIsNullJump(v,pRight,regBase+nEq,addrNxt);
									}
									if(zEndAff.Length>0) {
										if(pRight.sqlite3CompareAffinity(zEndAff[nEq])==SQLITE_AFF_NONE) {
											/* Since the comparison is to be performed with no conversions
                ** applied to the operands, set the affinity to apply to pRight to 
                ** SQLITE_AFF_NONE.  */zEndAff[nEq]=SQLITE_AFF_NONE;
										}
										if((sqlite3ExprNeedsNoAffinityChange(pRight,zEndAff[nEq]))!=0) {
											zEndAff[nEq]=SQLITE_AFF_NONE;
										}
									}
									pParse.codeApplyAffinity(regBase,nEq+1,zEndAff.ToString());
									nConstraint++;
									testcase(pRangeEnd.wtFlags&TERM_VIRTUAL);
									/* EV: R-30575-11662 */}
								pParse.db.sqlite3DbFree(ref zStartAff);
								pParse.db.sqlite3DbFree(ref zEndAff);
								/* Top of the loop body */pLevel.p2=v.sqlite3VdbeCurrentAddr();
								/* Check if the index cursor is past the end of the range. */op=aEndOp[((pRangeEnd!=null||nEq!=0)?1:0)*(1+bRev)];
								testcase(op==OP_Noop);
								testcase(op==OP_IdxGE);
								testcase(op==OP_IdxLT);
								if(op!=OP_Noop) {
									v.sqlite3VdbeAddOp4Int(op,iIdxCur,addrNxt,regBase,nConstraint);
									v.sqlite3VdbeChangeP5((u8)(endEq!=bRev?1:0));
								}
								/* If there are inequality constraints, check that the value
          ** of the table column that the inequality contrains is not NULL.
          ** If it is, jump to the next iteration of the loop.
          */r1=pParse.sqlite3GetTempReg();
								testcase(pLevel.plan.wsFlags&WHERE_BTM_LIMIT);
								testcase(pLevel.plan.wsFlags&WHERE_TOP_LIMIT);
								if((pLevel.plan.wsFlags&(WHERE_BTM_LIMIT|WHERE_TOP_LIMIT))!=0) {
									v.sqlite3VdbeAddOp3(OP_Column,iIdxCur,nEq,r1);
									v.sqlite3VdbeAddOp2(OP_IsNull,r1,addrCont);
								}
								pParse.sqlite3ReleaseTempReg(r1);
								/* Seek the table cursor, if required */pLevel.disableTerm(pRangeStart);
								pLevel.disableTerm(pRangeEnd);
								if(0==omitTable) {
									iRowidReg=iReleaseReg=pParse.sqlite3GetTempReg();
									v.sqlite3VdbeAddOp2(OP_IdxRowid,iIdxCur,iRowidReg);
									pParse.sqlite3ExprCacheStore(iCur,-1,iRowidReg);
									v.sqlite3VdbeAddOp2(OP_Seek,iCur,iRowidReg);
									/* Deferred seek */}
								/* Record the instruction used to terminate the loop. Disable
          ** WHERE clause terms made redundant by the index range scan.
          */if((pLevel.plan.wsFlags&WHERE_UNIQUE)!=0) {
									pLevel.op=OP_Noop;
								}
								else
									if(bRev!=0) {
										pLevel.op=OP_Prev;
									}
									else {
										pLevel.op=OP_Next;
									}
								pLevel.p1=iIdxCur;
							}
							else
								#if !SQLITE_OMIT_OR_OPTIMIZATION
								if((pLevel.plan.wsFlags&WHERE_MULTI_OR)!=0) {
									/* Case 4:  Two or more separately indexed terms connected by OR
            **
            ** Example:
            **
            **   CREATE TABLE t1(a,b,c,d);
            **   CREATE INDEX i1 ON t1(a);
            **   CREATE INDEX i2 ON t1(b);
            **   CREATE INDEX i3 ON t1(c);
            **
            **   SELECT * FROM t1 WHERE a=5 OR b=7 OR (c=11 AND d=13)
            **
            ** In the example, there are three indexed terms connected by OR.
            ** The top of the loop looks like this:
            **
            **          Null       1                # Zero the rowset in reg 1
            **
            ** Then, for each indexed term, the following. The arguments to
            ** RowSetTest are such that the rowid of the current row is inserted
            ** into the RowSet. If it is already present, control skips the
            ** Gosub opcode and jumps straight to the code generated by WhereEnd().
            **
            **        sqlite3WhereBegin(<term>)
            **          RowSetTest                  # Insert rowid into rowset
            **          Gosub      2 A
            **        sqlite3WhereEnd()
            **
            ** Following the above, code to terminate the loop. Label A, the target
            ** of the Gosub above, jumps to the instruction right after the Goto.
            **
            **          Null       1                # Zero the rowset in reg 1
            **          Goto       B                # The loop is finished.
            **
            **       A: <loop body>                 # Return data, whatever.
            **
            **          Return     2                # Jump back to the Gosub
            **
            **       B: <after the loop>
            **
            */WhereClause pOrWc;
									/* The OR-clause broken out into subterms */SrcList pOrTab;
									/* Shortened table list or OR-clause generation */int regReturn=++pParse.nMem;
									/* Register used with OP_Gosub */int regRowset=0;
									/* Register for RowSet object */int regRowid=0;
									/* Register holding rowid */int iLoopBody=v.sqlite3VdbeMakeLabel();
									/* Start of loop body */int iRetInit;
									/* Address of regReturn init */int untestedTerms=0;
									/* Some terms not completely tested */int ii;
									pTerm=pLevel.plan.u.pTerm;
									Debug.Assert(pTerm!=null);
									Debug.Assert(pTerm.eOperator==WO_OR);
									Debug.Assert((pTerm.wtFlags&TERM_ORINFO)!=0);
									pOrWc=pTerm.u.pOrInfo.wc;
									pLevel.op=OP_Return;
									pLevel.p1=regReturn;
									/* Set up a new SrcList in pOrTab containing the table being scanned
            ** by this loop in the a[0] slot and all notReady tables in a[1..] slots.
            ** This becomes the SrcList in the recursive call to sqlite3WhereBegin().
            */if(this.nLevel>1) {
										int nNotReady;
										/* The number of notReady tables */SrcList_item[] origSrc;
										/* Original list of tables */nNotReady=this.nLevel-iLevel-1;
										//sqlite3StackAllocRaw(pParse.db,
										//sizeof(*pOrTab)+ nNotReady*sizeof(pOrTab.a[0]));
										pOrTab=new SrcList();
										pOrTab.a=new SrcList_item[nNotReady+1];
										//if( pOrTab==0 ) return notReady;
										pOrTab.nAlloc=(i16)(nNotReady+1);
										pOrTab.nSrc=pOrTab.nAlloc;
										pOrTab.a[0]=pTabItem;
										//memcpy(pOrTab.a, pTabItem, sizeof(*pTabItem));
										origSrc=this.pTabList.a;
										for(k=1;k<=nNotReady;k++) {
											pOrTab.a[k]=origSrc[this.a[iLevel+k].iFrom];
											// memcpy(&pOrTab.a[k], &origSrc[pLevel[k].iFrom], sizeof(pOrTab.a[k]));
										}
									}
									else {
										pOrTab=this.pTabList;
									}
									/* Initialize the rowset register to contain NULL. An SQL NULL is
            ** equivalent to an empty rowset.
            **
            ** Also initialize regReturn to contain the address of the instruction
            ** immediately following the OP_Return at the bottom of the loop. This
            ** is required in a few obscure LEFT JOIN cases where control jumps
            ** over the top of the loop into the body of it. In this case the
            ** correct response for the end-of-loop code (the OP_Return) is to
            ** fall through to the next instruction, just as an OP_Next does if
            ** called on an uninitialized cursor.
            */if((wctrlFlags&WHERE_DUPLICATES_OK)==0) {
										regRowset=++pParse.nMem;
										regRowid=++pParse.nMem;
										v.sqlite3VdbeAddOp2(OP_Null,0,regRowset);
									}
									iRetInit=v.sqlite3VdbeAddOp2(OP_Integer,0,regReturn);
									for(ii=0;ii<pOrWc.nTerm;ii++) {
										WhereTerm pOrTerm=pOrWc.a[ii];
										if(pOrTerm.leftCursor==iCur||pOrTerm.eOperator==WO_AND) {
											WhereInfo pSubWInfo;
											/* Info for single OR-term scan *//* Loop through table entries that match term pOrTerm. */ExprList elDummy=null;
											pSubWInfo=pParse.sqlite3WhereBegin(pOrTab,pOrTerm.pExpr,ref elDummy,WHERE_OMIT_OPEN|WHERE_OMIT_CLOSE|WHERE_FORCE_TABLE|WHERE_ONETABLE_ONLY);
											if(pSubWInfo!=null) {
												pParse.explainOneScan(pOrTab,pSubWInfo.a[0],iLevel,pLevel.iFrom,0);
												if((wctrlFlags&WHERE_DUPLICATES_OK)==0) {
													int iSet=((ii==pOrWc.nTerm-1)?-1:ii);
													int r;
													r=pParse.sqlite3ExprCodeGetColumn(pTabItem.pTab,-1,iCur,regRowid);
													v.sqlite3VdbeAddOp4Int(OP_RowSetTest,regRowset,v.sqlite3VdbeCurrentAddr()+2,r,iSet);
												}
												v.sqlite3VdbeAddOp2(OP_Gosub,regReturn,iLoopBody);
												/* The pSubWInfo.untestedTerms flag means that this OR term
                  ** contained one or more AND term from a notReady table.  The
                  ** terms from the notReady table could not be tested and will
                  ** need to be tested later.
                  */if(pSubWInfo.untestedTerms!=0)
													untestedTerms=1;
												/* Finish the loop through table entries that match term pOrTerm. */pSubWInfo.sqlite3WhereEnd();
											}
										}
									}
									v.sqlite3VdbeChangeP1(iRetInit,v.sqlite3VdbeCurrentAddr());
									v.sqlite3VdbeAddOp2(OP_Goto,0,pLevel.addrBrk);
									v.sqlite3VdbeResolveLabel(iLoopBody);
									if(this.nLevel>1)
										pParse.db.sqlite3DbFree(ref pOrTab);
									//sqlite3DbFree(pParse.db, pOrTab)
									if(0==untestedTerms)
										pLevel.disableTerm(pTerm);
								}
								else
								#endif
								 {
									/* Case 5:  There is no usable index.  We must do a complete
            **          scan of the entire table.
            */u8[] aStep=new u8[] {
										OP_Next,
										OP_Prev
									};
									u8[] aStart=new u8[] {
										OP_Rewind,
										OP_Last
									};
									Debug.Assert(bRev==0||bRev==1);
									Debug.Assert(omitTable==0);
									pLevel.op=aStep[bRev];
									pLevel.p1=iCur;
									pLevel.p2=1+v.sqlite3VdbeAddOp2(aStart[bRev],iCur,addrBrk);
									pLevel.p5=SQLITE_STMTSTATUS_FULLSCAN_STEP;
								}
				notReady&=~pWC.pMaskSet.getMask(iCur);
				/* Insert code to test every subexpression that can be completely
      ** computed using the current set of tables.
      **
      ** IMPLEMENTATION-OF: R-49525-50935 Terms that cannot be satisfied through
      ** the use of indices become tests that are evaluated against each row of
      ** the relevant input tables.
      */for(j=pWC.nTerm;j>0;j--)//, pTerm++)
				 {
					pTerm=pWC.a[pWC.nTerm-j];
					Expr pE;
					testcase(pTerm.wtFlags&TERM_VIRTUAL);
					/* IMP: R-30575-11662 */testcase(pTerm.wtFlags&TERM_CODED);
					if((pTerm.wtFlags&(TERM_VIRTUAL|TERM_CODED))!=0)
						continue;
					if((pTerm.prereqAll&notReady)!=0) {
						testcase(this.untestedTerms==0&&(this.wctrlFlags&WHERE_ONETABLE_ONLY)!=0);
						this.untestedTerms=1;
						continue;
					}
					pE=pTerm.pExpr;
					Debug.Assert(pE!=null);
					if(pLevel.iLeftJoin!=0&&!((pE.flags&EP_FromJoin)==EP_FromJoin))// !ExprHasProperty(pE, EP_FromJoin) ){
					 {
						continue;
					}
					pParse.sqlite3ExprIfFalse(pE,addrCont,SQLITE_JUMPIFNULL);
					pTerm.wtFlags|=TERM_CODED;
				}
				/* For a LEFT OUTER JOIN, generate code that will record the fact that
      ** at least one row of the right table has matched the left table.
      */if(pLevel.iLeftJoin!=0) {
					pLevel.addrFirst=v.sqlite3VdbeCurrentAddr();
					v.sqlite3VdbeAddOp2(OP_Integer,1,pLevel.iLeftJoin);
					#if SQLITE_DEBUG
																																																																																																														        VdbeComment( v, "record LEFT JOIN hit" );
#endif
					pParse.sqlite3ExprCacheClear();
					for(j=0;j<pWC.nTerm;j++)//, pTerm++)
					 {
						pTerm=pWC.a[j];
						testcase(pTerm.wtFlags&TERM_VIRTUAL);
						/* IMP: R-30575-11662 */testcase(pTerm.wtFlags&TERM_CODED);
						if((pTerm.wtFlags&(TERM_VIRTUAL|TERM_CODED))!=0)
							continue;
						if((pTerm.prereqAll&notReady)!=0) {
							Debug.Assert(this.untestedTerms!=0);
							continue;
						}
						Debug.Assert(pTerm.pExpr!=null);
						pParse.sqlite3ExprIfFalse(pTerm.pExpr,addrCont,SQLITE_JUMPIFNULL);
						pTerm.wtFlags|=TERM_CODED;
					}
				}
				pParse.sqlite3ReleaseTempReg(iReleaseReg);
				return notReady;
			}
		}
		///<summary>
		/// A NameContext defines a context in which to resolve table and column
		/// names.  The context consists of a list of tables (the pSrcList) field and
		/// a list of named expression (pEList).  The named expression list may
		/// be NULL.  The pSrc corresponds to the FROM clause of a SELECT or
		/// to the table being operated on by INSERT, UPDATE, or DELETE.  The
		/// pEList corresponds to the result set of a SELECT and is NULL for
		/// other statements.
		///
		/// NameContexts can be nested.  When resolving names, the inner-most
		/// context is searched first.  If no match is found, the next outer
		/// context is checked.  If there is still no match, the next context
		/// is checked.  This process continues until either a match is found
		/// or all contexts are check.  When a match is found, the nRef member of
		/// the context containing the match is incremented.
		///
		/// Each subquery gets a new NameContext.  The pNext field points to the
		/// NameContext in the parent query.  Thus the process of scanning the
		/// NameContext list corresponds to searching through successively outer
		/// subqueries looking for a match.
		///
		///</summary>
		public class NameContext {
			public Parse pParse;
			/* The parser */public SrcList pSrcList;
			/* One or more tables used to resolve names */public ExprList pEList;
			/* Optional list of named expressions */public int nRef;
			/* Number of names resolved by this context */public int nErr;
			/* Number of errors encountered while resolving names */public u8 allowAgg;
			/* Aggregate functions allowed here */public u8 hasAgg;
			/* True if aggregates are seen */public u8 isCheck;
			/* True if resolving names in a CHECK constraint */public int nDepth;
			/* Depth of subquery recursion. 1 for no recursion */public AggInfo pAggInfo;
			/* Information about aggregates at this level */public NameContext pNext;
			/* Next outer name context.  NULL for outermost */public///<summary>
			/// Resolve an expression that was part of an ATTACH or DETACH statement. This
			/// is slightly different from resolving a normal SQL expression, because simple
			/// identifiers are treated as strings, not possible column names or aliases.
			///
			/// i.e. if the parser sees:
			///
			///     ATTACH DATABASE abc AS def
			///
			/// it treats the two expressions as literal strings 'abc' and 'def' instead of
			/// looking for columns of the same name.
			///
			/// This only applies to the root node of pExpr, so the statement:
			///
			///     ATTACH DATABASE abc||def AS 'db2'
			///
			/// will fail because neither abc or def can be resolved.
			///</summary>
			int resolveAttachExpr(Expr pExpr) {
				int rc=SQLITE_OK;
				if(pExpr!=null) {
					if(pExpr.op!=TK_ID) {
						rc=sqlite3ResolveExprNames(this,ref pExpr);
						if(rc==SQLITE_OK&&pExpr.sqlite3ExprIsConstant()==0) {
							sqlite3ErrorMsg(this.pParse,"invalid name: \"%s\"",pExpr.u.zToken);
							return SQLITE_ERROR;
						}
					}
					else {
						pExpr.op=TK_STRING;
					}
				}
				return rc;
			}
		}
		/*
    ** An instance of the following structure contains all information
    ** needed to generate code for a single SELECT statement.
    **
    ** nLimit is set to -1 if there is no LIMIT clause.  nOffset is set to 0.
    ** If there is a LIMIT clause, the parser sets nLimit to the value of the
    ** limit and nOffset to the value of the offset (or 0 if there is not
    ** offset).  But later on, nLimit and nOffset become the memory locations
    ** in the VDBE that record the limit and offset counters.
    **
    ** addrOpenEphm[] entries contain the address of OP_OpenEphemeral opcodes.
    ** These addresses must be stored so that we can go back and fill in
    ** the P4_KEYINFO and P2 parameters later.  Neither the KeyInfo nor
    ** the number of columns in P2 can be computed at the same time
    ** as the OP_OpenEphm instruction is coded because not
    ** enough information about the compound query is known at that point.
    ** The KeyInfo for addrOpenTran[0] and [1] contains collating sequences
    ** for the result set.  The KeyInfo for addrOpenTran[2] contains collating
    ** sequences for the ORDER BY clause.
    */public class Select {
			public ExprList pEList;
			/* The fields of the result */public u8 tk_op;
			/* One of: TK_UNION TK_ALL TK_INTERSECT TK_EXCEPT */public char affinity;
			/* MakeRecord with this affinity for SelectResultType.Set */public SelectFlags selFlags;
			/* Various SF_* values */public SrcList pSrc;
			/* The FROM clause */public Expr pWhere;
			/* The WHERE clause */public ExprList pGroupBy;
			/* The GROUP BY clause */public Expr pHaving;
			/* The HAVING clause */public ExprList pOrderBy;
			/* The ORDER BY clause */public Select pPrior;
			/* Prior select in a compound select statement */public Select pNext;
			/* Next select to the left in a compound */public Select pRightmost;
			/* Right-most select in a compound select statement */public Expr pLimit;
			/* LIMIT expression. NULL means not used. */public Expr pOffset;
			/* OFFSET expression. NULL means not used. */public int iLimit;
			public int iOffset;
			/* Memory registers holding LIMIT & OFFSET counters */public int[] addrOpenEphm=new int[3];
			///<summary>
			///OP_OpenEphem opcodes related to this select
			///</summary>
			public double nSelectRow;
			/* Estimated number of result rows */public Select Copy() {
				if(this==null)
					return null;
				else {
					Select cp=(Select)MemberwiseClone();
					if(pEList!=null)
						cp.pEList=pEList.Copy();
					if(pSrc!=null)
						cp.pSrc=pSrc.Copy();
					if(pWhere!=null)
						cp.pWhere=pWhere.Copy();
					if(pGroupBy!=null)
						cp.pGroupBy=pGroupBy.Copy();
					if(pHaving!=null)
						cp.pHaving=pHaving.Copy();
					if(pOrderBy!=null)
						cp.pOrderBy=pOrderBy.Copy();
					if(pPrior!=null)
						cp.pPrior=pPrior.Copy();
					if(pNext!=null)
						cp.pNext=pNext.Copy();
					if(pRightmost!=null)
						cp.pRightmost=pRightmost.Copy();
					if(pLimit!=null)
						cp.pLimit=pLimit.Copy();
					if(pOffset!=null)
						cp.pOffset=pOffset.Copy();
					return cp;
				}
			}
			public void heightOfSelect(ref int pnHeight) {
				if(this!=null) {
					this.pWhere.heightOfExpr(ref pnHeight);
					this.pHaving.heightOfExpr(ref pnHeight);
					this.pLimit.heightOfExpr(ref pnHeight);
					this.pOffset.heightOfExpr(ref pnHeight);
					this.pEList.heightOfExprList(ref pnHeight);
					this.pGroupBy.heightOfExprList(ref pnHeight);
					this.pOrderBy.heightOfExprList(ref pnHeight);
					this.pPrior.heightOfSelect(ref pnHeight);
				}
			}
			public int sqlite3SelectExprHeight() {
				int nHeight=0;
				this.heightOfSelect(ref nHeight);
				return nHeight;
			}
		}
		///<summary>
		/// Allowed values for Select.selFlags.  The "SF" prefix stands for
		/// "Select Flag".
		///
		///</summary>
		//#define SF_Distinct        0x0001  /* Output should be DISTINCT */
		//#define SF_Resolved        0x0002  /* Identifiers have been resolved */
		//#define SF_Aggregate       0x0004  /* Contains aggregate functions */
		//#define SF_UsesEphemeral   0x0008  /* Uses the OpenEphemeral opcode */
		//#define SF_Expanded        0x0010  /* sqlite3SelectExpand() called on this */
		//#define SF_HasTypeInfo     0x0020  /* FROM subqueries have Table metadata */
		public enum SelectFlags : ushort {
			Distinct=0x0001,
			/* Output should be DISTINCT */Resolved=0x0002,
			/* Identifiers have been resolved */Aggregate=0x0004,
			/* Contains aggregate functions */UsesEphemeral=0x0008,
			/* Uses the OpenEphemeral opcode */Expanded=0x0010,
			/* sqlite3SelectExpand() called on this */HasTypeInfo=0x0020
		/* FROM subqueries have Table metadata */}
		/*
    ** The results of a select can be distributed in several ways.  The
    ** "SRT" prefix means "SELECT Result Type".
    */public enum SelectResultType {
			Union=1,
			//#define SelectResultType.Union        1  /* Store result as keys in an index */
			Except=2,
			//#define SelectResultType.Except      2  /* Remove result from a UNION index */
			Exists=3,
			//#define SelectResultType.Exists      3  /* Store 1 if the result is not empty */
			Discard=4,
			//#define SelectResultType.Discard    4  /* Do not save the results anywhere */
			/* The ORDER BY clause is ignored for all of the above *///#define IgnorableOrderby(X) ((X->eDest)<=SelectResultType.Discard)
			Output=5,
			//#define SelectResultType.Output      5  /* Output each row of result */
			Mem=6,
			//#define SelectResultType.Mem            6  /* Store result in a memory cell */
			Set=7,
			//#define SelectResultType.Set            7  /* Store results as keys in an index */
			Table=8,
			//#define SelectResultType.Table        8  /* Store result as data with an automatic rowid */
			EphemTab=9,
			//#define SelectResultType.EphemTab  9  /* Create transient tab and store like SelectResultType.Table /
			Coroutine=10
		//#define SelectResultType.Coroutine   10  /* Generate a single row of result */
		}
		///<summary>
		/// A structure used to customize the behavior of sqlite3Select(). See
		/// comments above sqlite3Select() for details.
		///
		///</summary>
		//typedef struct SelectDest SelectDest;
		public class SelectDest {
			public SelectResultType eDest;
			/* How to dispose of the results */public char affinity;
			/* Affinity used when eDest==SelectResultType.Set */public int iParm;
			/* A parameter used by the eDest disposal method */public int iMem;
			/* Base register where results are written */public int nMem;
			/* Number of registers allocated */public SelectDest() {
				this.eDest=0;
				this.affinity='\0';
				this.iParm=0;
				this.iMem=0;
				this.nMem=0;
			}
			public SelectDest(SelectResultType eDest,char affinity,int iParm) {
				this.eDest=eDest;
				this.affinity=affinity;
				this.iParm=iParm;
				this.iMem=0;
				this.nMem=0;
			}
			public SelectDest(SelectResultType eDest,char affinity,int iParm,int iMem,int nMem) {
				this.eDest=eDest;
				this.affinity=affinity;
				this.iParm=iParm;
				this.iMem=iMem;
				this.nMem=nMem;
			}
		};

		/*
    ** During code generation of statements that do inserts into AUTOINCREMENT
    ** tables, the following information is attached to the Table.u.autoInc.p
    ** pointer of each autoincrement table to record some side information that
    ** the code generator needs.  We have to keep per-table autoincrement
    ** information in case inserts are down within triggers.  Triggers do not
    ** normally coordinate their activities, but we do need to coordinate the
    ** loading and saving of autoincrement information.
    */public class AutoincInfo {
			public AutoincInfo pNext;
			/* Next info block in a list of them all */public Table pTab;
			/* Table this info block refers to */public int iDb;
			/* Index in sqlite3.aDb[] of database holding pTab */public int regCtr;
		/* Memory register holding the rowid counter */};

		///<summary>
		/// Size of the column cache
		///
		///</summary>
		#if !SQLITE_N_COLCACHE
		//# define SQLITE_N_COLCACHE 10
		private const int SQLITE_N_COLCACHE=10;
		#endif
		///<summary>
		/// At least one instance of the following structure is created for each
		/// trigger that may be fired while parsing an INSERT, UPDATE or DELETE
		/// statement. All such objects are stored in the linked list headed at
		/// Parse.pTriggerPrg and deleted once statement compilation has been
		/// completed.
		///
		/// A Vdbe sub-program that implements the body and WHEN clause of trigger
		/// TriggerPrg.pTrigger, assuming a default ON CONFLICT clause of
		/// TriggerPrg.orconf, is stored in the TriggerPrg.pProgram variable.
		/// The Parse.pTriggerPrg list never contains two entries with the same
		/// values for both pTrigger and orconf.
		///
		/// The TriggerPrg.aColmask[0] variable is set to a mask of old.* columns
		/// accessed (or set to 0 for triggers fired as a result of INSERT
		/// statements). Similarly, the TriggerPrg.aColmask[1] variable is set to
		/// a mask of new.* columns used by the program.
		///</summary>
		public class TriggerPrg {
			public Trigger pTrigger;
			/* Trigger this program was coded from */public int orconf;
			/* Default ON CONFLICT policy */public SubProgram pProgram;
			/* Program implementing pTrigger/orconf */public u32[] aColmask=new u32[2];
			/* Masks of old.*, new.* columns accessed */public TriggerPrg pNext;
		/* Next entry in Parse.pTriggerPrg list */};

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
		/// compiled. Function sqlite3TableLock() is used to add entries to the
		/// list.
		///
		///</summary>
		public class yColCache {
			public int iTable;
			/* Table cursor number */public int iColumn;
			/* Table column number */public u8 tempReg;
			/* iReg is a temp register that needs to be freed */public int iLevel;
			/* Nesting level */public int iReg;
			/* Reg with value of this column. 0 means none. */public int lru;
		/* Least recently used entry has the smallest value */}
		#if SQLITE_OMIT_VIRTUALTABLE
																																																								//define IN_DECLARE_VTAB 0
    static bool IN_DECLARE_VTAB( Parse pParse )
    {
      return false;
    }
#else
		//#define IN_DECLARE_VTAB (pParse.declareVtab)
		private static bool IN_DECLARE_VTAB(Parse pParse) {
			return pParse.declareVtab!=0;
		}
		#endif
		/*
** An instance of the following structure can be declared on a stack and used
** to save the Parse.zAuthContext value so that it can be restored later.
*/public class AuthContext {
			public string zAuthContext;
			/* Put saved Parse.zAuthContext here */public Parse pParse;
		/* The Parse structure */};

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
		private const byte OPFLAG_NCHANGE=0x01;
		private const byte OPFLAG_LASTROWID=0x02;
		private const byte OPFLAG_ISUPDATE=0x04;
		private const byte OPFLAG_APPEND=0x08;
		private const byte OPFLAG_USESEEKRESULT=0x10;
		private const byte OPFLAG_CLEARCACHE=0x20;
		/*
    * Each trigger present in the database schema is stored as an instance of
    * struct Trigger.
    *
    * Pointers to instances of struct Trigger are stored in two ways.
    * 1. In the "trigHash" hash table (part of the sqlite3* that represents the
    *    database). This allows Trigger structures to be retrieved by name.
    * 2. All triggers associated with a single table form a linked list, using the
    *    pNext member of struct Trigger. A pointer to the first element of the
    *    linked list is stored as the "pTrigger" member of the associated
    *    struct Table.
    *
    * The "step_list" member points to the first element of a linked list
    * containing the SQL statements specified as the trigger program.
    */public class Trigger {
			public string zName;
			/* The name of the trigger                        */public string table;
			/* The table or view to which the trigger applies */public u8 op;
			/* One of TK_DELETE, TK_UPDATE, TK_INSERT         */public u8 tr_tm;
			/* One of TRIGGER_BEFORE, TRIGGER_AFTER */public Expr pWhen;
			/* The WHEN clause of the expression (may be NULL) */public IdList pColumns;
			/* If this is an UPDATE OF <column-list> trigger,
the <column-list> is stored here */public Schema pSchema;
			/* Schema containing the trigger */public Schema pTabSchema;
			/* Schema containing the table */public TriggerStep step_list;
			///<summary>
			///Link list of trigger program steps
			///</summary>
			public Trigger pNext;
			/* Next trigger associated with the table */public Trigger Copy() {
				if(this==null)
					return null;
				else {
					Trigger cp=(Trigger)MemberwiseClone();
					if(pWhen!=null)
						cp.pWhen=pWhen.Copy();
					if(pColumns!=null)
						cp.pColumns=pColumns.Copy();
					if(pSchema!=null)
						cp.pSchema=pSchema.Copy();
					if(pTabSchema!=null)
						cp.pTabSchema=pTabSchema.Copy();
					if(step_list!=null)
						cp.step_list=step_list.Copy();
					if(pNext!=null)
						cp.pNext=pNext.Copy();
					return cp;
				}
			}
		};

		///<summary>
		/// A trigger is either a BEFORE or an AFTER trigger.  The following constants
		/// determine which.
		///
		/// If there are multiple triggers, you might of some BEFORE and some AFTER.
		/// In that cases, the constants below can be ORed together.
		///
		///</summary>
		private const u8 TRIGGER_BEFORE=1;
		//#define TRIGGER_BEFORE  1
		private const u8 TRIGGER_AFTER=2;
		//#define TRIGGER_AFTER   2
		/*
    * An instance of struct TriggerStep is used to store a single SQL statement
    * that is a part of a trigger-program.
    *
    * Instances of struct TriggerStep are stored in a singly linked list (linked
    * using the "pNext" member) referenced by the "step_list" member of the
    * associated struct Trigger instance. The first element of the linked list is
    * the first step of the trigger-program.
    *
    * The "op" member indicates whether this is a "DELETE", "INSERT", "UPDATE" or
    * "SELECT" statement. The meanings of the other members is determined by the
    * value of "op" as follows:
    *
    * (op == TK_INSERT)
    * orconf    -> stores the ON CONFLICT algorithm
    * pSelect   -> If this is an INSERT INTO ... SELECT ... statement, then
    *              this stores a pointer to the SELECT statement. Otherwise NULL.
    * target    -> A token holding the quoted name of the table to insert into.
    * pExprList -> If this is an INSERT INTO ... VALUES ... statement, then
    *              this stores values to be inserted. Otherwise NULL.
    * pIdList   -> If this is an INSERT INTO ... (<column-names>) VALUES ...
    *              statement, then this stores the column-names to be
    *              inserted into.
    *
    * (op == TK_DELETE)
    * target    -> A token holding the quoted name of the table to delete from.
    * pWhere    -> The WHERE clause of the DELETE statement if one is specified.
    *              Otherwise NULL.
    *
    * (op == TK_UPDATE)
    * target    -> A token holding the quoted name of the table to update rows of.
    * pWhere    -> The WHERE clause of the UPDATE statement if one is specified.
    *              Otherwise NULL.
    * pExprList -> A list of the columns to update and the expressions to update
    *              them to. See sqlite3Update() documentation of "pChanges"
    *              argument.
    *
    */public class TriggerStep {
			public u8 op;
			/* One of TK_DELETE, TK_UPDATE, TK_INSERT, TK_SELECT */public u8 orconf;
			/* OE_Rollback etc. */public Trigger pTrig;
			/* The trigger that this step is a part of */public Select pSelect;
			/* SELECT statment or RHS of INSERT INTO .. SELECT ... */public Token target;
			/* Target table for DELETE, UPDATE, INSERT */public Expr pWhere;
			/* The WHERE clause for DELETE or UPDATE steps */public ExprList pExprList;
			/* SET clause for UPDATE.  VALUES clause for INSERT */public IdList pIdList;
			/* Column names for INSERT */public TriggerStep pNext;
			/* Next in the link-list */public TriggerStep pLast;
			///<summary>
			///Last element in link-list. Valid for 1st elem only
			///</summary>
			public TriggerStep() {
				target=new Token();
			}
			public TriggerStep Copy() {
				if(this==null)
					return null;
				else {
					TriggerStep cp=(TriggerStep)MemberwiseClone();
					return cp;
				}
			}
		};

		///<summary>
		/// The following structure contains information used by the sqliteFix...
		/// routines as they walk the parse tree to make database references
		/// explicit.
		///
		///</summary>
		//typedef struct DbFixer DbFixer;
		public class DbFixer {
			public Parse pParse;
			/* The parsing context.  Error messages written here */public string zDb;
			/* Make sure all objects are contained in this database */public string zType;
			/* Type of the container - used for error messages */public Token pName;
			/* Name of the container - used for error messages */public///<summary>
			/// Initialize a DbFixer structure.  This routine must be called prior
			/// to passing the structure to one of the sqliteFixAAAA() routines below.
			///
			/// The return value indicates whether or not fixation is required.  TRUE
			/// means we do need to fix the database references, FALSE means we do not.
			///</summary>
			int sqlite3FixInit(/* The fixer to be initialized */Parse pParse,/* Error messages will be written here */int iDb,/* This is the database that must be used */string zType,/* "view", "trigger", or "index" */Token pName/* Name of the view, trigger, or index */) {
				sqlite3 db;
				if(NEVER(iDb<0)||iDb==1)
					return 0;
				db=pParse.db;
				Debug.Assert(db.nDb>iDb);
				this.pParse=pParse;
				this.zDb=db.aDb[iDb].zName;
				this.zType=zType;
				this.pName=pName;
				return 1;
			}
			public///<summary>
			/// The following set of routines walk through the parse tree and assign
			/// a specific database to all table references where the database name
			/// was left unspecified in the original SQL statement.  The pFix structure
			/// must have been initialized by a prior call to sqlite3FixInit().
			///
			/// These routines are used to make sure that an index, trigger, or
			/// view in one database does not refer to objects in a different database.
			/// (Exception: indices, triggers, and views in the TEMP database are
			/// allowed to refer to anything.)  If a reference is explicitly made
			/// to an object in a different database, an error message is added to
			/// pParse.zErrMsg and these routines return non-zero.  If everything
			/// checks out, these routines return 0.
			///</summary>
			int sqlite3FixSrcList(/* Context of the fixation */SrcList pList/* The Source list to check and modify */) {
				int i;
				string zDb;
				SrcList_item pItem;
				if(NEVER(pList==null))
					return 0;
				zDb=this.zDb;
				for(i=0;i<pList.nSrc;i++) {
					//, pItem++){
					pItem=pList.a[i];
					if(pItem.zDatabase==null) {
						pItem.zDatabase=zDb;
						// sqlite3DbStrDup( pFix.pParse.db, zDb );
					}
					else
						if(!pItem.zDatabase.Equals(zDb,StringComparison.InvariantCultureIgnoreCase)) {
							sqlite3ErrorMsg(this.pParse,"%s %T cannot reference objects in database %s",this.zType,this.pName,pItem.zDatabase);
							return 1;
						}
					#if !SQLITE_OMIT_VIEW || !SQLITE_OMIT_TRIGGER
					if(this.sqlite3FixSelect(pItem.pSelect)!=0)
						return 1;
					if(this.sqlite3FixExpr(pItem.pOn)!=0)
						return 1;
					#endif
				}
				return 0;
			}
			public int sqlite3FixSelect(/* Context of the fixation */Select pSelect/* The SELECT statement to be fixed to one database */) {
				while(pSelect!=null) {
					if(this.sqlite3FixExprList(pSelect.pEList)!=0) {
						return 1;
					}
					if(this.sqlite3FixSrcList(pSelect.pSrc)!=0) {
						return 1;
					}
					if(this.sqlite3FixExpr(pSelect.pWhere)!=0) {
						return 1;
					}
					if(this.sqlite3FixExpr(pSelect.pHaving)!=0) {
						return 1;
					}
					pSelect=pSelect.pPrior;
				}
				return 0;
			}
			public int sqlite3FixExpr(/* Context of the fixation */Expr pExpr/* The expression to be fixed to one database */) {
				while(pExpr!=null) {
					if(ExprHasAnyProperty(pExpr,EP_TokenOnly))
						break;
					if(ExprHasProperty(pExpr,EP_xIsSelect)) {
						if(this.sqlite3FixSelect(pExpr.x.pSelect)!=0)
							return 1;
					}
					else {
						if(this.sqlite3FixExprList(pExpr.x.pList)!=0)
							return 1;
					}
					if(this.sqlite3FixExpr(pExpr.pRight)!=0) {
						return 1;
					}
					pExpr=pExpr.pLeft;
				}
				return 0;
			}
			public int sqlite3FixExprList(/* Context of the fixation */ExprList pList/* The expression to be fixed to one database */) {
				int i;
				ExprList_item pItem;
				if(pList==null)
					return 0;
				for(i=0;i<pList.nExpr;i++)//, pItem++ )
				 {
					pItem=pList.a[i];
					if(this.sqlite3FixExpr(pItem.pExpr)!=0) {
						return 1;
					}
				}
				return 0;
			}
			public int sqlite3FixTriggerStep(/* Context of the fixation */TriggerStep pStep/* The trigger step be fixed to one database */) {
				while(pStep!=null) {
					if(this.sqlite3FixSelect(pStep.pSelect)!=0) {
						return 1;
					}
					if(this.sqlite3FixExpr(pStep.pWhere)!=0) {
						return 1;
					}
					if(this.sqlite3FixExprList(pStep.pExprList)!=0) {
						return 1;
					}
					pStep=pStep.pNext;
				}
				return 0;
			}
		}
		///<summary>
		/// An objected used to accumulate the text of a string where we
		/// do not necessarily know how big the string will be in the end.
		///
		///</summary>
		public class StrAccum {
			public sqlite3 db;
			/* Optional database for lookaside.  Can be NULL *///public StringBuilder zBase; /* A base allocation.  Not from malloc. */
			public StringBuilder zText;
			/* The string collected so far *///public int nChar;           /* Length of the string so far */
			//public int nAlloc;          /* Amount of space allocated in zText */
			public int mxAlloc;
			/* Maximum allowed string length */// Cannot happen under C#
			//public u8 mallocFailed;   /* Becomes true if any memory allocation fails */
			//public u8 useMalloc;        /* 0: none,  1: sqlite3DbMalloc,  2: sqlite3_malloc */
			//public u8 tooBig;           /* Becomes true if string size exceeds limits */
			public Mem Context;
			public StrAccum(int n) {
				db=null;
				//zBase = new StringBuilder( n );
				zText=new StringBuilder(n);
				//nChar = 0;
				//nAlloc = n;
				mxAlloc=0;
				//useMalloc = 0;
				//tooBig = 0;
				Context=null;
			}
			public i64 nChar {
				get {
					return zText.Length;
				}
			}
			public bool tooBig {
				get {
					return mxAlloc>0&&zText.Length>mxAlloc;
				}
			}
			public void explainAppendTerm(/* The text expression being built */int iTerm,/* Index of this term.  First is zero */string zColumn,/* Name of the column */string zOp/* Name of the operator */) {
				if(iTerm!=0)
					sqlite3StrAccumAppend(this," AND ",5);
				sqlite3StrAccumAppend(this,zColumn,-1);
				sqlite3StrAccumAppend(this,zOp,1);
				sqlite3StrAccumAppend(this,"?",1);
			}
		}
		///<summary>
		/// A pointer to this structure is used to communicate information
		/// from sqlite3Init and OP_ParseSchema into the sqlite3InitCallback.
		///
		///</summary>
		public class InitData {
			public sqlite3 db;
			/* The database being initialized */public int iDb;
			/* 0 for main database.  1 for TEMP, 2.. for ATTACHed */public string pzErrMsg;
			/* Error message stored here */public int rc;
		/* Result code stored here */}
		///<summary>
		/// Structure containing global configuration data for the SQLite library.
		///
		/// This structure also contains some state information.
		///
		///</summary>
		public class Sqlite3Config {
			public bool bMemstat;
			/* True to enable memory status */public bool bCoreMutex;
			/* True to enable core mutexing */public bool bFullMutex;
			/* True to enable full mutexing */public bool bOpenUri;
			/* True to interpret filenames as URIs */public int mxStrlen;
			/* Maximum string length */public int szLookaside;
			/* Default lookaside buffer size */public int nLookaside;
			/* Default lookaside buffer count */public sqlite3_mem_methods m;
			/* Low-level memory allocation interface */public sqlite3_mutex_methods mutex;
			/* Low-level mutex interface */public sqlite3_pcache_methods pcache;
			/* Low-level page-cache interface */public byte[] pHeap;
			/* Heap storage space */public int nHeap;
			/* Size of pHeap[] */public int mnReq,mxReq;
			/* Min and max heap requests sizes */public byte[][] pScratch2;
			/* Scratch memory */public byte[][] pScratch;
			/* Scratch memory */public int szScratch;
			/* Size of each scratch buffer */public int nScratch;
			/* Number of scratch buffers */public MemPage pPage;
			/* Page cache memory */public int szPage;
			/* Size of each page in pPage[] */public int nPage;
			/* Number of pages in pPage[] */public int mxParserStack;
			/* maximum depth of the parser stack */public bool sharedCacheEnabled;
			/* true if shared-cache mode enabled *//* The above might be initialized to non-zero.  The following need to always
      ** initially be zero, however. */public int isInit;
			/* True after initialization has finished */public int inProgress;
			/* True while initialization in progress */public int isMutexInit;
			/* True after mutexes are initialized */public int isMallocInit;
			/* True after malloc is initialized */public int isPCacheInit;
			/* True after malloc is initialized */public sqlite3_mutex pInitMutex;
			/* Mutex used by sqlite3_initialize() */public int nRefInitMutex;
			/* Number of users of pInitMutex */public dxLog xLog;
			//void (*xLog)(void*,int,const char); /* Function for logging */
			public object pLogArg;
			/* First argument to xLog() */public bool bLocaltimeFault;
			/* True to fail localtime() calls */public Sqlite3Config(int bMemstat,int bCoreMutex,bool bFullMutex,bool bOpenUri,int mxStrlen,int szLookaside,int nLookaside,sqlite3_mem_methods m,sqlite3_mutex_methods mutex,sqlite3_pcache_methods pcache,byte[] pHeap,int nHeap,int mnReq,int mxReq,byte[][] pScratch,int szScratch,int nScratch,MemPage pPage,int szPage,int nPage,int mxParserStack,bool sharedCacheEnabled,int isInit,int inProgress,int isMutexInit,int isMallocInit,int isPCacheInit,sqlite3_mutex pInitMutex,int nRefInitMutex,dxLog xLog,object pLogArg,bool bLocaltimeFault) {
				this.bMemstat=bMemstat!=0;
				this.bCoreMutex=bCoreMutex!=0;
				this.bOpenUri=bOpenUri;
				this.bFullMutex=bFullMutex;
				this.mxStrlen=mxStrlen;
				this.szLookaside=szLookaside;
				this.nLookaside=nLookaside;
				this.m=m;
				this.mutex=mutex;
				this.pcache=pcache;
				this.pHeap=pHeap;
				this.nHeap=nHeap;
				this.mnReq=mnReq;
				this.mxReq=mxReq;
				this.pScratch=pScratch;
				this.szScratch=szScratch;
				this.nScratch=nScratch;
				this.pPage=pPage;
				this.szPage=szPage;
				this.nPage=nPage;
				this.mxParserStack=mxParserStack;
				this.sharedCacheEnabled=sharedCacheEnabled;
				this.isInit=isInit;
				this.inProgress=inProgress;
				this.isMutexInit=isMutexInit;
				this.isMallocInit=isMallocInit;
				this.isPCacheInit=isPCacheInit;
				this.pInitMutex=pInitMutex;
				this.nRefInitMutex=nRefInitMutex;
				this.xLog=xLog;
				this.pLogArg=pLogArg;
				this.bLocaltimeFault=bLocaltimeFault;
			}
		};

		/*
    ** Context pointer passed down through the tree-walk.
    */public class Walker {
			public dxExprCallback xExprCallback;
			//)(Walker*, Expr);     /* Callback for expressions */
			public dxSelectCallback xSelectCallback;
			//)(Walker*,Select);  /* Callback for SELECTs */
			public Parse pParse;
			/* Parser context.  */public struct uw {
				/* Extra data for callback */public NameContext pNC;
				/* Naming context */public int i;
			/* Integer value */}
			public uw u;
			public///<summary>
			/// 2008 August 16
			///
			/// The author disclaims copyright to this source code.  In place of
			/// a legal notice, here is a blessing:
			///
			///    May you do good and not evil.
			///    May you find forgiveness for yourself and forgive others.
			///    May you share freely, never taking more than you give.
			///
			///
			/// This file contains routines used for walking the parser tree for
			/// an SQL statement.
			///
			///  Included in SQLite3 port to C#-SQLite;  2008 Noah B Hart
			///  C#-SQLite is an independent reimplementation of the SQLite software library
			///
			///  SQLITE_SOURCE_ID: 2010-08-23 18:52:01 42537b60566f288167f1b5864a5435986838e3a3
			///
			///
			///
			///</summary>
			//#include "sqliteInt.h"
			//#include <stdlib.h>
			//#include <string.h>
			///<summary>
			/// Walk an expression tree.  Invoke the callback once for each node
			/// of the expression, while decending.  (In other words, the callback
			/// is invoked before visiting children.)
			///
			/// The return value from the callback should be one of the WRC_
			/// constants to specify how to proceed with the walk.
			///
			///    WRC_Continue      Continue descending down the tree.
			///
			///    WRC_Prune         Do not descend into child nodes.  But allow
			///                      the walk to continue with sibling nodes.
			///
			///    WRC_Abort         Do no more callbacks.  Unwind the stack and
			///                      return the top-level walk call.
			///
			/// The return value from this routine is WRC_Abort to abandon the tree walk
			/// and WRC_Continue to continue.
			///
			///</summary>
			int sqlite3WalkExpr(ref Expr pExpr) {
				int rc;
				if(pExpr==null)
					return WRC_Continue;
				testcase(ExprHasProperty(pExpr,EP_TokenOnly));
				testcase(ExprHasProperty(pExpr,EP_Reduced));
				rc=this.xExprCallback(this,ref pExpr);
				if(rc==WRC_Continue&&!ExprHasAnyProperty(pExpr,EP_TokenOnly)) {
					if(this.sqlite3WalkExpr(ref pExpr.pLeft)!=0)
						return WRC_Abort;
					if(this.sqlite3WalkExpr(ref pExpr.pRight)!=0)
						return WRC_Abort;
					if(ExprHasProperty(pExpr,EP_xIsSelect)) {
						if(this.sqlite3WalkSelect(pExpr.x.pSelect)!=0)
							return WRC_Abort;
					}
					else {
						if(this.sqlite3WalkExprList(pExpr.x.pList)!=0)
							return WRC_Abort;
					}
				}
				return rc&WRC_Abort;
			}
			public///<summary>
			/// Call sqlite3WalkExpr() for every expression in list p or until
			/// an abort request is seen.
			///
			///</summary>
			int sqlite3WalkExprList(ExprList p) {
				int i;
				ExprList_item pItem;
				if(p!=null) {
					for(i=p.nExpr;i>0;i--) {
						//, pItem++){
						pItem=p.a[p.nExpr-i];
						if(this.sqlite3WalkExpr(ref pItem.pExpr)!=0)
							return WRC_Abort;
					}
				}
				return WRC_Continue;
			}
			public///<summary>
			/// Walk all expressions associated with SELECT statement p.  Do
			/// not invoke the SELECT callback on p, but do (of course) invoke
			/// any expr callbacks and SELECT callbacks that come from subqueries.
			/// Return WRC_Abort or WRC_Continue.
			///
			///</summary>
			int sqlite3WalkSelectExpr(Select p) {
				if(this.sqlite3WalkExprList(p.pEList)!=0)
					return WRC_Abort;
				if(this.sqlite3WalkExpr(ref p.pWhere)!=0)
					return WRC_Abort;
				if(this.sqlite3WalkExprList(p.pGroupBy)!=0)
					return WRC_Abort;
				if(this.sqlite3WalkExpr(ref p.pHaving)!=0)
					return WRC_Abort;
				if(this.sqlite3WalkExprList(p.pOrderBy)!=0)
					return WRC_Abort;
				if(this.sqlite3WalkExpr(ref p.pLimit)!=0)
					return WRC_Abort;
				if(this.sqlite3WalkExpr(ref p.pOffset)!=0)
					return WRC_Abort;
				return WRC_Continue;
			}
			public///<summary>
			/// Walk the parse trees associated with all subqueries in the
			/// FROM clause of SELECT statement p.  Do not invoke the select
			/// callback on p, but do invoke it on each FROM clause subquery
			/// and on any subqueries further down in the tree.  Return
			/// WRC_Abort or WRC_Continue;
			///
			///</summary>
			int sqlite3WalkSelectFrom(Select p) {
				SrcList pSrc;
				int i;
				SrcList_item pItem;
				pSrc=p.pSrc;
				if(ALWAYS(pSrc)) {
					for(i=pSrc.nSrc;i>0;i--)// pItem++ )
					 {
						pItem=pSrc.a[pSrc.nSrc-i];
						if(this.sqlite3WalkSelect(pItem.pSelect)!=0) {
							return WRC_Abort;
						}
					}
				}
				return WRC_Continue;
			}
			public int sqlite3WalkSelect(Select p) {
				int rc;
				if(p==null||this.xSelectCallback==null)
					return WRC_Continue;
				rc=WRC_Continue;
				while(p!=null) {
					rc=this.xSelectCallback(this,p);
					if(rc!=0)
						break;
					if(this.sqlite3WalkSelectExpr(p)!=0)
						return WRC_Abort;
					if(this.sqlite3WalkSelectFrom(p)!=0)
						return WRC_Abort;
					p=p.pPrior;
				}
				return rc&WRC_Abort;
			}
		}
		/* Forward declarations *///int sqlite3WalkExpr(Walker*, Expr);
		//int sqlite3WalkExprList(Walker*, ExprList);
		//int sqlite3WalkSelect(Walker*, Select);
		//int sqlite3WalkSelectExpr(Walker*, Select);
		//int sqlite3WalkSelectFrom(Walker*, Select);
		/*
    ** Return code from the parse-tree walking primitives and their
    ** callbacks.
    *///#define WRC_Continue    0   /* Continue down into children */
		//#define WRC_Prune       1   /* Omit children but continue walking siblings */
		//#define WRC_Abort       2   /* Abandon the tree walk */
		private const int WRC_Continue=0;
		private const int WRC_Prune=1;
		private const int WRC_Abort=2;
		///<summary>
		/// Assuming zIn points to the first byte of a UTF-8 character,
		/// advance zIn to point to the first byte of the next UTF-8 character.
		///
		///</summary>
		//#define SQLITE_SKIP_UTF8(zIn) {                        \
		//  if( (*(zIn++))>=0xc0 ){                              \
		//    while( (*zIn & 0xc0)==0x80 ){ zIn++; }             \
		//  }                                                    \
		//}
		private static void SQLITE_SKIP_UTF8(string zIn,ref int iz) {
			iz++;
			if(iz<zIn.Length&&zIn[iz-1]>=0xC0) {
				while(iz<zIn.Length&&(zIn[iz]&0xC0)==0x80) {
					iz++;
				}
			}
		}
		private static void SQLITE_SKIP_UTF8(byte[] zIn,ref int iz) {
			iz++;
			if(iz<zIn.Length&&zIn[iz-1]>=0xC0) {
				while(iz<zIn.Length&&(zIn[iz]&0xC0)==0x80) {
					iz++;
				}
			}
		}
		///<summary>
		/// The SQLITE_*_BKPT macros are substitutes for the error codes with
		/// the same name but without the _BKPT suffix.  These macros invoke
		/// routines that report the line-number on which the error originated
		/// using sqlite3_log().  The routines also provide a convenient place
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

    //define SQLITE_MISUSE_BKPT sqlite3MisuseError(__LINE__)
    static int SQLITE_MISUSE_BKPT()
    {
      return sqlite3MisuseError( 0 );
    }

    //define SQLITE_CANTOPEN_BKPT sqlite3CantopenError(__LINE__)
    static int SQLITE_CANTOPEN_BKPT()
    {
      return sqlite3CantopenError( 0 );
    }
#else
		private static int SQLITE_CORRUPT_BKPT() {
			return SQLITE_CORRUPT;
		}
		private static int SQLITE_MISUSE_BKPT() {
			return SQLITE_MISUSE;
		}
		private static int SQLITE_CANTOPEN_BKPT() {
			return SQLITE_CANTOPEN;
		}
		#endif
		/*
** Internal function prototypes
*///int sqlite3StrICmp(string , string );
		//int StringExtensions.sqlite3Strlen30(const char);
		//#define StringExtensions.sqlite3StrNICmp sqlite3_strnicmp
		//int sqlite3MallocInit(void);
		//void sqlite3MallocEnd(void);
		//void *sqlite3Malloc(int);
		//void *sqlite3MallocZero(int);
		//void *sqlite3DbMallocZero(sqlite3*, int);
		//void *sqlite3DbMallocRaw(sqlite3*, int);
		//char *sqlite3DbStrDup(sqlite3*,const char);
		//char *sqlite3DbStrNDup(sqlite3*,const char*, int);
		//void *sqlite3Realloc(void*, int);
		//void *sqlite3DbReallocOrFree(sqlite3 *, object  *, int);
		//void *sqlite3DbRealloc(sqlite3 *, object  *, int);
		//void sqlite3DbFree(sqlite3*, void);
		//int sqlite3MallocSize(void);
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
		//void sqlite3XPrintf(StrAccum*, const char*, ...);
		#endif
		//char *sqlite3MPrintf(sqlite3*,const char*, ...);
		//char *sqlite3VMPrintf(sqlite3*,const char*, va_list);
		//char *sqlite3MAppendf(sqlite3*,char*,const char*,...);
		#if SQLITE_TEST || SQLITE_DEBUG
																																																								    //  void sqlite3DebugPrintf(const char*, ...);
#endif
		#if SQLITE_TEST
																																																								    //  void *sqlite3TestTextToPtr(const char);
#endif
		//void sqlite3SetString(char **, sqlite3*, const char*, ...);
		//void sqlite3ErrorMsg(Parse*, const char*, ...);
		//int StringExtensions.sqlite3Dequote(char);
		//int sqlite3KeywordCode(const unsigned char*, int);
		//int sqlite3RunParser(Parse*, const char*, char *);
		//void sqlite3FinishCoding(Parse);
		//int sqlite3GetTempReg(Parse);
		//void sqlite3ReleaseTempReg(Parse*,int);
		//int sqlite3GetTempRange(Parse*,int);
		//void sqlite3ReleaseTempRange(Parse*,int,int);
		//Expr *sqlite3ExprAlloc(sqlite3*,int,const Token*,int);
		//Expr *sqlite3Expr(sqlite3*,int,const char);
		//void sqlite3ExprAttachSubtrees(sqlite3*,Expr*,Expr*,Expr);
		//Expr *sqlite3PExpr(Parse*, int, Expr*, Expr*, const Token);
		//Expr *sqlite3ExprAnd(sqlite3*,Expr*, Expr);
		//Expr *sqlite3ExprFunction(Parse*,ExprList*, Token);
		//void sqlite3ExprAssignVarNumber(Parse*, Expr);
		//void sqlite3ExprDelete(sqlite3*, Expr);
		//ExprList *sqlite3ExprListAppend(Parse*,ExprList*,Expr);
		//void sqlite3ExprListSetName(Parse*,ExprList*,Token*,int);
		//void sqlite3ExprListSetSpan(Parse*,ExprList*,ExprSpan);
		//void sqlite3ExprListDelete(sqlite3*, ExprList);
		//int sqlite3Init(sqlite3*, char*);
		//int sqlite3InitCallback(void*, int, char**, char*);
		//void sqlite3Pragma(Parse*,Token*,Token*,Token*,int);
		//void sqlite3ResetInternalSchema(sqlite3*, int);
		//void sqlite3BeginParse(Parse*,int);
		//void sqlite3CommitInternalChanges(sqlite3);
		//Table *sqlite3ResultSetOfSelect(Parse*,Select);
		//void sqlite3OpenMasterTable(Parse *, int);
		//void sqlite3StartTable(Parse*,Token*,Token*,int,int,int,int);
		//void sqlite3AddColumn(Parse*,Token);
		//void sqlite3AddNotNull(Parse*, int);
		//void sqlite3AddPrimaryKey(Parse*, ExprList*, int, int, int);
		//void sqlite3AddCheckConstraint(Parse*, Expr);
		//void sqlite3AddColumnType(Parse*,Token);
		//void sqlite3AddDefaultValue(Parse*,ExprSpan);
		//void sqlite3AddCollateType(Parse*, Token);
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
		//int sqlite3ViewGetColumnNames(Parse*,Table);
		#else
																																																								    // define sqlite3ViewGetColumnNames(A,B) 0
    static int sqlite3ViewGetColumnNames( Parse A, Table B )
    {
      return 0;
    }
#endif
		//void sqlite3DropTable(Parse*, SrcList*, int, int);
		//void sqlite3DeleteTable(sqlite3*, Table);
		//#if !SQLITE_OMIT_AUTOINCREMENT
		//  void sqlite3AutoincrementBegin(Parse *pParse);
		//  void sqlite3AutoincrementEnd(Parse *pParse);
		//#else
		//# define sqlite3AutoincrementBegin(X)
		//# define sqlite3AutoincrementEnd(X)
		//#endif
		//void sqlite3Insert(Parse*, SrcList*, ExprList*, Select*, IdList*, int);
		//void *sqlite3ArrayAllocate(sqlite3*,void*,int,int,int*,int*,int);
		//IdList *sqlite3IdListAppend(sqlite3*, IdList*, Token);
		//int sqlite3IdListIndex(IdList*,const char);
		//SrcList *sqlite3SrcListEnlarge(sqlite3*, SrcList*, int, int);
		//SrcList *sqlite3SrcListAppend(sqlite3*, SrcList*, Token*, Token);
		//SrcList *sqlite3SrcListAppendFromTerm(Parse*, SrcList*, Token*, Token*,
		//                                      Token*, Select*, Expr*, IdList);
		//void sqlite3SrcListIndexedBy(Parse *, SrcList *, Token );
		//int sqlite3IndexedByLookup(Parse *, struct SrcList_item );
		//void sqlite3SrcListShiftJoinType(SrcList);
		//void sqlite3SrcListAssignCursors(Parse*, SrcList);
		//void sqlite3IdListDelete(sqlite3*, IdList);
		//void sqlite3SrcListDelete(sqlite3*, SrcList);
		//Index *sqlite3CreateIndex(Parse*,Token*,Token*,SrcList*,ExprList*,int,Token*,
		//                        Token*, int, int);
		//void sqlite3DropIndex(Parse*, SrcList*, int);
		//int sqlite3Select(Parse*, Select*, SelectDest);
		//Select *sqlite3SelectNew(Parse*,ExprList*,SrcList*,Expr*,ExprList*,
		//                         Expr*,ExprList*,int,Expr*,Expr);
		//void sqlite3SelectDelete(sqlite3*, Select);
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
		//int sqlite3ExprCodeGetColumn(Parse*, Table*, int, int, int);
		//void sqlite3ExprCodeGetColumnOfTable(Vdbe*, Table*, int, int, int);
		//void sqlite3ExprCodeMove(Parse*, int, int, int);
		//void sqlite3ExprCodeCopy(Parse*, int, int, int);
		//void sqlite3ExprCacheStore(Parse*, int, int, int);
		//void sqlite3ExprCachePush(Parse);
		//void sqlite3ExprCachePop(Parse*, int);
		//void sqlite3ExprCacheRemove(Parse*, int, int);
		//void sqlite3ExprCacheClear(Parse);
		//void sqlite3ExprCacheAffinityChange(Parse*, int, int);
		//int sqlite3ExprCode(Parse*, Expr*, int);
		//int sqlite3ExprCodeTemp(Parse*, Expr*, int);
		//int sqlite3ExprCodeTarget(Parse*, Expr*, int);
		//int sqlite3ExprCodeAndCache(Parse*, Expr*, int);
		//void sqlite3ExprCodeConstants(Parse*, Expr);
		//int sqlite3ExprCodeExprList(Parse*, ExprList*, int, int);
		//void sqlite3ExprIfTrue(Parse*, Expr*, int, int);
		//void sqlite3ExprIfFalse(Parse*, Expr*, int, int);
		//Table *sqlite3FindTable(sqlite3*,const char*, const char);
		//Table *sqlite3LocateTable(Parse*,int isView,const char*, const char);
		//Index *sqlite3FindIndex(sqlite3*,const char*, const char);
		//void sqlite3UnlinkAndDeleteTable(sqlite3*,int,const char);
		//void sqlite3UnlinkAndDeleteIndex(sqlite3*,int,const char);
		//void sqlite3Vacuum(Parse);
		//int sqlite3RunVacuum(char**, sqlite3);
		//char *sqlite3NameFromToken(sqlite3*, Token);
		//int sqlite3ExprCompare(Expr*, Expr);
		//int sqlite3ExprListCompare(ExprList*, ExprList);
		//void sqlite3ExprAnalyzeAggregates(NameContext*, Expr);
		//void sqlite3ExprAnalyzeAggList(NameContext*,ExprList);
		//Vdbe *sqlite3GetVdbe(Parse);
		//void sqlite3PrngSaveState(void);
		//void sqlite3PrngRestoreState(void);
		//void sqlite3PrngResetState(void);
		//void sqlite3RollbackAll(sqlite3);
		//void sqlite3CodeVerifySchema(Parse*, int);
		//void sqlite3CodeVerifyNamedSchema(Parse*, string zDb);
		//void sqlite3BeginTransaction(Parse*, int);
		//void sqlite3CommitTransaction(Parse);
		//void sqlite3RollbackTransaction(Parse);
		//void sqlite3Savepoint(Parse*, int, Token);
		//void sqlite3CloseSavepoints(sqlite3 );
		//int sqlite3ExprIsConstant(Expr);
		//int sqlite3ExprIsConstantNotJoin(Expr);
		//int sqlite3ExprIsConstantOrFunction(Expr);
		//int sqlite3ExprIsInteger(Expr*, int);
		//int sqlite3ExprCanBeNull(const Expr);
		//void sqlite3ExprCodeIsNullJump(Vdbe*, const Expr*, int, int);
		//int sqlite3ExprNeedsNoAffinityChange(const Expr*, char);
		//int sqlite3IsRowid(const char);
		//void sqlite3GenerateRowDelete(Parse*, Table*, int, int, int, Trigger *, int);
		//void sqlite3GenerateRowIndexDelete(Parse*, Table*, int, int);
		//int sqlite3GenerateIndexKey(Parse*, Index*, int, int, int);
		//void sqlite3GenerateConstraintChecks(Parse*,Table*,int,int,
		//                                     int*,int,int,int,int,int);
		//void sqlite3CompleteInsertion(Parse*, Table*, int, int, int*, int, int, int);
		//int sqlite3OpenTableAndIndices(Parse*, Table*, int, int);
		//void sqlite3BeginWriteOperation(Parse*, int, int);
		//void sqlite3MultiWrite(Parse);
		//void sqlite3MayAbort(Parse );
		//void sqlite3HaltConstraint(Parse*, int, char*, int);
		//Expr *sqlite3ExprDup(sqlite3*,Expr*,int);
		//ExprList *sqlite3ExprListDup(sqlite3*,ExprList*,int);
		//SrcList *sqlite3SrcListDup(sqlite3*,SrcList*,int);
		//IdList *sqlite3IdListDup(sqlite3*,IdList);
		//Select *sqlite3SelectDup(sqlite3*,Select*,int);
		//void sqlite3FuncDefInsert(FuncDefHash*, FuncDef);
		//FuncDef *sqlite3FindFunction(sqlite3*,const char*,int,int,u8,int);
		//void sqlite3RegisterBuiltinFunctions(sqlite3);
		//void sqlite3RegisterDateTimeFunctions(void);
		//void sqlite3RegisterGlobalFunctions(void);
		//int sqlite3SafetyCheckOk(sqlite3);
		//int sqlite3SafetyCheckSickOrOk(sqlite3);
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
		//# define sqlite3ParseToplevel(p) ((p)->pToplevel ? (p)->pToplevel : (p))
		private static Parse sqlite3ParseToplevel(Parse p) {
			return p.pToplevel!=null?p.pToplevel:p;
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

    // define sqlite3ParseToplevel(p) p
    static Parse sqlite3ParseToplevel( Parse p )
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
		//void sqlite3CreateForeignKey(Parse*, ExprList*, Token*, ExprList*, int);
		//void sqlite3DeferForeignKey(Parse*, int);
		#if !SQLITE_OMIT_AUTHORIZATION
																																																								void sqlite3AuthRead(Parse*,Expr*,Schema*,SrcList);
int sqlite3AuthCheck(Parse*,int, const char*, const char*, const char);
void sqlite3AuthContextPush(Parse*, AuthContext*, const char);
void sqlite3AuthContextPop(AuthContext);
int sqlite3AuthReadCol(Parse*, string , string , int);
#else
		//# define sqlite3AuthRead(a,b,c,d)
		private static void sqlite3AuthRead(Parse a,Expr b,Schema c,SrcList d) {
		}
		//# define sqlite3AuthCheck(a,b,c,d,e)    SQLITE_OK
		private static int sqlite3AuthCheck(Parse a,int b,string c,byte[] d,byte[] e) {
			return SQLITE_OK;
		}
		//# define sqlite3AuthContextPush(a,b,c)
		private static void sqlite3AuthContextPush(Parse a,AuthContext b,string c) {
		}
		//# define sqlite3AuthContextPop(a)  ((void)(a))
		private static Parse sqlite3AuthContextPop(Parse a) {
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
		/*
    ** Routines to read and write variable-length integers.  These used to
    ** be defined locally, but now we use the varint routines in the util.c
    ** file.  Code should use the MACRO forms below, as the Varint32 versions
    ** are coded to assume the single byte case is already handled (which
    ** the MACRO form does).
    *///int sqlite3PutVarint(unsigned char*, u64);
		//int putVarint32(unsigned char*, u32);
		//u8 sqlite3GetVarint(const unsigned char *, u64 );
		//u8 sqlite3GetVarint32(const unsigned char *, u32 );
		//int sqlite3VarintLen(u64 v);
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
		///     x = getVarint32( A, B );
		///     x = putVarint32( A, B );
		///
		///
		///</summary>
		//#define getVarint32(A,B)  (u8)((*(A)<(u8)0x80) ? ((B) = (u32)*(A)),1 : sqlite3GetVarint32((A), (u32 )&(B)))
		//#define putVarint32(A,B)  (u8)(((u32)(B)<(u32)0x80) ? (*(A) = (unsigned char)(B)),1 : sqlite3PutVarint32((A), (B)))
		//#define getVarint    sqlite3GetVarint
		//#define putVarint    sqlite3PutVarint
		//string sqlite3IndexAffinityStr(Vdbe *, Index );
		//void sqlite3TableAffinityStr(Vdbe *, Table );
		//char sqlite3CompareAffinity(Expr pExpr, char aff2);
		//int sqlite3IndexAffinityOk(Expr pExpr, char idx_affinity);
		//char sqlite3ExprAffinity(Expr pExpr);
		//int Converter.sqlite3Atoi64(const char*, i64*, int, u8);
		//void sqlite3Error(sqlite3*, int, const char*,...);
		//void *Converter.sqlite3HexToBlob(sqlite3*, string z, int n);
		//u8 Converter.sqlite3HexToInt(int h);
		//int sqlite3TwoPartName(Parse *, Token *, Token *, Token *);
		//string sqlite3ErrStr(int);
		//int sqlite3ReadSchema(Parse pParse);
		//CollSeq *sqlite3FindCollSeq(sqlite3*,u8 enc, const char*,int);
		//CollSeq *sqlite3LocateCollSeq(Parse *pParse, const char*zName);
		//CollSeq *sqlite3ExprCollSeq(Parse pParse, Expr pExpr);
		//Expr *sqlite3ExprSetColl(Expr*, CollSeq);
		//Expr *sqlite3ExprSetCollByToken(Parse *pParse, Expr*, Token);
		//int sqlite3CheckCollSeq(Parse *, CollSeq );
		//int sqlite3CheckObjectName(Parse *, string );
		//void sqlite3VdbeSetChanges(sqlite3 *, int);
		//int sqlite3AddInt64(i64*,i64);
		//int sqlite3SubInt64(i64*,i64);
		//int sqlite3MulInt64(i64*,i64);
		//int sqlite3AbsInt32(int);
		#if SQLITE_ENABLE_8_3_NAMES
																																																								    //void sqlite3FileSuffix3(const char*, char);
    #else
		//# define sqlite3FileSuffix3(X,Y)
		private static void sqlite3FileSuffix3(string X,string Y) {
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
		//extern const unsigned char sqlite3UpperToLower[];
		//extern const unsigned char sqlite3CtypeMap[];
		//extern const Token sqlite3IntTokens[];
		//extern SQLITE_WSD struct Sqlite3Config sqlite3Config;
		//extern SQLITE_WSD FuncDefHash sqlite3GlobalFunctions;
		//#if !SQLITE_OMIT_WSD
		//extern int sqlite3PendingByte;
		//#endif
		//#endif
		//void sqlite3RootPageMoved(sqlite3*, int, int, int);
		//void sqlite3Reindex(Parse*, Token*, Token);
		//void sqlite3AlterFunctions(void);
		//void sqlite3AlterRenameTable(Parse*, SrcList*, Token);
		//int sqlite3GetToken(const unsigned char *, int );
		//void sqlite3NestedParse(Parse*, const char*, ...);
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
		//int sqlite3FindDb(sqlite3*, Token);
		//int sqlite3FindDbName(sqlite3 *, string );
		//int sqlite3AnalysisLoad(sqlite3*,int iDB);
		//void sqlite3DeleteIndexSamples(sqlite3*,Index);
		//void sqlite3DefaultRowEst(Index);
		//void sqlite3RegisterLikeFunctions(sqlite3*, int);
		//int sqlite3IsLikeFunction(sqlite3*,Expr*,int*,char);
		//void sqlite3MinimumFileFormat(Parse*, int, int);
		//void sqlite3SchemaClear(void );
		//Schema *sqlite3SchemaGet(sqlite3 *, Btree );
		//int sqlite3SchemaToIndex(sqlite3 db, Schema );
		//KeyInfo *sqlite3IndexKeyinfo(Parse *, Index );
		//int sqlite3CreateFunc(sqlite3 *, string , int, int, object  *, 
		//  void ()(sqlite3_context*,int,sqlite3_value *),
		//  void ()(sqlite3_context*,int,sqlite3_value *), object  ()(sqlite3_context),
		//  FuncDestructor *pDestructor
		//);
		//int sqlite3ApiExit(sqlite3 db, int);
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
																																																								//void sqlite3TableLock(Parse *, int, int, u8, string );
#else
		//#define sqlite3TableLock(v,w,x,y,z)
		private static void sqlite3TableLock(Parse p,int p1,int p2,u8 p3,byte[] p4) {
		}
		private static void sqlite3TableLock(Parse p,int p1,int p2,u8 p3,string p4) {
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

    //  define sqlite3VtabSync(X,Y) SQLITE_OK
    static int sqlite3VtabSync( sqlite3 X, ref string Y )
    {
      return SQLITE_OK;
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
    //  define sqlite3VtabSavepoint(X, Y, Z) SQLITE_OK
    static int sqlite3VtabSavepoint( sqlite3 X, int Y, int Z )
    {
      return SQLITE_OK;
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
		private static bool sqlite3VtabInSync(sqlite3 db) {
			return (db.nVTrans>0&&db.aVTrans==null);
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
		//int sqlite3VdbeParameterIndex(Vdbe*, const char*, int);
		//int sqlite3TransferBindings(sqlite3_stmt *, sqlite3_stmt );
		//int sqlite3Reprepare(Vdbe);
		//void sqlite3ExprListCheckLength(Parse*, ExprList*, const char);
		//CollSeq *sqlite3BinaryCompareCollSeq(Parse *, Expr *, Expr );
		//int sqlite3TempInMemory(const sqlite3);
		//VTable *sqlite3GetVTable(sqlite3*, Table);
		//string sqlite3JournalModename(int);
		//int sqlite3Checkpoint(sqlite3*, int, int, int*, int);
		//int sqlite3WalDefaultHook(void*,sqlite3*,const char*,int);
		/* Declarations for functions in fkey.c. All of these are replaced by
    ** no-op macros if OMIT_FOREIGN_KEY is defined. In this case no foreign
    ** key functionality is available. If OMIT_TRIGGER is defined but
    ** OMIT_FOREIGN_KEY is not, only some of the functions are no-oped. In
    ** this case foreign keys are parsed, but no other functionality is 
    ** provided (enforcement of FK constraints requires the triggers sub-system).
    */
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
		/*
** Available fault injectors.  Should be numbered beginning with 0.
*/private const int SQLITE_FAULTINJECTOR_MALLOC=0;
		//#define SQLITE_FAULTINJECTOR_MALLOC     0
		private const int SQLITE_FAULTINJECTOR_COUNT=1;
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
		private const int IN_INDEX_ROWID=1;
		//#define IN_INDEX_ROWID           1
		private const int IN_INDEX_EPH=2;
		//#define IN_INDEX_EPH             2
		private const int IN_INDEX_INDEX=3;
		//#define IN_INDEX_INDEX           3
		//int sqlite3FindInIndex(Parse *, Expr *, int);
		#if SQLITE_ENABLE_ATOMIC_WRITE
																																																								//  int sqlite3JournalOpen(sqlite3_vfs *, string , sqlite3_file *, int, int);
//  int sqlite3JournalSize(sqlite3_vfs );
//  int sqlite3JournalCreate(sqlite3_file );
#else
		//#define sqlite3JournalSize(pVfs) ((pVfs)->szOsFile)
		private static int sqlite3JournalSize(sqlite3_vfs pVfs) {
			return pVfs.szOsFile;
		}
		#endif
		//void sqlite3MemJournalOpen(sqlite3_file );
		//int sqlite3MemJournalSize(void);
		//int sqlite3IsMemJournal(sqlite3_file );
		#if SQLITE_MAX_EXPR_DEPTH
		//  void sqlite3ExprSetHeight(Parse pParse, Expr p);
		//  int sqlite3SelectExprHeight(Select );
		//int sqlite3ExprCheckHeight(Parse*, int);
		#else
																																																								//define sqlite3ExprSetHeight(x,y)
//define sqlite3SelectExprHeight(x) 0
//define sqlite3ExprCheckHeight(x,y)
#endif
		//u32 sqlite3Get4byte(const u8);
		//void sqlite3sqlite3Put4byte(u8*, u32);
		#if SQLITE_ENABLE_UNLOCK_NOTIFY
																																																								void sqlite3ConnectionBlocked(sqlite3 *, sqlite3 );
void sqlite3ConnectionUnlocked(sqlite3 db);
void sqlite3ConnectionClosed(sqlite3 db);
#else
		private static void sqlite3ConnectionBlocked(sqlite3 x,sqlite3 y) {
		}
		//#define sqlite3ConnectionBlocked(x,y)
		private static void sqlite3ConnectionUnlocked(sqlite3 x) {
		}
		//#define sqlite3ConnectionUnlocked(x)
		private static void sqlite3ConnectionClosed(sqlite3 x) {
		}
		//#define sqlite3ConnectionClosed(x)
		#endif
		#if SQLITE_DEBUG
																																																								    //  void sqlite3ParserTrace(FILE*, char );
#endif
		///<summary>
		/// If the SQLITE_ENABLE IOTRACE exists then the global variable
		/// sqlite3IoTrace is a pointer to a printf-like routine used to
		/// print I/O tracing messages.
		///</summary>
		#if SQLITE_ENABLE_IOTRACE
																																																								static bool SQLite3IoTrace = false;
//define IOTRACE(A)  if( sqlite3IoTrace ){ sqlite3IoTrace A; }
static void IOTRACE( string X, params object[] ap ) { if ( SQLite3IoTrace ) { printf( X, ap ); } }

//  void sqlite3VdbeIOTraceSql(Vdbe);
//SQLITE_EXTERN void (*sqlite3IoTrace)(const char*,...);
#else
		//#define IOTRACE(A)
		private static void IOTRACE(string F,params object[] ap) {
		}
		//#define sqlite3VdbeIOTraceSql(X)
		private static void sqlite3VdbeIOTraceSql(Vdbe X) {
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
		/// sqlite3MemdebugNoType() returns true if none of the bits in its second
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
//  int sqlite3MemdebugNoType(void*,u8);
#else
		//# define sqlite3MemdebugSetType(X,Y)  /* no-op */
		private static void sqlite3MemdebugSetType<T>(T X,MemType Y) {
		}
		//# define sqlite3MemdebugHasType(X,Y)  1
		private static bool sqlite3MemdebugHasType<T>(T X,MemType Y) {
			return true;
		}
		//# define sqlite3MemdebugNoType(X,Y)   1
		private static bool sqlite3MemdebugNoType<T>(T X,MemType Y) {
			return true;
		}
	#endif
	//#endif //* _SQLITEINT_H_ */
	}
	public enum MemType {
		//#define MEMTYPE_HEAP       0x01  /* General heap allocations */
		//#define MEMTYPE_LOOKASIDE  0x02  /* Might have been lookaside memory */
		//#define MEMTYPE_SCRATCH    0x04  /* Scratch allocations */
		//#define MEMTYPE_PCACHE     0x08  /* Page cache allocations */
		//#define MEMTYPE_DB         0x10  /* Uses sqlite3DbMalloc, not sqlite_malloc */
		HEAP=0x01,
		LOOKASIDE=0x02,
		SCRATCH=0x04,
		PCACHE=0x08,
		DB=0x10
	}
	public partial class CharExtensions {
		static byte[] sqlite3CtypeMap {
			get {
				return Sqlite3.sqlite3CtypeMap;
			}
		}
		/*
** FTS4 is really an extension for FTS3.  It is enabled using the
** SQLITE_ENABLE_FTS3 macro.  But to avoid confusion we also all
** the SQLITE_ENABLE_FTS4 macro to serve as an alisse for SQLITE_ENABLE_FTS3.
*///#if (SQLITE_ENABLE_FTS4) && !defined(SQLITE_ENABLE_FTS3)
		//# define SQLITE_ENABLE_FTS3
		//#endif
		/*
        ** The ctype.h header is needed for non-ASCII systems.  It is also
        ** needed by FTS3 when FTS3 is included in the amalgamation.
        *///#if !defined(SQLITE_ASCII) || \
		//    (defined(SQLITE_ENABLE_FTS3) && defined(SQLITE_AMALGAMATION))
		//# include <ctype.h>
		//#endif
		/*
        ** The following macros mimic the standard library functions toupper(),
        ** isspace(), isalnum(), isdigit() and isxdigit(), respectively. The
        ** sqlite versions only work for ASCII characters, regardless of locale.
        */
		#if SQLITE_ASCII
		//# define sqlite3Toupper(x)  ((x)&~(sqlite3CtypeMap[(unsigned char)(x)]&0x20))
		//# define CharExtensions.sqlite3Isspace(x)   (sqlite3CtypeMap[(unsigned char)(x)]&0x01)
		//private 
		public static bool sqlite3Isspace(byte x) {
			return (Sqlite3.sqlite3CtypeMap[(byte)(x)]&0x01)!=0;
		}
		//private
		public static bool sqlite3Isspace(char x) {
			return x<256&&(sqlite3CtypeMap[(byte)(x)]&0x01)!=0;
		}
		//# define sqlite3Isalnum(x)   (sqlite3CtypeMap[(unsigned char)(x)]&0x06)
		public static bool sqlite3Isalnum(byte x) {
			return (sqlite3CtypeMap[(byte)(x)]&0x06)!=0;
		}
		public static bool sqlite3Isalnum(char x) {
			return x<256&&(sqlite3CtypeMap[(byte)(x)]&0x06)!=0;
		}
		//# define sqlite3Isalpha(x)   (sqlite3CtypeMap[(unsigned char)(x)]&0x02)
		//# define sqlite3Isdigit(x)   (sqlite3CtypeMap[(unsigned char)(x)]&0x04)
		public static bool sqlite3Isdigit(byte x) {
			return (sqlite3CtypeMap[((byte)x)]&0x04)!=0;
		}
		public static bool sqlite3Isdigit(char x) {
			return x<256&&(sqlite3CtypeMap[((byte)x)]&0x04)!=0;
		}
		//# define sqlite3Isxdigit(x)  (sqlite3CtypeMap[(unsigned char)(x)]&0x08)
		public static bool sqlite3Isxdigit(byte x) {
			return (sqlite3CtypeMap[((byte)x)]&0x08)!=0;
		}
		public static bool sqlite3Isxdigit(char x) {
			return x<256&&(sqlite3CtypeMap[((byte)x)]&0x08)!=0;
		}
	//# define sqlite3Tolower(x)   (sqlite3UpperToLower[(unsigned char)(x)])
	#else
																												// define sqlite3Toupper(x)   toupper((unsigned char)(x))
// define CharExtensions.sqlite3Isspace(x)   isspace((unsigned char)(x))
// define sqlite3Isalnum(x)   isalnum((unsigned char)(x))
// define sqlite3Isalpha(x)   isalpha((unsigned char)(x))
// define sqlite3Isdigit(x)   isdigit((unsigned char)(x))
// define sqlite3Isxdigit(x)  isxdigit((unsigned char)(x))
// define sqlite3Tolower(x)   tolower((unsigned char)(x))
#endif
	}
	public class IntegerExtensions {
		/*
** Constants for the largest and smallest possible 64-bit signed integers.
** These macros are designed to work correctly on both 32-bit and 64-bit
** compilers.
*///#define LARGEST_INT64  (0xffffffff|(((i64)0x7fffffff)<<32))
		//#define SMALLEST_INT64 (((i64)-1) - LARGEST_INT64)
		public const i64 LARGEST_INT64=i64.MaxValue;
		//( 0xffffffff | ( ( (i64)0x7fffffff ) << 32 ) );
		public const i64 SMALLEST_INT64=i64.MinValue;
	//( ( ( i64 ) - 1 ) - LARGEST_INT64 );
	}
}
