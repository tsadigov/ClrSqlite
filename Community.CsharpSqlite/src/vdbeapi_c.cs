using System;
using System.Diagnostics;
using System.Text;
using i64 = System.Int64;
using u16 = System.UInt16;
using u32 = System.UInt32;
using u64 = System.UInt64;
using u8 = System.Byte;
using sqlite3_int64 = System.Int64;

namespace Community.CsharpSqlite
{
    using Op = Engine.VdbeOp;
	using sqlite_int64 = System.Int64;
	using sqlite3_stmt = Engine.Vdbe;
    using sqlite3_value = Engine.Mem;
    using Community.CsharpSqlite.Os;
    using Community.CsharpSqlite.Engine;
    using Community.CsharpSqlite.Metadata;
    
    
	namespace Engine
	{
        using Metadata;
        using Community.CsharpSqlite.Utils;

        public static partial class VdbeExtensions
        {
            ///<summary>
            /// Return the number of columns in the result set for the statement pStmt.
            ///</summary>
            public static int getColumnCount(this sqlite3_stmt pStmt)
            {
                Vdbe pVm = pStmt;
                return pVm != null ? (int)pVm.nResColumn : 0;
            }
        }
        public static class vdbeapi
        {
            ///<summary>
            /// 2004 May 26
            ///
            /// The author disclaims copyright to this source code.  In place of
            /// a legal notice, here is a blessing:
            ///
            ///    May you do good and not evil.
            ///    May you find forgiveness for yourself and forgive others.
            ///    May you share freely, never taking more than you give.
            ///
            ///
            ///
            /// This file contains code use to implement APIs that are part of the
            /// VDBE.
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
            //#include "vdbeInt.h"
#if !SQLITE_OMIT_DEPRECATED
																																														///<summary>
/// Return TRUE (non-zero) of the statement supplied as an argument needs
/// to be recompiled.  A statement needs to be recompiled whenever the
/// execution environment changes in a way that would alter the program
/// that sqlite3_prepare() generates.  For example, if new functions or
/// collating sequences are registered or if an authorizer function is
/// added or changed.
///</summary>
static int sqlite3_expired( sqlite3_stmt pStmt )
{
Vdbe p = (Vdbe)pStmt;
return ( p == null || p.expired ) ? 1 : 0;
}
#endif
            ///<summary>
            /// Check on a Vdbe to make sure it has not been finalized.  Log
            /// an error and return true if it has been finalized (or is otherwise
            /// invalid).  Return false if it is ok.
            ///</summary>
            ///<summary>
            /// The following routine destroys a virtual machine that is created by
            /// the sqlite3_compile() routine. The integer returned is an SQLITE_
            /// success/failure code that describes the result of executing the virtual
            /// machine.
            ///
            /// This routine sets the error code and string returned by
            /// sqlite3_errcode(), sqlite3_errmsg() and sqlite3_errmsg16().
            ///
            ///</summary>
            public static SqlResult sqlite3_finalize(sqlite3_stmt pStmt)
            {
                SqlResult rc;
                if (pStmt == null)
                {
                    ///
                    ///<summary>
                    ///</summary>
                    ///<param name="IMPLEMENTATION">12904 Invoking sqlite3_finalize() on a NULL</param>
                    ///<param name="pointer is a harmless no">op. </param>

                    rc = SqlResult.SQLITE_OK;
                }
                else
                {
                    Vdbe v = pStmt;
                    Connection db = v.db;
#if SQLITE_THREADSAFE
																																																																																												        sqlite3_mutex mutex;
#endif
                    if (v.vdbeSafety())
                        return sqliteinth.SQLITE_MISUSE_BKPT();
#if SQLITE_THREADSAFE
																																																																																												        mutex = v.db.mutex;
#endif
                    Sqlite3.mutex.sqlite3_mutex_enter();
                    rc = vdbeaux.sqlite3VdbeFinalize(ref v);
                    rc = malloc_cs.sqlite3ApiExit(db, rc);
                    Sqlite3.mutex.sqlite3_mutex_leave();
                }
                return rc;
            }

            ///<summary>
            /// Terminate the current execution of an SQL statement and reset it
            /// back to its starting state so that it can be reused. A success code from
            /// the prior execution is returned.
            ///
            /// This routine sets the error code and string returned by
            /// sqlite3_errcode(), sqlite3_errmsg() and sqlite3_errmsg16().
            ///
            ///</summary>
            public static SqlResult sqlite3_reset(sqlite3_stmt pStmt)
            {
                SqlResult rc;
                if (pStmt == null)
                {
                    rc = SqlResult.SQLITE_OK;
                }
                else
                {
                    Vdbe v = (Vdbe)pStmt;
                    v.db.mutex.sqlite3_mutex_enter();
                    rc = v.sqlite3VdbeReset();
                    v.sqlite3VdbeRewind();
                    Debug.Assert((rc & (v.db.errMask)) == rc);
                    rc = malloc_cs.sqlite3ApiExit(v.db, rc);
                    v.db.mutex.sqlite3_mutex_leave();
                }
                return rc;
            }

            ///<summary>
            /// Set all the parameters in the compiled SQL statement to NULL.
            ///
            ///</summary>
            public static SqlResult sqlite3_clear_bindings(sqlite3_stmt pStmt)
            {
                int i;
                var rc = SqlResult.SQLITE_OK;
                Vdbe p = (Vdbe)pStmt;
#if SQLITE_THREADSAFE
																																																																					      sqlite3_mutex mutex = ( (Vdbe)pStmt ).db.mutex;
#endif
                Sqlite3.mutex.sqlite3_mutex_enter();
                for (i = 0; i < p.nVar; i++)
                {
                    p.aVar[i].sqlite3VdbeMemRelease();
                    p.aVar[i].flags = MemFlags.MEM_Null;
                }
                if (p.isPrepareV2 && p.expmask != 0)
                {
                    p.expired = true;
                }
                Sqlite3.mutex.sqlite3_mutex_leave();
                return rc;
            }

            ///<summary>
            /// sqlite3_value_  
            /// The following routines extract information from a Mem or sqlite3_value
            /// structure.
            ///
            ///</summary>
            public static byte[] sqlite3_value_blob(sqlite3_value pVal)
            {
                Mem p = pVal;
                if ((p.flags & (MemFlags.MEM_Blob | MemFlags.MEM_Str)) != 0)
                {
                    p.sqlite3VdbeMemExpandBlob();
                    if (p.zBLOB == null && p.AsString != null)
                    {
                        if (p.AsString.Length == 0)
                            p.zBLOB = malloc_cs.sqlite3Malloc(1);
                        else
                        {
                            p.zBLOB = malloc_cs.sqlite3Malloc(p.AsString.Length);
                            Debug.Assert(p.zBLOB.Length == p.AsString.Length);
                            for (int i = 0; i < p.zBLOB.Length; i++)
                                p.zBLOB[i] = (u8)p.AsString[i];
                        }
                        p.AsString = null;
                    }
                    p.flags = (p.flags & ~MemFlags.MEM_Str);
                    p.flags |= MemFlags.MEM_Blob;
                    return p.CharacterCount > 0 ? p.zBLOB : null;
                }
                else
                {
                    return vdbeapi.sqlite3_value_text(pVal) == null ? null : Encoding.UTF8.GetBytes(vdbeapi.sqlite3_value_text(pVal));
                }
            }

