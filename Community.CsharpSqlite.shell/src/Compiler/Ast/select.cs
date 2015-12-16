﻿using System;
using System.Diagnostics;
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

namespace Community.CsharpSqlite.Ast
{
    using Metadata;
    using Vdbe = Engine.Vdbe;
    using sqlite3_value = Engine.Mem;

    using Parse = Community.CsharpSqlite.Sqlite3.Parse;

    using ResolveExtensions = Sqlite3.ResolveExtensions;
    using Community.CsharpSqlite.Ast;
    using Community.CsharpSqlite.builder;
    using Community.CsharpSqlite.Os;
    using Community.CsharpSqlite.Engine;
    using Community.CsharpSqlite.Utils;
    using Compiler.Parser;
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
    ///<param name="the  P4Usage.P4_KEYINFO and P2 parameters later.  Neither the KeyInfo nor">the  P4Usage.P4_KEYINFO and P2 parameters later.  Neither the KeyInfo nor</param>
    ///<param name="the number of columns in P2 can be computed at the same time">the number of columns in P2 can be computed at the same time</param>
    ///<param name="as the OP_OpenEphm instruction is coded because not">as the OP_OpenEphm instruction is coded because not</param>
    ///<param name="enough information about the compound query is known at that point.">enough information about the compound query is known at that point.</param>
    ///<param name="The KeyInfo for addrOpenTran[0] and [1] contains collating sequences">The KeyInfo for addrOpenTran[0] and [1] contains collating sequences</param>
    ///<param name="for the result set.  The KeyInfo for addrOpenTran[2] contains collating">for the result set.  The KeyInfo for addrOpenTran[2] contains collating</param>
    ///<param name="sequences for the ORDER BY clause.">sequences for the ORDER BY clause.</param>
    ///<param name=""></param>

    public class Select
        {
            ///<summary>
            ///The fields of the result 
            ///</summary>
            public ExprList pEList;
            
            public u8 tk_op { get; set; }
            public TokenType TokenOp {
                get { return (TokenType)tk_op; }
                set { tk_op = (u8)value; }
            }

            ///
            ///<summary>
            ///One of: TokenType.TK_UNION TokenType.TK_ALL TokenType.TK_INTERSECT TokenType.TK_EXCEPT 
            ///</summary>

            public char affinity;

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
                        cp.pWhere = pWhere.Clone();
                    if (pGroupBy != null)
                        cp.pGroupBy = pGroupBy.Copy();
                    if (pHaving != null)
                        cp.pHaving = pHaving.Clone();
                    if (pOrderBy != null)
                        cp.pOrderBy = pOrderBy.Copy();
                    if (pPrior != null)
                        cp.pPrior = pPrior.Copy();
                    if (pNext != null)
                        cp.pNext = pNext.Copy();
                    if (pRightmost != null)
                        cp.pRightmost = pRightmost.Copy();
                    if (pLimit != null)
                        cp.pLimit = pLimit.Clone();
                    if (pOffset != null)
                        cp.pOffset = pOffset.Clone();
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
            public static void sqlite3SelectExpand(Sqlite3.Parse pParse, Select pSelect)
            {
                Walker w = new Walker();
                w.xSelectCallback = Select.selectExpander;
                w.xExprCallback = SelectMethods.exprWalkNoop;
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
            public static WRC selectExpander(Walker pWalker, Select p)
            {
                Parse pParse = pWalker.pParse;
                int i, j, k;
                SrcList pTabList;
                ExprList pEList;
                SrcList_item pFrom;
                Connection db = pParse.db;
                //if ( db.mallocFailed != 0 )
                //{
                //  return WRC.WRC_Abort;
                //}
                if (Sqlite3.NEVER(p.pSrc == null) || (p.selFlags & SelectFlags.Expanded) != 0)
                {
                    return WRC.WRC_Prune;
                }
                p.selFlags |= SelectFlags.Expanded;
                pTabList = p.pSrc;
                pEList = p.pEList;
                ///Make sure cursor numbers have been assigned to all entries in
                ///the FROM clause of the SELECT statement.
                build.sqlite3SrcListAssignCursors(pParse, pTabList);
                ///Look up every table named in the FROM clause of the select.  If
                ///an entry of the FROM clause is a subquery instead of a table or view,
                ///then create a transient table ure to describe the subquery.
                for (i = 0; i < pTabList.nSrc; i++)// pFrom++ )
                {
                    pFrom = pTabList.a[i];
                    Table pTab;
                    if (pFrom.pTab != null)
                    {
                        ///This statement has already been prepared.  There is no need
                        ///to go further. 
                        Debug.Assert(i == 0);
                        return WRC.WRC_Prune;
                    }
                    if (pFrom.zName == null)
                    {
#if !SQLITE_OMIT_SUBQUERY
                        Select pSel = pFrom.pSelect;
                        ///A sub-query in the FROM clause of a SELECT 
                        Debug.Assert(pSel != null);
                        Debug.Assert(pFrom.pTab == null);
                        pWalker.sqlite3WalkSelect(pSel);
                        pFrom.pTab = pTab = new Table();
                        // sqlite3DbMallocZero( db, sizeof( Table ) );
                        if (pTab == null)
                            return WRC.WRC_Abort;
                        pTab.nRef = 1;
                        pTab.zName = io.sqlite3MPrintf(db, "sqlite_subquery_%p_", pTab);
                        while (pSel.pPrior != null)
                        {
                            pSel = pSel.pPrior;
                        }
                        SelectMethods.selectColumnsFromExprList(pParse, pSel.pEList, ref pTab.nCol, ref pTab.aCol);
                        pTab.iPKey = -1;
                        pTab.nRowEst = 1000000;
                        pTab.tabFlags |= TableFlags.TF_Ephemeral;
#endif
                    }
                    else
                    {
                        ///
                        ///<summary>
                        ///An ordinary table or view name in the FROM clause 
                        ///</summary>
                        Debug.Assert(pFrom.pTab == null);
                        pFrom.pTab = pTab = TableBuilder.sqlite3LocateTable(pParse, 0, pFrom.zName, pFrom.zDatabase);
                        if (pTab == null)
                            return WRC.WRC_Abort;
                        pTab.nRef++;
#if !(SQLITE_OMIT_VIEW) || !(SQLITE_OMIT_VIRTUALTABLE)
                        if (pTab.pSelect != null || pTab.IsVirtual())
                        {
                            ///
                            ///<summary>
                            ///We reach here if the named table is a really a view 
                            ///</summary>
                            if (build.sqlite3ViewGetColumnNames(pParse, pTab) != 0)
                                return WRC.WRC_Abort;
                            pFrom.pSelect = exprc.sqlite3SelectDup(db, pTab.pSelect, 0);
                            pWalker.sqlite3WalkSelect(pFrom.pSelect);
                        }
#endif
                    }
                    ///Locate the index named by the INDEXED BY clause, if any. 
                    if (SelectMethods.sqlite3IndexedByLookup(pParse, pFrom) != 0)
                    {
                        return WRC.WRC_Abort;
                    }
                }
                ///Process NATURAL keywords, and ON and USING clauses of joins.
                if (
                    ///<summary>
                    ///db.mallocFailed != 0 || 
                    ///</summary>
                SelectMethods.sqliteProcessJoin(pParse, p) != 0)
                {
                    return WRC.WRC_Abort;
                }
                ///For every "*" that occurs in the column list, insert the names of
                ///all columns in all tables.  And for every TABLE.* insert the names
                ///of all columns in TABLE.  The parser inserted a special expression
                ///with the TokenType.TK_ALL operator for each "*" that it found in the column list.
                ///The following code just has to locate the TokenType.TK_ALL expressions and expand
                ///each one to the list of all columns in all tables.
                ///
                ///The first loop just checks to see if there are any "*" operators
                ///that need expanding.
                for (k = 0; k < pEList.nExpr; k++)
                {
                    Expr pE = pEList.a[k].pExpr;
                    if (pE.Operator == TokenType.TK_ALL)
                        break;
                    Debug.Assert(pE.Operator != TokenType.TK_DOT || pE.pRight != null);
                    Debug.Assert(pE.Operator != TokenType.TK_DOT || (pE.pLeft != null && pE.pLeft.Operator == TokenType.TK_ID));
                    if (pE.Operator == TokenType.TK_DOT && pE.pRight.Operator == TokenType.TK_ALL)
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
                    SqliteFlags flags = pParse.db.flags;
                    bool longNames = (flags & SqliteFlags.SQLITE_FullColNames) != 0 && (flags & SqliteFlags.SQLITE_ShortColNames) == 0;
                    for (k = 0; k < pEList.nExpr; k++)
                    {
                        Expr pE = a[k].pExpr;
                        Debug.Assert(pE.Operator != TokenType.TK_DOT || pE.pRight != null);
                        if (pE.Operator != TokenType.TK_ALL && (pE.Operator != TokenType.TK_DOT || pE.pRight.Operator != TokenType.TK_ALL))
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
                            if (pE.Operator == TokenType.TK_DOT)
                            {
                                Debug.Assert(pE.pLeft != null);
                                Debug.Assert(!pE.pLeft.ExprHasProperty(ExprFlags.EP_IntValue));
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
                                    if (pTab.aCol[j].IsHiddenColumn())
                                    {
                                        Debug.Assert(pTab.IsVirtual());
                                        continue;
                                    }
                                    if (i > 0 && (zTName == null || zTName.Length == 0))
                                    {
                                        int iDummy = 0;
                                        if ((pFrom.jointype & JoinType.JT_NATURAL) != 0 && SelectMethods.tableAndColumnIndex(pTabList, i, zName, ref iDummy, ref iDummy) != 0)
                                        {
                                            ///
                                            ///<summary>
                                            ///In a NATURAL join, omit the join columns from the
                                            ///table to the right of the join 
                                            ///</summary>
                                            continue;
                                        }
                                        if (build.sqlite3IdListIndex(pFrom.pUsing, zName) >= 0)
                                        {
                                            ///
                                            ///<summary>
                                            ///In a join with a USING clause, omit columns in the
                                            ///using clause from the table on the right. 
                                            ///</summary>
                                            continue;
                                        }
                                    }
                                    pRight = exprc.sqlite3Expr(db, TokenType.TK_ID, zName);
                                    zColname = zName;
                                    zToFree = "";
                                    if (longNames || pTabList.nSrc > 1)
                                    {
                                        Expr pLeft;
                                        pLeft = exprc.sqlite3Expr(db, TokenType.TK_ID, zTabName);
                                        pExpr = pParse.sqlite3PExpr(TokenType.TK_DOT, pLeft, pRight, 0);
                                        if (longNames)
                                        {
                                            zColname = io.sqlite3MPrintf(db, "%s.%s", zTabName, zName);
                                            zToFree = zColname;
                                        }
                                    }
                                    else
                                    {
                                        pExpr = pRight;
                                    }
                                    pNew = pParse.sqlite3ExprListAppend(pNew, pExpr);
                                    sColname.zRestSql = zColname;
                                    sColname.Length = StringExtensions.Strlen30(zColname);
                                    pParse.sqlite3ExprListSetName(pNew, sColname, 0);
                                    db.sqlite3DbFree(ref zToFree);
                                }
                            }
                            if (tableSeen == 0)
                            {
                                if (zTName != null)
                                {
                                    utilc.sqlite3ErrorMsg(pParse, "no such table: %s", zTName);
                                }
                                else
                                {
                                    utilc.sqlite3ErrorMsg(pParse, "no tables specified");
                                }
                            }
                        }
                    }
                    exprc.sqlite3ExprListDelete(db, ref pEList);
                    p.pEList = pNew;
                }
                //#if SQLITE_MAX_COLUMN
                if (p.pEList != null && p.pEList.nExpr > db.aLimit[Globals.SQLITE_LIMIT_COLUMN])
                {
                    utilc.sqlite3ErrorMsg(pParse, "too many columns in result set");
                }
                //#endif
                return WRC.WRC_Continue;
            }


