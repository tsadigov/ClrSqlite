using i64 = System.Int64;
using u8 = System.Byte;
using u64 = System.UInt64;

namespace Community.CsharpSqlite
{
	public partial class Sqlite3
	{
		///
///<summary>
///2001 September 15
///
///The author disclaims copyright to this source code.  In place of
///a legal notice, here is a blessing:
///
///May you do good and not evil.
///May you find forgiveness for yourself and forgive others.
///May you share freely, never taking more than you give.
///
///
///Header file for the Virtual DataBase Engine (VDBE)
///
///This header defines the interface to the virtual database engine
///or VDBE.  The VDBE implements an abstract machine that runs a
///simple program to access and modify the underlying database.
///
///</summary>
///<param name="Included in SQLite3 port to C#">SQLite;  2008 Noah B Hart</param>
///<param name="C#">SQLite is an independent reimplementation of the SQLite software library</param>
///<param name=""></param>
///<param name="SQLITE_SOURCE_ID: 2011">23 19:49:22 4374b7e83ea0a3fbc3691f9c0c936272862f32f2</param>
///<param name=""></param>
///<param name=""></param>
///<param name=""></param>

		//#if !_SQLITE_VDBE_H_
		//#define _SQLITE_VDBE_H_
		//#include <stdio.h>
		///
///<summary>
///A single VDBE is an opaque structure named "Vdbe".  Only routines
///in the source file sqliteVdbe.c are allowed to see the insides
///of this structure.
///
///</summary>

		//typedef struct Vdbe Vdbe;
		///<summary>
		/// The names of the following types declared in vdbeInt.h are required
		/// for the VdbeOp definition.
		///
		///</summary>
		//typedef struct VdbeFunc VdbeFunc;
		//typedef struct Mem Mem;
		//typedef struct SubProgram SubProgram;
		//typedef struct VdbeOp VdbeOp;
		///<summary>
		/// A sub-routine used to implement a trigger program.
		///
		///</summary>
		public class SubProgram
		{
			public VdbeOp[] aOp;

			///
///<summary>
///</summary>
///<param name="Array of opcodes for sub">program </param>

			public int nOp;

			///
///<summary>
///Elements in aOp[] 
///</summary>

			public int nMem;

			///
///<summary>
///Number of memory cells required 
///</summary>

			public int nCsr;

			///
///<summary>
///Number of cursors required 
///</summary>

			public int token;

			///
///<summary>
///id that may be used to recursive triggers 
///</summary>

			public SubProgram pNext;
		///
///<summary>
///</summary>
///<param name="Next sub">program already visited </param>

		};


		///
///<summary>
///A smaller version of VdbeOp used for the VdbeAddOpList() function because
///it takes up less space.
///
///</summary>

		public struct VdbeOpList
		{
			public u8 opcode;

			///
///<summary>
///What operation to perform 
///</summary>

			public int p1;

			///
///<summary>
///First operand 
///</summary>

			public int p2;

			///
///<summary>
///Second parameter (often the jump destination) 
///</summary>

			public int p3;

			///
///<summary>
///Third parameter 
///</summary>

			public VdbeOpList (u8 opcode, int p1, int p2, int p3)
			{
				this.opcode = opcode;
				this.p1 = p1;
				this.p2 = p2;
				this.p3 = p3;
			}
		}

		//typedef struct VdbeOpList VdbeOpList;
		///
///<summary>
///Allowed values of VdbeOp.p4type
///
///</summary>

		const int P4_NOTUSED = 0;

		///
///<summary>
///The P4 parameter is not used 
///</summary>

		public const int P4_DYNAMIC = (-1);

		///
///<summary>
///Pointer to a string obtained from sqliteMalloc=(); 
///</summary>

		const int P4_STATIC = (-2);

		///
///<summary>
///Pointer to a static string 
///</summary>

		const int P4_COLLSEQ = (-4);

		///
///<summary>
///P4 is a pointer to a CollSeq structure 
///</summary>

		const int P4_FUNCDEF = (-5);

		///
///<summary>
///P4 is a pointer to a FuncDef structure 
///</summary>

		const int P4_KEYINFO = (-6);

		///
///<summary>
///P4 is a pointer to a KeyInfo structure 
///</summary>

		const int P4_VDBEFUNC = (-7);

		///
///<summary>
///P4 is a pointer to a VdbeFunc structure 
///</summary>

		const int P4_MEM = (-8);

		///
///<summary>
///P4 is a pointer to a Mem*    structure 
///</summary>

