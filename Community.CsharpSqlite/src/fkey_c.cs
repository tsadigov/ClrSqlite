using System;
using System.Diagnostics;
using System.Text;
using Bitmask=System.UInt64;
using u8=System.Byte;
using u32=System.UInt32;
namespace Community.CsharpSqlite {
	public partial class Sqlite3 {
		/*
    **
    ** The author disclaims copyright to this source code.  In place of
    ** a legal notice, here is a blessing:
    **
    **    May you do good and not evil.
    **    May you find forgiveness for yourself and forgive others.
    **    May you share freely, never taking more than you give.
    **
    *************************************************************************
    ** This file contains code used by the compiler to add foreign key
    ** support to compiled SQL statements.
    *************************************************************************
    **  Included in SQLite3 port to C#-SQLite;  2008 Noah B Hart
    **  C#-SQLite is an independent reimplementation of the SQLite software library
    **
    **  SQLITE_SOURCE_ID: 2011-06-23 19:49:22 4374b7e83ea0a3fbc3691f9c0c936272862f32f2
    **
    *************************************************************************    *///#include "sqliteInt.h"
		#if !SQLITE_OMIT_FOREIGN_KEY
		#if !SQLITE_OMIT_TRIGGER
		/*
** Deferred and Immediate FKs
** --------------------------
**
** Foreign keys in SQLite come in two flavours: deferred and immediate.
** If an immediate foreign key constraint is violated, SQLITE_CONSTRAINT
** is returned and the current statement transaction rolled back. If a 
** deferred foreign key constraint is violated, no action is taken 
** immediately. However if the application attempts to commit the 
** transaction before fixing the constraint violation, the attempt fails.
**
** Deferred constraints are implemented using a simple counter associated
** with the database handle. The counter is set to zero each time a 
** database transaction is opened. Each time a statement is executed 
** that causes a foreign key violation, the counter is incremented. Each
** time a statement is executed that removes an existing violation from
** the database, the counter is decremented. When the transaction is
** committed, the commit fails if the current value of the counter is
** greater than zero. This scheme has two big drawbacks:
**
**   * When a commit fails due to a deferred foreign key constraint, 
**     there is no way to tell which foreign constraint is not satisfied,
**     or which row it is not satisfied for.
**
**   * If the database contains foreign key violations when the 
**     transaction is opened, this may cause the mechanism to malfunction.
**
** Despite these problems, this approach is adopted as it seems simpler
** than the alternatives.
**
** INSERT operations:
**
**   I.1) For each FK for which the table is the child table, search
**        the parent table for a match. If none is found increment the
**        constraint counter.
**
**   I.2) For each FK for which the table is the parent table, 
**        search the child table for rows that correspond to the new
**        row in the parent table. Decrement the counter for each row
**        found (as the constraint is now satisfied).
**
** DELETE operations:
**
**   D.1) For each FK for which the table is the child table, 
**        search the parent table for a row that corresponds to the 
**        deleted row in the child table. If such a row is not found, 
**        decrement the counter.
**
**   D.2) For each FK for which the table is the parent table, search 
**        the child table for rows that correspond to the deleted row 
**        in the parent table. For each found increment the counter.
**
** UPDATE operations:
**
**   An UPDATE command requires that all 4 steps above are taken, but only
**   for FK constraints for which the affected columns are actually 
**   modified (values must be compared at runtime).
**
** Note that I.1 and D.1 are very similar operations, as are I.2 and D.2.
** This simplifies the implementation a bit.
**
** For the purposes of immediate FK constraints, the OR REPLACE conflict
** resolution is considered to delete rows before the new row is inserted.
** If a delete caused by OR REPLACE violates an FK constraint, an exception
** is thrown, even if the FK constraint would be satisfied after the new 
** row is inserted.
**
** Immediate constraints are usually handled similarly. The only difference 
** is that the counter used is stored as part of each individual statement
** object (struct Vdbe). If, after the statement has run, its immediate
** constraint counter is greater than zero, it returns SQLITE_CONSTRAINT
** and the statement transaction is rolled back. An exception is an INSERT
** statement that inserts a single row only (no triggers). In this case,
** instead of using a counter, an exception is thrown immediately if the
** INSERT violates a foreign key constraint. This is necessary as such
** an INSERT does not open a statement transaction.
**
** TODO: How should dropping a table be handled? How should renaming a 
** table be handled?
**
**
** Query API Notes
** ---------------
**
** Before coding an UPDATE or DELETE row operation, the code-generator
** for those two operations needs to know whether or not the operation
** requires any FK processing and, if so, which columns of the original
** row are required by the FK processing VDBE code (i.e. if FKs were
** implemented using triggers, which of the old.* columns would be 
** accessed). No information is required by the code-generator before
** coding an INSERT operation. The functions used by the UPDATE/DELETE
** generation code to query for this information are:
**
**   sqlite3FkRequired() - Test to see if FK processing is required.
**   sqlite3FkOldmask()  - Query for the set of required old.* columns.
**
**
** Externally accessible module functions
** --------------------------------------
**
**   sqlite3FkCheck()    - Check for foreign key violations.
**   sqlite3FkActions()  - Code triggers for ON UPDATE/ON DELETE actions.
**   sqlite3FkDelete()   - Delete an FKey structure.
*////<summary>
		/// This function returns a pointer to the head of a linked list of FK
		/// constraints for which table pTab is the parent table. For example,
		/// given the following schema:
		///
		///   CREATE TABLE t1(a PRIMARY KEY);
		///   CREATE TABLE t2(b REFERENCES t1(a);
		///
		/// Calling this function with table "t1" as an argument returns a pointer
		/// to the FKey structure representing the foreign key constraint on table
		/// "t2". Calling this function with "t2" as the argument would return a
		/// NULL pointer (as there are no FK constraints for which t2 is the parent
		/// table).
		///
		///</summary>
		static FKey sqlite3FkReferences(Table pTab) {
			int nName=StringExtensions.sqlite3Strlen30(pTab.zName);
			return sqlite3HashFind(pTab.pSchema.fkeyHash,pTab.zName,nName,(FKey)null);
		}
		///<summary>
		/// The second argument is a Trigger structure allocated by the
		/// fkActionTrigger() routine. This function deletes the Trigger structure
		/// and all of its sub-components.
		///
		/// The Trigger structure or any of its sub-components may be allocated from
		/// the lookaside buffer belonging to database handle dbMem.
		///
		///</summary>
		static void fkTriggerDelete(sqlite3 dbMem,Trigger p) {
			if(p!=null) {
				TriggerStep pStep=p.step_list;
				sqlite3ExprDelete(dbMem,ref pStep.pWhere);
				sqlite3ExprListDelete(dbMem,ref pStep.pExprList);
				sqlite3SelectDelete(dbMem,ref pStep.pSelect);
				sqlite3ExprDelete(dbMem,ref p.pWhen);
				dbMem.sqlite3DbFree(ref p);
			}
		}
		//#define COLUMN_MASK(x) (((x)>31) ? 0xffffffff : ((u32)1<<(x)))
		static uint COLUMN_MASK(int x) {
			return ((x)>31)?0xffffffff:((u32)1<<(x));
		}
		#endif
		/*
** Free all memory associated with foreign key definitions attached to
** table pTab. Remove the deleted foreign keys from the Schema.fkeyHash
** hash table.
*/static void sqlite3FkDelete(sqlite3 db,Table pTab) {
			FKey pFKey;
			/* Iterator variable */FKey pNext;
			/* Copy of pFKey.pNextFrom */Debug.Assert(db==null||sqlite3SchemaMutexHeld(db,0,pTab.pSchema));
			for(pFKey=pTab.pFKey;pFKey!=null;pFKey=pNext) {
				/* Remove the FK from the fkeyHash hash table. *///if ( null == db || db.pnBytesFreed == 0 )
				{
					if(pFKey.pPrevTo!=null) {
						pFKey.pPrevTo.pNextTo=pFKey.pNextTo;
					}
					else {
						FKey p=pFKey.pNextTo;
						string z=(p!=null?pFKey.pNextTo.zTo:pFKey.zTo);
						sqlite3HashInsert(ref pTab.pSchema.fkeyHash,z,StringExtensions.sqlite3Strlen30(z),p);
					}
					if(pFKey.pNextTo!=null) {
						pFKey.pNextTo.pPrevTo=pFKey.pPrevTo;
					}
					/* EV: R-30323-21917 Each foreign key constraint in SQLite is
        ** classified as either immediate or deferred.
        *//* Delete any triggers created to implement actions for this FK. */
					#if !SQLITE_OMIT_TRIGGER
					#endif
				}
				Debug.Assert(pFKey.isDeferred==0||pFKey.isDeferred==1);
				fkTriggerDelete(db,pFKey.apTrigger[0]);
				fkTriggerDelete(db,pFKey.apTrigger[1]);
				pNext=pFKey.pNextFrom;
				db.sqlite3DbFree(ref pFKey);
			}
		}
	#endif
	}
}
