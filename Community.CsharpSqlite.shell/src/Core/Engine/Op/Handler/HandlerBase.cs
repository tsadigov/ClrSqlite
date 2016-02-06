using Community.CsharpSqlite.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Community.CsharpSqlite.Runtime
{
    public interface IRuntime {
        CPU cpu { get; set; }
    }
    public class HandlerBase<TRuntime> where TRuntime:IRuntime,new()
    {
        CPU cpu;
        public HandlerBase(CPU cpu)
        {
            this.cpu = cpu;
            runtime.cpu = cpu;
        }

        static TRuntime s_runtime = new TRuntime();
        public static TRuntime runtime
        {
            get { return s_runtime; }
        }

        public static RuntimeException Exec(CPU cpu, OpCode opcode, VdbeOp pOp)
        {
            HandlerBase<TRuntime> c = new HandlerBase<TRuntime>(cpu);
            return c.Exec(opcode, pOp);
        }

        public RuntimeException Exec(OpCode opcode, VdbeOp pOp)
        {
            Func<VdbeOp, RuntimeException> handle = null;
            var found = opcodeHandlers.TryGetValue(opcode, out handle);
            return found ? handle(pOp) : RuntimeException.noop;
        }

        public Dictionary<OpCode, Func<VdbeOp, RuntimeException>> opcodeHandlers = new Dictionary<OpCode, Func<VdbeOp, RuntimeException>>();

    }
}
