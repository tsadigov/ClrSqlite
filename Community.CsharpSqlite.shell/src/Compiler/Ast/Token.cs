using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Community.CsharpSqlite.Ast
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
		public string zRestSql {
			get;
			set;
		}

		///
///<summary>
///</summary>
///<param name="Text of the token.  Not NULL">terminated! </param>

		public Int32 Length;

		///<summary>
		///Number of characters in this token
		///</summary>
		#endif
		public Token ()
		{
			this.zRestSql = null;
			this.Length = 0;
		}

		public Token (string z, Int32 n)
		{
			this.zRestSql = z;
			this.Length = n;
		}

		public Token Copy ()
		{
			if (this == null)
				return null;
			else {
				Token cp = (Token)MemberwiseClone ();
				if (zRestSql == null || zRestSql.Length == 0)
					cp.Length = 0;
				else
					if (Length > zRestSql.Length)
						cp.Length = zRestSql.Length;
				return cp;
			}
		}

		public override string ToString ()
		{
			return TokenType + "[ " + Start + " : " + Length + " ] " + Text;
		}
	}


    public enum TokenType : byte
    {
        TK_SEMI = 1,
        TK_EXPLAIN = 2,
        TK_QUERY = 3,
        TK_PLAN = 4,
        TK_BEGIN = 5,
        TK_TRANSACTION = 6,
        TK_DEFERRED = 7,
        TK_IMMEDIATE = 8,
        TK_EXCLUSIVE = 9,
        TK_COMMIT = 10,
        TK_END = 11,
        TK_ROLLBACK = 12,
        TK_SAVEPOINT = 13,
        TK_RELEASE = 14,
        TK_TO = 15,
        TK_TABLE = 16,
        TK_CREATE = 17,
        TK_IF = 18,
        TK_NOT = 19,
        TK_EXISTS = 20,
        TK_TEMP = 21,
        TK_LP = 22,
        TK_RP = 23,
        TK_AS = 24,
        TK_COMMA = 25,
        TK_ID = 26,
        TK_INDEXED = 27,
        TK_ABORT = 28,
        TK_ACTION = 29,
        TK_AFTER = 30,
        TK_ANALYZE = 31,
        TK_ASC = 32,
        TK_ATTACH = 33,
        TK_BEFORE = 34,
        TK_BY = 35,
        TK_CASCADE = 36,
        TK_CAST = 37,
        TK_COLUMNKW = 38,
        TK_CONFLICT = 39,
        TK_DATABASE = 40,
        TK_DESC = 41,
        TK_DETACH = 42,
        TK_EACH = 43,
        TK_FAIL = 44,
        TK_FOR = 45,
        TK_IGNORE = 46,
        TK_INITIALLY = 47,
        TK_INSTEAD = 48,
        TK_LIKE_KW = 49,
        TK_MATCH = 50,
        TK_NO = 51,
        TK_KEY = 52,
        TK_OF = 53,
        TK_OFFSET = 54,
        TK_PRAGMA = 55,
        TK_RAISE = 56,
        TK_REPLACE = 57,
        TK_RESTRICT = 58,
        TK_ROW = 59,
        TK_TRIGGER = 60,
        TK_VACUUM = 61,
        TK_VIEW = 62,
        TK_VIRTUAL = 63,
        TK_REINDEX = 64,
        TK_RENAME = 65,
        TK_CTIME_KW = 66,
        TK_ANY = 67,
        TK_OR = 68,
        TK_AND = 69,
        TK_IS = 70,
        TK_BETWEEN = 71,
        TK_IN = 72,
        TK_ISNULL = 73,
        TK_NOTNULL = 74,
        TK_NE = 75,
        TK_EQ = 76,
        TK_GT = 77,
        TK_LE = 78,
        TK_LT = 79,
        TK_GE = 80,
        TK_ESCAPE = 81,
        TK_BITAND = 82,
        TK_BITOR = 83,
        TK_LSHIFT = 84,
        TK_RSHIFT = 85,
        TK_PLUS = 86,
        TK_MINUS = 87,
        TK_STAR = 88,
        TK_SLASH = 89,
        TK_REM = 90,
        TK_CONCAT = 91,
        TK_COLLATE = 92,
        TK_BITNOT = 93,
        TK_STRING = 94,
        TK_JOIN_KW = 95,
        TK_CONSTRAINT = 96,
        TK_DEFAULT = 97,
        TK_NULL = 98,
        TK_PRIMARY = 99,
        TK_UNIQUE = 100,
        TK_CHECK = 101,
        TK_REFERENCES = 102,
        TK_AUTOINCR = 103,
        TK_ON = 104,
        TK_INSERT = 105,
        TK_DELETE = 106,
        TK_UPDATE = 107,
        TK_SET = 108,
        TK_DEFERRABLE = 109,
        TK_FOREIGN = 110,
        TK_DROP = 111,
        TK_UNION = 112,
        TK_ALL = 113,
        TK_EXCEPT = 114,
        TK_INTERSECT = 115,
        TK_SELECT = 116,
        TK_DISTINCT = 117,
        TK_DOT = 118,
        TK_FROM = 119,
        TK_JOIN = 120,
        TK_USING = 121,
        TK_ORDER = 122,
        TK_GROUP = 123,
        TK_HAVING = 124,
        TK_LIMIT = 125,
        TK_WHERE = 126,
        TK_INTO = 127,
        TK_VALUES = 128,
        TK_INTEGER = 129,
        TK_FLOAT = 130,
        TK_BLOB = 131,
        TK_REGISTER = 132,
        TK_VARIABLE = 133,
        TK_CASE = 134,
        TK_WHEN = 135,
        TK_THEN = 136,
        TK_ELSE = 137,
        TK_INDEX = 138,
        TK_ALTER = 139,
        TK_ADD = 140,
        TK_TO_TEXT = 141,
        TK_TO_BLOB = 142,
        TK_TO_NUMERIC = 143,
        TK_TO_INT = 144,
        TK_TO_REAL = 145,
        TK_ISNOT = 146,
        TK_END_OF_FILE = 147,
        TK_ILLEGAL = 148,
        TK_SPACE = 149,
        TK_UNCLOSED_STRING = 150,
        TK_FUNCTION = 151,
        TK_COLUMN = 152,
        TK_AGG_FUNCTION = 153,
        TK_AGG_COLUMN = 154,
        TK_CONST_FUNC = 155,
        TK_UMINUS = 156,
        TK_UPLUS = 157
    }
}
