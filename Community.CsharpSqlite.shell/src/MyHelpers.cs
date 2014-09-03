using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Community.CsharpSqlite
{
	public static class MyHelpers
	{
		public static Sqlite3.VdbeFrame GetRoot (this Sqlite3.VdbeFrame _this)
		{
			Sqlite3.VdbeFrame pFrame = null;
			if (null != pFrame)
				for (pFrame = _this; pFrame.pParent != null; pFrame = pFrame.pParent)
					;
			return pFrame;
		}
	}
}