            public static int sqlite3_value_bytes(sqlite3_value pVal)
            {
                return vdbemem_cs.sqlite3ValueBytes(pVal, SqliteEncoding.UTF8);
            }

            public static int sqlite3_value_bytes16(sqlite3_value pVal)
            {
                return vdbemem_cs.sqlite3ValueBytes(pVal, SqliteEncoding.UTF16NATIVE);
            }

            public static double sqlite3_value_double(sqlite3_value pVal)
            {
                return pVal.ToReal();
            }

            public static int sqlite3_value_int(sqlite3_value pVal)
            {
                return (int)pVal.ToInt();
            }

            public static sqlite_int64 sqlite3_value_int64(sqlite3_value pVal)
            {
                return pVal.ToInt();
            }

            public static string sqlite3_value_text(sqlite3_value pVal)
            {
                return vdbemem_cs.sqlite3ValueText(pVal, SqliteEncoding.UTF8);
            }

#if !SQLITE_OMIT_UTF16
																																														public static string vdbeapi.sqlite3_value_text16(sqlite3_value pVal){
return sqlite3ValueText(pVal, SqliteEncoding.UTF16NATIVE);
}
public static string  vdbeapi.sqlite3_value_text16be(sqlite3_value pVal){
return sqlite3ValueText(pVal, SqliteEncoding.UTF16BE);
}
public static string vdbeapi.sqlite3_value_text16le(sqlite3_value pVal){
return sqlite3ValueText(pVal, SqliteEncoding.UTF16LE);
}
#endif
            public static FoundationalType sqlite3_value_type(sqlite3_value pval)
            {
                return pval.type;
            }

#if !SQLITE_OMIT_UTF16
																																														//void sqlite3_result_error16(sqlite3_context pCtx, string z, int n){
//  Debug.Assert( Sqlite3.pCtx.s.db.mutex.sqlite3_mutex_held() );
//  pCtx.isError = SqlResult.SQLITE_ERROR;
//  sqlite3VdbeMemSetStr(pCtx.s, z, n, SqliteEncoding.UTF16NATIVE, SQLITE_TRANSIENT);
//}
#endif
#if !SQLITE_OMIT_UTF16
																																														void sqlite3_result_text16(
sqlite3_context pCtx,
string z,
int n,
dxDel xDel
){
Debug.Assert( Sqlite3.pCtx.s.db.mutex.sqlite3_mutex_held() );
sqlite3VdbeMemSetStr(pCtx.s, z, n, SqliteEncoding.UTF16NATIVE, xDel);
}
void sqlite3_result_text16be(
sqlite3_context pCtx,
string z,
int n,
dxDel xDel
){
Debug.Assert( Sqlite3.pCtx.s.db.mutex.sqlite3_mutex_held() );
sqlite3VdbeMemSetStr(pCtx.s, z, n, SqliteEncoding.UTF16BE, xDel);
}
void sqlite3_result_text16le(
sqlite3_context pCtx,
string z,
int n,
dxDel xDel
){
Debug.Assert( Sqlite3.pCtx.s.db.mutex.sqlite3_mutex_held() );
sqlite3VdbeMemSetStr(pCtx.s, z, n, SqliteEncoding.UTF16LE, xDel);
}
#endif
            ///<summary>
            /// This function is called after a transaction has been committed. It
            /// invokes callbacks registered with sqlite3_wal_hook() as required.
            ///
            ///</summary>
            static SqlResult doWalCallbacks(Connection db)
            {
                var rc = SqlResult.SQLITE_OK;
#if !SQLITE_OMIT_WAL
																																																																					int i;
for(i=0; i<db->nDb; i++){
Btree *pBt = db->aDb[i].pBt;
if( pBt ){
int nEntry = sqlite3PagerWalCallback(sqlite3BtreePager(pBt));
if( db->xWalCallback && nEntry>0 && rc==SqlResult.SQLITE_OK ){
rc = db->xWalCallback(db->pWalArg, db, db->aDb[i].zName, nEntry);
}
}
}
#endif
                return rc;
            }

            ///
            ///<summary>
            ///Execute the statement pStmt, either until a row of data is ready, the
            ///statement is completely executed or an error occurs.
            ///
            ///This routine implements the bulk of the logic behind the sqlite_step()
            ///API.  The only thing omitted is the automatic recompile if a
            ///schema change has occurred.  That detail is handled by the
            ///outer sqlite3_step() wrapper procedure.
            ///
            ///</summary>

