using System;
using System.Diagnostics;
using u8=System.Byte;
using YYCODETYPE=System.Int32;
using YYACTIONTYPE=System.Int32;
namespace Community.CsharpSqlite {
    using sqlite3ParserTOKENTYPE = Ast.Token;
    using Community.CsharpSqlite.Ast;
    using Community.CsharpSqlite.Parsing;
    using Community.CsharpSqlite.builder;
    using Community.CsharpSqlite.Utils;
    using ParseState = Sqlite3.ParseState;
    using Compiler.Parser;
    using Parser;
    public  class ParseMethods {

        ///<summary>
        ///Constant tokens for values 0 and 1.
        ///</summary>
        public static Token[] sqlite3IntTokens =  {
            new Token ("0", 1),
            new Token ("1", 1)
        };

        ///<summary>
        ///</summary>
        ///<param name="Included in SQLite3 port to C#">SQLite;  2008 Noah B Hart</param>
        ///<param name="C#">SQLite is an independent reimplementation of the SQLite software library</param>
        ///<param name=""></param>
        ///<param name="SQLITE_SOURCE_ID: 2010">23 18:52:01 42537b60566f288167f1b5864a5435986838e3a3</param>
        ///<param name=""></param>
        ///<param name=""></param>
        ///<param name=""></param>
        ///
        ///<summary>
        ///Driver template for the LEMON parser generator.
        ///The author disclaims copyright to this source code.
        ///
        ///This version of "lempar.c" is modified, slightly, for use by SQLite.
        ///The only modifications are the addition of a couple of NEVER()
        ///macros to disable tests that are needed in the case of a general
        ///LALR(1) grammar but which are always false in the
        ///specific grammar used by SQLite.
        ///
        ///</summary>
        ///
        ///<summary>
        ///First off, code is included that follows the "include" declaration
        ///in the input grammar file. 
        ///</summary>
        //#include <stdio.h>
        //#line 51 "parse.y"
        //#include "sqliteInt.h"
        ///<summary>
        /// Disable all error recovery processing in the parser push-down
        /// automaton.
        ///
        ///</summary>
        //#define YYNOERRORRECOVERY 1
        const int YYNOERRORRECOVERY=1;
		///<summary>
		/// Make yysqliteinth.testcase() the same as sqliteinth.testcase()
		///
		///</summary>
		//#define yysqliteinth.testcase(X) sqliteinth.testcase(X)
		static void yytestcase<T>(T X) {
			sqliteinth.testcase(X);
		}
		
		///<summary>
		/// An instance of this structure holds the ATTACH key and the key type.
		///
		///</summary>
		public struct AttachKey {
			public int type;
			public Token key;
		}
		//#line 722 "parse.y"
		///<summary>
		///This is a utility routine used to set the ExprSpan.zStart and
		/// ExprSpan.zEnd values of pOut so that the span covers the complete
		/// range of text beginning with pStart and going to the end of pEnd.
		///
		///</summary>
		///<summary>
		///Construct a new Expr object from a single identifier.  Use the
		/// new Expr to populate pOut.  Set the span of pOut to be the identifier
		/// that created the expression.
		///
		///</summary>
		//#line 817 "parse.y"
		///<summary>
		///This routine constructs a binary expression node out of two ExprSpan
		/// objects and uses the result to populate a new ExprSpan object.
		///
		///</summary>
		//#line 873 "parse.y"
		///<summary>
		///Construct an expression node for a unary postfix operator
		///
		///</summary>
		//#line 892 "parse.y"
		///<summary>
		///A routine to convert a binary TokenType.TK_IS or TokenType.TK_ISNOT expression into a
		/// unary TokenType.TK_ISNULL or TokenType.TK_NOTNULL expression.
		///</summary>
		//#line 920 "parse.y"
		///
		///<summary>
		///Construct an expression node for a unary prefix operator
		///
		///</summary>
		//#line 141 "parse.c"
		///
		///<summary>
		///Next is all token values, in a form suitable for use by makeheaders.
		///</summary>
		///<param name="This section will be null unless lemon is run with the ">m switch.</param>
		///<param name=""></param>
		///
		///<summary>
		///These constants (all generated automatically by the parser generator)
		///specify the various kinds of tokens (terminals) that the parser
		///understands.
		///
		///Each symbol here is a terminal symbol in the grammar.
		///
		///</summary>
		///
		///<summary>
		///Make sure the INTERFACE macro is defined.
		///
		///</summary>
		#if !INTERFACE
		//# define INTERFACE 1
		#endif
		///<summary>
		///The next thing included is series of defines which control
		/// various aspects of the generated parser.
		///    YYCODETYPE         is the data type used for storing terminal
		///                       and nonterminal numbers.  "unsigned char" is
		///                       used if there are fewer than 250 terminals
		///                       and nonterminals.  "int" is used otherwise.
		///    YYNOCODE           is a number of type YYCODETYPE which corresponds
		///                       to no legal terminal or nonterminal number.  This
		///                       number is used to fill in empty slots of the hash
		///                       table.
		///    YYFALLBACK         If defined, this indicates that one or more tokens
		///                       have fall-back values which should be used if the
		///                       original value of the token will not parse.
		///    YYACTIONTYPE       is the data type used for storing terminal
		///                       and nonterminal numbers.  "unsigned char" is
		///                       used if there are fewer than 250 rules and
		///                       states combined.  "int" is used otherwise.
		///    sqlite3ParserTOKENTYPE     is the data type used for minor tokens given
		///                       directly to the parser from the tokenizer.
		///    YYMINORTYPE        is the data type used for all minor tokens.
		///                       This is typically a union of many types, one of
		///                       which is sqlite3ParserTOKENTYPE.  The entry in the union
		///                       for base tokens is called "yy0".
		///    YYSTACKDEPTH       is the maximum depth of the parser's stack.  If
		///                       zero the stack is dynamically sized using realloc()
		///    sqlite3ParserARG_SDECL     A static variable declaration for the %extra_argument
		///    sqlite3ParserARG_PDECL     A parameter declaration for the %extra_argument
		///    sqlite3ParserARG_STORE     Code to store %extra_argument into yypParser
		///    sqlite3ParserARG_FETCH     Code to extract %extra_argument from yypParser
		///    YYNSTATE           the combined number of states.
		///    YYNRULE            the number of rules in the grammar
		///    YYERRORSYMBOL      is the code number of the error symbol.  If not
		///                       defined, then do no error processing.
		///</summary>
		//#define YYCODETYPE unsigned short char
		const int YYNOCODE_253_VACUUM=253;
		//#define YYACTIONTYPE unsigned short int
		const int YYWILDCARD=67;
		//#define sqlite3ParserTOKENTYPE Token
		
