using Community.CsharpSqlite.Ast;
using Community.CsharpSqlite.Utils;
using System;
using System.Diagnostics;
using System.Text;

namespace Community.CsharpSqlite
{
    namespace Compiler
    {

        public class Lexer
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
            ///An tokenizer for SQL
            ///
            ///This file contains C code that splits an SQL input string up into
            ///</summary>
            ///<param name="individual tokens and sends those tokens one">one over to the</param>
            ///<param name="parser for analysis.">parser for analysis.</param>
            ///<param name=""></param>
            ///<param name="Included in SQLite3 port to C#">SQLite;  2008 Noah B Hart</param>
            ///<param name="C#">SQLite is an independent reimplementation of the SQLite software library</param>
            ///<param name=""></param>
            ///<param name="SQLITE_SOURCE_ID: 2011">23 19:49:22 4374b7e83ea0a3fbc3691f9c0c936272862f32f2</param>
            ///<param name=""></param>
            ///<param name=""></param>
            ///<param name=""></param>

            //#include "sqliteInt.h"
            //#include <stdlib.h>
            ///
            ///<summary>
            ///The charMap() macro maps alphabetic characters into their
            ///</summary>
            ///<param name="lower">case ASCII equivalent.  On ASCII machines, this is just</param>
            ///<param name="an upper">lower case map.  On EBCDIC machines we also need</param>
            ///<param name="to adjust the encoding.  Only alphabetic characters and underscores">to adjust the encoding.  Only alphabetic characters and underscores</param>
            ///<param name="need to be translated.">need to be translated.</param>
            ///<param name=""></param>

#if SQLITE_ASCII
            //# define charMap(X) _Custom.sqlite3UpperToLower[(unsigned char)X]
#endif
            //#if SQLITE_EBCDIC
            //# define charMap(X) ebcdicToAscii[(unsigned char)X]
            //const unsigned char ebcdicToAscii[] = {
            ///* 0   1   2   3   4   5   6   7   8   9   A   B   C   D   E   F */
            //   0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  /* 0x */
            //   0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  /* 1x */
            //   0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  /* 2x */
            //   0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  /* 3x */
            //   0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  /* 4x */
            //   0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  /* 5x */
            //   0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0, 95,  0,  0,  /* 6x */
            //   0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  /* 7x */
            //   0, 97, 98, 99,100,101,102,103,104,105,  0,  0,  0,  0,  0,  0,  /* 8x */
            //   0,106,107,108,109,110,111,112,113,114,  0,  0,  0,  0,  0,  0,  /* 9x */
            //   0,  0,115,116,117,118,119,120,121,122,  0,  0,  0,  0,  0,  0,  /* Ax */
            //   0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  /* Bx */
            //   0, 97, 98, 99,100,101,102,103,104,105,  0,  0,  0,  0,  0,  0,  /* Cx */
            //   0,106,107,108,109,110,111,112,113,114,  0,  0,  0,  0,  0,  0,  /* Dx */
            //   0,  0,115,116,117,118,119,120,121,122,  0,  0,  0,  0,  0,  0,  /* Ex */
            //   0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  0,  /* Fx */
            //};
            //#endif
            ///
            ///<summary>
            ///The sqlite3KeywordCode function looks up an identifier to determine if
            ///it is a keyword.  If it is a keyword, the token code of that keyword is
            ///returned.  If the input is not a keyword, TokenType.TK_ID is returned.
            ///
            ///The implementation of this routine was generated by a program,
            ///mkkeywordhash.h, located in the tool subdirectory of the distribution.
            ///The output of the mkkeywordhash.c program is written into a file
            ///named keywordhash.h and then included into this source file by
            ///the #include below.
            ///</summary>

