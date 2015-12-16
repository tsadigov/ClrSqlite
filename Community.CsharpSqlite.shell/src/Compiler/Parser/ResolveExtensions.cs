﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bitmask = System.UInt64;
using i16 = System.Int16;
using u8 = System.Byte;
using u16 = System.UInt16;
using u32 = System.UInt32;
using Community.CsharpSqlite.Compiler.Parser;
#if !SQLITE_MAX_VARIABLE_NUMBER
using ynVar = System.Int16;
using System.Diagnostics;
using Community.CsharpSqlite.Ast;
using Community.CsharpSqlite.Metadata;
using Community.CsharpSqlite.Utils;
using Community.CsharpSqlite.Metadata.Traverse;
#else
using ynVar = System.Int32; 
#endif
namespace Community.CsharpSqlite
{
    public partial class Sqlite3
    {

        public static class ResolveExtensions
        {

            ///<summary>
            /// 2008 August 18
            ///
            /// The author disclaims copyright to this source code.  In place of
            /// a legal notice, here is a blessing:
            ///
            ///    May you do good and not evil.
            ///    May you find forgiveness for yourself and forgive others.
            ///    May you share freely, never taking more than you give.
            ///
            ///
            ///
            /// This file contains routines used for walking the parser tree and
            /// resolve all identifiers by associating them with a particular
            /// table and column.
            ///
            ///  Included in SQLite3 port to C#-SQLite;  2008 Noah B Hart
            ///  C#-SQLite is an independent reimplementation of the SQLite software library
            ///
            ///  SQLITE_SOURCE_ID: 2010-08-23 18:52:01 42537b60566f288167f1b5864a5435986838e3a3
            ///
            ///
            ///
            ///</summary>
            //#include "sqliteInt.h"
            //#include <stdlib.h>
            //#include <string.h>
            ///<summary>
            /// Turn the pExpr expression into an alias for the iCol-th column of the
            /// result set in pEList.
            ///
            /// If the result set column is a simple column reference, then this routine
            /// makes an exact copy.  But for any other kind of expression, this
            /// routine make a copy of the result set column as the argument to the
            /// TokenType.TK_AS operator.  The TokenType.TK_AS operator causes the expression to be
            /// evaluated just once and then reused for each alias.
            ///
            /// The reason for suppressing the TokenType.TK_AS term when the expression is a simple
            /// column reference is so that the column reference will be recognized as
            /// usable by indices within the WHERE clause processing logic.
            ///
            /// Hack:  The TokenType.TK_AS operator is inhibited if zType[0]=='G'.  This means
            /// that in a GROUP BY clause, the expression is evaluated twice.  Hence:
            ///
            ///     SELECT random()%5 AS x, count(*) FROM tab GROUP BY x
            ///
            /// Is equivalent to:
            ///
            ///     SELECT random()%5 AS x, count(*) FROM tab GROUP BY random()%5
            ///
            /// The result of random()%5 in the GROUP BY clause is probably different
            /// from the result in the result-set.  We might fix this someday.  Or
            /// then again, we might not...
            ///
            ///</summary>
            public static void resolveAlias(Parse pParse,///
                ///Parsing context 
            ExprList pEList,///
                ///A result set 
            int iCol,///
                ///<param name="A column in the result set.  0..pEList.nExpr">1 </param>
            Expr pExpr,///
                ///Transform this into an alias to the result set 
            string zType///
                ///"GROUP" or "ORDER" or "" 
            )
            {
                Expr pOrig;
                ///<param name="The iCol">th column of the result set </param>
                Expr pDup;
                ///Copy of pOrig 
                Connection db;
                ///The database connection 
                Debug.Assert(iCol >= 0 && iCol < pEList.nExpr);
                pOrig = pEList.a[iCol].pExpr;
                Debug.Assert(pOrig != null);
                Debug.Assert((pOrig.Flags & ExprFlags.EP_Resolved) != 0);
                db = pParse.db;
                if (pOrig.Operator != TokenType.TK_COLUMN && (zType.Length == 0 || zType[0] != 'G'))
                {
                    pDup = exprc.sqlite3ExprDup(db, pOrig, 0);
                    pDup = pParse.sqlite3PExpr(TokenType.TK_AS, pDup, null, null);
                    if (pDup == null)
                        return;
                    if (pEList.a[iCol].iAlias == 0)
                    {
                        pEList.a[iCol].iAlias = (u16)(++pParse.nAlias);
                    }
                    pDup.iTable = pEList.a[iCol].iAlias;
                }
                else
                    if (pOrig.ExprHasProperty(ExprFlags.EP_IntValue) || pOrig.u.zToken == null)
                    {
                        pDup = exprc.sqlite3ExprDup(db, pOrig, 0);
                        if (pDup == null)
                            return;
                    }
                    else
                    {
                        string zToken = pOrig.u.zToken;
                        Debug.Assert(zToken != null);
                        pOrig.u.zToken = null;
                        pDup = exprc.sqlite3ExprDup(db, pOrig, 0);
                        pOrig.u.zToken = zToken;
                        if (pDup == null)
                            return;
                        Debug.Assert((pDup.Flags & (ExprFlags.EP_Reduced | ExprFlags.EP_TokenOnly)) == 0);
                        pDup.flags2 |= EP2_MallocedToken;
                        pDup.u.zToken = zToken;
                        // sqlite3DbStrDup( db, zToken );
                    }
                if ((pExpr.Flags & ExprFlags.EP_ExpCollate) != 0)
                {
                    pDup.CollatingSequence = pExpr.CollatingSequence;
                    pDup.Flags |= ExprFlags.EP_ExpCollate;
                }
                ///Before calling exprc.sqlite3ExprDelete(), set the ExprFlags.EP_Static flag. This 
                ///prevents ExprDelete() from deleting the Expr structure itself,
                ///allowing it to be repopulated by the memcpy() on the following line.
                pExpr.ExprSetProperty(ExprFlags.EP_Static);
                exprc.sqlite3ExprDelete(db, ref pExpr);
                pExpr.CopyFrom(pDup);
                //memcpy(pExpr, pDup, sizeof(*pExpr));
                db.sqlite3DbFree(ref pDup);
            }



            ///<summary>
            /// pEList is a list of expressions which are really the result set of the
            /// a SELECT statement.  pE is a term in an ORDER BY or GROUP BY clause.
            /// This routine checks to see if pE is a simple identifier which corresponds
            /// to the AS-name of one of the terms of the expression list.  If it is,
            /// this routine return an integer between 1 and N where N is the number of
            /// elements in pEList, corresponding to the matching entry.  If there is
            /// no match, or if pE is not a simple identifier, then this routine
            /// return 0.
            ///
            /// pEList has been resolved.  pE has not.
            ///
            ///</summary>
            public static int resolveAsName(Parse pParse,///
                ///Parsing context for error messages 
            ExprList pEList,///
                ///List of expressions to scan 
            Expr pE///
                ///Expression we are trying to match 
            )
            {
                int i;
                ///
                ///<summary>
                ///Loop counter 
                ///</summary>
                sqliteinth.UNUSED_PARAMETER(pParse);
                if (pE.Operator == TokenType.TK_ID)
                {
                    string zCol = pE.u.zToken;
                    for (i = 0; i < pEList.nExpr; i++)
                    {
                        string zAs = pEList.a[i].zName;
                        if (zAs != null && zAs.Equals(zCol, StringComparison.InvariantCultureIgnoreCase))
                        {
                            return i + 1;
                        }
                    }
                }
                return 0;
            }




