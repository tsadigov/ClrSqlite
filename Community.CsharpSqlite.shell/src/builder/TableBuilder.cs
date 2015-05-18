using Community.CsharpSqlite;
using Community.CsharpSqlite.Ast;
using Community.CsharpSqlite.Parsing;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Parse = Community.CsharpSqlite.Sqlite3.Parse;
namespace Community.CsharpSqlite.builder
{
        public static class TableBuilder
        {

            ///<summary>
            /// Unlink the given table from the hash tables and the delete the
            /// table structure with all its indices and foreign keys.
            ///
            ///</summary>
            public static void sqlite3UnlinkAndDeleteTable(sqlite3 db, int iDb, string zTabName)//OPCODE:OP_DropTable
            {
                Table p;
                Db pDb;
                Debug.Assert(db != null);
                Debug.Assert(iDb >= 0 && iDb < db.nDb);
                Debug.Assert(zTabName != null);
                Debug.Assert(Sqlite3.sqlite3SchemaMutexHeld(db, iDb, null));
                sqliteinth.testcase(zTabName.Length == 0);
                ///
                ///<summary>
                ///</summary>
                ///<param name="Zero">length table names are allowed </param>
                pDb = db.aDb[iDb];
                p = HashExtensions.sqlite3HashInsert(ref pDb.pSchema.tblHash, zTabName, StringExtensions.sqlite3Strlen30(zTabName), (Table)null);
                TableBuilder.sqlite3DeleteTable(db, ref p);
                db.flags |= SqliteFlags.SQLITE_InternChanges;
            }


            ///<summary>
            /// Locate the in-memory structure that describes a particular database
            /// table given the name of that table and (optionally) the name of the
            /// database containing the table.  Return NULL if not found.
            ///
            /// If zDatabase is 0, all databases are searched for the table and the
            /// first matching table is returned.  (No checking for duplicate table
            /// names is done.)  The search order is TEMP first, then MAIN, then any
            /// auxiliary databases added using the ATTACH command.
            ///
            /// See also build.sqlite3LocateTable().
            ///
            ///</summary>
            public static Table sqlite3FindTable(sqlite3 db, string zName, string zDatabase)
            {
                Table p = null;
                Debug.Assert(zName != null);
                var nName = StringExtensions.sqlite3Strlen30(zName);
                ///All mutexes are required for schema access.  Make sure we hold them. 
                Debug.Assert(zDatabase != null || Sqlite3.sqlite3BtreeHoldsAllMutexes(db));
                for (int i = sqliteinth.OMIT_TEMPDB; i < db.nDb; i++)
                {
                    int j = (i < 2) ? i ^ 1 : i;
                    ///Search TEMP before MAIN 
                    if (zDatabase != null && !zDatabase.Equals(db.aDb[j].zName, StringComparison.InvariantCultureIgnoreCase))
                        continue;
                    Debug.Assert(Sqlite3.sqlite3SchemaMutexHeld(db, j, null));
                    p = db.aDb[j].pSchema.tblHash.sqlite3HashFind(zName, nName, (Table)null);
                    if (p != null)
                        break;
                }
                return p;
            }
            ///<summary>
            /// Locate the in-memory structure that describes a particular database
            /// table given the name of that table and (optionally) the name of the
            /// database containing the table.  Return NULL if not found.  Also leave an
            /// error message in pParse.zErrMsg.
            ///
            /// The difference between this routine and build.sqlite3FindTable() is that this
            /// routine leaves an error message in pParse.zErrMsg where
            /// build.sqlite3FindTable() does not.
            ///
            ///</summary>
            public static Table sqlite3LocateTable(Parse pParse,///
                ///context in which to report errors 
            int isView,///
                ///True if looking for a VIEW rather than a TABLE 
            string zName,///
                ///Name of the table we are looking for 
            string zDbase///
                ///Name of the database.  Might be NULL 
            )
            {
                ///Read the database schema. If an error occurs, leave an error message
                ///and code in pParse and return NULL. 
                if (SqlResult.SQLITE_OK != Sqlite3.sqlite3ReadSchema(pParse))
                {
                    return null;
                }
                Table p = TableBuilder.sqlite3FindTable(pParse.db, zName, zDbase);
                if (p == null)
                {
                    string zMsg = isView != 0 ? "no such view" : "no such table";
                    if (zDbase != null)
                    {
                        utilc.sqlite3ErrorMsg(pParse, "%s: %s.%s", zMsg, zDbase, zName);
                    }
                    else
                    {
                        utilc.sqlite3ErrorMsg(pParse, "%s: %s", zMsg, zName);
                    }
                    pParse.checkSchema = 1;
                }
                return p;
            }


