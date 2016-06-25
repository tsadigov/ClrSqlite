
using System;
using System.Diagnostics;

namespace Community.CsharpSqlite
{
    using System;
    using System.Diagnostics;
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
    using sqlite3_value = Engine.Mem;
    using System.Linq;
#if !SQLITE_MAX_VARIABLE_NUMBER
    using ynVar = System.Int16;
#else
				using ynVar = System.Int32; 
#endif
    ///
    ///<summary>
    ///The yDbMask datatype for the bitmask of all attached databases.
    ///</summary>

    //  typedef unsigned int yDbMask;
    using yDbMask = System.Int32;
    using System.Collections.Generic;
    using Community.CsharpSqlite.Engine;
    using Community.CsharpSqlite.Ast;
    using Community.CsharpSqlite.Parsing;
    using Community.CsharpSqlite.builder;
    using Community.CsharpSqlite.Metadata;
    using Community.CsharpSqlite.Os;
    using Community.CsharpSqlite.Tree;
    using Community.CsharpSqlite.Utils;
    using Community.CsharpSqlite.Metadata.Traverse;
    using Compiler;
    using Compiler.CodeGeneration;
    using Compiler.Parser;
    public class VdbeFacade
    {
        public VdbeFacade()
        {

        }
        public VdbeFacade(Vdbe v)
        {
            this.pVdbe = v;
        }
        Connection _db;
        ///<summary>
        ///The main database structure 
        ///</summary>
        public Connection db
        {
            get
            {
                return _db;
            }
            set
            {
                _db = value;
            }
        }
        
        
        ///
        ///<summary>
        ///An engine for executing database bytecode 
        ///</summary>
        public Vdbe pVdbe;


        public int allocTempReg()
        {
            if (this.nTempReg == 0)
            {
                return ++this.UsedCellCount;
            }
            return this.aTempReg[--this.nTempReg];
        }
        public void deallocTempReg(params int[] iRegArray)
        {
            foreach (var item in iRegArray) deallocTempReg(item);
        }


        public void deallocTempReg(int iReg)
        {
            if (iReg != 0 && this.nTempReg < Sqlite3.ArraySize(this.aTempReg))
            {
                int i;
                sqliteinth.yColCache p;
                for (i = 0; i < sqliteinth.SQLITE_N_COLCACHE; i++)//p=pParse.aColCache... p++)
                {
                    p = this.aColCache[i];
                    if (p.iReg == iReg)
                    {
                        p.tempReg = 1;
                        return;
                    }
                }
                this.aTempReg[this.nTempReg++] = iReg;
            }
        }
        public virtual int sqlite3ExprCodeTarget(Expr pExpr, int target)
        {
            return 0;
        }
        public int sqlite3ExprCodeTemp(Expr pExpr, ref int pReg)
        {
            int r1 = this.allocTempReg();
            int r2 = this.sqlite3ExprCodeTarget(pExpr, r1);
            if (r2 == r1)
            {
                pReg = r1;
            }
            else {
                this.deallocTempReg(r1);
                pReg = 0;
            }
            return r2;
        }
        public void sqlite3ExprCodeCopy(int iFrom, int iTo, int nReg)
        {
            int i;
            if (NEVER(iFrom == iTo))
                return;
            for (i = 0; i < nReg; i++)
            {
                this.pVdbe.sqlite3VdbeAddOp2(OpCode.OP_Copy, iFrom + i, iTo + i);
            }
        }

        private bool NEVER(bool v)
        {
            if(v) throw new NotImplementedException();
            return v;
        }


        public int sqlite3ExprCodeExprList(
             ExprList pList,///The expression list to be coded                         
             int target,///Where to write results                     
             bool doHardCopy///Make a hard copy of every element 
         )
        {
            Debug.Assert(pList != null);
            Debug.Assert(target > 0);
            Debug.Assert(this.pVdbe != null);
            ///Never gets this far otherwise             
            var n = pList.Count;

            pList.ForEach(
                (pItem, i) => {                    
                    int inReg = this.sqlite3ExprCodeTarget(pItem.pExpr, target + i);
                    if (inReg != target + i)
                    {
                        this.pVdbe.sqlite3VdbeAddOp2(doHardCopy ? OpCode.OP_Copy : OpCode.OP_SCopy, inReg, target + i);
                    }
                }
            );
            
            return n;
        }

        //----------------

