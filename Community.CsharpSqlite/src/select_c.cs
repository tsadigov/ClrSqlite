#define SQLITE_MAX_EXPR_DEPTH
using System;
using System.Diagnostics;
using System.Text;
using i16=System.Int16;
using u8=System.Byte;
using u16=System.UInt16;
using u32=System.UInt32;
using Pgno=System.UInt32;
namespace Community.CsharpSqlite {
    public partial class Sqlite3
    {
        public class SelectMethods
        {
            ///<summary>
            /// 2001 September 15
            ///
            /// The author disclaims copyright to this source code.  In place of
            /// a legal notice, here is a blessing:
            ///
            ///    May you do good and not evil.
            ///    May you find forgiveness for yourself and forgive others.
            ///    May you share freely, never taking more than you give.
            ///
            ///
            /// This file contains C code routines that are called by the parser
            /// to handle SELECT statements in SQLite.
            ///
            ///  Included in SQLite3 port to C#-SQLite;  2008 Noah B Hart
            ///  C#-SQLite is an independent reimplementation of the SQLite software library
            ///
            ///  SQLITE_SOURCE_ID: 2011-06-23 19:49:22 4374b7e83ea0a3fbc3691f9c0c936272862f32f2
            ///
            ///
            ///
            ///</summary>
            //#include "sqliteInt.h"
            ///<summary>
            /// Delete all the content of a Select structure but do not deallocate
            /// the select structure itself.
            ///
            ///</summary>
            static void clearSelect(sqlite3 db, Select p)
            {
                exprc.sqlite3ExprListDelete(db, ref p.pEList);
                build.sqlite3SrcListDelete(db, ref p.pSrc);
                exprc.sqlite3ExprDelete(db, ref p.pWhere);
                exprc.sqlite3ExprListDelete(db, ref p.pGroupBy);
                exprc.sqlite3ExprDelete(db, ref p.pHaving);
                exprc.sqlite3ExprListDelete(db, ref p.pOrderBy);
                SelectMethods.sqlite3SelectDelete(db, ref p.pPrior);
                exprc.sqlite3ExprDelete(db, ref p.pLimit);
                exprc.sqlite3ExprDelete(db, ref p.pOffset);
            }



            ///<summary>
            /// Delete the given Select structure and all of its substructures.
            ///
            ///</summary>
            public static void sqlite3SelectDelete(sqlite3 db, ref Select p)
            {
                if (p != null)
                {
                    clearSelect(db, p);
                    db.sqlite3DbFree(ref p);
                }
            }

            // OVERLOADS, so I don't need to rewrite parse.c
            public static int sqlite3JoinType(Parse pParse, Token pA, int null_3, int null_4)
            {
                return sqlite3JoinType(pParse, pA, null, null);
            }
            public static int sqlite3JoinType(Parse pParse, Token pA, Token pB, int null_4)
            {
                return sqlite3JoinType(pParse, pA, pB, null);
            }
            public static int sqlite3JoinType(Parse pParse, Token pA, Token pB, Token pC)
            {
                int jointype = 0;
                Token[] apAll = new Token[3];
                Token p;
                ///
                ///<summary>
                ///0123456789 123456789 123456789 123 
                ///</summary>
                string zKeyText = "naturaleftouterightfullinnercross";
                Keyword[] aKeyword = new Keyword[] {
				///
				///<summary>
				///natural 
				///</summary>
				new Keyword(0,7,JT_NATURAL),
				///
				///<summary>
				///left    
				///</summary>
				new Keyword(6,4,JT_LEFT|JT_OUTER),
				///
				///<summary>
				///outer   
				///</summary>
				new Keyword(10,5,JT_OUTER),
				///
				///<summary>
				///right   
				///</summary>
				new Keyword(14,5,JT_RIGHT|JT_OUTER),
				///
				///<summary>
				///full    
				///</summary>
				new Keyword(19,4,JT_LEFT|JT_RIGHT|JT_OUTER),
				///
				///<summary>
				///inner   
				///</summary>
				new Keyword(23,5,JT_INNER),
				///
				///<summary>
				///cross   
				///</summary>
				new Keyword(28,5,JT_INNER|JT_CROSS),
			};
                int i, j;
                apAll[0] = pA;
                apAll[1] = pB;
                apAll[2] = pC;
                for (i = 0; i < 3 && apAll[i] != null; i++)
                {
                    p = apAll[i];
                    for (j = 0; j < ArraySize(aKeyword); j++)
                    {
                        if (p.Length == aKeyword[j].nChar && p.zRestSql.StartsWith(zKeyText.Substring(aKeyword[j].i, aKeyword[j].nChar), StringComparison.InvariantCultureIgnoreCase))
                        {
                            jointype |= aKeyword[j].code;
                            break;
                        }
                    }
                    testcase(j == 0 || j == 1 || j == 2 || j == 3 || j == 4 || j == 5 || j == 6);
                    if (j >= ArraySize(aKeyword))
                    {
                        jointype |= JT_ERROR;
                        break;
                    }
                }
                if ((jointype & (JT_INNER | JT_OUTER)) == (JT_INNER | JT_OUTER) || (jointype & JT_ERROR) != 0)
                {
                    string zSp = " ";
                    Debug.Assert(pB != null);
                    if (pC == null)
                    {
                        zSp = "";
                    }
                    utilc.sqlite3ErrorMsg(pParse, "unknown or unsupported join type: " + "%T %T%s%T", pA, pB, zSp, pC);
                    jointype = JT_INNER;
                }
                else
                    if ((jointype & JT_OUTER) != 0 && (jointype & (JT_LEFT | JT_RIGHT)) != JT_LEFT)
                    {
                        utilc.sqlite3ErrorMsg(pParse, "RIGHT and FULL OUTER JOINs are not currently supported");
                        jointype = JT_INNER;
                    }
                return jointype;
            }

