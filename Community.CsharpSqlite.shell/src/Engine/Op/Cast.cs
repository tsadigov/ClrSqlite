using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Community.CsharpSqlite.Engine.Op
{
    public static class Cast
    {
        public static RuntimeException Exec(CPU cpu,OpCode opcode,VdbeOp pOp)
        {
        //public static RuntimeException Exec(Community.CsharpSqlite.Vdbe vdbe, SqliteEncoding encoding,OpCode opcode,VdbeOp pOp,Mem [] aMem,ref SqlResult rc) {

            var aMem = cpu.aMem;
            var vdbe = cpu.vdbe;
            var encoding = cpu.encoding;

            switch (opcode) {
#if !SQLITE_OMIT_CAST
                ///
                ///<summary>
                ///Opcode: ToText P1 * * * *
                ///
                ///Force the value in register P1 to be text.
                ///If the value is numeric, convert it to a string using the
                ///equivalent of printf().  Blob values are unchanged and
                ///are afterwards simply interpreted as text.
                ///
                ///A NULL value is not changed by this routine.  It remains NULL.
                ///</summary>
                case OpCode.OP_ToText:
                    {
                        ///
                        ///<summary>
                        ///same as Sqlite3.TK_TO_TEXT, in1 
                        ///</summary>
                        var pIn1 = aMem[pOp.p1];
                        vdbe.memAboutToChange(pIn1);
                        if ((pIn1.flags & MemFlags.MEM_Null) != 0)
                            break;
                        Debug.Assert(MemFlags.MEM_Str == (MemFlags)((int)MemFlags.MEM_Blob >> 3));
                        pIn1.flags |= ((pIn1.flags & (MemFlags)((int)MemFlags.MEM_Blob >> 3)));
                        pIn1.applyAffinity(sqliteinth.SQLITE_AFF_TEXT, encoding);
                        cpu.rc = pIn1.ExpandBlob();
                        Debug.Assert((pIn1.flags & MemFlags.MEM_Str) != 0///
                            ///<summary>
                            ///|| db.mallocFailed != 0 
                            ///</summary>
                        );
                        pIn1.flags = (pIn1.flags & ~(MemFlags.MEM_Int | MemFlags.MEM_Real | MemFlags.MEM_Blob | MemFlags.MEM_Zero));
#if SQLITE_TEST
																																																																																																																																				              UPDATE_MAX_BLOBSIZE( pIn1 );
#endif
                        break;
                    }
                ///
                ///<summary>
                ///Opcode: ToBlob P1 * * * *
                ///
                ///Force the value in register P1 to be a BLOB.
                ///If the value is numeric, convert it to a string first.
                ///Strings are simply reinterpreted as blobs with no change
                ///to the underlying data.
                ///
                ///A NULL value is not changed by this routine.  It remains NULL.
                ///
                ///</summary>
                case OpCode.OP_ToBlob:
                    {
                        ///
                        ///<summary>
                        ///same as Sqlite3.TK_TO_BLOB, in1 
                        ///</summary>
                        var pIn1 = aMem[pOp.p1];
                        if ((pIn1.flags & MemFlags.MEM_Null) != 0)
                            break;
                        if ((pIn1.flags & MemFlags.MEM_Blob) == 0)
                        {
                            pIn1.applyAffinity(sqliteinth.SQLITE_AFF_TEXT, encoding);
                            Debug.Assert((pIn1.flags & MemFlags.MEM_Str) != 0///
                                ///|| db.mallocFailed != 0 
                            );
                            pIn1.MemSetTypeFlag(MemFlags.MEM_Blob);
                        }
                        else
                        {
                            pIn1.flags = (pIn1.flags & ~(MemFlags.MEM_TypeMask & ~MemFlags.MEM_Blob));
                        }
#if SQLITE_TEST
																																																																																																																																				              UPDATE_MAX_BLOBSIZE( pIn1 );
#endif
                        break;
                    }
                ///
                ///<summary>
                ///Opcode: ToNumeric P1 * * * *
                ///
                ///Force the value in register P1 to be numeric (either an
                ///</summary>
                ///<param name="integer or a floating">point number.)</param>
                ///<param name="If the value is text or blob, try to convert it to an using the">If the value is text or blob, try to convert it to an using the</param>
                ///<param name="equivalent of _Custom.atoi() or atof() and store 0 if no such conversion">equivalent of _Custom.atoi() or atof() and store 0 if no such conversion</param>
                ///<param name="is possible.">is possible.</param>
                ///<param name=""></param>
                ///<param name="A NULL value is not changed by this routine.  It remains NULL.">A NULL value is not changed by this routine.  It remains NULL.</param>
                ///<param name=""></param>
                case OpCode.OP_ToNumeric:
                    {
                        ///
                        ///<summary>
                        ///same as Sqlite3.TK_TO_NUMERIC, in1 
                        ///</summary>
                        var pIn1 = aMem[pOp.p1];
                        pIn1.sqlite3VdbeMemNumerify();
                        break;
                    }
#endif
                ///
                ///<summary>
                ///Opcode: ToInt P1 * * * *
                ///
                ///Force the value in register P1 to be an integer.  If
                ///The value is currently a real number, drop its fractional part.
                ///If the value is text or blob, try to convert it to an integer using the
                ///equivalent of _Custom.atoi() and store 0 if no such conversion is possible.
                ///
                ///A NULL value is not changed by this routine.  It remains NULL.
                ///</summary>
                case OpCode.OP_ToInt:
                    {
                        ///
                        ///<summary>
                        ///same as Sqlite3.TK_TO_INT, in1 
                        ///</summary>
                        var pIn1 = aMem[pOp.p1];
                        if ((pIn1.flags & MemFlags.MEM_Null) == 0)
                        {
                            pIn1.sqlite3VdbeMemIntegerify();
                        }
                        break;
                    }
#if !(SQLITE_OMIT_CAST) && !(SQLITE_OMIT_FLOATING_POINT)
                ///
                ///<summary>
                ///Opcode: ToReal P1 * * * *
                ///
                ///Force the value in register P1 to be a floating point number.
                ///If The value is currently an integer, convert it.
                ///If the value is text or blob, try to convert it to an integer using the
                ///equivalent of _Custom.atoi() and store 0.0 if no such conversion is possible.
                ///
                ///A NULL value is not changed by this routine.  It remains NULL.
                ///</summary>
                case OpCode.OP_ToReal:
                    {
                        ///
                        ///<summary>
                        ///same as Sqlite3.TK_TO_REAL, in1 
                        ///</summary>
                        var pIn1 = aMem[pOp.p1];
                        vdbe.memAboutToChange(pIn1);
                        if ((pIn1.flags & MemFlags.MEM_Null) == 0)
                        {
                            pIn1.sqlite3VdbeMemRealify();
                        }
                        break;
                    }
                    
#endif
                default: return RuntimeException.noop;
            }
            return RuntimeException.OK;
        }
    }
}