            ///<summary>
            ///Begin constructing a new table representation in memory.  This is
            ///the first of several action routines that get called in response
            ///to a CREATE TABLE statement.  In particular, this routine is called
            ///after seeing tokens "CREATE" and "TABLE" and the table name. The isTemp
            ///flag is true if the table should be stored in the auxiliary database
            ///file instead of in the main database file.  This is normally the case
            ///when the "TEMP" or "TEMPORARY" keyword occurs in between
            ///CREATE and TABLE.
            ///
            ///The new table record is initialized and put in pParse.pNewTable.
            ///As more of the CREATE TABLE statement is parsed, additional action
            ///routines will be called to add more information to this record.
            ///At the end of the CREATE TABLE statement, the sqlite3EndTable() routine
            ///is called to complete the construction of the new table record.
            ///
            ///</summary>
            public static void sqlite3StartTable(Parse pParse,///
                ///Parser context 
            Token pName1,///
                ///First part of the name of the table or view 
            Token pName2,///
                ///Second part of the name of the table or view 
            int isTemp,///
                ///True if this is a TEMP table 
            int isView,///
                ///True if this is a VIEW 
            int isVirtual,///
                ///True if this is a VIRTUAL table 
            int noErr///
                ///Do nothing if table already exists 
            )
            {
                Table pTable;
                string zName = null;
                ///The name of the new table 
                sqlite3 db = pParse.db;
                int iDb;
                ///Database number to create the table in 
                Token pName = new Token();
                ///Unqualified name of the table to create 
                ///The table or view name to create is passed to this routine via tokens
                ///pName1 and pName2. If the table name was fully qualified, for example:
                ///
                ///CREATE TABLE xxx.yyy (...);
                ///
                ///Then pName1 is set to "xxx" and pName2 "yyy". On the other hand if
                ///the table name is not fully qualified, i.e.:
                ///
                ///CREATE TABLE yyy(...);
                ///
                ///Then pName1 is set to "yyy" and pName2 is "".
                ///
                ///The call below sets the pName pointer to point at the token (pName1 or
                ///pName2) that stores the unqualified table name. The variable iDb is
                ///set to the index of the database that the table or view is to be
                ///created in.
                iDb = build.sqlite3TwoPartName(pParse, pName1, pName2, ref pName);
                if (iDb < 0)
                    return;
                if (0 == sqliteinth.OMIT_TEMPDB && isTemp != 0 && pName2.Length > 0 && iDb != 1)
                {
                    ///If creating a temp table, the name may not be qualified. Unless 
                    ///the database name is "temp" anyway.  
                    utilc.sqlite3ErrorMsg(pParse, "temporary table name must be unqualified");
                    return;
                }
                if (sqliteinth.OMIT_TEMPDB == 0 && isTemp != 0)
                    iDb = 1;
                pParse.sNameToken = pName;
                zName = build.sqlite3NameFromToken(db, pName);
                if (zName == null)
                    return;
                if (SqlResult.SQLITE_OK != build.sqlite3CheckObjectName(pParse, zName))
                {
                    goto begin_table_error;
                }
                if (db.init.iDb == 1)
                    isTemp = 1;
#if !SQLITE_OMIT_AUTHORIZATION
																																																																																	Debug.Assert( (isTemp & 1)==isTemp );
{
int code;
string zDb = db.aDb[iDb].zName;
if( sqlite3AuthCheck(pParse, SQLITE_INSERT, SCHEMA_TABLE(isTemp), 0, zDb) ){
goto begin_table_error;
}
if( isView ){
if( OMIT_TEMPDB ==0&& isTemp ){
code = SQLITE_CREATE_TEMP_VIEW;
}else{
code = SQLITE_CREATE_VIEW;
}
}else{
if( OMIT_TEMPDB ==0&& isTemp ){
code = SQLITE_CREATE_TEMP_TABLE;
}else{
code = SQLITE_CREATE_TABLE;
}
}
if( null==isVirtual && sqlite3AuthCheck(pParse, code, zName, 0, zDb) ){
goto begin_table_error;
}
}
#endif
                ///
                ///<summary>
                ///Make sure the new table name does not collide with an existing
                ///index or table name in the same database.  Issue an error message if
                ///it does. The exception is if the statement being parsed was passed
                ///to an sqlite3_declare_vtab() call. In that case only the column names
                ///and types will be used, so there is no need to test for namespace
                ///collisions.
                ///</summary>
                if (!sqliteinth.IN_DECLARE_VTAB(pParse))
                {
                    String zDb = db.aDb[iDb].zName;
                    if (SqlResult.SQLITE_OK != Sqlite3.sqlite3ReadSchema(pParse))
                    {
                        goto begin_table_error;
                    }
                    pTable = TableBuilder.sqlite3FindTable(db, zName, zDb);
                    if (pTable != null)
                    {
                        if (noErr == 0)
                        {
                            utilc.sqlite3ErrorMsg(pParse, "table %T already exists", pName);
                        }
                        else
                        {
                            Debug.Assert(0 == db.init.busy);
                            build.sqlite3CodeVerifySchema(pParse, iDb);
                        }
                        goto begin_table_error;
                    }
                    if (IndexBuilder.sqlite3FindIndex(db, zName, zDb) != null)
                    {
                        utilc.sqlite3ErrorMsg(pParse, "there is already an index named %s", zName);
                        goto begin_table_error;
                    }
                }
                pTable = new Table();
                // sqlite3DbMallocZero(db, Table).Length;
                //if ( pTable == null )
                //{
                //  db.mallocFailed = 1;
                //  pParse.rc = SQLITE_NOMEM;
                //  pParse.nErr++;
                //  goto begin_table_error;
                //}
                pTable.zName = zName;
                pTable.iPKey = -1;
                pTable.pSchema = db.aDb[iDb].pSchema;
                pTable.nRef = 1;
                pTable.nRowEst = 1000000;
                Debug.Assert(pParse.pNewTable == null);
                pParse.pNewTable = pTable;
                ///
                ///<summary>
                ///If this is the magic sqlite_sequence table used by autoincrement,
                ///then record a pointer to this table in the main database structure
                ///so that INSERT can find the table easily.
                ///
                ///</summary>
#if !SQLITE_OMIT_AUTOINCREMENT
                if (pParse.nested == 0 && zName == "sqlite_sequence")
                {
                    Debug.Assert(Sqlite3.sqlite3SchemaMutexHeld(db, iDb, null));
                    pTable.pSchema.pSeqTab = pTable;
                }
#endif
                ///
                ///<summary>
                ///Begin generating the code that will insert the table record into
                ///the SQLITE_MASTER table.  Note in particular that we must go ahead
                ///and allocate the record number for the table entry now.  Before any
                ///PRIMARY KEY or UNIQUE keywords are parsed.  Those keywords will cause
                ///indices to be created and the table record must come before the
                ///indices.  Hence, the record number for the table must be allocated
                ///now.
                ///</summary>
                var v = pParse.sqlite3GetVdbe();
                if (0 == db.init.busy && v != null)
                {
                    int j1;
                    int fileFormat;
                    int reg1, reg2, reg3;
                    build.sqlite3BeginWriteOperation(pParse, 0, iDb);
                    if (isVirtual != 0)
                    {
                        v.sqlite3VdbeAddOp0(OpCode.OP_VBegin);
                    }
                    ///
                    ///<summary>
                    ///If the file format and encoding in the database have not been set,
                    ///set them now.
                    ///
                    ///</summary>
                    reg1 = pParse.regRowid = ++pParse.nMem;
                    reg2 = pParse.regRoot = ++pParse.nMem;
                    reg3 = ++pParse.nMem;
                    v.sqlite3VdbeAddOp3(OpCode.OP_ReadCookie, iDb, reg3, Sqlite3.BTREE_FILE_FORMAT);
                    vdbeaux.sqlite3VdbeUsesBtree(v, iDb);
                    j1 = v.sqlite3VdbeAddOp1(OpCode.OP_If, reg3);
                    fileFormat = (db.flags & SqliteFlags.SQLITE_LegacyFileFmt) != 0 ? 1 : sqliteinth.SQLITE_MAX_FILE_FORMAT;
                    v.sqlite3VdbeAddOp2(OpCode.OP_Integer, fileFormat, reg3);
                    v.sqlite3VdbeAddOp3(OpCode.OP_SetCookie, iDb, Sqlite3.BTREE_FILE_FORMAT, reg3);
                    v.sqlite3VdbeAddOp2(OpCode.OP_Integer, (int)sqliteinth.ENC(db), reg3);
                    v.sqlite3VdbeAddOp3(OpCode.OP_SetCookie, iDb, Sqlite3.BTREE_TEXT_ENCODING, reg3);
                    v.sqlite3VdbeJumpHere(j1);
                    ///
                    ///<summary>
                    ///</summary>
                    ///This just creates a place holder record in the sqlite_master table.
                    ///The record created does not contain anything yet.  It will be replaced
                    ///by the real entry in code generated at sqlite3EndTable().by the real entry in code generated at sqlite3EndTable().
                    ///The rowid for the new entry is left in register pParse>regRowid.
                    ///The root page number of the new table is left in reg pParse>regRoot.
                    ///The rowid and root page number values are needed by the code that
                    ///sqlite3EndTable will generate.
                    if (isView != 0 || isVirtual != 0)
                    {
                        v.sqlite3VdbeAddOp2(OpCode.OP_Integer, 0, reg2);
                    }
                    else
                    {
                        v.sqlite3VdbeAddOp2(OpCode.OP_CreateTable, iDb, reg2);
                    }
                    build.sqlite3OpenMasterTable(pParse, iDb);
                    v.sqlite3VdbeAddOp2(OpCode.OP_NewRowid, 0, reg1);
                    v.sqlite3VdbeAddOp2(OpCode.OP_Null, 0, reg3);
                    v.sqlite3VdbeAddOp3(OpCode.OP_Insert, 0, reg3, reg1);
                    v.sqlite3VdbeChangeP5(OpFlag.OPFLAG_APPEND);
                    v.sqlite3VdbeAddOp0(OpCode.OP_Close);
                }
                ///
                ///<summary>
                ///</summary>
                ///<param name="Normal (non">error) return. </param>
                return;
            ///
            ///<summary>
            ///If an error occurs, we jump here 
            ///</summary>
            begin_table_error:
                db.sqlite3DbFree(ref zName);
                return;
            }
           


