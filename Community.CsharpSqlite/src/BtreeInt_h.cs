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
	using DbPage = Paging.PgHdr;
	using System.Text;
    using Metadata;
    using Community.CsharpSqlite.Os;
    using _Custom = Sqlite3._Custom;
    using Community.CsharpSqlite.tree;
    using Community.CsharpSqlite.Paging;
    using Community.CsharpSqlite.Utils;
    ///<summary>
    /// Btree.inTrans may take one of the following values.
    ///
    /// If the shared-data extension is enabled, there may be multiple users
    /// of the Btree structure. At most one of these may open a write transaction,
    /// but any number may have active read transactions.
    ///
    ///</summary>
    public enum TransType : byte
    {
        TRANS_NONE = 0,

        TRANS_READ = 1,

        TRANS_WRITE = 2
    }


    ///<summary>
    /// This structure is passed around through all the sanity checking routines
    /// in order to keep track of some global state information.
    ///</summary>
    //typedef struct IntegrityCk IntegrityCk;
    public class IntegrityCk
    {
        public BtShared pBt;

        ///
        ///<summary>
        ///The tree being checked out 
        ///</summary>

        public Pager pPager;

        ///
        ///<summary>
        ///The associated pager.  Also accessible by pBt.pPager 
        ///</summary>

        public Pgno nPage;

        ///
        ///<summary>
        ///Number of pages in the database 
        ///</summary>

        public int[] anRef;

        ///
        ///<summary>
        ///Number of times each page is referenced 
        ///</summary>

        public int mxErr;

        ///
        ///<summary>
        ///Stop accumulating errors when this reaches zero 
        ///</summary>

        public int nErr;

        ///
        ///<summary>
        ///Number of messages written to zErrMsg so far 
        ///</summary>

        //public int mallocFailed;  /* A memory allocation error has occurred */
        public StrAccum errMsg = new StrAccum(100);

        ///
        ///<summary>
        ///Accumulate the error message text here 
        ///</summary>

        public void checkAppendMsg(StringBuilder zMsg1, string zFormat, params object[] ap)
        {
            if (0 == this.mxErr)
                return;
            //va_list ap;
            lock (_Custom.lock_va_list)
            {
                this.mxErr--;
                this.nErr++;
                _Custom.va_start(ap, zFormat);
                if (this.errMsg.zText.Length != 0)
                {
                    this.errMsg.sqlite3StrAccumAppend("\n", 1);
                }
                if (zMsg1.Length > 0)
                {
                    this.errMsg.sqlite3StrAccumAppend(zMsg1.ToString(), -1);
                }
                io.sqlite3VXPrintf(this.errMsg, 1, zFormat, ap);
                _Custom.va_end(ref ap);
            }
            //if( pCheck.errMsg.mallocFailed ){
            //  pCheck.mallocFailed = 1;
            //}
        }


        public void checkAppendMsg(string zMsg1, string zFormat, params object[] ap)
        {
            if (0 == this.mxErr)
                return;
            //va_list ap;
            lock (_Custom.lock_va_list)
            {
                this.mxErr--;
                this.nErr++;
                _Custom.va_start(ap, zFormat);
                if (this.errMsg.zText.Length != 0)
                {
                    this.errMsg.sqlite3StrAccumAppend("\n", 1);
                }
                if (zMsg1.Length > 0)
                {
                    this.errMsg.sqlite3StrAccumAppend(zMsg1.ToString(), -1);
                }
                io.sqlite3VXPrintf(this.errMsg, 1, zFormat, ap);
                _Custom.va_end(ref ap);
            }
        }

        public int checkRef(Pgno iPage, string zContext)
        {
            if (iPage == 0)
                return 1;
            if (iPage > this.nPage)
            {
                this.checkAppendMsg(zContext, "invalid page number %d", iPage);
                return 1;
            }
            if (this.anRef[iPage] == 1)
            {
                this.checkAppendMsg(zContext, "2nd reference to page %d", iPage);
                return 1;
            }
            return ((this.anRef[iPage]++) > 1) ? 1 : 0;
        }

        public int checkTreePage(///
            ///<summary>
            ///Context for the sanity check 
            ///</summary>

        int iPage, ///
            ///<summary>
            ///Page number of the page to check 
            ///</summary>

        string zParentContext, ///
            ///<summary>
            ///Parent context 
            ///</summary>

        ref i64 pnParentMinKey, ref i64 pnParentMaxKey, object _pnParentMinKey, ///
            ///<summary>
            ///C# Needed to determine if content passed
            ///</summary>

        object _pnParentMaxKey///
            ///<summary>
            ///C# Needed to determine if content passed
            ///</summary>

        )
        {
            MemPage pPage = new MemPage();
            int i, depth, d2, pgno, cnt;
            SqlResult rc;
            int hdr, cellStart;
            int nCell;
            u8[] data;
            BtShared pBt;
            int usableSize;
            StringBuilder zContext = new StringBuilder(100);
            byte[] hit = null;
            i64 nMinKey = 0;
            i64 nMaxKey = 0;
            io.sqlite3_snprintf(200, zContext, "Page %d: ", iPage);
            ///
            ///<summary>
            ///Check that the page exists
            ///
            ///</summary>

            pBt = this.pBt;
            usableSize = (int)pBt.usableSize;
            if (iPage == 0)
                return 0;
            if (this.checkRef((u32)iPage, zParentContext) != 0)
                return 0;
            if ((rc = pBt.btreeGetPage((Pgno)iPage, ref pPage, 0)) != 0)
            {
                this.checkAppendMsg(zContext.ToString(), "unable to get the page. error code=%d", rc);
                return 0;
            }
            ///
            ///<summary>
            ///Clear MemPage.isInit to make sure the corruption detection code in
            ///btreeInitPage() is executed.  
            ///</summary>

            pPage.isInit = false;
            if ((rc = pPage.btreeInitPage()) != 0)
            {
                Debug.Assert(rc == SqlResult.SQLITE_CORRUPT);
                ///
                ///<summary>
                ///The only possible error from InitPage 
                ///</summary>

                this.checkAppendMsg(zContext.ToString(), "btreeInitPage() returns error code %d", rc);
                BTreeMethods.releasePage(pPage);
                return 0;
            }
            ///
            ///<summary>
            ///Check out all the cells.
            ///
            ///</summary>

            depth = 0;
            for (i = 0; i < pPage.nCell && this.mxErr != 0; i++)
            {
                u8[] pCell;
                u32 sz;
                CellInfo info = new CellInfo();
                ///
                ///<summary>
                ///Check payload overflow pages
                ///
                ///</summary>

                io.sqlite3_snprintf(200, zContext, "On tree page %d cell %d: ", iPage, i);
                int iCell = pPage.findCell(i);
                //pCell = findCell( pPage, i );
                pCell = pPage.aData;
                pPage.btreeParseCellPtr(iCell, ref info);
                //btreeParseCellPtr( pPage, pCell, info );
                sz = info.nData;
                if (false == pPage.intKey)
                    sz += (u32)info.nKey;
                ///
                ///<summary>
                ///For intKey pages, check that the keys are in order.
                ///
                ///</summary>

                else
                    if (i == 0)
                        nMinKey = nMaxKey = info.nKey;
                    else
                    {
                        if (info.nKey <= nMaxKey)
                        {
                            this.checkAppendMsg(zContext.ToString(), "Rowid %lld out of order (previous was %lld)", info.nKey, nMaxKey);
                        }
                        nMaxKey = info.nKey;
                    }
                Debug.Assert(sz == info.nPayload);
                if ((sz > info.nLocal)//&& (pCell[info.iOverflow]<=&pPage.aData[pBt.usableSize])
                )
                {
                    int nPage = (int)(sz - info.nLocal + usableSize - 5) / (usableSize - 4);
                    Pgno pgnoOvfl = Converter.sqlite3Get4byte(pCell, iCell, info.iOverflow);
#if !SQLITE_OMIT_AUTOVACUUM
                    if (pBt.autoVacuum)
                    {
                        this.checkPtrmap(pgnoOvfl, PTRMAP.OVERFLOW1, (u32)iPage, zContext.ToString());
                    }
#endif
                    this.checkList(0, (int)pgnoOvfl, nPage, zContext.ToString());
                }
                ///
                ///<summary>
                ///Check sanity of left child page.
                ///
                ///</summary>

                if (false == pPage.IsLeaf)
                {
                    pgno = (int)Converter.sqlite3Get4byte(pCell, iCell);
                    //sqlite3Get4byte( pCell );
#if !SQLITE_OMIT_AUTOVACUUM
                    if (pBt.autoVacuum)
                    {
                        this.checkPtrmap((u32)pgno, PTRMAP.BTREE, (u32)iPage, zContext.ToString());
                    }
#endif
                    if (i == 0)
                        d2 = this.checkTreePage(pgno, zContext.ToString(), ref nMinKey, ref BTreeMethods.refNULL, this, null);
                    else
                        d2 = this.checkTreePage(pgno, zContext.ToString(), ref nMinKey, ref nMaxKey, this, this);
                    if (i > 0 && d2 != depth)
                    {
                        this.checkAppendMsg(zContext, "Child page depth differs");
                    }
                    depth = d2;
                }
            }
            if (false == pPage.IsLeaf)
            {
                pgno = (int)Converter.sqlite3Get4byte(pPage.aData, pPage.hdrOffset + 8);
                io.sqlite3_snprintf(200, zContext, "On page %d at right child: ", iPage);
#if !SQLITE_OMIT_AUTOVACUUM
                if (pBt.autoVacuum)
                {
                    this.checkPtrmap((u32)pgno, PTRMAP.BTREE, (u32)iPage, zContext.ToString());
                }
#endif
                //    checkTreePage(pCheck, pgno, zContext, NULL, !pPage->nCell ? NULL : &nMaxKey);
                if (0 == pPage.nCell)
                    this.checkTreePage(pgno, zContext.ToString(), ref BTreeMethods.refNULL, ref BTreeMethods.refNULL, null, null);
                else
                    this.checkTreePage(pgno, zContext.ToString(), ref BTreeMethods.refNULL, ref nMaxKey, null, this);
            }
            ///
            ///<summary>
            ///For intKey leaf pages, check that the min/max keys are in order
            ///with any left/parent/right pages.
            ///
            ///</summary>

            if (pPage.IsLeaf != false && pPage.intKey != false)
            {
                ///
                ///<summary>
                ///if we are a left child page 
                ///</summary>

                if (_pnParentMinKey != null)
                {
                    ///
                    ///<summary>
                    ///if we are the left most child page 
                    ///</summary>

                    if (_pnParentMaxKey == null)
                    {
                        if (nMaxKey > pnParentMinKey)
                        {
                            this.checkAppendMsg(zContext, "Rowid %lld out of order (max larger than parent min of %lld)", nMaxKey, pnParentMinKey);
                        }
                    }
                    else
                    {
                        if (nMinKey <= pnParentMinKey)
                        {
                            this.checkAppendMsg(zContext, "Rowid %lld out of order (min less than parent min of %lld)", nMinKey, pnParentMinKey);
                        }
                        if (nMaxKey > pnParentMaxKey)
                        {
                            this.checkAppendMsg(zContext, "Rowid %lld out of order (max larger than parent max of %lld)", nMaxKey, pnParentMaxKey);
                        }
                        pnParentMinKey = nMaxKey;
                    }
                    ///
                    ///<summary>
                    ///else if we're a right child page 
                    ///</summary>

                }
                else
                    if (_pnParentMaxKey != null)
                    {
                        if (nMinKey <= pnParentMaxKey)
                        {
                            this.checkAppendMsg(zContext, "Rowid %lld out of order (min less than parent max of %lld)", nMinKey, pnParentMaxKey);
                        }
                    }
            }
            ///
            ///<summary>
            ///Check for complete coverage of the page
            ///
            ///</summary>

            data = pPage.aData;
            hdr = pPage.hdrOffset;
            hit = malloc_cs.sqlite3Malloc(pBt.pageSize);
            //if( hit==null ){
            //  pCheck.mallocFailed = 1;
            //}else
            {
                int contentOffset = BTreeMethods.get2byteNotZero(data, hdr + 5);
                Debug.Assert(contentOffset <= usableSize);
                ///
                ///<summary>
                ///Enforced by btreeInitPage() 
                ///</summary>

                Array.Clear(hit, contentOffset, usableSize - contentOffset);
                //memset(hit+contentOffset, 0, usableSize-contentOffset);
                for (int iLoop = contentOffset - 1; iLoop >= 0; iLoop--)
                    hit[iLoop] = 1;
                //memset(hit, 1, contentOffset);
                nCell = data.get2byte(hdr + 3);
                cellStart = hdr + 12 - (pPage.IsLeaf ? 4 : 0);
                for (i = 0; i < nCell; i++)
                {
                    int pc = data.get2byte(cellStart + i * 2);
                    u32 size = 65536;
                    int j;
                    if (pc <= usableSize - 4)
                    {
                        size = pPage.cellSizePtr(data, pc);
                    }
                    if ((int)(pc + size - 1) >= usableSize)
                    {
                        this.checkAppendMsg("", "Corruption detected in cell %d on page %d", i, iPage);
                    }
                    else
                    {
                        for (j = (int)(pc + size - 1); j >= pc; j--)
                            hit[j]++;
                    }
                }
                i = data.get2byte(hdr + 1);
                while (i > 0)
                {
                    int size, j;
                    Debug.Assert(i <= usableSize - 4);
                    ///
                    ///<summary>
                    ///Enforced by btreeInitPage() 
                    ///</summary>

                    size = data.get2byte(i + 2);
                    Debug.Assert(i + size <= usableSize);
                    ///
                    ///<summary>
                    ///Enforced by btreeInitPage() 
                    ///</summary>

                    for (j = i + size - 1; j >= i; j--)
                        hit[j]++;
                    j = data.get2byte(i);
                    Debug.Assert(j == 0 || j > i + size);
                    ///
                    ///<summary>
                    ///Enforced by btreeInitPage() 
                    ///</summary>

                    Debug.Assert(j <= usableSize - 4);
                    ///
                    ///<summary>
                    ///Enforced by btreeInitPage() 
                    ///</summary>

                    i = j;
                }
                for (i = cnt = 0; i < usableSize; i++)
                {
                    if (hit[i] == 0)
                    {
                        cnt++;
                    }
                    else
                        if (hit[i] > 1)
                        {
                            this.checkAppendMsg("", "Multiple uses for byte %d of page %d", i, iPage);
                            break;
                        }
                }
                if (cnt != data[hdr + 7])
                {
                    this.checkAppendMsg("", "Fragmentation of %d bytes reported as %d on page %d", cnt, data[hdr + 7], iPage);
                }
            }
            Sqlite3.sqlite3PageFree(ref hit);
            BTreeMethods.releasePage(pPage);
            return depth + 1;
        }

        public void checkList(
            int isFreeList, ///True for a freelist.  False for overflow page list 
            int iPage, ///Page number for first page in the list 
            int N, ///Expected number of pages in the list 
            string zContext///Context for error messages 
        )
        {
            int i;
            int expected = N;
            int iFirst = iPage;
            while (N-- > 0 && this.mxErr != 0)
            {
                PgHdr pOvflPage = new PgHdr();
                byte[] pOvflData;
                if (iPage < 1)
                {
                    this.checkAppendMsg(zContext, "%d of %d pages missing from overflow list starting at %d", N + 1, expected, iFirst);
                    break;
                }
                if (this.checkRef((u32)iPage, zContext) != 0)
                    break;
                if (this.pPager.sqlite3PagerGet((Pgno)iPage, ref pOvflPage) != 0)
                {
                    this.checkAppendMsg(zContext, "failed to get page %d", iPage);
                    break;
                }
                pOvflData = pOvflPage.sqlite3PagerGetData();
                if (isFreeList != 0)
                {
                    int n = (int)Converter.sqlite3Get4byte(pOvflData, 4);
#if !SQLITE_OMIT_AUTOVACUUM
                    if (this.pBt.autoVacuum)
                    {
                        this.checkPtrmap((u32)iPage, PTRMAP.FREEPAGE, 0, zContext);
                    }
#endif
                    if (n > (int)this.pBt.usableSize / 4 - 2)
                    {
                        this.checkAppendMsg(zContext, "freelist leaf count too big on page %d", iPage);
                        N--;
                    }
                    else
                    {
                        for (i = 0; i < n; i++)
                        {
                            Pgno iFreePage = Converter.sqlite3Get4byte(pOvflData, 8 + i * 4);
#if !SQLITE_OMIT_AUTOVACUUM
                            if (this.pBt.autoVacuum)
                            {
                                this.checkPtrmap(iFreePage, PTRMAP.FREEPAGE, 0, zContext);
                            }
#endif
                            this.checkRef(iFreePage, zContext);
                        }
                        N -= n;
                    }
                }
#if !SQLITE_OMIT_AUTOVACUUM
                else
                {
                    ///
                    ///<summary>
                    ///</summary>
                    ///<param name="If this database supports auto">vacuum and iPage is not the last</param>
                    ///<param name="page in this overflow list, check that the pointer">map entry for</param>
                    ///<param name="the following page matches iPage.">the following page matches iPage.</param>
                    ///<param name=""></param>

                    if (this.pBt.autoVacuum && N > 0)
                    {
                        i = (int)Converter.sqlite3Get4byte(pOvflData);
                        this.checkPtrmap((u32)i, PTRMAP.OVERFLOW2, (u32)iPage, zContext);
                    }
                }
#endif
                iPage = (int)Converter.sqlite3Get4byte(pOvflData);
                PagerMethods.sqlite3PagerUnref(pOvflPage);
            }
        }

        public void checkPtrmap(
            Pgno iChild, ///Child page number 
            u8 eType, ///Expected pointer map type 
            Pgno iParent,///Expected pointer map parent page number 
            string zContext///Context description (used for error msg) 
        )
        {
            SqlResult rc;
            u8 ePtrmapType = 0;
            Pgno iPtrmapParent = 0;
            rc = this.pBt.ptrmapGet(iChild, ref ePtrmapType, ref iPtrmapParent);
            if (rc != SqlResult.SQLITE_OK)
            {
                //if( rc==SQLITE_NOMEM || rc==SQLITE_IOERR_NOMEM ) pCheck.mallocFailed = 1;
                this.checkAppendMsg(zContext, "Failed to read ptrmap key=%d", iChild);
                return;
            }
            if (ePtrmapType != eType || iPtrmapParent != iParent)
            {
                this.checkAppendMsg(zContext, "Bad ptr map entry key=%d expected=(%d,%d) got=(%d,%d)", iChild, eType, iParent, ePtrmapType, iPtrmapParent);
            }
        }

    }



    ///<summary>
    /// As each page of the file is loaded into memory, an instance of the following
    /// structure is appended and initialized to zero.  This structure stores
    /// information about the page that is decoded from the raw file page.
    ///
    /// The pParent field points back to the parent page.  This allows us to
    /// walk up the BTree from any leaf to the root.  Care must be taken to
    /// unref() the parent page pointer when this page is no longer referenced.
    /// The pageDestructor() routine handles that chore.
    ///
    /// Access to all fields of this structure is controlled by the mutex
    /// stored in MemPage.pBt.mutex.
    ///
    ///</summary>
    public struct _OvflCell
    {
        ///
        ///<summary>
        ///Cells that will not fit on aData[] 
        ///</summary>

        public u8[] pCell;

        ///<summary>
        ///Pointers to the body of the overflow cell
        ///</summary>
        public u16 idx;

        ///
        ///<summary>
        ///</summary>
        ///<param name="Insert this cell before idx">overflow cell </param>

        public _OvflCell Copy()
        {
            _OvflCell cp = new _OvflCell();
            if (pCell != null)
            {
                cp.pCell = malloc_cs.sqlite3Malloc(pCell.Length);
                Buffer.BlockCopy(pCell, 0, cp.pCell, 0, pCell.Length);
            }
            cp.idx = idx;
            return cp;
        }
    }



    namespace tree { 
    // No used in C#, since we use create a class; was MemPage.Length;
    ///
    ///<summary>
    ///A linked list of the following structures is stored at BtShared.pLock.
    ///Locks are added (or upgraded from READ_LOCK to WRITE_LOCK) when a cursor 
    ///is opened on the table with root page BtShared.iTable. Locks are removed
    ///from this list when a transaction is committed or rolled back, or when
    ///a btree handle is closed.
    ///
    ///</summary>

    public class BtLock
    {
        Btree pBtree;

        ///
        ///<summary>
        ///Btree handle holding this lock 
        ///</summary>

        Pgno iTable;

        ///
        ///<summary>
        ///Root page of table 
        ///</summary>

        u8 eLock;

        ///
        ///<summary>
        ///READ_LOCK or WRITE_LOCK 
        ///</summary>

        BtLock pNext;
        ///
        ///<summary>
        ///Next in BtShared.pLock list 
        ///</summary>

    };



    ///
    ///<summary>
    ///An instance of this object represents a single database file.
    ///
    ///A single database file can be in use as the same time by two
    ///or more database connections.  When two or more connections are
    ///sharing the same database file, each connection has it own
    ///private Btree object for the file and each of those Btrees points
    ///to this one BtShared object.  BtShared.nRef is the number of
    ///connections currently sharing this database file.
    ///
    ///Fields in this structure are accessed under the BtShared.mutex
    ///mutex, except for nRef and pNext which are accessed under the
    ///global SQLITE_MUTEX_STATIC_MASTER mutex.  The pPager field
    ///may not be modified once it is initially set as long as nRef>0.
    ///The pSchema field may be set once under BtShared.mutex and
    ///thereafter is unchanged as long as nRef>0.
    ///
    ///isPending:
    ///
    ///</summary>
    ///<param name="If a BtShared client fails to obtain a write">lock on a database</param>
    ///<param name="table (because there exists one or more read">locks on the table),</param>
    ///<param name="the shared">lock' state and isPending is</param>
    ///<param name="set to true.">set to true.</param>
    ///<param name=""></param>
    ///<param name="The shared">cache leaves the 'pending lock' state when either of</param>
    ///<param name="the following occur:">the following occur:</param>
    ///<param name=""></param>
    ///<param name="1) The current writer (BtShared.pWriter) concludes its transaction, OR">1) The current writer (BtShared.pWriter) concludes its transaction, OR</param>
    ///<param name="2) The number of locks held by other connections drops to zero.">2) The number of locks held by other connections drops to zero.</param>
    ///<param name=""></param>
    ///<param name="while in the 'pending">lock' state, no connection may start a new</param>
    ///<param name="transaction.">transaction.</param>
    ///<param name=""></param>
    ///<param name="This feature is included to help prevent writer">starvation.</param>
    ///<param name=""></param>

    public class BtShared
    {
        ///
        ///<summary>
        ///The page cache 
        ///</summary>

        public Pager pPager;

        ///
        ///<summary>
        ///Database connection currently using this Btree 
        ///</summary>

        public Connection db;

        ///
        ///<summary>
        ///A list of all open cursors 
        ///</summary>

        public BtCursor pCursor;

        ///
        ///<summary>
        ///First page of the database 
        ///</summary>

        public MemPage pPage1;

        ///
        ///<summary>
        ///True if the underlying file is readonly 
        ///</summary>

        public bool readOnly;

        ///
        ///<summary>
        ///True if the page size can no longer be changed 
        ///</summary>

        public bool pageSizeFixed;

        ///
        ///<summary>
        ///True if secure_delete is enabled 
        ///</summary>

        public bool secureDelete;

        ///
        ///<summary>
        ///Database is empty at start of transaction 
        ///</summary>

        public bool initiallyEmpty;

        public u8 openFlags;

        ///
        ///<summary>
        ///Flags to sqlite3BtreeOpen() 
        ///</summary>

#if !SQLITE_OMIT_AUTOVACUUM
        public bool autoVacuum;

        ///
        ///<summary>
        ///</summary>
        ///<param name="True if auto">vacuum is enabled </param>

        public bool incrVacuum;

        ///
        ///<summary>
        ///</summary>
        ///<param name="True if incr">vacuum is enabled </param>

#endif
        public TransType inTransaction;

        ///
        ///<summary>
        ///Transaction state 
        ///</summary>

        public bool doNotUseWAL;

        ///
        ///<summary>
        ///</summary>
        ///<param name="If true, do not open write">log file </param>

        public u16 maxLocal;

        ///
        ///<summary>
        ///</summary>
        ///<param name="Maximum local payload in non">LEAFDATA tables </param>

        public u16 minLocal;

        ///
        ///<summary>
        ///</summary>
        ///<param name="Minimum local payload in non">LEAFDATA tables </param>

        public u16 maxLeaf;

        ///
        ///<summary>
        ///Maximum local payload in a LEAFDATA table 
        ///</summary>

        public u16 minLeaf;

        ///
        ///<summary>
        ///Minimum local payload in a LEAFDATA table 
        ///</summary>

        public u32 pageSize;

        ///
        ///<summary>
        ///Total number of bytes on a page 
        ///</summary>

        public u32 usableSize;

        ///
        ///<summary>
        ///Number of usable bytes on each page 
        ///</summary>

        public int nTransaction;

        ///
        ///<summary>
        ///Number of open transactions (read + write) 
        ///</summary>

        public Pgno nPage;

        ///
        ///<summary>
        ///Number of pages in the database 
        ///</summary>

        public Schema pSchema;

        ///
        ///<summary>
        ///Pointer to space allocated by sqlite3BtreeSchema() 
        ///</summary>

        public dxFreeSchema xFreeSchema;

        ///
        ///<summary>
        ///Destructor for BtShared.pSchema 
        ///</summary>

        public sqlite3_mutex mutex;

        ///
        ///<summary>
        ///</summary>
        ///<param name="Non">recursive mutex required to access this object </param>

        public Bitvec pHasContent;

        ///
        ///<summary>
        ///</summary>
        ///<param name="Set of pages moved to free">list this transaction </param>

#if !SQLITE_OMIT_SHARED_CACHE
																																																																					public int nRef;                /* Number of references to this structure */
public BtShared pNext;          /* Next on a list of sharable BtShared structs */
public BtLock pLock;            /* List of locks held on this shared-btree struct */
public Btree pWriter;           /* Btree with currently open write transaction */
public u8 isExclusive;          /* True if pWriter has an EXCLUSIVE lock on the db */
public u8 isPending;            /* If waiting for read-locks to clear */
#endif
        public byte[] pTmpSpace;

        ///
        ///<summary>
        ///BtShared.pageSize bytes of space for tmp use 
        ///</summary>

        public///<summary>
              /// Set bit pgno of the BtShared.pHasContent bitvec. This is called
              /// when a page that previously contained data becomes a free-list leaf
              /// page.
              ///
              /// The BtShared.pHasContent bitvec exists to work around an obscure
              /// bug caused by the interaction of two useful IO optimizations surrounding
              /// free-list leaf pages:
              ///
              ///   1) When all data is deleted from a page and the page becomes
              ///      a free-list leaf page, the page is not written to the database
              ///      (as free-list leaf pages contain no meaningful data). Sometimes
              ///      such a page is not even journalled (as it will not be modified,
              ///      why bother journalling it?).
              ///
              ///   2) When a free-list leaf page is reused, its content is not read
              ///      from the database or written to the journal file (why should it
              ///      be, if it is not at all meaningful?).
              ///
              /// By themselves, these optimizations work fine and provide a handy
              /// performance boost to bulk delete or insert operations. However, if
              /// a page is moved to the free-list and then reused within the same
              /// transaction, a problem comes up. If the page is not journalled when
              /// it is moved to the free-list and it is also not journalled when it
              /// is extracted from the free-list and reused, then the original data
              /// may be lost. In the event of a rollback, it may not be possible
              /// to restore the database to its original configuration.
              ///
              /// The solution is the BtShared.pHasContent bitvec. Whenever a page is
              /// moved to become a free-list leaf page, the corresponding bit is
              /// set in the bitvec. Whenever a leaf page is extracted from the free-list,
              /// optimization 2 above is omitted if the corresponding bit is already
              /// set in BtShared.pHasContent. The contents of the bitvec are cleared
              /// at the end of every transaction.
              ///</summary>
            SqlResult btreeSetHasContent(Pgno pgno)
        {
            SqlResult rc = SqlResult.SQLITE_OK;
            if (null == this.pHasContent)
            {
                Debug.Assert(pgno <= this.nPage);
                this.pHasContent = Bitvec.sqlite3BitvecCreate(this.nPage);
                //if ( null == pBt.pHasContent )
                //{
                //  rc = SQLITE_NOMEM;
                //}
            }
            if (rc == SqlResult.SQLITE_OK && pgno <= this.pHasContent.sqlite3BitvecSize())
            {
                rc = this.pHasContent.setBit(pgno);
            }
            return rc;
        }

        public///<summary>
              /// Query the BtShared.pHasContent vector.
              ///
              /// This function is called when a free-list leaf page is removed from the
              /// free-list for reuse. It returns false if it is safe to retrieve the
              /// page from the pager layer with the 'no-content' flag set. True otherwise.
              ///</summary>
          bool btreeGetHasContent(Pgno pgno)
        {
            Bitvec p = this.pHasContent;
            return (p != null && (pgno > p.sqlite3BitvecSize() || p.sqlite3BitvecTest( pgno) != 0));
        }

        public///<summary>
              /// Clear (destroy) the BtShared.pHasContent bitvec. This should be
              /// invoked at the conclusion of each write-transaction.
              ///</summary>
          void btreeClearHasContent()
        {
            BitvecExtensions.sqlite3BitvecDestroy(ref this.pHasContent);
            this.pHasContent = null;
        }

        public void invalidateAllOverflowCache()
        {
        }

        public SqlResult saveAllCursors(Pgno iRoot, BtCursor pExcept)
        {
            BtCursor p;
            Debug.Assert(this.mutex.sqlite3_mutex_held());
            Debug.Assert(pExcept == null || pExcept.pBt == this);
            for (p = this.pCursor; p != null; p = p.pNext)
            {
                if (p != pExcept && (0 == iRoot || p.pgnoRoot == iRoot) && p.eState == BtCursorState.CURSOR_VALID)
                {
                    SqlResult rc = p.saveCursorPosition();
                    if (SqlResult.SQLITE_OK != rc)
                    {
                        return rc;
                    }
                }
            }
            return SqlResult.SQLITE_OK;
        }

        public Pgno ptrmapPageno(Pgno pgno)
        {
            int nPagesPerMapPage;
            Pgno iPtrMap, ret;
            Debug.Assert(this.mutex.sqlite3_mutex_held());


            if (pgno < 2)
                return 0;
            nPagesPerMapPage = (int)(this.usableSize / 5 + 1);
            iPtrMap = (Pgno)((pgno - 2) / nPagesPerMapPage);
            ret = (Pgno)(iPtrMap * nPagesPerMapPage) + 2;
            if (ret == this.PENDING_BYTE_PAGE)
            {
                ret++;
            }
            return ret;
        }

        public void ptrmapPut(Pgno key, u8 eType, Pgno parent, ref SqlResult pRC)
        {
            PgHdr pDbPage = new PgHdr();
            ///
            ///<summary>
            ///The pointer map page 
            ///</summary>

            u8[] pPtrmap;
            ///
            ///<summary>
            ///The pointer map data 
            ///</summary>

            Pgno iPtrmap;
            ///
            ///<summary>
            ///The pointer map page number 
            ///</summary>

            int offset;
            ///
            ///<summary>
            ///Offset in pointer map page 
            ///</summary>

            SqlResult rc;
            ///
            ///<summary>
            ///Return code from subfunctions 
            ///</summary>

            if (pRC != 0)
                return;
            Debug.Assert(this.mutex.sqlite3_mutex_held());
            ///
            ///<summary>
            ///</summary>
            ///<param name="The master">journal page number must never be used as a pointer map page </param>

            Debug.Assert(false == this.PTRMAP_ISPAGE(this.PENDING_BYTE_PAGE));
            Debug.Assert(this.autoVacuum);
            if (key == 0)
            {
                pRC = sqliteinth.SQLITE_CORRUPT_BKPT();
                return;
            }
            iPtrmap = this.PTRMAP_PAGENO(key);
            rc = this.pPager.sqlite3PagerGet(iPtrmap, ref pDbPage);
            if (rc != SqlResult.SQLITE_OK)
            {
                pRC = rc;
                return;
            }
            offset = (int)BTreeMethods.PTRMAP_PTROFFSET(iPtrmap, key);
            if (offset < 0)
            {
                pRC = sqliteinth.SQLITE_CORRUPT_BKPT();
                goto ptrmap_exit;
            }
            Debug.Assert(offset <= (int)this.usableSize - 5);
            pPtrmap = pDbPage.sqlite3PagerGetData();
            if (eType != pPtrmap[offset] || Converter.sqlite3Get4byte(pPtrmap, offset + 1) != parent)
            {
                Sqlite3.TRACE("PTRMAP_UPDATE: %d->(%d,%d)\n", key, eType, parent);
                pRC = rc = PagerMethods.sqlite3PagerWrite(pDbPage);
                if (rc == SqlResult.SQLITE_OK)
                {
                    pPtrmap[offset] = eType;
                    Converter.sqlite3Put4byte(pPtrmap, offset + 1, parent);
                }
            }
            ptrmap_exit:
            PagerMethods.sqlite3PagerUnref(pDbPage);
        }

        public SqlResult ptrmapGet(Pgno key, ref u8 pEType, ref Pgno pPgno)
        {
            PgHdr pDbPage = new PgHdr();
            ///
            ///<summary>
            ///The pointer map page 
            ///</summary>

            int iPtrmap;
            ///
            ///<summary>
            ///Pointer map page index 
            ///</summary>

            u8[] pPtrmap;
            ///
            ///<summary>
            ///Pointer map page data 
            ///</summary>

            int offset;
            ///
            ///<summary>
            ///Offset of entry in pointer map 
            ///</summary>

            SqlResult rc;
            Debug.Assert(this.mutex.sqlite3_mutex_held());
            iPtrmap = (int)this.PTRMAP_PAGENO(key);
            rc = this.pPager.sqlite3PagerGet((u32)iPtrmap, ref pDbPage);
            if (rc != 0)
            {
                return rc;
            }
            pPtrmap = pDbPage.sqlite3PagerGetData();
            offset = (int)BTreeMethods.PTRMAP_PTROFFSET((u32)iPtrmap, key);
            if (offset < 0)
            {
                PagerMethods.sqlite3PagerUnref(pDbPage);
                return sqliteinth.SQLITE_CORRUPT_BKPT();
            }
            Debug.Assert(offset <= (int)this.usableSize - 5);
            // Under C# pEType will always exist. No need to test; //
            //Debug.Assert( pEType != 0 );
            pEType = pPtrmap[offset];
            // Under C# pPgno will always exist. No need to test; //
            //if ( pPgno != 0 )
            pPgno = Converter.sqlite3Get4byte(pPtrmap, offset + 1);
            PagerMethods.sqlite3PagerUnref(pDbPage);
            if (pEType < 1 || pEType > 5)
                return sqliteinth.SQLITE_CORRUPT_BKPT();
            return SqlResult.SQLITE_OK;
        }

        public SqlResult btreeGetPage(
            Pgno pgno, ///Number of the page to fetch 
            ref MemPage ppPage, ///Return the page in this parameter 
            int noContent///Do not load page content if true 
        )
        {
            SqlResult rc;
            DbPage pDbPage = null;
            Debug.Assert(this.mutex.sqlite3_mutex_held());
            rc = this.pPager.sqlite3PagerAcquire(pgno, ref pDbPage, (u8)noContent);
            if (rc != 0)
                return rc;
            ppPage = pDbPage.btreePageFromDbPage(pgno, this);
            return SqlResult.SQLITE_OK;
        }

        public MemPage btreePageLookup(Pgno pgno)
        {
            DbPage pDbPage;
            Debug.Assert(this.mutex.sqlite3_mutex_held());
            pDbPage = this.pPager.sqlite3PagerLookup(pgno);
            if (pDbPage)
            {
                return pDbPage.btreePageFromDbPage(pgno, this);
            }
            return null;
        }

        public Pgno btreePagecount()
        {
            return this.nPage;
        }





        //#include "sqliteInt.h"
        ///<summary>
        ///The following value is the maximum cell size assuming a maximum page
        /// size give above.
        ///
        ///</summary>
        //#define MX_CELL_SIZE(pBt)  ((int)(pBt->pageSize-8))
        public int MX_CELL_SIZE
        {
            get { return (int)(this.pageSize - 8); }
        }

        ///<summary>
        ///The maximum number of cells on a single page of the database.  This
        /// assumes a minimum cell size of 6 bytes  (4 bytes for the cell itself
        /// plus 2 bytes for the index to the cell in the page header).  Such
        /// small cells will be rare, but they are possible.
        ///
        ///</summary>
        //#define MX_CELL(pBt) ((pBt.pageSize-8)/6)
        public int MX_CELL
        {
            get { return ((int)(this.pageSize - 8) / 6); }
        }


        ///<summary>
        /// The database page the PENDING_BYTE occupies. This page is never used.
        ///
        ///</summary>
        //# define PENDING_BYTE_PAGE(pBt) PAGER_MJ_PGNO(pBt)
        // TODO -- Convert PENDING_BYTE_PAGE to inline
        public u32 PENDING_BYTE_PAGE
        {
            get
            {
                BtShared pBt = this;
                return (u32)pBt.pPager.PAGER_MJ_PGNO;
            }
        }


    }

}
    //----------------------------------------------------------------

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
///
///
///</summary>
///<param name="This file implements a external (disk">based) database using BTrees.</param>
///<param name="For a detailed discussion of BTrees, refer to">For a detailed discussion of BTrees, refer to</param>
///<param name=""></param>
///<param name="Donald E. Knuth, THE ART OF COMPUTER PROGRAMMING, Volume 3:">Donald E. Knuth, THE ART OF COMPUTER PROGRAMMING, Volume 3:</param>
///<param name=""Sorting And Searching", pages 473">Wesley</param>
///<param name="Publishing Company, Reading, Massachusetts.">Publishing Company, Reading, Massachusetts.</param>
///<param name=""></param>
///<param name="The basic idea is that each page of the file contains N database">The basic idea is that each page of the file contains N database</param>
///<param name="entries and N+1 pointers to subpages.">entries and N+1 pointers to subpages.</param>
///<param name=""></param>
///<param name=""></param>
///<param name="|  Ptr(0) | Key(0) | Ptr(1) | Key(1) | ... | Key(N">1) | Ptr(N) |</param>
///<param name=""></param>
///<param name=""></param>
///<param name="All of the keys on the page that Ptr(0) points to have values less">All of the keys on the page that Ptr(0) points to have values less</param>
///<param name="than Key(0).  All of the keys on page Ptr(1) and its subpages have">than Key(0).  All of the keys on page Ptr(1) and its subpages have</param>
///<param name="values greater than Key(0) and less than Key(1).  All of the keys">values greater than Key(0) and less than Key(1).  All of the keys</param>
///<param name="on Ptr(N) and its subpages have values greater than Key(N">1).  And</param>
///<param name="so forth.">so forth.</param>
///<param name=""></param>
///<param name="Finding a particular key requires reading O(log(M)) pages from the">Finding a particular key requires reading O(log(M)) pages from the</param>
///<param name="disk where M is the number of entries in the tree.">disk where M is the number of entries in the tree.</param>
///<param name=""></param>
///<param name="In this implementation, a single file can hold one or more separate">In this implementation, a single file can hold one or more separate</param>
///<param name="BTrees.  Each BTree is identified by the index of its root page.  The">BTrees.  Each BTree is identified by the index of its root page.  The</param>
///<param name="key and data for any entry are combined to form the "payload".  A">key and data for any entry are combined to form the "payload".  A</param>
///<param name="fixed amount of payload can be carried directly on the database">fixed amount of payload can be carried directly on the database</param>
///<param name="page.  If the payload is larger than the preset amount then surplus">page.  If the payload is larger than the preset amount then surplus</param>
///<param name="bytes are stored on overflow pages.  The payload for an entry">bytes are stored on overflow pages.  The payload for an entry</param>
///<param name="and the preceding pointer are combined to form a "Cell".  Each">and the preceding pointer are combined to form a "Cell".  Each</param>
///<param name="page has a small header which contains the Ptr(N) pointer and other">page has a small header which contains the Ptr(N) pointer and other</param>
///<param name="information such as the size of key and data.">information such as the size of key and data.</param>
///<param name=""></param>
///<param name="FORMAT DETAILS">FORMAT DETAILS</param>
///<param name=""></param>
///<param name="The file is divided into pages.  The first page is called page 1,">The file is divided into pages.  The first page is called page 1,</param>
///<param name="the second is page 2, and so forth.  A page number of zero indicates">the second is page 2, and so forth.  A page number of zero indicates</param>
///<param name=""no such page".  The page size can be any power of 2 between 512 and 65536.">"no such page".  The page size can be any power of 2 between 512 and 65536.</param>
///<param name="Each page can be either a btree page, a freelist page, an overflow">Each page can be either a btree page, a freelist page, an overflow</param>
///<param name="page, or a pointer">map page.</param>
///<param name=""></param>
///<param name="The first page is always a btree page.  The first 100 bytes of the first">The first page is always a btree page.  The first 100 bytes of the first</param>
///<param name="page contain a special header (the "file header") that describes the file.">page contain a special header (the "file header") that describes the file.</param>
///<param name="The format of the file header is as follows:">The format of the file header is as follows:</param>
///<param name=""></param>
///<param name="OFFSET   SIZE    DESCRIPTION">OFFSET   SIZE    DESCRIPTION</param>
///<param name="0      16     Header string: "SQLite format 3\000"">0      16     Header string: "SQLite format 3\000"</param>
///<param name="16       2     Page size in bytes.">16       2     Page size in bytes.</param>
///<param name="18       1     File format write version">18       1     File format write version</param>
///<param name="19       1     File format read version">19       1     File format read version</param>
///<param name="20       1     Bytes of unused space at the end of each page">20       1     Bytes of unused space at the end of each page</param>
///<param name="21       1     Max embedded payload fraction">21       1     Max embedded payload fraction</param>
///<param name="22       1     Min embedded payload fraction">22       1     Min embedded payload fraction</param>
///<param name="23       1     Min leaf payload fraction">23       1     Min leaf payload fraction</param>
///<param name="24       4     File change counter">24       4     File change counter</param>
///<param name="28       4     Reserved for future use">28       4     Reserved for future use</param>
///<param name="32       4     First freelist page">32       4     First freelist page</param>
///<param name="36       4     Number of freelist pages in the file">36       4     Number of freelist pages in the file</param>
///<param name="40      60     15 4">byte meta values passed to higher layers</param>
///<param name=""></param>
///<param name="40       4     Schema cookie">40       4     Schema cookie</param>
///<param name="44       4     File format of schema layer">44       4     File format of schema layer</param>
///<param name="48       4     Size of page cache">48       4     Size of page cache</param>
///<param name="52       4     Largest root">page (auto/incr_vacuum)</param>
///<param name="56       4     1=UTF">8 2=UTF16le 3=UTF16be</param>
///<param name="60       4     User version">60       4     User version</param>
///<param name="64       4     Incremental vacuum mode">64       4     Incremental vacuum mode</param>
///<param name="68       4     unused">68       4     unused</param>
///<param name="72       4     unused">72       4     unused</param>
///<param name="76       4     unused">76       4     unused</param>
///<param name=""></param>
///<param name="All of the integer values are big">endian (most significant byte first).</param>
///<param name=""></param>
///<param name="The file change counter is incremented when the database is changed">The file change counter is incremented when the database is changed</param>
///<param name="This counter allows other processes to know when the file has changed">This counter allows other processes to know when the file has changed</param>
///<param name="and thus when they need to flush their cache.">and thus when they need to flush their cache.</param>
///<param name=""></param>
///<param name="The max embedded payload fraction is the amount of the total usable">The max embedded payload fraction is the amount of the total usable</param>
///<param name="space in a page that can be consumed by a single cell for standard">space in a page that can be consumed by a single cell for standard</param>
///<param name="B">LEAFDATA) tables.  A value of 255 means 100%.  The default</param>
///<param name="is to limit the maximum cell size so that at least 4 cells will fit">is to limit the maximum cell size so that at least 4 cells will fit</param>
///<param name="on one page.  Thus the default max embedded payload fraction is 64.">on one page.  Thus the default max embedded payload fraction is 64.</param>
///<param name=""></param>
///<param name="If the payload for a cell is larger than the max payload, then extra">If the payload for a cell is larger than the max payload, then extra</param>
///<param name="payload is spilled to overflow pages.  Once an overflow page is allocated,">payload is spilled to overflow pages.  Once an overflow page is allocated,</param>
///<param name="as many bytes as possible are moved into the overflow pages without letting">as many bytes as possible are moved into the overflow pages without letting</param>
///<param name="the cell size drop below the min embedded payload fraction.">the cell size drop below the min embedded payload fraction.</param>
///<param name=""></param>
///<param name="The min leaf payload fraction is like the min embedded payload fraction">The min leaf payload fraction is like the min embedded payload fraction</param>
///<param name="except that it applies to leaf nodes in a LEAFDATA tree.  The maximum">except that it applies to leaf nodes in a LEAFDATA tree.  The maximum</param>
///<param name="payload fraction for a LEAFDATA tree is always 100% (or 255) and it">payload fraction for a LEAFDATA tree is always 100% (or 255) and it</param>
///<param name="not specified in the header.">not specified in the header.</param>
///<param name=""></param>
///<param name="Each btree pages is divided into three sections:  The header, the">Each btree pages is divided into three sections:  The header, the</param>
///<param name="cell pointer array, and the cell content area.  Page 1 also has a 100">byte</param>
///<param name="file header that occurs before the page header.">file header that occurs before the page header.</param>
///<param name=""></param>
///<param name="|">|</param>
///<param name="| file header    |   100 bytes.  Page 1 only.">| file header    |   100 bytes.  Page 1 only.</param>
///<param name="|">|</param>
///<param name="| page header    |   8 bytes for leaves.  12 bytes for interior nodes">| page header    |   8 bytes for leaves.  12 bytes for interior nodes</param>
///<param name="|">|</param>
///<param name="| cell pointer   |   |  2 bytes per cell.  Sorted order.">| cell pointer   |   |  2 bytes per cell.  Sorted order.</param>
///<param name="| array          |   |  Grows downward">| array          |   |  Grows downward</param>
///<param name="|                |   v">|                |   v</param>
///<param name="|">|</param>
///<param name="| unallocated    |">| unallocated    |</param>
///<param name="| space          |">| space          |</param>
///<param name="|">|   ^  Grows upwards</param>
///<param name="| cell content   |   |  Arbitrary order interspersed with freeblocks.">| cell content   |   |  Arbitrary order interspersed with freeblocks.</param>
///<param name="| area           |   |  and free space fragments.">| area           |   |  and free space fragments.</param>
///<param name="|">|</param>
///<param name=""></param>
///<param name="The page headers looks like this:">The page headers looks like this:</param>
///<param name=""></param>
///<param name="OFFSET   SIZE     DESCRIPTION">OFFSET   SIZE     DESCRIPTION</param>
///<param name="0       1      Flags. 1: intkey, 2: zerodata, 4: leafdata, 8: leaf">0       1      Flags. 1: intkey, 2: zerodata, 4: leafdata, 8: leaf</param>
///<param name="1       2      byte offset to the first freeblock">1       2      byte offset to the first freeblock</param>
///<param name="3       2      number of cells on this page">3       2      number of cells on this page</param>
///<param name="5       2      first byte of the cell content area">5       2      first byte of the cell content area</param>
///<param name="7       1      number of fragmented free bytes">7       1      number of fragmented free bytes</param>
///<param name="8       4      Right child (the Ptr(N) value).  Omitted on leaves.">8       4      Right child (the Ptr(N) value).  Omitted on leaves.</param>
///<param name=""></param>
///<param name="The flags define the format of this btree page.  The leaf flag means that">The flags define the format of this btree page.  The leaf flag means that</param>
///<param name="this page has no children.  The zerodata flag means that this page carries">this page has no children.  The zerodata flag means that this page carries</param>
///<param name="only keys and no data.  The intkey flag means that the key is a integer">only keys and no data.  The intkey flag means that the key is a integer</param>
///<param name="which is stored in the key size entry of the cell header rather than in">which is stored in the key size entry of the cell header rather than in</param>
///<param name="the payload area.">the payload area.</param>
///<param name=""></param>
///<param name="The cell pointer array begins on the first byte after the page header.">The cell pointer array begins on the first byte after the page header.</param>
///<param name="The cell pointer array contains zero or more 2">byte numbers which are</param>
///<param name="offsets from the beginning of the page to the cell content in the cell">offsets from the beginning of the page to the cell content in the cell</param>
///<param name="content area.  The cell pointers occur in sorted order.  The system strives">content area.  The cell pointers occur in sorted order.  The system strives</param>
///<param name="to keep free space after the last cell pointer so that new cells can">to keep free space after the last cell pointer so that new cells can</param>
///<param name="be easily added without having to defragment the page.">be easily added without having to defragment the page.</param>
///<param name=""></param>
///<param name="Cell content is stored at the very end of the page and grows toward the">Cell content is stored at the very end of the page and grows toward the</param>
///<param name="beginning of the page.">beginning of the page.</param>
///<param name=""></param>
///<param name="Unused space within the cell content area is collected into a linked list of">Unused space within the cell content area is collected into a linked list of</param>
///<param name="freeblocks.  Each freeblock is at least 4 bytes in size.  The byte offset">freeblocks.  Each freeblock is at least 4 bytes in size.  The byte offset</param>
///<param name="to the first freeblock is given in the header.  Freeblocks occur in">to the first freeblock is given in the header.  Freeblocks occur in</param>
///<param name="increasing order.  Because a freeblock must be at least 4 bytes in size,">increasing order.  Because a freeblock must be at least 4 bytes in size,</param>
///<param name="any group of 3 or fewer unused bytes in the cell content area cannot">any group of 3 or fewer unused bytes in the cell content area cannot</param>
///<param name="exist on the freeblock chain.  A group of 3 or fewer free bytes is called">exist on the freeblock chain.  A group of 3 or fewer free bytes is called</param>
///<param name="a fragment.  The total number of bytes in all fragments is recorded.">a fragment.  The total number of bytes in all fragments is recorded.</param>
///<param name="in the page header at offset 7.">in the page header at offset 7.</param>
///<param name=""></param>
///<param name="SIZE    DESCRIPTION">SIZE    DESCRIPTION</param>
///<param name="2     Byte offset of the next freeblock">2     Byte offset of the next freeblock</param>
///<param name="2     Bytes in this freeblock">2     Bytes in this freeblock</param>
///<param name=""></param>
///<param name="Cells are of variable length.  Cells are stored in the cell content area at">Cells are of variable length.  Cells are stored in the cell content area at</param>
///<param name="the end of the page.  Pointers to the cells are in the cell pointer array">the end of the page.  Pointers to the cells are in the cell pointer array</param>
///<param name="that immediately follows the page header.  Cells is not necessarily">that immediately follows the page header.  Cells is not necessarily</param>
///<param name="contiguous or in order, but cell pointers are contiguous and in order.">contiguous or in order, but cell pointers are contiguous and in order.</param>
///<param name=""></param>
///<param name="Cell content makes use of variable length integers.  A variable">Cell content makes use of variable length integers.  A variable</param>
///<param name="length integer is 1 to 9 bytes where the lower 7 bits of each">length integer is 1 to 9 bytes where the lower 7 bits of each</param>
///<param name="byte are used.  The integer consists of all bytes that have bit 8 set and">byte are used.  The integer consists of all bytes that have bit 8 set and</param>
///<param name="the first byte with bit 8 clear.  The most significant byte of the integer">the first byte with bit 8 clear.  The most significant byte of the integer</param>
///<param name="appears first.  A variable">length integer may not be more than 9 bytes long.</param>
///<param name="As a special case, all 8 bytes of the 9th byte are used as data.  This">As a special case, all 8 bytes of the 9th byte are used as data.  This</param>
///<param name="allows a 64">bit integer to be encoded in 9 bytes.</param>
///<param name=""></param>
///<param name="0x00                      becomes  0x00000000">0x00                      becomes  0x00000000</param>
///<param name="0x7f                      becomes  0x0000007f">0x7f                      becomes  0x0000007f</param>
///<param name="0x81 0x00                 becomes  0x00000080">0x81 0x00                 becomes  0x00000080</param>
///<param name="0x82 0x00                 becomes  0x00000100">0x82 0x00                 becomes  0x00000100</param>
///<param name="0x80 0x7f                 becomes  0x0000007f">0x80 0x7f                 becomes  0x0000007f</param>
///<param name="0x8a 0x91 0xd1 0xac 0x78  becomes  0x12345678">0x8a 0x91 0xd1 0xac 0x78  becomes  0x12345678</param>
///<param name="0x81 0x81 0x81 0x81 0x01  becomes  0x10204081">0x81 0x81 0x81 0x81 0x01  becomes  0x10204081</param>
///<param name=""></param>
///<param name="Variable length integers are used for rowids and to hold the number of">Variable length integers are used for rowids and to hold the number of</param>
///<param name="bytes of key and data in a btree cell.">bytes of key and data in a btree cell.</param>
///<param name=""></param>
///<param name="The content of a cell looks like this:">The content of a cell looks like this:</param>
///<param name=""></param>
///<param name="SIZE    DESCRIPTION">SIZE    DESCRIPTION</param>
///<param name="4     Page number of the left child. Omitted if leaf flag is set.">4     Page number of the left child. Omitted if leaf flag is set.</param>
///<param name="var    Number of bytes of data. Omitted if the zerodata flag is set.">var    Number of bytes of data. Omitted if the zerodata flag is set.</param>
///<param name="var    Number of bytes of key. Or the key itself if intkey flag is set.">var    Number of bytes of key. Or the key itself if intkey flag is set.</param>
///<param name="Payload">Payload</param>
///<param name="4     First page of the overflow chain.  Omitted if no overflow">4     First page of the overflow chain.  Omitted if no overflow</param>
///<param name=""></param>
///<param name="Overflow pages form a linked list.  Each page except the last is completely">Overflow pages form a linked list.  Each page except the last is completely</param>
///<param name="filled with data (pagesize "> 4 bytes).  The last page can have as little</param>
///<param name="as 1 byte of data.">as 1 byte of data.</param>
///<param name=""></param>
///<param name="SIZE    DESCRIPTION">SIZE    DESCRIPTION</param>
///<param name="4     Page number of next overflow page">4     Page number of next overflow page</param>
///<param name="Data">Data</param>
///<param name=""></param>
///<param name="Freelist pages come in two subtypes: trunk pages and leaf pages.  The">Freelist pages come in two subtypes: trunk pages and leaf pages.  The</param>
///<param name="file header points to the first in a linked list of trunk page.  Each trunk">file header points to the first in a linked list of trunk page.  Each trunk</param>
///<param name="page points to multiple leaf pages.  The content of a leaf page is">page points to multiple leaf pages.  The content of a leaf page is</param>
///<param name="unspecified.  A trunk page looks like this:">unspecified.  A trunk page looks like this:</param>
///<param name=""></param>
///<param name="SIZE    DESCRIPTION">SIZE    DESCRIPTION</param>
///<param name="4     Page number of next trunk page">4     Page number of next trunk page</param>
///<param name="4     Number of leaf pointers on this page">4     Number of leaf pointers on this page</param>
///<param name="zero or more pages numbers of leaves">zero or more pages numbers of leaves</param>
///<param name=""></param>
///<param name="Included in SQLite3 port to C#">SQLite;  2008 Noah B Hart</param>
///<param name="C#">SQLite is an independent reimplementation of the SQLite software library</param>
///<param name=""></param>
///<param name="SQLITE_SOURCE_ID: 2011">19 13:26:54 ed1da510a239ea767a01dc332b667119fa3c908e</param>
///<param name=""></param>
///<param name=""></param>
///<param name=""></param>

		

		///
