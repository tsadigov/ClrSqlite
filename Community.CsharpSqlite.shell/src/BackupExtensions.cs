﻿using System;
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
        public static void sqlite3BackupUpdate(this Sqlite3.sqlite3_backup _this, uint iPage, byte[] aData)
        {
            Sqlite3.sqlite3_backup p;
            ///
            ///<summary>
            ///Iterator variable 
            ///</summary>
            for (p = _this; p != null; p = p.pNext)
            {
                Debug.Assert(Sqlite3.sqlite3_mutex_held(p.pSrc.pBt.mutex));
                if (!Sqlite3.isFatalError(p.rc) && iPage < p.iNext)
                {
                    ///
                    ///<summary>
                    ///The backup process p has already copied page iPage. But now it
                    ///has been modified by a transaction on the source pager. Copy
                    ///the new data into the backup.
                    ///
                    ///</summary>
                    int rc;
                    Debug.Assert(p.pDestDb != null);
                    Sqlite3.sqlite3_mutex_enter(p.pDestDb.mutex);
                    rc = p.backupOnePage(iPage, aData);
                    Sqlite3.sqlite3_mutex_leave(p.pDestDb.mutex);
                    Debug.Assert(rc != Sqlite3.SQLITE_BUSY && rc != Sqlite3.SQLITE_LOCKED);
                    if (rc != Sqlite3.SQLITE_OK)
                    {
                        p.rc = rc;
                    }
                }
            }
        }
    }
}
