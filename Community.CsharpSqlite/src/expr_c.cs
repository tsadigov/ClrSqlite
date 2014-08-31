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
namespace Community.CsharpSqlite {
	public partial class Sqlite3 {
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
		/// The collating sequence is marked as "explicit" using the EP_ExpCollate
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
		/// opcode (OP_Eq, OP_Ge etc.) used to compare pExpr1 and pExpr2.
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
		///<summary>
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
		/// then the EP_DblQuoted flag is set on the expression node.
		///
		/// Special case:  If op==TK_INTEGER and pToken points to a string that
		/// can be translated into a 32-bit integer, then the token is not
		/// stored in u.zToken.  Instead, the integer values is written
		/// into u.iValue and the EP_IntValue flag is set.  No extra storage
		/// is allocated to hold the integer text and the dequote flag is ignored.
		///</summary>
		static Expr sqlite3ExprAlloc(sqlite3 db,/* Handle for sqlite3DbMallocZero() (may be null) */int op,/* Expression opcode */Token pToken,/* Token argument.  Might be NULL */int dequote/* True to dequote */) {
			Expr pNew;
			int nExtra=0;
			int iValue=0;
			if(pToken!=null) {
				if(op!=TK_INTEGER||pToken.z==null||pToken.z.Length==0||Converter.sqlite3GetInt32(pToken.z.ToString(),ref iValue)==false) {
					nExtra=pToken.n+1;
					Debug.Assert(iValue>=0);
				}
			}
			pNew=new Expr();
			//sqlite3DbMallocZero(db, sizeof(Expr)+nExtra);
			if(pNew!=null) {
				pNew.op=(u8)op;
				pNew.iAgg=-1;
				if(pToken!=null) {
					if(nExtra==0) {
						pNew.flags|=EP_IntValue;
						pNew.u.iValue=iValue;
					}
					else {
						int c;
						//pNew.u.zToken = (char)&pNew[1];
						if(pToken.n>0)
							pNew.u.zToken=pToken.z.Substring(0,pToken.n);
						//memcpy(pNew.u.zToken, pToken.z, pToken.n);
						else
							if(pToken.n==0&&pToken.z=="")
								pNew.u.zToken="";
						//pNew.u.zToken[pToken.n] = 0;
						if(dequote!=0&&nExtra>=3&&((c=pToken.z[0])=='\''||c=='"'||c=='['||c=='`')) {
							#if DEBUG_CLASS_EXPR || DEBUG_CLASS_ALL
																																																																																																																																												StringExtensions.sqlite3Dequote(ref pNew.u._zToken);
#else
							StringExtensions.sqlite3Dequote(ref pNew.u.zToken);
							#endif
							if(c=='"')
								pNew.flags|=EP_DblQuoted;
						}
					}
				}
				#if SQLITE_MAX_EXPR_DEPTH
				pNew.nHeight=1;
				#endif
			}
			return pNew;
		}
		///<summary>
		/// Allocate a new expression node from a zero-terminated token that has
		/// already been dequoted.
		///
		///</summary>
		static Expr sqlite3Expr(sqlite3 db,/* Handle for sqlite3DbMallocZero() (may be null) */int op,/* Expression opcode */string zToken/* Token argument.  Might be NULL */) {
			Token x=new Token();
			x.z=zToken;
			x.n=!String.IsNullOrEmpty(zToken)?StringExtensions.sqlite3Strlen30(zToken):0;
			return sqlite3ExprAlloc(db,op,x,0);
		}
		/*
    ** Attach subtrees pLeft and pRight to the Expr node pRoot.
    **
    ** If pRoot==NULL that means that a memory allocation error has occurred.
    ** In that case, delete the subtrees pLeft and pRight.
    */static void sqlite3ExprAttachSubtrees(sqlite3 db,Expr pRoot,Expr pLeft,Expr pRight) {
			if(pRoot==null) {
				//Debug.Assert( db.mallocFailed != 0 );
				sqlite3ExprDelete(db,ref pLeft);
				sqlite3ExprDelete(db,ref pRight);
			}
			else {
				if(pRight!=null) {
					pRoot.pRight=pRight;
					if((pRight.flags&EP_ExpCollate)!=0) {
						pRoot.flags|=EP_ExpCollate;
						pRoot.pColl=pRight.pColl;
					}
				}
				if(pLeft!=null) {
					pRoot.pLeft=pLeft;
					if((pLeft.flags&EP_ExpCollate)!=0) {
						pRoot.flags|=EP_ExpCollate;
						pRoot.pColl=pLeft.pColl;
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
		/*
    ** Join two expressions using an AND operator.  If either expression is
    ** NULL, then just return the other expression.
    */static Expr sqlite3ExprAnd(sqlite3 db,Expr pLeft,Expr pRight) {
			if(pLeft==null) {
				return pRight;
			}
			else
				if(pRight==null) {
					return pLeft;
				}
				else {
					Expr pNew=sqlite3ExprAlloc(db,TK_AND,null,0);
					sqlite3ExprAttachSubtrees(db,pNew,pLeft,pRight);
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
		static void sqlite3ExprDelete(sqlite3 db,ref Expr p) {
			if(p==null)
				return;
			/* Sanity check: Assert that the IntValue is non-negative if it exists */Debug.Assert(!ExprHasProperty(p,EP_IntValue)||p.u.iValue>=0);
			if(!ExprHasAnyProperty(p,EP_TokenOnly)) {
				sqlite3ExprDelete(db,ref p.pLeft);
				sqlite3ExprDelete(db,ref p.pRight);
				if(!ExprHasProperty(p,EP_Reduced)&&(p.flags2&EP2_MallocedToken)!=0) {
					#if DEBUG_CLASS_EXPR || DEBUG_CLASS_ALL
																																																																																																				sqlite3DbFree( db, ref p.u._zToken );
#else
					sqlite3DbFree(db,ref p.u.zToken);
					#endif
				}
				if(ExprHasProperty(p,EP_xIsSelect)) {
					sqlite3SelectDelete(db,ref p.x.pSelect);
				}
				else {
					sqlite3ExprListDelete(db,ref p.x.pList);
				}
			}
			if(!ExprHasProperty(p,EP_Static)) {
				sqlite3DbFree(db,ref p);
			}
		}
		///<summary>
		/// Return the number of bytes allocated for the expression structure
		/// passed as the first argument. This is always one of EXPR_FULLSIZE,
		/// EXPR_REDUCEDSIZE or EXPR_TOKENONLYSIZE.
		///
		///</summary>
		static int exprStructSize(Expr p) {
			if(ExprHasProperty(p,EP_TokenOnly))
				return EXPR_TOKENONLYSIZE;
			if(ExprHasProperty(p,EP_Reduced))
				return EXPR_REDUCEDSIZE;
			return EXPR_FULLSIZE;
		}
		///<summary>
		/// This function is similar to sqlite3ExprDup(), except that if pzBuffer
		/// is not NULL then *pzBuffer is assumed to point to a buffer large enough
		/// to store the copy of expression p, the copies of p->u.zToken
		/// (if applicable), and the copies of the p->pLeft and p->pRight expressions,
		/// if any. Before returning, *pzBuffer is set to the first byte passed the
		/// portion of the buffer copied into by this function.
		///
		///</summary>
		static Expr exprDup(sqlite3 db,Expr p,int flags,ref Expr pzBuffer) {
			Expr pNew=null;
			/* Value to return */if(p!=null) {
				bool isReduced=(flags&EXPRDUP_REDUCE)!=0;
				Expr zAlloc=new Expr();
				u32 staticFlag=0;
				Debug.Assert(pzBuffer==null||isReduced);
				/* Figure out where to write the new Expr structure. *///if ( pzBuffer !=null)
				//{
				//  zAlloc = pzBuffer;
				//  staticFlag = EP_Static;
				//}
				//else
				//{
				///Expr  zAlloc = new Expr();//sqlite3DbMallocRaw( db, dupedExprSize( p, flags ) );
				//}
				// (Expr)zAlloc;
				//if ( pNew != null )
				{
					/* Set nNewSize to the size allocated for the structure pointed to
          ** by pNew. This is either EXPR_FULLSIZE, EXPR_REDUCEDSIZE or
          ** EXPR_TOKENONLYSIZE. nToken is set to the number of bytes consumed
          ** by the copy of the p->u.zToken string (if any).
          */int nStructSize=p.dupedExprStructSize(flags);
					int nNewSize=nStructSize&0xfff;
					int nToken;
					if(!ExprHasProperty(p,EP_IntValue)&&!String.IsNullOrEmpty(p.u.zToken)) {
						nToken=StringExtensions.sqlite3Strlen30(p.u.zToken);
					}
					else {
						nToken=0;
					}
					if(isReduced) {
						Debug.Assert(!ExprHasProperty(p,EP_Reduced));
						pNew=p.Copy(EXPR_TOKENONLYSIZE);
						//memcpy( zAlloc, p, nNewSize );
					}
					else {
						int nSize=exprStructSize(p);
						//memcpy( zAlloc, p, nSize );
						pNew=p.Copy();
						//memset( &zAlloc[nSize], 0, EXPR_FULLSIZE - nSize );
					}
					/* Set the EP_Reduced, EP_TokenOnly, and EP_Static flags appropriately. */unchecked {
						pNew.flags&=(ushort)(~(EP_Reduced|EP_TokenOnly|EP_Static));
					}
					pNew.flags|=(ushort)(nStructSize&(EP_Reduced|EP_TokenOnly));
					pNew.flags|=(ushort)staticFlag;
					/* Copy the p->u.zToken string, if any. */if(nToken!=0) {
						string zToken;
						// = pNew.u.zToken = (char)&zAlloc[nNewSize];
						zToken=p.u.zToken.Substring(0,nToken);
						// memcpy( zToken, p.u.zToken, nToken );
					}
					if(0==((p.flags|pNew.flags)&EP_TokenOnly)) {
						/* Fill in the pNew.x.pSelect or pNew.x.pList member. */if(ExprHasProperty(p,EP_xIsSelect)) {
							pNew.x.pSelect=sqlite3SelectDup(db,p.x.pSelect,isReduced?1:0);
						}
						else {
							pNew.x.pList=sqlite3ExprListDup(db,p.x.pList,isReduced?1:0);
						}
					}
					/* Fill in pNew.pLeft and pNew.pRight. */if(ExprHasAnyProperty(pNew,EP_Reduced|EP_TokenOnly)) {
						//zAlloc += dupedExprNodeSize( p, flags );
						if(ExprHasProperty(pNew,EP_Reduced)) {
							pNew.pLeft=exprDup(db,p.pLeft,EXPRDUP_REDUCE,ref pzBuffer);
							pNew.pRight=exprDup(db,p.pRight,EXPRDUP_REDUCE,ref pzBuffer);
						}
						//if ( pzBuffer != null )
						//{
						//  pzBuffer = zAlloc;
						//}
					}
					else {
						pNew.flags2=0;
						if(!ExprHasAnyProperty(p,EP_TokenOnly)) {
							pNew.pLeft=sqlite3ExprDup(db,p.pLeft,0);
							pNew.pRight=sqlite3ExprDup(db,p.pRight,0);
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
		/// The expression list, ID, and source lists return by sqlite3ExprListDup(),
		/// sqlite3IdListDup(), and sqlite3SrcListDup() can not be further expanded
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
		static Expr sqlite3ExprDup(sqlite3 db,Expr p,int flags) {
			Expr ExprDummy=null;
			return exprDup(db,p,flags,ref ExprDummy);
		}
		static ExprList sqlite3ExprListDup(sqlite3 db,ExprList p,int flags) {
			ExprList pNew;
			ExprList_item pItem;
			ExprList_item pOldItem;
			int i;
			if(p==null)
				return null;
			pNew=new ExprList();
			//sqlite3DbMallocRaw(db, sizeof(*pNew) );
			//if ( pNew == null ) return null;
			pNew.iECursor=0;
			pNew.nExpr=pNew.nAlloc=p.nExpr;
			pNew.a=new ExprList_item[p.nExpr];
			//sqlite3DbMallocRaw(db,  p.nExpr*sizeof(p.a[0]) );
			//if( pItem==null ){
			//  sqlite3DbFree(db,ref pNew);
			//  return null;
			//}
			//pOldItem = p.a;
			for(i=0;i<p.nExpr;i++) {
				//pItem++, pOldItem++){
				pItem=pNew.a[i]=new ExprList_item();
				pOldItem=p.a[i];
				Expr pOldExpr=pOldItem.pExpr;
				pItem.pExpr=sqlite3ExprDup(db,pOldExpr,flags);
				pItem.zName=pOldItem.zName;
				// sqlite3DbStrDup(db, pOldItem.zName);
				pItem.zSpan=pOldItem.zSpan;
				// sqlite3DbStrDup( db, pOldItem.zSpan );
				pItem.sortOrder=pOldItem.sortOrder;
				pItem.done=0;
				pItem.iCol=pOldItem.iCol;
				pItem.iAlias=pOldItem.iAlias;
			}
			return pNew;
		}
		///<summary>
		/// If cursors, triggers, views and subqueries are all omitted from
		/// the build, then none of the following routines, except for
		/// sqlite3SelectDup(), can be called. sqlite3SelectDup() is sometimes
		/// called with a NULL argument.
		///
		///</summary>
		#if !SQLITE_OMIT_VIEW || !SQLITE_OMIT_TRIGGER  || !SQLITE_OMIT_SUBQUERY
		static SrcList sqlite3SrcListDup(sqlite3 db,SrcList p,int flags) {
			SrcList pNew;
			int i;
			int nByte;
			if(p==null)
				return null;
			//nByte = sizeof(*p) + (p.nSrc>0 ? sizeof(p.a[0]) * (p.nSrc-1) : 0);
			pNew=new SrcList();
			//sqlite3DbMallocRaw(db, nByte );
			if(p.nSrc>0)
				pNew.a=new SrcList_item[p.nSrc];
			if(pNew==null)
				return null;
			pNew.nSrc=pNew.nAlloc=p.nSrc;
			for(i=0;i<p.nSrc;i++) {
				pNew.a[i]=new SrcList_item();
				SrcList_item pNewItem=pNew.a[i];
				SrcList_item pOldItem=p.a[i];
				Table pTab;
				pNewItem.zDatabase=pOldItem.zDatabase;
				// sqlite3DbStrDup(db, pOldItem.zDatabase);
				pNewItem.zName=pOldItem.zName;
				// sqlite3DbStrDup(db, pOldItem.zName);
				pNewItem.zAlias=pOldItem.zAlias;
				// sqlite3DbStrDup(db, pOldItem.zAlias);
				pNewItem.jointype=pOldItem.jointype;
				pNewItem.iCursor=pOldItem.iCursor;
				pNewItem.isPopulated=pOldItem.isPopulated;
				pNewItem.zIndex=pOldItem.zIndex;
				// sqlite3DbStrDup( db, pOldItem.zIndex );
				pNewItem.notIndexed=pOldItem.notIndexed;
				pNewItem.pIndex=pOldItem.pIndex;
				pTab=pNewItem.pTab=pOldItem.pTab;
				if(pTab!=null) {
					pTab.nRef++;
				}
				pNewItem.pSelect=sqlite3SelectDup(db,pOldItem.pSelect,flags);
				pNewItem.pOn=sqlite3ExprDup(db,pOldItem.pOn,flags);
				pNewItem.pUsing=sqlite3IdListDup(db,pOldItem.pUsing);
				pNewItem.colUsed=pOldItem.colUsed;
			}
			return pNew;
		}
		static IdList sqlite3IdListDup(sqlite3 db,IdList p) {
			IdList pNew;
			int i;
			if(p==null)
				return null;
			pNew=new IdList();
			//sqlite3DbMallocRaw(db, sizeof(*pNew) );
			if(pNew==null)
				return null;
			pNew.nId=pNew.nAlloc=p.nId;
			pNew.a=new IdList_item[p.nId];
			//sqlite3DbMallocRaw(db, p.nId*sizeof(p.a[0]) );
			if(pNew.a==null) {
				sqlite3DbFree(db,ref pNew);
				return null;
			}
			for(i=0;i<p.nId;i++) {
				pNew.a[i]=new IdList_item();
				IdList_item pNewItem=pNew.a[i];
				IdList_item pOldItem=p.a[i];
				pNewItem.zName=pOldItem.zName;
				// sqlite3DbStrDup(db, pOldItem.zName);
				pNewItem.idx=pOldItem.idx;
			}
			return pNew;
		}
		static Select sqlite3SelectDup(sqlite3 db,Select p,int flags) {
			Select pNew;
			if(p==null)
				return null;
			pNew=new Select();
			//sqlite3DbMallocRaw(db, sizeof(*p) );
			//if ( pNew == null ) return null;
			pNew.pEList=sqlite3ExprListDup(db,p.pEList,flags);
			pNew.pSrc=sqlite3SrcListDup(db,p.pSrc,flags);
			pNew.pWhere=sqlite3ExprDup(db,p.pWhere,flags);
			pNew.pGroupBy=sqlite3ExprListDup(db,p.pGroupBy,flags);
			pNew.pHaving=sqlite3ExprDup(db,p.pHaving,flags);
			pNew.pOrderBy=sqlite3ExprListDup(db,p.pOrderBy,flags);
			pNew.tk_op=p.tk_op;
			pNew.pPrior=sqlite3SelectDup(db,p.pPrior,flags);
			pNew.pLimit=sqlite3ExprDup(db,p.pLimit,flags);
			pNew.pOffset=sqlite3ExprDup(db,p.pOffset,flags);
			pNew.iLimit=0;
			pNew.iOffset=0;
			pNew.selFlags=(p.selFlags&~SelectFlags.UsesEphemeral);
			pNew.pRightmost=null;
			pNew.addrOpenEphm[0]=-1;
			pNew.addrOpenEphm[1]=-1;
			pNew.addrOpenEphm[2]=-1;
			return pNew;
		}
		#else
																																								Select sqlite3SelectDup(sqlite3 db, Select p, int flags){
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
		static void sqlite3ExprListDelete(sqlite3 db,ref ExprList pList) {
			int i;
			ExprList_item pItem;
			if(pList==null)
				return;
			Debug.Assert(pList.a!=null||(pList.nExpr==0&&pList.nAlloc==0));
			Debug.Assert(pList.nExpr<=pList.nAlloc);
			for(i=0;i<pList.nExpr;i++) {
				if((pItem=pList.a[i])!=null) {
					sqlite3ExprDelete(db,ref pItem.pExpr);
					sqlite3DbFree(db,ref pItem.zName);
					sqlite3DbFree(db,ref pItem.zSpan);
				}
			}
			sqlite3DbFree(db,ref pList.a);
			sqlite3DbFree(db,ref pList);
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
		static int exprNodeIsConstant(Walker pWalker,ref Expr pExpr) {
			/* If pWalker.u.i is 3 then any term of the expression that comes from
      ** the ON or USING clauses of a join disqualifies the expression
      ** from being considered constant. */if(pWalker.u.i==3&&ExprHasAnyProperty(pExpr,EP_FromJoin)) {
				pWalker.u.i=0;
				return WRC_Abort;
			}
			switch(pExpr.op) {
			/* Consider functions to be constant if all their arguments are constant
        ** and pWalker.u.i==2 */case TK_FUNCTION:
			if((pWalker.u.i)==2)
				return 0;
			goto case TK_ID;
			/* Fall through */case TK_ID:
			case TK_COLUMN:
			case TK_AGG_FUNCTION:
			case TK_AGG_COLUMN:
			testcase(pExpr.op==TK_ID);
			testcase(pExpr.op==TK_COLUMN);
			testcase(pExpr.op==TK_AGG_FUNCTION);
			testcase(pExpr.op==TK_AGG_COLUMN);
			pWalker.u.i=0;
			return WRC_Abort;
			default:
			testcase(pExpr.op==TK_SELECT);
			/* selectNodeIsConstant will disallow */testcase(pExpr.op==TK_EXISTS);
			/* selectNodeIsConstant will disallow */return WRC_Continue;
			}
		}
		static int selectNodeIsConstant(Walker pWalker,Select NotUsed) {
			UNUSED_PARAMETER(NotUsed);
			pWalker.u.i=0;
			return WRC_Abort;
		}
		///<summary>
		/// Generate an OP_IsNull instruction that tests register iReg and jumps
		/// to location iDest if the value in iReg is NULL.  The value in iReg
		/// was computed by pExpr.  If we can look at pExpr at compile-time and
		/// determine that it can never generate a NULL, then the OP_IsNull operation
		/// can be omitted.
		///
		///</summary>
		static void sqlite3ExprCodeIsNullJump(Vdbe v,/* The VDBE under construction */Expr pExpr,/* Only generate OP_IsNull if this expr can be NULL */int iReg,/* Test the value in this register for NULL */int iDest/* Jump here if the value is null */) {
			if(pExpr.sqlite3ExprCanBeNull()!=0) {
				v.sqlite3VdbeAddOp2(OP_IsNull,iReg,iDest);
			}
		}
		///<summary>
		/// Return TRUE if the given expression is a constant which would be
		/// unchanged by OP_Affinity with the affinity given in the second
		/// argument.
		///
		/// This routine is used to determine if the OP_Affinity operation
		/// can be omitted.  When in doubt return FALSE.  A false negative
		/// is harmless.  A false positive, however, can result in the wrong
		/// answer.
		///
		///</summary>
		static int sqlite3ExprNeedsNoAffinityChange(Expr p,char aff) {
			u8 op;
			if(aff==SQLITE_AFF_NONE)
				return 1;
			while(p.op==TK_UPLUS||p.op==TK_UMINUS) {
				p=p.pLeft;
			}
			op=p.op;
			if(op==TK_REGISTER)
				op=p.op2;
			switch(op) {
			case TK_INTEGER: {
				return (aff==SQLITE_AFF_INTEGER||aff==SQLITE_AFF_NUMERIC)?1:0;
			}
			case TK_FLOAT: {
				return (aff==SQLITE_AFF_REAL||aff==SQLITE_AFF_NUMERIC)?1:0;
			}
			case TK_STRING: {
				return (aff==SQLITE_AFF_TEXT)?1:0;
			}
			case TK_BLOB: {
				return 1;
			}
			case TK_COLUMN: {
				Debug.Assert(p.iTable>=0);
				/* p cannot be part of a CHECK constraint */return (p.iColumn<0&&(aff==SQLITE_AFF_INTEGER||aff==SQLITE_AFF_NUMERIC))?1:0;
			}
			default: {
				return 0;
			}
			}
		}
		/*
    ** Return TRUE if the given string is a row-id column name.
    */static bool sqlite3IsRowid(string z) {
			if(z.Equals("_ROWID_",StringComparison.InvariantCultureIgnoreCase))
				return true;
			if(z.Equals("ROWID",StringComparison.InvariantCultureIgnoreCase))
				return true;
			if(z.Equals("OID",StringComparison.InvariantCultureIgnoreCase))
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
		static int isCandidateForInOpt(Select p) {
			SrcList pSrc;
			ExprList pEList;
			Table pTab;
			if(p==null)
				return 0;
			/* right-hand side of IN is SELECT */if(p.pPrior!=null)
				return 0;
			/* Not a compound SELECT */if((p.selFlags&(SelectFlags.Distinct|SelectFlags.Aggregate))!=0) {
				testcase((p.selFlags&(SelectFlags.Distinct|SelectFlags.Aggregate))==SelectFlags.Distinct);
				testcase((p.selFlags&(SelectFlags.Distinct|SelectFlags.Aggregate))==SelectFlags.Aggregate);
				return 0;
				/* No DISTINCT keyword and no aggregate functions */}
			Debug.Assert(p.pGroupBy==null);
			/* Has no GROUP BY clause */if(p.pLimit!=null)
				return 0;
			/* Has no LIMIT clause */Debug.Assert(p.pOffset==null);
			/* No LIMIT means no OFFSET */if(p.pWhere!=null)
				return 0;
			/* Has no WHERE clause */pSrc=p.pSrc;
			Debug.Assert(pSrc!=null);
			if(pSrc.nSrc!=1)
				return 0;
			/* Single term in FROM clause */if(pSrc.a[0].pSelect!=null)
				return 0;
			/* FROM is not a subquery or view */pTab=pSrc.a[0].pTab;
			if(NEVER(pTab==null))
				return 0;
			Debug.Assert(pTab.pSelect==null);
			/* FROM clause is not a view */if(IsVirtual(pTab))
				return 0;
			/* FROM clause not a virtual table */pEList=p.pEList;
			if(pEList.nExpr!=1)
				return 0;
			/* One column in the result set */if(pEList.a[0].pExpr.op!=TK_COLUMN)
				return 0;
			/* Result is a column */return 1;
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
		///   IN_INDEX_EPH -   The cursor was opened on a specially created and
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
		static int sqlite3FindInIndex(Parse pParse,Expr pX,ref int prNotFound) {
			Select p;
			/* SELECT to the right of IN operator */int eType=0;
			/* Type of RHS table. IN_INDEX_* */int iTab=pParse.nTab++;
			/* Cursor of the RHS table */bool mustBeUnique=(prNotFound!=0);
			/* True if RHS must be unique */Debug.Assert(pX.op==TK_IN);
			/* Check to see if an existing table or index can be used to
      ** satisfy the query.  This is preferable to generating a new
      ** ephemeral table.
      */p=(ExprHasProperty(pX,EP_xIsSelect)?pX.x.pSelect:null);
			if(ALWAYS(pParse.nErr==0)&&isCandidateForInOpt(p)!=0) {
				sqlite3 db=pParse.db;
				/* Database connection */Expr pExpr=p.pEList.a[0].pExpr;
				/* Expression <column> */int iCol=pExpr.iColumn;
				/* Index of column <column> */Vdbe v=sqlite3GetVdbe(pParse);
				/* Virtual machine being coded */Table pTab=p.pSrc.a[0].pTab;
				/* Table <table>. */int iDb;
				/* Database idx for pTab *//* Code an OP_VerifyCookie and OP_TableLock for <table>. */iDb=sqlite3SchemaToIndex(db,pTab.pSchema);
				sqlite3CodeVerifySchema(pParse,iDb);
				sqlite3TableLock(pParse,iDb,pTab.tnum,0,pTab.zName);
				/* This function is only called from two places. In both cases the vdbe
        ** has already been allocated. So assume sqlite3GetVdbe() is always
        ** successful here.
        */Debug.Assert(v!=null);
				if(iCol<0) {
					int iMem=++pParse.nMem;
					int iAddr;
					iAddr=v.sqlite3VdbeAddOp1(OP_If,iMem);
					v.sqlite3VdbeAddOp2(OP_Integer,1,iMem);
					pParse.sqlite3OpenTable(iTab,iDb,pTab,OP_OpenRead);
					eType=IN_INDEX_ROWID;
					v.sqlite3VdbeJumpHere(iAddr);
				}
				else {
					Index pIdx;
					/* Iterator variable *//* The collation sequence used by the comparison. If an index is to
          ** be used in place of a temp.table, it must be ordered according
          ** to this collation sequence. */CollSeq pReq=pParse.sqlite3BinaryCompareCollSeq(pX.pLeft,pExpr);
					/* Check that the affinity that will be used to perform the
          ** comparison is the same as the affinity of the column. If
          ** it is not, it is not possible to use any index.
          */char aff=pX.comparisonAffinity();
					bool affinity_ok=(pTab.aCol[iCol].affinity==aff||aff==SQLITE_AFF_NONE);
					for(pIdx=pTab.pIndex;pIdx!=null&&eType==0&&affinity_ok;pIdx=pIdx.pNext) {
						if((pIdx.aiColumn[0]==iCol)&&(sqlite3FindCollSeq(db,ENC(db),pIdx.azColl[0],0)==pReq)&&(mustBeUnique==false||(pIdx.nColumn==1&&pIdx.onError!=OE_None))) {
							int iMem=++pParse.nMem;
							int iAddr;
							KeyInfo pKey;
							pKey=sqlite3IndexKeyinfo(pParse,pIdx);
							iAddr=v.sqlite3VdbeAddOp1(OP_If,iMem);
							v.sqlite3VdbeAddOp2(OP_Integer,1,iMem);
							v.sqlite3VdbeAddOp4(OP_OpenRead,iTab,pIdx.tnum,iDb,pKey,P4_KEYINFO_HANDOFF);
							#if SQLITE_DEBUG
																																																																																																																																												              VdbeComment( v, "%s", pIdx.zName );
#endif
							eType=IN_INDEX_INDEX;
							v.sqlite3VdbeJumpHere(iAddr);
							if(//prNotFound != null &&         -- always exists under C#
							pTab.aCol[iCol].notNull==0) {
								prNotFound=++pParse.nMem;
							}
						}
					}
				}
			}
			if(eType==0) {
				/* Could not found an existing table or index to use as the RHS b-tree.
        ** We will have to generate an ephemeral table to do the job.
        */double savedNQueryLoop=pParse.nQueryLoop;
				int rMayHaveNull=0;
				eType=IN_INDEX_EPH;
				if(prNotFound!=-1)// Klude to show prNotFound not available
				 {
					prNotFound=rMayHaveNull=++pParse.nMem;
				}
				else {
					testcase(pParse.nQueryLoop>(double)1);
					pParse.nQueryLoop=(double)1;
					if(pX.pLeft.iColumn<0&&!ExprHasAnyProperty(pX,EP_xIsSelect)) {
						eType=IN_INDEX_ROWID;
					}
				}
				sqlite3CodeSubselect(pParse,pX,rMayHaveNull,eType==IN_INDEX_ROWID);
				pParse.nQueryLoop=savedNQueryLoop;
			}
			else {
				pX.iTable=iTab;
			}
			return eType;
		}
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
		static int sqlite3CodeSubselect(Parse pParse,/* Parsing context */Expr pExpr,/* The IN, SELECT, or EXISTS operator */int rMayHaveNull,/* Register that records whether NULLs exist in RHS */bool isRowid/* If true, LHS of IN operator is a rowid */) {
			int testAddr=0;
			/* One-time test address */int rReg=0;
			/* Register storing resulting */Vdbe v=sqlite3GetVdbe(pParse);
			if(NEVER(v==null))
				return 0;
			pParse.sqlite3ExprCachePush();
			/* This code must be run in its entirety every time it is encountered
      ** if any of the following is true:
      **
      **    *  The right-hand side is a correlated subquery
      **    *  The right-hand side is an expression list containing variables
      **    *  We are inside a trigger
      **
      ** If all of the above are false, then we can run this code just once
      ** save the results, and reuse the same result on subsequent invocations.
      */if(!ExprHasAnyProperty(pExpr,EP_VarSelect)&&null==pParse.pTriggerTab) {
				int mem=++pParse.nMem;
				v.sqlite3VdbeAddOp1(OP_If,mem);
				testAddr=v.sqlite3VdbeAddOp2(OP_Integer,1,mem);
				Debug.Assert(testAddr>0/* || pParse.db.mallocFailed != 0 */);
			}
			#if !SQLITE_OMIT_EXPLAIN
			if(pParse.explain==2) {
				string zMsg=sqlite3MPrintf(pParse.db,"EXECUTE %s%s SUBQUERY %d",testAddr!=0?"":"CORRELATED ",pExpr.op==TK_IN?"LIST":"SCALAR",pParse.iNextSelectId);
				v.sqlite3VdbeAddOp4(OP_Explain,pParse.iSelectId,0,0,zMsg,P4_DYNAMIC);
			}
			#endif
			switch(pExpr.op) {
			case TK_IN: {
				char affinity;
				/* Affinity of the LHS of the IN */KeyInfo keyInfo;
				/* Keyinfo for the generated table */int addr;
				/* Address of OP_OpenEphemeral instruction */Expr pLeft=pExpr.pLeft;
				/* the LHS of the IN operator */if(rMayHaveNull!=0) {
					v.sqlite3VdbeAddOp2(OP_Null,0,rMayHaveNull);
				}
				affinity=pLeft.sqlite3ExprAffinity();
				/* Whether this is an 'x IN(SELECT...)' or an 'x IN(<exprlist>)'
            ** expression it is handled the same way. An ephemeral table is
            ** filled with single-field index keys representing the results
            ** from the SELECT or the <exprlist>.
            **
            ** If the 'x' expression is a column value, or the SELECT...
            ** statement returns a column value, then the affinity of that
            ** column is used to build the index keys. If both 'x' and the
            ** SELECT... statement are columns, then numeric affinity is used
            ** if either column has NUMERIC or INTEGER affinity. If neither
            ** 'x' nor the SELECT... statement are columns, then numeric affinity
            ** is used.
            */pExpr.iTable=pParse.nTab++;
				addr=v.sqlite3VdbeAddOp2(OP_OpenEphemeral,(int)pExpr.iTable,!isRowid);
				if(rMayHaveNull==0)
					v.sqlite3VdbeChangeP5(BTREE_UNORDERED);
				keyInfo=new KeyInfo();
				// memset( &keyInfo, 0, sizeof(keyInfo ));
				keyInfo.nField=1;
				if(ExprHasProperty(pExpr,EP_xIsSelect)) {
					/* Case 1:     expr IN (SELECT ...)
              **
              ** Generate code to write the results of the select into the temporary
              ** table allocated and opened above.
              */SelectDest dest=new SelectDest();
					ExprList pEList;
					Debug.Assert(!isRowid);
					sqlite3SelectDestInit(dest,SelectResultType.Set,pExpr.iTable);
					dest.affinity=(char)affinity;
					Debug.Assert((pExpr.iTable&0x0000FFFF)==pExpr.iTable);
					pExpr.x.pSelect.iLimit=0;
					if(sqlite3Select(pParse,pExpr.x.pSelect,ref dest)!=0) {
						return 0;
					}
					pEList=pExpr.x.pSelect.pEList;
					if(ALWAYS(pEList!=null)&&pEList.nExpr>0) {
						keyInfo.aColl[0]=pParse.sqlite3BinaryCompareCollSeq(pExpr.pLeft,pEList.a[0].pExpr);
					}
				}
				else
					if(ALWAYS(pExpr.x.pList!=null)) {
						/* Case 2:     expr IN (exprlist)
              **
              ** For each expression, build an index key from the evaluation and
              ** store it in the temporary table. If <expr> is a column, then use
              ** that columns affinity when building index keys. If <expr> is not
              ** a column, use numeric affinity.
              */int i;
						ExprList pList=pExpr.x.pList;
						ExprList_item pItem;
						int r1,r2,r3;
						if(affinity=='\0') {
							affinity=SQLITE_AFF_NONE;
						}
						keyInfo.aColl[0]=pParse.sqlite3ExprCollSeq(pExpr.pLeft);
						/* Loop through each expression in <exprlist>. */r1=pParse.sqlite3GetTempReg();
						r2=pParse.sqlite3GetTempReg();
						v.sqlite3VdbeAddOp2(OP_Null,0,r2);
						for(i=0;i<pList.nExpr;i++) {
							//, pItem++){
							pItem=pList.a[i];
							Expr pE2=pItem.pExpr;
							int iValToIns=0;
							/* If the expression is not constant then we will need to
                ** disable the test that was generated above that makes sure
                ** this code only executes once.  Because for a non-constant
                ** expression we need to rerun this code each time.
                */if(testAddr!=0&&pE2.sqlite3ExprIsConstant()==0) {
								sqlite3VdbeChangeToNoop(v,testAddr-1,2);
								testAddr=0;
							}
							/* Evaluate the expression and insert it into the temp table */if(isRowid&&pE2.sqlite3ExprIsInteger(ref iValToIns)!=0) {
								v.sqlite3VdbeAddOp3(OP_InsertInt,pExpr.iTable,r2,iValToIns);
							}
							else {
								r3=pParse.sqlite3ExprCodeTarget(pE2,r1);
								if(isRowid) {
									v.sqlite3VdbeAddOp2(OP_MustBeInt,r3,v.sqlite3VdbeCurrentAddr()+2);
									v.sqlite3VdbeAddOp3(OP_Insert,pExpr.iTable,r2,r3);
								}
								else {
									v.sqlite3VdbeAddOp4(OP_MakeRecord,r3,1,r2,affinity,1);
									pParse.sqlite3ExprCacheAffinityChange(r3,1);
									v.sqlite3VdbeAddOp2(OP_IdxInsert,pExpr.iTable,r2);
								}
							}
						}
						pParse.sqlite3ReleaseTempReg(r1);
						pParse.sqlite3ReleaseTempReg(r2);
					}
				if(!isRowid) {
					v.sqlite3VdbeChangeP4(addr,keyInfo,P4_KEYINFO);
				}
				break;
			}
			case TK_EXISTS:
			case TK_SELECT:
			default: {
				/* If this has to be a scalar SELECT.  Generate code to put the
            ** value of this select in a memory cell and record the number
            ** of the memory cell in iColumn.  If this is an EXISTS, write
            ** an integer 0 (not exists) or 1 (exists) into a memory cell
            ** and record that memory cell in iColumn.
            */Select pSel;
				/* SELECT statement to encode */SelectDest dest=new SelectDest();
				/* How to deal with SELECt result */testcase(pExpr.op==TK_EXISTS);
				testcase(pExpr.op==TK_SELECT);
				Debug.Assert(pExpr.op==TK_EXISTS||pExpr.op==TK_SELECT);
				Debug.Assert(ExprHasProperty(pExpr,EP_xIsSelect));
				pSel=pExpr.x.pSelect;
				sqlite3SelectDestInit(dest,0,++pParse.nMem);
				if(pExpr.op==TK_SELECT) {
					dest.eDest=SelectResultType.Mem;
					v.sqlite3VdbeAddOp2(OP_Null,0,dest.iParm);
					#if SQLITE_DEBUG
																																																																																																				              VdbeComment( v, "Init subquery result" );
#endif
				}
				else {
					dest.eDest=SelectResultType.Exists;
					v.sqlite3VdbeAddOp2(OP_Integer,0,dest.iParm);
					#if SQLITE_DEBUG
																																																																																																				              VdbeComment( v, "Init EXISTS result" );
#endif
				}
				sqlite3ExprDelete(pParse.db,ref pSel.pLimit);
				pSel.pLimit=pParse.sqlite3PExpr(TK_INTEGER,null,null,sqlite3IntTokens[1]);
				pSel.iLimit=0;
				if(sqlite3Select(pParse,pSel,ref dest)!=0) {
					return 0;
				}
				rReg=dest.iParm;
				ExprSetIrreducible(pExpr);
				break;
			}
			}
			if(testAddr!=0) {
				v.sqlite3VdbeJumpHere(testAddr-1);
			}
			pParse.sqlite3ExprCachePop(1);
			return rReg;
		}
		#endif
		#if !SQLITE_OMIT_SUBQUERY
		/*
** Generate code for an IN expression.
**
**      x IN (SELECT ...)
**      x IN (value, value, ...)
**
** The left-hand side (LHS) is a scalar expression.  The right-hand side (RHS)
** is an array of zero or more values.  The expression is true if the LHS is
** contained within the RHS.  The value of the expression is unknown (NULL)
** if the LHS is NULL or if the LHS is not contained within the RHS and the
** RHS contains one or more NULL values.
**
** This routine generates code will jump to destIfFalse if the LHS is not 
** contained within the RHS.  If due to NULLs we cannot determine if the LHS
** is contained in the RHS then jump to destIfNull.  If the LHS is contained
** within the RHS then fall through.
*/static void sqlite3ExprCodeIN(Parse pParse,/* Parsing and code generating context */Expr pExpr,/* The IN expression */int destIfFalse,/* Jump here if LHS is not contained in the RHS */int destIfNull/* Jump here if the results are unknown due to NULLs */) {
			int rRhsHasNull=0;
			/* Register that is true if RHS contains NULL values */char affinity;
			/* Comparison affinity to use */int eType;
			/* Type of the RHS */int r1;
			/* Temporary use register */Vdbe v;
			/* Statement under construction *//* Compute the RHS.   After this step, the table with cursor
      ** pExpr.iTable will contains the values that make up the RHS.
      */v=pParse.pVdbe;
			Debug.Assert(v!=null);
			/* OOM detected prior to this routine */VdbeNoopComment(v,"begin IN expr");
			eType=sqlite3FindInIndex(pParse,pExpr,ref rRhsHasNull);
			/* Figure out the affinity to use to create a key from the results
      ** of the expression. affinityStr stores a static string suitable for
      ** P4 of OP_MakeRecord.
      */affinity=pExpr.comparisonAffinity();
			/* Code the LHS, the <expr> from "<expr> IN (...)".
      */pParse.sqlite3ExprCachePush();
			r1=pParse.sqlite3GetTempReg();
			pParse.sqlite3ExprCode(pExpr.pLeft,r1);
			/* If the LHS is NULL, then the result is either false or NULL depending
      ** on whether the RHS is empty or not, respectively.
      */if(destIfNull==destIfFalse) {
				/* Shortcut for the common case where the false and NULL outcomes are
        ** the same. */v.sqlite3VdbeAddOp2(OP_IsNull,r1,destIfNull);
			}
			else {
				int addr1=v.sqlite3VdbeAddOp1(OP_NotNull,r1);
				v.sqlite3VdbeAddOp2(OP_Rewind,pExpr.iTable,destIfFalse);
				v.sqlite3VdbeAddOp2(OP_Goto,0,destIfNull);
				v.sqlite3VdbeJumpHere(addr1);
			}
			if(eType==IN_INDEX_ROWID) {
				/* In this case, the RHS is the ROWID of table b-tree
        */v.sqlite3VdbeAddOp2(OP_MustBeInt,r1,destIfFalse);
				v.sqlite3VdbeAddOp3(OP_NotExists,pExpr.iTable,destIfFalse,r1);
			}
			else {
				/* In this case, the RHS is an index b-tree.
        */v.sqlite3VdbeAddOp4(OP_Affinity,r1,1,0,affinity,1);
				/* If the set membership test fails, then the result of the 
        ** "x IN (...)" expression must be either 0 or NULL. If the set
        ** contains no NULL values, then the result is 0. If the set 
        ** contains one or more NULL values, then the result of the
        ** expression is also NULL.
        */if(rRhsHasNull==0||destIfFalse==destIfNull) {
					/* This branch runs if it is known at compile time that the RHS
          ** cannot contain NULL values. This happens as the result
          ** of a "NOT NULL" constraint in the database schema.
          **
          ** Also run this branch if NULL is equivalent to FALSE
          ** for this particular IN operator.
          */v.sqlite3VdbeAddOp4Int(OP_NotFound,pExpr.iTable,destIfFalse,r1,1);
				}
				else {
					/* In this branch, the RHS of the IN might contain a NULL and
          ** the presence of a NULL on the RHS makes a difference in the
          ** outcome.
          */int j1,j2,j3;
					/* First check to see if the LHS is contained in the RHS.  If so,
          ** then the presence of NULLs in the RHS does not matter, so jump
          ** over all of the code that follows.
          */j1=v.sqlite3VdbeAddOp4Int(OP_Found,pExpr.iTable,0,r1,1);
					/* Here we begin generating code that runs if the LHS is not
          ** contained within the RHS.  Generate additional code that
          ** tests the RHS for NULLs.  If the RHS contains a NULL then
          ** jump to destIfNull.  If there are no NULLs in the RHS then
          ** jump to destIfFalse.
          */j2=v.sqlite3VdbeAddOp1(OP_NotNull,rRhsHasNull);
					j3=v.sqlite3VdbeAddOp4Int(OP_Found,pExpr.iTable,0,rRhsHasNull,1);
					v.sqlite3VdbeAddOp2(OP_Integer,-1,rRhsHasNull);
					v.sqlite3VdbeJumpHere(j3);
					v.sqlite3VdbeAddOp2(OP_AddImm,rRhsHasNull,1);
					v.sqlite3VdbeJumpHere(j2);
					/* Jump to the appropriate target depending on whether or not
          ** the RHS contains a NULL
          */v.sqlite3VdbeAddOp2(OP_If,rRhsHasNull,destIfNull);
					v.sqlite3VdbeAddOp2(OP_Goto,0,destIfFalse);
					/* The OP_Found at the top of this branch jumps here when true, 
          ** causing the overall IN expression evaluation to fall through.
          */v.sqlite3VdbeJumpHere(j1);
				}
			}
			pParse.sqlite3ReleaseTempReg(r1);
			pParse.sqlite3ExprCachePop(1);
			VdbeComment(v,"end IN expr");
		}
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
		static void codeReal(Vdbe v,string z,bool negateFlag,int iMem) {
			if(ALWAYS(!String.IsNullOrEmpty(z))) {
				double value=0;
				//string zV;
				Converter.sqlite3AtoF(z,ref value,StringExtensions.sqlite3Strlen30(z),SqliteEncoding.UTF8);
				Debug.Assert(!MathExtensions.sqlite3IsNaN(value));
				/* The new AtoF never returns NaN */if(negateFlag)
					value=-value;
				//zV = dup8bytes(v,  value);
				v.sqlite3VdbeAddOp4(OP_Real,0,iMem,0,value,P4_REAL);
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
		static void codeInteger(Parse pParse,Expr pExpr,bool negFlag,int iMem) {
			Vdbe v=pParse.pVdbe;
			if((pExpr.flags&EP_IntValue)!=0) {
				int i=pExpr.u.iValue;
				Debug.Assert(i>=0);
				if(negFlag)
					i=-i;
				v.sqlite3VdbeAddOp2(OP_Integer,i,iMem);
			}
			else {
				int c;
				i64 value=0;
				string z=pExpr.u.zToken;
				Debug.Assert(!String.IsNullOrEmpty(z));
				c=Converter.sqlite3Atoi64(z,ref value,StringExtensions.sqlite3Strlen30(z),SqliteEncoding.UTF8);
				if(c==0||(c==2&&negFlag)) {
					//char* zV;
					if(negFlag) {
						value=c==2?IntegerExtensions.SMALLEST_INT64:-value;
					}
					v.sqlite3VdbeAddOp4(OP_Int64,0,iMem,0,value,P4_INT64);
				}
				else {
					#if SQLITE_OMIT_FLOATING_POINT
																																																																																																				sqlite3ErrorMsg(pParse, "oversized integer: %s%s", negFlag ? "-" : "", z);
#else
					codeReal(v,z,negFlag,iMem);
					#endif
				}
			}
		}
		///<summary>
		/// Clear a cache entry.
		///
		///</summary>
		static void cacheEntryClear(Parse pParse,yColCache p) {
			if(p.tempReg!=0) {
				if(pParse.nTempReg<ArraySize(pParse.aTempReg)) {
					pParse.aTempReg[pParse.nTempReg++]=p.iReg;
				}
				p.tempReg=0;
			}
		}
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
** This routine is used within Debug.Assert() and testcase() macros only
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
		/// into a register and convert the expression into a TK_REGISTER
		/// expression.
		///
		///</summary>
		static int evalConstExpr(Walker pWalker,ref Expr pExpr) {
			Parse pParse=pWalker.pParse;
			switch(pExpr.op) {
			case TK_IN:
			case TK_REGISTER: {
				return WRC_Prune;
			}
			case TK_FUNCTION:
			case TK_AGG_FUNCTION:
			case TK_CONST_FUNC: {
				/* The arguments to a function have a fixed destination.
            ** Mark them this way to avoid generated unneeded OP_SCopy
            ** instructions.
            */ExprList pList=pExpr.x.pList;
				Debug.Assert(!ExprHasProperty(pExpr,EP_xIsSelect));
				if(pList!=null) {
					int i=pList.nExpr;
					ExprList_item pItem;
					//= pList.a;
					for(;i>0;i--) {
						//, pItem++){
						pItem=pList.a[pList.nExpr-i];
						if(ALWAYS(pItem.pExpr!=null))
							pItem.pExpr.flags|=EP_FixedDest;
					}
				}
				break;
			}
			}
			if(pExpr.isAppropriateForFactoring()!=0) {
				int r1=++pParse.nMem;
				int r2;
				r2=pParse.sqlite3ExprCodeTarget(pExpr,r1);
				if(NEVER(r1!=r2))
					pParse.sqlite3ReleaseTempReg(r1);
				pExpr.op2=pExpr.op;
				pExpr.op=TK_REGISTER;
				pExpr.iTable=r2;
				return WRC_Prune;
			}
			return WRC_Continue;
		}
		///<summary>
		/// Preevaluate constant subexpressions within pExpr and store the
		/// results in registers.  Modify pExpr so that the constant subexpresions
		/// are TK_REGISTER opcodes that refer to the precomputed values.
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
		/// take the jump if the jumpIfNull flag is SQLITE_JUMPIFNULL.
		///
		/// This code depends on the fact that certain token values (ex: TK_EQ)
		/// are the same as opcode values (ex: OP_Eq) that implement the corresponding
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
		/// jump if jumpIfNull is SQLITE_JUMPIFNULL or fall through if jumpIfNull
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
		static int sqlite3ExprCompare(Expr pA,Expr pB) {
			if(pA==null||pB==null) {
				return pB==pA?0:2;
			}
			Debug.Assert(!ExprHasAnyProperty(pA,EP_TokenOnly|EP_Reduced));
			Debug.Assert(!ExprHasAnyProperty(pB,EP_TokenOnly|EP_Reduced));
			if(ExprHasProperty(pA,EP_xIsSelect)||ExprHasProperty(pB,EP_xIsSelect)) {
				return 2;
			}
			if((pA.flags&EP_Distinct)!=(pB.flags&EP_Distinct))
				return 2;
			if(pA.op!=pB.op)
				return 2;
			if(sqlite3ExprCompare(pA.pLeft,pB.pLeft)!=0)
				return 2;
			if(sqlite3ExprCompare(pA.pRight,pB.pRight)!=0)
				return 2;
			if(sqlite3ExprListCompare(pA.x.pList,pB.x.pList)!=0)
				return 2;
			if(pA.iTable!=pB.iTable||pA.iColumn!=pB.iColumn)
				return 2;
			if(ExprHasProperty(pA,EP_IntValue)) {
				if(!ExprHasProperty(pB,EP_IntValue)||pA.u.iValue!=pB.u.iValue) {
					return 2;
				}
			}
			else
				if(pA.op!=TK_COLUMN&&pA.u.zToken!=null) {
					if(ExprHasProperty(pB,EP_IntValue)||NEVER(pB.u.zToken==null))
						return 2;
					if(!pA.u.zToken.Equals(pB.u.zToken,StringComparison.InvariantCultureIgnoreCase)) {
						return 2;
					}
				}
			if((pA.flags&EP_ExpCollate)!=(pB.flags&EP_ExpCollate))
				return 1;
			if((pA.flags&EP_ExpCollate)!=0&&pA.pColl!=pB.pColl)
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
		static int sqlite3ExprListCompare(ExprList pA,ExprList pB) {
			int i;
			if(pA==null&&pB==null)
				return 0;
			if(pA==null||pB==null)
				return 1;
			if(pA.nExpr!=pB.nExpr)
				return 1;
			for(i=0;i<pA.nExpr;i++) {
				Expr pExprA=pA.a[i].pExpr;
				Expr pExprB=pB.a[i].pExpr;
				if(pA.a[i].sortOrder!=pB.a[i].sortOrder)
					return 1;
				if(sqlite3ExprCompare(pExprA,pExprB)!=0)
					return 1;
			}
			return 0;
		}
		///<summary>
		/// Add a new element to the pAggInfo.aCol[] array.  Return the index of
		/// the new element.  Return a negative number if malloc fails.
		///
		///</summary>
		static int addAggInfoColumn(sqlite3 db,AggInfo pInfo) {
			int i=0;
			pInfo.aCol=sqlite3ArrayAllocate(db,pInfo.aCol,-1,//sizeof(pInfo.aCol[0]),
			3,ref pInfo.nColumn,ref pInfo.nColumnAlloc,ref i);
			return i;
		}
		///<summary>
		/// Add a new element to the pAggInfo.aFunc[] array.  Return the index of
		/// the new element.  Return a negative number if malloc fails.
		///
		///</summary>
		static int addAggInfoFunc(sqlite3 db,AggInfo pInfo) {
			int i=0;
			pInfo.aFunc=sqlite3ArrayAllocate(db,pInfo.aFunc,-1,//sizeof(pInfo.aFunc[0]),
			3,ref pInfo.nFunc,ref pInfo.nFuncAlloc,ref i);
			return i;
		}
		///<summary>
		/// This is the xExprCallback for a tree walker.  It is used to
		/// implement sqlite3ExprAnalyzeAggregates().  See sqlite3ExprAnalyzeAggregates
		/// for additional information.
		///
		///</summary>
		static int analyzeAggregate(Walker pWalker,ref Expr pExpr) {
			int i;
			NameContext pNC=pWalker.u.pNC;
			Parse pParse=pNC.pParse;
			SrcList pSrcList=pNC.pSrcList;
			AggInfo pAggInfo=pNC.pAggInfo;
			switch(pExpr.op) {
			case TK_AGG_COLUMN:
			case TK_COLUMN: {
				testcase(pExpr.op==TK_AGG_COLUMN);
				testcase(pExpr.op==TK_COLUMN);
				/* Check to see if the column is in one of the tables in the FROM
            ** clause of the aggregate query */if(ALWAYS(pSrcList!=null)) {
					SrcList_item pItem;
					// = pSrcList.a;
					for(i=0;i<pSrcList.nSrc;i++) {
						//, pItem++){
						pItem=pSrcList.a[i];
						AggInfo_col pCol;
						Debug.Assert(!ExprHasAnyProperty(pExpr,EP_TokenOnly|EP_Reduced));
						if(pExpr.iTable==pItem.iCursor) {
							/* If we reach this point, it means that pExpr refers to a table
                  ** that is in the FROM clause of the aggregate query.
                  **
                  ** Make an entry for the column in pAggInfo.aCol[] if there
                  ** is not an entry there already.
                  */int k;
							//pCol = pAggInfo.aCol;
							for(k=0;k<pAggInfo.nColumn;k++) {
								//, pCol++){
								pCol=pAggInfo.aCol[k];
								if(pCol.iTable==pExpr.iTable&&pCol.iColumn==pExpr.iColumn) {
									break;
								}
							}
							if((k>=pAggInfo.nColumn)&&(k=addAggInfoColumn(pParse.db,pAggInfo))>=0) {
								pCol=pAggInfo.aCol[k];
								pCol.pTab=pExpr.pTab;
								pCol.iTable=pExpr.iTable;
								pCol.iColumn=pExpr.iColumn;
								pCol.iMem=++pParse.nMem;
								pCol.iSorterColumn=-1;
								pCol.pExpr=pExpr;
								if(pAggInfo.pGroupBy!=null) {
									int j,n;
									ExprList pGB=pAggInfo.pGroupBy;
									ExprList_item pTerm;
									// = pGB.a;
									n=pGB.nExpr;
									for(j=0;j<n;j++) {
										//, pTerm++){
										pTerm=pGB.a[j];
										Expr pE=pTerm.pExpr;
										if(pE.op==TK_COLUMN&&pE.iTable==pExpr.iTable&&pE.iColumn==pExpr.iColumn) {
											pCol.iSorterColumn=j;
											break;
										}
									}
								}
								if(pCol.iSorterColumn<0) {
									pCol.iSorterColumn=pAggInfo.nSortingColumn++;
								}
							}
							/* There is now an entry for pExpr in pAggInfo.aCol[] (either
                  ** because it was there before or because we just created it).
                  ** Convert the pExpr to be a TK_AGG_COLUMN referring to that
                  ** pAggInfo.aCol[] entry.
                  */ExprSetIrreducible(pExpr);
							pExpr.pAggInfo=pAggInfo;
							pExpr.op=TK_AGG_COLUMN;
							pExpr.iAgg=(short)k;
							break;
						}
						/* endif pExpr.iTable==pItem.iCursor */}
					/* end loop over pSrcList */}
				return WRC_Prune;
			}
			case TK_AGG_FUNCTION: {
				/* The pNC.nDepth==0 test causes aggregate functions in subqueries
            ** to be ignored */if(pNC.nDepth==0) {
					/* Check to see if pExpr is a duplicate of another aggregate
              ** function that is already in the pAggInfo structure
              */AggInfo_func pItem;
					// = pAggInfo.aFunc;
					for(i=0;i<pAggInfo.nFunc;i++) {
						//, pItem++){
						pItem=pAggInfo.aFunc[i];
						if(sqlite3ExprCompare(pItem.pExpr,pExpr)==0) {
							break;
						}
					}
					if(i>=pAggInfo.nFunc) {
						/* pExpr is original.  Make a new entry in pAggInfo.aFunc[]
                */SqliteEncoding enc=pParse.db.aDbStatic[0].pSchema.enc;
						// ENC(pParse.db);
						i=addAggInfoFunc(pParse.db,pAggInfo);
						if(i>=0) {
							Debug.Assert(!ExprHasProperty(pExpr,EP_xIsSelect));
							pItem=pAggInfo.aFunc[i];
							pItem.pExpr=pExpr;
							pItem.iMem=++pParse.nMem;
							Debug.Assert(!ExprHasProperty(pExpr,EP_IntValue));
							pItem.pFunc=sqlite3FindFunction(pParse.db,pExpr.u.zToken,StringExtensions.sqlite3Strlen30(pExpr.u.zToken),pExpr.x.pList!=null?pExpr.x.pList.nExpr:0,enc,0);
							if((pExpr.flags&EP_Distinct)!=0) {
								pItem.iDistinct=pParse.nTab++;
							}
							else {
								pItem.iDistinct=-1;
							}
						}
					}
					/* Make pExpr point to the appropriate pAggInfo.aFunc[] entry
              */Debug.Assert(!ExprHasAnyProperty(pExpr,EP_TokenOnly|EP_Reduced));
					ExprSetIrreducible(pExpr);
					pExpr.iAgg=(short)i;
					pExpr.pAggInfo=pAggInfo;
					return WRC_Prune;
				}
				break;
			}
			}
			return WRC_Continue;
		}
		static int analyzeAggregatesInSelect(Walker pWalker,Select pSelect) {
			NameContext pNC=pWalker.u.pNC;
			if(pNC.nDepth==0) {
				pNC.nDepth++;
				pWalker.sqlite3WalkSelect(pSelect);
				pNC.nDepth--;
				return WRC_Prune;
			}
			else {
				return WRC_Continue;
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
		static void sqlite3ExprAnalyzeAggregates(NameContext pNC,ref Expr pExpr) {
			Walker w=new Walker();
			w.xExprCallback=(dxExprCallback)analyzeAggregate;
			w.xSelectCallback=(dxSelectCallback)analyzeAggregatesInSelect;
			w.u.pNC=pNC;
			Debug.Assert(pNC.pSrcList!=null);
			w.sqlite3WalkExpr(ref pExpr);
		}
		///<summary>
		/// Call sqlite3ExprAnalyzeAggregates() for every expression in an
		/// expression list.  Return the number of errors.
		///
		/// If an error is found, the analysis is cut short.
		///
		///</summary>
		static void sqlite3ExprAnalyzeAggList(NameContext pNC,ExprList pList) {
			ExprList_item pItem;
			int i;
			if(pList!=null) {
				for(i=0;i<pList.nExpr;i++)//, pItem++)
				 {
					pItem=pList.a[i];
					sqlite3ExprAnalyzeAggregates(pNC,ref pItem.pExpr);
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
