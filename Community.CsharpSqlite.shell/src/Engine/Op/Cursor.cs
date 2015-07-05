using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Community.CsharpSqlite.Engine.Op
{
    using Btree = Sqlite3.Btree;

    using Community.CsharpSqlite.Engine;
    using Metadata;
    public static class Cursor
    {
        public static RuntimeException Exec(CPU cpu,OpCode opcode,VdbeOp pOp)

        //public static RuntimeException Exec(CPU cpu, VdbeOp pOp, OpCode opcode)
        {

            var vdbe = cpu.vdbe;
            switch (opcode)
            {
                ///Opcode: OpenRead P1 P2 P3 P4 P5
                ///
                ///Open a read-only cursor for the database table whose root page is</param>
                ///P2 in a database file.  The database file is determined by P3.">P2 in a database file.  The database file is determined by P3.</param>
                ///P3==0 means the main database, P3==1 means the database used for</param>
                ///temporary tables, and P3>1 means used the corresponding attached</param>
                ///database.  Give the new cursor an identifier of P1.  The P1</param>
                ///values need not be contiguous but all P1 values should be small integers.</param>
                ///It is an error for P1 to be negative.">It is an error for P1 to be negative.</param>
                ///
                ///If P5!=0 then use the content of register P2 as the root page, not</param>
                ///the value of P2 itself.</param>
                ///
                ///There will be a read lock on the database whenever there is an</param>
                ///open cursor.  If the database was unlocked prior to vdbe instruction</param>
                ///then a read lock is acquired as part of vdbe instruction.  A read</param>
                ///lock allows other processes to read the database but prohibits</param>
                ///any other process from modifying the database.  The read lock is</param>
                ///released when all cursors are closed.  If vdbe instruction attempts</param>
                ///to get a read lock but fails, the script terminates with an</param>
                ///SQLITE_BUSY error code.</param>
                ///
                ///The P4 value may be either an integer ( P4Usage.P4_INT32) or a pointer to</param>
                ///a KeyInfo structure ( P4Usage.P4_KEYINFO). If it is a pointer to a KeyInfo</param>
                ///structure, then said structure defines the content and collating</param>
                ///sequence of the index being opened. Otherwise, if P4 is an integer</param>
                ///value, it is set to the number of columns in the table.</param>
                ///
                ///See also OpenWrite.</param>
                ///						///
                ///<summary>
                ///Opcode: OpenWrite P1 P2 P3 P4 P5
                ///
                ///Open a read/write cursor named P1 on the table or index whose root
                ///page is P2.  Or if P5!=0 use the content of register P2 to find the
                ///root page.
                ///
                ///The P4 value may be either an integer ( P4Usage.P4_INT32) or a pointer to
                ///a KeyInfo structure ( P4Usage.P4_KEYINFO). If it is a pointer to a KeyInfo
                ///structure, then said structure defines the content and collating
                ///sequence of the index being opened. Otherwise, if P4 is an integer
                ///value, it is set to the number of columns in the table, or to the
                ///largest index of any column of the table that is actually used.
                ///
                ///This instruction works just like OpenRead except that it opens the cursor
                ///</summary>
                ///<param name="in read/write mode.  For a given table, there can be one or more read">only</param>
                ///<param name="cursors or a single read/write cursor but not both.">cursors or a single read/write cursor but not both.</param>
                ///<param name=""></param>
                ///<param name="See also OpenRead.">See also OpenRead.</param>
                ///<param name=""></param>
                case OpCode.OP_OpenRead:
                case OpCode.OP_OpenWrite:
                    {
                        int nField;
                        KeyInfo pKeyInfo;
                        int wrFlag;
                        if (cpu.vdbe.expired)
                        {
                            cpu.rc = SqlResult.SQLITE_ABORT;
                            break;
                        }
                        nField = 0;
                        pKeyInfo = null;
                        var p2 = pOp.p2;
                        var iDb = pOp.p3;
                        Debug.Assert(iDb >= 0 && iDb < cpu.db.nDb);
                        //Debug.Assert((vdbe.btreeMask & (((yDbMask)1) << iDb)) != 0);
                        var pDb = cpu.db.aDb[iDb];
                        var pX = pDb.pBt;
                        Debug.Assert(pX != null);
                        if (pOp.OpCode == OpCode.OP_OpenWrite)
                        {
                            wrFlag = 1;
                            Debug.Assert(Sqlite3.sqlite3SchemaMutexHeld(cpu.db, iDb, null));
                            if (pDb.pSchema.file_format < cpu.vdbe.minWriteFileFormat)
                            {
                                cpu.vdbe.minWriteFileFormat = pDb.pSchema.file_format;
                            }
                        }
                        else
                        {
                            wrFlag = 0;
                        }
                        if (pOp.p5 != 0)
                        {
                            Debug.Assert(p2 > 0);
                            Debug.Assert(p2 <= cpu.vdbe.nMem);
                            var pIn2 = cpu.aMem[p2];
                            Debug.Assert(pIn2.memIsValid());
                            Debug.Assert((pIn2.flags & MemFlags.MEM_Int) != 0);
                            pIn2.sqlite3VdbeMemIntegerify();
                            p2 = (int)pIn2.u.i;
                            ///The p2 value always comes from a prior  OpCode.OP_CreateTable opcode and
                            ///that opcode will always set the p2 value to 2 or more or else fail.
                            ///If there were a failure, the prepared statement would have halted
                            ///before reaching vdbe instruction. 
                            if (Sqlite3.NEVER(p2 < 2))
                            {
                                cpu.rc = sqliteinth.SQLITE_CORRUPT_BKPT();
                                return RuntimeException.abort_due_to_error;
                            }
                        }
                        if (pOp.p4type == P4Usage.P4_KEYINFO)
                        {
                            pKeyInfo = pOp.p4.pKeyInfo;
                            pKeyInfo.enc = sqliteinth.ENC(cpu.vdbe.db);
                            nField = pKeyInfo.nField + 1;
                        }
                        else
                            if (pOp.p4type == P4Usage.P4_INT32)
                            {
                                nField = pOp.p4.i;
                            }
                        Debug.Assert(pOp.p1 >= 0);
                        var pCur = Sqlite3.allocateCursor(cpu.vdbe, pOp.p1, nField, iDb, 1);
                        if (pCur == null)
                            return RuntimeException.no_mem;
                        pCur.nullRow = true;
                        pCur.isOrdered = true;
                        cpu.rc = pX.sqlite3BtreeCursor(p2, wrFlag, pKeyInfo, pCur.pCursor);
                        pCur.pKeyInfo = pKeyInfo;
                        ///Since it performs no memory allocation or IO, the only values that
                        ///sqlite3BtreeCursor() may return are SQLITE_EMPTY and SqlResult.SQLITE_OK. 
                        ///SQLITE_EMPTY is only returned when attempting to open the table
                        ///<param name="rooted at page 1 of a zero">byte database.  </param>
                        Debug.Assert(cpu.rc == SqlResult.SQLITE_EMPTY || cpu.rc == SqlResult.SQLITE_OK);
                        if (cpu.rc == SqlResult.SQLITE_EMPTY)
                        {
                            mempoolMethods.sqlite3MemFreeBtCursor(ref pCur.pCursor);
                            cpu.rc = SqlResult.SQLITE_OK;
                        }
                        ///Set the VdbeCursor.isTable and isIndex variables. Previous versions of
                        ///</summary>
                        ///<param name="SQLite used to check if the root">page flags were sane at vdbe point</param>
                        ///<param name="and report database corruption if they were not, but vdbe check has">and report database corruption if they were not, but vdbe check has</param>
                        ///<param name="since moved into the btree layer.  ">since moved into the btree layer.  </param>
                        pCur.isTable = pOp.p4type != P4Usage.P4_KEYINFO;
                        pCur.isIndex = !pCur.isTable;
                        break;
                    }
                ///
                ///<summary>
                ///Opcode: OpenEphemeral P1 P2 * P4 *
                ///
                ///Open a new cursor P1 to a transient table.
                ///The cursor is always opened read/write even if 
                ///the main database is read">only.  The ephemeral</param>
                ///table is deleted automatically when the cursor is closed.">table is deleted automatically when the cursor is closed.</param>
                ///"></param>
                ///P2 is the number of columns in the ephemeral table.">P2 is the number of columns in the ephemeral table.</param>
                ///The cursor points to a BTree table if P4==0 and to a BTree index">The cursor points to a BTree table if P4==0 and to a BTree index</param>
                ///if P4 is not 0.  If P4 is not NULL, it points to a KeyInfo structure">if P4 is not 0.  If P4 is not NULL, it points to a KeyInfo structure</param>
                ///that defines the format of keys in the index.">that defines the format of keys in the index.</param>
                ///"></param>
                ///This opcode was once called OpenTemp.  But that created">This opcode was once called OpenTemp.  But that created</param>
                ///confusion because the term "temp table", might refer either">confusion because the term "temp table", might refer either</param>
                ///to a TEMP table at the SQL level, or to a table opened by">to a TEMP table at the SQL level, or to a table opened by</param>
                ///vdbe opcode.  Then vdbe opcode was call OpenVirtual.  But">vdbe opcode.  Then vdbe opcode was call OpenVirtual.  But</param>
                ///that created confusion with the whole virtual">table idea.</param>
                ///"></param>
                ///
                ///Opcode: OpenAutoindex P1 P2 * P4 *
                ///
                ///This opcode works the same as  OpCode.OP_OpenEphemeral.  It has a
                ///different name to distinguish its use.  Tables created using
                ///by vdbe opcode will be used for automatically created transient
                ///indices in joins.
                ///
                ///</summary>
                case OpCode.OP_OpenAutoindex:
                case OpCode.OP_OpenEphemeral:
                    {
                        VdbeCursor pCx;
                        const int vfsFlags = Sqlite3.SQLITE_OPEN_READWRITE | Sqlite3.SQLITE_OPEN_CREATE | Sqlite3.SQLITE_OPEN_EXCLUSIVE | Sqlite3.SQLITE_OPEN_DELETEONCLOSE | Sqlite3.SQLITE_OPEN_TRANSIENT_DB;
                        Debug.Assert(pOp.p1 >= 0);
                        pCx = Sqlite3.allocateCursor(cpu.vdbe, pOp.p1, pOp.p2, -1, 1);
                        if (pCx == null)
                            return RuntimeException.no_mem;
                        pCx.nullRow = true;
                        cpu.rc = Btree.Open(cpu.db.pVfs, null, cpu.db, ref pCx.pBt, Sqlite3.BTREE_OMIT_JOURNAL | Sqlite3.BTREE_SINGLE | pOp.p5, vfsFlags);
                        if (cpu.rc == SqlResult.SQLITE_OK)
                        {
                            cpu.rc = pCx.pBt.sqlite3BtreeBeginTrans(1);
                        }
                        if (cpu.rc == SqlResult.SQLITE_OK)
                        {
                            ///
                            ///<summary>
                            ///If a transient index is required, create it by calling
                            ///sqlite3BtreeCreateTable() with the BTREE_BLOBKEY flag before
                            ///opening it. If a transient table is required, just use the
                            ///</summary>
                            ///<param name="automatically created table with root">page 1 (an BLOB_INTKEY table).</param>
                            ///<param name=""></param>
                            if (pOp.p4.pKeyInfo != null)
                            {
                                int pgno = 0;
                                Debug.Assert(pOp.p4type == P4Usage.P4_KEYINFO);
                                cpu.rc = pCx.pBt.sqlite3BtreeCreateTable(ref pgno, Sqlite3.BTREE_BLOBKEY);
                                if (cpu.rc == SqlResult.SQLITE_OK)
                                {
                                    Debug.Assert(pgno == sqliteinth.MASTER_ROOT + 1);
                                   cpu.rc = pCx.pBt.sqlite3BtreeCursor(pgno, 1, pOp.p4.pKeyInfo, pCx.pCursor);
                                    pCx.pKeyInfo = pOp.p4.pKeyInfo;
                                    pCx.pKeyInfo.enc = sqliteinth.ENC(cpu.vdbe.db);
                                }
                                pCx.isTable = false;
                            }
                            else
                            {
                                cpu.rc = pCx.pBt.sqlite3BtreeCursor(sqliteinth.MASTER_ROOT, 1, null, pCx.pCursor);
                                pCx.isTable = true;
                            }
                        }
                        pCx.isOrdered = (pOp.p5 != Sqlite3.BTREE_UNORDERED);
                        pCx.isIndex = !pCx.isTable;
                        break;
                    }
                ///
                ///<summary>
                ///Opcode: OpenPseudo P1 P2 P3 * *
                ///
                ///Open a new cursor that points to a fake table that contains a single
                ///row of data.  The content of that one row in the content of memory
                ///register P2.  In other words, cursor P1 becomes an alias for the 
                ///MEM.MEM_Blob content contained in register P2.
                ///
                ///</summary>
                ///<param name="A pseudo">table created by vdbe opcode is used to hold a single</param>
                ///<param name="row output from the sorter so that the row can be decomposed into">row output from the sorter so that the row can be decomposed into</param>
                ///<param name="individual columns using the  OpCode.OP_Column opcode.  The  OpCode.OP_Column opcode">individual columns using the  OpCode.OP_Column opcode.  The  OpCode.OP_Column opcode</param>
                ///<param name="is the only cursor opcode that works with a pseudo">table.</param>
                ///<param name=""></param>
                ///<param name="P3 is the number of fields in the records that will be stored by">P3 is the number of fields in the records that will be stored by</param>
                ///<param name="the pseudo">table.</param>
                ///<param name=""></param>
                case OpCode.OP_OpenPseudo:
                    {
                        VdbeCursor pCx;
                        Debug.Assert(pOp.p1 >= 0);
                        pCx = Sqlite3.allocateCursor(cpu.vdbe, pOp.p1, pOp.p3, -1, 0);
                        if (pCx == null)
                            return RuntimeException.no_mem;
                        pCx.nullRow = true;
                        pCx.pseudoTableReg = pOp.p2;
                        pCx.isTable = true;
                        pCx.isIndex = false;
                        break;
                    }
                ///
                ///<summary>
                ///Opcode: Close P1 * * * *
                ///
                ///Close a cursor previously opened as P1.  If P1 is not
                ///</summary>
                ///<param name="currently open, vdbe instruction is a no">op.</param>
                ///<param name=""></param>
                case OpCode.OP_Close:
                    {
                        Debug.Assert(pOp.p1 >= 0 && pOp.p1 < cpu.vdbe.nCursor);
                        vdbeaux.sqlite3VdbeFreeCursor(cpu.vdbe, cpu.vdbe.OpenCursors[pOp.p1]);
                        cpu.vdbe.OpenCursors[pOp.p1] = null;
                        break;
                    }



                ///Opcode: Next P1 P2 * * P5
                ///
                ///Advance cursor P1 so that it points to the next key/data pair in its
                ///table or index.  If there are no more key/value pairs then fall through
                ///to the following instruction.  But if the cursor advance was successful,
                ///jump immediately to P2.
                ///
                ///</summary>
                ///<param name="The P1 cursor must be for a real table, not a pseudo">table.</param>
                ///<param name=""></param>
                ///<param name="See also: Prev">See also: Prev</param>
                ///<param name=""></param>
                ///
                ///<summary>
                ///Opcode: Prev P1 P2 * * *
                ///
                ///Back up cursor P1 so that it points to the previous key/data pair in its
                ///table or index.  If there is no previous key/value pairs then fall through
                ///to the following instruction.  But if the cursor backup was successful,
                ///jump immediately to P2.
                ///
                ///</summary>
                ///<param name="The P1 cursor must be for a real table, not a pseudo">table.</param>
                ///<param name=""></param>
                ///<param name="If P5 is positive and the jump is taken, then event counter">If P5 is positive and the jump is taken, then event counter</param>
                ///<param name="number P5">1 in the prepared statement is incremented.</param>
                ///<param name=""></param>
                ///<param name=""></param>
                case OpCode.OP_Prev:
                ///jump 
                case OpCode.OP_Next:
                    {
                        ///jump 
                        VdbeCursor pC;
                        BtCursor pCrsr;
                        int res;
                        if (cpu.db.u1.isInterrupted)
                            return RuntimeException.abort_due_to_interrupt;
                        //CHECK_FOR_INTERRUPT;
                        Debug.Assert(pOp.p1 >= 0 && pOp.p1 < vdbe.nCursor);
                        Debug.Assert(pOp.p5 <= Sqlite3.ArraySize(vdbe.aCounter));
                        pC = vdbe.OpenCursors[pOp.p1];
                        if (pC == null)
                        {
                            break;
                            ///See ticket #2273 
                        }
                        pCrsr = pC.pCursor;
                        if (pCrsr == null)
                        {
                            pC.nullRow = true;
                            break;
                        }
                        res = 1;
                        Debug.Assert(!pC.deferredMoveto);
                        cpu.rc = pOp.OpCode == OpCode.OP_Next ? pCrsr.sqlite3BtreeNext(ref res) : pCrsr.sqlite3BtreePrevious(ref res);
                        pC.nullRow = res == 1 ? true : false;
                        pC.cacheStatus = Sqlite3.CACHE_STALE;
                        if (res == 0)
                        {
                            cpu.opcodeIndex = pOp.p2 - 1;
                            if (pOp.p5 != 0)
                                vdbe.aCounter[pOp.p5 - 1]++;
#if SQLITE_TEST
#if !TCLSH
																																																																																																																																																													                sqlite3_search_count++;
#else
																																																																																																																																																													                sqlite3_search_count.iValue++;
#endif
#endif
                        }
                        pC.rowidIsValid = false;
                        break;
                    }
                default: return RuntimeException.noop;
            }
            return RuntimeException.OK;
        }
    }
}