            //#include "keywordhash.h"
            ///<summary>
            /// If X is a character that can be used in an identifier then
            /// IdChar(X) will be true.  Otherwise it is false.
            ///
            /// For ASCII, any character with the high-order bit set is
            /// allowed in an identifier.  For 7-bit characters,
            /// sqlite3IsIdChar[X] must be 1.
            ///
            /// For EBCDIC, the rules are more complex but have the same
            /// end result.
            ///
            /// Ticket #1066.  the SQL standard does not allow '$' in the
            /// middle of identfiers.  But many SQL implementations do.
            /// SQLite will allow '$' in identifiers for compatibility.
            /// But the feature is undocumented.
            ///
            ///</summary>
#if SQLITE_ASCII
            //#define IdChar(C)  ((sqlite3CtypeMap[(unsigned char)C]&0x46)!=0)
#endif
            //#if SQLITE_EBCDIC
            //const char sqlite3IsEbcdicIdChar[] = {
            ///* x0 x1 x2 x3 x4 x5 x6 x7 x8 x9 xA xB xC xD xE xF */
            //    0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0,  /* 4x */
            //    0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 1, 0, 0, 0, 0,  /* 5x */
            //    0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 1, 0, 0,  /* 6x */
            //    0, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0,  /* 7x */
            //    0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 1, 1, 1, 0,  /* 8x */
            //    0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 1, 0, 1, 0,  /* 9x */
            //    1, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 1, 1, 1, 0,  /* Ax */
            //    0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,  /* Bx */
            //    0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 1, 1, 1, 1, 1,  /* Cx */
            //    0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 1, 1, 1, 1, 1,  /* Dx */
            //    0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 0, 1, 1, 1, 1, 1,  /* Ex */
            //    1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 1, 1, 1, 1, 0,  /* Fx */
            //};
            //#define IdChar(C)  (((c=C)>=0x42 && sqlite3IsEbcdicIdChar[c-0x40]))
            //#endif
            ///<summary>
            /// Return the length of the token that begins at z[iOffset + 0].
            /// Store the token type in *tokenType before returning.
            ///</summary>
            public static int GetNextToken(string z, int iOffset, ref TokenType tokenType)
            {
                int i;
                byte c = 0;
                switch (z[iOffset + 0])
                {
                    case ' ':
                    case '\t':
                    case '\n':
                    case '\f':
                    case '\r':
                        {
                            sqliteinth.testcase(z[iOffset + 0] == ' ');
                            sqliteinth.testcase(z[iOffset + 0] == '\t');
                            sqliteinth.testcase(z[iOffset + 0] == '\n');
                            sqliteinth.testcase(z[iOffset + 0] == '\f');
                            sqliteinth.testcase(z[iOffset + 0] == '\r');
                            for (i = 1; z.Length > iOffset + i && CharExtensions.sqlite3Isspace(z[iOffset + i]); i++)
                            {
                            }
                            tokenType = TokenType.TK_SPACE;
                            return i;
                        }
                    case '-':
                        {
                            if (z.Length > iOffset + 1 && z[iOffset + 1] == '-')
                            {
                                ///<param name="IMP: R"> syntax diagram for comments </param>

                                for (i = 2; z.Length > iOffset + i && (c = (byte)z[iOffset + i]) != 0 && c != '\n'; i++)
                                {
                                }
                                tokenType = TokenType.TK_SPACE;
                                ///<param name="IMP: R">25134 </param>

                                return i;
                            }
                            tokenType = TokenType.TK_MINUS;
                            return 1;
                        }
                    case '(':
                        {
                            tokenType = TokenType.TK_LP;
                            return 1;
                        }
                    case ')':
                        {
                            tokenType = TokenType.TK_RP;
                            return 1;
                        }
                    case ';':
                        {
                            tokenType = TokenType.TK_SEMI;
                            return 1;
                        }
                    case '+':
                        {
                            tokenType = TokenType.TK_PLUS;
                            return 1;
                        }
                    case '*':
                        {
                            tokenType = TokenType.TK_STAR;
                            return 1;
                        }
                    case '/':
                        {
                            if (iOffset + 2 >= z.Length || z[iOffset + 1] != '*')
                            {
                                tokenType = TokenType.TK_SLASH;
                                return 1;
                            }
                            ///<param name="IMP: R"> syntax diagram for comments </param>

                            for (i = 3, c = (byte)z[iOffset + 2]; iOffset + i < z.Length && (c != '*' || (z[iOffset + i] != '/') && (c != 0)); i++)
                            {
                                c = (byte)z[iOffset + i];
                            }
                            if (iOffset + i == z.Length)
                                c = 0;
                            if (c != 0)
                                i++;
                            tokenType = TokenType.TK_SPACE;
                            ///<param name="IMP: R">25134 </param>

                            return i;
                        }
                    case '%':
                        {
                            tokenType = TokenType.TK_REM;
                            return 1;
                        }
                    case '=':
                        {
                            tokenType = TokenType.TK_EQ;
                            return 1 + (z[iOffset + 1] == '=' ? 1 : 0);
                        }
                    case '<':
                        {
                            if ((c = (byte)z[iOffset + 1]) == '=')
                            {
                                tokenType = TokenType.TK_LE;
                                return 2;
                            }
                            else
                                if (c == '>')
                            {
                                tokenType = TokenType.TK_NE;
                                return 2;
                            }
                            else
                                    if (c == '<')
                            {
                                tokenType = TokenType.TK_LSHIFT;
                                return 2;
                            }
                            else
                            {
                                tokenType = TokenType.TK_LT;
                                return 1;
                            }
                        }
                    case '>':
                        {
                            if (z.Length > iOffset + 1 && (c = (byte)z[iOffset + 1]) == '=')
                            {
                                tokenType = TokenType.TK_GE;
                                return 2;
                            }
                            else
                                if (c == '>')
                            {
                                tokenType = TokenType.TK_RSHIFT;
                                return 2;
                            }
                            else
                            {
                                tokenType = TokenType.TK_GT;
                                return 1;
                            }
                        }
                    case '!':
                        {
                            if (z[iOffset + 1] != '=')
                            {
                                tokenType = TokenType.TK_ILLEGAL;
                                return 2;
                            }
                            else
                            {
                                tokenType = TokenType.TK_NE;
                                return 2;
                            }
                        }
                    case '|':
                        {
                            if (z[iOffset + 1] != '|')
                            {
                                tokenType = TokenType.TK_BITOR;
                                return 1;
                            }
                            else
                            {
                                tokenType = TokenType.TK_CONCAT;
                                return 2;
                            }
                        }
                    case ',':
                        {
                            tokenType = TokenType.TK_COMMA;
                            return 1;
                        }
                    case '&':
                        {
                            tokenType = TokenType.TK_BITAND;
                            return 1;
                        }
                    case '~':
                        {
                            tokenType = TokenType.TK_BITNOT;
                            return 1;
                        }
                    case '`':
                    case '\'':
                    case '"':
                        {
                            int delim = z[iOffset + 0];
                            sqliteinth.testcase(delim == '`');
                            sqliteinth.testcase(delim == '\'');
                            sqliteinth.testcase(delim == '"');
                            for (i = 1; (iOffset + i) < z.Length && (c = (byte)z[iOffset + i]) != 0; i++)
                            {
                                if (c == delim)
                                {
                                    if (z.Length > iOffset + i + 1 && z[iOffset + i + 1] == delim)
                                    {
                                        i++;
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }
                            }
                            if ((iOffset + i == z.Length && c != delim) || z[iOffset + i] != delim)
                            {
                                tokenType = TokenType.TK_ILLEGAL;
                                return i + 1;
                            }
                            if (c == '\'')
                            {
                                tokenType = TokenType.TK_STRING;
                                return i + 1;
                            }
                            else
                                if (c != 0)
                            {
                                tokenType = TokenType.TK_ID;
                                return i + 1;
                            }
                            else
                            {
                                tokenType = TokenType.TK_ILLEGAL;
                                return i;
                            }
                        }
                    case '.':
                        {
#if !SQLITE_OMIT_FLOATING_POINT
                            if (!CharExtensions.sqlite3Isdigit(z[iOffset + 1]))
#endif
                            {
                                tokenType = TokenType.TK_DOT;
                                return 1;
                            }
                            ///
                            ///<summary>
                            ///If the next character is a digit, this is a floating point
                            ///number that begins with ".".  Fall thru into the next case 
                            ///</summary>

                            goto case '0';
                        }
                    case '0':
                    case '1':
                    case '2':
                    case '3':
                    case '4':
                    case '5':
                    case '6':
                    case '7':
                    case '8':
                    case '9':
                        {
                            sqliteinth.testcase(z[iOffset] == '0');
                            sqliteinth.testcase(z[iOffset] == '1');
                            sqliteinth.testcase(z[iOffset] == '2');
                            sqliteinth.testcase(z[iOffset] == '3');
                            sqliteinth.testcase(z[iOffset] == '4');
                            sqliteinth.testcase(z[iOffset] == '5');
                            sqliteinth.testcase(z[iOffset] == '6');
                            sqliteinth.testcase(z[iOffset] == '7');
                            sqliteinth.testcase(z[iOffset] == '8');
                            sqliteinth.testcase(z[iOffset] == '9');
                            tokenType = TokenType.TK_INTEGER;
                            for (i = 0; z.Length > iOffset + i && CharExtensions.sqlite3Isdigit(z[iOffset + i]); i++)
                            {
                            }
#if !SQLITE_OMIT_FLOATING_POINT
                            if (z.Length > iOffset + i && z[iOffset + i] == '.')
                            {
                                i++;
                                while (z.Length > iOffset + i && CharExtensions.sqlite3Isdigit(z[iOffset + i]))
                                {
                                    i++;
                                }
                                tokenType = TokenType.TK_FLOAT;
                            }
                            if (z.Length > iOffset + i + 1 && (z[iOffset + i] == 'e' || z[iOffset + i] == 'E') && (CharExtensions.sqlite3Isdigit(z[iOffset + i + 1]) || z.Length > iOffset + i + 2 && ((z[iOffset + i + 1] == '+' || z[iOffset + i + 1] == '-') && CharExtensions.sqlite3Isdigit(z[iOffset + i + 2]))))
                            {
                                i += 2;
                                while (z.Length > iOffset + i && CharExtensions.sqlite3Isdigit(z[iOffset + i]))
                                {
                                    i++;
                                }
                                tokenType = TokenType.TK_FLOAT;
                            }
#endif
                            while (iOffset + i < z.Length && Sqlite3.IdChar((byte)z[iOffset + i]))
                            {
                                tokenType = TokenType.TK_ILLEGAL;
                                i++;
                            }
                            return i;
                        }
                    case '[':
                        {
                            for (i = 1, c = (byte)z[iOffset + 0]; c != ']' && (iOffset + i) < z.Length && (c = (byte)z[iOffset + i]) != 0; i++)
                            {
                            }
                            tokenType = c == ']' ? TokenType.TK_ID : TokenType.TK_ILLEGAL;
                            return i;
                        }
                    case '?':
                        {
                            tokenType = TokenType.TK_VARIABLE;
                            for (i = 1; z.Length > iOffset + i && CharExtensions.sqlite3Isdigit(z[iOffset + i]); i++)
                            {
                            }
                            return i;
                        }
                    case '#':
                        {
                            for (i = 1; z.Length > iOffset + i && CharExtensions.sqlite3Isdigit(z[iOffset + i]); i++)
                            {
                            }
                            if (i > 1)
                            {
                                ///
                                ///<summary>
                                ///Parameters of the form #NNN (where NNN is a number) are used
                                ///internally by build.sqlite3NestedParse.  
                                ///</summary>

                                tokenType = TokenType.TK_REGISTER;
                                return i;
                            }
                            ///
                            ///<summary>
                            ///Fall through into the next case if the '#' is not followed by
                            ///a digit. Try to match #AAAA where AAAA is a parameter name. 
                            ///</summary>

                            goto case ':';
                        }
#if !SQLITE_OMIT_TCL_VARIABLE
                    case '$':
#endif
                    case '@':
                    ///
                    ///<summary>
                    ///For compatibility with MS SQL Server 
                    ///</summary>

                    case ':':
                        {
                            int n = 0;
                            sqliteinth.testcase(z[iOffset + 0] == '$');
                            sqliteinth.testcase(z[iOffset + 0] == '@');
                            sqliteinth.testcase(z[iOffset + 0] == ':');
                            tokenType = TokenType.TK_VARIABLE;
                            for (i = 1; z.Length > iOffset + i && (c = (byte)z[iOffset + i]) != 0; i++)
                            {
                                if (Sqlite3.IdChar(c))
                                {
                                    n++;
#if !SQLITE_OMIT_TCL_VARIABLE
                                }
                                else
                                    if (c == '(' && n > 0)
                                {
                                    do
                                    {
                                        i++;
                                    }
                                    while ((iOffset + i) < z.Length && (c = (byte)z[iOffset + i]) != 0 && !CharExtensions.sqlite3Isspace(c) && c != ')');
                                    if (c == ')')
                                    {
                                        i++;
                                    }
                                    else
                                    {
                                        tokenType = TokenType.TK_ILLEGAL;
                                    }
                                    break;
                                }
                                else
                                        if (c == ':' && z[iOffset + i + 1] == ':')
                                {
                                    i++;
#endif
                                }
                                else
                                {
                                    break;
                                }
                            }
                            if (n == 0)
                                tokenType = TokenType.TK_ILLEGAL;
                            return i;
                        }
#if !SQLITE_OMIT_BLOB_LITERAL
                    case 'x':
                    case 'X':
                        {
                            sqliteinth.testcase(z[iOffset + 0] == 'x');
                            sqliteinth.testcase(z[iOffset + 0] == 'X');
                            if (z.Length > iOffset + 1 && z[iOffset + 1] == '\'')
                            {
                                tokenType = TokenType.TK_BLOB;
                                for (i = 2; z.Length > iOffset + i && CharExtensions.sqlite3Isxdigit(z[iOffset + i]); i++)
                                {
                                }
                                if (iOffset + i == z.Length || z[iOffset + i] != '\'' || i % 2 != 0)
                                {
                                    tokenType = TokenType.TK_ILLEGAL;
                                    while (z.Length > iOffset + i && z[iOffset + i] != '\'')
                                    {
                                        i++;
                                    }
                                }
                                if (z.Length > iOffset + i)
                                    i++;
                                return i;
                            }
                            goto default;
                            ///
                            ///<summary>
                            ///Otherwise fall through to the next case 
                            ///</summary>

                        }
#endif
                    default:
                        {
                            if (!Sqlite3.IdChar((byte)z[iOffset]))
                            {
                                break;
                            }
                            for (i = 1; i < z.Length - iOffset && Sqlite3.IdChar((byte)z[iOffset + i]); i++)
                            {
                            }
                            tokenType = Sqlite3.keywordCode(z, iOffset, i);
                            return i;
                        }
                }
                tokenType = TokenType.TK_ILLEGAL;
                return 1;
            }

            public static Token GetNextToken(string z, int iOffset)
            {
                TokenType tokenType = 0;
                int length = GetNextToken(z, iOffset, ref tokenType);
                var token = new Token()
                {
                    Start = iOffset,
                    TokenType = tokenType,
                    Text = z.Substring(iOffset, length),
                    zRestSql = z.Substring(iOffset),
                    Length = length
                };
                return token;
            }
            ///
            ///<summary>
            ///Run the parser on the given SQL string.  The parser structure is
            ///passed in.  An SQLITE_ status code is returned.  If an error occurs
            ///then an and attempt is made to write an error message into
            ///memory obtained from sqlite3_malloc() and to make pzErrMsg point to that
            ///error message.
            ///
            ///</summary>
            ///



        }

    }
}
