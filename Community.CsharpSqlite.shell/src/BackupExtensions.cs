using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Community.CsharpSqlite
{
    public static class BackupExtensions
    {
        ///<summary>
        /// This function is called after the contents of page iPage of the
        /// source database have been modified. If page iPage has already been
        /// copied into the destination database, then the data written to the
        /// destination is now invalidated. The destination copy of iPage needs
        /// to be updated with the new data before the backup operation is
        /// complete.
        ///
        /// It is assumed that the mutex associated with the BtShared object
        /// corresponding to the source database is held when this function is
        /// called.
        ///</summary>
        public static void sqlite3BackupUpdate(this sqlite3_backup _this, uint iPage, byte[] aData)
        {
            sqlite3_backup p;
            ///
            ///<summary>
            ///Iterator variable 
            ///</summary>
            for (p = _this; p != null; p = p.pNext)
            {
                Debug.Assert(p.pSrc.pBt.mutex.sqlite3_mutex_held());
                if (!Sqlite3.isFatalError(p.rc) && iPage < p.iNext)
                {
                    ///
                    ///<summary>
                    ///The backup process p has already copied page iPage. But now it
                    ///has been modified by a transaction on the source pager. Copy
                    ///the new data into the backup.
                    ///
                    ///</summary>
                    SqlResult rc;
                    Debug.Assert(p.pDestDb != null);
                    p.pDestDb.mutex.Enter();
                    rc = p.backupOnePage(iPage, aData);
                    p.pDestDb.mutex.Exit();
                    Debug.Assert(rc != SqlResult.SQLITE_BUSY && rc != SqlResult.SQLITE_LOCKED);
                    if (rc != SqlResult.SQLITE_OK)
                    {
                        p.rc = rc;
                    }
                }
            }
        }
    }
}
