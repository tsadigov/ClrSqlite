using System;
using System.Diagnostics;
using i16 = System.Int16;
using i64 = System.Int64;
using u8 = System.Byte;
using u16 = System.UInt16;
using u32 = System.UInt32;
using u64 = System.UInt64;
using sqlite3_int64 = System.Int64;
using Pgno = System.UInt32;
namespace Community.CsharpSqlite
{
    using DbPage = Cache.PgHdr;
    using System.Text;
    using Metadata;
    using Community.CsharpSqlite.Os;
    using Community.CsharpSqlite.Paging;
    using Utils;


    namespace Tree
    {
        public partial class Btree
        {
            ///<summary>
            ///Open a database file.
            ///
            ///zFilename is the name of the database file.  If zFilename is NULL
            ///then an ephemeral database is created.  The ephemeral database might
            ///be exclusively in memory, or it might use a disk">based memory cache.</param>
            ///Either way, the ephemeral database will be automatically deleted ">Either way, the ephemeral database will be automatically deleted </param>
            ///when sqlite3BtreeClose() is called.">when sqlite3BtreeClose() is called.</param>
            ///If zFilename is ":memory:" then an in">memory database is created</param>
            ///that is automatically destroyed when it is closed.">that is automatically destroyed when it is closed.</param>
            ///The "flags" parameter is a bitmask that might contain bits">The "flags" parameter is a bitmask that might contain bits</param>
            ///BTREE_OMIT_JOURNAL and/or BTREE_NO_READLOCK.  The BTREE_NO_READLOCK">BTREE_OMIT_JOURNAL and/or BTREE_NO_READLOCK.  The BTREE_NO_READLOCK</param>
            ///bit is also set if the SQLITE_NoReadlock flags is set in db">>flags.</param>
            ///These flags are passed through into sqlite3PagerOpen() and must">These flags are passed through into sqlite3PagerOpen() and must</param>
            ///be the same values as PAGER_OMIT_JOURNAL and PAGER_NO_READLOCK.">be the same values as PAGER_OMIT_JOURNAL and PAGER_NO_READLOCK.</param>
            ///If the database is already opened in the same database connection">If the database is already opened in the same database connection</param>
            ///and we are in shared cache mode, then the open will fail with an">and we are in shared cache mode, then the open will fail with an</param>
            ///SQLITE_CONSTRAINT error.  We cannot allow two or more BtShared">SQLITE_CONSTRAINT error.  We cannot allow two or more BtShared</param>
            ///objects in the same database connection since doing so will lead">objects in the same database connection since doing so will lead</param>
            ///to problems with locking.">to problems with locking.</param>
            //sqlite3BtreeOpen
            public static SqlResult Open(
                Os.sqlite3_vfs pVfs,///VFS to use for this b"tree 
            string zFilename,
            ///Name of the file containing the BTree database 
            Connection db,
            ///Associated database handle 
            ref Btree refBtree,
            ///Pointer to new Btree object written here 
            int flags,
            ///<summary>
            ///Options 
            ///</summary>
            int vfsFlags///
                        ///<summary>
                        ///Flags passed through to sqlite3_vfs.xOpen() 
                        ///</summary>
            )
            {

                ///Handle to return 
                Btree createdBTreeInstance;
                ///Prevents a race condition. Ticket #3537 
                sqlite3_mutex mutexOpen = null;
                ///Result code from this function 
                SqlResult rc = SqlResult.SQLITE_OK;
                ///Byte of unused space on each page 
                u8 nReserve;
                ///Database header content 
                ///True if opening an ephemeral, temporary database 
                byte[] zDbHeader = new byte[100];
                //zFilename==0 || zFilename[0]==0;
                ///Set the variable isMemdb to true for an in-memory database, or 
                ///false for a file">based database.
                bool isTempDb = String.IsNullOrEmpty(zFilename);
#if SQLITE_OMIT_MEMORYDB
																																																																											bool isMemdb = false;
#else
                bool isMemdb = (zFilename == ":memory:") || (isTempDb && Sqlite3.sqlite3TempInMemory(db));
#endif
                Debug.Assert(db != null);
                Debug.Assert(pVfs != null);
                Debug.Assert(db.mutex.sqlite3_mutex_held());
                Debug.Assert((flags & 0xff) == flags);
                ///flags fit in 8 bits 

                ///Only a BTREE_SINGLE database can be BTREE_UNORDERED 
                Debug.Assert((flags & Sqlite3.BTREE_UNORDERED) == 0 || (flags & Sqlite3.BTREE_SINGLE) != 0);

                ///A BTREE_SINGLE database is always a temporary and/or ephemeral 
                Debug.Assert((flags & Sqlite3.BTREE_SINGLE) == 0 || isTempDb);
                if ((db.flags & SqliteFlags.SQLITE_NoReadlock) != 0)
                {
                    flags |= Sqlite3.BTREE_NO_READLOCK;
                }
                if (isMemdb)
                {
                    flags |= Sqlite3.BTREE_MEMORY;
                }
                if ((vfsFlags & Sqlite3.SQLITE_OPEN_MAIN_DB) != 0 && (isMemdb || isTempDb))
                {
                    vfsFlags = (vfsFlags & ~Sqlite3.SQLITE_OPEN_MAIN_DB) | Sqlite3.SQLITE_OPEN_TEMP_DB;
                }
                createdBTreeInstance = new Btree()
                {
                    inTrans = TransType.TRANS_NONE,
                    db = db
                };
                //malloc_cs.sqlite3MallocZero(sizeof(Btree));
                //if( !p ){
                //  return SQLITE_NOMEM;
                //}
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
string zFullPathname = malloc_cs.sqlite3Malloc(nFullPathname);
sqlite3_mutex *mutexShared;
p.sharable = 1;
if( !zFullPathname ){
p = null;//malloc_cs.sqlite3_free(ref p);
return SQLITE_NOMEM;
}
sqlite3OsFullPathname(pVfs, zFilename, nFullPathname, zFullPathname);
mutexOpen = sqlite3MutexAlloc(SQLITE_MUTEX_STATIC_OPEN);
mutexOpen.sqlite3_mutex_enter();
mutexShared = sqlite3MutexAlloc(SQLITE_MUTEX_STATIC_MASTER);
mutexShared.sqlite3_mutex_enter();
for(pBt=GLOBAL(BtShared*,sqlite3SharedCacheList); pBt; pBt=pBt.pNext){
Debug.Assert( pBt.nRef>0 );
if( 0==strcmp(zFullPathname, sqlite3PagerFilename(pBt.pPager))
&& sqlite3PagerVfs(pBt.pPager)==pVfs ){
int iDb;
for(iDb=db.nDb-1; iDb>=0; iDb--){
Btree pExisting = db.aDb[iDb].pBt;
if( pExisting && pExisting.pBt==pBt ){
mutexShared.sqlite3_mutex_leave();
mutexOpen.sqlite3_mutex_leave();
zFullPathname = null;//malloc_cs.sqlite3_free(ref zFullPathname);
p=null;//malloc_cs.sqlite3_free(ref p);
return SQLITE_CONSTRAINT;
}
}
p.pBt = pBt;
pBt.nRef++;
break;
}
}
mutexShared.sqlite3_mutex_leave();
zFullPathname=null;//malloc_cs.sqlite3_free(ref zFullPathname);
}
#if SQLITE_DEBUG
																																																																											else{
/* In debug mode, we mark all persistent databases as sharable
** even when they are not.  This exercises the locking code and
** gives more opportunity for asserts(Sqlite3.sqlite3_mutex_held())
** statements to find locking problems.
*/
p.sharable = 1;
}
#endif
																																																																											}
#endif
                ///Shared part of btree structure 
                BtShared pBt = null;
                if (pBt == null)
                {
                    ///The following asserts make sure that structures used by the btree are
                    ///the right size.  This is to guard against size changes that result
                    ///when compiling on a different architecture.
                    Debug.Assert(sizeof(i64) == 8 || sizeof(i64) == 4);
                    Debug.Assert(sizeof(u64) == 8 || sizeof(u64) == 4);
                    Debug.Assert(sizeof(u32) == 4);
                    Debug.Assert(sizeof(u16) == 2);
                    Debug.Assert(sizeof(Pgno) == 4);
                    pBt = new BtShared();
                    //malloc_cs.sqlite3MallocZero( sizeof(pBt) );
                    //if( pBt==null ){
                    //  rc = SQLITE_NOMEM;
                    //  goto btree_open_out;
                    //}
                    rc = PagerMethods.PagerOpen(pVfs, out pBt.pPager, zFilename, Globals.BTree.EXTRA_SIZE, flags, vfsFlags, BTreeMethods.pageReinit);
                    if (rc == SqlResult.SQLITE_OK)
                    {
                        rc = pBt.pPager.sqlite3PagerReadFileheader(zDbHeader.Length, zDbHeader);
                    }
                    if (rc != SqlResult.SQLITE_OK)
                    {
                        goto btree_open_out;
                    }
                    pBt.openFlags = (u8)flags;
                    pBt.db = db;
                    pBt.pPager.sqlite3PagerSetBusyhandler(BTreeMethods.btreeInvokeBusyHandler, pBt);
                    createdBTreeInstance.pBt = pBt;
                    pBt.pCursor = null;
                    pBt.pPage1 = null;
                    pBt.readOnly = pBt.pPager.sqlite3PagerIsreadonly();
#if SQLITE_SECURE_DELETE
																																																																																																				pBt.secureDelete = true;
#endif
                    pBt.pageSize = (u32)((zDbHeader[16] << 8) | (zDbHeader[17] << 16));
                    if (pBt.pageSize < 512 || pBt.pageSize > Limits.SQLITE_MAX_PAGE_SIZE || ((pBt.pageSize - 1) & pBt.pageSize) != 0)
                    {
                        pBt.pageSize = 0;
#if !SQLITE_OMIT_AUTOVACUUM
                        ///<param name="If the magic name ":memory:" will create an in">memory database, then</param>
                        ///<param name="leave the autoVacuum mode at 0 (do not auto">vacuum), even if</param>
                        ///<param name="SQLITE_DEFAULT_AUTOVACUUM is true. On the other hand, if">SQLITE_DEFAULT_AUTOVACUUM is true. On the other hand, if</param>
                        ///<param name="SQLITE_OMIT_MEMORYDB has been defined, then ":memory:" is just a">SQLITE_OMIT_MEMORYDB has been defined, then ":memory:" is just a</param>
                        ///<param name="regular file">vacuum applies as per normal.</param>
                        if (zFilename != "" && !isMemdb)
                        {
                            pBt.autoVacuum = (Sqlite3.SQLITE_DEFAULT_AUTOVACUUM != 0);
                            pBt.incrVacuum = (Sqlite3.SQLITE_DEFAULT_AUTOVACUUM == 2);
                        }
#endif
                        nReserve = 0;
                    }
                    else
                    {
                        nReserve = zDbHeader[20];
                        pBt.pageSizeFixed = true;
#if !SQLITE_OMIT_AUTOVACUUM
                        pBt.autoVacuum = Converter.sqlite3Get4byte(zDbHeader, 36 + 4 * 4) != 0;
                        pBt.incrVacuum = Converter.sqlite3Get4byte(zDbHeader, 36 + 7 * 4) != 0;
#endif
                    }
                    rc = pBt.pPager.sqlite3PagerSetPagesize(ref pBt.pageSize, nReserve);
                    if (rc != 0)
                        goto btree_open_out;
                    pBt.usableSize = (u16)(pBt.pageSize - nReserve);
                    Debug.Assert((pBt.pageSize & 7) == 0);
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
if( SQLITE_THREADSAFE && sqliteinth.sqlite3GlobalConfig.bCoreMutex ){
pBt.mutex = sqlite3MutexAlloc(SQLITE_MUTEX_FAST);
if( pBt.mutex==null ){
rc = SQLITE_NOMEM;
db.mallocFailed = 0;
goto btree_open_out;
}
}
mutexShared.sqlite3_mutex_enter();
pBt.pNext = GLOBAL(BtShared*,sqlite3SharedCacheList);
GLOBAL(BtShared*,sqlite3SharedCacheList) = pBt;
mutexShared.sqlite3_mutex_leave();
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
                refBtree = createdBTreeInstance;
                btree_open_out:
                if (rc != SqlResult.SQLITE_OK)
                {
                    if (pBt != null && pBt.pPager != null)
                    {
                        pBt.pPager.sqlite3PagerClose();
                    }
                    pBt = null;
                    //    malloc_cs.sqlite3_free(ref pBt);
                    createdBTreeInstance = null;
                    //    malloc_cs.sqlite3_free(ref p);
                    refBtree = null;
                }
                else
                {

                    ///If the B">cache size to the</param>
                    ///default value. Except, when opening on an existing shared pager">cache,</param>
                    ///do not change the pager">cache size.</param>
                    if (createdBTreeInstance.sqlite3BtreeSchema(0, null) == null)
                    {
                        createdBTreeInstance.pBt.pPager.sqlite3PagerSetCachesize(Globals.SQLITE_DEFAULT_CACHE_SIZE);
                    }
                }
                if (mutexOpen != null)
                {
                    Debug.Assert(mutexOpen.sqlite3_mutex_held());
                    mutexOpen.Exit();
                }
                return rc;
            }