            static SqlResult sqlite3Step(Vdbe p)
            {
                Connection db;
                SqlResult rc;
                #region error check
                Debug.Assert(p != null);
                if (p.magic != VdbeMagic.VDBE_MAGIC_RUN)
                {
#if SQLITE_OMIT_AUTORESET
																																																																																												if( p.rc==SQLITE_BUSY || p.rc==SQLITE_LOCKED ){
sqlite3_reset((sqlite3_stmt)p);
}else{
return sqliteinth.SQLITE_MISUSE_BKPT();
}
#else
                    sqlite3_reset((sqlite3_stmt)p);
#endif
                }
                ///
                ///<summary>
                ///Check that malloc() has not failed. If it has, return early. 
                ///</summary>

                db = p.db;
                //if ( db.mallocFailed != 0 )
                //{
                //p->rc = SQLITE_NOMEM;
                //  return SQLITE_NOMEM;
                //}
                if (p.currentOpCodeIndex <= 0 && p.expired)
                {
                    p.result = SqlResult.SQLITE_SCHEMA;
                    rc = SqlResult.SQLITE_ERROR;
                    goto end_of_step;
                }
                if (p.currentOpCodeIndex < 0)
                {
                    ///
                    ///<summary>
                    ///If there are no other statements currently running, then
                    ///reset the interrupt flag.  This prevents a call to sqlite3_interrupt
                    ///from interrupting a statement that has not yet started.
                    ///
                    ///</summary>

                    if (db.activeVdbeCnt == 0)
                    {
                        db.u1.isInterrupted = false;
                    }
                    Debug.Assert(db.writeVdbeCnt > 0 || db.autoCommit == 0 || db.nDeferredCons == 0);
#if !SQLITE_OMIT_TRACE
                    if (db.xProfile != null && 0 == db.init.busy)
                    {
                        os.sqlite3OsCurrentTimeInt64(db.pVfs, ref p.startTime);
                    }
#endif
                    db.activeVdbeCnt++;
                    if (p.readOnly == false)
                        db.writeVdbeCnt++;
                    p.currentOpCodeIndex = 0;
                }
                #endregion
#if !SQLITE_OMIT_EXPLAIN
                if (p.explain != 0)
                {
                    rc = (SqlResult)vdbeaux.sqlite3VdbeList(p);
                }
                else
#endif
                {
                    db.callStackDepth++;
                    rc = (SqlResult)p.sqlite3VdbeExec();
                    db.callStackDepth--;
                }
#if !SQLITE_OMIT_TRACE
                ///
                ///<summary>
                ///Invoke the profile callback if there is one
                ///</summary>

                if (rc != SqlResult.SQLITE_ROW && db.xProfile != null && 0 == db.init.busy && p.zSql != null)
                {
                    sqlite3_int64 iNow = 0;
                    os.sqlite3OsCurrentTimeInt64(db.pVfs, ref iNow);
                    db.xProfile(db.pProfileArg, p.zSql, (iNow - p.startTime) * 1000000);
                }
#endif
                if (rc == SqlResult.SQLITE_DONE)
                {
                    Debug.Assert(p.rc == SqlResult.SQLITE_OK);
                    p.rc = doWalCallbacks(db);
                    if (p.rc != SqlResult.SQLITE_OK)
                    {
                        rc = SqlResult.SQLITE_ERROR;
                    }
                }
                db.errCode = rc;
                if (SqlResult.SQLITE_NOMEM == malloc_cs.sqlite3ApiExit(p.db, p.rc))
                {
                    p.rc = SqlResult.SQLITE_NOMEM;
                }
            end_of_step:
                ///
                ///<summary>
                ///At this point local variable rc holds the value that should be
                ///returned if this statement was compiled using the legacy
                ///sqlite3_prepare() interface. According to the docs, this can only
                ///be one of the values in the first Debug.Assert() below. Variable p.rc
                ///contains the value that would be returned if sqlite3_finalize()
                ///were called on statement p.
                ///
                ///</summary>

                Debug.Assert(rc == SqlResult.SQLITE_ROW || rc == SqlResult.SQLITE_DONE || rc == SqlResult.SQLITE_ERROR || rc == SqlResult.SQLITE_BUSY || rc == SqlResult.SQLITE_MISUSE);
                Debug.Assert(p.result != SqlResult.SQLITE_ROW && p.result != SqlResult.SQLITE_DONE);
                if (p.isPrepareV2 && rc != SqlResult.SQLITE_ROW && rc != SqlResult.SQLITE_DONE)
                {
                    ///
                    ///<summary>
                    ///If this statement was prepared using sqlite3_prepare_v2(), and an
                    ///error has occured, then return the error code in p.rc to the
                    ///caller. Set the error code in the database handle to the same value.
                    ///
                    ///</summary>

                    rc = db.ErrCode = p.result;
                }
                return (rc & db.errMask);
            }

            ///<summary>
            /// The maximum number of times that a statement will try to reparse
            /// itself before giving up and returning SQLITE_SCHEMA.
            ///</summary>
#if !SQLITE_MAX_SCHEMA_RETRY
            //# define SQLITE_MAX_SCHEMA_RETRY 5
            public const int SQLITE_MAX_SCHEMA_RETRY = 5;

#endif
            ///<summary>
            /// This is the top-level implementation of sqlite3_step().  Call
            /// sqlite3Step() to do most of the work.  If a schema error occurs,
            /// call sqlite3Reprepare() and try again.
            ///
            ///</summary>
            public static SqlResult sqlite3_step(sqlite3_stmt pStmt)
            {
                SqlResult rc = SqlResult.SQLITE_OK;
                ///Result from sqlite3Step() 

                SqlResult rc2 = SqlResult.SQLITE_OK;
                ///Result from sqlite3Reprepare() 

                Vdbe v = (Vdbe)pStmt;
                ///the prepared statement 

                int cnt = 0;
                ///Counter to prevent infinite loop of reprepares 

                Connection db;
                ///The database connection 

                if (v.vdbeSafetyNotNull())
                {
                    return (SqlResult)sqliteinth.SQLITE_MISUSE_BKPT();
                }
                db = v.db;
                db.mutex.sqlite3_mutex_enter();
                while ((rc = (SqlResult)sqlite3Step(v)) == SqlResult.SQLITE_SCHEMA && cnt++ < SQLITE_MAX_SCHEMA_RETRY && (rc2 = rc = (SqlResult)Sqlite3.sqlite3Reprepare(v)) == SqlResult.SQLITE_OK)
                {
                    sqlite3_reset(pStmt);
                    v.expired = false;
                }
                if (rc2 != SqlResult.SQLITE_OK && Sqlite3.ALWAYS(v.isPrepareV2) && Sqlite3.ALWAYS(db.pErr != null))
                {
                    ///
                    ///<summary>
                    ///This case occurs after failing to recompile an sql statement.
                    ///The error message from the SQL compiler has already been loaded
                    ///into the database handle. This block copies the error message
                    ///from the database handle into the statement and sets the statement
                    ///program counter to 0 to ensure that when the statement is
                    ///finalized or reset the parser error message is available via
                    ///sqlite3_errmsg() and sqlite3_errcode().
                    ///
                    ///</summary>

                    string zErr = vdbeapi.sqlite3_value_text(db.pErr);
                    db.DbFree(ref v.zErrMsg);
                    //if ( 0 == db.mallocFailed )
                    {
                        v.zErrMsg = zErr;
                        // sqlite3DbStrDup(db, zErr);
                        v.rc = (SqlResult)rc2;
                        //else
                        //{
                        //  v.zErrMsg = "";
                        //  v->rc = rc = SQLITE_NOMEM;
                        //}
                    }
                }
                rc = (SqlResult)malloc_cs.sqlite3ApiExit(db, rc);
                db.mutex.sqlite3_mutex_leave();
                return rc;
            }

            ///<summary>
            /// Extract the user data from a sqlite3_context structure and return a
            /// pointer to it.
            ///
            /// IMPLEMENTATION-OF: R-46798-50301 The vdbeapi.sqlite3_context_db_handle() interface
            /// returns a copy of the pointer to the database connection (the 1st
            /// parameter) of the sqlite3_create_function() and
            /// sqlite3_create_function16() routines that originally registered the
            /// application defined function.
            ///
            ///</summary>
            public static object sqlite3_user_data(sqlite3_context p)
            {
                Debug.Assert(p != null && p.pFunc != null);
                return p.pFunc.pUserData;
            }

            ///<summary>
            /// Extract the user data from a sqlite3_context structure and return a
            /// pointer to it.
            ///
            ///</summary>
            public static Connection sqlite3_context_db_handle(sqlite3_context p)
            {
                Debug.Assert(p != null && p.pFunc != null);
                return p.s.db;
            }

