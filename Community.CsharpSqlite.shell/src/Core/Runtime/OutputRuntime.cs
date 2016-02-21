using Community.CsharpSqlite.Engine;
using System;
using System.Diagnostics;
using System.Linq;
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
using Community.CsharpSqlite.Engine.Core.Runtime;
using Community.CsharpSqlite.Runtime;
using System.Collections.Generic;
using Community.CsharpSqlite.tree;
using Vdbe = Community.CsharpSqlite.Engine.Vdbe;
using Operation = Community.CsharpSqlite.Engine.VdbeOp;


namespace Community.CsharpSqlite.Core.Runtime
{
    public enum ColumnResult
    {
        abort_due_to_error,
        op_column_out,
        too_big

    }

    public class OutputRuntime : IRuntime
    {
        public CPU cpu { get; set; }
        public OutputRuntime()
        { }
        public OutputRuntime(CPU p)
        {
            this.cpu = p;
        }



        ///
        ///<summary>
        ///Opcode: Column P1 P2 P3 P4 *
        ///
        ///Interpret the data that cursor P1 points to as a structure built using
        ///the MakeRecord instruction.  (See the MakeRecord opcode for additional
        ///</summary>
        ///<param name="information about the format of the data.)  Extract the P2">th column</param>
        ///<param name="from this record.  If there are less that (P2+1)">from this record.  If there are less that (P2+1)</param>
        ///<param name="values in the record, extract a NULL.">values in the record, extract a NULL.</param>
        ///<param name=""></param>
        ///<param name="The value extracted is stored in register P3.">The value extracted is stored in register P3.</param>
        ///<param name=""></param>
        ///<param name="If the column contains fewer than P2 fields, then extract a NULL.  Or,">If the column contains fewer than P2 fields, then extract a NULL.  Or,</param>
        ///<param name="if the P4 argument is a  P4Usage.P4_MEM use the value of the P4 argument as">if the P4 argument is a  P4Usage.P4_MEM use the value of the P4 argument as</param>
        ///<param name="the result.">the result.</param>
        ///<param name=""></param>
        ///<param name="If the OPFLAG_CLEARCACHE bit is set on P5 and P1 is a pseudo">table cursor,</param>
        ///<param name="then the cache of the cursor is reset prior to extracting the column.">then the cache of the cursor is reset prior to extracting the column.</param>
        ///<param name="The first  OpCode.OP_Column against a pseudo">table after the value of the content</param>
        ///<param name="register has changed should have this bit set.">register has changed should have this bit set.</param>
        ///<param name=""></param>

        public RuntimeException Column(Operation pOp){
            this.OpCode_Column(pOp, cpu.rc, cpu.db, cpu.encoding, cpu.aMem);
            return RuntimeException.OK;
        }

