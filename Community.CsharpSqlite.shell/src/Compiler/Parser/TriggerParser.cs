using System;
using System.Diagnostics;
using System.Text;
using u8 = System.Byte;
using u32 = System.UInt32;
using System.Linq;

namespace Community.CsharpSqlite.Parsing
{
    using ParseState = Sqlite3.ParseState;
    using Ast;
    using Metadata;
    using Os;
    using Vdbe = Engine.Vdbe;
    using Community.CsharpSqlite.Engine;
    using Community.CsharpSqlite.Utils;
    using Compiler.Parser;
    using Compiler.CodeGeneration;
    public static partial class TriggerParser
    {

        ///<summary>
        ///
        /// The author disclaims copyright to this source code.  In place of
        /// a legal notice, here is a blessing:
        ///
        ///    May you do good and not evil.
        ///    May you find forgiveness for yourself and forgive others.
        ///    May you share freely, never taking more than you give.
        ///
        ///
        /// This file contains the implementation for TRIGGERs
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
#if !SQLITE_OMIT_TRIGGER
        ///<summary>
        /// Delete a linked list of TriggerStep structures.
        ///</summary>
        public static void sqlite3DeleteTriggerStep(Connection db, ref TriggerStep pTriggerStep)
        {
            while (pTriggerStep != null)
            {
                TriggerStep pTmp = pTriggerStep;
                pTriggerStep = pTriggerStep.pNext;
                exprc.Delete(db, ref pTmp.pWhere);
                exprc.Delete(db, ref pTmp.pExprList);
                SelectMethods.SelectDestructor(db, ref pTmp.pSelect);
                build.sqlite3IdListDelete(db, ref pTmp.pIdList);
                pTriggerStep = null;
                db.DbFree(ref pTmp);
            }
        }
        ///<summary>
        /// Given table pTab, return a list of all the triggers attached to
        /// the table. The list is connected by Trigger.pNext pointers.
        ///
        /// All of the triggers on pTab that are in the same database as pTab
        /// are already attached to pTab.pTrigger.  But there might be additional
        /// triggers on pTab in the TEMP schema.  This routine prepends all
        /// TEMP triggers on pTab to the beginning of the pTab.pTrigger list
        /// and returns the combined list.
        ///
        /// To state it another way:  This routine returns a list of all triggers
        /// that fire off of pTab.  The list will include any TEMP triggers on
        /// pTab as well as the triggers lised in pTab.pTrigger.
        ///
        ///</summary>
        public static Trigger sqlite3TriggerList(this Table pTab, ParseState pParse)
        {
            Schema pTmpSchema = pParse.db.Backends[1].pSchema;
            Trigger pList = null;
            ///
            ///<summary>
            ///List of triggers to return 
            ///</summary>
            if (pParse.disableTriggers != 0)
            {
                return null;
            }
            if (pTmpSchema != pTab.pSchema)
            {
                HashElem p;
                Debug.Assert(Sqlite3.sqlite3SchemaMutexHeld(pParse.db, 0, pTmpSchema));
                for (p = pTmpSchema.Triggers.sqliteHashFirst(); p != null; p = p.sqliteHashNext())
                {
                    Trigger pTrig = (Trigger)p.sqliteHashData();
                    if (pTrig.pTabSchema == pTab.pSchema && pTrig.table.Equals(pTab.zName, StringComparison.InvariantCultureIgnoreCase))
                    {
                        pTrig.pNext = (pList != null ? pList : pTab.pTrigger);
                        pList = pTrig;
                    }
                }
            }
            return (pList != null ? pList : pTab.pTrigger);
        }
        ///<summary>
        /// This is called by the parser when it sees a CREATE TRIGGER statement
        /// up to the point of the BEGIN before the trigger actions.  A Trigger
        /// structure is generated based on the information available and stored
        /// in pParse.pNewTrigger.  After the trigger actions have been parsed, the
        /// sqlite3FinishTrigger() function is called to complete the trigger
        /// construction process.
        ///
        ///</summary>
        public static void sqlite3BeginTrigger(ParseState pParse,///
            ///<summary>
            ///The parse context of the CREATE TRIGGER statement 
            ///</summary>
        Token pName1,///
            ///<summary>
            ///The name of the trigger 
            ///</summary>
        Token pName2,///
            ///<summary>
            ///The name of the trigger 
            ///</summary>
        TokenType tr_tm,///
                  ///<summary>
                  ///One of TokenType.TK_BEFORE, TokenType.TK_AFTER, TokenType.TK_INSTEAD 
                  ///</summary>
        TokenType op,///
            ///<summary>
            ///One of TokenType.TK_INSERT, TokenType.TK_UPDATE, TokenType.TK_DELETE 
            ///</summary>
        IdList pColumns,///
            ///<summary>
            ///column list if this is an UPDATE OF trigger 
            ///</summary>
        SrcList pTableName,///
            ///<summary>
            ///The name of the table/view the trigger applies to 
            ///</summary>
        Expr pWhen,///
            ///<summary>
            ///WHEN clause 
            ///</summary>
        int isTemp,///
            ///<summary>
            ///True if the TEMPORARY keyword is present 
            ///</summary>
        int noErr///
            ///<summary>
            ///Suppress errors if the trigger already exists 
            ///</summary>
        )
        {
            Trigger pTrigger = null;
            ///The new trigger 
            
            string zName = null;
            ///Name of the trigger 
            Connection db = pParse.db;
            ///The database connection 
            int iDb;
            ///The database to store the trigger in 
            Token pName = null;
            ///The unqualified db name 
            DbFixer sFix = new DbFixer();
            ///State vector for the DB fixer 
            int iTabDb;
            ///Index of the database holding pTab 
            Debug.Assert(pName1 != null);
            ///pName1.z might be NULL, but not pName1 itself 
            Debug.Assert(pName2 != null);
            Debug.Assert(op == TokenType.TK_INSERT || op == TokenType.TK_UPDATE || op == TokenType.TK_DELETE);
            Debug.Assert(op > 0 && (int)op < 0xff);
            if (isTemp != 0)
            {
                ///If TEMP was specified, then the trigger name may not be qualified. 
                if (pName2.Length > 0)
                {
                    utilc.sqlite3ErrorMsg(pParse, "temporary trigger may not have qualified name");
                    goto trigger_cleanup;
                }
                iDb = 1;
                pName = pName1;
            }
            else
            {
                ///Figure out the db that the the trigger will be created in 
                iDb = build.sqlite3TwoPartName(pParse, pName1, pName2, ref pName);
                if (iDb < 0)
                {
                    goto trigger_cleanup;
                }
            }
            if (null == pTableName)//|| db.mallocFailed 
            {
                goto trigger_cleanup;
            }
            ///<param name="A long">standing parser bug is that this syntax was allowed:</param>
            ///<param name=""></param>
            ///<param name="CREATE TRIGGER attached.demo AFTER INSERT ON attached.tab ....">CREATE TRIGGER attached.demo AFTER INSERT ON attached.tab ....</param>
            ///<param name="^^^^^^^^">^^^^^^^^</param>
            ///<param name=""></param>
            ///<param name="To maintain backwards compatibility, ignore the database">To maintain backwards compatibility, ignore the database</param>
            ///<param name="name on pTableName if we are reparsing our of SQLITE_MASTER.">name on pTableName if we are reparsing our of SQLITE_MASTER.</param>
            ///<param name=""></param>
            if (db.init.busy != 0 && iDb != 1)
            {
                //sqlite3DbFree( db, pTableName.a[0].zDatabase );
                pTableName.a[0].zDatabase = null;
            }
            ///If the trigger name was unqualified, and the table is a temp table,
            ///then set iDb to 1 to create the trigger in the temporary database.
            ///If sqlite3SrcListLookup() returns 0, indicating the table does not
            ///exist, the error is caught by the block below.
            if (pTableName == null                
                ///|| db.mallocFailed != 0                 
            )
            {
                goto trigger_cleanup;
            }

            ///Table that the trigger fires off of 
            var pTab = pParse.GetFirstTableInTheList(pTableName);
            if (db.init.busy == 0 && pName2.Length == 0 && pTab != null && pTab.pSchema == db.Backends[1].pSchema)
            {
                iDb = 1;
            }
            ///
            ///<summary>
            ///Ensure the table name matches database name and that the table exists 
            ///</summary>
            //      if ( db.mallocFailed != 0 ) goto trigger_cleanup;
            Debug.Assert(pTableName.Count == 1);
            if (sFix.sqlite3FixInit(pParse, iDb, "trigger", pName) != 0 && sFix.sqlite3FixSrcList(pTableName) != 0)
            {
                goto trigger_cleanup;
            }
            pTab = pParse.GetFirstTableInTheList(pTableName);
            if (pTab == null)
            {
                ///The table does not exist. 
                if (db.init.iDb == 1)
                {
                    ///
                    ///<summary>
                    ///Ticket #3810.
                    ///Normally, whenever a table is dropped, all associated triggers are
                    ///</summary>
                    ///dropped too.  But if a TEMP trigger is created on a non-TEMP table
                    ///and the table is dropped by a different database connection, the
                    ///trigger is not visible to the database connection that does the
                    ///drop so the trigger cannot be dropped.  This results in an
                    ///orphaned trigger" "> a trigger whose associated table is missing.                    
                    db.init.orphanTrigger = true;
                }
                goto trigger_cleanup;
            }
            if (pTab.IsVirtual())
            {
                utilc.sqlite3ErrorMsg(pParse, "cannot create triggers on virtual tables");
                goto trigger_cleanup;
            }
            ///
            ///<summary>
            ///Check that the trigger name is not reserved and that no trigger of the
            ///specified name exists 
            ///</summary>
            zName = build.Token2Name(db, pName);
            if (zName == null || SqlResult.SQLITE_OK != build.sqlite3CheckObjectName(pParse, zName))
            {
                goto trigger_cleanup;
            }
            Debug.Assert(Sqlite3.sqlite3SchemaMutexHeld(db, iDb, null));
            if ((db.Backends[iDb].pSchema.Triggers).Find(zName, StringExtensions.Strlen30(zName), (Trigger)null) != null)
            {
                if (noErr == 0)
                {
                    utilc.sqlite3ErrorMsg(pParse, "trigger %T already exists", pName);
                }
                else
                {
                    Debug.Assert(0 == db.init.busy);
                    build.sqlite3CodeVerifySchema(pParse, iDb);
                }
                goto trigger_cleanup;
            }
            ///
            ///<summary>
            ///Do not create a trigger on a system table 
            ///</summary>
            if (pTab.zName.StartsWith("sqlite_", System.StringComparison.InvariantCultureIgnoreCase))
            {
                utilc.sqlite3ErrorMsg(pParse, "cannot create trigger on system table");
                pParse.nErr++;
                goto trigger_cleanup;
            }
            ///
            ///<summary>
            ///INSTEAD of triggers are only for views and views only support INSTEAD
            ///of triggers.
            ///
            ///</summary>
            if (pTab.pSelect != null && tr_tm != TokenType.TK_INSTEAD)
            {
                utilc.sqlite3ErrorMsg(pParse, "cannot create %s trigger on view: %S", (tr_tm == TokenType.TK_BEFORE) ? "BEFORE" : "AFTER", pTableName, 0);
                goto trigger_cleanup;
            }
            if (pTab.pSelect == null && tr_tm == TokenType.TK_INSTEAD)
            {
                utilc.sqlite3ErrorMsg(pParse, "cannot create INSTEAD OF" + " trigger on table: %S", pTableName, 0);
                goto trigger_cleanup;
            }
            iTabDb = db.indexOfBackendWithSchema( pTab.pSchema);
#if !SQLITE_OMIT_AUTHORIZATION
																																																																								{
int code = SQLITE_CREATE_TRIGGER;
string zDb = db.aDb[iTabDb].zName;
string zDbTrig = isTemp ? db.aDb[1].zName : zDb;
if( iTabDb==1 || isTemp ) code = SQLITE_CREATE_TEMP_TRIGGER;
if( sqlite3AuthCheck(pParse, code, zName, pTab.zName, zDbTrig) ){
goto trigger_cleanup;
}
if( sqlite3AuthCheck(pParse, SQLITE_INSERT, SCHEMA_TABLE(iTabDb),0,zDb)){
goto trigger_cleanup;
}
}
#endif
            ///
            ///<summary>
            ///INSTEAD OF triggers can only appear on views and BEFORE triggers
            ///cannot appear on views.  So we might as well translate every
            ///INSTEAD OF trigger into a BEFORE trigger.  It simplifies code
            ///elsewhere.
            ///</summary>
            if (tr_tm == TokenType.TK_INSTEAD)
            {
                tr_tm = TokenType.TK_BEFORE;
            }
            ///
            ///<summary>
            ///Build the Trigger object 
            ///</summary>
            pTrigger = new Trigger();
            // (Trigger*)sqlite3DbMallocZero( db, sizeof(Trigger ))
            if (pTrigger == null)
                goto trigger_cleanup;
            pTrigger.zName = zName;
            pTrigger.table = pTableName.a[0].zName;
            // sqlite3DbStrDup( db, pTableName.a[0].zName );
            pTrigger.pSchema = db.Backends[iDb].pSchema;
            pTrigger.pTabSchema = pTab.pSchema;
            pTrigger.op = (u8)op;
            pTrigger.tr_tm = tr_tm == TokenType.TK_BEFORE ? TriggerType.TRIGGER_BEFORE : TriggerType.TRIGGER_AFTER;
            pTrigger.pWhen = exprc.Duplicate(db, pWhen, Sqlite3.EXPRDUP_REDUCE);
            pTrigger.pColumns = exprc.sqlite3IdListDup(db, pColumns);
            Debug.Assert(pParse.pNewTrigger == null);
            pParse.pNewTrigger = pTrigger;
        trigger_cleanup:
            db.DbFree(ref zName);
            build.sqlite3SrcListDelete(db, ref pTableName);
            build.sqlite3IdListDelete(db, ref pColumns);
            exprc.Delete(db, ref pWhen);
            if (pParse.pNewTrigger == null)
            {
                sqlite3DeleteTrigger(db, ref pTrigger);
            }
            else
            {
                Debug.Assert(pParse.pNewTrigger == pTrigger);
            }
        }

