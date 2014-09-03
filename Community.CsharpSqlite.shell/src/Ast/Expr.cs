#define SQLITE_MAX_EXPR_DEPTH

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



        ///<summary>
        ///Macros to determine the number of bytes required by a normal Expr
        ///struct, an Expr struct with the EP_Reduced flag set in Expr.flags
        ///and an Expr struct with the EP_TokenOnly flag set.
        ///
        ///</summary>
        //#define EXPR_FULLSIZE           sizeof(Expr)           /* Full size */
        //#define EXPR_REDUCEDSIZE        offsetof(Expr,iTable)  /* Common features */
        //#define EXPR_TOKENONLYSIZE      offsetof(Expr,pLeft)   /* Fewer features */
        // We don't use these in C#, but define them anyway,
        private const int EXPR_FULLSIZE = 48;
        private const int EXPR_REDUCEDSIZE = 24;
        private const int EXPR_TOKENONLYSIZE = 8;
        ///<summary>
        /// Flags passed to the sqlite3ExprDup() function. See the header comment
        /// above sqlite3ExprDup() for details.
        ///
        ///</summary>
        //#define EXPRDUP_REDUCE         0x0001  /* Used reduced-size Expr nodes */
        private const int EXPRDUP_REDUCE = 0x0001;















        /**
    ** Each node of an expression in the parse tree is an instance
    ** of this structure.
    **
    ** Expr.op is the opcode.  The integer parser token codes are reused
    ** as opcodes here.  For example, the parser defines TK_GE to be an integer
    ** code representing the ">=" operator.  This same integer code is reused
    ** to represent the greater-than-or-equal-to operator in the expression
    ** tree.
    **
    ** If the expression is an SQL literal (TK_INTEGER, TK_FLOAT, TK_BLOB,
    ** or TK_STRING), then Expr.token contains the text of the SQL literal. If
    ** the expression is a variable (TK_VARIABLE), then Expr.token contains the
    ** variable name. Finally, if the expression is an SQL function (TK_FUNCTION),
    ** then Expr.token contains the name of the function.
    **
    ** Expr.pRight and Expr.pLeft are the left and right subexpressions of a
    ** binary operator. Either or both may be NULL.
    **
    ** Expr.x.pList is a list of arguments if the expression is an SQL function,
    ** a CASE expression or an IN expression of the form "<lhs> IN (<y>, <z>...)".
    ** Expr.x.pSelect is used if the expression is a sub-select or an expression of
    ** the form "<lhs> IN (SELECT ...)". If the EP_xIsSelect bit is set in the
    ** Expr.flags mask, then Expr.x.pSelect is valid. Otherwise, Expr.x.pList is
    ** valid.
    **
    ** An expression of the form ID or ID.ID refers to a column in a table.
    ** For such expressions, Expr.op is set to TK_COLUMN and Expr.iTable is
    ** the integer cursor number of a VDBE cursor pointing to that table and
    ** Expr.iColumn is the column number for the specific column.  If the
    ** expression is used as a result in an aggregate SELECT, then the
    ** value is also stored in the Expr.iAgg column in the aggregate so that
    ** it can be accessed after all aggregates are computed.
    **
    ** If the expression is an unbound variable marker (a question mark
    ** character '?' in the original SQL) then the Expr.iTable holds the index
    ** number for that variable.
    **
    ** If the expression is a subquery then Expr.iColumn holds an integer
    ** register number containing the result of the subquery.  If the
    ** subquery gives a constant result, then iTable is -1.  If the subquery
    ** gives a different answer at different times during statement processing
    ** then iTable is the address of a subroutine that computes the subquery.
    **
    ** If the Expr is of type OP_Column, and the table it is selecting from
    ** is a disk table or the "old.*" pseudo-table, then pTab points to the
    ** corresponding table definition.
    **
    ** ALLOCATION NOTES:
    **
    ** Expr objects can use a lot of memory space in database schema.  To
    ** help reduce memory requirements, sometimes an Expr object will be
    ** truncated.  And to reduce the number of memory allocations, sometimes
    ** two or more Expr objects will be stored in a single memory allocation,
    ** together with Expr.zToken strings.
    **
    ** If the EP_Reduced and EP_TokenOnly flags are set when
    ** an Expr object is truncated.  When EP_Reduced is set, then all
    ** the child Expr objects in the Expr.pLeft and Expr.pRight subtrees
    ** are contained within the same memory allocation.  Note, however, that
    ** the subtrees in Expr.x.pList or Expr.x.pSelect are always separately
    ** allocated, regardless of whether or not EP_Reduced is set.
    */

        public class Expr
        {
            public Expr()
            {
            }
#if DEBUG_CLASS_EXPR || DEBUG_CLASS_ALL
																																																																																										public u8 _op;                      /* Operation performed by this node */
public u8 op
{
get { return _op; }
set { _op = value; }
}
#else
            TokenType _op;
            public TokenType Operator
            {
                get
                {
                    return _op;
                }
                set
                {
                    _op = value;
                }
            }
            public u8 op
            {
                get
                {
                    return (u8)_op;
                }
                set
                {
                    _op = (TokenType)value;
                }
            }
            ///
            ///<summary>
            ///Operation performed by this node 
            ///</summary>
#endif
            public char affinity;
            ///<summary>
            ///The affinity of the column or 0 if not a column
            ///</summary>
#if DEBUG_CLASS_EXPR || DEBUG_CLASS_ALL
																																																																																										public u16 _flags;                            /* Various flags.  EP_* See below */
public u16 flags
{
get { return _flags; }
set { _flags = value; }
}
public struct _u
{
public string _zToken;         /* Token value. Zero terminated and dequoted */
public string zToken
{
get { return _zToken; }
set { _zToken = value; }
}
public int iValue;            /* Non-negative integer value if EP_IntValue */
}

#else
            public struct _u
            {
                public string zToken;
                ///
                ///<summary>
                ///Token value. Zero terminated and dequoted 
                ///</summary>
                public int iValue;
                ///
                ///<summary>
                ///</summary>
                ///<param name="Non">negative integer value if EP_IntValue </param>
            }
            public u16 flags;
            ///
            ///<summary>
            ///Various flags.  EP_* See below 
            ///</summary>
#endif
            public _u u;
            ///
            ///<summary>
            ///If the EP_TokenOnly flag is set in the Expr.flags mask, then no
            ///space is allocated for the fields below this point. An attempt to
            ///access them will result in a segfault or malfunction.
            ///
            ///</summary>
            public Expr pLeft;
            ///<summary>
            ///Left subnode
            ///</summary>
            public Expr pRight;
            ///
            ///<summary>
            ///Right subnode 
            ///</summary>
            public struct _x
            {
                public ExprList pList;
                ///
                ///<summary>
                ///</summary>
                ///<param name="Function arguments or in "<expr> IN (<expr">list)" </param>
                public Select pSelect;
                ///
                ///<summary>
                ///</summary>
                ///<param name="Used for sub">selects and "<expr> IN (<select>)" </param>
            }
            public _x x;
            public CollSeq pColl;
            ///
            ///<summary>
            ///The collation type of the column or 0 
            ///</summary>
            ///
            ///<summary>
            ///If the EP_Reduced flag is set in the Expr.flags mask, then no
            ///space is allocated for the fields below this point. An attempt to
            ///access them will result in a segfault or malfunction.
            ///
            ///</summary>
            public int iTable;
            ///
            ///<summary>
            ///TK_COLUMN: cursor number of table holding column
            ///TK_REGISTER: register number
            ///</summary>
            ///<param name="TK_TRIGGER: 1 ">> old </param>
            public ynVar iColumn;
            ///
            ///<summary>
            ///</summary>
            ///<param name="TK_COLUMN: column index.  ">1 for rowid.</param>
            ///<param name="TK_VARIABLE: variable number (always >= 1). ">TK_VARIABLE: variable number (always >= 1). </param>
            public i16 iAgg;
            ///
            ///<summary>
            ///</summary>
            ///<param name="Which entry in pAggInfo">>aFunc[] </param>
            public i16 iRightJoinTable;
            ///
            ///<summary>
            ///If EP_FromJoin, the right table of the join 
            ///</summary>
            public u8 flags2;
            ///
            ///<summary>
            ///Second set of flags.  EP2_... 
            ///</summary>
            public u8 op2;
            ///
            ///<summary>
            ///If a TK_REGISTER, the original value of Expr.op 
            ///</summary>
            public AggInfo pAggInfo;
            ///
            ///<summary>
            ///Used by TK_AGG_COLUMN and TK_AGG_FUNCTION 
            ///</summary>
            public Table pTab;
            ///
            ///<summary>
            ///Table for TK_COLUMN expressions. 
            ///</summary>
#if SQLITE_MAX_EXPR_DEPTH
			public int nHeight;
			///
			///<summary>
			///Height of the tree headed by this node 
			///</summary>
			public Table pZombieTab;
			///<summary>
			///List of Table objects to delete after code gen
			///</summary>
#endif
#if DEBUG_CLASS
																																																																																										public int op
{
get { return _op; }
set { _op = value; }
}
#endif
            public void CopyFrom(Expr cf)
            {
                op = cf.op;
                affinity = cf.affinity;
                flags = cf.flags;
                u = cf.u;
                pColl = cf.pColl == null ? null : cf.pColl.Copy();
                iTable = cf.iTable;
                iColumn = cf.iColumn;
                pAggInfo = cf.pAggInfo == null ? null : cf.pAggInfo.Copy();
                iAgg = cf.iAgg;
                iRightJoinTable = cf.iRightJoinTable;
                flags2 = cf.flags2;
                pTab = cf.pTab == null ? null : cf.pTab;
#if SQLITE_TEST || SQLITE_MAX_EXPR_DEPTH
				nHeight=cf.nHeight;
				pZombieTab=cf.pZombieTab;
#endif
                pLeft = cf.pLeft == null ? null : cf.pLeft.Copy();
                pRight = cf.pRight == null ? null : cf.pRight.Copy();
                x.pList = cf.x.pList == null ? null : cf.x.pList.Copy();
                x.pSelect = cf.x.pSelect == null ? null : cf.x.pSelect.Copy();
            }
            public Expr Copy()
            {
                if (this == null)
                    return null;
                else
                    return Copy(flags);
            }
            public Expr Copy(int flag)
            {
                Expr cp = new Expr();
                cp.op = op;
                cp.affinity = affinity;
                cp.flags = flags;
                cp.u = u;
                if ((flag & EP_TokenOnly) != 0)
                    return cp;
                if (pLeft != null)
                    cp.pLeft = pLeft.Copy();
                if (pRight != null)
                    cp.pRight = pRight.Copy();
                cp.x = x;
                cp.pColl = pColl;
                if ((flag & EP_Reduced) != 0)
                    return cp;
                cp.iTable = iTable;
                cp.iColumn = iColumn;
                cp.iAgg = iAgg;
                cp.iRightJoinTable = iRightJoinTable;
                cp.flags2 = flags2;
                cp.op2 = op2;
                cp.pAggInfo = pAggInfo;
                cp.pTab = pTab;
#if SQLITE_MAX_EXPR_DEPTH
				cp.nHeight=nHeight;
				cp.pZombieTab=pZombieTab;
#endif
                return cp;
            }
            public///<summary>
                /// pExpr is an operand of a comparison operator.  aff2 is the
                /// type affinity of the other operand.  This routine returns the
                /// type affinity that should be used for the comparison operator.
                ///
                ///</summary>
            char sqlite3CompareAffinity(char aff2)
            {
                char aff1 = this.sqlite3ExprAffinity();
                if (aff1 != '\0' && aff2 != '\0')
                {
                    ///
                    ///<summary>
                    ///Both sides of the comparison are columns. If one has numeric
                    ///affinity, use that. Otherwise use no affinity.
                    ///
                    ///</summary>
                    if (aff1 >= SQLITE_AFF_NUMERIC || aff2 >= SQLITE_AFF_NUMERIC)//        if (sqlite3IsNumericAffinity(aff1) || sqlite3IsNumericAffinity(aff2))
                    {
                        return SQLITE_AFF_NUMERIC;
                    }
                    else
                    {
                        return SQLITE_AFF_NONE;
                    }
                }
                else
                    if (aff1 == '\0' && aff2 == '\0')
                    {
                        ///
                        ///<summary>
                        ///Neither side of the comparison is a column.  Compare the
                        ///results directly.
                        ///
                        ///</summary>
                        return SQLITE_AFF_NONE;
                    }
                    else
                    {
                        ///
                        ///<summary>
                        ///One side is a column, the other is not. Use the columns affinity. 
                        ///</summary>
                        Debug.Assert(aff1 == 0 || aff2 == 0);
                        return (aff1 != '\0' ? aff1 : aff2);
                    }
            }
            public///<summary>
                /// pExpr is a comparison operator.  Return the type affinity that should
                /// be applied to both operands prior to doing the comparison.
                ///
                ///</summary>
            char comparisonAffinity()
            {
                char aff;
                Debug.Assert(this.op == TK_EQ || this.op == TK_IN || this.op == TK_LT || this.op == TK_GT || this.op == TK_GE || this.op == TK_LE || this.op == TK_NE || this.op == TK_IS || this.op == TK_ISNOT);
                Debug.Assert(this.pLeft != null);
                aff = this.pLeft.sqlite3ExprAffinity();
                if (this.pRight != null)
                {
                    aff = this.pRight.sqlite3CompareAffinity(aff);
                }
                else
                    if (this.ExprHasProperty(EP_xIsSelect))
                    {
                        aff = this.x.pSelect.pEList.a[0].pExpr.sqlite3CompareAffinity(aff);
                    }
                    else
                        if (aff == '\0')
                        {
                            aff = SQLITE_AFF_NONE;
                        }
                return aff;
            }
            public///<summary>
                /// pExpr is a comparison expression, eg. '=', '<', IN(...) etc.
                /// idx_affinity is the affinity of an indexed column. Return true
                /// if the index with affinity idx_affinity may be used to implement
                /// the comparison in pExpr.
                ///
                ///</summary>
            bool sqlite3IndexAffinityOk(char idx_affinity)
            {
                char aff = this.comparisonAffinity();
                switch (aff)
                {
                    case SQLITE_AFF_NONE:
                        return true;
                    case SQLITE_AFF_TEXT:
                        return idx_affinity == SQLITE_AFF_TEXT;
                    default:
                        return idx_affinity >= SQLITE_AFF_NUMERIC;
                    // sqlite3IsNumericAffinity(idx_affinity);
                }
            }
            public///<summary>
                /// The dupedExpr*Size() routines each return the number of bytes required
                /// to store a copy of an expression or expression tree.  They differ in
                /// how much of the tree is measured.
                ///
                ///     dupedExprStructSize()     Size of only the Expr structure
                ///     dupedExprNodeSize()       Size of Expr + space for token
                ///     dupedExprSize()           Expr + token + subtree components
                ///
                ///
                ///
                /// The dupedExprStructSize() function returns two values OR-ed together:
                /// (1) the space required for a copy of the Expr structure only and
                /// (2) the EP_xxx flags that indicate what the structure size should be.
                /// The return values is always one of:
                ///
                ///      EXPR_FULLSIZE
                ///      EXPR_REDUCEDSIZE   | EP_Reduced
                ///      EXPR_TOKENONLYSIZE | EP_TokenOnly
                ///
                /// The size of the structure can be found by masking the return value
                /// of this routine with 0xfff.  The flags can be found by masking the
                /// return value with EP_Reduced|EP_TokenOnly.
                ///
                /// Note that with flags==EXPRDUP_REDUCE, this routines works on full-size
                /// (unreduced) Expr objects as they or originally constructed by the parser.
                /// During expression analysis, extra information is computed and moved into
                /// later parts of teh Expr object and that extra information might get chopped
                /// off if the expression is reduced.  Note also that it does not work to
                /// make a EXPRDUP_REDUCE copy of a reduced expression.  It is only legal
                /// to reduce a pristine expression tree from the parser.  The implementation
                /// of dupedExprStructSize() contain multiple Debug.Assert() statements that attempt
                /// to enforce this constraint.
                ///
                ///</summary>
            int dupedExprStructSize(int flags)
            {
                int nSize;
                Debug.Assert(flags == EXPRDUP_REDUCE || flags == 0);
                ///
                ///<summary>
                ///Only one flag value allowed 
                ///</summary>
                if (0 == (flags & EXPRDUP_REDUCE))
                {
                    nSize = EXPR_FULLSIZE;
                }
                else
                {
                    Debug.Assert(!this.ExprHasAnyProperty(EP_TokenOnly | EP_Reduced));
                    Debug.Assert(!this.ExprHasProperty(EP_FromJoin));
                    Debug.Assert((this.flags2 & EP2_MallocedToken) == 0);
                    Debug.Assert((this.flags2 & EP2_Irreducible) == 0);
                    if (this.pLeft != null || this.pRight != null || this.pColl != null || this.x.pList != null || this.x.pSelect != null)
                    {
                        nSize = EXPR_REDUCEDSIZE | EP_Reduced;
                    }
                    else
                    {
                        nSize = EXPR_TOKENONLYSIZE | EP_TokenOnly;
                    }
                }
                return nSize;
            }
            public///<summary>
                /// Return the number of bytes required to create a duplicate of the
                /// expression passed as the first argument. The second argument is a
                /// mask containing EXPRDUP_XXX flags.
                ///
                /// The value returned includes space to create a copy of the Expr struct
                /// itself and the buffer referred to by Expr.u.zToken, if any.
                ///
                /// If the EXPRDUP_REDUCE flag is set, then the return value includes
                /// space to duplicate all Expr nodes in the tree formed by Expr.pLeft
                /// and Expr.pRight variables (but not for any structures pointed to or
                /// descended from the Expr.x.pList or Expr.x.pSelect variables).
                ///
                ///</summary>
            int dupedExprSize(int flags)
            {
                int nByte = 0;
                if (this != null)
                {
                    nByte = this.dupedExprNodeSize(flags);
                    if ((flags & EXPRDUP_REDUCE) != 0)
                    {
                        nByte += this.pLeft.dupedExprSize(flags) + this.pRight.dupedExprSize(flags);
                    }
                }
                return nByte;
            }
            public///<summary>
                /// This function returns the space in bytes required to store the copy
                /// of the Expr structure and a copy of the Expr.u.zToken string (if that
                /// string is defined.)
                ///
                ///</summary>
            int dupedExprNodeSize(int flags)
            {
                int nByte = this.dupedExprStructSize(flags) & 0xfff;
                if (!this.ExprHasProperty(EP_IntValue) && this.u.zToken != null)
                {
                    nByte += StringExtensions.sqlite3Strlen30(this.u.zToken) + 1;
                }
                return ROUND8(nByte);
            }
            public int exprIsConst(int initFlag)
            {
                Walker w = new Walker();
                w.u.i = initFlag;
                w.xExprCallback = exprNodeIsConstant;
                w.xSelectCallback = selectNodeIsConstant;
                Expr _this = this;
                w.sqlite3WalkExpr(ref _this);
                return w.u.i;
            }
            public///<summary>
                /// Walk an expression tree.  Return 1 if the expression is constant
                /// and 0 if it involves variables or function calls.
                ///
                /// For the purposes of this function, a double-quoted string (ex: "abc")
                /// is considered a variable but a single-quoted string (ex: 'abc') is
                /// a constant.
                ///
                ///</summary>
            int sqlite3ExprIsConstant()
            {
                return this.exprIsConst(1);
            }
            public///<summary>
                /// Walk an expression tree.  Return 1 if the expression is constant
                /// that does no originate from the ON or USING clauses of a join.
                /// Return 0 if it involves variables or function calls or terms from
                /// an ON or USING clause.
                ///
                ///</summary>
            int sqlite3ExprIsConstantNotJoin()
            {
                return this.exprIsConst(3);
            }
            public///<summary>
                /// Walk an expression tree.  Return 1 if the expression is constant
                /// or a function call with constant arguments.  Return and 0 if there
                /// are any variables.
                ///
                /// For the purposes of this function, a double-quoted string (ex: "abc")
                /// is considered a variable but a single-quoted string (ex: 'abc') is
                /// a constant.
                ///
                ///</summary>
            int sqlite3ExprIsConstantOrFunction()
            {
                return this.exprIsConst(2);
            }
            public///<summary>
                /// If the expression p codes a constant integer that is small enough
                /// to fit in a 32-bit integer, return 1 and put the value of the integer
                /// in pValue.  If the expression is not an integer or if it is too big
                /// to fit in a signed 32-bit integer, return 0 and leave pValue unchanged.
                ///
                ///</summary>
            int sqlite3ExprIsInteger(ref int pValue)
            {
                int rc = 0;
                ///
                ///<summary>
                ///</summary>
                ///<param name="If an expression is an integer literal that fits in a signed 32">bit</param>
                ///<param name="integer, then the EP_IntValue flag will have already been set ">integer, then the EP_IntValue flag will have already been set </param>
                Debug.Assert(this.op != TK_INTEGER || (this.flags & EP_IntValue) != 0 || !Converter.sqlite3GetInt32(this.u.zToken, ref rc));
                if ((this.flags & EP_IntValue) != 0)
                {
                    pValue = (int)this.u.iValue;
                    return 1;
                }
                switch (this.op)
                {
                    case TK_UPLUS:
                        {
                            rc = this.pLeft.sqlite3ExprIsInteger(ref pValue);
                            break;
                        }
                    case TK_UMINUS:
                        {
                            int v = 0;
                            if (this.pLeft.sqlite3ExprIsInteger(ref v) != 0)
                            {
                                pValue = -v;
                                rc = 1;
                            }
                            break;
                        }
                    default:
                        break;
                }
                return rc;
            }
            public///<summary>
                /// Return FALSE if there is no chance that the expression can be NULL.
                ///
                /// If the expression might be NULL or if the expression is too complex
                /// to tell return TRUE.
                ///
                /// This routine is used as an optimization, to skip OP_IsNull opcodes
                /// when we know that a value cannot be NULL.  Hence, a false positive
                /// (returning TRUE when in fact the expression can never be NULL) might
                /// be a small performance hit but is otherwise harmless.  On the other
                /// hand, a false negative (returning FALSE when the result could be NULL)
                /// will likely result in an incorrect answer.  So when in doubt, return
                /// TRUE.
                ///
                ///</summary>
            int sqlite3ExprCanBeNull()
            {
                u8 op;
                Expr expr = this;
                while (expr.op == TK_UPLUS || expr.op == TK_UMINUS)
                {
                    expr = expr.pLeft;
                }
                op = expr.op;
                if (op == TK_REGISTER)
                    op = expr.op2;
                switch (op)
                {
                    case TK_INTEGER:
                    case TK_STRING:
                    case TK_FLOAT:
                    case TK_BLOB:
                        return 0;
                    default:
                        return 1;
                }
            }
            public///<summary>
                /// Return TRUE if pExpr is an constant expression that is appropriate
                /// for factoring out of a loop.  Appropriate expressions are:
                ///
                ///    *  Any expression that evaluates to two or more opcodes.
                ///
                ///    *  Any OP_Integer, OP_Real, OP_String, OP_Blob, OP_Null,
                ///       or OP_Variable that does not need to be placed in a
                ///       specific register.
                ///
                /// There is no point in factoring out single-instruction constant
                /// expressions that need to be placed in a particular register.
                /// We could factor them out, but then we would end up adding an
                /// OP_SCopy instruction to move the value into the correct register
                /// later.  We might as well just use the original instruction and
                /// avoid the OP_SCopy.
                ///
                ///</summary>
            int isAppropriateForFactoring()
            {
                Expr expr = this;
                if (expr.sqlite3ExprIsConstantNotJoin() == 0)
                {
                    return 0;
                    ///
                    ///<summary>
                    ///Only constant expressions are appropriate for factoring 
                    ///</summary>
                }
                if ((expr.flags & EP_FixedDest) == 0)
                {
                    return 1;
                    ///
                    ///<summary>
                    ///Any constant without a fixed destination is appropriate 
                    ///</summary>
                }
                while (expr.op == TK_UPLUS)
                    expr = expr.pLeft;
                switch (expr.op)
                {
#if !SQLITE_OMIT_BLOB_LITERAL
                    case TK_BLOB:
#endif
                    case TK_VARIABLE:
                    case TK_INTEGER:
                    case TK_FLOAT:
                    case TK_NULL:
                    case TK_STRING:
                        {
                            testcase(expr.op == TK_BLOB);
                            testcase(expr.op == TK_VARIABLE);
                            testcase(expr.op == TK_INTEGER);
                            testcase(expr.op == TK_FLOAT);
                            testcase(expr.op == TK_NULL);
                            testcase(expr.op == TK_STRING);
                            ///
                            ///<summary>
                            ///</summary>
                            ///<param name="Single">instruction constants with a fixed destination are</param>
                            ///<param name="better done in">line.  If we factor them, they will just end</param>
                            ///<param name="up generating an OP_SCopy to move the value to the destination">up generating an OP_SCopy to move the value to the destination</param>
                            ///<param name="register. ">register. </param>
                            return 0;
                        }
                    case TK_UMINUS:
                        {
                            if (expr.pLeft.op == TK_FLOAT || expr.pLeft.op == TK_INTEGER)
                            {
                                return 0;
                            }
                            break;
                        }
                    default:
                        {
                            break;
                        }
                }
                return 1;
            }
            public char sqlite3ExprAffinity()
            {
                int op = this.op;
                if (op == TK_SELECT)
                {
                    Debug.Assert((this.flags & EP_xIsSelect) != 0);
                    return this.x.pSelect.pEList.a[0].pExpr.sqlite3ExprAffinity();
                }
#if !SQLITE_OMIT_CAST
                if (op == TK_CAST)
                {
                    Debug.Assert(!this.ExprHasProperty(EP_IntValue));
                    return sqlite3AffinityType(this.u.zToken);
                }
#endif
                if ((op == TK_AGG_COLUMN || op == TK_COLUMN || op == TK_REGISTER) && this.pTab != null)
                {
                    ///
                    ///<summary>
                    ///op==TK_REGISTER && pExpr.pTab!=0 happens when pExpr was originally
                    ///a TK_COLUMN but was previously evaluated and cached in a register 
                    ///</summary>
                    int j = this.iColumn;
                    if (j < 0)
                        return SQLITE_AFF_INTEGER;
                    Debug.Assert(this.pTab != null && j < this.pTab.nCol);
                    return this.pTab.aCol[j].affinity;
                }
                return this.affinity;
            }
            public Expr sqlite3ExprSetColl(CollSeq pColl)
            {
                if (this != null && pColl != null)
                {
                    this.pColl = pColl;
                    this.flags |= EP_ExpCollate;
                }
                return this;
            }
            public u8 binaryCompareP5(Expr pExpr2, int jumpIfNull)
            {
                u8 aff = (u8)pExpr2.sqlite3ExprAffinity();
                aff = (u8)((u8)this.sqlite3CompareAffinity((char)aff) | (u8)jumpIfNull);
                return aff;
            }
            public void exprSetHeight()
            {
                int nHeight = 0;
                this.pLeft.heightOfExpr(ref nHeight);
                this.pRight.heightOfExpr(ref nHeight);
                if (this.ExprHasProperty(EP_xIsSelect))
                {
                    this.x.pSelect.heightOfSelect(ref nHeight);
                }
                else
                {
                    this.x.pList.heightOfExprList(ref nHeight);
                }
                this.nHeight = nHeight + 1;
            }
            public int isMatchOfColumn(///
                ///<summary>
                ///Test this expression 
                ///</summary>
            )
            {
                ExprList pList;
                if (this.op != TK_FUNCTION)
                {
                    return 0;
                }
                if (!this.u.zToken.Equals("match", StringComparison.InvariantCultureIgnoreCase))
                {
                    return 0;
                }
                pList = this.x.pList;
                if (pList.nExpr != 2)
                {
                    return 0;
                }
                if (pList.a[1].pExpr.op != TK_COLUMN)
                {
                    return 0;
                }
                return 1;
            }
            public void transferJoinMarkings(Expr pBase)
            {
                this.flags = (u16)(this.flags | pBase.flags & EP_FromJoin);
                this.iRightJoinTable = pBase.iRightJoinTable;
            }
            public TokenType Operator2
            {
                get
                {
                    return (TokenType)op2;
                }
                set
                {
                    op2 = (u8)value;
                }
            }
            public void ExprSetIrreducible()
            {
            }
            public bool ExprHasProperty(int P)
            {
                return (this.flags & P) == P;
            }
            public bool ExprHasAnyProperty(int P)
            {
                return (this.flags & P) != 0;
            }
            public void ExprSetProperty(int P)
            {
                this.flags = (ushort)(this.flags | P);
            }
            public void ExprClearProperty(int P)
            {
                this.flags = (ushort)(this.flags & ~P);
            }
        }


































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