		#if !YYSTACKDEPTH
		const int YYSTACKDEPTH=100;
		#endif
		//#define sqlite3ParserARG_SDECL Parse pParse;
		//#define sqlite3ParserARG_PDECL ,Parse pParse
		//#define sqlite3ParserARG_FETCH Parse pParse = yypParser.pParse
		//#define sqlite3ParserARG_STORE yypParser.pParse = pParse
		const int YYNSTATE=630;
		const int YYNRULE=329;
		//#define YYFALLBACK 1
		const int YYFALLBACK=1;
		const int YY_NO_ACTION=(YYNSTATE+YYNRULE+2);
		const int YY_ACCEPT_ACTION=(YYNSTATE+YYNRULE+1);
		const int YY_ERROR_ACTION=(YYNSTATE+YYNRULE);
		///
		///<summary>
		///The yyzerominor constant is used to initialize instances of
		///YYMINORTYPE objects to zero. 
		///</summary>
		YYMINORTYPE yyzerominor=new YYMINORTYPE();
		//static const YYMINORTYPE yyzerominor = { 0 };
		///
		///<summary>
		///</summary>
		///<param name="Define the yysqliteinth.testcase() macro to be a no">op if is not already defined</param>
		///<param name="otherwise.">otherwise.</param>
		///<param name=""></param>
		///<param name="Applications can choose to define yysqliteinth.testcase() in the %include section">Applications can choose to define yysqliteinth.testcase() in the %include section</param>
		///<param name="to a macro that can assist in verifying code coverage.  For production">to a macro that can assist in verifying code coverage.  For production</param>
		///<param name="code the yysqliteinth.testcase() macro should be turned off.  But it is useful">code the yysqliteinth.testcase() macro should be turned off.  But it is useful</param>
		///<param name="for testing.">for testing.</param>
		///<param name=""></param>
		//#if !yysqliteinth.testcase
		//# define yysqliteinth.testcase(X)
		//#endif
		///
		///<summary>
		///Next are the tables used to determine what action to take based on the
		///current state and lookahead token.  These tables are used to implement
		///functions that take a state number and lookahead value and return an
		///action integer.
		///
		///Suppose the action integer is N.  Then the action is determined as
		///follows
		///
		///0 <= N < YYNSTATE                  Shift N.  That is, push the lookahead
		///token onto the stack and goto state N.
		///
		///</summary>
		///<param name="YYNSTATE <= N < YYNSTATE+YYNRULE   Reduce by rule N">YYNSTATE.</param>
		///<param name=""></param>
		///<param name="N == YYNSTATE+YYNRULE              A syntax error has occurred.">N == YYNSTATE+YYNRULE              A syntax error has occurred.</param>
		///<param name=""></param>
		///<param name="N == YYNSTATE+YYNRULE+1            The parser accepts its input.">N == YYNSTATE+YYNRULE+1            The parser accepts its input.</param>
		///<param name=""></param>
		///<param name="N == YYNSTATE+YYNRULE+2            No such action.  Denotes unused">N == YYNSTATE+YYNRULE+2            No such action.  Denotes unused</param>
		///<param name="slots in the yy_action[] table.">slots in the yy_action[] table.</param>
		///<param name=""></param>
		///<param name="The action table is constructed as a single large table named yy_action[].">The action table is constructed as a single large table named yy_action[].</param>
		///<param name="Given state S and lookahead X, the action is computed as">Given state S and lookahead X, the action is computed as</param>
		///<param name=""></param>
		///<param name="yy_action[ yy_shift_ofst[S] + X ]">yy_action[ yy_shift_ofst[S] + X ]</param>
		///<param name=""></param>
		///<param name="If the index value yy_shift_ofst[S]+X is out of range or if the value">If the index value yy_shift_ofst[S]+X is out of range or if the value</param>
		///<param name="yy_lookahead[yy_shift_ofst[S]+X] is not equal to X or if yy_shift_ofst[S]">yy_lookahead[yy_shift_ofst[S]+X] is not equal to X or if yy_shift_ofst[S]</param>
		///<param name="is equal to YY_SHIFT_USE_DFLT, it means that the action is not in the table">is equal to YY_SHIFT_USE_DFLT, it means that the action is not in the table</param>
		///<param name="and that yy_default[S] should be used instead.">and that yy_default[S] should be used instead.</param>
		///<param name=""></param>
		///<param name="The formula above is for computing the action when the lookahead is">The formula above is for computing the action when the lookahead is</param>
		///<param name="a terminal symbol.  If the lookahead is a non">terminal (as occurs after</param>
		///<param name="a reduce action) then the yy_reduce_ofst[] array is used in place of">a reduce action) then the yy_reduce_ofst[] array is used in place of</param>
		///<param name="the yy_shift_ofst[] array and YY_REDUCE_USE_DFLT is used in place of">the yy_shift_ofst[] array and YY_REDUCE_USE_DFLT is used in place of</param>
		///<param name="YY_SHIFT_USE_DFLT.">YY_SHIFT_USE_DFLT.</param>
		///<param name=""></param>
		///<param name="The following are the tables generated in this section:">The following are the tables generated in this section:</param>
		///<param name=""></param>
		///<param name="yy_action[]        A single table containing all actions.">yy_action[]        A single table containing all actions.</param>
		///<param name="yy_lookahead[]     A table containing the lookahead for each entry in">yy_lookahead[]     A table containing the lookahead for each entry in</param>
		///<param name="yy_action.  Used to detect hash collisions.">yy_action.  Used to detect hash collisions.</param>
		///<param name="yy_shift_ofst[]    For each state, the offset into yy_action for">yy_shift_ofst[]    For each state, the offset into yy_action for</param>
		///<param name="shifting terminals.">shifting terminals.</param>
		///<param name="yy_reduce_ofst[]   For each state, the offset into yy_action for">yy_reduce_ofst[]   For each state, the offset into yy_action for</param>
		///<param name="shifting non">terminals after a reduce.</param>
		///<param name="yy_default[]       Default action for each state.">yy_default[]       Default action for each state.</param>
		///<param name=""></param>
		//#define YY_ACTTAB_COUNT (1557)
		const int YY_ACTTAB_COUNT=1557;
		#region tables
		static YYACTIONTYPE[] yy_action=new YYACTIONTYPE[] {
			///
			///<summary>
			///0 
			///</summary>
			313,
			960,
			186,
			419,
			2,
			172,
			627,
			597,
			55,
			55,
			///
			///<summary>
			///10 
			///</summary>
			55,
			55,
			48,
			53,
			53,
			53,
			53,
			52,
			52,
			51,
			///
			///<summary>
			///20 
			///</summary>
			51,
			51,
			50,
			238,
			302,
			283,
			623,
			622,
			516,
			515,
			///
			///<summary>
			///30 
			///</summary>
			590,
			584,
			55,
			55,
			55,
			55,
			282,
			53,
			53,
			53,
			///
			///<summary>
			///40 
			///</summary>
			53,
			52,
			52,
			51,
			51,
			51,
			50,
			238,
			6,
			56,
			///
			///<summary>
			///50 
			///</summary>
			57,
			47,
			582,
			581,
			583,
			583,
			54,
			54,
			55,
			55,
			///
			///<summary>
			///60 
			///</summary>
			55,
			55,
			608,
			53,
			53,
			53,
			53,
			52,
			52,
			51,
			///
			///<summary>
			///70 
			///</summary>
			51,
			51,
			50,
			238,
			313,
			597,
			409,
			330,
			579,
			579,
			///
			///<summary>
			///80 
			///</summary>
			32,
			53,
			53,
			53,
			53,
			52,
			52,
			51,
			51,
			51,
			///
			///<summary>
			///90 
			///</summary>
			50,
			238,
			330,
			217,
			620,
			619,
			166,
			411,
			624,
			382,
			///
			///<summary>
			///100 
			///</summary>
			379,
			378,
			7,
			491,
			590,
			584,
			200,
			199,
			198,
			58,
			///
			///<summary>
			///110 
			///</summary>
			377,
			300,
			414,
			621,
			481,
			66,
			623,
			622,
			621,
			580,
			///
			///<summary>
			///120 
			///</summary>
			254,
			601,
			94,
			56,
			57,
			47,
			582,
			581,
			583,
			583,
			///
			///<summary>
			///130 
			///</summary>
			54,
			54,
			55,
			55,
			55,
			55,
			671,
			53,
			53,
			53,
			///
			///<summary>
			///140 
			///</summary>
			53,
			52,
			52,
			51,
			51,
			51,
			50,
			238,
			313,
			532,
			///
			///<summary>
			///150 
			///</summary>
			226,
			506,
			507,
			133,
			177,
			139,
			284,
			385,
			279,
			384,
			///
			///<summary>
			///160 
			///</summary>
			169,
			197,
			342,
			398,
			251,
			226,
			253,
			275,
			388,
			167,
			///
			///<summary>
			///170 
			///</summary>
			139,
			284,
			385,
			279,
			384,
			169,
			570,
			236,
			590,
			584,
			///
			///<summary>
			///180 
			///</summary>
			672,
			240,
			275,
			157,
			620,
			619,
			554,
			437,
			51,
			51,
			///
			///<summary>
			///190 
			///</summary>
			51,
			50,
			238,
			343,
			439,
			553,
			438,
			56,
			57,
			47,
			///
			///<summary>
			///200 
			///</summary>
			582,
			581,
			583,
			583,
			54,
			54,
			55,
			55,
			55,
			55,
			///
			///<summary>
			///210 
			///</summary>
			465,
			53,
			53,
			53,
			53,
			52,
			52,
			51,
			51,
			51,
			///
			///<summary>
			///220 
			///</summary>
			50,
			238,
			313,
			390,
			52,
			52,
			51,
			51,
			51,
			50,
			///
			///<summary>
			///230 
			///</summary>
			238,
			391,
			166,
			491,
			566,
			382,
			379,
			378,
			409,
			440,
			///
			///<summary>
			///240 
			///</summary>
			579,
			579,
			252,
			440,
			607,
			66,
			377,
			513,
			621,
			49,
			///
			///<summary>
			///250 
			///</summary>
			46,
			147,
			590,
			584,
			621,
			16,
			466,
			189,
			621,
			441,
			///
			///<summary>
			///260 
			///</summary>
			442,
			673,
			526,
			441,
			340,
			577,
			595,
			64,
			194,
			482,
			///
			///<summary>
			///270 
			///</summary>
			434,
			56,
			57,
			47,
			582,
			581,
			583,
			583,
			54,
			54,
			///
			///<summary>
			///280 
			///</summary>
			55,
			55,
			55,
			55,
			30,
			53,
			53,
			53,
			53,
			52,
			///
			///<summary>
			///290 
			///</summary>
			52,
			51,
			51,
			51,
			50,
			238,
			313,
			593,
			593,
			593,
			///
			///<summary>
			///300 
			///</summary>
			387,
			578,
			606,
			493,
			259,
			351,
			258,
			411,
			1,
			623,
			///
			///<summary>
			///310 
			///</summary>
			622,
			496,
			623,
			622,
			65,
			240,
			623,
			622,
			597,
			443,
			///
			///<summary>
			///320 
			///</summary>
			237,
			239,
			414,
			341,
			237,
			602,
			590,
			584,
			18,
			603,
			///
			///<summary>
			///330 
			///</summary>
			166,
			601,
			87,
			382,
			379,
			378,
			67,
			623,
			622,
			38,
			///
			///<summary>
			///340 
			///</summary>
			623,
			622,
			176,
			270,
			377,
			56,
			57,
			47,
			582,
			581,
			///
			///<summary>
			///350 
			///</summary>
			583,
			583,
			54,
			54,
			55,
			55,
			55,
			55,
			175,
			53,
			///
			///<summary>
			///360 
			///</summary>
			53,
			53,
			53,
			52,
			52,
			51,
			51,
			51,
			50,
			238,
			///
			///<summary>
			///370 
			///</summary>
			313,
			396,
			233,
			411,
			531,
			565,
			317,
			620,
			619,
			44,
			///
			///<summary>
			///380 
			///</summary>
			620,
			619,
			240,
			206,
			620,
			619,
			597,
			266,
			414,
			268,
			///
			///<summary>
			///390 
			///</summary>
			409,
			597,
			579,
			579,
			352,
			184,
			505,
			601,
			73,
			533,
			///
			///<summary>
			///400 
			///</summary>
			590,
			584,
			466,
			548,
			190,
			620,
			619,
			576,
			620,
			619,
			///
			///<summary>
			///410 
			///</summary>
			547,
			383,
			551,
			35,
			332,
			575,
			574,
			600,
			504,
			56,
			///
			///<summary>
			///420 
			///</summary>
			57,
			47,
			582,
			581,
			583,
			583,
			54,
			54,
			55,
			55,
			///
			///<summary>
			///430 
			///</summary>
			55,
			55,
			567,
			53,
			53,
			53,
			53,
			52,
			52,
			51,
			///
			///<summary>
			///440 
			///</summary>
			51,
			51,
			50,
			238,
			313,
			411,
			561,
			561,
			528,
			364,
			///
			///<summary>
			///450 
			///</summary>
			259,
			351,
			258,
			183,
			361,
			549,
			524,
			374,
			411,
			597,
			///
			///<summary>
			///460 
			///</summary>
			414,
			240,
			560,
			560,
			409,
			604,
			579,
			579,
			328,
			601,
			///
			///<summary>
			///470 
			///</summary>
			93,
			623,
			622,
			414,
			590,
			584,
			237,
			564,
			559,
			559,
			///
			///<summary>
			///480 
			///</summary>
			520,
			402,
			601,
			87,
			409,
			210,
			579,
			579,
			168,
			421,
			///
			///<summary>
			///490 
			///</summary>
			950,
			519,
			950,
			56,
			57,
			47,
			582,
			581,
			583,
			583,
			///
			///<summary>
			///500 
			///</summary>
			54,
			54,
			55,
			55,
			55,
			55,
			192,
			53,
			53,
			53,
			///
			///<summary>
			///510 
			///</summary>
			53,
			52,
			52,
			51,
			51,
			51,
			50,
			238,
			313,
			600,
			///
			///<summary>
			///520 
			///</summary>
			293,
			563,
			511,
			234,
			357,
			146,
			475,
			475,
			367,
			411,
			///
			///<summary>
			///530 
			///</summary>
			562,
			411,
			358,
			542,
			425,
			171,
			411,
			215,
			144,
			620,
			///
			///<summary>
			///540 
			///</summary>
			619,
			544,
			318,
			353,
			414,
			203,
			414,
			275,
			590,
			584,
			///
			///<summary>
			///550 
			///</summary>
			549,
			414,
			174,
			601,
			94,
			601,
			79,
			558,
			471,
			61,
			///
			///<summary>
			///560 
			///</summary>
			601,
			79,
			421,
			949,
			350,
			949,
			34,
			56,
			57,
			47,
			///
			///<summary>
			///570 
			///</summary>
			582,
			581,
			583,
			583,
			54,
			54,
			55,
			55,
			55,
			55,
			///
			///<summary>
			///580 
			///</summary>
			535,
			53,
			53,
			53,
			53,
			52,
			52,
			51,
			51,
			51,
			///
			///<summary>
			///590 
			///</summary>
			50,
			238,
			313,
			307,
			424,
			394,
			272,
			49,
			46,
			147,
			///
			///<summary>
			///600 
			///</summary>
			349,
			322,
			4,
			411,
			491,
			312,
			321,
			425,
			568,
			492,
			///
			///<summary>
			///610 
			///</summary>
			216,
			264,
			407,
			575,
			574,
			429,
			66,
			549,
			414,
			621,
			///
			///<summary>
			///620 
			///</summary>
			540,
			602,
			590,
			584,
			13,
			603,
			621,
			601,
			72,
			12,
			///
			///<summary>
			///630 
			///</summary>
			618,
			617,
			616,
			202,
			210,
			621,
			546,
			469,
			422,
			319,
			///
			///<summary>
			///640 
			///</summary>
			148,
			56,
			57,
			47,
			582,
			581,
			583,
			583,
			54,
			54,
			///
			///<summary>
			///650 
			///</summary>
			55,
			55,
			55,
			55,
			338,
			53,
			53,
			53,
			53,
			52,
			///
			///<summary>
			///660 
			///</summary>
			52,
			51,
			51,
			51,
			50,
			238,
			313,
			600,
			600,
			411,
			///
			///<summary>
			///670 
			///</summary>
			39,
			21,
			37,
			170,
			237,
			875,
			411,
			572,
			572,
			201,
			///
			///<summary>
			///680 
			///</summary>
			144,
			473,
			538,
			331,
			414,
			474,
			143,
			146,
			630,
			628,
			///
			///<summary>
			///690 
			///</summary>
			334,
			414,
			353,
			601,
			68,
			168,
			590,
			584,
			132,
			365,
			///
			///<summary>
			///700 
			///</summary>
			601,
			96,
			307,
			423,
			530,
			336,
			49,
			46,
			147,
			568,
			///
			///<summary>
			///710 
			///</summary>
			406,
			216,
			549,
			360,
			529,
			56,
			57,
			47,
			582,
			581,
			///
			///<summary>
			///720 
			///</summary>
			583,
			583,
			54,
			54,
			55,
			55,
			55,
			55,
			411,
			53,
			///
			///<summary>
			///730 
			///</summary>
			53,
			53,
			53,
			52,
			52,
			51,
			51,
			51,
			50,
			238,
			///
			///<summary>
			///740 
			///</summary>
			313,
			411,
			605,
			414,
			484,
			510,
			172,
			422,
			597,
			318,
			///
			///<summary>
			///750 
			///</summary>
			496,
			485,
			601,
			99,
			411,
			142,
			414,
			411,
			231,
			411,
			///
			///<summary>
			///760 
			///</summary>
			540,
			411,
			359,
			629,
			2,
			601,
			97,
			426,
			308,
			414,
			///
			///<summary>
			///770 
			///</summary>
			590,
			584,
			414,
			20,
			414,
			621,
			414,
			621,
			601,
			106,
			///
			///<summary>
			///780 
			///</summary>
			503,
			601,
			105,
			601,
			108,
			601,
			109,
			204,
			28,
			56,
			///
			///<summary>
			///790 
			///</summary>
			57,
			47,
			582,
			581,
			583,
			583,
			54,
			54,
			55,
			55,
			///
			///<summary>
			///800 
			///</summary>
			55,
			55,
			411,
			53,
			53,
			53,
			53,
			52,
			52,
			51,
			///
			///<summary>
			///810 
			///</summary>
			51,
			51,
			50,
			238,
			313,
			411,
			597,
			414,
			411,
			276,
			///
			///<summary>
			///820 
			///</summary>
			214,
			600,
			411,
			366,
			213,
			381,
			601,
			134,
			274,
			500,
			///
			///<summary>
			///830 
			///</summary>
			414,
			167,
			130,
			414,
			621,
			411,
			354,
			414,
			376,
			601,
			///
			///<summary>
			///840 
			///</summary>
			135,
			129,
			601,
			100,
			590,
			584,
			601,
			104,
			522,
			521,
			///
			///<summary>
			///850 
			///</summary>
			414,
			621,
			224,
			273,
			600,
			167,
			327,
			282,
			600,
			601,
			///
			///<summary>
			///860 
			///</summary>
			103,
			468,
			521,
			56,
			57,
			47,
			582,
			581,
			583,
			583,
			///
			///<summary>
			///870 
			///</summary>
			54,
			54,
			55,
			55,
			55,
			55,
			411,
			53,
			53,
			53,
			///
			///<summary>
			///880 
			///</summary>
			53,
			52,
			52,
			51,
			51,
			51,
			50,
			238,
			313,
			411,
			///
			///<summary>
			///890 
			///</summary>
			27,
			414,
			411,
			375,
			276,
			167,
			359,
			544,
			50,
			238,
			///
			///<summary>
			///900 
			///</summary>
			601,
			95,
			128,
			223,
			414,
			411,
			165,
			414,
			411,
			621,
			///
			///<summary>
			///910 
			///</summary>
			411,
			621,
			612,
			601,
			102,
			372,
			601,
			76,
			590,
			584,
			///
			///<summary>
			///920 
			///</summary>
			414,
			570,
			236,
			414,
			470,
			414,
			167,
			621,
			188,
			601,
			///
			///<summary>
			///930 
			///</summary>
			98,
			225,
			601,
			138,
			601,
			137,
			232,
			56,
			45,
			47,
			///
			///<summary>
			///940 
			///</summary>
			582,
			581,
			583,
			583,
			54,
			54,
			55,
			55,
			55,
			55,
			///
			///<summary>
			///950 
			///</summary>
			411,
			53,
			53,
			53,
			53,
			52,
			52,
			51,
			51,
			51,
			///
			///<summary>
			///960 
			///</summary>
			50,
			238,
			313,
			276,
			276,
			414,
			411,
			276,
			544,
			459,
			///
			///<summary>
			///970 
			///</summary>
			359,
			171,
			209,
			479,
			601,
			136,
			628,
			334,
			621,
			621,
			///
			///<summary>
			///980 
			///</summary>
			125,
			414,
			621,
			368,
			411,
			621,
			257,
			540,
			589,
			588,
			///
			///<summary>
			///990 
			///</summary>
			601,
			75,
			590,
			584,
			458,
			446,
			23,
			23,
			124,
			414,
			///
			///<summary>
			///1000 
			///</summary>
			326,
			325,
			621,
			427,
			324,
			309,
			600,
			288,
			601,
			92,
			///
			///<summary>
			///1010 
			///</summary>
			586,
			585,
			57,
			47,
			582,
			581,
			583,
			583,
			54,
			54,
			///
			///<summary>
			///1020 
			///</summary>
			55,
			55,
			55,
			55,
			411,
			53,
			53,
			53,
			53,
			52,
			///
			///<summary>
			///1030 
			///</summary>
			52,
			51,
			51,
			51,
			50,
			238,
			313,
			587,
			411,
			414,
			///
			///<summary>
			///1040 
			///</summary>
			411,
			207,
			611,
			476,
			171,
			472,
			160,
			123,
			601,
			91,
			///
			///<summary>
			///1050 
			///</summary>
			323,
			261,
			15,
			414,
			464,
			414,
			411,
			621,
			411,
			354,
			///
			///<summary>
			///1060 
			///</summary>
			222,
			411,
			601,
			74,
			601,
			90,
			590,
			584,
			159,
			264,
			///
			///<summary>
			///1070 
			///</summary>
			158,
			414,
			461,
			414,
			621,
			600,
			414,
			121,
			120,
			25,
			///
			///<summary>
			///1080 
			///</summary>
			601,
			89,
			601,
			101,
			621,
			601,
			88,
			47,
			582,
			581,
			///
			///<summary>
			///1090 
			///</summary>
			583,
			583,
			54,
			54,
			55,
			55,
			55,
			55,
			544,
			53,
			///
			///<summary>
			///1100 
			///</summary>
			53,
			53,
			53,
			52,
			52,
			51,
			51,
			51,
			50,
			238,
			///
			///<summary>
			///1110 
			///</summary>
			43,
			405,
			263,
			3,
			610,
			264,
			140,
			415,
			622,
			24,
			///
			///<summary>
			///1120 
			///</summary>
			410,
			11,
			456,
			594,
			118,
			155,
			219,
			452,
			408,
			621,
			///
			///<summary>
			///1130 
			///</summary>
			621,
			621,
			156,
			43,
			405,
			621,
			3,
			286,
			621,
			113,
			///
			///<summary>
			///1140 
			///</summary>
			415,
			622,
			111,
			445,
			411,
			400,
			557,
			403,
			545,
			10,
			///
			///<summary>
			///1150 
			///</summary>
			411,
			408,
			264,
			110,
			205,
			436,
			541,
			566,
			453,
			414,
			///
			///<summary>
			///1160 
			///</summary>
			621,
			621,
			63,
			621,
			435,
			414,
			411,
			621,
			601,
			94,
			///
			///<summary>
			///1170 
			///</summary>
			403,
			621,
			411,
			337,
			601,
			86,
			150,
			40,
			41,
			534,
			///
			///<summary>
			///1180 
			///</summary>
			566,
			414,
			242,
			264,
			42,
			413,
			412,
			414,
			600,
			595,
			///
			///<summary>
			///1190 
			///</summary>
			601,
			85,
			191,
			333,
			107,
			451,
			601,
			84,
			621,
			539,
			///
			///<summary>
			///1200 
			///</summary>
			40,
			41,
			420,
			230,
			411,
			149,
			316,
			42,
			413,
			412,
			///
			///<summary>
			///1210 
			///</summary>
			398,
			127,
			595,
			315,
			621,
			399,
			278,
			625,
			181,
			414,
			///
			///<summary>
			///1220 
			///</summary>
			593,
			593,
			593,
			592,
			591,
			14,
			450,
			411,
			601,
			71,
			///
			///<summary>
			///1230 
			///</summary>
			240,
			621,
			43,
			405,
			264,
			3,
			615,
			180,
			264,
			415,
			///
			///<summary>
			///1240 
			///</summary>
			622,
			614,
			414,
			593,
			593,
			593,
			592,
			591,
			14,
			621,
			///
			///<summary>
			///1250 
			///</summary>
			408,
			601,
			70,
			621,
			417,
			33,
			405,
			613,
			3,
			411,
			///
			///<summary>
			///1260 
			///</summary>
			264,
			411,
			415,
			622,
			418,
			626,
			178,
			509,
			8,
			403,
			///
			///<summary>
			///1270 
			///</summary>
			241,
			416,
			126,
			408,
			414,
			621,
			414,
			449,
			208,
			566,
			///
			///<summary>
			///1280 
			///</summary>
			240,
			221,
			621,
			601,
			83,
			601,
			82,
			599,
			297,
			277,
			///
			///<summary>
			///1290 
			///</summary>
			296,
			30,
			403,
			31,
			395,
			264,
			295,
			397,
			489,
			40,
			///
			///<summary>
			///1300 
			///</summary>
			41,
			411,
			566,
			220,
			621,
			294,
			42,
			413,
			412,
			271,
			///
			///<summary>
			///1310 
			///</summary>
			621,
			595,
			600,
			621,
			59,
			60,
			414,
			269,
			267,
			623,
			///
			///<summary>
			///1320 
			///</summary>
			622,
			36,
			40,
			41,
			621,
			601,
			81,
			598,
			235,
			42,
			///
			///<summary>
			///1330 
			///</summary>
			413,
			412,
			621,
			621,
			595,
			265,
			344,
			411,
			248,
			556,
			///
			///<summary>
			///1340 
			///</summary>
			173,
			185,
			593,
			593,
			593,
			592,
			591,
			14,
			218,
			29,
			///
			///<summary>
			///1350 
			///</summary>
			621,
			543,
			414,
			305,
			304,
			303,
			179,
			301,
			411,
			566,
			///
			///<summary>
			///1360 
			///</summary>
			454,
			601,
			80,
			289,
			335,
			593,
			593,
			593,
			592,
			591,
			///
			///<summary>
			///1370 
			///</summary>
			14,
			411,
			287,
			414,
			151,
			392,
			246,
			260,
			411,
			196,
			///
			///<summary>
			///1380 
			///</summary>
			195,
			523,
			601,
			69,
			411,
			245,
			414,
			526,
			537,
			285,
			///
			///<summary>
			///1390 
			///</summary>
			389,
			595,
			621,
			414,
			536,
			601,
			17,
			362,
			153,
			414,
			///
			///<summary>
			///1400 
			///</summary>
			466,
			463,
			601,
			78,
			154,
			414,
			462,
			152,
			601,
			77,
			///
			///<summary>
			///1410 
			///</summary>
			355,
			255,
			621,
			455,
			601,
			9,
			621,
			386,
			444,
			517,
			///
			///<summary>
			///1420 
			///</summary>
			247,
			621,
			593,
			593,
			593,
			621,
			621,
			244,
			621,
			243,
			///
			///<summary>
			///1430 
			///</summary>
			430,
			518,
			292,
			621,
			329,
			621,
			145,
			393,
			280,
			513,
			///
			///<summary>
			///1440 
			///</summary>
			291,
			131,
			621,
			514,
			621,
			621,
			311,
			621,
			259,
			346,
			///
			///<summary>
			///1450 
			///</summary>
			249,
			621,
			621,
			229,
			314,
			621,
			228,
			512,
			227,
			240,
			///
			///<summary>
			///1460 
			///</summary>
			494,
			488,
			310,
			164,
			487,
			486,
			373,
			480,
			163,
			262,
			///
			///<summary>
			///1470 
			///</summary>
			369,
			371,
			162,
			26,
			212,
			478,
			477,
			161,
			141,
			363,
			///
			///<summary>
			///1480 
			///</summary>
			467,
			122,
			339,
			187,
			119,
			348,
			347,
			117,
			116,
			115,
			///
			///<summary>
			///1490 
			///</summary>
			114,
			112,
			182,
			457,
			320,
			22,
			433,
			432,
			448,
			19,
			///
			///<summary>
			///1500 
			///</summary>
			609,
			431,
			428,
			62,
			193,
			596,
			573,
			298,
			555,
			552,
			///
			///<summary>
			///1510 
			///</summary>
			571,
			404,
			290,
			380,
			498,
			510,
			495,
			306,
			281,
			499,
			///
			///<summary>
			///1520 
			///</summary>
			250,
			5,
			497,
			460,
			345,
			447,
			569,
			550,
			238,
			299,
			///
			///<summary>
			///1530 
			///</summary>
			527,
			525,
			508,
			961,
			502,
			501,
			961,
			401,
			961,
			211,
			///
			///<summary>
			///1540 
			///</summary>
			490,
			356,
			256,
			961,
			483,
			961,
			961,
			961,
			961,
			961,
			///
			///<summary>
			///1550 
			///</summary>
			961,
			961,
			961,
			961,
			961,
			961,
			370,
		};
		static YYCODETYPE[] yy_lookahead=new YYCODETYPE[] {
			///
			///<summary>
			///0 
			///</summary>
			19,
			142,
			143,
			144,
			145,
			24,
			1,
			26,
			77,
			78,
			///
			///<summary>
			///10 
			///</summary>
			79,
			80,
			81,
			82,
			83,
			84,
			85,
			86,
			87,
			88,
			///
			///<summary>
			///20 
			///</summary>
			89,
			90,
			91,
			92,
			15,
			98,
			26,
			27,
			7,
			8,
			///
			///<summary>
			///30 
			///</summary>
			49,
			50,
			77,
			78,
			79,
			80,
			109,
			82,
			83,
			84,
			///
			///<summary>
			///40 
			///</summary>
			85,
			86,
			87,
			88,
			89,
			90,
			91,
			92,
			22,
			68,
			///
			///<summary>
			///50 
			///</summary>
			69,
			70,
			71,
			72,
			73,
			74,
			75,
			76,
			77,
			78,
			///
			///<summary>
			///60 
			///</summary>
			79,
			80,
			23,
			82,
			83,
			84,
			85,
			86,
			87,
			88,
			///
			///<summary>
			///70 
			///</summary>
			89,
			90,
			91,
			92,
			19,
			94,
			112,
			19,
			114,
			115,
			///
			///<summary>
			///80 
			///</summary>
			25,
			82,
			83,
			84,
			85,
			86,
			87,
			88,
			89,
			90,
			///
			///<summary>
			///90 
			///</summary>
			91,
			92,
			19,
			22,
			94,
			95,
			96,
			150,
			150,
			99,
			///
			///<summary>
			///100 
			///</summary>
			100,
			101,
			76,
			150,
			49,
			50,
			105,
			106,
			107,
			54,
			///
			///<summary>
			///110 
			///</summary>
			110,
			158,
			165,
			165,
			161,
			162,
			26,
			27,
			165,
			113,
			///
			///<summary>
			///120 
			///</summary>
			16,
			174,
			175,
			68,
			69,
			70,
			71,
			72,
			73,
			74,
			///
			///<summary>
			///130 
			///</summary>
			75,
			76,
			77,
			78,
			79,
			80,
			118,
			82,
			83,
			84,
			///
			///<summary>
			///140 
			///</summary>
			85,
			86,
			87,
			88,
			89,
			90,
			91,
			92,
			19,
			23,
			///
			///<summary>
			///150 
			///</summary>
			92,
			97,
			98,
			24,
			96,
			97,
			98,
			99,
			100,
			101,
			///
			///<summary>
			///160 
			///</summary>
			102,
			25,
			97,
			216,
			60,
			92,
			62,
			109,
			221,
			25,
			///
			///<summary>
			///170 
			///</summary>
			97,
			98,
			99,
			100,
			101,
			102,
			86,
			87,
			49,
			50,
			///
			///<summary>
			///180 
			///</summary>
			118,
			116,
			109,
			25,
			94,
			95,
			32,
			97,
			88,
			89,
			///
			///<summary>
			///190 
			///</summary>
			90,
			91,
			92,
			128,
			104,
			41,
			106,
			68,
			69,
			70,
			///
			///<summary>
			///200 
			///</summary>
			71,
			72,
			73,
			74,
			75,
			76,
			77,
			78,
			79,
			80,
			///
			///<summary>
			///210 
			///</summary>
			11,
			82,
			83,
			84,
			85,
			86,
			87,
			88,
			89,
			90,
			///
			///<summary>
			///220 
			///</summary>
			91,
			92,
			19,
			19,
			86,
			87,
			88,
			89,
			90,
			91,
			///
			///<summary>
			///230 
			///</summary>
			92,
			27,
			96,
			150,
			66,
			99,
			100,
			101,
			112,
			150,
			///
			///<summary>
			///240 
			///</summary>
			114,
			115,
			138,
			150,
			161,
			162,
			110,
			103,
			165,
			222,
			///
			///<summary>
			///250 
			///</summary>
			223,
			224,
			49,
			50,
			165,
			22,
			57,
			24,
			165,
			170,
			///
			///<summary>
			///260 
			///</summary>
			171,
			118,
			94,
			170,
			171,
			23,
			98,
			25,
			185,
			186,
			///
			///<summary>
			///270 
			///</summary>
			243,
			68,
			69,
			70,
			71,
			72,
			73,
			74,
			75,
			76,
			///
			///<summary>
			///280 
			///</summary>
			77,
			78,
			79,
			80,
			126,
			82,
			83,
			84,
			85,
			86,
			///
			///<summary>
			///290 
			///</summary>
			87,
			88,
			89,
			90,
			91,
			92,
			19,
			129,
			130,
			131,
			///
			///<summary>
			///300 
			///</summary>
			88,
			23,
			172,
			173,
			105,
			106,
			107,
			150,
			22,
			26,
			///
			///<summary>
			///310 
			///</summary>
			27,
			181,
			26,
			27,
			22,
			116,
			26,
			27,
			26,
			230,
			///
			///<summary>
			///320 
			///</summary>
			231,
			197,
			165,
			230,
			231,
			113,
			49,
			50,
			204,
			117,
			///
			///<summary>
			///330 
			///</summary>
			96,
			174,
			175,
			99,
			100,
			101,
			22,
			26,
			27,
			136,
			///
			///<summary>
			///340 
			///</summary>
			26,
			27,
			118,
			16,
			110,
			68,
			69,
			70,
			71,
			72,
			///
			///<summary>
			///350 
			///</summary>
			73,
			74,
			75,
			76,
			77,
			78,
			79,
			80,
			118,
			82,
			///
			///<summary>
			///360 
			///</summary>
			83,
			84,
			85,
			86,
			87,
			88,
			89,
			90,
			91,
			92,
			///
			///<summary>
			///370 
			///</summary>
			19,
			214,
			215,
			150,
			23,
			23,
			155,
			94,
			95,
			22,
			///
			///<summary>
			///380 
			///</summary>
			94,
			95,
			116,
			160,
			94,
			95,
			94,
			60,
			165,
			62,
			///
			///<summary>
			///390 
			///</summary>
			112,
			26,
			114,
			115,
			128,
			23,
			36,
			174,
			175,
			88,
			///
			///<summary>
			///400 
			///</summary>
			49,
			50,
			57,
			120,
			22,
			94,
			95,
			23,
			94,
			95,
			///
			///<summary>
			///410 
			///</summary>
			120,
			51,
			25,
			136,
			169,
			170,
			171,
			194,
			58,
			68,
			///
			///<summary>
			///420 
			///</summary>
			69,
			70,
			71,
			72,
			73,
			74,
			75,
			76,
			77,
			78,
			///
			///<summary>
			///430 
			///</summary>
			79,
			80,
			23,
			82,
			83,
			84,
			85,
			86,
			87,
			88,
			///
			///<summary>
			///440 
			///</summary>
			89,
			90,
			91,
			92,
			19,
			150,
			12,
			12,
			23,
			228,
			///
			///<summary>
			///450 
			///</summary>
			105,
			106,
			107,
			23,
			233,
			25,
			165,
			19,
			150,
			94,
			///
			///<summary>
			///460 
			///</summary>
			165,
			116,
			28,
			28,
			112,
			174,
			114,
			115,
			108,
			174,
			///
			///<summary>
			///470 
			///</summary>
			175,
			26,
			27,
			165,
			49,
			50,
			231,
			11,
			44,
			44,
			///
			///<summary>
			///480 
			///</summary>
			46,
			46,
			174,
			175,
			112,
			160,
			114,
			115,
			50,
			22,
			///
			///<summary>
			///490 
			///</summary>
			23,
			57,
			25,
			68,
			69,
			70,
			71,
			72,
			73,
			74,
			///
			///<summary>
			///500 
			///</summary>
			75,
			76,
			77,
			78,
			79,
			80,
			119,
			82,
			83,
			84,
			///
			///<summary>
			///510 
			///</summary>
			85,
			86,
			87,
			88,
			89,
			90,
			91,
			92,
			19,
			194,
			///
			///<summary>
			///520 
			///</summary>
			225,
			23,
			23,
			215,
			19,
			95,
			105,
			106,
			107,
			150,
			///
			///<summary>
			///530 
			///</summary>
			23,
			150,
			27,
			23,
			67,
			25,
			150,
			206,
			207,
			94,
			///
			///<summary>
			///540 
			///</summary>
			95,
			166,
			104,
			218,
			165,
			22,
			165,
			109,
			49,
			50,
			///
			///<summary>
			///550 
			///</summary>
			120,
			165,
			25,
			174,
			175,
			174,
			175,
			23,
			21,
			234,
			///
			///<summary>
			///560 
			///</summary>
			174,
			175,
			22,
			23,
			239,
			25,
			25,
			68,
			69,
			70,
			///
			///<summary>
			///570 
			///</summary>
			71,
			72,
			73,
			74,
			75,
			76,
			77,
			78,
			79,
			80,
			///
			///<summary>
			///580 
			///</summary>
			205,
			82,
			83,
			84,
			85,
			86,
			87,
			88,
			89,
			90,
			///
			///<summary>
			///590 
			///</summary>
			91,
			92,
			19,
			22,
			23,
			216,
			23,
			222,
			223,
			224,
			///
			///<summary>
			///600 
			///</summary>
			63,
			220,
			35,
			150,
			150,
			163,
			220,
			67,
			166,
			167,
			///
			///<summary>
			///610 
			///</summary>
			168,
			150,
			169,
			170,
			171,
			161,
			162,
			25,
			165,
			165,
			///
			///<summary>
			///620 
			///</summary>
			150,
			113,
			49,
			50,
			25,
			117,
			165,
			174,
			175,
			35,
			///
			///<summary>
			///630 
			///</summary>
			7,
			8,
			9,
			160,
			160,
			165,
			120,
			100,
			67,
			247,
			///
			///<summary>
			///640 
			///</summary>
			248,
			68,
			69,
			70,
			71,
			72,
			73,
			74,
			75,
			76,
			///
			///<summary>
			///650 
			///</summary>
			77,
			78,
			79,
			80,
			193,
			82,
			83,
			84,
			85,
			86,
			///
			///<summary>
			///660 
			///</summary>
			87,
			88,
			89,
			90,
			91,
			92,
			19,
			194,
			194,
			150,
			///
			///<summary>
			///670 
			///</summary>
			135,
			24,
			137,
			35,
			231,
			138,
			150,
			129,
			130,
			206,
			///
			///<summary>
			///680 
			///</summary>
			207,
			30,
			27,
			213,
			165,
			34,
			118,
			95,
			0,
			1,
			///
			///<summary>
			///690 
			///</summary>
			2,
			165,
			218,
			174,
			175,
			50,
			49,
			50,
			22,
			48,
			///
			///<summary>
			///700 
			///</summary>
			174,
			175,
			22,
			23,
			23,
			244,
			222,
			223,
			224,
			166,
			///
			///<summary>
			///710 
			///</summary>
			167,
			168,
			120,
			239,
			23,
			68,
			69,
			70,
			71,
			72,
			///
			///<summary>
			///720 
			///</summary>
			73,
			74,
			75,
			76,
			77,
			78,
			79,
			80,
			150,
			82,
			///
			///<summary>
			///730 
			///</summary>
			83,
			84,
			85,
			86,
			87,
			88,
			89,
			90,
			91,
			92,
			///
			///<summary>
			///740 
			///</summary>
			19,
			150,
			173,
			165,
			181,
			182,
			24,
			67,
			26,
			104,
			///
			///<summary>
			///750 
			///</summary>
			181,
			188,
			174,
			175,
			150,
			39,
			165,
			150,
			52,
			150,
			///
			///<summary>
			///760 
			///</summary>
			150,
			150,
			150,
			144,
			145,
			174,
			175,
			249,
			250,
			165,
			///
			///<summary>
			///770 
			///</summary>
			49,
			50,
			165,
			52,
			165,
			165,
			165,
			165,
			174,
			175,
			///
			///<summary>
			///780 
			///</summary>
			29,
			174,
			175,
			174,
			175,
			174,
			175,
			160,
			22,
			68,
			///
			///<summary>
			///790 
			///</summary>
			69,
			70,
			71,
			72,
			73,
			74,
			75,
			76,
			77,
			78,
			///
			///<summary>
			///800 
			///</summary>
			79,
			80,
			150,
			82,
			83,
			84,
			85,
			86,
			87,
			88,
			///
			///<summary>
			///810 
			///</summary>
			89,
			90,
			91,
			92,
			19,
			150,
			94,
			165,
			150,
			150,
			///
			///<summary>
			///820 
			///</summary>
			160,
			194,
			150,
			213,
			160,
			52,
			174,
			175,
			23,
			23,
			///
			///<summary>
			///830 
			///</summary>
			165,
			25,
			22,
			165,
			165,
			150,
			150,
			165,
			52,
			174,
			///
			///<summary>
			///840 
			///</summary>
			175,
			22,
			174,
			175,
			49,
			50,
			174,
			175,
			190,
			191,
			///
			///<summary>
			///850 
			///</summary>
			165,
			165,
			240,
			23,
			194,
			25,
			187,
			109,
			194,
			174,
			///
			///<summary>
			///860 
			///</summary>
			175,
			190,
			191,
			68,
			69,
			70,
			71,
			72,
			73,
			74,
			///
			///<summary>
			///870 
			///</summary>
			75,
			76,
			77,
			78,
			79,
			80,
			150,
			82,
			83,
			84,
			///
			///<summary>
			///880 
			///</summary>
			85,
			86,
			87,
			88,
			89,
			90,
			91,
			92,
			19,
			150,
			///
			///<summary>
			///890 
			///</summary>
			22,
			165,
			150,
			23,
			150,
			25,
			150,
			166,
			91,
			92,
			///
			///<summary>
			///900 
			///</summary>
			174,
			175,
			22,
			217,
			165,
			150,
			102,
			165,
			150,
			165,
			///
			///<summary>
			///910 
			///</summary>
			150,
			165,
			150,
			174,
			175,
			19,
			174,
			175,
			49,
			50,
			///
			///<summary>
			///920 
			///</summary>
			165,
			86,
			87,
			165,
			23,
			165,
			25,
			165,
			24,
			174,
			///
			///<summary>
			///930 
			///</summary>
			175,
			187,
			174,
			175,
			174,
			175,
			205,
			68,
			69,
			70,
			///
			///<summary>
			///940 
			///</summary>
			71,
			72,
			73,
			74,
			75,
			76,
			77,
			78,
			79,
			80,
			///
			///<summary>
			///950 
			///</summary>
			150,
			82,
			83,
			84,
			85,
			86,
			87,
			88,
			89,
			90,
			///
			///<summary>
			///960 
			///</summary>
			91,
			92,
			19,
			150,
			150,
			165,
			150,
			150,
			166,
			23,
			///
			///<summary>
			///970 
			///</summary>
			150,
			25,
			160,
			20,
			174,
			175,
			1,
			2,
			165,
			165,
			///
			///<summary>
			///980 
			///</summary>
			104,
			165,
			165,
			43,
			150,
			165,
			240,
			150,
			49,
			50,
			///
			///<summary>
			///990 
			///</summary>
			174,
			175,
			49,
			50,
			23,
			23,
			25,
			25,
			53,
			165,
			///
			///<summary>
			///1000 
			///</summary>
			187,
			187,
			165,
			23,
			187,
			25,
			194,
			205,
			174,
			175,
			///
			///<summary>
			///1010 
			///</summary>
			71,
			72,
			69,
			70,
			71,
			72,
			73,
			74,
			75,
			76,
			///
			///<summary>
			///1020 
			///</summary>
			77,
			78,
			79,
			80,
			150,
			82,
			83,
			84,
			85,
			86,
			///
			///<summary>
			///1030 
			///</summary>
			87,
			88,
			89,
			90,
			91,
			92,
			19,
			98,
			150,
			165,
			///
			///<summary>
			///1040 
			///</summary>
			150,
			160,
			150,
			59,
			25,
			53,
			104,
			22,
			174,
			175,
			///
			///<summary>
			///1050 
			///</summary>
			213,
			138,
			5,
			165,
			1,
			165,
			150,
			165,
			150,
			150,
			///
			///<summary>
			///1060 
			///</summary>
			240,
			150,
			174,
			175,
			174,
			175,
			49,
			50,
			118,
			150,
			///
			///<summary>
			///1070 
			///</summary>
			35,
			165,
			27,
			165,
			165,
			194,
			165,
			108,
			127,
			76,
			///
			///<summary>
			///1080 
			///</summary>
			174,
			175,
			174,
			175,
			165,
			174,
			175,
			70,
			71,
			72,
			///
			///<summary>
			///1090 
			///</summary>
			73,
			74,
			75,
			76,
			77,
			78,
			79,
			80,
			166,
			82,
			///
			///<summary>
			///1100 
			///</summary>
			83,
			84,
			85,
			86,
			87,
			88,
			89,
			90,
			91,
			92,
			///
			///<summary>
			///1110 
			///</summary>
			19,
			20,
			193,
			22,
			150,
			150,
			150,
			26,
			27,
			76,
			///
			///<summary>
			///1120 
			///</summary>
			150,
			22,
			1,
			150,
			119,
			121,
			217,
			20,
			37,
			165,
			///
			///<summary>
			///1130 
			///</summary>
			165,
			165,
			16,
			19,
			20,
			165,
			22,
			205,
			165,
			119,
			///
			///<summary>
			///1140 
			///</summary>
			26,
			27,
			108,
			128,
			150,
			150,
			150,
			56,
			150,
			22,
			///
			///<summary>
			///1150 
			///</summary>
			150,
			37,
			150,
			127,
			160,
			23,
			150,
			66,
			193,
			165,
			///
			///<summary>
			///1160 
			///</summary>
			165,
			165,
			16,
			165,
			23,
			165,
			150,
			165,
			174,
			175,
			///
			///<summary>
			///1170 
			///</summary>
			56,
			165,
			150,
			65,
			174,
			175,
			15,
			86,
			87,
			88,
			///
			///<summary>
			///1180 
			///</summary>
			66,
			165,
			140,
			150,
			93,
			94,
			95,
			165,
			194,
			98,
			///
			///<summary>
			///1190 
			///</summary>
			174,
			175,
			22,
			3,
			164,
			193,
			174,
			175,
			165,
			150,
			///
			///<summary>
			///1200 
			///</summary>
			86,
			87,
			4,
			180,
			150,
			248,
			251,
			93,
			94,
			95,
			///
			///<summary>
			///1210 
			///</summary>
			216,
			180,
			98,
			251,
			165,
			221,
			150,
			149,
			6,
			165,
			///
			///<summary>
			///1220 
			///</summary>
			129,
			130,
			131,
			132,
			133,
			134,
			193,
			150,
			174,
			175,
			///
			///<summary>
			///1230 
			///</summary>
			116,
			165,
			19,
			20,
			150,
			22,
			149,
			151,
			150,
			26,
			///
			///<summary>
			///1240 
			///</summary>
			27,
			149,
			165,
			129,
			130,
			131,
			132,
			133,
			134,
			165,
			///
			///<summary>
			///1250 
			///</summary>
			37,
			174,
			175,
			165,
			149,
			19,
			20,
			13,
			22,
			150,
			///
			///<summary>
			///1260 
			///</summary>
			150,
			150,
			26,
			27,
			146,
			147,
			151,
			150,
			25,
			56,
			///
			///<summary>
			///1270 
			///</summary>
			152,
			159,
			154,
			37,
			165,
			165,
			165,
			193,
			160,
			66,
			///
			///<summary>
			///1280 
			///</summary>
			116,
			193,
			165,
			174,
			175,
			174,
			175,
			194,
			199,
			150,
			///
			///<summary>
			///1290 
			///</summary>
			200,
			126,
			56,
			124,
			123,
			150,
			201,
			122,
			150,
			86,
			///
			///<summary>
			///1300 
			///</summary>
			87,
			150,
			66,
			193,
			165,
			202,
			93,
			94,
			95,
			150,
			///
			///<summary>
			///1310 
			///</summary>
			165,
			98,
			194,
			165,
			125,
			22,
			165,
			150,
			150,
			26,
			///
			///<summary>
			///1320 
			///</summary>
			27,
			135,
			86,
			87,
			165,
			174,
			175,
			203,
			226,
			93,
			///
			///<summary>
			///1330 
			///</summary>
			94,
			95,
			165,
			165,
			98,
			150,
			218,
			150,
			193,
			157,
			///
			///<summary>
			///1340 
			///</summary>
			118,
			157,
			129,
			130,
			131,
			132,
			133,
			134,
			5,
			104,
			///
			///<summary>
			///1350 
			///</summary>
			165,
			211,
			165,
			10,
			11,
			12,
			13,
			14,
			150,
			66,
			///
			///<summary>
			///1360 
			///</summary>
			17,
			174,
			175,
			210,
			246,
			129,
			130,
			131,
			132,
			133,
			///
			///<summary>
			///1370 
			///</summary>
			134,
			150,
			210,
			165,
			31,
			121,
			33,
			150,
			150,
			86,
			///
			///<summary>
			///1380 
			///</summary>
			87,
			176,
			174,
			175,
			150,
			42,
			165,
			94,
			211,
			210,
			///
			///<summary>
			///1390 
			///</summary>
			150,
			98,
			165,
			165,
			211,
			174,
			175,
			150,
			55,
			165,
			///
			///<summary>
			///1400 
			///</summary>
			57,
			150,
			174,
			175,
			61,
			165,
			150,
			64,
			174,
			175,
			///
			///<summary>
			///1410 
			///</summary>
			150,
			150,
			165,
			150,
			174,
			175,
			165,
			104,
			150,
			184,
			///
			///<summary>
			///1420 
			///</summary>
			150,
			165,
			129,
			130,
			131,
			165,
			165,
			150,
			165,
			150,
			///
			///<summary>
			///1430 
			///</summary>
			150,
			176,
			150,
			165,
			47,
			165,
			150,
			150,
			176,
			103,
			///
			///<summary>
			///1440 
			///</summary>
			150,
			22,
			165,
			178,
			165,
			165,
			179,
			165,
			105,
			106,
			///
			///<summary>
			///1450 
			///</summary>
			107,
			165,
			165,
			229,
			111,
			165,
			92,
			176,
			229,
			116,
			///
			///<summary>
			///1460 
			///</summary>
			184,
			176,
			179,
			156,
			176,
			176,
			18,
			157,
			156,
			237,
			///
			///<summary>
			///1470 
			///</summary>
			45,
			157,
			156,
			135,
			157,
			157,
			238,
			156,
			68,
			157,
			///
			///<summary>
			///1480 
			///</summary>
			189,
			189,
			139,
			219,
			22,
			157,
			18,
			192,
			192,
			192,
			///
			///<summary>
			///1490 
			///</summary>
			192,
			189,
			219,
			199,
			157,
			242,
			40,
			157,
			199,
			242,
			///
			///<summary>
			///1500 
			///</summary>
			153,
			157,
			38,
			245,
			196,
			166,
			232,
			198,
			177,
			177,
			///
			///<summary>
			///1510 
			///</summary>
			232,
			227,
			209,
			178,
			166,
			182,
			166,
			148,
			177,
			177,
			///
			///<summary>
			///1520 
			///</summary>
			209,
			196,
			177,
			199,
			209,
			199,
			166,
			208,
			92,
			195,
			///
			///<summary>
			///1530 
			///</summary>
			174,
			174,
			183,
			252,
			183,
			183,
			252,
			191,
			252,
			235,
			///
			///<summary>
			///1540 
			///</summary>
			186,
			241,
			241,
			252,
			186,
			252,
			252,
			252,
			252,
			252,
			///
			///<summary>
			///1550 
			///</summary>
			252,
			252,
			252,
			252,
			252,
			252,
			236,
		};
		const int YY_SHIFT_USE_DFLT=-74;
		//#define YY_SHIFT_USE_DFLT (-74)
		const int YY_SHIFT_COUNT=418;
		//#define YY_SHIFT_COUNT (418)
		const int YY_SHIFT_MIN=-73;
		//#define YY_SHIFT_MIN   (-73)
		const int YY_SHIFT_MAX=1468;
		//#define YY_SHIFT_MAX   (1468)
		static short[] yy_shift_ofst=new short[] {
			///
			///<summary>
			///0 
			///</summary>
			975,
			1114,
			1343,
			1114,
			1213,
			1213,
			90,
			90,
			0,
			-19,
			///
			///<summary>
			///10 
			///</summary>
			1213,
			1213,
			1213,
			1213,
			1213,
			345,
			445,
			721,
			1091,
			1213,
			///
			///<summary>
			///20 
			///</summary>
			1213,
			1213,
			1213,
			1213,
			1213,
			1213,
			1213,
			1213,
			1213,
			1213,
			///
			///<summary>
			///30 
			///</summary>
			1213,
			1213,
			1213,
			1213,
			1213,
			1213,
			1213,
			1213,
			1213,
			1213,
			///
			///<summary>
			///40 
			///</summary>
			1213,
			1213,
			1213,
			1213,
			1213,
			1213,
			1213,
			1236,
			1213,
			1213,
			///
			///<summary>
			///50 
			///</summary>
			1213,
			1213,
			1213,
			1213,
			1213,
			1213,
			1213,
			1213,
			1213,
			1213,
			///
			///<summary>
			///60 
			///</summary>
			1213,
			199,
			445,
			445,
			835,
			835,
			365,
			1164,
			55,
			647,
			///
			///<summary>
			///70 
			///</summary>
			573,
			499,
			425,
			351,
			277,
			203,
			129,
			795,
			795,
			795,
			///
			///<summary>
			///80 
			///</summary>
			795,
			795,
			795,
			795,
			795,
			795,
			795,
			795,
			795,
			795,
			///
			///<summary>
			///90 
			///</summary>
			795,
			795,
			795,
			795,
			795,
			869,
			795,
			943,
			1017,
			1017,
			///
			///<summary>
			///100 
			///</summary>
			-69,
			-45,
			-45,
			-45,
			-45,
			-45,
			-1,
			58,
			138,
			100,
			///
			///<summary>
			///110 
			///</summary>
			445,
			445,
			445,
			445,
			445,
			445,
			445,
			445,
			445,
			445,
			///
			///<summary>
			///120 
			///</summary>
			445,
			445,
			445,
			445,
			445,
			445,
			537,
			438,
			445,
			445,
			///
			///<summary>
			///130 
			///</summary>
			445,
			445,
			445,
			365,
			807,
			1436,
			-74,
			-74,
			-74,
			1293,
			///
			///<summary>
			///140 
			///</summary>
			73,
			434,
			434,
			311,
			314,
			290,
			283,
			286,
			540,
			467,
			///
			///<summary>
			///150 
			///</summary>
			445,
			445,
			445,
			445,
			445,
			445,
			445,
			445,
			445,
			445,
			///
			///<summary>
			///160 
			///</summary>
			445,
			445,
			445,
			445,
			445,
			445,
			445,
			445,
			445,
			445,
			///
			///<summary>
			///170 
			///</summary>
			445,
			445,
			445,
			445,
			445,
			445,
			445,
			445,
			445,
			445,
			///
			///<summary>
			///180 
			///</summary>
			445,
			445,
			65,
			722,
			722,
			722,
			688,
			266,
			1164,
			1164,
			///
			///<summary>
			///190 
			///</summary>
			1164,
			-74,
			-74,
			-74,
			136,
			168,
			168,
			234,
			360,
			360,
			///
			///<summary>
			///200 
			///</summary>
			360,
			430,
			372,
			435,
			352,
			278,
			126,
			-36,
			-36,
			-36,
			///
			///<summary>
			///210 
			///</summary>
			-36,
			421,
			651,
			-36,
			-36,
			592,
			292,
			212,
			623,
			158,
			///
			///<summary>
			///220 
			///</summary>
			204,
			204,
			505,
			158,
			505,
			144,
			365,
			154,
			365,
			154,
			///
			///<summary>
			///230 
			///</summary>
			645,
			154,
			204,
			154,
			154,
			535,
			548,
			548,
			365,
			387,
			///
			///<summary>
			///240 
			///</summary>
			508,
			233,
			1464,
			1222,
			1222,
			1456,
			1456,
			1222,
			1462,
			1410,
			///
			///<summary>
			///250 
			///</summary>
			1165,
			1468,
			1468,
			1468,
			1468,
			1222,
			1165,
			1462,
			1410,
			1410,
			///
			///<summary>
			///260 
			///</summary>
			1222,
			1448,
			1338,
			1425,
			1222,
			1222,
			1448,
			1222,
			1448,
			1222,
			///
			///<summary>
			///270 
			///</summary>
			1448,
			1419,
			1313,
			1313,
			1313,
			1387,
			1364,
			1364,
			1419,
			1313,
			///
			///<summary>
			///280 
			///</summary>
			1336,
			1313,
			1387,
			1313,
			1313,
			1254,
			1245,
			1254,
			1245,
			1254,
			///
			///<summary>
			///290 
			///</summary>
			1245,
			1222,
			1222,
			1186,
			1189,
			1175,
			1169,
			1171,
			1165,
			1164,
			///
			///<summary>
			///300 
			///</summary>
			1243,
			1244,
			1244,
			1212,
			1212,
			1212,
			1212,
			-74,
			-74,
			-74,
			///
			///<summary>
			///310 
			///</summary>
			-74,
			-74,
			-74,
			939,
			104,
			680,
			571,
			327,
			1,
			980,
			///
			///<summary>
			///320 
			///</summary>
			26,
			972,
			971,
			946,
			901,
			870,
			830,
			806,
			54,
			21,
			///
			///<summary>
			///330 
			///</summary>
			-73,
			510,
			242,
			1198,
			1190,
			1170,
			1042,
			1161,
			1108,
			1146,
			///
			///<summary>
			///340 
			///</summary>
			1141,
			1132,
			1015,
			1127,
			1026,
			1034,
			1020,
			1107,
			1004,
			1116,
			///
			///<summary>
			///350 
			///</summary>
			1121,
			1005,
			1099,
			951,
			1043,
			1003,
			969,
			1045,
			1035,
			950,
			///
			///<summary>
			///360 
			///</summary>
			1053,
			1047,
			1025,
			942,
			913,
			992,
			1019,
			945,
			984,
			940,
			///
			///<summary>
			///370 
			///</summary>
			876,
			904,
			953,
			896,
			748,
			804,
			880,
			786,
			868,
			819,
			///
			///<summary>
			///380 
			///</summary>
			805,
			810,
			773,
			751,
			766,
			706,
			716,
			691,
			681,
			568,
			///
			///<summary>
			///390 
			///</summary>
			655,
			638,
			676,
			516,
			541,
			594,
			599,
			567,
			541,
			534,
			///
			///<summary>
			///400 
			///</summary>
			507,
			527,
			498,
			523,
			466,
			382,
			409,
			384,
			357,
			6,
			///
			///<summary>
			///410 
			///</summary>
			240,
			224,
			143,
			62,
			18,
			71,
			39,
			9,
			5,
		};
		const int YY_REDUCE_USE_DFLT=-142;
		//#define YY_REDUCE_USE_DFLT (-142)
		const int YY_REDUCE_COUNT=312;
		//#define YY_REDUCE_COUNT (312)
		const int YY_REDUCE_MIN=-141;
		//#define YY_REDUCE_MIN   (-141)
		const int YY_REDUCE_MAX=1369;
		//#define YY_REDUCE_MAX   (1369)
		static short[] yy_reduce_ofst=new short[] {
			///
			///<summary>
			///0 
			///</summary>
			-141,
			994,
			1118,
			223,
			157,
			-53,
			93,
			89,
			83,
			375,
			///
			///<summary>
			///10 
			///</summary>
			386,
			381,
			379,
			308,
			295,
			325,
			-47,
			27,
			1240,
			1234,
			///
			///<summary>
			///20 
			///</summary>
			1228,
			1221,
			1208,
			1187,
			1151,
			1111,
			1109,
			1077,
			1054,
			1022,
			///
			///<summary>
			///30 
			///</summary>
			1016,
			1000,
			911,
			908,
			906,
			890,
			888,
			874,
			834,
			816,
			///
			///<summary>
			///40 
			///</summary>
			800,
			760,
			758,
			755,
			742,
			739,
			726,
			685,
			672,
			668,
			///
			///<summary>
			///50 
			///</summary>
			665,
			652,
			611,
			609,
			607,
			604,
			591,
			578,
			526,
			519,
			///
			///<summary>
			///60 
			///</summary>
			453,
			474,
			454,
			461,
			443,
			245,
			442,
			473,
			484,
			484,
			///
			///<summary>
			///70 
			///</summary>
			484,
			484,
			484,
			484,
			484,
			484,
			484,
			484,
			484,
			484,
			///
			///<summary>
			///80 
			///</summary>
			484,
			484,
			484,
			484,
			484,
			484,
			484,
			484,
			484,
			484,
			///
			///<summary>
			///90 
			///</summary>
			484,
			484,
			484,
			484,
			484,
			484,
			484,
			484,
			484,
			484,
			///
			///<summary>
			///100 
			///</summary>
			484,
			484,
			484,
			484,
			484,
			484,
			484,
			130,
			484,
			484,
			///
			///<summary>
			///110 
			///</summary>
			1145,
			909,
			1110,
			1088,
			1084,
			1033,
			1002,
			965,
			820,
			837,
			///
			///<summary>
			///120 
			///</summary>
			746,
			686,
			612,
			817,
			610,
			919,
			221,
			563,
			814,
			813,
			///
			///<summary>
			///130 
			///</summary>
			744,
			669,
			470,
			543,
			484,
			484,
			484,
			484,
			484,
			291,
			///
			///<summary>
			///140 
			///</summary>
			569,
			671,
			658,
			970,
			1290,
			1287,
			1286,
			1282,
			518,
			518,
			///
			///<summary>
			///150 
			///</summary>
			1280,
			1279,
			1277,
			1270,
			1268,
			1263,
			1261,
			1260,
			1256,
			1251,
			///
			///<summary>
			///160 
			///</summary>
			1247,
			1227,
			1185,
			1168,
			1167,
			1159,
			1148,
			1139,
			1117,
			1066,
			///
			///<summary>
			///170 
			///</summary>
			1049,
			1006,
			998,
			996,
			995,
			973,
			970,
			966,
			964,
			892,
			///
			///<summary>
			///180 
			///</summary>
			762,
			-52,
			881,
			932,
			802,
			731,
			619,
			812,
			664,
			660,
			///
			///<summary>
			///190 
			///</summary>
			627,
			392,
			331,
			124,
			1358,
			1357,
			1356,
			1354,
			1352,
			1351,
			///
			///<summary>
			///200 
			///</summary>
			1349,
			1319,
			1334,
			1346,
			1334,
			1334,
			1334,
			1334,
			1334,
			1334,
			///
			///<summary>
			///210 
			///</summary>
			1334,
			1320,
			1304,
			1334,
			1334,
			1319,
			1360,
			1325,
			1369,
			1326,
			///
			///<summary>
			///220 
			///</summary>
			1315,
			1311,
			1301,
			1324,
			1300,
			1335,
			1350,
			1345,
			1348,
			1342,
			///
			///<summary>
			///230 
			///</summary>
			1333,
			1341,
			1303,
			1332,
			1331,
			1284,
			1278,
			1274,
			1339,
			1309,
			///
			///<summary>
			///240 
			///</summary>
			1308,
			1347,
			1258,
			1344,
			1340,
			1257,
			1253,
			1337,
			1273,
			1302,
			///
			///<summary>
			///250 
			///</summary>
			1299,
			1298,
			1297,
			1296,
			1295,
			1328,
			1294,
			1264,
			1292,
			1291,
			///
			///<summary>
			///260 
			///</summary>
			1322,
			1321,
			1238,
			1232,
			1318,
			1317,
			1316,
			1314,
			1312,
			1310,
			///
			///<summary>
			///270 
			///</summary>
			1307,
			1283,
			1289,
			1288,
			1285,
			1276,
			1229,
			1224,
			1267,
			1281,
			///
			///<summary>
			///280 
			///</summary>
			1265,
			1262,
			1235,
			1255,
			1205,
			1183,
			1179,
			1177,
			1162,
			1140,
			///
			///<summary>
			///290 
			///</summary>
			1153,
			1184,
			1182,
			1102,
			1124,
			1103,
			1095,
			1090,
			1089,
			1093,
			///
			///<summary>
			///300 
			///</summary>
			1112,
			1115,
			1086,
			1105,
			1092,
			1087,
			1068,
			962,
			955,
			957,
			///
			///<summary>
			///310 
			///</summary>
			1031,
			1023,
			1030,
		};
		static YYACTIONTYPE[] yy_default=new YYACTIONTYPE[] {
			///
			///<summary>
			///0 
			///</summary>
			635,
			870,
			959,
			959,
			959,
			870,
			899,
			899,
			959,
			759,
			///
			///<summary>
			///10 
			///</summary>
			959,
			959,
			959,
			959,
			868,
			959,
			959,
			933,
			959,
			959,
			///
			///<summary>
			///20 
			///</summary>
			959,
			959,
			959,
			959,
			959,
			959,
			959,
			959,
			959,
			959,
			///
			///<summary>
			///30 
			///</summary>
			959,
			959,
			959,
			959,
			959,
			959,
			959,
			959,
			959,
			959,
			///
			///<summary>
			///40 
			///</summary>
			959,
			959,
			959,
			959,
			959,
			959,
			959,
			959,
			959,
			959,
			///
			///<summary>
			///50 
			///</summary>
			959,
			959,
			959,
			959,
			959,
			959,
			959,
			959,
			959,
			959,
			///
			///<summary>
			///60 
			///</summary>
			959,
			959,
			959,
			959,
			899,
			899,
			674,
			763,
			794,
			959,
			///
			///<summary>
			///70 
			///</summary>
			959,
			959,
			959,
			959,
			959,
			959,
			959,
			932,
			934,
			809,
			///
			///<summary>
			///80 
			///</summary>
			808,
			802,
			801,
			912,
			774,
			799,
			792,
			785,
			796,
			871,
			///
			///<summary>
			///90 
			///</summary>
			864,
			865,
			863,
			867,
			872,
			959,
			795,
			831,
			848,
			830,
			///
			///<summary>
			///100 
			///</summary>
			842,
			847,
			854,
			846,
			843,
			833,
			832,
			666,
			834,
			835,
			///
			///<summary>
			///110 
			///</summary>
			959,
			959,
			959,
			959,
			959,
			959,
			959,
			959,
			959,
			959,
			///
			///<summary>
			///120 
			///</summary>
			959,
			959,
			959,
			959,
			959,
			959,
			661,
			728,
			959,
			959,
			///
			///<summary>
			///130 
			///</summary>
			959,
			959,
			959,
			959,
			836,
			837,
			851,
			850,
			849,
			959,
			///
			///<summary>
			///140 
			///</summary>
			959,
			959,
			959,
			959,
			959,
			959,
			959,
			959,
			959,
			959,
			///
			///<summary>
			///150 
			///</summary>
			959,
			939,
			937,
			959,
			883,
			959,
			959,
			959,
			959,
			959,
			///
			///<summary>
			///160 
			///</summary>
			959,
			959,
			959,
			959,
			959,
			959,
			959,
			959,
			959,
			959,
			///
			///<summary>
			///170 
			///</summary>
			959,
			959,
			959,
			959,
			959,
			959,
			959,
			959,
			959,
			959,
			///
			///<summary>
			///180 
			///</summary>
			959,
			641,
			959,
			759,
			759,
			759,
			635,
			959,
			959,
			959,
			///
			///<summary>
			///190 
			///</summary>
			959,
			951,
			763,
			753,
			719,
			959,
			959,
			959,
			959,
			959,
			///
			///<summary>
			///200 
			///</summary>
			959,
			959,
			959,
			959,
			959,
			959,
			959,
			804,
			742,
			922,
			///
			///<summary>
			///210 
			///</summary>
			924,
			959,
			905,
			740,
			663,
			761,
			676,
			751,
			643,
			798,
			///
			///<summary>
			///220 
			///</summary>
			776,
			776,
			917,
			798,
			917,
			700,
			959,
			788,
			959,
			788,
			///
			///<summary>
			///230 
			///</summary>
			697,
			788,
			776,
			788,
			788,
			866,
			959,
			959,
			959,
			760,
			///
			///<summary>
			///240 
			///</summary>
			751,
			959,
			944,
			767,
			767,
			936,
			936,
			767,
			810,
			732,
			///
			///<summary>
			///250 
			///</summary>
			798,
			739,
			739,
			739,
			739,
			767,
			798,
			810,
			732,
			732,
			///
			///<summary>
			///260 
			///</summary>
			767,
			658,
			911,
			909,
			767,
			767,
			658,
			767,
			658,
			767,
			///
			///<summary>
			///270 
			///</summary>
			658,
			876,
			730,
			730,
			730,
			715,
			880,
			880,
			876,
			730,
			///
			///<summary>
			///280 
			///</summary>
			700,
			730,
			715,
			730,
			730,
			780,
			775,
			780,
			775,
			780,
			///
			///<summary>
			///290 
			///</summary>
			775,
			767,
			767,
			959,
			793,
			781,
			791,
			789,
			798,
			959,
			///
			///<summary>
			///300 
			///</summary>
			718,
			651,
			651,
			640,
			640,
			640,
			640,
			956,
			956,
			951,
			///
			///<summary>
			///310 
			///</summary>
			702,
			702,
			684,
			959,
			959,
			959,
			959,
			959,
			959,
			959,
			///
			///<summary>
			///320 
			///</summary>
			885,
			959,
			959,
			959,
			959,
			959,
			959,
			959,
			959,
			959,
			///
			///<summary>
			///330 
			///</summary>
			959,
			959,
			959,
			959,
			636,
			946,
			959,
			959,
			943,
			959,
			///
			///<summary>
			///340 
			///</summary>
			959,
			959,
			959,
			959,
			959,
			959,
			959,
			959,
			959,
			959,
			///
			///<summary>
			///350 
			///</summary>
			959,
			959,
			959,
			959,
			959,
			959,
			959,
			959,
			959,
			915,
			///
			///<summary>
			///360 
			///</summary>
			959,
			959,
			959,
			959,
			959,
			959,
			908,
			907,
			959,
			959,
			///
			///<summary>
			///370 
			///</summary>
			959,
			959,
			959,
			959,
			959,
			959,
			959,
			959,
			959,
			959,
			///
			///<summary>
			///380 
			///</summary>
			959,
			959,
			959,
			959,
			959,
			959,
			959,
			959,
			959,
			959,
			///
			///<summary>
			///390 
			///</summary>
			959,
			959,
			959,
			959,
			790,
			959,
			782,
			959,
			869,
			959,
			///
			///<summary>
			///400 
			///</summary>
			959,
			959,
			959,
			959,
			959,
			959,
			959,
			959,
			959,
			745,
			///
			///<summary>
			///410 
			///</summary>
			819,
			959,
			818,
			822,
			817,
			668,
			959,
			649,
			959,
			632,
			///
			///<summary>
			///420 
			///</summary>
			637,
			955,
			958,
			957,
			954,
			953,
			952,
			947,
			945,
			942,
			///
			///<summary>
			///430 
			///</summary>
			941,
			940,
			938,
			935,
			931,
			889,
			887,
			894,
			893,
			892,
			///
			///<summary>
			///440 
			///</summary>
			891,
			890,
			888,
			886,
			884,
			805,
			803,
			800,
			797,
			930,
			///
			///<summary>
			///450 
			///</summary>
			882,
			741,
			738,
			737,
			657,
			948,
			914,
			923,
			921,
			811,
			///
			///<summary>
			///460 
			///</summary>
			920,
			919,
			918,
			916,
			913,
			900,
			807,
			806,
			733,
			874,
			///
			///<summary>
			///470 
			///</summary>
			873,
			660,
			904,
			903,
			902,
			906,
			910,
			901,
			769,
			659,
			///
			///<summary>
			///480 
			///</summary>
			656,
			665,
			722,
			721,
			729,
			727,
			726,
			725,
			724,
			723,
			///
			///<summary>
			///490 
			///</summary>
			720,
			667,
			675,
			686,
			714,
			699,
			698,
			879,
			881,
			878,
			///
			///<summary>
			///500 
			///</summary>
			877,
			707,
			706,
			712,
			711,
			710,
			709,
			708,
			705,
			704,
			///
			///<summary>
			///510 
			///</summary>
			703,
			696,
			695,
			701,
			694,
			717,
			716,
			713,
			693,
			736,
			///
			///<summary>
			///520 
			///</summary>
			735,
			734,
			731,
			692,
			691,
			690,
			822,
			689,
			688,
			828,
			///
			///<summary>
			///530 
			///</summary>
			827,
			815,
			858,
			756,
			755,
			754,
			766,
			765,
			778,
			777,
			///
			///<summary>
			///540 
			///</summary>
			813,
			812,
			779,
			764,
			758,
			757,
			773,
			772,
			771,
			770,
			///
			///<summary>
			///550 
			///</summary>
			762,
			752,
			784,
			787,
			786,
			783,
			860,
			768,
			857,
			929,
			///
			///<summary>
			///560 
			///</summary>
			928,
			927,
			926,
			925,
			862,
			861,
			829,
			826,
			679,
			680,
			///
			///<summary>
			///570 
			///</summary>
			898,
			896,
			897,
			895,
			682,
			681,
			678,
			677,
			859,
			747,
			///
			///<summary>
			///580 
			///</summary>
			746,
			855,
			852,
			844,
			840,
			856,
			853,
			845,
			841,
			839,
			///
			///<summary>
			///590 
			///</summary>
			838,
			824,
			823,
			821,
			820,
			816,
			825,
			670,
			748,
			744,
			///
			///<summary>
			///600 
			///</summary>
			743,
			814,
			750,
			749,
			687,
			685,
			683,
			664,
			662,
			655,
			///
			///<summary>
			///610 
			///</summary>
			653,
			652,
			654,
			650,
			648,
			647,
			646,
			645,
			644,
			673,
			///
			///<summary>
			///620 
			///</summary>
			672,
			671,
			669,
			668,
			642,
			639,
			638,
			634,
			633,
			631,
		};
		#endregion
		///<summary>
		///The next table maps tokens into fallback tokens.  If a construct
		/// like the following:
		///
		///      %fallback ID X Y Z.
		///
		/// appears in the grammar, then ID becomes a fallback token for X, Y,
		/// and Z.  Whenever one of the tokens X, Y, or Z is input to the parser
		/// but it does not parse, the type of the token is changed to ID and
		/// the parse is retried before an error is thrown.
		///
		///</summary>
		#if YYFALLBACK || TRUE
		static YYCODETYPE[] yyFallback=new YYCODETYPE[] {
			0,
			///
			///<summary>
			///$ => nothing 
			///</summary>
			0,
			///
			///<summary>
			///SEMI => nothing 
			///</summary>
			26,
			///
			///<summary>
			///EXPLAIN => ID 
			///</summary>
			26,
			///
			///<summary>
			///QUERY => ID 
			///</summary>
			26,
			///
			///<summary>
			///PLAN => ID 
			///</summary>
			26,
			///
			///<summary>
			///BEGIN => ID 
			///</summary>
			0,
			///
			///<summary>
			///TRANSACTION => nothing 
			///</summary>
			26,
			///
			///<summary>
			///DEFERRED => ID 
			///</summary>
			26,
			///
			///<summary>
			///IMMEDIATE => ID 
			///</summary>
			26,
			///
			///<summary>
			///EXCLUSIVE => ID 
			///</summary>
			0,
			///
			///<summary>
			///COMMIT => nothing 
			///</summary>
			26,
			///
			///<summary>
			///END => ID 
			///</summary>
			26,
			///
			///<summary>
			///ROLLBACK => ID 
			///</summary>
			26,
			///
			///<summary>
			///SAVEPOINT => ID 
			///</summary>
			26,
			///
			///<summary>
			///RELEASE => ID 
			///</summary>
			0,
			///
			///<summary>
			///TO => nothing 
			///</summary>
			0,
			///
			///<summary>
			///TABLE => nothing 
			///</summary>
			0,
			///
			///<summary>
			///CREATE => nothing 
			///</summary>
			26,
			///
			///<summary>
			///IF => ID 
			///</summary>
			0,
			///
			///<summary>
			///NOT => nothing 
			///</summary>
			0,
			///
			///<summary>
			///EXISTS => nothing 
			///</summary>
			26,
			///
			///<summary>
			///TEMP => ID 
			///</summary>
			0,
			///
			///<summary>
			///LP => nothing 
			///</summary>
			0,
			///
			///<summary>
			///RP => nothing 
			///</summary>
			0,
			///
			///<summary>
			///AS => nothing 
			///</summary>
			0,
			///
			///<summary>
			///COMMA => nothing 
			///</summary>
			0,
			///
			///<summary>
			///ID => nothing 
			///</summary>
			0,
			///
			///<summary>
			///INDEXED => nothing 
			///</summary>
			26,
			///
			///<summary>
			///ABORT => ID 
			///</summary>
			26,
			///
			///<summary>
			///ACTION => ID 
			///</summary>
			26,
			///
			///<summary>
			///AFTER => ID 
			///</summary>
			26,
			///
			///<summary>
			///ANALYZE => ID 
			///</summary>
			26,
			///
			///<summary>
			///ASC => ID 
			///</summary>
			26,
			///
			///<summary>
			///ATTACH => ID 
			///</summary>
			26,
			///
			///<summary>
			///BEFORE => ID 
			///</summary>
			26,
			///
			///<summary>
			///BY => ID 
			///</summary>
			26,
			///
			///<summary>
			///CASCADE => ID 
			///</summary>
			26,
			///
			///<summary>
			///CAST => ID 
			///</summary>
			26,
			///
			///<summary>
			///COLUMNKW => ID 
			///</summary>
			26,
			///
			///<summary>
			///CONFLICT => ID 
			///</summary>
			26,
			///
			///<summary>
			///DATABASE => ID 
			///</summary>
			26,
			///
			///<summary>
			///DESC => ID 
			///</summary>
			26,
			///
			///<summary>
			///DETACH => ID 
			///</summary>
			26,
			///
			///<summary>
			///EACH => ID 
			///</summary>
			26,
			///
			///<summary>
			///FAIL => ID 
			///</summary>
			26,
			///
			///<summary>
			///FOR => ID 
			///</summary>
			26,
			///
			///<summary>
			///IGNORE => ID 
			///</summary>
			26,
			///
			///<summary>
			///INITIALLY => ID 
			///</summary>
			26,
			///
			///<summary>
			///INSTEAD => ID 
			///</summary>
			26,
			///
			///<summary>
			///LIKE_KW => ID 
			///</summary>
			26,
			///
			///<summary>
			///MATCH => ID 
			///</summary>
			26,
			///
			///<summary>
			///NO => ID 
			///</summary>
			26,
			///
			///<summary>
			///KEY => ID 
			///</summary>
			26,
			///
			///<summary>
			///OF => ID 
			///</summary>
			26,
			///
			///<summary>
			///OFFSET => ID 
			///</summary>
			26,
			///
			///<summary>
			///PRAGMA => ID 
			///</summary>
			26,
			///
			///<summary>
			///RAISE => ID 
			///</summary>
			26,
			///
			///<summary>
			///REPLACE => ID 
			///</summary>
			26,
			///
			///<summary>
			///RESTRICT => ID 
			///</summary>
			26,
			///
			///<summary>
			///ROW => ID 
			///</summary>
			26,
			///
			///<summary>
			///TRIGGER => ID 
			///</summary>
			26,
			///
			///<summary>
			///VACUUM => ID 
			///</summary>
			26,
			///
			///<summary>
			///VIEW => ID 
			///</summary>
			26,
			///
			///<summary>
			///VIRTUAL => ID 
			///</summary>
			26,
			///
			///<summary>
			///REINDEX => ID 
			///</summary>
			26,
			///
			///<summary>
			///RENAME => ID 
			///</summary>
			26,
		///
		///<summary>
		///CTIME_KW => ID 
		///</summary>
		};
		#endif
		