            ///<summary>
            /// The following is the implementation of an SQL function that always
            /// fails with an error message stating that the function is used in the
            /// wrong context.  The sqlite3_overload_function() API might construct
            /// SQL function that use this routine so that the functions will exist
            /// for name resolution but are actually overloaded by the xFindFunction
            /// method of virtual tables.
            ///
            ///</summary>
            public static void sqlite3InvalidFunction(sqlite3_context context, ///
                ///<summary>
                ///The function calling context 
                ///</summary>

            int NotUsed, ///
                ///<summary>
                ///Number of arguments to the function 
                ///</summary>

            sqlite3_value[] NotUsed2///
                ///<summary>
                ///Value of each argument 
                ///</summary>

            )
            {
                string zName = context.pFunc.zName;
                string zErr;
                sqliteinth.UNUSED_PARAMETER2(NotUsed, NotUsed2);
                zErr = io.sqlite3_mprintf("unable to use function %s in the requested context", zName);
                context.sqlite3_result_error(zErr, -1);
                //malloc_cs.sqlite3_free( ref zErr );
            }

            ///<summary>
            /// Allocate or return the aggregate context for a user function.  A new
            /// context is allocated on the first call.  Subsequent calls return the
            /// same context that was returned on prior calls.
            ///
            ///</summary>
            public static Mem sqlite3_aggregate_context(sqlite3_context p, int nByte)
            {
                Mem pMem;
                Debug.Assert(p != null && p.pFunc != null && p.pFunc.xStep != null);
                Debug.Assert(p.s.db.mutex.sqlite3_mutex_held());
                pMem = p.pMem;
                sqliteinth.testcase(nByte < 0);
                if ((pMem.flags & MemFlags.MEM_Agg) == 0)
                {
                    if (nByte <= 0)
                    {
                        pMem.sqlite3VdbeMemReleaseExternal();
                        pMem.flags = 0;
                        pMem.AsString = null;
                    }
                    else
                    {
                        pMem.Grow(nByte, 0);
                        pMem.flags = MemFlags.MEM_Agg;
                        pMem.u.pDef = p.pFunc;
                        if (pMem.AsString != null)
                        {
                            pMem.AsString = null;
                        }
                        pMem._Mem = malloc_cs.sqlite3Malloc(pMem._Mem);
                        pMem._Mem.flags = 0;
                        pMem._Mem.AsString = null;
                    }
                }
                return pMem._Mem;
            }

            ///<summary>
            /// Return the auxillary data pointer, if any, for the iArg'th argument to
            /// the user-function defined by pCtx.
            ///
            ///</summary>
            public static object sqlite3_get_auxdata(sqlite3_context pCtx, int iArg)
            {
                VdbeFunc pVdbeFunc;
                Debug.Assert(pCtx.s.db.mutex.sqlite3_mutex_held());
                pVdbeFunc = pCtx.pVdbeFunc;
                if (null == pVdbeFunc || iArg >= pVdbeFunc.nAux || iArg < 0)
                {
                    return null;
                }
                return pVdbeFunc.apAux[iArg].pAux;
            }

            ///<summary>
            /// Set the auxillary data pointer and delete function, for the iArg'th
            /// argument to the user-function defined by pCtx. Any previous value is
            /// deleted by calling the delete function specified when it was set.
            ///
            ///</summary>
            public static void sqlite3_set_auxdata(sqlite3_context pCtx, int iArg, object pAux//void (*xDelete)(void)
            )
            {
                AuxData pAuxData;
                VdbeFunc pVdbeFunc;
                if (iArg < 0)
                    goto failed;
                Debug.Assert(pCtx.s.db.mutex.sqlite3_mutex_held());
                pVdbeFunc = pCtx.pVdbeFunc;
                if (null == pVdbeFunc || pVdbeFunc.nAux <= iArg)
                {
                    int nAux = (pVdbeFunc != null ? pVdbeFunc.nAux : 0);
                    int nMalloc = iArg;
                    ;
                    //VdbeFunc+ sizeof(struct AuxData)*iArg;
                    if (pVdbeFunc == null)
                    {
                        //pVdbeFunc = (VdbeFunc)sqlite3DbRealloc( pCtx.s.db, pVdbeFunc, nMalloc );
                        pVdbeFunc = new VdbeFunc();
                        if (null == pVdbeFunc)
                        {
                            goto failed;
                        }
                        pCtx.pVdbeFunc = pVdbeFunc;
                    }
                    pVdbeFunc.apAux[nAux] = new AuxData();
                    //memset(pVdbeFunc.apAux[nAux], 0, sizeof(struct AuxData)*(iArg+1-nAux));
                    pVdbeFunc.nAux = iArg + 1;
                    pVdbeFunc.pFunc = pCtx.pFunc;
                }
                pAuxData = pVdbeFunc.apAux[iArg];
                if (pAuxData.pAux != null && pAuxData.pAux is IDisposable)
                {
                    (pAuxData.pAux as IDisposable).Dispose();
                }
                pAuxData.pAux = pAux;
                return;
            failed:
                if (pAux != null && pAux is IDisposable)
                {
                    (pAux as IDisposable).Dispose();
                }
            }

#if !SQLITE_OMIT_DEPRECATED
																																														///<summary>
/// Return the number of times the Step function of a aggregate has been
/// called.
///
/// This function is deprecated.  Do not use it for new code.  It is
/// provide only to avoid breaking legacy code.  New aggregate function
/// implementations should keep their own counts within their aggregate
/// context.
///</summary>
static int sqlite3_aggregate_count( sqlite3_context p )
{
Debug.Assert( p != null && p.pMem != null && p.pFunc != null && p.pFunc.xStep != null );
return p.pMem.n;
}
#endif
            

            ///<summary>
            /// Return the number of values available from the current row of the
            /// currently executing statement pStmt.
            ///
            ///</summary>
            public static int sqlite3_data_count(sqlite3_stmt pStmt)
            {
                Vdbe pVm = pStmt;
                if (pVm == null || pVm.pResultSet == null)
                    return 0;
                return pVm.nResColumn;
            }

