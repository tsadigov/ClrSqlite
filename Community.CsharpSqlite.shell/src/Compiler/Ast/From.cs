using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bitmask = System.UInt64;
using System.Threading.Tasks;
using i16 = System.Int16;
using i64 = System.Int64;
using sqlite3_int64 = System.Int64;
using u8 = System.Byte;
using u16 = System.UInt16;
using u32 = System.UInt32;
using u64 = System.UInt64;
using unsigned = System.UInt64;


namespace Community.CsharpSqlite.Ast
{
    using Metadata;

    ///<summary>
    /// The following structure describes the FROM clause of a SELECT statement.
    /// Each table or subquery in the FROM clause is a separate element of
    /// the SrcList.a[] array.
    ///
    /// With the addition of multiple database support, the following structure
    /// can also be used to describe a particular table such as the table that
    /// is modified by an INSERT, DELETE, or UPDATE statement.  In standard SQL,
    /// such a table must be a simple name: ID.  But in SQLite, the table can
    /// now be identified by a database name, a dot, then the table name: ID.ID.
    ///
    /// The jointype starts out showing the join type between the current table
    /// and the next table on the list.  The parser builds the list this way.
    /// But build.sqlite3SrcListShiftJoinType() later shifts the jointypes so that each
    /// jointype expresses the join between the table and the previous table.
    ///
    /// In the colUsed field, the high-order bit (bit 63) is set if the table
    /// contains more than 63 columns and the 64-th or later column is used.
    ///
    ///</summary>
    public class SrcList_item
    {
        ///<summary>
        ///Name of database holding this table 
        ///</summary>
        public string zDatabase;

        ///<summary>
        ///Name of the table 
        ///</summary>
        public string zName;

        ///<summary>
        ///The "B" part of a "A AS B" phrase.  zName is the "A" 
        ///</summary>
        public string zAlias;

        ///<summary>
        ///An SQL table corresponding to zName 
        ///</summary>
        public Table TableReference;

        public Select pSelect;
        ///
        ///<summary>
        ///A SELECT statement used in place of a table name 
        ///</summary>
        public u8 isPopulated;
        ///
        ///<summary>
        ///Temporary table associated with SELECT is populated 
        ///</summary>
        public JoinType jointype;
        ///
        ///<summary>
        ///Type of join between this able and the previous 
        ///</summary>
        public u8 notIndexed;
        ///
        ///<summary>
        ///True if there is a NOT INDEXED clause 
        ///</summary>
#if !SQLITE_OMIT_EXPLAIN
        public u8 iSelectId;
        ///
        ///<summary>
        ///</summary>
        ///<param name="If pSelect!=0, the id of the sub">select in EQP </param>
#endif
        public int iCursor;
        ///
        ///<summary>
        ///The VDBE cursor number used to access this table 
        ///</summary>
        public Expr pOn;
        ///
        ///<summary>
        ///The ON clause of a join 
        ///</summary>
        public IdList pUsing;
        ///
        ///<summary>
        ///The USING clause of a join 
        ///</summary>
        public Bitmask colUsed;
        ///
        ///<summary>
        ///Bit N (1<<N) set if column N of pTab is used 
        ///</summary>
        public string zIndex;
        ///
        ///<summary>
        ///Identifier from "INDEXED BY <zIndex>" clause 
        ///</summary>
        public Index pIndex;
        ///
        ///<summary>
        ///Index structure corresponding to zIndex, if any 
        ///</summary>
    }


    ///<summary>
    /// Permitted values of the SrcList.a.jointype field
    ///
    ///</summary>
    public enum JoinType : byte
    {
        JT_INNER = 0x0001,
        //#define JT_INNER     0x0001    /* Any kind of inner or cross join */
        JT_CROSS = 0x0002,
        //#define JT_CROSS     0x0002    /* Explicit use of the CROSS keyword */
        JT_NATURAL = 0x0004,
        //#define JT_NATURAL   0x0004    /* True for a "natural" join */
        JT_LEFT = 0x0008,
        //#define JT_LEFT      0x0008    /* Left outer join */
        JT_RIGHT = 0x0010,
        //#define JT_RIGHT     0x0010    /* Right outer join */
        JT_OUTER = 0x0020,
        //#define JT_OUTER     0x0020    /* The "OUTER" keyword is present */
        JT_ERROR = 0x0040
        //#define JT_ERROR     0x0040    /* unknown or unsupported join type */
    }

    ///<summary>
    /// Given 1 to 3 identifiers preceeding the JOIN keyword, determine the
    /// type of join.  Return an integer constant that expresses that type
    /// in terms of the following bit values:
    ///
    ///     JT_INNER
    ///     JT_CROSS
    ///     JT_OUTER
    ///     JT_NATURAL
    ///     JT_LEFT
    ///     JT_RIGHT
    ///
    /// A full outer join is the combination of JT_LEFT and JT_RIGHT.
    ///
    /// If an illegal or unsupported join type is seen, then still return
    /// a join type, but put an error in the pParse structure.
    ///
    ///</summary>
    class Keyword
    {
        ///<summary>
        ///Beginning of keyword text in zKeyText[] 
        ///</summary>
        public u8 i;

        ///<summary>
        ///Length of the keyword in characters 
        ///</summary>
        public u8 nChar;

        ///<summary>
        ///Join type mask 
        ///</summary>
        public JoinType code;

        public Keyword(u8 i, u8 nChar, JoinType code)
        {
            this.i = i;
            this.nChar = nChar;
            this.code = code;
        }
    }

}
