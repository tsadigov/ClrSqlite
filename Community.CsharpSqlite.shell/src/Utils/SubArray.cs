using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Community.CsharpSqlite.src.Utils
{
    public class SubArray<T>
    {
        public T[] buffer;
        public int start;
        public int end;
        public void insert(T val, int at){
            for (int j = end; j > at; j--)
            {
                buffer[j + 0] = buffer[j - 1];
            }
            buffer[at]=val;
        }
    }
}
