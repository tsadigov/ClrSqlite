using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System
//Community.CsharpSqlite
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
		public static void heightOfExpr (this Community.CsharpSqlite.Sqlite3.Expr _this, ref int pnHeight)
		{
			if (null != _this && _this.nHeight > pnHeight) {
				pnHeight = _this.nHeight;
			}
		}

		public static void heightOfExprList (this Community.CsharpSqlite.Sqlite3.ExprList _this, ref int pnHeight)
		{
			if (_this != null) {
				int i;
				for (i = 0; i < _this.nExpr; i++) {
					_this.a [i].pExpr.heightOfExpr (ref pnHeight);
				}
			}
		}
	}
}
