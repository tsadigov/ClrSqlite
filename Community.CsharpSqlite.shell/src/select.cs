using System;
using System.Diagnostics;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
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

#if !SQLITE_MAX_VARIABLE_NUMBER
using ynVar = System.Int16;

#else
using ynVar = System.Int32; 
#endif

namespace Community.CsharpSqlite
{
    public partial class Sqlite3
    {



        ///
        ///<summary>
        ///An instance of the following structure contains all information
        ///needed to generate code for a single SELECT statement.
        ///
        ///</summary>
        ///<param name="nLimit is set to ">1 if there is no LIMIT clause.  nOffset is set to 0.</param>
        ///<param name="If there is a LIMIT clause, the parser sets nLimit to the value of the">If there is a LIMIT clause, the parser sets nLimit to the value of the</param>
        ///<param name="limit and nOffset to the value of the offset (or 0 if there is not">limit and nOffset to the value of the offset (or 0 if there is not</param>
        ///<param name="offset).  But later on, nLimit and nOffset become the memory locations">offset).  But later on, nLimit and nOffset become the memory locations</param>
        ///<param name="in the VDBE that record the limit and offset counters.">in the VDBE that record the limit and offset counters.</param>
        ///<param name=""></param>
        ///<param name="addrOpenEphm[] entries contain the address of OP_OpenEphemeral opcodes.">addrOpenEphm[] entries contain the address of OP_OpenEphemeral opcodes.</param>
        ///<param name="These addresses must be stored so that we can go back and fill in">These addresses must be stored so that we can go back and fill in</param>
        ///<param name="the P4_KEYINFO and P2 parameters later.  Neither the KeyInfo nor">the P4_KEYINFO and P2 parameters later.  Neither the KeyInfo nor</param>
        ///<param name="the number of columns in P2 can be computed at the same time">the number of columns in P2 can be computed at the same time</param>
        ///<param name="as the OP_OpenEphm instruction is coded because not">as the OP_OpenEphm instruction is coded because not</param>
        ///<param name="enough information about the compound query is known at that point.">enough information about the compound query is known at that point.</param>
        ///<param name="The KeyInfo for addrOpenTran[0] and [1] contains collating sequences">The KeyInfo for addrOpenTran[0] and [1] contains collating sequences</param>
        ///<param name="for the result set.  The KeyInfo for addrOpenTran[2] contains collating">for the result set.  The KeyInfo for addrOpenTran[2] contains collating</param>
        ///<param name="sequences for the ORDER BY clause.">sequences for the ORDER BY clause.</param>
        ///<param name=""></param>

        public class Select
        {
            public ExprList pEList;

            ///
            ///<summary>
            ///The fields of the result 
            ///</summary>

            public u8 tk_op;

            ///
            ///<summary>
            ///One of: TK_UNION TK_ALL TK_INTERSECT TK_EXCEPT 
            ///</summary>

            public char affinity;

            ///
            ///<summary>
            ///MakeRecord with this affinity for SelectResultType.Set 
            ///</summary>

            public SelectFlags selFlags;

            ///
            ///<summary>
            ///Various SF_* values 
            ///</summary>

            public SrcList pSrc;

            ///
            ///<summary>
            ///The FROM clause 
            ///</summary>

            public Expr pWhere;

            ///
            ///<summary>
            ///The WHERE clause 
            ///</summary>

            public ExprList pGroupBy;

            ///
            ///<summary>
            ///The GROUP BY clause 
            ///</summary>

            public Expr pHaving;

            ///
            ///<summary>
            ///The HAVING clause 
            ///</summary>

            public ExprList pOrderBy;

            ///
            ///<summary>
            ///The ORDER BY clause 
            ///</summary>

            public Select pPrior;

            ///
            ///<summary>
            ///Prior select in a compound select statement 
            ///</summary>

            public Select pNext;

            ///
            ///<summary>
            ///Next select to the left in a compound 
            ///</summary>

            public Select pRightmost;

            ///
            ///<summary>
            ///</summary>
            ///<param name="Right">most select in a compound select statement </param>

            public Expr pLimit;

            ///
            ///<summary>
            ///LIMIT expression. NULL means not used. 
            ///</summary>

            public Expr pOffset;

            ///
            ///<summary>
            ///OFFSET expression. NULL means not used. 
            ///</summary>

            public int iLimit;

            public int iOffset;

            ///
            ///<summary>
            ///Memory registers holding LIMIT & OFFSET counters 
            ///</summary>

            public int[] addrOpenEphm = new int[3];

            ///<summary>
            ///OP_OpenEphem opcodes related to this select
            ///</summary>
            public double nSelectRow;

            ///
            ///<summary>
            ///Estimated number of result rows 
            ///</summary>

            public Select Copy()
            {
                if (this == null)
                    return null;
                else
                {
                    Select cp = (Select)MemberwiseClone();
                    if (pEList != null)
                        cp.pEList = pEList.Copy();
                    if (pSrc != null)
                        cp.pSrc = pSrc.Copy();
                    if (pWhere != null)
                        cp.pWhere = pWhere.Copy();
                    if (pGroupBy != null)
                        cp.pGroupBy = pGroupBy.Copy();
                    if (pHaving != null)
                        cp.pHaving = pHaving.Copy();
                    if (pOrderBy != null)
                        cp.pOrderBy = pOrderBy.Copy();
                    if (pPrior != null)
                        cp.pPrior = pPrior.Copy();
                    if (pNext != null)
                        cp.pNext = pNext.Copy();
                    if (pRightmost != null)
                        cp.pRightmost = pRightmost.Copy();
                    if (pLimit != null)
                        cp.pLimit = pLimit.Copy();
                    if (pOffset != null)
                        cp.pOffset = pOffset.Copy();
                    return cp;
                }
            }

