using System;
using System.Diagnostics;
using u8=System.Byte;
using u32=System.UInt32;
namespace Community.CsharpSqlite {
	using sqlite3_value=Sqlite3.Mem;
	public partial class Sqlite3 {
	/*
    ** 2001 September 15
    **
    ** The author disclaims copyright to this source code.  In place of
    ** a legal notice, here is a blessing:
    **
    **    May you do good and not evil.
    **    May you find forgiveness for yourself and forgive others.
    **    May you share freely, never taking more than you give.
    **
    *************************************************************************
    ** This file contains C code routines that are called by the parser
    ** to handle UPDATE statements.
    *************************************************************************
    **  Included in SQLite3 port to C#-SQLite;  2008 Noah B Hart
    **  C#-SQLite is an independent reimplementation of the SQLite software library
    **
    **  SQLITE_SOURCE_ID: 2011-06-23 19:49:22 4374b7e83ea0a3fbc3691f9c0c936272862f32f2
    **
    *************************************************************************
    *///#include "sqliteInt.h"
	#if !SQLITE_OMIT_VIRTUALTABLE
	///<summary>
	///Forward declaration
	///</summary>
	//static void updateVirtualTable(
	//Parse pParse,       /* The parsing context */
	//SrcList pSrc,       /* The virtual table to be modified */
	//Table pTab,         /* The virtual table */
	//ExprList pChanges,  /* The columns to change in the UPDATE statement */
	//Expr pRowidExpr,    /* Expression used to recompute the rowid */
	//int aXRef,          /* Mapping from columns of pTab to entries in pChanges */
	//Expr *pWhere,        /* WHERE clause of the UPDATE statement */
	//int onError          /* ON CONFLICT strategy */
	//);
	#endif
	///<summary>
	/// The most recently coded instruction was an OP_Column to retrieve the
	/// i-th column of table pTab. This routine sets the P4 parameter of the
	/// OP_Column to the default value, if any.
	///
	/// The default value of a column is specified by a DEFAULT clause in the
	/// column definition. This was either supplied by the user when the table
	/// was created, or added later to the table definition by an ALTER TABLE
	/// command. If the latter, then the row-records in the table btree on disk
	/// may not contain a value for the column and the default value, taken
	/// from the P4 parameter of the OP_Column instruction, is returned instead.
	/// If the former, then all row-records are guaranteed to include a value
	/// for the column and the P4 value is not required.
	///
	/// Column definitions created by an ALTER TABLE command may only have
	/// literal default values specified: a number, null or a string. (If a more
	/// complicated default expression value was provided, it is evaluated
	/// when the ALTER TABLE is executed and one of the literal values written
	/// into the sqlite_master table.)
	///
	/// Therefore, the P4 parameter is only required if the default value for
	/// the column is a literal number, string or null. The sqlite3ValueFromExpr()
	/// function is capable of transforming these types of expressions into
	/// sqlite3_value objects.
	///
	/// If parameter iReg is not negative, code an OP_RealAffinity instruction
	/// on register iReg. This is used when an equivalent integer value is
	/// stored in place of an 8-byte floating point value in order to save
	/// space.
	///</summary>
	/*
    ** Process an UPDATE statement.
    **
    **   UPDATE OR IGNORE table_wxyz SET a=b, c=d WHERE e<5 AND f NOT NULL;
    **          \_______/ \________/     \______/       \________________/
    *            onError   pTabList      pChanges             pWhere
    *////<summary>
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
	#if !SQLITE_OMIT_VIRTUALTABLE
	/*
** Generate code for an UPDATE of a virtual table.
**
** The strategy is that we create an ephemerial table that contains
** for each row to be changed:
**
**   (A)  The original rowid of that row.
**   (B)  The revised rowid for the row. (note1)
**   (C)  The content of every column in the row.
**
** Then we loop over this ephemeral table and for each row in
** the ephermeral table call VUpdate.
**
** When finished, drop the ephemeral table.
**
** (note1) Actually, if we know in advance that (A) is always the same
** as (B) we only store (A), then duplicate (A) when pulling
** it out of the ephemeral table before calling VUpdate.
*/
	#endif
	}
}
