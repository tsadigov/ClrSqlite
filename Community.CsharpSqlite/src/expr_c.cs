#define SQLITE_MAX_EXPR_DEPTH
using System;
using System.Diagnostics;
using System.Text;
using Bitmask=System.UInt64;
using i64=System.Int64;
using u8=System.Byte;
using u32=System.UInt32;
using u16=System.UInt16;
using Pgno=System.UInt32;
#if !SQLITE_MAX_VARIABLE_NUMBER
using ynVar=System.Int16;
#else
using ynVar = System.Int32; 
#endif
namespace Community.CsharpSqlite
{
    using Vdbe=Sqlite3.Vdbe;
    using Parse=Sqlite3.Parse;
    

        public class exprc
        {
            ///<summary>
            /// 2001 September 15
            ///
            /// The author disclaims copyright to this source code.  In place of
            /// a legal notice, here is a blessing:
            ///
            ///    May you do good and not evil.
            ///    May you find forgiveness for yourself and forgive others.
            ///    May you share freely, never taking more than you give.
            ///
            ///
            /// This file contains routines used for analyzing expressions and
            /// for generating VDBE code that evaluates expressions in SQLite.
            ///
            ///  Included in SQLite3 port to C#-SQLite;  2008 Noah B Hart
            ///  C#-SQLite is an independent reimplementation of the SQLite software library
            ///
            ///  SQLITE_SOURCE_ID: 2011-06-23 19:49:22 4374b7e83ea0a3fbc3691f9c0c936272862f32f2
            ///
            ///
            ///
            ///</summary>
            //#include "sqliteInt.h"
            ///<summary>
            /// Return the 'affinity' of the expression pExpr if any.
            ///
            /// If pExpr is a column, a reference to a column via an 'AS' alias,
            /// or a sub-select with a column as the return value, then the
            /// affinity of that column is returned. Otherwise, 0x00 is returned,
            /// indicating no affinity for the expression.
            ///
            /// i.e. the WHERE clause expresssions in the following statements all
            /// have an affinity:
            ///
            /// CREATE TABLE t1(a);
            /// SELECT * FROM t1 WHERE a;
            /// SELECT a AS b FROM t1 WHERE b;
            /// SELECT * FROM t1 WHERE (select a from t1);
            ///
            ///</summary>
            ///<summary>
            /// Set the explicit collating sequence for an expression to the
            /// collating sequence supplied in the second argument.
            ///
            ///</summary>
            ///<summary>
            /// Set the collating sequence for expression pExpr to be the collating
            /// sequence named by pToken.   Return a pointer to the revised expression.
            /// The collating sequence is marked as "explicit" using the ExprFlags.EP_ExpCollate
            /// flag.  An explicit collating sequence will override implicit
            /// collating sequences.
            ///
            ///</summary>
            ///<summary>
            /// Return the default collation sequence for the expression pExpr. If
            /// there is no default collation type, return 0.
            ///
            ///</summary>
            ///<summary>
            /// Return the P5 value that should be used for a binary comparison
            /// opcode ( OpCode.OP_Eq,  OpCode.OP_Ge etc.) used to compare pExpr1 and pExpr2.
            ///
            ///</summary>
            ///<summary>
            /// Return a pointer to the collation sequence that should be used by
            /// a binary comparison operator comparing pLeft and pRight.
            ///
            /// If the left hand expression has a collating sequence type, then it is
            /// used. Otherwise the collation sequence for the right hand expression
            /// is used, or the default (BINARY) if neither expression has a collating
            /// type.
            ///
            /// Argument pRight (but not pLeft) may be a null pointer. In this case,
            /// it is not considered.
            ///
            ///</summary>
            ///<summary>
            /// Generate code for a comparison operator.
            ///
            ///</summary>
#if SQLITE_MAX_EXPR_DEPTH
            ///<summary>
            /// Check that argument nHeight is less than or equal to the maximum
            /// expression depth allowed. If it is not, leave an error message in
            /// pParse.
            ///</summary>
            ///<summary>
            /// Set the Expr.nHeight variable in the structure passed as an
            /// argument. An expression with no children, Expr.x.pList or
            /// Expr.x.pSelect member has a height of 1. Any other expression
            /// has a height equal to the maximum height of any other
            /// referenced Expr plus one.
            ///
            ///</summary>
            ///<summary>
            /// Set the Expr.nHeight variable using the exprSetHeight() function. If
            /// the height is greater than the maximum allowed expression depth,
            /// leave an error in pParse.
            ///
            ///</summary>
            ///<summary>
            /// Return the maximum height of any expression tree referenced
            /// by the select statement passed as an argument.
            ///
            ///</summary>
#else
																																																//define exprSetHeight(y)
#endif
            ///<summary>exprc.sqlite3ExprAlloc
            /// This routine is the core allocator for Expr nodes.
            ///
            /// Construct a new expression node and return a pointer to it.  Memory
            /// for this node and for the pToken argument is a single allocation
            /// obtained from sqlite3DbMalloc().  The calling function
            /// is responsible for making sure the node eventually gets freed.
            ///
            /// If dequote is true, then the token (if it exists) is dequoted.
            /// If dequote is false, no dequoting is performance.  The deQuote
            /// parameter is ignored if pToken is NULL or if the token does not
            /// appear to be quoted.  If the quotes were of the form "..." (double-quotes)
            /// then the ExprFlags.EP_DblQuoted flag is set on the expression node.
            ///
            /// Special case:  If op==Sqlite3.TK_INTEGER and pToken points to a string that
            /// can be translated into a 32-bit integer, then the token is not
            /// stored in u.zToken.  Instead, the integer values is written
            /// into u.iValue and the ExprFlags.EP_IntValue flag is set.  No extra storage
            /// is allocated to hold the integer text and the dequote flag is ignored.
            ///</summary>
            public static Expr CreateExpr(sqlite3 db,///
                ///<summary>
                ///Handle for sqlite3DbMallocZero() (may be null) 
                ///</summary>
            int op,///
                ///<summary>
                ///Expression opcode 
                ///</summary>
            Token pToken,///
                ///<summary>
                ///Token argument.  Might be NULL 
                ///</summary>
            bool dequote///
                ///<summary>
                ///True to dequote 
                ///</summary>
            )
            {
                TokenType p_operator = (TokenType)op;
                Expr pNew;
                int nExtra = 0;
                int iValue = 0;
                if (pToken != null)
                {
                    if (p_operator != TokenType.TK_INTEGER || pToken.zRestSql == null || pToken.zRestSql.Length == 0 || Converter.sqlite3GetInt32(pToken.zRestSql.ToString(), ref iValue) == false)
                    {
                        nExtra = pToken.Length + 1;
                        Debug.Assert(iValue >= 0);
                    }
                }
                pNew = new Expr();
                pNew.token = pToken;
                //sqlite3DbMallocZero(db, sizeof(Expr)+nExtra);
                if (pNew != null)
                {
                    pNew.Operator = p_operator;
                    pNew.iAgg = -1;
                    if (pToken != null)
                    {
                        if (nExtra == 0)
                        {
                            pNew.Flags |= ExprFlags.EP_IntValue;
                            pNew.u.iValue = iValue;
                        }
                        else
                        {
                            int c;
                            //pNew.u.zToken = (char)&pNew[1];
                            if (pToken.Length > 0)
                                pNew.u.zToken = pToken.zRestSql.Substring(0, pToken.Length);
                            //memcpy(pNew.u.zToken, pToken.z, pToken.n);
                            else
                                if (pToken.Length == 0 && pToken.zRestSql == "")
                                    pNew.u.zToken = "";
                            //pNew.u.zToken[pToken.n] = 0;
                            if (dequote && nExtra >= 3 && ((c = pToken.zRestSql[0]) == '\'' || c == '"' || c == '[' || c == '`'))
                            {
#if DEBUG_CLASS_EXPR || DEBUG_CLASS_ALL
																																																																																																																																																																								StringExtensions.sqlite3Dequote(ref pNew.u._zToken);
#else
                                StringExtensions.sqlite3Dequote(ref pNew.u.zToken);
#endif
                                if (c == '"')
                                    pNew.Flags |= ExprFlags.EP_DblQuoted;
                            }
                        }
                    }
#if SQLITE_MAX_EXPR_DEPTH
                    pNew.nHeight = 1;
#endif
                }
                return pNew;
            }
            ///<summary>
            /// Allocate a new expression node from a zero-terminated token that has
            /// already been dequoted.
            ///
            ///</summary>
            public static Expr sqlite3Expr(sqlite3 db,///
                ///<summary>
                ///Handle for sqlite3DbMallocZero() (may be null) 
                ///</summary>
            int op,///
                ///<summary>
                ///Expression opcode 
                ///</summary>
            string zToken///
                ///<summary>
                ///Token argument.  Might be NULL 
                ///</summary>
            )
            {
                Token x = new Token();
                x.zRestSql = zToken;
                x.Length = !String.IsNullOrEmpty(zToken) ? StringExtensions.sqlite3Strlen30(zToken) : 0;
                return CreateExpr(db, op, x, false);
            }
            ///
            ///<summary>
            ///Attach subtrees pLeft and pRight to the Expr node pRoot.
            ///
            ///If pRoot==NULL that means that a memory allocation error has occurred.
            ///In that case, delete the subtrees pLeft and pRight.
            ///
            ///</summary>
            public static void sqlite3ExprAttachSubtrees(sqlite3 db, Expr pRoot, Expr pLeft, Expr pRight)
            {
                if (pRoot == null)
                {
                    //Debug.Assert( db.mallocFailed != 0 );
                    exprc.sqlite3ExprDelete(db, ref pLeft);
                    exprc.sqlite3ExprDelete(db, ref pRight);
                }
                else
                {
                    if (pRight != null)
                    {
                        pRoot.pRight = pRight;
                        if ((pRight.Flags & ExprFlags.EP_ExpCollate) != 0)
                        {
                            pRoot.Flags |= ExprFlags.EP_ExpCollate;
                            pRoot.pColl = pRight.pColl;
                        }
                    }
                    if (pLeft != null)
                    {
                        pRoot.pLeft = pLeft;
                        if ((pLeft.Flags & ExprFlags.EP_ExpCollate) != 0)
                        {
                            pRoot.Flags |= ExprFlags.EP_ExpCollate;
                            pRoot.pColl = pLeft.pColl;
                        }
                    }
                    pRoot.exprSetHeight();
                }
            }
            ///<summary>
            /// Allocate a Expr node which joins as many as two subtrees.
            ///
            /// One or both of the subtrees can be NULL.  Return a pointer to the new
            /// Expr node.  Or, if an OOM error occurs, set pParse->db->mallocFailed,
            /// free the subtrees and return NULL.
            ///
            ///</summary>
            // OVERLOADS, so I don't need to rewrite parse.c
            ///
            ///<summary>
            ///Join two expressions using an AND operator.  If either expression is
            ///NULL, then just return the other expression.
            ///
            ///</summary>
            public static Expr sqlite3ExprAnd(sqlite3 db, Expr pLeft, Expr pRight)
            {
                if (pLeft == null)
                {
                    return pRight;
                }
                else
                    if (pRight == null)
                    {
                        return pLeft;
                    }
                    else
                    {
                        Expr pNew = CreateExpr(db, Sqlite3.TK_AND, null, false);
                        exprc.sqlite3ExprAttachSubtrees(db, pNew, pLeft, pRight);
                        return pNew;
                    }
            }
            ///<summary>
            /// Construct a new expression node for a function with multiple
            /// arguments.
            ///
            ///</summary>
            // OVERLOADS, so I don't need to rewrite parse.c
            ///<summary>
            /// Assign a variable number to an expression that encodes a wildcard
            /// in the original SQL statement.
            ///
            /// Wildcards consisting of a single "?" are assigned the next sequential
            /// variable number.
            ///
            /// Wildcards of the form "?nnn" are assigned the number "nnn".  We make
            /// sure "nnn" is not too be to avoid a denial of service attack when
            /// the SQL statement comes from an external source.
            ///
            /// Wildcards of the form ":aaa", "@aaa" or "$aaa" are assigned the same number
            /// as the previous instance of the same wildcard.  Or if this is the first
            /// instance of the wildcard, the next sequenial variable number is
            /// assigned.
            ///
            ///</summary>
            ///<summary>
            /// Recursively delete an expression tree.
            ///
            ///</summary>
            public static void sqlite3ExprDelete(sqlite3 db, ref Expr p)
            {
                if (p == null)
                    return;
                ///
                ///<summary>
                ///</summary>
                ///<param name="Sanity check: Assert that the IntValue is non">negative if it exists </param>
                Debug.Assert(!p.ExprHasProperty(ExprFlags.EP_IntValue) || p.u.iValue >= 0);
                if (!p.ExprHasAnyProperty(ExprFlags.EP_TokenOnly))
                {
                    exprc.sqlite3ExprDelete(db, ref p.pLeft);
                    exprc.sqlite3ExprDelete(db, ref p.pRight);
                    if (!p.ExprHasProperty(ExprFlags.EP_Reduced) && (p.flags2 & Sqlite3.EP2_MallocedToken) != 0)
                    {
#if DEBUG_CLASS_EXPR || DEBUG_CLASS_ALL
																																																																																																																								sqlite3DbFree( db, ref p.u._zToken );
#else
                        db.sqlite3DbFree(ref p.u.zToken);
#endif
                    }
                    if (p.ExprHasProperty(ExprFlags.EP_xIsSelect))
                    {
                        SelectMethods.sqlite3SelectDelete(db, ref p.x.pSelect);
                    }
                    else
                    {
                        exprc.sqlite3ExprListDelete(db, ref p.x.pList);
                    }
                }
                if (!p.ExprHasProperty(ExprFlags.EP_Static))
                {
                    db.sqlite3DbFree(ref p);
                }
            }
            ///<summary>
            /// Return the number of bytes allocated for the expression structure
            /// passed as the first argument. This is always one of EXPR_FULLSIZE,
            /// EXPR_REDUCEDSIZE or EXPR_TOKENONLYSIZE.
            ///
            ///</summary>
            static int exprStructSize(Expr p)
            {
                if (p.ExprHasProperty(ExprFlags.EP_TokenOnly))
                    return Sqlite3.EXPR_TOKENONLYSIZE;
                if (p.ExprHasProperty(ExprFlags.EP_Reduced))
                    return Sqlite3.EXPR_REDUCEDSIZE;
                return Sqlite3.EXPR_FULLSIZE;
            }
            ///<summary>
            /// This function is similar to exprc.sqlite3ExprDup(), except that if pzBuffer
            /// is not NULL then *pzBuffer is assumed to point to a buffer large enough
            /// to store the copy of expression p, the copies of p->u.zToken
            /// (if applicable), and the copies of the p->pLeft and p->pRight expressions,
            /// if any. Before returning, *pzBuffer is set to the first byte passed the
            /// portion of the buffer copied into by this function.
            ///
            ///</summary>
            static Expr exprDup(sqlite3 db, Expr p, int flags, ref Expr pzBuffer)
            {
                Expr pNew = null;
                ///
                ///<summary>
                ///Value to return 
                ///</summary>
                if (p != null)
                {
                    bool isReduced = (flags & Sqlite3.EXPRDUP_REDUCE) != 0;
                    Expr zAlloc = new Expr();
                    ExprFlags staticFlag = 0;
                    Debug.Assert(pzBuffer == null || isReduced);
                    ///
                    ///<summary>
                    ///Figure out where to write the new Expr structure. 
                    ///</summary>
                    //if ( pzBuffer !=null)
                    //{
                    //  zAlloc = pzBuffer;
                    //  staticFlag = ExprFlags.EP_Static;
                    //}
                    //else
                    //{
                    ///Expr  zAlloc = new Expr();//sqlite3DbMallocRaw( db, dupedExprSize( p, flags ) );
                    //}
                    // (Expr)zAlloc;
                    //if ( pNew != null )
                    {
                        ///
                        ///<summary>
                        ///Set nNewSize to the size allocated for the structure pointed to
                        ///by pNew. This is either EXPR_FULLSIZE, EXPR_REDUCEDSIZE or
                        ///EXPR_TOKENONLYSIZE. nToken is set to the number of bytes consumed
                        ///</summary>
                        ///<param name="by the copy of the p">>u.zToken string (if any).</param>
                        ///<param name=""></param>
                        int nStructSize = p.dupedExprStructSize(flags);
                        int nNewSize = nStructSize & 0xfff;
                        int nToken;
                        if (!p.ExprHasProperty(ExprFlags.EP_IntValue) && !String.IsNullOrEmpty(p.u.zToken))
                        {
                            nToken = StringExtensions.sqlite3Strlen30(p.u.zToken);
                        }
                        else
                        {
                            nToken = 0;
                        }
                        if (isReduced)
                        {
                            Debug.Assert(!p.ExprHasProperty(ExprFlags.EP_Reduced));
                            pNew = p.Copy((ExprFlags)Sqlite3.EXPR_TOKENONLYSIZE);
                            //memcpy( zAlloc, p, nNewSize );
                        }
                        else
                        {
                            int nSize = exprStructSize(p);
                            //memcpy( zAlloc, p, nSize );
                            pNew = p.Copy();
                            //memset( &zAlloc[nSize], 0, EXPR_FULLSIZE - nSize );
                        }
                        ///
                        ///<summary>
                        ///Set the ExprFlags.EP_Reduced, ExprFlags.EP_TokenOnly, and ExprFlags.EP_Static flags appropriately. 
                        ///</summary>
                        unchecked
                        {
                            pNew.Flags &= (~(ExprFlags.EP_Reduced | ExprFlags.EP_TokenOnly | ExprFlags.EP_Static));
                        }
                        pNew.Flags |= ((ExprFlags)nStructSize & (ExprFlags.EP_Reduced | ExprFlags.EP_TokenOnly));
                        pNew.Flags |= staticFlag;
                        ///
                        ///<summary>
                        ///</summary>
                        ///<param name="Copy the p">>u.zToken string, if any. </param>
                        if (nToken != 0)
                        {
                            string zToken;
                            // = pNew.u.zToken = (char)&zAlloc[nNewSize];
                            zToken = p.u.zToken.Substring(0, nToken);
                            // memcpy( zToken, p.u.zToken, nToken );
                        }
                        if (0 == ((p.Flags | pNew.Flags) & ExprFlags.EP_TokenOnly))
                        {
                            ///
                            ///<summary>
                            ///Fill in the pNew.x.pSelect or pNew.x.pList member. 
                            ///</summary>
                            if (p.ExprHasProperty(ExprFlags.EP_xIsSelect))
                            {
                                pNew.x.pSelect = exprc.sqlite3SelectDup(db, p.x.pSelect, isReduced ? 1 : 0);
                            }
                            else
                            {
                                pNew.x.pList = exprc.sqlite3ExprListDup(db, p.x.pList, isReduced ? 1 : 0);
                            }
                        }
                        ///
                        ///<summary>
                        ///Fill in pNew.pLeft and pNew.pRight. 
                        ///</summary>
                        if (pNew.ExprHasAnyProperty(ExprFlags.EP_Reduced | ExprFlags.EP_TokenOnly))
                        {
                            //zAlloc += dupedExprNodeSize( p, flags );
                            if (pNew.ExprHasProperty(ExprFlags.EP_Reduced))
                            {
                                pNew.pLeft = exprDup(db, p.pLeft, Sqlite3.EXPRDUP_REDUCE, ref pzBuffer);
                                pNew.pRight = exprDup(db, p.pRight, Sqlite3.EXPRDUP_REDUCE, ref pzBuffer);
                            }
                            //if ( pzBuffer != null )
                            //{
                            //  pzBuffer = zAlloc;
                            //}
                        }
                        else
                        {
                            pNew.flags2 = 0;
                            if (!p.ExprHasAnyProperty(ExprFlags.EP_TokenOnly))
                            {
                                pNew.pLeft = exprc.sqlite3ExprDup(db, p.pLeft, 0);
                                pNew.pRight = exprc.sqlite3ExprDup(db, p.pRight, 0);
                            }
                        }
                    }
                }
                return pNew;
            }
            ///<summary>
            /// The following group of routines make deep copies of expressions,
            /// expression lists, ID lists, and select statements.  The copies can
            /// be deleted (by being passed to their respective ...Delete() routines)
            /// without effecting the originals.
            ///
            /// The expression list, ID, and source lists return by exprc.sqlite3ExprListDup(),
            /// exprc.sqlite3IdListDup(), and exprc.sqlite3SrcListDup() can not be further expanded
            /// by subsequent calls to sqlite*ListAppend() routines.
            ///
            /// Any tables that the SrcList might point to are not duplicated.
            ///
            /// The flags parameter contains a combination of the EXPRDUP_XXX flags.
            /// If the EXPRDUP_REDUCE flag is set, then the structure returned is a
            /// truncated version of the usual Expr structure that will be stored as
            /// part of the in-memory representation of the database schema.
            ///
            ///</summary>
            public static Expr sqlite3ExprDup(sqlite3 db, Expr p, int flags)
            {
                Expr ExprDummy = null;
                return exprDup(db, p, flags, ref ExprDummy);
            }
            public static ExprList sqlite3ExprListDup(sqlite3 db, ExprList p, int flags)
            {
                ExprList pNew;
                ExprList_item pItem;
                ExprList_item pOldItem;
                int i;
                if (p == null)
                    return null;
                pNew = new ExprList();
                //sqlite3DbMallocRaw(db, sizeof(*pNew) );
                //if ( pNew == null ) return null;
                pNew.iECursor = 0;
                pNew.nExpr = pNew.nAlloc = p.nExpr;
                pNew.a = new ExprList_item[p.nExpr];
                //sqlite3DbMallocRaw(db,  p.nExpr*sizeof(p.a[0]) );
                //if( pItem==null ){
                //  sqlite3DbFree(db,ref pNew);
                //  return null;
                //}
                //pOldItem = p.a;
                for (i = 0; i < p.nExpr; i++)
                {
                    //pItem++, pOldItem++){
                    pItem = pNew.a[i] = new ExprList_item();
                    pOldItem = p.a[i];
                    Expr pOldExpr = pOldItem.pExpr;
                    pItem.pExpr = exprc.sqlite3ExprDup(db, pOldExpr, flags);
                    pItem.zName = pOldItem.zName;
                    // sqlite3DbStrDup(db, pOldItem.zName);
                    pItem.zSpan = pOldItem.zSpan;
                    // sqlite3DbStrDup( db, pOldItem.zSpan );
                    pItem.sortOrder = pOldItem.sortOrder;
                    pItem.done = 0;
                    pItem.iCol = pOldItem.iCol;
                    pItem.iAlias = pOldItem.iAlias;
                }
                return pNew;
            }
            ///<summary>
            /// If cursors, triggers, views and subqueries are all omitted from
            /// the build, then none of the following routines, except for
            /// exprc.sqlite3SelectDup(), can be called. exprc.sqlite3SelectDup() is sometimes
            /// called with a NULL argument.
            ///
            ///</summary>
#if !SQLITE_OMIT_VIEW || !SQLITE_OMIT_TRIGGER  || !SQLITE_OMIT_SUBQUERY
            public static SrcList sqlite3SrcListDup(sqlite3 db, SrcList p, int flags)
            {
                SrcList pNew;
                int i;
                int nByte;
                if (p == null)
                    return null;
                //nByte = sizeof(*p) + (p.nSrc>0 ? sizeof(p.a[0]) * (p.nSrc-1) : 0);
                pNew = new SrcList();
                //sqlite3DbMallocRaw(db, nByte );
                if (p.nSrc > 0)
                    pNew.a = new SrcList_item[p.nSrc];
                if (pNew == null)
                    return null;
                pNew.nSrc = pNew.nAlloc = p.nSrc;
                for (i = 0; i < p.nSrc; i++)
                {
                    pNew.a[i] = new SrcList_item();
                    SrcList_item pNewItem = pNew.a[i];
                    SrcList_item pOldItem = p.a[i];
                    Table pTab;
                    pNewItem.zDatabase = pOldItem.zDatabase;
                    // sqlite3DbStrDup(db, pOldItem.zDatabase);
                    pNewItem.zName = pOldItem.zName;
                    // sqlite3DbStrDup(db, pOldItem.zName);
                    pNewItem.zAlias = pOldItem.zAlias;
                    // sqlite3DbStrDup(db, pOldItem.zAlias);
                    pNewItem.jointype = pOldItem.jointype;
                    pNewItem.iCursor = pOldItem.iCursor;
                    pNewItem.isPopulated = pOldItem.isPopulated;
                    pNewItem.zIndex = pOldItem.zIndex;
                    // sqlite3DbStrDup( db, pOldItem.zIndex );
                    pNewItem.notIndexed = pOldItem.notIndexed;
                    pNewItem.pIndex = pOldItem.pIndex;
                    pTab = pNewItem.pTab = pOldItem.pTab;
                    if (pTab != null)
                    {
                        pTab.nRef++;
                    }
                    pNewItem.pSelect = exprc.sqlite3SelectDup(db, pOldItem.pSelect, flags);
                    pNewItem.pOn = exprc.sqlite3ExprDup(db, pOldItem.pOn, flags);
                    pNewItem.pUsing = exprc.sqlite3IdListDup(db, pOldItem.pUsing);
                    pNewItem.colUsed = pOldItem.colUsed;
                }
                return pNew;
            }
            public static IdList sqlite3IdListDup(sqlite3 db, IdList p)
            {
                IdList pNew;
                int i;
                if (p == null)
                    return null;
                pNew = new IdList();
                //sqlite3DbMallocRaw(db, sizeof(*pNew) );
                if (pNew == null)
                    return null;
                pNew.nId = pNew.nAlloc = p.nId;
                pNew.a = new IdList_item[p.nId];
                //sqlite3DbMallocRaw(db, p.nId*sizeof(p.a[0]) );
                if (pNew.a == null)
                {
                    db.sqlite3DbFree(ref pNew);
                    return null;
                }
                for (i = 0; i < p.nId; i++)
                {
                    pNew.a[i] = new IdList_item();
                    IdList_item pNewItem = pNew.a[i];
                    IdList_item pOldItem = p.a[i];
                    pNewItem.zName = pOldItem.zName;
                    // sqlite3DbStrDup(db, pOldItem.zName);
                    pNewItem.idx = pOldItem.idx;
                }
                return pNew;
            }
            public static Select sqlite3SelectDup(sqlite3 db, Select p, int flags)
            {
                Select pNew;
                if (p == null)
                    return null;
                pNew = new Select();
                //sqlite3DbMallocRaw(db, sizeof(*p) );
                //if ( pNew == null ) return null;
                pNew.pEList = exprc.sqlite3ExprListDup(db, p.pEList, flags);
                pNew.pSrc = exprc.sqlite3SrcListDup(db, p.pSrc, flags);
                pNew.pWhere = exprc.sqlite3ExprDup(db, p.pWhere, flags);
                pNew.pGroupBy = exprc.sqlite3ExprListDup(db, p.pGroupBy, flags);
                pNew.pHaving = exprc.sqlite3ExprDup(db, p.pHaving, flags);
                pNew.pOrderBy = exprc.sqlite3ExprListDup(db, p.pOrderBy, flags);
                pNew.TokenOp  = p.TokenOp ;
                pNew.pPrior = exprc.sqlite3SelectDup(db, p.pPrior, flags);
                pNew.pLimit = exprc.sqlite3ExprDup(db, p.pLimit, flags);
                pNew.pOffset = exprc.sqlite3ExprDup(db, p.pOffset, flags);
                pNew.iLimit = 0;
                pNew.iOffset = 0;
                pNew.selFlags = (p.selFlags & ~SelectFlags.UsesEphemeral);
                pNew.pRightmost = null;
                pNew.addrOpenEphm[0] = -1;
                pNew.addrOpenEphm[1] = -1;
                pNew.addrOpenEphm[2] = -1;
                return pNew;
            }
#else
																																																Select exprc.sqlite3SelectDup(sqlite3 db, Select p, int flags){
Debug.Assert( p==null );
return null;
}
#endif
            ///<summary>
            /// Add a new element to the end of an expression list.  If pList is
            /// initially NULL, then create a new expression list.
            ///
            /// If a memory allocation error occurs, the entire list is freed and
            /// NULL is returned.  If non-NULL is returned, then it is guaranteed
            /// that the new entry was successfully appended.
            ///</summary>
            // OVERLOADS, so I don't need to rewrite parse.c
            ///<summary>
            /// Set the ExprList.a[].zName element of the most recently added item
            /// on the expression list.
            ///
            /// pList might be NULL following an OOM error.  But pName should never be
            /// NULL.  If a memory allocation fails, the pParse.db.mallocFailed flag
            /// is set.
            ///
            ///</summary>
            ///<summary>
            /// Set the ExprList.a[].zSpan element of the most recently added item
            /// on the expression list.
            ///
            /// pList might be NULL following an OOM error.  But pSpan should never be
            /// NULL.  If a memory allocation fails, the pParse.db.mallocFailed flag
            /// is set.
            ///
            ///</summary>
            ///<summary>
            /// If the expression list pEList contains more than iLimit elements,
            /// leave an error message in pParse.
            ///
            ///</summary>
            ///<summary>
            /// Delete an entire expression list.
            ///
            ///</summary>
            public static void sqlite3ExprListDelete(sqlite3 db, ref ExprList pList)
            {
                int i;
                ExprList_item pItem;
                if (pList == null)
                    return;
                Debug.Assert(pList.a != null || (pList.nExpr == 0 && pList.nAlloc == 0));
                Debug.Assert(pList.nExpr <= pList.nAlloc);
                for (i = 0; i < pList.nExpr; i++)
                {
                    if ((pItem = pList.a[i]) != null)
                    {
                        exprc.sqlite3ExprDelete(db, ref pItem.pExpr);
                        db.sqlite3DbFree(ref pItem.zName);
                        db.sqlite3DbFree(ref pItem.zSpan);
                    }
                }
                db.sqlite3DbFree(ref pList.a);
                db.sqlite3DbFree(ref pList);
            }
            ///<summary>
            /// These routines are Walker callbacks.  Walker.u.pi is a pointer
            /// to an integer.  These routines are checking an expression to see
            /// if it is a constant.  Set *Walker.u.pi to 0 if the expression is
            /// not constant.
            ///
            /// These callback routines are used to implement the following:
            ///
            ///     sqlite3ExprIsConstant()
            ///     sqlite3ExprIsConstantNotJoin()
            ///     sqlite3ExprIsConstantOrFunction()
            ///
            ///
            ///</summary>
            public static WRC exprNodeIsConstant(Walker pWalker, ref Expr pExpr)
            {
                ///
                ///<summary>
                ///If pWalker.u.i is 3 then any term of the expression that comes from
                ///the ON or USING clauses of a join disqualifies the expression
                ///from being considered constant. 
                ///</summary>
                if (pWalker.u.i == 3 && pExpr.ExprHasAnyProperty(ExprFlags.EP_FromJoin))
                {
                    pWalker.u.i = 0;
                    return WRC.WRC_Abort;
                }
                switch (pExpr.Operator)
                {
                    ///
                    ///<summary>
                    ///Consider functions to be constant if all their arguments are constant
                    ///and pWalker.u.i==2 
                    ///</summary>
                    case TokenType.TK_FUNCTION:
                        if ((pWalker.u.i) == 2)
                            return 0;
                        goto case TokenType.TK_ID;
                    ///
                    ///<summary>
                    ///Fall through 
                    ///</summary>
                    case TokenType.TK_ID:
                    case TokenType.TK_COLUMN:
                    case TokenType.TK_AGG_FUNCTION:
                    case TokenType.TK_AGG_COLUMN:
                        sqliteinth.testcase(pExpr.Operator == TokenType.TK_ID);
                        sqliteinth.testcase(pExpr.Operator == TokenType.TK_COLUMN);
                        sqliteinth.testcase(pExpr.Operator == TokenType.TK_AGG_FUNCTION);
                        sqliteinth.testcase(pExpr.Operator == TokenType.TK_AGG_COLUMN);
                        pWalker.u.i = 0;
                        return WRC.WRC_Abort;
                    default:
                        sqliteinth.testcase(pExpr.Operator == TokenType.TK_SELECT);
                        ///
                        ///<summary>
                        ///selectNodeIsConstant will disallow 
                        ///</summary>
                        sqliteinth.testcase(pExpr.Operator == TokenType.TK_EXISTS);
                        ///
                        ///<summary>
                        ///selectNodeIsConstant will disallow 
                        ///</summary>
                        return WRC.WRC_Continue;
                }
            }
            public static WRC selectNodeIsConstant(Walker pWalker, Select NotUsed)
            {
                sqliteinth.UNUSED_PARAMETER(NotUsed);
                pWalker.u.i = 0;
                return WRC.WRC_Abort;
            }
            ///<summary>
            /// Generate an  OpCode.OP_IsNull instruction that tests register iReg and jumps
            /// to location iDest if the value in iReg is NULL.  The value in iReg
            /// was computed by pExpr.  If we can look at pExpr at compile-time and
            /// determine that it can never generate a NULL, then the  OpCode.OP_IsNull operation
            /// can be omitted.
            ///
            ///</summary>
            public static void sqlite3ExprCodeIsNullJump(Vdbe v,///
                ///<summary>
                ///The VDBE under construction 
                ///</summary>
            Expr pExpr,///
                ///<summary>
                ///Only generate  OpCode.OP_IsNull if this expr can be NULL 
                ///</summary>
            int iReg,///
                ///<summary>
                ///Test the value in this register for NULL 
                ///</summary>
            int iDest///
                ///<summary>
                ///Jump here if the value is null 
                ///</summary>
            )
            {
                if (pExpr.sqlite3ExprCanBeNull() != 0)
                {
                    v.sqlite3VdbeAddOp2(OpCode.OP_IsNull, iReg, iDest);
                }
            }
            ///<summary>
            /// Return TRUE if the given expression is a constant which would be
            /// unchanged by  OpCode.OP_Affinity with the affinity given in the second
            /// argument.
            ///
            /// This routine is used to determine if the  OpCode.OP_Affinity operation
            /// can be omitted.  When in doubt return FALSE.  A false negative
            /// is harmless.  A false positive, however, can result in the wrong
            /// answer.
            ///
            ///</summary>
            public static int sqlite3ExprNeedsNoAffinityChange(Expr p, char aff)
            {
                TokenType op;
                if (aff == sqliteinth.SQLITE_AFF_NONE)
                    return 1;
                while (p.Operator == TokenType.TK_UPLUS || p.Operator == TokenType.TK_UMINUS)
                {
                    p = p.pLeft;
                }
                op = p.Operator;
                if (op == TokenType.TK_REGISTER)
                    op = p.Operator2;
                switch (op)
                {
                    case TokenType.TK_INTEGER:
                        {
                            return (aff == sqliteinth.SQLITE_AFF_INTEGER || aff == sqliteinth.SQLITE_AFF_NUMERIC) ? 1 : 0;
                        }
                    case TokenType.TK_FLOAT:
                        {
                            return (aff == sqliteinth.SQLITE_AFF_REAL || aff == sqliteinth.SQLITE_AFF_NUMERIC) ? 1 : 0;
                        }
                    case TokenType.TK_STRING:
                        {
                            return (aff == sqliteinth.SQLITE_AFF_TEXT) ? 1 : 0;
                        }
                    case TokenType.TK_BLOB:
                        {
                            return 1;
                        }
                    case TokenType.TK_COLUMN:
                        {
                            Debug.Assert(p.iTable >= 0);
                            ///
                            ///<summary>
                            ///p cannot be part of a CHECK constraint 
                            ///</summary>
                            return (p.iColumn < 0 && (aff == sqliteinth.SQLITE_AFF_INTEGER || aff == sqliteinth.SQLITE_AFF_NUMERIC)) ? 1 : 0;
                        }
                    default:
                        {
                            return 0;
                        }
                }
            }
            ///
            ///<summary>
            ///</summary>
            ///<param name="Return TRUE if the given string is a row">id column name.</param>
            ///<param name=""></param>
            public static bool sqlite3IsRowid(string z)
            {
                if (z.Equals("_ROWID_", StringComparison.InvariantCultureIgnoreCase))
                    return true;
                if (z.Equals("ROWID", StringComparison.InvariantCultureIgnoreCase))
                    return true;
                if (z.Equals("OID", StringComparison.InvariantCultureIgnoreCase))
                    return true;
                return false;
            }
            ///<summary>
            /// Return true if we are able to the IN operator optimization on a
            /// query of the form
            ///
            ///       x IN (SELECT ...)
            ///
            /// Where the SELECT... clause is as specified by the parameter to this
            /// routine.
            ///
            /// The Select object passed in has already been preprocessed and no
            /// errors have been found.
            ///
            ///</summary>
#if !SQLITE_OMIT_SUBQUERY
            public static int isCandidateForInOpt(Select p)
            {
                SrcList pSrc;
                ExprList pEList;
                Table pTab;
                if (p == null)
                    return 0;
                ///
                ///<summary>
                ///</summary>
                ///<param name="right">hand side of IN is SELECT </param>
                if (p.pPrior != null)
                    return 0;
                ///
                ///<summary>
                ///Not a compound SELECT 
                ///</summary>
                if ((p.selFlags & (SelectFlags.Distinct | SelectFlags.Aggregate)) != 0)
                {
                    sqliteinth.testcase((p.selFlags & (SelectFlags.Distinct | SelectFlags.Aggregate)) == SelectFlags.Distinct);
                    sqliteinth.testcase((p.selFlags & (SelectFlags.Distinct | SelectFlags.Aggregate)) == SelectFlags.Aggregate);
                    return 0;
                    ///
                    ///<summary>
                    ///No DISTINCT keyword and no aggregate functions 
                    ///</summary>
                }
                Debug.Assert(p.pGroupBy == null);
                ///
                ///<summary>
                ///Has no GROUP BY clause 
                ///</summary>
                if (p.pLimit != null)
                    return 0;
                ///
                ///<summary>
                ///Has no LIMIT clause 
                ///</summary>
                Debug.Assert(p.pOffset == null);
                ///
                ///<summary>
                ///No LIMIT means no OFFSET 
                ///</summary>
                if (p.pWhere != null)
                    return 0;
                ///
                ///<summary>
                ///Has no WHERE clause 
                ///</summary>
                pSrc = p.pSrc;
                Debug.Assert(pSrc != null);
                if (pSrc.nSrc != 1)
                    return 0;
                ///
                ///<summary>
                ///Single term in FROM clause 
                ///</summary>
                if (pSrc.a[0].pSelect != null)
                    return 0;
                ///
                ///<summary>
                ///FROM is not a subquery or view 
                ///</summary>
                pTab = pSrc.a[0].pTab;
                if (Sqlite3.NEVER(pTab == null))
                    return 0;
                Debug.Assert(pTab.pSelect == null);
                ///
                ///<summary>
                ///FROM clause is not a view 
                ///</summary>
                if (pTab.IsVirtual())
                    return 0;
                ///
                ///<summary>
                ///FROM clause not a virtual table 
                ///</summary>
                pEList = p.pEList;
                if (pEList.nExpr != 1)
                    return 0;
                ///
                ///<summary>
                ///One column in the result set 
                ///</summary>
                if (pEList.a[0].pExpr.Operator != TokenType.TK_COLUMN)
                    return 0;
                ///
                ///<summary>
                ///Result is a column 
                ///</summary>
                return 1;
            }
#endif
            ///<summary>
            /// This function is used by the implementation of the IN (...) operator.
            /// It's job is to find or create a b-tree structure that may be used
            /// either to test for membership of the (...) set or to iterate through
            /// its members, skipping duplicates.
            ///
            /// The index of the cursor opened on the b-tree (database table, database index
            /// or ephermal table) is stored in pX->iTable before this function returns.
            /// The returned value of this function indicates the b-tree type, as follows:
            ///
            ///   IN_INDEX_ROWID - The cursor was opened on a database table.
            ///   IN_INDEX_INDEX - The cursor was opened on a database index.
            ///   sqliteinth.IN_INDEX_EPH -   The cursor was opened on a specially created and
            ///                    populated epheremal table.
            ///
            /// An existing b-tree may only be used if the SELECT is of the simple
            /// form:
            ///
            ///     SELECT <column> FROM <table>
            ///
            /// If the prNotFound parameter is 0, then the b-tree will be used to iterate
            /// through the set members, skipping any duplicates. In this case an
            /// epheremal table must be used unless the selected <column> is guaranteed
            /// to be unique - either because it is an INTEGER PRIMARY KEY or it
            /// has a UNIQUE constraint or UNIQUE index.
            ///
            /// If the prNotFound parameter is not 0, then the b-tree will be used
            /// for fast set membership tests. In this case an epheremal table must
            /// be used unless <column> is an INTEGER PRIMARY KEY or an index can
            /// be found with <column> as its left-most column.
            ///
            /// When the b-tree is being used for membership tests, the calling function
            /// needs to know whether or not the structure contains an SQL NULL
            /// value in order to correctly evaluate expressions like "X IN (Y, Z)".
            /// If there is any chance that the (...) might contain a NULL value at
            /// runtime, then a register is allocated and the register number written
            /// to *prNotFound. If there is no chance that the (...) contains a
            /// NULL value, then *prNotFound is left unchanged.
            ///
            /// If a register is allocated and its location stored in *prNotFound, then
            /// its initial value is NULL.  If the (...) does not remain constant
            /// for the duration of the query (i.e. the SELECT within the (...)
            /// is a correlated subquery) then the value of the allocated register is
            /// reset to NULL each time the subquery is rerun. This allows the
            /// caller to use vdbe code equivalent to the following:
            ///
            ///   if( register==NULL ){
            ///     has_null = <test if data structure contains null>
            ///     register = 1
            ///   }
            ///
            /// in order to avoid running the <test if data structure contains null>
            /// test more often than is necessary.
            ///</summary>
#if !SQLITE_OMIT_SUBQUERY
#endif
            ///<summary>
            /// Generate code for scalar subqueries used as a subquery expression, EXISTS,
            /// or IN operators.  Examples:
            ///
            ///     (SELECT a FROM b)          -- subquery
            ///     EXISTS (SELECT a FROM b)   -- EXISTS subquery
            ///     x IN (4,5,11)              -- IN operator with list on right-hand side
            ///     x IN (SELECT a FROM b)     -- IN operator with subquery on the right
            ///
            /// The pExpr parameter describes the expression that contains the IN
            /// operator or subquery.
            ///
            /// If parameter isRowid is non-zero, then expression pExpr is guaranteed
            /// to be of the form "<rowid> IN (?, ?, ?)", where <rowid> is a reference
            /// to some integer key column of a table B-Tree. In this case, use an
            /// intkey B-Tree to store the set of IN(...) values instead of the usual
            /// (slower) variable length keys B-Tree.
            ///
            /// If rMayHaveNull is non-zero, that means that the operation is an IN
            /// (not a SELECT or EXISTS) and that the RHS might contains NULLs.
            /// Furthermore, the IN is in a WHERE clause and that we really want
            /// to iterate over the RHS of the IN operator in order to quickly locate
            /// all corresponding LHS elements.  All this routine does is initialize
            /// the register given by rMayHaveNull to NULL.  Calling routines will take
            /// care of changing this register value to non-NULL if the RHS is NULL-free.
            ///
            /// If rMayHaveNull is zero, that means that the subquery is being used
            /// for membership testing only.  There is no need to initialize any
            /// registers to indicate the presense or absence of NULLs on the RHS.
            ///
            /// For a SELECT or EXISTS operator, return the register that holds the
            /// result.  For IN operators or if an error occurs, the return value is 0.
            ///</summary>
#if !SQLITE_OMIT_SUBQUERY
#endif
#if !SQLITE_OMIT_SUBQUERY
            ///
            ///<summary>
            ///Generate code for an IN expression.
            ///
            ///x IN (SELECT ...)
            ///x IN (value, value, ...)
            ///
            ///</summary>
            ///<param name="The left">hand side (RHS)</param>
            ///<param name="is an array of zero or more values.  The expression is true if the LHS is">is an array of zero or more values.  The expression is true if the LHS is</param>
            ///<param name="contained within the RHS.  The value of the expression is unknown (NULL)">contained within the RHS.  The value of the expression is unknown (NULL)</param>
            ///<param name="if the LHS is NULL or if the LHS is not contained within the RHS and the">if the LHS is NULL or if the LHS is not contained within the RHS and the</param>
            ///<param name="RHS contains one or more NULL values.">RHS contains one or more NULL values.</param>
            ///<param name=""></param>
            ///<param name="This routine generates code will jump to destIfFalse if the LHS is not ">This routine generates code will jump to destIfFalse if the LHS is not </param>
            ///<param name="contained within the RHS.  If due to NULLs we cannot determine if the LHS">contained within the RHS.  If due to NULLs we cannot determine if the LHS</param>
            ///<param name="is contained in the RHS then jump to destIfNull.  If the LHS is contained">is contained in the RHS then jump to destIfNull.  If the LHS is contained</param>
            ///<param name="within the RHS then fall through.">within the RHS then fall through.</param>
#endif
            ///<summary>
            /// Duplicate an 8-byte value
            ///</summary>
            //static char *dup8bytes(Vdbe v, string in){
            //  char *out = sqlite3DbMallocRaw(sqlite3VdbeDb(v), 8);
            //  if( out ){
            //    memcpy(out, in, 8);
            //  }
            //  return out;
            //}
#if !SQLITE_OMIT_FLOATING_POINT
            ///<summary>
            /// Generate an instruction that will put the floating point
            /// value described by z[0..n-1] into register iMem.
            ///
            /// The z[] string will probably not be zero-terminated.  But the
            /// z[n] character is guaranteed to be something that does not look
            /// like the continuation of the number.
            ///</summary>
            public static void codeReal(Vdbe v, string z, bool negateFlag, int iMem)
            {
                if (Sqlite3.ALWAYS(!String.IsNullOrEmpty(z)))
                {
                    double value = 0;
                    //string zV;
                    Converter.sqlite3AtoF(z, ref value, StringExtensions.sqlite3Strlen30(z), SqliteEncoding.UTF8);
                    Debug.Assert(!MathExtensions.sqlite3IsNaN(value));
                    ///
                    ///<summary>
                    ///The new AtoF never returns NaN 
                    ///</summary>
                    if (negateFlag)
                        value = -value;
                    //zV = dup8bytes(v,  value);
                    v.sqlite3VdbeAddOp4(OpCode.OP_Real, 0, iMem, 0, value,  P4Usage.P4_REAL);
                }
            }
#endif
            ///<summary>
            /// Generate an instruction that will put the integer describe by
            /// text z[0..n-1] into register iMem.
            ///
            /// Expr.u.zToken is always UTF8 and zero-terminated.
            ///
            ///</summary>
            ///<summary>
            /// Clear a cache entry.
            ///
            ///</summary>
            ///<summary>
            /// Record in the column cache that a particular column from a
            /// particular table is stored in a particular register.
            ///
            ///</summary>
            ///<summary>
            /// Indicate that registers between iReg..iReg+nReg-1 are being overwritten.
            /// Purge the range of registers from the column cache.
            ///
            ///</summary>
            ///<summary>
            /// Remember the current column cache context.  Any new entries added
            /// added to the column cache after this call are removed when the
            /// corresponding pop occurs.
            ///
            ///</summary>
            ///<summary>
            /// Remove from the column cache any entries that were added since the
            /// the previous N Push operations.  In other words, restore the cache
            /// to the state it was in N Pushes ago.
            ///
            ///</summary>
            ///<summary>
            /// When a cached column is reused, make sure that its register is
            /// no longer available as a temp register.  ticket #3879:  that same
            /// register might be in the cache in multiple places, so be sure to
            /// get them all.
            ///
            ///</summary>
            ///<summary>
            /// Generate code to extract the value of the iCol-th column of a table.
            ///
            ///</summary>
            ///<summary>
            /// Generate code that will extract the iColumn-th column from
            /// table pTab and store the column value in a register.  An effort
            /// is made to store the column value in register iReg, but this is
            /// not guaranteed.  The location of the column value is returned.
            ///
            /// There must be an open cursor to pTab in iTable when this routine
            /// is called.  If iColumn<0 then code is generated that extracts the rowid.
            ///
            ///</summary>
            ///<summary>
            /// Clear all column cache entries.
            ///
            ///</summary>
            ///<summary>
            /// Record the fact that an affinity change has occurred on iCount
            /// registers starting with iStart.
            ///
            ///</summary>
            ///<summary>
            /// Generate code to move content from registers iFrom...iFrom+nReg-1
            /// over to iTo..iTo+nReg-1. Keep the column cache up-to-date.
            ///
            ///</summary>
            ///<summary>
            /// Generate code to copy content from registers iFrom...iFrom+nReg-1
            /// over to iTo..iTo+nReg-1.
            ///
            ///</summary>
#if (SQLITE_DEBUG) || (SQLITE_COVERAGE_TEST)
																																																    /*
** Return true if any register in the range iFrom..iTo (inclusive)
** is used as part of the column cache.
**
** This routine is used within Debug.Assert() and sqliteinth.testcase() macros only
** and does not appear in a normal build.
*/
    static int usedAsColumnCache( Parse pParse, int iFrom, int iTo )
    {
      int i;
      yColCache p;
      for ( i = 0; i < SQLITE_N_COLCACHE; i++ )//p=pParse.aColCache... p++)
      {
        p = pParse.aColCache[i];
        int r = p.iReg;
        if ( r >= iFrom && r <= iTo )
          return 1;    /*NO_TEST*/
      }
      return 0;
    }
#else
#endif
            ///<summary>
            /// Generate code into the current Vdbe to evaluate the given
            /// expression.  Attempt to store the results in register "target".
            /// Return the register where results are stored.
            ///
            /// With this routine, there is no guarantee  that results will
            /// be stored in target.  The result might be stored in some other
            /// register if it is convenient to do so.  The calling function
            /// must check the return code and move the results to the desired
            /// register.
            ///
            ///</summary>
            ///<summary>
            /// Generate code to evaluate an expression and store the results
            /// into a register.  Return the register number where the results
            /// are stored.
            ///
            /// If the register is a temporary register that can be deallocated,
            /// then write its number into pReg.  If the result register is not
            /// a temporary, then set pReg to zero.
            ///
            ///</summary>
            ///<summary>
            /// Generate code that will evaluate expression pExpr and store the
            /// results in register target.  The results are guaranteed to appear
            /// in register target.
            ///
            ///</summary>
            ///<summary>
            /// Generate code that evalutes the given expression and puts the result
            /// in register target.
            ///
            /// Also make a copy of the expression results into another "cache" register
            /// and modify the expression so that the next time it is evaluated,
            /// the result is a copy of the cache register.
            ///
            /// This routine is used for expressions that are used multiple
            /// times.  They are evaluated once and the results of the expression
            /// are reused.
            ///
            ///</summary>
            ///<summary>
            /// If pExpr is a constant expression that is appropriate for
            /// factoring out of a loop, then evaluate the expression
            /// into a register and convert the expression into a Sqlite3.TK_REGISTER
            /// expression.
            ///
            ///</summary>
            public static WRC evalConstExpr(Walker pWalker, ref Expr pExpr)
            {
                Parse pParse = pWalker.pParse;
                switch (pExpr.Operator)
                {
                    case TokenType.TK_IN:
                    case TokenType.TK_REGISTER:
                        {
                            return WRC.WRC_Prune;
                        }
                    case TokenType.TK_FUNCTION:
                    case TokenType.TK_AGG_FUNCTION:
                    case TokenType.TK_CONST_FUNC:
                        {
                            ///
                            ///<summary>
                            ///The arguments to a function have a fixed destination.
                            ///Mark them this way to avoid generated unneeded  OpCode.OP_SCopy
                            ///instructions.
                            ///
                            ///</summary>
                            ExprList pList = pExpr.x.pList;
                            Debug.Assert(!pExpr.ExprHasProperty(ExprFlags.EP_xIsSelect));
                            if (pList != null)
                            {
                                int i = pList.nExpr;
                                ExprList_item pItem;
                                //= pList.a;
                                for (; i > 0; i--)
                                {
                                    //, pItem++){
                                    pItem = pList.a[pList.nExpr - i];
                                    if (Sqlite3.ALWAYS(pItem.pExpr != null))
                                        pItem.pExpr.Flags |= ExprFlags.EP_FixedDest;
                                }
                            }
                            break;
                        }
                }
                if (pExpr.isAppropriateForFactoring() != 0)
                {
                    int r1 = ++pParse.nMem;
                    int r2;
                    r2 = pParse.sqlite3ExprCodeTarget(pExpr, r1);
                    if (Sqlite3.NEVER(r1 != r2))
                        pParse.sqlite3ReleaseTempReg(r1);
                    pExpr.Operator2 = pExpr.Operator;
                    pExpr.Operator = TokenType.TK_REGISTER;
                    pExpr.iTable = r2;
                    return WRC.WRC_Prune;
                }
                return WRC.WRC_Continue;
            }
            ///<summary>
            /// Preevaluate constant subexpressions within pExpr and store the
            /// results in registers.  Modify pExpr so that the constant subexpresions
            /// are Sqlite3.TK_REGISTER opcodes that refer to the precomputed values.
            ///
            /// This routine is a no-op if the jump to the cookie-check code has
            /// already occur.  Since the cookie-check jump is generated prior to
            /// any other serious processing, this check ensures that there is no
            /// way to accidently bypass the constant initializations.
            ///
            /// This routine is also a no-op if the SQLITE_FactorOutConst optimization
            /// is disabled via the sqlite3_test_control(SQLITE_TESTCTRL_OPTIMIZATIONS)
            /// interface.  This allows test logic to verify that the same answer is
            /// obtained for queries regardless of whether or not constants are
            /// precomputed into registers or if they are inserted in-line.
            ///
            ///</summary>
            ///<summary>
            /// Generate code that pushes the value of every element of the given
            /// expression list into a sequence of registers beginning at target.
            ///
            /// Return the number of elements evaluated.
            ///
            ///</summary>
            ///<summary>
            /// Generate code for a BETWEEN operator.
            ///
            ///    x BETWEEN y AND z
            ///
            /// The above is equivalent to
            ///
            ///    x>=y AND x<=z
            ///
            /// Code it as such, taking care to do the common subexpression
            /// elementation of x.
            ///
            ///</summary>
            ///<summary>
            /// Generate code for a boolean expression such that a jump is made
            /// to the label "dest" if the expression is true but execution
            /// continues straight thru if the expression is false.
            ///
            /// If the expression evaluates to NULL (neither true nor false), then
            /// take the jump if the jumpIfNull flag is sqliteinth.SQLITE_JUMPIFNULL.
            ///
            /// This code depends on the fact that certain token values (ex: Sqlite3.TK_EQ)
            /// are the same as opcode values (ex:  OpCode.OP_Eq) that implement the corresponding
            /// operation.  Special comments in vdbe.c and the mkopcodeh.awk script in
            /// the make process cause these values to align.  Assert()s in the code
            /// below verify that the numbers are aligned correctly.
            ///
            ///</summary>
            ///<summary>
            /// Generate code for a boolean expression such that a jump is made
            /// to the label "dest" if the expression is false but execution
            /// continues straight thru if the expression is true.
            ///
            /// If the expression evaluates to NULL (neither true nor false) then
            /// jump if jumpIfNull is sqliteinth.SQLITE_JUMPIFNULL or fall through if jumpIfNull
            /// is 0.
            ///
            ///</summary>
            ///<summary>
            /// Do a deep comparison of two expression trees.  Return 0 if the two
            /// expressions are completely identical.  Return 1 if they differ only
            /// by a COLLATE operator at the top level.  Return 2 if there are differences
            /// other than the top-level COLLATE operator.
            ///
            /// Sometimes this routine will return 2 even if the two expressions
            /// really are equivalent.  If we cannot prove that the expressions are
            /// identical, we return 2 just to be safe.  So if this routine
            /// returns 2, then you do not really know for certain if the two
            /// expressions are the same.  But if you get a 0 or 1 return, then you
            /// can be sure the expressions are the same.  In the places where
            /// this routine is used, it does not hurt to get an extra 2 - that
            /// just might result in some slightly slower code.  But returning
            /// an incorrect 0 or 1 could lead to a malfunction.
            ///
            ///</summary>
            public static int sqlite3ExprCompare(Expr pA, Expr pB)
            {
                if (pA == null || pB == null)
                {
                    return pB == pA ? 0 : 2;
                }
                Debug.Assert(!pA.ExprHasAnyProperty(ExprFlags.EP_TokenOnly | ExprFlags.EP_Reduced));
                Debug.Assert(!pB.ExprHasAnyProperty(ExprFlags.EP_TokenOnly | ExprFlags.EP_Reduced));
                if (pA.ExprHasProperty(ExprFlags.EP_xIsSelect) || pB.ExprHasProperty(ExprFlags.EP_xIsSelect))
                {
                    return 2;
                }
                if ((pA.Flags & ExprFlags.EP_Distinct) != (pB.Flags & ExprFlags.EP_Distinct))
                    return 2;
                if (pA.Operator != pB.Operator)
                    return 2;
                if (exprc.sqlite3ExprCompare(pA.pLeft, pB.pLeft) != 0)
                    return 2;
                if (exprc.sqlite3ExprCompare(pA.pRight, pB.pRight) != 0)
                    return 2;
                if (exprc.sqlite3ExprListCompare(pA.x.pList, pB.x.pList) != 0)
                    return 2;
                if (pA.iTable != pB.iTable || pA.iColumn != pB.iColumn)
                    return 2;
                if (pA.ExprHasProperty(ExprFlags.EP_IntValue))
                {
                    if (!pB.ExprHasProperty(ExprFlags.EP_IntValue) || pA.u.iValue != pB.u.iValue)
                    {
                        return 2;
                    }
                }
                else
                    if (pA.Operator != TokenType.TK_COLUMN && pA.u.zToken != null)
                    {
                        if (pB.ExprHasProperty(ExprFlags.EP_IntValue) || Sqlite3.NEVER(pB.u.zToken == null))
                            return 2;
                        if (!pA.u.zToken.Equals(pB.u.zToken, StringComparison.InvariantCultureIgnoreCase))
                        {
                            return 2;
                        }
                    }
                if ((pA.Flags & ExprFlags.EP_ExpCollate) != (pB.Flags & ExprFlags.EP_ExpCollate))
                    return 1;
                if ((pA.Flags & ExprFlags.EP_ExpCollate) != 0 && pA.pColl != pB.pColl)
                    return 2;
                return 0;
            }
            ///<summary>
            /// Compare two ExprList objects.  Return 0 if they are identical and
            /// non-zero if they differ in any way.
            ///
            /// This routine might return non-zero for equivalent ExprLists.  The
            /// only consequence will be disabled optimizations.  But this routine
            /// must never return 0 if the two ExprList objects are different, or
            /// a malfunction will result.
            ///
            /// Two NULL pointers are considered to be the same.  But a NULL pointer
            /// always differs from a non-NULL pointer.
            ///
            ///</summary>
            public static int sqlite3ExprListCompare(ExprList pA, ExprList pB)
            {
                int i;
                if (pA == null && pB == null)
                    return 0;
                if (pA == null || pB == null)
                    return 1;
                if (pA.nExpr != pB.nExpr)
                    return 1;
                for (i = 0; i < pA.nExpr; i++)
                {
                    Expr pExprA = pA.a[i].pExpr;
                    Expr pExprB = pB.a[i].pExpr;
                    if (pA.a[i].sortOrder != pB.a[i].sortOrder)
                        return 1;
                    if (exprc.sqlite3ExprCompare(pExprA, pExprB) != 0)
                        return 1;
                }
                return 0;
            }
            ///<summary>
            /// Add a new element to the pAggInfo.aCol[] array.  Return the index of
            /// the new element.  Return a negative number if malloc fails.
            ///
            ///</summary>
            static int addAggInfoColumn(sqlite3 db, AggInfo pInfo)
            {
                int i = 0;
                pInfo.aCol = build.sqlite3ArrayAllocate(db, pInfo.aCol, -1,//sizeof(pInfo.aCol[0]),
                3, ref pInfo.nColumn, ref pInfo.nColumnAlloc, ref i);
                return i;
            }
            ///<summary>
            /// Add a new element to the pAggInfo.aFunc[] array.  Return the index of
            /// the new element.  Return a negative number if malloc fails.
            ///
            ///</summary>
            static int addAggInfoFunc(sqlite3 db, AggInfo pInfo)
            {
                int i = 0;
                pInfo.aFunc = build.sqlite3ArrayAllocate(db, pInfo.aFunc, -1,//sizeof(pInfo.aFunc[0]),
                3, ref pInfo.nFunc, ref pInfo.nFuncAlloc, ref i);
                return i;
            }
            ///<summary>
            /// This is the xExprCallback for a tree walker.  It is used to
            /// implement exprc.sqlite3ExprAnalyzeAggregates().  See exprc.sqlite3ExprAnalyzeAggregates
            /// for additional information.
            ///
            ///</summary>
            static WRC analyzeAggregate(Walker pWalker, ref Expr pExpr)
            {
                int i;
                NameContext pNC = pWalker.u.pNC;
                Parse pParse = pNC.pParse;
                SrcList pSrcList = pNC.pSrcList;
                AggInfo pAggInfo = pNC.pAggInfo;
                switch (pExpr.Operator)
                {
                    case TokenType.TK_AGG_COLUMN:
                    case TokenType.TK_COLUMN:
                        {
                            sqliteinth.testcase(pExpr.Operator == TokenType.TK_AGG_COLUMN);
                            sqliteinth.testcase(pExpr.Operator == TokenType.TK_COLUMN);
                            ///
                            ///<summary>
                            ///Check to see if the column is in one of the tables in the FROM
                            ///clause of the aggregate query 
                            ///</summary>
                            if (Sqlite3.ALWAYS(pSrcList != null))
                            {
                                SrcList_item pItem;
                                // = pSrcList.a;
                                for (i = 0; i < pSrcList.nSrc; i++)
                                {
                                    //, pItem++){
                                    pItem = pSrcList.a[i];
                                    AggInfo_col pCol;
                                    Debug.Assert(!pExpr.ExprHasAnyProperty(ExprFlags.EP_TokenOnly | ExprFlags.EP_Reduced));
                                    if (pExpr.iTable == pItem.iCursor)
                                    {
                                        ///
                                        ///<summary>
                                        ///If we reach this point, it means that pExpr refers to a table
                                        ///that is in the FROM clause of the aggregate query.
                                        ///
                                        ///Make an entry for the column in pAggInfo.aCol[] if there
                                        ///is not an entry there already.
                                        ///
                                        ///</summary>
                                        int k;
                                        //pCol = pAggInfo.aCol;
                                        for (k = 0; k < pAggInfo.nColumn; k++)
                                        {
                                            //, pCol++){
                                            pCol = pAggInfo.aCol[k];
                                            if (pCol.iTable == pExpr.iTable && pCol.iColumn == pExpr.iColumn)
                                            {
                                                break;
                                            }
                                        }
                                        if ((k >= pAggInfo.nColumn) && (k = addAggInfoColumn(pParse.db, pAggInfo)) >= 0)
                                        {
                                            pCol = pAggInfo.aCol[k];
                                            pCol.pTab = pExpr.pTab;
                                            pCol.iTable = pExpr.iTable;
                                            pCol.iColumn = pExpr.iColumn;
                                            pCol.iMem = ++pParse.nMem;
                                            pCol.iSorterColumn = -1;
                                            pCol.pExpr = pExpr;
                                            if (pAggInfo.pGroupBy != null)
                                            {
                                                int j, n;
                                                ExprList pGB = pAggInfo.pGroupBy;
                                                ExprList_item pTerm;
                                                // = pGB.a;
                                                n = pGB.nExpr;
                                                for (j = 0; j < n; j++)
                                                {
                                                    //, pTerm++){
                                                    pTerm = pGB.a[j];
                                                    Expr pE = pTerm.pExpr;
                                                    if (pE.Operator == TokenType.TK_COLUMN && pE.iTable == pExpr.iTable && pE.iColumn == pExpr.iColumn)
                                                    {
                                                        pCol.iSorterColumn = j;
                                                        break;
                                                    }
                                                }
                                            }
                                            if (pCol.iSorterColumn < 0)
                                            {
                                                pCol.iSorterColumn = pAggInfo.nSortingColumn++;
                                            }
                                        }
                                        ///
                                        ///<summary>
                                        ///There is now an entry for pExpr in pAggInfo.aCol[] (either
                                        ///because it was there before or because we just created it).
                                        ///Convert the pExpr to be a Sqlite3.TK_AGG_COLUMN referring to that
                                        ///pAggInfo.aCol[] entry.
                                        ///
                                        ///</summary>
                                        pExpr.ExprSetIrreducible();
                                        pExpr.pAggInfo = pAggInfo;
                                        pExpr.Operator = TokenType.TK_AGG_COLUMN;
                                        pExpr.iAgg = (short)k;
                                        break;
                                    }
                                    ///
                                    ///<summary>
                                    ///endif pExpr.iTable==pItem.iCursor 
                                    ///</summary>
                                }
                                ///
                                ///<summary>
                                ///end loop over pSrcList 
                                ///</summary>
                            }
                            return WRC.WRC_Prune;
                        }
                    case TokenType.TK_AGG_FUNCTION:
                        {
                            ///
                            ///<summary>
                            ///The pNC.nDepth==0 test causes aggregate functions in subqueries
                            ///to be ignored 
                            ///</summary>
                            if (pNC.nDepth == 0)
                            {
                                ///
                                ///<summary>
                                ///Check to see if pExpr is a duplicate of another aggregate
                                ///function that is already in the pAggInfo structure
                                ///
                                ///</summary>
                                AggInfo_func pItem;
                                // = pAggInfo.aFunc;
                                for (i = 0; i < pAggInfo.nFunc; i++)
                                {
                                    //, pItem++){
                                    pItem = pAggInfo.aFunc[i];
                                    if (exprc.sqlite3ExprCompare(pItem.pExpr, pExpr) == 0)
                                    {
                                        break;
                                    }
                                }
                                if (i >= pAggInfo.nFunc)
                                {
                                    ///
                                    ///<summary>
                                    ///pExpr is original.  Make a new entry in pAggInfo.aFunc[]
                                    ///
                                    ///</summary>
                                    SqliteEncoding enc = pParse.db.aDbStatic[0].pSchema.enc;
                                    // ENC(pParse.db);
                                    i = addAggInfoFunc(pParse.db, pAggInfo);
                                    if (i >= 0)
                                    {
                                        Debug.Assert(!pExpr.ExprHasProperty(ExprFlags.EP_xIsSelect));
                                        pItem = pAggInfo.aFunc[i];
                                        pItem.pExpr = pExpr;
                                        pItem.iMem = ++pParse.nMem;
                                        Debug.Assert(!pExpr.ExprHasProperty(ExprFlags.EP_IntValue));
                                        pItem.pFunc = Sqlite3.sqlite3FindFunction(pParse.db, pExpr.u.zToken, StringExtensions.sqlite3Strlen30(pExpr.u.zToken), pExpr.x.pList != null ? pExpr.x.pList.nExpr : 0, enc, 0);
                                        if ((pExpr.Flags & ExprFlags.EP_Distinct) != 0)
                                        {
                                            pItem.iDistinct = pParse.nTab++;
                                        }
                                        else
                                        {
                                            pItem.iDistinct = -1;
                                        }
                                    }
                                }
                                ///
                                ///<summary>
                                ///Make pExpr point to the appropriate pAggInfo.aFunc[] entry
                                ///
                                ///</summary>
                                Debug.Assert(!pExpr.ExprHasAnyProperty(ExprFlags.EP_TokenOnly | ExprFlags.EP_Reduced));
                                pExpr.ExprSetIrreducible();
                                pExpr.iAgg = (short)i;
                                pExpr.pAggInfo = pAggInfo;
                                return WRC.WRC_Prune;
                            }
                            break;
                        }
                }
                return WRC.WRC_Continue;
            }
            static WRC analyzeAggregatesInSelect(Walker pWalker, Select pSelect)
            {
                NameContext pNC = pWalker.u.pNC;
                if (pNC.nDepth == 0)
                {
                    pNC.nDepth++;
                    pWalker.sqlite3WalkSelect(pSelect);
                    pNC.nDepth--;
                    return WRC.WRC_Prune;
                }
                else
                {
                    return WRC.WRC_Continue;
                }
            }
            ///<summary>
            /// Analyze the given expression looking for aggregate functions and
            /// for variables that need to be added to the pParse.aAgg[] array.
            /// Make additional entries to the pParse.aAgg[] array as necessary.
            ///
            /// This routine should only be called after the expression has been
            /// analyzed by sqlite3ResolveExprNames().
            ///
            ///</summary>
            public static void sqlite3ExprAnalyzeAggregates(NameContext pNC, ref Expr pExpr)
            {
                Walker w = new Walker();
                w.xExprCallback = (dxExprCallback)analyzeAggregate;
                w.xSelectCallback = (dxSelectCallback)analyzeAggregatesInSelect;
                w.u.pNC = pNC;
                Debug.Assert(pNC.pSrcList != null);
                w.sqlite3WalkExpr(ref pExpr);
            }
            ///<summary>
            /// Call exprc.sqlite3ExprAnalyzeAggregates() for every expression in an
            /// expression list.  Return the number of errors.
            ///
            /// If an error is found, the analysis is cut short.
            ///
            ///</summary>
            public static void sqlite3ExprAnalyzeAggList(NameContext pNC, ExprList pList)
            {
                ExprList_item pItem;
                int i;
                if (pList != null)
                {
                    for (i = 0; i < pList.nExpr; i++)//, pItem++)
                    {
                        pItem = pList.a[i];
                        exprc.sqlite3ExprAnalyzeAggregates(pNC, ref pItem.pExpr);
                    }
                }
            }
            ///<summary>
            /// Allocate a single new register for use to hold some intermediate result.
            ///
            ///</summary>
            ///<summary>
            /// Deallocate a register, making available for reuse for some other
            /// purpose.
            ///
            /// If a register is currently being used by the column cache, then
            /// the dallocation is deferred until the column cache line that uses
            /// the register becomes stale.
            ///
            ///</summary>
            ///<summary>
            /// Allocate or deallocate a block of nReg consecutive registers
            ///
            ///</summary>
        }
}