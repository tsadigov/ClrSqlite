using System;

namespace Community.CsharpSqlite.Engine.Op
{
    using System.Collections.Generic;
    using Community.CsharpSqlite.Engine;
    using CsharpSqlite.Core.Runtime;
    using Runtime;
    public class Crud : HandlerBase<CrudRuntime>
    {
        public Crud(CPU cpu) : base(cpu)
        {
            opcodeHandlers = new Dictionary<OpCode, Func<VdbeOp, RuntimeException>>() {
                { OpCode.OP_Insert      ,runtime.Insert },
                { OpCode.OP_InsertInt   ,runtime.Insert},
                { OpCode.OP_Delete      ,runtime.Delete },
                { OpCode.OP_NewRowid    ,runtime.NewRowId }
           };
        }

        public static RuntimeException Exec(CPU cpu, OpCode opcode, VdbeOp pOp)
        {
            return (new Crud(cpu)).Exec(opcode, pOp);
        }
    }
}
