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
    public partial class BtShared
    {
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
        /// If in the future we call PagerMethods.sqlite3PagerWrite() on this page, that
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
        ///<summary>
        /// Get a page from the pager and initialize it.  This routine is just a
        /// convenience wrapper around separate calls to btreeGetPage() and
        /// btreeInitPage().
        ///
        /// If an error occurs, then the value ppPage is set to is undefined. It
        /// may remain unchanged, or it may be set to an invalid value.
        ///</summary>
        public SqlResult getAndInitPage(
            
                Pgno pgno,///Number of the page to get 
                ref MemPage ppPage///Write the page pointer here 
            )
        {
            BtShared pBt = this;///The database file 
            SqlResult rc;
            Debug.Assert(pBt.mutex.sqlite3_mutex_held());
            if (pgno > pBt.btreePagecount())
            {
                rc = sqliteinth.SQLITE_CORRUPT_BKPT();
            }
            else
            {
                rc = pBt.btreeGetPage(pgno, ref ppPage, 0);
                if (rc == SqlResult.SQLITE_OK)
                {
                    rc = ppPage.btreeInitPage();
                    if (rc != SqlResult.SQLITE_OK)
                    {
                        BTreeMethods.releasePage(ppPage);
                    }
                }
            }
            sqliteinth.testcase(pgno == 0);
            Debug.Assert(pgno != 0 || rc == SqlResult.SQLITE_CORRUPT);
            return rc;
        }






        ///
        ///<summary>
        ///Erase the given database page and all its children.  Return
        ///the page to the freelist.
        ///</summary>
        public  SqlResult clearDatabasePage(
            Pgno pgno,///<summary>
                      ///Page number to clear 
                      ///</summary>
            int freePageFlag,///<summary>
                             ///Deallocate page if true 
                             ///</summary>
            ref int pnChange///<summary>
                            ///Add number of Cells freed to this counter 
                            ///</summary>
            )
        {
            BtShared pBt = this;///<summary>
                               ///The BTree that contains the table 
                               ///</summary>
            MemPage pPage = new MemPage();
            SqlResult rc;
            byte[] pCell;
            int i;
            Debug.Assert(pBt.mutex.sqlite3_mutex_held());
            if (pgno > pBt.btreePagecount())
            {
                return sqliteinth.SQLITE_CORRUPT_BKPT();
            }
            rc = pBt.getAndInitPage(pgno, ref pPage);
            if (rc != 0)
                return rc;
            for (i = 0; i < pPage.nCell; i++)
            {
                int iCell = pPage.findCellAddress(i);
                pCell = pPage.aData;
                //        pCell = findCell( pPage, i );
                if (false == pPage.IsLeaf)
                {
                    rc = pBt.clearDatabasePage( Converter.sqlite3Get4byte(pCell, iCell), 1, ref pnChange);
                    if (rc != 0)
                        goto cleardatabasepage_out;
                }
                rc = pPage.clearCell( iCell);
                if (rc != 0)
                    goto cleardatabasepage_out;
            }
            if (false == pPage.IsLeaf)
            {
                rc = pBt.clearDatabasePage( Converter.sqlite3Get4byte(pPage.aData, 8), 1, ref pnChange);
                if (rc != 0)
                    goto cleardatabasepage_out;
            }
            else//if (pnChange != 0)
            {
                //Debug.Assert(pPage.intKey != 0);
                pnChange += pPage.nCell;
            }
            if (freePageFlag != 0)
            {
                pPage.freePage( ref rc);
            }
            else
                if ((rc = PagerMethods.sqlite3PagerWrite(pPage.pDbPage)) == 0)
            {
                pPage.zeroPage((PTF)pPage.aData[0] | PTF.LEAF);
            }
            cleardatabasepage_out:
            BTreeMethods.releasePage(pPage);
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
        public SqlResult freePage2( MemPage pMemPage, Pgno iPage)
        {
            BtShared pBt = this;
            MemPage pTrunk = null;
            ///<param name="Free">list trunk page </param>
            Pgno iTrunk = 0;
            ///<param name="Page number of free">list trunk page </param>
            MemPage pPage1 = pBt.pPage1;
            ///Local reference to page 1 
            MemPage pPage;
            ///Page being freed. May be NULL. 
            SqlResult rc;
            ///Return Code 
            int nFree;
            ///<param name="Initial number of pages on free">list </param>
            Debug.Assert(pBt.mutex.sqlite3_mutex_held());
            Debug.Assert(iPage > 1);
            Debug.Assert(null == pMemPage || pMemPage.pgno == iPage);
            if (pMemPage != null)
            {
                pPage = pMemPage;
                PagerMethods.sqlite3PagerRef(pPage.pDbPage);
            }
            else
            {
                pPage = pBt.btreePageLookup(iPage);
            }
            ///Increment the free page count on pPage1 
            rc = PagerMethods.sqlite3PagerWrite(pPage1.pDbPage);
            if (rc != 0)
                goto freepage_out;
            nFree = (int)Converter.sqlite3Get4byte(pPage1.aData, 36);
            Converter.sqlite3Put4byte(pPage1.aData, 36, nFree + 1);
            if (pBt.secureDelete)
            {
                ///If the secure_delete option is enabled, then
                ///always fully overwrite deleted information with zeros.
                if ((null == pPage && ((rc = pBt.btreeGetPage(iPage, ref pPage, 0)) != 0)) || ((rc = PagerMethods.sqlite3PagerWrite(pPage.pDbPage)) != 0))
                {
                    goto freepage_out;
                }
                Array.Clear(pPage.aData, 0, (int)pPage.pBt.pageSize);
                //memset(pPage->aData, 0, pPage->pBt->pageSize);
            }
            ///<param name="If the database supports auto">map</param>
            ///<param name="to indicate that the page is free.">to indicate that the page is free.</param>
            ///<param name=""></param>
#if !SQLITE_OMIT_AUTOVACUUM
            if (pBt.autoVacuum)
#else
																																																																											if (false)
#endif
            {
                pBt.ptrmapPut(iPage, PTRMAP.FREEPAGE, 0, ref rc);
                if (rc != 0)
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
            if (nFree != 0)
            {
                u32 nLeaf;
                ///
                ///<summary>
                ///Initial number of leaf cells on trunk page 
                ///</summary>
                iTrunk = Converter.sqlite3Get4byte(pPage1.aData, 32);
                rc = pBt.btreeGetPage(iTrunk, ref pTrunk, 0);
                if (rc != SqlResult.SQLITE_OK)
                {
                    goto freepage_out;
                }
                nLeaf = Converter.sqlite3Get4byte(pTrunk.aData, 4);
                Debug.Assert(pBt.usableSize > 32);
                if (nLeaf > (u32)pBt.usableSize / 4 - 2)
                {
                    rc = sqliteinth.SQLITE_CORRUPT_BKPT();
                    goto freepage_out;
                }
                if (nLeaf < (u32)pBt.usableSize / 4 - 8)
                {
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
                    rc = PagerMethods.sqlite3PagerWrite(pTrunk.pDbPage);
                    if (rc == SqlResult.SQLITE_OK)
                    {
                        Converter.sqlite3Put4byte(pTrunk.aData, (u32)4, nLeaf + 1);
                        Converter.sqlite3Put4byte(pTrunk.aData, (u32)8 + nLeaf * 4, iPage);
                        if (pPage != null && !pBt.secureDelete)
                        {
                            PagerMethods.sqlite3PagerDontWrite(pPage.pDbPage);
                        }
                        rc = pBt.btreeSetHasContent(iPage);
                    }
                    Log.TRACE("FREE-PAGE: %d leaf on trunk page %d\n", iPage, pTrunk.pgno);
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
            if (pPage == null && SqlResult.SQLITE_OK != (rc = pBt.btreeGetPage(iPage, ref pPage, 0)))
            {
                goto freepage_out;
            }
            rc = PagerMethods.sqlite3PagerWrite(pPage.pDbPage);
            if (rc != SqlResult.SQLITE_OK)
            {
                goto freepage_out;
            }
            Converter.sqlite3Put4byte(pPage.aData, iTrunk);
            Converter.sqlite3Put4byte(pPage.aData, 4, 0);
            Converter.sqlite3Put4byte(pPage1.aData, (u32)32, iPage);
            Log.TRACE("FREE-PAGE: %d new trunk page replacing %d\n", pPage.pgno, iTrunk);
            freepage_out:
            if (pPage != null)
            {
                pPage.isInit = false;
            }
            BTreeMethods.releasePage(pPage);
            BTreeMethods.releasePage(pTrunk);
            return rc;
        }

    }
}
