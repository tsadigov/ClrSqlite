using System;
using System.Diagnostics;
using System.Text;
using Bitmask=System.UInt64;
using i16=System.Int16;
using u8=System.Byte;
using u16=System.UInt16;
using u32=System.UInt32;
using sqlite3_int64=System.Int64;

using Parse=Community.CsharpSqlite.Sqlite3.Parse;


namespace Community.CsharpSqlite
{
    using sqlite3_value = Engine.Mem;
    using Metadata;
    using Community.CsharpSqlite.Ast;

    namespace Ast
    {
        ///
        ///<summary>
        ///2001 September 15
        ///
        ///The author disclaims copyright to this source code.  In place of
        ///a legal notice, here is a blessing:
        ///
        ///May you do good and not evil.
        ///May you find forgiveness for yourself and forgive others.
        ///May you share freely, never taking more than you give.
        ///
        ///
        ///This module contains C code that generates VDBE code used to process
        ///the WHERE clause of SQL statements.  This module is responsible for
        ///generating the code that loops through a table looking for applicable
        ///rows.  Indices are selected and used to speed the search when doing
        ///so is applicable.  Because this module is responsible for selecting
        ///indices, you might also think of this module as the "query optimizer".
        ///
        ///</summary>
        ///<param name="Included in SQLite3 port to C#">SQLite;  2008 Noah B Hart</param>
        ///<param name="C#">SQLite is an independent reimplementation of the SQLite software library</param>
        ///<param name=""></param>
        ///<param name="SQLITE_SOURCE_ID: 2011">19 13:26:54 ed1da510a239ea767a01dc332b667119fa3c908ecd7</param>
        ///<param name=""></param>
        ///<param name=""></param>
        ///<param name=""></param>
        //#include "sqliteInt.h"

        ///
        ///<summary>
        ///Forward reference
        ///</summary>
        //typedef struct WhereClause WhereClause;
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
        /// the <op> using a bitmask encoding defined by wherec.WO_xxx below.  The
        /// use of a bitmask encoding for the operator allows us to search
        /// quickly for terms that match any of several different operators.
        ///
        /// A WhereTerm might also be two or more subterms connected by OR:
        ///
        ///         (t1.X <op> <expr>) OR (t1.Y <op> <expr>) OR ....
        ///
        /// In this second case, wtFlag as the WhereTermFlags.TERM_ORINFO set and eOperator==wherec.WO_OR
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
        public class WhereTerm
        {
            public Expr pExpr;
            ///
            ///<summary>
            ///Pointer to the subexpression that is this term 
            ///</summary>
            public int iParent;
            ///<summary>
            ///Disable pWC.a[iParent] when this term disabled
            ///</summary>
            public int leftCursor;
            ///
            ///<summary>
            ///Cursor number of X in "X <op> <expr>" 
            ///</summary>
            public class _u
            {
                public int leftColumn;
                ///
                ///<summary>
                ///Column number of X in "X <op> <expr>" 
                ///</summary>
                public WhereOrInfo pOrInfo;
                ///
                ///<summary>
                ///Extra information if eOperator==wherec.WO_OR 
                ///</summary>
                public WhereAndInfo pAndInfo;
                ///
                ///<summary>
                ///Extra information if eOperator==wherec.WO_AND 
                ///</summary>
            }
            public _u u = new _u();
            public u16 eOperator;
            ///
            ///<summary>
            ///A wherec.WO_xx value describing <op> 
            ///</summary>
            public WhereTermFlags wtFlags;
            ///
            ///<summary>
            ///WhereTermFlags.TERM_xxx bit flags.  See below 
            ///</summary>
            public u8 nChild;
            ///
            ///<summary>
            ///Number of children that must disable us 
            ///</summary>
            public WhereClause pWC;
            ///
            ///<summary>
            ///The clause this term is part of 
            ///</summary>
            public Bitmask prereqRight;
            ///
            ///<summary>
            ///Bitmask of tables used by pExpr.pRight 
            ///</summary>
            public Bitmask prereqAll;
            ///
            ///<summary>
            ///Bitmask of tables referenced by pExpr 
            ///</summary>
            public int termCanDriveIndex(///
                ///<summary>
                ///WHERE clause term to check 
                ///</summary>
            SrcList_item pSrc,///
                ///<summary>
                ///Table we are trying to access 
                ///</summary>
            Bitmask notReady///
                ///<summary>
                ///Tables in outer loops of the join 
                ///</summary>
            )
            {
                char aff;
                if (this.leftCursor != pSrc.iCursor)
                    return 0;
                if (this.eOperator != wherec.WO_EQ)
                    return 0;
                if ((this.prereqRight & notReady) != 0)
                    return 0;
                aff = pSrc.pTab.aCol[this.u.leftColumn].affinity;
                if (!this.pExpr.sqlite3IndexAffinityOk(aff))
                    return 0;
                return 1;
            }
        }
        ///
        ///<summary>
        ///Allowed values of WhereTerm.wtFlags
        ///
        ///</summary>
        //#define WhereTermFlags.TERM_DYNAMIC    0x01   /* Need to call exprc.sqlite3ExprDelete(db, ref pExpr) */
        //#define WhereTermFlags.TERM_VIRTUAL    0x02   /* Added by the optimizer.  Do not code */
        //#define WhereTermFlags.TERM_CODED      0x04   /* This term is already coded */
        //#define WhereTermFlags.TERM_COPIED     0x08   /* Has a child */
        //#define WhereTermFlags.TERM_ORINFO     0x10   /* Need to free the WhereTerm.u.pOrInfo object */
        //#define WhereTermFlags.TERM_ANDINFO    0x20   /* Need to free the WhereTerm.u.pAndInfo obj */
        //#define WhereTermFlags.TERM_OR_OK      0x40   /* Used during OR-clause processing */
#if SQLITE_ENABLE_STAT2
																																																    //  define WhereTermFlags.TERM_VNULL    0x80   /* Manufactured x>NULL or x<=NULL term */
#else
        //#  define WhereTermFlags.TERM_VNULL    0x00   /* Disabled if not using stat2 */
#endif

