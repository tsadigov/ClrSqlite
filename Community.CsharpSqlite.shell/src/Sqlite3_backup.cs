using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Community.CsharpSqlite
{
    public static class Sqlite3_backupExtensions
    {
        public///<summary>
            /// Restart the backup process. This is called when the pager layer
            /// detects that the database has been modified by an external database
            /// connection. In this case there is no way of knowing which of the
            /// pages that have been copied into the destination database are still
            /// valid and which are not, so the entire process needs to be restarted.
            ///
            /// It is assumed that the mutex associated with the BtShared object
            /// corresponding to the source database is held when this function is
            /// called.
            ///</summary>
        static    void sqlite3BackupRestart(this Sqlite3.sqlite3_backup _this)
        {
            Community.CsharpSqlite.Sqlite3.sqlite3_backup p;
            /* Iterator variable */
            for (p = _this; p != null; p = p.pNext)
            {
                Debug.Assert(Sqlite3.sqlite3_mutex_held(p.pSrc.pBt.mutex));
                p.iNext = 1;
            }
        }
    }
}