            ///<summary>
            ///Close an open database and invalidate all cursors.
            ///</summary>
            public SqlResult Close()
            {
                BtShared pBt = this.pBt;
                ///Close all cursors opened via this handle.  
                Debug.Assert(this.db.mutex.sqlite3_mutex_held());
                using (this.scope())
                {
                    pBt.pCursor.linkedList().ForEach(
                        pCur => pCur.Close()
                        );

                    ///Rollback any active transaction and free the handle structure.
                    ///The call to sqlite3BtreeRollback() drops any table-locks held by
                    ///this handle.
                    this.sqlite3BtreeRollback();
                }
                ///If there are still other outstanding references to the shared">btree</param>
                ///structure, return now. The remainder of this procedure cleans
                ///btree.
                Debug.Assert(this.wantToLock == 0 && !this.locked);
                if (!this.sharable || pBt.removeFromSharingList())
                {
                    ///The pBt is no longer on the sharing list, so we can access
                    ///it without having to hold the mutex.
                    ///Clean out and delete the BtShared object.
                    Debug.Assert(null == pBt.pCursor);
                    pBt.pPager.sqlite3PagerClose();
                    if (pBt.xFreeSchema != null && pBt.pSchema != null)
                    {
                        pBt.xFreeSchema(pBt.pSchema);
                    }
                    pBt.pSchema = null;
                    // sqlite3DbFree(0, pBt->pSchema);
                    //freeTempSpace(pBt);
                    pBt = null;
                    //malloc_cs.sqlite3_free(ref pBt);
                }
#if !SQLITE_OMIT_SHARED_CACHE
																																																																											Debug.Assert( p.wantToLock==null );
Debug.Assert( p.locked==null );
if( p.pPrev ) p.pPrev.pNext = p.pNext;
if( p.pNext ) p.pNext.pPrev = p.pPrev;
#endif
                //malloc_cs.sqlite3_free(ref p);
                return SqlResult.SQLITE_OK;
            }




