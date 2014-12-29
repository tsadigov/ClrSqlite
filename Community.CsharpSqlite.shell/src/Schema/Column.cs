﻿
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
        /// information about each column of an SQL table is held in an instance
        /// of this structure.
        ///
        ///</summary>
        public class Column
        {
            public string zName;
            ///
            ///<summary>
            ///Name of this column 
            ///</summary>
            public Expr pDflt;
            ///
            ///<summary>
            ///Default value of this column 
            ///</summary>
            public string zDflt;
            ///
            ///<summary>
            ///Original text of the default value 
            ///</summary>
            public string zType;
            ///
            ///<summary>
            ///Data type for this column 
            ///</summary>
            public string zColl;
            ///
            ///<summary>
            ///Collating sequence.  If NULL, use the default 
            ///</summary>
            public u8 notNull;
            ///
            ///<summary>
            ///True if there is a NOT NULL constraint 
            ///</summary>
            public u8 isPrimKey;
            ///
            ///<summary>
            ///True if this column is part of the PRIMARY KEY 
            ///</summary>
            public char affinity;
            ///
            ///<summary>
            ///One of the SQLITE_AFF_... values 
            ///</summary>
#if !SQLITE_OMIT_VIRTUALTABLE
            public u8 isHidden;
            ///<summary>
            ///True if this column is 'hidden'
            ///</summary>
#endif
            public Column Copy()
            {
                Column cp = (Column)MemberwiseClone();
                if (cp.pDflt != null)
                    cp.pDflt = pDflt.Copy();
                return cp;
            }
        };

        ///<summary>
        /// A "Collating Sequence" is defined by an instance of the following
        /// structure. Conceptually, a collating sequence consists of a name and
        /// a comparison routine that defines the order of that sequence.
        ///
        /// There may two separate implementations of the collation function, one
        /// that processes text in UTF-8 encoding (CollSeq.xCmp) and another that
        /// processes text encoded in UTF-16 (CollSeq.xCmp16), using the machine
        /// native byte order. When a collation sequence is invoked, SQLite selects
        /// the version that will require the least expensive encoding
        /// translations, if any.
        ///
        /// The CollSeq.pUser member variable is an extra parameter that passed in
        /// as the first argument to the UTF-8 comparison function, xCmp.
        /// CollSeq.pUser16 is the equivalent for the UTF-16 comparison function,
        /// xCmp16.
        ///
        /// If both CollSeq.xCmp and CollSeq.xCmp16 are NULL, it means that the
        /// collating sequence is undefined.  Indices built on an undefined
        /// collating sequence may not be read or written.
        ///
        ///</summary>
        public class CollSeq
        {
            public string zName;
            ///
            ///<summary>
            ///</summary>
            ///<param name="Name of the collating sequence, UTF">8 encoded </param>
            public SqliteEncoding enc;
            ///
            ///<summary>
            ///Text encoding handled by xCmp() 
            ///</summary>
            public CollationType type;
            ///
            ///<summary>
            ///One of the CollationType.... values below 
            ///</summary>
            public object pUser;
            ///<summary>
            ///First argument to xCmp()
            ///</summary>
            public dxCompare xCmp;
            //)(void*,int, const void*, int, const void);
            public dxDelCollSeq xDel;
            //)(void);  /* Destructor for pUser */
            public CollSeq Copy()
            {
                if (this == null)
                    return null;
                else
                {
                    CollSeq cp = (CollSeq)MemberwiseClone();
                    return cp;
                }
            }
        };





    }
}