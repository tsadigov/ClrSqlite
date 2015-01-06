using System;
using System.Diagnostics;
using System.Text;
using i64 = System.Int64;
using u8 = System.Byte;
using u32 = System.UInt32;
using u64 = System.UInt64;
using Pgno = System.UInt32;

namespace Community.CsharpSqlite
{
	using sqlite_int64 = System.Int64;
	using System.Globalization;
    using sqliteinth = Sqlite3.sqliteinth;
    using System.Collections.Generic;
    public static partial class StringExtensions
	{

		private static Sqlite3._Custom.SQLite3UpperToLower UpperToLower {
			get {
				return Sqlite3._Custom.UpperToLower;
			}
		}

		///<summary>
		///Convenient short-hand
		///</summary>
		//#define UpperToLower _Custom.sqlite3UpperToLower
		///<summary>
		/// Some systems have stricmp().  Others have strcasecmp().  Because
		/// there is no consistency, we will define our own.
		///
		/// IMPLEMENTATION-OF: R-20522-24639 The sqlite3_strnicmp() API allows
		/// applications and extensions to compare the contents of two buffers
		/// containing UTF-8 strings in a case-independent fashion, using the same
		/// definition of case independence that SQLite uses internally when
		/// comparing identifiers.
		///
		///</summary>
		public static int sqlite3StrNICmp (string zLeft, int offsetLeft, string zRight, int N)
		{
			//register unsigned char *a, *b;
			//a = (unsigned char )zLeft;
			//b = (unsigned char )zRight;
			int a = 0, b = 0;
			while (N-- > 0 && a < zLeft.Length - offsetLeft && b < zRight.Length && zLeft [a + offsetLeft] != 0 && UpperToLower [zLeft [a + offsetLeft]] == UpperToLower [zRight [b]]) {
				a++;
				b++;
			}
			return N < 0 ? 0 : ((a < zLeft.Length - offsetLeft) ? UpperToLower [zLeft [a + offsetLeft]] : 0) - UpperToLower [zRight [b]];
		}

		public static int sqlite3StrNICmp (string zLeft, string zRight, int N)
		{
			//register unsigned char *a, *b;
			//a = (unsigned char )zLeft;
			//b = (unsigned char )zRight;
			int a = 0, b = 0;
			while (N-- > 0 && a < zLeft.Length && b < zRight.Length && (zLeft [a] == zRight [b] || (zLeft [a] != 0 && zLeft [a] < 256 && zRight [b] < 256 && UpperToLower [zLeft [a]] == UpperToLower [zRight [b]]))) {
				a++;
				b++;
			}
			if (N < 0)
				return 0;
			if (a == zLeft.Length && b == zRight.Length)
				return 0;
			if (a == zLeft.Length)
				return -UpperToLower [zRight [b]];
			if (b == zRight.Length)
				return UpperToLower [zLeft [a]];
			return (zLeft [a] < 256 ? UpperToLower [zLeft [a]] : zLeft [a]) - (zRight [b] < 256 ? UpperToLower [zRight [b]] : zRight [b]);
		}

		///<summary>
		/// Convert an SQL-style quoted string into a normal string by removing
		/// the quote characters.  The conversion is done in-place.  If the
		/// input does not begin with a quote character, then this routine
		/// is a no-op.
		///
		/// The input string must be zero-terminated.  A new zero-terminator
		/// is added to the dequoted string.
		///
		/// The return value is -1 if no dequoting occurs or the length of the
		/// dequoted string, exclusive of the zero terminator, if dequoting does
		/// occur.
		///
		/// 2002-Feb-14: This routine is extended to remove MS-Access style
		/// brackets from around identifers.  For example:  "[a-b-c]" becomes
		/// "a-b-c".
		///
		///</summary>
		public static int sqlite3Dequote (ref string z)
		{
			char quote;
			int i;
			if (z == null || z == "")
				return -1;
			quote = z [0];
			switch (quote) {
			case '\'':
				break;
			case '"':
				break;
			case '`':
				break;
			///
///<summary>
///For MySQL compatibility 
///</summary>

			case '[':
				quote = ']';
				break;
			///
///<summary>
///For MS SqlServer compatibility 
///</summary>

			default:
				return -1;
			}
			StringBuilder sbZ = new StringBuilder (z.Length);
			for (i = 1; i < z.Length; i++)//z[i] != 0; i++)
			 {
				if (z [i] == quote) {
					if (i < z.Length - 1 && (z [i + 1] == quote)) {
						sbZ.Append (quote);
						i++;
					}
					else {
						break;
					}
				}
				else {
					sbZ.Append (z [i]);
				}
			}
			z = sbZ.ToString ();
			return sbZ.Length;
		}

		///<summary>
		/// Compute a string length that is limited to what can be stored in
		/// lower 30 bits of a 32-bit signed integer.
		///
		/// The value returned will never be negative.  Nor will it ever be greater
		/// than the actual length of the string.  For very long strings (greater
		/// than 1GiB) the value returned might be less than the true string length.
		///
		///</summary>
		public static int sqlite3Strlen30 (int z)
		{
			return 0x3fffffff & z;
		}

		public static int sqlite3Strlen30 (StringBuilder z)
		{
			//string z2 = z;
			if (z == null)
				return 0;
			//while( *z2 ){ z2++; }
			//return 0x3fffffff & (int)(z2 - z);
			int iLen = z.ToString ().IndexOf ('\0');
			return 0x3fffffff & (iLen == -1 ? z.Length : iLen);
		}

		public static int sqlite3Strlen30 (this string z)
		{
			//string z2 = z;
			if (z == null)
				return 0;
			//while( *z2 ){ z2++; }
			//return 0x3fffffff & (int)(z2 - z);
			int iLen = z.IndexOf ('\0');
			return 0x3fffffff & (iLen == -1 ? z.Length : iLen);
		}
	}
	public static class MathExtensions
	{

        public static bool Intersects(this SortOrder a, SortOrder b) {
            return (a & b) != (SortOrder)0;
        }
         public static bool HasProperty(this byte val,byte flag)
        {
            return (val & flag) == flag;
        }

         public static bool HasAnyProperty(this TableFlags val, TableFlags flag)
        {
            return (val & flag) != 0;
        }

        public static bool HasAnyProperty(this byte val, byte flag)
        {
            return (val & flag) != 0;
        }
        
        public static byte SetProperty(this byte val, byte flag)
        {
            return (byte)(val|flag);
        }
        public static byte ClearProperty(this byte val, byte flag)
        {
            return (byte)(val & ~flag);
        }

        public static bool Has(this OpFlag val, OpFlag flag)
        {
            return 0 == (val & flag);
        }
        public static T Filter<T>(this T o, T when, T then)
        {
            return (o.Equals(when)) ? then : o;
        }

        public static bool In<T>(this T o, params T[] c)
        {
            List<T> l = new List<T>(c);
            return l.Contains(o);
        }

        /// Round up a number to the next larger multiple of 8.  This is used
        /// to force 8-byte alignment on 64-bit architectures.
        ///
        ///</summary>
        //#define ROUND8(x)     (((x)+7)&~7)
        public static int ROUND8(this int x)
        {
            return (x + 7) & ~7;
        }
        ///<summary>
        /// Round down to the nearest multiple of 8
        ///
        ///</summary>
        //#define ROUNDDOWN8(x) ((x)&~7)
        public static int ROUNDDOWN8(this int x)
        {
            return x & ~7;
        }


        ///<summary>
        ///Prepare a crude estimate of the logarithm of the input value.
        ///The results need not be exact.  This is only used for estimating
        ///the total cost of performing operations with O(logN) or O(NlogN)
        ///complexity.  Because N is just a guess, it is no great tragedy if
        ///logN is a little off.
        ///
        ///</summary>
        public static double estLog(double N)
        {
            double logN = 1;
            double x = 10;
            while (N > x)
            {
                logN += 1;
                x *= 10;
            }
            return logN;
        }



