using Community.CsharpSqlite.Paging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Community.CsharpSqlite
{
    public class FreeSpaceHeader:IEnumerator
    {
       // public MemPage
        int addr;
        int ps;
        MemPage page;
        public FreeSpaceHeader(MemPage page) {
            this.page = page;
            Reset();
        }
        public object Current
        {
            get { throw new NotImplementedException(); }
        }

        public bool MoveNext()
        {
            addr = ps;
            ps=page.aData.get2byte(addr);
            return 0 != ps;
        }

        public void Reset()
        {
            addr = page.hdrOffset + 1;
        }
    }
}
