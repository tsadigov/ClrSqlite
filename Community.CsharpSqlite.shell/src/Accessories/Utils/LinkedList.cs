using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Community.CsharpSqlite.Utils
{
    public class LinkedList<T> where T :ILinkedListNode<T>, IBackwardLinkedListNode<T>
    {
        public T Head { get; set; }
        public T Tail { get; set; }
        public void Push(T pPage)
        {
            LinkedList<T> pCache = this;
            Debug.Assert(pPage.pNext == null && pPage.pPrev == null && !EqualityComparer<T>.Default.Equals(pCache.Head, pPage));
            pPage.pNext = pCache.Head;
            if (pPage.pNext != null)
            {
                Debug.Assert(pPage.pNext.pPrev== null);
                pPage.pNext.pPrev = pPage;
            }
            pCache.Head = pPage;

            if (null == pCache.Tail)
            {
                pCache.Tail = pPage;
            }
        }
        public void Remove(T pPage)
        {
            if (pPage.pNext != null)
            {
                pPage.pNext.pPrev = pPage.pPrev;
            }
            else
            {
                Debug.Assert(EqualityComparer<T>.Default.Equals(pPage , this.Tail));
                this.Tail = pPage.pPrev;
            }
            if (pPage.pPrev != null)
            {
                pPage.pPrev.pNext = pPage.pNext;
            }
            else
            {
                Debug.Assert(EqualityComparer<T>.Default.Equals(pPage , this.Head));
                this.Head = pPage.pNext;
            }
            pPage.pNext = default(T);
            pPage.pPrev = default(T);
        }
    }
}
