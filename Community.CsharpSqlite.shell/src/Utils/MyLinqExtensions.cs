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
