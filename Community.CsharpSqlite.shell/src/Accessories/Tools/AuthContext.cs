using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Community.CsharpSqlite
{
    ///<summary>
    ///An instance of the following structure can be declared on a stack and used
    ///to save the Parse.zAuthContext value so that it can be restored later.
    ///</summary>
    public class AuthContext
    {
        public string zAuthContext;
        ///
        ///<summary>
        ///Put saved Parse.zAuthContext here 
        ///</summary>
        public Sqlite3.Parse pParse;
        ///
        ///<summary>
        ///The Parse structure 
        ///</summary>
    };

}
