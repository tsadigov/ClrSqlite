using System;
using System.Collections.Generic;

namespace Community.CsharpSqlite.Engine.Op
{
    using CsharpSqlite.Core.Runtime;
    public class Cursor
    {
        CPU cpu;
        public Cursor(CPU cpu)
        {
            this.cpu = cpu;
            runtime.cpu = cpu;
        }

        static CursorRuntime s_runtime = new CursorRuntime();
        private static CursorRuntime runtime
        {
            get { return s_runtime; }
        }

        public static RuntimeException Exec(CPU cpu, OpCode opcode, VdbeOp pOp)
        {
            Cursor c = new Cursor(cpu);
            return c.Exec(opcode, pOp);
        }

        public RuntimeException Exec(OpCode opcode, VdbeOp pOp)
        {
            Func<VdbeOp, RuntimeException> handle = null;
            var found = opcodeHandlers.TryGetValue(opcode, out handle);
            return found ? handle(pOp) : RuntimeException.noop;
        }


        public Dictionary<OpCode, Func<VdbeOp, RuntimeException>> opcodeHandlers = new Dictionary<OpCode, Func<VdbeOp, RuntimeException>>() {
            #if !SQLITE_OMIT_BTREECOUNT
            { OpCode.OP_Count,runtime.Count},
            #endif
            { OpCode.OP_Sequence    ,runtime.Sequence },///out2-prerelease
            { OpCode.OP_NullRow     ,runtime.NullRow},
            { OpCode.OP_Last        ,runtime.Last },///jump 
            { OpCode.OP_OpenRead    ,runtime.OpenCursor },
            { OpCode.OP_OpenWrite   ,runtime.OpenCursor },
            { OpCode.OP_OpenAutoindex,runtime.OpenEphemeral},
            { OpCode.OP_OpenEphemeral,runtime.OpenEphemeral},
            { OpCode.OP_OpenPseudo  ,runtime.OpenPseudo },
            { OpCode.OP_Close       ,runtime.Close },
            { OpCode.OP_Prev        ,runtime.MoveStep },///jump 
            { OpCode.OP_Next        ,runtime.MoveStep},///jump 
            { OpCode.OP_Sort        ,runtime.Sort },
            { OpCode.OP_Rewind      ,runtime.Rewind }

        };


    }
}
