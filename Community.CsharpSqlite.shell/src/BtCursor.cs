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
	using DbPage=PgHdr;
	using System.Text;
	public partial class Sqlite3 {



        ///
        ///<summary>
        ///A cursor is a pointer to a particular entry within a particular
        ///</summary>
        ///<param name="b">tree within a database file.</param>
        ///<param name=""></param>
        ///<param name="The entry is identified by its MemPage and the index in">The entry is identified by its MemPage and the index in</param>
        ///<param name="MemPage.aCell[] of the entry.">MemPage.aCell[] of the entry.</param>
        ///<param name=""></param>
        ///<param name="A single database file can shared by two more database connections,">A single database file can shared by two more database connections,</param>
        ///<param name="but cursors cannot be shared.  Each cursor is associated with a">but cursors cannot be shared.  Each cursor is associated with a</param>
        ///<param name="particular database connection identified BtCursor.pBtree.db.">particular database connection identified BtCursor.pBtree.db.</param>
        ///<param name=""></param>
        ///<param name="Fields in this structure are accessed under the BtShared.mutex">Fields in this structure are accessed under the BtShared.mutex</param>
        ///<param name="found at self.pBt.mutex.">found at self.pBt.mutex.</param>
        ///<param name=""></param>
        public class BtCursor
        {
            public Btree pBtree;
            ///
            ///<summary>
            ///The Btree to which this cursor belongs 
            ///</summary>
            public BtShared pBt;
            ///
            ///<summary>
            ///The BtShared this cursor points to 
            ///</summary>
            public BtCursor pNext;
            public BtCursor pPrev;
            ///
            ///<summary>
            ///Forms a linked list of all cursors 
            ///</summary>
            public KeyInfo pKeyInfo;
            ///
            ///<summary>
            ///Argument passed to comparison function 
            ///</summary>
            public Pgno pgnoRoot;
            ///
            ///<summary>
            ///The root page of this tree 
            ///</summary>
            public sqlite3_int64 cachedRowid;
            ///
            ///<summary>
            ///Next rowid cache.  0 means not valid 
            ///</summary>
            public CellInfo info = new CellInfo();
            ///
            ///<summary>
            ///A parse of the cell we are pointing at 
            ///</summary>
            public byte[] pKey;
            ///
            ///<summary>
            ///Saved key that was cursor's last known position 
            ///</summary>
            public i64 nKey;
            ///
            ///<summary>
            ///Size of pKey, or last integer key 
            ///</summary>
            public int skipNext;
            ///
            ///<summary>
            ///Prev() is noop if negative. Next() is noop if positive 
            ///</summary>
            public u8 wrFlag;
            ///
            ///<summary>
            ///True if writable 
            ///</summary>
            public u8 atLast;
            ///
            ///<summary>
            ///VdbeCursor pointing to the last entry 
            ///</summary>
            public bool validNKey;
            ///
            ///<summary>
            ///True if info.nKey is valid 
            ///</summary>
            public int eState;
            public BtCursorState State
            {
                get
                {
                    return (BtCursorState)eState;
                }
                set
                {
                    eState = (int)value;
                }
            }
            ///
            ///<summary>
            ///One of the CURSOR_XXX constants (see below) 
            ///</summary>
#if !SQLITE_OMIT_INCRBLOB
																																																																								public Pgno[] aOverflow;         /* Cache of overflow page locations */
public bool isIncrblobHandle;   /* True if this cursor is an incr. io handle */
#endif
            public i16 iPage;
            ///
            ///<summary>
            ///Index of current page in apPage 
            ///</summary>
            public u16[] aiIdx = new u16[BTCURSOR_MAX_DEPTH];
            ///<summary>
            ///Current index in apPage[i]
            ///</summary>
            public MemPage[] apPage = new MemPage[BTCURSOR_MAX_DEPTH];
            ///<summary>
            ///Pages from root to current page
            ///</summary>
            public void Clear()
            {
                pNext = null;
                pPrev = null;
                pKeyInfo = null;
                pgnoRoot = 0;
                cachedRowid = 0;
                info = new CellInfo();
                wrFlag = 0;
                atLast = 0;
                validNKey = false;
                eState = 0;
                pKey = null;
                nKey = 0;
                skipNext = 0;
#if !SQLITE_OMIT_INCRBLOB
																																																																																																isIncrblobHandle=false;
aOverflow= null;
#endif
                iPage = 0;
            }
            public BtCursor Copy()
            {
                BtCursor cp = (BtCursor)MemberwiseClone();
                return cp;
            }
            public bool cursorHoldsMutex()
            {
                return true;
            }
            public SqlResult saveCursorPosition()
            {
                SqlResult rc;
                Debug.Assert(CURSOR_VALID == this.eState);
                Debug.Assert(null == this.pKey);
                Debug.Assert(this.cursorHoldsMutex());
                rc = this.sqlite3BtreeKeySize(ref this.nKey);
                Debug.Assert(rc == SqlResult.SQLITE_OK);
                ///
                ///<summary>
                ///KeySize() cannot fail 
                ///</summary>
                ///
                ///<summary>
                ///If this is an intKey table, then the above call to BtreeKeySize()
                ///stores the integer key in pCur.nKey. In this case this value is
                ///all that is required. Otherwise, if pCur is not open on an intKey
                ///table, then malloc space for and store the pCur.nKey bytes of key
                ///data.
                ///</summary>
                if (false == this.apPage[0].intKey)
                {
                    byte[] pKey = malloc_cs.sqlite3Malloc((int)this.nKey);
                    //if( pKey !=null){
                    rc = this.sqlite3BtreeKey(0, (u32)this.nKey, pKey);
                    if (rc == SqlResult.SQLITE_OK)
                    {
                        this.pKey = pKey;
                    }
                    //else{
                    //  malloc_cs.sqlite3_free(ref pKey);
                    //}
                    //}else{
                    //  rc = SQLITE_NOMEM;
                    //}
                }
                Debug.Assert(false == this.apPage[0].intKey || null == this.pKey);
                if (rc == SqlResult.SQLITE_OK)
                {
                    int i;
                    for (i = 0; i <= this.iPage; i++)
                    {
                        BTreeMethods.releasePage(this.apPage[i]);
                        this.apPage[i] = null;
                    }
                    this.iPage = -1;
                    this.eState = CURSOR_REQUIRESEEK;
                }
                BTreeMethods.invalidateOverflowCache(this);
                return rc;
            }
            public void sqlite3BtreeClearCursor()
            {
                Debug.Assert(this.cursorHoldsMutex());
                malloc_cs.sqlite3_free(ref this.pKey);
                this.eState = CURSOR_INVALID;
            }
            public SqlResult btreeMoveto(///
                ///<summary>
                ///Cursor open on the btree to be searched 
                ///</summary>
            byte[] pKey,///
                ///<summary>
                ///Packed key if the btree is an index 
                ///</summary>
            i64 nKey,///
                ///<summary>
                ///Integer key for tables.  Size of pKey for indices 
                ///</summary>
            int bias,///
                ///<summary>
                ///Bias search to the high end 
                ///</summary>
            ref int pRes///
                ///<summary>
                ///Write search results here 
                ///</summary>
            )
            {
                SqlResult rc;
                ///
                ///<summary>
                ///Status code 
                ///</summary>
                UnpackedRecord pIdxKey;
                ///
                ///<summary>
                ///Unpacked index key 
                ///</summary>
                UnpackedRecord aSpace = new UnpackedRecord();
                //char aSpace[150]; /* Temp space for pIdxKey - to avoid a malloc */
                if (pKey != null)
                {
                    Debug.Assert(nKey == (i64)(int)nKey);
                    pIdxKey = vdbeaux.sqlite3VdbeRecordUnpack(this.pKeyInfo, (int)nKey, pKey, aSpace, 16);
                    //sizeof( aSpace ) );
                    //if ( pIdxKey == null )
                    //  return SQLITE_NOMEM;
                }
                else
                {
                    pIdxKey = null;
                }
                rc = this.sqlite3BtreeMovetoUnpacked(pIdxKey, nKey, bias != 0 ? 1 : 0, ref pRes);
                if (pKey != null)
                {
                    vdbeaux.sqlite3VdbeDeleteUnpackedRecord(pIdxKey);
                }
                return rc;
            }
            public SqlResult btreeRestoreCursorPosition()
            {
                SqlResult rc;
                Debug.Assert(this.cursorHoldsMutex());
                Debug.Assert(this.eState >= CURSOR_REQUIRESEEK);
                if (this.eState == CURSOR_FAULT)
                {
                    return (SqlResult)this.skipNext;
                }
                this.eState = CURSOR_INVALID;
                rc = this.btreeMoveto(this.pKey, this.nKey, 0, ref this.skipNext);
                if (rc == SqlResult.SQLITE_OK)
                {
                    //malloc_cs.sqlite3_free(ref pCur.pKey);
                    this.pKey = null;
                    Debug.Assert(this.eState == CURSOR_VALID || this.eState == CURSOR_INVALID);
                }
                return rc;
            }
            public SqlResult restoreCursorPosition()
            {
                if (this.eState >= CURSOR_REQUIRESEEK)
                    return this.btreeRestoreCursorPosition();
                else
                    return SqlResult.SQLITE_OK;
            }
            public SqlResult sqlite3BtreeCursorHasMoved(ref int pHasMoved)
            {
                SqlResult rc;
                rc = this.restoreCursorPosition();
                if (rc != 0)
                {
                    pHasMoved = 1;
                    return rc;
                }
                if (this.eState != CURSOR_VALID || this.skipNext != 0)
                {
                    pHasMoved = 1;
                }
                else
                {
                    pHasMoved = 0;
                }
                return SqlResult.SQLITE_OK;
            }
            ///
            ///<summary>
            ///Set the cached rowid value of every cursor in the same database file
            ///as pCur and having the same root page number as pCur.  The value is
            ///set to iRowid.
            ///
            ///Only positive rowid values are considered valid for this cache.
            ///The cache is initialized to zero, indicating an invalid cache.
            ///A btree will work fine with zero or negative rowids.  We just cannot
            ///cache zero or negative rowids, which means tables that use zero or
            ///negative rowids might run a little slower.  But in practice, zero
            ///or negative rowids are very uncommon so this should not be a problem.
            ///</summary>
            public void sqlite3BtreeSetCachedRowid(sqlite3_int64 iRowid)
            {
                BtCursor p;
                for (p = this.pBt.pCursor; p != null; p = p.pNext)
                {
                    if (p.pgnoRoot == this.pgnoRoot)
                        p.cachedRowid = iRowid;
                }
                Debug.Assert(this.cachedRowid == iRowid);
            }
            ///
            ///<summary>
            ///Return the cached rowid for the given cursor.  A negative or zero
            ///return value indicates that the rowid cache is invalid and should be
            ///ignored.  If the rowid cache has never before been set, then a
            ///zero is returned.
            ///
            ///</summary>
            public sqlite3_int64 sqlite3BtreeGetCachedRowid()
            {
                return this.cachedRowid;
            }
            public SqlResult sqlite3BtreeCount(ref i64 pnEntry)
            {
                i64 nEntry = 0;
                ///
                ///<summary>
                ///Value to return in pnEntry 
                ///</summary>
                SqlResult rc;
                ///
                ///<summary>
                ///Return code 
                ///</summary>
                rc = this.moveToRoot();
                ///
                ///<summary>
                ///Unless an error occurs, the following loop runs one iteration for each
                ///</summary>
                ///<param name="page in the B">Tree structure (not including overflow pages).</param>
                ///<param name=""></param>
                while (rc == SqlResult.SQLITE_OK)
                {
                    int iIdx;
                    ///
                    ///<summary>
                    ///Index of child node in parent 
                    ///</summary>
                    MemPage pPage;
                    ///
                    ///<summary>
                    ///</summary>
                    ///<param name="Current page of the b">tree </param>
                    ///
                    ///<summary>
                    ///</summary>
                    ///<param name="If this is a leaf page or the tree is not an int">key tree, then</param>
                    ///<param name="this page contains countable entries. Increment the entry counter">this page contains countable entries. Increment the entry counter</param>
                    ///<param name="accordingly.">accordingly.</param>
                    ///<param name=""></param>
                    pPage = this.apPage[this.iPage];
                    if (pPage.IsLeaf != false || false == pPage.intKey)
                    {
                        nEntry += pPage.nCell;
                    }
                    ///
                    ///<summary>
                    ///pPage is a leaf node. This loop navigates the cursor so that it
                    ///points to the first interior cell that it points to the parent of
                    ///the next page in the tree that has not yet been visited. The
                    ///pCur.aiIdx[pCur.iPage] value is set to the index of the parent cell
                    ///of the page, or to the number of cells in the page if the next page
                    ///</summary>
                    ///<param name="to visit is the right">child of its parent.</param>
                    ///<param name=""></param>
                    ///<param name="If all pages in the tree have been visited, return SqlResult.SQLITE_OK to the">If all pages in the tree have been visited, return SqlResult.SQLITE_OK to the</param>
                    ///<param name="caller.">caller.</param>
                    ///<param name=""></param>
                    if (pPage.IsLeaf != false)
                    {
                        do
                        {
                            if (this.iPage == 0)
                            {
                                ///
                                ///<summary>
                                ///</summary>
                                ///<param name="All pages of the b">tree have been visited. Return successfully. </param>
                                pnEntry = nEntry;
                                return SqlResult.SQLITE_OK;
                            }
                            this.moveToParent();
                        }
                        while (this.aiIdx[this.iPage] >= this.apPage[this.iPage].nCell);
                        this.aiIdx[this.iPage]++;
                        pPage = this.apPage[this.iPage];
                    }
                    ///
                    ///<summary>
                    ///Descend to the child node of the cell that the cursor currently
                    ///</summary>
                    ///<param name="points at. This is the right">child if (iIdx==pPage.nCell).</param>
                    ///<param name=""></param>
                    iIdx = this.aiIdx[this.iPage];
                    if (iIdx == pPage.nCell)
                    {
                        rc = this.moveToChild(Converter.sqlite3Get4byte(pPage.aData, pPage.hdrOffset + 8));
                    }
                    else
                    {
                        rc = this.moveToChild(Converter.sqlite3Get4byte(pPage.aData, pPage.findCell(iIdx)));
                    }
                }
                ///
                ///<summary>
                ///An error has occurred. Return an error code. 
                ///</summary>
                return rc;
            }
            public SqlResult sqlite3BtreeDelete()
            {
                Btree p = this.pBtree;
                BtShared pBt = p.pBt;
                SqlResult rc;
                ///
                ///<summary>
                ///Return code 
                ///</summary>
                MemPage pPage;
                ///
                ///<summary>
                ///Page to delete cell from 
                ///</summary>
                int pCell;
                ///
                ///<summary>
                ///Pointer to cell to delete 
                ///</summary>
                int iCellIdx;
                ///
                ///<summary>
                ///Index of cell to delete 
                ///</summary>
                int iCellDepth;
                ///
                ///<summary>
                ///Depth of node containing pCell 
                ///</summary>
                Debug.Assert(this.cursorHoldsMutex());
                Debug.Assert(pBt.inTransaction == TransType.TRANS_WRITE);
                Debug.Assert(!pBt.readOnly);
                Debug.Assert(this.wrFlag != 0);
                Debug.Assert(p.hasSharedCacheTableLock(this.pgnoRoot, this.pKeyInfo != null ? 1 : 0, 2));
                Debug.Assert(!p.hasReadConflicts(this.pgnoRoot));
                if (NEVER(this.aiIdx[this.iPage] >= this.apPage[this.iPage].nCell) || NEVER(this.eState != CURSOR_VALID))
                {
                    return SqlResult.SQLITE_ERROR;
                    ///
                    ///<summary>
                    ///Something has gone awry. 
                    ///</summary>
                }
                ///
                ///<summary>
                ///</summary>
                ///<param name="If this is a delete operation to remove a row from a table b">tree,</param>
                ///<param name="invalidate any incrblob cursors open on the row being deleted.  ">invalidate any incrblob cursors open on the row being deleted.  </param>
                if (this.pKeyInfo == null)
                {
                    p.invalidateIncrblobCursors(this.info.nKey, 0);
                }
                iCellDepth = this.iPage;
                iCellIdx = this.aiIdx[iCellDepth];
                pPage = this.apPage[iCellDepth];
                pCell = pPage.findCell(iCellIdx);
                ///
                ///<summary>
                ///If the page containing the entry to delete is not a leaf page, move
                ///the cursor to the largest entry in the tree that is smaller than
                ///the entry being deleted. This cell will replace the cell being deleted
                ///from the internal node. The 'previous' entry is used for this instead
                ///of the 'next' entry, as the previous entry is always a part of the
                ///</summary>
                ///<param name="sub">tree headed by the child page of the cell being deleted. This makes</param>
                ///<param name="balancing the tree following the delete operation easier.  ">balancing the tree following the delete operation easier.  </param>
                if (false == pPage.IsLeaf)
                {
                    int notUsed = 0;
                    rc = this.sqlite3BtreePrevious(ref notUsed);
                    if (rc != 0)
                        return rc;
                }
                ///
                ///<summary>
                ///Save the positions of any other cursors open on this table before
                ///making any modifications. Make the page containing the entry to be
                ///deleted writable. Then free any overflow pages associated with the
                ///entry and finally remove the cell itself from within the page.
                ///
                ///</summary>
                rc = pBt.saveAllCursors(this.pgnoRoot, this);
                if (rc != 0)
                    return rc;
                rc = PagerMethods.sqlite3PagerWrite(pPage.pDbPage);
                if (rc != 0)
                    return rc;
                rc = BTreeMethods.clearCell(pPage, pCell);
                pPage.dropCell(iCellIdx, pPage.cellSizePtr(pCell), ref rc);
                if (rc != 0)
                    return rc;
                ///
                ///<summary>
                ///If the cell deleted was not located on a leaf page, then the cursor
                ///</summary>
                ///<param name="is currently pointing to the largest entry in the sub">tree headed</param>
                ///<param name="by the child">page of the cell that was just deleted from an internal</param>
                ///<param name="node. The cell from the leaf node needs to be moved to the internal">node. The cell from the leaf node needs to be moved to the internal</param>
                ///<param name="node to replace the deleted cell.  ">node to replace the deleted cell.  </param>
                if (false == pPage.IsLeaf)
                {
                    MemPage pLeaf = this.apPage[this.iPage];
                    int nCell;
                    Pgno n = this.apPage[iCellDepth + 1].pgno;
                    //byte[] pTmp;
                    pCell = pLeaf.findCell(pLeaf.nCell - 1);
                    nCell = pLeaf.cellSizePtr(pCell);
                    Debug.Assert(MX_CELL_SIZE(pBt) >= nCell);
                    //allocateTempSpace(pBt);
                    //pTmp = pBt.pTmpSpace;
                    rc = PagerMethods.sqlite3PagerWrite(pLeaf.pDbPage);
                    byte[] pNext_4 = malloc_cs.sqlite3Malloc(nCell + 4);
                    Buffer.BlockCopy(pLeaf.aData, pCell - 4, pNext_4, 0, nCell + 4);
                    pPage.insertCell(iCellIdx, pNext_4, nCell + 4, null, n, ref rc);
                    //insertCell( pPage, iCellIdx, pCell - 4, nCell + 4, pTmp, n, ref rc );
                    pLeaf.dropCell(pLeaf.nCell - 1, nCell, ref rc);
                    if (rc != 0)
                        return rc;
                }
                ///
                ///<summary>
                ///Balance the tree. If the entry deleted was located on a leaf page,
                ///then the cursor still points to that page. In this case the first
                ///call to balance() repairs the tree, and the if(...) condition is
                ///never true.
                ///
                ///Otherwise, if the entry deleted was on an internal node page, then
                ///pCur is pointing to the leaf page from which a cell was removed to
                ///replace the cell deleted from the internal node. This is slightly
                ///tricky as the leaf node may be underfull, and the internal node may
                ///be either under or overfull. In this case run the balancing algorithm
                ///on the leaf node first. If the balance proceeds far enough up the
                ///tree that we can be sure that any problem in the internal node has
                ///been corrected, so be it. Otherwise, after balancing the leaf node,
                ///walk the cursor up the tree to the internal node and balance it as
                ///well.  
                ///</summary>
                rc = this.balance();
                if (rc == SqlResult.SQLITE_OK && this.iPage > iCellDepth)
                {
                    while (this.iPage > iCellDepth)
                    {
                        BTreeMethods.releasePage(this.apPage[this.iPage--]);
                    }
                    rc = this.balance();
                }
                if (rc == SqlResult.SQLITE_OK)
                {
                    this.moveToRoot();
                }
                return rc;
            }
            public SqlResult sqlite3BtreeInsert(
                ///<summary>
                ///Insert data into the table of this cursor 
                ///</summary>
            byte[] pKey, i64 nKey,///
                ///<summary>
                ///The key of the new record 
                ///</summary>
            byte[] pData, int nData,///
                ///<summary>
                ///The data of the new record 
                ///</summary>
            int nZero,///
                ///<summary>
                ///Number of extra 0 bytes to append to data 
                ///</summary>
            int appendBias,///
                ///<summary>
                ///True if this is likely an append 
                ///</summary>
            int seekResult///
                ///<summary>
                ///Result of prior MovetoUnpacked() call 
                ///</summary>
            )
            {
                SqlResult rc;
                int loc = seekResult;
                ///1: before desired location  +1: after 
                int szNew = 0;
                int idx;
                MemPage pPage;
                Btree p = this.pBtree;
                BtShared pBt = p.pBt;
                int oldCell;
                byte[] newCell = null;
                if (this.State == BtCursorState.CURSOR_FAULT)
                {
                    Debug.Assert(this.skipNext != (int)SqlResult.SQLITE_OK);
                    return (SqlResult)this.skipNext;
                }
                Debug.Assert(this.cursorHoldsMutex());
                Debug.Assert(this.wrFlag != 0 && pBt.inTransaction == TransType.TRANS_WRITE && !pBt.readOnly);
                Debug.Assert(p.hasSharedCacheTableLock(this.pgnoRoot, this.pKeyInfo != null ? 1 : 0, 2));
                
                ///Assert that the caller has been consistent. If this cursor was opened
                ///<param name="expecting an index b">tree, then the caller should be inserting blob</param>
                ///<param name="keys with no associated data. If the cursor was opened expecting an">keys with no associated data. If the cursor was opened expecting an</param>
                ///<param name="intkey table, the caller should be inserting integer keys with a">intkey table, the caller should be inserting integer keys with a</param>
                ///<param name="blob of associated data.  ">blob of associated data.  </param>
                Debug.Assert((pKey == null) == (this.pKeyInfo == null));
                
                ///<param name="If this is an insert into a table b">tree, invalidate any incrblob</param>
                ///<param name="cursors open on the row being replaced (assuming this is a replace">cursors open on the row being replaced (assuming this is a replace</param>
                ///<param name="operation ">op).  </param>
                if (this.pKeyInfo == null)
                {
                    p.invalidateIncrblobCursors(nKey, 0);
                }
                
                ///Save the positions of any other cursors open on this table.
                ///In some cases, the call to btreeMoveto() below is a no">op. For</param>
                ///example, when inserting data into a table with auto">generated integer</param>
                ///keys, the VDBE layer invokes sqlite3BtreeLast() to figure out the">keys, the VDBE layer invokes sqlite3BtreeLast() to figure out the</param>
                ///integer key to use. It then calls this function to actually insert the
                ///data into the intkey B">Tree. In this case btreeMoveto() recognizes</param>
                ///that the cursor is already where it needs to be and returns without">that the cursor is already where it needs to be and returns without</param>
                ///doing any work. To avoid thwarting these optimizations, it is important">doing any work. To avoid thwarting these optimizations, it is important</param>
                ///not to clear the cursor here.">not to clear the cursor here.</param>
                rc = pBt.saveAllCursors(this.pgnoRoot, this);
                if (rc != 0)
                    return rc;
                if (0 == loc)
                {
                    rc = this.btreeMoveto(pKey, nKey, appendBias, ref loc);
                    if (rc != 0)
                        return rc;
                }
                Debug.Assert(this.State == BtCursorState.CURSOR_VALID || (this.State == BtCursorState.CURSOR_INVALID && loc != 0));
                pPage = this.apPage[this.iPage];
                Debug.Assert(pPage.intKey != false || nKey >= 0);
                Debug.Assert(pPage.IsLeaf != false || false == pPage.intKey);
                TRACE("INSERT: table=%d nkey=%lld ndata=%d page=%d %s\n", this.pgnoRoot, nKey, nData, pPage.pgno, loc == 0 ? "overwrite" : "new entry");
                Debug.Assert(pPage.isInit != false);
                BTreeMethods.allocateTempSpace(pBt);
                newCell = pBt.pTmpSpace;
                //if (newCell == null) return SQLITE_NOMEM;
                rc = pPage.fillInCell(newCell, pKey, nKey, pData, nData, nZero, ref szNew);
                if (rc != 0)
                    goto end_insert;
                Debug.Assert(szNew == pPage.cellSizePtr(newCell));
                Debug.Assert(szNew <= MX_CELL_SIZE(pBt));
                idx = this.aiIdx[this.iPage];
                if (loc == 0)
                {
                    u16 szOld;
                    Debug.Assert(idx < pPage.nCell);
                    rc = PagerMethods.sqlite3PagerWrite(pPage.pDbPage);
                    if (rc != 0)
                    {
                        goto end_insert;
                    }
                    oldCell = pPage.findCell(idx);
                    if (false == pPage.IsLeaf)
                    {
                        //memcpy(newCell, oldCell, 4);
                        newCell[0] = pPage.aData[oldCell + 0];
                        newCell[1] = pPage.aData[oldCell + 1];
                        newCell[2] = pPage.aData[oldCell + 2];
                        newCell[3] = pPage.aData[oldCell + 3];
                    }
                    szOld = pPage.cellSizePtr(oldCell);
                    rc = BTreeMethods.clearCell(pPage, oldCell);
                    pPage.dropCell(idx, szOld, ref rc);
                    if (rc != 0)
                        goto end_insert;
                }
                else
                    if (loc < 0 && pPage.nCell > 0)
                    {
                        Debug.Assert(pPage.IsLeaf != false);
                        idx = ++this.aiIdx[this.iPage];
                    }
                    else
                    {
                        Debug.Assert(pPage.IsLeaf != false);
                    }
                pPage.insertCell(idx, newCell, szNew, null, 0, ref rc);
                Debug.Assert(rc != SqlResult.SQLITE_OK || pPage.nCell > 0 || pPage.nOverflow > 0);
                ///
                ///<summary>
                ///If no error has occured and pPage has an overflow cell, call balance()
                ///to redistribute the cells within the tree. Since balance() may move
                ///the cursor, zero the BtCursor.info.nSize and BtCursor.validNKey
                ///variables.
                ///
                ///Previous versions of SQLite called moveToRoot() to move the cursor
                ///back to the root page as balance() used to invalidate the contents
                ///of BtCursor.apPage[] and BtCursor.aiIdx[]. Instead of doing that,
                ///set the cursor state to "invalid". This makes common insert operations
                ///slightly faster.
                ///
                ///There is a subtle but important optimization here too. When inserting
                ///</summary>
                ///<param name="multiple records into an intkey b">tree using a single cursor (as can</param>
                ///<param name="happen while processing an "INSERT INTO ... SELECT" statement), it">happen while processing an "INSERT INTO ... SELECT" statement), it</param>
                ///<param name="is advantageous to leave the cursor pointing to the last entry in">is advantageous to leave the cursor pointing to the last entry in</param>
                ///<param name="the b">tree if possible. If the cursor is left pointing to the last</param>
                ///<param name="entry in the table, and the next row inserted has an integer key">entry in the table, and the next row inserted has an integer key</param>
                ///<param name="larger than the largest existing key, it is possible to insert the">larger than the largest existing key, it is possible to insert the</param>
                ///<param name="row without seeking the cursor. This can be a big performance boost.">row without seeking the cursor. This can be a big performance boost.</param>
                ///<param name=""></param>
                this.info.nSize = 0;
                this.validNKey = false;
                if (rc == SqlResult.SQLITE_OK && pPage.nOverflow != 0)
                {
                    rc = this.balance();
                    ///
                    ///<summary>
                    ///Must make sure nOverflow is reset to zero even if the balance()
                    ///fails. Internal data structure corruption will result otherwise.
                    ///Also, set the cursor state to invalid. This stops saveCursorPosition()
                    ///from trying to save the current position of the cursor.  
                    ///</summary>
                    this.apPage[this.iPage].nOverflow = 0;
                    this.State = BtCursorState.CURSOR_INVALID;
                }
                Debug.Assert(this.apPage[this.iPage].nOverflow == 0);
            end_insert:
                return rc;
            }
            public SqlResult balance()
            {
                SqlResult rc = SqlResult.SQLITE_OK;
                int nMin = (int)this.pBt.usableSize * 2 / 3;
                //u8[] pFree = null;
#if !NDEBUG || SQLITE_COVERAGE_TEST || DEBUG
																																																																												  int balance_quick_called = 0;//TESTONLY( int balance_quick_called = 0 );
  int balance_deeper_called = 0;//TESTONLY( int balance_deeper_called = 0 );
#else
                int balance_quick_called = 0;
                int balance_deeper_called = 0;
#endif
                do
                {
                    int iPage = this.iPage;
                    MemPage pPage = this.apPage[iPage];
                    if (iPage == 0)
                    {
                        if (pPage.nOverflow != 0)
                        {
                            ///
                            ///<summary>
                            ///</summary>
                            ///<param name="The root page of the b">tree is overfull. In this case call the</param>
                            ///<param name="balance_deeper() function to create a new child for the root">page</param>
                            ///<param name="and copy the current contents of the root">page to it. The</param>
                            ///<param name="next iteration of the do">loop will balance the child page.</param>
                            ///<param name=""></param>
                            Debug.Assert((balance_deeper_called++) == 0);
                            rc = pPage.balance_deeper(ref this.apPage[1]);
                            if (rc == SqlResult.SQLITE_OK)
                            {
                                this.iPage = 1;
                                this.aiIdx[0] = 0;
                                this.aiIdx[1] = 0;
                                Debug.Assert(this.apPage[1].nOverflow != 0);
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                    else
                        if (pPage.nOverflow == 0 && pPage.nFree <= nMin)
                        {
                            break;
                        }
                        else
                        {
                            MemPage pParent = this.apPage[iPage - 1];
                            int iIdx = this.aiIdx[iPage - 1];
                            rc = PagerMethods.sqlite3PagerWrite(pParent.pDbPage);
                            if (rc == SqlResult.SQLITE_OK)
                            {
#if !SQLITE_OMIT_QUICKBALANCE
                                if (pPage.hasData != false && pPage.nOverflow == 1 && pPage.aOvfl[0].idx == pPage.nCell && pParent.pgno != 1 && pParent.nCell == iIdx)
                                {
                                    ///
                                    ///<summary>
                                    ///Call balance_quick() to create a new sibling of pPage on which
                                    ///to store the overflow cell. balance_quick() inserts a new cell
                                    ///into pParent, which may cause pParent overflow. If this
                                    ///</summary>
                                    ///<param name="happens, the next interation of the do">loop will balance pParent</param>
                                    ///<param name="use either balance_nonroot() or balance_deeper(). Until this">use either balance_nonroot() or balance_deeper(). Until this</param>
                                    ///<param name="happens, the overflow cell is stored in the aBalanceQuickSpace[]">happens, the overflow cell is stored in the aBalanceQuickSpace[]</param>
                                    ///<param name="buffer.">buffer.</param>
                                    ///<param name=""></param>
                                    ///<param name="The purpose of the following Debug.Assert() is to check that only a">The purpose of the following Debug.Assert() is to check that only a</param>
                                    ///<param name="single call to balance_quick() is made for each call to this">single call to balance_quick() is made for each call to this</param>
                                    ///<param name="function. If this were not verified, a subtle bug involving reuse">function. If this were not verified, a subtle bug involving reuse</param>
                                    ///<param name="of the aBalanceQuickSpace[] might sneak in.">of the aBalanceQuickSpace[] might sneak in.</param>
                                    ///<param name=""></param>
                                    Debug.Assert((balance_quick_called++) == 0);
                                    rc = pParent.balance_quick(pPage, BTreeMethods.aBalanceQuickSpace);
                                }
                                else
#endif
                                {
                                    ///
                                    ///<summary>
                                    ///In this case, call balance_nonroot() to redistribute cells
                                    ///between pPage and up to 2 of its sibling pages. This involves
                                    ///modifying the contents of pParent, which may cause pParent to
                                    ///</summary>
                                    ///<param name="become overfull or underfull. The next iteration of the do">loop</param>
                                    ///<param name="will balance the parent page to correct this.">will balance the parent page to correct this.</param>
                                    ///<param name=""></param>
                                    ///<param name="If the parent page becomes overfull, the overflow cell or cells">If the parent page becomes overfull, the overflow cell or cells</param>
                                    ///<param name="are stored in the pSpace buffer allocated immediately below.">are stored in the pSpace buffer allocated immediately below.</param>
                                    ///<param name="A subsequent iteration of the do">loop will deal with this by</param>
                                    ///<param name="calling balance_nonroot() (balance_deeper() may be called first,">calling balance_nonroot() (balance_deeper() may be called first,</param>
                                    ///<param name="but it doesn't deal with overflow cells "> just moves them to a</param>
                                    ///<param name="different page). Once this subsequent call to balance_nonroot()">different page). Once this subsequent call to balance_nonroot()</param>
                                    ///<param name="has completed, it is safe to release the pSpace buffer used by">has completed, it is safe to release the pSpace buffer used by</param>
                                    ///<param name="the previous call, as the overflow cell data will have been">the previous call, as the overflow cell data will have been</param>
                                    ///<param name="copied either into the body of a database page or into the new">copied either into the body of a database page or into the new</param>
                                    ///<param name="pSpace buffer passed to the latter call to balance_nonroot().">pSpace buffer passed to the latter call to balance_nonroot().</param>
                                    ///<param name=""></param>
                                    u8[] pSpace = new u8[this.pBt.pageSize];
                                    // u8 pSpace = sqlite3PageMalloc( pCur.pBt.pageSize );
                                    rc = pParent.balance_nonroot(iIdx, null, iPage == 1 ? 1 : 0);
                                    //if (pFree != null)
                                    //{
                                    //  /* If pFree is not NULL, it points to the pSpace buffer used
                                    //  ** by a previous call to balance_nonroot(). Its contents are
                                    //  ** now stored either on real database pages or within the
                                    //  ** new pSpace buffer, so it may be safely freed here. */
                                    //  sqlite3PageFree(ref pFree);
                                    //}
                                    ///
                                    ///<summary>
                                    ///The pSpace buffer will be freed after the next call to
                                    ///balance_nonroot(), or just before this function returns, whichever
                                    ///comes first. 
                                    ///</summary>
                                    //pFree = pSpace;
                                }
                            }
                            pPage.nOverflow = 0;
                            ///
                            ///<summary>
                            ///</summary>
                            ///<param name="The next iteration of the do">loop balances the parent page. </param>
                            BTreeMethods.releasePage(pPage);
                            this.iPage--;
                        }
                }
                while (rc == SqlResult.SQLITE_OK);
                //if (pFree != null)
                //{
                //  sqlite3PageFree(ref pFree);
                //}
                return rc;
            }
            public SqlResult sqlite3BtreePrevious(ref int pRes)
            {
                SqlResult rc;
                MemPage pPage;
                Debug.Assert(this.cursorHoldsMutex());
                rc = this.restoreCursorPosition();
                if (rc != SqlResult.SQLITE_OK)
                {
                    return rc;
                }
                this.atLast = 0;
                if (CURSOR_INVALID == this.eState)
                {
                    pRes = 1;
                    return SqlResult.SQLITE_OK;
                }
                if (this.skipNext < 0)
                {
                    this.skipNext = 0;
                    pRes = 0;
                    return SqlResult.SQLITE_OK;
                }
                this.skipNext = 0;
                pPage = this.apPage[this.iPage];
                Debug.Assert(pPage.isInit != false);
                if (false == pPage.IsLeaf)
                {
                    int idx = this.aiIdx[this.iPage];
                    rc = this.moveToChild(Converter.sqlite3Get4byte(pPage.aData, pPage.findCell(idx)));
                    if (rc != 0)
                    {
                        return rc;
                    }
                    rc = this.moveToRightmost();
                }
                else
                {
                    while (this.aiIdx[this.iPage] == 0)
                    {
                        if (this.iPage == 0)
                        {
                            this.State = BtCursorState.CURSOR_INVALID;
                            pRes = 1;
                            return SqlResult.SQLITE_OK;
                        }
                        this.moveToParent();
                    }
                    this.info.nSize = 0;
                    this.validNKey = false;
                    this.aiIdx[this.iPage]--;
                    pPage = this.apPage[this.iPage];
                    if (pPage.intKey != false && false == pPage.IsLeaf)
                    {
                        rc = this.sqlite3BtreePrevious(ref pRes);
                    }
                    else
                    {
                        rc = SqlResult.SQLITE_OK;
                    }
                }
                pRes = 0;
                return rc;
            }
            public SqlResult sqlite3BtreeNext(ref int pRes)
            {
                SqlResult rc;
                int idx;
                MemPage pPage;
                Debug.Assert(this.cursorHoldsMutex());
                rc = this.restoreCursorPosition();
                if (rc != SqlResult.SQLITE_OK)
                {
                    return rc;
                }
                // Not needed in C# // Debug.Assert( pRes != 0 );
                if (CURSOR_INVALID == this.eState)
                {
                    pRes = 1;
                    return SqlResult.SQLITE_OK;
                }
                if (this.skipNext > 0)
                {
                    this.skipNext = 0;
                    pRes = 0;
                    return SqlResult.SQLITE_OK;
                }
                this.skipNext = 0;
                pPage = this.apPage[this.iPage];
                idx = ++this.aiIdx[this.iPage];
                Debug.Assert(pPage.isInit != false);
                Debug.Assert(idx <= pPage.nCell);
                this.info.nSize = 0;
                this.validNKey = false;
                if (idx >= pPage.nCell)
                {
                    if (false == pPage.IsLeaf)
                    {
                        rc = this.moveToChild(Converter.sqlite3Get4byte(pPage.aData, pPage.hdrOffset + 8));
                        if (rc != 0)
                            return rc;
                        rc = this.moveToLeftmost();
                        pRes = 0;
                        return rc;
                    }
                    do
                    {
                        if (this.iPage == 0)
                        {
                            pRes = 1;
                            this.State = BtCursorState.CURSOR_INVALID;
                            return SqlResult.SQLITE_OK;
                        }
                        this.moveToParent();
                        pPage = this.apPage[this.iPage];
                    }
                    while (this.aiIdx[this.iPage] >= pPage.nCell);
                    pRes = 0;
                    if (pPage.intKey != false)
                    {
                        rc = this.sqlite3BtreeNext(ref pRes);
                    }
                    else
                    {
                        rc = SqlResult.SQLITE_OK;
                    }
                    return rc;
                }
                pRes = 0;
                if (pPage.IsLeaf != false)
                {
                    return SqlResult.SQLITE_OK;
                }
                rc = this.moveToLeftmost();
                return rc;
            }
            public bool sqlite3BtreeEof()
            {
                ///
                ///<summary>
                ///TODO: What if the cursor is in CURSOR_REQUIRESEEK but all table entries
                ///have been deleted? This API will need to change to return an error code
                ///as well as the boolean result value.
                ///
                ///</summary>
                return (BtCursorState.CURSOR_VALID != this.State);
            }
            public SqlResult sqlite3BtreeMovetoUnpacked(///
                ///<summary>
                ///The cursor to be moved 
                ///</summary>
            UnpackedRecord pIdxKey,///
                ///<summary>
                ///Unpacked index key 
                ///</summary>
            i64 intKey,///
                ///<summary>
                ///The table key 
                ///</summary>
            int biasRight,///
                ///<summary>
                ///If true, bias the search to the high end 
                ///</summary>
            ref int pRes///
                ///<summary>
                ///Write search results here 
                ///</summary>
            )
            {
                SqlResult rc;
                Debug.Assert(this.cursorHoldsMutex());
                Debug.Assert(this.pBtree.db.mutex.sqlite3_mutex_held());
                // Not needed in C# // Debug.Assert( pRes != 0 );
                Debug.Assert((pIdxKey == null) == (this.pKeyInfo == null));
                ///
                ///<summary>
                ///If the cursor is already positioned at the point we are trying
                ///to move to, then just return without doing any work 
                ///</summary>
                if (this.State == BtCursorState.CURSOR_VALID && this.validNKey && this.apPage[0].intKey != false)
                {
                    if (this.info.nKey == intKey)
                    {
                        pRes = 0;
                        return SqlResult.SQLITE_OK;
                    }
                    if (this.atLast != 0 && this.info.nKey < intKey)
                    {
                        pRes = -1;
                        return SqlResult.SQLITE_OK;
                    }
                }
                rc = this.moveToRoot();
                if (rc != 0)
                {
                    return rc;
                }
                Debug.Assert(this.apPage[this.iPage] != null);
                Debug.Assert(this.apPage[this.iPage].isInit != false);
                Debug.Assert(this.apPage[this.iPage].nCell > 0 || this.State == BtCursorState.CURSOR_INVALID);
                if (this.State == BtCursorState.CURSOR_INVALID)
                {
                    pRes = -1;
                    Debug.Assert(this.apPage[this.iPage].nCell == 0);
                    return SqlResult.SQLITE_OK;
                }
                Debug.Assert(this.apPage[0].intKey != false || pIdxKey != null);
                for (; ; )
                {
                    int lwr, upr, idx;
                    Pgno chldPg;
                    MemPage pPage = this.apPage[this.iPage];
                    int c;
                    ///
                    ///<summary>
                    ///</summary>
                    ///<param name="pPage.nCell must be greater than zero. If this is the root">page</param>
                    ///<param name="the cursor would have been INVALID above and this for(;;) loop">the cursor would have been INVALID above and this for(;;) loop</param>
                    ///<param name="not run. If this is not the root">page, then the moveToChild() routine</param>
                    ///<param name="would have already detected db corruption. Similarly, pPage must">would have already detected db corruption. Similarly, pPage must</param>
                    ///<param name="be the right kind (index or table) of b">tree page. Otherwise</param>
                    ///<param name="a moveToChild() or moveToRoot() call would have detected corruption.  ">a moveToChild() or moveToRoot() call would have detected corruption.  </param>
                    Debug.Assert(pPage.nCell > 0);
                    Debug.Assert(pPage.intKey == (pIdxKey == null));
                    lwr = 0;
                    upr = pPage.nCell - 1;
                    if (biasRight != 0)
                    {
                        this.aiIdx[this.iPage] = (u16)(idx = upr);
                    }
                    else
                    {
                        this.aiIdx[this.iPage] = (u16)(idx = (upr + lwr) / 2);
                    }
                    for (; ; )
                    {
                        int pCell;
                        ///
                        ///<summary>
                        ///Pointer to current cell in pPage 
                        ///</summary>
                        Debug.Assert(idx == this.aiIdx[this.iPage]);
                        this.info.nSize = 0;
                        pCell = pPage.findCell(idx) + pPage.childPtrSize;
                        if (pPage.intKey != false)
                        {
                            i64 nCellKey = 0;
                            if (pPage.hasData != false)
                            {
                                u32 Dummy0 = 0;
                                pCell += utilc.getVarint32(pPage.aData, pCell, out Dummy0);
                            }
                            utilc.getVarint(pPage.aData, pCell, out nCellKey);
                            if (nCellKey == intKey)
                            {
                                c = 0;
                            }
                            else
                                if (nCellKey < intKey)
                                {
                                    c = -1;
                                }
                                else
                                {
                                    Debug.Assert(nCellKey > intKey);
                                    c = +1;
                                }
                            this.validNKey = true;
                            this.info.nKey = nCellKey;
                        }
                        else
                        {
                            ///
                            ///<summary>
                            ///</summary>
                            ///<param name="The maximum supported page">size is 65536 bytes. This means that</param>
                            ///<param name="the maximum number of record bytes stored on an index B">Tree</param>
                            ///<param name="page is less than 16384 bytes and may be stored as a 2">byte</param>
                            ///<param name="varint. This information is used to attempt to avoid parsing">varint. This information is used to attempt to avoid parsing</param>
                            ///<param name="the entire cell by checking for the cases where the record is">the entire cell by checking for the cases where the record is</param>
                            ///<param name="stored entirely within the b">tree page by inspecting the first</param>
                            ///<param name="2 bytes of the cell.">2 bytes of the cell.</param>
                            ///<param name=""></param>
                            int nCell = pPage.aData[pCell + 0];
                            //pCell[0];
                            if (0 == (nCell & 0x80) && nCell <= pPage.maxLocal)
                            {
                                ///
                                ///<summary>
                                ///</summary>
                                ///<param name="This branch runs if the record">size field of the cell is a</param>
                                ///<param name="single byte varint and the record fits entirely on the main">single byte varint and the record fits entirely on the main</param>
                                ///<param name="b">tree page.  </param>
                                c = vdbeaux.sqlite3VdbeRecordCompare(nCell, pPage.aData, pCell + 1, pIdxKey);
                                //c = sqlite3VdbeRecordCompare( nCell, (void*)&pCell[1], pIdxKey );
                            }
                            else
                                if (0 == (pPage.aData[pCell + 1] & 0x80)//!(pCell[1] & 0x80)
                                && (nCell = ((nCell & 0x7f) << 7) + pPage.aData[pCell + 1]) <= pPage.maxLocal//pCell[1])<=pPage.maxLocal
                                )
                                {
                                    ///
                                    ///<summary>
                                    ///</summary>
                                    ///<param name="The record">size field is a 2 byte varint and the record</param>
                                    ///<param name="fits entirely on the main b">tree page.  </param>
                                    c = vdbeaux.sqlite3VdbeRecordCompare(nCell, pPage.aData, pCell + 2, pIdxKey);
                                    //c = sqlite3VdbeRecordCompare( nCell, (void*)&pCell[2], pIdxKey );
                                }
                                else
                                {
                                    ///
                                    ///<summary>
                                    ///The record flows over onto one or more overflow pages. In
                                    ///this case the whole cell needs to be parsed, a buffer allocated
                                    ///and accessPayload() used to retrieve the record into the
                                    ///buffer before VdbeRecordCompare() can be called. 
                                    ///</summary>
                                    u8[] pCellKey;
                                    u8[] pCellBody = new u8[pPage.aData.Length - pCell + pPage.childPtrSize];
                                    Buffer.BlockCopy(pPage.aData, pCell - pPage.childPtrSize, pCellBody, 0, pCellBody.Length);
                                    //          u8 * const pCellBody = pCell - pPage->childPtrSize;
                                    pPage.btreeParseCellPtr(pCellBody, ref this.info);
                                    nCell = (int)this.info.nKey;
                                    pCellKey = malloc_cs.sqlite3Malloc(nCell);
                                    //if ( pCellKey == null )
                                    //{
                                    //  rc = SQLITE_NOMEM;
                                    //  goto moveto_finish;
                                    //}
                                    rc = this.accessPayload(0, (u32)nCell, pCellKey, 0);
                                    if (rc != 0)
                                    {
                                        pCellKey = null;
                                        // malloc_cs.sqlite3_free(ref pCellKey );
                                        goto moveto_finish;
                                    }
                                    c = vdbeaux.sqlite3VdbeRecordCompare(nCell, pCellKey, pIdxKey);
                                    pCellKey = null;
                                    // malloc_cs.sqlite3_free(ref pCellKey );
                                }
                        }
                        if (c == 0)
                        {
                            if (pPage.intKey != false && false == pPage.IsLeaf)
                            {
                                lwr = idx;
                                upr = lwr - 1;
                                break;
                            }
                            else
                            {
                                pRes = 0;
                                rc = SqlResult.SQLITE_OK;
                                goto moveto_finish;
                            }
                        }
                        if (c < 0)
                        {
                            lwr = idx + 1;
                        }
                        else
                        {
                            upr = idx - 1;
                        }
                        if (lwr > upr)
                        {
                            break;
                        }
                        this.aiIdx[this.iPage] = (u16)(idx = (lwr + upr) / 2);
                    }
                    Debug.Assert(lwr == upr + 1);
                    Debug.Assert(pPage.isInit != false);
                    if (pPage.IsLeaf != false)
                    {
                        chldPg = 0;
                    }
                    else
                        if (lwr >= pPage.nCell)
                        {
                            chldPg = Converter.sqlite3Get4byte(pPage.aData, pPage.hdrOffset + 8);
                        }
                        else
                        {
                            chldPg = Converter.sqlite3Get4byte(pPage.aData, pPage.findCell(lwr));
                        }
                    if (chldPg == 0)
                    {
                        Debug.Assert(this.aiIdx[this.iPage] < this.apPage[this.iPage].nCell);
                        pRes = c;
                        rc = SqlResult.SQLITE_OK;
                        goto moveto_finish;
                    }
                    this.aiIdx[this.iPage] = (u16)lwr;
                    this.info.nSize = 0;
                    this.validNKey = false;
                    rc = this.moveToChild(chldPg);
                    if (rc != 0)
                        goto moveto_finish;
                }
            moveto_finish:
                return rc;
            }
            public SqlResult sqlite3BtreeLast(ref int pRes)
            {
                SqlResult rc;
                Debug.Assert(this.cursorHoldsMutex());
                Debug.Assert(this.pBtree.db.mutex.sqlite3_mutex_held());
                ///
                ///<summary>
                ///</summary>
                ///<param name="If the cursor already points to the last entry, this is a no">op. </param>
                if (BtCursorState.CURSOR_VALID == this.State && this.atLast != 0)
                {
#if SQLITE_DEBUG
																																																																																																					    /* This block serves to Debug.Assert() that the cursor really does point
** to the last entry in the b-tree. */
    int ii;
    for ( ii = 0; ii < pCur.iPage; ii++ )
    {
      Debug.Assert( pCur.aiIdx[ii] == pCur.apPage[ii].nCell );
    }
    Debug.Assert( pCur.aiIdx[pCur.iPage] == pCur.apPage[pCur.iPage].nCell - 1 );
    Debug.Assert( pCur.apPage[pCur.iPage].leaf != 0 );
#endif
                    return SqlResult.SQLITE_OK;
                }
                rc = this.moveToRoot();
                if (rc == SqlResult.SQLITE_OK)
                {
                    if (BtCursorState.CURSOR_INVALID == this.State)
                    {
                        Debug.Assert(this.apPage[this.iPage].nCell == 0);
                        pRes = 1;
                    }
                    else
                    {
                        Debug.Assert(this.State == BtCursorState.CURSOR_VALID);
                        pRes = 0;
                        rc = this.moveToRightmost();
                        this.atLast = (u8)(rc == SqlResult.SQLITE_OK ? 1 : 0);
                    }
                }
                return rc;
            }
            public SqlResult sqlite3BtreeFirst(ref int pRes)
            {
                SqlResult rc;
                Debug.Assert(this.cursorHoldsMutex());
                Debug.Assert(this.pBtree.db.mutex.sqlite3_mutex_held());
                
                rc = this.moveToRoot();
                if (rc == SqlResult.SQLITE_OK)
                {
                    if (this.State == BtCursorState.CURSOR_INVALID)
                    {
                        Debug.Assert(this.apPage[this.iPage].nCell == 0);
                        pRes = 1;
                    }
                    else
                    {
                        Debug.Assert(this.apPage[this.iPage].nCell > 0);
                        pRes = 0;
                        rc = this.moveToLeftmost();
                    }
                }
                return rc;
            }
            public SqlResult moveToRightmost()
            {
                Pgno pgno;
                SqlResult rc = SqlResult.SQLITE_OK;
                MemPage pPage = null;
                Debug.Assert(this.cursorHoldsMutex());
                Debug.Assert(this.State == BtCursorState.CURSOR_VALID);
                while (rc == SqlResult.SQLITE_OK && false == (pPage = this.apPage[this.iPage]).IsLeaf)
                {
                    pgno = Converter.sqlite3Get4byte(pPage.aData, pPage.hdrOffset + 8);
                    this.aiIdx[this.iPage] = pPage.nCell;
                    rc = this.moveToChild(pgno);
                }
                if (rc == SqlResult.SQLITE_OK)
                {
                    this.aiIdx[this.iPage] = (u16)(pPage.nCell - 1);
                    this.info.nSize = 0;
                    this.validNKey = false;
                }
                return rc;
            }
            public SqlResult moveToLeftmost()
            {
                Pgno pgno;
                var rc = SqlResult.SQLITE_OK;
                MemPage pPage;
                Debug.Assert(this.cursorHoldsMutex());
                Debug.Assert(this.State == BtCursorState.CURSOR_VALID);
                while (rc == SqlResult.SQLITE_OK && false == (pPage = this.apPage[this.iPage]).IsLeaf)
                {
                    Debug.Assert(this.aiIdx[this.iPage] < pPage.nCell);
                    pgno = Converter.sqlite3Get4byte(pPage.aData, pPage.findCell(this.aiIdx[this.iPage]));
                    rc = this.moveToChild(pgno);
                }
                return rc;
            }
            public SqlResult moveToRoot()
            {
                MemPage pRoot;
                SqlResult rc = SqlResult.SQLITE_OK;
                Btree p = this.pBtree;
                BtShared pBt = p.pBt;
                Debug.Assert(this.cursorHoldsMutex());
                Debug.Assert(CURSOR_INVALID < CURSOR_REQUIRESEEK);
                Debug.Assert(CURSOR_VALID < CURSOR_REQUIRESEEK);
                Debug.Assert(CURSOR_FAULT > CURSOR_REQUIRESEEK);
                if (this.eState >= CURSOR_REQUIRESEEK)
                {
                    if (this.State == BtCursorState.CURSOR_FAULT)
                    {
                        Debug.Assert(this.skipNext != (int)SqlResult.SQLITE_OK);
                        return (SqlResult)this.skipNext;
                    }
                    this.sqlite3BtreeClearCursor();
                }
                if (this.iPage >= 0)
                {
                    int i;
                    for (i = 1; i <= this.iPage; i++)
                    {
                        BTreeMethods.releasePage(this.apPage[i]);
                    }
                    this.iPage = 0;
                }
                else
                {
                    rc = BTreeMethods.getAndInitPage(pBt, this.pgnoRoot, ref this.apPage[0]);
                    if (rc != SqlResult.SQLITE_OK)
                    {
                        this.State = BtCursorState.CURSOR_INVALID;
                        return rc;
                    }
                    this.iPage = 0;
                    ///
                    ///<summary>
                    ///If pCur.pKeyInfo is not NULL, then the caller that opened this cursor
                    ///</summary>
                    ///<param name="expected to open it on an index b">tree. Otherwise, if pKeyInfo is</param>
                    ///<param name="NULL, the caller expects a table b">tree. If this is not the case,</param>
                    ///<param name="return an SQLITE_CORRUPT error.  ">return an SQLITE_CORRUPT error.  </param>
                    Debug.Assert(this.apPage[0].intKey == false || this.apPage[0].intKey == false);
                    if ((this.pKeyInfo == null) != (this.apPage[0].intKey != false))
                    {
                        return sqliteinth.SQLITE_CORRUPT_BKPT();
                    }
                }
                ///
                ///<summary>
                ///Assert that the root page is of the correct type. This must be the
                ///</summary>
                ///<param name="case as the call to this function that loaded the root">page (either</param>
                ///<param name="this call or a previous invocation) would have detected corruption">this call or a previous invocation) would have detected corruption</param>
                ///<param name="if the assumption were not true, and it is not possible for the flags">if the assumption were not true, and it is not possible for the flags</param>
                ///<param name="byte to have been modified while this cursor is holding a reference">byte to have been modified while this cursor is holding a reference</param>
                ///<param name="to the page.  ">to the page.  </param>
                pRoot = this.apPage[0];
                Debug.Assert(pRoot.pgno == this.pgnoRoot);
                Debug.Assert(pRoot.isInit != false && (this.pKeyInfo == null) == (pRoot.intKey != false));
                this.aiIdx[0] = 0;
                this.info.nSize = 0;
                this.atLast = 0;
                this.validNKey = false;
                if (pRoot.nCell == 0 && false == pRoot.IsLeaf)
                {
                    Pgno subpage;
                    if (pRoot.pgno != 1)
                        return sqliteinth.SQLITE_CORRUPT_BKPT();
                    subpage = Converter.sqlite3Get4byte(pRoot.aData, pRoot.hdrOffset + 8);
                    this.State = BtCursorState.CURSOR_VALID;
                    rc = this.moveToChild(subpage);
                }
                else
                {
                    this.State = ((pRoot.nCell > 0) ? BtCursorState.CURSOR_VALID : BtCursorState.CURSOR_INVALID);
                }
                return rc;
            }
            public void moveToParent()
            {
                Debug.Assert(this.cursorHoldsMutex());
                Debug.Assert(this.State == BtCursorState.CURSOR_VALID);
                Debug.Assert(this.iPage > 0);
                Debug.Assert(this.apPage[this.iPage] != null);
                this.apPage[this.iPage - 1].assertParentIndex(this.aiIdx[this.iPage - 1], this.apPage[this.iPage].pgno);
                BTreeMethods.releasePage(this.apPage[this.iPage]);
                this.iPage--;
                this.info.nSize = 0;
                this.validNKey = false;
            }
            public SqlResult moveToChild(u32 newPgno)
            {
                SqlResult rc;
                int i = this.iPage;
                MemPage pNewPage = new MemPage();
                BtShared pBt = this.pBt;
                Debug.Assert(this.cursorHoldsMutex());
                Debug.Assert(this.State == BtCursorState.CURSOR_VALID);
                Debug.Assert(this.iPage < BTCURSOR_MAX_DEPTH);
                if (this.iPage >= (BTCURSOR_MAX_DEPTH - 1))
                {
                    return sqliteinth.SQLITE_CORRUPT_BKPT();
                }
                rc = BTreeMethods.getAndInitPage(pBt, newPgno, ref pNewPage);
                if (rc != 0)
                    return rc;
                this.apPage[i + 1] = pNewPage;
                this.aiIdx[i + 1] = 0;
                this.iPage++;
                this.info.nSize = 0;
                this.validNKey = false;
                if (pNewPage.nCell < 1 || pNewPage.intKey != this.apPage[i].intKey)
                {
                    return sqliteinth.SQLITE_CORRUPT_BKPT();
                }
                return SqlResult.SQLITE_OK;
            }
            public byte[] DataFetch(ref int pAmt, ref int outOffset)
            {
                byte[] p = null;
                Debug.Assert(this.pBtree.db.mutex.sqlite3_mutex_held());
                Debug.Assert(this.cursorHoldsMutex());
                if (Sqlite3.ALWAYS(this.State == BtCursorState.CURSOR_VALID))
                {
                    p = this.fetchPayload(ref pAmt, ref outOffset, true);
                }
                return p;
            }
            public byte[] KeyFetch(ref int pAmt, ref int outOffset)
            {
                byte[] p = null;
                Debug.Assert(this.pBtree.db.mutex.sqlite3_mutex_held());
                Debug.Assert(this.cursorHoldsMutex());
                if (Sqlite3.ALWAYS(this.State == BtCursorState.CURSOR_VALID))
                {
                    p = this.fetchPayload(ref pAmt, ref outOffset, false);
                }
                return p;
            }
            public byte[] fetchPayload(///
                ///<summary>
                ///Cursor pointing to entry to read from 
                ///</summary>
            ref int pAmt,///
                ///<summary>
                ///Write the number of available bytes here 
                ///</summary>
            ref int outOffset,///
                ///<summary>
                ///Offset into Buffer 
                ///</summary>
            bool skipKey///
                ///<summary>
                ///read beginning at data if this is true 
                ///</summary>
            )
            {
                byte[] aPayload;
                MemPage pPage;
                u32 nKey;
                u32 nLocal;
                Debug.Assert(this != null && this.iPage >= 0 && this.apPage[this.iPage] != null);
                Debug.Assert(this.State == BtCursorState.CURSOR_VALID);
                Debug.Assert(this.cursorHoldsMutex());
                outOffset = -1;
                pPage = this.apPage[this.iPage];
                Debug.Assert(this.aiIdx[this.iPage] < pPage.nCell);
                if (NEVER(this.info.nSize == 0))
                {
                    this.apPage[this.iPage].btreeParseCell(this.aiIdx[this.iPage], ref this.info);
                }
                //aPayload = pCur.info.pCell;
                //aPayload += pCur.info.nHeader;
                aPayload = malloc_cs.sqlite3Malloc(this.info.nSize - this.info.nHeader);
                if (pPage.intKey != false)
                {
                    nKey = 0;
                }
                else
                {
                    nKey = (u32)this.info.nKey;
                }
                if (skipKey)
                {
                    //aPayload += nKey;
                    outOffset = (int)(this.info.iCell + this.info.nHeader + nKey);
                    Buffer.BlockCopy(this.info.pCell, outOffset, aPayload, 0, (int)(this.info.nSize - this.info.nHeader - nKey));
                    nLocal = this.info.nLocal - nKey;
                }
                else
                {
                    outOffset = (int)(this.info.iCell + this.info.nHeader);
                    Buffer.BlockCopy(this.info.pCell, outOffset, aPayload, 0, this.info.nSize - this.info.nHeader);
                    nLocal = this.info.nLocal;
                    Debug.Assert(nLocal <= nKey);
                }
                pAmt = (int)nLocal;
                return aPayload;
            }
            public SqlResult sqlite3BtreeData(u32 offset, u32 amt, byte[] pBuf)
            {
                SqlResult rc;
#if !SQLITE_OMIT_INCRBLOB
																																																																												if ( pCur.State==BtCursorState.CURSOR_INVALID ){
return SQLITE_ABORT;
}
#endif
                Debug.Assert(this.cursorHoldsMutex());
                rc = this.restoreCursorPosition();
                if (rc == SqlResult.SQLITE_OK)
                {
                    Debug.Assert(this.State == BtCursorState.CURSOR_VALID);
                    Debug.Assert(this.iPage >= 0 && this.apPage[this.iPage] != null);
                    Debug.Assert(this.aiIdx[this.iPage] < this.apPage[this.iPage].nCell);
                    rc = this.accessPayload(offset, amt, pBuf, 0);
                }
                return rc;
            }
            public SqlResult sqlite3BtreeKey(u32 offset, u32 amt, byte[] pBuf)
            {
                Debug.Assert(this.cursorHoldsMutex());
                Debug.Assert(this.State == BtCursorState.CURSOR_VALID);
                Debug.Assert(this.iPage >= 0 && this.apPage[this.iPage] != null);
                Debug.Assert(this.aiIdx[this.iPage] < this.apPage[this.iPage].nCell);
                return this.accessPayload(offset, amt, pBuf, 0);
            }
            public SqlResult accessPayload(///
                ///<summary>
                ///Cursor pointing to entry to read from 
                ///</summary>
            u32 offset,///
                ///<summary>
                ///Begin reading this far into payload 
                ///</summary>
            u32 amt,///
                ///<summary>
                ///Read this many bytes 
                ///</summary>
            byte[] pBuf,///
                ///<summary>
                ///Write the bytes into this buffer 
                ///</summary>
            int eOp///
                ///<summary>
                ///</summary>
                ///<param name="zero to read. non">zero to write. </param>
            )
            {
                u32 pBufOffset = 0;
                byte[] aPayload;
                SqlResult rc = SqlResult.SQLITE_OK;
                u32 nKey;
                int iIdx = 0;
                MemPage pPage = this.apPage[this.iPage];
                ///
                ///<summary>
                ///Btree page of current entry 
                ///</summary>
                BtShared pBt = this.pBt;
                ///
                ///<summary>
                ///Btree this cursor belongs to 
                ///</summary>
                Debug.Assert(pPage != null);
                Debug.Assert(this.State == BtCursorState.CURSOR_VALID);
                Debug.Assert(this.aiIdx[this.iPage] < pPage.nCell);
                Debug.Assert(this.cursorHoldsMutex());
                this.getCellInfo();
                aPayload = this.info.pCell;
                //pCur.info.pCell + pCur.info.nHeader;
                nKey = (u32)(pPage.intKey != false ? 0 : (int)this.info.nKey);
                if (NEVER(offset + amt > nKey + this.info.nData) || this.info.nLocal > pBt.usableSize//&aPayload[pCur.info.nLocal] > &pPage.aData[pBt.usableSize]
                )
                {
                    ///
                    ///<summary>
                    ///Trying to read or write past the end of the data is an error 
                    ///</summary>
                    return sqliteinth.SQLITE_CORRUPT_BKPT();
                }
                ///
                ///<summary>
                ///Check if data must be read/written to/from the btree page itself. 
                ///</summary>
                if (offset < this.info.nLocal)
                {
                    int a = (int)amt;
                    if (a + offset > this.info.nLocal)
                    {
                        a = (int)(this.info.nLocal - offset);
                    }
                    rc = BTreeMethods.copyPayload(aPayload, (u32)(offset + this.info.iCell + this.info.nHeader), pBuf, pBufOffset, (u32)a, eOp, pPage.pDbPage);
                    offset = 0;
                    pBufOffset += (u32)a;
                    //pBuf += a;
                    amt -= (u32)a;
                }
                else
                {
                    offset -= this.info.nLocal;
                }
                if (rc == SqlResult.SQLITE_OK && amt > 0)
                {
                    u32 ovflSize = (u32)(pBt.usableSize - 4);
                    ///
                    ///<summary>
                    ///Bytes content per ovfl page 
                    ///</summary>
                    Pgno nextPage;
                    nextPage = Converter.sqlite3Get4byte(aPayload, this.info.nLocal + this.info.iCell + this.info.nHeader);
#if !SQLITE_OMIT_INCRBLOB
																																																																																																					/* If the isIncrblobHandle flag is set and the BtCursor.aOverflow[]
** has not been allocated, allocate it now. The array is sized at
** one entry for each overflow page in the overflow chain. The
** page number of the first overflow page is stored in aOverflow[0],
** etc. A value of 0 in the aOverflow[] array means "not yet known"
** (the cache is lazily populated).
*/
if( pCur.isIncrblobHandle && !pCur.aOverflow ){
int nOvfl = (pCur.info.nPayload-pCur.info.nLocal+ovflSize-1)/ovflSize;
pCur.aOverflow = (Pgno *)malloc_cs.sqlite3MallocZero(sizeof(Pgno)*nOvfl);
/* nOvfl is always positive.  If it were zero, fetchPayload would have
** been used instead of this routine. */
if( Sqlite3.ALWAYS(nOvfl) && !pCur.aOverflow ){
rc = SQLITE_NOMEM;
}
}

/* If the overflow page-list cache has been allocated and the
** entry for the first required overflow page is valid, skip
** directly to it.
*/
if( pCur.aOverflow && pCur.aOverflow[offset/ovflSize] ){
iIdx = (offset/ovflSize);
nextPage = pCur.aOverflow[iIdx];
offset = (offset%ovflSize);
}
#endif
                    for (; rc == SqlResult.SQLITE_OK && amt > 0 && nextPage != 0; iIdx++)
                    {
#if !SQLITE_OMIT_INCRBLOB
																																																																																																																														/* If required, populate the overflow page-list cache. */
if( pCur.aOverflow ){
Debug.Assert(!pCur.aOverflow[iIdx] || pCur.aOverflow[iIdx]==nextPage);
pCur.aOverflow[iIdx] = nextPage;
}
#endif
                        MemPage MemPageDummy = null;
                        if (offset >= ovflSize)
                        {
                            ///
                            ///<summary>
                            ///The only reason to read this page is to obtain the page
                            ///number for the next page in the overflow chain. The page
                            ///data is not required. So first try to lookup the overflow
                            ///</summary>
                            ///<param name="page">list cache, if any, then fall back to the getOverflowPage()</param>
                            ///<param name="function.">function.</param>
                            ///<param name=""></param>
#if !SQLITE_OMIT_INCRBLOB
																																																																																																																																																							if( pCur.aOverflow && pCur.aOverflow[iIdx+1] ){
nextPage = pCur.aOverflow[iIdx+1];
} else
#endif
                            rc = BTreeMethods.getOverflowPage(pBt, nextPage, out MemPageDummy, out nextPage);
                            offset -= ovflSize;
                        }
                        else
                        {
                            ///
                            ///<summary>
                            ///Need to read this page properly. It contains some of the
                            ///range of data that is being read (eOp==null) or written (eOp!=null).
                            ///
                            ///</summary>
                            PgHdr pDbPage = new PgHdr();
                            int a = (int)amt;
                            rc = pBt.pPager.sqlite3PagerGet(nextPage, ref pDbPage);
                            if (rc == SqlResult.SQLITE_OK)
                            {
                                aPayload = (pDbPage.sqlite3PagerGetData());
                                nextPage = Converter.sqlite3Get4byte(aPayload);
                                if (a + offset > ovflSize)
                                {
                                    a = (int)(ovflSize - offset);
                                }
                                rc = BTreeMethods.copyPayload(aPayload, offset + 4, pBuf, pBufOffset, (u32)a, eOp, pDbPage);
                                PagerMethods.sqlite3PagerUnref(pDbPage);
                                offset = 0;
                                amt -= (u32)a;
                                pBufOffset += (u32)a;
                                //pBuf += a;
                            }
                        }
                    }
                }
                if (rc == SqlResult.SQLITE_OK && amt > 0)
                {
                    return sqliteinth.SQLITE_CORRUPT_BKPT();
                }
                return rc;
            }
            public SqlResult sqlite3BtreeDataSize(ref u32 pSize)
            {
                Debug.Assert(this.cursorHoldsMutex());
                Debug.Assert(this.State == BtCursorState.CURSOR_VALID);
                this.getCellInfo();
                pSize = this.info.nData;
                return SqlResult.SQLITE_OK;
            }
            public SqlResult sqlite3BtreeKeySize(ref i64 pSize)
            {
                Debug.Assert(this.cursorHoldsMutex());
                Debug.Assert(this.State == BtCursorState.CURSOR_INVALID || this.State == BtCursorState.CURSOR_VALID);
                if (this.State != BtCursorState.CURSOR_VALID)
                {
                    pSize = 0;
                }
                else
                {
                    this.getCellInfo();
                    pSize = this.info.nKey;
                }
                return SqlResult.SQLITE_OK;
            }
            public bool sqlite3BtreeCursorIsValid()
            {
                return true;
            }
            public void getCellInfo()
            {
                if (this.info.nSize == 0)
                {
                    int iPage = this.iPage;
                    this.apPage[iPage].btreeParseCell(this.aiIdx[iPage], ref this.info);
                    this.validNKey = true;
                }
                else
                {
                    this.assertCellInfo();
                }
            }
            public SqlResult sqlite3BtreeCloseCursor()
            {
                Btree pBtree = this.pBtree;
                if (pBtree != null)
                {
                    int i;
                    BtShared pBt = this.pBt;
                    sqlite3BtreeEnter(pBtree);
                    this.sqlite3BtreeClearCursor();
                    if (this.pPrev != null)
                    {
                        this.pPrev.pNext = this.pNext;
                    }
                    else
                    {
                        pBt.pCursor = this.pNext;
                    }
                    if (this.pNext != null)
                    {
                        this.pNext.pPrev = this.pPrev;
                    }
                    for (i = 0; i <= this.iPage; i++)
                    {
                        BTreeMethods.releasePage(this.apPage[i]);
                    }
                    BTreeMethods.unlockBtreeIfUnused(pBt);
                    BTreeMethods.invalidateOverflowCache(this);
                    ///
                    ///<summary>
                    ///malloc_cs.sqlite3_free(ref pCur); 
                    ///</summary>
                    sqlite3BtreeLeave(pBtree);
                }
                return SqlResult.SQLITE_OK;
            }
            public void assertCellInfo()
            {
            }
            public void sqlite3BtreeCursorZero()
            {
                this.Clear();
                // memset( p, 0, offsetof( BtCursor, iPage ) );
            }
        }
        
        public const int CURSOR_INVALID = 0;
        public const int CURSOR_VALID = 1;
        public const int CURSOR_REQUIRESEEK = 2;
        public const int CURSOR_FAULT = 3;
	}
    ///
    ///<summary>
    ///Potential values for BtCursor.eState.
    ///
    ///CURSOR_VALID:
    ///VdbeCursor points to a valid entry. getPayload() etc. may be called.
    ///
    ///CURSOR_INVALID:
    ///VdbeCursor does not point to a valid entry. This can happen (for example)
    ///because the table is empty or because BtreeCursorFirst() has not been
    ///called.
    ///
    ///CURSOR_REQUIRESEEK:
    ///The table that this cursor was opened on still exists, but has been
    ///modified since the cursor was last used. The cursor position is saved
    ///in variables BtCursor.pKey and BtCursor.nKey. When a cursor is in
    ///this state, restoreCursorPosition() can be called to attempt to
    ///seek the cursor to the saved position.
    ///
    ///CURSOR_FAULT:
    ///A unrecoverable error (an I/O error or a malloc failure) has occurred
    ///on a different connection that shares the BtShared cache with this
    ///cursor.  The error has left the cache in an inconsistent state.
    ///Do nothing else with this cursor.  Any attempt to use the cursor
    ///should return the error code stored in BtCursor.skip
    ///
    ///</summary>
    public enum BtCursorState
    {
        CURSOR_INVALID = 0,
        CURSOR_VALID = 1,
        CURSOR_REQUIRESEEK = 2,
        CURSOR_FAULT = 3
    }
}