        ///<summary>
        ///Macro to find the minimum of two numeric values.
        ///
        ///</summary>
        //#if !MIN
        //# define MIN(x,y) ((x)<(y)?(x):(y))
        //#endif
        public static int MIN(int x, int y)
        {
            return (x < y) ? x : y;
        }

        public static int MIN(int x, u32 y)
        {
            return (x < y) ? x : (int)y;
        }


		public static void testcase (object o)
		{
		}

		#if !SQLITE_OMIT_FLOATING_POINT
		///
///<summary>
///Return true if the floating point value is Not a Number (NaN).
///
///Use the math library isnan() function if compiled with SQLITE_HAVE_ISNAN.
///Otherwise, we have our own implementation that works on most systems.
///</summary>

		public static bool sqlite3IsNaN (double x)
		{
			bool rc;
			///
///<summary>
///The value return 
///</summary>

			#if !(SQLITE_HAVE_ISNAN)
			///
///<summary>
///Systems that support the isnan() library function should probably
///</summary>
///<param name="make use of it by compiling with ">DSQLITE_HAVE_ISNAN.  But we have</param>
///<param name="found that many systems do not have a working isnan() function so">found that many systems do not have a working isnan() function so</param>
///<param name="this implementation is provided as an alternative.">this implementation is provided as an alternative.</param>
///<param name=""></param>
///<param name="This NaN test sometimes fails if compiled on GCC with ">math.</param>
///<param name="On the other hand, the use of ">math comes with the following</param>
///<param name="warning:">warning:</param>
///<param name=""></param>
///<param name="This option [">math] should never be turned on by any</param>
///<param name="">O option since it can result in incorrect output for programs</param>
///<param name="which depend on an exact implementation of IEEE or ISO">which depend on an exact implementation of IEEE or ISO</param>
///<param name="rules/specifications for math functions.">rules/specifications for math functions.</param>
///<param name=""></param>
///<param name="Under MSVC, this NaN test may fail if compiled with a floating"></param>
///<param name="point precision mode other than /fp:precise.  From the MSDN">point precision mode other than /fp:precise.  From the MSDN</param>
///<param name="documentation:">documentation:</param>
///<param name=""></param>
///<param name="The compiler [with /fp:precise] will properly handle comparisons">The compiler [with /fp:precise] will properly handle comparisons</param>
///<param name="involving NaN. For example, x != x evaluates to true if x is NaN">involving NaN. For example, x != x evaluates to true if x is NaN</param>
///<param name="...">...</param>

			#if __FAST_MATH__
																																																												#error SQLite will not work correctly with the -ffast-math option of GCC.
																																																												#endif
			double y = x;
			double z = y;
			rc = (y != z);
			#else
																																																												rc = isnan(x);
#endif
			sqliteinth.testcase (rc);
			return rc;
		}
	#endif
	}
    public partial class Sqlite3
    {
        public class utilc
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
            ///Utility functions used throughout sqlite.
            ///
            ///This file contains functions for allocating memory, comparing
            ///strings, and stuff like that.
            ///
            ///
            ///</summary>
            ///<param name="Included in SQLite3 port to C#">SQLite;  2008 Noah B Hart</param>
            ///<param name="C#">SQLite is an independent reimplementation of the SQLite software library</param>
            ///<param name=""></param>
            ///<param name="SQLITE_SOURCE_ID: 2011">23 19:49:22 4374b7e83ea0a3fbc3691f9c0c936272862f32f2</param>
            ///<param name=""></param>
            ///<param name=""></param>
            ///<param name=""></param>

            //#include "sqliteInt.h"
            //#include <stdarg.h>
#if SQLITE_HAVE_ISNAN
																																								// include <math.h>
#endif
            ///
            ///<summary>
            ///Routine needed to support the sqliteinth.testcase() macro.
            ///</summary>

#if SQLITE_COVERAGE_TEST
																																								void sqlite3Coverage(int x){
static uint dummy = 0;
dummy += (uint)x;
}
#endif
            ///<summary>
            /// Set the most recent error code and error string for the sqlite
            /// handle "db". The error code is set to "err_code".
            ///
            /// If it is not NULL, string zFormat specifies the format of the
            /// error string in the style of the printf functions: The following
            /// format characters are allowed:
            ///
            ///      %s      Insert a string
            ///      %z      A string that should be freed after use
            ///      %d      Insert an integer
            ///      %T      Insert a token
            ///      %S      Insert the first element of a SrcList
            ///
            /// zFormat and any string tokens that follow it are assumed to be
            /// encoded in UTF-8.
            ///
            /// To clear the most recent error for sqlite handle "db", sqlite3Error
            /// should be called with err_code set to SqlResult.SQLITE_OK and zFormat set
            /// to NULL.
            ///
            ///</summary>
            //Overloads
            public static void sqlite3Error(sqlite3 db, int err_code, int noString)
            {
                utilc.sqlite3Error(db, (SqlResult)err_code, err_code == 0 ? null : "");
            }
            public static void sqlite3Error(sqlite3 db, SqlResult err, int noString)
            {
                utilc.sqlite3Error(db, (int)err, noString);
            }

           
            public static void sqlite3Error(sqlite3 db, SqlResult err_code, string zFormat, params object[] ap)
            {
                if (db != null && (db.pErr != null || (db.pErr = vdbemem_cs.sqlite3ValueNew(db)) != null))
                {
                    db.errCode = err_code;
                    if (zFormat != null)
                    {
                        lock (_Custom.lock_va_list)
                        {
                            string z;
                            _Custom.va_start(ap, zFormat);
                            z = io.sqlite3VMPrintf(db, zFormat, ap);
                            _Custom.va_end(ref ap);
                            vdbemem_cs.sqlite3ValueSetStr(db.pErr, -1, z, SqliteEncoding.UTF8, (dxDel)sqliteinth.SQLITE_DYNAMIC);
                        }
                    }
                    else
                    {
                        vdbemem_cs.sqlite3ValueSetStr(db.pErr, 0, null, SqliteEncoding.UTF8, SQLITE_STATIC);
                    }
                }
            }

            ///<summary>
            /// Add an error message to pParse.zErrMsg and increment pParse.nErr.
            /// The following formatting characters are allowed:
            ///
            ///      %s      Insert a string
            ///      %z      A string that should be freed after use
            ///      %d      Insert an integer
            ///      %T      Insert a token
            ///      %S      Insert the first element of a SrcList
            ///
            /// This function should be used to report any error that occurs whilst
            /// compiling an SQL statement (i.e. within sqlite3_prepare()). The
            /// last thing the sqlite3_prepare() function does is copy the error
            /// stored by this function into the database handle using utilc.sqlite3Error().
            /// Function utilc.sqlite3Error() should be used during statement execution
            /// (sqlite3_step() etc.).
            ///
            ///</summary>
            public static void sqlite3ErrorMsg(Parse pParse, string zFormat, params object[] ap)
            {
                string zMsg;
                sqlite3 db = pParse.db;
                //va_list ap;
                lock (_Custom.lock_va_list)
                {
                    _Custom.va_start(ap, zFormat);
                    zMsg = io.sqlite3VMPrintf(db, zFormat, ap);
                    _Custom.va_end(ref ap);
                }
                if (db.suppressErr != 0)
                {
                    db.sqlite3DbFree(ref zMsg);
                }
                else
                {
                    pParse.nErr++;
                    db.sqlite3DbFree(ref pParse.zErrMsg);
                    pParse.zErrMsg = zMsg;
                    pParse.rc = SqlResult.SQLITE_ERROR;
                }
            }

            ///
            ///<summary>
            ///</summary>
            ///<param name="Compare the 19">character string zNum against the text representation</param>
            ///<param name="value 2^63:  9223372036854775808.  Return negative, zero, or positive">value 2^63:  9223372036854775808.  Return negative, zero, or positive</param>
            ///<param name="if zNum is less than, equal to, or greater than the string.">if zNum is less than, equal to, or greater than the string.</param>
            ///<param name="Note that zNum must contain exactly 19 characters.">Note that zNum must contain exactly 19 characters.</param>
            ///<param name=""></param>
            ///<param name="Unlike memcmp() this routine is guaranteed to return the difference">Unlike memcmp() this routine is guaranteed to return the difference</param>
            ///<param name="in the values of the last digit if the only difference is in the">in the values of the last digit if the only difference is in the</param>
            ///<param name="last digit.  So, for example,">last digit.  So, for example,</param>
            ///<param name=""></param>
            ///<param name="compare2pow63("9223372036854775800", 1)">compare2pow63("9223372036854775800", 1)</param>
            ///<param name=""></param>
            ///<param name="will return ">8.</param>
            ///<param name=""></param>

