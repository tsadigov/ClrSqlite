using System;
using System.Collections.Generic;
using System.Linq;
using i16 = System.Int16;
using i64 = System.Int64;
using sqlite3_int64 = System.Int64;
using u8 = System.Byte;
using u16 = System.UInt16;
using u32 = System.UInt32;
using u64 = System.UInt64;
using unsigned = System.UInt64;
using Pgno = System.UInt32;
using System.Collections;

namespace Community.CsharpSqlite.Utils
{
    public class MyCollection<T> : IList<T> where T :class
    {
        public int Count
        {
            get { return a.Count; }
            set {
                if (value > a.Count)
                {
                    a.AddRange(Enumerable.Range(0, value - a.Count).Select(x => (T)null));
                }
                else
                    a.RemoveRange(value, a.Count - value);
            }
        }
        
        ///
        ///<summary>
        ///Number of tables or subqueries in the FROM clause 
        ///</summary>
        public int nAlloc {
            get {
                return a.Capacity;
            }
            set {
                a.Capacity = value;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        public T this[int index]
        {
            get
            {
                return a[index];
            }

            set
            {
                a[index]=value;
            }
        }

        ///<summary>
        ///Number of entries allocated in a[] below
        ///</summary>
        public List<T> a=new List<T>();
        ///
        ///<summary>
        ///One entry for each identifier on the list 
        ///</summary>

        public void Resize(int n)
        {
            Count = (i16)n;
        }

        public void Add(T item)
        {
            a.Add(item);
        }

        public void Clear()
        {
            a.Clear();
        }

        public bool Contains(T item)
        {
            return a.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            a.CopyTo(array,arrayIndex);
        }

        public bool Remove(T item)
        {
            return a.Remove(item);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return a.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return a.GetEnumerator();
        }

        public int IndexOf(T item)
        {
            return a.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            a.Insert(index,item);
        }

        public void RemoveAt(int index)
        {
            a.RemoveAt(index);
        }
    }
}
