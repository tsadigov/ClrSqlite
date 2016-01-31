using Community.CsharpSqlite.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
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
using Community.CsharpSqlite.Ast;
using System.Diagnostics;
using Community.CsharpSqlite.Utils;
using Community.CsharpSqlite.Engine;
using Community.CsharpSqlite.Compiler.Parser;

namespace Community.CsharpSqlite.Compiler.CodeGeneration
{
    public static class ForSelect
    {

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
        public static SqlResult codegenSelect(
            Sqlite3.ParseState pParse,
            /*The SELECT statement being coded.*/ Select pSelect,
            /*What to do with the query results */ref SelectDest pDest)
        {
            ///Return from sqlite3WhereBegin() 
            WhereInfo pWInfo;
            ///List of columns to extract. 
            ExprList pEList = new ExprList();
            ///List of tables to select from 
            SrcList SelectSourceList = new SrcList();
            ///Table to use for the distinct set 
            int distinct;
            ///Value to return from this function 
            var rc = (SqlResult)1;
            ///Address of an OP_OpenEphemeral instruction 
            int addrSortIndex;

#if !SQLITE_OMIT_EXPLAIN
            int iRestoreSelectId = pParse.iSelectId;
            pParse.iSelectId = pParse.iNextSelectId++;
#endif
            ///The database connection 
            Connection db = pParse.db;
            if (pSelect == null/*|| db.mallocFailed != 0 */|| pParse.nErr != 0)
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
                exprc.Delete(db, ref pSelect.pOrderBy); pSelect.pOrderBy = null;

                pSelect.Flags = (pSelect.Flags & ~SelectFlags.Distinct);
            }

            Select.sqlite3SelectPrep(pParse, pSelect, null);

            var pOrderBy = pSelect.pOrderBy;///The ORDER BY clause.  May be NULL 
            SelectSourceList = pSelect.FromSource;
            pEList = pSelect.ResultingFieldList;
            if (pParse.nErr != 0/*|| db.mallocFailed != 0*/)
            {
                goto select_end;
            }

            ///True for select lists like "count()" 
            var isAgg = (pSelect.Flags & SelectFlags.Aggregate) != 0;
            Debug.Assert(pEList != null);
            ///Begin generating code.
            ///The virtual machine under construction 
            var v = pParse.sqlite3GetVdbe();
            if (v == null)
                goto select_end;
            ///If writing to memory or generating a set
            ///only a single column may be output.

#if !SQLITE_OMIT_SUBQUERY
            if (SelectMethods.checkForMultiColumnSelectError(pParse, pDest, pEList.Count))
            {
                goto select_end;
            }
#endif

            ///Generate code for all subqueries in the FROM clause
#if !SQLITE_OMIT_SUBQUERY || !SQLITE_OMIT_VIEW
            for (var i = 0; pSelect.pPrior == null && i < SelectSourceList.Count; i++)
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
                pParse.nHeight += pSelect.SelectExprHeight();
                ///Check to see if the subquery can be absorbed into the parent. 
                isAggSub = (pSub.Flags & SelectFlags.Aggregate) != 0;
                if (SelectMethods.flattenSubquery(pParse, pSelect, i, isAgg, isAggSub) != 0)
                {
                    if (isAggSub)
                    {
                        isAgg = true;
                        pSelect.Flags |= SelectFlags.Aggregate;
                    }
                    i = -1;
                }
                else
                {
                    dest.Init(SelectResultType.EphemTab, pItem.iCursor);
                    Debug.Assert(0 == pItem.isPopulated);
                    SelectMethods.explainSetInteger(ref pItem.iSelectId, (int)pParse.iNextSelectId);
                    codegenSelect(pParse, pSub, ref dest);
                    pItem.isPopulated = 1;
                    pItem.TableReference.nRowEst = (uint)pSub.nSelectRow;
                }
                //if ( /* pParse.nErr != 0 || */ db.mallocFailed != 0 )
                //{
                //  goto select_end;
                //}
                pParse.nHeight -= pSelect.SelectExprHeight();
                SelectSourceList = pSelect.FromSource;
                if (!(pDest.eDest <= SelectResultType.Discard))//        if( null==IgnorableOrderby(pDest) )
                {
                    pOrderBy = pSelect.pOrderBy;
                }
            }
            pEList = pSelect.ResultingFieldList;
#endif
            var pWhere = pSelect.pWhere;
            var pGroupBy = pSelect.pGroupBy;///The GROUP BY clause.  May be NULL 
            var pHaving = pSelect.pHaving;///The HAVING clause.  May be NULL 

