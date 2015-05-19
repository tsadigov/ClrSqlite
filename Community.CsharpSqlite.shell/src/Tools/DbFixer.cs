using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Community.CsharpSqlite
{
    using Vdbe = Community.CsharpSqlite.Sqlite3.Vdbe;
    using sqlite3_value = Engine.Mem;
    
    using Parse = Community.CsharpSqlite.Sqlite3.Parse;
    
    using System.Diagnostics;
    using Community.CsharpSqlite.Ast;
    using Community.CsharpSqlite.Metadata;

    ///<summary>
    /// The following structure contains information used by the sqliteFix...
    /// routines as they walk the parse tree to make database references
    /// explicit.
    ///
    ///</summary>
    //typedef struct DbFixer DbFixer;
    public class DbFixer
    {
        public Parse pParse;
        ///
        ///<summary>
        ///The parsing context.  Error messages written here 
        ///</summary>
        public string zDb;
        ///
        ///<summary>
        ///Make sure all objects are contained in this database 
        ///</summary>
        public string zType;
        ///
        ///<summary>
        ///</summary>
        ///<param name="Type of the container "> used for error messages </param>
        public Token pName;
        ///
        ///<summary>
        ///</summary>
        ///<param name="Name of the container "> used for error messages </param>
        public///<summary>
            /// Initialize a DbFixer structure.  This routine must be called prior
            /// to passing the structure to one of the sqliteFixAAAA() routines below.
            ///
            /// The return value indicates whether or not fixation is required.  TRUE
            /// means we do need to fix the database references, FALSE means we do not.
            ///</summary>
        int sqlite3FixInit(///
            ///<summary>
            ///The fixer to be initialized 
            ///</summary>
        Parse pParse,///
            ///<summary>
            ///Error messages will be written here 
            ///</summary>
        int iDb,///
            ///<summary>
            ///This is the database that must be used 
            ///</summary>
        string zType,///
            ///<summary>
            ///"view", "trigger", or "index" 
            ///</summary>
        Token pName///
            ///<summary>
            ///Name of the view, trigger, or index 
            ///</summary>
        )
        {
            sqlite3 db;
            if (Sqlite3.NEVER(iDb < 0) || iDb == 1)
                return 0;
            db = pParse.db;
            Debug.Assert(db.nDb > iDb);
            this.pParse = pParse;
            this.zDb = db.aDb[iDb].zName;
            this.zType = zType;
            this.pName = pName;
            return 1;
        }
        public///<summary>
            /// The following set of routines walk through the parse tree and assign
            /// a specific database to all table references where the database name
            /// was left unspecified in the original SQL statement.  The pFix structure
            /// must have been initialized by a prior call to sqlite3FixInit().
            ///
            /// These routines are used to make sure that an index, trigger, or
            /// view in one database does not refer to objects in a different database.
            /// (Exception: indices, triggers, and views in the TEMP database are
            /// allowed to refer to anything.)  If a reference is explicitly made
            /// to an object in a different database, an error message is added to
            /// pParse.zErrMsg and these routines return non-zero.  If everything
            /// checks out, these routines return 0.
            ///</summary>
        int sqlite3FixSrcList(///
            ///<summary>
            ///Context of the fixation 
            ///</summary>
        SrcList pList///
            ///<summary>
            ///The Source list to check and modify 
            ///</summary>
        )
        {
            int i;
            string zDb;
            SrcList_item pItem;
            if (Sqlite3.NEVER(pList == null))
                return 0;
            zDb = this.zDb;
            for (i = 0; i < pList.nSrc; i++)
            {
                //, pItem++){
                pItem = pList.a[i];
                if (pItem.zDatabase == null)
                {
                    pItem.zDatabase = zDb;
                    // sqlite3DbStrDup( pFix.pParse.db, zDb );
                }
                else
                    if (!pItem.zDatabase.Equals(zDb, StringComparison.InvariantCultureIgnoreCase))
                    {
                        utilc.sqlite3ErrorMsg(this.pParse, "%s %T cannot reference objects in database %s", this.zType, this.pName, pItem.zDatabase);
                        return 1;
                    }
#if !SQLITE_OMIT_VIEW || !SQLITE_OMIT_TRIGGER
                if (this.sqlite3FixSelect(pItem.pSelect) != 0)
                    return 1;
                if (this.sqlite3FixExpr(pItem.pOn) != 0)
                    return 1;
#endif
            }
            return 0;
        }
        public int sqlite3FixSelect(///
            ///<summary>
            ///Context of the fixation 
            ///</summary>
        Select pSelect///
            ///<summary>
            ///The SELECT statement to be fixed to one database 
            ///</summary>
        )
        {
            while (pSelect != null)
            {
                if (this.sqlite3FixExprList(pSelect.pEList) != 0)
                {
                    return 1;
                }
                if (this.sqlite3FixSrcList(pSelect.pSrc) != 0)
                {
                    return 1;
                }
                if (this.sqlite3FixExpr(pSelect.pWhere) != 0)
                {
                    return 1;
                }
                if (this.sqlite3FixExpr(pSelect.pHaving) != 0)
                {
                    return 1;
                }
                pSelect = pSelect.pPrior;
            }
            return 0;
        }
        public int sqlite3FixExpr(///
            ///<summary>
            ///Context of the fixation 
            ///</summary>
        Expr pExpr///
            ///<summary>
            ///The expression to be fixed to one database 
            ///</summary>
        )
        {
            while (pExpr != null)
            {
                if (pExpr.ExprHasAnyProperty(ExprFlags.EP_TokenOnly))
                    break;
                if (pExpr.ExprHasProperty(ExprFlags.EP_xIsSelect))
                {
                    if (this.sqlite3FixSelect(pExpr.x.pSelect) != 0)
                        return 1;
                }
                else
                {
                    if (this.sqlite3FixExprList(pExpr.x.pList) != 0)
                        return 1;
                }
                if (this.sqlite3FixExpr(pExpr.pRight) != 0)
                {
                    return 1;
                }
                pExpr = pExpr.pLeft;
            }
            return 0;
        }
        public int sqlite3FixExprList(///
            ///<summary>
            ///Context of the fixation 
            ///</summary>
        ExprList pList///
            ///<summary>
            ///The expression to be fixed to one database 
            ///</summary>
        )
        {
            int i;
            ExprList_item pItem;
            if (pList == null)
                return 0;
            for (i = 0; i < pList.nExpr; i++)//, pItem++ )
            {
                pItem = pList.a[i];
                if (this.sqlite3FixExpr(pItem.pExpr) != 0)
                {
                    return 1;
                }
            }
            return 0;
        }
        public int sqlite3FixTriggerStep(///
            ///<summary>
            ///Context of the fixation 
            ///</summary>
        TriggerStep pStep///
            ///<summary>
            ///The trigger step be fixed to one database 
            ///</summary>
        )
        {
            while (pStep != null)
            {
                if (this.sqlite3FixSelect(pStep.pSelect) != 0)
                {
                    return 1;
                }
                if (this.sqlite3FixExpr(pStep.pWhere) != 0)
                {
                    return 1;
                }
                if (this.sqlite3FixExprList(pStep.pExprList) != 0)
                {
                    return 1;
                }
                pStep = pStep.pNext;
            }
            return 0;
        }
    }

}
