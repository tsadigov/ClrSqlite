
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
using Community.CsharpSqlite.Ast;

#else
using ynVar = System.Int32; 
#endif

namespace Community.CsharpSqlite.Metadata
{


        ///<summary>
        /// information about each column of an SQL table is held in an instance
        /// of this structure.
        ///
        ///</summary>
        public class Column
        {
            ///<summary>
            ///Name of this column 
            ///</summary>

            public string zName;

            ///<summary>
            ///Default value of this column 
            ///</summary>

            public Expr DefaultValue;
            
            ///<summary>
            ///Original text of the default value 
            ///</summary>
            public string DefaultValueSource;

            ///<summary>
            ///Data type for this column 
            ///</summary>

            public string zType;
            
            ///<summary>
            ///Collating sequence.  If NULL, use the default 
            ///</summary>
            public string Collation;

            
            ///<summary>
            ///True if there is a NOT NULL constraint 
            ///</summary>
            public u8 notNull;

            ///<summary>
            ///True if this column is part of the PRIMARY KEY 
            ///</summary>

            public u8 isPrimKey;
            
            ///<summary>
            ///One of the SQLITE_AFF_... values 
            ///</summary>
            public char affinity;
            
#if !SQLITE_OMIT_VIRTUALTABLE
            public u8 isHidden;
            
#endif
            public Column Copy()
            {
                Column cp = (Column)MemberwiseClone();
                if (cp.DefaultValue != null)
                    cp.DefaultValue = DefaultValue.Clone();
                return cp;
            }

            //#  define IsHiddenColumn(X) ((X)->isHidden)
            public bool IsHiddenColumn()
            {
                return this.isHidden != 0;
            }
        };


        ///
        ///<summary>
        ///Allowed values of CollSeq.type:
        ///
        ///</summary>
        public enum CollationType
        {
            BINARY = 1,
            //#define SQLITE_COLL_BINARY  1  /* The default memcmp() collating sequence */
            NOCASE = 2,
            //#define SQLITE_COLL_NOCASE  2  /* The built-in NOCASE collating sequence */
            REVERSE = 3,
            //#define SQLITE_COLL_REVERSE 3  /* The built-in REVERSE collating sequence */
            USER = 0
            //#define SQLITE_COLL_USER    0  /* Any other user-defined collating sequence */
        }

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
            ///<summary>
            ///"Name of the collating sequence, UTF-8 encoded
            ///</summary>
            public string zName;

            ///<summary>
            ///Text encoding handled by xCmp() 
            ///</summary>
            public SqliteEncoding enc;

            ///<summary>
            ///One of the CollationType.... values below 
            ///</summary>
            public CollationType type;
            ///<summary>
            ///First argument to xCmp()
            ///</summary>
            public object pUser;
            
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