            ///<summary>
            /// Search the first N tables in pSrc, from left to right, looking for a
            /// table that has a column named zCol.
            ///
            /// When found, set *piTab and *piCol to the table index and column index
            /// of the matching column and return TRUE.
            ///
            /// If not found, return FALSE.
            ///
            ///</summary>
            public static int tableAndColumnIndex(SrcList pSrc,///
                ///Array of tables to search 
            int N,///
                ///Number of tables in pSrc.a[] to search 
            string zCol,///
                ///Name of the column we are looking for 
            ref int piTab,///
                ///Write index of pSrc.a[] here 
            ref int piCol///
                ///Write index of pSrc.a[*piTab].pTab.aCol[] here 
            )
            {
                int i;
                ///
                ///<summary>
                ///For looping over tables in pSrc 
                ///</summary>
                int iCol;
                ///
                ///<summary>
                ///Index of column matching zCol 
                ///</summary>
                Debug.Assert((piTab == 0) == (piCol == 0));
                ///
                ///<summary>
                ///Both or neither are NULL 
                ///</summary>
                for (i = 0; i < N; i++)
                {
                    iCol = pSrc.a[i].pTab.columnIndex(zCol);
                    if (iCol >= 0)
                    {
                        //if( piTab )
                        {
                            piTab = i;
                            piCol = iCol;
                        }
                        return 1;
                    }
                }
                return 0;
            }
            ///<summary>
            /// This function is used to add terms implied by JOIN syntax to the
            /// WHERE clause expression of a SELECT statement. The new term, which
            /// is ANDed with the existing WHERE clause, is of the form:
            ///
            ///    (vtab1.col1 = tab2.col2)
            ///
            /// where tab1 is the iSrc'th table in SrcList pSrc and tab2 is the
            /// (iSrc+1)'th. Column col1 is column iColLeft of tab1, and col2 is
            /// column iColRight of tab2.
            ///
            ///</summary>
            static void addWhereTerm(Parse pParse,///
                ///Parsing context 
            SrcList pSrc,///
                ///List of tables in FROM clause 
            int iLeft,///
                ///Index of first table to join in pSrc 
            int iColLeft,///
                ///Index of column in first table 
            int iRight,///
                ///Index of second table in pSrc 
            int iColRight,///
                ///Index of column in second table 
            int isOuterJoin,///
                ///True if this is an OUTER join 
            ref Expr ppWhere///
                ///IN/OUT: The WHERE clause to add to 
            )
            {
                sqlite3 db = pParse.db;
                Expr pE1;
                Expr pE2;
                Expr pEq;
                Debug.Assert(iLeft < iRight);
                Debug.Assert(pSrc.nSrc > iRight);
                Debug.Assert(pSrc.a[iLeft].pTab != null);
                Debug.Assert(pSrc.a[iRight].pTab != null);
                pE1 = ResolveExtensions.sqlite3CreateColumnExpr(db, pSrc, iLeft, iColLeft);
                pE2 = ResolveExtensions.sqlite3CreateColumnExpr(db, pSrc, iRight, iColRight);
                pEq = pParse.sqlite3PExpr(TK_EQ, pE1, pE2, 0);
                if (pEq != null && isOuterJoin != 0)
                {
                    pEq.ExprSetProperty(EP_FromJoin);
                    Debug.Assert(!pEq.ExprHasAnyProperty(EP_TokenOnly | EP_Reduced));
                    pEq.ExprSetIrreducible();
                    pEq.iRightJoinTable = (i16)pE2.iTable;
                }
                ppWhere = exprc.sqlite3ExprAnd(db, ppWhere, pEq);
            }
            ///<summary>
            /// Set the EP_FromJoin property on all terms of the given expression.
            /// And set the Expr.iRightJoinTable to iTable for every term in the
            /// expression.
            ///
            /// The EP_FromJoin property is used on terms of an expression to tell
            /// the LEFT OUTER JOIN processing logic that this term is part of the
            /// join restriction specified in the ON or USING clause and not a part
            /// of the more general WHERE clause.  These terms are moved over to the
            /// WHERE clause during join processing but we need to remember that they
            /// originated in the ON or USING clause.
            ///
            /// The Expr.iRightJoinTable tells the WHERE clause processing that the
            /// expression depends on table iRightJoinTable even if that table is not
            /// explicitly mentioned in the expression.  That information is needed
            /// for cases like this:
            ///
            ///    SELECT * FROM t1 LEFT JOIN t2 ON t1.a=t2.b AND t1.x=5
            ///
            /// The where clause needs to defer the handling of the t1.x=5
            /// term until after the t2 loop of the join.  In that way, a
            /// NULL t2 row will be inserted whenever t1.x!=5.  If we do not
            /// defer the handling of t1.x=5, it will be processed immediately
            /// after the t1 loop and rows with t1.x!=5 will never appear in
            /// the output, which is incorrect.
            ///
            ///</summary>
            static void setJoinExpr(Expr p, int iTable)
            {
                while (p != null)
                {
                    p.ExprSetProperty(EP_FromJoin);
                    Debug.Assert(!p.ExprHasAnyProperty(EP_TokenOnly | EP_Reduced));
                    p.ExprSetIrreducible();
                    p.iRightJoinTable = (i16)iTable;
                    setJoinExpr(p.pLeft, iTable);
                    p = p.pRight;
                }
            }
            ///<summary>
            /// This routine processes the join information for a SELECT statement.
            /// ON and USING clauses are converted into extra terms of the WHERE clause.
            /// NATURAL joins also create extra WHERE clause terms.
            ///
            /// The terms of a FROM clause are contained in the Select.pSrc structure.
            /// The left most table is the first entry in Select.pSrc.  The right-most
            /// table is the last entry.  The join operator is held in the entry to
            /// the left.  Thus entry 0 contains the join operator for the join between
            /// entries 0 and 1.  Any ON or USING clauses associated with the join are
            /// also attached to the left entry.
            ///
            /// This routine returns the number of errors encountered.
            ///
            ///</summary>
            public static int sqliteProcessJoin(Parse pParse, Select p)
            {
                SrcList pSrc;
                ///
                ///<summary>
                ///All tables in the FROM clause 
                ///</summary>
                int i;
                int j;
                ///
                ///<summary>
                ///Loop counters 
                ///</summary>
                SrcList_item pLeft;
                ///
                ///<summary>
                ///Left table being joined 
                ///</summary>
                SrcList_item pRight;
                ///
                ///<summary>
                ///Right table being joined 
                ///</summary>
                pSrc = p.pSrc;
                //pLeft = pSrc.a[0];
                //pRight = pLeft[1];
                for (i = 0; i < pSrc.nSrc - 1; i++)
                {
                    pLeft = pSrc.a[i];
                    // pLeft ++
                    pRight = pSrc.a[i + 1];
                    //Right++,
                    Table pLeftTab = pLeft.pTab;
                    Table pRightTab = pRight.pTab;
                    bool isOuter;
                    if (NEVER(pLeftTab == null || pRightTab == null))
                        continue;
                    isOuter = (pRight.jointype & JT_OUTER) != 0;
                    ///
                    ///<summary>
                    ///When the NATURAL keyword is present, add WHERE clause terms for
                    ///every column that the two tables have in common.
                    ///
                    ///</summary>
                    if ((pRight.jointype & JT_NATURAL) != 0)
                    {
                        if (pRight.pOn != null || pRight.pUsing != null)
                        {
                            utilc.sqlite3ErrorMsg(pParse, "a NATURAL join may not have " + "an ON or USING clause", "");
                            return 1;
                        }
                        for (j = 0; j < pRightTab.nCol; j++)
                        {
                            string zName;
                            ///
                            ///<summary>
                            ///Name of column in the right table 
                            ///</summary>
                            int iLeft = 0;
                            ///
                            ///<summary>
                            ///Matching left table 
                            ///</summary>
                            int iLeftCol = 0;
                            ///
                            ///<summary>
                            ///Matching column in the left table 
                            ///</summary>
                            zName = pRightTab.aCol[j].zName;
                            int iRightCol = pRightTab.columnIndex(zName);
                            if (SelectMethods.tableAndColumnIndex(pSrc, i + 1, zName, ref iLeft, ref iLeftCol) != 0)
                            {
                                addWhereTerm(pParse, pSrc, iLeft, iLeftCol, i + 1, j, isOuter ? 1 : 0, ref p.pWhere);
                            }
                        }
                    }
                    ///
                    ///<summary>
                    ///Disallow both ON and USING clauses in the same join
                    ///
                    ///</summary>
                    if (pRight.pOn != null && pRight.pUsing != null)
                    {
                        utilc.sqlite3ErrorMsg(pParse, "cannot have both ON and USING " + "clauses in the same join");
                        return 1;
                    }
                    ///
                    ///<summary>
                    ///Add the ON clause to the end of the WHERE clause, connected by
                    ///an AND operator.
                    ///
                    ///</summary>
                    if (pRight.pOn != null)
                    {
                        if (isOuter)
                            setJoinExpr(pRight.pOn, pRight.iCursor);
                        p.pWhere = exprc.sqlite3ExprAnd(pParse.db, p.pWhere, pRight.pOn);
                        pRight.pOn = null;
                    }
                    ///
                    ///<summary>
                    ///Create extra terms on the WHERE clause for each column named
                    ///in the USING clause.  Example: If the two tables to be joined are
                    ///A and B and the USING clause names X, Y, and Z, then add this
                    ///to the WHERE clause:    A.X=B.X AND A.Y=B.Y AND A.Z=B.Z
                    ///Report an error if any column mentioned in the USING clause is
                    ///not contained in both tables to be joined.
                    ///
                    ///</summary>
                    if (pRight.pUsing != null)
                    {
                        IdList pList = pRight.pUsing;
                        for (j = 0; j < pList.nId; j++)
                        {
                            string zName;
                            ///
                            ///<summary>
                            ///Name of the term in the USING clause 
                            ///</summary>
                            int iLeft = 0;
                            ///
                            ///<summary>
                            ///Table on the left with matching column name 
                            ///</summary>
                            int iLeftCol = 0;
                            ///
                            ///<summary>
                            ///Column number of matching column on the left 
                            ///</summary>
                            int iRightCol;
                            ///
                            ///<summary>
                            ///Column number of matching column on the right 
                            ///</summary>
                            zName = pList.a[j].zName;
                            iRightCol = pRightTab.columnIndex(zName);
                            if (iRightCol < 0 || 0 == SelectMethods.tableAndColumnIndex(pSrc, i + 1, zName, ref iLeft, ref iLeftCol))
                            {
                                utilc.sqlite3ErrorMsg(pParse, "cannot join using column %s - column " + "not present in both tables", zName);
                                return 1;
                            }
                            addWhereTerm(pParse, pSrc, iLeft, iLeftCol, i + 1, iRightCol, isOuter ? 1 : 0, ref p.pWhere);
                        }
                    }
                }
                return 0;
            }
            ///<summary>
            /// Insert code into "v" that will push the record on the top of the
            /// stack into the sorter.
            ///
            ///</summary>
            static void pushOntoSorter(Parse pParse,///
                ///Parser context 
            ExprList pOrderBy,///
                ///The ORDER BY clause 
            Select pSelect,///
                ///The whole SELECT statement 
            int regData///
                ///Register holding data to be sorted 
            )
            {
                Vdbe v = pParse.pVdbe;
                int nExpr = pOrderBy.nExpr;
                int regBase = pParse.sqlite3GetTempRange(nExpr + 2);
                int regRecord = pParse.sqlite3GetTempReg();
                pParse.sqlite3ExprCacheClear();
                pParse.sqlite3ExprCodeExprList(pOrderBy, regBase, false);
                v.sqlite3VdbeAddOp2(OP_Sequence, pOrderBy.iECursor, regBase + nExpr);
                pParse.sqlite3ExprCodeMove(regData, regBase + nExpr + 1, 1);
                v.sqlite3VdbeAddOp3(OpCode.OP_MakeRecord, regBase, nExpr + 2, regRecord);
                v.sqlite3VdbeAddOp2(OP_IdxInsert, pOrderBy.iECursor, regRecord);
                pParse.sqlite3ReleaseTempReg(regRecord);
                pParse.sqlite3ReleaseTempRange(regBase, nExpr + 2);
                if (pSelect.iLimit != 0)
                {
                    int addr1, addr2;
                    int iLimit;
                    if (pSelect.iOffset != 0)
                    {
                        iLimit = pSelect.iOffset + 1;
                    }
                    else
                    {
                        iLimit = pSelect.iLimit;
                    }
                    addr1 = v.sqlite3VdbeAddOp1(OpCode.OP_IfZero, iLimit);
                    v.sqlite3VdbeAddOp2(OpCode.OP_AddImm, iLimit, -1);
                    addr2 = v.sqlite3VdbeAddOp0(OpCode.OP_Goto);
                    v.sqlite3VdbeJumpHere(addr1);
                    v.sqlite3VdbeAddOp1(OpCode.OP_Last, pOrderBy.iECursor);
                    v.sqlite3VdbeAddOp1(OpCode.OP_Delete, pOrderBy.iECursor);
                    v.sqlite3VdbeJumpHere(addr2);
                }
            }
            ///<summary>
            /// Add code to implement the OFFSET
            ///
            ///</summary>
            static void codeOffset(Vdbe v,///
                ///Generate code into this VM 
            Select p,///
                ///The SELECT statement being coded 
            int iContinue///
                ///Jump here to skip the current record 
            )
            {
                if (p.iOffset != 0 && iContinue != 0)
                {
                    int addr;
                    v.sqlite3VdbeAddOp2(OpCode.OP_AddImm, p.iOffset, -1);
                    addr = v.sqlite3VdbeAddOp1(OpCode.OP_IfNeg, p.iOffset);
                    v.sqlite3VdbeAddOp2(OpCode.OP_Goto, 0, iContinue);
#if SQLITE_DEBUG
																																																																																																				        VdbeComment( v, "skip OFFSET records" );
#endif
                    v.sqlite3VdbeJumpHere(addr);
                }
            }
            ///<summary>
            /// Add code that will check to make sure the N registers starting at iMem
            /// form a distinct entry.  iTab is a sorting index that holds previously
            /// seen combinations of the N values.  A new entry is made in iTab
            /// if the current N values are new.
            ///
            /// A jump to addrRepeat is made and the N+1 values are popped from the
            /// stack if the top N elements are not distinct.
            ///
            ///</summary>
            static void codeDistinct(Parse pParse,///
                ///<summary>
                ///Parsing and code generating context 
                ///</summary>
            int iTab,///
                ///<summary>
                ///A sorting index used to test for distinctness 
                ///</summary>
            int addrRepeat,///
                ///<summary>
                ///Jump to here if not distinct 
                ///</summary>
            int N,///
                ///<summary>
                ///Number of elements 
                ///</summary>
            int iMem///
                ///<summary>
                ///First element 
                ///</summary>
            )
            {
                Vdbe v;
                int r1;
                v = pParse.pVdbe;
                r1 = pParse.sqlite3GetTempReg();
                v.sqlite3VdbeAddOp4Int(OP_Found, iTab, addrRepeat, iMem, N);
                v.sqlite3VdbeAddOp3(OpCode.OP_MakeRecord, iMem, N, r1);
                v.sqlite3VdbeAddOp2(OP_IdxInsert, iTab, r1);
                pParse.sqlite3ReleaseTempReg(r1);
            }
#if !SQLITE_OMIT_SUBQUERY
            ///<summary>
            /// Generate an error message when a SELECT is used within a subexpression
            /// (example:  "a IN (SELECT * FROM table)") but it has more than 1 result
            /// column.  We do this in a subroutine because the error used to occur
            /// in multiple places.  (The error only occurs in one place now, but we
            /// retain the subroutine to minimize code disruption.)
            ///</summary>
            public static bool checkForMultiColumnSelectError(Parse pParse,///
                ///<summary>
                ///Parse context. 
                ///</summary>
            SelectDest pDest,///
                ///<summary>
                ///Destination of SELECT results 
                ///</summary>
            int nExpr///
                ///<summary>
                ///Number of result columns returned by SELECT 
                ///</summary>
            )
            {
                var eDest = pDest.eDest;
                if (nExpr > 1 && (eDest == SelectResultType.Mem || eDest == SelectResultType.Set))
                {
                    utilc.sqlite3ErrorMsg(pParse, "only a single result allowed for " + "a SELECT that is part of an expression");
                    return true;
                }
                else
                {
                    return false;
                }
            }
#endif
            ///<summary>
            /// This routine generates the code for the inside of the inner loop
            /// of a SELECT.
            ///
            /// If srcTab and nColumn are both zero, then the pEList expressions
            /// are evaluated in order to get the data for this row.  If nColumn>0
            /// then data is pulled from srcTab and pEList is used only to get the
            /// datatypes for each column.
            ///</summary>
            public static void selectInnerLoop(Parse pParse,///
                ///<summary>
                ///The parser context 
                ///</summary>
            Select p,///
                ///<summary>
                ///The complete select statement being coded 
                ///</summary>
            ExprList pEList,///
                ///<summary>
                ///List of values being extracted 
                ///</summary>
            int srcTab,///
                ///<summary>
                ///Pull data from this table 
                ///</summary>
            int nColumn,///
                ///<summary>
                ///Number of columns in the source table 
                ///</summary>
            ExprList pOrderBy,///
                ///<summary>
                ///If not NULL, sort results using this key 
                ///</summary>
            int distinct,///
                ///<summary>
                ///If >=0, make sure results are distinct 
                ///</summary>
            SelectDest pDest,///
                ///<summary>
                ///How to dispose of the results 
                ///</summary>
            int iContinue,///
                ///<summary>
                ///Jump here to continue with next row 
                ///</summary>
            int iBreak///
                ///<summary>
                ///Jump here to break out of the inner loop 
                ///</summary>
            )
            {
                Vdbe v = pParse.pVdbe;
                int i;
                bool hasDistinct;
                ///
                ///<summary>
                ///True if the DISTINCT keyword is present 
                ///</summary>
                int regResult;
                ///
                ///<summary>
                ///Start of memory holding result set 
                ///</summary>
                SelectResultType eDest = pDest.eDest;
                ///
                ///<summary>
                ///How to dispose of results 
                ///</summary>
                int iParm = pDest.iParm;
                ///
                ///<summary>
                ///First argument to disposal method 
                ///</summary>
                int nResultCol;
                ///
                ///<summary>
                ///Number of result columns 
                ///</summary>
                Debug.Assert(v != null);
                if (NEVER(v == null))
                    return;
                Debug.Assert(pEList != null);
                hasDistinct = distinct >= 0;
                if (pOrderBy == null && !hasDistinct)
                {
                    codeOffset(v, p, iContinue);
                }
                ///
                ///<summary>
                ///Pull the requested columns.
                ///
                ///</summary>
                if (nColumn > 0)
                {
                    nResultCol = nColumn;
                }
                else
                {
                    nResultCol = pEList.nExpr;
                }
                if (pDest.iMem == 0)
                {
                    pDest.iMem = pParse.nMem + 1;
                    pDest.nMem = nResultCol;
                    pParse.nMem += nResultCol;
                }
                else
                {
                    Debug.Assert(pDest.nMem == nResultCol);
                }
                regResult = pDest.iMem;
                if (nColumn > 0)
                {
                    for (i = 0; i < nColumn; i++)
                    {
                        v.sqlite3VdbeAddOp3(OpCode.OP_Column, srcTab, i, regResult + i);
                    }
                }
                else
                    if (eDest != SelectResultType.Exists)
                    {
                        ///
                        ///<summary>
                        ///If the destination is an EXISTS(...) expression, the actual
                        ///values returned by the SELECT are not required.
                        ///
                        ///</summary>
                        pParse.sqlite3ExprCacheClear();
                        pParse.sqlite3ExprCodeExprList(pEList, regResult, eDest == SelectResultType.Output);
                    }
                nColumn = nResultCol;
                ///
                ///<summary>
                ///If the DISTINCT keyword was present on the SELECT statement
                ///and this row has been seen before, then do not make this row
                ///part of the result.
                ///
                ///</summary>
                if (hasDistinct)
                {
                    Debug.Assert(pEList != null);
                    Debug.Assert(pEList.nExpr == nColumn);
                    codeDistinct(pParse, distinct, iContinue, nColumn, regResult);
                    if (pOrderBy == null)
                    {
                        codeOffset(v, p, iContinue);
                    }
                }
                switch (eDest)
                {
                    ///
                    ///<summary>
                    ///In this mode, write each query result to the key of the temporary
                    ///table iParm.
                    ///
                    ///</summary>
#if !SQLITE_OMIT_COMPOUND_SELECT
                    case SelectResultType.Union:
                        {
                            int r1;
                            r1 = pParse.sqlite3GetTempReg();
                            v.sqlite3VdbeAddOp3(OpCode.OP_MakeRecord, regResult, nColumn, r1);
                            v.sqlite3VdbeAddOp2(OP_IdxInsert, iParm, r1);
                            pParse.sqlite3ReleaseTempReg(r1);
                            break;
                        }
                    ///
                    ///<summary>
                    ///Construct a record from the query result, but instead of
                    ///saving that record, use it as a key to delete elements from
                    ///the temporary table iParm.
                    ///
                    ///</summary>
                    case SelectResultType.Except:
                        {
                            v.sqlite3VdbeAddOp3(OpCode.OP_IdxDelete, iParm, regResult, nColumn);
                            break;
                        }
#endif
                    ///
                    ///<summary>
                    ///Store the result as data using a unique key.
                    ///</summary>
                    case SelectResultType.Table:
                    case SelectResultType.EphemTab:
                        {
                            int r1 = pParse.sqlite3GetTempReg();
                            testcase(eDest == SelectResultType.Table);
                            testcase(eDest == SelectResultType.EphemTab);
                            v.sqlite3VdbeAddOp3(OpCode.OP_MakeRecord, regResult, nColumn, r1);
                            if (pOrderBy != null)
                            {
                                pushOntoSorter(pParse, pOrderBy, p, r1);
                            }
                            else
                            {
                                int r2 = pParse.sqlite3GetTempReg();
                                v.sqlite3VdbeAddOp2(OP_NewRowid, iParm, r2);
                                v.sqlite3VdbeAddOp3(OpCode.OP_Insert, iParm, r1, r2);
                                v.sqlite3VdbeChangeP5(OPFLAG_APPEND);
                                pParse.sqlite3ReleaseTempReg(r2);
                            }
                            pParse.sqlite3ReleaseTempReg(r1);
                            break;
                        }
#if !SQLITE_OMIT_SUBQUERY
                    ///
                    ///<summary>
                    ///If we are creating a set for an "expr IN (SELECT ...)" construct,
                    ///then there should be a single item on the stack.  Write this
                    ///item into the set table with bogus data.
                    ///</summary>
                    case SelectResultType.Set:
                        {
                            Debug.Assert(nColumn == 1);
                            p.affinity = pEList.a[0].pExpr.sqlite3CompareAffinity(pDest.affinity);
                            if (pOrderBy != null)
                            {
                                ///
                                ///<summary>
                                ///At first glance you would think we could optimize out the
                                ///ORDER BY in this case since the order of entries in the set
                                ///does not matter.  But there might be a LIMIT clause, in which
                                ///case the order does matter 
                                ///</summary>
                                pushOntoSorter(pParse, pOrderBy, p, regResult);
                            }
                            else
                            {
                                int r1 = pParse.sqlite3GetTempReg();
                                v.sqlite3VdbeAddOp4(OP_MakeRecord, regResult, 1, r1, p.affinity, 1);
                                pParse.sqlite3ExprCacheAffinityChange(regResult, 1);
                                v.sqlite3VdbeAddOp2(OP_IdxInsert, iParm, r1);
                                pParse.sqlite3ReleaseTempReg(r1);
                            }
                            break;
                        }
                    ///
                    ///<summary>
                    ///If any row exist in the result set, record that fact and abort.
                    ///
                    ///</summary>
                    case SelectResultType.Exists:
                        {
                            v.sqlite3VdbeAddOp2(OpCode.OP_Integer, 1, iParm);
                            ///
                            ///<summary>
                            ///The LIMIT clause will terminate the loop for us 
                            ///</summary>
                            break;
                        }
                    ///
                    ///<summary>
                    ///If this is a scalar select that is part of an expression, then
                    ///store the results in the appropriate memory cell and break out
                    ///of the scan loop.
                    ///
                    ///</summary>
                    case SelectResultType.Mem:
                        {
                            Debug.Assert(nColumn == 1);
                            if (pOrderBy != null)
                            {
                                pushOntoSorter(pParse, pOrderBy, p, regResult);
                            }
                            else
                            {
                                pParse.sqlite3ExprCodeMove(regResult, iParm, 1);
                                ///
                                ///<summary>
                                ///The LIMIT clause will jump out of the loop for us 
                                ///</summary>
                            }
                            break;
                        }
#endif
                    ///
                    ///<summary>
                    ///Send the data to the callback function or to a subroutine.  In the
                    ///case of a subroutine, the subroutine itself is responsible for
                    ///popping the data from the stack.
                    ///</summary>
                    case SelectResultType.Coroutine:
                    case SelectResultType.Output:
                        {
                            testcase(eDest == SelectResultType.Coroutine);
                            testcase(eDest == SelectResultType.Output);
                            if (pOrderBy != null)
                            {
                                int r1 = pParse.sqlite3GetTempReg();
                                v.sqlite3VdbeAddOp3(OpCode.OP_MakeRecord, regResult, nColumn, r1);
                                pushOntoSorter(pParse, pOrderBy, p, r1);
                                pParse.sqlite3ReleaseTempReg(r1);
                            }
                            else
                                if (eDest == SelectResultType.Coroutine)
                                {
                                    v.sqlite3VdbeAddOp1(OpCode.OP_Yield, pDest.iParm);
                                }
                                else
                                {
                                    v.sqlite3VdbeAddOp2(OpCode.OP_ResultRow, regResult, nColumn);
                                    pParse.sqlite3ExprCacheAffinityChange(regResult, nColumn);
                                }
                            break;
                        }
#if !SQLITE_OMIT_TRIGGER
                    ///
                    ///<summary>
                    ///Discard the results.  This is used for SELECT statements inside
                    ///the body of a TRIGGER.  The purpose of such selects is to call
                    ///</summary>
                    ///<param name="user">defined functions that have side effects.  We do not care</param>
                    ///<param name="about the actual results of the select.">about the actual results of the select.</param>
                    default:
                        {
                            Debug.Assert(eDest == SelectResultType.Discard);
                            break;
                        }
#endif
                }
                ///
                ///<summary>
                ///Jump to the end of the loop if the LIMIT is reached.  Except, if
                ///there is a sorter, in which case the sorter has already limited
                ///the output for us.
                ///
                ///</summary>
                if (pOrderBy == null && p.iLimit != 0)
                {
                    v.sqlite3VdbeAddOp3(OpCode.OP_IfZero, p.iLimit, iBreak, -1);
                }
            }
            ///<summary>
            /// Given an expression list, generate a KeyInfo structure that records
            /// the collating sequence for each expression in that expression list.
            ///
            /// If the ExprList is an ORDER BY or GROUP BY clause then the resulting
            /// KeyInfo structure is appropriate for initializing a virtual index to
            /// implement that clause.  If the ExprList is the result set of a SELECT
            /// then the KeyInfo structure is appropriate for initializing a virtual
            /// index to implement a DISTINCT test.
            ///
            /// Space to hold the KeyInfo structure is obtain from malloc.  The calling
            /// function is responsible for seeing that this structure is eventually
            /// freed.  Add the KeyInfo structure to the P4 field of an opcode using
            /// P4_KEYINFO_HANDOFF is the usual way of dealing with this.
            ///
            ///</summary>
            public static KeyInfo keyInfoFromExprList(Parse pParse, ExprList pList)
            {
                sqlite3 db = pParse.db;
                int nExpr;
                KeyInfo pInfo;
                ExprList_item pItem;
                int i;
                nExpr = pList.nExpr;
                pInfo = new KeyInfo();
                //sqlite3DbMallocZero(db, sizeof(*pInfo) + nExpr*(CollSeq*.Length+1) );
                if (pInfo != null)
                {
                    pInfo.aSortOrder = new byte[nExpr];
                    // pInfo.aColl[nExpr];
                    pInfo.aColl = new CollSeq[nExpr];
                    pInfo.nField = (u16)nExpr;
                    pInfo.enc = db.aDbStatic[0].pSchema.enc;
                    // ENC(db);
                    pInfo.db = db;
                    for (i = 0; i < nExpr; i++)
                    {
                        //, pItem++){
                        pItem = pList.a[i];
                        CollSeq pColl;
                        pColl = pParse.sqlite3ExprCollSeq(pItem.pExpr);
                        if (pColl == null)
                        {
                            pColl = db.pDfltColl;
                        }
                        pInfo.aColl[i] = pColl;
                        pInfo.aSortOrder[i] = (byte)pItem.sortOrder;
                    }
                }
                return pInfo;
            }
#if !SQLITE_OMIT_COMPOUND_SELECT
            ///<summary>
            /// Name of the connection operator, used for error messages.
            ///</summary>
            public static string selectOpName(TokenType id)
            {
                string z;
                switch (id)
                {
                    case TokenType.TK_ALL:
                        z = "UNION ALL";
                        break;
                    case TokenType.TK_INTERSECT:
                        z = "INTERSECT";
                        break;
                    case TokenType.TK_EXCEPT:
                        z = "EXCEPT";
                        break;
                    default:
                        z = "UNION";
                        break;
                }
                return z;
            }
#endif
#if !SQLITE_OMIT_EXPLAIN
            ///
            ///<summary>
            ///Unless an "EXPLAIN QUERY PLAN" command is being processed, this function
            ///</summary>
            ///<param name="is a no">op. Otherwise, it adds a single row of output to the EQP result,</param>
            ///<param name="where the caption is of the form:">where the caption is of the form:</param>
            ///<param name=""></param>
            ///<param name=""USE TEMP B">TREE FOR xxx"</param>
            ///<param name=""></param>
            ///<param name="where xxx is one of "DISTINCT", "ORDER BY" or "GROUP BY". Exactly which">where xxx is one of "DISTINCT", "ORDER BY" or "GROUP BY". Exactly which</param>
            ///<param name="is determined by the zUsage argument.">is determined by the zUsage argument.</param>
            public static void explainTempTable(Parse pParse, string zUsage)
            {
                if (pParse.explain == 2)
                {
                    Vdbe v = pParse.pVdbe;
                    string zMsg = io.sqlite3MPrintf(pParse.db, "USE TEMP B-TREE FOR %s", zUsage);
                    v.sqlite3VdbeAddOp4(OpCode.OP_Explain, pParse.iSelectId, 0, 0, zMsg, P4_DYNAMIC);
                }
            }
            ///<summary>
            /// Assign expression b to lvalue a. A second, no-op, version of this macro
            /// is provided when SQLITE_OMIT_EXPLAIN is defined. This allows the code
            /// in Select.sqlite3Select() to assign values to structure member variables that
            /// only exist if SQLITE_OMIT_EXPLAIN is not defined without polluting the
            /// code with #if !directives.
            ///</summary>
            //# define SelectMethods.explainSetInteger(a, b) a = b
            public static void explainSetInteger(ref int a, int b)
            {
                a = b;
            }
            public static void explainSetInteger(ref byte a, int b)
            {
                a = (byte)b;
            }
#else
																																																		/* No-op versions of the explainXXX() functions and macros. */
// define SelectMethods.explainTempTable(y,z)
static void SelectMethods.explainTempTable(ref int a, int b){ a = b;}

// define SelectMethods.explainSetInteger(y,z)
static void SelectMethods.explainSetInteger(ref int a, int b){ a = b;}
#endif
#if !(SQLITE_OMIT_EXPLAIN) && !(SQLITE_OMIT_COMPOUND_SELECT)
            ///<summary>
            /// Unless an "EXPLAIN QUERY PLAN" command is being processed, this function
            /// is a no-op. Otherwise, it adds a single row of output to the EQP result,
            /// where the caption is of one of the two forms:
            ///
            ///   "COMPOSITE SUBQUERIES iSub1 and iSub2 (op)"
            ///   "COMPOSITE SUBQUERIES iSub1 and iSub2 USING TEMP B-TREE (op)"
            ///
            /// where iSub1 and iSub2 are the integers passed as the corresponding
            /// function parameters, and op is the text representation of the parameter
            /// of the same name. The parameter "op" must be one of TK_UNION, TK_EXCEPT,
            /// TK_INTERSECT or TK_ALL. The first form is used if argument bUseTmp is
            /// false, or the second form if it is true.
            ///
            ///</summary>
            public static void explainComposite(Parse pParse,///
                ///<summary>
                ///Parse context 
                ///</summary>
            int op,///
                ///<summary>
                ///One of TK_UNION, TK_EXCEPT etc. 
                ///</summary>
            int iSub1,///
                ///<summary>
                ///Subquery id 1 
                ///</summary>
            int iSub2,///
                ///<summary>
                ///Subquery id 2 
                ///</summary>
            bool bUseTmp///
                ///<summary>
                ///True if a temp table was used 
                ///</summary>
            )
            {
                Debug.Assert(op == TK_UNION || op == TK_EXCEPT || op == TK_INTERSECT || op == TK_ALL);
                if (pParse.explain == 2)
                {
                    Vdbe v = pParse.pVdbe;
                    string zMsg = io.sqlite3MPrintf(pParse.db, "COMPOUND SUBQUERIES %d AND %d %s(%s)", iSub1, iSub2, bUseTmp ? "USING TEMP B-TREE " : "", SelectMethods.selectOpName((TokenType)op));
                    v.sqlite3VdbeAddOp4(OpCode.OP_Explain, pParse.iSelectId, 0, 0, zMsg, P4_DYNAMIC);
                }
            }
#else
																																																		/* No-op versions of the explainXXX() functions and macros. */
// define SelectMethods.explainComposite(v,w,x,y,z)
static void SelectMethods.explainComposite(Parse v, int w,int x,int y,bool z) {}
#endif
            ///<summary>
            /// If the inner loop was generated using a non-null pOrderBy argument,
            /// then the results were placed in a sorter.  After the loop is terminated
            /// we need to run the sorter and output the results.  The following
            /// routine generates the code needed to do that.
            ///</summary>
            public static void generateSortTail(Parse pParse,///
                ///<summary>
                ///Parsing context 
                ///</summary>
            Select p,///
                ///<summary>
                ///The SELECT statement 
                ///</summary>
            Vdbe v,///
                ///<summary>
                ///Generate code into this VDBE 
                ///</summary>
            int nColumn,///
                ///<summary>
                ///Number of columns of data 
                ///</summary>
            SelectDest pDest///
                ///<summary>
                ///Write the sorted results here 
                ///</summary>
            )
            {
                int addrBreak = v.sqlite3VdbeMakeLabel();
                ///
                ///<summary>
                ///Jump here to exit loop 
                ///</summary>
                int addrContinue = v.sqlite3VdbeMakeLabel();
                ///
                ///<summary>
                ///Jump here for next cycle 
                ///</summary>
                int addr;
                int iTab;
                int pseudoTab = 0;
                ExprList pOrderBy = p.pOrderBy;
                var eDest = pDest.eDest;
                int iParm = pDest.iParm;
                int regRow;
                int regRowid;
                iTab = pOrderBy.iECursor;
                regRow = pParse.sqlite3GetTempReg();
                if (eDest == SelectResultType.Output || eDest == SelectResultType.Coroutine)
                {
                    pseudoTab = pParse.nTab++;
                    v.sqlite3VdbeAddOp3(OpCode.OP_OpenPseudo, pseudoTab, regRow, nColumn);
                    regRowid = 0;
                }
                else
                {
                    regRowid = pParse.sqlite3GetTempReg();
                }
                addr = 1 + v.sqlite3VdbeAddOp2(OP_Sort, iTab, addrBreak);
                codeOffset(v, p, addrContinue);
                v.sqlite3VdbeAddOp3(OpCode.OP_Column, iTab, pOrderBy.nExpr + 1, regRow);
                switch (eDest)
                {
                    case SelectResultType.Table:
                    case SelectResultType.EphemTab:
                        {
                            testcase(eDest == SelectResultType.Table);
                            testcase(eDest == SelectResultType.EphemTab);
                            v.sqlite3VdbeAddOp2(OP_NewRowid, iParm, regRowid);
                            v.sqlite3VdbeAddOp3(OpCode.OP_Insert, iParm, regRow, regRowid);
                            v.sqlite3VdbeChangeP5(OPFLAG_APPEND);
                            break;
                        }
#if !SQLITE_OMIT_SUBQUERY
                    case SelectResultType.Set:
                        {
                            Debug.Assert(nColumn == 1);
                            v.sqlite3VdbeAddOp4(OP_MakeRecord, regRow, 1, regRowid, p.affinity, 1);
                            pParse.sqlite3ExprCacheAffinityChange(regRow, 1);
                            v.sqlite3VdbeAddOp2(OP_IdxInsert, iParm, regRowid);
                            break;
                        }
                    case SelectResultType.Mem:
                        {
                            Debug.Assert(nColumn == 1);
                            pParse.sqlite3ExprCodeMove(regRow, iParm, 1);
                            ///
                            ///<summary>
                            ///The LIMIT clause will terminate the loop for us 
                            ///</summary>
                            break;
                        }
#endif
                    default:
                        {
                            int i;
                            Debug.Assert(eDest == SelectResultType.Output || eDest == SelectResultType.Coroutine);
                            testcase(eDest == SelectResultType.Output);
                            testcase(eDest == SelectResultType.Coroutine);
                            for (i = 0; i < nColumn; i++)
                            {
                                Debug.Assert(regRow != pDest.iMem + i);
                                v.sqlite3VdbeAddOp3(OpCode.OP_Column, pseudoTab, i, pDest.iMem + i);
                                if (i == 0)
                                {
                                    v.sqlite3VdbeChangeP5(OPFLAG_CLEARCACHE);
                                }
                            }
                            if (eDest == SelectResultType.Output)
                            {
                                v.sqlite3VdbeAddOp2(OpCode.OP_ResultRow, pDest.iMem, nColumn);
                                pParse.sqlite3ExprCacheAffinityChange(pDest.iMem, nColumn);
                            }
                            else
                            {
                                v.sqlite3VdbeAddOp1(OpCode.OP_Yield, pDest.iParm);
                            }
                            break;
                        }
                }
                pParse.sqlite3ReleaseTempReg(regRow);
                pParse.sqlite3ReleaseTempReg(regRowid);
                ///
                ///<summary>
                ///The bottom of the loop
                ///
                ///</summary>
                v.sqlite3VdbeResolveLabel(addrContinue);
                v.sqlite3VdbeAddOp2(OP_Next, iTab, addr);
                v.sqlite3VdbeResolveLabel(addrBreak);
                if (eDest == SelectResultType.Output || eDest == SelectResultType.Coroutine)
                {
                    v.sqlite3VdbeAddOp2(OP_Close, pseudoTab, 0);
                }
            }







