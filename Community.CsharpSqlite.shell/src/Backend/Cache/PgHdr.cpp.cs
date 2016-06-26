using Community.CsharpSqlite.Paging;
using System.Diagnostics;
using Pgno = System.UInt32;
using u32 = System.UInt32;

namespace Community.CsharpSqlite.Cache
{
    using Utils;
    public partial class PgHdr
    {

        public void RemoveFromDirtyList() {
            this.pCache.RemoveFromDirtyList(this);
        }
        

        ///<summary>
        /// Increment the reference count for page pPg.
        ///</summary>
        public  void PagerAddRef()
        {
            this.PcacheAddRef();
        }

        ///<summary>
        /// Increase the reference count of a supplied page by 1.
        ///
        ///</summary>
        public void PcacheAddRef()
        {
            Debug.Assert(this.ReferenceCount > 0);
            this.ReferenceCount++;
        }


        ///<summary>
        /// Release a page reference.
        ///
        /// If the number of references to the page drop to zero, then the
        /// page is added to the LRU list.  When all references to all pages
        /// are released, a rollback occurs and the lock on the database is
        /// removed.
        ///
        ///</summary>
        public void Unref()
        {
            var pPg = this;
            if (pPg != null)
            {
                pPg.PcacheRelease();
                pPg.pPager.UnlockIfUnused();
            }
        }
        ///<summary>
        /// Decrement the reference count on a page. If the page is clean and the
        /// reference count drops to 0, then it is made elible for recycling.
        ///</summary>
        public void PcacheRelease()
        {
            var p = this;
            Debug.Assert(p.ReferenceCount > 0);
            p.ReferenceCount--;
            if (p.ReferenceCount == 0)
            {
                p.pCache.nRef--;
                if (!p.flags.HasFlag( PGHDR.DIRTY) )
                {
                    PCacheMethods.Unpin(p);
                }
                else
                {
                    MoveToHeadOfDirtyList();
                }
            }
        }

        void MoveToHeadOfDirtyList() {
            RemoveFromDirtyList();
            AddToDirtyList();
        }

        ///<summary>
        /// Add page pPage to the head of the dirty list (PCache1.pDirty is set to
        /// pPage).
        ///
        ///</summary>
        public void AddToDirtyList()
        {
            this.pCache.AddToDirtyList(this);
        }



    }
}
