using System;
using System.Diagnostics;
using System.Text;
using Bitmask = System.UInt64;
using u8 = System.Byte;
using u16 = System.UInt16;
using u32 = System.UInt32;
using Community.CsharpSqlite.Ast;
using Community.CsharpSqlite.Os;
using Community.CsharpSqlite.Engine;
using Community.CsharpSqlite.Utils;
using Community.CsharpSqlite.Compiler;

namespace Community.CsharpSqlite
{
	public partial class Sqlite3
	{
		///<summary>
		/// 2009 November 25
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
		/// This file contains code used to insert the values of host parameters
		/// (aka "wildcards") into the SQL text output by sqlite3_trace().
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
		//#include "vdbeInt.h"
		#if !SQLITE_OMIT_TRACE
		///<summary>
		/// zSql is a zero-terminated string of UTF-8 SQL text.  Return the number of
		/// bytes in this text up to but excluding the first character in
		/// a host parameter.  If the text contains no host parameters, return
		/// the total number of bytes in the text.
		///</summary>
		static int findNextHostParameter (string zSql, int iOffset, ref int pnToken)
		{
			TokenType tokenType = 0;
			int nTotal = 0;
			int n;
			pnToken = 0;
			while (iOffset < zSql.Length) {
                var token = Lexer.GetToken(zSql, iOffset);
				n = token.Length;
				Debug.Assert (n > 0 && token.TokenType != TokenType.TK_ILLEGAL);
				if (tokenType == TokenType.TK_VARIABLE) {
					pnToken = n;
					break;
				}
				nTotal += n;
				iOffset += n;
				// zSql += n;
			}
			return nTotal;
		}

		///
///<summary>
///</summary>
///<param name="This function returns a pointer to a nul">terminated string in memory</param>
///<param name="obtained from sqlite3DbMalloc(). If sqlite3.vdbeExecCnt is 1, then the">obtained from sqlite3DbMalloc(). If sqlite3.vdbeExecCnt is 1, then the</param>
///<param name="string contains a copy of zRawSql but with host parameters expanded to ">string contains a copy of zRawSql but with host parameters expanded to </param>
///<param name="their current bindings. Or, if sqlite3.vdbeExecCnt is greater than 1, ">their current bindings. Or, if sqlite3.vdbeExecCnt is greater than 1, </param>
///<param name="then the returned string holds a copy of zRawSql with ""> " prepended</param>
///<param name="to each line of text.">to each line of text.</param>
///<param name=""></param>
///<param name="The calling function is responsible for making sure the memory returned">The calling function is responsible for making sure the memory returned</param>
///<param name="is eventually freed.">is eventually freed.</param>
///<param name=""></param>
///<param name="ALGORITHM:  Scan the input string looking for host parameters in any of">ALGORITHM:  Scan the input string looking for host parameters in any of</param>
///<param name="these forms:  ?, ?N, $A, @A, :A.  Take care to avoid text within">these forms:  ?, ?N, $A, @A, :A.  Take care to avoid text within</param>
///<param name="string literals, quoted identifier names, and comments.  For text forms,">string literals, quoted identifier names, and comments.  For text forms,</param>
///<param name="the host parameter index is found by scanning the perpared">the host parameter index is found by scanning the perpared</param>
///<param name="statement for the corresponding OP_Variable opcode.  Once the host">statement for the corresponding OP_Variable opcode.  Once the host</param>
///<param name="parameter index is known, locate the value in p">>aVar[].  Then render</param>
///<param name="the value as a literal in place of the host parameter name.">the value as a literal in place of the host parameter name.</param>
///<param name=""></param>

