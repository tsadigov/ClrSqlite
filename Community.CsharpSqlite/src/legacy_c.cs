using System;
using System.Diagnostics;
using System.Text;

namespace Community.CsharpSqlite
{

    using System.Linq;
    using Engine;
    using sqlite3_stmt = Engine.Vdbe;
    
    using vdbeapi = Engine.vdbeapi;
    using Community.CsharpSqlite.Utils;

    public class legacy
    {

        ///
        ///<summary>
        ///2001 September 15
        ///
        ///The author disclaims copyright to this source code.  In place of
        ///a legal notice, here is a blessing:
        ///
        ///May you do good and not evil.
        ///May you find forgiveness for yourself and forgive others.
        ///May you share freely, never taking more than you give.
        ///
        ///
        ///Main file for the SQLite library.  The routines in this file
        ///implement the programmer interface to the library.  Routines in
        ///other files are for internal use by SQLite and should not be
        ///accessed by users of the library.
        ///
        ///</summary>
        ///<param name="Included in SQLite3 port to C#">SQLite;  2008 Noah B Hart</param>
        ///<param name="C#">SQLite is an independent reimplementation of the SQLite software library</param>
        ///<param name=""></param>
        ///<param name="SQLITE_SOURCE_ID: 2010">23 18:52:01 42537b60566f288167f1b5864a5435986838e3a3</param>
        ///<param name=""></param>
        ///<param name=""></param>
        ///<param name=""></param>

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
        static public SqlResult exec(Connection db,
            ///The database on which the SQL executes                 
            string zSql,
            ///The SQL to be executed 
            int NoCallback, int NoArgs, int NoErrors)
        {
            string Errors = "";
            return sqlite3_exec(db, zSql, null, null, ref Errors);
        }

        static public SqlResult exec(
            /*The database on which the SQL executes */Connection db,
            /*The SQL to be executed */string zSql,
            /*Invoke this callback routine *///sqlite3_callback 
            dxCallback xCallback,
            /*First argument to xCallback() */object pArg,
            int NoErrors)
        {
            string Errors = "";
            return legacy.sqlite3_exec(db, zSql, xCallback, pArg, ref Errors);
        }


        static public SqlResult exec(
            /*The database on which the SQL executes */
            Connection db,
            /*The SQL to be executed */
            string zSql,
            /*Invoke this callback routine */
            //sqlite3_callback 
            dxCallback xCallback,
            /*First argument to xCallback() */
            object pArg,
            /*Write error messages here */
            ref string pzErrMsg
        )
        {
            return sqlite3_exec(db, zSql, xCallback, pArg, ref pzErrMsg);
        }


        //OVERLOADS 
        public static SqlResult sqlite3_exec(
            /*The database on which the SQL executes */ Connection db,
            /*The SQL to be executed */string zSql,
            int NoCallback, int NoArgs, int NoErrors)
        {
            string Errors = "";
            return sqlite3_exec(db, zSql, null, null, ref Errors);
        }

        public static SqlResult sqlite3_exec(Connection db,
            ///The database on which the SQL executes 
            string zSql,
            ///The SQL to be executed 
            //sqlite3_callback 
            dxCallback xCallback, ///
                                  ///Invoke this callback routine 

        object pArg, ///
                     ///First argument to xCallback() 

        int NoErrors)
        {
            string Errors = "";
            return legacy.sqlite3_exec(db, zSql, xCallback, pArg, ref Errors);
        }