            ///<summary>
            /// Check to see if column iCol of the given statement is valid.  If
            /// it is, return a pointer to the Mem for the value of that column.
            /// If iCol is not valid, return a pointer to a Mem which has a value
            /// of NULL.
            ///
            ///</summary>
            static Mem columnMem(sqlite3_stmt pStmt, int i)
            {
                Vdbe pVm;
                Mem pOut;
                pVm = (Vdbe)pStmt;
                if (pVm != null && pVm.pResultSet != null && i < pVm.nResColumn && i >= 0)
                {
                    pVm.db.mutex.sqlite3_mutex_enter();
                    pOut = pVm.pResultSet[i];
                }
                else
                {
                    ///
                    ///<summary>
                    ///If the value passed as the second argument is out of range, return
                    ///a pointer to the following public static Mem object which contains the
                    ///value SQL NULL. Even though the Mem structure contains an element
                    ///of type i64, on certain architecture (x86) with certain compiler
                    ///</summary>
                    ///<param name="switches (">byte boundary</param>
                    ///<param name="instead of an 8">byte one. This all works fine, except that when</param>
                    ///<param name="running with SQLITE_DEBUG defined the SQLite code sometimes Debug.Assert()s">running with SQLITE_DEBUG defined the SQLite code sometimes Debug.Assert()s</param>
                    ///<param name="that a Mem structure is located on an 8">byte boundary. To prevent</param>
                    ///<param name="this Debug.Assert() from failing, when building with SQLITE_DEBUG defined">this Debug.Assert() from failing, when building with SQLITE_DEBUG defined</param>
                    ///<param name="using gcc, force nullMem to be 8">byte aligned using the magical</param>
                    ///<param name="__attribute__((aligned(8))) macro.  ">__attribute__((aligned(8))) macro.  </param>

                    //    static const Mem nullMem 
                    //#if defined(SQLITE_DEBUG) && defined(__GNUC__)
                    //      __attribute__((aligned(8))) 
                    //#endif
                    //      = {0, "", (double)0, {0}, 0, MEM.MEM_Null, SQLITE_NULL, 0,
                    //#if SQLITE_DEBUG
                    //         0, 0,  /* pScopyFrom, pFiller */
                    //#endif
                    //         0, 0 };
                    Mem nullMem = new Mem(null, "", (double)0, 0, 0, MemFlags.MEM_Null, FoundationalType.SQLITE_NULL, 0
#if SQLITE_DEBUG
																																																																																												         , null, null  /* pScopyFrom, pFiller */
#endif
);
                    if (pVm != null && Sqlite3.ALWAYS(pVm.db != null))
                    {
                        pVm.db.mutex.sqlite3_mutex_enter();
                        utilc.sqlite3Error(pVm.db, SqlResult.SQLITE_RANGE, 0);
                    }
                    pOut = nullMem;
                }
                return pOut;
            }

            ///<summary>
            /// This function is called after invoking an sqlite3_value_XXX function on a
            /// column value (i.e. a value returned by evaluating an SQL expression in the
            /// select list of a SELECT statement) that may cause a malloc() failure. If
            /// malloc() has failed, the threads mallocFailed flag is cleared and the result
            /// code of statement pStmt set to SQLITE_NOMEM.
            ///
            /// Specifically, this is called from within:
            ///
            ///     sqlite3_column_int()
            ///     sqlite3_column_int64()
            ///     sqlite3_column_text()
            ///     sqlite3_column_text16()
            ///     sqlite3_column_real()
            ///     sqlite3_column_bytes()
            ///     sqlite3_column_bytes16()
            ///     sqlite3_column_blob()
            ///
            ///</summary>
            static void columnMallocFailure(sqlite3_stmt pStmt)
            {
                ///
                ///<summary>
                ///If malloc() failed during an encoding conversion within an
                ///sqlite3_column_XXX API, then set the return code of the statement to
                ///SQLITE_NOMEM. The next call to _step() (if any) will return SqlResult.SQLITE_ERROR
                ///and _finalize() will return NOMEM.
                ///
                ///</summary>

                Vdbe p = pStmt;
                if (p != null)
                {
                    p.rc = malloc_cs.sqlite3ApiExit(p.db, p.rc);
                    p.db.mutex.sqlite3_mutex_leave();
                }
            }

            ///<summary>
            /// sqlite3_column_  
            /// The following routines are used to access elements of the current row
            /// in the result set.
            ///
            ///</summary>
            public static byte[] sqlite3_column_blob(sqlite3_stmt pStmt, int i)
            {
                byte[] val;
                val = sqlite3_value_blob(columnMem(pStmt, i));
                ///
                ///<summary>
                ///Even though there is no encoding conversion, value_blob() might
                ///need to call malloc() to expand the result of a zeroblob()
                ///expression.
                ///
                ///</summary>

                columnMallocFailure(pStmt);
                return val;
            }

            public static int sqlite3_column_bytes(sqlite3_stmt pStmt, int i)
            {
                int val = vdbeapi.sqlite3_value_bytes(columnMem(pStmt, i));
                columnMallocFailure(pStmt);
                return val;
            }

            public static int sqlite3_column_bytes16(sqlite3_stmt pStmt, int i)
            {
                int val = vdbeapi.sqlite3_value_bytes16(columnMem(pStmt, i));
                columnMallocFailure(pStmt);
                return val;
            }

            public static double sqlite3_column_double(sqlite3_stmt pStmt, int i)
            {
                double val = sqlite3_value_double(columnMem(pStmt, i));
                columnMallocFailure(pStmt);
                return val;
            }

            public static int sqlite3_column_int(sqlite3_stmt pStmt, int i)
            {
                int val = sqlite3_value_int(columnMem(pStmt, i));
                columnMallocFailure(pStmt);
                return val;
            }

            public static sqlite_int64 sqlite3_column_int64(this sqlite3_stmt pStmt, int i)
            {
                sqlite_int64 val = sqlite3_value_int64(columnMem(pStmt, i));
                columnMallocFailure(pStmt);
                return val;
            }

            public static string get_column_text(this sqlite3_stmt pStmt, int i)
            {
                string val = vdbeapi.sqlite3_value_text(columnMem(pStmt, i));
                columnMallocFailure(pStmt);
                return val;
            }

            public static sqlite3_value sqlite3_column_value(this sqlite3_stmt pStmt, int i)
            {
                Mem pOut = columnMem(pStmt, i);
                if ((pOut.flags & MemFlags.MEM_Static) != 0)
                {
                    pOut.flags = (pOut.flags & ~MemFlags.MEM_Static);
                    pOut.flags |= MemFlags.MEM_Ephem;
                }
                columnMallocFailure(pStmt);
                return (sqlite3_value)pOut;
            }

#if !SQLITE_OMIT_UTF16
																																														//const void *sqlite3_column_text16(sqlite3_stmt pStmt, int i){
//  const void *val = vdbeapi.sqlite3_value_text16( columnMem(pStmt,i) );
//  columnMallocFailure(pStmt);
//  return val;
//}
#endif
            public static FoundationalType get_column_type(this sqlite3_stmt pStmt, int i)
            {
                FoundationalType iType = vdbeapi.sqlite3_value_type(columnMem(pStmt, i));
                columnMallocFailure(pStmt);
                return iType;
            }

            ///
            ///<summary>
            ///The following function is experimental and subject to change or
            ///removal 
            ///</summary>

            ///<summary>
            ///int sqlite3_column_numeric_type(sqlite3_stmt pStmt, int i){
            ///  return sqlite3_value_numeric_type( columnMem(pStmt,i) );
            ///}
            ///
            ///</summary>
            ///<summary>
            /// Convert the N-th element of pStmt.pColName[] into a string using
            /// xFunc() then return that string.  If N is out of range, return 0.
            ///
            /// There are up to 5 names for each column.  useType determines which
            /// name is returned.  Here are the names:
            ///
            ///    0      The column name as it should be displayed for output
            ///    1      The datatype name for the column
            ///    2      The name of the database that the column derives from
            ///    3      The name of the table that the column derives from
            ///    4      The name of the table column that the result column derives from
            ///
            /// If the result is not a simple column reference (if it is an expression
            /// or a constant) then useTypes 2, 3, and 4 return NULL.
            ///
            ///</summary>
            public static string columnName(sqlite3_stmt pStmt, int N, dxColname xFunc, ColName useType)
            {
                string ret = null;
                Vdbe p = pStmt;
                int n;
                Connection db = p.db;
                Debug.Assert(db != null);
                n = pStmt.getColumnCount();
                if (N < n && N >= 0)
                {
                    N += (int)useType * n;
                    db.mutex.sqlite3_mutex_enter();
                    //Debug.Assert( db.mallocFailed == 0 );
                    ret = xFunc(p.aColName[N]);
                    ///
                    ///<summary>
                    ///A malloc may have failed inside of the xFunc() call. If this
                    ///is the case, clear the mallocFailed flag and return NULL.
                    ///
                    ///</summary>

                    //if ( db.mallocFailed != 0 )
                    //{
                    //  //db.mallocFailed = 0;
                    //  ret = null;
                    //}
                    db.mutex.sqlite3_mutex_leave();
                }
                return ret;
            }

