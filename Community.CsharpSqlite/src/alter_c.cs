using System;
using System.Diagnostics;
using System.Text;
namespace Community.CsharpSqlite {
	using sqlite3_value=Sqlite3.Mem;
	public partial class Sqlite3 {
		/*
** 2005 February 15
**
** The author disclaims copyright to this source code.  In place of
** a legal notice, here is a blessing:
**
**    May you do good and not evil.
**    May you find forgiveness for yourself and forgive others.
**    May you share freely, never taking more than you give.
**
*************************************************************************
** This file contains C code routines that used to generate VDBE code
** that implements the ALTER TABLE command.
*************************************************************************
**  Included in SQLite3 port to C#-SQLite;  2008 Noah B Hart
**  C#-SQLite is an independent reimplementation of the SQLite software library
**
**  SQLITE_SOURCE_ID: 2011-06-23 19:49:22 4374b7e83ea0a3fbc3691f9c0c936272862f32f2
**
*************************************************************************
*///#include "sqliteInt.h"
		///<summary>
		/// The code in this file only exists if we are not omitting the
		/// ALTER TABLE logic from the build.
		///</summary>
		#if !SQLITE_OMIT_ALTERTABLE
		/*
** This function is used by SQL generated to implement the
** ALTER TABLE command. The first argument is the text of a CREATE TABLE or
** CREATE INDEX command. The second is a table name. The table name in
** the CREATE TABLE or CREATE INDEX statement is replaced with the third
** argument and the result returned. Examples:
**
** sqlite_rename_table('CREATE TABLE abc(a, b, c)', 'def')
**     . 'CREATE TABLE def(a, b, c)'
**
** sqlite_rename_table('CREATE INDEX i ON abc(a)', 'def')
**     . 'CREATE INDEX i ON def(a, b, c)'
*/static void renameTableFunc(sqlite3_context context,int NotUsed,sqlite3_value[] argv) {
			string bResult=sqlite3_value_text(argv[0]);
			string zSql=bResult==null?"":bResult;
			string zTableName=sqlite3_value_text(argv[1]);
			int token=0;
			Token tname=new Token();
			int zCsr=0;
			int zLoc=0;
			int len=0;
			string zRet;
			sqlite3 db=sqlite3_context_db_handle(context);
			UNUSED_PARAMETER(NotUsed);
			/* The principle used to locate the table name in the CREATE TABLE
  ** statement is that the table name is the first non-space token that
  ** is immediately followed by a TK_LP or TK_USING token.
  */if(zSql!="") {
				do {
					if(zCsr==zSql.Length) {
						/* Ran out of input before finding an opening bracket. Return NULL. */return;
					}
					/* Store the token that zCsr points to in tname. */zLoc=zCsr;
					tname.z=zSql.Substring(zCsr);
					//(char*)zCsr;
					tname.n=len;
					/* Advance zCsr to the next token. Store that token type in 'token',
      ** and its length in 'len' (to be used next iteration of this loop).
      */do {
						zCsr+=len;
						len=(zCsr==zSql.Length)?1:sqlite3GetToken(zSql,zCsr,ref token);
					}
					while(token==TK_SPACE);
					Debug.Assert(len>0);
				}
				while(token!=TK_LP&&token!=TK_USING);
				zRet=sqlite3MPrintf(db,"%.*s\"%w\"%s",zLoc,zSql.Substring(0,zLoc),zTableName,zSql.Substring(zLoc+tname.n));
				context.sqlite3_result_text(zRet,-1,SQLITE_DYNAMIC);
			}
		}
		///<summary>
		/// This C function implements an SQL user function that is used by SQL code
		/// generated by the ALTER TABLE ... RENAME command to modify the definition
		/// of any foreign key constraints that use the table being renamed as the
		/// parent table. It is passed three arguments:
		///
		///   1) The complete text of the CREATE TABLE statement being modified,
		///   2) The old name of the table being renamed, and
		///   3) The new name of the table being renamed.
		///
		/// It returns the new CREATE TABLE statement. For example:
		///
		///   sqlite_rename_parent('CREATE TABLE t1(a REFERENCES t2)', 't2', 't3')
		///       -> 'CREATE TABLE t1(a REFERENCES t3)'
		///</summary>
		#if !SQLITE_OMIT_FOREIGN_KEY
		static void renameParentFunc(sqlite3_context context,int NotUsed,sqlite3_value[] argv) {
			sqlite3 db=sqlite3_context_db_handle(context);
			string zOutput="";
			string zResult;
			string zInput=sqlite3_value_text(argv[0]);
			string zOld=sqlite3_value_text(argv[1]);
			string zNew=sqlite3_value_text(argv[2]);
			int zIdx;
			/* Pointer to token */int zLeft=0;
			/* Pointer to remainder of String */int n=0;
			/* Length of token z */int token=0;
			/* Type of token */UNUSED_PARAMETER(NotUsed);
			for(zIdx=0;zIdx<zInput.Length;zIdx+=n)//z=zInput; *z; z=z+n)
			 {
				n=sqlite3GetToken(zInput,zIdx,ref token);
				if(token==TK_REFERENCES) {
					string zParent;
					do {
						zIdx+=n;
						n=sqlite3GetToken(zInput,zIdx,ref token);
					}
					while(token==TK_SPACE);
					zParent=zIdx+n<zInput.Length?zInput.Substring(zIdx,n):"";
					//sqlite3DbStrNDup(db, zIdx, n);
					if(String.IsNullOrEmpty(zParent))
						break;
					StringExtensions.sqlite3Dequote(ref zParent);
					if(zOld.Equals(zParent,StringComparison.InvariantCultureIgnoreCase)) {
						string zOut=sqlite3MPrintf(db,"%s%.*s\"%w\"",zOutput,zIdx-zLeft,zInput.Substring(zLeft),zNew);
						db.sqlite3DbFree(ref zOutput);
						zOutput=zOut;
						zIdx+=n;
						// zInput = &z[n];
						zLeft=zIdx;
					}
					db.sqlite3DbFree(ref zParent);
				}
			}
			zResult=sqlite3MPrintf(db,"%s%s",zOutput,zInput.Substring(zLeft));
			context.sqlite3_result_text(zResult,-1,SQLITE_DYNAMIC);
			db.sqlite3DbFree(ref zOutput);
		}
		#endif
		#if !SQLITE_OMIT_TRIGGER
		/* This function is used by SQL generated to implement the
** ALTER TABLE command. The first argument is the text of a CREATE TRIGGER
** statement. The second is a table name. The table name in the CREATE
** TRIGGER statement is replaced with the third argument and the result
** returned. This is analagous to renameTableFunc() above, except for CREATE
** TRIGGER, not CREATE INDEX and CREATE TABLE.
*/static void renameTriggerFunc(sqlite3_context context,int NotUsed,sqlite3_value[] argv) {
			string zSql=sqlite3_value_text(argv[0]);
			string zTableName=sqlite3_value_text(argv[1]);
			int token=0;
			Token tname=new Token();
			int dist=3;
			int zCsr=0;
			int zLoc=0;
			int len=1;
			string zRet;
			sqlite3 db=sqlite3_context_db_handle(context);
			UNUSED_PARAMETER(NotUsed);
			/* The principle used to locate the table name in the CREATE TRIGGER
  ** statement is that the table name is the first token that is immediatedly
  ** preceded by either TK_ON or TK_DOT and immediatedly followed by one
  ** of TK_WHEN, TK_BEGIN or TK_FOR.
  */if(zSql!=null) {
				do {
					if(zCsr==zSql.Length) {
						/* Ran out of input before finding the table name. Return NULL. */return;
					}
					/* Store the token that zCsr points to in tname. */zLoc=zCsr;
					tname.z=zSql.Substring(zCsr,len);
					//(char*)zCsr;
					tname.n=len;
					/* Advance zCsr to the next token. Store that token type in 'token',
      ** and its length in 'len' (to be used next iteration of this loop).
      */do {
						zCsr+=len;
						len=(zCsr==zSql.Length)?1:sqlite3GetToken(zSql,zCsr,ref token);
					}
					while(token==TK_SPACE);
					Debug.Assert(len>0);
					/* Variable 'dist' stores the number of tokens read since the most
      ** recent TK_DOT or TK_ON. This means that when a WHEN, FOR or BEGIN
      ** token is read and 'dist' equals 2, the condition stated above
      ** to be met.
      **
      ** Note that ON cannot be a database, table or column name, so
      ** there is no need to worry about syntax like
      ** "CREATE TRIGGER ... ON ON.ON BEGIN ..." etc.
      */dist++;
					if(token==TK_DOT||token==TK_ON) {
						dist=0;
					}
				}
				while(dist!=2||(token!=TK_WHEN&&token!=TK_FOR&&token!=TK_BEGIN));
				/* Variable tname now contains the token that is the old table-name
    ** in the CREATE TRIGGER statement.
    */zRet=sqlite3MPrintf(db,"%.*s\"%w\"%s",zLoc,zSql.Substring(0,zLoc),zTableName,zSql.Substring(zLoc+tname.n));
				context.sqlite3_result_text(zRet,-1,SQLITE_DYNAMIC);
			}
		}
		#endif
		///<summary>
		/// Register built-in functions used to help implement ALTER TABLE
		///</summary>
		static FuncDef[] aAlterTableFuncs;
		static void sqlite3AlterFunctions() {
			aAlterTableFuncs=new FuncDef[] {
				FUNCTION("sqlite_rename_table",2,0,0,renameTableFunc),
				#if !SQLITE_OMIT_TRIGGER
				FUNCTION("sqlite_rename_trigger",2,0,0,renameTriggerFunc),
				#endif
				#if !SQLITE_OMIT_FOREIGN_KEY
				FUNCTION("sqlite_rename_parent",3,0,0,renameParentFunc),
			#endif
			};
			int i;
			#if SQLITE_OMIT_WSD
																																																									  FuncDefHash pHash = GLOBAL(FuncDefHash, sqlite3GlobalFunctions);
  FuncDef[] aFunc = GLOBAL(FuncDef, aAlterTableFuncs);
#else
			FuncDefHash pHash=sqlite3GlobalFunctions;
			FuncDef[] aFunc=aAlterTableFuncs;
			#endif
			for(i=0;i<ArraySize(aAlterTableFuncs);i++) {
				sqlite3FuncDefInsert(pHash,aFunc[i]);
			}
		}
		///<summary>
		/// This function is used to create the text of expressions of the form:
		///
		///   name=<constant1> OR name=<constant2> OR ...
		///
		/// If argument zWhere is NULL, then a pointer string containing the text
		/// "name=<constant>" is returned, where <constant> is the quoted version
		/// of the string passed as argument zConstant. The returned buffer is
		/// allocated using sqlite3DbMalloc(). It is the responsibility of the
		/// caller to ensure that it is eventually freed.
		///
		/// If argument zWhere is not NULL, then the string returned is
		/// "<where> OR name=<constant>", where <where> is the contents of zWhere.
		/// In this case zWhere is passed to sqlite3DbFree() before returning.
		///
		///</summary>
		static string whereOrName(sqlite3 db,string zWhere,string zConstant) {
			string zNew;
			if(String.IsNullOrEmpty(zWhere)) {
				zNew=sqlite3MPrintf(db,"name=%Q",zConstant);
			}
			else {
				zNew=sqlite3MPrintf(db,"%s OR name=%Q",zWhere,zConstant);
				db.sqlite3DbFree(ref zWhere);
			}
			return zNew;
		}
	#if !(SQLITE_OMIT_FOREIGN_KEY) && !(SQLITE_OMIT_TRIGGER)
	#endif
	#endif
	}
}
