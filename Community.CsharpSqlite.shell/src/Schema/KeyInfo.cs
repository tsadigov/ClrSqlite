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



        ///<summary>
        /// An instance of the following structure is passed as the first
        /// argument to sqlite3VdbeKeyCompare and is used to control the
        /// comparison of the two index keys.
        ///
        ///</summary>
        public class KeyInfo
        {
            public Sqlite3.sqlite3 db;
            ///
            ///<summary>
            ///The database connection 
            ///</summary>
            public SqliteEncoding enc;
            ///
            ///<summary>
            ///</summary>
            ///<param name="Text encoding "> one of the SQLITE_UTF* values </param>
            public u16 nField;
            ///
            ///<summary>
            ///Number of entries in aColl[] 
            ///</summary>
            public SortOrder[] aSortOrder;
            ///<summary>
            ///Sort order for each column.  May be NULL
            ///</summary>
            private CollSeq[] _aColl = new CollSeq[1];

            public CollSeq[] aColl
            {
                get { return _aColl; }
                set { _aColl = value; }
            }
            ///
            ///<summary>
            ///Collating sequence for each term of the key 
            ///</summary>
            public KeyInfo Copy()
            {
                return (KeyInfo)MemberwiseClone();
            }
        };




    }
