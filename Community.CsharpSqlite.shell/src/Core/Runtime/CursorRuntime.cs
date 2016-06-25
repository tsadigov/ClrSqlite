using Community.CsharpSqlite.Engine;
using Community.CsharpSqlite.Metadata;
using Community.CsharpSqlite.Runtime;
using Community.CsharpSqlite.Tree;
using Community.CsharpSqlite.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using i64 = System.Int64;

namespace Community.CsharpSqlite.Core.Runtime
{
    public class CursorRuntime:IRuntime
    {
        public CPU cpu { get; set; }
        public CursorRuntime()
        { }
        public CursorRuntime(CPU p) {
            this.cpu = p;
        }
        public static CursorRuntime Create(CPU c) {
            return new CursorRuntime(c);
        }

        internal RuntimeException OpenCursor(int iDb,int rootPage,KeyInfo pKeyInfo, int nField,int cursorIndex, byte p5,bool isWrite,bool isTable) {
            Debug.Assert(iDb >= 0 && iDb < cpu.db.BackendCount);

            //Debug.Assert((vdbe.btreeMask & (((yDbMask)1) << iDb)) != 0);
            var pDb = cpu.db.Backends[iDb];
            var btree = pDb.BTree;
            Debug.Assert(btree != null);
            CursorMode wrFlag;
            if (isWrite)
            {
                wrFlag = CursorMode.ReadWrite;
                Debug.Assert(Sqlite3.sqlite3SchemaMutexHeld(cpu.db, iDb, null));
                if (pDb.pSchema.file_format < cpu.vdbe.minWriteFileFormat)
                    cpu.vdbe.minWriteFileFormat = pDb.pSchema.file_format;
            }
            else
                wrFlag = CursorMode.ReadOnly;

            if (p5 != 0)
            {
                Debug.Assert(rootPage > 0);
                Debug.Assert(rootPage <= cpu.vdbe.aMem.Count());
                var pIn2 = cpu.aMem[rootPage];
                Debug.Assert(pIn2.memIsValid());
                Debug.Assert((pIn2.flags & MemFlags.MEM_Int) != 0);
                pIn2.Integerify();
                rootPage = (int)pIn2.u.AsInteger;
                ///The p2 value always comes from a prior  OpCode.OP_CreateTable opcode and
                ///that opcode will always set the p2 value to 2 or more or else fail.
                ///If there were a failure, the prepared statement would have halted
                ///before reaching vdbe instruction. 
                if (Sqlite3.NEVER(rootPage < 2))
                {
                    cpu.rc = sqliteinth.SQLITE_CORRUPT_BKPT();
                    return RuntimeException.abort_due_to_error;
                }
            }

            
            Debug.Assert(cursorIndex >= 0);
            var vdbeCursor = Sqlite3.allocateCursor(cpu.vdbe, cursorIndex, nField, iDb, 1);
            if (vdbeCursor == null)
                return RuntimeException.no_mem;
            vdbeCursor.nullRow = true;
            vdbeCursor.isOrdered = true;
            cpu.rc = btree.sqlite3BtreeCursor(rootPage, wrFlag, pKeyInfo, vdbeCursor.pCursor);
            vdbeCursor.pKeyInfo = pKeyInfo;
            ///Since it performs no memory allocation or IO, the only values that
            ///sqlite3BtreeCursor() may return are SQLITE_EMPTY and SqlResult.SQLITE_OK. 
            ///SQLITE_EMPTY is only returned when attempting to open the table
            ///<param name="rooted at page 1 of a zero">byte database.  </param>
            Debug.Assert(cpu.rc == SqlResult.SQLITE_EMPTY || cpu.rc == SqlResult.SQLITE_OK);
            if (cpu.rc == SqlResult.SQLITE_EMPTY)
            {
                mempoolMethods.sqlite3MemFreeBtCursor(ref vdbeCursor.pCursor);
                cpu.rc = SqlResult.SQLITE_OK;
            }
            ///Set the VdbeCursor.isTable and isIndex variables. Previous versions of
            ///SQLite used to check if the root">page flags were sane at vdbe point</param>
            ///and report database corruption if they were not, but vdbe check has">and report database corruption if they were not, but vdbe check has</param>
            ///since moved into the btree layer.  ">since moved into the btree layer.  </param>
            vdbeCursor.isIndex = !(vdbeCursor.isTable = isTable);

            return RuntimeException.OK;
        }

