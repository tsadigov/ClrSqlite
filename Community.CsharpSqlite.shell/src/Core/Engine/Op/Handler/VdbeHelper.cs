using Community.CsharpSqlite.Engine;
using Community.CsharpSqlite.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Community.CsharpSqlite.Engine
{
    public static class _OpCode
    {
        public static RuntimeException Exec(CPU cpu,OpCode opcode,VdbeOp pOp)
        {
            Mem[]aMem=cpu.aMem;
            SqliteEncoding encoding = cpu.encoding;
            Connection db=cpu.db;
            Mem pOut = cpu.pOut;
            var vdbe = cpu.vdbe;

            switch (opcode)
            {

                ///Opcode: Null * P2 * * *
                ///
                ///Write a NULL into register P2.
                case OpCode.OP_Null:
                    {
                        ///<param name="out2">prerelease </param>
                        pOut.flags = MemFlags.MEM_Null;
                        break;
                    }


                ///Opcode: Blob P1 P2 * P4
                ///
                ///P4 points to a blob of data P1 bytes long.  Store this
                ///blob in register P2.
                case OpCode.OP_Blob:
                    {
                        ///<param name="out2">prerelease </param>
                        Debug.Assert(pOp.p1 <= db.aLimit[Globals.SQLITE_LIMIT_LENGTH]);
                        pOut.sqlite3VdbeMemSetStr(pOp.p4.z, pOp.p1, 0, null);
                        pOut.enc = encoding;
#if SQLITE_TEST
																																																																																																																																				              UPDATE_MAX_BLOBSIZE( pOut );
#endif
                        break;
                    }

                ///Opcode: Integer P1 P2 * * *
                ///<param name="The 32">bit integer value P1 is written into register P2.</param>
                ///<param name=""></param>
                case OpCode.OP_Integer:
                    {
                        ///<param name="out2">prerelease </param>
                        pOut.u.i = pOp.p1;
                        break;
                    }
                ///Opcode: Int64 * P2 * P4 *
                ///<param name="P4 is a pointer to a 64">bit integer value.</param>
                ///<param name="Write that value into register P2.">Write that value into register P2.</param>
                ///<param name=""></param>
                case OpCode.OP_Int64:
                    {
                        ///<param name="out2">prerelease </param>
                        // Integer pointer always exists Debug.Assert( pOp.p4.pI64 != 0 );
                        pOut.u.i = pOp.p4.pI64;
                        break;
                    }

#if !SQLITE_OMIT_FLOATING_POINT
                ///Opcode: Real * P2 * P4 *
                ///<param name="P4 is a pointer to a 64">bit floating point value.</param>
                ///<param name="Write that value into register P2.">Write that value into register P2.</param>
                case OpCode.OP_Real:
                    {
                        ///<param name="same as TokenType.TK_FLOAT, ref2">prerelease </param>
                        pOut.flags = MemFlags.MEM_Real;
                        Debug.Assert(!MathExtensions.sqlite3IsNaN(pOp.p4.pReal));
                        pOut.r = pOp.p4.pReal;
                        break;
                    }
#endif



                ///Opcode: String8 * P2 * P4 *
                ///<param name="P4 points to a nul terminated UTF">8 string. This opcode is transformed</param>
                ///<param name="into an  OpCode.OP_String before it is executed for the first time.">into an  OpCode.OP_String before it is executed for the first time.</param>
                case OpCode.OP_String8:
                    {
                        ///<param name="same as TokenType.TK_STRING, ref2">prerelease </param>
                        Debug.Assert(pOp.p4.z != null);
                        pOp.OpCode = OpCode.OP_String;
                        pOp.p1 = StringExtensions.Strlen30(pOp.p4.z);
#if !SQLITE_OMIT_UTF16
																																																																																																																																				if( encoding!=SqliteEncoding.UTF8 ){
rc = sqlite3VdbeMemSetStr(pOut, pOp.p4.z, -1, SqliteEncoding.UTF8, SQLITE_STATIC);
if( rc==SQLITE_TOOBIG ) goto too_big;
if( SqlResult.SQLITE_OK!=sqlite3VdbeChangeEncoding(pOut, encoding) ) goto no_mem;
Debug.Assert( pOut.zMalloc==pOut.z );
Debug.Assert( pOut.flags & MEM.MEM_Dyn );
pOut.zMalloc = 0;
pOut.flags |= MEM.MEM_Static;
pOut.flags &= ~MEM.MEM_Dyn;
if( pOp.p4type== P4Usage.P4_DYNAMIC ){
sqlite3DbFree(db, ref pOp.p4.z);
}
pOp.p4type =  P4Usage.P4_DYNAMIC;
pOp.p4.z = pOut.z;
pOp.p1 = pOut.n;
}
#endif
                        if (pOp.p1 > db.aLimit[Globals.SQLITE_LIMIT_LENGTH])
                        {
                            //goto too_big;//TODO:ERROR
                        }
                        ///
                        ///<summary>
                        ///Fall through to the next case,  OpCode.OP_String 
                        ///</summary>
                        goto case OpCode.OP_String;
                    }
                ///<summary>
                ///Opcode: String P1 P2 * P4 *
                ///
                ///The string value P4 of length P1 (bytes) is stored in register P2.
                ///</summary>
                case OpCode.OP_String:
                    {
                        ///<param name="out2">prerelease </param>
                        Debug.Assert(pOp.p4.z != null);
                        pOut.flags = MemFlags.MEM_Str | MemFlags.MEM_Static | MemFlags.MEM_Term;
                        malloc_cs.sqlite3_free(ref pOut.zBLOB);
                        pOut.z = pOp.p4.z;
                        pOut.n = pOp.p1;
#if SQLITE_OMIT_UTF16
                        pOut.enc = SqliteEncoding.UTF8;
#else
																																																																																																																																				              pOut.enc = encoding;
#endif
#if SQLITE_TEST
																																																																																																																																				              UPDATE_MAX_BLOBSIZE( pOut );
#endif
                        break;
                    }

                ///Opcode: Variable P1 P2 * P4 *
                ///
                ///Transfer the values of bound parameter P1 into register P2
                ///
                ///If the parameter is named, then its name appears in P4 and P3==1.
                ///The P4 value is used by sqlite3_bind_parameter_name().
                case OpCode.OP_Variable:
                    {
                        ///<param name="out2">prerelease </param>
                        Mem pVar;
                        ///Value being transferred 
                        //Debug.Assert(pOp.p1 >= 0 && pOp.p1 <= this.nVar);
                        Debug.Assert(pOp.p4.z == null || pOp.p4.z == vdbe.azVar[pOp.p1 - 1]);
                        pVar = vdbe.aVar[pOp.p1 - 1];
                        if (pVar.IsTooBig())
                        {
                            return RuntimeException.too_big;
                        }
                        vdbemem_cs.sqlite3VdbeMemShallowCopy(pOut, pVar, MemFlags.MEM_Static);
#if SQLITE_TEST
																																																																																																																																				              UPDATE_MAX_BLOBSIZE( pOut );
#endif
                        break;
                    }
                ///
                ///<summary>
                ///Opcode: Move P1 P2 P3 * *
                ///
                ///</summary>
                ///<param name="Move the values in register P1..P1+P3">1 over into</param>
                ///registers P2..P2+P3">1 are</param>
                ///left holding a NULL.  It is an error for register ranges</param>
                ///P1..P1+P3">1 to overlap.</param>
                case OpCode.OP_Move:
                    {
                        OpCode_Move(vdbe,pOp, aMem);
                        break;
                    }
                ///
                ///<summary>
                ///Opcode: Copy P1 P2 * * *
                ///
                ///Make a copy of register P1 into register P2.
                ///
                ///This instruction makes a deep copy of the value.  A duplicate
                ///is made of any string or blob constant.  See also  OpCode.OP_SCopy.
                ///
                ///</summary>
                case OpCode.OP_Copy:
                    {
                        ///
                        ///<summary>
                        ///in1, ref2 
                        ///</summary>
                        var pIn1 = aMem[pOp.p1];
                        pOut = aMem[pOp.p2];
                        Debug.Assert(pOut != pIn1);
                        vdbemem_cs.sqlite3VdbeMemShallowCopy(pOut, pIn1, MemFlags.MEM_Ephem);
                        if ((pOut.flags & MemFlags.MEM_Ephem) != 0 && pOut.sqlite3VdbeMemMakeWriteable() != 0)
                        {
                            return RuntimeException.no_mem;
                        }
                        //Deephemeralize( pOut );
                        Sqlite3.REGISTER_TRACE(vdbe, pOp.p2, pOut);
                        break;
                    }
                ///
                ///<summary>
                ///Opcode: SCopy P1 P2 * * *
                ///
                ///Make a shallow copy of register P1 into register P2.
                ///
                ///This instruction makes a shallow copy of the value.  If the value
                ///is a string or blob, then the copy is only a pointer to the
                ///original and hence if the original changes so will the copy.
                ///Worse, if the original is deallocated, the copy becomes invalid.
                ///Thus the program must guarantee that the original will not change
                ///during the lifetime of the copy.  Use  OpCode.OP_Copy to make a complete
                ///copy.
                ///
                ///</summary>
                case OpCode.OP_SCopy:
                    {
                        OpCode_SCopy(vdbe, pOp, aMem);
                        break;
                    }

                default: return RuntimeException.noop;
            }
            return RuntimeException.OK;
        }

        static  void OpCode_SCopy(Vdbe vdbe ,VdbeOp pOp, Mem[] aMem)
        {
            ///
            ///<summary>
            ///in1, ref2 
            ///</summary>
            var pIn1 = aMem[pOp.p1];
            var pOut = aMem[pOp.p2];
            Debug.Assert(pOut != pIn1);
            vdbemem_cs.sqlite3VdbeMemShallowCopy(pOut, pIn1, MemFlags.MEM_Ephem);
#if SQLITE_DEBUG
																																																																																																																																				              if ( pOut.pScopyFrom == null )
                pOut.pScopyFrom = pIn1;
#endif
            Sqlite3.REGISTER_TRACE(vdbe, pOp.p2, pOut);
        }

        static void OpCode_Move(Vdbe vdbe,VdbeOp pOp, Mem[] aMem)
        {
            //char* zMalloc;   /* Holding variable for allocated memory */
            int n;
            ///Number of registers left to copy 
            int p1;
            ///Register to copy from 
            int p2;
            ///Register to copy to 
            n = pOp.p3;
            p1 = pOp.p1;
            p2 = pOp.p2;
            Debug.Assert(n > 0 && p1 > 0 && p2 > 0);
            Debug.Assert(p1 + n <= p2 || p2 + n <= p1);
            //pIn1 = aMem[p1];
            //pOut = aMem[p2];
            while (n-- != 0)
            {
                var pIn1 = aMem[p1 + pOp.p3 - n - 1];
                var pOut = aMem[p2];
                //Debug.Assert( pOut<=&aMem[p.nMem] );
                //Debug.Assert( pIn1<=&aMem[p.nMem] );
                Debug.Assert(pIn1.memIsValid());
                vdbe.memAboutToChange(pOut);
                //zMalloc = pOut.zMalloc;
                //pOut.zMalloc = null;
                vdbemem_cs.sqlite3VdbeMemMove(pOut, pIn1);
                //pIn1.zMalloc = zMalloc;
                Sqlite3.REGISTER_TRACE(vdbe, p2++, pOut);
                //pIn1++;
                //pOut++;
            }
        }

    }
}
