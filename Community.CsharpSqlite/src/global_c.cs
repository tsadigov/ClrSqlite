using System;
using System.Diagnostics;
using System.Text;

namespace Community.CsharpSqlite
{
    using Community.CsharpSqlite.Ast;
    using Community.CsharpSqlite.Metadata;
    using sqlite3_value = Engine.Mem;

	public partial class Sqlite3
	{
		///
///<summary>
///2008 June 13
///
///The author disclaims copyright to this source code.  In place of
///a legal notice, here is a blessing:
///
///May you do good and not evil.
///May you find forgiveness for yourself and forgive others.
///May you share freely, never taking more than you give.
///
///
///
///This file contains definitions of global variables and contants.
///
///</summary>
///<param name="Included in SQLite3 port to C#">SQLite;  2008 Noah B Hart</param>
///<param name="C#">SQLite is an independent reimplementation of the SQLite software library</param>
///<param name=""></param>
///<param name="SQLITE_SOURCE_ID: 2011">23 19:49:22 4374b7e83ea0a3fbc3691f9c0c936272862f32f2</param>
///<param name=""></param>
///<param name=""></param>
///<param name=""></param>

		//#include "sqliteInt.h"
		///
///<summary>
///</summary>
///<param name="An array to map all upper">case characters into their corresponding</param>
///<param name="lower">case character.</param>
///<param name=""></param>
///<param name="SQLite only considers US">ASCII (or EBCDIC) characters.  We do not</param>
///<param name="handle case conversions for the UTF character set since the tables">handle case conversions for the UTF character set since the tables</param>
///<param name="involved are nearly as big or bigger than SQLite itself.">involved are nearly as big or bigger than SQLite itself.</param>
///<param name=""></param>

		///
///<summary>
///</summary>
///<param name="An array to map all upper">case characters into their corresponding</param>
///<param name="lower">case character.</param>
///<param name=""></param>

		//
		// Replaced in C# with _Custom.sqlite3UpperToLower class
		//    static int[] _Custom.sqlite3UpperToLower = new int[]  {
		//#if SQLITE_ASCII
		//0,  1,  2,  3,  4,  5,  6,  7,  8,  9, 10, 11, 12, 13, 14, 15, 16, 17,
		//18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35,
		//36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47, 48, 49, 50, 51, 52, 53,
		//54, 55, 56, 57, 58, 59, 60, 61, 62, 63, 64, 97, 98, 99,100,101,102,103,
		//104,105,106,107,108,109,110,111,112,113,114,115,116,117,118,119,120,121,
		//122, 91, 92, 93, 94, 95, 96, 97, 98, 99,100,101,102,103,104,105,106,107,
		//108,109,110,111,112,113,114,115,116,117,118,119,120,121,122,123,124,125,
		//126,127,128,129,130,131,132,133,134,135,136,137,138,139,140,141,142,143,
		//144,145,146,147,148,149,150,151,152,153,154,155,156,157,158,159,160,161,
		//162,163,164,165,166,167,168,169,170,171,172,173,174,175,176,177,178,179,
		//180,181,182,183,184,185,186,187,188,189,190,191,192,193,194,195,196,197,
		//198,199,200,201,202,203,204,205,206,207,208,209,210,211,212,213,214,215,
		//216,217,218,219,220,221,222,223,224,225,226,227,228,229,230,231,232,233,
		//234,235,236,237,238,239,240,241,242,243,244,245,246,247,248,249,250,251,
		//252,253,254,255
		//#endif
		//#if SQLITE_EBCDIC
		//0,  1,  2,  3,  4,  5,  6,  7,  8,  9, 10, 11, 12, 13, 14, 15, /* 0x */
		//16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, /* 1x */
		//32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47, /* 2x */
		//48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 58, 59, 60, 61, 62, 63, /* 3x */
		//64, 65, 66, 67, 68, 69, 70, 71, 72, 73, 74, 75, 76, 77, 78, 79, /* 4x */
		//80, 81, 82, 83, 84, 85, 86, 87, 88, 89, 90, 91, 92, 93, 94, 95, /* 5x */
		//96, 97, 66, 67, 68, 69, 70, 71, 72, 73,106,107,108,109,110,111, /* 6x */
		//112, 81, 82, 83, 84, 85, 86, 87, 88, 89,122,123,124,125,126,127, /* 7x */
		//128,129,130,131,132,133,134,135,136,137,138,139,140,141,142,143, /* 8x */
		//144,145,146,147,148,149,150,151,152,153,154,155,156,157,156,159, /* 9x */
		//160,161,162,163,164,165,166,167,168,169,170,171,140,141,142,175, /* Ax */
		//176,177,178,179,180,181,182,183,184,185,186,187,188,189,190,191, /* Bx */
		//192,129,130,131,132,133,134,135,136,137,202,203,204,205,206,207, /* Cx */
		//208,145,146,147,148,149,150,151,152,153,218,219,220,221,222,223, /* Dx */
		//224,225,162,163,164,165,166,167,168,169,232,203,204,205,206,207, /* Ex */
		//239,240,241,242,243,244,245,246,247,248,249,219,220,221,222,255, /* Fx */
		//#endif
		//};
		///
///<summary>
///</summary>
///<param name="The following 256 byte lookup table is used to support SQLites built">in</param>
///<param name="equivalents to the following standard library functions:">equivalents to the following standard library functions:</param>
///<param name=""></param>
///<param name="isspace()                        0x01">isspace()                        0x01</param>
///<param name="isalpha()                        0x02">isalpha()                        0x02</param>
///<param name="isdigit()                        0x04">isdigit()                        0x04</param>
///<param name="isalnum()                        0x06">isalnum()                        0x06</param>
///<param name="isxdigit()                       0x08">isxdigit()                       0x08</param>
///<param name="toupper()                        0x20">toupper()                        0x20</param>
///<param name="SQLite identifier character      0x40">SQLite identifier character      0x40</param>
///<param name=""></param>
///<param name="Bit 0x20 is set if the mapped character requires translation to upper">Bit 0x20 is set if the mapped character requires translation to upper</param>
///<param name="case. i.e. if the character is a lower">case ASCII character.</param>
///<param name="If x is a lower">case equivalent</param>
///<param name="is (x "> 0x20). Therefore toupper() can be implemented as:</param>
///<param name=""></param>
///<param name="(x & ~(map[x]&0x20))">(x & ~(map[x]&0x20))</param>
///<param name=""></param>
///<param name="Standard function tolower() is implemented using the _Custom.sqlite3UpperToLower[]">Standard function tolower() is implemented using the _Custom.sqlite3UpperToLower[]</param>
///<param name="array. tolower() is used more often than toupper() by SQLite.">array. tolower() is used more often than toupper() by SQLite.</param>
///<param name=""></param>
///<param name="Bit 0x40 is set if the character non">alphanumeric and can be used in an </param>
///<param name="SQLite identifier.  Identifiers are alphanumerics, "_", "$", and any">SQLite identifier.  Identifiers are alphanumerics, "_", "$", and any</param>
///<param name="non">ASCII UTF character. Hence the test for whether or not a character is</param>
///<param name="part of an identifier is 0x46.">part of an identifier is 0x46.</param>
///<param name=""></param>
///<param name="SQLite's versions are identical to the standard versions assuming a">SQLite's versions are identical to the standard versions assuming a</param>
///<param name="locale of "C". They are implemented as macros in sqliteInt.h.">locale of "C". They are implemented as macros in sqliteInt.h.</param>
///<param name=""></param>