		//typedef struct yyStackEntry yyStackEntry;
		///<summary>
		///The state of the parser is completely contained in an instance of
		/// the following structure
		///</summary>
		public class yyParser :IDisposable{
            int _yyidx;
			public int yyidx {
                get { return _yyidx; }
                set { _yyidx=value; if (null != stackTrace) stackTrace(); }
            }
            Action stackTrace;
			///
			///<summary>
			///Index of top element in stack 
			///</summary>
			#if YYTRACKMAXSTACKDEPTH
																																																																								int yyidxMax;                 /* Maximum value of yyidx */
#endif
			public int yyerrcnt;
            ///
            ///<summary>
            ///pParse
            ///Shifts left before out of the error 
            ///</summary>
            public ParseState parseState;
            // sqlite3ParserARG_SDECL                /* A place to hold %extra_argument */
#if YYSTACKDEPTH
																																																																								public int yystksz;                  /* Current side of the stack */
public yyStackEntry *yystack;        /* The parser's stack */
#else
            ///<summary>
            ///The parser's stack 
            ///</summary>
            public yyStackEntry[] yystack=new yyStackEntry[YYSTACKDEPTH];


            
#endif


            public void yy_destructor(
                YYCODETYPE yymajor,///Type code for object to destroy 
			    YYMINORTYPE yypminor///The object to be destroyed 
			) {
				var pParse=this.parseState;
				// sqlite3ParserARG_FETCH;
				switch(yymajor) {
				///
				///<summary>
				///Here is inserted the actions which take place when a
				///</summary>
				///<param name="terminal or non">terminal is destroyed.  This can happen</param>
				///<param name="when the symbol is popped from the stack during a">when the symbol is popped from the stack during a</param>
				///<param name="reduce or during error processing or when a parser is">reduce or during error processing or when a parser is</param>
				///<param name="being destroyed before it is finished parsing.">being destroyed before it is finished parsing.</param>
				///<param name=""></param>
				///<param name="Note: during a reduce, the only symbols destroyed are those">Note: during a reduce, the only symbols destroyed are those</param>
				///<param name="which appear on the RHS of the rule, but which are not used">which appear on the RHS of the rule, but which are not used</param>
				///<param name="inside the C code.">inside the C code.</param>
				///<param name=""></param>
				case 160:
				///
				///<summary>
				///select 
				///</summary>
				case 194:
				///
				///<summary>
				///oneselect 
				///</summary>
				{
					//#line 403 "parse.y"
					SelectMethods.SelectDestructor(pParse.db,ref (yypminor.yy387_Select));
					//#line 1399 "parse.c"
				}
				break;
				case 174:
				///
				///<summary>
				///term 
				///</summary>
				case 175:
				///
				///<summary>
				///Expr 
				///</summary>
				{
					//#line 720 "parse.y"
					exprc.Delete(pParse.db,ref (yypminor.yy118_ExprSpan).pExpr);
					//#line 1407 "parse.c"
				}
				break;
				case 179:
				///
				///<summary>
				///idxlist_opt 
				///</summary>
				case 187:
				///
				///<summary>
				///idxlist 
				///</summary>
				case 197:
				///
				///<summary>
				///selcollist 
				///</summary>
				case 200:
				///
				///<summary>
				///groupby_opt 
				///</summary>
				case 202:
				///
				///<summary>
				///orderby_opt 
				///</summary>
				case 204:
				///
				///<summary>
				///sclp 
				///</summary>
				case 214:
				///
				///<summary>
				///sortlist 
				///</summary>
				case 216:
				///
				///<summary>
				///nexprlist 
				///</summary>
				case 217:
				///
				///<summary>
				///setlist 
				///</summary>
				case 220:
				///
				///<summary>
				///itemlist 
				///</summary>
				case 221:
				///
				///<summary>
				///exprlist 
				///</summary>
				case 226:
				///
				///<summary>
				///case_exprlist 
				///</summary>
				{
					//#line 1103 "parse.y"
					exprc.Delete(pParse.db,ref (yypminor._ExprList));
					//#line 1425 "parse.c"
				}
				break;
				case 193:
				///
				///<summary>
				///fullname 
				///</summary>
				case 198:
				///
				///<summary>
				///from 
				///</summary>
				case 206:
				///
				///<summary>
				///seltablist 
				///</summary>
				case 207:
				///
				///<summary>
				///stl_prefix 
				///</summary>
				{
					//#line 534 "parse.y"
					build.sqlite3SrcListDelete(pParse.db,ref (yypminor.yy259_SrcList));
					//#line 1435 "parse.c"
				}
				break;
				case 199:
				///
				///<summary>
				///where_opt 
				///</summary>
				case 201:
				///
				///<summary>
				///having_opt 
				///</summary>
				case 210:
				///
				///<summary>
				///on_opt 
				///</summary>
				case 215:
				///
				///<summary>
				///sortitem 
				///</summary>
				case 225:
				///
				///<summary>
				///case_operand 
				///</summary>
				case 227:
				///
				///<summary>
				///case_else 
				///</summary>
				case 238:
				///
				///<summary>
				///when_clause 
				///</summary>
				case 243:
				///
				///<summary>
				///key_opt 
				///</summary>
				{
					//#line 644 "parse.y"
					exprc.Delete(pParse.db,ref (yypminor.yy314_Expr));
					//#line 1449 "parse.c"
				}
				break;
				case 211:
				///
				///<summary>
				///using_opt 
				///</summary>
				case 213:
				///
				///<summary>
				///inscollist 
				///</summary>
				case 219:
				///
				///<summary>
				///inscollist_opt 
				///</summary>
				{
					//#line 566 "parse.y"
					build.sqlite3IdListDelete(pParse.db,ref (yypminor.yy384_IdList));
					//#line 1458 "parse.c"
				}
				break;
				case 234:
				///
				///<summary>
				///trigger_cmd_list 
				///</summary>
				case 239:
				///
				///<summary>
				///trigger_cmd 
				///</summary>
				{
					//#line 1210"parse.y"
					TriggerParser.sqlite3DeleteTriggerStep(pParse.db,ref (yypminor.yy203_TriggerStep));
					//#line 1466 "parse.c"
				}
				break;
				case 236:
				///
				///<summary>
				///trigger_event 
				///</summary>
				{
					//#line 1196 "parse.y"
					build.sqlite3IdListDelete(pParse.db,ref (yypminor.yy90_TrigEvent).b);
					//#line 1473 "parse.c"
				}
				break;
				default:
				break;
				///
				///<summary>
				///If no destructor action specified: do nothing 
				///</summary>
				}
			}


            public int yy_pop_parser_stack() {
                ;
				var yytos=this.yystack[this.yyidx];
				///There is no mechanism by which the parser stack can be popped below
				///empty in SQLite.  
				if(Sqlite3.NEVER(this.yyidx<0))
					return 0;

                Log.WriteLine(String.Format("{0}Popping {1}", ">>>", yyTokenName[(int)yytos.major]));
#if !NDEBUG
																																																																												      if ( yyTraceFILE != null && pParser.yyidx >= 0 )
      {
        fprintf( yyTraceFILE, "%sPopping %s\n",
        yyTracePrompt,
        yyTokenName[yytos.major] );
      }
#endif
                var yymajor=yytos.major;
				this.yy_destructor((YYCODETYPE)yymajor,yytos.minor);
				this.yyidx--;
				return (YYCODETYPE)yymajor;
			}


			public void sqlite3ParserFree(
			dxDel freeProc//)(void*)     /* Function used to reclaim memory */
			) {
				yyParser pParser=this;
				///
				///<summary>
				///In SQLite, we never try to destroy a parser that was not successfully
				///created in the first place. 
				///</summary>
				if(Sqlite3.NEVER(pParser==null))
					return;
				while(pParser.yyidx>=0)
					pParser.yy_pop_parser_stack();
				#if YYSTACKDEPTH
																																																																												pParser.yystack = null;//free(pParser.yystack);
#endif
				pParser=null;
				// freeProc(ref pParser);
			}


            public int yy_find_shift_action(
                YYCODETYPE iLookAhead///<param name="The look">ahead token </param>
			) {
				int stateno=this.yystack[this.yyidx].stateno;
                int i;

                if (stateno>YY_SHIFT_COUNT||(i=yy_shift_ofst[stateno])==YY_SHIFT_USE_DFLT) {
					return yy_default[stateno];
				}
				//Log.WriteLine((TokenType)iLookAhead);
				//Log.WriteLine("current\t : "+i);
				Debug.Assert(iLookAhead!=YYNOCODE_253_VACUUM);
				i+=iLookAhead;
				//Log.WriteLine("new\t : " + i);
				if(i<0||i>=YY_ACTTAB_COUNT||yy_lookahead[i]!=iLookAhead) {
					if(iLookAhead>0) {
						//#if YYFALLBACK
						YYCODETYPE iFallback;
						///Fallback token 
						if(iLookAhead<yyFallback.Length//yyFallback.Length/sizeof(yyFallback[0])
						&&(iFallback=yyFallback[iLookAhead])!=0) {
							#if !NDEBUG
																																																																																																																																																				            if ( yyTraceFILE != null )
            {
              fprintf( yyTraceFILE, "%sFALLBACK %s => %s\n",
              yyTracePrompt, yyTokenName[iLookAhead], yyTokenName[iFallback] );
            }
#endif
							return this.yy_find_shift_action(iFallback);
						}
						//#endif
						//#if YYWILDCARD
						{
							int j=i-iLookAhead+YYWILDCARD;
							if(//#if YY_SHIFT_MIN+YYWILDCARD<0
							j>=0&&//#endif
							//#if YY_SHIFT_MAX+YYWILDCARD>=YY_ACTTAB_COUNT
							j<YY_ACTTAB_COUNT&&//#endif
							yy_lookahead[j]==YYWILDCARD) {
								#if !NDEBUG
																																																																																																																																																																												              if ( yyTraceFILE != null )
              {
                Debugger.Break(); // TODO --
                //fprintf(yyTraceFILE, "%sWILDCARD %s => %s\n",
                //   yyTracePrompt, yyTokenName[iLookAhead], yyTokenName[YYWILDCARD]);
              }
#endif
								return yy_action[j];
							}
							//#endif // * YYWILDCARD */
						}
					}
					return yy_default[stateno];
				}
				else {
					return yy_action[i];
				}
			}


            public void yy_shift(
                int yyNewState,///The new state to shift in 
                TokenType yyMajor,///The major token to shift in 
                YYMINORTYPE yypMinor///Pointer to the minor token to shift in 
            ) {
                this.yyidx++;
#if YYTRACKMAXSTACKDEPTH
																																																																												if( yypParser.yyidx>yypParser.yyidxMax ){
yypParser.yyidxMax = yypParser.yyidx;
}
#endif
#if !YYSTACKDEPTH
                if (this.yyidx >= YYSTACKDEPTH) {
                    this.yyStackOverflow(yypMinor);
                    return;
                }
#else
																																																																												if( yypParser.yyidx>=yypParser.yystksz ){
yyGrowStack(yypParser);
if( yypParser.yyidx>=yypParser.yystksz ){
yyStackOverflow(yypParser, yypMinor);
return;
}
}
#endif
                this.yystack[this.yyidx] = new yyStackEntry()
                {
                    //yytos = yypParser.yystack[yypParser.yyidx];
                    stateno = (YYACTIONTYPE)yyNewState,
                    major = yyMajor,
                    minor = yypMinor
                };
				#if !NDEBUG
																																																																												      if ( yyTraceFILE != null && yypParser.yyidx > 0 )
      {
        int i;
        fprintf( yyTraceFILE, "%sShift %d\n", yyTracePrompt, yyNewState );
        fprintf( yyTraceFILE, "%sStack:", yyTracePrompt );
        for ( i = 1; i <= yypParser.yyidx; i++ )
          fprintf( yyTraceFILE, " %s", yyTokenName[yypParser.yystack[i].major] );
        fprintf( yyTraceFILE, "\n" );
      }
#endif
			}


            public void yyStackOverflow(YYMINORTYPE yypMinor) {
				ParseState pParse=this.parseState;
				// sqlite3ParserARG_FETCH;
				this.yyidx--;
				#if !NDEBUG
																																																																												      if ( yyTraceFILE != null )
      {
        Debugger.Break(); // TODO --
        //fprintf(yyTraceFILE, "%sStack Overflow!\n", yyTracePrompt);
      }
#endif
				while(this.yyidx>=0)
					this.yy_pop_parser_stack();
				///
				///<summary>
				///Here code is inserted which will execute if the parser
				///stack every overflows 
				///</summary>
				//#line 38 "parse.y"
				sqliteinth.UNUSED_PARAMETER(yypMinor);
				///
				///<summary>
				///Silence some compiler warnings 
				///</summary>
				utilc.sqlite3ErrorMsg(pParse,"parser stack overflow");
				pParse.parseError=1;
				//#line 1664  "parse.c"
				this.parseState=pParse;
				//      sqlite3ParserARG_STORE; /* Suppress warning about unused %extra_argument var */
			}


            public void yy_parse_failed(
			) {
				ParseState pParse=this.parseState;
				//       sqlite3ParserARG_FETCH;
				#if !NDEBUG
																																																																												      if ( yyTraceFILE != null )
      {
        Debugger.Break(); // TODO --        fprintf(yyTraceFILE, "%sFail!\n", yyTracePrompt);
      }
#endif
				while(this.yyidx>=0)
					this.yy_pop_parser_stack();
				///Here code is inserted which will be executed whenever the
				///parser fails 
				this.parseState=pParse;
				//      sqlite3ParserARG_STORE; /* Suppress warning about unused %extra_argument variable */
			}


            public void yy_syntax_error(
			    TokenType yymajor,///The major type of the error token 
			    YYMINORTYPE yyminor///The minor type of the error token 
			) {
				var pParse=this.parseState;
				sqliteinth.UNUSED_PARAMETER(yymajor);///Silence some compiler warnings 
				Debug.Assert(yyminor.yy0Token.zRestSql.Length>0);//TOKEN.z[0]);  /* The tokenizer always gives us a token */
				utilc.sqlite3ErrorMsg(pParse,"near \"%T\": syntax error",yyminor.yy0Token);//&TOKEN);
				pParse.parseError=1;//#line 3661 "parse.c"
				this.parseState=pParse;// sqlite3ParserARG_STORE; /* Suppress warning about unused %extra_argument variable */
			}


