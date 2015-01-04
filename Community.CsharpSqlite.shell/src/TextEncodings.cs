using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Community.CsharpSqlite
{
    ///<summary>
    /// CAPI3REF: Text Encodings
    ///
    /// These constant define integer codes that represent the various
    /// text encodings supported by SQLite.
    ///</summary>
    //#define SQLITE_UTF8           1
    //#define SQLITE_UTF16LE        2
    //#define SQLITE_UTF16BE        3
    //#define SQLITE_UTF16          4    /* Use native byte order */
    //#define SQLITE_ANY            5    /* sqlite3_create_function only */
    //#define SQLITE_UTF16_ALIGNED  8    /* sqlite3_create_collation only */
    public enum SqliteEncoding : byte
    //u8
    {
        UTF8 = 1,
        UTF16LE = 2,
        UTF16BE = 3,
        UTF16 = 4,
        ANY = 5,
        UTF16_ALIGNED = 8
#if i386 || __i386__ || _M_IX86
																																						
#else
            //TODO: fix this
            //SQLITE_UTF16NATIVE
        ,
        UTF16NATIVE = SqliteEncoding.UTF16LE
        //(Sqlite3.SQLITE_BIGENDIAN != 0 ? SqliteEncoding.UTF16BE : SqliteEncoding.UTF16LE)
        //#define SQLITE_UTF16NATIVE (SQLITE_BIGENDIAN?SqliteEncoding.UTF16BE:SqliteEncoding.UTF16LE)
#endif
    }
}