		#if SQLITE_ASCII
		public static byte[] sqlite3CtypeMap = new byte[] {
			0x00,
			0x00,
			0x00,
			0x00,
			0x00,
			0x00,
			0x00,
			0x00,
			///
///<summary>
///00..07    ........ 
///</summary>

			0x00,
			0x01,
			0x01,
			0x01,
			0x01,
			0x01,
			0x00,
			0x00,
			///
///<summary>
///08..0f    ........ 
///</summary>

			0x00,
			0x00,
			0x00,
			0x00,
			0x00,
			0x00,
			0x00,
			0x00,
			///
///<summary>
///10..17    ........ 
///</summary>

			0x00,
			0x00,
			0x00,
			0x00,
			0x00,
			0x00,
			0x00,
			0x00,
			///
///<summary>
///18..1f    ........ 
///</summary>

			0x01,
			0x00,
			0x00,
			0x00,
			0x40,
			0x00,
			0x00,
			0x00,
			///
///<summary>
///20..27     !"#$%&' 
///</summary>

			0x00,
			0x00,
			0x00,
			0x00,
			0x00,
			0x00,
			0x00,
			0x00,
			///
///<summary>
///</summary>
///<param name="28..2f    ()*+,">./ </param>

			0x0c,
			0x0c,
			0x0c,
			0x0c,
			0x0c,
			0x0c,
			0x0c,
			0x0c,
			///
///<summary>
///30..37    01234567 
///</summary>

			0x0c,
			0x0c,
			0x00,
			0x00,
			0x00,
			0x00,
			0x00,
			0x00,
			///
///<summary>
///38..3f    89:;<=>? 
///</summary>

			0x00,
			0x0a,
			0x0a,
			0x0a,
			0x0a,
			0x0a,
			0x0a,
			0x02,
			///
///<summary>
///40..47    @ABCDEFG 
///</summary>

			0x02,
			0x02,
			0x02,
			0x02,
			0x02,
			0x02,
			0x02,
			0x02,
			///
///<summary>
///48..4f    HIJKLMNO 
///</summary>

			0x02,
			0x02,
			0x02,
			0x02,
			0x02,
			0x02,
			0x02,
			0x02,
			///
///<summary>
///50..57    PQRSTUVW 
///</summary>

			0x02,
			0x02,
			0x02,
			0x00,
			0x00,
			0x00,
			0x00,
			0x40,
			///
///<summary>
///58..5f    XYZ[\]^_ 
///</summary>

			0x00,
			0x2a,
			0x2a,
			0x2a,
			0x2a,
			0x2a,
			0x2a,
			0x22,
			///
///<summary>
///60..67    `abcdefg 
///</summary>

			0x22,
			0x22,
			0x22,
			0x22,
			0x22,
			0x22,
			0x22,
			0x22,
			///
///<summary>
///68..6f    hijklmno 
///</summary>

			0x22,
			0x22,
			0x22,
			0x22,
			0x22,
			0x22,
			0x22,
			0x22,
			///
///<summary>
///70..77    pqrstuvw 
///</summary>

			0x22,
			0x22,
			0x22,
			0x00,
			0x00,
			0x00,
			0x00,
			0x00,
			///
///<summary>
///78..7f    xyz{|}~. 
///</summary>

			0x40,
			0x40,
			0x40,
			0x40,
			0x40,
			0x40,
			0x40,
			0x40,
			///
///<summary>
///80..87    ........ 
///</summary>

			0x40,
			0x40,
			0x40,
			0x40,
			0x40,
			0x40,
			0x40,
			0x40,
			///
///<summary>
///88..8f    ........ 
///</summary>

			0x40,
			0x40,
			0x40,
			0x40,
			0x40,
			0x40,
			0x40,
			0x40,
			///
///<summary>
///90..97    ........ 
///</summary>

			0x40,
			0x40,
			0x40,
			0x40,
			0x40,
			0x40,
			0x40,
			0x40,
			///
///<summary>
///98..9f    ........ 
///</summary>

			0x40,
			0x40,
			0x40,
			0x40,
			0x40,
			0x40,
			0x40,
			0x40,
			///
///<summary>
///a0..a7    ........ 
///</summary>

			0x40,
			0x40,
			0x40,
			0x40,
			0x40,
			0x40,
			0x40,
			0x40,
			///
///<summary>
///a8..af    ........ 
///</summary>

			0x40,
			0x40,
			0x40,
			0x40,
			0x40,
			0x40,
			0x40,
			0x40,
			///
///<summary>
///b0..b7    ........ 
///</summary>

			0x40,
			0x40,
			0x40,
			0x40,
			0x40,
			0x40,
			0x40,
			0x40,
			///
///<summary>
///b8..bf    ........ 
///</summary>

			0x40,
			0x40,
			0x40,
			0x40,
			0x40,
			0x40,
			0x40,
			0x40,
			///
///<summary>
///c0..c7    ........ 
///</summary>

			0x40,
			0x40,
			0x40,
			0x40,
			0x40,
			0x40,
			0x40,
			0x40,
			///
///<summary>
///c8..cf    ........ 
///</summary>

			0x40,
			0x40,
			0x40,
			0x40,
			0x40,
			0x40,
			0x40,
			0x40,
			///
///<summary>
///d0..d7    ........ 
///</summary>

			0x40,
			0x40,
			0x40,
			0x40,
			0x40,
			0x40,
			0x40,
			0x40,
			///
///<summary>
///d8..df    ........ 
///</summary>

			0x40,
			0x40,
			0x40,
			0x40,
			0x40,
			0x40,
			0x40,
			0x40,
			///
///<summary>
///e0..e7    ........ 
///</summary>

			0x40,
			0x40,
			0x40,
			0x40,
			0x40,
			0x40,
			0x40,
			0x40,
			///
///<summary>
///e8..ef    ........ 
///</summary>

			0x40,
			0x40,
			0x40,
			0x40,
			0x40,
			0x40,
			0x40,
			0x40,
			///
///<summary>
///f0..f7    ........ 
///</summary>

			0x40,
			0x40,
			0x40,
			0x40,
			0x40,
			0x40,
			0x40,
			0x40
		///
///<summary>
///f8..ff    ........ 
///</summary>

		};

