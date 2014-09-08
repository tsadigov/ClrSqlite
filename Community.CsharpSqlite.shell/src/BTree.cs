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
	using DbPage=Sqlite3.PgHdr;
	using System.Text;
	public partial class Sqlite3 {

		///<summary>
		/// Maximum depth of an SQLite B-Tree structure. Any B-Tree deeper than
		/// this will be declared corrupt. This value is calculated based on a
		/// maximum database size of 2^31 pages a minimum fanout of 2 for a
		/// root-node and 3 for all other internal nodes.
		///
		/// If a tree that appears to be taller than this is encountered, it is
		/// assumed that the database is corrupt.
		///
		///</summary>
		//#define BTCURSOR_MAX_DEPTH 20
		const int BTCURSOR_MAX_DEPTH=20;



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
        public class Btree
        {
            public Btree()
            {

            }


            public int sqlite3BtreeCreateTable( ref int piTable, int flags)
            {
                Btree p = this;
                int rc;
                sqlite3BtreeEnter(p);
                rc = btreeCreateTable(p, ref piTable, flags);
                sqlite3BtreeLeave(p);
                return rc;
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
            //sqlite3BtreeOpen
            public static int Open(sqlite3_vfs pVfs,///
                ///VFS to use for this b"tree 
            string zFilename,
                ///Name of the file containing the BTree database 
            sqlite3 db,
                ///Associated database handle 
            ref Btree ppBtree,
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
                ///Shared part of btree structure 
                BtShared pBt = null;
                ///Handle to return 
                Btree p;
                ///Prevents a race condition. Ticket #3537 
                sqlite3_mutex mutexOpen = null;
                ///Result code from this function 
                int rc = SQLITE_OK;
                ///Byte of unused space on each page 
                u8 nReserve;
                ///Database header content 
                ///True if opening an ephemeral, temporary database 
                byte[] zDbHeader = new byte[100];
                //zFilename==0 || zFilename[0]==0;
                ///<param name="Set the variable isMemdb to true for an in">memory database, or </param>
                ///<param name="false for a file">based database.</param>
                bool isTempDb = String.IsNullOrEmpty(zFilename);
#if SQLITE_OMIT_MEMORYDB
																																																																											bool isMemdb = false;
#else
                bool isMemdb = (zFilename == ":memory:") || (isTempDb && sqlite3TempInMemory(db));
#endif
                Debug.Assert(db != null);
                Debug.Assert(pVfs != null);
                Debug.Assert(sqlite3_mutex_held(db.mutex));
                Debug.Assert((flags & 0xff) == flags);
                ///
                ///<summary>
                ///flags fit in 8 bits 
                ///</summary>
                ///
                ///<summary>
                ///Only a BTREE_SINGLE database can be BTREE_UNORDERED 
                ///</summary>
                Debug.Assert((flags & BTREE_UNORDERED) == 0 || (flags & BTREE_SINGLE) != 0);
                ///
                ///<summary>
                ///A BTREE_SINGLE database is always a temporary and/or ephemeral 
                ///</summary>
                Debug.Assert((flags & BTREE_SINGLE) == 0 || isTempDb);
                if ((db.flags & SQLITE_NoReadlock) != 0)
                {
                    flags |= BTREE_NO_READLOCK;
                }
                if (isMemdb)
                {
                    flags |= BTREE_MEMORY;
                }
                if ((vfsFlags & SQLITE_OPEN_MAIN_DB) != 0 && (isMemdb || isTempDb))
                {
                    vfsFlags = (vfsFlags & ~SQLITE_OPEN_MAIN_DB) | SQLITE_OPEN_TEMP_DB;
                }
                p = new Btree();
                //sqlite3MallocZero(sizeof(Btree));
                //if( !p ){
                //  return SQLITE_NOMEM;
                //}
                p.inTrans = TransType.TRANS_NONE;
                p.db = db;
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
                if (pBt == null)
                {
                    ///
                    ///<summary>
                    ///The following asserts make sure that structures used by the btree are
                    ///the right size.  This is to guard against size changes that result
                    ///when compiling on a different architecture.
                    ///
                    ///</summary>
                    Debug.Assert(sizeof(i64) == 8 || sizeof(i64) == 4);
                    Debug.Assert(sizeof(u64) == 8 || sizeof(u64) == 4);
                    Debug.Assert(sizeof(u32) == 4);
                    Debug.Assert(sizeof(u16) == 2);
                    Debug.Assert(sizeof(Pgno) == 4);
                    pBt = new BtShared();
                    //sqlite3MallocZero( sizeof(pBt) );
                    //if( pBt==null ){
                    //  rc = SQLITE_NOMEM;
                    //  goto btree_open_out;
                    //}
                    rc = sqlite3PagerOpen(pVfs, out pBt.pPager, zFilename, EXTRA_SIZE, flags, vfsFlags, pageReinit);
                    if (rc == SQLITE_OK)
                    {
                        rc = pBt.pPager.sqlite3PagerReadFileheader(zDbHeader.Length, zDbHeader);
                    }
                    if (rc != SQLITE_OK)
                    {
                        goto btree_open_out;
                    }
                    pBt.openFlags = (u8)flags;
                    pBt.db = db;
                    pBt.pPager.sqlite3PagerSetBusyhandler(btreeInvokeBusyHandler, pBt);
                    p.pBt = pBt;
                    pBt.pCursor = null;
                    pBt.pPage1 = null;
                    pBt.readOnly = pBt.pPager.sqlite3PagerIsreadonly();
#if SQLITE_SECURE_DELETE
																																																																																																				pBt.secureDelete = true;
#endif
                    pBt.pageSize = (u32)((zDbHeader[16] << 8) | (zDbHeader[17] << 16));
                    if (pBt.pageSize < 512 || pBt.pageSize > SQLITE_MAX_PAGE_SIZE || ((pBt.pageSize - 1) & pBt.pageSize) != 0)
                    {
                        pBt.pageSize = 0;
#if !SQLITE_OMIT_AUTOVACUUM
                        ///
                        ///<summary>
                        ///</summary>
                        ///<param name="If the magic name ":memory:" will create an in">memory database, then</param>
                        ///<param name="leave the autoVacuum mode at 0 (do not auto">vacuum), even if</param>
                        ///<param name="SQLITE_DEFAULT_AUTOVACUUM is true. On the other hand, if">SQLITE_DEFAULT_AUTOVACUUM is true. On the other hand, if</param>
                        ///<param name="SQLITE_OMIT_MEMORYDB has been defined, then ":memory:" is just a">SQLITE_OMIT_MEMORYDB has been defined, then ":memory:" is just a</param>
                        ///<param name="regular file">vacuum applies as per normal.</param>
                        if (zFilename != "" && !isMemdb)
                        {
                            pBt.autoVacuum = (SQLITE_DEFAULT_AUTOVACUUM != 0);
                            pBt.incrVacuum = (SQLITE_DEFAULT_AUTOVACUUM == 2);
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
                ppBtree = p;
            btree_open_out:
                if (rc != SQLITE_OK)
                {
                    if (pBt != null && pBt.pPager != null)
                    {
                        pBt.pPager.sqlite3PagerClose();
                    }
                    pBt = null;
                    //    sqlite3_free(ref pBt);
                    p = null;
                    //    sqlite3_free(ref p);
                    ppBtree = null;
                }
                else
                {
                    ///
                    ///<summary>
                    ///</summary>
                    ///<param name="If the B">cache size to the</param>
                    ///<param name="default value. Except, when opening on an existing shared pager">cache,</param>
                    ///<param name="do not change the pager">cache size.</param>
                    ///<param name=""></param>
                    if (p.sqlite3BtreeSchema(0, null) == null)
                    {
                        p.pBt.pPager.sqlite3PagerSetCachesize(SQLITE_DEFAULT_CACHE_SIZE);
                    }
                }
                if (mutexOpen != null)
                {
                    Debug.Assert(sqlite3_mutex_held(mutexOpen));
                    sqlite3_mutex_leave(mutexOpen);
                }
                return rc;
            }


            public sqlite3 db;
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
            public int sqlite3BtreeCopyFile(Btree pFrom)
            {
                int rc;
                sqlite3_backup b;
                sqlite3BtreeEnter(this);
                sqlite3BtreeEnter(pFrom);
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
                Debug.Assert(b.rc != SQLITE_OK);
                rc = b.sqlite3_backup_finish();
                if (rc == SQLITE_OK)
                {
                    this.pBt.pageSizeFixed = false;
                }
                sqlite3BtreeLeave(pFrom);
                sqlite3BtreeLeave(this);
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
                sqlite3BtreeEnter(this);
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
                    sqlite3BtreeLeave(this);
                    return "";
                }
                sCheck.anRef = sqlite3Malloc(sCheck.anRef, (int)sCheck.nPage + 1);
                //if( !sCheck.anRef ){
                //  pnErr = 1;
                //  sqlite3BtreeLeave(p);
                //  return 0;
                //}
                // for (i = 0; i <= sCheck.nPage; i++) { sCheck.anRef[i] = 0; }
                i = PENDING_BYTE_PAGE(pBt);
                if (i <= sCheck.nPage)
                {
                    sCheck.anRef[i] = 1;
                }
                sqlite3StrAccumInit(sCheck.errMsg, null, 1000, 20000);
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
                        sCheck.checkPtrmap((u32)aRoot[i], PTRMAP_ROOTPAGE, 0, "");
                    }
#endif
                    sCheck.checkTreePage(aRoot[i], "List of tree roots: ", ref refNULL, ref refNULL, null, null);
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
                    if (sCheck.anRef[i] == 0 && (PTRMAP_PAGENO(pBt, i) != i || !pBt.autoVacuum))
                    {
                        sCheck.checkAppendMsg("", "Page %d is never used", i);
                    }
                    if (sCheck.anRef[i] != 0 && (PTRMAP_PAGENO(pBt, i) == i && pBt.autoVacuum))
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
                sqlite3BtreeLeave(this);
                sCheck.anRef = null;
                // sqlite3_free( ref sCheck.anRef );
                //if( sCheck.mallocFailed ){
                //  sqlite3StrAccumReset(sCheck.errMsg);
                //  pnErr = sCheck.nErr+1;
                //  return 0;
                //}
                pnErr = sCheck.nErr;
                if (sCheck.nErr == 0)
                    sqlite3StrAccumReset(sCheck.errMsg);
                return sqlite3StrAccumFinish(sCheck.errMsg);
            }
            public Schema sqlite3BtreeSchema(int nBytes, dxFreeSchema xFree)
            {
                BtShared pBt = this.pBt;
                sqlite3BtreeEnter(this);
                if (null == pBt.pSchema && nBytes != 0)
                {
                    pBt.pSchema = new Schema();
                    //sqlite3DbMallocZero(0, nBytes);
                    pBt.xFreeSchema = xFree;
                }
                sqlite3BtreeLeave(this);
                return pBt.pSchema;
            }
            public int sqlite3BtreeSchemaLocked()
            {
                int rc;
                Debug.Assert(sqlite3_mutex_held(this.db.mutex));
                sqlite3BtreeEnter(this);
                rc = this.querySharedCacheTableLock(MASTER_ROOT, READ_LOCK);
                Debug.Assert(rc == SQLITE_OK || rc == SQLITE_LOCKED_SHAREDCACHE);
                sqlite3BtreeLeave(this);
                return rc;
            }
            public int sqlite3BtreeSetVersion(int iVersion)
            {
                BtShared pBt = this.pBt;
                int rc;
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
                if (rc == SQLITE_OK)
                {
                    u8[] aData = pBt.pPage1.aData;
                    if (aData[18] != (u8)iVersion || aData[19] != (u8)iVersion)
                    {
                        rc = this.sqlite3BtreeBeginTrans(2);
                        if (rc == SQLITE_OK)
                        {
                            rc = sqlite3PagerWrite(pBt.pPage1.pDbPage);
                            if (rc == SQLITE_OK)
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
            public int sqlite3BtreeClearTable(int iTable, ref int pnChange)
            {
                int rc;
                BtShared pBt = this.pBt;
                sqlite3BtreeEnter(this);
                Debug.Assert(this.inTrans == TransType.TRANS_WRITE);
                ///
                ///<summary>
                ///Invalidate all incrblob cursors open on table iTable (assuming iTable
                ///</summary>
                ///<param name="is the root of a table b"> if it is not, the following call is</param>
                ///<param name="a no">op).  </param>
                this.invalidateIncrblobCursors(0, 1);
                rc = pBt.saveAllCursors((Pgno)iTable, null);
                if (SQLITE_OK == rc)
                {
                    rc = clearDatabasePage(pBt, (Pgno)iTable, 0, ref pnChange);
                }
                sqlite3BtreeLeave(this);
                return rc;
            }
            public int btreeDropTable(Pgno iTable, ref int piMoved)
            {
                int rc;
                MemPage pPage = null;
                BtShared pBt = this.pBt;
                Debug.Assert(sqlite3BtreeHoldsMutex(this));
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
                    sqlite3ConnectionBlocked(this.db, pBt.pCursor.pBtree.db);
                    return SQLITE_LOCKED_SHAREDCACHE;
                }
                rc = pBt.btreeGetPage((Pgno)iTable, ref pPage, 0);
                if (rc != 0)
                    return rc;
                int Dummy0 = 0;
                rc = this.sqlite3BtreeClearTable((int)iTable, ref Dummy0);
                if (rc != 0)
                {
                    releasePage(pPage);
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
                        maxRootPgno = this.sqlite3BtreeGetMeta(BTREE_LARGEST_ROOT_PAGE);
                        if (iTable == maxRootPgno)
                        {
                            ///
                            ///<summary>
                            ///</summary>
                            ///<param name="If the table being dropped is the table with the largest root">page</param>
                            ///<param name="number in the database, put the root page on the free list.">number in the database, put the root page on the free list.</param>
                            ///<param name=""></param>
                            freePage(pPage, ref rc);
                            releasePage(pPage);
                            if (rc != SQLITE_OK)
                            {
                                return rc;
                            }
                        }
                        else
                        {
                            ///
                            ///<summary>
                            ///</summary>
                            ///<param name="The table being dropped does not have the largest root">page</param>
                            ///<param name="number in the database. So move the page that does into the">number in the database. So move the page that does into the</param>
                            ///<param name="gap left by the deleted root">page.</param>
                            ///<param name=""></param>
                            MemPage pMove = new MemPage();
                            releasePage(pPage);
                            rc = pBt.btreeGetPage(maxRootPgno, ref pMove, 0);
                            if (rc != SQLITE_OK)
                            {
                                return rc;
                            }
                            rc = relocatePage(pBt, pMove, PTRMAP_ROOTPAGE, 0, iTable, 0);
                            releasePage(pMove);
                            if (rc != SQLITE_OK)
                            {
                                return rc;
                            }
                            pMove = null;
                            rc = pBt.btreeGetPage(maxRootPgno, ref pMove, 0);
                            freePage(pMove, ref rc);
                            releasePage(pMove);
                            if (rc != SQLITE_OK)
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
                        while (maxRootPgno == PENDING_BYTE_PAGE(pBt) || PTRMAP_ISPAGE(pBt, maxRootPgno))
                        {
                            maxRootPgno--;
                        }
                        Debug.Assert(maxRootPgno != PENDING_BYTE_PAGE(pBt));
                        rc = this.sqlite3BtreeUpdateMeta(4, maxRootPgno);
                    }
                    else
                    {
                        freePage(pPage, ref rc);
                        releasePage(pPage);
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
                    pPage.zeroPage(PTF_INTKEY | PTF_LEAF);
                    releasePage(pPage);
                }
                return rc;
            }
            public int sqlite3BtreeDropTable(int iTable, ref int piMoved)
            {
                int rc;
                sqlite3BtreeEnter(this);
                rc = this.btreeDropTable((u32)iTable, ref piMoved);
                sqlite3BtreeLeave(this);
                return rc;
            }
            public u32 sqlite3BtreeGetMeta(int idx)
            {
                u32 pMeta;
                BtShared pBt = this.pBt;
                sqlite3BtreeEnter(this);
                Debug.Assert(this.inTrans > (byte)TransType.TRANS_NONE);
                Debug.Assert(SQLITE_OK == this.querySharedCacheTableLock(MASTER_ROOT, READ_LOCK));
                Debug.Assert(pBt.pPage1 != null);
                Debug.Assert(idx >= 0 && idx <= 15);
                pMeta = Converter.sqlite3Get4byte(pBt.pPage1.aData, 36 + idx * 4);
                ///
                ///<summary>
                ///</summary>
                ///<param name="If auto">vacuum</param>
                ///<param name="database, mark the database as read">only.  </param>
                return pMeta;
#if SQLITE_OMIT_AUTOVACUUM
																																																																																					if( idx==BTREE_LARGEST_ROOT_PAGE && pMeta>0 ) pBt.readOnly = 1;
#endif
                sqlite3BtreeLeave(this);
            }
            public int sqlite3BtreeUpdateMeta(int idx, u32 iMeta)
            {
                BtShared pBt = this.pBt;
                byte[] pP1;
                int rc;
                Debug.Assert(idx >= 1 && idx <= 15);
                sqlite3BtreeEnter(this);
                Debug.Assert(this.inTrans == TransType.TRANS_WRITE);
                Debug.Assert(pBt.pPage1 != null);
                pP1 = pBt.pPage1.aData;
                rc = sqlite3PagerWrite(pBt.pPage1.pDbPage);
                if (rc == SQLITE_OK)
                {
                    Converter.sqlite3Put4byte(pP1, 36 + idx * 4, iMeta);
#if !SQLITE_OMIT_AUTOVACUUM
                    if (idx == BTREE_INCR_VACUUM)
                    {
                        Debug.Assert(pBt.autoVacuum || iMeta == 0);
                        Debug.Assert(iMeta == 0 || iMeta == 1);
                        pBt.incrVacuum = iMeta != 0;
                    }
#endif
                }
                sqlite3BtreeLeave(this);
                return rc;
            }
            public int querySharedCacheTableLock(Pgno iTab, u8 eLock)
            {
                return SQLITE_OK;
            }
            public void invalidateIncrblobCursors(i64 y, int z)
            {
            }

            /// <summary>
            /// connection confguration
            /// </summary>
            /// <param name="mxPage"></param>
            /// <returns></returns>
            public int SetCacheSize(int mxPage)
            {
                BtShared pBt = this.pBt;
                Debug.Assert(sqlite3_mutex_held(this.db.mutex));
                sqlite3BtreeEnter(this);
                pBt.pPager.sqlite3PagerSetCachesize(mxPage);
                sqlite3BtreeLeave(this);
                return SQLITE_OK;
            }
            public int sqlite3BtreeSetSafetyLevel(///
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
                Debug.Assert(sqlite3_mutex_held(this.db.mutex));
                Debug.Assert(level >= 1 && level <= 3);
                sqlite3BtreeEnter(this);
                pBt.pPager.sqlite3PagerSetSafetyLevel(level, fullSync, ckptFullSync);
                sqlite3BtreeLeave(this);
                return SQLITE_OK;
            }
            public int GetPageSize()
            {
                return (int)this.pBt.pageSize;
            }
            public int sqlite3BtreeSetPageSize(int pageSize, int nReserve, int iFix)
            {
                int rc = SQLITE_OK;
                BtShared pBt = this.pBt;
                Debug.Assert(nReserve >= -1 && nReserve <= 255);
                sqlite3BtreeEnter(this);
                if (pBt.pageSizeFixed)
                {
                    sqlite3BtreeLeave(this);
                    return SQLITE_READONLY;
                }
                if (nReserve < 0)
                {
                    nReserve = (int)(pBt.pageSize - pBt.usableSize);
                }
                Debug.Assert(nReserve >= 0 && nReserve <= 255);
                if (pageSize >= 512 && pageSize <= SQLITE_MAX_PAGE_SIZE && ((pageSize - 1) & pageSize) == 0)
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
                sqlite3BtreeLeave(this);
                return rc;
            }
            public Pgno sqlite3BtreeLastPage()
            {
                Debug.Assert(sqlite3BtreeHoldsMutex(this));
                Debug.Assert(((this.pBt.nPage) & 0x8000000) == 0);
                return (Pgno)this.pBt.btreePagecount();
            }
            public int sqlite3BtreeSyncDisabled()
            {
                BtShared pBt = this.pBt;
                int rc;
                Debug.Assert(sqlite3_mutex_held(this.db.mutex));
                sqlite3BtreeEnter(this);
                Debug.Assert(pBt != null && pBt.pPager != null);
                rc = pBt.pPager.sqlite3PagerNosync() ? 1 : 0;
                sqlite3BtreeLeave(this);
                return rc;
            }
            public int GetReserve()
            {
                int n;
                sqlite3BtreeEnter(this);
                n = (int)(this.pBt.pageSize - this.pBt.usableSize);
                sqlite3BtreeLeave(this);
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
                sqlite3BtreeEnter(this);
                n = this.pBt.pPager.sqlite3PagerMaxPageCount(mxPage);
                sqlite3BtreeLeave(this);
                return n;
            }
            public int sqlite3BtreeSecureDelete(int newFlag)
            {
                int b;
                if (this == null)
                    return 0;
                sqlite3BtreeEnter(this);
                if (newFlag >= 0)
                {
                    this.pBt.secureDelete = (newFlag != 0);
                }
                b = this.pBt.secureDelete ? 1 : 0;
                sqlite3BtreeLeave(this);
                return b;
            }
            public int sqlite3BtreeSetAutoVacuum(int autoVacuum)
            {
#if SQLITE_OMIT_AUTOVACUUM
																																																																												return SQLITE_READONLY;
#else
                BtShared pBt = this.pBt;
                int rc = SQLITE_OK;
                u8 av = (u8)autoVacuum;
                sqlite3BtreeEnter(this);
                if (pBt.pageSizeFixed && (av != 0) != pBt.autoVacuum)
                {
                    rc = SQLITE_READONLY;
                }
                else
                {
                    pBt.autoVacuum = av != 0;
                    pBt.incrVacuum = av == 2;
                }
                sqlite3BtreeLeave(this);
                return rc;
#endif
            }
            public int GetAutoVacuum()
            {
#if SQLITE_OMIT_AUTOVACUUM
																																																																												return BTREE_AUTOVACUUM_NONE;
#else
                int rc;
                sqlite3BtreeEnter(this);
                rc = ((!this.pBt.autoVacuum) ? BTREE_AUTOVACUUM_NONE : (!this.pBt.incrVacuum) ? BTREE_AUTOVACUUM_FULL : BTREE_AUTOVACUUM_INCR);
                sqlite3BtreeLeave(this);
                return rc;
#endif
            }
            public int sqlite3BtreeBeginTrans(int wrflag)
            {
                BtShared pBt = this.pBt;
                int rc = SQLITE_OK;
                sqlite3BtreeEnter(this);
                btreeIntegrity(this);
                ///
                ///<summary>
                ///</summary>
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
                    rc = SQLITE_READONLY;
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
                rc = this.querySharedCacheTableLock(MASTER_ROOT, READ_LOCK);
                if (SQLITE_OK != rc)
                    goto trans_begun;
                pBt.initiallyEmpty = pBt.nPage == 0;
                do
                {
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
                    while (pBt.pPage1 == null && SQLITE_OK == (rc = lockBtree(pBt)))
                        ;
                    if (rc == SQLITE_OK && wrflag != 0)
                    {
                        if (pBt.readOnly)
                        {
                            rc = SQLITE_READONLY;
                        }
                        else
                        {
                            rc = pBt.pPager.sqlite3PagerBegin(wrflag > 1, sqlite3TempInMemory(this.db) ? 1 : 0);
                            if (rc == SQLITE_OK)
                            {
                                rc = newDatabase(pBt);
                            }
                        }
                    }
                    if (rc != SQLITE_OK)
                    {
                        unlockBtreeIfUnused(pBt);
                    }
                }
                while ((rc & 0xFF) == SQLITE_BUSY && pBt.inTransaction == TransType.TRANS_NONE && btreeInvokeBusyHandler(pBt) != 0);
                if (rc == SQLITE_OK)
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
                        ///
                        ///<summary>
                        ///</summary>
                        ///<param name="If the db">size header field is incorrect (as it may be if an old</param>
                        ///<param name="client has been writing the database file), update it now. Doing">client has been writing the database file), update it now. Doing</param>
                        ///<param name="this sooner rather than later means the database size can safely ">this sooner rather than later means the database size can safely </param>
                        ///<param name="re">read the database size from page 1 if a savepoint or transaction</param>
                        ///<param name="rollback occurs within the transaction.">rollback occurs within the transaction.</param>
                        if (pBt.nPage != Converter.sqlite3Get4byte(pPage1.aData, 28))
                        {
                            rc = sqlite3PagerWrite(pPage1.pDbPage);
                            if (rc == SQLITE_OK)
                            {
                                Converter.sqlite3Put4byte(pPage1.aData, (u32)28, pBt.nPage);
                            }
                        }
                    }
                }
            trans_begun:
                if (rc == SQLITE_OK && wrflag != 0)
                {
                    ///
                    ///<summary>
                    ///This call makes sure that the pager has the correct number of
                    ///open savepoints. If the second parameter is greater than 0 and
                    ///</summary>
                    ///<param name="the sub">journal is not already open, then it will be opened here.</param>
                    ///<param name=""></param>
                    rc = pBt.pPager.sqlite3PagerOpenSavepoint(this.db.nSavepoint);
                }
                btreeIntegrity(this);
                sqlite3BtreeLeave(this);
                return rc;
            }
            public int sqlite3BtreeIncrVacuum()
            {
                int rc;
                BtShared pBt = this.pBt;
                sqlite3BtreeEnter(this);
                Debug.Assert(pBt.inTransaction == TransType.TRANS_WRITE && this.inTrans == TransType.TRANS_WRITE);
                if (!pBt.autoVacuum)
                {
                    rc = SQLITE_DONE;
                }
                else
                {
                    pBt.invalidateAllOverflowCache();
                    rc = incrVacuumStep(pBt, 0, pBt.btreePagecount());
                    if (rc == SQLITE_OK)
                    {
                        rc = sqlite3PagerWrite(pBt.pPage1.pDbPage);
                        Converter.sqlite3Put4byte(pBt.pPage1.aData, (u32)28, pBt.nPage);
                        //put4byte(&pBt->pPage1->aData[28], pBt->nPage);
                    }
                }
                sqlite3BtreeLeave(this);
                return rc;
            }
            public int sqlite3BtreeCommitPhaseOne(string zMaster)
            {
                int rc = SQLITE_OK;
                if (this.inTrans == TransType.TRANS_WRITE)
                {
                    BtShared pBt = this.pBt;
                    sqlite3BtreeEnter(this);
#if !SQLITE_OMIT_AUTOVACUUM
                    if (pBt.autoVacuum)
                    {
                        rc = autoVacuumCommit(pBt);
                        if (rc != SQLITE_OK)
                        {
                            sqlite3BtreeLeave(this);
                            return rc;
                        }
                    }
#endif
                    rc = pBt.pPager.sqlite3PagerCommitPhaseOne(zMaster, false);
                    sqlite3BtreeLeave(this);
                }
                return rc;
            }
            public void btreeEndTransaction()
            {
                BtShared pBt = this.pBt;
                Debug.Assert(sqlite3BtreeHoldsMutex(this));
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
                    unlockBtreeIfUnused(pBt);
                }
                btreeIntegrity(this);
            }
            public int sqlite3BtreeCommitPhaseTwo(int bCleanup)
            {
                if (this.inTrans == TransType.TRANS_NONE)
                    return SQLITE_OK;
                sqlite3BtreeEnter(this);
                btreeIntegrity(this);
                ///
                ///<summary>
                ///</summary>
                ///<param name="If the handle has a write">btrees</param>
                ///<param name="transaction and set the shared state to TRANS_READ.">transaction and set the shared state to TRANS_READ.</param>
                ///<param name=""></param>
                if (this.inTrans == TransType.TRANS_WRITE)
                {
                    int rc;
                    BtShared pBt = this.pBt;
                    Debug.Assert(pBt.inTransaction == TransType.TRANS_WRITE);
                    Debug.Assert(pBt.nTransaction > 0);
                    rc = pBt.pPager.sqlite3PagerCommitPhaseTwo();
                    if (rc != SQLITE_OK && bCleanup == 0)
                    {
                        sqlite3BtreeLeave(this);
                        return rc;
                    }
                    pBt.inTransaction = TransType.TRANS_READ;
                }
                this.btreeEndTransaction();
                sqlite3BtreeLeave(this);
                return SQLITE_OK;
            }
            public int sqlite3BtreeCommit()
            {
                int rc;
                sqlite3BtreeEnter(this);
                rc = this.sqlite3BtreeCommitPhaseOne(null);
                if (rc == SQLITE_OK)
                {
                    rc = this.sqlite3BtreeCommitPhaseTwo(0);
                }
                sqlite3BtreeLeave(this);
                return rc;
            }
            public void sqlite3BtreeTripAllCursors(int errCode)
            {
                BtCursor p;
                sqlite3BtreeEnter(this);
                for (p = this.pBt.pCursor; p != null; p = p.pNext)
                {
                    int i;
                    p.sqlite3BtreeClearCursor();
                    p.State = BtCursorState.CURSOR_FAULT;
                    p.skipNext = errCode;
                    for (i = 0; i <= p.iPage; i++)
                    {
                        releasePage(p.apPage[i]);
                        p.apPage[i] = null;
                    }
                }
                sqlite3BtreeLeave(this);
            }
            public int sqlite3BtreeRollback()
            {
                int rc;
                BtShared pBt = this.pBt;
                MemPage pPage1 = new MemPage();
                sqlite3BtreeEnter(this);
                rc = pBt.saveAllCursors(0, null);
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
                btreeIntegrity(this);
                if (this.inTrans == TransType.TRANS_WRITE)
                {
                    int rc2;
                    Debug.Assert(TransType.TRANS_WRITE == pBt.inTransaction);
                    rc2 = pBt.pPager.sqlite3PagerRollback();
                    if (rc2 != SQLITE_OK)
                    {
                        rc = rc2;
                    }
                    ///
                    ///<summary>
                    ///The rollback may have destroyed the pPage1.aData value.  So
                    ///call btreeGetPage() on page 1 again to make
                    ///sure pPage1.aData is set correctly. 
                    ///</summary>
                    if (pBt.btreeGetPage(1, ref pPage1, 0) == SQLITE_OK)
                    {
                        Pgno nPage = Converter.sqlite3Get4byte(pPage1.aData, 28);
                        testcase(nPage == 0);
                        if (nPage == 0)
                            pBt.pPager.sqlite3PagerPagecount(out nPage);
                        testcase(pBt.nPage != nPage);
                        pBt.nPage = nPage;
                        releasePage(pPage1);
                    }
                    Debug.Assert(countWriteCursors(pBt) == 0);
                    pBt.inTransaction = TransType.TRANS_READ;
                }
                this.btreeEndTransaction();
                sqlite3BtreeLeave(this);
                return rc;
            }
            public int sqlite3BtreeBeginStmt(int iStatement)
            {
                int rc;
                BtShared pBt = this.pBt;
                sqlite3BtreeEnter(this);
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
                sqlite3BtreeLeave(this);
                return rc;
            }
            public int sqlite3BtreeSavepoint(int op, int iSavepoint)
            {
                int rc = SQLITE_OK;
                if (this != null && this.inTrans == TransType.TRANS_WRITE)
                {
                    BtShared pBt = this.pBt;
                    Debug.Assert(op == SAVEPOINT_RELEASE || op == SAVEPOINT_ROLLBACK);
                    Debug.Assert(iSavepoint >= 0 || (iSavepoint == -1 && op == SAVEPOINT_ROLLBACK));
                    sqlite3BtreeEnter(this);
                    rc = pBt.pPager.sqlite3PagerSavepoint(op, iSavepoint);
                    if (rc == SQLITE_OK)
                    {
                        if (iSavepoint < 0 && pBt.initiallyEmpty)
                            pBt.nPage = 0;
                        rc = newDatabase(pBt);
                        pBt.nPage = Converter.sqlite3Get4byte(pBt.pPage1.aData, 28);
                        ///
                        ///<summary>
                        ///The database size was written into the offset 28 of the header
                        ///when the transaction started, so we know that the value at offset
                        ///28 is nonzero. 
                        ///</summary>
                        Debug.Assert(pBt.nPage > 0);
                    }
                    sqlite3BtreeLeave(this);
                }
                return rc;
            }
            public int btreeCursor(///
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
            )
            {
                BtShared pBt = this.pBt;
                ///
                ///<summary>
                ///</summary>
                ///<param name="Shared b">tree handle </param>
                Debug.Assert(sqlite3BtreeHoldsMutex(this));
                Debug.Assert(wrFlag == 0 || wrFlag == 1);
                ///
                ///<summary>
                ///The following Debug.Assert statements verify that if this is a sharable
                ///</summary>
                ///<param name="b">tree database, the connection is holding the required table locks,</param>
                ///<param name="and that no other connection has any open cursor that conflicts with">and that no other connection has any open cursor that conflicts with</param>
                ///<param name="this lock.  ">this lock.  </param>
                Debug.Assert(this.hasSharedCacheTableLock((u32)iTable, pKeyInfo != null ? 1 : 0, wrFlag + 1));
                Debug.Assert(wrFlag == 0 || !this.hasReadConflicts((u32)iTable));
                ///
                ///<summary>
                ///Assert that the caller has opened the required transaction. 
                ///</summary>
                Debug.Assert(this.inTrans > TransType.TRANS_NONE);
                Debug.Assert(wrFlag == 0 || this.inTrans == TransType.TRANS_WRITE);
                Debug.Assert(pBt.pPage1 != null && pBt.pPage1.aData != null);
                if (NEVER(wrFlag != 0 && pBt.readOnly))
                {
                    return SQLITE_READONLY;
                }
                if (iTable == 1 && pBt.btreePagecount() == 0)
                {
                    return SQLITE_EMPTY;
                }
                ///
                ///<summary>
                ///Now that no other errors can occur, finish filling in the BtCursor
                ///variables and link the cursor into the BtShared list.  
                ///</summary>
                pCur.pgnoRoot = (Pgno)iTable;
                pCur.iPage = -1;
                pCur.pKeyInfo = pKeyInfo;
                pCur.pBtree = this;
                pCur.pBt = pBt;
                pCur.wrFlag = (u8)wrFlag;
                pCur.pNext = pBt.pCursor;
                if (pCur.pNext != null)
                {
                    pCur.pNext.pPrev = pCur;
                }
                pBt.pCursor = pCur;
                pCur.State = BtCursorState.CURSOR_INVALID;
                pCur.cachedRowid = 0;
                return SQLITE_OK;
            }
            public int sqlite3BtreeCursor(///
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
            )
            {
                int rc;
                sqlite3BtreeEnter(this);
                rc = this.btreeCursor(iTable, wrFlag, pKeyInfo, pCur);
                sqlite3BtreeLeave(this);
                return rc;
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
    
        public enum BTreeProp
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
        const int BTREE_FREE_PAGE_COUNT = 0;
        const int BTREE_SCHEMA_VERSION = 1;
        const int BTREE_FILE_FORMAT = 2;
        const int BTREE_DEFAULT_CACHE_SIZE = 3;
        const int BTREE_LARGEST_ROOT_PAGE = 4;
        const int BTREE_TEXT_ENCODING = 5;
        const int BTREE_USER_VERSION = 6;
        const int BTREE_INCR_VACUUM = 7;








	}
}
