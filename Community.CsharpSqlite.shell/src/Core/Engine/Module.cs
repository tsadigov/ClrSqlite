using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Community.CsharpSqlite.Metadata
{
    ///<summary>
    /// Each SQLite module (virtual table definition) is defined by an
    /// instance of the following structure, stored in the sqlite3.aModule
    /// hash table.
    ///
    ///</summary>
    public class Module
    {
        public sqlite3_module pModule;
        ///
        ///<summary>
        ///Callback pointers 
        ///</summary>
        public string zName;
        ///
        ///<summary>
        ///Name passed to create_module() 
        ///</summary>
        public object pAux;
        ///
        ///<summary>
        ///pAux passed to create_module() 
        ///</summary>
        public smdxDestroy xDestroy;
        //)(void );/* Module destructor function */
    };

}
