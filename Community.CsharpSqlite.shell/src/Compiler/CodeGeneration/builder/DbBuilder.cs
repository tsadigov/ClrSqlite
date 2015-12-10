using Community.CsharpSqlite.Ast;
using Community.CsharpSqlite.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Community.CsharpSqlite.builder
{
    public static class DbBuilder
    {
        ///<summary>
        /// Parameter zName points to a nul-terminated buffer containing the name
        /// of a database ("main", "temp" or the name of an attached db). This
        /// function returns the index of the named database in db->aDb[], or
        /// -1 if the named db cannot be found.
        ///
        ///</summary>
        public static int sqlite3FindDbName(this Connection db, string zName)
        {
            int i = -1;
            ///
            ///<summary>
            ///Database number 
            ///</summary>
            if (zName != null)
            {
                DbBackend pDb;
                int n = StringExtensions.sqlite3Strlen30(zName);
                for (i = (db.BackendCount - 1); i >= 0; i--)
                {
                    pDb = db.Backends[i];
                    if ((sqliteinth.OMIT_TEMPDB == 0 || i != 1) && n == StringExtensions.sqlite3Strlen30(pDb.Name) && pDb.Name.Equals(zName, StringComparison.InvariantCultureIgnoreCase))
                    {
                        break;
                    }
                }
            }
            return i;
        }
        ///<summary>
        /// The token *pName contains the name of a database (either "main" or
        /// "temp" or the name of an attached db). This routine returns the
        /// index of the named database in db->aDb[], or -1 if the named db
        /// does not exist.
        ///
        ///</summary>
        public static int sqlite3FindDb(this Connection db, Token pName)
        {
            int i;
            ///
            ///<summary>
            ///Database number 
            ///</summary>
            string zName;
            ///
            ///<summary>
            ///Name we are searching for 
            ///</summary>
            zName = build.sqlite3NameFromToken(db, pName);
            i = db.sqlite3FindDbName(zName);
            db.sqlite3DbFree(ref zName);
            return i;
        }
    }
}
