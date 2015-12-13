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
    public class AutoVacuum
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


#if !SQLITE_OMIT_VACUUM && !SQLITE_OMIT_ATTACH
                ///Opcode: Vacuum * * * * *
                ///
                ///Vacuum the entire database.  This opcode will cause other virtual
                ///machines to be created and run.  It may not be called from within
                ///a transaction.
                case OpCode.OP_Vacuum:
                    {
                        rc = Sqlite3.sqlite3RunVacuum(ref vdbe.zErrMsg, db);
                        break;
                    }
#endif
#if !SQLITE_OMIT_AUTOVACUUM
                ///
                ///<summary>
                ///Opcode: IncrVacuum P1 P2 * * *
                ///
                ///Perform a single step of the incremental vacuum procedure on
                ///the P1 database. If the vacuum has finished, jump to instruction
                ///P2. Otherwise, fall through to the next instruction.
                ///</summary>
                case OpCode.OP_IncrVacuum:
                    {
                        ///
                        ///<summary>
                        ///jump 
                        ///</summary>
                        Btree pBt;
                        Debug.Assert(pOp.p1 >= 0 && pOp.p1 < db.BackendCount);
                        Debug.Assert((vdbe.btreeMask & (((yDbMask)1) << pOp.p1)) != 0);
                        pBt = db.Backends[pOp.p1].BTree;
                        rc = pBt.sqlite3BtreeIncrVacuum();
                        if (rc == SqlResult.SQLITE_DONE)
                        {
                            cpu.opcodeIndex = pOp.p2 - 1;
                            rc = SqlResult.SQLITE_OK;
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
