using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Community.CsharpSqlite
{
    public static class MyLinqExtensions
    {
        public static IEnumerable<T> path<T>(this T start, Func<T, T> prop)
        {
            for(T itr=start;null!=itr;itr=prop(itr))
                yield return itr;
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