        public void exprCodeBetween(
                    Expr pExpr,         ///The BETWEEN expression 			
                    int dest,           ///Jump here if the jump is taken 
                    int jumpIfTrue,         ///Take the jump if the BETWEEN is true 
                    int jumpIfNull          ///Take the jump if the BETWEEN is NULL 
            )
        {
            ///The AND operator in  x>=y AND x<=z  
            int regFree1 = 0;///Temporary use register 

            Debug.Assert(!pExpr.HasProperty(ExprFlags.EP_xIsSelect));
            var exprX = pExpr.pLeft.Clone();
            {
                exprX.iTable = this.sqlite3ExprCodeTemp(exprX, ref regFree1);
                exprX.Operator = TokenType.TK_REGISTER;
            }

            Expr exprAnd = new Expr()
            {
                Operator = TokenType.TK_AND,
                pLeft = new Expr()
                {
                    Operator = TokenType.TK_GE,
                    pLeft = exprX,
                    pRight = pExpr.x.pList.a[0].pExpr
                },
                pRight = new Expr()
                {
                    Operator = TokenType.TK_LE,
                    pLeft = exprX,
                    pRight = pExpr.x.pList.a[1].pExpr
                }
            };

            if (jumpIfTrue != 0)
            {
                this.sqlite3ExprIfTrue(exprAnd, dest, jumpIfNull);
            }
            else {
                this.sqlite3ExprIfFalse(exprAnd, dest, jumpIfNull);
            }
            this.deallocTempReg(regFree1);
            ///Ensure adequate test coverage 
            sqliteinth.testcase(jumpIfTrue == 0 && jumpIfNull == 0 && regFree1 == 0);
            sqliteinth.testcase(jumpIfTrue == 0 && jumpIfNull == 0 && regFree1 != 0);
            sqliteinth.testcase(jumpIfTrue == 0 && jumpIfNull != 0 && regFree1 == 0);
            sqliteinth.testcase(jumpIfTrue == 0 && jumpIfNull != 0 && regFree1 != 0);
            sqliteinth.testcase(jumpIfTrue != 0 && jumpIfNull == 0 && regFree1 == 0);
            sqliteinth.testcase(jumpIfTrue != 0 && jumpIfNull == 0 && regFree1 != 0);
            sqliteinth.testcase(jumpIfTrue != 0 && jumpIfNull != 0 && regFree1 == 0);
            sqliteinth.testcase(jumpIfTrue != 0 && jumpIfNull != 0 && regFree1 != 0);
        }


