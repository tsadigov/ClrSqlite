using System;
using System.Diagnostics;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Text;
using Bitmask = System.UInt64;
using i16 = System.Int16;
using i64 = System.Int64;
using sqlite3_int64 = System.Int64;
using u8 = System.Byte;
using u16 = System.UInt16;
using u32 = System.UInt32;
using u64 = System.UInt64;
using unsigned = System.UInt64;
using Pgno = System.UInt32;

#if !SQLITE_MAX_VARIABLE_NUMBER
using ynVar = System.Int16;

#else
using ynVar = System.Int32; 
#endif

namespace Community.CsharpSqlite
{
    public partial class Sqlite3
    {



        ///
        ///<summary>
        ///An instance of the following structure contains all information
        ///needed to generate code for a single SELECT statement.
        ///
        ///</summary>
        ///<param name="nLimit is set to ">1 if there is no LIMIT clause.  nOffset is set to 0.</param>
        ///<param name="If there is a LIMIT clause, the parser sets nLimit to the value of the">If there is a LIMIT clause, the parser sets nLimit to the value of the</param>
        ///<param name="limit and nOffset to the value of the offset (or 0 if there is not">limit and nOffset to the value of the offset (or 0 if there is not</param>
        ///<param name="offset).  But later on, nLimit and nOffset become the memory locations">offset).  But later on, nLimit and nOffset become the memory locations</param>
        ///<param name="in the VDBE that record the limit and offset counters.">in the VDBE that record the limit and offset counters.</param>
        ///<param name=""></param>
        ///<param name="addrOpenEphm[] entries contain the address of OP_OpenEphemeral opcodes.">addrOpenEphm[] entries contain the address of OP_OpenEphemeral opcodes.</param>
        ///<param name="These addresses must be stored so that we can go back and fill in">These addresses must be stored so that we can go back and fill in</param>
        ///<param name="the P4_KEYINFO and P2 parameters later.  Neither the KeyInfo nor">the P4_KEYINFO and P2 parameters later.  Neither the KeyInfo nor</param>
        ///<param name="the number of columns in P2 can be computed at the same time">the number of columns in P2 can be computed at the same time</param>
        ///<param name="as the OP_OpenEphm instruction is coded because not">as the OP_OpenEphm instruction is coded because not</param>
        ///<param name="enough information about the compound query is known at that point.">enough information about the compound query is known at that point.</param>
        ///<param name="The KeyInfo for addrOpenTran[0] and [1] contains collating sequences">The KeyInfo for addrOpenTran[0] and [1] contains collating sequences</param>
        ///<param name="for the result set.  The KeyInfo for addrOpenTran[2] contains collating">for the result set.  The KeyInfo for addrOpenTran[2] contains collating</param>
        ///<param name="sequences for the ORDER BY clause.">sequences for the ORDER BY clause.</param>
        ///<param name=""></param>

        public class Select
        {
            public ExprList pEList;

            ///
            ///<summary>
            ///The fields of the result 
            ///</summary>

            public u8 tk_op;
            public TokenType TokenOp {
                get { return (TokenType)tk_op; }
                set { tk_op = (u8)value; }
            }

            ///
            ///<summary>
            ///One of: TK_UNION TK_ALL TK_INTERSECT TK_EXCEPT 
            ///</summary>

            public char affinity;

            ///
            ///<summary>
            ///MakeRecord with this affinity for SelectResultType.Set 
            ///</summary>

            public SelectFlags selFlags;

            ///
            ///<summary>
            ///Various SF_* values 
            ///</summary>

            public SrcList pSrc;

            ///
            ///<summary>
            ///The FROM clause 
            ///</summary>

            public Expr pWhere;

            ///
            ///<summary>
            ///The WHERE clause 
            ///</summary>

            public ExprList pGroupBy;

            ///
            ///<summary>
            ///The GROUP BY clause 
            ///</summary>

            public Expr pHaving;

            ///
            ///<summary>
            ///The HAVING clause 
            ///</summary>

            public ExprList pOrderBy;

            ///
            ///<summary>
            ///The ORDER BY clause 
            ///</summary>

            public Select pPrior;

            ///
            ///<summary>
            ///Prior select in a compound select statement 
            ///</summary>

            public Select pNext;

            ///
            ///<summary>
            ///Next select to the left in a compound 
            ///</summary>

            public Select pRightmost;

            ///
            ///<summary>
            ///</summary>
            ///<param name="Right">most select in a compound select statement </param>

            public Expr pLimit;

            ///
            ///<summary>
            ///LIMIT expression. NULL means not used. 
            ///</summary>

            public Expr pOffset;

            ///
            ///<summary>
            ///OFFSET expression. NULL means not used. 
            ///</summary>

            public int iLimit;

            public int iOffset;

            ///
            ///<summary>
            ///Memory registers holding LIMIT & OFFSET counters 
            ///</summary>

            public int[] addrOpenEphm = new int[3];

            ///<summary>
            ///OP_OpenEphem opcodes related to this select
            ///</summary>
            public double nSelectRow;

            ///
            ///<summary>
            ///Estimated number of result rows 
            ///</summary>

            public Select Copy()
            {
                if (this == null)
                    return null;
                else
                {
                    Select cp = (Select)MemberwiseClone();
                    if (pEList != null)
                        cp.pEList = pEList.Copy();
                    if (pSrc != null)
                        cp.pSrc = pSrc.Copy();
                    if (pWhere != null)
                        cp.pWhere = pWhere.Copy();
                    if (pGroupBy != null)
                        cp.pGroupBy = pGroupBy.Copy();
                    if (pHaving != null)
                        cp.pHaving = pHaving.Copy();
                    if (pOrderBy != null)
                        cp.pOrderBy = pOrderBy.Copy();
                    if (pPrior != null)
                        cp.pPrior = pPrior.Copy();
                    if (pNext != null)
                        cp.pNext = pNext.Copy();
                    if (pRightmost != null)
                        cp.pRightmost = pRightmost.Copy();
                    if (pLimit != null)
                        cp.pLimit = pLimit.Copy();
                    if (pOffset != null)
                        cp.pOffset = pOffset.Copy();
                    return cp;
                }
            }

            public int sqlite3SelectExprHeight()
            {
                int nHeight = 0;
                this.heightOfSelect(ref nHeight);
                return nHeight;
            }


            ///<summary>
            /// This routine "expands" a SELECT statement and all of its subqueries.
            /// For additional information on what it means to "expand" a SELECT
            /// statement, see the comment on the selectExpand worker callback above.
            ///
            /// Expanding a SELECT statement is the first step in processing a
            /// SELECT statement.  The SELECT statement must be expanded before
            /// name resolution is performed.
            ///
            /// If anything goes wrong, an error message is written into pParse.
            /// The calling function can detect the problem by looking at pParse.nErr
            /// and/or pParse.db.mallocFailed.
            ///
            ///</summary>
            public static void sqlite3SelectExpand(Parse pParse, Select pSelect)
            {
                Walker w = new Walker();
                w.xSelectCallback = Select.selectExpander;
                w.xExprCallback = exprWalkNoop;
                w.pParse = pParse;
                w.sqlite3WalkSelect(pSelect);
            }

