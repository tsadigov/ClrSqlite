#define SQLITE_OS_WIN
using u32 = System.UInt32;

namespace Community.CsharpSqlite
{

    ///
    ///<summary>
    ///The following values may be passed as the second argument to
    ///sqlite3OsLock(). The various locks exhibit the following semantics:
    ///
    ///SHARED:    Any number of processes may hold a SHARED lock simultaneously.
    ///RESERVED:  A single process may hold a RESERVED lock on a file at
    ///any time. Other processes may hold and obtain new SHARED locks.
    ///PENDING:   A single process may hold a PENDING lock on a file at
    ///any one time. Existing SHARED locks may persist, but no new
    ///SHARED locks may be obtained by other processes.
    ///EXCLUSIVE: An EXCLUSIVE lock precludes all other locks.
    ///
    ///PENDING_LOCK may not be passed directly to sqlite3OsLock(). Instead, a
    ///process that requests an EXCLUSIVE lock may actually obtain a PENDING
    ///lock. This can be upgraded to an EXCLUSIVE lock by a subsequent call to
    ///sqlite3OsLock().
    ///</summary>
    public enum LockType:byte
    {

        NO_LOCK = 0,

        SHARED_LOCK = 1,

        RESERVED_LOCK = 2,

        PENDING_LOCK = 3,

        EXCLUSIVE_LOCK = 4
    }

	public partial class Sqlite3
	{

		///
///<summary>
///2001 September 16
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
///<param name="This header file (together with is companion C source">code file</param>
///<param name=""os.c") attempt to abstract the underlying operating system so that">"os.c") attempt to abstract the underlying operating system so that</param>
///<param name="the SQLite library will work on both POSIX and windows systems.">the SQLite library will work on both POSIX and windows systems.</param>
///<param name=""></param>
///<param name="This header file is #include">ed by sqliteInt.h and thus ends up</param>
///<param name="being included by every source file.">being included by every source file.</param>
///<param name=""></param>
///<param name="Included in SQLite3 port to C#">SQLite;  2008 Noah B Hart</param>
///<param name="C#">SQLite is an independent reimplementation of the SQLite software library</param>
///<param name=""></param>
///<param name="SQLITE_SOURCE_ID: 2010">23 18:52:01 42537b60566f288167f1b5864a5435986838e3a3</param>
///<param name=""></param>
///<param name=""></param>
///<param name=""></param>

		#if !_SQLITE_OS_H_
		//#define _SQLITE_OS_H_
		///
///<summary>
///Figure out if we are dealing with Unix, Windows, or some other
///operating system.  After the following block of preprocess macros,
///all of SQLITE_OS_UNIX, SQLITE_OS_WIN, SQLITE_OS_OS2, and SQLITE_OS_OTHER
///will defined to either 1 or 0.  One of the four will be 1.  The other
///three will be 0.
///
///</summary>

		//#if (SQLITE_OS_OTHER)
		//# if SQLITE_OS_OTHER==1
		//#   undef SQLITE_OS_UNIX
		//#   define SQLITE_OS_UNIX 0
		//#   undef SQLITE_OS_WIN
		//#   define SQLITE_OS_WIN 0
		//#   undef SQLITE_OS_OS2
		//#   define SQLITE_OS_OS2 0
		//# else
		//#   undef SQLITE_OS_OTHER
		//# endif
		//#endif
		//#if !(SQLITE_OS_UNIX) && !SQLITE_OS_OTHER)
		//# define SQLITE_OS_OTHER 0
		//# ifndef SQLITE_OS_WIN
		//#   if defined(_WIN32) || defined(WIN32) || defined(__CYGWIN__) || defined(__MINGW32__) || defined(__BORLANDC__)
		//#     define SQLITE_OS_WIN 1
		//#     define SQLITE_OS_UNIX 0
		//#     define SQLITE_OS_OS2 0
		//#   elif defined(__EMX__) || defined(_OS2) || defined(OS2) || defined(_OS2_) || defined(__OS2__)
		//#     define SQLITE_OS_WIN 0
		//#     define SQLITE_OS_UNIX 0
		//#     define SQLITE_OS_OS2 1
		//#   else
		//#     define SQLITE_OS_WIN 0
		//#     define SQLITE_OS_UNIX 1
		//#     define SQLITE_OS_OS2 0
		//#  endif
		//# else
		//#  define SQLITE_OS_UNIX 0
		//#  define SQLITE_OS_OS2 0
		//# endif
		//#else
		//# ifndef SQLITE_OS_WIN
		//#  define SQLITE_OS_WIN 0
		//# endif
		//#endif
		const bool SQLITE_OS_WIN = true;

