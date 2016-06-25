using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Community.CsharpSqlite.Utils
{
    public interface IBusyScope
    {
        void Enter();
        void Exit();
    }

    public static class ScopeExtensions{
        public static IDisposable scope(this IBusyScope scope)
        {
            return new BusyScope(scope);
        }

        class BusyScope : IDisposable
        {
            IBusyScope owner;
            public BusyScope(IBusyScope owner)
            {
                this.owner = owner;
                if(null!=owner)
                    owner.Enter();
            }

            public void Dispose()
            {
                if (null != owner)
                    owner.Exit();
            }
        }
        public static bool sqlite3BtreeHoldsMutex(this Tree.Btree tree) {
            return true;
        }
            

    }



}
