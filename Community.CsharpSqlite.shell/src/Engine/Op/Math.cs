using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FILE = System.IO.TextWriter;
using i64 = System.Int64;
using u8 = System.Byte;
using u16 = System.UInt16;
using u32 = System.UInt32;
using u64 = System.UInt64;
using unsigned = System.UIntPtr;
using Pgno = System.UInt32;
using i32 = System.Int32;
using sqlite_int64 = System.Int64;

namespace Community.CsharpSqlite.Engine.Op
{
    public class Math
    {
        
					
       public static RuntimeException Exec(CPU cpu, OpCode opcode, VdbeOp pOp){
           var aMem = cpu.aMem;
            switch (opcode)
            {
                ///
                ///<summary>
                ///Opcode: Add P1 P2 P3 * *
                ///
                ///Add the value in register P1 to the value in register P2
                ///and store the result in register P3.
                ///If either input is NULL, the result is NULL.
                ///
                ///</summary>
                ///
                ///<summary>
                ///Opcode: Multiply P1 P2 P3 * *
                ///
                ///
                ///Multiply the value in register P1 by the value in register P2
                ///and store the result in register P3.
                ///If either input is NULL, the result is NULL.
                ///
                ///</summary>
                ///
                ///<summary>
                ///Opcode: Subtract P1 P2 P3 * *
                ///
                ///Subtract the value in register P1 from the value in register P2
                ///and store the result in register P3.
                ///If either input is NULL, the result is NULL.
                ///
                ///</summary>
                ///
                ///<summary>
                ///Opcode: Divide P1 P2 P3 * *
                ///
                ///Divide the value in register P1 by the value in register P2
                ///and store the result in register P3 (P3=P2/P1). If the value in 
                ///register P1 is zero, then the result is NULL. If either input is 
                ///NULL, the result is NULL.
                ///
                ///</summary>
                ///
                ///<summary>
                ///Opcode: Remainder P1 P2 P3 * *
                ///
                ///Compute the remainder after integer division of the value in
                ///register P1 by the value in register P2 and store the result in P3.
                ///If the value in register P2 is zero the result is NULL.
                ///If either operand is NULL, the result is NULL.
                ///
                ///</summary>
                case OpCode.OP_Add:
                ///
                ///<summary>
                ///same as Sqlite3.TK_PLUS, in1, in2, ref3 
                ///</summary>
                case OpCode.OP_Subtract:
                ///
                ///<summary>
                ///same as Sqlite3.TK_MINUS, in1, in2, ref3 
                ///</summary>
                case OpCode.OP_Multiply:
                ///
                ///<summary>
                ///same as Sqlite3.TK_STAR, in1, in2, ref3 
                ///</summary>
                case OpCode.OP_Divide:
                ///
                ///<summary>
                ///same as Sqlite3.TK_SLASH, in1, in2, ref3 
                ///</summary>
                case OpCode.OP_Remainder:
                    {
                        ///same as Sqlite3.TK_REM, in1, in2, ref3 
                        MemFlags flags;
                        ///Combined MEM.MEM_* flags from both inputs 
                        i64 iA;
                        ///Integer value of left operand 
                        i64 iB = 0;
                        ///Integer value of right operand 
                        double rA;
                        ///Real value of left operand 
                        double rB;
                        ///Real value of right operand 
                        var pIn1 = aMem[pOp.p1];
                        pIn1.applyNumericAffinity();
                        var pIn2 = aMem[pOp.p2];
                        pIn2.applyNumericAffinity();
                        var pOut = aMem[pOp.p3];
                        flags = pIn1.flags | pIn2.flags;
                        if ((flags & MemFlags.MEM_Null) != 0)
                            goto arithmetic_result_is_null;
                        bool fp_math;
                        if (!(fp_math = !((pIn1.Flags & pIn2.Flags & MemFlags.MEM_Int) == MemFlags.MEM_Int)))
                        {
                            iA = pIn1.u.i;
                            iB = pIn2.u.i;
                            switch (pOp.OpCode)
                            {
                                case OpCode.OP_Add:
                                    {
                                        if (utilc.sqlite3AddInt64(ref iB, iA) != 0)
                                            fp_math = true;
                                        // goto fp_math
                                        break;
                                    }
                                case OpCode.OP_Subtract:
                                    {
                                        if (utilc.sqlite3SubInt64(ref iB, iA) != 0)
                                            fp_math = true;
                                        // goto fp_math
                                        break;
                                    }
                                case OpCode.OP_Multiply:
                                    {
                                        if (utilc.sqlite3MulInt64(ref iB, iA) != 0)
                                            fp_math = true;
                                        // goto fp_math
                                        break;
                                    }
                                case OpCode.OP_Divide:
                                    {
                                        if (iA == 0)
                                            goto arithmetic_result_is_null;
                                        if (iA == -1 && iB == IntegerExtensions.SMALLEST_INT64)
                                        {
                                            fp_math = true;
                                            // goto fp_math
                                            break;
                                        }
                                        iB /= iA;
                                        break;
                                    }
                                default:
                                    {
                                        if (iA == 0)
                                            goto arithmetic_result_is_null;
                                        if (iA == -1)
                                            iA = 1;
                                        iB %= iA;
                                        break;
                                    }
                            }
                        }
                        if (!fp_math)
                        {
                            pOut.u.i = iB;
                            pOut.MemSetTypeFlag(MemFlags.MEM_Int);
                        }
                        else
                        {
                            //fp_math:
                            rA = pIn1.sqlite3VdbeRealValue();
                            rB = pIn2.sqlite3VdbeRealValue();
                            switch (pOp.OpCode)
                            {
                                case OpCode.OP_Add:
                                    rB += rA;
                                    break;
                                case OpCode.OP_Subtract:
                                    rB -= rA;
                                    break;
                                case OpCode.OP_Multiply:
                                    rB *= rA;
                                    break;
                                case OpCode.OP_Divide:
                                    {
                                        ///
                                        ///<summary>
                                        ///(double)0 In case of SQLITE_OMIT_FLOATING_POINT... 
                                        ///</summary>
                                        if (rA == (double)0)
                                            goto arithmetic_result_is_null;
                                        rB /= rA;
                                        break;
                                    }
                                default:
                                    {
                                        iA = (i64)rA;
                                        iB = (i64)rB;
                                        if (iA == 0)
                                            goto arithmetic_result_is_null;
                                        if (iA == -1)
                                            iA = 1;
                                        rB = (double)(iB % iA);
                                        break;
                                    }
                            }
#if SQLITE_OMIT_FLOATING_POINT
																																																																																																																																																													pOut->u.i = rB;
MemSetTypeFlag(pOut, MEM.MEM_Int);
#else
                            if (MathExtensions.sqlite3IsNaN(rB))
                            {
                                goto arithmetic_result_is_null;
                            }
                            pOut.r = rB;
                            pOut.MemSetTypeFlag(MemFlags.MEM_Real);
                            if ((flags & MemFlags.MEM_Real) == 0)
                            {
                                pOut.sqlite3VdbeIntegerAffinity();
                            }
#endif
                        }
                        break;
                    arithmetic_result_is_null:
                        pOut.sqlite3VdbeMemSetNull();
                        break;
                    }

                ///Opcode: AddImm  P1 P2 * * *
                ///
                ///Add the constant P2 to the value in register P1.
                ///The result is always an integer.
                ///
                ///To force any register to be an integer, just add 0.
                case OpCode.OP_AddImm:
                    {
                        ///in1 
                        var pIn1 = aMem[pOp.p1];
                        cpu.vdbe.memAboutToChange(pIn1);
                        pIn1.sqlite3VdbeMemIntegerify();
                        pIn1.u.i += pOp.p2;
                        break;
                    }

                default: return RuntimeException.noop;
            }
            return RuntimeException.OK;
        }
    }
}
