using System;
using System.Diagnostics;
using System.Text;
using sqlite3_int64=System.Int64;
using i64=System.Int64;
using u8=System.Byte;
using u32=System.UInt32;
using u64=System.UInt64;
namespace Community.CsharpSqlite {
	using sqlite3_value=Sqlite3.Mem;
	using sqlite_int64=System.Int64;
    public partial class Sqlite3
    {
        public class func
        {
            ///<summary>
            /// 2002 February 23
            ///
            /// The author disclaims copyright to this source code.  In place of
            /// a legal notice, here is a blessing:
            ///
            ///    May you do good and not evil.
            ///    May you find forgiveness for yourself and forgive others.
            ///    May you share freely, never taking more than you give.
            ///
            ///
            /// This file contains the C functions that implement various SQL
            /// functions of SQLite.
            ///
            /// There is only one exported symbol in this file - the function
            /// sqliteRegisterBuildinFunctions() found at the bottom of the file.
            /// All other code has file scope.
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
            //#include <stdlib.h>
            //#include <assert.h>
            //#include "vdbeInt.h"
            ///<summary>
            /// Return the collating function associated with a function.
            ///
            ///</summary>
            static CollSeq sqlite3GetFuncCollSeq(sqlite3_context context)
            {
                return context.pColl;
            }
            ///<summary>
            /// Implementation of the non-aggregate min() and max() functions
            ///
            ///</summary>
            static void minmaxFunc(sqlite3_context context, int argc, sqlite3_value[] argv)
            {
                int i;
                int mask;
                ///
                ///<summary>
                ///0 for min() or 0xffffffff for max() 
                ///</summary>
                int iBest;
                CollSeq pColl;
                Debug.Assert(argc > 1);
                mask = (int)vdbeapi.sqlite3_user_data(context) == 0 ? 0 : -1;
                pColl = sqlite3GetFuncCollSeq(context);
                Debug.Assert(pColl != null);
                Debug.Assert(mask == -1 || mask == 0);
                sqliteinth.testcase(mask == 0);
                iBest = 0;
                if (vdbeapi.sqlite3_value_type(argv[0]) == SQLITE_NULL)
                    return;
                for (i = 1; i < argc; i++)
                {
                    if (vdbeapi.sqlite3_value_type(argv[i]) == SQLITE_NULL)
                        return;
                    if ((sqlite3MemCompare(argv[iBest], argv[i], pColl) ^ mask) >= 0)
                    {
                        iBest = i;
                    }
                }
                context.sqlite3_result_value(argv[iBest]);
            }
            ///<summary>
            /// Return the type of the argument.
            ///
            ///</summary>
            static void typeofFunc(sqlite3_context context, int NotUsed, sqlite3_value[] argv)
            {
                string z = "";
                Sqlite3.sqliteinth.UNUSED_PARAMETER(NotUsed);
                switch (vdbeapi.sqlite3_value_type(argv[0]))
                {
                    case SQLITE_INTEGER:
                        z = "integer";
                        break;
                    case SQLITE_TEXT:
                        z = "text";
                        break;
                    case SQLITE_FLOAT:
                        z = "real";
                        break;
                    case SQLITE_BLOB:
                        z = "blob";
                        break;
                    default:
                        z = "null";
                        break;
                }
                context.sqlite3_result_text(z, -1, SQLITE_STATIC);
            }
            ///<summary>
            /// Implementation of the length() function
            ///
            ///</summary>
            static void lengthFunc(sqlite3_context context, int argc, sqlite3_value[] argv)
            {
                int len;
                Debug.Assert(argc == 1);
                Sqlite3.sqliteinth.UNUSED_PARAMETER(argc);
                switch (vdbeapi.sqlite3_value_type(argv[0]))
                {
                    case SQLITE_BLOB:
                    case SQLITE_INTEGER:
                    case SQLITE_FLOAT:
                        {
                            context.sqlite3_result_int(vdbeapi.sqlite3_value_bytes(argv[0]));
                            break;
                        }
                    case SQLITE_TEXT:
                        {
                            byte[] z = vdbeapi.sqlite3_value_blob(argv[0]);
                            if (z == null)
                                return;
                            len = 0;
                            int iz = 0;
                            while (iz < z.Length && z[iz] != '\0')
                            {
                                len++;
                                sqliteinth.SQLITE_SKIP_UTF8(z, ref iz);
                            }
                            context.sqlite3_result_int(len);
                            break;
                        }
                    default:
                        {
                            context.sqlite3_result_null();
                            break;
                        }
                }
            }
            ///<summary>
            /// Implementation of the abs() function.
            ///
            /// IMP: R-23979-26855 The abs(X) function returns the absolute value of
            /// the numeric argument X.
            ///
            ///</summary>
            static void absFunc(sqlite3_context context, int argc, sqlite3_value[] argv)
            {
                Debug.Assert(argc == 1);
                Sqlite3.sqliteinth.UNUSED_PARAMETER(argc);
                switch (vdbeapi.sqlite3_value_type(argv[0]))
                {
                    case SQLITE_INTEGER:
                        {
                            i64 iVal = vdbeapi.sqlite3_value_int64(argv[0]);
                            if (iVal < 0)
                            {
                                if ((iVal << 1) == 0)
                                {
                                    ///
                                    ///<summary>
                                    ///</summary>
                                    ///<param name="IMP: R">9223372036854775807 then</param>
                                    ///<param name="abs(X) throws an integer overflow error since there is no">abs(X) throws an integer overflow error since there is no</param>
                                    ///<param name="equivalent positive 64">bit two complement value. </param>
                                    context.sqlite3_result_error("integer overflow", -1);
                                    return;
                                }
                                iVal = -iVal;
                            }
                            context.sqlite3_result_int64(iVal);
                            break;
                        }
                    case SQLITE_NULL:
                        {
                            ///
                            ///<summary>
                            ///</summary>
                            ///<param name="IMP: R">19929 Abs(X) returns NULL if X is NULL. </param>
                            context.sqlite3_result_null();
                            break;
                        }
                    default:
                        {
                            ///
                            ///<summary>
                            ///Because sqlite3_value_double() returns 0.0 if the argument is not
                            ///something that can be converted into a number, we have:
                            ///</summary>
                            ///<param name="IMP: R">31541 Abs(X) return 0.0 if X is a string or blob that</param>
                            ///<param name="cannot be converted to a numeric value. ">cannot be converted to a numeric value. </param>
                            ///<param name=""></param>
                            double rVal = vdbeapi.sqlite3_value_double(argv[0]);
                            if (rVal < 0)
                                rVal = -rVal;
                            context.sqlite3_result_double(rVal);
                            break;
                        }
                }
            }
            ///
            ///<summary>
            ///Implementation of the substr() function.
            ///
            ///substr(x,p1,p2)  returns p2 characters of x[] beginning with p1.
            ///</summary>
            ///<param name="p1 is 1">indexed.  So substr(x,1,1) returns the first character</param>
            ///<param name="of x.  If x is text, then we actually count UTF">8 characters.</param>
            ///<param name="If x is a blob, then we count bytes.">If x is a blob, then we count bytes.</param>
            ///<param name=""></param>
            ///<param name="If p1 is negative, then we begin abs(p1) from the end of x[].">If p1 is negative, then we begin abs(p1) from the end of x[].</param>
            ///<param name=""></param>
            ///<param name="If p2 is negative, return the p2 characters preceeding p1.">If p2 is negative, return the p2 characters preceeding p1.</param>
            ///<param name=""></param>
            static void substrFunc(sqlite3_context context, int argc, sqlite3_value[] argv)
            {
                string z = "";
                byte[] zBLOB = null;
                string z2;
                int len;
                int p0type;
                int p1, p2;
                int negP2 = 0;
                Debug.Assert(argc == 3 || argc == 2);
                if (vdbeapi.sqlite3_value_type(argv[1]) == SQLITE_NULL || (argc == 3 && vdbeapi.sqlite3_value_type(argv[2]) == SQLITE_NULL))
                {
                    return;
                }
                p0type = vdbeapi.sqlite3_value_type(argv[0]);
                p1 = vdbeapi.sqlite3_value_int(argv[1]);
                if (p0type == SQLITE_BLOB)
                {
                    len = vdbeapi.sqlite3_value_bytes(argv[0]);
                    zBLOB = argv[0].zBLOB;
                    if (zBLOB == null)
                        return;
                    Debug.Assert(len == zBLOB.Length);
                }
                else
                {
                    z = vdbeapi.sqlite3_value_text(argv[0]);
                    if (String.IsNullOrEmpty(z))
                        return;
                    len = 0;
                    if (p1 < 0)
                    {
                        len = z.Length;
                        //for ( z2 = z ; z2 != "" ; len++ )
                        //{
                        //  sqliteinth.SQLITE_SKIP_UTF8( ref z2 );
                        //}
                    }
                }
                if (argc == 3)
                {
                    p2 = vdbeapi.sqlite3_value_int(argv[2]);
                    if (p2 < 0)
                    {
                        p2 = -p2;
                        negP2 = 1;
                    }
                }
                else
                {
                    p2 = (vdbeapi.sqlite3_context_db_handle(context)).aLimit[SQLITE_LIMIT_LENGTH];
                }
                if (p1 < 0)
                {
                    p1 += len;
                    if (p1 < 0)
                    {
                        p2 += p1;
                        if (p2 < 0)
                            p2 = 0;
                        p1 = 0;
                    }
                }
                else
                    if (p1 > 0)
                    {
                        p1--;
                    }
                    else
                        if (p2 > 0)
                        {
                            p2--;
                        }
                if (negP2 != 0)
                {
                    p1 -= p2;
                    if (p1 < 0)
                    {
                        p2 += p1;
                        p1 = 0;
                    }
                }
                Debug.Assert(p1 >= 0 && p2 >= 0);
                if (p0type != SQLITE_BLOB)
                {
                    //while ( z != "" && p1 != 0 )
                    //{
                    //  sqliteinth.SQLITE_SKIP_UTF8( ref z );
                    //  p1--;
                    //}
                    //for ( z2 = z ; z2 != "" && p2 != 0 ; p2-- )
                    //{
                    //  sqliteinth.SQLITE_SKIP_UTF8( ref z2 );
                    //}
                    context.sqlite3_result_text(z, p1, p2 <= z.Length - p1 ? p2 : z.Length - p1, SQLITE_TRANSIENT);
                }
                else
                {
                    if (p1 + p2 > len)
                    {
                        p2 = len - p1;
                        if (p2 < 0)
                            p2 = 0;
                    }
                    StringBuilder sb = new StringBuilder(zBLOB.Length);
                    if (zBLOB.Length == 0 || p1 > zBLOB.Length)
                        sb.Length = 0;
                    else
                    {
                        for (int i = p1; i < p1 + p2; i++)
                        {
                            sb.Append((char)zBLOB[i]);
                        }
                    }
                    context.sqlite3_result_blob(sb.ToString(), (int)p2, SQLITE_TRANSIENT);
                }
            }
            ///<summary>
            /// Implementation of the round() function
            ///
            ///</summary>
#if !SQLITE_OMIT_FLOATING_POINT
            static void roundFunc(sqlite3_context context, int argc, sqlite3_value[] argv)
            {
                int n = 0;
                double r;
                string zBuf = "";
                Debug.Assert(argc == 1 || argc == 2);
                if (argc == 2)
                {
                    if (SQLITE_NULL == vdbeapi.sqlite3_value_type(argv[1]))
                        return;
                    n = vdbeapi.sqlite3_value_int(argv[1]);
                    if (n > 30)
                        n = 30;
                    if (n < 0)
                        n = 0;
                }
                if (vdbeapi.sqlite3_value_type(argv[0]) == SQLITE_NULL)
                    return;
                r = vdbeapi.sqlite3_value_double(argv[0]);
                ///
                ///<summary>
                ///</summary>
                ///<param name="If Y==0 and X will fit in a 64">bit int,</param>
                ///<param name="handle the rounding directly,">handle the rounding directly,</param>
                ///<param name="otherwise use printf.">otherwise use printf.</param>
                ///<param name=""></param>
                if (n == 0 && r >= 0 && r < IntegerExtensions.LARGEST_INT64 - 1)
                {
                    r = (double)((sqlite_int64)(r + 0.5));
                }
                else
                    if (n == 0 && r < 0 && (-r) < IntegerExtensions.LARGEST_INT64 - 1)
                    {
                        r = -(double)((sqlite_int64)((-r) + 0.5));
                    }
                    else
                    {
                        zBuf = io.sqlite3_mprintf("%.*f", n, r);
                        if (zBuf == null)
                        {
                            context.sqlite3_result_error_nomem();
                            return;
                        }
                        Converter.sqlite3AtoF(zBuf, ref r, StringExtensions.sqlite3Strlen30(zBuf), SqliteEncoding.UTF8);
                        //malloc_cs.sqlite3_free( ref zBuf );
                    }
                context.sqlite3_result_double(r);
            }
#endif
            ///<summary>
            /// Allocate nByte bytes of space using sqlite3_malloc(). If the
            /// allocation fails, call sqlite3_result_error_nomem() to notify
            /// the database handle that malloc() has failed and return NULL.
            /// If nByte is larger than the maximum string or blob length, then
            /// raise an SQLITE_TOOBIG exception and return NULL.
            ///</summary>
            //static void* contextMalloc( sqlite3_context* context, i64 nByte )
            //{
            //  char* z;
            //  sqlite3* db = vdbeapi.sqlite3_context_db_handle( context );
            //  assert( nByte > 0 );
            //  sqliteinth.testcase( nByte == db->aLimit[SQLITE_LIMIT_LENGTH] );
            //  sqliteinth.testcase( nByte == db->aLimit[SQLITE_LIMIT_LENGTH] + 1 );
            //  if ( nByte > db->aLimit[SQLITE_LIMIT_LENGTH] )
            //  {
            //    sqlite3_result_error_toobig( context );
            //    z = 0;
            //  }
            //  else
            //  {
            //    z = malloc_cs.sqlite3Malloc( (int)nByte );
            //    if ( !z )
            //    {
            //      sqlite3_result_error_nomem( context );
            //    }
            //  }
            //  return z;
            //}
            ///<summary>
            /// Implementation of the upper() and lower() SQL functions.
            ///
            ///</summary>
            static void upperFunc(sqlite3_context context, int argc, sqlite3_value[] argv)
            {
                string z1;
                string z2;
                int i, n;
                Sqlite3.sqliteinth.UNUSED_PARAMETER(argc);
                z2 = vdbeapi.sqlite3_value_text(argv[0]);
                n = vdbeapi.sqlite3_value_bytes(argv[0]);
                ///
                ///<summary>
                ///Verify that the call to _bytes() does not invalidate the _text() pointer 
                ///</summary>
                //Debug.Assert( z2 == vdbeapi.sqlite3_value_text( argv[0] ) );
                if (z2 != null)
                {
                    //z1 = new byte[n];// contextMalloc(context, ((i64)n)+1);
                    //if ( z1 !=null)
                    //{
                    //  memcpy( z1, z2, n + 1 );
                    //for ( i = 0 ; i< z1.Length ; i++ )
                    //{
                    //(char)sqlite3Toupper( z1[i] );
                    //}
                    context.sqlite3_result_text(z2.Length == 0 ? "" : z2.Substring(0, n).ToUpper(), -1, null);
                    //malloc_cs.sqlite3_free );
                    // }
                }
            }
            static void lowerFunc(sqlite3_context context, int argc, sqlite3_value[] argv)
            {
                string z1;
                string z2;
                int i, n;
                Sqlite3.sqliteinth.UNUSED_PARAMETER(argc);
                z2 = vdbeapi.sqlite3_value_text(argv[0]);
                n = vdbeapi.sqlite3_value_bytes(argv[0]);
                ///
                ///<summary>
                ///Verify that the call to _bytes() does not invalidate the _text() pointer 
                ///</summary>
                //Debug.Assert( z2 == vdbeapi.sqlite3_value_text( argv[0] ) );
                if (z2 != null)
                {
                    //z1 = contextMalloc(context, ((i64)n)+1);
                    //if ( z1 )
                    //{
                    //  memcpy( z1, z2, n + 1 );
                    //  for ( i = 0 ; z1[i] ; i++ )
                    //  {
                    //    z1[i] = (char)sqlite3Tolower( z1[i] );
                    //  }
                    context.sqlite3_result_text(z2.Length == 0 ? "" : z2.Substring(0, n).ToLower(), -1, null);
                    //malloc_cs.sqlite3_free );
                    //}
                }
            }
#if FALSE
																																										/*
** The COALESCE() and IFNULL() functions used to be implemented as shown
** here.  But now they are implemented as VDBE code so that unused arguments
** do not have to be computed.  This legacy implementation is retained as
** comment.
*/
/*
** Implementation of the IFNULL(), NVL(), and COALESCE() functions.
** All three do the same thing.  They return the first non-NULL
** argument.
*/
static void ifnullFunc(
sqlite3_context context,
int argc,
sqlite3_value[] argv
)
{
int i;
for ( i = 0 ; i < argc ; i++ )
{
if ( SQLITE_NULL != vdbeapi.sqlite3_value_type( argv[i] ) )
{
sqlite3_result_value( context, argv[i] );
break;
}
}
}
#endif
            //#define ifnullFunc versionFunc   /* Substitute function - never called */
            ///<summary>
            /// Implementation of random().  Return a random integer.
            ///
            ///</summary>
            static void randomFunc(sqlite3_context context, int NotUsed, sqlite3_value[] NotUsed2)
            {
                sqlite_int64 r = 0;
                Sqlite3.sqliteinth.UNUSED_PARAMETER2(NotUsed, NotUsed2);
                sqlite3_randomness(sizeof(sqlite_int64), ref r);
                if (r < 0)
                {
                    ///
                    ///<summary>
                    ///We need to prevent a random number of 0x8000000000000000
                    ///</summary>
                    ///<param name="(or ">9223372036854775808) since when you do abs() of that</param>
                    ///<param name="number of you get the same value back again.  To do this">number of you get the same value back again.  To do this</param>
                    ///<param name="in a way that is testable, mask the sign bit off of negative">in a way that is testable, mask the sign bit off of negative</param>
                    ///<param name="values, resulting in a positive value.  Then take the">values, resulting in a positive value.  Then take the</param>
                    ///<param name="2s complement of that positive value.  The end result can">2s complement of that positive value.  The end result can</param>
                    ///<param name="therefore be no less than ">9223372036854775807.</param>
                    ///<param name=""></param>
                    r = -(r ^ (((sqlite3_int64)1) << 63));
                }
                context.sqlite3_result_int64(r);
            }
            ///<summary>
            /// Implementation of randomblob(N).  Return a random blob
            /// that is N bytes long.
            ///
            ///</summary>
            static void randomBlob(sqlite3_context context, int argc, sqlite3_value[] argv)
            {
                int n;
                char[] p;
                Debug.Assert(argc == 1);
                Sqlite3.sqliteinth.UNUSED_PARAMETER(argc);
                n = vdbeapi.sqlite3_value_int(argv[0]);
                if (n < 1)
                {
                    n = 1;
                }
                if (n > vdbeapi.sqlite3_context_db_handle(context).aLimit[SQLITE_LIMIT_LENGTH])
                {
                    context.sqlite3_result_error_toobig();
                    p = null;
                }
                else
                {
                    p = new char[n];
                    //contextMalloc( context, n );
                }
                if (p != null)
                {
                    i64 _p = 0;
                    for (int i = 0; i < n; i++)
                    {
                        sqlite3_randomness(sizeof(u8), ref _p);
                        p[i] = (char)(_p & 0x7F);
                    }
                    context.sqlite3_result_blob(new string(p), n, null);
                    //malloc_cs.sqlite3_free );
                }
            }
            ///<summary>
            /// Implementation of the last_insert_rowid() SQL function.  The return
            /// value is the same as the sqlite3_last_insert_rowid() API function.
            ///
            ///</summary>
            static void last_insert_rowid(sqlite3_context context, int NotUsed, sqlite3_value[] NotUsed2)
            {
                sqlite3 db = vdbeapi.sqlite3_context_db_handle(context);
                Sqlite3.sqliteinth.UNUSED_PARAMETER2(NotUsed, NotUsed2);
                ///
                ///<summary>
                ///</summary>
                ///<param name="IMP: R">12026 The last_insert_rowid() SQL function is a</param>
                ///<param name="wrapper around the sqlite3_last_insert_rowid() C/C++ interface">wrapper around the sqlite3_last_insert_rowid() C/C++ interface</param>
                ///<param name="function. ">function. </param>
                context.sqlite3_result_int64(sqlite3_last_insert_rowid(db));
            }
            ///<summary>
            /// Implementation of the changes() SQL function.
            ///
            /// IMP: R-62073-11209 The changes() SQL function is a wrapper
            /// around the sqlite3_changes() C/C++ function and hence follows the same
            /// rules for counting changes.
            ///
            ///</summary>
            static void changes(sqlite3_context context, int NotUsed, sqlite3_value[] NotUsed2)
            {
                sqlite3 db = vdbeapi.sqlite3_context_db_handle(context);
                Sqlite3.sqliteinth.UNUSED_PARAMETER2(NotUsed, NotUsed2);
                context.sqlite3_result_int(sqlite3_changes(db));
            }
            ///<summary>
            /// Implementation of the total_changes() SQL function.  The return value is
            /// the same as the sqlite3_total_changes() API function.
            ///
            ///</summary>
            static void total_changes(sqlite3_context context, int NotUsed, sqlite3_value[] NotUsed2)
            {
                sqlite3 db = (sqlite3)vdbeapi.sqlite3_context_db_handle(context);
                Sqlite3.sqliteinth.UNUSED_PARAMETER2(NotUsed, NotUsed2);
                ///
                ///<summary>
                ///</summary>
                ///<param name="IMP: R">41993 This function is a wrapper around the</param>
                ///<param name="sqlite3_total_changes() C/C++ interface. ">sqlite3_total_changes() C/C++ interface. </param>
                context.sqlite3_result_int(sqlite3_total_changes(db));
            }
            ///
            ///<summary>
            ///</summary>
            ///<param name="A structure defining how to do GLOB">style comparisons.</param>
            ///<param name=""></param>
            struct compareInfo
            {
                public char matchAll;
                public char matchOne;
                public char matchSet;
                public bool noCase;
                public compareInfo(char matchAll, char matchOne, char matchSet, bool noCase)
                {
                    this.matchAll = matchAll;
                    this.matchOne = matchOne;
                    this.matchSet = matchSet;
                    this.noCase = noCase;
                }
            }
            ///
            ///<summary>
            ///For LIKE and GLOB matching on EBCDIC machines, assume that every
            ///character is exactly one byte in size.  Also, all characters are
            ///</summary>
            ///<param name="able to participate in upper">case mappings in EBCDIC</param>
            ///<param name="whereas only characters less than 0x80 do in ASCII.">whereas only characters less than 0x80 do in ASCII.</param>
            ///<param name=""></param>
            //#if defined(SQLITE_EBCDIC)
            //# define sqlite3Utf8Read(A,C)  (*(A++))
            //# define GlogUpperToLower(A)   A = _Custom.sqlite3UpperToLower[A]
            //#else
            //# define GlogUpperToLower(A)   if( !((A)&~0x7f) ){ A = _Custom.sqlite3UpperToLower[A]; }
            //#endif
            static compareInfo globInfo = new compareInfo('*', '?', '[', false);
            ///
            ///<summary>
            ///</summary>
            ///<param name="The correct SQL">92 behavior is for the LIKE operator to ignore</param>
            ///<param name="case.  Thus  'a' LIKE 'A' would be true. ">case.  Thus  'a' LIKE 'A' would be true. </param>
            static compareInfo likeInfoNorm = new compareInfo('%', '_', '\0', true);
            ///<summary>
            ///If SQLITE_CASE_SENSITIVE_LIKE is defined, then the LIKE operator
            /// is case sensitive causing 'a' LIKE 'A' to be false
            ///</summary>
            static compareInfo likeInfoAlt = new compareInfo('%', '_', '\0', false);
            ///
            ///<summary>
            ///</summary>
            ///<param name="Compare two UTF">8 strings for equality where the first string can</param>
            ///<param name="potentially be a "glob" expression.  Return true (1) if they">potentially be a "glob" expression.  Return true (1) if they</param>
            ///<param name="are the same and false (0) if they are different.">are the same and false (0) if they are different.</param>
            ///<param name=""></param>
            ///<param name="Globbing rules:">Globbing rules:</param>
            ///<param name=""></param>
            ///<param name="'*'       Matches any sequence of zero or more characters.">'*'       Matches any sequence of zero or more characters.</param>
            ///<param name=""></param>
            ///<param name="'?'       Matches exactly one character.">'?'       Matches exactly one character.</param>
            ///<param name=""></param>
            ///<param name="[...]      Matches one character from the enclosed list of">[...]      Matches one character from the enclosed list of</param>
            ///<param name="characters.">characters.</param>
            ///<param name=""></param>
            ///<param name="[^...]     Matches one character not in the enclosed list.">[^...]     Matches one character not in the enclosed list.</param>
            ///<param name=""></param>
            ///<param name="With the [...] and [^...] matching, a ']' character can be included">With the [...] and [^...] matching, a ']' character can be included</param>
            ///<param name="in the list by making it the first character after '[' or '^'.  A">in the list by making it the first character after '[' or '^'.  A</param>
            ///<param name="range of characters can be specified using '">'.  Example:</param>
            ///<param name=""[a">', make</param>
            ///<param name="it the last character in the list.">it the last character in the list.</param>
            ///<param name=""></param>
            ///<param name="This routine is usually quick, but can be N**2 in the worst case.">This routine is usually quick, but can be N**2 in the worst case.</param>
            ///<param name=""></param>
            ///<param name="Hints: to match '*' or '?', put them in "[]".  Like this:">Hints: to match '*' or '?', put them in "[]".  Like this:</param>
            ///<param name=""></param>
            ///<param name="abc[*]xyz        Matches "abc*xyz" only">abc[*]xyz        Matches "abc*xyz" only</param>
            ///<param name=""></param>
            static bool patternCompare(string zPattern,///
                ///<summary>
                ///The glob pattern 
                ///</summary>
            string zString,///
                ///<summary>
                ///The string to compare against the glob 
                ///</summary>
            compareInfo pInfo,///
                ///<summary>
                ///Information about how to do the compare 
                ///</summary>
            u32 esc///
                ///<summary>
                ///The escape character 
                ///</summary>
            )
            {
                u32 c, c2;
                int invert;
                int seen;
                int matchOne = (int)pInfo.matchOne;
                int matchAll = (int)pInfo.matchAll;
                int matchSet = (int)pInfo.matchSet;
                bool noCase = pInfo.noCase;
                bool prevEscape = false;
                ///
                ///<summary>
                ///True if the previous character was 'escape' 
                ///</summary>
                string inPattern = zPattern;
                //Entered Pattern
                while ((c = sqlite3Utf8Read(zPattern, ref zPattern)) != 0)
                {
                    if (!prevEscape && c == matchAll)
                    {
                        while ((c = sqlite3Utf8Read(zPattern, ref zPattern)) == matchAll || c == matchOne)
                        {
                            if (c == matchOne && sqlite3Utf8Read(zString, ref zString) == 0)
                            {
                                return false;
                            }
                        }
                        if (c == 0)
                        {
                            return true;
                        }
                        else
                            if (c == esc)
                            {
                                c = sqlite3Utf8Read(zPattern, ref zPattern);
                                if (c == 0)
                                {
                                    return false;
                                }
                            }
                            else
                                if (c == matchSet)
                                {
                                    Debug.Assert(esc == 0);
                                    ///
                                    ///<summary>
                                    ///This is GLOB, not LIKE 
                                    ///</summary>
                                    Debug.Assert(matchSet < 0x80);
                                    ///
                                    ///<summary>
                                    ///</summary>
                                    ///<param name="'[' is a single">byte character </param>
                                    int len = 0;
                                    while (len < zString.Length && patternCompare(inPattern.Substring(inPattern.Length - zPattern.Length - 1), zString.Substring(len), pInfo, esc) == false)
                                    {
                                        sqliteinth.SQLITE_SKIP_UTF8(zString, ref len);
                                    }
                                    return len < zString.Length;
                                }
                        while ((c2 = sqlite3Utf8Read(zString, ref zString)) != 0)
                        {
                            if (noCase)
                            {
                                if (0 == ((c2) & ~0x7f))
                                    c2 = (u32)_Custom.sqlite3UpperToLower[c2];
                                //GlogUpperToLower(c2);
                                if (0 == ((c) & ~0x7f))
                                    c = (u32)_Custom.sqlite3UpperToLower[c];
                                //GlogUpperToLower(c);
                                while (c2 != 0 && c2 != c)
                                {
                                    c2 = sqlite3Utf8Read(zString, ref zString);
                                    if (0 == ((c2) & ~0x7f))
                                        c2 = (u32)_Custom.sqlite3UpperToLower[c2];
                                    //GlogUpperToLower(c2);
                                }
                            }
                            else
                            {
                                while (c2 != 0 && c2 != c)
                                {
                                    c2 = sqlite3Utf8Read(zString, ref zString);
                                }
                            }
                            if (c2 == 0)
                                return false;
                            if (patternCompare(zPattern, zString, pInfo, esc))
                                return true;
                        }
                        return false;
                    }
                    else
                        if (!prevEscape && c == matchOne)
                        {
                            if (sqlite3Utf8Read(zString, ref zString) == 0)
                            {
                                return false;
                            }
                        }
                        else
                            if (c == matchSet)
                            {
                                u32 prior_c = 0;
                                Debug.Assert(esc == 0);
                                ///
                                ///<summary>
                                ///This only occurs for GLOB, not LIKE 
                                ///</summary>
                                seen = 0;
                                invert = 0;
                                c = sqlite3Utf8Read(zString, ref zString);
                                if (c == 0)
                                    return false;
                                c2 = sqlite3Utf8Read(zPattern, ref zPattern);
                                if (c2 == '^')
                                {
                                    invert = 1;
                                    c2 = sqlite3Utf8Read(zPattern, ref zPattern);
                                }
                                if (c2 == ']')
                                {
                                    if (c == ']')
                                        seen = 1;
                                    c2 = sqlite3Utf8Read(zPattern, ref zPattern);
                                }
                                while (c2 != 0 && c2 != ']')
                                {
                                    if (c2 == '-' && zPattern[0] != ']' && zPattern[0] != 0 && prior_c > 0)
                                    {
                                        c2 = sqlite3Utf8Read(zPattern, ref zPattern);
                                        if (c >= prior_c && c <= c2)
                                            seen = 1;
                                        prior_c = 0;
                                    }
                                    else
                                    {
                                        if (c == c2)
                                        {
                                            seen = 1;
                                        }
                                        prior_c = c2;
                                    }
                                    c2 = sqlite3Utf8Read(zPattern, ref zPattern);
                                }
                                if (c2 == 0 || (seen ^ invert) == 0)
                                {
                                    return false;
                                }
                            }
                            else
                                if (esc == c && !prevEscape)
                                {
                                    prevEscape = true;
                                }
                                else
                                {
                                    c2 = sqlite3Utf8Read(zString, ref zString);
                                    if (noCase)
                                    {
                                        if (c < 0x80)
                                            c = (u32)_Custom.sqlite3UpperToLower[c];
                                        //GlogUpperToLower(c);
                                        if (c2 < 0x80)
                                            c2 = (u32)_Custom.sqlite3UpperToLower[c2];
                                        //GlogUpperToLower(c2);
                                    }
                                    if (c != c2)
                                    {
                                        return false;
                                    }
                                    prevEscape = false;
                                }
                }
                return zString.Length == 0;
            }
            ///<summary>
            /// Count the number of times that the LIKE operator (or GLOB which is
            /// just a variation of LIKE) gets called.  This is used for testing
            /// only.
            ///
            ///</summary>
#if SQLITE_TEST
#if !TCLSH
																																										    static int sqlite3_like_count = 0;
#else
																																										    static tcl.lang.Var.SQLITE3_GETSET sqlite3_like_count = new tcl.lang.Var.SQLITE3_GETSET( "sqlite3_like_count" );
#endif
#endif
            ///<summary>
            /// Implementation of the like() SQL function.  This function implements
            /// the build-in LIKE operator.  The first argument to the function is the
            /// pattern and the second argument is the string.  So, the SQL statements:
            ///
            ///       A LIKE B
            ///
            /// is implemented as like(B,A).
            ///
            /// This same function (with a different compareInfo structure) computes
            /// the GLOB operator.
            ///</summary>
            public static void likeFunc(sqlite3_context context, int argc, sqlite3_value[] argv)
            {
                string zA, zB;
                u32 escape = 0;
                int nPat;
                sqlite3 db = vdbeapi.sqlite3_context_db_handle(context);
                zB = vdbeapi.sqlite3_value_text(argv[0]);
                zA = vdbeapi.sqlite3_value_text(argv[1]);
                ///
                ///<summary>
                ///Limit the length of the LIKE or GLOB pattern to avoid problems
                ///of deep recursion and N*N behavior in patternCompare().
                ///
                ///</summary>
                nPat = vdbeapi.sqlite3_value_bytes(argv[0]);
                sqliteinth.testcase(nPat == db.aLimit[SQLITE_LIMIT_LIKE_PATTERN_LENGTH]);
                sqliteinth.testcase(nPat == db.aLimit[SQLITE_LIMIT_LIKE_PATTERN_LENGTH] + 1);
                if (nPat > db.aLimit[SQLITE_LIMIT_LIKE_PATTERN_LENGTH])
                {
                    context.sqlite3_result_error("LIKE or GLOB pattern too complex", -1);
                    return;
                }
                //Debug.Assert( zB == vdbeapi.sqlite3_value_text( argv[0] ) );  /* Encoding did not change */
                if (argc == 3)
                {
                    ///
                    ///<summary>
                    ///</summary>
                    ///<param name="The escape character string must consist of a single UTF">8 character.</param>
                    ///<param name="Otherwise, return an error.">Otherwise, return an error.</param>
                    ///<param name=""></param>
                    string zEsc = vdbeapi.sqlite3_value_text(argv[2]);
                    if (zEsc == null)
                        return;
                    if (sqlite3Utf8CharLen(zEsc, -1) != 1)
                    {
                        context.sqlite3_result_error("ESCAPE expression must be a single character", -1);
                        return;
                    }
                    escape = sqlite3Utf8Read(zEsc, ref zEsc);
                }
                if (zA != null && zB != null)
                {
                    compareInfo pInfo = (compareInfo)vdbeapi.sqlite3_user_data(context);
#if SQLITE_TEST
#if !TCLSH
																																																																																				        sqlite3_like_count++;
#else
																																																																																				        sqlite3_like_count.iValue++;
#endif
#endif
                    context.sqlite3_result_int(patternCompare(zB, zA, pInfo, escape) ? 1 : 0);
                }
            }
            ///<summary>
            /// Implementation of the NULLIF(x,y) function.  The result is the first
            /// argument if the arguments are different.  The result is NULL if the
            /// arguments are equal to each other.
            ///
            ///</summary>
            static void nullifFunc(sqlite3_context context, int NotUsed, sqlite3_value[] argv)
            {
                CollSeq pColl = sqlite3GetFuncCollSeq(context);
                Sqlite3.sqliteinth.UNUSED_PARAMETER(NotUsed);
                if (sqlite3MemCompare(argv[0], argv[1], pColl) != 0)
                {
                    context.sqlite3_result_value(argv[0]);
                }
            }
            ///<summary>
            /// Implementation of the sqlite_version() function.  The result is the version
            /// of the SQLite library that is running.
            ///
            ///</summary>
            static void versionFunc(sqlite3_context context, int NotUsed, sqlite3_value[] NotUsed2)
            {
                Sqlite3.sqliteinth.UNUSED_PARAMETER2(NotUsed, NotUsed2);
                ///
                ///<summary>
                ///</summary>
                ///<param name="IMP: R">48617 This function is an SQL wrapper around the</param>
                ///<param name="sqlite3_libversion() C">interface. </param>
                context.sqlite3_result_text(sqlite3_libversion(), -1, SQLITE_STATIC);
            }
            ///<summary>
            /// Implementation of the sqlite_source_id() function. The result is a string
            /// that identifies the particular version of the source code used to build
            /// SQLite.
            ///
            ///</summary>
            static void sourceidFunc(sqlite3_context context, int NotUsed, sqlite3_value[] NotUsed2)
            {
                Sqlite3.sqliteinth.UNUSED_PARAMETER2(NotUsed, NotUsed2);
                ///
                ///<summary>
                ///</summary>
                ///<param name="IMP: R">31136 This function is an SQL wrapper around the</param>
                ///<param name="sqlite3_sourceid() C interface. ">sqlite3_sourceid() C interface. </param>
                context.sqlite3_result_text(sqlite3_sourceid(), -1, SQLITE_STATIC);
            }
            ///
            ///<summary>
            ///Implementation of the sqlite_log() function.  This is a wrapper around
            ///io.sqlite3_log().  The return value is NULL.  The function exists purely for
            ///</summary>
            ///<param name="its side">effects.</param>
            ///<param name=""></param>
            static void errlogFunc(sqlite3_context context, int argc, sqlite3_value[] argv)
            {
                Sqlite3.sqliteinth.UNUSED_PARAMETER(argc);
                Sqlite3.sqliteinth.UNUSED_PARAMETER(context);
                io.sqlite3_log(vdbeapi.sqlite3_value_int(argv[0]), "%s", vdbeapi.sqlite3_value_text(argv[1]));
            }
            ///<summary>
            /// Implementation of the sqlite_compileoption_used() function.
            /// The result is an integer that identifies if the compiler option
            /// was used to build SQLite.
            ///
            ///</summary>
#if !SQLITE_OMIT_COMPILEOPTION_DIAGS
            static void compileoptionusedFunc(sqlite3_context context, int argc, sqlite3_value[] argv)
            {
                string zOptName;
                Debug.Assert(argc == 1);
                Sqlite3.sqliteinth.UNUSED_PARAMETER(argc);
                ///
                ///<summary>
                ///</summary>
                ///<param name="IMP: R">36305 The sqlite_compileoption_used() SQL</param>
                ///<param name="function is a wrapper around the sqlite3_compileoption_used() C/C++">function is a wrapper around the sqlite3_compileoption_used() C/C++</param>
                ///<param name="function.">function.</param>
                ///<param name=""></param>
                if ((zOptName = vdbeapi.sqlite3_value_text(argv[0])) != null)
                {
                    context.sqlite3_result_int(sqlite3_compileoption_used(zOptName));
                }
            }
#endif
            ///<summary>
            /// Implementation of the sqlite_compileoption_get() function.
            /// The result is a string that identifies the compiler options
            /// used to build SQLite.
            ///</summary>
#if !SQLITE_OMIT_COMPILEOPTION_DIAGS
            static void compileoptiongetFunc(sqlite3_context context, int argc, sqlite3_value[] argv)
            {
                int n;
                Debug.Assert(argc == 1);
                Sqlite3.sqliteinth.UNUSED_PARAMETER(argc);
                ///
                ///<summary>
                ///</summary>
                ///<param name="IMP: R">24076 The sqlite_compileoption_get() SQL function</param>
                ///<param name="is a wrapper around the sqlite3_compileoption_get() C/C++ function.">is a wrapper around the sqlite3_compileoption_get() C/C++ function.</param>
                ///<param name=""></param>
                n = vdbeapi.sqlite3_value_int(argv[0]);
                context.sqlite3_result_text(sqlite3_compileoption_get(n), -1, SQLITE_STATIC);
            }
#endif
            ///<summary>
            ///Array for converting from half-bytes (nybbles) into ASCII hex
            /// digits.
            ///</summary>
            static char[] hexdigits = new char[] {
			'0',
			'1',
			'2',
			'3',
			'4',
			'5',
			'6',
			'7',
			'8',
			'9',
			'A',
			'B',
			'C',
			'D',
			'E',
			'F'
		};
            ///<summary>
            /// EXPERIMENTAL - This is not an official function.  The interface may
            /// change.  This function may disappear.  Do not write code that depends
            /// on this function.
            ///
            /// Implementation of the QUOTE() function.  This function takes a single
            /// argument.  If the argument is numeric, the return value is the same as
            /// the argument.  If the argument is NULL, the return value is the string
            /// "NULL".  Otherwise, the argument is enclosed in single quotes with
            /// single-quote escapes.
            ///
            ///</summary>
            static void quoteFunc(sqlite3_context context, int argc, sqlite3_value[] argv)
            {
                Debug.Assert(argc == 1);
                Sqlite3.sqliteinth.UNUSED_PARAMETER(argc);
                switch (vdbeapi.sqlite3_value_type(argv[0]))
                {
                    case SQLITE_INTEGER:
                    case SQLITE_FLOAT:
                        {
                            context.sqlite3_result_value(argv[0]);
                            break;
                        }
                    case SQLITE_BLOB:
                        {
                            StringBuilder zText;
                            byte[] zBlob = vdbeapi.sqlite3_value_blob(argv[0]);
                            int nBlob = vdbeapi.sqlite3_value_bytes(argv[0]);
                            Debug.Assert(zBlob.Length == vdbeapi.sqlite3_value_blob(argv[0]).Length);
                            ///
                            ///<summary>
                            ///No encoding change 
                            ///</summary>
                            zText = new StringBuilder(2 * nBlob + 4);
                            //(char*)contextMalloc(context, (2*(i64)nBlob)+4);
                            zText.Append("X'");
                            if (zText != null)
                            {
                                int i;
                                for (i = 0; i < nBlob; i++)
                                {
                                    zText.Append(hexdigits[(zBlob[i] >> 4) & 0x0F]);
                                    zText.Append(hexdigits[(zBlob[i]) & 0x0F]);
                                }
                                zText.Append("'");
                                //zText[( nBlob * 2 ) + 2] = '\'';
                                //zText[( nBlob * 2 ) + 3] = '\0';
                                //zText[0] = 'X';
                                //zText[1] = '\'';
                                context.sqlite3_result_text(zText, -1, SQLITE_TRANSIENT);
                                //malloc_cs.sqlite3_free( zText );
                            }
                            break;
                        }
                    case SQLITE_TEXT:
                        {
                            int i, j;
                            int n;
                            string zArg = vdbeapi.sqlite3_value_text(argv[0]);
                            StringBuilder z;
                            if (zArg == null || zArg.Length == 0)
                                return;
                            for (i = 0, n = 0; i < zArg.Length; i++)
                            {
                                if (zArg[i] == '\'')
                                    n++;
                            }
                            z = new StringBuilder(i + n + 3);
                            // contextMalloc(context, ((i64)i)+((i64)n)+3);
                            if (z != null)
                            {
                                z.Append('\'');
                                for (i = 0, j = 1; i < zArg.Length && zArg[i] != 0; i++)
                                {
                                    z.Append((char)zArg[i]);
                                    j++;
                                    if (zArg[i] == '\'')
                                    {
                                        z.Append('\'');
                                        j++;
                                    }
                                }
                                z.Append('\'');
                                j++;
                                //z[j] = '\0'; ;
                                context.sqlite3_result_text(z, j, null);
                                //malloc_cs.sqlite3_free );
                            }
                            break;
                        }
                    default:
                        {
                            Debug.Assert(vdbeapi.sqlite3_value_type(argv[0]) == SQLITE_NULL);
                            context.sqlite3_result_text("NULL", 4, SQLITE_STATIC);
                            break;
                        }
                }
            }
            ///<summary>
            /// The hex() function.  Interpret the argument as a blob.  Return
            /// a hexadecimal rendering as text.
            ///
            ///</summary>
            static void hexFunc(sqlite3_context context, int argc, sqlite3_value[] argv)
            {
                int i, n;
                byte[] pBlob;
                //string zHex, z;
                Debug.Assert(argc == 1);
                Sqlite3.sqliteinth.UNUSED_PARAMETER(argc);
                pBlob = vdbeapi.sqlite3_value_blob(argv[0]);
                n = vdbeapi.sqlite3_value_bytes(argv[0]);
                Debug.Assert(n == (pBlob == null ? 0 : pBlob.Length));
                ///
                ///<summary>
                ///No encoding change 
                ///</summary>
                StringBuilder zHex = new StringBuilder(n * 2 + 1);
                //  z = zHex = contextMalloc(context, ((i64)n)*2 + 1);
                if (zHex != null)
                {
                    for (i = 0; i < n; i++)
                    {
                        //, pBlob++){
                        byte c = pBlob[i];
                        zHex.Append(hexdigits[(c >> 4) & 0xf]);
                        zHex.Append(hexdigits[c & 0xf]);
                    }
                    context.sqlite3_result_text(zHex, n * 2, null);
                    //malloc_cs.sqlite3_free );
                }
            }
            ///<summary>
            /// The zeroblob(N) function returns a zero-filled blob of size N bytes.
            ///
            ///</summary>
            static void zeroblobFunc(sqlite3_context context, int argc, sqlite3_value[] argv)
            {
                i64 n;
                sqlite3 db = vdbeapi.sqlite3_context_db_handle(context);
                Debug.Assert(argc == 1);
                Sqlite3.sqliteinth.UNUSED_PARAMETER(argc);
                n = vdbeapi.sqlite3_value_int64(argv[0]);
                sqliteinth.testcase(n == db.aLimit[SQLITE_LIMIT_LENGTH]);
                sqliteinth.testcase(n == db.aLimit[SQLITE_LIMIT_LENGTH] + 1);
                if (n > db.aLimit[SQLITE_LIMIT_LENGTH])
                {
                    context.sqlite3_result_error_toobig();
                }
                else
                {
                    context.sqlite3_result_zeroblob((int)n);
                    ///
                    ///<summary>
                    ///</summary>
                    ///<param name="IMP: R">64994 </param>
                }
            }
            ///<summary>
            /// The replace() function.  Three arguments are all strings: call
            /// them A, B, and C. The result is also a string which is derived
            /// from A by replacing every occurance of B with C.  The match
            /// must be exact.  Collating sequences are not used.
            ///
            ///</summary>
            static void replaceFunc(sqlite3_context context, int argc, sqlite3_value[] argv)
            {
                string zStr;
                ///
                ///<summary>
                ///The input string A 
                ///</summary>
                string zPattern;
                ///
                ///<summary>
                ///The pattern string B 
                ///</summary>
                string zRep;
                ///
                ///<summary>
                ///The replacement string C 
                ///</summary>
                string zOut = null;
                ///
                ///<summary>
                ///The output 
                ///</summary>
                int nStr;
                ///
                ///<summary>
                ///Size of zStr 
                ///</summary>
                int nPattern;
                ///
                ///<summary>
                ///Size of zPattern 
                ///</summary>
                int nRep;
                ///
                ///<summary>
                ///Size of zRep 
                ///</summary>
                int nOut;
                ///
                ///<summary>
                ///Maximum size of zOut 
                ///</summary>
                //int loopLimit;    /* Last zStr[] that might match zPattern[] */
                int i, j = 0;
                ///
                ///<summary>
                ///Loop counters 
                ///</summary>
                Debug.Assert(argc == 3);
                Sqlite3.sqliteinth.UNUSED_PARAMETER(argc);
                zStr = vdbeapi.sqlite3_value_text(argv[0]);
                if (zStr == null)
                    return;
                nStr = vdbeapi.sqlite3_value_bytes(argv[0]);
                Debug.Assert(zStr == vdbeapi.sqlite3_value_text(argv[0]));
                ///
                ///<summary>
                ///No encoding change 
                ///</summary>
                zPattern = vdbeapi.sqlite3_value_text(argv[1]);
                if (zPattern == null)
                {
                    Debug.Assert(vdbeapi.sqlite3_value_type(argv[1]) == SQLITE_NULL//|| vdbeapi.sqlite3_context_db_handle( context ).mallocFailed != 0
                    );
                    return;
                }
                if (zPattern == "")
                {
                    Debug.Assert(vdbeapi.sqlite3_value_type(argv[1]) != SQLITE_NULL);
                    context.sqlite3_result_value(argv[0]);
                    return;
                }
                nPattern = vdbeapi.sqlite3_value_bytes(argv[1]);
                Debug.Assert(zPattern == vdbeapi.sqlite3_value_text(argv[1]));
                ///
                ///<summary>
                ///No encoding change 
                ///</summary>
                zRep = vdbeapi.sqlite3_value_text(argv[2]);
                if (zRep == null)
                    return;
                nRep = vdbeapi.sqlite3_value_bytes(argv[2]);
                Debug.Assert(zRep == vdbeapi.sqlite3_value_text(argv[2]));
                nOut = nStr + 1;
                Debug.Assert(nOut < SQLITE_MAX_LENGTH);
                if (nOut <= vdbeapi.sqlite3_context_db_handle(context).aLimit[SQLITE_LIMIT_LENGTH])
                {
                    //zOut = contextMalloc(context, (i64)nOut);
                    //if( zOut==0 ){
                    //  return;
                    //}
                    //loopLimit = nStr - nPattern;
                    //for(i=j=0; i<=loopLimit; i++){
                    //  if( zStr[i]!=zPattern[0] || _Custom._Custom._Custom._Custom.memcmp(&zStr[i], zPattern, nPattern) ){
                    //    zOut[j++] = zStr[i];
                    //  }else{
                    //    u8 *zOld;
                    // sqlite3 db = vdbeapi.sqlite3_context_db_handle( context );
                    //    nOut += nRep - nPattern;
                    //sqliteinth.testcase( nOut-1==db->aLimit[SQLITE_LIMIT_LENGTH] );
                    //sqliteinth.testcase( nOut-2==db->aLimit[SQLITE_LIMIT_LENGTH] );
                    //if( nOut-1>db->aLimit[SQLITE_LIMIT_LENGTH] ){
                    //      sqlite3_result_error_toobig(context);
                    //      malloc_cs.sqlite3_free(zOut);
                    //      return;
                    //    }
                    //    zOld = zOut;
                    //    zOut = sqlite3_realloc(zOut, (int)nOut);
                    //    if( zOut==0 ){
                    //      sqlite3_result_error_nomem(context);
                    //      malloc_cs.sqlite3_free(zOld);
                    //      return;
                    //    }
                    //    memcpy(&zOut[j], zRep, nRep);
                    //    j += nRep;
                    //    i += nPattern-1;
                    //  }
                    //}
                    //Debug.Assert( j+nStr-i+1==nOut );
                    //memcpy(&zOut[j], zStr[i], nStr-i);
                    //j += nStr - i;
                    //Debug.Assert( j<=nOut );
                    //zOut[j] = 0;
                    try
                    {
                        zOut = zStr.Replace(zPattern, zRep);
                        j = zOut.Length;
                    }
                    catch
                    {
                        j = 0;
                    }
                }
                if (j == 0 || j > vdbeapi.sqlite3_context_db_handle(context).aLimit[SQLITE_LIMIT_LENGTH])
                {
                    context.sqlite3_result_error_toobig();
                }
                else
                {
                    context.sqlite3_result_text(zOut, j, null);
                    //malloc_cs.sqlite3_free );
                }
            }
            ///
            ///<summary>
            ///Implementation of the TRIM(), LTRIM(), and RTRIM() functions.
            ///The userdata is 0x1 for left trim, 0x2 for right trim, 0x3 for both.
            ///
            ///</summary>
            static void trimFunc(sqlite3_context context, int argc, sqlite3_value[] argv)
            {
                string zIn;
                ///
                ///<summary>
                ///Input string 
                ///</summary>
                string zCharSet;
                ///
                ///<summary>
                ///Set of characters to trim 
                ///</summary>
                int nIn;
                ///
                ///<summary>
                ///Number of bytes in input 
                ///</summary>
                int izIn = 0;
                ///
                ///<summary>
                ///C# string pointer 
                ///</summary>
                int flags;
                ///
                ///<summary>
                ///1: trimleft  2: trimright  3: trim 
                ///</summary>
                int i;
                ///
                ///<summary>
                ///Loop counter 
                ///</summary>
                int[] aLen = null;
                ///
                ///<summary>
                ///Length of each character in zCharSet 
                ///</summary>
                byte[][] azChar = null;
                ///
                ///<summary>
                ///Individual characters in zCharSet 
                ///</summary>
                int nChar = 0;
                ///
                ///<summary>
                ///Number of characters in zCharSet 
                ///</summary>
                byte[] zBytes = null;
                byte[] zBlob = null;
                if (vdbeapi.sqlite3_value_type(argv[0]) == SQLITE_NULL)
                {
                    return;
                }
                zIn = vdbeapi.sqlite3_value_text(argv[0]);
                if (zIn == null)
                    return;
                nIn = vdbeapi.sqlite3_value_bytes(argv[0]);
                zBlob = vdbeapi.sqlite3_value_blob(argv[0]);
                //Debug.Assert( zIn == vdbeapi.sqlite3_value_text( argv[0] ) );
                if (argc == 1)
                {
                    int[] lenOne = new int[] {
					1
				};
                    byte[] azOne = new byte[] {
					(u8)' '
				};
                    //static unsigned char * const azOne[] = { (u8*)" " };
                    nChar = 1;
                    aLen = lenOne;
                    azChar = new byte[1][];
                    azChar[0] = azOne;
                    zCharSet = null;
                }
                else
                    if ((zCharSet = vdbeapi.sqlite3_value_text(argv[1])) == null)
                    {
                        return;
                    }
                    else
                    {
                        if ((zBytes = vdbeapi.sqlite3_value_blob(argv[1])) != null)
                        {
                            int iz = 0;
                            for (nChar = 0; iz < zBytes.Length; nChar++)
                            {
                                sqliteinth.SQLITE_SKIP_UTF8(zBytes, ref iz);
                            }
                            if (nChar > 0)
                            {
                                azChar = new byte[nChar][];
                                //contextMalloc(context, ((i64)nChar)*(sizeof(char*)+1));
                                if (azChar == null)
                                {
                                    return;
                                }
                                aLen = new int[nChar];
                                int iz0 = 0;
                                int iz1 = 0;
                                for (int ii = 0; ii < nChar; ii++)
                                {
                                    sqliteinth.SQLITE_SKIP_UTF8(zBytes, ref iz1);
                                    aLen[ii] = iz1 - iz0;
                                    azChar[ii] = new byte[aLen[ii]];
                                    Buffer.BlockCopy(zBytes, iz0, azChar[ii], 0, azChar[ii].Length);
                                    iz0 = iz1;
                                }
                            }
                        }
                    }
                if (nChar > 0)
                {
                    flags = (int)vdbeapi.sqlite3_user_data(context);
                    // flags = SQLITE_PTR_TO_INT(sqlite3_user_data(context));
                    if ((flags & 1) != 0)
                    {
                        while (nIn > 0)
                        {
                            int len = 0;
                            for (i = 0; i < nChar; i++)
                            {
                                len = aLen[i];
                                if (len <= nIn && _Custom.memcmp(zBlob, izIn, azChar[i], len) == 0)
                                    break;
                            }
                            if (i >= nChar)
                                break;
                            izIn += len;
                            nIn -= len;
                        }
                    }
                    if ((flags & 2) != 0)
                    {
                        while (nIn > 0)
                        {
                            int len = 0;
                            for (i = 0; i < nChar; i++)
                            {
                                len = aLen[i];
                                if (len <= nIn && _Custom.memcmp(zBlob, izIn + nIn - len, azChar[i], len) == 0)
                                    break;
                            }
                            if (i >= nChar)
                                break;
                            nIn -= len;
                        }
                    }
                    if (zCharSet != null)
                    {
                        //malloc_cs.sqlite3_free( ref azChar );
                    }
                }
                StringBuilder sb = new StringBuilder(nIn);
                for (i = 0; i < nIn; i++)
                    sb.Append((char)zBlob[izIn + i]);
                context.sqlite3_result_text(sb, nIn, SQLITE_TRANSIENT);
            }
            ///<summary>
            ///IMP: R-25361-16150 This function is omitted from SQLite by default. It
            /// is only available if the SQLITE_SOUNDEX compile-time option is used
            /// when SQLite is built.
            ///
            ///</summary>
#if SQLITE_SOUNDEX
																																										/*
** Compute the soundex encoding of a word.
**
** IMP: R-59782-00072 The soundex(X) function returns a string that is the
** soundex encoding of the string X. 
*/
static void soundexFunc(
sqlite3_context context,
int argc,
sqlite3_value[] argv
)
{
Debug.Assert(false); // TODO -- func_c
char zResult[8];
const u8 *zIn;
int i, j;
static const unsigned char iCode[] = {
0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
0, 0, 1, 2, 3, 0, 1, 2, 0, 0, 2, 2, 4, 5, 5, 0,
1, 2, 6, 2, 3, 0, 1, 0, 2, 0, 2, 0, 0, 0, 0, 0,
0, 0, 1, 2, 3, 0, 1, 2, 0, 0, 2, 2, 4, 5, 5, 0,
1, 2, 6, 2, 3, 0, 1, 0, 2, 0, 2, 0, 0, 0, 0, 0,
};
Debug.Assert( argc==1 );
zIn = (u8*)vdbeapi.sqlite3_value_text(argv[0]);
if( zIn==0 ) zIn = (u8*)"";
for(i=0; zIn[i] && !sqlite3Isalpha(zIn[i]); i++){}
if( zIn[i] ){
u8 prevcode = iCode[zIn[i]&0x7f];
zResult[0] = sqlite3Toupper(zIn[i]);
for(j=1; j<4 && zIn[i]; i++){
int code = iCode[zIn[i]&0x7f];
if( code>0 ){
if( code!=prevcode ){
prevcode = code;
zResult[j++] = code + '0';
}
}else{
prevcode = 0;
}
}
while( j<4 ){
zResult[j++] = '0';
}
zResult[j] = 0;
sqlite3_result_text(context, zResult, 4, SQLITE_TRANSIENT);
}else{
/* IMP: R-64894-50321 The string "?000" is returned if the argument
** is NULL or contains no ASCII alphabetic characters. */
sqlite3_result_text(context, "?000", 4, SQLITE_STATIC);
}
}
#endif
#if !SQLITE_OMIT_LOAD_EXTENSION
            ///<summary>
            /// A function that loads a shared-library extension then returns NULL.
            ///</summary>
            static void loadExt(sqlite3_context context, int argc, sqlite3_value[] argv)
            {
                string zFile = vdbeapi.sqlite3_value_text(argv[0]);
                string zProc;
                sqlite3 db = (sqlite3)vdbeapi.sqlite3_context_db_handle(context);
                string zErrMsg = "";
                if (argc == 2)
                {
                    zProc = vdbeapi.sqlite3_value_text(argv[1]);
                }
                else
                {
                    zProc = "";
                }
                if (zFile != null && sqlite3_load_extension(db, zFile, zProc, ref zErrMsg) != 0)
                {
                    context.sqlite3_result_error(zErrMsg, -1);
                    db.sqlite3DbFree(ref zErrMsg);
                }
            }
#endif
            