            ///<summary>
            /// pE is a pointer to an expression which is a single term in the
            /// ORDER BY of a compound SELECT.  The expression has not been
            /// name resolved.
            ///
            /// At the point this routine is called, we already know that the
            /// ORDER BY term is not an integer index into the result set.  That
            /// case is handled by the calling routine.
            ///
            /// Attempt to match pE against result set columns in the left-most
            /// SELECT statement.  Return the index i of the matching column,
            /// as an indication to the caller that it should sort by the i-th column.
            /// The left-most column is 1.  In other words, the value returned is the
            /// same integer value that would be used in the SQL statement to indicate
            /// the column.
            ///
            /// If there is no match, return 0.  Return -1 if an error occurs.
            ///
            ///</summary>
            public static int resolveOrderByTermToExprList(Parse pParse,///
                ///Parsing context for error messages 
            Select pSelect,///
                ///The SELECT statement with the ORDER BY clause 
            Expr pE///
                ///The specific ORDER BY term 
            )
            {
                int i = 0;
                ///Loop counter 
                ExprList pEList;
                ///The columns of the result set 
                NameContext nc;
                ///Name context for resolving pE 
                Connection db;
                ///Database connection 
                SqlResult rc;
                ///Return code from subprocedures 
                u8 savedSuppErr;
                ///<param name="Saved value of db">>suppressErr </param>
                Debug.Assert( ! pE.sqlite3ExprIsInteger(ref i) );
                pEList = pSelect.pEList;
                ///Resolve all names in the ORDER BY term expression
                nc = new NameContext();
                // memset( &nc, 0, sizeof( nc ) );
                nc.pParse = pParse;
                nc.pSrcList = pSelect.pSrc;
                nc.pEList = pEList;
                nc.allowAgg = 1;
                nc.nErr = 0;
                db = pParse.db;
                savedSuppErr = db.suppressErr;
                db.suppressErr = 1;
                rc = sqlite3ResolveExprNames(nc, ref pE);
                db.suppressErr = savedSuppErr;
                if (rc != 0)
                    return 0;
                ///Try to match the ORDER BY expression against an expression
                ///<param name="in the result set.  Return an 1">based index of the matching</param>
                ///<param name="result">set entry.</param>
                for (i = 0; i < pEList.nExpr; i++)
                {
                    if (exprc.sqlite3ExprCompare(pEList.a[i].pExpr, pE) < 2)
                    {
                        return i + 1;
                    }
                }
                ///If no match, return 0. 
                return 0;
            }
		

            ///<summary>
            /// Allocate and return a pointer to an expression to load the column iCol
            /// from datasource iSrc in SrcList pSrc.
            ///
            ///</summary>
            public static Expr sqlite3CreateColumnExpr(Connection db, SrcList pSrc, int iSrc, int iCol)
            {
                Expr p = exprc.CreateExpr(db, TokenType.TK_COLUMN, null, false);
                if (p != null)
                {
                    SrcList_item pItem = pSrc.a[iSrc];
                    p.pTab = pItem.pTab;
                    p.iTable = pItem.iCursor;
                    if (p.pTab.iPKey == iCol)
                    {
                        p.iColumn = -1;
                    }
                    else
                    {
                        p.iColumn = (ynVar)iCol;
                        sqliteinth.testcase(iCol == Globals.BMS);
                        sqliteinth.testcase(iCol == Globals.BMS - 1);
                        pItem.colUsed |= ((Bitmask)1) << (iCol >= Globals.BMS ? Globals.BMS - 1 : iCol);
                    }
                    p.ExprSetProperty(ExprFlags.EP_Resolved);
                }
                return p;
            }


