using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Community.CsharpSqlite
{
    using Metadata;


    ///<summary>
    /// CAPI3REF: Virtual Table Cursor Object
    /// KEYWORDS: sqlite3_vtab_cursor {virtual table cursor}
    ///
    /// Every [virtual table module] implementation uses a subclass of the
    /// following structure to describe cursors that point into the
    /// [virtual table] and are used
    /// to loop through the virtual table.  Cursors are created using the
    /// [sqlite3_module.xOpen | xOpen] method of the module and are destroyed
    /// by the [sqlite3_module.xClose | xClose] method.  Cursors are used
    /// by the [xFilter], [xNext], [xEof], [xColumn], and [xRowid] methods
    /// of the module.  Each module implementation will define
    /// the content of a cursor structure to suit its own needs.
    ///
    /// This superclass exists in order to define fields of the cursor that
    /// are common to all implementations.
    ///
    ///</summary>
    //struct sqlite3_vtab_cursor {
    //  sqlite3_vtab *pVtab;      /* Virtual table of this cursor */
    //  /* Virtual table implementations will typically add additional fields */
    //};
    public class sqlite3_vtab_cursor
    {
        public sqlite3_vtab pVtab;
        ///
        ///<summary>
        ///Virtual table of this cursor 
        ///</summary>

        ///
        ///<summary>
        ///Virtual table implementations will typically add additional fields 
        ///</summary>

    };
}
