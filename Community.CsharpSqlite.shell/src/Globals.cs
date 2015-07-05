using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Community.CsharpSqlite
{
    //SQLITE_API int sqlite3_limit(sqlite3*, int id, int newVal);
    ///
    ///<summary>
    ///</summary>
    ///<param name="CAPI3REF: Run">Time Limit Categories</param>
    ///<param name="KEYWORDS: {limit category} {*limit categories}">KEYWORDS: {limit category} {*limit categories}</param>
    ///<param name=""></param>
    ///<param name="These constants define various performance limits">These constants define various performance limits</param>
    ///<param name="that can be lowered at run">time using [sqlite3_limit()].</param>
    ///<param name="The synopsis of the meanings of the various limits is shown below.">The synopsis of the meanings of the various limits is shown below.</param>
    ///<param name="Additional information is available at [limits | Limits in SQLite].">Additional information is available at [limits | Limits in SQLite].</param>
    ///<param name=""></param>
    ///<param name="<dl>"><dl></param>
    ///<param name="[[SQLITE_LIMIT_LENGTH]] ^(<dt>SQLITE_LIMIT_LENGTH</dt>">[[SQLITE_LIMIT_LENGTH]] ^(<dt>SQLITE_LIMIT_LENGTH</dt></param>
    ///<param name="<dd>The maximum size of any string or BLOB or table row, in bytes.<dd>)^"><dd>The maximum size of any string or BLOB or table row, in bytes.<dd>)^</param>
    ///<param name=""></param>
    ///<param name="[[SQLITE_LIMIT_SQL_LENGTH]] ^(<dt>SQLITE_LIMIT_SQL_LENGTH</dt>">[[SQLITE_LIMIT_SQL_LENGTH]] ^(<dt>SQLITE_LIMIT_SQL_LENGTH</dt></param>
    ///<param name="<dd>The maximum length of an SQL statement, in bytes.</dd>)^"><dd>The maximum length of an SQL statement, in bytes.</dd>)^</param>
    ///<param name=""></param>
    ///<param name="[[SQLITE_LIMIT_COLUMN]] ^(<dt>SQLITE_LIMIT_COLUMN</dt>">[[SQLITE_LIMIT_COLUMN]] ^(<dt>SQLITE_LIMIT_COLUMN</dt></param>
    ///<param name="<dd>The maximum number of columns in a table definition or in the"><dd>The maximum number of columns in a table definition or in the</param>
    ///<param name="result set of a [SELECT] or the maximum number of columns in an index">result set of a [SELECT] or the maximum number of columns in an index</param>
    ///<param name="or in an ORDER BY or GROUP BY clause.</dd>)^">or in an ORDER BY or GROUP BY clause.</dd>)^</param>
    ///<param name=""></param>
    ///<param name="[[SQLITE_LIMIT_EXPR_DEPTH]] ^(<dt>SQLITE_LIMIT_EXPR_DEPTH</dt>">[[SQLITE_LIMIT_EXPR_DEPTH]] ^(<dt>SQLITE_LIMIT_EXPR_DEPTH</dt></param>
    ///<param name="<dd>The maximum depth of the parse tree on any expression.</dd>)^"><dd>The maximum depth of the parse tree on any expression.</dd>)^</param>
    ///<param name=""></param>
    ///<param name="[[SQLITE_LIMIT_COMPOUND_SELECT]] ^(<dt>SQLITE_LIMIT_COMPOUND_SELECT</dt>">[[SQLITE_LIMIT_COMPOUND_SELECT]] ^(<dt>SQLITE_LIMIT_COMPOUND_SELECT</dt></param>
    ///<param name="<dd>The maximum number of terms in a compound SELECT statement.</dd>)^"><dd>The maximum number of terms in a compound SELECT statement.</dd>)^</param>
    ///<param name=""></param>
    ///<param name="[[SQLITE_LIMIT_VDBE_OP]] ^(<dt>SQLITE_LIMIT_VDBE_OP</dt>">[[SQLITE_LIMIT_VDBE_OP]] ^(<dt>SQLITE_LIMIT_VDBE_OP</dt></param>
    ///<param name="<dd>The maximum number of instructions in a virtual machine program"><dd>The maximum number of instructions in a virtual machine program</param>
    ///<param name="used to implement an SQL statement.  This limit is not currently">used to implement an SQL statement.  This limit is not currently</param>
    ///<param name="enforced, though that might be added in some future release of">enforced, though that might be added in some future release of</param>
    ///<param name="SQLite.</dd>)^">SQLite.</dd>)^</param>
    ///<param name=""></param>
    ///<param name="[[SQLITE_LIMIT_FUNCTION_ARG]] ^(<dt>SQLITE_LIMIT_FUNCTION_ARG</dt>">[[SQLITE_LIMIT_FUNCTION_ARG]] ^(<dt>SQLITE_LIMIT_FUNCTION_ARG</dt></param>
    ///<param name="<dd>The maximum number of arguments on a function.</dd>)^"><dd>The maximum number of arguments on a function.</dd>)^</param>
    ///<param name=""></param>
    ///<param name="[[SQLITE_LIMIT_ATTACHED]] ^(<dt>SQLITE_LIMIT_ATTACHED</dt>">[[SQLITE_LIMIT_ATTACHED]] ^(<dt>SQLITE_LIMIT_ATTACHED</dt></param>
    ///<param name="<dd>The maximum number of [ATTACH | attached databases].)^</dd>"><dd>The maximum number of [ATTACH | attached databases].)^</dd></param>
    ///<param name=""></param>
    ///<param name="[[SQLITE_LIMIT_LIKE_PATTERN_LENGTH]]">[[SQLITE_LIMIT_LIKE_PATTERN_LENGTH]]</param>
    ///<param name="^(<dt>SQLITE_LIMIT_LIKE_PATTERN_LENGTH</dt>">^(<dt>SQLITE_LIMIT_LIKE_PATTERN_LENGTH</dt></param>
    ///<param name="<dd>The maximum length of the pattern argument to the [LIKE] or"><dd>The maximum length of the pattern argument to the [LIKE] or</param>
    ///<param name="[GLOB] operators.</dd>)^">[GLOB] operators.</dd>)^</param>
    ///<param name=""></param>
    ///<param name="[[SQLITE_LIMIT_VARIABLE_NUMBER]]">[[SQLITE_LIMIT_VARIABLE_NUMBER]]</param>
    ///<param name="^(<dt>SQLITE_LIMIT_VARIABLE_NUMBER</dt>">^(<dt>SQLITE_LIMIT_VARIABLE_NUMBER</dt></param>
    ///<param name="<dd>The maximum index number of any [parameter] in an SQL statement.)^"><dd>The maximum index number of any [parameter] in an SQL statement.)^</param>
    ///<param name=""></param>
    ///<param name="[[SQLITE_LIMIT_TRIGGER_DEPTH]] ^(<dt>SQLITE_LIMIT_TRIGGER_DEPTH</dt>">[[SQLITE_LIMIT_TRIGGER_DEPTH]] ^(<dt>SQLITE_LIMIT_TRIGGER_DEPTH</dt></param>
    ///<param name="<dd>The maximum depth of recursion for triggers.</dd>)^"><dd>The maximum depth of recursion for triggers.</dd>)^</param>
    ///<param name="</dl>"></dl></param>
    ///<param name=""></param>

    //#define SQLITE_LIMIT_LENGTH                    0
    //#define SQLITE_LIMIT_SQL_LENGTH                1
    //#define SQLITE_LIMIT_COLUMN                    2
    //#define SQLITE_LIMIT_EXPR_DEPTH                3
    //#define SQLITE_LIMIT_COMPOUND_SELECT           4
    //#define SQLITE_LIMIT_VDBE_OP                   5
    //#define SQLITE_LIMIT_FUNCTION_ARG              6
    //#define SQLITE_LIMIT_ATTACHED                  7
    //#define SQLITE_LIMIT_LIKE_PATTERN_LENGTH       8
    //#define SQLITE_LIMIT_VARIABLE_NUMBER           9
    //#define SQLITE_LIMIT_TRIGGER_DEPTH            10

    public class Globals
    {
        public const int SQLITE_LIMIT_LENGTH = 0;

        public const int SQLITE_LIMIT_SQL_LENGTH = 1;

        public const int SQLITE_LIMIT_COLUMN = 2;

        public const int SQLITE_LIMIT_EXPR_DEPTH = 3;

        public const int SQLITE_LIMIT_COMPOUND_SELECT = 4;

        public const int SQLITE_LIMIT_VDBE_OP = 5;

        public const int SQLITE_LIMIT_FUNCTION_ARG = 6;

        public const int SQLITE_LIMIT_ATTACHED = 7;

        public const int SQLITE_LIMIT_LIKE_PATTERN_LENGTH = 8;

        public const int SQLITE_LIMIT_VARIABLE_NUMBER = 9;

        public const int SQLITE_LIMIT_TRIGGER_DEPTH = 10;

        ///
        ///<summary>
        ///</summary>
        ///<param name="The maximum number of in">memory pages to use for the main database</param>
        ///<param name="table and for temporary tables.  The SQLITE_DEFAULT_CACHE_SIZE">table and for temporary tables.  The SQLITE_DEFAULT_CACHE_SIZE</param>

#if !SQLITE_DEFAULT_CACHE_SIZE
        public const int SQLITE_DEFAULT_CACHE_SIZE = 2000;

#endif
    }
}
