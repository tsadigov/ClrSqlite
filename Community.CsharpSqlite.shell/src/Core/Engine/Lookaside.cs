using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using u8 = System.Byte;


namespace Community.CsharpSqlite
{
    ///<summary>
    /// Lookaside malloc is a set of fixed-size buffers that can be used
    /// to satisfy small transient memory allocation requests for objects
    /// associated with a particular database connection.  The use of
    /// lookaside malloc provides a significant performance enhancement
    /// (approx 10%) by avoiding numerous malloc/free requests while parsing
    /// SQL statements.
    ///
    /// The Lookaside structure holds configuration information about the
    /// lookaside malloc subsystem.  Each available memory allocation in
    /// the lookaside subsystem is stored on a linked list of LookasideSlot
    /// objects.
    ///
    /// Lookaside allocations are only allowed for objects that are associated
    /// with a particular database connection.  Hence, schema information cannot
    /// be stored in lookaside because in shared cache mode the schema information
    /// is shared by multiple database connections.  Therefore, while parsing
    /// schema information, the Lookaside.bEnabled flag is cleared so that
    /// lookaside allocations are not used to construct the schema objects.
    ///
    ///</summary>
    public class Lookaside
    {
        public int sz;
        ///
        ///<summary>
        ///Size of each buffer in bytes 
        ///</summary>
        public u8 bEnabled;
        ///
        ///<summary>
        ///False to disable new lookaside allocations 
        ///</summary>
        public bool bMalloced;
        ///
        ///<summary>
        ///True if pStart obtained from sqlite3_malloc() 
        ///</summary>
        public int nOut;
        ///
        ///<summary>
        ///Number of buffers currently checked out 
        ///</summary>
        public int mxOut;
        ///
        ///<summary>
        ///Highwater mark for nOut 
        ///</summary>
        public int[] anStat = new int[3];
        ///
        ///<summary>
        ///0: hits.  1: size misses.  2: full misses 
        ///</summary>
        public LookasideSlot pFree;
        ///
        ///<summary>
        ///List of available buffers 
        ///</summary>
        public int pStart;
        ///
        ///<summary>
        ///First byte of available memory space 
        ///</summary>
        public int pEnd;
        ///
        ///<summary>
        ///First byte past end of available space 
        ///</summary>
    };

    public class LookasideSlot
    {
        public LookasideSlot pNext;
        ///
        ///<summary>
        ///Next buffer in the list of free buffers 
        ///</summary>
    };
}