            ///<summary>
            ///This routine is called to do the work of a DROP TABLE statement.
            ///pName is the name of the table to be dropped.
            ///</summary>
            public static void sqlite3DropTable(Parse pParse, SrcList pName, int isView, int noErr)
            {
                //if ( db.mallocFailed != 0 )
                //{
                //  goto exit_drop_table;
                //}
                Debug.Assert(pParse.nErr == 0);
                Debug.Assert(pName.nSrc == 1);

                sqlite3 db = pParse.db;
                if (noErr != 0)
                    db.suppressErr++;
                var pTab = TableBuilder.sqlite3LocateTable(pParse, isView, pName.a[0].zName, pName.a[0].zDatabase);
                if (noErr != 0)
                    db.suppressErr--;
                if (pTab == null)
                {
                    if (noErr != 0)
                        build.sqlite3CodeVerifyNamedSchema(pParse, pName.a[0].zDatabase);
                    goto exit_drop_table;
                }
                var iDb = Sqlite3.sqlite3SchemaToIndex(db, pTab.pSchema);
                Debug.Assert(iDb >= 0 && iDb < db.nDb);
                ///If pTab is a virtual table, call ViewGetColumnNames() to ensure
                ///it is initialized.
                if (pTab.IsVirtual() && build.sqlite3ViewGetColumnNames(pParse, pTab) != 0)
                {
                    goto exit_drop_table;
                }
#if !SQLITE_OMIT_AUTHORIZATION
																																																																																	{
int code;
string zTab = SCHEMA_TABLE(iDb);
string zDb = db.aDb[iDb].zName;
string zArg2 = 0;
if( sqlite3AuthCheck(pParse, SQLITE_DELETE, zTab, 0, zDb)){
goto exit_drop_table;
}
if( isView ){
if( OMIT_TEMPDB ==0&& iDb==1 ){
code = SQLITE_DROP_TEMP_VIEW;
}else{
code = SQLITE_DROP_VIEW;
}
}else if( pTab.IsVirtual() ){
code = SQLITE_DROP_VTABLE;
zArg2 = sqlite3GetVTable(db, pTab)->pMod->zName;
}else{
if( OMIT_TEMPDB ==0&& iDb==1 ){
code = SQLITE_DROP_TEMP_TABLE;
}else{
code = SQLITE_DROP_TABLE;
}
}
if( sqlite3AuthCheck(pParse, code, pTab.zName, zArg2, zDb) ){
goto exit_drop_table;
}
if( sqlite3AuthCheck(pParse, SQLITE_DELETE, pTab.zName, 0, zDb) ){
goto exit_drop_table;
}
}
#endif
                if (pTab.zName.StartsWith("sqlite_", System.StringComparison.InvariantCultureIgnoreCase))
                {
                    utilc.sqlite3ErrorMsg(pParse, "table %s may not be dropped", pTab.zName);
                    goto exit_drop_table;
                }
#if !SQLITE_OMIT_VIEW
                ///Ensure DROP TABLE is not used on a view, and DROP VIEW is not used
                ///on a table.
                if (isView != 0 && pTab.pSelect == null)
                {
                    utilc.sqlite3ErrorMsg(pParse, "use DROP TABLE to delete table %s", pTab.zName);
                    goto exit_drop_table;
                }
                if (0 == isView && pTab.pSelect != null)
                {
                    utilc.sqlite3ErrorMsg(pParse, "use DROP VIEW to delete view %s", pTab.zName);
                    goto exit_drop_table;
                }
#endif
                ///Generate code to remove the table from the master table
                ///on disk.
                var v = pParse.sqlite3GetVdbe();
                if (v != null)
                {
                    Db pDb = db.aDb[iDb];
                    build.sqlite3BeginWriteOperation(pParse, 1, iDb);
#if !SQLITE_OMIT_VIRTUALTABLE
                    if (pTab.IsVirtual())
                    {
                        v.sqlite3VdbeAddOp0(OpCode.OP_VBegin);
                    }
#endif
                    pParse.sqlite3FkDropTable(pName, pTab);
                    ///Drop all triggers associated with the table being dropped. Code
                    ///is generated to remove entries from sqlite_master and/or
                    ///sqlite_temp_master if required.
                    pTab.sqlite3TriggerList(pParse).linkedList()
                        .ForEach(trg =>
                        {
                            Debug.Assert(trg.pSchema == pTab.pSchema || trg.pSchema == db.aDb[1].pSchema);
                            TriggerParser.sqlite3DropTriggerPtr(pParse, trg);
                        }
                    );

#if !SQLITE_OMIT_AUTOINCREMENT
                    ///Remove any entries of the sqlite_sequence table associated with
                    ///the table being dropped. This is done before the table is dropped
                    ///at the btree level, in case the sqlite_sequence table needs to
                    ///<param name="move as a result of the drop (can happen in auto">vacuum mode).</param>
                    if ((pTab.tabFlags & TableFlags.TF_Autoincrement) != 0)
                    {
                        build.sqlite3NestedParse(pParse, "DELETE FROM %s.sqlite_sequence WHERE name=%Q", pDb.zName, pTab.zName);
                    }
#endif
                    ///Drop all SQLITE_MASTER table and index entries that refer to the
                    ///table. The program name loops through the master table and deletes
                    ///every row that refers to a table of the same name as the one being
                    ///dropped. Triggers are handled seperately because a trigger can be
                    ///created in the temp database that refers to a table in another
                    ///database.
                    build.sqlite3NestedParse(pParse, "DELETE FROM %Q.%s WHERE tbl_name=%Q and type!='trigger'", pDb.zName, sqliteinth.SCHEMA_TABLE(iDb), pTab.zName);
                    ///Drop any statistics from the sqlite_stat1 table, if it exists 
                    if (TableBuilder.sqlite3FindTable(db, "sqlite_stat1", db.aDb[iDb].zName) != null)
                    {
                        build.sqlite3NestedParse(pParse, "DELETE FROM %Q.sqlite_stat1 WHERE tbl=%Q", pDb.zName, pTab.zName);
                    }
                    if (0 == isView && !pTab.IsVirtual())
                    {
                        destroyTable(pParse, pTab);
                    }
                    ///Remove the table entry from SQLite's internal schema and modify
                    ///the schema cookie.
                    if (pTab.IsVirtual())
                    {
                        v.sqlite3VdbeAddOp4(OpCode.OP_VDestroy, iDb, 0, 0, pTab.zName, 0);
                    }
                    v.sqlite3VdbeAddOp4(OpCode.OP_DropTable, iDb, 0, 0, pTab.zName, 0);
                    build.sqlite3ChangeCookie(pParse, iDb);
                }
                build.sqliteViewResetAll(db, iDb);
            exit_drop_table:
                build.sqlite3SrcListDelete(db, ref pName);
            }
            