            ///<summary>
            /// This routine is a Walker callback for "expanding" a SELECT statement.
            /// "Expanding" means to do the following:
            ///
            ///    (1)  Make sure VDBE cursor numbers have been assigned to every
            ///         element of the FROM clause.
            ///
            ///    (2)  Fill in the pTabList.a[].pTab fields in the SrcList that
            ///         defines FROM clause.  When views appear in the FROM clause,
            ///         fill pTabList.a[].x.pSelect with a copy of the SELECT statement
            ///         that implements the view.  A copy is made of the view's SELECT
            ///         statement so that we can freely modify or delete that statement
            ///         without worrying about messing up the presistent representation
            ///         of the view.
            ///
            ///    (3)  Add terms to the WHERE clause to accomodate the NATURAL keyword
            ///         on joins and the ON and USING clause of joins.
            ///
            ///    (4)  Scan the list of columns in the result set (pEList) looking
            ///         for instances of the "*" operator or the TABLE.* operator.
            ///         If found, expand each "*" to be every column in every table
            ///         and TABLE.* to be every column in TABLE.
            ///
            ///
            ///</summary>
            public static int selectExpander(Walker pWalker, Select p)
            {
                Parse pParse = pWalker.pParse;
                int i, j, k;
                SrcList pTabList;
                ExprList pEList;
                SrcList_item pFrom;
                sqlite3 db = pParse.db;
                //if ( db.mallocFailed != 0 )
                //{
                //  return WRC_Abort;
                //}
                if (NEVER(p.pSrc == null) || (p.selFlags & SelectFlags.Expanded) != 0)
                {
                    return WRC_Prune;
                }
                p.selFlags |= SelectFlags.Expanded;
                pTabList = p.pSrc;
                pEList = p.pEList;
                ///
                ///<summary>
                ///Make sure cursor numbers have been assigned to all entries in
                ///the FROM clause of the SELECT statement.
                ///
                ///</summary>
                sqlite3SrcListAssignCursors(pParse, pTabList);
                ///
                ///<summary>
                ///Look up every table named in the FROM clause of the select.  If
                ///an entry of the FROM clause is a subquery instead of a table or view,
                ///then create a transient table ure to describe the subquery.
                ///
                ///</summary>
                for (i = 0; i < pTabList.nSrc; i++)// pFrom++ )
                {
                    pFrom = pTabList.a[i];
                    Table pTab;
                    if (pFrom.pTab != null)
                    {
                        ///
                        ///<summary>
                        ///This statement has already been prepared.  There is no need
                        ///to go further. 
                        ///</summary>
                        Debug.Assert(i == 0);
                        return WRC_Prune;
                    }
                    if (pFrom.zName == null)
                    {
#if !SQLITE_OMIT_SUBQUERY
                        Select pSel = pFrom.pSelect;
                        ///
                        ///<summary>
                        ///</summary>
                        ///<param name="A sub">query in the FROM clause of a SELECT </param>
                        Debug.Assert(pSel != null);
                        Debug.Assert(pFrom.pTab == null);
                        pWalker.sqlite3WalkSelect(pSel);
                        pFrom.pTab = pTab = new Table();
                        // sqlite3DbMallocZero( db, sizeof( Table ) );
                        if (pTab == null)
                            return WRC_Abort;
                        pTab.nRef = 1;
                        pTab.zName = sqlite3MPrintf(db, "sqlite_subquery_%p_", pTab);
                        while (pSel.pPrior != null)
                        {
                            pSel = pSel.pPrior;
                        }
                        selectColumnsFromExprList(pParse, pSel.pEList, ref pTab.nCol, ref pTab.aCol);
                        pTab.iPKey = -1;
                        pTab.nRowEst = 1000000;
                        pTab.tabFlags |= TF_Ephemeral;
#endif
                    }
                    else
                    {
                        ///
                        ///<summary>
                        ///An ordinary table or view name in the FROM clause 
                        ///</summary>
                        Debug.Assert(pFrom.pTab == null);
                        pFrom.pTab = pTab = sqlite3LocateTable(pParse, 0, pFrom.zName, pFrom.zDatabase);
                        if (pTab == null)
                            return WRC_Abort;
                        pTab.nRef++;
#if !(SQLITE_OMIT_VIEW) || !(SQLITE_OMIT_VIRTUALTABLE)
                        if (pTab.pSelect != null || IsVirtual(pTab))
                        {
                            ///
                            ///<summary>
                            ///We reach here if the named table is a really a view 
                            ///</summary>
                            if (sqlite3ViewGetColumnNames(pParse, pTab) != 0)
                                return WRC_Abort;
                            pFrom.pSelect = sqlite3SelectDup(db, pTab.pSelect, 0);
                            pWalker.sqlite3WalkSelect(pFrom.pSelect);
                        }
#endif
                    }
                    ///
                    ///<summary>
                    ///Locate the index named by the INDEXED BY clause, if any. 
                    ///</summary>
                    if (sqlite3IndexedByLookup(pParse, pFrom) != 0)
                    {
                        return WRC_Abort;
                    }
                }
                ///
                ///<summary>
                ///Process NATURAL keywords, and ON and USING clauses of joins.
                ///
                ///</summary>
                if (///
                    ///<summary>
                    ///db.mallocFailed != 0 || 
                    ///</summary>
                sqliteProcessJoin(pParse, p) != 0)
                {
                    return WRC_Abort;
                }
                ///
                ///<summary>
                ///For every "*" that occurs in the column list, insert the names of
                ///all columns in all tables.  And for every TABLE.* insert the names
                ///of all columns in TABLE.  The parser inserted a special expression
                ///with the TK_ALL operator for each "*" that it found in the column list.
                ///The following code just has to locate the TK_ALL expressions and expand
                ///each one to the list of all columns in all tables.
                ///
                ///The first loop just checks to see if there are any "*" operators
                ///that need expanding.
                ///
                ///</summary>
                for (k = 0; k < pEList.nExpr; k++)
                {
                    Expr pE = pEList.a[k].pExpr;
                    if (pE.op == TK_ALL)
                        break;
                    Debug.Assert(pE.op != TK_DOT || pE.pRight != null);
                    Debug.Assert(pE.op != TK_DOT || (pE.pLeft != null && pE.pLeft.op == TK_ID));
                    if (pE.op == TK_DOT && pE.pRight.op == TK_ALL)
                        break;
                }
                if (k < pEList.nExpr)
                {
                    ///
                    ///<summary>
                    ///If we get here it means the result set contains one or more "*"
                    ///operators that need to be expanded.  Loop through each expression
                    ///in the result set and expand them one by one.
                    ///
                    ///</summary>
                    ExprList_item[] a = pEList.a;
                    ExprList pNew = null;
                    int flags = pParse.db.flags;
                    bool longNames = (flags & SQLITE_FullColNames) != 0 && (flags & SQLITE_ShortColNames) == 0;
                    for (k = 0; k < pEList.nExpr; k++)
                    {
                        Expr pE = a[k].pExpr;
                        Debug.Assert(pE.op != TK_DOT || pE.pRight != null);
                        if (pE.op != TK_ALL && (pE.op != TK_DOT || pE.pRight.op != TK_ALL))
                        {
                            ///
                            ///<summary>
                            ///This particular expression does not need to be expanded.
                            ///
                            ///</summary>
                            pNew = pParse.sqlite3ExprListAppend(pNew, a[k].pExpr);
                            if (pNew != null)
                            {
                                pNew.a[pNew.nExpr - 1].zName = a[k].zName;
                                pNew.a[pNew.nExpr - 1].zSpan = a[k].zSpan;
                                a[k].zName = null;
                                a[k].zSpan = null;
                            }
                            a[k].pExpr = null;
                        }
                        else
                        {
                            ///
                            ///<summary>
                            ///This expression is a "*" or a "TABLE.*" and needs to be
                            ///expanded. 
                            ///</summary>
                            int tableSeen = 0;
                            ///
                            ///<summary>
                            ///Set to 1 when TABLE matches 
                            ///</summary>
                            string zTName;
                            ///
                            ///<summary>
                            ///text of name of TABLE 
                            ///</summary>
                            if (pE.op == TK_DOT)
                            {
                                Debug.Assert(pE.pLeft != null);
                                Debug.Assert(!pE.pLeft.ExprHasProperty(EP_IntValue));
                                zTName = pE.pLeft.u.zToken;
                            }
                            else
                            {
                                zTName = null;
                            }
                            for (i = 0; i < pTabList.nSrc; i++)//, pFrom++ )
                            {
                                pFrom = pTabList.a[i];
                                Table pTab = pFrom.pTab;
                                string zTabName = pFrom.zAlias;
                                if (zTabName == null)
                                {
                                    zTabName = pTab.zName;
                                }
                                ///if ( db.mallocFailed != 0 ) break;
                                if (zTName != null && !zTName.Equals(zTabName, StringComparison.InvariantCultureIgnoreCase))
                                {
                                    continue;
                                }
                                tableSeen = 1;
                                for (j = 0; j < pTab.nCol; j++)
                                {
                                    Expr pExpr, pRight;
                                    string zName = pTab.aCol[j].zName;
                                    string zColname;
                                    ///
                                    ///<summary>
                                    ///The computed column name 
                                    ///</summary>
                                    string zToFree;
                                    ///
                                    ///<summary>
                                    ///Malloced string that needs to be freed 
                                    ///</summary>
                                    Token sColname = new Token();
                                    ///
                                    ///<summary>
                                    ///Computed column name as a token 
                                    ///</summary>
                                    ///
                                    ///<summary>
                                    ///If a column is marked as 'hidden' (currently only possible
                                    ///for virtual tables), do not include it in the expanded
                                    ///</summary>
                                    ///<param name="result">set list.</param>
                                    ///<param name=""></param>
                                    if (IsHiddenColumn(pTab.aCol[j]))
                                    {
                                        Debug.Assert(IsVirtual(pTab));
                                        continue;
                                    }
                                    if (i > 0 && (zTName == null || zTName.Length == 0))
                                    {
                                        int iDummy = 0;
                                        if ((pFrom.jointype & JT_NATURAL) != 0 && tableAndColumnIndex(pTabList, i, zName, ref iDummy, ref iDummy) != 0)
                                        {
                                            ///
                                            ///<summary>
                                            ///In a NATURAL join, omit the join columns from the
                                            ///table to the right of the join 
                                            ///</summary>
                                            continue;
                                        }
                                        if (sqlite3IdListIndex(pFrom.pUsing, zName) >= 0)
                                        {
                                            ///
                                            ///<summary>
                                            ///In a join with a USING clause, omit columns in the
                                            ///using clause from the table on the right. 
                                            ///</summary>
                                            continue;
                                        }
                                    }
                                    pRight = sqlite3Expr(db, TK_ID, zName);
                                    zColname = zName;
                                    zToFree = "";
                                    if (longNames || pTabList.nSrc > 1)
                                    {
                                        Expr pLeft;
                                        pLeft = sqlite3Expr(db, TK_ID, zTabName);
                                        pExpr = pParse.sqlite3PExpr(TK_DOT, pLeft, pRight, 0);
                                        if (longNames)
                                        {
                                            zColname = sqlite3MPrintf(db, "%s.%s", zTabName, zName);
                                            zToFree = zColname;
                                        }
                                    }
                                    else
                                    {
                                        pExpr = pRight;
                                    }
                                    pNew = pParse.sqlite3ExprListAppend(pNew, pExpr);
                                    sColname.zRestSql = zColname;
                                    sColname.Length = StringExtensions.sqlite3Strlen30(zColname);
                                    pParse.sqlite3ExprListSetName(pNew, sColname, 0);
                                    db.sqlite3DbFree(ref zToFree);
                                }
                            }
                            if (tableSeen == 0)
                            {
                                if (zTName != null)
                                {
                                    sqlite3ErrorMsg(pParse, "no such table: %s", zTName);
                                }
                                else
                                {
                                    sqlite3ErrorMsg(pParse, "no tables specified");
                                }
                            }
                        }
                    }
                    sqlite3ExprListDelete(db, ref pEList);
                    p.pEList = pNew;
                }
                //#if SQLITE_MAX_COLUMN
                if (p.pEList != null && p.pEList.nExpr > db.aLimit[SQLITE_LIMIT_COLUMN])
                {
                    sqlite3ErrorMsg(pParse, "too many columns in result set");
                }
                //#endif
                return WRC_Continue;
            }