        ///
        ///<summary>
        ///</summary>
        ///<param name="CAPI3REF: One">Step Query Execution Interface</param>
        ///<param name=""></param>
        ///The sqlite3_exec() interface is a convenience wrapper around
        ///[sqlite3_prepare_v2()], [sqlite3_step()], and [sqlite3_finalize()],">[sqlite3_prepare_v2()], [sqlite3_step()], and [sqlite3_finalize()],
        ///that allows an application to run multiple statements of SQL
        ///without having to use a lot of C code. </param>
        ///
        ///^The sqlite3_exec() interface runs zero or more UTF-8 encoded,
        ///semicolon-separate SQL statements passed into its 2nd argument,
        ///in the context of the [database connection] passed in as its 1st
        ///argument.  ^If the callback function of the 3rd argument to
        ///sqlite3_exec() is not NULL, then it is invoked for each result row
        ///coming out of the evaluated SQL statements.  ^The 4th argument to-coming out of the evaluated SQL statements.  ^The 4th argument to
        ///sqlite3_exec() is relayed through to the 1st argument of each
        ///callback invocation.  ^If the callback pointer to sqlite3_exec()
        ///is NULL, then no callback is ever invoked and result rows are
        ///ignored.
        ///
        ///^If an error occurs while evaluating the SQL statements passed into
        ///sqlite3_exec(), then execution of the current statement stops and
        ///subsequent statements are skipped.  ^If the 5th parameter to sqlite3_exec()
        ///<param name="is not NULL then any error message is written into memory obtained">is not NULL then any error message is written into memory obtained</param>
        ///<param name="from [sqlite3_malloc()] and passed back through the 5th parameter.">from [sqlite3_malloc()] and passed back through the 5th parameter.</param>
        ///<param name="To avoid memory leaks, the application should invoke [malloc_cs.sqlite3_free()]">To avoid memory leaks, the application should invoke [malloc_cs.sqlite3_free()]</param>
        ///<param name="on error message strings returned through the 5th parameter of">on error message strings returned through the 5th parameter of</param>
        ///<param name="of sqlite3_exec() after the error message string is no longer needed.">of sqlite3_exec() after the error message string is no longer needed.</param>
        ///<param name="^If the 5th parameter to sqlite3_exec() is not NULL and no errors">^If the 5th parameter to sqlite3_exec() is not NULL and no errors</param>
        ///<param name="occur, then sqlite3_exec() sets the pointer in its 5th parameter to">occur, then sqlite3_exec() sets the pointer in its 5th parameter to</param>
        ///<param name="NULL before returning.">NULL before returning.</param>
        ///<param name=""></param>
        ///<param name="^If an sqlite3_exec() callback returns non">zero, the sqlite3_exec()</param>
        ///<param name="routine returns SQLITE_ABORT without invoking the callback again and">routine returns SQLITE_ABORT without invoking the callback again and</param>
        ///<param name="without running any subsequent SQL statements.">without running any subsequent SQL statements.</param>
        ///<param name=""></param>
        ///<param name="^The 2nd argument to the sqlite3_exec() callback function is the">^The 2nd argument to the sqlite3_exec() callback function is the</param>
        ///<param name="number of columns in the result.  ^The 3rd argument to the sqlite3_exec()">number of columns in the result.  ^The 3rd argument to the sqlite3_exec()</param>
        ///<param name="callback is an array of pointers to strings obtained as if from">callback is an array of pointers to strings obtained as if from</param>
        ///<param name="[sqlite3_column_text()], one for each column.  ^If an element of a">[sqlite3_column_text()], one for each column.  ^If an element of a</param>
        ///<param name="result row is NULL then the corresponding string pointer for the">result row is NULL then the corresponding string pointer for the</param>
        ///<param name="sqlite3_exec() callback is a NULL pointer.  ^The 4th argument to the">sqlite3_exec() callback is a NULL pointer.  ^The 4th argument to the</param>
        ///<param name="sqlite3_exec() callback is an array of pointers to strings where each">sqlite3_exec() callback is an array of pointers to strings where each</param>
        ///<param name="entry represents the name of corresponding result column as obtained">entry represents the name of corresponding result column as obtained</param>
        ///<param name="from [vdbeapi.sqlite3_column_name()].">from [vdbeapi.sqlite3_column_name()].</param>
        ///<param name=""></param>
        ///<param name="^If the 2nd parameter to sqlite3_exec() is a NULL pointer, a pointer">^If the 2nd parameter to sqlite3_exec() is a NULL pointer, a pointer</param>
        ///<param name="to an empty string, or a pointer that contains only whitespace and/or ">to an empty string, or a pointer that contains only whitespace and/or </param>
        ///<param name="SQL comments, then no SQL statements are evaluated and the database">SQL comments, then no SQL statements are evaluated and the database</param>
        ///<param name="is not changed.">is not changed.</param>
        ///<param name=""></param>
        ///<param name="Restrictions:">Restrictions:</param>
        ///<param name=""></param>
        ///<param name="<ul>"><ul></param>
        ///<param name="<li> The application must insure that the 1st parameter to sqlite3_exec()"><li> The application must insure that the 1st parameter to sqlite3_exec()</param>
        ///<param name="is a valid and open [database connection].">is a valid and open [database connection].</param>
        ///<param name="<li> The application must not close [database connection] specified by"><li> The application must not close [database connection] specified by</param>
        ///<param name="the 1st parameter to sqlite3_exec() while sqlite3_exec() is running.">the 1st parameter to sqlite3_exec() while sqlite3_exec() is running.</param>
        ///<param name="<li> The application must not modify the SQL statement text passed into"><li> The application must not modify the SQL statement text passed into</param>
        ///<param name="the 2nd parameter of sqlite3_exec() while sqlite3_exec() is running.">the 2nd parameter of sqlite3_exec() while sqlite3_exec() is running.</param>
        ///<param name="</ul>"></ul></param>
        ///<param name=""></param>

