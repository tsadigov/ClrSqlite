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


namespace Community.CsharpSqlite
{
    using DbPage = Cache.PgHdr;
    using Tree;
    using Community.CsharpSqlite.Paging;
    using Community.CsharpSqlite.Utils;

    public partial class Sqlite3
    {


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
        /// Set this global variable to 1 to enable tracing using the TRACE
        /// macro.
        ///</summary>
#if TRACE
																																																		static bool sqlite3BtreeTrace=false;  /* True to enable tracing */
// define TRACE(X)  if(sqlite3BtreeTrace){printf X;fflush(stdout);}
static void TRACE(string X, params object[] ap) { if (sqlite3BtreeTrace)  printf(X, ap); }
#else
        //# define TRACE(X)
        public static void TRACE(string X, params object[] ap)
        {
        }
#endif
    }



    namespace Tree
    {
        public static class BTreeMethods
        {
#if !SQLITE_OMIT_SHARED_CACHE
																																						//void sqlite3BtreeEnter(Btree);
//void sqlite3BtreeEnterAll(sqlite3);
#else
          

            //# define sqlite3BtreeEnterAll(X)
            public static void sqlite3BtreeEnterAll(this Connection p)
            {
            }

#endif


            ///<summary>
            /// These macros define the location of the pointer-map entry for a
            /// database page. The first argument to each is the number of usable
            /// bytes on each page of the database (often 1024). The second is the
            /// page number to look up in the pointer map.
            ///
            /// PTRMAP_PAGENO returns the database page number of the pointer-map
            /// page that stores the required pointer. PTRMAP_PTROFFSET returns
            /// the offset of the requested map entry.
            ///
            /// If the pgno argument passed to PTRMAP_PAGENO is a pointer-map page,
            /// then pgno is returned. So (pgno==PTRMAP_PAGENO(pgsz, pgno)) can be
            /// used to test if pgno is a pointer-map page. PTRMAP_ISPAGE implements
            /// this test.
            ///
            ///</summary>
            //#define PTRMAP_PAGENO(pBt, pgno) ptrmapPageno(pBt, pgno)
            public static Pgno PTRMAP_PAGENO(this BtShared pBt, Pgno pgno)
            {
                return pBt.ptrmapPageno(pgno);
            }

            //#define PTRMAP_PTROFFSET(pgptrmap, pgno) (5*(pgno-pgptrmap-1))
            public static u32 PTRMAP_PTROFFSET(u32 pgptrmap, u32 pgno)
            {
                return (5 * (pgno - pgptrmap - 1));
            }

