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
		public class MemPage
		{
			public MemPage ()
			{
				Log.WriteLine ("page .ctor");
			}

			///
///<summary>
///True if previously initialized. MUST BE FIRST! 
///</summary>

			public u8 isInit;

			///
///<summary>
///Number of overflow cell bodies in aCell[] 
///</summary>

			public u8 nOverflow;

			///
///<summary>
///True if u8key flag is set 
///</summary>

			public u8 intKey;

			///
///<summary>
///1 if leaf flag is set 
///</summary>

			public u8 leaf;

			///
///<summary>
///True if this page stores data 
///</summary>

			public u8 hasData;

			///
///<summary>
///100 for page 1.  0 otherwise 
///</summary>

			public u8 hdrOffset;

			///
///<summary>
///0 if leaf==1.  4 if leaf==0 
///</summary>

			public u8 childPtrSize;

			///
///<summary>
///Copy of BtShared.maxLocal or BtShared.maxLeaf 
///</summary>

			public u16 maxLocal;

			///
///<summary>
///Copy of BtShared.minLocal or BtShared.minLeaf 
///</summary>

			public u16 minLocal;

			///
///<summary>
///Index in aData of first cell pou16er 
///</summary>

			public u16 cellOffset;

			///
///<summary>
///Number of free bytes on the page 
///</summary>

			public u16 nFree;

			///
///<summary>
///Number of cells on this page, local and ovfl 
///</summary>

			public u16 nCell;

			///
///<summary>
///Mask for page offset 
///</summary>

			public u16 maskPage;

			public _OvflCell[] aOvfl = new _OvflCell[5];

			public BtShared pBt;

			///
///<summary>
///Pointer to BtShared that this page is part of 
///</summary>

			public byte[] aData;

			///
///<summary>
///Pointer to disk image of the page data 
///</summary>

			public DbPage pDbPage;

			///
///<summary>
///Pager page handle 
///</summary>

			public Pgno pgno;

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
			public MemPage Copy ()
			{
				MemPage cp = (MemPage)MemberwiseClone ();
				if (aOvfl != null) {
					cp.aOvfl = new _OvflCell[aOvfl.Length];
					for (int i = 0; i < aOvfl.Length; i++)
						cp.aOvfl [i] = aOvfl [i].Copy ();
				}
				if (aData != null) {
					cp.aData = malloc_cs.sqlite3Malloc (aData.Length);
					Buffer.BlockCopy (aData, 0, cp.aData, 0, aData.Length);
				}
				return cp;
			}

			/**
///<summary>
///This a more complex version of findCell() that works for
///pages that do contain overflow cells.
///</summary>
*/public int findOverflowCell (int iCell)
			{
				int i;
				Debug.Assert (this.pBt.mutex.sqlite3_mutex_held());
				for (i = this.nOverflow - 1; i >= 0; i--) {
					int k;
					_OvflCell pOvfl;
					pOvfl = this.aOvfl [i];
					k = pOvfl.idx;
					if (k <= iCell) {
						if (k == iCell) {
							//return pOvfl.pCell;
							return -i - 1;
							// Negative Offset means overflow cells
						}
						iCell--;
					}
				}
				return this.findCell (iCell);
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
			void btreeParseCellPtr (///
///<summary>
///Page containing the cell 
///</summary>

			int iCell, ///
///<summary>
///Pointer to the cell text. 
///</summary>

			ref CellInfo pInfo///
///<summary>
///Fill in this structure 
///</summary>

			)
			{
				this.btreeParseCellPtr (this.aData, iCell, ref pInfo);
			}

			public void btreeParseCellPtr (///
///<summary>
///Page containing the cell 
///</summary>

			byte[] pCell, ///
///<summary>
///The actual data 
///</summary>

			ref CellInfo pInfo///
///<summary>
///Fill in this structure 
///</summary>

			)
			{
				this.btreeParseCellPtr (pCell, 0, ref pInfo);
			}

			public void btreeParseCellPtr (///
///<summary>
///Page containing the cell 
///</summary>

			u8[] pCell, ///
///<summary>
///Pointer to the cell text. 
///</summary>

			int iCell, ///
///<summary>
///Pointer to the cell text. 
///</summary>

			ref CellInfo pInfo///
///<summary>
///Fill in this structure 
///</summary>

			)
			{
				u16 n;
				///
///<summary>
///Number bytes in cell content header 
///</summary>

				u32 nPayload = 0;
				///
///<summary>
///Number of bytes of cell payload 
///</summary>

				Debug.Assert (this.pBt.mutex.sqlite3_mutex_held());
				if (pInfo.pCell != pCell)
					pInfo.pCell = pCell;
				pInfo.iCell = iCell;
				Debug.Assert (this.leaf == 0 || this.leaf == 1);
				n = this.childPtrSize;
				Debug.Assert (n == 4 - 4 * this.leaf);
				if (this.intKey != 0) {
					if (this.hasData != 0) {
                        n += (u16)utilc.getVarint32(pCell, iCell + n, out nPayload);
					}
					else {
						nPayload = 0;
					}
                    n += (u16)utilc.getVarint(pCell, iCell + n, out pInfo.nKey);
					pInfo.nData = nPayload;
				}
				else {
					pInfo.nData = 0;
                    n += (u16)utilc.getVarint32(pCell, iCell + n, out nPayload);
					pInfo.nKey = nPayload;
				}
				pInfo.nPayload = nPayload;
				pInfo.nHeader = n;
				sqliteinth.testcase (nPayload == this.maxLocal);
				sqliteinth.testcase (nPayload == this.maxLocal + 1);
				if (sqliteinth.likely (nPayload <= this.maxLocal)) {
					///
///<summary>
///This is the (easy) common case where the entire payload fits
///on the local page.  No overflow is required.
///
///</summary>

					if ((pInfo.nSize = (u16)(n + nPayload)) < 4)
						pInfo.nSize = 4;
					pInfo.nLocal = (u16)nPayload;
					pInfo.iOverflow = 0;
				}
				else {
					///
///<summary>
///If the payload will not fit completely on the local page, we have
///to decide how much to store locally and how much to spill onto
///overflow pages.  The strategy is to minimize the amount of unused
///space on overflow pages while keeping the amount of local storage
///in between minLocal and maxLocal.
///
///Warning:  changing the way overflow payload is distributed in any
///way will result in an incompatible file format.
///
///</summary>

					int minLocal;
					///
///<summary>
///Minimum amount of payload held locally 
///</summary>

					int maxLocal;
					///
///<summary>
///Maximum amount of payload held locally 
///</summary>

					int surplus;
					///
///<summary>
///Overflow payload available for local storage 
///</summary>

					minLocal = this.minLocal;
					maxLocal = this.maxLocal;
					surplus = (int)(minLocal + (nPayload - minLocal) % (this.pBt.usableSize - 4));
					sqliteinth.testcase (surplus == maxLocal);
					sqliteinth.testcase (surplus == maxLocal + 1);
					if (surplus <= maxLocal) {
						pInfo.nLocal = (u16)surplus;
					}
					else {
						pInfo.nLocal = (u16)minLocal;
					}
					pInfo.iOverflow = (u16)(pInfo.nLocal + n);
					pInfo.nSize = (u16)(pInfo.iOverflow + 4);
				}
			}

			//  btreeParseCellPtr((pPage), findCell((pPage), (iCell)), (pInfo))
			public void parseCell (int iCell, ref CellInfo pInfo)
			{
				this.btreeParseCellPtr (this.findCell (iCell), ref pInfo);
			}

			public///<summary>
			/// Compute the total number of bytes that a Cell needs in the cell
			/// data area of the btree-page.  The return number includes the cell
			/// data header and the local payload, but not any overflow page or
			/// the space used by the cell pointer.
			///</summary>
			// Alternative form for C#
			u16 cellSizePtr (int iCell)
			{
				CellInfo info = new CellInfo ();
				byte[] pCell = new byte[13];
				// Minimum Size = (2 bytes of Header  or (4) Child Pointer) + (maximum of) 9 bytes data
				if (iCell < 0)
					// Overflow Cell
					Buffer.BlockCopy (this.aOvfl [-(iCell + 1)].pCell, 0, pCell, 0, pCell.Length < this.aOvfl [-(iCell + 1)].pCell.Length ? pCell.Length : this.aOvfl [-(iCell + 1)].pCell.Length);
				else
					if (iCell >= this.aData.Length + 1 - pCell.Length)
						Buffer.BlockCopy (this.aData, iCell, pCell, 0, this.aData.Length - iCell);
					else
						Buffer.BlockCopy (this.aData, iCell, pCell, 0, pCell.Length);
				this.btreeParseCellPtr (pCell, ref info);
				return info.nSize;
			}

			public void btreeParseCell (///
///<summary>
///Page containing the cell 
///</summary>

			int iCell, ///
///<summary>
///The cell index.  First cell is 0 
///</summary>

			ref CellInfo pInfo///
///<summary>
///Fill in this structure 
///</summary>

			)
			{
				this.parseCell (iCell, ref pInfo);
			}

			// Alternative form for C#
			public u16 cellSizePtr (byte[] pCell, int offset)
			{
				CellInfo info = new CellInfo ();
				info.pCell = malloc_cs.sqlite3Malloc (pCell.Length);
				Buffer.BlockCopy (pCell, offset, info.pCell, 0, pCell.Length - offset);
				this.btreeParseCellPtr (info.pCell, ref info);
				return info.nSize;
			}

			public u16 cellSizePtr (u8[] pCell)
			{
				int _pIter = this.childPtrSize;
				//u8 pIter = &pCell[pPage.childPtrSize];
				u32 nSize = 0;
				#if SQLITE_DEBUG || DEBUG
																																																																																								  /* The value returned by this function should always be the same as
** the (CellInfo.nSize) value found by doing a full parse of the
** cell. If SQLITE_DEBUG is defined, an Debug.Assert() at the bottom of
** this function verifies that this invariant is not violated. */
  CellInfo debuginfo = new CellInfo();
  btreeParseCellPtr( pPage, pCell, ref debuginfo );
#else
				CellInfo debuginfo = new CellInfo ();
				#endif
				if (this.intKey != 0) {
					int pEnd;
					if (this.hasData != 0) {
                        _pIter += utilc.getVarint32(pCell, out nSize);
						// pIter += utilc.getVarint32( pIter, out nSize );
					}
					else {
						nSize = 0;
					}
					///
///<summary>
///</summary>
///<param name="pIter now points at the 64">bit integer key value, a variable length</param>
///<param name="integer. The following block moves pIter to point at the first byte">integer. The following block moves pIter to point at the first byte</param>
///<param name="past the end of the key value. ">past the end of the key value. </param>

					pEnd = _pIter + 9;
					//pEnd = &pIter[9];
					while (((pCell [_pIter++]) & 0x80) != 0 && _pIter < pEnd)
						;
					//while( (pIter++)&0x80 && pIter<pEnd );
				}
				else {
                    _pIter += utilc.getVarint32(pCell, _pIter, out nSize);
					//pIter += utilc.getVarint32( pIter, out nSize );
				}
				sqliteinth.testcase (nSize == this.maxLocal);
				sqliteinth.testcase (nSize == this.maxLocal + 1);
				if (nSize > this.maxLocal) {
					int minLocal = this.minLocal;
					nSize = (u32)(minLocal + (nSize - minLocal) % (this.pBt.usableSize - 4));
					sqliteinth.testcase (nSize == this.maxLocal);
					sqliteinth.testcase (nSize == this.maxLocal + 1);
					if (nSize > this.maxLocal) {
						nSize = (u32)minLocal;
					}
					nSize += 4;
				}
				nSize += (uint)_pIter;
				//nSize += (u32)(pIter - pCell);
				///
///<summary>
///The minimum size of any cell is 4 bytes. 
///</summary>

				if (nSize < 4) {
					nSize = 4;
				}
				Debug.Assert (nSize == debuginfo.nSize);
				return (u16)nSize;
			}

			public int cellSize (int iCell)
			{
				return -1;
			}

			public///<summary>
			/// If the cell pCell, part of page pPage contains a pointer
			/// to an overflow page, insert an entry into the pointer-map
			/// for the overflow page.
			///</summary>
			void ptrmapPutOvflPtr (int pCell, ref int pRC)
			{
				if (pRC != 0)
					return;
				CellInfo info = new CellInfo ();
				Debug.Assert (pCell != 0);
				this.btreeParseCellPtr (pCell, ref info);
				Debug.Assert ((info.nData + (this.intKey != 0 ? 0 : info.nKey)) == info.nPayload);
				if (info.iOverflow != 0) {
					Pgno ovfl = Converter.sqlite3Get4byte (this.aData, pCell, info.iOverflow);
					this.pBt.ptrmapPut (ovfl, PTRMAP_OVERFLOW1, this.pgno, ref pRC);
				}
			}

			public void ptrmapPutOvflPtr (u8[] pCell, ref int pRC)
			{
				if (pRC != 0)
					return;
				CellInfo info = new CellInfo ();
				Debug.Assert (pCell != null);
				this.btreeParseCellPtr (pCell, ref info);
				Debug.Assert ((info.nData + (this.intKey != 0 ? 0 : info.nKey)) == info.nPayload);
				if (info.iOverflow != 0) {
					Pgno ovfl = Converter.sqlite3Get4byte (pCell, info.iOverflow);
					this.pBt.ptrmapPut (ovfl, PTRMAP_OVERFLOW1, this.pgno, ref pRC);
				}
			}

			public///<summary>
			/// Defragment the page given.  All Cells are moved to the
			/// end of the page and all free space is collected into one
			/// big FreeBlk that occurs in between the header and cell
			/// pointer array and the cell content area.
			///</summary>
			int defragmentPage ()
			{
				int i;
				///
///<summary>
///Loop counter 
///</summary>

				int pc;
				///
///<summary>
///</summary>
///<param name="Address of a i">th cell </param>

				int addr;
				///
///<summary>
///Offset of first byte after cell pointer array 
///</summary>

				int hdr;
				///
///<summary>
///Offset to the page header 
///</summary>

				int size;
				///
///<summary>
///Size of a cell 
///</summary>

				int usableSize;
				///
///<summary>
///Number of usable bytes on a page 
///</summary>

				int cellOffset;
				///
///<summary>
///Offset to the cell pointer array 
///</summary>

				int cbrk;
				///
///<summary>
///Offset to the cell content area 
///</summary>

				int nCell;
				///
///<summary>
///Number of cells on the page 
///</summary>

				byte[] data;
				///
///<summary>
///The page data 
///</summary>

				byte[] temp;
				///
///<summary>
///Temp area for cell content 
///</summary>

				int iCellFirst;
				///
///<summary>
///First allowable cell index 
///</summary>

				int iCellLast;
				///
///<summary>
///Last possible cell index 
///</summary>

				Debug.Assert (sqlite3PagerIswriteable (this.pDbPage));
				Debug.Assert (this.pBt != null);
				Debug.Assert (this.pBt.usableSize <= SQLITE_MAX_PAGE_SIZE);
				Debug.Assert (this.nOverflow == 0);
				Debug.Assert (this.pBt.mutex.sqlite3_mutex_held());
				temp = this.pBt.pPager.sqlite3PagerTempSpace ();
				data = this.aData;
				hdr = this.hdrOffset;
				cellOffset = this.cellOffset;
				nCell = this.nCell;
				Debug.Assert (nCell == get2byte (data, hdr + 3));
				usableSize = (int)this.pBt.usableSize;
				cbrk = get2byte (data, hdr + 5);
				Buffer.BlockCopy (data, cbrk, temp, cbrk, usableSize - cbrk);
				//memcpy( temp[cbrk], ref data[cbrk], usableSize - cbrk );
				cbrk = usableSize;
				iCellFirst = cellOffset + 2 * nCell;
				iCellLast = usableSize - 4;
				for (i = 0; i < nCell; i++) {
					int pAddr;
					///
///<summary>
///</summary>
///<param name="The i">th cell pointer </param>

					pAddr = cellOffset + i * 2;
					// &data[cellOffset + i * 2];
					pc = get2byte (data, pAddr);
					sqliteinth.testcase (pc == iCellFirst);
					sqliteinth.testcase (pc == iCellLast);
					#if !(SQLITE_ENABLE_OVERSIZE_CELL_CHECK)
					///
///<summary>
///These conditions have already been verified in btreeInitPage()
///if SQLITE_ENABLE_OVERSIZE_CELL_CHECK is defined
///</summary>

					if (pc < iCellFirst || pc > iCellLast) {
						return sqliteinth.SQLITE_CORRUPT_BKPT();
					}
					#endif
					Debug.Assert (pc >= iCellFirst && pc <= iCellLast);
					size = this.cellSizePtr (temp, pc);
					cbrk -= size;
					#if (SQLITE_ENABLE_OVERSIZE_CELL_CHECK)
																																																																																																															    if ( cbrk < iCellFirst || pc + size > usableSize )
    {
      return SQLITE_CORRUPT_BKPT();
    }
#else
					if (cbrk < iCellFirst || pc + size > usableSize) {
						return sqliteinth.SQLITE_CORRUPT_BKPT();
					}
					#endif
					Debug.Assert (cbrk + size <= usableSize && cbrk >= iCellFirst);
					sqliteinth.testcase (cbrk + size == usableSize);
					sqliteinth.testcase (pc + size == usableSize);
					Buffer.BlockCopy (temp, pc, data, cbrk, size);
					//memcpy(data[cbrk], ref temp[pc], size);
					put2byte (data, pAddr, cbrk);
				}
				Debug.Assert (cbrk >= iCellFirst);
				put2byte (data, hdr + 5, cbrk);
				data [hdr + 1] = 0;
				data [hdr + 2] = 0;
				data [hdr + 7] = 0;
				addr = cellOffset + 2 * nCell;
				Array.Clear (data, addr, cbrk - addr);
				//memset(data[iCellFirst], 0, cbrk-iCellFirst);
				Debug.Assert (sqlite3PagerIswriteable (this.pDbPage));
				if (cbrk - iCellFirst != this.nFree) {
					return sqliteinth.SQLITE_CORRUPT_BKPT();
				}
				return Sqlite3.SQLITE_OK;
			}

			public///<summary>
			/// Allocate nByte bytes of space from within the B-Tree page passed
			/// as the first argument. Write into pIdx the index into pPage.aData[]
			/// of the first byte of allocated space. Return either Sqlite3.SQLITE_OK or
			/// an error code (usually SQLITE_CORRUPT).
			///
			/// The caller guarantees that there is sufficient space to make the
			/// allocation.  This routine might need to defragment in order to bring
			/// all the space together, however.  This routine will avoid using
			/// the first two bytes past the cell pointer area since presumably this
			/// allocation is being made in order to insert a new cell, so we will
			/// also end up needing a new cell pointer.
			///</summary>
			int allocateSpace (int nByte, ref int pIdx)
			{
				int hdr = this.hdrOffset;
				///
///<summary>
///Local cache of pPage.hdrOffset 
///</summary>

				u8[] data = this.aData;
				///
///<summary>
///Local cache of pPage.aData 
///</summary>

				int nFrag;
				///
///<summary>
///Number of fragmented bytes on pPage 
///</summary>

				int top;
				///
///<summary>
///First byte of cell content area 
///</summary>

				int gap;
				///
///<summary>
///First byte of gap between cell pointers and cell content 
///</summary>

				int rc;
				///
///<summary>
///Integer return code 
///</summary>

				u32 usableSize;
				///
///<summary>
///Usable size of the page 
///</summary>

				Debug.Assert (sqlite3PagerIswriteable (this.pDbPage));
				Debug.Assert (this.pBt != null);
				Debug.Assert (this.pBt.mutex.sqlite3_mutex_held());
				Debug.Assert (nByte >= 0);
				///
///<summary>
///Minimum cell size is 4 
///</summary>

				Debug.Assert (this.nFree >= nByte);
				Debug.Assert (this.nOverflow == 0);
				usableSize = this.pBt.usableSize;
				Debug.Assert (nByte < usableSize - 8);
				nFrag = data [hdr + 7];
				Debug.Assert (this.cellOffset == hdr + 12 - 4 * this.leaf);
				gap = this.cellOffset + 2 * this.nCell;
                top = BTreeMethods.get2byteNotZero(data, hdr + 5);
				if (gap > top)
					return sqliteinth.SQLITE_CORRUPT_BKPT();
				sqliteinth.testcase (gap + 2 == top);
				sqliteinth.testcase (gap + 1 == top);
				sqliteinth.testcase (gap == top);
				if (nFrag >= 60) {
					///
///<summary>
///Always defragment highly fragmented pages 
///</summary>

					rc = this.defragmentPage ();
					if (rc != 0)
						return rc;
                    top = BTreeMethods.get2byteNotZero(data, hdr + 5);
				}
				else
					if (gap + 2 <= top) {
						///
///<summary>
///Search the freelist looking for a free slot big enough to satisfy
///the request. The allocation is made from the first free slot in
///the list that is large enough to accomadate it.
///
///</summary>

						int pc, addr;
						for (addr = hdr + 1; (pc = get2byte (data, addr)) > 0; addr = pc) {
							int size;
							///
///<summary>
///Size of free slot 
///</summary>

							if (pc > usableSize - 4 || pc < addr + 4) {
								return sqliteinth.SQLITE_CORRUPT_BKPT();
							}
							size = get2byte (data, pc + 2);
							if (size >= nByte) {
								int x = size - nByte;
								sqliteinth.testcase (x == 4);
								sqliteinth.testcase (x == 3);
								if (x < 4) {
									///
///<summary>
///</summary>
///<param name="Remove the slot from the free">list. Update the number of</param>
///<param name="fragmented bytes within the page. ">fragmented bytes within the page. </param>

									data [addr + 0] = data [pc + 0];
									data [addr + 1] = data [pc + 1];
									//memcpy( data[addr], ref data[pc], 2 );
									data [hdr + 7] = (u8)(nFrag + x);
								}
								else
									if (size + pc > usableSize) {
										return sqliteinth.SQLITE_CORRUPT_BKPT();
									}
									else {
										///
///<summary>
///</summary>
///<param name="The slot remains on the free">list. Reduce its size to account</param>
///<param name="for the portion used by the new allocation. ">for the portion used by the new allocation. </param>

										put2byte (data, pc + 2, x);
									}
								pIdx = pc + x;
								return Sqlite3.SQLITE_OK;
							}
						}
					}
				///
///<summary>
///Check to make sure there is enough space in the gap to satisfy
///the allocation.  If not, defragment.
///
///</summary>

				sqliteinth.testcase (gap + 2 + nByte == top);
				if (gap + 2 + nByte > top) {
					rc = this.defragmentPage ();
					if (rc != 0)
						return rc;
                    top = BTreeMethods.get2byteNotZero(data, hdr + 5);
					Debug.Assert (gap + nByte <= top);
				}
				///
///<summary>
///Allocate memory from the gap in between the cell pointer array
///and the cell content area.  The btreeInitPage() call has already
///validated the freelist.  Given that the freelist is valid, there
///is no way that the allocation can extend off the end of the page.
///The Debug.Assert() below verifies the previous sentence.
///
///</summary>

				top -= nByte;
				put2byte (data, hdr + 5, top);
				Debug.Assert (top + nByte <= (int)this.pBt.usableSize);
				pIdx = top;
				return Sqlite3.SQLITE_OK;
			}

			public///<summary>
			/// Return a section of the pPage.aData to the freelist.
			/// The first byte of the new free block is pPage.aDisk[start]
			/// and the size of the block is "size" bytes.
			///
			/// Most of the effort here is involved in coalesing adjacent
			/// free blocks into a single big free block.
			///</summary>
			int freeSpace (u32 start, int size)
			{
				return this.freeSpace ((int)start, size);
			}

			public int freeSpace (int start, int size)
			{
				int addr, pbegin, hdr;
				int iLast;
				///
///<summary>
///Largest possible freeblock offset 
///</summary>

				byte[] data = this.aData;
				Debug.Assert (this.pBt != null);
				Debug.Assert (sqlite3PagerIswriteable (this.pDbPage));
				Debug.Assert (start >= this.hdrOffset + 6 + this.childPtrSize);
				Debug.Assert ((start + size) <= (int)this.pBt.usableSize);
				Debug.Assert (this.pBt.mutex.sqlite3_mutex_held());
				Debug.Assert (size >= 0);
				///
///<summary>
///Minimum cell size is 4 
///</summary>

				if (this.pBt.secureDelete) {
					///
///<summary>
///Overwrite deleted information with zeros when the secure_delete
///option is enabled 
///</summary>

					Array.Clear (data, start, size);
					// memset(&data[start], 0, size);
				}
				///
///<summary>
///Add the space back into the linked list of freeblocks.  Note that
///even though the freeblock list was checked by btreeInitPage(),
///btreeInitPage() did not detect overlapping cells or
///freeblocks that overlapped cells.   Nor does it detect when the
///cell content area exceeds the value in the page header.  If these
///situations arise, then subsequent insert operations might corrupt
///the freelist.  So we do need to check for corruption while scanning
///the freelist.
///
///</summary>

				hdr = this.hdrOffset;
				addr = hdr + 1;
				iLast = (int)this.pBt.usableSize - 4;
				Debug.Assert (start <= iLast);
				while ((pbegin = get2byte (data, addr)) < start && pbegin > 0) {
					if (pbegin < addr + 4) {
						return sqliteinth.SQLITE_CORRUPT_BKPT();
					}
					addr = pbegin;
				}
				if (pbegin > iLast) {
					return sqliteinth.SQLITE_CORRUPT_BKPT();
				}
				Debug.Assert (pbegin > addr || pbegin == 0);
				put2byte (data, addr, start);
				put2byte (data, start, pbegin);
				put2byte (data, start + 2, size);
				this.nFree = (u16)(this.nFree + size);
				///
///<summary>
///Coalesce adjacent free blocks 
///</summary>

				addr = hdr + 1;
				while ((pbegin = get2byte (data, addr)) > 0) {
					int pnext, psize, x;
					Debug.Assert (pbegin > addr);
					Debug.Assert (pbegin <= (int)this.pBt.usableSize - 4);
					pnext = get2byte (data, pbegin);
					psize = get2byte (data, pbegin + 2);
					if (pbegin + psize + 3 >= pnext && pnext > 0) {
						int frag = pnext - (pbegin + psize);
						if ((frag < 0) || (frag > (int)data [hdr + 7])) {
							return sqliteinth.SQLITE_CORRUPT_BKPT();
						}
						data [hdr + 7] -= (u8)frag;
						x = get2byte (data, pnext);
						put2byte (data, pbegin, x);
						x = pnext + get2byte (data, pnext + 2) - pbegin;
						put2byte (data, pbegin + 2, x);
					}
					else {
						addr = pbegin;
					}
				}
				///
///<summary>
///If the cell content area begins with a freeblock, remove it. 
///</summary>

				if (data [hdr + 1] == data [hdr + 5] && data [hdr + 2] == data [hdr + 6]) {
					int top;
					pbegin = get2byte (data, hdr + 1);
					put2byte (data, hdr + 1, get2byte (data, pbegin));
					//memcpy( data[hdr + 1], ref data[pbegin], 2 );
					top = get2byte (data, hdr + 5) + get2byte (data, pbegin + 2);
					put2byte (data, hdr + 5, top);
				}
				Debug.Assert (sqlite3PagerIswriteable (this.pDbPage));
				return Sqlite3.SQLITE_OK;
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
			int decodeFlags (int flagByte)
			{
				BtShared pBt;
				///
///<summary>
///A copy of pPage.pBt 
///</summary>

				Debug.Assert (this.hdrOffset == (this.pgno == 1 ? 100 : 0));
				Debug.Assert (this.pBt.mutex.sqlite3_mutex_held());
				this.leaf = (u8)(flagByte >> 3);
				Debug.Assert (PTF_LEAF == 1 << 3);
				flagByte &= ~PTF_LEAF;
				this.childPtrSize = (u8)(4 - 4 * this.leaf);
				pBt = this.pBt;
				if (flagByte == (PTF_LEAFDATA | PTF_INTKEY)) {
					this.intKey = 1;
					this.hasData = this.leaf;
					this.maxLocal = pBt.maxLeaf;
					this.minLocal = pBt.minLeaf;
				}
				else
					if (flagByte == PTF_ZERODATA) {
						this.intKey = 0;
						this.hasData = 0;
						this.maxLocal = pBt.maxLocal;
						this.minLocal = pBt.minLocal;
					}
					else {
						return sqliteinth.SQLITE_CORRUPT_BKPT();
					}
				return Sqlite3.SQLITE_OK;
			}

			public///<summary>
			/// Initialize the auxiliary information for a disk block.
			///
			/// Return Sqlite3.SQLITE_OK on success.  If we see that the page does
			/// not contain a well-formed database page, then return
			/// SQLITE_CORRUPT.  Note that a return of Sqlite3.SQLITE_OK does not
			/// guarantee that the page is well-formed.  It only shows that
			/// we failed to detect any corruption.
			///</summary>
			int btreeInitPage ()
			{
				Debug.Assert (this.pBt != null);
				Debug.Assert (this.pBt.mutex.sqlite3_mutex_held());
				Debug.Assert (this.pgno == PagerMethods.sqlite3PagerPagenumber (this.pDbPage));
				Debug.Assert (this ==  PagerMethods.sqlite3PagerGetExtra  (this.pDbPage));
                Debug.Assert(this.aData == this.pDbPage.sqlite3PagerGetData());
				if (0 == this.isInit) {
					u16 pc;
					///
///<summary>
///Address of a freeblock within pPage.aData[] 
///</summary>

					u8 hdr;
					///
///<summary>
///Offset to beginning of page header 
///</summary>

					u8[] data;
					///
///<summary>
///Equal to pPage.aData 
///</summary>

					BtShared pBt;
					///
///<summary>
///The main btree structure 
///</summary>

					int usableSize;
					///
///<summary>
///Amount of usable space on each page 
///</summary>

					u16 cellOffset;
					///
///<summary>
///Offset from start of page to first cell pointer 
///</summary>

					int nFree;
					///
///<summary>
///Number of unused bytes on the page 
///</summary>

					int top;
					///
///<summary>
///First byte of the cell content area 
///</summary>

					int iCellFirst;
					///
///<summary>
///First allowable cell or freeblock offset 
///</summary>

					int iCellLast;
					///
///<summary>
///Last possible cell or freeblock offset 
///</summary>

					pBt = this.pBt;
					hdr = this.hdrOffset;
					data = this.aData;
					if (this.decodeFlags (data [hdr]) != 0)
						return sqliteinth.SQLITE_CORRUPT_BKPT();
					Debug.Assert (pBt.pageSize >= 512 && pBt.pageSize <= 65536);
					this.maskPage = (u16)(pBt.pageSize - 1);
					this.nOverflow = 0;
					usableSize = (int)pBt.usableSize;
					this.cellOffset = (cellOffset = (u16)(hdr + 12 - 4 * this.leaf));
                    top = BTreeMethods.get2byteNotZero(data, hdr + 5);
					this.nCell = (u16)(get2byte (data, hdr + 3));
					if (this.nCell > MX_CELL (pBt)) {
						///
///<summary>
///To many cells for a single page.  The page must be corrupt 
///</summary>

						return sqliteinth.SQLITE_CORRUPT_BKPT();
					}
					sqliteinth.testcase (this.nCell == MX_CELL (pBt));
					///
///<summary>
///A malformed database page might cause us to read past the end
///of page when parsing a cell.
///
///The following block of code checks early to see if a cell extends
///past the end of a page boundary and causes SQLITE_CORRUPT to be
///returned if it does.
///
///</summary>

					iCellFirst = cellOffset + 2 * this.nCell;
					iCellLast = usableSize - 4;
					#if (SQLITE_ENABLE_OVERSIZE_CELL_CHECK)
																																																																																																															    {
      int i;            /* Index into the cell pointer array */
      int sz;           /* Size of a cell */

      if ( 0 == pPage.leaf )
        iCellLast--;
      for ( i = 0; i < pPage.nCell; i++ )
      {
        pc = (u16)get2byte( data, cellOffset + i * 2 );
        sqliteinth.testcase( pc == iCellFirst );
        sqliteinth.testcase( pc == iCellLast );
        if ( pc < iCellFirst || pc > iCellLast )
        {
          return SQLITE_CORRUPT_BKPT();
        }
        sz = cellSizePtr( pPage, data, pc );
        sqliteinth.testcase( pc + sz == usableSize );
        if ( pc + sz > usableSize )
        {
          return SQLITE_CORRUPT_BKPT();
        }
      }
      if ( 0 == pPage.leaf )
        iCellLast++;
    }
#endif
					///
///<summary>
///Compute the total free space on the page 
///</summary>

					pc = (u16)get2byte (data, hdr + 1);
					nFree = (u16)(data [hdr + 7] + top);
					while (pc > 0) {
						u16 next, size;
						if (pc < iCellFirst || pc > iCellLast) {
							///
///<summary>
///Start of free block is off the page 
///</summary>

							return sqliteinth.SQLITE_CORRUPT_BKPT();
						}
						next = (u16)get2byte (data, pc);
						size = (u16)get2byte (data, pc + 2);
						if ((next > 0 && next <= pc + size + 3) || pc + size > usableSize) {
							///
///<summary>
///Free blocks must be in ascending order. And the last byte of
///</summary>
///<param name="the free">block must lie on the database page.  </param>

							return sqliteinth.SQLITE_CORRUPT_BKPT();
						}
						nFree = (u16)(nFree + size);
						pc = next;
					}
					///
///<summary>
///At this point, nFree contains the sum of the offset to the start
///</summary>
///<param name="of the cell">content area plus the number of free bytes within</param>
///<param name="the cell">size</param>
///<param name="of the page, then the page must be corrupted. This check also">of the page, then the page must be corrupted. This check also</param>
///<param name="serves to verify that the offset to the start of the cell">content</param>
///<param name="area, according to the page header, lies within the page.">area, according to the page header, lies within the page.</param>
///<param name=""></param>

					if (nFree > usableSize) {
						return sqliteinth.SQLITE_CORRUPT_BKPT();
					}
					this.nFree = (u16)(nFree - iCellFirst);
					this.isInit = 1;
				}
				return Sqlite3.SQLITE_OK;
			}

			public///<summary>
			/// Set up a raw page so that it looks like a database page holding
			/// no entries.
			///</summary>
			void zeroPage (int flags)
			{
				byte[] data = this.aData;
				BtShared pBt = this.pBt;
				u8 hdr = this.hdrOffset;
				u16 first;
				Debug.Assert (PagerMethods.sqlite3PagerPagenumber (this.pDbPage) == this.pgno);
				Debug.Assert ( PagerMethods.sqlite3PagerGetExtra  (this.pDbPage) == this);
                Debug.Assert(this.pDbPage.sqlite3PagerGetData() == data);
				Debug.Assert (sqlite3PagerIswriteable (this.pDbPage));
				Debug.Assert (pBt.mutex.sqlite3_mutex_held());
				if (pBt.secureDelete) {
					Array.Clear (data, hdr, (int)(pBt.usableSize - hdr));
					//memset(&data[hdr], 0, pBt->usableSize - hdr);
				}
				data [hdr] = (u8)flags;
				first = (u16)(hdr + 8 + 4 * ((flags & PTF_LEAF) == 0 ? 1 : 0));
				Array.Clear (data, hdr + 1, 4);
				//memset(data[hdr+1], 0, 4);
				data [hdr + 7] = 0;
				put2byte (data, hdr + 5, pBt.usableSize);
				this.nFree = (u16)(pBt.usableSize - first);
				this.decodeFlags (flags);
				this.hdrOffset = hdr;
				this.cellOffset = first;
				this.nOverflow = 0;
				Debug.Assert (pBt.pageSize >= 512 && pBt.pageSize <= 65536);
				this.maskPage = (u16)(pBt.pageSize - 1);
				this.nCell = 0;
				this.isInit = 1;
			}

			/**
///<summary>
///</summary>
///<param name="Set the pointer">map entries for all children of page pPage. Also, if</param>
///<param name="pPage contains cells that point to overflow pages, set the pointer">pPage contains cells that point to overflow pages, set the pointer</param>
///<param name="map entries for the overflow pages as well.">map entries for the overflow pages as well.</param>
*/public int setChildPtrmaps ()
			{
				int i;
				///
///<summary>
///Counter variable 
///</summary>

				int nCell;
				///
///<summary>
///Number of cells in page pPage 
///</summary>

				int rc;
				///
///<summary>
///Return code 
///</summary>

				BtShared pBt = this.pBt;
				u8 isInitOrig = this.isInit;
				Pgno pgno = this.pgno;
				Debug.Assert (this.pBt.mutex.sqlite3_mutex_held());
				rc = this.btreeInitPage ();
				if (rc != Sqlite3.SQLITE_OK) {
					goto set_child_ptrmaps_out;
				}
				nCell = this.nCell;
				for (i = 0; i < nCell; i++) {
					int pCell = this.findCell (i);
					this.ptrmapPutOvflPtr (pCell, ref rc);
					if (0 == this.leaf) {
						Pgno childPgno = Converter.sqlite3Get4byte (this.aData, pCell);
						pBt.ptrmapPut (childPgno, PTRMAP_BTREE, pgno, ref rc);
					}
				}
				if (0 == this.leaf) {
					Pgno childPgno = Converter.sqlite3Get4byte (this.aData, this.hdrOffset + 8);
					pBt.ptrmapPut (childPgno, PTRMAP_BTREE, pgno, ref rc);
				}
				set_child_ptrmaps_out:
				this.isInit = isInitOrig;
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
*/public int modifyPagePointer (Pgno iFrom, Pgno iTo, u8 eType)
			{
				Debug.Assert (this.pBt.mutex.sqlite3_mutex_held());
				Debug.Assert (sqlite3PagerIswriteable (this.pDbPage));
				if (eType == PTRMAP_OVERFLOW2) {
					///
///<summary>
///The pointer is always the first 4 bytes of the page in this case.  
///</summary>

					if (Converter.sqlite3Get4byte (this.aData) != iFrom) {
						return sqliteinth.SQLITE_CORRUPT_BKPT();
					}
					Converter.sqlite3Put4byte (this.aData, iTo);
				}
				else {
					u8 isInitOrig = this.isInit;
					int i;
					int nCell;
					this.btreeInitPage ();
					nCell = this.nCell;
					for (i = 0; i < nCell; i++) {
						int pCell = this.findCell (i);
						if (eType == PTRMAP_OVERFLOW1) {
							CellInfo info = new CellInfo ();
							this.btreeParseCellPtr (pCell, ref info);
							if (info.iOverflow != 0) {
								if (iFrom == Converter.sqlite3Get4byte (this.aData, pCell, info.iOverflow)) {
									Converter.sqlite3Put4byte (this.aData, pCell + info.iOverflow, (int)iTo);
									break;
								}
							}
						}
						else {
							if (Converter.sqlite3Get4byte (this.aData, pCell) == iFrom) {
								Converter.sqlite3Put4byte (this.aData, pCell, (int)iTo);
								break;
							}
						}
					}
					if (i == nCell) {
						if (eType != PTRMAP_BTREE || Converter.sqlite3Get4byte (this.aData, this.hdrOffset + 8) != iFrom) {
							return sqliteinth.SQLITE_CORRUPT_BKPT();
						}
						Converter.sqlite3Put4byte (this.aData, this.hdrOffset + 8, iTo);
					}
					this.isInit = isInitOrig;
				}
				return Sqlite3.SQLITE_OK;
			}

			//#  define assertParentIndex(x,y,z)
			public void assertParentIndex (int iIdx, Pgno iChild)
			{
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
*/public int fillInCell (///
///<summary>
///The page that contains the cell 
///</summary>

			byte[] pCell, ///
///<summary>
///Complete text of the cell 
///</summary>

			byte[] pKey, i64 nKey, ///
///<summary>
///The key 
///</summary>

			byte[] pData, int nData, ///
///<summary>
///The data 
///</summary>

			int nZero, ///
///<summary>
///Extra zero bytes to append to pData 
///</summary>

			ref int pnSize///
///<summary>
///Write cell size here 
///</summary>

			)
			{
				int nPayload;
				u8[] pSrc;
				int pSrcIndex = 0;
				int nSrc, n, rc;
				int spaceLeft;
				MemPage pOvfl = null;
				MemPage pToRelease = null;
				byte[] pPrior;
				int pPriorIndex = 0;
				byte[] pPayload;
				int pPayloadIndex = 0;
				BtShared pBt = this.pBt;
				Pgno pgnoOvfl = 0;
				int nHeader;
				CellInfo info = new CellInfo ();
				Debug.Assert (this.pBt.mutex.sqlite3_mutex_held());
				///
///<summary>
///pPage is not necessarily writeable since pCell might be auxiliary
///buffer space that is separate from the pPage buffer area 
///</summary>

				// TODO -- Determine if the following Assert is needed under c#
				//Debug.Assert( pCell < pPage.aData || pCell >= &pPage.aData[pBt.pageSize]
				//          || sqlite3PagerIswriteable(pPage.pDbPage) );
				///
///<summary>
///Fill in the header. 
///</summary>

				nHeader = 0;
				if (0 == this.leaf) {
					nHeader += 4;
				}
				if (this.hasData != 0) {
                    nHeader += (int)utilc.putVarint(pCell, nHeader, (int)(nData + nZero));
					//putVarint( pCell[nHeader], nData + nZero );
				}
				else {
					nData = nZero = 0;
				}
                nHeader += utilc.putVarint(pCell, nHeader, (u64)nKey);
				//putVarint( pCell[nHeader], *(u64*)&nKey );
				this.btreeParseCellPtr (pCell, ref info);
				Debug.Assert (info.nHeader == nHeader);
				Debug.Assert (info.nKey == nKey);
				Debug.Assert (info.nData == (u32)(nData + nZero));
				///
///<summary>
///Fill in the payload 
///</summary>

				nPayload = nData + nZero;
				if (this.intKey != 0) {
					pSrc = pData;
					nSrc = nData;
					nData = 0;
				}
				else {
					if (NEVER (nKey > 0x7fffffff || pKey == null)) {
						return sqliteinth.SQLITE_CORRUPT_BKPT();
					}
					nPayload += (int)nKey;
					pSrc = pKey;
					nSrc = (int)nKey;
				}
				pnSize = info.nSize;
				spaceLeft = info.nLocal;
				//  pPayload = &pCell[nHeader];
				pPayload = pCell;
				pPayloadIndex = nHeader;
				//  pPrior = &pCell[info.iOverflow];
				pPrior = pCell;
				pPriorIndex = info.iOverflow;
				while (nPayload > 0) {
					if (spaceLeft == 0) {
						#if !SQLITE_OMIT_AUTOVACUUM
						Pgno pgnoPtrmap = pgnoOvfl;
						///
///<summary>
///</summary>
///<param name="Overflow page pointer">map entry page </param>

						if (pBt.autoVacuum) {
							do {
								pgnoOvfl++;
							}
							while (PTRMAP_ISPAGE (pBt, pgnoOvfl) || pgnoOvfl == PENDING_BYTE_PAGE (pBt));
						}
						#endif
                        rc = BTreeMethods.allocateBtreePage(pBt, ref pOvfl, ref pgnoOvfl, pgnoOvfl, 0);
						#if !SQLITE_OMIT_AUTOVACUUM
						///
///<summary>
///</summary>
///<param name="If the database supports auto">vacuum, and the second or subsequent</param>
///<param name="overflow page is being allocated, add an entry to the pointer">map</param>
///<param name="for that page now.">for that page now.</param>
///<param name=""></param>
///<param name="If this is the first overflow page, then write a partial entry">If this is the first overflow page, then write a partial entry</param>
///<param name="to the pointer">map slot,</param>
///<param name="then the optimistic overflow chain processing in clearCell()">then the optimistic overflow chain processing in clearCell()</param>
///<param name="may misinterpret the uninitialised values and delete the">may misinterpret the uninitialised values and delete the</param>
///<param name="wrong pages from the database.">wrong pages from the database.</param>

						if (pBt.autoVacuum && rc == Sqlite3.SQLITE_OK) {
							u8 eType = (u8)(pgnoPtrmap != 0 ? PTRMAP_OVERFLOW2 : PTRMAP_OVERFLOW1);
							pBt.ptrmapPut (pgnoOvfl, eType, pgnoPtrmap, ref rc);
							if (rc != 0) {
                                BTreeMethods.releasePage(pOvfl);
							}
						}
						#endif
						if (rc != 0) {
                            BTreeMethods.releasePage(pToRelease);
							return rc;
						}
						///
///<summary>
///If pToRelease is not zero than pPrior points into the data area
///of pToRelease.  Make sure pToRelease is still writeable. 
///</summary>

						Debug.Assert (pToRelease == null || sqlite3PagerIswriteable (pToRelease.pDbPage));
						///
///<summary>
///If pPrior is part of the data area of pPage, then make sure pPage
///is still writeable 
///</summary>

						// TODO -- Determine if the following Assert is needed under c#
						//Debug.Assert( pPrior < pPage.aData || pPrior >= &pPage.aData[pBt.pageSize]
						//      || sqlite3PagerIswriteable(pPage.pDbPage) );
						Converter.sqlite3Put4byte (pPrior, pPriorIndex, pgnoOvfl);
                        BTreeMethods.releasePage(pToRelease);
						pToRelease = pOvfl;
						pPrior = pOvfl.aData;
						pPriorIndex = 0;
						Converter.sqlite3Put4byte (pPrior, 0);
						pPayload = pOvfl.aData;
						pPayloadIndex = 4;
						//&pOvfl.aData[4];
						spaceLeft = (int)pBt.usableSize - 4;
					}
					n = nPayload;
					if (n > spaceLeft)
						n = spaceLeft;
					///
///<summary>
///If pToRelease is not zero than pPayload points into the data area
///of pToRelease.  Make sure pToRelease is still writeable. 
///</summary>

					Debug.Assert (pToRelease == null || sqlite3PagerIswriteable (pToRelease.pDbPage));
					///
///<summary>
///If pPayload is part of the data area of pPage, then make sure pPage
///is still writeable 
///</summary>

					// TODO -- Determine if the following Assert is needed under c#
					//Debug.Assert( pPayload < pPage.aData || pPayload >= &pPage.aData[pBt.pageSize]
					//        || sqlite3PagerIswriteable(pPage.pDbPage) );
					if (nSrc > 0) {
						if (n > nSrc)
							n = nSrc;
						Debug.Assert (pSrc != null);
						Buffer.BlockCopy (pSrc, pSrcIndex, pPayload, pPayloadIndex, n);
						//memcpy(pPayload, pSrc, n);
					}
					else {
						byte[] pZeroBlob = malloc_cs.sqlite3Malloc (n);
						// memset(pPayload, 0, n);
						Buffer.BlockCopy (pZeroBlob, 0, pPayload, pPayloadIndex, n);
					}
					nPayload -= n;
					pPayloadIndex += n;
					// pPayload += n;
					pSrcIndex += n;
					// pSrc += n;
					nSrc -= n;
					spaceLeft -= n;
					if (nSrc == 0) {
						nSrc = nData;
						pSrc = pData;
					}
				}
                BTreeMethods.releasePage(pToRelease);
				return Sqlite3.SQLITE_OK;
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
*/public void dropCell (int idx, int sz, ref int pRC)
			{
				u32 pc;
				///
///<summary>
///Offset to cell content of cell being deleted 
///</summary>

				u8[] data;
				///
///<summary>
///pPage.aData 
///</summary>

				int ptr;
				///
///<summary>
///Used to move bytes around within data[] 
///</summary>

				int endPtr;
				///
///<summary>
///End of loop 
///</summary>

				int rc;
				///
///<summary>
///The return code 
///</summary>

				int hdr;
				///
///<summary>
///Beginning of the header.  0 most pages.  100 page 1 
///</summary>

				if (pRC != 0)
					return;
				Debug.Assert (idx >= 0 && idx < this.nCell);
				#if SQLITE_DEBUG
																																																																																								  Debug.Assert( sz == cellSize( pPage, idx ) );
#endif
				Debug.Assert (sqlite3PagerIswriteable (this.pDbPage));
				Debug.Assert (this.pBt.mutex.sqlite3_mutex_held());
				data = this.aData;
				ptr = this.cellOffset + 2 * idx;
				//ptr = &data[pPage.cellOffset + 2 * idx];
				pc = (u32)get2byte (data, ptr);
				hdr = this.hdrOffset;
				sqliteinth.testcase (pc == get2byte (data, hdr + 5));
				sqliteinth.testcase (pc + sz == this.pBt.usableSize);
				if (pc < (u32)get2byte (data, hdr + 5) || pc + sz > this.pBt.usableSize) {
					pRC = sqliteinth.SQLITE_CORRUPT_BKPT();
					return;
				}
				rc = this.freeSpace (pc, sz);
				if (rc != 0) {
					pRC = rc;
					return;
				}
				//endPtr = &data[pPage->cellOffset + 2*pPage->nCell - 2];
				//assert( (SQLITE_PTR_TO_INT(ptr)&1)==0 );  /* ptr is always 2-byte aligned */
				//while( ptr<endPtr ){
				//  *(u16*)ptr = *(u16*)&ptr[2];
				//  ptr += 2;
				Buffer.BlockCopy (data, ptr + 2, data, ptr, (this.nCell - 1 - idx) * 2);
				this.nCell--;
				data [this.hdrOffset + 3] = (byte)(this.nCell >> 8);
				data [this.hdrOffset + 4] = (byte)(this.nCell);
				//put2byte( data, hdr + 3, pPage.nCell );
				this.nFree += 2;
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
*/public void insertCell (///
///<summary>
///Page into which we are copying 
///</summary>

			int i, ///
///<summary>
///</summary>
///<param name="New cell becomes the i">th cell of the page </param>

			u8[] pCell, ///
///<summary>
///Content of the new cell 
///</summary>

			int sz, ///
///<summary>
///Bytes of content in pCell 
///</summary>

			u8[] pTemp, ///
///<summary>
///Temp storage space for pCell, if needed 
///</summary>

			Pgno iChild, ///
///<summary>
///</summary>
///<param name="If non">zero, replace first 4 bytes with this value </param>

			ref int pRC///
///<summary>
///Read and write return code from here 
///</summary>

			)
			{
				int idx = 0;
				///
///<summary>
///Where to write new cell content in data[] 
///</summary>

				int j;
				///
///<summary>
///Loop counter 
///</summary>

				int end;
				///
///<summary>
///First byte past the last cell pointer in data[] 
///</summary>

				int ins;
				///
///<summary>
///Index in data[] where new cell pointer is inserted 
///</summary>

				int cellOffset;
				///
///<summary>
///Address of first cell pointer in data[] 
///</summary>

				u8[] data;
				///
///<summary>
///The content of the whole page 
///</summary>

				u8 ptr;
				///
///<summary>
///Used for moving information around in data[] 
///</summary>

				u8 endPtr;
				///
///<summary>
///End of the loop 
///</summary>

				int nSkip = (iChild != 0 ? 4 : 0);
				if (pRC != 0)
					return;
				Debug.Assert (i >= 0 && i <= this.nCell + this.nOverflow);
				Debug.Assert (this.nCell <= MX_CELL (this.pBt) && MX_CELL (this.pBt) <= 10921);
				Debug.Assert (this.nOverflow <= Sqlite3.ArraySize (this.aOvfl));
				Debug.Assert (this.pBt.mutex.sqlite3_mutex_held());
				///
///<summary>
///The cell should normally be sized correctly.  However, when moving a
///malformed cell from a leaf page to an interior page, if the cell size
///wanted to be less than 4 but got rounded up to 4 on the leaf, then size
///</summary>
///<param name="might be less than 8 (leaf">size + pointer) on the interior node.  Hence</param>
///<param name="the term after the || in the following assert(). ">the term after the || in the following assert(). </param>

				Debug.Assert (sz == this.cellSizePtr (pCell) || (sz == 8 && iChild > 0));
				if (this.nOverflow != 0 || sz + 2 > this.nFree) {
					if (pTemp != null) {
						Buffer.BlockCopy (pCell, nSkip, pTemp, nSkip, sz - nSkip);
						//memcpy(pTemp+nSkip, pCell+nSkip, sz-nSkip);
						pCell = pTemp;
					}
					if (iChild != 0) {
						Converter.sqlite3Put4byte (pCell, iChild);
					}
					j = this.nOverflow++;
					Debug.Assert (j < this.aOvfl.Length);
					//(int)(sizeof(pPage.aOvfl)/sizeof(pPage.aOvfl[0])) );
					this.aOvfl [j].pCell = pCell;
					this.aOvfl [j].idx = (u16)i;
				}
				else {
					int rc = PagerMethods.sqlite3PagerWrite (this.pDbPage);
					if (rc != Sqlite3.SQLITE_OK) {
						pRC = rc;
						return;
					}
					Debug.Assert (sqlite3PagerIswriteable (this.pDbPage));
					data = this.aData;
					cellOffset = this.cellOffset;
					end = cellOffset + 2 * this.nCell;
					ins = cellOffset + 2 * i;
					rc = this.allocateSpace (sz, ref idx);
					if (rc != 0) {
						pRC = rc;
						return;
					}
					///
///<summary>
///The allocateSpace() routine guarantees the following two properties
///if it returns success 
///</summary>

					Debug.Assert (idx >= end + 2);
					Debug.Assert (idx + sz <= (int)this.pBt.usableSize);
					this.nCell++;
					this.nFree -= (u16)(2 + sz);
					Buffer.BlockCopy (pCell, nSkip, data, idx + nSkip, sz - nSkip);
					//memcpy( data[idx + nSkip], pCell + nSkip, sz - nSkip );
					if (iChild != 0) {
						Converter.sqlite3Put4byte (data, idx, iChild);
					}
					//ptr = &data[end];
					//endPtr = &data[ins];
					//assert( ( SQLITE_PTR_TO_INT( ptr ) & 1 ) == 0 );  /* ptr is always 2-byte aligned */
					//while ( ptr > endPtr )
					//{
					//  *(u16*)ptr = *(u16*)&ptr[-2];
					//  ptr -= 2;
					//}
					for (j = end; j > ins; j -= 2) {
						data [j + 0] = data [j - 2];
						data [j + 1] = data [j - 1];
					}
					put2byte (data, ins, idx);
					put2byte (data, this.hdrOffset + 3, this.nCell);
					#if !SQLITE_OMIT_AUTOVACUUM
					if (this.pBt.autoVacuum) {
						///
///<summary>
///The cell may contain a pointer to an overflow page. If so, write
///the entry for the overflow page into the pointer map.
///
///</summary>

						this.ptrmapPutOvflPtr (pCell, ref pRC);
					}
					#endif
				}
			}

			/**
///<summary>
///Add a list of cells to a page.  The page should be initially empty.
///The cells are guaranteed to fit on the page.
///</summary>
*/public void assemblePage (///
///<summary>
///The page to be assemblied 
///</summary>

			int nCell, ///
///<summary>
///The number of cells to add to this page 
///</summary>

			u8[] apCell, ///
///<summary>
///Pointer to a single the cell bodies 
///</summary>

			int[] aSize///
///<summary>
///Sizes of the cells bodie
///</summary>

			)
			{
				int i;
				///
///<summary>
///Loop counter 
///</summary>

				int pCellptr;
				///
///<summary>
///Address of next cell pointer 
///</summary>

				int cellbody;
				///
///<summary>
///Address of next cell body 
///</summary>

				byte[] data = this.aData;
				///
///<summary>
///Pointer to data for pPage 
///</summary>

				int hdr = this.hdrOffset;
				///
///<summary>
///Offset of header on pPage 
///</summary>

				int nUsable = (int)this.pBt.usableSize;
				///
///<summary>
///Usable size of page 
///</summary>

				Debug.Assert (this.nOverflow == 0);
				Debug.Assert (  pBt.mutex.sqlite3_mutex_held());
				Debug.Assert (nCell >= 0 && nCell <= (int)MX_CELL (this.pBt) && (int)MX_CELL (this.pBt) <= 10921);
				Debug.Assert (sqlite3PagerIswriteable (this.pDbPage));
				///
///<summary>
///Check that the page has just been zeroed by zeroPage() 
///</summary>

				Debug.Assert (this.nCell == 0);
				Debug.Assert (BTreeMethods.get2byteNotZero (data, hdr + 5) == nUsable);
				pCellptr = this.cellOffset + nCell * 2;
				//data[pPage.cellOffset + nCell * 2];
				cellbody = nUsable;
				for (i = nCell - 1; i >= 0; i--) {
					u16 sz = (u16)aSize [i];
					pCellptr -= 2;
					cellbody -= sz;
					put2byte (data, pCellptr, cellbody);
					Buffer.BlockCopy (apCell, 0, data, cellbody, sz);
					// memcpy(&data[cellbody], apCell[i], sz);
				}
				put2byte (data, hdr + 3, nCell);
				put2byte (data, hdr + 5, cellbody);
				this.nFree -= (u16)(nCell * 2 + nUsable - cellbody);
				this.nCell = (u16)nCell;
			}

			public void assemblePage (///
///<summary>
///The page to be assemblied 
///</summary>

			int nCell, ///
///<summary>
///The number of cells to add to this page 
///</summary>

			u8[][] apCell, ///
///<summary>
///Pointers to cell bodies 
///</summary>

			u16[] aSize, ///
///<summary>
///Sizes of the cells 
///</summary>

			int offset///
///<summary>
///Offset into the cell bodies, for c#  
///</summary>

			)
			{
				int i;
				///
///<summary>
///Loop counter 
///</summary>

				int pCellptr;
				///
///<summary>
///Address of next cell pointer 
///</summary>

				int cellbody;
				///
///<summary>
///Address of next cell body 
///</summary>

				byte[] data = this.aData;
				///
///<summary>
///Pointer to data for pPage 
///</summary>

				int hdr = this.hdrOffset;
				///
///<summary>
///Offset of header on pPage 
///</summary>

				int nUsable = (int)this.pBt.usableSize;
				///
///<summary>
///Usable size of page 
///</summary>

				Debug.Assert (this.nOverflow == 0);
				Debug.Assert (this.pBt.mutex.sqlite3_mutex_held());
				Debug.Assert (nCell >= 0 && nCell <= MX_CELL (this.pBt) && MX_CELL (this.pBt) <= 5460);
				Debug.Assert (sqlite3PagerIswriteable (this.pDbPage));
				///
///<summary>
///Check that the page has just been zeroed by zeroPage() 
///</summary>

				Debug.Assert (this.nCell == 0);
				Debug.Assert (get2byte (data, hdr + 5) == nUsable);
				pCellptr = this.cellOffset + nCell * 2;
				//data[pPage.cellOffset + nCell * 2];
				cellbody = nUsable;
				for (i = nCell - 1; i >= 0; i--) {
					pCellptr -= 2;
					cellbody -= aSize [i + offset];
					put2byte (data, pCellptr, cellbody);
					Buffer.BlockCopy (apCell [offset + i], 0, data, cellbody, aSize [i + offset]);
					//          memcpy(&data[cellbody], apCell[i], aSize[i]);
				}
				put2byte (data, hdr + 3, nCell);
				put2byte (data, hdr + 5, cellbody);
				this.nFree -= (u16)(nCell * 2 + nUsable - cellbody);
				this.nCell = (u16)nCell;
			}

			public void assemblePage (///
///<summary>
///The page to be assemblied 
///</summary>

			int nCell, ///
///<summary>
///The number of cells to add to this page 
///</summary>

			u8[] apCell, ///
///<summary>
///Pointers to cell bodies 
///</summary>

			u16[] aSize///
///<summary>
///Sizes of the cells 
///</summary>

			)
			{
				int i;
				///
///<summary>
///Loop counter 
///</summary>

				int pCellptr;
				///
///<summary>
///Address of next cell pointer 
///</summary>

				int cellbody;
				///
///<summary>
///Address of next cell body 
///</summary>

				u8[] data = this.aData;
				///
///<summary>
///Pointer to data for pPage 
///</summary>

				int hdr = this.hdrOffset;
				///
///<summary>
///Offset of header on pPage 
///</summary>

				int nUsable = (int)this.pBt.usableSize;
				///
///<summary>
///Usable size of page 
///</summary>

				Debug.Assert (this.nOverflow == 0);
				Debug.Assert (this.pBt.mutex.sqlite3_mutex_held());
				Debug.Assert (nCell >= 0 && nCell <= MX_CELL (this.pBt) && MX_CELL (this.pBt) <= 5460);
				Debug.Assert (sqlite3PagerIswriteable (this.pDbPage));
				///
///<summary>
///Check that the page has just been zeroed by zeroPage() 
///</summary>

				Debug.Assert (this.nCell == 0);
				Debug.Assert (get2byte (data, hdr + 5) == nUsable);
				pCellptr = this.cellOffset + nCell * 2;
				//&data[pPage.cellOffset + nCell * 2];
				cellbody = nUsable;
				for (i = nCell - 1; i >= 0; i--) {
					pCellptr -= 2;
					cellbody -= aSize [i];
					put2byte (data, pCellptr, cellbody);
					Buffer.BlockCopy (apCell, 0, data, cellbody, aSize [i]);
					//memcpy( data[cellbody], apCell[i], aSize[i] );
				}
				put2byte (data, hdr + 3, nCell);
				put2byte (data, hdr + 5, cellbody);
				this.nFree -= (u16)(nCell * 2 + nUsable - cellbody);
				this.nCell = (u16)nCell;
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
*/public int balance_quick (MemPage pPage, u8[] pSpace)
			{
				BtShared pBt = pPage.pBt;
				///
///<summary>
///</summary>
///<param name="B">Tree Database </param>

				MemPage pNew = new MemPage ();
				///
///<summary>
///Newly allocated page 
///</summary>

				int rc;
				///
///<summary>
///Return Code 
///</summary>

				Pgno pgnoNew = 0;
				///
///<summary>
///Page number of pNew 
///</summary>

				Debug.Assert (pPage.pBt.mutex.sqlite3_mutex_held());
				Debug.Assert (sqlite3PagerIswriteable (this.pDbPage));
				Debug.Assert (pPage.nOverflow == 1);
				///
///<summary>
///This error condition is now caught prior to reaching this function 
///</summary>

				if (pPage.nCell <= 0)
					return sqliteinth.SQLITE_CORRUPT_BKPT();
				///
///<summary>
///</summary>
///<param name="Allocate a new page. This page will become the right">sibling of</param>
///<param name="pPage. Make the parent page writable, so that the new divider cell">pPage. Make the parent page writable, so that the new divider cell</param>
///<param name="may be inserted. If both these operations are successful, proceed.">may be inserted. If both these operations are successful, proceed.</param>
///<param name=""></param>

                rc = BTreeMethods.allocateBtreePage(pBt, ref pNew, ref pgnoNew, 0, 0);
				if (rc == Sqlite3.SQLITE_OK) {
					int pOut = 4;
					//u8 pOut = &pSpace[4];
					u8[] pCell = pPage.aOvfl [0].pCell;
					int[] szCell = new int[1];
					szCell [0] = pPage.cellSizePtr (pCell);
					int pStop;
					Debug.Assert (sqlite3PagerIswriteable (pNew.pDbPage));
					Debug.Assert (pPage.aData [0] == (PTF_INTKEY | PTF_LEAFDATA | PTF_LEAF));
					pNew.zeroPage (PTF_INTKEY | PTF_LEAFDATA | PTF_LEAF);
					pNew.assemblePage (1, pCell, szCell);
					///
///<summary>
///</summary>
///<param name="If this is an auto">vacuum database, update the pointer map</param>
///<param name="with entries for the new page, and any pointer from the">with entries for the new page, and any pointer from the</param>
///<param name="cell on the page to an overflow page. If either of these">cell on the page to an overflow page. If either of these</param>
///<param name="operations fails, the return code is set, but the contents">operations fails, the return code is set, but the contents</param>
///<param name="of the parent page are still manipulated by thh code below.">of the parent page are still manipulated by thh code below.</param>
///<param name="That is Ok, at this point the parent page is guaranteed to">That is Ok, at this point the parent page is guaranteed to</param>
///<param name="be marked as dirty. Returning an error code will cause a">be marked as dirty. Returning an error code will cause a</param>
///<param name="rollback, undoing any changes made to the parent page.">rollback, undoing any changes made to the parent page.</param>
///<param name=""></param>

					#if !SQLITE_OMIT_AUTOVACUUM
					if (pBt.autoVacuum)
					#else
																																																																																																															if (false)
#endif
					 {
						pBt.ptrmapPut (pgnoNew, PTRMAP_BTREE, this.pgno, ref rc);
						if (szCell [0] > pNew.minLocal) {
							pNew.ptrmapPutOvflPtr (pCell, ref rc);
						}
					}
					///
///<summary>
///Create a divider cell to insert into pParent. The divider cell
///</summary>
///<param name="consists of a 4">byte page number (the page number of pPage) and</param>
///<param name="a variable length key value (which must be the same value as the">a variable length key value (which must be the same value as the</param>
///<param name="largest key on pPage).">largest key on pPage).</param>
///<param name=""></param>
///<param name="To find the largest key value on pPage, first find the right">most</param>
///<param name="cell on pPage. The first two fields of this cell are the">cell on pPage. The first two fields of this cell are the</param>
///<param name="record">bits in size)</param>
///<param name="and the key value (a variable length integer, may have any value).">and the key value (a variable length integer, may have any value).</param>
///<param name="The first of the while(...) loops below skips over the record">length</param>
///<param name="field. The second while(...) loop copies the key value from the">field. The second while(...) loop copies the key value from the</param>
///<param name="cell on pPage into the pSpace buffer.">cell on pPage into the pSpace buffer.</param>
///<param name=""></param>

					int iCell = pPage.findCell (pPage.nCell - 1);
					//pCell = findCell( pPage, pPage.nCell - 1 );
					pCell = pPage.aData;
					int _pCell = iCell;
					pStop = _pCell + 9;
					//pStop = &pCell[9];
					while (((pCell [_pCell++]) & 0x80) != 0 && _pCell < pStop)
						;
					//while ( ( *( pCell++ ) & 0x80 ) && pCell < pStop ) ;
					pStop = _pCell + 9;
					//pStop = &pCell[9];
					while (((pSpace [pOut++] = pCell [_pCell++]) & 0x80) != 0 && _pCell < pStop)
						;
					//while ( ( ( *( pOut++ ) = *( pCell++ ) ) & 0x80 ) && pCell < pStop ) ;
					///
///<summary>
///Insert the new divider cell into pParent. 
///</summary>

					this.insertCell (this.nCell, pSpace, pOut, //(int)(pOut-pSpace),
					null, pPage.pgno, ref rc);
					///
///<summary>
///</summary>
///<param name="Set the right">child pointer of pParent to point to the new page. </param>

					Converter.sqlite3Put4byte (this.aData, this.hdrOffset + 8, pgnoNew);
					///
///<summary>
///Release the reference to the new page. 
///</summary>

					BTreeMethods.releasePage(pNew);
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
*/public void copyNodeContent (MemPage pTo, ref int pRC)
			{
				if ((pRC) == Sqlite3.SQLITE_OK) {
					BtShared pBt = this.pBt;
					u8[] aFrom = this.aData;
					u8[] aTo = pTo.aData;
					int iFromHdr = this.hdrOffset;
					int iToHdr = ((pTo.pgno == 1) ? 100 : 0);
					int rc;
					int iData;
					Debug.Assert (this.isInit != 0);
					Debug.Assert (this.nFree >= iToHdr);
					Debug.Assert (get2byte (aFrom, iFromHdr + 5) <= (int)pBt.usableSize);
					///
///<summary>
///</summary>
///<param name="Copy the b">tree node content from page pFrom to page pTo. </param>

					iData = get2byte (aFrom, iFromHdr + 5);
					Buffer.BlockCopy (aFrom, iData, aTo, iData, (int)pBt.usableSize - iData);
					//memcpy(aTo[iData], ref aFrom[iData], pBt.usableSize-iData);
					Buffer.BlockCopy (aFrom, iFromHdr, aTo, iToHdr, this.cellOffset + 2 * this.nCell);
					//memcpy(aTo[iToHdr], ref aFrom[iFromHdr], pFrom.cellOffset + 2*pFrom.nCell);
					///
///<summary>
///Reinitialize page pTo so that the contents of the MemPage structure
///match the new data. The initialization of pTo can actually fail under
///fairly obscure circumstances, even though it is a copy of initialized 
///page pFrom.
///
///</summary>

					pTo.isInit = 0;
					rc = pTo.btreeInitPage ();
					if (rc != Sqlite3.SQLITE_OK) {
						pRC = rc;
						return;
					}
					///
///<summary>
///</summary>
///<param name="If this is an auto">map entries</param>
///<param name="for any b">tree or overflow pages that pTo now contains the pointers to.</param>
///<param name=""></param>

					#if !SQLITE_OMIT_AUTOVACUUM
					if (pBt.autoVacuum)
					#else
																																																																																																															if (false)
#endif
					 {
						pRC = pTo.setChildPtrmaps ();
					}
				}
			}

			// under C#; Try to reuse Memory
			public int balance_nonroot (
                ///Parent page of siblings being balanced 
                int iParentIdx, 
                ///Index of "the page" in pParent 

                u8[] aOvflSpace, 
                ///page-size bytes of space for parent ovfl </param>

                int isRoot
                ///True if pParent is a root-page 

			)
			{
                MemPage[] apOld = new MemPage[BTreeMethods.NB];
				///
///<summary>
///pPage and up to two siblings 
///</summary>

                MemPage[] apCopy = new MemPage[BTreeMethods.NB];
				///
///<summary>
///Private copies of apOld[] pages 
///</summary>

                MemPage[] apNew = new MemPage[BTreeMethods.NB + 2];
				///
///<summary>
///pPage and up to NB siblings after balancing 
///</summary>

                int[] apDiv = new int[BTreeMethods.NB - 1];
				///
///<summary>
///Divider cells in pParent 
///</summary>

                int[] cntNew = new int[BTreeMethods.NB + 2];
				///
///<summary>
///</summary>
///<param name="Index in aCell[] of cell after i">th page </param>

                int[] szNew = new int[BTreeMethods.NB + 2];
				///
///<summary>
///</summary>
///<param name="Combined size of cells place on i">th page </param>

				u16[] szCell = new u16[1];
				///
///<summary>
///Local size of all cells in apCell[] 
///</summary>

				BtShared pBt;
				///
///<summary>
///The whole database 
///</summary>

				int nCell = 0;
				///
///<summary>
///Number of cells in apCell[] 
///</summary>

				int nMaxCells = 0;
				///
///<summary>
///Allocated size of apCell, szCell, aFrom. 
///</summary>

				int nNew = 0;
				///
///<summary>
///Number of pages in apNew[] 
///</summary>

				int nOld;
				///
///<summary>
///Number of pages in apOld[] 
///</summary>

				int i, j, k;
				///
///<summary>
///Loop counters 
///</summary>

				int nxDiv;
				///
///<summary>
///Next divider slot in pParent.aCell[] 
///</summary>

				int rc = Sqlite3.SQLITE_OK;
				///
///<summary>
///The return code 
///</summary>

				u16 leafCorrection;
				///
///<summary>
///4 if pPage is a leaf.  0 if not 
///</summary>

				int leafData;
				///
///<summary>
///True if pPage is a leaf of a LEAFDATA tree 
///</summary>

				int usableSpace;
				///
///<summary>
///Bytes in pPage beyond the header 
///</summary>

				int pageFlags;
				///
///<summary>
///Value of pPage.aData[0] 
///</summary>

				int subtotal;
				///
///<summary>
///Subtotal of bytes in cells on one page 
///</summary>

				//int iSpace1 = 0;             /* First unused byte of aSpace1[] */
				int iOvflSpace = 0;
				///
///<summary>
///First unused byte of aOvflSpace[] 
///</summary>

				int szScratch;
				///
///<summary>
///Size of scratch memory requested 
///</summary>

				int pRight;
				///
///<summary>
///</summary>
///<param name="Location in parent of right">sibling pointer </param>

				u8[][] apCell = null;
				///
///<summary>
///All cells begin balanced 
///</summary>

				//u16[] szCell;                         /* Local size of all cells in apCell[] */
				//u8[] aSpace1;                         /* Space for copies of dividers cells */
				Pgno pgno;
				///
///<summary>
///Temp var to store a page number in 
///</summary>

				pBt = this.pBt;
				Debug.Assert (pBt.mutex.sqlite3_mutex_held());
				Debug.Assert (sqlite3PagerIswriteable (this.pDbPage));
				#if FALSE
																																																																																								TRACE("BALANCE: begin page %d child of %d\n", pPage.pgno, pParent.pgno);
#endif
				///
///<summary>
///At this point pParent may have at most one overflow cell. And if
///this overflow cell is present, it must be the cell with
///index iParentIdx. This scenario comes about when this function
///is called (indirectly) from sqlite3BtreeDelete().
///</summary>

				Debug.Assert (this.nOverflow == 0 || this.nOverflow == 1);
				Debug.Assert (this.nOverflow == 0 || this.aOvfl [0].idx == iParentIdx);
				//if( !aOvflSpace ){
				//  return SQLITE_NOMEM;
				//}
				///
///<summary>
///Find the sibling pages to balance. Also locate the cells in pParent
///that divide the siblings. An attempt is made to find NN siblings on
///either side of pPage. More siblings are taken from one side, however,
///if there are fewer than NN siblings on the other side. If pParent
///has NB or fewer children then all children of pParent are taken.
///
///This loop also drops the divider cells from the parent page. This
///way, the remainder of the function does not have to deal with any
///overflow cells in the parent page, since if any existed they will
///have already been removed.
///
///</summary>

				i = this.nOverflow + this.nCell;
				if (i < 2) {
					nxDiv = 0;
					nOld = i + 1;
				}
				else {
					nOld = 3;
					if (iParentIdx == 0) {
						nxDiv = 0;
					}
					else
						if (iParentIdx == i) {
							nxDiv = i - 2;
						}
						else {
							nxDiv = iParentIdx - 1;
						}
					i = 2;
				}
				if ((i + nxDiv - this.nOverflow) == this.nCell) {
					pRight = this.hdrOffset + 8;
					//&pParent.aData[pParent.hdrOffset + 8];
				}
				else {
					pRight = this.findCell (i + nxDiv - this.nOverflow);
				}
				pgno = Converter.sqlite3Get4byte (this.aData, pRight);
				while (true) {
					rc = BTreeMethods.getAndInitPage (pBt, pgno, ref apOld [i]);
					if (rc != 0) {
						//memset(apOld, 0, (i+1)*sizeof(MemPage*));
						goto balance_cleanup;
					}
					nMaxCells += 1 + apOld [i].nCell + apOld [i].nOverflow;
					if ((i--) == 0)
						break;
					if (i + nxDiv == this.aOvfl [0].idx && this.nOverflow != 0) {
						apDiv [i] = 0;
						// = pParent.aOvfl[0].pCell;
						pgno = Converter.sqlite3Get4byte (this.aOvfl [0].pCell, apDiv [i]);
						szNew [i] = this.cellSizePtr (apDiv [i]);
						this.nOverflow = 0;
					}
					else {
						apDiv [i] = this.findCell (i + nxDiv - this.nOverflow);
						pgno = Converter.sqlite3Get4byte (this.aData, apDiv [i]);
						szNew [i] = this.cellSizePtr (apDiv [i]);
						///
///<summary>
///Drop the cell from the parent page. apDiv[i] still points to
///the cell within the parent, even though it has been dropped.
///This is safe because dropping a cell only overwrites the first
///four bytes of it, and this function does not need the first
///four bytes of the divider cell. So the pointer is safe to use
///later on.
///
///</summary>
///<param name="Unless SQLite is compiled in secure">delete mode. In this case,</param>
///<param name="the dropCell() routine will overwrite the entire cell with zeroes.">the dropCell() routine will overwrite the entire cell with zeroes.</param>
///<param name="In this case, temporarily copy the cell into the aOvflSpace[]">In this case, temporarily copy the cell into the aOvflSpace[]</param>
///<param name="buffer. It will be copied out again as soon as the aSpace[] buffer">buffer. It will be copied out again as soon as the aSpace[] buffer</param>
///<param name="is allocated.  ">is allocated.  </param>

						//if (pBt.secureDelete)
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
						this.dropCell (i + nxDiv - this.nOverflow, szNew [i], ref rc);
					}
				}
				///
///<summary>
///</summary>
///<param name="Make nMaxCells a multiple of 4 in order to preserve 8">byte</param>
///<param name="alignment ">alignment </param>

				nMaxCells = (nMaxCells + 3) & ~3;
				///
///<summary>
///Allocate space for memory structures
///
///</summary>

				//k = pBt.pageSize + ROUND8(sizeof(MemPage));
				//szScratch =
				//     nMaxCells*sizeof(u8*)                       /* apCell */
				//   + nMaxCells*sizeof(u16)                       /* szCell */
				//   + pBt.pageSize                               /* aSpace1 */
				//   + k*nOld;                                     /* Page copies (apCopy) */
				apCell = malloc_cs.sqlite3ScratchMalloc (apCell, nMaxCells);
				//if( apCell==null ){
				//  rc = SQLITE_NOMEM;
				//  goto balance_cleanup;
				//}
				if (szCell.Length < nMaxCells)
					Array.Resize (ref szCell, nMaxCells);
				//(u16*)&apCell[nMaxCells];
				//aSpace1 = new byte[pBt.pageSize * (nMaxCells)];//  aSpace1 = (u8*)&szCell[nMaxCells];
				//Debug.Assert( EIGHT_BYTE_ALIGNMENT(aSpace1) );
				///
///<summary>
///Load pointers to all cells on sibling pages and the divider cells
///into the local apCell[] array.  Make copies of the divider cells
///into space obtained from aSpace1[] and remove the the divider Cells
///from pParent.
///
///If the siblings are on leaf pages, then the child pointers of the
///divider cells are stripped from the cells before they are copied
///into aSpace1[].  In this way, all cells in apCell[] are without
///child pointers.  If siblings are not leaves, then all cell in
///apCell[] include child pointers.  Either way, all cells in apCell[]
///are alike.
///
///leafCorrection:  4 if pPage is a leaf.  0 if pPage is not a leaf.
///leafData:  1 if pPage holds key+data and pParent holds only keys.
///
///</summary>

				leafCorrection = (u16)(apOld [0].leaf * 4);
				leafData = apOld [0].hasData;
				for (i = 0; i < nOld; i++) {
					int limit;
					///
///<summary>
///Before doing anything else, take a copy of the i'th original sibling
///The rest of this function will use data from the copies rather
///that the original pages since the original pages will be in the
///process of being overwritten.  
///</summary>

					//MemPage pOld = apCopy[i] = (MemPage*)&aSpace1[pBt.pageSize + k*i];
					//memcpy(pOld, apOld[i], sizeof(MemPage));
					//pOld.aData = (void*)&pOld[1];
					//memcpy(pOld.aData, apOld[i].aData, pBt.pageSize);
					MemPage pOld = apCopy [i] = apOld [i].Copy ();
					limit = pOld.nCell + pOld.nOverflow;
					if (pOld.nOverflow > 0 || true) {
						for (j = 0; j < limit; j++) {
							Debug.Assert (nCell < nMaxCells);
							//apCell[nCell] = findOverflowCell( pOld, j );
							//szCell[nCell] = cellSizePtr( pOld, apCell, nCell );
							int iFOFC = pOld.findOverflowCell (j);
							szCell [nCell] = pOld.cellSizePtr (iFOFC);
							// Copy the Data Locally
							if (apCell [nCell] == null)
								apCell [nCell] = new u8[szCell [nCell]];
							else
								if (apCell [nCell].Length < szCell [nCell])
									Array.Resize (ref apCell [nCell], szCell [nCell]);
							if (iFOFC < 0)
								// Overflow Cell
								Buffer.BlockCopy (pOld.aOvfl [-(iFOFC + 1)].pCell, 0, apCell [nCell], 0, szCell [nCell]);
							else
								Buffer.BlockCopy (pOld.aData, iFOFC, apCell [nCell], 0, szCell [nCell]);
							nCell++;
						}
					}
					else {
						u8[] aData = pOld.aData;
						u16 maskPage = pOld.maskPage;
						u16 cellOffset = pOld.cellOffset;
						for (j = 0; j < limit; j++) {
							Debugger.Break ();
							Debug.Assert (nCell < nMaxCells);
							apCell [nCell] = BTreeMethods.findCellv2 (aData, maskPage, cellOffset, j);
							szCell [nCell] = pOld.cellSizePtr (apCell [nCell]);
							nCell++;
						}
					}
					if (i < nOld - 1 && 0 == leafData) {
						u16 sz = (u16)szNew [i];
						byte[] pTemp = malloc_cs.sqlite3Malloc (sz + leafCorrection);
						Debug.Assert (nCell < nMaxCells);
						szCell [nCell] = sz;
						//pTemp = &aSpace1[iSpace1];
						//iSpace1 += sz;
						Debug.Assert (sz <= pBt.maxLocal + 23);
						//Debug.Assert(iSpace1 <= (int)pBt.pageSize);
						Buffer.BlockCopy (this.aData, apDiv [i], pTemp, 0, sz);
						//memcpy( pTemp, apDiv[i], sz );
						if (apCell [nCell] == null || apCell [nCell].Length < sz)
							Array.Resize (ref apCell [nCell], sz);
						Buffer.BlockCopy (pTemp, leafCorrection, apCell [nCell], 0, sz);
						//apCell[nCell] = pTemp + leafCorrection;
						Debug.Assert (leafCorrection == 0 || leafCorrection == 4);
						szCell [nCell] = (u16)(szCell [nCell] - leafCorrection);
						if (0 == pOld.leaf) {
							Debug.Assert (leafCorrection == 0);
							Debug.Assert (pOld.hdrOffset == 0);
							///
///<summary>
///The right pointer of the child page pOld becomes the left
///pointer of the divider cell 
///</summary>

							Buffer.BlockCopy (pOld.aData, 8, apCell [nCell], 0, 4);
							//memcpy( apCell[nCell], ref pOld.aData[8], 4 );
						}
						else {
							Debug.Assert (leafCorrection == 4);
							if (szCell [nCell] < 4) {
								///
///<summary>
///Do not allow any cells smaller than 4 bytes. 
///</summary>

								szCell [nCell] = 4;
							}
						}
						nCell++;
					}
				}
				///
///<summary>
///Figure out the number of pages needed to hold all nCell cells.
///Store this number in "k".  Also compute szNew[] which is the total
///</summary>
///<param name="size of all cells on the i">th page and cntNew[] which is the index</param>
///<param name="in apCell[] of the cell that divides page i from page i+1.">in apCell[] of the cell that divides page i from page i+1.</param>
///<param name="cntNew[k] should equal nCell.">cntNew[k] should equal nCell.</param>
///<param name=""></param>
///<param name="Values computed by this block:">Values computed by this block:</param>
///<param name=""></param>
///<param name="k: The total number of sibling pages">k: The total number of sibling pages</param>
///<param name="szNew[i]: Spaced used on the i">th sibling page.</param>
///<param name="cntNew[i]: Index in apCell[] and szCell[] for the first cell to">cntNew[i]: Index in apCell[] and szCell[] for the first cell to</param>
///<param name="the right of the i">th sibling page.</param>
///<param name="usableSpace: Number of bytes of space available on each sibling.">usableSpace: Number of bytes of space available on each sibling.</param>
///<param name=""></param>
///<param name=""></param>

				usableSpace = (int)pBt.usableSize - 12 + leafCorrection;
				for (subtotal = k = i = 0; i < nCell; i++) {
					Debug.Assert (i < nMaxCells);
					subtotal += szCell [i] + 2;
					if (subtotal > usableSpace) {
						szNew [k] = subtotal - szCell [i];
						cntNew [k] = i;
						if (leafData != 0) {
							i--;
						}
						subtotal = 0;
						k++;
                        if (k > BTreeMethods.NB + 1)
                        {
							rc = sqliteinth.SQLITE_CORRUPT_BKPT();
							goto balance_cleanup;
						}
					}
				}
				szNew [k] = subtotal;
				cntNew [k] = nCell;
				k++;
				///
///<summary>
///The packing computed by the previous block is biased toward the siblings
///on the left side.  The left siblings are always nearly full, while the
///</summary>
///<param name="right">most sibling might be nearly empty.  This block of code attempts</param>
///<param name="to adjust the packing of siblings to get a better balance.">to adjust the packing of siblings to get a better balance.</param>
///<param name=""></param>
///<param name="This adjustment is more than an optimization.  The packing above might">This adjustment is more than an optimization.  The packing above might</param>
///<param name="be so out of balance as to be illegal.  For example, the right">most</param>
///<param name="sibling might be completely empty.  This adjustment is not optional.">sibling might be completely empty.  This adjustment is not optional.</param>
///<param name=""></param>

				for (i = k - 1; i > 0; i--) {
					int szRight = szNew [i];
					///
///<summary>
///Size of sibling on the right 
///</summary>

					int szLeft = szNew [i - 1];
					///
///<summary>
///Size of sibling on the left 
///</summary>

					int r;
					///
///<summary>
///</summary>
///<param name="Index of right">most cell in left sibling </param>

					int d;
					///
///<summary>
///Index of first cell to the left of right sibling 
///</summary>

					r = cntNew [i - 1] - 1;
					d = r + 1 - leafData;
					Debug.Assert (d < nMaxCells);
					Debug.Assert (r < nMaxCells);
					while (szRight == 0 || szRight + szCell [d] + 2 <= szLeft - (szCell [r] + 2)) {
						szRight += szCell [d] + 2;
						szLeft -= szCell [r] + 2;
						cntNew [i - 1]--;
						r = cntNew [i - 1] - 1;
						d = r + 1 - leafData;
					}
					szNew [i] = szRight;
					szNew [i - 1] = szLeft;
				}
				///
///<summary>
///Either we found one or more cells (cntnew[0])>0) or pPage is
///a virtual root page.  A virtual root page is when the real root
///page is page 1 and we are the only child of that page.
///
///</summary>

				Debug.Assert (cntNew [0] > 0 || (this.pgno == 1 && this.nCell == 0));
				TRACE ("BALANCE: old: %d %d %d  ", apOld [0].pgno, nOld >= 2 ? apOld [1].pgno : 0, nOld >= 3 ? apOld [2].pgno : 0);
				///
///<summary>
///Allocate k new pages.  Reuse old pages where possible.
///
///</summary>

				if (apOld [0].pgno <= 1) {
					rc = sqliteinth.SQLITE_CORRUPT_BKPT();
					goto balance_cleanup;
				}
				pageFlags = apOld [0].aData [0];
				for (i = 0; i < k; i++) {
					MemPage pNew = new MemPage ();
					if (i < nOld) {
						pNew = apNew [i] = apOld [i];
						apOld [i] = null;
						rc = PagerMethods.sqlite3PagerWrite (pNew.pDbPage);
						nNew++;
						if (rc != 0)
							goto balance_cleanup;
					}
					else {
						Debug.Assert (i > 0);
						rc = BTreeMethods.allocateBtreePage (pBt, ref pNew, ref pgno, pgno, 0);
						if (rc != 0)
							goto balance_cleanup;
						apNew [i] = pNew;
						nNew++;
						///
///<summary>
///</summary>
///<param name="Set the pointer">map entry for the new sibling page. </param>

						#if !SQLITE_OMIT_AUTOVACUUM
						if (pBt.autoVacuum)
						#else
																																																																																																																																						if (false)
#endif
						 {
							pBt.ptrmapPut (pNew.pgno, PTRMAP_BTREE, this.pgno, ref rc);
							if (rc != Sqlite3.SQLITE_OK) {
								goto balance_cleanup;
							}
						}
					}
				}
				///
///<summary>
///Free any old pages that were not reused as new pages.
///
///</summary>

				while (i < nOld) {
                    BTreeMethods.freePage(apOld[i], ref rc);
					if (rc != 0)
						goto balance_cleanup;
					BTreeMethods.releasePage(apOld [i]);
					apOld [i] = null;
					i++;
				}
				///
///<summary>
///Put the new pages in accending order.  This helps to
///keep entries in the disk file in order so that a scan
///of the table is a linear scan through the file.  That
///in turn helps the operating system to deliver pages
///from the disk more rapidly.
///
///An O(n^2) insertion sort algorithm is used, but since
///n is never more than NB (a small constant), that should
///not be a problem.
///
///When NB==3, this one optimization makes the database
///about 25% faster for large insertions and deletions.
///
///</summary>

				for (i = 0; i < k - 1; i++) {
					int minV = (int)apNew [i].pgno;
					int minI = i;
					for (j = i + 1; j < k; j++) {
						if (apNew [j].pgno < (u32)minV) {
							minI = j;
							minV = (int)apNew [j].pgno;
						}
					}
					if (minI > i) {
						MemPage pT;
						pT = apNew [i];
						apNew [i] = apNew [minI];
						apNew [minI] = pT;
					}
				}
				TRACE ("new: %d(%d) %d(%d) %d(%d) %d(%d) %d(%d)\n", apNew [0].pgno, szNew [0], nNew >= 2 ? apNew [1].pgno : 0, nNew >= 2 ? szNew [1] : 0, nNew >= 3 ? apNew [2].pgno : 0, nNew >= 3 ? szNew [2] : 0, nNew >= 4 ? apNew [3].pgno : 0, nNew >= 4 ? szNew [3] : 0, nNew >= 5 ? apNew [4].pgno : 0, nNew >= 5 ? szNew [4] : 0);
				Debug.Assert (sqlite3PagerIswriteable (this.pDbPage));
				Converter.sqlite3Put4byte (this.aData, pRight, apNew [nNew - 1].pgno);
				///
///<summary>
///Evenly distribute the data in apCell[] across the new pages.
///Insert divider cells into pParent as necessary.
///
///</summary>

				j = 0;
				for (i = 0; i < nNew; i++) {
					///
///<summary>
///Assemble the new sibling page. 
///</summary>

					MemPage pNew = apNew [i];
					Debug.Assert (j < nMaxCells);
					pNew.zeroPage (pageFlags);
					pNew.assemblePage (cntNew [i] - j, apCell, szCell, j);
					Debug.Assert (pNew.nCell > 0 || (nNew == 1 && cntNew [0] == 0));
					Debug.Assert (pNew.nOverflow == 0);
					j = cntNew [i];
					///
///<summary>
///</summary>
///<param name="If the sibling page assembled above was not the right">most sibling,</param>
///<param name="insert a divider cell into the parent page.">insert a divider cell into the parent page.</param>
///<param name=""></param>

					Debug.Assert (i < nNew - 1 || j == nCell);
					if (j < nCell) {
						u8[] pCell;
						u8[] pTemp;
						int sz;
						Debug.Assert (j < nMaxCells);
						pCell = apCell [j];
						sz = szCell [j] + leafCorrection;
						pTemp = malloc_cs.sqlite3Malloc (sz);
						//&aOvflSpace[iOvflSpace];
						if (0 == pNew.leaf) {
							Buffer.BlockCopy (pCell, 0, pNew.aData, 8, 4);
							//memcpy( pNew.aData[8], pCell, 4 );
						}
						else
							if (leafData != 0) {
								///
///<summary>
///</summary>
///<param name="If the tree is a leaf">data tree, and the siblings are leaves,</param>
///<param name="then there is no divider cell in apCell[]. Instead, the divider">then there is no divider cell in apCell[]. Instead, the divider</param>
///<param name="cell consists of the integer key for the right">most cell of</param>
///<param name="the sibling">page assembled above only.</param>
///<param name=""></param>

								CellInfo info = new CellInfo ();
								j--;
								pNew.btreeParseCellPtr (apCell [j], ref info);
								pCell = pTemp;
                                sz = 4 + utilc.putVarint(pCell, 4, (u64)info.nKey);
								pTemp = null;
							}
							else {
								//------------ pCell -= 4;
								byte[] _pCell_4 = malloc_cs.sqlite3Malloc (pCell.Length + 4);
								Buffer.BlockCopy (pCell, 0, _pCell_4, 4, pCell.Length);
								pCell = _pCell_4;
								//
								///
///<summary>
///</summary>
///<param name="Obscure case for non">data trees: If the cell at pCell was</param>
///<param name="previously stored on a leaf node, and its reported size was 4">previously stored on a leaf node, and its reported size was 4</param>
///<param name="bytes, then it may actually be smaller than this">bytes, then it may actually be smaller than this</param>
///<param name="(see btreeParseCellPtr(), 4 bytes is the minimum size of">(see btreeParseCellPtr(), 4 bytes is the minimum size of</param>
///<param name="any cell). But it is important to pass the correct size to">any cell). But it is important to pass the correct size to</param>
///<param name="insertCell(), so reparse the cell now.">insertCell(), so reparse the cell now.</param>
///<param name=""></param>
///<param name="Note that this can never happen in an SQLite data file, as all">Note that this can never happen in an SQLite data file, as all</param>
///<param name="cells are at least 4 bytes. It only happens in b">trees used</param>
///<param name="to evaluate "IN (SELECT ...)" and similar clauses.">to evaluate "IN (SELECT ...)" and similar clauses.</param>
///<param name=""></param>

								if (szCell [j] == 4) {
									Debug.Assert (leafCorrection == 4);
									sz = this.cellSizePtr (pCell);
								}
							}
						iOvflSpace += sz;
						Debug.Assert (sz <= pBt.maxLocal + 23);
						Debug.Assert (iOvflSpace <= (int)pBt.pageSize);
						this.insertCell (nxDiv, pCell, sz, pTemp, pNew.pgno, ref rc);
						if (rc != Sqlite3.SQLITE_OK)
							goto balance_cleanup;
						Debug.Assert (sqlite3PagerIswriteable (this.pDbPage));
						j++;
						nxDiv++;
					}
				}
				Debug.Assert (j == nCell);
				Debug.Assert (nOld > 0);
				Debug.Assert (nNew > 0);
				if ((pageFlags & PTF_LEAF) == 0) {
					Buffer.BlockCopy (apCopy [nOld - 1].aData, 8, apNew [nNew - 1].aData, 8, 4);
					//u8* zChild = &apCopy[nOld - 1].aData[8];
					//memcpy( apNew[nNew - 1].aData[8], zChild, 4 );
				}
				if (isRoot != 0 && this.nCell == 0 && this.hdrOffset <= apNew [0].nFree) {
					///
///<summary>
///</summary>
///<param name="The root page of the b">tree now contains no cells. The only sibling</param>
///<param name="page is the right">child of the parent. Copy the contents of the</param>
///<param name="child page into the parent, decreasing the overall height of the">child page into the parent, decreasing the overall height of the</param>
///<param name="b">shallower"</param>
///<param name="sub">algorithm in some documentation.</param>
///<param name=""></param>
///<param name="If this is an auto">vacuum database, the call to copyNodeContent()</param>
///<param name="sets all pointer">map entries corresponding to database image pages</param>
///<param name="for which the pointer is stored within the content being copied.">for which the pointer is stored within the content being copied.</param>
///<param name=""></param>
///<param name="The second Debug.Assert below verifies that the child page is defragmented">The second Debug.Assert below verifies that the child page is defragmented</param>
///<param name="(it must be, as it was just reconstructed using assemblePage()). This">(it must be, as it was just reconstructed using assemblePage()). This</param>
///<param name="is important if the parent page happens to be page 1 of the database">is important if the parent page happens to be page 1 of the database</param>
///<param name="image.  ">image.  </param>

					Debug.Assert (nNew == 1);
					Debug.Assert (apNew [0].nFree == (get2byte (apNew [0].aData, 5) - apNew [0].cellOffset - apNew [0].nCell * 2));
					apNew [0].copyNodeContent (this, ref rc);
                    BTreeMethods.freePage(apNew[0], ref rc);
				}
				else
					#if !SQLITE_OMIT_AUTOVACUUM
					if (pBt.autoVacuum)
					#else
																																																																																																															if (false)
#endif
					 {
						///
///<summary>
///</summary>
///<param name="Fix the pointer">map entries for all the cells that were shifted around.</param>
///<param name="There are several different types of pointer">map entries that need to</param>
///<param name="be dealt with by this routine. Some of these have been set already, but">be dealt with by this routine. Some of these have been set already, but</param>
///<param name="many have not. The following is a summary:">many have not. The following is a summary:</param>
///<param name=""></param>
///<param name="1) The entries associated with new sibling pages that were not">1) The entries associated with new sibling pages that were not</param>
///<param name="siblings when this function was called. These have already">siblings when this function was called. These have already</param>
///<param name="been set. We don't need to worry about old siblings that were">been set. We don't need to worry about old siblings that were</param>
///<param name="moved to the free"> the freePage() code has taken care</param>
///<param name="of those.">of those.</param>
///<param name=""></param>
///<param name="2) The pointer">map entries associated with the first overflow</param>
///<param name="page in any overflow chains used by new divider cells. These">page in any overflow chains used by new divider cells. These</param>
///<param name="have also already been taken care of by the insertCell() code.">have also already been taken care of by the insertCell() code.</param>
///<param name=""></param>
///<param name="3) If the sibling pages are not leaves, then the child pages of">3) If the sibling pages are not leaves, then the child pages of</param>
///<param name="cells stored on the sibling pages may need to be updated.">cells stored on the sibling pages may need to be updated.</param>
///<param name=""></param>
///<param name="4) If the sibling pages are not internal intkey nodes, then any">4) If the sibling pages are not internal intkey nodes, then any</param>
///<param name="overflow pages used by these cells may need to be updated">overflow pages used by these cells may need to be updated</param>
///<param name="(internal intkey nodes never contain pointers to overflow pages).">(internal intkey nodes never contain pointers to overflow pages).</param>
///<param name=""></param>
///<param name="5) If the sibling pages are not leaves, then the pointer">map</param>
///<param name="entries for the right">child pages of each sibling may need</param>
///<param name="to be updated.">to be updated.</param>
///<param name=""></param>
///<param name="Cases 1 and 2 are dealt with above by other code. The next">Cases 1 and 2 are dealt with above by other code. The next</param>
///<param name="block deals with cases 3 and 4 and the one after that, case 5. Since">block deals with cases 3 and 4 and the one after that, case 5. Since</param>
///<param name="setting a pointer map entry is a relatively expensive operation, this">setting a pointer map entry is a relatively expensive operation, this</param>
///<param name="code only sets pointer map entries for child or overflow pages that have">code only sets pointer map entries for child or overflow pages that have</param>
///<param name="actually moved between pages.  ">actually moved between pages.  </param>

						MemPage pNew = apNew [0];
						MemPage pOld = apCopy [0];
						int nOverflow = pOld.nOverflow;
						int iNextOld = pOld.nCell + nOverflow;
						int iOverflow = (nOverflow != 0 ? pOld.aOvfl [0].idx : -1);
						j = 0;
						///
///<summary>
///Current 'old' sibling page 
///</summary>

						k = 0;
						///
///<summary>
///Current 'new' sibling page 
///</summary>

						for (i = 0; i < nCell; i++) {
							int isDivider = 0;
							while (i == iNextOld) {
								///
///<summary>
///Cell i is the cell immediately following the last cell on old
///sibling page j. If the siblings are not leaf pages of an
///</summary>
///<param name="intkey b">tree, then cell i was a divider cell. </param>

								pOld = apCopy [++j];
								iNextOld = i + (0 == leafData ? 1 : 0) + pOld.nCell + pOld.nOverflow;
								if (pOld.nOverflow != 0) {
									nOverflow = pOld.nOverflow;
									iOverflow = i + (0 == leafData ? 1 : 0) + pOld.aOvfl [0].idx;
								}
								isDivider = 0 == leafData ? 1 : 0;
							}
							Debug.Assert (nOverflow > 0 || iOverflow < i);
							Debug.Assert (nOverflow < 2 || pOld.aOvfl [0].idx == pOld.aOvfl [1].idx - 1);
							Debug.Assert (nOverflow < 3 || pOld.aOvfl [1].idx == pOld.aOvfl [2].idx - 1);
							if (i == iOverflow) {
								isDivider = 1;
								if ((--nOverflow) > 0) {
									iOverflow++;
								}
							}
							if (i == cntNew [k]) {
								///
///<summary>
///Cell i is the cell immediately following the last cell on new
///sibling page k. If the siblings are not leaf pages of an
///</summary>
///<param name="intkey b">tree, then cell i is a divider cell.  </param>

								pNew = apNew [++k];
								if (0 == leafData)
									continue;
							}
							Debug.Assert (j < nOld);
							Debug.Assert (k < nNew);
							///
///<summary>
///If the cell was originally divider cell (and is not now) or
///an overflow cell, or if the cell was located on a different sibling
///page before the balancing, then the pointer map entries associated
///with any child or overflow pages need to be updated.  
///</summary>

							if (isDivider != 0 || pOld.pgno != pNew.pgno) {
								if (0 == leafCorrection) {
									pBt.ptrmapPut (Converter.sqlite3Get4byte (apCell [i]), PTRMAP_BTREE, pNew.pgno, ref rc);
								}
								if (szCell [i] > pNew.minLocal) {
									pNew.ptrmapPutOvflPtr (apCell [i], ref rc);
								}
							}
						}
						if (0 == leafCorrection) {
							for (i = 0; i < nNew; i++) {
								u32 key = Converter.sqlite3Get4byte (apNew [i].aData, 8);
								pBt.ptrmapPut (key, PTRMAP_BTREE, apNew [i].pgno, ref rc);
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
				Debug.Assert (this.isInit != 0);
				TRACE ("BALANCE: finished: old=%d new=%d cells=%d\n", nOld, nNew, nCell);
				///
///<summary>
///Cleanup before returning.
///</summary>

				balance_cleanup:
                malloc_cs.sqlite3ScratchFree(apCell);
				for (i = 0; i < nOld; i++) {
					BTreeMethods.releasePage(apOld [i]);
				}
				for (i = 0; i < nNew; i++) {
					BTreeMethods.releasePage(apNew [i]);
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
///<param name="page and Sqlite3.SQLITE_OK is returned. In this case the caller is required">page and Sqlite3.SQLITE_OK is returned. In this case the caller is required</param>
///<param name="to call releasePage() on ppChild exactly once. If an error occurs,">to call releasePage() on ppChild exactly once. If an error occurs,</param>
///<param name="an error code is returned and ppChild is set to 0.">an error code is returned and ppChild is set to 0.</param>
*/public int balance_deeper (ref MemPage ppChild)
			{
				int rc;
				///
///<summary>
///Return value from subprocedures 
///</summary>

				MemPage pChild = null;
				///
///<summary>
///Pointer to a new child page 
///</summary>

				Pgno pgnoChild = 0;
				///
///<summary>
///Page number of the new child page 
///</summary>

				BtShared pBt = this.pBt;
				///
///<summary>
///The BTree 
///</summary>

				Debug.Assert (this.nOverflow > 0);
				Debug.Assert (pBt.mutex.sqlite3_mutex_held());
				///
///<summary>
///</summary>
///<param name="Make pRoot, the root page of the b">tree, writable. Allocate a new</param>
///<param name="page that will become the new right">child of pPage. Copy the contents</param>
///<param name="of the node stored on pRoot into the new child page.">of the node stored on pRoot into the new child page.</param>
///<param name=""></param>

				rc = PagerMethods.sqlite3PagerWrite (this.pDbPage);
				if (rc == Sqlite3.SQLITE_OK) {
					rc = BTreeMethods.allocateBtreePage (pBt, ref pChild, ref pgnoChild, this.pgno, 0);
					this.copyNodeContent (pChild, ref rc);
					#if !SQLITE_OMIT_AUTOVACUUM
					if (pBt.autoVacuum)
					#else
																																																																																																															if (false)
#endif
					 {
						pBt.ptrmapPut (pgnoChild, PTRMAP_BTREE, this.pgno, ref rc);
					}
				}
				if (rc != 0) {
					ppChild = null;
					BTreeMethods.releasePage(pChild);
					return rc;
				}
				Debug.Assert (sqlite3PagerIswriteable (pChild.pDbPage));
				Debug.Assert (sqlite3PagerIswriteable (this.pDbPage));
				Debug.Assert (pChild.nCell == this.nCell);
				TRACE ("BALANCE: copy root %d into %d\n", this.pgno, pChild.pgno);
				///
///<summary>
///Copy the overflow cells from pRoot to pChild 
///</summary>

				Array.Copy (this.aOvfl, pChild.aOvfl, this.nOverflow);
				//memcpy(pChild.aOvfl, pRoot.aOvfl, pRoot.nOverflow*sizeof(pRoot.aOvfl[0]));
				pChild.nOverflow = this.nOverflow;
				///
///<summary>
///</summary>
///<param name="Zero the contents of pRoot. Then install pChild as the right">child. </param>

				this.zeroPage (pChild.aData [0] & ~PTF_LEAF);
				Converter.sqlite3Put4byte (this.aData, this.hdrOffset + 8, pgnoChild);
				ppChild = pChild;
				return Sqlite3.SQLITE_OK;
			}

  /*
** Given a btree page and a cell index (0 means the first cell on
** the page, 1 means the second cell, and so forth) return a pointer
** to the cell content.
**
** This routine works only for pages that do not contain overflow cells.
*/

			public int findCell (int iCell)
			{
				return get2byte (this.aData, this.cellOffset + 2 * (iCell));
			}
		}
	}
}