            ///<summary>
            /// Given the name of a column of the form X.Y.Z or Y.Z or just Z, look up
            /// that name in the set of source tables in pSrcList and make the pExpr
            /// expression node refer back to that source column.  The following changes
            /// are made to pExpr:
            ///
            ///    pExpr->iDb           Set the index in db->aDb[] of the database X
            ///                         (even if X is implied).
            ///    pExpr->iTable        Set to the cursor number for the table obtained
            ///                         from pSrcList.
            ///    pExpr->pTab          Points to the Table structure of X.Y (even if
            ///                         X and/or Y are implied.)
            ///    pExpr->iColumn       Set to the column number within the table.
            ///    pExpr->op            Set to TokenType.TK_COLUMN.
            ///    pExpr->pLeft         Any expression this points to is deleted
            ///    pExpr->pRight        Any expression this points to is deleted.
            ///
            /// The zDb variable is the name of the database (the "X").  This value may be
            /// NULL meaning that name is of the form Y.Z or Z.  Any available database
            /// can be used.  The zTable variable is the name of the table (the "Y").  This
            /// value can be NULL if zDb is also NULL.  If zTable is NULL it
            /// means that the form of the name is Z and that columns from any table
            /// can be used.
            ///
            /// If the name cannot be resolved unambiguously, leave an error message
            /// in pParse and return WRC.WRC_Abort.  Return WRC.WRC_Prune on success.
            ///
            ///</summary>
            public static WRC lookupName(Parse pParse,///
                ///The parsing context 
            string zDb,///
                ///Name of the database containing table, or NULL 
            string zTab,///
                ///Name of table containing column, or NULL 
            string zCol,///
                ///Name of the column. 
            NameContext pNC,///
                ///The name context used to resolve the name 
            Expr pExpr///
                ///Make this EXPR node point to the selected column 
            )
            {
                int i, j;
                ///Loop counters 
                int cnt = 0;
                ///Number of matching column names 
                int cntTab = 0;
                ///Number of matching table names 
                Connection db = pParse.db;
                ///The database connection 
                SrcList_item pItem;
                ///Use for looping over pSrcList items 
                SrcList_item pMatch = null;
                ///The matching pSrcList item 
                NameContext pTopNC = pNC;
                ///First namecontext in the list 
                Schema pSchema = null;
                ///Schema of the expression 
                int isTrigger = 0;
                Debug.Assert(pNC != null);
                ///the name context cannot be NULL. 
                Debug.Assert(zCol != null);
                ///The Z in X.Y.Z cannot be NULL 
                Debug.Assert(!pExpr.ExprHasAnyProperty(ExprFlags.EP_TokenOnly | ExprFlags.EP_Reduced));
                ///<param name="Initialize the node to no">match </param>
                pExpr.iTable = -1;
                pExpr.pTab = null;
                pExpr.ExprSetIrreducible();
                ///</summary>
                ///<param name="Start at the inner">most context and move outward until a match is found </param>
                while (pNC != null && cnt == 0)
                {
                    ExprList pEList;
                    SrcList pSrcList = pNC.pSrcList;
                    if (pSrcList != null)
                    {
                        for (i = 0; i < pSrcList.nSrc; i++)//, pItem++ )
                        {
                            pItem = pSrcList.a[i];
                            Table pTab;
                            int iDb;
                            Column pCol;
                            pTab = pItem.pTab;
                            Debug.Assert(pTab != null && pTab.zName != null);
                            iDb = sqlite3SchemaToIndex(db, pTab.pSchema);
                            Debug.Assert(pTab.nCol > 0);
                            if (zTab != null)
                            {
                                if (pItem.zAlias != null)
                                {
                                    string zTabName = pItem.zAlias;
                                    if (!zTabName.Equals(zTab, StringComparison.InvariantCultureIgnoreCase))
                                        continue;
                                }
                                else
                                {
                                    string zTabName = pTab.zName;
                                    if (NEVER(zTabName == null) || !zTabName.Equals(zTab, StringComparison.InvariantCultureIgnoreCase))
                                    {
                                        continue;
                                    }
                                    if (zDb != null && !db.Backends[iDb].Name.Equals(zDb, StringComparison.InvariantCultureIgnoreCase))
                                    {
                                        continue;
                                    }
                                }
                            }
                            if (0 == (cntTab++))
                            {
                                pExpr.iTable = pItem.iCursor;
                                pExpr.pTab = pTab;
                                pSchema = pTab.pSchema;
                                pMatch = pItem;
                            }
                            for (j = 0; j < pTab.nCol; j++)//, pCol++ )
                            {
                                pCol = pTab.aCol[j];
                                if (pCol.zName.Equals(zCol, StringComparison.InvariantCultureIgnoreCase))
                                {
                                    IdList pUsing;
                                    cnt++;
                                    pExpr.iTable = pItem.iCursor;
                                    pExpr.pTab = pTab;
                                    pMatch = pItem;
                                    pSchema = pTab.pSchema;
                                    ///<param name="Substitute the rowid (column ">1) for the INTEGER PRIMARY KEY </param>
                                    pExpr.iColumn = (short)(j == pTab.iPKey ? -1 : j);
                                    if (i < pSrcList.nSrc - 1)
                                    {
                                        if ((pSrcList.a[i + 1].jointype &  JoinType.JT_NATURAL) != 0)// pItem[1].jointype
                                        {
                                            ///If this match occurred in the left table of a natural join,
                                            ///then skip the right table to avoid a duplicate match 
                                            ///</summary>
                                            //pItem++;
                                            i++;
                                        }
                                        else
                                            if ((pUsing = pSrcList.a[i + 1].pUsing) != null)//pItem[1].pUsing
                                            {
                                                ///If this match occurs on a column that is in the USING clause
                                                ///of a join, skip the search of the right table of the join
                                                ///to avoid a duplicate match there. 
                                                int k;
                                                for (k = 0; k < pUsing.nId; k++)
                                                {
                                                    if (pUsing.a[k].zName.Equals(zCol, StringComparison.InvariantCultureIgnoreCase))
                                                    {
                                                        //pItem++;
                                                        i++;
                                                        break;
                                                    }
                                                }
                                            }
                                    }
                                    break;
                                }
                            }
                        }
                    }
#if !SQLITE_OMIT_TRIGGER
                    ///
                    ///<summary>
                    ///If we have not already resolved the name, then maybe
                    ///it is a new.* or old.* trigger argument reference
                    ///
                    ///</summary>
                    if (zDb == null && zTab != null && cnt == 0 && pParse.pTriggerTab != null)
                    {
                        var op = pParse.eTriggerOperator;
                        Table pTab = null;
                        Debug.Assert(op == TokenType.TK_DELETE || op == TokenType.TK_UPDATE || op == TokenType.TK_INSERT);
                        if (op != TokenType.TK_DELETE && "new".Equals(zTab, StringComparison.InvariantCultureIgnoreCase))
                        {
                            pExpr.iTable = 1;
                            pTab = pParse.pTriggerTab;
                        }
                        else
                            if (op != TokenType.TK_INSERT && "old".Equals(zTab, StringComparison.InvariantCultureIgnoreCase))
                            {
                                pExpr.iTable = 0;
                                pTab = pParse.pTriggerTab;
                            }
                        if (pTab != null)
                        {
                            int iCol;
                            pSchema = pTab.pSchema;
                            cntTab++;
                            for (iCol = 0; iCol < pTab.nCol; iCol++)
                            {
                                Column pCol = pTab.aCol[iCol];
                                if (pCol.zName.Equals(zCol, StringComparison.InvariantCultureIgnoreCase))
                                {
                                    if (iCol == pTab.iPKey)
                                    {
                                        iCol = -1;
                                    }
                                    break;
                                }
                            }
                            if (iCol >= pTab.nCol && exprc.sqlite3IsRowid(zCol))
                            {
                                iCol = -1;
                                ///
                                ///<summary>
                                ///</summary>
                                ///<param name="IMP: R">55124 </param>
                            }
                            if (iCol < pTab.nCol)
                            {
                                cnt++;
                                if (iCol < 0)
                                {
                                    pExpr.affinity = sqliteinth.SQLITE_AFF_INTEGER;
                                }
                                else
                                    if (pExpr.iTable == 0)
                                    {
                                        sqliteinth.testcase(iCol == 31);
                                        sqliteinth.testcase(iCol == 32);
                                        pParse.oldmask |= (iCol >= 32 ? 0xffffffff : (((u32)1) << iCol));
                                    }
                                    else
                                    {
                                        sqliteinth.testcase(iCol == 31);
                                        sqliteinth.testcase(iCol == 32);
                                        pParse.newmask |= (iCol >= 32 ? 0xffffffff : (((u32)1) << iCol));
                                    }
                                pExpr.iColumn = (i16)iCol;
                                pExpr.pTab = pTab;
                                isTrigger = 1;
                            }
                        }
                    }
#endif
                    ///Perhaps the name is a reference to the ROWID
                    if (cnt == 0 && cntTab == 1 && exprc.sqlite3IsRowid(zCol))
                    {
                        cnt = 1;
                        pExpr.iColumn = -1;
                        ///<param name="IMP: R">55124 </param>
                        pExpr.affinity = sqliteinth.SQLITE_AFF_INTEGER;
                    }
                    ///
                    ///<summary>
                    ///If the input is of the form Z (not Y.Z or X.Y.Z) then the name Z
                    ///</summary>
                    ///<param name="might refer to an result">set alias.  This happens, for example, when</param>
                    ///<param name="we are resolving names in the WHERE clause of the following command:">we are resolving names in the WHERE clause of the following command:</param>
                    ///<param name=""></param>
                    ///<param name="SELECT a+b AS x FROM table WHERE x<10;">SELECT a+b AS x FROM table WHERE x<10;</param>
                    ///<param name=""></param>
                    ///<param name="In cases like this, replace pExpr with a copy of the expression that">In cases like this, replace pExpr with a copy of the expression that</param>
                    ///<param name="forms the result set entry ("a+b" in the example) and return immediately.">forms the result set entry ("a+b" in the example) and return immediately.</param>
                    ///<param name="Note that the expression in the result set should have already been">Note that the expression in the result set should have already been</param>
                    ///<param name="resolved by the time the WHERE clause is resolved.">resolved by the time the WHERE clause is resolved.</param>
                    ///<param name=""></param>
                    if (cnt == 0 && (pEList = pNC.pEList) != null && zTab == null)
                    {
                        for (j = 0; j < pEList.nExpr; j++)
                        {
                            string zAs = pEList.a[j].zName;
                            if (zAs != null && zAs.Equals(zCol, StringComparison.InvariantCultureIgnoreCase))
                            {
                                Expr pOrig;
                                Debug.Assert(pExpr.pLeft == null && pExpr.pRight == null);
                                Debug.Assert(pExpr.x.pList == null);
                                Debug.Assert(pExpr.x.pSelect == null);
                                pOrig = pEList.a[j].pExpr;
                                if (0 == pNC.allowAgg && pOrig.ExprHasProperty(ExprFlags.EP_Agg))
                                {
                                    utilc.sqlite3ErrorMsg(pParse, "misuse of aliased aggregate %s", zAs);
                                    return WRC.WRC_Abort;
                                }
                                resolveAlias(pParse, pEList, j, pExpr, "");
                                cnt = 1;
                                pMatch = null;
                                Debug.Assert(zTab == null && zDb == null);
                                goto lookupname_end;
                            }
                        }
                    }
                    ///
                    ///<summary>
                    ///Advance to the next name context.  The loop will exit when either
                    ///we have a match (cnt>0) or when we run out of name contexts.
                    ///
                    ///</summary>
                    if (cnt == 0)
                    {
                        pNC = pNC.pNext;
                    }
                }
                ///
                ///<summary>
                ///If X and Y are NULL (in other words if only the column name Z is
                ///</summary>
                ///<param name="supplied) and the value of Z is enclosed in double">quotes, then</param>
                ///<param name="Z is a string literal if it doesn't match any column names.  In that">Z is a string literal if it doesn't match any column names.  In that</param>
                ///<param name="case, we need to return right away and not make any changes to">case, we need to return right away and not make any changes to</param>
                ///<param name="pExpr.">pExpr.</param>
                ///<param name=""></param>
                ///<param name="Because no reference was made to outer contexts, the pNC.nRef">Because no reference was made to outer contexts, the pNC.nRef</param>
                ///<param name="fields are not changed in any context.">fields are not changed in any context.</param>
                ///<param name=""></param>
                if (cnt == 0 && zTab == null && pExpr.ExprHasProperty(ExprFlags.EP_DblQuoted))
                {
                    pExpr.Operator = TokenType.TK_STRING;
                    pExpr.pTab = null;
                    return WRC.WRC_Prune;
                }
                ///
                ///<summary>
                ///cnt==0 means there was not match.  cnt>1 means there were two or
                ///more matches.  Either way, we have an error.
                ///
                ///</summary>
                if (cnt != 1)
                {
                    string zErr;
                    zErr = cnt == 0 ? "no such column" : "ambiguous column name";
                    if (zDb != null)
                    {
                        utilc.sqlite3ErrorMsg(pParse, "%s: %s.%s.%s", zErr, zDb, zTab, zCol);
                    }
                    else
                        if (zTab != null)
                        {
                            utilc.sqlite3ErrorMsg(pParse, "%s: %s.%s", zErr, zTab, zCol);
                        }
                        else
                        {
                            utilc.sqlite3ErrorMsg(pParse, "%s: %s", zErr, zCol);
                        }
                    pParse.checkSchema = 1;
                    pTopNC.nErr++;
                }
                ///
                ///<summary>
                ///If a column from a table in pSrcList is referenced, then record
                ///this fact in the pSrcList.a[].colUsed bitmask.  Column 0 causes
                ///bit 0 to be set.  Column 1 sets bit 1.  And so forth.  If the
                ///column number is greater than the number of bits in the bitmask
                ///</summary>
                ///<param name="then set the high">order bit of the bitmask.</param>
                ///<param name=""></param>
                if (pExpr.iColumn >= 0 && pMatch != null)
                {
                    int n = pExpr.iColumn;
                    sqliteinth.testcase(n == Globals.BMS - 1);
                    if (n >= Globals.BMS)
                    {
                        n = Globals.BMS - 1;
                    }
                    Debug.Assert(pMatch.iCursor == pExpr.iTable);
                    pMatch.colUsed |= ((Bitmask)1) << n;
                }
                ///Clean up and return
                exprc.sqlite3ExprDelete(db, ref pExpr.pLeft);
                pExpr.pLeft = null;
                exprc.sqlite3ExprDelete(db, ref pExpr.pRight);
                pExpr.pRight = null;
                pExpr.Operator = (isTrigger != 0 ? TokenType.TK_TRIGGER : TokenType.TK_COLUMN);
            lookupname_end:
                if (cnt == 1)
                {
                    Debug.Assert(pNC != null);
                    sqliteinth.sqlite3AuthRead(pParse, pExpr, pSchema, pNC.pSrcList);
                    ///Increment the nRef value on all name contexts from TopNC up to
                    ///the point where the name matched. 
                    for (; ; )
                    {
                        Debug.Assert(pTopNC != null);
                        pTopNC.nRef++;
                        if (pTopNC == pNC)
                            break;
                        pTopNC = pTopNC.pNext;
                    }
                    return WRC.WRC_Prune;
                }
                else
                {
                    return WRC.WRC_Abort;
                }
            }

