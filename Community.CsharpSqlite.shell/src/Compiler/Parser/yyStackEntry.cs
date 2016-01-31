using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using u8 = System.Byte;
using YYCODETYPE = System.Int32;
using YYACTIONTYPE = System.Int32;
using Community.CsharpSqlite.Ast;

namespace Community.CsharpSqlite.Parser
{
    ///<summary>
    ///The following structure represents a single element of the
    /// parser's stack.  Information stored includes:
    ///
    ///   +  The state number for the parser at this level of the stack.
    ///
    ///   +  The value of the token stored at this level of the stack.
    ///      (In other words, the "major" token.)
    ///
    ///   +  The semantic value stored at this level of the stack.  This is
    ///      the information used by the action routines in the grammar.
    ///      It is sometimes called the "minor" token.
    ///</summary>
    public class yyStackEntry
    {
        public static Action Print;

        public YYACTIONTYPE stateno;
        ///
        ///<summary>
        ///</summary>
        ///<param name="The state">number </param>
        public TokenType major;
        ///
        ///<summary>
        ///The major token value.  This is the code
        ///number for the token at this stack level 
        ///</summary>
        public YYMINORTYPE minor;
        ///
        ///<summary>
        ///</summary>
        ///<param name="The user">supplied minor token value.  This</param>
        ///<param name="is the value of the token  ">is the value of the token  </param>
    };
}
