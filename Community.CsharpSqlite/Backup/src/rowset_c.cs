using System;
using System.Diagnostics;
using System.Text;
using i64 = System.Int64;
using u8 = System.Byte;
using u32 = System.UInt32;
using Pgno = System.UInt32;

namespace Community.CsharpSqlite
{
	using sqlite3_int64 = System.Int64;

	public partial class Sqlite3
	{
		///
///<summary>
///2008 December 3
///
///The author disclaims copyright to this source code.  In place of
///a legal notice, here is a blessing:
///
///May you do good and not evil.
///May you find forgiveness for yourself and forgive others.
///May you share freely, never taking more than you give.
///
///
///
///This module implements an object we call a "RowSet".
///
///The RowSet object is a collection of rowids.  Rowids
///are inserted into the RowSet in an arbitrary order.  Inserts
///can be intermixed with tests to see if a given rowid has been
///previously inserted into the RowSet.
///
///After all inserts are finished, it is possible to extract the
///elements of the RowSet in sorted order.  Once this extraction
///process has started, no new elements may be inserted.
///
///Hence, the primitive operations for a RowSet are:
///
///CREATE
///INSERT
///TEST
///SMALLEST
///DESTROY
///
///The CREATE and DESTROY primitives are the constructor and destructor,
///obviously.  The INSERT primitive adds a new element to the RowSet.
///TEST checks to see if an element is already in the RowSet.  SMALLEST
///extracts the least value from the RowSet.
///
///The INSERT primitive might allocate additional memory.  Memory is
///allocated in chunks so most INSERTs do no allocation.  There is an
///upper bound on the size of allocated memory.  No memory is freed
///until DESTROY.
///
///The TEST primitive includes a "batch" number.  The TEST primitive
///will only see elements that were inserted before the last change
///in the batch number.  In other words, if an INSERT occurs between
///two TESTs where the TESTs have the same batch nubmer, then the
///value added by the INSERT will not be visible to the second TEST.
///The initial batch number is zero, so if the very first TEST contains
///</summary>
///<param name="a non">zero batch number, it will see all prior INSERTs.</param>
///<param name=""></param>
///<param name="No INSERTs may occurs after a SMALLEST.  An assertion will fail if">No INSERTs may occurs after a SMALLEST.  An assertion will fail if</param>
///<param name="that is attempted.">that is attempted.</param>
///<param name=""></param>
///<param name="The cost of an INSERT is roughly constant.  (Sometime new memory">The cost of an INSERT is roughly constant.  (Sometime new memory</param>
///<param name="has to be allocated on an INSERT.)  The cost of a TEST with a new">has to be allocated on an INSERT.)  The cost of a TEST with a new</param>
///<param name="batch number is O(NlogN) where N is the number of elements in the RowSet.">batch number is O(NlogN) where N is the number of elements in the RowSet.</param>
///<param name="The cost of a TEST using the same batch number is O(logN).  The cost">The cost of a TEST using the same batch number is O(logN).  The cost</param>
///<param name="of the first SMALLEST is O(NlogN).  Second and subsequent SMALLEST">of the first SMALLEST is O(NlogN).  Second and subsequent SMALLEST</param>
///<param name="primitives are constant time.  The cost of DESTROY is O(N).">primitives are constant time.  The cost of DESTROY is O(N).</param>
///<param name=""></param>
///<param name="There is an added cost of O(N) when switching between TEST and">There is an added cost of O(N) when switching between TEST and</param>
///<param name="SMALLEST primitives.">SMALLEST primitives.</param>
///<param name=""></param>
///<param name="Included in SQLite3 port to C#">SQLite;  2008 Noah B Hart</param>
///<param name="C#">SQLite is an independent reimplementation of the SQLite software library</param>
///<param name=""></param>
///<param name="SQLITE_SOURCE_ID: 2010">23 18:52:01 42537b60566f288167f1b5864a5435986838e3a3</param>
///<param name=""></param>
///<param name=""></param>
///<param name=""></param>

		//#include "sqliteInt.h"
		///
///<summary>
///Target size for allocation chunks.
///
///</summary>

		//#define ROWSET_ALLOCATION_SIZE 1024
        const int ROWSET_ALLOCATION_SIZE = 256;

		///<summary>
		/// The number of rowset entries per allocation chunk.
		///
		///</summary>
		//#define ROWSET_ENTRY_PER_CHUNK  \
		//                     ((ROWSET_ALLOCATION_SIZE-8)/sizeof(struct RowSetEntry))
		const int ROWSET_ENTRY_PER_CHUNK = 63;

		///<summary>
		/// Each entry in a RowSet is an instance of the following object.
		///
		///</summary>
		public class RowSetEntry
		{
			public i64 v;