            public static void sqlite3SelectPrep(Parse pParse,///
                ///<summary>
                ///The parser context 
                ///</summary>
        Select p,///
                ///<summary>
                ///The SELECT statement being coded. 
                ///</summary>
        NameContext pOuterNC///
                ///<summary>
                ///Name context for container 
                ///</summary>
        )
            {
                sqlite3 db;
                if (NEVER(p == null))
                    return;
                db = pParse.db;
                if ((p.selFlags & SelectFlags.HasTypeInfo) != 0)
                    return;
                Select.sqlite3SelectExpand(pParse, p);
                if (pParse.nErr != 0///
                    ///<summary>
                    ///|| db.mallocFailed != 0 
                    ///</summary>
                )
                    return;
                sqlite3ResolveSelectNames(pParse, p, pOuterNC);
                if (pParse.nErr != 0///
                    ///<summary>
                    ///|| db.mallocFailed != 0 
                    ///</summary>
                )
                    return;
                sqlite3SelectAddTypeInfo(pParse, p);
            }







		
        }

        ///<summary>
        /// Allowed values for Select.selFlags.  The "SF" prefix stands for
        /// "Select Flag".
        ///
        ///</summary>
        //#define SF_Distinct        0x0001  /* Output should be DISTINCT */
        //#define SF_Resolved        0x0002  /* Identifiers have been resolved */
        //#define SF_Aggregate       0x0004  /* Contains aggregate functions */
        //#define SF_UsesEphemeral   0x0008  /* Uses the OpenEphemeral opcode */
        //#define SF_Expanded        0x0010  /* sqlite3SelectExpand() called on this */
        //#define SF_HasTypeInfo     0x0020  /* FROM subqueries have Table metadata */
        public enum SelectFlags : ushort
        {
            Distinct = 0x0001,
            ///
            ///<summary>
            ///Output should be DISTINCT 
            ///</summary>

            Resolved = 0x0002,
            ///
            ///<summary>
            ///Identifiers have been resolved 
            ///</summary>

            Aggregate = 0x0004,
            ///
            ///<summary>
            ///Contains aggregate functions 
            ///</summary>

            UsesEphemeral = 0x0008,
            ///
            ///<summary>
            ///Uses the OpenEphemeral opcode 
            ///</summary>

            Expanded = 0x0010,
            ///
            ///<summary>
            ///sqlite3SelectExpand() called on this 
            ///</summary>

            HasTypeInfo = 0x0020
            ///
            ///<summary>
            ///FROM subqueries have Table metadata 
            ///</summary>

        }

        ///
        ///<summary>
        ///The results of a select can be distributed in several ways.  The
        ///"SRT" prefix means "SELECT Result Type".
        ///
        ///</summary>

