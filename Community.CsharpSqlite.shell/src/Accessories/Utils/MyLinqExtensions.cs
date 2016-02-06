using Community.CsharpSqlite.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Community.CsharpSqlite.Utils
{
    public static class MyLinqExtensions
    {
        public static IEnumerable<TSource> Swap<TSource>(this IEnumerable<TSource> source, int i1,int i2) {
            var arr = source.ToArray();
            var temp = arr[i1];
            arr[i1] = arr[i2];
            arr[i2] = temp;
            return arr;
        }
        public static IEnumerable<T> path<T>(this T start, Func<T, T> prop)
        {
            for(T itr=start;null!=itr;itr=prop(itr))
                yield return itr;
        }

        public static IEnumerable<T> linkedList<T>(this T start) where T:ILinkedListNode<T>
        {
            return start.path(t=>t.pNext);
        }


        public static void ForEach<T>(
            this IEnumerable<T> source,
            Action<T> action)
        {
            ForEach(source, (x, i) => { action(x); return true; });
        }

        public static void ForEach<T>(
            this IEnumerable<T> source,
            Action<T,int> action)
        {
            ForEach(source,(x,i)=> { action(x,i);return true; });
        }
        public static void ForEach<T>(
            this IEnumerable<T> source,
            Func<T,int, bool> action)
        {
            int idx = 0;
            foreach (T element in source)
                if(!action(element,idx++)) break;
        }

        public static void ForEach2<T>(
            this IEnumerable<T> source,
            Func<T,T, int, bool> action)
        {
            int idx = 0;
            var e = source.GetEnumerator();
            if(!e.MoveNext())return;
            
            for(var prev = e.Current; e.MoveNext();prev=e.Current)            
                if (!action(prev, e.Current, idx++)) break;
            
        }

        public static void push<T>(ref T head, T p)where T : IBackwardLinkedListNode<T>
        {
            if (head != null)
            {
                head.pPrev = p;
            }

            head = p;
        }

        public static void RemoveNext<T>(this T current) where T : ILinkedListNode<T>
        {
            current.pNext = current.pNext.pNext;
        }

        public static void removeFromLinkedList<T>(this T itemToRemove, ref T start, Func<T, T> prop, Action<T, T> setter) where T : class
        {

            if (Object.ReferenceEquals(start, itemToRemove))
            {
                start = prop(start);
            }
            else
            {
                ///Justification of Sqlite3.ALWAYS();  The index must be on the list of
                ///indices. 
                var previousIndex = start.path(prop).FirstOrDefault(idx => prop(idx) == itemToRemove);

                if (Sqlite3.ALWAYS(previousIndex != null && prop(previousIndex) == itemToRemove))
                {
                    setter(previousIndex, prop(itemToRemove));
                }
            }
        }
        public static Str AsStr(this String buffer)
        {
            return new Str(buffer, buffer.Strlen30());
        }
        public static Str sub(this String buffer,int len) {
            return new Str(buffer, len);
        }
    }
    public class PathEnumerator<T> : IEnumerator<T> {
        public T Start { get; set; }
        public T Current { get; set; }
        public Func<T, T> Prop;
        
        public void Dispose()
        {
        }

        object System.Collections.IEnumerator.Current
        {
            get { return Current; }
        }

        public bool MoveNext()
        {
            Current = Prop(Current);
            return null!=Current;
        }

        public void Reset()
        {
            Current = Start;
        }
    }
}