            ///<summary>
            /// This routine is callback for sqlite3WalkExpr().
            ///
            /// Resolve symbolic names into TokenType.TK_COLUMN operators for the current
            /// node in the expression tree.  Return 0 to continue the search down
            /// the tree or 2 to abort the tree walk.
            ///
            /// This routine also does error checking and name resolution for
            /// function names.  The operator for aggregate functions is changed
            /// to TokenType.TK_AGG_FUNCTION.
            ///
            ///</summary>
            public static WRC resolveExprStep(Walker pWalker, ref Expr pExpr)
            {
                NameContext pNC;
                Parse pParse;
                pNC = pWalker.u.pNC;
                Debug.Assert(pNC != null);
                pParse = pNC.pParse;
                Debug.Assert(pParse == pWalker.pParse);
                if (pExpr.ExprHasAnyProperty(ExprFlags.EP_Resolved))
                    return WRC.WRC_Prune;
                pExpr.ExprSetProperty(ExprFlags.EP_Resolved);
#if !NDEBUG
																																																																		      if ( pNC.pSrcList != null && pNC.pSrcList.nAlloc > 0 )
      {
        SrcList pSrcList = pNC.pSrcList;
        int i;
        for ( i = 0; i < pNC.pSrcList.nSrc; i++ )
        {
          Debug.Assert( pSrcList.a[i].iCursor >= 0 && pSrcList.a[i].iCursor < pParse.nTab );
        }
      }
#endif
                switch (pExpr.Operator)
                {
#if (SQLITE_ENABLE_UPDATE_DELETE_LIMIT) && !(SQLITE_OMIT_SUBQUERY)
																																																																		/* The special operator TokenType.TK_ROW means use the rowid for the first
** column in the FROM clause.  This is used by the LIMIT and ORDER BY
** clause processing on UPDATE and DELETE statements.
*/
case TokenType.TK_ROW: {
SrcList pSrcList = pNC.pSrcList;
SrcList_item pItem;
Debug.Assert( pSrcList !=null && pSrcList.nSrc==1 );
pItem = pSrcList.a[0];
pExpr.op = TokenType.TK_COLUMN;
pExpr.pTab = pItem.pTab;
pExpr.iTable = pItem.iCursor;
pExpr.iColumn = -1;
pExpr.affinity = SQLITE_AFF_INTEGER;
break;
}
#endif
                    ///A lone identifier is the name of a column.
                    case TokenType.TK_ID:
                        {
                            return ResolveExtensions.lookupName(pParse, null, null, pExpr.u.zToken, pNC, pExpr);
                        }
                    ///A table name and column name:     ID.ID
                    ///Or a database, table and column:  ID.ID.ID
                    case TokenType.TK_DOT:
                        {
                            string zColumn;
                            string zTable;
                            string zDb;
                            Expr pRight;
                            ///
                            ///<summary>
                            ///if( pSrcList==0 ) break; 
                            ///</summary>
                            pRight = pExpr.pRight;
                            if (pRight.Operator == TokenType.TK_ID)
                            {
                                zDb = null;
                                zTable = pExpr.pLeft.u.zToken;
                                zColumn = pRight.u.zToken;
                            }
                            else
                            {
                                Debug.Assert(pRight.Operator == TokenType.TK_DOT);
                                zDb = pExpr.pLeft.u.zToken;
                                zTable = pRight.pLeft.u.zToken;
                                zColumn = pRight.pRight.u.zToken;
                            }
                            return ResolveExtensions.lookupName(pParse, zDb, zTable, zColumn, pNC, pExpr);
                        }
                    ///
                    ///<summary>
                    ///Resolve function names
                    ///
                    ///</summary>
                    case TokenType.TK_CONST_FUNC:
                    case TokenType.TK_FUNCTION:
                        {
                            ExprList pList = pExpr.x.pList;
                            ///The argument list 
                            int n = pList != null ? pList.nExpr : 0;
                            ///Number of arguments 
                            bool no_such_func = false;
                            ///True if no such function exists 
                            bool wrong_num_args = false;
                            ///True if wrong number of arguments 
                            bool is_agg = false;
                            ///True if is an aggregate function 
                            int auth;
                            ///Authorization to use the function 
                            int nId;
                            ///Number of characters in function name 
                            string zId;
                            ///The function name. 
                            FuncDef pDef;
                            ///Information about the function 
                            SqliteEncoding enc = pParse.db.aDbStatic[0].pSchema.enc;
                            // ENC( pParse.db );   /* The database encoding */
                            sqliteinth.testcase(pExpr.Operator == TokenType.TK_CONST_FUNC);
                            Debug.Assert(!pExpr.ExprHasProperty(ExprFlags.EP_xIsSelect));
                            zId = pExpr.u.zToken;
                            nId = StringExtensions.Strlen30(zId);
                            pDef = FuncDefTraverse.sqlite3FindFunction(pParse.db, zId, nId, n, enc, 0);
                            if (pDef == null)
                            {
                                pDef = FuncDefTraverse.sqlite3FindFunction(pParse.db, zId, nId, -1, enc, 0);
                                if (pDef == null)
                                {
                                    no_such_func = true;
                                }
                                else
                                {
                                    wrong_num_args = true;
                                }
                            }
                            else
                            {
                                is_agg = pDef.xFunc == null;
                            }
#if !SQLITE_OMIT_AUTHORIZATION
																																																																																								if( pDef ){
auth = sqlite3AuthCheck(pParse, SQLITE_FUNCTION, 0, pDef.zName, 0);
if( auth!=SqlResult.SQLITE_OK ){
if( auth==SQLITE_DENY ){
utilc.sqlite3ErrorMsg(pParse, "not authorized to use function: %s",
pDef.zName);
pNC.nErr++;
}
pExpr.op = TokenType.TK_NULL;
return WRC.WRC_Prune;
}
}
#endif
                            if (is_agg && 0 == pNC.allowAgg)
                            {
                                utilc.sqlite3ErrorMsg(pParse, "misuse of aggregate function %.*s()", nId, zId);
                                pNC.nErr++;
                                is_agg = false;
                            }
                            else
                                if (no_such_func)
                                {
                                    utilc.sqlite3ErrorMsg(pParse, "no such function: %.*s", nId, zId);
                                    pNC.nErr++;
                                }
                                else
                                    if (wrong_num_args)
                                    {
                                        utilc.sqlite3ErrorMsg(pParse, "wrong number of arguments to function %.*s()", nId, zId);
                                        pNC.nErr++;
                                    }
                            if (is_agg)
                            {
                                pExpr.Operator = TokenType.TK_AGG_FUNCTION;
                                pNC.hasAgg = 1;
                            }
                            if (is_agg)
                                pNC.allowAgg = 0;
                            pWalker.sqlite3WalkExprList(pList);
                            if (is_agg)
                                pNC.allowAgg = 1;
                            ///FIX ME:  Compute pExpr.affinity based on the expected return
                            ///type of the function
                            return WRC.WRC_Prune;
                        }
#if !SQLITE_OMIT_SUBQUERY
                    case TokenType.TK_SELECT:
                    case TokenType.TK_EXISTS:
                        {
                            sqliteinth.testcase(pExpr.Operator == TokenType.TK_EXISTS);
                            goto case TokenType.TK_IN;
                        }
#endif
                    case TokenType.TK_IN:
                        {
                            sqliteinth.testcase(pExpr.Operator == TokenType.TK_IN);
                            if (pExpr.ExprHasProperty(ExprFlags.EP_xIsSelect))
                            {
                                int nRef = pNC.nRef;
#if !SQLITE_OMIT_CHECK
                                if (pNC.isCheck != 0)
                                {
                                    utilc.sqlite3ErrorMsg(pParse, "subqueries prohibited in CHECK constraints");
                                }
#endif
                                pWalker.sqlite3WalkSelect(pExpr.x.pSelect);
                                Debug.Assert(pNC.nRef >= nRef);
                                if (nRef != pNC.nRef)
                                {
                                    pExpr.ExprSetProperty(ExprFlags.EP_VarSelect);
                                }
                            }
                            break;
                        }
#if !SQLITE_OMIT_CHECK
                    case TokenType.TK_VARIABLE:
                        {
                            if (pNC.isCheck != 0)
                            {
                                utilc.sqlite3ErrorMsg(pParse, "parameters prohibited in CHECK constraints");
                            }
                            break;
                        }
#endif
                }
                return (pParse.nErr != 0///
                    ///<summary>
                    ///|| pParse.db.mallocFailed != 0 
                    ///</summary>
                ) ? WRC.WRC_Abort : WRC.WRC_Continue;
            }