		#endif
		#if SQLITE_USE_URI
																																						    const bool SQLITE_USE_URI = true;
#else
		//# define  SQLITE_USE_URI 0
		const bool SQLITE_USE_URI = false;

		#endif
		///
///<summary>
///The following singleton contains the global configuration for
///the SQLite library.
///</summary>

		static Sqlite3Config sqlite3Config = new Sqlite3Config (SQLITE_DEFAULT_MEMSTATUS, ///
///<summary>
///bMemstat 
///</summary>

		1, ///
///<summary>
///bCoreMutex 
///</summary>

		SQLITE_THREADSAFE != 0, ///
///<summary>
///bFullMutex 
///</summary>

		SQLITE_USE_URI, ///
///<summary>
///bOpenUri 
///</summary>

		0x7ffffffe, ///
///<summary>
///mxStrlen 
///</summary>

		100, ///
///<summary>
///szLookaside 
///</summary>

		500, ///
///<summary>
///nLookaside 
///</summary>

		new sqlite3_mem_methods (), ///
///<summary>
///m 
///</summary>

		new sqlite3_mutex_methods (null, null, null, null, null, null, null, null, null), ///
///<summary>
///mutex 
///</summary>

		new sqlite3_pcache_methods (), ///
///<summary>
///pcache 
///</summary>

		null, ///
///<summary>
///pHeap 
///</summary>

		0, ///
///<summary>
///nHeap 
///</summary>

		0, 0, ///
///<summary>
///mnHeap, mxHeap 
///</summary>

		null, ///
///<summary>
///pScratch 
///</summary>

		0, ///
///<summary>
///szScratch 
///</summary>

		0, ///
///<summary>
///nScratch 
///</summary>

		null, ///
///<summary>
///pPage 
///</summary>

		Limits.SQLITE_DEFAULT_PAGE_SIZE, ///
///<summary>
///szPage 
///</summary>

		0, ///
///<summary>
///nPage 
///</summary>

		0, ///
///<summary>
///mxParserStack 
///</summary>

		false, ///
///<summary>
///sharedCacheEnabled 
///</summary>

		///
///<summary>
///All the rest should always be initialized to zero 
///</summary>

		0, ///
///<summary>
///isInit 
///</summary>

		0, ///
///<summary>
///inProgress 
///</summary>

		0, ///
///<summary>
///isMutexInit 
///</summary>

		0, ///
///<summary>
///isMallocInit 
///</summary>

		0, ///
///<summary>
///isPCacheInit 
///</summary>

		null, ///
///<summary>
///pInitMutex 
///</summary>

		0, ///
///<summary>
///nRefInitMutex 
///</summary>

		null, ///
///<summary>
///xLog 
///</summary>

		0, ///
///<summary>
///pLogArg 
///</summary>

		false///
///<summary>
///bLocaltimeFault 
///</summary>

		);

