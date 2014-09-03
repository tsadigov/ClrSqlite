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
///</summary>
///<param name="This header file defines the interface that the sqlite B">Tree file</param>
///<param name="subsystem.  See comments in the source code for a detailed description">subsystem.  See comments in the source code for a detailed description</param>
///<param name="of what each interface routine does.">of what each interface routine does.</param>
///<param name=""></param>
///<param name="Included in SQLite3 port to C#">SQLite;  2008 Noah B Hart</param>
///<param name="C#">SQLite is an independent reimplementation of the SQLite software library</param>
///<param name=""></param>
///<param name="SQLITE_SOURCE_ID: 2011">23 19:49:22 4374b7e83ea0a3fbc3691f9c0c936272862f32f2</param>
///<param name=""></param>
///<param name=""></param>
///<param name=""></param>

		//#if !_BTREE_H_
		//#define _BTREE_H_
		///
///<summary>
///TODO: This definition is just included so other modules compile. It
///needs to be revisited.
///
///</summary>

		const int SQLITE_N_BTREE_META = 10;

		///
///<summary>
///</summary>
///<param name="If defined as non">vacuum is enabled by default. Otherwise</param>
///<param name="it must be turned on for each database using "PRAGMA auto_vacuum = 1".">it must be turned on for each database using "PRAGMA auto_vacuum = 1".</param>
///<param name=""></param>

		#if !SQLITE_DEFAULT_AUTOVACUUM
		const int SQLITE_DEFAULT_AUTOVACUUM = 0;

		#endif
		const int BTREE_AUTOVACUUM_NONE = 0;

		///
///<summary>
///</summary>
///<param name="Do not do auto">vacuum </param>

		const int BTREE_AUTOVACUUM_FULL = 1;

		///
///<summary>
///</summary>
///<param name="Do full auto">vacuum </param>

		const int BTREE_AUTOVACUUM_INCR = 2;

		///
///<summary>
///Incremental vacuum 
///</summary>

		///
///<summary>
///Forward declarations of structure
///
///</summary>

		//typedef struct Btree Btree;
		//typedef struct BtCursor BtCursor;
		//typedef struct BtShared BtShared;
		//int sqlite3BtreeOpen(
		//  sqlite3_vfs *pVfs,       /* VFS to use with this b-tree */
		//  string zFilename,       /* Name of database file to open */
		//  sqlite3 db,             /* Associated database connection */
		//  Btree **ppBtree,        /* Return open Btree* here */
		//  int flags,              /* Flags */
		//  int vfsFlags            /* Flags passed through to VFS open */
		//);
		///
///<summary>
///The flags parameter to sqlite3BtreeOpen can be the bitwise or of the
///following values.
///
///NOTE:  These values must match the corresponding PAGER_ values in
///pager.h.
///
///</summary>

		//#define BTREE_OMIT_JOURNAL  1  /* Do not create or use a rollback journal */
		//#define BTREE_NO_READLOCK   2  /* Omit readlocks on readonly files */
		//#define BTREE_MEMORY        4  /* This is an in-memory DB */
		//#define BTREE_SINGLE        8  /* The file contains at most 1 b-tree */
		//#define BTREE_UNORDERED    16  /* Use of a hash implementation is OK */
		const int BTREE_OMIT_JOURNAL = 1;

		///
///<summary>
///Do not create or use a rollback journal 
///</summary>

		const int BTREE_NO_READLOCK = 2;

		///
///<summary>
///Omit readlocks on readonly files 
///</summary>

		const int BTREE_MEMORY = 4;

		///
///<summary>
///</summary>
///<param name="This is an in">memory DB </param>

		const int BTREE_SINGLE = 8;

		///
///<summary>
///</summary>
///<param name="The file contains at most 1 b">tree </param>

		const int BTREE_UNORDERED = 16;

		///