            ///<summary>
            /// Generate code that will tell the VDBE the declaration types of columns
            /// in the result set.
            ///
            ///</summary>
            static void generateColumnTypes(Parse pParse,///
                ///<summary>
                ///Parser context 
                ///</summary>
            SrcList pTabList,///
                ///<summary>
                ///List of tables 
                ///</summary>
            ExprList pEList///
                ///<summary>
                ///Expressions defining the result set 
                ///</summary>
            )
            {
#if !SQLITE_OMIT_DECLTYPE
                Vdbe v = pParse.pVdbe;
                int i;
                NameContext sNC = new NameContext();
                sNC.pSrcList = pTabList;
                sNC.pParse = pParse;
                for (i = 0; i < pEList.nExpr; i++)
                {
                    Expr expr = pEList.a[i].pExpr;
                    string zType;
#if SQLITE_ENABLE_COLUMN_METADATA
																																																																																																				        string zOrigDb = null;
        string zOrigTab = null;
        string zOrigCol = null;
        zType = columnType( sNC, p, ref zOrigDb, ref zOrigTab, ref zOrigCol );

        /* The vdbe must make its own copy of the column-type and other
        ** column specific strings, in case the schema is reset before this
        ** virtual machine is deleted.
        */
        sqlite3VdbeSetColName( v, i, COLNAME_DATABASE, zOrigDb, SQLITE_TRANSIENT );
        sqlite3VdbeSetColName( v, i, COLNAME_TABLE, zOrigTab, SQLITE_TRANSIENT );
        sqlite3VdbeSetColName( v, i, COLNAME_COLUMN, zOrigCol, SQLITE_TRANSIENT );
#else
                    string sDummy = null;
                    zType = expr.columnType(sNC, ref sDummy, ref sDummy, ref sDummy);
#endif
                    v.sqlite3VdbeSetColName(i, COLNAME_DECLTYPE, zType, SQLITE_TRANSIENT);
                }
#endif
            }
            ///<summary>
            /// Generate code that will tell the VDBE the names of columns
            /// in the result set.  This information is used to provide the
            /// azCol[] values in the callback.
            ///
            ///</summary>
            public static void generateColumnNames(Parse pParse,///
                ///<summary>
                ///Parser context 
                ///</summary>
            SrcList pTabList,///
                ///<summary>
                ///List of tables 
                ///</summary>
            ExprList pEList///
                ///<summary>
                ///Expressions defining the result set 
                ///</summary>
            )
            {
                Vdbe v = pParse.pVdbe;
                int i, j;
                sqlite3 db = pParse.db;
                bool fullNames;
                bool shortNames;
#if !SQLITE_OMIT_EXPLAIN
                ///
                ///<summary>
                ///If this is an EXPLAIN, skip this step 
                ///</summary>
                if (pParse.explain != 0)
                {
                    return;
                }
#endif
                if (pParse.colNamesSet != 0 || NEVER(v == null)///
                    ///<summary>
                    ///|| db.mallocFailed != 0 
                    ///</summary>
                )
                    return;
                pParse.colNamesSet = 1;
                fullNames = (db.flags & SQLITE_FullColNames) != 0;
                shortNames = (db.flags & SQLITE_ShortColNames) != 0;
                v.sqlite3VdbeSetNumCols(pEList.nExpr);
                for (i = 0; i < pEList.nExpr; i++)
                {
                    Expr p;
                    p = pEList.a[i].pExpr;
                    if (NEVER(p == null))
                        continue;
                    if (pEList.a[i].zName != null)
                    {
                        string zName = pEList.a[i].zName;
                        v.sqlite3VdbeSetColName(i, COLNAME_NAME, zName, SQLITE_TRANSIENT);
                    }
                    else
                        if ((p.op == TK_COLUMN || p.op == TK_AGG_COLUMN) && pTabList != null)
                        {
                            Table pTab;
                            string zCol;
                            int iCol = p.iColumn;
                            for (j = 0; ALWAYS(j < pTabList.nSrc); j++)
                            {
                                if (pTabList.a[j].iCursor == p.iTable)
                                    break;
                            }
                            Debug.Assert(j < pTabList.nSrc);
                            pTab = pTabList.a[j].pTab;
                            if (iCol < 0)
                                iCol = pTab.iPKey;
                            Debug.Assert(iCol == -1 || (iCol >= 0 && iCol < pTab.nCol));
                            if (iCol < 0)
                            {
                                zCol = "rowid";
                            }
                            else
                            {
                                zCol = pTab.aCol[iCol].zName;
                            }
                            if (!shortNames && !fullNames)
                            {
                                v.sqlite3VdbeSetColName(i, COLNAME_NAME, pEList.a[i].zSpan, SQLITE_DYNAMIC);
                                //sqlite3DbStrDup(db, pEList->a[i].zSpan), SQLITE_DYNAMIC);
                            }
                            else
                                if (fullNames)
                                {
                                    string zName;
                                    zName = io.sqlite3MPrintf(db, "%s.%s", pTab.zName, zCol);
                                    v.sqlite3VdbeSetColName(i, COLNAME_NAME, zName, SQLITE_DYNAMIC);
                                }
                                else
                                {
                                    v.sqlite3VdbeSetColName(i, COLNAME_NAME, zCol, SQLITE_TRANSIENT);
                                }
                        }
                        else
                        {
                            v.sqlite3VdbeSetColName(i, COLNAME_NAME, pEList.a[i].zSpan, SQLITE_DYNAMIC);
                            //sqlite3DbStrDup(db, pEList->a[i].zSpan), SQLITE_DYNAMIC);
                        }
                }
                generateColumnTypes(pParse, pTabList, pEList);
            }
            ///<summary>
            /// Given a an expression list (which is really the list of expressions
            /// that form the result set of a SELECT statement) compute appropriate
            /// column names for a table that would hold the expression list.
            ///
            /// All column names will be unique.
            ///
            /// Only the column names are computed.  Column.zType, Column.zColl,
            /// and other fields of Column are zeroed.
            ///
            /// Return SQLITE_OK on success.  If a memory allocation error occurs,
            /// store NULL in paCol and 0 in pnCol and return SQLITE_NOMEM.
            ///
            ///</summary>
            public static int selectColumnsFromExprList(Parse pParse,///
                ///<summary>
                ///Parsing context 
                ///</summary>
            ExprList pEList,///
                ///<summary>
                ///Expr list from which to derive column names 
                ///</summary>
            ref int pnCol,///
                ///<summary>
                ///Write the number of columns here 
                ///</summary>
            ref Column[] paCol///
                ///<summary>
                ///Write the new column list here 
                ///</summary>
            )
            {
                sqlite3 db = pParse.db;
                ///
                ///<summary>
                ///Database connection 
                ///</summary>
                int i, j;
                ///
                ///<summary>
                ///Loop counters 
                ///</summary>
                int cnt;
                ///
                ///<summary>
                ///Index added to make the name unique 
                ///</summary>
                Column[] aCol;
                Column pCol;
                ///
                ///<summary>
                ///For looping over result columns 
                ///</summary>
                int nCol;
                ///
                ///<summary>
                ///Number of columns in the result set 
                ///</summary>
                Expr p;
                ///
                ///<summary>
                ///Expression for a single result column 
                ///</summary>
                string zName;
                ///
                ///<summary>
                ///Column name 
                ///</summary>
                int nName;
                ///
                ///<summary>
                ///Size of name in zName[] 
                ///</summary>
                pnCol = nCol = pEList.nExpr;
                aCol = paCol = new Column[nCol];
                //sqlite3DbMallocZero(db, sizeof(aCol[0])*nCol);
                //if ( aCol == null )
                //  return SQLITE_NOMEM;
                for (i = 0; i < nCol; i++)//, pCol++)
                {
                    if (aCol[i] == null)
                        aCol[i] = new Column();
                    pCol = aCol[i];
                    ///
                    ///<summary>
                    ///Get an appropriate name for the column
                    ///
                    ///</summary>
                    p = pEList.a[i].pExpr;
                    Debug.Assert(p.pRight == null || p.pRight.ExprHasProperty(EP_IntValue) || p.pRight.u.zToken == null || p.pRight.u.zToken.Length > 0);
                    if (pEList.a[i].zName != null && (zName = pEList.a[i].zName) != "")
                    {
                        ///
                        ///<summary>
                        ///If the column contains an "AS <name>" phrase, use <name> as the name 
                        ///</summary>
                        //zName = sqlite3DbStrDup(db, zName);
                    }
                    else
                    {
                        Expr pColExpr = p;
                        ///
                        ///<summary>
                        ///The expression that is the result column name 
                        ///</summary>
                        Table pTab;
                        ///
                        ///<summary>
                        ///Table associated with this expression 
                        ///</summary>
                        while (pColExpr.op == TK_DOT)
                            pColExpr = pColExpr.pRight;
                        if (pColExpr.op == TK_COLUMN && ALWAYS(pColExpr.pTab != null))
                        {
                            ///
                            ///<summary>
                            ///For columns use the column name name 
                            ///</summary>
                            int iCol = pColExpr.iColumn;
                            pTab = pColExpr.pTab;
                            if (iCol < 0)
                                iCol = pTab.iPKey;
                            zName = io.sqlite3MPrintf(db, "%s", iCol >= 0 ? pTab.aCol[iCol].zName : "rowid");
                        }
                        else
                            if (pColExpr.op == TK_ID)
                            {
                                Debug.Assert(!pColExpr.ExprHasProperty(EP_IntValue));
                                zName = io.sqlite3MPrintf(db, "%s", pColExpr.u.zToken);
                            }
                            else
                            {
                                ///
                                ///<summary>
                                ///Use the original text of the column expression as its name 
                                ///</summary>
                                zName = io.sqlite3MPrintf(db, "%s", pEList.a[i].zSpan);
                            }
                    }
                    //if ( db.mallocFailed != 0 )
                    //{
                    //  sqlite3DbFree( db, ref zName );
                    //  break;
                    //}
                    ///
                    ///<summary>
                    ///Make sure the column name is unique.  If the name is not unique,
                    ///append a integer to the name so that it becomes unique.
                    ///
                    ///</summary>
                    nName = StringExtensions.sqlite3Strlen30(zName);
                    for (j = cnt = 0; j < i; j++)
                    {
                        if (aCol[j].zName.Equals(zName, StringComparison.InvariantCultureIgnoreCase))
                        {
                            string zNewName;
                            //zName[nName] = 0;
                            zNewName = io.sqlite3MPrintf(db, "%s:%d", zName.Substring(0, nName), ++cnt);
                            db.sqlite3DbFree(ref zName);
                            zName = zNewName;
                            j = -1;
                            if (zName == "")
                                break;
                        }
                    }
                    pCol.zName = zName;
                }
                //if ( db.mallocFailed != 0 )
                //{
                //  for ( j = 0 ; j < i ; j++ )
                //  {
                //    sqlite3DbFree( db, aCol[j].zName );
                //  }
                //  sqlite3DbFree( db, aCol );
                //  paCol = null;
                //  pnCol = 0;
                //  return SQLITE_NOMEM;
                //}
                return SQLITE_OK;
            }