///<summary>
///Forward declarations 
///</summary>

		//typedef struct MemPage MemPage;
		//typedef struct BtLock BtLock;
		///

		






		
		


		
		


		
		///<summary>
		///A bunch of Debug.Assert() statements to check the transaction state variables
		/// of handle p (type Btree*) are internally consistent.
		///
		///</summary>
		#if DEBUG
																																														    //define btreeIntegrity(p) \
    //  Debug.Assert( p.pBt.inTransaction!=TRANS_NONE || p.pBt.nTransaction==0 ); \
    //  Debug.Assert( p.pBt.inTransaction>=p.inTrans );
    static void btreeIntegrity( Btree p )
    {
      Debug.Assert( p.pBt.inTransaction != TRANS_NONE || p.pBt.nTransaction == 0 );
      Debug.Assert( p.pBt.inTransaction >= p.inTrans );
    }
#else
		public static void btreeIntegrity (Btree p)
		{
		}

		#endif
		///
///<summary>
///The ISAUTOVACUUM macro is used within balance_nonroot() to determine
///</summary>
///<param name="if the database supports auto">vacuum or not. Because it is used</param>
///<param name="within an expression that is an argument to another macro">within an expression that is an argument to another macro</param>
///<param name="(sqliteMallocRaw), it is not possible to use conditional compilation.">(sqliteMallocRaw), it is not possible to use conditional compilation.</param>
///<param name="So, this macro is defined instead.">So, this macro is defined instead.</param>

		#if !SQLITE_OMIT_AUTOVACUUM
		//#define ISAUTOVACUUM (pBt.autoVacuum)
		#else
																																														//define ISAUTOVACUUM 0
