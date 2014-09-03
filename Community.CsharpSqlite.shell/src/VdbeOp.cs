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
        /* fourth parameter */
        public int i;
        /* Integer value if p4type==P4_INT32 */
        public object p;
        /* Generic pointer */
        //public string z;           /* Pointer to data for string (char array) types */
        public string z;
        // In C# string is unicode, so use byte[] instead
        public Int64 pI64;
        /* Used when p4type is P4_INT64 */
        public double pReal;
        /* Used when p4type is P4_REAL */
        public Community.CsharpSqlite.Sqlite3.FuncDef pFunc;
        /* Used when p4type is P4_FUNCDEF */
        public Community.CsharpSqlite.Sqlite3.VdbeFunc pVdbeFunc;
        /* Used when p4type is P4_VDBEFUNC */
        public Community.CsharpSqlite.Sqlite3.CollSeq pColl;
        /* Used when p4type is P4_COLLSEQ */
        public Community.CsharpSqlite.Sqlite3.Mem pMem;
        /* Used when p4type is P4_MEM */
        public Community.CsharpSqlite.Sqlite3.VTable pVtab;
        /* Used when p4type is P4_VTAB */
        public Community.CsharpSqlite.Sqlite3.KeyInfo pKeyInfo;
        /* Used when p4type is P4_KEYINFO */
        public int[] ai;
        /* Used when p4type is P4_INTARRAY */
        public Community.CsharpSqlite.Sqlite3.SubProgram pProgram;
        /* Used when p4type is P4_SUBPROGRAM */
        public Community.CsharpSqlite.Sqlite3.dxDel pFuncDel;
        /* Used when p4type is P4_FUNCDEL */
    };

    public class VdbeOp
    {
        public VdbeOp()
        {

        }
        public byte opcode { get; set; }
        public OpCode OpCode { get { return (OpCode)opcode; } set { opcode = (byte)value; } }
        /* What operation to perform */
        public int p4type;
        /* One of the P4_xxx constants for p4 */
        public byte opflags;
        /* Mask of the OPFLG_* flags in opcodes.h */
        public byte p5;
        /* Fifth parameter is an unsigned character */
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
        /* First operand */
        public int p2;
        /* Second parameter (often the jump destination) */
        public int p3;
        /* The third parameter */
#endif
        public union_p4 p4 = new union_p4();
#if SQLITE_DEBUG || DEBUG
																																				      public string zComment;     /* Comment to improve readability */
#endif
#if VDBE_PROFILE
																																				public int cnt;             /* Number of times this instruction was executed */
public u64 cycles;         /* Total time spend executing this instruction */
#endif

        public string ToString(Community.CsharpSqlite.Sqlite3.Parse parse)
        {
            var vdbe=parse.pVdbe;
            return ToString(vdbe);
        }

        internal string ToString(Sqlite3.Vdbe vdbe)
        {
            String str = null;
            switch (OpCode)
            {
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
                    str=Sqlite3.displayP4(this, "", 30);
                    break;
            }
            return OpCode.ToString() + " \t\t:\t " + str;
        }
    }
}