            public void yy_accept(
			) {
				ParseState pParse=this.parseState;
                //       sqlite3ParserARG_FETCH;
                Print("Accept");
				
				while(this.yyidx>=0)
					this.yy_pop_parser_stack();
				///Here code is inserted which will be executed whenever the
				///parser accepts 
				this.parseState=pParse;
				//      sqlite3ParserARG_STORE; /* Suppress warning about unused %extra_argument variable */
			}


            public void ConsumeToken(
			    TokenType _yymajor,///The major token code number 
			    sqlite3ParserTOKENTYPE yyminor,///The value for the token 
			    ParseState sState//sqlite3ParserARG_PDECL           /* Optional %extra_argument parameter */
			) {                
                
				var yymajor=_yymajor;
                var yyendofinput = (yymajor == 0); ///True if we are at the end of input 

				YYMINORTYPE yyminorunion =new YYMINORTYPE();
				int yyact=0;///The parser action. 


                stackTrace = yyStackEntry.Print = () =>
                {
                    Console.Clear();
                    Console.WriteLine();
                    Print(yyidx.ToString(), ConsoleColor.Blue);
                    Print(yyminor.ToString(), ConsoleColor.Red);
                    Console.WriteLine();
                    if(yyact<yyRuleName.Length)
                    Print(yyRuleName[yyact], ConsoleColor.Green);
                    for (int i = 0; i <= yyidx; i++)
                    {
                        Console.WriteLine();
                        if (null != yystack[i])
                            Print(yystack[i].stateno + " : " + yyRuleName[(int)yystack[i].major], ConsoleColor.Cyan);
                    }
                };
                stackTrace();


                yyParser yypParser = this;///The parser 

                ///(re)initialize the parser, if necessary 
                if (yypParser.yyidx<0) {
					yypParser.yyidx=0;
					yypParser.yyerrcnt=-1;
					yypParser.yystack[0]=new yyStackEntry();
					yypParser.yystack[0].stateno=0;
					yypParser.yystack[0].major=0;
				}
				yyminorunion.yy0Token=yyminor.Copy();
				
				yypParser.parseState=sState;
                string yyTracePrompt = "$";
                Console.WriteLine();
                //      sqlite3ParserARG_STORE;
                Print(yyTracePrompt+"Input " +  yyRuleName[(int)yymajor]);
                Print(yyminor.ToString(),ConsoleColor.Gray);
#if !NDEBUG
																																																																												      if ( yyTraceFILE != null )
      {
        fprintf( yyTraceFILE, "%sInput %s\n", yyTracePrompt, yyTokenName[yymajor] );
      }
#endif
                do {
					yyact=yypParser.yy_find_shift_action((YYCODETYPE)_yymajor);
                    
					if(yyact<YYNSTATE) {
						Debug.Assert(!yyendofinput);
						///Impossible to shift the $ token 
						yypParser.yy_shift(yyact,yymajor,yyminorunion);
						yypParser.yyerrcnt--;
						yymajor=(TokenType)YYNOCODE_253_VACUUM;
					}
					else
						if(yyact<YYNSTATE+YYNRULE) {
							yy_reduce(yypParser,yyact-YYNSTATE);
						}
						else {
							Debug.Assert(yyact==YY_ERROR_ACTION);
							#if YYERRORSYMBOL
																																																																																																																																																				int yymx;
#endif
							#if !NDEBUG
																																																																																																																																																				          if ( yyTraceFILE != null )
          {
            Debugger.Break(); // TODO --            fprintf(yyTraceFILE, "%sSyntax Error!\n", yyTracePrompt);
          }
#endif
							#if YYERRORSYMBOL
																																																																																																																																																				/* A syntax error has occurred.
** The response to an error depends upon whether or not the
** grammar defines an error token "ERROR".
**
** This is what we do if the grammar does define ERROR:
**
**  * Call the %syntax_error function.
**
**  * Begin popping the stack until we enter a state where
**    it is legal to shift the error symbol, then shift
**    the error symbol.
**
**  * Set the error count to three.
**
**  * Begin accepting and shifting new tokens.  No new error
**    processing will occur until three tokens have been
**    shifted successfully.
**
*/
if( yypParser.yyerrcnt<0 ){
yy_syntax_error(yypParser,yymajor,yyminorunion);
}
yymx = yypParser.yystack[yypParser.yyidx].major;
if( yymx==YYERRORSYMBOL || yyerrorhit ){
#if !NDEBUG
																																																																																																																																																				if( yyTraceFILE ){
Debug.Assert(false); // TODO --                      fprintf(yyTraceFILE,"%sDiscard input token %s\n",
yyTracePrompt,yyTokenName[yymajor]);
}
#endif
																																																																																																																																																				yy_destructor(yypParser,(YYCODETYPE)yymajor,yyminorunion);
yymajor = YYNOCODE;
}else{
while(
yypParser.yyidx >= 0 &&
yymx != YYERRORSYMBOL &&
(yyact = yy_find_reduce_action(
yypParser.yystack[yypParser.yyidx].stateno,
YYERRORSYMBOL)) >= YYNSTATE
){
yy_pop_parser_stack(yypParser);
}
if( yypParser.yyidx < 0 || yymajor==0 ){
yy_destructor(yypParser, (YYCODETYPE)yymajor,yyminorunion);
yy_parse_failed(yypParser);
yymajor = YYNOCODE;
}else if( yymx!=YYERRORSYMBOL ){
YYMINORTYPE u2;
u2.YYERRSYMDT = null;
yy_shift(yypParser,yyact,YYERRORSYMBOL,u2);
}
}
yypParser.yyerrcnt = 3;
yyerrorhit = 1;
#elif (YYNOERRORRECOVERY)
																																																																																																																																																				/* If the YYNOERRORRECOVERY macro is defined, then do not attempt to
** do any kind of error recovery.  Instead, simply invoke the syntax
** error routine and continue going as if nothing had happened.
**
** Applications can set this macro (for example inside %include) if
** they intend to abandon the parse upon the first syntax error seen.
*/
yy_syntax_error(yypParser,yymajor,yyminorunion);
yy_destructor(yypParser,(YYCODETYPE)yymajor,yyminorunion);
yymajor = YYNOCODE;
#else
							///This is what we do if the grammar does not define ERROR:
							///Report an error message, and throw away the input token.
							///If the input token is $, then fail the parse.
							///
							///As before, subsequent error messages are suppressed until
							///three input tokens have been successfully shifted.
							if(yypParser.yyerrcnt<=0) {
								yypParser.yy_syntax_error(yymajor,yyminorunion);
							}
							yypParser.yyerrcnt=3;
							yypParser.yy_destructor((YYCODETYPE)yymajor,yyminorunion);
							if(yyendofinput) {
								yypParser.yy_parse_failed();
							}
							yymajor=(TokenType)YYNOCODE_253_VACUUM;
							#endif
						}
                    stackTrace();
				}
				while(yymajor!=(TokenType)YYNOCODE_253_VACUUM&&yypParser.yyidx>=0);
				return;
			}

            public void Dispose()
            {
                this.sqlite3ParserFree(null);
            }
        }
		//typedef struct yyParser yyParser;
		#if !NDEBUG
																																																    //include <stdio.h>
    static TextWriter yyTraceFILE = null;
    static string yyTracePrompt = "";
#endif
		#if !NDEBUG
																																																    /*
** Turn parser tracing on by giving a stream to which to write the trace
** and a prompt to preface each trace message.  Tracing is turned off
** by making either argument NULL
**
** Inputs:
** <ul>
** <li> A FILE* to which trace output should be written.
**      If NULL, then tracing is turned off.
** <li> A prefix string written at the beginning of every
**      line of trace output.  If NULL, then tracing is
**      turned off.
** </ul>
**
** Outputs:
** None.
*/
    static void sqlite3ParserTrace( TextWriter TraceFILE, string zTracePrompt )
    {
      yyTraceFILE = TraceFILE;
      yyTracePrompt = zTracePrompt;
      if ( yyTraceFILE == null )
        yyTracePrompt = "";
      else if ( yyTracePrompt == "" )
        yyTraceFILE = null;
    }
#endif
		//#if !NDEBUG
																																																    /* For tracing shifts, the names of all terminals and nonterminals
** are required.  The following table supplies these names */
    static string[] yyTokenName = {
"$",             "SEMI",          "EXPLAIN",       "QUERY",       
"PLAN",          "BEGIN",         "TRANSACTION",   "DEFERRED",    
"IMMEDIATE",     "EXCLUSIVE",     "COMMIT",        "END",         
"ROLLBACK",      "SAVEPOINT",     "RELEASE",       "TO",          
"TABLE",         "CREATE",        "IF",            "NOT",         
"EXISTS",        "TEMP",          "LP",            "RP",          
"AS",            "COMMA",         "ID",            "INDEXED",     
"ABORT",         "ACTION",        "AFTER",         "ANALYZE",     
"ASC",           "ATTACH",        "BEFORE",        "BY",          
"CASCADE",       "CAST",          "COLUMNKW",      "CONFLICT",    
"DATABASE",      "DESC",          "DETACH",        "EACH",        
"FAIL",          "FOR",           "IGNORE",        "INITIALLY",   
"INSTEAD",       "LIKE_KW",       "MATCH",         "NO",          
"KEY",           "OF",            "OFFSET",        "PRAGMA",      
"RAISE",         "REPLACE",       "RESTRICT",      "ROW",         
"TRIGGER",       "VACUUM",        "VIEW",          "VIRTUAL",     
"REINDEX",       "RENAME",        "CTIME_KW",      "ANY",         
"OR",            "AND",           "IS",            "BETWEEN",     
"IN",            "ISNULL",        "NOTNULL",       "NE",          
"EQ",            "GT",            "LE",            "LT",          
"GE",            "ESCAPE",        "BITAND",        "BITOR",       
"LSHIFT",        "RSHIFT",        "PLUS",          "MINUS",       
"STAR",          "SLASH",         "REM",           "CONCAT",      
"COLLATE",       "BITNOT",        "STRING",        "JOIN_KW",     
"CONSTRAINT",    "DEFAULT",       "NULL",          "PRIMARY",     
"UNIQUE",        "CHECK",         "REFERENCES",    "AUTOINCR",    
"ON",            "INSERT",        "DELETE",        "UPDATE",      
"SET",           "DEFERRABLE",    "FOREIGN",       "DROP",        
"UNION",         "ALL",           "EXCEPT",        "INTERSECT",   
"SELECT",        "DISTINCT",      "DOT",           "FROM",        
"JOIN",          "USING",         "ORDER",         "GROUP",       
"HAVING",        "LIMIT",         "WHERE",         "INTO",        
"VALUES",        "INTEGER",       "FLOAT",         "BLOB",        
"REGISTER",      "VARIABLE",      "CASE",          "WHEN",        
"THEN",          "ELSE",          "INDEX",         "ALTER",       
"ADD",           "error",         "input",         "cmdlist",     
"ecmd",          "explain",       "cmdx",          "cmd",         
"transtype",     "trans_opt",     "nm",            "savepoint_opt",
"create_table",  "create_table_args",  "createkw",      "temp",        
"ifnotexists",   "dbnm",          "columnlist",    "conslist_opt",
"select",        "column",        "columnid",      "type",        
"carglist",      "id",            "ids",           "typetoken",   
"typename",      "signed",        "plus_num",      "minus_num",   
"carg",          "ccons",         "term",          "expr",        
"onconf",        "sortorder",     "autoinc",       "idxlist_opt", 
"refargs",       "defer_subclause",  "refarg",        "refact",      
"init_deferred_pred_opt",  "conslist",      "tcons",         "idxlist",     
"defer_subclause_opt",  "orconf",        "resolvetype",   "raisetype",   
"ifexists",      "fullname",      "oneselect",     "multiselect_op",
"distinct",      "selcollist",    "from",          "where_opt",   
"groupby_opt",   "having_opt",    "orderby_opt",   "limit_opt",   
"sclp",          "as",            "seltablist",    "stl_prefix",  
"joinop",        "indexed_opt",   "on_opt",        "using_opt",   
"joinop2",       "inscollist",    "sortlist",      "sortitem",    
"nexprlist",     "setlist",       "insert_cmd",    "inscollist_opt",
"itemlist",      "exprlist",      "likeop",        "between_op",  
"in_op",         "case_operand",  "case_exprlist",  "case_else",   
"uniqueflag",    "collate",       "nmnum",         "plus_opt",    
"number",        "trigger_decl",  "trigger_cmd_list",  "trigger_time",
"trigger_event",  "foreach_clause",  "when_clause",   "trigger_cmd", 
"trnm",          "tridxby",       "database_kw_opt",  "key_opt",     
"add_column_fullname",  "kwcolumn_opt",  "create_vtab",   "vtabarglist", 
"vtabarg",       "vtabargtoken",  "lp",            "anylist",     
};
//#endif
		//#if !NDEBUG
																																																    ///<summary>
///For tracing reduce actions, the names of all rules are required.
///</summary>
    static string[] yyRuleName = {
/*   0 */ "input ::= cmdlist",
/*   1 */ "cmdlist ::= cmdlist ecmd",
/*   2 */ "cmdlist ::= ecmd",
/*   3 */ "ecmd ::= SEMI",
/*   4 */ "ecmd ::= explain cmdx SEMI",
/*   5 */ "explain ::=",
/*   6 */ "explain ::= EXPLAIN",
/*   7 */ "explain ::= EXPLAIN QUERY PLAN",
/*   8 */ "cmdx ::= cmd",
/*   9 */ "cmd ::= BEGIN transtype trans_opt",
/*  10 */ "trans_opt ::=",
/*  11 */ "trans_opt ::= TRANSACTION",
/*  12 */ "trans_opt ::= TRANSACTION nm",
/*  13 */ "transtype ::=",
/*  14 */ "transtype ::= DEFERRED",
/*  15 */ "transtype ::= IMMEDIATE",
/*  16 */ "transtype ::= EXCLUSIVE",
/*  17 */ "cmd ::= COMMIT trans_opt",
/*  18 */ "cmd ::= END trans_opt",
/*  19 */ "cmd ::= ROLLBACK trans_opt",
/*  20 */ "savepoint_opt ::= SAVEPOINT",
/*  21 */ "savepoint_opt ::=",
/*  22 */ "cmd ::= SAVEPOINT nm",
/*  23 */ "cmd ::= RELEASE savepoint_opt nm",
/*  24 */ "cmd ::= ROLLBACK trans_opt TO savepoint_opt nm",
/*  25 */ "cmd ::= create_table create_table_args",
/*  26 */ "create_table ::= createkw temp TABLE ifnotexists nm dbnm",
/*  27 */ "createkw ::= CREATE",
/*  28 */ "ifnotexists ::=",
/*  29 */ "ifnotexists ::= IF NOT EXISTS",
/*  30 */ "temp ::= TEMP",
/*  31 */ "temp ::=",
/*  32 */ "create_table_args ::= LP columnlist conslist_opt RP",
/*  33 */ "create_table_args ::= AS select",
/*  34 */ "columnlist ::= columnlist COMMA column",
/*  35 */ "columnlist ::= column",
/*  36 */ "column ::= columnid type carglist",
/*  37 */ "columnid ::= nm",
/*  38 */ "id ::= ID",
/*  39 */ "id ::= INDEXED",
/*  40 */ "ids ::= ID|STRING",
/*  41 */ "nm ::= id",
/*  42 */ "nm ::= STRING",
/*  43 */ "nm ::= JOIN_KW",
/*  44 */ "type ::=",
/*  45 */ "type ::= typetoken",
/*  46 */ "typetoken ::= typename",
/*  47 */ "typetoken ::= typename LP signed RP",
/*  48 */ "typetoken ::= typename LP signed COMMA signed RP",
/*  49 */ "typename ::= ids",
/*  50 */ "typename ::= typename ids",
/*  51 */ "signed ::= plus_num",
/*  52 */ "signed ::= minus_num",
/*  53 */ "carglist ::= carglist carg",
/*  54 */ "carglist ::=",
/*  55 */ "carg ::= CONSTRAINT nm ccons",
/*  56 */ "carg ::= ccons",
/*  57 */ "ccons ::= DEFAULT term",
/*  58 */ "ccons ::= DEFAULT LP expr RP",
/*  59 */ "ccons ::= DEFAULT PLUS term",
/*  60 */ "ccons ::= DEFAULT MINUS term",
/*  61 */ "ccons ::= DEFAULT id",
/*  62 */ "ccons ::= NULL onconf",
/*  63 */ "ccons ::= NOT NULL onconf",
/*  64 */ "ccons ::= PRIMARY KEY sortorder onconf autoinc",
/*  65 */ "ccons ::= UNIQUE onconf",
/*  66 */ "ccons ::= CHECK LP expr RP",
/*  67 */ "ccons ::= REFERENCES nm idxlist_opt refargs",
/*  68 */ "ccons ::= defer_subclause",
/*  69 */ "ccons ::= COLLATE ids",
/*  70 */ "autoinc ::=",
/*  71 */ "autoinc ::= AUTOINCR",
/*  72 */ "refargs ::=",
/*  73 */ "refargs ::= refargs refarg",
/*  74 */ "refarg ::= MATCH nm",
/*  75 */ "refarg ::= ON INSERT refact",
/*  76 */ "refarg ::= ON DELETE refact",
/*  77 */ "refarg ::= ON UPDATE refact",
/*  78 */ "refact ::= SET NULL",
/*  79 */ "refact ::= SET DEFAULT",
/*  80 */ "refact ::= CASCADE",
/*  81 */ "refact ::= RESTRICT",
/*  82 */ "refact ::= NO ACTION",
/*  83 */ "defer_subclause ::= NOT DEFERRABLE init_deferred_pred_opt",
/*  84 */ "defer_subclause ::= DEFERRABLE init_deferred_pred_opt",
/*  85 */ "init_deferred_pred_opt ::=",
/*  86 */ "init_deferred_pred_opt ::= INITIALLY DEFERRED",
/*  87 */ "init_deferred_pred_opt ::= INITIALLY IMMEDIATE",
/*  88 */ "conslist_opt ::=",
/*  89 */ "conslist_opt ::= COMMA conslist",
/*  90 */ "conslist ::= conslist COMMA tcons",
/*  91 */ "conslist ::= conslist tcons",
/*  92 */ "conslist ::= tcons",
/*  93 */ "tcons ::= CONSTRAINT nm",
/*  94 */ "tcons ::= PRIMARY KEY LP idxlist autoinc RP onconf",
/*  95 */ "tcons ::= UNIQUE LP idxlist RP onconf",
/*  96 */ "tcons ::= CHECK LP expr RP onconf",
/*  97 */ "tcons ::= FOREIGN KEY LP idxlist RP REFERENCES nm idxlist_opt refargs defer_subclause_opt",
/*  98 */ "defer_subclause_opt ::=",
/*  99 */ "defer_subclause_opt ::= defer_subclause",
/* 100 */ "onconf ::=",
/* 101 */ "onconf ::= ON CONFLICT resolvetype",
/* 102 */ "orconf ::=",
/* 103 */ "orconf ::= OR resolvetype",
/* 104 */ "resolvetype ::= raisetype",
/* 105 */ "resolvetype ::= IGNORE",
/* 106 */ "resolvetype ::= REPLACE",
/* 107 */ "cmd ::= DROP TABLE ifexists fullname",
/* 108 */ "ifexists ::= IF EXISTS",
/* 109 */ "ifexists ::=",
/* 110 */ "cmd ::= createkw temp VIEW ifnotexists nm dbnm AS select",
/* 111 */ "cmd ::= DROP VIEW ifexists fullname",
/* 112 */ "cmd ::= select",
/* 113 */ "select ::= oneselect",
/* 114 */ "select ::= select multiselect_op oneselect",
/* 115 */ "multiselect_op ::= UNION",
/* 116 */ "multiselect_op ::= UNION ALL",
/* 117 */ "multiselect_op ::= EXCEPT|INTERSECT",
/* 118 */ "oneselect ::= SELECT distinct selcollist from where_opt groupby_opt having_opt orderby_opt limit_opt",
/* 119 */ "distinct ::= DISTINCT",
/* 120 */ "distinct ::= ALL",
/* 121 */ "distinct ::=",
/* 122 */ "sclp ::= selcollist COMMA",
/* 123 */ "sclp ::=",
/* 124 */ "selcollist ::= sclp expr as",
/* 125 */ "selcollist ::= sclp STAR",
/* 126 */ "selcollist ::= sclp nm DOT STAR",
/* 127 */ "as ::= AS nm",
/* 128 */ "as ::= ids",
/* 129 */ "as ::=",
/* 130 */ "from ::=",
/* 131 */ "from ::= FROM seltablist",
/* 132 */ "stl_prefix ::= seltablist joinop",
/* 133 */ "stl_prefix ::=",
/* 134 */ "seltablist ::= stl_prefix nm dbnm as indexed_opt on_opt using_opt",
/* 135 */ "seltablist ::= stl_prefix LP select RP as on_opt using_opt",
/* 136 */ "seltablist ::= stl_prefix LP seltablist RP as on_opt using_opt",
/* 137 */ "dbnm ::=",
/* 138 */ "dbnm ::= DOT nm",
/* 139 */ "fullname ::= nm dbnm",
/* 140 */ "joinop ::= COMMA|JOIN",
/* 141 */ "joinop ::= JOIN_KW JOIN",
/* 142 */ "joinop ::= JOIN_KW nm JOIN",
/* 143 */ "joinop ::= JOIN_KW nm nm JOIN",
/* 144 */ "on_opt ::= ON expr",
/* 145 */ "on_opt ::=",
/* 146 */ "indexed_opt ::=",
/* 147 */ "indexed_opt ::= INDEXED BY nm",
/* 148 */ "indexed_opt ::= NOT INDEXED",
/* 149 */ "using_opt ::= USING LP inscollist RP",
/* 150 */ "using_opt ::=",
/* 151 */ "orderby_opt ::=",
/* 152 */ "orderby_opt ::= ORDER BY sortlist",
/* 153 */ "sortlist ::= sortlist COMMA sortitem sortorder",
/* 154 */ "sortlist ::= sortitem sortorder",
/* 155 */ "sortitem ::= expr",
/* 156 */ "sortorder ::= ASC",
/* 157 */ "sortorder ::= DESC",
/* 158 */ "sortorder ::=",
/* 159 */ "groupby_opt ::=",
/* 160 */ "groupby_opt ::= GROUP BY nexprlist",
/* 161 */ "having_opt ::=",
/* 162 */ "having_opt ::= HAVING expr",
/* 163 */ "limit_opt ::=",
/* 164 */ "limit_opt ::= LIMIT expr",
/* 165 */ "limit_opt ::= LIMIT expr OFFSET expr",
/* 166 */ "limit_opt ::= LIMIT expr COMMA expr",
/* 167 */ "cmd ::= DELETE FROM fullname indexed_opt where_opt",
/* 168 */ "where_opt ::=",
/* 169 */ "where_opt ::= WHERE expr",
/* 170 */ "cmd ::= UPDATE orconf fullname indexed_opt SET setlist where_opt",
/* 171 */ "setlist ::= setlist COMMA nm EQ expr",
/* 172 */ "setlist ::= nm EQ expr",
/* 173 */ "cmd ::= insert_cmd INTO fullname inscollist_opt VALUES LP itemlist RP",
/* 174 */ "cmd ::= insert_cmd INTO fullname inscollist_opt select",
/* 175 */ "cmd ::= insert_cmd INTO fullname inscollist_opt DEFAULT VALUES",
/* 176 */ "insert_cmd ::= INSERT orconf",
/* 177 */ "insert_cmd ::= REPLACE",
/* 178 */ "itemlist ::= itemlist COMMA expr",
/* 179 */ "itemlist ::= expr",
/* 180 */ "inscollist_opt ::=",
/* 181 */ "inscollist_opt ::= LP inscollist RP",
/* 182 */ "inscollist ::= inscollist COMMA nm",
/* 183 */ "inscollist ::= nm",
/* 184 */ "expr ::= term",
/* 185 */ "expr ::= LP expr RP",
/* 186 */ "term ::= NULL",
/* 187 */ "expr ::= id",
/* 188 */ "expr ::= JOIN_KW",
/* 189 */ "expr ::= nm DOT nm",
/* 190 */ "expr ::= nm DOT nm DOT nm",
/* 191 */ "term ::= INTEGER|FLOAT|BLOB",
/* 192 */ "term ::= STRING",
/* 193 */ "expr ::= REGISTER",
/* 194 */ "expr ::= VARIABLE",
/* 195 */ "expr ::= expr COLLATE ids",
/* 196 */ "expr ::= CAST LP expr AS typetoken RP",
/* 197 */ "expr ::= ID LP distinct exprlist RP",
/* 198 */ "expr ::= ID LP STAR RP",
/* 199 */ "term ::= CTIME_KW",
/* 200 */ "expr ::= expr AND expr",
/* 201 */ "expr ::= expr OR expr",
/* 202 */ "expr ::= expr LT|GT|GE|LE expr",
/* 203 */ "expr ::= expr EQ|NE expr",
/* 204 */ "expr ::= expr BITAND|BITOR|LSHIFT|RSHIFT expr",
/* 205 */ "expr ::= expr PLUS|MINUS expr",
/* 206 */ "expr ::= expr STAR|SLASH|REM expr",
/* 207 */ "expr ::= expr CONCAT expr",
/* 208 */ "likeop ::= LIKE_KW",
/* 209 */ "likeop ::= NOT LIKE_KW",
/* 210 */ "likeop ::= MATCH",
/* 211 */ "likeop ::= NOT MATCH",
/* 212 */ "expr ::= expr likeop expr",
/* 213 */ "expr ::= expr likeop expr ESCAPE expr",
/* 214 */ "expr ::= expr ISNULL|NOTNULL",
/* 215 */ "expr ::= expr NOT NULL",
/* 216 */ "expr ::= expr IS expr",
/* 217 */ "expr ::= expr IS NOT expr",
/* 218 */ "expr ::= NOT expr",
/* 219 */ "expr ::= BITNOT expr",
/* 220 */ "expr ::= MINUS expr",
/* 221 */ "expr ::= PLUS expr",
/* 222 */ "between_op ::= BETWEEN",
/* 223 */ "between_op ::= NOT BETWEEN",
/* 224 */ "expr ::= expr between_op expr AND expr",
/* 225 */ "in_op ::= IN",
/* 226 */ "in_op ::= NOT IN",
/* 227 */ "expr ::= expr in_op LP exprlist RP",
/* 228 */ "expr ::= LP select RP",
/* 229 */ "expr ::= expr in_op LP select RP",
/* 230 */ "expr ::= expr in_op nm dbnm",
/* 231 */ "expr ::= EXISTS LP select RP",
/* 232 */ "expr ::= CASE case_operand case_exprlist case_else END",
/* 233 */ "case_exprlist ::= case_exprlist WHEN expr THEN expr",
/* 234 */ "case_exprlist ::= WHEN expr THEN expr",
/* 235 */ "case_else ::= ELSE expr",
/* 236 */ "case_else ::=",
/* 237 */ "case_operand ::= expr",
/* 238 */ "case_operand ::=",
/* 239 */ "exprlist ::= nexprlist",
/* 240 */ "exprlist ::=",
/* 241 */ "nexprlist ::= nexprlist COMMA expr",
/* 242 */ "nexprlist ::= expr",
/* 243 */ "cmd ::= createkw uniqueflag INDEX ifnotexists nm dbnm ON nm LP idxlist RP",
/* 244 */ "uniqueflag ::= UNIQUE",
/* 245 */ "uniqueflag ::=",
/* 246 */ "idxlist_opt ::=",
/* 247 */ "idxlist_opt ::= LP idxlist RP",
/* 248 */ "idxlist ::= idxlist COMMA nm collate sortorder",
/* 249 */ "idxlist ::= nm collate sortorder",
/* 250 */ "collate ::=",
/* 251 */ "collate ::= COLLATE ids",
/* 252 */ "cmd ::= DROP INDEX ifexists fullname",
/* 253 */ "cmd ::= VACUUM",
/* 254 */ "cmd ::= VACUUM nm",
/* 255 */ "cmd ::= PRAGMA nm dbnm",
/* 256 */ "cmd ::= PRAGMA nm dbnm EQ nmnum",
/* 257 */ "cmd ::= PRAGMA nm dbnm LP nmnum RP",
/* 258 */ "cmd ::= PRAGMA nm dbnm EQ minus_num",
/* 259 */ "cmd ::= PRAGMA nm dbnm LP minus_num RP",
/* 260 */ "nmnum ::= plus_num",
/* 261 */ "nmnum ::= nm",
/* 262 */ "nmnum ::= ON",
/* 263 */ "nmnum ::= DELETE",
/* 264 */ "nmnum ::= DEFAULT",
/* 265 */ "plus_num ::= plus_opt number",
/* 266 */ "minus_num ::= MINUS number",
/* 267 */ "number ::= INTEGER|FLOAT",
/* 268 */ "plus_opt ::= PLUS",
/* 269 */ "plus_opt ::=",
/* 270 */ "cmd ::= createkw trigger_decl BEGIN trigger_cmd_list END",
/* 271 */ "trigger_decl ::= temp TRIGGER ifnotexists nm dbnm trigger_time trigger_event ON fullname foreach_clause when_clause",
/* 272 */ "trigger_time ::= BEFORE",
/* 273 */ "trigger_time ::= AFTER",
/* 274 */ "trigger_time ::= INSTEAD OF",
/* 275 */ "trigger_time ::=",
/* 276 */ "trigger_event ::= DELETE|INSERT",
/* 277 */ "trigger_event ::= UPDATE",
/* 278 */ "trigger_event ::= UPDATE OF inscollist",
/* 279 */ "foreach_clause ::=",
/* 280 */ "foreach_clause ::= FOR EACH ROW",
/* 281 */ "when_clause ::=",
/* 282 */ "when_clause ::= WHEN expr",
/* 283 */ "trigger_cmd_list ::= trigger_cmd_list trigger_cmd SEMI",
/* 284 */ "trigger_cmd_list ::= trigger_cmd SEMI",
/* 285 */ "trnm ::= nm",
/* 286 */ "trnm ::= nm DOT nm",
/* 287 */ "tridxby ::=",
/* 288 */ "tridxby ::= INDEXED BY nm",
/* 289 */ "tridxby ::= NOT INDEXED",
/* 290 */ "trigger_cmd ::= UPDATE orconf trnm tridxby SET setlist where_opt",
/* 291 */ "trigger_cmd ::= insert_cmd INTO trnm inscollist_opt VALUES LP itemlist RP",
/* 292 */ "trigger_cmd ::= insert_cmd INTO trnm inscollist_opt select",
/* 293 */ "trigger_cmd ::= DELETE FROM trnm tridxby where_opt",
/* 294 */ "trigger_cmd ::= select",
/* 295 */ "expr ::= RAISE LP IGNORE RP",
/* 296 */ "expr ::= RAISE LP raisetype COMMA nm RP",
/* 297 */ "raisetype ::= ROLLBACK",
/* 298 */ "raisetype ::= ABORT",
/* 299 */ "raisetype ::= FAIL",
/* 300 */ "cmd ::= DROP TRIGGER ifexists fullname",
/* 301 */ "cmd ::= ATTACH database_kw_opt expr AS expr key_opt",
/* 302 */ "cmd ::= DETACH database_kw_opt expr",
/* 303 */ "key_opt ::=",
/* 304 */ "key_opt ::= KEY expr",
/* 305 */ "database_kw_opt ::= DATABASE",
/* 306 */ "database_kw_opt ::=",
/* 307 */ "cmd ::= REINDEX",
/* 308 */ "cmd ::= REINDEX nm dbnm",
/* 309 */ "cmd ::= ANALYZE",
/* 310 */ "cmd ::= ANALYZE nm dbnm",
/* 311 */ "cmd ::= ALTER TABLE fullname RENAME TO nm",
/* 312 */ "cmd ::= ALTER TABLE add_column_fullname ADD kwcolumn_opt column",
/* 313 */ "add_column_fullname ::= fullname",
/* 314 */ "kwcolumn_opt ::=",
/* 315 */ "kwcolumn_opt ::= COLUMNKW",
/* 316 */ "cmd ::= create_vtab",
/* 317 */ "cmd ::= create_vtab LP vtabarglist RP",
/* 318 */ "create_vtab ::= createkw VIRTUAL TABLE nm dbnm USING nm",
/* 319 */ "vtabarglist ::= vtabarg",
/* 320 */ "vtabarglist ::= vtabarglist COMMA vtabarg",
/* 321 */ "vtabarg ::=",
/* 322 */ "vtabarg ::= vtabarg vtabargtoken",
/* 323 */ "vtabargtoken ::= ANY",
/* 324 */ "vtabargtoken ::= lp anylist RP",
/* 325 */ "lp ::= LP",
/* 326 */ "anylist ::=",
/* 327 */ "anylist ::= anylist LP anylist RP",
/* 328 */ "anylist ::= anylist ANY",
};
//#endif
		#if YYSTACKDEPTH
																																																/*
** Try to increase the size of the parser stack.
*/
static void yyGrowStack(yyParser p){
int newSize;
//yyStackEntry pNew;

newSize = p.yystksz*2 + 100;
//pNew = realloc(p.yystack, newSize*sizeof(pNew[0]));
//if( pNew !=null){
p.yystack = Array.Resize(p.yystack,newSize); //pNew;
p.yystksz = newSize;
#if !NDEBUG
																																																if( yyTraceFILE ){
fprintf(yyTraceFILE,"%sStack grows to %d entries!\n",
yyTracePrompt, p.yystksz);
}
#endif
																																																//}
}
#endif
		///<summary>
		/// This function allocates a new parser.
		/// The only argument is a pointer to a function which works like
		/// malloc.
		///
		/// Inputs:
		/// A pointer to the function used to allocate memory.
		///
		/// Outputs:
		/// A pointer to a parser.  This pointer is used in subsequent calls
		/// to sqlite3Parser and sqlite3ParserFree.
		///</summary>
		public static yyParser ParseAlloc() {
			//void *(*mallocProc)(size_t)){
			yyParser pParser=new yyParser();
			//pParser = (yyParser*)(*mallocProc)( (size_t)yyParser.Length );
			if(pParser!=null) {
				pParser.yyidx=-1;
				#if YYTRACKMAXSTACKDEPTH
																																																																																																pParser.yyidxMax=0;
#endif
				#if YYSTACKDEPTH
																																																																																																pParser.yystack = NULL;
pParser.yystksz = null;
yyGrowStack(pParser);
#endif
			}
			return pParser;
		}


