using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Community.CsharpSqlite.src.Utils
{
    public class SubArray<T> where T : struct
    {
        public byte[] buffer;
        public int start;
        public int end;
        public int itemSize;
        public void insert(T val, int at){
            for (int j = end; j > at; j--)
            {
                buffer[j + 0] = buffer[j - itemSize];
            }
            this[at] = val;
        }

        public T this[int index]   // long is a 64-bit integer
        {
            // Read one byte at offset index and return it.
            get
            {
                byte[] buffer = new byte[itemSize];
                return default(T);
            }
            // Write one byte at offset index and return it.
            set
            {
                
            }
        }
    }
}
