using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Community.CsharpSqlite
{  
        public enum MemType
        {
            //#define MEMTYPE_HEAP       0x01  /* General heap allocations */
            //#define MEMTYPE_LOOKASIDE  0x02  /* Might have been lookaside memory */
            //#define MEMTYPE_SCRATCH    0x04  /* Scratch allocations */
            //#define MEMTYPE_PCACHE     0x08  /* Page cache allocations */
            //#define MEMTYPE_DB         0x10  /* Uses sqlite3DbMalloc, not sqlite_malloc */
            HEAP = 0x01,
            LOOKASIDE = 0x02,
            SCRATCH = 0x04,
            PCACHE = 0x08,
            DB = 0x10
        }
 
}
