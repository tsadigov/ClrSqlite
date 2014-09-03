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
namespace Community.CsharpSqlite {
	using sqlite3_value=Sqlite3.Mem;
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
		///<summary>
		///The number of samples of an index that SQLite takes in order to 
		///construct a histogram of the table content when running ANALYZE
		///and with SQLITE_ENABLE_STAT2
		///</summary>
		//#define SQLITE_INDEX_SAMPLES 10
		public const int SQLITE_INDEX_SAMPLES=10;
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
		private const int SQLITE_THREADSAFE=2;
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
		private const int SQLITE_DEFAULT_MEMSTATUS=0;
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
		private const int SQLITE_MALLOC_SOFT_LIMIT=1024;
		#endif
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
		///
		///<summary>
		///The TESTONLY macro is used to enclose variable declarations or
		///other bits of code that are needed to support the arguments
		///within testcase() and Debug.Assert() macros.
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
		///
		///<summary>
		///</summary>
		///<param name="Return true (non">zero) if the input is a integer that is too large</param>
		///<param name="to fit in 32">bits.  This macro is used inside of various testcase()</param>
		///<param name="macros to verify that we have tested SQLite for large">file support.</param>
		private static bool IS_BIG_INT(i64 X) {
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
		private const double SQLITE_BIG_DBL=(((sqlite3_int64)1)<<60);
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
		private static int OMIT_TEMPDB=0;
		#endif
		///
		///<summary>
		///The "file format" number is an integer that is incremented whenever
		///</summary>
		///<param name="the VDBE">level file format changes.  The following macros define the</param>
		///<param name="the default file format for new databases and the maximum file format">the default file format for new databases and the maximum file format</param>
		///<param name="that the library can read.">that the library can read.</param>
		public static int SQLITE_MAX_FILE_FORMAT=4;
		//#define SQLITE_MAX_FILE_FORMAT 4
		//#if !SQLITE_DEFAULT_FILE_FORMAT
		private static int SQLITE_DEFAULT_FILE_FORMAT=1;
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
		public static bool SQLITE_DEFAULT_RECURSIVE_TRIGGERS=false;
		#else
																																																												static public bool SQLITE_DEFAULT_RECURSIVE_TRIGGERS = true;
#endif
		///
		///<summary>
		///Provide a default value for SQLITE_TEMP_STORE in case it is not specified
		///</summary>
		///<param name="on the command">line</param>
		//#if !SQLITE_TEMP_STORE
		private static int SQLITE_TEMP_STORE=1;
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
		private const int SQLITE_ASCII=1;
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
		///SQLITE_MAX_U32 is a u64 constant that is the maximum u64 value
		///that can be stored in a u32 without loss of data.  The value
		///is 0x00000000ffffffff.  But because of quirks of some compilers, we
		///have to specify the value in the less intuitive manner shown:
		///
		///</summary>
		//#define SQLITE_MAX_U32  ((((u64)1)<<32)-1)
		private const u32 SQLITE_MAX_U32=(u32)((((u64)1)<<32)-1);
		///
		///<summary>
		///Macros to determine whether the machine is big or little endian,
		///evaluated at runtime.
		///
		///</summary>
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
			///
			///<summary>
			///First arg to busy callback 
			///</summary>
			public int nBusy;
		///
		///<summary>
		///Incremented with each busy call 
		///</summary>
		};