        public enum WhereTermFlags : byte
        {
            TERM_DYNAMIC = 0x01,
            ///
            ///<summary>
            ///Need to call exprc.sqlite3ExprDelete(db, ref pExpr) 
            ///</summary>
            TERM_VIRTUAL = 0x02,
            ///
            ///<summary>
            ///Added by the optimizer.  Do not code 
            ///</summary>
            TERM_CODED = 0x04,
            ///
            ///<summary>
            ///This term is already coded 
            ///</summary>
            TERM_COPIED = 0x08,
            ///
            ///<summary>
            ///Has a child 
            ///</summary>
            TERM_ORINFO = 0x10,
            ///
            ///<summary>
            ///Need to free the WhereTerm.u.pOrInfo object 
            ///</summary>
            TERM_ANDINFO = 0x20,
            ///
            ///<summary>
            ///Need to free the WhereTerm.u.pAndInfo obj 
            ///</summary>
            TERM_OR_OK = 0x40,
            ///
            ///<summary>
            ///</summary>
            ///<param name="Used during OR">clause processing </param>
#if SQLITE_ENABLE_STAT2
																																																    const int TERM_VNULL = 0x80;  /* Manufactured x>NULL or x<=NULL term */
#else
            TERM_VNULL = 0x0
            ///<summary>
            ///Disabled if not using stat2
            ///</summary>
#endif
        }
        ///<summary>
        /// An instance of the following structure holds all information about a
        /// WHERE clause.  Mostly this is a container for one or more WhereTerms.
        ///
        ///</summary>
        public class WhereClause
        {
            public Parse pParse;
            ///
            ///<summary>
            ///The parser context 
            ///</summary>
            public WhereMaskSet pMaskSet;
            ///
            ///<summary>
            ///Mapping of table cursor numbers to bitmasks 
            ///</summary>
            public Bitmask vmask;
            ///
            ///<summary>
            ///Bitmask identifying virtual table cursors 
            ///</summary>
            public u8 op;
            ///
            ///<summary>
            ///Split operator.  TokenType.TK_AND or TokenType.TK_OR 
            ///</summary>
            public int nTerm;
            ///
            ///<summary>
            ///Number of terms 
            ///</summary>
            public int nSlot;
            ///
            ///<summary>
            ///Number of entries in a[] 
            ///</summary>
            public WhereTerm[] a;
            ///
            ///<summary>
            ///Each a[] describes a term of the WHERE cluase 
            ///</summary>
#if (SQLITE_SMALL_STACK)
																																																																								public WhereTerm[] aStatic = new WhereTerm[1];    /* Initial static space for a[] */
#else
            public WhereTerm[] aStatic = new WhereTerm[8];
            ///<summary>
            ///Initial static space for a[]
            ///</summary>
#endif
            public void CopyTo(WhereClause wc)
            {
                wc.pParse = this.pParse;
                wc.pMaskSet = new WhereMaskSet();
                this.pMaskSet.CopyTo(wc.pMaskSet);
                wc.Operator = this.Operator;
                wc.nTerm = this.nTerm;
                wc.nSlot = this.nSlot;
                wc.a = (WhereTerm[])this.a.Clone();
                wc.aStatic = (WhereTerm[])this.aStatic.Clone();
            }
            public///<summary>
                /// Deallocate a WhereClause structure.  The WhereClause structure
                /// itself is not freed.  This routine is the inverse of whereClauseInit().
                ///
                ///</summary>
            void whereClauseClear()
            {
                int i;
                WhereTerm a;
                Connection db = this.pParse.db;
                for (i = this.nTerm - 1; i >= 0; i--)//, a++)
                {
                    a = this.a[i];
                    if ((a.wtFlags & WhereTermFlags.TERM_DYNAMIC) != 0)
                    {
                        exprc.sqlite3ExprDelete(db, ref a.pExpr);
                    }
                    if ((a.wtFlags & WhereTermFlags.TERM_ORINFO) != 0)
                    {
                        db.whereOrInfoDelete(a.u.pOrInfo);
                    }
                    else
                        if ((a.wtFlags & WhereTermFlags.TERM_ANDINFO) != 0)
                        {
                            db.whereAndInfoDelete(a.u.pAndInfo);
                        }
                }
                if (this.a != this.aStatic)
                {
                    db.sqlite3DbFree(ref this.a);
                }
            }
            public void whereClauseInit(///
                ///<summary>
                ///The WhereClause to be initialized 
                ///</summary>
            Parse pParse,///
                ///<summary>
                ///The parsing context 
                ///</summary>
            WhereMaskSet pMaskSet///
                ///<summary>
                ///Mapping from table cursor numbers to bitmasks 
                ///</summary>
            )
            {
                this.pParse = pParse;
                this.pMaskSet = pMaskSet;
                this.nTerm = 0;
                this.nSlot = Sqlite3.ArraySize(this.aStatic) - 1;
                this.a = this.aStatic;
                this.vmask = 0;
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
                /// If the wtFlags argument includes WhereTermFlags.TERM_DYNAMIC, then responsibility
                /// for freeing the expression p is Debug.Assumed by the WhereClause object pWC.
                /// This is true even if this routine fails to allocate a new WhereTerm.
                ///
                /// WARNING:  This routine might reallocate the space used to store
                /// WhereTerms.  All pointers to WhereTerms should be invalidated after
                /// calling this routine.  Such pointers may be reinitialized by referencing
                /// the pWC.a[] array.
                ///
                ///</summary>
            int whereClauseInsert(Expr p, WhereTermFlags wtFlags)
            {
                WhereTerm pTerm;
                int idx;
                sqliteinth.testcase(wtFlags & WhereTermFlags.TERM_VIRTUAL);
                ///
                ///<summary>
                ///</summary>
                ///<param name="EV: R">15100 </param>
                if (this.nTerm >= this.nSlot)
                {
                    //WhereTerm pOld = pWC.a;
                    Connection db = this.pParse.db;
                    Array.Resize(ref this.a, this.nSlot * 2);
                    //pWC.a = sqlite3DbMallocRaw(db, sizeof(pWC.a[0])*pWC.nSlot*2 );
                    //if( pWC.a==null ){
                    //  if( wtFlags & WhereTermFlags.TERM_DYNAMIC ){
                    //    exprc.sqlite3ExprDelete(db, ref p);
                    //  }
                    //  pWC.a = pOld;
                    //  return 0;
                    //}
                    //memcpy(pWC.a, pOld, sizeof(pWC.a[0])*pWC.nTerm);
                    //if( pOld!=pWC.aStatic ){
                    //  sqlite3DbFree(db, ref pOld);
                    //}
                    //pWC.nSlot = sqlite3DbMallocSize(db, pWC.a)/sizeof(pWC.a[0]);
                    this.nSlot = this.a.Length - 1;
                }
                this.a[idx = this.nTerm++] = new WhereTerm();
                pTerm = this.a[idx];
                pTerm.pExpr = p;
                pTerm.wtFlags = wtFlags;
                pTerm.pWC = this;
                pTerm.iParent = -1;
                return idx;
            }
            public void whereSplit(Expr pExpr, TokenType op)
            {
                this.Operator = op;
                if (pExpr == null)
                    return;
                if (pExpr.Operator != op)
                {
                    this.whereClauseInsert(pExpr, 0);
                }
                else
                {
                    this.whereSplit(pExpr.pLeft, op);
                    this.whereSplit(pExpr.pRight, op);
                }
            }
            public WhereTerm findTerm(///
                ///<summary>
                ///The WHERE clause to be searched 
                ///</summary>
            int iCur,///
                ///<summary>
                ///Cursor number of LHS 
                ///</summary>
            int iColumn,///
                ///<summary>
                ///Column number of LHS 
                ///</summary>
            Bitmask notReady,///
                ///<summary>
                ///RHS must not overlap with this mask 
                ///</summary>
            u32 op,///
                ///<summary>
                ///Mask of wherec.WO_xx values describing operator 
                ///</summary>
            Index pIdx///
                ///<summary>
                ///Must be compatible with this index, if not NULL 
                ///</summary>
            )
            {
                WhereTerm pTerm;
                int k;
                Debug.Assert(iCur >= 0);
                op &= wherec.WO_ALL;
                for (k = this.nTerm; k != 0; k--)//, pTerm++)
                {
                    pTerm = this.a[this.nTerm - k];
                    if (pTerm.leftCursor == iCur && (pTerm.prereqRight & notReady) == 0 && pTerm.u.leftColumn == iColumn && (pTerm.eOperator & op) != 0)
                    {
                        if (pIdx != null && pTerm.eOperator != wherec.WO_ISNULL)
                        {
                            Expr pX = pTerm.pExpr;
                            CollSeq pColl;
                            char idxaff;
                            int j;
                            Parse pParse = this.pParse;
                            idxaff = pIdx.pTable.aCol[iColumn].affinity;
                            if (!pX.sqlite3IndexAffinityOk(idxaff))
                                continue;
                            ///
                            ///<summary>
                            ///Figure out the collation sequence required from an index for
                            ///it to be useful for optimising expression pX. Store this
                            ///value in variable pColl.
                            ///
                            ///</summary>
                            Debug.Assert(pX.pLeft != null);
                            pColl = pParse.sqlite3BinaryCompareCollSeq(pX.pLeft, pX.pRight);
                            Debug.Assert(pColl != null || pParse.nErr != 0);
                            for (j = 0; pIdx.aiColumn[j] != iColumn; j++)
                            {
                                if (Sqlite3.NEVER(j >= pIdx.nColumn))
                                    return null;
                            }
                            if (pColl != null && !pColl.zName.Equals(pIdx.azColl[j], StringComparison.InvariantCultureIgnoreCase))
                                continue;
                        }
                        return pTerm;
                    }
                }
                return null;
            }
            public TokenType Operator
            {
                get
                {
                    return (TokenType)op;
                }
                set
                {
                    op = (u8)value;
                }
            }
        }
        ///<summary>
        /// A WhereTerm with eOperator==wherec.WO_OR has its u.pOrInfo pointer set to
        /// a dynamically allocated instance of the following structure.
        ///
        ///</summary>
        public class WhereOrInfo
        {
            public WhereClause wc = new WhereClause();
            ///
            ///<summary>
            ///Decomposition into subterms 
            ///</summary>
            public Bitmask indexable;
            ///
            ///<summary>
            ///Bitmask of all indexable tables in the clause 
            ///</summary>
        };

