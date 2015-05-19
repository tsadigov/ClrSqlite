using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Community.CsharpSqlite.Metadata
{
    ///
    ///<summary>
    ///During code generation of statements that do inserts into AUTOINCREMENT
    ///tables, the following information is attached to the Table.u.autoInc.p
    ///pointer of each autoincrement table to record some side information that
    ///</summary>
    ///<param name="the code generator needs.  We have to keep per">table autoincrement</param>
    ///<param name="information in case inserts are down within triggers.  Triggers do not">information in case inserts are down within triggers.  Triggers do not</param>
    ///<param name="normally coordinate their activities, but we do need to coordinate the">normally coordinate their activities, but we do need to coordinate the</param>
    ///<param name="loading and saving of autoincrement information.">loading and saving of autoincrement information.</param>
    ///<param name=""></param>
    public class AutoincInfo
    {
        public AutoincInfo pNext;
        ///
        ///<summary>
        ///Next info block in a list of them all 
        ///</summary>
        public Table pTab;
        ///
        ///<summary>
        ///Table this info block refers to 
        ///</summary>
        public int iDb;
        ///
        ///<summary>
        ///Index in sqlite3.aDb[] of database holding pTab 
        ///</summary>
        public int regCtr;
        ///
        ///<summary>
        ///Memory register holding the rowid counter 
        ///</summary>
    };

}
