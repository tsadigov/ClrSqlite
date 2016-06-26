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
namespace Community.CsharpSqlite.Tree {
    using DbPage = Cache.PgHdr;
    using System.Text;
    using Metadata;
    using Community.CsharpSqlite.Paging;
    using Community.CsharpSqlite.Utils;
    using Cache;
    public enum CursorMode {
        ReadOnly=0,
        ReadWrite=1
    }
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
    public class BtCursor:ILinkedListNode<BtCursor>, IBackwardLinkedListNode<BtCursor>
    {
            ///<summary>
            ///The Btree to which this cursor belongs 
            ///</summary>
            public Btree pBtree;

            ///
            ///<summary>
            ///The BtShared this cursor points to 
            ///</summary>
            public BtShared pBt;
            ///<summary>
            ///Forms a linked list of all cursors 
            ///</summary>
            public BtCursor pNext { get; set; }
            public BtCursor pPrev { get; set; }
            
            ///<summary>
            ///Argument passed to comparison function 
            ///</summary>
            public KeyInfo pKeyInfo;

            ///<summary>
            ///The root page of this tree 
            ///</summary>
            public Pgno pgnoRoot;
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
            public ThreeState skipNext;
        ///
        ///<summary>
        ///Prev() is noop if negative. Next() is noop if positive 
        ///</summary>
        public CursorMode wrFlag;
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
            public BtCursorState eState { get; set; }
            public BtCursorState State
            {
                get
                {
                    return (BtCursorState)eState;
                }
                set
                {
                    eState = value;
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
            ///<summary>
            ///Index of current page in apPage 
            ///</summary>
            public i16 pageStackIndex;


        ///<summary>
        ///Current index in apPage[i]
        ///</summary>
        public u16[] indexInPage = new u16[Limits.BTCURSOR_MAX_DEPTH];

        ///<summary>
        ///Pages from root to current page
        ///</summary>
        public MemPage[] PageStack = new MemPage[Limits.BTCURSOR_MAX_DEPTH];

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
                pageStackIndex = 0;
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
                Debug.Assert(BtCursorState.CURSOR_VALID == this.eState);
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
                if (false == this.PageStack[0].intKey)
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
                Debug.Assert(false == this.PageStack[0].intKey || null == this.pKey);
                if (rc == SqlResult.SQLITE_OK)
                {
                    int i;
                    for (i = 0; i <= this.pageStackIndex; i++)
                    {
                        BTreeMethods.release(this.PageStack[i]);
                        this.PageStack[i] = null;
                    }
                    this.pageStackIndex = -1;
                    this.eState = BtCursorState.CURSOR_REQUIRESEEK;
                }
                BTreeMethods.invalidateOverflowCache(this);
                return rc;
            }
            public void sqlite3BtreeClearCursor()
            {
                Debug.Assert(this.cursorHoldsMutex());
                malloc_cs.sqlite3_free(ref this.pKey);
                this.eState = BtCursorState.CURSOR_INVALID;
            }
            public SqlResult btreeMoveto(
                byte[] pKey,///Packed key if the btree is an index 

                i64 nKey,///Integer key for tables.  Size of pKey for indices 

                    ///Bias search to the high end 
                int bias,///
                    ///Write search results here 
                ref ThreeState pRes///

            )
            {
                SqlResult rc;
                ///Status code 
                UnpackedRecord pIdxKey;
                ///Unpacked index key 
                UnpackedRecord aSpace = new UnpackedRecord();
                //char aSpace[150]; /* Temp space for pIdxKey - to avoid a malloc */
                if (pKey != null)
                {
                    Debug.Assert(nKey == (i64)(int)nKey);
                    pIdxKey = Engine.vdbeaux.sqlite3VdbeRecordUnpack(this.pKeyInfo,(int)nKey, pKey, aSpace, 16);
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
                    Engine.vdbeaux.sqlite3VdbeDeleteUnpackedRecord(pIdxKey);//vdbeaux
                }
                return rc;
            }
            public SqlResult btreeRestoreCursorPosition()
            {
                SqlResult rc;
                Debug.Assert(this.cursorHoldsMutex());
                Debug.Assert(this.eState >= BtCursorState.CURSOR_REQUIRESEEK);
                if (this.eState == BtCursorState.CURSOR_FAULT)
                {
                    return (SqlResult)this.skipNext;
                }
                this.eState = BtCursorState.CURSOR_INVALID;
                rc = this.btreeMoveto(this.pKey, this.nKey, 0, ref this.skipNext);
                if (rc == SqlResult.SQLITE_OK)
                {
                    //malloc_cs.sqlite3_free(ref pCur.pKey);
                    this.pKey = null;
                    Debug.Assert(this.eState == BtCursorState.CURSOR_VALID || this.eState == BtCursorState.CURSOR_INVALID);
                }
                return rc;
            }
            public SqlResult restoreCursorPosition()
            {
                if (this.eState >= BtCursorState.CURSOR_REQUIRESEEK)
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
                if (this.eState != BtCursorState.CURSOR_VALID || this.skipNext != 0)
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
                ///Value to return in pnEntry 
                SqlResult rc;
                ///Return code 
                rc = this.moveToRoot();
                ///Unless an error occurs, the following loop runs one iteration for each
                ///<param name="page in the B">Tree structure (not including overflow pages).</param>
                while (rc == SqlResult.SQLITE_OK)
                {
                    int iIdx;
                    ///Index of child node in parent                     
                    ///<param name="Current page of the b">tree </param>
                    ///<param name="If this is a leaf page or the tree is not an int">key tree, then</param>
                    ///<param name="this page contains countable entries. Increment the entry counter">this page contains countable entries. Increment the entry counter</param>
                    ///<param name="accordingly.">accordingly.</param>
                    var pPage = this.PageStack[this.pageStackIndex];
                    if (pPage.IsLeaf != false || false == pPage.intKey)
                    {
                        nEntry += pPage.nCell;
                    }
                    ///pPage is a leaf node. This loop navigates the cursor so that it
                    ///points to the first interior cell that it points to the parent of
                    ///the next page in the tree that has not yet been visited. The
                    ///pCur.aiIdx[pCur.iPage] value is set to the index of the parent cell
                    ///of the page, or to the number of cells in the page if the next page
                    ///<param name="to visit is the right">child of its parent.</param>
                    ///<param name=""></param>
                    ///<param name="If all pages in the tree have been visited, return SqlResult.SQLITE_OK to the">If all pages in the tree have been visited, return SqlResult.SQLITE_OK to the</param>
                    ///<param name="caller.">caller.</param>
                    ///<param name=""></param>
                    if (pPage.IsLeaf != false)
                    {
                        do
                        {
                            if (this.pageStackIndex == 0)
                            {
                                ///All pages of the b-tree have been visited. Return successfully. 
                                pnEntry = nEntry;
                                return SqlResult.SQLITE_OK;
                            }
                            this.moveToParent();
                        }
                        while (this.indexInPage[this.pageStackIndex] >= this.PageStack[this.pageStackIndex].nCell);
                        this.indexInPage[this.pageStackIndex]++;
                        pPage = this.PageStack[this.pageStackIndex];
                    }

                    ///Descend to the child node of the cell that the cursor currently
                    ///points at. This is the right child if (iIdx==pPage.nCell).
                    iIdx = this.indexInPage[this.pageStackIndex];
                    if (iIdx == pPage.nCell)
                    {
                        rc = this.moveToChild(Converter.sqlite3Get4byte(pPage.aData, pPage.hdrOffset + 8));
                    }
                    else
                    {
                        rc = this.moveToChild(Converter.sqlite3Get4byte(pPage.aData, pPage.findCellAddress(iIdx)));
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
                ///<summary>
                ///Return code 
                ///</summary>
                MemPage pPage;
                ///<summary>
                ///Page to delete cell from 
                ///</summary>
                int pCell;
                ///<summary>
                ///Pointer to cell to delete 
                ///</summary>
                int iCellIdx;
                ///<summary>
                ///Index of cell to delete 
                ///</summary>
                int iCellDepth;
                ///<summary>
                ///Depth of node containing pCell 
                ///</summary>
                Debug.Assert(this.cursorHoldsMutex());
                Debug.Assert(pBt.inTransaction == TransType.TRANS_WRITE);
                Debug.Assert(!pBt.readOnly);
                Debug.Assert(this.wrFlag != 0);
                Debug.Assert(p.hasSharedCacheTableLock(this.pgnoRoot, this.pKeyInfo != null ? 1 : 0, 2));
                Debug.Assert(!p.hasReadConflicts(this.pgnoRoot));
                if (NEVER(this.indexInPage[this.pageStackIndex] >= this.PageStack[this.pageStackIndex].nCell) || NEVER(this.eState != BtCursorState.CURSOR_VALID))
                {
                    return SqlResult.SQLITE_ERROR;
                    ///<summary>
                    ///Something has gone awry. 
                    ///</summary>
                }
                ///
                ///<param name="If this is a delete operation to remove a row from a table b">tree,</param>
                ///<param name="invalidate any incrblob cursors open on the row being deleted.  ">invalidate any incrblob cursors open on the row being deleted.  </param>
                if (this.pKeyInfo == null)
                {
                    p.invalidateIncrblobCursors(this.info.nKey, 0);
                }
                iCellDepth = this.pageStackIndex;
                iCellIdx = this.indexInPage[iCellDepth];
                pPage = this.PageStack[iCellDepth];
                pCell = pPage.findCellAddress(iCellIdx);
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
                    ThreeState notUsed = 0;
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
                rc = pPage.clearCell( pCell);
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
                    MemPage pLeaf = this.PageStack[this.pageStackIndex];
                    int nCell;
                    Pgno n = this.PageStack[iCellDepth + 1].pgno;
                    //byte[] pTmp;
                    pCell = pLeaf.findCellAddress(pLeaf.nCell - 1);
                    nCell = pLeaf.cellSizePtr(pCell);
                    Debug.Assert(pBt.MX_CELL_SIZE >= nCell);
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
                if (rc == SqlResult.SQLITE_OK && this.pageStackIndex > iCellDepth)
                {
                    while (this.pageStackIndex > iCellDepth)
                    {
                        BTreeMethods.release(this.PageStack[this.pageStackIndex--]);
                    }
                    rc = this.balance();
                }
                if (rc == SqlResult.SQLITE_OK)
                {
                    this.moveToRoot();
                }
                return rc;
            }

            private bool NEVER(bool p)
            {
                return Sqlite3.NEVER(p);
            }
            public SqlResult sqlite3BtreeInsert(
                byte[] pKey, i64 nKey,///The key of the new record                 
                byte[] pData, int nData,///The data of the new record 
                int nZero,///Number of extra 0 bytes to append to data                 
                int appendBias,///True if this is likely an append 
                ThreeState seekResult///Result of prior MovetoUnpacked() call 
            )
            {                
                if (this.State == BtCursorState.CURSOR_FAULT)
                {
                    Debug.Assert(this.skipNext != (int)SqlResult.SQLITE_OK);
                    return (SqlResult)this.skipNext;
                }

            ThreeState loc = seekResult;
            ///1: before desired location  +1: after 
            int szNew = 0;
            Btree p = this.pBtree;
            BtShared pBt = p.pBt;

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
                ///In some cases, the call to btreeMoveto() below is a no-op. For
                ///example, when inserting data into a table with auto-generated integer
                ///keys, the VDBE layer invokes sqlite3BtreeLast() to figure out the
                ///integer key to use. It then calls this function to actually insert the
                ///data into the intkey B-Tree. In this case btreeMoveto() recognizes
                ///that the cursor is already where it needs to be and returns without
                ///doing any work. To avoid thwarting these optimizations, it is important
                ///not to clear the cursor here.
                var rc = pBt.saveAllCursors(this.pgnoRoot, this);
                if (rc != 0)
                    return rc;
                if (ThreeState.Neutral == loc)
                {
                    rc = this.btreeMoveto(pKey, nKey, appendBias, ref loc);
                    if (rc != 0)
                        return rc;
                }
                Debug.Assert(this.State == BtCursorState.CURSOR_VALID || (this.State == BtCursorState.CURSOR_INVALID && loc != 0));
            var pPage = CurrentPage;// this.PageStack[this.pageStackIndex];
                Debug.Assert(pPage.intKey != false || nKey >= 0);
                Debug.Assert(pPage.IsLeaf != false || false == pPage.intKey);
                Log.TRACE("INSERT: table=%d nkey=%lld ndata=%d page=%d %s\n", this.pgnoRoot, nKey, nData, pPage.pgno, loc == 0 ? "overwrite" : "new entry");
                Debug.Assert(pPage.isInit != false);
                pBt.allocateTempSpace();
                var newCellTemp = pBt.pTmpSpace;
                //if (newCell == null) return SQLITE_NOMEM;
                rc = pPage.fillInCell(newCellTemp, pKey, nKey, pData, nData, nZero, ref szNew);
                if (rc != 0)
                    goto end_insert;
                Debug.Assert(szNew == pPage.cellSizePtr(newCellTemp));
                Debug.Assert(szNew <= pBt.MX_CELL_SIZE);
            var idx = this.CurrentIndex;
                if (ThreeState.Neutral==loc)
                {
                    Debug.Assert(idx < pPage.nCell);
                    rc = PagerMethods.sqlite3PagerWrite(pPage.pDbPage);
                    if (rc != 0)
                    {
                        goto end_insert;
                    }
                    var oldCellAddress = pPage.findCellAddress(idx);
                    if ( !pPage.IsLeaf )
                    {
                        //memcpy(newCell, oldCell, 4);
                        newCellTemp[0] = pPage.aData[oldCellAddress + 0];
                        newCellTemp[1] = pPage.aData[oldCellAddress + 1];
                        newCellTemp[2] = pPage.aData[oldCellAddress + 2];
                        newCellTemp[3] = pPage.aData[oldCellAddress + 3];
                    }
                    var szOld = pPage.cellSizePtr(oldCellAddress);
                    rc = pPage.clearCell( oldCellAddress);
                    pPage.dropCell(idx, szOld, ref rc);
                    if (rc != 0)
                        goto end_insert;
                }
                else
                    if (loc < 0 && pPage.nCell > 0)
                    {
                        Debug.Assert(pPage.IsLeaf);
                        idx = ++this.indexInPage[this.pageStackIndex];
                    }
                    else
                    {
                        Debug.Assert(pPage.IsLeaf != false);
                    }
                pPage.insertCell(idx, newCellTemp, szNew, null, 0, ref rc);
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
                    ///Must make sure nOverflow is reset to zero even if the balance()
                    ///fails. Internal data structure corruption will result otherwise.
                    ///Also, set the cursor state to invalid. This stops saveCursorPosition()
                    ///from trying to save the current position of the cursor.  
                    this.CurrentPage.nOverflow = 0;
                    this.State = BtCursorState.CURSOR_INVALID;
                }
                Debug.Assert(this.PageStack[this.pageStackIndex].nOverflow == 0);
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
                    int iPage = this.pageStackIndex;
                    MemPage pPage = this.PageStack[iPage];
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
                            rc = pPage.balance_deeper(ref this.PageStack[1]);
                            if (rc == SqlResult.SQLITE_OK)
                            {
                                this.pageStackIndex = 1;
                                this.indexInPage[0] = 0;
                                this.indexInPage[1] = 0;
                                Debug.Assert(this.PageStack[1].nOverflow != 0);
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
                            MemPage pParent = this.PageStack[iPage - 1];
                            int iIdx = this.indexInPage[iPage - 1];
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
                            BTreeMethods.release(pPage);
                            this.pageStackIndex--;
                        }
                }
                while (rc == SqlResult.SQLITE_OK);
                //if (pFree != null)
                //{
                //  sqlite3PageFree(ref pFree);
                //}
                return rc;
            }
            public SqlResult sqlite3BtreePrevious(ref ThreeState pRes)
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
                if (BtCursorState.CURSOR_INVALID == this.eState)
                {
                    pRes = ThreeState.Positive;
                    return SqlResult.SQLITE_OK;
                }
                if (this.skipNext < 0)
                {
                    this.skipNext = 0;
                    pRes = ThreeState.Neutral;
                    return SqlResult.SQLITE_OK;
                }
                this.skipNext = 0;
                pPage = this.PageStack[this.pageStackIndex];
                Debug.Assert(pPage.isInit != false);
                if (false == pPage.IsLeaf)
                {
                    int idx = this.indexInPage[this.pageStackIndex];
                    rc = this.moveToChild(Converter.sqlite3Get4byte(pPage.aData, pPage.findCellAddress(idx)));
                    if (rc != 0)
                    {
                        return rc;
                    }
                    rc = this.moveToRightmost();
                }
                else
                {
                    while (this.indexInPage[this.pageStackIndex] == 0)
                    {
                        if (this.pageStackIndex == 0)
                        {
                            this.State = BtCursorState.CURSOR_INVALID;
                            pRes = ThreeState.Positive;
                            return SqlResult.SQLITE_OK;
                        }
                        this.moveToParent();
                    }
                    this.info.nSize = 0;
                    this.validNKey = false;
                    this.indexInPage[this.pageStackIndex]--;
                    pPage = this.PageStack[this.pageStackIndex];
                    if (pPage.intKey != false && false == pPage.IsLeaf)
                    {
                        rc = this.sqlite3BtreePrevious(ref pRes);
                    }
                    else
                    {
                        rc = SqlResult.SQLITE_OK;
                    }
                }
                pRes = ThreeState.Neutral;
                return rc;
            }
            public SqlResult sqlite3BtreeNext(ref ThreeState pRes)
            {
                Debug.Assert(this.cursorHoldsMutex());
                var rc = this.restoreCursorPosition();
                if (rc != SqlResult.SQLITE_OK)
                {
                    return rc;
                }
                // Not needed in C# // Debug.Assert( pRes != 0 );
                if (BtCursorState.CURSOR_INVALID == this.eState)
                {
                    pRes = ThreeState.Positive;
                    return SqlResult.SQLITE_OK;
                }
                if (this.skipNext > 0)
                {
                    this.skipNext = 0;
                    pRes = 0;
                    return SqlResult.SQLITE_OK;
                }
                this.skipNext = 0;
                var pPage = this.PageStack[this.pageStackIndex];
                var idx = ++this.indexInPage[this.pageStackIndex];
                Debug.Assert(pPage.isInit != false);
                Debug.Assert(idx <= pPage.nCell);
                this.info.nSize = 0;
                this.validNKey = false;
                if (idx >= pPage.nCell)
                {
                    if ( ! pPage.IsLeaf )
                    {
                        rc = this.moveToChild(pPage.ChildPageNo);
                        if (rc != SqlResult.SQLITE_OK)
                            return rc;
                        rc = this.moveToLeftmost();
                        pRes = 0;
                        return rc;
                    }
                    do
                    {
                        if (this.pageStackIndex == 0)
                        {
                            pRes = ThreeState.Positive;
                            this.State = BtCursorState.CURSOR_INVALID;
                            return SqlResult.SQLITE_OK;
                        }
                        this.moveToParent();
                        pPage = this.PageStack[this.pageStackIndex];
                    }
                    while (this.indexInPage[this.pageStackIndex] >= pPage.nCell);
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
            ref ThreeState pRes///
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
                if (this.State == BtCursorState.CURSOR_VALID && this.validNKey && this.PageStack[0].intKey != false)
                {
                    if (this.info.nKey == intKey)
                    {
                        pRes = 0;
                        return SqlResult.SQLITE_OK;
                    }
                    if (this.atLast != 0 && this.info.nKey < intKey)
                    {
                        pRes = ThreeState.Negative;
                        return SqlResult.SQLITE_OK;
                    }
                }
                rc = this.moveToRoot();
                if (rc != 0)
                {
                    return rc;
                }
                Debug.Assert(this.PageStack[this.pageStackIndex] != null);
                Debug.Assert(this.PageStack[this.pageStackIndex].isInit != false);
                Debug.Assert(this.PageStack[this.pageStackIndex].nCell > 0 || this.State == BtCursorState.CURSOR_INVALID);
                if (this.State == BtCursorState.CURSOR_INVALID)
                {
                    pRes = ThreeState.Negative;
                    Debug.Assert(this.PageStack[this.pageStackIndex].nCell == 0);
                    return SqlResult.SQLITE_OK;
                }
                Debug.Assert(this.PageStack[0].intKey != false || pIdxKey != null);
                for (; ; )
                {
                    int lwr, upr, idx;
                    Pgno chldPg;
                    MemPage pPage = this.PageStack[this.pageStackIndex];
                    ThreeState c;
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
                        this.indexInPage[this.pageStackIndex] = (u16)(idx = upr);
                    }
                    else
                    {
                        this.indexInPage[this.pageStackIndex] = (u16)(idx = (upr + lwr) / 2);
                    }
                    for (; ; )
                    {
                        int pCell;
                        ///
                        ///<summary>
                        ///Pointer to current cell in pPage 
                        ///</summary>
                        Debug.Assert(idx == this.indexInPage[this.pageStackIndex]);
                        this.info.nSize = 0;
                        pCell = pPage.findCellAddress(idx) + pPage.childPtrSize;
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
                                    c = ThreeState.Negative;
                                }
                                else
                                {
                                    Debug.Assert(nCellKey > intKey);
                                    c = ThreeState.Positive;
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
                                c = Engine.vdbeaux.sqlite3VdbeRecordCompare(nCell, pPage.aData, pCell + 1, pIdxKey);
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
                                    c = Engine.vdbeaux.sqlite3VdbeRecordCompare(nCell, pPage.aData, pCell + 2, pIdxKey);
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
                                    c = Engine.vdbeaux.sqlite3VdbeRecordCompare(nCell, pCellKey, pIdxKey);
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
                        this.indexInPage[this.pageStackIndex] = (u16)(idx = (lwr + upr) / 2);
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
                            chldPg = Converter.sqlite3Get4byte(pPage.aData, pPage.findCellAddress(lwr));
                        }
                    if (chldPg == 0)
                    {
                        Debug.Assert(this.indexInPage[this.pageStackIndex] < this.PageStack[this.pageStackIndex].nCell);
                        pRes = c;
                        rc = SqlResult.SQLITE_OK;
                        goto moveto_finish;
                    }
                    this.indexInPage[this.pageStackIndex] = (u16)lwr;
                    this.info.nSize = 0;
                    this.validNKey = false;
                    rc = this.moveToChild(chldPg);
                    if (rc != 0)
                        goto moveto_finish;
                }
            moveto_finish:
                return rc;
            }
            public SqlResult sqlite3BtreeLast(ref ThreeState pRes)
            {
                SqlResult rc;
                Debug.Assert(this.cursorHoldsMutex());
                Debug.Assert(this.pBtree.db.mutex.sqlite3_mutex_held());
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
                        Debug.Assert(this.PageStack[this.pageStackIndex].nCell == 0);
                        pRes = ThreeState.Positive;
                    }
                    else
                    {
                        Debug.Assert(this.State == BtCursorState.CURSOR_VALID);
                        pRes = ThreeState.Neutral;
                        rc = this.moveToRightmost();
                        this.atLast = (u8)(rc == SqlResult.SQLITE_OK ? 1 : 0);
                    }
                }
                return rc;
            }
            public SqlResult sqlite3BtreeFirst(ref ThreeState pRes)
            {
                SqlResult rc;
                Debug.Assert(this.cursorHoldsMutex());
                Debug.Assert(this.pBtree.db.mutex.sqlite3_mutex_held());
                
                rc = this.moveToRoot();
                if (rc == SqlResult.SQLITE_OK)
                {
                    if (this.State == BtCursorState.CURSOR_INVALID)
                    {
                        Debug.Assert(this.PageStack[this.pageStackIndex].nCell == 0);
                        pRes = ThreeState.Positive;
                    }
                    else
                    {
                        Debug.Assert(this.PageStack[this.pageStackIndex].nCell > 0);
                        pRes = ThreeState.Neutral;
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
                while (rc == SqlResult.SQLITE_OK && false == (pPage = this.PageStack[this.pageStackIndex]).IsLeaf)
                {
                    pgno = Converter.sqlite3Get4byte(pPage.aData, pPage.hdrOffset + 8);
                    this.indexInPage[this.pageStackIndex] = pPage.nCell;
                    rc = this.moveToChild(pgno);
                }
                if (rc == SqlResult.SQLITE_OK)
                {
                    this.indexInPage[this.pageStackIndex] = (u16)(pPage.nCell - 1);
                    this.info.nSize = 0;
                    this.validNKey = false;
                }
                return rc;
            }
            public SqlResult moveToLeftmost()
            {
                var rc = SqlResult.SQLITE_OK;
                
                Debug.Assert(this.cursorHoldsMutex());
                Debug.Assert(this.State == BtCursorState.CURSOR_VALID);

                while (   rc == SqlResult.SQLITE_OK &&   
                        ! this.CurrentPage.IsLeaf   )
                {
                    var cellIndex = CurrentIndex;
                    Debug.Assert(cellIndex< this.CurrentPage.nCell);                    
                    var pgno = Converter.sqlite3Get4byte(this.CurrentPage.aData, this.CurrentPage.findCellAddress(cellIndex));
                    rc = this.moveToChild(pgno);
                }
                return rc;
            }
        public u16 CurrentIndex {
            get { return this.indexInPage[this.pageStackIndex]; }
        }
        public MemPage CurrentPage {
            get { return this.PageStack[this.pageStackIndex]; }
        }
            public SqlResult moveToRoot()
            {
                MemPage pRoot;
                SqlResult rc = SqlResult.SQLITE_OK;
                Btree p = this.pBtree;
                BtShared pBt = p.pBt;
                Debug.Assert(this.cursorHoldsMutex());
                Debug.Assert(BtCursorState.CURSOR_INVALID < BtCursorState.CURSOR_REQUIRESEEK);
                Debug.Assert(BtCursorState.CURSOR_VALID < BtCursorState.CURSOR_REQUIRESEEK);
                Debug.Assert(BtCursorState.CURSOR_FAULT > BtCursorState.CURSOR_REQUIRESEEK);
                if (this.eState >= BtCursorState.CURSOR_REQUIRESEEK)
                {
                    if (this.State == BtCursorState.CURSOR_FAULT)
                    {
                        Debug.Assert(this.skipNext != (int)SqlResult.SQLITE_OK);
                        return (SqlResult)this.skipNext;
                    }
                    this.sqlite3BtreeClearCursor();
                }
                if (this.pageStackIndex >= 0)
                {
                    int i;
                    for (i = 1; i <= this.pageStackIndex; i++)
                    {
                        BTreeMethods.release(this.PageStack[i]);
                    }
                    this.pageStackIndex = 0;
                }
                else
                {
                    rc = pBt.getAndInitPage( this.pgnoRoot, ref this.PageStack[0]);
                    if (rc != SqlResult.SQLITE_OK)
                    {
                        this.State = BtCursorState.CURSOR_INVALID;
                        return rc;
                    }
                    this.pageStackIndex = 0;
                    ///
                    ///<summary>
                    ///If pCur.pKeyInfo is not NULL, then the caller that opened this cursor
                    ///</summary>
                    ///<param name="expected to open it on an index b">tree. Otherwise, if pKeyInfo is</param>
                    ///<param name="NULL, the caller expects a table b">tree. If this is not the case,</param>
                    ///<param name="return an SQLITE_CORRUPT error.  ">return an SQLITE_CORRUPT error.  </param>
                    Debug.Assert(this.PageStack[0].intKey == false || this.PageStack[0].intKey == false);
                    if ((this.pKeyInfo == null) != (this.PageStack[0].intKey != false))
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
                pRoot = this.PageStack[0];
                Debug.Assert(pRoot.pgno == this.pgnoRoot);
                Debug.Assert(pRoot.isInit != false && (this.pKeyInfo == null) == (pRoot.intKey != false));
                this.indexInPage[0] = 0;
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
                Debug.Assert(this.pageStackIndex > 0);
                Debug.Assert(this.PageStack[this.pageStackIndex] != null);
                this.PageStack[this.pageStackIndex - 1].assertParentIndex(this.indexInPage[this.pageStackIndex - 1], this.PageStack[this.pageStackIndex].pgno);
                BTreeMethods.release(this.PageStack[this.pageStackIndex]);
                this.pageStackIndex--;
                this.info.nSize = 0;
                this.validNKey = false;
            }
            public SqlResult moveToChild(u32 newPgno)
            {
                int stackIdx = this.pageStackIndex;
                MemPage pNewPage = new MemPage();
                BtShared pBt = this.pBt;
                Debug.Assert(this.cursorHoldsMutex());
                Debug.Assert(this.State == BtCursorState.CURSOR_VALID);
                Debug.Assert(this.pageStackIndex < Limits.BTCURSOR_MAX_DEPTH);
                if (this.pageStackIndex >= (Limits.BTCURSOR_MAX_DEPTH - 1))
                {
                    return sqliteinth.SQLITE_CORRUPT_BKPT();
                }
                var rc = pBt.getAndInitPage( newPgno, ref pNewPage);
                if (rc != 0)
                    return rc;
                this.PageStack[stackIdx + 1] = pNewPage;
                this.indexInPage[stackIdx + 1] = 0;
                this.pageStackIndex++;
                this.info.nSize = 0;
                this.validNKey = false;
                if (pNewPage.nCell < 1 || pNewPage.intKey != this.PageStack[stackIdx].intKey)
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
            public byte[] fetchPayload(
                ///Cursor pointing to entry to read from 
            ref int pAmt,
                ///Write the number of available bytes here 
            ref int outOffset,
                ///Offset into Buffer 
            bool skipKey
                ///read beginning at data if this is true 
            )
            {
                u32 nKey;
                u32 nLocal;
                Debug.Assert(this != null && this.pageStackIndex >= 0 && this.PageStack[this.pageStackIndex] != null);
                Debug.Assert(this.State == BtCursorState.CURSOR_VALID);
                Debug.Assert(this.cursorHoldsMutex());
                outOffset = -1;
                var pPage = this.PageStack[this.pageStackIndex];
                Debug.Assert(this.indexInPage[this.pageStackIndex] < pPage.nCell);
                if (NEVER(this.info.nSize == 0))
                {
                    this.PageStack[this.pageStackIndex].btreeParseCell(this.indexInPage[this.pageStackIndex], ref this.info);
                }
                //aPayload = pCur.info.pCell;
                //aPayload += pCur.info.nHeader;
                var aPayload = malloc_cs.sqlite3Malloc(this.info.nSize - this.info.nHeader);
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
                    Debug.Assert(this.pageStackIndex >= 0 && this.PageStack[this.pageStackIndex] != null);
                    Debug.Assert(this.indexInPage[this.pageStackIndex] < this.PageStack[this.pageStackIndex].nCell);
                    rc = this.accessPayload(offset, amt, pBuf, 0);
                }
                return rc;
            }
            public SqlResult sqlite3BtreeKey(u32 offset, u32 amt, byte[] pBuf)
            {
                Debug.Assert(this.cursorHoldsMutex());
                Debug.Assert(this.State == BtCursorState.CURSOR_VALID);
                Debug.Assert(this.pageStackIndex >= 0 && this.PageStack[this.pageStackIndex] != null);
                Debug.Assert(this.indexInPage[this.pageStackIndex] < this.PageStack[this.pageStackIndex].nCell);
                return this.accessPayload(offset, amt, pBuf, 0);
            }
            public SqlResult accessPayload(
                u32 offset,     ///Begin reading this far into payload 
                u32 amt,        ///Read this many bytes 
                byte[] pBuf,    ///Write the bytes into this buffer 
                int eOp         ///zero to read. non-zero to write. 
            )
        {
                u32 pBufOffset = 0;
                byte[] aPayload;
                SqlResult rc = SqlResult.SQLITE_OK;
                u32 nKey;
                int iIdx = 0;
                MemPage pPage = this.PageStack[this.pageStackIndex];
                ///Btree page of current entry 
                BtShared pBt = this.pBt;
                ///Btree this cursor belongs to 
                Debug.Assert(pPage != null);
                Debug.Assert(this.State == BtCursorState.CURSOR_VALID);
                Debug.Assert(this.indexInPage[this.pageStackIndex] < pPage.nCell);
                Debug.Assert(this.cursorHoldsMutex());
                this.getCellInfo();
                aPayload = this.info.pCell;
                //pCur.info.pCell + pCur.info.nHeader;
                nKey = (u32)(pPage.intKey != false ? 0 : (int)this.info.nKey);
                if (NEVER(offset + amt > nKey + this.info.nData) || this.info.nLocal > pBt.usableSize//&aPayload[pCur.info.nLocal] > &pPage.aData[pBt.usableSize]
                )
                {
                    ///Trying to read or write past the end of the data is an error 
                    return sqliteinth.SQLITE_CORRUPT_BKPT();
                }
                ///Check if data must be read/written to/from the btree page itself. 
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
                            ///The only reason to read this page is to obtain the page
                            ///number for the next page in the overflow chain. The page
                            ///data is not required. So first try to lookup the overflow
                            ///page-list cache, if any, then fall back to the getOverflowPage()
                            ///function.
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
                            ///Need to read this page properly. It contains some of the
                            ///range of data that is being read (eOp==null) or written (eOp!=null).
                            PgHdr pDbPage = new PgHdr();
                            int a = (int)amt;
                            rc = pBt.pPager.Get(nextPage, ref pDbPage);
                            if (rc == SqlResult.SQLITE_OK)
                            {
                                aPayload = (pDbPage.getData());
                                nextPage = Converter.sqlite3Get4byte(aPayload);
                                if (a + offset > ovflSize)
                                {
                                    a = (int)(ovflSize - offset);
                                }
                                rc = BTreeMethods.copyPayload(aPayload, offset + 4, pBuf, pBufOffset, (u32)a, eOp, pDbPage);
                                pDbPage.Unref();
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
                    this.CurrentPage.btreeParseCell(this.CurrentIndex, ref this.info);
                    this.validNKey = true;
                }
                else
                {
                    this.assertCellInfo();
                }
            }
            public SqlResult Close()
            {
                Btree pBtree = this.pBtree;
                if (pBtree != null)
                {
                    int i;
                    BtShared pBt = this.pBt;
                    pBtree.Enter();
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
                    for (i = 0; i <= this.pageStackIndex; i++)
                    {
                        BTreeMethods.release(this.PageStack[i]);
                    }
                    pBt.unlockIfUnused();
                    BTreeMethods.invalidateOverflowCache(this);
                    ///
                    ///<summary>
                    ///malloc_cs.sqlite3_free(ref pCur); 
                    ///</summary>
                    pBtree.Exit();
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
