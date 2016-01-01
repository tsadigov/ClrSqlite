using System;
using System.Diagnostics;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Text;
using System.Linq;
using Bitmask = System.UInt64;
using i16 = System.Int16;
using i64 = System.Int64;
using sqlite3_int64 = System.Int64;
using u8 = System.Byte;
using u16 = System.UInt16;
using u32 = System.UInt32;
using u64 = System.UInt64;
using unsigned = System.UInt64;
using Pgno = System.UInt32;
#if !SQLITE_MAX_VARIABLE_NUMBER
using ynVar = System.Int16;
#else
using ynVar = System.Int32; 
#endif

namespace Community.CsharpSqlite.Utils
{
    public class IntegerExtensions
    {
        ///
        ///<summary>
        ///</summary>
        ///<param name="Constants for the largest and smallest possible 64">bit signed integers.</param>
        ///<param name="These macros are designed to work correctly on both 32">bit</param>
        ///<param name="compilers.">compilers.</param>
        //#define LARGEST_INT64  (0xffffffff|(((i64)0x7fffffff)<<32))
        //#define SMALLEST_INT64 (((i64)-1) - LARGEST_INT64)
        public const i64 LARGEST_INT64 = i64.MaxValue;
        //( 0xffffffff | ( ( (i64)0x7fffffff ) << 32 ) );
        public const i64 SMALLEST_INT64 = i64.MinValue;
        //( ( ( i64 ) - 1 ) - LARGEST_INT64 );
    }
}