            public static void sqlite3SelectPrep(Sqlite3.Parse pParse,///
                ///The parser context 
        Select p,///
                ///The SELECT statement being coded. 
        NameContext pOuterNC///
                ///Name context for container 
        )
            {
                Connection db;
                if (Sqlite3.NEVER(p == null))
                    return;
                db = pParse.db;
                if ((p.selFlags & SelectFlags.HasTypeInfo) != 0)
                    return;
                Select.sqlite3SelectExpand(pParse, p);
                if (pParse.nErr != 0///|| db.mallocFailed != 0 
                )
                    return;
                ResolveExtensions.sqlite3ResolveSelectNames(pParse, p, pOuterNC);
                if (pParse.nErr != 0///|| db.mallocFailed != 0 
                )
                    return;
                SelectMethods.sqlite3SelectAddTypeInfo(pParse, p);
            }

            ///<summary>
            /// Generate code for the SELECT statement given in the p argument.
            ///
            /// The results are distributed in various ways depending on the
            /// contents of the SelectDest structure pointed to by argument pDest
            /// as follows:
            ///
            ///     pDest.eDest    Result
            ///     ------------    -------------------------------------------
            ///     SelectResultType.Output      Generate a row of output (using the OpCode.OP_ResultRow
            ///                     opcode) for each row in the result set.
            ///
            ///     SelectResultType.Mem         Only valid if the result is a single column.
            ///                     Store the first column of the first result row
            ///                     in register pDest.iParm then abandon the rest
            ///                     of the query.  This destination implies "LIMIT 1".
            ///
            ///     SelectResultType.Set         The result must be a single column.  Store each
            ///                     row of result as the key in table pDest.iParm.
            ///                     Apply the affinity pDest.affinity before storing
            ///                     results.  Used to implement "IN (SELECT ...)".
            ///
            ///     SelectResultType.Union       Store results as a key in a temporary table pDest.iParm.
            ///
            ///     SelectResultType.Except      Remove results from the temporary table pDest.iParm.
            ///
            ///     SelectResultType.Table       Store results in temporary table pDest.iParm.
            ///                     This is like SelectResultType.EphemTab except that the table
            ///                     is assumed to already be open.
            ///
            ///     SelectResultType.EphemTab    Create an temporary table pDest.iParm and store
            ///                     the result there. The cursor is left open after
            ///                     returning.  This is like SelectResultType.Table except that
            ///                     this destination uses OP_OpenEphemeral to create
            ///                     the table first.
            ///
            ///     SelectResultType.Coroutine   Generate a co-routine that returns a new row of
            ///                     results each time it is invoked.  The entry point
            ///                     of the co-routine is stored in register pDest.iParm.
            ///
            ///     SelectResultType.Exists      Store a 1 in memory cell pDest.iParm if the result
            ///                     set is not empty.
            ///
            ///     SelectResultType.Discard     Throw the results away.  This is used by SELECT
            ///                     statements within triggers whose only purpose is
            ///                     the side-effects of functions.
            ///
            /// This routine returns the number of errors.  If any errors are
            /// encountered, then an appropriate error message is left in
            /// pParse.zErrMsg.
            ///
            /// This routine does NOT free the Select structure passed in.  The
            /// calling function needs to do that.
            ///
            ///</summary>
            static SelectDest sdDummy = null;
            static bool bDummy = false;
            public static SqlResult sqlite3Select(Sqlite3.Parse pParse,/*The SELECT statement being coded.*/ Select p,/*What to do with the query results */ref SelectDest pDest)
            {
                ///Loop counters 
                int i, j;

                ///Return from sqlite3WhereBegin() 
                WhereInfo pWInfo;

                ///List of columns to extract. 
                ExprList pEList = new ExprList();

                ///List of tables to select from 
                SrcList SelectSourceList = new SrcList();

                ///The WHERE clause.  May be NULL 
                Expr pWhere;

                ///The ORDER BY clause.  May be NULL 
                ExprList pOrderBy;

                ///The GROUP BY clause.  May be NULL 
                ExprList pGroupBy;

                ///The HAVING clause.  May be NULL 
                Expr pHaving;

                
                ///Table to use for the distinct set 
                int distinct;

                ///Value to return from this function 
                var rc = (SqlResult)1;

                ///Address of an OP_OpenEphemeral instruction 
                int addrSortIndex;

                ///Address of the end of the query 
                int iEnd;

                

#if !SQLITE_OMIT_EXPLAIN
                int iRestoreSelectId = pParse.iSelectId;
                pParse.iSelectId = pParse.iNextSelectId++;
#endif
                ///The database connection 
                Connection db = pParse.db;
                if (p == null/*|| db.mallocFailed != 0 */|| pParse.nErr != 0)
                {
                    return (SqlResult)1;
                }
#if !SQLITE_OMIT_AUTHORIZATION
																																																																											if (sqlite3AuthCheck(pParse, SQLITE_SELECT, 0, 0, 0)) return 1;
#endif
                ///Information used by aggregate queries 
                AggInfo sAggInfo = new AggInfo();

                // memset(sAggInfo, 0, sAggInfo).Length;
                if (pDest.eDest <= SelectResultType.Discard)//IgnorableOrderby(pDest))
                {
                    Debug.Assert(pDest.eDest == SelectResultType.Exists || pDest.eDest == SelectResultType.Union || pDest.eDest == SelectResultType.Except || pDest.eDest == SelectResultType.Discard);
                    ///If ORDER BY makes no difference in the output then neither does
                    ///DISTINCT so it can be removed too. 
                    exprc.sqlite3ExprListDelete(db, ref p.pOrderBy);
                    p.pOrderBy = null;
                    p.selFlags = (p.selFlags & ~SelectFlags.Distinct);
                }

                Select.sqlite3SelectPrep(pParse, p, null);
                
                pOrderBy = p.pOrderBy;
                SelectSourceList = p.pSrc;
                pEList = p.pEList;
                if (pParse.nErr != 0/*|| db.mallocFailed != 0*/)
                {
                    goto select_end;
                }

                ///True for select lists like "count()" 
                var isAgg = (p.selFlags & SelectFlags.Aggregate) != 0;
                Debug.Assert(pEList != null);
                ///Begin generating code.
                ///The virtual machine under construction 
                var v = pParse.sqlite3GetVdbe();
                if (v == null)
                    goto select_end;
                ///If writing to memory or generating a set
                ///only a single column may be output.
                
#if !SQLITE_OMIT_SUBQUERY
                if (SelectMethods.checkForMultiColumnSelectError(pParse, pDest, pEList.nExpr))
                {
                    goto select_end;
                }
#endif

                ///Generate code for all subqueries in the FROM clause
#if !SQLITE_OMIT_SUBQUERY || !SQLITE_OMIT_VIEW
                for (i = 0; p.pPrior == null && i < SelectSourceList.nSrc; i++)
                {
                    SrcList_item pItem = SelectSourceList.a[i];
                    SelectDest dest = new SelectDest();
                    Select pSub = pItem.pSelect;
                    bool isAggSub;
                    if (pSub == null || pItem.isPopulated != 0)
                        continue;
                    ///Increment Parse.nHeight by the height of the largest expression
                    ///tree refered to by this, the parent select. The child select
                    ///may contain expression trees of at most
                    ///(SQLITE_MAX_EXPR_DEPTH">Parse.nHeight) height. This is a bit</param>
                    ///more conservative than necessary, but much easier than enforcing">more conservative than necessary, but much easier than enforcing</param>
                    ///an exact limit.">an exact limit.</param>
                    pParse.nHeight += p.sqlite3SelectExprHeight();
                    ///Check to see if the subquery can be absorbed into the parent. 
                    isAggSub = (pSub.selFlags & SelectFlags.Aggregate) != 0;
                    if (SelectMethods.flattenSubquery(pParse, p, i, isAgg, isAggSub) != 0)
                    {
                        if (isAggSub)
                        {
                            isAgg = true;
                            p.selFlags |= SelectFlags.Aggregate;
                        }
                        i = -1;
                    }
                    else
                    {
                        dest.Init(SelectResultType.EphemTab, pItem.iCursor);
                        Debug.Assert(0 == pItem.isPopulated);
                        SelectMethods.explainSetInteger(ref pItem.iSelectId, (int)pParse.iNextSelectId);
                        sqlite3Select(pParse, pSub, ref dest);
                        pItem.isPopulated = 1;
                        pItem.pTab.nRowEst = (uint)pSub.nSelectRow;
                    }
                    //if ( /* pParse.nErr != 0 || */ db.mallocFailed != 0 )
                    //{
                    //  goto select_end;
                    //}
                    pParse.nHeight -= p.sqlite3SelectExprHeight();
                    SelectSourceList = p.pSrc;
                    if (!(pDest.eDest <= SelectResultType.Discard))//        if( null==IgnorableOrderby(pDest) )
                    {
                        pOrderBy = p.pOrderBy;
                    }
                }
                pEList = p.pEList;
#endif
                pWhere = p.pWhere;
                pGroupBy = p.pGroupBy;
                pHaving = p.pHaving;

                ///True if the DISTINCT keyword is present 
                bool isDistinct = (p.selFlags & SelectFlags.Distinct) != 0;
#if !SQLITE_OMIT_COMPOUND_SELECT
                ///If there is are a sequence of queries, do the earlier ones first.
                if (p.pPrior != null)
                {
                    if (p.pRightmost == null)
                    {
                        Select pLoop, pRight = null;
                        int cnt = 0;
                        int mxSelect;
                        for (pLoop = p; pLoop != null; pLoop = pLoop.pPrior, cnt++)
                        {
                            pLoop.pRightmost = p;
                            pLoop.pNext = pRight;
                            pRight = pLoop;
                        }
                        mxSelect = db.aLimit[Globals.SQLITE_LIMIT_COMPOUND_SELECT];
                        if (mxSelect != 0 && cnt > mxSelect)
                        {
                            utilc.sqlite3ErrorMsg(pParse, "too many terms in compound SELECT");
                            goto select_end;
                        }
                    }
                    rc = multiSelect(pParse, p, pDest);
                    SelectMethods.explainSetInteger(ref pParse.iSelectId, iRestoreSelectId);
                    return rc;
                }
#endif
                ///If possible, rewrite the query to use GROUP BY instead of DISTINCT.
                ///GROUP BY might use an index, DISTINCT never does.
                Debug.Assert(p.pGroupBy == null || (p.selFlags & SelectFlags.Aggregate) != 0);
                if ((p.selFlags & (SelectFlags.Distinct | SelectFlags.Aggregate)) == SelectFlags.Distinct)
                {
                    p.pGroupBy = exprc.sqlite3ExprListDup(db, p.pEList, 0);
                    pGroupBy = p.pGroupBy;
                    p.selFlags = (p.selFlags & ~SelectFlags.Distinct);
                }
                ///If there is both a GROUP BY and an ORDER BY clause and they are
                ///identical, then disable the ORDER BY clause since the GROUP BY
                ///will cause elements to come out in the correct order.  This is
                ///<param name="an optimization "> the correct answer should result regardless.</param>
                ///<param name="Use the SQLITE_GroupByOrder flag with SQLITE_TESTCTRL_OPTIMIZER">Use the SQLITE_GroupByOrder flag with SQLITE_TESTCTRL_OPTIMIZER</param>
                ///<param name="to disable this optimization for testing purposes.">to disable this optimization for testing purposes.</param>
                ///<param name=""></param>
                if (exprc.sqlite3ExprListCompare(p.pGroupBy, pOrderBy) == 0 && (db.flags & SqliteFlags.SQLITE_GroupByOrder) == 0)
                {
                    pOrderBy = null;
                }
                ///
                ///<summary>
                ///If there is an ORDER BY clause, then this sorting
                ///index might end up being unused if the data can be
                ///</summary>
                ///<param name="extracted in pre">sorted order.  If that is the case, then the</param>
                ///<param name="OP_OpenEphemeral instruction will be changed to an OpCode.OP_Noop once">OP_OpenEphemeral instruction will be changed to an OpCode.OP_Noop once</param>
                ///<param name="we figure out that the sorting index is not needed.  The addrSortIndex">we figure out that the sorting index is not needed.  The addrSortIndex</param>
                ///<param name="variable is used to facilitate that change.">variable is used to facilitate that change.</param>
                ///<param name=""></param>
                if (pOrderBy != null)
                {
                    KeyInfo pKeyInfo;
                    pKeyInfo = SelectMethods.keyInfoFromExprList(pParse, pOrderBy);
                    pOrderBy.iECursor = pParse.nTab++;
                    p.addrOpenEphm[2] = addrSortIndex = v.sqlite3VdbeAddOp4(OpCode.OP_OpenEphemeral, pOrderBy.iECursor, pOrderBy.nExpr + 2, 0, pKeyInfo, P4Usage.P4_KEYINFO_HANDOFF);
                }
                else
                {
                    addrSortIndex = -1;
                }
                ///If the output is destined for a temporary table, open that table.
                if (pDest.eDest == SelectResultType.EphemTab)
                {
                    v.sqlite3VdbeAddOp2(OpCode.OP_OpenEphemeral, pDest.iParm, pEList.nExpr);
                }
                ///Set the limiter.
                iEnd = v.sqlite3VdbeMakeLabel();
                p.nSelectRow = (double)IntegerExtensions.LARGEST_INT64;
                SelectMethods.computeLimitRegisters(pParse, p, iEnd);
                ///Open a virtual index to use for the distinct set.
                if ((p.selFlags & SelectFlags.Distinct) != 0)
                {
                    KeyInfo pKeyInfo;
                    Debug.Assert(isAgg || pGroupBy != null);
                    distinct = pParse.nTab++;
                    pKeyInfo = SelectMethods.keyInfoFromExprList(pParse, p.pEList);
                    v.sqlite3VdbeAddOp4(OpCode.OP_OpenEphemeral, distinct, 0, 0, pKeyInfo, P4Usage.P4_KEYINFO_HANDOFF);
                    v.sqlite3VdbeChangeP5(Sqlite3.BTREE_UNORDERED);
                }
                else
                {
                    distinct = -1;
                }
                ///<param name="Aggregate and non">aggregate queries are handled differently </param>
                if (!isAgg && pGroupBy == null)
                {
                    ///<param name="This case is for non">aggregate queries</param>
                    ///<param name="Begin the database scan">Begin the database scan</param>
                    ///<param name=""></param>
                    pWInfo = pParse.sqlite3WhereBegin(SelectSourceList, pWhere, ref pOrderBy, 0);
                    if (pWInfo == null)
                        goto select_end;
                    if (pWInfo.nRowOut < p.nSelectRow)
                        p.nSelectRow = pWInfo.nRowOut;
                    ///If sorting index that was created by a prior OP_OpenEphemeral
                    ///instruction ended up not being needed, then change the OP_OpenEphemeral
                    ///into an OpCode.OP_Noop.
                    if (addrSortIndex >= 0 && pOrderBy == null)
                    {
                        Engine.vdbeaux.sqlite3VdbeChangeToNoop(v, addrSortIndex, 1);
                        p.addrOpenEphm[2] = -1;
                    }
                    ///Use the standard inner loop
                    Debug.Assert(!isDistinct);
                    SelectMethods.selectInnerLoop(pParse, p, pEList, 0, 0, pOrderBy, -1, pDest, pWInfo.iContinue, pWInfo.iBreak);
                    ///End the database scan loop.
                    pWInfo.sqlite3WhereEnd();
                }
                else
                {
                    ///This is the processing for aggregate queries 
                    ///Name context for processing aggregate information 
                    NameContext sNC;
                    ///First Mem address for storing current GROUP BY 
                    int iAMem;
                    ///First Mem address for previous GROUP BY 
                    int iBMem;
                    ///Mem address holding flag indicating that at least
                    ///one row of the input to the aggregator has been
                    ///processed 
                    int iUseFlag;
                    ///Mem address which causes query abort if positive 
                    int iAbortFlag;
                    ///Rows come from source in GR BY' clause thanROUP BY order 
                    int groupBySort;
                    ///End of processing for this SELECT 
                    int addrEnd;
                    ///Remove any and all aliases between the result set and the
                    ///GROUP BY clause.
                    if (pGroupBy != null)
                    {
                        int k;
                        ///Loop counter 
                        ExprList_item pItem;
                        ///For looping over expression in a list 
                        for (k = p.pEList.nExpr; k > 0; k--)//, pItem++)
                        {
                            pItem = p.pEList.a[p.pEList.nExpr - k];
                            pItem.iAlias = 0;
                        }
                        for (k = pGroupBy.nExpr; k > 0; k--)//, pItem++ )
                        {
                            pItem = pGroupBy.a[pGroupBy.nExpr - k];
                            pItem.iAlias = 0;
                        }
                        if (p.nSelectRow > (double)100)
                            p.nSelectRow = (double)100;
                    }
                    else
                    {
                        p.nSelectRow = (double)1;
                    }
                    ///Create a label to jump to when we want to abort the query 
                    addrEnd = v.sqlite3VdbeMakeLabel();
                    ///Convert TokenType.TK_COLUMN nodes into TokenType.TK_AGG_COLUMN and make entries in
                    ///sAggInfo for all TokenType.TK_AGG_FUNCTION nodes in expressions of the
                    ///SELECT statement.
                    sNC = new NameContext();
                    // memset(sNC, 0, sNC).Length;
                    sNC.pParse = pParse;
                    sNC.pSrcList = SelectSourceList;
                    sNC.pAggInfo = sAggInfo;
                    sAggInfo.nSortingColumn = pGroupBy != null ? pGroupBy.nExpr + 1 : 0;
                    sAggInfo.pGroupBy = pGroupBy;
                    exprc.sqlite3ExprAnalyzeAggList(sNC, pEList);
                    exprc.sqlite3ExprAnalyzeAggList(sNC, pOrderBy);
                    if (pHaving != null)
                    {
                        exprc.sqlite3ExprAnalyzeAggregates(sNC, ref pHaving);
                    }
                    sAggInfo.nAccumulator = sAggInfo.nColumn;
                    for (i = 0; i < sAggInfo.nFunc; i++)
                    {
                        Debug.Assert(!sAggInfo.aFunc[i].pExpr.ExprHasProperty(ExprFlags.EP_xIsSelect));
                        exprc.sqlite3ExprAnalyzeAggList(sNC, sAggInfo.aFunc[i].pExpr.x.pList);
                    }
                    //      if ( db.mallocFailed != 0 ) goto select_end;
                    ///Processing for aggregates with GROUP BY is very different and
                    ///much more complex than aggregates without a GROUP BY.
                    if (pGroupBy != null)
                    {
                        KeyInfo pKeyInfo;
                        ///Keying information for the group by clause 
                        int j1;
                        ///<param name="A">B comparision jump </param>
                        int addrOutputRow;
                        ///Start of subroutine that outputs a result row 
                        int regOutputRow;
                        ///Return address register for output subroutine 
                        int addrSetAbort;
                        ///Set the abort flag and return 
                        int addrTopOfLoop;
                        ///Top of the input loop 
                        int addrSortingIdx;
                        ///The OP_OpenEphemeral for the sorting index 
                        int addrReset;
                        ///Subroutine for resetting the accumulator 
                        int regReset;
                        ///Return address register for reset subroutine 
                        ///If there is a GROUP BY clause we might need a sorting index to
                        ///implement it.  Allocate that sorting index now.  If it turns out
                        ///that we do not need it after all, the OpenEphemeral instruction
                        ///will be converted into a Noop.
                        sAggInfo.sortingIdx = pParse.nTab++;
                        pKeyInfo = SelectMethods.keyInfoFromExprList(pParse, pGroupBy);
                        addrSortingIdx = v.sqlite3VdbeAddOp4(OpCode.OP_OpenEphemeral, sAggInfo.sortingIdx, sAggInfo.nSortingColumn, 0, pKeyInfo, P4Usage.P4_KEYINFO_HANDOFF);
                        ///Initialize memory locations used by GROUP BY aggregate processing
                        ///x
                        iUseFlag = ++pParse.nMem;
                        iAbortFlag = ++pParse.nMem;
                        regOutputRow = ++pParse.nMem;
                        addrOutputRow = v.sqlite3VdbeMakeLabel();
                        regReset = ++pParse.nMem;
                        addrReset = v.sqlite3VdbeMakeLabel();
                        iAMem = pParse.nMem + 1;
                        pParse.nMem += pGroupBy.nExpr;
                        iBMem = pParse.nMem + 1;
                        pParse.nMem += pGroupBy.nExpr;
                        v.sqlite3VdbeAddOp2(OpCode.OP_Integer, 0, iAbortFlag);
#if SQLITE_DEBUG
																																																																																																																													          VdbeComment( v, "clear abort flag" );
#endif
                        v.sqlite3VdbeAddOp2(OpCode.OP_Integer, 0, iUseFlag);
#if SQLITE_DEBUG
																																																																																																																													          VdbeComment( v, "indicate accumulator empty" );
#endif
                        ///Begin a loop that will extract all source rows in GROUP BY order.
                        ///This might involve two separate loops with an OP_Sort in between, or
                        ///it might be a single loop that uses an index to extract information
                        ///in the right order to begin with.
                        v.sqlite3VdbeAddOp2(OpCode.OP_Gosub, regReset, addrReset);
                        pWInfo = pParse.sqlite3WhereBegin(SelectSourceList, pWhere, ref pGroupBy, 0);
                        if (pWInfo == null)
                            goto select_end;
                        if (pGroupBy == null)
                        {
                            ///The optimizer is able to deliver rows in group by order so
                            ///we do not have to sort.  The OP_OpenEphemeral table will be
                            ///cancelled later because we still need to use the pKeyInfo
                            pGroupBy = p.pGroupBy;
                            groupBySort = 0;
                        }
                        else
                        {
                            ///Rows are coming out in undetermined order.  We have to push
                            ///each row into a sorting index, terminate the first loop,
                            ///then loop over the sorting index in order to get the output
                            ///in sorted order
                            int regBase;
                            int regRecord;
                            int nCol;
                            int nGroupBy;
                            SelectMethods.explainTempTable(pParse, isDistinct && 0 == (p.selFlags & SelectFlags.Distinct) ? "DISTINCT" : "GROUP BY");
                            groupBySort = 1;
                            nGroupBy = pGroupBy.nExpr;
                            nCol = nGroupBy + 1;
                            j = nGroupBy + 1;
                            for (i = 0; i < sAggInfo.nColumn; i++)
                            {
                                if (sAggInfo.aCol[i].iSorterColumn >= j)
                                {
                                    nCol++;
                                    j++;
                                }
                            }
                            regBase = pParse.sqlite3GetTempRange(nCol);
                            pParse.sqlite3ExprCacheClear();
                            pParse.sqlite3ExprCodeExprList(pGroupBy, regBase, false);
                            v.sqlite3VdbeAddOp2(OpCode.OP_Sequence, sAggInfo.sortingIdx, regBase + nGroupBy);
                            j = nGroupBy + 1;
                            for (i = 0; i < sAggInfo.nColumn; i++)
                            {
                                AggInfo_col pCol = sAggInfo.aCol[i];
                                if (pCol.iSorterColumn >= j)
                                {
                                    int r1 = j + regBase;
                                    int r2;
                                    r2 = pParse.sqlite3ExprCodeGetColumn(pCol.pTab, pCol.iColumn, pCol.iTable, r1);
                                    if (r1 != r2)
                                    {
                                        v.sqlite3VdbeAddOp2(OpCode.OP_SCopy, r2, r1);
                                    }
                                    j++;
                                }
                            }
                            regRecord = pParse.sqlite3GetTempReg();
                            v.sqlite3VdbeAddOp3(OpCode.OP_MakeRecord, regBase, nCol, regRecord);
                            v.sqlite3VdbeAddOp2(OpCode.OP_IdxInsert, sAggInfo.sortingIdx, regRecord);
                            pParse.sqlite3ReleaseTempReg(regRecord);
                            pParse.sqlite3ReleaseTempRange(regBase, nCol);
                            pWInfo.sqlite3WhereEnd();
                            v.sqlite3VdbeAddOp2(OpCode.OP_Sort, sAggInfo.sortingIdx, addrEnd);
#if SQLITE_DEBUG
																																																																																																																																																						            VdbeComment( v, "GROUP BY sort" );
#endif
                            sAggInfo.useSortingIdx = 1;
                            pParse.sqlite3ExprCacheClear();
                        }
                        ///Evaluate the current GROUP BY terms and store in b0, b1, b2...
                        ///(b0 is memory location iBMem+0, b1 is iBMem+1, and so forth)
                        ///Then compare the current GROUP BY terms against the GROUP BY terms
                        ///from the previous row currently stored in a0, a1, a2...
                        addrTopOfLoop = v.sqlite3VdbeCurrentAddr();
                        pParse.sqlite3ExprCacheClear();
                        for (j = 0; j < pGroupBy.nExpr; j++)
                        {
                            if (groupBySort != 0)
                            {
                                v.sqlite3VdbeAddOp3(OpCode.OP_Column, sAggInfo.sortingIdx, j, iBMem + j);
                            }
                            else
                            {
                                sAggInfo.directMode = 1;
                                pParse.sqlite3ExprCode(pGroupBy.a[j].pExpr, iBMem + j);
                            }
                        }
                        v.sqlite3VdbeAddOp4(OpCode.OP_Compare, iAMem, iBMem, pGroupBy.nExpr, pKeyInfo, P4Usage.P4_KEYINFO);
                        j1 = v.sqlite3VdbeCurrentAddr();
                        v.sqlite3VdbeAddOp3(OpCode.OP_Jump, j1 + 1, 0, j1 + 1);
                        ///Generate code that runs whenever the GROUP BY changes.
                        ///Changes in the GROUP BY are detected by the previous code
                        ///block.  If there were no changes, this block is skipped.
                        ///
                        ///This code copies current group by terms in b0,b1,b2,...
                        ///over to a0,a1,a2.  It then calls the output subroutine
                        ///and resets the aggregate accumulator registers in preparation
                        ///for the next GROUP BY batch.
                        pParse.sqlite3ExprCodeMove(iBMem, iAMem, pGroupBy.nExpr);
                        v.sqlite3VdbeAddOp2(OpCode.OP_Gosub, regOutputRow, addrOutputRow);
#if SQLITE_DEBUG
																																																																																																																													          VdbeComment( v, "output one row" );
#endif
                        v.sqlite3VdbeAddOp2(OpCode.OP_IfPos, iAbortFlag, addrEnd);
#if SQLITE_DEBUG
																																																																																																																													          VdbeComment( v, "check abort flag" );
#endif
                        v.sqlite3VdbeAddOp2(OpCode.OP_Gosub, regReset, addrReset);
#if SQLITE_DEBUG
																																																																																																																													          VdbeComment( v, "reset accumulator" );
#endif
                        ///Update the aggregate accumulators based on the content of
                        ///the current row
                        v.sqlite3VdbeJumpHere(j1);
                        SelectMethods.updateAccumulator(pParse, sAggInfo);
                        v.sqlite3VdbeAddOp2(OpCode.OP_Integer, 1, iUseFlag);
#if SQLITE_DEBUG
																																																																																																																													          VdbeComment( v, "indicate data in accumulator" );
#endif
                        ///End of the loop
                        if (groupBySort != 0)
                        {
                            v.sqlite3VdbeAddOp2(OpCode.OP_Next, sAggInfo.sortingIdx, addrTopOfLoop);
                        }
                        else
                        {
                            pWInfo.sqlite3WhereEnd();
                            Engine.vdbeaux.sqlite3VdbeChangeToNoop(v, addrSortingIdx, 1);
                        }
                        ///Output the final row of result
                        v.sqlite3VdbeAddOp2(OpCode.OP_Gosub, regOutputRow, addrOutputRow);
#if SQLITE_DEBUG
																																																																																																																													          VdbeComment( v, "output final row" );
#endif
                        ///Jump over the subroutines
                        v.sqlite3VdbeAddOp2(OpCode.OP_Goto, 0, addrEnd);
                        ///Generate a subroutine that outputs a single row of the result
                        ///set.  This subroutine first looks at the iUseFlag.  If iUseFlag
                        ///<param name="is less than or equal to zero, the subroutine is a no">op.  If</param>
                        ///<param name="the processing calls for the query to abort, this subroutine">the processing calls for the query to abort, this subroutine</param>
                        ///<param name="increments the iAbortFlag memory location before returning in">increments the iAbortFlag memory location before returning in</param>
                        ///<param name="order to signal the caller to abort.">order to signal the caller to abort.</param>
                        ///<param name=""></param>
                        addrSetAbort = v.sqlite3VdbeCurrentAddr();
                        v.sqlite3VdbeAddOp2(OpCode.OP_Integer, 1, iAbortFlag);
                        v.VdbeComment( "set abort flag");
                        v.sqlite3VdbeAddOp1(OpCode.OP_Return, regOutputRow);
                        v.sqlite3VdbeResolveLabel(addrOutputRow);
                        addrOutputRow = v.sqlite3VdbeCurrentAddr();
                        v.sqlite3VdbeAddOp2(OpCode.OP_IfPos, iUseFlag, addrOutputRow + 2);
                        v.VdbeComment( "Groupby result generator entry point");
                        v.sqlite3VdbeAddOp1(OpCode.OP_Return, regOutputRow);
                        SelectMethods.finalizeAggFunctions(pParse, sAggInfo);
                        pParse.sqlite3ExprIfFalse(pHaving, addrOutputRow + 1, sqliteinth.SQLITE_JUMPIFNULL);
                        SelectMethods.selectInnerLoop(pParse, p, p.pEList, 0, 0, pOrderBy, distinct, pDest, addrOutputRow + 1, addrSetAbort);
                        v.sqlite3VdbeAddOp1(OpCode.OP_Return, regOutputRow);
                        v.VdbeComment( "end groupby result generator");
                        ///<param name="Generate a subroutine that will reset the group">by accumulator</param>
                        v.sqlite3VdbeResolveLabel(addrReset);
                        SelectMethods.resetAccumulator(pParse, sAggInfo);
                        v.sqlite3VdbeAddOp1(OpCode.OP_Return, regReset);
                    }
                    ///endif pGroupBy.  Begin aggregate queries without GROUP BY: 
                    else
                    {
                        ExprList pDel = null;
#if !SQLITE_OMIT_BTREECOUNT
                        Table pTab;
                        if ((pTab = SelectMethods.isSimpleCount(p, sAggInfo)) != null)
                        {
                            ///If isSimpleCount() returns a pointer to a Table structure, then
                            ///the SQL statement is of the form:
                            ///SELECT count() FROM <tbl>
                            ///
                            ///where the Table structure returned represents table <tbl>.
                            ///
                            ///This statement is so common that it is optimized specially. The
                            ///OP_Count instruction is executed either on the intkey table that
                            ///contains the data for table <tbl> or on one of its indexes. It
                            ///is better to execute the op on an index, as indexes are almost
                            ///always spread across less pages than their corresponding tables.
                            int iDb = Sqlite3.sqlite3SchemaToIndex(pParse.db, pTab.pSchema);
                            int iCsr = pParse.nTab++;
                            ///<param name="Cursor to scan b">tree </param>
                            Index pIdx;
                            ///Iterator variable 
                            KeyInfo pKeyInfo = null;
                            ///Keyinfo for scanned index 
                            Index pBest = null;
                            ///Best index found so far 
                            int iRoot = pTab.tnum;
                            ///<param name="Root page of scanned b">tree </param>
                            build.sqlite3CodeVerifySchema(pParse, iDb);
                            sqliteinth.sqlite3TableLock(pParse, iDb, pTab.tnum, 0, pTab.zName);
                            ///Search for the index that has the least amount of columns. If
                            ///there is such an index, and it has less columns than the table
                            ///does, then we can assume that it consumes less space on disk and
                            ///will therefore be cheaper to scan to determine the query result.
                            ///<param name="In this case set iRoot to the root page number of the index b">tree</param>
                            ///<param name="and pKeyInfo to the KeyInfo structure required to navigate the">and pKeyInfo to the KeyInfo structure required to navigate the</param>
                            ///<param name="index.">index.</param>
                            ///<param name=""></param>
                            ///<param name="(2011">15) Do not do a full scan of an unordered index.</param>
                            ///<param name=""></param>
                            ///<param name="In practice the KeyInfo structure will not be used. It is only">In practice the KeyInfo structure will not be used. It is only</param>
                            ///<param name="passed to keep OP_OpenRead happy.">passed to keep OP_OpenRead happy.</param>
                            ///<param name=""></param>
                            for (pIdx = pTab.pIndex; pIdx != null; pIdx = pIdx.pNext)
                            {
                                if (pIdx.bUnordered == 0 && (null == pBest || pIdx.nColumn < pBest.nColumn))
                                {
                                    pBest = pIdx;
                                }
                            }
                            if (pBest != null && pBest.nColumn < pTab.nCol)
                            {
                                iRoot = pBest.tnum;
                                pKeyInfo = pBest.sqlite3IndexKeyinfo(pParse);
                            }
                            ///<param name="Open a read">only cursor, execute the OP_Count, close the cursor. </param>
                            v.sqlite3VdbeAddOp3(OpCode.OP_OpenRead, iCsr, iRoot, iDb);
                            if (pKeyInfo != null)
                            {
                                v.sqlite3VdbeChangeP4(-1, pKeyInfo,  P4Usage.P4_KEYINFO_HANDOFF);
                            }
                            v.sqlite3VdbeAddOp2(OpCode.OP_Count, iCsr, sAggInfo.aFunc[0].iMem);
                            v.sqlite3VdbeAddOp1(OpCode.OP_Close, iCsr);
                            pParse.explainSimpleCount(pTab, pBest);
                        }
                        else
#endif
                        {
                            ///Check if the query is of one of the following forms:
                            ///
                            ///SELECT min(x) FROM ...
                            ///SELECT max(x) FROM ...
                            ///
                            ///If it is, then ask the code in where.c to attempt to sort results
                            ///as if there was an "ORDER ON x" or "ORDER ON x DESC" clause.
                            ///If where.c is able to produce results sorted in this order, then
                            ///add vdbe code to break out of the processing loop after the
                            ///first iteration (since the first iteration of the loop is
                            ///guaranteed to operate on the row with the minimum or maximum
                            ///value of x, the only row required).
                            ///
                            ///A special flag must be passed to sqlite3WhereBegin() to slightly
                            ///modify behavior as follows:
                            ///
                            ///+ If the query is a "SELECT min(x)", then the loop coded by
                            ///where.c should not iterate over any values with a NULL value
                            ///for x.
                            ///
                            ///+ The optimizer code in where.c (the thing that decides which
                            ///index or indices to use) should place a different priority on
                            ///satisfying the 'ORDER BY' clause than it does in other cases.
                            ///Refer to code and comments in where.c for details.
                            ExprList pMinMax = null;
                            int flag = SelectMethods.minMaxQuery(p);
                            if (flag != 0)
                            {
                                Debug.Assert(!p.pEList.a[0].pExpr.ExprHasProperty(ExprFlags.EP_xIsSelect));
                                pMinMax = exprc.sqlite3ExprListDup(db, p.pEList.a[0].pExpr.x.pList, 0);
                                pDel = pMinMax;
                                if (pMinMax != null)///* && 0 == db.mallocFailed */ )
                                {
                                    pMinMax.a[0].sortOrder = (SortOrder)(flag != wherec.WHERE_ORDERBY_MIN ? 1 : 0);
                                    pMinMax.a[0].pExpr.Operator = TokenType.TK_COLUMN;
                                }
                            }
                            ///This case runs if the aggregate has no GROUP BY clause.  The
                            ///processing is much simpler since there is only a single row
                            ///of output.
                            SelectMethods.resetAccumulator(pParse, sAggInfo);
                            pWInfo = pParse.sqlite3WhereBegin(SelectSourceList, pWhere, ref pMinMax, (byte)flag);
                            if (pWInfo == null)
                            {
                                exprc.sqlite3ExprListDelete(db, ref pDel);
                                goto select_end;
                            }
                            SelectMethods.updateAccumulator(pParse, sAggInfo);
                            if (pMinMax == null && flag != 0)
                            {
                                v.sqlite3VdbeAddOp2(OpCode.OP_Goto, 0, pWInfo.iBreak);
#if SQLITE_DEBUG
																																																																																																																																																																															              VdbeComment( v, "%s() by index",
              ( flag == WHERE_ORDERBY_MIN ? "min" : "max" ) );
#endif
                            }
                            pWInfo.sqlite3WhereEnd();
                            SelectMethods.finalizeAggFunctions(pParse, sAggInfo);
                        }
                        pOrderBy = null;
                        pParse.sqlite3ExprIfFalse(pHaving, addrEnd, sqliteinth.SQLITE_JUMPIFNULL);
                        SelectMethods.selectInnerLoop(pParse, p, p.pEList, 0, 0, null, -1, pDest, addrEnd, addrEnd);
                        exprc.sqlite3ExprListDelete(db, ref pDel);
                    }
                    v.sqlite3VdbeResolveLabel(addrEnd);
                }
                ///endif aggregate query 
                if (distinct >= 0)
                {
                    SelectMethods.explainTempTable(pParse, "DISTINCT");
                }
                ///If there is an ORDER BY clause, then we need to sort the results
                ///and send them to the callback one by one.
                if (pOrderBy != null)
                {
                    SelectMethods.explainTempTable(pParse, "ORDER BY");
                    SelectMethods.generateSortTail(pParse, p, v, pEList.nExpr, pDest);
                }
                ///Jump here to skip this query
                v.sqlite3VdbeResolveLabel(iEnd);
                ///The SELECT was successfully coded.   Set the return code to 0
                ///to indicate no errors.
                rc = 0;
            ///Control jumps to here if an error is encountered above, or upon
            ///successful coding of the SELECT.
            select_end:
                SelectMethods.explainSetInteger(ref pParse.iSelectId, iRestoreSelectId);
                ///Identify column names if results of the SELECT are to be output.
            if (rc == SqlResult.SQLITE_OK && pDest.eDest == SelectResultType.Output)
                {
                    SelectMethods.generateColumnNames(pParse, SelectSourceList, pEList);
                }
                db.sqlite3DbFree(ref sAggInfo.aCol);
                db.sqlite3DbFree(ref sAggInfo.aFunc);
                return rc;
            }

#if !SQLITE_OMIT_COMPOUND_SELECT
            ///<summary>
            /// This routine is called to process a compound query form from
            /// two or more separate queries using UNION, UNION ALL, EXCEPT, or
            /// INTERSECT
            ///
            /// "p" points to the right-most of the two queries.  the query on the
            /// left is p.pPrior.  The left query could also be a compound query
            /// in which case this routine will be called recursively.
            ///
            /// The results of the total query are to be written into a destination
            /// of type eDest with parameter iParm.
            ///
            /// Example 1:  Consider a three-way compound SQL statement.
            ///
            ///     SELECT a FROM t1 UNION SELECT b FROM t2 UNION SELECT c FROM t3
            ///
            /// This statement is parsed up as follows:
            ///
            ///     SELECT c FROM t3
            ///      |
            ///      `----.  SELECT b FROM t2
            ///                |
            ///                `-----.  SELECT a FROM t1
            ///
            /// The arrows in the diagram above represent the Select.pPrior pointer.
            /// So if this routine is called with p equal to the t3 query, then
            /// pPrior will be the t2 query.  p.op will be TokenType.TK_UNION in this case.
            ///
            /// Notice that because of the way SQLite parses compound SELECTs, the
            /// individual selects always group from left to right.
            ///</summary>
            static SqlResult multiSelect(Sqlite3.Parse pParse,///
                ///Parsing context 
            Select p,///
                ///<param name="The right">most of SELECTs to be coded </param>
            SelectDest pDest///
                ///What to do with query results 
            )
            {
                var rc = SqlResult.SQLITE_OK;
                ///Success code from a subroutine 
                Select pPrior;
                ///Another SELECT immediately to our left 
                Vdbe v;
                ///Generate code to this VDBE 
                SelectDest dest = new SelectDest();
                ///Alternative data destination 
                Select pDelete = null;
                ///Chain of simple selects to delete 
                Connection db;
                ///Database connection 
#if !SQLITE_OMIT_EXPLAIN
                int iSub1 = 0;
                ///<param name="EQP id of left">hand query </param>
                int iSub2 = 0;
                ///<param name="EQP id of right">hand query </param>
#endif
                ///Make sure there is no ORDER BY or LIMIT clause on prior SELECTs.  Only
                ///<param name="the last (right">most) SELECT in the series may have an ORDER BY or LIMIT.</param>
                Debug.Assert(p != null && p.pPrior != null);
                ///Calling function guarantees this much 
                db = pParse.db;
                pPrior = p.pPrior;
                Debug.Assert(pPrior.pRightmost != pPrior);
                Debug.Assert(pPrior.pRightmost == p.pRightmost);
                dest = pDest;
                if (pPrior.pOrderBy != null)
                {
                    utilc.sqlite3ErrorMsg(pParse, "ORDER BY clause should come after %s not before", SelectMethods.selectOpName(p.TokenOp));
                    rc = (SqlResult)1;
                    goto multi_select_end;
                }
                if (pPrior.pLimit != null)
                {
                    utilc.sqlite3ErrorMsg(pParse, "LIMIT clause should come after %s not before", SelectMethods.selectOpName(p.TokenOp));
                    rc = (SqlResult)1;
                    goto multi_select_end;
                }
                v = pParse.sqlite3GetVdbe();
                Debug.Assert(v != null);
                ///The VDBE already created by calling function 
                ///
                ///Create the destination temporary table if necessary
                if (dest.eDest == SelectResultType.EphemTab)
                {
                    Debug.Assert(p.pEList != null);
                    v.sqlite3VdbeAddOp2(OpCode.OP_OpenEphemeral, dest.iParm, p.pEList.nExpr);
                    v.sqlite3VdbeChangeP5(Sqlite3.BTREE_UNORDERED);
                    dest.eDest = SelectResultType.Table;
                }
                ///Make sure all SELECTs in the statement have the same number of elements
                ///in their result sets.
                Debug.Assert(p.pEList != null && pPrior.pEList != null);
                if (p.pEList.nExpr != pPrior.pEList.nExpr)
                {
                    utilc.sqlite3ErrorMsg(pParse, "SELECTs to the left and right of %s" + " do not have the same number of result columns", SelectMethods.selectOpName(p.TokenOp));
                    rc = (SqlResult)1;
                    goto multi_select_end;
                }
                ///Compound SELECTs that have an ORDER BY clause are handled separately.
                if (p.pOrderBy != null)
                {
                    return SelectMethods.multiSelectOrderBy(pParse, p, pDest);
                }
                ///Generate code for the left and right SELECT statements.
                switch ((TokenType)p.tk_op)
                {
                    case TokenType.TK_ALL:
                        {
                            int addr = 0;
                            int nLimit = 0;
                            Debug.Assert(pPrior.pLimit == null);
                            pPrior.pLimit = p.pLimit;
                            pPrior.pOffset = p.pOffset;
                            SelectMethods.explainSetInteger(ref iSub1, pParse.iNextSelectId);
                            rc = Select.sqlite3Select(pParse, pPrior, ref dest);
                            p.pLimit = null;
                            p.pOffset = null;
                            if (rc != 0)
                            {
                                goto multi_select_end;
                            }
                            p.pPrior = null;
                            p.iLimit = pPrior.iLimit;
                            p.iOffset = pPrior.iOffset;
                            if (p.iLimit != 0)
                            {
                                addr = v.sqlite3VdbeAddOp1(OpCode.OP_IfZero, p.iLimit);
#if SQLITE_DEBUG
																																																																																																																													              VdbeComment( v, "Jump ahead if LIMIT reached" );
#endif
                            }
                            SelectMethods.explainSetInteger(ref iSub2, pParse.iNextSelectId);
                            rc = Select.sqlite3Select(pParse, p, ref dest);
                            sqliteinth.testcase(rc != SqlResult.SQLITE_OK);
                            pDelete = p.pPrior;
                            p.pPrior = pPrior;
                            p.nSelectRow += pPrior.nSelectRow;
                            if (pPrior.pLimit != null && pPrior.pLimit.sqlite3ExprIsInteger(ref nLimit)  && p.nSelectRow > (double)nLimit)
                            {
                                p.nSelectRow = (double)nLimit;
                            }
                            if (addr != 0)
                            {
                                v.sqlite3VdbeJumpHere(addr);
                            }
                            break;
                        }
                    case TokenType.TK_EXCEPT:
                    case TokenType.TK_UNION:
                        {
                            int unionTab;
                            ///VdbeCursor number of the temporary table holding result 
                            SelectResultType op = 0;
                            ///One of the SelectResultType. operations to apply to self 
                            SelectResultType priorOp;
                            ///The SelectResultType. operation to apply to prior selects 
                            Expr pLimit, pOffset;
                            ///Saved values of p.nLimit and p.nOffset 
                            int addr;
                            SelectDest uniondest = new SelectDest();
                            sqliteinth.testcase(p.TokenOp== TokenType.TK_EXCEPT);
                            sqliteinth.testcase(p.TokenOp == TokenType.TK_UNION);
                            priorOp = SelectResultType.Union;
                            if (dest.eDest == priorOp && Sqlite3.ALWAYS(null == p.pLimit && null == p.pOffset))
                            {
                                ///We can reuse a temporary table generated by a SELECT to our
                                ///right.
                                Debug.Assert(p.pRightmost != p);
                                ///Can only happen for leftward elements
                                ///<param name="of a 3">way or more compound </param>
                                Debug.Assert(p.pLimit == null);
                                ///Not allowed on leftward elements 
                                Debug.Assert(p.pOffset == null);
                                ///Not allowed on leftward elements 
                                unionTab = dest.iParm;
                            }
                            else
                            {
                                ///We will need to create our own temporary table to hold the
                                ///intermediate results.
                                unionTab = pParse.nTab++;
                                Debug.Assert(p.pOrderBy == null);
                                addr = v.sqlite3VdbeAddOp2(OpCode.OP_OpenEphemeral, unionTab, 0);
                                Debug.Assert(p.addrOpenEphm[0] == -1);
                                p.addrOpenEphm[0] = addr;
                                p.pRightmost.selFlags |= SelectFlags.UsesEphemeral;
                                Debug.Assert(p.pEList != null);
                            }
                            ///Code the SELECT statements to our left
                            Debug.Assert(pPrior.pOrderBy == null);
                            uniondest.Init(priorOp, unionTab);
                            SelectMethods.explainSetInteger(ref iSub1, pParse.iNextSelectId);
                            rc = Select.sqlite3Select(pParse, pPrior, ref uniondest);
                            if (rc != 0)
                            {
                                goto multi_select_end;
                            }
                            ///Code the current SELECT statement
                            if (p.TokenOp == TokenType.TK_EXCEPT)
                            {
                                op = SelectResultType.Except;
                            }
                            else
                            {
                                Debug.Assert(p.TokenOp == TokenType.TK_UNION);
                                op = SelectResultType.Union;
                            }
                            p.pPrior = null;
                            pLimit = p.pLimit;
                            p.pLimit = null;
                            pOffset = p.pOffset;
                            p.pOffset = null;
                            uniondest.eDest = op;
                            SelectMethods.explainSetInteger(ref iSub2, pParse.iNextSelectId);
                            rc = Select.sqlite3Select(pParse, p, ref uniondest);
                            sqliteinth.testcase(rc != SqlResult.SQLITE_OK);
                            ///
                            ///<summary>
                            ///Query flattening in Select.sqlite3Select() might refill p.pOrderBy.
                            ///Be sure to delete p.pOrderBy, therefore, to avoid a memory leak. 
                            ///</summary>
                            exprc.sqlite3ExprListDelete(db, ref p.pOrderBy);
                            pDelete = p.pPrior;
                            p.pPrior = pPrior;
                            p.pOrderBy = null;
                            if (p.TokenOp == TokenType.TK_UNION)
                                p.nSelectRow += pPrior.nSelectRow;
                            exprc.sqlite3ExprDelete(db, ref p.pLimit);
                            p.pLimit = pLimit;
                            p.pOffset = pOffset;
                            p.iLimit = 0;
                            p.iOffset = 0;
                            ///
                            ///<summary>
                            ///Convert the data in the temporary table into whatever form
                            ///it is that we currently need.
                            ///
                            ///</summary>
                            Debug.Assert(unionTab == dest.iParm || dest.eDest != priorOp);
                            if (dest.eDest != priorOp)
                            {
                                int iCont, iBreak, iStart;
                                Debug.Assert(p.pEList != null);
                                if (dest.eDest == SelectResultType.Output)
                                {
                                    Select pFirst = p;
                                    while (pFirst.pPrior != null)
                                        pFirst = pFirst.pPrior;
                                    SelectMethods.generateColumnNames(pParse, null, pFirst.pEList);
                                }
                                iBreak = v.sqlite3VdbeMakeLabel();
                                iCont = v.sqlite3VdbeMakeLabel();
                                SelectMethods.computeLimitRegisters(pParse, p, iBreak);
                                v.sqlite3VdbeAddOp2(OpCode.OP_Rewind, unionTab, iBreak);
                                iStart = v.sqlite3VdbeCurrentAddr();
                                SelectMethods.selectInnerLoop(pParse, p, p.pEList, unionTab, p.pEList.nExpr, null, -1, dest, iCont, iBreak);
                                v.sqlite3VdbeResolveLabel(iCont);
                                v.sqlite3VdbeAddOp2(OpCode.OP_Next, unionTab, iStart);
                                v.sqlite3VdbeResolveLabel(iBreak);
                                v.sqlite3VdbeAddOp2(OpCode.OP_Close, unionTab, 0);
                            }
                            break;
                        }
                    default:
                        Debug.Assert(p.TokenOp == TokenType.TK_INTERSECT);
                        {
                            int tab1, tab2;
                            int iCont, iBreak, iStart;
                            Expr pLimit, pOffset;
                            int addr;
                            SelectDest intersectdest = new SelectDest();
                            int r1;
                            ///INTERSECT is different from the others since it requires
                            ///two temporary tables.  Hence it has its own case.  Begin
                            ///by allocating the tables we will need.
                            tab1 = pParse.nTab++;
                            tab2 = pParse.nTab++;
                            Debug.Assert(p.pOrderBy == null);
                            addr = v.sqlite3VdbeAddOp2(OpCode.OP_OpenEphemeral, tab1, 0);
                            Debug.Assert(p.addrOpenEphm[0] == -1);
                            p.addrOpenEphm[0] = addr;
                            p.pRightmost.selFlags |= SelectFlags.UsesEphemeral;
                            Debug.Assert(p.pEList != null);
                            ///Code the SELECTs to our left into temporary table "tab1".
                            intersectdest.Init(SelectResultType.Union, tab1);
                            SelectMethods.explainSetInteger(ref iSub1, pParse.iNextSelectId);
                            rc = Select.sqlite3Select(pParse, pPrior, ref intersectdest);
                            if (rc != 0)
                            {
                                goto multi_select_end;
                            }
                            ///Code the current SELECT into temporary table "tab2"
                            addr = v.sqlite3VdbeAddOp2(OpCode.OP_OpenEphemeral, tab2, 0);
                            Debug.Assert(p.addrOpenEphm[1] == -1);
                            p.addrOpenEphm[1] = addr;
                            p.pPrior = null;
                            pLimit = p.pLimit;
                            p.pLimit = null;
                            pOffset = p.pOffset;
                            p.pOffset = null;
                            intersectdest.iParm = tab2;
                            SelectMethods.explainSetInteger(ref iSub2, pParse.iNextSelectId);
                            rc = Select.sqlite3Select(pParse, p, ref intersectdest);
                            sqliteinth.testcase(rc != SqlResult.SQLITE_OK);
                            p.pPrior = pPrior;
                            if (p.nSelectRow > pPrior.nSelectRow)
                                p.nSelectRow = pPrior.nSelectRow;
                            exprc.sqlite3ExprDelete(db, ref p.pLimit);
                            p.pLimit = pLimit;
                            p.pOffset = pOffset;
                            ///Generate code to take the intersection of the two temporary
                            ///tables.
                            Debug.Assert(p.pEList != null);
                            if (dest.eDest == SelectResultType.Output)
                            {
                                Select pFirst = p;
                                while (pFirst.pPrior != null)
                                    pFirst = pFirst.pPrior;
                                SelectMethods.generateColumnNames(pParse, null, pFirst.pEList);
                            }
                            iBreak = v.sqlite3VdbeMakeLabel();
                            iCont = v.sqlite3VdbeMakeLabel();
                            SelectMethods.computeLimitRegisters(pParse, p, iBreak);
                            v.sqlite3VdbeAddOp2(OpCode.OP_Rewind, tab1, iBreak);
                            r1 = pParse.sqlite3GetTempReg();
                            iStart = v.sqlite3VdbeAddOp2(OpCode.OP_RowKey, tab1, r1);
                            v.sqlite3VdbeAddOp4Int(OpCode.OP_NotFound, tab2, iCont, r1, 0);
                            pParse.sqlite3ReleaseTempReg(r1);
                            SelectMethods.selectInnerLoop(pParse, p, p.pEList, tab1, p.pEList.nExpr, null, -1, dest, iCont, iBreak);
                            v.sqlite3VdbeResolveLabel(iCont);
                            v.sqlite3VdbeAddOp2(OpCode.OP_Next, tab1, iStart);
                            v.sqlite3VdbeResolveLabel(iBreak);
                            v.sqlite3VdbeAddOp2(OpCode.OP_Close, tab2, 0);
                            v.sqlite3VdbeAddOp2(OpCode.OP_Close, tab1, 0);
                            break;
                        }
                }
                SelectMethods.explainComposite(pParse, p.TokenOp, iSub1, iSub2, p.TokenOp != TokenType.TK_ALL);
                ///
                ///<summary>
                ///Compute collating sequences used by
                ///temporary tables needed to implement the compound select.
                ///Attach the KeyInfo structure to all temporary tables.
                ///
                ///</summary>
                ///<param name="This section is run by the right">most SELECT statement only.</param>
                ///<param name="SELECT statements to the left always skip this part.  The right">most</param>
                ///<param name="SELECT might also skip this part if it has no ORDER BY clause and">SELECT might also skip this part if it has no ORDER BY clause and</param>
                ///<param name="no temp tables are required.">no temp tables are required.</param>
                ///<param name=""></param>
                if ((p.selFlags & SelectFlags.UsesEphemeral) != 0)
                {
                    int i;
                    ///Loop counter 
                    KeyInfo pKeyInfo;
                    ///Collating sequence for the result set 
                    Select pLoop;
                    ///For looping through SELECT statements 
                    CollSeq apColl;
                    ///For looping through pKeyInfo.aColl[] 
                    int nCol;
                    ///Number of columns in result set 
                    Debug.Assert(p.pRightmost == p);
                    nCol = p.pEList.nExpr;
                    pKeyInfo = new KeyInfo();
                    //sqlite3DbMallocZero(db,
                    pKeyInfo.aColl = new CollSeq[nCol];
                    //sizeof(*pKeyInfo)+nCol*(CollSeq*.Length + 1));
                    //if ( pKeyInfo == null )
                    //{
                    //  rc = SQLITE_NOMEM;
                    //  goto multi_select_end;
                    //}
                    pKeyInfo.enc = db.aDbStatic[0].pSchema.enc;
                    // sqliteinth.sqliteinth.ENC( pParse.db );
                    pKeyInfo.nField = (u16)nCol;
                    for (i = 0; i < nCol; i++)
                    {
                        //, apColl++){
                        apColl = SelectMethods.multiSelectCollSeq(pParse, p, i);
                        if (null == apColl)
                        {
                            apColl = db.pDfltColl;
                        }
                        pKeyInfo.aColl[i] = apColl;
                    }
                    for (pLoop = p; pLoop != null; pLoop = pLoop.pPrior)
                    {
                        for (i = 0; i < 2; i++)
                        {
                            int addr = pLoop.addrOpenEphm[i];
                            if (addr < 0)
                            {
                                ///If [0] is unused then [1] is also unused.  So we can
                                ///always safely abort as soon as the first unused slot is found 
                                Debug.Assert(pLoop.addrOpenEphm[1] < 0);
                                break;
                            }
                            v.sqlite3VdbeChangeP2(addr, nCol);
                            v.sqlite3VdbeChangeP4(addr, pKeyInfo,  P4Usage.P4_KEYINFO);
                            pLoop.addrOpenEphm[i] = -1;
                        }
                    }
                    db.sqlite3DbFree(ref pKeyInfo);
                }
            multi_select_end:
                pDest.iMem = dest.iMem;
                pDest.nMem = dest.nMem;
                SelectMethods.SelectDestructor(db, ref pDelete);
                return rc;
            }
#endif