            ///<summary>
            /// Given a SELECT statement, generate a Table structure that describes
            /// the result set of that SELECT.
            ///
            ///</summary>
            public static Table sqlite3ResultSetOfSelect(Parse pParse, Select pSelect)
            {
                Table pTab;
                sqlite3 db = pParse.db;
                int savedFlags;
                savedFlags = db.flags;
                db.flags &= ~SQLITE_FullColNames;
                db.flags |= SQLITE_ShortColNames;
                Select.sqlite3SelectPrep(pParse, pSelect, null);
                if (pParse.nErr != 0)
                    return null;
                while (pSelect.pPrior != null)
                    pSelect = pSelect.pPrior;
                db.flags = savedFlags;
                pTab = new Table();
                // sqlite3DbMallocZero( db, sizeof( Table ) );
                if (pTab == null)
                {
                    return null;
                }
                ///
                ///<summary>
                ///The SelectMethods.sqlite3ResultSetOfSelect() is only used n contexts where lookaside
                ///is disabled 
                ///</summary>
                Debug.Assert(db.lookaside.bEnabled == 0);
                pTab.nRef = 1;
                pTab.zName = null;
                pTab.nRowEst = 1000000;
                selectColumnsFromExprList(pParse, pSelect.pEList, ref pTab.nCol, ref pTab.aCol);
                Select.selectAddColumnTypeAndCollation(pParse, pTab.nCol, pTab.aCol, pSelect);
                pTab.iPKey = -1;
                //if ( db.mallocFailed != 0 )
                //{
                //  build.sqlite3DeleteTable(db, ref pTab );
                //  return null;
                //}
                return pTab;
            }

            ///<summary>
            /// Compute the iLimit and iOffset fields of the SELECT based on the
            /// pLimit and pOffset expressions.  pLimit and pOffset hold the expressions
            /// that appear in the original SQL statement after the LIMIT and OFFSET
            /// keywords.  Or NULL if those keywords are omitted. iLimit and iOffset
            /// are the integer memory register numbers for counters used to compute
            /// the limit and offset.  If there is no limit and/or offset, then
            /// iLimit and iOffset are negative.
            ///
            /// This routine changes the values of iLimit and iOffset only if
            /// a limit or offset is defined by pLimit and pOffset.  iLimit and
            /// iOffset should have been preset to appropriate default values
            /// (usually but not always -1) prior to calling this routine.
            /// Only if pLimit!=0 or pOffset!=0 do the limit registers get
            /// redefined.  The UNION ALL operator uses this property to force
            /// the reuse of the same limit and offset registers across multiple
            /// SELECT statements.
            ///
            ///</summary>
            public static void computeLimitRegisters(Parse pParse, Select p, int iBreak)
            {
                Vdbe v = null;
                int iLimit = 0;
                int iOffset;
                int addr1, n = 0;
                if (p.iLimit != 0)
                    return;
                ///
                ///<summary>
                ///</summary>
                ///<param name=""LIMIT ">1" always shows all rows.  There is some</param>
                ///<param name="contraversy about what the correct behavior should be.">contraversy about what the correct behavior should be.</param>
                ///<param name="The current implementation interprets "LIMIT 0" to mean">The current implementation interprets "LIMIT 0" to mean</param>
                ///<param name="no rows.">no rows.</param>
                ///<param name=""></param>
                pParse.sqlite3ExprCacheClear();
                Debug.Assert(p.pOffset == null || p.pLimit != null);
                if (p.pLimit != null)
                {
                    p.iLimit = iLimit = ++pParse.nMem;
                    v = pParse.sqlite3GetVdbe();
                    if (NEVER(v == null))
                        return;
                    ///
                    ///<summary>
                    ///VDBE should have already been allocated 
                    ///</summary>
                    if (p.pLimit.sqlite3ExprIsInteger(ref n) != 0)
                    {
                        v.sqlite3VdbeAddOp2(OpCode.OP_Integer, n, iLimit);
                        VdbeComment(v, "LIMIT counter");
                        if (n == 0)
                        {
                            v.sqlite3VdbeAddOp2(OpCode.OP_Goto, 0, iBreak);
                        }
                        else
                        {
                            if (p.nSelectRow > (double)n)
                                p.nSelectRow = (double)n;
                        }
                    }
                    else
                    {
                        pParse.sqlite3ExprCode(p.pLimit, iLimit);
                        v.sqlite3VdbeAddOp1(OpCode.OP_MustBeInt, iLimit);
#if SQLITE_DEBUG
																																																																																																																													          VdbeComment( v, "LIMIT counter" );
#endif
                        v.sqlite3VdbeAddOp2(OP_IfZero, iLimit, iBreak);
                    }
                    if (p.pOffset != null)
                    {
                        p.iOffset = iOffset = ++pParse.nMem;
                        pParse.nMem++;
                        ///
                        ///<summary>
                        ///Allocate an extra register for limit+offset 
                        ///</summary>
                        pParse.sqlite3ExprCode(p.pOffset, iOffset);
                        v.sqlite3VdbeAddOp1(OpCode.OP_MustBeInt, iOffset);
#if SQLITE_DEBUG
																																																																																																																													          VdbeComment( v, "OFFSET counter" );
#endif
                        addr1 = v.sqlite3VdbeAddOp1(OpCode.OP_IfPos, iOffset);
                        v.sqlite3VdbeAddOp2(OpCode.OP_Integer, 0, iOffset);
                        v.sqlite3VdbeJumpHere(addr1);
                        v.sqlite3VdbeAddOp3(OpCode.OP_Add, iLimit, iOffset, iOffset + 1);
#if SQLITE_DEBUG
																																																																																																																													          VdbeComment( v, "LIMIT+OFFSET" );
#endif
                        addr1 = v.sqlite3VdbeAddOp1(OpCode.OP_IfPos, iLimit);
                        v.sqlite3VdbeAddOp2(OpCode.OP_Integer, -1, iOffset + 1);
                        v.sqlite3VdbeJumpHere(addr1);
                    }
                }
            }
#if !SQLITE_OMIT_COMPOUND_SELECT
            ///
            ///<summary>
            ///</summary>
            ///<param name="Return the appropriate collating sequence for the iCol">th column of</param>
            ///<param name="the result set for the compound">select statement "p".  Return NULL if</param>
            ///<param name="the column has no default collating sequence.">the column has no default collating sequence.</param>
            ///<param name=""></param>
            ///<param name="The collating sequence for the compound select is taken from the">The collating sequence for the compound select is taken from the</param>
            ///<param name="left">most term of the select that has a collating sequence.</param>
            public static CollSeq multiSelectCollSeq(Parse pParse, Select p, int iCol)
            {
                CollSeq pRet;
                if (p.pPrior != null)
                {
                    pRet = multiSelectCollSeq(pParse, p.pPrior, iCol);
                }
                else
                {
                    pRet = null;
                }
                Debug.Assert(iCol >= 0);
                if (pRet == null && iCol < p.pEList.nExpr)
                {
                    pRet = pParse.sqlite3ExprCollSeq(p.pEList.a[iCol].pExpr);
                }
                return pRet;
            }
#endif
            ///<summary>
            ///Forward reference
            ///</summary>
            //static int multiSelectOrderBy(
            //  Parse* pParse,        /* Parsing context */
            //  Select* p,            /* The right-most of SELECTs to be coded */
            //  SelectDest* pDest     /* What to do with query results */
            //);

