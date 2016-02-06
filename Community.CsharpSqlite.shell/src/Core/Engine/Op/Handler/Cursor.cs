using System;
using System.Collections.Generic;

namespace Community.CsharpSqlite.Engine.Op
{
    using CsharpSqlite.Core.Runtime;
    using Runtime;
    public class Cursor : HandlerBase<CursorRuntime>
    {
        public Cursor(CPU cpu) : base(cpu)
        {
            opcodeHandlers = new Dictionary<OpCode, Func<VdbeOp, RuntimeException>>() {
                #if !SQLITE_OMIT_BTREECOUNT
                { OpCode.OP_Count       ,runtime.Count},
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

        public static RuntimeException Exec(CPU cpu, OpCode opcode, VdbeOp pOp)
        {
            return (new Cursor(cpu)).Exec(opcode, pOp);
        }
    }
}