		const bool SQLITE_OS_UNIX = false;

		const bool SQLITE_OS_OS2 = false;

		///
///<summary>
///</summary>
///<param name="Determine if we are dealing with WindowsCE "> which has a much</param>
///<param name="reduced API.">reduced API.</param>
///<param name=""></param>

		//#if (_WIN32_WCE)
		//# define SQLITE_OS_WINCE 1
		//#else
		//# define SQLITE_OS_WINCE 0
		//#endif
		///
///<summary>
///Define the maximum size of a temporary filename
///
///</summary>

		#if SQLITE_OS_WIN
		//# include <windows.h>
		const int MAX_PATH = 260;

		const int SQLITE_TEMPNAME_SIZE = (MAX_PATH + 50);

		//# define SQLITE_TEMPNAME_SIZE (MAX_PATH+50)
		#elif SQLITE_OS_OS2
																																						#if FALSE
																																						//  include <os2safe.h> /* has to be included before os2.h for linking to work */
#endif
																																						// define INCL_DOSDATETIME
// define INCL_DOSFILEMGR
// define INCL_DOSERRORS
// define INCL_DOSMISC
// define INCL_DOSPROCESS
// define INCL_DOSMODULEMGR
// define INCL_DOSSEMAPHORES
// include <os2.h>
// include <uconv.h>
// define SQLITE_TEMPNAME_SIZE (CCHMAXPATHCOMP)
//else
// define SQLITE_TEMPNAME_SIZE 200
#endif
		///
///<summary>
///If the SET_FULLSYNC macro is not defined above, then make it
///</summary>
///<param name="a no">op</param>

		//#if !SET_FULLSYNC
		//# define SET_FULLSYNC(x,y)
		//#endif
		///
///<summary>
///The default size of a disk sector
///
///</summary>

		#if !SQLITE_DEFAULT_SECTOR_SIZE
		public const int SQLITE_DEFAULT_SECTOR_SIZE = 512;

		//# define SQLITE_DEFAULT_SECTOR_SIZE 512
		#endif
		///
///<summary>
///Temporary files are named starting with this prefix followed by 16 random
///alphanumeric characters, and no file extension. They are stored in the
///OS's standard temporary file directory, and are deleted prior to exit.
///If sqlite is being embedded in another program, you may wish to change the
///prefix to reflect your program's name, so that if your program exits
///prematurely, old temporary files can be easily identified. This can be done
///</summary>
///<param name="using ">DSQLITE_TEMP_FILE_PREFIX=myprefix_ on the compiler command line.</param>
///<param name=""></param>
///<param name="2006">31:  The default prefix used to be "sqlite_".  But then</param>
///<param name="Mcafee started using SQLite in their anti">virus product and it</param>
///<param name="started putting files with the "sqlite" name in the c:/temp folder.">started putting files with the "sqlite" name in the c:/temp folder.</param>
///<param name="This annoyed many windows users.  Those users would then do a">This annoyed many windows users.  Those users would then do a</param>
///<param name="Google search for "sqlite", find the telephone numbers of the">Google search for "sqlite", find the telephone numbers of the</param>
///<param name="developers and call to wake them up at night and complain.">developers and call to wake them up at night and complain.</param>
///<param name="For this reason, the default name prefix is changed to be "sqlite"">For this reason, the default name prefix is changed to be "sqlite"</param>
///<param name="spelled backwards.  So the temp files are still identified, but">spelled backwards.  So the temp files are still identified, but</param>
///<param name="anybody smart enough to figure out the code is also likely smart">anybody smart enough to figure out the code is also likely smart</param>
///<param name="enough to know that calling the developer will not help get rid">enough to know that calling the developer will not help get rid</param>
///<param name="of the file.">of the file.</param>

		#if !SQLITE_TEMP_FILE_PREFIX
		const string SQLITE_TEMP_FILE_PREFIX = "etilqs_";

		//# define SQLITE_TEMP_FILE_PREFIX "etilqs_"
		#endif
		

