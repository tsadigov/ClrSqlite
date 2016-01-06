using Community.CsharpSqlite.Ast;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Community.CsharpSqlite.Ast
{
	public static class ExprExtensions
	{
		///<summary>
		///The following three functions, heightOfExpr(), heightOfExprList()
		/// and heightOfSelect(), are used to determine the maximum height
		/// of any expression tree referenced by the structure passed as the
		/// first argument.
		///
		/// If this maximum height is greater than the current value pointed
		/// to by pnHeight, the second parameter, then set pnHeight to that
		/// value.
		///
		///</summary>
		public static void heightOfExpr (this Expr _this, ref int pnHeight)
		{
			if (null != _this && _this.nHeight > pnHeight) {
				pnHeight = _this.nHeight;
			}
		}

		public static void heightOfExprList (this ExprList _this, ref int pnHeight)
		{
			if (_this != null) 
				for (var i = 0; i < _this.Count; i++) 
					_this[i].pExpr.heightOfExpr (ref pnHeight);
		}
	}
}