            ///<summary>
            /// Generate an ORDER BY or GROUP BY term out-of-range error.
            ///
            ///</summary>
            public static void resolveOutOfRangeError(Parse pParse,///
                ///The error context into which to write the error 
            string zType,///
                ///"ORDER" or "GROUP" 
            int i,///
                ///The index (1">based) of the term out of range </param>
            int mx///
                ///Largest permissible value of i 
            )
            {
                utilc.sqlite3ErrorMsg(pParse, "%r %s BY term out of range - should be " + "between 1 and %d", i, zType, mx);
            }




            ///<summary>
            /// Analyze the ORDER BY clause in a compound SELECT statement.   Modify
            /// each term of the ORDER BY clause is a constant integer between 1
            /// and N where N is the number of columns in the compound SELECT.
            ///
            /// ORDER BY terms that are already an integer between 1 and N are
            /// unmodified.  ORDER BY terms that are integers outside the range of
            /// 1 through N generate an error.  ORDER BY terms that are expressions
            /// are matched against result set expressions of compound SELECT
            /// beginning with the left-most SELECT and working toward the right.
            /// At the first match, the ORDER BY expression is transformed into
            /// the integer column number.
            ///
            /// Return the number of errors seen.
            ///
            ///</summary>
            public static int resolveCompoundOrderBy(Parse pParse,///
                ///<summary>
                ///Parsing context.  Leave error messages here 
                ///</summary>
            Select pSelect///
                ///<summary>
                ///The SELECT statement containing the ORDER BY 
                ///</summary>
            )
            {
                int i;
                ExprList pOrderBy;
                ExprList pEList;
                Connection db;
                int moreToDo = 1;
                pOrderBy = pSelect.pOrderBy;
                if (pOrderBy == null)
                    return 0;
                db = pParse.db;
                //#if SQLITE_MAX_COLUMN
                if (pOrderBy.nExpr > db.aLimit[Globals.SQLITE_LIMIT_COLUMN])
                {
                    utilc.sqlite3ErrorMsg(pParse, "too many terms in ORDER BY clause");
                    return 1;
                }
                //#endif
                for (i = 0; i < pOrderBy.nExpr; i++)
                {
                    pOrderBy.a[i].done = 0;
                }
                pSelect.pNext = null;
                while (pSelect.pPrior != null)
                {
                    pSelect.pPrior.pNext = pSelect;
                    pSelect = pSelect.pPrior;
                }
                while (pSelect != null && moreToDo != 0)
                {
                    ExprList_item pItem;
                    moreToDo = 0;
                    pEList = pSelect.pEList;
                    Debug.Assert(pEList != null);
                    for (i = 0; i < pOrderBy.nExpr; i++)//, pItem++)
                    {
                        pItem = pOrderBy.a[i];
                        int iCol = -1;
                        Expr pE, pDup;
                        if (pItem.done != 0)
                            continue;
                        pE = pItem.pExpr;
                        if (pE.sqlite3ExprIsInteger(ref iCol) )
                        {
                            if (iCol <= 0 || iCol > pEList.nExpr)
                            {
                                resolveOutOfRangeError(pParse, "ORDER", i + 1, pEList.nExpr);
                                return 1;
                            }
                        }
                        else
                        {
                            iCol = ResolveExtensions.resolveAsName(pParse, pEList, pE);
                            if (iCol == 0)
                            {
                                pDup = exprc.sqlite3ExprDup(db, pE, 0);
                                ////if ( 0 == db.mallocFailed )
                                {
                                    Debug.Assert(pDup != null);
                                    iCol = ResolveExtensions.resolveOrderByTermToExprList(pParse, pSelect, pDup);
                                }
                                exprc.sqlite3ExprDelete(db, ref pDup);
                            }
                        }
                        if (iCol > 0)
                        {
                            CollSeq pColl = pE.CollatingSequence;
                            ExprFlags flags = pE.Flags & ExprFlags.EP_ExpCollate;
                            exprc.sqlite3ExprDelete(db, ref pE);
                            pItem.pExpr = pE = exprc.sqlite3Expr(db, TokenType.TK_INTEGER, null);
                            if (pE == null)
                                return 1;
                            pE.CollatingSequence = pColl;
                            pE.Flags = (pE.Flags | ExprFlags.EP_IntValue | flags);
                            pE.u.iValue = iCol;
                            pItem.iCol = (u16)iCol;
                            pItem.done = 1;
                        }
                        else
                        {
                            moreToDo = 1;
                        }
                    }
                    pSelect = pSelect.pNext;
                }
                for (i = 0; i < pOrderBy.nExpr; i++)
                {
                    if (pOrderBy.a[i].done == 0)
                    {
                        utilc.sqlite3ErrorMsg(pParse, "%r ORDER BY term does not match any " + "column in the result set", i + 1);
                        return 1;
                    }
                }
                return 0;
            }