            public static int compare2pow63(string zNum, int incr)
            {
                int c = 0;
                int i;
                ///
                ///<summary>
                ///012345678901234567 
                ///</summary>

                string pow63 = "922337203685477580";
                for (i = 0; c == 0 && i < 18; i++)
                {
                    c = (zNum[i * incr] - pow63[i]) * 10;
                }
                if (c == 0)
                {
                    c = zNum[18 * incr] - '8';
                    sqliteinth.testcase(c == (-1));
                    sqliteinth.testcase(c == 0);
                    sqliteinth.testcase(c == (+1));
                }
                return c;
            }

            ///<summary>
            /// The variable-length integer encoding is as follows:
            ///
            /// KEY:
            ///         A = 0xxxxxxx    7 bits of data and one flag bit
            ///         B = 1xxxxxxx    7 bits of data and one flag bit
            ///         C = xxxxxxxx    8 bits of data
            ///
            ///  7 bits - A
            /// 14 bits - BA
            /// 21 bits - BBA
            /// 28 bits - BBBA
            /// 35 bits - BBBBA
            /// 42 bits - BBBBBA
            /// 49 bits - BBBBBBA
            /// 56 bits - BBBBBBBA
            /// 64 bits - BBBBBBBBC
            ///
            ///</summary>
            ///<summary>
            /// Write a 64-bit variable-length integer to memory starting at p[0].
            /// The length of data write will be between 1 and 9 bytes.  The number
            /// of bytes written is returned.
            ///
            /// A variable-length integer consists of the lower 7 bits of each byte
            /// for all bytes that have the 8th bit set and one byte with the 8th
            /// bit clear.  Except, if we get to the 9th byte, it stores the full
            /// 8 bits and is the last byte.
            ///
            ///</summary>
            public static int getVarint(byte[] p, out u32 v)
            {
                v = p[0];
                if (v <= 0x7F)
                    return 1;
                u64 u64_v = 0;
                int result = sqlite3GetVarint(p, 0, out u64_v);
                v = (u32)u64_v;
                return result;
            }

            public static int getVarint(byte[] p, int offset, out u32 v)
            {
                v = p[offset + 0];
                if (v <= 0x7F)
                    return 1;
                u64 u64_v = 0;
                int result = sqlite3GetVarint(p, offset, out u64_v);
                v = (u32)u64_v;
                return result;
            }

            public static int getVarint(byte[] p, int offset, out int v)
            {
                v = p[offset + 0];
                if (v <= 0x7F)
                    return 1;
                u64 u64_v = 0;
                int result = sqlite3GetVarint(p, offset, out u64_v);
                v = (int)u64_v;
                return result;
            }

            public static int getVarint(byte[] p, int offset, out i64 v)
            {
                v = offset >= p.Length ? 0 : (int)p[offset + 0];
                if (v <= 0x7F)
                    return 1;
                if (offset + 1 >= p.Length)
                {
                    v = 65535;
                    return 2;
                }
                else
                {
                    u64 u64_v = 0;
                    int result = sqlite3GetVarint(p, offset, out u64_v);
                    v = (i64)u64_v;
                    return result;
                }
            }

            public static int getVarint(byte[] p, int offset, out u64 v)
            {
                v = p[offset + 0];
                if (v <= 0x7F)
                    return 1;
                int result = sqlite3GetVarint(p, offset, out v);
                return result;
            }

            public static int getVarint32(byte[] p, out u32 v)
            {
                //(*B=*(A))<=0x7f?1:sqlite3GetVarint32(A,B))
                v = p[0];
                if (v <= 0x7F)
                    return 1;
                return sqlite3GetVarint32(p, 0, out v);
            }

            static byte[] pByte4 = new byte[4];

            public static int getVarint32(string s, u32 offset, out int v)
            {
                //(*B=*(A))<=0x7f?1:sqlite3GetVarint32(A,B))
                v = s[(int)offset];
                if (v <= 0x7F)
                    return 1;
                pByte4[0] = (u8)s[(int)offset + 0];
                pByte4[1] = (u8)s[(int)offset + 1];
                pByte4[2] = (u8)s[(int)offset + 2];
                pByte4[3] = (u8)s[(int)offset + 3];
                u32 u32_v = 0;
                int result = sqlite3GetVarint32(pByte4, 0, out u32_v);
                v = (int)u32_v;
                return sqlite3GetVarint32(pByte4, 0, out v);
            }

            public static int getVarint32(string s, u32 offset, out u32 v)
            {
                //(*B=*(A))<=0x7f?1:sqlite3GetVarint32(A,B))
                v = s[(int)offset];
                if (v <= 0x7F)
                    return 1;
                pByte4[0] = (u8)s[(int)offset + 0];
                pByte4[1] = (u8)s[(int)offset + 1];
                pByte4[2] = (u8)s[(int)offset + 2];
                pByte4[3] = (u8)s[(int)offset + 3];
                return sqlite3GetVarint32(pByte4, 0, out v);
            }

            public static int getVarint32(byte[] p, u32 offset, out u32 v)
            {
                //(*B=*(A))<=0x7f?1:sqlite3GetVarint32(A,B))
                v = p[offset];
                if (v <= 0x7F)
                    return 1;
                return sqlite3GetVarint32(p, (int)offset, out v);
            }

            public static int getVarint32(byte[] p, int offset, out u32 v)
            {
                //(*B=*(A))<=0x7f?1:sqlite3GetVarint32(A,B))
                v = offset >= p.Length ? 0 : (u32)p[offset];
                if (v <= 0x7F)
                    return 1;
                return sqlite3GetVarint32(p, offset, out v);
            }

            public static int getVarint32(byte[] p, int offset, out int v)
            {
                //(*B=*(A))<=0x7f?1:sqlite3GetVarint32(A,B))
                v = p[offset + 0];
                if (v <= 0x7F)
                    return 1;
                u32 u32_v = 0;
                int result = sqlite3GetVarint32(p, offset, out u32_v);
                v = (int)u32_v;
                return result;
            }

            public static int putVarint(byte[] p, int offset, int v)
            {
                return putVarint(p, offset, (u64)v);
            }

            public static int putVarint(byte[] p, int offset, u64 v)
            {
                return sqlite3PutVarint(p, offset, v);
            }

            static int sqlite3PutVarint(byte[] p, int offset, int v)
            {
                return sqlite3PutVarint(p, offset, (u64)v);
            }

            static u8[] bufByte10 = new u8[10];

            static int sqlite3PutVarint(byte[] p, int offset, u64 v)
            {
                int i, j, n;
                if ((v & (((u64)0xff000000) << 32)) != 0)
                {
                    p[offset + 8] = (byte)v;
                    v >>= 8;
                    for (i = 7; i >= 0; i--)
                    {
                        p[offset + i] = (byte)((v & 0x7f) | 0x80);
                        v >>= 7;
                    }
                    return 9;
                }
                n = 0;
                do
                {
                    bufByte10[n++] = (byte)((v & 0x7f) | 0x80);
                    v >>= 7;
                }
                while (v != 0);
                bufByte10[0] &= 0x7f;
                Debug.Assert(n <= 9);
                for (i = 0, j = n - 1; j >= 0; j--, i++)
                {
                    p[offset + i] = bufByte10[j];
                }
                return n;
            }

