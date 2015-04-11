using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using u8 = System.Byte;
using u64 = System.UInt64;
using i64 = System.Int64;

namespace Community.CsharpSqlite.Engine.Op
{

    using Vdbe=Community.CsharpSqlite.Sqlite3.Vdbe;
    using Community.CsharpSqlite.Engine;
    public class Logical
    {
        public static RuntimeException Exec(CPU cpu, OpCode opcode, VdbeOp pOp)

        //public static RuntimeException Exec(SqliteEncoding encoding,Vdbe vdbe, OpCode opcode, VdbeOp pOp, Mem[] aMem, Mem pOut, ref int opcodeIndex)
        {
            var vdbe = cpu.vdbe;
            var aMem = cpu.aMem;


            switch (opcode) {
                ///
                ///<summary>
                ///Opcode: Lt P1 P2 P3 P4 P5
                ///
                ///Compare the values in register P1 and P3.  If reg(P3)<reg(P1) then
                ///jump to address P2.
                ///
                ///If the sqliteinth.SQLITE_JUMPIFNULL bit of P5 is set and either reg(P1) or
                ///reg(P3) is NULL then take the jump.  If the sqliteinth.SQLITE_JUMPIFNULL
                ///bit is clear then fall through if either operand is NULL.
                ///
                ///</summary>
                ///<param name="The SQLITE_AFF_MASK portion of P5 must be an affinity character "></param>
                ///<param name="sqliteinth.SQLITE_AFF_TEXT, SQLITE_AFF_INTEGER, and so forth. An attempt is made">sqliteinth.SQLITE_AFF_TEXT, SQLITE_AFF_INTEGER, and so forth. An attempt is made</param>
                ///<param name="to coerce both inputs according to this affinity before the">to coerce both inputs according to this affinity before the</param>
                ///<param name="comparison is made. If the SQLITE_AFF_MASK is 0x00, then numeric">comparison is made. If the SQLITE_AFF_MASK is 0x00, then numeric</param>
                ///<param name="affinity is used. Note that the affinity conversions are stored">affinity is used. Note that the affinity conversions are stored</param>
                ///<param name="back into the input registers P1 and P3.  So this opcode can cause">back into the input registers P1 and P3.  So this opcode can cause</param>
                ///<param name="persistent changes to registers P1 and P3.">persistent changes to registers P1 and P3.</param>
                ///<param name=""></param>
                ///<param name="Once any conversions have taken place, and neither value is NULL,">Once any conversions have taken place, and neither value is NULL,</param>
                ///<param name="the values are compared. If both values are blobs then memcmp() is">the values are compared. If both values are blobs then memcmp() is</param>
                ///<param name="used to determine the results of the comparison.  If both values">used to determine the results of the comparison.  If both values</param>
                ///<param name="are text, then the appropriate collating function specified in">are text, then the appropriate collating function specified in</param>
                ///<param name="P4 is  used to do the comparison.  If P4 is not specified then">P4 is  used to do the comparison.  If P4 is not specified then</param>
                ///<param name="memcmp() is used to compare text string.  If both values are">memcmp() is used to compare text string.  If both values are</param>
                ///<param name="numeric, then a numeric comparison is used. If the two values">numeric, then a numeric comparison is used. If the two values</param>
                ///<param name="are of different types, then numbers are considered less than">are of different types, then numbers are considered less than</param>
                ///<param name="strings and strings are considered less than blobs.">strings and strings are considered less than blobs.</param>
                ///<param name=""></param>
                ///<param name="If the SQLITE_STOREP2 bit of P5 is set, then do not jump.  Instead,">If the SQLITE_STOREP2 bit of P5 is set, then do not jump.  Instead,</param>
                ///<param name="store a boolean result (either 0, or 1, or NULL) in register P2.">store a boolean result (either 0, or 1, or NULL) in register P2.</param>
                ///
                ///<summary>
                ///Opcode: Ne P1 P2 P3 P4 P5
                ///
                ///This works just like the Lt opcode except that the jump is taken if
                ///the operands in registers P1 and P3 are not equal.  See the Lt opcode for
                ///additional information.
                ///
                ///If sqliteinth.SQLITE_NULLEQ is set in P5 then the result of comparison is always either
                ///true or false and is never NULL.  If both operands are NULL then the result
                ///of comparison is false.  If either operand is NULL then the result is true.
                ///If neither operand is NULL the result is the same as it would be if
                ///the sqliteinth.SQLITE_NULLEQ flag were omitted from P5.
                ///
                ///</summary>
                ///
                ///<summary>
                ///Opcode: Eq P1 P2 P3 P4 P5
                ///
                ///This works just like the Lt opcode except that the jump is taken if
                ///the operands in registers P1 and P3 are equal.
                ///See the Lt opcode for additional information.
                ///
                ///If sqliteinth.SQLITE_NULLEQ is set in P5 then the result of comparison is always either
                ///true or false and is never NULL.  If both operands are NULL then the result
                ///of comparison is true.  If either operand is NULL then the result is false.
                ///If neither operand is NULL the result is the same as it would be if
                ///the sqliteinth.SQLITE_NULLEQ flag were omitted from P5.
                ///
                ///</summary>
                ///
                ///<summary>
                ///Opcode: Le P1 P2 P3 P4 P5
                ///
                ///This works just like the Lt opcode except that the jump is taken if
                ///the content of register P3 is less than or equal to the content of
                ///register P1.  See the Lt opcode for additional information.
                ///
                ///</summary>
                ///
                ///<summary>
                ///Opcode: Gt P1 P2 P3 P4 P5
                ///
                ///This works just like the Lt opcode except that the jump is taken if
                ///the content of register P3 is greater than the content of
                ///register P1.  See the Lt opcode for additional information.
                ///
                ///</summary>
                ///
                ///<summary>
                ///Opcode: Ge P1 P2 P3 P4 P5
                ///
                ///This works just like the Lt opcode except that the jump is taken if
                ///the content of register P3 is greater than or equal to the content of
                ///register P1.  See the Lt opcode for additional information.
                ///
                ///</summary>
                case OpCode.OP_Eq:
                ///
                ///<summary>
                ///same as Sqlite3.TK_EQ, jump, in1, in3 
                ///</summary>
                case OpCode.OP_Ne:
                ///
                ///<summary>
                ///same as Sqlite3.TK_NE, jump, in1, in3 
                ///</summary>
                case OpCode.OP_Lt:
                ///
                ///<summary>
                ///same as Sqlite3.TK_LT, jump, in1, in3 
                ///</summary>
                case OpCode.OP_Le:
                ///
                ///<summary>
                ///same as Sqlite3.TK_LE, jump, in1, in3 
                ///</summary>
                case OpCode.OP_Gt:
                ///
                ///<summary>
                ///same as Sqlite3.TK_GT, jump, in1, in3 
                ///</summary>
                case OpCode.OP_Ge:
                    {
                        ///
                        ///<summary>
                        ///same as Sqlite3.TK_GE, jump, in1, in3 
                        ///</summary>
                        int res = 0;
                        ///
                        ///<summary>
                        ///Result of the comparison of pIn1 against pIn3 
                        ///</summary>
                        char affinity;
                        ///
                        ///<summary>
                        ///Affinity to use for comparison 
                        ///</summary>
                        MemFlags flags1;
                        ///
                        ///<summary>
                        ///</summary>
                        ///<param name="Copy of initial value of pIn1">>flags </param>
                        MemFlags flags3;
                        ///
                        ///<summary>
                        ///</summary>
                        ///<param name="Copy of initial value of pIn3">>flags </param>
                        var pIn1 = aMem[pOp.p1];
                        var pIn3 = aMem[pOp.p3];
                        flags1 = pIn1.flags;
                        flags3 = pIn3.flags;
                        if (((pIn1.flags | pIn3.flags) & MemFlags.MEM_Null) != 0)
                        {
                            ///
                            ///<summary>
                            ///One or both operands are NULL 
                            ///</summary>
                            if ((pOp.p5 & sqliteinth.SQLITE_NULLEQ) != 0)
                            {
                                ///
                                ///<summary>
                                ///If sqliteinth.SQLITE_NULLEQ is set (which will only happen if the operator is
                                ///OP_Eq or  OpCode.OP_Ne) then take the jump or not depending on whether
                                ///or not both operands are null.
                                ///
                                ///</summary>
                                Debug.Assert(pOp.OpCode == OpCode.OP_Eq || pOp.OpCode == OpCode.OP_Ne);
                                res = (pIn1.flags & pIn3.flags & MemFlags.MEM_Null) == 0 ? 1 : 0;
                            }
                            else
                            {
                                ///
                                ///<summary>
                                ///sqliteinth.SQLITE_NULLEQ is clear and at least one operand is NULL,
                                ///then the result is always NULL.
                                ///The jump is taken if the sqliteinth.SQLITE_JUMPIFNULL bit is set.
                                ///
                                ///</summary>
                                if ((pOp.p5 & sqliteinth.SQLITE_STOREP2) != 0)
                                {
                                    var pOut = aMem[pOp.p2];
                                    pOut.MemSetTypeFlag(MemFlags.MEM_Null);
                                    Sqlite3.REGISTER_TRACE(vdbe, pOp.p2, pOut);
                                }
                                else
                                    if ((pOp.p5 & sqliteinth.SQLITE_JUMPIFNULL) != 0)
                                    {
                                        cpu.opcodeIndex = pOp.p2 - 1;
                                    }
                                break;
                            }
                        }
                        else
                        {
                            ///
                            ///<summary>
                            ///Neither operand is NULL.  Do a comparison. 
                            ///</summary>
                            affinity = (char)(pOp.p5 & sqliteinth.SQLITE_AFF_MASK);
                            if (affinity != '\0')
                            {
                                pIn1.applyAffinity(affinity, cpu.encoding);
                                pIn3.applyAffinity(affinity, cpu.encoding);
                                //      if ( db.mallocFailed != 0 ) goto no_mem;
                            }
                            Debug.Assert(pOp.p4type == P4Usage.P4_COLLSEQ || pOp.p4.pColl == null);
                            pIn1.ExpandBlob();
                            pIn3.ExpandBlob();
                            res = vdbemem_cs.sqlite3MemCompare(pIn3, pIn1, pOp.p4.pColl);
                        }
                        switch (pOp.OpCode)
                        {
                            case OpCode.OP_Eq:
                                res = (res == 0) ? 1 : 0;
                                break;
                            case OpCode.OP_Ne:
                                res = (res != 0) ? 1 : 0;
                                break;
                            case OpCode.OP_Lt:
                                res = (res < 0) ? 1 : 0;
                                break;
                            case OpCode.OP_Le:
                                res = (res <= 0) ? 1 : 0;
                                break;
                            case OpCode.OP_Gt:
                                res = (res > 0) ? 1 : 0;
                                break;
                            default:
                                res = (res >= 0) ? 1 : 0;
                                break;
                        }
                        if ((pOp.p5 & sqliteinth.SQLITE_STOREP2) != 0)
                        {
                            var pOut = aMem[pOp.p2];
                            vdbe.memAboutToChange(pOut);
                            pOut.MemSetTypeFlag(MemFlags.MEM_Int);
                            pOut.u.i = res;
                            Sqlite3.REGISTER_TRACE(vdbe, pOp.p2, pOut);
                        }
                        else
                            if (res != 0)
                            {
                                cpu.opcodeIndex = pOp.p2 - 1;
                            }
                        ///
                        ///<summary>
                        ///Undo any changes made by applyAffinity() to the input registers. 
                        ///</summary>
                        pIn1.flags = ((pIn1.flags & ~MemFlags.MEM_TypeMask) | (flags1 & MemFlags.MEM_TypeMask));
                        pIn3.flags = ((pIn3.flags & ~MemFlags.MEM_TypeMask) | (flags3 & MemFlags.MEM_TypeMask));
                        break;
                    }



                ///
                ///<summary>
                ///Opcode: BitAnd P1 P2 P3 * *
                ///
                ///</summary>
                ///<param name="Take the bit">wise AND of the values in register P1 and P2 and</param>
                ///<param name="store the result in register P3.">store the result in register P3.</param>
                ///<param name="If either input is NULL, the result is NULL.">If either input is NULL, the result is NULL.</param>
                ///<param name=""></param>
                ///
                ///<summary>
                ///Opcode: BitOr P1 P2 P3 * *
                ///
                ///</summary>
                ///<param name="Take the bit">wise OR of the values in register P1 and P2 and</param>
                ///<param name="store the result in register P3.">store the result in register P3.</param>
                ///<param name="If either input is NULL, the result is NULL.">If either input is NULL, the result is NULL.</param>
                ///<param name=""></param>
                ///
                ///<summary>
                ///Opcode: ShiftLeft P1 P2 P3 * *
                ///
                ///Shift the integer value in register P2 to the left by the
                ///number of bits specified by the integer in register P1.
                ///Store the result in register P3.
                ///If either input is NULL, the result is NULL.
                ///
                ///</summary>
                ///
                ///<summary>
                ///Opcode: ShiftRight P1 P2 P3 * *
                ///
                ///Shift the integer value in register P2 to the right by the
                ///number of bits specified by the integer in register P1.
                ///Store the result in register P3.
                ///If either input is NULL, the result is NULL.
                ///
                ///</summary>
                case OpCode.OP_BitAnd:
                ///same as Sqlite3.TK_BITAND, in1, in2, ref3 
                case OpCode.OP_BitOr:
                ///same as Sqlite3.TK_BITOR, in1, in2, ref3 
                case OpCode.OP_ShiftLeft:
                ///same as Sqlite3.TK_LSHIFT, in1, in2, ref3 
                case OpCode.OP_ShiftRight:
                    {
                        ///
                        ///<summary>
                        ///same as Sqlite3.TK_RSHIFT, in1, in2, ref3 
                        ///</summary>
                        i64 iA;
                        u64 uA;
                        i64 iB;
                        OpCode op;
                        var pIn1 = aMem[pOp.p1];
                        var pIn2 = aMem[pOp.p2];
                        var pOut = aMem[pOp.p3];
                        if (((pIn1.flags | pIn2.flags) & MemFlags.MEM_Null) != 0)
                        {
                            pOut.sqlite3VdbeMemSetNull();
                            break;
                        }
                        iA = pIn2.sqlite3VdbeIntValue();
                        iB = pIn1.sqlite3VdbeIntValue();
                        op = pOp.OpCode;
                        if (op == OpCode.OP_BitAnd)
                        {
                            iA &= iB;
                        }
                        else
                            if (op == OpCode.OP_BitOr)
                            {
                                iA |= iB;
                            }
                            else
                                if (iB != 0)
                                {
                                    Debug.Assert(op == OpCode.OP_ShiftRight || op == OpCode.OP_ShiftLeft);
                                    ///
                                    ///<summary>
                                    ///If shifting by a negative amount, shift in the other direction 
                                    ///</summary>
                                    if (iB < 0)
                                    {
                                        Debug.Assert(OpCode.OP_ShiftRight == OpCode.OP_ShiftLeft + 1);
                                        op = (OpCode)(2 * (u8)OpCode.OP_ShiftLeft + 1 - op);
                                        iB = iB > (-64) ? -iB : 64;
                                    }
                                    if (iB >= 64)
                                    {
                                        iA = (iA >= 0 || op == OpCode.OP_ShiftLeft) ? 0 : -1;
                                    }
                                    else
                                    {
                                        //uA = (ulong)(iA << 0); // memcpy( &uA, &iA, sizeof( uA ) );
                                        if (op == OpCode.OP_ShiftLeft)
                                        {
                                            iA = iA << (int)iB;
                                        }
                                        else
                                        {
                                            iA = iA >> (int)iB;
                                            ///
                                            ///<summary>
                                            ///</summary>
                                            ///<param name="Sign">extend on a right shift of a negative number </param>
                                            //if ( iA < 0 )
                                            //  uA |= ( ( (0xffffffff ) << (u8)32 ) | 0xffffffff ) << (u8)( 64 - iB );
                                        }
                                        //iA = (long)( uA << 0 ); //memcpy( &iA, &uA, sizeof( iA ) );
                                    }
                                }
                        pOut.u.i = iA;
                        pOut.MemSetTypeFlag(MemFlags.MEM_Int);
                        break;
                    }













                ///
                ///<summary>
                ///Opcode: And P1 P2 P3 * *
                ///
                ///Take the logical AND of the values in registers P1 and P2 and
                ///write the result into register P3.
                ///
                ///If either P1 or P2 is 0 (false) then the result is 0 even if
                ///the other input is NULL.  A NULL and true or two NULLs give
                ///a NULL output.
                ///
                ///</summary>
                ///
                ///<summary>
                ///Opcode: Or P1 P2 P3 * *
                ///
                ///Take the logical OR of the values in register P1 and P2 and
                ///store the answer in register P3.
                ///
                ///If either P1 or P2 is nonzero (true) then the result is 1 (true)
                ///even if the other input is NULL.  A NULL and false or two NULLs
                ///give a NULL output.
                ///
                ///</summary>
                case OpCode.OP_And:
                ///
                ///<summary>
                ///same as Sqlite3.TK_AND, in1, in2, ref3 
                ///</summary>
                case OpCode.OP_Or:
                    {   
                        OpCode_AndOr(pOp, aMem);
                        break;
                    }
                ///
                ///<summary>
                ///Opcode: Not P1 P2 * * *
                ///
                ///Interpret the value in register P1 as a boolean value.  Store the
                ///boolean complement in register P2.  If the value in register P1 is
                ///NULL, then a NULL is stored in P2.
                ///
                ///</summary>
                case OpCode.OP_Not:
                    {
                        ///
                        ///<summary>
                        ///same as Sqlite3.TK_NOT, in1 
                        ///</summary>
                        var pIn1 = aMem[pOp.p1];
                        var pOut = aMem[pOp.p2];
                        if ((pIn1.flags & MemFlags.MEM_Null) != 0)
                        {
                            pOut.sqlite3VdbeMemSetNull();
                        }
                        else
                        {
                            pOut.sqlite3VdbeMemSetInt64(pIn1.sqlite3VdbeIntValue() == 0 ? 1 : 0);
                        }
                        break;
                    }
                ///
                ///<summary>
                ///Opcode: BitNot P1 P2 * * *
                ///
                ///Interpret the content of register P1 as an integer.  Store the
                ///</summary>
                ///<param name="ones">complement of the P1 value into register P2.  If P1 holds</param>
                ///<param name="a NULL then store a NULL in P2.">a NULL then store a NULL in P2.</param>
                ///<param name=""></param>
                case OpCode.OP_BitNot:
                    {
                        ///
                        ///<summary>
                        ///same as Sqlite3.TK_BITNOT, in1 
                        ///</summary>
                        var pIn1 = aMem[pOp.p1];
                        var pOut = aMem[pOp.p2];
                        if ((pIn1.flags & MemFlags.MEM_Null) != 0)
                        {
                            pOut.sqlite3VdbeMemSetNull();
                        }
                        else
                        {
                            pOut.sqlite3VdbeMemSetInt64(~pIn1.sqlite3VdbeIntValue());
                        }
                        break;
                    }

                default: return RuntimeException.noop;
            }
            return RuntimeException.OK;
        }