        public void sqlite3ExprIfTrue(Expr pExpr, int dest, int jumpIfNull)
        {
            Vdbe v = this.pVdbe;
            int regFree1 = 0;
            int regFree2 = 0;
            int r1 = 0, r2 = 0;
            Debug.Assert(jumpIfNull == sqliteinth.SQLITE_JUMPIFNULL || jumpIfNull == 0);
            if (NEVER(v == null))
                return;
            ///Existance of VDBE checked by caller 
            if (NEVER(pExpr == null))
                return;
            ///No way this can happen 
            var op = pExpr.Operator;
            switch (op)
            {
                case TokenType.TK_AND:
                    {
                        int d2 = v.sqlite3VdbeMakeLabel();
                        sqliteinth.testcase(jumpIfNull == 0);
                        this.sqlite3ExprCachePush();
                        this.sqlite3ExprIfFalse(pExpr.pLeft, d2, jumpIfNull ^ sqliteinth.SQLITE_JUMPIFNULL);
                        this.sqlite3ExprIfTrue(pExpr.pRight, dest, jumpIfNull);
                        v.sqlite3VdbeResolveLabel(d2);
                        this.sqlite3ExprCachePop(1);
                        break;
                    }
                case TokenType.TK_OR:
                    {
                        sqliteinth.testcase(jumpIfNull == 0);
                        this.sqlite3ExprIfTrue(pExpr.pLeft, dest, jumpIfNull);
                        this.sqlite3ExprIfTrue(pExpr.pRight, dest, jumpIfNull);
                        break;
                    }
                case TokenType.TK_NOT:
                    {
                        sqliteinth.testcase(jumpIfNull == 0);
                        this.sqlite3ExprIfFalse(pExpr.pLeft, dest, jumpIfNull);
                        break;
                    }
                case TokenType.TK_LT:
                case TokenType.TK_LE:
                case TokenType.TK_GT:
                case TokenType.TK_GE:
                case TokenType.TK_NE:
                case TokenType.TK_EQ:
                    {
                        Debug.Assert(TokenType.TK_LT == (TokenType)OpCode.OP_Lt);
                        Debug.Assert(TokenType.TK_LE == (TokenType)OpCode.OP_Le);
                        Debug.Assert(TokenType.TK_GT == (TokenType)OpCode.OP_Gt);
                        Debug.Assert(TokenType.TK_GE == (TokenType)OpCode.OP_Ge);
                        Debug.Assert(TokenType.TK_EQ == (TokenType)OpCode.OP_Eq);
                        Debug.Assert(TokenType.TK_NE == (TokenType)OpCode.OP_Ne);
                        sqliteinth.testcase(op == TokenType.TK_LT);
                        sqliteinth.testcase(op == TokenType.TK_LE);
                        sqliteinth.testcase(op == TokenType.TK_GT);
                        sqliteinth.testcase(op == TokenType.TK_GE);
                        sqliteinth.testcase(op == TokenType.TK_EQ);
                        sqliteinth.testcase(op == TokenType.TK_NE);
                        sqliteinth.testcase(jumpIfNull == 0);
                        r1 = this.sqlite3ExprCodeTemp(pExpr.pLeft, ref regFree1);
                        r2 = this.sqlite3ExprCodeTemp(pExpr.pRight, ref regFree2);
                        this.codeCompare(pExpr.pLeft, pExpr.pRight, (OpCode)op, r1, r2, dest, jumpIfNull);
                        sqliteinth.testcase(regFree1 == 0);
                        sqliteinth.testcase(regFree2 == 0);
                        break;
                    }
                case TokenType.TK_IS:
                case TokenType.TK_ISNOT:
                    {
                        sqliteinth.testcase(op == TokenType.TK_IS);
                        sqliteinth.testcase(op == TokenType.TK_ISNOT);
                        r1 = this.sqlite3ExprCodeTemp(pExpr.pLeft, ref regFree1);
                        r2 = this.sqlite3ExprCodeTemp(pExpr.pRight, ref regFree2);
                        op = (op == TokenType.TK_IS) ? TokenType.TK_EQ : TokenType.TK_NE;
                        this.codeCompare(pExpr.pLeft, pExpr.pRight, (OpCode)op, r1, r2, dest, sqliteinth.SQLITE_NULLEQ);
                        sqliteinth.testcase(regFree1 == 0);
                        sqliteinth.testcase(regFree2 == 0);
                        break;
                    }
                case TokenType.TK_ISNULL:
                case TokenType.TK_NOTNULL:
                    {
                        Debug.Assert(TokenType.TK_ISNULL == (TokenType)OpCode.OP_IsNull);
                        Debug.Assert(TokenType.TK_NOTNULL == (TokenType)OpCode.OP_NotNull);
                        sqliteinth.testcase(op == TokenType.TK_ISNULL);
                        sqliteinth.testcase(op == TokenType.TK_NOTNULL);
                        r1 = this.sqlite3ExprCodeTemp(pExpr.pLeft, ref regFree1);
                        v.sqlite3VdbeAddOp2(op, r1, dest);
                        sqliteinth.testcase(regFree1 == 0);
                        break;
                    }
                case TokenType.TK_BETWEEN:
                    {
                        sqliteinth.testcase(jumpIfNull == 0);
                        this.exprCodeBetween(pExpr, dest, 1, jumpIfNull);
                        break;
                    }
#if SQLITE_OMIT_SUBQUERY
																																																																																																					        case TokenType.TK_IN:
          {
            int destIfFalse = sqlite3VdbeMakeLabel( v );
            int destIfNull = jumpIfNull != 0 ? dest : destIfFalse;
            exprc.sqlite3ExprCodeIN( pParse, pExpr, destIfFalse, destIfNull );
            sqlite3VdbeAddOp2( v, OpCode.OP_Goto, 0, dest );
            sqlite3VdbeResolveLabel( v, destIfFalse );
            break;
          }
#endif
                default:
                    {
                        r1 = this.sqlite3ExprCodeTemp(pExpr, ref regFree1);
                        v.AddOpp3(OpCode.OP_If, r1, dest, jumpIfNull != 0 ? 1 : 0);
                        sqliteinth.testcase(regFree1 == 0);
                        sqliteinth.testcase(jumpIfNull == 0);
                        break;
                    }
            }
            this.deallocTempReg(regFree1);
            this.deallocTempReg(regFree2);
        }

        public int iCacheLevel;
        ///
        ///<summary>
        ///ColCache valid when aColCache[].iLevel<=iCacheLevel 
        ///</summary>
        public void sqlite3ExprCachePush()
        {
            this.iCacheLevel++;
        }
        public void sqlite3ExprCachePop(int N)
        {
            int i;
            sqliteinth.yColCache p;
            Debug.Assert(N > 0);
            Debug.Assert(this.iCacheLevel >= N);
            this.iCacheLevel -= N;
            for (i = 0; i < sqliteinth.SQLITE_N_COLCACHE; i++)// p++)
            {
                p = this.aColCache[i];
                if (p.iReg != 0 && p.iLevel > this.iCacheLevel)
                {
                    this.cacheEntryClear(p);
                    p.iReg = 0;
                }
            }
        }

