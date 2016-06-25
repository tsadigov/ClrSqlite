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
using Community.CsharpSqlite.Paging;
using Community.CsharpSqlite.Metadata;
using Community.CsharpSqlite.Utils;
using Community.CsharpSqlite.Cache;


namespace Community.CsharpSqlite.tree
{
    using DbPage = Cache.PgHdr;

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

    public partial class BtShared
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
            return (p != null && (pgno > p.sqlite3BitvecSize() || p.sqlite3BitvecTest(pgno) != 0));
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
            pPtrmap = pDbPage.getData();
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
            pPtrmap = pDbPage.getData();
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
            rc = this.pPager.Acquire(pgno, ref pDbPage, (u8)noContent);
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
