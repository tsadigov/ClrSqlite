using System;
using System.Diagnostics;
using System.Text;
using i64=System.Int64;
using u8=System.Byte;
using Pgno=System.UInt32;
using sqlite3_int64=System.Int64;
using System.Globalization;
using Community.CsharpSqlite.builder;
using Community.CsharpSqlite.Ast;
namespace Community.CsharpSqlite {
    using Metadata;
    using Vdbe = Engine.Vdbe;
    using Community.CsharpSqlite.Os;
    using Community.CsharpSqlite.Engine;
    using Community.CsharpSqlite.tree;
    using Community.CsharpSqlite.Utils;
    using Community.CsharpSqlite.Paging;
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
		/// This file contains code used to implement the PRAGMA command.
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
		///<summary>
		/// Interpret the given string as a safety level.  Return 0 for OFF,
		/// 1 for ON or NORMAL and 2 for FULL.  Return 1 for an empty or
		/// unrecognized string argument.
		///
		/// Note that the values returned are one less that the values that
		/// should be passed into sqlite3BtreeSetSafetyLevel().  The is done
		/// to support legacy SQL code.  The safety level used to be boolean
		/// and older scripts may have used numbers 0 for OFF and 1 for ON.
		///</summary>
		static u8 getSafetyLevel(string z) {
			//                             /* 123456789 123456789 */
			string zText="onoffalseyestruefull";
			int[] iOffset=new int[] {
				0,
				1,
				2,
				4,
				9,
				12,
				16
			};
			int[] iLength=new int[] {
				2,
				2,
				3,
				5,
				3,
				4,
				4
			};
			u8[] iValue=new u8[] {
				1,
				0,
				0,
				0,
				1,
				1,
				2
			};
			int i,n;
			if(CharExtensions.sqlite3Isdigit(z[0])) {
				return (u8)Converter.sqlite3Atoi(z);
			}
            n = z.Strlen30();
			for(i=0;i<Sqlite3.ArraySize(iLength);i++) {
				if(iLength[i]==n&&StringExtensions.sqlite3StrNICmp(zText.Substring(iOffset[i]),z,n)==0) {
					return iValue[i];
				}
			}
			return 1;
		}
		///
		///<summary>
		///Interpret the given string as a boolean value.
		///
		///</summary>
		static u8 sqlite3GetBoolean(string z) {
			return (u8)(getSafetyLevel(z)&1);
			//return getSafetyLevel(z)&1;
		}
		///<summary>
		///The sqlite3GetBoolean() function is used by other modules but the
		/// remainder of this file is specific to PRAGMA processing.  So omit
		/// the rest of the file if PRAGMAs are omitted from the build.
		///</summary>
		#if !(SQLITE_OMIT_PRAGMA)
		///<summary>
		/// Interpret the given string as a locking mode value.
		///
		///</summary>
		static int getLockingMode(string z) {
			if(z!=null) {
				if(z.Equals("exclusive",StringComparison.InvariantCultureIgnoreCase))
					return Globals.Paging.PAGER_LOCKINGMODE_EXCLUSIVE;
				if(z.Equals("normal",StringComparison.InvariantCultureIgnoreCase))
					return Globals.Paging.PAGER_LOCKINGMODE_NORMAL;
			}
			return Globals.Paging.PAGER_LOCKINGMODE_QUERY;
		}
		#if !SQLITE_OMIT_AUTOVACUUM
		///<summary>
		/// Interpret the given string as an auto-vacuum mode value.
		///
		/// The following strings, "none", "full" and "incremental" are
		/// acceptable, as are their numeric equivalents: 0, 1 and 2 respectively.
		///</summary>
		static u8 getAutoVacuum(string z) {
			int i;
			if(z.Equals("none",StringComparison.InvariantCultureIgnoreCase))
				return BTREE_AUTOVACUUM_NONE;
			if(z.Equals("full",StringComparison.InvariantCultureIgnoreCase))
				return BTREE_AUTOVACUUM_FULL;
			if(z.Equals("incremental",StringComparison.InvariantCultureIgnoreCase))
				return BTREE_AUTOVACUUM_INCR;
			i=_Custom.atoi(z);
			return (u8)((i>=0&&i<=2)?i:0);
		}
		#endif
		#if !SQLITE_OMIT_PAGER_PRAGMAS
		///<summary>
		/// Interpret the given string as a temp db location. Return 1 for file
		/// backed temporary databases, 2 for the Red-Black tree in memory database
		/// and 0 to use the compile-time default.
		///</summary>
		static int getTempStore(string z) {
			if(z[0]>='0'&&z[0]<='2') {
				return z[0]-'0';
			}
			else
				if(z.Equals("file",StringComparison.InvariantCultureIgnoreCase)) {
					return 1;
				}
				else
					if(z.Equals("memory",StringComparison.InvariantCultureIgnoreCase)) {
						return 2;
					}
					else {
						return 0;
					}
		}
		#endif
		#if !SQLITE_OMIT_PAGER_PRAGMAS
		///<summary>
		/// Invalidate temp storage, either when the temp storage is changed
		/// from default, or when 'file' and the temp_store_directory has changed
		///</summary>
		static SqlResult invalidateTempStorage(ParseState pParse) {
			Connection db=pParse.db;
			if(db.Backends[1].BTree!=null) {
				if(0==db.autoCommit||db.Backends[1].BTree.sqlite3BtreeIsInReadTrans()) {
					utilc.sqlite3ErrorMsg(pParse,"temporary storage cannot be changed "+"from within a transaction");
					return SqlResult.SQLITE_ERROR;
				}
				BTreeMethods.sqlite3BtreeClose(ref db.Backends[1].BTree);
				db.Backends[1].BTree=null;
				build.sqlite3ResetInternalSchema(db,-1);
			}
			return SqlResult.SQLITE_OK;
		}
		#endif
		#if !SQLITE_OMIT_PAGER_PRAGMAS
		///<summary>
		/// If the TEMP database is open, close it and mark the database schema
		/// as needing reloading.  This must be done when using the SQLITE_TEMP_STORE
		/// or DEFAULT_TEMP_STORE pragmas.
		///</summary>
		static SqlResult changeTempStorage(ParseState pParse,string zStorageType) {
			int ts=getTempStore(zStorageType);
			Connection db=pParse.db;
			if(db.temp_store==ts)
				return SqlResult.SQLITE_OK;
			if(invalidateTempStorage(pParse)!=SqlResult.SQLITE_OK) {
				return SqlResult.SQLITE_ERROR;
			}
			db.temp_store=(u8)ts;
			return SqlResult.SQLITE_OK;
		}
		#endif
		///<summary>
		/// Generate code to return a single integer value.
		///</summary>
		static void returnSingleInt(ParseState pParse,string zLabel,i64 value) {
			var v=pParse.sqlite3GetVdbe();
			int mem=++pParse.UsedCellCount;
			//i64* pI64 = sqlite3DbMallocRaw( pParse->db, sizeof( value ) );
			//if ( pI64 )
			//{
			//  memcpy( pI64, &value, sizeof( value ) );
			//}
			//sqlite3VdbeAddOp4( v,  OpCode.OP_Int64, 0, mem, 0, (char*)pI64,  P4Usage.P4_INT64 );
			v.sqlite3VdbeAddOp4(OpCode.OP_Int64,0,mem,0,value, P4Usage.P4_INT64);
			v.sqlite3VdbeSetNumCols(1);
            v.sqlite3VdbeSetColName(0, ColName.NAME, zLabel, SQLITE_STATIC);
			v.sqlite3VdbeAddOp2(OpCode.OP_ResultRow,mem,1);
		}
		#if !SQLITE_OMIT_FLAG_PRAGMAS
		
		static int flagPragma(ParseState pParse,string zLeft,string zRight) {
			sPragmaType[] aPragma=new sPragmaType[] {
				new sPragmaType("full_column_names",SqliteFlags.SQLITE_FullColNames),
				new sPragmaType("short_column_names",SqliteFlags.SQLITE_ShortColNames),
				new sPragmaType("count_changes",SqliteFlags.SQLITE_CountRows),
				new sPragmaType("empty_result_callbacks",SqliteFlags.SQLITE_NullCallback),
				new sPragmaType("legacy_file_format",SqliteFlags.SQLITE_LegacyFileFmt),
				new sPragmaType("fullfsync",SqliteFlags.SQLITE_FullFSync),
				new sPragmaType("checkpoint_fullfsync",SqliteFlags.SQLITE_CkptFullFSync),
				new sPragmaType("reverse_unordered_selects",SqliteFlags.SQLITE_ReverseOrder),
				#if !SQLITE_OMIT_AUTOMATIC_INDEX
				new sPragmaType("automatic_index",SqliteFlags.SQLITE_AutoIndex),
				#endif
				#if SQLITE_DEBUG
																																																																																																								new sPragmaType( "sql_trace",                SQLITE_SqlTrace      ),
new sPragmaType( "vdbe_listing",             SQLITE_VdbeListing   ),
new sPragmaType( "vdbe_trace",               SQLITE_VdbeTrace     ),
#endif
				#if !SQLITE_OMIT_CHECK
				new sPragmaType("ignore_check_constraints",SqliteFlags.SQLITE_IgnoreChecks),
				#endif
				///
				///<summary>
				///The following is VERY experimental 
				///</summary>
				new sPragmaType("writable_schema",SqliteFlags.SQLITE_WriteSchema|SqliteFlags.SQLITE_RecoveryMode),
				new sPragmaType("omit_readlock",SqliteFlags.SQLITE_NoReadlock),
				///
				///<summary>
				///TODO: Maybe it shouldn't be possible to change the ReadUncommitted
				///flag if there are any active statements. 
				///</summary>
				new sPragmaType("read_uncommitted",SqliteFlags.SQLITE_ReadUncommitted),
				new sPragmaType("recursive_triggers",SqliteFlags.SQLITE_RecTriggers),
				///
				///<summary>
				///</summary>
				///<param name="This flag may only be set if both foreign">key and trigger support</param>
				///<param name="are present in the build.  ">are present in the build.  </param>
				#if !(SQLITE_OMIT_FOREIGN_KEY) && !(SQLITE_OMIT_TRIGGER)
				new sPragmaType("foreign_keys",SqliteFlags.SQLITE_ForeignKeys),
			#endif
			};
			int i;
			sPragmaType p;
			for(i=0;i<Sqlite3.ArraySize(aPragma);i++)//, p++)
			 {
				p=aPragma[i];
				if(zLeft.Equals(p.zName,StringComparison.InvariantCultureIgnoreCase)) {
					Connection db=pParse.db;
					Vdbe v;
					v=pParse.sqlite3GetVdbe();
					Debug.Assert(v!=null);
					///
					///<summary>
					///Already allocated by sqlite3Pragma() 
					///</summary>
					if(Sqlite3.ALWAYS(v)) {
						if(null==zRight) {
							returnSingleInt(pParse,p.zName,((db.flags&p.mask)!=0)?1:0);
						}
						else {
                            SqliteFlags mask = p.mask;
							///
							///<summary>
							///Mask of bits to set or clear. 
							///</summary>
							if(db.autoCommit==0) {
								///
								///<summary>
								///Foreign key support may not be enabled or disabled while not
								///</summary>
								///<param name="in auto">commit mode.  </param>
                                mask &= ~(SqliteFlags.SQLITE_ForeignKeys);
							}
							if(sqlite3GetBoolean(zRight)!=0) {
								db.flags|=mask;
							}
							else {
								db.flags&=~mask;
							}
							///
							///<summary>
							///</summary>
							///<param name="Many of the flag">pragmas modify the code generated by the SQL</param>
							///<param name="compiler (eg. count_changes). So add an opcode to expire all">compiler (eg. count_changes). So add an opcode to expire all</param>
							///<param name="compiled SQL statements after modifying a pragma value.">compiled SQL statements after modifying a pragma value.</param>
							///<param name=""></param>
							v.sqlite3VdbeAddOp2( OpCode.OP_Expire,0,0);
						}
					}
					return 1;
				}
			}
			return 0;
		}
		#endif
		///<summary>
		/// Return a human-readable name for a constraint resolution action.
		///</summary>
		#if !SQLITE_OMIT_FOREIGN_KEY
		static string actionName(OnConstraintError action) {
			string zName;
			switch(action) {
			case OnConstraintError.OE_SetNull:
			zName="SET NULL";
			break;
			case OnConstraintError.OE_SetDflt:
			zName="SET DEFAULT";
			break;
			case OnConstraintError.OE_Cascade:
			zName="CASCADE";
			break;
			case OnConstraintError.OE_Restrict:
			zName="RESTRICT";
			break;
			default:
			zName="NO ACTION";
			Debug.Assert(action==OnConstraintError.OE_None);
			break;
			}
			return zName;
		}
		#endif
		///<summary>
		/// Parameter eMode must be one of the Globals.Paging.PAGER_JOURNALMODE_XXX constants
		/// defined in pager.h. This function returns the associated lowercase
		/// journal-mode name.
		///</summary>
		public static string sqlite3JournalModename(JournalMode Mode) {
            int eMode = (int)Mode;
			string[] azModeName= {
				"delete",
				"persist",
				"off",
				"truncate",
				"memory"
			#if !SQLITE_OMIT_WAL
																																																																														, "wal"
#endif
			};
			Debug.Assert((int)JournalMode.PAGER_JOURNALMODE_DELETE==0);
			Debug.Assert((int)JournalMode.PAGER_JOURNALMODE_PERSIST==1);
			Debug.Assert((int)JournalMode.PAGER_JOURNALMODE_OFF==2);
			Debug.Assert((int)JournalMode.PAGER_JOURNALMODE_TRUNCATE==3);
			Debug.Assert((int)JournalMode.PAGER_JOURNALMODE_MEMORY==4);
			Debug.Assert((int)JournalMode.PAGER_JOURNALMODE_WAL==5);
			Debug.Assert(eMode>=0&&eMode<=Sqlite3.ArraySize(azModeName));
			if(eMode==Sqlite3.ArraySize(azModeName))
				return null;
			return azModeName[eMode];
		}
		