            ///<summary>
            /// Return the name of the Nth column of the result set returned by SQL
            /// statement pStmt.
            ///
            ///</summary>
            public static string get_column_name(this sqlite3_stmt pStmt, int N)
            {
                return columnName(pStmt, N, vdbeapi.sqlite3_value_text, ColName.NAME);
            }

#if !SQLITE_OMIT_UTF16
																																														public static string vdbeapi.sqlite3_column_name16(sqlite3_stmt pStmt, int N){
return columnName(
pStmt, N,  vdbeapi.sqlite3_value_text16, COLNAME_NAME);
}
#endif
            ///<summary>
            /// Constraint:  If you have ENABLE_COLUMN_METADATA then you must
            /// not define OMIT_DECLTYPE.
            ///</summary>
#if SQLITE_OMIT_DECLTYPE && SQLITE_ENABLE_COLUMN_METADATA
#error "Must not define both SQLITE_OMIT_DECLTYPE and SQLITE_ENABLE_COLUMN_METADATA"
#endif
#if !SQLITE_OMIT_DECLTYPE
            ///
            ///<summary>
            ///Return the column declaration type (if applicable) of the 'i'th column
            ///of the result set of SQL statement pStmt.
            ///</summary>

            public static string sqlite3_column_decltype(sqlite3_stmt pStmt, int N)
            {
                return columnName(pStmt, N, vdbeapi.sqlite3_value_text, ColName.DECLTYPE);
            }

#if !SQLITE_OMIT_UTF16
																																														//const void *sqlite3_column_decltype16(sqlite3_stmt pStmt, int N){
//  return columnName(
//      pStmt, N, (const void*()(Mem))vdbeapi.sqlite3_value_text16, COLNAME_DECLTYPE);
//}
#endif
#endif
#if SQLITE_ENABLE_COLUMN_METADATA
																																														
/*
** Return the name of the database from which a result column derives.
** NULL is returned if the result column is an expression or constant or
** anything else which is not an unabiguous reference to a database column.
*/
    public static string sqlite3_column_database_name( sqlite3_stmt pStmt, int N )
    {
      return columnName(
      pStmt, N, vdbeapi.sqlite3_value_text, COLNAME_DATABASE );
    }
#if !SQLITE_OMIT_UTF16
																																														const void *sqlite3_column_database_name16(sqlite3_stmt pStmt, int N){
return columnName(
pStmt, N, (const void*()(Mem))vdbeapi.sqlite3_value_text16, COLNAME_DATABASE);
}
#endif
																																														
/*
** Return the name of the table from which a result column derives.
** NULL is returned if the result column is an expression or constant or
** anything else which is not an unabiguous reference to a database column.
*/
    public static string sqlite3_column_table_name( sqlite3_stmt pStmt, int N )
    {
      return columnName(
      pStmt, N, vdbeapi.sqlite3_value_text, COLNAME_TABLE );
    }
#if !SQLITE_OMIT_UTF16
																																														const void *sqlite3_column_table_name16(sqlite3_stmt pStmt, int N){
return columnName(
pStmt, N, (const void*()(Mem))vdbeapi.sqlite3_value_text16, COLNAME_TABLE);
}
#endif
																																														
/*
** Return the name of the table column from which a result column derives.
** NULL is returned if the result column is an expression or constant or
** anything else which is not an unabiguous reference to a database column.
*/
    public static string sqlite3_column_origin_name( sqlite3_stmt pStmt, int N )
    {
      return columnName(
      pStmt, N, vdbeapi.sqlite3_value_text, COLNAME_COLUMN );
    }
#if !SQLITE_OMIT_UTF16
																																														const void *sqlite3_column_origin_name16(sqlite3_stmt pStmt, int N){
return columnName(
pStmt, N, (const void*()(Mem))vdbeapi.sqlite3_value_text16, COLNAME_COLUMN);
}
#endif
#endif
            ///<summary>
            /// sqlite3_bind_  
            ///
            /// Routines used to attach values to wildcards in a compiled SQL statement.
            ///</summary>
            ///<summary>
            /// Unbind the value bound to variable i in virtual machine p. This is the
            /// the same as binding a NULL value to the column. If the "i" parameter is
            /// out of range, then SQLITE_RANGE is returned. Othewise SqlResult.SQLITE_OK.
            ///
            /// A successful evaluation of this routine acquires the mutex on p.
            /// the mutex is released if any kind of error occurs.
            ///
            /// The error code stored in database p.db is overwritten with the return
            /// value in any case.
            ///
            ///</summary>
            public static SqlResult vdbeUnbind(Vdbe p, int i)
            {
                Mem pVar;
                if (p.vdbeSafetyNotNull())
                {
                    return sqliteinth.SQLITE_MISUSE_BKPT();
                }
                p.db.mutex.sqlite3_mutex_enter();
                if (p.magic != VdbeMagic.VDBE_MAGIC_RUN || p.currentOpCodeIndex >= 0)
                {
                    utilc.sqlite3Error(p.db, SqlResult.SQLITE_MISUSE, 0);
                    p.db.mutex.sqlite3_mutex_leave();
                    io.sqlite3_log(SqlResult.SQLITE_MISUSE, "bind on a busy prepared statement: [%s]", p.zSql);
                    return sqliteinth.SQLITE_MISUSE_BKPT();
                }
                if (i < 1 || i > p.nVar)
                {
                    utilc.sqlite3Error(p.db, SqlResult.SQLITE_RANGE, 0);
                    p.db.mutex.sqlite3_mutex_leave();
                    return SqlResult.SQLITE_RANGE;
                }
                i--;
                pVar = p.aVar[i];
                pVar.sqlite3VdbeMemRelease();
                pVar.flags = MemFlags.MEM_Null;
                utilc.sqlite3Error(p.db, SqlResult.SQLITE_OK, 0);
                ///
                ///<summary>
                ///If the bit corresponding to this variable in Vdbe.expmask is set, then 
                ///binding a new value to this variable invalidates the current query plan.
                ///
                ///</summary>
                ///<param name="IMPLEMENTATION">37595 If the specific value bound to host</param>
                ///<param name="parameter in the WHERE clause might influence the choice of query plan">parameter in the WHERE clause might influence the choice of query plan</param>
                ///<param name="for a statement, then the statement will be automatically recompiled,">for a statement, then the statement will be automatically recompiled,</param>
                ///<param name="as if there had been a schema change, on the first sqlite3_step() call">as if there had been a schema change, on the first sqlite3_step() call</param>
                ///<param name="following any change to the bindings of that parameter.">following any change to the bindings of that parameter.</param>
                ///<param name=""></param>

                if (p.isPrepareV2 && ((i < 32 && p.expmask != 0 & ((u32)1 << i) != 0) || p.expmask == 0xffffffff))
                {
                    p.expired = true;
                }
                return SqlResult.SQLITE_OK;
            }

