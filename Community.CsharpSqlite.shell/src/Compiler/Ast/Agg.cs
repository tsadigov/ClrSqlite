using Community.CsharpSqlite.Ast;
using Community.CsharpSqlite.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using u8 = System.Byte;

namespace Community.CsharpSqlite
{
    namespace Ast
    {


        ///<summary>
        /// An instance of this structure contains information needed to generate
        /// code for a SELECT that contains aggregate functions.
        ///
        /// If Expr.op==TokenType.TK_AGG_COLUMN or TokenType.TK_AGG_FUNCTION then Expr.pAggInfo is a
        /// pointer to this structure.  The Expr.iColumn field is the index in
        /// AggInfo.aCol[] or AggInfo.aFunc[] of information needed to generate
        /// code for that node.
        ///
        /// AggInfo.pGroupBy and AggInfo.aFunc.pExpr point to fields within the
        /// original Select structure that describes the SELECT statement.  These
        /// fields do not need to be freed when deallocating the AggInfo structure.
        ///
        ///</summary>
        public class AggInfo_col
        {
            ///
            ///<summary>
            ///For each column used in source tables 
            ///</summary>
            public Table pTab;
            ///
            ///<summary>
            ///Source table 
            ///</summary>
            public int iTable;
            ///
            ///<summary>
            ///VdbeCursor number of the source table 
            ///</summary>
            public int iColumn;
            ///
            ///<summary>
            ///Column number within the source table 
            ///</summary>
            public int iSorterColumn;
            ///
            ///<summary>
            ///Column number in the sorting index 
            ///</summary>
            public int iMem;
            ///
            ///<summary>
            ///Memory location that acts as accumulator 
            ///</summary>
            public Expr pExpr;
            ///
            ///<summary>
            ///The original expression 
            ///</summary>
        };

        public class AggInfo_func
        {
            ///
            ///<summary>
            ///For each aggregate function 
            ///</summary>
            public Expr pExpr;
            ///
            ///<summary>
            ///Expression encoding the function 
            ///</summary>
            public FuncDef pFunc;
            ///
            ///<summary>
            ///The aggregate function implementation 
            ///</summary>
            public int iMem;
            ///
            ///<summary>
            ///Memory location that acts as accumulator 
            ///</summary>
            public int iDistinct;
            ///
            ///<summary>
            ///Ephemeral table used to enforce DISTINCT 
            ///</summary>
        }
        public class AggInfo
        {
            public u8 directMode;
            ///
            ///<summary>
            ///Direct rendering mode means take data directly
            ///from source tables rather than from accumulators 
            ///</summary>
            public u8 useSortingIdx;
            ///
            ///<summary>
            ///In direct mode, reference the sorting index rather
            ///than the source table 
            ///</summary>
            public int sortingIdx;
            ///
            ///<summary>
            ///VdbeCursor number of the sorting index 
            ///</summary>
            public ExprList pGroupBy;
            ///
            ///<summary>
            ///The group by clause 
            ///</summary>
            public int nSortingColumn;
            ///
            ///<summary>
            ///Number of columns in the sorting index 
            ///</summary>
            public AggInfo_col[] aCol;
            public int nColumn;
            ///
            ///<summary>
            ///Number of used entries in aCol[] 
            ///</summary>
            public int nColumnAlloc;
            ///
            ///<summary>
            ///Number of slots allocated for aCol[] 
            ///</summary>
            public int nAccumulator;
            ///
            ///<summary>
            ///Number of columns that show through to the output.
            ///Additional columns are used only as parameters to
            ///aggregate functions 
            ///</summary>
            public AggInfo_func[] aFunc;
            public int nFunc;
            ///<summary>
            ///Number of entries in aFunc[]
            ///</summary>
            public int nFuncAlloc;
            ///
            ///<summary>
            ///Number of slots allocated for aFunc[] 
            ///</summary>
            public AggInfo Copy()
            {
                if (this == null)
                    return null;
                else
                {
                    AggInfo cp = (AggInfo)MemberwiseClone();
                    if (pGroupBy != null)
                        cp.pGroupBy = pGroupBy.Copy();
                    return cp;
                }
            }
        };
    }
}
