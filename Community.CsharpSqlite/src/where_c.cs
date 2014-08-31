using System;
using System.Diagnostics;
using System.Text;
using Bitmask=System.UInt64;
using i16=System.Int16;
using u8=System.Byte;
using u16=System.UInt16;
using u32=System.UInt32;
using sqlite3_int64=System.Int64;
namespace Community.CsharpSqlite {
	using sqlite3_value=Sqlite3.Mem;
	public partial class Sqlite3 {
		/*
    ** 2001 September 15
    **
    ** The author disclaims copyright to this source code.  In place of
    ** a legal notice, here is a blessing:
    **
    **    May you do good and not evil.
    **    May you find forgiveness for yourself and forgive others.
    **    May you share freely, never taking more than you give.
    **
    *************************************************************************
    ** This module contains C code that generates VDBE code used to process
    ** the WHERE clause of SQL statements.  This module is responsible for
    ** generating the code that loops through a table looking for applicable
    ** rows.  Indices are selected and used to speed the search when doing
    ** so is applicable.  Because this module is responsible for selecting
    ** indices, you might also think of this module as the "query optimizer".
    *************************************************************************
    **  Included in SQLite3 port to C#-SQLite;  2008 Noah B Hart
    **  C#-SQLite is an independent reimplementation of the SQLite software library
    **
    **  SQLITE_SOURCE_ID: 2011-05-19 13:26:54 ed1da510a239ea767a01dc332b667119fa3c908ecd7
    **
    *************************************************************************
    *///#include "sqliteInt.h"
		///<summary>
		/// Trace output macros
		///
		///</summary>
		#if (SQLITE_TEST) || (SQLITE_DEBUG)
																																										    static bool sqlite3WhereTrace = false;
#endif
		#if (SQLITE_TEST) && (SQLITE_DEBUG) && TRACE
																																										// define WHERETRACE(X)  if(sqlite3WhereTrace) sqlite3DebugPrintf X
static void WHERETRACE( string X, params object[] ap ) { if ( sqlite3WhereTrace ) sqlite3DebugPrintf( X, ap ); }
#else
		//# define WHERETRACE(X)
		static void WHERETRACE(string X,params object[] ap) {
		}
		#endif
		/* Forward reference
*///typedef struct WhereClause WhereClause;
		//typedef struct WhereMaskSet WhereMaskSet;
		//typedef struct WhereOrInfo WhereOrInfo;
		//typedef struct WhereAndInfo WhereAndInfo;
		//typedef struct WhereCost WhereCost;
		///<summary>
		/// The query generator uses an array of instances of this structure to
		/// help it analyze the subexpressions of the WHERE clause.  Each WHERE
		/// clause subexpression is separated from the others by AND operators,
		/// usually, or sometimes subexpressions separated by OR.
		///
		/// All WhereTerms are collected into a single WhereClause structure.
		/// The following identity holds:
		///
		///        WhereTerm.pWC.a[WhereTerm.idx] == WhereTerm
		///
		/// When a term is of the form:
		///
		///              X <op> <expr>
		///
		/// where X is a column name and <op> is one of certain operators,
		/// then WhereTerm.leftCursor and WhereTerm.u.leftColumn record the
		/// cursor number and column number for X.  WhereTerm.eOperator records
		/// the <op> using a bitmask encoding defined by WO_xxx below.  The
		/// use of a bitmask encoding for the operator allows us to search
		/// quickly for terms that match any of several different operators.
		///
		/// A WhereTerm might also be two or more subterms connected by OR:
		///
		///         (t1.X <op> <expr>) OR (t1.Y <op> <expr>) OR ....
		///
		/// In this second case, wtFlag as the TERM_ORINFO set and eOperator==WO_OR
		/// and the WhereTerm.u.pOrInfo field points to auxiliary information that
		/// is collected about the
		///
		/// If a term in the WHERE clause does not match either of the two previous
		/// categories, then eOperator==0.  The WhereTerm.pExpr field is still set
		/// to the original subexpression content and wtFlags is set up appropriately
		/// but no other fields in the WhereTerm object are meaningful.
		///
		/// When eOperator!=0, prereqRight and prereqAll record sets of cursor numbers,
		/// but they do so indirectly.  A single WhereMaskSet structure translates
		/// cursor number into bits and the translated bit is stored in the prereq
		/// fields.  The translation is used in order to maximize the number of
		/// bits that will fit in a Bitmask.  The VDBE cursor numbers might be
		/// spread out over the non-negative integers.  For example, the cursor
		/// numbers might be 3, 8, 9, 10, 20, 23, 41, and 45.  The WhereMaskSet
		/// translates these sparse cursor numbers into consecutive integers
		/// beginning with 0 in order to make the best possible use of the available
		/// bits in the Bitmask.  So, in the example above, the cursor numbers
		/// would be mapped into integers 0 through 7.
		///
		/// The number of terms in a join is limited by the number of bits
		/// in prereqRight and prereqAll.  The default is 64 bits, hence SQLite
		/// is only able to process joins with 64 or fewer tables.
		///
		///</summary>
		//typedef struct WhereTerm WhereTerm;
		public class WhereTerm {
			public Expr pExpr;
			/* Pointer to the subexpression that is this term */public int iParent;
			///<summary>
			///Disable pWC.a[iParent] when this term disabled
			///</summary>
			public int leftCursor;
			/* Cursor number of X in "X <op> <expr>" */public class _u {
				public int leftColumn;
				/* Column number of X in "X <op> <expr>" */public WhereOrInfo pOrInfo;
				/* Extra information if eOperator==WO_OR */public WhereAndInfo pAndInfo;
			/* Extra information if eOperator==WO_AND */}
			public _u u=new _u();
			public u16 eOperator;
			/* A WO_xx value describing <op> */public u8 wtFlags;
			/* TERM_xxx bit flags.  See below */public u8 nChild;
			/* Number of children that must disable us */public WhereClause pWC;
			/* The clause this term is part of */public Bitmask prereqRight;
			/* Bitmask of tables used by pExpr.pRight */public Bitmask prereqAll;
		/* Bitmask of tables referenced by pExpr */};

		/*
    ** Allowed values of WhereTerm.wtFlags
    *///#define TERM_DYNAMIC    0x01   /* Need to call sqlite3ExprDelete(db, ref pExpr) */
		//#define TERM_VIRTUAL    0x02   /* Added by the optimizer.  Do not code */
		//#define TERM_CODED      0x04   /* This term is already coded */
		//#define TERM_COPIED     0x08   /* Has a child */
		//#define TERM_ORINFO     0x10   /* Need to free the WhereTerm.u.pOrInfo object */
		//#define TERM_ANDINFO    0x20   /* Need to free the WhereTerm.u.pAndInfo obj */
		//#define TERM_OR_OK      0x40   /* Used during OR-clause processing */
		#if SQLITE_ENABLE_STAT2
																																										    //  define TERM_VNULL    0x80   /* Manufactured x>NULL or x<=NULL term */
#else
		//#  define TERM_VNULL    0x00   /* Disabled if not using stat2 */
		#endif
		const int TERM_DYNAMIC=0x01;
		/* Need to call sqlite3ExprDelete(db, ref pExpr) */const int TERM_VIRTUAL=0x02;
		/* Added by the optimizer.  Do not code */const int TERM_CODED=0x04;
		/* This term is already coded */const int TERM_COPIED=0x08;
		/* Has a child */const int TERM_ORINFO=0x10;
		/* Need to free the WhereTerm.u.pOrInfo object */const int TERM_ANDINFO=0x20;
		/* Need to free the WhereTerm.u.pAndInfo obj */const int TERM_OR_OK=0x40;
		/* Used during OR-clause processing */
		#if SQLITE_ENABLE_STAT2
																																										    const int TERM_VNULL = 0x80;  /* Manufactured x>NULL or x<=NULL term */
#else
		const int TERM_VNULL=0x00;
		///<summary>
		///Disabled if not using stat2
		///</summary>
		#endif
		///<summary>
		/// An instance of the following structure holds all information about a
		/// WHERE clause.  Mostly this is a container for one or more WhereTerms.
		///
		///</summary>
		public class WhereClause {
			public Parse pParse;
			/* The parser context */public WhereMaskSet pMaskSet;
			/* Mapping of table cursor numbers to bitmasks */public Bitmask vmask;
			/* Bitmask identifying virtual table cursors */public u8 op;
			/* Split operator.  TK_AND or TK_OR */public int nTerm;
			/* Number of terms */public int nSlot;
			/* Number of entries in a[] */public WhereTerm[] a;
			/* Each a[] describes a term of the WHERE cluase */
			#if (SQLITE_SMALL_STACK)
																																																															public WhereTerm[] aStatic = new WhereTerm[1];    /* Initial static space for a[] */
#else
			public WhereTerm[] aStatic=new WhereTerm[8];
			///<summary>
			///Initial static space for a[]
			///</summary>
			#endif
			public void CopyTo(WhereClause wc) {
				wc.pParse=this.pParse;
				wc.pMaskSet=new WhereMaskSet();
				this.pMaskSet.CopyTo(wc.pMaskSet);
				wc.op=this.op;
				wc.nTerm=this.nTerm;
				wc.nSlot=this.nSlot;
				wc.a=(WhereTerm[])this.a.Clone();
				wc.aStatic=(WhereTerm[])this.aStatic.Clone();
			}
			public///<summary>
			/// Deallocate a WhereClause structure.  The WhereClause structure
			/// itself is not freed.  This routine is the inverse of whereClauseInit().
			///
			///</summary>
			void whereClauseClear() {
				int i;
				WhereTerm a;
				sqlite3 db=this.pParse.db;
				for(i=this.nTerm-1;i>=0;i--)//, a++)
				 {
					a=this.a[i];
					if((a.wtFlags&TERM_DYNAMIC)!=0) {
						sqlite3ExprDelete(db,ref a.pExpr);
					}
					if((a.wtFlags&TERM_ORINFO)!=0) {
						whereOrInfoDelete(db,a.u.pOrInfo);
					}
					else
						if((a.wtFlags&TERM_ANDINFO)!=0) {
							whereAndInfoDelete(db,a.u.pAndInfo);
						}
				}
				if(this.a!=this.aStatic) {
					sqlite3DbFree(db,ref this.a);
				}
			}
			public void whereClauseInit(/* The WhereClause to be initialized */Parse pParse,/* The parsing context */WhereMaskSet pMaskSet/* Mapping from table cursor numbers to bitmasks */) {
				this.pParse=pParse;
				this.pMaskSet=pMaskSet;
				this.nTerm=0;
				this.nSlot=ArraySize(this.aStatic)-1;
				this.a=this.aStatic;
				this.vmask=0;
			}
			public///<summary>
			/// Add a single new WhereTerm entry to the WhereClause object pWC.
			/// The new WhereTerm object is constructed from Expr p and with wtFlags.
			/// The index in pWC.a[] of the new WhereTerm is returned on success.
			/// 0 is returned if the new WhereTerm could not be added due to a memory
			/// allocation error.  The memory allocation failure will be recorded in
			/// the db.mallocFailed flag so that higher-level functions can detect it.
			///
			/// This routine will increase the size of the pWC.a[] array as necessary.
			///
			/// If the wtFlags argument includes TERM_DYNAMIC, then responsibility
			/// for freeing the expression p is Debug.Assumed by the WhereClause object pWC.
			/// This is true even if this routine fails to allocate a new WhereTerm.
			///
			/// WARNING:  This routine might reallocate the space used to store
			/// WhereTerms.  All pointers to WhereTerms should be invalidated after
			/// calling this routine.  Such pointers may be reinitialized by referencing
			/// the pWC.a[] array.
			///
			///</summary>
			int whereClauseInsert(Expr p,u8 wtFlags) {
				WhereTerm pTerm;
				int idx;
				testcase(wtFlags&TERM_VIRTUAL);
				/* EV: R-00211-15100 */if(this.nTerm>=this.nSlot) {
					//WhereTerm pOld = pWC.a;
					sqlite3 db=this.pParse.db;
					Array.Resize(ref this.a,this.nSlot*2);
					//pWC.a = sqlite3DbMallocRaw(db, sizeof(pWC.a[0])*pWC.nSlot*2 );
					//if( pWC.a==null ){
					//  if( wtFlags & TERM_DYNAMIC ){
					//    sqlite3ExprDelete(db, ref p);
					//  }
					//  pWC.a = pOld;
					//  return 0;
					//}
					//memcpy(pWC.a, pOld, sizeof(pWC.a[0])*pWC.nTerm);
					//if( pOld!=pWC.aStatic ){
					//  sqlite3DbFree(db, ref pOld);
					//}
					//pWC.nSlot = sqlite3DbMallocSize(db, pWC.a)/sizeof(pWC.a[0]);
					this.nSlot=this.a.Length-1;
				}
				this.a[idx=this.nTerm++]=new WhereTerm();
				pTerm=this.a[idx];
				pTerm.pExpr=p;
				pTerm.wtFlags=wtFlags;
				pTerm.pWC=this;
				pTerm.iParent=-1;
				return idx;
			}
			public void whereSplit(Expr pExpr,int op) {
				this.op=(u8)op;
				if(pExpr==null)
					return;
				if(pExpr.op!=op) {
					this.whereClauseInsert(pExpr,0);
				}
				else {
					this.whereSplit(pExpr.pLeft,op);
					this.whereSplit(pExpr.pRight,op);
				}
			}
			public WhereTerm findTerm(/* The WHERE clause to be searched */int iCur,/* Cursor number of LHS */int iColumn,/* Column number of LHS */Bitmask notReady,/* RHS must not overlap with this mask */u32 op,/* Mask of WO_xx values describing operator */Index pIdx/* Must be compatible with this index, if not NULL */) {
				WhereTerm pTerm;
				int k;
				Debug.Assert(iCur>=0);
				op&=WO_ALL;
				for(k=this.nTerm;k!=0;k--)//, pTerm++)
				 {
					pTerm=this.a[this.nTerm-k];
					if(pTerm.leftCursor==iCur&&(pTerm.prereqRight&notReady)==0&&pTerm.u.leftColumn==iColumn&&(pTerm.eOperator&op)!=0) {
						if(pIdx!=null&&pTerm.eOperator!=WO_ISNULL) {
							Expr pX=pTerm.pExpr;
							CollSeq pColl;
							char idxaff;
							int j;
							Parse pParse=this.pParse;
							idxaff=pIdx.pTable.aCol[iColumn].affinity;
							if(!pX.sqlite3IndexAffinityOk(idxaff))
								continue;
							/* Figure out the collation sequence required from an index for
            ** it to be useful for optimising expression pX. Store this
            ** value in variable pColl.
            */Debug.Assert(pX.pLeft!=null);
							pColl=pParse.sqlite3BinaryCompareCollSeq(pX.pLeft,pX.pRight);
							Debug.Assert(pColl!=null||pParse.nErr!=0);
							for(j=0;pIdx.aiColumn[j]!=iColumn;j++) {
								if(NEVER(j>=pIdx.nColumn))
									return null;
							}
							if(pColl!=null&&!pColl.zName.Equals(pIdx.azColl[j],StringComparison.InvariantCultureIgnoreCase))
								continue;
						}
						return pTerm;
					}
				}
				return null;
			}
		}
		///<summary>
		/// A WhereTerm with eOperator==WO_OR has its u.pOrInfo pointer set to
		/// a dynamically allocated instance of the following structure.
		///
		///</summary>
		public class WhereOrInfo {
			public WhereClause wc=new WhereClause();
			/* Decomposition into subterms */public Bitmask indexable;
		/* Bitmask of all indexable tables in the clause */};

		///<summary>
		/// A WhereTerm with eOperator==WO_AND has its u.pAndInfo pointer set to
		/// a dynamically allocated instance of the following structure.
		///
		///</summary>
		public class WhereAndInfo {
			public WhereClause wc=new WhereClause();
		/* The subexpression broken out */};

		///<summary>
		/// An instance of the following structure keeps track of a mapping
		/// between VDBE cursor numbers and bits of the bitmasks in WhereTerm.
		///
		/// The VDBE cursor numbers are small integers contained in
		/// SrcList_item.iCursor and Expr.iTable fields.  For any given WHERE
		/// clause, the cursor numbers might not begin with 0 and they might
		/// contain gaps in the numbering sequence.  But we want to make maximum
		/// use of the bits in our bitmasks.  This structure provides a mapping
		/// from the sparse cursor numbers into consecutive integers beginning
		/// with 0.
		///
		/// If WhereMaskSet.ix[A]==B it means that The A-th bit of a Bitmask
		/// corresponds VDBE cursor number B.  The A-th bit of a bitmask is 1<<A.
		///
		/// For example, if the WHERE clause expression used these VDBE
		/// cursors:  4, 5, 8, 29, 57, 73.  Then the  WhereMaskSet structure
		/// would map those cursor numbers into bits 0 through 5.
		///
		/// Note that the mapping is not necessarily ordered.  In the example
		/// above, the mapping might go like this:  4.3, 5.1, 8.2, 29.0,
		/// 57.5, 73.4.  Or one of 719 other combinations might be used. It
		/// does not really matter.  What is important is that sparse cursor
		/// numbers all get mapped into bit numbers that begin with 0 and contain
		/// no gaps.
		///
		///</summary>
		public class WhereMaskSet {
			public int n;
			///<summary>
			///Number of Debug.Assigned cursor values
			///</summary>
			public int[] ix=new int[BMS];
			/* Cursor Debug.Assigned to each bit */public void CopyTo(WhereMaskSet wms) {
				wms.n=this.n;
				wms.ix=(int[])this.ix.Clone();
			}
		}
		/*
    ** A WhereCost object records a lookup strategy and the estimated
    ** cost of pursuing that strategy.
    */public class WhereCost {
			public WherePlan plan=new WherePlan();
			/* The lookup strategy */public double rCost;
			///<summary>
			///Overall cost of pursuing this search strategy
			///</summary>
			public Bitmask used;
			/* Bitmask of cursors used by this plan */public void Clear() {
				plan.Clear();
				rCost=0;
				used=0;
			}
		};

