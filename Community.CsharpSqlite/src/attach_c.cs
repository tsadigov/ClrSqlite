using System;
using System.Diagnostics;
using System.Text;
using u8 = System.Byte;
using u32 = System.UInt32;

namespace Community.CsharpSqlite
{
	using sqlite3_value = Sqlite3.Mem;

	public partial class Sqlite3
	{
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
		static void attachFunc (sqlite3_context context, int NotUsed, sqlite3_value[] argv)
		{
			int i;
			int rc = 0;
			sqlite3 db = sqlite3_context_db_handle (context);
			string zName;
			string zFile;
			string zPath = "";
			string zErr = "";
			int flags;
			Db aNew = null;
			string zErrDyn = "";
			sqlite3_vfs pVfs = null;
			UNUSED_PARAMETER (NotUsed);
			zFile = argv [0].z != null && (argv [0].z.Length > 0) && argv [0].flags != MEM_Null ? sqlite3_value_text (argv [0]) : "";
			zName = argv [1].z != null && (argv [1].z.Length > 0) && argv [1].flags != MEM_Null ? sqlite3_value_text (argv [1]) : "";
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

			if (db.nDb >= db.aLimit [SQLITE_LIMIT_ATTACHED] + 2) {
				zErrDyn = sqlite3MPrintf (db, "too many attached databases - max %d", db.aLimit [SQLITE_LIMIT_ATTACHED]);
				goto attach_error;
			}
			if (0 == db.autoCommit) {
				zErrDyn = sqlite3MPrintf (db, "cannot ATTACH database within transaction");
				goto attach_error;
			}
			for (i = 0; i < db.nDb; i++) {
				string z = db.aDb [i].zName;
				Debug.Assert (z != null && zName != null);
				if (z.Equals (zName, StringComparison.InvariantCultureIgnoreCase)) {
					zErrDyn = sqlite3MPrintf (db, "database %s is already in use", zName);
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
			if (db.aDb.Length <= db.nDb)
				Array.Resize (ref db.aDb, db.nDb + 1);
			//aNew = sqlite3DbRealloc(db, db.aDb, sizeof(db.aDb[0])*(db.nDb+1) );
			if (db.aDb == null)
				return;
			// if( aNew==0 ) return;
			//}
			db.aDb [db.nDb] = new Db ();
			//db.aDb = aNew;
			aNew = db.aDb [db.nDb];
			//memset(aNew, 0, sizeof(*aNew));
			//  memset(aNew, 0, sizeof(*aNew));
			///
///<summary>
///Open the database file. If the btree is successfully opened, use
///it to obtain the database schema. At this point the schema may
///or may not be initialised.
///
///</summary>

			flags = (int)db.openFlags;
			rc = sqlite3ParseUri (db.pVfs.zName, zFile, ref flags, ref pVfs, ref zPath, ref zErr);
			if (rc != SQLITE_OK) {
				//if ( rc == SQLITE_NOMEM )
				//db.mallocFailed = 1;
				context.sqlite3_result_error (zErr, -1);
				//sqlite3_free( zErr );
				return;
			}
			Debug.Assert (pVfs != null);
			flags |= SQLITE_OPEN_MAIN_DB;
			rc = sqlite3BtreeOpen (pVfs, zPath, db, ref aNew.pBt, 0, (int)flags);
			//sqlite3_free( zPath );
			db.nDb++;
			if (rc == SQLITE_CONSTRAINT) {
				rc = SQLITE_ERROR;
				zErrDyn = sqlite3MPrintf (db, "database is already attached");
			}
			else
				if (rc == SQLITE_OK) {
					Pager pPager;
					aNew.pSchema = sqlite3SchemaGet (db, aNew.pBt);
					//if ( aNew.pSchema == null )
					//{
					//  rc = SQLITE_NOMEM;
					//}
					//else 
					if (aNew.pSchema.file_format != 0 && aNew.pSchema.enc != ENC (db)) {
						zErrDyn = sqlite3MPrintf (db, "attached databases must use the same text encoding as main database");
						rc = SQLITE_ERROR;
					}
					pPager = sqlite3BtreePager (aNew.pBt);
					pPager.sqlite3PagerLockingMode (db.dfltLockMode);
					sqlite3BtreeSecureDelete (aNew.pBt, sqlite3BtreeSecureDelete (db.aDb [0].pBt, -1));
				}
			aNew.safety_level = 3;
			aNew.zName = zName;
			//sqlite3DbStrDup(db, zName);
			//if( rc==SQLITE_OK && aNew.zName==0 ){
			//  rc = SQLITE_NOMEM;
			//}
			#if SQLITE_HAS_CODEC
			if (rc == SQLITE_OK) {
				//extern int sqlite3CodecAttach(sqlite3*, int, const void*, int);
				//extern void sqlite3CodecGetKey(sqlite3*, int, void**, int*);
				int nKey;
				string zKey;
				int t = sqlite3_value_type (argv [2]);
				switch (t) {
				case SQLITE_INTEGER:
				case SQLITE_FLOAT:
					zErrDyn = "Invalid key value";
					//sqlite3DbStrDup( db, "Invalid key value" );
					rc = SQLITE_ERROR;
					break;
				case SQLITE_TEXT:
				case SQLITE_BLOB:
					nKey = sqlite3_value_bytes (argv [2]);
					zKey = sqlite3_value_blob (argv [2]).ToString ();
					// (char *)sqlite3_value_blob(argv[2]);
					rc = sqlite3CodecAttach (db, db.nDb - 1, zKey, nKey);
					break;
				case SQLITE_NULL:
					///
///<summary>
///No key specified.  Use the key from the main database 
///</summary>

					sqlite3CodecGetKey (db, 0, out zKey, out nKey);
					//sqlite3CodecGetKey(db, 0, (void**)&zKey, nKey);
					if (nKey > 0 || sqlite3BtreeGetReserve (db.aDb [0].pBt) > 0) {
						rc = sqlite3CodecAttach (db, db.nDb - 1, zKey, nKey);
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

			if (rc == SQLITE_OK) {
				sqlite3BtreeEnterAll (db);
				rc = sqlite3Init (db, ref zErrDyn);
				sqlite3BtreeLeaveAll (db);
			}
			if (rc != 0) {
				int iDb = db.nDb - 1;
				Debug.Assert (iDb >= 2);
				if (db.aDb [iDb].pBt != null) {
					sqlite3BtreeClose (ref db.aDb [iDb].pBt);
					db.aDb [iDb].pBt = null;
					db.aDb [iDb].pSchema = null;
				}
				sqlite3ResetInternalSchema (db, -1);
				db.nDb = iDb;
				if (rc == SQLITE_NOMEM || rc == SQLITE_IOERR_NOMEM) {
					////        db.mallocFailed = 1;
					db.sqlite3DbFree (ref zErrDyn);
					zErrDyn = sqlite3MPrintf (db, "out of memory");
				}
				else
					if (zErrDyn == "") {
						zErrDyn = sqlite3MPrintf (db, "unable to open database: %s", zFile);
					}
				goto attach_error;
			}
			return;
			attach_error:
			///
///<summary>
///Return an error if we get here 
///</summary>

			if (zErrDyn != "") {
				context.sqlite3_result_error (zErrDyn, -1);
				db.sqlite3DbFree (ref zErrDyn);
			}
			if (rc != 0)
				context.sqlite3_result_error_code (rc);
		}

		///<summary>
		/// An SQL user-function registered to do the work of an DETACH statement. The
		/// three arguments to the function come directly from a detach statement:
		///
		///     DETACH DATABASE x
		///
		///     SELECT sqlite_detach(x)
		///</summary>
		static void detachFunc (sqlite3_context context, int NotUsed, sqlite3_value[] argv)
		{
			string zName = zName = argv [0].z != null && (argv [0].z.Length > 0) ? sqlite3_value_text (argv [0]) : "";
			//(sqlite3_value_text(argv[0]);
			sqlite3 db = sqlite3_context_db_handle (context);
			int i;
			Db pDb = null;
			StringBuilder zErr = new StringBuilder (200);
			UNUSED_PARAMETER (NotUsed);
			if (zName == null)
				zName = "";
			for (i = 0; i < db.nDb; i++) {
				pDb = db.aDb [i];
				if (pDb.pBt == null)
					continue;
				if (pDb.zName.Equals (zName, StringComparison.InvariantCultureIgnoreCase))
					break;
			}
			if (i >= db.nDb) {
				sqlite3_snprintf (200, zErr, "no such database: %s", zName);
				goto detach_error;
			}
			if (i < 2) {
				sqlite3_snprintf (200, zErr, "cannot detach database %s", zName);
				goto detach_error;
			}
			if (0 == db.autoCommit) {
				sqlite3_snprintf (200, zErr, "cannot DETACH database within transaction");
				goto detach_error;
			}
			if (pDb.pBt.sqlite3BtreeIsInReadTrans () || pDb.pBt.sqlite3BtreeIsInBackup ()) {
				sqlite3_snprintf (200, zErr, "database %s is locked", zName);
				goto detach_error;
			}
			sqlite3BtreeClose (ref pDb.pBt);
			pDb.pBt = null;
			pDb.pSchema = null;
			sqlite3ResetInternalSchema (db, -1);
			return;
			detach_error:
			context.sqlite3_result_error (zErr.ToString (), -1);
		}

		///<summary>
		/// Called by the parser to compile a DETACH statement.
		///
		///     DETACH pDbname
		///</summary>
		static FuncDef detach_func = new FuncDef (1, ///
///<summary>
///nArg 
///</summary>

		SqliteEncoding.UTF8, ///
///<summary>
///iPrefEnc 
///</summary>

		0, ///
///<summary>
///flags 
///</summary>

		null, ///
///<summary>
///pUserData 
///</summary>

		null, ///
///<summary>
///pNext 
///</summary>

		detachFunc, ///
///<summary>
///xFunc 
///</summary>

		null, ///
///<summary>
///xStep 
///</summary>

		null, ///
///<summary>
///xFinalize 
///</summary>

		"sqlite_detach", ///
///<summary>
///zName 
///</summary>

		null, ///
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
		static FuncDef attach_func = new FuncDef (3, ///
///<summary>
///nArg 
///</summary>

		SqliteEncoding.UTF8, ///
///<summary>
///iPrefEnc 
///</summary>

		0, ///
///<summary>
///flags 
///</summary>

		null, ///
///<summary>
///pUserData 
///</summary>

		null, ///
///<summary>
///pNext 
///</summary>

		attachFunc, ///
///<summary>
///xFunc 
///</summary>

		null, ///
///<summary>
///xStep 
///</summary>

		null, ///
///<summary>
///xFinalize 
///</summary>

		"sqlite_attach", ///
///<summary>
///zName 
///</summary>

		null, ///
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
