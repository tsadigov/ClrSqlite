using System;
using System.Diagnostics;
using System.Text;
using u8 = System.Byte;
using u16 = System.UInt16;
using u32 = System.UInt32;
using sqlite3_int64 = System.Int64;
namespace Community.CsharpSqlite
{
    using sqlite3_stmt = Engine.Vdbe;
    using Community.CsharpSqlite.builder;
    using Community.CsharpSqlite.Metadata;
    using Community.CsharpSqlite.Os;
    using Community.CsharpSqlite.Engine;
    using Community.CsharpSqlite.tree;
    using Community.CsharpSqlite.Utils;
    using System.Collections.Generic;
    using System.Linq;
    using Ast;
    public static class prepare
    {
        ///<summary>
        /// 2005 May 25
        ///
        /// The author disclaims copyright to this source code.  In place of
        /// a legal notice, here is a blessing:
        ///
        ///    May you do good and not evil.
        ///    May you find forgiveness for yourself and forgive others.
        ///    May you share freely, never taking more than you give.
        ///
        ///
        /// This file contains the implementation of the sqlite3_prepare()
        /// interface, and routines that contribute to loading the database schema
        /// from disk.
        ///
        ///  Included in SQLite3 port to C#-SQLite;  2008 Noah B Hart
        ///  C#-SQLite is an independent reimplementation of the SQLite software library
        ///
        ///  SQLITE_SOURCE_ID: 2011-05-19 13:26:54 ed1da510a239ea767a01dc332b667119fa3c908e
        ///
        ///
        ///
        ///</summary>
        //#include "sqliteInt.h"