        public void sqlite3ExprIfFalse(Expr pExpr, int dest, int jumpIfNull)
        {
            Vdbe v = this.pVdbe;

            int regFree1 = 0;
            int regFree2 = 0;
            int r1 = 0, r2 = 0;
            Debug.Assert(jumpIfNull == sqliteinth.SQLITE_JUMPIFNULL || jumpIfNull == 0);
            if (NEVER(v == null))
                return;
            ///Existance of VDBE checked by caller 
            if (pExpr == null)
                return;
            ///The value of pExpr.op and op are related as follows:
            ///
            ///pExpr.op            op
            ///</summary>
            ///<param name=""></param>
            ///<param name="TokenType.TK_ISNULL           OpCode.OP_NotNull">TokenType.TK_ISNULL           OpCode.OP_NotNull</param>
            ///<param name="TokenType.TK_NOTNULL          OpCode.OP_IsNull">TokenType.TK_NOTNULL          OpCode.OP_IsNull</param>
            ///<param name="TokenType.TK_NE               OpCode.OP_Eq">TokenType.TK_NE               OpCode.OP_Eq</param>
            ///<param name="TokenType.TK_EQ               OpCode.OP_Ne">TokenType.TK_EQ               OpCode.OP_Ne</param>
            ///<param name="TokenType.TK_GT               OpCode.OP_Le">TokenType.TK_GT               OpCode.OP_Le</param>
            ///<param name="TokenType.TK_LE               OpCode.OP_Gt">TokenType.TK_LE               OpCode.OP_Gt</param>
            ///<param name="TokenType.TK_GE               OpCode.OP_Lt">TokenType.TK_GE               OpCode.OP_Lt</param>
            ///<param name="TokenType.TK_LT               OpCode.OP_Ge">TokenType.TK_LT               OpCode.OP_Ge</param>
            ///<param name=""></param>
            ///<param name="For other values of pExpr.op, op is undefined and unused.">For other values of pExpr.op, op is undefined and unused.</param>
            ///<param name="The value of TokenType.TK_ and  OpCode.OP_ constants are arranged such that we">The value of TokenType.TK_ and  OpCode.OP_ constants are arranged such that we</param>
            ///<param name="can compute the mapping above using the following expression.">can compute the mapping above using the following expression.</param>
            ///<param name="Assert()s verify that the computation is correct.">Assert()s verify that the computation is correct.</param>
            ///<param name=""></param>
            var op = (TokenType)((((int)pExpr.Operator + ((int)TokenType.TK_ISNULL & 1)) ^ 1) - ((int)TokenType.TK_ISNULL & 1));
            ///Verify correct alignment of TokenType.TK_ and  OpCode.OP_ constants
            Debug.Assert(pExpr.Operator != TokenType.TK_ISNULL || op == (TokenType)OpCode.OP_NotNull);
            Debug.Assert(pExpr.Operator != TokenType.TK_NOTNULL || op == (TokenType)OpCode.OP_IsNull);
            Debug.Assert(pExpr.Operator != TokenType.TK_NE || op == (TokenType)OpCode.OP_Eq);
            Debug.Assert(pExpr.Operator != TokenType.TK_EQ || op == (TokenType)OpCode.OP_Ne);
            Debug.Assert(pExpr.Operator != TokenType.TK_LT || op == (TokenType)OpCode.OP_Ge);
            Debug.Assert(pExpr.Operator != TokenType.TK_LE || op == (TokenType)OpCode.OP_Gt);
            Debug.Assert(pExpr.Operator != TokenType.TK_GT || op == (TokenType)OpCode.OP_Le);
            Debug.Assert(pExpr.Operator != TokenType.TK_GE || op == (TokenType)OpCode.OP_Lt);
            switch (pExpr.Operator)
            {
                case TokenType.TK_AND:
                    {
                        sqliteinth.testcase(jumpIfNull == 0);
                        this.sqlite3ExprIfFalse(pExpr.pLeft, dest, jumpIfNull);
                        this.sqlite3ExprIfFalse(pExpr.pRight, dest, jumpIfNull);
                        break;
                    }
                case TokenType.TK_OR:
                    {
                        int d2 = v.sqlite3VdbeMakeLabel();
                        sqliteinth.testcase(jumpIfNull == 0);
                        this.sqlite3ExprCachePush();
                        this.sqlite3ExprIfTrue(pExpr.pLeft, d2, jumpIfNull ^ sqliteinth.SQLITE_JUMPIFNULL);
                        this.sqlite3ExprIfFalse(pExpr.pRight, dest, jumpIfNull);
                        v.sqlite3VdbeResolveLabel(d2);
                        this.sqlite3ExprCachePop(1);
                        break;
                    }
                case TokenType.TK_NOT:
                    {
                        sqliteinth.testcase(jumpIfNull == 0);
                        this.sqlite3ExprIfTrue(pExpr.pLeft, dest, jumpIfNull);
                        break;
                    }
                case TokenType.TK_LT:
                case TokenType.TK_LE:
                case TokenType.TK_GT:
                case TokenType.TK_GE:
                case TokenType.TK_NE:
                case TokenType.TK_EQ:
                    {
                        sqliteinth.testcase(op == TokenType.TK_LT);
                        sqliteinth.testcase(op == TokenType.TK_LE);
                        sqliteinth.testcase(op == TokenType.TK_GT);
                        sqliteinth.testcase(op == TokenType.TK_GE);
                        sqliteinth.testcase(op == TokenType.TK_EQ);
                        sqliteinth.testcase(op == TokenType.TK_NE);
                        sqliteinth.testcase(jumpIfNull == 0);
                        r1 = this.sqlite3ExprCodeTemp(pExpr.pLeft, ref regFree1);
                        r2 = this.sqlite3ExprCodeTemp(pExpr.pRight, ref regFree2);
                        this.codeCompare(pExpr.pLeft, pExpr.pRight, (OpCode)op, r1, r2, dest, jumpIfNull);
                        sqliteinth.testcase(regFree1 == 0);
                        sqliteinth.testcase(regFree2 == 0);
                        break;
                    }
                case TokenType.TK_IS:
                case TokenType.TK_ISNOT:
                    {
                        sqliteinth.testcase(pExpr.Operator == TokenType.TK_IS);
                        sqliteinth.testcase(pExpr.Operator == TokenType.TK_ISNOT);
                        r1 = this.sqlite3ExprCodeTemp(pExpr.pLeft, ref regFree1);
                        r2 = this.sqlite3ExprCodeTemp(pExpr.pRight, ref regFree2);
                        op = (pExpr.Operator == TokenType.TK_IS) ? TokenType.TK_NE : TokenType.TK_EQ;
                        this.codeCompare(pExpr.pLeft, pExpr.pRight, (OpCode)op, r1, r2, dest, sqliteinth.SQLITE_NULLEQ);
                        sqliteinth.testcase(regFree1 == 0);
                        sqliteinth.testcase(regFree2 == 0);
                        break;
                    }
                case TokenType.TK_ISNULL:
                case TokenType.TK_NOTNULL:
                    {
                        sqliteinth.testcase(op == TokenType.TK_ISNULL);
                        sqliteinth.testcase(op == TokenType.TK_NOTNULL);
                        r1 = this.sqlite3ExprCodeTemp(pExpr.pLeft, ref regFree1);
                        v.sqlite3VdbeAddOp2(op, r1, dest);
                        sqliteinth.testcase(regFree1 == 0);
                        break;
                    }
                case TokenType.TK_BETWEEN:
                    {
                        sqliteinth.testcase(jumpIfNull == 0);
                        this.exprCodeBetween(pExpr, dest, 0, jumpIfNull);
                        break;
                    }
#if SQLITE_OMIT_SUBQUERY
																																																																																																					        case TokenType.TK_IN:
          {
            if ( jumpIfNull != 0 )
            {
              exprc.sqlite3ExprCodeIN( pParse, pExpr, dest, dest );
            }
            else
            {
              int destIfNull = sqlite3VdbeMakeLabel( v );
              exprc.sqlite3ExprCodeIN( pParse, pExpr, dest, destIfNull );
              sqlite3VdbeResolveLabel( v, destIfNull );
            }
          break;
          }
#endif
                default:
                    {
                        r1 = this.sqlite3ExprCodeTemp(pExpr, ref regFree1);
                        v.AddOpp3(OpCode.OP_IfNot, r1, dest, jumpIfNull != 0 ? 1 : 0);
                        sqliteinth.testcase(regFree1 == 0);
                        sqliteinth.testcase(jumpIfNull == 0);
                        break;
                    }
            }
            deallocTempReg(regFree1, regFree2);
        }


