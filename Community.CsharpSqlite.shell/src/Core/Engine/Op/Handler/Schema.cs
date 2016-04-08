using Community.CsharpSqlite.builder;
using Community.CsharpSqlite.Parsing;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using u8 = System.Byte;

namespace Community.CsharpSqlite.Engine.Op
{
    using CsharpSqlite.Core.Runtime;
    using CsharpSqlite.Metadata;
    using Metadata;
    using Runtime;
    using Utils;
    public class SchemaOps:HandlerBase<SchemaRuntime>
    {

        public SchemaOps(CPU cpu) : base(cpu)
        {
            opcodeHandlers = new Dictionary<OpCode, Func<VdbeOp, RuntimeException>>() {
                { OpCode.OP_Destroy      ,runtime.OP_Destroy },
                { OpCode.OP_DropIndex   ,runtime.OP_DropIndex},
                { OpCode.OP_DropTable      ,runtime.OP_DropTable},
                { OpCode.OP_DropTrigger    ,runtime.OP_DropTrigger},
                { OpCode.OP_ParseSchema    ,runtime.OP_ParseSchema},
                { OpCode.OP_CreateTable    ,runtime.OP_CreateTable_CreateIndex},
                { OpCode.OP_CreateIndex    ,runtime.OP_CreateTable_CreateIndex}
           };
        }

        public static RuntimeException Exec(CPU cpu, OpCode opcode, VdbeOp pOp)
        {
            return (new SchemaOps(cpu)).Exec(opcode, pOp);
        }


    }
}