        ///<summary>
        /// Attempt to read the database schema and initialize internal
        /// data structures for a single database file.  The index of the
        /// database file is given by iDb.  iDb==0 is used for the main
        /// database.  iDb==1 should never be used.  iDb>=2 is used for
        /// auxiliary databases.  Return one of the SQLITE_ error codes to
        /// indicate success or failure.
        ///
        ///</summary>
        public static SqlResult InitialiseSingleDatabase(this Connection db, int iDb, ref string pzErrMsg)
        {
            SqlResult rc;

            string zMasterSchema = String.Empty;
            string zMasterName;
            int openedTransaction = 0;

            Debug.Assert(iDb >= 0 && iDb < db.BackendCount);
            Debug.Assert(db.Backends[iDb].pSchema != null);
            Debug.Assert(db.mutex.sqlite3_mutex_held());
            Debug.Assert(iDb == 1 || db.Backends[iDb].BTree.sqlite3BtreeHoldsMutex());

            #region create schema for master table
            zMasterName = sqliteinth.SCHEMA_TABLE(iDb);
            ///Construct the schema tables.  
            string[] azArg = new string[] {
                zMasterName,
                "1",
                sqliteinth.MasterSchemaTableCreateCommand(iDb),
                ""
            };

            InitData initData = new InitData()
            {
                db = db,
                iDb = iDb,
                rc = SqlResult.SQLITE_OK,
                pzErrMsg = pzErrMsg
            };

            SchemaExtensions.InitTableDefinitionCallback(initData, 3, azArg, null);

            if (initData.rc != 0)
            {
                rc = initData.rc;
                goto error_out;
            }
            var pTab = TableBuilder.sqlite3FindTable(db, db.Backends[iDb].Name, zMasterName);
            if (Sqlite3.ALWAYS(pTab))
            {
                pTab.tabFlags |= TableFlags.TF_Readonly;
            }
            #endregion


            ///Create a cursor to hold the database open
            var pDb = db.Backends[iDb];
            if (pDb.BTree == null)
            {
                if (sqliteinth.OMIT_TEMPDB == 0 && Sqlite3.ALWAYS(iDb == 1))
                {
                    db.DbSetProperty(1, sqliteinth.DB_SchemaLoaded);
                }
                return SqlResult.SQLITE_OK;
            }
            ///If there is not already a read">write) transaction opened
            ///on the b">tree database, open one now. If a transaction is opened, it 
            ///will be closed before this function returns.
            //pDb.BTree.Enter();
            using (pDb.BTree.scope())
            {
                if (!pDb.BTree.sqlite3BtreeIsInReadTrans())
                {
                    rc = pDb.BTree.sqlite3BtreeBeginTrans(0);
                    if (rc != SqlResult.SQLITE_OK)
                    {
#if SQLITE_OMIT_WAL
                        if (pDb.BTree.pBt.pSchema.file_format == 2)
                            malloc_cs.sqlite3SetString(ref pzErrMsg, db, "%s (wal format detected)", rc.sqlite3ErrStr());
                        else
                            malloc_cs.sqlite3SetString(ref pzErrMsg, db, "%s", rc.sqlite3ErrStr());
#else
																																																																																																																								          malloc_cs.sqlite3SetString( ref pzErrMsg, db, "%s", sqlite3ErrStr( rc ) );
#endif
                        goto initone_error_out;
                    }
                    else
                        openedTransaction = 1;
                }

                #region setup btree meta properties
                ///Note: The #defined SQLITE_UTF* symbols in sqliteInt.h correspond to
                ///the possible values of meta[BTREE_TEXT_ENCODING">1].
                /// get btree meta values for keys between SCHEMA_VERSION..TEXT_ENCODING

                var meta = Enum
                    .Range(BTreeProp.SCHEMA_VERSION, BTreeProp.TEXT_ENCODING)
                    .ToDictionary(p => p, p => pDb.BTree.sqlite3BtreeGetMeta(p));

                pDb.pSchema.schema_cookie = (int)meta[BTreeProp.SCHEMA_VERSION];

                ///If opening a non-empty database, check the text encoding. For the
                ///main database, set sqlite3.enc to the encoding of the main database.
                ///For an attached db, it is an error if the encoding is not the same
                ///as sqlite3.enc.
                if (meta[BTreeProp.TEXT_ENCODING] != 0)
                {
                    ///text encoding 
                    if (iDb == 0)
                    {
                        ///If opening the main database, set ENC(db). 
                        var encoding = (SqliteEncoding)(meta[BTreeProp.TEXT_ENCODING] & 3);
                        if (0 == encoding)
                            encoding = SqliteEncoding.UTF8;
                        db.Backends[0].pSchema.enc = encoding;
                        //ENC( db ) = encoding;
                        db.pDfltColl = db.sqlite3FindCollSeq(SqliteEncoding.UTF8, "BINARY", 0);
                    }
                    else {
                        ///If opening an attached database, the encoding much match ENC(db) 
                        if ((SqliteEncoding)meta[BTreeProp.TEXT_ENCODING] != sqliteinth.ENC(db))
                        {
                            malloc_cs.sqlite3SetString(ref pzErrMsg, db, "attached databases must use the same text encoding as main database");
                            rc = SqlResult.SQLITE_ERROR;
                            goto initone_error_out;
                        }
                    }
                }
                else {
                    db.DbSetProperty(iDb, sqliteinth.DB_Empty);
                }
                pDb.pSchema.enc = sqliteinth.ENC(db);
                if (pDb.pSchema.cache_size == 0)
                {
                    var size = utilc.sqlite3AbsInt32((int)meta[BTreeProp.DEFAULT_CACHE_SIZE]);
                    if (size == 0)
                    {
                        size = Globals.SQLITE_DEFAULT_CACHE_SIZE;
                    }
                    pDb.pSchema.cache_size = size;
                    pDb.BTree.SetCacheSize(pDb.pSchema.cache_size);
                }
                ///file_format==1    Version 3.0.0.
                ///file_format==2    Version 3.1.3.  // ALTER TABLE ADD COLUMN
                ///<param name="file_format==3    Version 3.1.4.  // ditto but with non">NULL defaults</param>
                ///<param name="file_format==4    Version 3.3.0.  // DESC indices.  Boolean constants">file_format==4    Version 3.3.0.  // DESC indices.  Boolean constants</param>
                ///<param name=""></param>
                pDb.pSchema.file_format = (u8)meta[BTreeProp.FILE_FORMAT];
                if (pDb.pSchema.file_format == 0)
                {
                    pDb.pSchema.file_format = 1;
                }
                if (pDb.pSchema.file_format > sqliteinth.SQLITE_MAX_FILE_FORMAT)
                {
                    malloc_cs.sqlite3SetString(ref pzErrMsg, db, "unsupported file format");
                    rc = SqlResult.SQLITE_ERROR;
                    goto initone_error_out;
                }
                ///Ticket #2804:  When we open a database in the newer file format,
                ///clear the legacy_file_format pragma flag so that a VACUUM will
                ///not downgrade the database and thus invalidate any descending
                ///indices that the user might have created.
                if (iDb == 0 && meta[BTreeProp.FILE_FORMAT] >= 4)
                {
                    db.flags &= ~SqliteFlags.SQLITE_LegacyFileFmt;
                }

                #endregion

                ///Read the schema information out of the schema tables
                Debug.Assert(db.init.IsBusy);
                {
                    var sqlSelectTableDefinitions = io.sqlite3MPrintf(db, "SELECT name, rootpage, sql FROM '%q'.%s ORDER BY rowid", db.Backends[iDb].Name, zMasterName);
#if !SQLITE_OMIT_AUTHORIZATION
																																																																																																{
int (*xAuth)(void*,int,const char*,const char*,const char*,const char*);
xAuth = db.xAuth;
db.xAuth = 0;
#endif
                    rc = legacy.Exec(db:db, zSql:sqlSelectTableDefinitions, xCallback:SchemaExtensions.InitTableDefinitionCallback, pArg:initData, NoErrors:0);
                    pzErrMsg = initData.pzErrMsg;
#if !SQLITE_OMIT_AUTHORIZATION
																																																																																																db.xAuth = xAuth;
}
#endif
                    if (rc == SqlResult.SQLITE_OK)
                        rc = initData.rc;
                    db.DbFree(ref sqlSelectTableDefinitions);
#if !SQLITE_OMIT_ANALYZE
                    if (rc == SqlResult.SQLITE_OK)
                    {
                        Sqlite3.sqlite3AnalysisLoad(db, iDb);
                    }
#endif                    
                    ///Jump here for an error that occurs after successfully allocating
                    ///curMain and calling sqlite3BtreeEnter(). For an error that occurs
                    ///before that point, jump to error_out.
                }
                if (rc == SqlResult.SQLITE_OK || (db.flags & SqliteFlags.SQLITE_RecoveryMode) != 0)
                {
                    ///Black magic: If the SQLITE_RecoveryMode flag is set, then consider
                    ///the schema loaded, even if errors occurred. In this situation the
                    ///current sqlite3_prepare() operation will fail, but the following one
                    ///will attempt to compile the supplied statement against whatever subset
                    ///of the schema was loaded before the error occurred. The primary
                    ///purpose of this is to allow access to the sqlite_master table
                    ///even when its contents have been corrupted.
                    db.DbSetProperty(iDb, sqliteinth.DB_SchemaLoaded);
                    rc = SqlResult.SQLITE_OK;
                }
                initone_error_out:
                if (openedTransaction != 0)
                {
                    pDb.BTree.sqlite3BtreeCommit();
                }


            }//BTree Scope

            error_out:
            if (rc == SqlResult.SQLITE_NOMEM || rc == SqlResult.SQLITE_IOERR_NOMEM)
            {
                //        db.mallocFailed = 1;
            }
            return rc;
        }
        ///<summary>
        /// Initialize all database files - the main database file, the file
        /// used to store temporary tables, and any additional database files
        /// created using ATTACH statements.  Return a success code.  If an
        /// error occurs, write an error message into pzErrMsg.
        ///
        /// After a database is initialized, the DB_SchemaLoaded bit is set
        /// bit is set in the flags field of the Db structure. If the database
        /// file was of zero-length, then the DB_Empty flag is also set.
        ///
        ///</summary>
        public static SqlResult InitialiseAllDatabases(this Connection db, ref string err)
        {
            int i;
            SqlResult rc;
            bool commit_internal = !((db.flags & SqliteFlags.SQLITE_InternChanges) != 0);
            Debug.Assert(db.mutex.sqlite3_mutex_held());
            rc = SqlResult.SQLITE_OK;

            String pzErrMsg = String.Empty;
            using (db.init.scope())
            {
                Enumerable.Range(0, db.BackendCount)                    
                    .Where(x => !((db.DbHasProperty(x, sqliteinth.DB_SchemaLoaded) || x == 1)))
                    .ForEach(
                        (idx,i2) => {
                            rc = InitialiseSingleDatabase(db, idx, ref pzErrMsg);//  <<<<<<<<<<<<<<<<<<<<<<<<<
                            if (rc != 0)
                                build.sqlite3ResetInternalSchema(db, idx);
                            return rc == SqlResult.SQLITE_OK;
                        }
                    );
                
                
                ///Once all the other databases have been initialised, load the schema
                ///for the TEMP database. This is loaded last, as the TEMP database
                ///schema may contain references to objects in other databases.
#if !SQLITE_OMIT_TEMPDB
                if (rc == SqlResult.SQLITE_OK && Sqlite3.ALWAYS(db.BackendCount > 1) && !db.DbHasProperty(1, sqliteinth.DB_SchemaLoaded))
                {
                    rc = InitialiseSingleDatabase(db, 1, ref pzErrMsg);//<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<
                    if (rc != 0)
                        build.sqlite3ResetInternalSchema(db, 1);
                }
#endif
            }

            if (rc == SqlResult.SQLITE_OK && commit_internal)
                build.sqlite3CommitInternalChanges(db);

            return rc;
        }
        ///<summary>
        /// This routine is a no-op if the database schema is already initialised.
        /// Otherwise, the schema is loaded. An error code is returned.
        ///
        ///</summary>
        public static SqlResult sqlite3ReadSchema(this Sqlite3.Parse pParse)
        {
            SqlResult rc = SqlResult.SQLITE_OK;
            Connection db = pParse.db;
            Debug.Assert(db.mutex.sqlite3_mutex_held());
            if (!db.init.IsBusy)
            {
                rc = InitialiseAllDatabases(db, ref pParse.zErrMsg);
            }
            if (rc != SqlResult.SQLITE_OK)
            {
                pParse.rc = rc;
                pParse.nErr++;
            }
            return rc;
        }
        ///<summary>
        /// Check schema cookies in all databases.  If any cookie is out
        /// of date set pParse->rc to SQLITE_SCHEMA.  If all schema cookies
        /// make no changes to pParse->rc.
        ///
        ///</summary>
        static void schemaIsValid(Sqlite3.Parse pParse)
        {
            Connection db = pParse.db;
            int iDb;
            SqlResult rc;
            u32 cookie = 0;
            Debug.Assert(pParse.checkSchema != 0);
            Debug.Assert(db.mutex.sqlite3_mutex_held());
            for (iDb = 0; iDb < db.BackendCount; iDb++)
            {
                int openedTransaction = 0;
                ///True if a transaction is opened 
                Btree pBt = db.Backends[iDb].BTree;
                ///Btree database to read cookie from 
                if (pBt == null)
                    continue;
                ///If there is not already a read">write) transaction opened</param>
                ///on the b">tree database, open one now. If a transaction is opened, it </param>
                ///will be closed immediately after reading the meta">value. </param>
                if (!pBt.sqlite3BtreeIsInReadTrans())
                {
                    rc = pBt.sqlite3BtreeBeginTrans(0);
                    //if ( rc == SQLITE_NOMEM || rc == SQLITE_IOERR_NOMEM )
                    //{
                    //    db.mallocFailed = 1;
                    //}
                    if (rc != SqlResult.SQLITE_OK)
                        return;
                    openedTransaction = 1;
                }
                ///Read the schema cookie from the database. If it does not match the 
                ///value stored as part of the in-memory schema representation,
                ///set Parse.rc to SQLITE_SCHEMA. ">set Parse.rc to SQLITE_SCHEMA.
                cookie = pBt.sqlite3BtreeGetMeta(BTreeProp.SCHEMA_VERSION);
                Debug.Assert(Sqlite3.sqlite3SchemaMutexHeld(db, iDb, null));
                if (cookie != db.Backends[iDb].pSchema.schema_cookie)
                {
                    build.sqlite3ResetInternalSchema(db, iDb);
                    pParse.rc = SqlResult.SQLITE_SCHEMA;
                }
                ///Close the transaction, if one was opened. 
                if (openedTransaction != 0)
                {
                    pBt.sqlite3BtreeCommit();
                }
            }
        }
        ///<summary>
        ///sqlite3SchemaToIndex
        /// Convert a schema pointer into the iDb index that indicates
        /// which database file in db.aDb[] the schema refers to.
        ///
        /// If the same database is attached more than once, the first
        /// attached database is returned.
        ///
        ///</summary>
        public static int indexOfBackendWithSchema(this Connection db, Schema pSchema)
        {//TODO: extension method
            int i = -1000000;
            ///If pSchema is NULL, then return ">1000000. This happens when code in</param>
            ///expr.c is trying to resolve a reference to a transient table (i.e. one
            ///created by a sub-select). In this case the return value of this</param>
            ///function should never be used.
            ///We return 
            ///">>aDb[] is much
            ///more likely to cause a segfault than 1 (of course there are assert()
            ///statements too, but it never hurts to play the odds).
            Debug.Assert(db.mutex.sqlite3_mutex_held());
            if (pSchema != null)
            {
                for (i = 0; Sqlite3.ALWAYS(i < db.BackendCount); i++)
                {
                    if (db.Backends[i].pSchema == pSchema)
                    {
                        break;
                    }
                }
                Debug.Assert(i >= 0 && i < db.BackendCount);
            }
            return i;
        }
        ///<summary>
        /// Compile the UTF-8 encoded SQL statement zSql into a statement handle.
        ///
        ///</summary>
        static SqlResult sqlite3Prepare(Connection db,///
                                                      ///<summary>
                                                      ///Database handle. 
                                                      ///</summary>
      string zSql,///
                    ///<summary>
                    ///</summary>
                    ///<param name="UTF">8 encoded SQL statement. </param>
        int nBytes,///
                   ///<summary>
                   ///Length of zSql in bytes. 
                   ///</summary>
     int saveSqlFlag,///
                        ///<summary>
                        ///True to copy SQL text into the sqlite3_stmt 
                        ///</summary>
        Vdbe pReprepare,///
                        ///<summary>
                        ///VM being reprepared 
                        ///</summary>
        ref sqlite3_stmt ppStmt,///
                                ///<summary>
                                ///OUT: A pointer to the prepared statement 
                                ///</summary>
        ref string pzTail///
                         ///<summary>
                         ///OUT: End of parsed string 
                         ///</summary>
       )
        {
            ///Parsing context 
            string zErrMsg = "";
            ///Error message 
            var rc = SqlResult.SQLITE_OK;
            ///Result code 
            int i;
            ///Loop counter 
            ppStmt = null;
            pzTail = null;
            ///Allocate the parsing context 
            var pParse = new Sqlite3.Parse();
            //sqlite3StackAllocZero(db, sizeof(*pParse));
            //if ( pParse == null )
            //{
            //  rc = SQLITE_NOMEM;
            //  goto end_prepare;
            //}
            pParse.pReprepare = pReprepare;
            pParse.sLastToken.zRestSql = "";
            //  assert( ppStmt && *ppStmt==0 );
            //Debug.Assert( 0 == db.mallocFailed );
            Debug.Assert(db.mutex.sqlite3_mutex_held());
            ///
            ///<summary>
            ///Check to verify that it is possible to get a read lock on all
            ///database schemas.  The inability to get a read lock indicates that
            ///</summary>
            ///<param name="some other database connection is holding a write">lock, which in</param>
            ///<param name="turn means that the other connection has made uncommitted changes">turn means that the other connection has made uncommitted changes</param>
            ///<param name="to the schema.">to the schema.</param>
            ///<param name=""></param>
            ///<param name="Were we to proceed and prepare the statement against the uncommitted">Were we to proceed and prepare the statement against the uncommitted</param>
            ///<param name="schema changes and if those schema changes are subsequently rolled">schema changes and if those schema changes are subsequently rolled</param>
            ///<param name="back and different changes are made in their place, then when this">back and different changes are made in their place, then when this</param>
            ///<param name="prepared statement goes to run the schema cookie would fail to detect">prepared statement goes to run the schema cookie would fail to detect</param>
            ///<param name="the schema change.  Disaster would follow.">the schema change.  Disaster would follow.</param>
            ///<param name=""></param>
            ///<param name="This thread is currently holding mutexes on all Btrees (because">This thread is currently holding mutexes on all Btrees (because</param>
            ///<param name="of the sqlite3BtreeEnterAll() in sqlite3LockAndPrepare()) so it">of the sqlite3BtreeEnterAll() in sqlite3LockAndPrepare()) so it</param>
            ///<param name="is not possible for another thread to start a new schema change">is not possible for another thread to start a new schema change</param>
            ///<param name="while this routine is running.  Hence, we do not need to hold">while this routine is running.  Hence, we do not need to hold</param>
            ///<param name="locks on the schema, we just need to make sure nobody else is">locks on the schema, we just need to make sure nobody else is</param>
            ///<param name="holding them.">holding them.</param>
            ///<param name=""></param>
            ///<param name="Note that setting READ_UNCOMMITTED overrides most lock detection,">Note that setting READ_UNCOMMITTED overrides most lock detection,</param>
            ///<param name="but it does *not* override schema lock detection, so this all still">but it does *not* override schema lock detection, so this all still</param>
            ///<param name="works even if READ_UNCOMMITTED is set.">works even if READ_UNCOMMITTED is set.</param>
            ///<param name=""></param>
            for (i = 0; i < db.BackendCount; i++)
            {
                Btree pBt = db.Backends[i].BTree;
                if (pBt != null)
                {
                    Debug.Assert(Sqlite3.sqlite3BtreeHoldsMutex(pBt));
                    rc = pBt.sqlite3BtreeSchemaLocked();
                    if (rc != 0)
                    {
                        string zDb = db.Backends[i].Name;
                        utilc.sqlite3Error(db, rc, "database schema is locked: %s", zDb);
                        sqliteinth.testcase(db.flags & SqliteFlags.SQLITE_ReadUncommitted);
                        goto end_prepare;
                    }
                }
            }
            VTableMethodsExtensions.sqlite3VtabUnlockList(db);
            pParse.db = db;
            pParse.nQueryLoop = (double)1;
            if (nBytes >= 0 && (nBytes == 0 || zSql[nBytes - 1] != 0))
            {
                string zSqlCopy;
                int mxLen = db.aLimit[Globals.SQLITE_LIMIT_SQL_LENGTH];
                sqliteinth.testcase(nBytes == mxLen);
                sqliteinth.testcase(nBytes == mxLen + 1);
                if (nBytes > mxLen)
                {
                    utilc.sqlite3Error(db, SqlResult.SQLITE_TOOBIG, "statement too long");
                    rc = malloc_cs.sqlite3ApiExit(db, SqlResult.SQLITE_TOOBIG);
                    goto end_prepare;
                }
                zSqlCopy = zSql.Substring(0, nBytes);
                // sqlite3DbStrNDup(db, zSql, nBytes);
                if (zSqlCopy != null)
                {
                    pParse.sqlite3RunParser(zSqlCopy, ref zErrMsg);
                    db.DbFree(ref zSqlCopy);
                    //pParse->zTail = &zSql[pParse->zTail-zSqlCopy];
                }
                else {
                    //pParse->zTail = &zSql[nBytes];
                }
            }
            else {
                pParse.sqlite3RunParser(zSql, ref zErrMsg);
            }
            Debug.Assert(1 == (int)pParse.nQueryLoop);
            //if ( db.mallocFailed != 0 )
            //{
            //  pParse.rc = SQLITE_NOMEM;
            //}
            if (pParse.rc == SqlResult.SQLITE_DONE)
                pParse.rc = SqlResult.SQLITE_OK;
            if (pParse.checkSchema != 0)
            {
                schemaIsValid(pParse);
            }
            //if ( db.mallocFailed != 0 )
            //{
            //  pParse.rc = SQLITE_NOMEM;
            //}
            //if (pzTail != null)
            {
                pzTail = pParse.zTail == null ? "" : pParse.zTail.ToString();
#if !SQLITE_OMIT_EXPLAIN
#endif
                ///
                ///<summary>
                ///Delete any TriggerPrg structures allocated while parsing this statement. 
                ///</summary>
                //sqlite3StackFree( db, pParse );
            }
            rc = pParse.rc;
            if (rc == SqlResult.SQLITE_OK && pParse.pVdbe != null && pParse.explain != 0)
            {
                string[] azColName = new string[] {
                    "addr",
                    "opcode",
                    "p1",
                    "p2",
                    "p3",
                    "p4",
                    "p5",
                    "comment",
                    "selectid",
                    "order",
                    "from",
                    "detail"
                };
                int iFirst, mx;
                if (pParse.explain == 2)
                {
                    pParse.pVdbe.sqlite3VdbeSetNumCols(4);
                    iFirst = 8;
                    mx = 12;
                }
                else {
                    pParse.pVdbe.sqlite3VdbeSetNumCols(8);
                    iFirst = 0;
                    mx = 8;
                }
                for (i = iFirst; i < mx; i++)
                {
                    pParse.pVdbe.sqlite3VdbeSetColName(i - iFirst, ColName.NAME, azColName[i], Sqlite3.SQLITE_STATIC);
                }
            }
            Debug.Assert(!db.init.IsBusy || saveSqlFlag == 0);
            if (!db.init.IsBusy)
            {
                Vdbe pVdbe = pParse.pVdbe;
                vdbeaux.sqlite3VdbeSetSql(pVdbe, zSql, (int)(zSql.Length - (pParse.zTail == null ? 0 : pParse.zTail.Length)), saveSqlFlag);
            }
            if (pParse.pVdbe != null && (rc != SqlResult.SQLITE_OK///
                                                                  ///<summary>
                                                                  ///|| db.mallocFailed != 0 
                                                                  ///</summary>
          ))
            {
                vdbeaux.sqlite3VdbeFinalize(ref pParse.pVdbe);
                //Debug.Assert( ppStmt == null );
            }
            else {
                ppStmt = pParse.pVdbe;
            }
            if (zErrMsg != "")
            {
                utilc.sqlite3Error(db, rc, "%s", zErrMsg);
                db.DbFree(ref zErrMsg);
            }
            else {
                utilc.sqlite3Error(db, rc, 0);
            }
            while (pParse.pTriggerPrg != null)
            {
                TriggerPrg pT = pParse.pTriggerPrg;
                pParse.pTriggerPrg = pT.pNext;
                db.DbFree(ref pT);
            }
            end_prepare:
            rc = malloc_cs.sqlite3ApiExit(db, rc);
            Debug.Assert((rc & db.errMask) == rc);
            return rc;
        }
        //C# Version w/o End of Parsed String
        static SqlResult sqlite3LockAndPrepare(Connection db,///
                                                             ///<summary>
                                                             ///Database handle. 
                                                             ///</summary>
       string zSql,///
                    ///<summary>
                    ///</summary>
                    ///<param name="UTF">8 encoded SQL statement. </param>
        int nBytes,///
                   ///<summary>
                   ///Length of zSql in bytes. 
                   ///</summary>
     int saveSqlFlag,///
                        ///<summary>
                        ///True to copy SQL text into the sqlite3_stmt 
                        ///</summary>
        Vdbe pOld,///
                  ///<summary>
                  ///VM being reprepared 
                  ///</summary>
      ref sqlite3_stmt ppStmt,///
                                ///<summary>
                                ///OUT: A pointer to the prepared statement 
                                ///</summary>
        int dummy///
                 ///<summary>
                 ///OUT: End of parsed string 
                 ///</summary>
       )
        {
            string sOut = null;
            return sqlite3LockAndPrepare(db, zSql, nBytes, saveSqlFlag, pOld, ref ppStmt, ref sOut);
        }
        static SqlResult sqlite3LockAndPrepare(Connection db,///Database handle. 
            string zSql, ///<param name="UTF">8 encoded SQL statement. </param>
            int nBytes,///Length of zSql in bytes.                    
            int saveSqlFlag,///True to copy SQL text into the sqlite3_stmt                      
                    Vdbe pOld,///VM being reprepared 
      ref sqlite3_stmt ppStmt,///OUT: A pointer to the prepared statement 
        ref string pzTail///OUT: End of parsed string                          
       )
        {
            SqlResult rc;
            //  assert( ppStmt!=0 );
            if (!utilc.sqlite3SafetyCheckOk(db))
            {
                ppStmt = null;
                pzTail = null;
                return sqliteinth.SQLITE_MISUSE_BKPT();
            }
            using (db.mutex.scope())
            {
                db.sqlite3BtreeEnterAll();
                rc = sqlite3Prepare(db, zSql, nBytes, saveSqlFlag, pOld, ref ppStmt, ref pzTail);
                if (rc == SqlResult.SQLITE_SCHEMA)
                {
                    vdbeapi.sqlite3_finalize(ppStmt);
                    rc = sqlite3Prepare(db, zSql, nBytes, saveSqlFlag, pOld, ref ppStmt, ref pzTail);
                }
                Sqlite3.sqlite3BtreeLeaveAll(db);
            }
            return rc;
        }
        ///<summary>
        /// Rerun the compilation of a statement after a schema change.
        ///
        /// If the statement is successfully recompiled, return SqlResult.SQLITE_OK. Otherwise,
        /// if the statement cannot be recompiled because another connection has
        /// locked the sqlite3_master table, return SQLITE_LOCKED. If any other error
        /// occurs, return SQLITE_SCHEMA.
        ///
        ///</summary>
        public static SqlResult sqlite3Reprepare(Vdbe p)
        {
            SqlResult rc;
            sqlite3_stmt pNew = new sqlite3_stmt();
            string zSql;
            Connection db;
            Debug.Assert(p.sqlite3VdbeDb().mutex.sqlite3_mutex_held());
            zSql = vdbeaux.sqlite3_sql((sqlite3_stmt)p);
            Debug.Assert(zSql != null);
            ///Reprepare only called for prepare_v2() statements 
            db = p.sqlite3VdbeDb();
            Debug.Assert(db.mutex.sqlite3_mutex_held());
            rc = sqlite3LockAndPrepare(db, zSql, -1, 0, p, ref pNew, 0);
            if (rc != 0)
            {
                if (rc == SqlResult.SQLITE_NOMEM)
                {
                    //        db.mallocFailed = 1;
                }
                Debug.Assert(pNew == null);
                return rc;
            }
            else {
                Debug.Assert(pNew != null);
            }
            vdbeaux.sqlite3VdbeSwap((Vdbe)pNew, p);
            vdbeapi.sqlite3TransferBindings(pNew, (sqlite3_stmt)p);
            ((Vdbe)pNew).sqlite3VdbeResetStepResult();
            vdbeaux.sqlite3VdbeFinalize(ref pNew);
            return SqlResult.SQLITE_OK;
        }
        //C# Overload for ignore error out
        static public SqlResult sqlite3_prepare(Connection db,///Database handle. 
            string zSql,///<param name="UTF">8 encoded SQL statement. </param>
            int nBytes,///Length of zSql in bytes.                    
            ref sqlite3_stmt ppStmt,///OUT: A pointer to the prepared statement 
            int dummy///OUT: End of parsed string                  
       )
        {
            string sOut = null;
            return sqlite3_prepare(db, zSql, nBytes, ref ppStmt, ref sOut);
        }
        ///<summary>
        /// Two versions of the official API.  Legacy and new use.  In the legacy
        /// version, the original SQL text is not saved in the prepared statement
        /// and so if a schema change occurs, SQLITE_SCHEMA is returned by
        /// sqlite3_step().  In the new version, the original SQL text is retained
        /// and the statement is automatically recompiled if an schema change
        /// occurs.
        ///
        ///</summary>
        static public SqlResult sqlite3_prepare(Connection db,///Database handle. 
                string zSql,  ///<param name="UTF">8 encoded SQL statement. </param>
                int nBytes,///Length of zSql in bytes.                    
                ref sqlite3_stmt ppStmt,///OUT: A pointer to the prepared statement 
                ref string pzTail///OUT: End of parsed string 
       )
        {
            SqlResult rc;
            rc = sqlite3LockAndPrepare(db, zSql, nBytes, 0, null, ref ppStmt, ref pzTail);
            Debug.Assert(rc == SqlResult.SQLITE_OK || ppStmt == null);
            ///VERIFY: F13021 
            return rc;
        }
        public static SqlResult sqlite3_prepare_v2(Connection db,///
                                                                 ///<summary>
                                                                 ///Database handle. 
                                                                 ///</summary>
       string zSql,///
                    ///<summary>
                    ///</summary>
                    ///<param name="UTF">8 encoded SQL statement. </param>
        int nBytes,///
                   ///<summary>
                   ///Length of zSql in bytes. 
                   ///</summary>
     ref sqlite3_stmt ppStmt,///
                                ///<summary>
                                ///OUT: A pointer to the prepared statement 
                                ///</summary>
        int dummy///( No string passed) 
                 )
        {
            string pzTail = null;
            SqlResult rc;
            rc = sqlite3LockAndPrepare(db, zSql, nBytes, 1, null, ref ppStmt, ref pzTail);
            Debug.Assert(rc == SqlResult.SQLITE_OK || ppStmt == null);
            ///VERIFY: F13021 
            return rc;
        }
        public static SqlResult sqlite3_prepare_v2(Connection db,///
                                                                 ///<summary>
                                                                 ///Database handle. 
                                                                 ///</summary>
       string zSql,///
                    ///<summary>
                    ///</summary>
                    ///<param name="UTF">8 encoded SQL statement. </param>
        int nBytes,///
                   ///<summary>
                   ///Length of zSql in bytes. 
                   ///</summary>
     ref sqlite3_stmt ppStmt,///
                                ///<summary>
                                ///OUT: A pointer to the prepared statement 
                                ///</summary>
        ref string pzTail///
                         ///<summary>
                         ///OUT: End of parsed string 
                         ///</summary>
       )
        {
            SqlResult rc;
            rc = sqlite3LockAndPrepare(db, zSql, nBytes, 1, null, ref ppStmt, ref pzTail);
            Debug.Assert(rc == SqlResult.SQLITE_OK || ppStmt == null);
            ///VERIFY: F13021 
            return rc;
        }
#if !SQLITE_OMIT_UTF16
																								
///<summary>
/// Compile the UTF-16 encoded SQL statement zSql into a statement handle.
///</summary>
static int sqlite3Prepare16(
sqlite3 db,              /* Database handle. */
string zSql,             /* UTF-15 encoded SQL statement. */
int nBytes,              /* Length of zSql in bytes. */
bool saveSqlFlag,         /* True to save SQL text into the sqlite3_stmt */
out sqlite3_stmt ppStmt, /* OUT: A pointer to the prepared statement */
out string pzTail        /* OUT: End of parsed string */
){
/* This function currently works by first transforming the UTF-16
** encoded string to UTF-8, then invoking sqlite3_prepare(). The
** tricky bit is figuring out the pointer to return in pzTail.
*/
string zSql8;
string zTail8 = "";
var rc = SqlResult.SQLITE_OK;

assert( ppStmt );
*ppStmt = 0;
if( !sqlite3SafetyCheckOk(db) ){
return sqliteinth.SQLITE_MISUSE_BKPT;
}
db.mutex.sqlite3_mutex_enter();
zSql8 = sqlite3Utf16to8(db, zSql, nBytes, SqliteEncoding.UTF16NATIVE);
if( zSql8 !=""){
rc = sqlite3LockAndPrepare(db, zSql8, -1, saveSqlFlag, null, ref ppStmt, ref zTail8);
}

if( zTail8 !="" && pzTail !=""){
/* If sqlite3_prepare returns a tail pointer, we calculate the
** equivalent pointer into the UTF-16 string by counting the unicode
** characters between zSql8 and zTail8, and then returning a pointer
** the same number of characters into the UTF-16 string.
*/
Debugger.Break (); // TODO --
//  int chars_parsed = sqlite3Utf8CharLen(zSql8, (int)(zTail8-zSql8));
//  pzTail = (u8 *)zSql + sqlite3Utf16ByteLen(zSql, chars_parsed);
}
sqlite3DbFree(db,ref zSql8);
rc = malloc_cs.sqlite3ApiExit(db, rc);
db.mutex.sqlite3_mutex_leave();
return rc;
}

///<summary>
/// Two versions of the official API.  Legacy and new use.  In the legacy
/// version, the original SQL text is not saved in the prepared statement
/// and so if a schema change occurs, SQLITE_SCHEMA is returned by
/// sqlite3_step().  In the new version, the original SQL text is retained
/// and the statement is automatically recompiled if an schema change
/// occurs.
///</summary>
public static int sqlite3_prepare16(
sqlite3 db,               /* Database handle. */
string zSql,              /* UTF-16 encoded SQL statement. */
int nBytes,               /* Length of zSql in bytes. */
out sqlite3_stmt ppStmt,  /* OUT: A pointer to the prepared statement */
out string pzTail         /* OUT: End of parsed string */
){
int rc;
rc = sqlite3Prepare16(db,zSql,nBytes,false,ref ppStmt,ref pzTail);
Debug.Assert( rc==SqlResult.SQLITE_OK || ppStmt==null || ppStmt==null );  /* VERIFY: F13021 */
return rc;
}
public static int sqlite3_prepare16_v2(
sqlite3 db,               /* Database handle. */
string zSql,              /* UTF-16 encoded SQL statement. */
int nBytes,               /* Length of zSql in bytes. */
out sqlite3_stmt ppStmt,  /* OUT: A pointer to the prepared statement */
out string pzTail         /* OUT: End of parsed string */
)
{
int rc;
rc = sqlite3Prepare16(db,zSql,nBytes,true,ref ppStmt,ref pzTail);
Debug.Assert( rc==SqlResult.SQLITE_OK || ppStmt==null || ppStmt==null );  /* VERIFY: F13021 */
return rc;
}

#endif
    }
}
