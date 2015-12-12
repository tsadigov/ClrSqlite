using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Community.CsharpSqlite.Os
{
    class OpenMode
    {
        public string z;
        public int mode;
        public OpenMode(string z, int mode)
        {
            this.z = z;
            this.mode = mode;
        }
    }
}
