using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Community.CsharpSqlite
{

    ///<summary>
    ///CAPI3REF: Result Codes
    ///KEYWORDS: SqlResult.SQLITE_OK {error code} {error codes}
    ///KEYWORDS: {result code} {result codes}
    ///
    ///Many SQLite functions return an integer result code from the set shown
    ///here in order to indicates success or failure.
    ///
    ///New error codes may be added in future versions of SQLite.
    ///
    ///See also: [SQLITE_IOERR_READ | extended result codes],
    ///[sqlite3_vtab_on_conflict()] [SQLITE_ROLLBACK | result codes].
    ///
    ///</summary>
    public enum SqlResult
    {
        SQLITE_OK = 0,
        SQLITE_ERROR = 1,
        SQLITE_INTERNAL = 2,
        SQLITE_PERM = 3,
        SQLITE_ABORT = 4,
        SQLITE_BUSY = 5,
        SQLITE_LOCKED = 6,
        SQLITE_NOMEM = 7,
        SQLITE_READONLY = 8,
        SQLITE_INTERRUPT = 9,
        SQLITE_IOERR = 10,
        SQLITE_CORRUPT = 11,
        SQLITE_NOTFOUND = 12,
        SQLITE_FULL = 13,
        SQLITE_CANTOPEN = 14,
        SQLITE_PROTOCOL = 15,
        SQLITE_EMPTY = 16,
        SQLITE_SCHEMA = 17,
        SQLITE_TOOBIG = 18,
        SQLITE_CONSTRAINT = 19,
        SQLITE_MISMATCH = 20,
        SQLITE_MISUSE = 21,
        SQLITE_NOLFS = 22,
        SQLITE_AUTH = 23,
        SQLITE_FORMAT = 24,
        SQLITE_RANGE = 25,
        SQLITE_NOTADB = 26,
        SQLITE_ROW = 100,
        SQLITE_DONE = 101,



        ///<summary>
        ///CAPI3REF: Extended Result Codes
        ///KEYWORDS: {extended error code} {extended error codes}
        ///KEYWORDS: {extended result code} {extended result codes}
        ///
        ///In its default configuration, SQLite API routines return one of 26 integer
        ///[SqlResult.SQLITE_OK | result codes].  However, experience has shown that many of
        ///</summary>
        ///<param name="these result codes are too coarse">grained.  They do not provide as</param>
        ///<param name="much information about problems as programmers might like.  In an effort to">much information about problems as programmers might like.  In an effort to</param>
        ///<param name="address this, newer versions of SQLite (version 3.3.8 and later) include">address this, newer versions of SQLite (version 3.3.8 and later) include</param>
        ///<param name="support for additional result codes that provide more detailed information">support for additional result codes that provide more detailed information</param>
        ///<param name="about errors. The extended result codes are enabled or disabled">about errors. The extended result codes are enabled or disabled</param>
        ///<param name="on a per database connection basis using the">on a per database connection basis using the</param>
        ///<param name="[sqlite3_extended_result_codes()] API.">[sqlite3_extended_result_codes()] API.</param>
        ///<param name=""></param>
        ///<param name="Some of the available extended result codes are listed here.">Some of the available extended result codes are listed here.</param>
        ///<param name="One may expect the number of extended result codes will be expand">One may expect the number of extended result codes will be expand</param>
        ///<param name="over time.  Software that uses extended result codes should expect">over time.  Software that uses extended result codes should expect</param>
        ///<param name="to see new result codes in future releases of SQLite.">to see new result codes in future releases of SQLite.</param>
        ///<param name=""></param>
        ///<param name="The SqlResult.SQLITE_OK result code will never be extended.  It will always">The SqlResult.SQLITE_OK result code will never be extended.  It will always</param>
        ///<param name="be exactly zero.">be exactly zero.</param>
        ///<param name=""></param>

        //#define SQLITE_IOERR_READ              (SQLITE_IOERR | (1<<8))
        //#define SQLITE_IOERR_SHORT_READ        (SQLITE_IOERR | (2<<8))
        //#define SQLITE_IOERR_WRITE             (SQLITE_IOERR | (3<<8))
        //#define SQLITE_IOERR_FSYNC             (SQLITE_IOERR | (4<<8))
        //#define SQLITE_IOERR_DIR_FSYNC         (SQLITE_IOERR | (5<<8))
        //#define SQLITE_IOERR_TRUNCATE          (SQLITE_IOERR | (6<<8))
        //#define SQLITE_IOERR_FSTAT             (SQLITE_IOERR | (7<<8))
        //#define SQLITE_IOERR_UNLOCK            (SQLITE_IOERR | (8<<8))
        //#define SQLITE_IOERR_RDLOCK            (SQLITE_IOERR | (9<<8))
        //#define SQLITE_IOERR_DELETE            (SQLITE_IOERR | (10<<8))
        //#define SQLITE_IOERR_BLOCKED           (SQLITE_IOERR | (11<<8))
        //#define SQLITE_IOERR_NOMEM             (SQLITE_IOERR | (12<<8))
        //#define SQLITE_IOERR_ACCESS            (SQLITE_IOERR | (13<<8))
        //#define SQLITE_IOERR_CHECKRESERVEDLOCK (SQLITE_IOERR | (14<<8))
        //#define SQLITE_IOERR_LOCK              (SQLITE_IOERR | (15<<8))
        //#define SQLITE_IOERR_CLOSE             (SQLITE_IOERR | (16<<8))
        //#define SQLITE_IOERR_DIR_CLOSE         (SQLITE_IOERR | (17<<8))
        //#define SQLITE_IOERR_SHMOPEN           (SQLITE_IOERR | (18<<8))
        //#define SQLITE_IOERR_SHMSIZE           (SQLITE_IOERR | (19<<8))
        //#define SQLITE_IOERR_SHMLOCK           (SQLITE_IOERR | (20<<8))
        //#define SQLITE_IOERR_SHMMAP            (SQLITE_IOERR | (21<<8))
        //#define SQLITE_IOERR_SEEK              (SQLITE_IOERR | (22<<8))
        //#define SQLITE_LOCKED_SHAREDCACHE      (SQLITE_LOCKED |  (1<<8))
        //#define SQLITE_BUSY_RECOVERY           (SQLITE_BUSY   |  (1<<8))
        //#define SQLITE_CANTOPEN_NOTEMPDIR      (SQLITE_CANTOPEN | (1<<8))
        //#define SQLITE_CORRUPT_VTAB            (SQLITE_CORRUPT | (1<<8))
        //#define SQLITE_READONLY_RECOVERY       (SQLITE_READONLY | (1<<8))
        //#define SQLITE_READONLY_CANTLOCK       (SQLITE_READONLY | (2<<8))

    

    SQLITE_IOERR_READ = (SQLITE_IOERR | (1 << 8)),

    SQLITE_IOERR_SHORT_READ = (SQLITE_IOERR | (2 << 8)),

    SQLITE_IOERR_WRITE = (SQLITE_IOERR | (3 << 8)),

    SQLITE_IOERR_FSYNC = (SQLITE_IOERR | (4 << 8)),

    SQLITE_IOERR_DIR_FSYNC = (SQLITE_IOERR | (5 << 8)),

    SQLITE_IOERR_TRUNCATE = (SQLITE_IOERR | (6 << 8)),

    SQLITE_IOERR_FSTAT = (SQLITE_IOERR | (7 << 8)),

    SQLITE_IOERR_UNLOCK = (SQLITE_IOERR | (8 << 8)),

    SQLITE_IOERR_RDLOCK = (SQLITE_IOERR | (9 << 8)),

    SQLITE_IOERR_DELETE = (SQLITE_IOERR | (10 << 8)),

    SQLITE_IOERR_BLOCKED = (SQLITE_IOERR | (11 << 8)),

    SQLITE_IOERR_NOMEM = (SQLITE_IOERR | (12 << 8)),

    SQLITE_IOERR_ACCESS = (SQLITE_IOERR | (13 << 8)),

    SQLITE_IOERR_CHECKRESERVEDLOCK = (SQLITE_IOERR | (14 << 8)),

    SQLITE_IOERR_LOCK = (SQLITE_IOERR | (15 << 8)),

    SQLITE_IOERR_CLOSE = (SQLITE_IOERR | (16 << 8)),

    SQLITE_IOERR_DIR_CLOSE = (SQLITE_IOERR | (17 << 8)),

    SQLITE_IOERR_SHMOPEN = (SQLITE_IOERR | (18 << 8)),

    SQLITE_IOERR_SHMSIZE = (SQLITE_IOERR | (19 << 8)),

    SQLITE_IOERR_SHMLOCK = (SQLITE_IOERR | (20 << 8)),

    SQLITE_IOERR_SHMMAP = (SQLITE_IOERR | (21 << 8)),

    SQLITE_IOERR_SEEK = (SQLITE_IOERR | (22 << 8)),

    SQLITE_LOCKED_SHAREDCACHE = (SQLITE_LOCKED | (1 << 8)),

    SQLITE_BUSY_RECOVERY = (SQLITE_BUSY | (1 << 8)),

    SQLITE_CANTOPEN_NOTEMPDIR = (SQLITE_CANTOPEN | (1 << 8)),

    SQLITE_CORRUPT_VTAB = (SQLITE_CORRUPT | (1 << 8)),

    SQLITE_READONLY_RECOVERY = (SQLITE_READONLY | (1 << 8)),

    SQLITE_READONLY_CANTLOCK = (SQLITE_READONLY | (2 << 8))

}

    //#define SqlResult.SQLITE_OK           0   /* Successful result */
    ///* beginning-of-error-codes */
    //#define SqlResult.SQLITE_ERROR        1   /* SQL error or missing database */
    //#define SQLITE_INTERNAL     2   /* Internal logic error in SQLite */
    //#define SQLITE_PERM         3   /* Access permission denied */
    //#define SQLITE_ABORT        4   /* Callback routine requested an abort */
    //#define SQLITE_BUSY         5   /* The database file is locked */
    //#define SQLITE_LOCKED       6   /* A table in the database is locked */
    //#define SQLITE_NOMEM        7   /* A malloc() failed */
    //#define SQLITE_READONLY     8   /* Attempt to write a readonly database */
    //#define SQLITE_INTERRUPT    9   /* Operation terminated by sqlite3_interrupt()*/
    //#define SQLITE_IOERR       10   /* Some kind of disk I/O error occurred */
    //#define SQLITE_CORRUPT     11   /* The database disk image is malformed */
    //#define SQLITE_NOTFOUND    12   /* Unknown opcode in sqlite3_file_control() */
    //#define SQLITE_FULL        13   /* Insertion failed because database is full */
    //#define SQLITE_CANTOPEN    14   /* Unable to open the database file */
    //#define SQLITE_PROTOCOL    15   /* Database lock protocol error */
    //#define SQLITE_EMPTY       16   /* Database is empty */
    //#define SQLITE_SCHEMA      17   /* The database schema changed */
    //#define SQLITE_TOOBIG      18   /* String or BLOB exceeds size limit */
    //#define SQLITE_CONSTRAINT  19   /* Abort due to constraint violation */
    //#define SQLITE_MISMATCH    20   /* Data type mismatch */
    //#define SQLITE_MISUSE      21   /* Library used incorrectly */
    //#define SQLITE_NOLFS       22   /* Uses OS features not supported on host */
    //#define SQLITE_AUTH        23   /* Authorization denied */
    //#define SQLITE_FORMAT      24   /* Auxiliary database format error */
    //#define SQLITE_RANGE       25   /* 2nd parameter to sqlite3_bind out of range */
    //#define SQLITE_NOTADB      26   /* File opened that is not a database file */
    //#define SQLITE_ROW         100  /* sqlite3_step() has another row ready */
    //#define SQLITE_DONE        101  /* sqlite3_step() has finished executing */
    ///
    ///<summary>
    ///</summary>
    ///<param name="end">codes </param>
    /*
    public const int SQLITE_OK = 0;

    public const int SQLITE_ERROR = 1;

    public const int SQLITE_INTERNAL = 2;

    public const int SQLITE_PERM = 3;

    public const int SQLITE_ABORT = 4;

    public const int SQLITE_BUSY = 5;

    public const int SQLITE_LOCKED = 6;

    public const int SQLITE_NOMEM = 7;

    public const int SQLITE_READONLY = 8;

    public const int SQLITE_INTERRUPT = 9;

    public const int SQLITE_IOERR = 10;

    public const int SQLITE_CORRUPT = 11;

    public const int SQLITE_NOTFOUND = 12;

    public const int SQLITE_FULL = 13;

    public const int SQLITE_CANTOPEN = 14;

    public const int SQLITE_PROTOCOL = 15;

    public const int SQLITE_EMPTY = 16;

    public const int SQLITE_SCHEMA = 17;

    public const int SQLITE_TOOBIG = 18;

    public const int SQLITE_CONSTRAINT = 19;

    public const int SQLITE_MISMATCH = 20;

    public const int SQLITE_MISUSE = 21;

    public const int SQLITE_NOLFS = 22;

    public const int SQLITE_AUTH = 23;

    public const int SQLITE_FORMAT = 24;

    public const int SQLITE_RANGE = 25;

    public const int SQLITE_NOTADB = 26;

    public const int SQLITE_ROW = 100;

    public const int SQLITE_DONE = 101;


    */

}