            ///</summary>
            ///<summary>
            /// Write VDBE code to erase table pTab and all associated indices on disk.
            /// Code to update the sqlite_master tables and internal schema definitions
            /// in case a root-page belonging to another table is moved by the btree layer
            /// is also added (this can happen with an auto-vacuum database).
            ///
            ///</summary>
            static void destroyTable(Parse pParse, Table pTab)
            {
#if SQLITE_OMIT_AUTOVACUUM
																																																																																	Index pIdx;
int iDb = sqlite3SchemaToIndex( pParse.db, pTab.pSchema );
destroyRootPage( pParse, pTab.tnum, iDb );
for ( pIdx = pTab.pIndex ; pIdx != null ; pIdx = pIdx.pNext )
{
destroyRootPage( pParse, pIdx.tnum, iDb );
}
#else
                ///<param name="If the database may be auto">vacuum capable (if SQLITE_OMIT_AUTOVACUUM</param>
                ///<param name="is not defined), then it is important to call  OpCode.OP_Destroy on the">is not defined), then it is important to call  OpCode.OP_Destroy on the</param>
                ///<param name="table and index root">pages in order, starting with the numerically</param>
                ///<param name="largest root">pages</param>
                ///<param name="to be destroyed is relocated by an earlier  OpCode.OP_Destroy. i.e. if the">to be destroyed is relocated by an earlier  OpCode.OP_Destroy. i.e. if the</param>
                ///<param name="following were coded:">following were coded:</param>
                ///<param name=""></param>
                ///<param name="OP_Destroy 4 0">OP_Destroy 4 0</param>
                ///<param name="...">...</param>
                ///<param name="OP_Destroy 5 0">OP_Destroy 5 0</param>
                ///<param name=""></param>
                ///<param name="and root page 5 happened to be the largest root">page number in the</param>
                ///<param name="database, then root page 5 would be moved to page 4 by the">database, then root page 5 would be moved to page 4 by the</param>
                ///<param name=""OP_Destroy 4 0" opcode. The subsequent "OP_Destroy 5 0" would hit">"OP_Destroy 4 0" opcode. The subsequent "OP_Destroy 5 0" would hit</param>
                ///<param name="a free">list page.</param>
                int iTab = pTab.tnum;
                int iDestroyed = 0;
                while (true)
                {
                    Index pIdx;
                    int iLargest = 0;
                    if (iDestroyed == 0 || iTab < iDestroyed)
                    {
                        iLargest = iTab;
                    }
                    for (pIdx = pTab.pIndex; pIdx != null; pIdx = pIdx.pNext)
                    {
                        int iIdx = pIdx.tnum;
                        Debug.Assert(pIdx.pSchema == pTab.pSchema);
                        if ((iDestroyed == 0 || (iIdx < iDestroyed)) && iIdx > iLargest)
                        {
                            iLargest = iIdx;
                        }
                    }
                    if (iLargest == 0)
                    {
                        return;
                    }
                    else
                    {
                        int iDb = Sqlite3.sqlite3SchemaToIndex(pParse.db, pTab.pSchema);
                        build.destroyRootPage(pParse, iLargest, iDb);
                        iDestroyed = iLargest;
                    }
                }
#endif
            }


