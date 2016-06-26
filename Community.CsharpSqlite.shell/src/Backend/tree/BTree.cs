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
    namespace Tree
    {
        public partial class Btree:IBusyScope
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


            

            ///
            


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
                sCheck.nPage = sCheck.pBt.GetPageCount();
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
                using (this.scope())
                {
                    if (null == pBt.pSchema && nBytes != 0)
                    {
                        pBt.pSchema = new Schema();
                        //sqlite3DbMallocZero(0, nBytes);
                        pBt.xFreeSchema = xFree;
                    }
                }
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
            

            private bool NEVER(BtCursor btCursor)
            {
                return Sqlite3.NEVER(btCursor);
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
                return (Pgno)this.pBt.GetPageCount();
            }
            public int sqlite3BtreeSyncDisabled()
            {
                BtShared pBt = this.pBt;
                int rc;
                Debug.Assert(this.db.mutex.sqlite3_mutex_held());
                using (this.scope())
                {
                    Debug.Assert(pBt != null && pBt.pPager != null);
                    rc = pBt.pPager.sqlite3PagerNosync ? 1 : 0;
                }
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
                    rc = BTreeMethods.incrVacuumStep(pBt, 0, pBt.GetPageCount());
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
                    p.sqlite3BtreeClearCursor();
                    p.State = BtCursorState.CURSOR_FAULT;
                    p.skipNext = (ThreeState)errCode;
                    for (var i = 0; i <= p.pageStackIndex; i++)
                    {
                        BTreeMethods.release(p.PageStack[i]);
                        p.PageStack[i] = null;
                    }
                }
                this.Exit();
            }
            public SqlResult sqlite3BtreeRollback()
            {
                SqlResult rc;
                BtShared btShared = this.pBt;
                MemPage pPage1 = new MemPage();
                this.Enter();
                rc = btShared.saveAllCursors(0, null);
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
                    Debug.Assert(TransType.TRANS_WRITE == btShared.inTransaction);
                    rc2 = btShared.pPager.sqlite3PagerRollback();
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
                    if (btShared.GetPage(1, ref pPage1, 0) == SqlResult.SQLITE_OK)
                    {
                        Pgno nPage = Converter.sqlite3Get4byte(pPage1.aData, Offsets.DatabaseSize);
                        sqliteinth.testcase(nPage == 0);
                        if (nPage == 0)
                            btShared.pPager.GetPagecount(out nPage);
                        sqliteinth.testcase(btShared.nPage != nPage);
                        btShared.nPage = nPage;
                        pPage1.release();
                    }
                    Debug.Assert(BTreeMethods.countWriteCursors(btShared) == 0);
                    btShared.inTransaction = TransType.TRANS_READ;
                }
                this.btreeEndTransaction();
                this.Exit();
                return rc;
            }
            public SqlResult sqlite3BtreeBeginStmt(int iStatement)
            {
                SqlResult rc;
                BtShared pBt = this.pBt;
                using (this.scope())
                {
                    Debug.Assert(this.inTrans == TransType.TRANS_WRITE);
                    Debug.Assert(!pBt.readOnly);
                    Debug.Assert(iStatement > 0);
                    Debug.Assert(iStatement > this.db.nSavepoint);
                    Debug.Assert(pBt.inTransaction == TransType.TRANS_WRITE);
                    ///At the pager level, a statement transaction is a savepoint with
                    ///an index greater than all savepoints created explicitly using
                    ///SQL statements. It is illegal to open, release or rollback any
                    ///such savepoints while the statement transaction savepoint is active.
                    rc = pBt.pPager.sqlite3PagerOpenSavepoint(iStatement);
                }
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
                        rc = pBt.newDatabase();
                        pBt.nPage = Converter.sqlite3Get4byte(pBt.pPage1.aData, Offsets.DatabaseSize);
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

            public SqlResult btreeCursor(
                int rootPageOfTableToOpen,///Root page of table to open 
                CursorMode wrFlag,///<param name="1 to write. 0 read">only </param>
                KeyInfo pKeyInfo,///First arg to comparison function 
                BtCursor pCur///Space for new cursor 
            )
            {
                BtShared pBt = this.pBt;///<param name="Shared b">tree handle </param>
                Debug.Assert(Sqlite3.sqlite3BtreeHoldsMutex(this));
                Debug.Assert(wrFlag == CursorMode.ReadOnly || wrFlag == CursorMode.ReadWrite);
                ///The following Debug.Assert statements verify that if this is a sharable
                ///<param name="b">tree database, the connection is holding the required table locks,</param>
                ///<param name="and that no other connection has any open cursor that conflicts with">and that no other connection has any open cursor that conflicts with</param>
                ///<param name="this lock.  ">this lock.  </param>
                Debug.Assert(this.hasSharedCacheTableLock((u32)rootPageOfTableToOpen, pKeyInfo != null ? 1 : 0, (int)wrFlag + 1));
                Debug.Assert(wrFlag == 0 || !this.hasReadConflicts((u32)rootPageOfTableToOpen));
                ///Assert that the caller has opened the required transaction. 
                Debug.Assert(this.inTrans > TransType.TRANS_NONE);
                Debug.Assert(wrFlag == 0 || this.inTrans == TransType.TRANS_WRITE);
                Debug.Assert(pBt.pPage1 != null && pBt.pPage1.aData != null);
                if (NEVER(wrFlag != 0 && pBt.readOnly))
                {
                    return SqlResult.SQLITE_READONLY;
                }
                if (rootPageOfTableToOpen == 1 && pBt.GetPageCount() == 0)
                {
                    return SqlResult.SQLITE_EMPTY;
                }
                ///Now that no other errors can occur, finish filling in the BtCursor
                ///variables and link the cursor into the BtShared list.  
                pCur.pgnoRoot = (Pgno)rootPageOfTableToOpen;
                pCur.pageStackIndex = -1;
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

            public SqlResult sqlite3BtreeCursor(
                int rootPageOfTableToOpen,///Root page of table to open 
                CursorMode wrFlag,///<param name="1 to write. 0 read">only </param>
                KeyInfo pKeyInfo,///First arg to xCompare() 
                BtCursor pCur///Write new cursor here 
            )
            {           
                using (this.scope())
                    return this.btreeCursor(rootPageOfTableToOpen, wrFlag, pKeyInfo, pCur);
            }
            public Pager Pager
            {
                get
                {
                    return this.pBt.pPager;
                }
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
