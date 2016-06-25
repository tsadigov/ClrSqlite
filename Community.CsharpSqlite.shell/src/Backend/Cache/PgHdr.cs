using Community.CsharpSqlite.Paging;
using System.Diagnostics;
using Pgno = System.UInt32;
using u32 = System.UInt32;

namespace Community.CsharpSqlite.Cache
{
    using Utils;
    ///<summary>
    /// 2008 August 05
    ///
    /// The author disclaims copyright to this source code.  In place of
    /// a legal notice, here is a blessing:
    ///
    ///    May you do good and not evil.
    ///    May you find forgiveness for yourself and forgive others.
    ///    May you share freely, never taking more than you give.
    ///
    ///
    /// This header file defines the interface that the sqlite page cache
    /// subsystem.
    ///
    ///  Included in SQLite3 port to C#-SQLite;  2008 Noah B Hart
    ///  C#-SQLite is an independent reimplementation of the SQLite software library
    ///
    ///  SQLITE_SOURCE_ID: 2010-08-23 18:52:01 42537b60566f288167f1b5864a5435986838e3a3
    ///
    ///
    ///
    ///</summary>
#if !_PCACHE_H_
    //typedef struct PgHdr PgHdr;
    //typedef struct PCache PCache;
    ///
    ///<summary>
    ///Every page in the cache is controlled by an instance of the following
    ///structure.
    ///
    ///</summary>

    public partial class PgHdr:ReferenceCounted,ILinkedListNode<PgHdr>,IBackwardLinkedListNode<PgHdr>
    {
        ///<summary>
        ///pData Content of this page 
        ///</summary>
        public byte[] buffer;

        ///<summary>
        ///Extra content 
        ///</summary>
        public MemPage pExtra;

        ///

        ///<summary>
        ///Transient list of dirty pages 
        ///</summary>
        public PgHdr pDirty;

        ///

        ///<summary>
        ///The page number for this page 
        ///</summary>
        public Pgno pgno;

        ///

        ///<summary>
        ///The pager to which this page belongs 
        ///</summary>
        public Pager pPager;

        ///


#if SQLITE_CHECK_PAGES || (SQLITE_DEBUG)
																																																												      public int pageHash;          /* Hash of page content */
#endif
        ///<summary>
        ///PGHDR flags defined below 
        ///</summary>
        public PGHDR flags;

        ///<summary>
        ///Elements above are public.  All that follows is private to pcache.c
        ///and should not be accessed by other modules.
        ///</summary>


        ///<summary>
        ///Cache that owns this page 
        ///</summary>
        public PCache pCache { get; set; }

        ///<summary>
        ///True, if allocated from cache 
        ///</summary>
        public bool CacheAllocated;


        ///<summary>
        ///pDirtyNext
        ///Next element in list of dirty pages 
        ///</summary>
        public PgHdr pNext { get; set; }

        ///<summary>
        ///Previous element in list of dirty pages 
        ///</summary>
        public PgHdr pPrev { get; set; }


        ///<summary>
        ///PgHdr1
        ///Cache page header this this page
        ///</summary>
        public PgHdr1 pPgHdr1;


        public static implicit operator bool(PgHdr b)
        {
            return (b != null);
        }

        public void Clear()
        {            
            malloc_cs.sqlite3_free(ref this.buffer);
            this.buffer = null;
            this.pExtra = null;
            this.pDirty = null;
            this.pgno = 0;
            this.pPager = null;
#if SQLITE_CHECK_PAGES
																																																																																this.pageHash=0;
#endif
            this.flags = 0;
            this.ReferenceCount = 0;
            this.CacheAllocated = false;
            this.pCache = null;
            this.pNext = null;
            this.pPrev = null;
            this.pPgHdr1 = null;
        }