		const int P4_TRANSIENT = 0;

		///
///<summary>
///P4 is a pointer to a transient string 
///</summary>

		const int P4_VTAB = (-10);

		///
///<summary>
///P4 is a pointer to an sqlite3_vtab structure 
///</summary>

		const int P4_MPRINTF = (-11);

		///
///<summary>
///P4 is a string obtained from sqlite3_mprintf=(); 
///</summary>

		const int P4_REAL = (-12);

		///
///<summary>
///</summary>
///<param name="P4 is a 64">bit floating point value </param>

		const int P4_INT64 = (-13);

		///
///<summary>
///</summary>
///<param name="P4 is a 64">bit signed integer </param>

		const int P4_INT32 = (-14);

		///
///<summary>
///</summary>
///<param name="P4 is a 32">bit signed integer </param>

		const int P4_INTARRAY = (-15);

		///
///<summary>
///</summary>
///<param name="#define P4_INTARRAY (">bit integers </param>

		const int P4_SUBPROGRAM = (-18);

		///
///<summary>
///</summary>
///<param name="#define P4_SUBPROGRAM  (">18) /* P4 is a pointer to a SubProgram structure </param>

		///
///<summary>
///When adding a P4 argument using P4_KEYINFO, a copy of the KeyInfo structure
///is made.  That copy is freed when the Vdbe is finalized.  But if the
///argument is P4_KEYINFO_HANDOFF, the passed in pointer is used.  It still
///gets freed when the Vdbe is finalized so it still should be obtained
///from a single sqliteMalloc().  But no copy is made and the calling
///function should *not* try to free the KeyInfo.
///</summary>

		const int P4_KEYINFO_HANDOFF = (-16);

		// #define P4_KEYINFO_HANDOFF (-16)
		const int P4_KEYINFO_STATIC = (-17);

		// #define P4_KEYINFO_STATIC  (-17)
		///
///<summary>
///The Vdbe.aColName array contains 5n Mem structures, where n is the
///number of columns of data returned by the statement.
///
///</summary>

		//#define COLNAME_NAME     0
		//#define COLNAME_DECLTYPE 1
		//#define COLNAME_DATABASE 2
		//#define COLNAME_TABLE    3
		//#define COLNAME_COLUMN   4
		//#if SQLITE_ENABLE_COLUMN_METADATA
		//# define COLNAME_N        5      /* Number of COLNAME_xxx symbols */
		//#else
		//# ifdef SQLITE_OMIT_DECLTYPE
		//#   define COLNAME_N      1      /* Store only the name */
		//# else
		//#   define COLNAME_N      2      /* Store the name and decltype */
		//# endif
		//#endif
		const int COLNAME_NAME = 0;

		const int COLNAME_DECLTYPE = 1;

		const int COLNAME_DATABASE = 2;

		const int COLNAME_TABLE = 3;

		const int COLNAME_COLUMN = 4;

		#if SQLITE_ENABLE_COLUMN_METADATA
																																						const int COLNAME_N = 5;     /* Number of COLNAME_xxx symbols */
#else
		#if SQLITE_OMIT_DECLTYPE
																																						const int COLNAME_N = 1;     /* Number of COLNAME_xxx symbols */
#else
		const int COLNAME_N = 2;

		#endif
		#endif
		///<summary>
		/// The following macro converts a relative address in the p2 field
		/// of a VdbeOp structure into a negative number so that
		/// sqlite3VdbeAddOpList() knows that the address is relative.  Calling
		/// the macro again restores the address.
		///</summary>
		//#define ADDR(X)  (-1-(X))
		static int ADDR (int x)
		{
			return -1 - x;
		}

		///
///<summary>
///The makefile scans the vdbe.c source file and creates the "opcodes.h"
///header file that defines a number for each opcode used by the VDBE.
///
///</summary>

		//#include "opcodes.h"
		///
///<summary>
///Prototypes for the VDBE interface.  See comments on the implementation
///for a description of what each of these routines does.
///
///</summary>