			///
///<summary>
///ROWID value for this entry 
///</summary>

			public RowSetEntry pRight;

			///
///<summary>
///Right subtree (larger entries) or list 
///</summary>

			public RowSetEntry pLeft;
		///
///<summary>
///Left subtree (smaller entries) 
///</summary>

		};


		///<summary>
		/// Index entries are allocated in large chunks (instances of the
		/// following structure) to reduce memory allocation overhead.  The
		/// chunks are kept on a linked list so that they can be deallocated
		/// when the RowSet is destroyed.
		///
		///</summary>
		public class RowSetChunk
		{
			public RowSetChunk pNextChunk;

			///
///<summary>
///Next chunk on list of them all 
///</summary>

			public RowSetEntry[] aEntry = new RowSetEntry[ROWSET_ENTRY_PER_CHUNK];
		///
///<summary>
///Allocated entries 
///</summary>

		};


		///<summary>
		/// A RowSet in an instance of the following structure.
		///
		/// A typedef of this structure if found in sqliteInt.h.
		///
		///</summary>
		public class RowSet
		{
			public RowSetChunk pChunk;

			///
///<summary>
///List of all chunk allocations 
///</summary>

			public sqlite3 db;

			///
///<summary>
///The database connection 
///</summary>

			public RowSetEntry pEntry;

			///
///<summary>
////* List of entries using pRight 
///</summary>

			public RowSetEntry pLast;

			///
///<summary>
///Last entry on the pEntry list 
///</summary>

			public RowSetEntry[] pFresh;

			///
///<summary>
///Source of new entry objects 
///</summary>

			public RowSetEntry pTree;

			///
///<summary>
///Binary tree of entries 
///</summary>

			public int nFresh;

			///
///<summary>
///Number of objects on pFresh 
///</summary>

			public bool isSorted;

			///
///<summary>
///True if pEntry is sorted 
///</summary>

			public u8 iBatch;

			///
///<summary>
///Current insert batch 
///</summary>

			public RowSet (sqlite3 db, int N)
			{
				this.pChunk = null;
				this.db = db;
				this.pEntry = null;
				this.pLast = null;
				this.pFresh = new RowSetEntry[N];
				this.pTree = null;
				this.nFresh = N;
				this.isSorted = true;
				this.iBatch = 0;
			}
		};


		///<summary>
		/// Turn bulk memory into a RowSet object.  N bytes of memory
		/// are available at pSpace.  The db pointer is used as a memory context
		/// for any subsequent allocations that need to occur.
		/// Return a pointer to the new RowSet object.
		///
		/// It must be the case that N is sufficient to make a Rowset.  If not
		/// an assertion fault occurs.
		///
		/// If N is larger than the minimum, use the surplus as an initial
		/// allocation of entries available to be filled.
		///
		///</summary>
		static RowSet sqlite3RowSetInit (sqlite3 db, object pSpace, u32 N)
		{
			RowSet p = new RowSet (db, (int)N);
			//Debug.Assert(N >= ROUND8(sizeof(*p)) );
			//  p = pSpace;
			//  p.pChunk = 0;
			//  p.db = db;
			//  p.pEntry = 0;
			//  p.pLast = 0;
			//  p.pTree = 0;
			//  p.pFresh =(struct RowSetEntry*)(ROUND8(sizeof(*p)) + (char*)p);
			//  p.nFresh = (u16)((N - ROUND8(sizeof(*p)))/sizeof(struct RowSetEntry));
			//  p.isSorted = 1;
			//  p.iBatch = 0;
			return p;
		}

		///<summary>
		/// Deallocate all chunks from a RowSet.  This frees all memory that
		/// the RowSet has allocated over its lifetime.  This routine is
		/// the destructor for the RowSet.
		///
		///</summary>
		static void sqlite3RowSetClear (RowSet p)
		{
			RowSetChunk pChunk, pNextChunk;
			for (pChunk = p.pChunk; pChunk != null; pChunk = pNextChunk) {
				pNextChunk = pChunk.pNextChunk;
				p.db.sqlite3DbFree (ref pChunk);
			}
			p.pChunk = null;
			p.nFresh = 0;
			p.pEntry = null;
			p.pLast = null;
			p.pTree = null;
			p.isSorted = true;
		}

