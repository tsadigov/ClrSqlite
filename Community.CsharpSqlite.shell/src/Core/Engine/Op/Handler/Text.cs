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
using Community.CsharpSqlite.Utils;
using System.Diagnostics;

namespace Community.CsharpSqlite.Engine.Op
{
    public class Text
    {
        
					
       public static RuntimeException Exec(CPU cpu, OpCode opcode, VdbeOp pOp){
           var aMem = cpu.aMem;
            var db = cpu.db;

            switch (opcode)
            {
                ///<summary>
                ///Opcode: Concat P1 P2 P3 * *
                ///
                ///Add the text in register P1 onto the end of the text in
                ///register P2 and store the result in register P3.
                ///If either the P1 or P2 text are NULL then store NULL in P3.
                ///
                ///P3 = P2 || P1
                ///
                ///It is illegal for P1 and P3 to be the same register. Sometimes,
                ///if P3 is the same register as P2, the implementation is able
                ///to avoid a memcpy().
                ///
                ///</summary>
                case OpCode.OP_Concat:
                    {
                        ///
                        ///<summary>
                        ///same as TokenType.TK_CONCAT, in1, in2, ref3 
                        ///</summary>
                        i64 nByte;
                        var pIn1 = aMem[pOp.p1];
                        var pIn2 = aMem[pOp.p2];
                        var pOut = aMem[pOp.p3];
                        Debug.Assert(pIn1 != pOut);
                        if (((pIn1.flags | pIn2.flags) & MemFlags.MEM_Null) != 0)
                        {
                            pOut.sqlite3VdbeMemSetNull();
                            break;
                        }
                        if (pIn1.ExpandBlob() != 0 || pIn2.ExpandBlob() != 0)
                            return RuntimeException.no_mem;
                        if (((pIn1.flags & (MemFlags.MEM_Str | MemFlags.MEM_Blob)) == 0) && vdbemem_cs.sqlite3VdbeMemStringify(pIn1, cpu.encoding) != 0)
                        {
                            return RuntimeException.no_mem;
                        }
                        // Stringify(pIn1, encoding);
                        if (((pIn2.flags & (MemFlags.MEM_Str | MemFlags.MEM_Blob)) == 0) && vdbemem_cs.sqlite3VdbeMemStringify(pIn2, cpu.encoding) != 0)
                        {
                            return RuntimeException.no_mem;
                        }
                        // Stringify(pIn2, encoding);
                        nByte = pIn1.CharacterCount + pIn2.CharacterCount;
                        if (nByte > db.aLimit[Globals.SQLITE_LIMIT_LENGTH])
                        {
                            return RuntimeException.too_big;
                        }
                        pOut.MemSetTypeFlag(MemFlags.MEM_Str);
                        //if ( sqlite3VdbeMemGrow( pOut, (int)nByte + 2, ( pOut == pIn2 ) ? 1 : 0 ) != 0 )
                        //{
                        //  goto no_mem;
                        //}
                        //if ( pOut != pIn2 )
                        //{
                        //  memcpy( pOut.z, pIn2.z, pIn2.n );
                        //}
                        //memcpy( &pOut.z[pIn2.n], pIn1.z, pIn1.n );
                        if (pIn2.AsString != null && pIn2.AsString.Length >= pIn2.CharacterCount)
                            if (pIn1.AsString != null)
                                pOut.AsString = pIn2.AsString.Substring(0, pIn2.CharacterCount) + (pIn1.CharacterCount < pIn1.AsString.Length ? pIn1.AsString.Substring(0, pIn1.CharacterCount) : pIn1.AsString);
                            else
                            {
                                if ((pIn1.flags & MemFlags.MEM_Blob) == 0)//String as Blob
                                {
                                    StringBuilder sb = new StringBuilder(pIn1.CharacterCount);
                                    for (int i = 0; i < pIn1.CharacterCount; i++)
                                        sb.Append((byte)pIn1.zBLOB[i]);
                                    pOut.AsString = pIn2.AsString.Substring(0, pIn2.CharacterCount) + sb.ToString();
                                }
                                else
                                    // UTF-8 Blob
                                    pOut.AsString = pIn2.AsString.Substring(0, pIn2.CharacterCount) + Encoding.UTF8.GetString(pIn1.zBLOB, 0, pIn1.zBLOB.Length);
                            }
                        else
                        {
                            pOut.zBLOB = malloc_cs.sqlite3Malloc(pIn1.CharacterCount + pIn2.CharacterCount);
                            Buffer.BlockCopy(pIn2.zBLOB, 0, pOut.zBLOB, 0, pIn2.CharacterCount);
                            if (pIn1.zBLOB != null)
                                Buffer.BlockCopy(pIn1.zBLOB, 0, pOut.zBLOB, pIn2.CharacterCount, pIn1.CharacterCount);
                            else
                                for (int i = 0; i < pIn1.CharacterCount; i++)
                                    pOut.zBLOB[pIn2.CharacterCount + i] = (byte)pIn1.AsString[i];
                        }
                        //pOut.z[nByte] = 0;
                        //pOut.z[nByte + 1] = 0;
                        pOut.flags |= MemFlags.MEM_Term;
                        pOut.CharacterCount = (int)nByte;
                        pOut.enc = cpu.encoding;
#if SQLITE_TEST
																																																																																																																																				              UPDATE_MAX_BLOBSIZE( pOut );
#endif
                        break;
                    }




                default: return RuntimeException.noop;
            }
            return RuntimeException.OK;
        }
    }
}
