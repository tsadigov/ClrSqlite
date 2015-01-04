using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Community.CsharpSqlite
{
    ///<summary>
    /// CAPI3REF: Virtual Table Object
    /// KEYWORDS: sqlite3_module {virtual table module}
    ///
    /// This structure, sometimes called a "virtual table module",
    /// defines the implementation of a [virtual tables].
    /// This structure consists mostly of methods for the module.
    ///
    /// ^A virtual table module is created by filling in a persistent
    /// instance of this structure and passing a pointer to that instance
    /// to [sqlite3_create_module()] or [sqlite3_create_module_v2()].
    /// ^The registration remains valid until it is replaced by a different
    /// module or until the [database connection] closes.  The content
    /// of this structure must not change while it is registered with
    /// any database connection.
    ///
    ///</summary>
    //struct sqlite3_module {
    //  int iVersion;
    //  int (*xCreate)(sqlite3*, object  *pAux,
    //               int argc, string[] argv,
    //               sqlite3_vtab **ppVTab, char*);
    //  int (*xConnect)(sqlite3*, object  *pAux,
    //               int argc, string[] argv,
    //               sqlite3_vtab **ppVTab, char*);
    //  int (*xBestIndex)(sqlite3_vtab *pVTab, sqlite3_index_info);
    //  int (*xDisconnect)(sqlite3_vtab *pVTab);
    //  int (*xDestroy)(sqlite3_vtab *pVTab);
    //  int (*xOpen)(sqlite3_vtab *pVTab, sqlite3_vtab_cursor **ppCursor);
    //  int (*xClose)(sqlite3_vtab_cursor);
    //  int (*xFilter)(sqlite3_vtab_cursor*, int idxNum, string idxStr,
    //                int argc, sqlite3_value **argv);
    //  int (*xNext)(sqlite3_vtab_cursor);
    //  int (*xEof)(sqlite3_vtab_cursor);
    //  int (*xColumn)(sqlite3_vtab_cursor*, sqlite3_context*, int);
    //  int (*xRowid)(sqlite3_vtab_cursor*, sqlite3_int64 *pRowid);
    //  int (*xUpdate)(sqlite3_vtab *, int, sqlite3_value **, sqlite3_int64 );
    //  int (*xBegin)(sqlite3_vtab *pVTab);
    //  int (*xSync)(sqlite3_vtab *pVTab);
    //  int (*xCommit)(sqlite3_vtab *pVTab);
    //  int (*xRollback)(sqlite3_vtab *pVTab);
    //  int (*xFindFunction)(sqlite3_vtab *pVtab, int nArg, string zName,
    //                       void (**pxFunc)(sqlite3_context*,int,sqlite3_value*),
    //                       void **ppArg);
    //  int (*xRename)(sqlite3_vtab *pVtab, string zNew);
    ///* The methods above are in version 1 of the sqlite_module object. Those 
    //** below are for version 2 and greater. */
    //int (*xSavepoint)(sqlite3_vtab *pVTab, int);
    //int (*xRelease)(sqlite3_vtab *pVTab, int);
    //int (*xRollbackTo)(sqlite3_vtab *pVTab, int);
    //};
    // MINIMAL STRUCTURE
    public class sqlite3_module
    {
        public int iVersion;

        public smdxCreateConnect xCreate;

        public smdxCreateConnect xConnect;

        public smdxBestIndex xBestIndex;

        public smdxDisconnect xDisconnect;

        public smdxDestroy xDestroy;

        public smdxOpen xOpen;

        public smdxClose xClose;

        public smdxFilter xFilter;

        public smdxNext xNext;

        public smdxEof xEof;

        public smdxColumn xColumn;

        public smdxRowid xRowid;

        public smdxUpdate xUpdate;

        public smdxFunction xBegin;

        public smdxFunction xSync;

        public smdxFunction xCommit;

        public smdxFunction xRollback;

        public smdxFindFunction xFindFunction;

        public smdxRename xRename;

        ///
        ///<summary>
        ///The methods above are in version 1 of the sqlite_module object. Those 
        ///below are for version 2 and greater. 
        ///</summary>

        public smdxFunctionArg xSavepoint;

        public smdxFunctionArg xRelease;

        public smdxFunctionArg xRollbackTo;

        //Version 1
        public sqlite3_module(int iVersion, smdxCreateConnect xCreate, smdxCreateConnect xConnect, smdxBestIndex xBestIndex, smdxDisconnect xDisconnect, smdxDestroy xDestroy, smdxOpen xOpen, smdxClose xClose, smdxFilter xFilter, smdxNext xNext, smdxEof xEof, smdxColumn xColumn, smdxRowid xRowid, smdxUpdate xUpdate, smdxFunction xBegin, smdxFunction xSync, smdxFunction xCommit, smdxFunction xRollback, smdxFindFunction xFindFunction, smdxRename xRename)
        {
            this.iVersion = iVersion;
            this.xCreate = xCreate;
            this.xConnect = xConnect;
            this.xBestIndex = xBestIndex;
            this.xDisconnect = xDisconnect;
            this.xDestroy = xDestroy;
            this.xOpen = xOpen;
            this.xClose = xClose;
            this.xFilter = xFilter;
            this.xNext = xNext;
            this.xEof = xEof;
            this.xColumn = xColumn;
            this.xRowid = xRowid;
            this.xUpdate = xUpdate;
            this.xBegin = xBegin;
            this.xSync = xSync;
            this.xCommit = xCommit;
            this.xRollback = xRollback;
            this.xFindFunction = xFindFunction;
            this.xRename = xRename;
        }

        //Version 2
        public sqlite3_module(int iVersion, smdxCreateConnect xCreate, smdxCreateConnect xConnect, smdxBestIndex xBestIndex, smdxDisconnect xDisconnect, smdxDestroy xDestroy, smdxOpen xOpen, smdxClose xClose, smdxFilter xFilter, smdxNext xNext, smdxEof xEof, smdxColumn xColumn, smdxRowid xRowid, smdxUpdate xUpdate, smdxFunction xBegin, smdxFunction xSync, smdxFunction xCommit, smdxFunction xRollback, smdxFindFunction xFindFunction, smdxRename xRename, ///
            ///<summary>
            ///The methods above are in version 1 of the sqlite_module object. Those 
            ///below are for version 2 and greater. 
            ///</summary>

        smdxFunctionArg xSavepoint, smdxFunctionArg xRelease, smdxFunctionArg xRollbackTo)
        {
            this.iVersion = iVersion;
            this.xCreate = xCreate;
            this.xConnect = xConnect;
            this.xBestIndex = xBestIndex;
            this.xDisconnect = xDisconnect;
            this.xDestroy = xDestroy;
            this.xOpen = xOpen;
            this.xClose = xClose;
            this.xFilter = xFilter;
            this.xNext = xNext;
            this.xEof = xEof;
            this.xColumn = xColumn;
            this.xRowid = xRowid;
            this.xUpdate = xUpdate;
            this.xBegin = xBegin;
            this.xSync = xSync;
            this.xCommit = xCommit;
            this.xRollback = xRollback;
            this.xFindFunction = xFindFunction;
            this.xRename = xRename;
            this.xSavepoint = xSavepoint;
            this.xRelease = xRelease;
            this.xRollbackTo = xRollbackTo;
        }
    }

}
