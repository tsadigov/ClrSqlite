using i64 = System.Int64;
using u8 = System.Byte;
using u64 = System.UInt64;

namespace Community.CsharpSqlite
{

    


    // #define  P4Usage.P4_KEYINFO_STATIC  (-17)
    
     
		

	public partial class Sqlite3
	{

		
	///<summary>
		/// The following macro converts a relative address in the p2 field
		/// of a VdbeOp structure into a negative number so that
		/// sqlite3VdbeAddOpList() knows that the address is relative.  Calling
		/// the macro again restores the address.
		///</summary>
		//#define ADDR(X)  (-1-(X))
		public static int ADDR (int x)
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
		
	}
}
