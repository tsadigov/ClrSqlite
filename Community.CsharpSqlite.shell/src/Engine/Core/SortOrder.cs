using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Community.CsharpSqlite
{
    ///
    ///<summary>
    ///A sort order can be either ASC or DESC.
    ///
    ///</summary>
    public enum SortOrder : byte
    {
        SQLITE_SO_ASC = 0,
        SQLITE_SO_DESC = 1
    }
}
