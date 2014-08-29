using System;
using System.Diagnostics;
using System.Text;
using u8=System.Byte;
using u32=System.UInt32;
namespace Community.CsharpSqlite {
	using sqlite3_value=Sqlite3.Mem;
	public partial class Sqlite3 {
		///<summary>
		/// 2003 April 6
		///
		/// The author disclaims copyright to this source code.  In place of
		/// a legal notice, here is a blessing:
		///
		///    May you do good and not evil.
		///    May you find forgiveness for yourself and forgive others.
		///    May you share freely, never taking more than you give.
		///
		///
		/// This file contains code used to implement the ATTACH and DETACH commands.
		///
		///  Included in SQLite3 port to C#-SQLite;  2008 Noah B Hart
		///  C#-SQLite is an independent reimplementation of the SQLite software library
		///
		///  SQLITE_SOURCE_ID: 2011-06-23 19:49:22 4374b7e83ea0a3fbc3691f9c0c936272862f32f2
		///
		///
		///</summary>
		//#include "sqliteInt.h"
		#if !SQLITE_OMIT_ATTACH
		///<summary>
		/// Resolve an expression that was part of an ATTACH or DETACH statement. This
		/// is slightly different from resolving a normal SQL expression, because simple
		/// identifiers are treated as strings, not possible column names or aliases.
		///
		/// i.e. if the parser sees:
		///
		///     ATTACH DATABASE abc AS def
		///
		/// it treats the two expressions as literal strings 'abc' and 'def' instead of
		/// looking for columns of the same name.
		///
		/// This only applies to the root node of pExpr, so the statement:
		///
		///     ATTACH DATABASE abc||def AS 'db2'
		///
		/// will fail because neither abc or def can be resolved.
		///</summary>
		static int resolveAttachExpr(NameContext pName,Expr pExpr) {
			int rc=SQLITE_OK;
			if(pExpr!=null) {
				if(pExpr.op!=TK_ID) {
					rc=sqlite3ResolveExprNames(pName,ref pExpr);
					if(rc==SQLITE_OK&&sqlite3ExprIsConstant(pExpr)==0) {
						sqlite3ErrorMsg(pName.pParse,"invalid name: \"%s\"",pExpr.u.zToken);
						return SQLITE_ERROR;
					}
				}
				else {
					pExpr.op=TK_STRING;
				}
			}
			return rc;
		}
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
			int rc=0;
			sqlite3 db=sqlite3_context_db_handle(context);
			string zName;
			string zFile;
			string zPath="";
			string zErr="";
			int flags;
			Db aNew=null;
			string zErrDyn="";
			sqlite3_vfs pVfs=null;
			UNUSED_PARAMETER(NotUsed);
			zFile=argv[0].z!=null&&(argv[0].z.Length>0)&&argv[0].flags!=MEM_Null?sqlite3_value_text(argv[0]):"";
			zName=argv[1].z!=null&&(argv[1].z.Length>0)&&argv[1].flags!=MEM_Null?sqlite3_value_text(argv[1]):"";
			//if( zFile==null ) zFile = "";
			//if ( zName == null ) zName = "";
			/* Check for the following errors:
  **
  **     * Too many attached databases,
  **     * Transaction currently open
  **     * Specified database name already being used.
  */if(db.nDb>=db.aLimit[SQLITE_LIMIT_ATTACHED]+2) {
				zErrDyn=sqlite3MPrintf(db,"too many attached databases - max %d",db.aLimit[SQLITE_LIMIT_ATTACHED]);
				goto attach_error;
			}
			if(0==db.autoCommit) {
				zErrDyn=sqlite3MPrintf(db,"cannot ATTACH database within transaction");
				goto attach_error;
			}
			for(i=0;i<db.nDb;i++) {
				string z=db.aDb[i].zName;
				Debug.Assert(z!=null&&zName!=null);
				if(z.Equals(zName,StringComparison.InvariantCultureIgnoreCase)) {
					zErrDyn=sqlite3MPrintf(db,"database %s is already in use",zName);
					goto attach_error;
				}
			}
			/* Allocate the new entry in the db.aDb[] array and initialise the schema
  ** hash tables.
  *//* Allocate the new entry in the db.aDb[] array and initialise the schema
  ** hash tables.
  *///if( db.aDb==db.aDbStatic ){
			//  aNew = sqlite3DbMallocRaw(db, sizeof(db.aDb[0])*3 );
			//  if( aNew==0 ) return;
			//  memcpy(aNew, db.aDb, sizeof(db.aDb[0])*2);
			//}else {
			if(db.aDb.Length<=db.nDb)
				Array.Resize(ref db.aDb,db.nDb+1);
			//aNew = sqlite3DbRealloc(db, db.aDb, sizeof(db.aDb[0])*(db.nDb+1) );
			if(db.aDb==null)
				return;
			// if( aNew==0 ) return;
			//}
			db.aDb[db.nDb]=new Db();
			//db.aDb = aNew;
			aNew=db.aDb[db.nDb];
			//memset(aNew, 0, sizeof(*aNew));
			//  memset(aNew, 0, sizeof(*aNew));
			/* Open the database file. If the btree is successfully opened, use
  ** it to obtain the database schema. At this point the schema may
  ** or may not be initialised.
  */flags=(int)db.openFlags;
			rc=sqlite3ParseUri(db.pVfs.zName,zFile,ref flags,ref pVfs,ref zPath,ref zErr);
			if(rc!=SQLITE_OK) {
				//if ( rc == SQLITE_NOMEM )
				//db.mallocFailed = 1;
				sqlite3_result_error(context,zErr,-1);
				//sqlite3_free( zErr );
				return;
			}
			Debug.Assert(pVfs!=null);
			flags|=SQLITE_OPEN_MAIN_DB;
			rc=sqlite3BtreeOpen(pVfs,zPath,db,ref aNew.pBt,0,(int)flags);
			//sqlite3_free( zPath );
			db.nDb++;
			if(rc==SQLITE_CONSTRAINT) {
				rc=SQLITE_ERROR;
				zErrDyn=sqlite3MPrintf(db,"database is already attached");
			}
			else
				if(rc==SQLITE_OK) {
					Pager pPager;
					aNew.pSchema=sqlite3SchemaGet(db,aNew.pBt);
					//if ( aNew.pSchema == null )
					//{
					//  rc = SQLITE_NOMEM;
					//}
					//else 
					if(aNew.pSchema.file_format!=0&&aNew.pSchema.enc!=ENC(db)) {
						zErrDyn=sqlite3MPrintf(db,"attached databases must use the same text encoding as main database");
						rc=SQLITE_ERROR;
					}
					pPager=sqlite3BtreePager(aNew.pBt);
					pPager.sqlite3PagerLockingMode(db.dfltLockMode);
					sqlite3BtreeSecureDelete(aNew.pBt,sqlite3BtreeSecureDelete(db.aDb[0].pBt,-1));
				}
			aNew.safety_level=3;
			aNew.zName=zName;
			//sqlite3DbStrDup(db, zName);
			//if( rc==SQLITE_OK && aNew.zName==0 ){
			//  rc = SQLITE_NOMEM;
			//}
			#if SQLITE_HAS_CODEC
			if(rc==SQLITE_OK) {
				//extern int sqlite3CodecAttach(sqlite3*, int, const void*, int);
				//extern void sqlite3CodecGetKey(sqlite3*, int, void**, int*);
				int nKey;
				string zKey;
				int t=sqlite3_value_type(argv[2]);
				switch(t) {
				case SQLITE_INTEGER:
				case SQLITE_FLOAT:
				zErrDyn="Invalid key value";
				//sqlite3DbStrDup( db, "Invalid key value" );
				rc=SQLITE_ERROR;
				break;
				case SQLITE_TEXT:
				case SQLITE_BLOB:
				nKey=sqlite3_value_bytes(argv[2]);
				zKey=sqlite3_value_blob(argv[2]).ToString();
				// (char *)sqlite3_value_blob(argv[2]);
				rc=sqlite3CodecAttach(db,db.nDb-1,zKey,nKey);
				break;
				case SQLITE_NULL:
				/* No key specified.  Use the key from the main database */sqlite3CodecGetKey(db,0,out zKey,out nKey);
				//sqlite3CodecGetKey(db, 0, (void**)&zKey, nKey);
				if(nKey>0||sqlite3BtreeGetReserve(db.aDb[0].pBt)>0) {
					rc=sqlite3CodecAttach(db,db.nDb-1,zKey,nKey);
				}
				break;
				}
			}
			#endif
			/* If the file was opened successfully, read the schema for the new database.
** If this fails, or if opening the file failed, then close the file and
** remove the entry from the db.aDb[] array. i.e. put everything back the way
** we found it.
*/if(rc==SQLITE_OK) {
				sqlite3BtreeEnterAll(db);
				rc=sqlite3Init(db,ref zErrDyn);
				sqlite3BtreeLeaveAll(db);
			}
			if(rc!=0) {
				int iDb=db.nDb-1;
				Debug.Assert(iDb>=2);
				if(db.aDb[iDb].pBt!=null) {
					sqlite3BtreeClose(ref db.aDb[iDb].pBt);
					db.aDb[iDb].pBt=null;
					db.aDb[iDb].pSchema=null;
				}
				sqlite3ResetInternalSchema(db,-1);
				db.nDb=iDb;
				if(rc==SQLITE_NOMEM||rc==SQLITE_IOERR_NOMEM) {
					////        db.mallocFailed = 1;
					sqlite3DbFree(db,ref zErrDyn);
					zErrDyn=sqlite3MPrintf(db,"out of memory");
				}
				else
					if(zErrDyn=="") {
						zErrDyn=sqlite3MPrintf(db,"unable to open database: %s",zFile);
					}
				goto attach_error;
			}
			return;
			attach_error:
			/* Return an error if we get here */if(zErrDyn!="") {
				sqlite3_result_error(context,zErrDyn,-1);
				sqlite3DbFree(db,ref zErrDyn);
			}
			if(rc!=0)
				sqlite3_result_error_code(context,rc);
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
			string zName=zName=argv[0].z!=null&&(argv[0].z.Length>0)?sqlite3_value_text(argv[0]):"";
			//(sqlite3_value_text(argv[0]);
			sqlite3 db=sqlite3_context_db_handle(context);
			int i;
			Db pDb=null;
			StringBuilder zErr=new StringBuilder(200);
			UNUSED_PARAMETER(NotUsed);
			if(zName==null)
				zName="";
			for(i=0;i<db.nDb;i++) {
				pDb=db.aDb[i];
				if(pDb.pBt==null)
					continue;
				if(pDb.zName.Equals(zName,StringComparison.InvariantCultureIgnoreCase))
					break;
			}
			if(i>=db.nDb) {
				sqlite3_snprintf(200,zErr,"no such database: %s",zName);
				goto detach_error;
			}
			if(i<2) {
				sqlite3_snprintf(200,zErr,"cannot detach database %s",zName);
				goto detach_error;
			}
			if(0==db.autoCommit) {
				sqlite3_snprintf(200,zErr,"cannot DETACH database within transaction");
				goto detach_error;
			}
			if(sqlite3BtreeIsInReadTrans(pDb.pBt)||sqlite3BtreeIsInBackup(pDb.pBt)) {
				sqlite3_snprintf(200,zErr,"database %s is locked",zName);
				goto detach_error;
			}
			sqlite3BtreeClose(ref pDb.pBt);
			pDb.pBt=null;
			pDb.pSchema=null;
			sqlite3ResetInternalSchema(db,-1);
			return;
			detach_error:
			sqlite3_result_error(context,zErr.ToString(),-1);
		}
		///<summary>
		/// Called by the parser to compile a DETACH statement.
		///
		///     DETACH pDbname
		///</summary>
		static FuncDef detach_func=new FuncDef(1,/* nArg */SqliteEncoding.UTF8,/* iPrefEnc */0,/* flags */null,/* pUserData */null,/* pNext */detachFunc,/* xFunc */null,/* xStep */null,/* xFinalize */"sqlite_detach",/* zName */null,/* pHash */null/* pDestructor */);
		///<summary>
		/// Called by the parser to compile an ATTACH statement.
		///
		///     ATTACH p AS pDbname KEY pKey
		///</summary>
		static FuncDef attach_func=new FuncDef(3,/* nArg */SqliteEncoding.UTF8,/* iPrefEnc */0,/* flags */null,/* pUserData */null,/* pNext */attachFunc,/* xFunc */null,/* xStep */null,/* xFinalize */"sqlite_attach",/* zName */null,/* pHash */null/* pDestructor */);
	#endif
	#if !SQLITE_OMIT_VIEW || !SQLITE_OMIT_TRIGGER
	#endif
	#if !SQLITE_OMIT_TRIGGER
	#endif
	}
}
