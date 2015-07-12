using System;
using System.Diagnostics;
using System.Text;
using u8=System.Byte;
using u16=System.UInt16;
using u32=System.UInt32;
using sqlite3_int64=System.Int64;
namespace Community.CsharpSqlite {
	using sqlite3_stmt=Engine.Vdbe;
    using Community.CsharpSqlite.builder;
    using Community.CsharpSqlite.Metadata;
    using Community.CsharpSqlite.Os;
    using Community.CsharpSqlite.Engine;
    using Community.CsharpSqlite.tree;
	public partial class Sqlite3 {
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
		/// Fill the InitData structure with an error message that indicates
		/// that the database is corrupt.
		///
		///</summary>
		static void corruptSchema(InitData pData,///
		///<summary>
		///Initialization context 
		///</summary>
		string zObj,///
		///<summary>
		///Object being parsed at the point of error 
		///</summary>
		string zExtra///
		///<summary>
		///Error information 
		///</summary>
		) {
			sqlite3 db=pData.db;
			if(///
			///<summary>
			///0 == db.mallocFailed && 
			///</summary>
            (db.flags & SqliteFlags.SQLITE_RecoveryMode) == 0)
            {
				{
					if(zObj==null) {
						zObj="?";
						#if SQLITE_OMIT_UTF16
						if(sqliteinth.ENC(db)!=SqliteEncoding.UTF8)
                            zObj = encnames[((int)sqliteinth.ENC(db))].zName;
						#endif
					}
					malloc_cs.sqlite3SetString(ref pData.pzErrMsg,db,"malformed database schema (%s)",zObj);
					if(!String.IsNullOrEmpty(zExtra)) {
						pData.pzErrMsg=io.sqlite3MAppendf(db,pData.pzErrMsg,"%s - %s",pData.pzErrMsg,zExtra);
					}
				}
				pData.rc=//db.mallocFailed != 0 ? SQLITE_NOMEM :
                sqliteinth.SQLITE_CORRUPT_BKPT();
			}
		}
		///<summary>
		/// This is the callback routine for the code that initializes the
		/// database.  See sqlite3Init() below for additional information.
		/// This routine is also called from the OP_ParseSchema opcode of the VDBE.
		///
		/// Each callback contains the following information:
		///
		///     argv[0] = name of thing being created
		///     argv[1] = root page number for table or index. 0 for trigger or view.
		///     argv[2] = SQL text for the CREATE statement.
		///
		///
		///</summary>
		public static int sqlite3InitCallback(object pInit,sqlite3_int64 argc,object p2,object NotUsed) {
			string[] argv=(string[])p2;
			InitData pData=(InitData)pInit;
			sqlite3 db=pData.db;
			int iDb=pData.iDb;
			Debug.Assert(argc==3);
			sqliteinth.UNUSED_PARAMETER2(NotUsed,argc);
			Debug.Assert(db.mutex.sqlite3_mutex_held());
			db.DbClearProperty(iDb,sqliteinth.DB_Empty);
            
			//if ( db.mallocFailed != 0 )
			//{
			//  corruptSchema( pData, argv[0], "" );
			//  return 1;
			//}
			Debug.Assert(iDb>=0&&iDb<db.nDb);
			if(argv==null)
				return 0;
			///
			///<summary>
			///Might happen if EMPTY_RESULT_CALLBACKS are on 
			///</summary>
			if(argv[1]==null) {
				corruptSchema(pData,argv[0],"");
			}
			else
				if(!String.IsNullOrEmpty(argv[2])) {
					///
					///<summary>
					///Call the parser to process a CREATE TABLE, INDEX or VIEW.
					///But because db.init.busy is set to 1, no VDBE code is generated
					///or executed.  All the parser does is build the internal data
					///structures that describe the table, index, or view.
					///
					///</summary>
					SqlResult rc;
					sqlite3_stmt pStmt=null;
					#if !NDEBUG || SQLITE_COVERAGE_TEST
																																																																																																																								        //TESTONLY(int rcp);            /* Return code from sqlite3_prepare() */
        int rcp;
#endif
					Debug.Assert(db.init.busy!=0);
					db.init.iDb=iDb;
					db.init.newTnum=Converter.sqlite3Atoi(argv[1]);
					db.init.orphanTrigger=0;
					//TESTONLY(rcp = ) sqlite3_prepare(db, argv[2], -1, &pStmt, 0);
					#if !NDEBUG || SQLITE_COVERAGE_TEST
																																																																																																																								        rcp = sqlite3_prepare( db, argv[2], -1, ref pStmt, 0 );
#else
					sqlite3_prepare(db,argv[2],-1,ref pStmt,0);
					#endif
					rc=db.errCode;
					#if !NDEBUG || SQLITE_COVERAGE_TEST
																																																																																																																								        Debug.Assert( ( rc & 0xFF ) == ( rcp & 0xFF ) );
#endif
					db.init.iDb=0;
					if(SqlResult.SQLITE_OK!=rc) {
						if(db.init.orphanTrigger!=0) {
							Debug.Assert(iDb==1);
						}
						else {
							pData.rc=rc;
							//if ( rc == SQLITE_NOMEM )
							//{
							//  //        db.mallocFailed = 1;
							//}
							//else 
                            if (rc != SqlResult.SQLITE_INTERRUPT && (rc & (SqlResult)0xFF) != SqlResult.SQLITE_LOCKED)
                            {
								corruptSchema(pData,argv[0],sqlite3_errmsg(db));
							}
						}
					}
					vdbeapi.sqlite3_finalize(pStmt);
				}
				else
					if(argv[0]==null||argv[0]=="") {
						corruptSchema(pData,null,null);
					}
					else {
						///
						///<summary>
						///If the SQL column is blank it means this is an index that
						///was created to be the PRIMARY KEY or to fulfill a UNIQUE
						///constraint for a CREATE TABLE.  The index should have already
						///been created when we processed the CREATE TABLE.  All we have
						///to do here is record the root page number for that index.
						///
						///</summary>
						Index pIndex;
						pIndex=IndexBuilder.sqlite3FindIndex(db,argv[0],db.aDb[iDb].zName);
						if(pIndex==null) {
							///
							///<summary>
							///This can occur if there exists an index on a TEMP table which
							///has the same name as another index on a permanent index.  Since
							///the permanent table is hidden by the TEMP table, we can also
							///safely ignore the index on the permanent table.
							///
							///</summary>
							///
							///<summary>
							///Do Nothing 
							///</summary>
							;
						}
						else
							if(Converter.sqlite3GetInt32(argv[1],ref pIndex.tnum)==false) {
								corruptSchema(pData,argv[0],"invalid rootpage");
							}
					}
			return 0;
		}
		///<summary>
		/// Attempt to read the database schema and initialize internal
		/// data structures for a single database file.  The index of the
		/// database file is given by iDb.  iDb==0 is used for the main
		/// database.  iDb==1 should never be used.  iDb>=2 is used for
		/// auxiliary databases.  Return one of the SQLITE_ error codes to
		/// indicate success or failure.
		///
		///</summary>
		static SqlResult sqlite3InitOne(sqlite3 db,int iDb,ref string pzErrMsg) {
            SqlResult rc;
			int i;
			int size;
			Table pTab;
			Db pDb;
			string[] azArg=new string[4];
			u32[] meta=new u32[5];
			InitData initData=new InitData();
			string zMasterSchema;
			string zMasterName;
			int openedTransaction=0;
			///
			///<summary>
			///The master database table has a structure like this
			///
			///</summary>
			string master_schema="CREATE TABLE sqlite_master(\n"+"  type text,\n"+"  name text,\n"+"  tbl_name text,\n"+"  rootpage integer,\n"+"  sql text\n"+")";
			#if !SQLITE_OMIT_TEMPDB
			string temp_master_schema="CREATE TEMP TABLE sqlite_temp_master(\n"+"  type text,\n"+"  name text,\n"+"  tbl_name text,\n"+"  rootpage integer,\n"+"  sql text\n"+")";
			#else
																																																																								//define temp_master_schema 0
#endif
			Debug.Assert(iDb>=0&&iDb<db.nDb);
			Debug.Assert(db.aDb[iDb].pSchema!=null);
			Debug.Assert(db.mutex.sqlite3_mutex_held());
			Debug.Assert(iDb==1||sqlite3BtreeHoldsMutex(db.aDb[iDb].pBt));
			///
			///<summary>
			///zMasterSchema and zInitScript are set to point at the master schema
			///and initialisation script appropriate for the database being
			///initialised. zMasterName is the name of the master table.
			///
			///</summary>
            if (sqliteinth.OMIT_TEMPDB == 0 && iDb == 1)
            {
				zMasterSchema=temp_master_schema;
			}
			else {
				zMasterSchema=master_schema;
			}
            zMasterName = sqliteinth.SCHEMA_TABLE(iDb);
			///
			///<summary>
			///Construct the schema tables.  
			///</summary>
			azArg[0]=zMasterName;
			azArg[1]="1";
			azArg[2]=zMasterSchema;
			azArg[3]="";
			initData.db=db;
			initData.iDb=iDb;
			initData.rc=SqlResult.SQLITE_OK;
			initData.pzErrMsg=pzErrMsg;
			sqlite3InitCallback(initData,3,azArg,null);
			if(initData.rc!=0) {
				rc=initData.rc;
				goto error_out;
			}
			pTab=TableBuilder.sqlite3FindTable(db,zMasterName,db.aDb[iDb].zName);
			if(Sqlite3.ALWAYS(pTab)) {
				pTab.tabFlags|=TableFlags.TF_Readonly;
			}
			///
			///<summary>
			///Create a cursor to hold the database open
			///
			///</summary>
			pDb=db.aDb[iDb];
			if(pDb.pBt==null) {
                if (sqliteinth.OMIT_TEMPDB == 0 && Sqlite3.ALWAYS(iDb == 1))
                {
                    db.DbSetProperty(1, sqliteinth.DB_SchemaLoaded);
				}
				return SqlResult.SQLITE_OK;
			}
			///
			///<summary>
			///</summary>
			///<param name="If there is not already a read">write) transaction opened</param>
			///<param name="on the b">tree database, open one now. If a transaction is opened, it </param>
			///<param name="will be closed before this function returns.  ">will be closed before this function returns.  </param>
            pDb.pBt.sqlite3BtreeEnter();
			if(!pDb.pBt.sqlite3BtreeIsInReadTrans()) {
				rc=pDb.pBt.sqlite3BtreeBeginTrans(0);
				if(rc!=SqlResult.SQLITE_OK) {
					#if SQLITE_OMIT_WAL
					if(pDb.pBt.pBt.pSchema.file_format==2)
						malloc_cs.sqlite3SetString(ref pzErrMsg,db,"%s (wal format detected)",sqlite3ErrStr(rc));
					else
						malloc_cs.sqlite3SetString(ref pzErrMsg,db,"%s",sqlite3ErrStr(rc));
					#else
																																																																																																																								          malloc_cs.sqlite3SetString( ref pzErrMsg, db, "%s", sqlite3ErrStr( rc ) );
#endif
					goto initone_error_out;
				}
				openedTransaction=1;
			}
			///
			///<summary>
			///Get the database meta information.
			///
			///Meta values are as follows:
			///meta[0]   Schema cookie.  Changes with each schema change.
			///meta[1]   File format of schema layer.
			///meta[2]   Size of the page cache.
			///meta[3]   Largest rootpage (auto/incr_vacuum mode)
			///</summary>
			///<param name="meta[4]   Db text encoding. 1:UTF">16BE</param>
			///<param name="meta[5]   User version">meta[5]   User version</param>
			///<param name="meta[6]   Incremental vacuum mode">meta[6]   Incremental vacuum mode</param>
			///<param name="meta[7]   unused">meta[7]   unused</param>
			///<param name="meta[8]   unused">meta[8]   unused</param>
			///<param name="meta[9]   unused">meta[9]   unused</param>
			///<param name=""></param>
			///<param name="Note: The #defined SQLITE_UTF* symbols in sqliteInt.h correspond to">Note: The #defined SQLITE_UTF* symbols in sqliteInt.h correspond to</param>
			///<param name="the possible values of meta[BTREE_TEXT_ENCODING">1].</param>
			///<param name=""></param>
			for(i=0;i<Sqlite3.ArraySize(meta);i++) {
				meta[i]=pDb.pBt.sqlite3BtreeGetMeta(i+1);
			}
            pDb.pSchema.schema_cookie = (int)meta[BTreeProp.SCHEMA_VERSION - 1];
			///
			///<summary>
			///</summary>
			///<param name="If opening a non">empty database, check the text encoding. For the</param>
			///<param name="main database, set sqlite3.enc to the encoding of the main database.">main database, set sqlite3.enc to the encoding of the main database.</param>
			///<param name="For an attached db, it is an error if the encoding is not the same">For an attached db, it is an error if the encoding is not the same</param>
			///<param name="as sqlite3.enc.">as sqlite3.enc.</param>
			///<param name=""></param>
            if (meta[BTreeProp.TEXT_ENCODING - 1] != 0)
            {
				///
				///<summary>
				///text encoding 
				///</summary>
				if(iDb==0) {
					SqliteEncoding encoding;
					///
					///<summary>
					///If opening the main database, set ENC(db). 
					///</summary>
                    encoding = (SqliteEncoding)(meta[BTreeProp.TEXT_ENCODING - 1] & 3);
					if(encoding==0)
						encoding=SqliteEncoding.UTF8;
					db.aDb[0].pSchema.enc=encoding;
					//ENC( db ) = encoding;
					db.pDfltColl=db.sqlite3FindCollSeq(SqliteEncoding.UTF8,"BINARY",0);
				}
				else {
					///
					///<summary>
					///If opening an attached database, the encoding much match ENC(db) 
					///</summary>
                    if ((SqliteEncoding)meta[BTreeProp.TEXT_ENCODING - 1] != sqliteinth.ENC(db))
                    {
						malloc_cs.sqlite3SetString(ref pzErrMsg,db,"attached databases must use the same"+" text encoding as main database");
						rc=SqlResult.SQLITE_ERROR;
						goto initone_error_out;
					}
				}
			}
			else {
                db.DbSetProperty(iDb, sqliteinth.DB_Empty);
			}
            pDb.pSchema.enc = sqliteinth.ENC(db);
			if(pDb.pSchema.cache_size==0) {
                size = utilc.sqlite3AbsInt32((int)meta[BTreeProp.DEFAULT_CACHE_SIZE - 1]);
				if(size==0) {
					size=Globals.SQLITE_DEFAULT_CACHE_SIZE;
				}
				pDb.pSchema.cache_size=size;
				pDb.pBt.SetCacheSize(pDb.pSchema.cache_size);
			}
			///
			///<summary>
			///file_format==1    Version 3.0.0.
			///file_format==2    Version 3.1.3.  // ALTER TABLE ADD COLUMN
			///</summary>
			///<param name="file_format==3    Version 3.1.4.  // ditto but with non">NULL defaults</param>
			///<param name="file_format==4    Version 3.3.0.  // DESC indices.  Boolean constants">file_format==4    Version 3.3.0.  // DESC indices.  Boolean constants</param>
			///<param name=""></param>
            pDb.pSchema.file_format = (u8)meta[BTreeProp.FILE_FORMAT - 1];
			if(pDb.pSchema.file_format==0) {
				pDb.pSchema.file_format=1;
			}
            if (pDb.pSchema.file_format > sqliteinth.SQLITE_MAX_FILE_FORMAT)
            {
				malloc_cs.sqlite3SetString(ref pzErrMsg,db,"unsupported file format");
				rc=SqlResult.SQLITE_ERROR;
				goto initone_error_out;
			}
			///
			///<summary>
			///Ticket #2804:  When we open a database in the newer file format,
			///clear the legacy_file_format pragma flag so that a VACUUM will
			///not downgrade the database and thus invalidate any descending
			///indices that the user might have created.
			///
			///</summary>
            if (iDb == 0 && meta[BTreeProp.FILE_FORMAT - 1] >= 4)
            {
                db.flags &= ~SqliteFlags.SQLITE_LegacyFileFmt;
			}
			///
			///<summary>
			///Read the schema information out of the schema tables
			///
			///</summary>
			Debug.Assert(db.init.busy!=0);
			{
				string zSql;
				zSql=io.sqlite3MPrintf(db,"SELECT name, rootpage, sql FROM '%q'.%s ORDER BY rowid",db.aDb[iDb].zName,zMasterName);
				#if !SQLITE_OMIT_AUTHORIZATION
																																																																																																{
int (*xAuth)(void*,int,const char*,const char*,const char*,const char*);
xAuth = db.xAuth;
db.xAuth = 0;
#endif
				rc=legacy.sqlite3_exec(db,zSql,(dxCallback)sqlite3InitCallback,initData,0);
				pzErrMsg=initData.pzErrMsg;
				#if !SQLITE_OMIT_AUTHORIZATION
																																																																																																db.xAuth = xAuth;
}
#endif
				if(rc==SqlResult.SQLITE_OK)
					rc=initData.rc;
				db.sqlite3DbFree(ref zSql);
				#if !SQLITE_OMIT_ANALYZE
				if(rc==SqlResult.SQLITE_OK) {
					sqlite3AnalysisLoad(db,iDb);
				}
				#endif
				//if ( db.mallocFailed != 0 )
				//{
				//  rc = SQLITE_NOMEM;
				//  build.sqlite3ResetInternalSchema( db, -1 );
				//}
				///
				///<summary>
				///Jump here for an error that occurs after successfully allocating
				///curMain and calling sqlite3BtreeEnter(). For an error that occurs
				///before that point, jump to error_out.
				///</summary>
			}
            if (rc == SqlResult.SQLITE_OK || (db.flags & SqliteFlags.SQLITE_RecoveryMode) != 0)
            {
				///
				///<summary>
				///Black magic: If the SQLITE_RecoveryMode flag is set, then consider
				///the schema loaded, even if errors occurred. In this situation the
				///current sqlite3_prepare() operation will fail, but the following one
				///will attempt to compile the supplied statement against whatever subset
				///of the schema was loaded before the error occurred. The primary
				///purpose of this is to allow access to the sqlite_master table
				///even when its contents have been corrupted.
				///
				///</summary>
				db.DbSetProperty(iDb,sqliteinth.DB_SchemaLoaded);
				rc=SqlResult.SQLITE_OK;
			}
			initone_error_out:
			if(openedTransaction!=0) {
				pDb.pBt.sqlite3BtreeCommit();
			}
            
            pDb.pBt.sqlite3BtreeLeave();
			error_out:
			if(rc== SqlResult.SQLITE_NOMEM || rc== SqlResult.SQLITE_IOERR_NOMEM) {
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
		static SqlResult sqlite3Init(sqlite3 db,ref string pzErrMsg) {
            int i;
            SqlResult rc;
            bool commit_internal = !((db.flags & SqliteFlags.SQLITE_InternChanges) != 0);
			Debug.Assert(db.mutex.sqlite3_mutex_held());
			rc=SqlResult.SQLITE_OK;
			db.init.busy=1;
			for(i=0;rc==SqlResult.SQLITE_OK&&i<db.nDb;i++) {
				if(db.DbHasProperty(i,sqliteinth.DB_SchemaLoaded)||i==1)
					continue;
				rc=sqlite3InitOne(db,i,ref pzErrMsg);
				if(rc!=0) {
					build.sqlite3ResetInternalSchema(db,i);
				}
			}
			///
			///<summary>
			///Once all the other databases have been initialised, load the schema
			///for the TEMP database. This is loaded last, as the TEMP database
			///schema may contain references to objects in other databases.
			///
			///</summary>
			#if !SQLITE_OMIT_TEMPDB
			if(rc==SqlResult.SQLITE_OK&&Sqlite3.ALWAYS(db.nDb>1)&&!db.DbHasProperty(1,sqliteinth.DB_SchemaLoaded)) {
				rc=sqlite3InitOne(db,1,ref pzErrMsg);
				if(rc!=0) {
					build.sqlite3ResetInternalSchema(db,1);
				}
			}
			#endif
			db.init.busy=0;
			if(rc==SqlResult.SQLITE_OK&&commit_internal) {
				build.sqlite3CommitInternalChanges(db);
			}
			return rc;
		}
		///<summary>
		/// This routine is a no-op if the database schema is already initialised.
		/// Otherwise, the schema is loaded. An error code is returned.
		///
		///</summary>
		public static SqlResult sqlite3ReadSchema(Parse pParse) {
			SqlResult rc=SqlResult.SQLITE_OK;
			sqlite3 db=pParse.db;
			Debug.Assert(db.mutex.sqlite3_mutex_held());
			if(0==db.init.busy) {
				rc=(SqlResult)sqlite3Init(db,ref pParse.zErrMsg);
			}
			if(rc!=SqlResult.SQLITE_OK) {
				pParse.rc=rc;
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
		static void schemaIsValid(Parse pParse) {
			sqlite3 db=pParse.db;
			int iDb;
            SqlResult rc;
			u32 cookie=0;
			Debug.Assert(pParse.checkSchema!=0);
			Debug.Assert(db.mutex.sqlite3_mutex_held());
			for(iDb=0;iDb<db.nDb;iDb++) {
				int openedTransaction=0;
				///
				///<summary>
				///True if a transaction is opened 
				///</summary>
				Btree pBt=db.aDb[iDb].pBt;
				///
				///<summary>
				///Btree database to read cookie from 
				///</summary>
				if(pBt==null)
					continue;
				///
				///<summary>
				///</summary>
				///<param name="If there is not already a read">write) transaction opened</param>
				///<param name="on the b">tree database, open one now. If a transaction is opened, it </param>
				///<param name="will be closed immediately after reading the meta">value. </param>
				if(!pBt.sqlite3BtreeIsInReadTrans()) {
					rc=pBt.sqlite3BtreeBeginTrans(0);
					//if ( rc == SQLITE_NOMEM || rc == SQLITE_IOERR_NOMEM )
					//{
					//    db.mallocFailed = 1;
					//}
					if(rc!=SqlResult.SQLITE_OK)
						return;
					openedTransaction=1;
				}
				///
				///<summary>
				///Read the schema cookie from the database. If it does not match the 
				///</summary>
				///<param name="value stored as part of the in">memory schema representation,</param>
				///<param name="set Parse.rc to SQLITE_SCHEMA. ">set Parse.rc to SQLITE_SCHEMA. </param>
                cookie = pBt.sqlite3BtreeGetMeta(BTreeProp.SCHEMA_VERSION);
				Debug.Assert(sqlite3SchemaMutexHeld(db,iDb,null));
				if(cookie!=db.aDb[iDb].pSchema.schema_cookie) {
					build.sqlite3ResetInternalSchema(db,iDb);
					pParse.rc=SqlResult.SQLITE_SCHEMA;
				}
				///
				///<summary>
				///Close the transaction, if one was opened. 
				///</summary>
				if(openedTransaction!=0) {
					pBt.sqlite3BtreeCommit();
				}
			}
		}
		///<summary>
		/// Convert a schema pointer into the iDb index that indicates
		/// which database file in db.aDb[] the schema refers to.
		///
		/// If the same database is attached more than once, the first
		/// attached database is returned.
		///
		///</summary>
		public static int sqlite3SchemaToIndex(sqlite3 db,Schema pSchema) {//TODO: extension method
			int i=-1000000;
			///
			///<summary>
			///</summary>
			///<param name="If pSchema is NULL, then return ">1000000. This happens when code in</param>
			///<param name="expr.c is trying to resolve a reference to a transient table (i.e. one">expr.c is trying to resolve a reference to a transient table (i.e. one</param>
			///<param name="created by a sub">select). In this case the return value of this</param>
			///<param name="function should never be used.">function should never be used.</param>
			///<param name=""></param>
			///<param name="We return ">1 simply because using</param>
			///<param name="">>aDb[] is much</param>
			///<param name="more likely to cause a segfault than ">1 (of course there are assert()</param>
			///<param name="statements too, but it never hurts to play the odds).">statements too, but it never hurts to play the odds).</param>
			///<param name=""></param>
			Debug.Assert(db.mutex.sqlite3_mutex_held());
			if(pSchema!=null) {
				for(i=0;Sqlite3.ALWAYS(i<db.nDb);i++) {
					if(db.aDb[i].pSchema==pSchema) {
						break;
					}
				}
				Debug.Assert(i>=0&&i<db.nDb);
			}
			return i;
		}
		///<summary>
		/// Compile the UTF-8 encoded SQL statement zSql into a statement handle.
		///
		///</summary>
		static SqlResult sqlite3Prepare(sqlite3 db,///
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
		) {
			Parse pParse;
			///
			///<summary>
			///Parsing context 
			///</summary>
			string zErrMsg="";
			///
			///<summary>
			///Error message 
			///</summary>
			var rc=SqlResult.SQLITE_OK;
			///
			///<summary>
			///Result code 
			///</summary>
			int i;
			///
			///<summary>
			///Loop counter 
			///</summary>
			ppStmt=null;
			pzTail=null;
			///
			///<summary>
			///Allocate the parsing context 
			///</summary>
			pParse=new Parse();
			//sqlite3StackAllocZero(db, sizeof(*pParse));
			//if ( pParse == null )
			//{
			//  rc = SQLITE_NOMEM;
			//  goto end_prepare;
			//}
			pParse.pReprepare=pReprepare;
			pParse.sLastToken.zRestSql="";
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
			for(i=0;i<db.nDb;i++) {
				Btree pBt=db.aDb[i].pBt;
				if(pBt!=null) {
					Debug.Assert(sqlite3BtreeHoldsMutex(pBt));
					rc=pBt.sqlite3BtreeSchemaLocked();
					if(rc!=0) {
						string zDb=db.aDb[i].zName;
						utilc.sqlite3Error(db,rc,"database schema is locked: %s",zDb);
                        sqliteinth.testcase(db.flags & SqliteFlags.SQLITE_ReadUncommitted);
						goto end_prepare;
					}
				}
			}
            vtab.sqlite3VtabUnlockList(db);
			pParse.db=db;
			pParse.nQueryLoop=(double)1;
			if(nBytes>=0&&(nBytes==0||zSql[nBytes-1]!=0)) {
				string zSqlCopy;
				int mxLen=db.aLimit[Globals.SQLITE_LIMIT_SQL_LENGTH];
				sqliteinth.testcase(nBytes==mxLen);
				sqliteinth.testcase(nBytes==mxLen+1);
				if(nBytes>mxLen) {
					utilc.sqlite3Error(db, SqlResult.SQLITE_TOOBIG, "statement too long");
					rc=malloc_cs.sqlite3ApiExit(db, SqlResult.SQLITE_TOOBIG);
					goto end_prepare;
				}
				zSqlCopy=zSql.Substring(0,nBytes);
				// sqlite3DbStrNDup(db, zSql, nBytes);
				if(zSqlCopy!=null) {
					pParse.sqlite3RunParser(zSqlCopy,ref zErrMsg);
					db.sqlite3DbFree(ref zSqlCopy);
					//pParse->zTail = &zSql[pParse->zTail-zSqlCopy];
				}
				else {
					//pParse->zTail = &zSql[nBytes];
				}
			}
			else {
				pParse.sqlite3RunParser(zSql,ref zErrMsg);
			}
			Debug.Assert(1==(int)pParse.nQueryLoop);
			//if ( db.mallocFailed != 0 )
			//{
			//  pParse.rc = SQLITE_NOMEM;
			//}
			if(pParse.rc==SqlResult.SQLITE_DONE)
				pParse.rc=SqlResult.SQLITE_OK;
			if(pParse.checkSchema!=0) {
				schemaIsValid(pParse);
			}
			//if ( db.mallocFailed != 0 )
			//{
			//  pParse.rc = SQLITE_NOMEM;
			//}
			//if (pzTail != null)
			{
				pzTail=pParse.zTail==null?"":pParse.zTail.ToString();
				#if !SQLITE_OMIT_EXPLAIN
				#endif
				///
				///<summary>
				///Delete any TriggerPrg structures allocated while parsing this statement. 
				///</summary>
				//sqlite3StackFree( db, pParse );
			}
			rc=pParse.rc;
			if(rc==SqlResult.SQLITE_OK&&pParse.pVdbe!=null&&pParse.explain!=0) {
				string[] azColName=new string[] {
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
				int iFirst,mx;
				if(pParse.explain==2) {
					pParse.pVdbe.sqlite3VdbeSetNumCols(4);
					iFirst=8;
					mx=12;
				}
				else {
					pParse.pVdbe.sqlite3VdbeSetNumCols(8);
					iFirst=0;
					mx=8;
				}
				for(i=iFirst;i<mx;i++) {
                    pParse.pVdbe.sqlite3VdbeSetColName(i - iFirst, ColName.NAME, azColName[i], SQLITE_STATIC);
				}
			}
			Debug.Assert(db.init.busy==0||saveSqlFlag==0);
			if(db.init.busy==0) {
				Vdbe pVdbe=pParse.pVdbe;
                vdbeaux.sqlite3VdbeSetSql(pVdbe, zSql, (int)(zSql.Length - (pParse.zTail == null ? 0 : pParse.zTail.Length)), saveSqlFlag);
			}
			if(pParse.pVdbe!=null&&(rc!=SqlResult.SQLITE_OK///
			///<summary>
			///|| db.mallocFailed != 0 
			///</summary>
			)) {
                vdbeaux.sqlite3VdbeFinalize(ref pParse.pVdbe);
				//Debug.Assert( ppStmt == null );
			}
			else {
				ppStmt=pParse.pVdbe;
			}
			if(zErrMsg!="") {
				utilc.sqlite3Error(db,rc,"%s",zErrMsg);
				db.sqlite3DbFree(ref zErrMsg);
			}
			else {
				utilc.sqlite3Error(db,rc,0);
			}
			while(pParse.pTriggerPrg!=null) {
				TriggerPrg pT=pParse.pTriggerPrg;
				pParse.pTriggerPrg=pT.pNext;
				db.sqlite3DbFree(ref pT);
			}
			end_prepare:
			rc=malloc_cs.sqlite3ApiExit(db,rc);
			Debug.Assert((rc&db.errMask)==rc);
			return rc;
		}
		//C# Version w/o End of Parsed String
		static SqlResult sqlite3LockAndPrepare(sqlite3 db,///
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
		) {
			string sOut=null;
			return sqlite3LockAndPrepare(db,zSql,nBytes,saveSqlFlag,pOld,ref ppStmt,ref sOut);
		}
		static SqlResult sqlite3LockAndPrepare(sqlite3 db,///
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
		ref string pzTail///
		///<summary>
		///OUT: End of parsed string 
		///</summary>
		) {
            SqlResult rc;
			//  assert( ppStmt!=0 );
            if (!utilc.sqlite3SafetyCheckOk(db))
            {
				ppStmt=null;
				pzTail=null;
				return sqliteinth.SQLITE_MISUSE_BKPT();
			}
			db.mutex.sqlite3_mutex_enter();
            db.sqlite3BtreeEnterAll();
			rc=sqlite3Prepare(db,zSql,nBytes,saveSqlFlag,pOld,ref ppStmt,ref pzTail);
			if(rc== SqlResult.SQLITE_SCHEMA) {
                vdbeapi.sqlite3_finalize(ppStmt);
				rc=sqlite3Prepare(db,zSql,nBytes,saveSqlFlag,pOld,ref ppStmt,ref pzTail);
			}
			sqlite3BtreeLeaveAll(db);
			db.mutex.sqlite3_mutex_leave();
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
		public static SqlResult sqlite3Reprepare(Vdbe p) {
            SqlResult rc;
			sqlite3_stmt pNew=new sqlite3_stmt();
			string zSql;
			sqlite3 db;
            Debug.Assert(p.sqlite3VdbeDb().mutex.sqlite3_mutex_held());
            zSql = vdbeaux.sqlite3_sql((sqlite3_stmt)p);
			Debug.Assert(zSql!=null);
			///
			///<summary>
			///Reprepare only called for prepare_v2() statements 
			///</summary>
			db=p.sqlite3VdbeDb();
			Debug.Assert(db.mutex.sqlite3_mutex_held());
			rc=sqlite3LockAndPrepare(db,zSql,-1,0,p,ref pNew,0);
			if(rc!=0) {
				if(rc== SqlResult.SQLITE_NOMEM) {
					//        db.mallocFailed = 1;
				}
				Debug.Assert(pNew==null);
				return rc;
			}
			else {
				Debug.Assert(pNew!=null);
			}
            vdbeaux.sqlite3VdbeSwap((Vdbe)pNew, p);
            vdbeapi.sqlite3TransferBindings(pNew, (sqlite3_stmt)p);
			((Vdbe)pNew).sqlite3VdbeResetStepResult();
            vdbeaux.sqlite3VdbeFinalize(ref pNew);
			return SqlResult.SQLITE_OK;
		}
		//C# Overload for ignore error out
		static public SqlResult sqlite3_prepare(sqlite3 db,///
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
		int dummy///
		///<summary>
		///OUT: End of parsed string 
		///</summary>
		) {
			string sOut=null;
			return sqlite3_prepare(db,zSql,nBytes,ref ppStmt,ref sOut);
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
		static public SqlResult sqlite3_prepare(sqlite3 db,///
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
		) {
            SqlResult rc;
			rc=sqlite3LockAndPrepare(db,zSql,nBytes,0,null,ref ppStmt,ref pzTail);
			Debug.Assert(rc==SqlResult.SQLITE_OK||ppStmt==null);
			///
			///<summary>
			///VERIFY: F13021 
			///</summary>
			return rc;
		}
		public static SqlResult sqlite3_prepare_v2(sqlite3 db,///
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
		int dummy///
		///<summary>
		///( No string passed) 
		///</summary>
		) {
			string pzTail=null;
            SqlResult rc;
			rc=sqlite3LockAndPrepare(db,zSql,nBytes,1,null,ref ppStmt,ref pzTail);
			Debug.Assert(rc==SqlResult.SQLITE_OK||ppStmt==null);
			///
			///<summary>
			///VERIFY: F13021 
			///</summary>
			return rc;
		}
		public static SqlResult sqlite3_prepare_v2(sqlite3 db,///
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
		) {
            SqlResult rc;
			rc=sqlite3LockAndPrepare(db,zSql,nBytes,1,null,ref ppStmt,ref pzTail);
			Debug.Assert(rc==SqlResult.SQLITE_OK||ppStmt==null);
			///
			///<summary>
			///VERIFY: F13021 
			///</summary>
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