		public static string sqlite3VdbeExpandSql (Vdbe p, ///
///<summary>
///The prepared statement being evaluated 
///</summary>

		string zRawSql///
///<summary>
///Raw text of the SQL statement 
///</summary>

		)
		{
			Connection db;
			///
///<summary>
///The database connection 
///</summary>

			int idx = 0;
			///
///<summary>
///Index of a host parameter 
///</summary>

			int nextIndex = 1;
			///
///<summary>
///Index of next ? host parameter 
///</summary>

			int n;
			///
///<summary>
///Length of a token prefix 
///</summary>

			int nToken = 0;
			///
///<summary>
///Length of the parameter token 
///</summary>

			int i;
			///
///<summary>
///Loop counter 
///</summary>

			Mem pVar;
			///
///<summary>
///Value of a host parameter 
///</summary>

			StrAccum _out = new StrAccum (1000);
			///
///<summary>
///Accumulate the _output here 
///</summary>

			StringBuilder zBase = new StringBuilder (100);
			///
///<summary>
///Initial working space 
///</summary>

			int izRawSql = 0;
			db = p.db;
			io.sqlite3StrAccumInit (_out, null, 100, db.aLimit [Globals.SQLITE_LIMIT_LENGTH]);
			_out.db = db;
			if (db.callStackDepth > 1) {
				while (izRawSql < zRawSql.Length) {
					//string zStart = zRawSql;
					while (zRawSql [izRawSql++] != '\n' && izRawSql < zRawSql.Length)
						;
                    _out.sqlite3StrAccumAppend("-- ", 3);
                    _out.sqlite3StrAccumAppend(zRawSql, (int)izRawSql);
					//zRawSql - zStart );
				}
			}
			else {
				while (izRawSql < zRawSql.Length) {
					n = findNextHostParameter (zRawSql, izRawSql, ref nToken);
					Debug.Assert (n > 0);
                    _out.sqlite3StrAccumAppend(zRawSql.Substring(izRawSql, n), n);
					izRawSql += n;
					Debug.Assert (izRawSql < zRawSql.Length || nToken == 0);
					if (nToken == 0)
						break;
					if (zRawSql [izRawSql] == '?') {
						if (nToken > 1) {
							Debug.Assert (CharExtensions.sqlite3Isdigit (zRawSql [izRawSql + 1]));
							Converter.sqlite3GetInt32 (zRawSql, izRawSql + 1, ref idx);
						}
						else {
							idx = nextIndex;
						}
					}
					else {
						Debug.Assert (zRawSql [izRawSql] == ':' || zRawSql [izRawSql] == '$' || zRawSql [izRawSql] == '@');
						sqliteinth.testcase (zRawSql [izRawSql] == ':');
						sqliteinth.testcase (zRawSql [izRawSql] == '$');
						sqliteinth.testcase (zRawSql [izRawSql] == '@');
						idx = vdbeapi.sqlite3VdbeParameterIndex (p, zRawSql.Substring (izRawSql, nToken), nToken);
						Debug.Assert (idx > 0);
					}
					izRawSql += nToken;
					nextIndex = idx + 1;
					Debug.Assert (idx > 0 && idx <= p.nVar);
					pVar = p.aVar [idx - 1];
					if ((pVar.flags & MemFlags.MEM_Null) != 0) {
                        _out.sqlite3StrAccumAppend("NULL", 4);
					}
					else
						if ((pVar.flags & MemFlags.MEM_Int) != 0) {
							io.sqlite3XPrintf (_out, "%lld", pVar.u.AsInteger);
						}
						else
							if ((pVar.flags & MemFlags.MEM_Real) != 0) {
								io.sqlite3XPrintf (_out, "%!.15g", pVar.AsReal);
							}
							else
								if ((pVar.flags & MemFlags.MEM_Str) != 0) {
									#if !SQLITE_OMIT_UTF16
																																																																																																																																																																											SqliteEncoding enc = ENC(db);
if( enc!=SqliteEncoding.UTF8 ){
Mem utf8;
memset(&utf8, 0, sizeof(utf8));
utf8.db = db;
sqlite3VdbeMemSetStr(&utf8, pVar.z, pVar.n, enc, SQLITE_STATIC);
sqlite3VdbeChangeEncoding(&utf8, SqliteEncoding.UTF8);
io.sqlite3XPrintf(_out, "'%.*q'", utf8.n, utf8.z);
&utf8.sqlite3VdbeMemRelease();
}else
#endif
									{
										io.sqlite3XPrintf (_out, "'%.*q'", pVar.CharacterCount, pVar.AsString);
									}
								}
								else
									if ((pVar.flags & MemFlags.MEM_Zero) != 0) {
										io.sqlite3XPrintf (_out, "zeroblob(%d)", pVar.u.nZero);
									}
									else {
										Debug.Assert ((pVar.flags & MemFlags.MEM_Blob) != 0);
                                        _out.sqlite3StrAccumAppend("x'", 2);
										for (i = 0; i < pVar.CharacterCount; i++) {
											io.sqlite3XPrintf (_out, "%02x", pVar.zBLOB [i] & 0xff);
										}
                                        _out.sqlite3StrAccumAppend( "'", 1);
									}
				}
			}
			return io.sqlite3StrAccumFinish (_out);
		}
	#endif
	}
}