		///
///<summary>
///File Locking Notes:  (Mostly about windows but also some info for Unix)
///
///We cannot use LockFileEx() or UnlockFileEx() on Win95/98/ME because
///those functions are not available.  So we use only LockFile() and
///UnlockFile().
///
///LockFile() prevents not just writing but also reading by other processes.
///</summary>
///<param name="A SHARED_LOCK is obtained by locking a single randomly">chosen</param>
///<param name="byte out of a specific range of bytes. The lock byte is obtained at">byte out of a specific range of bytes. The lock byte is obtained at</param>
///<param name="random so two separate readers can probably access the file at the">random so two separate readers can probably access the file at the</param>
///<param name="same time, unless they are unlucky and choose the same lock byte.">same time, unless they are unlucky and choose the same lock byte.</param>
///<param name="An EXCLUSIVE_LOCK is obtained by locking all bytes in the range.">An EXCLUSIVE_LOCK is obtained by locking all bytes in the range.</param>
///<param name="There can only be one writer.  A RESERVED_LOCK is obtained by locking">There can only be one writer.  A RESERVED_LOCK is obtained by locking</param>
///<param name="a single byte of the file that is designated as the reserved lock byte.">a single byte of the file that is designated as the reserved lock byte.</param>
///<param name="A PENDING_LOCK is obtained by locking a designated byte different from">A PENDING_LOCK is obtained by locking a designated byte different from</param>
///<param name="the RESERVED_LOCK byte.">the RESERVED_LOCK byte.</param>
///<param name=""></param>
///<param name="On WinNT/2K/XP systems, LockFileEx() and UnlockFileEx() are available,">On WinNT/2K/XP systems, LockFileEx() and UnlockFileEx() are available,</param>
///<param name="which means we can use reader/writer locks.  When reader/writer locks">which means we can use reader/writer locks.  When reader/writer locks</param>
///<param name="are used, the lock is placed on the same range of bytes that is used">are used, the lock is placed on the same range of bytes that is used</param>
///<param name="for probabilistic locking in Win95/98/ME.  Hence, the locking scheme">for probabilistic locking in Win95/98/ME.  Hence, the locking scheme</param>
///<param name="will support two or more Win95 readers or two or more WinNT readers.">will support two or more Win95 readers or two or more WinNT readers.</param>
///<param name="But a single Win95 reader will lock out all WinNT readers and a single">But a single Win95 reader will lock out all WinNT readers and a single</param>
///<param name="WinNT reader will lock out all other Win95 readers.">WinNT reader will lock out all other Win95 readers.</param>
///<param name=""></param>
///<param name="The following #defines specify the range of bytes used for locking.">The following #defines specify the range of bytes used for locking.</param>
///<param name="SHARED_SIZE is the number of bytes available in the pool from which">SHARED_SIZE is the number of bytes available in the pool from which</param>
///<param name="a random byte is selected for a shared lock.  The pool of bytes for">a random byte is selected for a shared lock.  The pool of bytes for</param>
///<param name="shared locks begins at SHARED_FIRST.">shared locks begins at SHARED_FIRST.</param>
///<param name=""></param>
///<param name="The same locking strategy and">The same locking strategy and</param>
///<param name="byte ranges are used for Unix.  This leaves open the possiblity of having">byte ranges are used for Unix.  This leaves open the possiblity of having</param>
///<param name="clients on win95, winNT, and unix all talking to the same shared file">clients on win95, winNT, and unix all talking to the same shared file</param>
///<param name="and all locking correctly.  To do so would require that samba (or whatever">and all locking correctly.  To do so would require that samba (or whatever</param>
///<param name="tool is being used for file sharing) implements locks correctly between">tool is being used for file sharing) implements locks correctly between</param>
///<param name="windows and unix.  I'm guessing that isn't likely to happen, but by">windows and unix.  I'm guessing that isn't likely to happen, but by</param>
///<param name="using the same locking range we are at least open to the possibility.">using the same locking range we are at least open to the possibility.</param>
///<param name=""></param>
///<param name="Locking in windows is manditory.  For this reason, we cannot store">Locking in windows is manditory.  For this reason, we cannot store</param>
///<param name="actual data in the bytes used for locking.  The pager never allocates">actual data in the bytes used for locking.  The pager never allocates</param>
///<param name="the pages involved in locking therefore.  SHARED_SIZE is selected so">the pages involved in locking therefore.  SHARED_SIZE is selected so</param>
///<param name="that all locks will fit on a single page even at the minimum page size.">that all locks will fit on a single page even at the minimum page size.</param>
///<param name="PENDING_BYTE defines the beginning of the locks.  By default PENDING_BYTE">PENDING_BYTE defines the beginning of the locks.  By default PENDING_BYTE</param>
///<param name="is set high so that we don't have to allocate an unused page except">is set high so that we don't have to allocate an unused page except</param>
///<param name="for very large databases.  But one should test the page skipping logic">for very large databases.  But one should test the page skipping logic</param>
///<param name="by setting PENDING_BYTE low and running the entire regression suite.">by setting PENDING_BYTE low and running the entire regression suite.</param>
///<param name=""></param>
///<param name="Changing the value of PENDING_BYTE results in a subtly incompatible">Changing the value of PENDING_BYTE results in a subtly incompatible</param>
///<param name="file format.  Depending on how it is changed, you might not notice">file format.  Depending on how it is changed, you might not notice</param>
///<param name="the incompatibility right away, even running a full regression test.">the incompatibility right away, even running a full regression test.</param>
///<param name="The default location of PENDING_BYTE is the first byte past the">The default location of PENDING_BYTE is the first byte past the</param>
///<param name="1GB boundary.">1GB boundary.</param>
///<param name=""></param>
///<param name=""></param>