        ///<summary>
        ///The following function deletes the value associated with a
        /// symbol.  The symbol can be either a terminal or nonterminal.
        /// "yymajor" is the symbol code, and "yypminor" is a pointer to
        /// the value.
        ///
        ///</summary>
        ///<summary>
        /// Pop the parser's stack once.
        ///
        /// If there is a destructor routine associated with the token which
        /// is popped from the stack, then call it.
        ///
        /// Return the major token number for the symbol popped.
        ///
        ///</summary>
        ///
        ///<summary>
        ///Deallocate and destroy a parser.  Destructors are all called for
        ///all stack elements before shutting the parser down.
        ///
        ///Inputs:
        ///<ul>
        ///<li>  A pointer to the parser.  This should be a pointer
        ///obtained from sqlite3ParserAlloc.
        ///<li>  A pointer to a function used to reclaim memory obtained
        ///from malloc.
        ///</ul>
        ///
        ///</summary>
        ///<summary>
        /// Return the peak depth of the stack for a parser.
        ///
        ///</summary>
#if YYTRACKMAXSTACKDEPTH
																																																int sqlite3ParserStackPeak(void p){
yyParser pParser = (yyParser*)p;
return pParser.yyidxMax;
}
#endif
        ///<summary>
        /// Find the appropriate action for a parser given the terminal
        /// look-ahead token iLookAhead.
        ///
        /// If the look-ahead token is YYNOCODE, then check to see if the action is
        /// independent of the look-ahead.  If it is, return the action, otherwise
        /// return YY_NO_ACTION.
        ///</summary>
        ///<summary>
        /// Find the appropriate action for a parser given the non-terminal
        /// look-ahead token iLookAhead.
        ///
        /// If the look-ahead token is YYNOCODE, then check to see if the action is
        /// independent of the look-ahead.  If it is, return the action, otherwise
        /// return YY_NO_ACTION.
        ///
        ///</summary>


        static int yy_find_reduce_action(
            int stateno,///Current state number 		
		    YYCODETYPE iLookAhead///The look-ahead token
		) {
			Debug.Assert(stateno<=YY_REDUCE_COUNT);
			int i=yy_reduce_ofst[stateno];
			Debug.Assert(i!=YY_REDUCE_USE_DFLT);
			Debug.Assert(iLookAhead!=YYNOCODE_253_VACUUM);
			i+=iLookAhead;
			Debug.Assert(i>=0&&i<YY_ACTTAB_COUNT);
			Debug.Assert(yy_lookahead[i]==iLookAhead);
			return yy_action[i];
		}

