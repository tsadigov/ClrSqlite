using System;
using System.Diagnostics;
using System.Text;
using Pgno = System.UInt32;
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
		/// to handle INSERT statements in SQLite.
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
		/// Generate code that will open a table for reading.
		///
		///</summary>
		///<summary>
		/// Return a pointer to the column affinity string associated with index
		/// pIdx. A column affinity string has one character for each column in
		/// the table, according to the affinity of the column:
		///
		///  Character      Column affinity
		///  ------------------------------
		///  'a'            TEXT
		///  'b'            NONE
		///  'c'            NUMERIC
		///  'd'            INTEGER
		///  'e'            REAL
		///
		/// An extra 'b' is appended to the end of the string to cover the
		/// rowid that appears as the last column in every index.
		///
		/// Memory for the buffer containing the column index affinity string
		/// is managed along with the rest of the Index structure. It will be
		/// released when sqlite3DeleteIndex() is called.
		///
		///</summary>
		///<summary>
		/// Set P4 of the most recently inserted opcode to a column affinity
		/// string for table pTab. A column affinity string has one character
		/// for each column indexed by the index, according to the affinity of the
		/// column:
		///
		///  Character      Column affinity
		///  ------------------------------
		///  'a'            TEXT
		///  'b'            NONE
		///  'c'            NUMERIC
		///  'd'            INTEGER
		///  'e'            REAL
		///
		///</summary>
		///<summary>
		/// Return non-zero if the table pTab in database iDb or any of its indices
		/// have been opened at any point in the VDBE program beginning at location
		/// iStartAddr throught the end of the program.  This is used to see if
		/// a statement of the form  "INSERT INTO <iDb, pTab> SELECT ..." can
		/// run without using temporary table for the results of the SELECT.
		///
		///</summary>
		#if !SQLITE_OMIT_AUTOINCREMENT
		///<summary>
		/// Locate or create an AutoincInfo structure associated with table pTab
		/// which is in database iDb.  Return the register number for the register
		/// that holds the maximum rowid.
		///
		/// There is at most one AutoincInfo structure per table even if the
		/// same table is autoincremented multiple times due to inserts within
		/// triggers.  A new AutoincInfo structure is created if this is the
		/// first use of table pTab.  On 2nd and subsequent uses, the original
		/// AutoincInfo structure is used.
		///
		/// Three memory locations are allocated:
		///
		///   (1)  Register to hold the name of the pTab table.
		///   (2)  Register to hold the maximum ROWID of pTab.
		///   (3)  Register to hold the rowid in sqlite_sequence of pTab
		///
		/// The 2nd register is the one that is returned.  That is all the
		/// insert routine needs to know about.
		///</summary>
		///<summary>
		/// This routine generates code that will initialize all of the
		/// register used by the autoincrement tracker.
		///
		///</summary>
		///<summary>
		/// Update the maximum rowid for an autoincrement calculation.
		///
		/// This routine should be called when the top of the stack holds a
		/// new rowid that is about to be inserted.  If that new rowid is
		/// larger than the maximum rowid in the memId memory cell, then the
		/// memory cell is updated.  The stack is unchanged.
		///
		///</summary>
		///
///<summary>
///This routine generates the code needed to write autoincrement
///maximum rowid values back into the sqlite_sequence register.
///Every statement that might do an INSERT into an autoincrement
///table (either directly or through triggers) needs to call this
///routine just before the "exit" code.
///
///</summary>

		#else
																																										/*
** If SQLITE_OMIT_AUTOINCREMENT is defined, then the three routines
** above are all no-ops
*/
// define autoIncBegin(A,B,C) (0)
// define autoIncStep(A,B,C)
#endif
		///