		///<summary>
		/// Insert a new value into a RowSet.
		///
		/// The mallocFailed flag of the database connection is set if a
		/// memory allocation fails.
		///
		///</summary>
		static void sqlite3RowSetInsert (RowSet p, i64 rowid)
		{
			RowSetEntry pEntry;
			///
///<summary>
///The new entry 
///</summary>

			RowSetEntry pLast;
			///
///<summary>
///The last prior entry 
///</summary>

			Debug.Assert (p != null);
			if (p.nFresh == 0) {
				RowSetChunk pNew;
				pNew = new RowSetChunk ();
				//sqlite3DbMallocRaw(p.db, sizeof(*pNew));
				if (pNew == null) {
					return;
				}
				pNew.pNextChunk = p.pChunk;
				p.pChunk = pNew;
				p.pFresh = pNew.aEntry;
				p.nFresh = ROWSET_ENTRY_PER_CHUNK;
			}
			p.pFresh [p.pFresh.Length - p.nFresh] = new RowSetEntry ();
			pEntry = p.pFresh [p.pFresh.Length - p.nFresh];
			p.nFresh--;
			pEntry.v = rowid;
			pEntry.pRight = null;
			pLast = p.pLast;
			if (pLast != null) {
				if (p.isSorted && rowid <= pLast.v) {
					p.isSorted = false;
				}
				pLast.pRight = pEntry;
			}
			else {
				Debug.Assert (p.pEntry == null);
				///
///<summary>
///Fires if INSERT after SMALLEST 
///</summary>

				p.pEntry = pEntry;
			}
			p.pLast = pEntry;
		}

		///<summary>
		/// Merge two lists of RowSetEntry objects.  Remove duplicates.
		///
		/// The input lists are connected via pRight pointers and are
		/// assumed to each already be in sorted order.
		///
		///</summary>
		static RowSetEntry rowSetMerge (RowSetEntry pA, ///
///<summary>
///First sorted list to be merged 
///</summary>

		RowSetEntry pB///
///<summary>
///Second sorted list to be merged 
///</summary>

		)
		{
			RowSetEntry head = new RowSetEntry ();
			RowSetEntry pTail;
			pTail = head;
			while (pA != null && pB != null) {
				Debug.Assert (pA.pRight == null || pA.v <= pA.pRight.v);
				Debug.Assert (pB.pRight == null || pB.v <= pB.pRight.v);
				if (pA.v < pB.v) {
					pTail.pRight = pA;
					pA = pA.pRight;
					pTail = pTail.pRight;
				}
				else
					if (pB.v < pA.v) {
						pTail.pRight = pB;
						pB = pB.pRight;
						pTail = pTail.pRight;
					}
					else {
						pA = pA.pRight;
					}
			}
			if (pA != null) {
				Debug.Assert (pA.pRight == null || pA.v <= pA.pRight.v);
				pTail.pRight = pA;
			}
			else {
				Debug.Assert (pB == null || pB.pRight == null || pB.v <= pB.pRight.v);
				pTail.pRight = pB;
			}
			return head.pRight;
		}

		///<summary>
		/// Sort all elements on the pEntry list of the RowSet into ascending order.
		///
		///</summary>
		static void rowSetSort (RowSet p)
		{
			u32 i;
			RowSetEntry pEntry;
			RowSetEntry[] aBucket = new RowSetEntry[40];
			Debug.Assert (p.isSorted == false);
			//memset(aBucket, 0, sizeof(aBucket));
			while (p.pEntry != null) {
				pEntry = p.pEntry;
				p.pEntry = pEntry.pRight;
				pEntry.pRight = null;
				for (i = 0; aBucket [i] != null; i++) {
					pEntry = rowSetMerge (aBucket [i], pEntry);
					aBucket [i] = null;
				}
				aBucket [i] = pEntry;
			}
			pEntry = null;
			for (i = 0; i < aBucket.Length; i++)//sizeof(aBucket)/sizeof(aBucket[0])
			 {
				pEntry = rowSetMerge (pEntry, aBucket [i]);
			}
			p.pEntry = pEntry;
			p.pLast = null;
			p.isSorted = true;
		}

		///<summary>
		/// The input, pIn, is a binary tree (or subtree) of RowSetEntry objects.
		/// Convert this tree into a linked list connected by the pRight pointers
		/// and return pointers to the first and last elements of the new list.
		///
		///</summary>
		static void rowSetTreeToList (RowSetEntry pIn, ///
///<summary>
///Root of the input tree 
///</summary>

		ref RowSetEntry ppFirst, ///
///<summary>
///Write head of the output list here 
///</summary>

		ref RowSetEntry ppLast///
///<summary>
///Write tail of the output list here 
///</summary>

		)
		{
			Debug.Assert (pIn != null);
			if (pIn.pLeft != null) {
				RowSetEntry p = new RowSetEntry ();
				rowSetTreeToList (pIn.pLeft, ref ppFirst, ref p);
				p.pRight = pIn;
			}
			else {
				ppFirst = pIn;
			}
			if (pIn.pRight != null) {
				rowSetTreeToList (pIn.pRight, ref pIn.pRight, ref ppLast);
			}
			else {
				ppLast = pIn;
			}
			Debug.Assert ((ppLast).pRight == null);
		}