            ///<summary>
            /// Check every term in the ORDER BY or GROUP BY clause pOrderBy of
            /// the SELECT statement pSelect.  If any term is reference to a
            /// result set expression (as determined by the ExprList.a.iCol field)
            /// then convert that term into a copy of the corresponding result set
            /// column.
            ///
            /// If any errors are detected, add an error message to pParse and
            /// return non-zero.  Return zero if no errors are seen.
            ///
            ///</summary>
            public static int sqlite3ResolveOrderGroupBy(Parse pParse,///
                ///Parsing context.  Leave error messages here 
            Select pSelect,///
                ///The SELECT statement containing the clause 
            ExprList pOrderBy,///
                ///The ORDER BY or GROUP BY clause to be processed 
            string zType///
                ///"ORDER" or "GROUP" 
            )
            {
                int i;
                Connection db = pParse.db;
                ExprList pEList;
                ExprList_item pItem;
                if (pOrderBy == null///
                    ///<summary>
                    ///|| pParse.db.mallocFailed != 0 
                    ///</summary>
                )
                    return 0;
                //#if SQLITE_MAX_COLUMN
                if (pOrderBy.nExpr > db.aLimit[Globals.SQLITE_LIMIT_COLUMN])
                {
                    utilc.sqlite3ErrorMsg(pParse, "too many terms in %s BY clause", zType);
                    return 1;
                }
                //#endif
                pEList = pSelect.pEList;
                Debug.Assert(pEList != null);
                ///
                ///<summary>
                ///sqlite3SelectNew() guarantees this 
                ///</summary>
                for (i = 0; i < pOrderBy.nExpr; i++)//, pItem++)
                {
                    pItem = pOrderBy.a[i];
                    if (pItem.iCol != 0)
                    {
                        if (pItem.iCol > pEList.nExpr)
                        {
                            resolveOutOfRangeError(pParse, zType, i + 1, pEList.nExpr);
                            return 1;
                        }
                        ResolveExtensions.resolveAlias(pParse, pEList, pItem.iCol - 1, pItem.pExpr, zType);
                    }
                }
                return 0;
            }





            ///<summary>
            /// pOrderBy is an ORDER BY or GROUP BY clause in SELECT statement pSelect.
            /// The Name context of the SELECT statement is pNC.  zType is either
            /// "ORDER" or "GROUP" depending on which type of clause pOrderBy is.
            ///
            /// This routine resolves each term of the clause into an expression.
            /// If the order-by term is an integer I between 1 and N (where N is the
            /// number of columns in the result set of the SELECT) then the expression
            /// in the resolution is a copy of the I-th result-set expression.  If
            /// the order-by term is an identify that corresponds to the AS-name of
            /// a result-set expression, then the term resolves to a copy of the
            /// result-set expression.  Otherwise, the expression is resolved in
            /// the usual way - using sqlite3ResolveExprNames().
            ///
            /// This routine returns the number of errors.  If errors occur, then
            /// an appropriate error message might be left in pParse.  (OOM errors
            /// excepted.)
            ///
            ///</summary>
            static int resolveOrderGroupBy(NameContext pNC,///
                ///The name context of the SELECT statement 
            Select pSelect,///
                ///The SELECT statement holding pOrderBy 
            ExprList pOrderBy,///
                ///An ORDER BY or GROUP BY clause to resolve 
            string zType///
                ///Either "ORDER" or "GROUP", as appropriate 
            )
            {
                int i;
                ///
                ///<summary>
                ///Loop counter 
                ///</summary>
                int iCol;
                ///
                ///<summary>
                ///Column number 
                ///</summary>
                ExprList_item pItem;
                ///
                ///<summary>
                ///A term of the ORDER BY clause 
                ///</summary>
                Parse pParse;
                ///
                ///<summary>
                ///Parsing context 
                ///</summary>
                int nResult;
                ///
                ///<summary>
                ///Number of terms in the result set 
                ///</summary>
                if (pOrderBy == null)
                    return 0;
                nResult = pSelect.pEList.nExpr;
                pParse = pNC.pParse;
                for (i = 0; i < pOrderBy.nExpr; i++)//, pItem++ )
                {
                    pItem = pOrderBy.a[i];
                    Expr pE = pItem.pExpr;
                    iCol = ResolveExtensions.resolveAsName(pParse, pSelect.pEList, pE);
                    if (iCol > 0)
                    {
                        ///
                        ///<summary>
                        ///</summary>
                        ///<param name="If an AS">name match is found, mark this ORDER BY column as being</param>
                        ///<param name="a copy of the iCol">set column.  The subsequent call to</param>
                        ///<param name="sqlite3ResolveOrderGroupBy() will convert the expression to a">sqlite3ResolveOrderGroupBy() will convert the expression to a</param>
                        ///<param name="copy of the iCol">set expression. </param>
                        pItem.iCol = (u16)iCol;
                        continue;
                    }
                    if (pE.sqlite3ExprIsInteger(ref iCol))
                    {
                        ///
                        ///<summary>
                        ///The ORDER BY term is an integer constant.  Again, set the column
                        ///number so that sqlite3ResolveOrderGroupBy() will convert the
                        ///</summary>
                        ///<param name="order">set expression </param>
                        if (iCol < 1)
                        {
                            resolveOutOfRangeError(pParse, zType, i + 1, nResult);
                            return 1;
                        }
                        pItem.iCol = (u16)iCol;
                        continue;
                    }
                    ///
                    ///<summary>
                    ///Otherwise, treat the ORDER BY term as an ordinary expression 
                    ///</summary>
                    pItem.iCol = 0;
                    if (sqlite3ResolveExprNames(pNC, ref pE) != 0)
                    {
                        return 1;
                    }
                }
                return sqlite3ResolveOrderGroupBy(pParse, pSelect, pOrderBy, zType);
            }



