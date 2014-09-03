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
        ///The following are the meanings of bits in the Expr.flags field.
        ///
        ///</summary>

        //#define EP_FromJoin   0x0001  /* Originated in ON or USING clause of a join */
        //#define EP_Agg        0x0002  /* Contains one or more aggregate functions */
        //#define EP_Resolved   0x0004  /* IDs have been resolved to COLUMNs */
        //#define EP_Error      0x0008  /* Expression contains one or more errors */
        //#define EP_Distinct   0x0010  /* Aggregate function with DISTINCT keyword */
        //#define EP_VarSelect  0x0020  /* pSelect is correlated, not constant */
        //#define EP_DblQuoted  0x0040  /* token.z was originally in "..." */
        //#define EP_InfixFunc  0x0080  /* True for an infix function: LIKE, GLOB, etc */
        //#define EP_ExpCollate 0x0100  /* Collating sequence specified explicitly */
        //#define EP_FixedDest  0x0200  /* Result needed in a specific register */
        //#define EP_IntValue   0x0400  /* Integer value contained in u.iValue */
        //#define EP_xIsSelect  0x0800  /* x.pSelect is valid (otherwise x.pList is) */
        //#define EP_Reduced    0x1000  /* Expr struct is EXPR_REDUCEDSIZE bytes only */
        //#define EP_TokenOnly  0x2000  /* Expr struct is EXPR_TOKENONLYSIZE bytes only */
        //#define EP_Static     0x4000  /* Held in memory not obtained from malloc() */
        private const ushort EP_FromJoin = 0x0001;

        private const ushort EP_Agg = 0x0002;

        private const ushort EP_Resolved = 0x0004;

        private const ushort EP_Error = 0x0008;

        private const ushort EP_Distinct = 0x0010;

        private const ushort EP_VarSelect = 0x0020;

        private const ushort EP_DblQuoted = 0x0040;

        private const ushort EP_InfixFunc = 0x0080;

        private const ushort EP_ExpCollate = 0x0100;

        private const ushort EP_FixedDest = 0x0200;

        private const ushort EP_IntValue = 0x0400;

        private const ushort EP_xIsSelect = 0x0800;

        private const ushort EP_Reduced = 0x1000;

        private const ushort EP_TokenOnly = 0x2000;

        private const ushort EP_Static = 0x4000;

        ///
        ///<summary>
        ///The following are the meanings of bits in the Expr.flags2 field.
        ///
        ///</summary>

        //#define EP2_MallocedToken  0x0001  /* Need to sqlite3DbFree() Expr.zToken */
        //#define EP2_Irreducible    0x0002  /* Cannot EXPRDUP_REDUCE this Expr */
        private const u8 EP2_MallocedToken = 0x0001;

        private const u8 EP2_Irreducible = 0x0002;









        ///<summary>
        /// A list of expressions.  Each expression may optionally have a
        /// name.  An expr/name combination can be used in several ways, such
        /// as the list of "expr AS ID" fields following a "SELECT" or in the
        /// list of "ID = expr" items in an UPDATE.  A list of expressions can
        /// also be used as the argument to a function, in which case the a.zName
        /// field is not used.
        ///
        ///</summary>
        public class ExprList_item
        {
            public Expr pExpr;

            ///
            ///<summary>
            ///The list of expressions 
            ///</summary>

            public string zName;

            ///
            ///<summary>
            ///Token associated with this expression 
            ///</summary>

            public string zSpan;

            ///
            ///<summary>
            ///Original text of the expression 
            ///</summary>

            public u8 sortOrder;

            ///
            ///<summary>
            ///1 for DESC or 0 for ASC 
            ///</summary>

            public u8 done;

            ///
            ///<summary>
            ///A flag to indicate when processing is finished 
            ///</summary>

            public u16 iCol;

            ///
            ///<summary>
            ///For ORDER BY, column number in result set 
            ///</summary>

            public u16 iAlias;
            ///
            ///<summary>
            ///Index into Parse.aAlias[] for zName 
            ///</summary>

        }

        public class ExprList
        {
            public int nExpr;

            ///
            ///<summary>
            ///Number of expressions on the list 
            ///</summary>

            public int nAlloc;

            ///
            ///<summary>
            ///Number of entries allocated below 
            ///</summary>

            public int iECursor;

            ///<summary>
            ///VDBE VdbeCursor associated with this ExprList
            ///</summary>
            public ExprList_item[] a;

            ///
            ///<summary>
            ///One entry for each expression 
            ///</summary>

            public ExprList Copy()
            {
                if (this == null)
                    return null;
                else
                {
                    ExprList cp = (ExprList)MemberwiseClone();
                    a.CopyTo(cp.a, 0);
                    return cp;
                }
            }

            public bool referencesOtherTables(///
                ///<summary>
                ///Search expressions in ths list 
                ///</summary>

            WhereMaskSet pMaskSet, ///
                ///<summary>
                ///Mapping from tables to bitmaps 
                ///</summary>

            int iFirst, ///
                ///<summary>
                ///</summary>
                ///<param name="Be searching with the iFirst">th expression </param>

            int iBase///
                ///<summary>
                ///Ignore references to this table 
                ///</summary>

            )
            {
                Bitmask allowed = ~pMaskSet.getMask(iBase);
                while (iFirst < this.nExpr)
                {
                    if ((pMaskSet.exprTableUsage(this.a[iFirst++].pExpr) & allowed) != 0)
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        ///<summary>
        /// An instance of this structure is used by the parser to record both
        /// the parse tree for an expression and the span of input text for an
        /// expression.
        ///
        ///</summary>
        public class ExprSpan
        {
            public Expr pExpr;

            ///
            ///<summary>
            ///The expression parse tree 
            ///</summary>

            public string zStart;

            ///
            ///<summary>
            ///First character of input text 
            ///</summary>

            public string zEnd;

            ///
            ///<summary>
            ///One character past the end of input text 
            ///</summary>

            public void spanSet(Token pStart, Token pEnd)
            {
                this.zStart = pStart.zRestSql;
                this.zEnd = pEnd.zRestSql.Substring(pEnd.Length);
            }

            public void spanExpr(Parse pParse, int op, Token pValue)
            {
                //Log.WriteLine(String.Empty);
                //Log.WriteHeader("spanExpr");
                //Log.WriteLine(pValue.zRestSql);
                //Log.Indent();
                this.pExpr = pParse.sqlite3PExpr(op, 0, 0, pValue);
                this.zStart = pValue.zRestSql;
                this.zEnd = pValue.zRestSql.Substring(pValue.Length);
                //Log.Unindent();
            }

            public void spanBinaryExpr(///
                ///<summary>
                ///Write the result here 
                ///</summary>

            Parse pParse, ///
                ///<summary>
                ///The parsing context.  Errors accumulate here 
                ///</summary>

            int op, ///
                ///<summary>
                ///The binary operation 
                ///</summary>

            ExprSpan pLeft, ///
                ///<summary>
                ///The left operand 
                ///</summary>

            ExprSpan pRight///
                ///<summary>
                ///The right operand 
                ///</summary>

            )
            {
                this.pExpr = pParse.sqlite3PExpr(op, pLeft.pExpr, pRight.pExpr, 0);
                this.zStart = pLeft.zStart;
                this.zEnd = pRight.zEnd;
            }

            public void spanUnaryPostfix(///
                ///<summary>
                ///Write the new expression node here 
                ///</summary>

            Parse pParse, ///
                ///<summary>
                ///Parsing context to record errors 
                ///</summary>

            int op, ///
                ///<summary>
                ///The operator 
                ///</summary>

            ExprSpan pOperand, ///
                ///<summary>
                ///The operand 
                ///</summary>

            Token pPostOp///
                ///<summary>
                ///The operand token for setting the span 
                ///</summary>

            )
            {
                this.pExpr = pParse.sqlite3PExpr(op, pOperand.pExpr, 0, 0);
                this.zStart = pOperand.zStart;
                this.zEnd = pPostOp.zRestSql.Substring(pPostOp.Length);
            }

            public void spanUnaryPrefix(///
                ///<summary>
                ///Write the new expression node here 
                ///</summary>

            Parse pParse, ///
                ///<summary>
                ///Parsing context to record errors 
                ///</summary>

            int op, ///
                ///<summary>
                ///The operator 
                ///</summary>

            ExprSpan pOperand, ///
                ///<summary>
                ///The operand 
                ///</summary>

            Token pPreOp///
                ///<summary>
                ///The operand token for setting the span 
                ///</summary>

            )
            {
                this.pExpr = pParse.sqlite3PExpr(op, pOperand.pExpr, 0, 0);
                this.zStart = pPreOp.zRestSql;
                this.zEnd = pOperand.zEnd;
            }
        }

    }
}
