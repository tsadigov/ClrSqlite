using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using i16 = System.Int16;
using i64 = System.Int64;
using u8 = System.Byte;
using u16 = System.UInt16;
using u32 = System.UInt32;
using u64 = System.UInt64;
using sqlite3_int64 = System.Int64;
using Pgno = System.UInt32;

namespace Community.CsharpSqlite
{
    ///<summary>
    /// An instance of the following structure is used to hold information
    /// about a cell.  The parseCellPtr() function fills in this structure
    /// based on information extract from the raw disk page.
    ///</summary>
    public struct CellInfo
    {
        /// <summary>
        ///Offset to start of cell content  Needed for C# 
        /// </summary>
        public int iCell { get; set; }

        ///<summary>
        ///Pointer to the start of cell content 
        ///</summary>
        public byte[] pCell { get; set; }

        ///<summary>
        ///The key for INTKEY tables, or number of bytes in key 
        ///</summary>
        public i64 nKey;

        ///<summary>
        ///Number of bytes of data 
        ///</summary>
        public u32 nData { get; set; }

        ///<summary>
        ///Total amount of payload 
        ///</summary>
        public u32 nPayload { get; set; }

        ///<summary>
        ///Size of the cell content header in bytes 
        ///</summary>
        public u16 nHeader { get; set; }

        ///<summary>
        ///Amount of payload held locally 
        ///</summary>
        public u16 nLocal { get; set; }

        ///<summary>
        ///Offset to overflow page number.  Zero if no overflow
        ///</summary>
        public u16 iOverflow { get; set; }

        /// <summary>
        ///Size of the cell content on the main b-tree page
        /// </summary>
        public u16 nSize { get; set; }


        public bool Equals(CellInfo ci)
        {
            if (ci.iCell >= ci.pCell.Length || iCell >= this.pCell.Length)
                return false;
            if (ci.pCell[ci.iCell] != this.pCell[iCell])
                return false;
            if (ci.nKey != this.nKey || ci.nData != this.nData || ci.nPayload != this.nPayload)
                return false;
            if (ci.nHeader != this.nHeader || ci.nLocal != this.nLocal)
                return false;
            if (ci.iOverflow != this.iOverflow || ci.nSize != this.nSize)
                return false;
            return true;
        }
    }
}
