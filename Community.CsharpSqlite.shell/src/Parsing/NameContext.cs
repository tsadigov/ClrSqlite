using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bitmask=System.UInt64;
using i16=System.Int16;
using i64=System.Int64;
using sqlite3_int64=System.Int64;
using u8=System.Byte;
using u16=System.UInt16;
using u32=System.UInt32;
using u64=System.UInt64;
using unsigned = System.UInt64;
using Community.CsharpSqlite.Ast;

namespace Community.CsharpSqlite
{
    
        ///<summary>
        /// A NameContext defines a context in which to resolve table and column
        /// names.  The context consists of a list of tables (the pSrcList) field and
        /// a list of named expression (pEList).  The named expression list may
        /// be NULL.  The pSrc corresponds to the FROM clause of a SELECT or
        /// to the table being operated on by INSERT, UPDATE, or DELETE.  The
        /// pEList corresponds to the result set of a SELECT and is NULL for
        /// other statements.
        ///
        /// NameContexts can be nested.  When resolving names, the inner-most
        /// context is searched first.  If no match is found, the next outer
        /// context is checked.  If there is still no match, the next context
        /// is checked.  This process continues until either a match is found
        /// or all contexts are check.  When a match is found, the nRef member of
        /// the context containing the match is incremented.
        ///
        /// Each subquery gets a new NameContext.  The pNext field points to the
        /// NameContext in the parent query.  Thus the process of scanning the
        /// NameContext list corresponds to searching through successively outer
        /// subqueries looking for a match.
        ///
        ///</summary>
        public class NameContext
        {
            public Sqlite3.Parse pParse;
            ///
            ///<summary>
            ///The parser 
            ///</summary>
            public SrcList pSrcList;
            ///
            ///<summary>
            ///One or more tables used to resolve names 
            ///</summary>
            public ExprList pEList;
            ///
            ///<summary>
            ///Optional list of named expressions 
            ///</summary>
            public int nRef;
            ///
            ///<summary>
            ///Number of names resolved by this context 
            ///</summary>
            public int nErr;
            ///
            ///<summary>
            ///Number of errors encountered while resolving names 
            ///</summary>
            public u8 allowAgg;
            ///
            ///<summary>
            ///Aggregate functions allowed here 
            ///</summary>
            public u8 hasAgg;
            ///
            ///<summary>
            ///True if aggregates are seen 
            ///</summary>
            public u8 isCheck;
            ///
            ///<summary>
            ///True if resolving names in a CHECK constraint 
            ///</summary>
            public int nDepth;
            ///
            ///<summary>
            ///Depth of subquery recursion. 1 for no recursion 
            ///</summary>
            public AggInfo pAggInfo;
            ///
            ///<summary>
            ///Information about aggregates at this level 
            ///</summary>
            public NameContext pNext;
            ///
            ///<summary>
            ///Next outer name context.  NULL for outermost 
            ///</summary>
            public///<summary>
                  /// Resolve an expression that was part of an ATTACH or DETACH statement. This
                  /// is slightly different from resolving a normal SQL expression, because simple
                  /// identifiers are treated as strings, not possible column names or aliases.
                  ///
                  /// i.e. if the parser sees:
                  ///
                  ///     ATTACH DATABASE abc AS def
                  ///
                  /// it treats the two expressions as literal strings 'abc' and 'def' instead of
                  /// looking for columns of the same name.
                  ///
                  /// This only applies to the root node of pExpr, so the statement:
                  ///
                  ///     ATTACH DATABASE abc||def AS 'db2'
                  ///
                  /// will fail because neither abc or def can be resolved.
                  ///</summary>
            SqlResult resolveAttachExpr(Expr pExpr)
            {
                var rc = SqlResult.SQLITE_OK;
                if (pExpr != null)
                {
                    if (pExpr.op != Sqlite3.TK_ID)
                    {
                        rc = Sqlite3.ResolveExtensions.sqlite3ResolveExprNames(this, ref pExpr);
                        if (rc == SqlResult.SQLITE_OK && pExpr.sqlite3ExprIsConstant() == 0)
                        {
                            utilc.sqlite3ErrorMsg(this.pParse, "invalid name: \"%s\"", pExpr.u.zToken);
                            return SqlResult.SQLITE_ERROR;
                        }
                    }
                    else
                    {
                        pExpr.op = Sqlite3.TK_STRING;
                    }
                }
                return rc;
            }
        }
    
}