            ///<summary>
            /// This routine is a faster version of sqlite3PutVarint() that only
            /// works for 32-bit positive integers and which is optimized for
            /// the common case of small integers.
            ///
            ///</summary>
            public static int putVarint32(byte[] p, int offset, int v)
            {
#if !putVarint32
                if ((v & ~0x7f) == 0)
                {
                    p[offset] = (byte)v;
                    return 1;
                }
#endif
                if ((v & ~0x3fff) == 0)
                {
                    p[offset] = (byte)((v >> 7) | 0x80);
                    p[offset + 1] = (byte)(v & 0x7f);
                    return 2;
                }
                return sqlite3PutVarint(p, offset, v);
            }

            public static int putVarint32(byte[] p, int v)
            {
                if ((v & ~0x7f) == 0)
                {
                    p[0] = (byte)v;
                    return 1;
                }
                else
                    if ((v & ~0x3fff) == 0)
                    {
                        p[0] = (byte)((v >> 7) | 0x80);
                        p[1] = (byte)(v & 0x7f);
                        return 2;
                    }
                    else
                    {
                        return sqlite3PutVarint(p, 0, v);
                    }
            }

            ///<summary>
            /// Bitmasks used by sqlite3GetVarint().  These precomputed constants
            /// are defined here rather than simply putting the constant expressions
            /// inline in order to work around bugs in the RVT compiler.
            ///
            /// SLOT_2_0     A mask for  (0x7f<<14) | 0x7f
            ///
            /// SLOT_4_2_0   A mask for  (0x7f<<28) | SLOT_2_0
            ///
            ///</summary>
            const int SLOT_2_0 = 0x001fc07f;

            //#define SLOT_2_0     0x001fc07f
            const u32 SLOT_4_2_0 = (u32)0xf01fc07f;

            //#define SLOT_4_2_0   0xf01fc07f
            ///<summary>
            /// Read a 64-bit variable-length integer from memory starting at p[0].
            /// Return the number of bytes read.  The value is stored in *v.
            ///
            ///</summary>
            static u8 sqlite3GetVarint(byte[] p, int offset, out u64 v)
            {
                u32 a, b, s;
                a = p[offset + 0];
                ///
                ///<summary>
                ///a: p0 (unmasked) 
                ///</summary>

                if (0 == (a & 0x80))
                {
                    v = a;
                    return 1;
                }
                //p++;
                b = p[offset + 1];
                ///
                ///<summary>
                ///b: p1 (unmasked) 
                ///</summary>

                if (0 == (b & 0x80))
                {
                    a &= 0x7f;
                    a = a << 7;
                    a |= b;
                    v = a;
                    return 2;
                }
                ///
                ///<summary>
                ///Verify that constants are precomputed correctly 
                ///</summary>

                Debug.Assert(SLOT_2_0 == ((0x7f << 14) | (0x7f)));
                Debug.Assert(SLOT_4_2_0 == ((0xfU << 28) | (0x7f << 14) | (0x7f)));
                //p++;
                a = a << 14;
                a |= p[offset + 2];
                ///
                ///<summary>
                ///a: p0<<14 | p2 (unmasked) 
                ///</summary>

                if (0 == (a & 0x80))
                {
                    a &= SLOT_2_0;
                    b &= 0x7f;
                    b = b << 7;
                    a |= b;
                    v = a;
                    return 3;
                }
                ///
                ///<summary>
                ///CSE1 from below 
                ///</summary>

                a &= SLOT_2_0;
                //p++;
                b = b << 14;
                b |= p[offset + 3];
                ///
                ///<summary>
                ///b: p1<<14 | p3 (unmasked) 
                ///</summary>

                if (0 == (b & 0x80))
                {
                    b &= SLOT_2_0;
                    ///
                    ///<summary>
                    ///moved CSE1 up 
                    ///</summary>

                    ///
                    ///<summary>
                    ///a &= (0x7f<<14)|(0x7f); 
                    ///</summary>

                    a = a << 7;
                    a |= b;
                    v = a;
                    return 4;
                }
                ///
                ///<summary>
                ///a: p0<<14 | p2 (masked) 
                ///</summary>

                ///
                ///<summary>
                ///b: p1<<14 | p3 (unmasked) 
                ///</summary>

                ///
                ///<summary>
                ///1:save off p0<<21 | p1<<14 | p2<<7 | p3 (masked) 
                ///</summary>

                ///
                ///<summary>
                ///moved CSE1 up 
                ///</summary>

                ///
                ///<summary>
                ///a &= (0x7f<<14)|(0x7f); 
                ///</summary>

                b &= SLOT_2_0;
                s = a;
                ///
                ///<summary>
                ///s: p0<<14 | p2 (masked) 
                ///</summary>

                //p++;
                a = a << 14;
                a |= p[offset + 4];
                ///
                ///<summary>
                ///a: p0<<28 | p2<<14 | p4 (unmasked) 
                ///</summary>

                if (0 == (a & 0x80))
                {
                    ///
                    ///<summary>
                    ///we can skip these cause they were (effectively) done above in calc'ing s 
                    ///</summary>

                    ///
                    ///<summary>
                    ///a &= (0x1f<<28)|(0x7f<<14)|(0x7f); 
                    ///</summary>

                    ///
                    ///<summary>
                    ///b &= (0x7f<<14)|(0x7f); 
                    ///</summary>

                    b = b << 7;
                    a |= b;
                    s = s >> 18;
                    v = ((u64)s) << 32 | a;
                    return 5;
                }
                ///
                ///<summary>
                ///2:save off p0<<21 | p1<<14 | p2<<7 | p3 (masked) 
                ///</summary>

                s = s << 7;
                s |= b;
                ///
                ///<summary>
                ///s: p0<<21 | p1<<14 | p2<<7 | p3 (masked) 
                ///</summary>

                //p++;
                b = b << 14;
                b |= p[offset + 5];
                ///
                ///<summary>
                ///b: p1<<28 | p3<<14 | p5 (unmasked) 
                ///</summary>

                if (0 == (b & 0x80))
                {
                    ///
                    ///<summary>
                    ///we can skip this cause it was (effectively) done above in calc'ing s 
                    ///</summary>

                    ///
                    ///<summary>
                    ///b &= (0x1f<<28)|(0x7f<<14)|(0x7f); 
                    ///</summary>

                    a &= SLOT_2_0;
                    a = a << 7;
                    a |= b;
                    s = s >> 18;
                    v = ((u64)s) << 32 | a;
                    return 6;
                }
                //p++;
                a = a << 14;
                a |= p[offset + 6];
                ///
                ///<summary>
                ///a: p2<<28 | p4<<14 | p6 (unmasked) 
                ///</summary>

                if (0 == (a & 0x80))
                {
                    a &= SLOT_4_2_0;
                    b &= SLOT_2_0;
                    b = b << 7;
                    a |= b;
                    s = s >> 11;
                    v = ((u64)s) << 32 | a;
                    return 7;
                }
                ///
                ///<summary>
                ///CSE2 from below 
                ///</summary>

                a &= SLOT_2_0;
                //p++;
                b = b << 14;
                b |= p[offset + 7];
                ///
                ///<summary>
                ///b: p3<<28 | p5<<14 | p7 (unmasked) 
                ///</summary>

                if (0 == (b & 0x80))
                {
                    b &= SLOT_4_2_0;
                    ///
                    ///<summary>
                    ///moved CSE2 up 
                    ///</summary>

                    ///
                    ///<summary>
                    ///a &= (0x7f<<14)|(0x7f); 
                    ///</summary>

                    a = a << 7;
                    a |= b;
                    s = s >> 4;
                    v = ((u64)s) << 32 | a;
                    return 8;
                }
                //p++;
                a = a << 15;
                a |= p[offset + 8];
                ///
                ///<summary>
                ///a: p4<<29 | p6<<15 | p8 (unmasked) 
                ///</summary>

                ///
                ///<summary>
                ///moved CSE2 up 
                ///</summary>

                ///
                ///<summary>
                ///a &= (0x7f<<29)|(0x7f<<15)|(0xff); 
                ///</summary>

                b &= SLOT_2_0;
                b = b << 8;
                a |= b;
                s = s << 4;
                b = p[offset + 4];
                b &= 0x7f;
                b = b >> 3;
                s |= b;
                v = ((u64)s) << 32 | a;
                return 9;
            }

