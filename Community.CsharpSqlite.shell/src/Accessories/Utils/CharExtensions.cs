using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Community.CsharpSqlite.Utils
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
}