        ///<summary>
        /// This routine is called after all of the trigger actions have been parsed
        /// in order to complete the process of building the trigger.
        ///
        ///</summary>
        public static void sqlite3FinishTrigger(
            ///Parser context 
            ParseState pParse,

            ///The triggered program 
            TriggerStep pStepList,///

            ///Token that describes the complete CREATE TRIGGER 
            Token pAll///
        )
        {
            Trigger pTrig = pParse.pNewTrigger;
            ///Trigger being finished 
            
            Connection db = pParse.db;
            ///The database 
            DbFixer sFix = new DbFixer();
            ///Fixer object 
            Token nameToken = new Token();
            ///Trigger name for error reporting 
            pParse.pNewTrigger = null;
            if (Sqlite3.NEVER(pParse.nErr != 0) || pTrig == null)
                goto triggerfinish_cleanup;

            var zName = pTrig.zName;
            ///Name of trigger 
            var iDb = pParse.db.indexOfBackendWithSchema( pTrig.pSchema);
            ///Database containing the trigger 


            pTrig.step_list = pStepList;

            pStepList.linkedList()
                .ForEach(step=>step.pTrig=pTrig);
            
            nameToken.zRestSql = pTrig.zName;
            nameToken.Length = StringExtensions.Strlen30(nameToken.zRestSql);
            if (sFix.sqlite3FixInit(pParse, iDb, "trigger", nameToken) != 0 && sFix.sqlite3FixTriggerStep(pTrig.step_list) != 0)
            {
                goto triggerfinish_cleanup;
            }
            ///if we are not initializing,
            ///build the sqlite_master entry
            if (0 == db.init.busy)
            {
                Vdbe v;
                string z;
                ///Make an entry in the sqlite_master table 
                v = pParse.sqlite3GetVdbe();
                if (v == null)
                    goto triggerfinish_cleanup;
                build.sqlite3BeginWriteOperation(pParse, 0, iDb);
                z = pAll.zRestSql.Substring(0, pAll.Length);
                //sqlite3DbStrNDup( db, (char*)pAll.z, pAll.n );
                build.sqlite3NestedParse(pParse, "INSERT INTO %Q.%s VALUES('trigger',%Q,%Q,0,'CREATE TRIGGER %q')", db.Backends[iDb].Name, sqliteinth.SCHEMA_TABLE(iDb), zName, pTrig.table, z);
                db.DbFree(ref z);
                build.codegenChangeCookie(pParse, iDb);
                v.codegenAddParseSchemaOp(iDb, io.sqlite3MPrintf(db, "type='trigger' AND name='%q'", zName));
            }
            if (db.init.busy != 0)
            {
                Trigger pLink = pTrig;
                Hash pHash = db.Backends[iDb].pSchema.Triggers;
                Debug.Assert(Sqlite3.sqlite3SchemaMutexHeld(db, iDb, null));
                pTrig = HashExtensions.Insert(pHash, zName, zName.Strlen30(), pTrig);
                if (pTrig != null)
                {
                    //db.mallocFailed = 1;
                }
                else
                    if (pLink.pSchema == pLink.pTabSchema)
                    {
                        Table pTab;
                        int n = pLink.table.Strlen30();
                        pTab = pLink.pTabSchema.Tables.Find(pLink.table, n, (Table)null);
                        Debug.Assert(pTab != null);
                        pLink.pNext = pTab.pTrigger;
                        pTab.pTrigger = pLink;
                    }
            }
        triggerfinish_cleanup:
            sqlite3DeleteTrigger(db, ref pTrig);
            Debug.Assert(pParse.pNewTrigger == null);
            sqlite3DeleteTriggerStep(db, ref pStepList);
        }