        //SQLITE_API int sqlite3_exec(
        //  sqlite3*,                                  /* An open database */
        //  string sql,                           /* SQL to be evaluated */
        //  int (*callback)(void*,int,char**,char*),  /* Callback function */
        //  void *,                                    /* 1st argument to callback */
        //  char **errmsg                              /* Error msg written here */
        //);
        ///

        static public SqlResult sqlite3_exec(
            Connection db, ///The database on which the SQL executes 
            string zSql, ///The SQL to be executed 
            //sqlite3_callback 
            dxCallback xCallback, ///Invoke this callback routine 
            object pArg, ///First argument to xCallback() 
            ref string pzErrMsg///Write error messages here 
        )
        {
            ///Return code 
            SqlResult result = SqlResult.SQLITE_OK;

            ///Tail of unprocessed SQL 
            string zLeftover = "";

            ///The current SQL statement 
            sqlite3_stmt pStmt = null;

            ///Names of result columns 
            string[] azCols = null;

            ///Number of retry attempts
            int nRetry = 0;

            ///True if callback data is initialized 
            int callbackIsInit;


            if (!utilc.sqlite3SafetyCheckOk(db))
                return sqliteinth.SQLITE_MISUSE_BKPT();
            if (zSql == null)
                zSql = "";
            db.mutex.sqlite3_mutex_enter();
            utilc.sqlite3Error(db, SqlResult.SQLITE_OK, 0);
            while ((result == SqlResult.SQLITE_OK || (result == SqlResult.SQLITE_SCHEMA && (++nRetry) < 2)) && zSql != "")
            {
                int nCol;
                string[] azVals = null;
                pStmt = null;
                result = (SqlResult)Sqlite3.sqlite3_prepare(db, zSql, -1, ref pStmt, ref zLeftover);
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
                callbackIsInit = 0;
                nCol = pStmt.getColumnCount();
                while (true)
                {
                    int i;
                    result = vdbeapi.sqlite3_step(pStmt);

                    ///Invoke the callback function if required 
                    if (xCallback != null && (SqlResult.SQLITE_ROW == result || (SqlResult.SQLITE_DONE == result && callbackIsInit == 0 && (db.flags & SqliteFlags.SQLITE_NullCallback) != 0)))
                    {
                        if (0 == callbackIsInit)
                        {
                            //sqlite3DbMallocZero(db, 2*nCol*sizeof(const char*) + 1);
                            //if ( azCols == null )
                            //{
                            //  goto exec_out;
                            //}

                            azCols = Enumerable.Range(0, nCol)
                                .Select(idx => {
                                    var name = vdbeapi.sqlite3_column_name(pStmt, idx);
                                        ///sqlite3VdbeSetColName() installs column names as UTF8
                                        ///strings so there is no way for vdbeapi.sqlite3_column_name() to fail.
                                        Debug.Assert(null != name);

                                    return name;
                                })
                                .ToArray();


                            callbackIsInit = 1;
                        }
                        if (result == SqlResult.SQLITE_ROW)
                        {
                            azVals = new string[nCol];
                            // azCols[nCol];
                            for (i = 0; i < nCol; i++)
                            {
                                azVals[i] = vdbeapi.sqlite3_column_text(pStmt, i);
                                if (azVals[i] == null && vdbeapi.sqlite3_column_type(pStmt, i) != FoundationalType.SQLITE_NULL)
                                {
                                    //db.mallocFailed = 1;
                                    //goto exec_out;
                                }
                            }
                        }
                        if (xCallback(pArg, nCol, azVals, azCols) != 0)//-----<<----------<<----------<<----------<<-----
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
                db.sqlite3DbFree(ref azCols);
                azCols = null;
            }
        exec_out:
            if (pStmt != null)
                vdbeaux.sqlite3VdbeFinalize(ref pStmt);
            db.sqlite3DbFree(ref azCols);
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
            db.mutex.sqlite3_mutex_leave();
            return result;
        }


    }
}