        private static void OpCode_AndOr(VdbeOp pOp, Mem[] aMem)
        {
            ///same as Sqlite3.TK_OR, in1, in2, ref3 
            int v1;
            ///Left operand:  0==FALSE, 1==TRUE, 2==UNKNOWN or NULL 
            int v2;
            ///Right operand: 0==FALSE, 1==TRUE, 2==UNKNOWN or NULL 
            var pIn1 = aMem[pOp.p1];
            if ((pIn1.flags & MemFlags.MEM_Null) != 0)
            {
                v1 = 2;
            }
            else
            {
                v1 = (pIn1.sqlite3VdbeIntValue() != 0) ? 1 : 0;
            }
            var pIn2 = aMem[pOp.p2];
            if ((pIn2.flags & MemFlags.MEM_Null) != 0)
            {
                v2 = 2;
            }
            else
            {
                v2 = (pIn2.sqlite3VdbeIntValue() != 0) ? 1 : 0;
            }
            if (pOp.OpCode == OpCode.OP_And)
            {
                byte[] and_logic = new byte[] {
									0,
									0,
									0,
									0,
									1,
									2,
									0,
									2,
									2
								};
                v1 = and_logic[v1 * 3 + v2];
            }
            else
            {
                byte[] or_logic = new byte[] {
									0,
									1,
									2,
									1,
									1,
									1,
									2,
									1,
									2
								};
                v1 = or_logic[v1 * 3 + v2];
            }
            var pOut = aMem[pOp.p3];
            if (v1 == 2)
            {
                pOut.MemSetTypeFlag(MemFlags.MEM_Null);
            }
            else
            {
                pOut.u.i = v1;
                pOut.MemSetTypeFlag(MemFlags.MEM_Int);
            }
        }

    }
}
