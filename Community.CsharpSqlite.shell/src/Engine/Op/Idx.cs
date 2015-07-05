using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using u16=System.UInt16;
using i64 = System.Int64;

namespace Community.CsharpSqlite.Engine.Op
{
    using Community.CsharpSqlite.Engine;

    public static class Idx
    {
        public static RuntimeException Exec(CPU cpu, OpCode opcode, VdbeOp pOp)
        //public static RuntimeException Exec(ref int opcodeIndex, OpCode opcode, VdbeOp pOp, Mem pOut, Mem[] aMem, sqlite3 db, VdbeCursor[] openCursors, ref SqlResult rc)
        {
            var openCursors = cpu.vdbe.OpenCursors;
            var aMem=cpu.aMem;

            switch (opcode) {
                ///<summary>
                ///Opcode: IdxInsert P1 P2 P3 * P5
                ///
                ///Register P2 holds an SQL index key made using the
                ///MakeRecord instructions.  This opcode writes that key
                ///into the index P1.  Data for the entry is nil.
                ///
                ///</summary>
                ///<param name="P3 is a flag that provides a hint to the b">tree layer that this</param>
                ///<param name="insert is likely to be an append.">insert is likely to be an append.</param>
                ///<param name=""></param>
                ///<param name="This instruction only works for indices.  The equivalent instruction">This instruction only works for indices.  The equivalent instruction</param>
                ///<param name="for tables is  OpCode.OP_Insert.">for tables is  OpCode.OP_Insert.</param>
                ///<param name=""></param>
                case OpCode.OP_IdxInsert:
                    {
                        ///
                        ///<summary>
                        ///in2 
                        ///</summary>
                        VdbeCursor pC;
                        BtCursor pCrsr;
                        int nKey;
                        byte[] zKey;
                        //Debug.Assert(pOp.p1 >= 0 && pOp.p1 < this.nCursor);
                        pC = openCursors[pOp.p1];
                        Debug.Assert(pC != null);
                        var pIn2 = aMem[pOp.p2];
                        Debug.Assert((pIn2.flags & MemFlags.MEM_Blob) != 0);
                        pCrsr = pC.pCursor;
                        if (Sqlite3.ALWAYS(pCrsr != null))
                        {
                            Debug.Assert(!pC.isTable);
                            pIn2.ExpandBlob();
                            if (cpu.rc == SqlResult.SQLITE_OK)
                            {
                                nKey = pIn2.n;
                                zKey = (pIn2.flags & MemFlags.MEM_Blob) != 0 ? pIn2.zBLOB : Encoding.UTF8.GetBytes(pIn2.z);
                                cpu.rc = pCrsr.sqlite3BtreeInsert(zKey, nKey, null, 0, 0, (pOp.p3 != 0) ? 1 : 0, (((OpFlag)pOp.p5 & OpFlag.OPFLAG_USESEEKRESULT) != 0 ? pC.seekResult : 0));
                                Debug.Assert(!pC.deferredMoveto);
                                pC.cacheStatus = Sqlite3.CACHE_STALE;
                            }
                        }
                        break;
                    }
                ///
                ///<summary>
                ///Opcode: IdxDelete P1 P2 P3 * *
                ///
                ///The content of P3 registers starting at register P2 form
                ///an unpacked index key. This opcode removes that entry from the
                ///index opened by cursor P1.
                ///
                ///</summary>
                case OpCode.OP_IdxDelete:
                    {
                        VdbeCursor pC;
                        BtCursor pCrsr;
                        int res;
                        UnpackedRecord r;
                        res = 0;
                        r = new UnpackedRecord();
                        Debug.Assert(pOp.p3 > 0);
                        //Debug.Assert(pOp.p2 > 0 && pOp.p2 + pOp.p3 <= this.nMem + 1);
                        //Debug.Assert(pOp.p1 >= 0 && pOp.p1 < this.nCursor);
                        pC = openCursors[pOp.p1];
                        Debug.Assert(pC != null);
                        pCrsr = pC.pCursor;
                        if (Sqlite3.ALWAYS(pCrsr != null))
                        {
                            r.pKeyInfo = pC.pKeyInfo;
                            r.nField = (u16)pOp.p3;
                            r.flags = 0;
                            r.aMem = new Mem[r.nField];
                            for (int ra = 0; ra < r.nField; ra++)
                            {
                                r.aMem[ra] = aMem[pOp.p2 + ra];
#if SQLITE_DEBUG
																																																																																																																																																																																						                  Debug.Assert( memIsValid( r.aMem[ra] ) );
#endif
                            }
                            cpu.rc = pCrsr.sqlite3BtreeMovetoUnpacked(r, 0, 0, ref res);
                            if (cpu.rc == SqlResult.SQLITE_OK && res == 0)
                            {
                                cpu.rc = pCrsr.sqlite3BtreeDelete();
                            }
                            Debug.Assert(!pC.deferredMoveto);
                            pC.cacheStatus = Sqlite3.CACHE_STALE;
                        }
                        break;
                    }
                ///Opcode: IdxRowid P1 P2 * * *
                ///
                ///Write into register P2 an integer which is the last entry in the record at
                ///the end of the index key pointed to by cursor P1.  This integer should be
                ///the rowid of the table entry to which this index entry points.
                ///
                ///See also: Rowid, MakeRecord.
                case OpCode.OP_IdxRowid:
                    {
                        ///<param name="out2">prerelease </param>
                        BtCursor pCrsr;
                        VdbeCursor pC;
                        i64 rowid;
                        rowid = 0;
                        //Debug.Assert(pOp.p1 >= 0 && pOp.p1 < this.nCursor);
                        pC = openCursors[pOp.p1];
                        Debug.Assert(pC != null);
                        pCrsr = pC.pCursor;
                        cpu.pOut.flags = MemFlags.MEM_Null;
                        if (Sqlite3.ALWAYS(pCrsr != null))
                        {
                            cpu.rc = vdbeaux.sqlite3VdbeCursorMoveto(pC);
                            if (Sqlite3.NEVER(cpu.rc != 0))
                                return RuntimeException.abort_due_to_error;
                            Debug.Assert(!pC.deferredMoveto);
                            Debug.Assert(!pC.isTable);
                            if (!pC.nullRow)
                            {
                                cpu.rc = vdbeaux.sqlite3VdbeIdxRowid(cpu.db, pCrsr, ref rowid);
                                if (cpu.rc != SqlResult.SQLITE_OK)
                                {
                                    return RuntimeException.abort_due_to_error;
                                }
                                cpu.pOut.u.i = rowid;
                                cpu.pOut.flags = MemFlags.MEM_Int;
                            }
                        }
                        break;
                    }
                ///
                ///<summary>
                ///Opcode: IdxGE P1 P2 P3 P4 P5
                ///
                ///The P4 register values beginning with P3 form an unpacked index
                ///key that omits the ROWID.  Compare this key value against the index
                ///that P1 is currently pointing to, ignoring the ROWID on the P1 index.
                ///
                ///If the P1 index entry is greater than or equal to the key value
                ///then jump to P2.  Otherwise fall through to the next instruction.
                ///
                ///</summary>
                ///<param name="If P5 is non">zero then the key value is increased by an epsilon</param>
                ///<param name="prior to the comparison.  This make the opcode work like IdxGT except">prior to the comparison.  This make the opcode work like IdxGT except</param>
                ///<param name="that if the key from register P3 is a prefix of the key in the cursor,">that if the key from register P3 is a prefix of the key in the cursor,</param>
                ///<param name="the result is false whereas it would be true with IdxGT.">the result is false whereas it would be true with IdxGT.</param>
                ///<param name=""></param>
                ///
                ///<summary>
                ///Opcode: IdxLT P1 P2 P3 P4 P5
                ///
                ///The P4 register values beginning with P3 form an unpacked index
                ///key that omits the ROWID.  Compare this key value against the index
                ///that P1 is currently pointing to, ignoring the ROWID on the P1 index.
                ///
                ///If the P1 index entry is less than the key value then jump to P2.
                ///Otherwise fall through to the next instruction.
                ///
                ///</summary>
                ///<param name="If P5 is non">zero then the key value is increased by an epsilon prior</param>
                ///<param name="to the comparison.  This makes the opcode work like IdxLE.">to the comparison.  This makes the opcode work like IdxLE.</param>
                ///<param name=""></param>
                case OpCode.OP_IdxLT:
                ///
                ///<summary>
                ///jump 
                ///</summary>
                case OpCode.OP_IdxGE:
                    {
                        ///jump 
                        VdbeCursor pC;
                        int res;
                        UnpackedRecord r;
                        res = 0;
                        r = new UnpackedRecord();
                        //Debug.Assert(pOp.p1 >= 0 && pOp.p1 < this.nCursor);
                        pC = openCursors[pOp.p1];
                        Debug.Assert(pC != null);
                        Debug.Assert(pC.isOrdered);
                        if (Sqlite3.ALWAYS(pC.pCursor != null))
                        {
                            Debug.Assert(pC.deferredMoveto == false);
                            Debug.Assert(pOp.p5 == 0 || pOp.p5 == 1);
                            Debug.Assert(pOp.p4type == P4Usage.P4_INT32);
                            r.pKeyInfo = pC.pKeyInfo;
                            r.nField = (u16)pOp.p4.i;
                            if (pOp.p5 != 0)
                            {
                                r.flags = UnpackedRecordFlags.UNPACKED_INCRKEY | UnpackedRecordFlags.UNPACKED_IGNORE_ROWID;
                            }
                            else
                            {
                                r.flags = UnpackedRecordFlags.UNPACKED_IGNORE_ROWID;
                            }
                            r.aMem = new Mem[r.nField];
                            for (int rI = 0; rI < r.nField; rI++)
                            {
                                r.aMem[rI] = aMem[pOp.p3 + rI];
                                // r.aMem = aMem[pOp.p3];
#if SQLITE_DEBUG
																																																																																																																																																																																						                  Debug.Assert( memIsValid( r.aMem[rI] ) );
#endif
                            }
                            cpu.rc = vdbeaux.sqlite3VdbeIdxKeyCompare(pC, r, ref res);
                            if (pOp.OpCode == OpCode.OP_IdxLT)
                            {
                                res = -res;
                            }
                            else
                            {
                                Debug.Assert(pOp.OpCode == OpCode.OP_IdxGE);
                                res++;
                            }
                            if (res > 0)
                            {
                                cpu.opcodeIndex = pOp.p2 - 1;
                            }
                        }
                        break;
                    }
                default: return RuntimeException.noop;
            }
            return RuntimeException.OK;
        }
    }
}