        public enum SelectResultType
        {
            Union = 1,
            //#define SelectResultType.Union        1  /* Store result as keys in an index */
            Except = 2,
            //#define SelectResultType.Except      2  /* Remove result from a UNION index */
            Exists = 3,
            //#define SelectResultType.Exists      3  /* Store 1 if the result is not empty */
            Discard = 4,
            //#define SelectResultType.Discard    4  /* Do not save the results anywhere */
            ///
            ///<summary>
            ///The ORDER BY clause is ignored for all of the above 
            ///</summary>

            //#define IgnorableOrderby(X) ((X->eDest)<=SelectResultType.Discard)
            Output = 5,
            //#define SelectResultType.Output      5  /* Output each row of result */
            Mem = 6,
            //#define SelectResultType.Mem            6  /* Store result in a memory cell */
            Set = 7,
            //#define SelectResultType.Set            7  /* Store results as keys in an index */
            Table = 8,
            //#define SelectResultType.Table        8  /* Store result as data with an automatic rowid */
            EphemTab = 9,
            //#define SelectResultType.EphemTab  9  /* Create transient tab and store like SelectResultType.Table /
            Coroutine = 10
            //#define SelectResultType.Coroutine   10  /* Generate a single row of result */
        }

        ///<summary>
        /// A structure used to customize the behavior of sqlite3Select(). See
        /// comments above sqlite3Select() for details.
        ///
        ///</summary>
        //typedef struct SelectDest SelectDest;
        public class SelectDest
        {
            public SelectResultType eDest;

            ///
            ///<summary>
            ///How to dispose of the results 
            ///</summary>

            public char affinity;

            ///
            ///<summary>
            ///Affinity used when eDest==SelectResultType.Set 
            ///</summary>

            public int iParm;

            ///
            ///<summary>
            ///A parameter used by the eDest disposal method 
            ///</summary>

            int _iMem;

            public int iMem
            {
                get { return _iMem; }
                set { _iMem = value; }
            }

            ///
            ///<summary>
            ///Base register where results are written 
            ///</summary>

            int _nMem;

            public int nMem
            {
                get { return _nMem; }
                set { _nMem = value; }
            }

            ///
            ///<summary>
            ///Number of registers allocated 
            ///</summary>

            public SelectDest()
            {
                this.eDest = 0;
                this.affinity = '\0';
                this.iParm = 0;
                this.iMem = 0;
                this.nMem = 0;
            }

            public SelectDest(SelectResultType eDest, char affinity, int iParm)
            {
                this.eDest = eDest;
                this.affinity = affinity;
                this.iParm = iParm;
                this.iMem = 0;
                this.nMem = 0;
            }

            public SelectDest(SelectResultType eDest, char affinity, int iParm, int iMem, int nMem)
            {
                this.eDest = eDest;
                this.affinity = affinity;
                this.iParm = iParm;
                this.iMem = iMem;
                this.nMem = nMem;
            }
        };

