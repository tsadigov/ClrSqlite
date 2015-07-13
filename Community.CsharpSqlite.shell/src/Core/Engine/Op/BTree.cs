using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using u16 = System.UInt16;
using i64 = System.Int64;
using Community.CsharpSqlite.Engine;


namespace Community.CsharpSqlite.Engine.Op
{
    public static class BTree
    {
        public static RuntimeException Exec(CPU cpu,OpCode opcode,VdbeOp pOp)
        //(Community.CsharpSqlite.Vdbe vdbe, OpCode opcode, ref int opcodeIndex,Mem [] aMem,VdbeOp pOp,ref SqlResult rc)
        {
            var vdbe = cpu.vdbe;
            var aMem = vdbe.aMem;
            
            switch (opcode)
            {

                ///
                ///<summary>
                ///Opcode: SeekGe P1 P2 P3 P4 *
                ///
                ///</summary>
                ///<param name="If cursor P1 refers to an SQL table (B">Tree that uses integer keys),</param>
                ///<param name="use the value in register P3 as the key.  If cursor P1 refers">use the value in register P3 as the key.  If cursor P1 refers</param>
                ///<param name="to an SQL index, then P3 is the first in an array of P4 registers">to an SQL index, then P3 is the first in an array of P4 registers</param>
                ///<param name="that are used as an unpacked index key.">that are used as an unpacked index key.</param>
                ///<param name=""></param>
                ///<param name="Reposition cursor P1 so that  it points to the smallest entry that">Reposition cursor P1 so that  it points to the smallest entry that</param>
                ///<param name="is greater than or equal to the key value. If there are no records">is greater than or equal to the key value. If there are no records</param>
                ///<param name="greater than or equal to the key and P2 is not zero, then jump to P2.">greater than or equal to the key and P2 is not zero, then jump to P2.</param>
                ///<param name=""></param>
                ///<param name="See also: Found, NotFound, Distinct, SeekLt, SeekGt, SeekLe">See also: Found, NotFound, Distinct, SeekLt, SeekGt, SeekLe</param>
                ///<param name=""></param>
                ///
                ///<summary>
                ///Opcode: SeekGt P1 P2 P3 P4 *
                ///
                ///</summary>
                ///<param name="If cursor P1 refers to an SQL table (B">Tree that uses integer keys),</param>
                ///<param name="use the value in register P3 as a key. If cursor P1 refers">use the value in register P3 as a key. If cursor P1 refers</param>
                ///<param name="to an SQL index, then P3 is the first in an array of P4 registers">to an SQL index, then P3 is the first in an array of P4 registers</param>
                ///<param name="that are used as an unpacked index key.">that are used as an unpacked index key.</param>
                ///<param name=""></param>
                ///<param name="Reposition cursor P1 so that  it points to the smallest entry that">Reposition cursor P1 so that  it points to the smallest entry that</param>
                ///<param name="is greater than the key value. If there are no records greater than">is greater than the key value. If there are no records greater than</param>
                ///<param name="the key and P2 is not zero, then jump to P2.">the key and P2 is not zero, then jump to P2.</param>
                ///<param name=""></param>
                ///<param name="See also: Found, NotFound, Distinct, SeekLt, SeekGe, SeekLe">See also: Found, NotFound, Distinct, SeekLt, SeekGe, SeekLe</param>
                ///<param name=""></param>
                ///
                ///<summary>
                ///Opcode: SeekLt P1 P2 P3 P4 *
                ///
                ///</summary>
                ///<param name="If cursor P1 refers to an SQL table (B">Tree that uses integer keys),</param>
                ///<param name="use the value in register P3 as a key. If cursor P1 refers">use the value in register P3 as a key. If cursor P1 refers</param>
                ///<param name="to an SQL index, then P3 is the first in an array of P4 registers">to an SQL index, then P3 is the first in an array of P4 registers</param>
                ///<param name="that are used as an unpacked index key.">that are used as an unpacked index key.</param>
                ///<param name=""></param>
                ///<param name="Reposition cursor P1 so that  it points to the largest entry that">Reposition cursor P1 so that  it points to the largest entry that</param>
                ///<param name="is less than the key value. If there are no records less than">is less than the key value. If there are no records less than</param>
                ///<param name="the key and P2 is not zero, then jump to P2.">the key and P2 is not zero, then jump to P2.</param>
                ///<param name=""></param>
                ///<param name="See also: Found, NotFound, Distinct, SeekGt, SeekGe, SeekLe">See also: Found, NotFound, Distinct, SeekGt, SeekGe, SeekLe</param>
                ///<param name=""></param>
                ///
                ///<summary>
                ///Opcode: SeekLe P1 P2 P3 P4 *
                ///
                ///</summary>
                ///<param name="If cursor P1 refers to an SQL table (B">Tree that uses integer keys),</param>
                ///<param name="use the value in register P3 as a key. If cursor P1 refers">use the value in register P3 as a key. If cursor P1 refers</param>
                ///<param name="to an SQL index, then P3 is the first in an array of P4 registers">to an SQL index, then P3 is the first in an array of P4 registers</param>
                ///<param name="that are used as an unpacked index key.">that are used as an unpacked index key.</param>
                ///<param name=""></param>
                ///<param name="Reposition cursor P1 so that it points to the largest entry that">Reposition cursor P1 so that it points to the largest entry that</param>
                ///<param name="is less than or equal to the key value. If there are no records">is less than or equal to the key value. If there are no records</param>
                ///<param name="less than or equal to the key and P2 is not zero, then jump to P2.">less than or equal to the key and P2 is not zero, then jump to P2.</param>
                ///<param name=""></param>
                ///<param name="See also: Found, NotFound, Distinct, SeekGt, SeekGe, SeekLt">See also: Found, NotFound, Distinct, SeekGt, SeekGe, SeekLt</param>
                ///<param name=""></param>
                case OpCode.OP_SeekLt:
                ///jump, in3 
                case OpCode.OP_SeekLe:
                ///jump, in3 
                case OpCode.OP_SeekGe:
                ///jump, in3 
                case OpCode.OP_SeekGt:
                    {
                        ///jump, in3 
                        int res;
                        OpCode oc;
                        VdbeCursor pC;
                        UnpackedRecord r;
                        int nField;
                        i64 iKey;
                        ///
                        ///<summary>
                        ///The rowid we are to seek to 
                        ///</summary>
                        res = 0;
                        r = new UnpackedRecord();
                        Debug.Assert(pOp.p1 >= 0 && pOp.p1 < vdbe.nCursor);
                        Debug.Assert(pOp.p2 != 0);
                        pC = vdbe.OpenCursors[pOp.p1];
                        Debug.Assert(pC != null);
                        Debug.Assert(pC.pseudoTableReg == 0);
                        Debug.Assert(OpCode.OP_SeekLe == OpCode.OP_SeekLt + 1);
                        Debug.Assert(OpCode.OP_SeekGe == OpCode.OP_SeekLt + 2);
                        Debug.Assert(OpCode.OP_SeekGt == OpCode.OP_SeekLt + 3);
                        Debug.Assert(pC.isOrdered);
                        if (pC.pCursor != null)
                        {
                            oc = pOp.OpCode;
                            pC.nullRow = false;
                            if (pC.isTable)
                            {
                                ///The input value in P3 might be of any type: integer, real, string,
                                ///blob, or NULL.  But it needs to be an integer before we can do
                                ///the seek, so convert it. 
                                var pIn3 = aMem[pOp.p3];
                                pIn3.applyNumericAffinity();
                                iKey = pIn3.sqlite3VdbeIntValue();
                                pC.rowidIsValid = false;
                                ///If the P3 value could not be converted into an integer without
                                ///loss of information, then special processing is required... 
                                if ((pIn3.flags & MemFlags.MEM_Int) == 0)
                                {
                                    if ((pIn3.flags & MemFlags.MEM_Real) == 0)
                                    {
                                        ///If the P3 value cannot be converted into any kind of a number,
                                        ///then the seek is not possible, so jump to P2 
                                        cpu.opcodeIndex = pOp.p2 - 1;
                                        break;
                                    }
                                    ///If we reach vdbe point, then the P3 value must be a floating
                                    ///point number. 
                                    Debug.Assert((pIn3.flags & MemFlags.MEM_Real) != 0);
                                    if (iKey == IntegerExtensions.SMALLEST_INT64 && (pIn3.r < (double)iKey || pIn3.r > 0))
                                    {
                                        ///The P3 value is too large in magnitude to be expressed as an
                                        ///integer. 
                                        res = 1;
                                        if (pIn3.r < 0)
                                        {
                                            if (oc >= OpCode.OP_SeekGe)
                                            {
                                                Debug.Assert(oc == OpCode.OP_SeekGe || oc == OpCode.OP_SeekGt);
                                                cpu.rc = pC.pCursor.sqlite3BtreeFirst(ref res);
                                                if (cpu.rc != SqlResult.SQLITE_OK)
                                                    return RuntimeException.abort_due_to_error;
                                            }
                                        }
                                        else
                                        {
                                            if (oc <= OpCode.OP_SeekLe)
                                            {
                                                Debug.Assert(oc == OpCode.OP_SeekLt || oc == OpCode.OP_SeekLe);
                                                cpu.rc = pC.pCursor.sqlite3BtreeLast(ref res);
                                                if (cpu.rc != SqlResult.SQLITE_OK)
                                                    return RuntimeException.abort_due_to_error;
                                            }
                                        }
                                        if (res != 0)
                                        {
                                            cpu.opcodeIndex = pOp.p2 - 1;
                                        }
                                        break;
                                    }
                                    else
                                        if (oc == OpCode.OP_SeekLt || oc == OpCode.OP_SeekGe)
                                        {
                                            ///
                                            ///<summary>
                                            ///Use the ceiling() function to convert real.int 
                                            ///</summary>
                                            if (pIn3.r > (double)iKey)
                                                iKey++;
                                        }
                                        else
                                        {
                                            ///
                                            ///<summary>
                                            ///Use the floor() function to convert real.int 
                                            ///</summary>
                                            Debug.Assert(oc == OpCode.OP_SeekLe || oc == OpCode.OP_SeekGt);
                                            if (pIn3.r < (double)iKey)
                                                iKey--;
                                        }
                                }
                                cpu.rc = pC.pCursor.sqlite3BtreeMovetoUnpacked(null, iKey, 0, ref res);
                                if (cpu.rc != SqlResult.SQLITE_OK)
                                {
                                    return  RuntimeException.abort_due_to_error;
                                }
                                if (res == 0)
                                {
                                    pC.rowidIsValid = true;
                                    pC.lastRowid = iKey;
                                }
                            }
                            else
                            {
                                nField = pOp.p4.i;
                                Debug.Assert(pOp.p4type == P4Usage.P4_INT32);
                                Debug.Assert(nField > 0);
                                r.pKeyInfo = pC.pKeyInfo;
                                r.nField = (u16)nField;
                                ///
                                ///<summary>
                                ///The next line of code computes as follows, only faster:
                                ///if( oc== OpCode.OP_SeekGt || oc== OpCode.OP_SeekLe ){
                                ///r.flags = UnpackedRecordFlags.UNPACKED_INCRKEY;
                                ///}else{
                                ///r.flags = 0;
                                ///}
                                ///
                                
                                r.flags = (UnpackedRecordFlags)((int)UnpackedRecordFlags.UNPACKED_INCRKEY * (1 & (oc - OpCode.OP_SeekLt)));
                                Debug.Assert(oc != OpCode.OP_SeekGt || r.flags == UnpackedRecordFlags.UNPACKED_INCRKEY);
                                Debug.Assert(oc != OpCode.OP_SeekLe || r.flags == UnpackedRecordFlags.UNPACKED_INCRKEY);
                                Debug.Assert(oc != OpCode.OP_SeekGe || r.flags == 0);
                                Debug.Assert(oc != OpCode.OP_SeekLt || r.flags == 0);
                                r.aMem = new Mem[r.nField];
                                for (int rI = 0; rI < r.nField; rI++)
                                    r.aMem[rI] = aMem[pOp.p3 + rI];
                                // r.aMem = aMem[pOp.p3];
#if SQLITE_DEBUG
																																																																																																																																																																																						                  {
                    int i;
                    for ( i = 0; i < r.nField; i++ )
                      Debug.Assert( memIsValid( r.aMem[i] ) );
                  }
#endif
                                r.aMem[0].ExpandBlob();
                                cpu.rc = pC.pCursor.sqlite3BtreeMovetoUnpacked(r, 0, 0, ref res);
                                if (cpu.rc != SqlResult.SQLITE_OK)
                                {
                                    return RuntimeException.abort_due_to_error;
                                }
                                pC.rowidIsValid = false;
                            }
                            pC.deferredMoveto = false;
                            pC.cacheStatus = Sqlite3.CACHE_STALE;
#if SQLITE_TEST
#if !TCLSH
																																																																																																																																																													                sqlite3_search_count++;
#else
																																																																																																																																																													                sqlite3_search_count.iValue++;
#endif
#endif
                            if (oc >= OpCode.OP_SeekGe)
                            {
                                Debug.Assert(oc == OpCode.OP_SeekGe || oc == OpCode.OP_SeekGt);
                                if (res < 0 || (res == 0 && oc == OpCode.OP_SeekGt))
                                {
                                    cpu.rc = pC.pCursor.sqlite3BtreeNext(ref res);
                                    if (cpu.rc != SqlResult.SQLITE_OK)
                                        return RuntimeException.abort_due_to_error;
                                    pC.rowidIsValid = false;
                                }
                                else
                                {
                                    res = 0;
                                }
                            }
                            else
                            {
                                Debug.Assert(oc == OpCode.OP_SeekLt || oc == OpCode.OP_SeekLe);
                                if (res > 0 || (res == 0 && oc == OpCode.OP_SeekLt))
                                {
                                    cpu.rc = pC.pCursor.sqlite3BtreePrevious(ref res);
                                    if (cpu.rc != SqlResult.SQLITE_OK)
                                        return RuntimeException.abort_due_to_error;
                                    pC.rowidIsValid = false;
                                }
                                else
                                {
                                    ///
                                    ///<summary>
                                    ///res might be negative because the table is empty.  Check to
                                    ///see if vdbe is the case.
                                    ///
                                    ///</summary>
                                    res = pC.pCursor.sqlite3BtreeEof() ? 1 : 0;
                                }
                            }
                            Debug.Assert(pOp.p2 > 0);
                            if (res != 0)
                            {
                                cpu.opcodeIndex = pOp.p2 - 1;
                            }
                        }
                        else
                        {
                            ///
                            ///<summary>
                            ///This happens when attempting to open the sqlite3_master table
                            ///for read access returns SQLITE_EMPTY. In vdbe case always
                            ///take the jump (since there are no records in the table).
                            ///
                            ///</summary>
                            cpu.opcodeIndex = pOp.p2 - 1;
                        }
                        break;
                    }
                ///Opcode: Seek P1 P2 * * *
                ///
                ///P1 is an open table cursor and P2 is a rowid integer.  Arrange
                ///for P1 to move so that it points to the rowid given by P2.
                ///
                ///This is actually a deferred seek.  Nothing actually happens until
                ///the cursor is used to read a record.  That way, if no reads
                ///occur, no unnecessary I/O happens.
                case OpCode.OP_Seek:
                    {
                        ///in2 
                        VdbeCursor pC;
                        Debug.Assert(pOp.p1 >= 0 && pOp.p1 < vdbe.nCursor);
                        pC = vdbe.OpenCursors[pOp.p1];
                        Debug.Assert(Sqlite3.ALWAYS(pC != null));
                        if (pC.pCursor != null)
                        {
                            Debug.Assert(pC.isTable);
                            pC.nullRow = false;
                            var pIn2 = aMem[pOp.p2];
                            pC.movetoTarget = pIn2.sqlite3VdbeIntValue();
                            pC.rowidIsValid = false;
                            pC.deferredMoveto = true;
                        }
                        break;
                    }
                default: return RuntimeException.noop;
            }
            return RuntimeException.OK;
        }
    }
}
