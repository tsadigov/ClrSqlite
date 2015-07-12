using u8 = System.Byte;
using System.Diagnostics;

namespace Community.CsharpSqlite
{
    //SQLITE_API int sqlite3_vtab_on_conflict(sqlite3 );
    ///
    ///<summary>
    ///CAPI3REF: Conflict resolution modes
    ///
    ///These constants are returned by [sqlite3_vtab_on_conflict()] to
    ///inform a [virtual table] implementation what the [ON CONFLICT] mode
    ///is for the SQL statement being evaluated.
    ///
    ///Note that the [SQLITE_IGNORE] constant is also used as a potential
    ///return value from the [sqlite3_set_authorizer()] callback and that
    ///[SQLITE_ABORT] is also a [result code].
    ///
    ///</summary>
    public enum VTabConflictPolicy
    {
        //#define SQLITE_ROLLBACK 1
        SQLITE_ROLLBACK = 1,

        ///
        ///<summary>
        SQLITE_IGNORE = 2 ,// Also used by sqlite3_authorizer() callback 
        ///</summary>

        //#define SQLITE_FAIL     3
        SQLITE_FAIL = 3,

        ///
        ///<summary>
        SQLITE_ABORT = 4,  // Also an error code 
        ///</summary>

        //#define SQLITE_REPLACE  5
        SQLITE_REPLACE = 5
    }
	public partial class Sqlite3
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
///This header file defines the interface that the SQLite library
///</summary>
///<param name="presents to client programs.  If a C">function, structure, datatype,</param>
///<param name="or constant definition does not appear in this file, then it is">or constant definition does not appear in this file, then it is</param>
///<param name="not a published API of SQLite, is subject to change without">not a published API of SQLite, is subject to change without</param>
///<param name="notice, and should not be referenced by programs that use SQLite.">notice, and should not be referenced by programs that use SQLite.</param>
///<param name=""></param>
///<param name="Some of the definitions that are in this file are marked as">Some of the definitions that are in this file are marked as</param>
///<param name=""experimental".  Experimental interfaces are normally new">"experimental".  Experimental interfaces are normally new</param>
///<param name="features recently added to SQLite.  We do not anticipate changes">features recently added to SQLite.  We do not anticipate changes</param>
///<param name="to experimental interfaces but reserve the right to make minor changes">to experimental interfaces but reserve the right to make minor changes</param>
///<param name="if experience from use "in the wild" suggest such changes are prudent.">if experience from use "in the wild" suggest such changes are prudent.</param>
///<param name=""></param>
///<param name="The official C">language API documentation for SQLite is derived</param>
///<param name="from comments in this file.  This file is the authoritative source">from comments in this file.  This file is the authoritative source</param>
///<param name="on how SQLite interfaces are suppose to operate.">on how SQLite interfaces are suppose to operate.</param>
///<param name=""></param>
///<param name="The name of this file under configuration management is "sqlite.h.in".">The name of this file under configuration management is "sqlite.h.in".</param>
///<param name="The makefile makes some minor changes to this file (such as inserting">The makefile makes some minor changes to this file (such as inserting</param>
///<param name="the version number) and changes its name to "sqlite3.h" as">the version number) and changes its name to "sqlite3.h" as</param>
///<param name="part of the build process.">part of the build process.</param>
///<param name=""></param>
///<param name="Included in SQLite3 port to C#">SQLite;  2008 Noah B Hart</param>
///<param name="C#">SQLite is an independent reimplementation of the SQLite software library</param>
///<param name=""></param>
///<param name="SQLITE_SOURCE_ID: 2011">23 19:49:22 4374b7e83ea0a3fbc3691f9c0c936272862f32f2</param>
///<param name=""></param>
///<param name=""></param>
///<param name=""></param>

		//#if !_SQLITE3_H_
		//#define _SQLITE3_H_
		//#include <stdarg.h>     /* Needed for the definition of va_list */
		///
///<summary>
///Make sure we can call this stuff from C++.
///
///</summary>

		//#if __cplusplus
		//extern "C" {
		//#endif
		///
///<summary>
///Add the ability to override 'extern'
///
///</summary>

		//#if !SQLITE_EXTERN
		//# define SQLITE_EXTERN extern
		//#endif
		//#if !SQLITE_API
		//# define SQLITE_API
		//#endif
		///
///<summary>
///</summary>
///<param name="These no">op macros are used in front of interfaces to mark those</param>
///<param name="interfaces as either deprecated or experimental.  New applications">interfaces as either deprecated or experimental.  New applications</param>
///<param name="should not use deprecated interfaces "> they are support for backwards</param>
///<param name="compatibility only.  Application writers should be aware that">compatibility only.  Application writers should be aware that</param>
///<param name="experimental interfaces are subject to change in point releases.">experimental interfaces are subject to change in point releases.</param>
///<param name=""></param>
///<param name="These macros used to resolve to various kinds of compiler magic that">These macros used to resolve to various kinds of compiler magic that</param>
///<param name="would generate warning messages when they were used.  But that">would generate warning messages when they were used.  But that</param>
///<param name="compiler magic ended up generating such a flurry of bug reports">compiler magic ended up generating such a flurry of bug reports</param>
///<param name="that we have taken it all out and gone back to using simple">that we have taken it all out and gone back to using simple</param>
///<param name="noop macros.">noop macros.</param>
///<param name=""></param>

		//#define SQLITE_DEPRECATED
		//#define SQLITE_EXPERIMENTAL
		///
///<summary>
///Ensure these symbols were not defined by some previous header file.
///
///</summary>

		//#if SQLITE_VERSION
		//# undef SQLITE_VERSION
		//#endif
		//#if SQLITE_VERSION_NUMBER
		//# undef SQLITE_VERSION_NUMBER
		//#endif
		///
///<summary>
///</summary>
///<param name="CAPI3REF: Compile">Time Library Version Numbers</param>
///<param name=""></param>
///<param name="^(The [SQLITE_VERSION] C preprocessor macro in the sqlite3.h header">^(The [SQLITE_VERSION] C preprocessor macro in the sqlite3.h header</param>
///<param name="evaluates to a string literal that is the SQLite version in the">evaluates to a string literal that is the SQLite version in the</param>
///<param name="format "X.Y.Z" where X is the major version number (always 3 for">format "X.Y.Z" where X is the major version number (always 3 for</param>
///<param name="SQLite3) and Y is the minor version number and Z is the release number.)^">SQLite3) and Y is the minor version number and Z is the release number.)^</param>
///<param name="^(The [SQLITE_VERSION_NUMBER] C preprocessor macro resolves to an integer">^(The [SQLITE_VERSION_NUMBER] C preprocessor macro resolves to an integer</param>
///<param name="with the value (X*1000000 + Y*1000 + Z) where X, Y, and Z are the same">with the value (X*1000000 + Y*1000 + Z) where X, Y, and Z are the same</param>
///<param name="numbers used in [SQLITE_VERSION].)^">numbers used in [SQLITE_VERSION].)^</param>
///<param name="The SQLITE_VERSION_NUMBER for any given release of SQLite will also">The SQLITE_VERSION_NUMBER for any given release of SQLite will also</param>
///<param name="be larger than the release from which it is derived.  Either Y will">be larger than the release from which it is derived.  Either Y will</param>
///<param name="be held constant and Z will be incremented or else Y will be incremented">be held constant and Z will be incremented or else Y will be incremented</param>
///<param name="and Z will be reset to zero.">and Z will be reset to zero.</param>
///<param name=""></param>
///<param name="Since version 3.6.18, SQLite source code has been stored in the">Since version 3.6.18, SQLite source code has been stored in the</param>
///<param name="<a href="http://www.fossil">scm.org/">Fossil configuration management</param>
///<param name="system</a>.  ^The SQLITE_SOURCE_ID macro evaluates to">system</a>.  ^The SQLITE_SOURCE_ID macro evaluates to</param>
///<param name="a string which identifies a particular check">in of SQLite</param>
///<param name="within its configuration management system.  ^The SQLITE_SOURCE_ID">within its configuration management system.  ^The SQLITE_SOURCE_ID</param>
///<param name="string contains the date and time of the check">in (UTC) and an SHA1</param>
///<param name="hash of the entire source tree.">hash of the entire source tree.</param>
///<param name=""></param>
///<param name="See also: [sqlite3_libversion()],">See also: [sqlite3_libversion()],</param>
///<param name="[sqlite3_libversion_number()], [sqlite3_sourceid()],">[sqlite3_libversion_number()], [sqlite3_sourceid()],</param>
///<param name="[sqlite_version()] and [sqlite_source_id()].">[sqlite_version()] and [sqlite_source_id()].</param>
///<param name=""></param>

		//#define SQLITE_VERSION        "3.7.7"
		//#define SQLITE_VERSION_NUMBER 3007007
		//#define SQLITE_SOURCE_ID      "2011-06-23 19:49:22 4374b7e83ea0a3fbc3691f9c0c936272862f32f2"
		public const string SQLITE_VERSION = "3.7.7(C#)";

		public const int SQLITE_VERSION_NUMBER = 300700701;

		public const string SQLITE_SOURCE_ID = "Ported to C# from 2011-06-23 19:49:22 4374b7e83ea0a3fbc3691f9c0c936272862f32f2";

        ///
        ///<summary>
        ///</summary>
        ///<param name="CAPI3REF: Run">Time Library Version Numbers</param>
        ///<param name="KEYWORDS: sqlite3_version, sqlite3_sourceid">KEYWORDS: sqlite3_version, sqlite3_sourceid</param>
        ///<param name=""></param>
        ///<param name="These interfaces provide the same information as the [SQLITE_VERSION],">These interfaces provide the same information as the [SQLITE_VERSION],</param>
        ///<param name="[SQLITE_VERSION_NUMBER], and [SQLITE_SOURCE_ID] C preprocessor macros">[SQLITE_VERSION_NUMBER], and [SQLITE_SOURCE_ID] C preprocessor macros</param>
        ///<param name="but are associated with the library instead of the header file.  ^(Cautious">but are associated with the library instead of the header file.  ^(Cautious</param>
        ///<param name="programmers might include Debug.Assert() statements in their application to">programmers might include Debug.Assert() statements in their application to</param>
        ///<param name="verify that values returned by these interfaces match the macros in">verify that values returned by these interfaces match the macros in</param>
        ///<param name="the header, and thus insure that the application is">the header, and thus insure that the application is</param>
        ///<param name="compiled with matching library and header files.">compiled with matching library and header files.</param>
        ///<param name=""></param>
        ///<param name="<blockquote><pre>"><blockquote><pre></param>
        ///<param name="Debug.Assert( sqlite3_libversion_number()==SQLITE_VERSION_NUMBER );">Debug.Assert( sqlite3_libversion_number()==SQLITE_VERSION_NUMBER );</param>
        ///<param name="Debug.Assert( strcmp(sqlite3_sourceid(),SQLITE_SOURCE_ID)==0 );">Debug.Assert( strcmp(sqlite3_sourceid(),SQLITE_SOURCE_ID)==0 );</param>
        ///<param name="Debug.Assert( strcmp(sqlite3_libversion(),SQLITE_VERSION)==0 );">Debug.Assert( strcmp(sqlite3_libversion(),SQLITE_VERSION)==0 );</param>
        ///<param name="</pre></blockquote>)^"></pre></blockquote>)^</param>
        ///<param name=""></param>
        ///<param name="^The sqlite3_version[] string constant contains the text of [SQLITE_VERSION]">^The sqlite3_version[] string constant contains the text of [SQLITE_VERSION]</param>
        ///<param name="macro.  ^The sqlite3_libversion() function returns a pointer to the">macro.  ^The sqlite3_libversion() function returns a pointer to the</param>
        ///<param name="to the sqlite3_version[] string constant.  The sqlite3_libversion()">to the sqlite3_version[] string constant.  The sqlite3_libversion()</param>
        ///<param name="function is provided for use in DLLs since DLL users usually do not have">function is provided for use in DLLs since DLL users usually do not have</param>
        ///<param name="direct access to string constants within the DLL.  ^The">direct access to string constants within the DLL.  ^The</param>
        ///<param name="sqlite3_libversion_number() function returns an integer equal to">sqlite3_libversion_number() function returns an integer equal to</param>
        ///<param name="[SQLITE_VERSION_NUMBER].  ^The sqlite3_sourceid() function returns ">[SQLITE_VERSION_NUMBER].  ^The sqlite3_sourceid() function returns </param>
        ///<param name="a pointer to a string constant whose value is the same as the ">a pointer to a string constant whose value is the same as the </param>
        ///<param name="[SQLITE_SOURCE_ID] C preprocessor macro.">[SQLITE_SOURCE_ID] C preprocessor macro.</param>
        ///<param name=""></param>
        ///<param name="See also: [sqlite_version()] and [sqlite_source_id()].">See also: [sqlite_version()] and [sqlite_source_id()].</param>
        ///<param name=""></param>

        //SQLITE_API SQLITE_EXTERN const char sqlite3_version[];
        //SQLITE_API string sqlite3_libversion(void);
        //SQLITE_API string sqlite3_sourceid(void);
        //SQLITE_API int sqlite3_libversion_number(void);
        ///
        ///<summary>
        ///</summary>
        ///<param name="CAPI3REF: Run">Time Library Compilation Options Diagnostics</param>
        ///<param name=""></param>
        ///<param name="^The sqlite3_compileoption_used() function returns 0 or 1 ">^The sqlite3_compileoption_used() function returns 0 or 1 </param>
        ///<param name="indicating whether the specified option was defined at ">indicating whether the specified option was defined at </param>
        ///<param name="compile time.  ^The SQLITE_ prefix may be omitted from the ">compile time.  ^The SQLITE_ prefix may be omitted from the </param>
        ///<param name="option name passed to sqlite3_compileoption_used().  ">option name passed to sqlite3_compileoption_used().  </param>
        ///<param name=""></param>
        ///<param name="^The sqlite3_compileoption_get() function allows iterating">^The sqlite3_compileoption_get() function allows iterating</param>
        ///<param name="over the list of options that were defined at compile time by">over the list of options that were defined at compile time by</param>
        ///<param name="returning the N">th compile time option string.  ^If N is out of range,</param>
        ///<param name="sqlite3_compileoption_get() returns a NULL pointer.  ^The SQLITE_ ">sqlite3_compileoption_get() returns a NULL pointer.  ^The SQLITE_ </param>
        ///<param name="prefix is omitted from any strings returned by ">prefix is omitted from any strings returned by </param>
        ///<param name="sqlite3_compileoption_get().">sqlite3_compileoption_get().</param>
        ///<param name=""></param>
        ///<param name="^Support for the diagnostic functions sqlite3_compileoption_used()">^Support for the diagnostic functions sqlite3_compileoption_used()</param>
        ///<param name="and sqlite3_compileoption_get() may be omitted by specifying the ">and sqlite3_compileoption_get() may be omitted by specifying the </param>
        ///<param name="[SQLITE_OMIT_COMPILEOPTION_DIAGS] option at compile time.">[SQLITE_OMIT_COMPILEOPTION_DIAGS] option at compile time.</param>
        ///<param name=""></param>
        ///<param name="See also: SQL functions [sqlite_compileoption_used()] and">See also: SQL functions [sqlite_compileoption_used()] and</param>
        ///<param name="[sqlite_compileoption_get()] and the [compile_options pragma].">[sqlite_compileoption_get()] and the [compile_options pragma].</param>
        ///<param name=""></param>

        //#if !SQLITE_OMIT_COMPILEOPTION_DIAGS
        //SQLITE_API int sqlite3_compileoption_used(string zOptName);
        //SQLITE_API string sqlite3_compileoption_get(int N);
        //#endif
        ///
        ///<summary>
        ///CAPI3REF: Test To See If The Library Is Threadsafe
        ///
        ///^The sqlite3_threadsafe() function returns zero if and only if
        ///SQLite was compiled mutexing code omitted due to the
        ///</summary>
        ///<param name="[SQLITE_THREADSAFE] compile">time option being set to 0.</param>
        ///<param name=""></param>
        ///<param name="SQLite can be compiled with or without mutexes.  When">SQLite can be compiled with or without mutexes.  When</param>
        ///<param name="the [SQLITE_THREADSAFE] C preprocessor macro is 1 or 2, mutexes">the [SQLITE_THREADSAFE] C preprocessor macro is 1 or 2, mutexes</param>
        ///<param name="are enabled and SQLite is threadsafe.  When the">are enabled and SQLite is threadsafe.  When the</param>
        ///<param name="[SQLITE_THREADSAFE] macro is 0, ">[SQLITE_THREADSAFE] macro is 0, </param>
        ///<param name="the mutexes are omitted.  Without the mutexes, it is not safe">the mutexes are omitted.  Without the mutexes, it is not safe</param>
        ///<param name="to use SQLite concurrently from more than one thread.">to use SQLite concurrently from more than one thread.</param>
        ///<param name=""></param>
        ///<param name="Enabling mutexes incurs a measurable performance penalty.">Enabling mutexes incurs a measurable performance penalty.</param>
        ///<param name="So if speed is of utmost importance, it makes sense to disable">So if speed is of utmost importance, it makes sense to disable</param>
        ///<param name="the mutexes.  But for maximum safety, mutexes should be enabled.">the mutexes.  But for maximum safety, mutexes should be enabled.</param>
        ///<param name="^The default behavior is for mutexes to be enabled.">^The default behavior is for mutexes to be enabled.</param>
        ///<param name=""></param>
        ///<param name="This interface can be used by an application to make sure that the">This interface can be used by an application to make sure that the</param>
        ///<param name="version of SQLite that it is linking against was compiled with">version of SQLite that it is linking against was compiled with</param>
        ///<param name="the desired setting of the [SQLITE_THREADSAFE] macro.">the desired setting of the [SQLITE_THREADSAFE] macro.</param>
        ///<param name=""></param>
        ///<param name="This interface only reports on the compile">time mutex setting</param>
        ///<param name="of the [SQLITE_THREADSAFE] flag.  If SQLite is compiled with">of the [SQLITE_THREADSAFE] flag.  If SQLite is compiled with</param>
        ///<param name="SQLITE_THREADSAFE=1 or =2 then mutexes are enabled by default but">SQLITE_THREADSAFE=1 or =2 then mutexes are enabled by default but</param>
        ///<param name="can be fully or partially disabled using a call to [sqlite3_config()]">can be fully or partially disabled using a call to [sqlite3_config()]</param>
        ///<param name="with the verbs [SQLITE_CONFIG_SINGLETHREAD], [SQLITE_CONFIG_MULTITHREAD],">with the verbs [SQLITE_CONFIG_SINGLETHREAD], [SQLITE_CONFIG_MULTITHREAD],</param>
        ///<param name="or [SQLITE_CONFIG_MUTEX].  ^(The return value of the">or [SQLITE_CONFIG_MUTEX].  ^(The return value of the</param>
        ///<param name="sqlite3_threadsafe() function shows only the compile">time setting of</param>
        ///<param name="thread safety, not any run">time changes to that setting made by</param>
        ///<param name="sqlite3_config(). In other words, the return value from sqlite3_threadsafe()">sqlite3_config(). In other words, the return value from sqlite3_threadsafe()</param>
        ///<param name="is unchanged by calls to sqlite3_config().)^">is unchanged by calls to sqlite3_config().)^</param>
        ///<param name=""></param>
        ///<param name="See the [threading mode] documentation for additional information.">See the [threading mode] documentation for additional information.</param>
        ///<param name=""></param>

        //SQLITE_API int sqlite3_threadsafe(void);
        ///
        ///<summary>
        ///CAPI3REF: Database Connection Handle
        ///KEYWORDS: {database connection} {database connections}
        ///
        ///Each open SQLite database is represented by a pointer to an instance of
        ///the opaque structure named "sqlite3".  It is useful to think of an sqlite3
        ///pointer as an object.  The [sqlite3_open()], [sqlite3_open16()], and
        ///[sqlite3_open_v2()] interfaces are its constructors, and [sqlite3_close()]
        ///is its destructor.  There are many other interfaces (such as
        ///[sqlite3_prepare_v2()], [sqlite3_create_function()], and
        ///[sqlite3_busy_timeout()] to name but three) that are methods on an
        ///sqlite3 object.
        ///
        ///</summary>

        //typedef struct sqlite3 sqlite3;
        ///
        ///<summary>
        ///</summary>
        ///<param name="CAPI3REF: 64">Bit Integer Types</param>
        ///<param name="KEYWORDS: sqlite_int64 sqlite_uint64">KEYWORDS: sqlite_int64 sqlite_uint64</param>
        ///<param name=""></param>
        ///<param name="Because there is no cross">bit integer types</param>
        ///<param name="SQLite includes typedefs for 64">bit signed and unsigned integers.</param>
        ///<param name=""></param>
        ///<param name="The sqlite3_int64 and sqlite3_uint64 are the preferred type definitions.">The sqlite3_int64 and sqlite3_uint64 are the preferred type definitions.</param>
        ///<param name="The sqlite_int64 and sqlite_uint64 types are supported for backwards">The sqlite_int64 and sqlite_uint64 types are supported for backwards</param>
        ///<param name="compatibility only.">compatibility only.</param>
        ///<param name=""></param>
        ///<param name="^The sqlite3_int64 and sqlite_int64 types can store integer values">^The sqlite3_int64 and sqlite_int64 types can store integer values</param>
        ///<param name="between ">9223372036854775808 and +9223372036854775807 inclusive.  ^The</param>
        ///<param name="sqlite3_uint64 and sqlite_uint64 types can store integer values ">sqlite3_uint64 and sqlite_uint64 types can store integer values </param>
        ///<param name="between 0 and +18446744073709551615 inclusive.">between 0 and +18446744073709551615 inclusive.</param>
        ///<param name=""></param>

        //#if SQLITE_INT64_TYPE
        //  typedef SQLITE_INT64_TYPE sqlite_int64;
        //  typedef unsigned SQLITE_INT64_TYPE sqlite_uint64;
        //#elif defined(_MSC_VER) || defined(__BORLANDC__)
        //  typedef __int64 sqlite_int64;
        //  typedef unsigned __int64 sqlite_uint64;
        //#else
        //  typedef long long int sqlite_int64;
        //  typedef unsigned long long int sqlite_uint64;
        //#endif
        //typedef sqlite_int64 sqlite3_int64;
        //typedef sqlite_uint64 sqlite3_uint64;
        ///
        ///<summary>
        ///If compiling for a processor that lacks floating point support,
        ///</summary>
        ///<param name="substitute integer for floating">point.</param>
        ///<param name=""></param>

        //#if SQLITE_OMIT_FLOATING_POINT
        //# define double sqlite3_int64
        //#endif
        ///
        ///<summary>
        ///CAPI3REF: Closing A Database Connection
        ///
        ///^The sqlite3_close() routine is the destructor for the [sqlite3] object.
        ///^Calls to sqlite3_close() return SqlResult.SQLITE_OK if the [sqlite3] object is
        ///successfully destroyed and all associated resources are deallocated.
        ///
        ///Applications must [sqlite3_finalize | finalize] all [prepared statements]
        ///and [sqlite3_blob_close | close] all [BLOB handles] associated with
        ///the [sqlite3] object prior to attempting to close the object.  ^If
        ///sqlite3_close() is called on a [database connection] that still has
        ///outstanding [prepared statements] or [BLOB handles], then it returns
        ///SQLITE_BUSY.
        ///
        ///^If [sqlite3_close()] is invoked while a transaction is open,
        ///the transaction is automatically rolled back.
        ///
        ///The C parameter to [sqlite3_close(C)] must be either a NULL
        ///pointer or an [sqlite3] object pointer obtained
        ///from [sqlite3_open()], [sqlite3_open16()], or
        ///[sqlite3_open_v2()], and not previously closed.
        ///^Calling sqlite3_close() with a NULL pointer argument is a 
        ///</summary>
        ///<param name="harmless no">op.</param>
        ///<param name=""></param>

        //SQLITE_API int sqlite3_close(sqlite3 );
        ///
        ///<summary>
        ///The type for a callback function.
        ///This is legacy and deprecated.  It is included for historical
        ///compatibility and is not documented.
        ///
        ///</summary>

        //typedef int (*sqlite3_callback)(void*,int,char**, char*);

        ///

        ///
        ///<summary>
        ///CAPI3REF: Flags For File Open Operations
        ///
        ///These bit values are intended for use in the
        ///3rd parameter to the [sqlite3_open_v2()] interface and
        ///in the 4th parameter to the [sqlite3_vfs.xOpen] method.
        ///
        ///</summary>

        //#define SQLITE_OPEN_READONLY         0x00000001  /* Ok for sqlite3_open_v2() */
        //#define SQLITE_OPEN_READWRITE        0x00000002  /* Ok for sqlite3_open_v2() */
        //#define SQLITE_OPEN_CREATE           0x00000004  /* Ok for sqlite3_open_v2() */
        //#define SQLITE_OPEN_DELETEONCLOSE    0x00000008  /* VFS only */
        //#define SQLITE_OPEN_EXCLUSIVE        0x00000010  /* VFS only */
        //#define SQLITE_OPEN_AUTOPROXY        0x00000020  /* VFS only */
        //#define SQLITE_OPEN_URI              0x00000040  /* Ok for sqlite3_open_v2() */
        //#define SQLITE_OPEN_MAIN_DB          0x00000100  /* VFS only */
        //#define SQLITE_OPEN_TEMP_DB          0x00000200  /* VFS only */
        //#define SQLITE_OPEN_TRANSIENT_DB     0x00000400  /* VFS only */
        //#define SQLITE_OPEN_MAIN_JOURNAL     0x00000800  /* VFS only */
        //#define SQLITE_OPEN_TEMP_JOURNAL     0x00001000  /* VFS only */
        //#define SQLITE_OPEN_SUBJOURNAL       0x00002000  /* VFS only */
        //#define SQLITE_OPEN_MASTER_JOURNAL   0x00004000  /* VFS only */
        //#define SQLITE_OPEN_NOMUTEX          0x00008000  /* Ok for sqlite3_open_v2() */
        //#define SQLITE_OPEN_FULLMUTEX        0x00010000  /* Ok for sqlite3_open_v2() */
        //#define SQLITE_OPEN_SHAREDCACHE      0x00020000  /* Ok for sqlite3_open_v2() */
        //#define SQLITE_OPEN_PRIVATECACHE     0x00040000  /* Ok for sqlite3_open_v2() */
        //#define SQLITE_OPEN_WAL              0x00080000  /* VFS only */
        ///
        ///<summary>
        ///Reserved:                           0x00F00000 
        ///</summary>
        //TODO:public enum
        public const int SQLITE_OPEN_READONLY = 0x00000001;

		public const int SQLITE_OPEN_READWRITE = 0x00000002;

		public const int SQLITE_OPEN_CREATE = 0x00000004;

		public const int SQLITE_OPEN_DELETEONCLOSE = 0x00000008;

		public const int SQLITE_OPEN_EXCLUSIVE = 0x00000010;

		public const int SQLITE_OPEN_AUTOPROXY = 0x00000020;

		public const int SQLITE_OPEN_URI = 0x00000040;

		public const int SQLITE_OPEN_MAIN_DB = 0x00000100;

		public const int SQLITE_OPEN_TEMP_DB = 0x00000200;

		public const int SQLITE_OPEN_TRANSIENT_DB = 0x00000400;

		public const int SQLITE_OPEN_MAIN_JOURNAL = 0x00000800;

		public const int SQLITE_OPEN_TEMP_JOURNAL = 0x00001000;

		public const int SQLITE_OPEN_SUBJOURNAL = 0x00002000;

		public const int SQLITE_OPEN_MASTER_JOURNAL = 0x00004000;

		public const int SQLITE_OPEN_NOMUTEX = 0x00008000;

		public const int SQLITE_OPEN_FULLMUTEX = 0x00010000;

		public const int SQLITE_OPEN_SHAREDCACHE = 0x00020000;

		public const int SQLITE_OPEN_PRIVATECACHE = 0x00040000;

		public const int SQLITE_OPEN_WAL = 0x00080000;

		///
///<summary>
///CAPI3REF: Device Characteristics
///
///The xDeviceCharacteristics method of the [sqlite3_io_methods]
///object returns an integer which is a vector of the these
///bit values expressing I/O characteristics of the mass storage
///device that holds the file that the [sqlite3_io_methods]
///refers to.
///
///The SQLITE_IOCAP_ATOMIC property means that all writes of
///any size are atomic.  The SQLITE_IOCAP_ATOMICnnn values
///mean that writes of blocks that are nnn bytes in size and
///are aligned to an address which is an integer multiple of
///nnn are atomic.  The SQLITE_IOCAP_SAFE_APPEND value means
///that when data is appended to a file, the data is appended
///first then the size of the file is extended, never the other
///way around.  The SQLITE_IOCAP_SEQUENTIAL property means that
///information is written to disk in the same order as calls
///to xWrite().
///
///</summary>

		//#define SQLITE_IOCAP_ATOMIC                 0x00000001
		//#define SQLITE_IOCAP_ATOMIC512              0x00000002
		//#define SQLITE_IOCAP_ATOMIC1K               0x00000004
		//#define SQLITE_IOCAP_ATOMIC2K               0x00000008
		//#define SQLITE_IOCAP_ATOMIC4K               0x00000010
		//#define SQLITE_IOCAP_ATOMIC8K               0x00000020
		//#define SQLITE_IOCAP_ATOMIC16K              0x00000040
		//#define SQLITE_IOCAP_ATOMIC32K              0x00000080
		//#define SQLITE_IOCAP_ATOMIC64K              0x00000100
		//#define SQLITE_IOCAP_SAFE_APPEND            0x00000200
		//#define SQLITE_IOCAP_SEQUENTIAL             0x00000400
		//#define SQLITE_IOCAP_UNDELETABLE_WHEN_OPEN  0x00000800
		public const int SQLITE_IOCAP_ATOMIC = 0x00000001;

		public const int SQLITE_IOCAP_ATOMIC512 = 0x00000002;

		public const int SQLITE_IOCAP_ATOMIC1K = 0x00000004;

		public const int SQLITE_IOCAP_ATOMIC2K = 0x00000008;

		public const int SQLITE_IOCAP_ATOMIC4K = 0x00000010;

		public const int SQLITE_IOCAP_ATOMIC8K = 0x00000020;

		public const int SQLITE_IOCAP_ATOMIC16K = 0x00000040;

		public const int SQLITE_IOCAP_ATOMIC32K = 0x00000080;

		public const int SQLITE_IOCAP_ATOMIC64K = 0x00000100;

		public const int SQLITE_IOCAP_SAFE_APPEND = 0x00000200;

		public const int SQLITE_IOCAP_SEQUENTIAL = 0x00000400;

		public const int SQLITE_IOCAP_UNDELETABLE_WHEN_OPEN = 0x00000800;

		///
///<summary>
///CAPI3REF: File Locking Levels
///
///SQLite uses one of these integer values as the second
///argument to calls it makes to the xLock() and xUnlock() methods
///of an [sqlite3_io_methods] object.
///
///</summary>

		//#define SQLITE_LOCK_NONE          0
		//#define SQLITE_LOCK_SHARED        1
		//#define SQLITE_LOCK_RESERVED      2
		//#define SQLITE_LOCK_PENDING       3
		//#define SQLITE_LOCK_EXCLUSIVE     4
		public const int SQLITE_LOCK_NONE = 0;

		public const int SQLITE_LOCK_SHARED = 1;

		public const int SQLITE_LOCK_RESERVED = 2;

		public const int SQLITE_LOCK_PENDING = 3;

		public const int SQLITE_LOCK_EXCLUSIVE = 4;

		///
///<summary>
///CAPI3REF: Synchronization Type Flags
///
///When SQLite invokes the xSync() method of an
///[sqlite3_io_methods] object it uses a combination of
///these integer values as the second argument.
///
///When the SQLITE_SYNC_DATAONLY flag is used, it means that the
///sync operation only needs to flush data to mass storage.  Inode
///information need not be flushed. If the lower four bits of the flag
///equal SQLITE_SYNC_NORMAL, that means to use normal fsync() semantics.
///If the lower four bits equal SQLITE_SYNC_FULL, that means
///to use Mac OS X style fullsync instead of fsync().
///
///Do not confuse the SQLITE_SYNC_NORMAL and SQLITE_SYNC_FULL flags
///with the [PRAGMA synchronous]=NORMAL and [PRAGMA synchronous]=FULL
///settings.  The [synchronous pragma] determines when calls to the
///xSync VFS method occur and applies uniformly across all platforms.
///The SQLITE_SYNC_NORMAL and SQLITE_SYNC_FULL flags determine how
///energetic or rigorous or forceful the sync operations are and
///only make a difference on Mac OSX for the default SQLite code.
///</summary>
///<param name="(Third">party VFS implementations might also make the distinction</param>
///<param name="between SQLITE_SYNC_NORMAL and SQLITE_SYNC_FULL, but among the">between SQLITE_SYNC_NORMAL and SQLITE_SYNC_FULL, but among the</param>
///<param name="operating systems natively supported by SQLite, only Mac OSX">operating systems natively supported by SQLite, only Mac OSX</param>
///<param name="cares about the difference.)">cares about the difference.)</param>
///<param name=""></param>

		//#define SQLITE_SYNC_NORMAL        0x00002
		//#define SQLITE_SYNC_FULL          0x00003
		//#define SQLITE_SYNC_DATAONLY      0x00010
		public const int SQLITE_SYNC_NORMAL = 0x00002;

		public const int SQLITE_SYNC_FULL = 0x00003;

		public const int SQLITE_SYNC_DATAONLY = 0x00010;

		

	
		///
///<summary>
///CAPI3REF: Mutex Handle
///
///The mutex module within SQLite defines [sqlite3_mutex] to be an
///abstract type for a mutex object.  The SQLite core never looks
///at the internal representation of an [sqlite3_mutex].  It only
///deals with pointers to the [sqlite3_mutex] object.
///
///Mutexes are created using [sqlite3_mutex_alloc()].
///
///</summary>

		
		///
///<summary>
///CAPI3REF: Maximum xShmLock index
///
///The xShmLock method on [sqlite3_io_methods] may use values
///between 0 and this upper bound as its "offset" argument.
///The SQLite core will never attempt to acquire or release a
///lock outside of this range
///
///</summary>

		//#define SQLITE_SHM_NLOCK        8
		public const int SQLITE_SHM_NLOCK = 8;

		///
///<summary>
///CAPI3REF: Initialize The SQLite Library
///
///^The sqlite3_initialize() routine initializes the
///SQLite library.  ^The sqlite3_shutdown() routine
///deallocates any resources that were allocated by sqlite3_initialize().
///These routines are designed to aid in process initialization and
///shutdown on embedded systems.  Workstation applications using
///SQLite normally do not need to invoke either of these routines.
///
///A call to sqlite3_initialize() is an "effective" call if it is
///the first time sqlite3_initialize() is invoked during the lifetime of
///the process, or if it is the first time sqlite3_initialize() is invoked
///following a call to sqlite3_shutdown().  ^(Only an effective call
///of sqlite3_initialize() does any initialization.  All other calls
///</summary>
///<param name="are harmless no">ops.)^</param>
///<param name=""></param>
///<param name="A call to sqlite3_shutdown() is an "effective" call if it is the first">A call to sqlite3_shutdown() is an "effective" call if it is the first</param>
///<param name="call to sqlite3_shutdown() since the last sqlite3_initialize().  ^(Only">call to sqlite3_shutdown() since the last sqlite3_initialize().  ^(Only</param>
///<param name="an effective call to sqlite3_shutdown() does any deinitialization.">an effective call to sqlite3_shutdown() does any deinitialization.</param>
///<param name="All other valid calls to sqlite3_shutdown() are harmless no">ops.)^</param>
///<param name=""></param>
///<param name="The sqlite3_initialize() interface is threadsafe, but sqlite3_shutdown()">The sqlite3_initialize() interface is threadsafe, but sqlite3_shutdown()</param>
///<param name="is not.  The sqlite3_shutdown() interface must only be called from a">is not.  The sqlite3_shutdown() interface must only be called from a</param>
///<param name="single thread.  All open [database connections] must be closed and all">single thread.  All open [database connections] must be closed and all</param>
///<param name="other SQLite resources must be deallocated prior to invoking">other SQLite resources must be deallocated prior to invoking</param>
///<param name="sqlite3_shutdown().">sqlite3_shutdown().</param>
///<param name=""></param>
///<param name="Among other things, ^sqlite3_initialize() will invoke">Among other things, ^sqlite3_initialize() will invoke</param>
///<param name="sqlite3_os_init().  Similarly, ^sqlite3_shutdown()">sqlite3_os_init().  Similarly, ^sqlite3_shutdown()</param>
///<param name="will invoke sqlite3_os_end().">will invoke sqlite3_os_end().</param>
///<param name=""></param>
///<param name="^The sqlite3_initialize() routine returns [SqlResult.SQLITE_OK] on success.">^The sqlite3_initialize() routine returns [SqlResult.SQLITE_OK] on success.</param>
///<param name="^If for some reason, sqlite3_initialize() is unable to initialize">^If for some reason, sqlite3_initialize() is unable to initialize</param>
///<param name="the library (perhaps it is unable to allocate a needed resource such">the library (perhaps it is unable to allocate a needed resource such</param>
///<param name="as a mutex) it returns an [error code] other than [SqlResult.SQLITE_OK].">as a mutex) it returns an [error code] other than [SqlResult.SQLITE_OK].</param>
///<param name=""></param>
///<param name="^The sqlite3_initialize() routine is called internally by many other">^The sqlite3_initialize() routine is called internally by many other</param>
///<param name="SQLite interfaces so that an application usually does not need to">SQLite interfaces so that an application usually does not need to</param>
///<param name="invoke sqlite3_initialize() directly.  For example, [sqlite3_open()]">invoke sqlite3_initialize() directly.  For example, [sqlite3_open()]</param>
///<param name="calls sqlite3_initialize() so the SQLite library will be automatically">calls sqlite3_initialize() so the SQLite library will be automatically</param>
///<param name="initialized when [sqlite3_open()] is called if it has not be initialized">initialized when [sqlite3_open()] is called if it has not be initialized</param>
///<param name="already.  ^However, if SQLite is compiled with the [SQLITE_OMIT_AUTOINIT]">already.  ^However, if SQLite is compiled with the [SQLITE_OMIT_AUTOINIT]</param>
///<param name="compile">time option, then the automatic calls to sqlite3_initialize()</param>
///<param name="are omitted and the application must call sqlite3_initialize() directly">are omitted and the application must call sqlite3_initialize() directly</param>
///<param name="prior to using any other SQLite interface.  For maximum portability,">prior to using any other SQLite interface.  For maximum portability,</param>
///<param name="it is recommended that applications always invoke sqlite3_initialize()">it is recommended that applications always invoke sqlite3_initialize()</param>
///<param name="directly prior to using any other SQLite interface.  Future releases">directly prior to using any other SQLite interface.  Future releases</param>
///<param name="of SQLite may require this.  In other words, the behavior exhibited">of SQLite may require this.  In other words, the behavior exhibited</param>
///<param name="when SQLite is compiled with [SQLITE_OMIT_AUTOINIT] might become the">when SQLite is compiled with [SQLITE_OMIT_AUTOINIT] might become the</param>
///<param name="default behavior in some future release of SQLite.">default behavior in some future release of SQLite.</param>
///<param name=""></param>
///<param name="The sqlite3_os_init() routine does operating">system specific</param>
///<param name="initialization of the SQLite library.  The sqlite3_os_end()">initialization of the SQLite library.  The sqlite3_os_end()</param>
///<param name="routine undoes the effect of sqlite3_os_init().  Typical tasks">routine undoes the effect of sqlite3_os_init().  Typical tasks</param>
///<param name="performed by these routines include allocation or deallocation">performed by these routines include allocation or deallocation</param>
///<param name="of static resources, initialization of global variables,">of static resources, initialization of global variables,</param>
///<param name="setting up a default [sqlite3_vfs] module, or setting up">setting up a default [sqlite3_vfs] module, or setting up</param>
///<param name="a default configuration using [sqlite3_config()].">a default configuration using [sqlite3_config()].</param>
///<param name=""></param>
///<param name="The application should never invoke either sqlite3_os_init()">The application should never invoke either sqlite3_os_init()</param>
///<param name="or sqlite3_os_end() directly.  The application should only invoke">or sqlite3_os_end() directly.  The application should only invoke</param>
///<param name="sqlite3_initialize() and sqlite3_shutdown().  The sqlite3_os_init()">sqlite3_initialize() and sqlite3_shutdown().  The sqlite3_os_init()</param>
///<param name="interface is called automatically by sqlite3_initialize() and">interface is called automatically by sqlite3_initialize() and</param>
///<param name="sqlite3_os_end() is called by sqlite3_shutdown().  Appropriate">sqlite3_os_end() is called by sqlite3_shutdown().  Appropriate</param>
///<param name="implementations for sqlite3_os_init() and sqlite3_os_end()">implementations for sqlite3_os_init() and sqlite3_os_end()</param>
///<param name="are built into SQLite when it is compiled for Unix, Windows, or OS/2.">are built into SQLite when it is compiled for Unix, Windows, or OS/2.</param>
///<param name="When [custom builds | built for other platforms]">When [custom builds | built for other platforms]</param>
///<param name="(using the [SQLITE_OS_OTHER=1] compile">time</param>
///<param name="option) the application must supply a suitable implementation for">option) the application must supply a suitable implementation for</param>
///<param name="sqlite3_os_init() and sqlite3_os_end().  An application">supplied</param>
///<param name="implementation of sqlite3_os_init() or sqlite3_os_end()">implementation of sqlite3_os_init() or sqlite3_os_end()</param>
///<param name="must return [SqlResult.SQLITE_OK] on success and some other [error code] upon">must return [SqlResult.SQLITE_OK] on success and some other [error code] upon</param>
///<param name="failure.">failure.</param>
///<param name=""></param>

		//SQLITE_API int sqlite3_initialize(void);
		//SQLITE_API int sqlite3_shutdown(void);
		//SQLITE_API int sqlite3_os_init(void);
		//SQLITE_API int sqlite3_os_end(void);
		///
///<summary>
///CAPI3REF: Configuring The SQLite Library
///
///The sqlite3_config() interface is used to make global configuration
///changes to SQLite in order to tune SQLite to the specific needs of
///the application.  The default configuration is recommended for most
///applications and so this routine is usually not necessary.  It is
///provided to support rare applications with unusual needs.
///
///The sqlite3_config() interface is not threadsafe.  The application
///must insure that no other SQLite interfaces are invoked by other
///threads while sqlite3_config() is running.  Furthermore, sqlite3_config()
///may only be invoked prior to library initialization using
///[sqlite3_initialize()] or after shutdown by [sqlite3_shutdown()].
///^If sqlite3_config() is called after [sqlite3_initialize()] and before
///[sqlite3_shutdown()] then it will return SQLITE_MISUSE.
///Note, however, that ^sqlite3_config() can be called as part of the
///</summary>
///<param name="implementation of an application">defined [sqlite3_os_init()].</param>
///<param name=""></param>
///<param name="The first argument to sqlite3_config() is an integer">The first argument to sqlite3_config() is an integer</param>
///<param name="[configuration option] that determines">[configuration option] that determines</param>
///<param name="what property of SQLite is to be configured.  Subsequent arguments">what property of SQLite is to be configured.  Subsequent arguments</param>
///<param name="vary depending on the [configuration option]">vary depending on the [configuration option]</param>
///<param name="in the first argument.">in the first argument.</param>
///<param name=""></param>
///<param name="^When a configuration option is set, sqlite3_config() returns [SqlResult.SQLITE_OK].">^When a configuration option is set, sqlite3_config() returns [SqlResult.SQLITE_OK].</param>
///<param name="^If the option is unknown or SQLite is unable to set the option">^If the option is unknown or SQLite is unable to set the option</param>
///<param name="then this routine returns a non">zero [error code].</param>
///<param name=""></param>

		//SQLITE_API int sqlite3_config(int, ...);
		///
///<summary>
///CAPI3REF: Configure database connections
///
///The sqlite3_db_config() interface is used to make configuration
///changes to a [database connection].  The interface is similar to
///[sqlite3_config()] except that the changes apply to a single
///[database connection] (specified in the first argument).
///
///The second argument to sqlite3_db_config(D,V,...)  is the
///</summary>
///<param name="[SQLITE_DBCONFIG_LOOKASIDE | configuration verb] "> an integer code </param>
///<param name="that indicates what aspect of the [database connection] is being configured.">that indicates what aspect of the [database connection] is being configured.</param>
///<param name="Subsequent arguments vary depending on the configuration verb.">Subsequent arguments vary depending on the configuration verb.</param>
///<param name=""></param>
///<param name="^Calls to sqlite3_db_config() return SqlResult.SQLITE_OK if and only if">^Calls to sqlite3_db_config() return SqlResult.SQLITE_OK if and only if</param>
///<param name="the call is considered successful.">the call is considered successful.</param>
///<param name=""></param>

		//SQLITE_API int sqlite3_db_config(sqlite3*, int op, ...);
		///<summary>
		/// CAPI3REF: Memory Allocation Routines
		///
		/// An instance of this object defines the interface between SQLite
		/// and low-level memory allocation routines.
		///
		/// This object is used in only one place in the SQLite interface.
		/// A pointer to an instance of this object is the argument to
		/// [sqlite3_config()] when the configuration option is
		/// [SQLITE_CONFIG_MALLOC] or [SQLITE_CONFIG_GETMALLOC].
		/// By creating an instance of this object
		/// and passing it to [sqlite3_config]([SQLITE_CONFIG_MALLOC])
		/// during configuration, an application can specify an alternative
		/// memory allocation subsystem for SQLite to use for all of its
		/// dynamic memory needs.
		///
		/// Note that SQLite comes with several [built-in memory allocators]
		/// that are perfectly adequate for the overwhelming majority of applications
		/// and that this object is only useful to a tiny minority of applications
		/// with specialized memory allocation requirements.  This object is
		/// also used during testing of SQLite in order to specify an alternative
		/// memory allocator that simulates memory out-of-memory conditions in
		/// order to verify that SQLite recovers gracefully from such
		/// conditions.
		///
		/// The xMalloc and xFree methods must work like the
		/// malloc() and free() functions from the standard C library.
		/// The xRealloc method must work like realloc() from the standard C library
		/// with the exception that if the second argument to xRealloc is zero,
		/// xRealloc must be a no-op - it must not perform any allocation or
		/// deallocation.  ^SQLite guarantees that the second argument to
		/// xRealloc is always a value returned by a prior call to xRoundup.
		/// And so in cases where xRoundup always returns a positive number,
		/// xRealloc can perform exactly as the standard library realloc() and
		/// still be in compliance with this specification.
		///
		/// xSize should return the allocated size of a memory allocation
		/// previously obtained from xMalloc or xRealloc.  The allocated size
		/// is always at least as big as the requested size but may be larger.
		///
		/// The xRoundup method returns what would be the allocated size of
		/// a memory allocation given a particular requested size.  Most memory
		/// allocators round up memory allocations at least to the next multiple
		/// of 8.  Some allocators round up to a larger multiple or to a power of 2.
		/// Every memory allocation request coming in through [sqlite3_malloc()]
		/// or [sqlite3_realloc()] first calls xRoundup.  If xRoundup returns 0,
		/// that causes the corresponding memory allocation to fail.
		///
		/// The xInit method initializes the memory allocator.  (For example,
		/// it might allocate any require mutexes or initialize internal data
		/// structures.  The xShutdown method is invoked (indirectly) by
		/// [sqlite3_shutdown()] and should deallocate any resources acquired
		/// by xInit.  The pAppData pointer is used as the only parameter to
		/// xInit and xShutdown.
		///
		/// SQLite holds the [SQLITE_MUTEX_STATIC_MASTER] mutex when it invokes
		/// the xInit method, so the xInit method need not be threadsafe.  The
		/// xShutdown method is only called from [sqlite3_shutdown()] so it does
		/// not need to be threadsafe either.  For all other methods, SQLite
		/// holds the [SQLITE_MUTEX_STATIC_MEM] mutex as long as the
		/// [SQLITE_CONFIG_MEMSTATUS] configuration option is turned on (which
		/// it is by default) and so the methods are automatically serialized.
		/// However, if [SQLITE_CONFIG_MEMSTATUS] is disabled, then the other
		/// methods must be threadsafe or else make their own arrangements for
		/// serialization.
		///
		/// SQLite will never invoke xInit() more than once without an intervening
		/// call to xShutdown().
		///
		///</summary>
		

		///
///<summary>
///CAPI3REF: Database Connection Configuration Options
///
///These constants are the available integer configuration options that
///can be passed as the second argument to the [sqlite3_db_config()] interface.
///
///New configuration options may be added in future releases of SQLite.
///Existing configuration options might be discontinued.  Applications
///should check the return code from [sqlite3_db_config()] to make sure that
///the call worked.  ^The [sqlite3_db_config()] interface will return a
///</summary>
///<param name="non">zero [error code] if a discontinued or unsupported configuration option</param>
///<param name="is invoked.">is invoked.</param>
///<param name=""></param>
///<param name="<dl>"><dl></param>
///<param name="<dt>SQLITE_DBCONFIG_LOOKASIDE</dt>"><dt>SQLITE_DBCONFIG_LOOKASIDE</dt></param>
///<param name="<dd> ^This option takes three additional arguments that determine the "><dd> ^This option takes three additional arguments that determine the </param>
///<param name="[lookaside memory allocator] configuration for the [database connection].">[lookaside memory allocator] configuration for the [database connection].</param>
///<param name="^The first argument (the third parameter to [sqlite3_db_config()] is a">^The first argument (the third parameter to [sqlite3_db_config()] is a</param>
///<param name="pointer to a memory buffer to use for lookaside memory.">pointer to a memory buffer to use for lookaside memory.</param>
///<param name="^The first argument after the SQLITE_DBCONFIG_LOOKASIDE verb">^The first argument after the SQLITE_DBCONFIG_LOOKASIDE verb</param>
///<param name="may be NULL in which case SQLite will allocate the">may be NULL in which case SQLite will allocate the</param>
///<param name="lookaside buffer itself using [sqlite3_malloc()]. ^The second argument is the">lookaside buffer itself using [sqlite3_malloc()]. ^The second argument is the</param>
///<param name="size of each lookaside buffer slot.  ^The third argument is the number of">size of each lookaside buffer slot.  ^The third argument is the number of</param>
///<param name="slots.  The size of the buffer in the first argument must be greater than">slots.  The size of the buffer in the first argument must be greater than</param>
///<param name="or equal to the product of the second and third arguments.  The buffer">or equal to the product of the second and third arguments.  The buffer</param>
///<param name="must be aligned to an 8">byte boundary.  ^If the second argument to</param>
///<param name="SQLITE_DBCONFIG_LOOKASIDE is not a multiple of 8, it is internally">SQLITE_DBCONFIG_LOOKASIDE is not a multiple of 8, it is internally</param>
///<param name="rounded down to the next smaller multiple of 8.  ^(The lookaside memory">rounded down to the next smaller multiple of 8.  ^(The lookaside memory</param>
///<param name="configuration for a database connection can only be changed when that">configuration for a database connection can only be changed when that</param>
///<param name="connection is not currently using lookaside memory, or in other words">connection is not currently using lookaside memory, or in other words</param>
///<param name="when the "current value" returned by">when the "current value" returned by</param>
///<param name="[sqlite3_db_status](D,[SQLITE_CONFIG_LOOKASIDE],...) is zero.">[sqlite3_db_status](D,[SQLITE_CONFIG_LOOKASIDE],...) is zero.</param>
///<param name="Any attempt to change the lookaside memory configuration when lookaside">Any attempt to change the lookaside memory configuration when lookaside</param>
///<param name="memory is in use leaves the configuration unchanged and returns ">memory is in use leaves the configuration unchanged and returns </param>
///<param name="[SQLITE_BUSY].)^</dd>">[SQLITE_BUSY].)^</dd></param>
///<param name=""></param>
///<param name="<dt>SQLITE_DBCONFIG_ENABLE_FKEY</dt>"><dt>SQLITE_DBCONFIG_ENABLE_FKEY</dt></param>
///<param name="<dd> ^This option is used to enable or disable the enforcement of"><dd> ^This option is used to enable or disable the enforcement of</param>
///<param name="[foreign key constraints].  There should be two additional arguments.">[foreign key constraints].  There should be two additional arguments.</param>
///<param name="The first argument is an integer which is 0 to disable FK enforcement,">The first argument is an integer which is 0 to disable FK enforcement,</param>
///<param name="positive to enable FK enforcement or negative to leave FK enforcement">positive to enable FK enforcement or negative to leave FK enforcement</param>
///<param name="unchanged.  The second parameter is a pointer to an integer into which">unchanged.  The second parameter is a pointer to an integer into which</param>
///<param name="is written 0 or 1 to indicate whether FK enforcement is off or on">is written 0 or 1 to indicate whether FK enforcement is off or on</param>
///<param name="following this call.  The second parameter may be a NULL pointer, in">following this call.  The second parameter may be a NULL pointer, in</param>
///<param name="which case the FK enforcement setting is not reported back. </dd>">which case the FK enforcement setting is not reported back. </dd></param>
///<param name=""></param>
///<param name="<dt>SQLITE_DBCONFIG_ENABLE_TRIGGER</dt>"><dt>SQLITE_DBCONFIG_ENABLE_TRIGGER</dt></param>
///<param name="<dd> ^This option is used to enable or disable [CREATE TRIGGER | triggers]."><dd> ^This option is used to enable or disable [CREATE TRIGGER | triggers].</param>
///<param name="There should be two additional arguments.">There should be two additional arguments.</param>
///<param name="The first argument is an integer which is 0 to disable triggers,">The first argument is an integer which is 0 to disable triggers,</param>
///<param name="positive to enable triggers or negative to leave the setting unchanged.">positive to enable triggers or negative to leave the setting unchanged.</param>
///<param name="The second parameter is a pointer to an integer into which">The second parameter is a pointer to an integer into which</param>
///<param name="is written 0 or 1 to indicate whether triggers are disabled or enabled">is written 0 or 1 to indicate whether triggers are disabled or enabled</param>
///<param name="following this call.  The second parameter may be a NULL pointer, in">following this call.  The second parameter may be a NULL pointer, in</param>
///<param name="which case the trigger setting is not reported back. </dd>">which case the trigger setting is not reported back. </dd></param>
///<param name=""></param>
///<param name="</dl>"></dl></param>
///<param name=""></param>

		//#define SQLITE_DBCONFIG_LOOKASIDE       1001  /* void* int int */
		//#define SQLITE_DBCONFIG_ENABLE_FKEY     1002  /* int int* */
		//#define SQLITE_DBCONFIG_ENABLE_TRIGGER  1003  /* int int* */
		public const int SQLITE_DBCONFIG_LOOKASIDE = 1001;

		public const int SQLITE_DBCONFIG_ENABLE_FKEY = 1002;

		public const int SQLITE_DBCONFIG_ENABLE_TRIGGER = 1003;

		///
///<summary>
///CAPI3REF: Enable Or Disable Extended Result Codes
///
///^The sqlite3_extended_result_codes() routine enables or disables the
///[extended result codes] feature of SQLite. ^The extended result
///codes are disabled by default for historical compatibility.
///
///</summary>

		//SQLITE_API int sqlite3_extended_result_codes(sqlite3*, int onoff);
		///
///<summary>
///CAPI3REF: Last Insert Rowid
///
///</summary>
///<param name="^Each entry in an SQLite table has a unique 64">bit signed</param>
///<param name="integer key called the [ROWID | "rowid"]. ^The rowid is always available">integer key called the [ROWID | "rowid"]. ^The rowid is always available</param>
///<param name="as an undeclared column named ROWID, OID, or _ROWID_ as long as those">as an undeclared column named ROWID, OID, or _ROWID_ as long as those</param>
///<param name="names are not also used by explicitly declared columns. ^If">names are not also used by explicitly declared columns. ^If</param>
///<param name="the table has a column of type [INTEGER PRIMARY KEY] then that column">the table has a column of type [INTEGER PRIMARY KEY] then that column</param>
///<param name="is another alias for the rowid.">is another alias for the rowid.</param>
///<param name=""></param>
///<param name="^This routine returns the [rowid] of the most recent">^This routine returns the [rowid] of the most recent</param>
///<param name="successful [INSERT] into the database from the [database connection]">successful [INSERT] into the database from the [database connection]</param>
///<param name="in the first argument.  ^As of SQLite version 3.7.7, this routines">in the first argument.  ^As of SQLite version 3.7.7, this routines</param>
///<param name="records the last insert rowid of both ordinary tables and [virtual tables].">records the last insert rowid of both ordinary tables and [virtual tables].</param>
///<param name="^If no successful [INSERT]s">^If no successful [INSERT]s</param>
///<param name="have ever occurred on that database connection, zero is returned.">have ever occurred on that database connection, zero is returned.</param>
///<param name=""></param>
///<param name="^(If an [INSERT] occurs within a trigger or within a [virtual table]">^(If an [INSERT] occurs within a trigger or within a [virtual table]</param>
///<param name="method, then this routine will return the [rowid] of the inserted">method, then this routine will return the [rowid] of the inserted</param>
///<param name="row as long as the trigger or virtual table method is running.">row as long as the trigger or virtual table method is running.</param>
///<param name="But once the trigger or virtual table method ends, the value returned ">But once the trigger or virtual table method ends, the value returned </param>
///<param name="by this routine reverts to what it was before the trigger or virtual">by this routine reverts to what it was before the trigger or virtual</param>
///<param name="table method began.)^">table method began.)^</param>
///<param name=""></param>
///<param name="^An [INSERT] that fails due to a constraint violation is not a">^An [INSERT] that fails due to a constraint violation is not a</param>
///<param name="successful [INSERT] and does not change the value returned by this">successful [INSERT] and does not change the value returned by this</param>
///<param name="routine.  ^Thus INSERT OR FAIL, INSERT OR IGNORE, INSERT OR ROLLBACK,">routine.  ^Thus INSERT OR FAIL, INSERT OR IGNORE, INSERT OR ROLLBACK,</param>
///<param name="and INSERT OR ABORT make no changes to the return value of this">and INSERT OR ABORT make no changes to the return value of this</param>
///<param name="routine when their insertion fails.  ^(When INSERT OR REPLACE">routine when their insertion fails.  ^(When INSERT OR REPLACE</param>
///<param name="encounters a constraint violation, it does not fail.  The">encounters a constraint violation, it does not fail.  The</param>
///<param name="INSERT continues to completion after deleting rows that caused">INSERT continues to completion after deleting rows that caused</param>
///<param name="the constraint problem so INSERT OR REPLACE will always change">the constraint problem so INSERT OR REPLACE will always change</param>
///<param name="the return value of this interface.)^">the return value of this interface.)^</param>
///<param name=""></param>
///<param name="^For the purposes of this routine, an [INSERT] is considered to">^For the purposes of this routine, an [INSERT] is considered to</param>
///<param name="be successful even if it is subsequently rolled back.">be successful even if it is subsequently rolled back.</param>
///<param name=""></param>
///<param name="This function is accessible to SQL statements via the">This function is accessible to SQL statements via the</param>
///<param name="[last_insert_rowid() SQL function].">[last_insert_rowid() SQL function].</param>
///<param name=""></param>
///<param name="If a separate thread performs a new [INSERT] on the same">If a separate thread performs a new [INSERT] on the same</param>
///<param name="database connection while the [sqlite3_last_insert_rowid()]">database connection while the [sqlite3_last_insert_rowid()]</param>
///<param name="function is running and thus changes the last insert [rowid],">function is running and thus changes the last insert [rowid],</param>
///<param name="then the value returned by [sqlite3_last_insert_rowid()] is">then the value returned by [sqlite3_last_insert_rowid()] is</param>
///<param name="unpredictable and might not equal either the old or the new">unpredictable and might not equal either the old or the new</param>
///<param name="last insert [rowid].">last insert [rowid].</param>
///<param name=""></param>

		//SQLITE_API sqlite3_int64 sqlite3_last_insert_rowid(sqlite3);
		///
///<summary>
///CAPI3REF: Count The Number Of Rows Modified
///
///^This function returns the number of database rows that were changed
///or inserted or deleted by the most recently completed SQL statement
///on the [database connection] specified by the first parameter.
///^(Only changes that are directly specified by the [INSERT], [UPDATE],
///or [DELETE] statement are counted.  Auxiliary changes caused by
///triggers or [foreign key actions] are not counted.)^ Use the
///[sqlite3_total_changes()] function to find the total number of changes
///including changes caused by triggers and foreign key actions.
///
///^Changes to a view that are simulated by an [INSTEAD OF trigger]
///are not counted.  Only real table changes are counted.
///
///^(A "row change" is a change to a single row of a single table
///caused by an INSERT, DELETE, or UPDATE statement.  Rows that
///are changed as side effects of [REPLACE] constraint resolution,
///rollback, ABORT processing, [DROP TABLE], or by any other
///mechanisms do not count as direct row changes.)^
///
///A "trigger context" is a scope of execution that begins and
///ends with the script of a [CREATE TRIGGER | trigger]. 
///Most SQL statements are
///evaluated outside of any trigger.  This is the "top level"
///trigger context.  If a trigger fires from the top level, a
///new trigger context is entered for the duration of that one
///trigger.  Subtriggers create subcontexts for their duration.
///
///^Calling [sqlite3_exec()] or [sqlite3_step()] recursively does
///not create a new trigger context.
///
///^This function returns the number of direct row changes in the
///most recent INSERT, UPDATE, or DELETE statement within the same
///trigger context.
///
///^Thus, when called from the top level, this function returns the
///number of changes in the most recent INSERT, UPDATE, or DELETE
///that also occurred at the top level.  ^(Within the body of a trigger,
///the sqlite3_changes() interface can be called to find the number of
///changes in the most recently completed INSERT, UPDATE, or DELETE
///statement within the body of the same trigger.
///However, the number returned does not include changes
///caused by subtriggers since those have their own context.)^
///
///See also the [sqlite3_total_changes()] interface, the
///[count_changes pragma], and the [changes() SQL function].
///
///If a separate thread makes changes on the same database connection
///while [sqlite3_changes()] is running then the value returned
///is unpredictable and not meaningful.
///
///</summary>

		//SQLITE_API int sqlite3_changes(sqlite3);
		///
///<summary>
///CAPI3REF: Total Number Of Rows Modified
///
///^This function returns the number of row changes caused by [INSERT],
///[UPDATE] or [DELETE] statements since the [database connection] was opened.
///^(The count returned by sqlite3_total_changes() includes all changes
///from all [CREATE TRIGGER | trigger] contexts and changes made by
///[foreign key actions]. However,
///the count does not include changes used to implement [REPLACE] constraints,
///do rollbacks or ABORT processing, or [DROP TABLE] processing.  The
///count does not include rows of views that fire an [INSTEAD OF trigger],
///though if the INSTEAD OF trigger makes changes of its own, those changes 
///are counted.)^
///^The sqlite3_total_changes() function counts the changes as soon as
///the statement that makes them is completed (when the statement handle
///is passed to [sqlite3_reset()] or [sqlite3_finalize()]).
///
///See also the [sqlite3_changes()] interface, the
///[count_changes pragma], and the [total_changes() SQL function].
///
///If a separate thread makes changes on the same database connection
///while [sqlite3_total_changes()] is running then the value
///returned is unpredictable and not meaningful.
///
///</summary>

		//SQLITE_API int sqlite3_total_changes(sqlite3);
		///
///<summary>
///</summary>
///<param name="CAPI3REF: Interrupt A Long">Running Query</param>
///<param name=""></param>
///<param name="^This function causes any pending database operation to abort and">^This function causes any pending database operation to abort and</param>
///<param name="return at its earliest opportunity. This routine is typically">return at its earliest opportunity. This routine is typically</param>
///<param name="called in response to a user action such as pressing "Cancel"">called in response to a user action such as pressing "Cancel"</param>
///<param name="or Ctrl">C where the user wants a long query operation to halt</param>
///<param name="immediately.">immediately.</param>
///<param name=""></param>
///<param name="^It is safe to call this routine from a thread different from the">^It is safe to call this routine from a thread different from the</param>
///<param name="thread that is currently running the database operation.  But it">thread that is currently running the database operation.  But it</param>
///<param name="is not safe to call this routine with a [database connection] that">is not safe to call this routine with a [database connection] that</param>
///<param name="is closed or might close before sqlite3_interrupt() returns.">is closed or might close before sqlite3_interrupt() returns.</param>
///<param name=""></param>
///<param name="^If an SQL operation is very nearly finished at the time when">^If an SQL operation is very nearly finished at the time when</param>
///<param name="sqlite3_interrupt() is called, then it might not have an opportunity">sqlite3_interrupt() is called, then it might not have an opportunity</param>
///<param name="to be interrupted and might continue to completion.">to be interrupted and might continue to completion.</param>
///<param name=""></param>
///<param name="^An SQL operation that is interrupted will return [SQLITE_INTERRUPT].">^An SQL operation that is interrupted will return [SQLITE_INTERRUPT].</param>
///<param name="^If the interrupted SQL operation is an INSERT, UPDATE, or DELETE">^If the interrupted SQL operation is an INSERT, UPDATE, or DELETE</param>
///<param name="that is inside an explicit transaction, then the entire transaction">that is inside an explicit transaction, then the entire transaction</param>
///<param name="will be rolled back automatically.">will be rolled back automatically.</param>
///<param name=""></param>
///<param name="^The sqlite3_interrupt(D) call is in effect until all currently running">^The sqlite3_interrupt(D) call is in effect until all currently running</param>
///<param name="SQL statements on [database connection] D complete.  ^Any new SQL statements">SQL statements on [database connection] D complete.  ^Any new SQL statements</param>
///<param name="that are started after the sqlite3_interrupt() call and before the ">that are started after the sqlite3_interrupt() call and before the </param>
///<param name="running statements reaches zero are interrupted as if they had been">running statements reaches zero are interrupted as if they had been</param>
///<param name="running prior to the sqlite3_interrupt() call.  ^New SQL statements">running prior to the sqlite3_interrupt() call.  ^New SQL statements</param>
///<param name="that are started after the running statement count reaches zero are">that are started after the running statement count reaches zero are</param>
///<param name="not effected by the sqlite3_interrupt().">not effected by the sqlite3_interrupt().</param>
///<param name="^A call to sqlite3_interrupt(D) that occurs when there are no running">^A call to sqlite3_interrupt(D) that occurs when there are no running</param>
///<param name="SQL statements is a no">op and has no effect on SQL statements</param>
///<param name="that are started after the sqlite3_interrupt() call returns.">that are started after the sqlite3_interrupt() call returns.</param>
///<param name=""></param>
///<param name="If the database connection closes while [sqlite3_interrupt()]">If the database connection closes while [sqlite3_interrupt()]</param>
///<param name="is running then bad things will likely happen.">is running then bad things will likely happen.</param>
///<param name=""></param>

		//SQLITE_API void sqlite3_interrupt(sqlite3);
		///
///<summary>
///CAPI3REF: Determine If An SQL Statement Is Complete
///
///</summary>
///<param name="These routines are useful during command">line input to determine if the</param>
///<param name="currently entered text seems to form a complete SQL statement or">currently entered text seems to form a complete SQL statement or</param>
///<param name="if additional input is needed before sending the text into">if additional input is needed before sending the text into</param>
///<param name="SQLite for parsing.  ^These routines return 1 if the input string">SQLite for parsing.  ^These routines return 1 if the input string</param>
///<param name="appears to be a complete SQL statement.  ^A statement is judged to be">appears to be a complete SQL statement.  ^A statement is judged to be</param>
///<param name="complete if it ends with a semicolon token and is not a prefix of a">complete if it ends with a semicolon token and is not a prefix of a</param>
///<param name="well">formed CREATE TRIGGER statement.  ^Semicolons that are embedded within</param>
///<param name="string literals or quoted identifier names or comments are not">string literals or quoted identifier names or comments are not</param>
///<param name="independent tokens (they are part of the token in which they are">independent tokens (they are part of the token in which they are</param>
///<param name="embedded) and thus do not count as a statement terminator.  ^Whitespace">embedded) and thus do not count as a statement terminator.  ^Whitespace</param>
///<param name="and comments that follow the final semicolon are ignored.">and comments that follow the final semicolon are ignored.</param>
///<param name=""></param>
///<param name="^These routines return 0 if the statement is incomplete.  ^If a">^These routines return 0 if the statement is incomplete.  ^If a</param>
///<param name="memory allocation fails, then SQLITE_NOMEM is returned.">memory allocation fails, then SQLITE_NOMEM is returned.</param>
///<param name=""></param>
///<param name="^These routines do not parse the SQL statements thus">^These routines do not parse the SQL statements thus</param>
///<param name="will not detect syntactically incorrect SQL.">will not detect syntactically incorrect SQL.</param>
///<param name=""></param>
///<param name="^(If SQLite has not been initialized using [sqlite3_initialize()] prior ">^(If SQLite has not been initialized using [sqlite3_initialize()] prior </param>
///<param name="to invoking sqlite3_complete16() then sqlite3_initialize() is invoked">to invoking sqlite3_complete16() then sqlite3_initialize() is invoked</param>
///<param name="automatically by sqlite3_complete16().  If that initialization fails,">automatically by sqlite3_complete16().  If that initialization fails,</param>
///<param name="then the return value from sqlite3_complete16() will be non">zero</param>
///<param name="regardless of whether or not the input SQL is complete.)^">regardless of whether or not the input SQL is complete.)^</param>
///<param name=""></param>
///<param name="The input to [sqlite3_complete()] must be a zero">terminated</param>
///<param name="UTF">8 string.</param>
///<param name=""></param>
///<param name="The input to [sqlite3_complete16()] must be a zero">terminated</param>
///<param name="UTF">16 string in native byte order.</param>
///<param name=""></param>

		//SQLITE_API int sqlite3_complete(string sql);
		//SQLITE_API int sqlite3_complete16(const void *sql);
		///
///<summary>
///CAPI3REF: Register A Callback To Handle SQLITE_BUSY Errors
///
///^This routine sets a callback function that might be invoked whenever
///an attempt is made to open a database table that another thread
///or process has locked.
///
///^If the busy callback is NULL, then [SQLITE_BUSY] or [SQLITE_IOERR_BLOCKED]
///is returned immediately upon encountering the lock.  ^If the busy callback
///is not NULL, then the callback might be invoked with two arguments.
///
///^The first argument to the busy handler is a copy of the void* pointer which
///is the third argument to sqlite3_busy_handler().  ^The second argument to
///the busy handler callback is the number of times that the busy handler has
///been invoked for this locking event.  ^If the
///busy callback returns 0, then no additional attempts are made to
///access the database and [SQLITE_BUSY] or [SQLITE_IOERR_BLOCKED] is returned.
///</summary>
///<param name="^If the callback returns non">zero, then another attempt</param>
///<param name="is made to open the database for reading and the cycle repeats.">is made to open the database for reading and the cycle repeats.</param>
///<param name=""></param>
///<param name="The presence of a busy handler does not guarantee that it will be invoked">The presence of a busy handler does not guarantee that it will be invoked</param>
///<param name="when there is lock contention. ^If SQLite determines that invoking the busy">when there is lock contention. ^If SQLite determines that invoking the busy</param>
///<param name="handler could result in a deadlock, it will go ahead and return [SQLITE_BUSY]">handler could result in a deadlock, it will go ahead and return [SQLITE_BUSY]</param>
///<param name="or [SQLITE_IOERR_BLOCKED] instead of invoking the busy handler.">or [SQLITE_IOERR_BLOCKED] instead of invoking the busy handler.</param>
///<param name="Consider a scenario where one process is holding a read lock that">Consider a scenario where one process is holding a read lock that</param>
///<param name="it is trying to promote to a reserved lock and">it is trying to promote to a reserved lock and</param>
///<param name="a second process is holding a reserved lock that it is trying">a second process is holding a reserved lock that it is trying</param>
///<param name="to promote to an exclusive lock.  The first process cannot proceed">to promote to an exclusive lock.  The first process cannot proceed</param>
///<param name="because it is blocked by the second and the second process cannot">because it is blocked by the second and the second process cannot</param>
///<param name="proceed because it is blocked by the first.  If both processes">proceed because it is blocked by the first.  If both processes</param>
///<param name="invoke the busy handlers, neither will make any progress.  Therefore,">invoke the busy handlers, neither will make any progress.  Therefore,</param>
///<param name="SQLite returns [SQLITE_BUSY] for the first process, hoping that this">SQLite returns [SQLITE_BUSY] for the first process, hoping that this</param>
///<param name="will induce the first process to release its read lock and allow">will induce the first process to release its read lock and allow</param>
///<param name="the second process to proceed.">the second process to proceed.</param>
///<param name=""></param>
///<param name="^The default busy callback is NULL.">^The default busy callback is NULL.</param>
///<param name=""></param>
///<param name="^The [SQLITE_BUSY] error is converted to [SQLITE_IOERR_BLOCKED]">^The [SQLITE_BUSY] error is converted to [SQLITE_IOERR_BLOCKED]</param>
///<param name="when SQLite is in the middle of a large transaction where all the">when SQLite is in the middle of a large transaction where all the</param>
///<param name="changes will not fit into the in">memory cache.  SQLite will</param>
///<param name="already hold a RESERVED lock on the database file, but it needs">already hold a RESERVED lock on the database file, but it needs</param>
///<param name="to promote this lock to EXCLUSIVE so that it can spill cache">to promote this lock to EXCLUSIVE so that it can spill cache</param>
///<param name="pages into the database file without harm to concurrent">pages into the database file without harm to concurrent</param>
///<param name="readers.  ^If it is unable to promote the lock, then the in">memory</param>
///<param name="cache will be left in an inconsistent state and so the error">cache will be left in an inconsistent state and so the error</param>
///<param name="code is promoted from the relatively benign [SQLITE_BUSY] to">code is promoted from the relatively benign [SQLITE_BUSY] to</param>
///<param name="the more severe [SQLITE_IOERR_BLOCKED].  ^This error code promotion">the more severe [SQLITE_IOERR_BLOCKED].  ^This error code promotion</param>
///<param name="forces an automatic rollback of the changes.  See the">forces an automatic rollback of the changes.  See the</param>
///<param name="<a href="/cvstrac/wiki?p=CorruptionFollowingBusyError">"><a href="/cvstrac/wiki?p=CorruptionFollowingBusyError"></param>
///<param name="CorruptionFollowingBusyError</a> wiki page for a discussion of why">CorruptionFollowingBusyError</a> wiki page for a discussion of why</param>
///<param name="this is important.">this is important.</param>
///<param name=""></param>
///<param name="^(There can only be a single busy handler defined for each">^(There can only be a single busy handler defined for each</param>
///<param name="[database connection].  Setting a new busy handler clears any">[database connection].  Setting a new busy handler clears any</param>
///<param name="previously set handler.)^  ^Note that calling [sqlite3_busy_timeout()]">previously set handler.)^  ^Note that calling [sqlite3_busy_timeout()]</param>
///<param name="will also set or clear the busy handler.">will also set or clear the busy handler.</param>
///<param name=""></param>
///<param name="The busy callback should not take any actions which modify the">The busy callback should not take any actions which modify the</param>
///<param name="database connection that invoked the busy handler.  Any such actions">database connection that invoked the busy handler.  Any such actions</param>
///<param name="result in undefined behavior.">result in undefined behavior.</param>
///<param name=""></param>
///<param name="A busy handler must not close the database connection">A busy handler must not close the database connection</param>
///<param name="or [prepared statement] that invoked the busy handler.">or [prepared statement] that invoked the busy handler.</param>
///<param name=""></param>

		//SQLITE_API int sqlite3_busy_handler(sqlite3*, int()(void*,int), void);
		///
///<summary>
///CAPI3REF: Set A Busy Timeout
///
///^This routine sets a [sqlite3_busy_handler | busy handler] that sleeps
///for a specified amount of time when a table is locked.  ^The handler
///will sleep multiple times until at least "ms" milliseconds of sleeping
///have accumulated.  ^After at least "ms" milliseconds of sleeping,
///the handler returns 0 which causes [sqlite3_step()] to return
///[SQLITE_BUSY] or [SQLITE_IOERR_BLOCKED].
///
///^Calling this routine with an argument less than or equal to zero
///turns off all busy handlers.
///
///^(There can only be a single busy handler for a particular
///[database connection] any any given moment.  If another busy handler
///was defined  (using [sqlite3_busy_handler()]) prior to calling
///this routine, that other busy handler is cleared.)^
///
///</summary>

		//SQLITE_API int sqlite3_busy_timeout(sqlite3*, int ms);
		///
///<summary>
///CAPI3REF: Convenience Routines For Running Queries
///
///This is a legacy interface that is preserved for backwards compatibility.
///Use of this interface is not recommended.
///
///Definition: A <b>result table</b> is memory data structure created by the
///[sqlite3_get_table()] interface.  A result table records the
///complete query results from one or more queries.
///
///The table conceptually has a number of rows and columns.  But
///these numbers are not part of the result table itself.  These
///numbers are obtained separately.  Let N be the number of rows
///and M be the number of columns.
///
///</summary>
///<param name="A result table is an array of pointers to zero">8 strings.</param>
///<param name="There are (N+1)*M elements in the array.  The first M pointers point">There are (N+1)*M elements in the array.  The first M pointers point</param>
///<param name="to zero">terminated strings that  contain the names of the columns.</param>
///<param name="The remaining entries all point to query results.  NULL values result">The remaining entries all point to query results.  NULL values result</param>
///<param name="in NULL pointers.  All other values are in their UTF">terminated</param>
///<param name="string representation as returned by [sqlite3_column_text()].">string representation as returned by [sqlite3_column_text()].</param>
///<param name=""></param>
///<param name="A result table might consist of one or more memory allocations.">A result table might consist of one or more memory allocations.</param>
///<param name="It is not safe to pass a result table directly to [malloc_cs.sqlite3_free()].">It is not safe to pass a result table directly to [malloc_cs.sqlite3_free()].</param>
///<param name="A result table should be deallocated using [malloc_cs.sqlite3_free_table()].">A result table should be deallocated using [malloc_cs.sqlite3_free_table()].</param>
///<param name=""></param>
///<param name="^(As an example of the result table format, suppose a query result">^(As an example of the result table format, suppose a query result</param>
///<param name="is as follows:">is as follows:</param>
///<param name=""></param>
///<param name="<blockquote><pre>"><blockquote><pre></param>
///<param name="Name        | Age">Name        | Age</param>
///<param name=""></param>
///<param name="Alice       | 43">Alice       | 43</param>
///<param name="Bob         | 28">Bob         | 28</param>
///<param name="Cindy       | 21">Cindy       | 21</param>
///<param name="</pre></blockquote>"></pre></blockquote></param>
///<param name=""></param>
///<param name="There are two column (M==2) and three rows (N==3).  Thus the">There are two column (M==2) and three rows (N==3).  Thus the</param>
///<param name="result table has 8 entries.  Suppose the result table is stored">result table has 8 entries.  Suppose the result table is stored</param>
///<param name="in an array names azResult.  Then azResult holds this content:">in an array names azResult.  Then azResult holds this content:</param>
///<param name=""></param>
///<param name="<blockquote><pre>"><blockquote><pre></param>
///<param name="azResult&#91;0] = "Name";">azResult&#91;0] = "Name";</param>
///<param name="azResult&#91;1] = "Age";">azResult&#91;1] = "Age";</param>
///<param name="azResult&#91;2] = "Alice";">azResult&#91;2] = "Alice";</param>
///<param name="azResult&#91;3] = "43";">azResult&#91;3] = "43";</param>
///<param name="azResult&#91;4] = "Bob";">azResult&#91;4] = "Bob";</param>
///<param name="azResult&#91;5] = "28";">azResult&#91;5] = "28";</param>
///<param name="azResult&#91;6] = "Cindy";">azResult&#91;6] = "Cindy";</param>
///<param name="azResult&#91;7] = "21";">azResult&#91;7] = "21";</param>
///<param name="</pre></blockquote>)^"></pre></blockquote>)^</param>
///<param name=""></param>
///<param name="^The sqlite3_get_table() function evaluates one or more">^The sqlite3_get_table() function evaluates one or more</param>
///<param name="semicolon">8</param>
///<param name="string of its 2nd parameter and returns a result table to the">string of its 2nd parameter and returns a result table to the</param>
///<param name="pointer given in its 3rd parameter.">pointer given in its 3rd parameter.</param>
///<param name=""></param>
///<param name="After the application has finished with the result from sqlite3_get_table(),">After the application has finished with the result from sqlite3_get_table(),</param>
///<param name="it must pass the result table pointer to malloc_cs.sqlite3_free_table() in order to">it must pass the result table pointer to malloc_cs.sqlite3_free_table() in order to</param>
///<param name="release the memory that was malloced.  Because of the way the">release the memory that was malloced.  Because of the way the</param>
///<param name="[sqlite3_malloc()] happens within sqlite3_get_table(), the calling">[sqlite3_malloc()] happens within sqlite3_get_table(), the calling</param>
///<param name="function must not try to call [malloc_cs.sqlite3_free()] directly.  Only">function must not try to call [malloc_cs.sqlite3_free()] directly.  Only</param>
///<param name="[malloc_cs.sqlite3_free_table()] is able to release the memory properly and safely.">[malloc_cs.sqlite3_free_table()] is able to release the memory properly and safely.</param>
///<param name=""></param>
///<param name="The sqlite3_get_table() interface is implemented as a wrapper around">The sqlite3_get_table() interface is implemented as a wrapper around</param>
///<param name="[sqlite3_exec()].  The sqlite3_get_table() routine does not have access">[sqlite3_exec()].  The sqlite3_get_table() routine does not have access</param>
///<param name="to any internal data structures of SQLite.  It uses only the public">to any internal data structures of SQLite.  It uses only the public</param>
///<param name="interface defined here.  As a consequence, errors that occur in the">interface defined here.  As a consequence, errors that occur in the</param>
///<param name="wrapper layer outside of the internal [sqlite3_exec()] call are not">wrapper layer outside of the internal [sqlite3_exec()] call are not</param>
///<param name="reflected in subsequent calls to [sqlite3_errcode()] or">reflected in subsequent calls to [sqlite3_errcode()] or</param>
///<param name="[sqlite3_errmsg()].">[sqlite3_errmsg()].</param>
///<param name=""></param>

		//SQLITE_API int sqlite3_get_table(
		//  sqlite3 db,          /* An open database */
		//  string zSql,     /* SQL to be evaluated */
		//  char ***pazResult,    /* Results of the query */
		//  int *pnRow,           /* Number of result rows written here */
		//  int *pnColumn,        /* Number of result columns written here */
		//  char **pzErrmsg       /* Error msg written here */
		//);
		//SQLITE_API void malloc_cs.sqlite3_free_table(char **result);
		///
///<summary>
///CAPI3REF: Formatted String Printing Functions
///
///</summary>
///<param name="These routines are work">alikes of the "printf()" family of functions</param>
///<param name="from the standard C library.">from the standard C library.</param>
///<param name=""></param>
///<param name="^The io.sqlite3_mprintf() and sqlite3_vmprintf() routines write their">^The io.sqlite3_mprintf() and sqlite3_vmprintf() routines write their</param>
///<param name="results into memory obtained from [sqlite3_malloc()].">results into memory obtained from [sqlite3_malloc()].</param>
///<param name="The strings returned by these two routines should be">The strings returned by these two routines should be</param>
///<param name="released by [malloc_cs.sqlite3_free()].  ^Both routines return a">released by [malloc_cs.sqlite3_free()].  ^Both routines return a</param>
///<param name="NULL pointer if [sqlite3_malloc()] is unable to allocate enough">NULL pointer if [sqlite3_malloc()] is unable to allocate enough</param>
///<param name="memory to hold the resulting string.">memory to hold the resulting string.</param>
///<param name=""></param>
///<param name="^(The io.sqlite3_snprintf() routine is similar to "snprintf()" from">^(The io.sqlite3_snprintf() routine is similar to "snprintf()" from</param>
///<param name="the standard C library.  The result is written into the">the standard C library.  The result is written into the</param>
///<param name="buffer supplied as the second parameter whose size is given by">buffer supplied as the second parameter whose size is given by</param>
///<param name="the first parameter. Note that the order of the">the first parameter. Note that the order of the</param>
///<param name="first two parameters is reversed from snprintf().)^  This is an">first two parameters is reversed from snprintf().)^  This is an</param>
///<param name="historical accident that cannot be fixed without breaking">historical accident that cannot be fixed without breaking</param>
///<param name="backwards compatibility.  ^(Note also that io.sqlite3_snprintf()">backwards compatibility.  ^(Note also that io.sqlite3_snprintf()</param>
///<param name="returns a pointer to its buffer instead of the number of">returns a pointer to its buffer instead of the number of</param>
///<param name="characters actually written into the buffer.)^  We admit that">characters actually written into the buffer.)^  We admit that</param>
///<param name="the number of characters written would be a more useful return">the number of characters written would be a more useful return</param>
///<param name="value but we cannot change the implementation of io.sqlite3_snprintf()">value but we cannot change the implementation of io.sqlite3_snprintf()</param>
///<param name="now without breaking compatibility.">now without breaking compatibility.</param>
///<param name=""></param>
///<param name="^As long as the buffer size is greater than zero, io.sqlite3_snprintf()">^As long as the buffer size is greater than zero, io.sqlite3_snprintf()</param>
///<param name="guarantees that the buffer is always zero">terminated.  ^The first</param>
///<param name="parameter "n" is the total size of the buffer, including space for">parameter "n" is the total size of the buffer, including space for</param>
///<param name="the zero terminator.  So the longest string that can be completely">the zero terminator.  So the longest string that can be completely</param>
///<param name="written will be n">1 characters.</param>
///<param name=""></param>
///<param name="^The sqlite3_vsnprintf() routine is a varargs version of io.sqlite3_snprintf().">^The sqlite3_vsnprintf() routine is a varargs version of io.sqlite3_snprintf().</param>
///<param name=""></param>
///<param name="These routines all implement some additional formatting">These routines all implement some additional formatting</param>
///<param name="options that are useful for constructing SQL statements.">options that are useful for constructing SQL statements.</param>
///<param name="All of the usual printf() formatting options apply.  In addition, there">All of the usual printf() formatting options apply.  In addition, there</param>
///<param name="is are "%q", "%Q", and "%z" options.">is are "%q", "%Q", and "%z" options.</param>
///<param name=""></param>
///<param name="^(The %q option works like %s in that it substitutes a null">terminated</param>
///<param name="string from the argument list.  But %q also doubles every '\'' character.">string from the argument list.  But %q also doubles every '\'' character.</param>
///<param name="%q is designed for use inside a string literal.)^  By doubling each '\''">%q is designed for use inside a string literal.)^  By doubling each '\''</param>
///<param name="character it escapes that character and allows it to be inserted into">character it escapes that character and allows it to be inserted into</param>
///<param name="the string.">the string.</param>
///<param name=""></param>
///<param name="For example, assume the string variable zText contains text as follows:">For example, assume the string variable zText contains text as follows:</param>
///<param name=""></param>
///<param name="<blockquote><pre>"><blockquote><pre></param>
///<param name="string zText = "It's a happy day!";">string zText = "It's a happy day!";</param>
///<param name="</pre></blockquote>"></pre></blockquote></param>
///<param name=""></param>
///<param name="One can use this text in an SQL statement as follows:">One can use this text in an SQL statement as follows:</param>
///<param name=""></param>
///<param name="<blockquote><pre>"><blockquote><pre></param>
///<param name="string zSQL = io.sqlite3_mprintf("INSERT INTO table VALUES('%q')", zText);">string zSQL = io.sqlite3_mprintf("INSERT INTO table VALUES('%q')", zText);</param>
///<param name="sqlite3_exec(db, zSQL, 0, 0, 0);">sqlite3_exec(db, zSQL, 0, 0, 0);</param>
///<param name="malloc_cs.sqlite3_free(zSQL);">malloc_cs.sqlite3_free(zSQL);</param>
///<param name="</pre></blockquote>"></pre></blockquote></param>
///<param name=""></param>
///<param name="Because the %q format string is used, the '\'' character in zText">Because the %q format string is used, the '\'' character in zText</param>
///<param name="is escaped and the SQL generated is as follows:">is escaped and the SQL generated is as follows:</param>
///<param name=""></param>
///<param name="<blockquote><pre>"><blockquote><pre></param>
///<param name="INSERT INTO table1 VALUES('It''s a happy day!')">INSERT INTO table1 VALUES('It''s a happy day!')</param>
///<param name="</pre></blockquote>"></pre></blockquote></param>
///<param name=""></param>
///<param name="This is correct.  Had we used %s instead of %q, the generated SQL">This is correct.  Had we used %s instead of %q, the generated SQL</param>
///<param name="would have looked like this:">would have looked like this:</param>
///<param name=""></param>
///<param name="<blockquote><pre>"><blockquote><pre></param>
///<param name="INSERT INTO table1 VALUES('It's a happy day!');">INSERT INTO table1 VALUES('It's a happy day!');</param>
///<param name="</pre></blockquote>"></pre></blockquote></param>
///<param name=""></param>
///<param name="This second example is an SQL syntax error.  As a general rule you should">This second example is an SQL syntax error.  As a general rule you should</param>
///<param name="always use %q instead of %s when inserting text into a string literal.">always use %q instead of %s when inserting text into a string literal.</param>
///<param name=""></param>
///<param name="^(The %Q option works like %q except it also adds single quotes around">^(The %Q option works like %q except it also adds single quotes around</param>
///<param name="the outside of the total string.  Additionally, if the parameter in the">the outside of the total string.  Additionally, if the parameter in the</param>
///<param name="argument list is a NULL pointer, %Q substitutes the text "NULL" (without">argument list is a NULL pointer, %Q substitutes the text "NULL" (without</param>
///<param name="single quotes).)^  So, for example, one could say:">single quotes).)^  So, for example, one could say:</param>
///<param name=""></param>
///<param name="<blockquote><pre>"><blockquote><pre></param>
///<param name="string zSQL = io.sqlite3_mprintf("INSERT INTO table VALUES(%Q)", zText);">string zSQL = io.sqlite3_mprintf("INSERT INTO table VALUES(%Q)", zText);</param>
///<param name="sqlite3_exec(db, zSQL, 0, 0, 0);">sqlite3_exec(db, zSQL, 0, 0, 0);</param>
///<param name="malloc_cs.sqlite3_free(zSQL);">malloc_cs.sqlite3_free(zSQL);</param>
///<param name="</pre></blockquote>"></pre></blockquote></param>
///<param name=""></param>
///<param name="The code above will render a correct SQL statement in the zSQL">The code above will render a correct SQL statement in the zSQL</param>
///<param name="variable even if the zText variable is a NULL pointer.">variable even if the zText variable is a NULL pointer.</param>
///<param name=""></param>
///<param name="^(The "%z" formatting option works like "%s" but with the">^(The "%z" formatting option works like "%s" but with the</param>
///<param name="addition that after the string has been read and copied into">addition that after the string has been read and copied into</param>
///<param name="the result, [malloc_cs.sqlite3_free()] is called on the input string.)^">the result, [malloc_cs.sqlite3_free()] is called on the input string.)^</param>
///<param name=""></param>

		//SQLITE_API char *io.sqlite3_mprintf(const char*,...);
		//SQLITE_API char *sqlite3_vmprintf(const char*, va_list);
		//SQLITE_API char *sqlite3_snprintf(int,char*,const char*, ...);
		//SQLITE_API char *sqlite3_vsnprintf(int,char*,const char*, va_list);
		///
///<summary>
///CAPI3REF: Memory Allocation Subsystem
///
///The SQLite core uses these three routines for all of its own
///internal memory allocation needs. "Core" in the previous sentence
///</summary>
///<param name="does not include operating">system specific VFS implementation.  The</param>
///<param name="Windows VFS uses native malloc() and free() for some operations.">Windows VFS uses native malloc() and free() for some operations.</param>
///<param name=""></param>
///<param name="^The sqlite3_malloc() routine returns a pointer to a block">^The sqlite3_malloc() routine returns a pointer to a block</param>
///<param name="of memory at least N bytes in length, where N is the parameter.">of memory at least N bytes in length, where N is the parameter.</param>
///<param name="^If sqlite3_malloc() is unable to obtain sufficient free">^If sqlite3_malloc() is unable to obtain sufficient free</param>
///<param name="memory, it returns a NULL pointer.  ^If the parameter N to">memory, it returns a NULL pointer.  ^If the parameter N to</param>
///<param name="sqlite3_malloc() is zero or negative then sqlite3_malloc() returns">sqlite3_malloc() is zero or negative then sqlite3_malloc() returns</param>
///<param name="a NULL pointer.">a NULL pointer.</param>
///<param name=""></param>
///<param name="^Calling malloc_cs.sqlite3_free() with a pointer previously returned">^Calling malloc_cs.sqlite3_free() with a pointer previously returned</param>
///<param name="by sqlite3_malloc() or sqlite3_realloc() releases that memory so">by sqlite3_malloc() or sqlite3_realloc() releases that memory so</param>
///<param name="that it might be reused.  ^The malloc_cs.sqlite3_free() routine is">that it might be reused.  ^The malloc_cs.sqlite3_free() routine is</param>
///<param name="a no">op if is called with a NULL pointer.  Passing a NULL pointer</param>
///<param name="to malloc_cs.sqlite3_free() is harmless.  After being freed, memory">to malloc_cs.sqlite3_free() is harmless.  After being freed, memory</param>
///<param name="should neither be read nor written.  Even reading previously freed">should neither be read nor written.  Even reading previously freed</param>
///<param name="memory might result in a segmentation fault or other severe error.">memory might result in a segmentation fault or other severe error.</param>
///<param name="Memory corruption, a segmentation fault, or other severe error">Memory corruption, a segmentation fault, or other severe error</param>
///<param name="might result if malloc_cs.sqlite3_free() is called with a non">NULL pointer that</param>
///<param name="was not obtained from sqlite3_malloc() or sqlite3_realloc().">was not obtained from sqlite3_malloc() or sqlite3_realloc().</param>
///<param name=""></param>
///<param name="^(The sqlite3_realloc() interface attempts to resize a">^(The sqlite3_realloc() interface attempts to resize a</param>
///<param name="prior memory allocation to be at least N bytes, where N is the">prior memory allocation to be at least N bytes, where N is the</param>
///<param name="second parameter.  The memory allocation to be resized is the first">second parameter.  The memory allocation to be resized is the first</param>
///<param name="parameter.)^ ^ If the first parameter to sqlite3_realloc()">parameter.)^ ^ If the first parameter to sqlite3_realloc()</param>
///<param name="is a NULL pointer then its behavior is identical to calling">is a NULL pointer then its behavior is identical to calling</param>
///<param name="sqlite3_malloc(N) where N is the second parameter to sqlite3_realloc().">sqlite3_malloc(N) where N is the second parameter to sqlite3_realloc().</param>
///<param name="^If the second parameter to sqlite3_realloc() is zero or">^If the second parameter to sqlite3_realloc() is zero or</param>
///<param name="negative then the behavior is exactly the same as calling">negative then the behavior is exactly the same as calling</param>
///<param name="malloc_cs.sqlite3_free(P) where P is the first parameter to sqlite3_realloc().">malloc_cs.sqlite3_free(P) where P is the first parameter to sqlite3_realloc().</param>
///<param name="^sqlite3_realloc() returns a pointer to a memory allocation">^sqlite3_realloc() returns a pointer to a memory allocation</param>
///<param name="of at least N bytes in size or NULL if sufficient memory is unavailable.">of at least N bytes in size or NULL if sufficient memory is unavailable.</param>
///<param name="^If M is the size of the prior allocation, then min(N,M) bytes">^If M is the size of the prior allocation, then min(N,M) bytes</param>
///<param name="of the prior allocation are copied into the beginning of buffer returned">of the prior allocation are copied into the beginning of buffer returned</param>
///<param name="by sqlite3_realloc() and the prior allocation is freed.">by sqlite3_realloc() and the prior allocation is freed.</param>
///<param name="^If sqlite3_realloc() returns NULL, then the prior allocation">^If sqlite3_realloc() returns NULL, then the prior allocation</param>
///<param name="is not freed.">is not freed.</param>
///<param name=""></param>
///<param name="^The memory returned by sqlite3_malloc() and sqlite3_realloc()">^The memory returned by sqlite3_malloc() and sqlite3_realloc()</param>
///<param name="is always aligned to at least an 8 byte boundary, or to a">is always aligned to at least an 8 byte boundary, or to a</param>
///<param name="4 byte boundary if the [SQLITE_4_BYTE_ALIGNED_MALLOC] compile">time</param>
///<param name="option is used.">option is used.</param>
///<param name=""></param>
///<param name="In SQLite version 3.5.0 and 3.5.1, it was possible to define">In SQLite version 3.5.0 and 3.5.1, it was possible to define</param>
///<param name="the SQLITE_OMIT_MEMORY_ALLOCATION which would cause the built">in</param>
///<param name="implementation of these routines to be omitted.  That capability">implementation of these routines to be omitted.  That capability</param>
///<param name="is no longer provided.  Only built">in memory allocators can be used.</param>
///<param name=""></param>
///<param name="The Windows OS interface layer calls">The Windows OS interface layer calls</param>
///<param name="the system malloc() and free() directly when converting">the system malloc() and free() directly when converting</param>
///<param name="filenames between the UTF">8 encoding used by SQLite</param>
///<param name="and whatever filename encoding is used by the particular Windows">and whatever filename encoding is used by the particular Windows</param>
///<param name="installation.  Memory allocation errors are detected, but">installation.  Memory allocation errors are detected, but</param>
///<param name="they are reported back as [SQLITE_CANTOPEN] or">they are reported back as [SQLITE_CANTOPEN] or</param>
///<param name="[SQLITE_IOERR] rather than [SQLITE_NOMEM].">[SQLITE_IOERR] rather than [SQLITE_NOMEM].</param>
///<param name=""></param>
///<param name="The pointer arguments to [malloc_cs.sqlite3_free()] and [sqlite3_realloc()]">The pointer arguments to [malloc_cs.sqlite3_free()] and [sqlite3_realloc()]</param>
///<param name="must be either NULL or else pointers obtained from a prior">must be either NULL or else pointers obtained from a prior</param>
///<param name="invocation of [sqlite3_malloc()] or [sqlite3_realloc()] that have">invocation of [sqlite3_malloc()] or [sqlite3_realloc()] that have</param>
///<param name="not yet been released.">not yet been released.</param>
///<param name=""></param>
///<param name="The application must not read or write any part of">The application must not read or write any part of</param>
///<param name="a block of memory after it has been released using">a block of memory after it has been released using</param>
///<param name="[malloc_cs.sqlite3_free()] or [sqlite3_realloc()].">[malloc_cs.sqlite3_free()] or [sqlite3_realloc()].</param>
///<param name=""></param>

		//SQLITE_API void *sqlite3_malloc(int);
		//SQLITE_API void *sqlite3_realloc(void*, int);
		//SQLITE_API void malloc_cs.sqlite3_free(void);
		///
///<summary>
///CAPI3REF: Memory Allocator Statistics
///
///SQLite provides these two interfaces for reporting on the status
///of the [sqlite3_malloc()], [malloc_cs.sqlite3_free()], and [sqlite3_realloc()]
///</summary>
///<param name="routines, which form the built">in memory allocation subsystem.</param>
///<param name=""></param>
///<param name="^The [sqlite3_memory_used()] routine returns the number of bytes">^The [sqlite3_memory_used()] routine returns the number of bytes</param>
///<param name="of memory currently outstanding (malloced but not freed).">of memory currently outstanding (malloced but not freed).</param>
///<param name="^The [sqlite3_memory_highwater()] routine returns the maximum">^The [sqlite3_memory_highwater()] routine returns the maximum</param>
///<param name="value of [sqlite3_memory_used()] since the high">water mark</param>
///<param name="was last reset.  ^The values returned by [sqlite3_memory_used()] and">was last reset.  ^The values returned by [sqlite3_memory_used()] and</param>
///<param name="[sqlite3_memory_highwater()] include any overhead">[sqlite3_memory_highwater()] include any overhead</param>
///<param name="added by SQLite in its implementation of [sqlite3_malloc()],">added by SQLite in its implementation of [sqlite3_malloc()],</param>
///<param name="but not overhead added by the any underlying system library">but not overhead added by the any underlying system library</param>
///<param name="routines that [sqlite3_malloc()] may call.">routines that [sqlite3_malloc()] may call.</param>
///<param name=""></param>
///<param name="^The memory high">water mark is reset to the current value of</param>
///<param name="[sqlite3_memory_used()] if and only if the parameter to">[sqlite3_memory_used()] if and only if the parameter to</param>
///<param name="[sqlite3_memory_highwater()] is true.  ^The value returned">[sqlite3_memory_highwater()] is true.  ^The value returned</param>
///<param name="by [sqlite3_memory_highwater(1)] is the high">water mark</param>
///<param name="prior to the reset.">prior to the reset.</param>
///<param name=""></param>

		//SQLITE_API sqlite3_int64 sqlite3_memory_used(void);
		//SQLITE_API sqlite3_int64 sqlite3_memory_highwater(int resetFlag);
		///
///<summary>
///</summary>
///<param name="CAPI3REF: Pseudo">Random Number Generator</param>
///<param name=""></param>
///<param name="SQLite contains a high">random number generator (PRNG) used to</param>
///<param name="select random [ROWID | ROWIDs] when inserting new records into a table that">select random [ROWID | ROWIDs] when inserting new records into a table that</param>
///<param name="already uses the largest possible [ROWID].  The PRNG is also used for">already uses the largest possible [ROWID].  The PRNG is also used for</param>
///<param name="the build">in random() and randomblob() SQL functions.  This interface allows</param>
///<param name="applications to access the same PRNG for other purposes.">applications to access the same PRNG for other purposes.</param>
///<param name=""></param>
///<param name="^A call to this routine stores N bytes of randomness into buffer P.">^A call to this routine stores N bytes of randomness into buffer P.</param>
///<param name=""></param>
///<param name="^The first time this routine is invoked (either internally or by">^The first time this routine is invoked (either internally or by</param>
///<param name="the application) the PRNG is seeded using randomness obtained">the application) the PRNG is seeded using randomness obtained</param>
///<param name="from the xRandomness method of the default [sqlite3_vfs] object.">from the xRandomness method of the default [sqlite3_vfs] object.</param>
///<param name="^On all subsequent invocations, the pseudo">randomness is generated</param>
///<param name="internally and without recourse to the [sqlite3_vfs] xRandomness">internally and without recourse to the [sqlite3_vfs] xRandomness</param>
///<param name="method.">method.</param>
///<param name=""></param>

		//SQLITE_API void sqlite3_randomness(int N, object  *P);
		///
///<summary>
///</summary>
///<param name="CAPI3REF: Compile">Time Authorization Callbacks</param>
///<param name=""></param>
///<param name="^This routine registers an authorizer callback with a particular">^This routine registers an authorizer callback with a particular</param>
///<param name="[database connection], supplied in the first argument.">[database connection], supplied in the first argument.</param>
///<param name="^The authorizer callback is invoked as SQL statements are being compiled">^The authorizer callback is invoked as SQL statements are being compiled</param>
///<param name="by [sqlite3_prepare()] or its variants [sqlite3_prepare_v2()],">by [sqlite3_prepare()] or its variants [sqlite3_prepare_v2()],</param>
///<param name="[sqlite3_prepare16()] and [sqlite3_prepare16_v2()].  ^At various">[sqlite3_prepare16()] and [sqlite3_prepare16_v2()].  ^At various</param>
///<param name="points during the compilation process, as logic is being created">points during the compilation process, as logic is being created</param>
///<param name="to perform various actions, the authorizer callback is invoked to">to perform various actions, the authorizer callback is invoked to</param>
///<param name="see if those actions are allowed.  ^The authorizer callback should">see if those actions are allowed.  ^The authorizer callback should</param>
///<param name="return [SqlResult.SQLITE_OK] to allow the action, [SQLITE_IGNORE] to disallow the">return [SqlResult.SQLITE_OK] to allow the action, [SQLITE_IGNORE] to disallow the</param>
///<param name="specific action but allow the SQL statement to continue to be">specific action but allow the SQL statement to continue to be</param>
///<param name="compiled, or [SQLITE_DENY] to cause the entire SQL statement to be">compiled, or [SQLITE_DENY] to cause the entire SQL statement to be</param>
///<param name="rejected with an error.  ^If the authorizer callback returns">rejected with an error.  ^If the authorizer callback returns</param>
///<param name="any value other than [SQLITE_IGNORE], [SqlResult.SQLITE_OK], or [SQLITE_DENY]">any value other than [SQLITE_IGNORE], [SqlResult.SQLITE_OK], or [SQLITE_DENY]</param>
///<param name="then the [sqlite3_prepare_v2()] or equivalent call that triggered">then the [sqlite3_prepare_v2()] or equivalent call that triggered</param>
///<param name="the authorizer will fail with an error message.">the authorizer will fail with an error message.</param>
///<param name=""></param>
///<param name="When the callback returns [SqlResult.SQLITE_OK], that means the operation">When the callback returns [SqlResult.SQLITE_OK], that means the operation</param>
///<param name="requested is ok.  ^When the callback returns [SQLITE_DENY], the">requested is ok.  ^When the callback returns [SQLITE_DENY], the</param>
///<param name="[sqlite3_prepare_v2()] or equivalent call that triggered the">[sqlite3_prepare_v2()] or equivalent call that triggered the</param>
///<param name="authorizer will fail with an error message explaining that">authorizer will fail with an error message explaining that</param>
///<param name="access is denied. ">access is denied. </param>
///<param name=""></param>
///<param name="^The first parameter to the authorizer callback is a copy of the third">^The first parameter to the authorizer callback is a copy of the third</param>
///<param name="parameter to the sqlite3_set_authorizer() interface. ^The second parameter">parameter to the sqlite3_set_authorizer() interface. ^The second parameter</param>
///<param name="to the callback is an integer [SQLITE_COPY | action code] that specifies">to the callback is an integer [SQLITE_COPY | action code] that specifies</param>
///<param name="the particular action to be authorized. ^The third through sixth parameters">the particular action to be authorized. ^The third through sixth parameters</param>
///<param name="to the callback are zero">terminated strings that contain additional</param>
///<param name="details about the action to be authorized.">details about the action to be authorized.</param>
///<param name=""></param>
///<param name="^If the action code is [SQLITE_READ]">^If the action code is [SQLITE_READ]</param>
///<param name="and the callback returns [SQLITE_IGNORE] then the">and the callback returns [SQLITE_IGNORE] then the</param>
///<param name="[prepared statement] statement is constructed to substitute">[prepared statement] statement is constructed to substitute</param>
///<param name="a NULL value in place of the table column that would have">a NULL value in place of the table column that would have</param>
///<param name="been read if [SqlResult.SQLITE_OK] had been returned.  The [SQLITE_IGNORE]">been read if [SqlResult.SQLITE_OK] had been returned.  The [SQLITE_IGNORE]</param>
///<param name="return can be used to deny an untrusted user access to individual">return can be used to deny an untrusted user access to individual</param>
///<param name="columns of a table.">columns of a table.</param>
///<param name="^If the action code is [SQLITE_DELETE] and the callback returns">^If the action code is [SQLITE_DELETE] and the callback returns</param>
///<param name="[SQLITE_IGNORE] then the [DELETE] operation proceeds but the">[SQLITE_IGNORE] then the [DELETE] operation proceeds but the</param>
///<param name="[truncate optimization] is disabled and all rows are deleted individually.">[truncate optimization] is disabled and all rows are deleted individually.</param>
///<param name=""></param>
///<param name="An authorizer is used when [sqlite3_prepare | preparing]">An authorizer is used when [sqlite3_prepare | preparing]</param>
///<param name="SQL statements from an untrusted source, to ensure that the SQL statements">SQL statements from an untrusted source, to ensure that the SQL statements</param>
///<param name="do not try to access data they are not allowed to see, or that they do not">do not try to access data they are not allowed to see, or that they do not</param>
///<param name="try to execute malicious statements that damage the database.  For">try to execute malicious statements that damage the database.  For</param>
///<param name="example, an application may allow a user to enter arbitrary">example, an application may allow a user to enter arbitrary</param>
///<param name="SQL queries for evaluation by a database.  But the application does">SQL queries for evaluation by a database.  But the application does</param>
///<param name="not want the user to be able to make arbitrary changes to the">not want the user to be able to make arbitrary changes to the</param>
///<param name="database.  An authorizer could then be put in place while the">database.  An authorizer could then be put in place while the</param>
///<param name="user">entered SQL is being [sqlite3_prepare | prepared] that</param>
///<param name="disallows everything except [SELECT] statements.">disallows everything except [SELECT] statements.</param>
///<param name=""></param>
///<param name="Applications that need to process SQL from untrusted sources">Applications that need to process SQL from untrusted sources</param>
///<param name="might also consider lowering resource limits using [sqlite3_limit()]">might also consider lowering resource limits using [sqlite3_limit()]</param>
///<param name="and limiting database size using the [max_page_count] [PRAGMA]">and limiting database size using the [max_page_count] [PRAGMA]</param>
///<param name="in addition to using an authorizer.">in addition to using an authorizer.</param>
///<param name=""></param>
///<param name="^(Only a single authorizer can be in place on a database connection">^(Only a single authorizer can be in place on a database connection</param>
///<param name="at a time.  Each call to sqlite3_set_authorizer overrides the">at a time.  Each call to sqlite3_set_authorizer overrides the</param>
///<param name="previous call.)^  ^Disable the authorizer by installing a NULL callback.">previous call.)^  ^Disable the authorizer by installing a NULL callback.</param>
///<param name="The authorizer is disabled by default.">The authorizer is disabled by default.</param>
///<param name=""></param>
///<param name="The authorizer callback must not do anything that will modify">The authorizer callback must not do anything that will modify</param>
///<param name="the database connection that invoked the authorizer callback.">the database connection that invoked the authorizer callback.</param>
///<param name="Note that [sqlite3_prepare_v2()] and [sqlite3_step()] both modify their">Note that [sqlite3_prepare_v2()] and [sqlite3_step()] both modify their</param>
///<param name="database connections for the meaning of "modify" in this paragraph.">database connections for the meaning of "modify" in this paragraph.</param>
///<param name=""></param>
///<param name="^When [sqlite3_prepare_v2()] is used to prepare a statement, the">^When [sqlite3_prepare_v2()] is used to prepare a statement, the</param>
///<param name="statement might be re">prepared during [sqlite3_step()] due to a </param>
///<param name="schema change.  Hence, the application should ensure that the">schema change.  Hence, the application should ensure that the</param>
///<param name="correct authorizer callback remains in place during the [sqlite3_step()].">correct authorizer callback remains in place during the [sqlite3_step()].</param>
///<param name=""></param>
///<param name="^Note that the authorizer callback is invoked only during">^Note that the authorizer callback is invoked only during</param>
///<param name="[sqlite3_prepare()] or its variants.  Authorization is not">[sqlite3_prepare()] or its variants.  Authorization is not</param>
///<param name="performed during statement evaluation in [sqlite3_step()], unless">performed during statement evaluation in [sqlite3_step()], unless</param>
///<param name="as stated in the previous paragraph, sqlite3_step() invokes">as stated in the previous paragraph, sqlite3_step() invokes</param>
///<param name="sqlite3_prepare_v2() to reprepare a statement after a schema change.">sqlite3_prepare_v2() to reprepare a statement after a schema change.</param>
///<param name=""></param>

		//SQLITE_API int sqlite3_set_authorizer(
		//  sqlite3*,
		//  int (*xAuth)(void*,int,const char*,const char*,const char*,const char),
		//  void *pUserData
		//);
		///

		///
///<summary>
///CAPI3REF: Tracing And Profiling Functions
///
///These routines register callback functions that can be used for
///tracing and profiling the execution of SQL statements.
///
///^The callback function registered by sqlite3_trace() is invoked at
///various times when an SQL statement is being run by [sqlite3_step()].
///</summary>
///<param name="^The sqlite3_trace() callback is invoked with a UTF">8 rendering of the</param>
///<param name="SQL statement text as the statement first begins executing.">SQL statement text as the statement first begins executing.</param>
///<param name="^(Additional sqlite3_trace() callbacks might occur">^(Additional sqlite3_trace() callbacks might occur</param>
///<param name="as each triggered subprogram is entered.  The callbacks for triggers">as each triggered subprogram is entered.  The callbacks for triggers</param>
///<param name="contain a UTF">8 SQL comment that identifies the trigger.)^</param>
///<param name=""></param>
///<param name="^The callback function registered by sqlite3_profile() is invoked">^The callback function registered by sqlite3_profile() is invoked</param>
///<param name="as each SQL statement finishes.  ^The profile callback contains">as each SQL statement finishes.  ^The profile callback contains</param>
///<param name="the original statement text and an estimate of wall">clock time</param>
///<param name="of how long that statement took to run.  ^The profile callback">of how long that statement took to run.  ^The profile callback</param>
///<param name="time is in units of nanoseconds, however the current implementation">time is in units of nanoseconds, however the current implementation</param>
///<param name="is only capable of millisecond resolution so the six least significant">is only capable of millisecond resolution so the six least significant</param>
///<param name="digits in the time are meaningless.  Future versions of SQLite">digits in the time are meaningless.  Future versions of SQLite</param>
///<param name="might provide greater resolution on the profiler callback.  The">might provide greater resolution on the profiler callback.  The</param>
///<param name="sqlite3_profile() function is considered experimental and is">sqlite3_profile() function is considered experimental and is</param>
///<param name="subject to change in future versions of SQLite.">subject to change in future versions of SQLite.</param>
///<param name=""></param>

		//SQLITE_API void *sqlite3_trace(sqlite3*, void(*xTrace)(void*,const char), void);
		//SQLITE_API SQLITE_EXPERIMENTAL void *sqlite3_profile(sqlite3*,
		//   void(*xProfile)(void*,const char*,sqlite3_uint64), void);
		///
///<summary>
///CAPI3REF: Query Progress Callbacks
///
///^The sqlite3_progress_handler(D,N,X,P) interface causes the callback
///function X to be invoked periodically during long running calls to
///[sqlite3_exec()], [sqlite3_step()] and [sqlite3_get_table()] for
///database connection D.  An example use for this
///interface is to keep a GUI updated during a large query.
///
///^The parameter P is passed through as the only parameter to the 
///callback function X.  ^The parameter N is the number of 
///[virtual machine instructions] that are evaluated between successive
///invocations of the callback X.
///
///^Only a single progress handler may be defined at one time per
///[database connection]; setting a new progress handler cancels the
///old one.  ^Setting parameter X to NULL disables the progress handler.
///^The progress handler is also disabled by setting N to a value less
///than 1.
///
///</summary>
///<param name="^If the progress callback returns non">zero, the operation is</param>
///<param name="interrupted.  This feature can be used to implement a">interrupted.  This feature can be used to implement a</param>
///<param name=""Cancel" button on a GUI progress dialog box.">"Cancel" button on a GUI progress dialog box.</param>
///<param name=""></param>
///<param name="The progress handler callback must not do anything that will modify">The progress handler callback must not do anything that will modify</param>
///<param name="the database connection that invoked the progress handler.">the database connection that invoked the progress handler.</param>
///<param name="Note that [sqlite3_prepare_v2()] and [sqlite3_step()] both modify their">Note that [sqlite3_prepare_v2()] and [sqlite3_step()] both modify their</param>
///<param name="database connections for the meaning of "modify" in this paragraph.">database connections for the meaning of "modify" in this paragraph.</param>
///<param name=""></param>
///<param name=""></param>

		//SQLITE_API void sqlite3_progress_handler(sqlite3*, int, int()(void), void);
		///
///<summary>
///CAPI3REF: Opening A New Database Connection
///
///^These routines open an SQLite database file as specified by the 
///</summary>
///<param name="filename argument. ^The filename argument is interpreted as UTF">8 for</param>
///<param name="sqlite3_open() and sqlite3_open_v2() and as UTF">16 in the native byte</param>
///<param name="order for sqlite3_open16(). ^(A [database connection] handle is usually">order for sqlite3_open16(). ^(A [database connection] handle is usually</param>
///<param name="returned in *ppDb, even if an error occurs.  The only exception is that">returned in *ppDb, even if an error occurs.  The only exception is that</param>
///<param name="if SQLite is unable to allocate memory to hold the [sqlite3] object,">if SQLite is unable to allocate memory to hold the [sqlite3] object,</param>
///<param name="a NULL will be written into *ppDb instead of a pointer to the [sqlite3]">a NULL will be written into *ppDb instead of a pointer to the [sqlite3]</param>
///<param name="object.)^ ^(If the database is opened (and/or created) successfully, then">object.)^ ^(If the database is opened (and/or created) successfully, then</param>
///<param name="[SqlResult.SQLITE_OK] is returned.  Otherwise an [error code] is returned.)^ ^The">[SqlResult.SQLITE_OK] is returned.  Otherwise an [error code] is returned.)^ ^The</param>
///<param name="[sqlite3_errmsg()] or [sqlite3_errmsg16()] routines can be used to obtain">[sqlite3_errmsg()] or [sqlite3_errmsg16()] routines can be used to obtain</param>
///<param name="an English language description of the error following a failure of any">an English language description of the error following a failure of any</param>
///<param name="of the sqlite3_open() routines.">of the sqlite3_open() routines.</param>
///<param name=""></param>
///<param name="^The default encoding for the database will be UTF">8 if</param>
///<param name="sqlite3_open() or sqlite3_open_v2() is called and">sqlite3_open() or sqlite3_open_v2() is called and</param>
///<param name="UTF">16 in the native byte order if sqlite3_open16() is used.</param>
///<param name=""></param>
///<param name="Whether or not an error occurs when it is opened, resources">Whether or not an error occurs when it is opened, resources</param>
///<param name="associated with the [database connection] handle should be released by">associated with the [database connection] handle should be released by</param>
///<param name="passing it to [sqlite3_close()] when it is no longer required.">passing it to [sqlite3_close()] when it is no longer required.</param>
///<param name=""></param>
///<param name="The sqlite3_open_v2() interface works like sqlite3_open()">The sqlite3_open_v2() interface works like sqlite3_open()</param>
///<param name="except that it accepts two additional parameters for additional control">except that it accepts two additional parameters for additional control</param>
///<param name="over the new database connection.  ^(The flags parameter to">over the new database connection.  ^(The flags parameter to</param>
///<param name="sqlite3_open_v2() can take one of">sqlite3_open_v2() can take one of</param>
///<param name="the following three values, optionally combined with the ">the following three values, optionally combined with the </param>
///<param name="[SQLITE_OPEN_NOMUTEX], [SQLITE_OPEN_FULLMUTEX], [SQLITE_OPEN_SHAREDCACHE],">[SQLITE_OPEN_NOMUTEX], [SQLITE_OPEN_FULLMUTEX], [SQLITE_OPEN_SHAREDCACHE],</param>
///<param name="[SQLITE_OPEN_PRIVATECACHE], and/or [SQLITE_OPEN_URI] flags:)^">[SQLITE_OPEN_PRIVATECACHE], and/or [SQLITE_OPEN_URI] flags:)^</param>
///<param name=""></param>
///<param name="<dl>"><dl></param>
///<param name="^(<dt>[SQLITE_OPEN_READONLY]</dt>">^(<dt>[SQLITE_OPEN_READONLY]</dt></param>
///<param name="<dd>The database is opened in read">only mode.  If the database does not</param>
///<param name="already exist, an error is returned.</dd>)^">already exist, an error is returned.</dd>)^</param>
///<param name=""></param>
///<param name="^(<dt>[SQLITE_OPEN_READWRITE]</dt>">^(<dt>[SQLITE_OPEN_READWRITE]</dt></param>
///<param name="<dd>The database is opened for reading and writing if possible, or reading"><dd>The database is opened for reading and writing if possible, or reading</param>
///<param name="only if the file is write protected by the operating system.  In either">only if the file is write protected by the operating system.  In either</param>
///<param name="case the database must already exist, otherwise an error is returned.</dd>)^">case the database must already exist, otherwise an error is returned.</dd>)^</param>
///<param name=""></param>
///<param name="^(<dt>[SQLITE_OPEN_READWRITE] | [SQLITE_OPEN_CREATE]</dt>">^(<dt>[SQLITE_OPEN_READWRITE] | [SQLITE_OPEN_CREATE]</dt></param>
///<param name="<dd>The database is opened for reading and writing, and is created if"><dd>The database is opened for reading and writing, and is created if</param>
///<param name="it does not already exist. This is the behavior that is always used for">it does not already exist. This is the behavior that is always used for</param>
///<param name="sqlite3_open() and sqlite3_open16().</dd>)^">sqlite3_open() and sqlite3_open16().</dd>)^</param>
///<param name="</dl>"></dl></param>
///<param name=""></param>
///<param name="If the 3rd parameter to sqlite3_open_v2() is not one of the">If the 3rd parameter to sqlite3_open_v2() is not one of the</param>
///<param name="combinations shown above optionally combined with other">combinations shown above optionally combined with other</param>
///<param name="[SQLITE_OPEN_READONLY | SQLITE_OPEN_* bits]">[SQLITE_OPEN_READONLY | SQLITE_OPEN_* bits]</param>
///<param name="then the behavior is undefined.">then the behavior is undefined.</param>
///<param name=""></param>
///<param name="^If the [SQLITE_OPEN_NOMUTEX] flag is set, then the database connection">^If the [SQLITE_OPEN_NOMUTEX] flag is set, then the database connection</param>
///<param name="opens in the multi">thread</param>
///<param name="mode has not been set at compile">time.  ^If the</param>
///<param name="[SQLITE_OPEN_FULLMUTEX] flag is set then the database connection opens">[SQLITE_OPEN_FULLMUTEX] flag is set then the database connection opens</param>
///<param name="in the serialized [threading mode] unless single">thread was</param>
///<param name="previously selected at compile">time.</param>
///<param name="^The [SQLITE_OPEN_SHAREDCACHE] flag causes the database connection to be">^The [SQLITE_OPEN_SHAREDCACHE] flag causes the database connection to be</param>
///<param name="eligible to use [shared cache mode], regardless of whether or not shared">eligible to use [shared cache mode], regardless of whether or not shared</param>
///<param name="cache is enabled using [sqlite3_enable_shared_cache()].  ^The">cache is enabled using [sqlite3_enable_shared_cache()].  ^The</param>
///<param name="[SQLITE_OPEN_PRIVATECACHE] flag causes the database connection to not">[SQLITE_OPEN_PRIVATECACHE] flag causes the database connection to not</param>
///<param name="participate in [shared cache mode] even if it is enabled.">participate in [shared cache mode] even if it is enabled.</param>
///<param name=""></param>
///<param name="^The fourth parameter to sqlite3_open_v2() is the name of the">^The fourth parameter to sqlite3_open_v2() is the name of the</param>
///<param name="[sqlite3_vfs] object that defines the operating system interface that">[sqlite3_vfs] object that defines the operating system interface that</param>
///<param name="the new database connection should use.  ^If the fourth parameter is">the new database connection should use.  ^If the fourth parameter is</param>
///<param name="a NULL pointer then the default [sqlite3_vfs] object is used.">a NULL pointer then the default [sqlite3_vfs] object is used.</param>
///<param name=""></param>
///<param name="^If the filename is ":memory:", then a private, temporary in">memory database</param>
///<param name="is created for the connection.  ^This in">memory database will vanish when</param>
///<param name="the database connection is closed.  Future versions of SQLite might">the database connection is closed.  Future versions of SQLite might</param>
///<param name="make use of additional special filenames that begin with the ":" character.">make use of additional special filenames that begin with the ":" character.</param>
///<param name="It is recommended that when a database filename actually does begin with">It is recommended that when a database filename actually does begin with</param>
///<param name="a ":" character you should prefix the filename with a pathname such as">a ":" character you should prefix the filename with a pathname such as</param>
///<param name=""./" to avoid ambiguity.">"./" to avoid ambiguity.</param>
///<param name=""></param>
///<param name="^If the filename is an empty string, then a private, temporary">^If the filename is an empty string, then a private, temporary</param>
///<param name="on">disk database will be created.  ^This public database will be</param>
///<param name="automatically deleted as soon as the database connection is closed.">automatically deleted as soon as the database connection is closed.</param>
///<param name=""></param>
///<param name="[[URI filenames in sqlite3_open()]] <h3>URI Filenames</h3>">[[URI filenames in sqlite3_open()]] <h3>URI Filenames</h3></param>
///<param name=""></param>
///<param name="^If [URI filename] interpretation is enabled, and the filename argument">^If [URI filename] interpretation is enabled, and the filename argument</param>
///<param name="begins with "file:", then the filename is interpreted as a URI. ^URI">begins with "file:", then the filename is interpreted as a URI. ^URI</param>
///<param name="filename interpretation is enabled if the [SQLITE_OPEN_URI] flag is">filename interpretation is enabled if the [SQLITE_OPEN_URI] flag is</param>
///<param name="set in the fourth argument to sqlite3_open_v2(), or if it has">set in the fourth argument to sqlite3_open_v2(), or if it has</param>
///<param name="been enabled globally using the [SQLITE_CONFIG_URI] option with the">been enabled globally using the [SQLITE_CONFIG_URI] option with the</param>
///<param name="[sqlite3_config()] method or by the [SQLITE_USE_URI] compile">time option.</param>
///<param name="As of SQLite version 3.7.7, URI filename interpretation is turned off">As of SQLite version 3.7.7, URI filename interpretation is turned off</param>
///<param name="by default, but future releases of SQLite might enable URI filename">by default, but future releases of SQLite might enable URI filename</param>
///<param name="interpretation by default.  See "[URI filenames]" for additional">interpretation by default.  See "[URI filenames]" for additional</param>
///<param name="information.">information.</param>
///<param name=""></param>
///<param name="URI filenames are parsed according to RFC 3986. ^If the URI contains an">URI filenames are parsed according to RFC 3986. ^If the URI contains an</param>
///<param name="authority, then it must be either an empty string or the string ">authority, then it must be either an empty string or the string </param>
///<param name=""localhost". ^If the authority is not an empty string or "localhost", an ">"localhost". ^If the authority is not an empty string or "localhost", an </param>
///<param name="error is returned to the caller. ^The fragment component of a URI, if ">error is returned to the caller. ^The fragment component of a URI, if </param>
///<param name="present, is ignored.">present, is ignored.</param>
///<param name=""></param>
///<param name="^SQLite uses the path component of the URI as the name of the disk file">^SQLite uses the path component of the URI as the name of the disk file</param>
///<param name="which contains the database. ^If the path begins with a '/' character, ">which contains the database. ^If the path begins with a '/' character, </param>
///<param name="then it is interpreted as an absolute path. ^If the path does not begin ">then it is interpreted as an absolute path. ^If the path does not begin </param>
///<param name="with a '/' (meaning that the authority section is omitted from the URI)">with a '/' (meaning that the authority section is omitted from the URI)</param>
///<param name="then the path is interpreted as a relative path. ">then the path is interpreted as a relative path. </param>
///<param name="^On windows, the first component of an absolute path ">^On windows, the first component of an absolute path </param>
///<param name="is a drive specification (e.g. "C:").">is a drive specification (e.g. "C:").</param>
///<param name=""></param>
///<param name="[[core URI query parameters]]">[[core URI query parameters]]</param>
///<param name="The query component of a URI may contain parameters that are interpreted">The query component of a URI may contain parameters that are interpreted</param>
///<param name="either by SQLite itself, or by a [VFS | custom VFS implementation].">either by SQLite itself, or by a [VFS | custom VFS implementation].</param>
///<param name="SQLite interprets the following three query parameters:">SQLite interprets the following three query parameters:</param>
///<param name=""></param>
///<param name="<ul>"><ul></param>
///<param name="<li> <b>vfs</b>: ^The "vfs" parameter may be used to specify the name of"><li> <b>vfs</b>: ^The "vfs" parameter may be used to specify the name of</param>
///<param name="a VFS object that provides the operating system interface that should">a VFS object that provides the operating system interface that should</param>
///<param name="be used to access the database file on disk. ^If this option is set to">be used to access the database file on disk. ^If this option is set to</param>
///<param name="an empty string the default VFS object is used. ^Specifying an unknown">an empty string the default VFS object is used. ^Specifying an unknown</param>
///<param name="VFS is an error. ^If sqlite3_open_v2() is used and the vfs option is">VFS is an error. ^If sqlite3_open_v2() is used and the vfs option is</param>
///<param name="present, then the VFS specified by the option takes precedence over">present, then the VFS specified by the option takes precedence over</param>
///<param name="the value passed as the fourth parameter to sqlite3_open_v2().">the value passed as the fourth parameter to sqlite3_open_v2().</param>
///<param name=""></param>
///<param name="<li> <b>mode</b>: ^(The mode parameter may be set to either "ro", "rw" or"><li> <b>mode</b>: ^(The mode parameter may be set to either "ro", "rw" or</param>
///<param name=""rwc". Attempting to set it to any other value is an error)^. ">"rwc". Attempting to set it to any other value is an error)^. </param>
///<param name="^If "ro" is specified, then the database is opened for read">only </param>
///<param name="access, just as if the [SQLITE_OPEN_READONLY] flag had been set in the ">access, just as if the [SQLITE_OPEN_READONLY] flag had been set in the </param>
///<param name="third argument to sqlite3_prepare_v2(). ^If the mode option is set to ">third argument to sqlite3_prepare_v2(). ^If the mode option is set to </param>
///<param name=""rw", then the database is opened for read">write (but not create) </param>
///<param name="access, as if SQLITE_OPEN_READWRITE (but not SQLITE_OPEN_CREATE) had ">access, as if SQLITE_OPEN_READWRITE (but not SQLITE_OPEN_CREATE) had </param>
///<param name="been set. ^Value "rwc" is equivalent to setting both ">been set. ^Value "rwc" is equivalent to setting both </param>
///<param name="SQLITE_OPEN_READWRITE and SQLITE_OPEN_CREATE. ^If sqlite3_open_v2() is ">SQLITE_OPEN_READWRITE and SQLITE_OPEN_CREATE. ^If sqlite3_open_v2() is </param>
///<param name="used, it is an error to specify a value for the mode parameter that is ">used, it is an error to specify a value for the mode parameter that is </param>
///<param name="less restrictive than that specified by the flags passed as the third ">less restrictive than that specified by the flags passed as the third </param>
///<param name="parameter.">parameter.</param>
///<param name=""></param>
///<param name="<li> <b>cache</b>: ^The cache parameter may be set to either "shared" or"><li> <b>cache</b>: ^The cache parameter may be set to either "shared" or</param>
///<param name=""private". ^Setting it to "shared" is equivalent to setting the">"private". ^Setting it to "shared" is equivalent to setting the</param>
///<param name="SQLITE_OPEN_SHAREDCACHE bit in the flags argument passed to">SQLITE_OPEN_SHAREDCACHE bit in the flags argument passed to</param>
///<param name="sqlite3_open_v2(). ^Setting the cache parameter to "private" is ">sqlite3_open_v2(). ^Setting the cache parameter to "private" is </param>
///<param name="equivalent to setting the SQLITE_OPEN_PRIVATECACHE bit.">equivalent to setting the SQLITE_OPEN_PRIVATECACHE bit.</param>
///<param name="^If sqlite3_open_v2() is used and the "cache" parameter is present in">^If sqlite3_open_v2() is used and the "cache" parameter is present in</param>
///<param name="a URI filename, its value overrides any behaviour requested by setting">a URI filename, its value overrides any behaviour requested by setting</param>
///<param name="SQLITE_OPEN_PRIVATECACHE or SQLITE_OPEN_SHAREDCACHE flag.">SQLITE_OPEN_PRIVATECACHE or SQLITE_OPEN_SHAREDCACHE flag.</param>
///<param name="</ul>"></ul></param>
///<param name=""></param>
///<param name="^Specifying an unknown parameter in the query component of a URI is not an">^Specifying an unknown parameter in the query component of a URI is not an</param>
///<param name="error.  Future versions of SQLite might understand additional query">error.  Future versions of SQLite might understand additional query</param>
///<param name="parameters.  See "[query parameters with special meaning to SQLite]" for">parameters.  See "[query parameters with special meaning to SQLite]" for</param>
///<param name="additional information.">additional information.</param>
///<param name=""></param>
///<param name="[[URI filename examples]] <h3>URI filename examples</h3>">[[URI filename examples]] <h3>URI filename examples</h3></param>
///<param name=""></param>
///<param name="<table border="1" align=center cellpadding=5>"><table border="1" align=center cellpadding=5></param>
///<param name="<tr><th> URI filenames <th> Results"><tr><th> URI filenames <th> Results</param>
///<param name="<tr><td> file:data.db <td> "><tr><td> file:data.db <td> </param>
///<param name="Open the file "data.db" in the current directory.">Open the file "data.db" in the current directory.</param>
///<param name="<tr><td> file:/home/fred/data.db<br>"><tr><td> file:/home/fred/data.db<br></param>
///<param name="file:///home/fred/data.db <br> ">file:///home/fred/data.db <br> </param>
///<param name="file://localhost/home/fred/data.db <br> <td> ">file://localhost/home/fred/data.db <br> <td> </param>
///<param name="Open the database file "/home/fred/data.db".">Open the database file "/home/fred/data.db".</param>
///<param name="<tr><td> file://darkstar/home/fred/data.db <td> "><tr><td> file://darkstar/home/fred/data.db <td> </param>
///<param name="An error. "darkstar" is not a recognized authority.">An error. "darkstar" is not a recognized authority.</param>
///<param name="<tr><td style="white">space:nowrap"> </param>
///<param name="file:///C:/Documents%20and%20Settings/fred/Desktop/data.db">file:///C:/Documents%20and%20Settings/fred/Desktop/data.db</param>
///<param name="<td> Windows only: Open the file "data.db" on fred's desktop on drive"><td> Windows only: Open the file "data.db" on fred's desktop on drive</param>
///<param name="C:. Note that the %20 escaping in this example is not strictly ">C:. Note that the %20 escaping in this example is not strictly </param>
///<param name="necessary "> space characters can be used literally</param>
///<param name="in URI filenames.">in URI filenames.</param>
///<param name="<tr><td> file:data.db?mode=ro&cache=public <td> "><tr><td> file:data.db?mode=ro&cache=public <td> </param>
///<param name="Open file "data.db" in the current directory for read">only access.</param>
///<param name="Regardless of whether or not shared">cache mode is enabled by</param>
///<param name="default, use a public cache.">default, use a public cache.</param>
///<param name="<tr><td> file:/home/fred/data.db?vfs=unix">nolock <td></param>
///<param name="Open file "/home/fred/data.db". Use the special VFS "unix">nolock".</param>
///<param name="<tr><td> file:data.db?mode=readonly <td> "><tr><td> file:data.db?mode=readonly <td> </param>
///<param name="An error. "readonly" is not a valid option for the "mode" parameter.">An error. "readonly" is not a valid option for the "mode" parameter.</param>
///<param name="</table>"></table></param>
///<param name=""></param>
///<param name="^URI hexadecimal escape sequences (%HH) are supported within the path and">^URI hexadecimal escape sequences (%HH) are supported within the path and</param>
///<param name="query components of a URI. A hexadecimal escape sequence consists of a">query components of a URI. A hexadecimal escape sequence consists of a</param>
///<param name="percent sign "> followed by exactly two hexadecimal digits </param>
///<param name="specifying an octet value. ^Before the path or query components of a">specifying an octet value. ^Before the path or query components of a</param>
///<param name="URI filename are interpreted, they are encoded using UTF">8 and all </param>
///<param name="hexadecimal escape sequences replaced by a single byte containing the">hexadecimal escape sequences replaced by a single byte containing the</param>
///<param name="corresponding octet. If this process generates an invalid UTF">8 encoding,</param>
///<param name="the results are undefined.">the results are undefined.</param>
///<param name=""></param>
///<param name="<b>Note to Windows users:</b>  The encoding used for the filename argument"><b>Note to Windows users:</b>  The encoding used for the filename argument</param>
///<param name="of sqlite3_open() and sqlite3_open_v2() must be UTF">8, not whatever</param>
///<param name="codepage is currently defined.  Filenames containing international">codepage is currently defined.  Filenames containing international</param>
///<param name="characters must be converted to UTF">8 prior to passing them into</param>
///<param name="sqlite3_open() or sqlite3_open_v2().">sqlite3_open() or sqlite3_open_v2().</param>
///<param name=""></param>

		//SQLITE_API int sqlite3_open(
		//  string filename,   /* Database filename (UTF-8) */
		//  sqlite3 **ppDb          /* OUT: SQLite db handle */
		//);
		//SQLITE_API int sqlite3_open16(
		//  const void *filename,   /* Database filename (UTF-16) */
		//  sqlite3 **ppDb          /* OUT: SQLite db handle */
		//);
		//SQLITE_API int sqlite3_open_v2(
		//  string filename,   /* Database filename (UTF-8) */
		//  sqlite3 **ppDb,         /* OUT: SQLite db handle */
		//  int flags,              /* Flags */
		//  string zVfs        /* Name of VFS module to use */
		//);
		///
///<summary>
///CAPI3REF: Obtain Values For URI Parameters
///
///This is a utility routine, useful to VFS implementations, that checks
///to see if a database file was a URI that contained a specific query 
///parameter, and if so obtains the value of the query parameter.
///
///The zFilename argument is the filename pointer passed into the xOpen()
///method of a VFS implementation.  The zParam argument is the name of the
///query parameter we seek.  This routine returns the value of the zParam
///parameter if it exists.  If the parameter does not exist, this routine
///returns a NULL pointer.
///
///If the zFilename argument to this function is not a pointer that SQLite
///passed into the xOpen VFS method, then the behavior of this routine
///is undefined and probably undesirable.
///
///</summary>

		//SQLITE_API string sqlite3_uri_parameter(string zFilename, string zParam);
		///
///<summary>
///CAPI3REF: Error Codes And Messages
///
///^The sqlite3_errcode() interface returns the numeric [result code] or
///[extended result code] for the most recent failed sqlite3_* API call
///associated with a [database connection]. If a prior API call failed
///but the most recent API call succeeded, the return value from
///sqlite3_errcode() is undefined.  ^The sqlite3_extended_errcode()
///interface is the same except that it always returns the 
///[extended result code] even when extended result codes are
///disabled.
///
///</summary>
///<param name="^The sqlite3_errmsg() and sqlite3_errmsg16() return English">language</param>
///<param name="text that describes the error, as either UTF">16 respectively.</param>
///<param name="^(Memory to hold the error message string is managed internally.">^(Memory to hold the error message string is managed internally.</param>
///<param name="The application does not need to worry about freeing the result.">The application does not need to worry about freeing the result.</param>
///<param name="However, the error string might be overwritten or deallocated by">However, the error string might be overwritten or deallocated by</param>
///<param name="subsequent calls to other SQLite interface functions.)^">subsequent calls to other SQLite interface functions.)^</param>
///<param name=""></param>
///<param name="When the serialized [threading mode] is in use, it might be the">When the serialized [threading mode] is in use, it might be the</param>
///<param name="case that a second error occurs on a separate thread in between">case that a second error occurs on a separate thread in between</param>
///<param name="the time of the first error and the call to these interfaces.">the time of the first error and the call to these interfaces.</param>
///<param name="When that happens, the second error will be reported since these">When that happens, the second error will be reported since these</param>
///<param name="interfaces always report the most recent result.  To avoid">interfaces always report the most recent result.  To avoid</param>
///<param name="this, each thread can obtain exclusive use of the [database connection] D">this, each thread can obtain exclusive use of the [database connection] D</param>
///<param name="by invoking [sqlite3_mutex_enter]([sqlite3_db_mutex](D)) before beginning">by invoking [sqlite3_mutex_enter]([sqlite3_db_mutex](D)) before beginning</param>
///<param name="to use D and invoking [sqlite3_mutex_leave]([sqlite3_db_mutex](D)) after">to use D and invoking [sqlite3_mutex_leave]([sqlite3_db_mutex](D)) after</param>
///<param name="all calls to the interfaces listed here are completed.">all calls to the interfaces listed here are completed.</param>
///<param name=""></param>
///<param name="If an interface fails with SQLITE_MISUSE, that means the interface">If an interface fails with SQLITE_MISUSE, that means the interface</param>
///<param name="was invoked incorrectly by the application.  In that case, the">was invoked incorrectly by the application.  In that case, the</param>
///<param name="error code and message may or may not be set.">error code and message may or may not be set.</param>
///<param name=""></param>

		//SQLITE_API int sqlite3_errcode(sqlite3 db);
		//SQLITE_API int sqlite3_extended_errcode(sqlite3 db);
		//SQLITE_API string sqlite3_errmsg(sqlite3);
		//SQLITE_API const void *sqlite3_errmsg16(sqlite3);
		///
///<summary>
///CAPI3REF: SQL Statement Object
///KEYWORDS: {prepared statement} {prepared statements}
///
///An instance of this object represents a single SQL statement.
///This object is variously known as a "prepared statement" or a
///"compiled SQL statement" or simply as a "statement".
///
///The life of a statement object goes something like this:
///
///<ol>
///<li> Create the object using [sqlite3_prepare_v2()] or a related
///function.
///<li> Bind values to [host parameters] using the sqlite3_bind_*()
///interfaces.
///<li> Run the SQL by calling [sqlite3_step()] one or more times.
///<li> Reset the statement using [sqlite3_reset()] then go back
///to step 2.  Do this zero or more times.
///<li> Destroy the object using [sqlite3_finalize()].
///</ol>
///
///Refer to documentation on individual methods above for additional
///information.
///
///</summary>

		//typedef struct sqlite3_stmt sqlite3_stmt;
		///
///<summary>
///</summary>
///<param name="CAPI3REF: Run">time Limits</param>
///<param name=""></param>
///<param name="^(This interface allows the size of various constructs to be limited">^(This interface allows the size of various constructs to be limited</param>
///<param name="on a connection by connection basis.  The first parameter is the">on a connection by connection basis.  The first parameter is the</param>
///<param name="[database connection] whose limit is to be set or queried.  The">[database connection] whose limit is to be set or queried.  The</param>
///<param name="second parameter is one of the [limit categories] that define a">second parameter is one of the [limit categories] that define a</param>
///<param name="class of constructs to be size limited.  The third parameter is the">class of constructs to be size limited.  The third parameter is the</param>
///<param name="new limit for that construct.)^">new limit for that construct.)^</param>
///<param name=""></param>
///<param name="^If the new limit is a negative number, the limit is unchanged.">^If the new limit is a negative number, the limit is unchanged.</param>
///<param name="^(For each limit category SQLITE_LIMIT_<i>NAME</i> there is a ">^(For each limit category SQLITE_LIMIT_<i>NAME</i> there is a </param>
///<param name="[limits | hard upper bound]">[limits | hard upper bound]</param>
///<param name="set at compile">time by a C preprocessor macro called</param>
///<param name="[limits | SQLITE_MAX_<i>NAME</i>].">[limits | SQLITE_MAX_<i>NAME</i>].</param>
///<param name="(The "_LIMIT_" in the name is changed to "_MAX_".))^">(The "_LIMIT_" in the name is changed to "_MAX_".))^</param>
///<param name="^Attempts to increase a limit above its hard upper bound are">^Attempts to increase a limit above its hard upper bound are</param>
///<param name="silently truncated to the hard upper bound.">silently truncated to the hard upper bound.</param>
///<param name=""></param>
///<param name="^Regardless of whether or not the limit was changed, the ">^Regardless of whether or not the limit was changed, the </param>
///<param name="[sqlite3_limit()] interface returns the prior value of the limit.">[sqlite3_limit()] interface returns the prior value of the limit.</param>
///<param name="^Hence, to find the current value of a limit without changing it,">^Hence, to find the current value of a limit without changing it,</param>
///<param name="simply invoke this interface with the third parameter set to ">1.</param>
///<param name=""></param>
///<param name="Run">time limits are intended for use in applications that manage</param>
///<param name="both their own internal database and also databases that are controlled">both their own internal database and also databases that are controlled</param>
///<param name="by untrusted external sources.  An example application might be a">by untrusted external sources.  An example application might be a</param>
///<param name="web browser that has its own databases for storing history and">web browser that has its own databases for storing history and</param>
///<param name="separate databases controlled by JavaScript applications downloaded">separate databases controlled by JavaScript applications downloaded</param>
///<param name="off the Internet.  The internal databases can be given the">off the Internet.  The internal databases can be given the</param>
///<param name="large, default limits.  Databases managed by external sources can">large, default limits.  Databases managed by external sources can</param>
///<param name="be given much smaller limits designed to prevent a denial of service">be given much smaller limits designed to prevent a denial of service</param>
///<param name="attack.  Developers might also want to use the [sqlite3_set_authorizer()]">attack.  Developers might also want to use the [sqlite3_set_authorizer()]</param>
///<param name="interface to further control untrusted SQL.  The size of the database">interface to further control untrusted SQL.  The size of the database</param>
///<param name="created by an untrusted script can be contained using the">created by an untrusted script can be contained using the</param>
///<param name="[max_page_count] [PRAGMA].">[max_page_count] [PRAGMA].</param>
///<param name=""></param>
///<param name="New run">time limit categories may be added in future releases.</param>
///<param name=""></param>

		
		///
///<summary>
///CAPI3REF: Compiling An SQL Statement
///KEYWORDS: {SQL statement compiler}
///
///</summary>
///<param name="To execute an SQL query, it must first be compiled into a byte">code</param>
///<param name="program using one of these routines.">program using one of these routines.</param>
///<param name=""></param>
///<param name="The first argument, "db", is a [database connection] obtained from a">The first argument, "db", is a [database connection] obtained from a</param>
///<param name="prior successful call to [sqlite3_open()], [sqlite3_open_v2()] or">prior successful call to [sqlite3_open()], [sqlite3_open_v2()] or</param>
///<param name="[sqlite3_open16()].  The database connection must not have been closed.">[sqlite3_open16()].  The database connection must not have been closed.</param>
///<param name=""></param>
///<param name="The second argument, "zSql", is the statement to be compiled, encoded">The second argument, "zSql", is the statement to be compiled, encoded</param>
///<param name="as either UTF">16.  The sqlite3_prepare() and sqlite3_prepare_v2()</param>
///<param name="interfaces use UTF">8, and sqlite3_prepare16() and sqlite3_prepare16_v2()</param>
///<param name="use UTF">16.</param>
///<param name=""></param>
///<param name="^If the nByte argument is less than zero, then zSql is read up to the">^If the nByte argument is less than zero, then zSql is read up to the</param>
///<param name="first zero terminator. ^If nByte is non">negative, then it is the maximum</param>
///<param name="number of  bytes read from zSql.  ^When nByte is non">negative, the</param>
///<param name="zSql string ends at either the first '\000' or '\u0000' character or">zSql string ends at either the first '\000' or '\u0000' character or</param>
///<param name="the nByte">th byte, whichever comes first. If the caller knows</param>
///<param name="that the supplied string is nul">terminated, then there is a small</param>
///<param name="performance advantage to be gained by passing an nByte parameter that">performance advantage to be gained by passing an nByte parameter that</param>
///<param name="is equal to the number of bytes in the input string <i>including</i>">is equal to the number of bytes in the input string <i>including</i></param>
///<param name="the nul">terminator bytes.</param>
///<param name=""></param>
///<param name="^If pzTail is not NULL then *pzTail is made to point to the first byte">^If pzTail is not NULL then *pzTail is made to point to the first byte</param>
///<param name="past the end of the first SQL statement in zSql.  These routines only">past the end of the first SQL statement in zSql.  These routines only</param>
///<param name="compile the first statement in zSql, so *pzTail is left pointing to">compile the first statement in zSql, so *pzTail is left pointing to</param>
///<param name="what remains uncompiled.">what remains uncompiled.</param>
///<param name=""></param>
///<param name="^*ppStmt is left pointing to a compiled [prepared statement] that can be">^*ppStmt is left pointing to a compiled [prepared statement] that can be</param>
///<param name="executed using [sqlite3_step()].  ^If there is an error, *ppStmt is set">executed using [sqlite3_step()].  ^If there is an error, *ppStmt is set</param>
///<param name="to NULL.  ^If the input text contains no SQL (if the input is an empty">to NULL.  ^If the input text contains no SQL (if the input is an empty</param>
///<param name="string or a comment) then *ppStmt is set to NULL.">string or a comment) then *ppStmt is set to NULL.</param>
///<param name="The calling procedure is responsible for deleting the compiled">The calling procedure is responsible for deleting the compiled</param>
///<param name="SQL statement using [sqlite3_finalize()] after it has finished with it.">SQL statement using [sqlite3_finalize()] after it has finished with it.</param>
///<param name="ppStmt may not be NULL.">ppStmt may not be NULL.</param>
///<param name=""></param>
///<param name="^On success, the sqlite3_prepare() family of routines return [SqlResult.SQLITE_OK];">^On success, the sqlite3_prepare() family of routines return [SqlResult.SQLITE_OK];</param>
///<param name="otherwise an [error code] is returned.">otherwise an [error code] is returned.</param>
///<param name=""></param>
///<param name="The sqlite3_prepare_v2() and sqlite3_prepare16_v2() interfaces are">The sqlite3_prepare_v2() and sqlite3_prepare16_v2() interfaces are</param>
///<param name="recommended for all new programs. The two older interfaces are retained">recommended for all new programs. The two older interfaces are retained</param>
///<param name="for backwards compatibility, but their use is discouraged.">for backwards compatibility, but their use is discouraged.</param>
///<param name="^In the "v2" interfaces, the prepared statement">^In the "v2" interfaces, the prepared statement</param>
///<param name="that is returned (the [sqlite3_stmt] object) contains a copy of the">that is returned (the [sqlite3_stmt] object) contains a copy of the</param>
///<param name="original SQL text. This causes the [sqlite3_step()] interface to">original SQL text. This causes the [sqlite3_step()] interface to</param>
///<param name="behave differently in three ways:">behave differently in three ways:</param>
///<param name=""></param>
///<param name="<ol>"><ol></param>
///<param name="<li>"><li></param>
///<param name="^If the database schema changes, instead of returning [SQLITE_SCHEMA] as it">^If the database schema changes, instead of returning [SQLITE_SCHEMA] as it</param>
///<param name="always used to do, [sqlite3_step()] will automatically recompile the SQL">always used to do, [sqlite3_step()] will automatically recompile the SQL</param>
///<param name="statement and try to run it again.">statement and try to run it again.</param>
///<param name="</li>"></li></param>
///<param name=""></param>
///<param name="<li>"><li></param>
///<param name="^When an error occurs, [sqlite3_step()] will return one of the detailed">^When an error occurs, [sqlite3_step()] will return one of the detailed</param>
///<param name="[error codes] or [extended error codes].  ^The legacy behavior was that">[error codes] or [extended error codes].  ^The legacy behavior was that</param>
///<param name="[sqlite3_step()] would only return a generic [SqlResult.SQLITE_ERROR] result code">[sqlite3_step()] would only return a generic [SqlResult.SQLITE_ERROR] result code</param>
///<param name="and the application would have to make a second call to [sqlite3_reset()]">and the application would have to make a second call to [sqlite3_reset()]</param>
///<param name="in order to find the underlying cause of the problem. With the "v2" prepare">in order to find the underlying cause of the problem. With the "v2" prepare</param>
///<param name="interfaces, the underlying reason for the error is returned immediately.">interfaces, the underlying reason for the error is returned immediately.</param>
///<param name="</li>"></li></param>
///<param name=""></param>
///<param name="<li>"><li></param>
///<param name="^If the specific value bound to [parameter | host parameter] in the ">^If the specific value bound to [parameter | host parameter] in the </param>
///<param name="WHERE clause might influence the choice of query plan for a statement,">WHERE clause might influence the choice of query plan for a statement,</param>
///<param name="then the statement will be automatically recompiled, as if there had been ">then the statement will be automatically recompiled, as if there had been </param>
///<param name="a schema change, on the first  [sqlite3_step()] call following any change">a schema change, on the first  [sqlite3_step()] call following any change</param>
///<param name="to the [sqlite3_bind_text | bindings] of that [parameter]. ">to the [sqlite3_bind_text | bindings] of that [parameter]. </param>
///<param name="^The specific value of WHERE">clause [parameter] might influence the </param>
///<param name="choice of query plan if the parameter is the left">hand side of a [LIKE]</param>
///<param name="or [GLOB] operator or if the parameter is compared to an indexed column">or [GLOB] operator or if the parameter is compared to an indexed column</param>
///<param name="and the [SQLITE_ENABLE_STAT2] compile">time option is enabled.</param>
///<param name="the ">the </param>
///<param name="</li>"></li></param>
///<param name="</ol>"></ol></param>
///<param name=""></param>

		//SQLITE_API int sqlite3_prepare(
		//  sqlite3 db,            /* Database handle */
		//  string zSql,       /* SQL statement, UTF-8 encoded */
		//  int nByte,              /* Maximum length of zSql in bytes. */
		//  sqlite3_stmt **ppStmt,  /* OUT: Statement handle */
		//  string *pzTail     /* OUT: Pointer to unused portion of zSql */
		//);
		//SQLITE_API int sqlite3_prepare_v2(
		//  sqlite3 db,            /* Database handle */
		//  string zSql,       /* SQL statement, UTF-8 encoded */
		//  int nByte,              /* Maximum length of zSql in bytes. */
		//  sqlite3_stmt **ppStmt,  /* OUT: Statement handle */
		//  string *pzTail     /* OUT: Pointer to unused portion of zSql */
		//);
		//SQLITE_API int sqlite3_prepare16(
		//  sqlite3 db,            /* Database handle */
		//  string zSql,       /* SQL statement, UTF-16 encoded */
		//  int nByte,              /* Maximum length of zSql in bytes. */
		//  sqlite3_stmt **ppStmt,  /* OUT: Statement handle */
		//  const void **pzTail     /* OUT: Pointer to unused portion of zSql */
		//);
		//SQLITE_API int sqlite3_prepare16_v2(
		//  sqlite3 db,            /* Database handle */
		//  string zSql,       /* SQL statement, UTF-16 encoded */
		//  int nByte,              /* Maximum length of zSql in bytes. */
		//  sqlite3_stmt **ppStmt,  /* OUT: Statement handle */
		//  const void **pzTail     /* OUT: Pointer to unused portion of zSql */
		//);
		///
///<summary>
///CAPI3REF: Retrieving Statement SQL
///
///^This interface can be used to retrieve a saved copy of the original
///SQL text used to create a [prepared statement] if that statement was
///compiled using either [sqlite3_prepare_v2()] or [sqlite3_prepare16_v2()].
///
///</summary>

		//SQLITE_API string sqlite3_sql(sqlite3_stmt *pStmt);
		///
///<summary>
///CAPI3REF: Determine If An SQL Statement Writes The Database
///
///</summary>
///<param name="^The sqlite3_stmt_readonly(X) interface returns true (non">zero) if </param>
///<param name="and only if the [prepared statement] X makes no direct changes to">and only if the [prepared statement] X makes no direct changes to</param>
///<param name="the content of the database file.">the content of the database file.</param>
///<param name=""></param>
///<param name="Note that [application">defined SQL functions] or</param>
///<param name="[virtual tables] might change the database indirectly as a side effect.  ">[virtual tables] might change the database indirectly as a side effect.  </param>
///<param name="^(For example, if an application defines a function "eval()" that ">^(For example, if an application defines a function "eval()" that </param>
///<param name="calls [sqlite3_exec()], then the following SQL statement would">calls [sqlite3_exec()], then the following SQL statement would</param>
///<param name="change the database file through side">effects:</param>
///<param name=""></param>
///<param name="<blockquote><pre>"><blockquote><pre></param>
///<param name="SELECT eval('DELETE FROM t1') FROM t2;">SELECT eval('DELETE FROM t1') FROM t2;</param>
///<param name="</pre></blockquote>"></pre></blockquote></param>
///<param name=""></param>
///<param name="But because the [SELECT] statement does not change the database file">But because the [SELECT] statement does not change the database file</param>
///<param name="directly, sqlite3_stmt_readonly() would still return true.)^">directly, sqlite3_stmt_readonly() would still return true.)^</param>
///<param name=""></param>
///<param name="^Transaction control statements such as [BEGIN], [COMMIT], [ROLLBACK],">^Transaction control statements such as [BEGIN], [COMMIT], [ROLLBACK],</param>
///<param name="[SAVEPOINT], and [RELEASE] cause sqlite3_stmt_readonly() to return true,">[SAVEPOINT], and [RELEASE] cause sqlite3_stmt_readonly() to return true,</param>
///<param name="since the statements themselves do not actually modify the database but">since the statements themselves do not actually modify the database but</param>
///<param name="rather they control the timing of when other statements modify the ">rather they control the timing of when other statements modify the </param>
///<param name="database.  ^The [ATTACH] and [DETACH] statements also cause">database.  ^The [ATTACH] and [DETACH] statements also cause</param>
///<param name="sqlite3_stmt_readonly() to return true since, while those statements">sqlite3_stmt_readonly() to return true since, while those statements</param>
///<param name="change the configuration of a database connection, they do not make ">change the configuration of a database connection, they do not make </param>
///<param name="changes to the content of the database files on disk.">changes to the content of the database files on disk.</param>
///<param name=""></param>

		//SQLITE_API int sqlite3_stmt_readonly(sqlite3_stmt *pStmt);
		///
///<summary>
///CAPI3REF: Dynamically Typed Value Object
///KEYWORDS: {protected sqlite3_value} {unprotected sqlite3_value}
///
///SQLite uses the sqlite3_value object to represent all values
///that can be stored in a database table. SQLite uses dynamic typing
///for the values it stores.  ^Values stored in sqlite3_value objects
///can be integers, floating point values, strings, BLOBs, or NULL.
///
///An sqlite3_value object may be either "protected" or "unprotected".
///Some interfaces require a protected sqlite3_value.  Other interfaces
///will accept either a protected or an unprotected sqlite3_value.
///Every interface that accepts sqlite3_value arguments specifies
///whether or not it requires a protected sqlite3_value.
///
///The terms "protected" and "unprotected" refer to whether or not
///a mutex is held.  An internal mutex is held for a protected
///sqlite3_value object but no mutex is held for an unprotected
///</summary>
///<param name="sqlite3_value object.  If SQLite is compiled to be single">threaded</param>
///<param name="(with [SQLITE_THREADSAFE=0] and with [sqlite3_threadsafe()] returning 0)">(with [SQLITE_THREADSAFE=0] and with [sqlite3_threadsafe()] returning 0)</param>
///<param name="or if SQLite is run in one of reduced mutex modes ">or if SQLite is run in one of reduced mutex modes </param>
///<param name="[SQLITE_CONFIG_SINGLETHREAD] or [SQLITE_CONFIG_MULTITHREAD]">[SQLITE_CONFIG_SINGLETHREAD] or [SQLITE_CONFIG_MULTITHREAD]</param>
///<param name="then there is no distinction between protected and unprotected">then there is no distinction between protected and unprotected</param>
///<param name="sqlite3_value objects and they can be used interchangeably.  However,">sqlite3_value objects and they can be used interchangeably.  However,</param>
///<param name="for maximum code portability it is recommended that applications">for maximum code portability it is recommended that applications</param>
///<param name="still make the distinction between protected and unprotected">still make the distinction between protected and unprotected</param>
///<param name="sqlite3_value objects even when not strictly required.">sqlite3_value objects even when not strictly required.</param>
///<param name=""></param>
///<param name="^The sqlite3_value objects that are passed as parameters into the">^The sqlite3_value objects that are passed as parameters into the</param>
///<param name="implementation of [application">defined SQL functions] are protected.</param>
///<param name="^The sqlite3_value object returned by">^The sqlite3_value object returned by</param>
///<param name="[sqlite3_column_value()] is unprotected.">[sqlite3_column_value()] is unprotected.</param>
///<param name="Unprotected sqlite3_value objects may only be used with">Unprotected sqlite3_value objects may only be used with</param>
///<param name="[sqlite3_result_value()] and [sqlite3_bind_value()].">[sqlite3_result_value()] and [sqlite3_bind_value()].</param>
///<param name="The [sqlite3_value_blob | vdbeapi.sqlite3_value_type()] family of">The [sqlite3_value_blob | vdbeapi.sqlite3_value_type()] family of</param>
///<param name="interfaces require protected sqlite3_value objects.">interfaces require protected sqlite3_value objects.</param>
///<param name=""></param>

		//typedef struct Mem sqlite3_value;
		///
///<summary>
///CAPI3REF: SQL Function Context Object
///
///The context in which an SQL function executes is stored in an
///sqlite3_context object.  ^A pointer to an sqlite3_context object
///</summary>
///<param name="is always first parameter to [application">defined SQL functions].</param>
///<param name="The application">defined SQL function implementation will pass this</param>
///<param name="pointer through into calls to [sqlite3_result_int | sqlite3_result()],">pointer through into calls to [sqlite3_result_int | sqlite3_result()],</param>
///<param name="[vdbeapi.sqlite3_aggregate_context()], [sqlite3_user_data()],">[vdbeapi.sqlite3_aggregate_context()], [sqlite3_user_data()],</param>
///<param name="[vdbeapi.sqlite3_context_db_handle()], [sqlite3_get_auxdata()],">[vdbeapi.sqlite3_context_db_handle()], [sqlite3_get_auxdata()],</param>
///<param name="and/or [sqlite3_set_auxdata()].">and/or [sqlite3_set_auxdata()].</param>
///<param name=""></param>

		//typedef struct sqlite3_context sqlite3_context;
		///
///<summary>
///CAPI3REF: Binding Values To Prepared Statements
///KEYWORDS: {host parameter} {host parameters} {host parameter name}
///KEYWORDS: {SQL parameter} {SQL parameters} {parameter binding}
///
///^(In the SQL statement text input to [sqlite3_prepare_v2()] and its variants,
///literals may be replaced by a [parameter] that matches one of following
///templates:
///
///<ul>
///<li>  ?
///<li>  ?NNN
///<li>  :VVV
///<li>  @VVV
///<li>  $VVV
///</ul>
///
///In the templates above, NNN represents an integer literal,
///and VVV represents an alphanumeric identifier.)^  ^The values of these
///parameters (also called "host parameter names" or "SQL parameters")
///can be set using the sqlite3_bind_*() routines defined here.
///
///^The first argument to the sqlite3_bind_*() routines is always
///a pointer to the [sqlite3_stmt] object returned from
///[sqlite3_prepare_v2()] or its variants.
///
///^The second argument is the index of the SQL parameter to be set.
///^The leftmost SQL parameter has an index of 1.  ^When the same named
///SQL parameter is used more than once, second and subsequent
///occurrences have the same index as the first occurrence.
///^The index for named parameters can be looked up using the
///[sqlite3_bind_parameter_index()] API if desired.  ^The index
///for "?NNN" parameters is the value of NNN.
///^The NNN value must be between 1 and the [sqlite3_limit()]
///parameter [SQLITE_LIMIT_VARIABLE_NUMBER] (default value: 999).
///
///^The third argument is the value to bind to the parameter.
///
///^(In those routines that have a fourth argument, its value is the
///number of bytes in the parameter.  To be clear: the value is the
///number of <u>bytes</u> in the value, not the number of characters.)^
///^If the fourth parameter is negative, the length of the string is
///the number of bytes up to the first zero terminator.
///
///^The fifth argument to sqlite3_bind_blob(), sqlite3_bind_text(), and
///sqlite3_bind_text16() is a destructor used to dispose of the BLOB or
///string after SQLite has finished with it.  ^The destructor is called
///to dispose of the BLOB or string even if the call to sqlite3_bind_blob(),
///sqlite3_bind_text(), or sqlite3_bind_text16() fails.  
///^If the fifth argument is
///the special value [SQLITE_STATIC], then SQLite assumes that the
///information is in static, unmanaged space and does not need to be freed.
///^If the fifth argument has the value [SQLITE_TRANSIENT], then
///SQLite makes its own public copy of the data immediately, before
///the sqlite3_bind_*() routine returns.
///
///^The sqlite3_bind_zeroblob() routine binds a BLOB of length N that
///is filled with zeroes.  ^A zeroblob uses a fixed amount of memory
///(just an integer to hold its size) while it is being processed.
///Zeroblobs are intended to serve as placeholders for BLOBs whose
///content is later written using
///[sqlite3_blob_open | incremental BLOB I/O] routines.
///</summary>
///<param name="^A negative value for the zeroblob results in a zero">length BLOB.</param>
///<param name=""></param>
///<param name="^If any of the sqlite3_bind_*() routines are called with a NULL pointer">^If any of the sqlite3_bind_*() routines are called with a NULL pointer</param>
///<param name="for the [prepared statement] or with a prepared statement for which">for the [prepared statement] or with a prepared statement for which</param>
///<param name="[sqlite3_step()] has been called more recently than [sqlite3_reset()],">[sqlite3_step()] has been called more recently than [sqlite3_reset()],</param>
///<param name="then the call will return [SQLITE_MISUSE].  If any sqlite3_bind_()">then the call will return [SQLITE_MISUSE].  If any sqlite3_bind_()</param>
///<param name="routine is passed a [prepared statement] that has been finalized, the">routine is passed a [prepared statement] that has been finalized, the</param>
///<param name="result is undefined and probably harmful.">result is undefined and probably harmful.</param>
///<param name=""></param>
///<param name="^Bindings are not cleared by the [sqlite3_reset()] routine.">^Bindings are not cleared by the [sqlite3_reset()] routine.</param>
///<param name="^Unbound parameters are interpreted as NULL.">^Unbound parameters are interpreted as NULL.</param>
///<param name=""></param>
///<param name="^The sqlite3_bind_* routines return [SqlResult.SQLITE_OK] on success or an">^The sqlite3_bind_* routines return [SqlResult.SQLITE_OK] on success or an</param>
///<param name="[error code] if anything goes wrong.">[error code] if anything goes wrong.</param>
///<param name="^[SQLITE_RANGE] is returned if the parameter">^[SQLITE_RANGE] is returned if the parameter</param>
///<param name="index is out of range.  ^[SQLITE_NOMEM] is returned if malloc() fails.">index is out of range.  ^[SQLITE_NOMEM] is returned if malloc() fails.</param>
///<param name=""></param>
///<param name="See also: [sqlite3_bind_parameter_count()],">See also: [sqlite3_bind_parameter_count()],</param>
///<param name="[sqlite3_bind_parameter_name()], and [sqlite3_bind_parameter_index()].">[sqlite3_bind_parameter_name()], and [sqlite3_bind_parameter_index()].</param>
///<param name=""></param>

		//SQLITE_API int sqlite3_bind_blob(sqlite3_stmt*, int, const void*, int n, void()(void));
		//SQLITE_API int sqlite3_bind_double(sqlite3_stmt*, int, double);
		//SQLITE_API int sqlite3_bind_int(sqlite3_stmt*, int, int);
		//SQLITE_API int sqlite3_bind_int64(sqlite3_stmt*, int, sqlite3_int64);
		//SQLITE_API int sqlite3_bind_null(sqlite3_stmt*, int);
		//SQLITE_API int sqlite3_bind_text(sqlite3_stmt*, int, const char*, int n, void()(void));
		//SQLITE_API int sqlite3_bind_text16(sqlite3_stmt*, int, const void*, int, void()(void));
		//SQLITE_API int sqlite3_bind_value(sqlite3_stmt*, int, const sqlite3_value);
		//SQLITE_API int sqlite3_bind_zeroblob(sqlite3_stmt*, int, int n);
		///
///<summary>
///CAPI3REF: Number Of SQL Parameters
///
///^This routine can be used to find the number of [SQL parameters]
///in a [prepared statement].  SQL parameters are tokens of the
///form "?", "?NNN", ":AAA", "$AAA", or "@AAA" that serve as
///placeholders for values that are [sqlite3_bind_blob | bound]
///to the parameters at a later time.
///
///^(This routine actually returns the index of the largest (rightmost)
///parameter. For all forms except ?NNN, this will correspond to the
///number of unique parameters.  If parameters of the ?NNN form are used,
///there may be gaps in the list.)^
///
///See also: [sqlite3_bind_blob|sqlite3_bind()],
///[sqlite3_bind_parameter_name()], and
///[sqlite3_bind_parameter_index()].
///
///</summary>

		//SQLITE_API int sqlite3_bind_parameter_count(sqlite3_stmt);
		///
///<summary>
///CAPI3REF: Name Of A Host Parameter
///
///^The sqlite3_bind_parameter_name(P,N) interface returns
///</summary>
///<param name="the name of the N">th [SQL parameter] in the [prepared statement] P.</param>
///<param name="^(SQL parameters of the form "?NNN" or ":AAA" or "@AAA" or "$AAA"">^(SQL parameters of the form "?NNN" or ":AAA" or "@AAA" or "$AAA"</param>
///<param name="have a name which is the string "?NNN" or ":AAA" or "@AAA" or "$AAA"">have a name which is the string "?NNN" or ":AAA" or "@AAA" or "$AAA"</param>
///<param name="respectively.">respectively.</param>
///<param name="In other words, the initial ":" or "$" or "@" or "?"">In other words, the initial ":" or "$" or "@" or "?"</param>
///<param name="is included as part of the name.)^">is included as part of the name.)^</param>
///<param name="^Parameters of the form "?" without a following integer have no name">^Parameters of the form "?" without a following integer have no name</param>
///<param name="and are referred to as "nameless" or "anonymous parameters".">and are referred to as "nameless" or "anonymous parameters".</param>
///<param name=""></param>
///<param name="^The first host parameter has an index of 1, not 0.">^The first host parameter has an index of 1, not 0.</param>
///<param name=""></param>
///<param name="^If the value N is out of range or if the N">th parameter is</param>
///<param name="nameless, then NULL is returned.  ^The returned string is">nameless, then NULL is returned.  ^The returned string is</param>
///<param name="always in UTF">8 encoding even if the named parameter was</param>
///<param name="originally specified as UTF">16 in [sqlite3_prepare16()] or</param>
///<param name="[sqlite3_prepare16_v2()].">[sqlite3_prepare16_v2()].</param>
///<param name=""></param>
///<param name="See also: [sqlite3_bind_blob|sqlite3_bind()],">See also: [sqlite3_bind_blob|sqlite3_bind()],</param>
///<param name="[sqlite3_bind_parameter_count()], and">[sqlite3_bind_parameter_count()], and</param>
///<param name="[sqlite3_bind_parameter_index()].">[sqlite3_bind_parameter_index()].</param>
///<param name=""></param>

		//SQLITE_API string sqlite3_bind_parameter_name(sqlite3_stmt*, int);
		///
///<summary>
///CAPI3REF: Index Of A Parameter With A Given Name
///
///^Return the index of an SQL parameter given its name.  ^The
///index value returned is suitable for use as the second
///parameter to [sqlite3_bind_blob|sqlite3_bind()].  ^A zero
///is returned if no matching parameter is found.  ^The parameter
///</summary>
///<param name="name must be given in UTF">8 even if the original statement</param>
///<param name="was prepared from UTF">16 text using [sqlite3_prepare16_v2()].</param>
///<param name=""></param>
///<param name="See also: [sqlite3_bind_blob|sqlite3_bind()],">See also: [sqlite3_bind_blob|sqlite3_bind()],</param>
///<param name="[sqlite3_bind_parameter_count()], and">[sqlite3_bind_parameter_count()], and</param>
///<param name="[sqlite3_bind_parameter_index()].">[sqlite3_bind_parameter_index()].</param>
///<param name=""></param>

		//SQLITE_API int sqlite3_bind_parameter_index(sqlite3_stmt*, string zName);
		///
///<summary>
///CAPI3REF: Reset All Bindings On A Prepared Statement
///
///^Contrary to the intuition of many, [sqlite3_reset()] does not reset
///the [sqlite3_bind_blob | bindings] on a [prepared statement].
///^Use this routine to reset all host parameters to NULL.
///
///</summary>

		//SQLITE_API int sqlite3_clear_bindings(sqlite3_stmt);
		///
///<summary>
///CAPI3REF: Number Of Columns In A Result Set
///
///^Return the number of columns in the result set returned by the
///[prepared statement]. ^This routine returns 0 if pStmt is an SQL
///statement that does not return data (for example an [UPDATE]).
///
///See also: [sqlite3_data_count()]
///
///</summary>

		//SQLITE_API int vdbeapi.sqlite3_column_count(sqlite3_stmt *pStmt);
		///
///<summary>
///CAPI3REF: Column Names In A Result Set
///
///^These routines return the name assigned to a particular column
///in the result set of a [SELECT] statement.  ^The vdbeapi.sqlite3_column_name()
///</summary>
///<param name="interface returns a pointer to a zero">8 string</param>
///<param name="and vdbeapi.sqlite3_column_name16() returns a pointer to a zero">terminated</param>
///<param name="UTF">16 string.  ^The first parameter is the [prepared statement]</param>
///<param name="that implements the [SELECT] statement. ^The second parameter is the">that implements the [SELECT] statement. ^The second parameter is the</param>
///<param name="column number.  ^The leftmost column is number 0.">column number.  ^The leftmost column is number 0.</param>
///<param name=""></param>
///<param name="^The returned string pointer is valid until either the [prepared statement]">^The returned string pointer is valid until either the [prepared statement]</param>
///<param name="is destroyed by [sqlite3_finalize()] or until the statement is automatically">is destroyed by [sqlite3_finalize()] or until the statement is automatically</param>
///<param name="reprepared by the first call to [sqlite3_step()] for a particular run">reprepared by the first call to [sqlite3_step()] for a particular run</param>
///<param name="or until the next call to">or until the next call to</param>
///<param name="vdbeapi.sqlite3_column_name() or vdbeapi.sqlite3_column_name16() on the same column.">vdbeapi.sqlite3_column_name() or vdbeapi.sqlite3_column_name16() on the same column.</param>
///<param name=""></param>
///<param name="^If sqlite3_malloc() fails during the processing of either routine">^If sqlite3_malloc() fails during the processing of either routine</param>
///<param name="(for example during a conversion from UTF">16) then a</param>
///<param name="NULL pointer is returned.">NULL pointer is returned.</param>
///<param name=""></param>
///<param name="^The name of a result column is the value of the "AS" clause for">^The name of a result column is the value of the "AS" clause for</param>
///<param name="that column, if there is an AS clause.  If there is no AS clause">that column, if there is an AS clause.  If there is no AS clause</param>
///<param name="then the name of the column is unspecified and may change from">then the name of the column is unspecified and may change from</param>
///<param name="one release of SQLite to the next.">one release of SQLite to the next.</param>
///<param name=""></param>

		//SQLITE_API string vdbeapi.sqlite3_column_name(sqlite3_stmt*, int N);
		//SQLITE_API const void *vdbeapi.sqlite3_column_name16(sqlite3_stmt*, int N);
		///
///<summary>
///CAPI3REF: Source Of Data In A Query Result
///
///^These routines provide a means to determine the database, table, and
///table column that is the origin of a particular result column in
///[SELECT] statement.
///^The name of the database or table or column can be returned as
///</summary>
///<param name="either a UTF">16 string.  ^The _database_ routines return</param>
///<param name="the database name, the _table_ routines return the table name, and">the database name, the _table_ routines return the table name, and</param>
///<param name="the origin_ routines return the column name.">the origin_ routines return the column name.</param>
///<param name="^The returned string is valid until the [prepared statement] is destroyed">^The returned string is valid until the [prepared statement] is destroyed</param>
///<param name="using [sqlite3_finalize()] or until the statement is automatically">using [sqlite3_finalize()] or until the statement is automatically</param>
///<param name="reprepared by the first call to [sqlite3_step()] for a particular run">reprepared by the first call to [sqlite3_step()] for a particular run</param>
///<param name="or until the same information is requested">or until the same information is requested</param>
///<param name="again in a different encoding.">again in a different encoding.</param>
///<param name=""></param>
///<param name="^The names returned are the original un">aliased names of the</param>
///<param name="database, table, and column.">database, table, and column.</param>
///<param name=""></param>
///<param name="^The first argument to these interfaces is a [prepared statement].">^The first argument to these interfaces is a [prepared statement].</param>
///<param name="^These functions return information about the Nth result column returned by">^These functions return information about the Nth result column returned by</param>
///<param name="the statement, where N is the second function argument.">the statement, where N is the second function argument.</param>
///<param name="^The left">most column is column 0 for these routines.</param>
///<param name=""></param>
///<param name="^If the Nth column returned by the statement is an expression or">^If the Nth column returned by the statement is an expression or</param>
///<param name="subquery and is not a column value, then all of these functions return">subquery and is not a column value, then all of these functions return</param>
///<param name="NULL.  ^These routine might also return NULL if a memory allocation error">NULL.  ^These routine might also return NULL if a memory allocation error</param>
///<param name="occurs.  ^Otherwise, they return the name of the attached database, table,">occurs.  ^Otherwise, they return the name of the attached database, table,</param>
///<param name="or column that query result column was extracted from.">or column that query result column was extracted from.</param>
///<param name=""></param>
///<param name="^As with all other SQLite APIs, those whose names end with "16" return">^As with all other SQLite APIs, those whose names end with "16" return</param>
///<param name="UTF">8.</param>
///<param name=""></param>
///<param name="^These APIs are only available if the library was compiled with the">^These APIs are only available if the library was compiled with the</param>
///<param name="[SQLITE_ENABLE_COLUMN_METADATA] C">preprocessor symbol.</param>
///<param name=""></param>
///<param name="If two or more threads call one or more of these routines against the same">If two or more threads call one or more of these routines against the same</param>
///<param name="prepared statement and column at the same time then the results are">prepared statement and column at the same time then the results are</param>
///<param name="undefined.">undefined.</param>
///<param name=""></param>
///<param name="If two or more threads call one or more">If two or more threads call one or more</param>
///<param name="[sqlite3_column_database_name | column metadata interfaces]">[sqlite3_column_database_name | column metadata interfaces]</param>
///<param name="for the same [prepared statement] and result column">for the same [prepared statement] and result column</param>
///<param name="at the same time then the results are undefined.">at the same time then the results are undefined.</param>
///<param name=""></param>

		//SQLITE_API string sqlite3_column_database_name(sqlite3_stmt*,int);
		//SQLITE_API const void *sqlite3_column_database_name16(sqlite3_stmt*,int);
		//SQLITE_API string sqlite3_column_table_name(sqlite3_stmt*,int);
		//SQLITE_API const void *sqlite3_column_table_name16(sqlite3_stmt*,int);
		//SQLITE_API string sqlite3_column_origin_name(sqlite3_stmt*,int);
		//SQLITE_API const void *sqlite3_column_origin_name16(sqlite3_stmt*,int);
		///
///<summary>
///CAPI3REF: Declared Datatype Of A Query Result
///
///^(The first parameter is a [prepared statement].
///If this statement is a [SELECT] statement and the Nth column of the
///returned result set of that [SELECT] is a table column (not an
///expression or subquery) then the declared type of the table
///column is returned.)^  ^If the Nth column of the result set is an
///expression or subquery, then a NULL pointer is returned.
///</summary>
///<param name="^The returned string is always UTF">8 encoded.</param>
///<param name=""></param>
///<param name="^(For example, given the database schema:">^(For example, given the database schema:</param>
///<param name=""></param>
///<param name="CREATE TABLE t1(c1 VARIANT);">CREATE TABLE t1(c1 VARIANT);</param>
///<param name=""></param>
///<param name="and the following statement to be compiled:">and the following statement to be compiled:</param>
///<param name=""></param>
///<param name="SELECT c1 + 1, c1 FROM t1;">SELECT c1 + 1, c1 FROM t1;</param>
///<param name=""></param>
///<param name="this routine would return the string "VARIANT" for the second result">this routine would return the string "VARIANT" for the second result</param>
///<param name="column (i==1), and a NULL pointer for the first result column (i==0).)^">column (i==1), and a NULL pointer for the first result column (i==0).)^</param>
///<param name=""></param>
///<param name="^SQLite uses dynamic run">time typing.  ^So just because a column</param>
///<param name="is declared to contain a particular type does not mean that the">is declared to contain a particular type does not mean that the</param>
///<param name="data stored in that column is of the declared type.  SQLite is">data stored in that column is of the declared type.  SQLite is</param>
///<param name="strongly typed, but the typing is dynamic not static.  ^Type">strongly typed, but the typing is dynamic not static.  ^Type</param>
///<param name="is associated with individual values, not with the containers">is associated with individual values, not with the containers</param>
///<param name="used to hold those values.">used to hold those values.</param>
///<param name=""></param>

		//SQLITE_API string sqlite3_column_decltype(sqlite3_stmt*,int);
		//SQLITE_API const void *sqlite3_column_decltype16(sqlite3_stmt*,int);
		///
///<summary>
///CAPI3REF: Evaluate An SQL Statement
///
///After a [prepared statement] has been prepared using either
///[sqlite3_prepare_v2()] or [sqlite3_prepare16_v2()] or one of the legacy
///interfaces [sqlite3_prepare()] or [sqlite3_prepare16()], this function
///must be called one or more times to evaluate the statement.
///
///The details of the behavior of the sqlite3_step() interface depend
///on whether the statement was prepared using the newer "v2" interface
///[sqlite3_prepare_v2()] and [sqlite3_prepare16_v2()] or the older legacy
///interface [sqlite3_prepare()] and [sqlite3_prepare16()].  The use of the
///new "v2" interface is recommended for new applications but the legacy
///interface will continue to be supported.
///
///^In the legacy interface, the return value will be either [SQLITE_BUSY],
///[SQLITE_DONE], [SQLITE_ROW], [SqlResult.SQLITE_ERROR], or [SQLITE_MISUSE].
///^With the "v2" interface, any of the other [result codes] or
///[extended result codes] might be returned as well.
///
///^[SQLITE_BUSY] means that the database engine was unable to acquire the
///database locks it needs to do its job.  ^If the statement is a [COMMIT]
///or occurs outside of an explicit transaction, then you can retry the
///statement.  If the statement is not a [COMMIT] and occurs within an
///explicit transaction then you should rollback the transaction before
///continuing.
///
///^[SQLITE_DONE] means that the statement has finished executing
///successfully.  sqlite3_step() should not be called again on this virtual
///machine without first calling [sqlite3_reset()] to reset the virtual
///machine back to its initial state.
///
///^If the SQL statement being executed returns any data, then [SQLITE_ROW]
///is returned each time a new row of data is ready for processing by the
///caller. The values may be accessed using the [column access functions].
///sqlite3_step() is called again to retrieve the next row of data.
///
///</summary>
///<param name="^[SqlResult.SQLITE_ERROR] means that a run">time error (such as a constraint</param>
///<param name="violation) has occurred.  sqlite3_step() should not be called again on">violation) has occurred.  sqlite3_step() should not be called again on</param>
///<param name="the VM. More information may be found by calling [sqlite3_errmsg()].">the VM. More information may be found by calling [sqlite3_errmsg()].</param>
///<param name="^With the legacy interface, a more specific error code (for example,">^With the legacy interface, a more specific error code (for example,</param>
///<param name="[SQLITE_INTERRUPT], [SQLITE_SCHEMA], [SQLITE_CORRUPT], and so forth)">[SQLITE_INTERRUPT], [SQLITE_SCHEMA], [SQLITE_CORRUPT], and so forth)</param>
///<param name="can be obtained by calling [sqlite3_reset()] on the">can be obtained by calling [sqlite3_reset()] on the</param>
///<param name="[prepared statement].  ^In the "v2" interface,">[prepared statement].  ^In the "v2" interface,</param>
///<param name="the more specific error code is returned directly by sqlite3_step().">the more specific error code is returned directly by sqlite3_step().</param>
///<param name=""></param>
///<param name="[SQLITE_MISUSE] means that the this routine was called inappropriately.">[SQLITE_MISUSE] means that the this routine was called inappropriately.</param>
///<param name="Perhaps it was called on a [prepared statement] that has">Perhaps it was called on a [prepared statement] that has</param>
///<param name="already been [sqlite3_finalize | finalized] or on one that had">already been [sqlite3_finalize | finalized] or on one that had</param>
///<param name="previously returned [SqlResult.SQLITE_ERROR] or [SQLITE_DONE].  Or it could">previously returned [SqlResult.SQLITE_ERROR] or [SQLITE_DONE].  Or it could</param>
///<param name="be the case that the same database connection is being used by two or">be the case that the same database connection is being used by two or</param>
///<param name="more threads at the same moment in time.">more threads at the same moment in time.</param>
///<param name=""></param>
///<param name="For all versions of SQLite up to and including 3.6.23.1, a call to">For all versions of SQLite up to and including 3.6.23.1, a call to</param>
///<param name="[sqlite3_reset()] was required after sqlite3_step() returned anything">[sqlite3_reset()] was required after sqlite3_step() returned anything</param>
///<param name="other than [SQLITE_ROW] before any subsequent invocation of">other than [SQLITE_ROW] before any subsequent invocation of</param>
///<param name="sqlite3_step().  Failure to reset the prepared statement using ">sqlite3_step().  Failure to reset the prepared statement using </param>
///<param name="[sqlite3_reset()] would result in an [SQLITE_MISUSE] return from">[sqlite3_reset()] would result in an [SQLITE_MISUSE] return from</param>
///<param name="sqlite3_step().  But after version 3.6.23.1, sqlite3_step() began">sqlite3_step().  But after version 3.6.23.1, sqlite3_step() began</param>
///<param name="calling [sqlite3_reset()] automatically in this circumstance rather">calling [sqlite3_reset()] automatically in this circumstance rather</param>
///<param name="than returning [SQLITE_MISUSE].  This is not considered a compatibility">than returning [SQLITE_MISUSE].  This is not considered a compatibility</param>
///<param name="break because any application that ever receives an SQLITE_MISUSE error">break because any application that ever receives an SQLITE_MISUSE error</param>
///<param name="is broken by definition.  The [SQLITE_OMIT_AUTORESET] compile">time option</param>
///<param name="can be used to restore the legacy behavior.">can be used to restore the legacy behavior.</param>
///<param name=""></param>
///<param name="<b>Goofy Interface Alert:</b> In the legacy interface, the sqlite3_step()"><b>Goofy Interface Alert:</b> In the legacy interface, the sqlite3_step()</param>
///<param name="API always returns a generic error code, [SqlResult.SQLITE_ERROR], following any">API always returns a generic error code, [SqlResult.SQLITE_ERROR], following any</param>
///<param name="error other than [SQLITE_BUSY] and [SQLITE_MISUSE].  You must call">error other than [SQLITE_BUSY] and [SQLITE_MISUSE].  You must call</param>
///<param name="[sqlite3_reset()] or [sqlite3_finalize()] in order to find one of the">[sqlite3_reset()] or [sqlite3_finalize()] in order to find one of the</param>
///<param name="specific [error codes] that better describes the error.">specific [error codes] that better describes the error.</param>
///<param name="We admit that this is a goofy design.  The problem has been fixed">We admit that this is a goofy design.  The problem has been fixed</param>
///<param name="with the "v2" interface.  If you prepare all of your SQL statements">with the "v2" interface.  If you prepare all of your SQL statements</param>
///<param name="using either [sqlite3_prepare_v2()] or [sqlite3_prepare16_v2()] instead">using either [sqlite3_prepare_v2()] or [sqlite3_prepare16_v2()] instead</param>
///<param name="of the legacy [sqlite3_prepare()] and [sqlite3_prepare16()] interfaces,">of the legacy [sqlite3_prepare()] and [sqlite3_prepare16()] interfaces,</param>
///<param name="then the more specific [error codes] are returned directly">then the more specific [error codes] are returned directly</param>
///<param name="by sqlite3_step().  The use of the "v2" interface is recommended.">by sqlite3_step().  The use of the "v2" interface is recommended.</param>
///<param name=""></param>

		//SQLITE_API int sqlite3_step(sqlite3_stmt);
		///
///<summary>
///CAPI3REF: Number of columns in a result set
///
///^The sqlite3_data_count(P) interface returns the number of columns in the
///current row of the result set of [prepared statement] P.
///^If prepared statement P does not have results ready to return
///(via calls to the [sqlite3_column_int | sqlite3_column_*()] of
///interfaces) then sqlite3_data_count(P) returns 0.
///^The sqlite3_data_count(P) routine also returns 0 if P is a NULL pointer.
///
///See also: [vdbeapi.sqlite3_column_count()]
///
///</summary>

		//SQLITE_API int sqlite3_data_count(sqlite3_stmt *pStmt);
		///
///<summary>
///CAPI3REF: Result Values From A Query
///KEYWORDS: {column access functions}
///
///These routines form the "result set" interface.
///
///^These routines return information about a single column of the current
///result row of a query.  ^In every case the first argument is a pointer
///to the [prepared statement] that is being evaluated (the [sqlite3_stmt*]
///that was returned from [sqlite3_prepare_v2()] or one of its variants)
///and the second argument is the index of the column for which information
///should be returned. ^The leftmost column of the result set has the index 0.
///^The number of columns in the result can be determined using
///[vdbeapi.sqlite3_column_count()].
///
///If the SQL statement does not currently point to a valid row, or if the
///column index is out of range, the result is undefined.
///These routines may only be called when the most recent call to
///[sqlite3_step()] has returned [SQLITE_ROW] and neither
///[sqlite3_reset()] nor [sqlite3_finalize()] have been called subsequently.
///If any of these routines are called after [sqlite3_reset()] or
///[sqlite3_finalize()] or after [sqlite3_step()] has returned
///something other than [SQLITE_ROW], the results are undefined.
///If [sqlite3_step()] or [sqlite3_reset()] or [sqlite3_finalize()]
///are called from a different thread while any of these routines
///are pending, then the results are undefined.
///
///^The sqlite3_column_type() routine returns the
///[SQLITE_INTEGER | datatype code] for the initial data type
///of the result column.  ^The returned value is one of [SQLITE_INTEGER],
///[SQLITE_FLOAT], [SQLITE_TEXT], [SQLITE_BLOB], or [SQLITE_NULL].  The value
///returned by sqlite3_column_type() is only meaningful if no type
///conversions have occurred as described below.  After a type conversion,
///the value returned by sqlite3_column_type() is undefined.  Future
///versions of SQLite may change the behavior of sqlite3_column_type()
///following a type conversion.
///
///</summary>
///<param name="^If the result is a BLOB or UTF">8 string then the sqlite3_column_bytes()</param>
///<param name="routine returns the number of bytes in that BLOB or string.">routine returns the number of bytes in that BLOB or string.</param>
///<param name="^If the result is a UTF">16 string, then sqlite3_column_bytes() converts</param>
///<param name="the string to UTF">8 and then returns the number of bytes.</param>
///<param name="^If the result is a numeric value then sqlite3_column_bytes() uses">^If the result is a numeric value then sqlite3_column_bytes() uses</param>
///<param name="[sqlite3_snprintf()] to convert that value to a UTF">8 string and returns</param>
///<param name="the number of bytes in that string.">the number of bytes in that string.</param>
///<param name="^If the result is NULL, then sqlite3_column_bytes() returns zero.">^If the result is NULL, then sqlite3_column_bytes() returns zero.</param>
///<param name=""></param>
///<param name="^If the result is a BLOB or UTF">16 string then the sqlite3_column_bytes16()</param>
///<param name="routine returns the number of bytes in that BLOB or string.">routine returns the number of bytes in that BLOB or string.</param>
///<param name="^If the result is a UTF">8 string, then sqlite3_column_bytes16() converts</param>
///<param name="the string to UTF">16 and then returns the number of bytes.</param>
///<param name="^If the result is a numeric value then sqlite3_column_bytes16() uses">^If the result is a numeric value then sqlite3_column_bytes16() uses</param>
///<param name="[sqlite3_snprintf()] to convert that value to a UTF">16 string and returns</param>
///<param name="the number of bytes in that string.">the number of bytes in that string.</param>
///<param name="^If the result is NULL, then sqlite3_column_bytes16() returns zero.">^If the result is NULL, then sqlite3_column_bytes16() returns zero.</param>
///<param name=""></param>
///<param name="^The values returned by [sqlite3_column_bytes()] and ">^The values returned by [sqlite3_column_bytes()] and </param>
///<param name="[sqlite3_column_bytes16()] do not include the zero terminators at the end">[sqlite3_column_bytes16()] do not include the zero terminators at the end</param>
///<param name="of the string.  ^For clarity: the values returned by">of the string.  ^For clarity: the values returned by</param>
///<param name="[sqlite3_column_bytes()] and [sqlite3_column_bytes16()] are the number of">[sqlite3_column_bytes()] and [sqlite3_column_bytes16()] are the number of</param>
///<param name="bytes in the string, not the number of characters.">bytes in the string, not the number of characters.</param>
///<param name=""></param>
///<param name="^Strings returned by sqlite3_column_text() and sqlite3_column_text16(),">^Strings returned by sqlite3_column_text() and sqlite3_column_text16(),</param>
///<param name="even empty strings, are always zero terminated.  ^The return">even empty strings, are always zero terminated.  ^The return</param>
///<param name="value from sqlite3_column_blob() for a zero">length BLOB is a NULL pointer.</param>
///<param name=""></param>
///<param name="^The object returned by [sqlite3_column_value()] is an">^The object returned by [sqlite3_column_value()] is an</param>
///<param name="[unprotected sqlite3_value] object.  An unprotected sqlite3_value object">[unprotected sqlite3_value] object.  An unprotected sqlite3_value object</param>
///<param name="may only be used with [sqlite3_bind_value()] and [sqlite3_result_value()].">may only be used with [sqlite3_bind_value()] and [sqlite3_result_value()].</param>
///<param name="If the [unprotected sqlite3_value] object returned by">If the [unprotected sqlite3_value] object returned by</param>
///<param name="[sqlite3_column_value()] is used in any other way, including calls">[sqlite3_column_value()] is used in any other way, including calls</param>
///<param name="to routines like [sqlite3_value_int()], [vdbeapi.sqlite3_value_text()],">to routines like [sqlite3_value_int()], [vdbeapi.sqlite3_value_text()],</param>
///<param name="or [vdbeapi.sqlite3_value_bytes()], then the behavior is undefined.">or [vdbeapi.sqlite3_value_bytes()], then the behavior is undefined.</param>
///<param name=""></param>
///<param name="These routines attempt to convert the value where appropriate.  ^For">These routines attempt to convert the value where appropriate.  ^For</param>
///<param name="example, if the internal representation is FLOAT and a text result">example, if the internal representation is FLOAT and a text result</param>
///<param name="is requested, [sqlite3_snprintf()] is used internally to perform the">is requested, [sqlite3_snprintf()] is used internally to perform the</param>
///<param name="conversion automatically.  ^(The following table details the conversions">conversion automatically.  ^(The following table details the conversions</param>
///<param name="that are applied:">that are applied:</param>
///<param name=""></param>
///<param name="<blockquote>"><blockquote></param>
///<param name="<table border="1">"><table border="1"></param>
///<param name="<tr><th> Internal<br>Type <th> Requested<br>Type <th>  Conversion"><tr><th> Internal<br>Type <th> Requested<br>Type <th>  Conversion</param>
///<param name=""></param>
///<param name="<tr><td>  NULL    <td> INTEGER   <td> Result is 0"><tr><td>  NULL    <td> INTEGER   <td> Result is 0</param>
///<param name="<tr><td>  NULL    <td>  FLOAT    <td> Result is 0.0"><tr><td>  NULL    <td>  FLOAT    <td> Result is 0.0</param>
///<param name="<tr><td>  NULL    <td>   TEXT    <td> Result is NULL pointer"><tr><td>  NULL    <td>   TEXT    <td> Result is NULL pointer</param>
///<param name="<tr><td>  NULL    <td>   BLOB    <td> Result is NULL pointer"><tr><td>  NULL    <td>   BLOB    <td> Result is NULL pointer</param>
///<param name="<tr><td> INTEGER  <td>  FLOAT    <td> Convert from integer to float"><tr><td> INTEGER  <td>  FLOAT    <td> Convert from integer to float</param>
///<param name="<tr><td> INTEGER  <td>   TEXT    <td> ASCII rendering of the integer"><tr><td> INTEGER  <td>   TEXT    <td> ASCII rendering of the integer</param>
///<param name="<tr><td> INTEGER  <td>   BLOB    <td> Same as INTEGER">>TEXT</param>
///<param name="<tr><td>  FLOAT   <td> INTEGER   <td> Convert from float to integer"><tr><td>  FLOAT   <td> INTEGER   <td> Convert from float to integer</param>
///<param name="<tr><td>  FLOAT   <td>   TEXT    <td> ASCII rendering of the float"><tr><td>  FLOAT   <td>   TEXT    <td> ASCII rendering of the float</param>
///<param name="<tr><td>  FLOAT   <td>   BLOB    <td> Same as FLOAT">>TEXT</param>
///<param name="<tr><td>  TEXT    <td> INTEGER   <td> Use _Custom.atoi()"><tr><td>  TEXT    <td> INTEGER   <td> Use _Custom.atoi()</param>
///<param name="<tr><td>  TEXT    <td>  FLOAT    <td> Use atof()"><tr><td>  TEXT    <td>  FLOAT    <td> Use atof()</param>
///<param name="<tr><td>  TEXT    <td>   BLOB    <td> No change"><tr><td>  TEXT    <td>   BLOB    <td> No change</param>
///<param name="<tr><td>  BLOB    <td> INTEGER   <td> Convert to TEXT then use _Custom.atoi()"><tr><td>  BLOB    <td> INTEGER   <td> Convert to TEXT then use _Custom.atoi()</param>
///<param name="<tr><td>  BLOB    <td>  FLOAT    <td> Convert to TEXT then use atof()"><tr><td>  BLOB    <td>  FLOAT    <td> Convert to TEXT then use atof()</param>
///<param name="<tr><td>  BLOB    <td>   TEXT    <td> Add a zero terminator if needed"><tr><td>  BLOB    <td>   TEXT    <td> Add a zero terminator if needed</param>
///<param name="</table>"></table></param>
///<param name="</blockquote>)^"></blockquote>)^</param>
///<param name=""></param>
///<param name="The table above makes reference to standard C library functions _Custom.atoi()">The table above makes reference to standard C library functions _Custom.atoi()</param>
///<param name="and atof().  SQLite does not really use these functions.  It has its">and atof().  SQLite does not really use these functions.  It has its</param>
///<param name="own equivalent internal routines.  The _Custom.atoi() and atof() names are">own equivalent internal routines.  The _Custom.atoi() and atof() names are</param>
///<param name="used in the table for brevity and because they are familiar to most">used in the table for brevity and because they are familiar to most</param>
///<param name="C programmers.">C programmers.</param>
///<param name=""></param>
///<param name="Note that when type conversions occur, pointers returned by prior">Note that when type conversions occur, pointers returned by prior</param>
///<param name="calls to sqlite3_column_blob(), sqlite3_column_text(), and/or">calls to sqlite3_column_blob(), sqlite3_column_text(), and/or</param>
///<param name="sqlite3_column_text16() may be invalidated.">sqlite3_column_text16() may be invalidated.</param>
///<param name="Type conversions and pointer invalidations might occur">Type conversions and pointer invalidations might occur</param>
///<param name="in the following cases:">in the following cases:</param>
///<param name=""></param>
///<param name="<ul>"><ul></param>
///<param name="<li> The initial content is a BLOB and sqlite3_column_text() or"><li> The initial content is a BLOB and sqlite3_column_text() or</param>
///<param name="sqlite3_column_text16() is called.  A zero">terminator might</param>
///<param name="need to be added to the string.</li>">need to be added to the string.</li></param>
///<param name="<li> The initial content is UTF">8 text and sqlite3_column_bytes16() or</param>
///<param name="sqlite3_column_text16() is called.  The content must be converted">sqlite3_column_text16() is called.  The content must be converted</param>
///<param name="to UTF">16.</li></param>
///<param name="<li> The initial content is UTF">16 text and sqlite3_column_bytes() or</param>
///<param name="sqlite3_column_text() is called.  The content must be converted">sqlite3_column_text() is called.  The content must be converted</param>
///<param name="to UTF">8.</li></param>
///<param name="</ul>"></ul></param>
///<param name=""></param>
///<param name="^Conversions between UTF">16le are always done in place and do</param>
///<param name="not invalidate a prior pointer, though of course the content of the buffer">not invalidate a prior pointer, though of course the content of the buffer</param>
///<param name="that the prior pointer references will have been modified.  Other kinds">that the prior pointer references will have been modified.  Other kinds</param>
///<param name="of conversion are done in place when it is possible, but sometimes they">of conversion are done in place when it is possible, but sometimes they</param>
///<param name="are not possible and in those cases prior pointers are invalidated.">are not possible and in those cases prior pointers are invalidated.</param>
///<param name=""></param>
///<param name="The safest and easiest to remember policy is to invoke these routines">The safest and easiest to remember policy is to invoke these routines</param>
///<param name="in one of the following ways:">in one of the following ways:</param>
///<param name=""></param>
///<param name="<ul>"><ul></param>
///<param name="<li>sqlite3_column_text() followed by sqlite3_column_bytes()</li>"><li>sqlite3_column_text() followed by sqlite3_column_bytes()</li></param>
///<param name="<li>sqlite3_column_blob() followed by sqlite3_column_bytes()</li>"><li>sqlite3_column_blob() followed by sqlite3_column_bytes()</li></param>
///<param name="<li>sqlite3_column_text16() followed by sqlite3_column_bytes16()</li>"><li>sqlite3_column_text16() followed by sqlite3_column_bytes16()</li></param>
///<param name="</ul>"></ul></param>
///<param name=""></param>
///<param name="In other words, you should call sqlite3_column_text(),">In other words, you should call sqlite3_column_text(),</param>
///<param name="sqlite3_column_blob(), or sqlite3_column_text16() first to force the result">sqlite3_column_blob(), or sqlite3_column_text16() first to force the result</param>
///<param name="into the desired format, then invoke sqlite3_column_bytes() or">into the desired format, then invoke sqlite3_column_bytes() or</param>
///<param name="sqlite3_column_bytes16() to find the size of the result.  Do not mix calls">sqlite3_column_bytes16() to find the size of the result.  Do not mix calls</param>
///<param name="to sqlite3_column_text() or sqlite3_column_blob() with calls to">to sqlite3_column_text() or sqlite3_column_blob() with calls to</param>
///<param name="sqlite3_column_bytes16(), and do not mix calls to sqlite3_column_text16()">sqlite3_column_bytes16(), and do not mix calls to sqlite3_column_text16()</param>
///<param name="with calls to sqlite3_column_bytes().">with calls to sqlite3_column_bytes().</param>
///<param name=""></param>
///<param name="^The pointers returned are valid until a type conversion occurs as">^The pointers returned are valid until a type conversion occurs as</param>
///<param name="described above, or until [sqlite3_step()] or [sqlite3_reset()] or">described above, or until [sqlite3_step()] or [sqlite3_reset()] or</param>
///<param name="[sqlite3_finalize()] is called.  ^The memory space used to hold strings">[sqlite3_finalize()] is called.  ^The memory space used to hold strings</param>
///<param name="and BLOBs is freed automatically.  Do <b>not</b> pass the pointers returned">and BLOBs is freed automatically.  Do <b>not</b> pass the pointers returned</param>
///<param name="[sqlite3_column_blob()], [sqlite3_column_text()], etc. into">[sqlite3_column_blob()], [sqlite3_column_text()], etc. into</param>
///<param name="[malloc_cs.sqlite3_free()].">[malloc_cs.sqlite3_free()].</param>
///<param name=""></param>
///<param name="^(If a memory allocation error occurs during the evaluation of any">^(If a memory allocation error occurs during the evaluation of any</param>
///<param name="of these routines, a default value is returned.  The default value">of these routines, a default value is returned.  The default value</param>
///<param name="is either the integer 0, the floating point number 0.0, or a NULL">is either the integer 0, the floating point number 0.0, or a NULL</param>
///<param name="pointer.  Subsequent calls to [sqlite3_errcode()] will return">pointer.  Subsequent calls to [sqlite3_errcode()] will return</param>
///<param name="[SQLITE_NOMEM].)^">[SQLITE_NOMEM].)^</param>
///<param name=""></param>

		//SQLITE_API const void *sqlite3_column_blob(sqlite3_stmt*, int iCol);
		//SQLITE_API int sqlite3_column_bytes(sqlite3_stmt*, int iCol);
		//SQLITE_API int sqlite3_column_bytes16(sqlite3_stmt*, int iCol);
		//SQLITE_API double sqlite3_column_double(sqlite3_stmt*, int iCol);
		//SQLITE_API int sqlite3_column_int(sqlite3_stmt*, int iCol);
		//SQLITE_API sqlite3_int64 sqlite3_column_int64(sqlite3_stmt*, int iCol);
		//SQLITE_API const unsigned char *sqlite3_column_text(sqlite3_stmt*, int iCol);
		//SQLITE_API const void *sqlite3_column_text16(sqlite3_stmt*, int iCol);
		//SQLITE_API int sqlite3_column_type(sqlite3_stmt*, int iCol);
		//SQLITE_API sqlite3_value *sqlite3_column_value(sqlite3_stmt*, int iCol);
		///
///<summary>
///CAPI3REF: Destroy A Prepared Statement Object
///
///^The sqlite3_finalize() function is called to delete a [prepared statement].
///^If the most recent evaluation of the statement encountered no errors
///or if the statement is never been evaluated, then sqlite3_finalize() returns
///SqlResult.SQLITE_OK.  ^If the most recent evaluation of statement S failed, then
///sqlite3_finalize(S) returns the appropriate [error code] or
///[extended error code].
///
///^The sqlite3_finalize(S) routine can be called at any point during
///the life cycle of [prepared statement] S:
///before statement S is ever evaluated, after
///one or more calls to [sqlite3_reset()], or after any call
///to [sqlite3_step()] regardless of whether or not the statement has
///completed execution.
///
///</summary>
///<param name="^Invoking sqlite3_finalize() on a NULL pointer is a harmless no">op.</param>
///<param name=""></param>
///<param name="The application must finalize every [prepared statement] in order to avoid">The application must finalize every [prepared statement] in order to avoid</param>
///<param name="resource leaks.  It is a grievous error for the application to try to use">resource leaks.  It is a grievous error for the application to try to use</param>
///<param name="a prepared statement after it has been finalized.  Any use of a prepared">a prepared statement after it has been finalized.  Any use of a prepared</param>
///<param name="statement after it has been finalized can result in undefined and">statement after it has been finalized can result in undefined and</param>
///<param name="undesirable behavior such as segfaults and heap corruption.">undesirable behavior such as segfaults and heap corruption.</param>
///<param name=""></param>

		//SQLITE_API int sqlite3_finalize(sqlite3_stmt *pStmt);
		///
///<summary>
///CAPI3REF: Reset A Prepared Statement Object
///
///The sqlite3_reset() function is called to reset a [prepared statement]
///</summary>
///<param name="object back to its initial state, ready to be re">executed.</param>
///<param name="^Any SQL statement variables that had values bound to them using">^Any SQL statement variables that had values bound to them using</param>
///<param name="the [sqlite3_bind_blob | sqlite3_bind_*() API] retain their values.">the [sqlite3_bind_blob | sqlite3_bind_*() API] retain their values.</param>
///<param name="Use [sqlite3_clear_bindings()] to reset the bindings.">Use [sqlite3_clear_bindings()] to reset the bindings.</param>
///<param name=""></param>
///<param name="^The [sqlite3_reset(S)] interface resets the [prepared statement] S">^The [sqlite3_reset(S)] interface resets the [prepared statement] S</param>
///<param name="back to the beginning of its program.">back to the beginning of its program.</param>
///<param name=""></param>
///<param name="^If the most recent call to [sqlite3_step(S)] for the">^If the most recent call to [sqlite3_step(S)] for the</param>
///<param name="[prepared statement] S returned [SQLITE_ROW] or [SQLITE_DONE],">[prepared statement] S returned [SQLITE_ROW] or [SQLITE_DONE],</param>
///<param name="or if [sqlite3_step(S)] has never before been called on S,">or if [sqlite3_step(S)] has never before been called on S,</param>
///<param name="then [sqlite3_reset(S)] returns [SqlResult.SQLITE_OK].">then [sqlite3_reset(S)] returns [SqlResult.SQLITE_OK].</param>
///<param name=""></param>
///<param name="^If the most recent call to [sqlite3_step(S)] for the">^If the most recent call to [sqlite3_step(S)] for the</param>
///<param name="[prepared statement] S indicated an error, then">[prepared statement] S indicated an error, then</param>
///<param name="[sqlite3_reset(S)] returns an appropriate [error code].">[sqlite3_reset(S)] returns an appropriate [error code].</param>
///<param name=""></param>
///<param name="^The [sqlite3_reset(S)] interface does not change the values">^The [sqlite3_reset(S)] interface does not change the values</param>
///<param name="of any [sqlite3_bind_blob|bindings] on the [prepared statement] S.">of any [sqlite3_bind_blob|bindings] on the [prepared statement] S.</param>
///<param name=""></param>

		//SQLITE_API int sqlite3_reset(sqlite3_stmt *pStmt);
		///
///<summary>
///CAPI3REF: Create Or Redefine SQL Functions
///KEYWORDS: {function creation routines}
///</summary>
///<param name="KEYWORDS: {application">defined SQL function}</param>
///<param name="KEYWORDS: {application">defined SQL functions}</param>
///<param name=""></param>
///<param name="^These functions (collectively known as "function creation routines")">^These functions (collectively known as "function creation routines")</param>
///<param name="are used to add SQL functions or aggregates or to redefine the behavior">are used to add SQL functions or aggregates or to redefine the behavior</param>
///<param name="of existing SQL functions or aggregates.  The only differences between">of existing SQL functions or aggregates.  The only differences between</param>
///<param name="these routines are the text encoding expected for">these routines are the text encoding expected for</param>
///<param name="the second parameter (the name of the function being created)">the second parameter (the name of the function being created)</param>
///<param name="and the presence or absence of a destructor callback for">and the presence or absence of a destructor callback for</param>
///<param name="the application data pointer.">the application data pointer.</param>
///<param name=""></param>
///<param name="^The first parameter is the [database connection] to which the SQL">^The first parameter is the [database connection] to which the SQL</param>
///<param name="function is to be added.  ^If an application uses more than one database">function is to be added.  ^If an application uses more than one database</param>
///<param name="connection then application">defined SQL functions must be added</param>
///<param name="to each database connection separately.">to each database connection separately.</param>
///<param name=""></param>
///<param name="^The second parameter is the name of the SQL function to be created or">^The second parameter is the name of the SQL function to be created or</param>
///<param name="redefined.  ^The length of the name is limited to 255 bytes in a UTF">8</param>
///<param name="representation, exclusive of the zero">terminator.  ^Note that the name</param>
///<param name="length limit is in UTF">16 bytes.  </param>
///<param name="^Any attempt to create a function with a longer name">^Any attempt to create a function with a longer name</param>
///<param name="will result in [SQLITE_MISUSE] being returned.">will result in [SQLITE_MISUSE] being returned.</param>
///<param name=""></param>
///<param name="^The third parameter (nArg)">^The third parameter (nArg)</param>
///<param name="is the number of arguments that the SQL function or">is the number of arguments that the SQL function or</param>
///<param name="aggregate takes. ^If this parameter is ">1, then the SQL function or</param>
///<param name="aggregate may take any number of arguments between 0 and the limit">aggregate may take any number of arguments between 0 and the limit</param>
///<param name="set by [sqlite3_limit]([SQLITE_LIMIT_FUNCTION_ARG]).  If the third">set by [sqlite3_limit]([SQLITE_LIMIT_FUNCTION_ARG]).  If the third</param>
///<param name="parameter is less than ">1 or greater than 127 then the behavior is</param>
///<param name="undefined.">undefined.</param>
///<param name=""></param>
///<param name="^The fourth parameter, eTextRep, specifies what">^The fourth parameter, eTextRep, specifies what</param>
///<param name="[SQLITE_UTF8 | text encoding] this SQL function prefers for">[SQLITE_UTF8 | text encoding] this SQL function prefers for</param>
///<param name="its parameters.  Every SQL function implementation must be able to work">its parameters.  Every SQL function implementation must be able to work</param>
///<param name="with UTF">16be.  But some implementations may be</param>
///<param name="more efficient with one encoding than another.  ^An application may">more efficient with one encoding than another.  ^An application may</param>
///<param name="invoke sqlite3_create_function() or sqlite3_create_function16() multiple">invoke sqlite3_create_function() or sqlite3_create_function16() multiple</param>
///<param name="times with the same function but with different values of eTextRep.">times with the same function but with different values of eTextRep.</param>
///<param name="^When multiple implementations of the same function are available, SQLite">^When multiple implementations of the same function are available, SQLite</param>
///<param name="will pick the one that involves the least amount of data conversion.">will pick the one that involves the least amount of data conversion.</param>
///<param name="If there is only a single implementation which does not care what text">If there is only a single implementation which does not care what text</param>
///<param name="encoding is used, then the fourth argument should be [SQLITE_ANY].">encoding is used, then the fourth argument should be [SQLITE_ANY].</param>
///<param name=""></param>
///<param name="^(The fifth parameter is an arbitrary pointer.  The implementation of the">^(The fifth parameter is an arbitrary pointer.  The implementation of the</param>
///<param name="function can gain access to this pointer using [sqlite3_user_data()].)^">function can gain access to this pointer using [sqlite3_user_data()].)^</param>
///<param name=""></param>
///<param name="^The sixth, seventh and eighth parameters, xFunc, xStep and xFinal, are">^The sixth, seventh and eighth parameters, xFunc, xStep and xFinal, are</param>
///<param name="pointers to C">language functions that implement the SQL function or</param>
///<param name="aggregate. ^A scalar SQL function requires an implementation of the xFunc">aggregate. ^A scalar SQL function requires an implementation of the xFunc</param>
///<param name="callback only; NULL pointers must be passed as the xStep and xFinal">callback only; NULL pointers must be passed as the xStep and xFinal</param>
///<param name="parameters. ^An aggregate SQL function requires an implementation of xStep">parameters. ^An aggregate SQL function requires an implementation of xStep</param>
///<param name="and xFinal and NULL pointer must be passed for xFunc. ^To delete an existing">and xFinal and NULL pointer must be passed for xFunc. ^To delete an existing</param>
///<param name="SQL function or aggregate, pass NULL pointers for all three function">SQL function or aggregate, pass NULL pointers for all three function</param>
///<param name="callbacks.">callbacks.</param>
///<param name=""></param>
///<param name="^(If the ninth parameter to sqlite3_create_function_v2() is not NULL,">^(If the ninth parameter to sqlite3_create_function_v2() is not NULL,</param>
///<param name="then it is destructor for the application data pointer. ">then it is destructor for the application data pointer. </param>
///<param name="The destructor is invoked when the function is deleted, either by being">The destructor is invoked when the function is deleted, either by being</param>
///<param name="overloaded or when the database connection closes.)^">overloaded or when the database connection closes.)^</param>
///<param name="^The destructor is also invoked if the call to">^The destructor is also invoked if the call to</param>
///<param name="sqlite3_create_function_v2() fails.">sqlite3_create_function_v2() fails.</param>
///<param name="^When the destructor callback of the tenth parameter is invoked, it">^When the destructor callback of the tenth parameter is invoked, it</param>
///<param name="is passed a single argument which is a copy of the application data ">is passed a single argument which is a copy of the application data </param>
///<param name="pointer which was the fifth parameter to sqlite3_create_function_v2().">pointer which was the fifth parameter to sqlite3_create_function_v2().</param>
///<param name=""></param>
///<param name="^It is permitted to register multiple implementations of the same">^It is permitted to register multiple implementations of the same</param>
///<param name="functions with the same name but with either differing numbers of">functions with the same name but with either differing numbers of</param>
///<param name="arguments or differing preferred text encodings.  ^SQLite will use">arguments or differing preferred text encodings.  ^SQLite will use</param>
///<param name="the implementation that most closely matches the way in which the">the implementation that most closely matches the way in which the</param>
///<param name="SQL function is used.  ^A function implementation with a non">negative</param>
///<param name="nArg parameter is a better match than a function implementation with">nArg parameter is a better match than a function implementation with</param>
///<param name="a negative nArg.  ^A function where the preferred text encoding">a negative nArg.  ^A function where the preferred text encoding</param>
///<param name="matches the database encoding is a better">matches the database encoding is a better</param>
///<param name="match than a function where the encoding is different.  ">match than a function where the encoding is different.  </param>
///<param name="^A function where the encoding difference is between UTF16le and UTF16be">^A function where the encoding difference is between UTF16le and UTF16be</param>
///<param name="is a closer match than a function where the encoding difference is">is a closer match than a function where the encoding difference is</param>
///<param name="between UTF8 and UTF16.">between UTF8 and UTF16.</param>
///<param name=""></param>
///<param name="^Built">defined functions.</param>
///<param name=""></param>
///<param name="^An application">defined function is permitted to call other</param>
///<param name="SQLite interfaces.  However, such calls must not">SQLite interfaces.  However, such calls must not</param>
///<param name="close the database connection nor finalize or reset the prepared">close the database connection nor finalize or reset the prepared</param>
///<param name="statement in which the function is running.">statement in which the function is running.</param>
///<param name=""></param>

		//SQLITE_API int sqlite3_create_function(
		//  sqlite3 db,
		//  string zFunctionName,
		//  int nArg,
		//  int eTextRep,
		//  void *pApp,
		//  void (*xFunc)(sqlite3_context*,int,sqlite3_value*),
		//  void (*xStep)(sqlite3_context*,int,sqlite3_value*),
		//  void (*xFinal)(sqlite3_context)
		//);
		//SQLITE_API int sqlite3_create_function16(
		//  sqlite3 db,
		//  string zFunctionName,
		//  int nArg,
		//  int eTextRep,
		//  void *pApp,
		//  void (*xFunc)(sqlite3_context*,int,sqlite3_value*),
		//  void (*xStep)(sqlite3_context*,int,sqlite3_value*),
		//  void (*xFinal)(sqlite3_context)
		//);
		//SQLITE_API int sqlite3_create_function_v2(
		//  sqlite3 db,
		//  string zFunctionName,
		//  int nArg,
		//  int eTextRep,
		//  void *pApp,
		//  void (*xFunc)(sqlite3_context*,int,sqlite3_value*),
		//  void (*xStep)(sqlite3_context*,int,sqlite3_value*),
		//  void (*xFinal)(sqlite3_context),
		//  void(*xDestroy)(void)
		//);
		///
///<summary>
///CAPI3REF: Deprecated Functions
///DEPRECATED
///
///These functions are [deprecated].  In order to maintain
///backwards compatibility with older code, these functions continue 
///to be supported.  However, new applications should avoid
///the use of these functions.  To help encourage people to avoid
///using these functions, we are not going to tell you what they do.
///
///</summary>

		//#if !SQLITE_OMIT_DEPRECATED
		//SQLITE_API SQLITE_DEPRECATED int sqlite3_aggregate_count(sqlite3_context);
		//SQLITE_API SQLITE_DEPRECATED int sqlite3_expired(sqlite3_stmt);
		//SQLITE_API SQLITE_DEPRECATED int sqlite3_transfer_bindings(sqlite3_stmt*, sqlite3_stmt);
		//SQLITE_API SQLITE_DEPRECATED int sqlite3_global_recover(void);
		//SQLITE_API SQLITE_DEPRECATED void sqlite3_thread_cleanup(void);
		//SQLITE_API SQLITE_DEPRECATED int sqlite3_memory_alarm(void()(void*,sqlite3_int64,int),void*,sqlite3_int64);
		//#endif
		///
///<summary>
///CAPI3REF: Obtaining SQL Function Parameter Values
///
///</summary>
///<param name="The C">language implementation of SQL functions and aggregates uses</param>
///<param name="this set of interface routines to access the parameter values on">this set of interface routines to access the parameter values on</param>
///<param name="the function or aggregate.">the function or aggregate.</param>
///<param name=""></param>
///<param name="The xFunc (for scalar functions) or xStep (for aggregates) parameters">The xFunc (for scalar functions) or xStep (for aggregates) parameters</param>
///<param name="to [sqlite3_create_function()] and [sqlite3_create_function16()]">to [sqlite3_create_function()] and [sqlite3_create_function16()]</param>
///<param name="define callbacks that implement the SQL functions and aggregates.">define callbacks that implement the SQL functions and aggregates.</param>
///<param name="The 3rd parameter to these callbacks is an array of pointers to">The 3rd parameter to these callbacks is an array of pointers to</param>
///<param name="[protected sqlite3_value] objects.  There is one [sqlite3_value] object for">[protected sqlite3_value] objects.  There is one [sqlite3_value] object for</param>
///<param name="each parameter to the SQL function.  These routines are used to">each parameter to the SQL function.  These routines are used to</param>
///<param name="extract values from the [sqlite3_value] objects.">extract values from the [sqlite3_value] objects.</param>
///<param name=""></param>
///<param name="These routines work only with [protected sqlite3_value] objects.">These routines work only with [protected sqlite3_value] objects.</param>
///<param name="Any attempt to use these routines on an [unprotected sqlite3_value]">Any attempt to use these routines on an [unprotected sqlite3_value]</param>
///<param name="object results in undefined behavior.">object results in undefined behavior.</param>
///<param name=""></param>
///<param name="^These routines work just like the corresponding [column access functions]">^These routines work just like the corresponding [column access functions]</param>
///<param name="except that  these routines take a single [protected sqlite3_value] object">except that  these routines take a single [protected sqlite3_value] object</param>
///<param name="pointer instead of a [sqlite3_stmt*] pointer and an integer column number.">pointer instead of a [sqlite3_stmt*] pointer and an integer column number.</param>
///<param name=""></param>
///<param name="^The vdbeapi.sqlite3_value_text16() interface extracts a UTF">16 string</param>
///<param name="in the native byte">order of the host machine.  ^The</param>
///<param name="vdbeapi.sqlite3_value_text16be() and vdbeapi.sqlite3_value_text16le() interfaces">vdbeapi.sqlite3_value_text16be() and vdbeapi.sqlite3_value_text16le() interfaces</param>
///<param name="extract UTF">endian respectively.</param>
///<param name=""></param>
///<param name="^(The sqlite3_value_numeric_type() interface attempts to apply">^(The sqlite3_value_numeric_type() interface attempts to apply</param>
///<param name="numeric affinity to the value.  This means that an attempt is">numeric affinity to the value.  This means that an attempt is</param>
///<param name="made to convert the value to an integer or floating point.  If">made to convert the value to an integer or floating point.  If</param>
///<param name="such a conversion is possible without loss of information (in other">such a conversion is possible without loss of information (in other</param>
///<param name="words, if the value is a string that looks like a number)">words, if the value is a string that looks like a number)</param>
///<param name="then the conversion is performed.  Otherwise no conversion occurs.">then the conversion is performed.  Otherwise no conversion occurs.</param>
///<param name="The [SQLITE_INTEGER | datatype] after conversion is returned.)^">The [SQLITE_INTEGER | datatype] after conversion is returned.)^</param>
///<param name=""></param>
///<param name="Please pay particular attention to the fact that the pointer returned">Please pay particular attention to the fact that the pointer returned</param>
///<param name="from [sqlite3_value_blob()], [vdbeapi.sqlite3_value_text()], or">from [sqlite3_value_blob()], [vdbeapi.sqlite3_value_text()], or</param>
///<param name="[vdbeapi.sqlite3_value_text16()] can be invalidated by a subsequent call to">[vdbeapi.sqlite3_value_text16()] can be invalidated by a subsequent call to</param>
///<param name="[vdbeapi.sqlite3_value_bytes()], [vdbeapi.sqlite3_value_bytes16()], [vdbeapi.sqlite3_value_text()],">[vdbeapi.sqlite3_value_bytes()], [vdbeapi.sqlite3_value_bytes16()], [vdbeapi.sqlite3_value_text()],</param>
///<param name="or [vdbeapi.sqlite3_value_text16()].">or [vdbeapi.sqlite3_value_text16()].</param>
///<param name=""></param>
///<param name="These routines must be called from the same thread as">These routines must be called from the same thread as</param>
///<param name="the SQL function that supplied the [sqlite3_value*] parameters.">the SQL function that supplied the [sqlite3_value*] parameters.</param>
///<param name=""></param>

		//SQLITE_API const void *sqlite3_value_blob(sqlite3_value);
		//SQLITE_API int vdbeapi.sqlite3_value_bytes(sqlite3_value);
		//SQLITE_API int vdbeapi.sqlite3_value_bytes16(sqlite3_value);
		//SQLITE_API double sqlite3_value_double(sqlite3_value);
		//SQLITE_API int sqlite3_value_int(sqlite3_value);
		//SQLITE_API sqlite3_int64 sqlite3_value_int64(sqlite3_value);
		//SQLITE_API const unsigned char *vdbeapi.sqlite3_value_text(sqlite3_value);
		//SQLITE_API const void *vdbeapi.sqlite3_value_text16(sqlite3_value);
		//SQLITE_API const void *vdbeapi.sqlite3_value_text16le(sqlite3_value);
		//SQLITE_API const void *vdbeapi.sqlite3_value_text16be(sqlite3_value);
		//SQLITE_API int vdbeapi.sqlite3_value_type(sqlite3_value);
		//SQLITE_API int sqlite3_value_numeric_type(sqlite3_value);
		///
///<summary>
///CAPI3REF: Obtain Aggregate Function Context
///
///Implementations of aggregate SQL functions use this
///routine to allocate memory for storing their state.
///
///^The first time the vdbeapi.sqlite3_aggregate_context(C,N) routine is called 
///for a particular aggregate function, SQLite
///allocates N of memory, zeroes out that memory, and returns a pointer
///to the new memory. ^On second and subsequent calls to
///vdbeapi.sqlite3_aggregate_context() for the same aggregate function instance,
///the same buffer is returned.  vdbeapi.sqlite3_aggregate_context() is normally
///called once for each invocation of the xStep callback and then one
///last time when the xFinal callback is invoked.  ^(When no rows match
///an aggregate query, the xStep() callback of the aggregate function
///implementation is never called and xFinal() is called exactly once.
///In those cases, vdbeapi.sqlite3_aggregate_context() might be called for the
///first time from within xFinal().)^
///
///^The vdbeapi.sqlite3_aggregate_context(C,N) routine returns a NULL pointer if N is
///less than or equal to zero or if a memory allocate error occurs.
///
///^(The amount of space allocated by vdbeapi.sqlite3_aggregate_context(C,N) is
///determined by the N parameter on first successful call.  Changing the
///value of N in subsequent call to vdbeapi.sqlite3_aggregate_context() within
///the same aggregate function instance will not resize the memory
///allocation.)^
///
///^SQLite automatically frees the memory allocated by 
///vdbeapi.sqlite3_aggregate_context() when the aggregate query concludes.
///
///The first parameter must be a copy of the
///[sqlite3_context | SQL function context] that is the first parameter
///to the xStep or xFinal callback routine that implements the aggregate
///function.
///
///This routine must be called from the same thread in which
///the aggregate SQL function is running.
///
///</summary>

		//SQLITE_API void *vdbeapi.sqlite3_aggregate_context(sqlite3_context*, int nBytes);
		///
///<summary>
///CAPI3REF: User Data For Functions
///
///^The sqlite3_user_data() interface returns a copy of
///the pointer that was the pUserData parameter (the 5th parameter)
///of the [sqlite3_create_function()]
///and [sqlite3_create_function16()] routines that originally
///registered the application defined function.
///
///This routine must be called from the same thread in which
///</summary>
///<param name="the application">defined function is running.</param>
///<param name=""></param>

		//SQLITE_API void *sqlite3_user_data(sqlite3_context);
		///
///<summary>
///CAPI3REF: Database Connection For Functions
///
///^The vdbeapi.sqlite3_context_db_handle() interface returns a copy of
///the pointer to the [database connection] (the 1st parameter)
///of the [sqlite3_create_function()]
///and [sqlite3_create_function16()] routines that originally
///registered the application defined function.
///
///</summary>

		//SQLITE_API sqlite3 *vdbeapi.sqlite3_context_db_handle(sqlite3_context);
		///
///<summary>
///CAPI3REF: Function Auxiliary Data
///
///The following two functions may be used by scalar SQL functions to
///associate metadata with argument values. If the same value is passed to
///multiple invocations of the same SQL function during query execution, under
///some circumstances the associated metadata may be preserved. This may
///</summary>
///<param name="be used, for example, to add a regular">expression matching scalar</param>
///<param name="function. The compiled version of the regular expression is stored as">function. The compiled version of the regular expression is stored as</param>
///<param name="metadata associated with the SQL value passed as the regular expression">metadata associated with the SQL value passed as the regular expression</param>
///<param name="pattern.  The compiled regular expression can be reused on multiple">pattern.  The compiled regular expression can be reused on multiple</param>
///<param name="invocations of the same function so that the original pattern string">invocations of the same function so that the original pattern string</param>
///<param name="does not need to be recompiled on each invocation.">does not need to be recompiled on each invocation.</param>
///<param name=""></param>
///<param name="^The sqlite3_get_auxdata() interface returns a pointer to the metadata">^The sqlite3_get_auxdata() interface returns a pointer to the metadata</param>
///<param name="associated by the sqlite3_set_auxdata() function with the Nth argument">associated by the sqlite3_set_auxdata() function with the Nth argument</param>
///<param name="value to the application">defined function. ^If no metadata has been ever</param>
///<param name="been set for the Nth argument of the function, or if the corresponding">been set for the Nth argument of the function, or if the corresponding</param>
///<param name="function parameter has changed since the meta">data was set,</param>
///<param name="then sqlite3_get_auxdata() returns a NULL pointer.">then sqlite3_get_auxdata() returns a NULL pointer.</param>
///<param name=""></param>
///<param name="^The sqlite3_set_auxdata() interface saves the metadata">^The sqlite3_set_auxdata() interface saves the metadata</param>
///<param name="pointed to by its 3rd parameter as the metadata for the N">th</param>
///<param name="argument of the application">defined function.  Subsequent</param>
///<param name="calls to sqlite3_get_auxdata() might return this data, if it has">calls to sqlite3_get_auxdata() might return this data, if it has</param>
///<param name="not been destroyed.">not been destroyed.</param>
///<param name="^If it is not NULL, SQLite will invoke the destructor">^If it is not NULL, SQLite will invoke the destructor</param>
///<param name="function given by the 4th parameter to sqlite3_set_auxdata() on">function given by the 4th parameter to sqlite3_set_auxdata() on</param>
///<param name="the metadata when the corresponding function parameter changes">the metadata when the corresponding function parameter changes</param>
///<param name="or when the SQL statement completes, whichever comes first.">or when the SQL statement completes, whichever comes first.</param>
///<param name=""></param>
///<param name="SQLite is free to call the destructor and drop metadata on any">SQLite is free to call the destructor and drop metadata on any</param>
///<param name="parameter of any function at any time.  ^The only guarantee is that">parameter of any function at any time.  ^The only guarantee is that</param>
///<param name="the destructor will be called before the metadata is dropped.">the destructor will be called before the metadata is dropped.</param>
///<param name=""></param>
///<param name="^(In practice, metadata is preserved between function calls for">^(In practice, metadata is preserved between function calls for</param>
///<param name="expressions that are constant at compile time. This includes literal">expressions that are constant at compile time. This includes literal</param>
///<param name="values and [parameters].)^">values and [parameters].)^</param>
///<param name=""></param>
///<param name="These routines must be called from the same thread in which">These routines must be called from the same thread in which</param>
///<param name="the SQL function is running.">the SQL function is running.</param>
///<param name=""></param>

		//SQLITE_API void *sqlite3_get_auxdata(sqlite3_context*, int N);
		//SQLITE_API void sqlite3_set_auxdata(sqlite3_context*, int N, void*, object  ()(void));
		///
///<summary>
///CAPI3REF: Constants Defining Special Destructor Behavior
///
///These are special values for the destructor that is passed in as the
///final argument to routines like [sqlite3_result_blob()].  ^If the destructor
///argument is SQLITE_STATIC, it means that the content pointer is constant
///and will never change.  It does not need to be destroyed.  ^The
///SQLITE_TRANSIENT value means that the content will likely change in
///the near future and that SQLite should make its own public copy of
///the content before returning.
///
///The typedef is necessary to work around problems in certain
///C++ compilers.  See ticket #2191.
///
///</summary>

		//typedef void (*sqlite3_destructor_type)(void);
		//#define SQLITE_STATIC      ((sqlite3_destructor_type)0)
		//#define SQLITE_TRANSIENT   ((sqlite3_destructor_type)-1)
		static public dxDel SQLITE_STATIC;

		static public dxDel SQLITE_TRANSIENT;

		///
///<summary>
///CAPI3REF: Setting The Result Of An SQL Function
///
///These routines are used by the xFunc or xFinal callbacks that
///implement SQL functions and aggregates.  See
///[sqlite3_create_function()] and [sqlite3_create_function16()]
///for additional information.
///
///These functions work very much like the [parameter binding] family of
///functions used to bind values to host parameters in prepared statements.
///Refer to the [SQL parameter] documentation for additional information.
///
///^The sqlite3_result_blob() interface sets the result from
///</summary>
///<param name="an application">defined function to be the BLOB whose content is pointed</param>
///<param name="to by the second parameter and which is N bytes long where N is the">to by the second parameter and which is N bytes long where N is the</param>
///<param name="third parameter.">third parameter.</param>
///<param name=""></param>
///<param name="^The sqlite3_result_zeroblob() interfaces set the result of">^The sqlite3_result_zeroblob() interfaces set the result of</param>
///<param name="the application">defined function to be a BLOB containing all zero</param>
///<param name="bytes and N bytes in size, where N is the value of the 2nd parameter.">bytes and N bytes in size, where N is the value of the 2nd parameter.</param>
///<param name=""></param>
///<param name="^The sqlite3_result_double() interface sets the result from">^The sqlite3_result_double() interface sets the result from</param>
///<param name="an application">defined function to be a floating point value specified</param>
///<param name="by its 2nd argument.">by its 2nd argument.</param>
///<param name=""></param>
///<param name="^The sqlite3_result_error() and sqlite3_result_error16() functions">^The sqlite3_result_error() and sqlite3_result_error16() functions</param>
///<param name="cause the implemented SQL function to throw an exception.">cause the implemented SQL function to throw an exception.</param>
///<param name="^SQLite uses the string pointed to by the">^SQLite uses the string pointed to by the</param>
///<param name="2nd parameter of sqlite3_result_error() or sqlite3_result_error16()">2nd parameter of sqlite3_result_error() or sqlite3_result_error16()</param>
///<param name="as the text of an error message.  ^SQLite interprets the error">as the text of an error message.  ^SQLite interprets the error</param>
///<param name="message string from sqlite3_result_error() as UTF">8. ^SQLite</param>
///<param name="interprets the string from sqlite3_result_error16() as UTF">16 in native</param>
///<param name="byte order.  ^If the third parameter to sqlite3_result_error()">byte order.  ^If the third parameter to sqlite3_result_error()</param>
///<param name="or sqlite3_result_error16() is negative then SQLite takes as the error">or sqlite3_result_error16() is negative then SQLite takes as the error</param>
///<param name="message all text up through the first zero character.">message all text up through the first zero character.</param>
///<param name="^If the third parameter to sqlite3_result_error() or">^If the third parameter to sqlite3_result_error() or</param>
///<param name="sqlite3_result_error16() is non">negative then SQLite takes that many</param>
///<param name="bytes (not characters) from the 2nd parameter as the error message.">bytes (not characters) from the 2nd parameter as the error message.</param>
///<param name="^The sqlite3_result_error() and sqlite3_result_error16()">^The sqlite3_result_error() and sqlite3_result_error16()</param>
///<param name="routines make a public copy of the error message text before">routines make a public copy of the error message text before</param>
///<param name="they return.  Hence, the calling function can deallocate or">they return.  Hence, the calling function can deallocate or</param>
///<param name="modify the text after they return without harm.">modify the text after they return without harm.</param>
///<param name="^The sqlite3_result_error_code() function changes the error code">^The sqlite3_result_error_code() function changes the error code</param>
///<param name="returned by SQLite as a result of an error in a function.  ^By default,">returned by SQLite as a result of an error in a function.  ^By default,</param>
///<param name="the error code is SqlResult.SQLITE_ERROR.  ^A subsequent call to sqlite3_result_error()">the error code is SqlResult.SQLITE_ERROR.  ^A subsequent call to sqlite3_result_error()</param>
///<param name="or sqlite3_result_error16() resets the error code to SqlResult.SQLITE_ERROR.">or sqlite3_result_error16() resets the error code to SqlResult.SQLITE_ERROR.</param>
///<param name=""></param>
///<param name="^The sqlite3_result_toobig() interface causes SQLite to throw an error">^The sqlite3_result_toobig() interface causes SQLite to throw an error</param>
///<param name="indicating that a string or BLOB is too long to represent.">indicating that a string or BLOB is too long to represent.</param>
///<param name=""></param>
///<param name="^The sqlite3_result_nomem() interface causes SQLite to throw an error">^The sqlite3_result_nomem() interface causes SQLite to throw an error</param>
///<param name="indicating that a memory allocation failed.">indicating that a memory allocation failed.</param>
///<param name=""></param>
///<param name="^The sqlite3_result_int() interface sets the return value">^The sqlite3_result_int() interface sets the return value</param>
///<param name="of the application">bit signed integer</param>
///<param name="value given in the 2nd argument.">value given in the 2nd argument.</param>
///<param name="^The sqlite3_result_int64() interface sets the return value">^The sqlite3_result_int64() interface sets the return value</param>
///<param name="of the application">bit signed integer</param>
///<param name="value given in the 2nd argument.">value given in the 2nd argument.</param>
///<param name=""></param>
///<param name="^The sqlite3_result_null() interface sets the return value">^The sqlite3_result_null() interface sets the return value</param>
///<param name="of the application">defined function to be NULL.</param>
///<param name=""></param>
///<param name="^The sqlite3_result_text(), sqlite3_result_text16(),">^The sqlite3_result_text(), sqlite3_result_text16(),</param>
///<param name="sqlite3_result_text16le(), and sqlite3_result_text16be() interfaces">sqlite3_result_text16le(), and sqlite3_result_text16be() interfaces</param>
///<param name="set the return value of the application">defined function to be</param>
///<param name="a text string which is represented as UTF">16 native byte order,</param>
///<param name="UTF">16 big endian, respectively.</param>
///<param name="^SQLite takes the text result from the application from">^SQLite takes the text result from the application from</param>
///<param name="the 2nd parameter of the sqlite3_result_text* interfaces.">the 2nd parameter of the sqlite3_result_text* interfaces.</param>
///<param name="^If the 3rd parameter to the sqlite3_result_text* interfaces">^If the 3rd parameter to the sqlite3_result_text* interfaces</param>
///<param name="is negative, then SQLite takes result text from the 2nd parameter">is negative, then SQLite takes result text from the 2nd parameter</param>
///<param name="through the first zero character.">through the first zero character.</param>
///<param name="^If the 3rd parameter to the sqlite3_result_text* interfaces">^If the 3rd parameter to the sqlite3_result_text* interfaces</param>
///<param name="is non">negative, then as many bytes (not characters) of the text</param>
///<param name="pointed to by the 2nd parameter are taken as the application">defined</param>
///<param name="function result.">function result.</param>
///<param name="^If the 4th parameter to the sqlite3_result_text* interfaces">^If the 4th parameter to the sqlite3_result_text* interfaces</param>
///<param name="or sqlite3_result_blob is a non">NULL pointer, then SQLite calls that</param>
///<param name="function as the destructor on the text or BLOB result when it has">function as the destructor on the text or BLOB result when it has</param>
///<param name="finished using that result.">finished using that result.</param>
///<param name="^If the 4th parameter to the sqlite3_result_text* interfaces or to">^If the 4th parameter to the sqlite3_result_text* interfaces or to</param>
///<param name="sqlite3_result_blob is the special constant SQLITE_STATIC, then SQLite">sqlite3_result_blob is the special constant SQLITE_STATIC, then SQLite</param>
///<param name="assumes that the text or BLOB result is in constant space and does not">assumes that the text or BLOB result is in constant space and does not</param>
///<param name="copy the content of the parameter nor call a destructor on the content">copy the content of the parameter nor call a destructor on the content</param>
///<param name="when it has finished using that result.">when it has finished using that result.</param>
///<param name="^If the 4th parameter to the sqlite3_result_text* interfaces">^If the 4th parameter to the sqlite3_result_text* interfaces</param>
///<param name="or sqlite3_result_blob is the special constant SQLITE_TRANSIENT">or sqlite3_result_blob is the special constant SQLITE_TRANSIENT</param>
///<param name="then SQLite makes a copy of the result into space obtained from">then SQLite makes a copy of the result into space obtained from</param>
///<param name="from [sqlite3_malloc()] before it returns.">from [sqlite3_malloc()] before it returns.</param>
///<param name=""></param>
///<param name="^The sqlite3_result_value() interface sets the result of">^The sqlite3_result_value() interface sets the result of</param>
///<param name="the application">defined function to be a copy the</param>
///<param name="[unprotected sqlite3_value] object specified by the 2nd parameter.  ^The">[unprotected sqlite3_value] object specified by the 2nd parameter.  ^The</param>
///<param name="sqlite3_result_value() interface makes a copy of the [sqlite3_value]">sqlite3_result_value() interface makes a copy of the [sqlite3_value]</param>
///<param name="so that the [sqlite3_value] specified in the parameter may change or">so that the [sqlite3_value] specified in the parameter may change or</param>
///<param name="be deallocated after sqlite3_result_value() returns without harm.">be deallocated after sqlite3_result_value() returns without harm.</param>
///<param name="^A [protected sqlite3_value] object may always be used where an">^A [protected sqlite3_value] object may always be used where an</param>
///<param name="[unprotected sqlite3_value] object is required, so either">[unprotected sqlite3_value] object is required, so either</param>
///<param name="kind of [sqlite3_value] object can be used with this interface.">kind of [sqlite3_value] object can be used with this interface.</param>
///<param name=""></param>
///<param name="If these routines are called from within the different thread">If these routines are called from within the different thread</param>
///<param name="than the one containing the application">defined function that received</param>
///<param name="the [sqlite3_context] pointer, the results are undefined.">the [sqlite3_context] pointer, the results are undefined.</param>
///<param name=""></param>

		//SQLITE_API void sqlite3_result_blob(sqlite3_context*, const void*, int, void()(void));
		//SQLITE_API void sqlite3_result_double(sqlite3_context*, double);
		//SQLITE_API void sqlite3_result_error(sqlite3_context*, const char*, int);
		//SQLITE_API void sqlite3_result_error16(sqlite3_context*, const void*, int);
		//SQLITE_API void sqlite3_result_error_toobig(sqlite3_context);
		//SQLITE_API void sqlite3_result_error_nomem(sqlite3_context);
		//SQLITE_API void sqlite3_result_error_code(sqlite3_context*, int);
		//SQLITE_API void sqlite3_result_int(sqlite3_context*, int);
		//SQLITE_API void sqlite3_result_int64(sqlite3_context*, sqlite3_int64);
		//SQLITE_API void sqlite3_result_null(sqlite3_context);
		//SQLITE_API void sqlite3_result_text(sqlite3_context*, const char*, int, void()(void));
		//SQLITE_API void sqlite3_result_text16(sqlite3_context*, const void*, int, void()(void));
		//SQLITE_API void sqlite3_result_text16le(sqlite3_context*, const void*, int,void()(void));
		//SQLITE_API void sqlite3_result_text16be(sqlite3_context*, const void*, int,void()(void));
		//SQLITE_API void sqlite3_result_value(sqlite3_context*, sqlite3_value);
		//SQLITE_API void sqlite3_result_zeroblob(sqlite3_context*, int n);
		///
///<summary>
///CAPI3REF: Define New Collating Sequences
///
///^These functions add, remove, or modify a [collation] associated
///with the [database connection] specified as the first argument.
///
///</summary>
///<param name="^The name of the collation is a UTF">8 string</param>
///<param name="for sqlite3_create_collation() and sqlite3_create_collation_v2()">for sqlite3_create_collation() and sqlite3_create_collation_v2()</param>
///<param name="and a UTF">16 string in native byte order for sqlite3_create_collation16().</param>
///<param name="^Collation names that compare equal according to [sqlite3_strnicmp()] are">^Collation names that compare equal according to [sqlite3_strnicmp()] are</param>
///<param name="considered to be the same name.">considered to be the same name.</param>
///<param name=""></param>
///<param name="^(The third argument (eTextRep) must be one of the constants:">^(The third argument (eTextRep) must be one of the constants:</param>
///<param name="<ul>"><ul></param>
///<param name="<li> [SQLITE_UTF8],"><li> [SQLITE_UTF8],</param>
///<param name="<li> [SQLITE_UTF16LE],"><li> [SQLITE_UTF16LE],</param>
///<param name="<li> [SQLITE_UTF16BE],"><li> [SQLITE_UTF16BE],</param>
///<param name="<li> [SQLITE_UTF16], or"><li> [SQLITE_UTF16], or</param>
///<param name="<li> [SQLITE_UTF16_ALIGNED]."><li> [SQLITE_UTF16_ALIGNED].</param>
///<param name="</ul>)^"></ul>)^</param>
///<param name="^The eTextRep argument determines the encoding of strings passed">^The eTextRep argument determines the encoding of strings passed</param>
///<param name="to the collating function callback, xCallback.">to the collating function callback, xCallback.</param>
///<param name="^The [SQLITE_UTF16] and [SQLITE_UTF16_ALIGNED] values for eTextRep">^The [SQLITE_UTF16] and [SQLITE_UTF16_ALIGNED] values for eTextRep</param>
///<param name="force strings to be UTF16 with native byte order.">force strings to be UTF16 with native byte order.</param>
///<param name="^The [SQLITE_UTF16_ALIGNED] value for eTextRep forces strings to begin">^The [SQLITE_UTF16_ALIGNED] value for eTextRep forces strings to begin</param>
///<param name="on an even byte address.">on an even byte address.</param>
///<param name=""></param>
///<param name="^The fourth argument, pArg, is an application data pointer that is passed">^The fourth argument, pArg, is an application data pointer that is passed</param>
///<param name="through as the first argument to the collating function callback.">through as the first argument to the collating function callback.</param>
///<param name=""></param>
///<param name="^The fifth argument, xCallback, is a pointer to the collating function.">^The fifth argument, xCallback, is a pointer to the collating function.</param>
///<param name="^Multiple collating functions can be registered using the same name but">^Multiple collating functions can be registered using the same name but</param>
///<param name="with different eTextRep parameters and SQLite will use whichever">with different eTextRep parameters and SQLite will use whichever</param>
///<param name="function requires the least amount of data transformation.">function requires the least amount of data transformation.</param>
///<param name="^If the xCallback argument is NULL then the collating function is">^If the xCallback argument is NULL then the collating function is</param>
///<param name="deleted.  ^When all collating functions having the same name are deleted,">deleted.  ^When all collating functions having the same name are deleted,</param>
///<param name="that collation is no longer usable.">that collation is no longer usable.</param>
///<param name=""></param>
///<param name="^The collating function callback is invoked with a copy of the pArg ">^The collating function callback is invoked with a copy of the pArg </param>
///<param name="application data pointer and with two strings in the encoding specified">application data pointer and with two strings in the encoding specified</param>
///<param name="by the eTextRep argument.  The collating function must return an">by the eTextRep argument.  The collating function must return an</param>
///<param name="integer that is negative, zero, or positive">integer that is negative, zero, or positive</param>
///<param name="if the first string is less than, equal to, or greater than the second,">if the first string is less than, equal to, or greater than the second,</param>
///<param name="respectively.  A collating function must always return the same answer">respectively.  A collating function must always return the same answer</param>
///<param name="given the same inputs.  If two or more collating functions are registered">given the same inputs.  If two or more collating functions are registered</param>
///<param name="to the same collation name (using different eTextRep values) then all">to the same collation name (using different eTextRep values) then all</param>
///<param name="must give an equivalent answer when invoked with equivalent strings.">must give an equivalent answer when invoked with equivalent strings.</param>
///<param name="The collating function must obey the following properties for all">The collating function must obey the following properties for all</param>
///<param name="strings A, B, and C:">strings A, B, and C:</param>
///<param name=""></param>
///<param name="<ol>"><ol></param>
///<param name="<li> If A==B then B==A."><li> If A==B then B==A.</param>
///<param name="<li> If A==B and B==C then A==C."><li> If A==B and B==C then A==C.</param>
///<param name="<li> If A&lt;B THEN B&gt;A."><li> If A&lt;B THEN B&gt;A.</param>
///<param name="<li> If A&lt;B and B&lt;C then A&lt;C."><li> If A&lt;B and B&lt;C then A&lt;C.</param>
///<param name="</ol>"></ol></param>
///<param name=""></param>
///<param name="If a collating function fails any of the above constraints and that">If a collating function fails any of the above constraints and that</param>
///<param name="collating function is  registered and used, then the behavior of SQLite">collating function is  registered and used, then the behavior of SQLite</param>
///<param name="is undefined.">is undefined.</param>
///<param name=""></param>
///<param name="^The sqlite3_create_collation_v2() works like sqlite3_create_collation()">^The sqlite3_create_collation_v2() works like sqlite3_create_collation()</param>
///<param name="with the addition that the xDestroy callback is invoked on pArg when">with the addition that the xDestroy callback is invoked on pArg when</param>
///<param name="the collating function is deleted.">the collating function is deleted.</param>
///<param name="^Collating functions are deleted when they are overridden by later">^Collating functions are deleted when they are overridden by later</param>
///<param name="calls to the collation creation functions or when the">calls to the collation creation functions or when the</param>
///<param name="[database connection] is closed using [sqlite3_close()].">[database connection] is closed using [sqlite3_close()].</param>
///<param name=""></param>
///<param name="^The xDestroy callback is <u>not</u> called if the ">^The xDestroy callback is <u>not</u> called if the </param>
///<param name="sqlite3_create_collation_v2() function fails.  Applications that invoke">sqlite3_create_collation_v2() function fails.  Applications that invoke</param>
///<param name="sqlite3_create_collation_v2() with a non">NULL xDestroy argument should </param>
///<param name="check the return code and dispose of the application data pointer">check the return code and dispose of the application data pointer</param>
///<param name="themselves rather than expecting SQLite to deal with it for them.">themselves rather than expecting SQLite to deal with it for them.</param>
///<param name="This is different from every other SQLite interface.  The inconsistency ">This is different from every other SQLite interface.  The inconsistency </param>
///<param name="is unfortunate but cannot be changed without breaking backwards ">is unfortunate but cannot be changed without breaking backwards </param>
///<param name="compatibility.">compatibility.</param>
///<param name=""></param>
///<param name="See also:  [sqlite3_collation_needed()] and [sqlite3_collation_needed16()].">See also:  [sqlite3_collation_needed()] and [sqlite3_collation_needed16()].</param>
///<param name=""></param>

		//SQLITE_API int sqlite3_create_collation(
		//  sqlite3*, 
		//  string zName, 
		//  int eTextRep, 
		//  void *pArg,
		//  int(*xCompare)(void*,int,const void*,int,const void)
		//);
		//SQLITE_API int sqlite3_create_collation_v2(
		//  sqlite3*, 
		//  string zName, 
		//  int eTextRep, 
		//  void *pArg,
		//  int(*xCompare)(void*,int,const void*,int,const void),
		//  void(*xDestroy)(void)
		//);
		//SQLITE_API int sqlite3_create_collation16(
		//  sqlite3*, 
		//  string zName,
		//  int eTextRep, 
		//  void *pArg,
		//  int(*xCompare)(void*,int,const void*,int,const void)
		//);
		///
///<summary>
///CAPI3REF: Collation Needed Callbacks
///
///^To avoid having to register all collation sequences before a database
///can be used, a single callback function may be registered with the
///[database connection] to be invoked whenever an undefined collation
///sequence is required.
///
///^If the function is registered using the sqlite3_collation_needed() API,
///then it is passed the names of undefined collation sequences as strings
///</summary>
///<param name="encoded in UTF">8. ^If sqlite3_collation_needed16() is used,</param>
///<param name="the names are passed as UTF">16 in machine native byte order.</param>
///<param name="^A call to either function replaces the existing collation">needed callback.</param>
///<param name=""></param>
///<param name="^(When the callback is invoked, the first argument passed is a copy">^(When the callback is invoked, the first argument passed is a copy</param>
///<param name="of the second argument to sqlite3_collation_needed() or">of the second argument to sqlite3_collation_needed() or</param>
///<param name="sqlite3_collation_needed16().  The second argument is the database">sqlite3_collation_needed16().  The second argument is the database</param>
///<param name="connection.  The third argument is one of [SQLITE_UTF8], [SQLITE_UTF16BE],">connection.  The third argument is one of [SQLITE_UTF8], [SQLITE_UTF16BE],</param>
///<param name="or [SQLITE_UTF16LE], indicating the most desirable form of the collation">or [SQLITE_UTF16LE], indicating the most desirable form of the collation</param>
///<param name="sequence function required.  The fourth parameter is the name of the">sequence function required.  The fourth parameter is the name of the</param>
///<param name="required collation sequence.)^">required collation sequence.)^</param>
///<param name=""></param>
///<param name="The callback function should register the desired collation using">The callback function should register the desired collation using</param>
///<param name="[sqlite3_create_collation()], [sqlite3_create_collation16()], or">[sqlite3_create_collation()], [sqlite3_create_collation16()], or</param>
///<param name="[sqlite3_create_collation_v2()].">[sqlite3_create_collation_v2()].</param>
///<param name=""></param>

		//SQLITE_API int sqlite3_collation_needed(
		//  sqlite3*, 
		//  void*, 
		//  void()(void*,sqlite3*,int eTextRep,const char)
		//);
		//SQLITE_API int sqlite3_collation_needed16(
		//  sqlite3*, 
		//  void*,
		//  void()(void*,sqlite3*,int eTextRep,const void)
		//);
		//#if SQLITE_HAS_CODEC
		///
///<summary>
///Specify the key for an encrypted database.  This routine should be
///called right after sqlite3_open().
///
///The code to implement this API is not available in the public release
///of SQLite.
///
///</summary>

		//SQLITE_API int sqlite3_key(
		//  sqlite3 db,                   /* Database to be rekeyed */
		//  const void *pKey, int nKey     /* The key */
		//);
		///
///<summary>
///Change the key on an open database.  If the current database is not
///encrypted, this routine will encrypt it.  If pNew==0 or nNew==0, the
///database is decrypted.
///
///The code to implement this API is not available in the public release
///of SQLite.
///
///</summary>

		//SQLITE_API int sqlite3_rekey(
		//  sqlite3 db,                   /* Database to be rekeyed */
		//  const void *pKey, int nKey     /* The new key */
		//);
		///
///<summary>
///Specify the activation key for a SEE database.  Unless 
///activated, none of the SEE routines will work.
///
///</summary>

		//SQLITE_API void sqlite3_activate_see(
		//  string zPassPhrase        /* Activation phrase */
		//);
		//#endif
		//#if SQLITE_ENABLE_CEROD
		///
///<summary>
///Specify the activation key for a CEROD database.  Unless 
///activated, none of the CEROD routines will work.
///
///</summary>

		//SQLITE_API void sqlite3_activate_cerod(
		//  string zPassPhrase        /* Activation phrase */
		//);
		//#endif
		///
///<summary>
///CAPI3REF: Suspend Execution For A Short Time
///
///The sqlite3_sleep() function causes the current thread to suspend execution
///for at least a number of milliseconds specified in its parameter.
///
///If the operating system does not support sleep requests with
///millisecond time resolution, then the time will be rounded up to
///the nearest second. The number of milliseconds of sleep actually
///requested from the operating system is returned.
///
///^SQLite implements this interface by calling the xSleep()
///method of the default [sqlite3_vfs] object.  If the xSleep() method
///of the default VFS is not implemented correctly, or not implemented at
///all, then the behavior of sqlite3_sleep() may deviate from the description
///in the previous paragraphs.
///
///</summary>

		//SQLITE_API int sqlite3_sleep(int);
		///
///<summary>
///CAPI3REF: Name Of The Folder Holding Temporary Files
///
///^(If this global variable is made to point to a string which is
///the name of a folder (a.k.a. directory), then all temporary files
///</summary>
///<param name="created by SQLite when using a built">in [sqlite3_vfs | VFS]</param>
///<param name="will be placed in that directory.)^  ^If this variable">will be placed in that directory.)^  ^If this variable</param>
///<param name="is a NULL pointer, then SQLite performs a search for an appropriate">is a NULL pointer, then SQLite performs a search for an appropriate</param>
///<param name="temporary file directory.">temporary file directory.</param>
///<param name=""></param>
///<param name="It is not safe to read or modify this variable in more than one">It is not safe to read or modify this variable in more than one</param>
///<param name="thread at a time.  It is not safe to read or modify this variable">thread at a time.  It is not safe to read or modify this variable</param>
///<param name="if a [database connection] is being used at the same time in a separate">if a [database connection] is being used at the same time in a separate</param>
///<param name="thread.">thread.</param>
///<param name="It is intended that this variable be set once">It is intended that this variable be set once</param>
///<param name="as part of process initialization and before any SQLite interface">as part of process initialization and before any SQLite interface</param>
///<param name="routines have been called and that this variable remain unchanged">routines have been called and that this variable remain unchanged</param>
///<param name="thereafter.">thereafter.</param>
///<param name=""></param>
///<param name="^The [temp_store_directory pragma] may modify this variable and cause">^The [temp_store_directory pragma] may modify this variable and cause</param>
///<param name="it to point to memory obtained from [sqlite3_malloc].  ^Furthermore,">it to point to memory obtained from [sqlite3_malloc].  ^Furthermore,</param>
///<param name="the [temp_store_directory pragma] always assumes that any string">the [temp_store_directory pragma] always assumes that any string</param>
///<param name="that this variable points to is held in memory obtained from ">that this variable points to is held in memory obtained from </param>
///<param name="[sqlite3_malloc] and the pragma may attempt to free that memory">[sqlite3_malloc] and the pragma may attempt to free that memory</param>
///<param name="using [malloc_cs.sqlite3_free].">using [malloc_cs.sqlite3_free].</param>
///<param name="Hence, if this variable is modified directly, either it should be">Hence, if this variable is modified directly, either it should be</param>
///<param name="made NULL or made to point to memory obtained from [sqlite3_malloc]">made NULL or made to point to memory obtained from [sqlite3_malloc]</param>
///<param name="or else the use of the [temp_store_directory pragma] should be avoided.">or else the use of the [temp_store_directory pragma] should be avoided.</param>
///<param name=""></param>

		//SQLITE_API SQLITE_EXTERN char *sqlite3_temp_directory;
		///
///<summary>
///</summary>
///<param name="CAPI3REF: Test For Auto">Commit Mode</param>
///<param name="KEYWORDS: {autocommit mode}">KEYWORDS: {autocommit mode}</param>
///<param name=""></param>
///<param name="^The sqlite3_get_autocommit() interface returns non">zero or</param>
///<param name="zero if the given database connection is or is not in autocommit mode,">zero if the given database connection is or is not in autocommit mode,</param>
///<param name="respectively.  ^Autocommit mode is on by default.">respectively.  ^Autocommit mode is on by default.</param>
///<param name="^Autocommit mode is disabled by a [BEGIN] statement.">^Autocommit mode is disabled by a [BEGIN] statement.</param>
///<param name="^Autocommit mode is re">enabled by a [COMMIT] or [ROLLBACK].</param>
///<param name=""></param>
///<param name="If certain kinds of errors occur on a statement within a multi">statement</param>
///<param name="transaction (errors including [SQLITE_FULL], [SQLITE_IOERR],">transaction (errors including [SQLITE_FULL], [SQLITE_IOERR],</param>
///<param name="[SQLITE_NOMEM], [SQLITE_BUSY], and [SQLITE_INTERRUPT]) then the">[SQLITE_NOMEM], [SQLITE_BUSY], and [SQLITE_INTERRUPT]) then the</param>
///<param name="transaction might be rolled back automatically.  The only way to">transaction might be rolled back automatically.  The only way to</param>
///<param name="find out whether SQLite automatically rolled back the transaction after">find out whether SQLite automatically rolled back the transaction after</param>
///<param name="an error is to use this function.">an error is to use this function.</param>
///<param name=""></param>
///<param name="If another thread changes the autocommit status of the database">If another thread changes the autocommit status of the database</param>
///<param name="connection while this routine is running, then the return value">connection while this routine is running, then the return value</param>
///<param name="is undefined.">is undefined.</param>
///<param name=""></param>

		//SQLITE_API int sqlite3_get_autocommit(sqlite3);
		///
///<summary>
///CAPI3REF: Find The Database Handle Of A Prepared Statement
///
///^The sqlite3_db_handle interface returns the [database connection] handle
///to which a [prepared statement] belongs.  ^The [database connection]
///returned by sqlite3_db_handle is the same [database connection]
///that was the first argument
///to the [sqlite3_prepare_v2()] call (or its variants) that was used to
///create the statement in the first place.
///
///</summary>

		//SQLITE_API sqlite3 *sqlite3_db_handle(sqlite3_stmt);
		///
///<summary>
///CAPI3REF: Find the next prepared statement
///
///^This interface returns a pointer to the next [prepared statement] after
///pStmt associated with the [database connection] pDb.  ^If pStmt is NULL
///then this interface returns a pointer to the first prepared statement
///associated with the database connection pDb.  ^If no prepared statement
///satisfies the conditions of this routine, it returns NULL.
///
///The [database connection] pointer D in a call to
///[sqlite3_next_stmt(D,S)] must refer to an open database
///connection and in particular must not be a NULL pointer.
///
///</summary>

		//SQLITE_API sqlite3_stmt *sqlite3_next_stmt(sqlite3 *pDb, sqlite3_stmt *pStmt);
		///
///<summary>
///CAPI3REF: Commit And Rollback Notification Callbacks
///
///^The sqlite3_commit_hook() interface registers a callback
///function to be invoked whenever a transaction is [COMMIT | committed].
///^Any callback set by a previous call to sqlite3_commit_hook()
///for the same database connection is overridden.
///^The sqlite3_rollback_hook() interface registers a callback
///function to be invoked whenever a transaction is [ROLLBACK | rolled back].
///^Any callback set by a previous call to sqlite3_rollback_hook()
///for the same database connection is overridden.
///^The pArg argument is passed through to the callback.
///</summary>
///<param name="^If the callback on a commit hook function returns non">zero,</param>
///<param name="then the commit is converted into a rollback.">then the commit is converted into a rollback.</param>
///<param name=""></param>
///<param name="^The sqlite3_commit_hook(D,C,P) and sqlite3_rollback_hook(D,C,P) functions">^The sqlite3_commit_hook(D,C,P) and sqlite3_rollback_hook(D,C,P) functions</param>
///<param name="return the P argument from the previous call of the same function">return the P argument from the previous call of the same function</param>
///<param name="on the same [database connection] D, or NULL for">on the same [database connection] D, or NULL for</param>
///<param name="the first call for each function on D.">the first call for each function on D.</param>
///<param name=""></param>
///<param name="The callback implementation must not do anything that will modify">The callback implementation must not do anything that will modify</param>
///<param name="the database connection that invoked the callback.  Any actions">the database connection that invoked the callback.  Any actions</param>
///<param name="to modify the database connection must be deferred until after the">to modify the database connection must be deferred until after the</param>
///<param name="completion of the [sqlite3_step()] call that triggered the commit">completion of the [sqlite3_step()] call that triggered the commit</param>
///<param name="or rollback hook in the first place.">or rollback hook in the first place.</param>
///<param name="Note that [sqlite3_prepare_v2()] and [sqlite3_step()] both modify their">Note that [sqlite3_prepare_v2()] and [sqlite3_step()] both modify their</param>
///<param name="database connections for the meaning of "modify" in this paragraph.">database connections for the meaning of "modify" in this paragraph.</param>
///<param name=""></param>
///<param name="^Registering a NULL function disables the callback.">^Registering a NULL function disables the callback.</param>
///<param name=""></param>
///<param name="^When the commit hook callback routine returns zero, the [COMMIT]">^When the commit hook callback routine returns zero, the [COMMIT]</param>
///<param name="operation is allowed to continue normally.  ^If the commit hook">operation is allowed to continue normally.  ^If the commit hook</param>
///<param name="returns non">zero, then the [COMMIT] is converted into a [ROLLBACK].</param>
///<param name="^The rollback hook is invoked on a rollback that results from a commit">^The rollback hook is invoked on a rollback that results from a commit</param>
///<param name="hook returning non">zero, just as it would be with any other rollback.</param>
///<param name=""></param>
///<param name="^For the purposes of this API, a transaction is said to have been">^For the purposes of this API, a transaction is said to have been</param>
///<param name="rolled back if an explicit "ROLLBACK" statement is executed, or">rolled back if an explicit "ROLLBACK" statement is executed, or</param>
///<param name="an error or constraint causes an implicit rollback to occur.">an error or constraint causes an implicit rollback to occur.</param>
///<param name="^The rollback callback is not invoked if a transaction is">^The rollback callback is not invoked if a transaction is</param>
///<param name="automatically rolled back because the database connection is closed.">automatically rolled back because the database connection is closed.</param>
///<param name=""></param>
///<param name="See also the [sqlite3_update_hook()] interface.">See also the [sqlite3_update_hook()] interface.</param>
///<param name=""></param>

		//SQLITE_API void *sqlite3_commit_hook(sqlite3*, int()(void), void);
		//SQLITE_API void *sqlite3_rollback_hook(sqlite3*, void()(void ), void);
		///
///<summary>
///CAPI3REF: Data Change Notification Callbacks
///
///^The sqlite3_update_hook() interface registers a callback function
///with the [database connection] identified by the first argument
///to be invoked whenever a row is updated, inserted or deleted.
///^Any callback set by a previous call to this function
///for the same database connection is overridden.
///
///^The second argument is a pointer to the function to invoke when a
///row is updated, inserted or deleted.
///^The first argument to the callback is a copy of the third argument
///to sqlite3_update_hook().
///^The second callback argument is one of [SQLITE_INSERT], [SQLITE_DELETE],
///or [SQLITE_UPDATE], depending on the operation that caused the callback
///to be invoked.
///^The third and fourth arguments to the callback contain pointers to the
///database and table name containing the affected row.
///^The final callback parameter is the [rowid] of the row.
///^In the case of an update, this is the [rowid] after the update takes place.
///
///^(The update hook is not invoked when internal system tables are
///modified (i.e. sqlite_master and sqlite_sequence).)^
///
///^In the current implementation, the update hook
///is not invoked when duplication rows are deleted because of an
///[ON CONFLICT | ON CONFLICT REPLACE] clause.  ^Nor is the update hook
///invoked when rows are deleted using the [truncate optimization].
///The exceptions defined in this paragraph might change in a future
///release of SQLite.
///
///The update hook implementation must not do anything that will modify
///the database connection that invoked the update hook.  Any actions
///to modify the database connection must be deferred until after the
///completion of the [sqlite3_step()] call that triggered the update hook.
///Note that [sqlite3_prepare_v2()] and [sqlite3_step()] both modify their
///database connections for the meaning of "modify" in this paragraph.
///
///^The sqlite3_update_hook(D,C,P) function
///returns the P argument from the previous call
///on the same [database connection] D, or NULL for
///the first call on D.
///
///See also the [sqlite3_commit_hook()] and [sqlite3_rollback_hook()]
///interfaces.
///
///</summary>

		//SQLITE_API void *sqlite3_update_hook(
		//  sqlite3*, 
		//  void()(void *,int ,char const *,char const *,sqlite3_int64),
		//  void*
		//);
		///
///<summary>
///CAPI3REF: Enable Or Disable Shared Pager Cache
///KEYWORDS: {shared cache}
///
///^(This routine enables or disables the sharing of the database cache
///and schema data structures between [database connection | connections]
///to the same database. Sharing is enabled if the argument is true
///and disabled if the argument is false.)^
///
///^Cache sharing is enabled and disabled for an entire process.
///This is a change as of SQLite version 3.5.0. In prior versions of SQLite,
///sharing was enabled or disabled for each thread separately.
///
///^(The cache sharing mode set by this interface effects all subsequent
///calls to [sqlite3_open()], [sqlite3_open_v2()], and [sqlite3_open16()].
///Existing database connections continue use the sharing mode
///that was in effect at the time they were opened.)^
///
///^(This routine returns [SqlResult.SQLITE_OK] if shared cache was enabled or disabled
///successfully.  An [error code] is returned otherwise.)^
///
///^Shared cache is disabled by default. But this might change in
///future releases of SQLite.  Applications that care about shared
///cache setting should set it explicitly.
///
///</summary>
///<param name="See Also:  [SQLite Shared">Cache Mode]</param>
///<param name=""></param>

		//SQLITE_API int sqlite3_enable_shared_cache(int);
		///
///<summary>
///CAPI3REF: Attempt To Free Heap Memory
///
///^The sqlite3_release_memory() interface attempts to free N bytes
///</summary>
///<param name="of heap memory by deallocating non">essential memory allocations</param>
///<param name="held by the database library.   Memory used to cache database">held by the database library.   Memory used to cache database</param>
///<param name="pages to improve performance is an example of non">essential memory.</param>
///<param name="^sqlite3_release_memory() returns the number of bytes actually freed,">^sqlite3_release_memory() returns the number of bytes actually freed,</param>
///<param name="which might be more or less than the amount requested.">which might be more or less than the amount requested.</param>
///<param name="^The sqlite3_release_memory() routine is a no">op returning zero</param>
///<param name="if SQLite is not compiled with [SQLITE_ENABLE_MEMORY_MANAGEMENT].">if SQLite is not compiled with [SQLITE_ENABLE_MEMORY_MANAGEMENT].</param>
///<param name=""></param>

		//SQLITE_API int sqlite3_release_memory(int);
		///
///<summary>
///CAPI3REF: Impose A Limit On Heap Size
///
///^The sqlite3_soft_heap_limit64() interface sets and/or queries the
///soft limit on the amount of heap memory that may be allocated by SQLite.
///^SQLite strives to keep heap memory utilization below the soft heap
///limit by reducing the number of pages held in the page cache
///as heap memory usages approaches the limit.
///^The soft heap limit is "soft" because even though SQLite strives to stay
///below the limit, it will exceed the limit rather than generate
///an [SQLITE_NOMEM] error.  In other words, the soft heap limit 
///is advisory only.
///
///^The return value from sqlite3_soft_heap_limit64() is the size of
///the soft heap limit prior to the call.  ^If the argument N is negative
///then no change is made to the soft heap limit.  Hence, the current
///size of the soft heap limit can be determined by invoking
///sqlite3_soft_heap_limit64() with a negative argument.
///
///^If the argument N is zero then the soft heap limit is disabled.
///
///^(The soft heap limit is not enforced in the current implementation
///if one or more of following conditions are true:
///
///<ul>
///<li> The soft heap limit is set to zero.
///<li> Memory accounting is disabled using a combination of the
///</summary>
///<param name="[sqlite3_config]([SQLITE_CONFIG_MEMSTATUS],...) start">time option and</param>
///<param name="the [SQLITE_DEFAULT_MEMSTATUS] compile">time option.</param>
///<param name="<li> An alternative page cache implementation is specified using"><li> An alternative page cache implementation is specified using</param>
///<param name="[sqlite3_config]([SQLITE_CONFIG_PCACHE],...).">[sqlite3_config]([SQLITE_CONFIG_PCACHE],...).</param>
///<param name="<li> The page cache allocates from its own memory pool supplied"><li> The page cache allocates from its own memory pool supplied</param>
///<param name="by [sqlite3_config]([SQLITE_CONFIG_PAGECACHE],...) rather than">by [sqlite3_config]([SQLITE_CONFIG_PAGECACHE],...) rather than</param>
///<param name="from the heap.">from the heap.</param>
///<param name="</ul>)^"></ul>)^</param>
///<param name=""></param>
///<param name="Beginning with SQLite version 3.7.3, the soft heap limit is enforced">Beginning with SQLite version 3.7.3, the soft heap limit is enforced</param>
///<param name="regardless of whether or not the [SQLITE_ENABLE_MEMORY_MANAGEMENT]">regardless of whether or not the [SQLITE_ENABLE_MEMORY_MANAGEMENT]</param>
///<param name="compile">time option is invoked.  With [SQLITE_ENABLE_MEMORY_MANAGEMENT],</param>
///<param name="the soft heap limit is enforced on every memory allocation.  Without">the soft heap limit is enforced on every memory allocation.  Without</param>
///<param name="[SQLITE_ENABLE_MEMORY_MANAGEMENT], the soft heap limit is only enforced">[SQLITE_ENABLE_MEMORY_MANAGEMENT], the soft heap limit is only enforced</param>
///<param name="when memory is allocated by the page cache.  Testing suggests that because">when memory is allocated by the page cache.  Testing suggests that because</param>
///<param name="the page cache is the predominate memory user in SQLite, most">the page cache is the predominate memory user in SQLite, most</param>
///<param name="applications will achieve adequate soft heap limit enforcement without">applications will achieve adequate soft heap limit enforcement without</param>
///<param name="the use of [SQLITE_ENABLE_MEMORY_MANAGEMENT].">the use of [SQLITE_ENABLE_MEMORY_MANAGEMENT].</param>
///<param name=""></param>
///<param name="The circumstances under which SQLite will enforce the soft heap limit may">The circumstances under which SQLite will enforce the soft heap limit may</param>
///<param name="changes in future releases of SQLite.">changes in future releases of SQLite.</param>
///<param name=""></param>

		//SQLITE_API sqlite3_int64 sqlite3_soft_heap_limit64(sqlite3_int64 N);
		///
///<summary>
///CAPI3REF: Deprecated Soft Heap Limit Interface
///DEPRECATED
///
///This is a deprecated version of the [sqlite3_soft_heap_limit64()]
///interface.  This routine is provided for historical compatibility
///only.  All new applications should use the
///[sqlite3_soft_heap_limit64()] interface rather than this one.
///
///</summary>

		//SQLITE_API SQLITE_DEPRECATED void sqlite3_soft_heap_limit(int N);
		///
///<summary>
///CAPI3REF: Extract Metadata About A Column Of A Table
///
///^This routine returns metadata about a specific column of a specific
///database table accessible using the [database connection] handle
///passed as the first function argument.
///
///^The column is identified by the second, third and fourth parameters to
///this function. ^The second parameter is either the name of the database
///(i.e. "main", "temp", or an attached database) containing the specified
///table or NULL. ^If it is NULL, then all attached databases are searched
///for the table using the same algorithm used by the database engine to
///resolve unqualified table references.
///
///^The third and fourth parameters to this function are the table and column
///name of the desired column, respectively. Neither of these parameters
///may be NULL.
///
///^Metadata is returned by writing to the memory locations passed as the 5th
///and subsequent parameters to this function. ^Any of these arguments may be
///NULL, in which case the corresponding element of metadata is omitted.
///
///^(<blockquote>
///<table border="1">
///<tr><th> Parameter <th> Output<br>Type <th>  Description
///
///<tr><td> 5th <td> const char* <td> Data type
///<tr><td> 6th <td> const char* <td> Name of default collation sequence
///<tr><td> 7th <td> int         <td> True if column has a NOT NULL constraint
///<tr><td> 8th <td> int         <td> True if column is part of the PRIMARY KEY
///<tr><td> 9th <td> int         <td> True if column is [AUTOINCREMENT]
///</table>
///</blockquote>)^
///
///^The memory pointed to by the character pointers returned for the
///declaration type and collation sequence is valid only until the next
///call to any SQLite API function.
///
///^If the specified table is actually a view, an [error code] is returned.
///
///^If the specified column is "rowid", "oid" or "_rowid_" and an
///[INTEGER PRIMARY KEY] column has been explicitly declared, then the output
///parameters are set for the explicitly declared column. ^(If there is no
///explicitly declared [INTEGER PRIMARY KEY] column, then the output
///parameters are set as follows:
///
///<pre>
///data type: "INTEGER"
///collation sequence: "BINARY"
///not null: 0
///primary key: 1
///auto increment: 0
///</pre>)^
///
///^(This function may load one or more schemas from database files. If an
///error occurs during this process, or if the requested table or column
///cannot be found, an [error code] is returned and an error message left
///in the [database connection] (to be retrieved using sqlite3_errmsg()).)^
///
///^This API is only available if the library was compiled with the
///</summary>
///<param name="[SQLITE_ENABLE_COLUMN_METADATA] C">preprocessor symbol defined.</param>
///<param name=""></param>

		//SQLITE_API int sqlite3_table_column_metadata(
		//  sqlite3 db,                /* Connection handle */
		//  string zDbName,        /* Database name or NULL */
		//  string zTableName,     /* Table name */
		//  string zColumnName,    /* Column name */
		//  char const **pzDataType,    /* OUTPUT: Declared data type */
		//  char const **pzCollSeq,     /* OUTPUT: Collation sequence name */
		//  int *pNotNull,              /* OUTPUT: True if NOT NULL constraint exists */
		//  int *pPrimaryKey,           /* OUTPUT: True if column part of PK */
		//  int *pAutoinc               /* OUTPUT: True if column is auto-increment */
		//);
		///
///<summary>
///CAPI3REF: Load An Extension
///
///^This interface loads an SQLite extension library from the named file.
///
///^The sqlite3_load_extension() interface attempts to load an
///SQLite extension library contained in the file zFile.
///
///^The entry point is zProc.
///^zProc may be 0, in which case the name of the entry point
///defaults to "sqlite3_extension_init".
///^The sqlite3_load_extension() interface returns
///[SqlResult.SQLITE_OK] on success and [SqlResult.SQLITE_ERROR] if something goes wrong.
///^If an error occurs and pzErrMsg is not 0, then the
///[sqlite3_load_extension()] interface shall attempt to
///fill *pzErrMsg with error message text stored in memory
///obtained from [sqlite3_malloc()]. The calling function
///should free this memory by calling [malloc_cs.sqlite3_free()].
///
///^Extension loading must be enabled using
///[sqlite3_enable_load_extension()] prior to calling this API,
///otherwise an error will be returned.
///
///See also the [load_extension() SQL function].
///
///</summary>

		//SQLITE_API int sqlite3_load_extension(
		//  sqlite3 db,          /* Load the extension into this database connection */
		//  string zFile,    /* Name of the shared library containing extension */
		//  string zProc,    /* Entry point.  Derived from zFile if 0 */
		//  char **pzErrMsg       /* Put error message here if not 0 */
		//);
		///
///<summary>
///CAPI3REF: Enable Or Disable Extension Loading
///
///^So as not to open security holes in older applications that are
///unprepared to deal with extension loading, and as a means of disabling
///</summary>
///<param name="extension loading while evaluating user">entered SQL, the following API</param>
///<param name="is provided to turn the [sqlite3_load_extension()] mechanism on and off.">is provided to turn the [sqlite3_load_extension()] mechanism on and off.</param>
///<param name=""></param>
///<param name="^Extension loading is off by default. See ticket #1863.">^Extension loading is off by default. See ticket #1863.</param>
///<param name="^Call the sqlite3_enable_load_extension() routine with onoff==1">^Call the sqlite3_enable_load_extension() routine with onoff==1</param>
///<param name="to turn extension loading on and call it with onoff==0 to turn">to turn extension loading on and call it with onoff==0 to turn</param>
///<param name="it back off again.">it back off again.</param>
///<param name=""></param>

		//SQLITE_API int sqlite3_enable_load_extension(sqlite3 db, int onoff);
		///
///<summary>
///CAPI3REF: Automatically Load Statically Linked Extensions
///
///^This interface causes the xEntryPoint() function to be invoked for
///each new [database connection] that is created.  The idea here is that
///xEntryPoint() is the entry point for a statically linked SQLite extension
///that is to be automatically loaded into all new database connections.
///
///^(Even though the function prototype shows that xEntryPoint() takes
///no arguments and returns void, SQLite invokes xEntryPoint() with three
///arguments and expects and integer result as if the signature of the
///entry point where as follows:
///
///<blockquote><pre>
///&nbsp;  int xEntryPoint(
///&nbsp;    sqlite3 db,
///&nbsp;    string *pzErrMsg,
///&nbsp;    const struct sqlite3_api_routines *pThunk
///&nbsp;  );
///</pre></blockquote>)^
///
///If the xEntryPoint routine encounters an error, it should make *pzErrMsg
///point to an appropriate error message (obtained from [io.sqlite3_mprintf()])
///and return an appropriate [error code].  ^SQLite ensures that *pzErrMsg
///is NULL before calling the xEntryPoint().  ^SQLite will invoke
///[malloc_cs.sqlite3_free()] on *pzErrMsg after xEntryPoint() returns.  ^If any
///xEntryPoint() returns an error, the [sqlite3_open()], [sqlite3_open16()],
///or [sqlite3_open_v2()] call that provoked the xEntryPoint() will fail.
///
///^Calling sqlite3_auto_extension(X) with an entry point X that is already
///</summary>
///<param name="on the list of automatic extensions is a harmless no">op. ^No entry point</param>
///<param name="will be called more than once for each database connection that is opened.">will be called more than once for each database connection that is opened.</param>
///<param name=""></param>
///<param name="See also: [sqlite3_reset_auto_extension()].">See also: [sqlite3_reset_auto_extension()].</param>
///<param name=""></param>

		//SQLITE_API int sqlite3_auto_extension(void (*xEntryPoint)(void));
		///
///<summary>
///CAPI3REF: Reset Automatic Extension Loading
///
///^This interface disables all automatic extensions previously
///registered using [sqlite3_auto_extension()].
///
///</summary>

		//SQLITE_API void sqlite3_reset_auto_extension(void);
		///
///<summary>
///</summary>
///<param name="The interface to the virtual">table mechanism is currently considered</param>
///<param name="to be experimental.  The interface might change in incompatible ways.">to be experimental.  The interface might change in incompatible ways.</param>
///<param name="If this is a problem for you, do not use the interface at this time.">If this is a problem for you, do not use the interface at this time.</param>
///<param name=""></param>
///<param name="When the virtual">table mechanism stabilizes, we will declare the</param>
///<param name="interface fixed, support it indefinitely, and remove this comment.">interface fixed, support it indefinitely, and remove this comment.</param>
///<param name=""></param>

		///
///<summary>
///Structures used by the virtual table interface
///
///</summary>

		//typedef struct sqlite3_vtab sqlite3_vtab;
		//typedef struct sqlite3_index_info sqlite3_index_info;
		//typedef struct sqlite3_vtab_cursor sqlite3_vtab_cursor;
		//typedef struct sqlite3_module sqlite3_module;
		
		

		///
///<summary>
///CAPI3REF: Virtual Table Constraint Operator Codes
///
///These macros defined the allowed values for the
///[sqlite3_index_info].aConstraint[].op field.  Each value represents
///an operator that is part of a constraint term in the wHERE clause of
///a query that uses a [virtual table].
///
///</summary>

		//#define SQLITE_INDEX_CONSTRAINT_EQ    2
		//#define SQLITE_INDEX_CONSTRAINT_GT    4
		//#define SQLITE_INDEX_CONSTRAINT_LE    8
		//#define SQLITE_INDEX_CONSTRAINT_LT    16
		//#define SQLITE_INDEX_CONSTRAINT_GE    32
		//#define SQLITE_INDEX_CONSTRAINT_MATCH 64
		const int SQLITE_INDEX_CONSTRAINT_EQ = 2;

		const int SQLITE_INDEX_CONSTRAINT_GT = 4;

		const int SQLITE_INDEX_CONSTRAINT_LE = 8;

		const int SQLITE_INDEX_CONSTRAINT_LT = 16;

		const int SQLITE_INDEX_CONSTRAINT_GE = 32;

		const int SQLITE_INDEX_CONSTRAINT_MATCH = 64;

		///
///<summary>
///CAPI3REF: Register A Virtual Table Implementation
///
///^These routines are used to register a new [virtual table module] name.
///^Module names must be registered before
///creating a new [virtual table] using the module and before using a
///preexisting [virtual table] for the module.
///
///^The module name is registered on the [database connection] specified
///by the first parameter.  ^The name of the module is given by the 
///second parameter.  ^The third parameter is a pointer to
///the implementation of the [virtual table module].   ^The fourth
///parameter is an arbitrary client data pointer that is passed through
///into the [xCreate] and [xConnect] methods of the virtual table module
///when a new virtual table is be being created or reinitialized.
///
///^The sqlite3_create_module_v2() interface has a fifth parameter which
///is a pointer to a destructor for the pClientData.  ^SQLite will
///invoke the destructor function (if it is not NULL) when SQLite
///no longer needs the pClientData pointer.  ^The destructor will also
///be invoked if the call to sqlite3_create_module_v2() fails.
///^The sqlite3_create_module()
///interface is equivalent to sqlite3_create_module_v2() with a NULL
///destructor.
///
///</summary>

		//SQLITE_API int sqlite3_create_module(
		//  sqlite3 db,               /* SQLite connection to register module with */
		//  string zName,         /* Name of the module */
		//  const sqlite3_module *p,   /* Methods for the module */
		//  void *pClientData          /* Client data for xCreate/xConnect */
		//);
		//SQLITE_API int sqlite3_create_module_v2(
		//  sqlite3 db,               /* SQLite connection to register module with */
		//  string zName,         /* Name of the module */
		//  const sqlite3_module *p,   /* Methods for the module */
		//  void *pClientData,         /* Client data for xCreate/xConnect */
		//  void(*xDestroy)(void)     /* Module destructor function */
		//);
		



		///
///<summary>
///CAPI3REF: Declare The Schema Of A Virtual Table
///
///^The [xCreate] and [xConnect] methods of a
///[virtual table module] call this interface
///to declare the format (the names and datatypes of the columns) of
///the virtual tables they implement.
///
///</summary>

		//SQLITE_API int sqlite3_declare_vtab(sqlite3*, string zSQL);
		///
///<summary>
///CAPI3REF: Overload A Function For A Virtual Table
///
///^(Virtual tables can provide alternative implementations of functions
///using the [xFindFunction] method of the [virtual table module].  
///But global versions of those functions
///must exist in order to be overloaded.)^
///
///^(This API makes sure a global version of a function with a particular
///name and number of parameters exists.  If no such function exists
///before this API is called, a new function is created.)^  ^The implementation
///of the new function always causes an exception to be thrown.  So
///the new function is not good for anything by itself.  Its only
///purpose is to be a placeholder function that can be overloaded
///by a [virtual table].
///
///</summary>

		//SQLITE_API int sqlite3_overload_function(sqlite3*, string zFuncName, int nArg);
		///
///<summary>
///</summary>
///<param name="The interface to the virtual">table mechanism defined above (back up</param>
///<param name="to a comment remarkably similar to this one) is currently considered">to a comment remarkably similar to this one) is currently considered</param>
///<param name="to be experimental.  The interface might change in incompatible ways.">to be experimental.  The interface might change in incompatible ways.</param>
///<param name="If this is a problem for you, do not use the interface at this time.">If this is a problem for you, do not use the interface at this time.</param>
///<param name=""></param>
///<param name="When the virtual">table mechanism stabilizes, we will declare the</param>
///<param name="interface fixed, support it indefinitely, and remove this comment.">interface fixed, support it indefinitely, and remove this comment.</param>
///<param name=""></param>

		///
///<summary>
///CAPI3REF: A Handle To An Open BLOB
///KEYWORDS: {BLOB handle} {BLOB handles}
///
///An instance of this object represents an open BLOB on which
///[sqlite3_blob_open | incremental BLOB I/O] can be performed.
///^Objects of this type are created by [sqlite3_blob_open()]
///and destroyed by [sqlite3_blob_close()].
///^The [sqlite3_blob_read()] and [sqlite3_blob_write()] interfaces
///can be used to read or write small subsections of the BLOB.
///^The [sqlite3_blob_bytes()] interface returns the size of the BLOB in bytes.
///
///</summary>

		//typedef struct sqlite3_blob sqlite3_blob;
		///
///<summary>
///CAPI3REF: Open A BLOB For Incremental I/O
///
///^(This interfaces opens a [BLOB handle | handle] to the BLOB located
///in row iRow, column zColumn, table zTable in database zDb;
///in other words, the same BLOB that would be selected by:
///
///<pre>
///SELECT zColumn FROM zDb.zTable WHERE [rowid] = iRow;
///</pre>)^
///
///</summary>
///<param name="^If the flags parameter is non">zero, then the BLOB is opened for read</param>
///<param name="and write access. ^If it is zero, the BLOB is opened for read access.">and write access. ^If it is zero, the BLOB is opened for read access.</param>
///<param name="^It is not possible to open a column that is part of an index or primary ">^It is not possible to open a column that is part of an index or primary </param>
///<param name="key for writing. ^If [foreign key constraints] are enabled, it is ">key for writing. ^If [foreign key constraints] are enabled, it is </param>
///<param name="not possible to open a column that is part of a [child key] for writing.">not possible to open a column that is part of a [child key] for writing.</param>
///<param name=""></param>
///<param name="^Note that the database name is not the filename that contains">^Note that the database name is not the filename that contains</param>
///<param name="the database but rather the symbolic name of the database that">the database but rather the symbolic name of the database that</param>
///<param name="appears after the AS keyword when the database is connected using [ATTACH].">appears after the AS keyword when the database is connected using [ATTACH].</param>
///<param name="^For the main database file, the database name is "main".">^For the main database file, the database name is "main".</param>
///<param name="^For TEMP tables, the database name is "temp".">^For TEMP tables, the database name is "temp".</param>
///<param name=""></param>
///<param name="^(On success, [SqlResult.SQLITE_OK] is returned and the new [BLOB handle] is written">^(On success, [SqlResult.SQLITE_OK] is returned and the new [BLOB handle] is written</param>
///<param name="to *ppBlob. Otherwise an [error code] is returned and *ppBlob is set">to *ppBlob. Otherwise an [error code] is returned and *ppBlob is set</param>
///<param name="to be a null pointer.)^">to be a null pointer.)^</param>
///<param name="^This function sets the [database connection] error code and message">^This function sets the [database connection] error code and message</param>
///<param name="accessible via [sqlite3_errcode()] and [sqlite3_errmsg()] and related">accessible via [sqlite3_errcode()] and [sqlite3_errmsg()] and related</param>
///<param name="functions. ^Note that the *ppBlob variable is always initialized in a">functions. ^Note that the *ppBlob variable is always initialized in a</param>
///<param name="way that makes it safe to invoke [sqlite3_blob_close()] on *ppBlob">way that makes it safe to invoke [sqlite3_blob_close()] on *ppBlob</param>
///<param name="regardless of the success or failure of this routine.">regardless of the success or failure of this routine.</param>
///<param name=""></param>
///<param name="^(If the row that a BLOB handle points to is modified by an">^(If the row that a BLOB handle points to is modified by an</param>
///<param name="[UPDATE], [DELETE], or by [ON CONFLICT] side">effects</param>
///<param name="then the BLOB handle is marked as "expired".">then the BLOB handle is marked as "expired".</param>
///<param name="This is true if any column of the row is changed, even a column">This is true if any column of the row is changed, even a column</param>
///<param name="other than the one the BLOB handle is open on.)^">other than the one the BLOB handle is open on.)^</param>
///<param name="^Calls to [sqlite3_blob_read()] and [sqlite3_blob_write()] for">^Calls to [sqlite3_blob_read()] and [sqlite3_blob_write()] for</param>
///<param name="a expired BLOB handle fail with a return code of [SQLITE_ABORT].">a expired BLOB handle fail with a return code of [SQLITE_ABORT].</param>
///<param name="^(Changes written into a BLOB prior to the BLOB expiring are not">^(Changes written into a BLOB prior to the BLOB expiring are not</param>
///<param name="rolled back by the expiration of the BLOB.  Such changes will eventually">rolled back by the expiration of the BLOB.  Such changes will eventually</param>
///<param name="commit if the transaction continues to completion.)^">commit if the transaction continues to completion.)^</param>
///<param name=""></param>
///<param name="^Use the [sqlite3_blob_bytes()] interface to determine the size of">^Use the [sqlite3_blob_bytes()] interface to determine the size of</param>
///<param name="the opened blob.  ^The size of a blob may not be changed by this">the opened blob.  ^The size of a blob may not be changed by this</param>
///<param name="interface.  Use the [UPDATE] SQL command to change the size of a">interface.  Use the [UPDATE] SQL command to change the size of a</param>
///<param name="blob.">blob.</param>
///<param name=""></param>
///<param name="^The [sqlite3_bind_zeroblob()] and [sqlite3_result_zeroblob()] interfaces">^The [sqlite3_bind_zeroblob()] and [sqlite3_result_zeroblob()] interfaces</param>
///<param name="and the built">in [zeroblob] SQL function can be used, if desired,</param>
///<param name="to create an empty, zero">filled blob in which to read or write using</param>
///<param name="this interface.">this interface.</param>
///<param name=""></param>
///<param name="To avoid a resource leak, every open [BLOB handle] should eventually">To avoid a resource leak, every open [BLOB handle] should eventually</param>
///<param name="be released by a call to [sqlite3_blob_close()].">be released by a call to [sqlite3_blob_close()].</param>
///<param name=""></param>

		//SQLITE_API int sqlite3_blob_open(
		//  sqlite3*,
		//  string zDb,
		//  string zTable,
		//  string zColumn,
		//  sqlite3_int64 iRow,
		//  int flags,
		//  sqlite3_blob **ppBlob
		//);
		///
///<summary>
///CAPI3REF: Move a BLOB Handle to a New Row
///
///^This function is used to move an existing blob handle so that it points
///to a different row of the same database table. ^The new row is identified
///by the rowid value passed as the second argument. Only the row can be
///changed. ^The database, table and column on which the blob handle is open
///remain the same. Moving an existing blob handle to a new row can be
///faster than closing the existing handle and opening a new one.
///
///</summary>
///<param name="^(The new row must meet the same criteria as for [sqlite3_blob_open()] "></param>
///<param name="it must exist and there must be either a blob or text value stored in">it must exist and there must be either a blob or text value stored in</param>
///<param name="the nominated column.)^ ^If the new row is not present in the table, or if">the nominated column.)^ ^If the new row is not present in the table, or if</param>
///<param name="it does not contain a blob or text value, or if another error occurs, an">it does not contain a blob or text value, or if another error occurs, an</param>
///<param name="SQLite error code is returned and the blob handle is considered aborted.">SQLite error code is returned and the blob handle is considered aborted.</param>
///<param name="^All subsequent calls to [sqlite3_blob_read()], [sqlite3_blob_write()] or">^All subsequent calls to [sqlite3_blob_read()], [sqlite3_blob_write()] or</param>
///<param name="[sqlite3_blob_reopen()] on an aborted blob handle immediately return">[sqlite3_blob_reopen()] on an aborted blob handle immediately return</param>
///<param name="SQLITE_ABORT. ^Calling [sqlite3_blob_bytes()] on an aborted blob handle">SQLITE_ABORT. ^Calling [sqlite3_blob_bytes()] on an aborted blob handle</param>
///<param name="always returns zero.">always returns zero.</param>
///<param name=""></param>
///<param name="^This function sets the database handle error code and message.">^This function sets the database handle error code and message.</param>
///<param name=""></param>

		//SQLITE_API SQLITE_EXPERIMENTAL int sqlite3_blob_reopen(sqlite3_blob *, sqlite3_int64);
		///
///<summary>
///CAPI3REF: Close A BLOB Handle
///
///^Closes an open [BLOB handle].
///
///^Closing a BLOB shall cause the current transaction to commit
///if there are no other BLOBs, no pending prepared statements, and the
///database connection is in [autocommit mode].
///^If any writes were made to the BLOB, they might be held in cache
///until the close operation if they will fit.
///
///^(Closing the BLOB often forces the changes
///out to disk and so if any I/O errors occur, they will likely occur
///at the time when the BLOB is closed.  Any errors that occur during
///</summary>
///<param name="closing are reported as a non">zero return value.)^</param>
///<param name=""></param>
///<param name="^(The BLOB is closed unconditionally.  Even if this routine returns">^(The BLOB is closed unconditionally.  Even if this routine returns</param>
///<param name="an error code, the BLOB is still closed.)^">an error code, the BLOB is still closed.)^</param>
///<param name=""></param>
///<param name="^Calling this routine with a null pointer (such as would be returned">^Calling this routine with a null pointer (such as would be returned</param>
///<param name="by a failed call to [sqlite3_blob_open()]) is a harmless no">op.</param>
///<param name=""></param>

		//SQLITE_API int sqlite3_blob_close(sqlite3_blob );
		///
///<summary>
///CAPI3REF: Return The Size Of An Open BLOB
///
///^Returns the size in bytes of the BLOB accessible via the 
///successfully opened [BLOB handle] in its only argument.  ^The
///incremental blob I/O routines can only read or overwriting existing
///blob content; they cannot change the size of a blob.
///
///This routine only works on a [BLOB handle] which has been created
///by a prior successful call to [sqlite3_blob_open()] and which has not
///been closed by [sqlite3_blob_close()].  Passing any other pointer in
///to this routine results in undefined and probably undesirable behavior.
///
///</summary>

		//SQLITE_API int sqlite3_blob_bytes(sqlite3_blob );
		///
///<summary>
///CAPI3REF: Read Data From A BLOB Incrementally
///
///^(This function is used to read data from an open [BLOB handle] into a
///</summary>
///<param name="caller">supplied buffer. N bytes of data are copied into buffer Z</param>
///<param name="from the open BLOB, starting at offset iOffset.)^">from the open BLOB, starting at offset iOffset.)^</param>
///<param name=""></param>
///<param name="^If offset iOffset is less than N bytes from the end of the BLOB,">^If offset iOffset is less than N bytes from the end of the BLOB,</param>
///<param name="[SqlResult.SQLITE_ERROR] is returned and no data is read.  ^If N or iOffset is">[SqlResult.SQLITE_ERROR] is returned and no data is read.  ^If N or iOffset is</param>
///<param name="less than zero, [SqlResult.SQLITE_ERROR] is returned and no data is read.">less than zero, [SqlResult.SQLITE_ERROR] is returned and no data is read.</param>
///<param name="^The size of the blob (and hence the maximum value of N+iOffset)">^The size of the blob (and hence the maximum value of N+iOffset)</param>
///<param name="can be determined using the [sqlite3_blob_bytes()] interface.">can be determined using the [sqlite3_blob_bytes()] interface.</param>
///<param name=""></param>
///<param name="^An attempt to read from an expired [BLOB handle] fails with an">^An attempt to read from an expired [BLOB handle] fails with an</param>
///<param name="error code of [SQLITE_ABORT].">error code of [SQLITE_ABORT].</param>
///<param name=""></param>
///<param name="^(On success, sqlite3_blob_read() returns SqlResult.SQLITE_OK.">^(On success, sqlite3_blob_read() returns SqlResult.SQLITE_OK.</param>
///<param name="Otherwise, an [error code] or an [extended error code] is returned.)^">Otherwise, an [error code] or an [extended error code] is returned.)^</param>
///<param name=""></param>
///<param name="This routine only works on a [BLOB handle] which has been created">This routine only works on a [BLOB handle] which has been created</param>
///<param name="by a prior successful call to [sqlite3_blob_open()] and which has not">by a prior successful call to [sqlite3_blob_open()] and which has not</param>
///<param name="been closed by [sqlite3_blob_close()].  Passing any other pointer in">been closed by [sqlite3_blob_close()].  Passing any other pointer in</param>
///<param name="to this routine results in undefined and probably undesirable behavior.">to this routine results in undefined and probably undesirable behavior.</param>
///<param name=""></param>
///<param name="See also: [sqlite3_blob_write()].">See also: [sqlite3_blob_write()].</param>
///<param name=""></param>

		//SQLITE_API int sqlite3_blob_read(sqlite3_blob *, object  *Z, int N, int iOffset);
		///
///<summary>
///CAPI3REF: Write Data Into A BLOB Incrementally
///
///^This function is used to write data into an open [BLOB handle] from a
///</summary>
///<param name="caller">supplied buffer. ^N bytes of data are copied from the buffer Z</param>
///<param name="into the open BLOB, starting at offset iOffset.">into the open BLOB, starting at offset iOffset.</param>
///<param name=""></param>
///<param name="^If the [BLOB handle] passed as the first argument was not opened for">^If the [BLOB handle] passed as the first argument was not opened for</param>
///<param name="writing (the flags parameter to [sqlite3_blob_open()] was zero),">writing (the flags parameter to [sqlite3_blob_open()] was zero),</param>
///<param name="this function returns [SQLITE_READONLY].">this function returns [SQLITE_READONLY].</param>
///<param name=""></param>
///<param name="^This function may only modify the contents of the BLOB; it is">^This function may only modify the contents of the BLOB; it is</param>
///<param name="not possible to increase the size of a BLOB using this API.">not possible to increase the size of a BLOB using this API.</param>
///<param name="^If offset iOffset is less than N bytes from the end of the BLOB,">^If offset iOffset is less than N bytes from the end of the BLOB,</param>
///<param name="[SqlResult.SQLITE_ERROR] is returned and no data is written.  ^If N is">[SqlResult.SQLITE_ERROR] is returned and no data is written.  ^If N is</param>
///<param name="less than zero [SqlResult.SQLITE_ERROR] is returned and no data is written.">less than zero [SqlResult.SQLITE_ERROR] is returned and no data is written.</param>
///<param name="The size of the BLOB (and hence the maximum value of N+iOffset)">The size of the BLOB (and hence the maximum value of N+iOffset)</param>
///<param name="can be determined using the [sqlite3_blob_bytes()] interface.">can be determined using the [sqlite3_blob_bytes()] interface.</param>
///<param name=""></param>
///<param name="^An attempt to write to an expired [BLOB handle] fails with an">^An attempt to write to an expired [BLOB handle] fails with an</param>
///<param name="error code of [SQLITE_ABORT].  ^Writes to the BLOB that occurred">error code of [SQLITE_ABORT].  ^Writes to the BLOB that occurred</param>
///<param name="before the [BLOB handle] expired are not rolled back by the">before the [BLOB handle] expired are not rolled back by the</param>
///<param name="expiration of the handle, though of course those changes might">expiration of the handle, though of course those changes might</param>
///<param name="have been overwritten by the statement that expired the BLOB handle">have been overwritten by the statement that expired the BLOB handle</param>
///<param name="or by other independent statements.">or by other independent statements.</param>
///<param name=""></param>
///<param name="^(On success, sqlite3_blob_write() returns SqlResult.SQLITE_OK.">^(On success, sqlite3_blob_write() returns SqlResult.SQLITE_OK.</param>
///<param name="Otherwise, an  [error code] or an [extended error code] is returned.)^">Otherwise, an  [error code] or an [extended error code] is returned.)^</param>
///<param name=""></param>
///<param name="This routine only works on a [BLOB handle] which has been created">This routine only works on a [BLOB handle] which has been created</param>
///<param name="by a prior successful call to [sqlite3_blob_open()] and which has not">by a prior successful call to [sqlite3_blob_open()] and which has not</param>
///<param name="been closed by [sqlite3_blob_close()].  Passing any other pointer in">been closed by [sqlite3_blob_close()].  Passing any other pointer in</param>
///<param name="to this routine results in undefined and probably undesirable behavior.">to this routine results in undefined and probably undesirable behavior.</param>
///<param name=""></param>
///<param name="See also: [sqlite3_blob_read()].">See also: [sqlite3_blob_read()].</param>
///<param name=""></param>

		//SQLITE_API int sqlite3_blob_write(sqlite3_blob *, string z, int n, int iOffset);
		///
///<summary>
///CAPI3REF: Virtual File System Objects
///
///A virtual filesystem (VFS) is an [sqlite3_vfs] object
///that SQLite uses to interact
///with the underlying operating system.  Most SQLite builds come with a
///single default VFS that is appropriate for the host computer.
///New VFSes can be registered and existing VFSes can be unregistered.
///The following interfaces are provided.
///
///^The sqlite3_vfs_find() interface returns a pointer to a VFS given its name.
///^Names are case sensitive.
///</summary>
///<param name="^Names are zero">8 strings.</param>
///<param name="^If there is no match, a NULL pointer is returned.">^If there is no match, a NULL pointer is returned.</param>
///<param name="^If zVfsName is NULL then the default VFS is returned.">^If zVfsName is NULL then the default VFS is returned.</param>
///<param name=""></param>
///<param name="^New VFSes are registered with sqlite3_vfs_register().">^New VFSes are registered with sqlite3_vfs_register().</param>
///<param name="^Each new VFS becomes the default VFS if the makeDflt flag is set.">^Each new VFS becomes the default VFS if the makeDflt flag is set.</param>
///<param name="^The same VFS can be registered multiple times without injury.">^The same VFS can be registered multiple times without injury.</param>
///<param name="^To make an existing VFS into the default VFS, register it again">^To make an existing VFS into the default VFS, register it again</param>
///<param name="with the makeDflt flag set.  If two different VFSes with the">with the makeDflt flag set.  If two different VFSes with the</param>
///<param name="same name are registered, the behavior is undefined.  If a">same name are registered, the behavior is undefined.  If a</param>
///<param name="VFS is registered with a name that is NULL or an empty string,">VFS is registered with a name that is NULL or an empty string,</param>
///<param name="then the behavior is undefined.">then the behavior is undefined.</param>
///<param name=""></param>
///<param name="^Unregister a VFS with the sqlite3_vfs_unregister() interface.">^Unregister a VFS with the sqlite3_vfs_unregister() interface.</param>
///<param name="^(If the default VFS is unregistered, another VFS is chosen as">^(If the default VFS is unregistered, another VFS is chosen as</param>
///<param name="the default.  The choice for the new VFS is arbitrary.)^">the default.  The choice for the new VFS is arbitrary.)^</param>
///<param name=""></param>

		

		///
///<summary>
///CAPI3REF: Mutex Verification Routines
///
///The Sqlite3.sqlite3_mutex_held() and sqlite3_mutex_notheld() routines
///are intended for use inside Debug.Assert() statements.  ^The SQLite core
///never uses these routines except inside an Debug.Assert() and applications
///are advised to follow the lead of the core.  ^The SQLite core only
///provides implementations for these routines when it is compiled
///with the SQLITE_DEBUG flag.  ^External mutex implementations
///are only required to provide these routines if SQLITE_DEBUG is
///defined and if NDEBUG is not defined.
///
///^These routines should return true if the mutex in their argument
///is held or not held, respectively, by the calling thread.
///
///^The implementation is not required to provided versions of these
///routines that actually work. If the implementation does not provide working
///versions of these routines, it should at least provide stubs that always
///return true so that one does not get spurious assertion failures.
///
///^If the argument to Sqlite3.sqlite3_mutex_held() is a NULL pointer then
///</summary>
///<param name="the routine should return 1.   This seems counter">intuitive since</param>
///<param name="clearly the mutex cannot be held if it does not exist.  But">clearly the mutex cannot be held if it does not exist.  But</param>
///<param name="the reason the mutex does not exist is because the build is not">the reason the mutex does not exist is because the build is not</param>
///<param name="using mutexes.  And we do not want the Debug.Assert() containing the">using mutexes.  And we do not want the Debug.Assert() containing the</param>
///<param name="call to Sqlite3.sqlite3_mutex_held() to fail, so a non">zero return is</param>
///<param name="the appropriate thing to do.  ^The sqlite3_mutex_notheld()">the appropriate thing to do.  ^The sqlite3_mutex_notheld()</param>
///<param name="interface should also return 1 when given a NULL pointer.">interface should also return 1 when given a NULL pointer.</param>
///<param name=""></param>

		//#if !NDEBUG
		//SQLITE_API int Sqlite3.sqlite3_mutex.sqlite3_mutex_held();
		//SQLITE_API int sqlite3_mutex_notheld(sqlite3_mutex);
		//#endif
		///
///<summary>
///CAPI3REF: Mutex Types
///
///The [sqlite3_mutex_alloc()] interface takes a single argument
///which is one of these integer constants.
///
///The set of static mutexes may change from one SQLite release to the
///</summary>
///<param name="next.  Applications that override the built">in mutex logic must be</param>
///<param name="prepared to accommodate additional static mutexes.">prepared to accommodate additional static mutexes.</param>
///<param name=""></param>

		//#define SQLITE_MUTEX_FAST             0
		//#define SQLITE_MUTEX_RECURSIVE        1
		//#define SQLITE_MUTEX_STATIC_MASTER    2
		//#define SQLITE_MUTEX_STATIC_MEM       3  /* sqlite3_malloc() */
		//#define SQLITE_MUTEX_STATIC_MEM2      4  /* NOT USED */
		//#define SQLITE_MUTEX_STATIC_OPEN      4  /* sqlite3BtreeOpen() */
		//#define SQLITE_MUTEX_STATIC_PRNG      5  /* sqlite3_random() */
		//#define SQLITE_MUTEX_STATIC_LRU       6  /* lru page list */
		//#define SQLITE_MUTEX_STATIC_LRU2      7  /* NOT USED */
		//#define SQLITE_MUTEX_STATIC_PMEM      7  /* sqlite3PageMalloc() */
        public const int SQLITE_MUTEX_FAST = 0;

        public const int SQLITE_MUTEX_RECURSIVE = 1;

        public const int SQLITE_MUTEX_STATIC_MASTER = 2;

        public const int SQLITE_MUTEX_STATIC_MEM = 3;

        public const int SQLITE_MUTEX_STATIC_MEM2 = 4;

        public const int SQLITE_MUTEX_STATIC_OPEN = 4;

        public const int SQLITE_MUTEX_STATIC_PRNG = 5;

        public const int SQLITE_MUTEX_STATIC_LRU = 6;

        public const int SQLITE_MUTEX_STATIC_LRU2 = 7;

        public const int SQLITE_MUTEX_STATIC_PMEM = 7;

		///
///<summary>
///CAPI3REF: Retrieve the mutex for a database connection
///
///^This interface returns a pointer the [sqlite3_mutex] object that 
///serializes access to the [database connection] given in the argument
///when the [threading mode] is Serialized.
///</summary>
///<param name="^If the [threading mode] is Single">thread then this</param>
///<param name="routine returns a NULL pointer.">routine returns a NULL pointer.</param>
///<param name=""></param>

		//SQLITE_API sqlite3_mutex *sqlite3_db_mutex(sqlite3);
		///
///<summary>
///</summary>
///<param name="CAPI3REF: Low">Level Control Of Database Files</param>
///<param name=""></param>
///<param name="^The [sqlite3_file_control()] interface makes a direct call to the">^The [sqlite3_file_control()] interface makes a direct call to the</param>
///<param name="xFileControl method for the [sqlite3_io_methods] object associated">xFileControl method for the [sqlite3_io_methods] object associated</param>
///<param name="with a particular database identified by the second argument. ^The">with a particular database identified by the second argument. ^The</param>
///<param name="name of the database is "main" for the main database or "temp" for the">name of the database is "main" for the main database or "temp" for the</param>
///<param name="TEMP database, or the name that appears after the AS keyword for">TEMP database, or the name that appears after the AS keyword for</param>
///<param name="databases that are added using the [ATTACH] SQL command.">databases that are added using the [ATTACH] SQL command.</param>
///<param name="^A NULL pointer can be used in place of "main" to refer to the">^A NULL pointer can be used in place of "main" to refer to the</param>
///<param name="main database file.">main database file.</param>
///<param name="^The third and fourth parameters to this routine">^The third and fourth parameters to this routine</param>
///<param name="are passed directly through to the second and third parameters of">are passed directly through to the second and third parameters of</param>
///<param name="the xFileControl method.  ^The return value of the xFileControl">the xFileControl method.  ^The return value of the xFileControl</param>
///<param name="method becomes the return value of this routine.">method becomes the return value of this routine.</param>
///<param name=""></param>
///<param name="^The SQLITE_FCNTL_FILE_POINTER value for the op parameter causes">^The SQLITE_FCNTL_FILE_POINTER value for the op parameter causes</param>
///<param name="a pointer to the underlying [sqlite3_file] object to be written into">a pointer to the underlying [sqlite3_file] object to be written into</param>
///<param name="the space pointed to by the 4th parameter.  ^The SQLITE_FCNTL_FILE_POINTER">the space pointed to by the 4th parameter.  ^The SQLITE_FCNTL_FILE_POINTER</param>
///<param name="case is a short">circuit path which does not actually invoke the</param>
///<param name="underlying sqlite3_io_methods.xFileControl method.">underlying sqlite3_io_methods.xFileControl method.</param>
///<param name=""></param>
///<param name="^If the second parameter (zDbName) does not match the name of any">^If the second parameter (zDbName) does not match the name of any</param>
///<param name="open database file, then SqlResult.SQLITE_ERROR is returned.  ^This error">open database file, then SqlResult.SQLITE_ERROR is returned.  ^This error</param>
///<param name="code is not remembered and will not be recalled by [sqlite3_errcode()]">code is not remembered and will not be recalled by [sqlite3_errcode()]</param>
///<param name="or [sqlite3_errmsg()].  The underlying xFileControl method might">or [sqlite3_errmsg()].  The underlying xFileControl method might</param>
///<param name="also return SqlResult.SQLITE_ERROR.  There is no way to distinguish between">also return SqlResult.SQLITE_ERROR.  There is no way to distinguish between</param>
///<param name="an incorrect zDbName and an SqlResult.SQLITE_ERROR return from the underlying">an incorrect zDbName and an SqlResult.SQLITE_ERROR return from the underlying</param>
///<param name="xFileControl method.">xFileControl method.</param>
///<param name=""></param>
///<param name="See also: [SQLITE_FCNTL_LOCKSTATE]">See also: [SQLITE_FCNTL_LOCKSTATE]</param>
///<param name=""></param>

		//SQLITE_API int sqlite3_file_control(sqlite3*, string zDbName, int op, void);
		///
///<summary>
///CAPI3REF: Testing Interface
///
///^The sqlite3_test_control() interface is used to read out internal
///state of SQLite and to inject faults into SQLite for testing
///purposes.  ^The first parameter is an operation code that determines
///the number, meaning, and operation of all subsequent parameters.
///
///This interface is not for use by applications.  It exists solely
///for verifying the correct operation of the SQLite library.  Depending
///on how the SQLite library is compiled, this interface might not exist.
///
///The details of the operation codes, their meanings, the parameters
///they take, and what they do are all subject to change without notice.
///Unlike most of the SQLite API, this function is not guaranteed to
///operate consistently from one release to the next.
///
///</summary>

		//SQLITE_API int sqlite3_test_control(int op, ...);
		///
///<summary>
///CAPI3REF: Testing Interface Op Codes
///
///These constants are the valid operation code parameters used
///as the first argument to [sqlite3_test_control()].
///
///These parameters and their meanings are subject to change
///without notice.  These values are for testing purposes only.
///Applications should not use any of these parameters or the
///[sqlite3_test_control()] interface.
///
///</summary>

		//#define SQLITE_TESTCTRL_FIRST                    5
		//#define SQLITE_TESTCTRL_PRNG_SAVE                5
		//#define SQLITE_TESTCTRL_PRNG_RESTORE             6
		//#define SQLITE_TESTCTRL_PRNG_RESET               7
		//#define SQLITE_TESTCTRL_BITVEC_TEST              8
		//#define SQLITE_TESTCTRL_FAULT_INSTALL            9
		//#define SQLITE_TESTCTRL_BENIGN_MALLOC_HOOKS     10
		//#define SQLITE_TESTCTRL_PENDING_BYTE            11
		//#define SQLITE_TESTCTRL_ASSERT                  12
		//#define SQLITE_TESTCTRL_ALWAYS                  13
		//#define SQLITE_TESTCTRL_RESERVE                 14
		//#define SQLITE_TESTCTRL_OPTIMIZATIONS           15
		//#define SQLITE_TESTCTRL_ISKEYWORD               16
		//#define SQLITE_TESTCTRL_PGHDRSZ                 17
		//#define SQLITE_TESTCTRL_SCRATCHMALLOC           18
		//#define SQLITE_TESTCTRL_LOCALTIME_FAULT         19
		//#define SQLITE_TESTCTRL_LAST                    19
		const int SQLITE_TESTCTRL_FIRST = 5;

		const int SQLITE_TESTCTRL_PRNG_SAVE = 5;

		const int SQLITE_TESTCTRL_PRNG_RESTORE = 6;

		const int SQLITE_TESTCTRL_PRNG_RESET = 7;

		const int SQLITE_TESTCTRL_BITVEC_TEST = 8;

		const int SQLITE_TESTCTRL_FAULT_INSTALL = 9;

		const int SQLITE_TESTCTRL_BENIGN_MALLOC_HOOKS = 10;

		const int SQLITE_TESTCTRL_PENDING_BYTE = 11;

		const int SQLITE_TESTCTRL_ASSERT = 12;

		const int SQLITE_TESTCTRL_ALWAYS = 13;

		const int SQLITE_TESTCTRL_RESERVE = 14;

		const int SQLITE_TESTCTRL_OPTIMIZATIONS = 15;

		const int SQLITE_TESTCTRL_ISKEYWORD = 16;

		const int SQLITE_TESTCTRL_PGHDRSZ = 17;

		const int SQLITE_TESTCTRL_SCRATCHMALLOC = 18;

		const int SQLITE_TESTCTRL_LOCALTIME_FAULT = 19;

		const int SQLITE_TESTCTRL_LAST = 19;

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
		public const int SQLITE_STATUS_MEMORY_USED = 0;

        public const int SQLITE_STATUS_PAGECACHE_USED = 1;

        public const int SQLITE_STATUS_PAGECACHE_OVERFLOW = 2;

        public const int SQLITE_STATUS_SCRATCH_USED = 3;

        public const int SQLITE_STATUS_SCRATCH_OVERFLOW = 4;

        public const int SQLITE_STATUS_MALLOC_SIZE = 5;

        public const int SQLITE_STATUS_PARSER_STACK = 6;

        public const int SQLITE_STATUS_PAGECACHE_SIZE = 7;

        public const int SQLITE_STATUS_SCRATCH_SIZE = 8;

        public const int SQLITE_STATUS_MALLOC_COUNT = 9;

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
		const int SQLITE_DBSTATUS_LOOKASIDE_USED = 0;

		const int SQLITE_DBSTATUS_CACHE_USED = 1;

		const int SQLITE_DBSTATUS_SCHEMA_USED = 2;

		const int SQLITE_DBSTATUS_STMT_USED = 3;

		const int SQLITE_DBSTATUS_LOOKASIDE_HIT = 4;

		const int SQLITE_DBSTATUS_LOOKASIDE_MISS_SIZE = 5;

		const int SQLITE_DBSTATUS_LOOKASIDE_MISS_FULL = 6;

		const int SQLITE_DBSTATUS_MAX = 6;

		///
///<summary>
///CAPI3REF: Prepared Statement Status
///
///^(Each prepared statement maintains various
///[SQLITE_STMTSTATUS counters] that measure the number
///of times it has performed specific operations.)^  These counters can
///be used to monitor the performance characteristics of the prepared
///statements.  For example, if the number of table steps greatly exceeds
///the number of table searches or result rows, that would tend to indicate
///that the prepared statement is using a full table scan rather than
///an index.  
///
///^(This interface is used to retrieve and reset counter values from
///a [prepared statement].  The first argument is the prepared statement
///object to be interrogated.  The second argument
///is an integer code for a specific [SQLITE_STMTSTATUS counte
///to be interrogated.)^
///^The current value of the requested counter is returned.
///^If the resetFlg is true, then the counter is reset to zero after this
///interface call returns.
///
///See also: [sqlite3_status()] and [sqlite3_db_status()].
///
///</summary>

		//SQLITE_API int sqlite3_stmt_status(sqlite3_stmt*, int op,int resetFlg);
		///
///<summary>
///CAPI3REF: Status Parameters for prepared statements
///KEYWORDS: {SQLITE_STMTSTATUS counter} {SQLITE_STMTSTATUS counters}
///
///These preprocessor macros define integer codes that name counter
///values associated with the [sqlite3_stmt_status()] interface.
///The meanings of the various counters are as follows:
///
///<dl>
///[[SQLITE_STMTSTATUS_FULLSCAN_STEP]] <dt>SQLITE_STMTSTATUS_FULLSCAN_STEP</dt>
///<dd>^This is the number of times that SQLite has stepped forward in
///a table as part of a full table scan.  Large numbers for this counter
///may indicate opportunities for performance improvement through 
///careful use of indices.</dd>
///
///[[SQLITE_STMTSTATUS_SORT]] <dt>SQLITE_STMTSTATUS_SORT</dt>
///<dd>^This is the number of sort operations that have occurred.
///</summary>
///<param name="A non">zero value in this counter may indicate an opportunity to</param>
///<param name="improvement performance through careful use of indices.</dd>">improvement performance through careful use of indices.</dd></param>
///<param name=""></param>
///<param name="[[SQLITE_STMTSTATUS_AUTOINDEX]] <dt>SQLITE_STMTSTATUS_AUTOINDEX</dt>">[[SQLITE_STMTSTATUS_AUTOINDEX]] <dt>SQLITE_STMTSTATUS_AUTOINDEX</dt></param>
///<param name="<dd>^This is the number of rows inserted into transient indices that"><dd>^This is the number of rows inserted into transient indices that</param>
///<param name="were created automatically in order to help joins run faster.">were created automatically in order to help joins run faster.</param>
///<param name="A non">zero value in this counter may indicate an opportunity to</param>
///<param name="improvement performance by adding permanent indices that do not">improvement performance by adding permanent indices that do not</param>
///<param name="need to be reinitialized each time the statement is run.</dd>">need to be reinitialized each time the statement is run.</dd></param>
///<param name=""></param>
///<param name="</dl>"></dl></param>
///<param name=""></param>

		//#define SQLITE_STMTSTATUS_FULLSCAN_STEP     1
		//#define SQLITE_STMTSTATUS_SORT              2
		//#define SQLITE_STMTSTATUS_AUTOINDEX         3
		public const int SQLITE_STMTSTATUS_FULLSCAN_STEP = 1;

        public const int SQLITE_STMTSTATUS_SORT = 2;

        public const int SQLITE_STMTSTATUS_AUTOINDEX = 3;


		///
///<summary>
///CAPI3REF: Online Backup Object
///
///The sqlite3_backup object records state information about an ongoing
///online backup operation.  ^The sqlite3_backup object is created by
///a call to [sqlite3_backup_init()] and is destroyed by a call to
///[sqlite3_backup_finish()].
///
///See Also: [Using the SQLite Online Backup API]
///
///</summary>

		//typedef struct sqlite3_backup sqlite3_backup;
		///
///<summary>
///CAPI3REF: Online Backup API.
///
///The backup API copies the content of one database into another.
///It is useful either for creating backups of databases or
///</summary>
///<param name="for copying in">memory databases to or from persistent files. </param>
///<param name=""></param>
///<param name="See Also: [Using the SQLite Online Backup API]">See Also: [Using the SQLite Online Backup API]</param>
///<param name=""></param>
///<param name="^SQLite holds a write transaction open on the destination database file">^SQLite holds a write transaction open on the destination database file</param>
///<param name="for the duration of the backup operation.">for the duration of the backup operation.</param>
///<param name="^The source database is read">locked only while it is being read;</param>
///<param name="it is not locked continuously for the entire backup operation.">it is not locked continuously for the entire backup operation.</param>
///<param name="^Thus, the backup may be performed on a live source database without">^Thus, the backup may be performed on a live source database without</param>
///<param name="preventing other database connections from">preventing other database connections from</param>
///<param name="reading or writing to the source database while the backup is underway.">reading or writing to the source database while the backup is underway.</param>
///<param name=""></param>
///<param name="^(To perform a backup operation: ">^(To perform a backup operation: </param>
///<param name="<ol>"><ol></param>
///<param name="<li><b>sqlite3_backup_init()</b> is called once to initialize the"><li><b>sqlite3_backup_init()</b> is called once to initialize the</param>
///<param name="backup, ">backup, </param>
///<param name="<li><b>sqlite3_backup_step()</b> is called one or more times to transfer "><li><b>sqlite3_backup_step()</b> is called one or more times to transfer </param>
///<param name="the data between the two databases, and finally">the data between the two databases, and finally</param>
///<param name="<li><b>sqlite3_backup_finish()</b> is called to release all resources "><li><b>sqlite3_backup_finish()</b> is called to release all resources </param>
///<param name="associated with the backup operation. ">associated with the backup operation. </param>
///<param name="</ol>)^"></ol>)^</param>
///<param name="There should be exactly one call to sqlite3_backup_finish() for each">There should be exactly one call to sqlite3_backup_finish() for each</param>
///<param name="successful call to sqlite3_backup_init().">successful call to sqlite3_backup_init().</param>
///<param name=""></param>
///<param name="[[sqlite3_backup_init()]] <b>sqlite3_backup_init()</b>">[[sqlite3_backup_init()]] <b>sqlite3_backup_init()</b></param>
///<param name=""></param>
///<param name="^The D and N arguments to sqlite3_backup_init(D,N,S,M) are the ">^The D and N arguments to sqlite3_backup_init(D,N,S,M) are the </param>
///<param name="[database connection] associated with the destination database ">[database connection] associated with the destination database </param>
///<param name="and the database name, respectively.">and the database name, respectively.</param>
///<param name="^The database name is "main" for the main database, "temp" for the">^The database name is "main" for the main database, "temp" for the</param>
///<param name="temporary database, or the name specified after the AS keyword in">temporary database, or the name specified after the AS keyword in</param>
///<param name="an [ATTACH] statement for an attached database.">an [ATTACH] statement for an attached database.</param>
///<param name="^The S and M arguments passed to ">^The S and M arguments passed to </param>
///<param name="sqlite3_backup_init(D,N,S,M) identify the [database connection]">sqlite3_backup_init(D,N,S,M) identify the [database connection]</param>
///<param name="and database name of the source database, respectively.">and database name of the source database, respectively.</param>
///<param name="^The source and destination [database connections] (parameters S and D)">^The source and destination [database connections] (parameters S and D)</param>
///<param name="must be different or else sqlite3_backup_init(D,N,S,M) will fail with">must be different or else sqlite3_backup_init(D,N,S,M) will fail with</param>
///<param name="an error.">an error.</param>
///<param name=""></param>
///<param name="^If an error occurs within sqlite3_backup_init(D,N,S,M), then NULL is">^If an error occurs within sqlite3_backup_init(D,N,S,M), then NULL is</param>
///<param name="returned and an error code and error message are stored in the">returned and an error code and error message are stored in the</param>
///<param name="destination [database connection] D.">destination [database connection] D.</param>
///<param name="^The error code and message for the failed call to sqlite3_backup_init()">^The error code and message for the failed call to sqlite3_backup_init()</param>
///<param name="can be retrieved using the [sqlite3_errcode()], [sqlite3_errmsg()], and/or">can be retrieved using the [sqlite3_errcode()], [sqlite3_errmsg()], and/or</param>
///<param name="[sqlite3_errmsg16()] functions.">[sqlite3_errmsg16()] functions.</param>
///<param name="^A successful call to sqlite3_backup_init() returns a pointer to an">^A successful call to sqlite3_backup_init() returns a pointer to an</param>
///<param name="[sqlite3_backup] object.">[sqlite3_backup] object.</param>
///<param name="^The [sqlite3_backup] object may be used with the sqlite3_backup_step() and">^The [sqlite3_backup] object may be used with the sqlite3_backup_step() and</param>
///<param name="sqlite3_backup_finish() functions to perform the specified backup ">sqlite3_backup_finish() functions to perform the specified backup </param>
///<param name="operation.">operation.</param>
///<param name=""></param>
///<param name="[[sqlite3_backup_step()]] <b>sqlite3_backup_step()</b>">[[sqlite3_backup_step()]] <b>sqlite3_backup_step()</b></param>
///<param name=""></param>
///<param name="^Function sqlite3_backup_step(B,N) will copy up to N pages between ">^Function sqlite3_backup_step(B,N) will copy up to N pages between </param>
///<param name="the source and destination databases specified by [sqlite3_backup] object B.">the source and destination databases specified by [sqlite3_backup] object B.</param>
///<param name="^If N is negative, all remaining source pages are copied. ">^If N is negative, all remaining source pages are copied. </param>
///<param name="^If sqlite3_backup_step(B,N) successfully copies N pages and there">^If sqlite3_backup_step(B,N) successfully copies N pages and there</param>
///<param name="are still more pages to be copied, then the function returns [SqlResult.SQLITE_OK].">are still more pages to be copied, then the function returns [SqlResult.SQLITE_OK].</param>
///<param name="^If sqlite3_backup_step(B,N) successfully finishes copying all pages">^If sqlite3_backup_step(B,N) successfully finishes copying all pages</param>
///<param name="from source to destination, then it returns [SQLITE_DONE].">from source to destination, then it returns [SQLITE_DONE].</param>
///<param name="^If an error occurs while running sqlite3_backup_step(B,N),">^If an error occurs while running sqlite3_backup_step(B,N),</param>
///<param name="then an [error code] is returned. ^As well as [SqlResult.SQLITE_OK] and">then an [error code] is returned. ^As well as [SqlResult.SQLITE_OK] and</param>
///<param name="[SQLITE_DONE], a call to sqlite3_backup_step() may return [SQLITE_READONLY],">[SQLITE_DONE], a call to sqlite3_backup_step() may return [SQLITE_READONLY],</param>
///<param name="[SQLITE_NOMEM], [SQLITE_BUSY], [SQLITE_LOCKED], or an">[SQLITE_NOMEM], [SQLITE_BUSY], [SQLITE_LOCKED], or an</param>
///<param name="[SQLITE_IOERR_ACCESS | SQLITE_IOERR_XXX] extended error code.">[SQLITE_IOERR_ACCESS | SQLITE_IOERR_XXX] extended error code.</param>
///<param name=""></param>
///<param name="^(The sqlite3_backup_step() might return [SQLITE_READONLY] if">^(The sqlite3_backup_step() might return [SQLITE_READONLY] if</param>
///<param name="<ol>"><ol></param>
///<param name="<li> the destination database was opened read">only, or</param>
///<param name="<li> the destination database is using write">log journaling</param>
///<param name="and the destination and source page sizes differ, or">and the destination and source page sizes differ, or</param>
///<param name="<li> the destination database is an in">memory database and the</param>
///<param name="destination and source page sizes differ.">destination and source page sizes differ.</param>
///<param name="</ol>)^"></ol>)^</param>
///<param name=""></param>
///<param name="^If sqlite3_backup_step() cannot obtain a required file">system lock, then</param>
///<param name="the [sqlite3_busy_handler | busy">handler function]</param>
///<param name="is invoked (if one is specified). ^If the ">is invoked (if one is specified). ^If the </param>
///<param name="busy">zero before the lock is available, then </param>
///<param name="[SQLITE_BUSY] is returned to the caller. ^In this case the call to">[SQLITE_BUSY] is returned to the caller. ^In this case the call to</param>
///<param name="sqlite3_backup_step() can be retried later. ^If the source">sqlite3_backup_step() can be retried later. ^If the source</param>
///<param name="[database connection]">[database connection]</param>
///<param name="is being used to write to the source database when sqlite3_backup_step()">is being used to write to the source database when sqlite3_backup_step()</param>
///<param name="is called, then [SQLITE_LOCKED] is returned immediately. ^Again, in this">is called, then [SQLITE_LOCKED] is returned immediately. ^Again, in this</param>
///<param name="case the call to sqlite3_backup_step() can be retried later on. ^(If">case the call to sqlite3_backup_step() can be retried later on. ^(If</param>
///<param name="[SQLITE_IOERR_ACCESS | SQLITE_IOERR_XXX], [SQLITE_NOMEM], or">[SQLITE_IOERR_ACCESS | SQLITE_IOERR_XXX], [SQLITE_NOMEM], or</param>
///<param name="[SQLITE_READONLY] is returned, then ">[SQLITE_READONLY] is returned, then </param>
///<param name="there is no point in retrying the call to sqlite3_backup_step(). These ">there is no point in retrying the call to sqlite3_backup_step(). These </param>
///<param name="errors are considered fatal.)^  The application must accept ">errors are considered fatal.)^  The application must accept </param>
///<param name="that the backup operation has failed and pass the backup operation handle ">that the backup operation has failed and pass the backup operation handle </param>
///<param name="to the sqlite3_backup_finish() to release associated resources.">to the sqlite3_backup_finish() to release associated resources.</param>
///<param name=""></param>
///<param name="^The first call to sqlite3_backup_step() obtains an exclusive lock">^The first call to sqlite3_backup_step() obtains an exclusive lock</param>
///<param name="on the destination file. ^The exclusive lock is not released until either ">on the destination file. ^The exclusive lock is not released until either </param>
///<param name="sqlite3_backup_finish() is called or the backup operation is complete ">sqlite3_backup_finish() is called or the backup operation is complete </param>
///<param name="and sqlite3_backup_step() returns [SQLITE_DONE].  ^Every call to">and sqlite3_backup_step() returns [SQLITE_DONE].  ^Every call to</param>
///<param name="sqlite3_backup_step() obtains a [shared lock] on the source database that">sqlite3_backup_step() obtains a [shared lock] on the source database that</param>
///<param name="lasts for the duration of the sqlite3_backup_step() call.">lasts for the duration of the sqlite3_backup_step() call.</param>
///<param name="^Because the source database is not locked between calls to">^Because the source database is not locked between calls to</param>
///<param name="sqlite3_backup_step(), the source database may be modified mid">way</param>
///<param name="through the backup process.  ^If the source database is modified by an">through the backup process.  ^If the source database is modified by an</param>
///<param name="external process or via a database connection other than the one being">external process or via a database connection other than the one being</param>
///<param name="used by the backup operation, then the backup will be automatically">used by the backup operation, then the backup will be automatically</param>
///<param name="restarted by the next call to sqlite3_backup_step(). ^If the source ">restarted by the next call to sqlite3_backup_step(). ^If the source </param>
///<param name="database is modified by the using the same database connection as is used">database is modified by the using the same database connection as is used</param>
///<param name="by the backup operation, then the backup database is automatically">by the backup operation, then the backup database is automatically</param>
///<param name="updated at the same time.">updated at the same time.</param>
///<param name=""></param>
///<param name="[[sqlite3_backup_finish()]] <b>sqlite3_backup_finish()</b>">[[sqlite3_backup_finish()]] <b>sqlite3_backup_finish()</b></param>
///<param name=""></param>
///<param name="When sqlite3_backup_step() has returned [SQLITE_DONE], or when the ">When sqlite3_backup_step() has returned [SQLITE_DONE], or when the </param>
///<param name="application wishes to abandon the backup operation, the application">application wishes to abandon the backup operation, the application</param>
///<param name="should destroy the [sqlite3_backup] by passing it to sqlite3_backup_finish().">should destroy the [sqlite3_backup] by passing it to sqlite3_backup_finish().</param>
///<param name="^The sqlite3_backup_finish() interfaces releases all">^The sqlite3_backup_finish() interfaces releases all</param>
///<param name="resources associated with the [sqlite3_backup] object. ">resources associated with the [sqlite3_backup] object. </param>
///<param name="^If sqlite3_backup_step() has not yet returned [SQLITE_DONE], then any">^If sqlite3_backup_step() has not yet returned [SQLITE_DONE], then any</param>
///<param name="active write">transaction on the destination database is rolled back.</param>
///<param name="The [sqlite3_backup] object is invalid">The [sqlite3_backup] object is invalid</param>
///<param name="and may not be used following a call to sqlite3_backup_finish().">and may not be used following a call to sqlite3_backup_finish().</param>
///<param name=""></param>
///<param name="^The value returned by sqlite3_backup_finish is [SqlResult.SQLITE_OK] if no">^The value returned by sqlite3_backup_finish is [SqlResult.SQLITE_OK] if no</param>
///<param name="sqlite3_backup_step() errors occurred, regardless or whether or not">sqlite3_backup_step() errors occurred, regardless or whether or not</param>
///<param name="sqlite3_backup_step() completed.">sqlite3_backup_step() completed.</param>
///<param name="^If an out">memory condition or IO error occurred during any prior</param>
///<param name="sqlite3_backup_step() call on the same [sqlite3_backup] object, then">sqlite3_backup_step() call on the same [sqlite3_backup] object, then</param>
///<param name="sqlite3_backup_finish() returns the corresponding [error code].">sqlite3_backup_finish() returns the corresponding [error code].</param>
///<param name=""></param>
///<param name="^A return of [SQLITE_BUSY] or [SQLITE_LOCKED] from sqlite3_backup_step()">^A return of [SQLITE_BUSY] or [SQLITE_LOCKED] from sqlite3_backup_step()</param>
///<param name="is not a permanent error and does not affect the return value of">is not a permanent error and does not affect the return value of</param>
///<param name="sqlite3_backup_finish().">sqlite3_backup_finish().</param>
///<param name=""></param>
///<param name="[[sqlite3_backup__remaining()]] [[sqlite3_backup_pagecount()]]">[[sqlite3_backup__remaining()]] [[sqlite3_backup_pagecount()]]</param>
///<param name="<b>sqlite3_backup_remaining() and sqlite3_backup_pagecount()</b>"><b>sqlite3_backup_remaining() and sqlite3_backup_pagecount()</b></param>
///<param name=""></param>
///<param name="^Each call to sqlite3_backup_step() sets two values inside">^Each call to sqlite3_backup_step() sets two values inside</param>
///<param name="the [sqlite3_backup] object: the number of pages still to be backed">the [sqlite3_backup] object: the number of pages still to be backed</param>
///<param name="up and the total number of pages in the source database file.">up and the total number of pages in the source database file.</param>
///<param name="The sqlite3_backup_remaining() and sqlite3_backup_pagecount() interfaces">The sqlite3_backup_remaining() and sqlite3_backup_pagecount() interfaces</param>
///<param name="retrieve these two values, respectively.">retrieve these two values, respectively.</param>
///<param name=""></param>
///<param name="^The values returned by these functions are only updated by">^The values returned by these functions are only updated by</param>
///<param name="sqlite3_backup_step(). ^If the source database is modified during a backup">sqlite3_backup_step(). ^If the source database is modified during a backup</param>
///<param name="operation, then the values are not updated to account for any extra">operation, then the values are not updated to account for any extra</param>
///<param name="pages that need to be updated or the size of the source database file">pages that need to be updated or the size of the source database file</param>
///<param name="changing.">changing.</param>
///<param name=""></param>
///<param name="<b>Concurrent Usage of Database Handles</b>"><b>Concurrent Usage of Database Handles</b></param>
///<param name=""></param>
///<param name="^The source [database connection] may be used by the application for other">^The source [database connection] may be used by the application for other</param>
///<param name="purposes while a backup operation is underway or being initialized.">purposes while a backup operation is underway or being initialized.</param>
///<param name="^If SQLite is compiled and configured to support threadsafe database">^If SQLite is compiled and configured to support threadsafe database</param>
///<param name="connections, then the source database connection may be used concurrently">connections, then the source database connection may be used concurrently</param>
///<param name="from within other threads.">from within other threads.</param>
///<param name=""></param>
///<param name="However, the application must guarantee that the destination ">However, the application must guarantee that the destination </param>
///<param name="[database connection] is not passed to any other API (by any thread) after ">[database connection] is not passed to any other API (by any thread) after </param>
///<param name="sqlite3_backup_init() is called and before the corresponding call to">sqlite3_backup_init() is called and before the corresponding call to</param>
///<param name="sqlite3_backup_finish().  SQLite does not currently check to see">sqlite3_backup_finish().  SQLite does not currently check to see</param>
///<param name="if the application incorrectly accesses the destination [database connection]">if the application incorrectly accesses the destination [database connection]</param>
///<param name="and so no error code is reported, but the operations may malfunction">and so no error code is reported, but the operations may malfunction</param>
///<param name="nevertheless.  Use of the destination database connection while a">nevertheless.  Use of the destination database connection while a</param>
///<param name="backup is in progress might also also cause a mutex deadlock.">backup is in progress might also also cause a mutex deadlock.</param>
///<param name=""></param>
///<param name="If running in [shared cache mode], the application must">If running in [shared cache mode], the application must</param>
///<param name="guarantee that the shared cache used by the destination database">guarantee that the shared cache used by the destination database</param>
///<param name="is not accessed while the backup is running. In practice this means">is not accessed while the backup is running. In practice this means</param>
///<param name="that the application must guarantee that the disk file being ">that the application must guarantee that the disk file being </param>
///<param name="backed up to is not accessed by any connection within the process,">backed up to is not accessed by any connection within the process,</param>
///<param name="not just the specific connection that was passed to sqlite3_backup_init().">not just the specific connection that was passed to sqlite3_backup_init().</param>
///<param name=""></param>
///<param name="The [sqlite3_backup] object itself is partially threadsafe. Multiple ">The [sqlite3_backup] object itself is partially threadsafe. Multiple </param>
///<param name="threads may safely make multiple concurrent calls to sqlite3_backup_step().">threads may safely make multiple concurrent calls to sqlite3_backup_step().</param>
///<param name="However, the sqlite3_backup_remaining() and sqlite3_backup_pagecount()">However, the sqlite3_backup_remaining() and sqlite3_backup_pagecount()</param>
///<param name="APIs are not strictly speaking threadsafe. If they are invoked at the">APIs are not strictly speaking threadsafe. If they are invoked at the</param>
///<param name="same time as another thread is invoking sqlite3_backup_step() it is">same time as another thread is invoking sqlite3_backup_step() it is</param>
///<param name="possible that they return invalid values.">possible that they return invalid values.</param>
///<param name=""></param>

		//SQLITE_API sqlite3_backup *sqlite3_backup_init(
		//  sqlite3 *pDest,                        /* Destination database handle */
		//  string zDestName,                 /* Destination database name */
		//  sqlite3 *pSource,                      /* Source database handle */
		//  string zSourceName                /* Source database name */
		//);
		//SQLITE_API int sqlite3_backup_step(sqlite3_backup *p, int nPage);
		//SQLITE_API int sqlite3_backup_finish(sqlite3_backup *p);
		//SQLITE_API int sqlite3_backup_remaining(sqlite3_backup *p);
		//SQLITE_API int sqlite3_backup_pagecount(sqlite3_backup *p);
		///
///<summary>
///CAPI3REF: Unlock Notification
///
///</summary>
///<param name="^When running in shared">cache mode, a database operation may fail with</param>
///<param name="an [SQLITE_LOCKED] error if the required locks on the shared">cache or</param>
///<param name="individual tables within the shared">cache cannot be obtained. See</param>
///<param name="[SQLite Shared">cache locking. </param>
///<param name="^This API may be used to register a callback that SQLite will invoke ">^This API may be used to register a callback that SQLite will invoke </param>
///<param name="when the connection currently holding the required lock relinquishes it.">when the connection currently holding the required lock relinquishes it.</param>
///<param name="^This API is only available if the library was compiled with the">^This API is only available if the library was compiled with the</param>
///<param name="[SQLITE_ENABLE_UNLOCK_NOTIFY] C">preprocessor symbol defined.</param>
///<param name=""></param>
///<param name="See Also: [Using the SQLite Unlock Notification Feature].">See Also: [Using the SQLite Unlock Notification Feature].</param>
///<param name=""></param>
///<param name="^Shared">cache locks are released when a database connection concludes</param>
///<param name="its current transaction, either by committing it or rolling it back. ">its current transaction, either by committing it or rolling it back. </param>
///<param name=""></param>
///<param name="^When a connection (known as the blocked connection) fails to obtain a">^When a connection (known as the blocked connection) fails to obtain a</param>
///<param name="shared">cache lock and SQLITE_LOCKED is returned to the caller, the</param>
///<param name="identity of the database connection (the blocking connection) that">identity of the database connection (the blocking connection) that</param>
///<param name="has locked the required resource is stored internally. ^After an ">has locked the required resource is stored internally. ^After an </param>
///<param name="application receives an SQLITE_LOCKED error, it may call the">application receives an SQLITE_LOCKED error, it may call the</param>
///<param name="sqlite3_unlock_notify() method with the blocked connection handle as ">sqlite3_unlock_notify() method with the blocked connection handle as </param>
///<param name="the first argument to register for a callback that will be invoked">the first argument to register for a callback that will be invoked</param>
///<param name="when the blocking connections current transaction is concluded. ^The">when the blocking connections current transaction is concluded. ^The</param>
///<param name="callback is invoked from within the [sqlite3_step] or [sqlite3_close]">callback is invoked from within the [sqlite3_step] or [sqlite3_close]</param>
///<param name="call that concludes the blocking connections transaction.">call that concludes the blocking connections transaction.</param>
///<param name=""></param>
///<param name="^(If sqlite3_unlock_notify() is called in a multi">threaded application,</param>
///<param name="there is a chance that the blocking connection will have already">there is a chance that the blocking connection will have already</param>
///<param name="concluded its transaction by the time sqlite3_unlock_notify() is invoked.">concluded its transaction by the time sqlite3_unlock_notify() is invoked.</param>
///<param name="If this happens, then the specified callback is invoked immediately,">If this happens, then the specified callback is invoked immediately,</param>
///<param name="from within the call to sqlite3_unlock_notify().)^">from within the call to sqlite3_unlock_notify().)^</param>
///<param name=""></param>
///<param name="^If the blocked connection is attempting to obtain a write">lock on a</param>
///<param name="shared">cache table, and more than one other connection currently holds</param>
///<param name="a read">lock on the same table, then SQLite arbitrarily selects one of </param>
///<param name="the other connections to use as the blocking connection.">the other connections to use as the blocking connection.</param>
///<param name=""></param>
///<param name="^(There may be at most one unlock">notify callback registered by a </param>
///<param name="blocked connection. If sqlite3_unlock_notify() is called when the">blocked connection. If sqlite3_unlock_notify() is called when the</param>
///<param name="blocked connection already has a registered unlock">notify callback,</param>
///<param name="then the new callback replaces the old.)^ ^If sqlite3_unlock_notify() is">then the new callback replaces the old.)^ ^If sqlite3_unlock_notify() is</param>
///<param name="called with a NULL pointer as its second argument, then any existing">called with a NULL pointer as its second argument, then any existing</param>
///<param name="unlock">notify callback is canceled. ^The blocked connections </param>
///<param name="unlock">notify callback may also be canceled by closing the blocked</param>
///<param name="connection using [sqlite3_close()].">connection using [sqlite3_close()].</param>
///<param name=""></param>
///<param name="The unlock">notify callback is not reentrant. If an application invokes</param>
///<param name="any sqlite3_xxx API functions from within an unlock">notify callback, a</param>
///<param name="crash or deadlock may be the result.">crash or deadlock may be the result.</param>
///<param name=""></param>
///<param name="^Unless deadlock is detected (see below), sqlite3_unlock_notify() always">^Unless deadlock is detected (see below), sqlite3_unlock_notify() always</param>
///<param name="returns SqlResult.SQLITE_OK.">returns SqlResult.SQLITE_OK.</param>
///<param name=""></param>
///<param name="<b>Callback Invocation Details</b>"><b>Callback Invocation Details</b></param>
///<param name=""></param>
///<param name="When an unlock">notify callback is registered, the application provides a </param>
///<param name="single void* pointer that is passed to the callback when it is invoked.">single void* pointer that is passed to the callback when it is invoked.</param>
///<param name="However, the signature of the callback function allows SQLite to pass">However, the signature of the callback function allows SQLite to pass</param>
///<param name="it an array of void* context pointers. The first argument passed to">it an array of void* context pointers. The first argument passed to</param>
///<param name="an unlock">notify callback is a pointer to an array of void* pointers,</param>
///<param name="and the second is the number of entries in the array.">and the second is the number of entries in the array.</param>
///<param name=""></param>
///<param name="When a blocking connections transaction is concluded, there may be">When a blocking connections transaction is concluded, there may be</param>
///<param name="more than one blocked connection that has registered for an unlock">notify</param>
///<param name="callback. ^If two or more such blocked connections have specified the">callback. ^If two or more such blocked connections have specified the</param>
///<param name="same callback function, then instead of invoking the callback function">same callback function, then instead of invoking the callback function</param>
///<param name="multiple times, it is invoked once with the set of void* context pointers">multiple times, it is invoked once with the set of void* context pointers</param>
///<param name="specified by the blocked connections bundled together into an array.">specified by the blocked connections bundled together into an array.</param>
///<param name="This gives the application an opportunity to prioritize any actions ">This gives the application an opportunity to prioritize any actions </param>
///<param name="related to the set of unblocked database connections.">related to the set of unblocked database connections.</param>
///<param name=""></param>
///<param name="<b>Deadlock Detection</b>"><b>Deadlock Detection</b></param>
///<param name=""></param>
///<param name="Assuming that after registering for an unlock">notify callback a </param>
///<param name="database waits for the callback to be issued before taking any further">database waits for the callback to be issued before taking any further</param>
///<param name="action (a reasonable assumption), then using this API may cause the">action (a reasonable assumption), then using this API may cause the</param>
///<param name="application to deadlock. For example, if connection X is waiting for">application to deadlock. For example, if connection X is waiting for</param>
///<param name="connection Y's transaction to be concluded, and similarly connection">connection Y's transaction to be concluded, and similarly connection</param>
///<param name="Y is waiting on connection X's transaction, then neither connection">Y is waiting on connection X's transaction, then neither connection</param>
///<param name="will proceed and the system may remain deadlocked indefinitely.">will proceed and the system may remain deadlocked indefinitely.</param>
///<param name=""></param>
///<param name="To avoid this scenario, the sqlite3_unlock_notify() performs deadlock">To avoid this scenario, the sqlite3_unlock_notify() performs deadlock</param>
///<param name="detection. ^If a given call to sqlite3_unlock_notify() would put the">detection. ^If a given call to sqlite3_unlock_notify() would put the</param>
///<param name="system in a deadlocked state, then SQLITE_LOCKED is returned and no">system in a deadlocked state, then SQLITE_LOCKED is returned and no</param>
///<param name="unlock">notify callback is registered. The system is said to be in</param>
///<param name="a deadlocked state if connection A has registered for an unlock">notify</param>
///<param name="callback on the conclusion of connection B's transaction, and connection">callback on the conclusion of connection B's transaction, and connection</param>
///<param name="B has itself registered for an unlock">notify callback when connection</param>
///<param name="A's transaction is concluded. ^Indirect deadlock is also detected, so">A's transaction is concluded. ^Indirect deadlock is also detected, so</param>
///<param name="the system is also considered to be deadlocked if connection B has">the system is also considered to be deadlocked if connection B has</param>
///<param name="registered for an unlock">notify callback on the conclusion of connection</param>
///<param name="C's transaction, where connection C is waiting on connection A. ^Any">C's transaction, where connection C is waiting on connection A. ^Any</param>
///<param name="number of levels of indirection are allowed.">number of levels of indirection are allowed.</param>
///<param name=""></param>
///<param name="<b>The "DROP TABLE" Exception</b>"><b>The "DROP TABLE" Exception</b></param>
///<param name=""></param>
///<param name="When a call to [sqlite3_step()] returns SQLITE_LOCKED, it is almost ">When a call to [sqlite3_step()] returns SQLITE_LOCKED, it is almost </param>
///<param name="always appropriate to call sqlite3_unlock_notify(). There is however,">always appropriate to call sqlite3_unlock_notify(). There is however,</param>
///<param name="one exception. When executing a "DROP TABLE" or "DROP INDEX" statement,">one exception. When executing a "DROP TABLE" or "DROP INDEX" statement,</param>
///<param name="SQLite checks if there are any currently executing SELECT statements">SQLite checks if there are any currently executing SELECT statements</param>
///<param name="that belong to the same connection. If there are, SQLITE_LOCKED is">that belong to the same connection. If there are, SQLITE_LOCKED is</param>
///<param name="returned. In this case there is no "blocking connection", so invoking">returned. In this case there is no "blocking connection", so invoking</param>
///<param name="sqlite3_unlock_notify() results in the unlock">notify callback being</param>
///<param name="invoked immediately. If the application then re">attempts the "DROP TABLE"</param>
///<param name="or "DROP INDEX" query, an infinite loop might be the result.">or "DROP INDEX" query, an infinite loop might be the result.</param>
///<param name=""></param>
///<param name="One way around this problem is to check the extended error code returned">One way around this problem is to check the extended error code returned</param>
///<param name="by an sqlite3_step() call. ^(If there is a blocking connection, then the">by an sqlite3_step() call. ^(If there is a blocking connection, then the</param>
///<param name="extended error code is set to SQLITE_LOCKED_SHAREDCACHE. Otherwise, in">extended error code is set to SQLITE_LOCKED_SHAREDCACHE. Otherwise, in</param>
///<param name="the special "DROP TABLE/INDEX" case, the extended error code is just ">the special "DROP TABLE/INDEX" case, the extended error code is just </param>
///<param name="SQLITE_LOCKED.)^">SQLITE_LOCKED.)^</param>
///<param name=""></param>

		//SQLITE_API int sqlite3_unlock_notify(
		//  sqlite3 *pBlocked,                          /* Waiting connection */
		//  void (*xNotify)(void **apArg, int nArg),    /* Callback function to invoke */
		//  void *pNotifyArg                            /* Argument to pass to xNotify */
		//);
		///
///<summary>
///CAPI3REF: String Comparison
///
///^The [sqlite3_strnicmp()] API allows applications and extensions to
///</summary>
///<param name="compare the contents of two buffers containing UTF">8 strings in a</param>
///<param name="case">independent fashion, using the same definition of case independence </param>
///<param name="that SQLite uses internally when comparing identifiers.">that SQLite uses internally when comparing identifiers.</param>
///<param name=""></param>

		//SQLITE_API int sqlite3_strnicmp(string , string , int);
		///
///<summary>
///CAPI3REF: Error Logging Interface
///
///^The [io.sqlite3_log()] interface writes a message into the error log
///established by the [SQLITE_CONFIG_LOG] option to [sqlite3_config()].
///^If logging is enabled, the zFormat string and subsequent arguments are
///used with [sqlite3_snprintf()] to generate the final output string.
///
///The io.sqlite3_log() interface is intended for use by extensions such as
///virtual tables, collating functions, and SQL functions.  While there is
///nothing to prevent an application from calling io.sqlite3_log(), doing so
///is considered bad form.
///
///The zFormat string must not be NULL.
///
///To avoid deadlocks and other threading problems, the io.sqlite3_log() routine
///will not use dynamically allocated memory.  The log message is stored in
///</summary>
///<param name="a fixed">length buffer on the stack.  If the log message is longer than</param>
///<param name="a few hundred characters, it will be truncated to the length of the">a few hundred characters, it will be truncated to the length of the</param>
///<param name="buffer.">buffer.</param>
///<param name=""></param>

		//SQLITE_API void io.sqlite3_log(int iErrCode, string zFormat, ...);
		///
///<summary>
///</summary>
///<param name="CAPI3REF: Write">Ahead Log Commit Hook</param>
///<param name=""></param>
///<param name="^The [sqlite3_wal_hook()] function is used to register a callback that">^The [sqlite3_wal_hook()] function is used to register a callback that</param>
///<param name="will be invoked each time a database connection commits data to a">will be invoked each time a database connection commits data to a</param>
///<param name="[write">ahead log] (i.e. whenever a transaction is committed in</param>
///<param name="[journal_mode | journal_mode=WAL mode]). ">[journal_mode | journal_mode=WAL mode]). </param>
///<param name=""></param>
///<param name="^The callback is invoked by SQLite after the commit has taken place and ">^The callback is invoked by SQLite after the commit has taken place and </param>
///<param name="the associated write">lock on the database released, so the implementation </param>
///<param name="may read, write or [checkpoint] the database as required.">may read, write or [checkpoint] the database as required.</param>
///<param name=""></param>
///<param name="^The first parameter passed to the callback function when it is invoked">^The first parameter passed to the callback function when it is invoked</param>
///<param name="is a copy of the third parameter passed to sqlite3_wal_hook() when">is a copy of the third parameter passed to sqlite3_wal_hook() when</param>
///<param name="registering the callback. ^The second is a copy of the database handle.">registering the callback. ^The second is a copy of the database handle.</param>
///<param name="^The third parameter is the name of the database that was written to "></param>
///<param name="either "main" or the name of an [ATTACH]">ed database. ^The fourth parameter</param>
///<param name="is the number of pages currently in the write">ahead log file,</param>
///<param name="including those that were just committed.">including those that were just committed.</param>
///<param name=""></param>
///<param name="The callback function should normally return [SqlResult.SQLITE_OK].  ^If an error">The callback function should normally return [SqlResult.SQLITE_OK].  ^If an error</param>
///<param name="code is returned, that error will propagate back up through the">code is returned, that error will propagate back up through the</param>
///<param name="SQLite code base to cause the statement that provoked the callback">SQLite code base to cause the statement that provoked the callback</param>
///<param name="to report an error, though the commit will have still occurred. If the">to report an error, though the commit will have still occurred. If the</param>
///<param name="callback returns [SQLITE_ROW] or [SQLITE_DONE], or if it returns a value">callback returns [SQLITE_ROW] or [SQLITE_DONE], or if it returns a value</param>
///<param name="that does not correspond to any valid SQLite error code, the results">that does not correspond to any valid SQLite error code, the results</param>
///<param name="are undefined.">are undefined.</param>
///<param name=""></param>
///<param name="A single database handle may have at most a single write">ahead log callback </param>
///<param name="registered at one time. ^Calling [sqlite3_wal_hook()] replaces any">registered at one time. ^Calling [sqlite3_wal_hook()] replaces any</param>
///<param name="previously registered write">ahead log callback. ^Note that the</param>
///<param name="[sqlite3_wal_autocheckpoint()] interface and the">[sqlite3_wal_autocheckpoint()] interface and the</param>
///<param name="[wal_autocheckpoint pragma] both invoke [sqlite3_wal_hook()] and will">[wal_autocheckpoint pragma] both invoke [sqlite3_wal_hook()] and will</param>
///<param name="those overwrite any prior [sqlite3_wal_hook()] settings.">those overwrite any prior [sqlite3_wal_hook()] settings.</param>
///<param name=""></param>

		//SQLITE_API void *sqlite3_wal_hook(
		//  sqlite3*, 
		//  int()(void *,sqlite3*,const char*,int),
		//  void*
		//);
		///
///<summary>
///</summary>
///<param name="CAPI3REF: Configure an auto">checkpoint</param>
///<param name=""></param>
///<param name="^The [sqlite3_wal_autocheckpoint(D,N)] is a wrapper around">^The [sqlite3_wal_autocheckpoint(D,N)] is a wrapper around</param>
///<param name="[sqlite3_wal_hook()] that causes any database on [database connection] D">[sqlite3_wal_hook()] that causes any database on [database connection] D</param>
///<param name="to automatically [checkpoint]">to automatically [checkpoint]</param>
///<param name="after committing a transaction if there are N or">after committing a transaction if there are N or</param>
///<param name="more frames in the [write">ahead log] file.  ^Passing zero or </param>
///<param name="a negative value as the nFrame parameter disables automatic">a negative value as the nFrame parameter disables automatic</param>
///<param name="checkpoints entirely.">checkpoints entirely.</param>
///<param name=""></param>
///<param name="^The callback registered by this function replaces any existing callback">^The callback registered by this function replaces any existing callback</param>
///<param name="registered using [sqlite3_wal_hook()].  ^Likewise, registering a callback">registered using [sqlite3_wal_hook()].  ^Likewise, registering a callback</param>
///<param name="using [sqlite3_wal_hook()] disables the automatic checkpoint mechanism">using [sqlite3_wal_hook()] disables the automatic checkpoint mechanism</param>
///<param name="configured by this function.">configured by this function.</param>
///<param name=""></param>
///<param name="^The [wal_autocheckpoint pragma] can be used to invoke this interface">^The [wal_autocheckpoint pragma] can be used to invoke this interface</param>
///<param name="from SQL.">from SQL.</param>
///<param name=""></param>
///<param name="^Every new [database connection] defaults to having the auto">checkpoint</param>
///<param name="enabled with a threshold of 1000 or [SQLITE_DEFAULT_WAL_AUTOCHECKPOINT]">enabled with a threshold of 1000 or [SQLITE_DEFAULT_WAL_AUTOCHECKPOINT]</param>
///<param name="pages.  The use of this interface">pages.  The use of this interface</param>
///<param name="for a particular application.">for a particular application.</param>
///<param name=""></param>

		//SQLITE_API int sqlite3_wal_autocheckpoint(sqlite3 db, int N);
		///
///<summary>
///CAPI3REF: Checkpoint a database
///
///^The [sqlite3_wal_checkpoint(D,X)] interface causes database named X
///on [database connection] D to be [checkpointed].  ^If X is NULL or an
///empty string, then a checkpoint is run on all databases of
///connection D.  ^If the database connection D is not in
///</summary>
///<param name="[WAL | write">op.</param>
///<param name=""></param>
///<param name="^The [wal_checkpoint pragma] can be used to invoke this interface">^The [wal_checkpoint pragma] can be used to invoke this interface</param>
///<param name="from SQL.  ^The [sqlite3_wal_autocheckpoint()] interface and the">from SQL.  ^The [sqlite3_wal_autocheckpoint()] interface and the</param>
///<param name="[wal_autocheckpoint pragma] can be used to cause this interface to be">[wal_autocheckpoint pragma] can be used to cause this interface to be</param>
///<param name="run whenever the WAL reaches a certain size threshold.">run whenever the WAL reaches a certain size threshold.</param>
///<param name=""></param>
///<param name="See also: [sqlite3_wal_checkpoint_v2()]">See also: [sqlite3_wal_checkpoint_v2()]</param>
///<param name=""></param>

		//SQLITE_API int sqlite3_wal_checkpoint(sqlite3 db, string zDb);
		///
///<summary>
///CAPI3REF: Checkpoint a database
///
///Run a checkpoint operation on WAL database zDb attached to database 
///handle db. The specific operation is determined by the value of the 
///eMode parameter:
///
///<dl>
///<dt>SQLITE_CHECKPOINT_PASSIVE<dd>
///Checkpoint as many frames as possible without waiting for any database 
///readers or writers to finish. Sync the db file if all frames in the log
///are checkpointed. This mode is the same as calling 
///</summary>
///<param name="sqlite3_wal_checkpoint(). The busy">handler callback is never invoked.</param>
///<param name=""></param>
///<param name="<dt>SQLITE_CHECKPOINT_FULL<dd>"><dt>SQLITE_CHECKPOINT_FULL<dd></param>
///<param name="This mode blocks (calls the busy">handler callback) until there is no</param>
///<param name="database writer and all readers are reading from the most recent database">database writer and all readers are reading from the most recent database</param>
///<param name="snapshot. It then checkpoints all frames in the log file and syncs the">snapshot. It then checkpoints all frames in the log file and syncs the</param>
///<param name="database file. This call blocks database writers while it is running,">database file. This call blocks database writers while it is running,</param>
///<param name="but not database readers.">but not database readers.</param>
///<param name=""></param>
///<param name="<dt>SQLITE_CHECKPOINT_RESTART<dd>"><dt>SQLITE_CHECKPOINT_RESTART<dd></param>
///<param name="This mode works the same way as SQLITE_CHECKPOINT_FULL, except after ">This mode works the same way as SQLITE_CHECKPOINT_FULL, except after </param>
///<param name="checkpointing the log file it blocks (calls the busy">handler callback)</param>
///<param name="until all readers are reading from the database file only. This ensures ">until all readers are reading from the database file only. This ensures </param>
///<param name="that the next client to write to the database file restarts the log file ">that the next client to write to the database file restarts the log file </param>
///<param name="from the beginning. This call blocks database writers while it is running,">from the beginning. This call blocks database writers while it is running,</param>
///<param name="but not database readers.">but not database readers.</param>
///<param name="</dl>"></dl></param>
///<param name=""></param>
///<param name="If pnLog is not NULL, then *pnLog is set to the total number of frames in">If pnLog is not NULL, then *pnLog is set to the total number of frames in</param>
///<param name="the log file before returning. If pnCkpt is not NULL, then *pnCkpt is set to">the log file before returning. If pnCkpt is not NULL, then *pnCkpt is set to</param>
///<param name="the total number of checkpointed frames (including any that were already">the total number of checkpointed frames (including any that were already</param>
///<param name="checkpointed when this function is called). *pnLog and *pnCkpt may be">checkpointed when this function is called). *pnLog and *pnCkpt may be</param>
///<param name="populated even if sqlite3_wal_checkpoint_v2() returns other than SqlResult.SQLITE_OK.">populated even if sqlite3_wal_checkpoint_v2() returns other than SqlResult.SQLITE_OK.</param>
///<param name="If no values are available because of an error, they are both set to ">1</param>
///<param name="before returning to communicate this to the caller.">before returning to communicate this to the caller.</param>
///<param name=""></param>
///<param name="All calls obtain an exclusive "checkpoint" lock on the database file. If">All calls obtain an exclusive "checkpoint" lock on the database file. If</param>
///<param name="any other process is running a checkpoint operation at the same time, the ">any other process is running a checkpoint operation at the same time, the </param>
///<param name="lock cannot be obtained and SQLITE_BUSY is returned. Even if there is a ">lock cannot be obtained and SQLITE_BUSY is returned. Even if there is a </param>
///<param name="busy">handler configured, it will not be invoked in this case.</param>
///<param name=""></param>
///<param name="The SQLITE_CHECKPOINT_FULL and RESTART modes also obtain the exclusive ">The SQLITE_CHECKPOINT_FULL and RESTART modes also obtain the exclusive </param>
///<param name=""writer" lock on the database file. If the writer lock cannot be obtained">"writer" lock on the database file. If the writer lock cannot be obtained</param>
///<param name="immediately, and a busy">handler is configured, it is invoked and the writer</param>
///<param name="lock retried until either the busy">handler returns 0 or the lock is</param>
///<param name="successfully obtained. The busy">handler is also invoked while waiting for</param>
///<param name="database readers as described above. If the busy">handler returns 0 before</param>
///<param name="the writer lock is obtained or while waiting for database readers, the">the writer lock is obtained or while waiting for database readers, the</param>
///<param name="checkpoint operation proceeds from that point in the same way as ">checkpoint operation proceeds from that point in the same way as </param>
///<param name="SQLITE_CHECKPOINT_PASSIVE "> checkpointing as many frames as possible </param>
///<param name="without blocking any further. SQLITE_BUSY is returned in this case.">without blocking any further. SQLITE_BUSY is returned in this case.</param>
///<param name=""></param>
///<param name="If parameter zDb is NULL or points to a zero length string, then the">If parameter zDb is NULL or points to a zero length string, then the</param>
///<param name="specified operation is attempted on all WAL databases. In this case the">specified operation is attempted on all WAL databases. In this case the</param>
///<param name="values written to output parameters *pnLog and *pnCkpt are undefined. If ">values written to output parameters *pnLog and *pnCkpt are undefined. If </param>
///<param name="an SQLITE_BUSY error is encountered when processing one or more of the ">an SQLITE_BUSY error is encountered when processing one or more of the </param>
///<param name="attached WAL databases, the operation is still attempted on any remaining ">attached WAL databases, the operation is still attempted on any remaining </param>
///<param name="attached databases and SQLITE_BUSY is returned to the caller. If any other ">attached databases and SQLITE_BUSY is returned to the caller. If any other </param>
///<param name="error occurs while processing an attached database, processing is abandoned ">error occurs while processing an attached database, processing is abandoned </param>
///<param name="and the error code returned to the caller immediately. If no error ">and the error code returned to the caller immediately. If no error </param>
///<param name="(SQLITE_BUSY or otherwise) is encountered while processing the attached ">(SQLITE_BUSY or otherwise) is encountered while processing the attached </param>
///<param name="databases, SqlResult.SQLITE_OK is returned.">databases, SqlResult.SQLITE_OK is returned.</param>
///<param name=""></param>
///<param name="If database zDb is the name of an attached database that is not in WAL">If database zDb is the name of an attached database that is not in WAL</param>
///<param name="mode, SqlResult.SQLITE_OK is returned and both *pnLog and *pnCkpt set to ">1. If</param>
///<param name="zDb is not NULL (or a zero length string) and is not the name of any">zDb is not NULL (or a zero length string) and is not the name of any</param>
///<param name="attached database, SqlResult.SQLITE_ERROR is returned to the caller.">attached database, SqlResult.SQLITE_ERROR is returned to the caller.</param>
///<param name=""></param>

		//SQLITE_API int sqlite3_wal_checkpoint_v2(
		//  sqlite3 db,                    /* Database handle */
		//  string zDb,                /* Name of attached database (or NULL) */
		//  int eMode,                      /* SQLITE_CHECKPOINT_* value */
		//  int *pnLog,                     /* OUT: Size of WAL log in frames */
		//  int *pnCkpt                     /* OUT: Total number of frames checkpointed */
		//);
		///
///<summary>
///CAPI3REF: Checkpoint operation parameters
///
///These constants can be used as the 3rd parameter to
///[sqlite3_wal_checkpoint_v2()].  See the [sqlite3_wal_checkpoint_v2()]
///documentation for additional information about the meaning and use of
///each of these values.
///
///</summary>

		//#define SQLITE_CHECKPOINT_PASSIVE 0
		//#define SQLITE_CHECKPOINT_FULL    1
		//#define SQLITE_CHECKPOINT_RESTART 2
		static public int SQLITE_CHECKPOINT_PASSIVE = 0;

		static public int SQLITE_CHECKPOINT_FULL = 1;

		static public int SQLITE_CHECKPOINT_RESTART = 2;

		///
///<summary>
///CAPI3REF: Virtual Table Interface Configuration
///
///This function may be called by either the [xConnect] or [xCreate] method
///of a [virtual table] implementation to configure
///various facets of the virtual table interface.
///
///If this interface is invoked outside the context of an xConnect or
///xCreate virtual table method then the behavior is undefined.
///
///At present, there is only one option that may be configured using
///this function. (See [SQLITE_VTAB_CONSTRAINT_SUPPORT].)  Further options
///may be added in the future.
///
///</summary>

		//SQLITE_API int sqlite3_vtab_config(sqlite3*, int op, ...);
		///
///<summary>
///CAPI3REF: Virtual Table Configuration Options
///
///These macros define the various options to the
///[sqlite3_vtab_config()] interface that [virtual table] implementations
///can use to customize and optimize their behavior.
///
///<dl>
///<dt>SQLITE_VTAB_CONSTRAINT_SUPPORT
///<dd>Calls of the form
///[sqlite3_vtab_config](db,SQLITE_VTAB_CONSTRAINT_SUPPORT,X) are supported,
///where X is an integer.  If X is zero, then the [virtual table] whose
///[xCreate] or [xConnect] method invoked [sqlite3_vtab_config()] does not
///support constraints.  In this configuration (which is the default) if
///a call to the [xUpdate] method returns [SQLITE_CONSTRAINT], then the entire
///statement is rolled back as if [ON CONFLICT | OR ABORT] had been
///specified as part of the users SQL statement, regardless of the actual
///ON CONFLICT mode specified.
///
///</summary>
///<param name="If X is non">zero, then the virtual table implementation guarantees</param>
///<param name="that if [xUpdate] returns [SQLITE_CONSTRAINT], it will do so before">that if [xUpdate] returns [SQLITE_CONSTRAINT], it will do so before</param>
///<param name="any modifications to internal or persistent data structures have been made.">any modifications to internal or persistent data structures have been made.</param>
///<param name="If the [ON CONFLICT] mode is ABORT, FAIL, IGNORE or ROLLBACK, SQLite ">If the [ON CONFLICT] mode is ABORT, FAIL, IGNORE or ROLLBACK, SQLite </param>
///<param name="is able to roll back a statement or database transaction, and abandon">is able to roll back a statement or database transaction, and abandon</param>
///<param name="or continue processing the current SQL statement as appropriate. ">or continue processing the current SQL statement as appropriate. </param>
///<param name="If the ON CONFLICT mode is REPLACE and the [xUpdate] method returns">If the ON CONFLICT mode is REPLACE and the [xUpdate] method returns</param>
///<param name="[SQLITE_CONSTRAINT], SQLite handles this as if the ON CONFLICT mode">[SQLITE_CONSTRAINT], SQLite handles this as if the ON CONFLICT mode</param>
///<param name="had been ABORT.">had been ABORT.</param>
///<param name=""></param>
///<param name="Virtual table implementations that are required to handle OR REPLACE">Virtual table implementations that are required to handle OR REPLACE</param>
///<param name="must do so within the [xUpdate] method. If a call to the ">must do so within the [xUpdate] method. If a call to the </param>
///<param name="[sqlite3_vtab_on_conflict()] function indicates that the current ON ">[sqlite3_vtab_on_conflict()] function indicates that the current ON </param>
///<param name="CONFLICT policy is REPLACE, the virtual table implementation should ">CONFLICT policy is REPLACE, the virtual table implementation should </param>
///<param name="silently replace the appropriate rows within the xUpdate callback and">silently replace the appropriate rows within the xUpdate callback and</param>
///<param name="return SqlResult.SQLITE_OK. Or, if this is not possible, it may return">return SqlResult.SQLITE_OK. Or, if this is not possible, it may return</param>
///<param name="SQLITE_CONSTRAINT, in which case SQLite falls back to OR ABORT ">SQLITE_CONSTRAINT, in which case SQLite falls back to OR ABORT </param>
///<param name="constraint handling.">constraint handling.</param>
///<param name="</dl>"></dl></param>
///<param name=""></param>

		//#define SQLITE_VTAB_CONSTRAINT_SUPPORT 1
		public const int SQLITE_VTAB_CONSTRAINT_SUPPORT = 1;

		///
///<summary>
///CAPI3REF: Determine The Virtual Table Conflict Policy
///
///This function may only be called from within a call to the [xUpdate] method
///of a [virtual table] implementation for an INSERT or UPDATE operation. ^The
///value returned is one of [SQLITE_ROLLBACK], [SQLITE_IGNORE], [SQLITE_FAIL],
///[SQLITE_ABORT], or [SQLITE_REPLACE], according to the [ON CONFLICT] mode
///of the SQL statement that triggered the call to the [xUpdate] method of the
///[virtual table].
///
///</summary>

		
	///
///<summary>
///Undo the hack that converts floating point types to integer for
///builds on processors without floating point support.
///
///</summary>

	//#if SQLITE_OMIT_FLOATING_POINT
	//# undef double
	//#endif
	//#if __cplusplus
	//}  /* End of the 'extern "C"' block */
	//#endif
	//#endif
	///
///<summary>
///2010 August 30
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
///</summary>

	//#if !_SQLITE3RTREE_H_
	//#define _SQLITE3RTREE_H_
	//#if __cplusplus
	//extern "C" {
	//#endif
	//typedef struct sqlite3_rtree_geometry sqlite3_rtree_geometry;
	///
///<summary>
///Register a geometry callback named zGeom that can be used as part of an
///</summary>
///<param name="R">Tree geometry query as follows:</param>
///<param name=""></param>
///<param name="SELECT ... FROM <rtree> WHERE <rtree col> MATCH $zGeom(... params ...)">SELECT ... FROM <rtree> WHERE <rtree col> MATCH $zGeom(... params ...)</param>
///<param name=""></param>

	//SQLITE_API int sqlite3_rtree_geometry_callback(
	//  sqlite3 db,
	//  string zGeom,
	//  int (*xGeom)(sqlite3_rtree_geometry *, int nCoord, double *aCoord, int *pRes),
	//  void *pContext
	//);
	///
///<summary>
///A pointer to a structure of the following type is passed as the first
///argument to callbacks registered using rtree_geometry_callback().
///
///</summary>

	//struct sqlite3_rtree_geometry {
	//  void *pContext;                 /* Copy of pContext passed to s_r_g_c() */
	//  int nParam;                     /* Size of array aParam[] */
	//  double *aParam;                 /* Parameters passed to SQL geom function */
	//  void *pUser;                    /* Callback implementation user data */
	//  void (*xDelUser)(void );       /* Called by SQLite to clean up pUser */
	//};
	//#if __cplusplus
	//}  /* end of the 'extern "C"' block */
	//#endif
	//#endif  /* ifndef _SQLITE3RTREE_H_ */
	}

    ///
    ///<summary>
    ///CAPI3REF: Custom Page Cache Object
    ///
    ///The sqlite3_pcache type is opaque.  It is implemented by
    ///the pluggable module.  The SQLite core has no knowledge of
    ///its size or internal structure and never deals with the
    ///sqlite3_pcache object except by holding and passing pointers
    ///to the object.
    ///
    ///See [sqlite3_pcache_methods] for additional information.
    ///
    ///</summary>

    //typedef struct sqlite3_pcache sqlite3_pcache;
    ///<summary>
    /// CAPI3REF: Application Defined Page Cache.
    /// KEYWORDS: {page cache}
    ///
    /// ^(The [sqlite3_config]([SQLITE_CONFIG_PCACHE], ...) interface can
    /// register an alternative page cache implementation by passing in an
    /// instance of the sqlite3_pcache_methods structure.)^
    /// In many applications, most of the heap memory allocated by
    /// SQLite is used for the page cache.
    /// By implementing a
    /// custom page cache using this API, an application can better control
    /// the amount of memory consumed by SQLite, the way in which
    /// that memory is allocated and released, and the policies used to
    /// determine exactly which parts of a database file are cached and for
    /// how long.
    ///
    /// The alternative page cache mechanism is an
    /// extreme measure that is only needed by the most demanding applications.
    /// The built-in page cache is recommended for most uses.
    ///
    /// ^(The contents of the sqlite3_pcache_methods structure are copied to an
    /// internal buffer by SQLite within the call to [sqlite3_config].  Hence
    /// the application may discard the parameter after the call to
    /// [sqlite3_config()] returns.)^
    ///
    /// [[the xInit() page cache method]]
    /// ^(The xInit() method is called once for each effective
    /// call to [sqlite3_initialize()])^
    /// (usually only once during the lifetime of the process). ^(The xInit()
    /// method is passed a copy of the sqlite3_pcache_methods.pArg value.)^
    /// The intent of the xInit() method is to set up global data structures
    /// required by the custom page cache implementation.
    /// ^(If the xInit() method is NULL, then the
    /// built-in default page cache is used instead of the application defined
    /// page cache.)^
    ///
    /// [[the xShutdown() page cache method]]
    /// ^The xShutdown() method is called by [sqlite3_shutdown()].
    /// It can be used to clean up
    /// any outstanding resources before process shutdown, if required.
    /// ^The xShutdown() method may be NULL.
    ///
    /// ^SQLite automatically serializes calls to the xInit method,
    /// so the xInit method need not be threadsafe.  ^The
    /// xShutdown method is only called from [sqlite3_shutdown()] so it does
    /// not need to be threadsafe either.  All other methods must be threadsafe
    /// in multithreaded applications.
    ///
    /// ^SQLite will never invoke xInit() more than once without an intervening
    /// call to xShutdown().
    ///
    /// [[the xCreate() page cache methods]]
    /// ^SQLite invokes the xCreate() method to construct a new cache instance.
    /// SQLite will typically create one cache instance for each open database file,
    /// though this is not guaranteed. ^The
    /// first parameter, szPage, is the size in bytes of the pages that must
    /// be allocated by the cache.  ^szPage will not be a power of two.  ^szPage
    /// will the page size of the database file that is to be cached plus an
    /// increment (here called "R") of less than 250.  SQLite will use the
    /// extra R bytes on each page to store metadata about the underlying
    /// database page on disk.  The value of R depends
    /// on the SQLite version, the target platform, and how SQLite was compiled.
    /// ^(R is constant for a particular build of SQLite. Except, there are two
    /// distinct values of R when SQLite is compiled with the proprietary
    /// ZIPVFS extension.)^  ^The second argument to
    /// xCreate(), bPurgeable, is true if the cache being created will
    /// be used to cache database pages of a file stored on disk, or
    /// false if it is used for an in-memory database. The cache implementation
    /// does not have to do anything special based with the value of bPurgeable;
    /// it is purely advisory.  ^On a cache where bPurgeable is false, SQLite will
    /// never invoke xUnpin() except to deliberately delete a page.
    /// ^In other words, calls to xUnpin() on a cache with bPurgeable set to
    /// false will always have the "discard" flag set to true.
    /// ^Hence, a cache created with bPurgeable false will
    /// never contain any unpinned pages.
    ///
    /// [[the xCachesize() page cache method]]
    /// ^(The xCachesize() method may be called at any time by SQLite to set the
    /// suggested maximum cache-size (number of pages stored by) the cache
    /// instance passed as the first argument. This is the value configured using
    /// the SQLite "[PRAGMA cache_size]" command.)^  As with the bPurgeable
    /// parameter, the implementation is not required to do anything with this
    /// value; it is advisory only.
    ///
    /// [[the xPagecount() page cache methods]]
    /// The xPagecount() method must return the number of pages currently
    /// stored in the cache, both pinned and unpinned.
    ///
    /// [[the xFetch() page cache methods]]
    /// The xFetch() method locates a page in the cache and returns a pointer to
    /// the page, or a NULL pointer.
    /// A "page", in this context, means a buffer of szPage bytes aligned at an
    /// 8-byte boundary. The page to be fetched is determined by the key. ^The
    /// minimum key value is 1.  After it has been retrieved using xFetch, the page
    /// is considered to be "pinned".
    ///
    /// If the requested page is already in the page cache, then the page cache
    /// implementation must return a pointer to the page buffer with its content
    /// intact.  If the requested page is not already in the cache, then the
    /// cache implementation should use the value of the createFlag
    /// parameter to help it determined what action to take:
    ///
    /// <table border=1 width=85% align=center>
    /// <tr><th> createFlag <th> Behaviour when page is not already in cache
    /// <tr><td> 0 <td> Do not allocate a new page.  Return NULL.
    /// <tr><td> 1 <td> Allocate a new page if it easy and convenient to do so.
    ///                 Otherwise return NULL.
    /// <tr><td> 2 <td> Make every effort to allocate a new page.  Only return
    ///                 NULL if allocating a new page is effectively impossible.
    /// </table>
    ///
    /// ^(SQLite will normally invoke xFetch() with a createFlag of 0 or 1.  SQLite
    /// will only use a createFlag of 2 after a prior call with a createFlag of 1
    /// failed.)^  In between the to xFetch() calls, SQLite may
    /// attempt to unpin one or more cache pages by spilling the content of
    /// pinned pages to disk and synching the operating system disk cache.
    ///
    /// [[the xUnpin() page cache method]]
    /// ^xUnpin() is called by SQLite with a pointer to a currently pinned page
    /// as its second argument.  If the third parameter, discard, is non-zero,
    /// then the page must be evicted from the cache.
    /// ^If the discard parameter is
    /// zero, then the page may be discarded or retained at the discretion of
    /// page cache implementation. ^The page cache implementation
    /// may choose to evict unpinned pages at any time.
    ///
    /// The cache must not perform any reference counting. A single
    /// call to xUnpin() unpins the page regardless of the number of prior calls
    /// to xFetch().
    ///
    /// [[the xRekey() page cache methods]]
    /// The xRekey() method is used to change the key value associated with the
    /// page passed as the second argument. If the cache
    /// previously contains an entry associated with newKey, it must be
    /// discarded. ^Any prior cache entry associated with newKey is guaranteed not
    /// to be pinned.
    ///
    /// When SQLite calls the xTruncate() method, the cache must discard all
    /// existing cache entries with page numbers (keys) greater than or equal
    /// to the value of the iLimit parameter passed to xTruncate(). If any
    /// of these pages are pinned, they are implicitly unpinned, meaning that
    /// they can be safely discarded.
    ///
    /// [[the xDestroy() page cache method]]
    /// ^The xDestroy() method is used to delete a cache allocated by xCreate().
    /// All resources associated with the specified cache should be freed. ^After
    /// calling the xDestroy() method, SQLite considers the [sqlite3_pcache*]
    /// handle invalid, and will not use it with any other sqlite3_pcache_methods
    /// functions.
    ///
    ///</summary>
    //typedef struct sqlite3_pcache_methods sqlite3_pcache_methods;
    //struct sqlite3_pcache_methods {
    //  void *pArg;
    //  int (*xInit)(void);
    //  void (*xShutdown)(void);
    //  sqlite3_pcache *(*xCreate)(int szPage, int bPurgeable);
    //  void (*xCachesize)(sqlite3_pcache*, int nCachesize);
    //  int (*xPagecount)(sqlite3_pcache);
    //  void *(*xFetch)(sqlite3_pcache*, unsigned key, int createFlag);
    //  void (*xUnpin)(sqlite3_pcache*, void*, int discard);
    //  void (*xRekey)(sqlite3_pcache*, void*, unsigned oldKey, unsigned newKey);
    //  void (*xTruncate)(sqlite3_pcache*, unsigned iLimit);
    //  void (*xDestroy)(sqlite3_pcache);
    //};
    public class sqlite3_pcache_methods
    {
        public object pArg;

        dxPC_Init m_xInit;
        public dxPC_Init xInit
        {
            get { return m_xInit; }
            set { m_xInit = value; }
        }

        //int (*xInit)(void);
        public dxPC_Shutdown xShutdown;

        //public void (*xShutdown)(void);
        public dxPC_Create xCreate;

        //public sqlite3_pcache *(*xCreate)(int szPage, int bPurgeable);
        public dxPC_Cachesize xCachesize;

        //public void (*xCachesize)(sqlite3_pcache*, int nCachesize);
        public dxPC_Pagecount xPagecount;

        //public int (*xPagecount)(sqlite3_pcache);
        public dxPC_Fetch xFetch;

        //public void *(*xFetch)(sqlite3_pcache*, unsigned key, int createFlag);
        public dxPC_Unpin xUnpin;

        //public void (*xUnpin)(sqlite3_pcache*, void*, int discard);
        public dxPC_Rekey xRekey;

        //public void (*xRekey)(sqlite3_pcache*, void*, unsigned oldKey, unsigned newKey);
        public dxPC_Truncate xTruncate;

        //public void (*xTruncate)(sqlite3_pcache*, unsigned iLimit);
        public dxPC_Destroy xDestroy;

        //public void (*xDestroy)(sqlite3_pcache);
        public sqlite3_pcache_methods()
        {
        }

        public sqlite3_pcache_methods(object pArg, dxPC_Init xInit, dxPC_Shutdown xShutdown, dxPC_Create xCreate, dxPC_Cachesize xCachesize, dxPC_Pagecount xPagecount, dxPC_Fetch xFetch, dxPC_Unpin xUnpin, dxPC_Rekey xRekey, dxPC_Truncate xTruncate, dxPC_Destroy xDestroy)
        {
            this.pArg = pArg;
            this.xInit = xInit;
            this.xShutdown = xShutdown;
            this.xCreate = xCreate;
            this.xCachesize = xCachesize;
            this.xPagecount = xPagecount;
            this.xFetch = xFetch;
            this.xUnpin = xUnpin;
            this.xRekey = xRekey;
            this.xTruncate = xTruncate;
            this.xDestroy = xDestroy;
        }
    };

	
}
