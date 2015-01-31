using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Community.CsharpSqlite
{
    using Vdbe = Community.CsharpSqlite.Sqlite3.Vdbe;
    using Parse=Community.CsharpSqlite.Sqlite3.Parse;
    

        public static class ParserExtensions {
            ///<summary>
            /// Get a VDBE for the given parser context.  Create a new one if necessary.
            /// If an error occurs, return NULL and leave a message in pParse.
            ///
            ///</summary>
            public static Vdbe sqlite3GetVdbe(this Parse pParse)
            {
                Vdbe v = pParse.pVdbe;
                if (v == null)
                {
                    v = pParse.pVdbe = Sqlite3.Vdbe.Create(pParse.db);
#if !SQLITE_OMIT_TRACE
                    if (v != null)
                    {
                        v.sqlite3VdbeAddOp0(OpCode.OP_Trace);
                    }
#endif
                }
                return v;
            }


            ///<summary>
		/// Add a single OP_Explain instruction to the VDBE to explain a simple
		/// count() query ("SELECT count() FROM pTab").
		///
		///</summary>
		#if !SQLITE_OMIT_EXPLAIN
		public static void explainSimpleCount(this Parse pParse,///
		///<summary>
		///Parse context 
		///</summary>
		Table pTab,///
		///<summary>
		///Table being queried 
		///</summary>
		Index pIdx///
		///<summary>
		///Index used to optimize scan, or NULL 
		///</summary>
		) {
			if(pParse.explain==2) {
				string zEqp=io.sqlite3MPrintf(pParse.db,"SCAN TABLE %s %s%s(~%d rows)",pTab.zName,pIdx!=null?"USING COVERING INDEX ":"",pIdx!=null?pIdx.zName:"",pTab.nRowEst);
                pParse.pVdbe.sqlite3VdbeAddOp4(OpCode.OP_Explain, pParse.iSelectId, 0, 0, zEqp, P4Usage.P4_DYNAMIC);
			}
		}
		#else
																																																		// define explainSimpleCount(a,b,c)
    static void explainSimpleCount(Parse a, Table b, Index c){}
    #endif

        }
}
