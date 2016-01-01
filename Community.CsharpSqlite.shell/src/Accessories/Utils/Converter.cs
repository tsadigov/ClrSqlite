using System;
using System.Diagnostics;
using System.Text;
using i64 = System.Int64;
using u8 = System.Byte;
using u32 = System.UInt32;
using u64 = System.UInt64;
using Pgno = System.UInt32;
using Community.CsharpSqlite;
using sqlite_int64 = System.Int64;


namespace Community.CsharpSqlite.Utils
{
    public static class Converter
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
        public static bool sqlite3GetInt32(string zNum, ref int pValue)
        {
            return sqlite3GetInt32(zNum, 0, ref pValue);
        }

        public static bool sqlite3GetInt32(string zNum, int iZnum, ref int pValue)
        {
            sqlite_int64 v = 0;
            int i, c;
            int neg = 0;
            if (zNum[iZnum] == '-')
            {
                neg = 1;
                iZnum++;
            }
            else
                if (zNum[iZnum] == '+')
            {
                iZnum++;
            }
            while (iZnum < zNum.Length && zNum[iZnum] == '0')
                iZnum++;
            for (i = 0; i < 11 && i + iZnum < zNum.Length && (c = zNum[iZnum + i] - '0') >= 0 && c <= 9; i++)
            {
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

            sqliteinth.testcase(i == 10);
            if (i > 10)
            {
                return false;
            }
            sqliteinth.testcase(v - neg == 2147483647);
            if (v - neg > 2147483647)
            {
                return false;
            }
            if (neg != 0)
            {
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
        public static int sqlite3Atoi(string z)
        {
            int x = 0;
            if (!String.IsNullOrEmpty(z))
                sqlite3GetInt32(z, ref x);
            return x;
        }

        ///<summary>
        /// Read or write a four-byte big-endian integer value.
        ///
        ///</summary>
        public static u32 sqlite3Get4byte(u8[] p, int p_offset, int offset)
        {
            offset += p_offset;
            return (offset + 3 > p.Length) ? 0 : (u32)((p[0 + offset] << 24) | (p[1 + offset] << 16) | (p[2 + offset] << 8) | p[3 + offset]);
        }

        public static u32 sqlite3Get4byte(u8[] p, int offset)
        {
            return (offset + 3 > p.Length) ? 0 : (u32)((p[0 + offset] << 24) | (p[1 + offset] << 16) | (p[2 + offset] << 8) | p[3 + offset]);
        }

        public static u32 sqlite3Get4byte(u8[] p, u32 offset)
        {
            return (offset + 3 > p.Length) ? 0 : (u32)((p[0 + offset] << 24) | (p[1 + offset] << 16) | (p[2 + offset] << 8) | p[3 + offset]);
        }

        public static u32 sqlite3Get4byte(u8[] p)
        {
            return (u32)((p[0] << 24) | (p[1] << 16) | (p[2] << 8) | p[3]);
        }

        public static void sqlite3Put4byte(byte[] p, int v)
        {
            p[0] = (byte)(v >> 24 & 0xFF);
            p[1] = (byte)(v >> 16 & 0xFF);
            p[2] = (byte)(v >> 8 & 0xFF);
            p[3] = (byte)(v & 0xFF);
        }

        public static void sqlite3Put4byte(byte[] p, int offset, int v)
        {
            p[0 + offset] = (byte)(v >> 24 & 0xFF);
            p[1 + offset] = (byte)(v >> 16 & 0xFF);
            p[2 + offset] = (byte)(v >> 8 & 0xFF);
            p[3 + offset] = (byte)(v & 0xFF);
        }

        public static void sqlite3Put4byte(byte[] p, u32 offset, u32 v)
        {
            p[0 + offset] = (byte)(v >> 24 & 0xFF);
            p[1 + offset] = (byte)(v >> 16 & 0xFF);
            p[2 + offset] = (byte)(v >> 8 & 0xFF);
            p[3 + offset] = (byte)(v & 0xFF);
        }

        public static void sqlite3Put4byte(this byte[] p, int offset, u64 v)
        {
            p[0 + offset] = (byte)(v >> 24 & 0xFF);
            p[1 + offset] = (byte)(v >> 16 & 0xFF);
            p[2 + offset] = (byte)(v >> 8 & 0xFF);
            p[3 + offset] = (byte)(v & 0xFF);
        }

        public static void sqlite3Put4byte(byte[] p, u64 v)
        {
            p[0] = (byte)(v >> 24 & 0xFF);
            p[1] = (byte)(v >> 16 & 0xFF);
            p[2] = (byte)(v >> 8 & 0xFF);
            p[3] = (byte)(v & 0xFF);
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
        public static bool sqlite3AtoF(string z, ref double pResult, int length, SqliteEncoding enc)
        {
#if !SQLITE_OMIT_FLOATING_POINT
            if (String.IsNullOrEmpty(z))
            {
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
            while (zDx < length && CharExtensions.sqlite3Isspace(z[zDx]))
                zDx++;
            if (zDx >= length)
                return false;
            ///
            ///<summary>
            ///get sign of significand 
            ///</summary>

            if (z[zDx] == '-')
            {
                sign = -1;
                zDx += incr;
            }
            else
                if (z[zDx] == '+')
            {
                zDx += incr;
            }
            ///
            ///<summary>
            ///skip leading zeroes 
            ///</summary>

            while (zDx < z.Length && z[zDx] == '0')
            {
                zDx += incr;
                nDigits++;
            }
            ///
            ///<summary>
            ///copy max significant digits to significand 
            ///</summary>

            while (zDx < length && CharExtensions.sqlite3Isdigit(z[zDx]) && s < ((IntegerExtensions.LARGEST_INT64 - 9) / 10))
            {
                s = s * 10 + (z[zDx] - '0');
                zDx += incr;
                nDigits++;
            }
            ///
            ///<summary>
            ///</summary>
            ///<param name="skip non">significant significand digits</param>
            ///<param name="(increase exponent by d to shift decimal left) ">(increase exponent by d to shift decimal left) </param>

            while (zDx < length && CharExtensions.sqlite3Isdigit(z[zDx]))
            {
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

            if (z[zDx] == '.')
            {
                zDx += incr;
                ///
                ///<summary>
                ///copy digits from after decimal to significand
                ///(decrease exponent by d to shift decimal right) 
                ///</summary>

                while (zDx < length && CharExtensions.sqlite3Isdigit(z[zDx]) && s < ((IntegerExtensions.LARGEST_INT64 - 9) / 10))
                {
                    s = s * 10 + (z[zDx] - '0');
                    zDx += incr;
                    nDigits++;
                    d--;
                }
                ///
                ///<summary>
                ///</summary>
                ///<param name="skip non">significant digits </param>

                while (zDx < length && CharExtensions.sqlite3Isdigit(z[zDx]))
                {
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

            if (z[zDx] == 'e' || z[zDx] == 'E')
            {
                zDx += incr;
                eValid = 0;
                if (zDx >= length)
                    goto do_atof_calc;
                ///
                ///<summary>
                ///get sign of exponent 
                ///</summary>

                if (z[zDx] == '-')
                {
                    esign = -1;
                    zDx += incr;
                }
                else
                    if (z[zDx] == '+')
                {
                    zDx += incr;
                }
                ///
                ///<summary>
                ///copy digits to exponent 
                ///</summary>

                while (zDx < length && CharExtensions.sqlite3Isdigit(z[zDx]))
                {
                    e = e * 10 + (z[zDx] - '0');
                    zDx += incr;
                    eValid = 1;
                }
            }
            ///
            ///<summary>
            ///skip trailing spaces 
            ///</summary>

            if (nDigits > 0 && eValid > 0)
            {
                while (zDx < length && CharExtensions.sqlite3Isspace(z[zDx]))
                    zDx += incr;
            }
            do_atof_calc:
            ///
            ///<summary>
            ///adjust exponent by d, and update sign 
            ///</summary>

            e = (e * esign) + d;
            if (e < 0)
            {
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

            if (0 == s)
            {
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

                if (esign > 0)
                {
                    while (s < (IntegerExtensions.LARGEST_INT64 / 10) && e > 0)
                    {
                        e--;
                        s *= 10;
                    }
                }
                else {
                    while (0 == (s % 10) && e > 0)
                    {
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

                if (e != 0)
                {
                    double scale = 1.0;
                    ///
                    ///<summary>
                    ///attempt to handle extremely small/large numbers better 
                    ///</summary>

                    if (e > 307 && e < 342)
                    {
                        while ((e % 308) != 0)
                        {
                            scale *= 1.0e+1;
                            e -= 1;
                        }
                        if (esign < 0)
                        {
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

                        while ((e % 22) != 0)
                        {
                            scale *= 1.0e+1;
                            e -= 1;
                        }
                        while (e > 0)
                        {
                            scale *= 1.0e+22;
                            e -= 22;
                        }
                        if (esign < 0)
                        {
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
        public static int sqlite3Atoi64(string zNum, ref i64 pNum, int length, SqliteEncoding enc)
        {
            if (zNum == null)
            {
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
            while (zDx < length && CharExtensions.sqlite3Isspace(zNum[zDx]))
                zDx += incr;
            if (zDx < length)
            {
                if (zNum[zDx] == '-')
                {
                    neg = 1;
                    zDx += incr;
                }
                else
                    if (zNum[zDx] == '+')
                {
                    zDx += incr;
                }
            }
            //zStart = zNum;
            if (length > zNum.Length)
                length = zNum.Length;
            while (zDx < length - 1 && zNum[zDx] == '0')
            {
                zDx += incr;
            }
            ///
            ///<summary>
            ///Skip leading zeros. 
            ///</summary>

            for (i = zDx; i < length && (c = zNum[i]) >= '0' && c <= '9'; i += incr)
            {
                u = u * 10 + (u64)(c - '0');
            }
            if (u > IntegerExtensions.LARGEST_INT64)
            {
                pNum = IntegerExtensions.SMALLEST_INT64;
            }
            else
                if (neg != 0)
            {
                pNum = -(i64)u;
            }
            else {
                pNum = (i64)u;
            }
            sqliteinth.testcase(i - zDx == 18);
            sqliteinth.testcase(i - zDx == 19);
            sqliteinth.testcase(i - zDx == 20);
            if ((c != 0 && i < length) || i == zDx || i - zDx > 19 * incr)
            {
                ///
                ///<summary>
                ///</summary>
                ///<param name="zNum is empty or contains non">numeric text or is longer</param>
                ///<param name="than 19 digits (thus guaranteeing that it is too large) ">than 19 digits (thus guaranteeing that it is too large) </param>

                return 1;
            }
            else
                if (i - zDx < 19 * incr)
            {
                ///
                ///<summary>
                ///Less than 19 digits, so we know that it fits in 64 bits 
                ///</summary>

                Debug.Assert(u <= IntegerExtensions.LARGEST_INT64);
                return 0;
            }
            else {
                ///
                ///<summary>
                ///</summary>
                ///<param name="zNum is a 19">digit numbers.  Compare it against 9223372036854775808. </param>

                c = utilc.compare2pow63(zNum.Substring(zDx), incr);
                if (c < 0)
                {
                    ///
                    ///<summary>
                    ///zNum is less than 9223372036854775808 so it fits 
                    ///</summary>

                    Debug.Assert(u <= IntegerExtensions.LARGEST_INT64);
                    return 0;
                }
                else
                    if (c > 0)
                {
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

                    Debug.Assert(u - 1 == IntegerExtensions.LARGEST_INT64);
                    Debug.Assert((pNum) == IntegerExtensions.SMALLEST_INT64);
                    return neg != 0 ? 0 : 2;
                }
            }
        }

        ///<summary>
        /// Translate a single byte of Hex into an integer.
        /// This routine only works if h really is a valid hexadecimal
        /// character:  0..9a..fA..F
        ///</summary>
        public static int sqlite3HexToInt(int h)
        {
            Debug.Assert((h >= '0' && h <= '9') || (h >= 'a' && h <= 'f') || (h >= 'A' && h <= 'F'));
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
        public static byte[] sqlite3HexToBlob(Connection db, string z, int n)
        {
            StringBuilder zBlob;
            int i;
            zBlob = new StringBuilder(n / 2 + 1);
            // (char)sqlite3DbMallocRaw(db, n / 2 + 1);
            n--;
            if (zBlob != null)
            {
                for (i = 0; i < n; i += 2)
                {
                    zBlob.Append(Convert.ToChar((sqlite3HexToInt(z[i]) << 4) | sqlite3HexToInt(z[i + 1])));
                }
                //zBlob[i / 2] = '\0'; ;
            }
            return Encoding.UTF8.GetBytes(zBlob.ToString());
        }

#endif
        static bool testcase(bool b)
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
