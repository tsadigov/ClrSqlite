using System;
using System.Diagnostics;
using i16=System.Int16;
using i64=System.Int64;
using u8=System.Byte;
using u16=System.UInt16;
using u32=System.UInt32;
using u64=System.UInt64;
using sqlite3_int64=System.Int64;
using Pgno=System.UInt32;
namespace Community.CsharpSqlite {
    using DbPage = Cache.PgHdr;
    using System.Text;
    using Metadata;
    using Community.CsharpSqlite.Os;
    using Community.CsharpSqlite.Paging;
    using Utils;



    ///
    ///<summary>
    ///A Btree handle
    ///
    ///A database connection contains a pointer to an instance of
    ///this object for every database file that it has open.  This structure
    ///is opaque to the database connection.  The database connection cannot
    ///see the internals of this structure and only deals with pointers to
    ///this structure.
    ///
    ///For some database files, the same underlying database cache might be
    ///shared between multiple connections.  In that case, each connection
    ///has it own instance of this object.  But each instance of this object
    ///points to the same BtShared object.  The database cache and the
    ///schema associated with the database file are all contained within
    ///the BtShared object.
    ///
    ///All fields in this structure are accessed under sqlite3.mutex.
    ///The pBt pointer itself may not be changed while there exists cursors
    ///in the referenced BtShared that point back to this Btree since those
    ///cursors have to go through this Btree to find their BtShared and
    ///they often do so without holding sqlite3.mutex.
    ///
    ///</summary>
    namespace tree
    {
        public class Btree:IBusyScope
        {
            //# define sqlite3BtreeLeave(X)
            public  void Exit()
            {
            }


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
            public void Enter()
            {
            }

           
#endif


            public Btree()
            {

            }


            public SqlResult sqlite3BtreeCreateTable(ref int piTable, int flags)
            {
                SqlResult rc;
                this.Enter();
                rc = BTreeMethods.btreeCreateTable(this, ref piTable, flags);
                this.Exit();
                return rc;
            }

            ///
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


