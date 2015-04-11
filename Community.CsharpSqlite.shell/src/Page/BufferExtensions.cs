using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Community.CsharpSqlite
{
    public static class BufferExtensions
    {
        ///<summary>
        /// Read or write a two- and four-byte big-endian integer values.
        ///
        ///</summary>
        //#define get2byte(x)   ((x)[0]<<8 | (x)[1])
        public static UInt16 get2byte(this byte[] p, int offset)
        {
            return (UInt16)(p[offset + 0] << 8 | p[offset + 1]);
        }



        //#define put2byte(p,v) ((p)[0] = (u8)((v)>>8), (p)[1] = (u8)(v))
        public static void put2byte(this byte[] pData, int Offset, UInt32 v)
        {
            pData[Offset + 0] = (byte)(v >> 8);
            pData[Offset + 1] = (byte)v;
        }

        public static void put2byte(this byte[] pData, int Offset, int v)
        {
            pData[Offset + 0] = (byte)(v >> 8);
            pData[Offset + 1] = (byte)v;
        }
    }
}