            ///<summary>
            /// Resolve names in the SELECT statement p and all of its descendents.
            ///
            ///</summary>
            public static WRC resolveSelectStep(Walker pWalker, Select p)
            {
                NameContext pOuterNC;
                ///Context that contains this SELECT 
                NameContext sNC;
                ///Name context of this SELECT 
                bool isCompound;
                ///True if p is a compound select 
                int nCompound;
                ///Number of compound terms processed so far 
                Parse pParse;
                ///Parsing context 
                ExprList pEList;
                ///Result set expression list 
                int i;
                ///Loop counter 
                ExprList pGroupBy;
                ///The GROUP BY clause 
                Select pLeftmost;
                ///Left most of SELECT of a compound 
                Connection db;
                ///Database connection 
                Debug.Assert(p != null);
                if ((p.selFlags & SelectFlags.Resolved) != 0)
                {
                    return WRC.WRC_Prune;
                }
                pOuterNC = pWalker.u.pNC;
                pParse = pWalker.pParse;
                db = pParse.db;
                ///
                ///<summary>
                ///Normally sqlite3SelectExpand() will be called first and will have
                ///already expanded this SELECT.  However, if this is a subquery within
                ///an expression, sqlite3ResolveExprNames() will be called without a
                ///prior call to sqlite3SelectExpand().  When that happens, let
                ///sqlite3SelectPrep() do all of the processing for this SELECT.
                ///sqlite3SelectPrep() will invoke both sqlite3SelectExpand() and
                ///this routine in the correct order.
                ///
                ///</summary>
                if ((p.selFlags & SelectFlags.Expanded) == 0)
                {
                    Select.sqlite3SelectPrep(pParse, p, pOuterNC);
                    return (pParse.nErr != 0///
                        ///<summary>
                        ///|| db.mallocFailed != 0 
                        ///</summary>
                    ) ? WRC.WRC_Abort : WRC.WRC_Prune;
                }
                isCompound = p.pPrior != null;
                nCompound = 0;
                pLeftmost = p;
                while (p != null)
                {
                    Debug.Assert((p.selFlags & SelectFlags.Expanded) != 0);
                    Debug.Assert((p.selFlags & SelectFlags.Resolved) == 0);
                    p.selFlags |= SelectFlags.Resolved;
                    ///
                    ///<summary>
                    ///Resolve the expressions in the LIMIT and OFFSET clauses. These
                    ///are not allowed to refer to any names, so pass an empty NameContext.
                    ///
                    ///</summary>
                    sNC = new NameContext();
                    // memset( &sNC, 0, sizeof( sNC ) );
                    sNC.pParse = pParse;
                    if (sqlite3ResolveExprNames(sNC, ref p.pLimit) != 0 || sqlite3ResolveExprNames(sNC, ref p.pOffset) != 0)
                    {
                        return WRC.WRC_Abort;
                    }
                    ///
                    ///<summary>
                    ///</summary>
                    ///<param name="Set up the local name">context to pass to sqlite3ResolveExprNames() to</param>
                    ///<param name="resolve the result">set expression list.</param>
                    ///<param name=""></param>
                    sNC.allowAgg = 1;
                    sNC.pSrcList = p.pSrc;
                    sNC.pNext = pOuterNC;
                    ///
                    ///<summary>
                    ///Resolve names in the result set. 
                    ///</summary>
                    pEList = p.pEList;
                    Debug.Assert(pEList != null);
                    for (i = 0; i < pEList.nExpr; i++)
                    {
                        Expr pX = pEList.a[i].pExpr;
                        if (sqlite3ResolveExprNames(sNC, ref pX) != 0)
                        {
                            return WRC.WRC_Abort;
                        }
                    }
                    ///
                    ///<summary>
                    ///Recursively resolve names in all subqueries
                    ///
                    ///</summary>
                    for (i = 0; i < p.pSrc.nSrc; i++)
                    {
                        SrcList_item pItem = p.pSrc.a[i];
                        if (pItem.pSelect != null)
                        {
                            string zSavedContext = pParse.zAuthContext;
                            if (pItem.zName != null)
                                pParse.zAuthContext = pItem.zName;
                            ResolveExtensions.sqlite3ResolveSelectNames(pParse, pItem.pSelect, pOuterNC);
                            pParse.zAuthContext = zSavedContext;
                            if (pParse.nErr != 0///
                                ///<summary>
                                ///|| db.mallocFailed != 0 
                                ///</summary>
                            )
                                return WRC.WRC_Abort;
                        }
                    }
                    ///
                    ///<summary>
                    ///</summary>
                    ///<param name="If there are no aggregate functions in the result">set, and no GROUP BY</param>
                    ///<param name="expression, do not allow aggregates in any of the other expressions.">expression, do not allow aggregates in any of the other expressions.</param>
                    ///<param name=""></param>
                    Debug.Assert((p.selFlags & SelectFlags.Aggregate) == 0);
                    pGroupBy = p.pGroupBy;
                    if (pGroupBy != null || sNC.hasAgg != 0)
                    {
                        p.selFlags |= SelectFlags.Aggregate;
                    }
                    else
                    {
                        sNC.allowAgg = 0;
                    }
                    ///
                    ///<summary>
                    ///If a HAVING clause is present, then there must be a GROUP BY clause.
                    ///
                    ///</summary>
                    if (p.pHaving != null && pGroupBy == null)
                    {
                        utilc.sqlite3ErrorMsg(pParse, "a GROUP BY clause is required before HAVING");
                        return WRC.WRC_Abort;
                    }
                    ///
                    ///<summary>
                    ///</summary>
                    ///<param name="Add the expression list to the name">context before parsing the</param>
                    ///<param name="other expressions in the SELECT statement. This is so that">other expressions in the SELECT statement. This is so that</param>
                    ///<param name="expressions in the WHERE clause (etc.) can refer to expressions by">expressions in the WHERE clause (etc.) can refer to expressions by</param>
                    ///<param name="aliases in the result set.">aliases in the result set.</param>
                    ///<param name=""></param>
                    ///<param name="Minor point: If this is the case, then the expression will be">Minor point: If this is the case, then the expression will be</param>
                    ///<param name="re">evaluated for each reference to it.</param>
                    ///<param name=""></param>
                    sNC.pEList = p.pEList;
                    if (sqlite3ResolveExprNames(sNC, ref p.pWhere) != 0 || sqlite3ResolveExprNames(sNC, ref p.pHaving) != 0)
                    {
                        return WRC.WRC_Abort;
                    }
                    ///
                    ///<summary>
                    ///The ORDER BY and GROUP BY clauses may not refer to terms in
                    ///outer queries
                    ///
                    ///</summary>
                    sNC.pNext = null;
                    sNC.allowAgg = 1;
                    ///
                    ///<summary>
                    ///Process the ORDER BY clause for singleton SELECT statements.
                    ///The ORDER BY clause for compounds SELECT statements is handled
                    ///</summary>
                    ///<param name="below, after all of the result">sets for all of the elements of</param>
                    ///<param name="the compound have been resolved.">the compound have been resolved.</param>
                    ///<param name=""></param>
                    if (!isCompound && resolveOrderGroupBy(sNC, p, p.pOrderBy, "ORDER") != 0)
                    {
                        return WRC.WRC_Abort;
                    }
                    //if ( db.mallocFailed != 0 )
                    //{
                    //  return WRC.WRC_Abort;
                    //}
                    ///
                    ///<summary>
                    ///Resolve the GROUP BY clause.  At the same time, make sure
                    ///the GROUP BY clause does not contain aggregate functions.
                    ///
                    ///</summary>
                    if (pGroupBy != null)
                    {
                        ExprList_item pItem;
                        if (resolveOrderGroupBy(sNC, p, pGroupBy, "GROUP") != 0///
                            ///<summary>
                            ///|| db.mallocFailed != 0 
                            ///</summary>
                        )
                        {
                            return WRC.WRC_Abort;
                        }
                        for (i = 0; i < pGroupBy.nExpr; i++)//, pItem++)
                        {
                            pItem = pGroupBy.a[i];
                            if ((pItem.pExpr.Flags & ExprFlags.EP_Agg) != 0)//HasProperty(pItem.pExpr, ExprFlags.EP_Agg) )
                            {
                                utilc.sqlite3ErrorMsg(pParse, "aggregate functions are not allowed in " + "the GROUP BY clause");
                                return WRC.WRC_Abort;
                            }
                        }
                    }
                    ///
                    ///<summary>
                    ///Advance to the next term of the compound
                    ///
                    ///</summary>
                    p = p.pPrior;
                    nCompound++;
                }
                ///
                ///<summary>
                ///Resolve the ORDER BY on a compound SELECT after all terms of
                ///the compound have been resolved.
                ///
                ///</summary>
                if (isCompound && resolveCompoundOrderBy(pParse, pLeftmost) != 0)
                {
                    return WRC.WRC_Abort;
                }
                return WRC.WRC_Prune;
            }





