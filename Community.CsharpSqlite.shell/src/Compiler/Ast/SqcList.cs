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


namespace Community.CsharpSqlite.Ast
{
    using sqlite3_value = Engine.Mem;
    using Parse = Community.CsharpSqlite.Sqlite3.Parse;
    using Community.CsharpSqlite.Ast;
    using Metadata;
    using Community.CsharpSqlite.Utils;    
    

        public class SrcList:MyCollection<SrcList_item>
        {
            
            public SrcList Copy()
            {
                if (this == null)
                    return null;
                else
                {
                    SrcList cp = (SrcList)MemberwiseClone();
                    if (a != null)
                        cp.a = new List<SrcList_item>(a);
                        
                    return cp;
                }
            }
            public void exprAnalyzeAll(///
                ///<summary>
                ///the FROM clause 
                ///</summary>
            WhereClause pWC///
                ///<summary>
                ///the WHERE clause to be analyzed 
                ///</summary>
            )
            {
                int i;
                for (i = pWC.nTerm - 1; i >= 0; i--)
                {
                    this.exprAnalyze(pWC, i);
                }
            }
            public void exprAnalyzeOrTerm(///
                ///<summary>
                ///the FROM clause 
                ///</summary>
            WhereClause pWC,///
                ///<summary>
                ///the complete WHERE clause 
                ///</summary>
            int idxTerm///
                ///<summary>
                ///</summary>
                ///<param name="Index of the OR">term to be analyzed </param>
            )
            {
                Parse pParse = pWC.pParse;
                ///Parser context 
                Connection db = pParse.db;
                ///Data_base connection 
                WhereTerm pTerm = pWC.a[idxTerm];
                ///The term to be analyzed 
                Expr pExpr = pTerm.pExpr;
                ///The expression of the term 
                WhereMaskSet pMaskSet = pWC.pMaskSet;
                ///Table use masks 
                int i;
                ///Loop counters 
                WhereClause pOrWc;
                ///Breakup of pTerm into subterms 
                WhereTerm pOrTerm;
                ///<param name="A Sub">term within the pOrWc </param>
                WhereOrInfo pOrInfo;
                ///Additional information Debug.Associated with pTerm 
                Bitmask chngToIN;
                ///Tables that might satisfy case 1 
                Bitmask indexable;
                ///Tables that are indexable, satisfying case 2 
                ///
                ///Break the OR clause into its separate subterms.  The subterms are
                ///stored in a WhereClause structure containing within the WhereOrInfo
                ///object that is attached to the original OR clause term.
                Debug.Assert((pTerm.wtFlags & (WhereTermFlags.TERM_DYNAMIC | WhereTermFlags.TERM_ORINFO | WhereTermFlags.TERM_ANDINFO)) == 0);
                Debug.Assert(pExpr.Operator == TokenType.TK_OR);
                pTerm.u.pOrInfo = pOrInfo = new WhereOrInfo();
                //sqlite3DbMallocZero(db, sizeof(*pOrInfo));
                if (pOrInfo == null)
                    return;
                pTerm.wtFlags |= WhereTermFlags.TERM_ORINFO;
                pOrWc = pOrInfo.wc;
                pOrWc.whereClauseInit(pWC.pParse, pMaskSet);
                pOrWc.whereSplit(pExpr, TokenType.TK_OR);
                this.exprAnalyzeAll(pOrWc);
                //      if ( db.mallocFailed != 0 ) return;
                Debug.Assert(pOrWc.nTerm >= 2);
                ///Compute the set of tables that might satisfy cases 1 or 2.
                indexable = ~(Bitmask)0;
                chngToIN = ~(pWC.vmask);
                for (i = pOrWc.nTerm - 1; i >= 0 && indexable != 0; i--)//, pOrTerm++ )
                {
                    pOrTerm = pOrWc.a[i];
                    if ((pOrTerm.eOperator & wherec.WO_SINGLE) == 0)
                    {
                        WhereAndInfo pAndInfo;
                        Debug.Assert(pOrTerm.eOperator == 0);
                        Debug.Assert((pOrTerm.wtFlags & (WhereTermFlags.TERM_ANDINFO | WhereTermFlags.TERM_ORINFO)) == 0);
                        chngToIN = 0;
                        pAndInfo = new WhereAndInfo();
                        //sqlite3DbMallocRaw(db, sizeof(*pAndInfo));
                        if (pAndInfo != null)
                        {
                            WhereClause pAndWC;
                            WhereTerm pAndTerm;
                            int j;
                            Bitmask b = 0;
                            pOrTerm.u.pAndInfo = pAndInfo;
                            pOrTerm.wtFlags |= WhereTermFlags.TERM_ANDINFO;
                            pOrTerm.eOperator = wherec.WO_AND;
                            pAndWC = pAndInfo.wc;
                            pAndWC.whereClauseInit(pWC.pParse, pMaskSet);
                            pAndWC.whereSplit(pOrTerm.pExpr, TokenType.TK_AND);
                            this.exprAnalyzeAll(pAndWC);
                            //sqliteinth.testcase( db.mallocFailed );
                            ////if ( 0 == db.mallocFailed )
                            {
                                for (j = 0; j < pAndWC.nTerm; j++)//, pAndTerm++ )
                                {
                                    pAndTerm = pAndWC.a[j];
                                    Debug.Assert(pAndTerm.pExpr != null);
                                    if (wherec.allowedOp(pAndTerm.pExpr.Operator))
                                    {
                                        b |= pMaskSet.getMask(pAndTerm.leftCursor);
                                    }
                                }
                            }
                            indexable &= b;
                        }
                    }
                    else
                        if ((pOrTerm.wtFlags & WhereTermFlags.TERM_COPIED) != 0)
                        {
                            ///Skip this term for now.  We revisit it when we process the
                            ///corresponding WhereTermFlags.TERM_VIRTUAL term 
                        }
                        else
                        {
                            Bitmask b;
                            b = pMaskSet.getMask(pOrTerm.leftCursor);
                            if ((pOrTerm.wtFlags & WhereTermFlags.TERM_VIRTUAL) != 0)
                            {
                                WhereTerm pOther = pOrWc.a[pOrTerm.iParent];
                                b |= pMaskSet.getMask(pOther.leftCursor);
                            }
                            indexable &= b;
                            if (pOrTerm.eOperator != wherec.WO_EQ)
                            {
                                chngToIN = 0;
                            }
                            else
                            {
                                chngToIN &= b;
                            }
                        }
                }
                ///Record the set of tables that satisfy case 2.  The set might be
                ///empty.
                pOrInfo.indexable = indexable;
                pTerm.eOperator = (u16)(indexable == 0 ? 0 : wherec.WO_OR);
                ///chngToIN holds a set of tables that *might* satisfy case 1.  But
                ///we have to do some additional checking to see if case 1 really
                ///is satisfied.
                ///<param name="chngToIN will hold either 0, 1, or 2 bits.  The 0">bit case means</param>
                ///<param name="that there is no possibility of transforming the OR clause into an">that there is no possibility of transforming the OR clause into an</param>
                ///<param name="IN operator because one or more terms in the OR clause contain">IN operator because one or more terms in the OR clause contain</param>
                ///<param name="something other than == on a column in the single table.  The 1">bit</param>
                ///<param name="case means that every term of the OR clause is of the form">case means that every term of the OR clause is of the form</param>
                ///<param name=""table.column=expr" for some single table.  The one bit that is set">"table.column=expr" for some single table.  The one bit that is set</param>
                ///<param name="will correspond to the common table.  We still need to check to make">will correspond to the common table.  We still need to check to make</param>
                ///<param name="sure the same column is used on all terms.  The 2">bit case is when</param>
                ///<param name="the all terms are of the form "table1.column=table2.column".  It">the all terms are of the form "table1.column=table2.column".  It</param>
                ///<param name="might be possible to form an IN operator with either table1.column">might be possible to form an IN operator with either table1.column</param>
                ///<param name="or table2.column as the LHS if either is common to every term of">or table2.column as the LHS if either is common to every term of</param>
                ///<param name="the OR clause.">the OR clause.</param>
                ///<param name=""></param>
                ///<param name="Note that terms of the form "table.column1=table.column2" (the">Note that terms of the form "table.column1=table.column2" (the</param>
                ///<param name="same table on both sizes of the ==) cannot be optimized.">same table on both sizes of the ==) cannot be optimized.</param>
                if (chngToIN != 0)
                {
                    int okToChngToIN = 0;
                    ///True if the conversion to IN is valid 
                    int iColumn = -1;
                    ///Column index on lhs of IN operator 
                    int iCursor = -1;
                    ///Table cursor common to all terms 
                    int j = 0;
                    ///Loop counter 
                    ///
                    ///Search for a table and column that appears on one side or the
                    ///other of the == operator in every subterm.  That table and column
                    ///will be recorded in iCursor and iColumn.  There might not be any
                    ///such table and column.  Set okToChngToIN if an appropriate table
                    ///and column is found but leave okToChngToIN false if not found.
                    for (j = 0; j < 2 && 0 == okToChngToIN; j++)
                    {
                        //pOrTerm = pOrWc.a;
                        for (i = pOrWc.nTerm - 1; i >= 0; i--)//, pOrTerm++)
                        {
                            pOrTerm = pOrWc.a[pOrWc.nTerm - 1 - i];
                            Debug.Assert(pOrTerm.eOperator == wherec.WO_EQ);
                            pOrTerm.wtFlags = (pOrTerm.wtFlags & ~WhereTermFlags.TERM_OR_OK);
                            if (pOrTerm.leftCursor == iCursor)
                            {
                                ///<param name="This is the 2">bit case and we are on the second iteration and</param>
                                ///<param name="current term is from the first iteration.  So skip this term. ">current term is from the first iteration.  So skip this term. </param>
                                Debug.Assert(j == 1);
                                continue;
                            }
                            if ((chngToIN & pMaskSet.getMask(pOrTerm.leftCursor)) == 0)
                            {
                                ///This term must be of the form t1.a==t2.b where t2 is in the
                                ///chngToIN set but t1 is not.  This term will be either preceeded
                                ///or follwed by an inverted copy (t2.b==t1.a).  Skip this term
                                ///and use its inversion. 
                                sqliteinth.testcase(pOrTerm.wtFlags & WhereTermFlags.TERM_COPIED);
                                sqliteinth.testcase(pOrTerm.wtFlags & WhereTermFlags.TERM_VIRTUAL);
                                Debug.Assert((pOrTerm.wtFlags & (WhereTermFlags.TERM_COPIED | WhereTermFlags.TERM_VIRTUAL)) != 0);
                                continue;
                            }
                            iColumn = pOrTerm.u.leftColumn;
                            iCursor = pOrTerm.leftCursor;
                            break;
                        }
                        if (i < 0)
                        {
                            ///No candidate table+column was found.  This can only occur
                            ///on the second iteration 
                            Debug.Assert(j == 1);
                            Debug.Assert((chngToIN & (chngToIN - 1)) == 0);
                            Debug.Assert(chngToIN == pMaskSet.getMask(iCursor));
                            break;
                        }
                        sqliteinth.testcase(j == 1);
                        ///We have found a candidate table and column.  Check to see if that
                        ///table and column is common to every term in the OR clause 
                        okToChngToIN = 1;
                        for (; i >= 0 && okToChngToIN != 0; i--)//, pOrTerm++)
                        {
                            pOrTerm = pOrWc.a[pOrWc.nTerm - 1 - i];
                            Debug.Assert(pOrTerm.eOperator == wherec.WO_EQ);
                            if (pOrTerm.leftCursor != iCursor)
                            {
                                pOrTerm.wtFlags = (pOrTerm.wtFlags & ~WhereTermFlags.TERM_OR_OK);
                            }
                            else
                                if (pOrTerm.u.leftColumn != iColumn)
                                {
                                    okToChngToIN = 0;
                                }
                                else
                                {
                                    int affLeft, affRight;
                                    ///<param name="If the right">hand side is also a column, then the affinities</param>
                                    ///<param name="of both right and left sides must be such that no type">of both right and left sides must be such that no type</param>
                                    ///<param name="conversions are required on the right.  (Ticket #2249)">conversions are required on the right.  (Ticket #2249)</param>
                                    affRight = pOrTerm.pExpr.pRight.sqlite3ExprAffinity();
                                    affLeft = pOrTerm.pExpr.pLeft.sqlite3ExprAffinity();
                                    if (affRight != 0 && affRight != affLeft)
                                    {
                                        okToChngToIN = 0;
                                    }
                                    else
                                    {
                                        pOrTerm.wtFlags |= WhereTermFlags.TERM_OR_OK;
                                    }
                                }
                        }
                    }
                    ///At this point, okToChngToIN is true if original pTerm satisfies
                    ///case 1.  In that case, construct a new virtual term that is
                    ///pTerm converted into an IN operator.
                    ///<param name="EV: R">15100</param>
                    ///<param name=""></param>
                    if (okToChngToIN != 0)
                    {
                        Expr pDup;
                        ///A transient duplicate expression 
                        ExprList pList = null;
                        ///The RHS of the IN operator 
                        Expr pLeft = null;
                        ///The LHS of the IN operator 
                        Expr pNew;
                        ///The complete IN operator 
                        for (i = pOrWc.nTerm - 1; i >= 0; i--)//, pOrTerm++)
                        {
                            pOrTerm = pOrWc.a[pOrWc.nTerm - 1 - i];
                            if ((pOrTerm.wtFlags & WhereTermFlags.TERM_OR_OK) == 0)
                                continue;
                            Debug.Assert(pOrTerm.eOperator == wherec.WO_EQ);
                            Debug.Assert(pOrTerm.leftCursor == iCursor);
                            Debug.Assert(pOrTerm.u.leftColumn == iColumn);
                            pDup = exprc.sqlite3ExprDup(db, pOrTerm.pExpr.pRight, 0);
                            pList = pList.Append( pDup);
                            pLeft = pOrTerm.pExpr.pLeft;
                        }
                        Debug.Assert(pLeft != null);
                        pDup = exprc.sqlite3ExprDup(db, pLeft, 0);
                        pNew = pParse.sqlite3PExpr(TokenType.TK_IN, pDup, null, null);
                        if (pNew != null)
                        {
                            int idxNew;
                            pNew.transferJoinMarkings(pExpr);
                            Debug.Assert(!pNew.HasProperty(ExprFlags.EP_xIsSelect));
                            pNew.x.pList = pList;
                            idxNew = pWC.whereClauseInsert(pNew, WhereTermFlags.TERM_VIRTUAL | WhereTermFlags.TERM_DYNAMIC);
                            sqliteinth.testcase(idxNew == 0);
                            this.exprAnalyze(pWC, idxNew);
                            pTerm = pWC.a[idxTerm];
                            pWC.a[idxNew].iParent = idxTerm;
                            pTerm.nChild = 1;
                        }
                        else
                        {
                            exprc.Delete(db, ref pList);
                        }
                        pTerm.eOperator = wherec.WO_NOOP;
                        ///case 1 trumps case 2 
                    }
                }
            }
            public void exprAnalyze(
                ///<summary>
                ///the FROM clause 
                ///</summary>
            WhereClause pWC,///
                ///<summary>
                ///the WHERE clause 
                ///</summary>
            int idxTerm///
                ///<summary>
                ///Index of the term to be analyzed 
                ///</summary>
            )
            {
                WhereTerm pTerm;
                ///The term to be analyzed 
                WhereMaskSet pMaskSet;
                ///Set of table index masks 
                Expr pExpr;
                ///The expression to be analyzed 
                Bitmask prereqLeft;
                ///Prerequesites of the pExpr.pLeft 
                Bitmask prereqAll;
                ///Prerequesites of pExpr 
                Bitmask extraRight = 0;
                ///Extra dependencies on LEFT JOIN 
                Expr pStr1 = null;
                ///RHS of LIKE/GLOB operator 
                bool isComplete = false;
                ///RHS of LIKE/GLOB ends with wildcard 
                bool noCase = false;
                
                ///<param name="Top">level operator.  pExpr.op </param>
                Parse pParse = pWC.pParse;
                ///Parsing context 
                Connection db = pParse.db;
                ///Data_base connection 
                //if ( db.mallocFailed != 0 )
                //{
                //  return;
                //}
                pTerm = pWC.a[idxTerm];
                pMaskSet = pWC.pMaskSet;
                pExpr = pTerm.pExpr;
                prereqLeft = pMaskSet.exprTableUsage(pExpr.pLeft);

                ///LIKE/GLOB distinguishes case 

                var op = pExpr.Operator;
                if (op == TokenType.TK_IN)
                {
                    Debug.Assert(pExpr.pRight == null);
                    if (pExpr.HasProperty(ExprFlags.EP_xIsSelect))
                    {
                        pTerm.prereqRight = pMaskSet.exprSelectTableUsage(pExpr.x.pSelect);
                    }
                    else
                    {
                        pTerm.prereqRight = pMaskSet.exprListTableUsage(pExpr.x.pList);
                    }
                }
                else
                    if (op == TokenType.TK_ISNULL)
                    {
                        pTerm.prereqRight = 0;
                    }
                    else
                    {
                        pTerm.prereqRight = pMaskSet.exprTableUsage(pExpr.pRight);
                    }
                prereqAll = pMaskSet.exprTableUsage(pExpr);
                if (pExpr.HasProperty(ExprFlags.EP_FromJoin))
                {
                    Bitmask x = pMaskSet.getMask(pExpr.iRightJoinTable);
                    prereqAll |= x;
                    extraRight = x - 1;
                    ///ON clause terms may not be used with an index
                    ///on left table of a LEFT JOIN.  Ticket #3015 
                }
                pTerm.prereqAll = prereqAll;
                pTerm.leftCursor = -1;
                pTerm.iParent = -1;
                pTerm.eOperator = 0;
                if (wherec.allowedOp(op) && (pTerm.prereqRight & prereqLeft) == 0)
                {
                    Expr pLeft = pExpr.pLeft;
                    Expr pRight = pExpr.pRight;
                    if (pLeft.Operator == TokenType.TK_COLUMN)
                    {
                        pTerm.leftCursor = pLeft.iTable;
                        pTerm.u.leftColumn = pLeft.iColumn;
                        pTerm.eOperator = wherec.operatorMask(op);
                    }
                    if (pRight != null && pRight.Operator == TokenType.TK_COLUMN)
                    {
                        WhereTerm pNew;
                        Expr pDup;
                        if (pTerm.leftCursor >= 0)
                        {
                            int idxNew;
                            pDup = exprc.sqlite3ExprDup(db, pExpr, 0);
                            //if ( db.mallocFailed != 0 )
                            //{
                            //  exprc.sqlite3ExprDelete( db, ref pDup );
                            //  return;
                            //}
                            idxNew = pWC.whereClauseInsert(pDup, WhereTermFlags.TERM_VIRTUAL | WhereTermFlags.TERM_DYNAMIC);
                            if (idxNew == 0)
                                return;
                            pNew = pWC.a[idxNew];
                            pNew.iParent = idxTerm;
                            pTerm = pWC.a[idxTerm];
                            pTerm.nChild = 1;
                            pTerm.wtFlags |= WhereTermFlags.TERM_COPIED;
                        }
                        else
                        {
                            pDup = pExpr;
                            pNew = pTerm;
                        }
                        pParse.exprCommute(pDup);
                        pLeft = pDup.pLeft;
                        pNew.leftCursor = pLeft.iTable;
                        pNew.u.leftColumn = pLeft.iColumn;
                        sqliteinth.testcase((prereqLeft | extraRight) != prereqLeft);
                        pNew.prereqRight = prereqLeft | extraRight;
                        pNew.prereqAll = prereqAll;
                        pNew.eOperator = wherec.operatorMask(pDup.Operator);
                    }
                }
#if !SQLITE_OMIT_BETWEEN_OPTIMIZATION
                ///If a term is the BETWEEN operator, create two new virtual terms
                ///that define the range that the BETWEEN implements.  For example:
                ///
                ///a BETWEEN b AND c
                ///
                ///is converted into:
                ///
                ///(a BETWEEN b AND c) AND (a>=b) AND (a<=c)
                ///
                ///The two new terms are added onto the end of the WhereClause object.
                ///The new terms are "dynamic" and are children of the original BETWEEN
                ///term.  That means that if the BETWEEN term is coded, the children are
                ///skipped.  Or, if the children are satisfied by an index, the original
                ///BETWEEN term is skipped.
                else
                    if (pExpr.Operator == TokenType.TK_BETWEEN && pWC.Operator == TokenType.TK_AND)
                    {
                        ExprList pList = pExpr.x.pList;
                        int i;
                        TokenType[] ops = new TokenType[] {
							TokenType.TK_GE,
							TokenType.TK_LE
						};
                        Debug.Assert(pList != null);
                        Debug.Assert(pList.Count == 2);
                        for (i = 0; i < 2; i++)
                        {
                            Expr pNewExpr;
                            int idxNew;
                            pNewExpr = pParse.sqlite3PExpr(ops[i], exprc.sqlite3ExprDup(db, pExpr.pLeft, 0), exprc.sqlite3ExprDup(db, pList.a[i].pExpr, 0), null);
                            idxNew = pWC.whereClauseInsert(pNewExpr, WhereTermFlags.TERM_VIRTUAL | WhereTermFlags.TERM_DYNAMIC);
                            sqliteinth.testcase(idxNew == 0);
                            this.exprAnalyze(pWC, idxNew);
                            pTerm = pWC.a[idxTerm];
                            pWC.a[idxNew].iParent = idxTerm;
                        }
                        pTerm.nChild = 2;
                    }
#endif
#if !(SQLITE_OMIT_OR_OPTIMIZATION) && !(SQLITE_OMIT_SUBQUERY)
                    ///Analyze a term that is composed of two or more subterms connected by
                    ///an OR operator.
                    else
                        if (pExpr.Operator == TokenType.TK_OR)
                        {
                            Debug.Assert(pWC.Operator == TokenType.TK_AND);
                            this.exprAnalyzeOrTerm(pWC, idxTerm);
                            pTerm = pWC.a[idxTerm];
                        }
#endif
#if !SQLITE_OMIT_LIKE_OPTIMIZATION
                ///Add constraints to reduce the search space on a LIKE or GLOB
                ///operator.
                ///
                ///A like pattern of the form "x LIKE 'abc%'" is changed into constraints
                ///
                ///x>='abc' AND x<'abd' AND x LIKE 'abc%'
                ///
                ///The last character of the prefix "abc" is incremented to form the
                ///termination condition "abd".
                if (pWC.Operator == TokenType.TK_AND && pParse.isLikeOrGlob(pExpr, ref pStr1, ref isComplete, ref noCase) != 0)
                {
                    Expr pLeft;
                    ///LHS of LIKE/GLOB operator 
                    Expr pStr2;
                    ///<param name="Copy of pStr1 "> RHS of LIKE/GLOB operator </param>
                    Expr pNewExpr1;
                    Expr pNewExpr2;
                    int idxNew1;
                    int idxNew2;
                    CollSeq pColl;
                    ///Collating sequence to use 
                    pLeft = pExpr.x.pList.a[1].pExpr;
                    pStr2 = exprc.sqlite3ExprDup(db, pStr1, 0);
                    ////if ( 0 == db.mallocFailed )
                    {
                        int c, pC;
                        ///Last character before the first wildcard 
                        pC = pStr2.u.zToken[StringExtensions.Strlen30(pStr2.u.zToken) - 1];
                        c = pC;
                        if (noCase)
                        {
                            ///The point is to increment the last character before the first
                            ///wildcard.  But if we increment '@', that will push it into the
                            ///alphabetic range where case conversions will mess up the
                            ///inequality.  To avoid this, make sure to also run the full
                            ///LIKE on all candidate expressions by clearing the isComplete flag
                            if (c == 'A' - 1)
                                isComplete = false;
                            ///<param name="EV: R">08207 </param>
                            c = _Custom.sqlite3UpperToLower[c];
                        }
                        pStr2.u.zToken = pStr2.u.zToken.Substring(0, StringExtensions.Strlen30(pStr2.u.zToken) - 1) + (char)(c + 1);
                        // pC = c + 1;
                    }
                    pColl = db.sqlite3FindCollSeq( SqliteEncoding.UTF8, noCase ? "NOCASE" : "BINARY", 0);
                    pNewExpr1 = pParse.sqlite3PExpr(TokenType.TK_GE, exprc.sqlite3ExprDup(db, pLeft, 0).sqlite3ExprSetColl(pColl), pStr1, 0);
                    idxNew1 = pWC.whereClauseInsert(pNewExpr1, WhereTermFlags.TERM_VIRTUAL | WhereTermFlags.TERM_DYNAMIC);
                    sqliteinth.testcase(idxNew1 == 0);
                    this.exprAnalyze(pWC, idxNew1);
                    pNewExpr2 = pParse.sqlite3PExpr(TokenType.TK_LT, exprc.sqlite3ExprDup(db, pLeft, 0).sqlite3ExprSetColl(pColl), pStr2, null);
                    idxNew2 = pWC.whereClauseInsert(pNewExpr2, WhereTermFlags.TERM_VIRTUAL | WhereTermFlags.TERM_DYNAMIC);
                    sqliteinth.testcase(idxNew2 == 0);
                    this.exprAnalyze(pWC, idxNew2);
                    pTerm = pWC.a[idxTerm];
                    if (isComplete)
                    {
                        pWC.a[idxNew1].iParent = idxTerm;
                        pWC.a[idxNew2].iParent = idxTerm;
                        pTerm.nChild = 2;
                    }
                }
#endif
#if !SQLITE_OMIT_VIRTUALTABLE
                ///Add a wherec.WO_MATCH auxiliary term to the constraint set if the
                ///current expression is of the form:  column MATCH expr.
                ///This information is used by the xBestIndex methods of
                ///virtual tables.  The native query optimizer does not attempt
                ///to do anything with MATCH functions.
                if (pExpr.isMatchOfColumn() != false)
                {
                    int idxNew;
                    Expr pRight, pLeft;
                    WhereTerm pNewTerm;
                    Bitmask prereqColumn, prereqExpr;
                    pRight = pExpr.x.pList.a[0].pExpr;
                    pLeft = pExpr.x.pList.a[1].pExpr;
                    prereqExpr = pMaskSet.exprTableUsage(pRight);
                    prereqColumn = pMaskSet.exprTableUsage(pLeft);
                    if ((prereqExpr & prereqColumn) == 0)
                    {
                        Expr pNewExpr;
                        pNewExpr = pParse.sqlite3PExpr(TokenType.TK_MATCH, null, exprc.sqlite3ExprDup(db, pRight, 0), null);
                        idxNew = pWC.whereClauseInsert(pNewExpr, WhereTermFlags.TERM_VIRTUAL | WhereTermFlags.TERM_DYNAMIC);
                        sqliteinth.testcase(idxNew == 0);
                        pNewTerm = pWC.a[idxNew];
                        pNewTerm.prereqRight = prereqExpr;
                        pNewTerm.leftCursor = pLeft.iTable;
                        pNewTerm.u.leftColumn = pLeft.iColumn;
                        pNewTerm.eOperator = wherec.WO_MATCH;
                        pNewTerm.iParent = idxTerm;
                        pTerm = pWC.a[idxTerm];
                        pTerm.nChild = 1;
                        pTerm.wtFlags |= WhereTermFlags.TERM_COPIED;
                        pNewTerm.prereqAll = pTerm.prereqAll;
                    }
                }
#endif
#if SQLITE_ENABLE_STAT2
																																																																																												      /* When sqlite_stat2 histogram data is available an operator of the
  ** form "x IS NOT NULL" can sometimes be evaluated more efficiently
  ** as "x>NULL" if x is not an INTEGER PRIMARY KEY.  So construct a
  ** virtual term of that form.
  **
  ** Note that the virtual term must be tagged with WhereTermFlags.TERM_VNULL.  This
  ** WhereTermFlags.TERM_VNULL tag will suppress the not-null check at the beginning
  ** of the loop.  Without the WhereTermFlags.TERM_VNULL flag, the not-null check at
  ** the start of the loop will prevent any results from being returned.
  */
      if ( pExpr.op == TokenType.TK_NOTNULL
       && pExpr.pLeft.op == TokenType.TK_COLUMN
       && pExpr.pLeft.iColumn >= 0
      )
      {
        Expr pNewExpr;
        Expr pLeft = pExpr.pLeft;
        int idxNew;
        WhereTerm pNewTerm;

        pNewExpr = sqlite3PExpr( pParse, TokenType.TK_GT,
                                exprc.sqlite3ExprDup( db, pLeft, 0 ),
                                sqlite3PExpr( pParse, TokenType.TK_NULL, 0, 0, 0 ), 0 );

        idxNew = whereClauseInsert( pWC, pNewExpr,
                                  WhereTermFlags.TERM_VIRTUAL | WhereTermFlags.TERM_DYNAMIC | WhereTermFlags.TERM_VNULL );
        if ( idxNew != 0 )
        {
          pNewTerm = pWC.a[idxNew];
          pNewTerm.prereqRight = 0;
          pNewTerm.leftCursor = pLeft.iTable;
          pNewTerm.u.leftColumn = pLeft.iColumn;
          pNewTerm.eOperator = wherec.WO_GT;
          pNewTerm.iParent = idxTerm;
          pTerm = pWC.a[idxTerm];
          pTerm.nChild = 1;
          pTerm.wtFlags |= WhereTermFlags.TERM_COPIED;
          pNewTerm.prereqAll = pTerm.prereqAll;
        }
      }
#endif
                ///Prevent ON clause terms of a LEFT JOIN from being used to drive
                ///an index for tables to the left of the join.
                pTerm.prereqRight |= extraRight;
            }




        }
}
