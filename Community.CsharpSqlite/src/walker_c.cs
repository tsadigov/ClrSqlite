using Community.CsharpSqlite.Ast;
using System;
using System.Diagnostics;
using System.Text;
using Bitmask = System.UInt64;
using u32 = System.UInt32;
namespace Community.CsharpSqlite {
	public partial class Sqlite3 {
		///<summary>
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
		///    WRC.WRC_Continue      Continue descending down the tree.
		///
		///    WRC.WRC_Prune         Do not descend into child nodes.  But allow
		///                      the walk to continue with sibling nodes.
		///
		///    WRC.WRC_Abort         Do no more callbacks.  Unwind the stack and
		///                      return the top-level walk call.
		///
		/// The return value from this routine is WRC.WRC_Abort to abandon the tree walk
		/// and WRC.WRC_Continue to continue.
		///
		///</summary>
		static WRC sqlite3WalkExpr(Walker pWalker,ref Expr pExpr) {
            WRC rc;
			if(pExpr==null)
                return WRC.WRC_Continue;
			sqliteinth.testcase(pExpr.ExprHasProperty(ExprFlags.EP_TokenOnly));
			sqliteinth.testcase(pExpr.ExprHasProperty(ExprFlags.EP_Reduced));
			rc=(WRC)pWalker.xExprCallback(pWalker,ref pExpr);
            if (rc == WRC.WRC_Continue && !pExpr.ExprHasAnyProperty(ExprFlags.EP_TokenOnly))
            {
				if(sqlite3WalkExpr(pWalker,ref pExpr.pLeft)!=0)
                    return WRC.WRC_Abort;
				if(sqlite3WalkExpr(pWalker,ref pExpr.pRight)!=0)
                    return WRC.WRC_Abort;
				if(pExpr.ExprHasProperty(ExprFlags.EP_xIsSelect)) {
					if(sqlite3WalkSelect(pWalker,pExpr.x.pSelect)!=0)
                        return WRC.WRC_Abort;
				}
				else {
					if(sqlite3WalkExprList(pWalker,pExpr.x.pList)!=0)
                        return WRC.WRC_Abort;
				}
			}
            return rc & WRC.WRC_Abort;
		}
		///<summary>
		/// Call sqlite3WalkExpr() for every expression in list p or until
		/// an abort request is seen.
		///
		///</summary>
		static WRC sqlite3WalkExprList(Walker pWalker,ExprList p) {
			int i;
			ExprList_item pItem;
			if(p!=null) {
				for(i=p.nExpr;i>0;i--) {
					//, pItem++){
					pItem=p.a[p.nExpr-i];
					if(sqlite3WalkExpr(pWalker,ref pItem.pExpr)!=0)
						return WRC.WRC_Abort;
				}
			}
            return WRC.WRC_Continue;
		}
		///<summary>
		/// Walk all expressions associated with SELECT statement p.  Do
		/// not invoke the SELECT callback on p, but do (of course) invoke
		/// any expr callbacks and SELECT callbacks that come from subqueries.
		/// Return WRC.WRC_Abort or WRC.WRC_Continue.
		///
		///</summary>
		static WRC sqlite3WalkSelectExpr(Walker pWalker,Select p) {
			if(sqlite3WalkExprList(pWalker,p.pEList)!=0)
                return WRC.WRC_Abort;
			if(sqlite3WalkExpr(pWalker,ref p.pWhere)!=0)
                return WRC.WRC_Abort;
			if(sqlite3WalkExprList(pWalker,p.pGroupBy)!=0)
                return WRC.WRC_Abort;
			if(sqlite3WalkExpr(pWalker,ref p.pHaving)!=0)
                return WRC.WRC_Abort;
			if(sqlite3WalkExprList(pWalker,p.pOrderBy)!=0)
                return WRC.WRC_Abort;
			if(sqlite3WalkExpr(pWalker,ref p.pLimit)!=0)
                return WRC.WRC_Abort;
			if(sqlite3WalkExpr(pWalker,ref p.pOffset)!=0)
                return WRC.WRC_Abort;
            return WRC.WRC_Continue;
		}
		///<summary>
		/// Walk the parse trees associated with all subqueries in the
		/// FROM clause of SELECT statement p.  Do not invoke the select
		/// callback on p, but do invoke it on each FROM clause subquery
		/// and on any subqueries further down in the tree.  Return
		/// WRC.WRC_Abort or WRC.WRC_Continue;
		///
		///</summary>
		static WRC sqlite3WalkSelectFrom(Walker pWalker,Select p) {
			SrcList pSrc;
			int i;
			SrcList_item pItem;
			pSrc=p.pSrc;
			if(Sqlite3.ALWAYS(pSrc)) {
				for(i=pSrc.nSrc;i>0;i--)// pItem++ )
				 {
					pItem=pSrc.a[pSrc.nSrc-i];
					if(sqlite3WalkSelect(pWalker,pItem.pSelect)!=0) {
                        return WRC.WRC_Abort;
					}
				}
			}
			return WRC.WRC_Continue;
		}
		///
		///<summary>
		///Call sqlite3WalkExpr() for every expression in Select statement p.
		///Invoke sqlite3WalkSelect() for subqueries in the FROM clause and
		///on the compound select chain, p.pPrior.
		///
		///Return WRC.WRC_Continue under normal conditions.  Return WRC.WRC_Abort if
		///there is an abort request.
		///
		///If the Walker does not have an xSelectCallback() then this routine
		///</summary>
		///<param name="is a no">op returning WRC.WRC_Continue.</param>
		///<param name=""></param>
		static WRC sqlite3WalkSelect(Walker pWalker,Select p) {
			WRC rc=new WRC();
			if(p==null||pWalker.xSelectCallback==null)
                return WRC.WRC_Continue;
            rc = WRC.WRC_Continue;
			while(p!=null) {
				rc=pWalker.xSelectCallback(pWalker,p);
				if(rc!=0)
					break;
				if(sqlite3WalkSelectExpr(pWalker,p)!=0)
                    return WRC.WRC_Abort;
				if(sqlite3WalkSelectFrom(pWalker,p)!=0)
                    return WRC.WRC_Abort;
				p=p.pPrior;
			}
            return rc & WRC.WRC_Abort;
		}
	}
}
