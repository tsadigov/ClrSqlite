using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Community.CsharpSqlite.Ast
{
    ///<summary>
    /// An instance of this structure is used to store the LIKE,
    /// GLOB, NOT LIKE, and NOT GLOB operators.
    ///</summary>
    public struct LikeOp
    {
        public Token eOperator;
        ///<summary>
        ///"like" or "glob" or "regexp" 
        ///</summary>
        public bool not;
        ///<summary>
        ///True if the NOT keyword is present 
        ///</summary>
    }
}
