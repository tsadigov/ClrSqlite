using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Community.CsharpSqlite.Engine.Op
{
    using CsharpSqlite.Metadata;
    using sqlite3_value = Engine.Mem;
    public static class ControlFlow
    {
        public static RuntimeException Exec(CPU cpu, OpCode opcode, VdbeOp pOp)
        {
            var aMem = cpu.aMem;
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
                        if (pIn1.u.AsInteger > 0)
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
                        if (pIn1.u.AsInteger < 0)
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
                        pIn1.u.AsInteger += pOp.p3;
                        if (pIn1.u.AsInteger == 0)
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
                        pIn1.u.AsInteger = cpu.opcodeIndex;
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

                ///
                ///<summary>
                ///Opcode: Jump P1 P2 P3 * *
                ///
                ///Jump to the instruction at address P1, P2, or P3 depending on whether
                ///in the most recent  OpCode.OP_Compare instruction the P1 vector was less than
                ///equal to, or greater than the P2 vector, respectively.
                ///
                ///</summary>
                case OpCode.OP_Jump:
                    {
                        ///
                        ///<summary>
                        ///jump 
                        ///</summary>
                        if (cpu.iCompare < 0)
                        {
                            cpu.opcodeIndex = pOp.p1 - 1;
                        }
                        else
                            if (cpu.iCompare == 0)
                            {
                                cpu.opcodeIndex = pOp.p2 - 1;
                            }
                            else
                            {
                                cpu.opcodeIndex = pOp.p3 - 1;
                            }
                        break;
                    }

                ///
                ///<summary>
                ///Opcode: If P1 P2 P3 * *
                ///
                ///Jump to P2 if the value in register P1 is true.  The value
                ///</summary>
                ///<param name="is considered true if it is numeric and non">zero.  If the value</param>
                ///<param name="in P1 is NULL then take the jump if P3 is true.">in P1 is NULL then take the jump if P3 is true.</param>
                ///<param name=""></param>
                ///
                ///<summary>
                ///Opcode: IfNot P1 P2 P3 * *
                ///
                ///Jump to P2 if the value in register P1 is False.  The value
                ///is considered true if it has a numeric value of zero.  If the value
                ///in P1 is NULL then take the jump if P3 is true.
                ///
                ///</summary>
                case OpCode.OP_If:
                ///
                ///<summary>
                ///jump, in1 
                ///</summary>
                case OpCode.OP_IfNot:
                    {
                        ///
                        ///<summary>
                        ///jump, in1 
                        ///</summary>
                        int c;
                        var pIn1 = aMem[pOp.p1];
                        if ((pIn1.flags & MemFlags.MEM_Null) != 0)
                        {
                            c = pOp.p3;
                        }
                        else
                        {
#if SQLITE_OMIT_FLOATING_POINT
																																																																																																																																																													c = pIn1.sqlite3VdbeIntValue()!=0;
#else
                            c = (pIn1.ToReal() != 0.0) ? 1 : 0;
#endif
                            if (pOp.OpCode == OpCode.OP_IfNot)
                                c = (c == 0) ? 1 : 0;
                        }
                        if (c != 0)
                        {
                            cpu.opcodeIndex = pOp.p2 - 1;
                        }
                        break;
                    }
                ///
                ///<summary>
                ///Opcode: IsNull P1 P2 * * *
                ///
                ///Jump to P2 if the value in register P1 is NULL.
                ///
                ///</summary>
                case OpCode.OP_IsNull:
                    {
                        ///
                        ///<summary>
                        ///same as TokenType.TK_ISNULL, jump, in1 
                        ///</summary>
                        var pIn1 = aMem[pOp.p1];
                        if ((pIn1.flags & MemFlags.MEM_Null) != 0)
                        {
                            cpu.opcodeIndex = pOp.p2 - 1;
                        }
                        break;
                    }
                ///
                ///<summary>
                ///Opcode: NotNull P1 P2 * * *
                ///
                ///Jump to P2 if the value in register P1 is not NULL.
                ///
                ///</summary>
                case OpCode.OP_NotNull:
                    {
                        ///
                        ///<summary>
                        ///same as TokenType.TK_NOTNULL, jump, in1 
                        ///</summary>
                        var pIn1 = aMem[pOp.p1];
                        if ((pIn1.flags & MemFlags.MEM_Null) == 0)
                        {
                            cpu.opcodeIndex = pOp.p2 - 1;
                        }
                        break;
                    }




                ///
                ///<summary>
                ///Opcode: Function P1 P2 P3 P4 P5
                ///
                ///Invoke a user function (P4 is a pointer to a Function structure that
                ///defines the function) with P5 arguments taken from register P2 and
                ///successors.  The result of the function is stored in register P3.
                ///Register P3 must not be one of the function inputs.
                ///
                ///</summary>
                ///<param name="P1 is a 32">bit bitmask indicating whether or not each argument to the</param>
                ///<param name="function was determined to be constant at compile time. If the first">function was determined to be constant at compile time. If the first</param>
                ///<param name="argument was constant then bit 0 of P1 is set. This is used to determine">argument was constant then bit 0 of P1 is set. This is used to determine</param>
                ///<param name="whether meta data associated with a user function argument using the">whether meta data associated with a user function argument using the</param>
                ///<param name="sqlite3_set_auxdata() API may be safely retained until the next">sqlite3_set_auxdata() API may be safely retained until the next</param>
                ///<param name="invocation of this opcode.">invocation of this opcode.</param>
                ///<param name=""></param>
                ///<param name="See also: AggStep and AggFinal">See also: AggStep and AggFinal</param>
                ///<param name=""></param>
                case OpCode.OP_Function:
                    {
                        var vdbe = cpu.vdbe;
                        var db = vdbe.db;
                        int i;
                        Mem pArg;
                        sqlite3_context ctx = new sqlite3_context();
                        sqlite3_value[] apVal;
                        int n;
                        n = pOp.p5;
                        apVal = vdbe.apArg;
                        Debug.Assert(apVal != null || n == 0);
                        Debug.Assert(pOp.p3 > 0 && pOp.p3 <= vdbe.aMem.Count());
                        cpu.pOut = aMem[pOp.p3];
                        vdbe.memAboutToChange(cpu.pOut);
                        Debug.Assert(n == 0 || (pOp.p2 > 0 && pOp.p2 + n <= vdbe.aMem.Count() + 1));
                        Debug.Assert(pOp.p3 < pOp.p2 || pOp.p3 >= pOp.p2 + n);
                        //pArg = aMem[pOp.p2];
                        for (i = 0; i < n; i++)//, pArg++)
                        {
                            pArg = aMem[pOp.p2 + i];
                            Debug.Assert(pArg.memIsValid());
                            apVal[i] = pArg;
                            Sqlite3.Deephemeralize(pArg);
                            Sqlite3.sqlite3VdbeMemStoreType(pArg);
                            Sqlite3.REGISTER_TRACE(vdbe, pOp.p2 + i, pArg);
                        }
                        Debug.Assert(pOp.p4type == P4Usage.P4_FUNCDEF || pOp.p4type == P4Usage.P4_VDBEFUNC);
                        if (pOp.p4type == P4Usage.P4_FUNCDEF)
                        {
                            ctx.pFunc = pOp.p4.pFunc;
                            ctx.pVdbeFunc = null;
                        }
                        else
                        {
                            ctx.pVdbeFunc = (Metadata.VdbeFunc)pOp.p4.pVdbeFunc;
                            ctx.pFunc = ctx.pVdbeFunc.pFunc;
                        }
                        ctx.s.flags = MemFlags.MEM_Null;
                        ctx.s.db = db;
                        ctx.s.xDel = null;
                        //ctx.s.zMalloc = null;
                        ///
                        ///<summary>
                        ///The output cell may already have a buffer allocated. Move
                        ///</summary>
                        ///<param name="the pointer to ctx.s so in case the user">function can use</param>
                        ///<param name="the already allocated buffer instead of allocating a new one.">the already allocated buffer instead of allocating a new one.</param>
                        ///<param name=""></param>
                        vdbemem_cs.sqlite3VdbeMemMove(ctx.s, cpu.pOut);
                        ctx.s.MemSetTypeFlag(MemFlags.MEM_Null);
                        ctx.isError = 0;
                        if ((ctx.pFunc.flags & FuncFlags.SQLITE_FUNC_NEEDCOLL) != 0)
                        {
                            Debug.Assert(cpu.opcodeIndex > 1);
                            //Debug.Assert(pOp > aOp);
                            Debug.Assert(vdbe.lOp[cpu.opcodeIndex - 1].p4type == P4Usage.P4_COLLSEQ);
                            //Debug.Assert(pOp[-1].p4type ==  P4Usage.P4_COLLSEQ);
                            Debug.Assert(vdbe.lOp[cpu.opcodeIndex - 1].OpCode == OpCode.OP_CollSeq);
                            //Debug.Assert(pOp[-1].opcode ==  OpCode.OP_CollSeq);
                            ctx.pColl = vdbe.lOp[cpu.opcodeIndex - 1].p4.pColl;
                            //ctx.pColl = pOp[-1].p4.pColl;
                        }
                        db.lastRowid = cpu.lastRowid;
                        ctx.pFunc.xFunc(ctx, n, apVal);
                        ///* IMP: R-24505-23230 */
                        cpu.lastRowid = db.lastRowid;
                        ///
                        ///<summary>
                        ///If any auxillary data functions have been called by this user function,
                        ///</summary>
                        ///<param name="immediately call the destructor for any non">static values.</param>
                        ///<param name=""></param>
                        if (ctx.pVdbeFunc != null)
                        {
                            vdbeaux.sqlite3VdbeDeleteAuxData(ctx.pVdbeFunc, pOp.p1);
                            pOp.p4.pVdbeFunc = ctx.pVdbeFunc;
                            pOp.p4type = P4Usage.P4_VDBEFUNC;
                        }
                        //if ( db->mallocFailed )
                        //{
                        //  /* Even though a malloc() has failed, the implementation of the
                        //  ** user function may have called an sqlite3_result_XXX() function
                        //  ** to return a value. The following call releases any resources
                        //  ** associated with such a value.
                        //  */
                        //   &u.ag.ctx.s .sqlite3VdbeMemRelease();
                        //  goto no_mem;
                        //}
                        ///
                        ///<summary>
                        ///If the function returned an error, throw an exception 
                        ///</summary>
                        if (ctx.isError != 0)
                        {
                            malloc_cs.sqlite3SetString(ref vdbe.zErrMsg, db, vdbeapi.sqlite3_value_text(ctx.s));
                            cpu.rc = ctx.isError;
                        }
                        ///
                        ///<summary>
                        ///Copy the result of the function into register P3 
                        ///</summary>
                        vdbemem_cs.sqlite3VdbeChangeEncoding(ctx.s, vdbe.encoding);
                        vdbemem_cs.sqlite3VdbeMemMove(cpu.pOut, ctx.s);
                        if (cpu.pOut.IsTooBig())
                        {
                            return RuntimeException.too_big;
                        }
#if FALSE
																																																																																																																																				  /* The app-defined function has done something that as caused this
  ** statement to expire.  (Perhaps the function called sqlite3_exec()
  ** with a CREATE TABLE statement.)
  */
  if( p.expired ) rc = SQLITE_ABORT;
#endif
                        Sqlite3.REGISTER_TRACE(vdbe, pOp.p3, cpu.pOut);
#if SQLITE_TEST
																																																																																																																																				              UPDATE_MAX_BLOBSIZE( pOut );
#endif
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
            cpu.opcodeIndex = (int)pIn1.u.AsInteger;
        }


        private static void OpCode_Yield(CPU cpu, VdbeOp pOp)
        {
            ///in1 
            int pcDest;
            var pIn1 = cpu.aMem[pOp.p1];
            Debug.Assert((pIn1.flags & MemFlags.MEM_Dyn) == 0);
            pIn1.flags = MemFlags.MEM_Int;
            pcDest = (int)pIn1.u.AsInteger;
            pIn1.u.AsInteger = cpu.opcodeIndex;
            Sqlite3.REGISTER_TRACE(cpu.vdbe, pOp.p1, pIn1);
            cpu.opcodeIndex = pcDest;
        }

            
    }
}