        ///<summary>
        /// Turn a SELECT statement (that the pSelect parameter points to) into
        /// a trigger step.  Return a pointer to a TriggerStep structure.
        ///
        /// The parser calls this routine when it finds a SELECT statement in
        /// body of a TRIGGER.
        ///
        ///</summary>
        public static TriggerStep sqlite3TriggerSelectStep(Connection db, Select pSelect)
        {
            TriggerStep pTriggerStep = new TriggerStep() {
                Operator = TokenType.TK_SELECT,
                pSelect = pSelect,
                orconf = OnConstraintError.OE_Default
            };
            
            if (null == pTriggerStep)
            {
                SelectMethods.SelectDestructor(db, ref pSelect);
                return null;
            }
            return pTriggerStep;
        }
        ///
        ///<summary>
        ///Allocate space to hold a new trigger step.  The allocated space
        ///holds both the TriggerStep object and the TriggerStep.target.z string.
        ///
        ///If an OOM error occurs, NULL is returned and db.mallocFailed is set.
        ///
        ///</summary>
        static TriggerStep triggerStepAllocate(
            Connection db,///Database connection 
            TokenType op,///Trigger opcode 
            Token pName///The target name 
        )
        {
            return new TriggerStep()
            {
                Operator = op,
                target = pName.Copy()
            };            
        }
        ///<summary>
        /// Build a trigger step out of an INSERT statement.  Return a pointer
        /// to the new trigger step.
        ///
        /// The parser calls this routine when it sees an INSERT inside the
        /// body of a trigger.
        ///
        ///</summary>
        // OVERLOADS, so I don't need to rewrite parse.c
        public static TriggerStep sqlite3TriggerInsertStep(Connection db, Token pTableName, IdList pColumn, int null_4, int null_5, OnConstraintError orconf)
        {
            return sqlite3TriggerInsertStep(db, pTableName, pColumn, null, null, orconf);
        }
        public static TriggerStep sqlite3TriggerInsertStep(Connection db, Token pTableName, IdList pColumn, ExprList pEList, int null_5, OnConstraintError orconf)
        {
            return sqlite3TriggerInsertStep(db, pTableName, pColumn, pEList, null, orconf);
        }
        public static TriggerStep sqlite3TriggerInsertStep(Connection db, Token pTableName, IdList pColumn, int null_4, Select pSelect, OnConstraintError orconf)
        {
            return sqlite3TriggerInsertStep(db, pTableName, pColumn, null, pSelect, orconf);
        }
        public static TriggerStep sqlite3TriggerInsertStep(
            Connection db,///The database connection             
            Token pTableName,///Name of the table into which we insert 
            IdList pColumn,///List of columns in pTableName to insert into 
            ExprList pEList,///The VALUE clause: a list of values to be inserted 
            Select pSelect,///A SELECT statement that supplies values 
            OnConstraintError orconf///The conflict algorithm (OnConstraintError.OE_Abort, OnConstraintError.OE_Replace, etc.) 
        )
        {
            TriggerStep pTriggerStep;
            Debug.Assert(pEList == null || pSelect == null);
            Debug.Assert(pEList != null || pSelect != null///
                ///<summary>
                ///|| db.mallocFailed != 0 
                ///</summary>
            );
            pTriggerStep = triggerStepAllocate(db, TokenType.TK_INSERT, pTableName);
            //if ( pTriggerStep != null )
            //{
            pTriggerStep.pSelect = exprc.Clone(db, pSelect, Sqlite3.EXPRDUP_REDUCE);
            pTriggerStep.pIdList = pColumn;
            pTriggerStep.pExprList = exprc.Duplicate(db, pEList, Sqlite3.EXPRDUP_REDUCE);
            pTriggerStep.orconf = orconf;
            //}
            //else
            //{
            //  build.sqlite3IdListDelete( db, ref pColumn );
            //}
            exprc.Delete(db, ref pEList);
            SelectMethods.SelectDestructor(db, ref pSelect);
            return pTriggerStep;
        }
        ///<summary>
        /// Construct a trigger step that implements an UPDATE statement and return
        /// a pointer to that trigger step.  The parser calls this routine when it
        /// sees an UPDATE statement inside the body of a CREATE TRIGGER.
        ///
        ///</summary>
        public static TriggerStep sqlite3TriggerUpdateStep(Connection db,///
            ///<summary>
            ///The database connection 
            ///</summary>
        Token pTableName,///
            ///<summary>
            ///Name of the table to be updated 
            ///</summary>
        ExprList pEList,///
            ///<summary>
            ///The SET clause: list of column and new values 
            ///</summary>
        Expr pWhere,///
            ///<summary>
            ///The WHERE clause 
            ///</summary>
        OnConstraintError orconf///
            ///<summary>
            ///The conflict algorithm. (OnConstraintError.OE_Abort, OnConstraintError.OE_Ignore, etc) 
            ///</summary>
        )
        {
            TriggerStep pTriggerStep;
            pTriggerStep = triggerStepAllocate(db, TokenType.TK_UPDATE, pTableName);
            //if ( pTriggerStep != null )
            //{
            pTriggerStep.pExprList = exprc.Duplicate(db, pEList, Sqlite3.EXPRDUP_REDUCE);
            pTriggerStep.pWhere = exprc.Duplicate(db, pWhere, Sqlite3.EXPRDUP_REDUCE);
            pTriggerStep.orconf = orconf;
            //}
            exprc.Delete(db, ref pEList);
            exprc.Delete(db, ref pWhere);
            return pTriggerStep;
        }
        ///<summary>
        /// Construct a trigger step that implements a DELETE statement and return
        /// a pointer to that trigger step.  The parser calls this routine when it
        /// sees a DELETE statement inside the body of a CREATE TRIGGER.
        ///
        ///</summary>
        public static TriggerStep sqlite3TriggerDeleteStep(Connection db,///
            ///<summary>
            ///Database connection 
            ///</summary>
        Token pTableName,///
            ///<summary>
            ///The table from which rows are deleted 
            ///</summary>
        Expr pWhere///
            ///<summary>
            ///The WHERE clause 
            ///</summary>
        )
        {
            TriggerStep pTriggerStep;
            pTriggerStep = triggerStepAllocate(db, TokenType.TK_DELETE, pTableName);
            //if ( pTriggerStep != null )
            //{
            pTriggerStep.pWhere = exprc.Duplicate(db, pWhere, Sqlite3.EXPRDUP_REDUCE);
            pTriggerStep.orconf = OnConstraintError.OE_Default;
            //}
            exprc.Delete(db, ref pWhere);
            return pTriggerStep;
        }
        ///<summary>
        /// Recursively delete a Trigger structure
        ///
        ///</summary>
        public static void sqlite3DeleteTrigger(Connection db, ref Trigger pTrigger)
        {
            if (pTrigger == null)
                return;
            sqlite3DeleteTriggerStep(db, ref pTrigger.step_list);
            db.DbFree(ref pTrigger.zName);
            db.DbFree(ref pTrigger.table);
            exprc.Delete(db, ref pTrigger.pWhen);
            build.sqlite3IdListDelete(db, ref pTrigger.pColumns);
            pTrigger = null;
            db.DbFree(ref pTrigger);
        }
        ///<summary>
        /// This function is called to drop a trigger from the database schema.
        ///
        /// This may be called directly from the parser and therefore identifies
        /// the trigger by name.  The sqlite3DropTriggerPtr() routine does the
        /// same job as this routine except it takes a pointer to the trigger
        /// instead of the trigger name.
        ///
        ///</summary>
        public static void sqlite3DropTrigger(ParseState pParse, SrcList pName, int noErr)
        {
            Trigger pTrigger = null;
            int i;
            string zDb;
            string zName;
            int nName;
            Connection db = pParse.db;
            //      if ( db.mallocFailed != 0 ) goto drop_trigger_cleanup;
            if (SqlResult.SQLITE_OK != pParse.sqlite3ReadSchema())
            {
                goto drop_trigger_cleanup;
            }
            Debug.Assert(pName.Count == 1);
            zDb = pName.a[0].zDatabase;
            zName = pName.a[0].zName;
            nName = StringExtensions.Strlen30(zName);
            Debug.Assert(zDb != null || Sqlite3.sqlite3BtreeHoldsAllMutexes(db));
            for (i = sqliteinth.OMIT_TEMPDB; i < db.BackendCount; i++)
            {
                int j = (i < 2) ? i ^ 1 : i;
                ///
                ///<summary>
                ///Search TEMP before MAIN 
                ///</summary>
                if (zDb != null && !db.Backends[j].Name.Equals(zDb, StringComparison.InvariantCultureIgnoreCase))
                    continue;
                Debug.Assert(Sqlite3.sqlite3SchemaMutexHeld(db, j, null));
                pTrigger = (db.Backends[j].pSchema.Triggers).Find(zName, nName, (Trigger)null);
                if (pTrigger != null)
                    break;
            }
            if (pTrigger == null)
            {
                if (noErr == 0)
                {
                    utilc.sqlite3ErrorMsg(pParse, "no such trigger: %S", pName, 0);
                }
                else
                {
                    build.sqlite3CodeVerifyNamedSchema(pParse, zDb);
                }
                pParse.checkSchema = 1;
                goto drop_trigger_cleanup;
            }
            sqlite3DropTriggerPtr(pParse, pTrigger);
        drop_trigger_cleanup:
            build.sqlite3SrcListDelete(db, ref pName);
        }
        ///<summary>
        /// Return a pointer to the Table structure for the table that a trigger
        /// is set on.
        ///
        ///</summary>
        public static Table tableOfTrigger(this Trigger pTrigger)
        {
            int n = StringExtensions.Strlen30(pTrigger.table);
            return pTrigger.pTabSchema.Tables.Find(pTrigger.table, n, (Table)null);
        }
        ///<summary>
        /// Drop a trigger given a pointer to that trigger.
        ///
        ///</summary>
        public static void sqlite3DropTriggerPtr(ParseState pParse, Trigger pTrigger)
        {
            Vdbe v;
            Connection db = pParse.db;
            var iDb = pParse.db.indexOfBackendWithSchema( pTrigger.pSchema);
            Debug.Assert(iDb >= 0 && iDb < db.BackendCount);
            var pTable = tableOfTrigger(pTrigger);
            Debug.Assert(pTable != null);
            Debug.Assert(pTable.pSchema == pTrigger.pSchema || iDb == 1);
#if !SQLITE_OMIT_AUTHORIZATION
																																																																								{
int code = SQLITE_DROP_TRIGGER;
string zDb = db.aDb[iDb].zName;
string zTab = SCHEMA_TABLE(iDb);
if( iDb==1 ) code = SQLITE_DROP_TEMP_TRIGGER;
if( sqlite3AuthCheck(pParse, code, pTrigger.name, pTable.zName, zDb) ||
sqlite3AuthCheck(pParse, SQLITE_DELETE, zTab, 0, zDb) ){
return;
}
}
#endif
            ///Generate code to destroy the database record of the trigger.
            Debug.Assert(pTable != null);
            if ((v = pParse.sqlite3GetVdbe()) != null)
            {
                int _base;
                VdbeOpList[] dropTrigger = new VdbeOpList[] {
					new VdbeOpList(OpCode.OP_Rewind,0,Sqlite3.ADDR(9),0),
					new VdbeOpList(OpCode.OP_String8,0,1,0),
					///1 
					new VdbeOpList(OpCode.OP_Column,0,1,2),
					new VdbeOpList(OpCode.OP_Ne,2,Sqlite3.ADDR(8),1),
					new VdbeOpList(OpCode.OP_String8,0,1,0),
					///4: "trigger" 
					new VdbeOpList(OpCode.OP_Column,0,0,2),
					new VdbeOpList(OpCode.OP_Ne,2,Sqlite3.ADDR(8),1),
					new VdbeOpList(OpCode.OP_Delete,0,0,0),
					new VdbeOpList(OpCode.OP_Next,0,Sqlite3.ADDR(1),0),
				///8 
				};
                build.sqlite3BeginWriteOperation(pParse, 0, iDb);
                build.sqlite3OpenMasterTable(pParse, iDb);
                _base = v.sqlite3VdbeAddOpList(dropTrigger.Length, dropTrigger);
                v.sqlite3VdbeChangeP4(_base + 1, pTrigger.zName, P4Usage.P4_TRANSIENT);
                v.sqlite3VdbeChangeP4(_base + 4, "trigger", P4Usage.P4_STATIC);
                build.codegenChangeCookie(pParse, iDb);
                v.sqlite3VdbeAddOp2(OpCode.OP_Close, 0, 0);
                v.sqlite3VdbeAddOp4(OpCode.OP_DropTrigger, iDb, 0, 0, pTrigger.zName, 0);
                if (pParse.UsedCellCount < 3)
                {
                    pParse.UsedCellCount = 3;
                }
            }
        }
        ///<summary>
        /// Remove a trigger from the hash tables of the sqlite* pointer.
        ///
        ///</summary>
        public static void sqlite3UnlinkAndDeleteTrigger(Connection db, int iDb, string zName)//OPCODE:OP_DropTrigger
        {
            Debug.Assert(Sqlite3.sqlite3SchemaMutexHeld(db, iDb, null));
            var pHash = (db.Backends[iDb].pSchema.Triggers);
            var pTrigger = pHash.Insert(  zName, StringExtensions.Strlen30(zName), (Trigger)null);
            if (Sqlite3.ALWAYS(pTrigger != null))
            {
                if (pTrigger.pSchema == pTrigger.pTabSchema)
                {
                    Table pTab = tableOfTrigger(pTrigger);
                    //Trigger** pp;
                    //for ( pp = &pTab.pTrigger ; *pp != pTrigger ; pp = &( (*pp).pNext ) ) ;
                    //*pp = (*pp).pNext;
                    if (pTab.pTrigger == pTrigger)
                    {
                        pTab.pTrigger = pTrigger.pNext;
                    }
                    else
                    {
                        Trigger cc = pTab.pTrigger;
                        while (cc != null)
                        {
                            if (cc.pNext == pTrigger)
                            {
                                cc.pNext = cc.pNext.pNext;
                                break;
                            }
                            cc = cc.pNext;
                        }
                        Debug.Assert(cc != null);
                    }
                }
                sqlite3DeleteTrigger(db, ref pTrigger);
                db.flags |= SqliteFlags.SQLITE_InternChanges;
            }
        }
        ///<summary>
        /// pEList is the SET clause of an UPDATE statement.  Each entry
        /// in pEList is of the format <id>=<expr>.  If any of the entries
        /// in pEList have an <id> which matches an identifier in pIdList,
        /// then return TRUE.  If pIdList==NULL, then it is considered a
        /// wildcard that matches anything.  Likewise if pEList==NULL then
        /// it matches anything so always return true.  Return false only
        /// if there is no match.
        ///
        ///</summary>
        public static int checkColumnOverlap(IdList pIdList, ExprList pEList)
        {
            int e;
            if (pIdList == null || Sqlite3.NEVER(pEList == null))
                return 1;
            for (e = 0; e < pEList.Count; e++)
            {
                if (build.sqlite3IdListIndex(pIdList, pEList.a[e].zName) >= 0)
                    return 1;
            }
            return 0;
        }
        ///<summary>
        /// Return a list of all triggers on table pTab if there exists at least
        /// one trigger that must be fired when an operation of type 'op' is
        /// performed on the table, and, if that operation is an UPDATE, if at
        /// least one of the columns in pChanges is being modified.
        ///
        ///</summary>
        public static Trigger sqlite3TriggersExist(ParseState pParse,
            Table pTab,///The table the contains the triggers 
            TokenType op,///one of TokenType.TK_DELETE, TokenType.TK_INSERT, TokenType.TK_UPDATE 
            ExprList pChanges,///Columns that change in an UPDATE statement 
            out TriggerType pMask///OUT: Mask of TriggerType.TRIGGER_BEFORE|TriggerType.TRIGGER_AFTER 
        )
        {
            TriggerType mask = 0;
            Trigger pList = null;
            
            if ((pParse.db.flags & SqliteFlags.SQLITE_EnableTrigger) != 0)
            {
                pList = sqlite3TriggerList(pTab, pParse);
            }
            Debug.Assert(pList == null || pTab.IsVirtual() == false);
            pList.linkedList()
                .Where(p=> p.Operator == op && checkColumnOverlap(p.pColumns, pChanges) != 0)
                .ForEach(p => mask |= p.tr_tm);
            
            //if ( pMask != 0 )
            {
                pMask = mask;
            }
            return (mask != 0 ? pList : null);
        }

