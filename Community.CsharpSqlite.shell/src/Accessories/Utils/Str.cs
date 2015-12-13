using Community.CsharpSqlite.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using u32 = System.UInt32;

namespace Community.CsharpSqlite.Utils
{
    /// <summary>
    /// can use underlying buffer as seeral subtrings
    /// </summary>
    public class Str
    {
        public String buffer;
        public int length;
        public Str(String buffer, int length)
        {
            this.buffer=buffer;
            this.length=length;
        }

        public override int GetHashCode()
        {
            return (int)HashExtensions.strHash(buffer,length);
        }

        public u32 Hash {
            get {
                return HashExtensions.strHash(buffer, length);
            }
        }

        public static bool operator ==(Str x, Str y)
        {
            return x.length == y.length && (x.buffer==y.buffer||x.buffer.Equals(y.buffer, StringComparison.InvariantCultureIgnoreCase));
        }
        public static bool operator !=(Str x, Str y)
        {
            return !(x==y);
        }
    }
}
