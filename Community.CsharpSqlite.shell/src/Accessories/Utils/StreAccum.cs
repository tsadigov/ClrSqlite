using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Bitmask = System.UInt64;
using i16 = System.Int16;
using i64 = System.Int64;
using sqlite3_int64 = System.Int64;
using u8 = System.Byte;
using u16 = System.UInt16;
using u32 = System.UInt32;
using u64 = System.UInt64;
using unsigned = System.UInt64;
using Pgno = System.UInt32;
using System.Diagnostics;
using Community.CsharpSqlite.Engine;
using Community.CsharpSqlite.Utils;

namespace Community.CsharpSqlite.Utils
{
        ///<summary>
        /// An objected used to accumulate the text of a string where we
        /// do not necessarily know how big the string will be in the end.
        ///
        ///</summary>
        public class StrAccum
        {
            public Connection db;
            ///
            ///<summary>
            ///Optional database for lookaside.  Can be NULL 
            ///</summary>
            //public StringBuilder zBase; /* A base allocation.  Not from malloc. */
            public StringBuilder zText;
            ///
            ///<summary>
            ///The string collected so far 
            ///</summary>
            //public int nChar;           /* Length of the string so far */
            //public int nAlloc;          /* Amount of space allocated in zText */
            public int mxAlloc;
            ///
            ///<summary>
            ///Maximum allowed string length 
            ///</summary>
            // Cannot happen under C#
            //public u8 mallocFailed;   /* Becomes true if any memory allocation fails */
            //public u8 useMalloc;        /* 0: none,  1: sqlite3DbMalloc,  2: sqlite3_malloc */
            //public u8 tooBig;           /* Becomes true if string size exceeds limits */
            public Mem Context;
            public StrAccum(int n)
            {
                db = null;
                //zBase = new StringBuilder( n );
                zText = new StringBuilder(n);
                //nChar = 0;
                //nAlloc = n;
                mxAlloc = 0;
                //useMalloc = 0;
                //tooBig = 0;
                Context = null;
            }
            public i64 nChar
            {
                get
                {
                    return zText.Length;
                }
            }
            public bool tooBig
            {
                get
                {
                    return mxAlloc > 0 && zText.Length > mxAlloc;
                }
            }
            public void explainAppendTerm(
                ///The text expression being built 
            int iTerm,
                ///Index of this term.  First is zero 
            string zColumn,
                ///Name of the column 
            string zOp
                ///Name of the operator
            )
            {
                if (iTerm != 0)
                    this.sqlite3StrAccumAppend(" AND ", 5);
                this.sqlite3StrAccumAppend( zColumn, -1);
                this.sqlite3StrAccumAppend(zOp, 1);
                this.sqlite3StrAccumAppend("?", 1);
            }

            ///<summary>
            ///End of function
            ///</summary>
            ///<summary>
            /// Append N bytes of text from z to the StrAccum object.
            ///
            ///</summary>
            public void sqlite3StrAccumAppend( string z, int N)
            {
                StrAccum p = this;
                Debug.Assert(z != null || N == 0);
                if (p.tooBig)//|| p.mallocFailed != 0 )
                {
                    sqliteinth.testcase(p.tooBig);
                    //sqliteinth.testcase( p.mallocFailed );
                    return;
                }
                if (N < 0)
                {
                    N = StringExtensions.sqlite3Strlen30(z);
                }
                if (N == 0 || Sqlite3.NEVER(z == null))
                {
                    return;
                }
                //if( p->nChar+N >= p->nAlloc ){
                //  string zNew;
                //  if( null==p->useMalloc ){
                //    p->tooBig = 1;
                //    N = p->nAlloc - p->nChar - 1;
                //    if( N<=0 ){
                //      return;
                //    }
                //  }else{
                //    string zOld = (p->zText==p->zBase ? 0 : p->zText);
                //    i64 szNew = p->nChar;
                //    szNew += N + 1;
                //    if( szNew > p->mxAlloc ){
                //      sqlite3StrAccumReset(p);
                //      p->tooBig = 1;
                //      return;
                //    }else{
                //      p->nAlloc = (int)szNew;
                //    }
                //    if( p->useMalloc==1 ){
                //      zNew = sqlite3DbRealloc(p->db, zOld, p->nAlloc);
                //    }else{
                //      zNew = sqlite3_realloc(zOld, p->nAlloc);
                //    }
                //    if( zNew ){
                //      if( zOld==0 ) memcpy(zNew, p->zText, p->nChar);
                //      p->zText = zNew;
                //    }else{
                //      p->mallocFailed = 1;
                //      sqlite3StrAccumReset(p);
                //      return;
                //    }
                //  }
                //}
                //memcpy(&p->zText[p->nChar], z, N);
                p.zText.Append(z.Substring(0, N <= z.Length ? N : z.Length));
                //p.nChar += N;
            }

        }


}