public static bool ISAUTOVACUUM =false;
#endif

		
	
	//#define get4byte sqlite3Get4byte
	//#define put4byte sqlite3Put4byte
	}


    ///
    ///<summary>
    ///The pointer map is a lookup table that identifies the parent page for
    ///each child page in the database file.  The parent page is the page that
    ///contains a pointer to the child.  Every page in the database contains
    ///0 or 1 parent pages.  (In this context 'database page' refers
    ///to any page that is not part of the pointer map itself.)  Each pointer map
    ///entry consists of a single byte 'type' and a 4 byte parent page number.
    ///The PTRMAP_XXX identifiers below are the valid types.
    ///
    ///The purpose of the pointer map is to facility moving pages from one
    ///position in the file to another as part of autovacuum.  When a page
    ///is moved, the pointer in its parent must be updated to point to the
    ///new location.  The pointer map is used to locate the parent page quickly.
    ///
    ///</summary>
    ///<param name="PTRMAP_ROOTPAGE: The database page is a root">number is not</param>
    ///<param name="used in this case.">used in this case.</param>
    ///<param name=""></param>
    ///<param name="PTRMAP_FREEPAGE: The database page is an unused (free) page. The page">number</param>
    ///<param name="is not used in this case.">is not used in this case.</param>
    ///<param name=""></param>
    ///<param name="PTRMAP_OVERFLOW1: The database page is the first page in a list of">PTRMAP_OVERFLOW1: The database page is the first page in a list of</param>
    ///<param name="overflow pages. The page number identifies the page that">overflow pages. The page number identifies the page that</param>
    ///<param name="contains the cell with a pointer to this overflow page.">contains the cell with a pointer to this overflow page.</param>
    ///<param name=""></param>
    ///<param name="PTRMAP_OVERFLOW2: The database page is the second or later page in a list of">PTRMAP_OVERFLOW2: The database page is the second or later page in a list of</param>
    ///<param name="overflow pages. The page">number identifies the previous</param>
    ///<param name="page in the overflow page list.">page in the overflow page list.</param>
    ///<param name=""></param>
    ///<param name="PTRMAP_BTREE: The database page is a non">root btree page. The page number</param>
    ///<param name="identifies the parent page in the btree.">identifies the parent page in the btree.</param>
    ///<param name=""></param>

    //#define PTRMAP_ROOTPAGE 1
    //#define PTRMAP_FREEPAGE 2
    //#define PTRMAP_OVERFLOW1 3
    //#define PTRMAP_OVERFLOW2 4
    //#define PTRMAP_BTREE 5
    public static class PTRMAP
    {
        public const int ROOTPAGE = 1;

        public const int FREEPAGE = 2;

        public const int OVERFLOW1 = 3;

        public const int OVERFLOW2 = 4;

        public const int BTREE = 5;
    }


    ///<summary>
    ///Candidate values for BtLock.eLock
    ///</summary>
    //#define READ_LOCK     1
    //#define WRITE_LOCK    2
    public enum BtLockType
    {
        READ_LOCK = 1,

        WRITE_LOCK = 2
    };

}