            #region transaction

            public SqlResult sqlite3BtreeBeginTrans(int wrflag)
            {
                BtShared pBt = this.pBt;
                SqlResult rc = SqlResult.SQLITE_OK;
                this.Enter();
                Sqlite3.btreeIntegrity(this);
                ///If the btree is already in a write-transaction, or it
                ///is already in a read-transaction and
                ///is requested, this is a no-op.
                if (this.inTrans == TransType.TRANS_WRITE || (this.inTrans == TransType.TRANS_READ && 0 == wrflag))
                {
                    goto trans_begun;
                }
                ///Write transactions are not possible on a read-only database 
                if (pBt.readOnly && wrflag != 0)
                {
                    rc = SqlResult.SQLITE_READONLY;
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
                ///Any read-lock on
                ///page 1. So if some other shared-lock
                ///on page 1, the transaction cannot be opened. 
                rc = this.querySharedCacheTableLock(sqliteinth.MASTER_ROOT, BtLockType.READ_LOCK);
                if (SqlResult.SQLITE_OK != rc)
                    goto trans_begun;
                pBt.initiallyEmpty = pBt.nPage == 0;
                do
                {
                    ///Call lockBtree() until either pBt.pPage1 is populated or
                    ///lockBtree() returns something other than SqlResult.SQLITE_OK. lockBtree()
                    ///may return SqlResult.SQLITE_OK but leave pBt.pPage1 set to 0 if after
                    ///<param name="reading page 1 it discovers that the page">size of the database</param>
                    ///<param name="file is not pBt.pageSize. In this case lockBtree() will update">file is not pBt.pageSize. In this case lockBtree() will update</param>
                    ///<param name="pBt.pageSize to the page">size of the file on disk.</param>
                    ///<param name=""></param>
                    while (pBt.pPage1 == null && SqlResult.SQLITE_OK == (rc = pBt.lockBtree()))
                        ;
                    if (rc == SqlResult.SQLITE_OK && wrflag != 0)
                    {
                        if (pBt.readOnly)
                        {
                            rc = SqlResult.SQLITE_READONLY;
                        }
                        else
                        {
                            rc = pBt.pPager.sqlite3PagerBegin(wrflag > 1, Sqlite3.sqlite3TempInMemory(this.db) ? 1 : 0);
                            if (rc == SqlResult.SQLITE_OK)
                            {
                                rc = pBt.newDatabase();
                            }
                        }
                    }
                    if (rc != SqlResult.SQLITE_OK)
                    {
                        pBt.unlockIfUnused();
                    }
                }
                while ((rc & (SqlResult)0xFF) == SqlResult.SQLITE_BUSY && pBt.inTransaction == TransType.TRANS_NONE && BTreeMethods.btreeInvokeBusyHandler(pBt) != 0);
                if (rc == SqlResult.SQLITE_OK)
                {
                    if (this.inTrans == TransType.TRANS_NONE)
                    {
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
                    this.inTrans = (wrflag != 0 ? TransType.TRANS_WRITE : TransType.TRANS_READ);
                    if (this.inTrans > pBt.inTransaction)
                    {
                        pBt.inTransaction = this.inTrans;
                    }
                    if (wrflag != 0)
                    {
                        MemPage pPage1 = pBt.pPage1;
#if !SQLITE_OMIT_SHARED_CACHE
																																																																																																																														Debug.Assert( !pBt.pWriter );
pBt.pWriter = p;
pBt.isExclusive = (u8)(wrflag>1);
#endif
                        ///<param name="If the db">size header field is incorrect (as it may be if an old</param>
                        ///<param name="client has been writing the database file), update it now. Doing">client has been writing the database file), update it now. Doing</param>
                        ///<param name="this sooner rather than later means the database size can safely ">this sooner rather than later means the database size can safely </param>
                        ///<param name="re">read the database size from page 1 if a savepoint or transaction</param>
                        ///<param name="rollback occurs within the transaction.">rollback occurs within the transaction.</param>
                        if (pBt.nPage != Converter.sqlite3Get4byte(pPage1.aData, 28))
                        {
                            rc = PagerMethods.sqlite3PagerWrite(pPage1.pDbPage);
                            if (rc == SqlResult.SQLITE_OK)
                            {
                                Converter.sqlite3Put4byte(pPage1.aData, (u32)28, pBt.nPage);
                            }
                        }
                    }
                }
                trans_begun:
                if (rc == SqlResult.SQLITE_OK && wrflag != 0)
                {
                    ///This call makes sure that the pager has the correct number of
                    ///open savepoints. If the second parameter is greater than 0 and
                    ///<param name="the sub">journal is not already open, then it will be opened here.</param>
                    ///<param name=""></param>
                    rc = pBt.pPager.sqlite3PagerOpenSavepoint(this.db.nSavepoint);
                }
                Sqlite3.btreeIntegrity(this);
                this.Exit();
                return rc;
            }

            public void btreeEndTransaction()
            {
                BtShared pBt = this.pBt;
                Debug.Assert(Sqlite3.sqlite3BtreeHoldsMutex(this));
                pBt.btreeClearHasContent();
                if (this.inTrans > TransType.TRANS_NONE && this.db.activeVdbeCnt > 1)
                {
                    ///If there are other active statements that belong to this database
                    ///handle, downgrade to a read">only transaction. The other statements</param>
                    ///may still be reading from the database.  ">may still be reading from the database.  </param>
                    this.downgradeAllSharedCacheTableLocks();
                    this.inTrans = TransType.TRANS_READ;
                }
                else
                {
                    ///
                    ///<summary>
                    ///If the handle had any kind of transaction open, decrement the
                    ///transaction count of the shared btree. If the transaction count
                    ///reaches 0, set the shared state to TRANS_NONE. The unlockBtreeIfUnused()
                    ///call below will unlock the pager.  
                    ///</summary>
                    if (this.inTrans != TransType.TRANS_NONE)
                    {
                        this.clearAllSharedCacheTableLocks();
                        pBt.nTransaction--;
                        if (0 == pBt.nTransaction)
                        {
                            pBt.inTransaction = TransType.TRANS_NONE;
                        }
                    }
                    ///
                    ///<summary>
                    ///Set the current transaction state to TRANS_NONE and unlock the
                    ///pager if this call closed the only read or write transaction.  
                    ///</summary>
                    this.inTrans = TransType.TRANS_NONE;
                    pBt.unlockIfUnused();
                }
                Sqlite3.btreeIntegrity(this);
            }

            #endregion

            #region file
            /**
///<summary>
///Copy the complete content of pBtFrom into pBtTo.  A transaction
///must be active for both files.
///
///The size of file pTo may be reduced by this operation. If anything
///goes wrong, the transaction on pTo is rolled back. If successful, the
///transaction is committed before returning.
///</summary>
*/
            public SqlResult sqlite3BtreeCopyFile(Btree fromBTree)
            {
                SqlResult rc;
                sqlite3_backup b;
                using (this.scope())
                {
                    using (fromBTree.scope())
                    {
                        ///Set up an sqlite3_backup object. sqlite3_backup.pDestDb must be set
                        ///to 0. This is used by the implementations of sqlite3_backup_step()
                        ///and sqlite3_backup_finish() to detect that they are being called
                        ///from this function, not directly by the user.
                        b = new sqlite3_backup()
                        {
                            pSrcDb = fromBTree.db,
                            pSrc = fromBTree,
                            pDest = this,
                            iNext = 1,
                        };
                        // memset( &b, 0, sizeof( b ) );

                        ///0x7FFFFFFF is the hard limit for the number of pages in a database
                        ///file. By passing this as the number of pages to copy to
                        ///sqlite3_backup_step(), we can guarantee that the copy finishes
                        ///within a single call (unless an error occurs). The Debug.Assert() statement
                        ///checks this assumption  (p.rc) should be set to either SQLITE_DONE
                        ///or an error code.
                        b.sqlite3_backup_step(0x7FFFFFFF);
                        Debug.Assert(b.rc != SqlResult.SQLITE_OK);
                        rc = b.sqlite3_backup_finish();
                        if (rc == SqlResult.SQLITE_OK)
                        {
                            this.pBt.pageSizeFixed = false;
                        }
                    }
                }
                return rc;
            }
            #endregion



            #region table

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
            public SqlResult CreateTable(ref int piTable, int flags)
            {
                SqlResult rc;
                using (this.scope())
                {
                    rc = this._createTable(ref piTable, flags);
                }
                return rc;
            }

            
            SqlResult _createTable(ref int piTable, int createTabFlags)
            {
                Btree p = this;
                BtShared pBt = p.pBt;
                MemPage pRoot = new MemPage();
                Pgno pgnoRoot = 0;
                SqlResult rc;
                PTF ptfFlags;
                ///Page-type flage for the root page of new table
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
                    rc = pBt.allocateBtreePage(ref pPageMove, ref pgnoMove, pgnoRoot, 1);
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
                        pPageMove.release();
                        
                        ///Move the page currently at pgnoRoot to pgnoMove. 
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
                            pRoot.release();
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
                        pRoot.release();
                        return rc;
                    }
                    ///When the new root page was allocated, page 1 was made writable in
                    ///order either to increase the database filesize, or to decrement the
                    ///freelist count.  Hence, the sqlite3BtreeUpdateMeta() call cannot fail.
                    Debug.Assert(pBt.pPage1.pDbPage.sqlite3PagerIswriteable());
                    rc = p.sqlite3BtreeUpdateMeta(BTreeProp.LARGEST_ROOT_PAGE, pgnoRoot);
                    if (NEVER(rc != 0))
                    {
                        pRoot.release();
                        return rc;
                    }
                }
                else
                {
                    rc = pBt.allocateBtreePage(ref pRoot, ref pgnoRoot, 1, 0);
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



            public SqlResult sqlite3BtreeClearTable(int iTable, ref int pnChange)
            {
                SqlResult rc;
                BtShared pBt = this.pBt;
                using (this.scope())
                {
                    Debug.Assert(this.inTrans == TransType.TRANS_WRITE);
                    ///Invalidate all incrblob cursors open on table iTable (assuming iTable
                    ///is the root of a table b"- if it is not, the following call is
                    ///a no-op).
                    this.invalidateIncrblobCursors(0, 1);
                    rc = pBt.saveAllCursors((Pgno)iTable, null);
                    if (SqlResult.SQLITE_OK == rc)
                    {
                        rc = pBt.clearDatabasePage((Pgno)iTable, 0, ref pnChange);
                    }
                }
                return rc;
            }
            SqlResult btreeDropTable(Pgno iTable, ref int piMoved)
            {
                SqlResult rc;
                MemPage pPage = null;
                BtShared pBt = this.pBt;
                Debug.Assert(Sqlite3.sqlite3BtreeHoldsMutex(this));
                Debug.Assert(this.inTrans == TransType.TRANS_WRITE);
                ///It is illegal to drop a table if any cursors are open on the
                ///<param name="database. This is because in auto">vacuum mode the backend may</param>
                ///<param name="need to move another root">page to fill a gap left by the deleted</param>
                ///<param name="root page. If an open cursor was using this page a problem would">root page. If an open cursor was using this page a problem would</param>
                ///<param name="occur.">occur.</param>
                ///<param name=""></param>
                ///<param name="This error is caught long before control reaches this point.">This error is caught long before control reaches this point.</param>
                ///<param name=""></param>
                if (NEVER(pBt.pCursor))
                {
                    sqliteinth.sqlite3ConnectionBlocked(this.db, pBt.pCursor.pBtree.db);
                    return SqlResult.SQLITE_LOCKED_SHAREDCACHE;
                }

                rc = pBt.GetPage((Pgno)iTable, ref pPage, 0);
                if (rc != 0)
                    return rc;

                int Dummy0 = 0;
                rc = this.sqlite3BtreeClearTable((int)iTable, ref Dummy0);
                if (rc != 0)
                {
                    pPage.release();
                    return rc;
                }

                piMoved = 0;
                if (iTable > 1)
                {
#if SQLITE_OMIT_AUTOVACUUM
																																																																																																														freePage(pPage, ref rc);
releasePage(pPage);
#else
                    if (pBt.autoVacuum)
                    {
                        Pgno maxRootPgno = 0;
                        maxRootPgno = this.sqlite3BtreeGetMeta(BTreeProp.LARGEST_ROOT_PAGE);
                        if (iTable == maxRootPgno)
                        {
                            ///<param name="If the table being dropped is the table with the largest root">page</param>
                            ///<param name="number in the database, put the root page on the free list.">number in the database, put the root page on the free list.</param>
                            pPage.freePage(ref rc);
                            BTreeMethods.release(pPage);
                            if (rc != SqlResult.SQLITE_OK)
                            {
                                return rc;
                            }
                        }
                        else
                        {
                            ///<param name="The table being dropped does not have the largest root">page</param>
                            ///<param name="number in the database. So move the page that does into the">number in the database. So move the page that does into the</param>
                            ///<param name="gap left by the deleted root">page.</param>
                            MemPage pMove = new MemPage();
                            pPage.release();
                            rc = pBt.GetPage(maxRootPgno, ref pMove, 0);
                            if (rc != SqlResult.SQLITE_OK)
                            {
                                return rc;
                            }
                            rc = BTreeMethods.relocatePage(pBt, pMove, PTRMAP.ROOTPAGE, 0, iTable, 0);
                            pMove.release();
                            if (rc != SqlResult.SQLITE_OK)
                            {
                                return rc;
                            }
                            pMove = null;
                            rc = pBt.GetPage(maxRootPgno, ref pMove, 0);
                            pMove.freePage(ref rc);
                            pMove.release();
                            if (rc != SqlResult.SQLITE_OK)
                            {
                                return rc;
                            }
                            piMoved = (int)maxRootPgno;
                        }
                        ///
                        ///<summary>
                        ///</summary>
                        ///<param name="Set the new 'max">page' value in the database header. This</param>
                        ///<param name="is the old value less one, less one more if that happens to">is the old value less one, less one more if that happens to</param>
                        ///<param name="be a root">page number, less one again if that is the</param>
                        ///<param name="PENDING_BYTE_PAGE.">PENDING_BYTE_PAGE.</param>
                        ///<param name=""></param>
                        maxRootPgno--;
                        while (maxRootPgno == (pBt.PENDING_BYTE_PAGE) || pBt.PTRMAP_ISPAGE(maxRootPgno))
                        {
                            maxRootPgno--;
                        }
                        Debug.Assert(maxRootPgno != (pBt.PENDING_BYTE_PAGE));
                        rc = this.sqlite3BtreeUpdateMeta(BTreeProp.LARGEST_ROOT_PAGE, maxRootPgno);
                    }
                    else
                    {
                        pPage.freePage(ref rc);
                        pPage.release();
                    }
#endif
                }
                else
                {
                    ///If sqlite3BtreeDropTable was called on page 1.
                    ///This really never should happen except in a corrupt
                    ///database.
                    pPage.zeroPage(PTF.INTKEY | PTF.LEAF);
                    pPage.release();
                }
                return rc;
            }


            public SqlResult DropTable(int iTable, ref int piMoved)
            {
                SqlResult rc;
                using (this.scope())  
                    rc = this.btreeDropTable((u32)iTable, ref piMoved);
                this.Exit();
                return rc;
            }
            #endregion
        }
    }
}