        public int codeCompare(
             Expr pLeft,///The left operand                     
             Expr pRight,///The right operand                      
             OpCode opcode,///The comparison opcode                           
             int in1, int in2,///Register holding operands 
             int dest,///Jump here if true.  
             int jumpIfNull///If true, jump if either operand is NULL                          
          )
        {   
            var p4 = this.sqlite3BinaryCompareCollSeq(pLeft, pRight);
            var p5 = pLeft.binaryCompareP5(pRight, jumpIfNull);
            var addr = this.pVdbe.sqlite3VdbeAddOp4(opcode, in2, dest, in1, p4, P4Usage.P4_COLLSEQ);
            this.pVdbe.ChangeP5((u8)p5);
            return addr;
        }

        public CollSeq sqlite3BinaryCompareCollSeq(Expr pLeft, Expr pRight)
        {
            CollSeq pColl;
            Debug.Assert(pLeft != null);
            if ((pLeft.Flags & ExprFlags.EP_ExpCollate) != 0)
            {
                Debug.Assert(pLeft.CollatingSequence != null);
                pColl = pLeft.CollatingSequence;
            }
            else
                if (pRight != null && ((pRight.Flags & ExprFlags.EP_ExpCollate) != 0))
            {
                Debug.Assert(pRight.CollatingSequence != null);
                pColl = pRight.CollatingSequence;
            }
            else {
                pColl = this.sqlite3ExprCollSeq(pLeft);
                if (pColl == null)
                {
                    pColl = this.sqlite3ExprCollSeq(pRight);
                }
            }
            return pColl;
        }