///<summary>
///Forward declaration 
///</summary>

		//static int xferOptimization(
		//  Parse pParse,        /* Parser context */
		//  Table pDest,         /* The table we are inserting into */
		//  Select pSelect,      /* A SELECT statement to use as the data source */
		//  int onError,          /* How to handle constraint errors */
		//  int iDbDest           /* The database of pDest */
		//);
		///<summary>
		/// This routine is call to handle SQL of the following forms:
		///
		///    insert into TABLE (IDLIST) values(EXPRLIST)
		///    insert into TABLE (IDLIST) select
		///
		/// The IDLIST following the table name is always optional.  If omitted,
		/// then a list of all columns for the table is substituted.  The IDLIST
		/// appears in the pColumn parameter.  pColumn is NULL if IDLIST is omitted.
		///
		/// The pList parameter holds EXPRLIST in the first form of the INSERT
		/// statement above, and pSelect is NULL.  For the second form, pList is
		/// NULL and pSelect is a pointer to the select statement used to generate
		/// data for the insert.
		///
		/// The code generated follows one of four templates.  For a simple
		/// select with data coming from a VALUES clause, the code executes
		/// once straight down through.  Pseudo-code follows (we call this
		/// the "1st template"):
		///
		///         open write cursor to <table> and its indices
		///         puts VALUES clause expressions onto the stack
		///         write the resulting record into <table>
		///         cleanup
		///
		/// The three remaining templates assume the statement is of the form
		///
		///   INSERT INTO <table> SELECT ...
		///
		/// If the SELECT clause is of the restricted form "SELECT * FROM <table2>" -
		/// in other words if the SELECT pulls all columns from a single table
		/// and there is no WHERE or LIMIT or GROUP BY or ORDER BY clauses, and
		/// if <table2> and <table1> are distinct tables but have identical
		/// schemas, including all the same indices, then a special optimization
		/// is invoked that copies raw records from <table2> over to <table1>.
		/// See the xferOptimization() function for the implementation of this
		/// template.  This is the 2nd template.
		///
		///         open a write cursor to <table>
		///         open read cursor on <table2>
		///         transfer all records in <table2> over to <table>
		///         close cursors
		///         foreach index on <table>
		///           open a write cursor on the <table> index
		///           open a read cursor on the corresponding <table2> index
		///           transfer all records from the read to the write cursors
		///           close cursors
		///         end foreach
		///
		/// The 3rd template is for when the second template does not apply
		/// and the SELECT clause does not read from <table> at any time.
		/// The generated code follows this template:
		///
		///         EOF <- 0
		///         X <- A
		///         goto B
		///      A: setup for the SELECT
		///         loop over the rows in the SELECT
		///           load values into registers R..R+n
		///           yield X
		///         end loop
		///         cleanup after the SELECT
		///         EOF <- 1
		///         yield X
		///         goto A
		///      B: open write cursor to <table> and its indices
		///      C: yield X
		///         if EOF goto D
		///         insert the select result into <table> from R..R+n
		///         goto C
		///      D: cleanup
		///
		/// The 4th template is used if the insert statement takes its
		/// values from a SELECT but the data is being inserted into a table
		/// that is also read as part of the SELECT.  In the third form,
		/// we have to use a intermediate table to store the results of
		/// the select.  The template is like this:
		///
		///         EOF <- 0
		///         X <- A
		///         goto B
		///      A: setup for the SELECT
		///         loop over the tables in the SELECT
		///           load value into register R..R+n
		///           yield X
		///         end loop
		///         cleanup after the SELECT
		///         EOF <- 1
		///         yield X
		///         halt-error
		///      B: open temp table
		///      L: yield X
		///         if EOF goto M
		///         insert row from R..R+n into temp table
		///         goto L
		///      M: open write cursor to <table> and its indices
		///         rewind temp table
		///      C: loop over rows of intermediate table
		///           transfer values form intermediate table into <table>
		///         end loop
		///      D: cleanup
		///
		///</summary>
		// OVERLOADS, so I don't need to rewrite parse.c
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
		//#if tmask
		// #undef tmask
		//#endif
		///<summary>
		/// Generate code to do constraint checks prior to an INSERT or an UPDATE.
		///
		/// The input is a range of consecutive registers as follows:
		///
		///    1.  The rowid of the row after the update.
		///
		///    2.  The data in the first column of the entry after the update.
		///
		///    i.  Data from middle columns...
		///
		///    N.  The data in the last column of the entry after the update.
		///
		/// The regRowid parameter is the index of the register containing (1).
		///
		/// If isUpdate is true and rowidChng is non-zero, then rowidChng contains
		/// the address of a register containing the rowid before the update takes
		/// place. isUpdate is true for UPDATEs and false for INSERTs. If isUpdate
		/// is false, indicating an INSERT statement, then a non-zero rowidChng
		/// indicates that the rowid was explicitly specified as part of the
		/// INSERT statement. If rowidChng is false, it means that  the rowid is
		/// computed automatically in an insert or that the rowid value is not
		/// modified by an update.
		///
		/// The code generated by this routine store new index entries into
		/// registers identified by aRegIdx[].  No index entry is created for
		/// indices where aRegIdx[i]==0.  The order of indices in aRegIdx[] is
		/// the same as the order of indices on the linked list of indices
		/// attached to the table.
		///
		/// This routine also generates code to check constraints.  NOT NULL,
		/// CHECK, and UNIQUE constraints are all checked.  If a constraint fails,
		/// then the appropriate action is performed.  There are five possible
		/// actions: ROLLBACK, ABORT, FAIL, REPLACE, and IGNORE.
		///
		///  Constraint type  Action       What Happens
		///  ---------------  ----------   ----------------------------------------
		///  any              ROLLBACK     The current transaction is rolled back and
		///                                sqlite3_exec() returns immediately with a
		///                                return code of SQLITE_CONSTRAINT.
		///
		///  any              ABORT        Back out changes from the current command
		///                                only (do not do a complete rollback) then
		///                                cause sqlite3_exec() to return immediately
		///                                with SQLITE_CONSTRAINT.
		///
		///  any              FAIL         Sqlite_exec() returns immediately with a
		///                                return code of SQLITE_CONSTRAINT.  The
		///                                transaction is not rolled back and any
		///                                prior changes are retained.
		///
		///  any              IGNORE       The record number and data is popped from
		///                                the stack and there is an immediate jump
		///                                to label ignoreDest.
		///
		///  NOT NULL         REPLACE      The NULL value is replace by the default
		///                                value for that column.  If the default value
		///                                is NULL, the action is the same as ABORT.
		///
		///  UNIQUE           REPLACE      The other row that conflicts with the row
		///                                being inserted is removed.
		///
		///  CHECK            REPLACE      Illegal.  The results in an exception.
		///
		/// Which action to take is determined by the overrideError parameter.
		/// Or if overrideError==OE_Default, then the pParse.onError parameter
		/// is used.  Or if pParse.onError==OE_Default then the onError value
		/// for the constraint is used.
		///
		/// The calling routine must open a read/write cursor for pTab with
		/// cursor number "baseCur".  All indices of pTab must also have open
		/// read/write cursors with cursor number baseCur+i for the i-th cursor.
		/// Except, if there is no possibility of a REPLACE action then
		/// cursors do not need to be open for indices where aRegIdx[i]==0.
		///
		///</summary>
		///<summary>
		/// This routine generates code to finish the INSERT or UPDATE operation
		/// that was started by a prior call to sqlite3GenerateConstraintChecks.
		/// A consecutive range of registers starting at regRowid contains the
		/// rowid and the content to be inserted.
		///
		/// The arguments to this routine should be the same as the first six
		/// arguments to sqlite3GenerateConstraintChecks.
		///
		///</summary>
		///<summary>
		/// Generate code that will open cursors for a table and for all
		/// indices of that table.  The "baseCur" parameter is the cursor number used
		/// for the table.  Indices are opened on subsequent cursors.
		///
		/// Return the number of indices on the table.
		///
		///</summary>
		#if SQLITE_TEST
																																										    /*
** The following global variable is incremented whenever the
** transfer optimization is used.  This is used for testing
** purposes only - to make sure the transfer optimization really
** is happening when it is suppose to.
*/
#if !TCLSH
																																										    static int sqlite3_xferopt_count = 0;
