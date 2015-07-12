using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Community.CsharpSqlite
{
    ///<summary>
    ///CAPI3REF: Authorizer Return Codes
    ///
    ///The [sqlite3_set_authorizer | authorizer callback function] must
    ///return either [SqlResult.SQLITE_OK] or one of these two constants in order
    ///to signal SQLite whether or not the action is permitted.  See the
    ///[sqlite3_set_authorizer | authorizer documentation] for additional
    ///information.
    ///
    ///Note that SQLITE_IGNORE is also used as a [SQLITE_ROLLBACK | return code]
    ///from the [sqlite3_vtab_on_conflict()] interface.
    ///
    ///</summary>

    //#define SQLITE_DENY   1   /* Abort the SQL statement with an error */
    //#define SQLITE_IGNORE 2   /* Don't allow access, but don't generate an error */
    public enum AuthResult
    {
        SQLITE_DENY = 1,

        SQLITE_IGNORE = 2,

        SQLITE_OK = 0//tural added
    }

    ///
    ///<summary>
    ///CAPI3REF: Authorizer Action Codes
    ///
    ///The [sqlite3_set_authorizer()] interface registers a callback function
    ///that is invoked to authorize certain SQL statement actions.  The
    ///second parameter to the callback is an integer code that specifies
    ///what action is being authorized.  These are the integer action codes that
    ///the authorizer callback may be passed.
    ///
    ///These action code values signify what kind of operation is to be
    ///authorized.  The 3rd and 4th parameters to the authorization
    ///callback function will be parameters or NULL depending on which of these
    ///codes is used as the second parameter.  ^(The 5th parameter to the
    ///authorizer callback is the name of the database ("main", "temp",
    ///etc.) if applicable.)^  ^The 6th parameter to the authorizer callback
    ///</summary>
    ///<param name="is the name of the inner">most trigger or view that is responsible for</param>
    ///<param name="the access attempt or NULL if this access attempt is directly from">the access attempt or NULL if this access attempt is directly from</param>
    ///<param name="top">level SQL code.</param>
    ///<param name=""></param>

    ///
    ///<summary>
    ///3rd ************ 4th **********
    ///</summary>

    //#define SQLITE_CREATE_INDEX          1   /* Index Name      Table Name      */
    //#define SQLITE_CREATE_TABLE          2   /* Table Name      NULL            */
    //#define SQLITE_CREATE_TEMP_INDEX     3   /* Index Name      Table Name      */
    //#define SQLITE_CREATE_TEMP_TABLE     4   /* Table Name      NULL            */
    //#define SQLITE_CREATE_TEMP_TRIGGER   5   /* Trigger Name    Table Name      */
    //#define SQLITE_CREATE_TEMP_VIEW      6   /* View Name       NULL            */
    //#define SQLITE_CREATE_TRIGGER        7   /* Trigger Name    Table Name      */
    //#define SQLITE_CREATE_VIEW           8   /* View Name       NULL            */
    //#define SQLITE_DELETE                9   /* Table Name      NULL            */
    //#define SQLITE_DROP_INDEX           10   /* Index Name      Table Name      */
    //#define SQLITE_DROP_TABLE           11   /* Table Name      NULL            */
    //#define SQLITE_DROP_TEMP_INDEX      12   /* Index Name      Table Name      */
    //#define SQLITE_DROP_TEMP_TABLE      13   /* Table Name      NULL            */
    //#define SQLITE_DROP_TEMP_TRIGGER    14   /* Trigger Name    Table Name      */
    //#define SQLITE_DROP_TEMP_VIEW       15   /* View Name       NULL            */
    //#define SQLITE_DROP_TRIGGER         16   /* Trigger Name    Table Name      */
    //#define SQLITE_DROP_VIEW            17   /* View Name       NULL            */
    //#define SQLITE_INSERT               18   /* Table Name      NULL            */
    //#define SQLITE_PRAGMA               19   /* Pragma Name     1st arg or NULL */
    //#define SQLITE_READ                 20   /* Table Name      Column Name     */
    //#define SQLITE_SELECT               21   /* NULL            NULL            */
    //#define SQLITE_TRANSACTION          22   /* Op       NULL            */
    //#define SQLITE_UPDATE               23   /* Table Name      Column Name     */
    //#define SQLITE_ATTACH               24   /* Filename        NULL            */
    //#define SQLITE_DETACH               25   /* Database Name   NULL            */
    //#define SQLITE_ALTER_TABLE          26   /* Database Name   Table Name      */
    //#define SQLITE_REINDEX              27   /* Index Name      NULL            */
    //#define SQLITE_ANALYZE              28   /* Table Name      NULL            */
    //#define SQLITE_CREATE_VTABLE        29   /* Table Name      Module Name     */
    //#define SQLITE_DROP_VTABLE          30   /* Table Name      Module Name     */
    //#define SQLITE_FUNCTION             31   /* NULL            Function Name   */
    //#define SQLITE_SAVEPOINT            32   /* Op       Savepoint Name  */
    //#define SQLITE_COPY                  0   /* No longer used */
    public enum AuthTarget
    {
        SQLITE_CREATE_INDEX = 1,

        SQLITE_CREATE_TABLE = 2,

        SQLITE_CREATE_TEMP_INDEX = 3,

        SQLITE_CREATE_TEMP_TABLE = 4,

        SQLITE_CREATE_TEMP_TRIGGER = 5,

        SQLITE_CREATE_TEMP_VIEW = 6,

        SQLITE_CREATE_TRIGGER = 7,

        SQLITE_CREATE_VIEW = 8,

        SQLITE_DELETE = 9,

        SQLITE_DROP_INDEX = 10,

        SQLITE_DROP_TABLE = 11,

        SQLITE_DROP_TEMP_INDEX = 12,

        SQLITE_DROP_TEMP_TABLE = 13,

        SQLITE_DROP_TEMP_TRIGGER = 14,

        SQLITE_DROP_TEMP_VIEW = 15,

        SQLITE_DROP_TRIGGER = 16,

        SQLITE_DROP_VIEW = 17,

        SQLITE_INSERT = 18,

        SQLITE_PRAGMA = 19,

        SQLITE_READ = 20,

        SQLITE_SELECT = 21,

        SQLITE_TRANSACTION = 22,

        SQLITE_UPDATE = 23,

        SQLITE_ATTACH = 24,

        SQLITE_DETACH = 25,

        SQLITE_ALTER_TABLE = 26,

        SQLITE_REINDEX = 27,

        SQLITE_ANALYZE = 28,

        SQLITE_CREATE_VTABLE = 29,

        SQLITE_DROP_VTABLE = 30,

        SQLITE_FUNCTION = 31,

        SQLITE_SAVEPOINT = 32,

        SQLITE_COPY = 0
    }

}