            ///<summary>
            /// Bind a text or BLOB value.
            ///
            ///</summary>
            static SqlResult bindBlob(sqlite3_stmt pStmt, ///
                ///<summary>
                ///The statement to bind against 
                ///</summary>

            int i, ///
                ///<summary>
                ///Index of the parameter to bind 
                ///</summary>

            byte[] zData, ///
                ///<summary>
                ///Pointer to the data to be bound 
                ///</summary>

            int nData, ///
                ///<summary>
                ///Number of bytes of data to be bound 
                ///</summary>

            dxDel xDel, ///
                ///<summary>
                ///Destructor for the data 
                ///</summary>

            SqliteEncoding encoding///
                ///<summary>
                ///Encoding for the data 
                ///</summary>

            )
            {
                Vdbe p = pStmt;
                Mem pVar;
                SqlResult rc;
                rc = vdbeUnbind(p, i);
                if (rc == SqlResult.SQLITE_OK)
                {
                    if (zData != null)
                    {
                        pVar = p.aVar[i - 1];
                        rc = pVar.MakeBlob(zData, nData, encoding, xDel);
                        if (rc == SqlResult.SQLITE_OK && encoding != 0)
                        {
                            rc = vdbemem_cs.sqlite3VdbeChangeEncoding(pVar, sqliteinth.ENC(p.db));
                        }
                        utilc.sqlite3Error(p.db, rc, 0);
                        rc = malloc_cs.sqlite3ApiExit(p.db, rc);
                    }
                    p.db.mutex.sqlite3_mutex_leave();
                }
                return rc;
            }

            ///<summary>
            /// Bind a text value.
            ///
            ///</summary>
            public static SqlResult bindText(sqlite3_stmt pStmt, ///
                ///<summary>
                ///The statement to bind against 
                ///</summary>

            int i, ///
                ///<summary>
                ///Index of the parameter to bind 
                ///</summary>

            string zData, ///
                ///<summary>
                ///Pointer to the data to be bound 
                ///</summary>

            int nData, ///
                ///<summary>
                ///Number of bytes of data to be bound 
                ///</summary>

            dxDel xDel, ///
                ///<summary>
                ///Destructor for the data 
                ///</summary>

            SqliteEncoding encoding///
                ///<summary>
                ///Encoding for the data 
                ///</summary>

            )
            {
                Vdbe p = pStmt;
                Mem pVar;
                SqlResult rc;
                rc = vdbeUnbind(p, i);
                if (rc == SqlResult.SQLITE_OK)
                {
                    if (zData != null)
                    {
                        pVar = p.aVar[i - 1];
                        rc = pVar.sqlite3VdbeMemSetStr(zData, nData, encoding, xDel);
                        if (rc == SqlResult.SQLITE_OK && encoding != 0)
                        {
                            rc = vdbemem_cs.sqlite3VdbeChangeEncoding(pVar, sqliteinth.ENC(p.db));
                        }
                        utilc.sqlite3Error(p.db, rc, 0);
                        rc = malloc_cs.sqlite3ApiExit(p.db, rc);
                    }
                    p.db.mutex.sqlite3_mutex_leave();
                }
                else
                    if (xDel != Sqlite3.SQLITE_STATIC && xDel != Sqlite3.SQLITE_TRANSIENT)
                    {
                        xDel(ref zData);
                    }
                return rc;
            }

            public static SqlResult sqlite3_bind_double(sqlite3_stmt pStmt, int i, double rValue)
            {
                SqlResult rc;
                Vdbe p = pStmt;
                rc = vdbeUnbind(p, i);
                if (rc == SqlResult.SQLITE_OK)
                {
                    p.aVar[i - 1].Set( rValue);
                    p.db.mutex.sqlite3_mutex_leave();
                }
                return rc;
            }

            public static SqlResult sqlite3_bind_int(sqlite3_stmt p, int i, int iValue)
            {
                return sqlite3_bind_int64(p, i, (i64)iValue);
            }

            public static SqlResult sqlite3_bind_int64(sqlite3_stmt pStmt, int i, sqlite_int64 iValue)
            {
                SqlResult rc;
                Vdbe p = pStmt;
                rc = vdbeUnbind(p, i);
                if (rc == SqlResult.SQLITE_OK)
                {
                    p.aVar[i - 1].Set( iValue);
                    p.db.mutex.sqlite3_mutex_leave();
                }
                return rc;
            }

            public static SqlResult sqlite3_bind_null(sqlite3_stmt pStmt, int i)
            {
                SqlResult rc;
                Vdbe p = (Vdbe)pStmt;
                rc = vdbeUnbind(p, i);
                if (rc == SqlResult.SQLITE_OK)
                {
                    p.db.mutex.sqlite3_mutex_leave();
                }
                return rc;
            }

            public static SqlResult sqlite3_bind_text(sqlite3_stmt pStmt, int i, string zData, int nData, dxDel xDel)
            {
                return bindText(pStmt, i, zData, nData, xDel, SqliteEncoding.UTF8);
            }

            public static SqlResult sqlite3_bind_blob(sqlite3_stmt pStmt, int i, byte[] zData, int nData, dxDel xDel)
            {
                return bindBlob(pStmt, i, zData, nData >= 0 ? nData : zData.Length, xDel, 0);
            }

#if !SQLITE_OMIT_UTF16
																																														static int sqlite3_bind_text16(
sqlite3_stmt pStmt,
int i,
string zData,
int nData,
dxDel xDel
){
return bindText(pStmt, i, zData, nData, xDel, SqliteEncoding.UTF16NATIVE);
}
#endif
            public static SqlResult sqlite3_bind_value(sqlite3_stmt pStmt, int i, sqlite3_value pValue)
            {
                SqlResult rc;
                switch (pValue.type)
                {
                    case FoundationalType.SQLITE_INTEGER:
                        {
                            rc = sqlite3_bind_int64(pStmt, i, pValue.u.AsInteger);
                            break;
                        }
                    case FoundationalType.SQLITE_FLOAT:
                        {
                            rc = sqlite3_bind_double(pStmt, i, pValue.AsReal);
                            break;
                        }
                    case FoundationalType.SQLITE_BLOB:
                        {
                            if ((pValue.flags & MemFlags.MEM_Zero) != 0)
                            {
                                rc = sqlite3_bind_zeroblob(pStmt, i, pValue.u.nZero);
                            }
                            else
                            {
                                rc = sqlite3_bind_blob(pStmt, i, pValue.zBLOB, pValue.CharacterCount, Sqlite3.SQLITE_TRANSIENT);
                            }
                            break;
                        }
                    case FoundationalType.SQLITE_TEXT:
                        {
                            rc = bindText(pStmt, i, pValue.AsString, pValue.CharacterCount, Sqlite3.SQLITE_TRANSIENT, pValue.enc);
                            break;
                        }
                    default:
                        {
                            rc = sqlite3_bind_null(pStmt, i);
                            break;
                        }
                }
                return rc;
            }