#else
																																										    static tcl.lang.Var.SQLITE3_GETSET sqlite3_xferopt_count = new tcl.lang.Var.SQLITE3_GETSET( "sqlite3_xferopt_count" );
#endif
																																										#endif
		#if !SQLITE_OMIT_XFER_OPT
		///<summary>
		/// Check to collation names to see if they are compatible.
		///</summary>
		static bool xferCompatibleCollation (string z1, string z2)
		{
			if (z1 == null) {
				return z2 == null;
			}
			if (z2 == null) {
				return false;
			}
			return z1.Equals (z2, StringComparison.InvariantCultureIgnoreCase);
		}

		///<summary>
		/// Check to see if index pSrc is compatible as a source of data
		/// for index pDest in an insert transfer optimization.  The rules
		/// for a compatible index:
		///
		///    *   The index is over the same set of columns
		///    *   The same DESC and ASC markings occurs on all columns
		///    *   The same onError processing (OE_Abort, OE_Ignore, etc)
		///    *   The same collating sequence on each column
		///
		///</summary>
		static bool xferCompatibleIndex (Index pDest, Index pSrc)
		{
			int i;
			Debug.Assert (pDest != null && pSrc != null);
			Debug.Assert (pDest.pTable != pSrc.pTable);
			if (pDest.nColumn != pSrc.nColumn) {
				return false;
				///
///<summary>
///Different number of columns 
///</summary>

			}
			if (pDest.onError != pSrc.onError) {
				return false;
				///
///<summary>
///Different conflict resolution strategies 
///</summary>

			}
			for (i = 0; i < pSrc.nColumn; i++) {
				if (pSrc.aiColumn [i] != pDest.aiColumn [i]) {
					return false;
					///
///<summary>
///Different columns indexed 
///</summary>

				}
				if (pSrc.aSortOrder [i] != pDest.aSortOrder [i]) {
					return false;
					///
///<summary>
///Different sort orders 
///</summary>

				}
				if (!xferCompatibleCollation (pSrc.azColl [i], pDest.azColl [i])) {
					return false;
					///
///<summary>
///Different collating sequences 
///</summary>

				}
			}
			///
///<summary>
///If no test above fails then the indices must be compatible 
///</summary>

			return true;
		}

		///
