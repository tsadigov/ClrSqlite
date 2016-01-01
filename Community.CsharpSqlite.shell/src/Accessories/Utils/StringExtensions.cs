using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Community.CsharpSqlite.Utils
{
    public static partial class StringExtensions
    {

        private static _Custom.SQLite3UpperToLower UpperToLower
        {
            get
            {
                return _Custom.UpperToLower;
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
        public static int sqlite3StrNICmp(string zLeft, int offsetLeft, string zRight, int N)
        {
            //register unsigned char *a, *b;
            //a = (unsigned char )zLeft;
            //b = (unsigned char )zRight;
            int a = 0, b = 0;
            while (N-- > 0 && a < zLeft.Length - offsetLeft && b < zRight.Length && zLeft[a + offsetLeft] != 0 && UpperToLower[zLeft[a + offsetLeft]] == UpperToLower[zRight[b]])
            {
                a++;
                b++;
            }
            return N < 0 ? 0 : ((a < zLeft.Length - offsetLeft) ? UpperToLower[zLeft[a + offsetLeft]] : 0) - UpperToLower[zRight[b]];
        }

        public static int sqlite3StrNICmp(string zLeft, string zRight, int N)
        {
            //register unsigned char *a, *b;
            //a = (unsigned char )zLeft;
            //b = (unsigned char )zRight;
            int a = 0, b = 0;
            while (N-- > 0 && a < zLeft.Length && b < zRight.Length && (zLeft[a] == zRight[b] || (zLeft[a] != 0 && zLeft[a] < 256 && zRight[b] < 256 && UpperToLower[zLeft[a]] == UpperToLower[zRight[b]])))
            {
                a++;
                b++;
            }
            if (N < 0)
                return 0;
            if (a == zLeft.Length && b == zRight.Length)
                return 0;
            if (a == zLeft.Length)
                return -UpperToLower[zRight[b]];
            if (b == zRight.Length)
                return UpperToLower[zLeft[a]];
            return (zLeft[a] < 256 ? UpperToLower[zLeft[a]] : zLeft[a]) - (zRight[b] < 256 ? UpperToLower[zRight[b]] : zRight[b]);
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
        public static int sqlite3Dequote(ref string z)
        {
            char quote;
            int i;
            if (z == null || z == "")
                return -1;
            quote = z[0];
            switch (quote)
            {
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
            StringBuilder sbZ = new StringBuilder(z.Length);
            for (i = 1; i < z.Length; i++)//z[i] != 0; i++)
            {
                if (z[i] == quote)
                {
                    if (i < z.Length - 1 && (z[i + 1] == quote))
                    {
                        sbZ.Append(quote);
                        i++;
                    }
                    else {
                        break;
                    }
                }
                else {
                    sbZ.Append(z[i]);
                }
            }
            z = sbZ.ToString();
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
        public static int sqlite3Strlen30(int z)
        {
            return 0x3fffffff & z;
        }

        public static int sqlite3Strlen30(StringBuilder z)
        {
            //string z2 = z;
            if (z == null)
                return 0;
            //while( *z2 ){ z2++; }
            //return 0x3fffffff & (int)(z2 - z);
            int iLen = z.ToString().IndexOf('\0');
            return 0x3fffffff & (iLen == -1 ? z.Length : iLen);
        }

        public static int Strlen30(this string z)
        {
            //string z2 = z;
            if (z == null)
                return 0;
            //while( *z2 ){ z2++; }
            //return 0x3fffffff & (int)(z2 - z);
            int iLen = z.IndexOf('\0');
            return 0x3fffffff & (iLen == -1 ? z.Length : iLen);
        }
    }

}
