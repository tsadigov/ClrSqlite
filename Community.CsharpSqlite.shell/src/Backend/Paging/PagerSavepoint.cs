using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using i16 = System.Int16;
using i64 = System.Int64;
using u8 = System.Byte;
using u16 = System.UInt16;
using u32 = System.UInt32;
using Pgno = System.UInt32;
using sqlite3_int64 = System.Int64;


namespace Community.CsharpSqlite
{
    using DbPage = Paging.PgHdr;
    using Community.CsharpSqlite.Utils;
    ///<summary>
    /// An instance of the following structure is allocated for each active
    /// savepoint and statement transaction in the system. All such structures
    /// are stored in the Pager.aSavepoint[] array, which is allocated and
    /// resized using sqlite3Realloc().
    ///
    /// When a savepoint is created, the PagerSavepoint.iHdrOffset field is
    /// set to 0. If a journal-header is written into the main journal while
    /// the savepoint is active, then iHdrOffset is set to the byte offset
    /// immediately following the last journal record written into the main
    /// journal before the journal-header. This is required during savepoint
    /// rollback (see pagerPlaybackSavepoint()).
    ///
    ///</summary>
    //typedef struct PagerSavepoint PagerSavepoint;
    public class PagerSavepoint
    {
        ///<summary>
        ///Starting offset in main journal 
        ///</summary>
        public i64 iOffset;
        

        ///<summary>
        ///See above 
        ///</summary>
        public i64 iHdrOffset;

        


        ///<summary>
        ///Set of pages in this savepoint 
        ///</summary>
        public Bitvec pInSavepoint;
        
        ///<summary>
        ///Original number of pages in file 
        ///</summary>
        public Pgno nOrig;

        ///
        

        public Pgno iSubRec;

        ///
        ///<summary>
        ///</summary>
        ///<param name="Index of first record in sub">journal </param>

#if !SQLITE_OMIT_WAL
                                                                                                                                                                                                                                                                                                                                                      public u32 aWalData[WAL_SAVEPOINT_NDATA];        /* WAL savepoint context */
#else
        public object aWalData = null;

        ///
        ///<summary>
        ///Used for C# convenience 
        ///</summary>

#endif
        public static implicit operator bool(PagerSavepoint b)
        {
            return (b != null);
        }
    };


}