            public Connection db;
            ///
            ///<summary>
            ///The database connection holding this Btree 
            ///</summary>
            public BtShared pBt;
            ///
            ///<summary>
            ///Sharable content of this Btree 
            ///</summary>
            public TransType inTrans;
            ///
            ///<summary>
            ///TRANS_NONE, TRANS_READ or TRANS_WRITE 
            ///</summary>
            public bool sharable;
            ///
            ///<summary>
            ///True if we can share pBt with another db 
            ///</summary>
            public bool locked;
            ///
            ///<summary>
            ///True if db currently has pBt locked 
            ///</summary>
            public int wantToLock;
            ///
            ///<summary>
            ///Number of nested calls to sqlite3BtreeEnter() 
            ///</summary>
            public int nBackup;
            ///
            ///<summary>
            ///Number of backup operations reading this btree 
            ///</summary>
            public Btree pNext;
            ///
            ///<summary>
            ///List of other sharable Btrees from the same db 
            ///</summary>
            public Btree pPrev;
            ///
            ///<summary>
            ///Back pointer of the same list 
            ///</summary>
#if !SQLITE_OMIT_SHARED_CACHE
																																																																								BtLock lock;              /* Object used to lock page 1 */
#endif
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
            public SqlResult sqlite3BtreeCopyFile(Btree pFrom)
            {
                SqlResult rc;
                sqlite3_backup b;
                this.Enter();
                pFrom.Enter();
                ///
                ///<summary>
                ///Set up an sqlite3_backup object. sqlite3_backup.pDestDb must be set
                ///to 0. This is used by the implementations of sqlite3_backup_step()
                ///and sqlite3_backup_finish() to detect that they are being called
                ///from this function, not directly by the user.
                ///
                ///</summary>
                b = new sqlite3_backup();
                // memset( &b, 0, sizeof( b ) );
                b.pSrcDb = pFrom.db;
                b.pSrc = pFrom;
                b.pDest = this;
                b.iNext = 1;
                ///
                ///<summary>
                ///0x7FFFFFFF is the hard limit for the number of pages in a database
                ///file. By passing this as the number of pages to copy to
                ///sqlite3_backup_step(), we can guarantee that the copy finishes
                ///within a single call (unless an error occurs). The Debug.Assert() statement
                ///</summary>
                ///<param name="checks this assumption "> (p.rc) should be set to either SQLITE_DONE</param>
                ///<param name="or an error code.">or an error code.</param>
                ///<param name=""></param>
                b.sqlite3_backup_step(0x7FFFFFFF);
                Debug.Assert(b.rc != SqlResult.SQLITE_OK);
                rc = b.sqlite3_backup_finish();
                if (rc == SqlResult.SQLITE_OK)
                {
                    this.pBt.pageSizeFixed = false;
                }
                pFrom.Exit();
                this.Exit();
                return rc;
            }
            public void clearAllSharedCacheTableLocks()
            {
            }
            public void downgradeAllSharedCacheTableLocks()
            {
            }
            public bool hasSharedCacheTableLock(Pgno b, int c, int d)
            {
                return true;
            }
            public bool hasReadConflicts(Pgno b)
            {
                return false;
            }
            public string sqlite3BtreeIntegrityCheck(///
                ///<summary>
                ///The btree to be checked 
                ///</summary>
            int[] aRoot,///
                ///<summary>
                ///An array of root pages numbers for individual trees 
                ///</summary>
            int nRoot,///
                ///<summary>
                ///Number of entries in aRoot[] 
                ///</summary>
            int mxErr,///
                ///<summary>
                ///Stop reporting errors after this many 
                ///</summary>
            ref int pnErr///
                ///<summary>
                ///Write number of errors seen to this variable 
                ///</summary>
            )
            {
                Pgno i;
                int nRef;
                IntegrityCk sCheck = new IntegrityCk();
                BtShared pBt = this.pBt;
                StringBuilder zErr = new StringBuilder(100);
                //char zErr[100];
                this.Enter();
                Debug.Assert(this.inTrans > (byte)TransType.TRANS_NONE && pBt.inTransaction > (byte)TransType.TRANS_NONE);
                nRef = pBt.pPager.sqlite3PagerRefcount();
                sCheck.pBt = pBt;
                sCheck.pPager = pBt.pPager;
                sCheck.nPage = sCheck.pBt.btreePagecount();
                sCheck.mxErr = mxErr;
                sCheck.nErr = 0;
                //sCheck.mallocFailed = 0;
                pnErr = 0;
                if (sCheck.nPage == 0)
                {
                    this.Exit();
                    return "";
                }
                sCheck.anRef = malloc_cs.sqlite3Malloc(sCheck.anRef, (int)sCheck.nPage + 1);
                //if( !sCheck.anRef ){
                //  pnErr = 1;
                //  sqlite3BtreeLeave(p);
                //  return 0;
                //}
                // for (i = 0; i <= sCheck.nPage; i++) { sCheck.anRef[i] = 0; }
                i = pBt.PENDING_BYTE_PAGE;
                if (i <= sCheck.nPage)
                {
                    sCheck.anRef[i] = 1;
                }
                io.sqlite3StrAccumInit(sCheck.errMsg, null, 1000, 20000);
                //sCheck.errMsg.useMalloc = 2;
                ///
                ///<summary>
                ///Check the integrity of the freelist
                ///
                ///</summary>
                sCheck.checkList(1, (int)Converter.sqlite3Get4byte(pBt.pPage1.aData, 32), (int)Converter.sqlite3Get4byte(pBt.pPage1.aData, 36), "Main freelist: ");
                ///
                ///<summary>
                ///Check all the tables.
                ///
                ///</summary>
                for (i = 0; (int)i < nRoot && sCheck.mxErr != 0; i++)
                {
                    if (aRoot[i] == 0)
                        continue;
#if !SQLITE_OMIT_AUTOVACUUM
                    if (pBt.autoVacuum && aRoot[i] > 1)
                    {
                        sCheck.checkPtrmap((u32)aRoot[i], PTRMAP.ROOTPAGE, 0, "");
                    }
#endif
                    sCheck.checkTreePage(aRoot[i], "List of tree roots: ", ref BTreeMethods.refNULL, ref BTreeMethods.refNULL, null, null);
                }
                ///
                ///<summary>
                ///Make sure every page in the file is referenced
                ///
                ///</summary>
                for (i = 1; i <= sCheck.nPage && sCheck.mxErr != 0; i++)
                {
#if SQLITE_OMIT_AUTOVACUUM
																																																																																																															if( sCheck.anRef[i]==null ){
checkAppendMsg(sCheck, 0, "Page %d is never used", i);
}
#else
                    ///
                    ///<summary>
                    ///</summary>
                    ///<param name="If the database supports auto">vacuum, make sure no tables contain</param>
                    ///<param name="references to pointer">map pages.</param>
                    if (sCheck.anRef[i] == 0 && (pBt.PTRMAP_PAGENO(i) != i || !pBt.autoVacuum))
                    {
                        sCheck.checkAppendMsg("", "Page %d is never used", i);
                    }
                    if (sCheck.anRef[i] != 0 && (pBt.PTRMAP_PAGENO(i) == i && pBt.autoVacuum))
                    {
                        sCheck.checkAppendMsg("", "Pointer map page %d is referenced", i);
                    }
#endif
                }
                ///
                ///<summary>
                ///Make sure this analysis did not leave any unref() pages.
                ///This is an internal consistency check; an integrity check
                ///of the integrity check.
                ///
                ///</summary>
                if (NEVER(nRef != pBt.pPager.sqlite3PagerRefcount()))
                {
                    sCheck.checkAppendMsg("", "Outstanding page count goes from %d to %d during this analysis", nRef, pBt.pPager.sqlite3PagerRefcount());
                }
                ///
                ///<summary>
                ///Clean  up and report errors.
                ///
                ///</summary>
                this.Exit();
                sCheck.anRef = null;
                // malloc_cs.sqlite3_free( ref sCheck.anRef );
                //if( sCheck.mallocFailed ){
                //  sqlite3StrAccumReset(sCheck.errMsg);
                //  pnErr = sCheck.nErr+1;
                //  return 0;
                //}
                pnErr = sCheck.nErr;
                if (sCheck.nErr == 0)
                    io.sqlite3StrAccumReset(sCheck.errMsg);
                return io.sqlite3StrAccumFinish(sCheck.errMsg);
            }