		///<summary>
		/// Prototypes for the VDBE interface.  See comments on the implementation
		/// for a description of what each of these routines does.
		///
		///</summary>
		//Vdbe *sqlite3VdbeCreate(sqlite3);
		//int sqlite3VdbeAddOp0(Vdbe*,int);
		//int sqlite3VdbeAddOp1(Vdbe*,int,int);
		//int sqlite3VdbeAddOp2(Vdbe*,int,int,int);
		//int sqlite3VdbeAddOp3(Vdbe*,int,int,int,int);
		//int sqlite3VdbeAddOp4(Vdbe*,int,int,int,int,string zP4,int);
		//int sqlite3VdbeAddOp4Int(Vdbe*,int,int,int,int,int);
		//int sqlite3VdbeAddOpList(Vdbe*, int nOp, VdbeOpList const *aOp);
		//void sqlite3VdbeAddParseSchemaOp(Vdbe*,int,char);
		//void sqlite3VdbeChangeP1(Vdbe*, int addr, int P1);
		//void sqlite3VdbeChangeP2(Vdbe*, int addr, int P2);
		//void sqlite3VdbeChangeP3(Vdbe*, int addr, int P3);
		//void sqlite3VdbeChangeP5(Vdbe*, u8 P5);
		//void sqlite3VdbeJumpHere(Vdbe*, int addr);
		//void sqlite3VdbeChangeToNoop(Vdbe*, int addr, int N);
		//void sqlite3VdbeChangeP4(Vdbe*, int addr, string zP4, int N);
		//void sqlite3VdbeUsesBtree(Vdbe*, int);
		//VdbeOp *sqlite3VdbeGetOp(Vdbe*, int);
		//int sqlite3VdbeMakeLabel(Vdbe);
		//void sqlite3VdbeRunOnlyOnce(Vdbe);
		//void sqlite3VdbeDelete(Vdbe);
		//void sqlite3VdbeDeleteObject(sqlite3*,Vdbe);
		//void sqlite3VdbeMakeReady(Vdbe*,Parse);
		//int sqlite3VdbeFinalize(Vdbe);
		//void sqlite3VdbeResolveLabel(Vdbe*, int);
		//int sqlite3VdbeCurrentAddr(Vdbe);
		//#if SQLITE_DEBUG
		//  int sqlite3VdbeAssertMayAbort(Vdbe *, int);
		//  void sqlite3VdbeTrace(Vdbe*,FILE);
		//#endif
		//void sqlite3VdbeResetStepResult(Vdbe);
		//void sqlite3VdbeRewind(Vdbe);
		//int sqlite3VdbeReset(Vdbe);
		//void sqlite3VdbeSetNumCols(Vdbe*,int);
		//int sqlite3VdbeSetColName(Vdbe*, int, int, string , void()(void));
		//void sqlite3VdbeCountChanges(Vdbe);
		//sqlite3 *sqlite3VdbeDb(Vdbe);
		//void sqlite3VdbeSetSql(Vdbe*, string z, int n, int);
		//void sqlite3VdbeSwap(Vdbe*,Vdbe);
		//VdbeOp *sqlite3VdbeTakeOpArray(Vdbe*, int*, int);
		//sqlite3_value *sqlite3VdbeGetValue(Vdbe*, int, u8);
		//void sqlite3VdbeSetVarmask(Vdbe*, int);
		//#if !SQLITE_OMIT_TRACE
		//  char *sqlite3VdbeExpandSql(Vdbe*, const char);
		//#endif
		//UnpackedRecord *sqlite3VdbeRecordUnpack(KeyInfo*,int,const void*,char*,int);
		//void sqlite3VdbeDeleteUnpackedRecord(UnpackedRecord);
		//int sqlite3VdbeRecordCompare(int,const void*,UnpackedRecord);
		//#if !SQLITE_OMIT_TRIGGER
		//void sqlite3VdbeLinkSubProgram(Vdbe *, SubProgram );
		//#endif
		#if !NDEBUG
																																						    //void sqlite3VdbeComment(Vdbe*, const char*, ...);
    static void VdbeComment( Vdbe v, string zFormat, params object[] ap )
    {
      sqlite3VdbeComment( v, zFormat, ap );
    }// define VdbeComment(X)  sqlite3VdbeComment X
    //void sqlite3VdbeNoopComment(Vdbe*, const char*, ...);
    static void VdbeNoopComment( Vdbe v, string zFormat, params object[] ap )
    {
      sqlite3VdbeNoopComment( v, zFormat, ap );
    }// define VdbeNoopComment(X)  sqlite3VdbeNoopComment X
#else
		//# define VdbeComment(X)
		static void VdbeComment (Vdbe v, string zFormat, params object[] ap)
		{
		}

		//# define VdbeNoopComment(X)
		static void VdbeNoopComment (Vdbe v, string zFormat, params object[] ap)
		{
		}
	#endif
	}
}
