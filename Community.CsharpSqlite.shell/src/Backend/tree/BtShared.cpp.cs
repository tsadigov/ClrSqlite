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


namespace Community.CsharpSqlite.Tree
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
                rc = pBt.GetPage(pgno, ref ppPage, 0);
                if (rc == SqlResult.SQLITE_OK)
                {
                    rc = ppPage.btreeInitPage();
                    if (rc != SqlResult.SQLITE_OK)
                    {
                        BTreeMethods.release(ppPage);
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
            BTreeMethods.release(pPage);
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
                if ((null == pPage && ((rc = pBt.GetPage(iPage, ref pPage, 0)) != 0)) || ((rc = PagerMethods.sqlite3PagerWrite(pPage.pDbPage)) != 0))
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
                rc = pBt.GetPage(iTrunk, ref pTrunk, 0);
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
            if (pPage == null && SqlResult.SQLITE_OK != (rc = pBt.GetPage(iPage, ref pPage, 0)))
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
            BTreeMethods.release(pPage);
            BTreeMethods.release(pTrunk);
            return rc;
        }






        ///<summary>
        ///Allocate a new page from the database file.
        ///
        ///The new page is marked as dirty.  (In other words, PagerMethods.sqlite3PagerWrite()
        ///has already been called on the new page.)  The new page has also
        ///been referenced and the calling routine is responsible for calling
        ///PagerMethods.sqlite3PagerUnref() on the new page when it is done.
        ///
        ///SqlResult.SQLITE_OK is returned on success.  Any other return value indicates
        ///an error.  ppPage and pPgno are undefined in the event of an error.
        ///Do not invoke PagerMethods.sqlite3PagerUnref() on ppPage if an error is returned.
        ///
        ///If the "nearby" parameter is not 0, then a (feeble) effort is made to
        ///locate a page close to the page number "nearby".  This can be used in an
        ///attempt to keep related pages close to each other in the database file,
        ///which in turn can make database access faster.
        ///
        ///</summary>
        ///<param name="If the "exact" parameter is not 0, and the page">number nearby exists</param>
        ///<param name="anywhere on the free">list, then it is guarenteed to be returned. This</param>
        ///<param name="is only used by auto">vacuum databases when allocating a new table.</param>
        public SqlResult allocateBtreePage( ref MemPage ppPage, ref Pgno pPgno, Pgno nearby, u8 exact)
        {
            var pBt = this;
            SqlResult rc;
            u32 k;///Number of leaves on the trunk of the freelist 
            MemPage pTrunk = null;
            MemPage pPrevTrunk = null;
            Pgno mxPage;
            ///Total size of the database file 
            Debug.Assert(pBt.mutex.sqlite3_mutex_held());
            var pPage1 = pBt.pPage1;
            mxPage = pBt.btreePagecount();
            var n = Converter.sqlite3Get4byte(pPage1.aData, Offsets.NumberOfFreePage);///Number of pages on the freelist 
            sqliteinth.testcase(n == mxPage - 1);
            if (n >= mxPage)
            {
                return sqliteinth.SQLITE_CORRUPT_BKPT();
            }
            if (n > 0)
            {
                ///There are pages on the freelist.  Reuse one of those pages. 
                Pgno iTrunk;
                u8 searchList = 0;
                ///<param name="If the free">list must be searched for 'nearby' </param>
                ///<param name="If the 'exact' parameter was true and a query of the pointer">map</param>
                ///<param name="shows that the page 'nearby' is somewhere on the free">list, then</param>
                ///<param name="the entire">list will be searched for that page.</param>
#if !SQLITE_OMIT_AUTOVACUUM
                if (exact != 0 && nearby <= mxPage)
                {
                    u8 eType = 0;
                    Debug.Assert(nearby > 0);
                    Debug.Assert(pBt.autoVacuum);
                    u32 Dummy0 = 0;
                    rc = pBt.ptrmapGet(nearby, ref eType, ref Dummy0);
                    if (rc != 0)
                        return rc;
                    if (eType == PTRMAP.FREEPAGE)
                    {
                        searchList = 1;
                    }
                    pPgno = nearby;
                }
#endif
                ///<param name="Decrement the free">list count by 1. Set iTrunk to the index of the</param>
                ///<param name="first free">list trunk page. iPrevTrunk is initially 1.</param>
                rc = PagerMethods.sqlite3PagerWrite(pPage1.pDbPage);
                if (rc != 0)
                    return rc;
                Converter.sqlite3Put4byte(pPage1.aData, (u32)36, n - 1);
                ///The code within this loop is run only once if the 'searchList' variable
                ///<param name="is not true. Otherwise, it runs once for each trunk">page on the</param>
                ///<param name="free">list until the page 'nearby' is located.</param>
                do
                {
                    pPrevTrunk = pTrunk;
                    if (pPrevTrunk != null)
                    {
                        iTrunk = Converter.sqlite3Get4byte(pPrevTrunk.aData, 0);
                    }
                    else
                    {
                        iTrunk = Converter.sqlite3Get4byte(pPage1.aData, 32);
                    }
                    sqliteinth.testcase(iTrunk == mxPage);
                    if (iTrunk > mxPage)
                    {
                        rc = sqliteinth.SQLITE_CORRUPT_BKPT();
                    }
                    else
                    {
                        rc = pBt.GetPage(iTrunk, ref pTrunk, 0);
                    }
                    if (rc != 0)
                    {
                        pTrunk = null;
                        goto end_allocate_page;
                    }
                    k = Converter.sqlite3Get4byte(pTrunk.aData, 4);
                    ///# of leaves on this trunk page 
                    if (k == 0 && 0 == searchList)
                    {
                        ///The trunk has no leaves and the list is not being searched.
                        ///So extract the trunk page itself and use it as the newly
                        ///allocated page 
                        Debug.Assert(pPrevTrunk == null);
                        rc = PagerMethods.sqlite3PagerWrite(pTrunk.pDbPage);
                        if (rc != 0)
                        {
                            goto end_allocate_page;
                        }
                        pPgno = iTrunk;
                        Buffer.BlockCopy(pTrunk.aData, 0, pPage1.aData, 32, 4);
                        //memcpy( pPage1.aData[32], ref pTrunk.aData[0], 4 );
                        ppPage = pTrunk;
                        pTrunk = null;
                        Log.TRACE("ALLOCATE: %d trunk - %d free pages left\n", pPgno, n - 1);
                    }
                    else
                        if (k > (u32)(pBt.usableSize / 4 - 2))
                    {
                        ///Value of k is out of range.  Database corruption 
                        rc = sqliteinth.SQLITE_CORRUPT_BKPT();
                        goto end_allocate_page;
#if !SQLITE_OMIT_AUTOVACUUM
                    }
                    else
                            if (searchList != 0 && nearby == iTrunk)
                    {
                        ///The list is being searched and this trunk page is the page
                        ///to allocate, regardless of whether it has leaves.
                        Debug.Assert(pPgno == iTrunk);
                        ppPage = pTrunk;
                        searchList = 0;
                        rc = PagerMethods.sqlite3PagerWrite(pTrunk.pDbPage);
                        if (rc != 0)
                        {
                            goto end_allocate_page;
                        }
                        if (k == 0)
                        {
                            if (null == pPrevTrunk)
                            {
                                //memcpy(pPage1.aData[32], pTrunk.aData[0], 4);
                                pPage1.aData[32 + 0] = pTrunk.aData[0 + 0];
                                pPage1.aData[32 + 1] = pTrunk.aData[0 + 1];
                                pPage1.aData[32 + 2] = pTrunk.aData[0 + 2];
                                pPage1.aData[32 + 3] = pTrunk.aData[0 + 3];
                            }
                            else
                            {
                                rc = PagerMethods.sqlite3PagerWrite(pPrevTrunk.pDbPage);
                                if (rc != SqlResult.SQLITE_OK)
                                {
                                    goto end_allocate_page;
                                }
                                //memcpy(pPrevTrunk.aData[0], pTrunk.aData[0], 4);
                                pPrevTrunk.aData[0 + 0] = pTrunk.aData[0 + 0];
                                pPrevTrunk.aData[0 + 1] = pTrunk.aData[0 + 1];
                                pPrevTrunk.aData[0 + 2] = pTrunk.aData[0 + 2];
                                pPrevTrunk.aData[0 + 3] = pTrunk.aData[0 + 3];
                            }
                        }
                        else
                        {
                            ///
                            ///<summary>
                            ///The trunk page is required by the caller but it contains
                            ///</summary>
                            ///<param name="pointers to free">list leaves. The first leaf becomes a trunk</param>
                            ///<param name="page in this case.">page in this case.</param>
                            ///<param name=""></param>
                            MemPage pNewTrunk = new MemPage();
                            Pgno iNewTrunk = Converter.sqlite3Get4byte(pTrunk.aData, 8);
                            if (iNewTrunk > mxPage)
                            {
                                rc = sqliteinth.SQLITE_CORRUPT_BKPT();
                                goto end_allocate_page;
                            }
                            sqliteinth.testcase(iNewTrunk == mxPage);
                            rc = pBt.GetPage(iNewTrunk, ref pNewTrunk, 0);
                            if (rc != SqlResult.SQLITE_OK)
                            {
                                goto end_allocate_page;
                            }
                            rc = PagerMethods.sqlite3PagerWrite(pNewTrunk.pDbPage);
                            if (rc != SqlResult.SQLITE_OK)
                            {
                                BTreeMethods.release(pNewTrunk);
                                goto end_allocate_page;
                            }
                            //memcpy(pNewTrunk.aData[0], pTrunk.aData[0], 4);
                            pNewTrunk.aData[0 + 0] = pTrunk.aData[0 + 0];
                            pNewTrunk.aData[0 + 1] = pTrunk.aData[0 + 1];
                            pNewTrunk.aData[0 + 2] = pTrunk.aData[0 + 2];
                            pNewTrunk.aData[0 + 3] = pTrunk.aData[0 + 3];
                            Converter.sqlite3Put4byte(pNewTrunk.aData, (u32)4, (u32)(k - 1));
                            Buffer.BlockCopy(pTrunk.aData, 12, pNewTrunk.aData, 8, (int)(k - 1) * 4);
                            //memcpy( pNewTrunk.aData[8], ref pTrunk.aData[12], ( k - 1 ) * 4 );
                            BTreeMethods.release(pNewTrunk);
                            if (null == pPrevTrunk)
                            {
                                Debug.Assert(pPage1.pDbPage.sqlite3PagerIswriteable());
                                Converter.sqlite3Put4byte(pPage1.aData, (u32)32, iNewTrunk);
                            }
                            else
                            {
                                rc = PagerMethods.sqlite3PagerWrite(pPrevTrunk.pDbPage);
                                if (rc != 0)
                                {
                                    goto end_allocate_page;
                                }
                                Converter.sqlite3Put4byte(pPrevTrunk.aData, (u32)0, iNewTrunk);
                            }
                        }
                        pTrunk = null;
                        Log.TRACE("ALLOCATE: %d trunk - %d free pages left\n", pPgno, n - 1);
#endif
                    }
                    else
                                if (k > 0)
                    {
                        ///
                        ///<summary>
                        ///Extract a leaf from the trunk 
                        ///</summary>
                        u32 closest;
                        Pgno iPage;
                        byte[] aData = pTrunk.aData;
                        if (nearby > 0)
                        {
                            u32 i;
                            int dist;
                            closest = 0;
                            dist = utilc.sqlite3AbsInt32((int)(Converter.sqlite3Get4byte(aData, 8) - nearby));
                            for (i = 1; i < k; i++)
                            {
                                int d2 = utilc.sqlite3AbsInt32((int)(Converter.sqlite3Get4byte(aData, 8 + i * 4) - nearby));
                                if (d2 < dist)
                                {
                                    closest = i;
                                    dist = d2;
                                }
                            }
                        }
                        else
                        {
                            closest = 0;
                        }
                        iPage = Converter.sqlite3Get4byte(aData, 8 + closest * 4);
                        sqliteinth.testcase(iPage == mxPage);
                        if (iPage > mxPage)
                        {
                            rc = sqliteinth.SQLITE_CORRUPT_BKPT();
                            goto end_allocate_page;
                        }
                        sqliteinth.testcase(iPage == mxPage);
                        if (0 == searchList || iPage == nearby)
                        {
                            int noContent;
                            pPgno = iPage;
                            Log.TRACE("ALLOCATE: %d was leaf %d of %d on trunk %d" + ": %d more free pages\n", pPgno, closest + 1, k, pTrunk.pgno, n - 1);
                            rc = PagerMethods.sqlite3PagerWrite(pTrunk.pDbPage);
                            if (rc != 0)
                                goto end_allocate_page;
                            if (closest < k - 1)
                            {
                                Buffer.BlockCopy(aData, (int)(4 + k * 4), aData, 8 + (int)closest * 4, 4);
                                //memcpy( aData[8 + closest * 4], ref aData[4 + k * 4], 4 );
                            }
                            Converter.sqlite3Put4byte(aData, (u32)4, (k - 1));
                            // sqlite3Put4byte( aData, 4, k - 1 );
                            noContent = !pBt.btreeGetHasContent(pPgno) ? 1 : 0;
                            rc = pBt.GetPage(pPgno, ref ppPage, noContent);
                            if (rc == SqlResult.SQLITE_OK)
                            {
                                rc = PagerMethods.sqlite3PagerWrite((ppPage).pDbPage);
                                if (rc != SqlResult.SQLITE_OK)
                                {
                                    BTreeMethods.release(ppPage);
                                }
                            }
                            searchList = 0;
                        }
                    }
                    BTreeMethods.release(pPrevTrunk);
                    pPrevTrunk = null;
                }
                while (searchList != 0);
            }
            else
            {
                ///
                ///<summary>
                ///There are no pages on the freelist, so create a new page at the
                ///end of the file 
                ///</summary>
                rc = PagerMethods.sqlite3PagerWrite(pBt.pPage1.pDbPage);
                if (rc != 0)
                    return rc;
                pBt.nPage++;
                if (pBt.nPage == pBt.PENDING_BYTE_PAGE)
                    pBt.nPage++;
#if !SQLITE_OMIT_AUTOVACUUM
                if (pBt.autoVacuum && BTreeMethods.PTRMAP_ISPAGE(pBt, pBt.nPage))
                {
                    ///<param name="If pPgno refers to a pointer">map page, allocate two new pages</param>
                    ///<param name="at the end of the file instead of one. The first allocated page">at the end of the file instead of one. The first allocated page</param>
                    ///<param name="becomes a new pointer">map page, the second is used by the caller.</param>
                    ///<param name=""></param>
                    MemPage pPg = null;
                    Log.TRACE("ALLOCATE: %d from end of file (pointer-map page)\n", pPgno);
                    Debug.Assert(pBt.nPage != (pBt.PENDING_BYTE_PAGE));
                    rc = pBt.GetPage(pBt.nPage, ref pPg, 1);
                    if (rc == SqlResult.SQLITE_OK)
                    {
                        rc = PagerMethods.sqlite3PagerWrite(pPg.pDbPage);
                        BTreeMethods.release(pPg);
                    }
                    if (rc != 0)
                        return rc;
                    pBt.nPage++;
                    if (pBt.nPage == pBt.PENDING_BYTE_PAGE)
                    {
                        pBt.nPage++;
                    }
                }
#endif
                Converter.sqlite3Put4byte(pBt.pPage1.aData, (u32)28, pBt.nPage);
                pPgno = pBt.nPage;
                Debug.Assert(pPgno != pBt.PENDING_BYTE_PAGE);
                rc = pBt.GetPage(pPgno, ref ppPage, 1);
                if (rc != 0)
                    return rc;
                rc = PagerMethods.sqlite3PagerWrite((ppPage).pDbPage);
                if (rc != SqlResult.SQLITE_OK)
                {
                    BTreeMethods.release(ppPage);
                }
                Log.TRACE("ALLOCATE: %d from end of file\n", pPgno);
            }
            Debug.Assert(pPgno != pBt.PENDING_BYTE_PAGE);
            end_allocate_page:
            BTreeMethods.release(pTrunk);
            BTreeMethods.release(pPrevTrunk);
            if (rc == SqlResult.SQLITE_OK)
            {
                if ((ppPage).pDbPage.sqlite3PagerPageRefcount() > 1)
                {
                    BTreeMethods.release(ppPage);
                    return sqliteinth.SQLITE_CORRUPT_BKPT();
                }
                (ppPage).isInit = false;
            }
            else
            {
                ppPage = null;
            }
            Debug.Assert(rc != SqlResult.SQLITE_OK || (ppPage).pDbPage.sqlite3PagerIswriteable());
            return rc;
        }







        ///
        ///<summary>
        ///Make sure pBt.pTmpSpace points to an allocation of
        ///MX_CELL_SIZE(pBt) bytes.
        ///</summary>
        public  void allocateTempSpace()
        {
            if (null == this.pTmpSpace)
            {
                this.pTmpSpace = malloc_cs.sqlite3Malloc(this.pageSize);
            }
        }
        ///
        ///<summary>
        ///Free the pBt.pTmpSpace allocation
        ///</summary>
        public void freeTempSpace()
        {
            Cache.CacheMethods.sqlite3PageFree(ref this.pTmpSpace);
        }

        ///
        ///<summary>
        ///Decrement the BtShared.nRef counter.  When it reaches zero,
        ///remove the BtShared structure from the sharing list.  Return
        ///true if the BtShared.nRef counter reaches zero and return
        ///false if it is still positive.
        ///</summary>
        public bool removeFromSharingList()
        {
#if !SQLITE_OMIT_SHARED_CACHE
																																																																											sqlite3_mutex pMaster;
BtShared pList;
bool removed = false;

Debug.Assert( sqlite3_mutex_notheld(pBt.mutex) );
pMaster = sqlite3MutexAlloc(SQLITE_MUTEX_STATIC_MASTER);
$.sqlite3_mutex_enter();
pBt.nRef--;
if( pBt.nRef<=0 ){
if( GLOBAL(BtShared*,sqlite3SharedCacheList)==pBt ){
GLOBAL(BtShared*,sqlite3SharedCacheList) = pBt.pNext;
}else{
pList = GLOBAL(BtShared*,sqlite3SharedCacheList);
while( Sqlite3.ALWAYS(pList) && pList.pNext!=pBt ){
pList=pList.pNext;
}
if( Sqlite3.ALWAYS(pList) ){
pList.pNext = pBt.pNext;
}
}
if( SQLITE_THREADSAFE ){
sqlite3_mutex_free(pBt.mutex);
}
removed = true;
}
pMaster.sqlite3_mutex_leave();
return removed;
#else
            return true;
#endif
        }

        ///
        ///<summary>
        ///</summary>
        ///<param name="Change the 'auto">vacuum' property of the database. If the 'autoVacuum'</param>
        ///<param name="parameter is non">vacuum mode is enabled. If zero, it</param>
        ///<param name="is disabled. The default value for the auto">vacuum property is</param>
        ///<param name="determined by the SQLITE_DEFAULT_AUTOVACUUM macro.">determined by the SQLITE_DEFAULT_AUTOVACUUM macro.</param>
        ///
        ///<summary>
        ///</summary>
        ///<param name="Return the value of the 'auto">vacuum is</param>
        ///<param name="enabled 1 is returned. Otherwise 0.">enabled 1 is returned. Otherwise 0.</param>
        ///
        ///<summary>
        ///Get a reference to pPage1 of the database file.  This will
        ///also acquire a readlock on that file.
        ///
        ///SqlResult.SQLITE_OK is returned on success.  If the file is not a
        ///</summary>
        ///<param name="well">formed database file, then SQLITE_CORRUPT is returned.</param>
        ///<param name="SQLITE_BUSY is returned if the database is locked.  SQLITE_NOMEM">SQLITE_BUSY is returned if the database is locked.  SQLITE_NOMEM</param>
        ///<param name="is returned if we run out of memory.">is returned if we run out of memory.</param>
        public  SqlResult lockBtree()
        {
            BtShared pBt = this;
            SqlResult rc;
            ///Result code from subfunctions 
            MemPage pPage1 = null;
            ///Page 1 of the database file 
            Pgno nPage;
            ///Number of pages in the database 
            Pgno nPageFile = 0;
            ///Number of pages in the database file 
            Pgno nPageHeader;
            ///Number of pages in the database according to hdr 
            Debug.Assert(pBt.mutex.sqlite3_mutex_held());
            Debug.Assert(pBt.pPage1 == null);
            rc = pBt.pPager.sqlite3PagerSharedLock();
            if (rc != SqlResult.SQLITE_OK)
                return rc;
            rc = pBt.GetPage(1, ref pPage1, 0);
            if (rc != SqlResult.SQLITE_OK)
                return rc;
            ///Do some checking to help insure the file we opened really is
            ///a valid database file.
            nPage = nPageHeader = Converter.sqlite3Get4byte(pPage1.aData, 28);
            //get4byte(28+(u8*)pPage1->aData);
            pBt.pPager.sqlite3PagerPagecount(out nPageFile);
            if (nPage == 0 || _Custom.memcmp(pPage1.aData, 24, pPage1.aData, 92, 4) != 0)//_Custom.memcmp(24 + (u8*)pPage1.aData, 92 + (u8*)pPage1.aData, 4) != 0)
            {
                nPage = nPageFile;
            }
            if (nPage > 0)
            {
                u32 pageSize;
                u32 usableSize;
                u8[] page1 = pPage1.aData;
                rc = SqlResult.SQLITE_NOTADB;
                if (_Custom.memcmp(page1, Globals.zMagicHeader, 16) != 0)
                {
                    goto page1_init_failed;
                }
#if SQLITE_OMIT_WAL
                if (page1[18] > 1)
                {
                    pBt.readOnly = true;
                }
                if (page1[19] > 1)
                {
                    pBt.pSchema.file_format = page1[19];
                    goto page1_init_failed;
                }
#else
																																																																																																				if( page1[18]>2 ){
pBt.readOnly = true;
}
if( page1[19]>2 ){
goto page1_init_failed;
}

/* If the write version is set to 2, this database should be accessed
** in WAL mode. If the log is not already open, open it now. Then 
** return SqlResult.SQLITE_OK and return without populating BtShared.pPage1.
** The caller detects this and calls this function again. This is
** required as the version of page 1 currently in the page1 buffer
** may not be the latest version - there may be a newer one in the log
** file.
*/
if( page1[19]==2 && pBt.doNotUseWAL==false ){
int isOpen = 0;
rc = sqlite3PagerOpenWal(pBt.pPager, ref isOpen);
if( rc!=SqlResult.SQLITE_OK ){
goto page1_init_failed;
}else if( isOpen==0 ){
releasePage(pPage1);
return SqlResult.SQLITE_OK;
}
rc = SQLITE_NOTADB;
}
#endif
                ///
                ///<summary>
                ///The maximum embedded fraction must be exactly 25%.  And the minimum
                ///</summary>
                ///<param name="embedded fraction must be 12.5% for both leaf">data.</param>
                ///<param name="The original design allowed these amounts to vary, but as of">The original design allowed these amounts to vary, but as of</param>
                ///<param name="version 3.6.0, we require them to be fixed.">version 3.6.0, we require them to be fixed.</param>
                if (_Custom.memcmp(page1, 21, "\x0040\x0020\x0020", 3) != 0)//   "\100\040\040"
                {
                    goto page1_init_failed;
                }
                pageSize = (u32)((page1[16] << 8) | (page1[17] << 16));
                if (((pageSize - 1) & pageSize) != 0 || pageSize > Limits.SQLITE_MAX_PAGE_SIZE || pageSize <= 256)
                {
                    goto page1_init_failed;
                }
                Debug.Assert((pageSize & 7) == 0);
                usableSize = pageSize - page1[20];
                if (pageSize != pBt.pageSize)
                {
                    ///
                    ///<summary>
                    ///After reading the first page of the database assuming a page size
                    ///</summary>
                    ///<param name="of BtShared.pageSize, we have discovered that the page">size is</param>
                    ///<param name="actually pageSize. Unlock the database, leave pBt.pPage1 at">actually pageSize. Unlock the database, leave pBt.pPage1 at</param>
                    ///<param name="zero and return SqlResult.SQLITE_OK. The caller will call this function">zero and return SqlResult.SQLITE_OK. The caller will call this function</param>
                    ///<param name="again with the correct page">size.</param>
                    ///<param name=""></param>
                    BTreeMethods.release(pPage1);
                    pBt.usableSize = usableSize;
                    pBt.pageSize = pageSize;
                    //          freeTempSpace(pBt);
                    rc = pBt.pPager.sqlite3PagerSetPagesize(ref pBt.pageSize, (int)(pageSize - usableSize));
                    return rc;
                }
                if ((pBt.db.flags & SqliteFlags.SQLITE_RecoveryMode) == 0 && nPage > nPageFile)
                {
                    rc = sqliteinth.SQLITE_CORRUPT_BKPT();
                    goto page1_init_failed;
                }
                if (usableSize < 480)
                {
                    goto page1_init_failed;
                }
                pBt.pageSize = pageSize;
                pBt.usableSize = usableSize;
#if !SQLITE_OMIT_AUTOVACUUM
                pBt.autoVacuum = (Converter.sqlite3Get4byte(page1, 36 + 4 * 4) != 0);
                pBt.incrVacuum = (Converter.sqlite3Get4byte(page1, 36 + 7 * 4) != 0);
#endif
            }
            ///
            ///<summary>
            ///maxLocal is the maximum amount of payload to store locally for
            ///a cell.  Make sure it is small enough so that at least minFanout
            ///</summary>
            ///<param name="cells can will fit on one page.  We assume a 10">byte page header.</param>
            ///<param name="Besides the payload, the cell must store:">Besides the payload, the cell must store:</param>
            ///<param name="2">byte pointer to the cell</param>
            ///<param name="4">byte child pointer</param>
            ///<param name="9">byte nKey value</param>
            ///<param name="4">byte nData value</param>
            ///<param name="4">byte overflow page pointer</param>
            ///<param name="So a cell consists of a 2">byte pointer, a header which is as much as</param>
            ///<param name="17 bytes long, 0 to N bytes of payload, and an optional 4 byte overflow">17 bytes long, 0 to N bytes of payload, and an optional 4 byte overflow</param>
            ///<param name="page pointer.">page pointer.</param>
            ///<param name=""></param>
            pBt.maxLocal = (u16)((pBt.usableSize - 12) * 64 / 255 - 23);
            pBt.minLocal = (u16)((pBt.usableSize - 12) * 32 / 255 - 23);
            pBt.maxLeaf = (u16)(pBt.usableSize - 35);
            pBt.minLeaf = (u16)((pBt.usableSize - 12) * 32 / 255 - 23);
            Debug.Assert(pBt.maxLeaf + 23 <= pBt.MX_CELL_SIZE);
            pBt.pPage1 = pPage1;
            pBt.nPage = nPage;
            return SqlResult.SQLITE_OK;
            page1_init_failed:
            pPage1.release();
            pBt.pPage1 = null;
            return rc;
        }
        ///
        ///<summary>
        ///If there are no outstanding cursors and we are not in the middle
        ///of a transaction but there is a read lock on the database, then
        ///this routine unrefs the first page of the database file which
        ///has the effect of releasing the read lock.
        ///
        ///</summary>
        ///<param name="If there is a transaction in progress, this routine is a no">op.</param>
        public  void unlockIfUnused()
        {
            BtShared pBt = this;
            Debug.Assert(pBt.mutex.sqlite3_mutex_held());
            Debug.Assert(pBt.pCursor == null || pBt.inTransaction > TransType.TRANS_NONE);
            if (pBt.inTransaction == TransType.TRANS_NONE && pBt.pPage1 != null)
            {
                Debug.Assert(pBt.pPage1.aData != null);
                //Debug.Assert( sqlite3PagerRefcount( pBt.pPager ) == 1 );
                pBt.pPage1.release();
                pBt.pPage1 = null;
            }
        }
        ///
        ///<summary>
        ///If pBt points to an empty file then convert that empty file
        ///into a new empty database by initializing the first page of
        ///the database.
        ///</summary>
        public  SqlResult newDatabase()
        {
            BtShared pBt = this;
            Debug.Assert(pBt.mutex.sqlite3_mutex_held());
            if (pBt.nPage > 0)
            {
                return SqlResult.SQLITE_OK;
            }
            var pP1 = pBt.pPage1;
            Debug.Assert(pP1 != null);
            var data = pP1.aData;
            var rc = PagerMethods.sqlite3PagerWrite(pP1.pDbPage);
            if (rc != 0)
                return rc;
            Buffer.BlockCopy(Globals.zMagicHeader, 0, data, 0, 16);
            // memcpy(data, zMagicHeader, sizeof(zMagicHeader));
            Debug.Assert(Globals.zMagicHeader.Length == 16);
            data[16] = (u8)((pBt.pageSize >> 8) & 0xff);
            data[17] = (u8)((pBt.pageSize >> 16) & 0xff);
            data[18] = 1;
            data[19] = 1;
            Debug.Assert(pBt.usableSize <= pBt.pageSize && pBt.usableSize + 255 >= pBt.pageSize);
            data[20] = (u8)(pBt.pageSize - pBt.usableSize);
            data[21] = 64;
            data[22] = 32;
            data[23] = 32;
            //memset(&data[24], 0, 100-24);
            pP1.zeroPage(PTF.INTKEY | PTF.LEAF | PTF.LEAFDATA);
            pBt.pageSizeFixed = true;
#if !SQLITE_OMIT_AUTOVACUUM
            Debug.Assert(pBt.autoVacuum == true || pBt.autoVacuum == false);
            Debug.Assert(pBt.incrVacuum == true || pBt.incrVacuum == false);
            Converter.sqlite3Put4byte(data, 36 + 4 * 4, pBt.autoVacuum ? 1 : 0);
            Converter.sqlite3Put4byte(data, 36 + 7 * 4, pBt.incrVacuum ? 1 : 0);
#endif
            pBt.nPage = 1;
            data[31] = 1;
            return SqlResult.SQLITE_OK;
        }

    }
}