///<summary>
///Attempt the transfer optimization on INSERTs of the form
///
///INSERT INTO tab1 SELECT * FROM tab2;
///
///This optimization is only attempted if
///
///(1)  tab1 and tab2 have identical schemas including all the
///same indices and constraints
///
///(2)  tab1 and tab2 are different tables
///
///(3)  There must be no triggers on tab1
///
///(4)  The result set of the SELECT statement is "*"
///
///(5)  The SELECT statement has no WHERE, HAVING, ORDER BY, GROUP BY,
///or LIMIT clause.
///
///(6)  The SELECT statement is a simple (not a compound) select that
///contains only tab2 in its FROM clause
///
///This method for implementing the INSERT transfers raw records from
///tab2 over to tab1.  The columns are not decoded.  Raw records from
///the indices of tab2 are transfered to tab1 as well.  In so doing,
///the resulting tab1 has much less fragmentation.
///
///This routine returns TRUE if the optimization is attempted.  If any
///of the conditions above fail so that the optimization should not
///be attempted, then this routine returns FALSE.
///
///</summary>

		static int xferOptimization (Parse pParse, ///
///<summary>
///Parser context 
///</summary>

		Table pDest, ///
///<summary>
///The table we are inserting into 
///</summary>

		Select pSelect, ///
///<summary>
///A SELECT statement to use as the data source 
///</summary>

		int onError, ///
///<summary>
///How to handle constraint errors 
///</summary>

		int iDbDest///
///<summary>
///The database of pDest 
///</summary>

		)
		{
			ExprList pEList;
			///
///<summary>
///The result set of the SELECT 
///</summary>

			Table pSrc;
			///
///<summary>
///The table in the FROM clause of SELECT 
///</summary>

			Index pSrcIdx, pDestIdx;
			///
///<summary>
///Source and destination indices 
///</summary>

			SrcList_item pItem;
			///
///<summary>
///An element of pSelect.pSrc 
///</summary>

			int i;
			///
///<summary>
///Loop counter 
///</summary>

			int iDbSrc;
			///
///<summary>
///The database of pSrc 
///</summary>

			int iSrc, iDest;
			///
///<summary>
///Cursors from source and destination 
///</summary>

			int addr1, addr2;
			///
///<summary>
///Loop addresses 
///</summary>

			int emptyDestTest;
			///
///<summary>
///Address of test for empty pDest 
///</summary>

			int emptySrcTest;
			///
///<summary>
///Address of test for empty pSrc 
///</summary>

			Vdbe v;
			///
///<summary>
///The VDBE we are building 
///</summary>

			KeyInfo pKey;
			///
///<summary>
///Key information for an index 
///</summary>

			int regAutoinc;
			///
///<summary>
///Memory register used by AUTOINC 
///</summary>

			bool destHasUniqueIdx = false;
			///
///<summary>
///True if pDest has a UNIQUE index 
///</summary>

			int regData, regRowid;
			///
///<summary>
///Registers holding data and rowid 
///</summary>

			if (pSelect == null) {
				return 0;
				///
///<summary>
///Must be of the form  INSERT INTO ... SELECT ... 
///</summary>

			}
			#if !SQLITE_OMIT_TRIGGER
			if (sqlite3TriggerList (pParse, pDest) != null) {
				return 0;
				///
///<summary>
///tab1 must not have triggers 
///</summary>

			}
			#endif
			if ((pDest.tabFlags & TF_Virtual) != 0) {
				return 0;
				///
///<summary>
///tab1 must not be a virtual table 
///</summary>

			}
			if (onError == OE_Default) {
				onError = OE_Abort;
			}
			if (onError != OE_Abort && onError != OE_Rollback) {
				return 0;
				///
///<summary>
///Cannot do OR REPLACE or OR IGNORE or OR FAIL 
///</summary>

			}
			Debug.Assert (pSelect.pSrc != null);
			///
///<summary>
///allocated even if there is no FROM clause 
///</summary>

			if (pSelect.pSrc.nSrc != 1) {
				return 0;
				///
///<summary>
///FROM clause must have exactly one term 
///</summary>

			}
			if (pSelect.pSrc.a [0].pSelect != null) {
				return 0;
				///
///<summary>
///FROM clause cannot contain a subquery 
///</summary>

			}
			if (pSelect.pWhere != null) {
				return 0;
				///
///<summary>
///SELECT may not have a WHERE clause 
///</summary>

			}
			if (pSelect.pOrderBy != null) {
				return 0;
				///
///<summary>
///SELECT may not have an ORDER BY clause 
///</summary>

			}
			///
///<summary>
///Do not need to test for a HAVING clause.  If HAVING is present but
///there is no ORDER BY, we will get an error. 
///</summary>

			if (pSelect.pGroupBy != null) {
				return 0;
				///
///<summary>
///SELECT may not have a GROUP BY clause 
///</summary>

			}
			if (pSelect.pLimit != null) {
				return 0;
				///
///<summary>
///SELECT may not have a LIMIT clause 
///</summary>

			}
			Debug.Assert (pSelect.pOffset == null);
			///
///<summary>
///Must be so if pLimit==0 
///</summary>

			if (pSelect.pPrior != null) {
				return 0;
				///
///<summary>
///SELECT may not be a compound query 
///</summary>

			}
			if ((pSelect.selFlags & SelectFlags.Distinct) != 0) {
				return 0;
				///
///<summary>
///SELECT may not be DISTINCT 
///</summary>

			}
			pEList = pSelect.pEList;
			Debug.Assert (pEList != null);
			if (pEList.nExpr != 1) {
				return 0;
				///
///<summary>
///The result set must have exactly one column 
///</summary>

			}
			Debug.Assert (pEList.a [0].pExpr != null);
			if (pEList.a [0].pExpr.Operator != TokenType.TK_ALL) {
				return 0;
				///
///<summary>
///The result set must be the special operator "*" 
///</summary>

			}
			///
///<summary>
///At this point we have established that the statement is of the
///correct syntactic form to participate in this optimization.  Now
///we have to check the semantics.
///
///</summary>

			pItem = pSelect.pSrc.a [0];
			pSrc = sqlite3LocateTable (pParse, 0, pItem.zName, pItem.zDatabase);
			if (pSrc == null) {
				return 0;
				///
///<summary>
///FROM clause does not contain a real table 
///</summary>

			}
			if (pSrc == pDest) {
				return 0;
				///
///<summary>
///tab1 and tab2 may not be the same table 
///</summary>

			}
			if ((pSrc.tabFlags & TF_Virtual) != 0) {
				return 0;
				///
///<summary>
///tab2 must not be a virtual table 
///</summary>

			}
			if (pSrc.pSelect != null) {
				return 0;
				///
///<summary>
///tab2 may not be a view 
///</summary>

			}
			if (pDest.nCol != pSrc.nCol) {
				return 0;
				///
///<summary>
///Number of columns must be the same in tab1 and tab2 
///</summary>

			}
			if (pDest.iPKey != pSrc.iPKey) {
				return 0;
				///
///<summary>
///Both tables must have the same INTEGER PRIMARY KEY 
///</summary>

			}
			for (i = 0; i < pDest.nCol; i++) {
				if (pDest.aCol [i].affinity != pSrc.aCol [i].affinity) {
					return 0;
					///
///<summary>
///Affinity must be the same on all columns 
///</summary>

				}
				if (!xferCompatibleCollation (pDest.aCol [i].zColl, pSrc.aCol [i].zColl)) {
					return 0;
					///
///<summary>
///Collating sequence must be the same on all columns 
///</summary>

				}
				if (pDest.aCol [i].notNull != 0 && pSrc.aCol [i].notNull == 0) {
					return 0;
					///
///<summary>
///tab2 must be NOT NULL if tab1 is 
///</summary>

				}
			}
			for (pDestIdx = pDest.pIndex; pDestIdx != null; pDestIdx = pDestIdx.pNext) {
				if (pDestIdx.onError != OE_None) {
					destHasUniqueIdx = true;
				}
				for (pSrcIdx = pSrc.pIndex; pSrcIdx != null; pSrcIdx = pSrcIdx.pNext) {
					if (xferCompatibleIndex (pDestIdx, pSrcIdx))
						break;
				}
				if (pSrcIdx == null) {
					return 0;
					///
///<summary>
///pDestIdx has no corresponding index in pSrc 
///</summary>

				}
			}
			#if !SQLITE_OMIT_CHECK
			if (pDest.pCheck != null && 0 != sqlite3ExprCompare (pSrc.pCheck, pDest.pCheck)) {
				return 0;
				///
///<summary>
///Tables have different CHECK constraints.  Ticket #2252 
///</summary>

			}
			#endif
			#if !SQLITE_OMIT_FOREIGN_KEY
			///
///<summary>
///Disallow the transfer optimization if the destination table constains
///any foreign key constraints.  This is more restrictive than necessary.
///But the main beneficiary of the transfer optimization is the VACUUM 
///command, and the VACUUM command disables foreign key constraints.  So
///the extra complication to make this rule less restrictive is probably
///not worth the effort.  Ticket [6284df89debdfa61db8073e062908af0c9b6118e]
///
///</summary>

			if ((pParse.db.flags & SQLITE_ForeignKeys) != 0 && pDest.pFKey != null) {
				return 0;
			}
			#endif
			///
///<summary>
///If we get this far, it means either:
///
///We can always do the transfer if the table contains an
///an integer primary key
///
///We can conditionally do the transfer if the destination
///table is empty.
///
///</summary>

			#if SQLITE_TEST
																																																															#if !TCLSH
																																																															      sqlite3_xferopt_count++;
#else
																																																															      sqlite3_xferopt_count.iValue++;
#endif
																																																															#endif
			iDbSrc = sqlite3SchemaToIndex (pParse.db, pSrc.pSchema);
			v = pParse.sqlite3GetVdbe ();
			sqlite3CodeVerifySchema (pParse, iDbSrc);
			iSrc = pParse.nTab++;
			iDest = pParse.nTab++;
			regAutoinc = pParse.autoIncBegin (iDbDest, pDest);
			pParse.sqlite3OpenTable (iDest, iDbDest, pDest, OP_OpenWrite);
			if ((pDest.iPKey < 0 && pDest.pIndex != null) || destHasUniqueIdx) {
				///
///<summary>
///If tables do not have an INTEGER PRIMARY KEY and there
///are indices to be copied and the destination is not empty,
///we have to disallow the transfer optimization because the
///the rowids might change which will mess up indexing.
///
///Or if the destination has a UNIQUE index and is not empty,
///we also disallow the transfer optimization because we cannot
///insure that all entries in the union of DEST and SRC will be
///unique.
///
///</summary>

				addr1 = v.sqlite3VdbeAddOp2 (OP_Rewind, iDest, 0);
				emptyDestTest = v.sqlite3VdbeAddOp2 (OP_Goto, 0, 0);
				v.sqlite3VdbeJumpHere (addr1);
			}
			else {
				emptyDestTest = 0;
			}
			pParse.sqlite3OpenTable (iSrc, iDbSrc, pSrc, OP_OpenRead);
			emptySrcTest = v.sqlite3VdbeAddOp2 (OP_Rewind, iSrc, 0);
			regData = pParse.sqlite3GetTempReg ();
			regRowid = pParse.sqlite3GetTempReg ();
			if (pDest.iPKey >= 0) {
				addr1 = v.sqlite3VdbeAddOp2 (OP_Rowid, iSrc, regRowid);
				addr2 = v.sqlite3VdbeAddOp3 (OP_NotExists, iDest, 0, regRowid);
				sqlite3HaltConstraint (pParse, onError, "PRIMARY KEY must be unique", P4_STATIC);
				v.sqlite3VdbeJumpHere (addr2);
				pParse.autoIncStep (regAutoinc, regRowid);
			}
			else
				if (pDest.pIndex == null) {
					addr1 = v.sqlite3VdbeAddOp2 (OP_NewRowid, iDest, regRowid);
				}
				else {
					addr1 = v.sqlite3VdbeAddOp2 (OP_Rowid, iSrc, regRowid);
					Debug.Assert ((pDest.tabFlags & TF_Autoincrement) == 0);
				}
			v.sqlite3VdbeAddOp2 (OP_RowData, iSrc, regData);
			v.sqlite3VdbeAddOp3 (OP_Insert, iDest, regData, regRowid);
			v.sqlite3VdbeChangeP5 (OPFLAG_NCHANGE | OPFLAG_LASTROWID | OPFLAG_APPEND);
			v.sqlite3VdbeChangeP4 (-1, pDest.zName, 0);
			v.sqlite3VdbeAddOp2 (OP_Next, iSrc, addr1);
			for (pDestIdx = pDest.pIndex; pDestIdx != null; pDestIdx = pDestIdx.pNext) {
				for (pSrcIdx = pSrc.pIndex; pSrcIdx != null; pSrcIdx = pSrcIdx.pNext) {
					if (xferCompatibleIndex (pDestIdx, pSrcIdx))
						break;
				}
				Debug.Assert (pSrcIdx != null);
				v.sqlite3VdbeAddOp2 (OP_Close, iSrc, 0);
				v.sqlite3VdbeAddOp2 (OP_Close, iDest, 0);
				pKey = sqlite3IndexKeyinfo (pParse, pSrcIdx);
				v.sqlite3VdbeAddOp4 (OP_OpenRead, iSrc, pSrcIdx.tnum, iDbSrc, pKey, P4_KEYINFO_HANDOFF);
				#if SQLITE_DEBUG
																																																																																				        VdbeComment( v, "%s", pSrcIdx.zName );
#endif
				pKey = sqlite3IndexKeyinfo (pParse, pDestIdx);
				v.sqlite3VdbeAddOp4 (OP_OpenWrite, iDest, pDestIdx.tnum, iDbDest, pKey, P4_KEYINFO_HANDOFF);
				#if SQLITE_DEBUG
																																																																																				        VdbeComment( v, "%s", pDestIdx.zName );
#endif
				addr1 = v.sqlite3VdbeAddOp2 (OP_Rewind, iSrc, 0);
				v.sqlite3VdbeAddOp2 (OP_RowKey, iSrc, regData);
				v.sqlite3VdbeAddOp3 (OP_IdxInsert, iDest, regData, 1);
				v.sqlite3VdbeAddOp2 (OP_Next, iSrc, addr1 + 1);
				v.sqlite3VdbeJumpHere (addr1);
			}
			v.sqlite3VdbeJumpHere (emptySrcTest);
			pParse.sqlite3ReleaseTempReg (regRowid);
			pParse.sqlite3ReleaseTempReg (regData);
			v.sqlite3VdbeAddOp2 (OP_Close, iSrc, 0);
			v.sqlite3VdbeAddOp2 (OP_Close, iDest, 0);
			if (emptyDestTest != 0) {
				v.sqlite3VdbeAddOp2 (OP_Halt, SQLITE_OK, 0);
				v.sqlite3VdbeJumpHere (emptyDestTest);
				v.sqlite3VdbeAddOp2 (OP_Close, iDest, 0);
				return 0;
			}
			else {
				return 1;
			}
		}
	#endif
	}
}