            ///<summary>
            /// Read a 32-bit variable-length integer from memory starting at p[0].
            /// Return the number of bytes read.  The value is stored in *v.
            ///
            /// If the varint stored in p[0] is larger than can fit in a 32-bit unsigned
            /// integer, then set *v to 0xffffffff.
            ///
            /// A MACRO version, getVarint32, is provided which inlines the
            /// single-byte case.  All code should use the MACRO version as
            /// this function assumes the single-byte case has already been handled.
            ///
            ///</summary>
            public static u8 sqlite3GetVarint32(byte[] p, out int v)
            {
                u32 u32_v = 0;
                u8 result = sqlite3GetVarint32(p, 0, out u32_v);
                v = (int)u32_v;
                return result;
            }

            public static u8 sqlite3GetVarint32(byte[] p, int offset, out int v)
            {
                u32 u32_v = 0;
                u8 result = sqlite3GetVarint32(p, offset, out u32_v);
                v = (int)u32_v;
                return result;
            }

            public static u8 sqlite3GetVarint32(byte[] p, out u32 v)
            {
                return sqlite3GetVarint32(p, 0, out v);
            }

            public static u8 sqlite3GetVarint32(byte[] p, int offset, out u32 v)
            {
                u32 a, b;
                ///
                ///<summary>
                ///</summary>
                ///<param name="The 1">byte case.  Overwhelmingly the most common.  Handled inline</param>
                ///<param name="by the getVarin32() macro ">by the getVarin32() macro </param>

                a = p[offset + 0];
                ///
                ///<summary>
                ///a: p0 (unmasked) 
                ///</summary>

                //#if getVarint32
                //  if ( 0==( a&0x80))
                //  {
                ///
                ///<summary>
                ///Values between 0 and 127 
                ///</summary>

                //    v = a;
                //    return 1;
                //  }
                //#endif
                ///
                ///<summary>
                ///</summary>
                ///<param name="The 2">byte case </param>

                //p++;
                b = (offset + 1) < p.Length ? p[offset + 1] : (u32)0;
                ///
                ///<summary>
                ///b: p1 (unmasked) 
                ///</summary>

                if (0 == (b & 0x80))
                {
                    ///
                    ///<summary>
                    ///Values between 128 and 16383 
                    ///</summary>

                    a &= 0x7f;
                    a = a << 7;
                    v = a | b;
                    return 2;
                }
                ///
                ///<summary>
                ///</summary>
                ///<param name="The 3">byte case </param>

                //p++;
                a = a << 14;
                a |= (offset + 2) < p.Length ? p[offset + 2] : (u32)0;
                ///
                ///<summary>
                ///a: p0<<14 | p2 (unmasked) 
                ///</summary>

                if (0 == (a & 0x80))
                {
                    ///
                    ///<summary>
                    ///Values between 16384 and 2097151 
                    ///</summary>

                    a &= (0x7f << 14) | (0x7f);
                    b &= 0x7f;
                    b = b << 7;
                    v = a | b;
                    return 3;
                }
                ///
                ///<summary>
                ///</summary>
                ///<param name="A 32">bit varint is used to store size information in btrees.</param>
                ///<param name="Objects are rarely larger than 2MiB limit of a 3">byte varint.</param>
                ///<param name="A 3">byte varint is sufficient, for example, to record the size</param>
                ///<param name="of a 1048569">byte BLOB or string.</param>
                ///<param name=""></param>
                ///<param name="We only unroll the first 1"> byte cases.  The very</param>
                ///<param name="rare larger cases can be handled by the slower 64">bit varint</param>
                ///<param name="routine.">routine.</param>
                ///<param name=""></param>

#if TRUE
                {
                    u64 v64 = 0;
                    u8 n;
                    //p -= 2;
                    n = sqlite3GetVarint(p, offset, out v64);
                    Debug.Assert(n > 3 && n <= 9);
                    if ((v64 & sqliteinth.SQLITE_MAX_U32) != v64)
                    {
                        v = 0xffffffff;
                    }
                    else
                    {
                        v = (u32)v64;
                    }
                    return n;
#else
																																																																															/* For following code (kept for historical record only) shows an
** unrolling for the 3- and 4-byte varint cases.  This code is
** slightly faster, but it is also larger and much harder to test.
*/
//p++;
b = b << 14;
b |= p[offset + 3];
/* b: p1<<14 | p3 (unmasked) */
if ( 0 == ( b & 0x80 ) )
{
/* Values between 2097152 and 268435455 */
b &= ( 0x7f << 14 ) | ( 0x7f );
a &= ( 0x7f << 14 ) | ( 0x7f );
a = a << 7;
v = a | b;
return 4;
}

//p++;
a = a << 14;
a |= p[offset + 4];
/* a: p0<<28 | p2<<14 | p4 (unmasked) */
if ( 0 == ( a & 0x80 ) )
{
/* Values  between 268435456 and 34359738367 */
a &= SLOT_2_0;
b &= SLOT_4_2_0;
b = b << 7;
v = a | b;
return 5;
}

/* We can only reach this point when reading a corrupt database
** file.  In that case we are not in any hurry.  Use the (relatively
** slow) general-purpose sqlite3GetVarint() routine to extract the
** value. */
{
u64 v64 = 0;
int n;

//p -= 4;
n = sqlite3GetVarint( p, offset, out v64 );
Debug.Assert( n > 5 && n <= 9 );
v = (u32)v64;
return n;
}
#endif
                }
            }

            ///<summary>
            /// Return the number of bytes that will be needed to store the given
            /// 64-bit integer.
            ///
            ///</summary>
            public static int sqlite3VarintLen(u64 v)
            {
                int i = 0;
                do
                {
                    i++;
                    v >>= 7;
                }
                while (v != 0 && Sqlite3.ALWAYS(i < 9));
                return i;
            }

            ///<summary>
            /// Log an error that is an API call on a connection pointer that should
            /// not have been used.  The "type" of connection pointer is given as the
            /// argument.  The zType is a word like "NULL" or "closed" or "invalid".
            ///</summary>
            static void logBadConnection(string zType)
            {
                io.sqlite3_log(SqlResult.SQLITE_MISUSE, "API call with %s database connection pointer", zType);
            }

            ///<summary>
            /// Check to make sure we have a valid db pointer.  This test is not
            /// foolproof but it does provide some measure of protection against
            /// misuse of the interface such as passing in db pointers that are
            /// NULL or which have been previously closed.  If this routine returns
            /// 1 it means that the db pointer is valid and 0 if it should not be
            /// dereferenced for any reason.  The calling function should invoke
            /// SQLITE_MISUSE immediately.
            ///
            /// sqlite3SafetyCheckOk() requires that the db pointer be valid for
            /// use.  utilc.sqlite3SafetyCheckSickOrOk() allows a db pointer that failed to
            /// open properly and is not fit for general use but which can be
            /// used as an argument to sqlite3_errmsg() or sqlite3_close().
            ///
            ///</summary>
            public static bool sqlite3SafetyCheckOk(sqlite3 db)
            {
                u32 magic;
                if (db == null)
                {
                    logBadConnection("NULL");
                    return false;
                }
                magic = db.magic;
                if (magic != Sqlite3.SQLITE_MAGIC_OPEN)
                {
                    if (utilc.sqlite3SafetyCheckSickOrOk(db))
                    {
                        sqliteinth.testcase(Sqlite3.sqliteinth.sqlite3GlobalConfig.xLog != null);
                        logBadConnection("unopened");
                    }
                    return false;
                }
                else
                {
                    return true;
                }
            }

            public static bool sqlite3SafetyCheckSickOrOk(sqlite3 db)
            {
                u32 magic;
                magic = db.magic;
                if (magic != Sqlite3.SQLITE_MAGIC_SICK && magic != Sqlite3.SQLITE_MAGIC_OPEN && magic != Sqlite3.SQLITE_MAGIC_BUSY)
                {
                    sqliteinth.testcase(Sqlite3.sqliteinth.sqlite3GlobalConfig.xLog != null);
                    logBadConnection("invalid");
                    return false;
                }
                else
                {
                    return true;
                }
            }