        ///Opcode: Count P1 P2 * * *
        ///
        ///Store the number of entries (an integer value) in the table or index
        ///opened by cursor P1 in register P2
        internal RuntimeException Count(VdbeOp pOp)
        {
            ///<param name="out2">prerelease </param>
            i64 nEntry = 0;
            BtCursor pCrsr;
            pCrsr = vdbe.OpenCursors[pOp.p1].pCursor;
            if (pCrsr != null)
            {
                cpu.rc = pCrsr.sqlite3BtreeCount(ref nEntry);
            }
            else
            {
                nEntry = 0;
            }
            cpu.pOut.u.AsInteger = nEntry;
            return RuntimeException.OK;
        }

        ///<summary>
        ///Opcode: Sequence P1 P2 * * *
        ///
        ///Find the next available sequence number for cursor P1.
        ///Write the sequence number into register P2.
        ///The sequence number on the cursor is incremented after this
        ///instruction.
        ///</summary>
        internal RuntimeException Sequence(VdbeOp pOp)
        {
            Debug.Assert(pOp.p1 >= 0 && pOp.p1 < vdbe.nCursor);
            Debug.Assert(vdbe.OpenCursors[pOp.p1] != null);
            cpu.pOut.u.AsInteger = (long)vdbe.OpenCursors[pOp.p1].seqCount++;
            return RuntimeException.OK;
        }

        ///<summary>
        ///Opcode: NullRow P1 * * * *
        ///
        ///Move the cursor P1 to a null row.  Any  OpCode.OP_Column operations
        ///that occur while the cursor is on the null row will always
        ///write a NULL.
        ///</summary>
        internal RuntimeException NullRow(VdbeOp pOp)
        {
            Debug.Assert(pOp.p1 >= 0 && pOp.p1 < vdbe.nCursor);
            var pC = vdbe.OpenCursors[pOp.p1];
            Debug.Assert(pC != null);
            pC.nullRow = true;
            pC.rowidIsValid = false;
            if (pC.pCursor != null)
            {
                pC.pCursor.sqlite3BtreeClearCursor();
            }
            return RuntimeException.OK;
        }

        ///<summary>
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
        internal RuntimeException OpenCursor(VdbeOp pOp)
        {
            if (cpu.vdbe.expired)
            {
                cpu.rc = SqlResult.SQLITE_ABORT;
                return RuntimeException.OK;
            }
            var rootPage = pOp.p2;
            var iDb = pOp.p3;

            KeyInfo pKeyInfo = null;
            var nField = 0;

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

            return OpenCursor(iDb, rootPage, pKeyInfo,nField, pOp.p1,pOp.p5, pOp.OpCode == OpCode.OP_OpenWrite, pOp.p4type != P4Usage.P4_KEYINFO);
           
        }

        Vdbe vdbe {
            get { return cpu.vdbe; }
        }
        
        ///<summary>
        ///Opcode: Last P1 P2 * * *
        ///
        ///The next use of the Rowid or Column or Next instruction for P1
        ///will refer to the last entry in the database table or index.
        ///If the table or index is empty and P2>0, then jump immediately to P2.
        ///If P2 is 0 or if the table or index is not empty, fall through
        ///to the following instruction.
        ///</summary>
        internal RuntimeException Last(VdbeOp pOp)
        {
            Last(   cursorIndex:pOp.p1,
                    opcodeIndex:pOp.p2);            

            return RuntimeException.OK;
        }

