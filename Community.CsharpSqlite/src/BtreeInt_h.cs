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
		public class MemPage {
			public u8 isInit;
			/* True if previously initialized. MUST BE FIRST! */public u8 nOverflow;
			/* Number of overflow cell bodies in aCell[] */public u8 intKey;
			/* True if u8key flag is set */public u8 leaf;
			/* 1 if leaf flag is set */public u8 hasData;
			/* True if this page stores data */public u8 hdrOffset;
			/* 100 for page 1.  0 otherwise */public u8 childPtrSize;
			/* 0 if leaf==1.  4 if leaf==0 */public u16 maxLocal;
			/* Copy of BtShared.maxLocal or BtShared.maxLeaf */public u16 minLocal;
			/* Copy of BtShared.minLocal or BtShared.minLeaf */public u16 cellOffset;
			/* Index in aData of first cell pou16er */public u16 nFree;
			/* Number of free bytes on the page */public u16 nCell;
			/* Number of cells on this page, local and ovfl */public u16 maskPage;
			/* Mask for page offset */public _OvflCell[] aOvfl=new _OvflCell[5];
			public BtShared pBt;
			/* Pointer to BtShared that this page is part of */public byte[] aData;
			/* Pointer to disk image of the page data */public DbPage pDbPage;
			/* Pager page handle */public Pgno pgno;
			///<summary>
			///Page number for this page
			///</summary>
			//public byte[] aData
			//{
			//  get
			//  {
			//    Debug.Assert( pgno != 1 || pDbPage.pData == _aData );
			//    return _aData;
			//  }
			//  set
			//  {
			//    _aData = value;
			//    Debug.Assert( pgno != 1 || pDbPage.pData == _aData );
			//  }
			//}
			public MemPage Copy() {
				MemPage cp=(MemPage)MemberwiseClone();
				if(aOvfl!=null) {
					cp.aOvfl=new _OvflCell[aOvfl.Length];
					for(int i=0;i<aOvfl.Length;i++)
						cp.aOvfl[i]=aOvfl[i].Copy();
				}
				if(aData!=null) {
					cp.aData=sqlite3Malloc(aData.Length);
					Buffer.BlockCopy(aData,0,cp.aData,0,aData.Length);
				}
				return cp;
			}
			/**
///<summary>
///This a more complex version of findCell() that works for
///pages that do contain overflow cells.
///</summary>
*/public int findOverflowCell(int iCell) {
				int i;
				Debug.Assert(sqlite3_mutex_held(this.pBt.mutex));
				for(i=this.nOverflow-1;i>=0;i--) {
					int k;
					_OvflCell pOvfl;
					pOvfl=this.aOvfl[i];
					k=pOvfl.idx;
					if(k<=iCell) {
						if(k==iCell) {
							//return pOvfl.pCell;
							return -i-1;
							// Negative Offset means overflow cells
						}
						iCell--;
					}
				}
				return findCell(this,iCell);
			}
			public///<summary>
			/// Parse a cell content block and fill in the CellInfo structure.  There
			/// are two versions of this function.  btreeParseCell() takes a
			/// cell index as the second argument and btreeParseCellPtr()
			/// takes a pointer to the body of the cell as its second argument.
			///
			/// Within this file, the parseCell() macro can be called instead of
			/// btreeParseCellPtr(). Using some compilers, this will be faster.
			///</summary>
			//OVERLOADS
			void btreeParseCellPtr(/* Page containing the cell */int iCell,/* Pointer to the cell text. */ref CellInfo pInfo/* Fill in this structure */) {
				this.btreeParseCellPtr(this.aData,iCell,ref pInfo);
			}
			public void btreeParseCellPtr(/* Page containing the cell */byte[] pCell,/* The actual data */ref CellInfo pInfo/* Fill in this structure */) {
				this.btreeParseCellPtr(pCell,0,ref pInfo);
			}
			public void btreeParseCellPtr(/* Page containing the cell */u8[] pCell,/* Pointer to the cell text. */int iCell,/* Pointer to the cell text. */ref CellInfo pInfo/* Fill in this structure */) {
				u16 n;
				/* Number bytes in cell content header */u32 nPayload=0;
				/* Number of bytes of cell payload */Debug.Assert(sqlite3_mutex_held(this.pBt.mutex));
				if(pInfo.pCell!=pCell)
					pInfo.pCell=pCell;
				pInfo.iCell=iCell;
				Debug.Assert(this.leaf==0||this.leaf==1);
				n=this.childPtrSize;
				Debug.Assert(n==4-4*this.leaf);
				if(this.intKey!=0) {
					if(this.hasData!=0) {
						n+=(u16)getVarint32(pCell,iCell+n,out nPayload);
					}
					else {
						nPayload=0;
					}
					n+=(u16)getVarint(pCell,iCell+n,out pInfo.nKey);
					pInfo.nData=nPayload;
				}
				else {
					pInfo.nData=0;
					n+=(u16)getVarint32(pCell,iCell+n,out nPayload);
					pInfo.nKey=nPayload;
				}
				pInfo.nPayload=nPayload;
				pInfo.nHeader=n;
				testcase(nPayload==this.maxLocal);
				testcase(nPayload==this.maxLocal+1);
				if(likely(nPayload<=this.maxLocal)) {
					/* This is the (easy) common case where the entire payload fits
    ** on the local page.  No overflow is required.
    */if((pInfo.nSize=(u16)(n+nPayload))<4)
						pInfo.nSize=4;
					pInfo.nLocal=(u16)nPayload;
					pInfo.iOverflow=0;
				}
				else {
					/* If the payload will not fit completely on the local page, we have
    ** to decide how much to store locally and how much to spill onto
    ** overflow pages.  The strategy is to minimize the amount of unused
    ** space on overflow pages while keeping the amount of local storage
    ** in between minLocal and maxLocal.
    **
    ** Warning:  changing the way overflow payload is distributed in any
    ** way will result in an incompatible file format.
    */int minLocal;
					/* Minimum amount of payload held locally */int maxLocal;
					/* Maximum amount of payload held locally */int surplus;
					/* Overflow payload available for local storage */minLocal=this.minLocal;
					maxLocal=this.maxLocal;
					surplus=(int)(minLocal+(nPayload-minLocal)%(this.pBt.usableSize-4));
					testcase(surplus==maxLocal);
					testcase(surplus==maxLocal+1);
					if(surplus<=maxLocal) {
						pInfo.nLocal=(u16)surplus;
					}
					else {
						pInfo.nLocal=(u16)minLocal;
					}
					pInfo.iOverflow=(u16)(pInfo.nLocal+n);
					pInfo.nSize=(u16)(pInfo.iOverflow+4);
				}
			}
			//  btreeParseCellPtr((pPage), findCell((pPage), (iCell)), (pInfo))
			public void parseCell(int iCell,ref CellInfo pInfo) {
				this.btreeParseCellPtr(findCell(this,iCell),ref pInfo);
			}
			public///<summary>
			/// Compute the total number of bytes that a Cell needs in the cell
			/// data area of the btree-page.  The return number includes the cell
			/// data header and the local payload, but not any overflow page or
			/// the space used by the cell pointer.
			///</summary>
			// Alternative form for C#
			u16 cellSizePtr(int iCell) {
				CellInfo info=new CellInfo();
				byte[] pCell=new byte[13];
				// Minimum Size = (2 bytes of Header  or (4) Child Pointer) + (maximum of) 9 bytes data
				if(iCell<0)
					// Overflow Cell
					Buffer.BlockCopy(this.aOvfl[-(iCell+1)].pCell,0,pCell,0,pCell.Length<this.aOvfl[-(iCell+1)].pCell.Length?pCell.Length:this.aOvfl[-(iCell+1)].pCell.Length);
				else
					if(iCell>=this.aData.Length+1-pCell.Length)
						Buffer.BlockCopy(this.aData,iCell,pCell,0,this.aData.Length-iCell);
					else
						Buffer.BlockCopy(this.aData,iCell,pCell,0,pCell.Length);
				this.btreeParseCellPtr(pCell,ref info);
				return info.nSize;
			}
			public void btreeParseCell(/* Page containing the cell */int iCell,/* The cell index.  First cell is 0 */ref CellInfo pInfo/* Fill in this structure */) {
				this.parseCell(iCell,ref pInfo);
			}
			// Alternative form for C#
			public u16 cellSizePtr(byte[] pCell,int offset) {
				CellInfo info=new CellInfo();
				info.pCell=sqlite3Malloc(pCell.Length);
				Buffer.BlockCopy(pCell,offset,info.pCell,0,pCell.Length-offset);
				this.btreeParseCellPtr(info.pCell,ref info);
				return info.nSize;
			}
			public u16 cellSizePtr(u8[] pCell) {
				int _pIter=this.childPtrSize;
				//u8 pIter = &pCell[pPage.childPtrSize];
				u32 nSize=0;
				#if SQLITE_DEBUG || DEBUG
																  /* The value returned by this function should always be the same as
** the (CellInfo.nSize) value found by doing a full parse of the
** cell. If SQLITE_DEBUG is defined, an Debug.Assert() at the bottom of
** this function verifies that this invariant is not violated. */
  CellInfo debuginfo = new CellInfo();
  btreeParseCellPtr( pPage, pCell, ref debuginfo );
#else
				CellInfo debuginfo=new CellInfo();
				#endif
				if(this.intKey!=0) {
					int pEnd;
					if(this.hasData!=0) {
						_pIter+=getVarint32(pCell,out nSize);
						// pIter += getVarint32( pIter, out nSize );
					}
					else {
						nSize=0;
					}
					/* pIter now points at the 64-bit integer key value, a variable length
    ** integer. The following block moves pIter to point at the first byte
    ** past the end of the key value. */pEnd=_pIter+9;
					//pEnd = &pIter[9];
					while(((pCell[_pIter++])&0x80)!=0&&_pIter<pEnd)
						;
					//while( (pIter++)&0x80 && pIter<pEnd );
				}
				else {
					_pIter+=getVarint32(pCell,_pIter,out nSize);
					//pIter += getVarint32( pIter, out nSize );
				}
				testcase(nSize==this.maxLocal);
				testcase(nSize==this.maxLocal+1);
				if(nSize>this.maxLocal) {
					int minLocal=this.minLocal;
					nSize=(u32)(minLocal+(nSize-minLocal)%(this.pBt.usableSize-4));
					testcase(nSize==this.maxLocal);
					testcase(nSize==this.maxLocal+1);
					if(nSize>this.maxLocal) {
						nSize=(u32)minLocal;
					}
					nSize+=4;
				}
				nSize+=(uint)_pIter;
				//nSize += (u32)(pIter - pCell);
				/* The minimum size of any cell is 4 bytes. */if(nSize<4) {
					nSize=4;
				}
				Debug.Assert(nSize==debuginfo.nSize);
				return (u16)nSize;
			}
			public int cellSize(int iCell) {
				return -1;
			}
			public///<summary>
			/// If the cell pCell, part of page pPage contains a pointer
			/// to an overflow page, insert an entry into the pointer-map
			/// for the overflow page.
			///</summary>
			void ptrmapPutOvflPtr(int pCell,ref int pRC) {
				if(pRC!=0)
					return;
				CellInfo info=new CellInfo();
				Debug.Assert(pCell!=0);
				this.btreeParseCellPtr(pCell,ref info);
				Debug.Assert((info.nData+(this.intKey!=0?0:info.nKey))==info.nPayload);
				if(info.iOverflow!=0) {
					Pgno ovfl=Converter.sqlite3Get4byte(this.aData,pCell,info.iOverflow);
					ptrmapPut(this.pBt,ovfl,PTRMAP_OVERFLOW1,this.pgno,ref pRC);
				}
			}
			public void ptrmapPutOvflPtr(u8[] pCell,ref int pRC) {
				if(pRC!=0)
					return;
				CellInfo info=new CellInfo();
				Debug.Assert(pCell!=null);
				this.btreeParseCellPtr(pCell,ref info);
				Debug.Assert((info.nData+(this.intKey!=0?0:info.nKey))==info.nPayload);
				if(info.iOverflow!=0) {
					Pgno ovfl=Converter.sqlite3Get4byte(pCell,info.iOverflow);
					ptrmapPut(this.pBt,ovfl,PTRMAP_OVERFLOW1,this.pgno,ref pRC);
				}
			}
			public///<summary>
			/// Defragment the page given.  All Cells are moved to the
			/// end of the page and all free space is collected into one
			/// big FreeBlk that occurs in between the header and cell
			/// pointer array and the cell content area.
			///</summary>
			int defragmentPage() {
				int i;
				/* Loop counter */int pc;
				/* Address of a i-th cell */int addr;
				/* Offset of first byte after cell pointer array */int hdr;
				/* Offset to the page header */int size;
				/* Size of a cell */int usableSize;
				/* Number of usable bytes on a page */int cellOffset;
				/* Offset to the cell pointer array */int cbrk;
				/* Offset to the cell content area */int nCell;
				/* Number of cells on the page */byte[] data;
				/* The page data */byte[] temp;
				/* Temp area for cell content */int iCellFirst;
				/* First allowable cell index */int iCellLast;
				/* Last possible cell index */Debug.Assert(sqlite3PagerIswriteable(this.pDbPage));
				Debug.Assert(this.pBt!=null);
				Debug.Assert(this.pBt.usableSize<=SQLITE_MAX_PAGE_SIZE);
				Debug.Assert(this.nOverflow==0);
				Debug.Assert(sqlite3_mutex_held(this.pBt.mutex));
				temp=sqlite3PagerTempSpace(this.pBt.pPager);
				data=this.aData;
				hdr=this.hdrOffset;
				cellOffset=this.cellOffset;
				nCell=this.nCell;
				Debug.Assert(nCell==get2byte(data,hdr+3));
				usableSize=(int)this.pBt.usableSize;
				cbrk=get2byte(data,hdr+5);
				Buffer.BlockCopy(data,cbrk,temp,cbrk,usableSize-cbrk);
				//memcpy( temp[cbrk], ref data[cbrk], usableSize - cbrk );
				cbrk=usableSize;
				iCellFirst=cellOffset+2*nCell;
				iCellLast=usableSize-4;
				for(i=0;i<nCell;i++) {
					int pAddr;
					/* The i-th cell pointer */pAddr=cellOffset+i*2;
					// &data[cellOffset + i * 2];
					pc=get2byte(data,pAddr);
					testcase(pc==iCellFirst);
					testcase(pc==iCellLast);
					#if !(SQLITE_ENABLE_OVERSIZE_CELL_CHECK)
					/* These conditions have already been verified in btreeInitPage()
** if SQLITE_ENABLE_OVERSIZE_CELL_CHECK is defined
*/if(pc<iCellFirst||pc>iCellLast) {
						return SQLITE_CORRUPT_BKPT();
					}
					#endif
					Debug.Assert(pc>=iCellFirst&&pc<=iCellLast);
					size=this.cellSizePtr(temp,pc);
					cbrk-=size;
					#if (SQLITE_ENABLE_OVERSIZE_CELL_CHECK)
																					    if ( cbrk < iCellFirst || pc + size > usableSize )
    {
      return SQLITE_CORRUPT_BKPT();
    }
#else
					if(cbrk<iCellFirst||pc+size>usableSize) {
						return SQLITE_CORRUPT_BKPT();
					}
					#endif
					Debug.Assert(cbrk+size<=usableSize&&cbrk>=iCellFirst);
					testcase(cbrk+size==usableSize);
					testcase(pc+size==usableSize);
					Buffer.BlockCopy(temp,pc,data,cbrk,size);
					//memcpy(data[cbrk], ref temp[pc], size);
					put2byte(data,pAddr,cbrk);
				}
				Debug.Assert(cbrk>=iCellFirst);
				put2byte(data,hdr+5,cbrk);
				data[hdr+1]=0;
				data[hdr+2]=0;
				data[hdr+7]=0;
				addr=cellOffset+2*nCell;
				Array.Clear(data,addr,cbrk-addr);
				//memset(data[iCellFirst], 0, cbrk-iCellFirst);
				Debug.Assert(sqlite3PagerIswriteable(this.pDbPage));
				if(cbrk-iCellFirst!=this.nFree) {
					return SQLITE_CORRUPT_BKPT();
				}
				return SQLITE_OK;
			}
			public///<summary>
			/// Allocate nByte bytes of space from within the B-Tree page passed
			/// as the first argument. Write into pIdx the index into pPage.aData[]
			/// of the first byte of allocated space. Return either SQLITE_OK or
			/// an error code (usually SQLITE_CORRUPT).
			///
			/// The caller guarantees that there is sufficient space to make the
			/// allocation.  This routine might need to defragment in order to bring
			/// all the space together, however.  This routine will avoid using
			/// the first two bytes past the cell pointer area since presumably this
			/// allocation is being made in order to insert a new cell, so we will
			/// also end up needing a new cell pointer.
			///</summary>
			int allocateSpace(int nByte,ref int pIdx) {
				int hdr=this.hdrOffset;
				/* Local cache of pPage.hdrOffset */u8[] data=this.aData;
				/* Local cache of pPage.aData */int nFrag;
				/* Number of fragmented bytes on pPage */int top;
				/* First byte of cell content area */int gap;
				/* First byte of gap between cell pointers and cell content */int rc;
				/* Integer return code */u32 usableSize;
				/* Usable size of the page */Debug.Assert(sqlite3PagerIswriteable(this.pDbPage));
				Debug.Assert(this.pBt!=null);
				Debug.Assert(sqlite3_mutex_held(this.pBt.mutex));
				Debug.Assert(nByte>=0);
				/* Minimum cell size is 4 */Debug.Assert(this.nFree>=nByte);
				Debug.Assert(this.nOverflow==0);
				usableSize=this.pBt.usableSize;
				Debug.Assert(nByte<usableSize-8);
				nFrag=data[hdr+7];
				Debug.Assert(this.cellOffset==hdr+12-4*this.leaf);
				gap=this.cellOffset+2*this.nCell;
				top=get2byteNotZero(data,hdr+5);
				if(gap>top)
					return SQLITE_CORRUPT_BKPT();
				testcase(gap+2==top);
				testcase(gap+1==top);
				testcase(gap==top);
				if(nFrag>=60) {
					/* Always defragment highly fragmented pages */rc=this.defragmentPage();
					if(rc!=0)
						return rc;
					top=get2byteNotZero(data,hdr+5);
				}
				else
					if(gap+2<=top) {
						/* Search the freelist looking for a free slot big enough to satisfy
    ** the request. The allocation is made from the first free slot in
    ** the list that is large enough to accomadate it.
    */int pc,addr;
						for(addr=hdr+1;(pc=get2byte(data,addr))>0;addr=pc) {
							int size;
							/* Size of free slot */if(pc>usableSize-4||pc<addr+4) {
								return SQLITE_CORRUPT_BKPT();
							}
							size=get2byte(data,pc+2);
							if(size>=nByte) {
								int x=size-nByte;
								testcase(x==4);
								testcase(x==3);
								if(x<4) {
									/* Remove the slot from the free-list. Update the number of
          ** fragmented bytes within the page. */data[addr+0]=data[pc+0];
									data[addr+1]=data[pc+1];
									//memcpy( data[addr], ref data[pc], 2 );
									data[hdr+7]=(u8)(nFrag+x);
								}
								else
									if(size+pc>usableSize) {
										return SQLITE_CORRUPT_BKPT();
									}
									else {
										/* The slot remains on the free-list. Reduce its size to account
          ** for the portion used by the new allocation. */put2byte(data,pc+2,x);
									}
								pIdx=pc+x;
								return SQLITE_OK;
							}
						}
					}
				/* Check to make sure there is enough space in the gap to satisfy
  ** the allocation.  If not, defragment.
  */testcase(gap+2+nByte==top);
				if(gap+2+nByte>top) {
					rc=this.defragmentPage();
					if(rc!=0)
						return rc;
					top=get2byteNotZero(data,hdr+5);
					Debug.Assert(gap+nByte<=top);
				}
				/* Allocate memory from the gap in between the cell pointer array
  ** and the cell content area.  The btreeInitPage() call has already
  ** validated the freelist.  Given that the freelist is valid, there
  ** is no way that the allocation can extend off the end of the page.
  ** The Debug.Assert() below verifies the previous sentence.
  */top-=nByte;
				put2byte(data,hdr+5,top);
				Debug.Assert(top+nByte<=(int)this.pBt.usableSize);
				pIdx=top;
				return SQLITE_OK;
			}
			public///<summary>
			/// Return a section of the pPage.aData to the freelist.
			/// The first byte of the new free block is pPage.aDisk[start]
			/// and the size of the block is "size" bytes.
			///
			/// Most of the effort here is involved in coalesing adjacent
			/// free blocks into a single big free block.
			///</summary>
			int freeSpace(u32 start,int size) {
				return this.freeSpace((int)start,size);
			}
			public int freeSpace(int start,int size) {
				int addr,pbegin,hdr;
				int iLast;
				/* Largest possible freeblock offset */byte[] data=this.aData;
				Debug.Assert(this.pBt!=null);
				Debug.Assert(sqlite3PagerIswriteable(this.pDbPage));
				Debug.Assert(start>=this.hdrOffset+6+this.childPtrSize);
				Debug.Assert((start+size)<=(int)this.pBt.usableSize);
				Debug.Assert(sqlite3_mutex_held(this.pBt.mutex));
				Debug.Assert(size>=0);
				/* Minimum cell size is 4 */if(this.pBt.secureDelete) {
					/* Overwrite deleted information with zeros when the secure_delete
    ** option is enabled */Array.Clear(data,start,size);
					// memset(&data[start], 0, size);
				}
				/* Add the space back into the linked list of freeblocks.  Note that
  ** even though the freeblock list was checked by btreeInitPage(),
  ** btreeInitPage() did not detect overlapping cells or
  ** freeblocks that overlapped cells.   Nor does it detect when the
  ** cell content area exceeds the value in the page header.  If these
  ** situations arise, then subsequent insert operations might corrupt
  ** the freelist.  So we do need to check for corruption while scanning
  ** the freelist.
  */hdr=this.hdrOffset;
				addr=hdr+1;
				iLast=(int)this.pBt.usableSize-4;
				Debug.Assert(start<=iLast);
				while((pbegin=get2byte(data,addr))<start&&pbegin>0) {
					if(pbegin<addr+4) {
						return SQLITE_CORRUPT_BKPT();
					}
					addr=pbegin;
				}
				if(pbegin>iLast) {
					return SQLITE_CORRUPT_BKPT();
				}
				Debug.Assert(pbegin>addr||pbegin==0);
				put2byte(data,addr,start);
				put2byte(data,start,pbegin);
				put2byte(data,start+2,size);
				this.nFree=(u16)(this.nFree+size);
				/* Coalesce adjacent free blocks */addr=hdr+1;
				while((pbegin=get2byte(data,addr))>0) {
					int pnext,psize,x;
					Debug.Assert(pbegin>addr);
					Debug.Assert(pbegin<=(int)this.pBt.usableSize-4);
					pnext=get2byte(data,pbegin);
					psize=get2byte(data,pbegin+2);
					if(pbegin+psize+3>=pnext&&pnext>0) {
						int frag=pnext-(pbegin+psize);
						if((frag<0)||(frag>(int)data[hdr+7])) {
							return SQLITE_CORRUPT_BKPT();
						}
						data[hdr+7]-=(u8)frag;
						x=get2byte(data,pnext);
						put2byte(data,pbegin,x);
						x=pnext+get2byte(data,pnext+2)-pbegin;
						put2byte(data,pbegin+2,x);
					}
					else {
						addr=pbegin;
					}
				}
				/* If the cell content area begins with a freeblock, remove it. */if(data[hdr+1]==data[hdr+5]&&data[hdr+2]==data[hdr+6]) {
					int top;
					pbegin=get2byte(data,hdr+1);
					put2byte(data,hdr+1,get2byte(data,pbegin));
					//memcpy( data[hdr + 1], ref data[pbegin], 2 );
					top=get2byte(data,hdr+5)+get2byte(data,pbegin+2);
					put2byte(data,hdr+5,top);
				}
				Debug.Assert(sqlite3PagerIswriteable(this.pDbPage));
				return SQLITE_OK;
			}
			public///<summary>
			/// Decode the flags byte (the first byte of the header) for a page
			/// and initialize fields of the MemPage structure accordingly.
			///
			/// Only the following combinations are supported.  Anything different
			/// indicates a corrupt database files:
			///
			///         PTF_ZERODATA
			///         PTF_ZERODATA | PTF_LEAF
			///         PTF_LEAFDATA | PTF_INTKEY
			///         PTF_LEAFDATA | PTF_INTKEY | PTF_LEAF
			///</summary>
			int decodeFlags(int flagByte) {
				BtShared pBt;
				/* A copy of pPage.pBt */Debug.Assert(this.hdrOffset==(this.pgno==1?100:0));
				Debug.Assert(sqlite3_mutex_held(this.pBt.mutex));
				this.leaf=(u8)(flagByte>>3);
				Debug.Assert(PTF_LEAF==1<<3);
				flagByte&=~PTF_LEAF;
				this.childPtrSize=(u8)(4-4*this.leaf);
				pBt=this.pBt;
				if(flagByte==(PTF_LEAFDATA|PTF_INTKEY)) {
					this.intKey=1;
					this.hasData=this.leaf;
					this.maxLocal=pBt.maxLeaf;
					this.minLocal=pBt.minLeaf;
				}
				else
					if(flagByte==PTF_ZERODATA) {
						this.intKey=0;
						this.hasData=0;
						this.maxLocal=pBt.maxLocal;
						this.minLocal=pBt.minLocal;
					}
					else {
						return SQLITE_CORRUPT_BKPT();
					}
				return SQLITE_OK;
			}
			public///<summary>
			/// Initialize the auxiliary information for a disk block.
			///
			/// Return SQLITE_OK on success.  If we see that the page does
			/// not contain a well-formed database page, then return
			/// SQLITE_CORRUPT.  Note that a return of SQLITE_OK does not
			/// guarantee that the page is well-formed.  It only shows that
			/// we failed to detect any corruption.
			///</summary>
			int btreeInitPage() {
				Debug.Assert(this.pBt!=null);
				Debug.Assert(sqlite3_mutex_held(this.pBt.mutex));
				Debug.Assert(this.pgno==sqlite3PagerPagenumber(this.pDbPage));
				Debug.Assert(this==sqlite3PagerGetExtra(this.pDbPage));
				Debug.Assert(this.aData==sqlite3PagerGetData(this.pDbPage));
				if(0==this.isInit) {
					u16 pc;
					/* Address of a freeblock within pPage.aData[] */u8 hdr;
					/* Offset to beginning of page header */u8[] data;
					/* Equal to pPage.aData */BtShared pBt;
					/* The main btree structure */int usableSize;
					/* Amount of usable space on each page */u16 cellOffset;
					/* Offset from start of page to first cell pointer */int nFree;
					/* Number of unused bytes on the page */int top;
					/* First byte of the cell content area */int iCellFirst;
					/* First allowable cell or freeblock offset */int iCellLast;
					/* Last possible cell or freeblock offset */pBt=this.pBt;
					hdr=this.hdrOffset;
					data=this.aData;
					if(this.decodeFlags(data[hdr])!=0)
						return SQLITE_CORRUPT_BKPT();
					Debug.Assert(pBt.pageSize>=512&&pBt.pageSize<=65536);
					this.maskPage=(u16)(pBt.pageSize-1);
					this.nOverflow=0;
					usableSize=(int)pBt.usableSize;
					this.cellOffset=(cellOffset=(u16)(hdr+12-4*this.leaf));
					top=get2byteNotZero(data,hdr+5);
					this.nCell=(u16)(get2byte(data,hdr+3));
					if(this.nCell>MX_CELL(pBt)) {
						/* To many cells for a single page.  The page must be corrupt */return SQLITE_CORRUPT_BKPT();
					}
					testcase(this.nCell==MX_CELL(pBt));
					/* A malformed database page might cause us to read past the end
    ** of page when parsing a cell.
    **
    ** The following block of code checks early to see if a cell extends
    ** past the end of a page boundary and causes SQLITE_CORRUPT to be
    ** returned if it does.
    */iCellFirst=cellOffset+2*this.nCell;
					iCellLast=usableSize-4;
					#if (SQLITE_ENABLE_OVERSIZE_CELL_CHECK)
																					    {
      int i;            /* Index into the cell pointer array */
      int sz;           /* Size of a cell */

      if ( 0 == pPage.leaf )
        iCellLast--;
      for ( i = 0; i < pPage.nCell; i++ )
      {
        pc = (u16)get2byte( data, cellOffset + i * 2 );
        testcase( pc == iCellFirst );
        testcase( pc == iCellLast );
        if ( pc < iCellFirst || pc > iCellLast )
        {
          return SQLITE_CORRUPT_BKPT();
        }
        sz = cellSizePtr( pPage, data, pc );
        testcase( pc + sz == usableSize );
        if ( pc + sz > usableSize )
        {
          return SQLITE_CORRUPT_BKPT();
        }
      }
      if ( 0 == pPage.leaf )
        iCellLast++;
    }
#endif
					/* Compute the total free space on the page */pc=(u16)get2byte(data,hdr+1);
					nFree=(u16)(data[hdr+7]+top);
					while(pc>0) {
						u16 next,size;
						if(pc<iCellFirst||pc>iCellLast) {
							/* Start of free block is off the page */return SQLITE_CORRUPT_BKPT();
						}
						next=(u16)get2byte(data,pc);
						size=(u16)get2byte(data,pc+2);
						if((next>0&&next<=pc+size+3)||pc+size>usableSize) {
							/* Free blocks must be in ascending order. And the last byte of
        ** the free-block must lie on the database page.  */return SQLITE_CORRUPT_BKPT();
						}
						nFree=(u16)(nFree+size);
						pc=next;
					}
					/* At this point, nFree contains the sum of the offset to the start
    ** of the cell-content area plus the number of free bytes within
    ** the cell-content area. If this is greater than the usable-size
    ** of the page, then the page must be corrupted. This check also
    ** serves to verify that the offset to the start of the cell-content
    ** area, according to the page header, lies within the page.
    */if(nFree>usableSize) {
						return SQLITE_CORRUPT_BKPT();
					}
					this.nFree=(u16)(nFree-iCellFirst);
					this.isInit=1;
				}
				return SQLITE_OK;
			}
			public///<summary>
			/// Set up a raw page so that it looks like a database page holding
			/// no entries.
			///</summary>
			void zeroPage(int flags) {
				byte[] data=this.aData;
				BtShared pBt=this.pBt;
				u8 hdr=this.hdrOffset;
				u16 first;
				Debug.Assert(sqlite3PagerPagenumber(this.pDbPage)==this.pgno);
				Debug.Assert(sqlite3PagerGetExtra(this.pDbPage)==this);
				Debug.Assert(sqlite3PagerGetData(this.pDbPage)==data);
				Debug.Assert(sqlite3PagerIswriteable(this.pDbPage));
				Debug.Assert(sqlite3_mutex_held(pBt.mutex));
				if(pBt.secureDelete) {
					Array.Clear(data,hdr,(int)(pBt.usableSize-hdr));
					//memset(&data[hdr], 0, pBt->usableSize - hdr);
				}
				data[hdr]=(u8)flags;
				first=(u16)(hdr+8+4*((flags&PTF_LEAF)==0?1:0));
				Array.Clear(data,hdr+1,4);
				//memset(data[hdr+1], 0, 4);
				data[hdr+7]=0;
				put2byte(data,hdr+5,pBt.usableSize);
				this.nFree=(u16)(pBt.usableSize-first);
				this.decodeFlags(flags);
				this.hdrOffset=hdr;
				this.cellOffset=first;
				this.nOverflow=0;
				Debug.Assert(pBt.pageSize>=512&&pBt.pageSize<=65536);
				this.maskPage=(u16)(pBt.pageSize-1);
				this.nCell=0;
				this.isInit=1;
			}
			/**
///<summary>
///</summary>
///<param name="Set the pointer">map entries for all children of page pPage. Also, if</param>
///<param name="pPage contains cells that point to overflow pages, set the pointer">pPage contains cells that point to overflow pages, set the pointer</param>
///<param name="map entries for the overflow pages as well.">map entries for the overflow pages as well.</param>
*/public int setChildPtrmaps() {
				int i;
				/* Counter variable */int nCell;
				/* Number of cells in page pPage */int rc;
				/* Return code */BtShared pBt=this.pBt;
				u8 isInitOrig=this.isInit;
				Pgno pgno=this.pgno;
				Debug.Assert(sqlite3_mutex_held(this.pBt.mutex));
				rc=this.btreeInitPage();
				if(rc!=SQLITE_OK) {
					goto set_child_ptrmaps_out;
				}
				nCell=this.nCell;
				for(i=0;i<nCell;i++) {
					int pCell=findCell(this,i);
					this.ptrmapPutOvflPtr(pCell,ref rc);
					if(0==this.leaf) {
						Pgno childPgno=Converter.sqlite3Get4byte(this.aData,pCell);
						ptrmapPut(pBt,childPgno,PTRMAP_BTREE,pgno,ref rc);
					}
				}
				if(0==this.leaf) {
					Pgno childPgno=Converter.sqlite3Get4byte(this.aData,this.hdrOffset+8);
					ptrmapPut(pBt,childPgno,PTRMAP_BTREE,pgno,ref rc);
				}
				set_child_ptrmaps_out:
				this.isInit=isInitOrig;
				return rc;
			}
			/**
///<summary>
///Somewhere on pPage is a pointer to page iFrom.  Modify this pointer so
///that it points to iTo. Parameter eType describes the type of pointer to
///be modified, as  follows:
///
///</summary>
///<param name="PTRMAP_BTREE:     pPage is a btree">page. The pointer points at a child</param>
///<param name="page of pPage.">page of pPage.</param>
///<param name=""></param>
///<param name="PTRMAP_OVERFLOW1: pPage is a btree">page. The pointer points at an overflow</param>
///<param name="page pointed to by one of the cells on pPage.">page pointed to by one of the cells on pPage.</param>
///<param name=""></param>
///<param name="PTRMAP_OVERFLOW2: pPage is an overflow">page. The pointer points at the next</param>
///<param name="overflow page in the list.">overflow page in the list.</param>
*/public int modifyPagePointer(Pgno iFrom,Pgno iTo,u8 eType) {
				Debug.Assert(sqlite3_mutex_held(this.pBt.mutex));
				Debug.Assert(sqlite3PagerIswriteable(this.pDbPage));
				if(eType==PTRMAP_OVERFLOW2) {
					/* The pointer is always the first 4 bytes of the page in this case.  */if(Converter.sqlite3Get4byte(this.aData)!=iFrom) {
						return SQLITE_CORRUPT_BKPT();
					}
					Converter.sqlite3Put4byte(this.aData,iTo);
				}
				else {
					u8 isInitOrig=this.isInit;
					int i;
					int nCell;
					this.btreeInitPage();
					nCell=this.nCell;
					for(i=0;i<nCell;i++) {
						int pCell=findCell(this,i);
						if(eType==PTRMAP_OVERFLOW1) {
							CellInfo info=new CellInfo();
							this.btreeParseCellPtr(pCell,ref info);
							if(info.iOverflow!=0) {
								if(iFrom==Converter.sqlite3Get4byte(this.aData,pCell,info.iOverflow)) {
									Converter.sqlite3Put4byte(this.aData,pCell+info.iOverflow,(int)iTo);
									break;
								}
							}
						}
						else {
							if(Converter.sqlite3Get4byte(this.aData,pCell)==iFrom) {
								Converter.sqlite3Put4byte(this.aData,pCell,(int)iTo);
								break;
							}
						}
					}
					if(i==nCell) {
						if(eType!=PTRMAP_BTREE||Converter.sqlite3Get4byte(this.aData,this.hdrOffset+8)!=iFrom) {
							return SQLITE_CORRUPT_BKPT();
						}
						Converter.sqlite3Put4byte(this.aData,this.hdrOffset+8,iTo);
					}
					this.isInit=isInitOrig;
				}
				return SQLITE_OK;
			}
			//#  define assertParentIndex(x,y,z)
			public void assertParentIndex(int iIdx,Pgno iChild) {
			}
			/**
///<summary>
///Create the byte sequence used to represent a cell on page pPage
///and write that byte sequence into pCell[].  Overflow pages are
///allocated and filled in as necessary.  The calling procedure
///is responsible for making sure sufficient space has been allocated
///for pCell[].
///
///Note that pCell does not necessary need to point to the pPage.aData
///area.  pCell might point to some temporary storage.  The cell will
///be constructed in this temporary area then copied into pPage.aData
///later.
///</summary>
*/public int fillInCell(/* The page that contains the cell */byte[] pCell,/* Complete text of the cell */byte[] pKey,i64 nKey,/* The key */byte[] pData,int nData,/* The data */int nZero,/* Extra zero bytes to append to pData */ref int pnSize/* Write cell size here */) {
				int nPayload;
				u8[] pSrc;
				int pSrcIndex=0;
				int nSrc,n,rc;
				int spaceLeft;
				MemPage pOvfl=null;
				MemPage pToRelease=null;
				byte[] pPrior;
				int pPriorIndex=0;
				byte[] pPayload;
				int pPayloadIndex=0;
				BtShared pBt=this.pBt;
				Pgno pgnoOvfl=0;
				int nHeader;
				CellInfo info=new CellInfo();
				Debug.Assert(sqlite3_mutex_held(this.pBt.mutex));
				/* pPage is not necessarily writeable since pCell might be auxiliary
  ** buffer space that is separate from the pPage buffer area */// TODO -- Determine if the following Assert is needed under c#
				//Debug.Assert( pCell < pPage.aData || pCell >= &pPage.aData[pBt.pageSize]
				//          || sqlite3PagerIswriteable(pPage.pDbPage) );
				/* Fill in the header. */nHeader=0;
				if(0==this.leaf) {
					nHeader+=4;
				}
				if(this.hasData!=0) {
					nHeader+=(int)putVarint(pCell,nHeader,(int)(nData+nZero));
					//putVarint( pCell[nHeader], nData + nZero );
				}
				else {
					nData=nZero=0;
				}
				nHeader+=putVarint(pCell,nHeader,(u64)nKey);
				//putVarint( pCell[nHeader], *(u64*)&nKey );
				this.btreeParseCellPtr(pCell,ref info);
				Debug.Assert(info.nHeader==nHeader);
				Debug.Assert(info.nKey==nKey);
				Debug.Assert(info.nData==(u32)(nData+nZero));
				/* Fill in the payload */nPayload=nData+nZero;
				if(this.intKey!=0) {
					pSrc=pData;
					nSrc=nData;
					nData=0;
				}
				else {
					if(NEVER(nKey>0x7fffffff||pKey==null)) {
						return SQLITE_CORRUPT_BKPT();
					}
					nPayload+=(int)nKey;
					pSrc=pKey;
					nSrc=(int)nKey;
				}
				pnSize=info.nSize;
				spaceLeft=info.nLocal;
				//  pPayload = &pCell[nHeader];
				pPayload=pCell;
				pPayloadIndex=nHeader;
				//  pPrior = &pCell[info.iOverflow];
				pPrior=pCell;
				pPriorIndex=info.iOverflow;
				while(nPayload>0) {
					if(spaceLeft==0) {
						#if !SQLITE_OMIT_AUTOVACUUM
						Pgno pgnoPtrmap=pgnoOvfl;
						/* Overflow page pointer-map entry page */if(pBt.autoVacuum) {
							do {
								pgnoOvfl++;
							}
							while(PTRMAP_ISPAGE(pBt,pgnoOvfl)||pgnoOvfl==PENDING_BYTE_PAGE(pBt));
						}
						#endif
						rc=allocateBtreePage(pBt,ref pOvfl,ref pgnoOvfl,pgnoOvfl,0);
						#if !SQLITE_OMIT_AUTOVACUUM
						/* If the database supports auto-vacuum, and the second or subsequent
** overflow page is being allocated, add an entry to the pointer-map
** for that page now.
**
** If this is the first overflow page, then write a partial entry
** to the pointer-map. If we write nothing to this pointer-map slot,
** then the optimistic overflow chain processing in clearCell()
** may misinterpret the uninitialised values and delete the
** wrong pages from the database.
*/if(pBt.autoVacuum&&rc==SQLITE_OK) {
							u8 eType=(u8)(pgnoPtrmap!=0?PTRMAP_OVERFLOW2:PTRMAP_OVERFLOW1);
							ptrmapPut(pBt,pgnoOvfl,eType,pgnoPtrmap,ref rc);
							if(rc!=0) {
								releasePage(pOvfl);
							}
						}
						#endif
						if(rc!=0) {
							releasePage(pToRelease);
							return rc;
						}
						/* If pToRelease is not zero than pPrior points into the data area
      ** of pToRelease.  Make sure pToRelease is still writeable. */Debug.Assert(pToRelease==null||sqlite3PagerIswriteable(pToRelease.pDbPage));
						/* If pPrior is part of the data area of pPage, then make sure pPage
      ** is still writeable */// TODO -- Determine if the following Assert is needed under c#
						//Debug.Assert( pPrior < pPage.aData || pPrior >= &pPage.aData[pBt.pageSize]
						//      || sqlite3PagerIswriteable(pPage.pDbPage) );
						Converter.sqlite3Put4byte(pPrior,pPriorIndex,pgnoOvfl);
						releasePage(pToRelease);
						pToRelease=pOvfl;
						pPrior=pOvfl.aData;
						pPriorIndex=0;
						Converter.sqlite3Put4byte(pPrior,0);
						pPayload=pOvfl.aData;
						pPayloadIndex=4;
						//&pOvfl.aData[4];
						spaceLeft=(int)pBt.usableSize-4;
					}
					n=nPayload;
					if(n>spaceLeft)
						n=spaceLeft;
					/* If pToRelease is not zero than pPayload points into the data area
    ** of pToRelease.  Make sure pToRelease is still writeable. */Debug.Assert(pToRelease==null||sqlite3PagerIswriteable(pToRelease.pDbPage));
					/* If pPayload is part of the data area of pPage, then make sure pPage
    ** is still writeable */// TODO -- Determine if the following Assert is needed under c#
					//Debug.Assert( pPayload < pPage.aData || pPayload >= &pPage.aData[pBt.pageSize]
					//        || sqlite3PagerIswriteable(pPage.pDbPage) );
					if(nSrc>0) {
						if(n>nSrc)
							n=nSrc;
						Debug.Assert(pSrc!=null);
						Buffer.BlockCopy(pSrc,pSrcIndex,pPayload,pPayloadIndex,n);
						//memcpy(pPayload, pSrc, n);
					}
					else {
						byte[] pZeroBlob=sqlite3Malloc(n);
						// memset(pPayload, 0, n);
						Buffer.BlockCopy(pZeroBlob,0,pPayload,pPayloadIndex,n);
					}
					nPayload-=n;
					pPayloadIndex+=n;
					// pPayload += n;
					pSrcIndex+=n;
					// pSrc += n;
					nSrc-=n;
					spaceLeft-=n;
					if(nSrc==0) {
						nSrc=nData;
						pSrc=pData;
					}
				}
				releasePage(pToRelease);
				return SQLITE_OK;
			}
			/**
///<summary>
///</summary>
///<param name="Remove the i">th cell from pPage.  This routine effects pPage only.</param>
///<param name="The cell content is not freed or deallocated.  It is assumed that">The cell content is not freed or deallocated.  It is assumed that</param>
///<param name="the cell content has been copied someplace else.  This routine just">the cell content has been copied someplace else.  This routine just</param>
///<param name="removes the reference to the cell from pPage.">removes the reference to the cell from pPage.</param>
///<param name=""></param>
///<param name=""sz" must be the number of bytes in the cell.">"sz" must be the number of bytes in the cell.</param>
*/public void dropCell(int idx,int sz,ref int pRC) {
				u32 pc;
				/* Offset to cell content of cell being deleted */u8[] data;
				/* pPage.aData */int ptr;
				/* Used to move bytes around within data[] */int endPtr;
				/* End of loop */int rc;
				/* The return code */int hdr;
				/* Beginning of the header.  0 most pages.  100 page 1 */if(pRC!=0)
					return;
				Debug.Assert(idx>=0&&idx<this.nCell);
				#if SQLITE_DEBUG
																  Debug.Assert( sz == cellSize( pPage, idx ) );
#endif
				Debug.Assert(sqlite3PagerIswriteable(this.pDbPage));
				Debug.Assert(sqlite3_mutex_held(this.pBt.mutex));
				data=this.aData;
				ptr=this.cellOffset+2*idx;
				//ptr = &data[pPage.cellOffset + 2 * idx];
				pc=(u32)get2byte(data,ptr);
				hdr=this.hdrOffset;
				testcase(pc==get2byte(data,hdr+5));
				testcase(pc+sz==this.pBt.usableSize);
				if(pc<(u32)get2byte(data,hdr+5)||pc+sz>this.pBt.usableSize) {
					pRC=SQLITE_CORRUPT_BKPT();
					return;
				}
				rc=this.freeSpace(pc,sz);
				if(rc!=0) {
					pRC=rc;
					return;
				}
				//endPtr = &data[pPage->cellOffset + 2*pPage->nCell - 2];
				//assert( (SQLITE_PTR_TO_INT(ptr)&1)==0 );  /* ptr is always 2-byte aligned */
				//while( ptr<endPtr ){
				//  *(u16*)ptr = *(u16*)&ptr[2];
				//  ptr += 2;
				Buffer.BlockCopy(data,ptr+2,data,ptr,(this.nCell-1-idx)*2);
				this.nCell--;
				data[this.hdrOffset+3]=(byte)(this.nCell>>8);
				data[this.hdrOffset+4]=(byte)(this.nCell);
				//put2byte( data, hdr + 3, pPage.nCell );
				this.nFree+=2;
			}
			/**
///<summary>
///Insert a new cell on pPage at cell index "i".  pCell points to the
///content of the cell.
///
///If the cell content will fit on the page, then put it there.  If it
///will not fit, then make a copy of the cell content into pTemp if
///pTemp is not null.  Regardless of pTemp, allocate a new entry
///in pPage.aOvfl[] and make it point to the cell content (either
///in pTemp or the original pCell) and also record its index.
///Allocating a new entry in pPage.aCell[] implies that
///pPage.nOverflow is incremented.
///
///</summary>
///<param name="If nSkip is non">zero, then do not copy the first nSkip bytes of the</param>
///<param name="cell. The caller will overwrite them after this function returns. If">cell. The caller will overwrite them after this function returns. If</param>
///<param name="nSkip is non">zero, then pCell may not point to an invalid memory location</param>
///<param name="(but pCell+nSkip is always valid).">(but pCell+nSkip is always valid).</param>
*/public void insertCell(/* Page into which we are copying */int i,/* New cell becomes the i-th cell of the page */u8[] pCell,/* Content of the new cell */int sz,/* Bytes of content in pCell */u8[] pTemp,/* Temp storage space for pCell, if needed */Pgno iChild,/* If non-zero, replace first 4 bytes with this value */ref int pRC/* Read and write return code from here */) {
				int idx=0;
				/* Where to write new cell content in data[] */int j;
				/* Loop counter */int end;
				/* First byte past the last cell pointer in data[] */int ins;
				/* Index in data[] where new cell pointer is inserted */int cellOffset;
				/* Address of first cell pointer in data[] */u8[] data;
				/* The content of the whole page */u8 ptr;
				/* Used for moving information around in data[] */u8 endPtr;
				/* End of the loop */int nSkip=(iChild!=0?4:0);
				if(pRC!=0)
					return;
				Debug.Assert(i>=0&&i<=this.nCell+this.nOverflow);
				Debug.Assert(this.nCell<=MX_CELL(this.pBt)&&MX_CELL(this.pBt)<=10921);
				Debug.Assert(this.nOverflow<=ArraySize(this.aOvfl));
				Debug.Assert(sqlite3_mutex_held(this.pBt.mutex));
				/* The cell should normally be sized correctly.  However, when moving a
  ** malformed cell from a leaf page to an interior page, if the cell size
  ** wanted to be less than 4 but got rounded up to 4 on the leaf, then size
  ** might be less than 8 (leaf-size + pointer) on the interior node.  Hence
  ** the term after the || in the following assert(). */Debug.Assert(sz==this.cellSizePtr(pCell)||(sz==8&&iChild>0));
				if(this.nOverflow!=0||sz+2>this.nFree) {
					if(pTemp!=null) {
						Buffer.BlockCopy(pCell,nSkip,pTemp,nSkip,sz-nSkip);
						//memcpy(pTemp+nSkip, pCell+nSkip, sz-nSkip);
						pCell=pTemp;
					}
					if(iChild!=0) {
						Converter.sqlite3Put4byte(pCell,iChild);
					}
					j=this.nOverflow++;
					Debug.Assert(j<this.aOvfl.Length);
					//(int)(sizeof(pPage.aOvfl)/sizeof(pPage.aOvfl[0])) );
					this.aOvfl[j].pCell=pCell;
					this.aOvfl[j].idx=(u16)i;
				}
				else {
					int rc=sqlite3PagerWrite(this.pDbPage);
					if(rc!=SQLITE_OK) {
						pRC=rc;
						return;
					}
					Debug.Assert(sqlite3PagerIswriteable(this.pDbPage));
					data=this.aData;
					cellOffset=this.cellOffset;
					end=cellOffset+2*this.nCell;
					ins=cellOffset+2*i;
					rc=this.allocateSpace(sz,ref idx);
					if(rc!=0) {
						pRC=rc;
						return;
					}
					/* The allocateSpace() routine guarantees the following two properties
    ** if it returns success */Debug.Assert(idx>=end+2);
					Debug.Assert(idx+sz<=(int)this.pBt.usableSize);
					this.nCell++;
					this.nFree-=(u16)(2+sz);
					Buffer.BlockCopy(pCell,nSkip,data,idx+nSkip,sz-nSkip);
					//memcpy( data[idx + nSkip], pCell + nSkip, sz - nSkip );
					if(iChild!=0) {
						Converter.sqlite3Put4byte(data,idx,iChild);
					}
					//ptr = &data[end];
					//endPtr = &data[ins];
					//assert( ( SQLITE_PTR_TO_INT( ptr ) & 1 ) == 0 );  /* ptr is always 2-byte aligned */
					//while ( ptr > endPtr )
					//{
					//  *(u16*)ptr = *(u16*)&ptr[-2];
					//  ptr -= 2;
					//}
					for(j=end;j>ins;j-=2) {
						data[j+0]=data[j-2];
						data[j+1]=data[j-1];
					}
					put2byte(data,ins,idx);
					put2byte(data,this.hdrOffset+3,this.nCell);
					#if !SQLITE_OMIT_AUTOVACUUM
					if(this.pBt.autoVacuum) {
						/* The cell may contain a pointer to an overflow page. If so, write
      ** the entry for the overflow page into the pointer map.
      */this.ptrmapPutOvflPtr(pCell,ref pRC);
					}
					#endif
				}
			}
			/**
///<summary>
///Add a list of cells to a page.  The page should be initially empty.
///The cells are guaranteed to fit on the page.
///</summary>
*/public void assemblePage(/* The page to be assemblied */int nCell,/* The number of cells to add to this page */u8[] apCell,/* Pointer to a single the cell bodies */int[] aSize/* Sizes of the cells bodie*/) {
				int i;
				/* Loop counter */int pCellptr;
				/* Address of next cell pointer */int cellbody;
				/* Address of next cell body */byte[] data=this.aData;
				/* Pointer to data for pPage */int hdr=this.hdrOffset;
				/* Offset of header on pPage */int nUsable=(int)this.pBt.usableSize;
				/* Usable size of page */Debug.Assert(this.nOverflow==0);
				Debug.Assert(sqlite3_mutex_held(this.pBt.mutex));
				Debug.Assert(nCell>=0&&nCell<=(int)MX_CELL(this.pBt)&&(int)MX_CELL(this.pBt)<=10921);
				Debug.Assert(sqlite3PagerIswriteable(this.pDbPage));
				/* Check that the page has just been zeroed by zeroPage() */Debug.Assert(this.nCell==0);
				Debug.Assert(get2byteNotZero(data,hdr+5)==nUsable);
				pCellptr=this.cellOffset+nCell*2;
				//data[pPage.cellOffset + nCell * 2];
				cellbody=nUsable;
				for(i=nCell-1;i>=0;i--) {
					u16 sz=(u16)aSize[i];
					pCellptr-=2;
					cellbody-=sz;
					put2byte(data,pCellptr,cellbody);
					Buffer.BlockCopy(apCell,0,data,cellbody,sz);
					// memcpy(&data[cellbody], apCell[i], sz);
				}
				put2byte(data,hdr+3,nCell);
				put2byte(data,hdr+5,cellbody);
				this.nFree-=(u16)(nCell*2+nUsable-cellbody);
				this.nCell=(u16)nCell;
			}
			public void assemblePage(/* The page to be assemblied */int nCell,/* The number of cells to add to this page */u8[][] apCell,/* Pointers to cell bodies */u16[] aSize,/* Sizes of the cells */int offset/* Offset into the cell bodies, for c#  */) {
				int i;
				/* Loop counter */int pCellptr;
				/* Address of next cell pointer */int cellbody;
				/* Address of next cell body */byte[] data=this.aData;
				/* Pointer to data for pPage */int hdr=this.hdrOffset;
				/* Offset of header on pPage */int nUsable=(int)this.pBt.usableSize;
				/* Usable size of page */Debug.Assert(this.nOverflow==0);
				Debug.Assert(sqlite3_mutex_held(this.pBt.mutex));
				Debug.Assert(nCell>=0&&nCell<=MX_CELL(this.pBt)&&MX_CELL(this.pBt)<=5460);
				Debug.Assert(sqlite3PagerIswriteable(this.pDbPage));
				/* Check that the page has just been zeroed by zeroPage() */Debug.Assert(this.nCell==0);
				Debug.Assert(get2byte(data,hdr+5)==nUsable);
				pCellptr=this.cellOffset+nCell*2;
				//data[pPage.cellOffset + nCell * 2];
				cellbody=nUsable;
				for(i=nCell-1;i>=0;i--) {
					pCellptr-=2;
					cellbody-=aSize[i+offset];
					put2byte(data,pCellptr,cellbody);
					Buffer.BlockCopy(apCell[offset+i],0,data,cellbody,aSize[i+offset]);
					//          memcpy(&data[cellbody], apCell[i], aSize[i]);
				}
				put2byte(data,hdr+3,nCell);
				put2byte(data,hdr+5,cellbody);
				this.nFree-=(u16)(nCell*2+nUsable-cellbody);
				this.nCell=(u16)nCell;
			}
			public void assemblePage(/* The page to be assemblied */int nCell,/* The number of cells to add to this page */u8[] apCell,/* Pointers to cell bodies */u16[] aSize/* Sizes of the cells */) {
				int i;
				/* Loop counter */int pCellptr;
				/* Address of next cell pointer */int cellbody;
				/* Address of next cell body */u8[] data=this.aData;
				/* Pointer to data for pPage */int hdr=this.hdrOffset;
				/* Offset of header on pPage */int nUsable=(int)this.pBt.usableSize;
				/* Usable size of page */Debug.Assert(this.nOverflow==0);
				Debug.Assert(sqlite3_mutex_held(this.pBt.mutex));
				Debug.Assert(nCell>=0&&nCell<=MX_CELL(this.pBt)&&MX_CELL(this.pBt)<=5460);
				Debug.Assert(sqlite3PagerIswriteable(this.pDbPage));
				/* Check that the page has just been zeroed by zeroPage() */Debug.Assert(this.nCell==0);
				Debug.Assert(get2byte(data,hdr+5)==nUsable);
				pCellptr=this.cellOffset+nCell*2;
				//&data[pPage.cellOffset + nCell * 2];
				cellbody=nUsable;
				for(i=nCell-1;i>=0;i--) {
					pCellptr-=2;
					cellbody-=aSize[i];
					put2byte(data,pCellptr,cellbody);
					Buffer.BlockCopy(apCell,0,data,cellbody,aSize[i]);
					//memcpy( data[cellbody], apCell[i], aSize[i] );
				}
				put2byte(data,hdr+3,nCell);
				put2byte(data,hdr+5,cellbody);
				this.nFree-=(u16)(nCell*2+nUsable-cellbody);
				this.nCell=(u16)nCell;
			}
			/**
///<summary>
///This version of balance() handles the common special case where
///</summary>
///<param name="a new entry is being inserted on the extreme right">end of the</param>
///<param name="tree, in other words, when the new entry will become the largest">tree, in other words, when the new entry will become the largest</param>
///<param name="entry in the tree.">entry in the tree.</param>
///<param name=""></param>
///<param name="Instead of trying to balance the 3 right">most leaf pages, just add</param>
///<param name="a new page to the right">hand side and put the one new entry in</param>
///<param name="that page.  This leaves the right side of the tree somewhat">that page.  This leaves the right side of the tree somewhat</param>
///<param name="unbalanced.  But odds are that we will be inserting new entries">unbalanced.  But odds are that we will be inserting new entries</param>
///<param name="at the end soon afterwards so the nearly empty page will quickly">at the end soon afterwards so the nearly empty page will quickly</param>
///<param name="fill up.  On average.">fill up.  On average.</param>
///<param name=""></param>
///<param name="pPage is the leaf page which is the right">most page in the tree.</param>
///<param name="pParent is its parent.  pPage must have a single overflow entry">pParent is its parent.  pPage must have a single overflow entry</param>
///<param name="which is also the right">most entry on the page.</param>
///<param name=""></param>
///<param name="The pSpace buffer is used to store a temporary copy of the divider">The pSpace buffer is used to store a temporary copy of the divider</param>
///<param name="cell that will be inserted into pParent. Such a cell consists of a 4">cell that will be inserted into pParent. Such a cell consists of a 4</param>
///<param name="byte page number followed by a variable length integer. In other">byte page number followed by a variable length integer. In other</param>
///<param name="words, at most 13 bytes. Hence the pSpace buffer must be at">words, at most 13 bytes. Hence the pSpace buffer must be at</param>
///<param name="least 13 bytes in size.">least 13 bytes in size.</param>
*/public int balance_quick(MemPage pPage,u8[] pSpace) {
				BtShared pBt=pPage.pBt;
				/* B-Tree Database */MemPage pNew=new MemPage();
				/* Newly allocated page */int rc;
				/* Return Code */Pgno pgnoNew=0;
				/* Page number of pNew */Debug.Assert(sqlite3_mutex_held(pPage.pBt.mutex));
				Debug.Assert(sqlite3PagerIswriteable(this.pDbPage));
				Debug.Assert(pPage.nOverflow==1);
				/* This error condition is now caught prior to reaching this function */if(pPage.nCell<=0)
					return SQLITE_CORRUPT_BKPT();
				/* Allocate a new page. This page will become the right-sibling of
  ** pPage. Make the parent page writable, so that the new divider cell
  ** may be inserted. If both these operations are successful, proceed.
  */rc=allocateBtreePage(pBt,ref pNew,ref pgnoNew,0,0);
				if(rc==SQLITE_OK) {
					int pOut=4;
					//u8 pOut = &pSpace[4];
					u8[] pCell=pPage.aOvfl[0].pCell;
					int[] szCell=new int[1];
					szCell[0]=pPage.cellSizePtr(pCell);
					int pStop;
					Debug.Assert(sqlite3PagerIswriteable(pNew.pDbPage));
					Debug.Assert(pPage.aData[0]==(PTF_INTKEY|PTF_LEAFDATA|PTF_LEAF));
					pNew.zeroPage(PTF_INTKEY|PTF_LEAFDATA|PTF_LEAF);
					pNew.assemblePage(1,pCell,szCell);
					/* If this is an auto-vacuum database, update the pointer map
    ** with entries for the new page, and any pointer from the
    ** cell on the page to an overflow page. If either of these
    ** operations fails, the return code is set, but the contents
    ** of the parent page are still manipulated by thh code below.
    ** That is Ok, at this point the parent page is guaranteed to
    ** be marked as dirty. Returning an error code will cause a
    ** rollback, undoing any changes made to the parent page.
    */
					#if !SQLITE_OMIT_AUTOVACUUM
					if(pBt.autoVacuum)
					#else
																					if (false)
#endif
					 {
						ptrmapPut(pBt,pgnoNew,PTRMAP_BTREE,this.pgno,ref rc);
						if(szCell[0]>pNew.minLocal) {
							pNew.ptrmapPutOvflPtr(pCell,ref rc);
						}
					}
					/* Create a divider cell to insert into pParent. The divider cell
    ** consists of a 4-byte page number (the page number of pPage) and
    ** a variable length key value (which must be the same value as the
    ** largest key on pPage).
    **
    ** To find the largest key value on pPage, first find the right-most
    ** cell on pPage. The first two fields of this cell are the
    ** record-length (a variable length integer at most 32-bits in size)
    ** and the key value (a variable length integer, may have any value).
    ** The first of the while(...) loops below skips over the record-length
    ** field. The second while(...) loop copies the key value from the
    ** cell on pPage into the pSpace buffer.
    */int iCell=findCell(pPage,pPage.nCell-1);
					//pCell = findCell( pPage, pPage.nCell - 1 );
					pCell=pPage.aData;
					int _pCell=iCell;
					pStop=_pCell+9;
					//pStop = &pCell[9];
					while(((pCell[_pCell++])&0x80)!=0&&_pCell<pStop)
						;
					//while ( ( *( pCell++ ) & 0x80 ) && pCell < pStop ) ;
					pStop=_pCell+9;
					//pStop = &pCell[9];
					while(((pSpace[pOut++]=pCell[_pCell++])&0x80)!=0&&_pCell<pStop)
						;
					//while ( ( ( *( pOut++ ) = *( pCell++ ) ) & 0x80 ) && pCell < pStop ) ;
					/* Insert the new divider cell into pParent. */this.insertCell(this.nCell,pSpace,pOut,//(int)(pOut-pSpace),
					null,pPage.pgno,ref rc);
					/* Set the right-child pointer of pParent to point to the new page. */Converter.sqlite3Put4byte(this.aData,this.hdrOffset+8,pgnoNew);
					/* Release the reference to the new page. */releasePage(pNew);
				}
				return rc;
			}
			/**
///<summary>
///</summary>
///<param name="This function is used to copy the contents of the b">tree node stored</param>
///<param name="on page pFrom to page pTo. If page pFrom was not a leaf page, then">on page pFrom to page pTo. If page pFrom was not a leaf page, then</param>
///<param name="the pointer">map entries for each child page are updated so that the</param>
///<param name="parent page stored in the pointer map is page pTo. If pFrom contained">parent page stored in the pointer map is page pTo. If pFrom contained</param>
///<param name="any cells with overflow page pointers, then the corresponding pointer">any cells with overflow page pointers, then the corresponding pointer</param>
///<param name="map entries are also updated so that the parent page is page pTo.">map entries are also updated so that the parent page is page pTo.</param>
///<param name=""></param>
///<param name="If pFrom is currently carrying any overflow cells (entries in the">If pFrom is currently carrying any overflow cells (entries in the</param>
///<param name="MemPage.aOvfl[] array), they are not copied to pTo.">MemPage.aOvfl[] array), they are not copied to pTo.</param>
///<param name=""></param>
///<param name="Before returning, page pTo is reinitialized using btreeInitPage().">Before returning, page pTo is reinitialized using btreeInitPage().</param>
///<param name=""></param>
///<param name="The performance of this function is not critical. It is only used by">The performance of this function is not critical. It is only used by</param>
///<param name="the balance_shallower() and balance_deeper() procedures, neither of">the balance_shallower() and balance_deeper() procedures, neither of</param>
///<param name="which are called often under normal circumstances.">which are called often under normal circumstances.</param>
*/public void copyNodeContent(MemPage pTo,ref int pRC) {
				if((pRC)==SQLITE_OK) {
					BtShared pBt=this.pBt;
					u8[] aFrom=this.aData;
					u8[] aTo=pTo.aData;
					int iFromHdr=this.hdrOffset;
					int iToHdr=((pTo.pgno==1)?100:0);
					int rc;
					int iData;
					Debug.Assert(this.isInit!=0);
					Debug.Assert(this.nFree>=iToHdr);
					Debug.Assert(get2byte(aFrom,iFromHdr+5)<=(int)pBt.usableSize);
					/* Copy the b-tree node content from page pFrom to page pTo. */iData=get2byte(aFrom,iFromHdr+5);
					Buffer.BlockCopy(aFrom,iData,aTo,iData,(int)pBt.usableSize-iData);
					//memcpy(aTo[iData], ref aFrom[iData], pBt.usableSize-iData);
					Buffer.BlockCopy(aFrom,iFromHdr,aTo,iToHdr,this.cellOffset+2*this.nCell);
					//memcpy(aTo[iToHdr], ref aFrom[iFromHdr], pFrom.cellOffset + 2*pFrom.nCell);
					/* Reinitialize page pTo so that the contents of the MemPage structure
    ** match the new data. The initialization of pTo can actually fail under
    ** fairly obscure circumstances, even though it is a copy of initialized 
    ** page pFrom.
    */pTo.isInit=0;
					rc=pTo.btreeInitPage();
					if(rc!=SQLITE_OK) {
						pRC=rc;
						return;
					}
					/* If this is an auto-vacuum database, update the pointer-map entries
    ** for any b-tree or overflow pages that pTo now contains the pointers to.
    */
					#if !SQLITE_OMIT_AUTOVACUUM
					if(pBt.autoVacuum)
					#else
																					if (false)
#endif
					 {
						pRC=pTo.setChildPtrmaps();
					}
				}
			}
			// under C#; Try to reuse Memory
			public int balance_nonroot(/* Parent page of siblings being balanced */int iParentIdx,/* Index of "the page" in pParent */u8[] aOvflSpace,/* page-size bytes of space for parent ovfl */int isRoot/* True if pParent is a root-page */) {
				MemPage[] apOld=new MemPage[NB];
				/* pPage and up to two siblings */MemPage[] apCopy=new MemPage[NB];
				/* Private copies of apOld[] pages */MemPage[] apNew=new MemPage[NB+2];
				/* pPage and up to NB siblings after balancing */int[] apDiv=new int[NB-1];
				/* Divider cells in pParent */int[] cntNew=new int[NB+2];
				/* Index in aCell[] of cell after i-th page */int[] szNew=new int[NB+2];
				/* Combined size of cells place on i-th page */u16[] szCell=new u16[1];
				/* Local size of all cells in apCell[] */BtShared pBt;
				/* The whole database */int nCell=0;
				/* Number of cells in apCell[] */int nMaxCells=0;
				/* Allocated size of apCell, szCell, aFrom. */int nNew=0;
				/* Number of pages in apNew[] */int nOld;
				/* Number of pages in apOld[] */int i,j,k;
				/* Loop counters */int nxDiv;
				/* Next divider slot in pParent.aCell[] */int rc=SQLITE_OK;
				/* The return code */u16 leafCorrection;
				/* 4 if pPage is a leaf.  0 if not */int leafData;
				/* True if pPage is a leaf of a LEAFDATA tree */int usableSpace;
				/* Bytes in pPage beyond the header */int pageFlags;
				/* Value of pPage.aData[0] */int subtotal;
				/* Subtotal of bytes in cells on one page *///int iSpace1 = 0;             /* First unused byte of aSpace1[] */
				int iOvflSpace=0;
				/* First unused byte of aOvflSpace[] */int szScratch;
				/* Size of scratch memory requested */int pRight;
				/* Location in parent of right-sibling pointer */u8[][] apCell=null;
				/* All cells begin balanced *///u16[] szCell;                         /* Local size of all cells in apCell[] */
				//u8[] aSpace1;                         /* Space for copies of dividers cells */
				Pgno pgno;
				/* Temp var to store a page number in */pBt=this.pBt;
				Debug.Assert(sqlite3_mutex_held(pBt.mutex));
				Debug.Assert(sqlite3PagerIswriteable(this.pDbPage));
				#if FALSE
																TRACE("BALANCE: begin page %d child of %d\n", pPage.pgno, pParent.pgno);
#endif
				/* At this point pParent may have at most one overflow cell. And if
** this overflow cell is present, it must be the cell with
** index iParentIdx. This scenario comes about when this function
** is called (indirectly) from sqlite3BtreeDelete().
*/Debug.Assert(this.nOverflow==0||this.nOverflow==1);
				Debug.Assert(this.nOverflow==0||this.aOvfl[0].idx==iParentIdx);
				//if( !aOvflSpace ){
				//  return SQLITE_NOMEM;
				//}
				/* Find the sibling pages to balance. Also locate the cells in pParent
  ** that divide the siblings. An attempt is made to find NN siblings on
  ** either side of pPage. More siblings are taken from one side, however,
  ** if there are fewer than NN siblings on the other side. If pParent
  ** has NB or fewer children then all children of pParent are taken.
  **
  ** This loop also drops the divider cells from the parent page. This
  ** way, the remainder of the function does not have to deal with any
  ** overflow cells in the parent page, since if any existed they will
  ** have already been removed.
  */i=this.nOverflow+this.nCell;
				if(i<2) {
					nxDiv=0;
					nOld=i+1;
				}
				else {
					nOld=3;
					if(iParentIdx==0) {
						nxDiv=0;
					}
					else
						if(iParentIdx==i) {
							nxDiv=i-2;
						}
						else {
							nxDiv=iParentIdx-1;
						}
					i=2;
				}
				if((i+nxDiv-this.nOverflow)==this.nCell) {
					pRight=this.hdrOffset+8;
					//&pParent.aData[pParent.hdrOffset + 8];
				}
				else {
					pRight=findCell(this,i+nxDiv-this.nOverflow);
				}
				pgno=Converter.sqlite3Get4byte(this.aData,pRight);
				while(true) {
					rc=getAndInitPage(pBt,pgno,ref apOld[i]);
					if(rc!=0) {
						//memset(apOld, 0, (i+1)*sizeof(MemPage*));
						goto balance_cleanup;
					}
					nMaxCells+=1+apOld[i].nCell+apOld[i].nOverflow;
					if((i--)==0)
						break;
					if(i+nxDiv==this.aOvfl[0].idx&&this.nOverflow!=0) {
						apDiv[i]=0;
						// = pParent.aOvfl[0].pCell;
						pgno=Converter.sqlite3Get4byte(this.aOvfl[0].pCell,apDiv[i]);
						szNew[i]=this.cellSizePtr(apDiv[i]);
						this.nOverflow=0;
					}
					else {
						apDiv[i]=findCell(this,i+nxDiv-this.nOverflow);
						pgno=Converter.sqlite3Get4byte(this.aData,apDiv[i]);
						szNew[i]=this.cellSizePtr(apDiv[i]);
						/* Drop the cell from the parent page. apDiv[i] still points to
      ** the cell within the parent, even though it has been dropped.
      ** This is safe because dropping a cell only overwrites the first
      ** four bytes of it, and this function does not need the first
      ** four bytes of the divider cell. So the pointer is safe to use
      ** later on.
      **
      ** Unless SQLite is compiled in secure-delete mode. In this case,
      ** the dropCell() routine will overwrite the entire cell with zeroes.
      ** In this case, temporarily copy the cell into the aOvflSpace[]
      ** buffer. It will be copied out again as soon as the aSpace[] buffer
      ** is allocated.  *///if (pBt.secureDelete)
						//{
						//  int iOff = (int)(apDiv[i]) - (int)(pParent.aData); //SQLITE_PTR_TO_INT(apDiv[i]) - SQLITE_PTR_TO_INT(pParent.aData);
						//         if( (iOff+szNew[i])>(int)pBt->usableSize )
						//  {
						//    rc = SQLITE_CORRUPT_BKPT();
						//    Array.Clear(apOld[0].aData,0,apOld[0].aData.Length); //memset(apOld, 0, (i + 1) * sizeof(MemPage*));
						//    goto balance_cleanup;
						//  }
						//  else
						//  {
						//    memcpy(&aOvflSpace[iOff], apDiv[i], szNew[i]);
						//    apDiv[i] = &aOvflSpace[apDiv[i] - pParent.aData];
						//  }
						//}
						this.dropCell(i+nxDiv-this.nOverflow,szNew[i],ref rc);
					}
				}
				/* Make nMaxCells a multiple of 4 in order to preserve 8-byte
  ** alignment */nMaxCells=(nMaxCells+3)&~3;
				/*
  ** Allocate space for memory structures
  *///k = pBt.pageSize + ROUND8(sizeof(MemPage));
				//szScratch =
				//     nMaxCells*sizeof(u8*)                       /* apCell */
				//   + nMaxCells*sizeof(u16)                       /* szCell */
				//   + pBt.pageSize                               /* aSpace1 */
				//   + k*nOld;                                     /* Page copies (apCopy) */
				apCell=sqlite3ScratchMalloc(apCell,nMaxCells);
				//if( apCell==null ){
				//  rc = SQLITE_NOMEM;
				//  goto balance_cleanup;
				//}
				if(szCell.Length<nMaxCells)
					Array.Resize(ref szCell,nMaxCells);
				//(u16*)&apCell[nMaxCells];
				//aSpace1 = new byte[pBt.pageSize * (nMaxCells)];//  aSpace1 = (u8*)&szCell[nMaxCells];
				//Debug.Assert( EIGHT_BYTE_ALIGNMENT(aSpace1) );
				/*
  ** Load pointers to all cells on sibling pages and the divider cells
  ** into the local apCell[] array.  Make copies of the divider cells
  ** into space obtained from aSpace1[] and remove the the divider Cells
  ** from pParent.
  **
  ** If the siblings are on leaf pages, then the child pointers of the
  ** divider cells are stripped from the cells before they are copied
  ** into aSpace1[].  In this way, all cells in apCell[] are without
  ** child pointers.  If siblings are not leaves, then all cell in
  ** apCell[] include child pointers.  Either way, all cells in apCell[]
  ** are alike.
  **
  ** leafCorrection:  4 if pPage is a leaf.  0 if pPage is not a leaf.
  **       leafData:  1 if pPage holds key+data and pParent holds only keys.
  */leafCorrection=(u16)(apOld[0].leaf*4);
				leafData=apOld[0].hasData;
				for(i=0;i<nOld;i++) {
					int limit;
					/* Before doing anything else, take a copy of the i'th original sibling
    ** The rest of this function will use data from the copies rather
    ** that the original pages since the original pages will be in the
    ** process of being overwritten.  *///MemPage pOld = apCopy[i] = (MemPage*)&aSpace1[pBt.pageSize + k*i];
					//memcpy(pOld, apOld[i], sizeof(MemPage));
					//pOld.aData = (void*)&pOld[1];
					//memcpy(pOld.aData, apOld[i].aData, pBt.pageSize);
					MemPage pOld=apCopy[i]=apOld[i].Copy();
					limit=pOld.nCell+pOld.nOverflow;
					if(pOld.nOverflow>0||true) {
						for(j=0;j<limit;j++) {
							Debug.Assert(nCell<nMaxCells);
							//apCell[nCell] = findOverflowCell( pOld, j );
							//szCell[nCell] = cellSizePtr( pOld, apCell, nCell );
							int iFOFC=pOld.findOverflowCell(j);
							szCell[nCell]=pOld.cellSizePtr(iFOFC);
							// Copy the Data Locally
							if(apCell[nCell]==null)
								apCell[nCell]=new u8[szCell[nCell]];
							else
								if(apCell[nCell].Length<szCell[nCell])
									Array.Resize(ref apCell[nCell],szCell[nCell]);
							if(iFOFC<0)
								// Overflow Cell
								Buffer.BlockCopy(pOld.aOvfl[-(iFOFC+1)].pCell,0,apCell[nCell],0,szCell[nCell]);
							else
								Buffer.BlockCopy(pOld.aData,iFOFC,apCell[nCell],0,szCell[nCell]);
							nCell++;
						}
					}
					else {
						u8[] aData=pOld.aData;
						u16 maskPage=pOld.maskPage;
						u16 cellOffset=pOld.cellOffset;
						for(j=0;j<limit;j++) {
							Debugger.Break();
							Debug.Assert(nCell<nMaxCells);
							apCell[nCell]=findCellv2(aData,maskPage,cellOffset,j);
							szCell[nCell]=pOld.cellSizePtr(apCell[nCell]);
							nCell++;
						}
					}
					if(i<nOld-1&&0==leafData) {
						u16 sz=(u16)szNew[i];
						byte[] pTemp=sqlite3Malloc(sz+leafCorrection);
						Debug.Assert(nCell<nMaxCells);
						szCell[nCell]=sz;
						//pTemp = &aSpace1[iSpace1];
						//iSpace1 += sz;
						Debug.Assert(sz<=pBt.maxLocal+23);
						//Debug.Assert(iSpace1 <= (int)pBt.pageSize);
						Buffer.BlockCopy(this.aData,apDiv[i],pTemp,0,sz);
						//memcpy( pTemp, apDiv[i], sz );
						if(apCell[nCell]==null||apCell[nCell].Length<sz)
							Array.Resize(ref apCell[nCell],sz);
						Buffer.BlockCopy(pTemp,leafCorrection,apCell[nCell],0,sz);
						//apCell[nCell] = pTemp + leafCorrection;
						Debug.Assert(leafCorrection==0||leafCorrection==4);
						szCell[nCell]=(u16)(szCell[nCell]-leafCorrection);
						if(0==pOld.leaf) {
							Debug.Assert(leafCorrection==0);
							Debug.Assert(pOld.hdrOffset==0);
							/* The right pointer of the child page pOld becomes the left
        ** pointer of the divider cell */Buffer.BlockCopy(pOld.aData,8,apCell[nCell],0,4);
							//memcpy( apCell[nCell], ref pOld.aData[8], 4 );
						}
						else {
							Debug.Assert(leafCorrection==4);
							if(szCell[nCell]<4) {
								/* Do not allow any cells smaller than 4 bytes. */szCell[nCell]=4;
							}
						}
						nCell++;
					}
				}
				/*
  ** Figure out the number of pages needed to hold all nCell cells.
  ** Store this number in "k".  Also compute szNew[] which is the total
  ** size of all cells on the i-th page and cntNew[] which is the index
  ** in apCell[] of the cell that divides page i from page i+1.
  ** cntNew[k] should equal nCell.
  **
  ** Values computed by this block:
  **
  **           k: The total number of sibling pages
  **    szNew[i]: Spaced used on the i-th sibling page.
  **   cntNew[i]: Index in apCell[] and szCell[] for the first cell to
  **              the right of the i-th sibling page.
  ** usableSpace: Number of bytes of space available on each sibling.
  **
  */usableSpace=(int)pBt.usableSize-12+leafCorrection;
				for(subtotal=k=i=0;i<nCell;i++) {
					Debug.Assert(i<nMaxCells);
					subtotal+=szCell[i]+2;
					if(subtotal>usableSpace) {
						szNew[k]=subtotal-szCell[i];
						cntNew[k]=i;
						if(leafData!=0) {
							i--;
						}
						subtotal=0;
						k++;
						if(k>NB+1) {
							rc=SQLITE_CORRUPT_BKPT();
							goto balance_cleanup;
						}
					}
				}
				szNew[k]=subtotal;
				cntNew[k]=nCell;
				k++;
				/*
  ** The packing computed by the previous block is biased toward the siblings
  ** on the left side.  The left siblings are always nearly full, while the
  ** right-most sibling might be nearly empty.  This block of code attempts
  ** to adjust the packing of siblings to get a better balance.
  **
  ** This adjustment is more than an optimization.  The packing above might
  ** be so out of balance as to be illegal.  For example, the right-most
  ** sibling might be completely empty.  This adjustment is not optional.
  */for(i=k-1;i>0;i--) {
					int szRight=szNew[i];
					/* Size of sibling on the right */int szLeft=szNew[i-1];
					/* Size of sibling on the left */int r;
					/* Index of right-most cell in left sibling */int d;
					/* Index of first cell to the left of right sibling */r=cntNew[i-1]-1;
					d=r+1-leafData;
					Debug.Assert(d<nMaxCells);
					Debug.Assert(r<nMaxCells);
					while(szRight==0||szRight+szCell[d]+2<=szLeft-(szCell[r]+2)) {
						szRight+=szCell[d]+2;
						szLeft-=szCell[r]+2;
						cntNew[i-1]--;
						r=cntNew[i-1]-1;
						d=r+1-leafData;
					}
					szNew[i]=szRight;
					szNew[i-1]=szLeft;
				}
				/* Either we found one or more cells (cntnew[0])>0) or pPage is
  ** a virtual root page.  A virtual root page is when the real root
  ** page is page 1 and we are the only child of that page.
  */Debug.Assert(cntNew[0]>0||(this.pgno==1&&this.nCell==0));
				TRACE("BALANCE: old: %d %d %d  ",apOld[0].pgno,nOld>=2?apOld[1].pgno:0,nOld>=3?apOld[2].pgno:0);
				/*
  ** Allocate k new pages.  Reuse old pages where possible.
  */if(apOld[0].pgno<=1) {
					rc=SQLITE_CORRUPT_BKPT();
					goto balance_cleanup;
				}
				pageFlags=apOld[0].aData[0];
				for(i=0;i<k;i++) {
					MemPage pNew=new MemPage();
					if(i<nOld) {
						pNew=apNew[i]=apOld[i];
						apOld[i]=null;
						rc=sqlite3PagerWrite(pNew.pDbPage);
						nNew++;
						if(rc!=0)
							goto balance_cleanup;
					}
					else {
						Debug.Assert(i>0);
						rc=allocateBtreePage(pBt,ref pNew,ref pgno,pgno,0);
						if(rc!=0)
							goto balance_cleanup;
						apNew[i]=pNew;
						nNew++;
						/* Set the pointer-map entry for the new sibling page. */
						#if !SQLITE_OMIT_AUTOVACUUM
						if(pBt.autoVacuum)
						#else
																										if (false)
#endif
						 {
							ptrmapPut(pBt,pNew.pgno,PTRMAP_BTREE,this.pgno,ref rc);
							if(rc!=SQLITE_OK) {
								goto balance_cleanup;
							}
						}
					}
				}
				/* Free any old pages that were not reused as new pages.
  */while(i<nOld) {
					freePage(apOld[i],ref rc);
					if(rc!=0)
						goto balance_cleanup;
					releasePage(apOld[i]);
					apOld[i]=null;
					i++;
				}
				/*
  ** Put the new pages in accending order.  This helps to
  ** keep entries in the disk file in order so that a scan
  ** of the table is a linear scan through the file.  That
  ** in turn helps the operating system to deliver pages
  ** from the disk more rapidly.
  **
  ** An O(n^2) insertion sort algorithm is used, but since
  ** n is never more than NB (a small constant), that should
  ** not be a problem.
  **
  ** When NB==3, this one optimization makes the database
  ** about 25% faster for large insertions and deletions.
  */for(i=0;i<k-1;i++) {
					int minV=(int)apNew[i].pgno;
					int minI=i;
					for(j=i+1;j<k;j++) {
						if(apNew[j].pgno<(u32)minV) {
							minI=j;
							minV=(int)apNew[j].pgno;
						}
					}
					if(minI>i) {
						MemPage pT;
						pT=apNew[i];
						apNew[i]=apNew[minI];
						apNew[minI]=pT;
					}
				}
				TRACE("new: %d(%d) %d(%d) %d(%d) %d(%d) %d(%d)\n",apNew[0].pgno,szNew[0],nNew>=2?apNew[1].pgno:0,nNew>=2?szNew[1]:0,nNew>=3?apNew[2].pgno:0,nNew>=3?szNew[2]:0,nNew>=4?apNew[3].pgno:0,nNew>=4?szNew[3]:0,nNew>=5?apNew[4].pgno:0,nNew>=5?szNew[4]:0);
				Debug.Assert(sqlite3PagerIswriteable(this.pDbPage));
				Converter.sqlite3Put4byte(this.aData,pRight,apNew[nNew-1].pgno);
				/*
  ** Evenly distribute the data in apCell[] across the new pages.
  ** Insert divider cells into pParent as necessary.
  */j=0;
				for(i=0;i<nNew;i++) {
					/* Assemble the new sibling page. */MemPage pNew=apNew[i];
					Debug.Assert(j<nMaxCells);
					pNew.zeroPage(pageFlags);
					pNew.assemblePage(cntNew[i]-j,apCell,szCell,j);
					Debug.Assert(pNew.nCell>0||(nNew==1&&cntNew[0]==0));
					Debug.Assert(pNew.nOverflow==0);
					j=cntNew[i];
					/* If the sibling page assembled above was not the right-most sibling,
    ** insert a divider cell into the parent page.
    */Debug.Assert(i<nNew-1||j==nCell);
					if(j<nCell) {
						u8[] pCell;
						u8[] pTemp;
						int sz;
						Debug.Assert(j<nMaxCells);
						pCell=apCell[j];
						sz=szCell[j]+leafCorrection;
						pTemp=sqlite3Malloc(sz);
						//&aOvflSpace[iOvflSpace];
						if(0==pNew.leaf) {
							Buffer.BlockCopy(pCell,0,pNew.aData,8,4);
							//memcpy( pNew.aData[8], pCell, 4 );
						}
						else
							if(leafData!=0) {
								/* If the tree is a leaf-data tree, and the siblings are leaves,
        ** then there is no divider cell in apCell[]. Instead, the divider
        ** cell consists of the integer key for the right-most cell of
        ** the sibling-page assembled above only.
        */CellInfo info=new CellInfo();
								j--;
								pNew.btreeParseCellPtr(apCell[j],ref info);
								pCell=pTemp;
								sz=4+putVarint(pCell,4,(u64)info.nKey);
								pTemp=null;
							}
							else {
								//------------ pCell -= 4;
								byte[] _pCell_4=sqlite3Malloc(pCell.Length+4);
								Buffer.BlockCopy(pCell,0,_pCell_4,4,pCell.Length);
								pCell=_pCell_4;
								//
								/* Obscure case for non-leaf-data trees: If the cell at pCell was
        ** previously stored on a leaf node, and its reported size was 4
        ** bytes, then it may actually be smaller than this
        ** (see btreeParseCellPtr(), 4 bytes is the minimum size of
        ** any cell). But it is important to pass the correct size to
        ** insertCell(), so reparse the cell now.
        **
        ** Note that this can never happen in an SQLite data file, as all
        ** cells are at least 4 bytes. It only happens in b-trees used
        ** to evaluate "IN (SELECT ...)" and similar clauses.
        */if(szCell[j]==4) {
									Debug.Assert(leafCorrection==4);
									sz=this.cellSizePtr(pCell);
								}
							}
						iOvflSpace+=sz;
						Debug.Assert(sz<=pBt.maxLocal+23);
						Debug.Assert(iOvflSpace<=(int)pBt.pageSize);
						this.insertCell(nxDiv,pCell,sz,pTemp,pNew.pgno,ref rc);
						if(rc!=SQLITE_OK)
							goto balance_cleanup;
						Debug.Assert(sqlite3PagerIswriteable(this.pDbPage));
						j++;
						nxDiv++;
					}
				}
				Debug.Assert(j==nCell);
				Debug.Assert(nOld>0);
				Debug.Assert(nNew>0);
				if((pageFlags&PTF_LEAF)==0) {
					Buffer.BlockCopy(apCopy[nOld-1].aData,8,apNew[nNew-1].aData,8,4);
					//u8* zChild = &apCopy[nOld - 1].aData[8];
					//memcpy( apNew[nNew - 1].aData[8], zChild, 4 );
				}
				if(isRoot!=0&&this.nCell==0&&this.hdrOffset<=apNew[0].nFree) {
					/* The root page of the b-tree now contains no cells. The only sibling
    ** page is the right-child of the parent. Copy the contents of the
    ** child page into the parent, decreasing the overall height of the
    ** b-tree structure by one. This is described as the "balance-shallower"
    ** sub-algorithm in some documentation.
    **
    ** If this is an auto-vacuum database, the call to copyNodeContent()
    ** sets all pointer-map entries corresponding to database image pages
    ** for which the pointer is stored within the content being copied.
    **
    ** The second Debug.Assert below verifies that the child page is defragmented
    ** (it must be, as it was just reconstructed using assemblePage()). This
    ** is important if the parent page happens to be page 1 of the database
    ** image.  */Debug.Assert(nNew==1);
					Debug.Assert(apNew[0].nFree==(get2byte(apNew[0].aData,5)-apNew[0].cellOffset-apNew[0].nCell*2));
					apNew[0].copyNodeContent(this,ref rc);
					freePage(apNew[0],ref rc);
				}
				else
					#if !SQLITE_OMIT_AUTOVACUUM
					if(pBt.autoVacuum)
					#else
																					if (false)
#endif
					 {
						/* Fix the pointer-map entries for all the cells that were shifted around.
      ** There are several different types of pointer-map entries that need to
      ** be dealt with by this routine. Some of these have been set already, but
      ** many have not. The following is a summary:
      **
      **   1) The entries associated with new sibling pages that were not
      **      siblings when this function was called. These have already
      **      been set. We don't need to worry about old siblings that were
      **      moved to the free-list - the freePage() code has taken care
      **      of those.
      **
      **   2) The pointer-map entries associated with the first overflow
      **      page in any overflow chains used by new divider cells. These
      **      have also already been taken care of by the insertCell() code.
      **
      **   3) If the sibling pages are not leaves, then the child pages of
      **      cells stored on the sibling pages may need to be updated.
      **
      **   4) If the sibling pages are not internal intkey nodes, then any
      **      overflow pages used by these cells may need to be updated
      **      (internal intkey nodes never contain pointers to overflow pages).
      **
      **   5) If the sibling pages are not leaves, then the pointer-map
      **      entries for the right-child pages of each sibling may need
      **      to be updated.
      **
      ** Cases 1 and 2 are dealt with above by other code. The next
      ** block deals with cases 3 and 4 and the one after that, case 5. Since
      ** setting a pointer map entry is a relatively expensive operation, this
      ** code only sets pointer map entries for child or overflow pages that have
      ** actually moved between pages.  */MemPage pNew=apNew[0];
						MemPage pOld=apCopy[0];
						int nOverflow=pOld.nOverflow;
						int iNextOld=pOld.nCell+nOverflow;
						int iOverflow=(nOverflow!=0?pOld.aOvfl[0].idx:-1);
						j=0;
						/* Current 'old' sibling page */k=0;
						/* Current 'new' sibling page */for(i=0;i<nCell;i++) {
							int isDivider=0;
							while(i==iNextOld) {
								/* Cell i is the cell immediately following the last cell on old
          ** sibling page j. If the siblings are not leaf pages of an
          ** intkey b-tree, then cell i was a divider cell. */pOld=apCopy[++j];
								iNextOld=i+(0==leafData?1:0)+pOld.nCell+pOld.nOverflow;
								if(pOld.nOverflow!=0) {
									nOverflow=pOld.nOverflow;
									iOverflow=i+(0==leafData?1:0)+pOld.aOvfl[0].idx;
								}
								isDivider=0==leafData?1:0;
							}
							Debug.Assert(nOverflow>0||iOverflow<i);
							Debug.Assert(nOverflow<2||pOld.aOvfl[0].idx==pOld.aOvfl[1].idx-1);
							Debug.Assert(nOverflow<3||pOld.aOvfl[1].idx==pOld.aOvfl[2].idx-1);
							if(i==iOverflow) {
								isDivider=1;
								if((--nOverflow)>0) {
									iOverflow++;
								}
							}
							if(i==cntNew[k]) {
								/* Cell i is the cell immediately following the last cell on new
          ** sibling page k. If the siblings are not leaf pages of an
          ** intkey b-tree, then cell i is a divider cell.  */pNew=apNew[++k];
								if(0==leafData)
									continue;
							}
							Debug.Assert(j<nOld);
							Debug.Assert(k<nNew);
							/* If the cell was originally divider cell (and is not now) or
        ** an overflow cell, or if the cell was located on a different sibling
        ** page before the balancing, then the pointer map entries associated
        ** with any child or overflow pages need to be updated.  */if(isDivider!=0||pOld.pgno!=pNew.pgno) {
								if(0==leafCorrection) {
									ptrmapPut(pBt,Converter.sqlite3Get4byte(apCell[i]),PTRMAP_BTREE,pNew.pgno,ref rc);
								}
								if(szCell[i]>pNew.minLocal) {
									pNew.ptrmapPutOvflPtr(apCell[i],ref rc);
								}
							}
						}
						if(0==leafCorrection) {
							for(i=0;i<nNew;i++) {
								u32 key=Converter.sqlite3Get4byte(apNew[i].aData,8);
								ptrmapPut(pBt,key,PTRMAP_BTREE,apNew[i].pgno,ref rc);
							}
						}
						#if FALSE
																										/* The ptrmapCheckPages() contains Debug.Assert() statements that verify that
** all pointer map pages are set correctly. This is helpful while
** debugging. This is usually disabled because a corrupt database may
** cause an Debug.Assert() statement to fail.  */
ptrmapCheckPages(apNew, nNew);
ptrmapCheckPages(pParent, 1);
#endif
					}
				Debug.Assert(this.isInit!=0);
				TRACE("BALANCE: finished: old=%d new=%d cells=%d\n",nOld,nNew,nCell);
				/*
** Cleanup before returning.
*/balance_cleanup:
				sqlite3ScratchFree(apCell);
				for(i=0;i<nOld;i++) {
					releasePage(apOld[i]);
				}
				for(i=0;i<nNew;i++) {
					releasePage(apNew[i]);
				}
				return rc;
			}
			/**
///<summary>
///</summary>
///<param name="This function is called when the root page of a b">tree structure is</param>
///<param name="overfull (has one or more overflow pages).">overfull (has one or more overflow pages).</param>
///<param name=""></param>
///<param name="A new child page is allocated and the contents of the current root">A new child page is allocated and the contents of the current root</param>
///<param name="page, including overflow cells, are copied into the child. The root">page, including overflow cells, are copied into the child. The root</param>
///<param name="page is then overwritten to make it an empty page with the right">child</param>
///<param name="pointer pointing to the new page.">pointer pointing to the new page.</param>
///<param name=""></param>
///<param name="Before returning, all pointer">map entries corresponding to pages</param>
///<param name="that the new child">page now contains pointers to are updated. The</param>
///<param name="entry corresponding to the new right">child pointer of the root</param>
///<param name="page is also updated.">page is also updated.</param>
///<param name=""></param>
///<param name="If successful, ppChild is set to contain a reference to the child">If successful, ppChild is set to contain a reference to the child</param>
///<param name="page and SQLITE_OK is returned. In this case the caller is required">page and SQLITE_OK is returned. In this case the caller is required</param>
///<param name="to call releasePage() on ppChild exactly once. If an error occurs,">to call releasePage() on ppChild exactly once. If an error occurs,</param>
///<param name="an error code is returned and ppChild is set to 0.">an error code is returned and ppChild is set to 0.</param>
*/public int balance_deeper(ref MemPage ppChild) {
				int rc;
				/* Return value from subprocedures */MemPage pChild=null;
				/* Pointer to a new child page */Pgno pgnoChild=0;
				/* Page number of the new child page */BtShared pBt=this.pBt;
				/* The BTree */Debug.Assert(this.nOverflow>0);
				Debug.Assert(sqlite3_mutex_held(pBt.mutex));
				/* Make pRoot, the root page of the b-tree, writable. Allocate a new
  ** page that will become the new right-child of pPage. Copy the contents
  ** of the node stored on pRoot into the new child page.
  */rc=sqlite3PagerWrite(this.pDbPage);
				if(rc==SQLITE_OK) {
					rc=allocateBtreePage(pBt,ref pChild,ref pgnoChild,this.pgno,0);
					this.copyNodeContent(pChild,ref rc);
					#if !SQLITE_OMIT_AUTOVACUUM
					if(pBt.autoVacuum)
					#else
																					if (false)
#endif
					 {
						ptrmapPut(pBt,pgnoChild,PTRMAP_BTREE,this.pgno,ref rc);
					}
				}
				if(rc!=0) {
					ppChild=null;
					releasePage(pChild);
					return rc;
				}
				Debug.Assert(sqlite3PagerIswriteable(pChild.pDbPage));
				Debug.Assert(sqlite3PagerIswriteable(this.pDbPage));
				Debug.Assert(pChild.nCell==this.nCell);
				TRACE("BALANCE: copy root %d into %d\n",this.pgno,pChild.pgno);
				/* Copy the overflow cells from pRoot to pChild */Array.Copy(this.aOvfl,pChild.aOvfl,this.nOverflow);
				//memcpy(pChild.aOvfl, pRoot.aOvfl, pRoot.nOverflow*sizeof(pRoot.aOvfl[0]));
				pChild.nOverflow=this.nOverflow;
				/* Zero the contents of pRoot. Then install pChild as the right-child. */this.zeroPage(pChild.aData[0]&~PTF_LEAF);
				Converter.sqlite3Put4byte(this.aData,this.hdrOffset+8,pgnoChild);
				ppChild=pChild;
				return SQLITE_OK;
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
		/* A Btree handle
    **
    ** A database connection contains a pointer to an instance of
    ** this object for every database file that it has open.  This structure
    ** is opaque to the database connection.  The database connection cannot
    ** see the internals of this structure and only deals with pointers to
    ** this structure.
    **
    ** For some database files, the same underlying database cache might be
    ** shared between multiple connections.  In that case, each connection
    ** has it own instance of this object.  But each instance of this object
    ** points to the same BtShared object.  The database cache and the
    ** schema associated with the database file are all contained within
    ** the BtShared object.
    **
    ** All fields in this structure are accessed under sqlite3.mutex.
    ** The pBt pointer itself may not be changed while there exists cursors
    ** in the referenced BtShared that point back to this Btree since those
    ** cursors have to go through this Btree to find their BtShared and
    ** they often do so without holding sqlite3.mutex.
    */public class Btree {
			public sqlite3 db;
			/* The database connection holding this Btree */public BtShared pBt;
			/* Sharable content of this Btree */public u8 inTrans;
			/* TRANS_NONE, TRANS_READ or TRANS_WRITE */public bool sharable;
			/* True if we can share pBt with another db */public bool locked;
			/* True if db currently has pBt locked */public int wantToLock;
			/* Number of nested calls to sqlite3BtreeEnter() */public int nBackup;
			/* Number of backup operations reading this btree */public Btree pNext;
			/* List of other sharable Btrees from the same db */public Btree pPrev;
			/* Back pointer of the same list */
			#if !SQLITE_OMIT_SHARED_CACHE
												BtLock lock;              /* Object used to lock page 1 */
#endif
			/**
///<summary>
///Copy the complete content of pBtFrom into pBtTo.  A transaction
///must be active for both files.
///
///The size of file pTo may be reduced by this operation. If anything
///goes wrong, the transaction on pTo is rolled back. If successful, the
///transaction is committed before returning.
///</summary>
*/public int sqlite3BtreeCopyFile(Btree pFrom) {
				int rc;
				sqlite3_backup b;
				sqlite3BtreeEnter(this);
				sqlite3BtreeEnter(pFrom);
				/* Set up an sqlite3_backup object. sqlite3_backup.pDestDb must be set
  ** to 0. This is used by the implementations of sqlite3_backup_step()
  ** and sqlite3_backup_finish() to detect that they are being called
  ** from this function, not directly by the user.
  */b=new sqlite3_backup();
				// memset( &b, 0, sizeof( b ) );
				b.pSrcDb=pFrom.db;
				b.pSrc=pFrom;
				b.pDest=this;
				b.iNext=1;
				/* 0x7FFFFFFF is the hard limit for the number of pages in a database
  ** file. By passing this as the number of pages to copy to
  ** sqlite3_backup_step(), we can guarantee that the copy finishes
  ** within a single call (unless an error occurs). The Debug.Assert() statement
  ** checks this assumption - (p.rc) should be set to either SQLITE_DONE
  ** or an error code.
  */b.sqlite3_backup_step(0x7FFFFFFF);
				Debug.Assert(b.rc!=SQLITE_OK);
				rc=b.sqlite3_backup_finish();
				if(rc==SQLITE_OK) {
					this.pBt.pageSizeFixed=false;
				}
				sqlite3BtreeLeave(pFrom);
				sqlite3BtreeLeave(this);
				return rc;
			}
		}
		///<summary>
		/// Btree.inTrans may take one of the following values.
		///
		/// If the shared-data extension is enabled, there may be multiple users
		/// of the Btree structure. At most one of these may open a write transaction,
		/// but any number may have active read transactions.
		///
		///</summary>
		const byte TRANS_NONE=0;
		const byte TRANS_READ=1;
		const byte TRANS_WRITE=2;
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
		/* BtShared.pageSize bytes of space for tmp use */};

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
		const int BTCURSOR_MAX_DEPTH=20;
		/*
    ** A cursor is a pointer to a particular entry within a particular
    ** b-tree within a database file.
    **
    ** The entry is identified by its MemPage and the index in
    ** MemPage.aCell[] of the entry.
    **
    ** A single database file can shared by two more database connections,
    ** but cursors cannot be shared.  Each cursor is associated with a
    ** particular database connection identified BtCursor.pBtree.db.
    **
    ** Fields in this structure are accessed under the BtShared.mutex
    ** found at self.pBt.mutex.
    */public class BtCursor {
			public Btree pBtree;
			/* The Btree to which this cursor belongs */public BtShared pBt;
			/* The BtShared this cursor points to */public BtCursor pNext;
			public BtCursor pPrev;
			/* Forms a linked list of all cursors */public KeyInfo pKeyInfo;
			/* Argument passed to comparison function */public Pgno pgnoRoot;
			/* The root page of this tree */public sqlite3_int64 cachedRowid;
			/* Next rowid cache.  0 means not valid */public CellInfo info=new CellInfo();
			/* A parse of the cell we are pointing at */public byte[] pKey;
			/* Saved key that was cursor's last known position */public i64 nKey;
			/* Size of pKey, or last integer key */public int skipNext;
			/* Prev() is noop if negative. Next() is noop if positive */public u8 wrFlag;
			/* True if writable */public u8 atLast;
			/* VdbeCursor pointing to the last entry */public bool validNKey;
			/* True if info.nKey is valid */public int eState;
			/* One of the CURSOR_XXX constants (see below) */
			#if !SQLITE_OMIT_INCRBLOB
															public Pgno[] aOverflow;         /* Cache of overflow page locations */
public bool isIncrblobHandle;   /* True if this cursor is an incr. io handle */
#endif
			public i16 iPage;
			/* Index of current page in apPage */public u16[] aiIdx=new u16[BTCURSOR_MAX_DEPTH];
			///<summary>
			///Current index in apPage[i]
			///</summary>
			public MemPage[] apPage=new MemPage[BTCURSOR_MAX_DEPTH];
			///<summary>
			///Pages from root to current page
			///</summary>
			public void Clear() {
				pNext=null;
				pPrev=null;
				pKeyInfo=null;
				pgnoRoot=0;
				cachedRowid=0;
				info=new CellInfo();
				wrFlag=0;
				atLast=0;
				validNKey=false;
				eState=0;
				pKey=null;
				nKey=0;
				skipNext=0;
				#if !SQLITE_OMIT_INCRBLOB
																				isIncrblobHandle=false;
aOverflow= null;
#endif
				iPage=0;
			}
			public BtCursor Copy() {
				BtCursor cp=(BtCursor)MemberwiseClone();
				return cp;
			}
		};

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
			return ptrmapPageno(pBt,pgno);
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
		/* Accumulate the error message text here */};

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
