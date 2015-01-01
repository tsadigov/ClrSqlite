using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Community.CsharpSqlite
{

    ///<summary>
    /// A pointer to this structure is used to communicate information
    /// from sqlite3Init and OP_ParseSchema into the sqlite3InitCallback.
    ///
    ///</summary>
    public class InitData
    {
        public Sqlite3.sqlite3 db;
        ///
        ///<summary>
        ///The database being initialized 
        ///</summary>
        public int iDb;
        ///
        ///<summary>
        ///0 for main database.  1 for TEMP, 2.. for ATTACHed 
        ///</summary>
        public string pzErrMsg;
        ///
        ///<summary>
        ///Error message stored here 
        ///</summary>
        public int rc;
        ///
        ///<summary>
        ///Result code stored here 
        ///</summary>
    }


}
