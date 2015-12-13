using Pgno = System.UInt32;

namespace Community.CsharpSqlite.Cache
{
    ///<summary>
    ///Each page cache is an instance of the following object.  Every
    /// open database file (including each in-memory database and each
    /// temporary or transient database) has a single page cache which
    /// is an instance of this object.
    ///
    /// Pointers to structures of this type are cast and returned as
    /// opaque sqlite3_pcache* handles.
    ///
    ///</summary>
    public class PCache1
    {
        ///
        ///<summary>
        ///Cache configuration parameters. Page size (szPage) and the purgeable
        ///flag (bPurgeable) are set when the cache is created. nMax may be 
        ///modified at any time by a call to the pcache1CacheSize() method.
        ///The PGroup mutex must be held when accessing nMax.
        ///
        ///</summary>

        public PGroup pGroup;

        ///
        ///<summary>
        ///PGroup this cache belongs to 
        ///</summary>

        public int szPage;

        ///
        ///<summary>
        ///Size of allocated pages in bytes 
        ///</summary>

        public bool bPurgeable;

        ///
        ///<summary>
        ///True if cache is purgeable 
        ///</summary>

        public int nMin;

        ///
        ///<summary>
        ///Minimum number of pages reserved 
        ///</summary>

        public int nMax;

        ///
        ///<summary>
        ///Configured "cache_size" value 
        ///</summary>

        public int n90pct;

        ///
        ///<summary>
        ///nMax*9/10 
        ///</summary>

        ///
        ///<summary>
        ///Hash table of all pages. The following variables may only be accessed
        ///when the accessor is holding the PGroup mutex.
        ///
        ///</summary>

        public int nRecyclable;

        ///
        ///<summary>
        ///Number of pages in the LRU list 
        ///</summary>

        public int nPage;

        ///
        ///<summary>
        ///Total number of pages in apHash 
        ///</summary>

        public int nHash;

        ///
        ///<summary>
        ///Number of slots in apHash[] 
        ///</summary>

        public PgHdr1[] apHash;

        ///<summary>
        ///Hash table for fast lookup by key
        ///</summary>
        public Pgno iMaxKey;

        ///
        ///<summary>
        ///Largest key seen since xTruncate() 
        ///</summary>

        public void Clear()
        {
            nRecyclable = 0;
            nPage = 0;
            nHash = 0;
            apHash = null;
            iMaxKey = 0;
        }
    };
}