		///
///<summary>
///</summary>
///<param name="Hash table for global functions "> functions common to all</param>
///<param name="database connections.  After initialization, this table is">database connections.  After initialization, this table is</param>
///<param name="read">only.</param>
///<param name=""></param>

		static FuncDefHash sqlite3GlobalFunctions;

		///
///<summary>
///Constant tokens for values 0 and 1.
///
///</summary>

		static Token[] sqlite3IntTokens =  {
			new Token ("0", 1),
			new Token ("1", 1)
		};

		///
///<summary>
///The value of the "pending" byte must be 0x40000000 (1 byte past the
///</summary>
///<param name="1">gibabyte boundary) in a compatible database.  SQLite never uses</param>
///<param name="the database page that contains the pending byte.  It never attempts">the database page that contains the pending byte.  It never attempts</param>
///<param name="to read or write that page.  The pending byte page is set assign">to read or write that page.  The pending byte page is set assign</param>
///<param name="for use by the VFS layers as space for managing file locks.">for use by the VFS layers as space for managing file locks.</param>
///<param name=""></param>
///<param name="During testing, it is often desirable to move the pending byte to">During testing, it is often desirable to move the pending byte to</param>
///<param name="a different position in the file.  This allows code that has to">a different position in the file.  This allows code that has to</param>
///<param name="deal with the pending byte to run on files that are much smaller">deal with the pending byte to run on files that are much smaller</param>
///<param name="than 1 GiB.  The sqlite3_test_control() interface can be used to">than 1 GiB.  The sqlite3_test_control() interface can be used to</param>
///<param name="move the pending byte.">move the pending byte.</param>
///<param name=""></param>
///<param name="IMPORTANT:  Changing the pending byte to any value other than">IMPORTANT:  Changing the pending byte to any value other than</param>
///<param name="0x40000000 results in an incompatible database file format!">0x40000000 results in an incompatible database file format!</param>
///<param name="Changing the pending byte during operating results in undefined">Changing the pending byte during operating results in undefined</param>
///<param name="and dileterious behavior.">and dileterious behavior.</param>
///<param name=""></param>

		#if !SQLITE_OMIT_WSD
		static int sqlite3PendingByte = 0x40000000;

		#endif
		//#include "opcodes.h"
		///
///<summary>
///Properties of opcodes.  The OPFLG_INITIALIZER macro is
///created by mkopcodeh.awk during compilation.  Data is obtained
///from the comments following the "case OP_xxxx:" statements in
///the vdbe.c file.  
///
///</summary>

		public static OpFlag[] sqlite3OpcodeProperty;
	}
}
