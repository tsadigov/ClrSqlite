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
using Hash=Community.CsharpSqlite.Sqlite3.Hash;
#if !SQLITE_MAX_VARIABLE_NUMBER
using ynVar = System.Int16;
using System.Collections.Generic;

#else
using ynVar = System.Int32; 
#endif

namespace Community.CsharpSqlite
{
        ///
        ///<summary>
        ///An instance of the following structure stores a database schema.
        ///
        ///Most Schema objects are associated with a Btree.  The exception is
        ///</summary>
        ///<param name="the Schema for the TEMP databaes (sqlite3.aDb[1]) which is free">standing.</param>
        ///<param name="In shared cache mode, a single Schema object can be shared by multiple">In shared cache mode, a single Schema object can be shared by multiple</param>
        ///<param name="Btrees that refer to the same underlying BtShared object.">Btrees that refer to the same underlying BtShared object.</param>
        ///<param name=""></param>
        ///<param name="Schema objects are automatically deallocated when the last Btree that">Schema objects are automatically deallocated when the last Btree that</param>
        ///<param name="references them is destroyed.   The TEMP Schema is manually freed by">references them is destroyed.   The TEMP Schema is manually freed by</param>
        ///<param name="sqlite3_close().">sqlite3_close().</param>
        ///<param name=""></param>
        ///<param name="A thread must be holding a mutex on the corresponding Btree in order">A thread must be holding a mutex on the corresponding Btree in order</param>
        ///<param name="to access Schema content.  This implies that the thread must also be">to access Schema content.  This implies that the thread must also be</param>
        ///<param name="holding a mutex on the sqlite3 connection pointer that owns the Btree.">holding a mutex on the sqlite3 connection pointer that owns the Btree.</param>
        ///<param name="For a TEMP Schema, only the connection mutex is required.">For a TEMP Schema, only the connection mutex is required.</param>
        ///<param name=""></param>
        public class Schema
        {

            static List<Schema> s_instances = new List<Schema>();
            public Schema()
            {
                s_instances.Add(this);
            }

            ///
            ///<summary>
            ///Database schema version number for this file 
            ///</summary>
            public int schema_cookie;
            ///
            ///<summary>
            ///Generation counter.  Incremented with each change 
            ///</summary>
            public u32 iGeneration;
            ///
            ///<summary>
            ///All tables indexed by name 
            ///</summary>
            public Hash tblHash = new Hash();
            ///
            ///<summary>
            ///All (named) indices indexed by name 
            ///</summary>
            public Hash idxHash = new Hash();
            ///
            ///<summary>
            ///All triggers indexed by name 
            ///</summary>
            public Hash trigHash = new Hash();
            ///
            ///<summary>
            ///All foreign keys by referenced table name 
            ///</summary>
            public Hash fkeyHash = new Hash();
            ///
            ///<summary>
            ///The sqlite_sequence table used by AUTOINCREMENT 
            ///</summary>
            public Table pSeqTab;
            ///
            ///<summary>
            ///Schema format version for this file 
            ///</summary>
            public u8 file_format;
            ///
            ///<summary>
            ///Text encoding used by this database 
            ///</summary>
            public SqliteEncoding enc;
            ///<summary>
            ///Flags associated with this schema
            ///</summary>
            public u16 flags;
            ///<summary>
            ///Number of pages to use in the cache
            ///</summary>
            public int cache_size;
            
            public Schema Copy()
            {
                if (this == null)
                    return null;
                else
                {
                    Schema cp = (Schema)MemberwiseClone();
                    return cp;
                }
            }
            public void Clear()
            {
                if (this != null)
                {
                    schema_cookie = 0;
                    tblHash = new Hash();
                    idxHash = new Hash();
                    trigHash = new Hash();
                    fkeyHash = new Hash();
                    pSeqTab = null;
                }
            }
        };

    }
