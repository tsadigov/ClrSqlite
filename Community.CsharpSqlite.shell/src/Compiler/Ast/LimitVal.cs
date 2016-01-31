using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Community.CsharpSqlite.Ast
{
    ///<summary>
    /// An instance of this structure holds information about the
    /// LIMIT clause of a SELECT statement.
    ///</summary>
    public struct LimitVal
    {
        public Expr pLimit;
        ///<summary>
        ///The LIMIT expression.  NULL if there is no limit 
        ///</summary>
        public Expr pOffset;
        ///<summary>
        ///The OFFSET expression.  NULL if there is none 
        ///</summary>
    }
}