        internal void Last(int cursorIndex, int opcodeIndex) {
            var res = ThreeState.Neutral;
            Debug.Assert(cursorIndex >= 0 && cursorIndex < vdbe.nCursor);
            var vdbeCursor = vdbe.OpenCursors[cursorIndex];
            Debug.Assert(vdbeCursor != null);
            var pCrsr = vdbeCursor.pCursor;

            if (pCrsr == null)
                res = ThreeState.Positive;
            else
                cpu.rc = pCrsr.sqlite3BtreeLast(ref res);

            vdbeCursor.nullRow = res == ThreeState.Positive ? true : false;
            vdbeCursor.deferredMoveto = false;
            vdbeCursor.rowidIsValid = false;
            vdbeCursor.cacheStatus = Sqlite3.CACHE_STALE;

            if (opcodeIndex > 0 && res != 0)
                cpu.opcodeIndex = opcodeIndex - 1;

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
        internal RuntimeException MoveStep(VdbeOp pOp) {
            var direction = OpCode.OP_Next == pOp.OpCode ?ThreeState.Positive:ThreeState.Negative;
            return MoveStep(pOp.p1,pOp.p2,pOp.p5, direction);
        }
        
        internal RuntimeException MoveStep(int cursorIndex,int opcodeIndex,int p5,ThreeState direction)
        {
            ///jump 
            if (cpu.db.u1.isInterrupted)
                return RuntimeException.abort_due_to_interrupt;
            //CHECK_FOR_INTERRUPT;
            Debug.Assert(cursorIndex >= 0 && cursorIndex < vdbe.nCursor);
            Debug.Assert(p5 <= Sqlite3.ArraySize(vdbe.aCounter));
            var vdbeCursor = vdbe.OpenCursors[cursorIndex];
            if (vdbeCursor == null)
            {
                return RuntimeException.OK;
                ///See ticket #2273 
            }
            var treeCursor = vdbeCursor.pCursor;
            if (treeCursor == null)
            {
                vdbeCursor.nullRow = true;
                return RuntimeException.OK;
            }
            var res = ThreeState.Positive;
            Debug.Assert(!vdbeCursor.deferredMoveto);
            cpu.rc = direction == ThreeState.Positive 
                    ? treeCursor.sqlite3BtreeNext(ref res) 
                    : treeCursor.sqlite3BtreePrevious(ref res);

            vdbeCursor.nullRow = res == ThreeState.Positive ? true : false;
            vdbeCursor.cacheStatus = Sqlite3.CACHE_STALE;
            if (res == 0)
            {
                cpu.opcodeIndex = opcodeIndex - 1;
                if (p5 != 0)
                    vdbe.aCounter[p5 - 1]++;
#if SQLITE_TEST
#if !TCLSH
#else
#endif
#endif
            }
            vdbeCursor.rowidIsValid = false;

            return RuntimeException.OK;
        }

        ///<summary>
        ///Opcode: Rewind P1 P2 * * *
        ///
        ///The next use of the Rowid or Column or Next instruction for P1
        ///will refer to the first entry in the database table or index.
        ///If the table or index is empty and P2>0, then jump immediately to P2.
        ///If P2 is 0 or if the table or index is not empty, fall through
        ///to the following instruction.
        ///
        ///</summary>
        internal RuntimeException Rewind(VdbeOp pOp) {
            return Rewind(pOp.p1,pOp.p2);
        }

        internal RuntimeException Rewind(int openCurorIndex,int opcodeIndex)
        {
            ///jump             
            Debug.Assert(openCurorIndex >= 0 && openCurorIndex < vdbe.nCursor);
            var pC = vdbe.OpenCursors[openCurorIndex];
            Rewind(pC, opcodeIndex);
            return RuntimeException.OK;
        }

        private void Rewind(Engine.Core.Runtime.VdbeCursor pC, int opcodeIndex)
        {
            BtCursor pCrsr = pC.pCursor;
            Debug.Assert(pC != null);
            var res = ThreeState.Positive;
            if (pCrsr != null)
            {
                cpu.rc = pCrsr.sqlite3BtreeFirst(ref res);
                pC.atFirst = res == 0 ? true : false;
                pC.deferredMoveto = false;
                pC.cacheStatus = Sqlite3.CACHE_STALE;
                pC.rowidIsValid = false;
            }
            pC.nullRow = res == ThreeState.Positive ? true : false;
            Debug.Assert(opcodeIndex > 0 && opcodeIndex < vdbe.aOp.Count());
            if (res != 0)
            {
                cpu.opcodeIndex = opcodeIndex - 1;
            }
        }


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
        internal RuntimeException OpenEphemeral(VdbeOp pOp)
        {
            const int vfsFlags = Sqlite3.SQLITE_OPEN_READWRITE | Sqlite3.SQLITE_OPEN_CREATE | Sqlite3.SQLITE_OPEN_EXCLUSIVE | Sqlite3.SQLITE_OPEN_DELETEONCLOSE | Sqlite3.SQLITE_OPEN_TRANSIENT_DB;
            Debug.Assert(pOp.p1 >= 0);
            var pCx = Sqlite3.allocateCursor(cpu.vdbe, pOp.p1, pOp.p2, -1, 1);
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
                ///If a transient index is required, create it by calling
                ///sqlite3BtreeCreateTable() with the BTREE_BLOBKEY flag before
                ///opening it. If a transient table is required, just use the
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
                        cpu.rc = pCx.pBt.sqlite3BtreeCursor(pgno, CursorMode.ReadWrite, pOp.p4.pKeyInfo, pCx.pCursor);
                        pCx.pKeyInfo = pOp.p4.pKeyInfo;
                        pCx.pKeyInfo.enc = sqliteinth.ENC(cpu.vdbe.db);
                    }
                    pCx.isTable = false;
                }
                else
                {
                    cpu.rc = pCx.pBt.sqlite3BtreeCursor(sqliteinth.MASTER_ROOT, CursorMode.ReadWrite, null, pCx.pCursor);
                    pCx.isTable = true;
                }
            }
            pCx.isOrdered = (pOp.p5 != Sqlite3.BTREE_UNORDERED);
            pCx.isIndex = !pCx.isTable;