		public static EncName[] encnames=new EncName[] {
			new EncName("UTF8",SqliteEncoding.UTF8),
			new EncName("UTF-8",SqliteEncoding.UTF8),
			///
			///<summary>
			///Must be element [1] 
			///</summary>
			new EncName("UTF-16le (not supported)",SqliteEncoding.UTF16LE),
			///
			///<summary>
			///Must be element [2] 
			///</summary>
			new EncName("UTF-16be (not supported)",SqliteEncoding.UTF16BE),
			///
			///<summary>
			///Must be element [3] 
			///</summary>
			new EncName("UTF16le (not supported)",SqliteEncoding.UTF16LE),
			new EncName("UTF16be (not supported)",SqliteEncoding.UTF16BE),
			new EncName("UTF-16 (not supported)",0),
			///
			///<summary>
			///SQLITE_UTF16NATIVE 
			///</summary>
			new EncName("UTF16",0),
			///
			///<summary>
			///SQLITE_UTF16NATIVE 
			///</summary>
			new EncName(null,0)
		};
		// OVERLOADS, so I don't need to rewrite parse.c
		public static void sqlite3Pragma(ParseState pParse,Token pId1,Token pId2,int null_4,int minusFlag) {
			sqlite3Pragma(pParse,pId1,pId2,null,minusFlag);
		}
		public static void sqlite3Pragma(ParseState pParse,Token pId1,///
		///<summary>
		///First part of [database.]id field 
		///</summary>
		Token pId2,///
		///<summary>
		///Second part of [database.]id field, or NULL 
		///</summary>
		Token pValue,///
		///<summary>
		///Token for <value>, or NULL 
		///</summary>
		int minusFlag///
		///<summary>
		///</summary>
		///<param name="True if a '">' sign preceded <value> </param>
		) {
			string zLeft=null;
			///<param name="Nul">8 string <id> </param>
			string zRight=null;
			///<param name="Nul">8 string <value>, or NULL </param>
			string zDb=null;
			///The database name 
			Token pId=new Token();
			///Pointer to <id> token 
			int iDb;
			///Database index for <database> 
			Connection db=pParse.db;
			DbBackend pDb;
			var v=pParse.pVdbe=Vdbe.Create(db);
			if(v==null)
				return;
			v.sqlite3VdbeRunOnlyOnce();
			pParse.UsedCellCount=2;
			///Interpret the [database.] part of the pragma statement. iDb is the
			///index of the database this pragma is being applied to in db.aDb[]. 
			iDb=build.sqlite3TwoPartName(pParse,pId1,pId2,ref pId);
			if(iDb<0)
				return;
			pDb=db.Backends[iDb];
			///If the temp database has been explicitly named as part of the
			///pragma, make sure it is open.
			if(iDb==1&&build.sqlite3OpenTempDatabase(pParse)!=0) {
				return;
			}
			zLeft=build.Token2Name(db,pId);
			if(zLeft=="")
				return;
			if(minusFlag!=0) {
				zRight=(pValue==null)?"":io.sqlite3MPrintf(db,"-%T",pValue);
			}
			else {
				zRight=build.Token2Name(db,pValue);
			}
			Debug.Assert(pId2!=null);
			zDb=pId2.Length>0?pDb.Name:null;
			#if !SQLITE_OMIT_AUTHORIZATION
																																																																														if ( sqlite3AuthCheck( pParse, SQLITE_PRAGMA, zLeft, zRight, zDb ) )
{
goto pragma_out;
}
#endif
			#if !SQLITE_OMIT_PAGER_PRAGMAS
			///
			///<summary>
			///PRAGMA [database.]default_cache_size
			///PRAGMA [database.]default_cache_size=N
			///
			///The first form reports the current persistent setting for the
			///page cache size.  The value returned is the maximum number of
			///pages in the page cache.  The second form sets both the current
			///page cache size value and the persistent page cache size value
			///stored in the database file.
			///
			///Older versions of SQLite would set the default cache size to a
			///negative number to indicate synchronous=OFF.  These days, synchronous
			///is always on by default regardless of the sign of the default cache
			///size.  But continue to take the absolute value of the default cache
			///size of historical compatibility.
			///</summary>
			if(zLeft.Equals("default_cache_size",StringComparison.InvariantCultureIgnoreCase)) {
				VdbeOpList[] getCacheSize=new VdbeOpList[] {
					new VdbeOpList(OpCode.OP_Transaction,0,0,0),
					///
					///<summary>
					///0 
					///</summary>
					new VdbeOpList(OpCode.OP_ReadCookie,0,1,BTreeProp.DEFAULT_CACHE_SIZE),
					///
					///<summary>
					///1 
					///</summary>
					new VdbeOpList(OpCode.OP_IfPos,1,7,0),
					new VdbeOpList(OpCode.OP_Integer,0,2,0),
					new VdbeOpList(OpCode.OP_Subtract,1,2,1),
					new VdbeOpList(OpCode.OP_IfPos,1,7,0),
					new VdbeOpList(OpCode.OP_Integer,0,1,0),
					///
					///<summary>
					///6 
					///</summary>
					new VdbeOpList(OpCode.OP_ResultRow,1,1,0),
				};
				int addr;
				if(SqlResult.SQLITE_OK!= pParse.sqlite3ReadSchema())
					goto pragma_out;
                Engine.vdbeaux.markUsed(v, iDb);
				if(null==zRight) {
					v.sqlite3VdbeSetNumCols(1);
                    v.sqlite3VdbeSetColName(0, ColName.NAME, "cache_size", SQLITE_STATIC);
					pParse.UsedCellCount+=2;
					addr=v.sqlite3VdbeAddOpList(getCacheSize.Length,getCacheSize);
					v.sqlite3VdbeChangeP1(addr,iDb);
					v.sqlite3VdbeChangeP1(addr+1,iDb);
					v.sqlite3VdbeChangeP1(addr+6,Globals.SQLITE_DEFAULT_CACHE_SIZE);
				}
				else {
                    int size = utilc.sqlite3AbsInt32(Converter.sqlite3Atoi(zRight));
					build.sqlite3BeginWriteOperation(pParse,0,iDb);
					v.sqlite3VdbeAddOp2(OpCode.OP_Integer,size,1);
                    v.sqlite3VdbeAddOp3(OpCode.OP_SetCookie, iDb, BTreeProp.DEFAULT_CACHE_SIZE, 1);
					Debug.Assert(sqlite3SchemaMutexHeld(db,iDb,null));
					pDb.pSchema.cache_size=size;
					pDb.BTree.SetCacheSize(pDb.pSchema.cache_size);
				}
			}
			else
				///
				///<summary>
				///PRAGMA [database.]page_size
				///PRAGMA [database.]page_size=N
				///
				///The first form reports the current setting for the
				///database page size in bytes.  The second form sets the
				///database page size value.  The value can only be set if
				///the database has not yet been created.
				///
				///</summary>
				if(zLeft.Equals("page_size",StringComparison.InvariantCultureIgnoreCase)) {
					Btree pBt=pDb.BTree;
					Debug.Assert(pBt!=null);
					if(null==zRight) {
						int size=Sqlite3.ALWAYS(pBt)?pBt.GetPageSize():0;
						returnSingleInt(pParse,"page_size",size);
					}
					else {
						///
						///<summary>
						///</summary>
						///<param name="Malloc may fail when setting the page">size, as there is an internal</param>
						///<param name="buffer that the pager module resizes using sqlite3_realloc().">buffer that the pager module resizes using sqlite3_realloc().</param>
						///<param name=""></param>
						db.nextPagesize=Converter.sqlite3Atoi(zRight);
						if(SqlResult.SQLITE_NOMEM == pBt.sqlite3BtreeSetPageSize(db.nextPagesize,-1,0)) {
							////        db.mallocFailed = 1;
						}
					}
				}
				else
					///
					///<summary>
					///PRAGMA [database.]secure_delete
					///PRAGMA [database.]secure_delete=ON/OFF
					///
					///The first form reports the current setting for the
					///secure_delete flag.  The second form changes the secure_delete
					///flag setting and reports thenew value.
					///
					///</summary>
					if(zLeft.Equals("secure_delete",StringComparison.InvariantCultureIgnoreCase)) {
						Btree pBt=pDb.BTree;
						int b=-1;
						Debug.Assert(pBt!=null);
						if(zRight!=null) {
							b=sqlite3GetBoolean(zRight);
						}
						if(pId2.Length==0&&b>=0) {
							int ii;
							for(ii=0;ii<db.BackendCount;ii++) {
								db.Backends[ii].BTree.sqlite3BtreeSecureDelete(b);
							}
						}
						b=pBt.sqlite3BtreeSecureDelete(b);
						returnSingleInt(pParse,"secure_delete",b);
					}
					else
						///
						///<summary>
						///PRAGMA [database.]max_page_count
						///PRAGMA [database.]max_page_count=N
						///
						///The first form reports the current setting for the
						///maximum number of pages in the database file.  The 
						///second form attempts to change this setting.  Both
						///forms return the current setting.
						///
						///PRAGMA [database.]page_count
						///
						///Return the number of pages in the specified database.
						///
						///</summary>
						if(zLeft.Equals("page_count",StringComparison.InvariantCultureIgnoreCase)||zLeft.Equals("max_page_count",StringComparison.InvariantCultureIgnoreCase)) {
							int iReg;
							if(SqlResult.SQLITE_OK!= pParse.sqlite3ReadSchema())
								goto pragma_out;
							build.sqlite3CodeVerifySchema(pParse,iDb);
							iReg=++pParse.UsedCellCount;
							if(zLeft[0]=='p') {
								v.sqlite3VdbeAddOp2( OpCode.OP_Pagecount,iDb,iReg);
							}
							else {
								v.AddOpp3( OpCode.OP_MaxPgcnt,iDb,iReg,Converter.sqlite3Atoi(zRight));
							}
							v.sqlite3VdbeAddOp2(OpCode.OP_ResultRow,iReg,1);
							v.sqlite3VdbeSetNumCols(1);
                            v.sqlite3VdbeSetColName(0, ColName.NAME, zLeft, SQLITE_TRANSIENT);
						}
						else
							///
							///<summary>
							///PRAGMA [database.]page_count
							///
							///Return the number of pages in the specified database.
							///
							///</summary>
							if(zLeft=="page_count") {
								Vdbe _v;
								int iReg;
								_v=pParse.sqlite3GetVdbe();
								if(_v==null||SqlResult.SQLITE_OK!= pParse.sqlite3ReadSchema())
									goto pragma_out;
								build.sqlite3CodeVerifySchema(pParse,iDb);
								iReg=++pParse.UsedCellCount;
								_v.sqlite3VdbeAddOp2( OpCode.OP_Pagecount,iDb,iReg);
								_v.sqlite3VdbeAddOp2(OpCode.OP_ResultRow,iReg,1);
								_v.sqlite3VdbeSetNumCols(1);
                                _v.sqlite3VdbeSetColName(0, ColName.NAME, "page_count", SQLITE_STATIC);
							}
							else
								///
								///<summary>
								///PRAGMA [database.]locking_mode
								///PRAGMA [database.]locking_mode = (normal|exclusive)
								///
								///</summary>
								if(zLeft.Equals("locking_mode",StringComparison.InvariantCultureIgnoreCase)) {
									string zRet="normal";
									int eMode=getLockingMode(zRight);
									if(pId2.Length==0&&eMode==Globals.Paging.PAGER_LOCKINGMODE_QUERY) {
										///
										///<summary>
										///Simple "PRAGMA locking_mode;" statement. This is a query for
										///the current default locking mode (which may be different to
										///</summary>
										///<param name="the locking">mode of the main database).</param>
										///<param name=""></param>
										eMode=db.dfltLockMode;
									}
									else {
										Pager pPager;
										if(pId2.Length==0) {
											///
											///<summary>
											///This indicates that no database name was specified as part
											///</summary>
											///<param name="of the PRAGMA command. In this case the locking">mode must be</param>
											///<param name="set on all attached databases, as well as the main db file.">set on all attached databases, as well as the main db file.</param>
											///<param name=""></param>
											///<param name="Also, the sqlite3.dfltLockMode variable is set so that">Also, the sqlite3.dfltLockMode variable is set so that</param>
											///<param name="any subsequently attached databases also use the specified">any subsequently attached databases also use the specified</param>
											///<param name="locking mode.">locking mode.</param>
											///<param name=""></param>
											int ii;
											Debug.Assert(pDb==db.Backends[0]);
											for(ii=2;ii<db.BackendCount;ii++) {
												pPager=db.Backends[ii].BTree.sqlite3BtreePager();
												pPager.sqlite3PagerLockingMode(eMode);
											}
											db.dfltLockMode=(u8)eMode;
										}
										pPager=pDb.BTree.sqlite3BtreePager();
										eMode=pPager.sqlite3PagerLockingMode(eMode)?1:0;
									}
									Debug.Assert(eMode==Globals.Paging.PAGER_LOCKINGMODE_NORMAL||eMode==Globals.Paging.PAGER_LOCKINGMODE_EXCLUSIVE);
									if(eMode==Globals.Paging.PAGER_LOCKINGMODE_EXCLUSIVE) {
										zRet="exclusive";
									}
									v.sqlite3VdbeSetNumCols(1);
                                    v.sqlite3VdbeSetColName(0, ColName.NAME, "locking_mode", SQLITE_STATIC);
									v.sqlite3VdbeAddOp4(OpCode.OP_String8,0,1,0,zRet,0);
									v.sqlite3VdbeAddOp2(OpCode.OP_ResultRow,1,1);
								}
								else
									///
									///<summary>
									///PRAGMA [database.]journal_mode
									///PRAGMA [database.]journal_mode =
									///(delete|persist|off|truncate|memory|wal|off)
									///
									///</summary>
									if(zLeft=="journal_mode") {
                                        JournalMode eMode;
										///One of the Globals.Paging.PAGER_JOURNALMODE_XXX symbols 
										int ii;
										///Loop counter 
										
                                        ///Force the schema to be loaded on all databases.  This cases all
										///database files to be opened and the journal_modes set. 
										if(SqlResult.SQLITE_OK!= pParse.sqlite3ReadSchema()) {
											goto pragma_out;
										}
										v.sqlite3VdbeSetNumCols(1);
                                        v.sqlite3VdbeSetColName(0, ColName.NAME, "journal_mode", SQLITE_STATIC);
										if(null==zRight) {
											///
											///<summary>
											///If there is no "=MODE" part of the pragma, do a query for the
											///current mode 
											///</summary>
											eMode=JournalMode.PAGER_JOURNALMODE_QUERY;
										}
										else {
											string zMode;
											int n=StringExtensions.Strlen30(zRight);
											for(eMode=0;(zMode=sqlite3JournalModename(eMode))!=null;eMode++) {
												if(StringExtensions.sqlite3StrNICmp(zRight,zMode,n)==0)
													break;
											}
											if(null==zMode) {
												///
												///<summary>
												///If the "=MODE" part does not match any known journal mode,
												///then do a query 
												///</summary>
												eMode= JournalMode.PAGER_JOURNALMODE_QUERY;
											}
										}
										if(eMode== JournalMode.PAGER_JOURNALMODE_QUERY&&pId2.Length==0) {
											///
											///<summary>
											///Convert "PRAGMA journal_mode" into "PRAGMA main.journal_mode" 
											///</summary>
											iDb=0;
											pId2.Length=1;
										}
										for(ii=db.BackendCount-1;ii>=0;ii--) {
											if(db.Backends[ii].BTree!=null&&(ii==iDb||pId2.Length==0)) {
                                                Engine.vdbeaux.markUsed(v, ii);
                                                v.AddOpp3(OpCode.OP_JournalMode, ii, 1, (int)eMode);
											}
										}
										v.sqlite3VdbeAddOp2(OpCode.OP_ResultRow,1,1);
									}
									else
										///
										///<summary>
										///PRAGMA [database.]journal_size_limit
										///PRAGMA [database.]journal_size_limit=N
										///
										///Get or set the size limit on rollback journal files.
										///
										///</summary>
										if(zLeft.Equals("journal_size_limit",StringComparison.InvariantCultureIgnoreCase)) {
											Pager pPager=pDb.BTree.sqlite3BtreePager();
											i64 iLimit=-2;
											if(!String.IsNullOrEmpty(zRight)) {
												Converter.sqlite3Atoi64(zRight,ref iLimit,1000000,SqliteEncoding.UTF8);
												if(iLimit<-1)
													iLimit=-1;
											}
											iLimit=pPager.sqlite3PagerJournalSizeLimit(iLimit);
											returnSingleInt(pParse,"journal_size_limit",iLimit);
										}
										else
											#endif
											///
											///<summary>
											///PRAGMA [database.]auto_vacuum
											///PRAGMA [database.]auto_vacuum=N
											///
											///</summary>
											///<param name="Get or set the value of the database 'auto">vacuum' parameter.</param>
											///<param name="The value is one of:  0 NONE 1 FULL 2 INCREMENTAL">The value is one of:  0 NONE 1 FULL 2 INCREMENTAL</param>
											#if !SQLITE_OMIT_AUTOVACUUM
											if(zLeft.Equals("auto_vacuum",StringComparison.InvariantCultureIgnoreCase)) {
												Btree pBt=pDb.BTree;
												Debug.Assert(pBt!=null);
												if(SqlResult.SQLITE_OK!= pParse.sqlite3ReadSchema()) {
													goto pragma_out;
												}
												if(null==zRight) {
													int auto_vacuum;
													if(Sqlite3.ALWAYS(pBt)) {
														auto_vacuum=pBt.GetAutoVacuum();
													}
													else {
														auto_vacuum=SQLITE_DEFAULT_AUTOVACUUM;
													}
													returnSingleInt(pParse,"auto_vacuum",auto_vacuum);
												}
												else {
													int eAuto=getAutoVacuum(zRight);
													Debug.Assert(eAuto>=0&&eAuto<=2);
													db.nextAutovac=(u8)eAuto;
													if(Sqlite3.ALWAYS(eAuto>=0)) {
														///
														///<summary>
														///Call SetAutoVacuum() to set initialize the internal auto and
														///</summary>
														///<param name="incr">vacuum flags. This is required in case this connection</param>
														///<param name="creates the database file. It is important that it is created">creates the database file. It is important that it is created</param>
														///<param name="as an auto">vacuum capable db.</param>
														///<param name=""></param>
														var rc=pBt.sqlite3BtreeSetAutoVacuum(eAuto);
														if(rc==SqlResult.SQLITE_OK&&(eAuto==1||eAuto==2)) {
															///
															///<summary>
															///When setting the auto_vacuum mode to either "full" or
															///"incremental", write the value of meta[6] in the database
															///file. Before writing to meta[6], check that meta[3] indicates
															///</summary>
															///<param name="that this really is an auto">vacuum capable database.</param>
															///<param name=""></param>
															VdbeOpList[] setMeta6=new VdbeOpList[] {
																new VdbeOpList(OpCode.OP_Transaction,0,1,0),
																///
																///<summary>
																///0 
																///</summary>
																new VdbeOpList(OpCode.OP_ReadCookie,0,1,BTreeProp.LARGEST_ROOT_PAGE),
																///
																///<summary>
																///1 
																///</summary>
																new VdbeOpList(OpCode.OP_If,1,0,0),
																///
																///<summary>
																///2 
																///</summary>
																new VdbeOpList(OpCode.OP_Halt,(int)SqlResult.SQLITE_OK,(int)OnConstraintError.OE_Abort,0),
																///
																///<summary>
																///3 
																///</summary>
																new VdbeOpList(OpCode.OP_Integer,0,1,0),
																///
																///<summary>
																///4 
																///</summary>
																new VdbeOpList(OpCode.OP_SetCookie,0,BTreeProp.INCR_VACUUM,1),
															///
															///<summary>
															///5 
															///</summary>
															};
															int iAddr;
															iAddr=v.sqlite3VdbeAddOpList(Sqlite3.ArraySize(setMeta6),setMeta6);
															v.sqlite3VdbeChangeP1(iAddr,iDb);
															v.sqlite3VdbeChangeP1(iAddr+1,iDb);
															v.sqlite3VdbeChangeP2(iAddr+2,iAddr+4);
															v.sqlite3VdbeChangeP1(iAddr+4,eAuto-1);
															v.sqlite3VdbeChangeP1(iAddr+5,iDb);
                                                            Engine.vdbeaux.markUsed(v, iDb);
														}
													}
												}
											}
											else
												#endif
												///
												///<summary>
												///PRAGMA [database.]incremental_vacuum(N)
												///
												///Do N steps of incremental vacuuming on a database.
												///</summary>
												#if !SQLITE_OMIT_AUTOVACUUM
												if(zLeft.Equals("incremental_vacuum",StringComparison.InvariantCultureIgnoreCase)) {
													int iLimit=0,addr;
													if(SqlResult.SQLITE_OK!= pParse.sqlite3ReadSchema()) {
														goto pragma_out;
													}
													if(zRight==null||!Converter.sqlite3GetInt32(zRight,ref iLimit)||iLimit<=0) {
														iLimit=0x7fffffff;
													}
													build.sqlite3BeginWriteOperation(pParse,0,iDb);
													v.sqlite3VdbeAddOp2(OpCode.OP_Integer,iLimit,1);
													addr=v.AddOpp1(OpCode.OP_IncrVacuum,iDb);
													v.AddOpp1(OpCode.OP_ResultRow,1);
													v.sqlite3VdbeAddOp2(OpCode.OP_AddImm,1,-1);
													v.sqlite3VdbeAddOp2( OpCode.OP_IfPos,1,addr);
													v.sqlite3VdbeJumpHere(addr);
												}
												else
													#endif
													#if !SQLITE_OMIT_PAGER_PRAGMAS
													///
													///<summary>
													///PRAGMA [database.]cache_size
													///PRAGMA [database.]cache_size=N
													///
													///The first form reports the current local setting for the
													///page cache size.  The local setting can be different from
													///the persistent cache size value that is stored in the database
													///file itself.  The value returned is the maximum number of
													///pages in the page cache.  The second form sets the local
													///page cache size value.  It does not change the persistent
													///cache size stored on the disk so the cache size will revert
													///to its default value when the database is closed and reopened.
													///N should be a positive integer.
													///</summary>
													if(zLeft.Equals("cache_size",StringComparison.InvariantCultureIgnoreCase)) {
														if(SqlResult.SQLITE_OK!= pParse.sqlite3ReadSchema())
															goto pragma_out;
														Debug.Assert(sqlite3SchemaMutexHeld(db,iDb,null));
														if(null==zRight) {
															returnSingleInt(pParse,"cache_size",pDb.pSchema.cache_size);
														}
														else {
                                                            int size = utilc.sqlite3AbsInt32(Converter.sqlite3Atoi(zRight));
															pDb.pSchema.cache_size=size;
															pDb.BTree.SetCacheSize(pDb.pSchema.cache_size);
														}
													}
													else
														///
														///<summary>
														///PRAGMA temp_store
														///PRAGMA temp_store = "default"|"memory"|"file"
														///
														///Return or set the local value of the temp_store flag.  Changing
														///the local value does not make changes to the disk file and the default
														///value will be restored the next time the database is opened.
														///
														///</summary>
														///<param name="Note that it is possible for the library compile">time options to</param>
														///<param name="override this setting">override this setting</param>
														///<param name=""></param>
														if(zLeft.Equals("temp_store",StringComparison.InvariantCultureIgnoreCase)) {
															if(zRight==null) {
																returnSingleInt(pParse,"temp_store",db.temp_store);
															}
															else {
																changeTempStorage(pParse,zRight);
															}
														}
														else
															///
															///<summary>
															///PRAGMA temp_store_directory
															///PRAGMA temp_store_directory = ""|"directory_name"
															///
															///Return or set the local value of the temp_store_directory flag.  Changing
															///the value sets a specific directory to be used for temporary files.
															///Setting to a null string reverts to the default temporary directory search.
															///If temporary directory is changed, then invalidateTempStorage.
															///
															///
															///</summary>
															if(zLeft.Equals("temp_store_directory",StringComparison.InvariantCultureIgnoreCase)) {
																if(null==zRight) {
																	if(sqlite3_temp_directory!="") {
																		v.sqlite3VdbeSetNumCols(1);
                                                                        v.sqlite3VdbeSetColName(0, ColName.NAME, "temp_store_directory", SQLITE_STATIC);
																		v.sqlite3VdbeAddOp4(OpCode.OP_String8,0,1,0,sqlite3_temp_directory,0);
																		v.sqlite3VdbeAddOp2(OpCode.OP_ResultRow,1,1);
																	}
																}
																else {
																	#if !SQLITE_OMIT_WSD
																	if(zRight.Length>0) {
																		SqlResult rc;
																		int res=0;
																		rc=os.sqlite3OsAccess(db.pVfs,zRight,SQLITE_ACCESS.READWRITE,ref res);
																		if(rc!=SqlResult.SQLITE_OK||res==0) {
																			utilc.sqlite3ErrorMsg(pParse,"not a writable directory");
																			goto pragma_out;
																		}
																	}
                                                                    if (sqliteinth.SQLITE_TEMP_STORE == 0 || (sqliteinth.SQLITE_TEMP_STORE == 1 && db.temp_store <= 1) || (sqliteinth.SQLITE_TEMP_STORE == 2 && db.temp_store == 1))
                                                                    {
																		invalidateTempStorage(pParse);
																	}
																	//malloc_cs.sqlite3_free( ref sqlite3_temp_directory );
																	if(zRight.Length>0) {
																		sqlite3_temp_directory=zRight;
																		//io.sqlite3_mprintf("%s", zRight);
																	}
																	else {
																		sqlite3_temp_directory="";
																	}
																	#endif
																}
															}
															else
																#if !(SQLITE_ENABLE_LOCKING_STYLE)
																#if (__APPLE__)
																																																																																																																																																																																																																																																																																																																																																																																																																																//    define SQLITE_ENABLE_LOCKING_STYLE 1
#else
																//#    define SQLITE_ENABLE_LOCKING_STYLE 0
																#endif
																#endif
																#if SQLITE_ENABLE_LOCKING_STYLE
																																																																																																																																																																																																																																																																																																																																																																																																																																/*
**   PRAGMA [database.]lock_proxy_file
**   PRAGMA [database.]lock_proxy_file = ":auto:"|"lock_file_path"
**
** Return or set the value of the lock_proxy_file flag.  Changing
** the value sets a specific file to be used for database access locks.
**
*/
if ( zLeft.Equals( "lock_proxy_file" ,StringComparison.InvariantCultureIgnoreCase )  )
{
if ( zRight !="")
{
Pager pPager = sqlite3BtreePager( pDb.pBt );
int proxy_file_path = 0;
sqlite3_file pFile = sqlite3PagerFile( pPager );
sqlite3OsFileControl( pFile, SQLITE_GET_LOCKPROXYFILE,
ref proxy_file_path );

if ( proxy_file_path!=0 )
{
sqlite3VdbeSetNumCols( v, 1 );
sqlite3VdbeSetColName( v, 0, COLNAME_NAME,
"lock_proxy_file", SQLITE_STATIC );
sqlite3VdbeAddOp4( v, OpCode.OP_String8, 0, 1, 0, proxy_file_path, 0 );
sqlite3VdbeAddOp2( v, OpCode.OP_ResultRow, 1, 1 );
}
}
else
{
Pager pPager = sqlite3BtreePager( pDb.pBt );
sqlite3_file pFile = sqlite3PagerFile( pPager );
int res;
int iDummy = 0;
if ( zRight[0]!=0 )
{
iDummy = zRight[0];
res = sqlite3OsFileControl( pFile, SQLITE_SET_LOCKPROXYFILE,
ref iDummy );
}
else
{
res = sqlite3OsFileControl( pFile, SQLITE_SET_LOCKPROXYFILE,
ref iDummy );
}
if ( res != SqlResult.SQLITE_OK )
{
utilc.sqlite3ErrorMsg( pParse, "failed to set lock proxy file" );
goto pragma_out;
}
}
}
else
#endif
																///
																///<summary>
																///PRAGMA [database.]synchronous
																///PRAGMA [database.]synchronous=OFF|ON|NORMAL|FULL
																///
																///Return or set the local value of the synchronous flag.  Changing
																///the local value does not make changes to the disk file and the
																///default value will be restored the next time the database is
																///opened.
																///</summary>
																if(zLeft.Equals("synchronous",StringComparison.InvariantCultureIgnoreCase)) {
																	if(SqlResult.SQLITE_OK!= pParse.sqlite3ReadSchema())
																		goto pragma_out;
																	if(null==zRight) {
																		returnSingleInt(pParse,"synchronous",pDb.safety_level-1);
																	}
																	else {
																		if(0==db.autoCommit) {
																			utilc.sqlite3ErrorMsg(pParse,"Safety level may not be changed inside a transaction");
																		}
																		else {
																			pDb.safety_level=(byte)(getSafetyLevel(zRight)+1);
																		}
																	}
																}
																else
																	#endif
																	#if !SQLITE_OMIT_FLAG_PRAGMAS
																	if(flagPragma(pParse,zLeft,zRight)!=0) {
																		///
																		///<summary>
																		///The flagPragma() subroutine also generates any necessary code
																		///there is nothing more to do here 
																		///</summary>
																	}
																	else
																		#endif
																		#if !SQLITE_OMIT_SCHEMA_PRAGMAS
																		///
																		///<summary>
																		///PRAGMA table_info(<table>)
																		///
																		///Return a single row for each column of the named table. The columns of
																		///the returned data set are:
																		///
																		///cid:        Column id (numbered from left to right, starting at 0)
																		///name:       Column name
																		///type:       Column declaration type.
																		///notnull:    True if 'NOT NULL' is part of column declaration
																		///dflt_value: The default value for the column, if any.
																		///</summary>
																		if(zLeft.Equals("table_info",StringComparison.InvariantCultureIgnoreCase)&&zRight!=null) {
																			Table pTab;
																			if(SqlResult.SQLITE_OK!= pParse.sqlite3ReadSchema())
																				goto pragma_out;
																			pTab= db.FindByName( zDb, zRight);
																			if(pTab!=null) {
																				int i;
																				int nHidden=0;
																				Column pCol;
																				v.sqlite3VdbeSetNumCols(6);
																				pParse.UsedCellCount=6;
                                                                                v.sqlite3VdbeSetColName(0, ColName.NAME, "cid", SQLITE_STATIC);
                                                                                v.sqlite3VdbeSetColName(1, ColName.NAME, "name", SQLITE_STATIC);
                                                                                v.sqlite3VdbeSetColName(2, ColName.NAME, "type", SQLITE_STATIC);
                                                                                v.sqlite3VdbeSetColName(3, ColName.NAME, "notnull", SQLITE_STATIC);
                                                                                v.sqlite3VdbeSetColName(4, ColName.NAME, "dflt_value", SQLITE_STATIC);
                                                                                v.sqlite3VdbeSetColName(5, ColName.NAME, "pk", SQLITE_STATIC);
																				build.sqlite3ViewGetColumnNames(pParse,pTab);
																				for(i=0;i<pTab.nCol;i++)//, pCol++)
																				 {
																					pCol=pTab.aCol[i];
																					if(pCol.IsHiddenColumn()) {
																						nHidden++;
																						continue;
																					}
																					v.sqlite3VdbeAddOp2(OpCode.OP_Integer,i-nHidden,1);
																					v.sqlite3VdbeAddOp4(OpCode.OP_String8,0,2,0,pCol.zName,0);
																					v.sqlite3VdbeAddOp4(OpCode.OP_String8,0,3,0,pCol.zType!=null?pCol.zType:"",0);
																					v.sqlite3VdbeAddOp2(OpCode.OP_Integer,(pCol.notNull!=0?1:0),4);
																					if(pCol.DefaultValueSource!=null) {
																						v.sqlite3VdbeAddOp4(OpCode.OP_String8,0,5,0,pCol.DefaultValueSource,0);
																					}
																					else {
																						v.sqlite3VdbeAddOp2(OpCode.OP_Null,0,5);
																					}
																					v.sqlite3VdbeAddOp2(OpCode.OP_Integer,pCol.isPrimKey!=0?1:0,6);
																					v.sqlite3VdbeAddOp2(OpCode.OP_ResultRow,1,6);
																				}
																			}
																		}
																		else
																			if(zLeft.Equals("index_info",StringComparison.InvariantCultureIgnoreCase)&&zRight!=null) {
																				Index pIdx;
																				Table pTab;
																				if(SqlResult.SQLITE_OK!= pParse.sqlite3ReadSchema())
																					goto pragma_out;
																				pIdx=IndexBuilder.FindByName(db, zDb, zRight);
																				if(pIdx!=null) {
																					int i;
																					pTab=pIdx.pTable;
																					v.sqlite3VdbeSetNumCols(3);
																					pParse.UsedCellCount=3;
                                                                                    v.sqlite3VdbeSetColName(0, ColName.NAME, "seqno", SQLITE_STATIC);
                                                                                    v.sqlite3VdbeSetColName(1, ColName.NAME, "cid", SQLITE_STATIC);
                                                                                    v.sqlite3VdbeSetColName(2, ColName.NAME, "name", SQLITE_STATIC);
																					for(i=0;i<pIdx.nColumn;i++) {
																						int cnum=pIdx.ColumnIdx[i];
																						v.sqlite3VdbeAddOp2(OpCode.OP_Integer,i,1);
																						v.sqlite3VdbeAddOp2(OpCode.OP_Integer,cnum,2);
																						Debug.Assert(pTab.nCol>cnum);
																						v.sqlite3VdbeAddOp4(OpCode.OP_String8,0,3,0,pTab.aCol[cnum].zName,0);
																						v.sqlite3VdbeAddOp2(OpCode.OP_ResultRow,1,3);
																					}
																				}
																			}
																			else
																				if(zLeft.Equals("index_list",StringComparison.InvariantCultureIgnoreCase)&&zRight!=null) {
																					Index pIdx;
																					Table pTab;
																					if(SqlResult.SQLITE_OK!= pParse.sqlite3ReadSchema())
																						goto pragma_out;
																					pTab=TableBuilder.FindByName(db, zDb, zRight);
																					if(pTab!=null) {
																						v=pParse.sqlite3GetVdbe();
																						pIdx=pTab.pIndex;
																						if(pIdx!=null) {
																							int i=0;
																							v.sqlite3VdbeSetNumCols(3);
																							pParse.UsedCellCount=3;
                                                                                            v.sqlite3VdbeSetColName(0, ColName.NAME, "seq", SQLITE_STATIC);
                                                                                            v.sqlite3VdbeSetColName(1, ColName.NAME, "name", SQLITE_STATIC);
                                                                                            v.sqlite3VdbeSetColName(2, ColName.NAME, "unique", SQLITE_STATIC);
																							while(pIdx!=null) {
																								v.sqlite3VdbeAddOp2(OpCode.OP_Integer,i,1);
																								v.sqlite3VdbeAddOp4(OpCode.OP_String8,0,2,0,pIdx.zName,0);
																								v.sqlite3VdbeAddOp2(OpCode.OP_Integer,(pIdx.onError!=OnConstraintError.OE_None)?1:0,3);
																								v.sqlite3VdbeAddOp2(OpCode.OP_ResultRow,1,3);
																								++i;
																								pIdx=pIdx.pNext;
																							}
																						}
																					}
																				}
																				else
																					if(zLeft.Equals("database_list",StringComparison.InvariantCultureIgnoreCase)) {
																						int i;
																						if(SqlResult.SQLITE_OK!= pParse.sqlite3ReadSchema())
																							goto pragma_out;
																						v.sqlite3VdbeSetNumCols(3);
																						pParse.UsedCellCount=3;
                                                                                        v.sqlite3VdbeSetColName(0, ColName.NAME, "seq", SQLITE_STATIC);
                                                                                        v.sqlite3VdbeSetColName(1, ColName.NAME, "name", SQLITE_STATIC);
                                                                                        v.sqlite3VdbeSetColName(2, ColName.NAME, "file", SQLITE_STATIC);
																						for(i=0;i<db.BackendCount;i++) {
																							if(db.Backends[i].BTree==null)
																								continue;
																							Debug.Assert(db.Backends[i].Name!=null);
																							v.sqlite3VdbeAddOp2(OpCode.OP_Integer,i,1);
																							v.sqlite3VdbeAddOp4(OpCode.OP_String8,0,2,0,db.Backends[i].Name,0);
																							v.sqlite3VdbeAddOp4(OpCode.OP_String8,0,3,0,db.Backends[i].BTree.GetFilename(),0);
																							v.sqlite3VdbeAddOp2(OpCode.OP_ResultRow,1,3);
																						}
																					}
																					else
																						if(zLeft.Equals("collation_list",StringComparison.InvariantCultureIgnoreCase)) {
																							int i=0;
																							HashElem p;
																							v.sqlite3VdbeSetNumCols(2);
																							pParse.UsedCellCount=2;
                                                                                            v.sqlite3VdbeSetColName(0, ColName.NAME, "seq", SQLITE_STATIC);
                                                                                            v.sqlite3VdbeSetColName(1, ColName.NAME, "name", SQLITE_STATIC);
																							for(p=db.aCollSeq.first;p!=null;p=p.pNext)//( p = sqliteHashFirst( db.aCollSeq ) ; p; p = sqliteHashNext( p ) )
																							 {
																								CollSeq pColl=((CollSeq[])p.data)[0];
																								// sqliteHashData( p );
																								v.sqlite3VdbeAddOp2(OpCode.OP_Integer,i++,1);
																								v.sqlite3VdbeAddOp4(OpCode.OP_String8,0,2,0,pColl.zName,0);
																								v.sqlite3VdbeAddOp2(OpCode.OP_ResultRow,1,2);
																							}
																						}
																						else
																							#endif
																							#if !SQLITE_OMIT_FOREIGN_KEY
																							if(zLeft.Equals("foreign_key_list",StringComparison.InvariantCultureIgnoreCase)&&zRight!=null) {
																								FKey pFK;
																								Table pTab;
																								if(SqlResult.SQLITE_OK!= pParse.sqlite3ReadSchema())
																									goto pragma_out;
																								pTab=TableBuilder.FindByName(db, zDb, zRight);
																								if(pTab!=null) {
																									v=pParse.sqlite3GetVdbe();
																									pFK=pTab.pFKey;
																									if(pFK!=null) {
																										int i=0;
																										v.sqlite3VdbeSetNumCols(8);
																										pParse.UsedCellCount=8;
																										v.sqlite3VdbeSetColName(0, ColName.NAME, "id",SQLITE_STATIC);
                                                                                                        v.sqlite3VdbeSetColName(1, ColName.NAME, "seq", SQLITE_STATIC);
                                                                                                        v.sqlite3VdbeSetColName(2, ColName.NAME, "table", SQLITE_STATIC);
                                                                                                        v.sqlite3VdbeSetColName(3, ColName.NAME, "from", SQLITE_STATIC);
                                                                                                        v.sqlite3VdbeSetColName(4, ColName.NAME, "to", SQLITE_STATIC);
                                                                                                        v.sqlite3VdbeSetColName(5, ColName.NAME, "on_update", SQLITE_STATIC);
                                                                                                        v.sqlite3VdbeSetColName(6, ColName.NAME, "on_delete", SQLITE_STATIC);
                                                                                                        v.sqlite3VdbeSetColName(7, ColName.NAME, "match", SQLITE_STATIC);
																										while(pFK!=null) {
																											int j;
																											for(j=0;j<pFK.nCol;j++) {
																												string zCol=pFK.aCol[j].zCol;
																												string zOnDelete=actionName(pFK.aAction[0]);
																												string zOnUpdate=actionName(pFK.aAction[1]);
																												v.sqlite3VdbeAddOp2(OpCode.OP_Integer,i,1);
																												v.sqlite3VdbeAddOp2(OpCode.OP_Integer,j,2);
																												v.sqlite3VdbeAddOp4(OpCode.OP_String8,0,3,0,pFK.zTo,0);
																												v.sqlite3VdbeAddOp4(OpCode.OP_String8,0,4,0,pTab.aCol[pFK.aCol[j].iFrom].zName,0);
                                                                                                                v.sqlite3VdbeAddOp4(zCol != null ? OpCode.OP_String8 : OpCode.OP_Null, 0, 5, 0, zCol, 0);
																												v.sqlite3VdbeAddOp4(OpCode.OP_String8,0,6,0,zOnUpdate,0);
																												v.sqlite3VdbeAddOp4(OpCode.OP_String8,0,7,0,zOnDelete,0);
																												v.sqlite3VdbeAddOp4(OpCode.OP_String8,0,8,0,"NONE",0);
																												v.sqlite3VdbeAddOp2(OpCode.OP_ResultRow,1,8);
																											}
																											++i;
																											pFK=pFK.pNextFrom;
																										}
																									}
																								}
																							}
																							else
																								#endif
																								#if !NDEBUG
																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																                                                if ( zLeft.Equals( "parser_trace" ,StringComparison.InvariantCultureIgnoreCase )  )
                                                {
                                                  if ( zRight != null )
                                                  {
                                                    if ( sqlite3GetBoolean( zRight ) != 0 )
                                                    {
                                                      sqlite3ParserTrace( Console.Out, "parser: " );
                                                    }
                                                    else
                                                    {
                                                      sqlite3ParserTrace( null, "" );
                                                    }
                                                  }
                                                }
                                                else
#endif
																								///
																								///<summary>
																								///Reinstall the LIKE and GLOB functions.  The variant of LIKE
																								///used will be case sensitive or not depending on the RHS.
																								///</summary>
																								if(zLeft.Equals("case_sensitive_like",StringComparison.InvariantCultureIgnoreCase)) {
																									if(zRight!=null) {
																										PredefinedFunctions.sqlite3RegisterLikeFunctions(db,sqlite3GetBoolean(zRight));
																									}
																								}
																								else
																									#if !SQLITE_INTEGRITY_CHECK_ERROR_MAX
																									//const int SQLITE_INTEGRITY_CHECK_ERROR_MAX = 100;
																									#endif
																									#if !SQLITE_OMIT_INTEGRITY_CHECK
																									///
																									///<summary>
																									///Pragma "quick_check" is an experimental reduced version of
																									///integrity_check designed to detect most database corruption
																									///</summary>
																									///<param name="without most of the overhead of a full integrity">check.</param>
																									if(zLeft.Equals("integrity_check",StringComparison.InvariantCultureIgnoreCase)||zLeft.Equals("quick_check",StringComparison.InvariantCultureIgnoreCase)) {
																										const int SQLITE_INTEGRITY_CHECK_ERROR_MAX=100;
																										int i,j,addr,mxErr;
																										///
																										///<summary>
																										///Code that appears at the end of the integrity check.  If no error
																										///messages have been generated, refput OK.  Otherwise output the
																										///error message
																										///
																										///</summary>
																										VdbeOpList[] endCode=new VdbeOpList[] {
																											new VdbeOpList(OpCode.OP_AddImm,1,0,0),
																											///
																											///<summary>
																											///0 
																											///</summary>
																											new VdbeOpList(OpCode.OP_IfNeg,1,0,0),
																											///
																											///<summary>
																											///1 
																											///</summary>
																											new VdbeOpList(OpCode.OP_String8,0,3,0),
																											///
																											///<summary>
																											///2 
																											///</summary>
																											new VdbeOpList(OpCode.OP_ResultRow,3,1,0),
																										};
																										bool isQuick=(zLeft[0]=='q');
																										///
																										///<summary>
																										///Initialize the VDBE program 
																										///</summary>
																										if(SqlResult.SQLITE_OK!= pParse.sqlite3ReadSchema())
																											goto pragma_out;
																										pParse.UsedCellCount=6;
																										v.sqlite3VdbeSetNumCols(1);
                                                                                                        v.sqlite3VdbeSetColName(0, ColName.NAME, "integrity_check", SQLITE_STATIC);
																										///
																										///<summary>
																										///Set the maximum error count 
																										///</summary>
																										mxErr=SQLITE_INTEGRITY_CHECK_ERROR_MAX;
																										if(zRight!=null) {
																											Converter.sqlite3GetInt32(zRight,ref mxErr);
																											if(mxErr<=0) {
																												mxErr=SQLITE_INTEGRITY_CHECK_ERROR_MAX;
																											}
																										}
																										v.sqlite3VdbeAddOp2(OpCode.OP_Integer,mxErr,1);
																										///
																										///<summary>
																										///reg[1] holds errors left 
																										///</summary>
																										///
																										///<summary>
																										///Do an integrity check on each database file 
																										///</summary>
																										for(i=0;i<db.BackendCount;i++) {
																											HashElem x;
																											Hash pTbls;
																											int cnt=0;
																											if(sqliteinth.OMIT_TEMPDB!=0&&i==1)
																												continue;
																											build.sqlite3CodeVerifySchema(pParse,i);
																											addr=v.AddOpp1(OpCode.OP_IfPos,1);
																											///
																											///<summary>
																											///Halt if out of errors 
																											///</summary>
																											v.sqlite3VdbeAddOp2(OpCode.OP_Halt,0,0);
																											v.sqlite3VdbeJumpHere(addr);
																											///
																											///<summary>
																											///</summary>
																											///<param name="Do an integrity check of the B">Tree</param>
																											///<param name=""></param>
																											///<param name="Begin by filling registers 2, 3, ... with the root pages numbers">Begin by filling registers 2, 3, ... with the root pages numbers</param>
																											///<param name="for all tables and indices in the database.">for all tables and indices in the database.</param>
																											///<param name=""></param>
																											Debug.Assert(sqlite3SchemaMutexHeld(db,iDb,null));
																											pTbls=db.Backends[i].pSchema.Tables;
																											for(x=pTbls.first;x!=null;x=x.pNext) {
																												//          for(x=sqliteHashFirst(pTbls); x; x=sqliteHashNext(x)){
																												Table pTab=(Table)x.data;
																												// sqliteHashData( x );
																												Index pIdx;
																												v.sqlite3VdbeAddOp2(OpCode.OP_Integer,pTab.tnum,2+cnt);
																												cnt++;
																												for(pIdx=pTab.pIndex;pIdx!=null;pIdx=pIdx.pNext) {
																													v.sqlite3VdbeAddOp2(OpCode.OP_Integer,pIdx.tnum,2+cnt);
																													cnt++;
																												}
																											}

																											///Make sure sufficient number of registers have been allocated 
																											
																											if(pParse.UsedCellCount<cnt+4) {
																												pParse.UsedCellCount=cnt+4;
																											}
																											
																											///Do the b-tree integrity checks 
																											v.AddOpp3( OpCode.OP_IntegrityCk,2,cnt,1);
																											v.ChangeP5((u8)i);
																											addr=v.AddOpp1(OpCode.OP_IsNull,2);
																											v.sqlite3VdbeAddOp4(OpCode.OP_String8,0,3,0,io.sqlite3MPrintf(db,"*** in database %s ***\n",db.Backends[i].Name), P4Usage.P4_DYNAMIC);
                                                                                                            v.AddOpp3(OpCode.OP_Move, 2, 4, 1);
																											v.AddOpp3(OpCode.OP_Concat,4,3,2);
																											v.sqlite3VdbeAddOp2(OpCode.OP_ResultRow,2,1);
																											v.sqlite3VdbeJumpHere(addr);
																											
                                                                                                            ///Make sure all the indices are constructed correctly.
																											
																											for(x=pTbls.first;x!=null&&!isQuick;x=x.pNext) {
																												;
																												//          for(x=sqliteHashFirst(pTbls); x && !isQuick; x=sqliteHashNext(x)){
																												Table pTab=(Table)x.data;
																												// sqliteHashData( x );
																												Index pIdx;
																												int loopTop;
																												if(pTab.pIndex==null)
																													continue;
																												addr=v.AddOpp1(OpCode.OP_IfPos,1);
																												///
																												///<summary>
																												///Stop if out of errors 
																												///</summary>
																												v.sqlite3VdbeAddOp2(OpCode.OP_Halt,0,0);
																												v.sqlite3VdbeJumpHere(addr);
																												pParse.sqlite3OpenTableAndIndices(pTab,1,OpCode.OP_OpenRead);
																												v.sqlite3VdbeAddOp2(OpCode.OP_Integer,0,2);
																												///
																												///<summary>
																												///reg(2) will count entries 
																												///</summary>
																												loopTop=v.sqlite3VdbeAddOp2( OpCode.OP_Rewind,1,0);
																												v.sqlite3VdbeAddOp2(OpCode.OP_AddImm,2,1);
																												///
																												///<summary>
																												///increment entry count 
																												///</summary>
																												for(j=0,pIdx=pTab.pIndex;pIdx!=null;pIdx=pIdx.pNext,j++) {
																													int jmp2;
																													int r1;
																													VdbeOpList[] idxErr=new VdbeOpList[] {
																														new VdbeOpList(OpCode.OP_AddImm,1,-1,0),
																														new VdbeOpList(OpCode.OP_String8,0,3,0),
																														///
																														///<summary>
																														///1 
																														///</summary>
																														new VdbeOpList(OpCode.OP_Rowid,1,4,0),
																														new VdbeOpList(OpCode.OP_String8,0,5,0),
																														///
																														///<summary>
																														///3 
																														///</summary>
																														new VdbeOpList(OpCode.OP_String8,0,6,0),
																														///
																														///<summary>
																														///4 
																														///</summary>
																														new VdbeOpList(OpCode.OP_Concat,4,3,3),
																														new VdbeOpList(OpCode.OP_Concat,5,3,3),
																														new VdbeOpList(OpCode.OP_Concat,6,3,3),
																														new VdbeOpList(OpCode.OP_ResultRow,3,1,0),
																														new VdbeOpList(OpCode.OP_IfPos,1,0,0),
																														///
																														///<summary>
																														///9 
																														///</summary>
																														new VdbeOpList(OpCode.OP_Halt,0,0,0),
																													};
																													r1=pParse.codegenGenerateIndexKey(pIdx,1,3,false);
																													jmp2=v.sqlite3VdbeAddOp4Int( OpCode.OP_Found,j+2,0,r1,pIdx.nColumn+1);
																													addr=v.sqlite3VdbeAddOpList(Sqlite3.ArraySize(idxErr),idxErr);
																													v.sqlite3VdbeChangeP4(addr+1,"rowid ",SQLITE_STATIC);
																													v.sqlite3VdbeChangeP4(addr+3," missing from index ",SQLITE_STATIC);
																													v.sqlite3VdbeChangeP4(addr+4,pIdx.zName, P4Usage.P4_TRANSIENT);
																													v.sqlite3VdbeJumpHere(addr+9);
																													v.sqlite3VdbeJumpHere(jmp2);
																												}
																												v.sqlite3VdbeAddOp2( OpCode.OP_Next,1,loopTop+1);
																												v.sqlite3VdbeJumpHere(loopTop);
																												for(j=0,pIdx=pTab.pIndex;pIdx!=null;pIdx=pIdx.pNext,j++) {
																													VdbeOpList[] cntIdx=new VdbeOpList[] {
																														new VdbeOpList(OpCode.OP_Integer,0,3,0),
																														new VdbeOpList(OpCode.OP_Rewind,0,0,0),
																														///
																														///<summary>
																														///1 
																														///</summary>
																														new VdbeOpList(OpCode.OP_AddImm,3,1,0),
																														new VdbeOpList(OpCode.OP_Next,0,0,0),
																														///
																														///<summary>
																														///3 
																														///</summary>
																														new VdbeOpList(OpCode.OP_Eq,2,0,3),
																														///
																														///<summary>
																														///4 
																														///</summary>
																														new VdbeOpList(OpCode.OP_AddImm,1,-1,0),
																														new VdbeOpList(OpCode.OP_String8,0,2,0),
																														///
																														///<summary>
																														///6 
																														///</summary>
																														new VdbeOpList(OpCode.OP_String8,0,3,0),
																														///
																														///<summary>
																														///7 
																														///</summary>
																														new VdbeOpList(OpCode.OP_Concat,3,2,2),
																														new VdbeOpList(OpCode.OP_ResultRow,2,1,0),
																													};
																													addr=v.AddOpp1(OpCode.OP_IfPos,1);
																													v.sqlite3VdbeAddOp2(OpCode.OP_Halt,0,0);
																													v.sqlite3VdbeJumpHere(addr);
																													addr=v.sqlite3VdbeAddOpList(Sqlite3.ArraySize(cntIdx),cntIdx);
																													v.sqlite3VdbeChangeP1(addr+1,j+2);
																													v.sqlite3VdbeChangeP2(addr+1,addr+4);
																													v.sqlite3VdbeChangeP1(addr+3,j+2);
																													v.sqlite3VdbeChangeP2(addr+3,addr+2);
																													v.sqlite3VdbeJumpHere(addr+4);
																													v.sqlite3VdbeChangeP4(addr+6,"wrong # of entries in index ", P4Usage.P4_STATIC);
																													v.sqlite3VdbeChangeP4(addr+7,pIdx.zName, P4Usage.P4_TRANSIENT);
																												}
																											}
																										}
																										addr=v.sqlite3VdbeAddOpList(Sqlite3.ArraySize(endCode),endCode);
																										v.sqlite3VdbeChangeP2(addr,-mxErr);
																										v.sqlite3VdbeJumpHere(addr+1);
																										v.sqlite3VdbeChangeP4(addr+2,"ok",P4Usage.P4_STATIC);
																									}
																									else
																										#endif
																										///
																										///<summary>
																										///PRAGMA encoding
																										///</summary>
																										///<param name="PRAGMA encoding = "utf">16be"</param>
																										///<param name=""></param>
																										///<param name="In its first form, this pragma returns the encoding of the main">In its first form, this pragma returns the encoding of the main</param>
																										///<param name="database. If the database is not initialized, it is initialized now.">database. If the database is not initialized, it is initialized now.</param>
																										///<param name=""></param>
																										///<param name="The second form of this pragma is a no">op if the main database file</param>
																										///<param name="has not already been initialized. In this case it sets the default">has not already been initialized. In this case it sets the default</param>
																										///<param name="encoding that will be used for the main database file if a new file">encoding that will be used for the main database file if a new file</param>
																										///<param name="is created. If an existing main database file is opened, then the">is created. If an existing main database file is opened, then the</param>
																										///<param name="default text encoding for the existing database is used.">default text encoding for the existing database is used.</param>
																										///<param name=""></param>
																										///<param name="In all cases new databases created using the ATTACH command are">In all cases new databases created using the ATTACH command are</param>
																										///<param name="created to use the same default text encoding as the main database. If">created to use the same default text encoding as the main database. If</param>
																										///<param name="the main database has not been initialized and/or created when ATTACH">the main database has not been initialized and/or created when ATTACH</param>
																										///<param name="is executed, this is done before the ATTACH operation.">is executed, this is done before the ATTACH operation.</param>
																										///<param name=""></param>
																										///<param name="In the second form this pragma sets the text encoding to be used in">In the second form this pragma sets the text encoding to be used in</param>
																										///<param name="new database files created using this database handle. It is only">new database files created using this database handle. It is only</param>
																										///<param name="useful if invoked immediately after the main database i">useful if invoked immediately after the main database i</param>
																										if(zLeft.Equals("encoding",StringComparison.InvariantCultureIgnoreCase)) {
																											int iEnc;
																											if(null==zRight) {
																												///
																												///<summary>
																												///"PRAGMA encoding" 
																												///</summary>
																												if(SqlResult.SQLITE_OK!= pParse.sqlite3ReadSchema()) {
																													pParse.nErr=0;
																													pParse.zErrMsg=null;
																													pParse.rc=0;
																													//  reset errors goto pragma_out;
																												}
																												v.sqlite3VdbeSetNumCols(1);
                                                                                                                v.sqlite3VdbeSetColName(0, ColName.NAME, "encoding", SQLITE_STATIC);
																												v.sqlite3VdbeAddOp2(OpCode.OP_String8,0,1);
																												Debug.Assert(encnames[(int)SqliteEncoding.UTF8].enc==SqliteEncoding.UTF8);
																												Debug.Assert(encnames[(int)SqliteEncoding.UTF16LE].enc==SqliteEncoding.UTF16LE);
																												Debug.Assert(encnames[(int)SqliteEncoding.UTF16BE].enc==SqliteEncoding.UTF16BE);
                                                                                                                v.sqlite3VdbeChangeP4(-1, encnames[(int)sqliteinth.ENC(pParse.db)].zName,  P4Usage.P4_STATIC);
																												v.sqlite3VdbeAddOp2(OpCode.OP_ResultRow,1,1);
																											}
																											#if !SQLITE_OMIT_UTF16
																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																														else
{                        /* "PRAGMA encoding = XXX" */
/* Only change the value of sqlite.enc if the database handle is not
** initialized. If the main database exists, the new sqlite.enc value
** will be overwritten when the schema is next loaded. If it does not
** already exists, it will be created to use the new encoding value.
*/
if (
//!(DbHasProperty(db, 0, DB_SchemaLoaded)) ||
//DbHasProperty(db, 0, DB_Empty)
( db.flags & DB_SchemaLoaded ) != DB_SchemaLoaded || ( db.flags & DB_Empty ) == DB_Empty
)
{
for ( iEnc = 0 ; encnames[iEnc].zName != null ; iEnc++ )
{
if ( zRight.Equals( encnames[iEnc].zName ,StringComparison.InvariantCultureIgnoreCase ) )
{
pParse.db.aDbStatic[0].pSchema.enc = encnames[iEnc].enc != 0 ? encnames[iEnc].enc : SqliteEncoding.UTF16NATIVE;
break;
}
}
if ( encnames[iEnc].zName == null )
{
utilc.sqlite3ErrorMsg( pParse, "unsupported encoding: %s", zRight );
}
}
}
#endif
																										}
																										else
																											#if !SQLITE_OMIT_SCHEMA_VERSION_PRAGMAS
																											///
																											///<summary>
																											///PRAGMA [database.]schema_version
																											///PRAGMA [database.]schema_version = <integer>
																											///
																											///PRAGMA [database.]user_version
																											///PRAGMA [database.]user_version = <integer>
																											///
																											///The pragma's schema_version and user_version are used to set or get
																											///</summary>
																											///<param name="the value of the schema">version, respectively. Both</param>
																											///<param name="the schema">bit signed integers</param>
																											///<param name="stored in the database header.">stored in the database header.</param>
																											///<param name=""></param>
																											///<param name="The schema">cookie is usually only manipulated internally by SQLite. It</param>
																											///<param name="is incremented by SQLite whenever the database schema is modified (by">is incremented by SQLite whenever the database schema is modified (by</param>
																											///<param name="creating or dropping a table or index). The schema version is used by">creating or dropping a table or index). The schema version is used by</param>
																											///<param name="SQLite each time a query is executed to ensure that the internal cache">SQLite each time a query is executed to ensure that the internal cache</param>
																											///<param name="of the schema used when compiling the SQL query matches the schema of">of the schema used when compiling the SQL query matches the schema of</param>
																											///<param name="the database against which the compiled query is actually executed.">the database against which the compiled query is actually executed.</param>
																											///<param name="Subverting this mechanism by using "PRAGMA schema_version" to modify">Subverting this mechanism by using "PRAGMA schema_version" to modify</param>
																											///<param name="the schema">version is potentially dangerous and may lead to program</param>
																											///<param name="crashes or database corruption. Use with caution!">crashes or database corruption. Use with caution!</param>
																											///<param name=""></param>
																											///<param name="The user">version is not used internally by SQLite. It may be used by</param>
																											///<param name="applications for any purpose.">applications for any purpose.</param>
																											if(zLeft.Equals("schema_version",StringComparison.InvariantCultureIgnoreCase)||zLeft.Equals("user_version",StringComparison.InvariantCultureIgnoreCase)||zLeft.Equals("freelist_count",StringComparison.InvariantCultureIgnoreCase)) {
                                                                                                                BTreeProp iCookie;
																												///Cookie index. 1 for schema-cookie.
                                                                                                                Engine.vdbeaux.markUsed(v, iDb);
																												switch(zLeft[0]) {
																												case 'f':
																												case 'F':
                                                                                                                    iCookie = BTreeProp.FREE_PAGE_COUNT;
																												break;
																												case 's':
																												case 'S':
                                                                                                                iCookie = BTreeProp.SCHEMA_VERSION;
																												break;
																												default:
                                                                                                                iCookie = BTreeProp.USER_VERSION;
																												break;
																												}
                                                                                                                if (zRight != null && iCookie != BTreeProp.FREE_PAGE_COUNT)
                                                                                                                {
																													///Write the specified cookie value 
																													VdbeOpList[] setCookie=new VdbeOpList[] {
																														new VdbeOpList(OpCode.OP_Transaction,0,1,0),
																														///0 
																														new VdbeOpList(OpCode.OP_Integer,0,1,0),
																														///1 
																														new VdbeOpList(OpCode.OP_SetCookie,0,0,1),
																													    ///2 
																													};
																													int addr=v.sqlite3VdbeAddOpList(Sqlite3.ArraySize(setCookie),setCookie);
																													v.sqlite3VdbeChangeP1(addr,iDb);
																													v.sqlite3VdbeChangeP1(addr+1,Converter.sqlite3Atoi(zRight));
																													v.sqlite3VdbeChangeP1(addr+2,iDb);
																													v.sqlite3VdbeChangeP2(addr+2,(int)iCookie);
																												}
																												else {
																													///
																													///<summary>
																													///Read the specified cookie value 
																													///</summary>
																													VdbeOpList[] readCookie=new VdbeOpList[] {
																														new VdbeOpList(OpCode.OP_Transaction,0,0,0),
																														///
																														///<summary>
																														///0 
																														///</summary>
																														new VdbeOpList(OpCode.OP_ReadCookie,0,1,0),
																														///
																														///<summary>
																														///1 
																														///</summary>
																														new VdbeOpList(OpCode.OP_ResultRow,1,1,0)
																													};
																													int addr=v.sqlite3VdbeAddOpList(readCookie.Length,readCookie);
																													// Sqlite3.ArraySize(readCookie), readCookie);
																													v.sqlite3VdbeChangeP1(addr,iDb);
																													v.sqlite3VdbeChangeP1(addr+1,iDb);
																													v.sqlite3VdbeChangeP3(addr+1,(int)iCookie);
																													v.sqlite3VdbeSetNumCols(1);
                                                                                                                    v.sqlite3VdbeSetColName(0, ColName.NAME, zLeft, SQLITE_TRANSIENT);
																												}
																											}
																											else
																												if(zLeft.Equals("reload_schema",StringComparison.InvariantCultureIgnoreCase)) {
																													///
																													///<summary>
																													///force schema reloading
																													///</summary>
																													build.sqlite3ResetInternalSchema(db,-1);
																												}
																												else
																													if(zLeft.Equals("file_format",StringComparison.InvariantCultureIgnoreCase)) {
																														pDb.pSchema.file_format=(u8)_Custom.atoi(zRight);
																														build.sqlite3ResetInternalSchema(db,-1);
																													}
																													else
																														#endif
																														#if !SQLITE_OMIT_COMPILEOPTION_DIAGS
																														///
																														///<summary>
																														///PRAGMA compile_options
																														///
																														///</summary>
																														///<param name="Return the names of all compile">time options used in this build,</param>
																														///<param name="one option per row.">one option per row.</param>
																														if(zLeft.Equals("compile_options",StringComparison.InvariantCultureIgnoreCase)) {
																															int i=0;
																															string zOpt;
																															v.sqlite3VdbeSetNumCols(1);
																															pParse.UsedCellCount=1;
                                                                                                                            v.sqlite3VdbeSetColName(0, ColName.NAME, "compile_option", SQLITE_STATIC);
																															while((zOpt=sqlite3_compileoption_get(i++))!=null) {
																																v.sqlite3VdbeAddOp4(OpCode.OP_String8,0,1,0,zOpt,0);
																																v.sqlite3VdbeAddOp2(OpCode.OP_ResultRow,1,1);
																															}
																														}
																														else
																															#endif
																															#if !SQLITE_OMIT_WAL
																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																						  /*
  **   PRAGMA [database.]wal_checkpoint = passive|full|restart
  **
  ** Checkpoint the database.
  */
  if( sqlite3StrICmp(zLeft, "wal_checkpoint")==0 ){
    int iBt = (pId2->z?iDb:SQLITE_MAX_ATTACHED);
    int eMode = SQLITE_CHECKPOINT_PASSIVE;
    if( zRight ){
      if( sqlite3StrICmp(zRight, "full")==0 ){
        eMode = SQLITE_CHECKPOINT_FULL;
      }else if( sqlite3StrICmp(zRight, "restart")==0 ){
        eMode = SQLITE_CHECKPOINT_RESTART;
      }
    }
    if( sqlite3ReadSchema(pParse) ) goto pragma_out;
    sqlite3VdbeSetNumCols(v, 3);
    pParse->nMem = 3;
    sqlite3VdbeSetColName(v, 0, COLNAME_NAME, "busy", SQLITE_STATIC);
    sqlite3VdbeSetColName(v, 1, COLNAME_NAME, "log", SQLITE_STATIC);
    sqlite3VdbeSetColName(v, 2, COLNAME_NAME, "checkpointed", SQLITE_STATIC);

    sqlite3VdbeAddOp3(v,  OpCode.OP_Checkpoint, iBt, eMode, 1);
    sqlite3VdbeAddOp2(v, OpCode.OP_ResultRow, 1, 3);
  }else

  /*
  **   PRAGMA wal_autocheckpoint
  **   PRAGMA wal_autocheckpoint = N
  **
  ** Configure a database connection to automatically checkpoint a database
  ** after accumulating N frames in the log. Or query for the current value
  ** of N.
  */
  if( sqlite3StrICmp(zLeft, "wal_autocheckpoint")==0 ){
    if( zRight ){
      sqlite3_wal_autocheckpoint(db, refaactorrConverter__sqlite3Atoi(zRight));
    }
    returnSingleInt(pParse, "wal_autocheckpoint", 
       db->xWalCallback==sqlite3WalDefaultHook ? 
           SQLITE_PTR_TO_INT(db->pWalArg) : 0);
  }else
#endif
																															#if SQLITE_DEBUG || SQLITE_TEST
																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																						                                                            /*
** Report the current state of file logs for all databases
*/
                                                            if ( zLeft.Equals( "lock_status" ,StringComparison.InvariantCultureIgnoreCase )  )
                                                            {
                                                              string[] azLockName = {
"unlocked", "shared", "reserved", "pending", "exclusive"
};
                                                              int i;
                                                              sqlite3VdbeSetNumCols( v, 2 );
                                                              pParse.nMem = 2;
                                                              sqlite3VdbeSetColName( v, 0, COLNAME_NAME, "database", SQLITE_STATIC );
                                                              sqlite3VdbeSetColName( v, 1, COLNAME_NAME, "status", SQLITE_STATIC );
                                                              for ( i = 0; i < db.nDb; i++ )
                                                              {
                                                                Btree pBt;
                                                                Pager pPager;
                                                                string zState = "unknown";
                                                                sqlite3_int64 j = 0;
                                                                if ( db.aDb[i].zName == null )
                                                                  continue;
                                                                sqlite3VdbeAddOp4( v, OpCode.OP_String8, 0, 1, 0, db.aDb[i].zName,  P4Usage.P4_STATIC );
                                                                pBt = db.aDb[i].pBt;
                                                                if ( pBt == null || ( pPager = sqlite3BtreePager( pBt ) ) == null )
                                                                {
                                                                  zState = "closed";
                                                                }
                                                                else if ( sqlite3_file_control( db, i != 0 ? db.aDb[i].zName : null,
                                                         SQLITE_FCNTL_LOCKSTATE, ref j ) == SqlResult.SQLITE_OK )
                                                                {
                                                                  zState = azLockName[j];
                                                                }
                                                                sqlite3VdbeAddOp4( v, OpCode.OP_String8, 0, 2, 0, zState,  P4Usage.P4_STATIC );
                                                                sqlite3VdbeAddOp2( v, OpCode.OP_ResultRow, 1, 2 );
                                                              }
                                                            }
                                                            else
#endif
																															#if SQLITE_HAS_CODEC
																															// needed to support key/rekey/hexrekey with pragma cmds
																															if(zLeft.Equals("key",StringComparison.InvariantCultureIgnoreCase)&&!String.IsNullOrEmpty(zRight)) {
                                                                                                                                crypto.sqlite3_key(db, zRight, StringExtensions.Strlen30(zRight));
																															}
																															else
																																if(zLeft.Equals("rekey",StringComparison.InvariantCultureIgnoreCase)&&!String.IsNullOrEmpty(zRight)) {
																																	crypto.sqlite3_rekey(db,zRight,StringExtensions.Strlen30(zRight));
																																}
																																else
																																	if(!String.IsNullOrEmpty(zRight)&&(zLeft.Equals("hexkey",StringComparison.InvariantCultureIgnoreCase)||zLeft.Equals("hexrekey",StringComparison.InvariantCultureIgnoreCase))) {
																																		StringBuilder zKey=new StringBuilder(40);
																																		zRight.ToLower(new CultureInfo("en-us"));
																																		// expected '0x0102030405060708090a0b0c0d0e0f10'
																																		if(zRight.Length!=34)
																																			return;
																																		for(int i=2;i<zRight.Length;i+=2) {
																																			int h1=zRight[i];
																																			int h2=zRight[i+1];
																																			h1+=9*(1&(h1>>6));
																																			h2+=9*(1&(h2>>6));
																																			zKey.Append(Convert.ToChar((h2&0x0f)|((h1&0xf)<<4)));
																																		}
																																		if((zLeft[3]&0xf)==0xb) {
                                                                                                                                            crypto.sqlite3_key(db, zKey.ToString(), zKey.Length);
																																		}
																																		else {
																																			crypto.sqlite3_rekey(db,zKey.ToString(),zKey.Length);
																																		}
																																	}
																																	else
																																		#endif
																																		#if SQLITE_HAS_CODEC || SQLITE_ENABLE_CEROD
																																		if(zLeft.Equals("activate_extensions",StringComparison.InvariantCultureIgnoreCase)) {
																																			#if SQLITE_HAS_CODEC
																																			if(!String.IsNullOrEmpty(zRight)&&zRight.Length>4&&StringExtensions.sqlite3StrNICmp(zRight,"see-",4)==0) {
                                                                                                                                                crypto.sqlite3_activate_see(zRight.Substring(4));
																																			}
																																			#endif
																																			#if SQLITE_ENABLE_CEROD
																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																														if( StringExtensions.sqlite3StrNICmp(zRight, "cerod-", 6)==0 ){
sqlite3_activate_cerod(&zRight[6]);
}
#endif
																																		}
																																		else
																																		#endif
																																		 {
																																			///
																																			///<summary>
																																			///Empty ELSE clause 
																																			///</summary>
																																		}
			///
			///<summary>
			///Reset the safety level, in case the fullfsync flag or synchronous
			///setting changed.
			///
			///</summary>
			#if !SQLITE_OMIT_PAGER_PRAGMAS
			if(db.autoCommit!=0) {
                pDb.BTree.sqlite3BtreeSetSafetyLevel(pDb.safety_level, ((db.flags & SqliteFlags.SQLITE_FullFSync) != 0) ? 1 : 0, ((db.flags & SqliteFlags.SQLITE_CkptFullFSync) != 0) ? 1 : 0);
			}
			#endif
			pragma_out:
			db.DbFree(ref zLeft);
			db.DbFree(ref zRight);
			;
		}
	#endif
	}

    ///<summary>
    /// Process a pragma statement.
    ///
    /// Pragmas are of this form:
    ///
    ///      PRAGMA [database.]id [= value]
    ///
    /// The identifier might also be a string.  The value is a string, and
    /// identifier, or a number.  If minusFlag is true, then the value is
    /// a number that was preceded by a minus sign.
    ///
    /// If the left side is "database.id" then pId1 is the database name
    /// and pId2 is the id.  If the left side is just "id" then pId1 is the
    /// id and pId2 is any empty string.
    ///
    ///</summary>
    public class EncName
    {
        public string zName;
        public SqliteEncoding enc;
        public EncName(string zName, SqliteEncoding enc)
        {
            this.zName = zName;
            this.enc = enc;
        }
    };


#if !SQLITE_OMIT_FLAG_PRAGMAS
    ///<summary>
    /// Check to see if zRight and zLeft refer to a pragma that queries
    /// or changes one of the flags in db.flags.  Return 1 if so and 0 if not.
    /// Also, implement the pragma.
    ///</summary>
    struct sPragmaType
    {
        public string zName;
        ///
        ///<summary>
        ///Name of the pragma 
        ///</summary>
        public SqliteFlags mask;
        ///
        ///<summary>
        ///Mask for the db.flags value 
        ///</summary>
        public sPragmaType(string zName, SqliteFlags mask)
        {
            this.zName = zName;
            this.mask = mask;
        }
    }
#endif
}