            ///<summary>
            /// Add type and collation information to a column list based on
            /// a SELECT statement.
            ///
            /// The column list presumably came from selectColumnNamesFromExprList().
            /// The column list has only names, not types or collations.  This
            /// routine goes through and adds the types and collations.
            ///
            /// This routine requires that all identifiers in the SELECT
            /// statement be resolved.
            ///
            ///</summary>
            public static void selectAddColumnTypeAndCollation(Sqlite3.Parse pParse,///
                ///Parsing contexts 
            int nCol,///
                ///Number of columns 
            Column[] aCol,///
                ///List of columns 
            Select pSelect///
                ///SELECT used to determine types and collations 
            )
            {
                Connection db = pParse.db;
                NameContext sNC;
                Column pCol;
                CollSeq pColl;
                int i;
                Expr expr;
                ExprList_item[] a;
                Debug.Assert(pSelect != null);
                Debug.Assert((pSelect.selFlags & SelectFlags.Resolved) != 0);
                Debug.Assert(nCol == pSelect.pEList.nExpr///
                    ///<summary>
                    ///|| db.mallocFailed != 0 
                    ///</summary>
                );
                //      if ( db.mallocFailed != 0 ) return;
                sNC = new NameContext();
                // memset( &sNC, 0, sizeof( sNC ) );
                sNC.pSrcList = pSelect.pSrc;
                a = pSelect.pEList.a;
                for (i = 0; i < nCol; i++)//, pCol++ )
                {
                    pCol = aCol[i];
                    expr = a[i].pExpr;
                    string bDummy = null;
                    pCol.zType = expr.columnType(sNC, ref bDummy, ref bDummy, ref bDummy);
                    // sqlite3DbStrDup( db, columnType( sNC, p, 0, 0, 0 ) );
                    pCol.affinity = expr.sqlite3ExprAffinity();
                    if (pCol.affinity == 0)
                        pCol.affinity = sqliteinth.SQLITE_AFF_NONE;
                    pColl = pParse.sqlite3ExprCollSeq(expr);
                    if (pColl != null)
                    {
                        pCol.zColl = pColl.zName;
                        // sqlite3DbStrDup( db, pColl.zName );
                    }
                }
            }
            ///<summary>
            /// Allocate a new Select structure and return a pointer to that
            /// structure.
            ///
            ///</summary>
            // OVERLOADS, so I don't need to rewrite parse.c
            public static Select sqlite3SelectNew(Sqlite3.Parse pParse, int null_2, SrcList pSrc, int null_4, int null_5, int null_6, int null_7, int isDistinct, int null_9, int null_10)
            {
                return sqlite3SelectNew(pParse, null, pSrc, null, null, null, null, isDistinct, null, null);
            }
            public static Select sqlite3SelectNew(Sqlite3.Parse pParse,/*which columns to include in the result */ExprList pEList, /*the FROM clause "> which tables to scan */SrcList pSrc, /*the WHERE clause */Expr pWhere,
                /*the GROUP BY clause */ExprList pGroupBy,/*the HAVING clause */Expr pHaving, /*the ORDER BY clause*/ExprList pOrderBy,/*true if the DISTINCT keyword is present */int isDistinct,
                /*LIMIT value.  NULL means not used */Expr pLimit,/*OFFSET value.  NULL means no offset */Expr pOffset
            )
            {
                Select pNew;
                //           Select standin;
                Connection db = pParse.db;
                pNew = new Select();
                //sqlite3DbMallocZero(db, sizeof(*pNew) );
                Debug.Assert(//db.mallocFailed != 0 ||
                null == pOffset || pLimit != null);
                ///OFFSET implies LIMIT 
                //if( pNew==null   ){
                //  pNew = standin;
                //  memset(pNew, 0, sizeof(*pNew));
                //}
                if (pEList == null)
                {
                    pEList = pParse.sqlite3ExprListAppend(null, exprc.sqlite3Expr(db, TokenType.TK_ALL, null));
                }
                pNew.pEList = pEList;
                pNew.pSrc = pSrc;
                pNew.pWhere = pWhere;
                pNew.pGroupBy = pGroupBy;
                pNew.pHaving = pHaving;
                pNew.pOrderBy = pOrderBy;
                pNew.selFlags = (isDistinct != 0 ? SelectFlags.Distinct : 0);
                pNew.TokenOp = TokenType.TK_SELECT;
                pNew.pLimit = pLimit;
                pNew.pOffset = pOffset;
                Debug.Assert(pOffset == null || pLimit != null);
                pNew.addrOpenEphm[0] = -1;
                pNew.addrOpenEphm[1] = -1;
                pNew.addrOpenEphm[2] = -1;
                //if ( db.mallocFailed != 0 )
                //{
                //  clearSelect( db, pNew );
                //  //if ( pNew != standin ) sqlite3DbFree( db, ref pNew );
                //  pNew = null;
                //}
                return pNew;
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
            ///Output should be DISTINCT 
            Resolved = 0x0002,
            ///Identifiers have been resolved 
            Aggregate = 0x0004,
            ///Contains aggregate functions 
            UsesEphemeral = 0x0008,
            ///Uses the OpenEphemeral opcode 
            Expanded = 0x0010,
            ///sqlite3SelectExpand() called on this 
            HasTypeInfo = 0x0020
            ///FROM subqueries have Table metadata 
        }


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
        /// But build.sqlite3SrcListShiftJoinType() later shifts the jointypes so that each
        /// jointype expresses the join between the table and the previous table.
        ///
        /// In the colUsed field, the high-order bit (bit 63) is set if the table
        /// contains more than 63 columns and the 64-th or later column is used.
        ///
        ///</summary>
        public class SrcList_item
        {
            ///<summary>
            ///Name of database holding this table 
            ///</summary>
            public string zDatabase;

            
            ///<summary>
            ///Name of the table 
            ///</summary>
            public string zName;

