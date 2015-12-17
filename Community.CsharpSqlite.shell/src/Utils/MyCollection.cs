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

namespace Community.CsharpSqlite.Utils
{
    public class MyCollection<T> where T :class
    {
        public i16 nSrc {
            get { return (i16)a.Count; }
            set {
                if (value > a.Count)
                {
                    a.AddRange(Enumerable.Range(0, value - a.Count).Select(x => (T)null));
                }
                else
                    a.RemoveRange(value,a.Count-value);
            }
        }
        ///
        ///<summary>
        ///Number of tables or subqueries in the FROM clause 
        ///</summary>
        public i16 nAlloc {
            get {
                return (i16)a.Capacity;
            }
            set {
                a.Capacity = value;
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
    }
}