            ///<summary>
            /// Remove the memory data structures associated with the given
            /// Table.  No changes are made to disk by this routine.
            ///
            /// This routine just deletes the data structure.  It does not unlink
            /// the table data structure from the hash table.  But it does destroy
            /// memory structures of the indices and foreign keys associated with
            /// the table.
            ///
            ///</summary>
            public static void sqlite3DeleteTable(sqlite3 db, ref Table pTable)
            {
                Debug.Assert(null == pTable || pTable.nRef > 0);
                ///Do not delete the table until the reference count reaches zero. 
                if (null == pTable)
                    return;
                if ((// ( !db || db->pnBytesFreed == 0 ) && 
                (--pTable.nRef) > 0))
                    return;
                ///Delete all indices associated with this table. 
                foreach (var pIndex in pTable.pIndex.path(x => x.pNext))
                {
                    Debug.Assert(pIndex.pSchema == pTable.pSchema);
                    //if( null==db || db.pnBytesFreed==0 ){
                    string zName = pIndex.zName;
                    //
#if !NDEBUG || SQLITE_COVERAGE_TEST
																																																																																																												        //  TESTONLY ( Index pOld = ) sqlite3HashInsert(
        //ref pIndex.pSchema.idxHash, zName, StringExtensions.sqlite3Strlen30(zName), 0
        //  );
        Index pOld = sqlite3HashInsert(
      ref pIndex.pSchema.idxHash, zName, StringExtensions.sqlite3Strlen30( zName ), (Index)null
        );
        Debug.Assert( db == null || sqlite3SchemaMutexHeld( db, 0, pIndex.pSchema ) );
        Debug.Assert( pOld == pIndex || pOld == null );
#else
                    //  TESTONLY ( Index pOld = ) sqlite3HashInsert(
                    //ref pIndex.pSchema.idxHash, zName, StringExtensions.sqlite3Strlen30(zName), 0
                    //  );
                    HashExtensions.sqlite3HashInsert(ref pIndex.pSchema.idxHash, zName, StringExtensions.sqlite3Strlen30(zName), (Index)null);
#endif
                    //}
                    var index = pIndex;
                    IndexBuilder.freeIndex(db, ref index);
                }
                ///Delete any foreign keys attached to this table. 
                fkeyc.sqlite3FkDelete(db, pTable);
                ///Delete the Table structure itself.
                build.sqliteDeleteColumnNames(db, pTable);
                db.sqlite3DbFree(ref pTable.zName);
                db.sqlite3DbFree(ref pTable.zColAff);
                SelectMethods.sqlite3SelectDelete(db, ref pTable.pSelect);
#if !SQLITE_OMIT_CHECK
                exprc.sqlite3ExprDelete(db, ref pTable.pCheck);
#endif
#if !SQLITE_OMIT_VIRTUALTABLE
                vtab.sqlite3VtabClear(db, pTable);
#endif
                pTable = null;
                //      sqlite3DbFree( db, ref pTable );
            }



