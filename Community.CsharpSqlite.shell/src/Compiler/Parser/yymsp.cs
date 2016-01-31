using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using yyParser = Community.CsharpSqlite.ParseMethods.yyParser;

namespace Community.CsharpSqlite.Parser
{
    ///<summary>
    ///The main parser program.
    /// The first argument is a pointer to a structure obtained from
    /// "sqlite3ParserAlloc" which describes the current state of the parser.
    /// The second argument is the major token number.  The third is
    /// the minor token.  The fourth optional argument is whatever the
    /// user wants (and specified in the grammar) and is available for
    /// use by the action routines.
    ///
    /// Inputs:
    /// <ul>
    /// <li> A pointer to the parser (an opaque structure.)
    /// <li> The major token number.
    /// <li> The minor token number.
    /// <li> An option argument of a grammar-specified type.
    /// </ul>
    ///
    /// Outputs:
    /// None.
    ///
    ///</summary>
    public class OffsettedList
    {
        public yyParser _yyParser;
        public int _yyidx;
        // CONSTRUCTOR
        public OffsettedList(yyParser pointer_to_yyParser, int yyidx)//' Parser and Stack Index
        {
            this._yyParser = pointer_to_yyParser;
            this._yyidx = yyidx;
        }
        // Default Value
        public yyStackEntry this[int offset]
        {
            get
            {
                return _yyParser.yystack[_yyidx + offset];
            }
        }
    }
}
