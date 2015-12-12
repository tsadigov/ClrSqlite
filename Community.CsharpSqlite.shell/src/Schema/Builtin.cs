using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Community.CsharpSqlite.Metadata
{
    public class Builtin
    {
        ///<summary>
        ///Name of the master database table.  The master database table
        ///is a special table that holds the names and attributes of all
        ///user tables and indices.
        ///
        ///</summary>
        private const string MASTER_NAME = "sqlite_master";
        //#define MASTER_NAME       "sqlite_master"
        private const string TEMP_MASTER_NAME = "sqlite_temp_master";
        //#define TEMP_MASTER_NAME  "sqlite_temp_master"
        ///<summary>
        /// The root-page of the master database table.
        ///
        ///</summary>
        public const int MASTER_ROOT = 1;
        //#define MASTER_ROOT       1
        ///
        ///<summary>
        ///The name of the schema table.
        ///
        ///</summary>
        public static string SCHEMA_TABLE(int x)//#define SCHEMA_TABLE(x)  ((!OMIT_TEMPDB)&&(x==1)?TEMP_MASTER_NAME:MASTER_NAME)
        {
            return ((OMIT_TEMPDB == 0) && (x == 1) ? TEMP_MASTER_NAME : MASTER_NAME);
        }


        ///
        ///<summary>
        ///OMIT_TEMPDB is set to 1 if SQLITE_OMIT_TEMPDB is defined, or 0
        ///afterward. Having this macro allows us to cause the C compiler
        ///to omit code used by TEMP tables without messy #if !statements.
        ///</summary>
#if SQLITE_OMIT_TEMPDB
																																																												//define OMIT_TEMPDB 1
#else
        public static int OMIT_TEMPDB = 0;
#endif
    }
}