		///<summary>
		/// Convert a sorted list of elements (connected by pRight) into a binary
		/// tree with depth of iDepth.  A depth of 1 means the tree contains a single
		/// node taken from the head of *ppList.  A depth of 2 means a tree with
		/// three nodes.  And so forth.
		///
		/// Use as many entries from the input list as required and update the
		/// *ppList to point to the unused elements of the list.  If the input
		/// list contains too few elements, then construct an incomplete tree
		/// and leave *ppList set to NULL.
		///
		/// Return a pointer to the root of the constructed binary tree.
		///
		///</summary>
		static RowSetEntry rowSetNDeepTree (ref RowSetEntry ppList, int iDepth)
		{
			RowSetEntry p;
			///
///<summary>
///Root of the new tree 
///</summary>

			RowSetEntry pLeft;
			///
///<summary>
///Left subtree 
///</summary>

			if (ppList == null) {
				return null;
			}
			if (iDepth == 1) {
				p = ppList;
				ppList = p.pRight;
				p.pLeft = p.pRight = null;
				return p;
			}
			pLeft = rowSetNDeepTree (ref ppList, iDepth - 1);
			p = ppList;
			if (p == null) {
				return pLeft;
			}
			p.pLeft = pLeft;
			ppList = p.pRight;
			p.pRight = rowSetNDeepTree (ref ppList, iDepth - 1);
			return p;
		}

		///<summary>
		/// Convert a sorted list of elements into a binary tree. Make the tree
		/// as deep as it needs to be in order to contain the entire list.
		///
		///</summary>
		static RowSetEntry rowSetListToTree (RowSetEntry pList)
		{
			int iDepth;
			///
///<summary>
///Depth of the tree so far 
///</summary>

			RowSetEntry p;
			///
///<summary>
///Current tree root 
///</summary>

			RowSetEntry pLeft;
			///
///<summary>
///Left subtree 
///</summary>

			Debug.Assert (pList != null);
			p = pList;
			pList = p.pRight;
			p.pLeft = p.pRight = null;
			for (iDepth = 1; pList != null; iDepth++) {
				pLeft = p;
				p = pList;
				pList = p.pRight;
				p.pLeft = pLeft;
				p.pRight = rowSetNDeepTree (ref pList, iDepth);
			}
			return p;
		}

		///<summary>
		/// Convert the list in p.pEntry into a sorted list if it is not
		/// sorted already.  If there is a binary tree on p.pTree, then
		/// convert it into a list too and merge it into the p.pEntry list.
		///
		///</summary>
		static void rowSetToList (RowSet p)
		{
			if (!p.isSorted) {
				rowSetSort (p);
			}
			if (p.pTree != null) {
				RowSetEntry pHead = new RowSetEntry ();
				RowSetEntry pTail = new RowSetEntry ();
				rowSetTreeToList (p.pTree, ref pHead, ref pTail);
				p.pTree = null;
				p.pEntry = rowSetMerge (p.pEntry, pHead);
			}
		}

		///<summary>
		/// Extract the smallest element from the RowSet.
		/// Write the element into *pRowid.  Return 1 on success.  Return
		/// 0 if the RowSet is already empty.
		///
		/// After this routine has been called, the sqlite3RowSetInsert()
		/// routine may not be called again.
		///
		///</summary>
		static int sqlite3RowSetNext (RowSet p, ref i64 pRowid)
		{
			rowSetToList (p);
			if (p.pEntry != null) {
				pRowid = p.pEntry.v;
				p.pEntry = p.pEntry.pRight;
				if (p.pEntry == null) {
					sqlite3RowSetClear (p);
				}
				return 1;
			}
			else {
				return 0;
			}
		}

		///
///<summary>
///Check to see if element iRowid was inserted into the the rowset as
///part of any insert batch prior to iBatch.  Return 1 or 0.
///
///</summary>

		static int sqlite3RowSetTest (RowSet pRowSet, u8 iBatch, sqlite3_int64 iRowid)
		{
			RowSetEntry p;
			if (iBatch != pRowSet.iBatch) {
				if (pRowSet.pEntry != null) {
					rowSetToList (pRowSet);
					pRowSet.pTree = rowSetListToTree (pRowSet.pEntry);
					pRowSet.pEntry = null;
					pRowSet.pLast = null;
				}
				pRowSet.iBatch = iBatch;
			}
			p = pRowSet.pTree;
			while (p != null) {
				if (p.v < iRowid) {
					p = p.pRight;
				}
				else
					if (p.v > iRowid) {
						p = p.pLeft;
					}
					else {
						return 1;
					}
			}
			return 0;
		}
	}
}
