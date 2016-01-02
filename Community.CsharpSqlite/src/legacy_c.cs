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
** Main file for the SQLite library.  The routines in this file
** implement the programmer interface to the library.  Routines in
** other files are for internal use by SQLite and should not be
** accessed by users of the library.
*************************************************************************
**  Included in SQLite3 port to C#-SQLite;  2008 Noah B Hart
**  C#-SQLite is an independent reimplementation of the SQLite software library
**
**  SQLITE_SOURCE_ID: 2010-08-23 18:52:01 42537b60566f288167f1b5864a5435986838e3a3
**
*************************************************************************
*/
using System.Diagnostics;

namespace Community.CsharpSqlite
{
    using System.Linq;
    using Engine;
    using sqlite3_stmt = Engine.Vdbe;
    using vdbeapi = Engine.vdbeapi;
    using Community.CsharpSqlite.Utils;

    /*
    ** CAPI3REF: One-Step Query Execution Interface
    ** METHOD: sqlite3
    **
    ** The sqlite3_exec() interface is a convenience wrapper around
    ** [sqlite3_prepare_v2()], [sqlite3_step()], and [sqlite3_finalize()],
    ** that allows an application to run multiple statements of SQL
    ** without having to use a lot of C code. 
    **
    ** ^The sqlite3_exec() interface runs zero or more UTF-8 encoded,
    ** semicolon-separate SQL statements passed into its 2nd argument,
    ** in the context of the [database connection] passed in as its 1st
    ** argument.  ^If the callback function of the 3rd argument to
    ** sqlite3_exec() is not NULL, then it is invoked for each result row
    ** coming out of the evaluated SQL statements.  ^The 4th argument to
    ** sqlite3_exec() is relayed through to the 1st argument of each
    ** callback invocation.  ^If the callback pointer to sqlite3_exec()
    ** is NULL, then no callback is ever invoked and result rows are
    ** ignored.
    **
    ** ^If an error occurs while evaluating the SQL statements passed into
    ** sqlite3_exec(), then execution of the current statement stops and
    ** subsequent statements are skipped.  ^If the 5th parameter to sqlite3_exec()
    ** is not NULL then any error message is written into memory obtained
    ** from [sqlite3_malloc()] and passed back through the 5th parameter.
    ** To avoid memory leaks, the application should invoke [sqlite3_free()]
    ** on error message strings returned through the 5th parameter of
    ** of sqlite3_exec() after the error message string is no longer needed.
    ** ^If the 5th parameter to sqlite3_exec() is not NULL and no errors
    ** occur, then sqlite3_exec() sets the pointer in its 5th parameter to
    ** NULL before returning.
    **
    ** ^If an sqlite3_exec() callback returns non-zero, the sqlite3_exec()
    ** routine returns SQLITE_ABORT without invoking the callback again and
    ** without running any subsequent SQL statements.
    **
    ** ^The 2nd argument to the sqlite3_exec() callback function is the
    ** number of columns in the result.  ^The 3rd argument to the sqlite3_exec()
    ** callback is an array of pointers to strings obtained as if from
    ** [sqlite3_column_text()], one for each column.  ^If an element of a
    ** result row is NULL then the corresponding string pointer for the
    ** sqlite3_exec() callback is a NULL pointer.  ^The 4th argument to the
    ** sqlite3_exec() callback is an array of pointers to strings where each
    ** entry represents the name of corresponding result column as obtained
    ** from [sqlite3_column_name()].
    **
    ** ^If the 2nd parameter to sqlite3_exec() is a NULL pointer, a pointer
    ** to an empty string, or a pointer that contains only whitespace and/or 
    ** SQL comments, then no SQL statements are evaluated and the database
    ** is not changed.
    **
    ** Restrictions:
    **
    ** <ul>
    ** <li> The application must insure that the 1st parameter to sqlite3_exec()
    **      is a valid and open [database connection].
    ** <li> The application must not close the [database connection] specified by
    **      the 1st parameter to sqlite3_exec() while sqlite3_exec() is running.
    ** <li> The application must not modify the SQL statement text passed into
    **      the 2nd parameter of sqlite3_exec() while sqlite3_exec() is running.
    ** </ul>
    */
    public static class legacy
    {
        

