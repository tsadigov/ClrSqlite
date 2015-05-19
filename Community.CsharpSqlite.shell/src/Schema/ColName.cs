using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Community.CsharpSqlite.Metadata
{
    ///
    ///<summary>
    ///The Vdbe.aColName array contains 5n Mem structures, where n is the
    ///number of columns of data returned by the statement.
    ///
    ///</summary>

    //#if SQLITE_ENABLE_COLUMN_METADATA
    //# define COLNAME_N        5      /* Number of COLNAME_xxx symbols */
    //#else
    //# ifdef SQLITE_OMIT_DECLTYPE
    //#   define COLNAME_N      1      /* Store only the name */
    //# else
    //#   define COLNAME_N      2      /* Store the name and decltype */
    //# endif
    //#endif

    public enum ColName { NAME, DECLTYPE, DATABASE, TABLE, COLUMN }
}
