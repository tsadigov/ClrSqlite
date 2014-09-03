using System;
using System.Diagnostics;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Text;
using Bitmask = System.UInt64;
using i16 = System.Int16;
using i64 = System.Int64;
using sqlite3_int64 = System.Int64;
using u8 = System.Byte;
using u16 = System.UInt16;
using u32 = System.UInt32;
using u64 = System.UInt64;
using unsigned = System.UInt64;
using Pgno = System.UInt32;

#if !SQLITE_MAX_VARIABLE_NUMBER
using ynVar = System.Int16;

#else
using ynVar = System.Int32; 
#endif

namespace Community.CsharpSqlite
{
    public partial class Sqlite3
    {








        ///<summary>
        /// For each nested loop in a WHERE clause implementation, the WhereInfo
        /// structure contains a single instance of this structure.  This structure
        /// is intended to be private the the where.c module and should not be
        /// access or modified by other modules.
        ///
        /// The pIdxInfo field is used to help pick the best index on a
        /// virtual table.  The pIdxInfo pointer contains indexing
        /// information for the i-th table in the FROM clause before reordering.
        /// All the pIdxInfo pointers are freed by whereInfoFree() in where.c.
        /// All other information in the i-th WhereLevel object for the i-th table
        /// after FROM clause ordering.
        ///
        ///</summary>
        public class InLoop
        {
            public int iCur;

            ///
            ///<summary>
            ///The VDBE cursor used by this IN operator 
            ///</summary>

            public int addrInTop;
            ///
            ///<summary>
            ///Top of the IN loop 
            ///</summary>

        }



    }
}