        public CollSeq sqlite3ExprCollSeq(Expr pExpr)
        {
            CollSeq pColl = null;
            Expr p = pExpr;
            while (Sqlite3.ALWAYS(p))
            {
                TokenType op;
                pColl = pExpr.CollatingSequence;
                if (pColl != null)
                    break;
                op = p.Operator;
                if (p.TableReference != null && (op == TokenType.TK_AGG_COLUMN || op == TokenType.TK_COLUMN || op == TokenType.TK_REGISTER || op == TokenType.TK_TRIGGER))
                {
                    ///<param name="op==TokenType.TK_REGISTER && p">>pTab!=0 happens when pExpr was originally</param>
                    ///<param name="a TokenType.TK_COLUMN but was previously evaluated and cached in a register ">a TokenType.TK_COLUMN but was previously evaluated and cached in a register </param>
                    string zColl;
                    int j = p.iColumn;
                    if (j >= 0)
                    {
                        Connection db = this.db;
                        zColl = p.TableReference.aCol[j].Collation;
                        pColl = db.FindCollSeq(sqliteinth.ENC(db), zColl, 0);
                        pExpr.CollatingSequence = pColl;
                    }
                    break;
                }
                if (op != TokenType.TK_CAST && op != TokenType.TK_UPLUS)
                {
                    break;
                }
                p = p.pLeft;
            }
            /*if (Sqlite3.sqlite3CheckCollSeq(this, pColl) != 0)
            {
                pColl = null;
            }*/
            //TODO:UNCOMMENT
            return pColl;
        }
        //---------------------------------------------------------
        public void Drop_Trigger(Trigger trg, Vdbe v, int iDb)
        {
            var iTrigDb = this.db.indexOfBackendWithSchema(trg.pSchema);
            Debug.Assert(iTrigDb == iDb || iTrigDb == 1);
            v.sqlite3VdbeAddOp4(OpCode.OP_DropTrigger, iTrigDb, 0, 0, trg.zName, 0);
        }

        public void Drop_Table(Table pTab, Vdbe v, int iDb)
        {
            ///Drop the table and index from the internal schema. 
            v.sqlite3VdbeAddOp4(OpCode.OP_DropTable, iDb, 0, 0, pTab.zName, 0);
        }

        /// <summary>
        /// from vdbe
        /// </summary>
        /// <param name="iDb"></param>
        /// <param name="zWhere"></param>
        public void codegenAddParseSchemaOp(int iDb, string zWhere)
        {
            int j;
            int addr = pVdbe.AddOpp3(OpCode.OP_ParseSchema, iDb, 0, 0);
            pVdbe.sqlite3VdbeChangeP4(addr, zWhere, P4Usage.P4_DYNAMIC);
            for (j = 0; j < this.db.BackendCount; j++)
                vdbeaux.markUsed(pVdbe, j);
        }