///<summary>
///Use of a hash implementation is OK 
///</summary>

		//int sqlite3BtreeClose(Btree);
		//int sqlite3BtreeSetCacheSize(Btree*,int);
		//int sqlite3BtreeSetSafetyLevel(Btree*,int,int,int);
		//int sqlite3BtreeSyncDisabled(Btree);
		//int sqlite3BtreeSetPageSize(Btree *p, int nPagesize, int nReserve, int eFix);
		//int sqlite3BtreeGetPageSize(Btree);
		//int sqlite3BtreeMaxPageCount(Btree*,int);
		//u32 sqlite3BtreeLastPage(Btree);
		//int sqlite3BtreeSecureDelete(Btree*,int);
		//int sqlite3BtreeGetReserve(Btree);
		//int sqlite3BtreeSetAutoVacuum(Btree , int);
		//int sqlite3BtreeGetAutoVacuum(Btree );
		//int sqlite3BtreeBeginTrans(Btree*,int);
		//int sqlite3BtreeCommitPhaseOne(Btree*, string zMaster);
		//int sqlite3BtreeCommitPhaseTwo(Btree*, int);
		//int sqlite3BtreeCommit(Btree);
		//int sqlite3BtreeRollback(Btree);
		//int sqlite3BtreeBeginStmt(Btree);
		//int sqlite3BtreeCreateTable(Btree*, int*, int flags);
		//int sqlite3BtreeIsInTrans(Btree);
		//int sqlite3BtreeIsInReadTrans(Btree);
		//int sqlite3BtreeIsInBackup(Btree);
		//void *sqlite3BtreeSchema(Btree , int, void()(void ));
		//int sqlite3BtreeSchemaLocked( Btree* pBtree );
		//int sqlite3BtreeLockTable( Btree* pBtree, int iTab, u8 isWriteLock );
		//int sqlite3BtreeSavepoint(Btree *, int, int);
		//string sqlite3BtreeGetFilename(Btree );
		//string sqlite3BtreeGetJournalname(Btree );
		//int sqlite3BtreeCopyFile(Btree *, Btree );
		//int sqlite3BtreeIncrVacuum(Btree );
		///
///<summary>
///The flags parameter to sqlite3BtreeCreateTable can be the bitwise OR
///of the flags shown below.
///
///Every SQLite table must have either BTREE_INTKEY or BTREE_BLOBKEY set.
///</summary>
///<param name="With BTREE_INTKEY, the table key is a 64">bit integer and arbitrary data</param>
///<param name="is stored in the leaves.  (BTREE_INTKEY is used for SQL tables.)  With">is stored in the leaves.  (BTREE_INTKEY is used for SQL tables.)  With</param>
///<param name="BTREE_BLOBKEY, the key is an arbitrary BLOB and no content is stored">BTREE_BLOBKEY, the key is an arbitrary BLOB and no content is stored</param>
///<param name="anywhere "> the key is the content.  (BTREE_BLOBKEY is used for SQL</param>
///<param name="indices.)">indices.)</param>
///<param name=""></param>

		//#define BTREE_INTKEY     1    /* Table has only 64-bit signed integer keys */
		//#define BTREE_BLOBKEY    2    /* Table has keys only - no data */
		const int BTREE_INTKEY = 1;

		const int BTREE_BLOBKEY = 2;

		//int sqlite3BtreeDropTable(Btree*, int, int);
		//int sqlite3BtreeClearTable(Btree*, int, int);
		//void sqlite3BtreeTripAllCursors(Btree*, int);
		//void sqlite3BtreeGetMeta(Btree *pBtree, int idx, u32 *pValue);
		//int sqlite3BtreeUpdateMeta(Btree*, int idx, u32 value);
		//int sqlite3BtreeCursor(
		//  Btree*,                              /* BTree containing table to open */
		//  int iTable,                          /* Index of root page */
		//  int wrFlag,                          /* 1 for writing.  0 for read-only */
		//  struct KeyInfo*,                     /* First argument to compare function */
		//  BtCursor pCursor                    /* Space to write cursor structure */
		//);
		//int sqlite3BtreeCursorSize(void);
		//void sqlite3BtreeCursorZero(BtCursor);
		//int sqlite3BtreeCloseCursor(BtCursor);
		//int sqlite3BtreeMovetoUnpacked(
		//  BtCursor*,
		//  UnpackedRecord pUnKey,
		//  i64 intKey,
		//  int bias,
		//  int pRes
		//);
		//int sqlite3BtreeCursorHasMoved(BtCursor*, int);
		//int sqlite3BtreeDelete(BtCursor);
		//int sqlite3BtreeInsert(BtCursor*, const void pKey, i64 nKey,
		//                                  const void pData, int nData,
		//                                  int nZero, int bias, int seekResult);
		//int sqlite3BtreeFirst(BtCursor*, int pRes);
		//int sqlite3BtreeLast(BtCursor*, int pRes);
		//int sqlite3BtreeNext(BtCursor*, int pRes);
		//int sqlite3BtreeEof(BtCursor);
		//int sqlite3BtreePrevious(BtCursor*, int pRes);
		//int sqlite3BtreeKeySize(BtCursor*, i64 pSize);
		//int sqlite3BtreeKey(BtCursor*, u32 offset, u32 amt, void);
		//const void *sqlite3BtreeKeyFetch(BtCursor*, int pAmt);
		//const void *sqlite3BtreeDataFetch(BtCursor*, int pAmt);
		//int sqlite3BtreeDataSize(BtCursor*, u32 pSize);
		//int sqlite3BtreeData(BtCursor*, u32 offset, u32 amt, void);
		//void sqlite3BtreeSetCachedRowid(BtCursor*, sqlite3_int64);
		//sqlite3_int64 sqlite3BtreeGetCachedRowid(BtCursor);
		//char *sqlite3BtreeIntegrityCheck(Btree*, int *aRoot, int nRoot, int, int);
		//struct Pager *sqlite3BtreePager(Btree);
		//int sqlite3BtreePutData(BtCursor*, u32 offset, u32 amt, void);
		//void sqlite3BtreeCacheOverflow(BtCursor );
		//void sqlite3BtreeClearCursor(BtCursor );
		//int sqlite3BtreeSetVersion(Btree *pBt, int iVersion);
		//#if !NDEBUG
		//int sqlite3BtreeCursorIsValid(BtCursor);
		//#endif
		//#if !SQLITE_OMIT_BTREECOUNT
		//int sqlite3BtreeCount(BtCursor *, i64 );
		//#endif
		//#if SQLITE_TEST
		//int sqlite3BtreeCursorInfo(BtCursor*, int*, int);
		//void sqlite3BtreeCursorList(Btree);
		//#endif
		#if !SQLITE_OMIT_WAL
																																						//int sqlite3BtreeCheckpoint(Btree*, int, int *, int );
