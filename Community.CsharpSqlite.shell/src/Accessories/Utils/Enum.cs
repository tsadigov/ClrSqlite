using Community.CsharpSqlite.Tree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Community.CsharpSqlite.Utils
{
    public static class Enum
    {
        public static IEnumerable<BTreeProp> Range(BTreeProp start, BTreeProp end)
        {
            var l = System.Enum.GetValues(typeof(BTreeProp)).Cast<BTreeProp>();
            return l.Where(x=>x>=start&&x<=end);        
        }
    }
}