        public SqlResult OpCode_Column(Operation pOp, SqlResult rc, Connection db, SqliteEncoding encoding, IList<Mem> aMem)
        {
            ///The length of the serialized data for the column 
            int len;
            ///Number of bytes in the record 
            u32 payloadSize = 0;
            ///Pointer to complete record
            byte[] zRecord = null;
            ///Number of bytes in the record : INDEX 
            i64 payloadSize64 = 0;

            ///P1 value of the opcode 
            ///
            int p1 = pOp.p1;
            Debug.Assert(p1 < cpu.vdbe.nCursor);

            ///column number to retrieve 
            int clumnNumber_p2 = pOp.p2;
            ///The VDBE cursor 
            VdbeCursor vdbeCursor = cpu.vdbe.OpenCursors[p1];
            ///This block sets the variable payloadSize to be the total number of
            ///bytes in the record.
            ///
            ///zRec is set to be the complete text of the record if it is available.
            ///The complete record text is always available for pseudo">tables</param>
            ///If the record is stored in a cursor, the complete record text
            ///might be available in the  pC.aRow cache.  Or it might not be.
            ///If the data is unavailable,  zRec is set to NULL.">If the data is unavailable,  zRec is set to NULL.</param>
            ///We also compute the number of columns in the record.  For cursors,
            ///the number of columns is stored in the VdbeCursor.nField element.

            ///number of fields in the record 
            int nField = vdbeCursor.FieldCount;

            ///The BTree cursor 
            BtCursor btCursor = vdbeCursor.pCursor;
            ///aType[i] holds the numeric type of the i"th column 
            u32[] columnTypes = vdbeCursor.aType;


            ///Loop counter 
            int i;
            ///Part of the record being decoded 
            byte[] zData = null;
            ///Where to write the extracted value 
            Debug.Assert(pOp.p3 > 0 && pOp.p3 <= cpu.aMem.Count());
            Mem pDest = aMem[pOp.p3];
            ///For storing the record being decoded 
            Mem sMem = null;
            sMem = malloc_cs.sqlite3Malloc(sMem);


            ///Index into header 
            int idxHeader;
            ///Pointer to first byte after the header 
            int idxEndHeader;
            ///Offset into the data 
            u32 offsetIntoData = 0;
            ///Number of bytes in the content of a field 
            u32 szField = 0;

            ///Number of bytes of available data 
            int avail;
            ///PseudoTable input register 
            Mem pReg;


            //  memset(&sMem, 0, sizeof(sMem));

            cpu.vdbe.memAboutToChange(pDest);
            pDest.MemSetTypeFlag(MemFlags.MEM_Null);

            Debug.Assert(vdbeCursor != null);
#if !SQLITE_OMIT_VIRTUALTABLE
            Debug.Assert(vdbeCursor.pVtabCursor == null);
#endif

            #region payload size
            if (btCursor != null)
            {
                ///The record is stored in a B">Tree
                rc = vdbeaux.sqlite3VdbeCursorMoveto(vdbeCursor);
                if (rc != 0)
                    return (int)ColumnResult.abort_due_to_error;
                //goto  abort_due_to_error;
                if (vdbeCursor.nullRow)
                {
                    payloadSize = 0;
                }
                else
                {
                    if ((vdbeCursor.cacheStatus == cpu.vdbe.cacheCtr) && (vdbeCursor.aRow != -1))
                    {
                        payloadSize = vdbeCursor.payloadSize;
                        zRecord = malloc_cs.sqlite3Malloc((int)payloadSize);
                        Buffer.BlockCopy(btCursor.info.pCell, vdbeCursor.aRow, zRecord, 0, (int)payloadSize);
                    }
                    else
                    {
                        if (vdbeCursor.isIndex)
                        {
                            Debug.Assert(btCursor.sqlite3BtreeCursorIsValid());
                            rc = btCursor.sqlite3BtreeKeySize(ref payloadSize64);
                            Debug.Assert(rc == SqlResult.SQLITE_OK);
                            ///True because of CursorMoveto() call above 
                            ///sqlite3BtreeParseCellPtr() uses utilc.getVarint32() to extract the
                            ///payload size, so it is impossible for payloadSize64 to be
                            ///larger than 32 bits. 
                            Debug.Assert(((u64)payloadSize64 & sqliteinth.SQLITE_MAX_U32) == (u64)payloadSize64);
                            payloadSize = (u32)payloadSize64;
                        }
                        else
                        {
                            Debug.Assert(btCursor.sqlite3BtreeCursorIsValid());
                            rc = btCursor.sqlite3BtreeDataSize(ref payloadSize);
                            Debug.Assert(rc == SqlResult.SQLITE_OK);
                            ///
                            ///<summary>
                            ///DataSize() cannot fail 
                            ///</summary>
                        }
                    }
                }
            }
            else
            {
                if (vdbeCursor.pseudoTableReg > 0)
                {
                    ///The record is the sole entry of a pseudo">table
                    pReg = aMem[vdbeCursor.pseudoTableReg];
                    Debug.Assert((pReg.flags & MemFlags.MEM_Blob) != 0);
                    Debug.Assert(pReg.memIsValid());
                    payloadSize = (u32)pReg.CharacterCount;
                    zRecord = pReg.zBLOB;
                    vdbeCursor.cacheStatus = ((OpFlag)pOp.p5 & OpFlag.OPFLAG_CLEARCACHE) != 0 ? Sqlite3.CACHE_STALE : cpu.vdbe.cacheCtr;
                    Debug.Assert(payloadSize == 0 || zRecord != null);
                }
                else
                {
                    ///Consider the row to be NULL 
                    payloadSize = 0;
                }
            }

            #endregion

            ///If payloadSize is 0, then just store a NULL 
            if (payloadSize == 0)
            {
                Debug.Assert((pDest.flags & MemFlags.MEM_Null) != 0);
                return (SqlResult)ColumnResult.op_column_out;
            }
            Debug.Assert(db.aLimit[Globals.SQLITE_LIMIT_LENGTH] >= 0);
            if (payloadSize > (u32)db.aLimit[Globals.SQLITE_LIMIT_LENGTH])
            {
                return (SqlResult)ColumnResult.too_big;
            }


            Debug.Assert(clumnNumber_p2 < nField);

            //aOffset[i] is offset to start of data for i">th column
            u32[] clumnOffsets;

            ///Read and parse the table header.  Store the results of the parse
            ///into the record header cache fields of the cursor.

            if (vdbeCursor.cacheStatus == cpu.vdbe.cacheCtr)
            {
                clumnOffsets = vdbeCursor.aOffset;
            }
            else
            {
                Debug.Assert(columnTypes != null);
                avail = 0;
                //pC.aOffset = aOffset = aType[nField];
                clumnOffsets = new u32[nField];
                vdbeCursor.aOffset = clumnOffsets;
                vdbeCursor.payloadSize = payloadSize;
                vdbeCursor.cacheStatus = cpu.vdbe.cacheCtr;
                ///Figure out how many bytes are in the header 
                if (zRecord != null)
                {
                    zData = zRecord;
                }
                else
                {
                    if (vdbeCursor.isIndex)
                    {
                        zData = btCursor.KeyFetch(ref avail, ref vdbeCursor.aRow);
                    }
                    else
                    {
                        zData = btCursor.DataFetch(ref avail, ref vdbeCursor.aRow);
                    }
                    ///If KeyFetch()/DataFetch() managed to get the entire payload,
                    ///save the payload in the pC.aRow cache.  That will save us from
                    ///having to make additional calls to fetch the content portion of
                    ///the record.
                    Debug.Assert(avail >= 0);
                    if (payloadSize <= (u32)avail)
                    {
                        zRecord = zData;
                        //vdbeCursor.aRow = zData;
                    }
                    else
                    {
                        vdbeCursor.aRow = -1;
                        //pC.aRow = null;
                    }
                }
                ///The following Debug.Assert is true in all cases accept when
                ///the database file has been corrupted externally.
                ///Debug.Assert( zRec!=0 || avail>=payloadSize || avail>=9 );
                ///
                ///Size of the header size field at start of record 
                int szHdr;
                szHdr = utilc.getVarint32(zData, out offsetIntoData);
                ///Make sure a corrupt database has not given us an oversize header.
                ///Do this now to avoid an oversize memory allocation.
                ///
                ///Type entries can be between 1 and 5 bytes each.  But 4 and 5 byte
                ///types use so much data space that there can only be 4096 and 32 of
                ///them, respectively.  So the maximum header length results from a
                ///3">byte type for each of the maximum of 32768 columns plus three</param>
                ///extra bytes for the header length itself.  32768*3 + 3 = 98307.">extra bytes for the header length itself.  32768*3 + 3 = 98307.</param>
                ///"></param>
                if (offsetIntoData > 98307)
                {
                    rc = sqliteinth.SQLITE_CORRUPT_BKPT();
                    return (SqlResult)ColumnResult.op_column_out;
                }
                ///Compute in len the number of bytes of data we need to read in order
                ///to get nField type values.  offset is an upper bound on this.  But
                ///nField might be significantly less than the true number of columns
                ///in the table, and in that case, 5*nField+3 might be smaller than offset.
                ///We want to minimize len in order to limit the size of the memory
                ///allocation, especially if a corrupt database file has caused offset
                ///to be oversized. Offset is limited to 98307 above.  But 98307 might
                ///still exceed Robson memory allocation limits on some configurations.
                ///On systems that cannot tolerate large memory allocations, nField*5+3
                ///will likely be much smaller since nField will likely be less than
                ///20 or so.  This insures that Robson memory allocation limits are
                ///not exceeded even for corrupt database files.
                len = nField * 5 + 3;
                if (len > (int)offsetIntoData)
                    len = (int)offsetIntoData;
                ///The KeyFetch() or DataFetch() above are fast and will get the entire
                ///record header in most cases.  But they will fail to get the complete
                ///record header if the record header does not fit on a single page
                ///in the B"Tree.  When that happens, use sqlite3VdbeMemFromBtree() to
                ///acquire the complete header text.
                if (zRecord == null && avail < len)
                {
                    sMem.db = null;
                    sMem.flags = 0;
                    rc = vdbemem_cs.sqlite3VdbeMemFromBtree(btCursor, 0, len, vdbeCursor.isIndex, sMem);
                    if (rc != SqlResult.SQLITE_OK)
                    {
                        return (SqlResult)ColumnResult.op_column_out;
                    }
                    zData = sMem.zBLOB;
                }
                idxEndHeader = len;
                // zData[len];
                idxHeader = szHdr;
                // zData[szHdr];
                ///
                ///Scan the header and use it to fill in the aType[] and aOffset[]
                ///arrays.  aType[i] will contain the type integer for the i'th
                ///column and aOffset[i] will contain the offset from the beginning
                ///of the record to the start of the data for the i"th column
                ///
                ///Tural: zData 
                ///[0]          : number of columns
                ///[1-num]      : type of the column
                ///[num,2*num]  : data of the column
                for (i = 0; i < nField; i++)
                {
                    if (idxHeader < idxEndHeader)
                    {
                        clumnOffsets[i] = offsetIntoData;
                        idxHeader += utilc.getVarint32(zData, idxHeader, out columnTypes[i]);
                        //utilc.getVarint32(zIdx, aType[i]);
                        szField = vdbeaux.sqlite3VdbeSerialTypeLen(columnTypes[i]);
                        offsetIntoData += szField;
                        if (offsetIntoData < szField)
                        {
                            ///True if offset overflows 
                            idxHeader = int.MaxValue;
                            ///Forces SQLITE_CORRUPT return below 
                            break;
                        }
                    }
                    else
                    {
                        ///If i is less that nField, then there are less fields in this
                        ///record than SetNumColumns indicated there are columns in the
                        ///table. Set the offset for any extra columns not present in
                        ///the record to 0. This tells code below to store a NULL
                        ///instead of deserializing a value from the record.
                        clumnOffsets[i] = 0;
                    }
                }
                sMem.Release();
                sMem.flags = MemFlags.MEM_Null;
                ///If we have read more header data than was contained in the header,
                ///or if the end of the last field appears to be past the end of the
                ///record, or if the end of the last field appears to be before the end
                ///of the record (when all fields present), then we must be dealing
                ///with a corrupt database.
                if ((idxHeader > idxEndHeader) || (offsetIntoData > payloadSize) || (idxHeader == idxEndHeader && offsetIntoData != payloadSize))
                {
                    rc = sqliteinth.SQLITE_CORRUPT_BKPT();
                    return (SqlResult)ColumnResult.op_column_out;
                }
            }

            ///Get the column information. If aOffset[p2] is non"zero, then
            ///deserialize the value from the record. If aOffset[p2] is zero,
            ///then there are not enough fields in the record to satisfy the">then there are not enough fields in the record to satisfy the</param>
            ///request.  In this case, set the value NULL or to P4 if P4 is">request.  In this case, set the value NULL or to P4 if P4 is</param>
            ///a pointer to a Mem object.
            if (clumnOffsets[clumnNumber_p2] != 0)
            {
                Debug.Assert(rc == SqlResult.SQLITE_OK);
                if (zRecord != null)
                {
                    pDest.sqlite3VdbeMemReleaseExternal();
                    vdbeaux.sqlite3VdbeSerialGet(zRecord, (int)clumnOffsets[clumnNumber_p2], columnTypes[clumnNumber_p2], pDest);
                }
                else
                {
                    len = (int)vdbeaux.sqlite3VdbeSerialTypeLen(columnTypes[clumnNumber_p2]);
                    vdbemem_cs.sqlite3VdbeMemMove(sMem, pDest);
                    rc = vdbemem_cs.sqlite3VdbeMemFromBtree(btCursor, (int)clumnOffsets[clumnNumber_p2], len, vdbeCursor.isIndex, sMem);
                    if (rc != SqlResult.SQLITE_OK)
                    {
                        return (SqlResult)ColumnResult.op_column_out;
                    }
                    zData = sMem.zBLOB;
                    sMem.zBLOB = null;
                    vdbeaux.sqlite3VdbeSerialGet(zData, columnTypes[clumnNumber_p2], pDest);
                }
                pDest.enc = encoding;
            }
            else
            {
                if (pOp.p4type == P4Usage.P4_MEM)
                {
                    vdbemem_cs.sqlite3VdbeMemShallowCopy(pDest, pOp.p4.pMem, MemFlags.MEM_Static);
                }
                else
                {
                    Debug.Assert((pDest.flags & MemFlags.MEM_Null) != 0);
                }
            }
            ///If we dynamically allocated space to hold the data (in the
            ///sqlite3VdbeMemFromBtree() call above) then transfer control of that
            ///dynamically allocated space over to the pDest structure.
            ///This prevents a memory copy.
            //if ( sMem.zMalloc != null )
            //{
            //  Debug.Assert( sMem.z == sMem.zMalloc);
            //  Debug.Assert( sMem.xDel == null );
            //  Debug.Assert( ( pDest.flags & MEM.MEM_Dyn ) == 0 );
            //  Debug.Assert( ( pDest.flags & ( MEM.MEM_Blob | MEM.MEM_Str ) ) == 0 || pDest.z == sMem.z );
            //  pDest.flags &= ~( MEM.MEM_Ephem | MEM.MEM_Static );
            //  pDest.flags |= MEM.MEM_Term;
            //  pDest.z = sMem.z;
            //  pDest.zMalloc = sMem.zMalloc;
            //}
            rc = pDest.MakeWriteable();
            op_column_out:
#if SQLITE_TEST
																																																																																																																																				              UPDATE_MAX_BLOBSIZE( pDest );
#endif
            Sqlite3.REGISTER_TRACE(cpu.vdbe, pOp.p3, pDest);
            if (zData != null && zData != zRecord)
                malloc_cs.sqlite3_free(ref zData);
            //malloc_cs.sqlite3_free( ref zRec );
            malloc_cs.sqlite3_free(ref sMem);
            return rc;
        }
        
