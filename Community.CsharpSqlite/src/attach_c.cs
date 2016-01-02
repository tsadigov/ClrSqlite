using System;
using System.Diagnostics;
using System.Text;
using u8=System.Byte;
using u32=System.UInt32;
namespace Community.CsharpSqlite {
    using sqlite3_value = Engine.Mem;
    using tree;
    using Community.CsharpSqlite.Os;
    using Community.CsharpSqlite.Metadata;
    using Community.CsharpSqlite.Engine;
    using Community.CsharpSqlite.Paging;
	public partial class Sqlite3 {
		#if !SQLITE_OMIT_ATTACH
		///<summary>
		/// An SQL user-function registered to do the work of an ATTACH statement. The
		/// three arguments to the function come directly from an attach statement:
		///
		///     ATTACH DATABASE x AS y KEY z
		///
		///     SELECT sqlite_attach(x, y, z)
		///
		/// If the optional "KEY z" syntax is omitted, an SQL NULL is passed as the
		/// third argument.
		///</summary>
		static void attachFunc(sqlite3_context context,int NotUsed,sqlite3_value[] argv) {
			int i;
            SqlResult rc =(SqlResult)0;
			Connection db=vdbeapi.sqlite3_context_db_handle(context);
			string zName;
			string zFile;
			string zPath="";
			string zErr="";
			int flags;
			DbBackend aNew=null;
			string zErrDyn="";
			sqlite3_vfs pVfs=null;
			sqliteinth.UNUSED_PARAMETER(NotUsed);
			zFile=argv[0].AsString!=null&&(argv[0].AsString.Length>0)&&argv[0].flags!=MemFlags.MEM_Null?vdbeapi.sqlite3_value_text(argv[0]):"";
			zName=argv[1].AsString!=null&&(argv[1].AsString.Length>0)&&argv[1].flags!=MemFlags.MEM_Null?vdbeapi.sqlite3_value_text(argv[1]):"";
			//if( zFile==null ) zFile = "";
			//if ( zName == null ) zName = "";
			///
			///<summary>
			///Check for the following errors:
			///
			///Too many attached databases,
			///Transaction currently open
			///Specified database name already being used.
			///
			///</summary>
			if(db.BackendCount>=db.aLimit[Globals.SQLITE_LIMIT_ATTACHED]+2) {
				zErrDyn=io.sqlite3MPrintf(db,"too many attached databases - max %d",db.aLimit[Globals.SQLITE_LIMIT_ATTACHED]);
				goto attach_error;
			}
			if(0==db.autoCommit) {
				zErrDyn=io.sqlite3MPrintf(db,"cannot ATTACH database within transaction");
				goto attach_error;
			}
			for(i=0;i<db.BackendCount;i++) {
				string z=db.Backends[i].Name;
				Debug.Assert(z!=null&&zName!=null);
				if(z.Equals(zName,StringComparison.InvariantCultureIgnoreCase)) {
					zErrDyn=io.sqlite3MPrintf(db,"database %s is already in use",zName);
					goto attach_error;
				}
			}
			///
			///<summary>
			///Allocate the new entry in the db.aDb[] array and initialise the schema
			///hash tables.
			///
			///</summary>
			///
			///<summary>
			///Allocate the new entry in the db.aDb[] array and initialise the schema
			///hash tables.
			///
			///</summary>
			//if( db.aDb==db.aDbStatic ){
			//  aNew = sqlite3DbMallocRaw(db, sizeof(db.aDb[0])*3 );
			//  if( aNew==0 ) return;
			//  memcpy(aNew, db.aDb, sizeof(db.aDb[0])*2);
			//}else {
			if(db.Backends.Length<=db.BackendCount)
				Array.Resize(ref db.Backends,db.BackendCount+1);
			//aNew = sqlite3DbRealloc(db, db.aDb, sizeof(db.aDb[0])*(db.nDb+1) );
			if(db.Backends==null)
				return;
			// if( aNew==0 ) return;
			//}
			db.Backends[db.BackendCount]=new DbBackend();
			//db.aDb = aNew;
			aNew=db.Backends[db.BackendCount];
			//memset(aNew, 0, sizeof(*aNew));
			//  memset(aNew, 0, sizeof(*aNew));
			///
			///<summary>
			///Open the database file. If the btree is successfully opened, use
			///it to obtain the database schema. At this point the schema may
			///or may not be initialised.
			///
			///</summary>
			flags=(int)db.openFlags;
			rc=sqlite3ParseUri(db.pVfs.zName,zFile,ref flags,ref pVfs,ref zPath,ref zErr);
			if(rc!=SqlResult.SQLITE_OK) {
				//if ( rc == SQLITE_NOMEM )
				//db.mallocFailed = 1;
				context.sqlite3_result_error(zErr,-1);
				//malloc_cs.sqlite3_free( zErr );
				return;
			}
			Debug.Assert(pVfs!=null);
			flags|=SQLITE_OPEN_MAIN_DB;
			rc=Btree.Open(pVfs,zPath,db,ref aNew.BTree,0,(int)flags);
			//malloc_cs.sqlite3_free( zPath );
			db.BackendCount++;
            if (rc == SqlResult.SQLITE_CONSTRAINT)
            {
				rc=SqlResult.SQLITE_ERROR;
				zErrDyn=io.sqlite3MPrintf(db,"database is already attached");
			}
			else
				if(rc==SqlResult.SQLITE_OK) {
					Pager pPager;
					aNew.pSchema= aNew.BTree.sqlite3SchemaGet(db);
					//if ( aNew.pSchema == null )
					//{
					//  rc = SQLITE_NOMEM;
					//}
					//else 
					if(aNew.pSchema.file_format!=0&&aNew.pSchema.enc!=sqliteinth.ENC(db)) {
						zErrDyn=io.sqlite3MPrintf(db,"attached databases must use the same text encoding as main database");
						rc=SqlResult.SQLITE_ERROR;
					}
					pPager=aNew.BTree.sqlite3BtreePager();
					pPager.sqlite3PagerLockingMode(db.dfltLockMode);
					aNew.BTree.sqlite3BtreeSecureDelete(db.Backends[0].BTree.sqlite3BtreeSecureDelete(-1));
				}
			aNew.safety_level=3;
			aNew.Name=zName;
			//sqlite3DbStrDup(db, zName);
			//if( rc==SqlResult.SQLITE_OK && aNew.zName==0 ){
			//  rc = SQLITE_NOMEM;
			//}
			#if SQLITE_HAS_CODEC
			if(rc==SqlResult.SQLITE_OK) {
				//extern int sqlite3CodecAttach(sqlite3*, int, const void*, int);
				//extern void sqlite3CodecGetKey(sqlite3*, int, void**, int*);
				int nKey;
				string zKey;
                FoundationalType t = vdbeapi.sqlite3_value_type(argv[2]);
				switch(t) {
                    case FoundationalType.SQLITE_INTEGER:
                    case FoundationalType.SQLITE_FLOAT:
				zErrDyn="Invalid key value";
				//sqlite3DbStrDup( db, "Invalid key value" );
				rc=SqlResult.SQLITE_ERROR;
				break;
                    case FoundationalType.SQLITE_TEXT:
                    case FoundationalType.SQLITE_BLOB:
				nKey=vdbeapi.sqlite3_value_bytes(argv[2]);
                zKey = vdbeapi.sqlite3_value_blob(argv[2]).ToString();
				// (char *)sqlite3_value_blob(argv[2]);
                rc = crypto.sqlite3CodecAttach(db, db.BackendCount - 1, zKey, nKey);
				break;
                    case FoundationalType.SQLITE_NULL:
				///
				///<summary>
				///No key specified.  Use the key from the main database 
				///</summary>
				crypto.sqlite3CodecGetKey(db,0,out zKey,out nKey);
				//sqlite3CodecGetKey(db, 0, (void**)&zKey, nKey);
				if(nKey>0||db.Backends[0].BTree.GetReserve()>0) {
                    rc = crypto.sqlite3CodecAttach(db, db.BackendCount - 1, zKey, nKey);
				}
				break;
				}
			}
			#endif
			///
			///<summary>
			///If the file was opened successfully, read the schema for the new database.
			///If this fails, or if opening the file failed, then close the file and
			///remove the entry from the db.aDb[] array. i.e. put everything back the way
			///we found it.
			///</summary>
			if(rc==SqlResult.SQLITE_OK) {
                db.sqlite3BtreeEnterAll();
				rc=db.InitialiseAllDatabases(ref zErrDyn);
				sqlite3BtreeLeaveAll(db);
			}
			if(rc!=0) {
				int iDb=db.BackendCount-1;
				Debug.Assert(iDb>=2);
				if(db.Backends[iDb].BTree!=null) {
					BTreeMethods.sqlite3BtreeClose(ref db.Backends[iDb].BTree);
					db.Backends[iDb].BTree=null;
					db.Backends[iDb].pSchema=null;
				}
				build.sqlite3ResetInternalSchema(db,-1);
				db.BackendCount=iDb;
                if (rc == SqlResult.SQLITE_NOMEM || rc == SqlResult.SQLITE_IOERR_NOMEM)
                {
					////        db.mallocFailed = 1;
					db.DbFree(ref zErrDyn);
					zErrDyn=io.sqlite3MPrintf(db,"out of memory");
				}
				else
					if(zErrDyn=="") {
						zErrDyn=io.sqlite3MPrintf(db,"unable to open database: %s",zFile);
					}
				goto attach_error;
			}
			return;
			attach_error:
			///
			///<summary>
			///Return an error if we get here 
			///</summary>
			if(zErrDyn!="") {
				context.sqlite3_result_error(zErrDyn,-1);
				db.DbFree(ref zErrDyn);
			}
			if(rc!=0)
				context.sqlite3_result_error_code(rc);
		}
		///<summary>
		/// An SQL user-function registered to do the work of an DETACH statement. The
		/// three arguments to the function come directly from a detach statement:
		///
		///     DETACH DATABASE x
		///
		///     SELECT sqlite_detach(x)
		///</summary>
		static void detachFunc(sqlite3_context context,int NotUsed,sqlite3_value[] argv) {
			string zName=zName=argv[0].AsString!=null&&(argv[0].AsString.Length>0)?vdbeapi.sqlite3_value_text(argv[0]):"";
			//(vdbeapi.sqlite3_value_text(argv[0]);
			Connection db=vdbeapi.sqlite3_context_db_handle(context);
			int i;
			DbBackend pDb=null;
			StringBuilder zErr=new StringBuilder(200);
			sqliteinth.UNUSED_PARAMETER(NotUsed);
			if(zName==null)
				zName="";
			for(i=0;i<db.BackendCount;i++) {
				pDb=db.Backends[i];
				if(pDb.BTree==null)
					continue;
				if(pDb.Name.Equals(zName,StringComparison.InvariantCultureIgnoreCase))
					break;
			}
			if(i>=db.BackendCount) {
				 io.sqlite3_snprintf(200,zErr,"no such database: %s",zName);
				goto detach_error;
			}
			if(i<2) {
				io.sqlite3_snprintf(200,zErr,"cannot detach database %s",zName);
				goto detach_error;
			}
			if(0==db.autoCommit) {
				io.sqlite3_snprintf(200,zErr,"cannot DETACH database within transaction");
				goto detach_error;
			}
			if(pDb.BTree.sqlite3BtreeIsInReadTrans()||pDb.BTree.sqlite3BtreeIsInBackup()) {
				io.sqlite3_snprintf(200,zErr,"database %s is locked",zName);
				goto detach_error;
			}
			BTreeMethods.sqlite3BtreeClose(ref pDb.BTree);
			pDb.BTree=null;
			pDb.pSchema=null;
			build.sqlite3ResetInternalSchema(db,-1);
			return;
			detach_error:
			context.sqlite3_result_error(zErr.ToString(),-1);
		}
		///<summary>
		/// Called by the parser to compile a DETACH statement.
		///
		///     DETACH pDbname
		///</summary>
		static FuncDef detach_func=new FuncDef(1,///
		///<summary>
		///nArg 
		///</summary>
		SqliteEncoding.UTF8,///
		///<summary>
		///iPrefEnc 
		///</summary>
		0,///
		///<summary>
		///flags 
		///</summary>
		null,///
		///<summary>
		///pUserData 
		///</summary>
		null,///
		///<summary>
		///pNext 
		///</summary>
		detachFunc,///
		///<summary>
		///xFunc 
		///</summary>
		null,///
		///<summary>
		///xStep 
		///</summary>
		null,///
		///<summary>
		///xFinalize 
		///</summary>
		"sqlite_detach",///
		///<summary>
		///zName 
		///</summary>
		null,///
		///<summary>
		///pHash 
		///</summary>
		null///
		///<summary>
		///pDestructor 
		///</summary>
		);
		///<summary>
		/// Called by the parser to compile an ATTACH statement.
		///
		///     ATTACH p AS pDbname KEY pKey
		///</summary>
		static FuncDef attach_func=new FuncDef(3,///
		///<summary>
		///nArg 
		///</summary>
		SqliteEncoding.UTF8,///
		///<summary>
		///iPrefEnc 
		///</summary>
		0,///
		///<summary>
		///flags 
		///</summary>
		null,///
		///<summary>
		///pUserData 
		///</summary>
		null,///
		///<summary>
		///pNext 
		///</summary>
		attachFunc,///
		///<summary>
		///xFunc 
		///</summary>
		null,///
		///<summary>
		///xStep 
		///</summary>
		null,///
		///<summary>
		///xFinalize 
		///</summary>
		"sqlite_attach",///
		///<summary>
		///zName 
		///</summary>
		null,///
		///<summary>
		///pHash 
		///</summary>
		null///
		///<summary>
		///pDestructor 
		///</summary>
		);
	#endif
	#if !SQLITE_OMIT_VIEW || !SQLITE_OMIT_TRIGGER
	#endif
	#if !SQLITE_OMIT_TRIGGER
	#endif
	}
}
