using System;
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
using System.Linq;
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

    using ParseState = Community.CsharpSqlite.Sqlite3.ParseState;

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

        public static Select Create(Sqlite3.ParseState pParse,/*which columns to include in the result */ExprList pEList, /*the FROM clause "> which tables to scan */SrcList pSrc, /*the WHERE clause */Expr pWhere,
            /*the GROUP BY clause */ExprList pGroupBy,/*the HAVING clause */Expr pHaving, /*the ORDER BY clause*/ExprList pOrderBy,/*true if the DISTINCT keyword is present */int isDistinct,
            /*LIMIT value.  NULL means not used */Expr pLimit,/*OFFSET value.  NULL means no offset */Expr pOffset
        )
        {
            //           Select standin;
            Connection db = pParse.db;
            Debug.Assert(pOffset == null || pLimit != null);
            var pNew = new Select()
            {
                ResultingFieldList = pEList,
                FromSource = pSrc,
                pWhere = pWhere,
                pGroupBy = pGroupBy,
                pHaving = pHaving,
                pOrderBy = pOrderBy,
                Flags = (isDistinct != 0 ? SelectFlags.Distinct : 0),
                TokenOp = TokenType.TK_SELECT,
                pLimit = pLimit,
                pOffset = pOffset,
                addrOpenEphm = new int[] { -1, -1, -1 }
            };
            //sqlite3DbMallocZero(db, sizeof(*pNew) );
            Debug.Assert(null == pOffset || pLimit != null);
            ///OFFSET implies LIMIT 
            if (pEList == null)
            {
                pEList = CollectionExtensions.Append(null, exprc.sqlite3Expr(db, TokenType.TK_ALL, null));
            }

            return pNew;
        }


        public void ShowDebugInfo()
        {
            //this.ResultingFieldList.a[0].pExpr
        }

        ///<summary>
        ///pEList
        ///The fields of the result 
        ///</summary>
        public ExprList ResultingFieldList;

        ///<summary>
        ///One of: TokenType.TK_UNION TokenType.TK_ALL TokenType.TK_INTERSECT TokenType.TK_EXCEPT 
        ///</summary>        
        public TokenType TokenOp{get; set;}

        ///<summary>
        ///MakeRecord with this affinity for SelectResultType.Set 
        ///</summary>
        public char affinity;

        ///<summary>
        ///Various SF_* values 
        ///</summary>
        public SelectFlags Flags;

        ///<summary>
        ///The FROM clause 
        ///</summary>
        public SrcList FromSource;

        ///<summary>
        ///The WHERE clause 
        ///</summary>
        public Expr pWhere;

        ///<summary>
        ///The GROUP BY clause 
        ///</summary>
        public ExprList pGroupBy;

        ///<summary>
        ///The HAVING clause 
        ///</summary>
        public Expr pHaving;

        public ExprList pOrderBy;
        ///<summary>
        ///The ORDER BY clause 
        ///</summary>

        public Select pPrior;

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
                if (ResultingFieldList != null)
                    cp.ResultingFieldList = ResultingFieldList.Copy();
                if (FromSource != null)
                    cp.FromSource = FromSource.Copy();
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

        public int SelectExprHeight()
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
        public static void sqlite3SelectExpand(Sqlite3.ParseState pParse, Select pSelect)
        {
            Walker w = new Walker()
            {
                xSelectCallback = Select.selectExpander,
                xExprCallback = SelectMethods.exprWalkNoop,
                ParseState = pParse
            };
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
            var parseState = pWalker.ParseState;
            int i, j, k;
            Connection db = parseState.db;

            if (Sqlite3.NEVER(p.FromSource == null) || (p.Flags & SelectFlags.Expanded) != 0)
            {
                return WRC.WRC_Prune;
            }
            p.Flags |= SelectFlags.Expanded;
            var pTabList = p.FromSource;
            var ResultingFieldList = p.ResultingFieldList;
            ///Make sure cursor numbers have been assigned to all entries in
            ///the FROM clause of the SELECT statement.
            build.sqlite3SrcListAssignCursors(parseState, pTabList);
            ///Look up every table named in the FROM clause of the select.  If
            ///an entry of the FROM clause is a subquery instead of a table or view,
            ///then create a transient table ure to describe the subquery.
            for (i = 0; i < pTabList.Count; i++)// pFrom++ )
            {
                var pFrom = pTabList[i];
                if (pFrom.TableReference != null)
                {
                    ///This statement has already been prepared.  There is no need
                    ///to go further. 
                    Debug.Assert(i == 0);
                    return WRC.WRC_Prune;
                }
                if (pFrom.zName == null)
                {
#if !SQLITE_OMIT_SUBQUERY
                    var fromSelect = pFrom.pSelect;
                    ///A sub-query in the FROM clause of a SELECT 
                    Debug.Assert(fromSelect != null);
                    Debug.Assert(pFrom.TableReference == null);
                    pWalker.sqlite3WalkSelect(fromSelect);
                    var pTab = pFrom.TableReference = new Table()//dummy
                    {
                        nRef = 1,
                        zName = io.sqlite3MPrintf(db, "sqlite_subquery_%p_", fromSelect.FromSource[0].zName),
                        iPKey = -1,
                        nRowEst = 1000000
                    };
                    pTab.tabFlags |= TableFlags.TF_Ephemeral;

                    fromSelect = fromSelect.path(x => x.pPrior).Last();

                    SelectMethods.selectColumnsFromExprList(parseState, fromSelect.ResultingFieldList, ref pTab.nCol, ref pTab.aCol);
#endif
                }
                else
                {
                    ///An ordinary table or view name in the FROM clause 
                    Debug.Assert(pFrom.TableReference == null);
                    var pTab = pFrom.TableReference = TableBuilder.sqlite3LocateTable(parseState, pFrom.zName, pFrom.zDatabase);
                    if (pTab == null)
                        return WRC.WRC_Abort;
                    pTab.nRef++;
#if !(SQLITE_OMIT_VIEW) || !(SQLITE_OMIT_VIRTUALTABLE)
                    if (pTab.pSelect != null || pTab.IsVirtual())
                    {
                        ///We reach here if the named table is a really a view 
                        if (build.sqlite3ViewGetColumnNames(parseState, pTab) != 0)
                            return WRC.WRC_Abort;
                        pFrom.pSelect = exprc.Clone(db, pTab.pSelect, 0);
                        pWalker.sqlite3WalkSelect(pFrom.pSelect);
                    }
#endif
                }
                ///Locate the index named by the INDEXED BY clause, if any. 
                if (SelectMethods.sqlite3IndexedByLookup(parseState, pFrom) != 0)
                {
                    return WRC.WRC_Abort;
                }
            }

            ///Process NATURAL keywords, and ON and USING clauses of joins.
            if (SelectMethods.sqliteProcessJoin(parseState, p) != 0)
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
            for (k = 0; k < ResultingFieldList.Count; k++)
            {
                Expr pE = ResultingFieldList[k].pExpr;
                if (pE.Operator == TokenType.TK_ALL)
                    break;
                Debug.Assert(pE.Operator != TokenType.TK_DOT || pE.pRight != null);
                Debug.Assert(pE.Operator != TokenType.TK_DOT || (pE.pLeft != null && pE.pLeft.Operator == TokenType.TK_ID));
                if (pE.Operator == TokenType.TK_DOT && pE.pRight.Operator == TokenType.TK_ALL)
                    break;
            }
            if (k < ResultingFieldList.Count)
            {
                ///If we get here it means the result set contains one or more "*"
                ///operators that need to be expanded.  Loop through each expression
                ///in the result set and expand them one by one.
                var a = ResultingFieldList.a;
                ExprList pNew = null;
                SqliteFlags flags = parseState.db.flags;
                bool longNames = (flags & SqliteFlags.SQLITE_FullColNames) != 0 && (flags & SqliteFlags.SQLITE_ShortColNames) == 0;
                for (k = 0; k < ResultingFieldList.Count; k++)
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
                        pNew = pNew.Append(a[k].pExpr);
                        if (pNew != null)
                        {
                            pNew[pNew.Count - 1].zName = a[k].zName;
                            pNew[pNew.Count - 1].zSpan = a[k].zSpan;
                            a[k].zName = null;
                            a[k].zSpan = null;
                        }
                        a[k].pExpr = null;
                    }
                    else
                    {
                        ///This expression is a "*" or a "TABLE.*" and needs to be                            
                        int tableSeen = 0;
                        ///Set to 1 when TABLE matches 
                        string zTName;
                        ///text of name of TABLE 
                        if (pE.Operator == TokenType.TK_DOT)
                        {
                            Debug.Assert(pE.pLeft != null);
                            Debug.Assert(!pE.pLeft.HasProperty(ExprFlags.EP_IntValue));
                            zTName = pE.pLeft.u.zToken;
                        }
                        else
                        {
                            zTName = null;
                        }
                        for (i = 0; i < pTabList.Count; i++)//, pFrom++ )
                        {
                            var pFrom = pTabList.a[i];
                            Table pTab = pFrom.TableReference;
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
                                ///The computed column name 
                                string zToFree;
                                ///Malloced string that needs to be freed 
                                Token sColname = new Token();
                                ///Computed column name as a token 
                                ///
                                ///If a column is marked as 'hidden' (currently only possible
                                ///for virtual tables), do not include it in the expanded
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
                                        ///In a NATURAL join, omit the join columns from the
                                        ///table to the right of the join 
                                        continue;
                                    }
                                    if (build.sqlite3IdListIndex(pFrom.pUsing, zName) >= 0)
                                    {
                                        ///In a join with a USING clause, omit columns in the
                                        ///using clause from the table on the right. 
                                        continue;
                                    }
                                }
                                pRight = exprc.sqlite3Expr(db, TokenType.TK_ID, zName);
                                zColname = zName;
                                zToFree = "";
                                if (longNames || pTabList.Count > 1)
                                {
                                    Expr pLeft;
                                    pLeft = exprc.sqlite3Expr(db, TokenType.TK_ID, zTabName);
                                    pExpr = parseState.sqlite3PExpr(TokenType.TK_DOT, pLeft, pRight, 0);
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
                                pNew = pNew.Append(pExpr);
                                sColname.zRestSql = zColname;
                                sColname.Length = StringExtensions.Strlen30(zColname);
                                parseState.sqlite3ExprListSetName(pNew, sColname, 0);
                                db.DbFree(ref zToFree);
                            }
                        }
                        if (tableSeen == 0)
                        {
                            if (zTName != null)
                            {
                                utilc.sqlite3ErrorMsg(parseState, "no such table: %s", zTName);
                            }
                            else
                            {
                                utilc.sqlite3ErrorMsg(parseState, "no tables specified");
                            }
                        }
                    }
                }
                exprc.Delete(db, ref ResultingFieldList);
                p.ResultingFieldList = pNew;
            }
            //#if SQLITE_MAX_COLUMN
            if (p.ResultingFieldList != null && p.ResultingFieldList.Count > db.aLimit[Globals.SQLITE_LIMIT_COLUMN])
            {
                utilc.sqlite3ErrorMsg(parseState, "too many columns in result set");
            }
            //#endif
            return WRC.WRC_Continue;
        }


        public static void sqlite3SelectPrep(
            Sqlite3.ParseState pParse,///The parser context 
                Select p,///The SELECT statement being coded. 
                NameContext pOuterNC///Name context for container 
            )
        {
            if (Sqlite3.NEVER(p == null))
                return;
            var db = pParse.db;
            if ((p.Flags & SelectFlags.HasTypeInfo) != 0)
                return;

            Select.sqlite3SelectExpand(pParse, p);
            if (pParse.nErr != 0)
                return;
            ResolveExtensions.ResolveSelectNames(pParse, p, pOuterNC);
            if (pParse.nErr != 0)
                return;
            SelectMethods.AddTypeInfo(pParse, p);
        }





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
        public static void selectAddColumnTypeAndCollation(
            Sqlite3.ParseState pParse,///Parsing contexts 
                int nCol,///Number of columns 
                Column[] aCol,///List of columns 
                Select pSelect///SELECT used to determine types and collations 
            )
        {
            Debug.Assert(pSelect != null);
            Debug.Assert((pSelect.Flags & SelectFlags.Resolved) != 0);
            Debug.Assert(nCol == pSelect.ResultingFieldList.Count);

            var sNC = new NameContext()
            {
                pSrcList = pSelect.FromSource
            };

            var a = pSelect.ResultingFieldList.a;
            for (var i = 0; i < nCol; i++)//, pCol++ )
            {
                var pCol = aCol[i];
                var expr = a[i].pExpr;
                string bDummy = null;
                pCol.zType = expr.columnType(sNC, ref bDummy, ref bDummy, ref bDummy);
                // sqlite3DbStrDup( db, columnType( sNC, p, 0, 0, 0 ) );
                pCol.affinity = expr.sqlite3ExprAffinity();
                if (pCol.affinity == 0)
                    pCol.affinity = sqliteinth.SQLITE_AFF_NONE;
                var pColl = pParse.sqlite3ExprCollSeq(expr);
                if (pColl != null)
                {
                    pCol.Collation = pColl.zName;
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
        public static Select sqlite3SelectNew(Sqlite3.ParseState pParse, int null_2, SrcList pSrc, int null_4, int null_5, int null_6, int null_7, int isDistinct, int null_9, int null_10)
        {
            return Create(pParse, null, pSrc, null, null, null, null, isDistinct, null, null);
        }


        

    }

        ///<summary>
        /// Allowed values for Select.selFlags.  The "SF" prefix stands for
        /// "Select Flag".
        ///
        ///</summary>
        /* Output should be DISTINCT */
        /* Identifiers have been resolved */
        /* Contains aggregate functions */
        /* Uses the OpenEphemeral opcode */
        /* sqlite3SelectExpand() called on this */
        /* FROM subqueries have Table metadata */
        public enum SelectFlags : ushort//PHASES
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

     


    }

