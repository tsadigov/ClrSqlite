using Community.CsharpSqlite.tree;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using i64 = System.Int64;
using u8 = System.Byte;
using u32 = System.UInt32;
using yDbMask = System.Int32;


namespace Community.CsharpSqlite.Engine.Op
{
    public static class Others
    {
        public static RuntimeException Exec(CPU cpu,OpCode opcode,VdbeOp pOp)
        {
        //public static RuntimeException Exec(Community.CsharpSqlite.Vdbe vdbe, SqliteEncoding encoding,OpCode opcode,VdbeOp pOp,Mem [] aMem,ref SqlResult rc) {

            var aMem = cpu.aMem;
            var vdbe = cpu.vdbe;
            var encoding = cpu.encoding;

            switch (opcode) {

                ///
                ///<summary>
                ///Opcode: CollSeq * * P4
                ///
                ///P4 is a pointer to a CollSeq struct. If the next call to a user function
                ///or aggregate calls sqlite3GetFuncCollSeq(), this collation sequence will
                ///</summary>
                ///<param name="be returned. This is used by the built">in min(), max() and nullif()</param>
                ///<param name="functions.">functions.</param>
                ///<param name=""></param>
                ///<param name="The interface used by the implementation of the aforementioned functions">The interface used by the implementation of the aforementioned functions</param>
                ///<param name="to retrieve the collation sequence set by this opcode is not available">to retrieve the collation sequence set by this opcode is not available</param>
                ///<param name="publicly, only to user functions defined in func.c.">publicly, only to user functions defined in func.c.</param>
                ///<param name=""></param>
                case OpCode.OP_CollSeq:
                    {
                        Debug.Assert(pOp.p4type == P4Usage.P4_COLLSEQ);
                        break;
                    }

                ///
                ///<summary>
                ///Opcode: MustBeInt P1 P2 * * *
                ///
                ///Force the value in register P1 to be an integer.  If the value
                ///in P1 is not an integer and cannot be converted into an integer
                ///without data loss, then jump immediately to P2, or if P2==0
                ///raise an SQLITE_MISMATCH exception.
                ///
                ///</summary>
                case OpCode.OP_MustBeInt:
                    {
                        ///
                        ///<summary>
                        ///jump, in1 
                        ///</summary>
                        var pIn1 = aMem[pOp.p1];
                        pIn1.applyAffinity(sqliteinth.SQLITE_AFF_NUMERIC, encoding);
                        if ((pIn1.flags & MemFlags.MEM_Int) == 0)
                        {
                            if (pOp.p2 == 0)
                            {
                                cpu.rc = SqlResult.SQLITE_MISMATCH;
                                return RuntimeException.abort_due_to_error;
                            }
                            else
                            {
                                cpu.opcodeIndex = pOp.p2 - 1;
                            }
                        }
                        else
                        {
                            pIn1.MemSetTypeFlag(MemFlags.MEM_Int);
                        }
                        break;
                    }



#if !SQLITE_OMIT_FLOATING_POINT
                ///
                ///<summary>
                ///Opcode: RealAffinity P1 * * * *
                ///
                ///If register P1 holds an integer convert it to a real value.
                ///
                ///This opcode is used when extracting information from a column that
                ///has REAL affinity.  Such column values may still be stored as
                ///integers, for space efficiency, but after extraction we want them
                ///to have only a real value.
                ///</summary>
                case OpCode.OP_RealAffinity:
                    {
                        ///
                        ///<summary>
                        ///in1 
                        ///</summary>
                        var pIn1 = aMem[pOp.p1];
                        if ((pIn1.flags & MemFlags.MEM_Int) != 0)
                        {
                            pIn1.sqlite3VdbeMemRealify();
                        }
                        break;
                    }
#endif





                ///
                ///<summary>
                ///Opcode: Affinity P1 P2 * P4 *
                ///
                ///Apply affinities to a range of P2 registers starting with P1.
                ///
                ///P4 is a string that is P2 characters long. The nth character of the
                ///string indicates the column affinity that should be used for the nth
                ///memory cell in the range.
                ///
                ///</summary>
                case OpCode.OP_Affinity:
                    {
                        string zAffinity;
                        ///The affinity to be applied 
                        char cAff;
                        ///A single character of affinity 
                        zAffinity = pOp.p4.z;
                        Debug.Assert(!String.IsNullOrEmpty(zAffinity));
                        Debug.Assert(zAffinity.Length <= pOp.p2);
                        //zAffinity[pOp.p2] == 0
                        //pIn1 = aMem[pOp.p1];
                        for (int zI = 0; zI < zAffinity.Length; zI++)// while( (cAff = *(zAffinity++))!=0 ){
                        {
                            cAff = zAffinity[zI];
                            var pIn1 = aMem[pOp.p1 + zI];
                            //Debug.Assert( pIn1 <= p->aMem[p->nMem] );
                            Debug.Assert(pIn1.memIsValid());
                            pIn1.ExpandBlob();
                            pIn1.applyAffinity(cAff, encoding);
                            //pIn1++;
                        }
                        break;
                    }




                ///
                ///<summary>
                ///Opcode: Count P1 P2 * * *
                ///
                ///Store the number of entries (an integer value) in the table or index
                ///opened by cursor P1 in register P2
                ///
                ///</summary>
#if !SQLITE_OMIT_BTREECOUNT
                case OpCode.OP_Count:
                    {
                        ///
                        ///<summary>
                        ///</summary>
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
                        cpu.pOut.u.i = nEntry;
                        break;
                    }
#endif






                #region cookie




                ///
                ///<summary>
                ///Opcode: ReadCookie P1 P2 P3 * *
                ///
                ///Read cookie number P3 from database P1 and write it into register P2.
                ///P3==1 is the schema version.  P3==2 is the database format.
                ///P3==3 is the recommended pager cache size, and so forth.  P1==0 is
                ///the main database file and P1==1 is the database file used to store
                ///temporary tables.
                ///
                ///</summary>
                ///<param name="There must be a read">lock on the database (either a transaction</param>
                ///<param name="must be started or there must be an open cursor) before">must be started or there must be an open cursor) before</param>
                ///<param name="executing this instruction.">executing this instruction.</param>
                ///<param name=""></param>
                case OpCode.OP_ReadCookie:
                    {
                        ///
                        ///<summary>
                        ///</summary>
                        ///<param name="out2">prerelease </param>
                        u32 iMeta;
                        int iDb;
                        int iCookie;
                        iMeta = 0;
                        iDb = pOp.p1;
                        iCookie = pOp.p3;
                        Debug.Assert(pOp.p3 < Sqlite3.SQLITE_N_BTREE_META);
                        Debug.Assert(iDb >= 0 && iDb < vdbe.db.nDb);
                        Debug.Assert(vdbe.db.aDb[iDb].pBt != null);
                        Debug.Assert((vdbe.btreeMask & (((yDbMask)1) << iDb)) != 0);
                        iMeta = vdbe.db.aDb[iDb].pBt.sqlite3BtreeGetMeta(iCookie);
                        cpu.pOut.u.i = (int)iMeta;
                        break;
                    }
                ///
                ///<summary>
                ///Opcode: SetCookie P1 P2 P3 * *
                ///
                ///Write the content of register P3 (interpreted as an integer)
                ///into cookie number P2 of database P1.  P2==1 is the schema version.
                ///P2==2 is the database format. P2==3 is the recommended pager cache
                ///size, and so forth.  P1==0 is the main database file and P1==1 is the
                ///database file used to store temporary tables.
                ///
                ///A transaction must be started before executing this opcode.
                ///
                ///</summary>
                case OpCode.OP_SetCookie:
                    {
                        ///
                        ///<summary>
                        ///in3 
                        ///</summary>
                        Db pDb;
                        Debug.Assert(pOp.p2 < Sqlite3.SQLITE_N_BTREE_META);
                        Debug.Assert(pOp.p1 >= 0 && pOp.p1 < vdbe.db.nDb);
                        Debug.Assert((vdbe.btreeMask & (((yDbMask)1) << pOp.p1)) != 0);
                        pDb = vdbe.db.aDb[pOp.p1];
                        Debug.Assert(pDb.pBt != null);
                        Debug.Assert(Sqlite3.sqlite3SchemaMutexHeld(vdbe.db, pOp.p1, null));
                        var pIn3 = aMem[pOp.p3];
                        pIn3.sqlite3VdbeMemIntegerify();
                        ///
                        ///<summary>
                        ///See note about index shifting on  OpCode.OP_ReadCookie 
                        ///</summary>
                        cpu.rc = pDb.pBt.sqlite3BtreeUpdateMeta(pOp.p2, (u32)pIn3.u.i);
                        if (pOp.p2 == (int)BTreeProp.SCHEMA_VERSION)
                        {
                            ///
                            ///<summary>
                            ///When the schema cookie changes, record the new cookie internally 
                            ///</summary>
                            pDb.pSchema.schema_cookie = (int)pIn3.u.i;
                            vdbe.db.flags |= SqliteFlags.SQLITE_InternChanges;
                        }
                        else
                            if (pOp.p2 == (int)BTreeProp.FILE_FORMAT)
                            {
                                ///
                                ///<summary>
                                ///Record changes in the file format 
                                ///</summary>
                                pDb.pSchema.file_format = (u8)pIn3.u.i;
                            }
                        if (pOp.p1 == 1)
                        {
                            ///
                            ///<summary>
                            ///Invalidate all prepared statements whenever the TEMP database
                            ///schema is changed.  Ticket #1644 
                            ///</summary>
                            vdbeaux.sqlite3ExpirePreparedStatements(vdbe.db);
                            vdbe.expired = false;
                        }
                        break;
                    }
                ///
                ///<summary>
                ///Opcode: VerifyCookie P1 P2 P3 * *
                ///
                ///Check the value of global database parameter number 0 (the
                ///schema version) and make sure it is equal to P2 and that the
                ///generation counter on the local schema parse equals P3.
                ///
                ///P1 is the database number which is 0 for the main database file
                ///and 1 for the file holding temporary tables and some higher number
                ///for auxiliary databases.
                ///
                ///The cookie changes its value whenever the database schema changes.
                ///This operation is used to detect when that the cookie has changed
                ///and that the current process needs to reread the schema.
                ///
                ///Either a transaction needs to have been started or an  OpCode.OP_Open needs
                ///to be executed (to establish a read lock) before this opcode is
                ///invoked.
                ///
                ///</summary>
                case OpCode.OP_VerifyCookie:
                    {
                        u32 iMeta = 0;
                        u32 iGen;
                        Btree pBt;
                        Debug.Assert(pOp.p1 >= 0 && pOp.p1 < vdbe.db.nDb);
                        Debug.Assert((vdbe.btreeMask & ((yDbMask)1 << pOp.p1)) != 0);
                        Debug.Assert(Sqlite3.sqlite3SchemaMutexHeld(vdbe.db, pOp.p1, null));
                        pBt = vdbe.db.aDb[pOp.p1].pBt;
                        if (pBt != null)
                        {
                            iMeta = pBt.sqlite3BtreeGetMeta(BTreeProp.SCHEMA_VERSION);
                            iGen = vdbe.db.aDb[pOp.p1].pSchema.iGeneration;
                        }
                        else
                        {
                            iGen = iMeta = 0;
                        }
                        if (iMeta != pOp.p2 || iGen != pOp.p3)
                        {
                            vdbe.db.sqlite3DbFree(ref vdbe.zErrMsg);
                            vdbe.zErrMsg = "database schema has changed";
                            // sqlite3DbStrDup(db, "database schema has changed");
                            ///
                            ///<summary>
                            ///</summary>
                            ///<param name="If the schema">cookie from the database file matches the cookie</param>
                            ///<param name="stored with the in">memory representation of the schema, do</param>
                            ///<param name="not reload the schema from the database file.">not reload the schema from the database file.</param>
                            ///<param name=""></param>
                            ///<param name="If virtual">tables are in use, this is not just an optimization.</param>
                            ///<param name="Often, v">tables store their data in other SQLite tables, which</param>
                            ///<param name="are queried from within xNext() and other v">table methods using</param>
                            ///<param name="prepared queries. If such a query is out">date, we do not want to</param>
                            ///<param name="discard the database schema, as the user code implementing the">discard the database schema, as the user code implementing the</param>
                            ///<param name="v">table would have to be ready for the sqlite3_vtab structure itself</param>
                            ///<param name="to be invalidated whenever sqlite3_step() is called from within">to be invalidated whenever sqlite3_step() is called from within</param>
                            ///<param name="a v">table method.</param>
                            ///<param name=""></param>
                            if (vdbe.db.aDb[pOp.p1].pSchema.schema_cookie != iMeta)
                            {
                                build.sqlite3ResetInternalSchema(vdbe.db, pOp.p1);
                            }
                            vdbe.expired = true;
                            vdbe.rc = SqlResult.SQLITE_SCHEMA;
                        }
                        break;
                    }
                #endregion






                ///
                ///<summary>
                ///Opcode: Sequence P1 P2 * * *
                ///
                ///Find the next available sequence number for cursor P1.
                ///Write the sequence number into register P2.
                ///The sequence number on the cursor is incremented after this
                ///instruction.
                ///
                ///</summary>
                case OpCode.OP_Sequence:
                    {
                        ///
                        ///<summary>
                        ///</summary>
                        ///<param name="out2">prerelease </param>
                        Debug.Assert(pOp.p1 >= 0 && pOp.p1 < vdbe.nCursor);
                        Debug.Assert(vdbe.OpenCursors[pOp.p1] != null);
                        cpu.pOut.u.i = (long)vdbe.OpenCursors[pOp.p1].seqCount++;
                        break;
                    }




















                ///
                ///<summary>
                ///Opcode: ResetCount P1 * *
                ///
                ///The value of the change counter is copied to the database handle
                ///change counter (returned by subsequent calls to sqlite3_changes()).
                ///Then the VMs internal change counter resets to 0.
                ///This is used by trigger programs.
                ///
                ///</summary>
                case OpCode.OP_ResetCount:
                    {
                        vdbeaux.sqlite3VdbeSetChanges(vdbe.db, vdbe.nChange);
                        vdbe.nChange = 0;
                        break;
                    }









                ///
                ///<summary>
                ///Opcode: NullRow P1 * * * *
                ///
                ///Move the cursor P1 to a null row.  Any  OpCode.OP_Column operations
                ///that occur while the cursor is on the null row will always
                ///write a NULL.
                ///
                ///</summary>
                case OpCode.OP_NullRow:
                    {
                        VdbeCursor pC;
                        Debug.Assert(pOp.p1 >= 0 && pOp.p1 < vdbe.nCursor);
                        pC = vdbe.OpenCursors[pOp.p1];
                        Debug.Assert(pC != null);
                        pC.nullRow = true;
                        pC.rowidIsValid = false;
                        if (pC.pCursor != null)
                        {
                            pC.pCursor.sqlite3BtreeClearCursor();
                        }
                        break;
                    }











                ///
                ///<summary>
                ///Opcode: Last P1 P2 * * *
                ///
                ///The next use of the Rowid or Column or Next instruction for P1
                ///will refer to the last entry in the database table or index.
                ///If the table or index is empty and P2>0, then jump immediately to P2.
                ///If P2 is 0 or if the table or index is not empty, fall through
                ///to the following instruction.
                ///
                ///</summary>
                case OpCode.OP_Last:
                    {
                        ///
                        ///<summary>
                        ///jump 
                        ///</summary>
                        VdbeCursor pC;
                        BtCursor pCrsr;
                        int res = 0;
                        Debug.Assert(pOp.p1 >= 0 && pOp.p1 < vdbe.nCursor);
                        pC = vdbe.OpenCursors[pOp.p1];
                        Debug.Assert(pC != null);
                        pCrsr = pC.pCursor;
                        if (pCrsr == null)
                        {
                            res = 1;
                        }
                        else
                        {
                            cpu.rc = pCrsr.sqlite3BtreeLast(ref res);
                        }
                        pC.nullRow = res == 1 ? true : false;
                        pC.deferredMoveto = false;
                        pC.rowidIsValid = false;
                        pC.cacheStatus = Sqlite3.CACHE_STALE;
                        if (pOp.p2 > 0 && res != 0)
                        {
                            cpu.opcodeIndex = pOp.p2 - 1;
                        }
                        break;
                    }






                ///
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
                case OpCode.OP_Sort:
                    {
                        ///
                        ///<summary>
                        ///jump 
                        ///</summary>
#if SQLITE_TEST
#if !TCLSH
																																																																																																																																				              sqlite3_sort_count++;
              sqlite3_search_count--;
#else
																																																																																																																																				              sqlite3_sort_count.iValue++;
              sqlite3_search_count.iValue--;
#endif
#endif
                        vdbe.aCounter[Sqlite3.SQLITE_STMTSTATUS_SORT - 1]++;
                        ///
                        ///<summary>
                        ///Fall through into  OpCode.OP_Rewind 
                        ///</summary>
                        goto case OpCode.OP_Rewind;
                    }




                ///
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
                case OpCode.OP_Rewind:
                    {
                        ///jump 
                        VdbeCursor pC;
                        BtCursor pCrsr;
                        int res = 0;
                        Debug.Assert(pOp.p1 >= 0 && pOp.p1 < vdbe.nCursor);
                        pC = vdbe.OpenCursors[pOp.p1];
                        Debug.Assert(pC != null);
                        res = 1;
                        if ((pCrsr = pC.pCursor) != null)
                        {
                            cpu.rc = pCrsr.sqlite3BtreeFirst(ref res);
                            pC.atFirst = res == 0 ? true : false;
                            pC.deferredMoveto = false;
                            pC.cacheStatus = Sqlite3.CACHE_STALE;
                            pC.rowidIsValid = false;
                        }
                        pC.nullRow = res == 1 ? true : false;
                        Debug.Assert(pOp.p2 > 0 && pOp.p2 < vdbe.nOp);
                        if (res != 0)
                        {
                            cpu.opcodeIndex = pOp.p2 - 1;
                        }
                        break;
                    }























                ///
                ///<summary>
                ///Opcode: ParseSchema P1 * * P4 *
                ///
                ///Read and parse all entries from the SQLITE_MASTER table of database P1
                ///that match the WHERE clause P4. 
                ///
                ///This opcode invokes the parser to create a new virtual machine,
                ///</summary>
                ///<param name="then runs the new virtual machine.  It is thus a re">entrant opcode.</param>
                ///<param name=""></param>
                case OpCode.OP_ParseSchema:
                    {
                        int iDb;
                        string zMaster;
                        string zSql;
                        InitData initData;
                        ///
                        ///<summary>
                        ///Any prepared statement that invokes this opcode will hold mutexes
                        ///on every btree.  This is a prerequisite for invoking
                        ///sqlite3InitCallback().
                        ///
                        ///</summary>
#if SQLITE_DEBUG
																																																																																																																																				              for ( iDb = 0; iDb < db.nDb; iDb++ )
              {
                Debug.Assert( iDb == 1 || sqlite3BtreeHoldsMutex( db.aDb[iDb].pBt ) );
              }
#endif

                        var db = vdbe.db;
                        iDb = pOp.p1;
                        Debug.Assert(iDb >= 0 && iDb < db.nDb);
                        Debug.Assert(db.DbHasProperty(iDb, sqliteinth.DB_SchemaLoaded));
                        ///
                        ///<summary>
                        ///Used to be a conditional 
                        ///</summary>
                        {
                            zMaster = sqliteinth.SCHEMA_TABLE(iDb);
                            initData = new InitData();
                            initData.db = db;
                            initData.iDb = pOp.p1;
                            initData.pzErrMsg = vdbe.zErrMsg;
                            zSql = Os.io.sqlite3MPrintf(db, "SELECT name, rootpage, sql FROM '%q'.%s WHERE %s ORDER BY rowid", db.aDb[iDb].zName, zMaster, pOp.p4.z);
                            if (String.IsNullOrEmpty(zSql))
                            {
                                cpu.rc = SqlResult.SQLITE_NOMEM;
                            }
                            else
                            {
                                Debug.Assert(0 == db.init.busy);
                                db.init.busy = 1;
                                initData.rc = SqlResult.SQLITE_OK;
                                //Debug.Assert( 0 == db.mallocFailed );
                                cpu.rc = legacy.sqlite3_exec(db, zSql, (dxCallback)Sqlite3.sqlite3InitCallback, (object)initData, 0);
                                if (cpu.rc == SqlResult.SQLITE_OK)
                                    cpu.rc = initData.rc;
                                db.sqlite3DbFree(ref zSql);
                                db.init.busy = 0;
                            }
                        }
                        if (cpu.rc == SqlResult.SQLITE_NOMEM)
                        {
                            return RuntimeException.no_mem;
                        }
                        break;
                    }
















#if !SQLITE_OMIT_ANALYZE
                ///<summary>
                ///Opcode: LoadAnalysis P1 * * * *
                ///
                ///Read the sqlite_stat1 table for database P1 and load the content
                ///of that table into the internal index hash table.  This will cause
                ///the analysis to be used when preparing all subsequent queries.
                ///</summary>
                case OpCode.OP_LoadAnalysis:
                    {
                        Debug.Assert(pOp.p1 >= 0 && pOp.p1 < vdbe.db.nDb);
                        cpu.rc = Sqlite3.sqlite3AnalysisLoad(vdbe.db, pOp.p1);
                        break;
                    }
#endif






























#if !SQLITE_OMIT_INTEGRITY_CHECK
                ///
                ///<summary>
                ///Opcode: IntegrityCk P1 P2 P3 * P5
                ///
                ///Do an analysis of the currently open database.  Store in
                ///register P1 the text of an error message describing any problems.
                ///If no problems are found, store a NULL in register P1.
                ///
                ///The register P3 contains the maximum number of allowed errors.
                ///At most reg(P3) errors will be reported.
                ///In other words, the analysis stops as soon as reg(P1) errors are
                ///seen.  Reg(P1) is updated with the number of errors remaining.
                ///
                ///The root page numbers of all tables in the database are integer
                ///stored in reg(P1), reg(P1+1), reg(P1+2), ....  There are P2 tables
                ///total.
                ///
                ///If P5 is not zero, the check is done on the auxiliary database
                ///file, not the main database file.
                ///
                ///This opcode is used to implement the integrity_check pragma.
                ///</summary>
                case OpCode.OP_IntegrityCk:
                    {
                        int nRoot;
                        ///Number of tables to check.  (Number of root pages.) 
                        int[] aRoot = null;
                        ///Array of rootpage numbers for tables to be checked 
                        int j;
                        ///Loop counter 
                        int nErr = 0;
                        ///Number of errors reported 
                        string z;
                        ///Text of the error report 
                        Mem pnErr;
                        ///Register keeping track of errors remaining 
                        nRoot = pOp.p2;
                        Debug.Assert(nRoot > 0);
                        aRoot = malloc_cs.sqlite3Malloc(aRoot, (nRoot + 1));
                        // sqlite3DbMallocRaw(db, sizeof(int) * (nRoot + 1));
                        if (aRoot == null)
                            return RuntimeException.no_mem;
                        Debug.Assert(pOp.p3 > 0 && pOp.p3 <= vdbe.nMem);
                        pnErr = aMem[pOp.p3];
                        Debug.Assert((pnErr.flags & MemFlags.MEM_Int) != 0);
                        Debug.Assert((pnErr.flags & (MemFlags.MEM_Str | MemFlags.MEM_Blob)) == 0);
                        var pIn1 = aMem[pOp.p1];
                        for (j = 0; j < nRoot; j++)
                        {
                            aRoot[j] = (int)vdbe.aMem[pOp.p1 + j].sqlite3VdbeIntValue();
                            // pIn1[j]);
                        }
                        aRoot[j] = 0;
                        Debug.Assert(pOp.p5 < vdbe.db.nDb);
                        Debug.Assert((vdbe.btreeMask & (((yDbMask)1) << pOp.p5)) != 0);
                        z = vdbe.db.aDb[pOp.p5].pBt.sqlite3BtreeIntegrityCheck(aRoot, nRoot, (int)pnErr.u.i, ref nErr);
                        vdbe.db.sqlite3DbFree(ref aRoot);
                        pnErr.u.i -= nErr;
                        pIn1.sqlite3VdbeMemSetNull();
                        if (nErr == 0)
                        {
                            Debug.Assert(z == "");
                        }
                        else
                            if (String.IsNullOrEmpty(z))
                            {
                                return RuntimeException.no_mem;
                            }
                            else
                            {
                                pIn1.sqlite3VdbeMemSetStr(z, -1, SqliteEncoding.UTF8, null);
                                //malloc_cs.sqlite3_free );
                            }
#if SQLITE_TEST
																																																																																																																																				              UPDATE_MAX_BLOBSIZE( pIn1 );
#endif
                        vdbemem_cs.sqlite3VdbeChangeEncoding(pIn1, encoding);
                        break;
                    }
#endif






























                ///
                ///<summary>
                ///Opcode: RowSetRead P1 P2 P3 * *
                ///
                ///Extract the smallest value from boolean index P1 and put that value into
                ///register P3.  Or, if boolean index P1 is initially empty, leave P3
                ///unchanged and jump to instruction P2.
                ///
                ///</summary>
                case OpCode.OP_RowSetRead:
                    {
                        ///jump, in1, ref3 
                        i64 val = 0;
                        if (vdbe.db.u1.isInterrupted)
                            return RuntimeException.abort_due_to_interrupt;
                        //CHECK_FOR_INTERRUPT;
                        var pIn1 = aMem[pOp.p1];
                        if ((pIn1.flags & MemFlags.MEM_RowSet) == 0 || pIn1.u.pRowSet.sqlite3RowSetNext(ref val) == 0)
                        {
                            ///The boolean index is empty 
                            pIn1.sqlite3VdbeMemSetNull();
                            cpu.opcodeIndex = pOp.p2 - 1;
                        }
                        else
                        {
                            ///A value was pulled from the index 
                            aMem[pOp.p3].sqlite3VdbeMemSetInt64(val);
                        }
                        break;
                    }
                ///



















                ///
                ///<summary>
                ///Opcode: Clear P1 P2 P3
                ///
                ///Delete all contents of the database table or index whose root page
                ///in the database file is given by P1.  But, unlike Destroy, do not
                ///remove the table or index from the database file.
                ///
                ///The table being clear is in the main database file if P2==0.  If
                ///P2==1 then the table to be clear is in the auxiliary database file
                ///that is used to store tables create using CREATE TEMPORARY TABLE.
                ///
                ///</summary>
                ///<param name="If the P3 value is non">zero, then the table referred to must be an</param>
                ///<param name="intkey table (an SQL table, not an index). In this case the row change">intkey table (an SQL table, not an index). In this case the row change</param>
                ///<param name="count is incremented by the number of rows in the table being cleared.">count is incremented by the number of rows in the table being cleared.</param>
                ///<param name="If P3 is greater than zero, then the value stored in register P3 is">If P3 is greater than zero, then the value stored in register P3 is</param>
                ///<param name="also incremented by the number of rows in the table being cleared.">also incremented by the number of rows in the table being cleared.</param>
                ///<param name=""></param>
                ///<param name="See also: Destroy">See also: Destroy</param>
                ///<param name=""></param>
                case OpCode.OP_Clear:
                    {
                        int nChange;
                        nChange = 0;
                        Debug.Assert((vdbe.btreeMask & (((yDbMask)1) << pOp.p2)) != 0);
                        int iDummy0 = 0;
                        if (pOp.p3 != 0)
                            cpu.rc = vdbe.db.aDb[pOp.p2].pBt.sqlite3BtreeClearTable(pOp.p1, ref nChange);
                        else
                            cpu.rc = vdbe.db.aDb[pOp.p2].pBt.sqlite3BtreeClearTable(pOp.p1, ref iDummy0);
                        if (pOp.p3 != 0)
                        {
                            vdbe.nChange += nChange;
                            if (pOp.p3 > 0)
                            {
                                Debug.Assert(aMem[pOp.p3].memIsValid());
                                vdbe.memAboutToChange(aMem[pOp.p3]);
                                aMem[pOp.p3].u.i += nChange;
                            }
                        }
                        break;
                    }

                ///
                ///<summary>
                ///Opcode: RowSetAdd P1 P2 * * *
                ///
                ///Insert the integer value held by register P2 into a boolean index
                ///held in register P1.
                ///
                ///An assertion fails if P2 is not an integer.
                ///</summary>
                case OpCode.OP_RowSetAdd:
                    {
                        ///in1, in2 
                        var pIn1 = aMem[pOp.p1];
                        var pIn2 = aMem[pOp.p2];
                        Debug.Assert((pIn2.flags & MemFlags.MEM_Int) != 0);
                        if ((pIn1.flags & MemFlags.MEM_RowSet) == 0)
                        {
                            pIn1.sqlite3VdbeMemSetRowSet();
                            if ((pIn1.flags & MemFlags.MEM_RowSet) == 0)
                                return RuntimeException.no_mem;
                        }
                        pIn1.u.pRowSet.sqlite3RowSetInsert(pIn2.u.i);
                        break;
                    }
                               












                    //------------------------------------------------------------
                    //------------------------------------------------------------
                    //------------------------------------------------------------









                    #if !SQLITE_OMIT_PAGER_PRAGMAS
                                ///
                                ///<summary>
                                ///Opcode: Pagecount P1 P2 * * *
                                ///
                                ///Write the current number of pages in database P1 to memory cell P2.
                                ///</summary>
                                case OpCode.OP_Pagecount:
                                    {
                                        ///
                                        ///<summary>
                                        ///</summary>
                                        ///<param name="out2">prerelease </param>
                                        cpu.pOut.u.i = vdbe.db.aDb[pOp.p1].pBt.sqlite3BtreeLastPage();
                                        break;
                                    }
#endif
#if !SQLITE_OMIT_PAGER_PRAGMAS
                                ///
                                ///<summary>
                                ///Opcode: MaxPgcnt P1 P2 P3 * *
                                ///
                                ///Try to set the maximum page count for database P1 to the value in P3.
                                ///Do not let the maximum page count fall below the current page count and
                                ///do not change the maximum page count value if P3==0.
                                ///
                                ///Store the maximum page count after the change in register P2.
                                ///</summary>
                                case OpCode.OP_MaxPgcnt:
                                    {
                                        ///
                                        ///<summary>
                                        ///</summary>
                                        ///<param name="out2">prerelease </param>
                                        i64 newMax;
                                        Btree pBt;
                                        pBt = vdbe.db.aDb[pOp.p1].pBt;
                                        newMax = 0;
                                        if (pOp.p3 != 0)
                                        {
                                            newMax = pBt.sqlite3BtreeLastPage();
                                            if (newMax < pOp.p3)
                                                newMax = pOp.p3;
                                        }
                                        cpu.pOut.u.i = (i64)pBt.GetMaxPageCount((int)newMax);
                                        break;
                                    }
#endif
#if !SQLITE_OMIT_TRACE
                                ///
                                ///<summary>
                                ///Opcode: Trace * * * P4 *
                                ///
                                ///If tracing is enabled (by the sqlite3_trace()) interface, then
                                ///</summary>
                                ///<param name="the UTF">8 string contained in P4 is emitted on the trace callback.</param>
                                case OpCode.OP_Trace:
                                    {
                                        string zTrace;
                                        string z;
                                        if (vdbe.db.xTrace != null && !String.IsNullOrEmpty(zTrace = (pOp.p4.z != null ? pOp.p4.z : vdbe.zSql)))
                                        {
                                            z = Sqlite3.sqlite3VdbeExpandSql(vdbe, zTrace);
                                            vdbe.db.xTrace(vdbe.db.pTraceArg, z);
                                            //sqlite3DbFree( db, ref z );
                                        }
#if SQLITE_DEBUG
																																																																																																																																				              if ( ( db.flags & SQLITE_SqlTrace ) != 0
                && ( zTrace = ( pOp.p4.z != null ? pOp.p4.z : p.zSql ) ) != "" )
              {
                sqlite3DebugPrintf( "SQL-trace: %s\n", zTrace );
              }
#endif
                                        break;
                                    }

#endif




                                default: return RuntimeException.noop;



            }
            return RuntimeException.OK;
        }
    }
}