        ///<summary>
        /// Convert the pStep.target token into a SrcList and return a pointer
        /// to that SrcList.
        ///
        /// This routine adds a specific database name, if needed, to the target when
        /// forming the SrcList.  This prevents a trigger in one database from
        /// referring to a target in another database.  An exception is when the
        /// trigger is in TEMP in which case it can refer to any other database it
        /// wants.
        ///
        ///</summary>
        public static SrcList targetSrcList(
            ParseState pParse,///The parsing context 
            TriggerStep pStep///The trigger containing the target token 
        )
        {            
            var pSrc = build.sqlite3SrcListAppend(pParse.db, 0, pStep.target, 0);///SrcList to be returned 
            //if ( pSrc != null )
            //{
            Debug.Assert(pSrc.Count > 0);
            Debug.Assert(pSrc.a != null);
            var iDb = pParse.db.indexOfBackendWithSchema( pStep.pTrig.pSchema);///Index of the database to use 
            if (iDb == 0 || iDb >= 2)//=> iDb!=1
            {
                Connection db = pParse.db;
                Debug.Assert(iDb < pParse.db.BackendCount);
                pSrc.Last().zDatabase = db.Backends[iDb].Name;
                // sqlite3DbStrDup( db, db.aDb[iDb].zName );
            }
            //}
            return pSrc;
        }
        
#if SQLITE_DEBUG
																																																    /*
** This function is used to add VdbeComment() annotations to a VDBE
** program. It is not used in production code, only for debugging.
*/
    static string onErrorText( int onError )
    {
      switch ( onError )
      {
        case OnConstraintError.OE_Abort:
          return "abort";
        case OnConstraintError.OE_Rollback:
          return "rollback";
        case OnConstraintError.OE_Fail:
          return "fail";
        case OnConstraintError.OE_Replace:
          return "replace";
        case OnConstraintError.OE_Ignore:
          return "ignore";
        case OnConstraintError.OE_Default:
          return "default";
      }
      return "n/a";
    }
#endif
        ///<summary>
        /// Parse context structure pFrom has just been used to create a sub-vdbe
        /// (trigger program). If an error has occurred, transfer error information
        /// from pFrom to pTo.
        ///</summary>
        public static void transferParseError(ParseState pTo, ParseState pFrom)
        {
            Debug.Assert(String.IsNullOrEmpty(pFrom.zErrMsg) || pFrom.nErr != 0);
            Debug.Assert(String.IsNullOrEmpty(pTo.zErrMsg) || pTo.nErr != 0);
            if (pTo.nErr == 0)
            {
                pTo.zErrMsg = pFrom.zErrMsg;
                pTo.nErr = pFrom.nErr;
            }
            else
            {
                pFrom.db.DbFree(ref pFrom.zErrMsg);
            }
        }