#endif
		///<summary>
		/// If we are not using shared cache, then there is no need to
		/// use mutexes to access the BtShared structures.  So make the
		/// Enter and Leave procedures no-ops.
		///</summary>
		#if !SQLITE_OMIT_SHARED_CACHE
																																						//void sqlite3BtreeEnter(Btree);
//void sqlite3BtreeEnterAll(sqlite3);
#else
		//# define sqlite3BtreeEnter(X)
		static void sqlite3BtreeEnter (Btree bt)
		{
		}

		//# define sqlite3BtreeEnterAll(X)
		static void sqlite3BtreeEnterAll (sqlite3 p)
		{
		}

		#endif
		#if !(SQLITE_OMIT_SHARED_CACHE) && SQLITE_THREADSAFE
																																						//int sqlite3BtreeSharable(Btree);
//void sqlite3BtreeLeave(Btree);
//void sqlite3BtreeEnterCursor(BtCursor);
//void sqlite3BtreeLeaveCursor(BtCursor);
//void sqlite3BtreeLeaveAll(sqlite3);
#if !NDEBUG
																																						/* These routines are used inside Debug.Assert() statements only. */
int sqlite3BtreeHoldsMutex(Btree);
int sqlite3BtreeHoldsAllMutexes(sqlite3);
int sqlite3SchemaMutexHeld(sqlite3*,int,Schema);
#endif
																																						#else
		//# define sqlite3BtreeSharable(X) 0
		static bool sqlite3BtreeSharable (Btree X)
		{
			return false;
		}

		//# define sqlite3BtreeLeave(X)
		static void sqlite3BtreeLeave (Btree X)
		{
		}

		//# define sqlite3BtreeEnterCursor(X)
		static void sqlite3BtreeEnterCursor (BtCursor X)
		{
		}

		//# define sqlite3BtreeLeaveCursor(X)
		static void sqlite3BtreeLeaveCursor (BtCursor X)
		{
		}

		//# define sqlite3BtreeLeaveAll(X)
		static void sqlite3BtreeLeaveAll (sqlite3 X)
		{
		}

		//# define sqlite3BtreeHoldsMutex(X) 1
		static bool sqlite3BtreeHoldsMutex (Btree X)
		{
			return true;
		}

		//# define sqlite3BtreeHoldsAllMutexes(X) 1
		static bool sqlite3BtreeHoldsAllMutexes (sqlite3 X)
		{
			return true;
		}

		//# define sqlite3SchemaMutexHeld(X,Y,Z) 1
		static bool sqlite3SchemaMutexHeld (sqlite3 X, int y, Schema z)
		{
			return true;
		}
	#endif
	//#endif // * _BTREE_H_ */
	}
}