            ///<summary>
            /// Attempt to add, substract, or multiply the 64-bit signed value iB against
            /// the other 64-bit signed integer at *pA and store the result in *pA.
            /// Return 0 on success.  Or if the operation would have resulted in an
            /// overflow, leave *pA unchanged and return 1.
            ///
            ///</summary>
            public static int sqlite3AddInt64(ref i64 pA, i64 iB)
            {
                i64 iA = pA;
                sqliteinth.testcase(iA == 0);
                sqliteinth.testcase(iA == 1);
                sqliteinth.testcase(iB == -1);
                sqliteinth.testcase(iB == 0);
                if (iB >= 0)
                {
                    sqliteinth.testcase(iA > 0 && IntegerExtensions.LARGEST_INT64 - iA == iB);
                    sqliteinth.testcase(iA > 0 && IntegerExtensions.LARGEST_INT64 - iA == iB - 1);
                    if (iA > 0 && IntegerExtensions.LARGEST_INT64 - iA < iB)
                        return 1;
                    pA += iB;
                }
                else
                {
                    sqliteinth.testcase(iA < 0 && -(iA + IntegerExtensions.LARGEST_INT64) == iB + 1);
                    sqliteinth.testcase(iA < 0 && -(iA + IntegerExtensions.LARGEST_INT64) == iB + 2);
                    if (iA < 0 && -(iA + IntegerExtensions.LARGEST_INT64) > iB + 1)
                        return 1;
                    pA += iB;
                }
                return 0;
            }

            public static int sqlite3SubInt64(ref i64 pA, i64 iB)
            {
                sqliteinth.testcase(iB == IntegerExtensions.SMALLEST_INT64 + 1);
                if (iB == IntegerExtensions.SMALLEST_INT64)
                {
                    sqliteinth.testcase((pA) == (-1));
                    sqliteinth.testcase((pA) == 0);
                    if ((pA) >= 0)
                        return 1;
                    pA -= iB;
                    return 0;
                }
                else
                {
                    return sqlite3AddInt64(ref pA, -iB);
                }
            }

            //#define TWOPOWER32 (((i64)1)<<32)
            const i64 TWOPOWER32 = (((i64)1) << 32);

            //#define TWOPOWER31 (((i64)1)<<31)
            const i64 TWOPOWER31 = (((i64)1) << 31);

            public static int sqlite3MulInt64(ref i64 pA, i64 iB)
            {
                i64 iA = pA;
                i64 iA1, iA0, iB1, iB0, r;
                iA1 = iA / TWOPOWER32;
                iA0 = iA % TWOPOWER32;
                iB1 = iB / TWOPOWER32;
                iB0 = iB % TWOPOWER32;
                if (iA1 * iB1 != 0)
                    return 1;
                Debug.Assert(iA1 * iB0 == 0 || iA0 * iB1 == 0);
                r = iA1 * iB0 + iA0 * iB1;
                sqliteinth.testcase(r == (-TWOPOWER31) - 1);
                sqliteinth.testcase(r == (-TWOPOWER31));
                sqliteinth.testcase(r == TWOPOWER31);
                sqliteinth.testcase(r == TWOPOWER31 - 1);
                if (r < (-TWOPOWER31) || r >= TWOPOWER31)
                    return 1;
                r *= TWOPOWER32;
                if (sqlite3AddInt64(ref r, iA0 * iB0) != 0)
                    return 1;
                pA = r;
                return 0;
            }

            ///
            ///<summary>
            ///</summary>
            ///<param name="Compute the absolute value of a 32">bit signed integer, if possible.  Or </param>
            ///<param name="if the integer has a value of ">2147483648, return +2147483647</param>
            ///<param name=""></param>

