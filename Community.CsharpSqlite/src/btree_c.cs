using System;
using System.Diagnostics;
using System.Text;
using i64=System.Int64;
using u8=System.Byte;
using u16=System.UInt16;
using u32=System.UInt32;
using u64=System.UInt64;
using sqlite3_int64=System.Int64;
using Pgno=System.UInt32;
namespace Community.CsharpSqlite {
	using DbPage=Sqlite3.PgHdr;
	public partial class Sqlite3 {
		///
		///<summary>
		///2004 April 6
		///
		///The author disclaims copyright to this source code.  In place of
		///a legal notice, here is a blessing:
		///
		///May you do good and not evil.
		///May you find forgiveness for yourself and forgive others.
		///May you share freely, never taking more than you give.
		///
		///</summary>
		///<param name="This file implements a external (disk">based) database using BTrees.</param>
		///<param name="See the header comment on "btreeInt.h" for additional information.">See the header comment on "btreeInt.h" for additional information.</param>
		///<param name="Including a description of file format and an overview of operation.">Including a description of file format and an overview of operation.</param>
		///<param name=""></param>
		///<param name="Included in SQLite3 port to C#">SQLite;  2008 Noah B Hart</param>
		///<param name="C#">SQLite is an independent reimplementation of the SQLite software library</param>
		///<param name=""></param>
		///<param name="SQLITE_SOURCE_ID: 2011">23 19:49:22 4374b7e83ea0a3fbc3691f9c0c936272862f32f2 </param>
		///<param name=""></param>
		///<param name=""></param>
		//#include "btreeInt.h"
		///
		///<summary>
		///The header string that appears at the beginning of every
		///SQLite database.
		///</summary>
		static byte[] zMagicHeader=Encoding.UTF8.GetBytes(SQLITE_FILE_HEADER);
		///<summary>
		/// Set this global variable to 1 to enable tracing using the TRACE
		/// macro.
		///</summary>
		#if TRACE
																																																static bool sqlite3BtreeTrace=false;  /* True to enable tracing */
// define TRACE(X)  if(sqlite3BtreeTrace){printf X;fflush(stdout);}
static void TRACE(string X, params object[] ap) { if (sqlite3BtreeTrace)  printf(X, ap); }
#else
		//# define TRACE(X)
		static void TRACE(string X,params object[] ap) {
		}
		#endif
		///<summary>
		/// Extract a 2-byte big-endian integer from an array of unsigned bytes.
		/// But if the value is zero, make it 65536.
		///
		/// This routine is used to extract the "offset to cell content area" value
		/// from the header of a btree page.  If the page size is 65536 and the page
		/// is empty, the offset should be 65536, but the 2-byte value stores zero.
		/// This routine makes the necessary adjustment to 65536.
		///</summary>
		//#define get2byteNotZero(X)  (((((int)get2byte(X))-1)&0xffff)+1)
		static int get2byteNotZero(byte[] X,int offset) {
			return (((((int)get2byte(X,offset))-1)&0xffff)+1);
		}
		#if !SQLITE_OMIT_SHARED_CACHE
																																																///<summary>
/// A list of BtShared objects that are eligible for participation
/// in shared cache.  This variable has file scope during normal builds,
/// but the test harness needs to access it so we make it global for
/// test builds.
///
/// Access to this variable is protected by SQLITE_MUTEX_STATIC_MASTER.
///</summary>
#if SQLITE_TEST
																																																BtShared *SQLITE_WSD sqlite3SharedCacheList = 0;
#else
																																																static BtShared *SQLITE_WSD sqlite3SharedCacheList = 0;
#endif
																																																#endif
		#if !SQLITE_OMIT_SHARED_CACHE
																																																///<summary>
/// Enable or disable the shared pager and schema features.
///
/// This routine has no effect on existing database connections.
/// The shared cache setting effects only future calls to
/// sqlite3_open(), sqlite3_open16(), or sqlite3_open_v2().
///</summary>
int sqlite3_enable_shared_cache(int enable){
sqlite3GlobalConfig.sharedCacheEnabled = enable;
return SQLITE_OK;
}
#endif
		#if SQLITE_OMIT_SHARED_CACHE
		///
		///<summary>
		///The functions querySharedCacheTableLock(), setSharedCacheTableLock(),
		///and clearAllSharedCacheTableLocks()
		///manipulate entries in the BtShared.pLock linked list used to store
		///</summary>
		///<param name="shared">cache table level locks. If the library is compiled with the</param>
		///<param name="shared">cache feature disabled, then there is only ever one user</param>
		///<param name="of each BtShared structure and so this locking is not necessary.">of each BtShared structure and so this locking is not necessary.</param>
		///<param name="So define the lock related functions as no">ops.</param>
		//#define querySharedCacheTableLock(a,b,c) SQLITE_OK
		//#define setSharedCacheTableLock(a,b,c) SQLITE_OK
		//#define clearAllSharedCacheTableLocks(a)
		//#define downgradeAllSharedCacheTableLocks(a)
		//#define hasSharedCacheTableLock(a,b,c,d) 1
		//#define hasReadConflicts(a, b) 0
		#endif
		#if !SQLITE_OMIT_SHARED_CACHE
																																																
#if SQLITE_DEBUG
																																																/*
**** This function is only used as part of an assert() statement. ***
**
** Check to see if pBtree holds the required locks to read or write to the 
** table with root page iRoot.   Return 1 if it does and 0 if not.
**
** For example, when writing to a table with root-page iRoot via 
** Btree connection pBtree:
**
**    assert( hasSharedCacheTableLock(pBtree, iRoot, 0, WRITE_LOCK) );
**
** When writing to an index that resides in a sharable database, the 
** caller should have first obtained a lock specifying the root page of
** the corresponding table. This makes things a bit more complicated,
** as this module treats each table as a separate structure. To determine
** the table corresponding to the index being written, this
** function has to search through the database schema.
**
** Instead of a lock on the table/index rooted at page iRoot, the caller may
** hold a write-lock on the schema table (root page 1). This is also
** acceptable.
*/
static int hasSharedCacheTableLock(
Btree pBtree,         /* Handle that must hold lock */
Pgno iRoot,            /* Root page of b-tree */
int isIndex,           /* True if iRoot is the root of an index b-tree */
int eLockType          /* Required lock type (READ_LOCK or WRITE_LOCK) */
){
Schema pSchema = (Schema *)pBtree.pBt.pSchema;
Pgno iTab = 0;
BtLock pLock;

/* If this database is not shareable, or if the client is reading
** and has the read-uncommitted flag set, then no lock is required. 
** Return true immediately.
*/
if( (pBtree.sharable==null)
|| (eLockType==READ_LOCK && (pBtree.db.flags & SQLITE_ReadUncommitted))
){
return 1;
}

/* If the client is reading  or writing an index and the schema is
** not loaded, then it is too difficult to actually check to see if
** the correct locks are held.  So do not bother - just return true.
** This case does not come up very often anyhow.
*/
if( isIndex && (!pSchema || (pSchema->flags&DB_SchemaLoaded)==0) ){
return 1;
}

/* Figure out the root-page that the lock should be held on. For table
** b-trees, this is just the root page of the b-tree being read or
** written. For index b-trees, it is the root page of the associated
** table.  */
if( isIndex ){
HashElem p;
for(p=sqliteHashFirst(pSchema.idxHash); p!=null; p=sqliteHashNext(p)){
Index pIdx = (Index *)sqliteHashData(p);
if( pIdx.tnum==(int)iRoot ){
iTab = pIdx.pTable.tnum;
}
}
}else{
iTab = iRoot;
}

/* Search for the required lock. Either a write-lock on root-page iTab, a
** write-lock on the schema table, or (if the client is reading) a
** read-lock on iTab will suffice. Return 1 if any of these are found.  */
for(pLock=pBtree.pBt.pLock; pLock; pLock=pLock.pNext){
if( pLock.pBtree==pBtree
&& (pLock.iTable==iTab || (pLock.eLock==WRITE_LOCK && pLock.iTable==1))
&& pLock.eLock>=eLockType
){
return 1;
}
}

/* Failed to find the required lock. */
return 0;
}

#endif
																																																
#if SQLITE_DEBUG
																																																/*
** This function may be used as part of assert() statements only. ****
**
** Return true if it would be illegal for pBtree to write into the
** table or index rooted at iRoot because other shared connections are
** simultaneously reading that same table or index.
**
** It is illegal for pBtree to write if some other Btree object that
** shares the same BtShared object is currently reading or writing
** the iRoot table.  Except, if the other Btree object has the
** read-uncommitted flag set, then it is OK for the other object to
** have a read cursor.
**
** For example, before writing to any part of the table or index
** rooted at page iRoot, one should call:
**
**    assert( !hasReadConflicts(pBtree, iRoot) );
*/
static int hasReadConflicts(Btree pBtree, Pgno iRoot){
BtCursor p;
for(p=pBtree.pBt.pCursor; p!=null; p=p.pNext){
if( p.pgnoRoot==iRoot
&& p.pBtree!=pBtree
&& 0==(p.pBtree.db.flags & SQLITE_ReadUncommitted)
){
return 1;
}
}
return 0;
}
#endif
																																																
///<summary>
/// Query to see if Btree handle p may obtain a lock of type eLock
/// (READ_LOCK or WRITE_LOCK) on the table with root-page iTab. Return
/// SQLITE_OK if the lock may be obtained (by calling
/// setSharedCacheTableLock()), or SQLITE_LOCKED if not.
///</summary>
static int querySharedCacheTableLock(Btree p, Pgno iTab, u8 eLock){
BtShared pBt = p.pBt;
BtLock pIter;

Debug.Assert( sqlite3BtreeHoldsMutex(p) );
Debug.Assert( eLock==READ_LOCK || eLock==WRITE_LOCK );
Debug.Assert( p.db!=null );
Debug.Assert( !(p.db.flags&SQLITE_ReadUncommitted)||eLock==WRITE_LOCK||iTab==1 );

/* If requesting a write-lock, then the Btree must have an open write
** transaction on this file. And, obviously, for this to be so there
** must be an open write transaction on the file itself.
*/
Debug.Assert( eLock==READ_LOCK || (p==pBt.pWriter && p.inTrans==TRANS_WRITE) );
Debug.Assert( eLock==READ_LOCK || pBt.inTransaction==TRANS_WRITE );

/* This routine is a no-op if the shared-cache is not enabled */
if( !p.sharable ){
return SQLITE_OK;
}

/* If some other connection is holding an exclusive lock, the
** requested lock may not be obtained.
*/
if( pBt.pWriter!=p && pBt.isExclusive ){
sqlite3ConnectionBlocked(p.db, pBt.pWriter.db);
return SQLITE_LOCKED_SHAREDCACHE;
}

for(pIter=pBt.pLock; pIter; pIter=pIter.pNext){
/* The condition (pIter.eLock!=eLock) in the following if(...)
** statement is a simplification of:
**
**   (eLock==WRITE_LOCK || pIter.eLock==WRITE_LOCK)
**
** since we know that if eLock==WRITE_LOCK, then no other connection
** may hold a WRITE_LOCK on any table in this file (since there can
** only be a single writer).
*/
Debug.Assert( pIter.eLock==READ_LOCK || pIter.eLock==WRITE_LOCK );
Debug.Assert( eLock==READ_LOCK || pIter.pBtree==p || pIter.eLock==READ_LOCK);
if( pIter.pBtree!=p && pIter.iTable==iTab && pIter.eLock!=eLock ){
sqlite3ConnectionBlocked(p.db, pIter.pBtree.db);
if( eLock==WRITE_LOCK ){
Debug.Assert( p==pBt.pWriter );
pBt.isPending = 1;
}
return SQLITE_LOCKED_SHAREDCACHE;
}
}
return SQLITE_OK;
}
#endif
		#if !SQLITE_OMIT_SHARED_CACHE
																																																///<summary>
/// Add a lock on the table with root-page iTable to the shared-btree used
/// by Btree handle p. Parameter eLock must be either READ_LOCK or
/// WRITE_LOCK.
///
/// This function assumes the following:
///
///   (a) The specified Btree object p is connected to a sharable
///       database (one with the BtShared.sharable flag set), and
///
///   (b) No other Btree objects hold a lock that conflicts
///       with the requested lock (i.e. querySharedCacheTableLock() has
///       already been called and returned SQLITE_OK).
///
/// SQLITE_OK is returned if the lock is added successfully. SQLITE_NOMEM
/// is returned if a malloc attempt fails.
///</summary>
static int setSharedCacheTableLock(Btree p, Pgno iTable, u8 eLock){
BtShared pBt = p.pBt;
BtLock pLock = 0;
BtLock pIter;

Debug.Assert( sqlite3BtreeHoldsMutex(p) );
Debug.Assert( eLock==READ_LOCK || eLock==WRITE_LOCK );
Debug.Assert( p.db!=null );

/* A connection with the read-uncommitted flag set will never try to
** obtain a read-lock using this function. The only read-lock obtained
** by a connection in read-uncommitted mode is on the sqlite_master
** table, and that lock is obtained in BtreeBeginTrans().  */
Debug.Assert( 0==(p.db.flags&SQLITE_ReadUncommitted) || eLock==WRITE_LOCK );

/* This function should only be called on a sharable b-tree after it
** has been determined that no other b-tree holds a conflicting lock.  */
Debug.Assert( p.sharable );
Debug.Assert( SQLITE_OK==querySharedCacheTableLock(p, iTable, eLock) );

/* First search the list for an existing lock on this table. */
for(pIter=pBt.pLock; pIter; pIter=pIter.pNext){
if( pIter.iTable==iTable && pIter.pBtree==p ){
pLock = pIter;
break;
}
}

/* If the above search did not find a BtLock struct associating Btree p
** with table iTable, allocate one and link it into the list.
*/
if( !pLock ){
pLock = (BtLock *)sqlite3MallocZero(sizeof(BtLock));
if( !pLock ){
return SQLITE_NOMEM;
}
pLock.iTable = iTable;
pLock.pBtree = p;
pLock.pNext = pBt.pLock;
pBt.pLock = pLock;
}

/* Set the BtLock.eLock variable to the maximum of the current lock
** and the requested lock. This means if a write-lock was already held
** and a read-lock requested, we don't incorrectly downgrade the lock.
*/
Debug.Assert( WRITE_LOCK>READ_LOCK );
if( eLock>pLock.eLock ){
pLock.eLock = eLock;
}

return SQLITE_OK;
}
#endif
		#if !SQLITE_OMIT_SHARED_CACHE
																																																///<summary>
/// Release all the table locks (locks obtained via calls to
/// the setSharedCacheTableLock() procedure) held by Btree object p.
///
/// This function assumes that Btree p has an open read or write
/// transaction. If it does not, then the BtShared.isPending variable
/// may be incorrectly cleared.
///</summary>
static void clearAllSharedCacheTableLocks(Btree p){
BtShared pBt = p.pBt;
BtLock **ppIter = &pBt.pLock;

Debug.Assert( sqlite3BtreeHoldsMutex(p) );
Debug.Assert( p.sharable || 0==*ppIter );
Debug.Assert( p.inTrans>0 );

while( ppIter ){
BtLock pLock = ppIter;
Debug.Assert( pBt.isExclusive==null || pBt.pWriter==pLock.pBtree );
Debug.Assert( pLock.pBtree.inTrans>=pLock.eLock );
if( pLock.pBtree==p ){
ppIter = pLock.pNext;
Debug.Assert( pLock.iTable!=1 || pLock==&p.lock );
if( pLock.iTable!=1 ){
pLock=null;//sqlite3_free(ref pLock);
}
}else{
ppIter = &pLock.pNext;
}
}

Debug.Assert( pBt.isPending==null || pBt.pWriter );
if( pBt.pWriter==p ){
pBt.pWriter = 0;
pBt.isExclusive = 0;
pBt.isPending = 0;
}else if( pBt.nTransaction==2 ){
/* This function is called when Btree p is concluding its 
** transaction. If there currently exists a writer, and p is not
** that writer, then the number of locks held by connections other
** than the writer must be about to drop to zero. In this case
** set the isPending flag to 0.
**
** If there is not currently a writer, then BtShared.isPending must
** be zero already. So this next line is harmless in that case.
*/
pBt.isPending = 0;
}
}

/*
** This function changes all write-locks held by Btree p into read-locks.
*/
static void downgradeAllSharedCacheTableLocks(Btree p){
BtShared pBt = p.pBt;
if( pBt.pWriter==p ){
BtLock pLock;
pBt.pWriter = 0;
pBt.isExclusive = 0;
pBt.isPending = 0;
for(pLock=pBt.pLock; pLock; pLock=pLock.pNext){
Debug.Assert( pLock.eLock==READ_LOCK || pLock.pBtree==p );
pLock.eLock = READ_LOCK;
}
}
}

#endif
		//static void releasePage(MemPage pPage);  /* Forward reference */
		///<summary>
		/// This routine is used inside of assert() only 
		///
		/// Verify that the cursor holds the mutex on its BtShared
		///</summary>
		#if SQLITE_DEBUG
																																																static bool cursorHoldsMutex( BtCursor p )
{
  return sqlite3_mutex_held( p.pBt.mutex );
}
#else
		#endif
		#if !SQLITE_OMIT_INCRBLOB
																																																///<summary>
/// Invalidate the overflow page-list cache for cursor pCur, if any.
///</summary>
static void invalidateOverflowCache(BtCursor pCur){
Debug.Assert( cursorHoldsMutex(pCur) );
//sqlite3_free(ref pCur.aOverflow);
pCur.aOverflow = null;
}

///<summary>
/// Invalidate the overflow page-list cache for all cursors opened
/// on the shared btree structure pBt.
///</summary>
static void invalidateAllOverflowCache(BtShared pBt){
BtCursor p;
Debug.Assert( sqlite3_mutex_held(pBt.mutex) );
for(p=pBt.pCursor; p!=null; p=p.pNext){
invalidateOverflowCache(p);
}
}

///<summary>
/// This function is called before modifying the contents of a table
/// to invalidate any incrblob cursors that are open on the
/// row or one of the rows being modified.
///
/// If argument isClearTable is true, then the entire contents of the
/// table is about to be deleted. In this case invalidate all incrblob
/// cursors open on any row within the table with root-page pgnoRoot.
///
/// Otherwise, if argument isClearTable is false, then the row with
/// rowid iRow is being replaced or deleted. In this case invalidate
/// only those incrblob cursors open on that specific row.
///</summary>
static void invalidateIncrblobCursors(
Btree pBtree,          /* The database file to check */
i64 iRow,               /* The rowid that might be changing */
int isClearTable        /* True if all rows are being deleted */
){
BtCursor p;
BtShared pBt = pBtree.pBt;
Debug.Assert( sqlite3BtreeHoldsMutex(pBtree) );
for(p=pBt.pCursor; p!=null; p=p.pNext){
if( p.isIncrblobHandle && (isClearTable || p.info.nKey==iRow) ){
p.eState = CURSOR_INVALID;
}
}
}

#else
		///
		///<summary>
		///Stub functions when INCRBLOB is omitted 
		///</summary>
		//#define invalidateOverflowCache(x)
		static void invalidateOverflowCache(BtCursor pCur) {
		}
		//#define invalidateAllOverflowCache(x)
		//#define invalidateIncrblobCursors(x,y,z)
		#endif
		///<summary>
		/// Save the current cursor position in the variables BtCursor.nKey
		/// and BtCursor.pKey. The cursor's state is set to CURSOR_REQUIRESEEK.
		///
		/// The caller must ensure that the cursor is valid (has eState==CURSOR_VALID)
		/// prior to calling this routine.
		///</summary>
		///<summary>
		/// Save the positions of all cursors (except pExcept) that are open on
		/// the table  with root-page iRoot. Usually, this is called just before cursor
		/// pExcept is used to modify the table (BtreeDelete() or BtreeInsert()).
		///</summary>
		///<summary>
		/// Clear the current cursor position.
		///</summary>
		///<summary>
		/// In this version of BtreeMoveto, pKey is a packed index record
		/// such as is generated by the OP_MakeRecord opcode.  Unpack the
		/// record and then call BtreeMovetoUnpacked() to do the work.
		///</summary>
		///<summary>
		/// Restore the cursor to the position it was in (or as close to as possible)
		/// when saveCursorPosition() was called. Note that this call deletes the
		/// saved position info stored by saveCursorPosition(), so there can be
		/// at most one effective restoreCursorPosition() call after each
		/// saveCursorPosition().
		///</summary>
		//#define restoreCursorPosition(p) \
		//  (p.eState>=CURSOR_REQUIRESEEK ? \
		//         btreeRestoreCursorPosition(p) : \
		//         SQLITE_OK)
		///<summary>
		/// Determine whether or not a cursor has moved from the position it
		/// was last placed at.  Cursors can move when the row they are pointing
		/// at is deleted out from under them.
		///
		/// This routine returns an error code if something goes wrong.  The
		/// integer pHasMoved is set to one if the cursor has moved and 0 if not.
		///</summary>
		#if !SQLITE_OMIT_AUTOVACUUM
		///<summary>
		/// Given a page number of a regular database page, return the page
		/// number for the pointer-map page that contains the entry for the
		/// input page number.
		///
		/// Return 0 (not a valid page) for pgno==1 since there is
		/// no pointer map associated with page 1.  The integrity_check logic
		/// requires that ptrmapPageno(*,1)!=1.
		///</summary>
		///<summary>
		/// Write an entry into the pointer map.
		///
		/// This routine updates the pointer map entry for page number 'key'
		/// so that it maps to type 'eType' and parent page number 'pgno'.
		///
		/// If pRC is initially non-zero (non-SQLITE_OK) then this routine is
		/// a no-op.  If an error occurs, the appropriate error code is written
		/// into pRC.
		///</summary>
		///
		///<summary>
		///Read an entry from the pointer map.
		///
		///This routine retrieves the pointer map entry for page 'key', writing
		///the type and parent page number to pEType and pPgno respectively.
		///An error code is returned if something goes wrong, otherwise SQLITE_OK.
		///</summary>
		#else
																																																//define ptrmapPut(w,x,y,z,rc)
//define ptrmapGet(w,x,y,z) SQLITE_OK
//define ptrmapPutOvflPtr(x, y, rc)
#endif
		///<summary>
		/// Given a btree page and a cell index (0 means the first cell on
		/// the page, 1 means the second cell, and so forth) return a pointer
		/// to the cell content.
		///
		/// This routine works only for pages that do not contain overflow cells.
		///</summary>
		//#define findCell(P,I) \
		//  ((P).aData + ((P).maskPage & get2byte((P).aData[(P).cellOffset+2*(I)])))
		//#define findCellv2(D,M,O,I) (D+(M&get2byte(D+(O+2*(I)))))
		static u8[] findCellv2(u8[] pPage,u16 iCell,u16 O,int I) {
			Debugger.Break();
			return pPage;
		}
		//#define parseCell(pPage, iCell, pInfo) \
		#if SQLITE_DEBUG
																																																/* This variation on cellSizePtr() is used inside of assert() statements
** only. */
static u16 cellSize( MemPage pPage, int iCell )
{
  return cellSizePtr( pPage, findCell( pPage, iCell ) );
}
#else
		#endif
		#if !SQLITE_OMIT_AUTOVACUUM
		#endif
		///<summary>
		/// Convert a DbPage obtained from the pager into a MemPage used by
		/// the btree layer.
		///</summary>
		///<summary>
		/// Get a page from the pager.  Initialize the MemPage.pBt and
		/// MemPage.aData elements if needed.
		///
		/// If the noContent flag is set, it means that we do not care about
		/// the content of the page at this time.  So do not go to the disk
		/// to fetch the content.  Just fill in the content with zeros for now.
		/// If in the future we call sqlite3PagerWrite() on this page, that
		/// means we have started to be concerned about content and the disk
		/// read should occur at that point.
		///</summary>
		///<summary>
		/// Retrieve a page from the pager cache. If the requested page is not
		/// already in the pager cache return NULL. Initialize the MemPage.pBt and
		/// MemPage.aData elements if needed.
		///</summary>
		///<summary>
		/// Return the size of the database file in pages. If there is any kind of
		/// error, return ((unsigned int)-1).
		///</summary>
		static Pgno sqlite3BtreeLastPage(Btree p) {
			Debug.Assert(sqlite3BtreeHoldsMutex(p));
			Debug.Assert(((p.pBt.nPage)&0x8000000)==0);
			return (Pgno)p.pBt.btreePagecount();
		}
		///<summary>
		/// Get a page from the pager and initialize it.  This routine is just a
		/// convenience wrapper around separate calls to btreeGetPage() and
		/// btreeInitPage().
		///
		/// If an error occurs, then the value ppPage is set to is undefined. It
		/// may remain unchanged, or it may be set to an invalid value.
		///</summary>
		static int getAndInitPage(BtShared pBt,///
		///<summary>
		///The database file 
		///</summary>
		Pgno pgno,///
		///<summary>
		///Number of the page to get 
		///</summary>
		ref MemPage ppPage///
		///<summary>
		///Write the page pointer here 
		///</summary>
		) {
			int rc;
			Debug.Assert(sqlite3_mutex_held(pBt.mutex));
			if(pgno>pBt.btreePagecount()) {
				rc=SQLITE_CORRUPT_BKPT();
			}
			else {
				rc=pBt.btreeGetPage(pgno,ref ppPage,0);
				if(rc==SQLITE_OK) {
					rc=ppPage.btreeInitPage();
					if(rc!=SQLITE_OK) {
						releasePage(ppPage);
					}
				}
			}
			testcase(pgno==0);
			Debug.Assert(pgno!=0||rc==SQLITE_CORRUPT);
			return rc;
		}
		///<summary>
		/// Release a MemPage.  This should be called once for each prior
		/// call to btreeGetPage.
		///</summary>
		static void releasePage(MemPage pPage) {
			if(pPage!=null) {
				Debug.Assert(pPage.aData!=null);
				Debug.Assert(pPage.pBt!=null);
				//TODO -- find out why corrupt9 & diskfull fail on this tests 
				//Debug.Assert( sqlite3PagerGetExtra( pPage.pDbPage ) == pPage );
				//Debug.Assert( sqlite3PagerGetData( pPage.pDbPage ) == pPage.aData );
				Debug.Assert(sqlite3_mutex_held(pPage.pBt.mutex));
				sqlite3PagerUnref(pPage.pDbPage);
			}
		}
		///<summary>
		/// During a rollback, when the pager reloads information into the cache
		/// so that the cache is restored to its original state at the start of
		/// the transaction, for each page restored this routine is called.
		///
		/// This routine needs to reset the extra data section at the end of the
		/// page to agree with the restored data.
		///</summary>
		static void pageReinit(DbPage pData) {
			MemPage pPage;
			pPage=sqlite3PagerGetExtra(pData);
			Debug.Assert(sqlite3PagerPageRefcount(pData)>0);
			if(pPage.isInit!=0) {
				Debug.Assert(sqlite3_mutex_held(pPage.pBt.mutex));
				pPage.isInit=0;
				if(sqlite3PagerPageRefcount(pData)>1) {
					///
					///<summary>
					///pPage might not be a btree page;  it might be an overflow page
					///or ptrmap page or a free page.  In those cases, the following
					///call to btreeInitPage() will likely return SQLITE_CORRUPT.
					///But no harm is done by this.  And it is very important that
					///btreeInitPage() be called on every btree page so we make
					///</summary>
					///<param name="the call for every page that comes in for re">initing. </param>
					pPage.btreeInitPage();
				}
			}
		}
		///<summary>
		/// Invoke the busy handler for a btree.
		///</summary>
		static int btreeInvokeBusyHandler(object pArg) {
			BtShared pBt=(BtShared)pArg;
			Debug.Assert(pBt.db!=null);
			Debug.Assert(sqlite3_mutex_held(pBt.db.mutex));
			return sqlite3InvokeBusyHandler(pBt.db.busyHandler);
		}
		///
		///<summary>
		///Open a database file.
		///
		///zFilename is the name of the database file.  If zFilename is NULL
		///then an ephemeral database is created.  The ephemeral database might
		///</summary>
		///<param name="be exclusively in memory, or it might use a disk">based memory cache.</param>
		///<param name="Either way, the ephemeral database will be automatically deleted ">Either way, the ephemeral database will be automatically deleted </param>
		///<param name="when sqlite3BtreeClose() is called.">when sqlite3BtreeClose() is called.</param>
		///<param name=""></param>
		///<param name="If zFilename is ":memory:" then an in">memory database is created</param>
		///<param name="that is automatically destroyed when it is closed.">that is automatically destroyed when it is closed.</param>
		///<param name=""></param>
		///<param name="The "flags" parameter is a bitmask that might contain bits">The "flags" parameter is a bitmask that might contain bits</param>
		///<param name="BTREE_OMIT_JOURNAL and/or BTREE_NO_READLOCK.  The BTREE_NO_READLOCK">BTREE_OMIT_JOURNAL and/or BTREE_NO_READLOCK.  The BTREE_NO_READLOCK</param>
		///<param name="bit is also set if the SQLITE_NoReadlock flags is set in db">>flags.</param>
		///<param name="These flags are passed through into sqlite3PagerOpen() and must">These flags are passed through into sqlite3PagerOpen() and must</param>
		///<param name="be the same values as PAGER_OMIT_JOURNAL and PAGER_NO_READLOCK.">be the same values as PAGER_OMIT_JOURNAL and PAGER_NO_READLOCK.</param>
		///<param name=""></param>
		///<param name="If the database is already opened in the same database connection">If the database is already opened in the same database connection</param>
		///<param name="and we are in shared cache mode, then the open will fail with an">and we are in shared cache mode, then the open will fail with an</param>
		///<param name="SQLITE_CONSTRAINT error.  We cannot allow two or more BtShared">SQLITE_CONSTRAINT error.  We cannot allow two or more BtShared</param>
		///<param name="objects in the same database connection since doing so will lead">objects in the same database connection since doing so will lead</param>
		///<param name="to problems with locking.">to problems with locking.</param>
		static int sqlite3BtreeOpen(sqlite3_vfs pVfs,///
		///<summary>
		///</summary>
		///<param name="VFS to use for this b">tree </param>
		string zFilename,///
		///<summary>
		///Name of the file containing the BTree database 
		///</summary>
		sqlite3 db,///
		///<summary>
		///Associated database handle 
		///</summary>
		ref Btree ppBtree,///
		///<summary>
		///Pointer to new Btree object written here 
		///</summary>
		int flags,///
		///<summary>
		///Options 
		///</summary>
		int vfsFlags///
		///<summary>
		///Flags passed through to sqlite3_vfs.xOpen() 
		///</summary>
		) {
			BtShared pBt=null;
			///
			///<summary>
			///Shared part of btree structure 
			///</summary>
			Btree p;
			///
			///<summary>
			///Handle to return 
			///</summary>
			sqlite3_mutex mutexOpen=null;
			///
			///<summary>
			///Prevents a race condition. Ticket #3537 
			///</summary>
			int rc=SQLITE_OK;
			///
			///<summary>
			///Result code from this function 
			///</summary>
			u8 nReserve;
			///
			///<summary>
			///Byte of unused space on each page 
			///</summary>
			byte[] zDbHeader=new byte[100];
			///
			///<summary>
			///Database header content 
			///</summary>
			///
			///<summary>
			///True if opening an ephemeral, temporary database 
			///</summary>
			bool isTempDb=String.IsNullOrEmpty(zFilename);
			//zFilename==0 || zFilename[0]==0;
			///
			///<summary>
			///</summary>
			///<param name="Set the variable isMemdb to true for an in">memory database, or </param>
			///<param name="false for a file">based database.</param>
			///<param name=""></param>
			#if SQLITE_OMIT_MEMORYDB
																																																																								bool isMemdb = false;
#else
			bool isMemdb=(zFilename==":memory:")||(isTempDb&&sqlite3TempInMemory(db));
			#endif
			Debug.Assert(db!=null);
			Debug.Assert(pVfs!=null);
			Debug.Assert(sqlite3_mutex_held(db.mutex));
			Debug.Assert((flags&0xff)==flags);
			///
			///<summary>
			///flags fit in 8 bits 
			///</summary>
			///
			///<summary>
			///Only a BTREE_SINGLE database can be BTREE_UNORDERED 
			///</summary>
			Debug.Assert((flags&BTREE_UNORDERED)==0||(flags&BTREE_SINGLE)!=0);
			///
			///<summary>
			///A BTREE_SINGLE database is always a temporary and/or ephemeral 
			///</summary>
			Debug.Assert((flags&BTREE_SINGLE)==0||isTempDb);
			if((db.flags&SQLITE_NoReadlock)!=0) {
				flags|=BTREE_NO_READLOCK;
			}
			if(isMemdb) {
				flags|=BTREE_MEMORY;
			}
			if((vfsFlags&SQLITE_OPEN_MAIN_DB)!=0&&(isMemdb||isTempDb)) {
				vfsFlags=(vfsFlags&~SQLITE_OPEN_MAIN_DB)|SQLITE_OPEN_TEMP_DB;
			}
			p=new Btree();
			//sqlite3MallocZero(sizeof(Btree));
			//if( !p ){
			//  return SQLITE_NOMEM;
			//}
			p.inTrans=TRANS_NONE;
			p.db=db;
			#if !SQLITE_OMIT_SHARED_CACHE
																																																																								p.lock.pBtree = p;
p.lock.iTable = 1;
#endif
			#if !(SQLITE_OMIT_SHARED_CACHE) && !(SQLITE_OMIT_DISKIO)
																																																																								/*
** If this Btree is a candidate for shared cache, try to find an
** existing BtShared object that we can share with
*/
if( !isMemdb && !isTempDb ){
if( vfsFlags & SQLITE_OPEN_SHAREDCACHE ){
int nFullPathname = pVfs.mxPathname+1;
string zFullPathname = sqlite3Malloc(nFullPathname);
sqlite3_mutex *mutexShared;
p.sharable = 1;
if( !zFullPathname ){
p = null;//sqlite3_free(ref p);
return SQLITE_NOMEM;
}
sqlite3OsFullPathname(pVfs, zFilename, nFullPathname, zFullPathname);
mutexOpen = sqlite3MutexAlloc(SQLITE_MUTEX_STATIC_OPEN);
sqlite3_mutex_enter(mutexOpen);
mutexShared = sqlite3MutexAlloc(SQLITE_MUTEX_STATIC_MASTER);
sqlite3_mutex_enter(mutexShared);
for(pBt=GLOBAL(BtShared*,sqlite3SharedCacheList); pBt; pBt=pBt.pNext){
Debug.Assert( pBt.nRef>0 );
if( 0==strcmp(zFullPathname, sqlite3PagerFilename(pBt.pPager))
&& sqlite3PagerVfs(pBt.pPager)==pVfs ){
int iDb;
for(iDb=db.nDb-1; iDb>=0; iDb--){
Btree pExisting = db.aDb[iDb].pBt;
if( pExisting && pExisting.pBt==pBt ){
sqlite3_mutex_leave(mutexShared);
sqlite3_mutex_leave(mutexOpen);
zFullPathname = null;//sqlite3_free(ref zFullPathname);
p=null;//sqlite3_free(ref p);
return SQLITE_CONSTRAINT;
}
}
p.pBt = pBt;
pBt.nRef++;
break;
}
}
sqlite3_mutex_leave(mutexShared);
zFullPathname=null;//sqlite3_free(ref zFullPathname);
}
#if SQLITE_DEBUG
																																																																								else{
/* In debug mode, we mark all persistent databases as sharable
** even when they are not.  This exercises the locking code and
** gives more opportunity for asserts(sqlite3_mutex_held())
** statements to find locking problems.
*/
p.sharable = 1;
}
#endif
																																																																								}
#endif
			if(pBt==null) {
				///
				///<summary>
				///The following asserts make sure that structures used by the btree are
				///the right size.  This is to guard against size changes that result
				///when compiling on a different architecture.
				///
				///</summary>
				Debug.Assert(sizeof(i64)==8||sizeof(i64)==4);
				Debug.Assert(sizeof(u64)==8||sizeof(u64)==4);
				Debug.Assert(sizeof(u32)==4);
				Debug.Assert(sizeof(u16)==2);
				Debug.Assert(sizeof(Pgno)==4);
				pBt=new BtShared();
				//sqlite3MallocZero( sizeof(pBt) );
				//if( pBt==null ){
				//  rc = SQLITE_NOMEM;
				//  goto btree_open_out;
				//}
				rc=sqlite3PagerOpen(pVfs,out pBt.pPager,zFilename,EXTRA_SIZE,flags,vfsFlags,pageReinit);
				if(rc==SQLITE_OK) {
					rc=pBt.pPager.sqlite3PagerReadFileheader(zDbHeader.Length,zDbHeader);
				}
				if(rc!=SQLITE_OK) {
					goto btree_open_out;
				}
				pBt.openFlags=(u8)flags;
				pBt.db=db;
				pBt.pPager.sqlite3PagerSetBusyhandler(btreeInvokeBusyHandler,pBt);
				p.pBt=pBt;
				pBt.pCursor=null;
				pBt.pPage1=null;
				pBt.readOnly=pBt.pPager.sqlite3PagerIsreadonly();
				#if SQLITE_SECURE_DELETE
																																																																																																pBt.secureDelete = true;
#endif
				pBt.pageSize=(u32)((zDbHeader[16]<<8)|(zDbHeader[17]<<16));
				if(pBt.pageSize<512||pBt.pageSize>SQLITE_MAX_PAGE_SIZE||((pBt.pageSize-1)&pBt.pageSize)!=0) {
					pBt.pageSize=0;
					#if !SQLITE_OMIT_AUTOVACUUM
					///
					///<summary>
					///</summary>
					///<param name="If the magic name ":memory:" will create an in">memory database, then</param>
					///<param name="leave the autoVacuum mode at 0 (do not auto">vacuum), even if</param>
					///<param name="SQLITE_DEFAULT_AUTOVACUUM is true. On the other hand, if">SQLITE_DEFAULT_AUTOVACUUM is true. On the other hand, if</param>
					///<param name="SQLITE_OMIT_MEMORYDB has been defined, then ":memory:" is just a">SQLITE_OMIT_MEMORYDB has been defined, then ":memory:" is just a</param>
					///<param name="regular file">vacuum applies as per normal.</param>
					if(zFilename!=""&&!isMemdb) {
						pBt.autoVacuum=(SQLITE_DEFAULT_AUTOVACUUM!=0);
						pBt.incrVacuum=(SQLITE_DEFAULT_AUTOVACUUM==2);
					}
					#endif
					nReserve=0;
				}
				else {
					nReserve=zDbHeader[20];
					pBt.pageSizeFixed=true;
					#if !SQLITE_OMIT_AUTOVACUUM
					pBt.autoVacuum=Converter.sqlite3Get4byte(zDbHeader,36+4*4)!=0;
					pBt.incrVacuum=Converter.sqlite3Get4byte(zDbHeader,36+7*4)!=0;
					#endif
				}
				rc=pBt.pPager.sqlite3PagerSetPagesize(ref pBt.pageSize,nReserve);
				if(rc!=0)
					goto btree_open_out;
				pBt.usableSize=(u16)(pBt.pageSize-nReserve);
				Debug.Assert((pBt.pageSize&7)==0);
				///
				///<summary>
				///</summary>
				///<param name="8">byte alignment of pageSize </param>
				#if !(SQLITE_OMIT_SHARED_CACHE) && !(SQLITE_OMIT_DISKIO)
																																																																																																/* Add the new BtShared object to the linked list sharable BtShareds.
*/
if( p.sharable ){
sqlite3_mutex *mutexShared;
pBt.nRef = 1;
mutexShared = sqlite3MutexAlloc(SQLITE_MUTEX_STATIC_MASTER);
if( SQLITE_THREADSAFE && sqlite3GlobalConfig.bCoreMutex ){
pBt.mutex = sqlite3MutexAlloc(SQLITE_MUTEX_FAST);
if( pBt.mutex==null ){
rc = SQLITE_NOMEM;
db.mallocFailed = 0;
goto btree_open_out;
}
}
sqlite3_mutex_enter(mutexShared);
pBt.pNext = GLOBAL(BtShared*,sqlite3SharedCacheList);
GLOBAL(BtShared*,sqlite3SharedCacheList) = pBt;
sqlite3_mutex_leave(mutexShared);
}
#endif
			}
			#if !(SQLITE_OMIT_SHARED_CACHE) && !(SQLITE_OMIT_DISKIO)
																																																																								/* If the new Btree uses a sharable pBtShared, then link the new
** Btree into the list of all sharable Btrees for the same connection.
** The list is kept in ascending order by pBt address.
*/
if( p.sharable ){
int i;
Btree pSib;
for(i=0; i<db.nDb; i++){
if( (pSib = db.aDb[i].pBt)!=null && pSib.sharable ){
while( pSib.pPrev ){ pSib = pSib.pPrev; }
if( p.pBt<pSib.pBt ){
p.pNext = pSib;
p.pPrev = 0;
pSib.pPrev = p;
}else{
while( pSib.pNext && pSib.pNext.pBt<p.pBt ){
pSib = pSib.pNext;
}
p.pNext = pSib.pNext;
p.pPrev = pSib;
if( p.pNext ){
p.pNext.pPrev = p;
}
pSib.pNext = p;
}
break;
}
}
}
#endif
			ppBtree=p;
			btree_open_out:
			if(rc!=SQLITE_OK) {
				if(pBt!=null&&pBt.pPager!=null) {
					pBt.pPager.sqlite3PagerClose();
				}
				pBt=null;
				//    sqlite3_free(ref pBt);
				p=null;
				//    sqlite3_free(ref p);
				ppBtree=null;
			}
			else {
				///
				///<summary>
				///</summary>
				///<param name="If the B">cache size to the</param>
				///<param name="default value. Except, when opening on an existing shared pager">cache,</param>
				///<param name="do not change the pager">cache size.</param>
				///<param name=""></param>
				if(p.sqlite3BtreeSchema(0,null)==null) {
					p.pBt.pPager.sqlite3PagerSetCachesize(SQLITE_DEFAULT_CACHE_SIZE);
				}
			}
			if(mutexOpen!=null) {
				Debug.Assert(sqlite3_mutex_held(mutexOpen));
				sqlite3_mutex_leave(mutexOpen);
			}
			return rc;
		}
		///
		///<summary>
		///Decrement the BtShared.nRef counter.  When it reaches zero,
		///remove the BtShared structure from the sharing list.  Return
		///true if the BtShared.nRef counter reaches zero and return
		///false if it is still positive.
		///</summary>
		static bool removeFromSharingList(BtShared pBt) {
			#if !SQLITE_OMIT_SHARED_CACHE
																																																																								sqlite3_mutex pMaster;
BtShared pList;
bool removed = false;

Debug.Assert( sqlite3_mutex_notheld(pBt.mutex) );
pMaster = sqlite3MutexAlloc(SQLITE_MUTEX_STATIC_MASTER);
sqlite3_mutex_enter(pMaster);
pBt.nRef--;
if( pBt.nRef<=0 ){
if( GLOBAL(BtShared*,sqlite3SharedCacheList)==pBt ){
GLOBAL(BtShared*,sqlite3SharedCacheList) = pBt.pNext;
}else{
pList = GLOBAL(BtShared*,sqlite3SharedCacheList);
while( ALWAYS(pList) && pList.pNext!=pBt ){
pList=pList.pNext;
}
if( ALWAYS(pList) ){
pList.pNext = pBt.pNext;
}
}
if( SQLITE_THREADSAFE ){
sqlite3_mutex_free(pBt.mutex);
}
removed = true;
}
sqlite3_mutex_leave(pMaster);
return removed;
#else
			return true;
			#endif
		}
		///
		///<summary>
		///Make sure pBt.pTmpSpace points to an allocation of
		///MX_CELL_SIZE(pBt) bytes.
		///</summary>
		static void allocateTempSpace(BtShared pBt) {
			if(null==pBt.pTmpSpace) {
				pBt.pTmpSpace=sqlite3Malloc(pBt.pageSize);
			}
		}
		///
		///<summary>
		///Free the pBt.pTmpSpace allocation
		///</summary>
		static void freeTempSpace(BtShared pBt) {
			sqlite3PageFree(ref pBt.pTmpSpace);
		}
		///
		///<summary>
		///Close an open database and invalidate all cursors.
		///</summary>
		static int sqlite3BtreeClose(ref Btree p) {
			BtShared pBt=p.pBt;
			BtCursor pCur;
			///
			///<summary>
			///Close all cursors opened via this handle.  
			///</summary>
			Debug.Assert(sqlite3_mutex_held(p.db.mutex));
			sqlite3BtreeEnter(p);
			pCur=pBt.pCursor;
			while(pCur!=null) {
				BtCursor pTmp=pCur;
				pCur=pCur.pNext;
				if(pTmp.pBtree==p) {
					sqlite3BtreeCloseCursor(pTmp);
				}
			}
			///
			///<summary>
			///Rollback any active transaction and free the handle structure.
			///</summary>
			///<param name="The call to sqlite3BtreeRollback() drops any table">locks held by</param>
			///<param name="this handle.">this handle.</param>
			///<param name=""></param>
			sqlite3BtreeRollback(p);
			sqlite3BtreeLeave(p);
			///
			///<summary>
			///</summary>
			///<param name="If there are still other outstanding references to the shared">btree</param>
			///<param name="structure, return now. The remainder of this procedure cleans">structure, return now. The remainder of this procedure cleans</param>
			///<param name="up the shared">btree.</param>
			///<param name=""></param>
			Debug.Assert(p.wantToLock==0&&!p.locked);
			if(!p.sharable||removeFromSharingList(pBt)) {
				///
				///<summary>
				///The pBt is no longer on the sharing list, so we can access
				///it without having to hold the mutex.
				///
				///Clean out and delete the BtShared object.
				///
				///</summary>
				Debug.Assert(null==pBt.pCursor);
				pBt.pPager.sqlite3PagerClose();
				if(pBt.xFreeSchema!=null&&pBt.pSchema!=null) {
					pBt.xFreeSchema(pBt.pSchema);
				}
				pBt.pSchema=null;
				// sqlite3DbFree(0, pBt->pSchema);
				//freeTempSpace(pBt);
				pBt=null;
				//sqlite3_free(ref pBt);
			}
			#if !SQLITE_OMIT_SHARED_CACHE
																																																																								Debug.Assert( p.wantToLock==null );
Debug.Assert( p.locked==null );
if( p.pPrev ) p.pPrev.pNext = p.pNext;
if( p.pNext ) p.pNext.pPrev = p.pPrev;
#endif
			//sqlite3_free(ref p);
			return SQLITE_OK;
		}
		///
		///<summary>
		///Change the limit on the number of pages allowed in the cache.
		///
		///The maximum number of cache pages is set to the absolute
		///value of mxPage.  If mxPage is negative, the pager will
		///</summary>
		///<param name="operate asynchronously "> it will not stop to do fsync()s</param>
		///<param name="to insure data is written to the disk surface before">to insure data is written to the disk surface before</param>
		///<param name="continuing.  Transactions still work if synchronous is off,">continuing.  Transactions still work if synchronous is off,</param>
		///<param name="and the database cannot be corrupted if this program">and the database cannot be corrupted if this program</param>
		///<param name="crashes.  But if the operating system crashes or there is">crashes.  But if the operating system crashes or there is</param>
		///<param name="an abrupt power failure when synchronous is off, the database">an abrupt power failure when synchronous is off, the database</param>
		///<param name="could be left in an inconsistent and unrecoverable state.">could be left in an inconsistent and unrecoverable state.</param>
		///<param name="Synchronous is on by default so database corruption is not">Synchronous is on by default so database corruption is not</param>
		///<param name="normally a worry.">normally a worry.</param>
		///
		///<summary>
		///Change the way data is synced to disk in order to increase or decrease
		///how well the database resists damage due to OS crashes and power
		///failures.  Level 1 is the same as asynchronous (no syncs() occur and
		///there is a high probability of damage)  Level 2 is the default.  There
		///</summary>
		///<param name="is a very low but non">zero probability of damage.  Level 3 reduces the</param>
		///<param name="probability of damage to near zero but with a write performance reduction.">probability of damage to near zero but with a write performance reduction.</param>
		#if !SQLITE_OMIT_PAGER_PRAGMAS
		#endif
		///
		///<summary>
		///Return TRUE if the given btree is set to safety level 1.  In other
		///words, return TRUE if no sync() occurs on the disk files.
		///</summary>
		static int sqlite3BtreeSyncDisabled(Btree p) {
			BtShared pBt=p.pBt;
			int rc;
			Debug.Assert(sqlite3_mutex_held(p.db.mutex));
			sqlite3BtreeEnter(p);
			Debug.Assert(pBt!=null&&pBt.pPager!=null);
			rc=pBt.pPager.sqlite3PagerNosync()?1:0;
			sqlite3BtreeLeave(p);
			return rc;
		}
		///
		///<summary>
		///Change the default pages size and the number of reserved bytes per page.
		///Or, if the page size has already been fixed, return SQLITE_READONLY
		///without changing anything.
		///
		///The page size must be a power of 2 between 512 and 65536.  If the page
		///size supplied does not meet this constraint then the page size is not
		///changed.
		///
		///Page sizes are constrained to be a power of two so that the region
		///of the database file used for locking (beginning at PENDING_BYTE,
		///the first byte past the 1GB boundary, 0x40000000) needs to occur
		///at the beginning of a page.
		///
		///If parameter nReserve is less than zero, then the number of reserved
		///bytes per page is left unchanged.
		///
		///If iFix!=0 then the pageSizeFixed flag is set so that the page size
		///and autovacuum mode can no longer be changed.
		///</summary>
		///
		///<summary>
		///Return the currently defined page size
		///</summary>
		#if !(SQLITE_OMIT_PAGER_PRAGMAS) || !(SQLITE_OMIT_VACUUM)
		///
		///<summary>
		///Return the number of bytes of space at the end of every page that
		///are intentually left unused.  This is the "reserved" space that is
		///sometimes used by extensions.
		///</summary>
		static int sqlite3BtreeGetReserve(Btree p) {
			int n;
			sqlite3BtreeEnter(p);
			n=(int)(p.pBt.pageSize-p.pBt.usableSize);
			sqlite3BtreeLeave(p);
			return n;
		}
		///
		///<summary>
		///Set the maximum page count for a database if mxPage is positive.
		///No changes are made if mxPage is 0 or negative.
		///Regardless of the value of mxPage, return the maximum page count.
		///</summary>
		static Pgno sqlite3BtreeMaxPageCount(Btree p,int mxPage) {
			Pgno n;
			sqlite3BtreeEnter(p);
			n=p.pBt.pPager.sqlite3PagerMaxPageCount(mxPage);
			sqlite3BtreeLeave(p);
			return n;
		}
		///
		///<summary>
		///</summary>
		///<param name="Set the secureDelete flag if newFlag is 0 or 1.  If newFlag is ">1,</param>
		///<param name="then make no changes.  Always return the value of the secureDelete">then make no changes.  Always return the value of the secureDelete</param>
		///<param name="setting after the change.">setting after the change.</param>
		static int sqlite3BtreeSecureDelete(Btree p,int newFlag) {
			int b;
			if(p==null)
				return 0;
			sqlite3BtreeEnter(p);
			if(newFlag>=0) {
				p.pBt.secureDelete=(newFlag!=0);
			}
			b=p.pBt.secureDelete?1:0;
			sqlite3BtreeLeave(p);
			return b;
		}
		#endif
		///
		///<summary>
		///</summary>
		///<param name="Change the 'auto">vacuum' property of the database. If the 'autoVacuum'</param>
		///<param name="parameter is non">vacuum mode is enabled. If zero, it</param>
		///<param name="is disabled. The default value for the auto">vacuum property is</param>
		///<param name="determined by the SQLITE_DEFAULT_AUTOVACUUM macro.">determined by the SQLITE_DEFAULT_AUTOVACUUM macro.</param>
		static int sqlite3BtreeSetAutoVacuum(Btree p,int autoVacuum) {
			#if SQLITE_OMIT_AUTOVACUUM
																																																																								return SQLITE_READONLY;
#else
			BtShared pBt=p.pBt;
			int rc=SQLITE_OK;
			u8 av=(u8)autoVacuum;
			sqlite3BtreeEnter(p);
			if(pBt.pageSizeFixed&&(av!=0)!=pBt.autoVacuum) {
				rc=SQLITE_READONLY;
			}
			else {
				pBt.autoVacuum=av!=0;
				pBt.incrVacuum=av==2;
			}
			sqlite3BtreeLeave(p);
			return rc;
			#endif
		}
		///
		///<summary>
		///</summary>
		///<param name="Return the value of the 'auto">vacuum is</param>
		///<param name="enabled 1 is returned. Otherwise 0.">enabled 1 is returned. Otherwise 0.</param>
		static int sqlite3BtreeGetAutoVacuum(Btree p) {
			#if SQLITE_OMIT_AUTOVACUUM
																																																																								return BTREE_AUTOVACUUM_NONE;
#else
			int rc;
			sqlite3BtreeEnter(p);
			rc=((!p.pBt.autoVacuum)?BTREE_AUTOVACUUM_NONE:(!p.pBt.incrVacuum)?BTREE_AUTOVACUUM_FULL:BTREE_AUTOVACUUM_INCR);
			sqlite3BtreeLeave(p);
			return rc;
			#endif
		}
		///
		///<summary>
		///Get a reference to pPage1 of the database file.  This will
		///also acquire a readlock on that file.
		///
		///SQLITE_OK is returned on success.  If the file is not a
		///</summary>
		///<param name="well">formed database file, then SQLITE_CORRUPT is returned.</param>
		///<param name="SQLITE_BUSY is returned if the database is locked.  SQLITE_NOMEM">SQLITE_BUSY is returned if the database is locked.  SQLITE_NOMEM</param>
		///<param name="is returned if we run out of memory.">is returned if we run out of memory.</param>
		static int lockBtree(BtShared pBt) {
			int rc;
			///
			///<summary>
			///Result code from subfunctions 
			///</summary>
			MemPage pPage1=null;
			///
			///<summary>
			///Page 1 of the database file 
			///</summary>
			Pgno nPage;
			///
			///<summary>
			///Number of pages in the database 
			///</summary>
			Pgno nPageFile=0;
			///
			///<summary>
			///Number of pages in the database file 
			///</summary>
			Pgno nPageHeader;
			///
			///<summary>
			///Number of pages in the database according to hdr 
			///</summary>
			Debug.Assert(sqlite3_mutex_held(pBt.mutex));
			Debug.Assert(pBt.pPage1==null);
			rc=pBt.pPager.sqlite3PagerSharedLock();
			if(rc!=SQLITE_OK)
				return rc;
			rc=pBt.btreeGetPage(1,ref pPage1,0);
			if(rc!=SQLITE_OK)
				return rc;
			///
			///<summary>
			///Do some checking to help insure the file we opened really is
			///a valid database file.
			///
			///</summary>
			nPage=nPageHeader=Converter.sqlite3Get4byte(pPage1.aData,28);
			//get4byte(28+(u8*)pPage1->aData);
			pBt.pPager.sqlite3PagerPagecount(out nPageFile);
			if(nPage==0||memcmp(pPage1.aData,24,pPage1.aData,92,4)!=0)//memcmp(24 + (u8*)pPage1.aData, 92 + (u8*)pPage1.aData, 4) != 0)
			 {
				nPage=nPageFile;
			}
			if(nPage>0) {
				u32 pageSize;
				u32 usableSize;
				u8[] page1=pPage1.aData;
				rc=SQLITE_NOTADB;
				if(memcmp(page1,zMagicHeader,16)!=0) {
					goto page1_init_failed;
				}
				#if SQLITE_OMIT_WAL
				if(page1[18]>1) {
					pBt.readOnly=true;
				}
				if(page1[19]>1) {
					pBt.pSchema.file_format=page1[19];
					goto page1_init_failed;
				}
				#else
																																																																																																if( page1[18]>2 ){
pBt.readOnly = true;
}
if( page1[19]>2 ){
goto page1_init_failed;
}

/* If the write version is set to 2, this database should be accessed
** in WAL mode. If the log is not already open, open it now. Then 
** return SQLITE_OK and return without populating BtShared.pPage1.
** The caller detects this and calls this function again. This is
** required as the version of page 1 currently in the page1 buffer
** may not be the latest version - there may be a newer one in the log
** file.
*/
if( page1[19]==2 && pBt.doNotUseWAL==false ){
int isOpen = 0;
rc = sqlite3PagerOpenWal(pBt.pPager, ref isOpen);
if( rc!=SQLITE_OK ){
goto page1_init_failed;
}else if( isOpen==0 ){
releasePage(pPage1);
return SQLITE_OK;
}
rc = SQLITE_NOTADB;
}
#endif
				///
				///<summary>
				///The maximum embedded fraction must be exactly 25%.  And the minimum
				///</summary>
				///<param name="embedded fraction must be 12.5% for both leaf">data.</param>
				///<param name="The original design allowed these amounts to vary, but as of">The original design allowed these amounts to vary, but as of</param>
				///<param name="version 3.6.0, we require them to be fixed.">version 3.6.0, we require them to be fixed.</param>
				if(memcmp(page1,21,"\x0040\x0020\x0020",3)!=0)//   "\100\040\040"
				 {
					goto page1_init_failed;
				}
				pageSize=(u32)((page1[16]<<8)|(page1[17]<<16));
				if(((pageSize-1)&pageSize)!=0||pageSize>SQLITE_MAX_PAGE_SIZE||pageSize<=256) {
					goto page1_init_failed;
				}
				Debug.Assert((pageSize&7)==0);
				usableSize=pageSize-page1[20];
				if(pageSize!=pBt.pageSize) {
					///
					///<summary>
					///After reading the first page of the database assuming a page size
					///</summary>
					///<param name="of BtShared.pageSize, we have discovered that the page">size is</param>
					///<param name="actually pageSize. Unlock the database, leave pBt.pPage1 at">actually pageSize. Unlock the database, leave pBt.pPage1 at</param>
					///<param name="zero and return SQLITE_OK. The caller will call this function">zero and return SQLITE_OK. The caller will call this function</param>
					///<param name="again with the correct page">size.</param>
					///<param name=""></param>
					releasePage(pPage1);
					pBt.usableSize=usableSize;
					pBt.pageSize=pageSize;
					//          freeTempSpace(pBt);
					rc=pBt.pPager.sqlite3PagerSetPagesize(ref pBt.pageSize,(int)(pageSize-usableSize));
					return rc;
				}
				if((pBt.db.flags&SQLITE_RecoveryMode)==0&&nPage>nPageFile) {
					rc=SQLITE_CORRUPT_BKPT();
					goto page1_init_failed;
				}
				if(usableSize<480) {
					goto page1_init_failed;
				}
				pBt.pageSize=pageSize;
				pBt.usableSize=usableSize;
				#if !SQLITE_OMIT_AUTOVACUUM
				pBt.autoVacuum=(Converter.sqlite3Get4byte(page1,36+4*4)!=0);
				pBt.incrVacuum=(Converter.sqlite3Get4byte(page1,36+7*4)!=0);
				#endif
			}
			///
			///<summary>
			///maxLocal is the maximum amount of payload to store locally for
			///a cell.  Make sure it is small enough so that at least minFanout
			///</summary>
			///<param name="cells can will fit on one page.  We assume a 10">byte page header.</param>
			///<param name="Besides the payload, the cell must store:">Besides the payload, the cell must store:</param>
			///<param name="2">byte pointer to the cell</param>
			///<param name="4">byte child pointer</param>
			///<param name="9">byte nKey value</param>
			///<param name="4">byte nData value</param>
			///<param name="4">byte overflow page pointer</param>
			///<param name="So a cell consists of a 2">byte pointer, a header which is as much as</param>
			///<param name="17 bytes long, 0 to N bytes of payload, and an optional 4 byte overflow">17 bytes long, 0 to N bytes of payload, and an optional 4 byte overflow</param>
			///<param name="page pointer.">page pointer.</param>
			///<param name=""></param>
			pBt.maxLocal=(u16)((pBt.usableSize-12)*64/255-23);
			pBt.minLocal=(u16)((pBt.usableSize-12)*32/255-23);
			pBt.maxLeaf=(u16)(pBt.usableSize-35);
			pBt.minLeaf=(u16)((pBt.usableSize-12)*32/255-23);
			Debug.Assert(pBt.maxLeaf+23<=MX_CELL_SIZE(pBt));
			pBt.pPage1=pPage1;
			pBt.nPage=nPage;
			return SQLITE_OK;
			page1_init_failed:
			releasePage(pPage1);
			pBt.pPage1=null;
			return rc;
		}
		///
		///<summary>
		///If there are no outstanding cursors and we are not in the middle
		///of a transaction but there is a read lock on the database, then
		///this routine unrefs the first page of the database file which
		///has the effect of releasing the read lock.
		///
		///</summary>
		///<param name="If there is a transaction in progress, this routine is a no">op.</param>
		static void unlockBtreeIfUnused(BtShared pBt) {
			Debug.Assert(sqlite3_mutex_held(pBt.mutex));
			Debug.Assert(pBt.pCursor==null||pBt.inTransaction>TRANS_NONE);
			if(pBt.inTransaction==TRANS_NONE&&pBt.pPage1!=null) {
				Debug.Assert(pBt.pPage1.aData!=null);
				//Debug.Assert( sqlite3PagerRefcount( pBt.pPager ) == 1 );
				releasePage(pBt.pPage1);
				pBt.pPage1=null;
			}
		}
		///
		///<summary>
		///If pBt points to an empty file then convert that empty file
		///into a new empty database by initializing the first page of
		///the database.
		///</summary>
		static int newDatabase(BtShared pBt) {
			MemPage pP1;
			byte[] data;
			int rc;
			Debug.Assert(sqlite3_mutex_held(pBt.mutex));
			if(pBt.nPage>0) {
				return SQLITE_OK;
			}
			pP1=pBt.pPage1;
			Debug.Assert(pP1!=null);
			data=pP1.aData;
			rc=sqlite3PagerWrite(pP1.pDbPage);
			if(rc!=0)
				return rc;
			Buffer.BlockCopy(zMagicHeader,0,data,0,16);
			// memcpy(data, zMagicHeader, sizeof(zMagicHeader));
			Debug.Assert(zMagicHeader.Length==16);
			data[16]=(u8)((pBt.pageSize>>8)&0xff);
			data[17]=(u8)((pBt.pageSize>>16)&0xff);
			data[18]=1;
			data[19]=1;
			Debug.Assert(pBt.usableSize<=pBt.pageSize&&pBt.usableSize+255>=pBt.pageSize);
			data[20]=(u8)(pBt.pageSize-pBt.usableSize);
			data[21]=64;
			data[22]=32;
			data[23]=32;
			//memset(&data[24], 0, 100-24);
			pP1.zeroPage(PTF_INTKEY|PTF_LEAF|PTF_LEAFDATA);
			pBt.pageSizeFixed=true;
			#if !SQLITE_OMIT_AUTOVACUUM
			Debug.Assert(pBt.autoVacuum==true||pBt.autoVacuum==false);
			Debug.Assert(pBt.incrVacuum==true||pBt.incrVacuum==false);
			Converter.sqlite3Put4byte(data,36+4*4,pBt.autoVacuum?1:0);
			Converter.sqlite3Put4byte(data,36+7*4,pBt.incrVacuum?1:0);
			#endif
			pBt.nPage=1;
			data[31]=1;
			return SQLITE_OK;
		}
		///
		///<summary>
		///</summary>
		///<param name="Attempt to start a new transaction. A write">transaction</param>
		///<param name="is started if the second argument is nonzero, otherwise a read"></param>
		///<param name="transaction.  If the second argument is 2 or more and exclusive">transaction.  If the second argument is 2 or more and exclusive</param>
		///<param name="transaction is started, meaning that no other process is allowed">transaction is started, meaning that no other process is allowed</param>
		///<param name="to access the database.  A preexisting transaction may not be">to access the database.  A preexisting transaction may not be</param>
		///<param name="upgraded to exclusive by calling this routine a second time "> the</param>
		///<param name="exclusivity flag only works for a new transaction.">exclusivity flag only works for a new transaction.</param>
		///<param name=""></param>
		///<param name="A write">transaction must be started before attempting any</param>
		///<param name="changes to the database.  None of the following routines">changes to the database.  None of the following routines</param>
		///<param name="will work unless a transaction is started first:">will work unless a transaction is started first:</param>
		///<param name=""></param>
		///<param name="sqlite3BtreeCreateTable()">sqlite3BtreeCreateTable()</param>
		///<param name="sqlite3BtreeCreateIndex()">sqlite3BtreeCreateIndex()</param>
		///<param name="sqlite3BtreeClearTable()">sqlite3BtreeClearTable()</param>
		///<param name="sqlite3BtreeDropTable()">sqlite3BtreeDropTable()</param>
		///<param name="sqlite3BtreeInsert()">sqlite3BtreeInsert()</param>
		///<param name="sqlite3BtreeDelete()">sqlite3BtreeDelete()</param>
		///<param name="sqlite3BtreeUpdateMeta()">sqlite3BtreeUpdateMeta()</param>
		///<param name=""></param>
		///<param name="If an initial attempt to acquire the lock fails because of lock contention">If an initial attempt to acquire the lock fails because of lock contention</param>
		///<param name="and the database was previously unlocked, then invoke the busy handler">and the database was previously unlocked, then invoke the busy handler</param>
		///<param name="if there is one.  But if there was previously a read">lock, do not</param>
		///<param name="invoke the busy handler "> just return SQLITE_BUSY.  SQLITE_BUSY is</param>
		///<param name="returned when there is already a read">lock in order to avoid a deadlock.</param>
		///<param name=""></param>
		///<param name="Suppose there are two processes A and B.  A has a read lock and B has">Suppose there are two processes A and B.  A has a read lock and B has</param>
		///<param name="a reserved lock.  B tries to promote to exclusive but is blocked because">a reserved lock.  B tries to promote to exclusive but is blocked because</param>
		///<param name="of A's read lock.  A tries to promote to reserved but is blocked by B.">of A's read lock.  A tries to promote to reserved but is blocked by B.</param>
		///<param name="One or the other of the two processes must give way or there can be">One or the other of the two processes must give way or there can be</param>
		///<param name="no progress.  By returning SQLITE_BUSY and not invoking the busy callback">no progress.  By returning SQLITE_BUSY and not invoking the busy callback</param>
		///<param name="when A already has a read lock, we encourage A to give up and let B">when A already has a read lock, we encourage A to give up and let B</param>
		///<param name="proceed.">proceed.</param>
		static int sqlite3BtreeBeginTrans(Btree p,int wrflag) {
			BtShared pBt=p.pBt;
			int rc=SQLITE_OK;
			sqlite3BtreeEnter(p);
			btreeIntegrity(p);
			///
			///<summary>
			///</summary>
			///<param name="If the btree is already in a write">transaction, or it</param>
			///<param name="is already in a read">transaction</param>
			///<param name="is requested, this is a no">op.</param>
			///<param name=""></param>
			if(p.inTrans==TRANS_WRITE||(p.inTrans==TRANS_READ&&0==wrflag)) {
				goto trans_begun;
			}
			///
			///<summary>
			///</summary>
			///<param name="Write transactions are not possible on a read">only database </param>
			if(pBt.readOnly&&wrflag!=0) {
				rc=SQLITE_READONLY;
				goto trans_begun;
			}
			#if !SQLITE_OMIT_SHARED_CACHE
																																																																								/* If another database handle has already opened a write transaction
** on this shared-btree structure and a second write transaction is
** requested, return SQLITE_LOCKED.
*/
if( (wrflag && pBt.inTransaction==TRANS_WRITE) || pBt.isPending ){
sqlite3 pBlock = pBt.pWriter.db;
}else if( wrflag>1 ){
BtLock pIter;
for(pIter=pBt.pLock; pIter; pIter=pIter.pNext){
if( pIter.pBtree!=p ){
pBlock = pIter.pBtree.db;
break;
}
}
}
if( pBlock ){
sqlite3ConnectionBlocked(p.db, pBlock);
rc = SQLITE_LOCKED_SHAREDCACHE;
goto trans_begun;
}
#endif
			///
			///<summary>
			///</summary>
			///<param name="Any read">lock on</param>
			///<param name="page 1. So if some other shared">lock</param>
			///<param name="on page 1, the transaction cannot be opened. ">on page 1, the transaction cannot be opened. </param>
			rc=p.querySharedCacheTableLock(MASTER_ROOT,READ_LOCK);
			if(SQLITE_OK!=rc)
				goto trans_begun;
			pBt.initiallyEmpty=pBt.nPage==0;
			do {
				///
				///<summary>
				///Call lockBtree() until either pBt.pPage1 is populated or
				///lockBtree() returns something other than SQLITE_OK. lockBtree()
				///may return SQLITE_OK but leave pBt.pPage1 set to 0 if after
				///</summary>
				///<param name="reading page 1 it discovers that the page">size of the database</param>
				///<param name="file is not pBt.pageSize. In this case lockBtree() will update">file is not pBt.pageSize. In this case lockBtree() will update</param>
				///<param name="pBt.pageSize to the page">size of the file on disk.</param>
				///<param name=""></param>
				while(pBt.pPage1==null&&SQLITE_OK==(rc=lockBtree(pBt)))
					;
				if(rc==SQLITE_OK&&wrflag!=0) {
					if(pBt.readOnly) {
						rc=SQLITE_READONLY;
					}
					else {
						rc=pBt.pPager.sqlite3PagerBegin(wrflag>1,sqlite3TempInMemory(p.db)?1:0);
						if(rc==SQLITE_OK) {
							rc=newDatabase(pBt);
						}
					}
				}
				if(rc!=SQLITE_OK) {
					unlockBtreeIfUnused(pBt);
				}
			}
			while((rc&0xFF)==SQLITE_BUSY&&pBt.inTransaction==TRANS_NONE&&btreeInvokeBusyHandler(pBt)!=0);
			if(rc==SQLITE_OK) {
				if(p.inTrans==TRANS_NONE) {
					pBt.nTransaction++;
					#if !SQLITE_OMIT_SHARED_CACHE
																																																																																																																								if( p.sharable ){
Debug.Assert( p.lock.pBtree==p && p.lock.iTable==1 );
p.lock.eLock = READ_LOCK;
p.lock.pNext = pBt.pLock;
pBt.pLock = &p.lock;
}
#endif
				}
				p.inTrans=(wrflag!=0?TRANS_WRITE:TRANS_READ);
				if(p.inTrans>pBt.inTransaction) {
					pBt.inTransaction=p.inTrans;
				}
				if(wrflag!=0) {
					MemPage pPage1=pBt.pPage1;
					#if !SQLITE_OMIT_SHARED_CACHE
																																																																																																																								Debug.Assert( !pBt.pWriter );
pBt.pWriter = p;
pBt.isExclusive = (u8)(wrflag>1);
#endif
					///
					///<summary>
					///</summary>
					///<param name="If the db">size header field is incorrect (as it may be if an old</param>
					///<param name="client has been writing the database file), update it now. Doing">client has been writing the database file), update it now. Doing</param>
					///<param name="this sooner rather than later means the database size can safely ">this sooner rather than later means the database size can safely </param>
					///<param name="re">read the database size from page 1 if a savepoint or transaction</param>
					///<param name="rollback occurs within the transaction.">rollback occurs within the transaction.</param>
					if(pBt.nPage!=Converter.sqlite3Get4byte(pPage1.aData,28)) {
						rc=sqlite3PagerWrite(pPage1.pDbPage);
						if(rc==SQLITE_OK) {
							Converter.sqlite3Put4byte(pPage1.aData,(u32)28,pBt.nPage);
						}
					}
				}
			}
			trans_begun:
			if(rc==SQLITE_OK&&wrflag!=0) {
				///
				///<summary>
				///This call makes sure that the pager has the correct number of
				///open savepoints. If the second parameter is greater than 0 and
				///</summary>
				///<param name="the sub">journal is not already open, then it will be opened here.</param>
				///<param name=""></param>
				rc=pBt.pPager.sqlite3PagerOpenSavepoint(p.db.nSavepoint);
			}
			btreeIntegrity(p);
			sqlite3BtreeLeave(p);
			return rc;
		}
		#if !SQLITE_OMIT_AUTOVACUUM
		///
		///<summary>
		///Move the open database page pDbPage to location iFreePage in the
		///database. The pDbPage reference remains valid.
		///
		///The isCommit flag indicates that there is no need to remember that
		///the journal needs to be sync()ed before database page pDbPage.pgno
		///can be written to. The caller has already promised not to write to that
		///page.
		///</summary>
		static int relocatePage(BtShared pBt,///
		///<summary>
		///Btree 
		///</summary>
		MemPage pDbPage,///
		///<summary>
		///Open page to move 
		///</summary>
		u8 eType,///
		///<summary>
		///Pointer map 'type' entry for pDbPage 
		///</summary>
		Pgno iPtrPage,///
		///<summary>
		///</summary>
		///<param name="Pointer map 'page">no' entry for pDbPage </param>
		Pgno iFreePage,///
		///<summary>
		///The location to move pDbPage to 
		///</summary>
		int isCommit///
		///<summary>
		///isCommit flag passed to sqlite3PagerMovepage 
		///</summary>
		) {
			MemPage pPtrPage=new MemPage();
			///
			///<summary>
			///The page that contains a pointer to pDbPage 
			///</summary>
			Pgno iDbPage=pDbPage.pgno;
			Pager pPager=pBt.pPager;
			int rc;
			Debug.Assert(eType==PTRMAP_OVERFLOW2||eType==PTRMAP_OVERFLOW1||eType==PTRMAP_BTREE||eType==PTRMAP_ROOTPAGE);
			Debug.Assert(sqlite3_mutex_held(pBt.mutex));
			Debug.Assert(pDbPage.pBt==pBt);
			///
			///<summary>
			///Move page iDbPage from its current location to page number iFreePage 
			///</summary>
			TRACE("AUTOVACUUM: Moving %d to free page %d (ptr page %d type %d)\n",iDbPage,iFreePage,iPtrPage,eType);
			rc=pPager.sqlite3PagerMovepage(pDbPage.pDbPage,iFreePage,isCommit);
			if(rc!=SQLITE_OK) {
				return rc;
			}
			pDbPage.pgno=iFreePage;
			///
			///<summary>
			///</summary>
			///<param name="If pDbPage was a btree">page, then it may have child pages and/or cells</param>
			///<param name="that point to overflow pages. The pointer map entries for all these">that point to overflow pages. The pointer map entries for all these</param>
			///<param name="pages need to be changed.">pages need to be changed.</param>
			///<param name=""></param>
			///<param name="If pDbPage is an overflow page, then the first 4 bytes may store a">If pDbPage is an overflow page, then the first 4 bytes may store a</param>
			///<param name="pointer to a subsequent overflow page. If this is the case, then">pointer to a subsequent overflow page. If this is the case, then</param>
			///<param name="the pointer map needs to be updated for the subsequent overflow page.">the pointer map needs to be updated for the subsequent overflow page.</param>
			///<param name=""></param>
			if(eType==PTRMAP_BTREE||eType==PTRMAP_ROOTPAGE) {
				rc=pDbPage.setChildPtrmaps();
				if(rc!=SQLITE_OK) {
					return rc;
				}
			}
			else {
				Pgno nextOvfl=Converter.sqlite3Get4byte(pDbPage.aData);
				if(nextOvfl!=0) {
					pBt.ptrmapPut(nextOvfl,PTRMAP_OVERFLOW2,iFreePage,ref rc);
					if(rc!=SQLITE_OK) {
						return rc;
					}
				}
			}
			///
			///<summary>
			///Fix the database pointer on page iPtrPage that pointed at iDbPage so
			///that it points at iFreePage. Also fix the pointer map entry for
			///iPtrPage.
			///
			///</summary>
			if(eType!=PTRMAP_ROOTPAGE) {
				rc=pBt.btreeGetPage(iPtrPage,ref pPtrPage,0);
				if(rc!=SQLITE_OK) {
					return rc;
				}
				rc=sqlite3PagerWrite(pPtrPage.pDbPage);
				if(rc!=SQLITE_OK) {
					releasePage(pPtrPage);
					return rc;
				}
				rc=pPtrPage.modifyPagePointer(iDbPage,iFreePage,eType);
				releasePage(pPtrPage);
				if(rc==SQLITE_OK) {
					pBt.ptrmapPut(iFreePage,eType,iPtrPage,ref rc);
				}
			}
			return rc;
		}
		///
		///<summary>
		///Forward declaration required by incrVacuumStep(). 
		///</summary>
		//static int allocateBtreePage(BtShared *, MemPage **, Pgno *, Pgno, u8);
		///
		///<summary>
		///</summary>
		///<param name="Perform a single step of an incremental">vacuum. If successful,</param>
		///<param name="return SQLITE_OK. If there is no work to do (and therefore no">return SQLITE_OK. If there is no work to do (and therefore no</param>
		///<param name="point in calling this function again), return SQLITE_DONE.">point in calling this function again), return SQLITE_DONE.</param>
		///<param name=""></param>
		///<param name="More specificly, this function attempts to re">organize the</param>
		///<param name="database so that the last page of the file currently in use">database so that the last page of the file currently in use</param>
		///<param name="is no longer in use.">is no longer in use.</param>
		///<param name=""></param>
		///<param name="If the nFin parameter is non">zero, this function assumes</param>
		///<param name="that the caller will keep calling incrVacuumStep() until">that the caller will keep calling incrVacuumStep() until</param>
		///<param name="it returns SQLITE_DONE or an error, and that nFin is the">it returns SQLITE_DONE or an error, and that nFin is the</param>
		///<param name="number of pages the database file will contain after this">number of pages the database file will contain after this</param>
		///<param name="process is complete.  If nFin is zero, it is assumed that">process is complete.  If nFin is zero, it is assumed that</param>
		///<param name="incrVacuumStep() will be called a finite amount of times">incrVacuumStep() will be called a finite amount of times</param>
		///<param name="which may or may not empty the freelist.  A full autovacuum">which may or may not empty the freelist.  A full autovacuum</param>
		///<param name="has nFin>0.  A "PRAGMA incremental_vacuum" has nFin==null.">has nFin>0.  A "PRAGMA incremental_vacuum" has nFin==null.</param>
		static int incrVacuumStep(BtShared pBt,Pgno nFin,Pgno iLastPg) {
			Pgno nFreeList;
			///
			///<summary>
			///</summary>
			///<param name="Number of pages still on the free">list </param>
			int rc;
			Debug.Assert(sqlite3_mutex_held(pBt.mutex));
			Debug.Assert(iLastPg>nFin);
			if(!PTRMAP_ISPAGE(pBt,iLastPg)&&iLastPg!=PENDING_BYTE_PAGE(pBt)) {
				u8 eType=0;
				Pgno iPtrPage=0;
				nFreeList=Converter.sqlite3Get4byte(pBt.pPage1.aData,36);
				if(nFreeList==0) {
					return SQLITE_DONE;
				}
				rc=pBt.ptrmapGet(iLastPg,ref eType,ref iPtrPage);
				if(rc!=SQLITE_OK) {
					return rc;
				}
				if(eType==PTRMAP_ROOTPAGE) {
					return SQLITE_CORRUPT_BKPT();
				}
				if(eType==PTRMAP_FREEPAGE) {
					if(nFin==0) {
						///
						///<summary>
						///</summary>
						///<param name="Remove the page from the files free">list. This is not required</param>
						///<param name="if nFin is non">list will be</param>
						///<param name="truncated to zero after this function returns, so it doesn't">truncated to zero after this function returns, so it doesn't</param>
						///<param name="matter if it still contains some garbage entries.">matter if it still contains some garbage entries.</param>
						///<param name=""></param>
						Pgno iFreePg=0;
						MemPage pFreePg=new MemPage();
						rc=allocateBtreePage(pBt,ref pFreePg,ref iFreePg,iLastPg,1);
						if(rc!=SQLITE_OK) {
							return rc;
						}
						Debug.Assert(iFreePg==iLastPg);
						releasePage(pFreePg);
					}
				}
				else {
					Pgno iFreePg=0;
					///
					///<summary>
					///Index of free page to move pLastPg to 
					///</summary>
					MemPage pLastPg=new MemPage();
					rc=pBt.btreeGetPage(iLastPg,ref pLastPg,0);
					if(rc!=SQLITE_OK) {
						return rc;
					}
					///
					///<summary>
					///If nFin is zero, this loop runs exactly once and page pLastPg
					///is swapped with the first free page pulled off the free list.
					///
					///On the other hand, if nFin is greater than zero, then keep
					///</summary>
					///<param name="looping until a free">page located within the first nFin pages</param>
					///<param name="of the file is found.">of the file is found.</param>
					///<param name=""></param>
					do {
						MemPage pFreePg=new MemPage();
						rc=allocateBtreePage(pBt,ref pFreePg,ref iFreePg,0,0);
						if(rc!=SQLITE_OK) {
							releasePage(pLastPg);
							return rc;
						}
						releasePage(pFreePg);
					}
					while(nFin!=0&&iFreePg>nFin);
					Debug.Assert(iFreePg<iLastPg);
					rc=sqlite3PagerWrite(pLastPg.pDbPage);
					if(rc==SQLITE_OK) {
						rc=relocatePage(pBt,pLastPg,eType,iPtrPage,iFreePg,(nFin!=0)?1:0);
					}
					releasePage(pLastPg);
					if(rc!=SQLITE_OK) {
						return rc;
					}
				}
			}
			if(nFin==0) {
				iLastPg--;
				while(iLastPg==PENDING_BYTE_PAGE(pBt)||PTRMAP_ISPAGE(pBt,iLastPg)) {
					if(PTRMAP_ISPAGE(pBt,iLastPg)) {
						MemPage pPg=new MemPage();
						rc=pBt.btreeGetPage(iLastPg,ref pPg,0);
						if(rc!=SQLITE_OK) {
							return rc;
						}
						rc=sqlite3PagerWrite(pPg.pDbPage);
						releasePage(pPg);
						if(rc!=SQLITE_OK) {
							return rc;
						}
					}
					iLastPg--;
				}
				pBt.pPager.sqlite3PagerTruncateImage(iLastPg);
				pBt.nPage=iLastPg;
			}
			return SQLITE_OK;
		}
		///
		///<summary>
		///</summary>
		///<param name="A write">transaction must be opened before calling this function.</param>
		///<param name="It performs a single unit of work towards an incremental vacuum.">It performs a single unit of work towards an incremental vacuum.</param>
		///<param name=""></param>
		///<param name="If the incremental vacuum is finished after this function has run,">If the incremental vacuum is finished after this function has run,</param>
		///<param name="SQLITE_DONE is returned. If it is not finished, but no error occurred,">SQLITE_DONE is returned. If it is not finished, but no error occurred,</param>
		///<param name="SQLITE_OK is returned. Otherwise an SQLite error code.">SQLITE_OK is returned. Otherwise an SQLite error code.</param>
		static int sqlite3BtreeIncrVacuum(Btree p) {
			int rc;
			BtShared pBt=p.pBt;
			sqlite3BtreeEnter(p);
			Debug.Assert(pBt.inTransaction==TRANS_WRITE&&p.inTrans==TRANS_WRITE);
			if(!pBt.autoVacuum) {
				rc=SQLITE_DONE;
			}
			else {
				pBt.invalidateAllOverflowCache();
				rc=incrVacuumStep(pBt,0,pBt.btreePagecount());
				if(rc==SQLITE_OK) {
					rc=sqlite3PagerWrite(pBt.pPage1.pDbPage);
					Converter.sqlite3Put4byte(pBt.pPage1.aData,(u32)28,pBt.nPage);
					//put4byte(&pBt->pPage1->aData[28], pBt->nPage);
				}
			}
			sqlite3BtreeLeave(p);
			return rc;
		}
		///
		///<summary>
		///This routine is called prior to sqlite3PagerCommit when a transaction
		///</summary>
		///<param name="is commited for an auto">vacuum database.</param>
		///<param name=""></param>
		///<param name="If SQLITE_OK is returned, then pnTrunc is set to the number of pages">If SQLITE_OK is returned, then pnTrunc is set to the number of pages</param>
		///<param name="the database file should be truncated to during the commit process.">the database file should be truncated to during the commit process.</param>
		///<param name="i.e. the database has been reorganized so that only the first pnTrunc">i.e. the database has been reorganized so that only the first pnTrunc</param>
		///<param name="pages are in use.">pages are in use.</param>
		static int autoVacuumCommit(BtShared pBt) {
			int rc=SQLITE_OK;
			Pager pPager=pBt.pPager;
			// VVA_ONLY( int nRef = sqlite3PagerRefcount(pPager) );
			#if !NDEBUG || DEBUG
																																																																								  int nRef = sqlite3PagerRefcount( pPager );
#else
			int nRef=0;
			#endif
			Debug.Assert(sqlite3_mutex_held(pBt.mutex));
			pBt.invalidateAllOverflowCache();
			Debug.Assert(pBt.autoVacuum);
			if(!pBt.incrVacuum) {
				Pgno nFin;
				///
				///<summary>
				///Number of pages in database after autovacuuming 
				///</summary>
				Pgno nFree;
				///
				///<summary>
				///Number of pages on the freelist initially 
				///</summary>
				Pgno nPtrmap;
				///
				///<summary>
				///Number of PtrMap pages to be freed 
				///</summary>
				Pgno iFree;
				///
				///<summary>
				///The next page to be freed 
				///</summary>
				int nEntry;
				///
				///<summary>
				///Number of entries on one ptrmap page 
				///</summary>
				Pgno nOrig;
				///
				///<summary>
				///Database size before freeing 
				///</summary>
				nOrig=pBt.btreePagecount();
				if(PTRMAP_ISPAGE(pBt,nOrig)||nOrig==PENDING_BYTE_PAGE(pBt)) {
					///
					///<summary>
					///It is not possible to create a database for which the final page
					///</summary>
					///<param name="is either a pointer">byte page. If one</param>
					///<param name="is encountered, this indicates corruption.">is encountered, this indicates corruption.</param>
					///<param name=""></param>
					return SQLITE_CORRUPT_BKPT();
				}
				nFree=Converter.sqlite3Get4byte(pBt.pPage1.aData,36);
				nEntry=(int)pBt.usableSize/5;
				nPtrmap=(Pgno)((nFree-nOrig+PTRMAP_PAGENO(pBt,nOrig)+(Pgno)nEntry)/nEntry);
				nFin=nOrig-nFree-nPtrmap;
				if(nOrig>PENDING_BYTE_PAGE(pBt)&&nFin<PENDING_BYTE_PAGE(pBt)) {
					nFin--;
				}
				while(PTRMAP_ISPAGE(pBt,nFin)||nFin==PENDING_BYTE_PAGE(pBt)) {
					nFin--;
				}
				if(nFin>nOrig)
					return SQLITE_CORRUPT_BKPT();
				for(iFree=nOrig;iFree>nFin&&rc==SQLITE_OK;iFree--) {
					rc=incrVacuumStep(pBt,nFin,iFree);
				}
				if((rc==SQLITE_DONE||rc==SQLITE_OK)&&nFree>0) {
					rc=sqlite3PagerWrite(pBt.pPage1.pDbPage);
					Converter.sqlite3Put4byte(pBt.pPage1.aData,32,0);
					Converter.sqlite3Put4byte(pBt.pPage1.aData,36,0);
					Converter.sqlite3Put4byte(pBt.pPage1.aData,(u32)28,nFin);
					pBt.pPager.sqlite3PagerTruncateImage(nFin);
					pBt.nPage=nFin;
				}
				if(rc!=SQLITE_OK) {
					pPager.sqlite3PagerRollback();
				}
			}
			Debug.Assert(nRef==pPager.sqlite3PagerRefcount());
			return rc;
		}
		#else
																																																// define setChildPtrmaps(x) SQLITE_OK
#endif
		///
		///<summary>
		///</summary>
		///<param name="This routine does the first phase of a two">phase commit.  This routine</param>
		///<param name="causes a rollback journal to be created (if it does not already exist)">causes a rollback journal to be created (if it does not already exist)</param>
		///<param name="and populated with enough information so that if a power loss occurs">and populated with enough information so that if a power loss occurs</param>
		///<param name="the database can be restored to its original state by playing back">the database can be restored to its original state by playing back</param>
		///<param name="the journal.  Then the contents of the journal are flushed out to">the journal.  Then the contents of the journal are flushed out to</param>
		///<param name="the disk.  After the journal is safely on oxide, the changes to the">the disk.  After the journal is safely on oxide, the changes to the</param>
		///<param name="database are written into the database file and flushed to oxide.">database are written into the database file and flushed to oxide.</param>
		///<param name="At the end of this call, the rollback journal still exists on the">At the end of this call, the rollback journal still exists on the</param>
		///<param name="disk and we are still holding all locks, so the transaction has not">disk and we are still holding all locks, so the transaction has not</param>
		///<param name="committed.  See sqlite3BtreeCommitPhaseTwo() for the second phase of the">committed.  See sqlite3BtreeCommitPhaseTwo() for the second phase of the</param>
		///<param name="commit process.">commit process.</param>
		///<param name=""></param>
		///<param name="This call is a no">transaction is currently active on pBt.</param>
		///<param name=""></param>
		///<param name="Otherwise, sync the database file for the btree pBt. zMaster points to">Otherwise, sync the database file for the btree pBt. zMaster points to</param>
		///<param name="the name of a master journal file that should be written into the">the name of a master journal file that should be written into the</param>
		///<param name="individual journal file, or is NULL, indicating no master journal file">individual journal file, or is NULL, indicating no master journal file</param>
		///<param name="(single database transaction).">(single database transaction).</param>
		///<param name=""></param>
		///<param name="When this is called, the master journal should already have been">When this is called, the master journal should already have been</param>
		///<param name="created, populated with this journal pointer and synced to disk.">created, populated with this journal pointer and synced to disk.</param>
		///<param name=""></param>
		///<param name="Once this is routine has returned, the only thing required to commit">Once this is routine has returned, the only thing required to commit</param>
		///<param name="the write">transaction for this database file is to delete the journal.</param>
		static int sqlite3BtreeCommitPhaseOne(Btree p,string zMaster) {
			int rc=SQLITE_OK;
			if(p.inTrans==TRANS_WRITE) {
				BtShared pBt=p.pBt;
				sqlite3BtreeEnter(p);
				#if !SQLITE_OMIT_AUTOVACUUM
				if(pBt.autoVacuum) {
					rc=autoVacuumCommit(pBt);
					if(rc!=SQLITE_OK) {
						sqlite3BtreeLeave(p);
						return rc;
					}
				}
				#endif
				rc=pBt.pPager.sqlite3PagerCommitPhaseOne(zMaster,false);
				sqlite3BtreeLeave(p);
			}
			return rc;
		}
		///
		///<summary>
		///This function is called from both BtreeCommitPhaseTwo() and BtreeRollback()
		///at the conclusion of a transaction.
		///</summary>
		static void btreeEndTransaction(Btree p) {
			BtShared pBt=p.pBt;
			Debug.Assert(sqlite3BtreeHoldsMutex(p));
			pBt.btreeClearHasContent();
			if(p.inTrans>TRANS_NONE&&p.db.activeVdbeCnt>1) {
				///
				///<summary>
				///If there are other active statements that belong to this database
				///</summary>
				///<param name="handle, downgrade to a read">only transaction. The other statements</param>
				///<param name="may still be reading from the database.  ">may still be reading from the database.  </param>
				p.downgradeAllSharedCacheTableLocks();
				p.inTrans=TRANS_READ;
			}
			else {
				///
				///<summary>
				///If the handle had any kind of transaction open, decrement the
				///transaction count of the shared btree. If the transaction count
				///reaches 0, set the shared state to TRANS_NONE. The unlockBtreeIfUnused()
				///call below will unlock the pager.  
				///</summary>
				if(p.inTrans!=TRANS_NONE) {
					p.clearAllSharedCacheTableLocks();
					pBt.nTransaction--;
					if(0==pBt.nTransaction) {
						pBt.inTransaction=TRANS_NONE;
					}
				}
				///
				///<summary>
				///Set the current transaction state to TRANS_NONE and unlock the
				///pager if this call closed the only read or write transaction.  
				///</summary>
				p.inTrans=TRANS_NONE;
				unlockBtreeIfUnused(pBt);
			}
			btreeIntegrity(p);
		}
		///
		///<summary>
		///Commit the transaction currently in progress.
		///
		///</summary>
		///<param name="This routine implements the second phase of a 2">phase commit.  The</param>
		///<param name="sqlite3BtreeCommitPhaseOne() routine does the first phase and should">sqlite3BtreeCommitPhaseOne() routine does the first phase and should</param>
		///<param name="be invoked prior to calling this routine.  The sqlite3BtreeCommitPhaseOne()">be invoked prior to calling this routine.  The sqlite3BtreeCommitPhaseOne()</param>
		///<param name="routine did all the work of writing information out to disk and flushing the">routine did all the work of writing information out to disk and flushing the</param>
		///<param name="contents so that they are written onto the disk platter.  All this">contents so that they are written onto the disk platter.  All this</param>
		///<param name="routine has to do is delete or truncate or zero the header in the">routine has to do is delete or truncate or zero the header in the</param>
		///<param name="the rollback journal (which causes the transaction to commit) and">the rollback journal (which causes the transaction to commit) and</param>
		///<param name="drop locks.">drop locks.</param>
		///<param name=""></param>
		///<param name="Normally, if an error occurs while the pager layer is attempting to ">Normally, if an error occurs while the pager layer is attempting to </param>
		///<param name="finalize the underlying journal file, this function returns an error and">finalize the underlying journal file, this function returns an error and</param>
		///<param name="the upper layer will attempt a rollback. However, if the second argument">the upper layer will attempt a rollback. However, if the second argument</param>
		///<param name="is non">file </param>
		///<param name="transaction. In this case, the transaction has already been committed ">transaction. In this case, the transaction has already been committed </param>
		///<param name="(by deleting a master journal file) and the caller will ignore this ">(by deleting a master journal file) and the caller will ignore this </param>
		///<param name="functions return code. So, even if an error occurs in the pager layer,">functions return code. So, even if an error occurs in the pager layer,</param>
		///<param name="reset the b">tree objects internal state to indicate that the write</param>
		///<param name="transaction has been closed. This is quite safe, as the pager will have">transaction has been closed. This is quite safe, as the pager will have</param>
		///<param name="transitioned to the error state.">transitioned to the error state.</param>
		///<param name=""></param>
		///<param name="This will release the write lock on the database file.  If there">This will release the write lock on the database file.  If there</param>
		///<param name="are no active cursors, it also releases the read lock.">are no active cursors, it also releases the read lock.</param>
		static int sqlite3BtreeCommitPhaseTwo(Btree p,int bCleanup) {
			if(p.inTrans==TRANS_NONE)
				return SQLITE_OK;
			sqlite3BtreeEnter(p);
			btreeIntegrity(p);
			///
			///<summary>
			///</summary>
			///<param name="If the handle has a write">btrees</param>
			///<param name="transaction and set the shared state to TRANS_READ.">transaction and set the shared state to TRANS_READ.</param>
			///<param name=""></param>
			if(p.inTrans==TRANS_WRITE) {
				int rc;
				BtShared pBt=p.pBt;
				Debug.Assert(pBt.inTransaction==TRANS_WRITE);
				Debug.Assert(pBt.nTransaction>0);
				rc=pBt.pPager.sqlite3PagerCommitPhaseTwo();
				if(rc!=SQLITE_OK&&bCleanup==0) {
					sqlite3BtreeLeave(p);
					return rc;
				}
				pBt.inTransaction=TRANS_READ;
			}
			btreeEndTransaction(p);
			sqlite3BtreeLeave(p);
			return SQLITE_OK;
		}
		///
		///<summary>
		///Do both phases of a commit.
		///</summary>
		static int sqlite3BtreeCommit(Btree p) {
			int rc;
			sqlite3BtreeEnter(p);
			rc=sqlite3BtreeCommitPhaseOne(p,null);
			if(rc==SQLITE_OK) {
				rc=sqlite3BtreeCommitPhaseTwo(p,0);
			}
			sqlite3BtreeLeave(p);
			return rc;
		}
		#if !NDEBUG || DEBUG
																																																/*
** Return the number of write-cursors open on this handle. This is for use
** in Debug.Assert() expressions, so it is only compiled if NDEBUG is not
** defined.
**
** For the purposes of this routine, a write-cursor is any cursor that
** is capable of writing to the databse.  That means the cursor was
** originally opened for writing and the cursor has not be disabled
** by having its state changed to CURSOR_FAULT.
*/
static int countWriteCursors( BtShared pBt )
{
  BtCursor pCur;
  int r = 0;
  for ( pCur = pBt.pCursor; pCur != null; pCur = pCur.pNext )
  {
    if ( pCur.wrFlag != 0 && pCur.eState != CURSOR_FAULT )
      r++;
  }
  return r;
}
#else
		static int countWriteCursors(BtShared pBt) {
			return -1;
		}
		#endif
		///
		///<summary>
		///This routine sets the state to CURSOR_FAULT and the error
		///code to errCode for every cursor on BtShared that pBtree
		///references.
		///
		///Every cursor is tripped, including cursors that belong
		///to other database connections that happen to be sharing
		///the cache with pBtree.
		///
		///This routine gets called when a rollback occurs.
		///All cursors using the same cache must be tripped
		///to prevent them from trying to use the btree after
		///the rollback.  The rollback may have deleted tables
		///or moved root pages, so it is not sufficient to
		///save the state of the cursor.  The cursor must be
		///invalidated.
		///</summary>
		static void sqlite3BtreeTripAllCursors(Btree pBtree,int errCode) {
			BtCursor p;
			sqlite3BtreeEnter(pBtree);
			for(p=pBtree.pBt.pCursor;p!=null;p=p.pNext) {
				int i;
				p.sqlite3BtreeClearCursor();
				p.eState=CURSOR_FAULT;
				p.skipNext=errCode;
				for(i=0;i<=p.iPage;i++) {
					releasePage(p.apPage[i]);
					p.apPage[i]=null;
				}
			}
			sqlite3BtreeLeave(pBtree);
		}
		///
		///<summary>
		///Rollback the transaction in progress.  All cursors will be
		///invalided by this operation.  Any attempt to use a cursor
		///that was open at the beginning of this operation will result
		///in an error.
		///
		///This will release the write lock on the database file.  If there
		///are no active cursors, it also releases the read lock.
		///</summary>
		static int sqlite3BtreeRollback(Btree p) {
			int rc;
			BtShared pBt=p.pBt;
			MemPage pPage1=new MemPage();
			sqlite3BtreeEnter(p);
			rc=pBt.saveAllCursors(0,null);
			#if !SQLITE_OMIT_SHARED_CACHE
																																																																								if( rc!=SQLITE_OK ){
/* This is a horrible situation. An IO or malloc() error occurred whilst
** trying to save cursor positions. If this is an automatic rollback (as
** the result of a constraint, malloc() failure or IO error) then
** the cache may be internally inconsistent (not contain valid trees) so
** we cannot simply return the error to the caller. Instead, abort
** all queries that may be using any of the cursors that failed to save.
*/
sqlite3BtreeTripAllCursors(p, rc);
}
#endif
			btreeIntegrity(p);
			if(p.inTrans==TRANS_WRITE) {
				int rc2;
				Debug.Assert(TRANS_WRITE==pBt.inTransaction);
				rc2=pBt.pPager.sqlite3PagerRollback();
				if(rc2!=SQLITE_OK) {
					rc=rc2;
				}
				///
				///<summary>
				///The rollback may have destroyed the pPage1.aData value.  So
				///call btreeGetPage() on page 1 again to make
				///sure pPage1.aData is set correctly. 
				///</summary>
				if(pBt.btreeGetPage(1,ref pPage1,0)==SQLITE_OK) {
					Pgno nPage=Converter.sqlite3Get4byte(pPage1.aData,28);
					testcase(nPage==0);
					if(nPage==0)
						pBt.pPager.sqlite3PagerPagecount(out nPage);
					testcase(pBt.nPage!=nPage);
					pBt.nPage=nPage;
					releasePage(pPage1);
				}
				Debug.Assert(countWriteCursors(pBt)==0);
				pBt.inTransaction=TRANS_READ;
			}
			btreeEndTransaction(p);
			sqlite3BtreeLeave(p);
			return rc;
		}
		///
		///<summary>
		///Start a statement subtransaction. The subtransaction can can be rolled
		///back independently of the main transaction. You must start a transaction
		///before starting a subtransaction. The subtransaction is ended automatically
		///if the main transaction commits or rolls back.
		///
		///Statement subtransactions are used around individual SQL statements
		///that are contained within a BEGIN...COMMIT block.  If a constraint
		///error occurs within the statement, the effect of that one statement
		///can be rolled back without having to rollback the entire transaction.
		///
		///</summary>
		///<param name="A statement sub">transaction is implemented as an anonymous savepoint. The</param>
		///<param name="value passed as the second parameter is the total number of savepoints,">value passed as the second parameter is the total number of savepoints,</param>
		///<param name="including the new anonymous savepoint, open on the B">Tree. i.e. if there</param>
		///<param name="are no active savepoints and no other statement">transactions open,</param>
		///<param name="iStatement is 1. This anonymous savepoint can be released or rolled back">iStatement is 1. This anonymous savepoint can be released or rolled back</param>
		///<param name="using the sqlite3BtreeSavepoint() function.">using the sqlite3BtreeSavepoint() function.</param>
		static int sqlite3BtreeBeginStmt(Btree p,int iStatement) {
			int rc;
			BtShared pBt=p.pBt;
			sqlite3BtreeEnter(p);
			Debug.Assert(p.inTrans==TRANS_WRITE);
			Debug.Assert(!pBt.readOnly);
			Debug.Assert(iStatement>0);
			Debug.Assert(iStatement>p.db.nSavepoint);
			Debug.Assert(pBt.inTransaction==TRANS_WRITE);
			///
			///<summary>
			///At the pager level, a statement transaction is a savepoint with
			///an index greater than all savepoints created explicitly using
			///SQL statements. It is illegal to open, release or rollback any
			///such savepoints while the statement transaction savepoint is active.
			///
			///</summary>
			rc=pBt.pPager.sqlite3PagerOpenSavepoint(iStatement);
			sqlite3BtreeLeave(p);
			return rc;
		}
		///
		///<summary>
		///The second argument to this function, op, is always SAVEPOINT_ROLLBACK
		///or SAVEPOINT_RELEASE. This function either releases or rolls back the
		///savepoint identified by parameter iSavepoint, depending on the value
		///of op.
		///
		///Normally, iSavepoint is greater than or equal to zero. However, if op is
		///</summary>
		///<param name="SAVEPOINT_ROLLBACK, then iSavepoint may also be ">1. In this case the</param>
		///<param name="contents of the entire transaction are rolled back. This is different">contents of the entire transaction are rolled back. This is different</param>
		///<param name="from a normal transaction rollback, as no locks are released and the">from a normal transaction rollback, as no locks are released and the</param>
		///<param name="transaction remains open.">transaction remains open.</param>
		static int sqlite3BtreeSavepoint(Btree p,int op,int iSavepoint) {
			int rc=SQLITE_OK;
			if(p!=null&&p.inTrans==TRANS_WRITE) {
				BtShared pBt=p.pBt;
				Debug.Assert(op==SAVEPOINT_RELEASE||op==SAVEPOINT_ROLLBACK);
				Debug.Assert(iSavepoint>=0||(iSavepoint==-1&&op==SAVEPOINT_ROLLBACK));
				sqlite3BtreeEnter(p);
				rc=pBt.pPager.sqlite3PagerSavepoint(op,iSavepoint);
				if(rc==SQLITE_OK) {
					if(iSavepoint<0&&pBt.initiallyEmpty)
						pBt.nPage=0;
					rc=newDatabase(pBt);
					pBt.nPage=Converter.sqlite3Get4byte(pBt.pPage1.aData,28);
					///
					///<summary>
					///The database size was written into the offset 28 of the header
					///when the transaction started, so we know that the value at offset
					///28 is nonzero. 
					///</summary>
					Debug.Assert(pBt.nPage>0);
				}
				sqlite3BtreeLeave(p);
			}
			return rc;
		}
		///
		///<summary>
		///Create a new cursor for the BTree whose root is on the page
		///</summary>
		///<param name="iTable. If a read">only cursor is requested, it is assumed that</param>
		///<param name="the caller already has at least a read">only transaction open</param>
		///<param name="on the database already. If a write">cursor is requested, then</param>
		///<param name="the caller is assumed to have an open write transaction.">the caller is assumed to have an open write transaction.</param>
		///<param name=""></param>
		///<param name="If wrFlag==null, then the cursor can only be used for reading.">If wrFlag==null, then the cursor can only be used for reading.</param>
		///<param name="If wrFlag==1, then the cursor can be used for reading or for">If wrFlag==1, then the cursor can be used for reading or for</param>
		///<param name="writing if other conditions for writing are also met.  These">writing if other conditions for writing are also met.  These</param>
		///<param name="are the conditions that must be met in order for writing to">are the conditions that must be met in order for writing to</param>
		///<param name="be allowed:">be allowed:</param>
		///<param name=""></param>
		///<param name="1:  The cursor must have been opened with wrFlag==1">1:  The cursor must have been opened with wrFlag==1</param>
		///<param name=""></param>
		///<param name="2:  Other database connections that share the same pager cache">2:  Other database connections that share the same pager cache</param>
		///<param name="but which are not in the READ_UNCOMMITTED state may not have">but which are not in the READ_UNCOMMITTED state may not have</param>
		///<param name="cursors open with wrFlag==null on the same table.  Otherwise">cursors open with wrFlag==null on the same table.  Otherwise</param>
		///<param name="the changes made by this write cursor would be visible to">the changes made by this write cursor would be visible to</param>
		///<param name="the read cursors in the other database connection.">the read cursors in the other database connection.</param>
		///<param name=""></param>
		///<param name="3:  The database must be writable (not on read">only media)</param>
		///<param name=""></param>
		///<param name="4:  There must be an active transaction.">4:  There must be an active transaction.</param>
		///<param name=""></param>
		///<param name="No checking is done to make sure that page iTable really is the">No checking is done to make sure that page iTable really is the</param>
		///<param name="root page of a b">tree.  If it is not, then the cursor acquired</param>
		///<param name="will not work correctly.">will not work correctly.</param>
		///<param name=""></param>
		///<param name="It is assumed that the sqlite3BtreeCursorZero() has been called">It is assumed that the sqlite3BtreeCursorZero() has been called</param>
		///<param name="on pCur to initialize the memory space prior to invoking this routine.">on pCur to initialize the memory space prior to invoking this routine.</param>
		static int btreeCursor(Btree p,///
		///<summary>
		///The btree 
		///</summary>
		int iTable,///
		///<summary>
		///Root page of table to open 
		///</summary>
		int wrFlag,///
		///<summary>
		///</summary>
		///<param name="1 to write. 0 read">only </param>
		KeyInfo pKeyInfo,///
		///<summary>
		///First arg to comparison function 
		///</summary>
		BtCursor pCur///
		///<summary>
		///Space for new cursor 
		///</summary>
		) {
			BtShared pBt=p.pBt;
			///
			///<summary>
			///</summary>
			///<param name="Shared b">tree handle </param>
			Debug.Assert(sqlite3BtreeHoldsMutex(p));
			Debug.Assert(wrFlag==0||wrFlag==1);
			///
			///<summary>
			///The following Debug.Assert statements verify that if this is a sharable
			///</summary>
			///<param name="b">tree database, the connection is holding the required table locks,</param>
			///<param name="and that no other connection has any open cursor that conflicts with">and that no other connection has any open cursor that conflicts with</param>
			///<param name="this lock.  ">this lock.  </param>
			Debug.Assert(p.hasSharedCacheTableLock((u32)iTable,pKeyInfo!=null?1:0,wrFlag+1));
			Debug.Assert(wrFlag==0||!p.hasReadConflicts((u32)iTable));
			///
			///<summary>
			///Assert that the caller has opened the required transaction. 
			///</summary>
			Debug.Assert(p.inTrans>TRANS_NONE);
			Debug.Assert(wrFlag==0||p.inTrans==TRANS_WRITE);
			Debug.Assert(pBt.pPage1!=null&&pBt.pPage1.aData!=null);
			if(NEVER(wrFlag!=0&&pBt.readOnly)) {
				return SQLITE_READONLY;
			}
			if(iTable==1&&pBt.btreePagecount()==0) {
				return SQLITE_EMPTY;
			}
			///
			///<summary>
			///Now that no other errors can occur, finish filling in the BtCursor
			///variables and link the cursor into the BtShared list.  
			///</summary>
			pCur.pgnoRoot=(Pgno)iTable;
			pCur.iPage=-1;
			pCur.pKeyInfo=pKeyInfo;
			pCur.pBtree=p;
			pCur.pBt=pBt;
			pCur.wrFlag=(u8)wrFlag;
			pCur.pNext=pBt.pCursor;
			if(pCur.pNext!=null) {
				pCur.pNext.pPrev=pCur;
			}
			pBt.pCursor=pCur;
			pCur.eState=CURSOR_INVALID;
			pCur.cachedRowid=0;
			return SQLITE_OK;
		}
		static int sqlite3BtreeCursor(Btree p,///
		///<summary>
		///The btree 
		///</summary>
		int iTable,///
		///<summary>
		///Root page of table to open 
		///</summary>
		int wrFlag,///
		///<summary>
		///</summary>
		///<param name="1 to write. 0 read">only </param>
		KeyInfo pKeyInfo,///
		///<summary>
		///First arg to xCompare() 
		///</summary>
		BtCursor pCur///
		///<summary>
		///Write new cursor here 
		///</summary>
		) {
			int rc;
			sqlite3BtreeEnter(p);
			rc=btreeCursor(p,iTable,wrFlag,pKeyInfo,pCur);
			sqlite3BtreeLeave(p);
			return rc;
		}
		///
		///<summary>
		///Return the size of a BtCursor object in bytes.
		///
		///This interfaces is needed so that users of cursors can preallocate
		///sufficient storage to hold a cursor.  The BtCursor object is opaque
		///</summary>
		///<param name="to users so they cannot do the sizeof() themselves "> they must call</param>
		///<param name="this routine.">this routine.</param>
		static int sqlite3BtreeCursorSize() {
			return -1;
			// Not Used --  return ROUND8(sizeof(BtCursor));
		}
		///
		///<summary>
		///Initialize memory that will be converted into a BtCursor object.
		///
		///The simple approach here would be to memset() the entire object
		///to zero.  But it turns out that the apPage[] and aiIdx[] arrays
		///do not need to be zeroed and they are large, so we can save a lot
		///</summary>
		///<param name="of run">time by skipping the initialization of those elements.</param>
		static void sqlite3BtreeCursorZero(BtCursor p) {
			p.Clear();
			// memset( p, 0, offsetof( BtCursor, iPage ) );
		}
		///
		///<summary>
		///Close a cursor.  The read lock on the database file is released
		///when the last cursor is closed.
		///</summary>
		static int sqlite3BtreeCloseCursor(BtCursor pCur) {
			Btree pBtree=pCur.pBtree;
			if(pBtree!=null) {
				int i;
				BtShared pBt=pCur.pBt;
				sqlite3BtreeEnter(pBtree);
				pCur.sqlite3BtreeClearCursor();
				if(pCur.pPrev!=null) {
					pCur.pPrev.pNext=pCur.pNext;
				}
				else {
					pBt.pCursor=pCur.pNext;
				}
				if(pCur.pNext!=null) {
					pCur.pNext.pPrev=pCur.pPrev;
				}
				for(i=0;i<=pCur.iPage;i++) {
					releasePage(pCur.apPage[i]);
				}
				unlockBtreeIfUnused(pBt);
				invalidateOverflowCache(pCur);
				///
				///<summary>
				///sqlite3_free(ref pCur); 
				///</summary>
				sqlite3BtreeLeave(pBtree);
			}
			return SQLITE_OK;
		}
		///
		///<summary>
		///Make sure the BtCursor* given in the argument has a valid
		///BtCursor.info structure.  If it is not already valid, call
		///btreeParseCell() to fill it in.
		///
		///BtCursor.info is a cache of the information in the current cell.
		///Using this cache reduces the number of calls to btreeParseCell().
		///
		///</summary>
		///<param name="2007">25:  There is a bug in some versions of MSVC that cause the</param>
		///<param name="compiler to crash when getCellInfo() is implemented as a macro.">compiler to crash when getCellInfo() is implemented as a macro.</param>
		///<param name="But there is a measureable speed advantage to using the macro on gcc">But there is a measureable speed advantage to using the macro on gcc</param>
		///<param name="(when less compiler optimizations like ">O0 are used and the</param>
		///<param name="compiler is not doing agressive inlining.)  So we use a real function">compiler is not doing agressive inlining.)  So we use a real function</param>
		///<param name="for MSVC and a macro for everything else.  Ticket #2457.">for MSVC and a macro for everything else.  Ticket #2457.</param>
		#if !NDEBUG
																																																static void assertCellInfo( BtCursor pCur )
{
  CellInfo info;
  int iPage = pCur.iPage;
  info = new CellInfo();//memset(info, 0, sizeof(info));
  btreeParseCell( pCur.apPage[iPage], pCur.aiIdx[iPage], ref info );
  Debug.Assert( info.GetHashCode() == pCur.info.GetHashCode() || info.Equals( pCur.info ) );//memcmp(info, pCur.info, sizeof(info))==0 );
}
#else
		//  #define assertCellInfo(x)
		static void assertCellInfo(BtCursor pCur) {
		}
		#endif
		#if _MSC_VER
		///
		///<summary>
		///Use a real function in MSVC to work around bugs in that compiler. 
		///</summary>
		static void getCellInfo(BtCursor pCur) {
			if(pCur.info.nSize==0) {
				int iPage=pCur.iPage;
				pCur.apPage[iPage].btreeParseCell(pCur.aiIdx[iPage],ref pCur.info);
				pCur.validNKey=true;
			}
			else {
				assertCellInfo(pCur);
			}
		}
		#else
																																																/* Use a macro in all other compilers so that the function is inlined */
//define getCellInfo(pCur)                                                      \
//  if( pCur.info.nSize==null ){                                                   \
//    int iPage = pCur.iPage;                                                   \
//    btreeParseCell(pCur.apPage[iPage],pCur.aiIdx[iPage],&pCur.info); \
//    pCur.validNKey = true;                                                       \
//  }else{                                                                       \
//    assertCellInfo(pCur);                                                      \
//  }
#endif
		#if !NDEBUG
																																																/*
** Return true if the given BtCursor is valid.  A valid cursor is one
** that is currently pointing to a row in a (non-empty) table.
** This is a verification routine is used only within Debug.Assert() statements.
*/
static bool sqlite3BtreeCursorIsValid( BtCursor pCur )
{
  return pCur != null && pCur.eState == CURSOR_VALID;
}
#else
		static bool sqlite3BtreeCursorIsValid(BtCursor pCur) {
			return true;
		}
		#endif
		///
		///<summary>
		///Set pSize to the size of the buffer needed to hold the value of
		///the key for the current entry.  If the cursor is not pointing
		///to a valid entry, pSize is set to 0.
		///
		///For a table with the INTKEY flag set, this routine returns the key
		///itself, not the number of bytes in the key.
		///
		///The caller must position the cursor prior to invoking this routine.
		///
		///This routine cannot fail.  It always returns SQLITE_OK.
		///</summary>
		static int sqlite3BtreeKeySize(BtCursor pCur,ref i64 pSize) {
			Debug.Assert(pCur.cursorHoldsMutex());
			Debug.Assert(pCur.eState==CURSOR_INVALID||pCur.eState==CURSOR_VALID);
			if(pCur.eState!=CURSOR_VALID) {
				pSize=0;
			}
			else {
				getCellInfo(pCur);
				pSize=pCur.info.nKey;
			}
			return SQLITE_OK;
		}
		///
		///<summary>
		///Set pSize to the number of bytes of data in the entry the
		///cursor currently points to.
		///
		///</summary>
		///<param name="The caller must guarantee that the cursor is pointing to a non">NULL</param>
		///<param name="valid entry.  In other words, the calling procedure must guarantee">valid entry.  In other words, the calling procedure must guarantee</param>
		///<param name="that the cursor has Cursor.eState==CURSOR_VALID.">that the cursor has Cursor.eState==CURSOR_VALID.</param>
		///<param name=""></param>
		///<param name="Failure is not possible.  This function always returns SQLITE_OK.">Failure is not possible.  This function always returns SQLITE_OK.</param>
		///<param name="It might just as well be a procedure (returning void) but we continue">It might just as well be a procedure (returning void) but we continue</param>
		///<param name="to return an integer result code for historical reasons.">to return an integer result code for historical reasons.</param>
		static int sqlite3BtreeDataSize(BtCursor pCur,ref u32 pSize) {
			Debug.Assert(pCur.cursorHoldsMutex());
			Debug.Assert(pCur.eState==CURSOR_VALID);
			getCellInfo(pCur);
			pSize=pCur.info.nData;
			return SQLITE_OK;
		}
		///
		///<summary>
		///Given the page number of an overflow page in the database (parameter
		///ovfl), this function finds the page number of the next page in the
		///</summary>
		///<param name="linked list of overflow pages. If possible, it uses the auto">vacuum</param>
		///<param name="pointer">map data instead of reading the content of page ovfl to do so.</param>
		///<param name=""></param>
		///<param name="If an error occurs an SQLite error code is returned. Otherwise:">If an error occurs an SQLite error code is returned. Otherwise:</param>
		///<param name=""></param>
		///<param name="The page number of the next overflow page in the linked list is">The page number of the next overflow page in the linked list is</param>
		///<param name="written to pPgnoNext. If page ovfl is the last page in its linked">written to pPgnoNext. If page ovfl is the last page in its linked</param>
		///<param name="list, pPgnoNext is set to zero.">list, pPgnoNext is set to zero.</param>
		///<param name=""></param>
		///<param name="If ppPage is not NULL, and a reference to the MemPage object corresponding">If ppPage is not NULL, and a reference to the MemPage object corresponding</param>
		///<param name="to page number pOvfl was obtained, then ppPage is set to point to that">to page number pOvfl was obtained, then ppPage is set to point to that</param>
		///<param name="reference. It is the responsibility of the caller to call releasePage()">reference. It is the responsibility of the caller to call releasePage()</param>
		///<param name="on ppPage to free the reference. In no reference was obtained (because">on ppPage to free the reference. In no reference was obtained (because</param>
		///<param name="the pointer">map was used to obtain the value for pPgnoNext), then</param>
		///<param name="ppPage is set to zero.">ppPage is set to zero.</param>
		static int getOverflowPage(BtShared pBt,///
		///<summary>
		///The database file 
		///</summary>
		Pgno ovfl,///
		///<summary>
		///Current overflow page number 
		///</summary>
		out MemPage ppPage,///
		///<summary>
		///OUT: MemPage handle (may be NULL) 
		///</summary>
		out Pgno pPgnoNext///
		///<summary>
		///OUT: Next overflow page number 
		///</summary>
		) {
			Pgno next=0;
			MemPage pPage=null;
			ppPage=null;
			int rc=SQLITE_OK;
			Debug.Assert(sqlite3_mutex_held(pBt.mutex));
			// Debug.Assert( pPgnoNext);
			#if !SQLITE_OMIT_AUTOVACUUM
			///
			///<summary>
			///Try to find the next page in the overflow list using the
			///</summary>
			///<param name="autovacuum pointer">map pages. Guess that the next page in</param>
			///<param name="the overflow list is page number (ovfl+1). If that guess turns">the overflow list is page number (ovfl+1). If that guess turns</param>
			///<param name="out to be wrong, fall back to loading the data of page">out to be wrong, fall back to loading the data of page</param>
			///<param name="number ovfl to determine the next page number.">number ovfl to determine the next page number.</param>
			if(pBt.autoVacuum) {
				Pgno pgno=0;
				Pgno iGuess=ovfl+1;
				u8 eType=0;
				while(PTRMAP_ISPAGE(pBt,iGuess)||iGuess==PENDING_BYTE_PAGE(pBt)) {
					iGuess++;
				}
				if(iGuess<=pBt.btreePagecount()) {
					rc=pBt.ptrmapGet(iGuess,ref eType,ref pgno);
					if(rc==SQLITE_OK&&eType==PTRMAP_OVERFLOW2&&pgno==ovfl) {
						next=iGuess;
						rc=SQLITE_DONE;
					}
				}
			}
			#endif
			Debug.Assert(next==0||rc==SQLITE_DONE);
			if(rc==SQLITE_OK) {
				rc=pBt.btreeGetPage(ovfl,ref pPage,0);
				Debug.Assert(rc==SQLITE_OK||pPage==null);
				if(rc==SQLITE_OK) {
					next=Converter.sqlite3Get4byte(pPage.aData);
				}
			}
			pPgnoNext=next;
			if(ppPage!=null) {
				ppPage=pPage;
			}
			else {
				releasePage(pPage);
			}
			return (rc==SQLITE_DONE?SQLITE_OK:rc);
		}
		///
		///<summary>
		///Copy data from a buffer to a page, or from a page to a buffer.
		///
		///pPayload is a pointer to data stored on database page pDbPage.
		///If argument eOp is false, then nByte bytes of data are copied
		///from pPayload to the buffer pointed at by pBuf. If eOp is true,
		///then sqlite3PagerWrite() is called on pDbPage and nByte bytes
		///of data are copied from the buffer pBuf to pPayload.
		///
		///SQLITE_OK is returned on success, otherwise an error code.
		///</summary>
		static int copyPayload(byte[] pPayload,///
		///<summary>
		///Pointer to page data 
		///</summary>
		u32 payloadOffset,///
		///<summary>
		///Offset into page data 
		///</summary>
		byte[] pBuf,///
		///<summary>
		///Pointer to buffer 
		///</summary>
		u32 pBufOffset,///
		///<summary>
		///Offset into buffer 
		///</summary>
		u32 nByte,///
		///<summary>
		///Number of bytes to copy 
		///</summary>
		int eOp,///
		///<summary>
		///0 . copy from page, 1 . copy to page 
		///</summary>
		DbPage pDbPage///
		///<summary>
		///Page containing pPayload 
		///</summary>
		) {
			if(eOp!=0) {
				///
				///<summary>
				///Copy data from buffer to page (a write operation) 
				///</summary>
				int rc=sqlite3PagerWrite(pDbPage);
				if(rc!=SQLITE_OK) {
					return rc;
				}
				Buffer.BlockCopy(pBuf,(int)pBufOffset,pPayload,(int)payloadOffset,(int)nByte);
				// memcpy( pPayload, pBuf, nByte );
			}
			else {
				///
				///<summary>
				///Copy data from page to buffer (a read operation) 
				///</summary>
				Buffer.BlockCopy(pPayload,(int)payloadOffset,pBuf,(int)pBufOffset,(int)nByte);
				//memcpy(pBuf, pPayload, nByte);
			}
			return SQLITE_OK;
		}
		//static int copyPayload(
		//  byte[] pPayload,           /* Pointer to page data */
		//  byte[] pBuf,               /* Pointer to buffer */
		//  int nByte,                 /* Number of bytes to copy */
		//  int eOp,                   /* 0 -> copy from page, 1 -> copy to page */
		//  DbPage pDbPage             /* Page containing pPayload */
		//){
		//  if( eOp!=0 ){
		//    /* Copy data from buffer to page (a write operation) */
		//    int rc = sqlite3PagerWrite(pDbPage);
		//    if( rc!=SQLITE_OK ){
		//      return rc;
		//    }
		//    memcpy(pPayload, pBuf, nByte);
		//  }else{
		//    /* Copy data from page to buffer (a read operation) */
		//    memcpy(pBuf, pPayload, nByte);
		//  }
		//  return SQLITE_OK;
		//}
		///
		///<summary>
		///This function is used to read or overwrite payload information
		///for the entry that the pCur cursor is pointing to. If the eOp
		///parameter is 0, this is a read operation (data copied into
		///</summary>
		///<param name="buffer pBuf). If it is non">zero, a write (data copied from</param>
		///<param name="buffer pBuf).">buffer pBuf).</param>
		///<param name=""></param>
		///<param name="A total of "amt" bytes are read or written beginning at "offset".">A total of "amt" bytes are read or written beginning at "offset".</param>
		///<param name="Data is read to or from the buffer pBuf.">Data is read to or from the buffer pBuf.</param>
		///<param name=""></param>
		///<param name="The content being read or written might appear on the main page">The content being read or written might appear on the main page</param>
		///<param name="or be scattered out on multiple overflow pages.">or be scattered out on multiple overflow pages.</param>
		///<param name=""></param>
		///<param name="If the BtCursor.isIncrblobHandle flag is set, and the current">If the BtCursor.isIncrblobHandle flag is set, and the current</param>
		///<param name="cursor entry uses one or more overflow pages, this function">cursor entry uses one or more overflow pages, this function</param>
		///<param name="allocates space for and lazily popluates the overflow page">list</param>
		///<param name="cache array (BtCursor.aOverflow). Subsequent calls use this">cache array (BtCursor.aOverflow). Subsequent calls use this</param>
		///<param name="cache to make seeking to the supplied offset more efficient.">cache to make seeking to the supplied offset more efficient.</param>
		///<param name=""></param>
		///<param name="Once an overflow page">list cache has been allocated, it may be</param>
		///<param name="invalidated if some other cursor writes to the same table, or if">invalidated if some other cursor writes to the same table, or if</param>
		///<param name="the cursor is moved to a different row. Additionally, in auto">vacuum</param>
		///<param name="mode, the following events may invalidate an overflow page">list cache.</param>
		///<param name=""></param>
		///<param name="An incremental vacuum,">An incremental vacuum,</param>
		///<param name="A commit in auto_vacuum="full" mode,">A commit in auto_vacuum="full" mode,</param>
		///<param name="Creating a table (may require moving an overflow page).">Creating a table (may require moving an overflow page).</param>
		static int accessPayload(BtCursor pCur,///
		///<summary>
		///Cursor pointing to entry to read from 
		///</summary>
		u32 offset,///
		///<summary>
		///Begin reading this far into payload 
		///</summary>
		u32 amt,///
		///<summary>
		///Read this many bytes 
		///</summary>
		byte[] pBuf,///
		///<summary>
		///Write the bytes into this buffer 
		///</summary>
		int eOp///
		///<summary>
		///</summary>
		///<param name="zero to read. non">zero to write. </param>
		) {
			u32 pBufOffset=0;
			byte[] aPayload;
			int rc=SQLITE_OK;
			u32 nKey;
			int iIdx=0;
			MemPage pPage=pCur.apPage[pCur.iPage];
			///
			///<summary>
			///Btree page of current entry 
			///</summary>
			BtShared pBt=pCur.pBt;
			///
			///<summary>
			///Btree this cursor belongs to 
			///</summary>
			Debug.Assert(pPage!=null);
			Debug.Assert(pCur.eState==CURSOR_VALID);
			Debug.Assert(pCur.aiIdx[pCur.iPage]<pPage.nCell);
			Debug.Assert(pCur.cursorHoldsMutex());
			getCellInfo(pCur);
			aPayload=pCur.info.pCell;
			//pCur.info.pCell + pCur.info.nHeader;
			nKey=(u32)(pPage.intKey!=0?0:(int)pCur.info.nKey);
			if(NEVER(offset+amt>nKey+pCur.info.nData)||pCur.info.nLocal>pBt.usableSize//&aPayload[pCur.info.nLocal] > &pPage.aData[pBt.usableSize]
			) {
				///
				///<summary>
				///Trying to read or write past the end of the data is an error 
				///</summary>
				return SQLITE_CORRUPT_BKPT();
			}
			///
			///<summary>
			///Check if data must be read/written to/from the btree page itself. 
			///</summary>
			if(offset<pCur.info.nLocal) {
				int a=(int)amt;
				if(a+offset>pCur.info.nLocal) {
					a=(int)(pCur.info.nLocal-offset);
				}
				rc=copyPayload(aPayload,(u32)(offset+pCur.info.iCell+pCur.info.nHeader),pBuf,pBufOffset,(u32)a,eOp,pPage.pDbPage);
				offset=0;
				pBufOffset+=(u32)a;
				//pBuf += a;
				amt-=(u32)a;
			}
			else {
				offset-=pCur.info.nLocal;
			}
			if(rc==SQLITE_OK&&amt>0) {
				u32 ovflSize=(u32)(pBt.usableSize-4);
				///
				///<summary>
				///Bytes content per ovfl page 
				///</summary>
				Pgno nextPage;
				nextPage=Converter.sqlite3Get4byte(aPayload,pCur.info.nLocal+pCur.info.iCell+pCur.info.nHeader);
				#if !SQLITE_OMIT_INCRBLOB
																																																																																																/* If the isIncrblobHandle flag is set and the BtCursor.aOverflow[]
** has not been allocated, allocate it now. The array is sized at
** one entry for each overflow page in the overflow chain. The
** page number of the first overflow page is stored in aOverflow[0],
** etc. A value of 0 in the aOverflow[] array means "not yet known"
** (the cache is lazily populated).
*/
if( pCur.isIncrblobHandle && !pCur.aOverflow ){
int nOvfl = (pCur.info.nPayload-pCur.info.nLocal+ovflSize-1)/ovflSize;
pCur.aOverflow = (Pgno *)sqlite3MallocZero(sizeof(Pgno)*nOvfl);
/* nOvfl is always positive.  If it were zero, fetchPayload would have
** been used instead of this routine. */
if( ALWAYS(nOvfl) && !pCur.aOverflow ){
rc = SQLITE_NOMEM;
}
}

/* If the overflow page-list cache has been allocated and the
** entry for the first required overflow page is valid, skip
** directly to it.
*/
if( pCur.aOverflow && pCur.aOverflow[offset/ovflSize] ){
iIdx = (offset/ovflSize);
nextPage = pCur.aOverflow[iIdx];
offset = (offset%ovflSize);
}
#endif
				for(;rc==SQLITE_OK&&amt>0&&nextPage!=0;iIdx++) {
					#if !SQLITE_OMIT_INCRBLOB
																																																																																																																								/* If required, populate the overflow page-list cache. */
if( pCur.aOverflow ){
Debug.Assert(!pCur.aOverflow[iIdx] || pCur.aOverflow[iIdx]==nextPage);
pCur.aOverflow[iIdx] = nextPage;
}
#endif
					MemPage MemPageDummy=null;
					if(offset>=ovflSize) {
						///
						///<summary>
						///The only reason to read this page is to obtain the page
						///number for the next page in the overflow chain. The page
						///data is not required. So first try to lookup the overflow
						///</summary>
						///<param name="page">list cache, if any, then fall back to the getOverflowPage()</param>
						///<param name="function.">function.</param>
						///<param name=""></param>
						#if !SQLITE_OMIT_INCRBLOB
																																																																																																																																																if( pCur.aOverflow && pCur.aOverflow[iIdx+1] ){
nextPage = pCur.aOverflow[iIdx+1];
} else
#endif
						rc=getOverflowPage(pBt,nextPage,out MemPageDummy,out nextPage);
						offset-=ovflSize;
					}
					else {
						///
						///<summary>
						///Need to read this page properly. It contains some of the
						///range of data that is being read (eOp==null) or written (eOp!=null).
						///
						///</summary>
						PgHdr pDbPage=new PgHdr();
						int a=(int)amt;
						rc=pBt.pPager.sqlite3PagerGet(nextPage,ref pDbPage);
						if(rc==SQLITE_OK) {
							aPayload=sqlite3PagerGetData(pDbPage);
							nextPage=Converter.sqlite3Get4byte(aPayload);
							if(a+offset>ovflSize) {
								a=(int)(ovflSize-offset);
							}
							rc=copyPayload(aPayload,offset+4,pBuf,pBufOffset,(u32)a,eOp,pDbPage);
							sqlite3PagerUnref(pDbPage);
							offset=0;
							amt-=(u32)a;
							pBufOffset+=(u32)a;
							//pBuf += a;
						}
					}
				}
			}
			if(rc==SQLITE_OK&&amt>0) {
				return SQLITE_CORRUPT_BKPT();
			}
			return rc;
		}
		///
		///<summary>
		///Read part of the key associated with cursor pCur.  Exactly
		///"amt" bytes will be transfered into pBuf[].  The transfer
		///begins at "offset".
		///
		///The caller must ensure that pCur is pointing to a valid row
		///in the table.
		///
		///Return SQLITE_OK on success or an error code if anything goes
		///wrong.  An error is returned if "offset+amt" is larger than
		///the available payload.
		///</summary>
		static int sqlite3BtreeKey(BtCursor pCur,u32 offset,u32 amt,byte[] pBuf) {
			Debug.Assert(pCur.cursorHoldsMutex());
			Debug.Assert(pCur.eState==CURSOR_VALID);
			Debug.Assert(pCur.iPage>=0&&pCur.apPage[pCur.iPage]!=null);
			Debug.Assert(pCur.aiIdx[pCur.iPage]<pCur.apPage[pCur.iPage].nCell);
			return accessPayload(pCur,offset,amt,pBuf,0);
		}
		///
		///<summary>
		///Read part of the data associated with cursor pCur.  Exactly
		///"amt" bytes will be transfered into pBuf[].  The transfer
		///begins at "offset".
		///
		///Return SQLITE_OK on success or an error code if anything goes
		///wrong.  An error is returned if "offset+amt" is larger than
		///the available payload.
		///</summary>
		static int sqlite3BtreeData(BtCursor pCur,u32 offset,u32 amt,byte[] pBuf) {
			int rc;
			#if !SQLITE_OMIT_INCRBLOB
																																																																								if ( pCur.eState==CURSOR_INVALID ){
return SQLITE_ABORT;
}
#endif
			Debug.Assert(pCur.cursorHoldsMutex());
			rc=pCur.restoreCursorPosition();
			if(rc==SQLITE_OK) {
				Debug.Assert(pCur.eState==CURSOR_VALID);
				Debug.Assert(pCur.iPage>=0&&pCur.apPage[pCur.iPage]!=null);
				Debug.Assert(pCur.aiIdx[pCur.iPage]<pCur.apPage[pCur.iPage].nCell);
				rc=accessPayload(pCur,offset,amt,pBuf,0);
			}
			return rc;
		}
		///
		///<summary>
		///Return a pointer to payload information from the entry that the
		///pCur cursor is pointing to.  The pointer is to the beginning of
		///the key if skipKey==null and it points to the beginning of data if
		///skipKey==1.  The number of bytes of available key/data is written
		///into pAmt.  If pAmt==null, then the value returned will not be
		///a valid pointer.
		///
		///This routine is an optimization.  It is common for the entire key
		///and data to fit on the local page and for there to be no overflow
		///pages.  When that is so, this routine can be used to access the
		///key and data without making a copy.  If the key and/or data spills
		///onto overflow pages, then accessPayload() must be used to reassemble
		///the key/data and copy it into a preallocated buffer.
		///
		///The pointer returned by this routine looks directly into the cached
		///page of the database.  The data might change or move the next time
		///any btree routine is called.
		///</summary>
		static byte[] fetchPayload(BtCursor pCur,///
		///<summary>
		///Cursor pointing to entry to read from 
		///</summary>
		ref int pAmt,///
		///<summary>
		///Write the number of available bytes here 
		///</summary>
		ref int outOffset,///
		///<summary>
		///Offset into Buffer 
		///</summary>
		bool skipKey///
		///<summary>
		///read beginning at data if this is true 
		///</summary>
		) {
			byte[] aPayload;
			MemPage pPage;
			u32 nKey;
			u32 nLocal;
			Debug.Assert(pCur!=null&&pCur.iPage>=0&&pCur.apPage[pCur.iPage]!=null);
			Debug.Assert(pCur.eState==CURSOR_VALID);
			Debug.Assert(pCur.cursorHoldsMutex());
			outOffset=-1;
			pPage=pCur.apPage[pCur.iPage];
			Debug.Assert(pCur.aiIdx[pCur.iPage]<pPage.nCell);
			if(NEVER(pCur.info.nSize==0)) {
				pCur.apPage[pCur.iPage].btreeParseCell(pCur.aiIdx[pCur.iPage],ref pCur.info);
			}
			//aPayload = pCur.info.pCell;
			//aPayload += pCur.info.nHeader;
			aPayload=sqlite3Malloc(pCur.info.nSize-pCur.info.nHeader);
			if(pPage.intKey!=0) {
				nKey=0;
			}
			else {
				nKey=(u32)pCur.info.nKey;
			}
			if(skipKey) {
				//aPayload += nKey;
				outOffset=(int)(pCur.info.iCell+pCur.info.nHeader+nKey);
				Buffer.BlockCopy(pCur.info.pCell,outOffset,aPayload,0,(int)(pCur.info.nSize-pCur.info.nHeader-nKey));
				nLocal=pCur.info.nLocal-nKey;
			}
			else {
				outOffset=(int)(pCur.info.iCell+pCur.info.nHeader);
				Buffer.BlockCopy(pCur.info.pCell,outOffset,aPayload,0,pCur.info.nSize-pCur.info.nHeader);
				nLocal=pCur.info.nLocal;
				Debug.Assert(nLocal<=nKey);
			}
			pAmt=(int)nLocal;
			return aPayload;
		}
		///
		///<summary>
		///For the entry that cursor pCur is point to, return as
		///many bytes of the key or data as are available on the local
		///</summary>
		///<param name="b">tree page.  Write the number of available bytes into pAmt.</param>
		///<param name=""></param>
		///<param name="The pointer returned is ephemeral.  The key/data may move">The pointer returned is ephemeral.  The key/data may move</param>
		///<param name="or be destroyed on the next call to any Btree routine,">or be destroyed on the next call to any Btree routine,</param>
		///<param name="including calls from other threads against the same cache.">including calls from other threads against the same cache.</param>
		///<param name="Hence, a mutex on the BtShared should be held prior to calling">Hence, a mutex on the BtShared should be held prior to calling</param>
		///<param name="this routine.">this routine.</param>
		///<param name=""></param>
		///<param name="These routines is used to get quick access to key and data">These routines is used to get quick access to key and data</param>
		///<param name="in the common case where no overflow pages are used.">in the common case where no overflow pages are used.</param>
		static byte[] sqlite3BtreeKeyFetch(BtCursor pCur,ref int pAmt,ref int outOffset) {
			byte[] p=null;
			Debug.Assert(sqlite3_mutex_held(pCur.pBtree.db.mutex));
			Debug.Assert(pCur.cursorHoldsMutex());
			if(ALWAYS(pCur.eState==CURSOR_VALID)) {
				p=fetchPayload(pCur,ref pAmt,ref outOffset,false);
			}
			return p;
		}
		static byte[] sqlite3BtreeDataFetch(BtCursor pCur,ref int pAmt,ref int outOffset) {
			byte[] p=null;
			Debug.Assert(sqlite3_mutex_held(pCur.pBtree.db.mutex));
			Debug.Assert(pCur.cursorHoldsMutex());
			if(ALWAYS(pCur.eState==CURSOR_VALID)) {
				p=fetchPayload(pCur,ref pAmt,ref outOffset,true);
			}
			return p;
		}
		///
		///<summary>
		///Move the cursor down to a new child page.  The newPgno argument is the
		///page number of the child page to move to.
		///
		///</summary>
		///<param name="This function returns SQLITE_CORRUPT if the page">header flags field of</param>
		///<param name="the new child page does not match the flags field of the parent (i.e.">the new child page does not match the flags field of the parent (i.e.</param>
		///<param name="if an intkey page appears to be the parent of a non">intkey page, or</param>
		///<param name="vice">versa).</param>
		static int moveToChild(BtCursor pCur,u32 newPgno) {
			int rc;
			int i=pCur.iPage;
			MemPage pNewPage=new MemPage();
			BtShared pBt=pCur.pBt;
			Debug.Assert(pCur.cursorHoldsMutex());
			Debug.Assert(pCur.eState==CURSOR_VALID);
			Debug.Assert(pCur.iPage<BTCURSOR_MAX_DEPTH);
			if(pCur.iPage>=(BTCURSOR_MAX_DEPTH-1)) {
				return SQLITE_CORRUPT_BKPT();
			}
			rc=getAndInitPage(pBt,newPgno,ref pNewPage);
			if(rc!=0)
				return rc;
			pCur.apPage[i+1]=pNewPage;
			pCur.aiIdx[i+1]=0;
			pCur.iPage++;
			pCur.info.nSize=0;
			pCur.validNKey=false;
			if(pNewPage.nCell<1||pNewPage.intKey!=pCur.apPage[i].intKey) {
				return SQLITE_CORRUPT_BKPT();
			}
			return SQLITE_OK;
		}
		#if !NDEBUG
																																																/*
** Page pParent is an internal (non-leaf) tree page. This function
** asserts that page number iChild is the left-child if the iIdx'th
** cell in page pParent. Or, if iIdx is equal to the total number of
** cells in pParent, that page number iChild is the right-child of
** the page.
*/
static void assertParentIndex( MemPage pParent, int iIdx, Pgno iChild )
{
  Debug.Assert( iIdx <= pParent.nCell );
  if ( iIdx == pParent.nCell )
  {
    Debug.Assert( sqlite3Get4byte( pParent.aData, pParent.hdrOffset + 8 ) == iChild );
  }
  else
  {
    Debug.Assert( sqlite3Get4byte( pParent.aData, findCell( pParent, iIdx ) ) == iChild );
  }
}
#else
		#endif
		///
		///<summary>
		///Move the cursor up to the parent page.
		///
		///pCur.idx is set to the cell index that contains the pointer
		///to the page we are coming from.  If we are coming from the
		///</summary>
		///<param name="right">most child page then pCur.idx is set to one more than</param>
		///<param name="the largest cell index.">the largest cell index.</param>
		static void moveToParent(BtCursor pCur) {
			Debug.Assert(pCur.cursorHoldsMutex());
			Debug.Assert(pCur.eState==CURSOR_VALID);
			Debug.Assert(pCur.iPage>0);
			Debug.Assert(pCur.apPage[pCur.iPage]!=null);
			pCur.apPage[pCur.iPage-1].assertParentIndex(pCur.aiIdx[pCur.iPage-1],pCur.apPage[pCur.iPage].pgno);
			releasePage(pCur.apPage[pCur.iPage]);
			pCur.iPage--;
			pCur.info.nSize=0;
			pCur.validNKey=false;
		}
		///
		///<summary>
		///</summary>
		///<param name="Move the cursor to point to the root page of its b">tree structure.</param>
		///<param name=""></param>
		///<param name="If the table has a virtual root page, then the cursor is moved to point">If the table has a virtual root page, then the cursor is moved to point</param>
		///<param name="to the virtual root page instead of the actual root page. A table has a">to the virtual root page instead of the actual root page. A table has a</param>
		///<param name="virtual root page when the actual root page contains no cells and a">virtual root page when the actual root page contains no cells and a</param>
		///<param name="single child page. This can only happen with the table rooted at page 1.">single child page. This can only happen with the table rooted at page 1.</param>
		///<param name=""></param>
		///<param name="If the b">tree structure is empty, the cursor state is set to</param>
		///<param name="CURSOR_INVALID. Otherwise, the cursor is set to point to the first">CURSOR_INVALID. Otherwise, the cursor is set to point to the first</param>
		///<param name="cell located on the root (or virtual root) page and the cursor state">cell located on the root (or virtual root) page and the cursor state</param>
		///<param name="is set to CURSOR_VALID.">is set to CURSOR_VALID.</param>
		///<param name=""></param>
		///<param name="If this function returns successfully, it may be assumed that the">If this function returns successfully, it may be assumed that the</param>
		///<param name="page">page is the expected</param>
		///<param name="kind of b">tree page (i.e. if when opening the cursor the caller did not</param>
		///<param name="specify a KeyInfo structure the flags byte is set to 0x05 or 0x0D,">specify a KeyInfo structure the flags byte is set to 0x05 or 0x0D,</param>
		///<param name="indicating a table b">tree, or if the caller did specify a KeyInfo</param>
		///<param name="structure the flags byte is set to 0x02 or 0x0A, indicating an index">structure the flags byte is set to 0x02 or 0x0A, indicating an index</param>
		///<param name="b">tree).</param>
		static int moveToRoot(BtCursor pCur) {
			MemPage pRoot;
			int rc=SQLITE_OK;
			Btree p=pCur.pBtree;
			BtShared pBt=p.pBt;
			Debug.Assert(pCur.cursorHoldsMutex());
			Debug.Assert(CURSOR_INVALID<CURSOR_REQUIRESEEK);
			Debug.Assert(CURSOR_VALID<CURSOR_REQUIRESEEK);
			Debug.Assert(CURSOR_FAULT>CURSOR_REQUIRESEEK);
			if(pCur.eState>=CURSOR_REQUIRESEEK) {
				if(pCur.eState==CURSOR_FAULT) {
					Debug.Assert(pCur.skipNext!=SQLITE_OK);
					return pCur.skipNext;
				}
				pCur.sqlite3BtreeClearCursor();
			}
			if(pCur.iPage>=0) {
				int i;
				for(i=1;i<=pCur.iPage;i++) {
					releasePage(pCur.apPage[i]);
				}
				pCur.iPage=0;
			}
			else {
				rc=getAndInitPage(pBt,pCur.pgnoRoot,ref pCur.apPage[0]);
				if(rc!=SQLITE_OK) {
					pCur.eState=CURSOR_INVALID;
					return rc;
				}
				pCur.iPage=0;
				///
				///<summary>
				///If pCur.pKeyInfo is not NULL, then the caller that opened this cursor
				///</summary>
				///<param name="expected to open it on an index b">tree. Otherwise, if pKeyInfo is</param>
				///<param name="NULL, the caller expects a table b">tree. If this is not the case,</param>
				///<param name="return an SQLITE_CORRUPT error.  ">return an SQLITE_CORRUPT error.  </param>
				Debug.Assert(pCur.apPage[0].intKey==1||pCur.apPage[0].intKey==0);
				if((pCur.pKeyInfo==null)!=(pCur.apPage[0].intKey!=0)) {
					return SQLITE_CORRUPT_BKPT();
				}
			}
			///
			///<summary>
			///Assert that the root page is of the correct type. This must be the
			///</summary>
			///<param name="case as the call to this function that loaded the root">page (either</param>
			///<param name="this call or a previous invocation) would have detected corruption">this call or a previous invocation) would have detected corruption</param>
			///<param name="if the assumption were not true, and it is not possible for the flags">if the assumption were not true, and it is not possible for the flags</param>
			///<param name="byte to have been modified while this cursor is holding a reference">byte to have been modified while this cursor is holding a reference</param>
			///<param name="to the page.  ">to the page.  </param>
			pRoot=pCur.apPage[0];
			Debug.Assert(pRoot.pgno==pCur.pgnoRoot);
			Debug.Assert(pRoot.isInit!=0&&(pCur.pKeyInfo==null)==(pRoot.intKey!=0));
			pCur.aiIdx[0]=0;
			pCur.info.nSize=0;
			pCur.atLast=0;
			pCur.validNKey=false;
			if(pRoot.nCell==0&&0==pRoot.leaf) {
				Pgno subpage;
				if(pRoot.pgno!=1)
					return SQLITE_CORRUPT_BKPT();
				subpage=Converter.sqlite3Get4byte(pRoot.aData,pRoot.hdrOffset+8);
				pCur.eState=CURSOR_VALID;
				rc=moveToChild(pCur,subpage);
			}
			else {
				pCur.eState=((pRoot.nCell>0)?CURSOR_VALID:CURSOR_INVALID);
			}
			return rc;
		}
		///
		///<summary>
		///</summary>
		///<param name="Move the cursor down to the left">most leaf entry beneath the</param>
		///<param name="entry to which it is currently pointing.">entry to which it is currently pointing.</param>
		///<param name=""></param>
		///<param name="The left"> the first</param>
		///<param name="in ascending order.">in ascending order.</param>
		static int moveToLeftmost(BtCursor pCur) {
			Pgno pgno;
			int rc=SQLITE_OK;
			MemPage pPage;
			Debug.Assert(pCur.cursorHoldsMutex());
			Debug.Assert(pCur.eState==CURSOR_VALID);
			while(rc==SQLITE_OK&&0==(pPage=pCur.apPage[pCur.iPage]).leaf) {
				Debug.Assert(pCur.aiIdx[pCur.iPage]<pPage.nCell);
				pgno=Converter.sqlite3Get4byte(pPage.aData,pPage.findCell(pCur.aiIdx[pCur.iPage]));
				rc=moveToChild(pCur,pgno);
			}
			return rc;
		}
		///
		///<summary>
		///</summary>
		///<param name="Move the cursor down to the right">most leaf entry beneath the</param>
		///<param name="page to which it is currently pointing.  Notice the difference">page to which it is currently pointing.  Notice the difference</param>
		///<param name="between moveToLeftmost() and moveToRightmost().  moveToLeftmost()">between moveToLeftmost() and moveToRightmost().  moveToLeftmost()</param>
		///<param name="finds the left">most entry beneath the *entry* whereas moveToRightmost()</param>
		///<param name="finds the right">most entry beneath the page*.</param>
		///<param name=""></param>
		///<param name="The right"> the last</param>
		///<param name="key in ascending order.">key in ascending order.</param>
		static int moveToRightmost(BtCursor pCur) {
			Pgno pgno;
			int rc=SQLITE_OK;
			MemPage pPage=null;
			Debug.Assert(pCur.cursorHoldsMutex());
			Debug.Assert(pCur.eState==CURSOR_VALID);
			while(rc==SQLITE_OK&&0==(pPage=pCur.apPage[pCur.iPage]).leaf) {
				pgno=Converter.sqlite3Get4byte(pPage.aData,pPage.hdrOffset+8);
				pCur.aiIdx[pCur.iPage]=pPage.nCell;
				rc=moveToChild(pCur,pgno);
			}
			if(rc==SQLITE_OK) {
				pCur.aiIdx[pCur.iPage]=(u16)(pPage.nCell-1);
				pCur.info.nSize=0;
				pCur.validNKey=false;
			}
			return rc;
		}
		///
		///<summary>
		///Move the cursor to the first entry in the table.  Return SQLITE_OK
		///on success.  Set pRes to 0 if the cursor actually points to something
		///or set pRes to 1 if the table is empty.
		///</summary>
		static int sqlite3BtreeFirst(BtCursor pCur,ref int pRes) {
			int rc;
			Debug.Assert(pCur.cursorHoldsMutex());
			Debug.Assert(sqlite3_mutex_held(pCur.pBtree.db.mutex));
			rc=moveToRoot(pCur);
			if(rc==SQLITE_OK) {
				if(pCur.eState==CURSOR_INVALID) {
					Debug.Assert(pCur.apPage[pCur.iPage].nCell==0);
					pRes=1;
				}
				else {
					Debug.Assert(pCur.apPage[pCur.iPage].nCell>0);
					pRes=0;
					rc=moveToLeftmost(pCur);
				}
			}
			return rc;
		}
		///
		///<summary>
		///Move the cursor to the last entry in the table.  Return SQLITE_OK
		///on success.  Set pRes to 0 if the cursor actually points to something
		///or set pRes to 1 if the table is empty.
		///</summary>
		public static int sqlite3BtreeLast(BtCursor pCur,ref int pRes) {
			int rc;
			Debug.Assert(pCur.cursorHoldsMutex());
			Debug.Assert(sqlite3_mutex_held(pCur.pBtree.db.mutex));
			///
			///<summary>
			///</summary>
			///<param name="If the cursor already points to the last entry, this is a no">op. </param>
			if(CURSOR_VALID==pCur.eState&&pCur.atLast!=0) {
				#if SQLITE_DEBUG
																																																																																																    /* This block serves to Debug.Assert() that the cursor really does point
** to the last entry in the b-tree. */
    int ii;
    for ( ii = 0; ii < pCur.iPage; ii++ )
    {
      Debug.Assert( pCur.aiIdx[ii] == pCur.apPage[ii].nCell );
    }
    Debug.Assert( pCur.aiIdx[pCur.iPage] == pCur.apPage[pCur.iPage].nCell - 1 );
    Debug.Assert( pCur.apPage[pCur.iPage].leaf != 0 );
#endif
				return SQLITE_OK;
			}
			rc=moveToRoot(pCur);
			if(rc==SQLITE_OK) {
				if(CURSOR_INVALID==pCur.eState) {
					Debug.Assert(pCur.apPage[pCur.iPage].nCell==0);
					pRes=1;
				}
				else {
					Debug.Assert(pCur.eState==CURSOR_VALID);
					pRes=0;
					rc=moveToRightmost(pCur);
					pCur.atLast=(u8)(rc==SQLITE_OK?1:0);
				}
			}
			return rc;
		}
		///
		///<summary>
		///Move the cursor so that it points to an entry near the key
		///specified by pIdxKey or intKey.   Return a success code.
		///
		///For INTKEY tables, the intKey parameter is used.  pIdxKey
		///must be NULL.  For index tables, pIdxKey is used and intKey
		///is ignored.
		///
		///If an exact match is not found, then the cursor is always
		///left pointing at a leaf page which would hold the entry if it
		///were present.  The cursor might point to an entry that comes
		///before or after the key.
		///
		///An integer is written into pRes which is the result of
		///comparing the key with the entry to which the cursor is
		///pointing.  The meaning of the integer written into
		///pRes is as follows:
		///
		///pRes<0      The cursor is left pointing at an entry that
		///is smaller than intKey/pIdxKey or if the table is empty
		///and the cursor is therefore left point to nothing.
		///
		///pRes==null     The cursor is left pointing at an entry that
		///exactly matches intKey/pIdxKey.
		///
		///pRes>0      The cursor is left pointing at an entry that
		///is larger than intKey/pIdxKey.
		///
		///</summary>
		static int sqlite3BtreeMovetoUnpacked(BtCursor pCur,///
		///<summary>
		///The cursor to be moved 
		///</summary>
		UnpackedRecord pIdxKey,///
		///<summary>
		///Unpacked index key 
		///</summary>
		i64 intKey,///
		///<summary>
		///The table key 
		///</summary>
		int biasRight,///
		///<summary>
		///If true, bias the search to the high end 
		///</summary>
		ref int pRes///
		///<summary>
		///Write search results here 
		///</summary>
		) {
			int rc;
			Debug.Assert(pCur.cursorHoldsMutex());
			Debug.Assert(sqlite3_mutex_held(pCur.pBtree.db.mutex));
			// Not needed in C# // Debug.Assert( pRes != 0 );
			Debug.Assert((pIdxKey==null)==(pCur.pKeyInfo==null));
			///
			///<summary>
			///If the cursor is already positioned at the point we are trying
			///to move to, then just return without doing any work 
			///</summary>
			if(pCur.eState==CURSOR_VALID&&pCur.validNKey&&pCur.apPage[0].intKey!=0) {
				if(pCur.info.nKey==intKey) {
					pRes=0;
					return SQLITE_OK;
				}
				if(pCur.atLast!=0&&pCur.info.nKey<intKey) {
					pRes=-1;
					return SQLITE_OK;
				}
			}
			rc=moveToRoot(pCur);
			if(rc!=0) {
				return rc;
			}
			Debug.Assert(pCur.apPage[pCur.iPage]!=null);
			Debug.Assert(pCur.apPage[pCur.iPage].isInit!=0);
			Debug.Assert(pCur.apPage[pCur.iPage].nCell>0||pCur.eState==CURSOR_INVALID);
			if(pCur.eState==CURSOR_INVALID) {
				pRes=-1;
				Debug.Assert(pCur.apPage[pCur.iPage].nCell==0);
				return SQLITE_OK;
			}
			Debug.Assert(pCur.apPage[0].intKey!=0||pIdxKey!=null);
			for(;;) {
				int lwr,upr,idx;
				Pgno chldPg;
				MemPage pPage=pCur.apPage[pCur.iPage];
				int c;
				///
				///<summary>
				///</summary>
				///<param name="pPage.nCell must be greater than zero. If this is the root">page</param>
				///<param name="the cursor would have been INVALID above and this for(;;) loop">the cursor would have been INVALID above and this for(;;) loop</param>
				///<param name="not run. If this is not the root">page, then the moveToChild() routine</param>
				///<param name="would have already detected db corruption. Similarly, pPage must">would have already detected db corruption. Similarly, pPage must</param>
				///<param name="be the right kind (index or table) of b">tree page. Otherwise</param>
				///<param name="a moveToChild() or moveToRoot() call would have detected corruption.  ">a moveToChild() or moveToRoot() call would have detected corruption.  </param>
				Debug.Assert(pPage.nCell>0);
				Debug.Assert(pPage.intKey==((pIdxKey==null)?1:0));
				lwr=0;
				upr=pPage.nCell-1;
				if(biasRight!=0) {
					pCur.aiIdx[pCur.iPage]=(u16)(idx=upr);
				}
				else {
					pCur.aiIdx[pCur.iPage]=(u16)(idx=(upr+lwr)/2);
				}
				for(;;) {
					int pCell;
					///
					///<summary>
					///Pointer to current cell in pPage 
					///</summary>
					Debug.Assert(idx==pCur.aiIdx[pCur.iPage]);
					pCur.info.nSize=0;
					pCell=pPage.findCell(idx)+pPage.childPtrSize;
					if(pPage.intKey!=0) {
						i64 nCellKey=0;
						if(pPage.hasData!=0) {
							u32 Dummy0=0;
							pCell+=getVarint32(pPage.aData,pCell,out Dummy0);
						}
						getVarint(pPage.aData,pCell,out nCellKey);
						if(nCellKey==intKey) {
							c=0;
						}
						else
							if(nCellKey<intKey) {
								c=-1;
							}
							else {
								Debug.Assert(nCellKey>intKey);
								c=+1;
							}
						pCur.validNKey=true;
						pCur.info.nKey=nCellKey;
					}
					else {
						///
						///<summary>
						///</summary>
						///<param name="The maximum supported page">size is 65536 bytes. This means that</param>
						///<param name="the maximum number of record bytes stored on an index B">Tree</param>
						///<param name="page is less than 16384 bytes and may be stored as a 2">byte</param>
						///<param name="varint. This information is used to attempt to avoid parsing">varint. This information is used to attempt to avoid parsing</param>
						///<param name="the entire cell by checking for the cases where the record is">the entire cell by checking for the cases where the record is</param>
						///<param name="stored entirely within the b">tree page by inspecting the first</param>
						///<param name="2 bytes of the cell.">2 bytes of the cell.</param>
						///<param name=""></param>
						int nCell=pPage.aData[pCell+0];
						//pCell[0];
						if(0==(nCell&0x80)&&nCell<=pPage.maxLocal) {
							///
							///<summary>
							///</summary>
							///<param name="This branch runs if the record">size field of the cell is a</param>
							///<param name="single byte varint and the record fits entirely on the main">single byte varint and the record fits entirely on the main</param>
							///<param name="b">tree page.  </param>
							c=sqlite3VdbeRecordCompare(nCell,pPage.aData,pCell+1,pIdxKey);
							//c = sqlite3VdbeRecordCompare( nCell, (void*)&pCell[1], pIdxKey );
						}
						else
							if(0==(pPage.aData[pCell+1]&0x80)//!(pCell[1] & 0x80)
							&&(nCell=((nCell&0x7f)<<7)+pPage.aData[pCell+1])<=pPage.maxLocal//pCell[1])<=pPage.maxLocal
							) {
								///
								///<summary>
								///</summary>
								///<param name="The record">size field is a 2 byte varint and the record</param>
								///<param name="fits entirely on the main b">tree page.  </param>
								c=sqlite3VdbeRecordCompare(nCell,pPage.aData,pCell+2,pIdxKey);
								//c = sqlite3VdbeRecordCompare( nCell, (void*)&pCell[2], pIdxKey );
							}
							else {
								///
								///<summary>
								///The record flows over onto one or more overflow pages. In
								///this case the whole cell needs to be parsed, a buffer allocated
								///and accessPayload() used to retrieve the record into the
								///buffer before VdbeRecordCompare() can be called. 
								///</summary>
								u8[] pCellKey;
								u8[] pCellBody=new u8[pPage.aData.Length-pCell+pPage.childPtrSize];
								Buffer.BlockCopy(pPage.aData,pCell-pPage.childPtrSize,pCellBody,0,pCellBody.Length);
								//          u8 * const pCellBody = pCell - pPage->childPtrSize;
								pPage.btreeParseCellPtr(pCellBody,ref pCur.info);
								nCell=(int)pCur.info.nKey;
								pCellKey=sqlite3Malloc(nCell);
								//if ( pCellKey == null )
								//{
								//  rc = SQLITE_NOMEM;
								//  goto moveto_finish;
								//}
								rc=accessPayload(pCur,0,(u32)nCell,pCellKey,0);
								if(rc!=0) {
									pCellKey=null;
									// sqlite3_free(ref pCellKey );
									goto moveto_finish;
								}
								c=sqlite3VdbeRecordCompare(nCell,pCellKey,pIdxKey);
								pCellKey=null;
								// sqlite3_free(ref pCellKey );
							}
					}
					if(c==0) {
						if(pPage.intKey!=0&&0==pPage.leaf) {
							lwr=idx;
							upr=lwr-1;
							break;
						}
						else {
							pRes=0;
							rc=SQLITE_OK;
							goto moveto_finish;
						}
					}
					if(c<0) {
						lwr=idx+1;
					}
					else {
						upr=idx-1;
					}
					if(lwr>upr) {
						break;
					}
					pCur.aiIdx[pCur.iPage]=(u16)(idx=(lwr+upr)/2);
				}
				Debug.Assert(lwr==upr+1);
				Debug.Assert(pPage.isInit!=0);
				if(pPage.leaf!=0) {
					chldPg=0;
				}
				else
					if(lwr>=pPage.nCell) {
						chldPg=Converter.sqlite3Get4byte(pPage.aData,pPage.hdrOffset+8);
					}
					else {
						chldPg=Converter.sqlite3Get4byte(pPage.aData,pPage.findCell(lwr));
					}
				if(chldPg==0) {
					Debug.Assert(pCur.aiIdx[pCur.iPage]<pCur.apPage[pCur.iPage].nCell);
					pRes=c;
					rc=SQLITE_OK;
					goto moveto_finish;
				}
				pCur.aiIdx[pCur.iPage]=(u16)lwr;
				pCur.info.nSize=0;
				pCur.validNKey=false;
				rc=moveToChild(pCur,chldPg);
				if(rc!=0)
					goto moveto_finish;
			}
			moveto_finish:
			return rc;
		}
		///
		///<summary>
		///Return TRUE if the cursor is not pointing at an entry of the table.
		///
		///TRUE will be returned after a call to sqlite3BtreeNext() moves
		///past the last entry in the table or sqlite3BtreePrev() moves past
		///the first entry.  TRUE is also returned if the table is empty.
		///</summary>
		static bool sqlite3BtreeEof(BtCursor pCur) {
			///
			///<summary>
			///TODO: What if the cursor is in CURSOR_REQUIRESEEK but all table entries
			///have been deleted? This API will need to change to return an error code
			///as well as the boolean result value.
			///
			///</summary>
			return (CURSOR_VALID!=pCur.eState);
		}
		///
		///<summary>
		///Advance the cursor to the next entry in the database.  If
		///successful then set pRes=0.  If the cursor
		///was already pointing to the last entry in the database before
		///this routine was called, then set pRes=1.
		///</summary>
		static int sqlite3BtreeNext(BtCursor pCur,ref int pRes) {
			int rc;
			int idx;
			MemPage pPage;
			Debug.Assert(pCur.cursorHoldsMutex());
			rc=pCur.restoreCursorPosition();
			if(rc!=SQLITE_OK) {
				return rc;
			}
			// Not needed in C# // Debug.Assert( pRes != 0 );
			if(CURSOR_INVALID==pCur.eState) {
				pRes=1;
				return SQLITE_OK;
			}
			if(pCur.skipNext>0) {
				pCur.skipNext=0;
				pRes=0;
				return SQLITE_OK;
			}
			pCur.skipNext=0;
			pPage=pCur.apPage[pCur.iPage];
			idx=++pCur.aiIdx[pCur.iPage];
			Debug.Assert(pPage.isInit!=0);
			Debug.Assert(idx<=pPage.nCell);
			pCur.info.nSize=0;
			pCur.validNKey=false;
			if(idx>=pPage.nCell) {
				if(0==pPage.leaf) {
					rc=moveToChild(pCur,Converter.sqlite3Get4byte(pPage.aData,pPage.hdrOffset+8));
					if(rc!=0)
						return rc;
					rc=moveToLeftmost(pCur);
					pRes=0;
					return rc;
				}
				do {
					if(pCur.iPage==0) {
						pRes=1;
						pCur.eState=CURSOR_INVALID;
						return SQLITE_OK;
					}
					moveToParent(pCur);
					pPage=pCur.apPage[pCur.iPage];
				}
				while(pCur.aiIdx[pCur.iPage]>=pPage.nCell);
				pRes=0;
				if(pPage.intKey!=0) {
					rc=sqlite3BtreeNext(pCur,ref pRes);
				}
				else {
					rc=SQLITE_OK;
				}
				return rc;
			}
			pRes=0;
			if(pPage.leaf!=0) {
				return SQLITE_OK;
			}
			rc=moveToLeftmost(pCur);
			return rc;
		}
		///
		///<summary>
		///Step the cursor to the back to the previous entry in the database.  If
		///successful then set pRes=0.  If the cursor
		///was already pointing to the first entry in the database before
		///this routine was called, then set pRes=1.
		///</summary>
		static int sqlite3BtreePrevious(BtCursor pCur,ref int pRes) {
			int rc;
			MemPage pPage;
			Debug.Assert(pCur.cursorHoldsMutex());
			rc=pCur.restoreCursorPosition();
			if(rc!=SQLITE_OK) {
				return rc;
			}
			pCur.atLast=0;
			if(CURSOR_INVALID==pCur.eState) {
				pRes=1;
				return SQLITE_OK;
			}
			if(pCur.skipNext<0) {
				pCur.skipNext=0;
				pRes=0;
				return SQLITE_OK;
			}
			pCur.skipNext=0;
			pPage=pCur.apPage[pCur.iPage];
			Debug.Assert(pPage.isInit!=0);
			if(0==pPage.leaf) {
				int idx=pCur.aiIdx[pCur.iPage];
				rc=moveToChild(pCur,Converter.sqlite3Get4byte(pPage.aData,pPage.findCell(idx)));
				if(rc!=0) {
					return rc;
				}
				rc=moveToRightmost(pCur);
			}
			else {
				while(pCur.aiIdx[pCur.iPage]==0) {
					if(pCur.iPage==0) {
						pCur.eState=CURSOR_INVALID;
						pRes=1;
						return SQLITE_OK;
					}
					moveToParent(pCur);
				}
				pCur.info.nSize=0;
				pCur.validNKey=false;
				pCur.aiIdx[pCur.iPage]--;
				pPage=pCur.apPage[pCur.iPage];
				if(pPage.intKey!=0&&0==pPage.leaf) {
					rc=sqlite3BtreePrevious(pCur,ref pRes);
				}
				else {
					rc=SQLITE_OK;
				}
			}
			pRes=0;
			return rc;
		}
		///
		///<summary>
		///Allocate a new page from the database file.
		///
		///The new page is marked as dirty.  (In other words, sqlite3PagerWrite()
		///has already been called on the new page.)  The new page has also
		///been referenced and the calling routine is responsible for calling
		///sqlite3PagerUnref() on the new page when it is done.
		///
		///SQLITE_OK is returned on success.  Any other return value indicates
		///an error.  ppPage and pPgno are undefined in the event of an error.
		///Do not invoke sqlite3PagerUnref() on ppPage if an error is returned.
		///
		///If the "nearby" parameter is not 0, then a (feeble) effort is made to
		///locate a page close to the page number "nearby".  This can be used in an
		///attempt to keep related pages close to each other in the database file,
		///which in turn can make database access faster.
		///
		///</summary>
		///<param name="If the "exact" parameter is not 0, and the page">number nearby exists</param>
		///<param name="anywhere on the free">list, then it is guarenteed to be returned. This</param>
		///<param name="is only used by auto">vacuum databases when allocating a new table.</param>
		static int allocateBtreePage(BtShared pBt,ref MemPage ppPage,ref Pgno pPgno,Pgno nearby,u8 exact) {
			MemPage pPage1;
			int rc;
			u32 n;
			///
			///<summary>
			///Number of pages on the freelist 
			///</summary>
			u32 k;
			///
			///<summary>
			///Number of leaves on the trunk of the freelist 
			///</summary>
			MemPage pTrunk=null;
			MemPage pPrevTrunk=null;
			Pgno mxPage;
			///
			///<summary>
			///Total size of the database file 
			///</summary>
			Debug.Assert(sqlite3_mutex_held(pBt.mutex));
			pPage1=pBt.pPage1;
			mxPage=pBt.btreePagecount();
			n=Converter.sqlite3Get4byte(pPage1.aData,36);
			testcase(n==mxPage-1);
			if(n>=mxPage) {
				return SQLITE_CORRUPT_BKPT();
			}
			if(n>0) {
				///
				///<summary>
				///There are pages on the freelist.  Reuse one of those pages. 
				///</summary>
				Pgno iTrunk;
				u8 searchList=0;
				///
				///<summary>
				///</summary>
				///<param name="If the free">list must be searched for 'nearby' </param>
				///
				///<summary>
				///</summary>
				///<param name="If the 'exact' parameter was true and a query of the pointer">map</param>
				///<param name="shows that the page 'nearby' is somewhere on the free">list, then</param>
				///<param name="the entire">list will be searched for that page.</param>
				///<param name=""></param>
				#if !SQLITE_OMIT_AUTOVACUUM
				if(exact!=0&&nearby<=mxPage) {
					u8 eType=0;
					Debug.Assert(nearby>0);
					Debug.Assert(pBt.autoVacuum);
					u32 Dummy0=0;
					rc=pBt.ptrmapGet(nearby,ref eType,ref Dummy0);
					if(rc!=0)
						return rc;
					if(eType==PTRMAP_FREEPAGE) {
						searchList=1;
					}
					pPgno=nearby;
				}
				#endif
				///
				///<summary>
				///</summary>
				///<param name="Decrement the free">list count by 1. Set iTrunk to the index of the</param>
				///<param name="first free">list trunk page. iPrevTrunk is initially 1.</param>
				rc=sqlite3PagerWrite(pPage1.pDbPage);
				if(rc!=0)
					return rc;
				Converter.sqlite3Put4byte(pPage1.aData,(u32)36,n-1);
				///
				///<summary>
				///The code within this loop is run only once if the 'searchList' variable
				///</summary>
				///<param name="is not true. Otherwise, it runs once for each trunk">page on the</param>
				///<param name="free">list until the page 'nearby' is located.</param>
				///<param name=""></param>
				do {
					pPrevTrunk=pTrunk;
					if(pPrevTrunk!=null) {
						iTrunk=Converter.sqlite3Get4byte(pPrevTrunk.aData,0);
					}
					else {
						iTrunk=Converter.sqlite3Get4byte(pPage1.aData,32);
					}
					testcase(iTrunk==mxPage);
					if(iTrunk>mxPage) {
						rc=SQLITE_CORRUPT_BKPT();
					}
					else {
						rc=pBt.btreeGetPage(iTrunk,ref pTrunk,0);
					}
					if(rc!=0) {
						pTrunk=null;
						goto end_allocate_page;
					}
					k=Converter.sqlite3Get4byte(pTrunk.aData,4);
					///
					///<summary>
					///# of leaves on this trunk page 
					///</summary>
					if(k==0&&0==searchList) {
						///
						///<summary>
						///The trunk has no leaves and the list is not being searched.
						///So extract the trunk page itself and use it as the newly
						///allocated page 
						///</summary>
						Debug.Assert(pPrevTrunk==null);
						rc=sqlite3PagerWrite(pTrunk.pDbPage);
						if(rc!=0) {
							goto end_allocate_page;
						}
						pPgno=iTrunk;
						Buffer.BlockCopy(pTrunk.aData,0,pPage1.aData,32,4);
						//memcpy( pPage1.aData[32], ref pTrunk.aData[0], 4 );
						ppPage=pTrunk;
						pTrunk=null;
						TRACE("ALLOCATE: %d trunk - %d free pages left\n",pPgno,n-1);
					}
					else
						if(k>(u32)(pBt.usableSize/4-2)) {
							///
							///<summary>
							///Value of k is out of range.  Database corruption 
							///</summary>
							rc=SQLITE_CORRUPT_BKPT();
							goto end_allocate_page;
							#if !SQLITE_OMIT_AUTOVACUUM
						}
						else
							if(searchList!=0&&nearby==iTrunk) {
								///
								///<summary>
								///The list is being searched and this trunk page is the page
								///to allocate, regardless of whether it has leaves.
								///
								///</summary>
								Debug.Assert(pPgno==iTrunk);
								ppPage=pTrunk;
								searchList=0;
								rc=sqlite3PagerWrite(pTrunk.pDbPage);
								if(rc!=0) {
									goto end_allocate_page;
								}
								if(k==0) {
									if(null==pPrevTrunk) {
										//memcpy(pPage1.aData[32], pTrunk.aData[0], 4);
										pPage1.aData[32+0]=pTrunk.aData[0+0];
										pPage1.aData[32+1]=pTrunk.aData[0+1];
										pPage1.aData[32+2]=pTrunk.aData[0+2];
										pPage1.aData[32+3]=pTrunk.aData[0+3];
									}
									else {
										rc=sqlite3PagerWrite(pPrevTrunk.pDbPage);
										if(rc!=SQLITE_OK) {
											goto end_allocate_page;
										}
										//memcpy(pPrevTrunk.aData[0], pTrunk.aData[0], 4);
										pPrevTrunk.aData[0+0]=pTrunk.aData[0+0];
										pPrevTrunk.aData[0+1]=pTrunk.aData[0+1];
										pPrevTrunk.aData[0+2]=pTrunk.aData[0+2];
										pPrevTrunk.aData[0+3]=pTrunk.aData[0+3];
									}
								}
								else {
									///
									///<summary>
									///The trunk page is required by the caller but it contains
									///</summary>
									///<param name="pointers to free">list leaves. The first leaf becomes a trunk</param>
									///<param name="page in this case.">page in this case.</param>
									///<param name=""></param>
									MemPage pNewTrunk=new MemPage();
									Pgno iNewTrunk=Converter.sqlite3Get4byte(pTrunk.aData,8);
									if(iNewTrunk>mxPage) {
										rc=SQLITE_CORRUPT_BKPT();
										goto end_allocate_page;
									}
									testcase(iNewTrunk==mxPage);
									rc=pBt.btreeGetPage(iNewTrunk,ref pNewTrunk,0);
									if(rc!=SQLITE_OK) {
										goto end_allocate_page;
									}
									rc=sqlite3PagerWrite(pNewTrunk.pDbPage);
									if(rc!=SQLITE_OK) {
										releasePage(pNewTrunk);
										goto end_allocate_page;
									}
									//memcpy(pNewTrunk.aData[0], pTrunk.aData[0], 4);
									pNewTrunk.aData[0+0]=pTrunk.aData[0+0];
									pNewTrunk.aData[0+1]=pTrunk.aData[0+1];
									pNewTrunk.aData[0+2]=pTrunk.aData[0+2];
									pNewTrunk.aData[0+3]=pTrunk.aData[0+3];
									Converter.sqlite3Put4byte(pNewTrunk.aData,(u32)4,(u32)(k-1));
									Buffer.BlockCopy(pTrunk.aData,12,pNewTrunk.aData,8,(int)(k-1)*4);
									//memcpy( pNewTrunk.aData[8], ref pTrunk.aData[12], ( k - 1 ) * 4 );
									releasePage(pNewTrunk);
									if(null==pPrevTrunk) {
										Debug.Assert(sqlite3PagerIswriteable(pPage1.pDbPage));
										Converter.sqlite3Put4byte(pPage1.aData,(u32)32,iNewTrunk);
									}
									else {
										rc=sqlite3PagerWrite(pPrevTrunk.pDbPage);
										if(rc!=0) {
											goto end_allocate_page;
										}
										Converter.sqlite3Put4byte(pPrevTrunk.aData,(u32)0,iNewTrunk);
									}
								}
								pTrunk=null;
								TRACE("ALLOCATE: %d trunk - %d free pages left\n",pPgno,n-1);
								#endif
							}
							else
								if(k>0) {
									///
									///<summary>
									///Extract a leaf from the trunk 
									///</summary>
									u32 closest;
									Pgno iPage;
									byte[] aData=pTrunk.aData;
									if(nearby>0) {
										u32 i;
										int dist;
										closest=0;
										dist=sqlite3AbsInt32((int)(Converter.sqlite3Get4byte(aData,8)-nearby));
										for(i=1;i<k;i++) {
											int d2=sqlite3AbsInt32((int)(Converter.sqlite3Get4byte(aData,8+i*4)-nearby));
											if(d2<dist) {
												closest=i;
												dist=d2;
											}
										}
									}
									else {
										closest=0;
									}
									iPage=Converter.sqlite3Get4byte(aData,8+closest*4);
									testcase(iPage==mxPage);
									if(iPage>mxPage) {
										rc=SQLITE_CORRUPT_BKPT();
										goto end_allocate_page;
									}
									testcase(iPage==mxPage);
									if(0==searchList||iPage==nearby) {
										int noContent;
										pPgno=iPage;
										TRACE("ALLOCATE: %d was leaf %d of %d on trunk %d"+": %d more free pages\n",pPgno,closest+1,k,pTrunk.pgno,n-1);
										rc=sqlite3PagerWrite(pTrunk.pDbPage);
										if(rc!=0)
											goto end_allocate_page;
										if(closest<k-1) {
											Buffer.BlockCopy(aData,(int)(4+k*4),aData,8+(int)closest*4,4);
											//memcpy( aData[8 + closest * 4], ref aData[4 + k * 4], 4 );
										}
										Converter.sqlite3Put4byte(aData,(u32)4,(k-1));
										// sqlite3Put4byte( aData, 4, k - 1 );
										noContent=!pBt.btreeGetHasContent(pPgno)?1:0;
										rc=pBt.btreeGetPage(pPgno,ref ppPage,noContent);
										if(rc==SQLITE_OK) {
											rc=sqlite3PagerWrite((ppPage).pDbPage);
											if(rc!=SQLITE_OK) {
												releasePage(ppPage);
											}
										}
										searchList=0;
									}
								}
					releasePage(pPrevTrunk);
					pPrevTrunk=null;
				}
				while(searchList!=0);
			}
			else {
				///
				///<summary>
				///There are no pages on the freelist, so create a new page at the
				///end of the file 
				///</summary>
				rc=sqlite3PagerWrite(pBt.pPage1.pDbPage);
				if(rc!=0)
					return rc;
				pBt.nPage++;
				if(pBt.nPage==PENDING_BYTE_PAGE(pBt))
					pBt.nPage++;
				#if !SQLITE_OMIT_AUTOVACUUM
				if(pBt.autoVacuum&&PTRMAP_ISPAGE(pBt,pBt.nPage)) {
					///
					///<summary>
					///</summary>
					///<param name="If pPgno refers to a pointer">map page, allocate two new pages</param>
					///<param name="at the end of the file instead of one. The first allocated page">at the end of the file instead of one. The first allocated page</param>
					///<param name="becomes a new pointer">map page, the second is used by the caller.</param>
					///<param name=""></param>
					MemPage pPg=null;
					TRACE("ALLOCATE: %d from end of file (pointer-map page)\n",pPgno);
					Debug.Assert(pBt.nPage!=PENDING_BYTE_PAGE(pBt));
					rc=pBt.btreeGetPage(pBt.nPage,ref pPg,1);
					if(rc==SQLITE_OK) {
						rc=sqlite3PagerWrite(pPg.pDbPage);
						releasePage(pPg);
					}
					if(rc!=0)
						return rc;
					pBt.nPage++;
					if(pBt.nPage==PENDING_BYTE_PAGE(pBt)) {
						pBt.nPage++;
					}
				}
				#endif
				Converter.sqlite3Put4byte(pBt.pPage1.aData,(u32)28,pBt.nPage);
				pPgno=pBt.nPage;
				Debug.Assert(pPgno!=PENDING_BYTE_PAGE(pBt));
				rc=pBt.btreeGetPage(pPgno,ref ppPage,1);
				if(rc!=0)
					return rc;
				rc=sqlite3PagerWrite((ppPage).pDbPage);
				if(rc!=SQLITE_OK) {
					releasePage(ppPage);
				}
				TRACE("ALLOCATE: %d from end of file\n",pPgno);
			}
			Debug.Assert(pPgno!=PENDING_BYTE_PAGE(pBt));
			end_allocate_page:
			releasePage(pTrunk);
			releasePage(pPrevTrunk);
			if(rc==SQLITE_OK) {
				if(sqlite3PagerPageRefcount((ppPage).pDbPage)>1) {
					releasePage(ppPage);
					return SQLITE_CORRUPT_BKPT();
				}
				(ppPage).isInit=0;
			}
			else {
				ppPage=null;
			}
			Debug.Assert(rc!=SQLITE_OK||sqlite3PagerIswriteable((ppPage).pDbPage));
			return rc;
		}
		///
		///<summary>
		///</summary>
		///<param name="This function is used to add page iPage to the database file free">list.</param>
		///<param name="It is assumed that the page is not already a part of the free">list.</param>
		///<param name=""></param>
		///<param name="The value passed as the second argument to this function is optional.">The value passed as the second argument to this function is optional.</param>
		///<param name="If the caller happens to have a pointer to the MemPage object">If the caller happens to have a pointer to the MemPage object</param>
		///<param name="corresponding to page iPage handy, it may pass it as the second value.">corresponding to page iPage handy, it may pass it as the second value.</param>
		///<param name="Otherwise, it may pass NULL.">Otherwise, it may pass NULL.</param>
		///<param name=""></param>
		///<param name="If a pointer to a MemPage object is passed as the second argument,">If a pointer to a MemPage object is passed as the second argument,</param>
		///<param name="its reference count is not altered by this function.">its reference count is not altered by this function.</param>
		static int freePage2(BtShared pBt,MemPage pMemPage,Pgno iPage) {
			MemPage pTrunk=null;
			///
			///<summary>
			///</summary>
			///<param name="Free">list trunk page </param>
			Pgno iTrunk=0;
			///
			///<summary>
			///</summary>
			///<param name="Page number of free">list trunk page </param>
			MemPage pPage1=pBt.pPage1;
			///
			///<summary>
			///Local reference to page 1 
			///</summary>
			MemPage pPage;
			///
			///<summary>
			///Page being freed. May be NULL. 
			///</summary>
			int rc;
			///
			///<summary>
			///Return Code 
			///</summary>
			int nFree;
			///
			///<summary>
			///</summary>
			///<param name="Initial number of pages on free">list </param>
			Debug.Assert(sqlite3_mutex_held(pBt.mutex));
			Debug.Assert(iPage>1);
			Debug.Assert(null==pMemPage||pMemPage.pgno==iPage);
			if(pMemPage!=null) {
				pPage=pMemPage;
				sqlite3PagerRef(pPage.pDbPage);
			}
			else {
				pPage=pBt.btreePageLookup(iPage);
			}
			///
			///<summary>
			///Increment the free page count on pPage1 
			///</summary>
			rc=sqlite3PagerWrite(pPage1.pDbPage);
			if(rc!=0)
				goto freepage_out;
			nFree=(int)Converter.sqlite3Get4byte(pPage1.aData,36);
			Converter.sqlite3Put4byte(pPage1.aData,36,nFree+1);
			if(pBt.secureDelete) {
				///
				///<summary>
				///If the secure_delete option is enabled, then
				///always fully overwrite deleted information with zeros.
				///
				///</summary>
				if((null==pPage&&((rc=pBt.btreeGetPage(iPage,ref pPage,0))!=0))||((rc=sqlite3PagerWrite(pPage.pDbPage))!=0)) {
					goto freepage_out;
				}
				Array.Clear(pPage.aData,0,(int)pPage.pBt.pageSize);
				//memset(pPage->aData, 0, pPage->pBt->pageSize);
			}
			///
			///<summary>
			///</summary>
			///<param name="If the database supports auto">map</param>
			///<param name="to indicate that the page is free.">to indicate that the page is free.</param>
			///<param name=""></param>
			#if !SQLITE_OMIT_AUTOVACUUM
			if(pBt.autoVacuum)
			#else
																																																																								if (false)
#endif
			 {
				pBt.ptrmapPut(iPage,PTRMAP_FREEPAGE,0,ref rc);
				if(rc!=0)
					goto freepage_out;
			}
			///
			///<summary>
			///</summary>
			///<param name="Now manipulate the actual database free">list structure. There are two</param>
			///<param name="possibilities. If the free">list is currently empty, or if the first</param>
			///<param name="trunk page in the free">list is full, then this page will become a</param>
			///<param name="new free">list trunk page. Otherwise, it will become a leaf of the</param>
			///<param name="first trunk page in the current free">list. This block tests if it</param>
			///<param name="is possible to add the page as a new free">list leaf.</param>
			///<param name=""></param>
			if(nFree!=0) {
				u32 nLeaf;
				///
				///<summary>
				///Initial number of leaf cells on trunk page 
				///</summary>
				iTrunk=Converter.sqlite3Get4byte(pPage1.aData,32);
				rc=pBt.btreeGetPage(iTrunk,ref pTrunk,0);
				if(rc!=SQLITE_OK) {
					goto freepage_out;
				}
				nLeaf=Converter.sqlite3Get4byte(pTrunk.aData,4);
				Debug.Assert(pBt.usableSize>32);
				if(nLeaf>(u32)pBt.usableSize/4-2) {
					rc=SQLITE_CORRUPT_BKPT();
					goto freepage_out;
				}
				if(nLeaf<(u32)pBt.usableSize/4-8) {
					///
					///<summary>
					///In this case there is room on the trunk page to insert the page
					///being freed as a new leaf.
					///
					///Note that the trunk page is not really full until it contains
					///</summary>
					///<param name="usableSize/4 "> 8 entries as we have</param>
					///<param name="coded.  But due to a coding error in versions of SQLite prior to">coded.  But due to a coding error in versions of SQLite prior to</param>
					///<param name="3.6.0, databases with freelist trunk pages holding more than">3.6.0, databases with freelist trunk pages holding more than</param>
					///<param name="usableSize/4 "> 8 entries will be reported as corrupt.  In order</param>
					///<param name="to maintain backwards compatibility with older versions of SQLite,">to maintain backwards compatibility with older versions of SQLite,</param>
					///<param name="we will continue to restrict the number of entries to usableSize/4 "> 8</param>
					///<param name="for now.  At some point in the future (once everyone has upgraded">for now.  At some point in the future (once everyone has upgraded</param>
					///<param name="to 3.6.0 or later) we should consider fixing the conditional above">to 3.6.0 or later) we should consider fixing the conditional above</param>
					///<param name="to read "usableSize/4">8".</param>
					///<param name=""></param>
					rc=sqlite3PagerWrite(pTrunk.pDbPage);
					if(rc==SQLITE_OK) {
						Converter.sqlite3Put4byte(pTrunk.aData,(u32)4,nLeaf+1);
						Converter.sqlite3Put4byte(pTrunk.aData,(u32)8+nLeaf*4,iPage);
						if(pPage!=null&&!pBt.secureDelete) {
							sqlite3PagerDontWrite(pPage.pDbPage);
						}
						rc=pBt.btreeSetHasContent(iPage);
					}
					TRACE("FREE-PAGE: %d leaf on trunk page %d\n",iPage,pTrunk.pgno);
					goto freepage_out;
				}
			}
			///
			///<summary>
			///If control flows to this point, then it was not possible to add the
			///</summary>
			///<param name="the page being freed as a leaf page of the first trunk in the free">list.</param>
			///<param name="Possibly because the free">list is empty, or possibly because the</param>
			///<param name="first trunk in the free">list is full. Either way, the page being freed</param>
			///<param name="will become the new first trunk page in the free">list.</param>
			///<param name=""></param>
			if(pPage==null&&SQLITE_OK!=(rc=pBt.btreeGetPage(iPage,ref pPage,0))) {
				goto freepage_out;
			}
			rc=sqlite3PagerWrite(pPage.pDbPage);
			if(rc!=SQLITE_OK) {
				goto freepage_out;
			}
			Converter.sqlite3Put4byte(pPage.aData,iTrunk);
			Converter.sqlite3Put4byte(pPage.aData,4,0);
			Converter.sqlite3Put4byte(pPage1.aData,(u32)32,iPage);
			TRACE("FREE-PAGE: %d new trunk page replacing %d\n",pPage.pgno,iTrunk);
			freepage_out:
			if(pPage!=null) {
				pPage.isInit=0;
			}
			releasePage(pPage);
			releasePage(pTrunk);
			return rc;
		}
		static void freePage(MemPage pPage,ref int pRC) {
			if((pRC)==SQLITE_OK) {
				pRC=freePage2(pPage.pBt,pPage,pPage.pgno);
			}
		}
		///
		///<summary>
		///Free any overflow pages associated with the given Cell.
		///</summary>
		static int clearCell(MemPage pPage,int pCell) {
			BtShared pBt=pPage.pBt;
			CellInfo info=new CellInfo();
			Pgno ovflPgno;
			int rc;
			int nOvfl;
			u32 ovflPageSize;
			Debug.Assert(sqlite3_mutex_held(pPage.pBt.mutex));
			pPage.btreeParseCellPtr(pCell,ref info);
			if(info.iOverflow==0) {
				return SQLITE_OK;
				///
				///<summary>
				///No overflow pages. Return without doing anything 
				///</summary>
			}
			ovflPgno=Converter.sqlite3Get4byte(pPage.aData,pCell,info.iOverflow);
			Debug.Assert(pBt.usableSize>4);
			ovflPageSize=(u16)(pBt.usableSize-4);
			nOvfl=(int)((info.nPayload-info.nLocal+ovflPageSize-1)/ovflPageSize);
			Debug.Assert(ovflPgno==0||nOvfl>0);
			while(nOvfl--!=0) {
				Pgno iNext=0;
				MemPage pOvfl=null;
				if(ovflPgno<2||ovflPgno>pBt.btreePagecount()) {
					///
					///<summary>
					///0 is not a legal page number and page 1 cannot be an
					///overflow page. Therefore if ovflPgno<2 or past the end of the
					///file the database must be corrupt. 
					///</summary>
					return SQLITE_CORRUPT_BKPT();
				}
				if(nOvfl!=0) {
					rc=getOverflowPage(pBt,ovflPgno,out pOvfl,out iNext);
					if(rc!=0)
						return rc;
				}
				if((pOvfl!=null||((pOvfl=pBt.btreePageLookup(ovflPgno))!=null))&&sqlite3PagerPageRefcount(pOvfl.pDbPage)!=1) {
					///
					///<summary>
					///There is no reason any cursor should have an outstanding reference 
					///to an overflow page belonging to a cell that is being deleted/updated.
					///So if there exists more than one reference to this page, then it 
					///must not really be an overflow page and the database must be corrupt. 
					///It is helpful to detect this before calling freePage2(), as 
					///</summary>
					///<param name="freePage2() may zero the page contents if secure">delete mode is</param>
					///<param name="enabled. If this 'overflow' page happens to be a page that the">enabled. If this 'overflow' page happens to be a page that the</param>
					///<param name="caller is iterating through or using in some other way, this">caller is iterating through or using in some other way, this</param>
					///<param name="can be problematic.">can be problematic.</param>
					///<param name=""></param>
					rc=SQLITE_CORRUPT_BKPT();
				}
				else {
					rc=freePage2(pBt,pOvfl,ovflPgno);
				}
				if(pOvfl!=null) {
					sqlite3PagerUnref(pOvfl.pDbPage);
				}
				if(rc!=0)
					return rc;
				ovflPgno=iNext;
			}
			return SQLITE_OK;
		}
		///
		///<summary>
		///The following parameters determine how many adjacent pages get involved
		///in a balancing operation.  NN is the number of neighbors on either side
		///of the page that participate in the balancing operation.  NB is the
		///total number of pages that participate, including the target page and
		///NN neighbors on either side.
		///
		///The minimum value of NN is 1 (of course).  Increasing NN above 1
		///(to 2 or 3) gives a modest improvement in SELECT and DELETE performance
		///in exchange for a larger degradation in INSERT and UPDATE performance.
		///The value of NN appears to give the best results overall.
		///</summary>
		static int NN=1;
		///
		///<summary>
		///Number of neighbors on either side of pPage 
		///</summary>
		static int NB=(NN*2+1);
		///
		///<summary>
		///Total pages involved in the balance 
		///</summary>
		#if !SQLITE_OMIT_QUICKBALANCE
		#endif
		#if FALSE
																																																/*
** This function does not contribute anything to the operation of SQLite.
** it is sometimes activated temporarily while debugging code responsible
** for setting pointer-map entries.
*/
static int ptrmapCheckPages(MemPage **apPage, int nPage){
int i, j;
for(i=0; i<nPage; i++){
Pgno n;
u8 e;
MemPage pPage = apPage[i];
BtShared pBt = pPage.pBt;
Debug.Assert( pPage.isInit!=0 );

for(j=0; j<pPage.nCell; j++){
CellInfo info;
u8 *z;

z = findCell(pPage, j);
btreeParseCellPtr(pPage, z,  info);
if( info.iOverflow ){
Pgno ovfl = sqlite3Get4byte(z[info.iOverflow]);
ptrmapGet(pBt, ovfl, ref e, ref n);
Debug.Assert( n==pPage.pgno && e==PTRMAP_OVERFLOW1 );
}
if( 0==pPage.leaf ){
Pgno child = sqlite3Get4byte(z);
ptrmapGet(pBt, child, ref e, ref n);
Debug.Assert( n==pPage.pgno && e==PTRMAP_BTREE );
}
}
if( 0==pPage.leaf ){
Pgno child = sqlite3Get4byte(pPage.aData,pPage.hdrOffset+8]);
ptrmapGet(pBt, child, ref e, ref n);
Debug.Assert( n==pPage.pgno && e==PTRMAP_BTREE );
}
}
return 1;
}
#endif
		///
		///<summary>
		///This routine redistributes cells on the iParentIdx'th child of pParent
		///(hereafter "the page") and up to 2 siblings so that all pages have about the
		///same amount of free space. Usually a single sibling on either side of the
		///page are used in the balancing, though both siblings might come from one
		///side if the page is the first or last child of its parent. If the page
		///has fewer than 2 siblings (something which can only happen if the page
		///is a root page or a child of a root page) then all available siblings
		///participate in the balancing.
		///
		///The number of siblings of the page might be increased or decreased by
		///one or two in an effort to keep pages nearly full but not over full.
		///
		///Note that when this routine is called, some of the cells on the page
		///might not actually be stored in MemPage.aData[]. This can happen
		///if the page is overfull. This routine ensures that all cells allocated
		///to the page and its siblings fit into MemPage.aData[] before returning.
		///
		///In the course of balancing the page and its siblings, cells may be
		///inserted into or removed from the parent page (pParent). Doing so
		///may cause the parent page to become overfull or underfull. If this
		///happens, it is the responsibility of the caller to invoke the correct
		///balancing routine to fix this problem (see the balance() routine).
		///
		///If this routine fails for any reason, it might leave the database
		///in a corrupted state. So if this routine fails, the database should
		///be rolled back.
		///
		///The third argument to this function, aOvflSpace, is a pointer to a
		///buffer big enough to hold one page. If while inserting cells into the parent
		///page (pParent) the parent page becomes overfull, this buffer is
		///used to store the parent's overflow cells. Because this function inserts
		///a maximum of four divider cells into the parent page, and the maximum
		///size of a cell stored within an internal node is always less than 1/4
		///</summary>
		///<param name="of the page">size, the aOvflSpace[] buffer is guaranteed to be large</param>
		///<param name="enough for all overflow cells.">enough for all overflow cells.</param>
		///<param name=""></param>
		///<param name="If aOvflSpace is set to a null pointer, this function returns">If aOvflSpace is set to a null pointer, this function returns</param>
		///<param name="SQLITE_NOMEM.">SQLITE_NOMEM.</param>
		///
		///<summary>
		///The page that pCur currently points to has just been modified in
		///some way. This function figures out if this modification means the
		///tree needs to be balanced, and if so calls the appropriate balancing
		///routine. Balancing routines are:
		///
		///balance_quick()
		///balance_deeper()
		///balance_nonroot()
		///</summary>
		static u8[] aBalanceQuickSpace=new u8[13];
		static int balance(BtCursor pCur) {
			int rc=SQLITE_OK;
			int nMin=(int)pCur.pBt.usableSize*2/3;
			//u8[] pFree = null;
			#if !NDEBUG || SQLITE_COVERAGE_TEST || DEBUG
																																																																								  int balance_quick_called = 0;//TESTONLY( int balance_quick_called = 0 );
  int balance_deeper_called = 0;//TESTONLY( int balance_deeper_called = 0 );
#else
			int balance_quick_called=0;
			int balance_deeper_called=0;
			#endif
			do {
				int iPage=pCur.iPage;
				MemPage pPage=pCur.apPage[iPage];
				if(iPage==0) {
					if(pPage.nOverflow!=0) {
						///
						///<summary>
						///</summary>
						///<param name="The root page of the b">tree is overfull. In this case call the</param>
						///<param name="balance_deeper() function to create a new child for the root">page</param>
						///<param name="and copy the current contents of the root">page to it. The</param>
						///<param name="next iteration of the do">loop will balance the child page.</param>
						///<param name=""></param>
						Debug.Assert((balance_deeper_called++)==0);
						rc=pPage.balance_deeper(ref pCur.apPage[1]);
						if(rc==SQLITE_OK) {
							pCur.iPage=1;
							pCur.aiIdx[0]=0;
							pCur.aiIdx[1]=0;
							Debug.Assert(pCur.apPage[1].nOverflow!=0);
						}
					}
					else {
						break;
					}
				}
				else
					if(pPage.nOverflow==0&&pPage.nFree<=nMin) {
						break;
					}
					else {
						MemPage pParent=pCur.apPage[iPage-1];
						int iIdx=pCur.aiIdx[iPage-1];
						rc=sqlite3PagerWrite(pParent.pDbPage);
						if(rc==SQLITE_OK) {
							#if !SQLITE_OMIT_QUICKBALANCE
							if(pPage.hasData!=0&&pPage.nOverflow==1&&pPage.aOvfl[0].idx==pPage.nCell&&pParent.pgno!=1&&pParent.nCell==iIdx) {
								///
								///<summary>
								///Call balance_quick() to create a new sibling of pPage on which
								///to store the overflow cell. balance_quick() inserts a new cell
								///into pParent, which may cause pParent overflow. If this
								///</summary>
								///<param name="happens, the next interation of the do">loop will balance pParent</param>
								///<param name="use either balance_nonroot() or balance_deeper(). Until this">use either balance_nonroot() or balance_deeper(). Until this</param>
								///<param name="happens, the overflow cell is stored in the aBalanceQuickSpace[]">happens, the overflow cell is stored in the aBalanceQuickSpace[]</param>
								///<param name="buffer.">buffer.</param>
								///<param name=""></param>
								///<param name="The purpose of the following Debug.Assert() is to check that only a">The purpose of the following Debug.Assert() is to check that only a</param>
								///<param name="single call to balance_quick() is made for each call to this">single call to balance_quick() is made for each call to this</param>
								///<param name="function. If this were not verified, a subtle bug involving reuse">function. If this were not verified, a subtle bug involving reuse</param>
								///<param name="of the aBalanceQuickSpace[] might sneak in.">of the aBalanceQuickSpace[] might sneak in.</param>
								///<param name=""></param>
								Debug.Assert((balance_quick_called++)==0);
								rc=pParent.balance_quick(pPage,aBalanceQuickSpace);
							}
							else
							#endif
							 {
								///
								///<summary>
								///In this case, call balance_nonroot() to redistribute cells
								///between pPage and up to 2 of its sibling pages. This involves
								///modifying the contents of pParent, which may cause pParent to
								///</summary>
								///<param name="become overfull or underfull. The next iteration of the do">loop</param>
								///<param name="will balance the parent page to correct this.">will balance the parent page to correct this.</param>
								///<param name=""></param>
								///<param name="If the parent page becomes overfull, the overflow cell or cells">If the parent page becomes overfull, the overflow cell or cells</param>
								///<param name="are stored in the pSpace buffer allocated immediately below.">are stored in the pSpace buffer allocated immediately below.</param>
								///<param name="A subsequent iteration of the do">loop will deal with this by</param>
								///<param name="calling balance_nonroot() (balance_deeper() may be called first,">calling balance_nonroot() (balance_deeper() may be called first,</param>
								///<param name="but it doesn't deal with overflow cells "> just moves them to a</param>
								///<param name="different page). Once this subsequent call to balance_nonroot()">different page). Once this subsequent call to balance_nonroot()</param>
								///<param name="has completed, it is safe to release the pSpace buffer used by">has completed, it is safe to release the pSpace buffer used by</param>
								///<param name="the previous call, as the overflow cell data will have been">the previous call, as the overflow cell data will have been</param>
								///<param name="copied either into the body of a database page or into the new">copied either into the body of a database page or into the new</param>
								///<param name="pSpace buffer passed to the latter call to balance_nonroot().">pSpace buffer passed to the latter call to balance_nonroot().</param>
								///<param name=""></param>
								u8[] pSpace=new u8[pCur.pBt.pageSize];
								// u8 pSpace = sqlite3PageMalloc( pCur.pBt.pageSize );
								rc=pParent.balance_nonroot(iIdx,null,iPage==1?1:0);
								//if (pFree != null)
								//{
								//  /* If pFree is not NULL, it points to the pSpace buffer used
								//  ** by a previous call to balance_nonroot(). Its contents are
								//  ** now stored either on real database pages or within the
								//  ** new pSpace buffer, so it may be safely freed here. */
								//  sqlite3PageFree(ref pFree);
								//}
								///
								///<summary>
								///The pSpace buffer will be freed after the next call to
								///balance_nonroot(), or just before this function returns, whichever
								///comes first. 
								///</summary>
								//pFree = pSpace;
							}
						}
						pPage.nOverflow=0;
						///
						///<summary>
						///</summary>
						///<param name="The next iteration of the do">loop balances the parent page. </param>
						releasePage(pPage);
						pCur.iPage--;
					}
			}
			while(rc==SQLITE_OK);
			//if (pFree != null)
			//{
			//  sqlite3PageFree(ref pFree);
			//}
			return rc;
		}
		///
		///<summary>
		///Insert a new record into the BTree.  The key is given by (pKey,nKey)
		///and the data is given by (pData,nData).  The cursor is used only to
		///define what table the record should be inserted into.  The cursor
		///is left pointing at a random location.
		///
		///For an INTKEY table, only the nKey value of the key is used.  pKey is
		///ignored.  For a ZERODATA table, the pData and nData are both ignored.
		///
		///</summary>
		///<param name="If the seekResult parameter is non">zero, then a successful call to</param>
		///<param name="MovetoUnpacked() to seek cursor pCur to (pKey, nKey) has already">MovetoUnpacked() to seek cursor pCur to (pKey, nKey) has already</param>
		///<param name="been performed. seekResult is the search result returned (a negative">been performed. seekResult is the search result returned (a negative</param>
		///<param name="number if pCur points at an entry that is smaller than (pKey, nKey), or">number if pCur points at an entry that is smaller than (pKey, nKey), or</param>
		///<param name="a positive value if pCur points at an etry that is larger than">a positive value if pCur points at an etry that is larger than</param>
		///<param name="(pKey, nKey)).">(pKey, nKey)).</param>
		///<param name=""></param>
		///<param name="If the seekResult parameter is non">zero, then the caller guarantees that</param>
		///<param name="cursor pCur is pointing at the existing copy of a row that is to be">cursor pCur is pointing at the existing copy of a row that is to be</param>
		///<param name="overwritten.  If the seekResult parameter is 0, then cursor pCur may">overwritten.  If the seekResult parameter is 0, then cursor pCur may</param>
		///<param name="point to any entry or to no entry at all and so this function has to seek">point to any entry or to no entry at all and so this function has to seek</param>
		///<param name="the cursor before the new key can be inserted.">the cursor before the new key can be inserted.</param>
		static int sqlite3BtreeInsert(BtCursor pCur,///
		///<summary>
		///Insert data into the table of this cursor 
		///</summary>
		byte[] pKey,i64 nKey,///
		///<summary>
		///The key of the new record 
		///</summary>
		byte[] pData,int nData,///
		///<summary>
		///The data of the new record 
		///</summary>
		int nZero,///
		///<summary>
		///Number of extra 0 bytes to append to data 
		///</summary>
		int appendBias,///
		///<summary>
		///True if this is likely an append 
		///</summary>
		int seekResult///
		///<summary>
		///Result of prior MovetoUnpacked() call 
		///</summary>
		) {
			int rc;
			int loc=seekResult;
			///
			///<summary>
			///</summary>
			///<param name="">1: before desired location  +1: after </param>
			int szNew=0;
			int idx;
			MemPage pPage;
			Btree p=pCur.pBtree;
			BtShared pBt=p.pBt;
			int oldCell;
			byte[] newCell=null;
			if(pCur.eState==CURSOR_FAULT) {
				Debug.Assert(pCur.skipNext!=SQLITE_OK);
				return pCur.skipNext;
			}
			Debug.Assert(pCur.cursorHoldsMutex());
			Debug.Assert(pCur.wrFlag!=0&&pBt.inTransaction==TRANS_WRITE&&!pBt.readOnly);
			Debug.Assert(p.hasSharedCacheTableLock(pCur.pgnoRoot,pCur.pKeyInfo!=null?1:0,2));
			///
			///<summary>
			///Assert that the caller has been consistent. If this cursor was opened
			///</summary>
			///<param name="expecting an index b">tree, then the caller should be inserting blob</param>
			///<param name="keys with no associated data. If the cursor was opened expecting an">keys with no associated data. If the cursor was opened expecting an</param>
			///<param name="intkey table, the caller should be inserting integer keys with a">intkey table, the caller should be inserting integer keys with a</param>
			///<param name="blob of associated data.  ">blob of associated data.  </param>
			Debug.Assert((pKey==null)==(pCur.pKeyInfo==null));
			///
			///<summary>
			///</summary>
			///<param name="If this is an insert into a table b">tree, invalidate any incrblob</param>
			///<param name="cursors open on the row being replaced (assuming this is a replace">cursors open on the row being replaced (assuming this is a replace</param>
			///<param name="operation ">op).  </param>
			if(pCur.pKeyInfo==null) {
				p.invalidateIncrblobCursors(nKey,0);
			}
			///
			///<summary>
			///Save the positions of any other cursors open on this table.
			///
			///</summary>
			///<param name="In some cases, the call to btreeMoveto() below is a no">op. For</param>
			///<param name="example, when inserting data into a table with auto">generated integer</param>
			///<param name="keys, the VDBE layer invokes sqlite3BtreeLast() to figure out the">keys, the VDBE layer invokes sqlite3BtreeLast() to figure out the</param>
			///<param name="integer key to use. It then calls this function to actually insert the">integer key to use. It then calls this function to actually insert the</param>
			///<param name="data into the intkey B">Tree. In this case btreeMoveto() recognizes</param>
			///<param name="that the cursor is already where it needs to be and returns without">that the cursor is already where it needs to be and returns without</param>
			///<param name="doing any work. To avoid thwarting these optimizations, it is important">doing any work. To avoid thwarting these optimizations, it is important</param>
			///<param name="not to clear the cursor here.">not to clear the cursor here.</param>
			///<param name=""></param>
			rc=pBt.saveAllCursors(pCur.pgnoRoot,pCur);
			if(rc!=0)
				return rc;
			if(0==loc) {
				rc=pCur.btreeMoveto(pKey,nKey,appendBias,ref loc);
				if(rc!=0)
					return rc;
			}
			Debug.Assert(pCur.eState==CURSOR_VALID||(pCur.eState==CURSOR_INVALID&&loc!=0));
			pPage=pCur.apPage[pCur.iPage];
			Debug.Assert(pPage.intKey!=0||nKey>=0);
			Debug.Assert(pPage.leaf!=0||0==pPage.intKey);
			TRACE("INSERT: table=%d nkey=%lld ndata=%d page=%d %s\n",pCur.pgnoRoot,nKey,nData,pPage.pgno,loc==0?"overwrite":"new entry");
			Debug.Assert(pPage.isInit!=0);
			allocateTempSpace(pBt);
			newCell=pBt.pTmpSpace;
			//if (newCell == null) return SQLITE_NOMEM;
			rc=pPage.fillInCell(newCell,pKey,nKey,pData,nData,nZero,ref szNew);
			if(rc!=0)
				goto end_insert;
			Debug.Assert(szNew==pPage.cellSizePtr(newCell));
			Debug.Assert(szNew<=MX_CELL_SIZE(pBt));
			idx=pCur.aiIdx[pCur.iPage];
			if(loc==0) {
				u16 szOld;
				Debug.Assert(idx<pPage.nCell);
				rc=sqlite3PagerWrite(pPage.pDbPage);
				if(rc!=0) {
					goto end_insert;
				}
				oldCell=pPage.findCell(idx);
				if(0==pPage.leaf) {
					//memcpy(newCell, oldCell, 4);
					newCell[0]=pPage.aData[oldCell+0];
					newCell[1]=pPage.aData[oldCell+1];
					newCell[2]=pPage.aData[oldCell+2];
					newCell[3]=pPage.aData[oldCell+3];
				}
				szOld=pPage.cellSizePtr(oldCell);
				rc=clearCell(pPage,oldCell);
				pPage.dropCell(idx,szOld,ref rc);
				if(rc!=0)
					goto end_insert;
			}
			else
				if(loc<0&&pPage.nCell>0) {
					Debug.Assert(pPage.leaf!=0);
					idx=++pCur.aiIdx[pCur.iPage];
				}
				else {
					Debug.Assert(pPage.leaf!=0);
				}
			pPage.insertCell(idx,newCell,szNew,null,0,ref rc);
			Debug.Assert(rc!=SQLITE_OK||pPage.nCell>0||pPage.nOverflow>0);
			///
			///<summary>
			///If no error has occured and pPage has an overflow cell, call balance()
			///to redistribute the cells within the tree. Since balance() may move
			///the cursor, zero the BtCursor.info.nSize and BtCursor.validNKey
			///variables.
			///
			///Previous versions of SQLite called moveToRoot() to move the cursor
			///back to the root page as balance() used to invalidate the contents
			///of BtCursor.apPage[] and BtCursor.aiIdx[]. Instead of doing that,
			///set the cursor state to "invalid". This makes common insert operations
			///slightly faster.
			///
			///There is a subtle but important optimization here too. When inserting
			///</summary>
			///<param name="multiple records into an intkey b">tree using a single cursor (as can</param>
			///<param name="happen while processing an "INSERT INTO ... SELECT" statement), it">happen while processing an "INSERT INTO ... SELECT" statement), it</param>
			///<param name="is advantageous to leave the cursor pointing to the last entry in">is advantageous to leave the cursor pointing to the last entry in</param>
			///<param name="the b">tree if possible. If the cursor is left pointing to the last</param>
			///<param name="entry in the table, and the next row inserted has an integer key">entry in the table, and the next row inserted has an integer key</param>
			///<param name="larger than the largest existing key, it is possible to insert the">larger than the largest existing key, it is possible to insert the</param>
			///<param name="row without seeking the cursor. This can be a big performance boost.">row without seeking the cursor. This can be a big performance boost.</param>
			///<param name=""></param>
			pCur.info.nSize=0;
			pCur.validNKey=false;
			if(rc==SQLITE_OK&&pPage.nOverflow!=0) {
				rc=balance(pCur);
				///
				///<summary>
				///Must make sure nOverflow is reset to zero even if the balance()
				///fails. Internal data structure corruption will result otherwise.
				///Also, set the cursor state to invalid. This stops saveCursorPosition()
				///from trying to save the current position of the cursor.  
				///</summary>
				pCur.apPage[pCur.iPage].nOverflow=0;
				pCur.eState=CURSOR_INVALID;
			}
			Debug.Assert(pCur.apPage[pCur.iPage].nOverflow==0);
			end_insert:
			return rc;
		}
		///
		///<summary>
		///Delete the entry that the cursor is pointing to.  The cursor
		///is left pointing at a arbitrary location.
		///</summary>
		static int sqlite3BtreeDelete(BtCursor pCur) {
			Btree p=pCur.pBtree;
			BtShared pBt=p.pBt;
			int rc;
			///
			///<summary>
			///Return code 
			///</summary>
			MemPage pPage;
			///
			///<summary>
			///Page to delete cell from 
			///</summary>
			int pCell;
			///
			///<summary>
			///Pointer to cell to delete 
			///</summary>
			int iCellIdx;
			///
			///<summary>
			///Index of cell to delete 
			///</summary>
			int iCellDepth;
			///
			///<summary>
			///Depth of node containing pCell 
			///</summary>
			Debug.Assert(pCur.cursorHoldsMutex());
			Debug.Assert(pBt.inTransaction==TRANS_WRITE);
			Debug.Assert(!pBt.readOnly);
			Debug.Assert(pCur.wrFlag!=0);
			Debug.Assert(p.hasSharedCacheTableLock(pCur.pgnoRoot,pCur.pKeyInfo!=null?1:0,2));
			Debug.Assert(!p.hasReadConflicts(pCur.pgnoRoot));
			if(NEVER(pCur.aiIdx[pCur.iPage]>=pCur.apPage[pCur.iPage].nCell)||NEVER(pCur.eState!=CURSOR_VALID)) {
				return SQLITE_ERROR;
				///
				///<summary>
				///Something has gone awry. 
				///</summary>
			}
			///
			///<summary>
			///</summary>
			///<param name="If this is a delete operation to remove a row from a table b">tree,</param>
			///<param name="invalidate any incrblob cursors open on the row being deleted.  ">invalidate any incrblob cursors open on the row being deleted.  </param>
			if(pCur.pKeyInfo==null) {
				p.invalidateIncrblobCursors(pCur.info.nKey,0);
			}
			iCellDepth=pCur.iPage;
			iCellIdx=pCur.aiIdx[iCellDepth];
			pPage=pCur.apPage[iCellDepth];
			pCell=pPage.findCell(iCellIdx);
			///
			///<summary>
			///If the page containing the entry to delete is not a leaf page, move
			///the cursor to the largest entry in the tree that is smaller than
			///the entry being deleted. This cell will replace the cell being deleted
			///from the internal node. The 'previous' entry is used for this instead
			///of the 'next' entry, as the previous entry is always a part of the
			///</summary>
			///<param name="sub">tree headed by the child page of the cell being deleted. This makes</param>
			///<param name="balancing the tree following the delete operation easier.  ">balancing the tree following the delete operation easier.  </param>
			if(0==pPage.leaf) {
				int notUsed=0;
				rc=sqlite3BtreePrevious(pCur,ref notUsed);
				if(rc!=0)
					return rc;
			}
			///
			///<summary>
			///Save the positions of any other cursors open on this table before
			///making any modifications. Make the page containing the entry to be
			///deleted writable. Then free any overflow pages associated with the
			///entry and finally remove the cell itself from within the page.
			///
			///</summary>
			rc=pBt.saveAllCursors(pCur.pgnoRoot,pCur);
			if(rc!=0)
				return rc;
			rc=sqlite3PagerWrite(pPage.pDbPage);
			if(rc!=0)
				return rc;
			rc=clearCell(pPage,pCell);
			pPage.dropCell(iCellIdx,pPage.cellSizePtr(pCell),ref rc);
			if(rc!=0)
				return rc;
			///
			///<summary>
			///If the cell deleted was not located on a leaf page, then the cursor
			///</summary>
			///<param name="is currently pointing to the largest entry in the sub">tree headed</param>
			///<param name="by the child">page of the cell that was just deleted from an internal</param>
			///<param name="node. The cell from the leaf node needs to be moved to the internal">node. The cell from the leaf node needs to be moved to the internal</param>
			///<param name="node to replace the deleted cell.  ">node to replace the deleted cell.  </param>
			if(0==pPage.leaf) {
				MemPage pLeaf=pCur.apPage[pCur.iPage];
				int nCell;
				Pgno n=pCur.apPage[iCellDepth+1].pgno;
				//byte[] pTmp;
				pCell=pLeaf.findCell(pLeaf.nCell-1);
				nCell=pLeaf.cellSizePtr(pCell);
				Debug.Assert(MX_CELL_SIZE(pBt)>=nCell);
				//allocateTempSpace(pBt);
				//pTmp = pBt.pTmpSpace;
				rc=sqlite3PagerWrite(pLeaf.pDbPage);
				byte[] pNext_4=sqlite3Malloc(nCell+4);
				Buffer.BlockCopy(pLeaf.aData,pCell-4,pNext_4,0,nCell+4);
				pPage.insertCell(iCellIdx,pNext_4,nCell+4,null,n,ref rc);
				//insertCell( pPage, iCellIdx, pCell - 4, nCell + 4, pTmp, n, ref rc );
				pLeaf.dropCell(pLeaf.nCell-1,nCell,ref rc);
				if(rc!=0)
					return rc;
			}
			///
			///<summary>
			///Balance the tree. If the entry deleted was located on a leaf page,
			///then the cursor still points to that page. In this case the first
			///call to balance() repairs the tree, and the if(...) condition is
			///never true.
			///
			///Otherwise, if the entry deleted was on an internal node page, then
			///pCur is pointing to the leaf page from which a cell was removed to
			///replace the cell deleted from the internal node. This is slightly
			///tricky as the leaf node may be underfull, and the internal node may
			///be either under or overfull. In this case run the balancing algorithm
			///on the leaf node first. If the balance proceeds far enough up the
			///tree that we can be sure that any problem in the internal node has
			///been corrected, so be it. Otherwise, after balancing the leaf node,
			///walk the cursor up the tree to the internal node and balance it as
			///well.  
			///</summary>
			rc=balance(pCur);
			if(rc==SQLITE_OK&&pCur.iPage>iCellDepth) {
				while(pCur.iPage>iCellDepth) {
					releasePage(pCur.apPage[pCur.iPage--]);
				}
				rc=balance(pCur);
			}
			if(rc==SQLITE_OK) {
				moveToRoot(pCur);
			}
			return rc;
		}
		///
		///<summary>
		///Create a new BTree table.  Write into piTable the page
		///number for the root page of the new table.
		///
		///The type of type is determined by the flags parameter.  Only the
		///following values of flags are currently in use.  Other values for
		///flags might not work:
		///
		///BTREE_INTKEY|BTREE_LEAFDATA     Used for SQL tables with rowid keys
		///BTREE_ZERODATA                  Used for SQL indices
		///</summary>
		static int btreeCreateTable(Btree p,ref int piTable,int createTabFlags) {
			BtShared pBt=p.pBt;
			MemPage pRoot=new MemPage();
			Pgno pgnoRoot=0;
			int rc;
			int ptfFlags;
			///
			///<summary>
			///</summary>
			///<param name="Page">type flage for the root page of new table </param>
			Debug.Assert(sqlite3BtreeHoldsMutex(p));
			Debug.Assert(pBt.inTransaction==TRANS_WRITE);
			Debug.Assert(!pBt.readOnly);
			#if SQLITE_OMIT_AUTOVACUUM
																																																																								rc = allocateBtreePage(pBt, ref pRoot, ref pgnoRoot, 1, 0);
if( rc !=0){
return rc;
}
#else
			if(pBt.autoVacuum) {
				Pgno pgnoMove=0;
				///
				///<summary>
				///</summary>
				///<param name="Move a page here to make room for the root">page </param>
				MemPage pPageMove=new MemPage();
				///
				///<summary>
				///The page to move to. 
				///</summary>
				///
				///<summary>
				///Creating a new table may probably require moving an existing database
				///to make room for the new tables root page. In case this page turns
				///</summary>
				///<param name="out to be an overflow page, delete all overflow page">map caches</param>
				///<param name="held by open cursors.">held by open cursors.</param>
				///<param name=""></param>
				pBt.invalidateAllOverflowCache();
				///
				///<summary>
				///Read the value of meta[3] from the database to determine where the
				///</summary>
				///<param name="root page of the new table should go. meta[3] is the largest root">page</param>
				///<param name="created so far, so the new root">page is (meta[3]+1).</param>
				///<param name=""></param>
				pgnoRoot=p.sqlite3BtreeGetMeta(BTREE_LARGEST_ROOT_PAGE);
				pgnoRoot++;
				///
				///<summary>
				///</summary>
				///<param name="The new root">map page, or the</param>
				///<param name="PENDING_BYTE page.">PENDING_BYTE page.</param>
				///<param name=""></param>
				while(pgnoRoot==PTRMAP_PAGENO(pBt,pgnoRoot)||pgnoRoot==PENDING_BYTE_PAGE(pBt)) {
					pgnoRoot++;
				}
				Debug.Assert(pgnoRoot>=3);
				///
				///<summary>
				///Allocate a page. The page that currently resides at pgnoRoot will
				///be moved to the allocated page (unless the allocated page happens
				///to reside at pgnoRoot).
				///
				///</summary>
				rc=allocateBtreePage(pBt,ref pPageMove,ref pgnoMove,pgnoRoot,1);
				if(rc!=SQLITE_OK) {
					return rc;
				}
				if(pgnoMove!=pgnoRoot) {
					///
					///<summary>
					///</summary>
					///<param name="pgnoRoot is the page that will be used for the root">page of</param>
					///<param name="the new table (assuming an error did not occur). But we were">the new table (assuming an error did not occur). But we were</param>
					///<param name="allocated pgnoMove. If required (i.e. if it was not allocated">allocated pgnoMove. If required (i.e. if it was not allocated</param>
					///<param name="by extending the file), the current page at position pgnoMove">by extending the file), the current page at position pgnoMove</param>
					///<param name="is already journaled.">is already journaled.</param>
					///<param name=""></param>
					u8 eType=0;
					Pgno iPtrPage=0;
					releasePage(pPageMove);
					///
					///<summary>
					///Move the page currently at pgnoRoot to pgnoMove. 
					///</summary>
					rc=pBt.btreeGetPage(pgnoRoot,ref pRoot,0);
					if(rc!=SQLITE_OK) {
						return rc;
					}
					rc=pBt.ptrmapGet(pgnoRoot,ref eType,ref iPtrPage);
					if(eType==PTRMAP_ROOTPAGE||eType==PTRMAP_FREEPAGE) {
						rc=SQLITE_CORRUPT_BKPT();
					}
					if(rc!=SQLITE_OK) {
						releasePage(pRoot);
						return rc;
					}
					Debug.Assert(eType!=PTRMAP_ROOTPAGE);
					Debug.Assert(eType!=PTRMAP_FREEPAGE);
					rc=relocatePage(pBt,pRoot,eType,iPtrPage,pgnoMove,0);
					releasePage(pRoot);
					///
					///<summary>
					///Obtain the page at pgnoRoot 
					///</summary>
					if(rc!=SQLITE_OK) {
						return rc;
					}
					rc=pBt.btreeGetPage(pgnoRoot,ref pRoot,0);
					if(rc!=SQLITE_OK) {
						return rc;
					}
					rc=sqlite3PagerWrite(pRoot.pDbPage);
					if(rc!=SQLITE_OK) {
						releasePage(pRoot);
						return rc;
					}
				}
				else {
					pRoot=pPageMove;
				}
				///
				///<summary>
				///</summary>
				///<param name="Update the pointer">page number. </param>
				pBt.ptrmapPut(pgnoRoot,PTRMAP_ROOTPAGE,0,ref rc);
				if(rc!=0) {
					releasePage(pRoot);
					return rc;
				}
				///
				///<summary>
				///When the new root page was allocated, page 1 was made writable in
				///order either to increase the database filesize, or to decrement the
				///freelist count.  Hence, the sqlite3BtreeUpdateMeta() call cannot fail.
				///
				///</summary>
				Debug.Assert(sqlite3PagerIswriteable(pBt.pPage1.pDbPage));
				rc=p.sqlite3BtreeUpdateMeta(4,pgnoRoot);
				if(NEVER(rc!=0)) {
					releasePage(pRoot);
					return rc;
				}
			}
			else {
				rc=allocateBtreePage(pBt,ref pRoot,ref pgnoRoot,1,0);
				if(rc!=0)
					return rc;
			}
			#endif
			Debug.Assert(sqlite3PagerIswriteable(pRoot.pDbPage));
			if((createTabFlags&BTREE_INTKEY)!=0) {
				ptfFlags=PTF_INTKEY|PTF_LEAFDATA|PTF_LEAF;
			}
			else {
				ptfFlags=PTF_ZERODATA|PTF_LEAF;
			}
			pRoot.zeroPage(ptfFlags);
			sqlite3PagerUnref(pRoot.pDbPage);
			Debug.Assert((pBt.openFlags&BTREE_SINGLE)==0||pgnoRoot==2);
			piTable=(int)pgnoRoot;
			return SQLITE_OK;
		}
		static int sqlite3BtreeCreateTable(Btree p,ref int piTable,int flags) {
			int rc;
			sqlite3BtreeEnter(p);
			rc=btreeCreateTable(p,ref piTable,flags);
			sqlite3BtreeLeave(p);
			return rc;
		}
		///
		///<summary>
		///Erase the given database page and all its children.  Return
		///the page to the freelist.
		///</summary>
		static int clearDatabasePage(BtShared pBt,///
		///<summary>
		///The BTree that contains the table 
		///</summary>
		Pgno pgno,///
		///<summary>
		///Page number to clear 
		///</summary>
		int freePageFlag,///
		///<summary>
		///Deallocate page if true 
		///</summary>
		ref int pnChange///
		///<summary>
		///Add number of Cells freed to this counter 
		///</summary>
		) {
			MemPage pPage=new MemPage();
			int rc;
			byte[] pCell;
			int i;
			Debug.Assert(sqlite3_mutex_held(pBt.mutex));
			if(pgno>pBt.btreePagecount()) {
				return SQLITE_CORRUPT_BKPT();
			}
			rc=getAndInitPage(pBt,pgno,ref pPage);
			if(rc!=0)
				return rc;
			for(i=0;i<pPage.nCell;i++) {
				int iCell=pPage.findCell(i);
				pCell=pPage.aData;
				//        pCell = findCell( pPage, i );
				if(0==pPage.leaf) {
					rc=clearDatabasePage(pBt,Converter.sqlite3Get4byte(pCell,iCell),1,ref pnChange);
					if(rc!=0)
						goto cleardatabasepage_out;
				}
				rc=clearCell(pPage,iCell);
				if(rc!=0)
					goto cleardatabasepage_out;
			}
			if(0==pPage.leaf) {
				rc=clearDatabasePage(pBt,Converter.sqlite3Get4byte(pPage.aData,8),1,ref pnChange);
				if(rc!=0)
					goto cleardatabasepage_out;
			}
			else//if (pnChange != 0)
			 {
				//Debug.Assert(pPage.intKey != 0);
				pnChange+=pPage.nCell;
			}
			if(freePageFlag!=0) {
				freePage(pPage,ref rc);
			}
			else
				if((rc=sqlite3PagerWrite(pPage.pDbPage))==0) {
					pPage.zeroPage(pPage.aData[0]|PTF_LEAF);
				}
			cleardatabasepage_out:
			releasePage(pPage);
			return rc;
		}
		///
		///<summary>
		///Delete all information from a single table in the database.  iTable is
		///the page number of the root of the table.  After this routine returns,
		///the root page is empty, but still exists.
		///
		///This routine will fail with SQLITE_LOCKED if there are any open
		///read cursors on the table.  Open write cursors are moved to the
		///root of the table.
		///
		///If pnChange is not NULL, then table iTable must be an intkey table. The
		///integer value pointed to by pnChange is incremented by the number of
		///entries in the table.
		///</summary>
		///
		///<summary>
		///Erase all information in a table and add the root of the table to
		///the freelist.  Except, the root of the principle table (the one on
		///page 1) is never added to the freelist.
		///
		///This routine will fail with SQLITE_LOCKED if there are any open
		///cursors on the table.
		///
		///If AUTOVACUUM is enabled and the page at iTable is not the last
		///root page in the database file, then the last root page
		///in the database file is moved into the slot formerly occupied by
		///iTable and that last slot formerly occupied by the last root page
		///is added to the freelist instead of iTable.  In this say, all
		///root pages are kept at the beginning of the database file, which
		///is necessary for AUTOVACUUM to work right.  piMoved is set to the
		///page number that used to be the last root page in the file before
		///the move.  If no page gets moved, piMoved is set to 0.
		///The last root page is recorded in meta[3] and the value of
		///meta[3] is updated by this procedure.
		///</summary>
		///
		///<summary>
		///</summary>
		///<param name="This function may only be called if the b">tree connection already</param>
		///<param name="has a read or write transaction open on the database.">has a read or write transaction open on the database.</param>
		///<param name=""></param>
		///<param name="Read the meta">information out of a database file.  Meta[0]</param>
		///<param name="is the number of free pages currently in the database.  Meta[1]">is the number of free pages currently in the database.  Meta[1]</param>
		///<param name="through meta[15] are available for use by higher layers.  Meta[0]">through meta[15] are available for use by higher layers.  Meta[0]</param>
		///<param name="is read">only, the others are read/write.</param>
		///<param name=""></param>
		///<param name="The schema layer numbers meta values differently.  At the schema">The schema layer numbers meta values differently.  At the schema</param>
		///<param name="layer (and the SetCookie and ReadCookie opcodes) the number of">layer (and the SetCookie and ReadCookie opcodes) the number of</param>
		///<param name="free pages is not visible.  So Cookie[0] is the same as Meta[1].">free pages is not visible.  So Cookie[0] is the same as Meta[1].</param>
		///
		///<summary>
		///</summary>
		///<param name="Write meta">information back into the database.  Meta[0] is</param>
		///<param name="read">only and may not be written.</param>
		#if !SQLITE_OMIT_BTREECOUNT
		///
		///<summary>
		///</summary>
		///<param name="The first argument, pCur, is a cursor opened on some b">tree. Count the</param>
		///<param name="number of entries in the b">tree and write the result to pnEntry.</param>
		///<param name=""></param>
		///<param name="SQLITE_OK is returned if the operation is successfully executed.">SQLITE_OK is returned if the operation is successfully executed.</param>
		///<param name="Otherwise, if an error is encountered (i.e. an IO error or database">Otherwise, if an error is encountered (i.e. an IO error or database</param>
		///<param name="corruption) an SQLite error code is returned.">corruption) an SQLite error code is returned.</param>
		static int sqlite3BtreeCount(BtCursor pCur,ref i64 pnEntry) {
			i64 nEntry=0;
			///
			///<summary>
			///Value to return in pnEntry 
			///</summary>
			int rc;
			///
			///<summary>
			///Return code 
			///</summary>
			rc=moveToRoot(pCur);
			///
			///<summary>
			///Unless an error occurs, the following loop runs one iteration for each
			///</summary>
			///<param name="page in the B">Tree structure (not including overflow pages).</param>
			///<param name=""></param>
			while(rc==SQLITE_OK) {
				int iIdx;
				///
				///<summary>
				///Index of child node in parent 
				///</summary>
				MemPage pPage;
				///
				///<summary>
				///</summary>
				///<param name="Current page of the b">tree </param>
				///
				///<summary>
				///</summary>
				///<param name="If this is a leaf page or the tree is not an int">key tree, then</param>
				///<param name="this page contains countable entries. Increment the entry counter">this page contains countable entries. Increment the entry counter</param>
				///<param name="accordingly.">accordingly.</param>
				///<param name=""></param>
				pPage=pCur.apPage[pCur.iPage];
				if(pPage.leaf!=0||0==pPage.intKey) {
					nEntry+=pPage.nCell;
				}
				///
				///<summary>
				///pPage is a leaf node. This loop navigates the cursor so that it
				///points to the first interior cell that it points to the parent of
				///the next page in the tree that has not yet been visited. The
				///pCur.aiIdx[pCur.iPage] value is set to the index of the parent cell
				///of the page, or to the number of cells in the page if the next page
				///</summary>
				///<param name="to visit is the right">child of its parent.</param>
				///<param name=""></param>
				///<param name="If all pages in the tree have been visited, return SQLITE_OK to the">If all pages in the tree have been visited, return SQLITE_OK to the</param>
				///<param name="caller.">caller.</param>
				///<param name=""></param>
				if(pPage.leaf!=0) {
					do {
						if(pCur.iPage==0) {
							///
							///<summary>
							///</summary>
							///<param name="All pages of the b">tree have been visited. Return successfully. </param>
							pnEntry=nEntry;
							return SQLITE_OK;
						}
						moveToParent(pCur);
					}
					while(pCur.aiIdx[pCur.iPage]>=pCur.apPage[pCur.iPage].nCell);
					pCur.aiIdx[pCur.iPage]++;
					pPage=pCur.apPage[pCur.iPage];
				}
				///
				///<summary>
				///Descend to the child node of the cell that the cursor currently
				///</summary>
				///<param name="points at. This is the right">child if (iIdx==pPage.nCell).</param>
				///<param name=""></param>
				iIdx=pCur.aiIdx[pCur.iPage];
				if(iIdx==pPage.nCell) {
					rc=moveToChild(pCur,Converter.sqlite3Get4byte(pPage.aData,pPage.hdrOffset+8));
				}
				else {
					rc=moveToChild(pCur,Converter.sqlite3Get4byte(pPage.aData,pPage.findCell(iIdx)));
				}
			}
			///
			///<summary>
			///An error has occurred. Return an error code. 
			///</summary>
			return rc;
		}
		#endif
		///
		///<summary>
		///Return the pager associated with a BTree.  This routine is used for
		///testing and debugging only.
		///</summary>
		static Pager sqlite3BtreePager(Btree p) {
			return p.pBt.pPager;
		}
		#if !SQLITE_OMIT_INTEGRITY_CHECK
		///
		///<summary>
		///Append a message to the error message string.
		///</summary>
		#endif
		#if !SQLITE_OMIT_INTEGRITY_CHECK
		///
		///<summary>
		///Add 1 to the reference count for page iPage.  If this is the second
		///reference to the page, add an error message to pCheck.zErrMsg.
		///Return 1 if there are 2 ore more references to the page and 0 if
		///if this is the first reference to the page.
		///
		///Also check that the page number is in bounds.
		///</summary>
		#if !SQLITE_OMIT_AUTOVACUUM
		///
		///<summary>
		///</summary>
		///<param name="Check that the entry in the pointer">map for page iChild maps to</param>
		///<param name="page iParent, pointer type ptrType. If not, append an error message">page iParent, pointer type ptrType. If not, append an error message</param>
		///<param name="to pCheck.">to pCheck.</param>
		#endif
		///
		///<summary>
		///Check the integrity of the freelist or of an overflow page list.
		///Verify that the number of pages on the list is N.
		///</summary>
		#endif
		#if !SQLITE_OMIT_INTEGRITY_CHECK
		///
		///<summary>
		///Do various sanity checks on a single page of a tree.  Return
		///the tree depth.  Root pages return 0.  Parents of root pages
		///return 1, and so forth.
		///
		///These checks are done:
		///
		///1.  Make sure that cells and freeblocks do not overlap
		///but combine to completely cover the page.
		///NO  2.  Make sure cell keys are in order.
		///NO  3.  Make sure no key is less than or equal to zLowerBound.
		///NO  4.  Make sure no key is greater than or equal to zUpperBound.
		///5.  Check the integrity of overflow pages.
		///6.  Recursively call checkTreePage on all children.
		///7.  Verify that the depth of all children is the same.
		///8.  Make sure this page is at least 33% full or else it is
		///the root of the tree.
		///</summary>
		static i64 refNULL=0;
	//Dummy for C# ref NULL
	#endif
	#if !SQLITE_OMIT_INTEGRITY_CHECK
	///
	///<summary>
	///This routine does a complete check of the given BTree file.  aRoot[] is
	///an array of pages numbers were each page number is the root page of
	///a table.  nRoot is the number of entries in aRoot.
	///
	///</summary>
	///<param name="A read">write transaction must be opened before calling</param>
	///<param name="this function.">this function.</param>
	///<param name=""></param>
	///<param name="Write the number of error seen in pnErr.  Except for some memory">Write the number of error seen in pnErr.  Except for some memory</param>
	///<param name="allocation errors,  an error message held in memory obtained from">allocation errors,  an error message held in memory obtained from</param>
	///<param name="malloc is returned if pnErr is non">zero.  If pnErr==null then NULL is</param>
	///<param name="returned.  If a memory allocation error occurs, NULL is returned.">returned.  If a memory allocation error occurs, NULL is returned.</param>
	#endif
	///
	///<summary>
	///Return the full pathname of the underlying database file.
	///
	///The pager filename is invariant as long as the pager is
	///open so it is safe to access without the BtShared mutex.
	///</summary>
	///
	///<summary>
	///Return the pathname of the journal file for this database. The return
	///value of this routine is the same regardless of whether the journal file
	///has been created or not.
	///
	///The pager journal filename is invariant as long as the pager is
	///open so it is safe to access without the BtShared mutex.
	///</summary>
	///
	///<summary>
	///</summary>
	///<param name="Return non">zero if a transaction is active.</param>
	#if !SQLITE_OMIT_WAL
																																						/*
** Run a checkpoint on the Btree passed as the first argument.
**
** Return SQLITE_LOCKED if this or any other connection has an open 
** transaction on the shared-cache the argument Btree is connected to.
**
** Parameter eMode is one of SQLITE_CHECKPOINT_PASSIVE, FULL or RESTART.
*/
static int sqlite3BtreeCheckpointBtree *p, int eMode, int *pnLog, int *pnCkpt){
int rc = SQLITE_OK;
if( p != null){
BtShared pBt = p.pBt;
sqlite3BtreeEnter(p);
if( pBt.inTransaction!=TRANS_NONE ){
rc = SQLITE_LOCKED;
}else{
rc = sqlite3PagerCheckpoint(pBt.pPager, eMode, pnLog, pnCkpt);
}
sqlite3BtreeLeave(p);
}
return rc;
}
#endif
	///
	///<summary>
	///</summary>
	///<param name="Return non">zero if a read (or write) transaction is active.</param>
	///
	///<summary>
	///This function returns a pointer to a blob of memory associated with
	///</summary>
	///<param name="a single shared">btree. The memory is used by client code for its own</param>
	///<param name="purposes (for example, to store a high">level schema associated with</param>
	///<param name="the shared">btree). The btree layer manages reference counting issues.</param>
	///<param name=""></param>
	///<param name="The first time this is called on a shared">btree, nBytes bytes of memory</param>
	///<param name="are allocated, zeroed, and returned to the caller. For each subsequent">are allocated, zeroed, and returned to the caller. For each subsequent</param>
	///<param name="call the nBytes parameter is ignored and a pointer to the same blob">call the nBytes parameter is ignored and a pointer to the same blob</param>
	///<param name="of memory returned.">of memory returned.</param>
	///<param name=""></param>
	///<param name="If the nBytes parameter is 0 and the blob of memory has not yet been">If the nBytes parameter is 0 and the blob of memory has not yet been</param>
	///<param name="allocated, a null pointer is returned. If the blob has already been">allocated, a null pointer is returned. If the blob has already been</param>
	///<param name="allocated, it is returned as normal.">allocated, it is returned as normal.</param>
	///<param name=""></param>
	///<param name="Just before the shared">btree is closed, the function passed as the </param>
	///<param name="xFree argument when the memory allocation was made is invoked on the ">xFree argument when the memory allocation was made is invoked on the </param>
	///<param name="blob of allocated memory. The xFree function should not call sqlite3_free()">blob of allocated memory. The xFree function should not call sqlite3_free()</param>
	///<param name="on the memory, the btree layer does that.">on the memory, the btree layer does that.</param>
	///
	///<summary>
	///Return SQLITE_LOCKED_SHAREDCACHE if another user of the same shared
	///btree as the argument handle holds an exclusive lock on the
	///sqlite_master table. Otherwise SQLITE_OK.
	///</summary>
	#if !SQLITE_OMIT_SHARED_CACHE
																																						/*
** Obtain a lock on the table whose root page is iTab.  The
** lock is a write lock if isWritelock is true or a read lock
** if it is false.
*/
int sqlite3BtreeLockTable(Btree p, int iTab, u8 isWriteLock){
int rc = SQLITE_OK;
Debug.Assert( p.inTrans!=TRANS_NONE );
if( p.sharable ){
u8 lockType = READ_LOCK + isWriteLock;
Debug.Assert( READ_LOCK+1==WRITE_LOCK );
Debug.Assert( isWriteLock==null || isWriteLock==1 );

sqlite3BtreeEnter(p);
rc = querySharedCacheTableLock(p, iTab, lockType);
if( rc==SQLITE_OK ){
rc = setSharedCacheTableLock(p, iTab, lockType);
}
sqlite3BtreeLeave(p);
}
return rc;
}
#endif
	#if !SQLITE_OMIT_INCRBLOB
																																						/*
** Argument pCsr must be a cursor opened for writing on an
** INTKEY table currently pointing at a valid table entry.
** This function modifies the data stored as part of that entry.
**
** Only the data content may only be modified, it is not possible to
** change the length of the data stored. If this function is called with
** parameters that attempt to write past the end of the existing data,
** no modifications are made and SQLITE_CORRUPT is returned.
*/
int sqlite3BtreePutData(BtCursor pCsr, u32 offset, u32 amt, void *z){
int rc;
Debug.Assert( cursorHoldsMutex(pCsr) );
Debug.Assert( sqlite3_mutex_held(pCsr.pBtree.db.mutex) );
Debug.Assert( pCsr.isIncrblobHandle );

rc = restoreCursorPosition(pCsr);
if( rc!=SQLITE_OK ){
return rc;
}
Debug.Assert( pCsr.eState!=CURSOR_REQUIRESEEK );
if( pCsr.eState!=CURSOR_VALID ){
return SQLITE_ABORT;
}

/* Check some assumptions:
**   (a) the cursor is open for writing,
**   (b) there is a read/write transaction open,
**   (c) the connection holds a write-lock on the table (if required),
**   (d) there are no conflicting read-locks, and
**   (e) the cursor points at a valid row of an intKey table.
*/
if( !pCsr.wrFlag ){
return SQLITE_READONLY;
}
Debug.Assert( !pCsr.pBt.readOnly && pCsr.pBt.inTransaction==TRANS_WRITE );
Debug.Assert( hasSharedCacheTableLock(pCsr.pBtree, pCsr.pgnoRoot, 0, 2) );
Debug.Assert( !hasReadConflicts(pCsr.pBtree, pCsr.pgnoRoot) );
Debug.Assert( pCsr.apPage[pCsr.iPage].intKey );

return accessPayload(pCsr, offset, amt, (byte[] *)z, 1);
}

/*
** Set a flag on this cursor to cache the locations of pages from the
** overflow list for the current row. This is used by cursors opened
** for incremental blob IO only.
**
** This function sets a flag only. The actual page location cache
** (stored in BtCursor.aOverflow[]) is allocated and used by function
** accessPayload() (the worker function for sqlite3BtreeData() and
** sqlite3BtreePutData()).
*/
static void sqlite3BtreeCacheOverflow(BtCursor pCur){
Debug.Assert( cursorHoldsMutex(pCur) );
Debug.Assert( sqlite3_mutex_held(pCur.pBtree.db.mutex) );
invalidateOverflowCache(pCur)
pCur.isIncrblobHandle = 1;
}
#endif
	///
	///<summary>
	///Set both the "read version" (single byte at byte offset 18) and 
	///"write version" (single byte at byte offset 19) fields in the database
	///header to iVersion.
	///</summary>
	}
}
