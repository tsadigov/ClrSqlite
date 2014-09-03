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
	using DbPage=Sqlite3.PgHdr;
	using System.Text;
	public partial class Sqlite3 {
		/*
    ** 2004 April 6
    **
    ** The author disclaims copyright to this source code.  In place of
    ** a legal notice, here is a blessing:
    **
    **    May you do good and not evil.
    **    May you find forgiveness for yourself and forgive others.
    **    May you share freely, never taking more than you give.
    **
    *************************************************************************
    **
    ** This file implements a external (disk-based) database using BTrees.
    ** For a detailed discussion of BTrees, refer to
    **
    **     Donald E. Knuth, THE ART OF COMPUTER PROGRAMMING, Volume 3:
    **     "Sorting And Searching", pages 473-480. Addison-Wesley
    **     Publishing Company, Reading, Massachusetts.
    **
    ** The basic idea is that each page of the file contains N database
    ** entries and N+1 pointers to subpages.
    **
    **   ----------------------------------------------------------------
    **   |  Ptr(0) | Key(0) | Ptr(1) | Key(1) | ... | Key(N-1) | Ptr(N) |
    **   ----------------------------------------------------------------
    **
    ** All of the keys on the page that Ptr(0) points to have values less
    ** than Key(0).  All of the keys on page Ptr(1) and its subpages have
    ** values greater than Key(0) and less than Key(1).  All of the keys
    ** on Ptr(N) and its subpages have values greater than Key(N-1).  And
    ** so forth.
    **
    ** Finding a particular key requires reading O(log(M)) pages from the
    ** disk where M is the number of entries in the tree.
    **
    ** In this implementation, a single file can hold one or more separate
    ** BTrees.  Each BTree is identified by the index of its root page.  The
    ** key and data for any entry are combined to form the "payload".  A
    ** fixed amount of payload can be carried directly on the database
    ** page.  If the payload is larger than the preset amount then surplus
    ** bytes are stored on overflow pages.  The payload for an entry
    ** and the preceding pointer are combined to form a "Cell".  Each
    ** page has a small header which contains the Ptr(N) pointer and other
    ** information such as the size of key and data.
    **
    ** FORMAT DETAILS
    **
    ** The file is divided into pages.  The first page is called page 1,
    ** the second is page 2, and so forth.  A page number of zero indicates
    ** "no such page".  The page size can be any power of 2 between 512 and 65536.
    ** Each page can be either a btree page, a freelist page, an overflow
    ** page, or a pointer-map page.
    **
    ** The first page is always a btree page.  The first 100 bytes of the first
    ** page contain a special header (the "file header") that describes the file.
    ** The format of the file header is as follows:
    **
    **   OFFSET   SIZE    DESCRIPTION
    **      0      16     Header string: "SQLite format 3\000"
    **     16       2     Page size in bytes.
    **     18       1     File format write version
    **     19       1     File format read version
    **     20       1     Bytes of unused space at the end of each page
    **     21       1     Max embedded payload fraction
    **     22       1     Min embedded payload fraction
    **     23       1     Min leaf payload fraction
    **     24       4     File change counter
    **     28       4     Reserved for future use
    **     32       4     First freelist page
    **     36       4     Number of freelist pages in the file
    **     40      60     15 4-byte meta values passed to higher layers
    **
    **     40       4     Schema cookie
    **     44       4     File format of schema layer
    **     48       4     Size of page cache
    **     52       4     Largest root-page (auto/incr_vacuum)
    **     56       4     1=UTF-8 2=UTF16le 3=UTF16be
    **     60       4     User version
    **     64       4     Incremental vacuum mode
    **     68       4     unused
    **     72       4     unused
    **     76       4     unused
    **
    ** All of the integer values are big-endian (most significant byte first).
    **
    ** The file change counter is incremented when the database is changed
    ** This counter allows other processes to know when the file has changed
    ** and thus when they need to flush their cache.
    **
    ** The max embedded payload fraction is the amount of the total usable
    ** space in a page that can be consumed by a single cell for standard
    ** B-tree (non-LEAFDATA) tables.  A value of 255 means 100%.  The default
    ** is to limit the maximum cell size so that at least 4 cells will fit
    ** on one page.  Thus the default max embedded payload fraction is 64.
    **
    ** If the payload for a cell is larger than the max payload, then extra
    ** payload is spilled to overflow pages.  Once an overflow page is allocated,
    ** as many bytes as possible are moved into the overflow pages without letting
    ** the cell size drop below the min embedded payload fraction.
    **
    ** The min leaf payload fraction is like the min embedded payload fraction
    ** except that it applies to leaf nodes in a LEAFDATA tree.  The maximum
    ** payload fraction for a LEAFDATA tree is always 100% (or 255) and it
    ** not specified in the header.
    **
    ** Each btree pages is divided into three sections:  The header, the
    ** cell pointer array, and the cell content area.  Page 1 also has a 100-byte
    ** file header that occurs before the page header.
    **
    **      |----------------|
    **      | file header    |   100 bytes.  Page 1 only.
    **      |----------------|
    **      | page header    |   8 bytes for leaves.  12 bytes for interior nodes
    **      |----------------|
    **      | cell pointer   |   |  2 bytes per cell.  Sorted order.
    **      | array          |   |  Grows downward
    **      |                |   v
    **      |----------------|
    **      | unallocated    |
    **      | space          |
    **      |----------------|   ^  Grows upwards
    **      | cell content   |   |  Arbitrary order interspersed with freeblocks.
    **      | area           |   |  and free space fragments.
    **      |----------------|
    **
    ** The page headers looks like this:
    **
    **   OFFSET   SIZE     DESCRIPTION
    **      0       1      Flags. 1: intkey, 2: zerodata, 4: leafdata, 8: leaf
    **      1       2      byte offset to the first freeblock
    **      3       2      number of cells on this page
    **      5       2      first byte of the cell content area
    **      7       1      number of fragmented free bytes
    **      8       4      Right child (the Ptr(N) value).  Omitted on leaves.
    **
    ** The flags define the format of this btree page.  The leaf flag means that
    ** this page has no children.  The zerodata flag means that this page carries
    ** only keys and no data.  The intkey flag means that the key is a integer
    ** which is stored in the key size entry of the cell header rather than in
    ** the payload area.
    **
    ** The cell pointer array begins on the first byte after the page header.
    ** The cell pointer array contains zero or more 2-byte numbers which are
    ** offsets from the beginning of the page to the cell content in the cell
    ** content area.  The cell pointers occur in sorted order.  The system strives
    ** to keep free space after the last cell pointer so that new cells can
    ** be easily added without having to defragment the page.
    **
    ** Cell content is stored at the very end of the page and grows toward the
    ** beginning of the page.
    **
    ** Unused space within the cell content area is collected into a linked list of
    ** freeblocks.  Each freeblock is at least 4 bytes in size.  The byte offset
    ** to the first freeblock is given in the header.  Freeblocks occur in
    ** increasing order.  Because a freeblock must be at least 4 bytes in size,
    ** any group of 3 or fewer unused bytes in the cell content area cannot
    ** exist on the freeblock chain.  A group of 3 or fewer free bytes is called
    ** a fragment.  The total number of bytes in all fragments is recorded.
    ** in the page header at offset 7.
    **
    **    SIZE    DESCRIPTION
    **      2     Byte offset of the next freeblock
    **      2     Bytes in this freeblock
    **
    ** Cells are of variable length.  Cells are stored in the cell content area at
    ** the end of the page.  Pointers to the cells are in the cell pointer array
    ** that immediately follows the page header.  Cells is not necessarily
    ** contiguous or in order, but cell pointers are contiguous and in order.
    **
    ** Cell content makes use of variable length integers.  A variable
    ** length integer is 1 to 9 bytes where the lower 7 bits of each
    ** byte are used.  The integer consists of all bytes that have bit 8 set and
    ** the first byte with bit 8 clear.  The most significant byte of the integer
    ** appears first.  A variable-length integer may not be more than 9 bytes long.
    ** As a special case, all 8 bytes of the 9th byte are used as data.  This
    ** allows a 64-bit integer to be encoded in 9 bytes.
    **
    **    0x00                      becomes  0x00000000
    **    0x7f                      becomes  0x0000007f
    **    0x81 0x00                 becomes  0x00000080
    **    0x82 0x00                 becomes  0x00000100
    **    0x80 0x7f                 becomes  0x0000007f
    **    0x8a 0x91 0xd1 0xac 0x78  becomes  0x12345678
    **    0x81 0x81 0x81 0x81 0x01  becomes  0x10204081
    **
    ** Variable length integers are used for rowids and to hold the number of
    ** bytes of key and data in a btree cell.
    **
    ** The content of a cell looks like this:
    **
    **    SIZE    DESCRIPTION
    **      4     Page number of the left child. Omitted if leaf flag is set.
    **     var    Number of bytes of data. Omitted if the zerodata flag is set.
    **     var    Number of bytes of key. Or the key itself if intkey flag is set.
    **      *     Payload
    **      4     First page of the overflow chain.  Omitted if no overflow
    **
    ** Overflow pages form a linked list.  Each page except the last is completely
    ** filled with data (pagesize - 4 bytes).  The last page can have as little
    ** as 1 byte of data.
    **
    **    SIZE    DESCRIPTION
    **      4     Page number of next overflow page
    **      *     Data
    **
    ** Freelist pages come in two subtypes: trunk pages and leaf pages.  The
    ** file header points to the first in a linked list of trunk page.  Each trunk
    ** page points to multiple leaf pages.  The content of a leaf page is
    ** unspecified.  A trunk page looks like this:
    **
    **    SIZE    DESCRIPTION
    **      4     Page number of next trunk page
    **      4     Number of leaf pointers on this page
    **      *     zero or more pages numbers of leaves
    *************************************************************************
    **  Included in SQLite3 port to C#-SQLite;  2008 Noah B Hart
    **  C#-SQLite is an independent reimplementation of the SQLite software library
    **
    **  SQLITE_SOURCE_ID: 2011-05-19 13:26:54 ed1da510a239ea767a01dc332b667119fa3c908e
    **
    *************************************************************************
    *///#include "sqliteInt.h"
		///<summary>
		///The following value is the maximum cell size assuming a maximum page
		/// size give above.
		///
		///</summary>
		//#define MX_CELL_SIZE(pBt)  ((int)(pBt->pageSize-8))
		static int MX_CELL_SIZE(BtShared pBt) {
			return (int)(pBt.pageSize-8);
		}
		///<summary>
		///The maximum number of cells on a single page of the database.  This
		/// assumes a minimum cell size of 6 bytes  (4 bytes for the cell itself
		/// plus 2 bytes for the index to the cell in the page header).  Such
		/// small cells will be rare, but they are possible.
		///
		///</summary>
		//#define MX_CELL(pBt) ((pBt.pageSize-8)/6)
		static int MX_CELL(BtShared pBt) {
			return ((int)(pBt.pageSize-8)/6);
		}
		/* Forward declarations *///typedef struct MemPage MemPage;
		//typedef struct BtLock BtLock;
		/*
    ** This is a magic string that appears at the beginning of every
    ** SQLite database in order to identify the file as a real database.
    **
    ** You can change this value at compile-time by specifying a
    ** -DSQLITE_FILE_HEADER="..." on the compiler command-line.  The
    ** header must be exactly 16 bytes including the zero-terminator so
    ** the string itself should be 15 characters long.  If you change
    ** the header, then your custom library will not be able to read
    ** databases generated by the standard tools and the standard tools
    ** will not be able to read databases created by your custom library.
    */
		#if !SQLITE_FILE_HEADER
		const string SQLITE_FILE_HEADER="SQLite format 3\0";
		#endif
		///<summary>
		/// Page type flags.  An ORed combination of these flags appear as the
		/// first byte of on-disk image of every BTree page.
		///</summary>
		const byte PTF_INTKEY=0x01;
		const byte PTF_ZERODATA=0x02;
		const byte PTF_LEAFDATA=0x04;
		const byte PTF_LEAF=0x08;
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
		public struct _OvflCell {
			/* Cells that will not fit on aData[] */public u8[] pCell;
			///<summary>
			///Pointers to the body of the overflow cell
			///</summary>
			public u16 idx;
			/* Insert this cell before idx-th non-overflow cell */public _OvflCell Copy() {
				_OvflCell cp=new _OvflCell();
				if(pCell!=null) {
					cp.pCell=sqlite3Malloc(pCell.Length);
					Buffer.BlockCopy(pCell,0,cp.pCell,0,pCell.Length);
				}
				cp.idx=idx;
				return cp;
			}
		}
		
		///<summary>
		/// The in-memory image of a disk page has the auxiliary information appended
		/// to the end.  EXTRA_SIZE is the number of bytes of space needed to hold
		/// that extra information.
		///
		///</summary>
		const int EXTRA_SIZE=0;
		// No used in C#, since we use create a class; was MemPage.Length;
		/*
    ** A linked list of the following structures is stored at BtShared.pLock.
    ** Locks are added (or upgraded from READ_LOCK to WRITE_LOCK) when a cursor 
    ** is opened on the table with root page BtShared.iTable. Locks are removed
    ** from this list when a transaction is committed or rolled back, or when
    ** a btree handle is closed.
    */public class BtLock {
			Btree pBtree;
			/* Btree handle holding this lock */Pgno iTable;
			/* Root page of table */u8 eLock;
			/* READ_LOCK or WRITE_LOCK */BtLock pNext;
		/* Next in BtShared.pLock list */};

		///<summary>
		///Candidate values for BtLock.eLock
		///</summary>
		//#define READ_LOCK     1
		//#define WRITE_LOCK    2
		const int READ_LOCK=1;
		const int WRITE_LOCK=2;
		
		///<summary>
		/// Btree.inTrans may take one of the following values.
		///
		/// If the shared-data extension is enabled, there may be multiple users
		/// of the Btree structure. At most one of these may open a write transaction,
		/// but any number may have active read transactions.
		///
		///</summary>
		public const byte TRANS_NONE=0;
		public const byte TRANS_READ=1;
		public const byte TRANS_WRITE=2;
		/*
    ** An instance of this object represents a single database file.
    **
    ** A single database file can be in use as the same time by two
    ** or more database connections.  When two or more connections are
    ** sharing the same database file, each connection has it own
    ** private Btree object for the file and each of those Btrees points
    ** to this one BtShared object.  BtShared.nRef is the number of
    ** connections currently sharing this database file.
    **
    ** Fields in this structure are accessed under the BtShared.mutex
    ** mutex, except for nRef and pNext which are accessed under the
    ** global SQLITE_MUTEX_STATIC_MASTER mutex.  The pPager field
    ** may not be modified once it is initially set as long as nRef>0.
    ** The pSchema field may be set once under BtShared.mutex and
    ** thereafter is unchanged as long as nRef>0.
    **
    ** isPending:
    **
    **   If a BtShared client fails to obtain a write-lock on a database
    **   table (because there exists one or more read-locks on the table),
    **   the shared-cache enters 'pending-lock' state and isPending is
    **   set to true.
    **
    **   The shared-cache leaves the 'pending lock' state when either of
    **   the following occur:
    **
    **     1) The current writer (BtShared.pWriter) concludes its transaction, OR
    **     2) The number of locks held by other connections drops to zero.
    **
    **   while in the 'pending-lock' state, no connection may start a new
    **   transaction.
    **
    **   This feature is included to help prevent writer-starvation.
    */public class BtShared {
			public Pager pPager;
			/* The page cache */public sqlite3 db;
			/* Database connection currently using this Btree */public BtCursor pCursor;
			/* A list of all open cursors */public MemPage pPage1;
			/* First page of the database */public bool readOnly;
			/* True if the underlying file is readonly */public bool pageSizeFixed;
			/* True if the page size can no longer be changed */public bool secureDelete;
			/* True if secure_delete is enabled */public bool initiallyEmpty;
			/* Database is empty at start of transaction */public u8 openFlags;
			/* Flags to sqlite3BtreeOpen() */
			#if !SQLITE_OMIT_AUTOVACUUM
			public bool autoVacuum;
			/* True if auto-vacuum is enabled */public bool incrVacuum;
			/* True if incr-vacuum is enabled */
			#endif
			public u8 inTransaction;
			/* Transaction state */public bool doNotUseWAL;
			/* If true, do not open write-ahead-log file */public u16 maxLocal;
			/* Maximum local payload in non-LEAFDATA tables */public u16 minLocal;
			/* Minimum local payload in non-LEAFDATA tables */public u16 maxLeaf;
			/* Maximum local payload in a LEAFDATA table */public u16 minLeaf;
			/* Minimum local payload in a LEAFDATA table */public u32 pageSize;
			/* Total number of bytes on a page */public u32 usableSize;
			/* Number of usable bytes on each page */public int nTransaction;
			/* Number of open transactions (read + write) */public Pgno nPage;
			/* Number of pages in the database */public Schema pSchema;
			/* Pointer to space allocated by sqlite3BtreeSchema() */public dxFreeSchema xFreeSchema;
			/* Destructor for BtShared.pSchema */public sqlite3_mutex mutex;
			/* Non-recursive mutex required to access this object */public Bitvec pHasContent;
			/* Set of pages moved to free-list this transaction */
			#if !SQLITE_OMIT_SHARED_CACHE
																																																																		public int nRef;                /* Number of references to this structure */
public BtShared pNext;          /* Next on a list of sharable BtShared structs */
public BtLock pLock;            /* List of locks held on this shared-btree struct */
public Btree pWriter;           /* Btree with currently open write transaction */
public u8 isExclusive;          /* True if pWriter has an EXCLUSIVE lock on the db */
public u8 isPending;            /* If waiting for read-locks to clear */
#endif
			public byte[] pTmpSpace;
			/* BtShared.pageSize bytes of space for tmp use */public///<summary>
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
			int btreeSetHasContent(Pgno pgno) {
				int rc=SQLITE_OK;
				if(null==this.pHasContent) {
					Debug.Assert(pgno<=this.nPage);
					this.pHasContent=sqlite3BitvecCreate(this.nPage);
					//if ( null == pBt.pHasContent )
					//{
					//  rc = SQLITE_NOMEM;
					//}
				}
				if(rc==SQLITE_OK&&pgno<=sqlite3BitvecSize(this.pHasContent)) {
					rc=sqlite3BitvecSet(this.pHasContent,pgno);
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
			bool btreeGetHasContent(Pgno pgno) {
				Bitvec p=this.pHasContent;
				return (p!=null&&(pgno>sqlite3BitvecSize(p)||sqlite3BitvecTest(p,pgno)!=0));
			}
			public///<summary>
			/// Clear (destroy) the BtShared.pHasContent bitvec. This should be
			/// invoked at the conclusion of each write-transaction.
			///</summary>
			void btreeClearHasContent() {
				sqlite3BitvecDestroy(ref this.pHasContent);
				this.pHasContent=null;
			}
			public void invalidateAllOverflowCache() {
			}
			public int saveAllCursors(Pgno iRoot,BtCursor pExcept) {
				BtCursor p;
				Debug.Assert(sqlite3_mutex_held(this.mutex));
				Debug.Assert(pExcept==null||pExcept.pBt==this);
				for(p=this.pCursor;p!=null;p=p.pNext) {
					if(p!=pExcept&&(0==iRoot||p.pgnoRoot==iRoot)&&p.eState==CURSOR_VALID) {
						int rc=p.saveCursorPosition();
						if(SQLITE_OK!=rc) {
							return rc;
						}
					}
				}
				return SQLITE_OK;
			}
			public Pgno ptrmapPageno(Pgno pgno) {
				int nPagesPerMapPage;
				Pgno iPtrMap,ret;
				Debug.Assert(sqlite3_mutex_held(this.mutex));
				if(pgno<2)
					return 0;
				nPagesPerMapPage=(int)(this.usableSize/5+1);
				iPtrMap=(Pgno)((pgno-2)/nPagesPerMapPage);
				ret=(Pgno)(iPtrMap*nPagesPerMapPage)+2;
				if(ret==PENDING_BYTE_PAGE(this)) {
					ret++;
				}
				return ret;
			}
			public void ptrmapPut(Pgno key,u8 eType,Pgno parent,ref int pRC) {
				PgHdr pDbPage=new PgHdr();
				/* The pointer map page */u8[] pPtrmap;
				/* The pointer map data */Pgno iPtrmap;
				/* The pointer map page number */int offset;
				/* Offset in pointer map page */int rc;
				/* Return code from subfunctions */if(pRC!=0)
					return;
				Debug.Assert(sqlite3_mutex_held(this.mutex));
				/* The master-journal page number must never be used as a pointer map page */Debug.Assert(false==PTRMAP_ISPAGE(this,PENDING_BYTE_PAGE(this)));
				Debug.Assert(this.autoVacuum);
				if(key==0) {
					pRC=SQLITE_CORRUPT_BKPT();
					return;
				}
				iPtrmap=PTRMAP_PAGENO(this,key);
				rc=this.pPager.sqlite3PagerGet(iPtrmap,ref pDbPage);
				if(rc!=SQLITE_OK) {
					pRC=rc;
					return;
				}
				offset=(int)PTRMAP_PTROFFSET(iPtrmap,key);
				if(offset<0) {
					pRC=SQLITE_CORRUPT_BKPT();
					goto ptrmap_exit;
				}
				Debug.Assert(offset<=(int)this.usableSize-5);
				pPtrmap=sqlite3PagerGetData(pDbPage);
				if(eType!=pPtrmap[offset]||Converter.sqlite3Get4byte(pPtrmap,offset+1)!=parent) {
					TRACE("PTRMAP_UPDATE: %d->(%d,%d)\n",key,eType,parent);
					pRC=rc=sqlite3PagerWrite(pDbPage);
					if(rc==SQLITE_OK) {
						pPtrmap[offset]=eType;
						Converter.sqlite3Put4byte(pPtrmap,offset+1,parent);
					}
				}
				ptrmap_exit:
				sqlite3PagerUnref(pDbPage);
			}
			public int ptrmapGet(Pgno key,ref u8 pEType,ref Pgno pPgno) {
				PgHdr pDbPage=new PgHdr();
				/* The pointer map page */int iPtrmap;
				/* Pointer map page index */u8[] pPtrmap;
				/* Pointer map page data */int offset;
				/* Offset of entry in pointer map */int rc;
				Debug.Assert(sqlite3_mutex_held(this.mutex));
				iPtrmap=(int)PTRMAP_PAGENO(this,key);
				rc=this.pPager.sqlite3PagerGet((u32)iPtrmap,ref pDbPage);
				if(rc!=0) {
					return rc;
				}
				pPtrmap=sqlite3PagerGetData(pDbPage);
				offset=(int)PTRMAP_PTROFFSET((u32)iPtrmap,key);
				if(offset<0) {
					sqlite3PagerUnref(pDbPage);
					return SQLITE_CORRUPT_BKPT();
				}
				Debug.Assert(offset<=(int)this.usableSize-5);
				// Under C# pEType will always exist. No need to test; //
				//Debug.Assert( pEType != 0 );
				pEType=pPtrmap[offset];
				// Under C# pPgno will always exist. No need to test; //
				//if ( pPgno != 0 )
				pPgno=Converter.sqlite3Get4byte(pPtrmap,offset+1);
				sqlite3PagerUnref(pDbPage);
				if(pEType<1||pEType>5)
					return SQLITE_CORRUPT_BKPT();
				return SQLITE_OK;
			}
			public int btreeGetPage(/* The btree */Pgno pgno,/* Number of the page to fetch */ref MemPage ppPage,/* Return the page in this parameter */int noContent/* Do not load page content if true */) {
				int rc;
				DbPage pDbPage=null;
				Debug.Assert(sqlite3_mutex_held(this.mutex));
				rc=this.pPager.sqlite3PagerAcquire(pgno,ref pDbPage,(u8)noContent);
				if(rc!=0)
					return rc;
				ppPage=pDbPage.btreePageFromDbPage(pgno,this);
				return SQLITE_OK;
			}
			public MemPage btreePageLookup(Pgno pgno) {
				DbPage pDbPage;
				Debug.Assert(sqlite3_mutex_held(this.mutex));
				pDbPage=this.pPager.sqlite3PagerLookup(pgno);
				if(pDbPage) {
					return pDbPage.btreePageFromDbPage(pgno,this);
				}
				return null;
			}
			public Pgno btreePagecount() {
				return this.nPage;
			}
		}
		///<summary>
		/// An instance of the following structure is used to hold information
		/// about a cell.  The parseCellPtr() function fills in this structure
		/// based on information extract from the raw disk page.
		///
		///</summary>
		//typedef struct CellInfo CellInfo;
		public struct CellInfo {
			public int iCell;
			/* Offset to start of cell content -- Needed for C# */public byte[] pCell;
			/* Pointer to the start of cell content */public i64 nKey;
			/* The key for INTKEY tables, or number of bytes in key */public u32 nData;
			/* Number of bytes of data */public u32 nPayload;
			/* Total amount of payload */public u16 nHeader;
			/* Size of the cell content header in bytes */public u16 nLocal;
			/* Amount of payload held locally */public u16 iOverflow;
			///<summary>
			///Offset to overflow page number.  Zero if no overflow
			///</summary>
			public u16 nSize;
			/* Size of the cell content on the main b-tree page */public bool Equals(CellInfo ci) {
				if(ci.iCell>=ci.pCell.Length||iCell>=this.pCell.Length)
					return false;
				if(ci.pCell[ci.iCell]!=this.pCell[iCell])
					return false;
				if(ci.nKey!=this.nKey||ci.nData!=this.nData||ci.nPayload!=this.nPayload)
					return false;
				if(ci.nHeader!=this.nHeader||ci.nLocal!=this.nLocal)
					return false;
				if(ci.iOverflow!=this.iOverflow||ci.nSize!=this.nSize)
					return false;
				return true;
			}
		}
	  
        
        /*
    ** Potential values for BtCursor.eState.
    **
    ** CURSOR_VALID:
    **   VdbeCursor points to a valid entry. getPayload() etc. may be called.
    **
    ** CURSOR_INVALID:
    **   VdbeCursor does not point to a valid entry. This can happen (for example)
    **   because the table is empty or because BtreeCursorFirst() has not been
    **   called.
    **
    ** CURSOR_REQUIRESEEK:
    **   The table that this cursor was opened on still exists, but has been
    **   modified since the cursor was last used. The cursor position is saved
    **   in variables BtCursor.pKey and BtCursor.nKey. When a cursor is in
    **   this state, restoreCursorPosition() can be called to attempt to
    **   seek the cursor to the saved position.
    **
    ** CURSOR_FAULT:
    **   A unrecoverable error (an I/O error or a malloc failure) has occurred
    **   on a different connection that shares the BtShared cache with this
    **   cursor.  The error has left the cache in an inconsistent state.
    **   Do nothing else with this cursor.  Any attempt to use the cursor
    **   should return the error code stored in BtCursor.skip
    */const int CURSOR_INVALID=0;
		const int CURSOR_VALID=1;
		const int CURSOR_REQUIRESEEK=2;
		const int CURSOR_FAULT=3;
		///<summary>
		/// The database page the PENDING_BYTE occupies. This page is never used.
		///
		///</summary>
		//# define PENDING_BYTE_PAGE(pBt) PAGER_MJ_PGNO(pBt)
		// TODO -- Convert PENDING_BYTE_PAGE to inline
		static u32 PENDING_BYTE_PAGE(BtShared pBt) {
			return (u32)PAGER_MJ_PGNO(pBt.pPager);
		}
		///<summary>
		/// These macros define the location of the pointer-map entry for a
		/// database page. The first argument to each is the number of usable
		/// bytes on each page of the database (often 1024). The second is the
		/// page number to look up in the pointer map.
		///
		/// PTRMAP_PAGENO returns the database page number of the pointer-map
		/// page that stores the required pointer. PTRMAP_PTROFFSET returns
		/// the offset of the requested map entry.
		///
		/// If the pgno argument passed to PTRMAP_PAGENO is a pointer-map page,
		/// then pgno is returned. So (pgno==PTRMAP_PAGENO(pgsz, pgno)) can be
		/// used to test if pgno is a pointer-map page. PTRMAP_ISPAGE implements
		/// this test.
		///
		///</summary>
		//#define PTRMAP_PAGENO(pBt, pgno) ptrmapPageno(pBt, pgno)
		static Pgno PTRMAP_PAGENO(BtShared pBt,Pgno pgno) {
			return pBt.ptrmapPageno(pgno);
		}
		//#define PTRMAP_PTROFFSET(pgptrmap, pgno) (5*(pgno-pgptrmap-1))
		static u32 PTRMAP_PTROFFSET(u32 pgptrmap,u32 pgno) {
			return (5*(pgno-pgptrmap-1));
		}
		//#define PTRMAP_ISPAGE(pBt, pgno) (PTRMAP_PAGENO((pBt),(pgno))==(pgno))
		static bool PTRMAP_ISPAGE(BtShared pBt,u32 pgno) {
			return (PTRMAP_PAGENO((pBt),(pgno))==(pgno));
		}
		/*
    ** The pointer map is a lookup table that identifies the parent page for
    ** each child page in the database file.  The parent page is the page that
    ** contains a pointer to the child.  Every page in the database contains
    ** 0 or 1 parent pages.  (In this context 'database page' refers
    ** to any page that is not part of the pointer map itself.)  Each pointer map
    ** entry consists of a single byte 'type' and a 4 byte parent page number.
    ** The PTRMAP_XXX identifiers below are the valid types.
    **
    ** The purpose of the pointer map is to facility moving pages from one
    ** position in the file to another as part of autovacuum.  When a page
    ** is moved, the pointer in its parent must be updated to point to the
    ** new location.  The pointer map is used to locate the parent page quickly.
    **
    ** PTRMAP_ROOTPAGE: The database page is a root-page. The page-number is not
    **                  used in this case.
    **
    ** PTRMAP_FREEPAGE: The database page is an unused (free) page. The page-number
    **                  is not used in this case.
    **
    ** PTRMAP_OVERFLOW1: The database page is the first page in a list of
    **                   overflow pages. The page number identifies the page that
    **                   contains the cell with a pointer to this overflow page.
    **
    ** PTRMAP_OVERFLOW2: The database page is the second or later page in a list of
    **                   overflow pages. The page-number identifies the previous
    **                   page in the overflow page list.
    **
    ** PTRMAP_BTREE: The database page is a non-root btree page. The page number
    **               identifies the parent page in the btree.
    *///#define PTRMAP_ROOTPAGE 1
		//#define PTRMAP_FREEPAGE 2
		//#define PTRMAP_OVERFLOW1 3
		//#define PTRMAP_OVERFLOW2 4
		//#define PTRMAP_BTREE 5
		const int PTRMAP_ROOTPAGE=1;
		const int PTRMAP_FREEPAGE=2;
		const int PTRMAP_OVERFLOW1=3;
		const int PTRMAP_OVERFLOW2=4;
		const int PTRMAP_BTREE=5;
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
		static void btreeIntegrity(Btree p) {
		}
		#endif
		/*
** The ISAUTOVACUUM macro is used within balance_nonroot() to determine
** if the database supports auto-vacuum or not. Because it is used
** within an expression that is an argument to another macro
** (sqliteMallocRaw), it is not possible to use conditional compilation.
** So, this macro is defined instead.
*/
		#if !SQLITE_OMIT_AUTOVACUUM
		//#define ISAUTOVACUUM (pBt.autoVacuum)
		#else
																																												//define ISAUTOVACUUM 0
public static bool ISAUTOVACUUM =false;
#endif
		///<summary>
		/// This structure is passed around through all the sanity checking routines
		/// in order to keep track of some global state information.
		///</summary>
		//typedef struct IntegrityCk IntegrityCk;
		public class IntegrityCk {
			public BtShared pBt;
			/* The tree being checked out */public Pager pPager;
			/* The associated pager.  Also accessible by pBt.pPager */public Pgno nPage;
			/* Number of pages in the database */public int[] anRef;
			/* Number of times each page is referenced */public int mxErr;
			/* Stop accumulating errors when this reaches zero */public int nErr;
			/* Number of messages written to zErrMsg so far *///public int mallocFailed;  /* A memory allocation error has occurred */
			public StrAccum errMsg=new StrAccum(100);
			/* Accumulate the error message text here */public void checkAppendMsg(StringBuilder zMsg1,string zFormat,params object[] ap) {
				if(0==this.mxErr)
					return;
				//va_list ap;
				lock(lock_va_list) {
					this.mxErr--;
					this.nErr++;
					va_start(ap,zFormat);
					if(this.errMsg.zText.Length!=0) {
						sqlite3StrAccumAppend(this.errMsg,"\n",1);
					}
					if(zMsg1.Length>0) {
						sqlite3StrAccumAppend(this.errMsg,zMsg1.ToString(),-1);
					}
					sqlite3VXPrintf(this.errMsg,1,zFormat,ap);
					va_end(ref ap);
				}
				//if( pCheck.errMsg.mallocFailed ){
				//  pCheck.mallocFailed = 1;
				//}
			}
			public int checkRef(Pgno iPage,string zContext) {
				if(iPage==0)
					return 1;
				if(iPage>this.nPage) {
					this.checkAppendMsg(zContext,"invalid page number %d",iPage);
					return 1;
				}
				if(this.anRef[iPage]==1) {
					this.checkAppendMsg(zContext,"2nd reference to page %d",iPage);
					return 1;
				}
				return ((this.anRef[iPage]++)>1)?1:0;
			}
			public int checkTreePage(/* Context for the sanity check */int iPage,/* Page number of the page to check */string zParentContext,/* Parent context */ref i64 pnParentMinKey,ref i64 pnParentMaxKey,object _pnParentMinKey,/* C# Needed to determine if content passed*/object _pnParentMaxKey/* C# Needed to determine if content passed*/) {
				MemPage pPage=new MemPage();
				int i,rc,depth,d2,pgno,cnt;
				int hdr,cellStart;
				int nCell;
				u8[] data;
				BtShared pBt;
				int usableSize;
				StringBuilder zContext=new StringBuilder(100);
				byte[] hit=null;
				i64 nMinKey=0;
				i64 nMaxKey=0;
				sqlite3_snprintf(200,zContext,"Page %d: ",iPage);
				/* Check that the page exists
  */pBt=this.pBt;
				usableSize=(int)pBt.usableSize;
				if(iPage==0)
					return 0;
				if(this.checkRef((u32)iPage,zParentContext)!=0)
					return 0;
				if((rc=pBt.btreeGetPage((Pgno)iPage,ref pPage,0))!=0) {
					this.checkAppendMsg(zContext.ToString(),"unable to get the page. error code=%d",rc);
					return 0;
				}
				/* Clear MemPage.isInit to make sure the corruption detection code in
  ** btreeInitPage() is executed.  */pPage.isInit=0;
				if((rc=pPage.btreeInitPage())!=0) {
					Debug.Assert(rc==SQLITE_CORRUPT);
					/* The only possible error from InitPage */this.checkAppendMsg(zContext.ToString(),"btreeInitPage() returns error code %d",rc);
					releasePage(pPage);
					return 0;
				}
				/* Check out all the cells.
  */depth=0;
				for(i=0;i<pPage.nCell&&this.mxErr!=0;i++) {
					u8[] pCell;
					u32 sz;
					CellInfo info=new CellInfo();
					/* Check payload overflow pages
    */sqlite3_snprintf(200,zContext,"On tree page %d cell %d: ",iPage,i);
					int iCell=pPage.findCell(i);
					//pCell = findCell( pPage, i );
					pCell=pPage.aData;
					pPage.btreeParseCellPtr(iCell,ref info);
					//btreeParseCellPtr( pPage, pCell, info );
					sz=info.nData;
					if(0==pPage.intKey)
						sz+=(u32)info.nKey;
					/* For intKey pages, check that the keys are in order.
    */else
						if(i==0)
							nMinKey=nMaxKey=info.nKey;
						else {
							if(info.nKey<=nMaxKey) {
								this.checkAppendMsg(zContext.ToString(),"Rowid %lld out of order (previous was %lld)",info.nKey,nMaxKey);
							}
							nMaxKey=info.nKey;
						}
					Debug.Assert(sz==info.nPayload);
					if((sz>info.nLocal)//&& (pCell[info.iOverflow]<=&pPage.aData[pBt.usableSize])
					) {
						int nPage=(int)(sz-info.nLocal+usableSize-5)/(usableSize-4);
						Pgno pgnoOvfl=Converter.sqlite3Get4byte(pCell,iCell,info.iOverflow);
						#if !SQLITE_OMIT_AUTOVACUUM
						if(pBt.autoVacuum) {
							this.checkPtrmap(pgnoOvfl,PTRMAP_OVERFLOW1,(u32)iPage,zContext.ToString());
						}
						#endif
						this.checkList(0,(int)pgnoOvfl,nPage,zContext.ToString());
					}
					/* Check sanity of left child page.
    */if(0==pPage.leaf) {
						pgno=(int)Converter.sqlite3Get4byte(pCell,iCell);
						//sqlite3Get4byte( pCell );
						#if !SQLITE_OMIT_AUTOVACUUM
						if(pBt.autoVacuum) {
							this.checkPtrmap((u32)pgno,PTRMAP_BTREE,(u32)iPage,zContext.ToString());
						}
						#endif
						if(i==0)
							d2=this.checkTreePage(pgno,zContext.ToString(),ref nMinKey,ref refNULL,this,null);
						else
							d2=this.checkTreePage(pgno,zContext.ToString(),ref nMinKey,ref nMaxKey,this,this);
						if(i>0&&d2!=depth) {
							this.checkAppendMsg(zContext,"Child page depth differs");
						}
						depth=d2;
					}
				}
				if(0==pPage.leaf) {
					pgno=(int)Converter.sqlite3Get4byte(pPage.aData,pPage.hdrOffset+8);
					sqlite3_snprintf(200,zContext,"On page %d at right child: ",iPage);
					#if !SQLITE_OMIT_AUTOVACUUM
					if(pBt.autoVacuum) {
						this.checkPtrmap((u32)pgno,PTRMAP_BTREE,(u32)iPage,zContext.ToString());
					}
					#endif
					//    checkTreePage(pCheck, pgno, zContext, NULL, !pPage->nCell ? NULL : &nMaxKey);
					if(0==pPage.nCell)
						this.checkTreePage(pgno,zContext.ToString(),ref refNULL,ref refNULL,null,null);
					else
						this.checkTreePage(pgno,zContext.ToString(),ref refNULL,ref nMaxKey,null,this);
				}
				/* For intKey leaf pages, check that the min/max keys are in order
  ** with any left/parent/right pages.
  */if(pPage.leaf!=0&&pPage.intKey!=0) {
					/* if we are a left child page */if(_pnParentMinKey!=null) {
						/* if we are the left most child page */if(_pnParentMaxKey==null) {
							if(nMaxKey>pnParentMinKey) {
								this.checkAppendMsg(zContext,"Rowid %lld out of order (max larger than parent min of %lld)",nMaxKey,pnParentMinKey);
							}
						}
						else {
							if(nMinKey<=pnParentMinKey) {
								this.checkAppendMsg(zContext,"Rowid %lld out of order (min less than parent min of %lld)",nMinKey,pnParentMinKey);
							}
							if(nMaxKey>pnParentMaxKey) {
								this.checkAppendMsg(zContext,"Rowid %lld out of order (max larger than parent max of %lld)",nMaxKey,pnParentMaxKey);
							}
							pnParentMinKey=nMaxKey;
						}
						/* else if we're a right child page */}
					else
						if(_pnParentMaxKey!=null) {
							if(nMinKey<=pnParentMaxKey) {
								this.checkAppendMsg(zContext,"Rowid %lld out of order (min less than parent max of %lld)",nMinKey,pnParentMaxKey);
							}
						}
				}
				/* Check for complete coverage of the page
  */data=pPage.aData;
				hdr=pPage.hdrOffset;
				hit=sqlite3Malloc(pBt.pageSize);
				//if( hit==null ){
				//  pCheck.mallocFailed = 1;
				//}else
				{
					int contentOffset=get2byteNotZero(data,hdr+5);
					Debug.Assert(contentOffset<=usableSize);
					/* Enforced by btreeInitPage() */Array.Clear(hit,contentOffset,usableSize-contentOffset);
					//memset(hit+contentOffset, 0, usableSize-contentOffset);
					for(int iLoop=contentOffset-1;iLoop>=0;iLoop--)
						hit[iLoop]=1;
					//memset(hit, 1, contentOffset);
					nCell=get2byte(data,hdr+3);
					cellStart=hdr+12-4*pPage.leaf;
					for(i=0;i<nCell;i++) {
						int pc=get2byte(data,cellStart+i*2);
						u32 size=65536;
						int j;
						if(pc<=usableSize-4) {
							size=pPage.cellSizePtr(data,pc);
						}
						if((int)(pc+size-1)>=usableSize) {
							this.checkAppendMsg("","Corruption detected in cell %d on page %d",i,iPage);
						}
						else {
							for(j=(int)(pc+size-1);j>=pc;j--)
								hit[j]++;
						}
					}
					i=get2byte(data,hdr+1);
					while(i>0) {
						int size,j;
						Debug.Assert(i<=usableSize-4);
						/* Enforced by btreeInitPage() */size=get2byte(data,i+2);
						Debug.Assert(i+size<=usableSize);
						/* Enforced by btreeInitPage() */for(j=i+size-1;j>=i;j--)
							hit[j]++;
						j=get2byte(data,i);
						Debug.Assert(j==0||j>i+size);
						/* Enforced by btreeInitPage() */Debug.Assert(j<=usableSize-4);
						/* Enforced by btreeInitPage() */i=j;
					}
					for(i=cnt=0;i<usableSize;i++) {
						if(hit[i]==0) {
							cnt++;
						}
						else
							if(hit[i]>1) {
								this.checkAppendMsg("","Multiple uses for byte %d of page %d",i,iPage);
								break;
							}
					}
					if(cnt!=data[hdr+7]) {
						this.checkAppendMsg("","Fragmentation of %d bytes reported as %d on page %d",cnt,data[hdr+7],iPage);
					}
				}
				sqlite3PageFree(ref hit);
				releasePage(pPage);
				return depth+1;
			}
			public void checkList(/* Integrity checking context */int isFreeList,/* True for a freelist.  False for overflow page list */int iPage,/* Page number for first page in the list */int N,/* Expected number of pages in the list */string zContext/* Context for error messages */) {
				int i;
				int expected=N;
				int iFirst=iPage;
				while(N-->0&&this.mxErr!=0) {
					PgHdr pOvflPage=new PgHdr();
					byte[] pOvflData;
					if(iPage<1) {
						this.checkAppendMsg(zContext,"%d of %d pages missing from overflow list starting at %d",N+1,expected,iFirst);
						break;
					}
					if(this.checkRef((u32)iPage,zContext)!=0)
						break;
					if(this.pPager.sqlite3PagerGet((Pgno)iPage,ref pOvflPage)!=0) {
						this.checkAppendMsg(zContext,"failed to get page %d",iPage);
						break;
					}
					pOvflData=sqlite3PagerGetData(pOvflPage);
					if(isFreeList!=0) {
						int n=(int)Converter.sqlite3Get4byte(pOvflData,4);
						#if !SQLITE_OMIT_AUTOVACUUM
						if(this.pBt.autoVacuum) {
							this.checkPtrmap((u32)iPage,PTRMAP_FREEPAGE,0,zContext);
						}
						#endif
						if(n>(int)this.pBt.usableSize/4-2) {
							this.checkAppendMsg(zContext,"freelist leaf count too big on page %d",iPage);
							N--;
						}
						else {
							for(i=0;i<n;i++) {
								Pgno iFreePage=Converter.sqlite3Get4byte(pOvflData,8+i*4);
								#if !SQLITE_OMIT_AUTOVACUUM
								if(this.pBt.autoVacuum) {
									this.checkPtrmap(iFreePage,PTRMAP_FREEPAGE,0,zContext);
								}
								#endif
								this.checkRef(iFreePage,zContext);
							}
							N-=n;
						}
					}
					#if !SQLITE_OMIT_AUTOVACUUM
					else {
						/* If this database supports auto-vacuum and iPage is not the last
      ** page in this overflow list, check that the pointer-map entry for
      ** the following page matches iPage.
      */if(this.pBt.autoVacuum&&N>0) {
							i=(int)Converter.sqlite3Get4byte(pOvflData);
							this.checkPtrmap((u32)i,PTRMAP_OVERFLOW2,(u32)iPage,zContext);
						}
					}
					#endif
					iPage=(int)Converter.sqlite3Get4byte(pOvflData);
					sqlite3PagerUnref(pOvflPage);
				}
			}
			public void checkPtrmap(/* Integrity check context */Pgno iChild,/* Child page number */u8 eType,/* Expected pointer map type */Pgno iParent,/* Expected pointer map parent page number */string zContext/* Context description (used for error msg) */) {
				int rc;
				u8 ePtrmapType=0;
				Pgno iPtrmapParent=0;
				rc=this.pBt.ptrmapGet(iChild,ref ePtrmapType,ref iPtrmapParent);
				if(rc!=SQLITE_OK) {
					//if( rc==SQLITE_NOMEM || rc==SQLITE_IOERR_NOMEM ) pCheck.mallocFailed = 1;
					this.checkAppendMsg(zContext,"Failed to read ptrmap key=%d",iChild);
					return;
				}
				if(ePtrmapType!=eType||iPtrmapParent!=iParent) {
					this.checkAppendMsg(zContext,"Bad ptr map entry key=%d expected=(%d,%d) got=(%d,%d)",iChild,eType,iParent,ePtrmapType,iPtrmapParent);
				}
			}
			public void checkAppendMsg(string zMsg1,string zFormat,params object[] ap) {
				if(0==this.mxErr)
					return;
				//va_list ap;
				lock(lock_va_list) {
					this.mxErr--;
					this.nErr++;
					va_start(ap,zFormat);
					if(this.errMsg.zText.Length!=0) {
						sqlite3StrAccumAppend(this.errMsg,"\n",1);
					}
					if(zMsg1.Length>0) {
						sqlite3StrAccumAppend(this.errMsg,zMsg1.ToString(),-1);
					}
					sqlite3VXPrintf(this.errMsg,1,zFormat,ap);
					va_end(ref ap);
				}
			}
		}
		///<summary>
		/// Read or write a two- and four-byte big-endian integer values.
		///
		///</summary>
		//#define get2byte(x)   ((x)[0]<<8 | (x)[1])
		static int get2byte(byte[] p,int offset) {
			return p[offset+0]<<8|p[offset+1];
		}
		//#define put2byte(p,v) ((p)[0] = (u8)((v)>>8), (p)[1] = (u8)(v))
		static void put2byte(byte[] pData,int Offset,u32 v) {
			pData[Offset+0]=(byte)(v>>8);
			pData[Offset+1]=(byte)v;
		}
		static void put2byte(byte[] pData,int Offset,int v) {
			pData[Offset+0]=(byte)(v>>8);
			pData[Offset+1]=(byte)v;
		}
	//#define get4byte sqlite3Get4byte
	//#define put4byte sqlite3Put4byte
	}
}