            ///True if the DISTINCT keyword is present 
            bool isDistinct = (pSelect.Flags & SelectFlags.Distinct) != 0;
#if !SQLITE_OMIT_COMPOUND_SELECT
            ///If there is are a sequence of queries, do the earlier ones first.
            if (pSelect.pPrior != null)
            {
                if (pSelect.pRightmost == null)
                {
                    Select pLoop, pRight = null;
                    int cnt = 0;
                    int mxSelect;
                    for (pLoop = pSelect; pLoop != null; pLoop = pLoop.pPrior, cnt++)
                    {
                        pLoop.pRightmost = pSelect;
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
                rc = multiSelect(pParse, pSelect, pDest);
                SelectMethods.explainSetInteger(ref pParse.iSelectId, iRestoreSelectId);
                return rc;
            }
#endif
            ///If possible, rewrite the query to use GROUP BY instead of DISTINCT.
            ///GROUP BY might use an index, DISTINCT never does.
            Debug.Assert(pSelect.pGroupBy == null || (pSelect.Flags & SelectFlags.Aggregate) != 0);
            if ((pSelect.Flags & (SelectFlags.Distinct | SelectFlags.Aggregate)) == SelectFlags.Distinct)
            {
                pSelect.pGroupBy = exprc.Duplicate(db, pSelect.ResultingFieldList, 0);
                pGroupBy = pSelect.pGroupBy;
                pSelect.Flags = (pSelect.Flags & ~SelectFlags.Distinct);
            }
            ///If there is both a GROUP BY and an ORDER BY clause and they are
            ///identical, then disable the ORDER BY clause since the GROUP BY
            ///will cause elements to come out in the correct order.  This is
            ///<param name="an optimization "> the correct answer should result regardless.</param>
            ///<param name="Use the SQLITE_GroupByOrder flag with SQLITE_TESTCTRL_OPTIMIZER">Use the SQLITE_GroupByOrder flag with SQLITE_TESTCTRL_OPTIMIZER</param>
            ///<param name="to disable this optimization for testing purposes.">to disable this optimization for testing purposes.</param>
            ///<param name=""></param>
            if (exprc.sqlite3ExprListCompare(pSelect.pGroupBy, pOrderBy) == 0 && (db.flags & SqliteFlags.SQLITE_GroupByOrder) == 0)
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
                pOrderBy.iECursor = pParse.AllocatedCursorCount++;
                pSelect.addrOpenEphm[2] = addrSortIndex = v.sqlite3VdbeAddOp4(OpCode.OP_OpenEphemeral, pOrderBy.iECursor, pOrderBy.Count + 2, 0, pKeyInfo);
            }
            else
            {
                addrSortIndex = -1;
            }
            ///If the output is destined for a temporary table, open that table.
            if (pDest.eDest == SelectResultType.EphemTab)
            {
                v.sqlite3VdbeAddOp2(OpCode.OP_OpenEphemeral, pDest.iParm, pEList.Count);
            }
            ///Set the limiter.
            var iEnd = v.sqlite3VdbeMakeLabel();///Address of the end of the query 
            pSelect.nSelectRow = (double)IntegerExtensions.LARGEST_INT64;
            SelectMethods.computeLimitRegisters(pParse, pSelect, iEnd);
            ///Open a virtual index to use for the distinct set.
            if ((pSelect.Flags & SelectFlags.Distinct) != 0)
            {
                KeyInfo pKeyInfo;
                Debug.Assert(isAgg || pGroupBy != null);
                distinct = pParse.AllocatedCursorCount++;
                pKeyInfo = SelectMethods.keyInfoFromExprList(pParse, pSelect.ResultingFieldList);
                v.sqlite3VdbeAddOp4(OpCode.OP_OpenEphemeral, distinct, 0, 0, pKeyInfo);
                v.ChangeP5(Sqlite3.BTREE_UNORDERED);
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
                if (pWInfo.nRowOut < pSelect.nSelectRow)
                    pSelect.nSelectRow = pWInfo.nRowOut;
                ///If sorting index that was created by a prior OP_OpenEphemeral
                ///instruction ended up not being needed, then change the OP_OpenEphemeral
                ///into an OpCode.OP_Noop.
                if (addrSortIndex >= 0 && pOrderBy == null)
                {
                    Engine.vdbeaux.sqlite3VdbeChangeToNoop(v, addrSortIndex, 1);
                    pSelect.addrOpenEphm[2] = -1;
                }
                ///Use the standard inner loop
                Debug.Assert(!isDistinct);
                SelectMethods.selectInnerLoop(pParse, pSelect, pEList, 0, 0, pOrderBy, -1, pDest, pWInfo.iContinue, pWInfo.iBreak);
                ///End the database scan loop.
                pWInfo.sqlite3WhereEnd();
            }
            else
            {
                ///This is the processing for aggregate queries 

                ///First Mem address for previous GROUP BY 
                int iBMem;
                ///Mem address holding flag indicating that at least
                ///one row of the input to the aggregator has been
                ///processed 
                int iUseFlag;

                ///Rows come from source in GR BY' clause thanROUP BY order 
                int groupBySort;

                ///Remove any and all aliases between the result set and the
                ///GROUP BY clause.
                if (pGroupBy != null)
                {
                    ///For looping over expression in a list 
                    for (var k = pSelect.ResultingFieldList.Count; k > 0; k--)//, pItem++)
                    {
                        var pItem = pSelect.ResultingFieldList.a[pSelect.ResultingFieldList.Count - k];
                        pItem.iAlias = 0;
                    }
                    for (var k = pGroupBy.Count; k > 0; k--)//, pItem++ )
                    {
                        var pItem = pGroupBy.a[pGroupBy.Count - k];
                        pItem.iAlias = 0;
                    }
                    if (pSelect.nSelectRow > (double)100)
                        pSelect.nSelectRow = (double)100;
                }
                else
                {
                    pSelect.nSelectRow = (double)1;
                }
                ///Create a label to jump to when we want to abort the query 
                var addrEnd = v.sqlite3VdbeMakeLabel();///End of processing for this SELECT 
                                                       ///Convert TokenType.TK_COLUMN nodes into TokenType.TK_AGG_COLUMN and make entries in
                                                       ///sAggInfo for all TokenType.TK_AGG_FUNCTION nodes in expressions of the
                                                       ///SELECT statement.
                var sNC = new NameContext()///Name context for processing aggregate information 
                {
                    ParseState = pParse,
                    pSrcList = SelectSourceList,
                    pAggInfo = sAggInfo
                };
                // memset(sNC, 0, sNC).Length;

                sAggInfo.nSortingColumn = pGroupBy != null ? pGroupBy.Count + 1 : 0;
                sAggInfo.pGroupBy = pGroupBy;
                exprc.sqlite3ExprAnalyzeAggList(sNC, pEList);
                exprc.sqlite3ExprAnalyzeAggList(sNC, pOrderBy);
                if (pHaving != null)
                {
                    exprc.sqlite3ExprAnalyzeAggregates(sNC, ref pHaving);
                }
                sAggInfo.nAccumulator = sAggInfo.nColumn;
                for (var i = 0; i < sAggInfo.nFunc; i++)
                {
                    Debug.Assert(!sAggInfo.aFunc[i].pExpr.HasProperty(ExprFlags.EP_xIsSelect));
                    exprc.sqlite3ExprAnalyzeAggList(sNC, sAggInfo.aFunc[i].pExpr.x.pList);
                }
                //      if ( db.mallocFailed != 0 ) goto select_end;
                ///Processing for aggregates with GROUP BY is very different and
                ///much more complex than aggregates without a GROUP BY.
                if (pGroupBy != null)
                {
                    int j1;

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
                    sAggInfo.sortingIdx = pParse.AllocatedCursorCount++;
                    var pKeyInfo = SelectMethods.keyInfoFromExprList(pParse, pGroupBy);///Keying information for the group by clause 
                    addrSortingIdx = v.sqlite3VdbeAddOp4(OpCode.OP_OpenEphemeral, sAggInfo.sortingIdx, sAggInfo.nSortingColumn, 0, pKeyInfo);
                    ///Initialize memory locations used by GROUP BY aggregate processing
                    ///x
                    iUseFlag = ++pParse.UsedCellCount;
                    var iAbortFlag = ++pParse.UsedCellCount;///Mem address which causes query abort if positive 
                    var regOutputRow = ++pParse.UsedCellCount;///Start of subroutine that outputs a result row 
                    var addrOutputRow = v.sqlite3VdbeMakeLabel();///<param name="A">B comparision jump </param>
                    regReset = ++pParse.UsedCellCount;
                    addrReset = v.sqlite3VdbeMakeLabel();
                    var iAMem = pParse.UsedCellCount + 1;///First Mem address for storing current GROUP BY 
                    pParse.UsedCellCount += pGroupBy.Count;
                    iBMem = pParse.UsedCellCount + 1;
                    pParse.UsedCellCount += pGroupBy.Count;
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
                        pGroupBy = pSelect.pGroupBy;
                        groupBySort = 0;
                    }
                    else
                    {
                        ///Rows are coming out in undetermined order.  We have to push
                        ///each row into a sorting index, terminate the first loop,
                        ///then loop over the sorting index in order to get the output
                        ///in sorted order
                        SelectMethods.explainTempTable(pParse, isDistinct && 0 == (pSelect.Flags & SelectFlags.Distinct) ? "DISTINCT" : "GROUP BY");
                        groupBySort = 1;
                        var nGroupBy = pGroupBy.Count;
                        var nCol = nGroupBy + 1;
                        var j = nGroupBy + 1;
                        for (var i = 0; i < sAggInfo.nColumn; i++)
                        {
                            if (sAggInfo.aCol[i].iSorterColumn >= j)
                            {
                                nCol++;
                                j++;
                            }
                        }
                        var regBase = pParse.sqlite3GetTempRange(nCol);
                        pParse.sqlite3ExprCacheClear();
                        pParse.sqlite3ExprCodeExprList(pGroupBy, regBase, false);
                        v.sqlite3VdbeAddOp2(OpCode.OP_Sequence, sAggInfo.sortingIdx, regBase + nGroupBy);
                        j = nGroupBy + 1;
                        for (var i = 0; i < sAggInfo.nColumn; i++)
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
                        var regRecord = pParse.allocTempReg();
                        v.AddOpp3(OpCode.OP_MakeRecord, regBase, nCol, regRecord);
                        v.sqlite3VdbeAddOp2(OpCode.OP_IdxInsert, sAggInfo.sortingIdx, regRecord);
                        pParse.deallocTempReg(regRecord);
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
                    for (var j = 0; j < pGroupBy.Count; j++)
                    {
                        if (groupBySort != 0)
                        {
                            v.AddOpp3(OpCode.OP_Column, sAggInfo.sortingIdx, j, iBMem + j);
                        }
                        else
                        {
                            sAggInfo.directMode = 1;
                            pParse.sqlite3ExprCode(pGroupBy.a[j].pExpr, iBMem + j);
                        }
                    }
                    v.sqlite3VdbeAddOp4(OpCode.OP_Compare, iAMem, iBMem, pGroupBy.Count, pKeyInfo, P4Usage.P4_KEYINFO);
                    j1 = v.sqlite3VdbeCurrentAddr();
                    v.AddOpp3(OpCode.OP_Jump, j1 + 1, 0, j1 + 1);
                    ///Generate code that runs whenever the GROUP BY changes.
                    ///Changes in the GROUP BY are detected by the previous code
                    ///block.  If there were no changes, this block is skipped.
                    ///
                    ///This code copies current group by terms in b0,b1,b2,...
                    ///over to a0,a1,a2.  It then calls the output subroutine
                    ///and resets the aggregate accumulator registers in preparation
                    ///for the next GROUP BY batch.
                    pParse.sqlite3ExprCodeMove(iBMem, iAMem, pGroupBy.Count);
                    v.sqlite3VdbeAddOp2(OpCode.OP_Gosub, regOutputRow, addrOutputRow);
                    v.sqlite3VdbeAddOp2(OpCode.OP_IfPos, iAbortFlag, addrEnd);
                    v.sqlite3VdbeAddOp2(OpCode.OP_Gosub, regReset, addrReset);

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
                    var addrSetAbort = v.sqlite3VdbeCurrentAddr();///Return address register for output subroutine 
                    v.sqlite3VdbeAddOp2(OpCode.OP_Integer, 1, iAbortFlag);
                    v.VdbeComment("set abort flag");
                    v.AddOpp1(OpCode.OP_Return, regOutputRow);
                    v.sqlite3VdbeResolveLabel(addrOutputRow);
                    addrOutputRow = v.sqlite3VdbeCurrentAddr();
                    v.sqlite3VdbeAddOp2(OpCode.OP_IfPos, iUseFlag, addrOutputRow + 2);
                    v.VdbeComment("Groupby result generator entry point");
                    v.AddOpp1(OpCode.OP_Return, regOutputRow);
                    SelectMethods.finalizeAggFunctions(pParse, sAggInfo);
                    pParse.sqlite3ExprIfFalse(pHaving, addrOutputRow + 1, sqliteinth.SQLITE_JUMPIFNULL);
                    SelectMethods.selectInnerLoop(pParse, pSelect, pSelect.ResultingFieldList, 0, 0, pOrderBy, distinct, pDest, addrOutputRow + 1, addrSetAbort);
                    v.AddOpp1(OpCode.OP_Return, regOutputRow);
                    v.VdbeComment("end groupby result generator");
                    ///<param name="Generate a subroutine that will reset the group">by accumulator</param>
                    v.sqlite3VdbeResolveLabel(addrReset);
                    SelectMethods.resetAccumulator(pParse, sAggInfo);
                    v.AddOpp1(OpCode.OP_Return, regReset);
                }
                ///endif pGroupBy.  Begin aggregate queries without GROUP BY: 
                else
                {
                    ExprList pDel = null;
#if !SQLITE_OMIT_BTREECOUNT
                    Table pTab;
                    if ((pTab = SelectMethods.isSimpleCount(pSelect, sAggInfo)) != null)
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
                        int iDb = pParse.db.indexOfBackendWithSchema(pTab.pSchema);
                        int iCsr = pParse.AllocatedCursorCount++;
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
                        sqliteinth.TableLock(pParse, iDb, pTab.tnum, 0, pTab.zName);
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
                            pKeyInfo = builder.IndexBuilder.GetKeyinfo(pBest, pParse);
                        }
                        ///<param name="Open a read">only cursor, execute the OP_Count, close the cursor. </param>
                        v.AddOpp3(OpCode.OP_OpenRead, iCsr, iRoot, iDb);
                        if (pKeyInfo != null)
                        {
                            v.sqlite3VdbeChangeP4(-1, pKeyInfo, P4Usage.P4_KEYINFO_HANDOFF);
                        }
                        v.sqlite3VdbeAddOp2(OpCode.OP_Count, iCsr, sAggInfo.aFunc[0].iMem);
                        v.AddOpp1(OpCode.OP_Close, iCsr);
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
                        int flag = SelectMethods.minMaxQuery(pSelect);
                        if (flag != 0)
                        {
                            Debug.Assert(!pSelect.ResultingFieldList.a[0].pExpr.HasProperty(ExprFlags.EP_xIsSelect));
                            pMinMax = exprc.Duplicate(db, pSelect.ResultingFieldList.a[0].pExpr.x.pList, 0);
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
                            exprc.Delete(db, ref pDel);
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
                    SelectMethods.selectInnerLoop(pParse, pSelect, pSelect.ResultingFieldList, 0, 0, null, -1, pDest, addrEnd, addrEnd);
                    exprc.Delete(db, ref pDel);
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
                SelectMethods.generateSortTail(pParse, pSelect, v, pEList.Count, pDest);
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
            db.DbFree(ref sAggInfo.aCol);
            db.DbFree(ref sAggInfo.aFunc);
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
        static SqlResult multiSelect(Sqlite3.ParseState pParse,///
                                                               ///Parsing context 
            Select select,///
                          ///<param name="The right">most of SELECTs to be coded </param>
            SelectDest pDest///
                            ///What to do with query results 
            )
        {
            var rc = SqlResult.SQLITE_OK;///Success code from a subroutine 

            SelectDest dest = new SelectDest();///Alternative data destination 
            Select pDelete = null;///Chain of simple selects to delete 

#if !SQLITE_OMIT_EXPLAIN
            int iSub1 = 0;///<param name="EQP id of left">hand query </param>
            int iSub2 = 0;///<param name="EQP id of right">hand query </param>
#endif
            ///Make sure there is no ORDER BY or LIMIT clause on prior SELECTs.  Only
            ///<param name="the last (right">most) SELECT in the series may have an ORDER BY or LIMIT.</param>
            Debug.Assert(select != null && select.pPrior != null);
            ///Calling function guarantees this much 
            var db = pParse.db;///Database connection 
            var pPrior = select.pPrior;///Another SELECT immediately to our left 
            Debug.Assert(pPrior.pRightmost != pPrior);
            Debug.Assert(pPrior.pRightmost == select.pRightmost);
            dest = pDest;
            if (pPrior.pOrderBy != null)
            {
                utilc.sqlite3ErrorMsg(pParse, "ORDER BY clause should come after %s not before", SelectMethods.selectOpName(select.TokenOp));
                rc = (SqlResult)1;
                goto multi_select_end;
            }
            if (pPrior.pLimit != null)
            {
                utilc.sqlite3ErrorMsg(pParse, "LIMIT clause should come after %s not before", SelectMethods.selectOpName(select.TokenOp));
                rc = (SqlResult)1;
                goto multi_select_end;
            }
            var v = pParse.sqlite3GetVdbe();///Generate code to this VDBE 
            Debug.Assert(v != null);
            ///The VDBE already created by calling function 
            ///
            ///Create the destination temporary table if necessary
            if (dest.eDest == SelectResultType.EphemTab)
            {
                Debug.Assert(select.ResultingFieldList != null);
                v.sqlite3VdbeAddOp2(OpCode.OP_OpenEphemeral, dest.iParm, select.ResultingFieldList.Count);
                v.ChangeP5(Sqlite3.BTREE_UNORDERED);
                dest.eDest = SelectResultType.Table;
            }
            ///Make sure all SELECTs in the statement have the same number of elements
            ///in their result sets.
            Debug.Assert(select.ResultingFieldList != null && pPrior.ResultingFieldList != null);
            if (select.ResultingFieldList.Count != pPrior.ResultingFieldList.Count)
            {
                utilc.sqlite3ErrorMsg(pParse, "SELECTs to the left and right of %s" + " do not have the same number of result columns", SelectMethods.selectOpName(select.TokenOp));
                rc = (SqlResult)1;
                goto multi_select_end;
            }
            ///Compound SELECTs that have an ORDER BY clause are handled separately.
            if (select.pOrderBy != null)
            {
                return SelectMethods.multiSelectOrderBy(pParse, select, pDest);
            }
            ///Generate code for the left and right SELECT statements.
            switch (select.TokenOp)
            {
                case TokenType.TK_ALL:
                    {
                        int addr = 0;
                        int nLimit = 0;
                        Debug.Assert(pPrior.pLimit == null);
                        pPrior.pLimit = select.pLimit;
                        pPrior.pOffset = select.pOffset;
                        SelectMethods.explainSetInteger(ref iSub1, pParse.iNextSelectId);
                        rc = codegenSelect(pParse, pPrior, ref dest);
                        select.pLimit = null;
                        select.pOffset = null;
                        if (rc != 0)
                        {
                            goto multi_select_end;
                        }
                        select.pPrior = null;
                        select.iLimit = pPrior.iLimit;
                        select.iOffset = pPrior.iOffset;
                        if (select.iLimit != 0)
                        {
                            addr = v.AddOpp1(OpCode.OP_IfZero, select.iLimit);
                        }
                        SelectMethods.explainSetInteger(ref iSub2, pParse.iNextSelectId);
                        rc = codegenSelect(pParse, select, ref dest);
                        sqliteinth.testcase(rc != SqlResult.SQLITE_OK);
                        pDelete = select.pPrior;
                        select.pPrior = pPrior;
                        select.nSelectRow += pPrior.nSelectRow;
                        if (pPrior.pLimit != null && pPrior.pLimit.sqlite3ExprIsInteger(ref nLimit) && select.nSelectRow > (double)nLimit)
                        {
                            select.nSelectRow = (double)nLimit;
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
                        sqliteinth.testcase(select.TokenOp == TokenType.TK_EXCEPT);
                        sqliteinth.testcase(select.TokenOp == TokenType.TK_UNION);
                        priorOp = SelectResultType.Union;
                        if (dest.eDest == priorOp && Sqlite3.ALWAYS(null == select.pLimit && null == select.pOffset))
                        {
                            ///We can reuse a temporary table generated by a SELECT to our
                            ///right.
                            Debug.Assert(select.pRightmost != select);
                            ///Can only happen for leftward elements
                            ///<param name="of a 3">way or more compound </param>
                            Debug.Assert(select.pLimit == null);
                            ///Not allowed on leftward elements 
                            Debug.Assert(select.pOffset == null);
                            ///Not allowed on leftward elements 
                            unionTab = dest.iParm;
                        }
                        else
                        {
                            ///We will need to create our own temporary table to hold the
                            ///intermediate results.
                            unionTab = pParse.AllocatedCursorCount++;
                            Debug.Assert(select.pOrderBy == null);
                            addr = v.sqlite3VdbeAddOp2(OpCode.OP_OpenEphemeral, unionTab, 0);
                            Debug.Assert(select.addrOpenEphm[0] == -1);
                            select.addrOpenEphm[0] = addr;
                            select.pRightmost.Flags |= SelectFlags.UsesEphemeral;
                            Debug.Assert(select.ResultingFieldList != null);
                        }
                        ///Code the SELECT statements to our left
                        Debug.Assert(pPrior.pOrderBy == null);
                        uniondest.Init(priorOp, unionTab);
                        SelectMethods.explainSetInteger(ref iSub1, pParse.iNextSelectId);
                        rc = codegenSelect(pParse, pPrior, ref uniondest);
                        if (rc != 0)
                        {
                            goto multi_select_end;
                        }
                        ///Code the current SELECT statement
                        if (select.TokenOp == TokenType.TK_EXCEPT)
                        {
                            op = SelectResultType.Except;
                        }
                        else
                        {
                            Debug.Assert(select.TokenOp == TokenType.TK_UNION);
                            op = SelectResultType.Union;
                        }
                        select.pPrior = null;
                        pLimit = select.pLimit;
                        select.pLimit = null;
                        pOffset = select.pOffset;
                        select.pOffset = null;
                        uniondest.eDest = op;
                        SelectMethods.explainSetInteger(ref iSub2, pParse.iNextSelectId);
                        rc = codegenSelect(pParse, select, ref uniondest);
                        sqliteinth.testcase(rc != SqlResult.SQLITE_OK);
                        ///Query flattening in Select.sqlite3Select() might refill p.pOrderBy.
                        ///Be sure to delete p.pOrderBy, therefore, to avoid a memory leak. 
                        exprc.Delete(db, ref select.pOrderBy);
                        pDelete = select.pPrior;
                        select.pPrior = pPrior;
                        select.pOrderBy = null;
                        if (select.TokenOp == TokenType.TK_UNION)
                            select.nSelectRow += pPrior.nSelectRow;
                        exprc.Delete(db, ref select.pLimit);
                        select.pLimit = pLimit;
                        select.pOffset = pOffset;
                        select.iLimit = 0;
                        select.iOffset = 0;
                        ///Convert the data in the temporary table into whatever form
                        ///it is that we currently need.
                        Debug.Assert(unionTab == dest.iParm || dest.eDest != priorOp);
                        if (dest.eDest != priorOp)
                        {
                            int iCont, iBreak, iStart;
                            Debug.Assert(select.ResultingFieldList != null);
                            if (dest.eDest == SelectResultType.Output)
                            {
                                Select pFirst = select;
                                while (pFirst.pPrior != null)
                                    pFirst = pFirst.pPrior;
                                SelectMethods.generateColumnNames(pParse, null, pFirst.ResultingFieldList);
                            }
                            iBreak = v.sqlite3VdbeMakeLabel();
                            iCont = v.sqlite3VdbeMakeLabel();
                            SelectMethods.computeLimitRegisters(pParse, select, iBreak);
                            v.sqlite3VdbeAddOp2(OpCode.OP_Rewind, unionTab, iBreak);
                            iStart = v.sqlite3VdbeCurrentAddr();
                            SelectMethods.selectInnerLoop(pParse, select, select.ResultingFieldList, unionTab, select.ResultingFieldList.Count, null, -1, dest, iCont, iBreak);
                            v.sqlite3VdbeResolveLabel(iCont);
                            v.sqlite3VdbeAddOp2(OpCode.OP_Next, unionTab, iStart);
                            v.sqlite3VdbeResolveLabel(iBreak);
                            v.sqlite3VdbeAddOp2(OpCode.OP_Close, unionTab, 0);
                        }
                        break;
                    }
                default:
                    Debug.Assert(select.TokenOp == TokenType.TK_INTERSECT);
                    {
                        int iCont, iBreak, iStart;
                        Expr pLimit, pOffset;
                        int addr;
                        SelectDest intersectdest = new SelectDest();
                        int r1;
                        ///INTERSECT is different from the others since it requires
                        ///two temporary tables.  Hence it has its own case.  Begin
                        ///by allocating the tables we will need.
                        var tab1 = pParse.AllocatedCursorCount++;
                        var tab2 = pParse.AllocatedCursorCount++;
                        Debug.Assert(select.pOrderBy == null);
                        addr = v.sqlite3VdbeAddOp2(OpCode.OP_OpenEphemeral, tab1, 0);
                        Debug.Assert(select.addrOpenEphm[0] == -1);
                        select.addrOpenEphm[0] = addr;
                        select.pRightmost.Flags |= SelectFlags.UsesEphemeral;
                        Debug.Assert(select.ResultingFieldList != null);
                        ///Code the SELECTs to our left into temporary table "tab1".
                        intersectdest.Init(SelectResultType.Union, tab1);
                        SelectMethods.explainSetInteger(ref iSub1, pParse.iNextSelectId);
                        rc = codegenSelect(pParse, pPrior, ref intersectdest);
                        if (rc != 0)
                        {
                            goto multi_select_end;
                        }
                        ///Code the current SELECT into temporary table "tab2"
                        addr = v.sqlite3VdbeAddOp2(OpCode.OP_OpenEphemeral, tab2, 0);
                        Debug.Assert(select.addrOpenEphm[1] == -1);
                        select.addrOpenEphm[1] = addr;
                        select.pPrior = null;
                        pLimit = select.pLimit;
                        select.pLimit = null;
                        pOffset = select.pOffset;
                        select.pOffset = null;
                        intersectdest.iParm = tab2;
                        SelectMethods.explainSetInteger(ref iSub2, pParse.iNextSelectId);
                        rc = codegenSelect(pParse, select, ref intersectdest);
                        sqliteinth.testcase(rc != SqlResult.SQLITE_OK);
                        select.pPrior = pPrior;
                        if (select.nSelectRow > pPrior.nSelectRow)
                            select.nSelectRow = pPrior.nSelectRow;
                        exprc.Delete(db, ref select.pLimit);
                        select.pLimit = pLimit;
                        select.pOffset = pOffset;
                        ///Generate code to take the intersection of the two temporary
                        ///tables.
                        Debug.Assert(select.ResultingFieldList != null);
                        if (dest.eDest == SelectResultType.Output)
                        {
                            Select pFirst = select;
                            while (pFirst.pPrior != null)
                                pFirst = pFirst.pPrior;
                            SelectMethods.generateColumnNames(pParse, null, pFirst.ResultingFieldList);
                        }
                        iBreak = v.sqlite3VdbeMakeLabel();
                        iCont = v.sqlite3VdbeMakeLabel();
                        SelectMethods.computeLimitRegisters(pParse, select, iBreak);
                        v.sqlite3VdbeAddOp2(OpCode.OP_Rewind, tab1, iBreak);
                        r1 = pParse.allocTempReg();
                        iStart = v.sqlite3VdbeAddOp2(OpCode.OP_RowKey, tab1, r1);
                        v.sqlite3VdbeAddOp4Int(OpCode.OP_NotFound, tab2, iCont, r1, 0);
                        pParse.deallocTempReg(r1);
                        SelectMethods.selectInnerLoop(pParse, select, select.ResultingFieldList, tab1, select.ResultingFieldList.Count, null, -1, dest, iCont, iBreak);
                        v.sqlite3VdbeResolveLabel(iCont);
                        v.sqlite3VdbeAddOp2(OpCode.OP_Next, tab1, iStart);
                        v.sqlite3VdbeResolveLabel(iBreak);
                        v.sqlite3VdbeAddOp2(OpCode.OP_Close, tab2, 0);
                        v.sqlite3VdbeAddOp2(OpCode.OP_Close, tab1, 0);
                        break;
                    }
            }
            SelectMethods.explainComposite(pParse, select.TokenOp, iSub1, iSub2, select.TokenOp != TokenType.TK_ALL);
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
            if ((select.Flags & SelectFlags.UsesEphemeral) != 0)
            {

                Select pLoop;///For looping through SELECT statements 
                Debug.Assert(select.pRightmost == select);
                var nCol = select.ResultingFieldList.Count;///Number of columns in result set 
                var pKeyInfo = new KeyInfo()///Collating sequence for the result set 
                {
                    aColl = new CollSeq[nCol],
                    enc = db.aDbStatic[0].pSchema.enc,
                    nField = (u16)nCol
                };

                for (var i = 0; i < nCol; i++)
                {
                    //, apColl++){
                    var apColl = SelectMethods.multiSelectCollSeq(pParse, select, i);///For looping through pKeyInfo.aColl[] 
                    if (null == apColl)
                    {
                        apColl = db.pDfltColl;
                    }
                    pKeyInfo.aColl[i] = apColl;
                }
                for (pLoop = select; pLoop != null; pLoop = pLoop.pPrior)
                {
                    for (var i = 0; i < 2; i++)
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
                        v.sqlite3VdbeChangeP4(addr, pKeyInfo, P4Usage.P4_KEYINFO);
                        pLoop.addrOpenEphm[i] = -1;
                    }
                }
                db.DbFree(ref pKeyInfo);
            }
            multi_select_end:
            pDest.iMem = dest.iMem;
            pDest.nMem = dest.nMem;
            SelectMethods.SelectDestructor(db, ref pDelete);
            return rc;
        }
#endif

    }
}
