using Community.CsharpSqlite.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Community.CsharpSqlite.Engine;
using System.Diagnostics;
using Community.CsharpSqlite.Utils;
using Community.CsharpSqlite.Metadata;
using u8 = System.Byte;
using Community.CsharpSqlite.builder;
using Community.CsharpSqlite.Parsing;

namespace Community.CsharpSqlite.Core.Runtime
{
    public class SchemaRuntime : IRuntime
    {
        public CPU cpu { get; set; }
        public SchemaRuntime()
        { }
        public SchemaRuntime(CPU p)
        {
            this.cpu = p;
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
        public RuntimeException OP_ParseSchema(VdbeOp pOp)
        {

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

            var iDb = pOp.p1;
            var db = cpu.db;
            Debug.Assert(iDb >= 0 && iDb < db.BackendCount);
            Debug.Assert(db.DbHasProperty(iDb, sqliteinth.DB_SchemaLoaded));
            ///
            ///<summary>
            ///Used to be a conditional 
            ///</summary>
            {
                var zMaster = sqliteinth.SCHEMA_TABLE(iDb);
                var initData = new InitData();
                initData.db = db;
                initData.iDb = pOp.p1;
                initData.pzErrMsg = cpu.vdbe.zErrMsg;
                var zSql = Os.io.sqlite3MPrintf(db, "SELECT name, rootpage, sql FROM '%q'.%s WHERE %s ORDER BY rowid", db.Backends[iDb].Name, zMaster, pOp.p4.z);
                if (String.IsNullOrEmpty(zSql))
                {
                    cpu.rc = SqlResult.SQLITE_NOMEM;
                }
                else
                {
                    Debug.Assert(db.init.IsBusy);
                    using (db.init.scope())
                    {
                        initData.rc = SqlResult.SQLITE_OK;
                        //Debug.Assert( 0 == db.mallocFailed );
                        cpu.rc = legacy.Exec(db, zSql, SchemaExtensions.InitTableDefinitionCallback, initData, 0);
                        if (cpu.rc == SqlResult.SQLITE_OK)
                            cpu.rc = initData.rc;
                        db.DbFree(ref zSql);
                    }
                }
            }
            if (cpu.rc == SqlResult.SQLITE_NOMEM)
            {
                return RuntimeException.no_mem;
            }
            return RuntimeException.OK;
        }


        Mem pOut
        {
            get { return cpu.pOut; }
            set { cpu.pOut = value; }
        }

        ///<summary>
        ///Opcode: Destroy P1 P2 P3 * *
        ///
        ///Delete an entire database table or index whose root page in the database
        ///file is given by P1.
        ///
        ///The table being destroyed is in the main database file if P3==0.  If
        ///P3==1 then the table to be clear is in the auxiliary database file
        ///that is used to store tables create using CREATE TEMPORARY TABLE.
        ///
        ///If AUTOVACUUM is enabled then it is possible that another root page
        ///might be moved into the newly deleted root page in order to keep all
        ///root pages contiguous at the beginning of the database.  The former
        ///</summary>
        ///<param name="value of the root page that moved "></param>
        ///<param name="is stored in register P2.  If no page">is stored in register P2.  If no page</param>
        ///<param name="movement was required (because the table being dropped was already">movement was required (because the table being dropped was already</param>
        ///<param name="the last one in the database) then a zero is stored in register P2.">the last one in the database) then a zero is stored in register P2.</param>
        ///<param name="If AUTOVACUUM is disabled then a zero is stored in register P2.">If AUTOVACUUM is disabled then a zero is stored in register P2.</param>
        ///<param name=""></param>
        ///<param name="See also: Clear">See also: Clear</param>
        ///<param name=""></param>
        public RuntimeException OP_Destroy(VdbeOp pOp)
        {
            var db = cpu.db;

            ///<param name="out2">prerelease </param>
            int iMoved = 0;
            int iDb;
#if !SQLITE_OMIT_VIRTUALTABLE
            var iCnt = db.pVdbe.linkedList()
                .Where(pVdbe => pVdbe.magic == VdbeMagic.VDBE_MAGIC_RUN && pVdbe.inVtabMethod < 2 && pVdbe.currentOpCodeIndex >= 0)
                .Count();
#else
																																																																																																																																				              iCnt = db.activeVdbeCnt;
#endif
            pOut.flags = MemFlags.MEM_Null;
            if (iCnt > 1)
            {
                cpu.rc = SqlResult.SQLITE_LOCKED;
                cpu.errorAction = OnConstraintError.OE_Abort;
            }
            else
            {
                iDb = pOp.p3;
                Debug.Assert(iCnt == 1);
                //Debug.Assert((vdbe.btreeMask & (((yDbMask)1) << iDb)) != 0);//TODO:ERROR
                cpu.rc = db.Backends[iDb].BTree.DropTable(pOp.p1, ref iMoved);
                pOut.flags = MemFlags.MEM_Int;
                pOut.u.AsInteger = iMoved;
#if !SQLITE_OMIT_AUTOVACUUM
                if (cpu.rc == SqlResult.SQLITE_OK && iMoved != 0)
                {
                    build.sqlite3RootPageMoved(db, iDb, iMoved, pOp.p1);
                    ///
                    ///<summary>
                    ///All  OpCode.OP_Destroy operations occur on the same btree 
                    ///</summary>
                    Debug.Assert(cpu.resetSchemaOnFault == 0 || cpu.resetSchemaOnFault == iDb + 1);
                    cpu.resetSchemaOnFault = (u8)(iDb + 1);
                }
#endif
            }
            return RuntimeException.OK;
        }

        ///<summary>
        ///Opcode: DropTable P1 * * P4 *
        ///</summary>
        ///Remove the internal (in">memory) data structures that describe</param>
        ///the table named P4 in database P1.  This is called after a table</param>
        ///is dropped in order to keep the internal representation of the</param>
        ///schema consistent with what is on disk.</param>
        public RuntimeException OP_DropTable(VdbeOp pOp)
        {
            var db = cpu.db;
            TableBuilder.sqlite3UnlinkAndDeleteTable(db, pOp.p1, pOp.p4.z);
            return RuntimeException.OK;
        }

        ///
        ///<summary>
        ///Opcode: DropIndex P1 * * P4 *
        ///
        ///Remove the internal (in-memory) data structures that describe
        ///the index named P4 in database P1.  This is called after an index
        ///is dropped in order to keep the internal representation of the
        ///schema consistent with what is on disk.
        public RuntimeException OP_DropIndex(VdbeOp pOp)
        {
            var db = cpu.db;
            IndexBuilder.sqlite3UnlinkAndDeleteIndex(db, pOp.p1, pOp.p4.z);
            return RuntimeException.OK;
        }


        ///
        ///<summary>
        ///Opcode: DropTrigger P1 * * P4 *
        ///
        ///</summary>
        ///<param name="Remove the internal (in">memory) data structures that describe</param>
        ///<param name="the trigger named P4 in database P1.  This is called after a trigger">the trigger named P4 in database P1.  This is called after a trigger</param>
        ///<param name="is dropped in order to keep the internal representation of the">is dropped in order to keep the internal representation of the</param>
        ///<param name="schema consistent with what is on disk.">schema consistent with what is on disk.</param>
        ///<param name=""></param>
        public RuntimeException OP_DropTrigger(VdbeOp pOp)
        {
            var db = cpu.db;
            TriggerParser.sqlite3UnlinkAndDeleteTrigger(db, pOp.p1, pOp.p4.z);
            return RuntimeException.OK;
        }

        //
        ///<summary>
        ///Opcode: CreateTable P1 P2 * * *
        ///
        ///Allocate a new table in the main database file if P1==0 or in the
        ///auxiliary database file if P1==1 or in an attached database if
        ///P1>1.  Write the root page number of the new table into
        ///register P2
        ///
        ///The difference between a table and an index is this:  A table must
        ///</summary>
        ///<param name="have a 4">byte integer key and can have arbitrary data.  An index</param>
        ///<param name="has an arbitrary key but no data.">has an arbitrary key but no data.</param>
        ///<param name=""></param>
        ///<param name="See also: CreateIndex">See also: CreateIndex</param>
        ///<param name=""></param>
        ///
        ///<summary>
        ///Opcode: CreateIndex P1 P2 * * *
        ///
        ///Allocate a new index in the main database file if P1==0 or in the
        ///auxiliary database file if P1==1 or in an attached database if
        ///P1>1.  Write the root page number of the new table into
        ///register P2.
        ///
        ///See documentation on  OpCode.OP_CreateTable for additional information.
        ///
        ///</summary>
        public RuntimeException OP_CreateTable_CreateIndex(VdbeOp pOp)
        {
            var db = cpu.db;
            ///<param name="out2">prerelease </param>
            int flags;
            var pgno = 0;
            Debug.Assert(pOp.p1 >= 0 && pOp.p1 < db.BackendCount);
            //Debug.Assert((this.btreeMask & (((yDbMask)1) << pOp.p1)) != 0);//TODO:ERROR
            var pDb = db.Backends[pOp.p1];
            Debug.Assert(pDb.BTree != null);
            if (pOp.OpCode == OpCode.OP_CreateTable)
            {
                ///flags = BTREE_INTKEY; 
                flags = Sqlite3.BTREE_INTKEY;
            }
            else
            {
                flags = Sqlite3.BTREE_BLOBKEY;
            }
            cpu.rc = pDb.BTree.CreateTable(ref pgno, flags);
            pOut.u.AsInteger = pgno;
            return RuntimeException.OK;
        }
    }
}