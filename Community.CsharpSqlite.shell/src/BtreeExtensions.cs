using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Community.CsharpSqlite
{
    public static class BtreeExtensions
    {
        public static string sqlite3BtreeGetFilename(this Community.CsharpSqlite.Sqlite3.Btree _this) 
        {
            Debug.Assert(_this.pBt.pPager != null);
            return _this.pBt.pPager.sqlite3PagerFilename();
        }
        public static string sqlite3BtreeGetJournalname(this Community.CsharpSqlite.Sqlite3.Btree _this)
        {
            Debug.Assert(_this.pBt.pPager != null);
            return _this.pBt.pPager.sqlite3PagerJournalname();
        }
        public static bool sqlite3BtreeIsInTrans(this Community.CsharpSqlite.Sqlite3.Btree _this)
        {
            Debug.Assert(_this == null || Sqlite3.sqlite3_mutex_held(_this.db.mutex));
            return (_this != null && (_this.inTrans == Sqlite3.TRANS_WRITE));
        }
        public static bool sqlite3BtreeIsInReadTrans(this Community.CsharpSqlite.Sqlite3.Btree _this)
        {
            Debug.Assert(_this != null);
            Debug.Assert(Sqlite3.sqlite3_mutex_held(_this.db.mutex));
            return _this.inTrans != Sqlite3.TRANS_NONE;
        }
        public static bool sqlite3BtreeIsInBackup(this Community.CsharpSqlite.Sqlite3.Btree _this)
        {
            Debug.Assert(_this != null);
            Debug.Assert(Sqlite3.sqlite3_mutex_held(_this.db.mutex));
            return _this.nBackup != 0;
        }
    }
}
