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
using Community.CsharpSqlite.Utils;

namespace Community.CsharpSqlite.Utils
{
    using Metadata;
    using System.Globalization;
    using System.Collections.Generic;
    using Parse = Sqlite3.ParseState;
    using Community.CsharpSqlite.Os;

    public static class MathExtensions
    {
        public static ThreeState Negate(this ThreeState state) {
            return (ThreeState)(-(int)state);
        }
        public static bool Intersects(this SortOrder a, SortOrder b)
        {
            return (a & b) != (SortOrder)0;
        }
        public static bool HasProperty(this byte val, byte flag)
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
            return (byte)(val | flag);
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


        public static void testcase(object o)
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

        public static bool sqlite3IsNaN(double x)
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
            sqliteinth.testcase(rc);
            return rc;
        }
#endif
    }

}
