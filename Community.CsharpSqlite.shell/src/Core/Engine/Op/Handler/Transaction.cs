using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using i64 = System.Int64;
using u8 = System.Byte;
using u16 = System.UInt16;
using u32 = System.UInt32;
using u64 = System.UInt64;
using unsigned = System.UIntPtr;
using Pgno = System.UInt32;
using i32 = System.Int32;
using sqlite_int64 = System.Int64;
using Community.CsharpSqlite.Engine;
using Community.CsharpSqlite.Utils;
using Community.CsharpSqlite.Metadata;
using yDbMask = System.Int32;

namespace Community.CsharpSqlite.Engine.Op
{
    using System.Text;
    using sqlite3_value = Engine.Mem;
    using System.Collections.Generic;
    using Community.CsharpSqlite.Engine;
    using Community.CsharpSqlite.Metadata;
    using Community.CsharpSqlite.Os;
    using Vdbe = Engine.Vdbe;
    using tree;
    public class Transaction
    {


        public static RuntimeException Exec(CPU cpu, OpCode opcode, VdbeOp pOp)
        //(Community.CsharpSqlite.Vdbe vdbe, OpCode opcode, ref int opcodeIndex,Mem [] aMem,VdbeOp pOp,ref SqlResult rc)
        {
            var vdbe = cpu.vdbe;
            var aMem = vdbe.aMem;
            var db = cpu.db;

            SqlResult rc;
            switch (opcode)
            {


                ///
                ///<summary>
                ///Opcode: Savepoint P1 * * P4 *
                ///
                ///Open, release or rollback the savepoint named by parameter P4, depending
                ///on the value of P1. To open a new savepoint, P1==0. To release (commit) an
                ///existing savepoint, P1==1, or to rollback an existing savepoint P1==2.
                ///</summary>
                case OpCode.OP_Savepoint:
                    {
                        int p1;
                        ///
                        ///<summary>
                        ///Value of P1 operand 
                        ///</summary>
                        string zName;
                        ///
                        ///<summary>
                        ///Name of savepoint 
                        ///</summary>
                        int nName;
                        Savepoint pNew;
                        Savepoint pSavepoint;
                        Savepoint pTmp;
                        int iSavepoint;
                        int ii;
                        p1 = pOp.p1;
                        zName = pOp.p4.z;
                        ///
                        ///<summary>
                        ///Assert that the p1 parameter is valid. Also that if there is no open
                        ///transaction, then there cannot be any savepoints.
                        ///
                        ///</summary>
                        Debug.Assert(db.pSavepoint == null || db.autoCommit == 0);
                        Debug.Assert(p1 == sqliteinth.SAVEPOINT_BEGIN || p1 == sqliteinth.SAVEPOINT_RELEASE || p1 == sqliteinth.SAVEPOINT_ROLLBACK);
                        Debug.Assert(db.pSavepoint != null || db.isTransactionSavepoint == 0);
                        Debug.Assert(Sqlite3.checkSavepointCount(db) != 0);
                        if (p1 == sqliteinth.SAVEPOINT_BEGIN)
                        {
                            if (db.writeVdbeCnt > 0)
                            {
                                ///
                                ///<summary>
                                ///A new savepoint cannot be created if there are active write
                                ///statements (i.e. open read/write incremental blob handles).
                                ///
                                ///</summary>
                                malloc_cs.sqlite3SetString(ref vdbe.zErrMsg, db, "cannot open savepoint - ", "SQL statements in progress");
                                rc = SqlResult.SQLITE_BUSY;
                            }
                            else
                            {
                                nName = StringExtensions.Strlen30(zName);
#if !SQLITE_OMIT_VIRTUALTABLE
                                ///
                                ///<summary>
                                ///vdbe call is Ok even if vdbe savepoint is actually a transaction
                                ///savepoint (and therefore should not prompt xSavepoint()) callbacks.
                                ///If vdbe is a transaction savepoint being opened, it is guaranteed
                                ///</summary>
                                ///<param name="that the db">>aVTrans[] array is empty.  </param>
                                Debug.Assert(db.autoCommit == 0 || db.nVTrans == 0);
                                rc = VTableMethodsExtensions.sqlite3VtabSavepoint(db, sqliteinth.SAVEPOINT_BEGIN, db.nStatement + db.nSavepoint);
                                if (rc != SqlResult.SQLITE_OK)
                                    return RuntimeException.abort_due_to_error;
#endif
                                ///
                                ///<summary>
                                ///Create a new savepoint structure. 
                                ///</summary>
                                pNew = new Savepoint();
                                // sqlite3DbMallocRaw( db, sizeof( Savepoint ) + nName + 1 );
                                if (pNew != null)
                                {
                                    //pNew.zName = (char )&pNew[1];
                                    //memcpy(pNew.zName, zName, nName+1);
                                    pNew.zName = zName;
                                    ///
                                    ///<summary>
                                    ///If there is no open transaction, then mark vdbe as a special
                                    ///"transaction savepoint". 
                                    ///</summary>
                                    if (db.autoCommit != 0)
                                    {
                                        db.autoCommit = 0;
                                        db.isTransactionSavepoint = 1;
                                    }
                                    else
                                    {
                                        db.nSavepoint++;
                                    }
                                    ///
                                    ///<summary>
                                    ///Link the new savepoint into the database handle's list. 
                                    ///</summary>
                                    pNew.pNext = db.pSavepoint;
                                    db.pSavepoint = pNew;
                                    pNew.nDeferredCons = db.nDeferredCons;
                                }
                            }
                        }
                        else
                        {
                            iSavepoint = 0;
                            ///
                            ///<summary>
                            ///Find the named savepoint. If there is no such savepoint, then an
                            ///an error is returned to the user.  
                            ///</summary>
                            for (pSavepoint = db.pSavepoint; pSavepoint != null && !pSavepoint.zName.Equals(zName, StringComparison.InvariantCultureIgnoreCase); pSavepoint = pSavepoint.pNext)
                            {
                                iSavepoint++;
                            }
                            if (null == pSavepoint)
                            {
                                malloc_cs.sqlite3SetString(ref vdbe.zErrMsg, db, "no such savepoint: %s", zName);
                                rc = SqlResult.SQLITE_ERROR;
                            }
                            else
                                if (db.writeVdbeCnt > 0 || (p1 == sqliteinth.SAVEPOINT_ROLLBACK && db.activeVdbeCnt > 1))
                            {
                                ///
                                ///<summary>
                                ///It is not possible to release (commit) a savepoint if there are
                                ///active write statements. It is not possible to rollback a savepoint
                                ///if there are any active statements at all.
                                ///
                                ///</summary>
                                malloc_cs.sqlite3SetString(ref vdbe.zErrMsg, db, "cannot %s savepoint - SQL statements in progress", (p1 == sqliteinth.SAVEPOINT_ROLLBACK ? "rollback" : "release"));
                                rc = SqlResult.SQLITE_BUSY;
                            }
                            else
                            {
                                ///
                                ///<summary>
                                ///Determine whether or not vdbe is a transaction savepoint. If so,
                                ///and vdbe is a RELEASE command, then the current transaction
                                ///is committed.
                                ///
                                ///</summary>
                                int isTransaction = (pSavepoint.pNext == null && db.isTransactionSavepoint != 0) ? 1 : 0;
                                if (isTransaction != 0 && p1 == sqliteinth.SAVEPOINT_RELEASE)
                                {
                                    if ((rc = vdbe.sqlite3VdbeCheckFk(1)) != SqlResult.SQLITE_OK)
                                    {
                                        return RuntimeException.vdbe_return;
                                    }
                                    db.autoCommit = 1;
                                    if (vdbe.sqlite3VdbeHalt() == SqlResult.SQLITE_BUSY)
                                    {
                                        vdbe.currentOpCodeIndex = cpu.opcodeIndex;
                                        db.autoCommit = 0;
                                        vdbe.rc = rc = SqlResult.SQLITE_BUSY;
                                        return RuntimeException.vdbe_return;
                                    }
                                    db.isTransactionSavepoint = 0;
                                    rc = vdbe.rc;
                                }
                                else
                                {
                                    iSavepoint = db.nSavepoint - iSavepoint - 1;
                                    for (ii = 0; ii < db.BackendCount; ii++)
                                    {
                                        rc = db.Backends[ii].BTree.sqlite3BtreeSavepoint(p1, iSavepoint);
                                        if (rc != SqlResult.SQLITE_OK)
                                        {
                                            return RuntimeException.abort_due_to_error;
                                        }
                                    }
                                    if (p1 == sqliteinth.SAVEPOINT_ROLLBACK && (db.flags & SqliteFlags.SQLITE_InternChanges) != 0)
                                    {
                                        vdbeaux.sqlite3ExpirePreparedStatements(db);
                                        build.sqlite3ResetInternalSchema(db, -1);
                                        db.flags = (db.flags | SqliteFlags.SQLITE_InternChanges);
                                    }
                                }
                                ///
                                ///<summary>
                                ///Regardless of whether vdbe is a RELEASE or ROLLBACK, destroy all
                                ///savepoints nested inside of the savepoint being operated on. 
                                ///</summary>
                                while (db.pSavepoint != pSavepoint)
                                {
                                    pTmp = db.pSavepoint;
                                    db.pSavepoint = pTmp.pNext;
                                    db.DbFree(ref pTmp);
                                    db.nSavepoint--;
                                }
                                ///
                                ///<summary>
                                ///If it is a RELEASE, then destroy the savepoint being operated on 
                                ///too. If it is a ROLLBACK TO, then set the number of deferred 
                                ///constraint violations present in the database to the value stored
                                ///when the savepoint was created.  
                                ///</summary>
                                if (p1 == sqliteinth.SAVEPOINT_RELEASE)
                                {
                                    Debug.Assert(pSavepoint == db.pSavepoint);
                                    db.pSavepoint = pSavepoint.pNext;
                                    db.DbFree(ref pSavepoint);
                                    if (0 == isTransaction)
                                    {
                                        db.nSavepoint--;
                                    }
                                }
                                else
                                {
                                    db.nDeferredCons = pSavepoint.nDeferredCons;
                                }
                                if (0 == isTransaction)
                                {
                                    rc = VTableMethodsExtensions.sqlite3VtabSavepoint(db, p1, iSavepoint);
                                    if (rc != SqlResult.SQLITE_OK)
                                        return RuntimeException.abort_due_to_error;
                                }
                            }
                        }
                        break;
                    }
                ///
                ///<summary>
                ///Opcode: AutoCommit P1 P2 * * *
                ///
                ///</summary>
                ///<param name="Set the database auto">commit flag to P1 (1 or 0). If P2 is true, roll</param>
                ///<param name="back any currently active btree transactions. If there are any active">back any currently active btree transactions. If there are any active</param>
                ///<param name="VMs (apart from vdbe one), then the COMMIT or ROLLBACK statement fails.">VMs (apart from vdbe one), then the COMMIT or ROLLBACK statement fails.</param>
                ///<param name=""></param>
                ///<param name="vdbe instruction causes the VM to halt.">vdbe instruction causes the VM to halt.</param>
                ///<param name=""></param>
                case OpCode.OP_AutoCommit:
                    {
                        int desiredAutoCommit;
                        int iRollback;
                        int turnOnAC;
                        desiredAutoCommit = (u8)pOp.p1;
                        iRollback = pOp.p2;
                        turnOnAC = (desiredAutoCommit != 0 && 0 == db.autoCommit) ? 1 : 0;
                        Debug.Assert(desiredAutoCommit != 0 || 0 == desiredAutoCommit);
                        Debug.Assert(desiredAutoCommit != 0 || 0 == iRollback);
                        Debug.Assert(db.activeVdbeCnt > 0);
                        ///
                        ///<summary>
                        ///At least vdbe one VM is active 
                        ///</summary>
                        if (turnOnAC != 0 && iRollback != 0 && db.activeVdbeCnt > 1)
                        {
                            ///
                            ///<summary>
                            ///If vdbe instruction implements a ROLLBACK and other VMs are
                            ///still running, and a transaction is active, return an error indicating
                            ///that the other VMs must complete first.
                            ///
                            ///</summary>
                            malloc_cs.sqlite3SetString(ref vdbe.zErrMsg, db, "cannot rollback transaction - " + "SQL statements in progress");
                            rc = SqlResult.SQLITE_BUSY;
                        }
                        else
                            if (turnOnAC != 0 && 0 == iRollback && db.writeVdbeCnt > 0)
                        {
                            ///
                            ///<summary>
                            ///If vdbe instruction implements a COMMIT and other VMs are writing
                            ///return an error indicating that the other VMs must complete first.
                            ///
                            ///</summary>
                            malloc_cs.sqlite3SetString(ref vdbe.zErrMsg, db, "cannot commit transaction - " + "SQL statements in progress");
                            rc = SqlResult.SQLITE_BUSY;
                        }
                        else
                                if (desiredAutoCommit != db.autoCommit)
                        {
                            if (iRollback != 0)
                            {
                                Debug.Assert(desiredAutoCommit != 0);
                                Sqlite3.sqlite3RollbackAll(db);
                                db.autoCommit = 1;
                            }
                            else
                                if ((rc = vdbe.sqlite3VdbeCheckFk(1)) != SqlResult.SQLITE_OK)
                            {
                                return RuntimeException.vdbe_return;
                            }
                            else
                            {
                                db.autoCommit = (u8)desiredAutoCommit;
                                if (vdbe.sqlite3VdbeHalt() == SqlResult.SQLITE_BUSY)
                                {
                                    vdbe.currentOpCodeIndex = cpu.opcodeIndex;
                                    db.autoCommit = (u8)(desiredAutoCommit == 0 ? 1 : 0);
                                    vdbe.rc = rc = SqlResult.SQLITE_BUSY;
                                    return RuntimeException.vdbe_return;
                                }
                            }
                            Debug.Assert(db.nStatement == 0);
                            Sqlite3.sqlite3CloseSavepoints(db);
                            if (vdbe.rc == SqlResult.SQLITE_OK)
                            {
                                rc = SqlResult.SQLITE_DONE;
                            }
                            else
                            {
                                rc = SqlResult.SQLITE_ERROR;
                            }
                            return RuntimeException.vdbe_return;
                        }
                        else
                        {
                            malloc_cs.sqlite3SetString(ref vdbe.zErrMsg, db, (0 == desiredAutoCommit) ? "cannot start a transaction within a transaction" : ((iRollback != 0) ? "cannot rollback - no transaction is active" : "cannot commit - no transaction is active"));
                            rc = SqlResult.SQLITE_ERROR;
                        }
                        break;
                    }
                ///
                ///<summary>
                ///Opcode: Transaction P1 P2 * * *
                ///
                ///Begin a transaction.  The transaction ends when a Commit or Rollback
                ///opcode is encountered.  Depending on the ON CONFLICT setting, the
                ///transaction might also be rolled back if an error is encountered.
                ///
                ///P1 is the index of the database file on which the transaction is
                ///started.  Index 0 is the main database file and index 1 is the
                ///file used for temporary tables.  Indices of 2 or more are used for
                ///attached databases.
                ///
                ///</summary>
                ///<param name="If P2 is non">transaction is started.  A RESERVED lock is</param>
                ///<param name="obtained on the database file when a write">transaction is started.  No</param>
                ///<param name="other process can start another write transaction while vdbe transaction is">other process can start another write transaction while vdbe transaction is</param>
                ///<param name="underway.  Starting a write transaction also creates a rollback journal. A">underway.  Starting a write transaction also creates a rollback journal. A</param>
                ///<param name="write transaction must be started before any changes can be made to the">write transaction must be started before any changes can be made to the</param>
                ///<param name="database.  If P2 is 2 or greater then an EXCLUSIVE lock is also obtained">database.  If P2 is 2 or greater then an EXCLUSIVE lock is also obtained</param>
                ///<param name="on the file.">on the file.</param>
                ///<param name=""></param>
                ///<param name="If a write">transaction is started and the Vdbe.usesStmtJournal flag is</param>
                ///<param name="true (vdbe flag is set if the Vdbe may modify more than one row and may">true (vdbe flag is set if the Vdbe may modify more than one row and may</param>
                ///<param name="throw an ABORT exception), a statement transaction may also be opened.">throw an ABORT exception), a statement transaction may also be opened.</param>
                ///<param name="More specifically, a statement transaction is opened iff the database">More specifically, a statement transaction is opened iff the database</param>
                ///<param name="connection is currently not in autocommit mode, or if there are other">connection is currently not in autocommit mode, or if there are other</param>
                ///<param name="active statements. A statement transaction allows the affects of vdbe">active statements. A statement transaction allows the affects of vdbe</param>
                ///<param name="VDBE to be rolled back after an error without having to roll back the">VDBE to be rolled back after an error without having to roll back the</param>
                ///<param name="entire transaction. If no error is encountered, the statement transaction">entire transaction. If no error is encountered, the statement transaction</param>
                ///<param name="will automatically commit when the VDBE halts.">will automatically commit when the VDBE halts.</param>
                ///<param name=""></param>
                ///<param name="If P2 is zero, then a read">lock is obtained on the database file.</param>
                ///<param name=""></param>
                case OpCode.OP_Transaction:
                    {
                        Btree pBt;
                        Debug.Assert(pOp.p1 >= 0 && pOp.p1 < db.BackendCount);
                        Debug.Assert((vdbe.btreeMask & (((yDbMask)1) << pOp.p1)) != 0);
                        pBt = db.Backends[pOp.p1].BTree;
                        if (pBt != null)
                        {
                            rc = pBt.sqlite3BtreeBeginTrans(pOp.p2);
                            if (rc == SqlResult.SQLITE_BUSY)
                            {
                                vdbe.currentOpCodeIndex = cpu.opcodeIndex;
                                vdbe.rc = rc = SqlResult.SQLITE_BUSY;
                                return RuntimeException.vdbe_return;
                            }
                            if (rc != SqlResult.SQLITE_OK)
                            {
                                return RuntimeException.abort_due_to_error;
                            }
                            if (pOp.p2 != 0 && vdbe.usesStmtJournal && (db.autoCommit == 0 || db.activeVdbeCnt > 1))
                            {
                                Debug.Assert(pBt.sqlite3BtreeIsInTrans());
                                if (vdbe.iStatement == 0)
                                {
                                    Debug.Assert(db.nStatement >= 0 && db.nSavepoint >= 0);
                                    db.nStatement++;
                                    vdbe.iStatement = db.nSavepoint + db.nStatement;
                                }
                                rc = VTableMethodsExtensions.sqlite3VtabSavepoint(db, sqliteinth.SAVEPOINT_BEGIN, vdbe.iStatement - 1);
                                if (rc == SqlResult.SQLITE_OK)
                                {
                                    rc = pBt.sqlite3BtreeBeginStmt(vdbe.iStatement);
                                }
                                ///
                                ///<summary>
                                ///Store the current value of the database handles deferred constraint
                                ///counter. If the statement transaction needs to be rolled back,
                                ///the value of vdbe counter needs to be restored too.  
                                ///</summary>
                                vdbe.nStmtDefCons = db.nDeferredCons;
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
