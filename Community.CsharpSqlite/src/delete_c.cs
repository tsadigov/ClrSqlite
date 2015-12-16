using System;
using System.Diagnostics;
using System.Text;
using u8 = System.Byte;
using u32 = System.UInt32;

namespace Community.CsharpSqlite
{
	public partial class Sqlite3
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
	/// in order to generate code for DELETE FROM statements.
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
	/// While a SrcList can in general represent multiple tables and subqueries
	/// (as in the FROM clause of a SELECT statement) in this case it contains
	/// the name of a single table, as one might find in an INSERT, DELETE,
	/// or UPDATE statement.  Look up that table in the symbol table and
	/// return a pointer.  Set an error message and return NULL if the table
	/// name is not found or if any other error occurs.
	///
	/// The following fields are initialized appropriate in pSrc:
	///
	///    pSrc->a[0].pTab       Pointer to the Table object
	///    pSrc->a[0].pIndex     Pointer to the INDEXED BY index, if there is one
	///
	///
	///</summary>
	///<summary>
	/// Check to make sure the given table is writable.  If it is not
	/// writable, generate an error message and return 1.  If it is
	/// writable return 0;
	///
	///</summary>
	#if !SQLITE_OMIT_VIEW && !SQLITE_OMIT_TRIGGER
	///<summary>
	/// Evaluate a view and store its result in an ephemeral table.  The
	/// pWhere argument is an optional WHERE clause that restricts the
	/// set of rows in the view that are to be added to the ephemeral table.
	///</summary>
	#endif
	#if (SQLITE_ENABLE_UPDATE_DELETE_LIMIT) && !(SQLITE_OMIT_SUBQUERY)
																																								/*
** Generate an expression tree to implement the WHERE, ORDER BY,
** and LIMIT/OFFSET portion of DELETE and UPDATE statements.
**
**     DELETE FROM table_wxyz WHERE a<5 ORDER BY a LIMIT 1;
**                            \__________________________/
**                               pLimitWhere (pInClause)
*/
Expr sqlite3LimitWhere(
Parse pParse,               /* The parser context */
SrcList pSrc,               /* the FROM clause -- which tables to scan */
Expr pWhere,                /* The WHERE clause.  May be null */
ExprList pOrderBy,          /* The ORDER BY clause.  May be null */
Expr pLimit,                /* The LIMIT clause.  May be null */
Expr pOffset,               /* The OFFSET clause.  May be null */
char zStmtType              /* Either DELETE or UPDATE.  For error messages. */
){
Expr pWhereRowid = null;    /* WHERE rowid .. */
Expr pInClause = null;      /* WHERE rowid IN ( select ) */
Expr pSelectRowid = null;   /* SELECT rowid ... */
ExprList pEList = null;     /* Expression list contaning only pSelectRowid */
SrcList pSelectSrc = null;  /* SELECT rowid FROM x ... (dup of pSrc) */
Select pSelect = null;      /* Complete SELECT tree */

/* Check that there isn't an ORDER BY without a LIMIT clause.
*/
if( pOrderBy!=null && (pLimit == null) ) {
utilc.sqlite3ErrorMsg(pParse, "ORDER BY without LIMIT on %s", zStmtType);
pParse.parseError = 1;
goto limit_where_cleanup_2;
}

/* We only need to generate a select expression if there
** is a limit/offset term to enforce.
*/
if ( pLimit == null )
{
/* if pLimit is null, pOffset will always be null as well. */
Debug.Assert( pOffset == null );
return pWhere;
}

/* Generate a select expression tree to enforce the limit/offset
** term for the DELETE or UPDATE statement.  For example:
**   DELETE FROM table_a WHERE col1=1 ORDER BY col2 LIMIT 1 OFFSET 1
** becomes:
**   DELETE FROM table_a WHERE rowid IN (
**     SELECT rowid FROM table_a WHERE col1=1 ORDER BY col2 LIMIT 1 OFFSET 1
**   );
*/

pSelectRowid = sqlite3PExpr( pParse, TokenType.TK_ROW, null, null, null );
if( pSelectRowid == null ) goto limit_where_cleanup_2;
pEList = exprc.sqlite3ExprListAppend( pParse, null, pSelectRowid);
if( pEList == null ) goto limit_where_cleanup_2;

/* duplicate the FROM clause as it is needed by both the DELETE/UPDATE tree
** and the SELECT subtree. */
pSelectSrc = exprc.sqlite3SrcListDup(pParse.db, pSrc,0);
if( pSelectSrc == null ) {
exprc.sqlite3ExprListDelete(pParse.db, pEList);
goto limit_where_cleanup_2;
}

/* generate the SELECT expression tree. */
pSelect = sqlite3SelectNew( pParse, pEList, pSelectSrc, pWhere, null, null,
pOrderBy, 0, pLimit, pOffset );
if( pSelect == null ) return null;

/* now generate the new WHERE rowid IN clause for the DELETE/UDPATE */
pWhereRowid = sqlite3PExpr( pParse, TokenType.TK_ROW, null, null, null );
if( pWhereRowid == null ) goto limit_where_cleanup_1;
pInClause = sqlite3PExpr( pParse, TokenType.TK_IN, pWhereRowid, null, null );
if( pInClause == null ) goto limit_where_cleanup_1;

pInClause->x.pSelect = pSelect;
pInClause->flags |= ExprFlags.EP_xIsSelect;
exprc.sqlite3ExprSetHeight(pParse, pInClause);
return pInClause;

/* something went wrong. clean up anything allocated. */
limit_where_cleanup_1:
SelectMethods.sqlite3SelectDelete(pParse.db, pSelect);
return null;

limit_where_cleanup_2:
exprc.sqlite3ExprDelete(pParse.db, ref pWhere);
exprc.sqlite3ExprListDelete(pParse.db, pOrderBy);
exprc.sqlite3ExprDelete(pParse.db, ref pLimit);
exprc.sqlite3ExprDelete(pParse.db, ref pOffset);
return null;
}
#endif
	///
///<summary>
///Generate code for a DELETE FROM statement.
///
///DELETE FROM table_wxyz WHERE a<5 AND b NOT NULL;
///\________/       \________________/
///pTabList              pWhere
///</summary>

