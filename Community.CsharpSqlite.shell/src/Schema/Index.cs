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
        ///A prefix match is considered OK
        ///</summary>
        ///<summary>
        /// Each SQL index is represented in memory by an
        /// instance of the following structure.
        ///
        /// The columns of the table that are to be indexed are described
        /// by the aiColumn[] field of this structure.  For example, suppose
        /// we have the following table and index:
        ///
        ///     CREATE TABLE Ex1(c1 int, c2 int, c3 text);
        ///     CREATE INDEX Ex2 ON Ex1(c3,c1);
        ///
        /// In the Table structure describing Ex1, nCol==3 because there are
        /// three columns in the table.  In the Index structure describing
        /// Ex2, nColumn==2 since 2 of the 3 columns of Ex1 are indexed.
        /// The value of aiColumn is {2, 0}.  aiColumn[0]==2 because the
        /// first column to be indexed (c3) has an index of 2 in Ex1.aCol[].
        /// The second column to be indexed (c1) has an index of 0 in
        /// Ex1.aCol[], hence Ex2.aiColumn[1]==0.
        ///
        /// The Index.onError field determines whether or not the indexed columns
        /// must be unique and what to do if they are not.  When Index.onError=OE_None,
        /// it means this is not a unique index.  Otherwise it is a unique index
        /// and the value of Index.onError indicate the which conflict resolution
        /// algorithm to employ whenever an attempt is made to insert a non-unique
        /// element.
        ///
        ///</summary>
        public class Index
        {
            public string zName;
            ///
            ///<summary>
            ///Name of this index 
            ///</summary>
            public int nColumn;
            ///
            ///<summary>
            ///Number of columns in the table used by this index 
            ///</summary>
            public int[] aiColumn;
            ///
            ///<summary>
            ///Which columns are used by this index.  1st is 0 
            ///</summary>
            public int[] aiRowEst;
            ///
            ///<summary>
            ///Result of ANALYZE: Est. rows selected by each column 
            ///</summary>
            public Table pTable;
            ///
            ///<summary>
            ///The SQL table being indexed 
            ///</summary>
            public int tnum;
            ///
            ///<summary>
            ///Page containing root of this index in database file 
            ///</summary>
            public u8 onError;
            ///
            ///<summary>
            ///OE_Abort, OE_Ignore, OE_Replace, or OE_None 
            ///</summary>
            public u8 autoIndex;
            ///
            ///<summary>
            ///True if is automatically created (ex: by UNIQUE) 
            ///</summary>
            public u8 bUnordered;
            ///
            ///<summary>
            ///Use this index for == or IN queries only 
            ///</summary>
            public string zColAff;
            ///
            ///<summary>
            ///String defining the affinity of each column 
            ///</summary>
            public Index pNext;
            ///
            ///<summary>
            ///The next index associated with the same table 
            ///</summary>
            public Schema pSchema;
            ///
            ///<summary>
            ///Schema containing this index 
            ///</summary>
            public u8[] aSortOrder;
            ///
            ///<summary>
            ///Array of size Index.nColumn. True==DESC, False==ASC 
            ///</summary>
            public string[] azColl;
            ///<summary>
            ///Array of collation sequence names for index
            ///</summary>
            public IndexSample[] aSample;
            ///
            ///<summary>
            ///Array of SQLITE_INDEX_SAMPLES samples 
            ///</summary>
            public Index Copy()
            {
                if (this == null)
                    return null;
                else
                {
                    Index cp = (Index)MemberwiseClone();
                    return cp;
                }
            }
        };

        ///<summary>
        /// Each sample stored in the sqlite_stat2 table is represented in memory
        /// using a structure of this type.
        ///
        ///</summary>
        public class IndexSample
        {
            public struct _u
            {
                //union {
                public string z;
                ///
                ///<summary>
                ///Value if eType is SQLITE_TEXT 
                ///</summary>
                public byte[] zBLOB;
                ///
                ///<summary>
                ///Value if eType is SQLITE_BLOB 
                ///</summary>
                public double r;
                ///
                ///<summary>
                ///Value if eType is SQLITE_FLOAT or SQLITE_INTEGER 
                ///</summary>
            }
            public _u u;
            public u8 eType;
            ///
            ///<summary>
            ///SQLITE_NULL, SQLITE_INTEGER ... etc. 
            ///</summary>
            public u8 nByte;
            ///
            ///<summary>
            ///Size in byte of text or blob. 
            ///</summary>
        };



    }
}