            public static SqlResult sqlite3_bind_zeroblob(sqlite3_stmt pStmt, int i, int n)
            {
                SqlResult rc;
                Vdbe p = pStmt;
                rc = vdbeUnbind(p, i);
                if (rc == SqlResult.SQLITE_OK)
                {
                    p.aVar[i - 1].sqlite3VdbeMemSetZeroBlob( n);
                    p.db.mutex.sqlite3_mutex_leave();
                }
                return rc;
            }

            ///<summary>
            /// Return the number of wildcards that can be potentially bound to.
            /// This routine is added to support DBD::SQLite.
            ///
            ///</summary>
            public static int sqlite3_bind_parameter_count(sqlite3_stmt pStmt)
            {
                Vdbe p = (Vdbe)pStmt;
                return (p != null) ? (int)p.nVar : 0;
            }

            ///<summary>
            /// Return the name of a wildcard parameter.  Return NULL if the index
            /// is out of range or if the wildcard is unnamed.
            ///
            /// The result is always UTF-8.
            ///
            ///</summary>
            public static string sqlite3_bind_parameter_name(sqlite3_stmt pStmt, int i)
            {
                Vdbe p = (Vdbe)pStmt;
                if (p == null || i < 1 || i > p.nzVar)
                {
                    return "";
                }
                return p.azVar[i - 1];
            }

            ///<summary>
            /// Given a wildcard parameter name, return the index of the variable
            /// with that name.  If there is no variable with the given name,
            /// return 0.
            ///
            ///</summary>
            public static int sqlite3VdbeParameterIndex(Vdbe p, string zName, int nName)
            {
                int i;
                if (p == null)
                {
                    return 0;
                }
                if (zName != null && zName != "")
                {
                    for (i = 0; i < p.nzVar; i++)
                    {
                        string z = p.azVar[i];
                        if (z != null && z == zName)//&& memcmp(z,zName,nName)==0 && z[nName]==0)
                        {
                            return i + 1;
                        }
                    }
                }
                return 0;
            }

            public static int sqlite3_bind_parameter_index(sqlite3_stmt pStmt, string zName)
            {
                return vdbeapi.sqlite3VdbeParameterIndex((Vdbe)pStmt, zName, StringExtensions.Strlen30(zName));
            }

            ///<summary>
            /// Transfer all bindings from the first statement over to the second.
            ///
            ///</summary>
            public static SqlResult sqlite3TransferBindings(sqlite3_stmt pFromStmt, sqlite3_stmt pToStmt)
            {
                Vdbe pFrom = (Vdbe)pFromStmt;
                Vdbe pTo = (Vdbe)pToStmt;
                int i;
                Debug.Assert(pTo.db == pFrom.db);
                Debug.Assert(pTo.nVar == pFrom.nVar);
                pTo.db.mutex.sqlite3_mutex_enter();
                for (i = 0; i < pFrom.nVar; i++)
                {
                    vdbemem_cs.sqlite3VdbeMemMove(pTo.aVar[i], pFrom.aVar[i]);
                }
                pTo.db.mutex.sqlite3_mutex_leave();
                return SqlResult.SQLITE_OK;
            }

#if !SQLITE_OMIT_DEPRECATED
																																														///<summary>
/// Deprecated external interface.  Internal/core SQLite code
/// should call sqlite3TransferBindings.
///
/// Is is misuse to call this routine with statements from different
/// database connections.  But as this is a deprecated interface, we
/// will not bother to check for that condition.
///
/// If the two statements contain a different number of bindings, then
/// an SqlResult.SQLITE_ERROR is returned.  Nothing else can go wrong, so otherwise
/// SqlResult.SQLITE_OK is returned.
///</summary>
static int sqlite3_transfer_bindings( sqlite3_stmt pFromStmt, sqlite3_stmt pToStmt )
{
Vdbe pFrom = (Vdbe)pFromStmt;
Vdbe pTo = (Vdbe)pToStmt;
if ( pFrom.nVar != pTo.nVar )
{
return SqlResult.SQLITE_ERROR;
}
if( pTo.isPrepareV2 && pTo.expmask ){
pTo.expired = 1;
}
if( pFrom.isPrepareV2 && pFrom.expmask ){
pFrom.expired = 1;
}
return sqlite3TransferBindings( pFromStmt, pToStmt );
}
#endif
            ///<summary>
            /// Return the sqlite3* database handle to which the prepared statement given
            /// in the argument belongs.  This is the same database handle that was
            /// the first argument to the sqlite3_prepare() that was used to create
            /// the statement in the first place.
            ///</summary>
            public static Connection sqlite3_db_handle(sqlite3_stmt pStmt)
            {
                return pStmt != null ? ((Vdbe)pStmt).db : null;
            }

            ///<summary>
            /// Return true if the prepared statement is guaranteed to not modify the
            /// database.
            ///
            ///</summary>
            static bool sqlite3_stmt_readonly(sqlite3_stmt pStmt)
            {
                return pStmt != null ? ((Vdbe)pStmt).readOnly : true;
            }

            ///<summary>
            /// Return a pointer to the next prepared statement after pStmt associated
            /// with database connection pDb.  If pStmt is NULL, return the first
            /// prepared statement for the database connection.  Return NULL if there
            /// are no more.
            ///
            ///</summary>
            public static sqlite3_stmt sqlite3_next_stmt(Connection pDb, sqlite3_stmt pStmt)
            {
                sqlite3_stmt pNext;
                pDb.mutex.sqlite3_mutex_enter();
                if (pStmt == null)
                {
                    pNext = (sqlite3_stmt)pDb.pVdbe;
                }
                else
                {
                    pNext = (sqlite3_stmt)((Vdbe)pStmt).pNext;
                }
                pDb.mutex.sqlite3_mutex_leave();
                return pNext;
            }

            ///
            ///<summary>
            ///Return the value of a status counter for a prepared statement
            ///
            ///</summary>

            public static int sqlite3_stmt_status(sqlite3_stmt pStmt, int op, int resetFlag)
            {
                Vdbe pVdbe = (Vdbe)pStmt;
                int v = pVdbe.aCounter[op - 1];
                if (resetFlag != 0)
                    pVdbe.aCounter[op - 1] = 0;
                return v;
            }
        }
	}
}
