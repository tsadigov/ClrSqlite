using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Community.CsharpSqlite
{
  

        ///<summary>
        /// Each token coming out of the lexer is an instance of
        /// this structure.  Tokens are also used as part of an expression.
        ///
        /// Note if Token.z==0 then Token.dyn and Token.n are undefined and
        /// may contain random values.  Do not make any assumptions about Token.dyn
        /// and Token.n when Token.z==0.
        ///
        ///</summary>
        public class Token
        {
            public TokenType TokenType;
            public int Start;
            public String Text;

#if DEBUG_CLASS_TOKEN || DEBUG_CLASS_ALL
																																																																																				public string _z; /* Text of the token.  Not NULL-terminated! */
public bool dyn;//  : 1;      /* True for malloced memory, false for static */
public Int32 _n;//  : 31;     /* Number of characters in this token */

public string z
{
get { return _z; }
set { _z = value; }
}

public Int32 n
{
get { return _n; }
set { _n = value; }
}
#else
            public string zRestSql { get; set; }
            /* Text of the token.  Not NULL-terminated! */
            public Int32 Length;
            ///<summary>
            ///Number of characters in this token
            ///</summary>
#endif
            public Token()
            {
                this.zRestSql = null;
                this.Length = 0;
            }
            public Token(string z, Int32 n)
            {
                this.zRestSql = z;
                this.Length = n;
            }
            public Token Copy()
            {
                if (this == null)
                    return null;
                else
                {
                    Token cp = (Token)MemberwiseClone();
                    if (zRestSql == null || zRestSql.Length == 0)
                        cp.Length = 0;
                    else
                        if (Length > zRestSql.Length)
                            cp.Length = zRestSql.Length;
                    return cp;
                }
            }

            public override string ToString()
            {
                return TokenType+"\t[\t"+Start+"\t:\t"+Length+"\t]\t"+Text+"\tz:"+zRestSql;
            }
        }
    
}
