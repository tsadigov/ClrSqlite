using System;
using System.Diagnostics;
using System.Text;
namespace Community.CsharpSqlite {
	using sqlite3_value=Sqlite3.Mem;
	public partial class Sqlite3 {
		/*
    ** 2008 June 13
    **
    ** The author disclaims copyright to this source code.  In place of
    ** a legal notice, here is a blessing:
    **
    **    May you do good and not evil.
    **    May you find forgiveness for yourself and forgive others.
    **    May you share freely, never taking more than you give.
    **
    *************************************************************************
    **
    ** This file contains definitions of global variables and contants.
    *************************************************************************
    **  Included in SQLite3 port to C#-SQLite;  2008 Noah B Hart
    **  C#-SQLite is an independent reimplementation of the SQLite software library
    **
    **  SQLITE_SOURCE_ID: 2011-06-23 19:49:22 4374b7e83ea0a3fbc3691f9c0c936272862f32f2
    **
    *************************************************************************
    *///#include "sqliteInt.h"
		/* An array to map all upper-case characters into their corresponding
    ** lower-case character.
    **
    ** SQLite only considers US-ASCII (or EBCDIC) characters.  We do not
    ** handle case conversions for the UTF character set since the tables
    ** involved are nearly as big or bigger than SQLite itself.
    *//* An array to map all upper-case characters into their corresponding
    ** lower-case character.
    *///
		// Replaced in C# with sqlite3UpperToLower class
		//    static int[] sqlite3UpperToLower = new int[]  {
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
		/*
    ** The following 256 byte lookup table is used to support SQLites built-in
    ** equivalents to the following standard library functions:
    **
    **   isspace()                        0x01
    **   isalpha()                        0x02
    **   isdigit()                        0x04
    **   isalnum()                        0x06
    **   isxdigit()                       0x08
    **   toupper()                        0x20
    **   SQLite identifier character      0x40
    **
    ** Bit 0x20 is set if the mapped character requires translation to upper
    ** case. i.e. if the character is a lower-case ASCII character.
    ** If x is a lower-case ASCII character, then its upper-case equivalent
    ** is (x - 0x20). Therefore toupper() can be implemented as:
    **
    **   (x & ~(map[x]&0x20))
    **
    ** Standard function tolower() is implemented using the sqlite3UpperToLower[]
    ** array. tolower() is used more often than toupper() by SQLite.
    **
    ** Bit 0x40 is set if the character non-alphanumeric and can be used in an 
    ** SQLite identifier.  Identifiers are alphanumerics, "_", "$", and any
    ** non-ASCII UTF character. Hence the test for whether or not a character is
    ** part of an identifier is 0x46.
    **
    ** SQLite's versions are identical to the standard versions assuming a
    ** locale of "C". They are implemented as macros in sqliteInt.h.
    */
		#if SQLITE_ASCII
		public static byte[] sqlite3CtypeMap=new byte[] {
			0x00,
			0x00,
			0x00,
			0x00,
			0x00,
			0x00,
			0x00,
			0x00,
			/* 00..07    ........ */0x00,
			0x01,
			0x01,
			0x01,
			0x01,
			0x01,
			0x00,
			0x00,
			/* 08..0f    ........ */0x00,
			0x00,
			0x00,
			0x00,
			0x00,
			0x00,
			0x00,
			0x00,
			/* 10..17    ........ */0x00,
			0x00,
			0x00,
			0x00,
			0x00,
			0x00,
			0x00,
			0x00,
			/* 18..1f    ........ */0x01,
			0x00,
			0x00,
			0x00,
			0x40,
			0x00,
			0x00,
			0x00,
			/* 20..27     !"#$%&' */0x00,
			0x00,
			0x00,
			0x00,
			0x00,
			0x00,
			0x00,
			0x00,
			/* 28..2f    ()*+,-./ */0x0c,
			0x0c,
			0x0c,
			0x0c,
			0x0c,
			0x0c,
			0x0c,
			0x0c,
			/* 30..37    01234567 */0x0c,
			0x0c,
			0x00,
			0x00,
			0x00,
			0x00,
			0x00,
			0x00,
			/* 38..3f    89:;<=>? */0x00,
			0x0a,
			0x0a,
			0x0a,
			0x0a,
			0x0a,
			0x0a,
			0x02,
			/* 40..47    @ABCDEFG */0x02,
			0x02,
			0x02,
			0x02,
			0x02,
			0x02,
			0x02,
			0x02,
			/* 48..4f    HIJKLMNO */0x02,
			0x02,
			0x02,
			0x02,
			0x02,
			0x02,
			0x02,
			0x02,
			/* 50..57    PQRSTUVW */0x02,
			0x02,
			0x02,
			0x00,
			0x00,
			0x00,
			0x00,
			0x40,
			/* 58..5f    XYZ[\]^_ */0x00,
			0x2a,
			0x2a,
			0x2a,
			0x2a,
			0x2a,
			0x2a,
			0x22,
			/* 60..67    `abcdefg */0x22,
			0x22,
			0x22,
			0x22,
			0x22,
			0x22,
			0x22,
			0x22,
			/* 68..6f    hijklmno */0x22,
			0x22,
			0x22,
			0x22,
			0x22,
			0x22,
			0x22,
			0x22,
			/* 70..77    pqrstuvw */0x22,
			0x22,
			0x22,
			0x00,
			0x00,
			0x00,
			0x00,
			0x00,
			/* 78..7f    xyz{|}~. */0x40,
			0x40,
			0x40,
			0x40,
			0x40,
			0x40,
			0x40,
			0x40,
			/* 80..87    ........ */0x40,
			0x40,
			0x40,
			0x40,
			0x40,
			0x40,
			0x40,
			0x40,
			/* 88..8f    ........ */0x40,
			0x40,
			0x40,
			0x40,
			0x40,
			0x40,
			0x40,
			0x40,
			/* 90..97    ........ */0x40,
			0x40,
			0x40,
			0x40,
			0x40,
			0x40,
			0x40,
			0x40,
			/* 98..9f    ........ */0x40,
			0x40,
			0x40,
			0x40,
			0x40,
			0x40,
			0x40,
			0x40,
			/* a0..a7    ........ */0x40,
			0x40,
			0x40,
			0x40,
			0x40,
			0x40,
			0x40,
			0x40,
			/* a8..af    ........ */0x40,
			0x40,
			0x40,
			0x40,
			0x40,
			0x40,
			0x40,
			0x40,
			/* b0..b7    ........ */0x40,
			0x40,
			0x40,
			0x40,
			0x40,
			0x40,
			0x40,
			0x40,
			/* b8..bf    ........ */0x40,
			0x40,
			0x40,
			0x40,
			0x40,
			0x40,
			0x40,
			0x40,
			/* c0..c7    ........ */0x40,
			0x40,
			0x40,
			0x40,
			0x40,
			0x40,
			0x40,
			0x40,
			/* c8..cf    ........ */0x40,
			0x40,
			0x40,
			0x40,
			0x40,
			0x40,
			0x40,
			0x40,
			/* d0..d7    ........ */0x40,
			0x40,
			0x40,
			0x40,
			0x40,
			0x40,
			0x40,
			0x40,
			/* d8..df    ........ */0x40,
			0x40,
			0x40,
			0x40,
			0x40,
			0x40,
			0x40,
			0x40,
			/* e0..e7    ........ */0x40,
			0x40,
			0x40,
			0x40,
			0x40,
			0x40,
			0x40,
			0x40,
			/* e8..ef    ........ */0x40,
			0x40,
			0x40,
			0x40,
			0x40,
			0x40,
			0x40,
			0x40,
			/* f0..f7    ........ */0x40,
			0x40,
			0x40,
			0x40,
			0x40,
			0x40,
			0x40,
			0x40
		/* f8..ff    ........ */};
		#endif
		#if SQLITE_USE_URI
																																    const bool SQLITE_USE_URI = true;
#else
		//# define  SQLITE_USE_URI 0
		const bool SQLITE_USE_URI=false;
		#endif
		/*
** The following singleton contains the global configuration for
** the SQLite library.
*/static Sqlite3Config sqlite3Config=new Sqlite3Config(SQLITE_DEFAULT_MEMSTATUS,/* bMemstat */1,/* bCoreMutex */SQLITE_THREADSAFE!=0,/* bFullMutex */SQLITE_USE_URI,/* bOpenUri */0x7ffffffe,/* mxStrlen */100,/* szLookaside */500,/* nLookaside */new sqlite3_mem_methods(),/* m */new sqlite3_mutex_methods(null,null,null,null,null,null,null,null,null),/* mutex */new sqlite3_pcache_methods(),/* pcache */null,/* pHeap */0,/* nHeap */0,0,/* mnHeap, mxHeap */null,/* pScratch */0,/* szScratch */0,/* nScratch */null,/* pPage */SQLITE_DEFAULT_PAGE_SIZE,/* szPage */0,/* nPage */0,/* mxParserStack */false,/* sharedCacheEnabled *//* All the rest should always be initialized to zero */0,/* isInit */0,/* inProgress */0,/* isMutexInit */0,/* isMallocInit */0,/* isPCacheInit */null,/* pInitMutex */0,/* nRefInitMutex */null,/* xLog */0,/* pLogArg */false/* bLocaltimeFault */);
		/*
    ** Hash table for global functions - functions common to all
    ** database connections.  After initialization, this table is
    ** read-only.
    */static FuncDefHash sqlite3GlobalFunctions;
		/*
    ** Constant tokens for values 0 and 1.
    */static Token[] sqlite3IntTokens= {
			new Token("0",1),
			new Token("1",1)
		};
		/*
    ** The value of the "pending" byte must be 0x40000000 (1 byte past the
    ** 1-gibabyte boundary) in a compatible database.  SQLite never uses
    ** the database page that contains the pending byte.  It never attempts
    ** to read or write that page.  The pending byte page is set assign
    ** for use by the VFS layers as space for managing file locks.
    **
    ** During testing, it is often desirable to move the pending byte to
    ** a different position in the file.  This allows code that has to
    ** deal with the pending byte to run on files that are much smaller
    ** than 1 GiB.  The sqlite3_test_control() interface can be used to
    ** move the pending byte.
    **
    ** IMPORTANT:  Changing the pending byte to any value other than
    ** 0x40000000 results in an incompatible database file format!
    ** Changing the pending byte during operating results in undefined
    ** and dileterious behavior.
    */
		#if !SQLITE_OMIT_WSD
		static int sqlite3PendingByte=0x40000000;
		#endif
		//#include "opcodes.h"
		/*
    ** Properties of opcodes.  The OPFLG_INITIALIZER macro is
    ** created by mkopcodeh.awk during compilation.  Data is obtained
    ** from the comments following the "case OP_xxxx:" statements in
    ** the vdbe.c file.  
    */public static int[] sqlite3OpcodeProperty;
	}
}
