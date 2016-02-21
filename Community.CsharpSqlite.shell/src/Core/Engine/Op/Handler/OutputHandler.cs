using System;

namespace Community.CsharpSqlite.Engine.Op
{
    using System.Collections.Generic;
    using Community.CsharpSqlite.Engine;
    using CsharpSqlite.Core.Runtime;
    using Runtime;
    public class Output : HandlerBase<OutputRuntime>
    {
        public Output(CPU cpu) : base(cpu)
        {
            opcodeHandlers = new Dictionary<OpCode, Func<VdbeOp, RuntimeException>>() {
                { OpCode.OP_ResultRow      ,runtime.ResultRow},
                { OpCode.OP_Column      ,runtime.Column}
           };
        }

        public static RuntimeException Exec(CPU cpu, OpCode opcode, VdbeOp pOp)
        {
            return (new Output(cpu)).Exec(opcode, pOp);
        }
    }
}