        ///<summary>
        /// A WhereTerm with eOperator==wherec.WO_AND has its u.pAndInfo pointer set to
        /// a dynamically allocated instance of the following structure.
        ///
        ///</summary>
        public class WhereAndInfo
        {
            public WhereClause wc = new WhereClause();
            ///
            ///<summary>
            ///The subexpression broken out 
            ///</summary>
        };

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
        public class WhereMaskSet
        {
            public int n;
            ///<summary>
            ///Number of Debug.Assigned cursor values
            ///</summary>
            public int[] ix = new int[Globals.BMS];
            ///
            ///<summary>
            ///Cursor Debug.Assigned to each bit 
            ///</summary>
            public void CopyTo(WhereMaskSet wms)
            {
                wms.n = this.n;
                wms.ix = (int[])this.ix.Clone();
            }
            public Bitmask getMask(int iCursor)
            {
                int i;
                Debug.Assert(this.n <= (int)sizeof(Bitmask) * 8);
                for (i = 0; i < this.n; i++)
                {
                    if (this.ix[i] == iCursor)
                    {
                        return ((Bitmask)1) << i;
                    }
                }
                return 0;
            }
            public void createMask(int iCursor)
            {
                Debug.Assert(this.n < Sqlite3.ArraySize(this.ix));
                this.ix[this.n++] = iCursor;
            }
            public Bitmask exprTableUsage(Expr p)
            {
                Bitmask mask = 0;
                if (p == null)
                    return 0;
                if (p.Operator == TokenType.TK_COLUMN)
                {
                    mask = this.getMask(p.iTable);
                    return mask;
                }
                mask = this.exprTableUsage(p.pRight);
                mask |= this.exprTableUsage(p.pLeft);
                if (p.ExprHasProperty(ExprFlags.EP_xIsSelect))
                {
                    mask |= this.exprSelectTableUsage(p.x.pSelect);
                }
                else
                {
                    mask |= this.exprListTableUsage(p.x.pList);
                }
                return mask;
            }
            public Bitmask exprListTableUsage(ExprList pList)
            {
                int i;
                Bitmask mask = 0;
                if (pList != null)
                {
                    for (i = 0; i < pList.nExpr; i++)
                    {
                        mask |= this.exprTableUsage(pList.a[i].pExpr);
                    }
                }
                return mask;
            }
            public Bitmask exprSelectTableUsage(Select pS)
            {
                Bitmask mask = 0;
                while (pS != null)
                {
                    mask |= this.exprListTableUsage(pS.pEList);
                    mask |= this.exprListTableUsage(pS.pGroupBy);
                    mask |= this.exprListTableUsage(pS.pOrderBy);
                    mask |= this.exprTableUsage(pS.pWhere);
                    mask |= this.exprTableUsage(pS.pHaving);
                    pS = pS.pPrior;
                }
                return mask;
            }
        }
        ///
        ///<summary>
        ///A WhereCost object records a lookup strategy and the estimated
        ///cost of pursuing that strategy.
        ///
        ///</summary>
        public class WhereCost
        {
            public WherePlan plan = new WherePlan();
            ///
            ///<summary>
            ///The lookup strategy 
            ///</summary>
            public double rCost;
            ///<summary>
            ///Overall cost of pursuing this search strategy
            ///</summary>
            public Bitmask used;
            ///
            ///<summary>
            ///Bitmask of cursors used by this plan 
            ///</summary>
            public void Clear()
            {
                plan.Clear();
                rCost = 0;
                used = 0;
            }
        };


