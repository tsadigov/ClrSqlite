﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Community.CsharpSqlite
{
    public interface ILinkedListNode<T> where T : ILinkedListNode<T>
    {
        T pNext { get;set; }
    }
}
