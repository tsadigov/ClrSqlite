using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Community.CsharpSqlite.Utils
{
    public interface ILinkedListNode<T> where T : ILinkedListNode<T>
    {
        T pNext { get;set; }
    }

    public interface IBackwardLinkedListNode<T> where T : IBackwardLinkedListNode<T>
    {
        T pPrev { get; set; }
    }
}
