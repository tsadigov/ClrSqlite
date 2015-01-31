using System;
using System.Diagnostics;
using System.Text;
using Bitmask=System.UInt64;
using u8=System.Byte;
using u32=System.UInt32;
namespace Community.CsharpSqlite
{
        public class fkeyc
        {
            ///
            ///<summary>
            ///
            ///The author disclaims copyright to this source code.  In place of
            ///a legal notice, here is a blessing:
            ///
            ///May you do good and not evil.
            ///May you find forgiveness for yourself and forgive others.
            ///May you share freely, never taking more than you give.
            ///
            ///
            ///This file contains code used by the compiler to add foreign key
            ///support to compiled SQL statements.
            ///
            ///</summary>
            ///<param name="Included in SQLite3 port to C#">SQLite;  2008 Noah B Hart</param>
            ///<param name="C#">SQLite is an independent reimplementation of the SQLite software library</param>
            ///<param name=""></param>
            ///<param name="SQLITE_SOURCE_ID: 2011">23 19:49:22 4374b7e83ea0a3fbc3691f9c0c936272862f32f2</param>
            ///<param name=""></param>`
            ///<param name=""></param>
            //#include "sqliteInt.h"
#if !SQLITE_OMIT_FOREIGN_KEY
#if !SQLITE_OMIT_TRIGGER
            ///
            ///<summary>
            ///Deferred and Immediate FKs
            ///</summary>
            ///<param name=""></param>
            ///<param name=""></param>
            ///<param name="Foreign keys in SQLite come in two flavours: deferred and immediate.">Foreign keys in SQLite come in two flavours: deferred and immediate.</param>
            ///<param name="If an immediate foreign key constraint is violated, SQLITE_CONSTRAINT">If an immediate foreign key constraint is violated, SQLITE_CONSTRAINT</param>
            ///<param name="is returned and the current statement transaction rolled back. If a ">is returned and the current statement transaction rolled back. If a </param>
            ///<param name="deferred foreign key constraint is violated, no action is taken ">deferred foreign key constraint is violated, no action is taken </param>
            ///<param name="immediately. However if the application attempts to commit the ">immediately. However if the application attempts to commit the </param>
            ///<param name="transaction before fixing the constraint violation, the attempt fails.">transaction before fixing the constraint violation, the attempt fails.</param>
            ///<param name=""></param>
            ///<param name="Deferred constraints are implemented using a simple counter associated">Deferred constraints are implemented using a simple counter associated</param>
            ///<param name="with the database handle. The counter is set to zero each time a ">with the database handle. The counter is set to zero each time a </param>
            ///<param name="database transaction is opened. Each time a statement is executed ">database transaction is opened. Each time a statement is executed </param>
            ///<param name="that causes a foreign key violation, the counter is incremented. Each">that causes a foreign key violation, the counter is incremented. Each</param>
            ///<param name="time a statement is executed that removes an existing violation from">time a statement is executed that removes an existing violation from</param>
            ///<param name="the database, the counter is decremented. When the transaction is">the database, the counter is decremented. When the transaction is</param>
            ///<param name="committed, the commit fails if the current value of the counter is">committed, the commit fails if the current value of the counter is</param>
            ///<param name="greater than zero. This scheme has two big drawbacks:">greater than zero. This scheme has two big drawbacks:</param>
            ///<param name=""></param>
            ///<param name="When a commit fails due to a deferred foreign key constraint, ">When a commit fails due to a deferred foreign key constraint, </param>
            ///<param name="there is no way to tell which foreign constraint is not satisfied,">there is no way to tell which foreign constraint is not satisfied,</param>
            ///<param name="or which row it is not satisfied for.">or which row it is not satisfied for.</param>
            ///<param name=""></param>
            ///<param name="If the database contains foreign key violations when the ">If the database contains foreign key violations when the </param>
            ///<param name="transaction is opened, this may cause the mechanism to malfunction.">transaction is opened, this may cause the mechanism to malfunction.</param>
            ///<param name=""></param>
            ///<param name="Despite these problems, this approach is adopted as it seems simpler">Despite these problems, this approach is adopted as it seems simpler</param>
            ///<param name="than the alternatives.">than the alternatives.</param>
            ///<param name=""></param>
            ///<param name="INSERT operations:">INSERT operations:</param>
            ///<param name=""></param>
            ///<param name="I.1) For each FK for which the table is the child table, search">I.1) For each FK for which the table is the child table, search</param>
            ///<param name="the parent table for a match. If none is found increment the">the parent table for a match. If none is found increment the</param>
            ///<param name="constraint counter.">constraint counter.</param>
            ///<param name=""></param>
            ///<param name="I.2) For each FK for which the table is the parent table, ">I.2) For each FK for which the table is the parent table, </param>
            ///<param name="search the child table for rows that correspond to the new">search the child table for rows that correspond to the new</param>
            ///<param name="row in the parent table. Decrement the counter for each row">row in the parent table. Decrement the counter for each row</param>
            ///<param name="found (as the constraint is now satisfied).">found (as the constraint is now satisfied).</param>
            ///<param name=""></param>
            ///<param name="DELETE operations:">DELETE operations:</param>
            ///<param name=""></param>
            ///<param name="D.1) For each FK for which the table is the child table, ">D.1) For each FK for which the table is the child table, </param>
            ///<param name="search the parent table for a row that corresponds to the ">search the parent table for a row that corresponds to the </param>
            ///<param name="deleted row in the child table. If such a row is not found, ">deleted row in the child table. If such a row is not found, </param>
            ///<param name="decrement the counter.">decrement the counter.</param>
            ///<param name=""></param>
            ///<param name="D.2) For each FK for which the table is the parent table, search ">D.2) For each FK for which the table is the parent table, search </param>
            ///<param name="the child table for rows that correspond to the deleted row ">the child table for rows that correspond to the deleted row </param>
            ///<param name="in the parent table. For each found increment the counter.">in the parent table. For each found increment the counter.</param>
            ///<param name=""></param>
            ///<param name="UPDATE operations:">UPDATE operations:</param>
            ///<param name=""></param>
            ///<param name="An UPDATE command requires that all 4 steps above are taken, but only">An UPDATE command requires that all 4 steps above are taken, but only</param>
            ///<param name="for FK constraints for which the affected columns are actually ">for FK constraints for which the affected columns are actually </param>
            ///<param name="modified (values must be compared at runtime).">modified (values must be compared at runtime).</param>
            ///<param name=""></param>
            ///<param name="Note that I.1 and D.1 are very similar operations, as are I.2 and D.2.">Note that I.1 and D.1 are very similar operations, as are I.2 and D.2.</param>
            ///<param name="This simplifies the implementation a bit.">This simplifies the implementation a bit.</param>
            ///<param name=""></param>
            ///<param name="For the purposes of immediate FK constraints, the OR REPLACE conflict">For the purposes of immediate FK constraints, the OR REPLACE conflict</param>
            ///<param name="resolution is considered to delete rows before the new row is inserted.">resolution is considered to delete rows before the new row is inserted.</param>
            ///<param name="If a delete caused by OR REPLACE violates an FK constraint, an exception">If a delete caused by OR REPLACE violates an FK constraint, an exception</param>
            ///<param name="is thrown, even if the FK constraint would be satisfied after the new ">is thrown, even if the FK constraint would be satisfied after the new </param>
            ///<param name="row is inserted.">row is inserted.</param>
            ///<param name=""></param>
            ///<param name="Immediate constraints are usually handled similarly. The only difference ">Immediate constraints are usually handled similarly. The only difference </param>
            ///<param name="is that the counter used is stored as part of each individual statement">is that the counter used is stored as part of each individual statement</param>
            ///<param name="object (struct Vdbe). If, after the statement has run, its immediate">object (struct Vdbe). If, after the statement has run, its immediate</param>
            ///<param name="constraint counter is greater than zero, it returns SQLITE_CONSTRAINT">constraint counter is greater than zero, it returns SQLITE_CONSTRAINT</param>
            ///<param name="and the statement transaction is rolled back. An exception is an INSERT">and the statement transaction is rolled back. An exception is an INSERT</param>
            ///<param name="statement that inserts a single row only (no triggers). In this case,">statement that inserts a single row only (no triggers). In this case,</param>
            ///<param name="instead of using a counter, an exception is thrown immediately if the">instead of using a counter, an exception is thrown immediately if the</param>
            ///<param name="INSERT violates a foreign key constraint. This is necessary as such">INSERT violates a foreign key constraint. This is necessary as such</param>
            ///<param name="an INSERT does not open a statement transaction.">an INSERT does not open a statement transaction.</param>
            ///<param name=""></param>
            ///<param name="TODO: How should dropping a table be handled? How should renaming a ">TODO: How should dropping a table be handled? How should renaming a </param>
            ///<param name="table be handled?">table be handled?</param>
            ///<param name=""></param>
            ///<param name=""></param>
            ///<param name="Query API Notes">Query API Notes</param>
            ///<param name=""></param>
            ///<param name=""></param>
            ///<param name="Before coding an UPDATE or DELETE row operation, the code">generator</param>
            ///<param name="for those two operations needs to know whether or not the operation">for those two operations needs to know whether or not the operation</param>
            ///<param name="requires any FK processing and, if so, which columns of the original">requires any FK processing and, if so, which columns of the original</param>
            ///<param name="row are required by the FK processing VDBE code (i.e. if FKs were">row are required by the FK processing VDBE code (i.e. if FKs were</param>
            ///<param name="implemented using triggers, which of the old.* columns would be ">implemented using triggers, which of the old.* columns would be </param>
            ///<param name="accessed). No information is required by the code">generator before</param>
            ///<param name="coding an INSERT operation. The functions used by the UPDATE/DELETE">coding an INSERT operation. The functions used by the UPDATE/DELETE</param>
            ///<param name="generation code to query for this information are:">generation code to query for this information are:</param>
            ///<param name=""></param>
            ///<param name="sqlite3FkRequired() "> Test to see if FK processing is required.</param>
            ///<param name="sqlite3FkOldmask()  "> Query for the set of required old.* columns.</param>
            ///<param name=""></param>
            ///<param name=""></param>
            ///<param name="Externally accessible module functions">Externally accessible module functions</param>
            ///<param name=""></param>
            ///<param name=""></param>
            ///<param name="sqlite3FkCheck()    "> Check for foreign key violations.</param>
            ///<param name="sqlite3FkActions()  "> Code triggers for ON UPDATE/ON DELETE actions.</param>
            ///<param name="sqlite3FkDelete()   "> Delete an FKey structure.</param>
            ///<summary>
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
            public static FKey sqlite3FkReferences(Table pTab)
            {
                int nName = StringExtensions.sqlite3Strlen30(pTab.zName);
                return pTab.pSchema.fkeyHash.sqlite3HashFind(pTab.zName, nName, (FKey)null);
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
            static void fkTriggerDelete(sqlite3 dbMem, Trigger p)
            {
                if (p != null)
                {
                    TriggerStep pStep = p.step_list;
                    exprc.sqlite3ExprDelete(dbMem, ref pStep.pWhere);
                    exprc.sqlite3ExprListDelete(dbMem, ref pStep.pExprList);
                    SelectMethods.sqlite3SelectDelete(dbMem, ref pStep.pSelect);
                    exprc.sqlite3ExprDelete(dbMem, ref p.pWhen);
                    dbMem.sqlite3DbFree(ref p);
                }
            }
            //#define COLUMN_MASK(x) (((x)>31) ? 0xffffffff : ((u32)1<<(x)))
            public static uint COLUMN_MASK(int x)
            {
                return ((x) > 31) ? 0xffffffff : ((u32)1 << (x));
            }
#endif
            ///
            ///<summary>
            ///Free all memory associated with foreign key definitions attached to
            ///table pTab. Remove the deleted foreign keys from the Schema.fkeyHash
            ///hash table.
            ///</summary>
            public static void sqlite3FkDelete(sqlite3 db, Table pTab)
            {
                FKey pFKey;
                ///
                ///<summary>
                ///Iterator variable 
                ///</summary>
                FKey pNext;
                ///
                ///<summary>
                ///Copy of pFKey.pNextFrom 
                ///</summary>
                Debug.Assert(db == null || Sqlite3.sqlite3SchemaMutexHeld(db, 0, pTab.pSchema));
                for (pFKey = pTab.pFKey; pFKey != null; pFKey = pNext)
                {
                    ///
                    ///<summary>
                    ///Remove the FK from the fkeyHash hash table. 
                    ///</summary>
                    //if ( null == db || db.pnBytesFreed == 0 )
                    {
                        if (pFKey.pPrevTo != null)
                        {
                            pFKey.pPrevTo.pNextTo = pFKey.pNextTo;
                        }
                        else
                        {
                            FKey p = pFKey.pNextTo;
                            string z = (p != null ? pFKey.pNextTo.zTo : pFKey.zTo);
                            HashExtensions.sqlite3HashInsert( ref pTab.pSchema.fkeyHash, z, StringExtensions.sqlite3Strlen30(z), p);
                        }
                        if (pFKey.pNextTo != null)
                        {
                            pFKey.pNextTo.pPrevTo = pFKey.pPrevTo;
                        }
                        ///
                        ///<summary>
                        ///</summary>
                        ///<param name="EV: R">21917 Each foreign key constraint in SQLite is</param>
                        ///<param name="classified as either immediate or deferred.">classified as either immediate or deferred.</param>
                        ///<param name=""></param>
                        ///
                        ///<summary>
                        ///Delete any triggers created to implement actions for this FK. 
                        ///</summary>
#if !SQLITE_OMIT_TRIGGER
#endif
                    }
                    Debug.Assert(pFKey.isDeferred == 0 || pFKey.isDeferred == 1);
                    fkTriggerDelete(db, pFKey.apTrigger[0]);
                    fkTriggerDelete(db, pFKey.apTrigger[1]);
                    pNext = pFKey.pNextFrom;
                    db.sqlite3DbFree(ref pFKey);
                }
            }
#endif
        }
    
}