            ///<summary>
            /// Routines used to compute the sum, average, and total.
            ///
            /// The SUM() function follows the (broken) SQL standard which means
            /// that it returns NULL if it sums over no inputs.  TOTAL returns
            /// 0.0 in that case.  In addition, TOTAL always returns a float where
            /// SUM might return an integer if it never encounters a floating point
            /// value.  TOTAL never fails, but SUM might through an exception if
            /// it overflows an integer.
            ///
            ///</summary>
            static void sumStep(sqlite3_context context, int argc, sqlite3_value[] argv)
            {
                SumCtx p;
                int type;
                Debug.Assert(argc == 1);
                Sqlite3.sqliteinth.UNUSED_PARAMETER(argc);
                Mem pMem = vdbeapi.sqlite3_aggregate_context(context, 1);
                //sizeof(*p));
                if (pMem._SumCtx == null)
                    pMem._SumCtx = new SumCtx();
                p = pMem._SumCtx;
                if (p.Context == null)
                    p.Context = pMem;
                type = sqlite3_value_numeric_type(argv[0]);
                if (p != null && type != SQLITE_NULL)
                {
                    p.cnt++;
                    if (type == SQLITE_INTEGER)
                    {
                        i64 v = vdbeapi.sqlite3_value_int64(argv[0]);
                        p.rSum += v;
                        if (!(p.approx | p.overflow != 0) && 0 != utilc.sqlite3AddInt64(ref p.iSum, v))
                        {
                            p.overflow = 1;
                        }
                    }
                    else
                    {
                        p.rSum += vdbeapi.sqlite3_value_double(argv[0]);
                        p.approx = true;
                    }
                }
            }
            static void sumFinalize(sqlite3_context context)
            {
                SumCtx p = null;
                Mem pMem = vdbeapi.sqlite3_aggregate_context(context, 0);
                if (pMem != null)
                    p = pMem._SumCtx;
                if (p != null && p.cnt > 0)
                {
                    if (p.overflow != 0)
                    {
                        context.sqlite3_result_error("integer overflow", -1);
                    }
                    else
                        if (p.approx)
                        {
                            context.sqlite3_result_double(p.rSum);
                        }
                        else
                        {
                            context.sqlite3_result_int64(p.iSum);
                        }
                    p.cnt = 0;
                    // Reset for C#
                }
            }
            static void avgFinalize(sqlite3_context context)
            {
                SumCtx p = null;
                Mem pMem = vdbeapi.sqlite3_aggregate_context(context, 0);
                if (pMem != null)
                    p = pMem._SumCtx;
                if (p != null && p.cnt > 0)
                {
                    context.sqlite3_result_double(p.rSum / (double)p.cnt);
                }
            }
            static void totalFinalize(sqlite3_context context)
            {
                SumCtx p = null;
                Mem pMem = vdbeapi.sqlite3_aggregate_context(context, 0);
                if (pMem != null)
                    p = pMem._SumCtx;
                ///
                ///<summary>
                ///(double)0 In case of SQLITE_OMIT_FLOATING_POINT... 
                ///</summary>
                context.sqlite3_result_double(p != null ? p.rSum : (double)0);
            }
            ///<summary>
            /// The following structure keeps track of state information for the
            /// count() aggregate function.
            ///
            ///</summary>
            //typedef struct CountCtx CountCtx;
            public class CountCtx
            {
                i64 _n;
                Mem _M;
                public Mem Context
                {
                    get
                    {
                        return _M;
                    }
                    set
                    {
                        _M = value;
                        if (_M == null || _M.z == null)
                            _n = 0;
                        else
                            _n = Convert.ToInt64(_M.z);
                    }
                }
                public i64 n
                {
                    get
                    {
                        return _n;
                    }
                    set
                    {
                        _n = value;
                        if (_M != null)
                            _M.z = _n.ToString();
                    }
                }
            }
            ///<summary>
            /// Routines to implement the count() aggregate function.
            ///
            ///</summary>
            static void countStep(sqlite3_context context, int argc, sqlite3_value[] argv)
            {
                CountCtx p = new CountCtx();
                p.Context = vdbeapi.sqlite3_aggregate_context(context, 1);
                //sizeof(*p));
                if ((argc == 0 || SQLITE_NULL != vdbeapi.sqlite3_value_type(argv[0])) && p.Context != null)
                {
                    p.n++;
                }
#if !SQLITE_OMIT_DEPRECATED
																																																															/* The sqlite3_aggregate_count() function is deprecated.  But just to make
** sure it still operates correctly, verify that its count agrees with our
** internal count when using count(*) and when the total count can be
** expressed as a 32-bit integer. */
Debug.Assert( argc == 1 || p == null || p.n > 0x7fffffff
|| p.n == sqlite3_aggregate_count( context ) );
#endif
            }
            static void countFinalize(sqlite3_context context)
            {
                CountCtx p = new CountCtx();
                p.Context = vdbeapi.sqlite3_aggregate_context(context, 0);
                context.sqlite3_result_int64(p != null ? p.n : 0);
            }
            ///<summary>
            /// Routines to implement min() and max() aggregate functions.
            ///
            ///</summary>
            static void minmaxStep(sqlite3_context context, int NotUsed, sqlite3_value[] argv)
            {
                Mem pArg = (Mem)argv[0];
                Mem pBest;
                Sqlite3.sqliteinth.UNUSED_PARAMETER(NotUsed);
                if (vdbeapi.sqlite3_value_type(argv[0]) == SQLITE_NULL)
                    return;
                pBest = (Mem)vdbeapi.sqlite3_aggregate_context(context, 1);
                //sizeof(*pBest));
                //if ( pBest == null ) return;
                if (pBest.flags != 0)
                {
                    bool max;
                    int cmp;
                    CollSeq pColl = sqlite3GetFuncCollSeq(context);
                    ///
                    ///<summary>
                    ///This step function is used for both the min() and max() aggregates,
                    ///the only difference between the two being that the sense of the
                    ///comparison is inverted. For the max() aggregate, the
                    ///</summary>
                    ///<param name="vdbeapi.sqlite3_context_db_handle() function returns (void *)">1. For min() it</param>
                    ///<param name="returns (void *)db, where db is the sqlite3* database pointer.">returns (void *)db, where db is the sqlite3* database pointer.</param>
                    ///<param name="Therefore the next statement sets variable 'max' to 1 for the max()">Therefore the next statement sets variable 'max' to 1 for the max()</param>
                    ///<param name="aggregate, or 0 for min().">aggregate, or 0 for min().</param>
                    ///<param name=""></param>
                    max = vdbeapi.sqlite3_context_db_handle(context) != null && (int)vdbeapi.sqlite3_user_data(context) != 0;
                    cmp = sqlite3MemCompare(pBest, pArg, pColl);
                    if ((max && cmp < 0) || (!max && cmp > 0))
                    {
                        sqlite3VdbeMemCopy(pBest, pArg);
                    }
                }
                else
                {
                    sqlite3VdbeMemCopy(pBest, pArg);
                }
            }
            static void minMaxFinalize(sqlite3_context context)
            {
                sqlite3_value pRes;
                pRes = (sqlite3_value)vdbeapi.sqlite3_aggregate_context(context, 0);
                if (pRes != null)
                {
                    if (Sqlite3.ALWAYS(pRes.flags != 0))
                    {
                        context.sqlite3_result_value(pRes);
                    }
                    sqlite3VdbeMemRelease(pRes);
                }
            }
            ///<summary>
            /// group_concat(EXPR, ?SEPARATOR?)
            ///
            ///</summary>
            static void groupConcatStep(sqlite3_context context, int argc, sqlite3_value[] argv)
            {
                string zVal;
                //StrAccum pAccum;
                string zSep;
                int nVal, nSep;
                Debug.Assert(argc == 1 || argc == 2);
                if (vdbeapi.sqlite3_value_type(argv[0]) == SQLITE_NULL)
                    return;
                Mem pMem = vdbeapi.sqlite3_aggregate_context(context, 1);
                //sizeof(*pAccum));
                if (pMem._StrAccum == null)
                    pMem._StrAccum = new StrAccum(100);
                //pAccum = pMem._StrAccum;
                //if ( pMem._StrAccum != null )
                //{
                sqlite3 db = vdbeapi.sqlite3_context_db_handle(context);
                //int firstTerm = pMem._StrAccum.useMalloc == 0 ? 1 : 0;
                //pMem._StrAccum.useMalloc = 2;
                pMem._StrAccum.mxAlloc = db.aLimit[SQLITE_LIMIT_LENGTH];
                if (pMem._StrAccum.Context == null)
                    // first term
                    pMem._StrAccum.Context = pMem;
                else
                {
                    if (argc == 2)
                    {
                        zSep = vdbeapi.sqlite3_value_text(argv[1]);
                        nSep = vdbeapi.sqlite3_value_bytes(argv[1]);
                    }
                    else
                    {
                        zSep = ",";
                        nSep = 1;
                    }
                    pMem._StrAccum.sqlite3StrAccumAppend(zSep, nSep);
                }
                zVal = vdbeapi.sqlite3_value_text(argv[0]);
                nVal = vdbeapi.sqlite3_value_bytes(argv[0]);
                pMem._StrAccum.sqlite3StrAccumAppend(zVal, nVal);
                //}
            }
            static void groupConcatFinalize(sqlite3_context context)
            {
                //StrAccum pAccum = null;
                Mem pMem = vdbeapi.sqlite3_aggregate_context(context, 0);
                if (pMem != null)
                {
                    if (pMem._StrAccum == null)
                        pMem._StrAccum = new StrAccum(100);
                    StrAccum pAccum = pMem._StrAccum;
                    //}
                    //if ( pAccum != null )
                    //{
                    if (pAccum.tooBig)
                    {
                        context.sqlite3_result_error_toobig();
                    }
                    //else if ( pAccum.mallocFailed != 0 )
                    //{
                    //  sqlite3_result_error_nomem( context );
                    //}
                    else
                    {
                        context.sqlite3_result_text(io.sqlite3StrAccumFinish(pAccum), -1, null);
                        //malloc_cs.sqlite3_free );
                    }
                }
            }
            ///<summary>
            /// This routine does per-connection function registration.  Most
            /// of the built-in functions above are part of the global function set.
            /// This routine only deals with those that are not global.
            ///
            ///</summary>
            public struct sFuncs
            {
                public string zName;
                public sbyte nArg;
                public u8 argType;
                ///
                ///<summary>
                ///</summary>
                ///<param name="1: 0, 2: 1, 3: 2,...  N:  N">1. </param>
                public u8 eTextRep;
                ///
                ///<summary>
                ///</summary>
                ///<param name="1: UTF">8 </param>
                public u8 needCollSeq;
                public dxFunc xFunc;
                //(sqlite3_context*,int,sqlite3_value **);
                // Constructor
                public sFuncs(string zName, sbyte nArg, u8 argType, u8 eTextRep, u8 needCollSeq, dxFunc xFunc)
                {
                    this.zName = zName;
                    this.nArg = nArg;
                    this.argType = argType;
                    this.eTextRep = eTextRep;
                    this.needCollSeq = needCollSeq;
                    this.xFunc = xFunc;
                }
            }
            public struct sAggs
            {
                public string zName;
                public sbyte nArg;
                public u8 argType;
                public u8 needCollSeq;
                public dxStep xStep;
                //(sqlite3_context*,int,sqlite3_value**);
                public dxFinal xFinalize;
                //(sqlite3_context*);
                // Constructor
                public sAggs(string zName, sbyte nArg, u8 argType, u8 needCollSeq, dxStep xStep, dxFinal xFinalize)
                {
                    this.zName = zName;
                    this.nArg = nArg;
                    this.argType = argType;
                    this.needCollSeq = needCollSeq;
                    this.xStep = xStep;
                    this.xFinalize = xFinalize;
                }
            }
            public static void sqlite3RegisterBuiltinFunctions(sqlite3 db)
            {
                int rc = sqlite3_overload_function(db, "MATCH", 2);
                Debug.Assert(rc == SQLITE_NOMEM || rc == SQLITE_OK);
                if (rc == SQLITE_NOMEM)
                {
                    ////        db.mallocFailed = 1;
                }
            }
            ///<summary>
            /// Set the LIKEOPT flag on the 2-argument function with the given name.
            ///
            ///</summary>
            static void setLikeOptFlag(sqlite3 db, string zName, FuncFlags flagVal)
            {
                FuncDef pDef;
                pDef = sqlite3FindFunction(db, zName, StringExtensions.sqlite3Strlen30(zName), 2, SqliteEncoding.UTF8, 0);
                if (Sqlite3.ALWAYS(pDef != null))
                {
                    pDef.flags = flagVal;
                }
            }
            ///<summary>
            /// Register the built-in LIKE and GLOB functions.  The caseSensitive
            /// parameter determines whether or not the LIKE operator is case
            /// sensitive.  GLOB is always case sensitive.
            ///
            ///</summary>
            public static void sqlite3RegisterLikeFunctions(sqlite3 db, int caseSensitive)
            {
                compareInfo pInfo;
                if (caseSensitive != 0)
                {
                    pInfo = likeInfoAlt;
                }
                else
                {
                    pInfo = likeInfoNorm;
                }
                sqlite3CreateFunc(db, "like", 2, SqliteEncoding.UTF8, pInfo, (dxFunc)likeFunc, null, null, null);
                sqlite3CreateFunc(db, "like", 3, SqliteEncoding.UTF8, pInfo, (dxFunc)likeFunc, null, null, null);
                sqlite3CreateFunc(db, "glob", 2, SqliteEncoding.UTF8, globInfo, (dxFunc)likeFunc, null, null, null);
                setLikeOptFlag(db, "glob", FuncFlags.SQLITE_FUNC_LIKE | FuncFlags.SQLITE_FUNC_CASE);
                setLikeOptFlag(db, "like", caseSensitive != 0 ? (FuncFlags.SQLITE_FUNC_LIKE | FuncFlags.SQLITE_FUNC_CASE) : FuncFlags.SQLITE_FUNC_LIKE);
            }
            ///<summary>
            /// pExpr points to an expression which implements a function.  If
            /// it is appropriate to apply the LIKE optimization to that function
            /// then set aWc[0] through aWc[2] to the wildcard characters and
            /// return TRUE.  If the function is not a LIKE-style function then
            /// return FALSE.
            ///
            ///</summary>
            public static bool sqlite3IsLikeFunction(sqlite3 db, Expr pExpr, ref bool pIsNocase, char[] aWc)
            {
                FuncDef pDef;
                if (pExpr.Operator != TokenType.TK_FUNCTION || null == pExpr.x.pList || pExpr.x.pList.nExpr != 2)
                {
                    return false;
                }
                Debug.Assert(!pExpr.ExprHasProperty(ExprFlags.EP_xIsSelect));
                pDef = sqlite3FindFunction(db, pExpr.u.zToken, StringExtensions.sqlite3Strlen30(pExpr.u.zToken), 2, SqliteEncoding.UTF8, 0);
                if (NEVER(pDef == null) || (pDef.flags & FuncFlags.SQLITE_FUNC_LIKE) == 0)
                {
                    return false;
                }
                ///
                ///<summary>
                ///The memcpy() statement assumes that the wildcard characters are
                ///the first three statements in the compareInfo structure.  The
                ///Debug.Asserts() that follow verify that assumption
                ///
                ///</summary>
                //memcpy( aWc, pDef.pUserData, 3 );
                aWc[0] = ((compareInfo)pDef.pUserData).matchAll;
                aWc[1] = ((compareInfo)pDef.pUserData).matchOne;
                aWc[2] = ((compareInfo)pDef.pUserData).matchSet;
                // Debug.Assert((char*)&likeInfoAlt == (char*)&likeInfoAlt.matchAll);
                // Debug.Assert(&((char*)&likeInfoAlt)[1] == (char*)&likeInfoAlt.matchOne);
                // Debug.Assert(&((char*)&likeInfoAlt)[2] == (char*)&likeInfoAlt.matchSet);
                pIsNocase = (pDef.flags & FuncFlags.SQLITE_FUNC_CASE) == 0;
                return true;
            }
            ///
            ///<summary>
            ///All all of the FuncDef structures in the aBuiltinFunc[] array above
            ///</summary>
            ///<param name="to the global function hash table.  This occurs at start">time (as</param>
            ///<param name="a consequence of calling sqlite3_initialize()).">a consequence of calling sqlite3_initialize()).</param>
            ///<param name=""></param>
            ///<param name="After this routine runs">After this routine runs</param>
            ///<param name=""></param>
            public static void sqlite3RegisterGlobalFunctions()
            {
                ///
                ///<summary>
                ///The following array holds FuncDef structures for all of the functions
                ///defined in this file.
                ///
                ///The array cannot be constant since changes are made to the
                ///</summary>
                ///<param name="FuncDef.pHash elements at start">time.  The elements of this array</param>
                ///<param name="are read">only after initialization is complete.</param>
                ///<param name=""></param>
                FuncDef[] aBuiltinFunc = {
				    FuncDef.FUNCTION("ltrim",1,1,0,trimFunc),
				    FuncDef.FUNCTION("ltrim",2,1,0,trimFunc),
				    FuncDef.FUNCTION("rtrim",1,2,0,trimFunc),
				    FuncDef.FUNCTION("rtrim",2,2,0,trimFunc),
				    FuncDef.FUNCTION("trim",1,3,0,trimFunc),
				    FuncDef.FUNCTION("trim",2,3,0,trimFunc),
				    FuncDef.FUNCTION("min",-1,0,1,minmaxFunc),
				    FuncDef.FUNCTION("min",0,0,1,null),
				    FuncDef.AGGREGATE("min",1,0,1,minmaxStep,minMaxFinalize),
				    FuncDef.FUNCTION("max",-1,1,1,minmaxFunc),
				    FuncDef.FUNCTION("max",0,1,1,null),
				    FuncDef.AGGREGATE("max",1,1,1,minmaxStep,minMaxFinalize),
				    FuncDef.FUNCTION("typeof",1,0,0,typeofFunc),
				    FuncDef.FUNCTION("length",1,0,0,lengthFunc),
				    FuncDef.FUNCTION("substr",2,0,0,substrFunc),
				    FuncDef.FUNCTION("substr",3,0,0,substrFunc),
				    FuncDef.FUNCTION("abs",1,0,0,absFunc),
				    #if !SQLITE_OMIT_FLOATING_POINT
				    FuncDef.FUNCTION("round",1,0,0,roundFunc),
				    FuncDef.FUNCTION("round",2,0,0,roundFunc),
				    #endif
				    FuncDef.FUNCTION("upper",1,0,0,upperFunc),
				    FuncDef.FUNCTION("lower",1,0,0,lowerFunc),
				    FuncDef.FUNCTION("coalesce",1,0,0,null),
				    FuncDef.FUNCTION("coalesce",0,0,0,null),
				///
				///<summary>
				///</summary>
				///<param name="FUNCTION(coalesce,          ">1, 0, 0, ifnullFunc       ), </param>
				// use versionFunc here just for a dummy placeholder
				new FuncDef(-1,SqliteEncoding.UTF8,FuncFlags.SQLITE_FUNC_COALESCE,null,null,versionFunc,null,null,"coalesce",null,null),
				FuncDef.FUNCTION("hex",1,0,0,hexFunc),
				///
				///<summary>
				///FUNCTION(ifnull,             2, 0, 0, ifnullFunc       ), 
				///</summary>
				// use versionFunc here just for a dummy placeholder
				new FuncDef(2,SqliteEncoding.UTF8,FuncFlags.SQLITE_FUNC_COALESCE,null,null,versionFunc,null,null,"ifnull",null,null),
				FuncDef.FUNCTION("random",0,0,0,randomFunc),
				FuncDef.FUNCTION("randomblob",1,0,0,randomBlob),
				FuncDef.FUNCTION("nullif",2,0,1,nullifFunc),
				FuncDef.FUNCTION("sqlite_version",0,0,0,versionFunc),
				FuncDef.FUNCTION("sqlite_source_id",0,0,0,sourceidFunc),
				FuncDef.FUNCTION("sqlite_log",2,0,0,errlogFunc),
				#if !SQLITE_OMIT_COMPILEOPTION_DIAGS
				FuncDef.FUNCTION("sqlite_compileoption_used",1,0,0,compileoptionusedFunc),
				FuncDef.FUNCTION("sqlite_compileoption_get",1,0,0,compileoptiongetFunc),
				#endif
				FuncDef.FUNCTION("quote",1,0,0,quoteFunc),
				FuncDef.FUNCTION("last_insert_rowid",0,0,0,last_insert_rowid),
				FuncDef.FUNCTION("changes",0,0,0,changes),
				FuncDef.FUNCTION("total_changes",0,0,0,total_changes),
				FuncDef.FUNCTION("replace",3,0,0,replaceFunc),
				FuncDef.FUNCTION("zeroblob",1,0,0,zeroblobFunc),
				#if SQLITE_SOUNDEX
																																																																																				FUNCTION("soundex",            1, 0, 0, soundexFunc      ),
#endif
				#if !SQLITE_OMIT_LOAD_EXTENSION
				FuncDef.FUNCTION("load_extension",1,0,0,loadExt),
				FuncDef.FUNCTION("load_extension",2,0,0,loadExt),
				#endif
				FuncDef.AGGREGATE("sum",1,0,0,sumStep,sumFinalize),
				FuncDef.AGGREGATE("total",1,0,0,sumStep,totalFinalize),
				FuncDef.AGGREGATE("avg",1,0,0,sumStep,avgFinalize),
				///
				///<summary>
				///AGGREGATE("count",             0, 0, 0, countStep,       countFinalize  ), 
				///</summary>
				///
				///<summary>
				///AGGREGATE(count,             0, 0, 0, countStep,       countFinalize  ), 
				///</summary>
				new FuncDef(0,SqliteEncoding.UTF8,FuncFlags.SQLITE_FUNC_COUNT,null,null,null,countStep,countFinalize,"count",null,null),
				FuncDef.AGGREGATE("count",1,0,0,countStep,countFinalize),
				FuncDef.AGGREGATE("group_concat",1,0,0,groupConcatStep,groupConcatFinalize),
				FuncDef.AGGREGATE("group_concat",2,0,0,groupConcatStep,groupConcatFinalize),
				FuncDef.LIKEFUNC("glob",2,globInfo,FuncFlags.SQLITE_FUNC_LIKE|FuncFlags.SQLITE_FUNC_CASE),
				#if SQLITE_CASE_SENSITIVE_LIKE
																																																																																				LIKEFUNC("like", 2, likeInfoAlt, FuncFlags.SQLITE_FUNC_LIKE|FuncFlags.SQLITE_FUNC_CASE),
LIKEFUNC("like", 3, likeInfoAlt, FuncFlags.SQLITE_FUNC_LIKE|FuncFlags.SQLITE_FUNC_CASE),
#else
				FuncDef.LIKEFUNC("like",2,likeInfoNorm,FuncFlags.SQLITE_FUNC_LIKE),
				FuncDef.LIKEFUNC("like",3,likeInfoNorm,FuncFlags.SQLITE_FUNC_LIKE),
				#endif
				FuncDef.FUNCTION("regexp",2,0,0,_Custom.regexpFunc),
			};
                int i;
#if SQLITE_OMIT_WSD
																																																															FuncDefHash pHash = GLOBAL( FuncDefHash, sqlite3GlobalFunctions );
FuncDef[] aFunc = (FuncDef[])GLOBAL( FuncDef, aBuiltinFunc );
#else
                FuncDefHash pHash = sqlite3GlobalFunctions;
                FuncDef[] aFunc = aBuiltinFunc;
#endif
                for (i = 0; i < Sqlite3.ArraySize(aBuiltinFunc); i++)
                {
                    sqlite3FuncDefInsert(pHash, aFunc[i]);
                }
                DateUtils.sqlite3RegisterDateTimeFunctions();
#if !SQLITE_OMIT_ALTERTABLE
                alter.sqlite3AlterFunctions();
#endif
            }
        }
    }

    ///<summary>
    /// An instance of the following structure holds the context of a
    /// sum() or avg() aggregate computation.
    ///</summary>
    //typedef struct SumCtx SumCtx;
    public class SumCtx
    {
        public double rSum;
        ///
        ///<summary>
        ///Floating point sum 
        ///</summary>
        public i64 iSum;
        ///
        ///<summary>
        ///Integer sum 
        ///</summary>
        public i64 cnt;
        ///
        ///<summary>
        ///Number of elements summed 
        ///</summary>
        public int overflow;
        ///
        ///<summary>
        ///True if integer overflow seen 
        ///</summary>
        public bool approx;
        ///
        ///<summary>
        ///</summary>
        ///<param name="True if non">integer value was input to the sum </param>
        public Community.CsharpSqlite.Sqlite3.Mem _M;
        public Community.CsharpSqlite.Sqlite3.Mem Context
        {
            get
            {
                return _M;
            }
            set
            {
                _M = value;
                if (_M == null || _M.z == null)
                    iSum = 0;
                else
                    iSum = Convert.ToInt64(_M.z);
            }
        }
    };

}
