using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Community.CsharpSqlite.Ast
{
   ///<summary>
     /// An instance of the following structure describes the event of a
     /// TRIGGER.  "a" is the event type, one of TokenType.TK_UPDATE, TokenType.TK_INSERT,
     /// TokenType.TK_DELETE, or TokenType.TK_INSTEAD.  If the event is of the form
     ///
     ///      UPDATE ON (a,b,c)
     ///
     /// Then the "b" IdList records the list "a,b,c".
     ///
     ///</summary>
    public struct TrigEvent
    {
        public TokenType a;
        public IdList b;
    }
}