		/*
    ** Bitmasks for the operators that indices are able to exploit.  An
    ** OR-ed combination of these values can be used when searching for
    ** terms in the where clause.
    *///#define WO_IN     0x001
		//#define WO_EQ     0x002
		//#define WO_LT     (WO_EQ<<(TK_LT-TK_EQ))
		//#define WO_LE     (WO_EQ<<(TK_LE-TK_EQ))
		//#define WO_GT     (WO_EQ<<(TK_GT-TK_EQ))
		//#define WO_GE     (WO_EQ<<(TK_GE-TK_EQ))
		//#define WO_MATCH  0x040
		//#define WO_ISNULL 0x080
		//#define WO_OR     0x100       /* Two or more OR-connected terms */
		//#define WO_AND    0x200       /* Two or more AND-connected terms */
		//#define WO_NOOP   0x800       /* This term does not restrict search space */
		//#define WO_ALL    0xfff       /* Mask of all possible WO_* values */
		//#define WO_SINGLE 0x0ff       /* Mask of all non-compound WO_* values */
		const int WO_IN=0x001;
		const int WO_EQ=0x002;
		const int WO_LT=(WO_EQ<<(TK_LT-TK_EQ));
		const int WO_LE=(WO_EQ<<(TK_LE-TK_EQ));
		const int WO_GT=(WO_EQ<<(TK_GT-TK_EQ));
		const int WO_GE=(WO_EQ<<(TK_GE-TK_EQ));
		const int WO_MATCH=0x040;
		const int WO_ISNULL=0x080;
		const int WO_OR=0x100;
		/* Two or more OR-connected terms */const int WO_AND=0x200;
		/* Two or more AND-connected terms */const int WO_NOOP=0x800;
		/* This term does not restrict search space */const int WO_ALL=0xfff;
		/* Mask of all possible WO_* values */const int WO_SINGLE=0x0ff;
		/* Mask of all non-compound WO_* values *////<summary>
		/// Value for wsFlags returned by bestIndex() and stored in
		/// WhereLevel.wsFlags.  These flags determine which search
		/// strategies are appropriate.
		///
		/// The least significant 12 bits is reserved as a mask for WO_ values above.
		/// The WhereLevel.wsFlags field is usually set to WO_IN|WO_EQ|WO_ISNULL.
		/// But if the table is the right table of a left join, WhereLevel.wsFlags
		/// is set to WO_IN|WO_EQ.  The WhereLevel.wsFlags field can then be used as
		/// the "op" parameter to findTerm when we are resolving equality constraints.
		/// ISNULL constraints will then not be used on the right table of a left
		/// join.  Tickets #2177 and #2189.
		///
		///</summary>
		//#define WHERE_ROWID_EQ     0x00001000  /* rowid=EXPR or rowid IN (...) */
		//#define WHERE_ROWID_RANGE  0x00002000  /* rowid<EXPR and/or rowid>EXPR */
		//#define WHERE_COLUMN_EQ    0x00010000  /* x=EXPR or x IN (...) or x IS NULL */
		//#define WHERE_COLUMN_RANGE 0x00020000  /* x<EXPR and/or x>EXPR */
		//#define WHERE_COLUMN_IN    0x00040000  /* x IN (...) */
		//#define WHERE_COLUMN_NULL  0x00080000  /* x IS NULL */
		//#define WHERE_INDEXED      0x000f0000  /* Anything that uses an index */
		//#define WHERE_IN_ABLE      0x000f1000  /* Able to support an IN operator */
		//#define WHERE_NOT_FULLSCAN 0x100f3000  /* Does not do a full table scan */
		//#define WHERE_TOP_LIMIT    0x00100000  /* x<EXPR or x<=EXPR constraint */
		//#define WHERE_BTM_LIMIT    0x00200000  /* x>EXPR or x>=EXPR constraint */
		//#define WHERE_BOTH_LIMIT   0x00300000  /* Both x>EXPR and x<EXPR */
		//#define WHERE_IDX_ONLY     0x00800000  /* Use index only - omit table */
		//#define WHERE_ORDERBY      0x01000000  /* Output will appear in correct order */
		//#define WHERE_REVERSE      0x02000000  /* Scan in reverse order */
		//#define WHERE_UNIQUE       0x04000000  /* Selects no more than one row */
		//#define WHERE_VIRTUALTABLE 0x08000000  /* Use virtual-table processing */
		//#define WHERE_MULTI_OR     0x10000000  /* OR using multiple indices */
		//#define WHERE_TEMP_INDEX   0x20000000  /* Uses an ephemeral index */
		const int WHERE_ROWID_EQ=0x00001000;
		const int WHERE_ROWID_RANGE=0x00002000;
		const int WHERE_COLUMN_EQ=0x00010000;
		const int WHERE_COLUMN_RANGE=0x00020000;
		const int WHERE_COLUMN_IN=0x00040000;
		const int WHERE_COLUMN_NULL=0x00080000;
		const int WHERE_INDEXED=0x000f0000;
		const int WHERE_IN_ABLE=0x000f1000;
		const int WHERE_NOT_FULLSCAN=0x100f3000;
		const int WHERE_TOP_LIMIT=0x00100000;
		const int WHERE_BTM_LIMIT=0x00200000;
		const int WHERE_BOTH_LIMIT=0x00300000;
		const int WHERE_IDX_ONLY=0x00800000;
		const int WHERE_ORDERBY=0x01000000;
		const int WHERE_REVERSE=0x02000000;
		const int WHERE_UNIQUE=0x04000000;
		const int WHERE_VIRTUALTABLE=0x08000000;
		const int WHERE_MULTI_OR=0x10000000;
		const int WHERE_TEMP_INDEX=0x20000000;
		/*
    ** Initialize a preallocated WhereClause structure.
    *////<summary>
		///Forward reference
		///</summary>
		//static void whereClauseClear(WhereClause);
		///<summary>
		/// Deallocate all memory Debug.Associated with a WhereOrInfo object.
		///
		///</summary>
		static void whereOrInfoDelete(sqlite3 db,WhereOrInfo p) {
			p.wc.whereClauseClear();
			sqlite3DbFree(db,ref p);
		}
		///<summary>
		/// Deallocate all memory Debug.Associated with a WhereAndInfo object.
		///
		///</summary>
		static void whereAndInfoDelete(sqlite3 db,WhereAndInfo p) {
			p.wc.whereClauseClear();
			sqlite3DbFree(db,ref p);
		}
		/*
    ** This routine identifies subexpressions in the WHERE clause where
    ** each subexpression is separated by the AND operator or some other
    ** operator specified in the op parameter.  The WhereClause structure
    ** is filled with pointers to subexpressions.  For example:
    **
    **    WHERE  a=='hello' AND coalesce(b,11)<10 AND (c+12!=d OR c==22)
    **           \________/     \_______________/     \________________/
    **            slot[0]            slot[1]               slot[2]
    **
    ** The original WHERE clause in pExpr is unaltered.  All this routine
    ** does is make slot[] entries point to substructure within pExpr.
    **
    ** In the previous sentence and in the diagram, "slot[]" refers to
    ** the WhereClause.a[] array.  The slot[] array grows as needed to contain
    ** all terms of the WHERE clause.
    *////<summary>
		/// Initialize an expression mask set (a WhereMaskSet object)
		///
		///</summary>
		//#define initMaskSet(P)  memset(P, 0, sizeof(*P))
		///<summary>
		/// Return the bitmask for the given cursor number.  Return 0 if
		/// iCursor is not in the set.
		///
		///</summary>
		static Bitmask getMask(WhereMaskSet pMaskSet,int iCursor) {
			int i;
			Debug.Assert(pMaskSet.n<=(int)sizeof(Bitmask)*8);
			for(i=0;i<pMaskSet.n;i++) {
				if(pMaskSet.ix[i]==iCursor) {
					return ((Bitmask)1)<<i;
				}
			}
			return 0;
		}
		/*
    ** Create a new mask for cursor iCursor.
    **
    ** There is one cursor per table in the FROM clause.  The number of
    ** tables in the FROM clause is limited by a test early in the
    ** sqlite3WhereBegin() routine.  So we know that the pMaskSet.ix[]
    ** array will never overflow.
    */static void createMask(WhereMaskSet pMaskSet,int iCursor) {
			Debug.Assert(pMaskSet.n<ArraySize(pMaskSet.ix));
			pMaskSet.ix[pMaskSet.n++]=iCursor;
		}
		///<summary>
		/// This routine walks (recursively) an expression tree and generates
		/// a bitmask indicating which tables are used in that expression
		/// tree.
		///
		/// In order for this routine to work, the calling function must have
		/// previously invoked sqlite3ResolveExprNames() on the expression.  See
		/// the header comment on that routine for additional information.
		/// The sqlite3ResolveExprNames() routines looks for column names and
		/// sets their opcodes to TK_COLUMN and their Expr.iTable fields to
		/// the VDBE cursor number of the table.  This routine just has to
		/// translate the cursor numbers into bitmask values and OR all
		/// the bitmasks together.
		///
		///</summary>
		//static Bitmask exprListTableUsage(WhereMaskSet*, ExprList);
		//static Bitmask exprSelectTableUsage(WhereMaskSet*, Select);
		static Bitmask exprTableUsage(WhereMaskSet pMaskSet,Expr p) {
			Bitmask mask=0;
			if(p==null)
				return 0;
			if(p.op==TK_COLUMN) {
				mask=getMask(pMaskSet,p.iTable);
				return mask;
			}
			mask=exprTableUsage(pMaskSet,p.pRight);
			mask|=exprTableUsage(pMaskSet,p.pLeft);
			if(ExprHasProperty(p,EP_xIsSelect)) {
				mask|=exprSelectTableUsage(pMaskSet,p.x.pSelect);
			}
			else {
				mask|=exprListTableUsage(pMaskSet,p.x.pList);
			}
			return mask;
		}
		static Bitmask exprListTableUsage(WhereMaskSet pMaskSet,ExprList pList) {
			int i;
			Bitmask mask=0;
			if(pList!=null) {
				for(i=0;i<pList.nExpr;i++) {
					mask|=exprTableUsage(pMaskSet,pList.a[i].pExpr);
				}
			}
			return mask;
		}
		static Bitmask exprSelectTableUsage(WhereMaskSet pMaskSet,Select pS) {
			Bitmask mask=0;
			while(pS!=null) {
				mask|=exprListTableUsage(pMaskSet,pS.pEList);
				mask|=exprListTableUsage(pMaskSet,pS.pGroupBy);
				mask|=exprListTableUsage(pMaskSet,pS.pOrderBy);
				mask|=exprTableUsage(pMaskSet,pS.pWhere);
				mask|=exprTableUsage(pMaskSet,pS.pHaving);
				pS=pS.pPrior;
			}
			return mask;
		}
		/*
    ** Return TRUE if the given operator is one of the operators that is
    ** allowed for an indexable WHERE clause term.  The allowed operators are
    ** "=", "<", ">", "<=", ">=", and "IN".
    **
    ** IMPLEMENTATION-OF: R-59926-26393 To be usable by an index a term must be
    ** of one of the following forms: column = expression column > expression
    ** column >= expression column < expression column <= expression
    ** expression = column expression > column expression >= column
    ** expression < column expression <= column column IN
    ** (expression-list) column IN (subquery) column IS NULL
    */static bool allowedOp(int op) {
			Debug.Assert(TK_GT>TK_EQ&&TK_GT<TK_GE);
			Debug.Assert(TK_LT>TK_EQ&&TK_LT<TK_GE);
			Debug.Assert(TK_LE>TK_EQ&&TK_LE<TK_GE);
			Debug.Assert(TK_GE==TK_EQ+4);
			return op==TK_IN||(op>=TK_EQ&&op<=TK_GE)||op==TK_ISNULL;
		}
		///<summary>
		/// Swap two objects of type TYPE.
		///
		///</summary>
		//#define SWAP(TYPE,A,B) {TYPE t=A; A=B; B=t;}
		///<summary>
		/// Commute a comparison operator.  Expressions of the form "X op Y"
		/// are converted into "Y op X".
		///
		/// If a collation sequence is Debug.Associated with either the left or right
		/// side of the comparison, it remains Debug.Associated with the same side after
		/// the commutation. So "Y collate NOCASE op X" becomes
		/// "X collate NOCASE op Y". This is because any collation sequence on
		/// the left hand side of a comparison overrides any collation sequence
		/// attached to the right. For the same reason the EP_ExpCollate flag
		/// is not commuted.
		///
		///</summary>
		///<summary>
		/// Translate from TK_xx operator to WO_xx bitmask.
		///
		///</summary>
		static u16 operatorMask(int op) {
			u16 c;
			Debug.Assert(allowedOp(op));
			if(op==TK_IN) {
				c=WO_IN;
			}
			else
				if(op==TK_ISNULL) {
					c=WO_ISNULL;
				}
				else {
					Debug.Assert((WO_EQ<<(op-TK_EQ))<0x7fff);
					c=(u16)(WO_EQ<<(op-TK_EQ));
				}
			Debug.Assert(op!=TK_ISNULL||c==WO_ISNULL);
			Debug.Assert(op!=TK_IN||c==WO_IN);
			Debug.Assert(op!=TK_EQ||c==WO_EQ);
			Debug.Assert(op!=TK_LT||c==WO_LT);
			Debug.Assert(op!=TK_LE||c==WO_LE);
			Debug.Assert(op!=TK_GT||c==WO_GT);
			Debug.Assert(op!=TK_GE||c==WO_GE);
			return c;
		}
		/*
    ** Search for a term in the WHERE clause that is of the form "X <op> <expr>"
    ** where X is a reference to the iColumn of table iCur and <op> is one of
    ** the WO_xx operator codes specified by the op parameter.
    ** Return a pointer to the term.  Return 0 if not found.
    *////<summary>
		///Forward reference
		///</summary>
		//static void exprAnalyze(SrcList*, WhereClause*, int);
		///<summary>
		/// Call exprAnalyze on all terms in a WHERE clause.
		///
		///
		///
		///</summary>
		static void exprAnalyzeAll(SrcList pTabList,/* the FROM clause */WhereClause pWC/* the WHERE clause to be analyzed */) {
			int i;
			for(i=pWC.nTerm-1;i>=0;i--) {
				exprAnalyze(pTabList,pWC,i);
			}
		}
		#if !SQLITE_OMIT_LIKE_OPTIMIZATION
		///<summary>
		/// Check to see if the given expression is a LIKE or GLOB operator that
		/// can be optimized using inequality constraints.  Return TRUE if it is
		/// so and false if not.
		///
		/// In order for the operator to be optimizible, the RHS must be a string
		/// literal that does not begin with a wildcard.
		///</summary>
		#endif
		#if !SQLITE_OMIT_VIRTUALTABLE
		///<summary>
		/// Check to see if the given expression is of the form
		///
		///         column MATCH expr
		///
		/// If it is then return TRUE.  If not, return FALSE.
		///</summary>
		static int isMatchOfColumn(Expr pExpr/* Test this expression */) {
			ExprList pList;
			if(pExpr.op!=TK_FUNCTION) {
				return 0;
			}
			if(!pExpr.u.zToken.Equals("match",StringComparison.InvariantCultureIgnoreCase)) {
				return 0;
			}
			pList=pExpr.x.pList;
			if(pList.nExpr!=2) {
				return 0;
			}
			if(pList.a[1].pExpr.op!=TK_COLUMN) {
				return 0;
			}
			return 1;
		}
		#endif
		///<summary>
		/// If the pBase expression originated in the ON or USING clause of
		/// a join, then transfer the appropriate markings over to derived.
		///</summary>
		static void transferJoinMarkings(Expr pDerived,Expr pBase) {
			pDerived.flags=(u16)(pDerived.flags|pBase.flags&EP_FromJoin);
			pDerived.iRightJoinTable=pBase.iRightJoinTable;
		}
		#if !(SQLITE_OMIT_OR_OPTIMIZATION) && !(SQLITE_OMIT_SUBQUERY)
		///<summary>
		/// Analyze a term that consists of two or more OR-connected
		/// subterms.  So in:
		///
		///     ... WHERE  (a=5) AND (b=7 OR c=9 OR d=13) AND (d=13)
		///                          ^^^^^^^^^^^^^^^^^^^^
		///
		/// This routine analyzes terms such as the middle term in the above example.
		/// A WhereOrTerm object is computed and attached to the term under
		/// analysis, regardless of the outcome of the analysis.  Hence:
		///
		///     WhereTerm.wtFlags   |=  TERM_ORINFO
		///     WhereTerm.u.pOrInfo  =  a dynamically allocated WhereOrTerm object
		///
		/// The term being analyzed must have two or more of OR-connected subterms.
		/// A single subterm might be a set of AND-connected sub-subterms.
		/// Examples of terms under analysis:
		///
		///     (A)     t1.x=t2.y OR t1.x=t2.z OR t1.y=15 OR t1.z=t3.a+5
		///     (B)     x=expr1 OR expr2=x OR x=expr3
		///     (C)     t1.x=t2.y OR (t1.x=t2.z AND t1.y=15)
		///     (D)     x=expr1 OR (y>11 AND y<22 AND z LIKE '*hello*')
		///     (E)     (p.a=1 AND q.b=2 AND r.c=3) OR (p.x=4 AND q.y=5 AND r.z=6)
		///
		/// CASE 1:
		///
		/// If all subterms are of the form T.C=expr for some single column of C
		/// a single table T (as shown in example B above) then create a new virtual
		/// term that is an equivalent IN expression.  In other words, if the term
		/// being analyzed is:
		///
		///      x = expr1  OR  expr2 = x  OR  x = expr3
		///
		/// then create a new virtual term like this:
		///
		///      x IN (expr1,expr2,expr3)
		///
		/// CASE 2:
		///
		/// If all subterms are indexable by a single table T, then set
		///
		///     WhereTerm.eOperator              =  WO_OR
		///     WhereTerm.u.pOrInfo.indexable  |=  the cursor number for table T
		///
		/// A subterm is "indexable" if it is of the form
		/// "T.C <op> <expr>" where C is any column of table T and
		/// <op> is one of "=", "<", "<=", ">", ">=", "IS NULL", or "IN".
		/// A subterm is also indexable if it is an AND of two or more
		/// subsubterms at least one of which is indexable.  Indexable AND
		/// subterms have their eOperator set to WO_AND and they have
		/// u.pAndInfo set to a dynamically allocated WhereAndTerm object.
		///
		/// From another point of view, "indexable" means that the subterm could
		/// potentially be used with an index if an appropriate index exists.
		/// This analysis does not consider whether or not the index exists; that
		/// is something the bestIndex() routine will determine.  This analysis
		/// only looks at whether subterms appropriate for indexing exist.
		///
		/// All examples A through E above all satisfy case 2.  But if a term
		/// also statisfies case 1 (such as B) we know that the optimizer will
		/// always prefer case 1, so in that case we pretend that case 2 is not
		/// satisfied.
		///
		/// It might be the case that multiple tables are indexable.  For example,
		/// (E) above is indexable on tables P, Q, and R.
		///
		/// Terms that satisfy case 2 are candidates for lookup by using
		/// separate indices to find rowids for each subterm and composing
		/// the union of all rowids using a RowSet object.  This is similar
		/// to "bitmap indices" in other data_base engines.
		///
		/// OTHERWISE:
		///
		/// If neither case 1 nor case 2 apply, then leave the eOperator set to
		/// zero.  This term is not useful for search.
		///</summary>
		static void exprAnalyzeOrTerm(SrcList pSrc,/* the FROM clause */WhereClause pWC,/* the complete WHERE clause */int idxTerm/* Index of the OR-term to be analyzed */) {
			Parse pParse=pWC.pParse;
			/* Parser context */sqlite3 db=pParse.db;
			/* Data_base connection */WhereTerm pTerm=pWC.a[idxTerm];
			/* The term to be analyzed */Expr pExpr=pTerm.pExpr;
			/* The expression of the term */WhereMaskSet pMaskSet=pWC.pMaskSet;
			/* Table use masks */int i;
			/* Loop counters */WhereClause pOrWc;
			/* Breakup of pTerm into subterms */WhereTerm pOrTerm;
			/* A Sub-term within the pOrWc */WhereOrInfo pOrInfo;
			/* Additional information Debug.Associated with pTerm */Bitmask chngToIN;
			/* Tables that might satisfy case 1 */Bitmask indexable;
			/* Tables that are indexable, satisfying case 2 *//*
      ** Break the OR clause into its separate subterms.  The subterms are
      ** stored in a WhereClause structure containing within the WhereOrInfo
      ** object that is attached to the original OR clause term.
      */Debug.Assert((pTerm.wtFlags&(TERM_DYNAMIC|TERM_ORINFO|TERM_ANDINFO))==0);
			Debug.Assert(pExpr.op==TK_OR);
			pTerm.u.pOrInfo=pOrInfo=new WhereOrInfo();
			//sqlite3DbMallocZero(db, sizeof(*pOrInfo));
			if(pOrInfo==null)
				return;
			pTerm.wtFlags|=TERM_ORINFO;
			pOrWc=pOrInfo.wc;
			pOrWc.whereClauseInit(pWC.pParse,pMaskSet);
			pOrWc.whereSplit(pExpr,TK_OR);
			exprAnalyzeAll(pSrc,pOrWc);
			//      if ( db.mallocFailed != 0 ) return;
			Debug.Assert(pOrWc.nTerm>=2);
			/*
      ** Compute the set of tables that might satisfy cases 1 or 2.
      */indexable=~(Bitmask)0;
			chngToIN=~(pWC.vmask);
			for(i=pOrWc.nTerm-1;i>=0&&indexable!=0;i--)//, pOrTerm++ )
			 {
				pOrTerm=pOrWc.a[i];
				if((pOrTerm.eOperator&WO_SINGLE)==0) {
					WhereAndInfo pAndInfo;
					Debug.Assert(pOrTerm.eOperator==0);
					Debug.Assert((pOrTerm.wtFlags&(TERM_ANDINFO|TERM_ORINFO))==0);
					chngToIN=0;
					pAndInfo=new WhereAndInfo();
					//sqlite3DbMallocRaw(db, sizeof(*pAndInfo));
					if(pAndInfo!=null) {
						WhereClause pAndWC;
						WhereTerm pAndTerm;
						int j;
						Bitmask b=0;
						pOrTerm.u.pAndInfo=pAndInfo;
						pOrTerm.wtFlags|=TERM_ANDINFO;
						pOrTerm.eOperator=WO_AND;
						pAndWC=pAndInfo.wc;
						pAndWC.whereClauseInit(pWC.pParse,pMaskSet);
						pAndWC.whereSplit(pOrTerm.pExpr,TK_AND);
						exprAnalyzeAll(pSrc,pAndWC);
						//testcase( db.mallocFailed );
						////if ( 0 == db.mallocFailed )
						{
							for(j=0;j<pAndWC.nTerm;j++)//, pAndTerm++ )
							 {
								pAndTerm=pAndWC.a[j];
								Debug.Assert(pAndTerm.pExpr!=null);
								if(allowedOp(pAndTerm.pExpr.op)) {
									b|=getMask(pMaskSet,pAndTerm.leftCursor);
								}
							}
						}
						indexable&=b;
					}
				}
				else
					if((pOrTerm.wtFlags&TERM_COPIED)!=0) {
						/* Skip this term for now.  We revisit it when we process the
          ** corresponding TERM_VIRTUAL term */}
					else {
						Bitmask b;
						b=getMask(pMaskSet,pOrTerm.leftCursor);
						if((pOrTerm.wtFlags&TERM_VIRTUAL)!=0) {
							WhereTerm pOther=pOrWc.a[pOrTerm.iParent];
							b|=getMask(pMaskSet,pOther.leftCursor);
						}
						indexable&=b;
						if(pOrTerm.eOperator!=WO_EQ) {
							chngToIN=0;
						}
						else {
							chngToIN&=b;
						}
					}
			}
			/*
      ** Record the set of tables that satisfy case 2.  The set might be
      ** empty.
      */pOrInfo.indexable=indexable;
			pTerm.eOperator=(u16)(indexable==0?0:WO_OR);
			/*
      ** chngToIN holds a set of tables that *might* satisfy case 1.  But
      ** we have to do some additional checking to see if case 1 really
      ** is satisfied.
      **
      ** chngToIN will hold either 0, 1, or 2 bits.  The 0-bit case means
      ** that there is no possibility of transforming the OR clause into an
      ** IN operator because one or more terms in the OR clause contain
      ** something other than == on a column in the single table.  The 1-bit
      ** case means that every term of the OR clause is of the form
      ** "table.column=expr" for some single table.  The one bit that is set
      ** will correspond to the common table.  We still need to check to make
      ** sure the same column is used on all terms.  The 2-bit case is when
      ** the all terms are of the form "table1.column=table2.column".  It
      ** might be possible to form an IN operator with either table1.column
      ** or table2.column as the LHS if either is common to every term of
      ** the OR clause.
      **
      ** Note that terms of the form "table.column1=table.column2" (the
      ** same table on both sizes of the ==) cannot be optimized.
      */if(chngToIN!=0) {
				int okToChngToIN=0;
				/* True if the conversion to IN is valid */int iColumn=-1;
				/* Column index on lhs of IN operator */int iCursor=-1;
				/* Table cursor common to all terms */int j=0;
				/* Loop counter *//* Search for a table and column that appears on one side or the
        ** other of the == operator in every subterm.  That table and column
        ** will be recorded in iCursor and iColumn.  There might not be any
        ** such table and column.  Set okToChngToIN if an appropriate table
        ** and column is found but leave okToChngToIN false if not found.
        */for(j=0;j<2&&0==okToChngToIN;j++) {
					//pOrTerm = pOrWc.a;
					for(i=pOrWc.nTerm-1;i>=0;i--)//, pOrTerm++)
					 {
						pOrTerm=pOrWc.a[pOrWc.nTerm-1-i];
						Debug.Assert(pOrTerm.eOperator==WO_EQ);
						pOrTerm.wtFlags=(u8)(pOrTerm.wtFlags&~TERM_OR_OK);
						if(pOrTerm.leftCursor==iCursor) {
							/* This is the 2-bit case and we are on the second iteration and
              ** current term is from the first iteration.  So skip this term. */Debug.Assert(j==1);
							continue;
						}
						if((chngToIN&getMask(pMaskSet,pOrTerm.leftCursor))==0) {
							/* This term must be of the form t1.a==t2.b where t2 is in the
              ** chngToIN set but t1 is not.  This term will be either preceeded
              ** or follwed by an inverted copy (t2.b==t1.a).  Skip this term
              ** and use its inversion. */testcase(pOrTerm.wtFlags&TERM_COPIED);
							testcase(pOrTerm.wtFlags&TERM_VIRTUAL);
							Debug.Assert((pOrTerm.wtFlags&(TERM_COPIED|TERM_VIRTUAL))!=0);
							continue;
						}
						iColumn=pOrTerm.u.leftColumn;
						iCursor=pOrTerm.leftCursor;
						break;
					}
					if(i<0) {
						/* No candidate table+column was found.  This can only occur
            ** on the second iteration */Debug.Assert(j==1);
						Debug.Assert((chngToIN&(chngToIN-1))==0);
						Debug.Assert(chngToIN==getMask(pMaskSet,iCursor));
						break;
					}
					testcase(j==1);
					/* We have found a candidate table and column.  Check to see if that
          ** table and column is common to every term in the OR clause */okToChngToIN=1;
					for(;i>=0&&okToChngToIN!=0;i--)//, pOrTerm++)
					 {
						pOrTerm=pOrWc.a[pOrWc.nTerm-1-i];
						Debug.Assert(pOrTerm.eOperator==WO_EQ);
						if(pOrTerm.leftCursor!=iCursor) {
							pOrTerm.wtFlags=(u8)(pOrTerm.wtFlags&~TERM_OR_OK);
						}
						else
							if(pOrTerm.u.leftColumn!=iColumn) {
								okToChngToIN=0;
							}
							else {
								int affLeft,affRight;
								/* If the right-hand side is also a column, then the affinities
              ** of both right and left sides must be such that no type
              ** conversions are required on the right.  (Ticket #2249)
              */affRight=pOrTerm.pExpr.pRight.sqlite3ExprAffinity();
								affLeft=pOrTerm.pExpr.pLeft.sqlite3ExprAffinity();
								if(affRight!=0&&affRight!=affLeft) {
									okToChngToIN=0;
								}
								else {
									pOrTerm.wtFlags|=TERM_OR_OK;
								}
							}
					}
				}
				/* At this point, okToChngToIN is true if original pTerm satisfies
        ** case 1.  In that case, construct a new virtual term that is
        ** pTerm converted into an IN operator.
        **
        ** EV: R-00211-15100
        */if(okToChngToIN!=0) {
					Expr pDup;
					/* A transient duplicate expression */ExprList pList=null;
					/* The RHS of the IN operator */Expr pLeft=null;
					/* The LHS of the IN operator */Expr pNew;
					/* The complete IN operator */for(i=pOrWc.nTerm-1;i>=0;i--)//, pOrTerm++)
					 {
						pOrTerm=pOrWc.a[pOrWc.nTerm-1-i];
						if((pOrTerm.wtFlags&TERM_OR_OK)==0)
							continue;
						Debug.Assert(pOrTerm.eOperator==WO_EQ);
						Debug.Assert(pOrTerm.leftCursor==iCursor);
						Debug.Assert(pOrTerm.u.leftColumn==iColumn);
						pDup=sqlite3ExprDup(db,pOrTerm.pExpr.pRight,0);
						pList=pWC.pParse.sqlite3ExprListAppend(pList,pDup);
						pLeft=pOrTerm.pExpr.pLeft;
					}
					Debug.Assert(pLeft!=null);
					pDup=sqlite3ExprDup(db,pLeft,0);
					pNew=pParse.sqlite3PExpr(TK_IN,pDup,null,null);
					if(pNew!=null) {
						int idxNew;
						transferJoinMarkings(pNew,pExpr);
						Debug.Assert(!ExprHasProperty(pNew,EP_xIsSelect));
						pNew.x.pList=pList;
						idxNew=pWC.whereClauseInsert(pNew,TERM_VIRTUAL|TERM_DYNAMIC);
						testcase(idxNew==0);
						exprAnalyze(pSrc,pWC,idxNew);
						pTerm=pWC.a[idxTerm];
						pWC.a[idxNew].iParent=idxTerm;
						pTerm.nChild=1;
					}
					else {
						sqlite3ExprListDelete(db,ref pList);
					}
					pTerm.eOperator=WO_NOOP;
					/* case 1 trumps case 2 */}
			}
		}
		#endif
		///<summary>
		/// The input to this routine is an WhereTerm structure with only the
		/// "pExpr" field filled in.  The job of this routine is to analyze the
		/// subexpression and populate all the other fields of the WhereTerm
		/// structure.
		///
		/// If the expression is of the form "<expr> <op> X" it gets commuted
		/// to the standard form of "X <op> <expr>".
		///
		/// If the expression is of the form "X <op> Y" where both X and Y are
		/// columns, then the original expression is unchanged and a new virtual
		/// term of the form "Y <op> X" is added to the WHERE clause and
		/// analyzed separately.  The original term is marked with TERM_COPIED
		/// and the new term is marked with TERM_DYNAMIC (because it's pExpr
		/// needs to be freed with the WhereClause) and TERM_VIRTUAL (because it
		/// is a commuted copy of a prior term.)  The original term has nChild=1
		/// and the copy has idxParent set to the index of the original term.
		///</summary>
		static void exprAnalyze(SrcList pSrc,/* the FROM clause */WhereClause pWC,/* the WHERE clause */int idxTerm/* Index of the term to be analyzed */) {
			WhereTerm pTerm;
			/* The term to be analyzed */WhereMaskSet pMaskSet;
			/* Set of table index masks */Expr pExpr;
			/* The expression to be analyzed */Bitmask prereqLeft;
			/* Prerequesites of the pExpr.pLeft */Bitmask prereqAll;
			/* Prerequesites of pExpr */Bitmask extraRight=0;
			/* Extra dependencies on LEFT JOIN */Expr pStr1=null;
			/* RHS of LIKE/GLOB operator */bool isComplete=false;
			/* RHS of LIKE/GLOB ends with wildcard */bool noCase=false;
			/* LIKE/GLOB distinguishes case */int op;
			/* Top-level operator.  pExpr.op */Parse pParse=pWC.pParse;
			/* Parsing context */sqlite3 db=pParse.db;
			/* Data_base connection *///if ( db.mallocFailed != 0 )
			//{
			//  return;
			//}
			pTerm=pWC.a[idxTerm];
			pMaskSet=pWC.pMaskSet;
			pExpr=pTerm.pExpr;
			prereqLeft=exprTableUsage(pMaskSet,pExpr.pLeft);
			op=pExpr.op;
			if(op==TK_IN) {
				Debug.Assert(pExpr.pRight==null);
				if(ExprHasProperty(pExpr,EP_xIsSelect)) {
					pTerm.prereqRight=exprSelectTableUsage(pMaskSet,pExpr.x.pSelect);
				}
				else {
					pTerm.prereqRight=exprListTableUsage(pMaskSet,pExpr.x.pList);
				}
			}
			else
				if(op==TK_ISNULL) {
					pTerm.prereqRight=0;
				}
				else {
					pTerm.prereqRight=exprTableUsage(pMaskSet,pExpr.pRight);
				}
			prereqAll=exprTableUsage(pMaskSet,pExpr);
			if(ExprHasProperty(pExpr,EP_FromJoin)) {
				Bitmask x=getMask(pMaskSet,pExpr.iRightJoinTable);
				prereqAll|=x;
				extraRight=x-1;
				/* ON clause terms may not be used with an index
** on left table of a LEFT JOIN.  Ticket #3015 */}
			pTerm.prereqAll=prereqAll;
			pTerm.leftCursor=-1;
			pTerm.iParent=-1;
			pTerm.eOperator=0;
			if(allowedOp(op)&&(pTerm.prereqRight&prereqLeft)==0) {
				Expr pLeft=pExpr.pLeft;
				Expr pRight=pExpr.pRight;
				if(pLeft.op==TK_COLUMN) {
					pTerm.leftCursor=pLeft.iTable;
					pTerm.u.leftColumn=pLeft.iColumn;
					pTerm.eOperator=operatorMask(op);
				}
				if(pRight!=null&&pRight.op==TK_COLUMN) {
					WhereTerm pNew;
					Expr pDup;
					if(pTerm.leftCursor>=0) {
						int idxNew;
						pDup=sqlite3ExprDup(db,pExpr,0);
						//if ( db.mallocFailed != 0 )
						//{
						//  sqlite3ExprDelete( db, ref pDup );
						//  return;
						//}
						idxNew=pWC.whereClauseInsert(pDup,TERM_VIRTUAL|TERM_DYNAMIC);
						if(idxNew==0)
							return;
						pNew=pWC.a[idxNew];
						pNew.iParent=idxTerm;
						pTerm=pWC.a[idxTerm];
						pTerm.nChild=1;
						pTerm.wtFlags|=TERM_COPIED;
					}
					else {
						pDup=pExpr;
						pNew=pTerm;
					}
					pParse.exprCommute(pDup);
					pLeft=pDup.pLeft;
					pNew.leftCursor=pLeft.iTable;
					pNew.u.leftColumn=pLeft.iColumn;
					testcase((prereqLeft|extraRight)!=prereqLeft);
					pNew.prereqRight=prereqLeft|extraRight;
					pNew.prereqAll=prereqAll;
					pNew.eOperator=operatorMask(pDup.op);
				}
			}
			#if !SQLITE_OMIT_BETWEEN_OPTIMIZATION
			/* If a term is the BETWEEN operator, create two new virtual terms
** that define the range that the BETWEEN implements.  For example:
**
**      a BETWEEN b AND c
**
** is converted into:
**
**      (a BETWEEN b AND c) AND (a>=b) AND (a<=c)
**
** The two new terms are added onto the end of the WhereClause object.
** The new terms are "dynamic" and are children of the original BETWEEN
** term.  That means that if the BETWEEN term is coded, the children are
** skipped.  Or, if the children are satisfied by an index, the original
** BETWEEN term is skipped.
*/else
				if(pExpr.op==TK_BETWEEN&&pWC.op==TK_AND) {
					ExprList pList=pExpr.x.pList;
					int i;
					u8[] ops=new u8[] {
						TK_GE,
						TK_LE
					};
					Debug.Assert(pList!=null);
					Debug.Assert(pList.nExpr==2);
					for(i=0;i<2;i++) {
						Expr pNewExpr;
						int idxNew;
						pNewExpr=pParse.sqlite3PExpr(ops[i],sqlite3ExprDup(db,pExpr.pLeft,0),sqlite3ExprDup(db,pList.a[i].pExpr,0),null);
						idxNew=pWC.whereClauseInsert(pNewExpr,TERM_VIRTUAL|TERM_DYNAMIC);
						testcase(idxNew==0);
						exprAnalyze(pSrc,pWC,idxNew);
						pTerm=pWC.a[idxTerm];
						pWC.a[idxNew].iParent=idxTerm;
					}
					pTerm.nChild=2;
				}
				#endif
				#if !(SQLITE_OMIT_OR_OPTIMIZATION) && !(SQLITE_OMIT_SUBQUERY)
				/* Analyze a term that is composed of two or more subterms connected by
** an OR operator.
*/else
					if(pExpr.op==TK_OR) {
						Debug.Assert(pWC.op==TK_AND);
						exprAnalyzeOrTerm(pSrc,pWC,idxTerm);
						pTerm=pWC.a[idxTerm];
					}
			#endif
			#if !SQLITE_OMIT_LIKE_OPTIMIZATION
			/* Add constraints to reduce the search space on a LIKE or GLOB
** operator.
**
** A like pattern of the form "x LIKE 'abc%'" is changed into constraints
**
**          x>='abc' AND x<'abd' AND x LIKE 'abc%'
**
** The last character of the prefix "abc" is incremented to form the
** termination condition "abd".
*/if(pWC.op==TK_AND&&pParse.isLikeOrGlob(pExpr,ref pStr1,ref isComplete,ref noCase)!=0) {
				Expr pLeft;
				/* LHS of LIKE/GLOB operator */Expr pStr2;
				/* Copy of pStr1 - RHS of LIKE/GLOB operator */Expr pNewExpr1;
				Expr pNewExpr2;
				int idxNew1;
				int idxNew2;
				CollSeq pColl;
				/* Collating sequence to use */pLeft=pExpr.x.pList.a[1].pExpr;
				pStr2=sqlite3ExprDup(db,pStr1,0);
				////if ( 0 == db.mallocFailed )
				{
					int c,pC;
					/* Last character before the first wildcard */pC=pStr2.u.zToken[StringExtensions.sqlite3Strlen30(pStr2.u.zToken)-1];
					c=pC;
					if(noCase) {
						/* The point is to increment the last character before the first
            ** wildcard.  But if we increment '@', that will push it into the
            ** alphabetic range where case conversions will mess up the
            ** inequality.  To avoid this, make sure to also run the full
            ** LIKE on all candidate expressions by clearing the isComplete flag
            */if(c=='A'-1)
							isComplete=false;
						/* EV: R-64339-08207 */c=sqlite3UpperToLower[c];
					}
					pStr2.u.zToken=pStr2.u.zToken.Substring(0,StringExtensions.sqlite3Strlen30(pStr2.u.zToken)-1)+(char)(c+1);
					// pC = c + 1;
				}
				pColl=sqlite3FindCollSeq(db,SqliteEncoding.UTF8,noCase?"NOCASE":"BINARY",0);
				pNewExpr1=pParse.sqlite3PExpr(TK_GE,sqlite3ExprDup(db,pLeft,0).sqlite3ExprSetColl(pColl),pStr1,0);
				idxNew1=pWC.whereClauseInsert(pNewExpr1,TERM_VIRTUAL|TERM_DYNAMIC);
				testcase(idxNew1==0);
				exprAnalyze(pSrc,pWC,idxNew1);
				pNewExpr2=pParse.sqlite3PExpr(TK_LT,sqlite3ExprDup(db,pLeft,0).sqlite3ExprSetColl(pColl),pStr2,null);
				idxNew2=pWC.whereClauseInsert(pNewExpr2,TERM_VIRTUAL|TERM_DYNAMIC);
				testcase(idxNew2==0);
				exprAnalyze(pSrc,pWC,idxNew2);
				pTerm=pWC.a[idxTerm];
				if(isComplete) {
					pWC.a[idxNew1].iParent=idxTerm;
					pWC.a[idxNew2].iParent=idxTerm;
					pTerm.nChild=2;
				}
			}
			#endif
			#if !SQLITE_OMIT_VIRTUALTABLE
			/* Add a WO_MATCH auxiliary term to the constraint set if the
** current expression is of the form:  column MATCH expr.
** This information is used by the xBestIndex methods of
** virtual tables.  The native query optimizer does not attempt
** to do anything with MATCH functions.
*/if(isMatchOfColumn(pExpr)!=0) {
				int idxNew;
				Expr pRight,pLeft;
				WhereTerm pNewTerm;
				Bitmask prereqColumn,prereqExpr;
				pRight=pExpr.x.pList.a[0].pExpr;
				pLeft=pExpr.x.pList.a[1].pExpr;
				prereqExpr=exprTableUsage(pMaskSet,pRight);
				prereqColumn=exprTableUsage(pMaskSet,pLeft);
				if((prereqExpr&prereqColumn)==0) {
					Expr pNewExpr;
					pNewExpr=pParse.sqlite3PExpr(TK_MATCH,null,sqlite3ExprDup(db,pRight,0),null);
					idxNew=pWC.whereClauseInsert(pNewExpr,TERM_VIRTUAL|TERM_DYNAMIC);
					testcase(idxNew==0);
					pNewTerm=pWC.a[idxNew];
					pNewTerm.prereqRight=prereqExpr;
					pNewTerm.leftCursor=pLeft.iTable;
					pNewTerm.u.leftColumn=pLeft.iColumn;
					pNewTerm.eOperator=WO_MATCH;
					pNewTerm.iParent=idxTerm;
					pTerm=pWC.a[idxTerm];
					pTerm.nChild=1;
					pTerm.wtFlags|=TERM_COPIED;
					pNewTerm.prereqAll=pTerm.prereqAll;
				}
			}
			#endif
			#if SQLITE_ENABLE_STAT2
																																																															      /* When sqlite_stat2 histogram data is available an operator of the
  ** form "x IS NOT NULL" can sometimes be evaluated more efficiently
  ** as "x>NULL" if x is not an INTEGER PRIMARY KEY.  So construct a
  ** virtual term of that form.
  **
  ** Note that the virtual term must be tagged with TERM_VNULL.  This
  ** TERM_VNULL tag will suppress the not-null check at the beginning
  ** of the loop.  Without the TERM_VNULL flag, the not-null check at
  ** the start of the loop will prevent any results from being returned.
  */
      if ( pExpr.op == TK_NOTNULL
       && pExpr.pLeft.op == TK_COLUMN
       && pExpr.pLeft.iColumn >= 0
      )
      {
        Expr pNewExpr;
        Expr pLeft = pExpr.pLeft;
        int idxNew;
        WhereTerm pNewTerm;

        pNewExpr = sqlite3PExpr( pParse, TK_GT,
                                sqlite3ExprDup( db, pLeft, 0 ),
                                sqlite3PExpr( pParse, TK_NULL, 0, 0, 0 ), 0 );

        idxNew = whereClauseInsert( pWC, pNewExpr,
                                  TERM_VIRTUAL | TERM_DYNAMIC | TERM_VNULL );
        if ( idxNew != 0 )
        {
          pNewTerm = pWC.a[idxNew];
          pNewTerm.prereqRight = 0;
          pNewTerm.leftCursor = pLeft.iTable;
          pNewTerm.u.leftColumn = pLeft.iColumn;
          pNewTerm.eOperator = WO_GT;
          pNewTerm.iParent = idxTerm;
          pTerm = pWC.a[idxTerm];
          pTerm.nChild = 1;
          pTerm.wtFlags |= TERM_COPIED;
          pNewTerm.prereqAll = pTerm.prereqAll;
        }
      }
#endif
			/* Prevent ON clause terms of a LEFT JOIN from being used to drive
** an index for tables to the left of the join.
*/pTerm.prereqRight|=extraRight;
		}
		///<summary>
		/// Return TRUE if any of the expressions in pList.a[iFirst...] contain
		/// a reference to any table other than the iBase table.
		///
		///</summary>
		static bool referencesOtherTables(ExprList pList,/* Search expressions in ths list */WhereMaskSet pMaskSet,/* Mapping from tables to bitmaps */int iFirst,/* Be searching with the iFirst-th expression */int iBase/* Ignore references to this table */) {
			Bitmask allowed=~getMask(pMaskSet,iBase);
			while(iFirst<pList.nExpr) {
				if((exprTableUsage(pMaskSet,pList.a[iFirst++].pExpr)&allowed)!=0) {
					return true;
				}
			}
			return false;
		}
		///<summary>
		/// This routine decides if pIdx can be used to satisfy the ORDER BY
		/// clause.  If it can, it returns 1.  If pIdx cannot satisfy the
		/// ORDER BY clause, this routine returns 0.
		///
		/// pOrderBy is an ORDER BY clause from a SELECT statement.  pTab is the
		/// left-most table in the FROM clause of that same SELECT statement and
		/// the table has a cursor number of "_base".  pIdx is an index on pTab.
		///
		/// nEqCol is the number of columns of pIdx that are used as equality
		/// constraints.  Any of these columns may be missing from the ORDER BY
		/// clause and the match can still be a success.
		///
		/// All terms of the ORDER BY that match against the index must be either
		/// ASC or DESC.  (Terms of the ORDER BY clause past the end of a UNIQUE
		/// index do not need to satisfy this constraint.)  The pbRev value is
		/// set to 1 if the ORDER BY clause is all DESC and it is set to 0 if
		/// the ORDER BY clause is all ASC.
		///
		///</summary>
		static bool isSortingIndex(Parse pParse,/* Parsing context */WhereMaskSet pMaskSet,/* Mapping from table cursor numbers to bitmaps */Index pIdx,/* The index we are testing */int _base,/* Cursor number for the table to be sorted */ExprList pOrderBy,/* The ORDER BY clause */int nEqCol,/* Number of index columns with == constraints */int wsFlags,/* Index usages flags */ref int pbRev/* Set to 1 if ORDER BY is DESC */) {
			int i,j;
			/* Loop counters */int sortOrder=0;
			/* XOR of index and ORDER BY sort direction */int nTerm;
			/* Number of ORDER BY terms */ExprList_item pTerm;
			/* A term of the ORDER BY clause */sqlite3 db=pParse.db;
			Debug.Assert(pOrderBy!=null);
			nTerm=pOrderBy.nExpr;
			Debug.Assert(nTerm>0);
			/* Argument pIdx must either point to a 'real' named index structure, 
      ** or an index structure allocated on the stack by bestBtreeIndex() to
      ** represent the rowid index that is part of every table.  */Debug.Assert(!String.IsNullOrEmpty(pIdx.zName)||(pIdx.nColumn==1&&pIdx.aiColumn[0]==-1));
			/* Match terms of the ORDER BY clause against columns of
      ** the index.
      **
      ** Note that indices have pIdx.nColumn regular columns plus
      ** one additional column containing the rowid.  The rowid column
      ** of the index is also allowed to match against the ORDER BY
      ** clause.
      */for(i=j=0;j<nTerm&&i<=pIdx.nColumn;i++) {
				pTerm=pOrderBy.a[j];
				Expr pExpr;
				/* The expression of the ORDER BY pTerm */CollSeq pColl;
				/* The collating sequence of pExpr */int termSortOrder;
				/* Sort order for this term */int iColumn;
				/* The i-th column of the index.  -1 for rowid */int iSortOrder;
				/* 1 for DESC, 0 for ASC on the i-th index term */string zColl;
				/* Name of the collating sequence for i-th index term */pExpr=pTerm.pExpr;
				if(pExpr.op!=TK_COLUMN||pExpr.iTable!=_base) {
					/* Can not use an index sort on anything that is not a column in the
          ** left-most table of the FROM clause */break;
				}
				pColl=pParse.sqlite3ExprCollSeq(pExpr);
				if(null==pColl) {
					pColl=db.pDfltColl;
				}
				if(!String.IsNullOrEmpty(pIdx.zName)&&i<pIdx.nColumn) {
					iColumn=pIdx.aiColumn[i];
					if(iColumn==pIdx.pTable.iPKey) {
						iColumn=-1;
					}
					iSortOrder=pIdx.aSortOrder[i];
					zColl=pIdx.azColl[i];
				}
				else {
					iColumn=-1;
					iSortOrder=0;
					zColl=pColl.zName;
				}
				if(pExpr.iColumn!=iColumn||!pColl.zName.Equals(zColl,StringComparison.InvariantCultureIgnoreCase)) {
					/* Term j of the ORDER BY clause does not match column i of the index */if(i<nEqCol) {
						/* If an index column that is constrained by == fails to match an
            ** ORDER BY term, that is OK.  Just ignore that column of the index
            */continue;
					}
					else
						if(i==pIdx.nColumn) {
							/* Index column i is the rowid.  All other terms match. */break;
						}
						else {
							/* If an index column fails to match and is not constrained by ==
            ** then the index cannot satisfy the ORDER BY constraint.
            */return false;
						}
				}
				Debug.Assert(pIdx.aSortOrder!=null||iColumn==-1);
				Debug.Assert(pTerm.sortOrder==0||pTerm.sortOrder==1);
				Debug.Assert(iSortOrder==0||iSortOrder==1);
				termSortOrder=iSortOrder^pTerm.sortOrder;
				if(i>nEqCol) {
					if(termSortOrder!=sortOrder) {
						/* Indices can only be used if all ORDER BY terms past the
            ** equality constraints are all either DESC or ASC. */return false;
					}
				}
				else {
					sortOrder=termSortOrder;
				}
				j++;
				//pTerm++;
				if(iColumn<0&&!referencesOtherTables(pOrderBy,pMaskSet,j,_base)) {
					/* If the indexed column is the primary key and everything matches
          ** so far and none of the ORDER BY terms to the right reference other
          ** tables in the join, then we are Debug.Assured that the index can be used
          ** to sort because the primary key is unique and so none of the other
          ** columns will make any difference
          */j=nTerm;
				}
			}
			pbRev=sortOrder!=0?1:0;
			if(j>=nTerm) {
				/* All terms of the ORDER BY clause are covered by this index so
        ** this index can be used for sorting. */return true;
			}
			if(pIdx.onError!=OE_None&&i==pIdx.nColumn&&(wsFlags&WHERE_COLUMN_NULL)==0&&!referencesOtherTables(pOrderBy,pMaskSet,j,_base)) {
				/* All terms of this index match some prefix of the ORDER BY clause
        ** and the index is UNIQUE and no terms on the tail of the ORDER BY
        ** clause reference other tables in a join.  If this is all true then
        ** the order by clause is superfluous.  Not that if the matching
        ** condition is IS NULL then the result is not necessarily unique
        ** even on a UNIQUE index, so disallow those cases. */return true;
			}
			return false;
		}
		/*
    ** Prepare a crude estimate of the logarithm of the input value.
    ** The results need not be exact.  This is only used for estimating
    ** the total cost of performing operations with O(logN) or O(NlogN)
    ** complexity.  Because N is just a guess, it is no great tragedy if
    ** logN is a little off.
    */static double estLog(double N) {
			double logN=1;
			double x=10;
			while(N>x) {
				logN+=1;
				x*=10;
			}
			return logN;
		}
		///<summary>
		/// Two routines for printing the content of an sqlite3_index_info
		/// structure.  Used for testing and debugging only.  If neither
		/// SQLITE_TEST or SQLITE_DEBUG are defined, then these routines
		/// are no-ops.
		///
		///</summary>
		#if !(SQLITE_OMIT_VIRTUALTABLE) && (SQLITE_DEBUG)
																																										static void TRACE_IDX_INPUTS( sqlite3_index_info p )
{
int i;
if ( !sqlite3WhereTrace ) return;
for ( i = 0 ; i < p.nConstraint ; i++ )
{
sqlite3DebugPrintf( "  constraint[%d]: col=%d termid=%d op=%d usabled=%d\n",
i,
p.aConstraint[i].iColumn,
p.aConstraint[i].iTermOffset,
p.aConstraint[i].op,
p.aConstraint[i].usable );
}
for ( i = 0 ; i < p.nOrderBy ; i++ )
{
sqlite3DebugPrintf( "  orderby[%d]: col=%d desc=%d\n",
i,
p.aOrderBy[i].iColumn,
p.aOrderBy[i].desc );
}
}
static void TRACE_IDX_OUTPUTS( sqlite3_index_info p )
{
int i;
if ( !sqlite3WhereTrace ) return;
for ( i = 0 ; i < p.nConstraint ; i++ )
{
sqlite3DebugPrintf( "  usage[%d]: argvIdx=%d omit=%d\n",
i,
p.aConstraintUsage[i].argvIndex,
p.aConstraintUsage[i].omit );
}
sqlite3DebugPrintf( "  idxNum=%d\n", p.idxNum );
sqlite3DebugPrintf( "  idxStr=%s\n", p.idxStr );
sqlite3DebugPrintf( "  orderByConsumed=%d\n", p.orderByConsumed );
sqlite3DebugPrintf( "  estimatedCost=%g\n", p.estimatedCost );
}
#else
		//#define TRACE_IDX_INPUTS(A)
		static void TRACE_IDX_INPUTS(sqlite3_index_info p) {
		}
		//#define TRACE_IDX_OUTPUTS(A)
		static void TRACE_IDX_OUTPUTS(sqlite3_index_info p) {
		}
		#endif
		///<summary>
		/// Required because bestIndex() is called by bestOrClauseIndex()
		///</summary>
		//static void bestIndex(
		//Parse*, WhereClause*, struct SrcList_item*, 
		//Bitmask, ExprList*, WhereCost);
		///<summary>
		/// This routine attempts to find an scanning strategy that can be used
		/// to optimize an 'OR' expression that is part of a WHERE clause.
		///
		/// The table associated with FROM clause term pSrc may be either a
		/// regular B-Tree table or a virtual table.
		///
		///</summary>
		#if !SQLITE_OMIT_AUTOMATIC_INDEX
		///<summary>
		/// Return TRUE if the WHERE clause term pTerm is of a form where it
		/// could be used with an index to access pSrc, assuming an appropriate
		/// index existed.
		///</summary>
		static int termCanDriveIndex(WhereTerm pTerm,/* WHERE clause term to check */SrcList_item pSrc,/* Table we are trying to access */Bitmask notReady/* Tables in outer loops of the join */) {
			char aff;
			if(pTerm.leftCursor!=pSrc.iCursor)
				return 0;
			if(pTerm.eOperator!=WO_EQ)
				return 0;
			if((pTerm.prereqRight&notReady)!=0)
				return 0;
			aff=pSrc.pTab.aCol[pTerm.u.leftColumn].affinity;
			if(!pTerm.pExpr.sqlite3IndexAffinityOk(aff))
				return 0;
			return 1;
		}
		#endif
		#if !SQLITE_OMIT_AUTOMATIC_INDEX
		///<summary>
		/// If the query plan for pSrc specified in pCost is a full table scan
		/// and indexing is allows (if there is no NOT INDEXED clause) and it
		/// possible to construct a transient index that would perform better
		/// than a full table scan even when the cost of constructing the index
		/// is taken into account, then alter the query plan to use the
		/// transient index.
		///</summary>
		#else
																																										// define bestAutomaticIndex(A,B,C,D,E)  /* no-op */
static void bestAutomaticIndex(
Parse pParse,              /* The parsing context */
WhereClause pWC,           /* The WHERE clause */
SrcList_item pSrc,         /* The FROM clause term to search */
Bitmask notReady,          /* Mask of cursors that are not available */
WhereCost pCost            /* Lowest cost query plan */
){}
#endif
		#if !SQLITE_OMIT_AUTOMATIC_INDEX
		///<summary>
		/// Generate code to construct the Index object for an automatic index
		/// and to set up the WhereLevel object pLevel so that the code generator
		/// makes use of the automatic index.
		///</summary>
		#endif
		#if !SQLITE_OMIT_VIRTUALTABLE
		///<summary>
		/// Allocate and populate an sqlite3_index_info structure. It is the
		/// responsibility of the caller to eventually release the structure
		/// by passing the pointer returned by this function to //sqlite3_free().
		///</summary>
		///<summary>
		/// The table object reference passed as the second argument to this function
		/// must represent a virtual table. This function invokes the xBestIndex()
		/// method of the virtual table with the sqlite3_index_info pointer passed
		/// as the argument.
		///
		/// If an error occurs, pParse is populated with an error message and a
		/// non-zero value is returned. Otherwise, 0 is returned and the output
		/// part of the sqlite3_index_info structure is left populated.
		///
		/// Whether or not an error is returned, it is the responsibility of the
		/// caller to eventually free p.idxStr if p.needToFreeIdxStr indicates
		/// that this is required.
		///
		///</summary>
		/*
    ** Compute the best index for a virtual table.
    **
    ** The best index is computed by the xBestIndex method of the virtual
    ** table module.  This routine is really just a wrapper that sets up
    ** the sqlite3_index_info structure that is used to communicate with
    ** xBestIndex.
    **
    ** In a join, this routine might be called multiple times for the
    ** same virtual table.  The sqlite3_index_info structure is created
    ** and initialized on the first invocation and reused on all subsequent
    ** invocations.  The sqlite3_index_info structure is also used when
    ** code is generated to access the virtual table.  The whereInfoDelete()
    ** routine takes care of freeing the sqlite3_index_info structure after
    ** everybody has finished with it.
    */
		#endif
		/*
** Argument pIdx is a pointer to an index structure that has an array of
** SQLITE_INDEX_SAMPLES evenly spaced samples of the first indexed column
** stored in Index.aSample. These samples divide the domain of values stored
** the index into (SQLITE_INDEX_SAMPLES+1) regions.
** Region 0 contains all values less than the first sample value. Region
** 1 contains values between the first and second samples.  Region 2 contains
** values between samples 2 and 3.  And so on.  Region SQLITE_INDEX_SAMPLES
** contains values larger than the last sample.
**
** If the index contains many duplicates of a single value, then it is
** possible that two or more adjacent samples can hold the same value.
** When that is the case, the smallest possible region code is returned
** when roundUp is false and the largest possible region code is returned
** when roundUp is true.
**
** If successful, this function determines which of the regions value 
** pVal lies in, sets *piRegion to the region index (a value between 0
** and SQLITE_INDEX_SAMPLES+1, inclusive) and returns SQLITE_OK.
** Or, if an OOM occurs while converting text values between encodings,
** SQLITE_NOMEM is returned and *piRegion is undefined.
*/
		#if SQLITE_ENABLE_STAT2
																																										    static int whereRangeRegion(
    Parse pParse,               /* Database connection */
    Index pIdx,                 /* Index to consider domain of */
    sqlite3_value pVal,         /* Value to consider */
    int roundUp,                /* Return largest valid region if true */
    out int piRegion            /* OUT: Region of domain in which value lies */
    )
    {
      piRegion = 0;
      Debug.Assert( roundUp == 0 || roundUp == 1 );
      if ( ALWAYS( pVal ) )
      {
        IndexSample[] aSample = pIdx.aSample;
        int i = 0;
        int eType = sqlite3_value_type( pVal );

        if ( eType == SQLITE_INTEGER || eType == SQLITE_FLOAT )
        {
          double r = sqlite3_value_double( pVal );
          for ( i = 0; i < SQLITE_INDEX_SAMPLES; i++ )
          {
            if ( aSample[i].eType == SQLITE_NULL )
              continue;
            if ( aSample[i].eType >= SQLITE_TEXT )
              break;
            if ( roundUp != 0 )
            {
              if ( aSample[i].u.r > r )
                break;
            }
            else
            {
              if ( aSample[i].u.r >= r )
                break;
            }
          }
        }
        else if ( eType == SQLITE_NULL )
        {
          i = 0;
          if ( roundUp != 0 )
          {
            while ( i < SQLITE_INDEX_SAMPLES && aSample[i].eType == SQLITE_NULL )
              i++;
          }
        }
        else
        {
          sqlite3 db = pParse.db;
          CollSeq pColl;
          string z;
          int n;

          /* pVal comes from sqlite3ValueFromExpr() so the type cannot be NULL */
          Debug.Assert( eType == SQLITE_TEXT || eType == SQLITE_BLOB );

          if ( eType == SQLITE_BLOB )
          {
            byte[] blob = sqlite3_value_blob( pVal );
            z = Encoding.UTF8.GetString( blob, 0, blob.Length );
            pColl = db.pDfltColl;
            Debug.Assert( pColl.enc == SqliteEncoding.UTF8 );
          }
          else
          {
            pColl = sqlite3GetCollSeq( db, SqliteEncoding.UTF8, null, pIdx.azColl[0] );
            if ( pColl == null )
            {
              sqlite3ErrorMsg( pParse, "no such collation sequence: %s",
                  pIdx.azColl );
              return SQLITE_ERROR;
            }
            z = sqlite3ValueText( pVal, pColl.enc );
            //if( null==z ){
            //  return SQLITE_NOMEM;
            //}
            Debug.Assert( z != "" && pColl != null && pColl.xCmp != null );
          }
          n = sqlite3ValueBytes( pVal, pColl.enc );

          for ( i = 0; i < SQLITE_INDEX_SAMPLES; i++ )
          {
            int c;
            int eSampletype = aSample[i].eType;
            if ( eSampletype == SQLITE_NULL || eSampletype < eType )
              continue;
            if ( ( eSampletype != eType ) )
              break;
#if !SQLITE_OMIT_UTF16
																																										if( pColl.enc!=SqliteEncoding.UTF8 ){
int nSample;
string zSample;
zSample = sqlite3Utf8to16(
db, pColl.enc, aSample[i].u.z, aSample[i].nByte, ref nSample
);
zSample = aSample[i].u.z;
nSample = aSample[i].u.z.Length;
//if( null==zSample ){
//  Debug.Assert( db.mallocFailed );
//  return SQLITE_NOMEM;
//}
c = pColl.xCmp(pColl.pUser, nSample, zSample, n, z);
sqlite3DbFree(db, ref zSample);
}else
#endif
																																										            {
              c = pColl.xCmp( pColl.pUser, aSample[i].nByte, aSample[i].u.z, n, z );
            }
            if ( c - roundUp >= 0 )
              break;
          }
        }

        Debug.Assert( i >= 0 && i <= SQLITE_INDEX_SAMPLES );
        piRegion = i;
      }
      return SQLITE_OK;
    }
#endif
		///<summary>
		/// If expression pExpr represents a literal value, set *pp to point to
		/// an sqlite3_value structure containing the same value, with affinity
		/// aff applied to it, before returning. It is the responsibility of the
		/// caller to eventually release this structure by passing it to
		/// sqlite3ValueFree().
		///
		/// If the current parse is a recompile (sqlite3Reprepare()) and pExpr
		/// is an SQL variable that currently has a non-NULL value bound to it,
		/// create an sqlite3_value structure containing this value, again with
		/// affinity aff applied to it, instead.
		///
		/// If neither of the above apply, set *pp to NULL.
		///
		/// If an error occurs, return an error code. Otherwise, SQLITE_OK.
		///</summary>
		#if SQLITE_ENABLE_STAT2
																																										    static int valueFromExpr(
    Parse pParse,
    Expr pExpr,
    char aff,
    ref sqlite3_value pp
    )
    {
      if ( pExpr.op == TK_VARIABLE
      || ( pExpr.op == TK_REGISTER && pExpr.op2 == TK_VARIABLE )
      )
      {
        int iVar = pExpr.iColumn;
        sqlite3VdbeSetVarmask( pParse.pVdbe, iVar ); /* IMP: R-23257-02778 */
        pp = sqlite3VdbeGetValue( pParse.pReprepare, iVar, (u8)aff );
        return SQLITE_OK;
      }
      return sqlite3ValueFromExpr( pParse.db, pExpr, SqliteEncoding.UTF8, aff, ref pp );
    }
#endif
		///<summary>
		/// This function is used to estimate the number of rows that will be visited
		/// by scanning an index for a range of values. The range may have an upper
		/// bound, a lower bound, or both. The WHERE clause terms that set the upper
		/// and lower bounds are represented by pLower and pUpper respectively. For
		/// example, assuming that index p is on t1(a):
		///
		///   ... FROM t1 WHERE a > ? AND a < ? ...
		///                    |_____|   |_____|
		///                       |         |
		///                     pLower    pUpper
		///
		/// If either of the upper or lower bound is not present, then NULL is passed in
		/// place of the corresponding WhereTerm.
		///
		/// The nEq parameter is passed the index of the index column subject to the
		/// range constraint. Or, equivalently, the number of equality constraints
		/// optimized by the proposed index scan. For example, assuming index p is
		/// on t1(a, b), and the SQL query is:
		///
		///   ... FROM t1 WHERE a = ? AND b > ? AND b < ? ...
		///
		/// then nEq should be passed the value 1 (as the range restricted column,
		/// b, is the second left-most column of the index). Or, if the query is:
		///
		///   ... FROM t1 WHERE a > ? AND a < ? ...
		///
		/// then nEq should be passed 0.
		///
		/// The returned value is an integer between 1 and 100, inclusive. A return
		/// value of 1 indicates that the proposed range scan is expected to visit
		/// approximately 1/100th (1%) of the rows selected by the nEq equality
		/// constraints (if any). A return value of 100 indicates that it is expected
		/// that the range scan will visit every row (100%) selected by the equality
		/// constraints.
		///
		/// In the absence of sqlite_stat2 ANALYZE data, each range inequality
		/// reduces the search space by 3/4ths.  Hence a single constraint (x>?)
		/// results in a return of 25 and a range constraint (x>? AND x<?) results
		/// in a return of 6.
		///</summary>
		#if SQLITE_ENABLE_STAT2
																																										    /*
** Estimate the number of rows that will be returned based on
** an equality constraint x=VALUE and where that VALUE occurs in
** the histogram data.  This only works when x is the left-most
** column of an index and sqlite_stat2 histogram data is available
** for that index.  When pExpr==NULL that means the constraint is
** "x IS NULL" instead of "x=VALUE".
**
** Write the estimated row count into *pnRow and return SQLITE_OK. 
** If unable to make an estimate, leave *pnRow unchanged and return
** non-zero.
**
** This routine can fail if it is unable to load a collating sequence
** required for string comparison, or if unable to allocate memory
** for a UTF conversion required for comparison.  The error is stored
** in the pParse structure.
*/
    static int whereEqualScanEst(
      Parse pParse,       /* Parsing & code generating context */
      Index p,            /* The index whose left-most column is pTerm */
      Expr pExpr,         /* Expression for VALUE in the x=VALUE constraint */
      ref double pnRow    /* Write the revised row estimate here */
    )
    {
      sqlite3_value pRhs = null;/* VALUE on right-hand side of pTerm */
      int iLower = 0;
      int iUpper = 0;           /* Range of histogram regions containing pRhs */
      char aff;                 /* Column affinity */
      int rc;                   /* Subfunction return code */
      double nRowEst;           /* New estimate of the number of rows */

      Debug.Assert( p.aSample != null );
      aff = p.pTable.aCol[p.aiColumn[0]].affinity;
      if ( pExpr != null )
      {
        rc = valueFromExpr( pParse, pExpr, aff, ref pRhs );
        if ( rc != 0 )
          goto whereEqualScanEst_cancel;
      }
      else
      {
        pRhs = sqlite3ValueNew( pParse.db );
      }
      if ( pRhs == null )
        return SQLITE_NOTFOUND;
      rc = whereRangeRegion( pParse, p, pRhs, 0, out iLower );
      if ( rc != 0 )
        goto whereEqualScanEst_cancel;
      rc = whereRangeRegion( pParse, p, pRhs, 1, out iUpper );
      if ( rc != 0 )
        goto whereEqualScanEst_cancel;
      WHERETRACE( "equality scan regions: %d..%d\n", iLower, iUpper );
      if ( iLower >= iUpper )
      {
        nRowEst = p.aiRowEst[0] / ( SQLITE_INDEX_SAMPLES * 2 );
        if ( nRowEst < pnRow )
          pnRow = nRowEst;
      }
      else
      {
        nRowEst = ( iUpper - iLower ) * p.aiRowEst[0] / SQLITE_INDEX_SAMPLES;
        pnRow = nRowEst;
      }

whereEqualScanEst_cancel:
      sqlite3ValueFree( ref pRhs );
      return rc;
    }
#endif
		#if SQLITE_ENABLE_STAT2
																																										    /*
** Estimate the number of rows that will be returned based on
** an IN constraint where the right-hand side of the IN operator
** is a list of values.  Example:
**
**        WHERE x IN (1,2,3,4)
**
** Write the estimated row count into *pnRow and return SQLITE_OK. 
** If unable to make an estimate, leave *pnRow unchanged and return
** non-zero.
**
** This routine can fail if it is unable to load a collating sequence
** required for string comparison, or if unable to allocate memory
** for a UTF conversion required for comparison.  The error is stored
** in the pParse structure.
*/
    static int whereInScanEst(
      Parse pParse,       /* Parsing & code generating context */
      Index p,            /* The index whose left-most column is pTerm */
      ExprList pList,     /* The value list on the RHS of "x IN (v1,v2,v3,...)" */
      ref double pnRow    /* Write the revised row estimate here */
    )
    {
      sqlite3_value pVal = null;/* One value from list */
      int iLower = 0;
      int iUpper = 0;           /* Range of histogram regions containing pRhs */
      char aff;                 /* Column affinity */
      int rc = SQLITE_OK;       /* Subfunction return code */
      double nRowEst;           /* New estimate of the number of rows */
      int nSpan = 0;            /* Number of histogram regions spanned */
      int nSingle = 0;          /* Histogram regions hit by a single value */
      int nNotFound = 0;        /* Count of values that are not constants */
      int i;                               /* Loop counter */
      u8[] aSpan = new u8[SQLITE_INDEX_SAMPLES + 1];    /* Histogram regions that are spanned */
      u8[] aSingle = new u8[SQLITE_INDEX_SAMPLES + 1];  /* Histogram regions hit once */

      Debug.Assert( p.aSample != null );
      aff = p.pTable.aCol[p.aiColumn[0]].affinity;
      //memset(aSpan, 0, sizeof(aSpan));
      //memset(aSingle, 0, sizeof(aSingle));
      for ( i = 0; i < pList.nExpr; i++ )
      {
        sqlite3ValueFree( ref pVal );
        rc = valueFromExpr( pParse, pList.a[i].pExpr, aff, ref pVal );
        if ( rc != 0 )
          break;
        if ( pVal == null || sqlite3_value_type( pVal ) == SQLITE_NULL )
        {
          nNotFound++;
          continue;
        }
        rc = whereRangeRegion( pParse, p, pVal, 0, out iLower );
        if ( rc != 0 )
          break;
        rc = whereRangeRegion( pParse, p, pVal, 1, out iUpper );
        if ( rc != 0 )
          break;
        if ( iLower >= iUpper )
        {
          aSingle[iLower] = 1;
        }
        else
        {
          Debug.Assert( iLower >= 0 && iUpper <= SQLITE_INDEX_SAMPLES );
          while ( iLower < iUpper )
            aSpan[iLower++] = 1;
        }
      }
      if ( rc == SQLITE_OK )
      {
        for ( i = nSpan = 0; i <= SQLITE_INDEX_SAMPLES; i++ )
        {
          if ( aSpan[i] != 0 )
          {
            nSpan++;
          }
          else if ( aSingle[i] != 0 )
          {
            nSingle++;
          }
        }
        nRowEst = ( nSpan * 2 + nSingle ) * p.aiRowEst[0] / ( 2 * SQLITE_INDEX_SAMPLES )
                   + nNotFound * p.aiRowEst[1];
        if ( nRowEst > p.aiRowEst[0] )
          nRowEst = p.aiRowEst[0];
        pnRow = nRowEst;
        WHERETRACE( "IN row estimate: nSpan=%d, nSingle=%d, nNotFound=%d, est=%g\n",
                     nSpan, nSingle, nNotFound, nRowEst );
      }
      sqlite3ValueFree( ref pVal );
      return rc;
    }
#endif
		///<summary>
		/// Find the best query plan for accessing a particular table.  Write the
		/// best query plan and its cost into the WhereCost object supplied as the
		/// last parameter.
		///
		/// The lowest cost plan wins.  The cost is an estimate of the amount of
		/// CPU and disk I/O needed to process the requested result.
		/// Factors that influence cost include:
		///
		///    *  The estimated number of rows that will be retrieved.  (The
		///       fewer the better.)
		///
		///    *  Whether or not sorting must occur.
		///
		///    *  Whether or not there must be separate lookups in the
		///       index and in the main table.
		///
		/// If there was an INDEXED BY clause (pSrc->pIndex) attached to the table in
		/// the SQL statement, then this function only considers plans using the
		/// named index. If no such plan is found, then the returned cost is
		/// SQLITE_BIG_DBL. If a plan is found that uses the named index,
		/// then the cost is calculated in the usual way.
		///
		/// If a NOT INDEXED clause (pSrc->notIndexed!=0) was attached to the table
		/// in the SELECT statement, then no indexes are considered. However, the
		/// selected plan may still take advantage of the built-in rowid primary key
		/// index.
		///</summary>
		///<summary>
		/// Find the query plan for accessing table pSrc.pTab. Write the
		/// best query plan and its cost into the WhereCost object supplied
		/// as the last parameter. This function may calculate the cost of
		/// both real and virtual table scans.
		///
		///</summary>
		///<summary>
		/// Disable a term in the WHERE clause.  Except, do not disable the term
		/// if it controls a LEFT OUTER JOIN and it did not originate in the ON
		/// or USING clause of that join.
		///
		/// Consider the term t2.z='ok' in the following queries:
		///
		///   (1)  SELECT * FROM t1 LEFT JOIN t2 ON t1.a=t2.x WHERE t2.z='ok'
		///   (2)  SELECT * FROM t1 LEFT JOIN t2 ON t1.a=t2.x AND t2.z='ok'
		///   (3)  SELECT * FROM t1, t2 WHERE t1.a=t2.x AND t2.z='ok'
		///
		/// The t2.z='ok' is disabled in the in (2) because it originates
		/// in the ON clause.  The term is disabled in (3) because it is not part
		/// of a LEFT OUTER JOIN.  In (1), the term is not disabled.
		///
		/// IMPLEMENTATION-OF: R-24597-58655 No tests are done for terms that are
		/// completely satisfied by indices.
		///
		/// Disabling a term causes that term to not be tested in the inner loop
		/// of the join.  Disabling is an optimization.  When terms are satisfied
		/// by indices, we disable them to prevent redundant tests in the inner
		/// loop.  We would get the correct results if nothing were ever disabled,
		/// but joins might run a little slower.  The trick is to disable as much
		/// as we can without disabling too much.  If we disabled in (1), we'd get
		/// the wrong answer.  See ticket #813.
		///
		///</summary>
		static void disableTerm(WhereLevel pLevel,WhereTerm pTerm) {
			if(pTerm!=null&&(pTerm.wtFlags&TERM_CODED)==0&&(pLevel.iLeftJoin==0||ExprHasProperty(pTerm.pExpr,EP_FromJoin))) {
				pTerm.wtFlags|=TERM_CODED;
				if(pTerm.iParent>=0) {
					WhereTerm pOther=pTerm.pWC.a[pTerm.iParent];
					if((--pOther.nChild)==0) {
						disableTerm(pLevel,pOther);
					}
				}
			}
		}
		///<summary>
		/// Code an OP_Affinity opcode to apply the column affinity string zAff
		/// to the n registers starting at base.
		///
		/// As an optimization, SQLITE_AFF_NONE entries (which are no-ops) at the
		/// beginning and end of zAff are ignored.  If all entries in zAff are
		/// SQLITE_AFF_NONE, then no code gets generated.
		///
		/// This routine makes its own copy of zAff so that the caller is free
		/// to modify zAff after this routine returns.
		///
		///</summary>
		///<summary>
		/// Generate code for a single equality term of the WHERE clause.  An equality
		/// term can be either X=expr or X IN (...).   pTerm is the term to be
		/// coded.
		///
		/// The current value for the constraint is left in register iReg.
		///
		/// For a constraint of the form X=expr, the expression is evaluated and its
		/// result is left on the stack.  For constraints of the form X IN (...)
		/// this routine sets up a loop that will iterate over all values of X.
		///
		///</summary>
		///<summary>
		/// Generate code for a single equality term of the WHERE clause.  An equality
		/// term can be either X=expr or X IN (...).   pTerm is the term to be
		/// coded.
		///
		/// For example, consider table t1(a,b,c,d,e,f) with index i1(a,b,c).
		/// Suppose the WHERE clause is this:  a==5 AND b IN (1,2,3) AND c>5 AND c<10
		/// The index has as many as three equality constraints, but in this
		/// example, the third "c" value is an inequality.  So only two
		/// constraints are coded.  This routine will generate code to evaluate
		/// a==5 and b IN (1,2,3).  The current values for a and b will be stored
		/// in consecutive registers and the index of the first register is returned.
		///
		/// In the example above nEq==2.  But this subroutine works for any value
		/// of nEq including 0.  If nEq==null, this routine is nearly a no-op.
		/// The only thing it does is allocate the pLevel.iMem memory cell and
		/// compute the affinity string.
		///
		/// This routine always allocates at least one memory cell and returns
		/// the index of that memory cell. The code that
		/// calls this routine will use that memory cell to store the termination
		/// key value of the loop.  If one or more IN operators appear, then
		/// this routine allocates an additional nEq memory cells for internal
		/// use.
		///
		/// Before returning, *pzAff is set to point to a buffer containing a
		/// copy of the column affinity string of the index allocated using
		/// sqlite3DbMalloc(). Except, entries in the copy of the string associated
		/// with equality constraints that use NONE affinity are set to
		/// SQLITE_AFF_NONE. This is to deal with SQL such as the following:
		///
		///   CREATE TABLE t1(a TEXT PRIMARY KEY, b);
		///   SELECT ... FROM t1 AS t2, t1 WHERE t1.a = t2.b;
		///
		/// In the example above, the index on t1(a) has TEXT affinity. But since
		/// the right hand side of the equality constraint (t2.b) has NONE affinity,
		/// no conversion should be attempted before using a t2.b value as part of
		/// a key to search the index. Hence the first byte in the returned affinity
		/// string in this example would be set to SQLITE_AFF_NONE.
		///
		///</summary>
		#if !SQLITE_OMIT_EXPLAIN
		///<summary>
		/// This routine is a helper for explainIndexRange() below
		///
		/// pStr holds the text of an expression that we are building up one term
		/// at a time.  This routine adds a new term to the end of the expression.
		/// Terms are separated by AND so add the "AND" text for second and subsequent
		/// terms only.
		///</summary>
		static void explainAppendTerm(StrAccum pStr,/* The text expression being built */int iTerm,/* Index of this term.  First is zero */string zColumn,/* Name of the column */string zOp/* Name of the operator */) {
			if(iTerm!=0)
				sqlite3StrAccumAppend(pStr," AND ",5);
			sqlite3StrAccumAppend(pStr,zColumn,-1);
			sqlite3StrAccumAppend(pStr,zOp,1);
			sqlite3StrAccumAppend(pStr,"?",1);
		}
		///<summary>
		/// Argument pLevel describes a strategy for scanning table pTab. This
		/// function returns a pointer to a string buffer containing a description
		/// of the subset of table rows scanned by the strategy in the form of an
		/// SQL expression. Or, if all rows are scanned, NULL is returned.
		///
		/// For example, if the query:
		///
		///   SELECT * FROM t1 WHERE a=1 AND b>2;
		///
		/// is run and there is an index on (a, b), then this function returns a
		/// string similar to:
		///
		///   "a=? AND b>?"
		///
		/// The returned pointer points to memory obtained from sqlite3DbMalloc().
		/// It is the responsibility of the caller to free the buffer when it is
		/// no longer required.
		///
		///</summary>
		static string explainIndexRange(sqlite3 db,WhereLevel pLevel,Table pTab) {
			WherePlan pPlan=pLevel.plan;
			Index pIndex=pPlan.u.pIdx;
			uint nEq=pPlan.nEq;
			int i,j;
			Column[] aCol=pTab.aCol;
			int[] aiColumn=pIndex.aiColumn;
			StrAccum txt=new StrAccum(100);
			if(nEq==0&&(pPlan.wsFlags&(WHERE_BTM_LIMIT|WHERE_TOP_LIMIT))==0) {
				return null;
			}
			sqlite3StrAccumInit(txt,null,0,SQLITE_MAX_LENGTH);
			txt.db=db;
			sqlite3StrAccumAppend(txt," (",2);
			for(i=0;i<nEq;i++) {
				explainAppendTerm(txt,i,aCol[aiColumn[i]].zName,"=");
			}
			j=i;
			if((pPlan.wsFlags&WHERE_BTM_LIMIT)!=0) {
				explainAppendTerm(txt,i++,aCol[aiColumn[j]].zName,">");
			}
			if((pPlan.wsFlags&WHERE_TOP_LIMIT)!=0) {
				explainAppendTerm(txt,i,aCol[aiColumn[j]].zName,"<");
			}
			sqlite3StrAccumAppend(txt,")",1);
			return sqlite3StrAccumFinish(txt);
		}
		///<summary>
		/// This function is a no-op unless currently processing an EXPLAIN QUERY PLAN
		/// command. If the query being compiled is an EXPLAIN QUERY PLAN, a single
		/// record is added to the output to describe the table scan strategy in
		/// pLevel.
		///
		///</summary>
		#else
																																										// define explainOneScan(u,v,w,x,y,z)
static void explainOneScan(  Parse u,  SrcList v,  WhereLevel w,  int x,  int y,  u16 z){}
#endif
		///<summary>
		/// Generate code for the start of the iLevel-th loop in the WHERE clause
		/// implementation described by pWInfo.
		///</summary>
		static Bitmask codeOneLoopStart(WhereInfo pWInfo,/* Complete information about the WHERE clause */int iLevel,/* Which level of pWInfo.a[] should be coded */u16 wctrlFlags,/* One of the WHERE_* flags defined in sqliteInt.h */Bitmask notReady/* Which tables are currently available */) {
			int j,k;
			/* Loop counters */int iCur;
			/* The VDBE cursor for the table */int addrNxt;
			/* Where to jump to continue with the next IN case */int omitTable;
			/* True if we use the index only */int bRev;
			/* True if we need to scan in reverse order */WhereLevel pLevel;
			/* The where level to be coded */WhereClause pWC;
			/* Decomposition of the entire WHERE clause */WhereTerm pTerm;
			/* A WHERE clause term */Parse pParse;
			/* Parsing context */Vdbe v;
			/* The prepared stmt under constructions */SrcList_item pTabItem;
			/* FROM clause term being coded */int addrBrk;
			/* Jump here to break out of the loop */int addrCont;
			/* Jump here to continue with next cycle */int iRowidReg=0;
			/* Rowid is stored in this register, if not zero */int iReleaseReg=0;
			/* Temp register to free before returning */pParse=pWInfo.pParse;
			v=pParse.pVdbe;
			pWC=pWInfo.pWC;
			pLevel=pWInfo.a[iLevel];
			pTabItem=pWInfo.pTabList.a[pLevel.iFrom];
			iCur=pTabItem.iCursor;
			bRev=(pLevel.plan.wsFlags&WHERE_REVERSE)!=0?1:0;
			omitTable=((pLevel.plan.wsFlags&WHERE_IDX_ONLY)!=0&&(wctrlFlags&WHERE_FORCE_TABLE)==0)?1:0;
			/* Create labels for the "break" and "continue" instructions
      ** for the current loop.  Jump to addrBrk to break out of a loop.
      ** Jump to cont to go immediately to the next iteration of the
      ** loop.
      **
      ** When there is an IN operator, we also have a "addrNxt" label that
      ** means to continue with the next IN value combination.  When
      ** there are no IN operators in the constraints, the "addrNxt" label
      ** is the same as "addrBrk".
      */addrBrk=pLevel.addrBrk=pLevel.addrNxt=v.sqlite3VdbeMakeLabel();
			addrCont=pLevel.addrCont=v.sqlite3VdbeMakeLabel();
			/* If this is the right table of a LEFT OUTER JOIN, allocate and
      ** initialize a memory cell that records if this table matches any
      ** row of the left table of the join.
      */if(pLevel.iFrom>0&&(pTabItem.jointype&JT_LEFT)!=0)// Check value of pTabItem[0].jointype
			 {
				pLevel.iLeftJoin=++pParse.nMem;
				v.sqlite3VdbeAddOp2(OP_Integer,0,pLevel.iLeftJoin);
				#if SQLITE_DEBUG
																																																																																				        VdbeComment( v, "init LEFT JOIN no-match flag" );
#endif
			}
			#if !SQLITE_OMIT_VIRTUALTABLE
			if((pLevel.plan.wsFlags&WHERE_VIRTUALTABLE)!=0) {
				/* Case 0:  The table is a virtual-table.  Use the VFilter and VNext
        **          to access the data.
        */int iReg;
				/* P3 Value for OP_VFilter */sqlite3_index_info pVtabIdx=pLevel.plan.u.pVtabIdx;
				int nConstraint=pVtabIdx.nConstraint;
				sqlite3_index_constraint_usage[] aUsage=pVtabIdx.aConstraintUsage;
				sqlite3_index_constraint[] aConstraint=pVtabIdx.aConstraint;
				pParse.sqlite3ExprCachePush();
				iReg=pParse.sqlite3GetTempRange(nConstraint+2);
				for(j=1;j<=nConstraint;j++) {
					for(k=0;k<nConstraint;k++) {
						if(aUsage[k].argvIndex==j) {
							int iTerm=aConstraint[k].iTermOffset;
							pParse.sqlite3ExprCode(pWC.a[iTerm].pExpr.pRight,iReg+j+1);
							break;
						}
					}
					if(k==nConstraint)
						break;
				}
				v.sqlite3VdbeAddOp2(OP_Integer,pVtabIdx.idxNum,iReg);
				v.sqlite3VdbeAddOp2(OP_Integer,j-1,iReg+1);
				v.sqlite3VdbeAddOp4(OP_VFilter,iCur,addrBrk,iReg,pVtabIdx.idxStr,pVtabIdx.needToFreeIdxStr!=0?P4_MPRINTF:P4_STATIC);
				pVtabIdx.needToFreeIdxStr=0;
				for(j=0;j<nConstraint;j++) {
					if(aUsage[j].omit!=false) {
						int iTerm=aConstraint[j].iTermOffset;
						disableTerm(pLevel,pWC.a[iTerm]);
					}
				}
				pLevel.op=OP_VNext;
				pLevel.p1=iCur;
				pLevel.p2=v.sqlite3VdbeCurrentAddr();
				pParse.sqlite3ReleaseTempRange(iReg,nConstraint+2);
				pParse.sqlite3ExprCachePop(1);
			}
			else
				#endif
				if((pLevel.plan.wsFlags&WHERE_ROWID_EQ)!=0) {
					/* Case 1:  We can directly reference a single row using an
          **          equality comparison against the ROWID field.  Or
          **          we reference multiple rows using a "rowid IN (...)"
          **          construct.
          */iReleaseReg=pParse.sqlite3GetTempReg();
					pTerm=pWC.findTerm(iCur,-1,notReady,WO_EQ|WO_IN,null);
					Debug.Assert(pTerm!=null);
					Debug.Assert(pTerm.pExpr!=null);
					Debug.Assert(pTerm.leftCursor==iCur);
					Debug.Assert(omitTable==0);
					testcase(pTerm.wtFlags&TERM_VIRTUAL);
					/* EV: R-30575-11662 */iRowidReg=pParse.codeEqualityTerm(pTerm,pLevel,iReleaseReg);
					addrNxt=pLevel.addrNxt;
					v.sqlite3VdbeAddOp2(OP_MustBeInt,iRowidReg,addrNxt);
					v.sqlite3VdbeAddOp3(OP_NotExists,iCur,addrNxt,iRowidReg);
					pParse.sqlite3ExprCacheStore(iCur,-1,iRowidReg);
					#if SQLITE_DEBUG
																																																																																																									          VdbeComment( v, "pk" );
#endif
					pLevel.op=OP_Noop;
				}
				else
					if((pLevel.plan.wsFlags&WHERE_ROWID_RANGE)!=0) {
						/* Case 2:  We have an inequality comparison against the ROWID field.
          */int testOp=OP_Noop;
						int start;
						int memEndValue=0;
						WhereTerm pStart,pEnd;
						Debug.Assert(omitTable==0);
						pStart=pWC.findTerm(iCur,-1,notReady,WO_GT|WO_GE,null);
						pEnd=pWC.findTerm(iCur,-1,notReady,WO_LT|WO_LE,null);
						if(bRev!=0) {
							pTerm=pStart;
							pStart=pEnd;
							pEnd=pTerm;
						}
						if(pStart!=null) {
							Expr pX;
							/* The expression that defines the start bound */int r1,rTemp=0;
							/* Registers for holding the start boundary *//* The following constant maps TK_xx codes into corresponding
            ** seek opcodes.  It depends on a particular ordering of TK_xx
            */u8[] aMoveOp=new u8[] {
								/* TK_GT */OP_SeekGt,
								/* TK_LE */OP_SeekLe,
								/* TK_LT */OP_SeekLt,
								/* TK_GE */OP_SeekGe
							};
							Debug.Assert(TK_LE==TK_GT+1);
							/* Make sure the ordering.. */Debug.Assert(TK_LT==TK_GT+2);
							/*  ... of the TK_xx values... */Debug.Assert(TK_GE==TK_GT+3);
							/*  ... is correcct. */testcase(pStart.wtFlags&TERM_VIRTUAL);
							/* EV: R-30575-11662 */pX=pStart.pExpr;
							Debug.Assert(pX!=null);
							Debug.Assert(pStart.leftCursor==iCur);
							r1=pParse.sqlite3ExprCodeTemp(pX.pRight,ref rTemp);
							v.sqlite3VdbeAddOp3(aMoveOp[pX.op-TK_GT],iCur,addrBrk,r1);
							#if SQLITE_DEBUG
																																																																																																																																																			            VdbeComment( v, "pk" );
#endif
							pParse.sqlite3ExprCacheAffinityChange(r1,1);
							pParse.sqlite3ReleaseTempReg(rTemp);
							disableTerm(pLevel,pStart);
						}
						else {
							v.sqlite3VdbeAddOp2(bRev!=0?OP_Last:OP_Rewind,iCur,addrBrk);
						}
						if(pEnd!=null) {
							Expr pX;
							pX=pEnd.pExpr;
							Debug.Assert(pX!=null);
							Debug.Assert(pEnd.leftCursor==iCur);
							testcase(pEnd.wtFlags&TERM_VIRTUAL);
							/* EV: R-30575-11662 */memEndValue=++pParse.nMem;
							pParse.sqlite3ExprCode(pX.pRight,memEndValue);
							if(pX.op==TK_LT||pX.op==TK_GT) {
								testOp=bRev!=0?OP_Le:OP_Ge;
							}
							else {
								testOp=bRev!=0?OP_Lt:OP_Gt;
							}
							disableTerm(pLevel,pEnd);
						}
						start=v.sqlite3VdbeCurrentAddr();
						pLevel.op=(u8)(bRev!=0?OP_Prev:OP_Next);
						pLevel.p1=iCur;
						pLevel.p2=start;
						if(pStart==null&&pEnd==null) {
							pLevel.p5=SQLITE_STMTSTATUS_FULLSCAN_STEP;
						}
						else {
							Debug.Assert(pLevel.p5==0);
						}
						if(testOp!=OP_Noop) {
							iRowidReg=iReleaseReg=pParse.sqlite3GetTempReg();
							v.sqlite3VdbeAddOp2(OP_Rowid,iCur,iRowidReg);
							pParse.sqlite3ExprCacheStore(iCur,-1,iRowidReg);
							v.sqlite3VdbeAddOp3(testOp,memEndValue,addrBrk,iRowidReg);
							v.sqlite3VdbeChangeP5(SQLITE_AFF_NUMERIC|SQLITE_JUMPIFNULL);
						}
					}
					else
						if((pLevel.plan.wsFlags&(WHERE_COLUMN_RANGE|WHERE_COLUMN_EQ))!=0) {
							/* Case 3: A scan using an index.
          **
          **         The WHERE clause may contain zero or more equality
          **         terms ("==" or "IN" operators) that refer to the N
          **         left-most columns of the index. It may also contain
          **         inequality constraints (>, <, >= or <=) on the indexed
          **         column that immediately follows the N equalities. Only
          **         the right-most column can be an inequality - the rest must
          **         use the "==" and "IN" operators. For example, if the
          **         index is on (x,y,z), then the following clauses are all
          **         optimized:
          **
          **            x=5
          **            x=5 AND y=10
          **            x=5 AND y<10
          **            x=5 AND y>5 AND y<10
          **            x=5 AND y=5 AND z<=10
          **
          **         The z<10 term of the following cannot be used, only
          **         the x=5 term:
          **
          **            x=5 AND z<10
          **
          **         N may be zero if there are inequality constraints.
          **         If there are no inequality constraints, then N is at
          **         least one.
          **
          **         This case is also used when there are no WHERE clause
          **         constraints but an index is selected anyway, in order
          **         to force the output order to conform to an ORDER BY.
          */u8[] aStartOp=new u8[] {
								0,
								0,
								OP_Rewind,
								/* 2: (!start_constraints && startEq &&  !bRev) */OP_Last,
								/* 3: (!start_constraints && startEq &&   bRev) */OP_SeekGt,
								/* 4: (start_constraints  && !startEq && !bRev) */OP_SeekLt,
								/* 5: (start_constraints  && !startEq &&  bRev) */OP_SeekGe,
								/* 6: (start_constraints  &&  startEq && !bRev) */OP_SeekLe
							/* 7: (start_constraints  &&  startEq &&  bRev) */};
							u8[] aEndOp=new u8[] {
								OP_Noop,
								/* 0: (!end_constraints) */OP_IdxGE,
								/* 1: (end_constraints && !bRev) */OP_IdxLT
							/* 2: (end_constraints && bRev) */};
							int nEq=(int)pLevel.plan.nEq;
							/* Number of == or IN terms */int isMinQuery=0;
							/* If this is an optimized SELECT min(x).. */int regBase;
							/* Base register holding constraint values */int r1;
							/* Temp register */WhereTerm pRangeStart=null;
							/* Inequality constraint at range start */WhereTerm pRangeEnd=null;
							/* Inequality constraint at range end */int startEq;
							/* True if range start uses ==, >= or <= */int endEq;
							/* True if range end uses ==, >= or <= */int start_constraints;
							/* Start of range is constrained */int nConstraint;
							/* Number of constraint terms */Index pIdx;
							/* The index we will be using */int iIdxCur;
							/* The VDBE cursor for the index */int nExtraReg=0;
							/* Number of extra registers needed */int op;
							/* Instruction opcode */StringBuilder zStartAff=new StringBuilder("");
							;
							/* Affinity for start of range constraint */StringBuilder zEndAff;
							/* Affinity for end of range constraint */pIdx=pLevel.plan.u.pIdx;
							iIdxCur=pLevel.iIdxCur;
							k=pIdx.aiColumn[nEq];
							/* Column for inequality constraints *//* If this loop satisfies a sort order (pOrderBy) request that
          ** was pDebug.Assed to this function to implement a "SELECT min(x) ..."
          ** query, then the caller will only allow the loop to run for
          ** a single iteration. This means that the first row returned
          ** should not have a NULL value stored in 'x'. If column 'x' is
          ** the first one after the nEq equality constraints in the index,
          ** this requires some special handling.
          */if((wctrlFlags&WHERE_ORDERBY_MIN)!=0&&((pLevel.plan.wsFlags&WHERE_ORDERBY)!=0)&&(pIdx.nColumn>nEq)) {
								/* Debug.Assert( pOrderBy.nExpr==1 ); *//* Debug.Assert( pOrderBy.a[0].pExpr.iColumn==pIdx.aiColumn[nEq] ); */isMinQuery=1;
								nExtraReg=1;
							}
							/* Find any inequality constraint terms for the start and end
          ** of the range.
          */if((pLevel.plan.wsFlags&WHERE_TOP_LIMIT)!=0) {
								pRangeEnd=pWC.findTerm(iCur,k,notReady,(WO_LT|WO_LE),pIdx);
								nExtraReg=1;
							}
							if((pLevel.plan.wsFlags&WHERE_BTM_LIMIT)!=0) {
								pRangeStart=pWC.findTerm(iCur,k,notReady,(WO_GT|WO_GE),pIdx);
								nExtraReg=1;
							}
							/* Generate code to evaluate all constraint terms using == or IN
          ** and store the values of those terms in an array of registers
          ** starting at regBase.
          */regBase=pParse.codeAllEqualityTerms(pLevel,pWC,notReady,nExtraReg,out zStartAff);
							zEndAff=new StringBuilder(zStartAff.ToString());
							//sqlite3DbStrDup(pParse.db, zStartAff);
							addrNxt=pLevel.addrNxt;
							/* If we are doing a reverse order scan on an ascending index, or
          ** a forward order scan on a descending index, interchange the
          ** start and end terms (pRangeStart and pRangeEnd).
          */if(nEq<pIdx.nColumn&&bRev==(pIdx.aSortOrder[nEq]==SQLITE_SO_ASC?1:0)) {
								SWAP(ref pRangeEnd,ref pRangeStart);
							}
							testcase(pRangeStart!=null&&(pRangeStart.eOperator&WO_LE)!=0);
							testcase(pRangeStart!=null&&(pRangeStart.eOperator&WO_GE)!=0);
							testcase(pRangeEnd!=null&&(pRangeEnd.eOperator&WO_LE)!=0);
							testcase(pRangeEnd!=null&&(pRangeEnd.eOperator&WO_GE)!=0);
							startEq=(null==pRangeStart||(pRangeStart.eOperator&(WO_LE|WO_GE))!=0)?1:0;
							endEq=(null==pRangeEnd||(pRangeEnd.eOperator&(WO_LE|WO_GE))!=0)?1:0;
							start_constraints=(pRangeStart!=null||nEq>0)?1:0;
							/* Seek the index cursor to the start of the range. */nConstraint=nEq;
							if(pRangeStart!=null) {
								Expr pRight=pRangeStart.pExpr.pRight;
								pParse.sqlite3ExprCode(pRight,regBase+nEq);
								if((pRangeStart.wtFlags&TERM_VNULL)==0) {
									sqlite3ExprCodeIsNullJump(v,pRight,regBase+nEq,addrNxt);
								}
								if(zStartAff.Length!=0) {
									if(pRight.sqlite3CompareAffinity(zStartAff[nEq])==SQLITE_AFF_NONE) {
										/* Since the comparison is to be performed with no conversions
                ** applied to the operands, set the affinity to apply to pRight to 
                ** SQLITE_AFF_NONE.  */zStartAff[nEq]=SQLITE_AFF_NONE;
									}
									if((sqlite3ExprNeedsNoAffinityChange(pRight,zStartAff[nEq]))!=0) {
										zStartAff[nEq]=SQLITE_AFF_NONE;
									}
								}
								nConstraint++;
								testcase(pRangeStart.wtFlags&TERM_VIRTUAL);
								/* EV: R-30575-11662 */}
							else
								if(isMinQuery!=0) {
									v.sqlite3VdbeAddOp2(OP_Null,0,regBase+nEq);
									nConstraint++;
									startEq=0;
									start_constraints=1;
								}
							pParse.codeApplyAffinity(regBase,nConstraint,zStartAff.ToString());
							op=aStartOp[(start_constraints<<2)+(startEq<<1)+bRev];
							Debug.Assert(op!=0);
							testcase(op==OP_Rewind);
							testcase(op==OP_Last);
							testcase(op==OP_SeekGt);
							testcase(op==OP_SeekGe);
							testcase(op==OP_SeekLe);
							testcase(op==OP_SeekLt);
							v.sqlite3VdbeAddOp4Int(op,iIdxCur,addrNxt,regBase,nConstraint);
							/* Load the value for the inequality constraint at the end of the
          ** range (if any).
          */nConstraint=nEq;
							if(pRangeEnd!=null) {
								Expr pRight=pRangeEnd.pExpr.pRight;
								pParse.sqlite3ExprCacheRemove(regBase+nEq,1);
								pParse.sqlite3ExprCode(pRight,regBase+nEq);
								if((pRangeEnd.wtFlags&TERM_VNULL)==0) {
									sqlite3ExprCodeIsNullJump(v,pRight,regBase+nEq,addrNxt);
								}
								if(zEndAff.Length>0) {
									if(pRight.sqlite3CompareAffinity(zEndAff[nEq])==SQLITE_AFF_NONE) {
										/* Since the comparison is to be performed with no conversions
                ** applied to the operands, set the affinity to apply to pRight to 
                ** SQLITE_AFF_NONE.  */zEndAff[nEq]=SQLITE_AFF_NONE;
									}
									if((sqlite3ExprNeedsNoAffinityChange(pRight,zEndAff[nEq]))!=0) {
										zEndAff[nEq]=SQLITE_AFF_NONE;
									}
								}
								pParse.codeApplyAffinity(regBase,nEq+1,zEndAff.ToString());
								nConstraint++;
								testcase(pRangeEnd.wtFlags&TERM_VIRTUAL);
								/* EV: R-30575-11662 */}
							sqlite3DbFree(pParse.db,ref zStartAff);
							sqlite3DbFree(pParse.db,ref zEndAff);
							/* Top of the loop body */pLevel.p2=v.sqlite3VdbeCurrentAddr();
							/* Check if the index cursor is past the end of the range. */op=aEndOp[((pRangeEnd!=null||nEq!=0)?1:0)*(1+bRev)];
							testcase(op==OP_Noop);
							testcase(op==OP_IdxGE);
							testcase(op==OP_IdxLT);
							if(op!=OP_Noop) {
								v.sqlite3VdbeAddOp4Int(op,iIdxCur,addrNxt,regBase,nConstraint);
								v.sqlite3VdbeChangeP5((u8)(endEq!=bRev?1:0));
							}
							/* If there are inequality constraints, check that the value
          ** of the table column that the inequality contrains is not NULL.
          ** If it is, jump to the next iteration of the loop.
          */r1=pParse.sqlite3GetTempReg();
							testcase(pLevel.plan.wsFlags&WHERE_BTM_LIMIT);
							testcase(pLevel.plan.wsFlags&WHERE_TOP_LIMIT);
							if((pLevel.plan.wsFlags&(WHERE_BTM_LIMIT|WHERE_TOP_LIMIT))!=0) {
								v.sqlite3VdbeAddOp3(OP_Column,iIdxCur,nEq,r1);
								v.sqlite3VdbeAddOp2(OP_IsNull,r1,addrCont);
							}
							pParse.sqlite3ReleaseTempReg(r1);
							/* Seek the table cursor, if required */disableTerm(pLevel,pRangeStart);
							disableTerm(pLevel,pRangeEnd);
							if(0==omitTable) {
								iRowidReg=iReleaseReg=pParse.sqlite3GetTempReg();
								v.sqlite3VdbeAddOp2(OP_IdxRowid,iIdxCur,iRowidReg);
								pParse.sqlite3ExprCacheStore(iCur,-1,iRowidReg);
								v.sqlite3VdbeAddOp2(OP_Seek,iCur,iRowidReg);
								/* Deferred seek */}
							/* Record the instruction used to terminate the loop. Disable
          ** WHERE clause terms made redundant by the index range scan.
          */if((pLevel.plan.wsFlags&WHERE_UNIQUE)!=0) {
								pLevel.op=OP_Noop;
							}
							else
								if(bRev!=0) {
									pLevel.op=OP_Prev;
								}
								else {
									pLevel.op=OP_Next;
								}
							pLevel.p1=iIdxCur;
						}
						else
							#if !SQLITE_OMIT_OR_OPTIMIZATION
							if((pLevel.plan.wsFlags&WHERE_MULTI_OR)!=0) {
								/* Case 4:  Two or more separately indexed terms connected by OR
            **
            ** Example:
            **
            **   CREATE TABLE t1(a,b,c,d);
            **   CREATE INDEX i1 ON t1(a);
            **   CREATE INDEX i2 ON t1(b);
            **   CREATE INDEX i3 ON t1(c);
            **
            **   SELECT * FROM t1 WHERE a=5 OR b=7 OR (c=11 AND d=13)
            **
            ** In the example, there are three indexed terms connected by OR.
            ** The top of the loop looks like this:
            **
            **          Null       1                # Zero the rowset in reg 1
            **
            ** Then, for each indexed term, the following. The arguments to
            ** RowSetTest are such that the rowid of the current row is inserted
            ** into the RowSet. If it is already present, control skips the
            ** Gosub opcode and jumps straight to the code generated by WhereEnd().
            **
            **        sqlite3WhereBegin(<term>)
            **          RowSetTest                  # Insert rowid into rowset
            **          Gosub      2 A
            **        sqlite3WhereEnd()
            **
            ** Following the above, code to terminate the loop. Label A, the target
            ** of the Gosub above, jumps to the instruction right after the Goto.
            **
            **          Null       1                # Zero the rowset in reg 1
            **          Goto       B                # The loop is finished.
            **
            **       A: <loop body>                 # Return data, whatever.
            **
            **          Return     2                # Jump back to the Gosub
            **
            **       B: <after the loop>
            **
            */WhereClause pOrWc;
								/* The OR-clause broken out into subterms */SrcList pOrTab;
								/* Shortened table list or OR-clause generation */int regReturn=++pParse.nMem;
								/* Register used with OP_Gosub */int regRowset=0;
								/* Register for RowSet object */int regRowid=0;
								/* Register holding rowid */int iLoopBody=v.sqlite3VdbeMakeLabel();
								/* Start of loop body */int iRetInit;
								/* Address of regReturn init */int untestedTerms=0;
								/* Some terms not completely tested */int ii;
								pTerm=pLevel.plan.u.pTerm;
								Debug.Assert(pTerm!=null);
								Debug.Assert(pTerm.eOperator==WO_OR);
								Debug.Assert((pTerm.wtFlags&TERM_ORINFO)!=0);
								pOrWc=pTerm.u.pOrInfo.wc;
								pLevel.op=OP_Return;
								pLevel.p1=regReturn;
								/* Set up a new SrcList in pOrTab containing the table being scanned
            ** by this loop in the a[0] slot and all notReady tables in a[1..] slots.
            ** This becomes the SrcList in the recursive call to sqlite3WhereBegin().
            */if(pWInfo.nLevel>1) {
									int nNotReady;
									/* The number of notReady tables */SrcList_item[] origSrc;
									/* Original list of tables */nNotReady=pWInfo.nLevel-iLevel-1;
									//sqlite3StackAllocRaw(pParse.db,
									//sizeof(*pOrTab)+ nNotReady*sizeof(pOrTab.a[0]));
									pOrTab=new SrcList();
									pOrTab.a=new SrcList_item[nNotReady+1];
									//if( pOrTab==0 ) return notReady;
									pOrTab.nAlloc=(i16)(nNotReady+1);
									pOrTab.nSrc=pOrTab.nAlloc;
									pOrTab.a[0]=pTabItem;
									//memcpy(pOrTab.a, pTabItem, sizeof(*pTabItem));
									origSrc=pWInfo.pTabList.a;
									for(k=1;k<=nNotReady;k++) {
										pOrTab.a[k]=origSrc[pWInfo.a[iLevel+k].iFrom];
										// memcpy(&pOrTab.a[k], &origSrc[pLevel[k].iFrom], sizeof(pOrTab.a[k]));
									}
								}
								else {
									pOrTab=pWInfo.pTabList;
								}
								/* Initialize the rowset register to contain NULL. An SQL NULL is
            ** equivalent to an empty rowset.
            **
            ** Also initialize regReturn to contain the address of the instruction
            ** immediately following the OP_Return at the bottom of the loop. This
            ** is required in a few obscure LEFT JOIN cases where control jumps
            ** over the top of the loop into the body of it. In this case the
            ** correct response for the end-of-loop code (the OP_Return) is to
            ** fall through to the next instruction, just as an OP_Next does if
            ** called on an uninitialized cursor.
            */if((wctrlFlags&WHERE_DUPLICATES_OK)==0) {
									regRowset=++pParse.nMem;
									regRowid=++pParse.nMem;
									v.sqlite3VdbeAddOp2(OP_Null,0,regRowset);
								}
								iRetInit=v.sqlite3VdbeAddOp2(OP_Integer,0,regReturn);
								for(ii=0;ii<pOrWc.nTerm;ii++) {
									WhereTerm pOrTerm=pOrWc.a[ii];
									if(pOrTerm.leftCursor==iCur||pOrTerm.eOperator==WO_AND) {
										WhereInfo pSubWInfo;
										/* Info for single OR-term scan *//* Loop through table entries that match term pOrTerm. */ExprList elDummy=null;
										pSubWInfo=pParse.sqlite3WhereBegin(pOrTab,pOrTerm.pExpr,ref elDummy,WHERE_OMIT_OPEN|WHERE_OMIT_CLOSE|WHERE_FORCE_TABLE|WHERE_ONETABLE_ONLY);
										if(pSubWInfo!=null) {
											pParse.explainOneScan(pOrTab,pSubWInfo.a[0],iLevel,pLevel.iFrom,0);
											if((wctrlFlags&WHERE_DUPLICATES_OK)==0) {
												int iSet=((ii==pOrWc.nTerm-1)?-1:ii);
												int r;
												r=pParse.sqlite3ExprCodeGetColumn(pTabItem.pTab,-1,iCur,regRowid);
												v.sqlite3VdbeAddOp4Int(OP_RowSetTest,regRowset,v.sqlite3VdbeCurrentAddr()+2,r,iSet);
											}
											v.sqlite3VdbeAddOp2(OP_Gosub,regReturn,iLoopBody);
											/* The pSubWInfo.untestedTerms flag means that this OR term
                  ** contained one or more AND term from a notReady table.  The
                  ** terms from the notReady table could not be tested and will
                  ** need to be tested later.
                  */if(pSubWInfo.untestedTerms!=0)
												untestedTerms=1;
											/* Finish the loop through table entries that match term pOrTerm. */sqlite3WhereEnd(pSubWInfo);
										}
									}
								}
								v.sqlite3VdbeChangeP1(iRetInit,v.sqlite3VdbeCurrentAddr());
								v.sqlite3VdbeAddOp2(OP_Goto,0,pLevel.addrBrk);
								v.sqlite3VdbeResolveLabel(iLoopBody);
								if(pWInfo.nLevel>1)
									sqlite3DbFree(pParse.db,ref pOrTab);
								//sqlite3DbFree(pParse.db, pOrTab)
								if(0==untestedTerms)
									disableTerm(pLevel,pTerm);
							}
							else
							#endif
							 {
								/* Case 5:  There is no usable index.  We must do a complete
            **          scan of the entire table.
            */u8[] aStep=new u8[] {
									OP_Next,
									OP_Prev
								};
								u8[] aStart=new u8[] {
									OP_Rewind,
									OP_Last
								};
								Debug.Assert(bRev==0||bRev==1);
								Debug.Assert(omitTable==0);
								pLevel.op=aStep[bRev];
								pLevel.p1=iCur;
								pLevel.p2=1+v.sqlite3VdbeAddOp2(aStart[bRev],iCur,addrBrk);
								pLevel.p5=SQLITE_STMTSTATUS_FULLSCAN_STEP;
							}
			notReady&=~getMask(pWC.pMaskSet,iCur);
			/* Insert code to test every subexpression that can be completely
      ** computed using the current set of tables.
      **
      ** IMPLEMENTATION-OF: R-49525-50935 Terms that cannot be satisfied through
      ** the use of indices become tests that are evaluated against each row of
      ** the relevant input tables.
      */for(j=pWC.nTerm;j>0;j--)//, pTerm++)
			 {
				pTerm=pWC.a[pWC.nTerm-j];
				Expr pE;
				testcase(pTerm.wtFlags&TERM_VIRTUAL);
				/* IMP: R-30575-11662 */testcase(pTerm.wtFlags&TERM_CODED);
				if((pTerm.wtFlags&(TERM_VIRTUAL|TERM_CODED))!=0)
					continue;
				if((pTerm.prereqAll&notReady)!=0) {
					testcase(pWInfo.untestedTerms==0&&(pWInfo.wctrlFlags&WHERE_ONETABLE_ONLY)!=0);
					pWInfo.untestedTerms=1;
					continue;
				}
				pE=pTerm.pExpr;
				Debug.Assert(pE!=null);
				if(pLevel.iLeftJoin!=0&&!((pE.flags&EP_FromJoin)==EP_FromJoin))// !ExprHasProperty(pE, EP_FromJoin) ){
				 {
					continue;
				}
				pParse.sqlite3ExprIfFalse(pE,addrCont,SQLITE_JUMPIFNULL);
				pTerm.wtFlags|=TERM_CODED;
			}
			/* For a LEFT OUTER JOIN, generate code that will record the fact that
      ** at least one row of the right table has matched the left table.
      */if(pLevel.iLeftJoin!=0) {
				pLevel.addrFirst=v.sqlite3VdbeCurrentAddr();
				v.sqlite3VdbeAddOp2(OP_Integer,1,pLevel.iLeftJoin);
				#if SQLITE_DEBUG
																																																																																				        VdbeComment( v, "record LEFT JOIN hit" );
#endif
				pParse.sqlite3ExprCacheClear();
				for(j=0;j<pWC.nTerm;j++)//, pTerm++)
				 {
					pTerm=pWC.a[j];
					testcase(pTerm.wtFlags&TERM_VIRTUAL);
					/* IMP: R-30575-11662 */testcase(pTerm.wtFlags&TERM_CODED);
					if((pTerm.wtFlags&(TERM_VIRTUAL|TERM_CODED))!=0)
						continue;
					if((pTerm.prereqAll&notReady)!=0) {
						Debug.Assert(pWInfo.untestedTerms!=0);
						continue;
					}
					Debug.Assert(pTerm.pExpr!=null);
					pParse.sqlite3ExprIfFalse(pTerm.pExpr,addrCont,SQLITE_JUMPIFNULL);
					pTerm.wtFlags|=TERM_CODED;
				}
			}
			pParse.sqlite3ReleaseTempReg(iReleaseReg);
			return notReady;
		}
		#if (SQLITE_TEST)
																																										    /*
** The following variable holds a text description of query plan generated
** by the most recent call to sqlite3WhereBegin().  Each call to WhereBegin
** overwrites the previous.  This information is used for testing and
** analysis only.
*/
#if !TCLSH
																																										    //char sqlite3_query_plan[BMS*2*40];  /* Text of the join */
    static StringBuilder sqlite3_query_plan;
#else
																																										    static tcl.lang.Var.SQLITE3_GETSET sqlite3_query_plan = new tcl.lang.Var.SQLITE3_GETSET( "sqlite3_query_plan" );
#endif
																																										    static int nQPlan = 0;              /* Next free slow in _query_plan[] */

#endif
		///<summary>
		/// Free a WhereInfo structure
		///</summary>
		static void whereInfoFree(sqlite3 db,WhereInfo pWInfo) {
			if(ALWAYS(pWInfo!=null)) {
				int i;
				for(i=0;i<pWInfo.nLevel;i++) {
					sqlite3_index_info pInfo=pWInfo.a[i]!=null?pWInfo.a[i].pIdxInfo:null;
					if(pInfo!=null) {
						/* Debug.Assert( pInfo.needToFreeIdxStr==0 || db.mallocFailed ); */if(pInfo.needToFreeIdxStr!=0) {
							//sqlite3_free( ref pInfo.idxStr );
						}
						sqlite3DbFree(db,ref pInfo);
					}
					if(pWInfo.a[i]!=null&&(pWInfo.a[i].plan.wsFlags&WHERE_TEMP_INDEX)!=0) {
						Index pIdx=pWInfo.a[i].plan.u.pIdx;
						if(pIdx!=null) {
							sqlite3DbFree(db,ref pIdx.zColAff);
							sqlite3DbFree(db,ref pIdx);
						}
					}
				}
				pWInfo.pWC.whereClauseClear();
				sqlite3DbFree(db,ref pWInfo);
			}
		}
		///<summary>
		/// Generate the beginning of the loop used for WHERE clause processing.
		/// The return value is a pointer to an opaque structure that contains
		/// information needed to terminate the loop.  Later, the calling routine
		/// should invoke sqlite3WhereEnd() with the return value of this function
		/// in order to complete the WHERE clause processing.
		///
		/// If an error occurs, this routine returns NULL.
		///
		/// The basic idea is to do a nested loop, one loop for each table in
		/// the FROM clause of a select.  (INSERT and UPDATE statements are the
		/// same as a SELECT with only a single table in the FROM clause.)  For
		/// example, if the SQL is this:
		///
		///       SELECT * FROM t1, t2, t3 WHERE ...;
		///
		/// Then the code generated is conceptually like the following:
		///
		///      foreach row1 in t1 do       \    Code generated
		///        foreach row2 in t2 do      |-- by sqlite3WhereBegin()
		///          foreach row3 in t3 do   /
		///            ...
		///          end                     \    Code generated
		///        end                        |-- by sqlite3WhereEnd()
		///      end                         /
		///
		/// Note that the loops might not be nested in the order in which they
		/// appear in the FROM clause if a different order is better able to make
		/// use of indices.  Note also that when the IN operator appears in
		/// the WHERE clause, it might result in additional nested loops for
		/// scanning through all values on the right-hand side of the IN.
		///
		/// There are Btree cursors Debug.Associated with each table.  t1 uses cursor
		/// number pTabList.a[0].iCursor.  t2 uses the cursor pTabList.a[1].iCursor.
		/// And so forth.  This routine generates code to open those VDBE cursors
		/// and sqlite3WhereEnd() generates the code to close them.
		///
		/// The code that sqlite3WhereBegin() generates leaves the cursors named
		/// in pTabList pointing at their appropriate entries.  The [...] code
		/// can use OP_Column and OP_Rowid opcodes on these cursors to extract
		/// data from the various tables of the loop.
		///
		/// If the WHERE clause is empty, the foreach loops must each scan their
		/// entire tables.  Thus a three-way join is an O(N^3) operation.  But if
		/// the tables have indices and there are terms in the WHERE clause that
		/// refer to those indices, a complete table scan can be avoided and the
		/// code will run much faster.  Most of the work of this routine is checking
		/// to see if there are indices that can be used to speed up the loop.
		///
		/// Terms of the WHERE clause are also used to limit which rows actually
		/// make it to the "..." in the middle of the loop.  After each "foreach",
		/// terms of the WHERE clause that use only terms in that loop and outer
		/// loops are evaluated and if false a jump is made around all subsequent
		/// inner loops (or around the "..." if the test occurs within the inner-
		/// most loop)
		///
		/// OUTER JOINS
		///
		/// An outer join of tables t1 and t2 is conceptally coded as follows:
		///
		///    foreach row1 in t1 do
		///      flag = 0
		///      foreach row2 in t2 do
		///        start:
		///          ...
		///          flag = 1
		///      end
		///      if flag==null then
		///        move the row2 cursor to a null row
		///        goto start
		///      fi
		///    end
		///
		/// ORDER BY CLAUSE PROCESSING
		///
		/// ppOrderBy is a pointer to the ORDER BY clause of a SELECT statement,
		/// if there is one.  If there is no ORDER BY clause or if this routine
		/// is called from an UPDATE or DELETE statement, then ppOrderBy is NULL.
		///
		/// If an index can be used so that the natural output order of the table
		/// scan is correct for the ORDER BY clause, then that index is used and
		/// ppOrderBy is set to NULL.  This is an optimization that prevents an
		/// unnecessary sort of the result set if an index appropriate for the
		/// ORDER BY clause already exists.
		///
		/// If the where clause loops cannot be arranged to provide the correct
		/// output order, then the ppOrderBy is unchanged.
		///
		///</summary>
		/*
    ** Generate the end of the WHERE loop.  See comments on
    ** sqlite3WhereBegin() for additional information.
    */static void sqlite3WhereEnd(WhereInfo pWInfo) {
			Parse pParse=pWInfo.pParse;
			Vdbe v=pParse.pVdbe;
			int i;
			WhereLevel pLevel;
			SrcList pTabList=pWInfo.pTabList;
			sqlite3 db=pParse.db;
			/* Generate loop termination code.
      */pParse.sqlite3ExprCacheClear();
			for(i=pWInfo.nLevel-1;i>=0;i--) {
				pLevel=pWInfo.a[i];
				v.sqlite3VdbeResolveLabel(pLevel.addrCont);
				if(pLevel.op!=OP_Noop) {
					v.sqlite3VdbeAddOp2(pLevel.op,pLevel.p1,pLevel.p2);
					v.sqlite3VdbeChangeP5(pLevel.p5);
				}
				if((pLevel.plan.wsFlags&WHERE_IN_ABLE)!=0&&pLevel.u._in.nIn>0) {
					InLoop pIn;
					int j;
					v.sqlite3VdbeResolveLabel(pLevel.addrNxt);
					for(j=pLevel.u._in.nIn;j>0;j--)//, pIn--)
					 {
						pIn=pLevel.u._in.aInLoop[j-1];
						v.sqlite3VdbeJumpHere(pIn.addrInTop+1);
						v.sqlite3VdbeAddOp2(OP_Next,pIn.iCur,pIn.addrInTop);
						v.sqlite3VdbeJumpHere(pIn.addrInTop-1);
					}
					sqlite3DbFree(db,ref pLevel.u._in.aInLoop);
				}
				v.sqlite3VdbeResolveLabel(pLevel.addrBrk);
				if(pLevel.iLeftJoin!=0) {
					int addr;
					addr=v.sqlite3VdbeAddOp1(OP_IfPos,pLevel.iLeftJoin);
					Debug.Assert((pLevel.plan.wsFlags&WHERE_IDX_ONLY)==0||(pLevel.plan.wsFlags&WHERE_INDEXED)!=0);
					if((pLevel.plan.wsFlags&WHERE_IDX_ONLY)==0) {
						v.sqlite3VdbeAddOp1(OP_NullRow,pTabList.a[i].iCursor);
					}
					if(pLevel.iIdxCur>=0) {
						v.sqlite3VdbeAddOp1(OP_NullRow,pLevel.iIdxCur);
					}
					if(pLevel.op==OP_Return) {
						v.sqlite3VdbeAddOp2(OP_Gosub,pLevel.p1,pLevel.addrFirst);
					}
					else {
						v.sqlite3VdbeAddOp2(OP_Goto,0,pLevel.addrFirst);
					}
					v.sqlite3VdbeJumpHere(addr);
				}
			}
			/* The "break" point is here, just past the end of the outer loop.
      ** Set it.
      */v.sqlite3VdbeResolveLabel(pWInfo.iBreak);
			/* Close all of the cursors that were opened by sqlite3WhereBegin.
      */Debug.Assert(pWInfo.nLevel==1||pWInfo.nLevel==pTabList.nSrc);
			for(i=0;i<pWInfo.nLevel;i++)//  for(i=0, pLevel=pWInfo.a; i<pWInfo.nLevel; i++, pLevel++){
			 {
				pLevel=pWInfo.a[i];
				SrcList_item pTabItem=pTabList.a[pLevel.iFrom];
				Table pTab=pTabItem.pTab;
				Debug.Assert(pTab!=null);
				if((pTab.tabFlags&TF_Ephemeral)==0&&pTab.pSelect==null&&(pWInfo.wctrlFlags&WHERE_OMIT_CLOSE)==0) {
					u32 ws=pLevel.plan.wsFlags;
					if(0==pWInfo.okOnePass&&(ws&WHERE_IDX_ONLY)==0) {
						v.sqlite3VdbeAddOp1(OP_Close,pTabItem.iCursor);
					}
					if((ws&WHERE_INDEXED)!=0&&(ws&WHERE_TEMP_INDEX)==0) {
						v.sqlite3VdbeAddOp1(OP_Close,pLevel.iIdxCur);
					}
				}
				/* If this scan uses an index, make code substitutions to read data
        ** from the index in preference to the table. Sometimes, this means
        ** the table need never be read from. This is a performance boost,
        ** as the vdbe level waits until the table is read before actually
        ** seeking the table cursor to the record corresponding to the current
        ** position in the index.
        **
        ** Calls to the code generator in between sqlite3WhereBegin and
        ** sqlite3WhereEnd will have created code that references the table
        ** directly.  This loop scans all that code looking for opcodes
        ** that reference the table and converts them into opcodes that
        ** reference the index.
        */if((pLevel.plan.wsFlags&WHERE_INDEXED)!=0)///* && 0 == db.mallocFailed */ )
				 {
					int k,j,last;
					VdbeOp pOp;
					Index pIdx=pLevel.plan.u.pIdx;
					Debug.Assert(pIdx!=null);
					//pOp = sqlite3VdbeGetOp( v, pWInfo.iTop );
					last=v.sqlite3VdbeCurrentAddr();
					for(k=pWInfo.iTop;k<last;k++)//, pOp++ )
					 {
						pOp=v.sqlite3VdbeGetOp(k);
						if(pOp.p1!=pLevel.iTabCur)
							continue;
						if(pOp.opcode==OP_Column) {
							for(j=0;j<pIdx.nColumn;j++) {
								if(pOp.p2==pIdx.aiColumn[j]) {
									pOp.p2=j;
									pOp.p1=pLevel.iIdxCur;
									break;
								}
							}
							Debug.Assert((pLevel.plan.wsFlags&WHERE_IDX_ONLY)==0||j<pIdx.nColumn);
						}
						else
							if(pOp.opcode==OP_Rowid) {
								pOp.p1=pLevel.iIdxCur;
								pOp.opcode=OP_IdxRowid;
							}
					}
				}
			}
			/* Final cleanup
      */pParse.nQueryLoop=pWInfo.savedNQueryLoop;
			whereInfoFree(db,pWInfo);
			return;
		}
	}
}