            ///
            ///<summary>
            ///Code an output subroutine for a coroutine implementation of a
            ///SELECT statment.
            ///
            ///The data to be output is contained in pIn.iMem.  There are
            ///pIn.nMem columns to be output.  pDest is where the output should
            ///be sent.
            ///
            ///regReturn is the number of the register holding the subroutine
            ///return address.
            ///
            ///If regPrev>0 then it is the first register in a vector that
            ///records the previous output.  mem[regPrev] is a flag that is false
            ///if there has been no previous output.  If regPrev>0 then code is
            ///generated to suppress duplicates.  pKeyInfo is used for comparing
            ///keys.
            ///
            ///If the LIMIT found in p.iLimit is reached, jump immediately to
            ///iBreak.
            ///</summary>
            static int generateOutputSubroutine(Parse pParse,///
                ///<summary>
                ///Parsing context 
                ///</summary>
            Select p,///
                ///<summary>
                ///The SELECT statement 
                ///</summary>
            SelectDest pIn,///
                ///<summary>
                ///Coroutine supplying data 
                ///</summary>
            SelectDest pDest,///
                ///<summary>
                ///Where to send the data 
                ///</summary>
            int regReturn,///
                ///<summary>
                ///The return address register 
                ///</summary>
            int regPrev,///
                ///<summary>
                ///Previous result register.  No uniqueness if 0 
                ///</summary>
            KeyInfo pKeyInfo,///
                ///<summary>
                ///For comparing with previous entry 
                ///</summary>
            int p4type,///
                ///<summary>
                ///The p4 type for pKeyInfo 
                ///</summary>
            int iBreak///
                ///<summary>
                ///Jump here if we hit the LIMIT 
                ///</summary>
            )
            {
                Vdbe v = pParse.pVdbe;
                int iContinue;
                int addr;
                addr = v.sqlite3VdbeCurrentAddr();
                iContinue = v.sqlite3VdbeMakeLabel();
                ///
                ///<summary>
                ///Suppress duplicates for UNION, EXCEPT, and INTERSECT
                ///
                ///</summary>
                if (regPrev != 0)
                {
                    int j1, j2;
                    j1 = v.sqlite3VdbeAddOp1(OpCode.OP_IfNot, regPrev);
                    j2 = v.sqlite3VdbeAddOp4(OpCode.OP_Compare, pIn.iMem, regPrev + 1, pIn.nMem, pKeyInfo, p4type);
                    v.sqlite3VdbeAddOp3(OpCode.OP_Jump, j2 + 2, iContinue, j2 + 2);
                    v.sqlite3VdbeJumpHere(j1);
                    pParse.sqlite3ExprCodeCopy(pIn.iMem, regPrev + 1, pIn.nMem);
                    v.sqlite3VdbeAddOp2(OpCode.OP_Integer, 1, regPrev);
                }
                //if ( pParse.db.mallocFailed != 0 ) return 0;
                ///
                ///<summary>
                ///Suppress the the first OFFSET entries if there is an OFFSET clause
                ///
                ///</summary>
                codeOffset(v, p, iContinue);
                switch (pDest.eDest)
                {
                    ///
                    ///<summary>
                    ///Store the result as data using a unique key.
                    ///
                    ///</summary>
                    case SelectResultType.Table:
                    case SelectResultType.EphemTab:
                        {
                            int r1 = pParse.sqlite3GetTempReg();
                            int r2 = pParse.sqlite3GetTempReg();
                            testcase(pDest.eDest == SelectResultType.Table);
                            testcase(pDest.eDest == SelectResultType.EphemTab);
                            v.sqlite3VdbeAddOp3(OpCode.OP_MakeRecord, pIn.iMem, pIn.nMem, r1);
                            v.sqlite3VdbeAddOp2(OP_NewRowid, pDest.iParm, r2);
                            v.sqlite3VdbeAddOp3(OpCode.OP_Insert, pDest.iParm, r1, r2);
                            v.sqlite3VdbeChangeP5(OPFLAG_APPEND);
                            pParse.sqlite3ReleaseTempReg(r2);
                            pParse.sqlite3ReleaseTempReg(r1);
                            break;
                        }
#if !SQLITE_OMIT_SUBQUERY
                    ///
                    ///<summary>
                    ///If we are creating a set for an "expr IN (SELECT ...)" construct,
                    ///then there should be a single item on the stack.  Write this
                    ///item into the set table with bogus data.
                    ///</summary>
                    case SelectResultType.Set:
                        {
                            int r1;
                            Debug.Assert(pIn.nMem == 1);
                            p.affinity = p.pEList.a[0].pExpr.sqlite3CompareAffinity(pDest.affinity);
                            r1 = pParse.sqlite3GetTempReg();
                            v.sqlite3VdbeAddOp4(OP_MakeRecord, pIn.iMem, 1, r1, p.affinity, 1);
                            pParse.sqlite3ExprCacheAffinityChange(pIn.iMem, 1);
                            v.sqlite3VdbeAddOp2(OP_IdxInsert, pDest.iParm, r1);
                            pParse.sqlite3ReleaseTempReg(r1);
                            break;
                        }
#if FALSE
																																																																											/* If any row exist in the result set, record that fact and abort.
*/
case SelectResultType.Exists: {
sqlite3VdbeAddOp2(v, OpCode.OP_Integer, 1, pDest.iParm);
/* The LIMIT clause will terminate the loop for us */
break;
}
#endif
                    ///
                    ///<summary>
                    ///If this is a scalar select that is part of an expression, then
                    ///store the results in the appropriate memory cell and break out
                    ///of the scan loop.
                    ///</summary>
                    case SelectResultType.Mem:
                        {
                            Debug.Assert(pIn.nMem == 1);
                            pParse.sqlite3ExprCodeMove(pIn.iMem, pDest.iParm, 1);
                            ///
                            ///<summary>
                            ///The LIMIT clause will jump out of the loop for us 
                            ///</summary>
                            break;
                        }
#endif
                    ///
                    ///<summary>
                    ///The results are stored in a sequence of registers
                    ///</summary>
                    ///<param name="starting at pDest.iMem.  Then the co">routine yields.</param>
                    case SelectResultType.Coroutine:
                        {
                            if (pDest.iMem == 0)
                            {
                                pDest.iMem = pParse.sqlite3GetTempRange(pIn.nMem);
                                pDest.nMem = pIn.nMem;
                            }
                            pParse.sqlite3ExprCodeMove(pIn.iMem, pDest.iMem, pDest.nMem);
                            v.sqlite3VdbeAddOp1(OpCode.OP_Yield, pDest.iParm);
                            break;
                        }
                    ///
                    ///<summary>
                    ///If none of the above, then the result destination must be
                    ///SelectResultType.Output.  This routine is never called with any other
                    ///destination other than the ones handled above or SelectResultType.Output.
                    ///
                    ///For SelectResultType.Output, results are stored in a sequence of registers.
                    ///Then the OpCode.OP_ResultRow opcode is used to cause sqlite3_step() to
                    ///return the next row of result.
                    ///
                    ///</summary>
                    default:
                        {
                            Debug.Assert(pDest.eDest == SelectResultType.Output);
                            v.sqlite3VdbeAddOp2(OpCode.OP_ResultRow, pIn.iMem, pIn.nMem);
                            pParse.sqlite3ExprCacheAffinityChange(pIn.iMem, pIn.nMem);
                            break;
                        }
                }
                ///
                ///<summary>
                ///Jump to the end of the loop if the LIMIT is reached.
                ///
                ///</summary>
                if (p.iLimit != 0)
                {
                    v.sqlite3VdbeAddOp3(OpCode.OP_IfZero, p.iLimit, iBreak, -1);
                }
                ///
                ///<summary>
                ///Generate the subroutine return
                ///
                ///</summary>
                v.sqlite3VdbeResolveLabel(iContinue);
                v.sqlite3VdbeAddOp1(OpCode.OP_Return, regReturn);
                return addr;
            }
            ///<summary>
            /// Alternative compound select code generator for cases when there
            /// is an ORDER BY clause.
            ///
            /// We assume a query of the following form:
            ///
            ///      <selectA>  <operator>  <selectB>  ORDER BY <orderbylist>
            ///
            /// <operator> is one of UNION ALL, UNION, EXCEPT, or INTERSECT.  The idea
            /// is to code both <selectA> and <selectB> with the ORDER BY clause as
            /// co-routines.  Then run the co-routines in parallel and merge the results
            /// into the output.  In addition to the two coroutines (called selectA and
            /// selectB) there are 7 subroutines:
            ///
            ///    outA:    Move the output of the selectA coroutine into the output
            ///             of the compound query.
            ///
            ///    outB:    Move the output of the selectB coroutine into the output
            ///             of the compound query.  (Only generated for UNION and
            ///             UNION ALL.  EXCEPT and INSERTSECT never output a row that
            ///             appears only in B.)
            ///
            ///    AltB:    Called when there is data from both coroutines and A<B.
            ///
            ///    AeqB:    Called when there is data from both coroutines and A==B.
            ///
            ///    AgtB:    Called when there is data from both coroutines and A>B.
            ///
            ///    EofA:    Called when data is exhausted from selectA.
            ///
            ///    EofB:    Called when data is exhausted from selectB.
            ///
            /// The implementation of the latter five subroutines depend on which
            /// <operator> is used:
            ///
            ///
            ///             UNION ALL         UNION            EXCEPT          INTERSECT
            ///          -------------  -----------------  --------------  -----------------
            ///   AltB:   outA, nextA      outA, nextA       outA, nextA         nextA
            ///
            ///   AeqB:   outA, nextA         nextA             nextA         outA, nextA
            ///
            ///   AgtB:   outB, nextB      outB, nextB          nextB            nextB
            ///
            ///   EofA:   outB, nextB      outB, nextB          halt             halt
            ///
            ///   EofB:   outA, nextA      outA, nextA       outA, nextA         halt
            ///
            /// In the AltB, AeqB, and AgtB subroutines, an EOF on A following nextA
            /// causes an immediate jump to EofA and an EOF on B following nextB causes
            /// an immediate jump to EofB.  Within EofA and EofB, and EOF on entry or
            /// following nextX causes a jump to the end of the select processing.
            ///
            /// Duplicate removal in the UNION, EXCEPT, and INTERSECT cases is handled
            /// within the output subroutine.  The regPrev register set holds the previously
            /// output value.  A comparison is made against this value and the output
            /// is skipped if the next results would be the same as the previous.
            ///
            /// The implementation plan is to implement the two coroutines and seven
            /// subroutines first, then put the control logic at the bottom.  Like this:
            ///
            ///          goto Init
            ///     coA: coroutine for left query (A)
            ///     coB: coroutine for right query (B)
            ///    outA: output one row of A
            ///    outB: output one row of B (UNION and UNION ALL only)
            ///    EofA: ...
            ///    EofB: ...
            ///    AltB: ...
            ///    AeqB: ...
            ///    AgtB: ...
            ///    Init: initialize coroutine registers
            ///          yield coA
            ///          if eof(A) goto EofA
            ///          yield coB
            ///          if eof(B) goto EofB
            ///    Cmpr: Compare A, B
            ///          Jump AltB, AeqB, AgtB
            ///     End: ...
            ///
            /// We call AltB, AeqB, AgtB, EofA, and EofB "subroutines" but they are not
            /// actually called using Gosub and they do not Return.  EofA and EofB loop
            /// until all data is exhausted then jump to the "end" labe.  AltB, AeqB,
            /// and AgtB jump to either L2 or to one of EofA or EofB.
            ///
            ///</summary>
#if !SQLITE_OMIT_COMPOUND_SELECT
            public static int multiSelectOrderBy(Parse pParse,///
                ///<summary>
                ///Parsing context 
                ///</summary>
            Select p,///
                ///<summary>
                ///</summary>
                ///<param name="The right">most of SELECTs to be coded </param>
            SelectDest pDest///
                ///<summary>
                ///What to do with query results 
                ///</summary>
            )
            {
                int i, j;
                ///
                ///<summary>
                ///Loop counters 
                ///</summary>
                Select pPrior;
                ///
                ///<summary>
                ///Another SELECT immediately to our left 
                ///</summary>
                Vdbe v;
                ///
                ///<summary>
                ///Generate code to this VDBE 
                ///</summary>
                SelectDest destA = new SelectDest();
                ///
                ///<summary>
                ///Destination for coroutine A 
                ///</summary>
                SelectDest destB = new SelectDest();
                ///
                ///<summary>
                ///Destination for coroutine B 
                ///</summary>
                int regAddrA;
                ///
                ///<summary>
                ///</summary>
                ///<param name="Address register for select">A coroutine </param>
                int regEofA;
                ///
                ///<summary>
                ///</summary>
                ///<param name="Flag to indicate when select">A is complete </param>
                int regAddrB;
                ///
                ///<summary>
                ///</summary>
                ///<param name="Address register for select">B coroutine </param>
                int regEofB;
                ///
                ///<summary>
                ///</summary>
                ///<param name="Flag to indicate when select">B is complete </param>
                int addrSelectA;
                ///
                ///<summary>
                ///</summary>
                ///<param name="Address of the select">A coroutine </param>
                int addrSelectB;
                ///
                ///<summary>
                ///</summary>
                ///<param name="Address of the select">B coroutine </param>
                int regOutA;
                ///
                ///<summary>
                ///</summary>
                ///<param name="Address register for the output">A subroutine </param>
                int regOutB;
                ///
                ///<summary>
                ///</summary>
                ///<param name="Address register for the output">B subroutine </param>
                int addrOutA;
                ///
                ///<summary>
                ///</summary>
                ///<param name="Address of the output">A subroutine </param>
                int addrOutB = 0;
                ///
                ///<summary>
                ///</summary>
                ///<param name="Address of the output">B subroutine </param>
                int addrEofA;
                ///
                ///<summary>
                ///</summary>
                ///<param name="Address of the select">exhausted subroutine </param>
                int addrEofB;
                ///
                ///<summary>
                ///</summary>
                ///<param name="Address of the select">exhausted subroutine </param>
                int addrAltB;
                ///
                ///<summary>
                ///Address of the A<B subroutine 
                ///</summary>
                int addrAeqB;
                ///
                ///<summary>
                ///Address of the A==B subroutine 
                ///</summary>
                int addrAgtB;
                ///
                ///<summary>
                ///Address of the A>B subroutine 
                ///</summary>
                int regLimitA;
                ///
                ///<summary>
                ///</summary>
                ///<param name="Limit register for select">A </param>
                int regLimitB;
                ///
                ///<summary>
                ///</summary>
                ///<param name="Limit register for select">A </param>
                int regPrev;
                ///
                ///<summary>
                ///A range of registers to hold previous output 
                ///</summary>
                int savedLimit;
                ///
                ///<summary>
                ///Saved value of p.iLimit 
                ///</summary>
                int savedOffset;
                ///
                ///<summary>
                ///Saved value of p.iOffset 
                ///</summary>
                int labelCmpr;
                ///
                ///<summary>
                ///Label for the start of the merge algorithm 
                ///</summary>
                int labelEnd;
                ///
                ///<summary>
                ///Label for the end of the overall SELECT stmt 
                ///</summary>
                int j1;
                ///
                ///<summary>
                ///Jump instructions that get retargetted 
                ///</summary>
                int op;
                ///
                ///<summary>
                ///One of TK_ALL, TK_UNION, TK_EXCEPT, TK_INTERSECT 
                ///</summary>
                KeyInfo pKeyDup = null;
                ///
                ///<summary>
                ///Comparison information for duplicate removal 
                ///</summary>
                KeyInfo pKeyMerge;
                ///
                ///<summary>
                ///Comparison information for merging rows 
                ///</summary>
                sqlite3 db;
                ///
                ///<summary>
                ///Database connection 
                ///</summary>
                ExprList pOrderBy;
                ///
                ///<summary>
                ///The ORDER BY clause 
                ///</summary>
                int nOrderBy;
                ///
                ///<summary>
                ///Number of terms in the ORDER BY clause 
                ///</summary>
                int[] aPermute;
                ///
                ///<summary>
                ///Mapping from ORDER BY terms to result set columns 
                ///</summary>
#if !SQLITE_OMIT_EXPLAIN
                int iSub1 = 0;
                ///
                ///<summary>
                ///</summary>
                ///<param name="EQP id of left">hand query </param>
                int iSub2 = 0;
                ///
                ///<summary>
                ///</summary>
                ///<param name="EQP id of right">hand query </param>
#endif
                Debug.Assert(p.pOrderBy != null);
                Debug.Assert(pKeyDup == null);
                ///
                ///<summary>
                ///"Managed" code needs this.  Ticket #3382. 
                ///</summary>
                db = pParse.db;
                v = pParse.pVdbe;
                Debug.Assert(v != null);
                ///
                ///<summary>
                ///Already thrown the error if VDBE alloc failed 
                ///</summary>
                labelEnd = v.sqlite3VdbeMakeLabel();
                labelCmpr = v.sqlite3VdbeMakeLabel();
                ///
                ///<summary>
                ///Patch up the ORDER BY clause
                ///
                ///</summary>
                op = p.tk_op;
                pPrior = p.pPrior;
                Debug.Assert(pPrior.pOrderBy == null);
                pOrderBy = p.pOrderBy;
                Debug.Assert(pOrderBy != null);
                nOrderBy = pOrderBy.nExpr;
                ///
                ///<summary>
                ///For operators other than UNION ALL we have to make sure that
                ///the ORDER BY clause covers every term of the result set.  Add
                ///terms to the ORDER BY clause as necessary.
                ///
                ///</summary>
                if (op != TK_ALL)
                {
                    for (i = 1;///
                        ///<summary>
                        ///db.mallocFailed == 0 && 
                        ///</summary>
                    i <= p.pEList.nExpr; i++)
                    {
                        ExprList_item pItem;
                        for (j = 0; j < nOrderBy; j++)//, pItem++)
                        {
                            pItem = pOrderBy.a[j];
                            Debug.Assert(pItem.iCol > 0);
                            if (pItem.iCol == i)
                                break;
                        }
                        if (j == nOrderBy)
                        {
                            Expr pNew = exprc.sqlite3Expr(db, TK_INTEGER, null);
                            //if ( pNew == null )
                            //  return SQLITE_NOMEM;
                            pNew.Flags |= ExprFlags.EP_IntValue;
                            pNew.u.iValue = i;
                            pOrderBy = pParse.sqlite3ExprListAppend(pOrderBy, pNew);
                            pOrderBy.a[nOrderBy++].iCol = (u16)i;
                        }
                    }
                }
                ///
                ///<summary>
                ///Compute the comparison permutation and keyinfo that is used with
                ///the permutation used to determine if the next
                ///row of results comes from selectA or selectB.  Also add explicit
                ///collations to the ORDER BY clause terms so that when the subqueries
                ///to the right and the left are evaluated, they use the correct
                ///collation.
                ///
                ///</summary>
                aPermute = new int[nOrderBy];
                // sqlite3DbMallocRaw( db, sizeof( int ) * nOrderBy );
                if (aPermute != null)
                {
                    ExprList_item pItem;
                    for (i = 0; i < nOrderBy; i++)//, pItem++)
                    {
                        pItem = pOrderBy.a[i];
                        Debug.Assert(pItem.iCol > 0 && pItem.iCol <= p.pEList.nExpr);
                        aPermute[i] = pItem.iCol - 1;
                    }
                    pKeyMerge = new KeyInfo();
                    //      sqlite3DbMallocRaw(db, sizeof(*pKeyMerge)+nOrderBy*(sizeof(CollSeq)+1));
                    if (pKeyMerge != null)
                    {
                        pKeyMerge.aColl = new CollSeq[nOrderBy];
                        pKeyMerge.aSortOrder = new byte[nOrderBy];
                        //(u8)&pKeyMerge.aColl[nOrderBy];
                        pKeyMerge.nField = (u16)nOrderBy;
                        pKeyMerge.enc = ENC(db);
                        for (i = 0; i < nOrderBy; i++)
                        {
                            CollSeq pColl;
                            Expr pTerm = pOrderBy.a[i].pExpr;
                            if ((pTerm.Flags & ExprFlags.EP_ExpCollate) != 0)
                            {
                                pColl = pTerm.pColl;
                            }
                            else
                            {
                                pColl = multiSelectCollSeq(pParse, p, aPermute[i]);
                                pTerm.Flags |= ExprFlags.EP_ExpCollate;
                                pTerm.pColl = pColl;
                            }
                            pKeyMerge.aColl[i] = pColl;
                            pKeyMerge.aSortOrder[i] = (byte)pOrderBy.a[i].sortOrder;
                        }
                    }
                }
                else
                {
                    pKeyMerge = null;
                }
                ///
                ///<summary>
                ///Reattach the ORDER BY clause to the query.
                ///
                ///</summary>
                p.pOrderBy = pOrderBy;
                pPrior.pOrderBy = exprc.sqlite3ExprListDup(pParse.db, pOrderBy, 0);
                ///
                ///<summary>
                ///Allocate a range of temporary registers and the KeyInfo needed
                ///for the logic that removes duplicate result rows when the
                ///operator is UNION, EXCEPT, or INTERSECT (but not UNION ALL).
                ///
                ///</summary>
                if (op == TK_ALL)
                {
                    regPrev = 0;
                }
                else
                {
                    int nExpr = p.pEList.nExpr;
                    Debug.Assert(nOrderBy >= nExpr///
                        ///<summary>
                        ///|| db.mallocFailed != 0 
                        ///</summary>
                    );
                    regPrev = pParse.sqlite3GetTempRange(nExpr + 1);
                    v.sqlite3VdbeAddOp2(OpCode.OP_Integer, 0, regPrev);
                    pKeyDup = new KeyInfo();
                    //sqlite3DbMallocZero(db,
                    //sizeof(*pKeyDup) + nExpr*(sizeof(CollSeq)+1) );
                    if (pKeyDup != null)
                    {
                        pKeyDup.aColl = new CollSeq[nExpr];
                        pKeyDup.aSortOrder = new byte[nExpr];
                        //(u8)&pKeyDup.aColl[nExpr];
                        pKeyDup.nField = (u16)nExpr;
                        pKeyDup.enc = ENC(db);
                        for (i = 0; i < nExpr; i++)
                        {
                            pKeyDup.aColl[i] = multiSelectCollSeq(pParse, p, i);
                            pKeyDup.aSortOrder[i] = 0;
                        }
                    }
                }
                ///
                ///<summary>
                ///Separate the left and the right query from one another
                ///
                ///</summary>
                p.pPrior = null;
                ResolveExtensions.sqlite3ResolveOrderGroupBy(pParse, p, p.pOrderBy, "ORDER");
                if (pPrior.pPrior == null)
                {
                    ResolveExtensions.sqlite3ResolveOrderGroupBy(pParse, pPrior, pPrior.pOrderBy, "ORDER");
                }
                ///
                ///<summary>
                ///Compute the limit registers 
                ///</summary>
                SelectMethods.computeLimitRegisters(pParse, p, labelEnd);
                if (p.iLimit != 0 && op == TK_ALL)
                {
                    regLimitA = ++pParse.nMem;
                    regLimitB = ++pParse.nMem;
                    v.sqlite3VdbeAddOp2(OpCode.OP_Copy, (p.iOffset != 0) ? p.iOffset + 1 : p.iLimit, regLimitA);
                    v.sqlite3VdbeAddOp2(OpCode.OP_Copy, regLimitA, regLimitB);
                }
                else
                {
                    regLimitA = regLimitB = 0;
                }
                exprc.sqlite3ExprDelete(db, ref p.pLimit);
                p.pLimit = null;
                exprc.sqlite3ExprDelete(db, ref p.pOffset);
                p.pOffset = null;
                regAddrA = ++pParse.nMem;
                regEofA = ++pParse.nMem;
                regAddrB = ++pParse.nMem;
                regEofB = ++pParse.nMem;
                regOutA = ++pParse.nMem;
                regOutB = ++pParse.nMem;
                destA.Init(SelectResultType.Coroutine, regAddrA);
                destB.Init(SelectResultType.Coroutine, regAddrB);
                ///
                ///<summary>
                ///Jump past the various subroutines and coroutines to the main
                ///merge loop
                ///
                ///</summary>
                j1 = v.sqlite3VdbeAddOp0(OpCode.OP_Goto);
                addrSelectA = v.sqlite3VdbeCurrentAddr();
                ///
                ///<summary>
                ///Generate a coroutine to evaluate the SELECT statement to the
                ///</summary>
                ///<param name="left of the compound operator "> the "A" select.</param>
                ///<param name=""></param>
                VdbeNoopComment(v, "Begin coroutine for left SELECT");
                pPrior.iLimit = regLimitA;
                SelectMethods.explainSetInteger(ref iSub1, pParse.iNextSelectId);
                Select.sqlite3Select(pParse, pPrior, ref destA);
                v.sqlite3VdbeAddOp2(OpCode.OP_Integer, 1, regEofA);
                v.sqlite3VdbeAddOp1(OpCode.OP_Yield, regAddrA);
                VdbeNoopComment(v, "End coroutine for left SELECT");
                ///
                ///<summary>
                ///Generate a coroutine to evaluate the SELECT statement on
                ///</summary>
                ///<param name="the right "> the "B" select</param>
                ///<param name=""></param>
                addrSelectB = v.sqlite3VdbeCurrentAddr();
                VdbeNoopComment(v, "Begin coroutine for right SELECT");
                savedLimit = p.iLimit;
                savedOffset = p.iOffset;
                p.iLimit = regLimitB;
                p.iOffset = 0;
                SelectMethods.explainSetInteger(ref iSub2, pParse.iNextSelectId);
                Select.sqlite3Select(pParse, p, ref destB);
                p.iLimit = savedLimit;
                p.iOffset = savedOffset;
                v.sqlite3VdbeAddOp2(OpCode.OP_Integer, 1, regEofB);
                v.sqlite3VdbeAddOp1(OpCode.OP_Yield, regAddrB);
                VdbeNoopComment(v, "End coroutine for right SELECT");
                ///
                ///<summary>
                ///Generate a subroutine that outputs the current row of the A
                ///select as the next output row of the compound select.
                ///
                ///</summary>
                VdbeNoopComment(v, "Output routine for A");
                addrOutA = generateOutputSubroutine(pParse, p, destA, pDest, regOutA, regPrev, pKeyDup, P4_KEYINFO_HANDOFF, labelEnd);
                ///
                ///<summary>
                ///Generate a subroutine that outputs the current row of the B
                ///select as the next output row of the compound select.
                ///
                ///</summary>
                if (op == TK_ALL || op == TK_UNION)
                {
                    VdbeNoopComment(v, "Output routine for B");
                    addrOutB = generateOutputSubroutine(pParse, p, destB, pDest, regOutB, regPrev, pKeyDup, P4_KEYINFO_STATIC, labelEnd);
                }
                ///
                ///<summary>
                ///Generate a subroutine to run when the results from select A
                ///are exhausted and only data in select B remains.
                ///
                ///</summary>
                VdbeNoopComment(v, "eof-A subroutine");
                if (op == TK_EXCEPT || op == TK_INTERSECT)
                {
                    addrEofA = v.sqlite3VdbeAddOp2(OpCode.OP_Goto, 0, labelEnd);
                }
                else
                {
                    addrEofA = v.sqlite3VdbeAddOp2(OP_If, regEofB, labelEnd);
                    v.sqlite3VdbeAddOp2(OpCode.OP_Gosub, regOutB, addrOutB);
                    v.sqlite3VdbeAddOp1(OpCode.OP_Yield, regAddrB);
                    v.sqlite3VdbeAddOp2(OpCode.OP_Goto, 0, addrEofA);
                    p.nSelectRow += pPrior.nSelectRow;
                }
                ///
                ///<summary>
                ///Generate a subroutine to run when the results from select B
                ///are exhausted and only data in select A remains.
                ///
                ///</summary>
                if (op == TK_INTERSECT)
                {
                    addrEofB = addrEofA;
                    if (p.nSelectRow > pPrior.nSelectRow)
                        p.nSelectRow = pPrior.nSelectRow;
                }
                else
                {
                    VdbeNoopComment(v, "eof-B subroutine");
                    addrEofB = v.sqlite3VdbeAddOp2(OP_If, regEofA, labelEnd);
                    v.sqlite3VdbeAddOp2(OpCode.OP_Gosub, regOutA, addrOutA);
                    v.sqlite3VdbeAddOp1(OpCode.OP_Yield, regAddrA);
                    v.sqlite3VdbeAddOp2(OpCode.OP_Goto, 0, addrEofB);
                }
                ///
                ///<summary>
                ///Generate code to handle the case of A<B
                ///
                ///</summary>
                VdbeNoopComment(v, "A-lt-B subroutine");
                addrAltB = v.sqlite3VdbeAddOp2(OpCode.OP_Gosub, regOutA, addrOutA);
                v.sqlite3VdbeAddOp1(OpCode.OP_Yield, regAddrA);
                v.sqlite3VdbeAddOp2(OP_If, regEofA, addrEofA);
                v.sqlite3VdbeAddOp2(OpCode.OP_Goto, 0, labelCmpr);
                ///
                ///<summary>
                ///Generate code to handle the case of A==B
                ///
                ///</summary>
                if (op == TK_ALL)
                {
                    addrAeqB = addrAltB;
                }
                else
                    if (op == TK_INTERSECT)
                    {
                        addrAeqB = addrAltB;
                        addrAltB++;
                    }
                    else
                    {
                        VdbeNoopComment(v, "A-eq-B subroutine");
                        addrAeqB = v.sqlite3VdbeAddOp1(OpCode.OP_Yield, regAddrA);
                        v.sqlite3VdbeAddOp2(OP_If, regEofA, addrEofA);
                        v.sqlite3VdbeAddOp2(OpCode.OP_Goto, 0, labelCmpr);
                    }
                ///
                ///<summary>
                ///Generate code to handle the case of A>B
                ///
                ///</summary>
                VdbeNoopComment(v, "A-gt-B subroutine");
                addrAgtB = v.sqlite3VdbeCurrentAddr();
                if (op == TK_ALL || op == TK_UNION)
                {
                    v.sqlite3VdbeAddOp2(OpCode.OP_Gosub, regOutB, addrOutB);
                }
                v.sqlite3VdbeAddOp1(OpCode.OP_Yield, regAddrB);
                v.sqlite3VdbeAddOp2(OP_If, regEofB, addrEofB);
                v.sqlite3VdbeAddOp2(OpCode.OP_Goto, 0, labelCmpr);
                ///
                ///<summary>
                ///This code runs once to initialize everything.
                ///
                ///</summary>
                v.sqlite3VdbeJumpHere(j1);
                v.sqlite3VdbeAddOp2(OpCode.OP_Integer, 0, regEofA);
                v.sqlite3VdbeAddOp2(OpCode.OP_Integer, 0, regEofB);
                v.sqlite3VdbeAddOp2(OpCode.OP_Gosub, regAddrA, addrSelectA);
                v.sqlite3VdbeAddOp2(OpCode.OP_Gosub, regAddrB, addrSelectB);
                v.sqlite3VdbeAddOp2(OP_If, regEofA, addrEofA);
                v.sqlite3VdbeAddOp2(OP_If, regEofB, addrEofB);
                ///
                ///<summary>
                ///Implement the main merge loop
                ///
                ///</summary>
                v.sqlite3VdbeResolveLabel(labelCmpr);
                v.sqlite3VdbeAddOp4(OP_Permutation, 0, 0, 0, aPermute, P4_INTARRAY);
                v.sqlite3VdbeAddOp4(OpCode.OP_Compare, destA.iMem, destB.iMem, nOrderBy, pKeyMerge, P4_KEYINFO_HANDOFF);
                v.sqlite3VdbeAddOp3(OpCode.OP_Jump, addrAltB, addrAeqB, addrAgtB);
                ///
                ///<summary>
                ///Release temporary registers
                ///
                ///</summary>
                if (regPrev != 0)
                {
                    pParse.sqlite3ReleaseTempRange(regPrev, nOrderBy + 1);
                }
                ///
                ///<summary>
                ///Jump to the this point in order to terminate the query.
                ///
                ///</summary>
                v.sqlite3VdbeResolveLabel(labelEnd);
                ///
                ///<summary>
                ///Set the number of output columns
                ///
                ///</summary>
                if (pDest.eDest == SelectResultType.Output)
                {
                    Select pFirst = pPrior;
                    while (pFirst.pPrior != null)
                        pFirst = pFirst.pPrior;
                    SelectMethods.generateColumnNames(pParse, null, pFirst.pEList);
                }
                ///
                ///<summary>
                ///Reassembly the compound query so that it will be freed correctly
                ///by the calling function 
                ///</summary>
                if (p.pPrior != null)
                {
                    SelectMethods.sqlite3SelectDelete(db, ref p.pPrior);
                }
                p.pPrior = pPrior;
                ///
                ///<summary>
                ///TBD:  Insert subroutine calls to close cursors on incomplete
                ///subqueries ***
                ///</summary>
                SelectMethods.explainComposite(pParse, p.tk_op, iSub1, iSub2, false);
                return SQLITE_OK;
            }
#endif
#if !(SQLITE_OMIT_SUBQUERY) || !(SQLITE_OMIT_VIEW)
            ///<summary>
            ///Forward Declarations
            ///</summary>
            //static void substExprList(sqlite3*, ExprList*, int, ExprList);
            //static void substSelect(sqlite3*, Select *, int, ExprList );
            ///<summary>
            /// Scan through the expression pExpr.  Replace every reference to
            /// a column in table number iTable with a copy of the iColumn-th
            /// entry in pEList.  (But leave references to the ROWID column
            /// unchanged.)
            ///
            /// This routine is part of the flattening procedure.  A subquery
            /// whose result set is defined by pEList appears as entry in the
            /// FROM clause of a SELECT such that the VDBE cursor assigned to that
            /// FORM clause entry is iTable.  This routine make the necessary
            /// changes to pExpr so that it refers directly to the source table
            /// of the subquery rather the result set of the subquery.
            ///
            ///</summary>
            static Expr substExpr(sqlite3 db,///
                ///<summary>
                ///Report malloc errors to this connection 
                ///</summary>
            Expr pExpr,///
                ///<summary>
                ///Expr in which substitution occurs 
                ///</summary>
            int iTable,///
                ///<summary>
                ///Table to be substituted 
                ///</summary>
            ExprList pEList///
                ///<summary>
                ///Substitute expressions 
                ///</summary>
            )
            {
                if (pExpr == null)
                    return null;
                if (pExpr.op == TK_COLUMN && pExpr.iTable == iTable)
                {
                    if (pExpr.iColumn < 0)
                    {
                        pExpr.op = TK_NULL;
                    }
                    else
                    {
                        Expr pNew;
                        Debug.Assert(pEList != null && pExpr.iColumn < pEList.nExpr);
                        Debug.Assert(pExpr.pLeft == null && pExpr.pRight == null);
                        pNew = exprc.sqlite3ExprDup(db, pEList.a[pExpr.iColumn].pExpr, 0);
                        if (pExpr.pColl != null)
                        {
                            pNew.pColl = pExpr.pColl;
                        }
                        exprc.sqlite3ExprDelete(db, ref pExpr);
                        pExpr = pNew;
                    }
                }
                else
                {
                    pExpr.pLeft = substExpr(db, pExpr.pLeft, iTable, pEList);
                    pExpr.pRight = substExpr(db, pExpr.pRight, iTable, pEList);
                    if (pExpr.ExprHasProperty(EP_xIsSelect))
                    {
                        substSelect(db, pExpr.x.pSelect, iTable, pEList);
                    }
                    else
                    {
                        substExprList(db, pExpr.x.pList, iTable, pEList);
                    }
                }
                return pExpr;
            }
            static void substExprList(sqlite3 db,///
                ///<summary>
                ///Report malloc errors here 
                ///</summary>
            ExprList pList,///
                ///<summary>
                ///List to scan and in which to make substitutes 
                ///</summary>
            int iTable,///
                ///<summary>
                ///Table to be substituted 
                ///</summary>
            ExprList pEList///
                ///<summary>
                ///Substitute values 
                ///</summary>
            )
            {
                int i;
                if (pList == null)
                    return;
                for (i = 0; i < pList.nExpr; i++)
                {
                    pList.a[i].pExpr = substExpr(db, pList.a[i].pExpr, iTable, pEList);
                }
            }
            static void substSelect(sqlite3 db,///
                ///<summary>
                ///Report malloc errors here 
                ///</summary>
            Select p,///
                ///<summary>
                ///SELECT statement in which to make substitutions 
                ///</summary>
            int iTable,///
                ///<summary>
                ///Table to be replaced 
                ///</summary>
            ExprList pEList///
                ///<summary>
                ///Substitute values 
                ///</summary>
            )
            {
                SrcList pSrc;
                SrcList_item pItem;
                int i;
                if (p == null)
                    return;
                substExprList(db, p.pEList, iTable, pEList);
                substExprList(db, p.pGroupBy, iTable, pEList);
                substExprList(db, p.pOrderBy, iTable, pEList);
                p.pHaving = substExpr(db, p.pHaving, iTable, pEList);
                p.pWhere = substExpr(db, p.pWhere, iTable, pEList);
                substSelect(db, p.pPrior, iTable, pEList);
                pSrc = p.pSrc;
                Debug.Assert(pSrc != null);
                ///
                ///<summary>
                ///</summary>
                ///<param name="Even for (SELECT 1) we have: pSrc!=0 but pSrc">>nSrc==0 </param>
                if (ALWAYS(pSrc))
                {
                    for (i = pSrc.nSrc; i > 0; i--)//, pItem++ )
                    {
                        pItem = pSrc.a[pSrc.nSrc - i];
                        substSelect(db, pItem.pSelect, iTable, pEList);
                    }
                }
            }
#endif
#if !(SQLITE_OMIT_SUBQUERY) || !(SQLITE_OMIT_VIEW)
            ///<summary>
            /// This routine attempts to flatten subqueries in order to speed
            /// execution.  It returns 1 if it makes changes and 0 if no flattening
            /// occurs.
            ///
            /// To understand the concept of flattening, consider the following
            /// query:
            ///
            ///     SELECT a FROM (SELECT x+y AS a FROM t1 WHERE z<100) WHERE a>5
            ///
            /// The default way of implementing this query is to execute the
            /// subquery first and store the results in a temporary table, then
            /// run the outer query on that temporary table.  This requires two
            /// passes over the data.  Furthermore, because the temporary table
            /// has no indices, the WHERE clause on the outer query cannot be
            /// optimized.
            ///
            /// This routine attempts to rewrite queries such as the above into
            /// a single flat select, like this:
            ///
            ///     SELECT x+y AS a FROM t1 WHERE z<100 AND a>5
            ///
            /// The code generated for this simpification gives the same result
            /// but only has to scan the data once.  And because indices might
            /// exist on the table t1, a complete scan of the data might be
            /// avoided.
            ///
            /// Flattening is only attempted if all of the following are true:
            ///
            ///   (1)  The subquery and the outer query do not both use aggregates.
            ///
            ///   (2)  The subquery is not an aggregate or the outer query is not a join.
            ///
            ///   (3)  The subquery is not the right operand of a left outer join
            ///        (Originally ticket #306.  Strengthened by ticket #3300)
            ///
            ///   (4)  The subquery is not DISTINCT.
            ///
            ///  (*)  At one point restrictions (4) and (5) defined a subset of DISTINCT
            ///        sub-queries that were excluded from this optimization. Restriction
            ///        (4) has since been expanded to exclude all DISTINCT subqueries.
            ///
            ///   (6)  The subquery does not use aggregates or the outer query is not
            ///        DISTINCT.
            ///
            ///   (7)  The subquery has a FROM clause.
            ///
            ///   (8)  The subquery does not use LIMIT or the outer query is not a join.
            ///
            ///   (9)  The subquery does not use LIMIT or the outer query does not use
            ///        aggregates.
            ///
            ///  (10)  The subquery does not use aggregates or the outer query does not
            ///        use LIMIT.
            ///
            ///  (11)  The subquery and the outer query do not both have ORDER BY clauses.
            ///
            ///  (*)  Not implemented.  Subsumed into restriction (3).  Was previously
            ///        a separate restriction deriving from ticket #350.
            ///
            ///  (13)  The subquery and outer query do not both use LIMIT.
            ///
            ///  (14)  The subquery does not use OFFSET.
            ///
            ///  (15)  The outer query is not part of a compound select or the
            ///        subquery does not have a LIMIT clause.
            ///        (See ticket #2339 and ticket [02a8e81d44]).
            ///
            ///  (16)  The outer query is not an aggregate or the subquery does
            ///        not contain ORDER BY.  (Ticket #2942)  This used to not matter
            ///        until we introduced the group_concat() function.
            ///
            ///  (17)  The sub-query is not a compound select, or it is a UNION ALL
            ///        compound clause made up entirely of non-aggregate queries, and
            ///        the parent query:
            ///
            ///          * is not itself part of a compound select,
            ///          * is not an aggregate or DISTINCT query, and
            ///          * has no other tables or sub-selects in the FROM clause.
            ///
            ///        The parent and sub-query may contain WHERE clauses. Subject to
            ///        rules (11), (13) and (14), they may also contain ORDER BY,
            ///        LIMIT and OFFSET clauses.
            ///
            ///  (18)  If the sub-query is a compound select, then all terms of the
            ///        ORDER by clause of the parent must be simple references to
            ///        columns of the sub-query.
            ///
            ///  (19)  The subquery does not use LIMIT or the outer query does not
            ///        have a WHERE clause.
            ///
            ///  (20)  If the sub-query is a compound select, then it must not use
            ///        an ORDER BY clause.  Ticket #3773.  We could relax this constraint
            ///        somewhat by saying that the terms of the ORDER BY clause must
            ///        appear as unmodified result columns in the outer query.  But
            ///        have other optimizations in mind to deal with that case.
            ///
            ///  (21)  The subquery does not use LIMIT or the outer query is not
            ///        DISTINCT.  (See ticket [752e1646fc]).
            ///
            /// In this routine, the "p" parameter is a pointer to the outer query.
            /// The subquery is p.pSrc.a[iFrom].  isAgg is true if the outer query
            /// uses aggregates and subqueryIsAgg is true if the subquery uses aggregates.
            ///
            /// If flattening is not attempted, this routine is a no-op and returns 0.
            /// If flattening is attempted this routine returns 1.
            ///
            /// All of the expression analysis must occur on both the outer query and
            /// the subquery before this routine runs.
            ///</summary>
            public static int flattenSubquery(Parse pParse,///
                ///<summary>
                ///Parsing context 
                ///</summary>
            Select p,///
                ///<summary>
                ///The parent or outer SELECT statement 
                ///</summary>
            int iFrom,///
                ///<summary>
                ///Index in p.pSrc.a[] of the inner subquery 
                ///</summary>
            bool isAgg,///
                ///<summary>
                ///True if outer SELECT uses aggregate functions 
                ///</summary>
            bool subqueryIsAgg///
                ///<summary>
                ///True if the subquery uses aggregate functions 
                ///</summary>
            )
            {
                string zSavedAuthContext = pParse.zAuthContext;
                Select pParent;
                Select pSub;
                ///
                ///<summary>
                ///The inner query or "subquery" 
                ///</summary>
                Select pSub1;
                ///
                ///<summary>
                ///</summary>
                ///<param name="Pointer to the rightmost select in sub">query </param>
                SrcList pSrc;
                ///
                ///<summary>
                ///The FROM clause of the outer query 
                ///</summary>
                SrcList pSubSrc;
                ///
                ///<summary>
                ///The FROM clause of the subquery 
                ///</summary>
                ExprList pList;
                ///
                ///<summary>
                ///The result set of the outer query 
                ///</summary>
                int iParent;
                ///
                ///<summary>
                ///VDBE cursor number of the pSub result set temp table 
                ///</summary>
                int i;
                ///
                ///<summary>
                ///Loop counter 
                ///</summary>
                Expr pWhere;
                ///
                ///<summary>
                ///The WHERE clause 
                ///</summary>
                SrcList_item pSubitem;
                ///
                ///<summary>
                ///The subquery 
                ///</summary>
                sqlite3 db = pParse.db;
                ///
                ///<summary>
                ///Check to see if flattening is permitted.  Return 0 if not.
                ///
                ///</summary>
                Debug.Assert(p != null);
                Debug.Assert(p.pPrior == null);
                ///
                ///<summary>
                ///Unable to flatten compound queries 
                ///</summary>
                if ((db.flags & SQLITE_QueryFlattener) != 0)
                    return 0;
                pSrc = p.pSrc;
                Debug.Assert(pSrc != null && iFrom >= 0 && iFrom < pSrc.nSrc);
                pSubitem = pSrc.a[iFrom];
                iParent = pSubitem.iCursor;
                pSub = pSubitem.pSelect;
                Debug.Assert(pSub != null);
                if (isAgg && subqueryIsAgg)
                    return 0;
                ///
                ///<summary>
                ///Restriction (1)  
                ///</summary>
                if (subqueryIsAgg && pSrc.nSrc > 1)
                    return 0;
                ///
                ///<summary>
                ///Restriction (2)  
                ///</summary>
                pSubSrc = pSub.pSrc;
                Debug.Assert(pSubSrc != null);
                ///
                ///<summary>
                ///Prior to version 3.1.2, when LIMIT and OFFSET had to be simple constants,
                ///not arbitrary expresssions, we allowed some combining of LIMIT and OFFSET
                ///</summary>
                ///<param name="because they could be computed at compile">time.  But when LIMIT and OFFSET</param>
                ///<param name="became arbitrary expressions, we were forced to add restrictions (13)">became arbitrary expressions, we were forced to add restrictions (13)</param>
                ///<param name="and (14). ">and (14). </param>
                if (pSub.pLimit != null && p.pLimit != null)
                    return 0;
                ///
                ///<summary>
                ///Restriction (13) 
                ///</summary>
                if (pSub.pOffset != null)
                    return 0;
                ///
                ///<summary>
                ///Restriction (14) 
                ///</summary>
                if (p.pRightmost != null && pSub.pLimit != null)
                {
                    return 0;
                    ///
                    ///<summary>
                    ///Restriction (15) 
                    ///</summary>
                }
                if (pSubSrc.nSrc == 0)
                    return 0;
                ///
                ///<summary>
                ///Restriction (7)  
                ///</summary>
                if ((pSub.selFlags & SelectFlags.Distinct) != 0)
                    return 0;
                ///
                ///<summary>
                ///Restriction (5)  
                ///</summary>
                if (pSub.pLimit != null && (pSrc.nSrc > 1 || isAgg))
                {
                    return 0;
                    ///
                    ///<summary>
                    ///Restrictions (8)(9) 
                    ///</summary>
                }
                if ((p.selFlags & SelectFlags.Distinct) != 0 && subqueryIsAgg)
                {
                    return 0;
                    ///
                    ///<summary>
                    ///Restriction (6)  
                    ///</summary>
                }
                if (p.pOrderBy != null && pSub.pOrderBy != null)
                {
                    return 0;
                    ///
                    ///<summary>
                    ///Restriction (11) 
                    ///</summary>
                }
                if (isAgg && pSub.pOrderBy != null)
                    return 0;
                ///
                ///<summary>
                ///Restriction (16) 
                ///</summary>
                if (pSub.pLimit != null && p.pWhere != null)
                    return 0;
                ///
                ///<summary>
                ///Restriction (19) 
                ///</summary>
                if (pSub.pLimit != null && (p.selFlags & SelectFlags.Distinct) != 0)
                {
                    return 0;
                    ///
                    ///<summary>
                    ///Restriction (21) 
                    ///</summary>
                }
                ///
                ///<summary>
                ///OBSOLETE COMMENT 1:
                ///Restriction 3:  If the subquery is a join, make sure the subquery is
                ///not used as the right operand of an outer join.  Examples of why this
                ///is not allowed:
                ///
                ///t1 LEFT OUTER JOIN (t2 JOIN t3)
                ///
                ///If we flatten the above, we would get
                ///
                ///(t1 LEFT OUTER JOIN t2) JOIN t3
                ///
                ///which is not at all the same thing.
                ///
                ///OBSOLETE COMMENT 2:
                ///Restriction 12:  If the subquery is the right operand of a left outer
                ////* Restriction 12:  If the subquery is the right operand of a left outer
                ///join, make sure the subquery has no WHERE clause.
                ///An examples of why this is not allowed:
                ///
                ///t1 LEFT OUTER JOIN (SELECT * FROM t2 WHERE t2.x>0)
                ///
                ///If we flatten the above, we would get
                ///
                ///(t1 LEFT OUTER JOIN t2) WHERE t2.x>0
                ///
                ///But the t2.x>0 test will always fail on a NULL row of t2, which
                ///effectively converts the OUTER JOIN into an INNER JOIN.
                ///
                ///THIS OVERRIDES OBSOLETE COMMENTS 1 AND 2 ABOVE:
                ///Ticket #3300 shows that flattening the right term of a LEFT JOIN
                ///is fraught with danger.  Best to avoid the whole thing.  If the
                ///subquery is the right term of a LEFT JOIN, then do not flatten.
                ///
                ///</summary>
                if ((pSubitem.jointype & JT_OUTER) != 0)
                {
                    return 0;
                }
                ///
                ///<summary>
                ///</summary>
                ///<param name="Restriction 17: If the sub">query is a compound SELECT, then it must</param>
                ///<param name="use only the UNION ALL operator. And none of the simple select queries">use only the UNION ALL operator. And none of the simple select queries</param>
                ///<param name="that make up the compound SELECT are allowed to be aggregate or distinct">that make up the compound SELECT are allowed to be aggregate or distinct</param>
                ///<param name="queries.">queries.</param>
                ///<param name=""></param>
                if (pSub.pPrior != null)
                {
                    if (pSub.pOrderBy != null)
                    {
                        return 0;
                        ///
                        ///<summary>
                        ///Restriction 20 
                        ///</summary>
                    }
                    if (isAgg || (p.selFlags & SelectFlags.Distinct) != 0 || pSrc.nSrc != 1)
                    {
                        return 0;
                    }
                    for (pSub1 = pSub; pSub1 != null; pSub1 = pSub1.pPrior)
                    {
                        testcase((pSub1.selFlags & (SelectFlags.Distinct | SelectFlags.Aggregate)) == SelectFlags.Distinct);
                        testcase((pSub1.selFlags & (SelectFlags.Distinct | SelectFlags.Aggregate)) == SelectFlags.Aggregate);
                        if ((pSub1.selFlags & (SelectFlags.Distinct | SelectFlags.Aggregate)) != 0 || (pSub1.pPrior != null && pSub1.tk_op != TK_ALL) || NEVER(pSub1.pSrc == null) || pSub1.pSrc.nSrc != 1)
                        {
                            return 0;
                        }
                    }
                    ///
                    ///<summary>
                    ///Restriction 18. 
                    ///</summary>
                    if (p.pOrderBy != null)
                    {
                        int ii;
                        for (ii = 0; ii < p.pOrderBy.nExpr; ii++)
                        {
                            if (p.pOrderBy.a[ii].iCol == 0)
                                return 0;
                        }
                    }
                }
                ///
                ///<summary>
                ///If we reach this point, flattening is permitted. ****
                ///</summary>
                ///
                ///<summary>
                ///Authorize the subquery 
                ///</summary>
                pParse.zAuthContext = pSubitem.zName;
                sqlite3AuthCheck(pParse, SQLITE_SELECT, null, null, null);
                pParse.zAuthContext = zSavedAuthContext;
                ///
                ///<summary>
                ///</summary>
                ///<param name="If the sub">query is a compound SELECT statement, then (by restrictions</param>
                ///<param name="17 and 18 above) it must be a UNION ALL and the parent query must">17 and 18 above) it must be a UNION ALL and the parent query must</param>
                ///<param name="be of the form:">be of the form:</param>
                ///<param name=""></param>
                ///<param name="SELECT <expr">clause></param>
                ///<param name=""></param>
                ///<param name="followed by any ORDER BY, LIMIT and/or OFFSET clauses. This block">followed by any ORDER BY, LIMIT and/or OFFSET clauses. This block</param>
                ///<param name="creates N">1 copies of the parent query without any ORDER BY, LIMIT or</param>
                ///<param name="OFFSET clauses and joins them to the left">side of the original</param>
                ///<param name="using UNION ALL operators. In this case N is the number of simple">using UNION ALL operators. In this case N is the number of simple</param>
                ///<param name="select statements in the compound sub">query.</param>
                ///<param name=""></param>
                ///<param name="Example:">Example:</param>
                ///<param name=""></param>
                ///<param name="SELECT a+1 FROM (">SELECT a+1 FROM (</param>
                ///<param name="SELECT x FROM tab">SELECT x FROM tab</param>
                ///<param name="UNION ALL">UNION ALL</param>
                ///<param name="SELECT y FROM tab">SELECT y FROM tab</param>
                ///<param name="UNION ALL">UNION ALL</param>
                ///<param name="SELECT abs(z*2) FROM tab2">SELECT abs(z*2) FROM tab2</param>
                ///<param name=") WHERE a!=5 ORDER BY 1">) WHERE a!=5 ORDER BY 1</param>
                ///<param name=""></param>
                ///<param name="Transformed into:">Transformed into:</param>
                ///<param name=""></param>
                ///<param name="SELECT x+1 FROM tab WHERE x+1!=5">SELECT x+1 FROM tab WHERE x+1!=5</param>
                ///<param name="UNION ALL">UNION ALL</param>
                ///<param name="SELECT y+1 FROM tab WHERE y+1!=5">SELECT y+1 FROM tab WHERE y+1!=5</param>
                ///<param name="UNION ALL">UNION ALL</param>
                ///<param name="SELECT abs(z*2)+1 FROM tab2 WHERE abs(z*2)+1!=5">SELECT abs(z*2)+1 FROM tab2 WHERE abs(z*2)+1!=5</param>
                ///<param name="ORDER BY 1">ORDER BY 1</param>
                ///<param name=""></param>
                ///<param name="We call this the "compound">subquery flattening".</param>
                ///<param name=""></param>
                for (pSub = pSub.pPrior; pSub != null; pSub = pSub.pPrior)
                {
                    Select pNew;
                    ExprList pOrderBy = p.pOrderBy;
                    Expr pLimit = p.pLimit;
                    Select pPrior = p.pPrior;
                    p.pOrderBy = null;
                    p.pSrc = null;
                    p.pPrior = null;
                    p.pLimit = null;
                    pNew = exprc.sqlite3SelectDup(db, p, 0);
                    p.pLimit = pLimit;
                    p.pOrderBy = pOrderBy;
                    p.pSrc = pSrc;
                    p.tk_op = TK_ALL;
                    p.pRightmost = null;
                    if (pNew == null)
                    {
                        pNew = pPrior;
                    }
                    else
                    {
                        pNew.pPrior = pPrior;
                        pNew.pRightmost = null;
                    }
                    p.pPrior = pNew;
                    //        if ( db.mallocFailed != 0 ) return 1;
                }
                ///
                ///<summary>
                ///</summary>
                ///<param name="Begin flattening the iFrom">th entry of the FROM clause</param>
                ///<param name="in the outer query.">in the outer query.</param>
                ///<param name=""></param>
                pSub = pSub1 = pSubitem.pSelect;
                ///
                ///<summary>
                ///Delete the transient table structure associated with the
                ///subquery
                ///
                ///</summary>
                db.sqlite3DbFree(ref pSubitem.zDatabase);
                db.sqlite3DbFree(ref pSubitem.zName);
                db.sqlite3DbFree(ref pSubitem.zAlias);
                pSubitem.zDatabase = null;
                pSubitem.zName = null;
                pSubitem.zAlias = null;
                pSubitem.pSelect = null;
                ///
                ///<summary>
                ///Defer deleting the Table object associated with the
                ///subquery until code generation is
                ///complete, since there may still exist Expr.pTab entries that
                ///refer to the subquery even after flattening.  Ticket #3346.
                ///
                ///</summary>
                ///<param name="pSubitem">NULL by test restrictions and tests above.</param>
                ///<param name=""></param>
                if (ALWAYS(pSubitem.pTab != null))
                {
                    Table pTabToDel = pSubitem.pTab;
                    if (pTabToDel.nRef == 1)
                    {
                        Parse pToplevel = sqlite3ParseToplevel(pParse);
                        pTabToDel.pNextZombie = pToplevel.pZombieTab;
                        pToplevel.pZombieTab = pTabToDel;
                    }
                    else
                    {
                        pTabToDel.nRef--;
                    }
                    pSubitem.pTab = null;
                }
                ///
                ///<summary>
                ///</summary>
                ///<param name="The following loop runs once for each term in a compound">subquery</param>
                ///<param name="flattening (as described above).  If we are doing a different kind">flattening (as described above).  If we are doing a different kind</param>
                ///<param name="of flattening "></param>
                ///<param name="then this loop only runs once.">then this loop only runs once.</param>
                ///<param name=""></param>
                ///<param name="This loop moves all of the FROM elements of the subquery into the">This loop moves all of the FROM elements of the subquery into the</param>
                ///<param name="the FROM clause of the outer query.  Before doing this, remember">the FROM clause of the outer query.  Before doing this, remember</param>
                ///<param name="the cursor number for the original outer query FROM element in">the cursor number for the original outer query FROM element in</param>
                ///<param name="iParent.  The iParent cursor will never be used.  Subsequent code">iParent.  The iParent cursor will never be used.  Subsequent code</param>
                ///<param name="will scan expressions looking for iParent references and replace">will scan expressions looking for iParent references and replace</param>
                ///<param name="those references with expressions that resolve to the subquery FROM">those references with expressions that resolve to the subquery FROM</param>
                ///<param name="elements we are now copying in.">elements we are now copying in.</param>
                ///<param name=""></param>
                for (pParent = p; pParent != null; pParent = pParent.pPrior, pSub = pSub.pPrior)
                {
                    int nSubSrc;
                    u8 jointype = 0;
                    pSubSrc = pSub.pSrc;
                    ///
                    ///<summary>
                    ///FROM clause of subquery 
                    ///</summary>
                    nSubSrc = pSubSrc.nSrc;
                    ///
                    ///<summary>
                    ///Number of terms in subquery FROM clause 
                    ///</summary>
                    pSrc = pParent.pSrc;
                    ///
                    ///<summary>
                    ///FROM clause of the outer query 
                    ///</summary>
                    if (pSrc != null)
                    {
                        Debug.Assert(pParent == p);
                        ///
                        ///<summary>
                        ///First time through the loop 
                        ///</summary>
                        jointype = pSubitem.jointype;
                    }
                    else
                    {
                        Debug.Assert(pParent != p);
                        ///
                        ///<summary>
                        ///2nd and subsequent times through the loop 
                        ///</summary>
                        pSrc = pParent.pSrc = build.sqlite3SrcListAppend(db, null, null, null);
                        //if ( pSrc == null )
                        //{
                        //  //Debug.Assert( db.mallocFailed != 0 );
                        //  break;
                        //}
                    }
                    ///
                    ///<summary>
                    ///The subquery uses a single slot of the FROM clause of the outer
                    ///query.  If the subquery has more than one element in its FROM clause,
                    ///then expand the outer query to make space for it to hold all elements
                    ///of the subquery.
                    ///
                    ///Example:
                    ///
                    ///SELECT * FROM tabA, (SELECT * FROM sub1, sub2), tabB;
                    ///
                    ///The outer query has 3 slots in its FROM clause.  One slot of the
                    ///outer query (the middle slot) is used by the subquery.  The next
                    ///block of code will expand the out query to 4 slots.  The middle
                    ///slot is expanded to two slots in order to make space for the
                    ///two elements in the FROM clause of the subquery.
                    ///
                    ///</summary>
                    if (nSubSrc > 1)
                    {
                        pParent.pSrc = pSrc = build.sqlite3SrcListEnlarge(db, pSrc, nSubSrc - 1, iFrom + 1);
                        //if ( db.mallocFailed != 0 )
                        //{
                        //  break;
                        //}
                    }
                    ///
                    ///<summary>
                    ///Transfer the FROM clause terms from the subquery into the
                    ///outer query.
                    ///
                    ///</summary>
                    for (i = 0; i < nSubSrc; i++)
                    {
                        build.sqlite3IdListDelete(db, ref pSrc.a[i + iFrom].pUsing);
                        pSrc.a[i + iFrom] = pSubSrc.a[i];
                        pSubSrc.a[i] = new SrcList_item();
                        //memset(pSubSrc.a[i], 0, sizeof(pSubSrc.a[i]));
                    }
                    pSubitem = pSrc.a[iFrom];
                    // Reset for C#
                    pSrc.a[iFrom].jointype = jointype;
                    ///
                    ///<summary>
                    ///Now begin substituting subquery result set expressions for
                    ///references to the iParent in the outer query.
                    ///
                    ///Example:
                    ///
                    ///SELECT a+5, b*10 FROM (SELECT x*3 AS a, y+10 AS b FROM t1) WHERE a>b;
                    ///\                     \_____________ subquery __________/          /
                    ///\_____________________ outer query ______________________________/
                    ///
                    ///We look at every expression in the outer query and every place we see
                    ///"a" we substitute "x*3" and every place we see "b" we substitute "y+10".
                    ///
                    ///</summary>
                    pList = pParent.pEList;
                    for (i = 0; i < pList.nExpr; i++)
                    {
                        if (pList.a[i].zName == null)
                        {
                            string zSpan = pList.a[i].zSpan;
                            if (ALWAYS(zSpan))
                            {
                                pList.a[i].zName = zSpan;
                                // sqlite3DbStrDup( db, zSpan );
                            }
                        }
                    }
                    substExprList(db, pParent.pEList, iParent, pSub.pEList);
                    if (isAgg)
                    {
                        substExprList(db, pParent.pGroupBy, iParent, pSub.pEList);
                        pParent.pHaving = substExpr(db, pParent.pHaving, iParent, pSub.pEList);
                    }
                    if (pSub.pOrderBy != null)
                    {
                        Debug.Assert(pParent.pOrderBy == null);
                        pParent.pOrderBy = pSub.pOrderBy;
                        pSub.pOrderBy = null;
                    }
                    else
                        if (pParent.pOrderBy != null)
                        {
                            substExprList(db, pParent.pOrderBy, iParent, pSub.pEList);
                        }
                    if (pSub.pWhere != null)
                    {
                        pWhere = exprc.sqlite3ExprDup(db, pSub.pWhere, 0);
                    }
                    else
                    {
                        pWhere = null;
                    }
                    if (subqueryIsAgg)
                    {
                        Debug.Assert(pParent.pHaving == null);
                        pParent.pHaving = pParent.pWhere;
                        pParent.pWhere = pWhere;
                        pParent.pHaving = substExpr(db, pParent.pHaving, iParent, pSub.pEList);
                        pParent.pHaving = exprc.sqlite3ExprAnd(db, pParent.pHaving, exprc.sqlite3ExprDup(db, pSub.pHaving, 0));
                        Debug.Assert(pParent.pGroupBy == null);
                        pParent.pGroupBy = exprc.sqlite3ExprListDup(db, pSub.pGroupBy, 0);
                    }
                    else
                    {
                        pParent.pWhere = substExpr(db, pParent.pWhere, iParent, pSub.pEList);
                        pParent.pWhere = exprc.sqlite3ExprAnd(db, pParent.pWhere, pWhere);
                    }
                    ///
                    ///<summary>
                    ///The flattened query is distinct if either the inner or the
                    ///outer query is distinct.
                    ///
                    ///</summary>
                    pParent.selFlags = (pParent.selFlags | pSub.selFlags & SelectFlags.Distinct);
                    ///
                    ///<summary>
                    ///SELECT ... FROM (SELECT ... LIMIT a OFFSET b) LIMIT x OFFSET y;
                    ///
                    ///One is tempted to try to add a and b to combine the limits.  But this
                    ///does not work if either limit is negative.
                    ///
                    ///</summary>
                    if (pSub.pLimit != null)
                    {
                        pParent.pLimit = pSub.pLimit;
                        pSub.pLimit = null;
                    }
                }
                ///
                ///<summary>
                ///Finially, delete what is left of the subquery and return
                ///success.
                ///
                ///</summary>
                SelectMethods.sqlite3SelectDelete(db, ref pSub);
                SelectMethods.sqlite3SelectDelete(db, ref pSub1);
                return 1;
            }
#endif
            ///<summary>
            /// Analyze the SELECT statement passed as an argument to see if it
            /// is a min() or max() query. Return WHERE_ORDERBY_MIN or WHERE_ORDERBY_MAX if
            /// it is, or 0 otherwise. At present, a query is considered to be
            /// a min()/max() query if:
            ///
            ///   1. There is a single object in the FROM clause.
            ///
            ///   2. There is a single expression in the result set, and it is
            ///      either min(x) or max(x), where x is a column reference.
            ///</summary>
            public static u8 minMaxQuery(Select p)
            {
                Expr pExpr;
                ExprList pEList = p.pEList;
                if (pEList.nExpr != 1)
                    return WHERE_ORDERBY_NORMAL;
                pExpr = pEList.a[0].pExpr;
                if (pExpr.op != TK_AGG_FUNCTION)
                    return 0;
                if (NEVER(pExpr.ExprHasProperty(EP_xIsSelect)))
                    return 0;
                pEList = pExpr.x.pList;
                if (pEList == null || pEList.nExpr != 1)
                    return 0;
                if (pEList.a[0].pExpr.op != TK_AGG_COLUMN)
                    return WHERE_ORDERBY_NORMAL;
                Debug.Assert(!pExpr.ExprHasProperty(EP_IntValue));
                if (pExpr.u.zToken.Equals("min", StringComparison.InvariantCultureIgnoreCase))
                {
                    return WHERE_ORDERBY_MIN;
                }
                else
                    if (pExpr.u.zToken.Equals("max", StringComparison.InvariantCultureIgnoreCase))
                    {
                        return WHERE_ORDERBY_MAX;
                    }
                return WHERE_ORDERBY_NORMAL;
            }
            ///<summary>
            /// The select statement passed as the first argument is an aggregate query.
            /// The second argment is the associated aggregate-info object. This
            /// function tests if the SELECT is of the form:
            ///
            ///   SELECT count() FROM <tbl>
            ///
            /// where table is a database table, not a sub-select or view. If the query
            /// does match this pattern, then a pointer to the Table object representing
            /// <tbl> is returned. Otherwise, 0 is returned.
            ///
            ///</summary>
            public static Table isSimpleCount(Select p, AggInfo pAggInfo)
            {
                Table pTab;
                Expr pExpr;
                Debug.Assert(null == p.pGroupBy);
                if (p.pWhere != null || p.pEList.nExpr != 1 || p.pSrc.nSrc != 1 || p.pSrc.a[0].pSelect != null)
                {
                    return null;
                }
                pTab = p.pSrc.a[0].pTab;
                pExpr = p.pEList.a[0].pExpr;
                Debug.Assert(pTab != null && null == pTab.pSelect && pExpr != null);
                if (IsVirtual(pTab))
                    return null;
                if (pExpr.op != TK_AGG_FUNCTION)
                    return null;
                if ((pAggInfo.aFunc[0].pFunc.flags & SQLITE_FUNC_COUNT) == 0)
                    return null;
                if ((pExpr.Flags & ExprFlags.EP_Distinct) != 0)
                    return null;
                return pTab;
            }
            ///<summary>
            /// If the source-list item passed as an argument was augmented with an
            /// INDEXED BY clause, then try to locate the specified index. If there
            /// was such a clause and the named index cannot be found, return
            /// SQLITE_ERROR and leave an error in pParse. Otherwise, populate
            /// pFrom.pIndex and return SQLITE_OK.
            ///
            ///</summary>
            public static int sqlite3IndexedByLookup(Parse pParse, SrcList_item pFrom)
            {
                if (pFrom.pTab != null && pFrom.zIndex != null && pFrom.zIndex.Length != 0)
                {
                    Table pTab = pFrom.pTab;
                    string zIndex = pFrom.zIndex;
                    Index pIdx;
                    for (pIdx = pTab.pIndex; pIdx != null && !pIdx.zName.Equals(zIndex, StringComparison.InvariantCultureIgnoreCase); pIdx = pIdx.pNext)
                        ;
                    if (null == pIdx)
                    {
                        utilc.sqlite3ErrorMsg(pParse, "no such index: %s", zIndex);
                        pParse.checkSchema = 1;
                        return SQLITE_ERROR;
                    }
                    pFrom.pIndex = pIdx;
                }
                return SQLITE_OK;
            }
            ///<summary>
            /// No-op routine for the parse-tree walker.
            ///
            /// When this routine is the Walker.xExprCallback then expression trees
            /// are walked without any actions being taken at each node.  Presumably,
            /// when this routine is used for Walker.xExprCallback then
            /// Walker.xSelectCallback is set to do something useful for every
            /// subquery in the parser tree.
            ///
            ///</summary>
            public static int exprWalkNoop(Walker NotUsed, ref Expr NotUsed2)
            {
                UNUSED_PARAMETER2(NotUsed, NotUsed2);
                return WRC_Continue;
            }

#if !SQLITE_OMIT_SUBQUERY
            ///<summary>
            /// This is a Walker.xSelectCallback callback for the sqlite3SelectTypeInfo()
            /// interface.
            ///
            /// For each FROM-clause subquery, add Column.zType and Column.zColl
            /// information to the Table ure that represents the result set
            /// of that subquery.
            ///
            /// The Table ure that represents the result set was coned
            /// by selectExpander() but the type and collation information was omitted
            /// at that point because identifiers had not yet been resolved.  This
            /// routine is called after identifier resolution.
            ///</summary>
            static int selectAddSubqueryTypeInfo(Walker pWalker, Select p)
            {
                Parse pParse;
                int i;
                SrcList pTabList;
                SrcList_item pFrom;
                Debug.Assert((p.selFlags & SelectFlags.Resolved) != 0);
                if ((p.selFlags & SelectFlags.HasTypeInfo) == 0)
                {
                    p.selFlags |= SelectFlags.HasTypeInfo;
                    pParse = pWalker.pParse;
                    pTabList = p.pSrc;
                    for (i = 0; i < pTabList.nSrc; i++)//, pFrom++ )
                    {
                        pFrom = pTabList.a[i];
                        Table pTab = pFrom.pTab;
                        if (ALWAYS(pTab != null) && (pTab.tabFlags & TF_Ephemeral) != 0)
                        {
                            ///
                            ///<summary>
                            ///</summary>
                            ///<param name="A sub">query in the FROM clause of a SELECT </param>
                            Select pSel = pFrom.pSelect;
                            Debug.Assert(pSel != null);
                            while (pSel.pPrior != null)
                                pSel = pSel.pPrior;
                            Select.selectAddColumnTypeAndCollation(pParse, pTab.nCol, pTab.aCol, pSel);
                        }
                    }
                }
                return WRC_Continue;
            }
#endif
            ///<summary>
            /// This routine adds datatype and collating sequence information to
            /// the Table ures of all FROM-clause subqueries in a
            /// SELECT statement.
            ///
            /// Use this routine after name resolution.
            ///</summary>
            public static void sqlite3SelectAddTypeInfo(Parse pParse, Select pSelect)
            {
#if !SQLITE_OMIT_SUBQUERY
                Walker w = new Walker();
                w.xSelectCallback = selectAddSubqueryTypeInfo;
                w.xExprCallback = exprWalkNoop;
                w.pParse = pParse;
                w.sqlite3WalkSelect(pSelect);
#endif
            }
            ///<summary>
            /// This routine sets of a SELECT statement for processing.  The
            /// following is accomplished:
            ///
            ///     *  VDBE VdbeCursor numbers are assigned to all FROM-clause terms.
            ///     *  Ephemeral Table objects are created for all FROM-clause subqueries.
            ///     *  ON and USING clauses are shifted into WHERE statements
            ///     *  Wildcards "*" and "TABLE.*" in result sets are expanded.
            ///     *  Identifiers in expression are matched to tables.
            ///
            /// This routine acts recursively on all subqueries within the SELECT.
            ///
            ///</summary>

