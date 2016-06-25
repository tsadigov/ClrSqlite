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
    }
}
