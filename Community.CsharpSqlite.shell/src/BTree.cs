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
		const int BTCURSOR_MAX_DEPTH = 20;

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

			public CellInfo info = new CellInfo ();

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
            public BtCursorState State {
                get { return (BtCursorState)eState; }
                set { eState = (int)value; }
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
			public void Clear ()
			{
				pNext = null;
				pPrev = null;
				pKeyInfo = null;
				pgnoRoot = 0;
				cachedRowid = 0;
				info = new CellInfo ();
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

			public BtCursor Copy ()
			{
				BtCursor cp = (BtCursor)MemberwiseClone ();
				return cp;
			}

			public bool cursorHoldsMutex ()
			{
				return true;
			}

			public int saveCursorPosition ()
			{
				int rc;
				Debug.Assert (CURSOR_VALID == this.eState);
				Debug.Assert (null == this.pKey);
				Debug.Assert (this.cursorHoldsMutex ());
				rc = sqlite3BtreeKeySize (this, ref this.nKey);
				Debug.Assert (rc == SQLITE_OK);
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

				if (0 == this.apPage [0].intKey) {
					byte[] pKey = sqlite3Malloc ((int)this.nKey);
					//if( pKey !=null){
					rc = sqlite3BtreeKey (this, 0, (u32)this.nKey, pKey);
					if (rc == SQLITE_OK) {
						this.pKey = pKey;
					}
					//else{
					//  sqlite3_free(ref pKey);
					//}
					//}else{
					//  rc = SQLITE_NOMEM;
					//}
				}
				Debug.Assert (0 == this.apPage [0].intKey || null == this.pKey);
				if (rc == SQLITE_OK) {
					int i;
					for (i = 0; i <= this.iPage; i++) {
						releasePage (this.apPage [i]);
						this.apPage [i] = null;
					}
					this.iPage = -1;
					this.eState = CURSOR_REQUIRESEEK;
				}
				invalidateOverflowCache (this);
				return rc;
			}

			public void sqlite3BtreeClearCursor ()
			{
				Debug.Assert (this.cursorHoldsMutex ());
				sqlite3_free (ref this.pKey);
				this.eState = CURSOR_INVALID;
			}

			public int btreeMoveto (///
///<summary>
///Cursor open on the btree to be searched 
///</summary>

			byte[] pKey, ///
///<summary>
///Packed key if the btree is an index 
///</summary>

			i64 nKey, ///
///<summary>
///Integer key for tables.  Size of pKey for indices 
///</summary>

			int bias, ///
///<summary>
///Bias search to the high end 
///</summary>

			ref int pRes///
///<summary>
///Write search results here 
///</summary>

			)
			{
				int rc;
				///
///<summary>
///Status code 
///</summary>

				UnpackedRecord pIdxKey;
				///
///<summary>
///Unpacked index key 
///</summary>

				UnpackedRecord aSpace = new UnpackedRecord ();
				//char aSpace[150]; /* Temp space for pIdxKey - to avoid a malloc */
				if (pKey != null) {
					Debug.Assert (nKey == (i64)(int)nKey);
					pIdxKey = sqlite3VdbeRecordUnpack (this.pKeyInfo, (int)nKey, pKey, aSpace, 16);
					//sizeof( aSpace ) );
					//if ( pIdxKey == null )
					//  return SQLITE_NOMEM;
				}
				else {
					pIdxKey = null;
				}
				rc = sqlite3BtreeMovetoUnpacked (this, pIdxKey, nKey, bias != 0 ? 1 : 0, ref pRes);
				if (pKey != null) {
					sqlite3VdbeDeleteUnpackedRecord (pIdxKey);
				}
				return rc;
			}

			public int btreeRestoreCursorPosition ()
			{
				int rc;
				Debug.Assert (this.cursorHoldsMutex ());
				Debug.Assert (this.eState >= CURSOR_REQUIRESEEK);
				if (this.eState == CURSOR_FAULT) {
					return this.skipNext;
				}
				this.eState = CURSOR_INVALID;
				rc = this.btreeMoveto (this.pKey, this.nKey, 0, ref this.skipNext);
				if (rc == SQLITE_OK) {
					//sqlite3_free(ref pCur.pKey);
					this.pKey = null;
					Debug.Assert (this.eState == CURSOR_VALID || this.eState == CURSOR_INVALID);
				}
				return rc;
			}

			public int restoreCursorPosition ()
			{
				if (this.eState >= CURSOR_REQUIRESEEK)
					return this.btreeRestoreCursorPosition ();
				else
					return SQLITE_OK;
			}

			public int sqlite3BtreeCursorHasMoved (ref int pHasMoved)
			{
				int rc;
				rc = this.restoreCursorPosition ();
				if (rc != 0) {
					pHasMoved = 1;
					return rc;
				}
				if (this.eState != CURSOR_VALID || this.skipNext != 0) {
					pHasMoved = 1;
				}
				else {
					pHasMoved = 0;
				}
				return SQLITE_OK;
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

			public void sqlite3BtreeSetCachedRowid (sqlite3_int64 iRowid)
			{
				BtCursor p;
				for (p = this.pBt.pCursor; p != null; p = p.pNext) {
					if (p.pgnoRoot == this.pgnoRoot)
						p.cachedRowid = iRowid;
				}
				Debug.Assert (this.cachedRowid == iRowid);
			}

			///
///<summary>
///Return the cached rowid for the given cursor.  A negative or zero
///return value indicates that the rowid cache is invalid and should be
///ignored.  If the rowid cache has never before been set, then a
///zero is returned.
///
///</summary>

			public sqlite3_int64 sqlite3BtreeGetCachedRowid ()
			{
				return this.cachedRowid;
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

        public enum BtCursorState { 
            CURSOR_INVALID = 0,

            CURSOR_VALID = 1,

            CURSOR_REQUIRESEEK = 2,

            CURSOR_FAULT = 3

        }
        const int CURSOR_INVALID = 0;

        const int CURSOR_VALID = 1;

        const int CURSOR_REQUIRESEEK = 2;

        const int CURSOR_FAULT = 3;

    }
}