		#if SQLITE_OMIT_WSD
																																						// define PENDING_BYTE     (0x40000000)
    static int PENDING_BYTE = 0x40000000; 
#else
		//# define PENDING_BYTE      sqlite3PendingByte
		public static int PENDING_BYTE = 0x40000000;

		#endif
		static int RESERVED_BYTE = (PENDING_BYTE + 1);

		public static int SHARED_FIRST = (PENDING_BYTE + 2);

		public static int SHARED_SIZE = 510;

		///
///<summary>
///Wrapper around OS specific sqlite3_os_init() function.
///
///</summary>

		//int sqlite3OsInit(void);
		///
///<summary>
///Functions for accessing sqlite3_file methods
///
///</summary>

		//int sqlite3OsClose(sqlite3_file);
		//int sqlite3OsRead(sqlite3_file*, void*, int amt, i64 offset);
		//int sqlite3OsWrite(sqlite3_file*, const void*, int amt, i64 offset);
		//int sqlite3OsTruncate(sqlite3_file*, i64 size);
		//int sqlite3OsSync(sqlite3_file*, int);
		//int sqlite3OsFileSize(sqlite3_file*, i64 pSize);
		//int sqlite3OsLock(sqlite3_file*, int);
		//int sqlite3OsUnlock(sqlite3_file*, int);
		//int sqlite3OsCheckReservedLock(sqlite3_file *id, int pResOut);
		//int sqlite3OsFileControl(sqlite3_file*,int,void);
		//#define SQLITE_FCNTL_DB_UNCHANGED 0xca093fa0
		public const u32 SQLITE_FCNTL_DB_UNCHANGED = 0xca093fa0;
	//int sqlite3OsSectorSize(sqlite3_file *id);
	//int sqlite3OsDeviceCharacteristics(sqlite3_file *id);
	//int sqlite3OsShmMap(sqlite3_file *,int,int,int,object volatile *);
	//int sqlite3OsShmLock(sqlite3_file *id, int, int, int);
	//void sqlite3OsShmBarrier(sqlite3_file *id);
	//int sqlite3OsShmUnmap(sqlite3_file *id, int);
	///
///<summary>
///Functions for accessing sqlite3_vfs methods
///
///</summary>

	//int sqlite3OsOpen(sqlite3_vfs *, string , sqlite3_file*, int, int );
	//int sqlite3OsDelete(sqlite3_vfs *, string , int);
	//int sqlite3OsAccess(sqlite3_vfs *, string , int, int pResOut);
	//int sqlite3OsFullPathname(sqlite3_vfs *, string , int, char );
	#if !SQLITE_OMIT_LOAD_EXTENSION
	//void *sqlite3OsDlOpen(sqlite3_vfs *, string );
	//void sqlite3OsDlError(sqlite3_vfs *, int, char );
	//void (*sqlite3OsDlSym(sqlite3_vfs *, object  *, string ))(void);
	//void sqlite3OsDlClose(sqlite3_vfs *, object  );
	#endif
	//int sqlite3OsRandomness(sqlite3_vfs *, int, char );
	//int sqlite3OsSleep(sqlite3_vfs *, int);
	//int sqlite3OsCurrentTimeInt64(sqlite3_vfs *, sqlite3_int64);
	///
///<summary>
///Convenience functions for opening and closing files using
///</summary>
///<param name="malloc_cs.sqlite3Malloc() to obtain space for the file">handle structure.</param>
///<param name=""></param>

	//int sqlite3OsOpenMalloc(sqlite3_vfs *, string , sqlite3_file **, int,int);
	//int sqlite3OsCloseFree(sqlite3_file );
	#endif
	}
}