        static public SqlResult Exec<T>(
            this Connection db, ///The database on which the SQL executes 
            string zSql, ///The SQL to be executed             
            dxCallback<T> xCallback, ///Invoke this callback routine  //sqlite3_callback 
            T pArg, ///First argument to xCallback() 
            ref string pzErrMsg///Write error messages here 
        )
        {
            SqlResult result = SqlResult.SQLITE_OK;///Return code 
            string zLeftover = "";///Tail of unprocessed SQL 
            sqlite3_stmt pStmt = null;///The current SQL statement 
            string[] ColumnNames = null;
            int nRetry = 0;///Number of retry attempts


            if (!utilc.sqlite3SafetyCheckOk(db))
                return sqliteinth.SQLITE_MISUSE_BKPT();
            if (zSql == null)
                zSql = "";
            using (db.mutex.scope())
            {
                utilc.sqlite3Error(db, SqlResult.SQLITE_OK, 0);
                while ((result == SqlResult.SQLITE_OK || (result == SqlResult.SQLITE_SCHEMA && (++nRetry) < 2)) && zSql != "")
                {
                    #region compile program
                    pStmt = null;
                    result = Sqlite3.sqlite3_prepare(db, zSql, -1, ref pStmt, ref zLeftover);
                    Debug.Assert(result == SqlResult.SQLITE_OK || pStmt == null);
                    if (result != SqlResult.SQLITE_OK)
                    {
                        continue;
                    }
                    if (pStmt == null)
                    {
                        ///this happens for a comment or white-space 
                        zSql = zLeftover;
                        continue;
                    }
                    var callbackIsInit = false;///True if callback data is initialized 
                    var nCol = pStmt.getColumnCount();
                    #endregion

                    while (true)
                    {                        
                        result = vdbeapi.sqlite3_step(pStmt);

                        ///Invoke the callback function if required 
                        if (xCallback != null && (SqlResult.SQLITE_ROW == result || (SqlResult.SQLITE_DONE == result && !callbackIsInit  && (db.flags & SqliteFlags.SQLITE_NullCallback) != 0)))
                        {
                            #region get column names
                            if (!callbackIsInit)
                            {                                
                                ColumnNames = Enumerable.Range(0, nCol)
                                    .Select(idx =>pStmt.get_column_name(idx)).ToArray();//GUARD: Debug.Assert(null != name);///sqlite3VdbeSetColName() installs column names as UTF8///strings so there is no way for vdbeapi.sqlite3_column_name() to fail.
                                
                                callbackIsInit = true;
                            }
                            #endregion

                            ///get field values
                            string[] rowData = SqlResult.SQLITE_ROW == result
                                            ? Enumerable.Range(0, nCol).Select(i => pStmt.get_column_text(i)).ToArray()//GUARD://db.mallocFailed = 1;//goto exec_out;// azCols[nCol];
                                            : null;
                            

                            if (xCallback(pArg, nCol, rowData, ColumnNames) != 0)//-----<<----------<<----------<<----------<<-----
                            {
                                result = SqlResult.SQLITE_ABORT;
                                vdbeaux.sqlite3VdbeFinalize(ref pStmt);
                                pStmt = null;
                                utilc.sqlite3Error(db, SqlResult.SQLITE_ABORT, 0);
                                goto exec_out;
                            }
                        }
                        if (SqlResult.SQLITE_ROW != result)
                        {
                            result = (SqlResult)vdbeaux.sqlite3VdbeFinalize(ref pStmt);
                            pStmt = null;
                            if (SqlResult.SQLITE_SCHEMA != result)
                            {
                                nRetry = 0;
                                if ((zSql = zLeftover) != "")
                                {
                                    int zindex = 0;
                                    while (zindex < zSql.Length && CharExtensions.sqlite3Isspace(zSql[zindex]))
                                        zindex++;
                                    if (zindex != 0)
                                        zSql = zindex < zSql.Length ? zSql.Substring(zindex) : "";
                                }
                            }
                            break;
                        }
                    }
                    db.DbFree(ref ColumnNames);
                    ColumnNames = null;
                }
                exec_out:
                if (pStmt != null)
                    vdbeaux.sqlite3VdbeFinalize(ref pStmt);
                db.DbFree(ref ColumnNames);
                result = (SqlResult)malloc_cs.sqlite3ApiExit(db, result);
                if (result != SqlResult.SQLITE_OK && Sqlite3.ALWAYS(result == (SqlResult)Sqlite3.sqlite3_errcode(db)) && pzErrMsg != null)
                {
                    //int nErrMsg = 1 + StringExtensions.sqlite3Strlen30(sqlite3_errmsg(db));
                    //pzErrMsg = malloc_cs.sqlite3Malloc(nErrMsg);
                    //if (pzErrMsg)
                    //{
                    //   memcpy(pzErrMsg, sqlite3_errmsg(db), nErrMsg);
                    //}else{
                    //rc = SQLITE_NOMEM;
                    //utilc.sqlite3Error(db, SQLITE_NOMEM, 0);
                    //}
                    pzErrMsg = Sqlite3.sqlite3_errmsg(db);
                }
                else
                    if (pzErrMsg != "")
                {
                    pzErrMsg = "";
                }
                Debug.Assert((result & (SqlResult)db.errMask) == result);
            }
            return result;
        }



        //#include "sqliteInt.h"
        ///<summary>
        /// Execute SQL code.  Return one of the SQLITE_ success/failure
        /// codes.  Also write an error message into memory obtained from
        /// malloc() and make pzErrMsg point to that message.
        ///
        /// If the SQL is a query, then for each row in the query result
        /// the xCallback() function is called.  pArg becomes the first
        /// argument to xCallback().  If xCallback=NULL then no callback
        /// is invoked, even for queries.
        ///
        ///</summary>
        //C# Alias
        
        public static SqlResult Exec(
             this Connection db,/*The database on which the SQL executes */
             string zSql,/*The SQL to be executed */
            int NoCallback, int NoArgs, int NoErrors)
        {
            string Errors = "";
            return Exec<object>(db, zSql, null, null, ref Errors);
        }

    

        public static SqlResult Exec<T>(
           this Connection db,///The database on which the SQL executes 
            string zSql,///The SQL to be executed 
            dxCallback<T> xCallback,//sqlite3_callback 
                                     ///Invoke this callback routine 
            T pArg,///First argument to xCallback() 
            int NoErrors
           )
        {
            string Errors = "";
            return legacy.Exec(db, zSql, xCallback, pArg, ref Errors);
        }
    }
}