	///<summary>
	///Make sure "isView" and other macros defined above are undefined. Otherwise
	/// thely may interfere with compilation of other functions in this file
	/// (or in another file, if this file becomes part of the amalgamation).
	///</summary>
	//#if isView
	// #undef isView
	//#endif
	//#if pTrigger
	// #undef pTrigger
	//#endif
	///<summary>
	/// This routine generates VDBE code that causes a single row of a
	/// single table to be deleted.
	///
	/// The VDBE must be in a particular state when this routine is called.
	/// These are the requirements:
	///
	///   1.  A read/write cursor pointing to pTab, the table containing the row
	///       to be deleted, must be opened as cursor number $iCur.
	///
	///   2.  Read/write cursors for all indices of pTab must be open as
	///       cursor number base+i for the i-th index.
	///
	///   3.  The record number of the row to be deleted must be stored in
	///       memory cell iRowid.
	///
	/// This routine generates code to remove both the table record and all
	/// index entries that point to that record.
	///
	///</summary>
	///<summary>
	/// This routine generates VDBE code that causes the deletion of all
	/// index entries associated with a single row of a single table.
	///
	/// The VDBE must be in a particular state when this routine is called.
	/// These are the requirements:
	///
	///   1.  A read/write cursor pointing to pTab, the table containing the row
	///       to be deleted, must be opened as cursor number "iCur".
	///
	///   2.  Read/write cursors for all indices of pTab must be open as
	///       cursor number iCur+i for the i-th index.
	///
	///   3.  The "iCur" cursor must be pointing to the row that is to be
	///       deleted.
	///
	///</summary>
	///
///<summary>
///Generate code that will assemble an index key and put it in register
///regOut.  The key with be for index pIdx which is an index on pTab.
///iCur is the index of a cursor open on the pTab table and pointing to
///the entry that needs indexing.
///
///Return a register number which is the first in a block of
///registers that holds the elements of the index key.  The
///block of registers has already been deallocated by the time
///this routine returns.
///
///</summary>

	}
}