            private bool NEVER(bool p)
            {
                return Sqlite3.NEVER(p);
            }
            public Schema sqlite3BtreeSchema(int nBytes, dxFreeSchema xFree)
            {
                BtShared pBt = this.pBt;
                this.Enter();
                if (null == pBt.pSchema && nBytes != 0)
                {
                    pBt.pSchema = new Schema();
                    //sqlite3DbMallocZero(0, nBytes);
                    pBt.xFreeSchema = xFree;
                }
                this.Exit();
                return pBt.pSchema;
            }
            public SqlResult sqlite3BtreeSchemaLocked()
            {
                SqlResult rc;
                Debug.Assert(this.db.mutex.sqlite3_mutex_held());
                this.Enter();
                rc = this.querySharedCacheTableLock(sqliteinth.MASTER_ROOT, BtLockType.READ_LOCK);
                Debug.Assert(rc == SqlResult.SQLITE_OK || rc == SqlResult.SQLITE_LOCKED_SHAREDCACHE);
                this.Exit();
                return rc;
            }
            public SqlResult sqlite3BtreeSetVersion(int iVersion)
            {
                BtShared pBt = this.pBt;
                SqlResult rc;
                ///
                ///<summary>
                ///Return code 
                ///</summary>
                Debug.Assert(this.inTrans == TransType.TRANS_NONE);
                Debug.Assert(iVersion == 1 || iVersion == 2);
                ///
                ///<summary>
                ///If setting the version fields to 1, do not automatically open the
                ///WAL connection, even if the version fields are currently set to 2.
                ///
                ///</summary>
                pBt.doNotUseWAL = iVersion == 1;
                rc = this.sqlite3BtreeBeginTrans(0);
                if (rc == SqlResult.SQLITE_OK)
                {
                    u8[] aData = pBt.pPage1.aData;
                    if (aData[18] != (u8)iVersion || aData[19] != (u8)iVersion)
                    {
                        rc = this.sqlite3BtreeBeginTrans(2);
                        if (rc == SqlResult.SQLITE_OK)
                        {
                            rc = PagerMethods.sqlite3PagerWrite(pBt.pPage1.pDbPage);
                            if (rc == SqlResult.SQLITE_OK)
                            {
                                aData[18] = (u8)iVersion;
                                aData[19] = (u8)iVersion;
                            }
                        }
                    }
                }
                pBt.doNotUseWAL = false;
                return rc;
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
                        rc = BTreeMethods.clearDatabasePage(pBt, (Pgno)iTable, 0, ref pnChange);
                    }
                }
                return rc;
            }
            public SqlResult btreeDropTable(Pgno iTable, ref int piMoved)
            {
                SqlResult rc;
                MemPage pPage = null;
                BtShared pBt = this.pBt;
                Debug.Assert(Sqlite3.sqlite3BtreeHoldsMutex(this));
                Debug.Assert(this.inTrans == TransType.TRANS_WRITE);
                ///
                ///<summary>
                ///It is illegal to drop a table if any cursors are open on the
                ///</summary>
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
                rc = pBt.btreeGetPage((Pgno)iTable, ref pPage, 0);
                if (rc != 0)
                    return rc;
                int Dummy0 = 0;
                rc = this.sqlite3BtreeClearTable((int)iTable, ref Dummy0);
                if (rc != 0)
                {
                    BTreeMethods.releasePage(pPage);
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
                            BTreeMethods.freePage(pPage, ref rc);
                            BTreeMethods.releasePage(pPage);
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
                            BTreeMethods.releasePage(pPage);
                            rc = pBt.btreeGetPage(maxRootPgno, ref pMove, 0);
                            if (rc != SqlResult.SQLITE_OK)
                            {
                                return rc;
                            }
                            rc = BTreeMethods.relocatePage(pBt, pMove, PTRMAP.ROOTPAGE, 0, iTable, 0);
                            BTreeMethods.releasePage(pMove);
                            if (rc != SqlResult.SQLITE_OK)
                            {
                                return rc;
                            }
                            pMove = null;
                            rc = pBt.btreeGetPage(maxRootPgno, ref pMove, 0);
                            BTreeMethods.freePage(pMove, ref rc);
                            BTreeMethods.releasePage(pMove);
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
                        BTreeMethods.freePage(pPage, ref rc);
                        BTreeMethods.releasePage(pPage);
                    }
#endif
                }
                else
                {
                    ///
                    ///<summary>
                    ///If sqlite3BtreeDropTable was called on page 1.
                    ///This really never should happen except in a corrupt
                    ///database.
                    ///
                    ///</summary>
                    pPage.zeroPage(PTF.INTKEY | PTF.LEAF);
                    BTreeMethods.releasePage(pPage);
                }
                return rc;
            }

            private bool NEVER(BtCursor btCursor)
            {
                return Sqlite3.NEVER(btCursor);
            }
            public SqlResult sqlite3BtreeDropTable(int iTable, ref int piMoved)
            {
                SqlResult rc;
                this.Enter();
                rc = this.btreeDropTable((u32)iTable, ref piMoved);
                this.Exit();
                return rc;
            }

            /// <summary>
            /// ///Get the database meta information.
            ///
            ///Meta values are as follows:
            ///meta[0]   Schema cookie.  Changes with each schema change.
            ///meta[1]   File format of schema layer.
            ///meta[2]   Size of the page cache.
            ///meta[3]   Largest rootpage (auto/incr_vacuum mode)

            ///meta[4]   Db text encoding. 1:UTF-16BE
            ///meta[5]   User version
            ///meta[6]   Incremental vacuum mode
            ///meta[7]   unused
            ///meta[8]   unused</param>
            ///meta[9]   unused
            /// </summary>
            /*public enum DtabaseMetaInfrmation
            {
                SchemaCookie = 1,
                FileFormatOfSchemaLayer,
                SizeOfThePageCache,
                LargestRootPage,
                DbTextEncoding,
                UserVersion,
                IncrementalVacuumMode,
                Unused7,
                Unused8,
                Unused9
            }*/
            public u32 sqlite3BtreeGetMeta(BTreeProp prop)
            {
                var idx = (int) prop;
                u32 pMeta;
                BtShared pBt = this.pBt;
                using (this.scope())
                {
                    Debug.Assert(this.inTrans > (byte)TransType.TRANS_NONE);
                    Debug.Assert(SqlResult.SQLITE_OK == this.querySharedCacheTableLock(sqliteinth.MASTER_ROOT, BtLockType.READ_LOCK));
                    Debug.Assert(pBt.pPage1 != null);
                    Debug.Assert(idx >= 0 && idx <= 15);
                    pMeta = Converter.sqlite3Get4byte(pBt.pPage1.aData, 36 + idx * 4);
                    //If auto-vacuum
                    //database, mark the database as read-only.  
                    return pMeta;
#if SQLITE_OMIT_AUTOVACUUM	
                    if( idx==BTREE_LARGEST_ROOT_PAGE && pMeta>0 ) pBt.readOnly = 1;
#endif
                }
            }
            public SqlResult sqlite3BtreeUpdateMeta(BTreeProp prop, u32 iMeta)
            {
                var idx = (int)prop;
                BtShared pBt = this.pBt;
                byte[] pP1;
                SqlResult rc;
                Debug.Assert(idx >= 1 && idx <= 15);
                this.Enter();
                Debug.Assert(this.inTrans == TransType.TRANS_WRITE);
                Debug.Assert(pBt.pPage1 != null);
                pP1 = pBt.pPage1.aData;
                rc = PagerMethods.sqlite3PagerWrite(pBt.pPage1.pDbPage);
                if (rc == SqlResult.SQLITE_OK)
                {
                    Converter.sqlite3Put4byte(pP1, 36 + idx * 4, iMeta);
#if !SQLITE_OMIT_AUTOVACUUM
                    if (prop == BTreeProp.INCR_VACUUM)
                    {
                        Debug.Assert(pBt.autoVacuum || iMeta == 0);
                        Debug.Assert(iMeta == 0 || iMeta == 1);
                        pBt.incrVacuum = iMeta != 0;
                    }
#endif
                }
                this.Exit();
                return rc;
            }
            public SqlResult querySharedCacheTableLock(Pgno iTab, BtLockType eLock)
            {
                return SqlResult.SQLITE_OK;
            }
            public void invalidateIncrblobCursors(i64 y, int z)
            {
            }

            /// <summary>
            /// connection confguration
            /// </summary>
            /// <param name="mxPage"></param>
            /// <returns></returns>
            public SqlResult SetCacheSize(int mxPage)
            {
                BtShared pBt = this.pBt;
                Debug.Assert(this.db.mutex.sqlite3_mutex_held());
                this.Enter();
                pBt.pPager.sqlite3PagerSetCachesize(mxPage);
                this.Exit();
                return SqlResult.SQLITE_OK;
            }
            public SqlResult sqlite3BtreeSetSafetyLevel(///
                ///<summary>
                ///The btree to set the safety level on 
                ///</summary>
            int level,///
                ///<summary>
                ///PRAGMA synchronous.  1=OFF, 2=NORMAL, 3=FULL 
                ///</summary>
            int fullSync,///
                ///<summary>
                ///PRAGMA fullfsync. 
                ///</summary>
            int ckptFullSync///
                ///<summary>
                ///PRAGMA checkpoint_fullfync 
                ///</summary>
            )
            {
                BtShared pBt = this.pBt;
                Debug.Assert(this.db.mutex.sqlite3_mutex_held());
                Debug.Assert(level >= 1 && level <= 3);
                this.Enter();
                pBt.pPager.sqlite3PagerSetSafetyLevel(level, fullSync, ckptFullSync);
                this.Exit();
                return SqlResult.SQLITE_OK;
            }
            public int GetPageSize()
            {
                return (int)this.pBt.pageSize;
            }
            public SqlResult sqlite3BtreeSetPageSize(int pageSize, int nReserve, int iFix)
            {
                SqlResult rc = SqlResult.SQLITE_OK;
                BtShared pBt = this.pBt;
                Debug.Assert(nReserve >= -1 && nReserve <= 255);
                this.Enter();
                if (pBt.pageSizeFixed)
                {
                    this.Exit();
                    return SqlResult.SQLITE_READONLY;
                }
                if (nReserve < 0)
                {
                    nReserve = (int)(pBt.pageSize - pBt.usableSize);
                }
                Debug.Assert(nReserve >= 0 && nReserve <= 255);
                if (pageSize >= 512 && pageSize <= Limits.SQLITE_MAX_PAGE_SIZE && ((pageSize - 1) & pageSize) == 0)
                {
                    Debug.Assert((pageSize & 7) == 0);
                    Debug.Assert(null == pBt.pPage1 && null == pBt.pCursor);
                    pBt.pageSize = (u32)pageSize;
                    //        freeTempSpace(pBt);
                }
                rc = pBt.pPager.sqlite3PagerSetPagesize(ref pBt.pageSize, nReserve);
                pBt.usableSize = (u16)(pBt.pageSize - nReserve);
                if (iFix != 0)
                    pBt.pageSizeFixed = true;
                this.Exit();
                return rc;
            }
            public Pgno sqlite3BtreeLastPage()
            {
                Debug.Assert(Sqlite3.sqlite3BtreeHoldsMutex(this));
                Debug.Assert(((this.pBt.nPage) & 0x8000000) == 0);
                return (Pgno)this.pBt.btreePagecount();
            }
            public int sqlite3BtreeSyncDisabled()
            {
                BtShared pBt = this.pBt;
                int rc;
                Debug.Assert(this.db.mutex.sqlite3_mutex_held());
                this.Enter();
                Debug.Assert(pBt != null && pBt.pPager != null);
                rc = pBt.pPager.sqlite3PagerNosync() ? 1 : 0;
                this.Exit();
                return rc;
            }
            public int GetReserve()
            {
                int n;
                this.Enter();
                n = (int)(this.pBt.pageSize - this.pBt.usableSize);
                this.Exit();
                return n;
            }

            /// <summary>
            /// connection confguration
            /// </summary>
            /// <param name="mxPage"></param>
            /// <returns></returns>
            public Pgno GetMaxPageCount(int mxPage)
            {
                Pgno n;
                this.Enter();
                n = this.pBt.pPager.sqlite3PagerMaxPageCount(mxPage);
                this.Exit();
                return n;
            }
            public int sqlite3BtreeSecureDelete(int newFlag)
            {
                int b;
                if (this == null)
                    return 0;
                this.Enter();
                if (newFlag >= 0)
                {
                    this.pBt.secureDelete = (newFlag != 0);
                }
                b = this.pBt.secureDelete ? 1 : 0;
                this.Exit();
                return b;
            }
            public SqlResult sqlite3BtreeSetAutoVacuum(int autoVacuum)
            {
#if SQLITE_OMIT_AUTOVACUUM
																																																																												return SQLITE_READONLY;
#else
                BtShared pBt = this.pBt;
                SqlResult rc = SqlResult.SQLITE_OK;
                u8 av = (u8)autoVacuum;
                this.Enter();
                if (pBt.pageSizeFixed && (av != 0) != pBt.autoVacuum)
                {
                    rc = SqlResult.SQLITE_READONLY;
                }
                else
                {
                    pBt.autoVacuum = av != 0;
                    pBt.incrVacuum = av == 2;
                }
                this.Exit();
                return rc;
#endif
            }
            public int GetAutoVacuum()
            {
#if SQLITE_OMIT_AUTOVACUUM
																																																																												return BTREE_AUTOVACUUM_NONE;
#else
                int rc;
                this.Enter();
                rc = ((!this.pBt.autoVacuum) ? Sqlite3.BTREE_AUTOVACUUM_NONE : (!this.pBt.incrVacuum) ? Sqlite3.BTREE_AUTOVACUUM_FULL : Sqlite3.BTREE_AUTOVACUUM_INCR);
                this.Exit();
                return rc;
#endif
            }
            public SqlResult sqlite3BtreeBeginTrans(int wrflag)
            {
                BtShared pBt = this.pBt;
                SqlResult rc = SqlResult.SQLITE_OK;
                this.Enter();
                Sqlite3.btreeIntegrity(this);
                ///<param name="If the btree is already in a write">transaction, or it</param>
                ///<param name="is already in a read">transaction</param>
                ///<param name="is requested, this is a no">op.</param>
                ///<param name=""></param>
                if (this.inTrans == TransType.TRANS_WRITE || (this.inTrans == TransType.TRANS_READ && 0 == wrflag))
                {
                    goto trans_begun;
                }
                ///
                ///<summary>
                ///</summary>
                ///<param name="Write transactions are not possible on a read">only database </param>
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
                ///<param name="Any read">lock on</param>
                ///<param name="page 1. So if some other shared">lock</param>
                ///<param name="on page 1, the transaction cannot be opened. ">on page 1, the transaction cannot be opened. </param>
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
                    while (pBt.pPage1 == null && SqlResult.SQLITE_OK == (rc = BTreeMethods.lockBtree(pBt)))
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
                                rc = BTreeMethods.newDatabase(pBt);
                            }
                        }
                    }
                    if (rc != SqlResult.SQLITE_OK)
                    {
                        BTreeMethods.unlockBtreeIfUnused(pBt);
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
            public SqlResult sqlite3BtreeIncrVacuum()
            {
                SqlResult rc;
                BtShared pBt = this.pBt;
                this.Enter();
                Debug.Assert(pBt.inTransaction == TransType.TRANS_WRITE && this.inTrans == TransType.TRANS_WRITE);
                if (!pBt.autoVacuum)
                {
                    rc = SqlResult.SQLITE_DONE;
                }
                else
                {
                    pBt.invalidateAllOverflowCache();
                    rc = BTreeMethods.incrVacuumStep(pBt, 0, pBt.btreePagecount());
                    if (rc == SqlResult.SQLITE_OK)
                    {
                        rc = PagerMethods.sqlite3PagerWrite(pBt.pPage1.pDbPage);
                        Converter.sqlite3Put4byte(pBt.pPage1.aData, (u32)28, pBt.nPage);
                        //put4byte(&pBt->pPage1->aData[28], pBt->nPage);
                    }
                }
                this.Exit();
                return rc;
            }
            public SqlResult sqlite3BtreeCommitPhaseOne(string zMaster)
            {
                SqlResult rc = SqlResult.SQLITE_OK;
                if (this.inTrans == TransType.TRANS_WRITE)
                {
                    BtShared pBt = this.pBt;
                    this.Enter();
#if !SQLITE_OMIT_AUTOVACUUM
                    if (pBt.autoVacuum)
                    {
                        rc = BTreeMethods.autoVacuumCommit(pBt);
                        if (rc != SqlResult.SQLITE_OK)
                        {
                            this.Exit();
                            return rc;
                        }
                    }
#endif
                    rc = pBt.pPager.sqlite3PagerCommitPhaseOne(zMaster, false);
                    this.Exit();
                }
                return rc;
            }
            public void btreeEndTransaction()
            {
                BtShared pBt = this.pBt;
                Debug.Assert(Sqlite3.sqlite3BtreeHoldsMutex(this));
                pBt.btreeClearHasContent();
                if (this.inTrans > TransType.TRANS_NONE && this.db.activeVdbeCnt > 1)
                {
                    ///
                    ///<summary>
                    ///If there are other active statements that belong to this database
                    ///</summary>
                    ///<param name="handle, downgrade to a read">only transaction. The other statements</param>
                    ///<param name="may still be reading from the database.  ">may still be reading from the database.  </param>
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
                    BTreeMethods.unlockBtreeIfUnused(pBt);
                }
                Sqlite3.btreeIntegrity(this);
            }
            public SqlResult sqlite3BtreeCommitPhaseTwo(int bCleanup)
            {
                if (this.inTrans == TransType.TRANS_NONE)
                    return SqlResult.SQLITE_OK;
                this.Enter();
                Sqlite3.btreeIntegrity(this);
                ///
                ///<summary>
                ///</summary>
                ///<param name="If the handle has a write">btrees</param>
                ///<param name="transaction and set the shared state to TRANS_READ.">transaction and set the shared state to TRANS_READ.</param>
                ///<param name=""></param>
                if (this.inTrans == TransType.TRANS_WRITE)
                {
                    SqlResult rc;
                    BtShared pBt = this.pBt;
                    Debug.Assert(pBt.inTransaction == TransType.TRANS_WRITE);
                    Debug.Assert(pBt.nTransaction > 0);
                    rc = pBt.pPager.sqlite3PagerCommitPhaseTwo();
                    if (rc != SqlResult.SQLITE_OK && bCleanup == 0)
                    {
                        this.Exit();
                        return rc;
                    }
                    pBt.inTransaction = TransType.TRANS_READ;
                }
                this.btreeEndTransaction();
                this.Exit();
                return SqlResult.SQLITE_OK;
            }
            public SqlResult sqlite3BtreeCommit()
            {
                SqlResult rc;
                this.Enter();
                rc = this.sqlite3BtreeCommitPhaseOne(null);
                if (rc == SqlResult.SQLITE_OK)
                {
                    rc = this.sqlite3BtreeCommitPhaseTwo(0);
                }
                this.Exit();
                return rc;
            }
            public void sqlite3BtreeTripAllCursors(SqlResult errCode)
            {
                BtCursor p;
                this.Enter();
                for (p = this.pBt.pCursor; p != null; p = p.pNext)
                {
                    int i;
                    p.sqlite3BtreeClearCursor();
                    p.State = BtCursorState.CURSOR_FAULT;
                    p.skipNext = (int)errCode;
                    for (i = 0; i <= p.iPage; i++)
                    {
                        BTreeMethods.releasePage(p.apPage[i]);
                        p.apPage[i] = null;
                    }
                }
                this.Exit();
            }
            public SqlResult sqlite3BtreeRollback()
            {
                SqlResult rc;
                BtShared pBt = this.pBt;
                MemPage pPage1 = new MemPage();
                this.Enter();
                rc = pBt.saveAllCursors(0, null);
#if !SQLITE_OMIT_SHARED_CACHE
																																																																												if( rc!=SqlResult.SQLITE_OK ){
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
                Sqlite3.btreeIntegrity(this);
                if (this.inTrans == TransType.TRANS_WRITE)
                {
                    SqlResult rc2;
                    Debug.Assert(TransType.TRANS_WRITE == pBt.inTransaction);
                    rc2 = pBt.pPager.sqlite3PagerRollback();
                    if (rc2 != SqlResult.SQLITE_OK)
                    {
                        rc = rc2;
                    }
                    ///
                    ///<summary>
                    ///The rollback may have destroyed the pPage1.aData value.  So
                    ///call btreeGetPage() on page 1 again to make
                    ///sure pPage1.aData is set correctly. 
                    ///</summary>
                    if (pBt.btreeGetPage(1, ref pPage1, 0) == SqlResult.SQLITE_OK)
                    {
                        Pgno nPage = Converter.sqlite3Get4byte(pPage1.aData, 28);
                        sqliteinth.testcase(nPage == 0);
                        if (nPage == 0)
                            pBt.pPager.sqlite3PagerPagecount(out nPage);
                        sqliteinth.testcase(pBt.nPage != nPage);
                        pBt.nPage = nPage;
                        BTreeMethods.releasePage(pPage1);
                    }
                    Debug.Assert(BTreeMethods.countWriteCursors(pBt) == 0);
                    pBt.inTransaction = TransType.TRANS_READ;
                }
                this.btreeEndTransaction();
                this.Exit();
                return rc;
            }
            public SqlResult sqlite3BtreeBeginStmt(int iStatement)
            {
                SqlResult rc;
                BtShared pBt = this.pBt;
                this.Enter();
                Debug.Assert(this.inTrans == TransType.TRANS_WRITE);
                Debug.Assert(!pBt.readOnly);
                Debug.Assert(iStatement > 0);
                Debug.Assert(iStatement > this.db.nSavepoint);
                Debug.Assert(pBt.inTransaction == TransType.TRANS_WRITE);
                ///
                ///<summary>
                ///At the pager level, a statement transaction is a savepoint with
                ///an index greater than all savepoints created explicitly using
                ///SQL statements. It is illegal to open, release or rollback any
                ///such savepoints while the statement transaction savepoint is active.
                ///
                ///</summary>
                rc = pBt.pPager.sqlite3PagerOpenSavepoint(iStatement);
                this.Exit();
                return rc;
            }
            public SqlResult sqlite3BtreeSavepoint(int op, int iSavepoint)
            {
                SqlResult rc = SqlResult.SQLITE_OK;
                if (this != null && this.inTrans == TransType.TRANS_WRITE)
                {
                    BtShared pBt = this.pBt;
                    Debug.Assert(op == sqliteinth.SAVEPOINT_RELEASE || op == sqliteinth.SAVEPOINT_ROLLBACK);
                    Debug.Assert(iSavepoint >= 0 || (iSavepoint == -1 && op == sqliteinth.SAVEPOINT_ROLLBACK));
                    this.Enter();
                    rc = pBt.pPager.sqlite3PagerSavepoint(op, iSavepoint);
                    if (rc == SqlResult.SQLITE_OK)
                    {
                        if (iSavepoint < 0 && pBt.initiallyEmpty)
                            pBt.nPage = 0;
                        rc = BTreeMethods.newDatabase(pBt);
                        pBt.nPage = Converter.sqlite3Get4byte(pBt.pPage1.aData, 28);
                        ///
                        ///<summary>
                        ///The database size was written into the offset 28 of the header
                        ///when the transaction started, so we know that the value at offset
                        ///28 is nonzero. 
                        ///</summary>
                        Debug.Assert(pBt.nPage > 0);
                    }
                    this.Exit();
                }
                return rc;
            }
            public SqlResult btreeCursor(///
                ///<summary>
                ///The btree 
                ///</summary>
            int iTable,///
                ///<summary>
                ///Root page of table to open 
                ///</summary>
            CursorMode wrFlag,///
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
            )
            {
                BtShared pBt = this.pBt;
                ///<param name="Shared b">tree handle </param>
                Debug.Assert(Sqlite3.sqlite3BtreeHoldsMutex(this));
                Debug.Assert(wrFlag == CursorMode.ReadOnly || wrFlag == CursorMode.ReadWrite);
                ///The following Debug.Assert statements verify that if this is a sharable
                ///<param name="b">tree database, the connection is holding the required table locks,</param>
                ///<param name="and that no other connection has any open cursor that conflicts with">and that no other connection has any open cursor that conflicts with</param>
                ///<param name="this lock.  ">this lock.  </param>
                Debug.Assert(this.hasSharedCacheTableLock((u32)iTable, pKeyInfo != null ? 1 : 0, (int)wrFlag + 1));
                Debug.Assert(wrFlag == 0 || !this.hasReadConflicts((u32)iTable));
                ///Assert that the caller has opened the required transaction. 
                Debug.Assert(this.inTrans > TransType.TRANS_NONE);
                Debug.Assert(wrFlag == 0 || this.inTrans == TransType.TRANS_WRITE);
                Debug.Assert(pBt.pPage1 != null && pBt.pPage1.aData != null);
                if (NEVER(wrFlag != 0 && pBt.readOnly))
                {
                    return SqlResult.SQLITE_READONLY;
                }
                if (iTable == 1 && pBt.btreePagecount() == 0)
                {
                    return SqlResult.SQLITE_EMPTY;
                }
                ///Now that no other errors can occur, finish filling in the BtCursor
                ///variables and link the cursor into the BtShared list.  
                pCur.pgnoRoot = (Pgno)iTable;
                pCur.iPage = -1;
                pCur.pKeyInfo = pKeyInfo;
                pCur.pBtree = this;
                pCur.pBt = pBt;
                pCur.wrFlag = wrFlag;
                pCur.pNext = pBt.pCursor;
                if (pCur.pNext != null)
                {
                    pCur.pNext.pPrev = pCur;
                }
                pBt.pCursor = pCur;
                pCur.State = BtCursorState.CURSOR_INVALID;
                pCur.cachedRowid = 0;
                return SqlResult.SQLITE_OK;
            }
            public SqlResult sqlite3BtreeCursor(///
                ///<summary>
                ///The btree 
                ///</summary>
            int iTable,///
                ///<summary>
                ///Root page of table to open 
                ///</summary>
            CursorMode wrFlag,///
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
            )
            {           
                using (this.scope())
                    return this.btreeCursor(iTable, wrFlag, pKeyInfo, pCur);
            }
            public Pager sqlite3BtreePager()
            {
                return this.pBt.pPager;
            }
            


        }












        ///
        ///<summary>
        ///The second parameter to sqlite3BtreeGetMeta or sqlite3BtreeUpdateMeta
        ///should be one of the following values. The integer values are assigned
        ///to constants so that the offset of the corresponding field in an
        ///SQLite database header may be found using the following formula:
        ///
        ///offset = 36 + (idx * 4)
        ///
        ///</summary>
        ///<param name="For example, the free">count field is located at byte offset 36 of</param>
        ///<param name="the database file header. The incr">flag field is located at</param>
        ///<param name="byte offset 64 (== 36+4*7).">byte offset 64 (== 36+4*7).</param>
        ///<param name=""></param>
        ///DtabaseMetaInfrmation
    public enum BTreeProp:byte
    {
        FREE_PAGE_COUNT = 0,
        SCHEMA_VERSION = 1,
        FILE_FORMAT = 2,
        DEFAULT_CACHE_SIZE = 3,
        LARGEST_ROOT_PAGE = 4,
        TEXT_ENCODING = 5,
        USER_VERSION = 6,
        INCR_VACUUM = 7
    }
        ///Meta values are as follows:
        ///meta[0]   Schema cookie.  Changes with each schema change.
        ///meta[1]   File format of schema layer.
        ///meta[2]   Size of the page cache.
        ///meta[3]   Largest rootpage (auto/incr_vacuum mode)

        ///meta[4]   Db text encoding. 1:UTF-16BE
        ///meta[5]   User version
        ///meta[6]   Incremental vacuum mode
        ///meta[7]   unused
        ///meta[8]   unused</param>
        ///meta[9]   unused

        /*
        
            const int BTREE_FREE_PAGE_COUNT = 0;
    const int BTREE_SCHEMA_VERSION = 1;
    const int BTREE_FILE_FORMAT = 2;
    const int BTREE_DEFAULT_CACHE_SIZE = 3;
    const int BTREE_LARGEST_ROOT_PAGE = 4;
    const int BTREE_TEXT_ENCODING = 5;
    const int BTREE_USER_VERSION = 6;
    const int BTREE_INCR_VACUUM = 7;
        */
    }
}