        ///<summary>
        /// Return a pointer to a TriggerPrg object containing the sub-program for
        /// trigger pTrigger with default ON CONFLICT algorithm orconf. If no such
        /// TriggerPrg object exists, a new object is allocated and populated before
        /// being returned.
        ///</summary>
        public static TriggerPrg getRowTrigger(
            ParseState pParse,///Current parse context 
            Trigger pTrigger,///Trigger to code 
            Table pTab,///The table trigger pTrigger is attached to 
            OnConstraintError orconf///ON CONFLICT algorithm. 
        )
        {
            ParseState pRoot = sqliteinth.sqlite3ParseToplevel(pParse);
            
            Debug.Assert(pTrigger.zName == null || pTab == pTrigger.tableOfTrigger());
            ///It may be that this trigger has already been coded (or is in the
            ///process of being coded). If this is the case, then an entry with
            ///a matching TriggerPrg.pTrigger field will be present somewhere
            ///in the Parse.pTriggerPrg list. Search for such an entry.  
            var pPrg = pRoot.pTriggerPrg.linkedList()
                .FirstOrDefault(p => !(p.pTrigger != pTrigger || p.orconf != orconf));
            
            ///If an existing TriggerPrg could not be located, create a new one. 
            if (null == pPrg)
            {
                pPrg = codeRowTrigger(pParse, pTrigger, pTab, orconf);
            }
            return pPrg;
        }


