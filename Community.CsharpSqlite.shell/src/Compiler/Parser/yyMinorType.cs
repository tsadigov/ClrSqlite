using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using u8 = System.Byte;
using YYCODETYPE = System.Int32;
using YYACTIONTYPE = System.Int32;

namespace Community.CsharpSqlite.Parser
{

    using sqlite3ParserTOKENTYPE = Ast.Token;
    using Community.CsharpSqlite.Ast;
    using Community.CsharpSqlite.Parsing;
    using Community.CsharpSqlite.builder;
    using Community.CsharpSqlite.Metadata;
    using Community.CsharpSqlite.Utils;
    using Parse = Sqlite3.ParseState;

    public class YYMINORTYPE
    {
        public int yyinit;
        public sqlite3ParserTOKENTYPE yy0Token = new sqlite3ParserTOKENTYPE();
        public int yy4_Int;
        public TrigEvent yy90_TrigEvent;
        public ExprSpan yy118_ExprSpan = new ExprSpan();
        public TriggerStep yy203_TriggerStep;
        public u8 yy210;
        public struct _yy215
        {
            public int value;
            public int mask;
        }
        public _yy215 yy215;
        public SrcList yy259_SrcList;
        public LimitVal yy292_LimitVal;
        public Expr yy314_Expr;
        public ExprList _ExprList;
        public LikeOp yy342_LikeOp;
        public IdList yy384_IdList;
        public Select yy387_Select;
    }
}