            return RuntimeException.OK;
        }


        ///<summary>
        ///Opcode: Close P1 * * * *
        ///
        ///Close a cursor previously opened as P1.  If P1 is not
        ///</summary>
        ///<param name="currently open, vdbe instruction is a no">op.</param>
        ///<param name=""></param>
        internal RuntimeException Close(VdbeOp pOp)
        {
            Debug.Assert(pOp.p1 >= 0 && pOp.p1 < cpu.vdbe.nCursor);
            vdbeaux.sqlite3VdbeFreeCursor(cpu.vdbe, cpu.vdbe.OpenCursors[pOp.p1]);
            cpu.vdbe.OpenCursors[pOp.p1] = null;
            return RuntimeException.OK;
        }


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
        internal RuntimeException OpenPseudo(VdbeOp pOp) {
            return OpenPseudo(
                iCur:pOp.p1, 
                pseudoTableReg:pOp.p2,
                nField:pOp.p3);
        }
        internal RuntimeException OpenPseudo(int iCur,int pseudoTableReg,int nField)
        {
            Debug.Assert(iCur >= 0);
            var pCx = Sqlite3.allocateCursor(cpu.vdbe, iCur, nField, -1, 0);
            if (pCx == null)
                return RuntimeException.no_mem;
            pCx.nullRow = true;
            pCx.pseudoTableReg = pseudoTableReg;
            pCx.isTable = true;
            pCx.isIndex = false;
            return RuntimeException.OK;
        }

        ///<summary>
        ///Opcode: Sort P1 P2 * * *
        ///
        ///This opcode does exactly the same thing as  OpCode.OP_Rewind except that
        ///it increments an undocumented global variable used for testing.
        ///
        ///Sorting is accomplished by writing records into a sorting index,
        ///then rewinding that index and playing it back from beginning to
        ///end.  We use the  OpCode.OP_Sort opcode instead of  OpCode.OP_Rewind to do the
        ///rewinding so that the global variable will be incremented and
        ///regression tests can determine whether or not the optimizer is
        ///correctly optimizing out sorts.
        ///
        ///</summary>
        internal RuntimeException Sort(VdbeOp pOp)
        {
            ///jump 
#if SQLITE_TEST
#if !TCLSH
              sqlite3_search_count--;
#else
              sqlite3_search_count.iValue--;
#endif
#endif
            vdbe.aCounter[Sqlite3.SQLITE_STMTSTATUS_SORT - 1]++;
            ///Fall through into  OpCode.OP_Rewind 
            return Rewind(pOp);
        }
    }
}
