using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Community.CsharpSqlite.Cache
{
    ///<summary>
    ///Each page cache (or PCache) belongs to a PGroup.  A PGroup is a set
    /// of one or more PCaches that are able to recycle each others unpinned
    /// pages when they are under memory pressure.  A PGroup is an instance of
    /// the following object.
    ///
    /// This page cache implementation works in one of two modes:
    ///
    ///   (1)  Every PCache is the sole member of its own PGroup.  There is
    ///        one PGroup per PCache.
    ///
    ///   (2)  There is a single global PGroup that all PCaches are a member
    ///        of.
    ///
    /// Mode 1 uses more memory (since PCache instances are not able to rob
    /// unused pages from other PCaches) but it also operates without a mutex,
    /// and is therefore often faster.  Mode 2 requires a mutex in order to be
    /// threadsafe, but is able recycle pages more efficient.
    ///
    /// For mode (1), PGroup.mutex is NULL.  For mode (2) there is only a single
    /// PGroup which is the pcache1.grp global variable and its mutex is
    /// SQLITE_MUTEX_STATIC_LRU.
    ///
    ///</summary>
    public class PGroup
    {
        public sqlite3_mutex mutex;

        ///
        ///<summary>
        ///MUTEX_STATIC_LRU or NULL 
        ///</summary>

        public int nMaxPage;

        ///
        ///<summary>
        ///Sum of nMax for purgeable caches 
        ///</summary>

        public int nMinPage;

        ///
        ///<summary>
        ///Sum of nMin for purgeable caches 
        ///</summary>

        public int mxPinned;

        ///
        ///<summary>
        ///</summary>
        ///<param name="nMaxpage + 10 "> nMinPage </param>

        public int nCurrentPage;

        ///
        ///<summary>
        ///Number of purgeable pages allocated 
        ///</summary>

        public PgHdr1 pLruHead, pLruTail;

        ///
        ///<summary>
        ///LRU list of unpinned pages 
        ///</summary>

        // C#
        public PGroup()
        {
            mutex = new sqlite3_mutex();
        }
    };

}