            ///
            ///<summary>
            ///Generate a CREATE TABLE statement appropriate for the given
            ///table.  Memory to hold the text of the statement is obtained
            ///from sqliteMalloc() and must be freed by the calling function.
            ///
            ///</summary>
            public static string createTableStmt(this sqlite3 db, Table p)
            {
                int i, k, n;
                StringBuilder zStmt;
                string zSep;
                string zSep2;
                string zEnd;
                Column pCol;
                n = 0;
                for (i = 0; i < p.nCol; i++)
                {
                    //, pCol++){
                    pCol = p.aCol[i];
                    n += build.identLength(pCol.zName) + 5;
                }
                n += build.identLength(p.zName);
                if (n < 50)
                {
                    zSep = "";
                    zSep2 = ",";
                    zEnd = ")";
                }
                else
                {
                    zSep = "\n  ";
                    zSep2 = ",\n  ";
                    zEnd = "\n)";
                }
                n += 35 + 6 * p.nCol;
                zStmt = new StringBuilder(n);
                //zStmt = sqlite3DbMallocRaw(0, n);
                //if( zStmt==0 ){
                //  db.mallocFailed = 1;
                //  return 0;
                //}
                //io.sqlite3_snprintf(n, zStmt,"CREATE TABLE ");
                zStmt.Append("CREATE TABLE ");
                k = StringExtensions.sqlite3Strlen30(zStmt);
                build.identPut(zStmt, ref k, p.zName);
                zStmt.Append('(');
                //zStmt[k++] = '(';
                for (i = 0; i < p.nCol; i++)
                {
                    //, pCol++){
                    pCol = p.aCol[i];
                    string[] azType = new string[] {
					///
					///<summary>
					///sqliteinth.SQLITE_AFF_TEXT    
					///</summary>
					" TEXT",
					///
					///<summary>
					///SQLITE_AFF_NONE    
					///</summary>
					"",
					///
					///<summary>
					///SQLITE_AFF_NUMERIC 
					///</summary>
					" NUM",
					///
					///<summary>
					///SQLITE_AFF_INTEGER 
					///</summary>
					" INT",
					///
					///<summary>
					///SQLITE_AFF_REAL    
					///</summary>
					" REAL"
				};
                    int len;
                    string zType;
                    zStmt.Append(zSep);
                    //  io.sqlite3_snprintf(n-k, zStmt[k], zSep);
                    k = StringExtensions.sqlite3Strlen30(zStmt);
                    //  k += strlen(zStmt[k]);
                    zSep = zSep2;
                    build.identPut(zStmt, ref k, pCol.zName);
                    Debug.Assert(pCol.affinity - sqliteinth.SQLITE_AFF_TEXT >= 0);
                    Debug.Assert(pCol.affinity - sqliteinth.SQLITE_AFF_TEXT < Sqlite3.ArraySize(azType));
                    sqliteinth.testcase(pCol.affinity == sqliteinth.SQLITE_AFF_TEXT);
                    sqliteinth.testcase(pCol.affinity == sqliteinth.SQLITE_AFF_NONE);
                    sqliteinth.testcase(pCol.affinity == sqliteinth.SQLITE_AFF_NUMERIC);
                    sqliteinth.testcase(pCol.affinity == sqliteinth.SQLITE_AFF_INTEGER);
                    sqliteinth.testcase(pCol.affinity == sqliteinth.SQLITE_AFF_REAL);
                    zType = azType[pCol.affinity - sqliteinth.SQLITE_AFF_TEXT];
                    len = StringExtensions.sqlite3Strlen30(zType);
                    Debug.Assert(pCol.affinity == sqliteinth.SQLITE_AFF_NONE || pCol.affinity == build.sqlite3AffinityType(zType));
                    zStmt.Append(zType);
                    // memcpy( &zStmt[k], zType, len );
                    k += len;
                    Debug.Assert(k <= n);
                }
                zStmt.Append(zEnd);
                //sqlite3_snprintf(n-k, zStmt[k], "%s", zEnd);
                return zStmt.ToString();
            }
            ///<summary>
            /// This routine is called to report the final ")" that terminates
            /// a CREATE TABLE statement.
            ///
            /// The table structure that other action routines have been building
            /// is added to the internal hash tables, assuming no errors have
            /// occurred.
            ///
            /// An entry for the table is made in the master table on disk, unless
            /// this is a temporary table or db.init.busy==1.  When db.init.busy==1
            /// it means we are reading the sqlite_master table because we just
            /// connected to the database or because the sqlite_master table has
            /// recently changed, so the entry for this table already exists in
            /// the sqlite_master table.  We do not want to create it again.
            ///
            /// If the pSelect argument is not NULL, it means that this routine
            /// was called to create a table generated from a
            /// "CREATE TABLE ... AS SELECT ..." statement.  The column names of
            /// the new table will match the result set of the SELECT.
            ///
            ///</summary>
            // OVERLOADS, so I don't need to rewrite parse.c
            public static void sqlite3EndTable(this Parse pParse, Token pCons, Token pEnd, int null_4)
            {
                sqlite3EndTable(pParse, pCons, pEnd, null);
            }
            public static void sqlite3EndTable(this Parse pParse, int null_2, int null_3, Select pSelect)
            {
                sqlite3EndTable(pParse, null, null, pSelect);
            }
            public static void sqlite3EndTable(this Parse pParse,///Parse context 
                Token pCons,///The ',' token after the last column defn. 
                Token pEnd,///The final ')' token in the CREATE TABLE 
                Select pSelect///Select from a "CREATE ... AS SELECT" 
            )
            {
                if ((pEnd == null && pSelect == null)///
                    ///|| db.mallocFailed != 0 
                )
                {
                    return;
                }

                var pNewTable = pParse.pNewTable;
                if (pNewTable == null)
                    return;
                sqlite3 db = pParse.db;
                Debug.Assert(0 == db.init.busy || pSelect == null);
#if !SQLITE_OMIT_CHECK
                ///Resolve names in all CHECK constraint expressions.
                if (pNewTable.pCheck != null)
                {
                    // memset(sNC, 0, sizeof(sNC));
                    ///Fake SrcList for pParse.pNewTable 
                    var sSrc = new SrcList()
                    {
                        // memset(sSrc, 0, sizeof(sSrc));
                        nSrc = 1,
                        a = new SrcList_item[] {
                            new SrcList_item(){
                                zName = pNewTable.zName,
                                pTab = pNewTable,
                                iCursor = -1,
                            }
                        }
                    };

                    ///Name context for pParse.pNewTable 
                    var sNC = new NameContext()
                    {
                        pParse = pParse,
                        pSrcList = sSrc,
                        isCheck = 1
                    };

                    if (Sqlite3.ResolveExtensions.sqlite3ResolveExprNames(sNC, ref pNewTable.pCheck) != 0)
                    {
                        return;
                    }
                }
#endif
                ///If the db.init.busy is 1 it means we are reading the SQL off the
                ///"sqlite_master" or "sqlite_temp_master" table on the disk.
                ///So do not write to the disk again.  Extract the root page number
                ///for the table from the db.init.newTnum field.  (The page number
                ///should have been put there by the sqliteOpenCb routine.)
                if (db.init.busy != 0)
                {
                    pNewTable.tnum = db.init.newTnum;
                }

                var iDb = Sqlite3.sqlite3SchemaToIndex(db, pNewTable.pSchema);

                ///If not initializing, then create a record for the new table
                ///in the SQLITE_MASTER table of the database.
                ///
                ///If this is a TEMPORARY table, write the entry into the auxiliary
                ///file instead of into the main database file.
                if (0 == db.init.busy)
                {
                    int n;
                    String zType = "";
                    ///"view" or "table" 
                    String zType2 = "";
                    ///"VIEW" or "TABLE" 
                    String zStmt = "";
                    ///Text of the CREATE TABLE or CREATE VIEW statement 
                    var v = pParse.sqlite3GetVdbe();
                    if (Sqlite3.NEVER(v == null))
                        return;
                    v.sqlite3VdbeAddOp1(OpCode.OP_Close, 0);
                    ///Initialize zType for the new view or table.
                    if (pNewTable.pSelect == null)
                    {
                        ///A regular table 
                        zType = "table";
                        zType2 = "TABLE";
#if !SQLITE_OMIT_VIEW
                    }
                    else
                    {
                        ///A view 
                        zType = "view";
                        zType2 = "VIEW";
#endif
                    }
                    ///If this is a CREATE TABLE xx AS SELECT ..., execute the SELECT
                    ///
                    ///statement to populate the new table. The root">page number for the</param>
                    ///new table is in register pParse">>regRoot.</param>
                    ///
                    ///Once the SELECT has been coded by sqlite3Select(), it is in a">Once the SELECT has been coded by sqlite3Select(), it is in a</param>
                    ///suitable state to query for the column names and types to be used">suitable state to query for the column names and types to be used</param>
                    ///by the new table.">by the new table.</param>
                    ///
                    ///A shared-lock is not required to write to the new table,
                    ///lock must have already been obtained to create it. 
                    ///Since a schema-lock would be redundant.

                    if (pSelect != null)
                    {
                        Debug.Assert(pParse.nTab == 1);
                        v.sqlite3VdbeAddOp3(OpCode.OP_OpenWrite, 1, pParse.regRoot, iDb);
                        v.sqlite3VdbeChangeP5(1);
                        SelectDest dest = new SelectDest();
                        pParse.nTab = 2;
                        dest.Init(SelectResultType.Table, 1);
                        Select.sqlite3Select(pParse, pSelect, ref dest);
                        v.sqlite3VdbeAddOp1(OpCode.OP_Close, 1);
                        if (pParse.nErr == 0)
                        {
                            var pSelTab = SelectMethods.sqlite3ResultSetOfSelect(pParse, pSelect);
                            if (pSelTab == null)
                                return;
                            Debug.Assert(pNewTable.aCol == null);
                            pNewTable.nCol = pSelTab.nCol;
                            pNewTable.aCol = pSelTab.aCol;
                            pSelTab.nCol = 0;
                            pSelTab.aCol = null;
                            TableBuilder.sqlite3DeleteTable(db, ref pSelTab);
                        }
                    }

                    ///Compute the complete text of the CREATE statement 
                    if (pSelect != null)
                    {
                        zStmt = createTableStmt(db, pNewTable);
                    }
                    else
                    {
                        n = (int)(pParse.sNameToken.zRestSql.Length - pEnd.zRestSql.Length) + 1;
                        zStmt = io.sqlite3MPrintf(db, "CREATE %s %.*s", zType2, n, pParse.sNameToken.zRestSql);
                    }
                    ///A slot for the record has already been allocated in the
                    ///SQLITE_MASTER table.  We just need to update that slot with all
                    ///the information we've collected.
                    build.sqlite3NestedParse(pParse, "UPDATE %Q.%s " + "SET type='%s', name=%Q, tbl_name=%Q, rootpage=#%d, sql=%Q " + "WHERE rowid=#%d", db.aDb[iDb].zName, sqliteinth.SCHEMA_TABLE(iDb), zType, pNewTable.zName, pNewTable.zName, pParse.regRoot, zStmt, pParse.regRowid);
                    db.sqlite3DbFree(ref zStmt);
                    build.sqlite3ChangeCookie(pParse, iDb);
#if !SQLITE_OMIT_AUTOINCREMENT
                    ///Check to see if we need to create an sqlite_sequence table for
                    ///keeping track of autoincrement keys.
                    if ((pNewTable.tabFlags & TableFlags.TF_Autoincrement) != 0)
                    {
                        Db pDb = db.aDb[iDb];
                        Debug.Assert(Sqlite3.sqlite3SchemaMutexHeld(db, iDb, null));
                        if (pDb.pSchema.pSeqTab == null)
                        {
                            build.sqlite3NestedParse(pParse, "CREATE TABLE %Q.sqlite_sequence(name,seq)", pDb.zName);
                        }
                    }
#endif
                    ///Reparse everything to update our internal data structures 
                    v.sqlite3VdbeAddParseSchemaOp(iDb, io.sqlite3MPrintf(db, "tbl_name='%q'", pNewTable.zName));
                }
                ///Add the table to the in-memory representation of the database.</param>
                ///<param name=""></param>
                if (db.init.busy != 0)
                {
                    Table pOld;
                    Schema pSchema = pNewTable.pSchema;
                    Debug.Assert(Sqlite3.sqlite3SchemaMutexHeld(db, iDb, null));
                    pOld = pSchema.tblHash.HashInsert(pNewTable.zName.sub(), pNewTable);
                    if (pOld != null)
                    {
                        Debug.Assert(pNewTable == pOld);
                        ///Malloc must have failed inside HashInsert() 
                        //        db.mallocFailed = 1;
                        return;
                    }
                    pParse.pNewTable = null;
                    db.nTable++;
                    db.flags |= SqliteFlags.SQLITE_InternChanges;
#if !SQLITE_OMIT_ALTERTABLE
                    if (pNewTable.pSelect == null)
                    {
                        string zName = pParse.sNameToken.zRestSql;
                        int nName;
                        Debug.Assert(pSelect == null && pCons != null && pEnd != null);
                        if (pCons.zRestSql == null)
                        {
                            pCons = pEnd;
                        }
                        nName = zName.Length - pCons.zRestSql.Length;
                        pNewTable.addColOffset = 13 + nName;
                        // sqlite3Utf8CharLen(zName, nName);
                    }
#endif
                }
            }
            
        }


}