        ///<summary>
        /// Create and populate a new TriggerPrg object with a sub-program
        /// implementing trigger pTrigger with ON CONFLICT policy orconf.
        ///
        ///</summary>
        public static TriggerPrg codeRowTrigger(
            ParseState pParse,///Current parse context 
            Trigger pTrigger,///Trigger to code 
            Table pTab,///The table pTrigger is attached to 
            OnConstraintError orconf///ON CONFLICT policy to code trigger program with 
        )
        {
            ParseState pTop = sqliteinth.sqlite3ParseToplevel(pParse);
            Connection db = pParse.db;///Database handle 
            Expr pWhen = null;///Duplicate of trigger WHEN expression 
            
            int iEndTrigger = 0;
            ///Label to jump to if WHEN is false 
            Debug.Assert(pTrigger.zName == null || pTab == tableOfTrigger(pTrigger));
            Debug.Assert(pTop.pVdbe != null);
            ///Allocate the TriggerPrg and SubProgram objects. To ensure that they
            ///are freed if an error occurs, link them into the Parse.pTriggerPrg 
            ///list of the top-level Parse object sooner rather than later.
            var pPrg = pTop.pTriggerPrg = new TriggerPrg()
            {
                // sqlite3DbMallocZero( db, sizeof( TriggerPrg ) );
                //if ( null == pPrg ) return 0;
                pNext = pTop.pTriggerPrg,
                pTrigger = pTrigger,
                orconf = orconf,
                aColmask =new u32[2] { 0xffffffff, 0xffffffff }
            };

            ///<param name="Sub">vdbe for trigger program </param>
            SubProgram pProgram = pPrg.pProgram  = new SubProgram();
            // sqlite3DbMallocZero( db, sizeof( SubProgram ) );
            //if( null==pProgram ) return 0;
            pTop.pVdbe.sqlite3VdbeLinkSubProgram(pProgram);

            ///Allocate and populate a new Parse context to use for coding the 
            ///<param name="trigger sub">program.  </param>
            var pSubParse = new ParseState()///<param name="Parse context for sub">vdbe </param>
            {
                db = db,
                pTriggerTab = pTab,
                pToplevel = pTop,
                zAuthContext = pTrigger.zName,
                eTriggerOp = pTrigger.op,
                nQueryLoop = pParse.nQueryLoop
            };
            // sqlite3StackAllocZero( db, sizeof( Parse ) );
            //if ( null == pSubParse ) return null;
            var sNC = new NameContext() { ParseState = pSubParse };///<param name="Name context for sub">vdbe </param>
            // memset( &sNC, 0, sizeof( sNC ) );


            var v = pSubParse.sqlite3GetVdbe();///Temporary VM 
            if (v != null)
            {
#if SQLITE_DEBUG
																																																																																																        VdbeComment( v, "Start: %s.%s (%s %s%s%s ON %s)",
          pTrigger.zName != null ? pTrigger.zName : "", onErrorText( orconf ),
          ( pTrigger.tr_tm == TriggerType.TRIGGER_BEFORE ? "BEFORE" : "AFTER" ),
            ( pTrigger.op == TokenType.TK_UPDATE ? "UPDATE" : "" ),
            ( pTrigger.op == TokenType.TK_INSERT ? "INSERT" : "" ),
            ( pTrigger.op == TokenType.TK_DELETE ? "DELETE" : "" ),
          pTab.zName
        );
#endif
#if !SQLITE_OMIT_TRACE
                v.sqlite3VdbeChangeP4(-1, io.sqlite3MPrintf(db, "-- TRIGGER %s", pTrigger.zName), P4Usage.P4_DYNAMIC);
#endif
                ///If one was specified, code the WHEN clause. If it evaluates to false
                ///<param name="(or NULL) the sub">vdbe is immediately halted by jumping to the </param>
                ///<param name="OP_Halt inserted at the end of the program.  ">OP_Halt inserted at the end of the program.  </param>
                if (pTrigger.pWhen != null)
                {
                    pWhen = exprc.Duplicate(db, pTrigger.pWhen, 0);
                    if (SqlResult.SQLITE_OK == Sqlite3.ResolveExtensions.ResolveExprNames(sNC, ref pWhen)//&& db.mallocFailed==0 
                    )
                    {
                        iEndTrigger = v.sqlite3VdbeMakeLabel();
                        pSubParse.sqlite3ExprIfFalse(pWhen, iEndTrigger, sqliteinth.SQLITE_JUMPIFNULL);
                    }
                    exprc.Delete(db, ref pWhen);
                }
                ///<param name="Code the trigger program into the sub">vdbe. </param>
                TriggerBuilder.codeTriggerProgram(pSubParse, pTrigger.step_list, orconf);
                ///<param name="Insert an  OpCode.OP_Halt at the end of the sub">program. </param>
                if (iEndTrigger != 0)
                {
                    v.sqlite3VdbeResolveLabel(iEndTrigger);
                }
                v.sqlite3VdbeAddOp0(OpCode.OP_Halt);
#if SQLITE_DEBUG
																																																																																																        VdbeComment( v, "End: %s.%s", pTrigger.zName, onErrorText( orconf ) );
#endif
                transferParseError(pParse, pSubParse);
                //if( db.mallocFailed==0 ){
                pProgram.aOp = v.sqlite3VdbeTakeOpArray(ref pProgram.nOp, ref pTop.nMaxArg);
                //}
                pProgram.nMem = pSubParse.UsedCellCount;
                pProgram.nCsr = pSubParse.AllocatedCursorCount;
                pProgram.token = pTrigger.GetHashCode();
                pPrg.aColmask[0] = pSubParse.oldmask;
                pPrg.aColmask[1] = pSubParse.newmask;
                Engine.vdbeaux.sqlite3VdbeDelete(ref v);
            }
            Debug.Assert(null == pSubParse.pAinc && null == pSubParse.pZombieTab);
            Debug.Assert(null == pSubParse.pTriggerPrg && 0 == pSubParse.nMaxArg);
            //sqlite3StackFree(db, pSubParse);
            return pPrg;
        }
        
        
        
