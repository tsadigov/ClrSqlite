using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Community.CsharpSqlite.Engine.Op
{
    public static class ControlFlow
    {
        public static RuntimeException Exec(CPU cpu, OpCode opcode, VdbeOp pOp)
        {
            switch (opcode)
            {
                ///
                ///<summary>
                ///Opcode: IfPos P1 P2 * * *
                ///
                ///If the value of register P1 is 1 or greater, jump to P2.
                ///
                ///It is illegal to use this instruction on a register that does
                ///not contain an integer.  An Debug.Assertion fault will result if you try.
                ///</summary>
                case OpCode.OP_IfPos:
                    {
                        ///jump, in1 
                        var pIn1 = cpu.aMem[pOp.p1];
                        Debug.Assert((pIn1.flags & MemFlags.MEM_Int) != 0);
                        if (pIn1.u.i > 0)
                        {
                            cpu.opcodeIndex = pOp.p2 - 1;
                        }
                        break;
                    }
                ///
                ///<summary>
                ///Opcode: IfNeg P1 P2 * * *
                ///
                ///If the value of register P1 is less than zero, jump to P2.
                ///
                ///It is illegal to use this instruction on a register that does
                ///not contain an integer.  An Debug.Assertion fault will result if you try.
                ///
                ///</summary>
                case OpCode.OP_IfNeg:
                    {
                        ///
                        ///<summary>
                        ///jump, in1 
                        ///</summary>
                        var pIn1 = cpu.aMem[pOp.p1];
                        Debug.Assert((pIn1.flags & MemFlags.MEM_Int) != 0);
                        if (pIn1.u.i < 0)
                        {
                            cpu.opcodeIndex = pOp.p2 - 1;
                        }
                        break;
                    }
                ///
                ///<summary>
                ///Opcode: IfZero P1 P2 P3 * *
                ///
                ///The register P1 must contain an integer.  Add literal P3 to the
                ///value in register P1.  If the result is exactly 0, jump to P2. 
                ///
                ///It is illegal to use this instruction on a register that does
                ///not contain an integer.  An assertion fault will result if you try.
                ///
                ///</summary>
                case OpCode.OP_IfZero:
                    {
                        ///
                        ///<summary>
                        ///jump, in1 
                        ///</summary>
                        var pIn1 = cpu.aMem[pOp.p1];
                        Debug.Assert((pIn1.flags & MemFlags.MEM_Int) != 0);
                        pIn1.u.i += pOp.p3;
                        if (pIn1.u.i == 0)
                        {
                            cpu.opcodeIndex = pOp.p2 - 1;
                        }
                        break;
                    }



                ///<summary>
                ///Opcode:  Goto * P2 * * *
                ///
                ///An unconditional jump to address P2.
                ///The next instruction executed will be
                ///the one at index P2 from the beginning of
                ///the program.
                ///</summary>
                case OpCode.OP_Goto:
                    {
                        ///
                        ///<summary>
                        ///jump 
                        ///</summary>
                        if (cpu.db.u1.isInterrupted)
                            return RuntimeException.abort_due_to_interrupt;
                        //CHECK_FOR_INTERRUPT;
                        cpu.opcodeIndex = pOp.p2 - 1;
                        break;
                    }
                ///Opcode:  Gosub P1 P2 * * *
                ///
                ///Write the current address onto register P1
                ///and then jump to address P2.
                case OpCode.OP_Gosub:
                    {
                        ///jump, in1 
                        var pIn1 = cpu.aMem[pOp.p1];
                        Debug.Assert((pIn1.flags & MemFlags.MEM_Dyn) == 0);
                        cpu.vdbe.memAboutToChange(pIn1);
                        pIn1.flags = MemFlags.MEM_Int;
                        pIn1.u.i = cpu.opcodeIndex;
                        Sqlite3.REGISTER_TRACE(cpu.vdbe, pOp.p1, pIn1);
                        cpu.opcodeIndex = pOp.p2 - 1;
                        break;
                    }

                ///<summary>
                ///Opcode:  Return P1 * * * *
                ///
                ///Jump to the next instruction after the address in register P1.
                ///
                ///</summary>
                case OpCode.OP_Return:
                    {
                        OpCode_Return(cpu ,pOp );
                        break;
                    }
                ///
                ///<summary>
                ///Opcode:  Yield P1 * * * *
                ///
                ///Swap the program counter with the value in register P1.
                ///
                ///</summary>
                case OpCode.OP_Yield:
                    {
                        OpCode_Yield(cpu, pOp);
                        break;
                    }
                default: return RuntimeException.noop;
            }
            return RuntimeException.OK;
        }
        private static void OpCode_Return(CPU cpu, VdbeOp pOp)
        {
            ///in1 
            var pIn1 = cpu.aMem[pOp.p1];
            Debug.Assert((pIn1.flags & MemFlags.MEM_Int) != 0);
            cpu.opcodeIndex = (int)pIn1.u.i;
        }


        private static void OpCode_Yield(CPU cpu, VdbeOp pOp)
        {
            ///in1 
            int pcDest;
            var pIn1 = cpu.aMem[pOp.p1];
            Debug.Assert((pIn1.flags & MemFlags.MEM_Dyn) == 0);
            pIn1.flags = MemFlags.MEM_Int;
            pcDest = (int)pIn1.u.i;
            pIn1.u.i = cpu.opcodeIndex;
            Sqlite3.REGISTER_TRACE(cpu.vdbe, pOp.p1, pIn1);
            cpu.opcodeIndex = pcDest;
        }

            
    }
}
