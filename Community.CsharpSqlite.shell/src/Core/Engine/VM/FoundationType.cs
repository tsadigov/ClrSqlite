using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Community.CsharpSqlite.Engine
{
    ///
    ///<summary>
    ///CAPI3REF: Fundamental Datatypes
    ///KEYWORDS: SQLITE_TEXT
    ///
    ///^(Every value in SQLite has one of five fundamental datatypes:
    ///
    ///<ul>
    ///</summary>
    ///<param name="<li> 64">bit signed integer</param>
    ///<param name="<li> 64">bit IEEE floating point number</param>
    ///<param name="<li> string"><li> string</param>
    ///<param name="<li> BLOB"><li> BLOB</param>
    ///<param name="<li> NULL"><li> NULL</param>
    ///<param name="</ul>)^"></ul>)^</param>
    ///<param name=""></param>
    ///<param name="These constants are codes for each of those types.">These constants are codes for each of those types.</param>
    ///<param name=""></param>
    ///<param name="Note that the SQLITE_TEXT constant was also used in SQLite version 2">Note that the SQLITE_TEXT constant was also used in SQLite version 2</param>
    ///<param name="for a completely different meaning.  Software that links against both">for a completely different meaning.  Software that links against both</param>
    ///<param name="SQLite version 2 and SQLite version 3 should use SQLITE3_TEXT, not">SQLite version 2 and SQLite version 3 should use SQLITE3_TEXT, not</param>
    ///<param name="SQLITE_TEXT.">SQLITE_TEXT.</param>
    ///<param name=""></param>
    //#define SQLITE_INTEGER  1
    //#define SQLITE_FLOAT    2
    //#define SQLITE_BLOB     4
    //#define SQLITE_NULL     5
    //#if SQLITE_TEXT
    //# undef SQLITE_TEXT
    //#else
    //# define SQLITE_TEXT     3
    //#endif
    //#define SQLITE3_TEXT     3
    public enum FoundationalType : byte
    {
        SQLITE_INTEGER = 1,
        SQLITE_FLOAT = 2,
        SQLITE_BLOB = 4,
        SQLITE_NULL = 5,
        SQLITE_TEXT = 3,
        SQLITE3_TEXT = 3
    };
}