            ///<summary>
            ///The "B" part of a "A AS B" phrase.  zName is the "A" 
            ///</summary>
            public string zAlias;
            
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
            public JoinType jointype;
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
        


        ///<summary>
        /// Permitted values of the SrcList.a.jointype field
        ///
        ///</summary>
        public enum JoinType : byte{
            JT_INNER = 0x0001,
            //#define JT_INNER     0x0001    /* Any kind of inner or cross join */
            JT_CROSS = 0x0002,
            //#define JT_CROSS     0x0002    /* Explicit use of the CROSS keyword */
            JT_NATURAL = 0x0004,
            //#define JT_NATURAL   0x0004    /* True for a "natural" join */
            JT_LEFT = 0x0008,
            //#define JT_LEFT      0x0008    /* Left outer join */
            JT_RIGHT = 0x0010,
            //#define JT_RIGHT     0x0010    /* Right outer join */
            JT_OUTER = 0x0020,
            //#define JT_OUTER     0x0020    /* The "OUTER" keyword is present */
            JT_ERROR = 0x0040
            //#define JT_ERROR     0x0040    /* unknown or unsupported join type */
        }

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

            ///<summary>
            ///Beginning of keyword text in zKeyText[] 
            ///</summary>
            public u8 i;


            ///<summary>
            ///Length of the keyword in characters 
            ///</summary>
            public u8 nChar;
            

            ///<summary>
            ///Join type mask 
            ///</summary>
            public JoinType code;
            
            
            public Keyword(u8 i, u8 nChar, JoinType code)
            {
                this.i = i;
                this.nChar = nChar;
                this.code = code;
            }
        }
    }