        ///<summary>
        /// Generate code to make sure the file format number is at least minFormat.
        /// The generated code will increase the file format number if necessary.
        ///</summary>
        public void codegenMinimumFileFormat(int iDb, int minFormat)
        {
            var v = pVdbe;//this.sqlite3GetVdbe();
            ///The VDBE should have been allocated before this routine is called.
            ///If that allocation failed, we would have quit before reaching this
            ///point 
            if (Sqlite3.ALWAYS(v))
            {
                int r1 = this.allocTempReg(), r2 = this.allocTempReg();

                v.sqlite3VdbeAddOp3(OpCode.OP_ReadCookie, iDb, r1, BTreeProp.FILE_FORMAT);      //r1=cookie[FILE_FORMAT]
                vdbeaux.markUsed(v, iDb);
                v.sqlite3VdbeAddOp2(OpCode.OP_Integer, minFormat, r2);                            //r2=minFormat
                var j1 = v.AddOpp3(OpCode.OP_Ge, r2, 0, r1);                               //if(r2>r1)
                v.AddOpp3(OpCode.OP_SetCookie, iDb, (int)BTreeProp.FILE_FORMAT, r2);  //  cookie[FileFormat]=r2
                v.sqlite3VdbeJumpHere(j1);

                this.deallocTempReg(r1, r2);
            }
        }
        public void codeInteger(Expr pExpr, bool negFlag, int iMem)
        {
            Vdbe v = this.pVdbe;
            if ((pExpr.Flags & ExprFlags.EP_IntValue) != 0)
            {
                var i = pExpr.u.iValue;
                Debug.Assert(i >= 0);
                if (negFlag)
                    i = -i;
                v.sqlite3VdbeAddOp2(OpCode.OP_Integer, i, iMem);
            }
            else {

                i64 value = 0;
                var z = pExpr.u.zToken;
                Debug.Assert(!String.IsNullOrEmpty(z));
                var c = Converter.sqlite3Atoi64(z, ref value, StringExtensions.Strlen30(z), SqliteEncoding.UTF8);
                if (c == 0 || (c == 2 && negFlag))
                {
                    //char* zV;
                    if (negFlag)
                    {
                        value = c == 2 ? IntegerExtensions.SMALLEST_INT64 : -value;
                    }
                    v.sqlite3VdbeAddOp4(OpCode.OP_Int64, 0, iMem, 0, value, P4Usage.P4_INT64);
                }
                else {
#if SQLITE_OMIT_FLOATING_POINT
																																																																																																																																														utilc.sqlite3ErrorMsg(pParse, "oversized integer: %s%s", negFlag ? "-" : "", z);
#else
                    codeReal(v, z, negFlag, iMem);
#endif
                }
            }
        }

#if !SQLITE_OMIT_FLOATING_POINT
        ///<summary>
        ///from exprc
        /// Generate an instruction that will put the floating point
        /// value described by z[0..n-1] into register iMem.
        ///
        /// The z[] string will probably not be zero-terminated.  But the
        /// z[n] character is guaranteed to be something that does not look
        /// like the continuation of the number.
        ///</summary>
        public void codeReal(Vdbe v, string z, bool negateFlag, int iMem)
        {
            if (Sqlite3.ALWAYS(!String.IsNullOrEmpty(z)))
            {
                double value = 0;
                //string zV;
                Converter.sqlite3AtoF(z, ref value, StringExtensions.Strlen30(z), SqliteEncoding.UTF8);
                Debug.Assert(!MathExtensions.sqlite3IsNaN(value));
                ///The new AtoF never returns NaN 
                if (negateFlag)
                    value = -value;
                //zV = dup8bytes(v,  value);
                v.sqlite3VdbeAddOp4(OpCode.OP_Real, 0, iMem, 0, value, P4Usage.P4_REAL);
            }
        }
#endif
        public void codegenRowIndexDelete(
                Table pTab,
                ///Table containing the row to be deleted 
                int iCur,///
                         ///VdbeCursor number for the table 
               int[] aRegIdx///
                            ///Only delete if aRegIdx!=0 && aRegIdx[i]>0 
           )
        {
            pTab.pIndex.path(x => x.pNext).ForEach(
                (pIdx, idx) => {//start with 1
                    var i = idx + 1;
                    if (aRegIdx != null && aRegIdx[i - 1] == 0)
                        return;
                    var r1 = this.codegenGenerateIndexKey(pIdx, iCur, 0, false);
                    this.pVdbe.AddOpp3(OpCode.OP_IdxDelete, iCur + i, r1, pIdx.nColumn + 1);
                }
            );

        }

        public void codegenColumnDefault(Table pTab, int i, int iReg)
        {
            Debug.Assert(pTab != null);
            if (null == pTab.pSelect)
            {
                sqlite3_value pValue = new sqlite3_value();
                SqliteEncoding enc = sqliteinth.ENC(pVdbe.sqlite3VdbeDb());
                Column pCol = pTab.aCol[i];
#if SQLITE_DEBUG
																																																																																																															        VdbeComment( v, "%s.%s", pTab.zName, pCol.zName );
#endif
                Debug.Assert(i < pTab.nCol);
                vdbemem_cs.sqlite3ValueFromExpr(pVdbe.sqlite3VdbeDb(), pCol.DefaultValue, enc, pCol.affinity, ref pValue);
                if (pValue != null)
                {
                    pVdbe.sqlite3VdbeChangeP4(-1, pValue, P4Usage.P4_MEM);
                }
#if !SQLITE_OMIT_FLOATING_POINT
                if (iReg >= 0 && pTab.aCol[i].affinity == sqliteinth.SQLITE_AFF_REAL)
                {
                    pVdbe.AddOpp1(OpCode.OP_RealAffinity, iReg);
                }
#endif
            }
        }