        ///
        ///<summary>
        ///Opcode: ResultRow P1 P2 * * *
        ///
        ///</summary>
        ///<param name="The registers P1 through P1+P2">1 contain a single row of</param>
        ///results. This opcode causes the sqlite3_step() call to terminate</param>
        ///with an SQLITE_ROW return code and it sets up the sqlite3_stmt</param>
        ///structure to provide access to the top P1 values as the result</param>
        ///<param name="row.">row.</param>
        ///<param name=""></param>
        public RuntimeException ResultRow(VdbeOp pOp)
        {
            var vdbe = this.cpu.vdbe;
            Debug.Assert(vdbe.nResColumn == pOp.p2);
            Debug.Assert(pOp.p1 > 0);
            Debug.Assert(pOp.p1 + pOp.p2 <= this.cpu.aMem.Count() + 1);

            vdbe.rc=OpCode_ResultRow(cpu.opcodeIndex, pOp.p1, pOp.p2, this.cpu.rc, vdbe.aMem);

            return RuntimeException.vdbe_return;
        }
        private SqlResult OpCode_ResultRow(int opcodeIndex, int dataOffset, int columnCount, SqlResult rc, IList<Mem> memoryBuffer)
        {
            var vdbe = this.cpu as Vdbe;

            ///If this statement has violated immediate foreign key constraints, do
            ///not return the number of rows modified. And do not RELEASE the statement
            ///transaction. It needs to be rolled back.  
            if (SqlResult.SQLITE_OK != (rc = vdbe.sqlite3VdbeCheckFk(0)))
            {
                Debug.Assert((cpu.db.flags & SqliteFlags.SQLITE_CountRows) != 0);
                Debug.Assert(vdbe.usesStmtJournal);
                return rc;
            }
            ///If the SQLITE_CountRows flag is set in sqlite3.flags mask, then
            ///DML statements invoke this opcode to return the number of rows
            ///modified to the user. This is the only way that a VM that
            ///opens a statement transaction may invoke this opcode.
            ///
            ///In case this is such a statement, close any statement transaction
            ///opened by this VM before returning control to the user. This is to
            ///<param name="ensure that statement">transactions are always nested, not overlapping.</param>
            ///<param name="If the open statement">transaction is not closed here, then the user</param>
            ///<param name="may step another VM that opens its own statement transaction. This">may step another VM that opens its own statement transaction. This</param>
            ///<param name="may lead to overlapping statement transactions.">may lead to overlapping statement transactions.</param>
            ///<param name=""></param>
            ///<param name="The statement transaction is never a top">level transaction.  Hence</param>
            ///<param name="the RELEASE call below can never fail.">the RELEASE call below can never fail.</param>
            ///<param name=""></param>
            Debug.Assert(vdbe.iStatement == 0 || (cpu.db.flags & SqliteFlags.SQLITE_CountRows) != 0);
            rc = vdbe.sqlite3VdbeCloseStatement(sqliteinth.SAVEPOINT_RELEASE);
            if (Sqlite3.NEVER(rc != SqlResult.SQLITE_OK))
            {
                return rc;
            }
            ///Invalidate all ephemeral cursor row caches 
            vdbe.cacheCtr = (vdbe.cacheCtr + 2) | 1;
            ///Make sure the results of the current row are \000 terminated
            ///and have an assigned type.  The results are de-ephemeralized as
            ///as side effect.

            //pMem = p.pResultSet = aMem[pOp.p1];
            vdbe.pResultSet = memoryBuffer.Skip(dataOffset).Take(columnCount).ToArray();
            vdbe.pResultSet.ForEach(
                (mem) => {
                    Debug.Assert(mem.memIsValid());
                    vdbemem_cs.sqlite3VdbeMemNulTerminate(mem);
                    Sqlite3.sqlite3VdbeMemStoreType(mem);
                    //Sqlite3.REGISTER_TRACE(this, dataOffset + i, this.pResultSet[i]);
                }
            );

            ///Return SQLITE_ROW
            vdbe.currentOpCodeIndex = opcodeIndex + 1;
            rc = SqlResult.SQLITE_ROW;
            return rc;
        }
    }
}
