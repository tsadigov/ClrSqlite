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
        ///</summary>
        ///<param name="Context pointer passed down through the tree">walk.</param>
        ///<param name=""></param>

        public class Walker
        {
            public dxExprCallback xExprCallback;

            //)(Walker*, Expr);     /* Callback for expressions */
            public dxSelectCallback xSelectCallback;

            //)(Walker*,Select);  /* Callback for SELECTs */
            public Parse pParse;

            ///
            ///<summary>
            ///Parser context.  
            ///</summary>

            public struct uw
            {
                ///
                ///<summary>
                ///Extra data for callback 
                ///</summary>

                public NameContext pNC;

                ///
                ///<summary>
                ///Naming context 
                ///</summary>

                public int i;
                ///
                ///<summary>
                ///Integer value 
                ///</summary>

            }

            public uw u;

            public///<summary>
                /// 2008 August 16
                ///
                /// The author disclaims copyright to this source code.  In place of
                /// a legal notice, here is a blessing:
                ///
                ///    May you do good and not evil.
                ///    May you find forgiveness for yourself and forgive others.
                ///    May you share freely, never taking more than you give.
                ///
                ///
                /// This file contains routines used for walking the parser tree for
                /// an SQL statement.
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
                /// Walk an expression tree.  Invoke the callback once for each node
                /// of the expression, while decending.  (In other words, the callback
                /// is invoked before visiting children.)
                ///
                /// The return value from the callback should be one of the WRC_
                /// constants to specify how to proceed with the walk.
                ///
                ///    WRC_Continue      Continue descending down the tree.
                ///
                ///    WRC_Prune         Do not descend into child nodes.  But allow
                ///                      the walk to continue with sibling nodes.
                ///
                ///    WRC_Abort         Do no more callbacks.  Unwind the stack and
                ///                      return the top-level walk call.
                ///
                /// The return value from this routine is WRC_Abort to abandon the tree walk
                /// and WRC_Continue to continue.
                ///
                ///</summary>
            int sqlite3WalkExpr(ref Expr pExpr)
            {
                int rc;
                if (pExpr == null)
                    return WRC_Continue;
                testcase(ExprHasProperty(pExpr, EP_TokenOnly));
                testcase(ExprHasProperty(pExpr, EP_Reduced));
                rc = this.xExprCallback(this, ref pExpr);
                if (rc == WRC_Continue && !ExprHasAnyProperty(pExpr, EP_TokenOnly))
                {
                    if (this.sqlite3WalkExpr(ref pExpr.pLeft) != 0)
                        return WRC_Abort;
                    if (this.sqlite3WalkExpr(ref pExpr.pRight) != 0)
                        return WRC_Abort;
                    if (ExprHasProperty(pExpr, EP_xIsSelect))
                    {
                        if (this.sqlite3WalkSelect(pExpr.x.pSelect) != 0)
                            return WRC_Abort;
                    }
                    else
                    {
                        if (this.sqlite3WalkExprList(pExpr.x.pList) != 0)
                            return WRC_Abort;
                    }
                }
                return rc & WRC_Abort;
            }

            public///<summary>
                /// Call sqlite3WalkExpr() for every expression in list p or until
                /// an abort request is seen.
                ///
                ///</summary>
            int sqlite3WalkExprList(ExprList p)
            {
                int i;
                ExprList_item pItem;
                if (p != null)
                {
                    for (i = p.nExpr; i > 0; i--)
                    {
                        //, pItem++){
                        pItem = p.a[p.nExpr - i];
                        if (this.sqlite3WalkExpr(ref pItem.pExpr) != 0)
                            return WRC_Abort;
                    }
                }
                return WRC_Continue;
            }

            public///<summary>
                /// Walk all expressions associated with SELECT statement p.  Do
                /// not invoke the SELECT callback on p, but do (of course) invoke
                /// any expr callbacks and SELECT callbacks that come from subqueries.
                /// Return WRC_Abort or WRC_Continue.
                ///
                ///</summary>
            int sqlite3WalkSelectExpr(Select p)
            {
                if (this.sqlite3WalkExprList(p.pEList) != 0)
                    return WRC_Abort;
                if (this.sqlite3WalkExpr(ref p.pWhere) != 0)
                    return WRC_Abort;
                if (this.sqlite3WalkExprList(p.pGroupBy) != 0)
                    return WRC_Abort;
                if (this.sqlite3WalkExpr(ref p.pHaving) != 0)
                    return WRC_Abort;
                if (this.sqlite3WalkExprList(p.pOrderBy) != 0)
                    return WRC_Abort;
                if (this.sqlite3WalkExpr(ref p.pLimit) != 0)
                    return WRC_Abort;
                if (this.sqlite3WalkExpr(ref p.pOffset) != 0)
                    return WRC_Abort;
                return WRC_Continue;
            }

            public///<summary>
                /// Walk the parse trees associated with all subqueries in the
                /// FROM clause of SELECT statement p.  Do not invoke the select
                /// callback on p, but do invoke it on each FROM clause subquery
                /// and on any subqueries further down in the tree.  Return
                /// WRC_Abort or WRC_Continue;
                ///
                ///</summary>
            int sqlite3WalkSelectFrom(Select p)
            {
                SrcList pSrc;
                int i;
                SrcList_item pItem;
                pSrc = p.pSrc;
                if (ALWAYS(pSrc))
                {
                    for (i = pSrc.nSrc; i > 0; i--)// pItem++ )
                    {
                        pItem = pSrc.a[pSrc.nSrc - i];
                        if (this.sqlite3WalkSelect(pItem.pSelect) != 0)
                        {
                            return WRC_Abort;
                        }
                    }
                }
                return WRC_Continue;
            }

            public int sqlite3WalkSelect(Select p)
            {
                int rc;
                if (p == null || this.xSelectCallback == null)
                    return WRC_Continue;
                rc = WRC_Continue;
                while (p != null)
                {
                    rc = this.xSelectCallback(this, p);
                    if (rc != 0)
                        break;
                    if (this.sqlite3WalkSelectExpr(p) != 0)
                        return WRC_Abort;
                    if (this.sqlite3WalkSelectFrom(p) != 0)
                        return WRC_Abort;
                    p = p.pPrior;
                }
                return rc & WRC_Abort;
            }
        }

        ///
        ///<summary>
        ///Forward declarations 
        ///</summary>

        //int sqlite3WalkExpr(Walker*, Expr);
        //int sqlite3WalkExprList(Walker*, ExprList);
        //int sqlite3WalkSelect(Walker*, Select);
        //int sqlite3WalkSelectExpr(Walker*, Select);
        //int sqlite3WalkSelectFrom(Walker*, Select);
        ///
        ///<summary>
        ///</summary>
        ///<param name="Return code from the parse">tree walking primitives and their</param>
        ///<param name="callbacks.">callbacks.</param>
        ///<param name=""></param>

        //#define WRC_Continue    0   /* Continue down into children */
        //#define WRC_Prune       1   /* Omit children but continue walking siblings */
        //#define WRC_Abort       2   /* Abandon the tree walk */
        private const int WRC_Continue = 0;

        private const int WRC_Prune = 1;

        private const int WRC_Abort = 2;






    }
}
