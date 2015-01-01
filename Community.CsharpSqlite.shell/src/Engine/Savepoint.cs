using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using i64 = System.Int64;

namespace Community.CsharpSqlite
{
    using u64 = System.UInt64;
    ///<summary>
    /// All current savepoints are stored in a linked list starting at
    /// sqlite3.pSavepoint. The first element in the list is the most recently
    /// opened savepoint. Savepoints are added to the list by the vdbe
    /// OP_Savepoint instruction.
    ///
    ///</summary>
    //struct Savepoint {
    //  string zName;                        /* Savepoint name (nul-terminated) */
    //  i64 nDeferredCons;                  /* Number of deferred fk violations */
    //  Savepoint *pNext;                   /* Parent savepoint (if any) */
    //};
    public class Savepoint
    {
        public string zName;
        ///
        ///<summary>
        ///</summary>
        ///<param name="Savepoint name (nul">terminated) </param>
        public i64 nDeferredCons;
        ///
        ///<summary>
        ///Number of deferred fk violations 
        ///</summary>
        public Savepoint pNext;
        ///
        ///<summary>
        ///Parent savepoint (if any) 
        ///</summary>
    };
}
