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
    using DbPage = Sqlite3.PgHdr;
    using System.Text;
    public partial class Sqlite3
    {
        /* A Btree handle
    **
    ** A database connection contains a pointer to an instance of
    ** this object for every database file that it has open.  This structure
    ** is opaque to the database connection.  The database connection cannot
    ** see the internals of this structure and only deals with pointers to
    ** this structure.
    **
    ** For some database files, the same underlying database cache might be
    ** shared between multiple connections.  In that case, each connection
    ** has it own instance of this object.  But each instance of this object
    ** points to the same BtShared object.  The database cache and the
    ** schema associated with the database file are all contained within
    ** the BtShared object.
    **
    ** All fields in this structure are accessed under sqlite3.mutex.
    ** The pBt pointer itself may not be changed while there exists cursors
    ** in the referenced BtShared that point back to this Btree since those
    ** cursors have to go through this Btree to find their BtShared and
    ** they often do so without holding sqlite3.mutex.
    */
        public class Btree
        {
            public sqlite3 db;
            /* The database connection holding this Btree */
            public BtShared pBt;
            /* Sharable content of this Btree */
            public u8 inTrans;
            /* TRANS_NONE, TRANS_READ or TRANS_WRITE */
            public bool sharable;
            /* True if we can share pBt with another db */
            public bool locked;
            /* True if db currently has pBt locked */
            public int wantToLock;
            /* Number of nested calls to sqlite3BtreeEnter() */
            public int nBackup;
            /* Number of backup operations reading this btree */
            public Btree pNext;
            /* List of other sharable Btrees from the same db */
            public Btree pPrev;
            /* Back pointer of the same list */
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
                /* Set up an sqlite3_backup object. sqlite3_backup.pDestDb must be set
  ** to 0. This is used by the implementations of sqlite3_backup_step()
  ** and sqlite3_backup_finish() to detect that they are being called
  ** from this function, not directly by the user.
  */
                b = new sqlite3_backup();
                // memset( &b, 0, sizeof( b ) );
                b.pSrcDb = pFrom.db;
                b.pSrc = pFrom;
                b.pDest = this;
                b.iNext = 1;
                /* 0x7FFFFFFF is the hard limit for the number of pages in a database
  ** file. By passing this as the number of pages to copy to
  ** sqlite3_backup_step(), we can guarantee that the copy finishes
  ** within a single call (unless an error occurs). The Debug.Assert() statement
  ** checks this assumption - (p.rc) should be set to either SQLITE_DONE
  ** or an error code.
  */
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
            public string sqlite3BtreeIntegrityCheck(/* The btree to be checked */int[] aRoot,/* An array of root pages numbers for individual trees */int nRoot,/* Number of entries in aRoot[] */int mxErr,/* Stop reporting errors after this many */ref int pnErr/* Write number of errors seen to this variable */)
            {
                Pgno i;
                int nRef;
                IntegrityCk sCheck = new IntegrityCk();
                BtShared pBt = this.pBt;
                StringBuilder zErr = new StringBuilder(100);
                //char zErr[100];
                sqlite3BtreeEnter(this);
                Debug.Assert(this.inTrans > TRANS_NONE && pBt.inTransaction > TRANS_NONE);
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
                /* Check the integrity of the freelist
  */
                sCheck.checkList(1, (int)Converter.sqlite3Get4byte(pBt.pPage1.aData, 32), (int)Converter.sqlite3Get4byte(pBt.pPage1.aData, 36), "Main freelist: ");
                /* Check all the tables.
  */
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
                /* Make sure every page in the file is referenced
  */
                for (i = 1; i <= sCheck.nPage && sCheck.mxErr != 0; i++)
                {
#if SQLITE_OMIT_AUTOVACUUM
																																																																																																if( sCheck.anRef[i]==null ){
checkAppendMsg(sCheck, 0, "Page %d is never used", i);
}
#else
                    /* If the database supports auto-vacuum, make sure no tables contain
** references to pointer-map pages.
*/
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
                /* Make sure this analysis did not leave any unref() pages.
  ** This is an internal consistency check; an integrity check
  ** of the integrity check.
  */
                if (NEVER(nRef != pBt.pPager.sqlite3PagerRefcount()))
                {
                    sCheck.checkAppendMsg("", "Outstanding page count goes from %d to %d during this analysis", nRef, pBt.pPager.sqlite3PagerRefcount());
                }
                /* Clean  up and report errors.
  */
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
                /* Return code */
                Debug.Assert(this.inTrans == TRANS_NONE);
                Debug.Assert(iVersion == 1 || iVersion == 2);
                /* If setting the version fields to 1, do not automatically open the
  ** WAL connection, even if the version fields are currently set to 2.
  */
                pBt.doNotUseWAL = iVersion == 1;
                rc = sqlite3BtreeBeginTrans(this, 0);
                if (rc == SQLITE_OK)
                {
                    u8[] aData = pBt.pPage1.aData;
                    if (aData[18] != (u8)iVersion || aData[19] != (u8)iVersion)
                    {
                        rc = sqlite3BtreeBeginTrans(this, 2);
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
                Debug.Assert(this.inTrans == TRANS_WRITE);
                /* Invalidate all incrblob cursors open on table iTable (assuming iTable
  ** is the root of a table b-tree - if it is not, the following call is
  ** a no-op).  */
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
                Debug.Assert(this.inTrans == TRANS_WRITE);
                /* It is illegal to drop a table if any cursors are open on the
  ** database. This is because in auto-vacuum mode the backend may
  ** need to move another root-page to fill a gap left by the deleted
  ** root page. If an open cursor was using this page a problem would
  ** occur.
  **
  ** This error is caught long before control reaches this point.
  */
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
                            /* If the table being dropped is the table with the largest root-page
        ** number in the database, put the root page on the free list.
        */
                            freePage(pPage, ref rc);
                            releasePage(pPage);
                            if (rc != SQLITE_OK)
                            {
                                return rc;
                            }
                        }
                        else
                        {
                            /* The table being dropped does not have the largest root-page
        ** number in the database. So move the page that does into the
        ** gap left by the deleted root-page.
        */
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
                        /* Set the new 'max-root-page' value in the database header. This
      ** is the old value less one, less one more if that happens to
      ** be a root-page number, less one again if that is the
      ** PENDING_BYTE_PAGE.
      */
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
                    /* If sqlite3BtreeDropTable was called on page 1.
    ** This really never should happen except in a corrupt
    ** database.
    */
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
                Debug.Assert(this.inTrans > TRANS_NONE);
                Debug.Assert(SQLITE_OK == this.querySharedCacheTableLock(MASTER_ROOT, READ_LOCK));
                Debug.Assert(pBt.pPage1 != null);
                Debug.Assert(idx >= 0 && idx <= 15);
                pMeta = Converter.sqlite3Get4byte(pBt.pPage1.aData, 36 + idx * 4);
                /* If auto-vacuum is disabled in this build and this is an auto-vacuum
  ** database, mark the database as read-only.  */
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
                Debug.Assert(this.inTrans == TRANS_WRITE);
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
            public int sqlite3BtreeSetCacheSize(int mxPage)
            {
                BtShared pBt = this.pBt;
                Debug.Assert(sqlite3_mutex_held(this.db.mutex));
                sqlite3BtreeEnter(this);
                pBt.pPager.sqlite3PagerSetCachesize(mxPage);
                sqlite3BtreeLeave(this);
                return SQLITE_OK;
            }
            public int sqlite3BtreeSetSafetyLevel(/* The btree to set the safety level on */int level,/* PRAGMA synchronous.  1=OFF, 2=NORMAL, 3=FULL */int fullSync,/* PRAGMA fullfsync. */int ckptFullSync/* PRAGMA checkpoint_fullfync */)
            {
                BtShared pBt = this.pBt;
                Debug.Assert(sqlite3_mutex_held(this.db.mutex));
                Debug.Assert(level >= 1 && level <= 3);
                sqlite3BtreeEnter(this);
                pBt.pPager.sqlite3PagerSetSafetyLevel(level, fullSync, ckptFullSync);
                sqlite3BtreeLeave(this);
                return SQLITE_OK;
            }
        }

        /*
    ** The second parameter to sqlite3BtreeGetMeta or sqlite3BtreeUpdateMeta
    ** should be one of the following values. The integer values are assigned
    ** to constants so that the offset of the corresponding field in an
    ** SQLite database header may be found using the following formula:
    **
    **   offset = 36 + (idx * 4)
    **
    ** For example, the free-page-count field is located at byte offset 36 of
    ** the database file header. The incr-vacuum-flag field is located at
    ** byte offset 64 (== 36+4*7).
    */
        //#define BTREE_FREE_PAGE_COUNT     0
        //#define BTREE_SCHEMA_VERSION      1
        //#define BTREE_FILE_FORMAT         2
        //#define BTREE_DEFAULT_CACHE_SIZE  3
        //#define BTREE_LARGEST_ROOT_PAGE   4
        //#define BTREE_TEXT_ENCODING       5
        //#define BTREE_USER_VERSION        6
        //#define BTREE_INCR_VACUUM         7
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