            ///<summary>
            /// This routine walks an expression tree and resolves references to
            /// table columns and result-set columns.  At the same time, do error
            /// checking on function usage and set a flag if any aggregate functions
            /// are seen.
            ///
            /// To resolve table columns references we look for nodes (or subtrees) of the
            /// form X.Y.Z or Y.Z or just Z where
            ///
            ///      X:   The name of a database.  Ex:  "main" or "temp" or
            ///           the symbolic name assigned to an ATTACH-ed database.
            ///
            ///      Y:   The name of a table in a FROM clause.  Or in a trigger
            ///           one of the special names "old" or "new".
            ///
            ///      Z:   The name of a column in table Y.
            ///
            /// The node at the root of the subtree is modified as follows:
            ///
            ///    Expr.op        Changed to TokenType.TK_COLUMN
            ///    Expr.pTab      Points to the Table object for X.Y
            ///    Expr.iColumn   The column index in X.Y.  -1 for the rowid.
            ///    Expr.iTable    The VDBE cursor number for X.Y
            ///
            ///
            /// To resolve result-set references, look for expression nodes of the
            /// form Z (with no X and Y prefix) where the Z matches the right-hand
            /// size of an AS clause in the result-set of a SELECT.  The Z expression
            /// is replaced by a copy of the left-hand side of the result-set expression.
            /// Table-name and function resolution occurs on the substituted expression
            /// tree.  For example, in:
            ///
            ///      SELECT a+b AS x, c+d AS y FROM t1 ORDER BY x;
            ///
            /// The "x" term of the order by is replaced by "a+b" to render:
            ///
            ///      SELECT a+b AS x, c+d AS y FROM t1 ORDER BY a+b;
            ///
            /// Function calls are checked to make sure that the function is
            /// defined and that the correct number of arguments are specified.
            /// If the function is an aggregate function, then the pNC.hasAgg is
            /// set and the opcode is changed from TokenType.TK_FUNCTION to TokenType.TK_AGG_FUNCTION.
            /// If an expression contains aggregate functions then the ExprFlags.EP_Agg
            /// property on the expression is set.
            ///
            /// An error message is left in pParse if anything is amiss.  The number
            /// if errors is returned.
            ///
            ///</summary>
            public static SqlResult sqlite3ResolveExprNames(NameContext pNC,///
                ///<summary>
                ///Namespace to resolve expressions in. 
                ///</summary>
            ref Expr pExpr///
                ///<summary>
                ///The expression to be analyzed. 
                ///</summary>
            )
            {
                u8 savedHasAgg;
                Walker w = new Walker();
                if (pExpr == null)
                    return 0;
#if SQLITE_MAX_EXPR_DEPTH
																																																																		{
Parse pParse = pNC.pParse;
if( exprc.sqlite3ExprCheckHeight(pParse, pExpr.nHeight+pNC.pParse.nHeight) ){
return 1;
}
pParse.nHeight += pExpr.nHeight;
}
#endif
                savedHasAgg = pNC.hasAgg;
                pNC.hasAgg = 0;
                w.xExprCallback = ResolveExtensions.resolveExprStep;
                w.xSelectCallback = resolveSelectStep;
                w.pParse = pNC.pParse;
                w.u.pNC = pNC;
                w.sqlite3WalkExpr(ref pExpr);
#if SQLITE_MAX_EXPR_DEPTH
																																																																		pNC.pParse.nHeight -= pExpr.nHeight;
#endif
                if (pNC.nErr > 0 || w.pParse.nErr > 0)
                {
                    pExpr.ExprSetProperty(ExprFlags.EP_Error);
                }
                if (pNC.hasAgg != 0)
                {
                    pExpr.ExprSetProperty(ExprFlags.EP_Agg);
                }
                else
                    if (savedHasAgg != 0)
                    {
                        pNC.hasAgg = 1;
                    }
                return (SqlResult) (pExpr.ExprHasProperty(ExprFlags.EP_Error) ? 1 : 0);
            }


            ///
            ///<summary>
            ///Resolve all names in all expressions of a SELECT and in all
            ///decendents of the SELECT, including compounds off of p.pPrior,
            ///subqueries in expressions, and subqueries used as FROM clause
            ///terms.
            ///
            ///See sqlite3ResolveExprNames() for a description of the kinds of
            ///transformations that occur.
            ///
            ///All SELECT statements should have been expanded using
            ///sqlite3SelectExpand() prior to invoking this routine.
            ///
            ///</summary>
            public static void sqlite3ResolveSelectNames(/*The parser context */Parse pParse,
                /*The SELECT statement being coded.*/
            Select p,
                /*Name context for parent SELECT statement */
            NameContext pOuterNC
                
            )
            {
                Walker w = new Walker();
                Debug.Assert(p != null);
                w.xExprCallback = ResolveExtensions.resolveExprStep;
                w.xSelectCallback = resolveSelectStep;
                w.pParse = pParse;
                w.u.pNC = pOuterNC;
                w.sqlite3WalkSelect(p);
            }

        }
    }
}