		///
		///<summary>
		///Name of the master database table.  The master database table
		///is a special table that holds the names and attributes of all
		///user tables and indices.
		///
		///</summary>
		private const string MASTER_NAME="sqlite_master";
		//#define MASTER_NAME       "sqlite_master"
		private const string TEMP_MASTER_NAME="sqlite_temp_master";
		//#define TEMP_MASTER_NAME  "sqlite_temp_master"
		///<summary>
		/// The root-page of the master database table.
		///
		///</summary>
		private const int MASTER_ROOT=1;
		//#define MASTER_ROOT       1
		///
		///<summary>
		///The name of the schema table.
		///
		///</summary>
		private static string SCHEMA_TABLE(int x)//#define SCHEMA_TABLE(x)  ((!OMIT_TEMPDB)&&(x==1)?TEMP_MASTER_NAME:MASTER_NAME)
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
		///
		///<summary>
		///The following value as a destructor means to use sqlite3DbFree().
		///This is an internal extension to SQLITE_STATIC and SQLITE_TRANSIENT.
		///
		///</summary>
		//#define SQLITE_DYNAMIC   ((sqlite3_destructor_type)sqlite3DbFree)
		private static dxDel SQLITE_DYNAMIC;
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
		///
		///<summary>
		///Forward references to structures
		///
		///</summary>
		///
		///<summary>
		///An instance of the following structure stores a database schema.
		///
		///Most Schema objects are associated with a Btree.  The exception is
		///</summary>
		///<param name="the Schema for the TEMP databaes (sqlite3.aDb[1]) which is free">standing.</param>
		///<param name="In shared cache mode, a single Schema object can be shared by multiple">In shared cache mode, a single Schema object can be shared by multiple</param>
		///<param name="Btrees that refer to the same underlying BtShared object.">Btrees that refer to the same underlying BtShared object.</param>
		///<param name=""></param>
		///<param name="Schema objects are automatically deallocated when the last Btree that">Schema objects are automatically deallocated when the last Btree that</param>
		///<param name="references them is destroyed.   The TEMP Schema is manually freed by">references them is destroyed.   The TEMP Schema is manually freed by</param>
		///<param name="sqlite3_close().">sqlite3_close().</param>
		///<param name=""></param>
		///<param name="A thread must be holding a mutex on the corresponding Btree in order">A thread must be holding a mutex on the corresponding Btree in order</param>
		///<param name="to access Schema content.  This implies that the thread must also be">to access Schema content.  This implies that the thread must also be</param>
		///<param name="holding a mutex on the sqlite3 connection pointer that owns the Btree.">holding a mutex on the sqlite3 connection pointer that owns the Btree.</param>
		///<param name="For a TEMP Schema, only the connection mutex is required.">For a TEMP Schema, only the connection mutex is required.</param>
		///<param name=""></param>
		public class Schema {
			public int schema_cookie;
			///
			///<summary>
			///Database schema version number for this file 
			///</summary>
			public u32 iGeneration;
			///
			///<summary>
			///Generation counter.  Incremented with each change 
			///</summary>
			public Hash tblHash=new Hash();
			///
			///<summary>
			///All tables indexed by name 
			///</summary>
			public Hash idxHash=new Hash();
			///
			///<summary>
			///All (named) indices indexed by name 
			///</summary>
			public Hash trigHash=new Hash();
			///
			///<summary>
			///All triggers indexed by name 
			///</summary>
			public Hash fkeyHash=new Hash();
			///
			///<summary>
			///All foreign keys by referenced table name 
			///</summary>
			public Table pSeqTab;
			///
			///<summary>
			///The sqlite_sequence table used by AUTOINCREMENT 
			///</summary>
			public u8 file_format;
			///
			///<summary>
			///Schema format version for this file 
			///</summary>
			public SqliteEncoding enc;
			///
			///<summary>
			///Text encoding used by this database 
			///</summary>
			public u16 flags;
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
			///
			///<summary>
			///Size of each buffer in bytes 
			///</summary>
			public u8 bEnabled;
			///
			///<summary>
			///False to disable new lookaside allocations 
			///</summary>
			public bool bMalloced;
			///
			///<summary>
			///True if pStart obtained from sqlite3_malloc() 
			///</summary>
			public int nOut;
			///
			///<summary>
			///Number of buffers currently checked out 
			///</summary>
			public int mxOut;
			///
			///<summary>
			///Highwater mark for nOut 
			///</summary>
			public int[] anStat=new int[3];
			///
			///<summary>
			///0: hits.  1: size misses.  2: full misses 
			///</summary>
			public LookasideSlot pFree;
			///
			///<summary>
			///List of available buffers 
			///</summary>
			public int pStart;
			///
			///<summary>
			///First byte of available memory space 
			///</summary>
			public int pEnd;
		///
		///<summary>
		///First byte past end of available space 
		///</summary>
		};

		public class LookasideSlot {
			public LookasideSlot pNext;
		///
		///<summary>
		///Next buffer in the list of free buffers 
		///</summary>
		};

		///<summary>
		/// A hash table for function definitions.
		///
		/// Hash each FuncDef structure into one of the FuncDefHash.a[] slots.
		/// Collisions are on the FuncDef.pHash chain.
		///
		///</summary>
		public class FuncDefHash {
			public FuncDef[] a=new FuncDef[23];
		///
		///<summary>
		///Hash table for functions 
		///</summary>
		};