            ///<summary>
            /// Reset the aggregate accumulator.
            ///
            /// The aggregate accumulator is a set of memory cells that hold
            /// intermediate results while calculating an aggregate.  This
            /// routine simply stores NULLs in all of those memory cells.
            ///
            ///</summary>
            public static void resetAccumulator(Parse pParse, AggInfo pAggInfo)
            {
                Vdbe v = pParse.pVdbe;
                int i;
                AggInfo_func pFunc;
                if (pAggInfo.nFunc + pAggInfo.nColumn == 0)
                {
                    return;
                }
                for (i = 0; i < pAggInfo.nColumn; i++)
                {
                    v.sqlite3VdbeAddOp2(OpCode.OP_Null, 0, pAggInfo.aCol[i].iMem);
                }
                for (i = 0; i < pAggInfo.nFunc; i++)
                {
                    //, pFunc++){
                    pFunc = pAggInfo.aFunc[i];
                    v.sqlite3VdbeAddOp2(OpCode.OP_Null, 0, pFunc.iMem);
                    if (pFunc.iDistinct >= 0)
                    {
                        Expr pE = pFunc.pExpr;
                        Debug.Assert(!pE.ExprHasProperty(EP_xIsSelect));
                        if (pE.x.pList == null || pE.x.pList.nExpr != 1)
                        {
                            utilc.sqlite3ErrorMsg(pParse, "DISTINCT aggregates must have exactly one " + "argument");
                            pFunc.iDistinct = -1;
                        }
                        else
                        {
                            KeyInfo pKeyInfo = SelectMethods.keyInfoFromExprList(pParse, pE.x.pList);
                            v.sqlite3VdbeAddOp4(OP_OpenEphemeral, pFunc.iDistinct, 0, 0, pKeyInfo, P4_KEYINFO_HANDOFF);
                        }
                    }
                }
            }
            ///<summary>
            /// Invoke the OP_AggFinalize opcode for every aggregate function
            /// in the AggInfo structure.
            ///
            ///</summary>
            public static void finalizeAggFunctions(Parse pParse, AggInfo pAggInfo)
            {
                Vdbe v = pParse.pVdbe;
                int i;
                AggInfo_func pF;
                for (i = 0; i < pAggInfo.nFunc; i++)
                {
                    //, pF++){
                    pF = pAggInfo.aFunc[i];
                    ExprList pList = pF.pExpr.x.pList;
                    Debug.Assert(!pF.pExpr.ExprHasProperty(EP_xIsSelect));
                    v.sqlite3VdbeAddOp4(OP_AggFinal, pF.iMem, pList != null ? pList.nExpr : 0, 0, pF.pFunc, P4_FUNCDEF);
                }
            }
            ///
            ///<summary>
            ///Update the accumulator memory cells for an aggregate based on
            ///the current cursor position.
            ///
            ///</summary>
            public static void updateAccumulator(Parse pParse, AggInfo pAggInfo)
            {
                Vdbe v = pParse.pVdbe;
                int i;
                AggInfo_func pF;
                AggInfo_col pC;
                pAggInfo.directMode = 1;
                pParse.sqlite3ExprCacheClear();
                for (i = 0; i < pAggInfo.nFunc; i++)
                {
                    //, pF++){
                    pF = pAggInfo.aFunc[i];
                    int nArg;
                    int addrNext = 0;
                    int regAgg;
                    Debug.Assert(!pF.pExpr.ExprHasProperty(EP_xIsSelect));
                    ExprList pList = pF.pExpr.x.pList;
                    if (pList != null)
                    {
                        nArg = pList.nExpr;
                        regAgg = pParse.sqlite3GetTempRange(nArg);
                        pParse.sqlite3ExprCodeExprList(pList, regAgg, true);
                    }
                    else
                    {
                        nArg = 0;
                        regAgg = 0;
                    }
                    if (pF.iDistinct >= 0)
                    {
                        addrNext = v.sqlite3VdbeMakeLabel();
                        Debug.Assert(nArg == 1);
                        codeDistinct(pParse, pF.iDistinct, addrNext, 1, regAgg);
                    }
                    if ((pF.pFunc.flags & SQLITE_FUNC_NEEDCOLL) != 0)
                    {
                        CollSeq pColl = null;
                        ExprList_item pItem;
                        int j;
                        Debug.Assert(pList != null);
                        ///
                        ///<summary>
                        ///</summary>
                        ///<param name="pList!=0 if pF">>pFunc has NEEDCOLL </param>
                        for (j = 0; pColl == null && j < nArg; j++)
                        {
                            //, pItem++){
                            pItem = pList.a[j];
                            pColl = pParse.sqlite3ExprCollSeq(pItem.pExpr);
                        }
                        if (pColl == null)
                        {
                            pColl = pParse.db.pDfltColl;
                        }
                        v.sqlite3VdbeAddOp4(OP_CollSeq, 0, 0, 0, pColl, P4_COLLSEQ);
                    }
                    v.sqlite3VdbeAddOp4(OP_AggStep, 0, regAgg, pF.iMem, pF.pFunc, P4_FUNCDEF);
                    v.sqlite3VdbeChangeP5((u8)nArg);
                    pParse.sqlite3ExprCacheAffinityChange(regAgg, nArg);
                    pParse.sqlite3ReleaseTempRange(regAgg, nArg);
                    if (addrNext != 0)
                    {
                        v.sqlite3VdbeResolveLabel(addrNext);
                        pParse.sqlite3ExprCacheClear();
                    }
                }
                ///
                ///<summary>
                ///Before populating the accumulator registers, clear the column cache.
                ///Otherwise, if any of the required column values are already present 
                ///in registers, exprc.sqlite3ExprCode() may use OP_SCopy to copy the value
                ///</summary>
                ///<param name="to pC">>iMem. But by the time the value is used, the original register</param>
                ///<param name="may have been used, invalidating the underlying buffer holding the">may have been used, invalidating the underlying buffer holding the</param>
                ///<param name="text or blob value. See ticket [883034dcb5].">text or blob value. See ticket [883034dcb5].</param>
                ///<param name=""></param>
                ///<param name="Another solution would be to change the OP_SCopy used to copy cached">Another solution would be to change the OP_SCopy used to copy cached</param>
                ///<param name="values to an OP_Copy.">values to an OP_Copy.</param>
                ///<param name=""></param>
                pParse.sqlite3ExprCacheClear();
                for (i = 0; i < pAggInfo.nAccumulator; i++)//, pC++)
                {
                    pC = pAggInfo.aCol[i];
                    pParse.sqlite3ExprCode(pC.pExpr, pC.iMem);
                }
                pAggInfo.directMode = 0;
                pParse.sqlite3ExprCacheClear();
            }



#if SQLITE_DEBUG
																									    /*
*******************************************************************************
** The following code is used for testing and debugging only.  The code
** that follows does not appear in normal builds.
**
** These routines are used to print out the content of all or part of a
** parse structures such as Select or Expr.  Such printouts are useful
** for helping to understand what is happening inside the code generator
** during the execution of complex SELECT statements.
**
** These routine are not called anywhere from within the normal
** code base.  Then are intended to be called from within the debugger
** or from temporary "printf" statements inserted for debugging.
*/
    void sqlite3PrintExpr( Expr p )
    {
      if ( !ExprHasProperty( p, EP_IntValue ) && p.u.zToken != null )
      {
        sqlite3DebugPrintf( "(%s", p.u.zToken );
      }
      else
      {
        sqlite3DebugPrintf( "(%d", p.op );
      }
      if ( p.pLeft != null )
      {
        sqlite3DebugPrintf( " " );
        sqlite3PrintExpr( p.pLeft );
      }
      if ( p.pRight != null )
      {
        sqlite3DebugPrintf( " " );
        sqlite3PrintExpr( p.pRight );
      }
      sqlite3DebugPrintf( ")" );
    }
    void sqlite3PrintExprList( ExprList pList )
    {
      int i;
      for ( i = 0; i < pList.nExpr; i++ )
      {
        sqlite3PrintExpr( pList.a[i].pExpr );
        if ( i < pList.nExpr - 1 )
        {
          sqlite3DebugPrintf( ", " );
        }
      }
    }
    void sqlite3PrintSelect( Select p, int indent )
    {
      sqlite3DebugPrintf( "%*sSELECT(%p) ", indent, "", p );
      sqlite3PrintExprList( p.pEList );
      sqlite3DebugPrintf( "\n" );
      if ( p.pSrc != null )
      {
        string zPrefix;
        int i;
        zPrefix = "FROM";
        for ( i = 0; i < p.pSrc.nSrc; i++ )
        {
          SrcList_item pItem = p.pSrc.a[i];
          sqlite3DebugPrintf( "%*s ", indent + 6, zPrefix );
          zPrefix = "";
          if ( pItem.pSelect != null )
          {
            sqlite3DebugPrintf( "(\n" );
            sqlite3PrintSelect( pItem.pSelect, indent + 10 );
            sqlite3DebugPrintf( "%*s)", indent + 8, "" );
          }
          else if ( pItem.zName != null )
          {
            sqlite3DebugPrintf( "%s", pItem.zName );
          }
          if ( pItem.pTab != null )
          {
            sqlite3DebugPrintf( "(vtable: %s)", pItem.pTab.zName );
          }
          if ( pItem.zAlias != null )
          {
            sqlite3DebugPrintf( " AS %s", pItem.zAlias );
          }
          if ( i < p.pSrc.nSrc - 1 )
          {
            sqlite3DebugPrintf( "," );
          }
          sqlite3DebugPrintf( "\n" );
        }
      }
      if ( p.pWhere != null )
      {
        sqlite3DebugPrintf( "%*s WHERE ", indent, "" );
        sqlite3PrintExpr( p.pWhere );
        sqlite3DebugPrintf( "\n" );
      }
      if ( p.pGroupBy != null )
      {
        sqlite3DebugPrintf( "%*s GROUP BY ", indent, "" );
        sqlite3PrintExprList( p.pGroupBy );
        sqlite3DebugPrintf( "\n" );
      }
      if ( p.pHaving != null )
      {
        sqlite3DebugPrintf( "%*s HAVING ", indent, "" );
        sqlite3PrintExpr( p.pHaving );
        sqlite3DebugPrintf( "\n" );
      }
      if ( p.pOrderBy != null )
      {
        sqlite3DebugPrintf( "%*s ORDER BY ", indent, "" );
        sqlite3PrintExprList( p.pOrderBy );
        sqlite3DebugPrintf( "\n" );
      }
    }
    /* End of the structure debug printing code
    *****************************************************************************/
#endif
        }
    }
}