        public int codegenGenerateIndexKey(
             Index pIdx,///The index for which to generate a key 
             int iCur,///VdbeCursor number for the pIdx.pTable table 
             int regOut,///Write the new index key to this register 
             bool doMakeRec///Run the  OpCode.OP_MakeRecord instruction if true 
          )
        {
            var v = this.pVdbe;
            var facade = new VdbeFacade(v);
            Table pTab = pIdx.pTable;
            var nCol = pIdx.nColumn;
            var regBase = this.sqlite3GetTempRange(nCol + 1);
            v.sqlite3VdbeAddOp2(OpCode.OP_Rowid, iCur, regBase + nCol);
            for (var j = 0; j < nCol; j++)
            {
                int idx = pIdx.ColumnIdx[j];
                if (idx == pTab.iPKey)                
                    v.sqlite3VdbeAddOp2(OpCode.OP_SCopy, regBase + nCol, regBase + j);                
                else {
                    v.AddOpp3(OpCode.OP_Column, iCur, idx, regBase + j);
                    facade.codegenColumnDefault(pTab, idx, -1);
                }
            }
            if (doMakeRec)
            {
                string zAff;
                if (pTab.pSelect != null || (this.db.flags & SqliteFlags.SQLITE_IdxRealAsInt) != 0)
                {
                    zAff = "";
                }
                else {
                    zAff = v.sqlite3IndexAffinityStr(pIdx);
                }
                v.AddOpp3(OpCode.OP_MakeRecord, regBase, nCol + 1, regOut);
                v.sqlite3VdbeChangeP4(-1, zAff, P4Usage.P4_TRANSIENT);
            }
            this.sqlite3ReleaseTempRange(regBase, nCol + 1);
            return regBase;
        }

        public int sqlite3GetTempRange(int nReg)
        {
            var i = this.iRangeReg;
            var n = this.nRangeReg;
            if (nReg <= n)
            {
                //Debug.Assert( 1 == usedAsColumnCache( pParse, i, i + n - 1 ) );
                this.iRangeReg += nReg;
                this.nRangeReg -= nReg;
            }
            else {
                i = this.UsedCellCount + 1;
                this.UsedCellCount += nReg;
            }
            return i;
        }

        int _nMem;
        ///<summary>
        ///Name:nMem >>> Number of memory cells used so far 
        ///</summary>
        public int UsedCellCount
        {
            get
            {
                return _nMem;
            }
            set
            {
                _nMem = value;
            }
        }
        
        public void sqlite3ReleaseTempRange(int iReg, int nReg)
        {
            this.sqlite3ExprCacheRemove(iReg, nReg);
            if (nReg > this.nRangeReg)
            {
                this.nRangeReg = nReg;
                this.iRangeReg = iReg;
            }
        }

        public void sqlite3ExprCacheRemove(int iReg, int nReg)
        {
            int i;
            int iLast = iReg + nReg - 1;
            sqliteinth.yColCache p;
            for (i = 0; i < sqliteinth.SQLITE_N_COLCACHE; i++)//p=pParse.aColCache... p++)
            {
                p = this.aColCache[i];
                int r = p.iReg;
                if (r >= iReg && r <= iLast)
                {
                    this.cacheEntryClear(p);
                    p.iReg = 0;
                }
            }
        }

        public void cacheEntryClear(sqliteinth.yColCache p)
        {
            if (p.tempReg != 0)
            {
                if (this.nTempReg < Sqlite3.ArraySize(this.aTempReg))
                {
                    this.aTempReg[this.nTempReg++] = p.iReg;
                }
                p.tempReg = 0;
            }
        }
        public int[] aTempReg
        {
            get;
            set;
        }
        ///
        ///<summary>
        ///Holding area for temporary registers 
        ///</summary>
        public u8 nTempReg;
        ///
        ///<summary>
        ///Number of temporary registers in aTempReg[] 
        ///</summary>
        ///
        int _nRangeReg;
        public int nRangeReg
        {
            get
            {
                return _nRangeReg;
            }
            set
            {
                _nRangeReg = value;
            }
        }
        ///
        ///<summary>
        ///Size of the temporary register block 
        ///</summary>
        public int iRangeReg;
        ///
        ///<summary>
        ///First register in temporary register block 
        ///</summary>
        ///
        public sqliteinth.yColCache[] aColCache;
        ///
        ///<summary>
        ///One for each valid column cache entry 
        ///</summary>

    }
}