		///<summary>
		/// A macro to discover the encoding of a database.
		///
		///</summary>
		//#define ENC(db) ((db)->aDb[0].pSchema->enc)
		private static SqliteEncoding ENC(sqlite3 db) {
			return db.aDb[0].pSchema.enc;
		}
		///
		///<summary>
		///Possible values for the sqlite3.flags.
		///
		///</summary>
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
		///
		
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
			///
			///<summary>
			///</summary>
			///<param name="Savepoint name (nul">terminated) </param>
			public i64 nDeferredCons;
			///
			///<summary>
			///Number of deferred fk violations 
			///</summary>
			public Savepoint pNext;
		///
		///<summary>
		///Parent savepoint (if any) 
		///</summary>
		};

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
			///
			///<summary>
			///Callback pointers 
			///</summary>
			public string zName;
			///
			///<summary>
			///Name passed to create_module() 
			///</summary>
			public object pAux;
			///
			///<summary>
			///pAux passed to create_module() 
			///</summary>
			public smdxDestroy xDestroy;
		//)(void );/* Module destructor function */
		};

		///
		///<summary>
		///Allowed values of CollSeq.type:
		///
		///</summary>
		public enum CollationType {
			BINARY=1,
			//#define SQLITE_COLL_BINARY  1  /* The default memcmp() collating sequence */
			NOCASE=2,
			//#define SQLITE_COLL_NOCASE  2  /* The built-in NOCASE collating sequence */
			REVERSE=3,
			//#define SQLITE_COLL_REVERSE 3  /* The built-in REVERSE collating sequence */
			USER=0
		//#define SQLITE_COLL_USER    0  /* Any other user-defined collating sequence */
		}
		///
		///<summary>
		///A sort order can be either ASC or DESC.
		///
		///</summary>
		private const int SQLITE_SO_ASC=0;
		//#define SQLITE_SO_ASC       0  /* Sort in ascending order */
		private const int SQLITE_SO_DESC=1;
		//#define SQLITE_SO_DESC     1  /* Sort in ascending order */
		///
		///<summary>
		///Column affinity types.
		///
		///These used to have mnemonic name like 'i' for SQLITE_AFF_INTEGER and
		///'t' for SQLITE_AFF_TEXT.  But we can save a little space and improve
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
		private const char SQLITE_AFF_TEXT='a';
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
		///
		///<summary>
		///The SQLITE_AFF_MASK values masks off the significant bits of an
		///affinity value.
		///
		///</summary>
		private const int SQLITE_AFF_MASK=0x67;
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
			///
			///<summary>
			///Database connection associated with this table 
			///</summary>
			public Module pMod;
			///
			///<summary>
			///Pointer to module implementation 
			///</summary>
			public sqlite3_vtab pVtab;
			///
			///<summary>
			///Pointer to vtab instance 
			///</summary>
			public int nRef;
			///
			///<summary>
			///Number of pointers to this structure 
			///</summary>
			public u8 bConstraint;
			///
			///<summary>
			///True if constraints are supported 
			///</summary>
			public int iSavepoint;
			///
			///<summary>
			///Depth of the SAVEPOINT stack 
			///</summary>
			public VTable pNext;
		///
		///<summary>
		///Next in linked list (see above) 
		///</summary>
		};

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
			///
			///<summary>
			///For each column used in source tables 
			///</summary>
			public Table pTab;
			///
			///<summary>
			///Source table 
			///</summary>
			public int iTable;
			///
			///<summary>
			///VdbeCursor number of the source table 
			///</summary>
			public int iColumn;
			///
			///<summary>
			///Column number within the source table 
			///</summary>
			public int iSorterColumn;
			///
			///<summary>
			///Column number in the sorting index 
			///</summary>
			public int iMem;
			///
			///<summary>
			///Memory location that acts as accumulator 
			///</summary>
			public Expr pExpr;
		///
		///<summary>
		///The original expression 
		///</summary>
		};

		public class AggInfo_func {
			///
			///<summary>
			///For each aggregate function 
			///</summary>
			public Expr pExpr;
			///
			///<summary>
			///Expression encoding the function 
			///</summary>
			public FuncDef pFunc;
			///
			///<summary>
			///The aggregate function implementation 
			///</summary>
			public int iMem;
			///
			///<summary>
			///Memory location that acts as accumulator 
			///</summary>
			public int iDistinct;
		///
		///<summary>
		///Ephemeral table used to enforce DISTINCT 
		///</summary>
		}
		public class AggInfo {
			public u8 directMode;
			///
			///<summary>
			///Direct rendering mode means take data directly
			///from source tables rather than from accumulators 
			///</summary>
			public u8 useSortingIdx;
			///
			///<summary>
			///In direct mode, reference the sorting index rather
			///than the source table 
			///</summary>
			public int sortingIdx;
			///
			///<summary>
			///VdbeCursor number of the sorting index 
			///</summary>
			public ExprList pGroupBy;
			///
			///<summary>
			///The group by clause 
			///</summary>
			public int nSortingColumn;
			///
			///<summary>
			///Number of columns in the sorting index 
			///</summary>
			public AggInfo_col[] aCol;
			public int nColumn;
			///
			///<summary>
			///Number of used entries in aCol[] 
			///</summary>
			public int nColumnAlloc;
			///
			///<summary>
			///Number of slots allocated for aCol[] 
			///</summary>
			public int nAccumulator;
			///
			///<summary>
			///Number of columns that show through to the output.
			///Additional columns are used only as parameters to
			///aggregate functions 
			///</summary>
			public AggInfo_func[] aFunc;
			public int nFunc;
			///<summary>
			///Number of entries in aFunc[]
			///</summary>
			public int nFuncAlloc;
			///
			///<summary>
			///Number of slots allocated for aFunc[] 
			///</summary>
			public AggInfo Copy() {
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
    */
        
        public class Expr {
			public Expr() {
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
			public TokenType Operator {
				get {
					return _op;
				}
				set {
					_op=value;
				}
			}
			public u8 op {
				get {
					return (u8)_op;
				}
				set {
					_op=(TokenType)value;
				}
			}
			///
			///<summary>
			///Operation performed by this node 
			///</summary>
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
				///
				///<summary>
				///Token value. Zero terminated and dequoted 
				///</summary>
				public int iValue;
			///
			///<summary>
			///</summary>
			///<param name="Non">negative integer value if EP_IntValue </param>
			}
			public u16 flags;
			///
			///<summary>
			///Various flags.  EP_* See below 
			///</summary>
			#endif
			public _u u;
			///
			///<summary>
			///If the EP_TokenOnly flag is set in the Expr.flags mask, then no
			///space is allocated for the fields below this point. An attempt to
			///access them will result in a segfault or malfunction.
			///
			///</summary>
			public Expr pLeft;
			///<summary>
			///Left subnode
			///</summary>
			public Expr pRight;
			///
			///<summary>
			///Right subnode 
			///</summary>
			public struct _x {
				public ExprList pList;
				///
				///<summary>
				///</summary>
				///<param name="Function arguments or in "<expr> IN (<expr">list)" </param>
				public Select pSelect;
			///
			///<summary>
			///</summary>
			///<param name="Used for sub">selects and "<expr> IN (<select>)" </param>
			}
			public _x x;
			public CollSeq pColl;
			///
			///<summary>
			///The collation type of the column or 0 
			///</summary>
			///
			///<summary>
			///If the EP_Reduced flag is set in the Expr.flags mask, then no
			///space is allocated for the fields below this point. An attempt to
			///access them will result in a segfault or malfunction.
			///
			///</summary>
			public int iTable;
			///
			///<summary>
			///TK_COLUMN: cursor number of table holding column
			///TK_REGISTER: register number
			///</summary>
			///<param name="TK_TRIGGER: 1 ">> old </param>
			public ynVar iColumn;
			///
			///<summary>
			///</summary>
			///<param name="TK_COLUMN: column index.  ">1 for rowid.</param>
			///<param name="TK_VARIABLE: variable number (always >= 1). ">TK_VARIABLE: variable number (always >= 1). </param>
			public i16 iAgg;
			///
			///<summary>
			///</summary>
			///<param name="Which entry in pAggInfo">>aFunc[] </param>
			public i16 iRightJoinTable;
			///
			///<summary>
			///If EP_FromJoin, the right table of the join 
			///</summary>
			public u8 flags2;
			///
			///<summary>
			///Second set of flags.  EP2_... 
			///</summary>
			public u8 op2;
			///
			///<summary>
			///If a TK_REGISTER, the original value of Expr.op 
			///</summary>
			public AggInfo pAggInfo;
			///
			///<summary>
			///Used by TK_AGG_COLUMN and TK_AGG_FUNCTION 
			///</summary>
			public Table pTab;
			///
			///<summary>
			///Table for TK_COLUMN expressions. 
			///</summary>
			#if SQLITE_MAX_EXPR_DEPTH
			public int nHeight;
			///
			///<summary>
			///Height of the tree headed by this node 
			///</summary>
			public Table pZombieTab;
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
					///
					///<summary>
					///Both sides of the comparison are columns. If one has numeric
					///affinity, use that. Otherwise use no affinity.
					///
					///</summary>
					if(aff1>=SQLITE_AFF_NUMERIC||aff2>=SQLITE_AFF_NUMERIC)//        if (sqlite3IsNumericAffinity(aff1) || sqlite3IsNumericAffinity(aff2))
					 {
						return SQLITE_AFF_NUMERIC;
					}
					else {
						return SQLITE_AFF_NONE;
					}
				}
				else
					if(aff1=='\0'&&aff2=='\0') {
						///
						///<summary>
						///Neither side of the comparison is a column.  Compare the
						///results directly.
						///
						///</summary>
						return SQLITE_AFF_NONE;
					}
					else {
						///
						///<summary>
						///One side is a column, the other is not. Use the columns affinity. 
						///</summary>
						Debug.Assert(aff1==0||aff2==0);
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
					if(this.ExprHasProperty(EP_xIsSelect)) {
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
				///
				///<summary>
				///Only one flag value allowed 
				///</summary>
				if(0==(flags&EXPRDUP_REDUCE)) {
					nSize=EXPR_FULLSIZE;
				}
				else {
					Debug.Assert(!this.ExprHasAnyProperty(EP_TokenOnly|EP_Reduced));
					Debug.Assert(!this.ExprHasProperty(EP_FromJoin));
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
				if(!this.ExprHasProperty(EP_IntValue)&&this.u.zToken!=null) {
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
				///
				///<summary>
				///</summary>
				///<param name="If an expression is an integer literal that fits in a signed 32">bit</param>
				///<param name="integer, then the EP_IntValue flag will have already been set ">integer, then the EP_IntValue flag will have already been set </param>
				Debug.Assert(this.op!=TK_INTEGER||(this.flags&EP_IntValue)!=0||!Converter.sqlite3GetInt32(this.u.zToken,ref rc));
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
					///
					///<summary>
					///Only constant expressions are appropriate for factoring 
					///</summary>
				}
				if((expr.flags&EP_FixedDest)==0) {
					return 1;
					///
					///<summary>
					///Any constant without a fixed destination is appropriate 
					///</summary>
				}
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
					///
					///<summary>
					///</summary>
					///<param name="Single">instruction constants with a fixed destination are</param>
					///<param name="better done in">line.  If we factor them, they will just end</param>
					///<param name="up generating an OP_SCopy to move the value to the destination">up generating an OP_SCopy to move the value to the destination</param>
					///<param name="register. ">register. </param>
					return 0;
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
					Debug.Assert(!this.ExprHasProperty(EP_IntValue));
					return sqlite3AffinityType(this.u.zToken);
				}
				#endif
				if((op==TK_AGG_COLUMN||op==TK_COLUMN||op==TK_REGISTER)&&this.pTab!=null) {
					///
					///<summary>
					///op==TK_REGISTER && pExpr.pTab!=0 happens when pExpr was originally
					///a TK_COLUMN but was previously evaluated and cached in a register 
					///</summary>
					int j=this.iColumn;
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
				if(this.ExprHasProperty(EP_xIsSelect)) {
					this.x.pSelect.heightOfSelect(ref nHeight);
				}
				else {
					this.x.pList.heightOfExprList(ref nHeight);
				}
				this.nHeight=nHeight+1;
			}
			public int isMatchOfColumn(///
			///<summary>
			///Test this expression 
			///</summary>
			) {
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
			public TokenType Operator2 {
				get {
					return (TokenType)op2;
				}
				set {
					op2=(u8)value;
				}
			}
			public void ExprSetIrreducible() {
			}
			public bool ExprHasProperty(int P) {
				return (this.flags&P)==P;
			}
			public bool ExprHasAnyProperty(int P) {
				return (this.flags&P)!=0;
			}
			public void ExprSetProperty(int P) {
				this.flags=(ushort)(this.flags|P);
			}
			public void ExprClearProperty(int P) {
				this.flags=(ushort)(this.flags&~P);
			}
		}
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
		///Macros to determine the number of bytes required by a normal Expr
		///struct, an Expr struct with the EP_Reduced flag set in Expr.flags
		///and an Expr struct with the EP_TokenOnly flag set.
		///
		///</summary>
		//#define EXPR_FULLSIZE           sizeof(Expr)           /* Full size */
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
			///
			///<summary>
			///Name of the identifier 
			///</summary>
			public int idx;
		///
		///<summary>
		///Index in some Table.aCol[] of a column named zName 
		///</summary>
		}
		public class IdList {
			public IdList_item[] a;
			public int nId;
			///<summary>
			///Number of identifiers on the list
			///</summary>
			public int nAlloc;
			///
			///<summary>
			///Number of entries allocated for a[] below 
			///</summary>
			public IdList Copy() {
				if(this==null)
					return null;
				else {
					IdList cp=(IdList)MemberwiseClone();
					a.CopyTo(cp.a,0);
					return cp;
				}
			}
		};

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
		private const int BMS=((int)(sizeof(Bitmask)*8));
		
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
			///
			///<summary>
			///The parser 
			///</summary>
			public SrcList pSrcList;
			///
			///<summary>
			///One or more tables used to resolve names 
			///</summary>
			public ExprList pEList;
			///
			///<summary>
			///Optional list of named expressions 
			///</summary>
			public int nRef;
			///
			///<summary>
			///Number of names resolved by this context 
			///</summary>
			public int nErr;
			///
			///<summary>
			///Number of errors encountered while resolving names 
			///</summary>
			public u8 allowAgg;
			///
			///<summary>
			///Aggregate functions allowed here 
			///</summary>
			public u8 hasAgg;
			///
			///<summary>
			///True if aggregates are seen 
			///</summary>
			public u8 isCheck;
			///
			///<summary>
			///True if resolving names in a CHECK constraint 
			///</summary>
			public int nDepth;
			///
			///<summary>
			///Depth of subquery recursion. 1 for no recursion 
			///</summary>
			public AggInfo pAggInfo;
			///
			///<summary>
			///Information about aggregates at this level 
			///</summary>
			public NameContext pNext;
			///
			///<summary>
			///Next outer name context.  NULL for outermost 
			///</summary>
			public///<summary>
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
		///
		///<summary>
		///During code generation of statements that do inserts into AUTOINCREMENT
		///tables, the following information is attached to the Table.u.autoInc.p
		///pointer of each autoincrement table to record some side information that
		///</summary>
		///<param name="the code generator needs.  We have to keep per">table autoincrement</param>
		///<param name="information in case inserts are down within triggers.  Triggers do not">information in case inserts are down within triggers.  Triggers do not</param>
		///<param name="normally coordinate their activities, but we do need to coordinate the">normally coordinate their activities, but we do need to coordinate the</param>
		///<param name="loading and saving of autoincrement information.">loading and saving of autoincrement information.</param>
		///<param name=""></param>
		public class AutoincInfo {
			public AutoincInfo pNext;
			///
			///<summary>
			///Next info block in a list of them all 
			///</summary>
			public Table pTab;
			///
			///<summary>
			///Table this info block refers to 
			///</summary>
			public int iDb;
			///
			///<summary>
			///Index in sqlite3.aDb[] of database holding pTab 
			///</summary>
			public int regCtr;
		///
		///<summary>
		///Memory register holding the rowid counter 
		///</summary>
		};

		///<summary>
		/// Size of the column cache
		///
		///</summary>
		#if !SQLITE_N_COLCACHE
		//# define SQLITE_N_COLCACHE 10
		private const int SQLITE_N_COLCACHE=10;
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
		/// compiled. Function sqlite3TableLock() is used to add entries to the
		/// list.
		///
		///</summary>
		public class yColCache {
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
		private static bool IN_DECLARE_VTAB(Parse pParse) {
			return pParse.declareVtab!=0;
		}
		#endif
		///
		///<summary>
		///An instance of the following structure can be declared on a stack and used
		///to save the Parse.zAuthContext value so that it can be restored later.
		///</summary>
		public class AuthContext {
			public string zAuthContext;
			///
			///<summary>
			///Put saved Parse.zAuthContext here 
			///</summary>
			public Parse pParse;
		///
		///<summary>
		///The Parse structure 
		///</summary>
		};

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
		///<summary>
		/// The following structure contains information used by the sqliteFix...
		/// routines as they walk the parse tree to make database references
		/// explicit.
		///
		///</summary>
		//typedef struct DbFixer DbFixer;
		public class DbFixer {
			public Parse pParse;
			///
			///<summary>
			///The parsing context.  Error messages written here 
			///</summary>
			public string zDb;
			///
			///<summary>
			///Make sure all objects are contained in this database 
			///</summary>
			public string zType;
			///
			///<summary>
			///</summary>
			///<param name="Type of the container "> used for error messages </param>
			public Token pName;
			///
			///<summary>
			///</summary>
			///<param name="Name of the container "> used for error messages </param>
			public///<summary>
			/// Initialize a DbFixer structure.  This routine must be called prior
			/// to passing the structure to one of the sqliteFixAAAA() routines below.
			///
			/// The return value indicates whether or not fixation is required.  TRUE
			/// means we do need to fix the database references, FALSE means we do not.
			///</summary>
			int sqlite3FixInit(///
			///<summary>
			///The fixer to be initialized 
			///</summary>
			Parse pParse,///
			///<summary>
			///Error messages will be written here 
			///</summary>
			int iDb,///
			///<summary>
			///This is the database that must be used 
			///</summary>
			string zType,///
			///<summary>
			///"view", "trigger", or "index" 
			///</summary>
			Token pName///
			///<summary>
			///Name of the view, trigger, or index 
			///</summary>
			) {
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
			int sqlite3FixSrcList(///
			///<summary>
			///Context of the fixation 
			///</summary>
			SrcList pList///
			///<summary>
			///The Source list to check and modify 
			///</summary>
			) {
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
			public int sqlite3FixSelect(///
			///<summary>
			///Context of the fixation 
			///</summary>
			Select pSelect///
			///<summary>
			///The SELECT statement to be fixed to one database 
			///</summary>
			) {
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
			public int sqlite3FixExpr(///
			///<summary>
			///Context of the fixation 
			///</summary>
			Expr pExpr///
			///<summary>
			///The expression to be fixed to one database 
			///</summary>
			) {
				while(pExpr!=null) {
					if(pExpr.ExprHasAnyProperty(EP_TokenOnly))
						break;
					if(pExpr.ExprHasProperty(EP_xIsSelect)) {
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
			public int sqlite3FixExprList(///
			///<summary>
			///Context of the fixation 
			///</summary>
			ExprList pList///
			///<summary>
			///The expression to be fixed to one database 
			///</summary>
			) {
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
			public int sqlite3FixTriggerStep(///
			///<summary>
			///Context of the fixation 
			///</summary>
			TriggerStep pStep///
			///<summary>
			///The trigger step be fixed to one database 
			///</summary>
			) {
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
			///
			///<summary>
			///Optional database for lookaside.  Can be NULL 
			///</summary>
			//public StringBuilder zBase; /* A base allocation.  Not from malloc. */
			public StringBuilder zText;
			///
			///<summary>
			///The string collected so far 
			///</summary>
			//public int nChar;           /* Length of the string so far */
			//public int nAlloc;          /* Amount of space allocated in zText */
			public int mxAlloc;
			///
			///<summary>
			///Maximum allowed string length 
			///</summary>
			// Cannot happen under C#
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
			public void explainAppendTerm(///
			///<summary>
			///The text expression being built 
			///</summary>
			int iTerm,///
			///<summary>
			///Index of this term.  First is zero 
			///</summary>
			string zColumn,///
			///<summary>
			///Name of the column 
			///</summary>
			string zOp///
			///<summary>
			///Name of the operator 
			///</summary>
			) {
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
			///
			///<summary>
			///The database being initialized 
			///</summary>
			public int iDb;
			///
			///<summary>
			///0 for main database.  1 for TEMP, 2.. for ATTACHed 
			///</summary>
			public string pzErrMsg;
			///
			///<summary>
			///Error message stored here 
			///</summary>
			public int rc;
		///
		///<summary>
		///Result code stored here 
		///</summary>
		}
		///<summary>
		/// Structure containing global configuration data for the SQLite library.
		///
		/// This structure also contains some state information.
		///
		///</summary>
		public class Sqlite3Config {
			public bool bMemstat;
			///
			///<summary>
			///True to enable memory status 
			///</summary>
			public bool bCoreMutex;
			///
			///<summary>
			///True to enable core mutexing 
			///</summary>
			public bool bFullMutex;
			///
			///<summary>
			///True to enable full mutexing 
			///</summary>
			public bool bOpenUri;
			///
			///<summary>
			///True to interpret filenames as URIs 
			///</summary>
			public int mxStrlen;
			///
			///<summary>
			///Maximum string length 
			///</summary>
			public int szLookaside;
			///
			///<summary>
			///Default lookaside buffer size 
			///</summary>
			public int nLookaside;
			///
			///<summary>
			///Default lookaside buffer count 
			///</summary>
			public sqlite3_mem_methods m;
			///
			///<summary>
			///</summary>
			///<param name="Low">level memory allocation interface </param>
			public sqlite3_mutex_methods mutex;
			///
			///<summary>
			///</summary>
			///<param name="Low">level mutex interface </param>
			public sqlite3_pcache_methods pcache;
			///
			///<summary>
			///</summary>
			///<param name="Low">cache interface </param>
			public byte[] pHeap;
			///
			///<summary>
			///Heap storage space 
			///</summary>
			public int nHeap;
			///
			///<summary>
			///Size of pHeap[] 
			///</summary>
			public int mnReq,mxReq;
			///
			///<summary>
			///Min and max heap requests sizes 
			///</summary>
			public byte[][] pScratch2;
			///
			///<summary>
			///Scratch memory 
			///</summary>
			public byte[][] pScratch;
			///
			///<summary>
			///Scratch memory 
			///</summary>
			public int szScratch;
			///
			///<summary>
			///Size of each scratch buffer 
			///</summary>
			public int nScratch;
			///
			///<summary>
			///Number of scratch buffers 
			///</summary>
			public MemPage pPage;
			///
			///<summary>
			///Page cache memory 
			///</summary>
			public int szPage;
			///
			///<summary>
			///Size of each page in pPage[] 
			///</summary>
			public int nPage;
			///
			///<summary>
			///Number of pages in pPage[] 
			///</summary>
			public int mxParserStack;
			///
			///<summary>
			///maximum depth of the parser stack 
			///</summary>
			public bool sharedCacheEnabled;
			///
			///<summary>
			///</summary>
			///<param name="true if shared">cache mode enabled </param>
			///
			///<summary>
			///</summary>
			///<param name="The above might be initialized to non">zero.  The following need to always</param>
			///<param name="initially be zero, however. ">initially be zero, however. </param>
			public int isInit;
			///
			///<summary>
			///True after initialization has finished 
			///</summary>
			public int inProgress;
			///
			///<summary>
			///True while initialization in progress 
			///</summary>
			public int isMutexInit;
			///
			///<summary>
			///True after mutexes are initialized 
			///</summary>
			public int isMallocInit;
			///
			///<summary>
			///True after malloc is initialized 
			///</summary>
			public int isPCacheInit;
			///
			///<summary>
			///True after malloc is initialized 
			///</summary>
			public sqlite3_mutex pInitMutex;
			///
			///<summary>
			///Mutex used by sqlite3_initialize() 
			///</summary>
			public int nRefInitMutex;
			///
			///<summary>
			///Number of users of pInitMutex 
			///</summary>
			public dxLog xLog;
			//void (*xLog)(void*,int,const char); /* Function for logging */
			public object pLogArg;
			///
			///<summary>
			///First argument to xLog() 
			///</summary>
			public bool bLocaltimeFault;
			///
			///<summary>
			///True to fail localtime() calls 
			///</summary>
			public Sqlite3Config(int bMemstat,int bCoreMutex,bool bFullMutex,bool bOpenUri,int mxStrlen,int szLookaside,int nLookaside,sqlite3_mem_methods m,sqlite3_mutex_methods mutex,sqlite3_pcache_methods pcache,byte[] pHeap,int nHeap,int mnReq,int mxReq,byte[][] pScratch,int szScratch,int nScratch,MemPage pPage,int szPage,int nPage,int mxParserStack,bool sharedCacheEnabled,int isInit,int inProgress,int isMutexInit,int isMallocInit,int isPCacheInit,sqlite3_mutex pInitMutex,int nRefInitMutex,dxLog xLog,object pLogArg,bool bLocaltimeFault) {
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
		///
		///<summary>
		///Internal function prototypes
		///</summary>
		//int sqlite3StrICmp(string , string );
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
		private const int SQLITE_FAULTINJECTOR_MALLOC=0;
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
	
}
