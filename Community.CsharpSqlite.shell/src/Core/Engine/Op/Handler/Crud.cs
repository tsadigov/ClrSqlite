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

namespace Community.CsharpSqlite.Engine.Op
{
    using System.Text;
    using sqlite3_value = Engine.Mem;
    using System.Collections.Generic;
    using Community.CsharpSqlite.Engine;
    using Community.CsharpSqlite.Metadata;
    using Community.CsharpSqlite.Os;
    using Vdbe = Engine.Vdbe;
    using Core.Runtime;
    using CsharpSqlite.Core.Runtime;
    public class Crud
    {

        public Dictionary<OpCode, Func<VdbeOp, RuntimeException>> opcodeHandlers = new Dictionary<OpCode, Func<VdbeOp, RuntimeException>>() {
            { OpCode.OP_Insert      ,runtime.Insert },
            { OpCode.OP_InsertInt   ,runtime.Insert},
            { OpCode.OP_Delete      ,runtime.Delete },
            { OpCode.OP_NewRowid    ,runtime.NewRowId }
        };





        CPU cpu;
        public Crud(CPU cpu)
        {
            this.cpu = cpu;
            runtime.cpu = cpu;
        }

        static CrudRuntime s_runtime = new CrudRuntime();
        private static CrudRuntime runtime
        {
            get { return s_runtime; }
        }

        public static RuntimeException Exec(CPU cpu, OpCode opcode, VdbeOp pOp)
        {
            Crud c = new Crud(cpu);
            return c.Exec(opcode, pOp);
        }

        public RuntimeException Exec(OpCode opcode, VdbeOp pOp)
        {
            Func<VdbeOp, RuntimeException> handle = null;
            var found = opcodeHandlers.TryGetValue(opcode, out handle);
            return found ? handle(pOp) : RuntimeException.noop;
        }
    }
}
