using Community.CsharpSqlite.Utils;
using Pgno = System.UInt32;

namespace Community.CsharpSqlite.Cache
{

    ///<summary>
    /// Each cache entry is represented by an instance of the following
    /// structure. A buffer of PgHdr1.pCache.szPage bytes is allocated
    /// directly before this structure in memory (see the PGHDR1_TO_PAGE()
    /// macro below).
    ///
    ///</summary>
    public class PgHdr1:ILinkedListNode<PgHdr1>
    {
        ///<summary>
        ///Key value (page number) 
        ///</summary>
        public Pgno iKey;

        ///<summary>
        ///Next in hash table chain 
        ///</summary>
        public PgHdr1 pNext { get; set; }

        ///<summary>
        ///Cache that currently owns this page 
        ///</summary>
        public PCache1 pCache;

        ///<summary>
        ///Next in LRU list of unpinned pages 
        ///</summary>
        public PgHdr1 pLruNext;

        ///<summary>
        ///Previous in LRU list of unpinned pages
        ///</summary>
        public PgHdr1 pLruPrev;


        ///<summary>
        ///Pointer to Actual Page Header 
        ///</summary>
        // For C#
        public PgHdr pPgHdr = new PgHdr();
        

        public void Clear()
        {
            this.iKey = 0;
            this.pNext = null;
            this.pCache = null;
            this.pPgHdr.Clear();
        }
    };
}