        ///
        ///<summary>
        ///</summary>
        ///<param name="Triggers may access values stored in the old.* or new.* pseudo">table. </param>
        ///<param name="This function returns a 32">bit bitmask indicating which columns of the </param>
        ///<param name="old.* or new.* tables actually are used by triggers. This information ">old.* or new.* tables actually are used by triggers. This information </param>
        ///<param name="may be used by the caller, for example, to avoid having to load the entire">may be used by the caller, for example, to avoid having to load the entire</param>
        ///<param name="old.* record into memory when executing an UPDATE or DELETE command.">old.* record into memory when executing an UPDATE or DELETE command.</param>
        ///<param name=""></param>
        ///<param name="Bit 0 of the returned mask is set if the left">most column of the</param>
        ///<param name="table may be accessed using an [old|new].<col> reference. Bit 1 is set if">table may be accessed using an [old|new].<col> reference. Bit 1 is set if</param>
        ///<param name="the second leftmost column value is required, and so on. If there">the second leftmost column value is required, and so on. If there</param>
        ///<param name="are more than 32 columns in the table, and at least one of the columns">are more than 32 columns in the table, and at least one of the columns</param>
        ///<param name="with an index greater than 32 may be accessed, 0xffffffff is returned.">with an index greater than 32 may be accessed, 0xffffffff is returned.</param>
        ///<param name=""></param>
        ///<param name="It is not possible to determine if the old.rowid or new.rowid column is ">It is not possible to determine if the old.rowid or new.rowid column is </param>
        ///<param name="accessed by triggers. The caller must always assume that it is.">accessed by triggers. The caller must always assume that it is.</param>
        ///<param name=""></param>
        ///<param name="Parameter isNew must be either 1 or 0. If it is 0, then the mask returned">Parameter isNew must be either 1 or 0. If it is 0, then the mask returned</param>
        ///<param name="applies to the old.* table. If 1, the new.* table.">applies to the old.* table. If 1, the new.* table.</param>
        ///<param name=""></param>
        ///<param name="Parameter tr_tm must be a mask with one or both of the TriggerType.TRIGGER_BEFORE">Parameter tr_tm must be a mask with one or both of the TriggerType.TRIGGER_BEFORE</param>
        ///<param name="and TriggerType.TRIGGER_AFTER bits set. Values accessed by BEFORE triggers are only">and TriggerType.TRIGGER_AFTER bits set. Values accessed by BEFORE triggers are only</param>
        ///<param name="included in the returned mask if the TriggerType.TRIGGER_BEFORE bit is set in the">included in the returned mask if the TriggerType.TRIGGER_BEFORE bit is set in the</param>
        ///<param name="tr_tm parameter. Similarly, values accessed by AFTER triggers are only">tr_tm parameter. Similarly, values accessed by AFTER triggers are only</param>
        ///<param name="included in the returned mask if the TriggerType.TRIGGER_AFTER bit is set in tr_tm.">included in the returned mask if the TriggerType.TRIGGER_AFTER bit is set in tr_tm.</param>
        ///<param name=""></param>
        public static u32 sqlite3TriggerColmask(ParseState pParse,///Parse context 
            Trigger pTrigger,///List of triggers on table pTab 
            ExprList pChanges,///Changes list for any UPDATE OF triggers 
            int isNew,///1 for new.* ref mask, 0 for old.* ref mask 
            TriggerType tr_tm,///Mask of TriggerType.TRIGGER_BEFORE|TriggerType.TRIGGER_AFTER 
            Table pTab,///The table to code triggers from 
            OnConstraintError orconf///Default ON CONFLICT policy for trigger steps 
        )
        {
            var op = pChanges != null ? TokenType.TK_UPDATE : TokenType.TK_DELETE;
            u32 mask = 0;
            Debug.Assert(isNew == 1 || isNew == 0);

            pTrigger.linkedList().ForEach(
                    p => {
                        if (p.Operator == op && (tr_tm & p.tr_tm) != 0 && checkColumnOverlap(p.pColumns, pChanges) != 0)
                        {
                            var pPrg = getRowTrigger(pParse, p, pTab, orconf);
                            if (pPrg != null)
                                mask |= pPrg.aColmask[isNew];
                        }
                    }
            );
            
            return mask;
        }
#endif
    }

}
