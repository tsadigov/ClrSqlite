using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Community.CsharpSqlite
{
    public static class Limits
    {
        public static class Pager{
            ///<summary>
            ///Default maximum size for persistent journal files. A negative
            ///value means no limit. This value may be overridden using the
            ///sqlite3PagerJournalSizeLimit() API. See also "PRAGMA journal_size_limit".
            ///
            ///</summary>

#if !SQLITE_DEFAULT_JOURNAL_SIZE_LIMIT
            public const int SQLITE_DEFAULT_JOURNAL_SIZE_LIMIT = -1;

            //#define SQLITE_DEFAULT_JOURNAL_SIZE_LIMIT -1
#endif
        }

        ///<summary>
        /// Maximum depth of an SQLite B-Tree structure. Any B-Tree deeper than
        /// this will be declared corrupt. This value is calculated based on a
        /// maximum database size of 2^31 pages a minimum fanout of 2 for a
        /// root-node and 3 for all other internal nodes.
        ///
        /// If a tree that appears to be taller than this is encountered, it is
        /// assumed that the database is corrupt.
        ///
        ///</summary>
        //#define BTCURSOR_MAX_DEPTH 20
        public const int BTCURSOR_MAX_DEPTH = 20;


        ///The maximum length of a TEXT or BLOB in bytes.   This also
        ///limits the size of a row in a table or index.
        ///The hard limit is the ability of a 32-bit signed integer
        ///to count the size: 2^31-1 or 2147483647.

#if !SQLITE_MAX_LENGTH
        public const int SQLITE_MAX_LENGTH = 1000000000;

#endif


        ///
        ///<summary>
        ///This is the maximum number of
        ///
        ///Columns in a table
        ///Columns in an index
        ///Columns in a view
        ///Terms in the SET clause of an UPDATE statement
        ///Terms in the result set of a SELECT statement
        ///Terms in the GROUP BY or ORDER BY clauses of a SELECT statement.
        ///Terms in the VALUES clause of an INSERT statement
        ///
        ///The hard upper limit here is 32676.  Most database people will
        ///</summary>
        ///<param name="tell you that in a well">normalized database, you usually should</param>
        ///<param name="not have more than a dozen or so columns in any table.  And if">not have more than a dozen or so columns in any table.  And if</param>
        ///<param name="that is the case, there is no point in having more than a few">that is the case, there is no point in having more than a few</param>
        ///<param name="dozen values in any of the other situations described above.">dozen values in any of the other situations described above.</param>

#if !SQLITE_MAX_COLUMN
        public const int SQLITE_MAX_COLUMN = 2000;

#endif
        ///
        ///<summary>
        ///The maximum length of a single SQL statement in bytes.
        ///
        ///It used to be the case that setting this value to zero would
        ///turn the limit off.  That is no longer true.  It is not possible
        ///to turn this limit off.
        ///</summary>

#if !SQLITE_MAX_SQL_LENGTH
        public const int SQLITE_MAX_SQL_LENGTH = 1000000000;

#endif
        ///
        ///<summary>
        ///The maximum depth of an expression tree. This is limited to
        ///some extent by SQLITE_MAX_SQL_LENGTH. But sometime you might
        ///want to place more severe limits on the complexity of an
        ///expression.
        ///
        ///A value of 0 used to mean that the limit was not enforced.
        ///But that is no longer true.  The limit is now strictly enforced
        ///at all times.
        ///</summary>

#if !SQLITE_MAX_EXPR_DEPTH
        public const int SQLITE_MAX_EXPR_DEPTH = 1000;

#endif
        ///
        ///<summary>
        ///The maximum number of terms in a compound SELECT statement.
        ///The code generator for compound SELECT statements does one
        ///level of recursion for each term.  A stack overflow can result
        ///if the number of terms is too large.  In practice, most SQL
        ///never has more than 3 or 4 terms.  Use a value of 0 to disable
        ///any limit on the number of terms in a compount SELECT.
        ///</summary>

#if !SQLITE_MAX_COMPOUND_SELECT
        public const int SQLITE_MAX_COMPOUND_SELECT = 250;

#endif
        ///
        ///<summary>
        ///The maximum number of opcodes in a VDBE program.
        ///Not currently enforced.
        ///</summary>

#if !SQLITE_MAX_VDBE_OP
        public const int SQLITE_MAX_VDBE_OP = 25000;

#endif



        ///
        ///<summary>
        ///The maximum number of arguments to an SQL function.
        ///</summary>

#if !SQLITE_MAX_FUNCTION_ARG
        public const int SQLITE_MAX_FUNCTION_ARG = 127;

        //# define SQLITE_MAX_FUNCTION_ARG 127
#endif

#if !SQLITE_DEFAULT_TEMP_CACHE_SIZE
        public const int SQLITE_DEFAULT_TEMP_CACHE_SIZE = 500;

#endif
        ///
        ///<summary>
        ///The default number of frames to accumulate in the log file before
        ///checkpointing the database in WAL mode.
        ///</summary>

#if !SQLITE_DEFAULT_WAL_AUTOCHECKPOINT
        public const int SQLITE_DEFAULT_WAL_AUTOCHECKPOINT = 1000;

        //# define SQLITE_DEFAULT_WAL_AUTOCHECKPOINT  1000
#endif
        ///
        ///<summary>
        ///The maximum number of attached databases.  This must be between 0
        ///</summary>
        ///<param name="and 62.  The upper bound on 62 is because a 64">bit integer bitmap</param>
        ///<param name="is used internally to track attached databases.">is used internally to track attached databases.</param>

#if !SQLITE_MAX_ATTACHED
        public const int SQLITE_MAX_ATTACHED = 10;

#endif
        ///
        ///<summary>
        ///The maximum value of a ?nnn wildcard that the parser will accept.
        ///</summary>

#if !SQLITE_MAX_VARIABLE_NUMBER
        public const int SQLITE_MAX_VARIABLE_NUMBER = 999;

#endif



















        ///
        ///<summary>
        ///Maximum page size.  The upper bound on this value is 65536.  This a limit
        ///</summary>
        ///<param name="imposed by the use of 16">bit offsets within each page.</param>
        ///<param name=""></param>
        ///<param name=""></param>
        ///<param name="Earlier versions of SQLite allowed the user to change this value at">Earlier versions of SQLite allowed the user to change this value at</param>
        ///<param name="compile time. This is no longer permitted, on the grounds that it creates">compile time. This is no longer permitted, on the grounds that it creates</param>
        ///<param name="a library that is technically incompatible with an SQLite library ">a library that is technically incompatible with an SQLite library </param>
        ///<param name="compiled with a different limit. If a process operating on a database ">compiled with a different limit. If a process operating on a database </param>
        ///<param name="with a page">size of 65536 bytes crashes, then an instance of SQLite </param>
        ///<param name="compiled with the default page">size limit will not be able to rollback </param>
        ///<param name="the aborted transaction. This could lead to database corruption.">the aborted transaction. This could lead to database corruption.</param>

        //#if SQLITE_MAX_PAGE_SIZE
        //# undef SQLITE_MAX_PAGE_SIZE
        //#endif
        //#define SQLITE_MAX_PAGE_SIZE 65536
        public const int SQLITE_MAX_PAGE_SIZE = 65535;

        ///
        ///<summary>
        ///The default size of a database page.
        ///</summary>

#if !SQLITE_DEFAULT_PAGE_SIZE
        public const int SQLITE_DEFAULT_PAGE_SIZE = 1024;

#endif
#if SQLITE_DEFAULT_PAGE_SIZE
																																						// undef SQLITE_DEFAULT_PAGE_SIZE
const int SQLITE_DEFAULT_PAGE_SIZE SQLITE_MAX_PAGE_SIZE
#endif
        ///
        ///<summary>
        ///Ordinarily, if no value is explicitly provided, SQLite creates databases
        ///with page size SQLITE_DEFAULT_PAGE_SIZE. However, based on certain
        ///</summary>
        ///<param name="device characteristics (sector">size and atomic write() support),</param>
        ///<param name="SQLite may choose a larger value. This constant is the maximum value">SQLite may choose a larger value. This constant is the maximum value</param>
        ///<param name="SQLite will choose on its own.">SQLite will choose on its own.</param>

#if !SQLITE_MAX_DEFAULT_PAGE_SIZE
        public const int SQLITE_MAX_DEFAULT_PAGE_SIZE = 8192;

#endif
#if SQLITE_MAX_DEFAULT_PAGE_SIZE
																																						// undef SQLITE_MAX_DEFAULT_PAGE_SIZE
const int SQLITE_MAX_DEFAULT_PAGE_SIZE SQLITE_MAX_PAGE_SIZE
#endif
        ///
        ///<summary>
        ///Maximum number of pages in one database file.
        ///
        ///This is really just the default value for the max_page_count pragma.
        ///</summary>
        ///<param name="This value can be lowered (or raised) at run">time using that the</param>
        ///<param name="max_page_count macro.">max_page_count macro.</param>

#if !SQLITE_MAX_PAGE_COUNT
        public const int SQLITE_MAX_PAGE_COUNT = 1073741823;

#endif
        ///
        ///<summary>
        ///Maximum length (in bytes) of the pattern in a LIKE or GLOB
        ///operator.
        ///</summary>

        //#if !SQLITE_MAX_LIKE_PATTERN_LENGTH
        public const int SQLITE_MAX_LIKE_PATTERN_LENGTH = 50000;

        //#endif
        ///
        ///<summary>
        ///Maximum depth of recursion for triggers.
        ///
        ///A value of 1 means that a trigger program will not be able to itself
        ///fire any triggers. A value of 0 means that no trigger programs at all 
        ///may be executed.
        ///
        ///</summary>

#if !SQLITE_MAX_TRIGGER_DEPTH
        public const int SQLITE_MAX_TRIGGER_DEPTH = 101;
        // # define SQLITE_MAX_TRIGGER_DEPTH 1000
#else
																			  const int SQLITE_MAX_TRIGGER_DEPTH=1;
#endif
    }
}