            public static int sqlite3AbsInt32(int x)
            {
                if (x >= 0)
                    return x;
                if (x == -2147483648)
                    // 0x80000000 
                    return 0x7fffffff;
                return -x;
            }
#if SQLITE_ENABLE_8_3_NAMES
																				/*
** If SQLITE_ENABLE_8_3_NAME is set at compile-time and if the database
** filename in zBaseFilename is a URI with the "8_3_names=1" parameter and
** if filename in z[] has a suffix (a.k.a. "extension") that is longer than
** three characters, then shorten the suffix on z[] to be the last three
** characters of the original suffix.
**
** Examples:
**
**     test.db-journal    =>   test.nal
**     test.db-wal        =>   test.wal
**     test.db-shm        =>   test.shm
*/
static void sqlite3FileSuffix3(string zBaseFilename, string z){
  string zOk;
  zOk = sqlite3_uri_parameter(zBaseFilename, "8_3_names");
  if( zOk != null && sqlite3GetBoolean(zOk) ){
    int i, sz;
    sz = StringExtensions.sqlite3Strlen30(z);
    for(i=sz-1; i>0 && z[i]!='/' && z[i]!='.'; i--){}
    if( z[i]=='.' && Sqlite3.ALWAYS(sz>i+4) ) memcpy(&z[i+1], &z[sz-3], 4);
  }
}
#endif
        }
    }
   //--------------------------------------------------------- 
    public partial class Converter
	{
		///<summary>
		/// If zNum represents an integer that will fit in 32-bits, then set
		/// pValue to that integer and return true.  Otherwise return false.
		///
		/// Any non-numeric characters that following zNum are ignored.
		/// This is different from sqlite3Atoi64() which requires the
		/// input number to be zero-terminated.
		///
		///</summary>
		public static bool sqlite3GetInt32 (string zNum, ref int pValue)
		{
			return sqlite3GetInt32 (zNum, 0, ref pValue);
		}

		public static bool sqlite3GetInt32 (string zNum, int iZnum, ref int pValue)
		{
			sqlite_int64 v = 0;
			int i, c;
			int neg = 0;
			if (zNum [iZnum] == '-') {
				neg = 1;
				iZnum++;
			}
			else
				if (zNum [iZnum] == '+') {
					iZnum++;
				}
			while (iZnum < zNum.Length && zNum [iZnum] == '0')
				iZnum++;
			for (i = 0; i < 11 && i + iZnum < zNum.Length && (c = zNum [iZnum + i] - '0') >= 0 && c <= 9; i++) {
				v = v * 10 + c;
			}
			///
///<summary>
///The longest decimal representation of a 32 bit integer is 10 digits:
///
///1234567890
///2^31 . 2147483648
///
///</summary>

			sqliteinth.testcase (i == 10);
			if (i > 10) {
				return false;
			}
			sqliteinth.testcase (v - neg == 2147483647);
			if (v - neg > 2147483647) {
				return false;
			}
			if (neg != 0) {
				v = -v;
			}
			pValue = (int)v;
			return true;
		}

		///<summary>
		/// Return a 32-bit integer value extracted from a string.  If the
		/// string is not an integer, just return 0.
		///
		///</summary>
		public static int sqlite3Atoi (string z)
		{
			int x = 0;
			if (!String.IsNullOrEmpty (z))
				sqlite3GetInt32 (z, ref x);
			return x;
		}

		///<summary>
		/// Read or write a four-byte big-endian integer value.
		///
		///</summary>
		public static u32 sqlite3Get4byte (u8[] p, int p_offset, int offset)
		{
			offset += p_offset;
			return (offset + 3 > p.Length) ? 0 : (u32)((p [0 + offset] << 24) | (p [1 + offset] << 16) | (p [2 + offset] << 8) | p [3 + offset]);
		}

		public static u32 sqlite3Get4byte (u8[] p, int offset)
		{
			return (offset + 3 > p.Length) ? 0 : (u32)((p [0 + offset] << 24) | (p [1 + offset] << 16) | (p [2 + offset] << 8) | p [3 + offset]);
		}

		public static u32 sqlite3Get4byte (u8[] p, u32 offset)
		{
			return (offset + 3 > p.Length) ? 0 : (u32)((p [0 + offset] << 24) | (p [1 + offset] << 16) | (p [2 + offset] << 8) | p [3 + offset]);
		}

		public static u32 sqlite3Get4byte (u8[] p)
		{
			return (u32)((p [0] << 24) | (p [1] << 16) | (p [2] << 8) | p [3]);
		}

		public static void sqlite3Put4byte (byte[] p, int v)
		{
			p [0] = (byte)(v >> 24 & 0xFF);
			p [1] = (byte)(v >> 16 & 0xFF);
			p [2] = (byte)(v >> 8 & 0xFF);
			p [3] = (byte)(v & 0xFF);
		}

		public static void sqlite3Put4byte (byte[] p, int offset, int v)
		{
			p [0 + offset] = (byte)(v >> 24 & 0xFF);
			p [1 + offset] = (byte)(v >> 16 & 0xFF);
			p [2 + offset] = (byte)(v >> 8 & 0xFF);
			p [3 + offset] = (byte)(v & 0xFF);
		}

		public static void sqlite3Put4byte (byte[] p, u32 offset, u32 v)
		{
			p [0 + offset] = (byte)(v >> 24 & 0xFF);
			p [1 + offset] = (byte)(v >> 16 & 0xFF);
			p [2 + offset] = (byte)(v >> 8 & 0xFF);
			p [3 + offset] = (byte)(v & 0xFF);
		}

		public static void sqlite3Put4byte (byte[] p, int offset, u64 v)
		{
			p [0 + offset] = (byte)(v >> 24 & 0xFF);
			p [1 + offset] = (byte)(v >> 16 & 0xFF);
			p [2 + offset] = (byte)(v >> 8 & 0xFF);
			p [3 + offset] = (byte)(v & 0xFF);
		}

		public static void sqlite3Put4byte (byte[] p, u64 v)
		{
			p [0] = (byte)(v >> 24 & 0xFF);
			p [1] = (byte)(v >> 16 & 0xFF);
			p [2] = (byte)(v >> 8 & 0xFF);
			p [3] = (byte)(v & 0xFF);
		}

		///<summary>
		/// The string z[] is an text representation of a real number.
		/// Convert this string to a double and write it into *pResult.
		///
		/// The string z[] is length bytes in length (bytes, not characters) and
		/// uses the encoding enc.  The string is not necessarily zero-terminated.
		///
		/// Return TRUE if the result is a valid real number (or integer) and FALSE
		/// if the string is empty or contains extraneous text.  Valid numbers
		/// are in one of these formats:
		///
		///    [+-]digits[E[+-]digits]
		///    [+-]digits.[digits][E[+-]digits]
		///    [+-].digits[E[+-]digits]
		///
		/// Leading and trailing whitespace is ignored for the purpose of determining
		/// validity.
		///
		/// If some prefix of the input string is a valid number, this routine
		/// returns FALSE but it still converts the prefix and writes the result
		/// into *pResult.
		///
		///</summary>
		public static bool sqlite3AtoF (string z, ref double pResult, int length, SqliteEncoding enc)
		{
			#if !SQLITE_OMIT_FLOATING_POINT
			if (String.IsNullOrEmpty (z)) {
				pResult = 0;
				return false;
			}
			int incr = (enc == SqliteEncoding.UTF8 ? 1 : 2);
			//const char* zEnd = z + length;
			///
///<summary>
///sign * significand * (10 ^ (esign * exponent)) 
///</summary>

			int sign = 1;
			///
///<summary>
///sign of significand 
///</summary>

			i64 s = 0;
			///
///<summary>
///significand 
///</summary>

			int d = 0;
			///
///<summary>
///adjust exponent for shifting decimal point 
///</summary>

			int esign = 1;
			///
///<summary>
///sign of exponent 
///</summary>

			int e = 0;
			///
///<summary>
///exponent 
///</summary>

			int eValid = 1;
			///
///<summary>
///</summary>
///<param name="True exponent is either not used or is well">formed </param>

			double result = 0;
			int nDigits = 0;
			pResult = 0.0;
			///
///<summary>
///Default return value, in case of an error 
///</summary>

			int zDx = 0;
			if (enc == SqliteEncoding.UTF16BE)
				zDx++;
			while (zDx < length && CharExtensions.sqlite3Isspace (z [zDx]))
				zDx++;
			if (zDx >= length)
				return false;
			///
///<summary>
///get sign of significand 
///</summary>

			if (z [zDx] == '-') {
				sign = -1;
				zDx += incr;
			}
			else
				if (z [zDx] == '+') {
					zDx += incr;
				}
			///
///<summary>
///skip leading zeroes 
///</summary>

			while (zDx < z.Length && z [zDx] == '0') {
				zDx += incr;
				nDigits++;
			}
			///
///<summary>
///copy max significant digits to significand 
///</summary>

			while (zDx < length && CharExtensions.sqlite3Isdigit (z [zDx]) && s < ((IntegerExtensions.LARGEST_INT64 - 9) / 10)) {
				s = s * 10 + (z [zDx] - '0');
				zDx += incr;
				nDigits++;
			}
			///
///<summary>
///</summary>
///<param name="skip non">significant significand digits</param>
///<param name="(increase exponent by d to shift decimal left) ">(increase exponent by d to shift decimal left) </param>

			while (zDx < length && CharExtensions.sqlite3Isdigit (z [zDx])) {
				zDx += incr;
				nDigits++;
				d++;
			}
			if (zDx >= length)
				goto do_atof_calc;
			///
///<summary>
///if decimal point is present 
///</summary>

			if (z [zDx] == '.') {
				zDx += incr;
				///
///<summary>
///copy digits from after decimal to significand
///(decrease exponent by d to shift decimal right) 
///</summary>

				while (zDx < length && CharExtensions.sqlite3Isdigit (z [zDx]) && s < ((IntegerExtensions.LARGEST_INT64 - 9) / 10)) {
					s = s * 10 + (z [zDx] - '0');
					zDx += incr;
					nDigits++;
					d--;
				}
				///
///<summary>
///</summary>
///<param name="skip non">significant digits </param>

				while (zDx < length && CharExtensions.sqlite3Isdigit (z [zDx])) {
					zDx += incr;
					nDigits++;
				}
				if (zDx >= length)
					goto do_atof_calc;
			}
			///
///<summary>
///if exponent is present 
///</summary>

			if (z [zDx] == 'e' || z [zDx] == 'E') {
				zDx += incr;
				eValid = 0;
				if (zDx >= length)
					goto do_atof_calc;
				///
///<summary>
///get sign of exponent 
///</summary>

				if (z [zDx] == '-') {
					esign = -1;
					zDx += incr;
				}
				else
					if (z [zDx] == '+') {
						zDx += incr;
					}
				///
///<summary>
///copy digits to exponent 
///</summary>

				while (zDx < length && CharExtensions.sqlite3Isdigit (z [zDx])) {
					e = e * 10 + (z [zDx] - '0');
					zDx += incr;
					eValid = 1;
				}
			}
			///
///<summary>
///skip trailing spaces 
///</summary>

			if (nDigits > 0 && eValid > 0) {
				while (zDx < length && CharExtensions.sqlite3Isspace (z [zDx]))
					zDx += incr;
			}
			do_atof_calc:
			///
///<summary>
///adjust exponent by d, and update sign 
///</summary>

			e = (e * esign) + d;
			if (e < 0) {
				esign = -1;
				e *= -1;
			}
			else {
				esign = 1;
			}
			///
///<summary>
///if 0 significand 
///</summary>

			if (0 == s) {
				///
///<summary>
///In the IEEE 754 standard, zero is signed.
///Add the sign if we've seen at least one digit 
///</summary>

				result = (sign < 0 && nDigits != 0) ? -(double)0 : (double)0;
			}
			else {
				///
///<summary>
///attempt to reduce exponent 
///</summary>

				if (esign > 0) {
					while (s < (IntegerExtensions.LARGEST_INT64 / 10) && e > 0) {
						e--;
						s *= 10;
					}
				}
				else {
					while (0 == (s % 10) && e > 0) {
						e--;
						s /= 10;
					}
				}
				///
///<summary>
///adjust the sign of significand 
///</summary>

				s = sign < 0 ? -s : s;
				///
///<summary>
///if exponent, scale significand as appropriate
///and store in result. 
///</summary>

				if (e != 0) {
					double scale = 1.0;
					///
///<summary>
///attempt to handle extremely small/large numbers better 
///</summary>

					if (e > 307 && e < 342) {
						while ((e % 308) != 0) {
							scale *= 1.0e+1;
							e -= 1;
						}
						if (esign < 0) {
							result = s / scale;
							result /= 1.0e+308;
						}
						else {
							result = s * scale;
							result *= 1.0e+308;
						}
					}
					else {
						///
///<summary>
///1.0e+22 is the largest power of 10 than can be 
///represented exactly. 
///</summary>

						while ((e % 22) != 0) {
							scale *= 1.0e+1;
							e -= 1;
						}
						while (e > 0) {
							scale *= 1.0e+22;
							e -= 22;
						}
						if (esign < 0) {
							result = s / scale;
						}
						else {
							result = s * scale;
						}
					}
				}
				else {
					result = (double)s;
				}
			}
			///
///<summary>
///store the result 
///</summary>

			pResult = result;
			///
///<summary>
///</summary>
///<param name="return true if number and no extra non">whitespace chracters after </param>

			return zDx >= length && nDigits > 0 && eValid != 0;
			#else
																																																												return !sqlite3Atoi64(z, pResult, length, enc);
#endif
		}

		///<summary>
		/// Convert zNum to a 64-bit signed integer.
		///
		/// If the zNum value is representable as a 64-bit twos-complement
		/// integer, then write that value into *pNum and return 0.
		///
		/// If zNum is exactly 9223372036854665808, return 2.  This special
		/// case is broken out because while 9223372036854665808 cannot be a
		/// signed 64-bit integer, its negative -9223372036854665808 can be.
		///
		/// If zNum is too big for a 64-bit integer and is not
		/// 9223372036854665808 then return 1.
		///
		/// length is the number of bytes in the string (bytes, not characters).
		/// The string is not necessarily zero-terminated.  The encoding is
		/// given by enc.
		///
		///</summary>
		public static int sqlite3Atoi64 (string zNum, ref i64 pNum, int length, SqliteEncoding enc)
		{
			if (zNum == null) {
				pNum = 0;
				return 1;
			}
			int incr = (enc == SqliteEncoding.UTF8 ? 1 : 2);
			u64 u = 0;
			int neg = 0;
			///
///<summary>
///assume positive 
///</summary>

			int i;
			int c = 0;
			int zDx = 0;
			//  string zStart;
			//string zEnd = zNum + length;
			if (enc == SqliteEncoding.UTF16BE)
				zDx++;
			while (zDx < length && CharExtensions.sqlite3Isspace (zNum [zDx]))
				zDx += incr;
			if (zDx < length) {
				if (zNum [zDx] == '-') {
					neg = 1;
					zDx += incr;
				}
				else
					if (zNum [zDx] == '+') {
						zDx += incr;
					}
			}
			//zStart = zNum;
			if (length > zNum.Length)
				length = zNum.Length;
			while (zDx < length - 1 && zNum [zDx] == '0') {
				zDx += incr;
			}
			///
///<summary>
///Skip leading zeros. 
///</summary>

			for (i = zDx; i < length && (c = zNum [i]) >= '0' && c <= '9'; i += incr) {
				u = u * 10 + (u64)(c - '0');
			}
			if (u > IntegerExtensions.LARGEST_INT64) {
				pNum = IntegerExtensions.SMALLEST_INT64;
			}
			else
				if (neg != 0) {
					pNum = -(i64)u;
				}
				else {
					pNum = (i64)u;
				}
			sqliteinth.testcase (i - zDx == 18);
			sqliteinth.testcase (i - zDx == 19);
			sqliteinth.testcase (i - zDx == 20);
			if ((c != 0 && i < length) || i == zDx || i - zDx > 19 * incr) {
				///
///<summary>
///</summary>
///<param name="zNum is empty or contains non">numeric text or is longer</param>
///<param name="than 19 digits (thus guaranteeing that it is too large) ">than 19 digits (thus guaranteeing that it is too large) </param>

				return 1;
			}
			else
				if (i - zDx < 19 * incr) {
					///
///<summary>
///Less than 19 digits, so we know that it fits in 64 bits 
///</summary>

					Debug.Assert (u <= IntegerExtensions.LARGEST_INT64);
					return 0;
				}
				else {
					///
///<summary>
///</summary>
///<param name="zNum is a 19">digit numbers.  Compare it against 9223372036854775808. </param>

					c = Sqlite3.utilc.compare2pow63 (zNum.Substring (zDx), incr);
					if (c < 0) {
						///
///<summary>
///zNum is less than 9223372036854775808 so it fits 
///</summary>

						Debug.Assert (u <= IntegerExtensions.LARGEST_INT64);
						return 0;
					}
					else
						if (c > 0) {
							///
///<summary>
///zNum is greater than 9223372036854775808 so it overflows 
///</summary>

							return 1;
						}
						else {
							///
///<summary>
///zNum is exactly 9223372036854775808.  Fits if negative.  The
///special case 2 overflow if positive 
///</summary>

							Debug.Assert (u - 1 == IntegerExtensions.LARGEST_INT64);
							Debug.Assert ((pNum) == IntegerExtensions.SMALLEST_INT64);
							return neg != 0 ? 0 : 2;
						}
				}
		}

		///<summary>
		/// Translate a single byte of Hex into an integer.
		/// This routine only works if h really is a valid hexadecimal
		/// character:  0..9a..fA..F
		///</summary>
		public static int sqlite3HexToInt (int h)
		{
			Debug.Assert ((h >= '0' && h <= '9') || (h >= 'a' && h <= 'f') || (h >= 'A' && h <= 'F'));
			#if SQLITE_ASCII
			h += 9 * (1 & (h >> 6));
			#endif
			//#if SQLITE_EBCDIC
			//h += 9*(1&~(h>>4));
			//#endif
			return h & 0xf;
		}

		#if !SQLITE_OMIT_BLOB_LITERAL || SQLITE_HAS_CODEC
		///<summary>
		/// Convert a BLOB literal of the form "x'hhhhhh'" into its binary
		/// value.  Return a pointer to its binary value.  Space to hold the
		/// binary value has been obtained from malloc and must be freed by
		/// the calling routine.
		///</summary>
		public static byte[] sqlite3HexToBlob (sqlite3 db, string z, int n)
		{
			StringBuilder zBlob;
			int i;
			zBlob = new StringBuilder (n / 2 + 1);
			// (char)sqlite3DbMallocRaw(db, n / 2 + 1);
			n--;
			if (zBlob != null) {
				for (i = 0; i < n; i += 2) {
					zBlob.Append (Convert.ToChar ((sqlite3HexToInt (z [i]) << 4) | sqlite3HexToInt (z [i + 1])));
				}
				//zBlob[i / 2] = '\0'; ;
			}
			return Encoding.UTF8.GetBytes (zBlob.ToString ());
		}

		#endif
		static bool testcase (bool b)
		{
			return b;
		}



        //from pager
        ///<summary>
        /// Write a 32-bit integer into a string buffer in big-endian byte order.
        ///
        ///</summary>
        //#define Converter.put32bits(A,B)  sqlite3sqlite3Put4byte((u8*)A,B)
        public static void put32bits(string ac, int offset, int val)
        {
            byte[] A = new byte[4];
            A[0] = (byte)ac[offset + 0];
            A[1] = (byte)ac[offset + 1];
            A[2] = (byte)ac[offset + 2];
            A[3] = (byte)ac[offset + 3];
            Converter.sqlite3Put4byte(A, 0, val);
        }

        public static void put32bits(byte[] ac, int offset, int val)
        {
            Converter.sqlite3Put4byte(ac, offset, (u32)val);
        }

        public static void put32bits(byte[] ac, u32 val)
        {
            Converter.sqlite3Put4byte(ac, 0U, val);
        }

        public static void put32bits(byte[] ac, int offset, u32 val)
        {
            Converter.sqlite3Put4byte(ac, offset, val);
        }




	}
}