            public void heightOfSelect(ref int pnHeight)
            {
                if (this != null)
                {
                    this.pWhere.heightOfExpr(ref pnHeight);
                    this.pHaving.heightOfExpr(ref pnHeight);
                    this.pLimit.heightOfExpr(ref pnHeight);
                    this.pOffset.heightOfExpr(ref pnHeight);
                    this.pEList.heightOfExprList(ref pnHeight);
                    this.pGroupBy.heightOfExprList(ref pnHeight);
                    this.pOrderBy.heightOfExprList(ref pnHeight);
                    this.pPrior.heightOfSelect(ref pnHeight);
                }
            }

            public int sqlite3SelectExprHeight()
            {
                int nHeight = 0;
                this.heightOfSelect(ref nHeight);
                return nHeight;
            }
        }

        ///<summary>
        /// Allowed values for Select.selFlags.  The "SF" prefix stands for
        /// "Select Flag".
        ///
        ///</summary>
        //#define SF_Distinct        0x0001  /* Output should be DISTINCT */
        //#define SF_Resolved        0x0002  /* Identifiers have been resolved */
        //#define SF_Aggregate       0x0004  /* Contains aggregate functions */
        //#define SF_UsesEphemeral   0x0008  /* Uses the OpenEphemeral opcode */
        //#define SF_Expanded        0x0010  /* sqlite3SelectExpand() called on this */
        //#define SF_HasTypeInfo     0x0020  /* FROM subqueries have Table metadata */
        public enum SelectFlags : ushort
        {
            Distinct = 0x0001,
            ///
            ///<summary>
            ///Output should be DISTINCT 
            ///</summary>

            Resolved = 0x0002,
            ///
            ///<summary>
            ///Identifiers have been resolved 
            ///</summary>

            Aggregate = 0x0004,
            ///
            ///<summary>
            ///Contains aggregate functions 
            ///</summary>

            UsesEphemeral = 0x0008,
            ///
            ///<summary>
            ///Uses the OpenEphemeral opcode 
            ///</summary>

            Expanded = 0x0010,
            ///
            ///<summary>
            ///sqlite3SelectExpand() called on this 
            ///</summary>

            HasTypeInfo = 0x0020
            ///
            ///<summary>
            ///FROM subqueries have Table metadata 
            ///</summary>

        }

        ///
        ///<summary>
        ///The results of a select can be distributed in several ways.  The
        ///"SRT" prefix means "SELECT Result Type".
        ///
        ///</summary>

        public enum SelectResultType
        {
            Union = 1,
            //#define SelectResultType.Union        1  /* Store result as keys in an index */
            Except = 2,
            //#define SelectResultType.Except      2  /* Remove result from a UNION index */
            Exists = 3,
            //#define SelectResultType.Exists      3  /* Store 1 if the result is not empty */
            Discard = 4,
            //#define SelectResultType.Discard    4  /* Do not save the results anywhere */
            ///
            ///<summary>
            ///The ORDER BY clause is ignored for all of the above 
            ///</summary>

            //#define IgnorableOrderby(X) ((X->eDest)<=SelectResultType.Discard)
            Output = 5,
            //#define SelectResultType.Output      5  /* Output each row of result */
            Mem = 6,
            //#define SelectResultType.Mem            6  /* Store result in a memory cell */
            Set = 7,
            //#define SelectResultType.Set            7  /* Store results as keys in an index */
            Table = 8,
            //#define SelectResultType.Table        8  /* Store result as data with an automatic rowid */
            EphemTab = 9,
            //#define SelectResultType.EphemTab  9  /* Create transient tab and store like SelectResultType.Table /
            Coroutine = 10
            //#define SelectResultType.Coroutine   10  /* Generate a single row of result */
        }

        ///<summary>
        /// A structure used to customize the behavior of sqlite3Select(). See
        /// comments above sqlite3Select() for details.
        ///
        ///</summary>
        //typedef struct SelectDest SelectDest;
        public class SelectDest
        {
            public SelectResultType eDest;

            ///
            ///<summary>
            ///How to dispose of the results 
            ///</summary>

            public char affinity;

            ///
            ///<summary>
            ///Affinity used when eDest==SelectResultType.Set 
            ///</summary>

            public int iParm;

            ///
            ///<summary>
            ///A parameter used by the eDest disposal method 
            ///</summary>

            public int iMem;

            ///
            ///<summary>
            ///Base register where results are written 
            ///</summary>

            public int nMem;

            ///
            ///<summary>
            ///Number of registers allocated 
            ///</summary>

            public SelectDest()
            {
                this.eDest = 0;
                this.affinity = '\0';
                this.iParm = 0;
                this.iMem = 0;
                this.nMem = 0;
            }

            public SelectDest(SelectResultType eDest, char affinity, int iParm)
            {
                this.eDest = eDest;
                this.affinity = affinity;
                this.iParm = iParm;
                this.iMem = 0;
                this.nMem = 0;
            }

            public SelectDest(SelectResultType eDest, char affinity, int iParm, int iMem, int nMem)
            {
                this.eDest = eDest;
                this.affinity = affinity;
                this.iParm = iParm;
                this.iMem = iMem;
                this.nMem = nMem;
            }
        };



    }
}