            //#define PTRMAP_ISPAGE(pBt, pgno) (PTRMAP_PAGENO((pBt),(pgno))==(pgno))
            public static bool PTRMAP_ISPAGE(this BtShared pBt, u32 pgno)
            {
                return (PTRMAP_PAGENO((pBt), (pgno)) == (pgno));
            }



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
            public static int get2byteNotZero(byte[] X, int offset)
            {
                return (((((int)X.get2byte(offset)) - 1) & 0xffff) + 1);
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
sqliteinth.sqlite3GlobalConfig.sharedCacheEnabled = enable;
return SqlResult.SQLITE_OK;
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
            //#define querySharedCacheTableLock(a,b,c) SqlResult.SQLITE_OK
            //#define setSharedCacheTableLock(a,b,c) SqlResult.SQLITE_OK
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
/// SqlResult.SQLITE_OK if the lock may be obtained (by calling
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
return SqlResult.SQLITE_OK;
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
return SqlResult.SQLITE_OK;
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
///       already been called and returned SqlResult.SQLITE_OK).
///
/// SqlResult.SQLITE_OK is returned if the lock is added successfully. SQLITE_NOMEM
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
Debug.Assert( SqlResult.SQLITE_OK==querySharedCacheTableLock(p, iTable, eLock) );

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
pLock = (BtLock *)malloc_cs.sqlite3MallocZero(sizeof(BtLock));
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

return SqlResult.SQLITE_OK;
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
pLock=null;//malloc_cs.sqlite3_free(ref pLock);
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
  return Sqlite3.sqlite3_mutex_held( p.pBt.mutex );
}
#else
#endif
#if !SQLITE_OMIT_INCRBLOB
																																																		///<summary>
/// Invalidate the overflow page-list cache for cursor pCur, if any.
///</summary>
static void invalidateOverflowCache(BtCursor pCur){
Debug.Assert( cursorHoldsMutex(pCur) );
//malloc_cs.sqlite3_free(ref pCur.aOverflow);
pCur.aOverflow = null;
}

///<summary>
/// Invalidate the overflow page-list cache for all cursors opened
/// on the shared btree structure pBt.
///</summary>
static void invalidateAllOverflowCache(BtShared pBt){
BtCursor p;
Debug.Assert( pBt.mutex.sqlite3_mutex_held() );
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
            public static void invalidateOverflowCache(BtCursor pCur)
            {
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
            //         SqlResult.SQLITE_OK)
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
            /// If pRC is initially non-zero (non-SqlResult.SQLITE_OK) then this routine is
            /// a no-op.  If an error occurs, the appropriate error code is written
            /// into pRC.
            ///</summary>
            ///
            ///<summary>
            ///Read an entry from the pointer map.
            ///
            ///This routine retrieves the pointer map entry for page 'key', writing
            ///the type and parent page number to pEType and pPgno respectively.
            ///An error code is returned if something goes wrong, otherwise SqlResult.SQLITE_OK.
            ///</summary>
#else
																																																		//define ptrmapPut(w,x,y,z,rc)
//define ptrmapGet(w,x,y,z) SqlResult.SQLITE_OK
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
            public static u8[] findCellv2(u8[] pPage, u16 iCell, u16 O, int I)
            {
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
            /// Release a MemPage.  This should be called once for each prior
            /// call to btreeGetPage.
            ///</summary>
            public static void release(this MemPage pPage)
            {
                if (pPage != null)
                {
                    Debug.Assert(pPage.aData != null);
                    Debug.Assert(pPage.pBt != null);
                    //TODO -- find out why corrupt9 & diskfull fail on this tests 
                    //Debug.Assert(  PagerMethods.sqlite3PagerGetExtra ( pPage.pDbPage ) == pPage );
                    //Debug.Assert( sqlite3PagerGetData( pPage.pDbPage ) == pPage.aData );
                    Debug.Assert(pPage.pBt.mutex.sqlite3_mutex_held());
                    pPage.pDbPage.Unref();
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
            public static void pageReinit(DbPage pData)
            {
                var pPage = pData.getExtra();
                Debug.Assert(pData.sqlite3PagerPageRefcount() > 0);
                if (pPage.isInit != false)
                {
                    Debug.Assert(pPage.pBt.mutex.sqlite3_mutex_held());
                    pPage.isInit = false;
                    if (pData.sqlite3PagerPageRefcount() > 1)
                    {
                        ///pPage might not be a btree page;  it might be an overflow page
                        ///or ptrmap page or a free page.  In those cases, the following
                        ///call to btreeInitPage() will likely return SQLITE_CORRUPT.
                        ///But no harm is done by this.  And it is very important that
                        ///btreeInitPage() be called on every btree page so we make
                        ///the call for every page that comes in for re-initing. 
                        pPage.btreeInitPage();
                    }
                }
            }
            ///<summary>
            /// Invoke the busy handler for a btree.
            ///</summary>
            public static int btreeInvokeBusyHandler(object pArg)
            {
                BtShared pBt = (BtShared)pArg;
                Debug.Assert(pBt.db != null);
                Debug.Assert(pBt.db.mutex.sqlite3_mutex_held());
                return Sqlite3.sqlite3InvokeBusyHandler(pBt.db.busyHandler);
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
            ///
            ///<summary>
            ///Set the maximum page count for a database if mxPage is positive.
            ///No changes are made if mxPage is 0 or negative.
            ///Regardless of the value of mxPage, return the maximum page count.
            ///</summary>
            ///
            ///<summary>
            ///</summary>
            ///<param name="Set the secureDelete flag if newFlag is 0 or 1.  If newFlag is ">1,</param>
            ///<param name="then make no changes.  Always return the value of the secureDelete">then make no changes.  Always return the value of the secureDelete</param>
            ///<param name="setting after the change.">setting after the change.</param>
#endif
           
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
            public static SqlResult relocatePage(BtShared pBt,///
                ///Btree 
            MemPage pDbPage,///
                ///Open page to move 
            u8 eType,///
                ///Pointer map 'type' entry for pDbPage 
            Pgno iPtrPage,///
                ///<param name="Pointer map 'page">no' entry for pDbPage </param>
            Pgno iFreePage,///
                ///The location to move pDbPage to 
            int isCommit///
                ///isCommit flag passed to sqlite3PagerMovepage 
            )
            {
                MemPage pPtrPage = new MemPage();
                ///
                ///<summary>
                ///The page that contains a pointer to pDbPage 
                ///</summary>
                Pgno iDbPage = pDbPage.pgno;
                Pager pPager = pBt.pPager;
                SqlResult rc;
                Debug.Assert(eType == PTRMAP.OVERFLOW2 || eType == PTRMAP.OVERFLOW1 || eType == PTRMAP.BTREE || eType == PTRMAP.ROOTPAGE);
                Debug.Assert(pBt.mutex.sqlite3_mutex_held());
                Debug.Assert(pDbPage.pBt == pBt);
                ///
                ///<summary>
                ///Move page iDbPage from its current location to page number iFreePage 
                ///</summary>
                Log.TRACE("AUTOVACUUM: Moving %d to free page %d (ptr page %d type %d)\n", iDbPage, iFreePage, iPtrPage, eType);
                rc = pPager.sqlite3PagerMovepage(pDbPage.pDbPage, iFreePage, isCommit);
                if (rc != SqlResult.SQLITE_OK)
                {
                    return rc;
                }
                pDbPage.pgno = iFreePage;
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
                if (eType == PTRMAP.BTREE || eType == PTRMAP.ROOTPAGE)
                {
                    rc = pDbPage.setChildPtrmaps();
                    if (rc != SqlResult.SQLITE_OK)
                    {
                        return rc;
                    }
                }
                else
                {
                    Pgno nextOvfl = Converter.sqlite3Get4byte(pDbPage.aData);
                    if (nextOvfl != 0)
                    {
                        pBt.ptrmapPut(nextOvfl, PTRMAP.OVERFLOW2, iFreePage, ref rc);
                        if (rc != SqlResult.SQLITE_OK)
                        {
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
                if (eType != PTRMAP.ROOTPAGE)
                {
                    rc = pBt.GetPage(iPtrPage, ref pPtrPage, 0);
                    if (rc != SqlResult.SQLITE_OK)
                    {
                        return rc;
                    }
                    rc = PagerMethods.sqlite3PagerWrite(pPtrPage.pDbPage);
                    if (rc != SqlResult.SQLITE_OK)
                    {
                        BTreeMethods.release(pPtrPage);
                        return rc;
                    }
                    rc = pPtrPage.modifyPagePointer(iDbPage, iFreePage, eType);
                    release(pPtrPage);
                    if (rc == SqlResult.SQLITE_OK)
                    {
                        pBt.ptrmapPut(iFreePage, eType, iPtrPage, ref rc);
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
            ///<param name="return SqlResult.SQLITE_OK. If there is no work to do (and therefore no">return SqlResult.SQLITE_OK. If there is no work to do (and therefore no</param>
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
            public static SqlResult incrVacuumStep(BtShared pBt, Pgno nFin, Pgno iLastPg)
            {
                Pgno nFreeList;
                ///
                ///<summary>
                ///</summary>
                ///<param name="Number of pages still on the free">list </param>
                SqlResult rc;
                Debug.Assert(pBt.mutex.sqlite3_mutex_held());
                Debug.Assert(iLastPg > nFin);
                if (!pBt.PTRMAP_ISPAGE(iLastPg) && iLastPg != pBt.PENDING_BYTE_PAGE)
                {
                    u8 eType = 0;
                    Pgno iPtrPage = 0;
                    nFreeList = Converter.sqlite3Get4byte(pBt.pPage1.aData, 36);
                    if (nFreeList == 0)
                    {
                        return SqlResult.SQLITE_DONE;
                    }
                    rc = pBt.ptrmapGet(iLastPg, ref eType, ref iPtrPage);
                    if (rc != SqlResult.SQLITE_OK)
                    {
                        return rc;
                    }
                    if (eType == PTRMAP.ROOTPAGE)
                    {
                        return sqliteinth.SQLITE_CORRUPT_BKPT();
                    }
                    if (eType == PTRMAP.FREEPAGE)
                    {
                        if (nFin == 0)
                        {
                            ///
                            ///<summary>
                            ///</summary>
                            ///<param name="Remove the page from the files free">list. This is not required</param>
                            ///<param name="if nFin is non">list will be</param>
                            ///<param name="truncated to zero after this function returns, so it doesn't">truncated to zero after this function returns, so it doesn't</param>
                            ///<param name="matter if it still contains some garbage entries.">matter if it still contains some garbage entries.</param>
                            ///<param name=""></param>
                            Pgno iFreePg = 0;
                            MemPage pFreePg = new MemPage();
                            rc = pBt.allocateBtreePage( ref pFreePg, ref iFreePg, iLastPg, 1);
                            if (rc != SqlResult.SQLITE_OK)
                            {
                                return rc;
                            }
                            Debug.Assert(iFreePg == iLastPg);
                            BTreeMethods.release(pFreePg);
                        }
                    }
                    else
                    {
                        Pgno iFreePg = 0;
                        ///
                        ///<summary>
                        ///Index of free page to move pLastPg to 
                        ///</summary>
                        MemPage pLastPg = new MemPage();
                        rc = pBt.GetPage(iLastPg, ref pLastPg, 0);
                        if (rc != SqlResult.SQLITE_OK)
                        {
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
                        do
                        {
                            MemPage pFreePg = new MemPage();
                            rc = pBt.allocateBtreePage(ref pFreePg, ref iFreePg, 0, 0);
                            if (rc != SqlResult.SQLITE_OK)
                            {
                                pLastPg.release();
                                return rc;
                            }
                            pFreePg.release();
                        }
                        while (nFin != 0 && iFreePg > nFin);
                        Debug.Assert(iFreePg < iLastPg);
                        rc = PagerMethods.sqlite3PagerWrite(pLastPg.pDbPage);
                        if (rc == SqlResult.SQLITE_OK)
                        {
                            rc = BTreeMethods.relocatePage(pBt, pLastPg, eType, iPtrPage, iFreePg, (nFin != 0) ? 1 : 0);
                        }
                        pLastPg.release();
                        if (rc != SqlResult.SQLITE_OK)
                        {
                            return rc;
                        }
                    }
                }
                if (nFin == 0)
                {
                    iLastPg--;
                    while (iLastPg == (pBt.PENDING_BYTE_PAGE) || PTRMAP_ISPAGE(pBt, iLastPg))
                    {
                        if (PTRMAP_ISPAGE(pBt, iLastPg))
                        {
                            MemPage pPg = new MemPage();
                            rc = pBt.GetPage(iLastPg, ref pPg, 0);
                            if (rc != SqlResult.SQLITE_OK)
                            {
                                return rc;
                            }
                            rc = PagerMethods.sqlite3PagerWrite(pPg.pDbPage);
                            pPg.release();
                            if (rc != SqlResult.SQLITE_OK)
                            {
                                return rc;
                            }
                        }
                        iLastPg--;
                    }
                    pBt.pPager.sqlite3PagerTruncateImage(iLastPg);
                    pBt.nPage = iLastPg;
                }
                return SqlResult.SQLITE_OK;
            }
            ///
            ///<summary>
            ///</summary>
            ///<param name="A write">transaction must be opened before calling this function.</param>
            ///<param name="It performs a single unit of work towards an incremental vacuum.">It performs a single unit of work towards an incremental vacuum.</param>
            ///<param name=""></param>
            ///<param name="If the incremental vacuum is finished after this function has run,">If the incremental vacuum is finished after this function has run,</param>
            ///<param name="SQLITE_DONE is returned. If it is not finished, but no error occurred,">SQLITE_DONE is returned. If it is not finished, but no error occurred,</param>
            ///<param name="SqlResult.SQLITE_OK is returned. Otherwise an SQLite error code.">SqlResult.SQLITE_OK is returned. Otherwise an SQLite error code.</param>
            ///
            ///<summary>
            ///This routine is called prior to sqlite3PagerCommit when a transaction
            ///</summary>
            ///<param name="is commited for an auto">vacuum database.</param>
            ///<param name=""></param>
            ///<param name="If SqlResult.SQLITE_OK is returned, then pnTrunc is set to the number of pages">If SqlResult.SQLITE_OK is returned, then pnTrunc is set to the number of pages</param>
            ///<param name="the database file should be truncated to during the commit process.">the database file should be truncated to during the commit process.</param>
            ///<param name="i.e. the database has been reorganized so that only the first pnTrunc">i.e. the database has been reorganized so that only the first pnTrunc</param>
            ///<param name="pages are in use.">pages are in use.</param>
            public static SqlResult autoVacuumCommit(BtShared pBt)
            {
                SqlResult rc = SqlResult.SQLITE_OK;
                Pager pPager = pBt.pPager;
                // VVA_ONLY( int nRef = sqlite3PagerRefcount(pPager) );
#if !NDEBUG || DEBUG
																																																																											  int nRef = sqlite3PagerRefcount( pPager );
#else
                int nRef = 0;
#endif
                Debug.Assert(pBt.mutex.sqlite3_mutex_held());
                pBt.invalidateAllOverflowCache();
                Debug.Assert(pBt.autoVacuum);
                if (!pBt.incrVacuum)
                {
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
                    nOrig = pBt.btreePagecount();
                    if (PTRMAP_ISPAGE(pBt, nOrig) || nOrig == (pBt.PENDING_BYTE_PAGE))
                    {
                        ///
                        ///<summary>
                        ///It is not possible to create a database for which the final page
                        ///</summary>
                        ///<param name="is either a pointer">byte page. If one</param>
                        ///<param name="is encountered, this indicates corruption.">is encountered, this indicates corruption.</param>
                        ///<param name=""></param>
                        return sqliteinth.SQLITE_CORRUPT_BKPT();
                    }
                    nFree = Converter.sqlite3Get4byte(pBt.pPage1.aData, 36);
                    nEntry = (int)pBt.usableSize / 5;
                    nPtrmap = (Pgno)((nFree - nOrig + PTRMAP_PAGENO(pBt, nOrig) + (Pgno)nEntry) / nEntry);
                    nFin = nOrig - nFree - nPtrmap;
                    if (nOrig > pBt.PENDING_BYTE_PAGE && nFin < pBt.PENDING_BYTE_PAGE)
                    {
                        nFin--;
                    }
                    while (PTRMAP_ISPAGE(pBt, nFin) || nFin == pBt.PENDING_BYTE_PAGE)
                    {
                        nFin--;
                    }
                    if (nFin > nOrig)
                        return sqliteinth.SQLITE_CORRUPT_BKPT();
                    for (iFree = nOrig; iFree > nFin && rc == SqlResult.SQLITE_OK; iFree--)
                    {
                        rc = incrVacuumStep(pBt, nFin, iFree);
                    }
                    if ((rc == SqlResult.SQLITE_DONE || rc == SqlResult.SQLITE_OK) && nFree > 0)
                    {
                        rc = PagerMethods.sqlite3PagerWrite(pBt.pPage1.pDbPage);
                        Converter.sqlite3Put4byte(pBt.pPage1.aData, 32, 0);
                        Converter.sqlite3Put4byte(pBt.pPage1.aData, 36, 0);
                        Converter.sqlite3Put4byte(pBt.pPage1.aData, (u32)28, nFin);
                        pBt.pPager.sqlite3PagerTruncateImage(nFin);
                        pBt.nPage = nFin;
                    }
                    if (rc != SqlResult.SQLITE_OK)
                    {
                        pPager.sqlite3PagerRollback();
                    }
                }
                Debug.Assert(nRef == pPager.sqlite3PagerRefcount());
                return rc;
            }
#else
																																																		// define setChildPtrmaps(x) SqlResult.SQLITE_OK
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
            ///
            ///<summary>
            ///This function is called from both BtreeCommitPhaseTwo() and BtreeRollback()
            ///at the conclusion of a transaction.
            ///</summary>
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
            ///
            ///<summary>
            ///Do both phases of a commit.
            ///</summary>
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
            public static int countWriteCursors(BtShared pBt)
            {
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
            ///
            ///<summary>
            ///The second argument to this function, op, is always sqliteinth.SAVEPOINT_ROLLBACK
            ///or SAVEPOINT_RELEASE. This function either releases or rolls back the
            ///savepoint identified by parameter iSavepoint, depending on the value
            ///of op.
            ///
            ///Normally, iSavepoint is greater than or equal to zero. However, if op is
            ///</summary>
            ///<param name="sqliteinth.SAVEPOINT_ROLLBACK, then iSavepoint may also be ">1. In this case the</param>
            ///<param name="contents of the entire transaction are rolled back. This is different">contents of the entire transaction are rolled back. This is different</param>
            ///<param name="from a normal transaction rollback, as no locks are released and the">from a normal transaction rollback, as no locks are released and the</param>
            ///<param name="transaction remains open.">transaction remains open.</param>
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
            ///
            ///<summary>
            ///Return the size of a BtCursor object in bytes.
            ///
            ///This interfaces is needed so that users of cursors can preallocate
            ///sufficient storage to hold a cursor.  The BtCursor object is opaque
            ///</summary>
            ///<param name="to users so they cannot do the sizeof() themselves "> they must call</param>
            ///<param name="this routine.">this routine.</param>
            static int sqlite3BtreeCursorSize()
            {
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
            ///
            ///<summary>
            ///Close a cursor.  The read lock on the database file is released
            ///when the last cursor is closed.
            ///</summary>
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
  Debug.Assert( info.GetHashCode() == pCur.info.GetHashCode() || info.Equals( pCur.info ) );//_Custom.memcmp(info, pCur.info, sizeof(info))==0 );
}
#else
            //  #define assertCellInfo(x)
#endif
#if _MSC_VER
            ///
            ///<summary>
            ///Use a real function in MSVC to work around bugs in that compiler. 
            ///</summary>
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
            ///This routine cannot fail.  It always returns SqlResult.SQLITE_OK.
            ///</summary>
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
            ///<param name="Failure is not possible.  This function always returns SqlResult.SQLITE_OK.">Failure is not possible.  This function always returns SqlResult.SQLITE_OK.</param>
            ///<param name="It might just as well be a procedure (returning void) but we continue">It might just as well be a procedure (returning void) but we continue</param>
            ///<param name="to return an integer result code for historical reasons.">to return an integer result code for historical reasons.</param>
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
            public static SqlResult getOverflowPage(BtShared pBt,///
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
            )
            {
                Pgno next = 0;
                MemPage pPage = null;
                ppPage = null;
                SqlResult rc = SqlResult.SQLITE_OK;
                Debug.Assert(pBt.mutex.sqlite3_mutex_held());
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
                if (pBt.autoVacuum)
                {
                    Pgno pgno = 0;
                    Pgno iGuess = ovfl + 1;
                    u8 eType = 0;
                    while (PTRMAP_ISPAGE(pBt, iGuess) || iGuess == (pBt.PENDING_BYTE_PAGE))
                    {
                        iGuess++;
                    }
                    if (iGuess <= pBt.btreePagecount())
                    {
                        rc = pBt.ptrmapGet(iGuess, ref eType, ref pgno);
                        if (rc == SqlResult.SQLITE_OK && eType == PTRMAP.OVERFLOW2 && pgno == ovfl)
                        {
                            next = iGuess;
                            rc = SqlResult.SQLITE_DONE;
                        }
                    }
                }
#endif
                Debug.Assert(next == 0 || rc == SqlResult.SQLITE_DONE);
                if (rc == SqlResult.SQLITE_OK)
                {
                    rc = pBt.GetPage(ovfl, ref pPage, 0);
                    Debug.Assert(rc == SqlResult.SQLITE_OK || pPage == null);
                    if (rc == SqlResult.SQLITE_OK)
                    {
                        next = Converter.sqlite3Get4byte(pPage.aData);
                    }
                }
                pPgnoNext = next;
                if (ppPage != null)
                {
                    ppPage = pPage;
                }
                else
                {
                    BTreeMethods.release(pPage);
                }
                return (rc == SqlResult.SQLITE_DONE ? SqlResult.SQLITE_OK : rc);
            }
            ///
            ///<summary>
            ///Copy data from a buffer to a page, or from a page to a buffer.
            ///
            ///pPayload is a pointer to data stored on database page pDbPage.
            ///If argument eOp is false, then nByte bytes of data are copied
            ///from pPayload to the buffer pointed at by pBuf. If eOp is true,
            ///then PagerMethods.sqlite3PagerWrite() is called on pDbPage and nByte bytes
            ///of data are copied from the buffer pBuf to pPayload.
            ///
            ///SqlResult.SQLITE_OK is returned on success, otherwise an error code.
            ///</summary>
            public static SqlResult copyPayload(byte[] pPayload,///
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
            )
            {
                if (eOp != 0)
                {
                    ///
                    ///<summary>
                    ///Copy data from buffer to page (a write operation) 
                    ///</summary>
                    SqlResult rc = PagerMethods.sqlite3PagerWrite(pDbPage);
                    if (rc != SqlResult.SQLITE_OK)
                    {
                        return rc;
                    }
                    Buffer.BlockCopy(pBuf, (int)pBufOffset, pPayload, (int)payloadOffset, (int)nByte);
                    // memcpy( pPayload, pBuf, nByte );
                }
                else
                {
                    ///
                    ///<summary>
                    ///Copy data from page to buffer (a read operation) 
                    ///</summary>
                    Buffer.BlockCopy(pPayload, (int)payloadOffset, pBuf, (int)pBufOffset, (int)nByte);
                    //memcpy(pBuf, pPayload, nByte);
                }
                return SqlResult.SQLITE_OK;
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
            //    var rc = PagerMethods.sqlite3PagerWrite(pDbPage);
            //    if( rc!=SqlResult.SQLITE_OK ){
            //      return rc;
            //    }
            //    memcpy(pPayload, pBuf, nByte);
            //  }else{
            //    /* Copy data from page to buffer (a read operation) */
            //    memcpy(pBuf, pPayload, nByte);
            //  }
            //  return SqlResult.SQLITE_OK;
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
            ///
            ///<summary>
            ///Read part of the key associated with cursor pCur.  Exactly
            ///"amt" bytes will be transfered into pBuf[].  The transfer
            ///begins at "offset".
            ///
            ///The caller must ensure that pCur is pointing to a valid row
            ///in the table.
            ///
            ///Return SqlResult.SQLITE_OK on success or an error code if anything goes
            ///wrong.  An error is returned if "offset+amt" is larger than
            ///the available payload.
            ///</summary>
            ///
            ///<summary>
            ///Read part of the data associated with cursor pCur.  Exactly
            ///"amt" bytes will be transfered into pBuf[].  The transfer
            ///begins at "offset".
            ///
            ///Return SqlResult.SQLITE_OK on success or an error code if anything goes
            ///wrong.  An error is returned if "offset+amt" is larger than
            ///the available payload.
            ///</summary>
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
            ///
            ///<summary>
            ///</summary>
            ///<param name="Move the cursor down to the left">most leaf entry beneath the</param>
            ///<param name="entry to which it is currently pointing.">entry to which it is currently pointing.</param>
            ///<param name=""></param>
            ///<param name="The left"> the first</param>
            ///<param name="in ascending order.">in ascending order.</param>
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
            ///
            ///<summary>
            ///Move the cursor to the first entry in the table.  Return SqlResult.SQLITE_OK
            ///on success.  Set pRes to 0 if the cursor actually points to something
            ///or set pRes to 1 if the table is empty.
            ///</summary>
            ///
            ///<summary>
            ///Move the cursor to the last entry in the table.  Return SqlResult.SQLITE_OK
            ///on success.  Set pRes to 0 if the cursor actually points to something
            ///or set pRes to 1 if the table is empty.
            ///</summary>
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
            ///
            ///<summary>
            ///Return TRUE if the cursor is not pointing at an entry of the table.
            ///
            ///TRUE will be returned after a call to sqlite3BtreeNext() moves
            ///past the last entry in the table or sqlite3BtreePrev() moves past
            ///the first entry.  TRUE is also returned if the table is empty.
            ///</summary>
            ///
            ///<summary>
            ///Advance the cursor to the next entry in the database.  If
            ///successful then set pRes=0.  If the cursor
            ///was already pointing to the last entry in the database before
            ///this routine was called, then set pRes=1.
            ///</summary>
            ///
            ///<summary>
            ///Step the cursor to the back to the previous entry in the database.  If
            ///successful then set pRes=0.  If the cursor
            ///was already pointing to the first entry in the database before
            ///this routine was called, then set pRes=1.
            ///</summary>
            ///
                 
            
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
            public static int NN = 1;
            ///
            ///<summary>
            ///Number of neighbors on either side of pPage 
            ///</summary>
            public static int NB = (NN * 2 + 1);
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
            public static u8[] aBalanceQuickSpace = new u8[13];
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
            ///
            ///<summary>
            ///Delete the entry that the cursor is pointing to.  The cursor
            ///is left pointing at a arbitrary location.
            ///</summary>
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
            public static SqlResult btreeCreateTable(Btree p, ref int piTable, int createTabFlags)
            {
                BtShared pBt = p.pBt;
                MemPage pRoot = new MemPage();
                Pgno pgnoRoot = 0;
                SqlResult rc;
                PTF ptfFlags;
                ///
                ///<summary>
                ///</summary>
                ///<param name="Page">type flage for the root page of new table </param>
                Debug.Assert(Sqlite3.sqlite3BtreeHoldsMutex(p));
                Debug.Assert(pBt.inTransaction == TransType.TRANS_WRITE);
                Debug.Assert(!pBt.readOnly);
#if SQLITE_OMIT_AUTOVACUUM
																																																																											rc = allocateBtreePage(pBt, ref pRoot, ref pgnoRoot, 1, 0);
if( rc !=0){
return rc;
}
#else
                if (pBt.autoVacuum)
                {
                    Pgno pgnoMove = 0;
                    ///<param name="Move a page here to make room for the root">page </param>
                    MemPage pPageMove = new MemPage();
                    ///The page to move to. 
                    ///Creating a new table may probably require moving an existing database
                    ///to make room for the new tables root page. In case this page turns
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
                    pgnoRoot = p.sqlite3BtreeGetMeta(BTreeProp.LARGEST_ROOT_PAGE);
                    pgnoRoot++;
                    ///
                    ///<summary>
                    ///</summary>
                    ///<param name="The new root">map page, or the</param>
                    ///<param name="PENDING_BYTE page.">PENDING_BYTE page.</param>
                    ///<param name=""></param>
                    while (pgnoRoot == pBt.ptrmapPageno(pgnoRoot) || pgnoRoot == (pBt.PENDING_BYTE_PAGE))
                    {
                        pgnoRoot++;
                    }
                    Debug.Assert(pgnoRoot >= 3);
                    ///
                    ///<summary>
                    ///Allocate a page. The page that currently resides at pgnoRoot will
                    ///be moved to the allocated page (unless the allocated page happens
                    ///to reside at pgnoRoot).
                    ///
                    ///</summary>
                    rc = pBt.allocateBtreePage( ref pPageMove, ref pgnoMove, pgnoRoot, 1);
                    if (rc != SqlResult.SQLITE_OK)
                    {
                        return rc;
                    }
                    if (pgnoMove != pgnoRoot)
                    {
                        ///
                        ///<summary>
                        ///</summary>
                        ///<param name="pgnoRoot is the page that will be used for the root">page of</param>
                        ///<param name="the new table (assuming an error did not occur). But we were">the new table (assuming an error did not occur). But we were</param>
                        ///<param name="allocated pgnoMove. If required (i.e. if it was not allocated">allocated pgnoMove. If required (i.e. if it was not allocated</param>
                        ///<param name="by extending the file), the current page at position pgnoMove">by extending the file), the current page at position pgnoMove</param>
                        ///<param name="is already journaled.">is already journaled.</param>
                        ///<param name=""></param>
                        u8 eType = 0;
                        Pgno iPtrPage = 0;
                        BTreeMethods.release(pPageMove);
                        ///
                        ///<summary>
                        ///Move the page currently at pgnoRoot to pgnoMove. 
                        ///</summary>
                        rc = pBt.GetPage(pgnoRoot, ref pRoot, 0);
                        if (rc != SqlResult.SQLITE_OK)
                        {
                            return rc;
                        }
                        rc = pBt.ptrmapGet(pgnoRoot, ref eType, ref iPtrPage);
                        if (eType == PTRMAP.ROOTPAGE || eType == PTRMAP.FREEPAGE)
                        {
                            rc = sqliteinth.SQLITE_CORRUPT_BKPT();
                        }
                        if (rc != SqlResult.SQLITE_OK)
                        {
                            release(pRoot);
                            return rc;
                        }
                        Debug.Assert(eType != PTRMAP.ROOTPAGE);
                        Debug.Assert(eType != PTRMAP.FREEPAGE);
                        rc = BTreeMethods.relocatePage(pBt, pRoot, eType, iPtrPage, pgnoMove, 0);
                        BTreeMethods.release(pRoot);
                        ///
                        ///<summary>
                        ///Obtain the page at pgnoRoot 
                        ///</summary>
                        if (rc != SqlResult.SQLITE_OK)
                        {
                            return rc;
                        }
                        rc = pBt.GetPage(pgnoRoot, ref pRoot, 0);
                        if (rc != SqlResult.SQLITE_OK)
                        {
                            return rc;
                        }
                        rc = PagerMethods.sqlite3PagerWrite(pRoot.pDbPage);
                        if (rc != SqlResult.SQLITE_OK)
                        {
                            BTreeMethods.release(pRoot);
                            return rc;
                        }
                    }
                    else
                    {
                        pRoot = pPageMove;
                    }
                    ///
                    ///<summary>
                    ///</summary>
                    ///<param name="Update the pointer">page number. </param>
                    pBt.ptrmapPut(pgnoRoot, PTRMAP.ROOTPAGE, 0, ref rc);
                    if (rc != 0)
                    {
                        BTreeMethods.release(pRoot);
                        return rc;
                    }
                    ///
                    ///<summary>
                    ///When the new root page was allocated, page 1 was made writable in
                    ///order either to increase the database filesize, or to decrement the
                    ///freelist count.  Hence, the sqlite3BtreeUpdateMeta() call cannot fail.
                    ///
                    ///</summary>
                    Debug.Assert(pBt.pPage1.pDbPage.sqlite3PagerIswriteable());
                    rc = p.sqlite3BtreeUpdateMeta(BTreeProp.LARGEST_ROOT_PAGE, pgnoRoot);
                    if (NEVER(rc != 0))
                    {
                        BTreeMethods.release(pRoot);
                        return rc;
                    }
                }
                else
                {
                    rc = pBt.allocateBtreePage( ref pRoot, ref pgnoRoot, 1, 0);
                    if (rc != 0)
                        return rc;
                }
#endif
                Debug.Assert(pRoot.pDbPage.sqlite3PagerIswriteable());
                if ((createTabFlags & Sqlite3.BTREE_INTKEY) != 0)
                {
                    ptfFlags = PTF.INTKEY | PTF.LEAFDATA | PTF.LEAF;
                }
                else
                {
                    ptfFlags = PTF.ZERODATA | PTF.LEAF;
                }
                pRoot.zeroPage(ptfFlags);
                pRoot.pDbPage.Unref();
                Debug.Assert((pBt.openFlags & Sqlite3.BTREE_SINGLE) == 0 || pgnoRoot == 2);
                piTable = (int)pgnoRoot;
                return SqlResult.SQLITE_OK;
            }

            private static bool NEVER(bool p)
            {
                return Sqlite3.NEVER(p);
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
            ///<param name="SqlResult.SQLITE_OK is returned if the operation is successfully executed.">SqlResult.SQLITE_OK is returned if the operation is successfully executed.</param>
            ///<param name="Otherwise, if an error is encountered (i.e. an IO error or database">Otherwise, if an error is encountered (i.e. an IO error or database</param>
            ///<param name="corruption) an SQLite error code is returned.">corruption) an SQLite error code is returned.</param>
#endif
            ///
            ///<summary>
            ///Return the pager associated with a BTree.  This routine is used for
            ///testing and debugging only.
            ///</summary>
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
            public static i64 refNULL = 0;
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
var rc = SqlResult.SQLITE_OK;
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
            ///<param name="blob of allocated memory. The xFree function should not call malloc_cs.sqlite3_free()">blob of allocated memory. The xFree function should not call malloc_cs.sqlite3_free()</param>
            ///<param name="on the memory, the btree layer does that.">on the memory, the btree layer does that.</param>
            ///
            ///<summary>
            ///Return SQLITE_LOCKED_SHAREDCACHE if another user of the same shared
            ///btree as the argument handle holds an exclusive lock on the
            ///sqlite_master table. Otherwise SqlResult.SQLITE_OK.
            ///</summary>
#if !SQLITE_OMIT_SHARED_CACHE
																																							/*
** Obtain a lock on the table whose root page is iTab.  The
** lock is a write lock if isWritelock is true or a read lock
** if it is false.
*/
int sqlite3BtreeLockTable(Btree p, int iTab, u8 isWriteLock){
var rc = SqlResult.SQLITE_OK;
Debug.Assert( p.inTrans!=TRANS_NONE );
if( p.sharable ){
u8 lockType = READ_LOCK + isWriteLock;
Debug.Assert( READ_LOCK+1==WRITE_LOCK );
Debug.Assert( isWriteLock==null || isWriteLock==1 );

sqlite3BtreeEnter(p);
rc = querySharedCacheTableLock(p, iTab, lockType);
if( rc==SqlResult.SQLITE_OK ){
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
Debug.Assert( Sqlite3.pCsr.pBtree.db.mutex.sqlite3_mutex_held() );
Debug.Assert( pCsr.isIncrblobHandle );

rc = restoreCursorPosition(pCsr);
if( rc!=SqlResult.SQLITE_OK ){
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
Debug.Assert( Sqlite3.pCur.pBtree.db.mutex.sqlite3_mutex_held() );
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
}