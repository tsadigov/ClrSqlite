﻿using System;
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
using Community.CsharpSqlite.Compiler.Parser;
#if !SQLITE_MAX_VARIABLE_NUMBER
using ynVar = System.Int16;
using Community.CsharpSqlite.Ast;
#else
using ynVar = System.Int32; 
#endif
namespace Community.CsharpSqlite.Ast {
		///
		///<summary>
		///</summary>
		///<param name="Context pointer passed down through the tree">walk.</param>
		///<param name=""></param>
		public class Walker {
			public dxExprCallback xExprCallback;
			//)(Walker*, Expr);     /* Callback for expressions */
			public dxSelectCallback xSelectCallback;
			//)(Walker*,Select);  /* Callback for SELECTs */
			public Sqlite3.Parse pParse;
			///
			///<summary>
			///Parser context.  
			///</summary>
			public struct uw {
				///<summary>
				///Extra data for callback 
				///</summary>
				public NameContext pNC;
				///<summary>
				///Naming context 
				///</summary>
				public int i;
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
			WRC sqlite3WalkExpr(ref Expr pExpr) {
				WRC rc;
				if(pExpr==null)
                    return WRC.WRC_Continue;
				sqliteinth.testcase(pExpr.HasProperty(ExprFlags.EP_TokenOnly));
				sqliteinth.testcase(pExpr.HasProperty(ExprFlags.EP_Reduced));
				rc=this.xExprCallback(this,ref pExpr);
                if (rc == WRC.WRC_Continue && !pExpr.ExprHasAnyProperty(ExprFlags.EP_TokenOnly))
                {
					if(this.sqlite3WalkExpr(ref pExpr.pLeft)!=0)
                        return WRC.WRC_Abort;
					if(this.sqlite3WalkExpr(ref pExpr.pRight)!=0)
                        return WRC.WRC_Abort;
					if(pExpr.HasProperty(ExprFlags.EP_xIsSelect)) {
						if(this.sqlite3WalkSelect(pExpr.x.pSelect)!=0)
                            return WRC.WRC_Abort;
					}
					else {
						if(this.sqlite3WalkExprList(pExpr.x.pList)!=0)
                            return WRC.WRC_Abort;
					}
				}
                return rc & WRC.WRC_Abort;
			}

            ///<summary>
            /// Call sqlite3WalkExpr() for every expression in list p or until
            /// an abort request is seen.
            ///</summary>
			public
			WRC sqlite3WalkExprList(ExprList p) {
				int i;
				ExprList_item pItem;
				if(p!=null) {
					for(i=p.Count;i>0;i--) {
						//, pItem++){
						pItem=p.a[p.Count-i];
						if(this.sqlite3WalkExpr(ref pItem.pExpr)!=0)
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
			public	WRC sqlite3WalkSelectExpr(Select p) {
				if(this.sqlite3WalkExprList(p.ResultingFieldList)!=0)
                    return WRC.WRC_Abort;
				if(this.sqlite3WalkExpr(ref p.pWhere)!=0)
                    return WRC.WRC_Abort;
				if(this.sqlite3WalkExprList(p.pGroupBy)!=0)
                    return WRC.WRC_Abort;
				if(this.sqlite3WalkExpr(ref p.pHaving)!=0)
                    return WRC.WRC_Abort;
				if(this.sqlite3WalkExprList(p.pOrderBy)!=0)
                    return WRC.WRC_Abort;
				if(this.sqlite3WalkExpr(ref p.pLimit)!=0)
                    return WRC.WRC_Abort;
				if(this.sqlite3WalkExpr(ref p.pOffset)!=0)
                    return WRC.WRC_Abort;
                return WRC.WRC_Continue;
			}
			public///<summary>
			/// Walk the parse trees associated with all subqueries in the
			/// FROM clause of SELECT statement p.  Do not invoke the select
			/// callback on p, but do invoke it on each FROM clause subquery
			/// and on any subqueries further down in the tree.  Return
			/// WRC.WRC_Abort or WRC.WRC_Continue;
			///
			///</summary>
			WRC sqlite3WalkSelectFrom(Select p) {
				SrcList pSrc;
				int i;
				SrcList_item pItem;
				pSrc=p.pSrc;
				if(Sqlite3.ALWAYS(pSrc)) {
					for(i=pSrc.Count;i>0;i--)// pItem++ )
					 {
						pItem=pSrc.a[pSrc.Count-i];
						if(this.sqlite3WalkSelect(pItem.pSelect)!=0) {
                            return WRC.WRC_Abort;
						}
					}
				}
				return WRC.WRC_Continue;
			}
			public WRC sqlite3WalkSelect(Select p) {
                WRC rc;
				if(p==null||this.xSelectCallback==null)
                    return WRC.WRC_Continue;
                rc = WRC.WRC_Continue;
				while(p!=null) {
					rc=this.xSelectCallback(this,p);
					if(rc!=0)
						break;
					if(this.sqlite3WalkSelectExpr(p)!=0)
                        return WRC.WRC_Abort;
					if(this.sqlite3WalkSelectFrom(p)!=0)
                        return WRC.WRC_Abort;
					p=p.pPrior;
				}
                return rc & WRC.WRC_Abort;
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
		//#define WRC.WRC_Continue    0   /* Continue down into children */
		//#define WRC.WRC_Prune       1   /* Omit children but continue walking siblings */
		//#define WRC.WRC_Abort       2   /* Abandon the tree walk */
        public enum WRC
        {
            WRC_Continue = 0,
            WRC_Prune = 1,
            WRC_Abort = 2
        }
}
