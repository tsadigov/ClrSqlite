using Pgno = System.UInt32;

namespace Community.CsharpSqlite
{
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
///This header file defines the interface that the sqlite page cache
///subsystem.  The page cache subsystem reads and writes a file a page
///at a time and provides a journal for rollback.
///
///</summary>
///<param name="Included in SQLite3 port to C#">SQLite;  2008 Noah B Hart</param>
///<param name="C#">SQLite is an independent reimplementation of the SQLite software library</param>
///<param name=""></param>
///<param name="SQLITE_SOURCE_ID: 2011">19 13:26:54 ed1da510a239ea767a01dc332b667119fa3c908e</param>
///<param name=""></param>
///<param name=""></param>
///<param name=""></param>

		//#if !_PAGER_H_
		//#define _PAGER_H_
		///

		///
///<summary>
///The type used to represent a page number.  The first page in a file
///is called page 1.  0 is used to represent "not a page".
///</summary>

		//typedef u32 Pgno;
		///
///<summary>
///Each open file is managed by a separate instance of the "Pager" structure.
///
///</summary>

		//typedef struct Pager Pager;
		///


		///

	///
///<summary>
///The remainder of this file contains the declarations of the functions
///</summary>
///<param name="that make up the Pager sub">system API. See source code comments for</param>
///<param name="a detailed description of each routine.">a detailed description of each routine.</param>
///<param name=""></param>

	///
///<summary>
///Open and close a Pager connection. 
///</summary>

	//int sqlite3PagerOpen(
	//  sqlite3_vfs*,
	//  Pager **ppPager,
	//  const char*,
	//  int,
	//  int,
	//  int,
	////  void()(DbPage)
	//);
	//int sqlite3PagerClose(Pager *pPager);
	//int sqlite3PagerReadFileheader(Pager*, int, unsigned char);
	///
///<summary>
///Functions used to configure a Pager object. 
///</summary>

	//void sqlite3PagerSetBusyhandler(Pager*, int()(void ), object  );
	//int sqlite3PagerSetPagesize(Pager*, u32*, int);
	//int sqlite3PagerMaxPageCount(Pager*, int);
	//void sqlite3PagerSetCachesize(Pager*, int);
	//void sqlite3PagerSetSafetyLevel(Pager*,int,int,int);
	//int sqlite3PagerLockingMode(Pager *, int);
	//int sqlite3PagerSetJournalMode(Pager *, int);
	//int sqlite3PagerGetJournalMode(Pager);
	//int sqlite3PagerOkToChangeJournalMode(Pager);
	//i64 sqlite3PagerJournalSizeLimit(Pager *, i64);
	//sqlite3_backup **sqlite3PagerBackupPtr(Pager);
	///
///<summary>
///Functions used to obtain and release page references. 
///</summary>

	//int sqlite3PagerAcquire(Pager *pPager, Pgno pgno, DbPage **ppPage, int clrFlag);
	//#define sqlite3PagerGet(A,B,C) sqlite3PagerAcquire(A,B,C,0)
	//DbPage *sqlite3PagerLookup(Pager *pPager, Pgno pgno);
	//void sqlite3PagerRef(DbPage);
	//void PagerMethods.sqlite3PagerUnref(DbPage);
	///
///<summary>
///Operations on page references. 
///</summary>

	//int PagerMethods.sqlite3PagerWrite(DbPage);
	//void sqlite3PagerDontWrite(DbPage);
	//int sqlite3PagerMovepage(Pager*,DbPage*,Pgno,int);
	//int PagerMethods.sqlite3PagerPageRefcount(DbPage);
	//void *sqlite3PagerGetData(DbPage );
	//void * PagerMethods.sqlite3PagerGetExtra (DbPage );
	///
///<summary>
///Functions used to manage pager transactions and savepoints. 
///</summary>

	//void sqlite3PagerPagecount(Pager*, int);
	//int sqlite3PagerBegin(Pager*, int exFlag, int);
	//int sqlite3PagerCommitPhaseOne(Pager*,string zMaster, int);
	//int sqlite3PagerExclusiveLock(Pager);
	//int sqlite3PagerSync(Pager *pPager);
	//int sqlite3PagerCommitPhaseTwo(Pager);
	//int sqlite3PagerRollback(Pager);
	//int sqlite3PagerOpenSavepoint(Pager *pPager, int n);
	//int sqlite3PagerSavepoint(Pager *pPager, int op, int iSavepoint);
	//int sqlite3PagerSharedLock(Pager *pPager);
	//int sqlite3PagerCheckpoint(Pager *pPager, int, int*, int);
	//int sqlite3PagerWalSupported(Pager* pPager);
	//int sqlite3PagerWalCallback(Pager* pPager);
	//int sqlite3PagerOpenWal(Pager* pPager, int* pisOpen);
	//int sqlite3PagerCloseWal(Pager* pPager);
	///
///<summary>
///Functions used to query pager state and configuration. 
///</summary>

	//u8 sqlite3PagerIsreadonly(Pager);
	//int sqlite3PagerRefcount(Pager);
	//int sqlite3PagerMemUsed(Pager);
	//string sqlite3PagerFilename(Pager);
	//const sqlite3_vfs *sqlite3PagerVfs(Pager);
	//sqlite3_file *sqlite3PagerFile(Pager);
	//string sqlite3PagerJournalname(Pager);
	//int sqlite3PagerNosync(Pager);
	//void *sqlite3PagerTempSpace(Pager);
	//int sqlite3PagerIsMemdb(Pager);
	///
///<summary>
///Functions used to truncate the database file. 
///</summary>

	//void sqlite3PagerTruncateImage(Pager*,Pgno);
	//#if (SQLITE_HAS_CODEC) && !defined(SQLITE_OMIT_WAL)
	//void *sqlite3PagerCodec(DbPage );
	//#endif
	///
///<summary>
///Functions to support testing and debugging. 
///</summary>

	//#if !NDEBUG || SQLITE_TEST
	//  Pgno sqlite3PagerPagenumber(DbPage);
	//  int sqlite3PagerIswriteable(DbPage);
	//#endif
	//#if SQLITE_TEST
	//  int *sqlite3PagerStats(Pager);
	//  void sqlite3PagerRefdump(Pager);
	//  void disable_simulated_io_errors(void);
	//  void enable_simulated_io_errors(void);
	//#else
	//# define disable_simulated_io_errors()
	//# define enable_simulated_io_errors()
	//#endif
	}
}