		///<    summary>
		/// The following routine is called if the stack overflows.
		///
		///</summary>
		///<summary>
		/// Perform a shift action.
		///
		///</summary>
		///<summary>
		///The following table contains information about every rule that
		/// is used during the reduce.
		///
		///</summary>
		public struct _yyRuleInfo {
			public TokenType lhs;
			///
			///<summary>
			///</summary>
			///<param name="Symbol on the left">hand side of the rule </param>
			public byte nrhs;
			///
			///<summary>
			///</summary>
			///<param name="Number of right">hand side symbols in the rule </param>
			public _yyRuleInfo(YYCODETYPE lhs,byte nrhs) {
				this.lhs=(TokenType)lhs;
				this.nrhs=nrhs;
			}
		}
		#region rule info
		static _yyRuleInfo[] yyRuleInfo=new _yyRuleInfo[] {
			new _yyRuleInfo(142,1),
			new _yyRuleInfo(143,2),
			new _yyRuleInfo(143,1),
			new _yyRuleInfo(144,1),
			new _yyRuleInfo(144,3),
			new _yyRuleInfo(145,0),
			new _yyRuleInfo(145,1),
			new _yyRuleInfo(145,3),
			new _yyRuleInfo(146,1),
			new _yyRuleInfo(147,3),
			new _yyRuleInfo(149,0),
			new _yyRuleInfo(149,1),
			new _yyRuleInfo(149,2),
			new _yyRuleInfo(148,0),
			new _yyRuleInfo(148,1),
			new _yyRuleInfo(148,1),
			new _yyRuleInfo(148,1),
			new _yyRuleInfo(147,2),
			new _yyRuleInfo(147,2),
			new _yyRuleInfo(147,2),
			new _yyRuleInfo(151,1),
			new _yyRuleInfo(151,0),
			new _yyRuleInfo(147,2),
			new _yyRuleInfo(147,3),
			new _yyRuleInfo(147,5),
			new _yyRuleInfo(147,2),
			new _yyRuleInfo(152,6),
			new _yyRuleInfo(154,1),
			new _yyRuleInfo(156,0),
			new _yyRuleInfo(156,3),
			new _yyRuleInfo(155,1),
			new _yyRuleInfo(155,0),
			new _yyRuleInfo(153,4),
			new _yyRuleInfo(153,2),
			new _yyRuleInfo(158,3),
			new _yyRuleInfo(158,1),
			new _yyRuleInfo(161,3),
			new _yyRuleInfo(162,1),
			new _yyRuleInfo(165,1),
			new _yyRuleInfo(165,1),
			new _yyRuleInfo(166,1),
			new _yyRuleInfo(150,1),
			new _yyRuleInfo(150,1),
			new _yyRuleInfo(150,1),
			new _yyRuleInfo(163,0),
			new _yyRuleInfo(163,1),
			new _yyRuleInfo(167,1),
			new _yyRuleInfo(167,4),
			new _yyRuleInfo(167,6),
			new _yyRuleInfo(168,1),
			new _yyRuleInfo(168,2),
			new _yyRuleInfo(169,1),
			new _yyRuleInfo(169,1),
			new _yyRuleInfo(164,2),
			new _yyRuleInfo(164,0),
			new _yyRuleInfo(172,3),
			new _yyRuleInfo(172,1),
			new _yyRuleInfo(173,2),
			new _yyRuleInfo(173,4),
			new _yyRuleInfo(173,3),
			new _yyRuleInfo(173,3),
			new _yyRuleInfo(173,2),
			new _yyRuleInfo(173,2),
			new _yyRuleInfo(173,3),
			new _yyRuleInfo(173,5),
			new _yyRuleInfo(173,2),
			new _yyRuleInfo(173,4),
			new _yyRuleInfo(173,4),
			new _yyRuleInfo(173,1),
			new _yyRuleInfo(173,2),
			new _yyRuleInfo(178,0),
			new _yyRuleInfo(178,1),
			new _yyRuleInfo(180,0),
			new _yyRuleInfo(180,2),
			new _yyRuleInfo(182,2),
			new _yyRuleInfo(182,3),
			new _yyRuleInfo(182,3),
			new _yyRuleInfo(182,3),
			new _yyRuleInfo(183,2),
			new _yyRuleInfo(183,2),
			new _yyRuleInfo(183,1),
			new _yyRuleInfo(183,1),
			new _yyRuleInfo(183,2),
			new _yyRuleInfo(181,3),
			new _yyRuleInfo(181,2),
			new _yyRuleInfo(184,0),
			new _yyRuleInfo(184,2),
			new _yyRuleInfo(184,2),
			new _yyRuleInfo(159,0),
			new _yyRuleInfo(159,2),
			new _yyRuleInfo(185,3),
			new _yyRuleInfo(185,2),
			new _yyRuleInfo(185,1),
			new _yyRuleInfo(186,2),
			new _yyRuleInfo(186,7),
			new _yyRuleInfo(186,5),
			new _yyRuleInfo(186,5),
			new _yyRuleInfo(186,10),
			new _yyRuleInfo(188,0),
			new _yyRuleInfo(188,1),
			new _yyRuleInfo(176,0),
			new _yyRuleInfo(176,3),
			new _yyRuleInfo(189,0),
			new _yyRuleInfo(189,2),
			new _yyRuleInfo(190,1),
			new _yyRuleInfo(190,1),
			new _yyRuleInfo(190,1),
			new _yyRuleInfo(147,4),
			new _yyRuleInfo(192,2),
			new _yyRuleInfo(192,0),
			new _yyRuleInfo(147,8),
			new _yyRuleInfo(147,4),
			new _yyRuleInfo(147,1),
			new _yyRuleInfo(160,1),
			new _yyRuleInfo(160,3),
			new _yyRuleInfo(195,1),
			new _yyRuleInfo(195,2),
			new _yyRuleInfo(195,1),
			new _yyRuleInfo(194,9),
			new _yyRuleInfo(196,1),
			new _yyRuleInfo(196,1),
			new _yyRuleInfo(196,0),
			new _yyRuleInfo(204,2),
			new _yyRuleInfo(204,0),
			new _yyRuleInfo(197,3),
			new _yyRuleInfo(197,2),
			new _yyRuleInfo(197,4),
			new _yyRuleInfo(205,2),
			new _yyRuleInfo(205,1),
			new _yyRuleInfo(205,0),
			new _yyRuleInfo(198,0),
			new _yyRuleInfo(198,2),
			new _yyRuleInfo(207,2),
			new _yyRuleInfo(207,0),
			new _yyRuleInfo(206,7),
			new _yyRuleInfo(206,7),
			new _yyRuleInfo(206,7),
			new _yyRuleInfo(157,0),
			new _yyRuleInfo(157,2),
			new _yyRuleInfo(193,2),
			new _yyRuleInfo(208,1),
			new _yyRuleInfo(208,2),
			new _yyRuleInfo(208,3),
			new _yyRuleInfo(208,4),
			new _yyRuleInfo(210,2),
			new _yyRuleInfo(210,0),
			new _yyRuleInfo(209,0),
			new _yyRuleInfo(209,3),
			new _yyRuleInfo(209,2),
			new _yyRuleInfo(211,4),
			new _yyRuleInfo(211,0),
			new _yyRuleInfo(202,0),
			new _yyRuleInfo(202,3),
			new _yyRuleInfo(214,4),
			new _yyRuleInfo(214,2),
			new _yyRuleInfo(215,1),
			new _yyRuleInfo(177,1),
			new _yyRuleInfo(177,1),
			new _yyRuleInfo(177,0),
			new _yyRuleInfo(200,0),
			new _yyRuleInfo(200,3),
			new _yyRuleInfo(201,0),
			new _yyRuleInfo(201,2),
			new _yyRuleInfo(203,0),
			new _yyRuleInfo(203,2),
			new _yyRuleInfo(203,4),
			new _yyRuleInfo(203,4),
			new _yyRuleInfo(147,5),
			new _yyRuleInfo(199,0),
			new _yyRuleInfo(199,2),
			new _yyRuleInfo(147,7),
			new _yyRuleInfo(217,5),
			new _yyRuleInfo(217,3),
			new _yyRuleInfo(147,8),
			new _yyRuleInfo(147,5),
			new _yyRuleInfo(147,6),
			new _yyRuleInfo(218,2),
			new _yyRuleInfo(218,1),
			new _yyRuleInfo(220,3),
			new _yyRuleInfo(220,1),
			new _yyRuleInfo(219,0),
			new _yyRuleInfo(219,3),
			new _yyRuleInfo(213,3),
			new _yyRuleInfo(213,1),
			new _yyRuleInfo(175,1),
			new _yyRuleInfo(175,3),
			new _yyRuleInfo(174,1),
			new _yyRuleInfo(175,1),
			new _yyRuleInfo(175,1),
			new _yyRuleInfo(175,3),
			new _yyRuleInfo(175,5),
			new _yyRuleInfo(174,1),
			new _yyRuleInfo(174,1),
			new _yyRuleInfo(175,1),
			new _yyRuleInfo(175,1),
			new _yyRuleInfo(175,3),
			new _yyRuleInfo(175,6),
			new _yyRuleInfo(175,5),
			new _yyRuleInfo(175,4),
			new _yyRuleInfo(174,1),
			new _yyRuleInfo(175,3),
			new _yyRuleInfo(175,3),
			new _yyRuleInfo(175,3),
			new _yyRuleInfo(175,3),
			new _yyRuleInfo(175,3),
			new _yyRuleInfo(175,3),
			new _yyRuleInfo(175,3),
			new _yyRuleInfo(175,3),
			new _yyRuleInfo(222,1),
			new _yyRuleInfo(222,2),
			new _yyRuleInfo(222,1),
			new _yyRuleInfo(222,2),
			new _yyRuleInfo(175,3),
			new _yyRuleInfo(175,5),
			new _yyRuleInfo(175,2),
			new _yyRuleInfo(175,3),
			new _yyRuleInfo(175,3),
			new _yyRuleInfo(175,4),
			new _yyRuleInfo(175,2),
			new _yyRuleInfo(175,2),
			new _yyRuleInfo(175,2),
			new _yyRuleInfo(175,2),
			new _yyRuleInfo(223,1),
			new _yyRuleInfo(223,2),
			new _yyRuleInfo(175,5),
			new _yyRuleInfo(224,1),
			new _yyRuleInfo(224,2),
			new _yyRuleInfo(175,5),
			new _yyRuleInfo(175,3),
			new _yyRuleInfo(175,5),
			new _yyRuleInfo(175,4),
			new _yyRuleInfo(175,4),
			new _yyRuleInfo(175,5),
			new _yyRuleInfo(226,5),
			new _yyRuleInfo(226,4),
			new _yyRuleInfo(227,2),
			new _yyRuleInfo(227,0),
			new _yyRuleInfo(225,1),
			new _yyRuleInfo(225,0),
			new _yyRuleInfo(221,1),
			new _yyRuleInfo(221,0),
			new _yyRuleInfo(216,3),
			new _yyRuleInfo(216,1),
			new _yyRuleInfo(147,11),
			new _yyRuleInfo(228,1),
			new _yyRuleInfo(228,0),
			new _yyRuleInfo(179,0),
			new _yyRuleInfo(179,3),
			new _yyRuleInfo(187,5),
			new _yyRuleInfo(187,3),
			new _yyRuleInfo(229,0),
			new _yyRuleInfo(229,2),
			new _yyRuleInfo(147,4),
			new _yyRuleInfo(147,1),
			new _yyRuleInfo(147,2),
			new _yyRuleInfo(147,3),
			new _yyRuleInfo(147,5),
			new _yyRuleInfo(147,6),
			new _yyRuleInfo(147,5),
			new _yyRuleInfo(147,6),
			new _yyRuleInfo(230,1),
			new _yyRuleInfo(230,1),
			new _yyRuleInfo(230,1),
			new _yyRuleInfo(230,1),
			new _yyRuleInfo(230,1),
			new _yyRuleInfo(170,2),
			new _yyRuleInfo(171,2),
			new _yyRuleInfo(232,1),
			new _yyRuleInfo(231,1),
			new _yyRuleInfo(231,0),
			new _yyRuleInfo(147,5),
			new _yyRuleInfo(233,11),
			new _yyRuleInfo(235,1),
			new _yyRuleInfo(235,1),
			new _yyRuleInfo(235,2),
			new _yyRuleInfo(235,0),
			new _yyRuleInfo(236,1),
			new _yyRuleInfo(236,1),
			new _yyRuleInfo(236,3),
			new _yyRuleInfo(237,0),
			new _yyRuleInfo(237,3),
			new _yyRuleInfo(238,0),
			new _yyRuleInfo(238,2),
			new _yyRuleInfo(234,3),
			new _yyRuleInfo(234,2),
			new _yyRuleInfo(240,1),
			new _yyRuleInfo(240,3),
			new _yyRuleInfo(241,0),
			new _yyRuleInfo(241,3),
			new _yyRuleInfo(241,2),
			new _yyRuleInfo(239,7),
			new _yyRuleInfo(239,8),
			new _yyRuleInfo(239,5),
			new _yyRuleInfo(239,5),
			new _yyRuleInfo(239,1),
			new _yyRuleInfo(175,4),
			new _yyRuleInfo(175,6),
			new _yyRuleInfo(191,1),
			new _yyRuleInfo(191,1),
			new _yyRuleInfo(191,1),
			new _yyRuleInfo(147,4),
			new _yyRuleInfo(147,6),
			new _yyRuleInfo(147,3),
			new _yyRuleInfo(243,0),
			new _yyRuleInfo(243,2),
			new _yyRuleInfo(242,1),
			new _yyRuleInfo(242,0),
			new _yyRuleInfo(147,1),
			new _yyRuleInfo(147,3),
			new _yyRuleInfo(147,1),
			new _yyRuleInfo(147,3),
			new _yyRuleInfo(147,6),
			new _yyRuleInfo(147,6),
			new _yyRuleInfo(244,1),
			new _yyRuleInfo(245,0),
			new _yyRuleInfo(245,1),
			new _yyRuleInfo(147,1),
			new _yyRuleInfo(147,4),
			new _yyRuleInfo(246,7),
			new _yyRuleInfo(247,1),
			new _yyRuleInfo(247,3),
			new _yyRuleInfo(248,0),
			new _yyRuleInfo(248,2),
			new _yyRuleInfo(249,1),
			new _yyRuleInfo(249,3),
			new _yyRuleInfo(250,1),
			new _yyRuleInfo(251,0),
			new _yyRuleInfo(251,4),
			new _yyRuleInfo(251,2),
		};
		#endregion
		//static void yy_accept(yyParser*);  /* Forward Declaration */
		///
		///<summary>
		///Perform a reduce action and the shift that must immediately
		///follow the reduce.
		///
		///</summary>
		static void yy_reduce(
            yyParser yypParser,///The parser 
		    int yyruleno///Number of the rule by which to reduce 
		) {
			YYMINORTYPE yygotominor;
			///The LHS of the rule reduced 
			ParseState pParse=yypParser.parseState;
			//sqlite3ParserARG_FETCH;
			var offsettedStackList=new OffsettedList(yypParser,yypParser.yyidx);// yyStackEntry[] yymsp = new yyStackEntry[0];            /* The top of the parser's stack */
                                                               //      yymsp[0] = yypParser.yystack[yypParser.yyidx];
#if !NDEBUG
																																																																								      if ( yyTraceFILE != null && yyruleno >= 0
      && yyruleno < yyRuleName.Length )
      { //(int)(yyRuleName.Length/sizeof(yyRuleName[0])) ){
        fprintf( yyTraceFILE, "%sReduce [%s].\n", yyTracePrompt,
        yyRuleName[yyruleno] );
      }
#endif
            ///
            ///<summary>
            ///Silence complaints from purify about yygotominor being uninitialized
            ///in some cases when it is copied into the stack after the following
            ///switch.  yygotominor is uninitialized when a rule reduces that does
            ///</summary>
            ///<param name="not set the value of its left">hand side nonterminal.  Leaving the</param>
            ///<param name="value of the nonterminal uninitialized is utterly harmless as long">value of the nonterminal uninitialized is utterly harmless as long</param>
            ///<param name="as the value is never used.  So really the only thing this code">as the value is never used.  So really the only thing this code</param>
            ///<param name="accomplishes is to quieten purify.">accomplishes is to quieten purify.</param>
            ///<param name=""></param>
            ///<param name="2007">16:  The wireshark project (www.wireshark.org) reports that</param>
            ///<param name="without this code, their parser segfaults.  I'm not sure what there">without this code, their parser segfaults.  I'm not sure what there</param>
            ///<param name="parser is doing to make this happen.  This is the second bug report">parser is doing to make this happen.  This is the second bug report</param>
            ///<param name="from wireshark this week.  Clearly they are stressing Lemon in ways">from wireshark this week.  Clearly they are stressing Lemon in ways</param>
            ///<param name="that it has not been previously stressed...  (SQLite ticket #2172)">that it has not been previously stressed...  (SQLite ticket #2172)</param>
            yygotominor =new YYMINORTYPE();

            //memset(yygotominor, 0, yygotominor).Length;
            #region switch
            switch(yyruleno) {
			///
			///<summary>
			///Beginning here are the reduction cases.  A typical example
			///follows:
			///case 0:
			/////#line <lineno> <grammarfile>
			///{ ... }           // User supplied code
			/////#line <lineno> <thisfile>
			///break;
			///
			///</summary>
			case 5:
			///
			///<summary>
			///explain ::= 
			///</summary>
			//#line 107 "parse.y"
			{
				build.setupExplain(pParse,0);
			}
			//#line 2107 "parse.c"
			break;
			case 6:
			///
			///<summary>
			///explain ::= EXPLAIN 
			///</summary>
			//#line 109 "parse.y"
			{
				build.setupExplain(pParse,1);
			}
			//#line 2112 "parse.c"
			break;
			case 7:
			///
			///<summary>
			///explain ::= EXPLAIN QUERY PLAN 
			///</summary>
			//#line 110 "parse.y"
			{
				build.setupExplain(pParse,2);
			}
			//#line 2117 "parse.c"
			break;
			case 8:
			///
			///<summary>
			///cmdx ::= cmd 
			///</summary>
			//#line 112 "parse.y"
			{
				build.sqlite3FinishCoding(pParse);
			}
			//#line 2122 "parse.c"
			break;
			case 9:
			///
			///<summary>
			///cmd ::= BEGIN transtype trans_opt 
			///</summary>
			//#line 117 "parse.y"
			{
				build.sqlite3BeginTransaction(pParse,offsettedStackList[-1].minor.yy4_Int);
			}
			//#line 2127 "parse.c"
			break;
			case 13:
			///
			///<summary>
			///transtype ::= 
			///</summary>
			//#line 122 "parse.y"
			{
				yygotominor.yy4_Int=(int)TokenType.TK_DEFERRED;
			}
			//#line 2132 "parse.c"
			break;
			case 14:
			///
			///<summary>
			///transtype ::= DEFERRED 
			///</summary>
			case 15:
			///
			///<summary>
			///transtype ::= IMMEDIATE 
			///</summary>
			//yysqliteinth.testcase(yyruleno==15);
			case 16:
			///
			///<summary>
			///transtype ::= EXCLUSIVE 
			///</summary>
			//yysqliteinth.testcase(yyruleno==16);
			case 115:
			///
			///<summary>
			///multiselect_op ::= UNION 
			///</summary>
			//yysqliteinth.testcase(yyruleno==114);
			case 117:
			///
			///<summary>
			///multiselect_op ::= EXCEPT|INTERSECT 
			///</summary>
			//yysqliteinth.testcase(yyruleno==116);
			//#line 123 "parse.y"
			{
				yygotominor.yy4_Int=(YYCODETYPE)offsettedStackList[0].major;
			}
			//#line 2141 "parse.c"
			break;
			case 17:
			///
			///<summary>
			///cmd ::= COMMIT trans_opt 
			///</summary>
			case 18:
			///
			///<summary>
			///cmd ::= END trans_opt 
			///</summary>
			//yysqliteinth.testcase(yyruleno==18);
			//#line 126 "parse.y"
			{
				build.sqlite3CommitTransaction(pParse);
			}
			//#line 2147 "parse.c"
			break;
			case 19:
			///
			///<summary>
			///cmd ::= ROLLBACK trans_opt 
			///</summary>
			//#line 128 "parse.y"
			{
				build.sqlite3RollbackTransaction(pParse);
			}
			//#line 2152 "parse.c"
			break;
			case 22:
			///
			///<summary>
			///cmd ::= SAVEPOINT nm 
			///</summary>
			//#line 132 "parse.y"
			{
                build.sqlite3Savepoint(pParse, sqliteinth.SAVEPOINT_BEGIN, offsettedStackList[0].minor.yy0Token);
			}
			//#line 2159 "parse.c"
			break;
			case 23:
			///
			///<summary>
			///cmd ::= RELEASE savepoint_opt nm 
			///</summary>
			//#line 135 "parse.y"
			{
                build.sqlite3Savepoint(pParse, sqliteinth.SAVEPOINT_RELEASE, offsettedStackList[0].minor.yy0Token);
			}
			//#line 2166 "parse.c"
			break;
			case 24:
			///
			///<summary>
			///cmd ::= ROLLBACK trans_opt TO savepoint_opt nm 
			///</summary>
			//#line 138 "parse.y"
			{
				build.sqlite3Savepoint(pParse,sqliteinth.SAVEPOINT_ROLLBACK,offsettedStackList[0].minor.yy0Token);
			}
			//#line 2173 "parse.c"
			break;
			case 26:
			///
			///<summary>
			///create_table ::= createkw temp TABLE ifnotexists nm dbnm 
			///</summary>
			//#line 145 "parse.y"
			{
				TableBuilder.sqlite3StartTable(pParse,offsettedStackList[-1].minor.yy0Token,offsettedStackList[0].minor.yy0Token,offsettedStackList[-4].minor.yy4_Int,0,0,offsettedStackList[-2].minor.yy4_Int);
			}
			//#line 2180 "parse.c"
			break;
			case 27:
			///
			///<summary>
			///createkw ::= CREATE 
			///</summary>
			//#line 148 "parse.y"
			{
				pParse.db.lookaside.bEnabled=0;
				yygotominor.yy0Token=offsettedStackList[0].minor.yy0Token;
			}
			//#line 2188 "parse.c"
			break;
			case 28:
			///
			///<summary>
			///ifnotexists ::= 
			///</summary>
			case 31:
			///
			///<summary>
			///temp ::= 
			///</summary>
			//yysqliteinth.testcase(yyruleno == 31);
			case 70:
			///
			///<summary>
			///autoinc ::= 
			///</summary>
			//yysqliteinth.testcase(yyruleno == 70);
			case 83:
			///
			///<summary>
			///defer_subclause ::= NOT DEFERRABLE init_deferred_pred_opt 
			///</summary>
			//yysqliteinth.testcase(yyruleno == 83);
			case 85:
			///
			///<summary>
			///init_deferred_pred_opt ::= 
			///</summary>
			//yysqliteinth.testcase(yyruleno == 85);
			case 87:
			///
			///<summary>
			///init_deferred_pred_opt ::= INITIALLY IMMEDIATE 
			///</summary>
			//yysqliteinth.testcase(yyruleno == 87);
			case 98:
			///
			///<summary>
			///defer_subclause_opt ::= 
			///</summary>
			//yysqliteinth.testcase(yyruleno == 98);
			case 109:
			///
			///<summary>
			///ifexists ::= 
			///</summary>
			//yysqliteinth.testcase(yyruleno == 109);
			case 120:
			///
			///<summary>
			///distinct ::= ALL 
			///</summary>
			//yysqliteinth.testcase(yyruleno == 120);
			case 121:
			///
			///<summary>
			///distinct ::= 
			///</summary>
			//yysqliteinth.testcase(yyruleno == 121);
			case 222:
			///
			///<summary>
			///between_op ::= BETWEEN 
			///</summary>
			//yysqliteinth.testcase(yyruleno == 223);
			case 225:
			///
			///<summary>
			///in_op ::= IN 
			///</summary>
			//yysqliteinth.testcase(yyruleno == 226);
			//#line 153 "parse.y"
			{
				yygotominor.yy4_Int=0;
			}
			//#line 2204 "parse.c"
			break;
			case 29:
			///
			///<summary>
			///ifnotexists ::= IF NOT EXISTS 
			///</summary>
			case 30:
			///
			///<summary>
			///temp ::= TEMP 
			///</summary>
			//yysqliteinth.testcase(yyruleno == 30);
			case 71:
			///
			///<summary>
			///autoinc ::= AUTOINCR 
			///</summary>
			//yysqliteinth.testcase(yyruleno == 71);
			case 86:
			///
			///<summary>
			///init_deferred_pred_opt ::= INITIALLY DEFERRED 
			///</summary>
			//yysqliteinth.testcase(yyruleno == 86);
			case 108:
			///
			///<summary>
			///ifexists ::= IF EXISTS 
			///</summary>
			//yysqliteinth.testcase(yyruleno == 108);
			case 119:
			///
			///<summary>
			///distinct ::= DISTINCT 
			///</summary>
			//yysqliteinth.testcase(yyruleno == 119);
			case 223:
			///
			///<summary>
			///between_op ::= NOT BETWEEN 
			///</summary>
			//yysqliteinth.testcase(yyruleno == 224);
			case 226:
			///
			///<summary>
			///in_op ::= NOT IN 
			///</summary>
			//yysqliteinth.testcase(yyruleno == 227);
			//#line 154 "parse.y"
			{
				yygotominor.yy4_Int=1;
			}
			//#line 2216 "parse.c"
			break;
			case 32:
			///
			///<summary>
			///create_table_args ::= LP columnlist conslist_opt RP 
			///</summary>
			//#line 160 "parse.y"
			{
				pParse.sqlite3EndTable(offsettedStackList[-1].minor.yy0Token,offsettedStackList[0].minor.yy0Token,0);
			}
			//#line 2223 "parse.c"
			break;
			case 33:
			///
			///<summary>
			///create_table_args ::= AS select 
			///</summary>
			//#line 163 "parse.y"
			{
                pParse.sqlite3EndTable(0, 0, offsettedStackList[0].minor.yy387_Select);
				SelectMethods.SelectDestructor(pParse.db,ref offsettedStackList[0].minor.yy387_Select);
			}
			//#line 2231 "parse.c"
			break;
			case 36:
			///
			///<summary>
			///column ::= columnid type carglist 
			///</summary>
			//#line 175 "parse.y"
			{
				//yygotominor.yy0.z = yymsp[-2].minor.yy0.z;
				//yygotominor.yy0.n = (int)( pParse.sLastToken.z - yymsp[-2].minor.yy0.z ) + pParse.sLastToken.n; 
				yygotominor.yy0Token.Length=(int)(offsettedStackList[-2].minor.yy0Token.zRestSql.Length-pParse.sLastToken.zRestSql.Length)+pParse.sLastToken.Length;
				yygotominor.yy0Token.zRestSql=offsettedStackList[-2].minor.yy0Token.zRestSql.Substring(0,yygotominor.yy0Token.Length);
			}
			//#line 2239 "parse.c"
			break;
			case 37:
			///
			///<summary>
			///columnid ::= nm 
			///</summary>
			//#line 179 "parse.y"
			{
				build.parse_AddColumn(pParse,offsettedStackList[0].minor.yy0Token);
				yygotominor.yy0Token=offsettedStackList[0].minor.yy0Token;
			}
			//#line 2247 "parse.c"
			break;
			case 38:
			///
			///<summary>
			///id ::= ID 
			///</summary>
			case 39:
			///
			///<summary>
			///id ::= INDEXED 
			///</summary>
			//yysqliteinth.testcase(yyruleno==39);
			case 40:
			///
			///<summary>
			///ids ::= ID|STRING 
			///</summary>
			//yysqliteinth.testcase(yyruleno==40);
			case 41:
			///
			///<summary>
			///nm ::= id 
			///</summary>
			//yysqliteinth.testcase(yyruleno==41);
			case 42:
			///
			///<summary>
			///nm ::= STRING 
			///</summary>
			//yysqliteinth.testcase(yyruleno==42);
			case 43:
			///
			///<summary>
			///nm ::= JOIN_KW 
			///</summary>
			//yysqliteinth.testcase(yyruleno==43);
			case 46:
			///
			///<summary>
			///typetoken ::= typename 
			///</summary>
			//yysqliteinth.testcase(yyruleno==46);
			case 49:
			///
			///<summary>
			///typename ::= ids 
			///</summary>
			//yysqliteinth.testcase(yyruleno==49);
			case 127:
			///
			///<summary>
			///as ::= AS nm 
			///</summary>
			////yysqliteinth.testcase(yyruleno == 127);
			case 128:
			///
			///<summary>
			///as ::= ids 
			///</summary>
			////yysqliteinth.testcase(yyruleno == 128);
			case 138:
			///
			///<summary>
			///dbnm ::= DOT nm 
			///</summary>
			////yysqliteinth.testcase(yyruleno == 138);
			case 147:
			///
			///<summary>
			///indexed_opt ::= INDEXED BY nm 
			///</summary>
			////yysqliteinth.testcase(yyruleno == 147);
			case 251:
			///
			///<summary>
			///collate ::= COLLATE ids 
			///</summary>
			//sqliteinth.testcase(yyruleno == 251);
			case 260:
			///
			///<summary>
			///nmnum ::= plus_num 
			///</summary>
			//sqliteinth.testcase(yyruleno == 260);
			case 261:
			///
			///<summary>
			///nmnum ::= nm 
			///</summary>
			//sqliteinth.testcase(yyruleno == 261);
			case 262:
			///
			///<summary>
			///nmnum ::= ON 
			///</summary>
			//sqliteinth.testcase(yyruleno == 262);
			case 263:
			///
			///<summary>
			///nmnum ::= DELETE 
			///</summary>
			//sqliteinth.testcase(yyruleno == 263);
			case 264:
			///
			///<summary>
			///nmnum ::= DEFAULT 
			///</summary>
			//sqliteinth.testcase(yyruleno == 264);
			case 265:
			///
			///<summary>
			///plus_num ::= plus_opt number 
			///</summary>
			//sqliteinth.testcase(yyruleno == 265);
			case 266:
			///
			///<summary>
			///minus_num ::= MINUS number 
			///</summary>
			//sqliteinth.testcase(yyruleno == 266);
			case 267:
			///
			///<summary>
			///number ::= INTEGER|FLOAT 
			///</summary>
			//sqliteinth.testcase(yyruleno == 267);
			case 285:
			///
			///<summary>
			///trnm ::= nm 
			///</summary>
			//sqliteinth.testcase(yyruleno == 285);
			//#line 189 "parse.y"
			{
				yygotominor.yy0Token=offsettedStackList[0].minor.yy0Token;
			}
			//#line 2273 "parse.c"
			break;
			case 45:
			///
			///<summary>
			///type ::= typetoken 
			///</summary>
			//#line 251 "parse.y"
			{
				build.InitColumnType(pParse,offsettedStackList[0].minor.yy0Token);
			}
			//#line 2278 "parse.c"
			break;
			case 47:
			///
			///<summary>
			///typetoken ::= typename LP signed RP 
			///</summary>
			//#line 253 "parse.y"
			{
				//yygotominor.yy0.z = yymsp[-3].minor.yy0.z;
				//yygotominor.yy0.n = (int)( &yymsp[0].minor.yy0.z[yymsp[0].minor.yy0.n] - yymsp[-3].minor.yy0.z );
				yygotominor.yy0Token.Length=offsettedStackList[-3].minor.yy0Token.zRestSql.Length-offsettedStackList[0].minor.yy0Token.zRestSql.Length+offsettedStackList[0].minor.yy0Token.Length;
				yygotominor.yy0Token.zRestSql=offsettedStackList[-3].minor.yy0Token.zRestSql.Substring(0,yygotominor.yy0Token.Length);
			}
			//#line 2286 "parse.c"
			break;
			case 48:
			///
			///<summary>
			///typetoken ::= typename LP signed COMMA signed RP 
			///</summary>
			//#line 257 "parse.y"
			{
				//yygotominor.yy0.z = yymsp[-5].minor.yy0.z;
				//yygotominor.yy0.n = (int)( &yymsp[0].minor.yy0.z[yymsp[0].minor.yy0.n] - yymsp[-5].minor.yy0.z );
				yygotominor.yy0Token.Length=offsettedStackList[-5].minor.yy0Token.zRestSql.Length-offsettedStackList[0].minor.yy0Token.zRestSql.Length+1;
				yygotominor.yy0Token.zRestSql=offsettedStackList[-5].minor.yy0Token.zRestSql.Substring(0,yygotominor.yy0Token.Length);
			}
			//#line 2294 "parse.c"
			break;
			case 50:
			///
			///<summary>
			///typename ::= typename ids 
			///</summary>
			//#line 263 "parse.y"
			{
				//yygotominor.yy0.z=yymsp[-1].minor.yy0.z; yygotominor.yy0.n=yymsp[0].minor.yy0.n+(int)(yymsp[0].minor.yy0.z-yymsp[-1].minor.yy0.z);
				yygotominor.yy0Token.zRestSql=offsettedStackList[-1].minor.yy0Token.zRestSql;
				yygotominor.yy0Token.Length=offsettedStackList[0].minor.yy0Token.Length+(int)(offsettedStackList[-1].minor.yy0Token.zRestSql.Length-offsettedStackList[0].minor.yy0Token.zRestSql.Length);
			}
			//#line 2299 "parse.c"
			break;
			case 57:
			///
			///<summary>
			///ccons ::= DEFAULT term 
			///</summary>
			case 59:
			///
			///<summary>
			///ccons ::= DEFAULT PLUS term 
			///</summary>
			//yysqliteinth.testcase(yyruleno==59);
			//#line 274 "parse.y"
			{
				build.sqlite3AddDefaultValue(pParse,offsettedStackList[0].minor.yy118_ExprSpan);
			}
			//#line 2308 "parse.c"
			break;
			case 58:
			///
			///<summary>
			///ccons ::= DEFAULT LP expr RP 
			///</summary>
			//#line 275 "parse.y"
			{
				build.sqlite3AddDefaultValue(pParse,offsettedStackList[-1].minor.yy118_ExprSpan);
			}
			//#line 2310 "parse.c"
			break;
			case 60:
			///
			///<summary>
			///ccons ::= DEFAULT MINUS term 
			///</summary>
			//#line 277 "parse.y"
			{
				ExprSpan v=new ExprSpan();
				v.pExpr=pParse.sqlite3PExpr(TokenType.TK_UMINUS,offsettedStackList[0].minor.yy118_ExprSpan.pExpr,0,0);
				v.zStart=offsettedStackList[-1].minor.yy0Token.zRestSql;
				v.zEnd=offsettedStackList[0].minor.yy118_ExprSpan.zEnd;
				build.sqlite3AddDefaultValue(pParse,v);
			}
			//#line 2321 "parse.c"
			break;
			case 61:
			///
			///<summary>
			///ccons ::= DEFAULT id 
			///</summary>
			//#line 284 "parse.y"
			{
				ExprSpan v=new ExprSpan();
				v.spanExpr(pParse,TokenType.TK_STRING,offsettedStackList[0].minor.yy0Token);
				build.sqlite3AddDefaultValue(pParse,v);
			}
			//#line 2330 "parse.c"
			break;
			case 63:
			///ccons ::= NOT NULL onconf 
			//#line 294 "parse.y"
			{
				build.sqlite3AddNotNull(pParse,offsettedStackList[0].minor.yy4_Int);
			}
			//#line 2335 "parse.c"
			break;
			case 64:
			///ccons ::= PRIMARY KEY sortorder onconf autoinc 
			//#line 296 "parse.y"
			{
				build.sqlite3AddPrimaryKey(pParse,0,(OnConstraintError)offsettedStackList[-1].minor.yy4_Int,offsettedStackList[0].minor.yy4_Int,(SortOrder)offsettedStackList[-2].minor.yy4_Int);
			}
			//#line 2340 "parse.c"
			break;
			case 65:
			///ccons ::= UNIQUE onconf 
			//#line 297 "parse.y"
			{
                pParse.sqlite3CreateIndex(0, 0, 0, 0, (OnConstraintError)offsettedStackList[0].minor.yy4_Int, 0, 0, (SortOrder)0, 0);
			}
			//#line 2345 "parse.c"
			break;
			case 66:
			///ccons ::= CHECK LP expr RP 
			//#line 298 "parse.y"
			{
				build.sqlite3AddCheckConstraint(pParse,offsettedStackList[-1].minor.yy118_ExprSpan.pExpr);
			}
			//#line 2350 "parse.c"
			break;
			case 67:
			///ccons ::= REFERENCES nm idxlist_opt refargs 
			//#line 300 "parse.y"
			{
				build.sqlite3CreateForeignKey(pParse,0,offsettedStackList[-2].minor.yy0Token,offsettedStackList[-1].minor._ExprList,offsettedStackList[0].minor.yy4_Int);
			}
			//#line 2355 "parse.c"
			break;
			case 68:
			///
			///<summary>
			///ccons ::= defer_subclause 
			///</summary>
			//#line 301 "parse.y"
			{
				build.sqlite3DeferForeignKey(pParse,offsettedStackList[0].minor.yy4_Int);
			}
			//#line 2360 "parse.c"
			break;
			case 69:
			///
			///<summary>
			///ccons ::= COLLATE ids 
			///</summary>
			//#line 302 "parse.y"
			{
				build.sqlite3AddCollateType(pParse,offsettedStackList[0].minor.yy0Token);
			}
			//#line 2365 "parse.c"
			break;
			case 72:
			///
			///<summary>
			///refargs ::= 
			///</summary>
			//#line 315 "parse.y"
			{
				yygotominor.yy4_Int=(int)OnConstraintError.OE_None*0x0101;
				///
				///<summary>
				///</summary>
				///<param name="EV: R">45884 </param>
			}
			//#line 2370 "parse.c"
			break;
			case 73:
			///
			///<summary>
			///refargs ::= refargs refarg 
			///</summary>
			//#line 316 "parse.y"
			{
				yygotominor.yy4_Int=(offsettedStackList[-1].minor.yy4_Int&~offsettedStackList[0].minor.yy215.mask)|offsettedStackList[0].minor.yy215.value;
			}
			//#line 2375 "parse.c"
			break;
			case 74:
			///
			///<summary>
			///refarg ::= MATCH nm 
			///</summary>
			case 75:
			///
			///<summary>
			///refarg ::= ON INSERT refact 
			///</summary>
			//yysqliteinth.testcase(yyruleno==75);
			//#line 318 "parse.y"
			{
				yygotominor.yy215.value=0;
				yygotominor.yy215.mask=0x000000;
			}
			//#line 2381 "parse.c"
			break;
			case 76:
			///
			///<summary>
			///refarg ::= ON DELETE refact 
			///</summary>
			//#line 320 "parse.y"
			{
				yygotominor.yy215.value=offsettedStackList[0].minor.yy4_Int;
				yygotominor.yy215.mask=0x0000ff;
			}
			//#line 2386 "parse.c"
			break;
			case 77:
			///
			///<summary>
			///refarg ::= ON UPDATE refact 
			///</summary>
			//#line 321 "parse.y"
			{
				yygotominor.yy215.value=offsettedStackList[0].minor.yy4_Int<<8;
				yygotominor.yy215.mask=0x00ff00;
			}
			//#line 2391 "parse.c"
			break;
			case 78:
			///
			///<summary>
			///refact ::= SET NULL 
			///</summary>
			//#line 323 "parse.y"
			{
				yygotominor.yy4_Int=(int)OnConstraintError.OE_SetNull;
				///
				///<summary>
				///</summary>
				///<param name="EV: R">45252 </param>
			}
			//#line 2396 "parse.c"
			break;
			case 79:
			///
			///<summary>
			///refact ::= SET DEFAULT 
			///</summary>
			//#line 324 "parse.y"
			{
				yygotominor.yy4_Int=(int)OnConstraintError.OE_SetDflt;
				///
				///<summary>
				///</summary>
				///<param name="EV: R">45252 </param>
			}
			//#line 2401 "parse.c"
			break;
			case 80:
			///
			///<summary>
			///refact ::= CASCADE 
			///</summary>
			//#line 325 "parse.y"
			{
				yygotominor.yy4_Int=(int)OnConstraintError.OE_Cascade;
				///
				///<summary>
				///</summary>
				///<param name="EV: R">45252 </param>
			}
			//#line 2406 "parse.c"
			break;
			case 81:
			///
			///<summary>
			///refact ::= RESTRICT 
			///</summary>
			//#line 326 "parse.y"
			{
				yygotominor.yy4_Int=(int)OnConstraintError.OE_Restrict;
				///
				///<summary>
				///</summary>
				///<param name="EV: R">45252 </param>
			}
			//#line 2411 "parse.c"
			break;
			case 82:
			///
			///<summary>
			///refact ::= NO ACTION 
			///</summary>
			//#line 327 "parse.y"
			{
				yygotominor.yy4_Int=(int)OnConstraintError.OE_None;
				///
				///<summary>
				///</summary>
				///<param name="EV: R">45252 </param>
			}
			//#line 2416 "parse.c"
			break;
			case 84:
			///
			///<summary>
			///defer_subclause ::= DEFERRABLE init_deferred_pred_opt 
			///</summary>
			case 99:
			///
			///<summary>
			///defer_subclause_opt ::= defer_subclause 
			///</summary>
			//yysqliteinth.testcase(yyruleno==99);
			case 101:
			///
			///<summary>
			///onconf ::= ON CONFLICT resolvetype 
			///</summary>
			//yysqliteinth.testcase(yyruleno==101);
			case 104:
			///
			///<summary>
			///resolvetype ::= raisetype 
			///</summary>
			//yysqliteinth.testcase(yyruleno==104);
			//#line 330 "parse.y"
			{
				yygotominor.yy4_Int=offsettedStackList[0].minor.yy4_Int;
			}
			//#line 2424 "parse.c"
			break;
			case 88:
			///
			///<summary>
			///conslist_opt ::= 
			///</summary>
			//#line 339 "parse.y"
			{
				yygotominor.yy0Token.Length=0;
				yygotominor.yy0Token.zRestSql=null;
			}
			//#line 2429 "parse.c"
			break;
			case 89:
			///
			///<summary>
			///conslist_opt ::= COMMA conslist 
			///</summary>
			//#line 340 "parse.y"
			{
				yygotominor.yy0Token=offsettedStackList[-1].minor.yy0Token;
			}
			//#line 2434 "parse.c"
			break;
			case 94:
			///
			///<summary>
			///tcons ::= PRIMARY KEY LP idxlist autoinc RP onconf 
			///</summary>
			//#line 346 "parse.y"
			{
                build.sqlite3AddPrimaryKey(pParse, offsettedStackList[-3].minor._ExprList, (OnConstraintError)offsettedStackList[0].minor.yy4_Int, offsettedStackList[-2].minor.yy4_Int, (SortOrder)0);
			}
			//#line 2439 "parse.c"
			break;
			case 95:
			///
			///<summary>
			///tcons ::= UNIQUE LP idxlist RP onconf 
			///</summary>
			//#line 348 "parse.y"
			{
                pParse.sqlite3CreateIndex(0, 0, 0, offsettedStackList[-2].minor._ExprList, (OnConstraintError)offsettedStackList[0].minor.yy4_Int, 0, 0, (SortOrder)0, 0);
			}
			//#line 2444 "parse.c"
			break;
			case 96:
			///
			///<summary>
			///tcons ::= CHECK LP expr RP onconf 
			///</summary>
			//#line 350 "parse.y"
			{
				build.sqlite3AddCheckConstraint(pParse,offsettedStackList[-2].minor.yy118_ExprSpan.pExpr);
			}
			//#line 2449 "parse.c"
			break;
			case 97:
			///
			///<summary>
			///tcons ::= FOREIGN KEY LP idxlist RP REFERENCES nm idxlist_opt refargs defer_subclause_opt 
			///</summary>
			//#line 352 "parse.y"
			{
				build.sqlite3CreateForeignKey(pParse,offsettedStackList[-6].minor._ExprList,offsettedStackList[-3].minor.yy0Token,offsettedStackList[-2].minor._ExprList,offsettedStackList[-1].minor.yy4_Int);
				build.sqlite3DeferForeignKey(pParse,offsettedStackList[0].minor.yy4_Int);
			}
			//#line 2457 "parse.c"
			break;
			case 100:
			///
			///<summary>
			///onconf ::= 
			///</summary>
			//#line 366 "parse.y"
			{
				yygotominor.yy4_Int=(int)OnConstraintError.OE_Default;
			}
			//#line 2462 "parse.c"
			break;
			case 102:
			///
			///<summary>
			///orconf ::= 
			///</summary>
			//#line 368 "parse.y"
			{
                yygotominor.yy210 = (int)OnConstraintError.OE_Default;
			}
			//#line 2467 "parse.c"
			break;
			case 103:
			///
			///<summary>
			///orconf ::= OR resolvetype 
			///</summary>
			//#line 369 "parse.y"
			{
				yygotominor.yy210=(u8)offsettedStackList[0].minor.yy4_Int;
			}
			//#line 2472 "parse.c"
			break;
			case 105:
			///
			///<summary>
			///resolvetype ::= IGNORE 
			///</summary>
			//#line 371 "parse.y"
			{
                yygotominor.yy4_Int = (int)OnConstraintError.OE_Ignore;
			}
			//#line 2477 "parse.c"
			break;
			case 106:
			///
			///<summary>
			///resolvetype ::= REPLACE 
			///</summary>
			//#line 372 "parse.y"
			{
                yygotominor.yy4_Int = (int)OnConstraintError.OE_Replace;
			}
			//#line 2482 "parse.c"
			break;
			case 107:
			///
			///<summary>
			///cmd ::= DROP TABLE ifexists fullname 
			///</summary>
			//#line 376 "parse.y"
			{
				TableBuilder.sqlite3DropTable(pParse,offsettedStackList[0].minor.yy259_SrcList,false,offsettedStackList[-1].minor.yy4_Int);
			}
			//#line 2489 "parse.c"
			break;
			case 110:
			///
			///<summary>
			///cmd ::= createkw temp VIEW ifnotexists nm dbnm AS select 
			///</summary>
			//#line 386 "parse.y"
			{
				build.sqlite3CreateView(pParse,offsettedStackList[-7].minor.yy0Token,offsettedStackList[-3].minor.yy0Token,offsettedStackList[-2].minor.yy0Token,offsettedStackList[0].minor.yy387_Select,offsettedStackList[-6].minor.yy4_Int,offsettedStackList[-4].minor.yy4_Int);
			}
			//#line 2496 "parse.c"
			break;
			case 111:
			///
			///<summary>
			///cmd ::= DROP VIEW ifexists fullname 
			///</summary>
			//#line 389 "parse.y"
			{
				TableBuilder.sqlite3DropTable(pParse,offsettedStackList[0].minor.yy259_SrcList,true,offsettedStackList[-1].minor.yy4_Int);
			}
			//#line 2503 "parse.c"
			break;
			case 112:
			///
			///<summary>
			///cmd ::= select 
			///</summary>
			//#line 396 "parse.y"
			{
				SelectDest dest=new SelectDest(SelectResultType.Output,'\0',0,0,0);
                        Compiler.CodeGeneration.ForSelect.codegenSelect(pParse,offsettedStackList[0].minor.yy387_Select,ref dest);
				SelectMethods.SelectDestructor(pParse.db,ref offsettedStackList[0].minor.yy387_Select);
			}
			//#line 2512 "parse.c"
			break;
			case 113:
			///
			///<summary>
			///select ::= oneselect 
			///</summary>
			//#line 407 "parse.y"
			{
				yygotominor.yy387_Select=offsettedStackList[0].minor.yy387_Select;
			}
			//#line 2517 "parse.c"
			break;
			case 114:
			///
			///<summary>
			///select ::= select multiselect_op oneselect 
			///</summary>
			//#line 409 "parse.y"
			{
				if(offsettedStackList[0].minor.yy387_Select!=null) {
					offsettedStackList[0].minor.yy387_Select.TokenOp=(TokenType)offsettedStackList[-1].minor.yy4_Int;
					offsettedStackList[0].minor.yy387_Select.pPrior=offsettedStackList[-2].minor.yy387_Select;
				}
				else {
					SelectMethods.SelectDestructor(pParse.db,ref offsettedStackList[-2].minor.yy387_Select);
				}
				yygotominor.yy387_Select=offsettedStackList[0].minor.yy387_Select;
			}
			//#line 2530 "parse.c"
			break;
			case 116:
			///
			///<summary>
			///multiselect_op ::= UNION ALL 
			///</summary>
			//#line 420 "parse.y"
			{
				yygotominor.yy4_Int=(int)TokenType.TK_ALL;
			}
			//#line 2535 "parse.c"
			break;
			case 118:
			///
			///<summary>
			///oneselect ::= SELECT distinct selcollist from where_opt groupby_opt having_opt orderby_opt limit_opt 
			///</summary>
			//#line 424 "parse.y"
			{
				yygotominor.yy387_Select=Select.Create(
                    pParse,offsettedStackList[-6].minor._ExprList,
                    offsettedStackList[-5].minor.yy259_SrcList,
                    offsettedStackList[-4].minor.yy314_Expr,
                    offsettedStackList[-3].minor._ExprList,
                    offsettedStackList[-2].minor.yy314_Expr,
                    offsettedStackList[-1].minor._ExprList,
                    offsettedStackList[-7].minor.yy4_Int,
                    offsettedStackList[0].minor.yy292_LimitVal.pLimit,
                    offsettedStackList[0].minor.yy292_LimitVal.pOffset);
			}
			//#line 2542 "parse.c"
			break;
			case 122:
			///
			///<summary>
			///sclp ::= selcollist COMMA 
			///</summary>
			case 247:
			///
			///<summary>
			///idxlist_opt ::= LP idxlist RP 
			///</summary>
			//yysqliteinth.testcase(yyruleno==247);
			//#line 445 "parse.y"
			{
				yygotominor._ExprList=offsettedStackList[-1].minor._ExprList;
			}
			//#line 2548 "parse.c"
			break;
			case 123:
			///
			///<summary>
			///sclp ::= 
			///</summary>
			case 151:
			///
			///<summary>
			///orderby_opt ::= 
			///</summary>
			//yysqliteinth.testcase(yyruleno==151);
			case 159:
			///
			///<summary>
			///groupby_opt ::= 
			///</summary>
			//yysqliteinth.testcase(yyruleno==159);
			case 240:
			///
			///<summary>
			///exprlist ::= 
			///</summary>
			//yysqliteinth.testcase(yyruleno==240);
			case 246:
			///
			///<summary>
			///idxlist_opt ::= 
			///</summary>
			//yysqliteinth.testcase(yyruleno==246);
			//#line 446 "parse.y"
			{
				yygotominor._ExprList=null;
			}
			//#line 2557 "parse.c"
			break;
			case 124:
			///
			///<summary>
			///selcollist ::= sclp expr as 
			///</summary>
			//#line 447 "parse.y"
			{
				yygotominor._ExprList= offsettedStackList[-2].minor._ExprList.Append(offsettedStackList[-1].minor.yy118_ExprSpan.pExpr);
				if(offsettedStackList[0].minor.yy0Token.Length>0)
					pParse.sqlite3ExprListSetName(yygotominor._ExprList,offsettedStackList[0].minor.yy0Token,1);
				pParse.sqlite3ExprListSetSpan(yygotominor._ExprList,offsettedStackList[-1].minor.yy118_ExprSpan);
			}
			//#line 2566 "parse.c"
			break;
			case 125:
			///
			///<summary>
			///selcollist ::= sclp STAR 
			///</summary>
			//#line 452 "parse.y"
			{
				Expr p=exprc.sqlite3Expr(pParse.db,TokenType.TK_ALL,null);
				yygotominor._ExprList= offsettedStackList[-1].minor._ExprList.Append(p);
			}
			//#line 2574 "parse.c"
			break;
			case 126:
			///
			///<summary>
			///selcollist ::= sclp nm DOT STAR 
			///</summary>
			//#line 456 "parse.y"
			{
				Expr pRight=pParse.sqlite3PExpr(TokenType.TK_ALL,0,0,offsettedStackList[0].minor.yy0Token);
				Expr pLeft=pParse.sqlite3PExpr(TokenType.TK_ID,0,0,offsettedStackList[-2].minor.yy0Token);
				Expr pDot=pParse.sqlite3PExpr(TokenType.TK_DOT,pLeft,pRight,0);
				yygotominor._ExprList= offsettedStackList[-3].minor._ExprList.Append(pDot);
			}
			//#line 2584 "parse.c"
			break;
			case 129:
			///
			///<summary>
			///as ::= 
			///</summary>
			//#line 469 "parse.y"
			{
				yygotominor.yy0Token.Length=0;
			}
			//#line 2589 "parse.c"
			break;
			case 130:
			///
			///<summary>
			///from ::= 
			///</summary>
			//#line 481 "parse.y"
			{
				yygotominor.yy259_SrcList=new SrcList();
			}
			//sqlite3DbMallocZero(pParse.db, sizeof(*yygotominor.yy259));}
			//#line 2594 "parse.c"
			break;
			case 131:
			///
			///<summary>
			///from ::= FROM seltablist 
			///</summary>
			//#line 482 "parse.y"
			{
				yygotominor.yy259_SrcList=offsettedStackList[0].minor.yy259_SrcList;
				build.sqlite3SrcListShiftJoinType(yygotominor.yy259_SrcList);
			}
			//#line 2602 "parse.c"
			break;
			case 132:
			///
			///<summary>
			///stl_prefix ::= seltablist joinop 
			///</summary>
			//#line 490 "parse.y"
			{
				yygotominor.yy259_SrcList=offsettedStackList[-1].minor.yy259_SrcList;
				if(Sqlite3.ALWAYS(yygotominor.yy259_SrcList!=null&&yygotominor.yy259_SrcList.Count>0))
					yygotominor.yy259_SrcList.a[yygotominor.yy259_SrcList.Count-1].jointype=(JoinType)offsettedStackList[0].minor.yy4_Int;
			}
			//#line 2610 "parse.c"
			break;
			case 133:
			///
			///<summary>
			///stl_prefix ::= 
			///</summary>
			//#line 494 "parse.y"
			{
				yygotominor.yy259_SrcList=null;
			}
			//#line 2615 "parse.c"
			break;
			case 134:
			///
			///<summary>
			///seltablist ::= stl_prefix nm dbnm as indexed_opt on_opt using_opt 
			///</summary>
			//#line 495 "parse.y"
			{
				yygotominor.yy259_SrcList=build.sqlite3SrcListAppendFromTerm(pParse,offsettedStackList[-6].minor.yy259_SrcList,offsettedStackList[-5].minor.yy0Token,offsettedStackList[-4].minor.yy0Token,offsettedStackList[-3].minor.yy0Token,0,offsettedStackList[-1].minor.yy314_Expr,offsettedStackList[0].minor.yy384_IdList);
				build.sqlite3SrcListIndexedBy(pParse,yygotominor.yy259_SrcList,offsettedStackList[-2].minor.yy0Token);
			}
			//#line 2623 "parse.c"
			break;
			case 135:
			///
			///<summary>
			///seltablist ::= stl_prefix LP select RP as on_opt using_opt 
			///</summary>
			//#line 501 "parse.y"
			{
				yygotominor.yy259_SrcList=build.sqlite3SrcListAppendFromTerm(pParse,offsettedStackList[-6].minor.yy259_SrcList,0,0,offsettedStackList[-2].minor.yy0Token,offsettedStackList[-4].minor.yy387_Select,offsettedStackList[-1].minor.yy314_Expr,offsettedStackList[0].minor.yy384_IdList);
			}
			//#line 2630 "parse.c"
			break;
			case 136:
			///
			///<summary>
			///seltablist ::= stl_prefix LP seltablist RP as on_opt using_opt 
			///</summary>
			//#line 505 "parse.y"
			{
				if(offsettedStackList[-6].minor.yy259_SrcList==null&&offsettedStackList[-2].minor.yy0Token.Length==0&&offsettedStackList[-1].minor.yy314_Expr==null&&offsettedStackList[0].minor.yy384_IdList==null) {
					yygotominor.yy259_SrcList=offsettedStackList[-4].minor.yy259_SrcList;
				}
				else {
					Select pSubquery;
					build.sqlite3SrcListShiftJoinType(offsettedStackList[-4].minor.yy259_SrcList);
					pSubquery=Select.sqlite3SelectNew(pParse,0,offsettedStackList[-4].minor.yy259_SrcList,0,0,0,0,0,0,0);
					yygotominor.yy259_SrcList=build.sqlite3SrcListAppendFromTerm(pParse,offsettedStackList[-6].minor.yy259_SrcList,0,0,offsettedStackList[-2].minor.yy0Token,pSubquery,offsettedStackList[-1].minor.yy314_Expr,offsettedStackList[0].minor.yy384_IdList);
				}
			}
			//#line 2644 "parse.c"
			break;
			case 137:
			///
			///<summary>
			///dbnm ::= 
			///</summary>
			case 146:
			///
			///<summary>
			///indexed_opt ::= 
			///</summary>
			//yysqliteinth.testcase(yyruleno==146);
			//#line 530 "parse.y"
			{
				yygotominor.yy0Token.zRestSql=null;
				yygotominor.yy0Token.Length=0;
			}
			//#line 2650 "parse.c"
			break;
			case 139:
			///
			///<summary>
			///fullname ::= nm dbnm 
			///</summary>
			//#line 535 "parse.y"
			{
				yygotominor.yy259_SrcList=build.sqlite3SrcListAppend(pParse.db,0,offsettedStackList[-1].minor.yy0Token,offsettedStackList[0].minor.yy0Token);
			}
			//#line 2655 "parse.c"
			break;
			case 140:
			///
			///<summary>
			///joinop ::= COMMA|JOIN 
			///</summary>
			//#line 539 "parse.y"
			{
                yygotominor.yy4_Int = (int) JoinType.JT_INNER;
			}
			//#line 2660 "parse.c"
			break;
			case 141:
			///
			///<summary>
			///joinop ::= JOIN_KW JOIN 
			///</summary>
			//#line 540 "parse.y"
			{
				yygotominor.yy4_Int=(int)SelectMethods.sqlite3JoinType(pParse,offsettedStackList[-1].minor.yy0Token,0,0);
			}
			//#line 2665 "parse.c"
			break;
			case 142:
			///
			///<summary>
			///joinop ::= JOIN_KW nm JOIN 
			///</summary>
			//#line 541 "parse.y"
			{
                yygotominor.yy4_Int = (int)SelectMethods.sqlite3JoinType(pParse, offsettedStackList[-2].minor.yy0Token, offsettedStackList[-1].minor.yy0Token, 0);
			}
			//#line 2670 "parse.c"
			break;
			case 143:
			///
			///<summary>
			///joinop ::= JOIN_KW nm nm JOIN 
			///</summary>
			//#line 543 "parse.y"
			{
                yygotominor.yy4_Int =(int) SelectMethods.sqlite3JoinType(pParse, offsettedStackList[-3].minor.yy0Token, offsettedStackList[-2].minor.yy0Token, offsettedStackList[-1].minor.yy0Token);
			}
			//#line 2675 "parse.c"
			break;
			case 144:
			///
			///<summary>
			///on_opt ::= ON expr 
			///</summary>
			case 155:
			///
			///<summary>
			///sortitem ::= expr 
			///</summary>
			//yysqliteinth.testcase(yyruleno==155);
			case 162:
			///
			///<summary>
			///having_opt ::= HAVING expr 
			///</summary>
			//yysqliteinth.testcase(yyruleno==162);
			case 169:
			///
			///<summary>
			///where_opt ::= WHERE expr 
			///</summary>
			//yysqliteinth.testcase(yyruleno==169);
			case 235:
			///
			///<summary>
			///case_else ::= ELSE expr 
			///</summary>
			//yysqliteinth.testcase(yyruleno==235);
			case 237:
			///
			///<summary>
			///case_operand ::= expr 
			///</summary>
			//yysqliteinth.testcase(yyruleno==237);
			//#line 547 "parse.y"
			{
				yygotominor.yy314_Expr=offsettedStackList[0].minor.yy118_ExprSpan.pExpr;
			}
			//#line 2685 "parse.c"
			break;
			case 145:
			///
			///<summary>
			///on_opt ::= 
			///</summary>
			case 161:
			///
			///<summary>
			///having_opt ::= 
			///</summary>
			//yysqliteinth.testcase(yyruleno==161);
			case 168:
			///
			///<summary>
			///where_opt ::= 
			///</summary>
			//yysqliteinth.testcase(yyruleno==168);
			case 236:
			///
			///<summary>
			///case_else ::= 
			///</summary>
			//yysqliteinth.testcase(yyruleno==236);
			case 238:
			///
			///<summary>
			///case_operand ::= 
			///</summary>
			//yysqliteinth.testcase(yyruleno==238);
			//#line 548 "parse.y"
			{
				yygotominor.yy314_Expr=null;
			}
			//#line 2694 "parse.c"
			break;
			case 148:
			///
			///<summary>
			///indexed_opt ::= NOT INDEXED 
			///</summary>
			//#line 563 "parse.y"
			{
				yygotominor.yy0Token.zRestSql=null;
				yygotominor.yy0Token.Length=1;
			}
			//#line 2699 "parse.c"
			break;
			case 149:
			///
			///<summary>
			///using_opt ::= USING LP inscollist RP 
			///</summary>
			case 181:
			///
			///<summary>
			///inscollist_opt ::= LP inscollist RP 
			///</summary>
			//yysqliteinth.testcase(yyruleno==181);
			//#line 567 "parse.y"
			{
				yygotominor.yy384_IdList=offsettedStackList[-1].minor.yy384_IdList;
			}
			//#line 2705 "parse.c"
			break;
			case 150:
			///
			///<summary>
			///using_opt ::= 
			///</summary>
			case 180:
			///
			///<summary>
			///inscollist_opt ::= 
			///</summary>
			//yysqliteinth.testcase(yyruleno==180);
			//#line 568 "parse.y"
			{
				yygotominor.yy384_IdList=null;
			}
			//#line 2711 "parse.c"
			break;
			case 152:
			///
			///<summary>
			///orderby_opt ::= ORDER BY sortlist 
			///</summary>
			case 160:
			///
			///<summary>
			///groupby_opt ::= GROUP BY nexprlist 
			///</summary>
			//yysqliteinth.testcase(yyruleno==160);
			case 239:
			///
			///<summary>
			///exprlist ::= nexprlist 
			///</summary>
			//yysqliteinth.testcase(yyruleno==239);
			//#line 579 "parse.y"
			{
				yygotominor._ExprList=offsettedStackList[0].minor._ExprList;
			}
			//#line 2718 "parse.c"
			break;
			case 153:
			///
			///<summary>
			///sortlist ::= sortlist COMMA sortitem sortorder 
			///</summary>
			//#line 580 "parse.y"
			{
				yygotominor._ExprList= offsettedStackList[-3].minor._ExprList.Append(offsettedStackList[-1].minor.yy314_Expr);
				if(yygotominor._ExprList!=null)
					yygotominor._ExprList.a[yygotominor._ExprList.Count-1].sortOrder=(SortOrder)offsettedStackList[0].minor.yy4_Int;
			}
			//#line 2726 "parse.c"
			break;
			case 154:
			///
			///<summary>
			///sortlist ::= sortitem sortorder 
			///</summary>
			//#line 584 "parse.y"
			{
				yygotominor._ExprList=CollectionExtensions.Append(null,offsettedStackList[-1].minor.yy314_Expr);
				if(yygotominor._ExprList!=null&&Sqlite3.ALWAYS(yygotominor._ExprList.a!=null))
					yygotominor._ExprList.a[0].sortOrder=(SortOrder)offsettedStackList[0].minor.yy4_Int;
			}
			//#line 2734 "parse.c"
			break;
			case 156:
			///
			///<summary>
			///sortorder ::= ASC 
			///</summary>
			case 158:
			///
			///<summary>
			///sortorder ::= 
			///</summary>
			//yysqliteinth.testcase(yyruleno==158);
			//#line 592 "parse.y"
			{
                yygotominor.yy4_Int = (int)SortOrder.SQLITE_SO_ASC;
			}
			//#line 2740 "parse.c"
			break;
			case 157:
			///
			///<summary>
			///sortorder ::= DESC 
			///</summary>
			//#line 593 "parse.y"
			{
				yygotominor.yy4_Int=(int)SortOrder.SQLITE_SO_DESC;
			}
			//#line 2745 "parse.c"
			break;
			case 163:
			///
			///<summary>
			///limit_opt ::= 
			///</summary>
			//#line 619 "parse.y"
			{
				yygotominor.yy292_LimitVal.pLimit=null;
				yygotominor.yy292_LimitVal.pOffset=null;
			}
			//#line 2750 "parse.c"
			break;
			case 164:
			///
			///<summary>
			///limit_opt ::= LIMIT expr 
			///</summary>
			//#line 620 "parse.y"
			{
				yygotominor.yy292_LimitVal.pLimit=offsettedStackList[0].minor.yy118_ExprSpan.pExpr;
				yygotominor.yy292_LimitVal.pOffset=null;
			}
			//#line 2755 "parse.c"
			break;
			case 165:
			///
			///<summary>
			///limit_opt ::= LIMIT expr OFFSET expr 
			///</summary>
			//#line 622 "parse.y"
			{
				yygotominor.yy292_LimitVal.pLimit=offsettedStackList[-2].minor.yy118_ExprSpan.pExpr;
				yygotominor.yy292_LimitVal.pOffset=offsettedStackList[0].minor.yy118_ExprSpan.pExpr;
			}
			//#line 2760 "parse.c"
			break;
			case 166:
			///
			///<summary>
			///limit_opt ::= LIMIT expr COMMA expr 
			///</summary>
			//#line 624 "parse.y"
			{
				yygotominor.yy292_LimitVal.pOffset=offsettedStackList[-2].minor.yy118_ExprSpan.pExpr;
				yygotominor.yy292_LimitVal.pLimit=offsettedStackList[0].minor.yy118_ExprSpan.pExpr;
			}
			//#line 2765 "parse.c"
			break;
			case 167:
			///
			///<summary>
			///cmd ::= DELETE FROM fullname indexed_opt where_opt 
			///</summary>
			//#line 637 "parse.y"
			{
				build.sqlite3SrcListIndexedBy(pParse,offsettedStackList[-2].minor.yy259_SrcList,offsettedStackList[-1].minor.yy0Token);
				pParse.sqlite3DeleteFrom(offsettedStackList[-2].minor.yy259_SrcList,offsettedStackList[0].minor.yy314_Expr);
			}
			//#line 2773 "parse.c"
			break;
			case 170:
			///
			///<summary>
			///cmd ::= UPDATE orconf fullname indexed_opt SET setlist where_opt 
			///</summary>
			//#line 660 "parse.y"
			{
				build.sqlite3SrcListIndexedBy(pParse,offsettedStackList[-4].minor.yy259_SrcList,offsettedStackList[-3].minor.yy0Token);
				pParse.sqlite3ExprListCheckLength(offsettedStackList[-1].minor._ExprList,"set list");
				pParse.sqlite3Update(offsettedStackList[-4].minor.yy259_SrcList,offsettedStackList[-1].minor._ExprList,offsettedStackList[0].minor.yy314_Expr, (OnConstraintError)offsettedStackList[-5].minor.yy210);
			}
			//#line 2782 "parse.c"
			break;
			case 171:
			///
			///<summary>
			///setlist ::= setlist COMMA nm EQ expr 
			///</summary>
			//#line 670 "parse.y"
			{
				yygotominor._ExprList= offsettedStackList[-4].minor._ExprList.Append(offsettedStackList[0].minor.yy118_ExprSpan.pExpr);
				pParse.sqlite3ExprListSetName(yygotominor._ExprList,offsettedStackList[-2].minor.yy0Token,1);
			}
			//#line 2790 "parse.c"
			break;
			case 172:
			///
			///<summary>
			///setlist ::= nm EQ expr 
			///</summary>
			//#line 674 "parse.y"
			{
				yygotominor._ExprList=CollectionExtensions.Append(null,offsettedStackList[0].minor.yy118_ExprSpan.pExpr);
				pParse.sqlite3ExprListSetName(yygotominor._ExprList,offsettedStackList[-2].minor.yy0Token,1);
			}
			//#line 2798 "parse.c"
			break;
			case 173:
			///
			///<summary>
			///cmd ::= insert_cmd INTO fullname inscollist_opt VALUES LP itemlist RP 
			///</summary>
			//#line 683 "parse.y"
			{
				pParse.sqlite3Insert(offsettedStackList[-5].minor.yy259_SrcList,offsettedStackList[-1].minor._ExprList,0,offsettedStackList[-4].minor.yy384_IdList,(OnConstraintError)offsettedStackList[-7].minor.yy210);
			}
			//#line 2803 "parse.c"
			break;
			case 174:
			///
			///<summary>
			///cmd ::= insert_cmd INTO fullname inscollist_opt select 
			///</summary>
			//#line 685 "parse.y"
			{
				pParse.sqlite3Insert(offsettedStackList[-2].minor.yy259_SrcList,0,offsettedStackList[0].minor.yy387_Select,offsettedStackList[-1].minor.yy384_IdList,(OnConstraintError)offsettedStackList[-4].minor.yy210);
			}
			//#line 2808 "parse.c"
			break;
			case 175:
			///
			///<summary>
			///cmd ::= insert_cmd INTO fullname inscollist_opt DEFAULT VALUES 
			///</summary>
			//#line 687 "parse.y"
			{
				pParse.sqlite3Insert(offsettedStackList[-3].minor.yy259_SrcList,0,0,offsettedStackList[-2].minor.yy384_IdList,(OnConstraintError)offsettedStackList[-5].minor.yy210);
			}
			//#line 2813 "parse.c"
			break;
			case 176:
			///
			///<summary>
			///insert_cmd ::= INSERT orconf 
			///</summary>
			//#line 690 "parse.y"
			{
				yygotominor.yy210=offsettedStackList[0].minor.yy210;
			}
			//#line 2818 "parse.c"
			break;
			case 177:
			///
			///<summary>
			///insert_cmd ::= REPLACE 
			///</summary>
			//#line 691 "parse.y"
			{
				yygotominor.yy210=(int)OnConstraintError.OE_Replace;
			}
			//#line 2823 "parse.c"
			break;
			case 178:
			///
			///<summary>
			///itemlist ::= itemlist COMMA expr 
			///</summary>
			case 241:
			///
			///<summary>
			///nexprlist ::= nexprlist COMMA expr 
			///</summary>
			//yysqliteinth.testcase(yyruleno==241);
			//#line 698 "parse.y"
			{
				yygotominor._ExprList= offsettedStackList[-2].minor._ExprList.Append(offsettedStackList[0].minor.yy118_ExprSpan.pExpr);
			}
			//#line 2829 "parse.c"
			break;
			case 179:
			///
			///<summary>
			///itemlist ::= expr 
			///</summary>
			case 242:
			///
			///<summary>
			///nexprlist ::= expr 
			///</summary>
			//yysqliteinth.testcase(yyruleno==242);
			//#line 700 "parse.y"
			{
				yygotominor._ExprList=CollectionExtensions.Append(null,offsettedStackList[0].minor.yy118_ExprSpan.pExpr);
			}
			//#line 2835 "parse.c"
			break;
			case 182:
			///
			///<summary>
			///inscollist ::= inscollist COMMA nm 
			///</summary>
			//#line 710 "parse.y"
			{
				yygotominor.yy384_IdList=build.sqlite3IdListAppend(pParse.db,offsettedStackList[-2].minor.yy384_IdList,offsettedStackList[0].minor.yy0Token);
			}
			//#line 2840 "parse.c"
			break;
			case 183:
			///
			///<summary>
			///inscollist ::= nm 
			///</summary>
			//#line 712 "parse.y"
			{
				yygotominor.yy384_IdList=build.sqlite3IdListAppend(pParse.db,0,offsettedStackList[0].minor.yy0Token);
			}
			//#line 2845 "parse.c"
			break;
			case 184:
			///
			///<summary>
			///expr ::= term 
			///</summary>
			//#line 743 "parse.y"
			{
				yygotominor.yy118_ExprSpan=offsettedStackList[0].minor.yy118_ExprSpan;
			}
			//#line 2850 "parse.c"
			break;
			case 185:
			///
			///<summary>
			///expr ::= LP expr RP 
			///</summary>
			//#line 744 "parse.y"
			{
				yygotominor.yy118_ExprSpan.pExpr=offsettedStackList[-1].minor.yy118_ExprSpan.pExpr;
				yygotominor.yy118_ExprSpan.spanSet(offsettedStackList[-2].minor.yy0Token,offsettedStackList[0].minor.yy0Token);
			}
			//#line 2855 "parse.c"
			break;
			case 186:
			///
			///<summary>
			///term ::= NULL 
			///</summary>
			case 191:
			///
			///<summary>
			///term ::= INTEGER|FLOAT|BLOB 
			///</summary>
			//yysqliteinth.testcase(yyruleno==191);
			case 192:
			///
			///<summary>
			///term ::= STRING 
			///</summary>
			//yysqliteinth.testcase(yyruleno==192);
			//#line 745 "parse.y"
			{
				yygotominor.yy118_ExprSpan.spanExpr(pParse,(TokenType)offsettedStackList[0].major,offsettedStackList[0].minor.yy0Token);
			}
			//#line 2862 "parse.c"
			break;
			case 187:
			///
			///<summary>
			///expr ::= id 
			///</summary>
			case 188:
			///
			///<summary>
			///expr ::= JOIN_KW 
			///</summary>
			//yysqliteinth.testcase(yyruleno==188);
			//#line 746 "parse.y"
			{
				yygotominor.yy118_ExprSpan.spanExpr(pParse,TokenType.TK_ID,offsettedStackList[0].minor.yy0Token);
			}
			//#line 2868 "parse.c"
			break;
			case 189:
			///
			///<summary>
			///expr ::= nm DOT nm 
			///</summary>
			//#line 748 "parse.y"
			{
				Expr temp1=pParse.sqlite3PExpr(TokenType.TK_ID,0,0,offsettedStackList[-2].minor.yy0Token);
				Expr temp2=pParse.sqlite3PExpr(TokenType.TK_ID,0,0,offsettedStackList[0].minor.yy0Token);
				yygotominor.yy118_ExprSpan.pExpr=pParse.sqlite3PExpr(TokenType.TK_DOT,temp1,temp2,0);
				yygotominor.yy118_ExprSpan.spanSet(offsettedStackList[-2].minor.yy0Token,offsettedStackList[0].minor.yy0Token);
			}
			//#line 2878 "parse.c"
			break;
			case 190:
			///
			///<summary>
			///expr ::= nm DOT nm DOT nm 
			///</summary>
			//#line 754 "parse.y"
			{
				Expr temp1=pParse.sqlite3PExpr(TokenType.TK_ID,0,0,offsettedStackList[-4].minor.yy0Token);
				Expr temp2=pParse.sqlite3PExpr(TokenType.TK_ID,0,0,offsettedStackList[-2].minor.yy0Token);
				Expr temp3=pParse.sqlite3PExpr(TokenType.TK_ID,0,0,offsettedStackList[0].minor.yy0Token);
				Expr temp4=pParse.sqlite3PExpr(TokenType.TK_DOT,temp2,temp3,0);
				yygotominor.yy118_ExprSpan.pExpr=pParse.sqlite3PExpr(TokenType.TK_DOT,temp1,temp4,0);
				yygotominor.yy118_ExprSpan.spanSet(offsettedStackList[-4].minor.yy0Token,offsettedStackList[0].minor.yy0Token);
			}
			//#line 2890 "parse.c"
			break;
			case 193:
			///
			///<summary>
			///expr ::= REGISTER 
			///</summary>
			//#line 764 "parse.y"
			{
				///
				///<summary>
				///When doing a nested parse, one can include terms in an expression
				///that look like this:   #1 #2 ...  These terms refer to registers
				///</summary>
				///<param name="in the virtual machine.  #N is the N">th register. </param>
				if(pParse.nested==0) {
					utilc.sqlite3ErrorMsg(pParse,"near \"%T\": syntax error",offsettedStackList[0].minor.yy0Token);
					yygotominor.yy118_ExprSpan.pExpr=null;
				}
				else {
					yygotominor.yy118_ExprSpan.pExpr=pParse.sqlite3PExpr(TokenType.TK_REGISTER,0,0,offsettedStackList[0].minor.yy0Token);
					if(yygotominor.yy118_ExprSpan.pExpr!=null)
						Converter.sqlite3GetInt32(offsettedStackList[0].minor.yy0Token.zRestSql,1,ref yygotominor.yy118_ExprSpan.pExpr.iTable);
				}
				yygotominor.yy118_ExprSpan.spanSet(offsettedStackList[0].minor.yy0Token,offsettedStackList[0].minor.yy0Token);
			}
			//#line 2907 "parse.c"
			break;
			case 194:
			///
			///<summary>
			///expr ::= VARIABLE 
			///</summary>
			//#line 777 "parse.y"
			{
				yygotominor.yy118_ExprSpan.spanExpr(pParse,TokenType.TK_VARIABLE,offsettedStackList[0].minor.yy0Token);
				pParse.sqlite3ExprAssignVarNumber(yygotominor.yy118_ExprSpan.pExpr);
				yygotominor.yy118_ExprSpan.spanSet(offsettedStackList[0].minor.yy0Token,offsettedStackList[0].minor.yy0Token);
			}
			//#line 2916 "parse.c"
			break;
			case 195:
			///
			///<summary>
			///expr ::= expr COLLATE ids 
			///</summary>
			//#line 782 "parse.y"
			{
				yygotominor.yy118_ExprSpan.pExpr=pParse.sqlite3ExprSetCollByToken(offsettedStackList[-2].minor.yy118_ExprSpan.pExpr,offsettedStackList[0].minor.yy0Token);
				yygotominor.yy118_ExprSpan.zStart=offsettedStackList[-2].minor.yy118_ExprSpan.zStart;
				yygotominor.yy118_ExprSpan.zEnd=offsettedStackList[0].minor.yy0Token.zRestSql.Substring(offsettedStackList[0].minor.yy0Token.Length);
				//z[yymsp[0].minor.yy0.n];
			}
			//#line 2925 "parse.c"
			break;
			case 196:
			///
			///<summary>
			///expr ::= CAST LP expr AS typetoken RP 
			///</summary>
			//#line 788 "parse.y"
			{
				yygotominor.yy118_ExprSpan.pExpr=pParse.sqlite3PExpr(TokenType.TK_CAST,offsettedStackList[-3].minor.yy118_ExprSpan.pExpr,0,offsettedStackList[-1].minor.yy0Token);
				yygotominor.yy118_ExprSpan.spanSet(offsettedStackList[-5].minor.yy0Token,offsettedStackList[0].minor.yy0Token);
			}
			//#line 2933 "parse.c"
			break;
			case 197:
			///
			///<summary>
			///expr ::= ID LP distinct exprlist RP 
			///</summary>
			//#line 793 "parse.y"
			{
				if(offsettedStackList[-1].minor._ExprList!=null&&offsettedStackList[-1].minor._ExprList.Count>pParse.db.aLimit[Globals.SQLITE_LIMIT_FUNCTION_ARG]) {
					utilc.sqlite3ErrorMsg(pParse,"too many arguments on function %T",offsettedStackList[-4].minor.yy0Token);
				}
				yygotominor.yy118_ExprSpan.pExpr=pParse.sqlite3ExprFunction(offsettedStackList[-1].minor._ExprList,offsettedStackList[-4].minor.yy0Token);
				yygotominor.yy118_ExprSpan.spanSet(offsettedStackList[-4].minor.yy0Token,offsettedStackList[0].minor.yy0Token);
				if(offsettedStackList[-2].minor.yy4_Int!=0&&yygotominor.yy118_ExprSpan.pExpr!=null) {
                    yygotominor.yy118_ExprSpan.pExpr.Flags |= ExprFlags.EP_Distinct;
				}
			}
			//#line 2947 "parse.c"
			break;
			case 198:
			///
			///<summary>
			///expr ::= ID LP STAR RP 
			///</summary>
			//#line 803 "parse.y"
			{
				yygotominor.yy118_ExprSpan.pExpr=pParse.sqlite3ExprFunction(0,offsettedStackList[-3].minor.yy0Token);
				yygotominor.yy118_ExprSpan.spanSet(offsettedStackList[-3].minor.yy0Token,offsettedStackList[0].minor.yy0Token);
			}
			//#line 2955 "parse.c"
			break;
			case 199:
			///
			///<summary>
			///term ::= CTIME_KW 
			///</summary>
			//#line 807 "parse.y"
			{
				///
				///<summary>
				///The CURRENT_TIME, CURRENT_DATE, and CURRENT_TIMESTAMP values are
				///treated as functions that return constants 
				///</summary>
				yygotominor.yy118_ExprSpan.pExpr=pParse.sqlite3ExprFunction(0,offsettedStackList[0].minor.yy0Token);
				if(yygotominor.yy118_ExprSpan.pExpr!=null) {
					yygotominor.yy118_ExprSpan.pExpr.Operator=TokenType.TK_CONST_FUNC;
				}
				yygotominor.yy118_ExprSpan.spanSet(offsettedStackList[0].minor.yy0Token,offsettedStackList[0].minor.yy0Token);
			}
			//#line 2968 "parse.c"
			break;
			case 200:
			///
			///<summary>
			///expr ::= expr AND expr 
			///</summary>
			case 201:
			///
			///<summary>
			///expr ::= expr OR expr 
			///</summary>
			//yysqliteinth.testcase(yyruleno==201);
			case 202:
			///
			///<summary>
			///expr ::= expr LT|GT|GE|LE expr 
			///</summary>
			//yysqliteinth.testcase(yyruleno==202);
			case 203:
			///
			///<summary>
			///expr ::= expr EQ|NE expr 
			///</summary>
			//yysqliteinth.testcase(yyruleno==203);
			case 204:
			///
			///<summary>
			///expr ::= expr BITAND|BITOR|LSHIFT|RSHIFT expr 
			///</summary>
			//yysqliteinth.testcase(yyruleno==204);
			case 205:
			///
			///<summary>
			///expr ::= expr PLUS|MINUS expr 
			///</summary>
			//yysqliteinth.testcase(yyruleno==205);
			case 206:
			///
			///<summary>
			///expr ::= expr STAR|SLASH|REM expr 
			///</summary>
			//yysqliteinth.testcase(yyruleno==206);
			case 207:
			///
			///<summary>
			///expr ::= expr CONCAT expr 
			///</summary>
			//yysqliteinth.testcase(yyruleno==207);
			//#line 834 "parse.y"
			{
				yygotominor.yy118_ExprSpan.spanBinaryExpr(pParse,(TokenType)offsettedStackList[-1].major,offsettedStackList[-2].minor.yy118_ExprSpan,offsettedStackList[0].minor.yy118_ExprSpan);
			}
			//#line 2980 "parse.c"
			break;
			case 208:
			///
			///<summary>
			///likeop ::= LIKE_KW 
			///</summary>
			case 210:
			///
			///<summary>
			///likeop ::= MATCH 
			///</summary>
			//yysqliteinth.testcase(yyruleno==210);
			//#line 847 "parse.y"
			{
				yygotominor.yy342_LikeOp.eOperator=offsettedStackList[0].minor.yy0Token;
				yygotominor.yy342_LikeOp.not=false;
			}
			//#line 2986 "parse.c"
			break;
			case 209:
			///
			///<summary>
			///likeop ::= NOT LIKE_KW 
			///</summary>
			case 211:
			///
			///<summary>
			///likeop ::= NOT MATCH 
			///</summary>
			//yysqliteinth.testcase(yyruleno==211);
			//#line 848 "parse.y"
			{
				yygotominor.yy342_LikeOp.eOperator=offsettedStackList[0].minor.yy0Token;
				yygotominor.yy342_LikeOp.not=true;
			}
			//#line 2992 "parse.c"
			break;
			case 212:
			///
			///<summary>
			///expr ::= expr likeop expr 
			///</summary>
			//#line 851 "parse.y"
			{
				ExprList pList;
				pList=CollectionExtensions
                            .Append(null,offsettedStackList[0].minor.yy118_ExprSpan.pExpr)
                            .Append(offsettedStackList[-2].minor.yy118_ExprSpan.pExpr);
                            
				yygotominor.yy118_ExprSpan.pExpr=pParse.sqlite3ExprFunction(pList,offsettedStackList[-1].minor.yy342_LikeOp.eOperator);
				if(offsettedStackList[-1].minor.yy342_LikeOp.not)
					yygotominor.yy118_ExprSpan.pExpr=pParse.sqlite3PExpr(TokenType.TK_NOT,yygotominor.yy118_ExprSpan.pExpr,0,0);
				yygotominor.yy118_ExprSpan.zStart=offsettedStackList[-2].minor.yy118_ExprSpan.zStart;
				yygotominor.yy118_ExprSpan.zEnd=offsettedStackList[0].minor.yy118_ExprSpan.zEnd;
				if(yygotominor.yy118_ExprSpan.pExpr!=null)
                    yygotominor.yy118_ExprSpan.pExpr.Flags |= ExprFlags.EP_InfixFunc;
			}
			//#line 3006 "parse.c"
			break;
			case 213:
			///
			///<summary>
			///expr ::= expr likeop expr ESCAPE expr 
			///</summary>
			//#line 861 "parse.y"
			{
				ExprList pList;
				pList=CollectionExtensions
                            .Append(null,offsettedStackList[-2].minor.yy118_ExprSpan.pExpr)
                            .Append(offsettedStackList[-4].minor.yy118_ExprSpan.pExpr)
                            .Append(offsettedStackList[0].minor.yy118_ExprSpan.pExpr);

				yygotominor.yy118_ExprSpan.pExpr=pParse.sqlite3ExprFunction(pList,offsettedStackList[-3].minor.yy342_LikeOp.eOperator);
				if(offsettedStackList[-3].minor.yy342_LikeOp.not)
					yygotominor.yy118_ExprSpan.pExpr=pParse.sqlite3PExpr(TokenType.TK_NOT,yygotominor.yy118_ExprSpan.pExpr,0,0);
				yygotominor.yy118_ExprSpan.zStart=offsettedStackList[-4].minor.yy118_ExprSpan.zStart;
				yygotominor.yy118_ExprSpan.zEnd=offsettedStackList[0].minor.yy118_ExprSpan.zEnd;
				if(yygotominor.yy118_ExprSpan.pExpr!=null)
                    yygotominor.yy118_ExprSpan.pExpr.Flags |= ExprFlags.EP_InfixFunc;
			}
			//#line 3021 "parse.c"
			break;
			case 214:
			///
			///<summary>
			///expr ::= expr ISNULL|NOTNULL 
			///</summary>
			//#line 889 "parse.y"
			{
				yygotominor.yy118_ExprSpan.spanUnaryPostfix(pParse,(Engine.OpCode)offsettedStackList[0].major,offsettedStackList[-1].minor.yy118_ExprSpan,offsettedStackList[0].minor.yy0Token);
			}
			//#line 3026 "parse.c"
			break;
			case 215:
			///
			///<summary>
			///expr ::= expr NOT NULL 
			///</summary>
			//#line 890 "parse.y"
			{
				yygotominor.yy118_ExprSpan.spanUnaryPostfix(pParse,TokenType.TK_NOTNULL,offsettedStackList[-2].minor.yy118_ExprSpan,offsettedStackList[0].minor.yy0Token);
			}
			//#line 3031 "parse.c"
			break;
			case 216:
			///
			///<summary>
			///expr ::= expr IS expr 
			///</summary>
			//#line 911 "parse.y"
			{
				yygotominor.yy118_ExprSpan.spanBinaryExpr(pParse,TokenType.TK_IS,offsettedStackList[-2].minor.yy118_ExprSpan,offsettedStackList[0].minor.yy118_ExprSpan);
				pParse.binaryToUnaryIfNull(offsettedStackList[0].minor.yy118_ExprSpan.pExpr,yygotominor.yy118_ExprSpan.pExpr,(int)TokenType.TK_ISNULL);
			}
			//#line 3039 "parse.c"
			break;
			case 217:
			///
			///<summary>
			///expr ::= expr IS NOT expr 
			///</summary>
			//#line 915 "parse.y"
			{
				yygotominor.yy118_ExprSpan.spanBinaryExpr(pParse,TokenType.TK_ISNOT,offsettedStackList[-3].minor.yy118_ExprSpan,offsettedStackList[0].minor.yy118_ExprSpan);
				pParse.binaryToUnaryIfNull(offsettedStackList[0].minor.yy118_ExprSpan.pExpr,yygotominor.yy118_ExprSpan.pExpr,(int)TokenType.TK_NOTNULL);
			}
			//#line 3047 "parse.c"
			break;
			case 218:
			///
			///<summary>
			///expr ::= NOT expr 
			///</summary>
			case 219:
			///
			///<summary>
			///expr ::= BITNOT expr 
			///</summary>
			//yysqliteinth.testcase(yyruleno==219);
			//#line 938 "parse.y"
			{
				yygotominor.yy118_ExprSpan.spanUnaryPrefix(pParse,(TokenType)offsettedStackList[-1].major,offsettedStackList[0].minor.yy118_ExprSpan,offsettedStackList[-1].minor.yy0Token);
			}
			//#line 3053 "parse.c"
			break;
			case 220:
			///
			///<summary>
			///expr ::= MINUS expr 
			///</summary>
			//#line 941 "parse.y"
			{
				yygotominor.yy118_ExprSpan.spanUnaryPrefix(pParse, TokenType.TK_UMINUS,offsettedStackList[0].minor.yy118_ExprSpan,offsettedStackList[-1].minor.yy0Token);
			}
			//#line 3058 "parse.c"
			break;
			case 221:
			///
			///<summary>
			///expr ::= PLUS expr 
			///</summary>
			//#line 943 "parse.y"
			{
				yygotominor.yy118_ExprSpan.spanUnaryPrefix(pParse,TokenType.TK_UPLUS,offsettedStackList[0].minor.yy118_ExprSpan,offsettedStackList[-1].minor.yy0Token);
			}
			//#line 3063 "parse.c"
			break;
			case 224:
			///
			///<summary>
			///expr ::= expr between_op expr AND expr 
			///</summary>
			//#line 948 "parse.y"
			{
				ExprList pList=CollectionExtensions
                            .Append(null,offsettedStackList[-2].minor.yy118_ExprSpan.pExpr)
                            .Append(offsettedStackList[0].minor.yy118_ExprSpan.pExpr);

				yygotominor.yy118_ExprSpan.pExpr=pParse.sqlite3PExpr(TokenType.TK_BETWEEN,offsettedStackList[-4].minor.yy118_ExprSpan.pExpr,0,0);
				if(yygotominor.yy118_ExprSpan.pExpr!=null) {
					yygotominor.yy118_ExprSpan.pExpr.x.pList=pList;
				}
				else {
					exprc.Delete(pParse.db,ref pList);
				}
				if(offsettedStackList[-3].minor.yy4_Int!=0)
					yygotominor.yy118_ExprSpan.pExpr=pParse.sqlite3PExpr(TokenType.TK_NOT,yygotominor.yy118_ExprSpan.pExpr,0,0);
				yygotominor.yy118_ExprSpan.zStart=offsettedStackList[-4].minor.yy118_ExprSpan.zStart;
				yygotominor.yy118_ExprSpan.zEnd=offsettedStackList[0].minor.yy118_ExprSpan.zEnd;
			}
			//#line 3080 "parse.c"
			break;
			case 227:
			///
			///<summary>
			///expr ::= expr in_op LP exprlist RP 
			///</summary>
			//#line 965 "parse.y"
			{
				if(offsettedStackList[-1].minor._ExprList==null) {
					///
					///<summary>
					///Expressions of the form
					///
					///expr1 IN ()
					///expr1 NOT IN ()
					///
					///simplify to constants 0 (false) and 1 (true), respectively,
					///regardless of the value of expr1.
					///
					///</summary>
					yygotominor.yy118_ExprSpan.pExpr=pParse.sqlite3PExpr(TokenType.TK_INTEGER,0,0,sqlite3IntTokens[offsettedStackList[-3].minor.yy4_Int]);
					exprc.Delete(pParse.db,ref offsettedStackList[-4].minor.yy118_ExprSpan.pExpr);
				}
				else {
					yygotominor.yy118_ExprSpan.pExpr=pParse.sqlite3PExpr(TokenType.TK_IN,offsettedStackList[-4].minor.yy118_ExprSpan.pExpr,0,0);
					if(yygotominor.yy118_ExprSpan.pExpr!=null) {
						yygotominor.yy118_ExprSpan.pExpr.x.pList=offsettedStackList[-1].minor._ExprList;
						pParse.sqlite3ExprSetHeight(yygotominor.yy118_ExprSpan.pExpr);
					}
					else {
						exprc.Delete(pParse.db,ref offsettedStackList[-1].minor._ExprList);
					}
					if(offsettedStackList[-3].minor.yy4_Int!=0)
						yygotominor.yy118_ExprSpan.pExpr=pParse.sqlite3PExpr(TokenType.TK_NOT,yygotominor.yy118_ExprSpan.pExpr,0,0);
				}
				yygotominor.yy118_ExprSpan.zStart=offsettedStackList[-4].minor.yy118_ExprSpan.zStart;
				yygotominor.yy118_ExprSpan.zEnd=offsettedStackList[0].minor.yy0Token.zRestSql.Substring(offsettedStackList[0].minor.yy0Token.Length);
				//[yymsp[0].minor.yy0.n];
			}
			//#line 3109 "parse.c"
			break;
			case 228:
			///
			///<summary>
			///expr ::= LP select RP 
			///</summary>
			//#line 990 "parse.y"
			{
				yygotominor.yy118_ExprSpan.pExpr=pParse.sqlite3PExpr(TokenType.TK_SELECT,0,0,0);
				if(yygotominor.yy118_ExprSpan.pExpr!=null) {
					yygotominor.yy118_ExprSpan.pExpr.x.pSelect=offsettedStackList[-1].minor.yy387_Select;
					yygotominor.yy118_ExprSpan.pExpr.ExprSetProperty(ExprFlags.EP_xIsSelect);
					pParse.sqlite3ExprSetHeight(yygotominor.yy118_ExprSpan.pExpr);
				}
				else {
					SelectMethods.SelectDestructor(pParse.db,ref offsettedStackList[-1].minor.yy387_Select);
				}
				yygotominor.yy118_ExprSpan.zStart=offsettedStackList[-2].minor.yy0Token.zRestSql;
				yygotominor.yy118_ExprSpan.zEnd=offsettedStackList[0].minor.yy0Token.zRestSql.Substring(offsettedStackList[0].minor.yy0Token.Length);
				//z[yymsp[0].minor.yy0.n];
			}
			//#line 3125 "parse.c"
			break;
			case 229:
			///
			///<summary>
			///expr ::= expr in_op LP select RP 
			///</summary>
			//#line 1002 "parse.y"
			{
				yygotominor.yy118_ExprSpan.pExpr=pParse.sqlite3PExpr(TokenType.TK_IN,offsettedStackList[-4].minor.yy118_ExprSpan.pExpr,0,0);
				if(yygotominor.yy118_ExprSpan.pExpr!=null) {
					yygotominor.yy118_ExprSpan.pExpr.x.pSelect=offsettedStackList[-1].minor.yy387_Select;
					yygotominor.yy118_ExprSpan.pExpr.ExprSetProperty(ExprFlags.EP_xIsSelect);
					pParse.sqlite3ExprSetHeight(yygotominor.yy118_ExprSpan.pExpr);
				}
				else {
					SelectMethods.SelectDestructor(pParse.db,ref offsettedStackList[-1].minor.yy387_Select);
				}
				if(offsettedStackList[-3].minor.yy4_Int!=0)
					yygotominor.yy118_ExprSpan.pExpr=pParse.sqlite3PExpr(TokenType.TK_NOT,yygotominor.yy118_ExprSpan.pExpr,0,0);
				yygotominor.yy118_ExprSpan.zStart=offsettedStackList[-4].minor.yy118_ExprSpan.zStart;
				yygotominor.yy118_ExprSpan.zEnd=offsettedStackList[0].minor.yy0Token.zRestSql.Substring(offsettedStackList[0].minor.yy0Token.Length);
				//z[yymsp[0].minor.yy0.n];
			}
			//#line 3142 "parse.c"
			break;
			case 230:
			///
			///<summary>
			///expr ::= expr in_op nm dbnm 
			///</summary>
			//#line 1015 "parse.y"
			{
				SrcList pSrc=build.sqlite3SrcListAppend(pParse.db,0,offsettedStackList[-1].minor.yy0Token,offsettedStackList[0].minor.yy0Token);
				yygotominor.yy118_ExprSpan.pExpr=pParse.sqlite3PExpr(TokenType.TK_IN,offsettedStackList[-3].minor.yy118_ExprSpan.pExpr,0,0);
				if(yygotominor.yy118_ExprSpan.pExpr!=null) {
					yygotominor.yy118_ExprSpan.pExpr.x.pSelect=Select.sqlite3SelectNew(pParse,0,pSrc,0,0,0,0,0,0,0);
					yygotominor.yy118_ExprSpan.pExpr.ExprSetProperty(ExprFlags.EP_xIsSelect);
					pParse.sqlite3ExprSetHeight(yygotominor.yy118_ExprSpan.pExpr);
				}
				else {
					build.sqlite3SrcListDelete(pParse.db,ref pSrc);
				}
				if(offsettedStackList[-2].minor.yy4_Int!=0)
					yygotominor.yy118_ExprSpan.pExpr=pParse.sqlite3PExpr(TokenType.TK_NOT,yygotominor.yy118_ExprSpan.pExpr,0,0);
				yygotominor.yy118_ExprSpan.zStart=offsettedStackList[-3].minor.yy118_ExprSpan.zStart;
				yygotominor.yy118_ExprSpan.zEnd=offsettedStackList[0].minor.yy0Token.zRestSql!=null?offsettedStackList[0].minor.yy0Token.zRestSql.Substring(offsettedStackList[0].minor.yy0Token.Length):offsettedStackList[-1].minor.yy0Token.zRestSql.Substring(offsettedStackList[-1].minor.yy0Token.Length);
			}
			//#line 3160 "parse.c"
			break;
			case 231:
			///
			///<summary>
			///expr ::= EXISTS LP select RP 
			///</summary>
			//#line 1029 "parse.y"
			{
				Expr p=yygotominor.yy118_ExprSpan.pExpr=pParse.sqlite3PExpr(TokenType.TK_EXISTS,0,0,0);
				if(p!=null) {
					p.x.pSelect=offsettedStackList[-1].minor.yy387_Select;
					p.ExprSetProperty(ExprFlags.EP_xIsSelect);
					pParse.sqlite3ExprSetHeight(p);
				}
				else {
					SelectMethods.SelectDestructor(pParse.db,ref offsettedStackList[-1].minor.yy387_Select);
				}
				yygotominor.yy118_ExprSpan.zStart=offsettedStackList[-3].minor.yy0Token.zRestSql;
				yygotominor.yy118_ExprSpan.zEnd=offsettedStackList[0].minor.yy0Token.zRestSql.Substring(offsettedStackList[0].minor.yy0Token.Length);
				//z[yymsp[0].minor.yy0.n];
			}
			//#line 3176 "parse.c"
			break;
			case 232:
			///
			///<summary>
			///expr ::= CASE case_operand case_exprlist case_else END 
			///</summary>
			//#line 1044 "parse.y"
			{
				yygotominor.yy118_ExprSpan.pExpr=pParse.sqlite3PExpr(TokenType.TK_CASE,offsettedStackList[-3].minor.yy314_Expr,offsettedStackList[-1].minor.yy314_Expr,0);
				if(yygotominor.yy118_ExprSpan.pExpr!=null) {
					yygotominor.yy118_ExprSpan.pExpr.x.pList=offsettedStackList[-2].minor._ExprList;
					pParse.sqlite3ExprSetHeight(yygotominor.yy118_ExprSpan.pExpr);
				}
				else {
					exprc.Delete(pParse.db,ref offsettedStackList[-2].minor._ExprList);
				}
				yygotominor.yy118_ExprSpan.zStart=offsettedStackList[-4].minor.yy0Token.zRestSql;
				yygotominor.yy118_ExprSpan.zEnd=offsettedStackList[0].minor.yy0Token.zRestSql.Substring(offsettedStackList[0].minor.yy0Token.Length);
				//z[yymsp[0].minor.yy0.n];
			}
			//#line 3191 "parse.c"
			break;
			case 233:
			///
			///<summary>
			///case_exprlist ::= case_exprlist WHEN expr THEN expr 
			///</summary>
			//#line 1057 "parse.y"
			{
				yygotominor._ExprList= offsettedStackList[-4].minor._ExprList
                            .Append(offsettedStackList[-2].minor.yy118_ExprSpan.pExpr)
				            .Append(offsettedStackList[0].minor.yy118_ExprSpan.pExpr);
			}
			//#line 3199 "parse.c"
			break;
			case 234:
			///
			///<summary>
			///case_exprlist ::= WHEN expr THEN expr 
			///</summary>
			//#line 1061 "parse.y"
			{
				yygotominor._ExprList=CollectionExtensions
                            .Append(null,offsettedStackList[-2].minor.yy118_ExprSpan.pExpr)
				            .Append(offsettedStackList[0].minor.yy118_ExprSpan.pExpr);
			}
			//#line 3207 "parse.c"
			break;
			case 243:
			///
			///<summary>
			///cmd ::= createkw uniqueflag INDEX ifnotexists nm dbnm ON nm LP idxlist RP 
			///</summary>
			//#line 1090 "parse.y"
			{
                pParse.sqlite3CreateIndex(offsettedStackList[-6].minor.yy0Token, offsettedStackList[-5].minor.yy0Token, build.sqlite3SrcListAppend(pParse.db, 0, offsettedStackList[-3].minor.yy0Token, 0), offsettedStackList[-1].minor._ExprList, (OnConstraintError)offsettedStackList[-9].minor.yy4_Int, offsettedStackList[-10].minor.yy0Token, offsettedStackList[0].minor.yy0Token, SortOrder.SQLITE_SO_ASC, offsettedStackList[-7].minor.yy4_Int);
			}
			//#line 3216 "parse.c"
			break;
			case 244:
			///
			///<summary>
			///uniqueflag ::= UNIQUE 
			///</summary>
			case 298:
			///
			///<summary>
			///raisetype ::= ABORT 
			///</summary>
			//yysqliteinth.testcase(yyruleno==298);
			//#line 1097 "parse.y"
			{
				yygotominor.yy4_Int=(int)OnConstraintError.OE_Abort;
			}
			//#line 3222 "parse.c"
			break;
			case 245:
			///
			///<summary>
			///uniqueflag ::= 
			///</summary>
			//#line 1098 "parse.y"
			{
				yygotominor.yy4_Int=(int)OnConstraintError.OE_None;
			}
			//#line 3227 "parse.c"
			break;
			case 248:
			///
			///<summary>
			///idxlist ::= idxlist COMMA nm collate sortorder 
			///</summary>
			//#line 1107 "parse.y"
			{
				Expr p=null;
				if(offsettedStackList[-1].minor.yy0Token.Length>0) {
					p=exprc.sqlite3Expr(pParse.db,TokenType.TK_COLUMN,null);
					pParse.sqlite3ExprSetCollByToken(p,offsettedStackList[-1].minor.yy0Token);
				}
				yygotominor._ExprList= offsettedStackList[-4].minor._ExprList.Append(p);
				pParse.sqlite3ExprListSetName(yygotominor._ExprList,offsettedStackList[-2].minor.yy0Token,1);
				pParse.sqlite3ExprListCheckLength(yygotominor._ExprList,"index");
				if(yygotominor._ExprList!=null)
					yygotominor._ExprList.a[yygotominor._ExprList.Count-1].sortOrder=(SortOrder)offsettedStackList[0].minor.yy4_Int;
			}
			//#line 3242 "parse.c"
			break;
			case 249:
			///
			///<summary>
			///idxlist ::= nm collate sortorder 
			///</summary>
			//#line 1118 "parse.y"
			{
				Expr p=null;
				if(offsettedStackList[-1].minor.yy0Token.Length>0) {
					p=pParse.sqlite3PExpr(TokenType.TK_COLUMN,0,0,0);
					pParse.sqlite3ExprSetCollByToken(p,offsettedStackList[-1].minor.yy0Token);
				}
				yygotominor._ExprList=CollectionExtensions.Append(null,p);
				pParse.sqlite3ExprListSetName(yygotominor._ExprList,offsettedStackList[-2].minor.yy0Token,1);
				pParse.sqlite3ExprListCheckLength(yygotominor._ExprList,"index");
				if(yygotominor._ExprList!=null)
					yygotominor._ExprList.a[yygotominor._ExprList.Count-1].sortOrder=(SortOrder)offsettedStackList[0].minor.yy4_Int;
			}
			//#line 3257 "parse.c"
			break;
			case 250:
			///
			///<summary>
			///collate ::= 
			///</summary>
			//#line 1131 "parse.y"
			{
				yygotominor.yy0Token.zRestSql=null;
				yygotominor.yy0Token.Length=0;
			}
			//#line 3262 "parse.c"
			break;
			case 252:
			///
			///<summary>
			///cmd ::= DROP INDEX ifexists fullname 
			///</summary>
			//#line 1137 "parse.y"
			{
				build.sqlite3DropIndex(pParse,offsettedStackList[0].minor.yy259_SrcList,offsettedStackList[-1].minor.yy4_Int);
			}
			//#line 3267 "parse.c"
			break;
			case 253:
			///
			///<summary>
			///cmd ::= VACUUM 
			///</summary>
			case 254:
			///
			///<summary>
			///cmd ::= VACUUM nm 
			///</summary>
			//yysqliteinth.testcase(yyruleno==254);
			//#line 1143 "parse.y"
			{
				Sqlite3.sqlite3Vacuum(pParse);
			}
			//#line 3273 "parse.c"
			break;
			case 255:
			///
			///<summary>
			///cmd ::= PRAGMA nm dbnm 
			///</summary>
			//#line 1151 "parse.y"
			{
                        Sqlite3.sqlite3Pragma(pParse,offsettedStackList[-1].minor.yy0Token,offsettedStackList[0].minor.yy0Token,0,0);
			}
			//#line 3278 "parse.c"
			break;
			case 256:
			///
			///<summary>
			///cmd ::= PRAGMA nm dbnm EQ nmnum 
			///</summary>
			//#line 1152 "parse.y"
			{
                        Sqlite3.sqlite3Pragma(pParse,offsettedStackList[-3].minor.yy0Token,offsettedStackList[-2].minor.yy0Token,offsettedStackList[0].minor.yy0Token,0);
			}
			//#line 3283 "parse.c"
			break;
			case 257:
			///
			///<summary>
			///cmd ::= PRAGMA nm dbnm LP nmnum RP 
			///</summary>
			//#line 1153 "parse.y"
			{
                        Sqlite3.sqlite3Pragma(pParse,offsettedStackList[-4].minor.yy0Token,offsettedStackList[-3].minor.yy0Token,offsettedStackList[-1].minor.yy0Token,0);
			}
			//#line 3288 "parse.c"
			break;
			case 258:
			///
			///<summary>
			///cmd ::= PRAGMA nm dbnm EQ minus_num 
			///</summary>
			//#line 1155 "parse.y"
			{
                        Sqlite3.sqlite3Pragma(pParse,offsettedStackList[-3].minor.yy0Token,offsettedStackList[-2].minor.yy0Token,offsettedStackList[0].minor.yy0Token,1);
			}
			//#line 3293 "parse.c"
			break;
			case 259:
			///
			///<summary>
			///cmd ::= PRAGMA nm dbnm LP minus_num RP 
			///</summary>
			//#line 1157 "parse.y"
			{
                        Sqlite3.sqlite3Pragma(pParse,offsettedStackList[-4].minor.yy0Token,offsettedStackList[-3].minor.yy0Token,offsettedStackList[-1].minor.yy0Token,1);
			}
			//#line 3298 "parse.c"
			break;
			case 270:
                    ///
                    ///<summary>
                    ///cmd ::= createkw trigger_decl BEGIN trigger_cmd_list END 
                    ///</summary>
                    //#line 1175 "parse.y"
                    {
                        Token all = new Token();
                        //all.z = yymsp[-3].minor.yy0.z;
                        //all.n = (int)(yymsp[0].minor.yy0.z - yymsp[-3].minor.yy0.z) + yymsp[0].minor.yy0.n;
                        all.Length = (int)(offsettedStackList[-3].minor.yy0Token.zRestSql.Length - offsettedStackList[0].minor.yy0Token.zRestSql.Length) + offsettedStackList[0].minor.yy0Token.Length;
                        all.zRestSql = offsettedStackList[-3].minor.yy0Token.zRestSql.Substring(0, all.Length);
                        TriggerParser.sqlite3FinishTrigger(pParse, offsettedStackList[-1].minor.yy203_TriggerStep, all);
                    }
			//#line 3308 "parse.c"
			break;
			case 271:
			///
			///<summary>
			///trigger_decl ::= temp TRIGGER ifnotexists nm dbnm trigger_time trigger_event ON fullname foreach_clause when_clause 
			///</summary>
			//#line 1184 "parse.y"
			{
				TriggerParser.sqlite3BeginTrigger(pParse,offsettedStackList[-7].minor.yy0Token,offsettedStackList[-6].minor.yy0Token,(TokenType)offsettedStackList[-5].minor.yy4_Int,(TokenType)offsettedStackList[-4].minor.yy90_TrigEvent.a,offsettedStackList[-4].minor.yy90_TrigEvent.b,offsettedStackList[-2].minor.yy259_SrcList,offsettedStackList[0].minor.yy314_Expr,offsettedStackList[-10].minor.yy4_Int,offsettedStackList[-8].minor.yy4_Int);
				yygotominor.yy0Token=(offsettedStackList[-6].minor.yy0Token.Length==0?offsettedStackList[-7].minor.yy0Token:offsettedStackList[-6].minor.yy0Token);
			}
			//#line 3316 "parse.c"
			break;
			case 272:
			///
			///<summary>
			///trigger_time ::= BEFORE 
			///</summary>
			case 275:
			///
			///<summary>
			///trigger_time ::= 
			///</summary>
			//yysqliteinth.testcase(yyruleno==275);
			//#line 1190 "parse.y"
			{
				yygotominor.yy4_Int=(int)TokenType.TK_BEFORE;
			}
			//#line 3322 "parse.c"
			break;
			case 273:
			///
			///<summary>
			///trigger_time ::= AFTER 
			///</summary>
			//#line 1191 "parse.y"
			{
				yygotominor.yy4_Int=(int)TokenType.TK_AFTER;
			}
			//#line 3327 "parse.c"
			break;
			case 274:
			///
			///<summary>
			///trigger_time ::= INSTEAD OF 
			///</summary>
			//#line 1192 "parse.y"
			{
				yygotominor.yy4_Int=(int)TokenType.TK_INSTEAD;
			}
			//#line 3332 "parse.c"
			break;
			case 276:
			///
			///<summary>
			///trigger_event ::= DELETE|INSERT 
			///</summary>
			case 277:
			///
			///<summary>
			///trigger_event ::= UPDATE 
			///</summary>
			//yysqliteinth.testcase(yyruleno==277);
			//#line 1197 "parse.y"
			{
				yygotominor.yy90_TrigEvent.a=offsettedStackList[0].major;
				yygotominor.yy90_TrigEvent.b=null;
			}
			//#line 3338 "parse.c"
			break;
			case 278:
			///
			///<summary>
			///trigger_event ::= UPDATE OF inscollist 
			///</summary>
			//#line 1199 "parse.y"
			{
				yygotominor.yy90_TrigEvent.a=TokenType.TK_UPDATE;
				yygotominor.yy90_TrigEvent.b=offsettedStackList[0].minor.yy384_IdList;
			}
			//#line 3343 "parse.c"
			break;
			case 281:
			///
			///<summary>
			///when_clause ::= 
			///</summary>
			case 303:
			///
			///<summary>
			///key_opt ::= 
			///</summary>
			//yysqliteinth.testcase(yyruleno==303);
			//#line 1206 "parse.y"
			{
				yygotominor.yy314_Expr=null;
			}
			//#line 3349 "parse.c"
			break;
			case 282:
			///
			///<summary>
			///when_clause ::= WHEN expr 
			///</summary>
			case 304:
			///
			///<summary>
			///key_opt ::= KEY expr 
			///</summary>
			//yysqliteinth.testcase(yyruleno==304);
			//#line 1207 "parse.y"
			{
				yygotominor.yy314_Expr=offsettedStackList[0].minor.yy118_ExprSpan.pExpr;
			}
			//#line 3355 "parse.c"
			break;
			case 283:
			///
			///<summary>
			///trigger_cmd_list ::= trigger_cmd_list trigger_cmd SEMI 
			///</summary>
			//#line 1211 "parse.y"
			{
				Debug.Assert(offsettedStackList[-2].minor.yy203_TriggerStep!=null);
				offsettedStackList[-2].minor.yy203_TriggerStep.pLast.pNext=offsettedStackList[-1].minor.yy203_TriggerStep;
				offsettedStackList[-2].minor.yy203_TriggerStep.pLast=offsettedStackList[-1].minor.yy203_TriggerStep;
				yygotominor.yy203_TriggerStep=offsettedStackList[-2].minor.yy203_TriggerStep;
			}
			//#line 3365 "parse.c"
			break;
			case 284:
			///
			///<summary>
			///trigger_cmd_list ::= trigger_cmd SEMI 
			///</summary>
			//#line 1217 "parse.y"
			{
				Debug.Assert(offsettedStackList[-1].minor.yy203_TriggerStep!=null);
				offsettedStackList[-1].minor.yy203_TriggerStep.pLast=offsettedStackList[-1].minor.yy203_TriggerStep;
				yygotominor.yy203_TriggerStep=offsettedStackList[-1].minor.yy203_TriggerStep;
			}
			//#line 3374 "parse.c"
			break;
			case 286:
			///
			///<summary>
			///trnm ::= nm DOT nm 
			///</summary>
			//#line 1229 "parse.y"
			{
				yygotominor.yy0Token=offsettedStackList[0].minor.yy0Token;
				utilc.sqlite3ErrorMsg(pParse,"qualified table names are not allowed on INSERT, UPDATE, and DELETE "+"statements within triggers");
			}
			//#line 3384 "parse.c"
			break;
			case 288:
			///
			///<summary>
			///tridxby ::= INDEXED BY nm 
			///</summary>
			//#line 1241 "parse.y"
			{
				utilc.sqlite3ErrorMsg(pParse,"the INDEXED BY clause is not allowed on UPDATE or DELETE statements "+"within triggers");
			}
			//#line 3393 "parse.c"
			break;
			case 289:
			///
			///<summary>
			///tridxby ::= NOT INDEXED 
			///</summary>
			//#line 1246 "parse.y"
			{
				utilc.sqlite3ErrorMsg(pParse,"the NOT INDEXED clause is not allowed on UPDATE or DELETE statements "+"within triggers");
			}
			//#line 3402 "parse.c"
			break;
			case 290:
			///
			///<summary>
			///trigger_cmd ::= UPDATE orconf trnm tridxby SET setlist where_opt 
			///</summary>
			//#line 1259 "parse.y"
			{
				yygotominor.yy203_TriggerStep= TriggerParser.sqlite3TriggerUpdateStep(pParse.db,offsettedStackList[-4].minor.yy0Token,offsettedStackList[-1].minor._ExprList,offsettedStackList[0].minor.yy314_Expr,(OnConstraintError)offsettedStackList[-5].minor.yy210);
			}
			//#line 3407 "parse.c"
			break;
			case 291:
			///
			///<summary>
			///trigger_cmd ::= insert_cmd INTO trnm inscollist_opt VALUES LP itemlist RP 
			///</summary>
			//#line 1264 "parse.y"
			{
				yygotominor.yy203_TriggerStep=TriggerParser.sqlite3TriggerInsertStep( pParse.db,offsettedStackList[-5].minor.yy0Token,offsettedStackList[-4].minor.yy384_IdList,offsettedStackList[-1].minor._ExprList,0,(OnConstraintError)offsettedStackList[-7].minor.yy210);
			}
			//#line 3412 "parse.c"
			break;
			case 292:
			///
			///<summary>
			///trigger_cmd ::= insert_cmd INTO trnm inscollist_opt select 
			///</summary>
			//#line 1267 "parse.y"
			{
				yygotominor.yy203_TriggerStep= TriggerParser.sqlite3TriggerInsertStep(pParse.db,offsettedStackList[-2].minor.yy0Token,offsettedStackList[-1].minor.yy384_IdList,0,offsettedStackList[0].minor.yy387_Select,(OnConstraintError)offsettedStackList[-4].minor.yy210);
			}
			//#line 3417 "parse.c"
			break;
			case 293:
			///
			///<summary>
			///trigger_cmd ::= DELETE FROM trnm tridxby where_opt 
			///</summary>
			//#line 1271 "parse.y"
			{
				yygotominor.yy203_TriggerStep= TriggerParser.sqlite3TriggerDeleteStep(pParse.db,offsettedStackList[-2].minor.yy0Token,offsettedStackList[0].minor.yy314_Expr);
			}
			//#line 3422 "parse.c"
			break;
			case 294:
			///
			///<summary>
			///trigger_cmd ::= select 
			///</summary>
			//#line 1274 "parse.y"
			{
				yygotominor.yy203_TriggerStep= TriggerParser.sqlite3TriggerSelectStep(pParse.db,offsettedStackList[0].minor.yy387_Select);
			}
			//#line 3427 "parse.c"
			break;
			case 295:
			///
			///<summary>
			///expr ::= RAISE LP IGNORE RP 
			///</summary>
			//#line 1277 "parse.y"
			{
				yygotominor.yy118_ExprSpan.pExpr=pParse.sqlite3PExpr(TokenType.TK_RAISE,0,0,0);
				if(yygotominor.yy118_ExprSpan.pExpr!=null) {
					yygotominor.yy118_ExprSpan.pExpr.affinity=(char)OnConstraintError.OE_Ignore;
				}
				yygotominor.yy118_ExprSpan.zStart=offsettedStackList[-3].minor.yy0Token.zRestSql;
				yygotominor.yy118_ExprSpan.zEnd=offsettedStackList[0].minor.yy0Token.zRestSql.Substring(offsettedStackList[0].minor.yy0Token.Length);
				//z[yymsp[0].minor.yy0.n];
			}
			//#line 3439 "parse.c"
			break;
			case 296:
			///
			///<summary>
			///expr ::= RAISE LP raisetype COMMA nm RP 
			///</summary>
			//#line 1285 "parse.y"
			{
				yygotominor.yy118_ExprSpan.pExpr=pParse.sqlite3PExpr(TokenType.TK_RAISE,0,0,offsettedStackList[-1].minor.yy0Token);
				if(yygotominor.yy118_ExprSpan.pExpr!=null) {
					yygotominor.yy118_ExprSpan.pExpr.affinity=(char)offsettedStackList[-3].minor.yy4_Int;
				}
				yygotominor.yy118_ExprSpan.zStart=offsettedStackList[-5].minor.yy0Token.zRestSql;
				yygotominor.yy118_ExprSpan.zEnd=offsettedStackList[0].minor.yy0Token.zRestSql.Substring(offsettedStackList[0].minor.yy0Token.Length);
				//z[yymsp[0].minor.yy0.n];
			}
			//#line 3451 "parse.c"
			break;
			case 297:
			///
			///<summary>
			///raisetype ::= ROLLBACK 
			///</summary>
			//#line 1296 "parse.y"
			{
				yygotominor.yy4_Int=(int)OnConstraintError.OE_Rollback;
			}
			//#line 3456 "parse.c"
			break;
			case 299:
			///
			///<summary>
			///raisetype ::= FAIL 
			///</summary>
			//#line 1298 "parse.y"
			{
                yygotominor.yy4_Int = (int)OnConstraintError.OE_Fail;
			}
			//#line 3461 "parse.c"
			break;
			case 300:
			///
			///<summary>
			///cmd ::= DROP TRIGGER ifexists fullname 
			///</summary>
			//#line 1303 "parse.y"
			{
                        TriggerParser.sqlite3DropTrigger(pParse,offsettedStackList[0].minor.yy259_SrcList,offsettedStackList[-1].minor.yy4_Int);
			}
			//#line 3468 "parse.c"
			break;
			case 301:
			///
			///<summary>
			///cmd ::= ATTACH database_kw_opt expr AS expr key_opt 
			///</summary>
			//#line 1310 "parse.y"
			{
				pParse.sqlite3Attach(offsettedStackList[-3].minor.yy118_ExprSpan.pExpr,offsettedStackList[-1].minor.yy118_ExprSpan.pExpr,offsettedStackList[0].minor.yy314_Expr);
			}
			//#line 3475 "parse.c"
			break;
			case 302:
			///
			///<summary>
			///cmd ::= DETACH database_kw_opt expr 
			///</summary>
			//#line 1313 "parse.y"
			{
				pParse.sqlite3Detach(offsettedStackList[0].minor.yy118_ExprSpan.pExpr);
			}
			//#line 3482 "parse.c"
			break;
			case 307:
			///
			///<summary>
			///cmd ::= REINDEX 
			///</summary>
			//#line 1328 "parse.y"
			{
                pParse.sqlite3Reindex(0, 0);
			}
			//#line 3487 "parse.c"
			break;
			case 308:
			///
			///<summary>
			///cmd ::= REINDEX nm dbnm 
			///</summary>
			//#line 1329 "parse.y"
			{
                pParse.codegenReindex(offsettedStackList[-1].minor.yy0Token, offsettedStackList[0].minor.yy0Token);
			}
			//#line 3492 "parse.c"
			break;
			case 309:
			///
			///<summary>
			///cmd ::= ANALYZE 
			///</summary>
			//#line 1334 "parse.y"
			{
				pParse.sqlite3Analyze(0,0);
			}
			//#line 3497 "parse.c"
			break;
			case 310:
			///
			///<summary>
			///cmd ::= ANALYZE nm dbnm 
			///</summary>
			//#line 1335 "parse.y"
			{
				pParse.sqlite3Analyze(offsettedStackList[-1].minor.yy0Token,offsettedStackList[0].minor.yy0Token);
			}
			//#line 3502 "parse.c"
			break;
			case 311:
			///
			///<summary>
			///cmd ::= ALTER TABLE fullname RENAME TO nm 
			///</summary>
			//#line 1340 "parse.y"
			{
				pParse.sqlite3AlterRenameTable(offsettedStackList[-3].minor.yy259_SrcList,offsettedStackList[0].minor.yy0Token);
			}
			//#line 3509 "parse.c"
			break;
			case 312:
			///
			///<summary>
			///cmd ::= ALTER TABLE add_column_fullname ADD kwcolumn_opt column 
			///</summary>
			//#line 1343 "parse.y"
			{
				pParse.sqlite3AlterFinishAddColumn(offsettedStackList[0].minor.yy0Token);
			}
			//#line 3516 "parse.c"
			break;
			case 313:
			///
			///<summary>
			///add_column_fullname ::= fullname 
			///</summary>
			//#line 1346 "parse.y"
			{
				pParse.db.lookaside.bEnabled=0;
				pParse.sqlite3AlterBeginAddColumn(offsettedStackList[0].minor.yy259_SrcList);
			}
			//#line 3524 "parse.c"
			break;
			case 316:
			///
			///<summary>
			///cmd ::= create_vtab 
			///</summary>
			//#line 1356 "parse.y"
			{
				pParse.sqlite3VtabFinishParse((Token)null);
			}
			//#line 3529 "parse.c"
			break;
			case 317:
			///
			///<summary>
			///cmd ::= create_vtab LP vtabarglist RP 
			///</summary>
			//#line 1357 "parse.y"
			{
				pParse.sqlite3VtabFinishParse(offsettedStackList[0].minor.yy0Token);
			}
			//#line 3534 "parse.c"
			break;
			case 318:
			///
			///<summary>
			///create_vtab ::= createkw VIRTUAL TABLE nm dbnm USING nm 
			///</summary>
			//#line 1358 "parse.y"
			{
				pParse.sqlite3VtabBeginParse(offsettedStackList[-3].minor.yy0Token,offsettedStackList[-2].minor.yy0Token,offsettedStackList[0].minor.yy0Token);
			}
			//#line 3541 "parse.c"
			break;
			case 321:
			///
			///<summary>
			///vtabarg ::= 
			///</summary>
			//#line 1363 "parse.y"
			{
				pParse.sqlite3VtabArgInit();
			}
			//#line 3546 "parse.c"
			break;
			case 323:
			///
			///<summary>
			///vtabargtoken ::= ANY 
			///</summary>
			case 324:
			///
			///<summary>
			///vtabargtoken ::= lp anylist RP 
			///</summary>
			//yysqliteinth.testcase(yyruleno==324);
			case 325:
			///
			///<summary>
			///lp ::= LP 
			///</summary>
			//yysqliteinth.testcase(yyruleno==325);
			//#line 1365 "parse.y"
			{
				pParse.sqlite3VtabArgExtend(offsettedStackList[0].minor.yy0Token);
			}
			//#line 3553 "parse.c"
			break;
			default:
			///
			///<summary>
			///(0) input ::= cmdlist 
			///</summary>
			//yysqliteinth.testcase(yyruleno==0);
			///
			///<summary>
			///(1) cmdlist ::= cmdlist ecmd 
			///</summary>
			//yysqliteinth.testcase(yyruleno==1);
			///
			///<summary>
			///(2) cmdlist ::= ecmd 
			///</summary>
			//yysqliteinth.testcase(yyruleno==2);
			///
			///<summary>
			///(3) ecmd ::= SEMI 
			///</summary>
			//yysqliteinth.testcase(yyruleno==3);
			///
			///<summary>
			///(4) ecmd ::= explain cmdx SEMI 
			///</summary>
			//yysqliteinth.testcase(yyruleno==4);
			///
			///<summary>
			///(10) trans_opt ::= 
			///</summary>
			//yysqliteinth.testcase(yyruleno==10);
			///
			///<summary>
			///(11) trans_opt ::= TRANSACTION 
			///</summary>
			//yysqliteinth.testcase(yyruleno==11);
			///
			///<summary>
			///(12) trans_opt ::= TRANSACTION nm 
			///</summary>
			//yysqliteinth.testcase(yyruleno==12);
			///
			///<summary>
			///(20) savepoint_opt ::= SAVEPOINT 
			///</summary>
			//yysqliteinth.testcase(yyruleno==20);
			///
			///<summary>
			///(21) savepoint_opt ::= 
			///</summary>
			//yysqliteinth.testcase(yyruleno==21);
			///
			///<summary>
			///(25) cmd ::= create_table create_table_args 
			///</summary>
			//yysqliteinth.testcase(yyruleno==25);
			///
			///<summary>
			///(34) columnlist ::= columnlist COMMA column 
			///</summary>
			//yysqliteinth.testcase(yyruleno==34);
			///
			///<summary>
			///(35) columnlist ::= column 
			///</summary>
			//yysqliteinth.testcase(yyruleno==35);
			///
			///<summary>
			///(44) type ::= 
			///</summary>
			//yysqliteinth.testcase(yyruleno==44);
			///
			///<summary>
			///(51) signed ::= plus_num 
			///</summary>
			//yysqliteinth.testcase(yyruleno==51);
			///
			///<summary>
			///(52) signed ::= minus_num 
			///</summary>
			//yysqliteinth.testcase(yyruleno==52);
			///
			///<summary>
			///(53) carglist ::= carglist carg 
			///</summary>
			//yysqliteinth.testcase(yyruleno==53);
			///
			///<summary>
			///(54) carglist ::= 
			///</summary>
			//yysqliteinth.testcase(yyruleno==54);
			///
			///<summary>
			///(55) carg ::= CONSTRAINT nm ccons 
			///</summary>
			//yysqliteinth.testcase(yyruleno==55);
			///
			///<summary>
			///(56) carg ::= ccons 
			///</summary>
			//yysqliteinth.testcase(yyruleno==56);
			///
			///<summary>
			///(62) ccons ::= NULL onconf 
			///</summary>
			//yysqliteinth.testcase(yyruleno==62);
			///
			///<summary>
			///(90) conslist ::= conslist COMMA tcons 
			///</summary>
			//yysqliteinth.testcase(yyruleno==90);
			///
			///<summary>
			///(91) conslist ::= conslist tcons 
			///</summary>
			//yysqliteinth.testcase(yyruleno==91);
			///
			///<summary>
			///(92) conslist ::= tcons 
			///</summary>
			//yysqliteinth.testcase(yyruleno==92);
			///
			///<summary>
			///(93) tcons ::= CONSTRAINT nm 
			///</summary>
			//yysqliteinth.testcase(yyruleno==93);
			///
			///<summary>
			///(268) plus_opt ::= PLUS 
			///</summary>
			//yysqliteinth.testcase(yyruleno==268);
			///
			///<summary>
			///(269) plus_opt ::= 
			///</summary>
			//yysqliteinth.testcase(yyruleno==269);
			///
			///<summary>
			///(279) foreach_clause ::= 
			///</summary>
			//yysqliteinth.testcase(yyruleno==279);
			///
			///<summary>
			///(280) foreach_clause ::= FOR EACH ROW 
			///</summary>
			//yysqliteinth.testcase(yyruleno==280);
			///
			///<summary>
			///(287) tridxby ::= 
			///</summary>
			//yysqliteinth.testcase(yyruleno==287);
			///
			///<summary>
			///(305) database_kw_opt ::= DATABASE 
			///</summary>
			//yysqliteinth.testcase(yyruleno==305);
			///
			///<summary>
			///(306) database_kw_opt ::= 
			///</summary>
			//yysqliteinth.testcase(yyruleno==306);
			///
			///<summary>
			///(314) kwcolumn_opt ::= 
			///</summary>
			//yysqliteinth.testcase(yyruleno==314);
			///
			///<summary>
			///(315) kwcolumn_opt ::= COLUMNKW 
			///</summary>
			//yysqliteinth.testcase(yyruleno==315);
			///
			///<summary>
			///(319) vtabarglist ::= vtabarg 
			///</summary>
			//yysqliteinth.testcase(yyruleno==319);
			///
			///<summary>
			///(320) vtabarglist ::= vtabarglist COMMA vtabarg 
			///</summary>
			//yysqliteinth.testcase(yyruleno==320);
			///
			///<summary>
			///(322) vtabarg ::= vtabarg vtabargtoken 
			///</summary>
			//yysqliteinth.testcase(yyruleno==322);
			///
			///<summary>
			///(326) anylist ::= 
			///</summary>
			//yysqliteinth.testcase(yyruleno==326);
			///
			///<summary>
			///(327) anylist ::= anylist LP anylist RP 
			///</summary>
			//yysqliteinth.testcase(yyruleno==327);
			///
			///<summary>
			///(328) anylist ::= anylist ANY 
			///</summary>
			//yysqliteinth.testcase(yyruleno==328);
			break;
			}
            #endregion
            
            
            
            var yygoto = yyRuleInfo[yyruleno].lhs;            ///The next state 
            var yysize = yyRuleInfo[yyruleno].nrhs;///Amount to pop the stack 
            var yyact = yy_find_reduce_action(offsettedStackList[-yysize].stateno, (YYCODETYPE)yygoto);  ///The next action

            Console.WriteLine();
            Print(yypParser.yyidx.ToString(), ConsoleColor.Black, ConsoleColor.DarkGray);
            Print(yyRuleName[yyruleno],ConsoleColor.Blue);
            Print("->>-", ConsoleColor.DarkYellow);
            Print(yyRuleName[(int)yygoto], ConsoleColor.DarkYellow);
            Print("  >>>   ", ConsoleColor.Gray);
            Print( yygoto+":"+ yysize+">"+ yyact,ConsoleColor.DarkGreen);
            yypParser.yyidx-=yysize;
			

            

            if (yyact<YYNSTATE) {
				#if NDEBUG
				///
				///<summary>
				///If we are not debugging and the reduce action popped at least
				///one element off the stack, then we can push the new element back
				///onto the stack here, and skip the stack overflow test in yy_shift().
				///That gives a significant speed improvement. 
				///</summary>
				if(yysize!=0) {
					yypParser.yyidx++;
					offsettedStackList._yyidx-=yysize-1;
					offsettedStackList[0].stateno=yyact;
					offsettedStackList[0].major=yygoto;
					offsettedStackList[0].minor=yygotominor;
				}
				else
				#endif
				 {
					yypParser.yy_shift(yyact,yygoto,yygotominor);
				}
			}
			else {
				Debug.Assert(yyact==YYNSTATE+YYNRULE+1);
				yypParser.yy_accept();
			}
		}

        private static void Print(string v, ConsoleColor foreground=ConsoleColor.White, ConsoleColor background=ConsoleColor.Black)
        {
            Console.ForegroundColor = foreground;
            Console.BackgroundColor = background;
            Console.Write(v);
        }

        ///<summary>
        /// The following code executes when the parse fails
        ///
        ///</summary>
#if !YYNOERRORRECOVERY
#endif

	}
}
