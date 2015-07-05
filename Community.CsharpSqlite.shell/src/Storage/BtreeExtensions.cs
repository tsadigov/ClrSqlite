using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Community.CsharpSqlite
{
	public static class BtreeExtensions
	{
		public static string GetFilename (this Community.CsharpSqlite.Sqlite3.Btree _this)
		{
			Debug.Assert (_this.pBt.pPager != null);
			return _this.pBt.pPager.Filename ();
		}

		public static string GetJournalname (this Community.CsharpSqlite.Sqlite3.Btree _this)
		{
			Debug.Assert (_this.pBt.pPager != null);
			return _this.pBt.pPager.sqlite3PagerJournalname ();
		}

		public static bool sqlite3BtreeIsInTrans (this Community.CsharpSqlite.Sqlite3.Btree _this)
		{
			Debug.Assert (_this == null || _this.db.mutex.sqlite3_mutex_held());
            return (_this != null && (_this.inTrans == TransType.TRANS_WRITE));
		}

		public static bool sqlite3BtreeIsInReadTrans (this Community.CsharpSqlite.Sqlite3.Btree _this)
		{
			Debug.Assert (_this != null);
			Debug.Assert (_this.db.mutex.sqlite3_mutex_held());
            return _this.inTrans != TransType.TRANS_NONE;
		}

		public static bool sqlite3BtreeIsInBackup (this Community.CsharpSqlite.Sqlite3.Btree _this)
		{
			Debug.Assert (_this != null);
			Debug.Assert (_this.db.mutex.sqlite3_mutex_held());
			return _this.nBackup != 0;
		}
	}
}
