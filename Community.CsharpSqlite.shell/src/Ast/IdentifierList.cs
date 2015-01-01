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
using System.Collections.Generic;

#else
using ynVar = System.Int32; 
#endif

namespace Community.CsharpSqlite
{
        ///<summary>
        /// An instance of this structure can hold a simple list of identifiers,
        /// such as the list "a,b,c" in the following statements:
        ///
        ///      INSERT INTO t(a,b,c) VALUES ...;
        ///      CREATE INDEX idx ON t(a,b,c);
        ///      CREATE TRIGGER trig BEFORE UPDATE ON t(a,b,c) ...;
        ///
        /// The IdList.a.idx field is used when the IdList represents the list of
        /// column names after a table name in an INSERT statement.  In the statement
        ///
        ///     INSERT INTO t(a,b,c) ...
        ///
        /// If "a" is the k-th column of table "t", then IdList.a[0].idx==k.
        ///
        ///</summary>
        public class IdList_item
        {
            ///<summary>
            ///Name of the identifier 
            ///</summary>
            public string zName;

            ///<summary>
            ///Index in some Table.aCol[] of a column named zName 
            ///</summary>
            public int idx;            
        }

        //TODO: List<IdList_item>

        public class IdList 
        {
            public IdList_item[] a;
            public int nId;
            ///<summary>
            ///Number of identifiers on the list
            ///</summary>
            public int nAlloc;
            ///
            ///<summary>
            ///Number of entries allocated for a[] below 
            ///</summary>
            public IdList Copy()
            {
                if (this == null)
                    return null;
                else
                {
                    IdList cp = (IdList)MemberwiseClone();
                    a.CopyTo(cp.a, 0);
                    return cp;
                }
            }
        };

    }
