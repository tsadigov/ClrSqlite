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

namespace Community.CsharpSqlite
{
	

    public partial class CharExtensions
    {
        static byte[] sqlite3CtypeMap
        {
            get
            {
                return Sqlite3.sqlite3CtypeMap;
            }
        }
        ///
        ///<summary>
        ///FTS4 is really an extension for FTS3.  It is enabled using the
        ///SQLITE_ENABLE_FTS3 macro.  But to avoid confusion we also all
        ///the SQLITE_ENABLE_FTS4 macro to serve as an alisse for SQLITE_ENABLE_FTS3.
        ///</summary>
        //#if (SQLITE_ENABLE_FTS4) && !defined(SQLITE_ENABLE_FTS3)
        //# define SQLITE_ENABLE_FTS3
        //#endif
        ///
        ///<summary>
        ///</summary>
        ///<param name="The ctype.h header is needed for non">ASCII systems.  It is also</param>
        ///<param name="needed by FTS3 when FTS3 is included in the amalgamation.">needed by FTS3 when FTS3 is included in the amalgamation.</param>
        ///<param name=""></param>
        //#if !defined(SQLITE_ASCII) || \
        //    (defined(SQLITE_ENABLE_FTS3) && defined(SQLITE_AMALGAMATION))
        //# include <ctype.h>
        //#endif
        ///
        ///<summary>
        ///The following macros mimic the standard library functions toupper(),
        ///isspace(), isalnum(), isdigit() and isxdigit(), respectively. The
        ///sqlite versions only work for ASCII characters, regardless of locale.
        ///
        ///</summary>
#if SQLITE_ASCII
        //# define sqlite3Toupper(x)  ((x)&~(sqlite3CtypeMap[(unsigned char)(x)]&0x20))
        //# define CharExtensions.sqlite3Isspace(x)   (sqlite3CtypeMap[(unsigned char)(x)]&0x01)
        //private 
        public static bool sqlite3Isspace(byte x)
        {
            return (Sqlite3.sqlite3CtypeMap[(byte)(x)] & 0x01) != 0;
        }
        //private
        public static bool sqlite3Isspace(char x)
        {
            return x < 256 && (sqlite3CtypeMap[(byte)(x)] & 0x01) != 0;
        }
        //# define sqlite3Isalnum(x)   (sqlite3CtypeMap[(unsigned char)(x)]&0x06)
        public static bool sqlite3Isalnum(byte x)
        {
            return (sqlite3CtypeMap[(byte)(x)] & 0x06) != 0;
        }
        public static bool sqlite3Isalnum(char x)
        {
            return x < 256 && (sqlite3CtypeMap[(byte)(x)] & 0x06) != 0;
        }
        //# define sqlite3Isalpha(x)   (sqlite3CtypeMap[(unsigned char)(x)]&0x02)
        //# define sqlite3Isdigit(x)   (sqlite3CtypeMap[(unsigned char)(x)]&0x04)
        public static bool sqlite3Isdigit(byte x)
        {
            return (sqlite3CtypeMap[((byte)x)] & 0x04) != 0;
        }
        public static bool sqlite3Isdigit(char x)
        {
            return x < 256 && (sqlite3CtypeMap[((byte)x)] & 0x04) != 0;
        }
        //# define sqlite3Isxdigit(x)  (sqlite3CtypeMap[(unsigned char)(x)]&0x08)
        public static bool sqlite3Isxdigit(byte x)
        {
            return (sqlite3CtypeMap[((byte)x)] & 0x08) != 0;
        }
        public static bool sqlite3Isxdigit(char x)
        {
            return x < 256 && (sqlite3CtypeMap[((byte)x)] & 0x08) != 0;
        }
        //# define sqlite3Tolower(x)   (_Custom.sqlite3UpperToLower[(unsigned char)(x)])
#else
																														// define sqlite3Toupper(x)   toupper((unsigned char)(x))
// define CharExtensions.sqlite3Isspace(x)   isspace((unsigned char)(x))
// define sqlite3Isalnum(x)   isalnum((unsigned char)(x))
// define sqlite3Isalpha(x)   isalpha((unsigned char)(x))
// define sqlite3Isdigit(x)   isdigit((unsigned char)(x))
// define sqlite3Isxdigit(x)  isxdigit((unsigned char)(x))
// define sqlite3Tolower(x)   tolower((unsigned char)(x))
#endif
    }
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