        ///<summary>
        /// The following structure describes the FROM clause of a SELECT statement.
        /// Each table or subquery in the FROM clause is a separate element of
        /// the SrcList.a[] array.
        ///
        /// With the addition of multiple database support, the following structure
        /// can also be used to describe a particular table such as the table that
        /// is modified by an INSERT, DELETE, or UPDATE statement.  In standard SQL,
        /// such a table must be a simple name: ID.  But in SQLite, the table can
        /// now be identified by a database name, a dot, then the table name: ID.ID.
        ///
        /// The jointype starts out showing the join type between the current table
        /// and the next table on the list.  The parser builds the list this way.
        /// But sqlite3SrcListShiftJoinType() later shifts the jointypes so that each
        /// jointype expresses the join between the table and the previous table.
        ///
        /// In the colUsed field, the high-order bit (bit 63) is set if the table
        /// contains more than 63 columns and the 64-th or later column is used.
        ///
        ///</summary>
        public class SrcList_item
        {
            public string zDatabase;
            ///
            ///<summary>
            ///Name of database holding this table 
            ///</summary>
            public string zName;
            ///
            ///<summary>
            ///Name of the table 
            ///</summary>
            public string zAlias;
            ///
            ///<summary>
            ///The "B" part of a "A AS B" phrase.  zName is the "A" 
            ///</summary>
            public Table pTab;
            ///
            ///<summary>
            ///An SQL table corresponding to zName 
            ///</summary>
            public Select pSelect;
            ///
            ///<summary>
            ///A SELECT statement used in place of a table name 
            ///</summary>
            public u8 isPopulated;
            ///
            ///<summary>
            ///Temporary table associated with SELECT is populated 
            ///</summary>
            public u8 jointype;
            ///
            ///<summary>
            ///Type of join between this able and the previous 
            ///</summary>
            public u8 notIndexed;
            ///
            ///<summary>
            ///True if there is a NOT INDEXED clause 
            ///</summary>
#if !SQLITE_OMIT_EXPLAIN
            public u8 iSelectId;
            ///
            ///<summary>
            ///</summary>
            ///<param name="If pSelect!=0, the id of the sub">select in EQP </param>
#endif
            public int iCursor;
            ///
            ///<summary>
            ///The VDBE cursor number used to access this table 
            ///</summary>
            public Expr pOn;
            ///
            ///<summary>
            ///The ON clause of a join 
            ///</summary>
            public IdList pUsing;
            ///
            ///<summary>
            ///The USING clause of a join 
            ///</summary>
            public Bitmask colUsed;
            ///
            ///<summary>
            ///Bit N (1<<N) set if column N of pTab is used 
            ///</summary>
            public string zIndex;
            ///
            ///<summary>
            ///Identifier from "INDEXED BY <zIndex>" clause 
            ///</summary>
            public Index pIndex;
            ///
            ///<summary>
            ///Index structure corresponding to zIndex, if any 
            ///</summary>
        }
        public class SrcList
        {
            public i16 nSrc;
            ///
            ///<summary>
            ///Number of tables or subqueries in the FROM clause 
            ///</summary>
            public i16 nAlloc;
            ///<summary>
            ///Number of entries allocated in a[] below
            ///</summary>
            public SrcList_item[] a;
            ///
            ///<summary>
            ///One entry for each identifier on the list 
            ///</summary>
            public SrcList Copy()
            {
                if (this == null)
                    return null;
                else
                {
                    SrcList cp = (SrcList)MemberwiseClone();
                    if (a != null)
                        a.CopyTo(cp.a, 0);
                    return cp;
                }
            }
            public void exprAnalyzeAll(///
                ///<summary>
                ///the FROM clause 
                ///</summary>
            WhereClause pWC///
                ///<summary>
                ///the WHERE clause to be analyzed 
                ///</summary>
            )
            {
                int i;
                for (i = pWC.nTerm - 1; i >= 0; i--)
                {
                    this.exprAnalyze(pWC, i);
                }
            }
            public void exprAnalyzeOrTerm(///
                ///<summary>
                ///the FROM clause 
                ///</summary>
            WhereClause pWC,///
                ///<summary>
                ///the complete WHERE clause 
                ///</summary>
            int idxTerm///
                ///<summary>
                ///</summary>
                ///<param name="Index of the OR">term to be analyzed </param>
            )
            {
                Parse pParse = pWC.pParse;
                ///
                ///<summary>
                ///Parser context 
                ///</summary>
                sqlite3 db = pParse.db;
                ///
                ///<summary>
                ///Data_base connection 
                ///</summary>
                WhereTerm pTerm = pWC.a[idxTerm];
                ///
                ///<summary>
                ///The term to be analyzed 
                ///</summary>
                Expr pExpr = pTerm.pExpr;
                ///
                ///<summary>
                ///The expression of the term 
                ///</summary>
                WhereMaskSet pMaskSet = pWC.pMaskSet;
                ///
                ///<summary>
                ///Table use masks 
                ///</summary>
                int i;
                ///
                ///<summary>
                ///Loop counters 
                ///</summary>
                WhereClause pOrWc;
                ///
                ///<summary>
                ///Breakup of pTerm into subterms 
                ///</summary>
                WhereTerm pOrTerm;
                ///
                ///<summary>
                ///</summary>
                ///<param name="A Sub">term within the pOrWc </param>
                WhereOrInfo pOrInfo;
                ///
                ///<summary>
                ///Additional information Debug.Associated with pTerm 
                ///</summary>
                Bitmask chngToIN;
                ///
                ///<summary>
                ///Tables that might satisfy case 1 
                ///</summary>
                Bitmask indexable;
                ///
                ///<summary>
                ///Tables that are indexable, satisfying case 2 
                ///</summary>
                ///
                ///<summary>
                ///Break the OR clause into its separate subterms.  The subterms are
                ///stored in a WhereClause structure containing within the WhereOrInfo
                ///object that is attached to the original OR clause term.
                ///
                ///</summary>
                Debug.Assert((pTerm.wtFlags & (TERM_DYNAMIC | TERM_ORINFO | TERM_ANDINFO)) == 0);
                Debug.Assert(pExpr.op == TK_OR);
                pTerm.u.pOrInfo = pOrInfo = new WhereOrInfo();
                //sqlite3DbMallocZero(db, sizeof(*pOrInfo));
                if (pOrInfo == null)
                    return;
                pTerm.wtFlags |= TERM_ORINFO;
                pOrWc = pOrInfo.wc;
                pOrWc.whereClauseInit(pWC.pParse, pMaskSet);
                pOrWc.whereSplit(pExpr, TokenType.TK_OR);
                this.exprAnalyzeAll(pOrWc);
                //      if ( db.mallocFailed != 0 ) return;
                Debug.Assert(pOrWc.nTerm >= 2);
                ///
                ///<summary>
                ///Compute the set of tables that might satisfy cases 1 or 2.
                ///
                ///</summary>
                indexable = ~(Bitmask)0;
                chngToIN = ~(pWC.vmask);
                for (i = pOrWc.nTerm - 1; i >= 0 && indexable != 0; i--)//, pOrTerm++ )
                {
                    pOrTerm = pOrWc.a[i];
                    if ((pOrTerm.eOperator & WO_SINGLE) == 0)
                    {
                        WhereAndInfo pAndInfo;
                        Debug.Assert(pOrTerm.eOperator == 0);
                        Debug.Assert((pOrTerm.wtFlags & (TERM_ANDINFO | TERM_ORINFO)) == 0);
                        chngToIN = 0;
                        pAndInfo = new WhereAndInfo();
                        //sqlite3DbMallocRaw(db, sizeof(*pAndInfo));
                        if (pAndInfo != null)
                        {
                            WhereClause pAndWC;
                            WhereTerm pAndTerm;
                            int j;
                            Bitmask b = 0;
                            pOrTerm.u.pAndInfo = pAndInfo;
                            pOrTerm.wtFlags |= TERM_ANDINFO;
                            pOrTerm.eOperator = WO_AND;
                            pAndWC = pAndInfo.wc;
                            pAndWC.whereClauseInit(pWC.pParse, pMaskSet);
                            pAndWC.whereSplit(pOrTerm.pExpr, TokenType.TK_AND);
                            this.exprAnalyzeAll(pAndWC);
                            //testcase( db.mallocFailed );
                            ////if ( 0 == db.mallocFailed )
                            {
                                for (j = 0; j < pAndWC.nTerm; j++)//, pAndTerm++ )
                                {
                                    pAndTerm = pAndWC.a[j];
                                    Debug.Assert(pAndTerm.pExpr != null);
                                    if (allowedOp(pAndTerm.pExpr.op))
                                    {
                                        b |= pMaskSet.getMask(pAndTerm.leftCursor);
                                    }
                                }
                            }
                            indexable &= b;
                        }
                    }
                    else
                        if ((pOrTerm.wtFlags & TERM_COPIED) != 0)
                        {
                            ///
                            ///<summary>
                            ///Skip this term for now.  We revisit it when we process the
                            ///corresponding TERM_VIRTUAL term 
                            ///</summary>
                        }
                        else
                        {
                            Bitmask b;
                            b = pMaskSet.getMask(pOrTerm.leftCursor);
                            if ((pOrTerm.wtFlags & TERM_VIRTUAL) != 0)
                            {
                                WhereTerm pOther = pOrWc.a[pOrTerm.iParent];
                                b |= pMaskSet.getMask(pOther.leftCursor);
                            }
                            indexable &= b;
                            if (pOrTerm.eOperator != WO_EQ)
                            {
                                chngToIN = 0;
                            }
                            else
                            {
                                chngToIN &= b;
                            }
                        }
                }
                ///
                ///<summary>
                ///Record the set of tables that satisfy case 2.  The set might be
                ///empty.
                ///
                ///</summary>
                pOrInfo.indexable = indexable;
                pTerm.eOperator = (u16)(indexable == 0 ? 0 : WO_OR);
                ///
                ///<summary>
                ///chngToIN holds a set of tables that *might* satisfy case 1.  But
                ///we have to do some additional checking to see if case 1 really
                ///is satisfied.
                ///
                ///</summary>
                ///<param name="chngToIN will hold either 0, 1, or 2 bits.  The 0">bit case means</param>
                ///<param name="that there is no possibility of transforming the OR clause into an">that there is no possibility of transforming the OR clause into an</param>
                ///<param name="IN operator because one or more terms in the OR clause contain">IN operator because one or more terms in the OR clause contain</param>
                ///<param name="something other than == on a column in the single table.  The 1">bit</param>
                ///<param name="case means that every term of the OR clause is of the form">case means that every term of the OR clause is of the form</param>
                ///<param name=""table.column=expr" for some single table.  The one bit that is set">"table.column=expr" for some single table.  The one bit that is set</param>
                ///<param name="will correspond to the common table.  We still need to check to make">will correspond to the common table.  We still need to check to make</param>
                ///<param name="sure the same column is used on all terms.  The 2">bit case is when</param>
                ///<param name="the all terms are of the form "table1.column=table2.column".  It">the all terms are of the form "table1.column=table2.column".  It</param>
                ///<param name="might be possible to form an IN operator with either table1.column">might be possible to form an IN operator with either table1.column</param>
                ///<param name="or table2.column as the LHS if either is common to every term of">or table2.column as the LHS if either is common to every term of</param>
                ///<param name="the OR clause.">the OR clause.</param>
                ///<param name=""></param>
                ///<param name="Note that terms of the form "table.column1=table.column2" (the">Note that terms of the form "table.column1=table.column2" (the</param>
                ///<param name="same table on both sizes of the ==) cannot be optimized.">same table on both sizes of the ==) cannot be optimized.</param>
                ///<param name=""></param>
                if (chngToIN != 0)
                {
                    int okToChngToIN = 0;
                    ///
                    ///<summary>
                    ///True if the conversion to IN is valid 
                    ///</summary>
                    int iColumn = -1;
                    ///
                    ///<summary>
                    ///Column index on lhs of IN operator 
                    ///</summary>
                    int iCursor = -1;
                    ///
                    ///<summary>
                    ///Table cursor common to all terms 
                    ///</summary>
                    int j = 0;
                    ///
                    ///<summary>
                    ///Loop counter 
                    ///</summary>
                    ///
                    ///<summary>
                    ///Search for a table and column that appears on one side or the
                    ///other of the == operator in every subterm.  That table and column
                    ///will be recorded in iCursor and iColumn.  There might not be any
                    ///such table and column.  Set okToChngToIN if an appropriate table
                    ///and column is found but leave okToChngToIN false if not found.
                    ///
                    ///</summary>
                    for (j = 0; j < 2 && 0 == okToChngToIN; j++)
                    {
                        //pOrTerm = pOrWc.a;
                        for (i = pOrWc.nTerm - 1; i >= 0; i--)//, pOrTerm++)
                        {
                            pOrTerm = pOrWc.a[pOrWc.nTerm - 1 - i];
                            Debug.Assert(pOrTerm.eOperator == WO_EQ);
                            pOrTerm.wtFlags = (u8)(pOrTerm.wtFlags & ~TERM_OR_OK);
                            if (pOrTerm.leftCursor == iCursor)
                            {
                                ///
                                ///<summary>
                                ///</summary>
                                ///<param name="This is the 2">bit case and we are on the second iteration and</param>
                                ///<param name="current term is from the first iteration.  So skip this term. ">current term is from the first iteration.  So skip this term. </param>
                                Debug.Assert(j == 1);
                                continue;
                            }
                            if ((chngToIN & pMaskSet.getMask(pOrTerm.leftCursor)) == 0)
                            {
                                ///
                                ///<summary>
                                ///This term must be of the form t1.a==t2.b where t2 is in the
                                ///chngToIN set but t1 is not.  This term will be either preceeded
                                ///or follwed by an inverted copy (t2.b==t1.a).  Skip this term
                                ///and use its inversion. 
                                ///</summary>
                                testcase(pOrTerm.wtFlags & TERM_COPIED);
                                testcase(pOrTerm.wtFlags & TERM_VIRTUAL);
                                Debug.Assert((pOrTerm.wtFlags & (TERM_COPIED | TERM_VIRTUAL)) != 0);
                                continue;
                            }
                            iColumn = pOrTerm.u.leftColumn;
                            iCursor = pOrTerm.leftCursor;
                            break;
                        }
                        if (i < 0)
                        {
                            ///
                            ///<summary>
                            ///No candidate table+column was found.  This can only occur
                            ///on the second iteration 
                            ///</summary>
                            Debug.Assert(j == 1);
                            Debug.Assert((chngToIN & (chngToIN - 1)) == 0);
                            Debug.Assert(chngToIN == pMaskSet.getMask(iCursor));
                            break;
                        }
                        testcase(j == 1);
                        ///
                        ///<summary>
                        ///We have found a candidate table and column.  Check to see if that
                        ///table and column is common to every term in the OR clause 
                        ///</summary>
                        okToChngToIN = 1;
                        for (; i >= 0 && okToChngToIN != 0; i--)//, pOrTerm++)
                        {
                            pOrTerm = pOrWc.a[pOrWc.nTerm - 1 - i];
                            Debug.Assert(pOrTerm.eOperator == WO_EQ);
                            if (pOrTerm.leftCursor != iCursor)
                            {
                                pOrTerm.wtFlags = (u8)(pOrTerm.wtFlags & ~TERM_OR_OK);
                            }
                            else
                                if (pOrTerm.u.leftColumn != iColumn)
                                {
                                    okToChngToIN = 0;
                                }
                                else
                                {
                                    int affLeft, affRight;
                                    ///
                                    ///<summary>
                                    ///</summary>
                                    ///<param name="If the right">hand side is also a column, then the affinities</param>
                                    ///<param name="of both right and left sides must be such that no type">of both right and left sides must be such that no type</param>
                                    ///<param name="conversions are required on the right.  (Ticket #2249)">conversions are required on the right.  (Ticket #2249)</param>
                                    ///<param name=""></param>
                                    affRight = pOrTerm.pExpr.pRight.sqlite3ExprAffinity();
                                    affLeft = pOrTerm.pExpr.pLeft.sqlite3ExprAffinity();
                                    if (affRight != 0 && affRight != affLeft)
                                    {
                                        okToChngToIN = 0;
                                    }
                                    else
                                    {
                                        pOrTerm.wtFlags |= TERM_OR_OK;
                                    }
                                }
                        }
                    }
                    ///
                    ///<summary>
                    ///At this point, okToChngToIN is true if original pTerm satisfies
                    ///case 1.  In that case, construct a new virtual term that is
                    ///pTerm converted into an IN operator.
                    ///
                    ///</summary>
                    ///<param name="EV: R">15100</param>
                    ///<param name=""></param>
                    if (okToChngToIN != 0)
                    {
                        Expr pDup;
                        ///
                        ///<summary>
                        ///A transient duplicate expression 
                        ///</summary>
                        ExprList pList = null;
                        ///
                        ///<summary>
                        ///The RHS of the IN operator 
                        ///</summary>
                        Expr pLeft = null;
                        ///
                        ///<summary>
                        ///The LHS of the IN operator 
                        ///</summary>
                        Expr pNew;
                        ///
                        ///<summary>
                        ///The complete IN operator 
                        ///</summary>
                        for (i = pOrWc.nTerm - 1; i >= 0; i--)//, pOrTerm++)
                        {
                            pOrTerm = pOrWc.a[pOrWc.nTerm - 1 - i];
                            if ((pOrTerm.wtFlags & TERM_OR_OK) == 0)
                                continue;
                            Debug.Assert(pOrTerm.eOperator == WO_EQ);
                            Debug.Assert(pOrTerm.leftCursor == iCursor);
                            Debug.Assert(pOrTerm.u.leftColumn == iColumn);
                            pDup = sqlite3ExprDup(db, pOrTerm.pExpr.pRight, 0);
                            pList = pWC.pParse.sqlite3ExprListAppend(pList, pDup);
                            pLeft = pOrTerm.pExpr.pLeft;
                        }
                        Debug.Assert(pLeft != null);
                        pDup = sqlite3ExprDup(db, pLeft, 0);
                        pNew = pParse.sqlite3PExpr(TK_IN, pDup, null, null);
                        if (pNew != null)
                        {
                            int idxNew;
                            pNew.transferJoinMarkings(pExpr);
                            Debug.Assert(!pNew.ExprHasProperty(EP_xIsSelect));
                            pNew.x.pList = pList;
                            idxNew = pWC.whereClauseInsert(pNew, TERM_VIRTUAL | TERM_DYNAMIC);
                            testcase(idxNew == 0);
                            this.exprAnalyze(pWC, idxNew);
                            pTerm = pWC.a[idxTerm];
                            pWC.a[idxNew].iParent = idxTerm;
                            pTerm.nChild = 1;
                        }
                        else
                        {
                            sqlite3ExprListDelete(db, ref pList);
                        }
                        pTerm.eOperator = WO_NOOP;
                        ///
                        ///<summary>
                        ///case 1 trumps case 2 
                        ///</summary>
                    }
                }
            }
            public void exprAnalyze(///
                ///<summary>
                ///the FROM clause 
                ///</summary>
            WhereClause pWC,///
                ///<summary>
                ///the WHERE clause 
                ///</summary>
            int idxTerm///
                ///<summary>
                ///Index of the term to be analyzed 
                ///</summary>
            )
            {
                WhereTerm pTerm;
                ///
                ///<summary>
                ///The term to be analyzed 
                ///</summary>
                WhereMaskSet pMaskSet;
                ///
                ///<summary>
                ///Set of table index masks 
                ///</summary>
                Expr pExpr;
                ///
                ///<summary>
                ///The expression to be analyzed 
                ///</summary>
                Bitmask prereqLeft;
                ///
                ///<summary>
                ///Prerequesites of the pExpr.pLeft 
                ///</summary>
                Bitmask prereqAll;
                ///
                ///<summary>
                ///Prerequesites of pExpr 
                ///</summary>
                Bitmask extraRight = 0;
                ///
                ///<summary>
                ///Extra dependencies on LEFT JOIN 
                ///</summary>
                Expr pStr1 = null;
                ///
                ///<summary>
                ///RHS of LIKE/GLOB operator 
                ///</summary>
                bool isComplete = false;
                ///
                ///<summary>
                ///RHS of LIKE/GLOB ends with wildcard 
                ///</summary>
                bool noCase = false;
                ///
                ///<summary>
                ///LIKE/GLOB distinguishes case 
                ///</summary>
                int op;
                ///
                ///<summary>
                ///</summary>
                ///<param name="Top">level operator.  pExpr.op </param>
                Parse pParse = pWC.pParse;
                ///
                ///<summary>
                ///Parsing context 
                ///</summary>
                sqlite3 db = pParse.db;
                ///
                ///<summary>
                ///Data_base connection 
                ///</summary>
                //if ( db.mallocFailed != 0 )
                //{
                //  return;
                //}
                pTerm = pWC.a[idxTerm];
                pMaskSet = pWC.pMaskSet;
                pExpr = pTerm.pExpr;
                prereqLeft = pMaskSet.exprTableUsage(pExpr.pLeft);
                op = pExpr.op;
                if (op == TK_IN)
                {
                    Debug.Assert(pExpr.pRight == null);
                    if (pExpr.ExprHasProperty(EP_xIsSelect))
                    {
                        pTerm.prereqRight = pMaskSet.exprSelectTableUsage(pExpr.x.pSelect);
                    }
                    else
                    {
                        pTerm.prereqRight = pMaskSet.exprListTableUsage(pExpr.x.pList);
                    }
                }
                else
                    if (op == TK_ISNULL)
                    {
                        pTerm.prereqRight = 0;
                    }
                    else
                    {
                        pTerm.prereqRight = pMaskSet.exprTableUsage(pExpr.pRight);
                    }
                prereqAll = pMaskSet.exprTableUsage(pExpr);
                if (pExpr.ExprHasProperty(EP_FromJoin))
                {
                    Bitmask x = pMaskSet.getMask(pExpr.iRightJoinTable);
                    prereqAll |= x;
                    extraRight = x - 1;
                    ///
                    ///<summary>
                    ///ON clause terms may not be used with an index
                    ///on left table of a LEFT JOIN.  Ticket #3015 
                    ///</summary>
                }
                pTerm.prereqAll = prereqAll;
                pTerm.leftCursor = -1;
                pTerm.iParent = -1;
                pTerm.eOperator = 0;
                if (allowedOp(op) && (pTerm.prereqRight & prereqLeft) == 0)
                {
                    Expr pLeft = pExpr.pLeft;
                    Expr pRight = pExpr.pRight;
                    if (pLeft.op == TK_COLUMN)
                    {
                        pTerm.leftCursor = pLeft.iTable;
                        pTerm.u.leftColumn = pLeft.iColumn;
                        pTerm.eOperator = operatorMask(op);
                    }
                    if (pRight != null && pRight.op == TK_COLUMN)
                    {
                        WhereTerm pNew;
                        Expr pDup;
                        if (pTerm.leftCursor >= 0)
                        {
                            int idxNew;
                            pDup = sqlite3ExprDup(db, pExpr, 0);
                            //if ( db.mallocFailed != 0 )
                            //{
                            //  sqlite3ExprDelete( db, ref pDup );
                            //  return;
                            //}
                            idxNew = pWC.whereClauseInsert(pDup, TERM_VIRTUAL | TERM_DYNAMIC);
                            if (idxNew == 0)
                                return;
                            pNew = pWC.a[idxNew];
                            pNew.iParent = idxTerm;
                            pTerm = pWC.a[idxTerm];
                            pTerm.nChild = 1;
                            pTerm.wtFlags |= TERM_COPIED;
                        }
                        else
                        {
                            pDup = pExpr;
                            pNew = pTerm;
                        }
                        pParse.exprCommute(pDup);
                        pLeft = pDup.pLeft;
                        pNew.leftCursor = pLeft.iTable;
                        pNew.u.leftColumn = pLeft.iColumn;
                        testcase((prereqLeft | extraRight) != prereqLeft);
                        pNew.prereqRight = prereqLeft | extraRight;
                        pNew.prereqAll = prereqAll;
                        pNew.eOperator = operatorMask(pDup.op);
                    }
                }
#if !SQLITE_OMIT_BETWEEN_OPTIMIZATION
                ///
                ///<summary>
                ///If a term is the BETWEEN operator, create two new virtual terms
                ///that define the range that the BETWEEN implements.  For example:
                ///
                ///a BETWEEN b AND c
                ///
                ///is converted into:
                ///
                ///(a BETWEEN b AND c) AND (a>=b) AND (a<=c)
                ///
                ///The two new terms are added onto the end of the WhereClause object.
                ///The new terms are "dynamic" and are children of the original BETWEEN
                ///term.  That means that if the BETWEEN term is coded, the children are
                ///skipped.  Or, if the children are satisfied by an index, the original
                ///BETWEEN term is skipped.
                ///</summary>
                else
                    if (pExpr.Operator == TokenType.TK_BETWEEN && pWC.Operator == TokenType.TK_AND)
                    {
                        ExprList pList = pExpr.x.pList;
                        int i;
                        u8[] ops = new u8[] {
							TK_GE,
							TK_LE
						};
                        Debug.Assert(pList != null);
                        Debug.Assert(pList.nExpr == 2);
                        for (i = 0; i < 2; i++)
                        {
                            Expr pNewExpr;
                            int idxNew;
                            pNewExpr = pParse.sqlite3PExpr(ops[i], sqlite3ExprDup(db, pExpr.pLeft, 0), sqlite3ExprDup(db, pList.a[i].pExpr, 0), null);
                            idxNew = pWC.whereClauseInsert(pNewExpr, TERM_VIRTUAL | TERM_DYNAMIC);
                            testcase(idxNew == 0);
                            this.exprAnalyze(pWC, idxNew);
                            pTerm = pWC.a[idxTerm];
                            pWC.a[idxNew].iParent = idxTerm;
                        }
                        pTerm.nChild = 2;
                    }
#endif
#if !(SQLITE_OMIT_OR_OPTIMIZATION) && !(SQLITE_OMIT_SUBQUERY)
                    ///
                    ///<summary>
                    ///Analyze a term that is composed of two or more subterms connected by
                    ///an OR operator.
                    ///</summary>
                    else
                        if (pExpr.Operator == TokenType.TK_OR)
                        {
                            Debug.Assert(pWC.op == TK_AND);
                            this.exprAnalyzeOrTerm(pWC, idxTerm);
                            pTerm = pWC.a[idxTerm];
                        }
#endif
#if !SQLITE_OMIT_LIKE_OPTIMIZATION
                ///
                ///<summary>
                ///Add constraints to reduce the search space on a LIKE or GLOB
                ///operator.
                ///
                ///A like pattern of the form "x LIKE 'abc%'" is changed into constraints
                ///
                ///x>='abc' AND x<'abd' AND x LIKE 'abc%'
                ///
                ///The last character of the prefix "abc" is incremented to form the
                ///termination condition "abd".
                ///</summary>
                if (pWC.Operator == TokenType.TK_AND && pParse.isLikeOrGlob(pExpr, ref pStr1, ref isComplete, ref noCase) != 0)
                {
                    Expr pLeft;
                    ///
                    ///<summary>
                    ///LHS of LIKE/GLOB operator 
                    ///</summary>
                    Expr pStr2;
                    ///
                    ///<summary>
                    ///</summary>
                    ///<param name="Copy of pStr1 "> RHS of LIKE/GLOB operator </param>
                    Expr pNewExpr1;
                    Expr pNewExpr2;
                    int idxNew1;
                    int idxNew2;
                    CollSeq pColl;
                    ///
                    ///<summary>
                    ///Collating sequence to use 
                    ///</summary>
                    pLeft = pExpr.x.pList.a[1].pExpr;
                    pStr2 = sqlite3ExprDup(db, pStr1, 0);
                    ////if ( 0 == db.mallocFailed )
                    {
                        int c, pC;
                        ///
                        ///<summary>
                        ///Last character before the first wildcard 
                        ///</summary>
                        pC = pStr2.u.zToken[StringExtensions.sqlite3Strlen30(pStr2.u.zToken) - 1];
                        c = pC;
                        if (noCase)
                        {
                            ///
                            ///<summary>
                            ///The point is to increment the last character before the first
                            ///wildcard.  But if we increment '@', that will push it into the
                            ///alphabetic range where case conversions will mess up the
                            ///inequality.  To avoid this, make sure to also run the full
                            ///LIKE on all candidate expressions by clearing the isComplete flag
                            ///
                            ///</summary>
                            if (c == 'A' - 1)
                                isComplete = false;
                            ///
                            ///<summary>
                            ///</summary>
                            ///<param name="EV: R">08207 </param>
                            c = sqlite3UpperToLower[c];
                        }
                        pStr2.u.zToken = pStr2.u.zToken.Substring(0, StringExtensions.sqlite3Strlen30(pStr2.u.zToken) - 1) + (char)(c + 1);
                        // pC = c + 1;
                    }
                    pColl = sqlite3FindCollSeq(db, SqliteEncoding.UTF8, noCase ? "NOCASE" : "BINARY", 0);
                    pNewExpr1 = pParse.sqlite3PExpr(TK_GE, sqlite3ExprDup(db, pLeft, 0).sqlite3ExprSetColl(pColl), pStr1, 0);
                    idxNew1 = pWC.whereClauseInsert(pNewExpr1, TERM_VIRTUAL | TERM_DYNAMIC);
                    testcase(idxNew1 == 0);
                    this.exprAnalyze(pWC, idxNew1);
                    pNewExpr2 = pParse.sqlite3PExpr(TK_LT, sqlite3ExprDup(db, pLeft, 0).sqlite3ExprSetColl(pColl), pStr2, null);
                    idxNew2 = pWC.whereClauseInsert(pNewExpr2, TERM_VIRTUAL | TERM_DYNAMIC);
                    testcase(idxNew2 == 0);
                    this.exprAnalyze(pWC, idxNew2);
                    pTerm = pWC.a[idxTerm];
                    if (isComplete)
                    {
                        pWC.a[idxNew1].iParent = idxTerm;
                        pWC.a[idxNew2].iParent = idxTerm;
                        pTerm.nChild = 2;
                    }
                }
#endif
#if !SQLITE_OMIT_VIRTUALTABLE
                ///
                ///<summary>
                ///Add a WO_MATCH auxiliary term to the constraint set if the
                ///current expression is of the form:  column MATCH expr.
                ///This information is used by the xBestIndex methods of
                ///virtual tables.  The native query optimizer does not attempt
                ///to do anything with MATCH functions.
                ///</summary>
                if (pExpr.isMatchOfColumn() != 0)
                {
                    int idxNew;
                    Expr pRight, pLeft;
                    WhereTerm pNewTerm;
                    Bitmask prereqColumn, prereqExpr;
                    pRight = pExpr.x.pList.a[0].pExpr;
                    pLeft = pExpr.x.pList.a[1].pExpr;
                    prereqExpr = pMaskSet.exprTableUsage(pRight);
                    prereqColumn = pMaskSet.exprTableUsage(pLeft);
                    if ((prereqExpr & prereqColumn) == 0)
                    {
                        Expr pNewExpr;
                        pNewExpr = pParse.sqlite3PExpr(TK_MATCH, null, sqlite3ExprDup(db, pRight, 0), null);
                        idxNew = pWC.whereClauseInsert(pNewExpr, TERM_VIRTUAL | TERM_DYNAMIC);
                        testcase(idxNew == 0);
                        pNewTerm = pWC.a[idxNew];
                        pNewTerm.prereqRight = prereqExpr;
                        pNewTerm.leftCursor = pLeft.iTable;
                        pNewTerm.u.leftColumn = pLeft.iColumn;
                        pNewTerm.eOperator = WO_MATCH;
                        pNewTerm.iParent = idxTerm;
                        pTerm = pWC.a[idxTerm];
                        pTerm.nChild = 1;
                        pTerm.wtFlags |= TERM_COPIED;
                        pNewTerm.prereqAll = pTerm.prereqAll;
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
                ///
                ///<summary>
                ///Prevent ON clause terms of a LEFT JOIN from being used to drive
                ///an index for tables to the left of the join.
                ///</summary>
                pTerm.prereqRight |= extraRight;
            }
        }


        ///<summary>
        /// Permitted values of the SrcList.a.jointype field
        ///
        ///</summary>
        private const int JT_INNER = 0x0001;
        //#define JT_INNER     0x0001    /* Any kind of inner or cross join */
        private const int JT_CROSS = 0x0002;
        //#define JT_CROSS     0x0002    /* Explicit use of the CROSS keyword */
        private const int JT_NATURAL = 0x0004;
        //#define JT_NATURAL   0x0004    /* True for a "natural" join */
        private const int JT_LEFT = 0x0008;
        //#define JT_LEFT      0x0008    /* Left outer join */
        private const int JT_RIGHT = 0x0010;
        //#define JT_RIGHT     0x0010    /* Right outer join */
        private const int JT_OUTER = 0x0020;
        //#define JT_OUTER     0x0020    /* The "OUTER" keyword is present */
        private const int JT_ERROR = 0x0040;
        //#define JT_ERROR     0x0040    /* unknown or unsupported join type */









        ///<summary>
        /// Given 1 to 3 identifiers preceeding the JOIN keyword, determine the
        /// type of join.  Return an integer constant that expresses that type
        /// in terms of the following bit values:
        ///
        ///     JT_INNER
        ///     JT_CROSS
        ///     JT_OUTER
        ///     JT_NATURAL
        ///     JT_LEFT
        ///     JT_RIGHT
        ///
        /// A full outer join is the combination of JT_LEFT and JT_RIGHT.
        ///
        /// If an illegal or unsupported join type is seen, then still return
        /// a join type, but put an error in the pParse structure.
        ///
        ///</summary>
        class Keyword
        {
            public u8 i;
            ///
            ///<summary>
            ///Beginning of keyword text in zKeyText[] 
            ///</summary>
            public u8 nChar;
            ///
            ///<summary>
            ///Length of the keyword in characters 
            ///</summary>
            public u8 code;
            ///
            ///<summary>
            ///Join type mask 
            ///</summary>
            public Keyword(u8 i, u8 nChar, u8 code)
            {
                this.i = i;
                this.nChar = nChar;
                this.code = code;
            }
        }
    }
}