        public///<summary>subjRequiresPage
              /// Return true if it is necessary to write page *pPg into the sub-journal.
              /// A page needs to be written into the sub-journal if there exists one
              /// or more open savepoints for which:
              ///
              ///   * The page-number is less than or equal to PagerSavepoint.nOrig, and
              ///   * The bit corresponding to the page-number is not set in
              ///     PagerSavepoint.pInSavepoint.
              ///</summary>
            bool needsToBeWrittenToSubJournal()
        {
            u32 pgno = this.pgno;
            Pager pPager = this.pPager;
            int i;
            for (i = 0; i < pPager.nSavepoint; i++)
            {
                PagerSavepoint p = pPager.aSavepoint[i];
                if (p.nOrig >= pgno && 0 == p.pInSavepoint.sqlite3BitvecTest(pgno))
                {
                    return true;
                }
            }
            return false;
        }

        ///<summary>
        /// Return true if the page is already in the journal file.
        ///
        ///</summary>
        public bool pageInJournal()
        {
            return this.pPager.pInJournal.sqlite3BitvecTest(this.pgno) != 0;
        }

        public int pager_pagehash()
        {
            return 0;
        }

        public void pager_set_pagehash()
        {
        }

#if !NDEBUG || SQLITE_TEST
																																						    ///<summary>
/// Return the page number for page pPg.
///</summary>
    static Pgno sqlite3PagerPagenumber( DbPage pPg )
    {
      return pPg.pgno;
    }
#else
        /// <summary>
        /// sqlite3PagerPagenumber
        /// </summary>
        /// <returns></returns>
        public Pgno getPageNo()
        {
            return this.pgno;
        }

#endif


        ///<summary>
        ///sqlite3PagerGetExtra
        /// Return a pointer to the Pager.nExtra bytes of "extra" space
        /// allocated along with the specified page.
        ///
        ///</summary>
        public MemPage getExtra()
        {
            return this.pExtra;
        }

        public MemPage btreePageFromDbPage(Pgno pgno, Tree.BtShared pBt)
        {
            MemPage pPage = this.getExtra();
            pPage.aData = this.getData();
            pPage.pDbPage = this;
            pPage.pBt = pBt;
            pPage.pgno = pgno;
            //pPage.hdrOffset = (u8)(pPage.pgno == 1 ? 100 : 0);
            return pPage;
        }

        ///<summary>
        ///sqlite3PagerGetData
        /// Return a pointer to the data for the specified page.
        ///</summary>
        public byte[] getData()
        {
            PgHdr pPg = this;
            Debug.Assert(pPg.ReferenceCount > 0 || pPg.pPager.memDb != 0);
            return pPg.buffer;
        }


        ///<summary>
        /// Increase the reference count of a supplied page by 1.
        ///
        ///</summary>
        public void sqlite3PcacheRef()
        {
            Debug.Assert(this.ReferenceCount > 0);
            this.ReferenceCount++;
        }

        ///<summary>
        /// Update the value of the change-counter at offsets 24 and 92 in
        /// the header and the sqlite version number at offset 96.
        ///
        /// This is an unconditional update.  See also the pager_incr_changecounter()
        /// routine which only updates the change-counter if the update is actually
        /// needed, as determined by the pPager.changeCountDone state variable.
        ///
        ///</summary>
        public void pager_write_changecounter()
        {
            PgHdr pPg = this;
            
            ///Increment the value just read and write it back to byte 24. 

            u32 change_counter = Converter.sqlite3Get4byte(pPg.pPager.dbFileVers, 0) + 1;
            Converter.put32bits(pPg.buffer, 24, change_counter);

            ///Also store the SQLite version number in bytes 96..99 and in
            ///bytes 92..95 store the change counter for which the version number
            Converter.put32bits(pPg.buffer, Offsets.ChangeCounter, change_counter);
            Converter.put32bits(pPg.buffer, Offsets.SqliteVersion, Sqlite3.SQLITE_VERSION_NUMBER);
        }

        ///<summary>
        /// Return the number of references to the page supplied as an argument.
        ///
        ///</summary>
        public int sqlite3PcachePageRefcount()
        {
            return this.ReferenceCount;
        }

        ///<summary>
        /// Return the number of references to the specified page.
        ///
        ///</summary>
        public int sqlite3PagerPageRefcount()
        {
            return this.sqlite3PcachePageRefcount();
        }

    }
#endif
}
