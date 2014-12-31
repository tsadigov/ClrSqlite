using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Community.CsharpSqlite
{
	///<summary>
	/// A single instruction of the virtual machine has an opcode
	/// and as many as three operands.  The instruction is recorded
	/// as an instance of the following structure:
	///
	///</summary>
	public class union_p4
	{
		///
///<summary>
///fourth parameter 
///</summary>

		public int i;

		///
///<summary>
///Integer value if p4type==P4_INT32 
///</summary>

		public object p;

		///
///<summary>
///Generic pointer 
///</summary>

		//public string z;           /* Pointer to data for string (char array) types */
		public string z;

		// In C# string is unicode, so use byte[] instead
		public Int64 pI64;

		///
///<summary>
///Used when p4type is P4_INT64 
///</summary>

		public double pReal;

		///
///<summary>
///Used when p4type is P4_REAL 
///</summary>

		public Community.CsharpSqlite.Sqlite3.FuncDef pFunc;

		///
///<summary>
///Used when p4type is P4_FUNCDEF 
///</summary>

		public Community.CsharpSqlite.Sqlite3.VdbeFunc pVdbeFunc;

		///
///<summary>
///Used when p4type is P4_VDBEFUNC 
///</summary>

		public Community.CsharpSqlite.Sqlite3.CollSeq pColl;

		///
///<summary>
///Used when p4type is P4_COLLSEQ 
///</summary>

		public Community.CsharpSqlite.Sqlite3.Mem pMem;

		///
///<summary>
///Used when p4type is P4_MEM 
///</summary>

		public Community.CsharpSqlite.Sqlite3.VTable pVtab;

		///
///<summary>
///Used when p4type is P4_VTAB 
///</summary>

		public Community.CsharpSqlite.Sqlite3.KeyInfo pKeyInfo;

		///
///<summary>
///Used when p4type is P4_KEYINFO 
///</summary>

		public int[] ai;

		///
///<summary>
///Used when p4type is P4_INTARRAY 
///</summary>

		public Community.CsharpSqlite.SubProgram pProgram;

		///
///<summary>
///Used when p4type is P4_SUBPROGRAM 
///</summary>

		public Community.CsharpSqlite.Sqlite3.dxDel pFuncDel;
	///
///<summary>
///Used when p4type is P4_FUNCDEL 
///</summary>

	};

	public class VdbeOp
	{
		public VdbeOp ()
		{
		}

		public byte opcode {
			get;
			set;
		}

		public OpCode OpCode {
			get {
				return (OpCode)opcode;
			}
			set {
				opcode = (byte)value;
			}
		}

		///
///<summary>
///What operation to perform 
///</summary>

		public int p4type;

		///
///<summary>
///One of the P4_xxx constants for p4 
///</summary>

		public byte opflags;

		///
///<summary>
///Mask of the OPFLG_* flags in opcodes.h 
///</summary>

		public byte p5;

		///
///<summary>
///Fifth parameter is an unsigned character 
///</summary>

		#if DEBUG_CLASS_VDBEOP || DEBUG_CLASS_ALL
																																																								public int _p1;              /* First operand */
public int p1
{
get { return _p1; }
set { _p1 = value; }
}

public int _p2;              /* Second parameter (often the jump destination) */
public int p2
{
get { return _p2; }
set { _p2 = value; }
}

public int _p3;              /* The third parameter */
public int p3
{
get { return _p3; }
set { _p3 = value; }
}
#else
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
///The third parameter 
///</summary>

		#endif
		public union_p4 p4 = new union_p4 ();

		#if SQLITE_DEBUG || DEBUG
																																						      public string zComment;     /* Comment to improve readability */
#endif
		#if VDBE_PROFILE
																																						public int cnt;             /* Number of times this instruction was executed */
public u64 cycles;         /* Total time spend executing this instruction */
#endif
		public string ToString (Community.CsharpSqlite.Sqlite3.Parse parse)
		{
			var vdbe = parse.pVdbe;
			return ToString (vdbe);
		}

		internal string ToString (Sqlite3.Vdbe vdbe)
		{
            var p1 = this.p1;

			String str = null;
            switch (OpCode)
            {
                case OpCode.OP_MakeRecord:

                    break;

                case OpCode.OP_Column:

                    var vdbeCursor = vdbe.apCsr[p1];
                    str = "nField:" + vdbeCursor.nField + "\tpayloadSize" + vdbeCursor.payloadSize + "\taRow:" + vdbeCursor.aRow + "\taOffset:" + (vdbeCursor.aOffset==null?"":String.Join(",", vdbeCursor.aOffset.Select(x => "x" + x.ToString()).ToArray()));

                    break;

                case OpCode.OP_Insert:
                    var pData = vdbe.aMem[p2];
                    var pKey = vdbe.aMem[p3];
                    str = pKey.u.i + "<<";
                    if (null != pData.zBLOB)
                        str += String.Join(",", pData.zBLOB.Select(x => "x" + x.ToString()).ToArray());// Converter.ToH(pData.zBLOB);
                    break;

                case OpCode.OP_Yield:
                    var pIn1 = vdbe.aMem[this.p1];
                    str = pIn1.u.i.ToString();
                    break;

                case OpCode.OP_Goto:
                    str = p2.ToString();
                    break;
                case OpCode.OP_Integer:
                    str = p1.ToString();
                    break;
                case OpCode.OP_String8:
                    str = this.p4.z;
                    break;
                case OpCode.OP_Add:
                    {
                        var mem1 = vdbe.aMem[p1];
                        var mem2 = vdbe.aMem[p2];
                        str = mem2.u.i + "+" + mem1.u.i;
                    }
                    break;
                case OpCode.OP_Divide:
                    {
                        var mem1 = vdbe.aMem[p1];
                        var mem2 = vdbe.aMem[p2];
                        str = mem2.u.i + "/" + mem1.u.i;
                    }
                    break;
                case OpCode.OP_OpenRead:
                case OpCode.OP_OpenWrite:
                    var iDb = this.p3;
                    var pDb = vdbe.db.aDb[iDb];
                    str = pDb.zName;
                    break;
                case OpCode.OP_ParseSchema:
                    str = Sqlite3.vdbeaux.displayP4(this, "", 30);
                    break;
                case OpCode.OP_ReadCookie:
                case OpCode.OP_SetCookie:
                    str = ((Sqlite3.BTreeProp)p3).ToString();
                    break;
                case OpCode.OP_NewRowid:
                    var pC = vdbe.apCsr[p1];
                    if (null != pC && null != pC.pCursor)
                    {
                        int id = 0;
                        id = (int)pC.pCursor.sqlite3BtreeGetCachedRowid();
                        if (0 == id)
                            pC.pCursor.sqlite3BtreeLast(ref id);
                        str = "cached " + id;
                    }
                    break;
            }
			return OpCode.ToString () + " \t\t:\t " + str+Environment.NewLine;
		}
	}
}