        public class wherec
        {


            ///<summary>
            /// Flags appropriate for the wctrlFlags parameter of sqlite3WhereBegin()
            /// and the WhereInfo.wctrlFlags member.
            ///
            ///</summary>
            //#define wherec.WHERE_ORDERBY_NORMAL   0x0000 /* No-op */
            //#define wherec.WHERE_ORDERBY_MIN      0x0001 /* ORDER BY processing for min() func */
            //#define wherec.WHERE_ORDERBY_MAX      0x0002 /* ORDER BY processing for max() func */
            //#define wherec.WHERE_ONEPASS_DESIRED  0x0004 /* Want to do one-pass UPDATE/DELETE */
            //#define wherec.WHERE_DUPLICATES_OK    0x0008 /* Ok to return a row more than once */
            //#define wherec.WHERE_OMIT_OPEN        0x0010  /* Table cursors are already open */
            //#define wherec.WHERE_OMIT_CLOSE       0x0020  /* Omit close of table & index cursors */
            //#define wherec.WHERE_FORCE_TABLE      0x0040 /* Do not use an index-only search */
            //#define wherec.WHERE_ONETABLE_ONLY    0x0080 /* Only code the 1st table in pTabList */
            public const int WHERE_ORDERBY_NORMAL = 0x0000;
            public const int WHERE_ORDERBY_MIN = 0x0001;
            public const int WHERE_ORDERBY_MAX = 0x0002;
            public const int WHERE_ONEPASS_DESIRED = 0x0004;
            public const int WHERE_DUPLICATES_OK = 0x0008;
            public const int WHERE_OMIT_OPEN = 0x0010;
            public const int WHERE_OMIT_CLOSE = 0x0020;
            public const int WHERE_FORCE_TABLE = 0x0040;
            public const int WHERE_ONETABLE_ONLY = 0x0080;



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
            public static void WHERETRACE(string X, params object[] ap)
            {
            }
#endif
            ///
            ///<summary>
            ///Bitmasks for the operators that indices are able to exploit.  An
            ///</summary>
            ///<param name="OR">ed combination of these values can be used when searching for</param>
            ///<param name="terms in the where clause.">terms in the where clause.</param>
            ///<param name=""></param>
            //#define wherec.WO_IN     0x001
            //#define wherec.WO_EQ     0x002
            //#define wherec.WO_LT     (wherec.WO_EQ<<(TokenType.TK_LT-TokenType.TK_EQ))
            //#define wherec.WO_LE     (wherec.WO_EQ<<(TokenType.TK_LE-TokenType.TK_EQ))
            //#define wherec.WO_GT     (wherec.WO_EQ<<(TokenType.TK_GT-TokenType.TK_EQ))
            //#define wherec.WO_GE     (wherec.WO_EQ<<(TokenType.TK_GE-TokenType.TK_EQ))
            //#define wherec.WO_MATCH  0x040
            //#define wherec.WO_ISNULL 0x080
            //#define wherec.WO_OR     0x100       /* Two or more OR-connected terms */
            //#define wherec.WO_AND    0x200       /* Two or more AND-connected terms */
            //#define wherec.WO_NOOP   0x800       /* This term does not restrict search space */
            //#define wherec.WO_ALL    0xfff       /* Mask of all possible wherec.WO_* values */
            //#define wherec.WO_SINGLE 0x0ff       /* Mask of all non-compound wherec.WO_* values */
            public const int WO_IN = 0x001;
            public const int WO_EQ = 0x002;
            public const int WO_LT = (wherec.WO_EQ << (TokenType.TK_LT - TokenType.TK_EQ));
            public const int WO_LE = (wherec.WO_EQ << (TokenType.TK_LE - TokenType.TK_EQ));
            public const int WO_GT = (wherec.WO_EQ << (TokenType.TK_GT - TokenType.TK_EQ));
            public const int WO_GE = (wherec.WO_EQ << (TokenType.TK_GE - TokenType.TK_EQ));
            public const int WO_MATCH = 0x040;
            public const int WO_ISNULL = 0x080;
            public const int WO_OR = 0x100;
            ///
            ///<summary>
            ///</summary>
            ///<param name="Two or more OR">connected terms </param>
            public const int WO_AND = 0x200;
            ///
            ///<summary>
            ///</summary>
            ///<param name="Two or more AND">connected terms </param>
            public const int WO_NOOP = 0x800;
            ///
            ///<summary>
            ///This term does not restrict search space 
            ///</summary>
            public const int WO_ALL = 0xfff;
            ///
            ///<summary>
            ///Mask of all possible WO_* values 
            ///</summary>
            public const int WO_SINGLE = 0x0ff;
            ///
            ///<summary>
            ///</summary>
            ///<param name="Mask of all non">compound WO_* values </param>
            ///<summary>
            /// Value for wsFlags returned by bestIndex() and stored in
            /// WhereLevel.wsFlags.  These flags determine which search
            /// strategies are appropriate.
            ///
            /// The least significant 12 bits is reserved as a mask for WO_ values above.
            /// The WhereLevel.wsFlags field is usually set to WO_IN|wherec.WO_EQ|wherec.WO_ISNULL.
            /// But if the table is the right table of a left join, WhereLevel.wsFlags
            /// is set to WO_IN|wherec.WO_EQ.  The WhereLevel.wsFlags field can then be used as
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
            public const int WHERE_ROWID_EQ = 0x00001000;
            public const int WHERE_ROWID_RANGE = 0x00002000;
            public const int WHERE_COLUMN_EQ = 0x00010000;
            public const int WHERE_COLUMN_RANGE = 0x00020000;
            public const int WHERE_COLUMN_IN = 0x00040000;
            public const int WHERE_COLUMN_NULL = 0x00080000;
            public const int WHERE_INDEXED = 0x000f0000;
            public const int WHERE_IN_ABLE = 0x000f1000;
            public const int WHERE_NOT_FULLSCAN = 0x100f3000;
            public const int WHERE_TOP_LIMIT = 0x00100000;
            public const int WHERE_BTM_LIMIT = 0x00200000;
            public const int WHERE_BOTH_LIMIT = 0x00300000;
            public const int WHERE_IDX_ONLY = 0x00800000;
            public const int WHERE_ORDERBY = 0x01000000;
            public const int WHERE_REVERSE = 0x02000000;
            public const int WHERE_UNIQUE = 0x04000000;
            public const int WHERE_VIRTUALTABLE = 0x08000000;
            public const int WHERE_MULTI_OR = 0x10000000;
            public const int WHERE_TEMP_INDEX = 0x20000000;
            ///
            ///<summary>
            ///Initialize a preallocated WhereClause structure.
            ///
            ///</summary>
            ///<summary>
            ///Forward reference
            ///</summary>
            //static void whereClauseClear(WhereClause);
            ///<summary>
            /// Deallocate all memory Debug.Associated with a WhereOrInfo object.
            ///
            ///</summary>
            ///<summary>
            /// Deallocate all memory Debug.Associated with a WhereAndInfo object.
            ///
            ///</summary>
            ///
            ///<summary>
            ///This routine identifies subexpressions in the WHERE clause where
            ///each subexpression is separated by the AND operator or some other
            ///operator specified in the op parameter.  The WhereClause structure
            ///is filled with pointers to subexpressions.  For example:
            ///
            ///WHERE  a=='hello' AND coalesce(b,11)<10 AND (c+12!=d OR c==22)
            ///\________/     \_______________/     \________________/
            ///slot[0]            slot[1]               slot[2]
            ///
            ///The original WHERE clause in pExpr is unaltered.  All this routine
            ///does is make slot[] entries point to substructure within pExpr.
            ///
            ///In the previous sentence and in the diagram, "slot[]" refers to
            ///the WhereClause.a[] array.  The slot[] array grows as needed to contain
            ///all terms of the WHERE clause.
            ///
            ///</summary>
            ///<summary>
            /// Initialize an expression mask set (a WhereMaskSet object)
            ///
            ///</summary>
            //#define initMaskSet(P)  memset(P, 0, sizeof(*P))
            ///<summary>
            /// Return the bitmask for the given cursor number.  Return 0 if
            /// iCursor is not in the set.
            ///
            ///</summary>
            ///
            ///<summary>
            ///Create a new mask for cursor iCursor.
            ///
            ///There is one cursor per table in the FROM clause.  The number of
            ///tables in the FROM clause is limited by a test early in the
            ///sqlite3WhereBegin() routine.  So we know that the pMaskSet.ix[]
            ///array will never overflow.
            ///
            ///</summary>
            ///<summary>
            /// This routine walks (recursively) an expression tree and generates
            /// a bitmask indicating which tables are used in that expression
            /// tree.
            ///
            /// In order for this routine to work, the calling function must have
            /// previously invoked sqlite3ResolveExprNames() on the expression.  See
            /// the header comment on that routine for additional information.
            /// The sqlite3ResolveExprNames() routines looks for column names and
            /// sets their opcodes to TokenType.TK_COLUMN and their Expr.iTable fields to
            /// the VDBE cursor number of the table.  This routine just has to
            /// translate the cursor numbers into bitmask values and OR all
            /// the bitmasks together.
            ///
            ///</summary>
            //static Bitmask exprListTableUsage(WhereMaskSet*, ExprList);
            //static Bitmask exprSelectTableUsage(WhereMaskSet*, Select);
            ///
            ///<summary>
            ///Return TRUE if the given operator is one of the operators that is
            ///allowed for an indexable WHERE clause term.  The allowed operators are
            ///"=", "<", ">", "<=", ">=", and "IN".
            ///
            ///</summary>
            ///<param name="IMPLEMENTATION">26393 To be usable by an index a term must be</param>
            ///<param name="of one of the following forms: column = expression column > expression">of one of the following forms: column = expression column > expression</param>
            ///<param name="column >= expression column < expression column <= expression">column >= expression column < expression column <= expression</param>
            ///<param name="expression = column expression > column expression >= column">expression = column expression > column expression >= column</param>
            ///<param name="expression < column expression <= column column IN">expression < column expression <= column column IN</param>
            ///<param name="(expression">list) column IN (subquery) column IS NULL</param>
            ///<param name=""></param>
            public static bool allowedOp(TokenType op)
            {
                Debug.Assert(TokenType.TK_GT > TokenType.TK_EQ && TokenType.TK_GT < TokenType.TK_GE);
                Debug.Assert(TokenType.TK_LT > TokenType.TK_EQ && TokenType.TK_LT < TokenType.TK_GE);
                Debug.Assert(TokenType.TK_LE > TokenType.TK_EQ && TokenType.TK_LE < TokenType.TK_GE);
                Debug.Assert(TokenType.TK_GE == TokenType.TK_EQ + 4);
                return op == TokenType.TK_IN || (op >= TokenType.TK_EQ && op <= TokenType.TK_GE) || op == TokenType.TK_ISNULL;
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
            /// attached to the right. For the same reason the ExprFlags.EP_ExpCollate flag
            /// is not commuted.
            ///
            ///</summary>
            ///<summary>
            /// Translate from TokenType.TK_xx operator to WO_xx bitmask.
            ///
            ///</summary>
            public static u16 operatorMask(TokenType op)
            {
                u16 c;
                Debug.Assert(allowedOp(op));
                if (op == TokenType.TK_IN)
                {
                    c = wherec.WO_IN;
                }
                else
                    if (op == TokenType.TK_ISNULL)
                    {
                        c = wherec.WO_ISNULL;
                    }
                    else
                    {
                        Debug.Assert((wherec.WO_EQ << (op - TokenType.TK_EQ)) < 0x7fff);
                        c = (u16)(wherec.WO_EQ << (op - TokenType.TK_EQ));
                    }
                Debug.Assert(op != TokenType.TK_ISNULL || c == wherec.WO_ISNULL);
                Debug.Assert(op != TokenType.TK_IN || c == wherec.WO_IN);
                Debug.Assert(op != TokenType.TK_EQ || c == wherec.WO_EQ);
                Debug.Assert(op != TokenType.TK_LT || c == wherec.WO_LT);
                Debug.Assert(op != TokenType.TK_LE || c == wherec.WO_LE);
                Debug.Assert(op != TokenType.TK_GT || c == wherec.WO_GT);
                Debug.Assert(op != TokenType.TK_GE || c == wherec.WO_GE);
                return c;
            }
            ///
            ///<summary>
            ///Search for a term in the WHERE clause that is of the form "X <op> <expr>"
            ///where X is a reference to the iColumn of table iCur and <op> is one of
            ///the WO_xx operator codes specified by the op parameter.
            ///Return a pointer to the term.  Return 0 if not found.
            ///
            ///</summary>
            ///<summary>
            ///Forward reference
            ///</summary>
            //static void exprAnalyze(SrcList*, WhereClause*, int);
            ///<summary>
            /// Call exprAnalyze on all terms in a WHERE clause.
            ///
            ///
            ///
            ///</summary>
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
#endif
            ///<summary>
            /// If the pBase expression originated in the ON or USING clause of
            /// a join, then transfer the appropriate markings over to derived.
            ///</summary>
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
            ///     WhereTerm.wtFlags   |=  WhereTermFlags.TERM_ORINFO
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
            /// analyzed separately.  The original term is marked with WhereTermFlags.TERM_COPIED
            /// and the new term is marked with WhereTermFlags.TERM_DYNAMIC (because it's pExpr
            /// needs to be freed with the WhereClause) and WhereTermFlags.TERM_VIRTUAL (because it
            /// is a commuted copy of a prior term.)  The original term has nChild=1
            /// and the copy has idxParent set to the index of the original term.
            ///</summary>
            ///<summary>
            /// Return TRUE if any of the expressions in pList.a[iFirst...] contain
            /// a reference to any table other than the iBase table.
            ///
            ///</summary>
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
            ///

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
            public static void TRACE_IDX_INPUTS(sqlite3_index_info p)
            {
            }
            //#define TRACE_IDX_OUTPUTS(A)
            public static void TRACE_IDX_OUTPUTS(sqlite3_index_info p)
            {
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
            /// by passing the pointer returned by this function to //malloc_cs.sqlite3_free().
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
            ///
            ///<summary>
            ///Compute the best index for a virtual table.
            ///
            ///The best index is computed by the xBestIndex method of the virtual
            ///table module.  This routine is really just a wrapper that sets up
            ///the sqlite3_index_info structure that is used to communicate with
            ///xBestIndex.
            ///
            ///In a join, this routine might be called multiple times for the
            ///same virtual table.  The sqlite3_index_info structure is created
            ///and initialized on the first invocation and reused on all subsequent
            ///invocations.  The sqlite3_index_info structure is also used when
            ///code is generated to access the virtual table.  The whereInfoDelete()
            ///routine takes care of freeing the sqlite3_index_info structure after
            ///everybody has finished with it.
            ///
            ///</summary>
#endif
            ///
            ///<summary>
            ///Argument pIdx is a pointer to an index structure that has an array of
            ///SQLITE_INDEX_SAMPLES evenly spaced samples of the first indexed column
            ///stored in Index.aSample. These samples divide the domain of values stored
            ///the index into (SQLITE_INDEX_SAMPLES+1) regions.
            ///Region 0 contains all values less than the first sample value. Region
            ///1 contains values between the first and second samples.  Region 2 contains
            ///values between samples 2 and 3.  And so on.  Region SQLITE_INDEX_SAMPLES
            ///contains values larger than the last sample.
            ///
            ///If the index contains many duplicates of a single value, then it is
            ///possible that two or more adjacent samples can hold the same value.
            ///When that is the case, the smallest possible region code is returned
            ///when roundUp is false and the largest possible region code is returned
            ///when roundUp is true.
            ///
            ///If successful, this function determines which of the regions value 
            ///pVal lies in, sets *piRegion to the region index (a value between 0
            ///and SQLITE_INDEX_SAMPLES+1, inclusive) and returns SqlResult.SQLITE_OK.
            ///Or, if an OOM occurs while converting text values between encodings,
            ///SQLITE_NOMEM is returned and *piRegion is undefined.
            ///</summary>
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
      if ( Sqlite3.ALWAYS( pVal ) )
      {
        IndexSample[] aSample = pIdx.aSample;
        int i = 0;
        int eType = vdbeapi.sqlite3_value_type( pVal );

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
              utilc.sqlite3ErrorMsg( pParse, "no such collation sequence: %s",
                  pIdx.azColl );
              return SqlResult.SQLITE_ERROR;
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
      return SqlResult.SQLITE_OK;
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
            /// If an error occurs, return an error code. Otherwise, SqlResult.SQLITE_OK.
            ///</summary>
#if SQLITE_ENABLE_STAT2
																																												    static int valueFromExpr(
    Parse pParse,
    Expr pExpr,
    char aff,
    ref sqlite3_value pp
    )
    {
      if ( pExpr.op == TokenType.TK_VARIABLE
      || ( pExpr.op == TokenType.TK_REGISTER && pExpr.op2 == TokenType.TK_VARIABLE )
      )
      {
        int iVar = pExpr.iColumn;
        sqlite3VdbeSetVarmask( pParse.pVdbe, iVar ); /* IMP: R-23257-02778 */
        pp = sqlite3VdbeGetValue( pParse.pReprepare, iVar, (u8)aff );
        return SqlResult.SQLITE_OK;
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
** Write the estimated row count into *pnRow and return SqlResult.SQLITE_OK. 
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
** Write the estimated row count into *pnRow and return SqlResult.SQLITE_OK. 
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
      var rc = SqlResult.SQLITE_OK;       /* Subfunction return code */
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
        if ( pVal == null || vdbeapi.sqlite3_value_type( pVal ) == SQLITE_NULL )
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
      if ( rc == SqlResult.SQLITE_OK )
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
            ///
            ///<summary>
            ///Generate the end of the WHERE loop.  See comments on
            ///sqlite3WhereBegin() for additional information.
            ///
            ///</summary>
        }
    }
}