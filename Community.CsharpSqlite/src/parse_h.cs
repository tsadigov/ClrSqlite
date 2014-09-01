/*
*************************************************************************
**  Included in SQLite3 port to C#-SQLite;  2008 Noah B Hart
**  C#-SQLite is an independent reimplementation of the SQLite software library
**
**  SQLITE_SOURCE_ID: 2010-08-23 18:52:01 42537b60566f288167f1b5864a5435986838e3a3
**
*************************************************************************
*/namespace Community.CsharpSqlite {
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
      using sqlite3_value = Sqlite3.Mem;
#if !SQLITE_MAX_VARIABLE_NUMBER
      using ynVar = System.Int16;
#else
using ynVar = System.Int32; 
#endif
      /*
** The yDbMask datatype for the bitmask of all attached databases.
*/
#if SQLITE_MAX_ATTACHED
//  typedef sqlite3_uint64 yDbMask;
using yDbMask = System.Int64; 
#else
      //  typedef unsigned int yDbMask;
      using yDbMask = System.Int32;
#endif
	public partial class Sqlite3 {
		



        public class Parse
        {
            public sqlite3 db;
            /* The main database structure */
            public int rc;
            /* Return code from execution */
            public string zErrMsg;
            /* An error message */
            public Vdbe pVdbe;
            /* An engine for executing database bytecode */
            public u8 colNamesSet;
            /* TRUE after OP_ColumnName has been issued to pVdbe */
            public u8 nameClash;
            /* A permanent table name clashes with temp table name */
            public u8 checkSchema;
            /* Causes schema cookie check after an error */
            public u8 nested;
            /* Number of nested calls to the parser/code generator */
            public u8 parseError;
            /* True after a parsing error.  Ticket #1794 */
            public u8 nTempReg;
            /* Number of temporary registers in aTempReg[] */
            public u8 nTempInUse;
            /* Number of aTempReg[] currently checked out */
            public int[] aTempReg;
            /* Holding area for temporary registers */
            public int nRangeReg;
            /* Size of the temporary register block */
            public int iRangeReg;
            /* First register in temporary register block */
            public int nErr;
            /* Number of errors seen */
            public int nTab;
            /* Number of previously allocated VDBE cursors */
            public int nMem;
            /* Number of memory cells used so far */
            public int nSet;
            /* Number of sets used so far */
            public int ckBase;
            /* Base register of data during check constraints */
            public int iCacheLevel;
            /* ColCache valid when aColCache[].iLevel<=iCacheLevel */
            public int iCacheCnt;
            /* Counter used to generate aColCache[].lru values */
            public u8 nColCache;
            /* Number of entries in the column cache */
            public u8 iColCache;
            /* Next entry of the cache to replace */
            public yColCache[] aColCache;
            /* One for each valid column cache entry */
            public yDbMask writeMask;
            /* Start a write transaction on these databases */
            public yDbMask cookieMask;
            /* Bitmask of schema verified databases */
            public u8 isMultiWrite;
            /* True if statement may affect/insert multiple rows */
            public u8 mayAbort;
            /* True if statement may throw an ABORT exception */
            public int cookieGoto;
            /* Address of OP_Goto to cookie verifier subroutine */
            public int[] cookieValue;
            /* Values of cookies to verify */
#if !SQLITE_OMIT_SHARED_CACHE
																																																																																				public int nTableLock;         /* Number of locks in aTableLock */
public TableLock[] aTableLock; /* Required table locks for shared-cache mode */
#endif
            public int regRowid;
            /* Register holding rowid of CREATE TABLE entry */
            public int regRoot;
            /* Register holding root page number for new objects */
            public AutoincInfo pAinc;
            /* Information about AUTOINCREMENT counters */
            public int nMaxArg;
            /* Max args passed to user function by sub-program */
            /* Information used while coding trigger programs. */
            public Parse pToplevel;
            /* Parse structure for main program (or NULL) */
            public Table pTriggerTab;
            /* Table triggers are being coded for */
            public u32 oldmask;
            /* Mask of old.* columns referenced */
            public u32 newmask;
            /* Mask of new.* columns referenced */
            public u8 eTriggerOp;
            /* TK_UPDATE, TK_INSERT or TK_DELETE */
            public u8 eOrconf;
            /* Default ON CONFLICT policy for trigger steps */
            public u8 disableTriggers;
            /* True to disable triggers */
            public double nQueryLoop;
            /* Estimated number of iterations of a query */
            /* Above is constant between recursions.  Below is reset before and after
** each recursion */
            public int nVar;
            /* Number of '?' variables seen in the SQL so far */
            public int nzVar;
            /* Number of available slots in azVar[] */
            public string[] azVar;
            /* Pointers to names of parameters */
            public Vdbe pReprepare;
            /* VM being reprepared (sqlite3Reprepare()) */
            public int nAlias;
            /* Number of aliased result set columns */
            public int nAliasAlloc;
            /* Number of allocated slots for aAlias[] */
            public int[] aAlias;
            /* Register used to hold aliased result */
            public u8 explain;
            /* True if the EXPLAIN flag is found on the query */
            public Token sNameToken;
            /* Token with unqualified schema object name */
            public Token sLastToken;
            /* The last token parsed */
            public StringBuilder zTail { get; set; }
            /* All SQL text past the last semicolon parsed */
            public Table pNewTable;
            /* A table being constructed by CREATE TABLE */
            public Trigger pNewTrigger;
            /* Trigger under construct by a CREATE TRIGGER */
            public string zAuthContext;
            /* The 6th parameter to db.xAuth callbacks */
#if !SQLITE_OMIT_VIRTUALTABLE
            public Token sArg;
            /* Complete text of a module argument */
            public u8 declareVtab;
            /* True if inside sqlite3_declare_vtab() */
            public int nVtabLock;
            /* Number of virtual tables to lock */
            public Table[] apVtabLock;
            /* Pointer to virtual tables needing locking */
#endif
            public int nHeight;
            /* Expression tree height of current sub-select */
            public Table pZombieTab;
            /* List of Table objects to delete after code gen */
            public TriggerPrg pTriggerPrg;
            ///<summary>
            ///Linked list of coded triggers
            ///</summary>
#if !SQLITE_OMIT_EXPLAIN
            public int iSelectId;
            public int iNextSelectId;
#endif
            // We need to create instances of the col cache
            public Parse()
            {
                aTempReg = new int[8];
                /* Holding area for temporary registers */
                aColCache = new yColCache[SQLITE_N_COLCACHE];
                /* One for each valid column cache entry */
                for (int i = 0; i < this.aColCache.Length; i++)
                {
                    this.aColCache[i] = new yColCache();
                }
                cookieValue = new int[SQLITE_MAX_ATTACHED + 2];
                /* Values of cookies to verify */
                sLastToken = new Token();
                /* The last token parsed */
#if !SQLITE_OMIT_VIRTUALTABLE
                sArg = new Token();
#endif
            }
            public void ResetMembers()// Need to clear all the following variables during each recursion
            {
                nVar = 0;
                nzVar = 0;
                azVar = null;
                nAlias = 0;
                nAliasAlloc = 0;
                aAlias = null;
                explain = 0;
                sNameToken = new Token();
                sLastToken = new Token();
                zTail.Length = 0;
                pNewTable = null;
                pNewTrigger = null;
                zAuthContext = null;
#if !SQLITE_OMIT_VIRTUALTABLE
                sArg = new Token();
                declareVtab = 0;
                nVtabLock = 0;
                apVtabLock = null;
#endif
                nHeight = 0;
                pZombieTab = null;
                pTriggerPrg = null;
            }
            private Parse[] SaveBuf = new Parse[10];
            //For Recursion Storage
            public void RestoreMembers()// Need to clear all the following variables during each recursion
            {
                if (SaveBuf[nested] != null)
                {
                    nVar = SaveBuf[nested].nVar;
                    nzVar = SaveBuf[nested].nzVar;
                    azVar = SaveBuf[nested].azVar;
                    nAlias = SaveBuf[nested].nAlias;
                    nAliasAlloc = SaveBuf[nested].nAliasAlloc;
                    aAlias = SaveBuf[nested].aAlias;
                    explain = SaveBuf[nested].explain;
                    sNameToken = SaveBuf[nested].sNameToken;
                    sLastToken = SaveBuf[nested].sLastToken;
                    zTail = SaveBuf[nested].zTail;
                    pNewTable = SaveBuf[nested].pNewTable;
                    pNewTrigger = SaveBuf[nested].pNewTrigger;
                    zAuthContext = SaveBuf[nested].zAuthContext;
#if !SQLITE_OMIT_VIRTUALTABLE
                    sArg = SaveBuf[nested].sArg;
                    declareVtab = SaveBuf[nested].declareVtab;
                    nVtabLock = SaveBuf[nested].nVtabLock;
                    apVtabLock = SaveBuf[nested].apVtabLock;
#endif
                    nHeight = SaveBuf[nested].nHeight;
                    pZombieTab = SaveBuf[nested].pZombieTab;
                    pTriggerPrg = SaveBuf[nested].pTriggerPrg;
                    SaveBuf[nested] = null;
                }
            }
            public void SaveMembers()// Need to clear all the following variables during each recursion
            {
                SaveBuf[nested] = new Parse();
                SaveBuf[nested].nVar = nVar;
                SaveBuf[nested].nzVar = nzVar;
                SaveBuf[nested].azVar = azVar;
                SaveBuf[nested].nAlias = nAlias;
                SaveBuf[nested].nAliasAlloc = nAliasAlloc;
                SaveBuf[nested].aAlias = aAlias;
                SaveBuf[nested].explain = explain;
                SaveBuf[nested].sNameToken = sNameToken;
                SaveBuf[nested].sLastToken = sLastToken;
                SaveBuf[nested].zTail = zTail;
                SaveBuf[nested].pNewTable = pNewTable;
                SaveBuf[nested].pNewTrigger = pNewTrigger;
                SaveBuf[nested].zAuthContext = zAuthContext;
#if !SQLITE_OMIT_VIRTUALTABLE
                SaveBuf[nested].sArg = sArg;
                SaveBuf[nested].declareVtab = declareVtab;
                SaveBuf[nested].nVtabLock = nVtabLock;
                SaveBuf[nested].apVtabLock = apVtabLock;
#endif
                SaveBuf[nested].nHeight = nHeight;
                SaveBuf[nested].pZombieTab = pZombieTab;
                SaveBuf[nested].pTriggerPrg = pTriggerPrg;
            }
            /**
///<summary>
///</summary>
///<param name="This function is called by the parser after the table">name in</param>
///<param name="an "ALTER TABLE <table">name> ADD" statement is parsed. Argument</param>
///<param name="pSrc is the full">name of the table being altered.</param>
///<param name=""></param>
///<param name="This routine makes a (partial) copy of the Table structure">This routine makes a (partial) copy of the Table structure</param>
///<param name="for the table being altered and sets Parse.pNewTable to point">for the table being altered and sets Parse.pNewTable to point</param>
///<param name="to it. Routines called by the parser as the column definition">to it. Routines called by the parser as the column definition</param>
///<param name="is parsed (i.e. sqlite3AddColumn()) add the new Column data to">is parsed (i.e. sqlite3AddColumn()) add the new Column data to</param>
///<param name="the copy. The copy of the Table structure is deleted by tokenize.c">the copy. The copy of the Table structure is deleted by tokenize.c</param>
///<param name="after parsing is finished.">after parsing is finished.</param>
///<param name=""></param>
///<param name="Routine sqlite3AlterFinishAddColumn() will be called to complete">Routine sqlite3AlterFinishAddColumn() will be called to complete</param>
///<param name="coding the "ALTER TABLE ... ADD" statement.">coding the "ALTER TABLE ... ADD" statement.</param>
*/
            public void sqlite3AlterBeginAddColumn(SrcList pSrc)
            {
                Table pNew;
                Table pTab;
                Vdbe v;
                int iDb;
                int i;
                int nAlloc;
                sqlite3 db = this.db;
                /* Look up the table being altered. */
                Debug.Assert(this.pNewTable == null);
                Debug.Assert(sqlite3BtreeHoldsAllMutexes(db));
                //      if ( db.mallocFailed != 0 ) goto exit_begin_add_column;
                pTab = sqlite3LocateTable(this, 0, pSrc.a[0].zName, pSrc.a[0].zDatabase);
                if (pTab == null)
                    goto exit_begin_add_column;
                if (IsVirtual(pTab))
                {
                    sqlite3ErrorMsg(this, "virtual tables may not be altered");
                    goto exit_begin_add_column;
                }
                /* Make sure this is not an attempt to ALTER a view. */
                if (pTab.pSelect != null)
                {
                    sqlite3ErrorMsg(this, "Cannot add a column to a view");
                    goto exit_begin_add_column;
                }
                if (SQLITE_OK != this.isSystemTable(pTab.zName))
                {
                    goto exit_begin_add_column;
                }
                Debug.Assert(pTab.addColOffset > 0);
                iDb = sqlite3SchemaToIndex(db, pTab.pSchema);
                /* Put a copy of the Table struct in Parse.pNewTable for the
  ** sqlite3AddColumn() function and friends to modify.  But modify
  ** the name by adding an "sqlite_altertab_" prefix.  By adding this
  ** prefix, we insure that the name will not collide with an existing
  ** table because user table are not allowed to have the "sqlite_"
  ** prefix on their name.
  */
                pNew = new Table();
                // (Table*)sqlite3DbMallocZero( db, sizeof(Table))
                if (pNew == null)
                    goto exit_begin_add_column;
                this.pNewTable = pNew;
                pNew.nRef = 1;
                pNew.nCol = pTab.nCol;
                Debug.Assert(pNew.nCol > 0);
                nAlloc = (((pNew.nCol - 1) / 8) * 8) + 8;
                Debug.Assert(nAlloc >= pNew.nCol && nAlloc % 8 == 0 && nAlloc - pNew.nCol < 8);
                pNew.aCol = new Column[nAlloc];
                // (Column*)sqlite3DbMallocZero( db, sizeof(Column) * nAlloc );
                pNew.zName = sqlite3MPrintf(db, "sqlite_altertab_%s", pTab.zName);
                if (pNew.aCol == null || pNew.zName == null)
                {
                    //        db.mallocFailed = 1;
                    goto exit_begin_add_column;
                }
                // memcpy( pNew.aCol, pTab.aCol, sizeof(Column) * pNew.nCol );
                for (i = 0; i < pNew.nCol; i++)
                {
                    Column pCol = pTab.aCol[i].Copy();
                    // sqlite3DbStrDup( db, pCol.zName );
                    pCol.zColl = null;
                    pCol.zType = null;
                    pCol.pDflt = null;
                    pCol.zDflt = null;
                    pNew.aCol[i] = pCol;
                }
                pNew.pSchema = db.aDb[iDb].pSchema;
                pNew.addColOffset = pTab.addColOffset;
                pNew.nRef = 1;
                /* Begin a transaction and increment the schema cookie.  */
                sqlite3BeginWriteOperation(this, 0, iDb);
                v = sqlite3GetVdbe(this);
                if (v == null)
                    goto exit_begin_add_column;
                sqlite3ChangeCookie(this, iDb);
            exit_begin_add_column:
                sqlite3SrcListDelete(db, ref pSrc);
                return;
            }
            public///<summary>
                /// This function is called after an "ALTER TABLE ... ADD" statement
                /// has been parsed. Argument pColDef contains the text of the new
                /// column definition.
                ///
                /// The Table structure pParse.pNewTable was extended to include
                /// the new column during parsing.
                ///</summary>
            void sqlite3AlterFinishAddColumn(Token pColDef)
            {
                Table pNew;
                /* Copy of pParse.pNewTable */
                Table pTab;
                /* Table being altered */
                int iDb;
                /* Database number */
                string zDb;
                /* Database name */
                string zTab;
                /* Table name */
                string zCol;
                /* Null-terminated column definition */
                Column pCol;
                /* The new column */
                Expr pDflt;
                /* Default value for the new column */
                sqlite3 db;
                /* The database connection; */
                db = this.db;
                if (this.nErr != 0/*|| db.mallocFailed != 0 */)
                    return;
                pNew = this.pNewTable;
                Debug.Assert(pNew != null);
                Debug.Assert(sqlite3BtreeHoldsAllMutexes(db));
                iDb = sqlite3SchemaToIndex(db, pNew.pSchema);
                zDb = db.aDb[iDb].zName;
                zTab = pNew.zName.Substring(16);
                // zTab = &pNew->zName[16]; /* Skip the "sqlite_altertab_" prefix on the name */
                pCol = pNew.aCol[pNew.nCol - 1];
                pDflt = pCol.pDflt;
                pTab = sqlite3FindTable(db, zTab, zDb);
                Debug.Assert(pTab != null);
#if !SQLITE_OMIT_AUTHORIZATION
																																																																																																																/* Invoke the authorization callback. */
if( sqlite3AuthCheck(pParse, SQLITE_ALTER_TABLE, zDb, pTab.zName, 0) ){
return;
}
#endif
                /* If the default value for the new column was specified with a
** literal NULL, then set pDflt to 0. This simplifies checking
** for an SQL NULL default below.
*/
                if (pDflt != null && pDflt.Operator == Operator.TK_NULL)
                {
                    pDflt = null;
                }
                /* Check that the new column is not specified as PRIMARY KEY or UNIQUE.
  ** If there is a NOT NULL constraint, then the default value for the
  ** column must not be NULL.
  */
                if (pCol.isPrimKey != 0)
                {
                    sqlite3ErrorMsg(this, "Cannot add a PRIMARY KEY column");
                    return;
                }
                if (pNew.pIndex != null)
                {
                    sqlite3ErrorMsg(this, "Cannot add a UNIQUE column");
                    return;
                }
                if ((db.flags & SQLITE_ForeignKeys) != 0 && pNew.pFKey != null && pDflt != null)
                {
                    sqlite3ErrorMsg(this, "Cannot add a REFERENCES column with non-NULL default value");
                    return;
                }
                if (pCol.notNull != 0 && pDflt == null)
                {
                    sqlite3ErrorMsg(this, "Cannot add a NOT NULL column with default value NULL");
                    return;
                }
                /* Ensure the default expression is something that sqlite3ValueFromExpr()
  ** can handle (i.e. not CURRENT_TIME etc.)
  */
                if (pDflt != null)
                {
                    sqlite3_value pVal = null;
                    if (sqlite3ValueFromExpr(db, pDflt, SqliteEncoding.UTF8, SQLITE_AFF_NONE, ref pVal) != 0)
                    {
                        //        db.mallocFailed = 1;
                        return;
                    }
                    if (pVal == null)
                    {
                        sqlite3ErrorMsg(this, "Cannot add a column with non-constant default");
                        return;
                    }
                    sqlite3ValueFree(ref pVal);
                }
                /* Modify the CREATE TABLE statement. */
                zCol = pColDef.zRestSql.Substring(0, pColDef.Length).Replace(";", " ").Trim();
                //sqlite3DbStrNDup(db, (char*)pColDef.z, pColDef.n);
                if (zCol != null)
                {
                    //  char zEnd = zCol[pColDef.n-1];
                    int savedDbFlags = db.flags;
                    //      while( zEnd>zCol && (*zEnd==';' || CharExtensions.sqlite3Isspace(*zEnd)) ){
                    //    zEnd-- = '\0';
                    //  }
                    db.flags |= SQLITE_PreferBuiltin;
                    sqlite3NestedParse(this, "UPDATE \"%w\".%s SET " + "sql = substr(sql,1,%d) || ', ' || %Q || substr(sql,%d) " + "WHERE type = 'table' AND name = %Q", zDb, SCHEMA_TABLE(iDb), pNew.addColOffset, zCol, pNew.addColOffset + 1, zTab);
                    db.sqlite3DbFree(ref zCol);
                    db.flags = savedDbFlags;
                }
                /* If the default value of the new column is NULL, then set the file
  ** format to 2. If the default value of the new column is not NULL,
  ** the file format becomes 3.
  */
                this.sqlite3MinimumFileFormat(iDb, pDflt != null ? 3 : 2);
                /* Reload the schema of the modified table. */
                this.reloadTableSchema(pTab, pTab.zName);
            }
            public///<summary>
                /// Generate code to make sure the file format number is at least minFormat.
                /// The generated code will increase the file format number if necessary.
                ///</summary>
            void sqlite3MinimumFileFormat(int iDb, int minFormat)
            {
                Vdbe v;
                v = sqlite3GetVdbe(this);
                /* The VDBE should have been allocated before this routine is called.
  ** If that allocation failed, we would have quit before reaching this
  ** point */
                if (ALWAYS(v))
                {
                    int r1 = this.sqlite3GetTempReg();
                    int r2 = this.sqlite3GetTempReg();
                    int j1;
                    v.sqlite3VdbeAddOp3(OP_ReadCookie, iDb, r1, BTREE_FILE_FORMAT);
                    sqlite3VdbeUsesBtree(v, iDb);
                    v.sqlite3VdbeAddOp2(OP_Integer, minFormat, r2);
                    j1 = v.sqlite3VdbeAddOp3(OP_Ge, r2, 0, r1);
                    v.sqlite3VdbeAddOp3(OP_SetCookie, iDb, BTREE_FILE_FORMAT, r2);
                    v.sqlite3VdbeJumpHere(j1);
                    this.sqlite3ReleaseTempReg(r1);
                    this.sqlite3ReleaseTempReg(r2);
                }
            }
            public///<summary>
                /// Parameter zName is the name of a table that is about to be altered
                /// (either with ALTER TABLE ... RENAME TO or ALTER TABLE ... ADD COLUMN).
                /// If the table is a system table, this function leaves an error message
                /// in pParse->zErr (system tables may not be altered) and returns non-zero.
                ///
                /// Or, if zName is not a system table, zero is returned.
                ///</summary>
            int isSystemTable(string zName)
            {
                if (zName.StartsWith("sqlite_", System.StringComparison.InvariantCultureIgnoreCase))
                {
                    sqlite3ErrorMsg(this, "table %s may not be altered", zName);
                    return 1;
                }
                return 0;
            }
            public///<summary>
                /// Generate code to implement the "ALTER TABLE xxx RENAME TO yyy"
                /// command.
                ///</summary>
            void sqlite3AlterRenameTable(/* Parser context. */SrcList pSrc,/* The table to rename. */Token pName/* The new table name. */)
            {
                int iDb;
                /* Database that contains the table */
                string zDb;
                /* Name of database iDb */
                Table pTab;
                /* Table being renamed */
                string zName = null;
                /* NULL-terminated version of pName */
                sqlite3 db = this.db;
                /* Database connection */
                int nTabName;
                /* Number of UTF-8 characters in zTabName */
                string zTabName;
                /* Original name of the table */
                Vdbe v;
#if !SQLITE_OMIT_TRIGGER
                string zWhere = "";
                /* Where clause to locate temp triggers */
#endif
                VTable pVTab = null;
                /* Non-zero if this is a v-tab with an xRename() */
                int savedDbFlags;
                /* Saved value of db->flags */
                savedDbFlags = db.flags;
                //if ( NEVER( db.mallocFailed != 0 ) ) goto exit_rename_table;
                Debug.Assert(pSrc.nSrc == 1);
                Debug.Assert(sqlite3BtreeHoldsAllMutexes(this.db));
                pTab = sqlite3LocateTable(this, 0, pSrc.a[0].zName, pSrc.a[0].zDatabase);
                if (pTab == null)
                    goto exit_rename_table;
                iDb = sqlite3SchemaToIndex(this.db, pTab.pSchema);
                zDb = db.aDb[iDb].zName;
                db.flags |= SQLITE_PreferBuiltin;
                /* Get a NULL terminated version of the new table name. */
                zName = sqlite3NameFromToken(db, pName);
                if (zName == null)
                    goto exit_rename_table;
                /* Check that a table or index named 'zName' does not already exist
  ** in database iDb. If so, this is an error.
  */
                if (sqlite3FindTable(db, zName, zDb) != null || sqlite3FindIndex(db, zName, zDb) != null)
                {
                    sqlite3ErrorMsg(this, "there is already another table or index with this name: %s", zName);
                    goto exit_rename_table;
                }
                /* Make sure it is not a system table being altered, or a reserved name
  ** that the table is being renamed to.
  */
                if (SQLITE_OK != this.isSystemTable(pTab.zName))
                {
                    goto exit_rename_table;
                }
                if (SQLITE_OK != sqlite3CheckObjectName(this, zName))
                {
                    goto exit_rename_table;
                }
#if !SQLITE_OMIT_VIEW
                if (pTab.pSelect != null)
                {
                    sqlite3ErrorMsg(this, "view %s may not be altered", pTab.zName);
                    goto exit_rename_table;
                }
#endif
#if !SQLITE_OMIT_AUTHORIZATION
																																																																																																																/* Invoke the authorization callback. */
if( sqlite3AuthCheck(pParse, SQLITE_ALTER_TABLE, zDb, pTab.zName, 0) ){
goto exit_rename_table;
}
#endif
                if (sqlite3ViewGetColumnNames(this, pTab) != 0)
                {
                    goto exit_rename_table;
                }
#if !SQLITE_OMIT_VIRTUALTABLE
                if (IsVirtual(pTab))
                {
                    pVTab = sqlite3GetVTable(db, pTab);
                    if (pVTab.pVtab.pModule.xRename == null)
                    {
                        pVTab = null;
                    }
                }
#endif
                /* Begin a transaction and code the VerifyCookie for database iDb.
** Then modify the schema cookie (since the ALTER TABLE modifies the
** schema). Open a statement transaction if the table is a virtual
** table.
*/
                v = sqlite3GetVdbe(this);
                if (v == null)
                {
                    goto exit_rename_table;
                }
                sqlite3BeginWriteOperation(this, pVTab != null ? 1 : 0, iDb);
                sqlite3ChangeCookie(this, iDb);
                /* If this is a virtual table, invoke the xRename() function if
  ** one is defined. The xRename() callback will modify the names
  ** of any resources used by the v-table implementation (including other
  ** SQLite tables) that are identified by the name of the virtual table.
  */
#if !SQLITE_OMIT_VIRTUALTABLE
                if (pVTab != null)
                {
                    int i = ++this.nMem;
                    v.sqlite3VdbeAddOp4(OP_String8, 0, i, 0, zName, 0);
                    v.sqlite3VdbeAddOp4(OP_VRename, i, 0, 0, pVTab, P4_VTAB);
                    sqlite3MayAbort(this);
                }
#endif
                /* figure out how many UTF-8 characters are in zName */
                zTabName = pTab.zName;
                nTabName = sqlite3Utf8CharLen(zTabName, -1);
#if !(SQLITE_OMIT_FOREIGN_KEY) && !(SQLITE_OMIT_TRIGGER)
                if ((db.flags & SQLITE_ForeignKeys) != 0)
                {
                    /* If foreign-key support is enabled, rewrite the CREATE TABLE 
    ** statements corresponding to all child tables of foreign key constraints
    ** for which the renamed table is the parent table.  */
                    if ((zWhere = this.whereForeignKeys(pTab)) != null)
                    {
                        sqlite3NestedParse(this, "UPDATE \"%w\".%s SET " + "sql = sqlite_rename_parent(sql, %Q, %Q) " + "WHERE %s;", zDb, SCHEMA_TABLE(iDb), zTabName, zName, zWhere);
                        db.sqlite3DbFree(ref zWhere);
                    }
                }
#endif
                /* Modify the sqlite_master table to use the new table name. */
                sqlite3NestedParse(this, "UPDATE %Q.%s SET " +
#if SQLITE_OMIT_TRIGGER
																																																																																																																 "sql = sqlite_rename_table(sql, %Q), " +
#else
 "sql = CASE " + "WHEN type = 'trigger' THEN sqlite_rename_trigger(sql, %Q)" + "ELSE sqlite_rename_table(sql, %Q) END, " +
#endif
 "tbl_name = %Q, " + "name = CASE " + "WHEN type='table' THEN %Q " + "WHEN name LIKE 'sqlite_autoindex%%' AND type='index' THEN " + "'sqlite_autoindex_' || %Q || substr(name,%d+18) " + "ELSE name END " + "WHERE tbl_name=%Q AND " + "(type='table' OR type='index' OR type='trigger');", zDb, SCHEMA_TABLE(iDb), zName, zName, zName,
#if !SQLITE_OMIT_TRIGGER
 zName,
#endif
 zName, nTabName, zTabName);
#if !SQLITE_OMIT_AUTOINCREMENT
                /* If the sqlite_sequence table exists in this database, then update
** it with the new table name.
*/
                if (sqlite3FindTable(db, "sqlite_sequence", zDb) != null)
                {
                    sqlite3NestedParse(this, "UPDATE \"%w\".sqlite_sequence set name = %Q WHERE name = %Q", zDb, zName, pTab.zName);
                }
#endif
#if !SQLITE_OMIT_TRIGGER
                /* If there are TEMP triggers on this table, modify the sqlite_temp_master
** table. Don't do this if the table being ALTERed is itself located in
** the temp database.
*/
                if ((zWhere = this.whereTempTriggers(pTab)) != "")
                {
                    sqlite3NestedParse(this, "UPDATE sqlite_temp_master SET " + "sql = sqlite_rename_trigger(sql, %Q), " + "tbl_name = %Q " + "WHERE %s;", zName, zName, zWhere);
                    db.sqlite3DbFree(ref zWhere);
                }
#endif
#if !(SQLITE_OMIT_FOREIGN_KEY) && !(SQLITE_OMIT_TRIGGER)
                if ((db.flags & SQLITE_ForeignKeys) != 0)
                {
                    FKey p;
                    for (p = sqlite3FkReferences(pTab); p != null; p = p.pNextTo)
                    {
                        Table pFrom = p.pFrom;
                        if (pFrom != pTab)
                        {
                            this.reloadTableSchema(p.pFrom, pFrom.zName);
                        }
                    }
                }
#endif
                /* Drop and reload the internal table schema. */
                this.reloadTableSchema(pTab, zName);
            exit_rename_table:
                sqlite3SrcListDelete(db, ref pSrc);
                db.sqlite3DbFree(ref zName);
                db.flags = savedDbFlags;
            }
            public///<summary>
                /// Generate code to drop and reload the internal representation of table
                /// pTab from the database, including triggers and temporary triggers.
                /// Argument zName is the name of the table in the database schema at
                /// the time the generated code is executed. This can be different from
                /// pTab.zName if this function is being called to code part of an
                /// "ALTER TABLE RENAME TO" statement.
                ///</summary>
            void reloadTableSchema(Table pTab, string zName)
            {
                Vdbe v;
                string zWhere;
                int iDb;
                /* Index of database containing pTab */
#if !SQLITE_OMIT_TRIGGER
                Trigger pTrig;
#endif
                v = sqlite3GetVdbe(this);
                if (NEVER(v == null))
                    return;
                Debug.Assert(sqlite3BtreeHoldsAllMutexes(this.db));
                iDb = sqlite3SchemaToIndex(this.db, pTab.pSchema);
                Debug.Assert(iDb >= 0);
#if !SQLITE_OMIT_TRIGGER
                /* Drop any table triggers from the internal schema. */
                for (pTrig = sqlite3TriggerList(this, pTab); pTrig != null; pTrig = pTrig.pNext)
                {
                    int iTrigDb = sqlite3SchemaToIndex(this.db, pTrig.pSchema);
                    Debug.Assert(iTrigDb == iDb || iTrigDb == 1);
                    v.sqlite3VdbeAddOp4(OP_DropTrigger, iTrigDb, 0, 0, pTrig.zName, 0);
                }
#endif
                /* Drop the table and index from the internal schema. */
                v.sqlite3VdbeAddOp4(OP_DropTable, iDb, 0, 0, pTab.zName, 0);
                /* Reload the table, index and permanent trigger schemas. */
                zWhere = sqlite3MPrintf(this.db, "tbl_name=%Q", zName);
                if (zWhere == null)
                    return;
                sqlite3VdbeAddParseSchemaOp(v, iDb, zWhere);
#if !SQLITE_OMIT_TRIGGER
                /* Now, if the table is not stored in the temp database, reload any temp
** triggers. Don't use IN(...) in case SQLITE_OMIT_SUBQUERY is defined.
*/
                if ((zWhere = this.whereTempTriggers(pTab)) != "")
                {
                    sqlite3VdbeAddParseSchemaOp(v, 1, zWhere);
                }
#endif
            }
            public///<summary>
                /// Generate the text of a WHERE expression which can be used to select all
                /// temporary triggers on table pTab from the sqlite_temp_master table. If
                /// table pTab has no temporary triggers, or is itself stored in the
                /// temporary database, NULL is returned.
                ///</summary>
            string whereTempTriggers(Table pTab)
            {
                Trigger pTrig;
                string zWhere = "";
                Schema pTempSchema = this.db.aDb[1].pSchema;
                /* Temp db schema */
                /* If the table is not located in the temp.db (in which case NULL is
** returned, loop through the tables list of triggers. For each trigger
** that is not part of the temp.db schema, add a clause to the WHERE
** expression being built up in zWhere.
*/
                if (pTab.pSchema != pTempSchema)
                {
                    sqlite3 db = this.db;
                    for (pTrig = sqlite3TriggerList(this, pTab); pTrig != null; pTrig = pTrig.pNext)
                    {
                        if (pTrig.pSchema == pTempSchema)
                        {
                            zWhere = whereOrName(db, zWhere, pTrig.zName);
                        }
                    }
                }
                if (!String.IsNullOrEmpty(zWhere))
                {
                    zWhere = sqlite3MPrintf(this.db, "type='trigger' AND (%s)", zWhere);
                    //sqlite3DbFree( pParse.db, ref zWhere );
                    //zWhere = zNew;
                }
                return zWhere;
            }
            public///<summary>
                /// Generate the text of a WHERE expression which can be used to select all
                /// tables that have foreign key constraints that refer to table pTab (i.e.
                /// constraints for which pTab is the parent table) from the sqlite_master
                /// table.
                ///</summary>
            string whereForeignKeys(Table pTab)
            {
                FKey p;
                string zWhere = "";
                for (p = sqlite3FkReferences(pTab); p != null; p = p.pNextTo)
                {
                    zWhere = whereOrName(this.db, zWhere, p.pFrom.zName);
                }
                return zWhere;
            }
            public void openStatTable(/* Parsing context */int iDb,/* The database we are looking in */int iStatCur,/* Open the sqlite_stat1 table on this cursor */string zWhere,/* Delete entries for this table or index */string zWhereType/* Either "tbl" or "idx" */)
            {
                int[] aRoot = new int[] {
					0,
					0
				};
                u8[] aCreateTbl = new u8[] {
					0,
					0
				};
                int i;
                sqlite3 db = this.db;
                Db pDb;
                Vdbe v = sqlite3GetVdbe(this);
                if (v == null)
                    return;
                Debug.Assert(sqlite3BtreeHoldsAllMutexes(db));
                Debug.Assert(sqlite3VdbeDb(v) == db);
                pDb = db.aDb[iDb];
                for (i = 0; i < ArraySize(aTable); i++)
                {
                    string zTab = aTable[i].zName;
                    Table pStat;
                    if ((pStat = sqlite3FindTable(db, zTab, pDb.zName)) == null)
                    {
                        /* The sqlite_stat[12] table does not exist. Create it. Note that a 
      ** side-effect of the CREATE TABLE statement is to leave the rootpage 
      ** of the new table in register pParse.regRoot. This is important 
      ** because the OpenWrite opcode below will be needing it. */
                        sqlite3NestedParse(this, "CREATE TABLE %Q.%s(%s)", pDb.zName, zTab, aTable[i].zCols);
                        aRoot[i] = this.regRoot;
                        aCreateTbl[i] = 1;
                    }
                    else
                    {
                        /* The table already exists. If zWhere is not NULL, delete all entries 
      ** associated with the table zWhere. If zWhere is NULL, delete the
      ** entire contents of the table. */
                        aRoot[i] = pStat.tnum;
                        sqlite3TableLock(this, iDb, aRoot[i], 1, zTab);
                        if (!String.IsNullOrEmpty(zWhere))
                        {
                            sqlite3NestedParse(this, "DELETE FROM %Q.%s WHERE %s=%Q", pDb.zName, zTab, zWhereType, zWhere);
                        }
                        else
                        {
                            /* The sqlite_stat[12] table already exists.  Delete all rows. */
                            v.sqlite3VdbeAddOp2(OP_Clear, aRoot[i], iDb);
                        }
                    }
                }
                /* Open the sqlite_stat[12] tables for writing. */
                for (i = 0; i < ArraySize(aTable); i++)
                {
                    v.sqlite3VdbeAddOp3(OP_OpenWrite, iStatCur + i, aRoot[i], iDb);
                    v.sqlite3VdbeChangeP4(-1, 3, P4_INT32);
                    v.sqlite3VdbeChangeP5(aCreateTbl[i]);
                }
            }
            public///<summary>
                /// Generate code to do an analysis of all indices associated with
                /// a single table.
                ///</summary>
            void analyzeOneTable(/* Parser context */Table pTab,/* Table whose indices are to be analyzed */Index pOnlyIdx,/* If not NULL, only analyze this one index */int iStatCur,/* Index of VdbeCursor that writes the sqlite_stat1 table */int iMem/* Available memory locations begin here */)
            {
                sqlite3 db = this.db;
                /* Database handle */
                Index pIdx;
                /* An index to being analyzed */
                int iIdxCur;
                /* Cursor open on index being analyzed */
                Vdbe v;
                /* The virtual machine being built up */
                int i;
                /* Loop counter */
                int topOfLoop;
                /* The top of the loop */
                int endOfLoop;
                /* The end of the loop */
                int jZeroRows = -1;
                /* Jump from here if number of rows is zero */
                int iDb;
                /* Index of database containing pTab */
                int regTabname = iMem++;
                /* Register containing table name */
                int regIdxname = iMem++;
                /* Register containing index name */
                int regSampleno = iMem++;
                /* Register containing next sample number */
                int regCol = iMem++;
                /* Content of a column analyzed table */
                int regRec = iMem++;
                /* Register holding completed record */
                int regTemp = iMem++;
                /* Temporary use register */
                int regRowid = iMem++;
                /* Rowid for the inserted record */
#if SQLITE_ENABLE_STAT2
																																																																																																															  int addr = 0;                /* Instruction address */
  int regTemp2 = iMem++;       /* Temporary use register */
  int regSamplerecno = iMem++; /* Index of next sample to record */
  int regRecno = iMem++;       /* Current sample index */
  int regLast = iMem++;        /* Index of last sample to record */
  int regFirst = iMem++;       /* Index of first sample to record */
#endif
                v = sqlite3GetVdbe(this);
                if (v == null || NEVER(pTab == null))
                {
                    return;
                }
                if (pTab.tnum == 0)
                {
                    /* Do not gather statistics on views or virtual tables */
                    return;
                }
                if (pTab.zName.StartsWith("sqlite_", StringComparison.InvariantCultureIgnoreCase))
                {
                    /* Do not gather statistics on system tables */
                    return;
                }
                Debug.Assert(sqlite3BtreeHoldsAllMutexes(db));
                iDb = sqlite3SchemaToIndex(db, pTab.pSchema);
                Debug.Assert(iDb >= 0);
                Debug.Assert(sqlite3SchemaMutexHeld(db, iDb, null));
#if !SQLITE_OMIT_AUTHORIZATION
																																																																																																															if( sqlite3AuthCheck(pParse, SQLITE_ANALYZE, pTab.zName, 0,
db.aDb[iDb].zName ) ){
return;
}
#endif
                /* Establish a read-lock on the table at the shared-cache level. */
                sqlite3TableLock(this, iDb, pTab.tnum, 0, pTab.zName);
                iIdxCur = this.nTab++;
                v.sqlite3VdbeAddOp4(OP_String8, 0, regTabname, 0, pTab.zName, 0);
                for (pIdx = pTab.pIndex; pIdx != null; pIdx = pIdx.pNext)
                {
                    int nCol;
                    KeyInfo pKey;
                    if (pOnlyIdx != null && pOnlyIdx != pIdx)
                        continue;
                    nCol = pIdx.nColumn;
                    pKey = sqlite3IndexKeyinfo(this, pIdx);
                    if (iMem + 1 + (nCol * 2) > this.nMem)
                    {
                        this.nMem = iMem + 1 + (nCol * 2);
                    }
                    /* Open a cursor to the index to be analyzed. */
                    Debug.Assert(iDb == sqlite3SchemaToIndex(db, pIdx.pSchema));
                    v.sqlite3VdbeAddOp4(OP_OpenRead, iIdxCur, pIdx.tnum, iDb, pKey, P4_KEYINFO_HANDOFF);
                    VdbeComment(v, "%s", pIdx.zName);
                    /* Populate the registers containing the index names. */
                    v.sqlite3VdbeAddOp4(OP_String8, 0, regIdxname, 0, pIdx.zName, 0);
#if SQLITE_ENABLE_STAT2
																																																																																																																																											
    /* If this iteration of the loop is generating code to analyze the
** first index in the pTab.pIndex list, then register regLast has
** not been populated. In this case populate it now.  */
    if ( pTab.pIndex == pIdx )
    {
      sqlite3VdbeAddOp2( v, OP_Integer, SQLITE_INDEX_SAMPLES, regSamplerecno );
      sqlite3VdbeAddOp2( v, OP_Integer, SQLITE_INDEX_SAMPLES * 2 - 1, regTemp );
      sqlite3VdbeAddOp2( v, OP_Integer, SQLITE_INDEX_SAMPLES * 2, regTemp2 );

      sqlite3VdbeAddOp2( v, OP_Count, iIdxCur, regLast );
      sqlite3VdbeAddOp2( v, OP_Null, 0, regFirst );
      addr = sqlite3VdbeAddOp3( v, OP_Lt, regSamplerecno, 0, regLast );
      sqlite3VdbeAddOp3( v, OP_Divide, regTemp2, regLast, regFirst );
      sqlite3VdbeAddOp3( v, OP_Multiply, regLast, regTemp, regLast );
      sqlite3VdbeAddOp2( v, OP_AddImm, regLast, SQLITE_INDEX_SAMPLES * 2 - 2 );
      sqlite3VdbeAddOp3( v, OP_Divide, regTemp2, regLast, regLast );
      sqlite3VdbeJumpHere( v, addr );
    }

    /* Zero the regSampleno and regRecno registers. */
    sqlite3VdbeAddOp2( v, OP_Integer, 0, regSampleno );
    sqlite3VdbeAddOp2( v, OP_Integer, 0, regRecno );
    sqlite3VdbeAddOp2( v, OP_Copy, regFirst, regSamplerecno );
#endif
                    /* The block of memory cells initialized here is used as follows.
**
**    iMem:                
**        The total number of rows in the table.
**
**    iMem+1 .. iMem+nCol: 
**        Number of distinct entries in index considering the 
**        left-most N columns only, where N is between 1 and nCol, 
**        inclusive.
**
**    iMem+nCol+1 .. Mem+2*nCol:  
**        Previous value of indexed columns, from left to right.
**
** Cells iMem through iMem+nCol are initialized to 0. The others are 
** initialized to contain an SQL NULL.
*/
                    for (i = 0; i <= nCol; i++)
                    {
                        v.sqlite3VdbeAddOp2(OP_Integer, 0, iMem + i);
                    }
                    for (i = 0; i < nCol; i++)
                    {
                        v.sqlite3VdbeAddOp2(OP_Null, 0, iMem + nCol + i + 1);
                    }
                    /* Start the analysis loop. This loop runs through all the entries in
    ** the index b-tree.  */
                    endOfLoop = v.sqlite3VdbeMakeLabel();
                    v.sqlite3VdbeAddOp2(OP_Rewind, iIdxCur, endOfLoop);
                    topOfLoop = v.sqlite3VdbeCurrentAddr();
                    v.sqlite3VdbeAddOp2(OP_AddImm, iMem, 1);
                    for (i = 0; i < nCol; i++)
                    {
                        v.sqlite3VdbeAddOp3(OP_Column, iIdxCur, i, regCol);
                        CollSeq pColl;
                        if (i == 0)
                        {
#if SQLITE_ENABLE_STAT2
																																																																																																																																																																																																			        /* Check if the record that cursor iIdxCur points to contains a
** value that should be stored in the sqlite_stat2 table. If so,
** store it.  */
        int ne = sqlite3VdbeAddOp3( v, OP_Ne, regRecno, 0, regSamplerecno );
        Debug.Assert( regTabname + 1 == regIdxname
        && regTabname + 2 == regSampleno
        && regTabname + 3 == regCol
        );
        sqlite3VdbeChangeP5( v, SQLITE_JUMPIFNULL );
        sqlite3VdbeAddOp4( v, OP_MakeRecord, regTabname, 4, regRec, "aaab", 0 );
        sqlite3VdbeAddOp2( v, OP_NewRowid, iStatCur + 1, regRowid );
        sqlite3VdbeAddOp3( v, OP_Insert, iStatCur + 1, regRec, regRowid );

        /* Calculate new values for regSamplerecno and regSampleno.
        **
        **   sampleno = sampleno + 1
        **   samplerecno = samplerecno+(remaining records)/(remaining samples)
        */
        sqlite3VdbeAddOp2( v, OP_AddImm, regSampleno, 1 );
        sqlite3VdbeAddOp3( v, OP_Subtract, regRecno, regLast, regTemp );
        sqlite3VdbeAddOp2( v, OP_AddImm, regTemp, -1 );
        sqlite3VdbeAddOp2( v, OP_Integer, SQLITE_INDEX_SAMPLES, regTemp2 );
        sqlite3VdbeAddOp3( v, OP_Subtract, regSampleno, regTemp2, regTemp2 );
        sqlite3VdbeAddOp3( v, OP_Divide, regTemp2, regTemp, regTemp );
        sqlite3VdbeAddOp3( v, OP_Add, regSamplerecno, regTemp, regSamplerecno );

        sqlite3VdbeJumpHere( v, ne );
        sqlite3VdbeAddOp2( v, OP_AddImm, regRecno, 1 );
#endif
                            /* Always record the very first row */
                            v.sqlite3VdbeAddOp1(OP_IfNot, iMem + 1);
                        }
                        Debug.Assert(pIdx.azColl != null);
                        Debug.Assert(pIdx.azColl[i] != null);
                        pColl = sqlite3LocateCollSeq(this, pIdx.azColl[i]);
                        v.sqlite3VdbeAddOp4(OP_Ne, regCol, 0, iMem + nCol + i + 1, pColl, P4_COLLSEQ);
                        v.sqlite3VdbeChangeP5(SQLITE_NULLEQ);
                    }
                    //if( db.mallocFailed ){
                    //  /* If a malloc failure has occurred, then the result of the expression 
                    //  ** passed as the second argument to the call to sqlite3VdbeJumpHere() 
                    //  ** below may be negative. Which causes an Debug.Assert() to fail (or an
                    //  ** out-of-bounds write if SQLITE_DEBUG is not defined).  */
                    //  return;
                    //}
                    v.sqlite3VdbeAddOp2(OP_Goto, 0, endOfLoop);
                    for (i = 0; i < nCol; i++)
                    {
                        int addr2 = v.sqlite3VdbeCurrentAddr() - (nCol * 2);
                        if (i == 0)
                        {
                            v.sqlite3VdbeJumpHere(addr2 - 1);
                            /* Set jump dest for the OP_IfNot */
                        }
                        v.sqlite3VdbeJumpHere(addr2);
                        /* Set jump dest for the OP_Ne */
                        v.sqlite3VdbeAddOp2(OP_AddImm, iMem + i + 1, 1);
                        v.sqlite3VdbeAddOp3(OP_Column, iIdxCur, i, iMem + nCol + i + 1);
                    }
                    /* End of the analysis loop. */
                    v.sqlite3VdbeResolveLabel(endOfLoop);
                    v.sqlite3VdbeAddOp2(OP_Next, iIdxCur, topOfLoop);
                    v.sqlite3VdbeAddOp1(OP_Close, iIdxCur);
                    /* Store the results in sqlite_stat1.
    **
    ** The result is a single row of the sqlite_stat1 table.  The first
    ** two columns are the names of the table and index.  The third column
    ** is a string composed of a list of integer statistics about the
    ** index.  The first integer in the list is the total number of entries
    ** in the index.  There is one additional integer in the list for each
    ** column of the table.  This additional integer is a guess of how many
    ** rows of the table the index will select.  If D is the count of distinct
    ** values and K is the total number of rows, then the integer is computed
    ** as:
    **
    **        I = (K+D-1)/D
    **
    ** If K==0 then no entry is made into the sqlite_stat1 table.  
    ** If K>0 then it is always the case the D>0 so division by zero
    ** is never possible.
    */
                    v.sqlite3VdbeAddOp2(OP_SCopy, iMem, regSampleno);
                    if (jZeroRows < 0)
                    {
                        jZeroRows = v.sqlite3VdbeAddOp1(OP_IfNot, iMem);
                    }
                    for (i = 0; i < nCol; i++)
                    {
                        v.sqlite3VdbeAddOp4(OP_String8, 0, regTemp, 0, " ", 0);
                        v.sqlite3VdbeAddOp3(OP_Concat, regTemp, regSampleno, regSampleno);
                        v.sqlite3VdbeAddOp3(OP_Add, iMem, iMem + i + 1, regTemp);
                        v.sqlite3VdbeAddOp2(OP_AddImm, regTemp, -1);
                        v.sqlite3VdbeAddOp3(OP_Divide, iMem + i + 1, regTemp, regTemp);
                        v.sqlite3VdbeAddOp1(OP_ToInt, regTemp);
                        v.sqlite3VdbeAddOp3(OP_Concat, regTemp, regSampleno, regSampleno);
                    }
                    v.sqlite3VdbeAddOp4(OP_MakeRecord, regTabname, 3, regRec, "aaa", 0);
                    v.sqlite3VdbeAddOp2(OP_NewRowid, iStatCur, regRowid);
                    v.sqlite3VdbeAddOp3(OP_Insert, iStatCur, regRec, regRowid);
                    v.sqlite3VdbeChangeP5(OPFLAG_APPEND);
                }
                /* If the table has no indices, create a single sqlite_stat1 entry
  ** containing NULL as the index name and the row count as the content.
  */
                if (pTab.pIndex == null)
                {
                    v.sqlite3VdbeAddOp3(OP_OpenRead, iIdxCur, pTab.tnum, iDb);
                    VdbeComment(v, "%s", pTab.zName);
                    v.sqlite3VdbeAddOp2(OP_Count, iIdxCur, regSampleno);
                    v.sqlite3VdbeAddOp1(OP_Close, iIdxCur);
                    jZeroRows = v.sqlite3VdbeAddOp1(OP_IfNot, regSampleno);
                }
                else
                {
                    v.sqlite3VdbeJumpHere(jZeroRows);
                    jZeroRows = v.sqlite3VdbeAddOp0(OP_Goto);
                }
                v.sqlite3VdbeAddOp2(OP_Null, 0, regIdxname);
                v.sqlite3VdbeAddOp4(OP_MakeRecord, regTabname, 3, regRec, "aaa", 0);
                v.sqlite3VdbeAddOp2(OP_NewRowid, iStatCur, regRowid);
                v.sqlite3VdbeAddOp3(OP_Insert, iStatCur, regRec, regRowid);
                v.sqlite3VdbeChangeP5(OPFLAG_APPEND);
                if (this.nMem < regRec)
                    this.nMem = regRec;
                v.sqlite3VdbeJumpHere(jZeroRows);
            }
            public///<summary>
                /// Generate code that will cause the most recent index analysis to
                /// be loaded into internal hash tables where is can be used.
                ///</summary>
            void loadAnalysis(int iDb)
            {
                Vdbe v = sqlite3GetVdbe(this);
                if (v != null)
                {
                    v.sqlite3VdbeAddOp1(OP_LoadAnalysis, iDb);
                }
            }
            public///<summary>
                /// Generate code that will do an analysis of an entire database
                ///</summary>
            void analyzeDatabase(int iDb)
            {
                sqlite3 db = this.db;
                Schema pSchema = db.aDb[iDb].pSchema;
                /* Schema of database iDb */
                HashElem k;
                int iStatCur;
                int iMem;
                sqlite3BeginWriteOperation(this, 0, iDb);
                iStatCur = this.nTab;
                this.nTab += 2;
                this.openStatTable(iDb, iStatCur, null, null);
                iMem = this.nMem + 1;
                Debug.Assert(sqlite3SchemaMutexHeld(db, iDb, null));
                //for(k=sqliteHashFirst(pSchema.tblHash); k; k=sqliteHashNext(k)){
                for (k = pSchema.tblHash.first; k != null; k = k.next)
                {
                    Table pTab = (Table)k.data;
                    // sqliteHashData( k );
                    this.analyzeOneTable(pTab, null, iStatCur, iMem);
                }
                this.loadAnalysis(iDb);
            }
            /**
///<summary>
///Generate code that will do an analysis of a single table in
///a database.  If pOnlyIdx is not NULL then it is a single index
///in pTab that should be analyzed.
///</summary>
*/
            public void analyzeTable(Table pTab, Index pOnlyIdx)
            {
                int iDb;
                int iStatCur;
                Debug.Assert(pTab != null);
                Debug.Assert(sqlite3BtreeHoldsAllMutexes(this.db));
                iDb = sqlite3SchemaToIndex(this.db, pTab.pSchema);
                sqlite3BeginWriteOperation(this, 0, iDb);
                iStatCur = this.nTab;
                this.nTab += 2;
                if (pOnlyIdx != null)
                {
                    this.openStatTable(iDb, iStatCur, pOnlyIdx.zName, "idx");
                }
                else
                {
                    this.openStatTable(iDb, iStatCur, pTab.zName, "tbl");
                }
                this.analyzeOneTable(pTab, pOnlyIdx, iStatCur, this.nMem + 1);
                this.loadAnalysis(iDb);
            }
            public///<summary>
                /// Generate code for the ANALYZE command.  The parser calls this routine
                /// when it recognizes an ANALYZE command.
                ///
                ///        ANALYZE                            -- 1
                ///        ANALYZE  <database>                -- 2
                ///        ANALYZE  ?<database>.?<tablename>  -- 3
                ///
                /// Form 1 causes all indices in all attached databases to be analyzed.
                /// Form 2 analyzes all indices the single database named.
                /// Form 3 analyzes all indices associated with the named table.
                ///</summary>
                // OVERLOADS, so I don't need to rewrite parse.c
            void sqlite3Analyze(int null_2, int null_3)
            {
                this.sqlite3Analyze(null, null);
            }
            public void sqlite3Analyze(Token pName1, Token pName2)
            {
                sqlite3 db = this.db;
                int iDb;
                int i;
                string z, zDb;
                Table pTab;
                Index pIdx;
                Token pTableName = null;
                /* Read the database schema. If an error occurs, leave an error message
  ** and code in pParse and return NULL. */
                Debug.Assert(sqlite3BtreeHoldsAllMutexes(this.db));
                if (SQLITE_OK != sqlite3ReadSchema(this))
                {
                    return;
                }
                Debug.Assert(pName2 != null || pName1 == null);
                if (pName1 == null)
                {
                    /* Form 1:  Analyze everything */
                    for (i = 0; i < db.nDb; i++)
                    {
                        if (i == 1)
                            continue;
                        /* Do not analyze the TEMP database */
                        this.analyzeDatabase(i);
                    }
                }
                else
                    if (pName2.Length == 0)
                    {
                        /* Form 2:  Analyze the database or table named */
                        iDb = sqlite3FindDb(db, pName1);
                        if (iDb >= 0)
                        {
                            this.analyzeDatabase(iDb);
                        }
                        else
                        {
                            z = sqlite3NameFromToken(db, pName1);
                            if (z != null)
                            {
                                if ((pIdx = sqlite3FindIndex(db, z, null)) != null)
                                {
                                    this.analyzeTable(pIdx.pTable, pIdx);
                                }
                                else
                                    if ((pTab = sqlite3LocateTable(this, 0, z, null)) != null)
                                    {
                                        this.analyzeTable(pTab, null);
                                    }
                                z = null;
                                //sqlite3DbFree( db, z );
                            }
                        }
                    }
                    else
                    {
                        /* Form 3: Analyze the fully qualified table name */
                        iDb = sqlite3TwoPartName(this, pName1, pName2, ref pTableName);
                        if (iDb >= 0)
                        {
                            zDb = db.aDb[iDb].zName;
                            z = sqlite3NameFromToken(db, pTableName);
                            if (z != null)
                            {
                                if ((pIdx = sqlite3FindIndex(db, z, zDb)) != null)
                                {
                                    this.analyzeTable(pIdx.pTable, pIdx);
                                }
                                else
                                    if ((pTab = sqlite3LocateTable(this, 0, z, zDb)) != null)
                                    {
                                        this.analyzeTable(pTab, null);
                                    }
                                z = null;
                                //sqlite3DbFree( db, z );
                            }
                        }
                    }
            }
            /**
///<summary>
///This procedure generates VDBE code for a single invocation of either the
///sqlite_detach() or sqlite_attach() SQL user functions.
///</summary>
*/
            public void codeAttach(/* The parser context */int type,/* Either SQLITE_ATTACH or SQLITE_DETACH */FuncDef pFunc,/* FuncDef wrapper for detachFunc() or attachFunc() */Expr pAuthArg,/* Expression to pass to authorization callback */Expr pFilename,/* Name of database file */Expr pDbname,/* Name of the database to use internally */Expr pKey/* Database key for encryption extension */)
            {
                int rc;
                NameContext sName;
                Vdbe v;
                sqlite3 db = this.db;
                int regArgs;
                sName = new NameContext();
                // memset( &sName, 0, sizeof(NameContext));
                sName.pParse = this;
                if (SQLITE_OK != (rc = sName.resolveAttachExpr(pFilename)) || SQLITE_OK != (rc = sName.resolveAttachExpr(pDbname)) || SQLITE_OK != (rc = sName.resolveAttachExpr(pKey)))
                {
                    this.nErr++;
                    goto attach_end;
                }
#if !SQLITE_OMIT_AUTHORIZATION
																																																																																																														if( pAuthArg ){
char *zAuthArg;
if( pAuthArg->op==TK_STRING ){
  zAuthArg = pAuthArg->u.zToken;
}else{
  zAuthArg = 0;
}
rc = sqlite3AuthCheck(pParse, type, zAuthArg, 0, 0);
if(rc!=SQLITE_OK ){
goto attach_end;
}
}
#endif
                v = sqlite3GetVdbe(this);
                regArgs = this.sqlite3GetTempRange(4);
                this.sqlite3ExprCode(pFilename, regArgs);
                this.sqlite3ExprCode(pDbname, regArgs + 1);
                this.sqlite3ExprCode(pKey, regArgs + 2);
                Debug.Assert(v != null/*|| db.mallocFailed != 0 */);
                if (v != null)
                {
                    v.sqlite3VdbeAddOp3(OP_Function, 0, regArgs + 3 - pFunc.nArg, regArgs + 3);
                    Debug.Assert(pFunc.nArg == -1 || (pFunc.nArg & 0xff) == pFunc.nArg);
                    v.sqlite3VdbeChangeP5((u8)(pFunc.nArg));
                    v.sqlite3VdbeChangeP4(-1, pFunc, P4_FUNCDEF);
                    /* Code an OP_Expire. For an ATTACH statement, set P1 to true (expire this
    ** statement only). For DETACH, set it to false (expire all existing
    ** statements).
    */
                    v.sqlite3VdbeAddOp1(OP_Expire, (type == SQLITE_ATTACH) ? 1 : 0);
                }
            attach_end:
                sqlite3ExprDelete(db, ref pFilename);
                sqlite3ExprDelete(db, ref pDbname);
                sqlite3ExprDelete(db, ref pKey);
            }
            public void sqlite3Detach(Expr pDbname)
            {
                this.codeAttach(SQLITE_DETACH, detach_func, pDbname, null, null, pDbname);
            }
            public void sqlite3Attach(Expr p, Expr pDbname, Expr pKey)
            {
                this.codeAttach(SQLITE_ATTACH, attach_func, p, p, pDbname, pKey);
            }
            public///<summary>
                /// VDBE Calling Convention
                /// -----------------------
                ///
                /// Example:
                ///
                ///   For the following INSERT statement:
                ///
                ///     CREATE TABLE t1(a, b INTEGER PRIMARY KEY, c);
                ///     INSERT INTO t1 VALUES(1, 2, 3.1);
                ///
                ///   Register (x):        2    (type integer)
                ///   Register (x+1):      1    (type integer)
                ///   Register (x+2):      NULL (type NULL)
                ///   Register (x+3):      3.1  (type real)
                ///
                ///</summary>
                ///<summary>
                /// A foreign key constraint requires that the key columns in the parent
                /// table are collectively subject to a UNIQUE or PRIMARY KEY constraint.
                /// Given that pParent is the parent table for foreign key constraint pFKey,
                /// search the schema a unique index on the parent key columns.
                ///
                /// If successful, zero is returned. If the parent key is an INTEGER PRIMARY
                /// KEY column, then output variable *ppIdx is set to NULL. Otherwise, *ppIdx
                /// is set to point to the unique index.
                ///
                /// If the parent key consists of a single column (the foreign key constraint
                /// is not a composite foreign key), refput variable *paiCol is set to NULL.
                /// Otherwise, it is set to point to an allocated array of size N, where
                /// N is the number of columns in the parent key. The first element of the
                /// array is the index of the child table column that is mapped by the FK
                /// constraint to the parent table column stored in the left-most column
                /// of index *ppIdx. The second element of the array is the index of the
                /// child table column that corresponds to the second left-most column of
                /// *ppIdx, and so on.
                ///
                /// If the required index cannot be found, either because:
                ///
                ///   1) The named parent key columns do not exist, or
                ///
                ///   2) The named parent key columns do exist, but are not subject to a
                ///      UNIQUE or PRIMARY KEY constraint, or
                ///
                ///   3) No parent key columns were provided explicitly as part of the
                ///      foreign key definition, and the parent table does not have a
                ///      PRIMARY KEY, or
                ///
                ///   4) No parent key columns were provided explicitly as part of the
                ///      foreign key definition, and the PRIMARY KEY of the parent table
                ///      consists of a different number of columns to the child key in
                ///      the child table.
                ///
                /// then non-zero is returned, and a "foreign key mismatch" error loaded
                /// into pParse. If an OOM error occurs, non-zero is returned and the
                /// pParse.db.mallocFailed flag is set.
                ///
                ///</summary>
            int locateFkeyIndex(/* Parse context to store any error in */Table pParent,/* Parent table of FK constraint pFKey */FKey pFKey,/* Foreign key to find index for */out Index ppIdx,/* OUT: Unique index on parent table */out int[] paiCol/* OUT: Map of index columns in pFKey */)
            {
                Index pIdx = null;
                /* Value to return via *ppIdx */
                ppIdx = null;
                int[] aiCol = null;
                /* Value to return via *paiCol */
                paiCol = null;
                int nCol = pFKey.nCol;
                /* Number of columns in parent key */
                string zKey = pFKey.aCol[0].zCol;
                /* Name of left-most parent key column */
                /* The caller is responsible for zeroing output parameters. */
                //assert( ppIdx && *ppIdx==0 );
                //assert( !paiCol || *paiCol==0 );
                Debug.Assert(this != null);
                /* If this is a non-composite (single column) foreign key, check if it 
      ** maps to the INTEGER PRIMARY KEY of table pParent. If so, leave *ppIdx 
      ** and *paiCol set to zero and return early. 
      **
      ** Otherwise, for a composite foreign key (more than one column), allocate
      ** space for the aiCol array (returned via output parameter *paiCol).
      ** Non-composite foreign keys do not require the aiCol array.
      */
                if (nCol == 1)
                {
                    /* The FK maps to the IPK if any of the following are true:
        **
        **   1) There is an INTEGER PRIMARY KEY column and the FK is implicitly 
        **      mapped to the primary key of table pParent, or
        **   2) The FK is explicitly mapped to a column declared as INTEGER
        **      PRIMARY KEY.
        */
                    if (pParent.iPKey >= 0)
                    {
                        if (null == zKey)
                            return 0;
                        if (pParent.aCol[pParent.iPKey].zName.Equals(zKey, StringComparison.InvariantCultureIgnoreCase))
                            return 0;
                    }
                }
                else//if( paiCol ){
                {
                    Debug.Assert(nCol > 1);
                    aiCol = new int[nCol];
                    // (int*)sqlite3DbMallocRaw( pParse.db, nCol * sizeof( int ) );
                    //if( !aiCol ) return 1;
                    paiCol = aiCol;
                }
                for (pIdx = pParent.pIndex; pIdx != null; pIdx = pIdx.pNext)
                {
                    if (pIdx.nColumn == nCol && pIdx.onError != OE_None)
                    {
                        /* pIdx is a UNIQUE index (or a PRIMARY KEY) and has the right number
          ** of columns. If each indexed column corresponds to a foreign key
          ** column of pFKey, then this index is a winner.  */
                        if (zKey == null)
                        {
                            /* If zKey is NULL, then this foreign key is implicitly mapped to 
            ** the PRIMARY KEY of table pParent. The PRIMARY KEY index may be 
            ** identified by the test (Index.autoIndex==2).  */
                            if (pIdx.autoIndex == 2)
                            {
                                if (aiCol != null)
                                {
                                    int i;
                                    for (i = 0; i < nCol; i++)
                                        aiCol[i] = pFKey.aCol[i].iFrom;
                                }
                                break;
                            }
                        }
                        else
                        {
                            /* If zKey is non-NULL, then this foreign key was declared to
            ** map to an explicit list of columns in table pParent. Check if this
            ** index matches those columns. Also, check that the index uses
            ** the default collation sequences for each column. */
                            int i, j;
                            for (i = 0; i < nCol; i++)
                            {
                                int iCol = pIdx.aiColumn[i];
                                /* Index of column in parent tbl */
                                string zDfltColl;
                                /* Def. collation for column */
                                string zIdxCol;
                                /* Name of indexed column */
                                /* If the index uses a collation sequence that is different from
** the default collation sequence for the column, this index is
** unusable. Bail out early in this case.  */
                                zDfltColl = pParent.aCol[iCol].zColl;
                                if (String.IsNullOrEmpty(zDfltColl))
                                {
                                    zDfltColl = "BINARY";
                                }
                                if (!pIdx.azColl[i].Equals(zDfltColl, StringComparison.InvariantCultureIgnoreCase))
                                    break;
                                zIdxCol = pParent.aCol[iCol].zName;
                                for (j = 0; j < nCol; j++)
                                {
                                    if (pFKey.aCol[j].zCol.Equals(zIdxCol, StringComparison.InvariantCultureIgnoreCase))
                                    {
                                        if (aiCol != null)
                                            aiCol[i] = pFKey.aCol[j].iFrom;
                                        break;
                                    }
                                }
                                if (j == nCol)
                                    break;
                            }
                            if (i == nCol)
                                break;
                            /* pIdx is usable */
                        }
                    }
                }
                if (null == pIdx)
                {
                    if (0 == this.disableTriggers)
                    {
                        sqlite3ErrorMsg(this, "foreign key mismatch");
                    }
                    this.db.sqlite3DbFree(ref aiCol);
                    return 1;
                }
                ppIdx = pIdx;
                return 0;
            }
            public///<summary>
                /// This function is called when a row is inserted into or deleted from the
                /// child table of foreign key constraint pFKey. If an SQL UPDATE is executed
                /// on the child table of pFKey, this function is invoked twice for each row
                /// affected - once to "delete" the old row, and then again to "insert" the
                /// new row.
                ///
                /// Each time it is called, this function generates VDBE code to locate the
                /// row in the parent table that corresponds to the row being inserted into
                /// or deleted from the child table. If the parent row can be found, no
                /// special action is taken. Otherwise, if the parent row can *not* be
                /// found in the parent table:
                ///
                ///   Operation | FK type   | Action taken
                ///   --------------------------------------------------------------------------
                ///   INSERT      immediate   Increment the "immediate constraint counter".
                ///
                ///   DELETE      immediate   Decrement the "immediate constraint counter".
                ///
                ///   INSERT      deferred    Increment the "deferred constraint counter".
                ///
                ///   DELETE      deferred    Decrement the "deferred constraint counter".
                ///
                /// These operations are identified in the comment at the top of this file
                /// (fkey.c) as "I.1" and "D.1".
                ///
                ///</summary>
            void fkLookupParent(/* Parse context */int iDb,/* Index of database housing pTab */Table pTab,/* Parent table of FK pFKey */Index pIdx,/* Unique index on parent key columns in pTab */FKey pFKey,/* Foreign key constraint */int[] aiCol,/* Map from parent key columns to child table columns */int regData,/* Address of array containing child table row */int nIncr,/* Increment constraint counter by this */int isIgnore/* If true, pretend pTab contains all NULL values */)
            {
                int i;
                /* Iterator variable */
                Vdbe v = sqlite3GetVdbe(this);
                /* Vdbe to add code to */
                int iCur = this.nTab - 1;
                /* Cursor number to use */
                int iOk = v.sqlite3VdbeMakeLabel();
                /* jump here if parent key found */
                /* If nIncr is less than zero, then check at runtime if there are any
** outstanding constraints to resolve. If there are not, there is no need
** to check if deleting this row resolves any outstanding violations.
**
** Check if any of the key columns in the child table row are NULL. If 
** any are, then the constraint is considered satisfied. No need to 
** search for a matching row in the parent table.  */
                if (nIncr < 0)
                {
                    v.sqlite3VdbeAddOp2(OP_FkIfZero, pFKey.isDeferred, iOk);
                }
                for (i = 0; i < pFKey.nCol; i++)
                {
                    int iReg = aiCol[i] + regData + 1;
                    v.sqlite3VdbeAddOp2(OP_IsNull, iReg, iOk);
                }
                if (isIgnore == 0)
                {
                    if (pIdx == null)
                    {
                        /* If pIdx is NULL, then the parent key is the INTEGER PRIMARY KEY
          ** column of the parent table (table pTab).  */
                        int iMustBeInt;
                        /* Address of MustBeInt instruction */
                        int regTemp = this.sqlite3GetTempReg();
                        /* Invoke MustBeInt to coerce the child key value to an integer (i.e. 
          ** apply the affinity of the parent key). If this fails, then there
          ** is no matching parent key. Before using MustBeInt, make a copy of
          ** the value. Otherwise, the value inserted into the child key column
          ** will have INTEGER affinity applied to it, which may not be correct.  */
                        v.sqlite3VdbeAddOp2(OP_SCopy, aiCol[0] + 1 + regData, regTemp);
                        iMustBeInt = v.sqlite3VdbeAddOp2(OP_MustBeInt, regTemp, 0);
                        /* If the parent table is the same as the child table, and we are about
          ** to increment the constraint-counter (i.e. this is an INSERT operation),
          ** then check if the row being inserted matches itself. If so, do not
          ** increment the constraint-counter.  */
                        if (pTab == pFKey.pFrom && nIncr == 1)
                        {
                            v.sqlite3VdbeAddOp3(OP_Eq, regData, iOk, regTemp);
                        }
                        this.sqlite3OpenTable(iCur, iDb, pTab, OP_OpenRead);
                        v.sqlite3VdbeAddOp3(OP_NotExists, iCur, 0, regTemp);
                        v.sqlite3VdbeAddOp2(OP_Goto, 0, iOk);
                        v.sqlite3VdbeJumpHere(v.sqlite3VdbeCurrentAddr() - 2);
                        v.sqlite3VdbeJumpHere(iMustBeInt);
                        this.sqlite3ReleaseTempReg(regTemp);
                    }
                    else
                    {
                        int nCol = pFKey.nCol;
                        int regTemp = this.sqlite3GetTempRange(nCol);
                        int regRec = this.sqlite3GetTempReg();
                        KeyInfo pKey = sqlite3IndexKeyinfo(this, pIdx);
                        v.sqlite3VdbeAddOp3(OP_OpenRead, iCur, pIdx.tnum, iDb);
                        v.sqlite3VdbeChangeP4(-1, pKey, P4_KEYINFO_HANDOFF);
                        for (i = 0; i < nCol; i++)
                        {
                            v.sqlite3VdbeAddOp2(OP_Copy, aiCol[i] + 1 + regData, regTemp + i);
                        }
                        /* If the parent table is the same as the child table, and we are about
          ** to increment the constraint-counter (i.e. this is an INSERT operation),
          ** then check if the row being inserted matches itself. If so, do not
          ** increment the constraint-counter. 
          **
          ** If any of the parent-key values are NULL, then the row cannot match 
          ** itself. So set JUMPIFNULL to make sure we do the OP_Found if any
          ** of the parent-key values are NULL (at this point it is known that
          ** none of the child key values are).
          */
                        if (pTab == pFKey.pFrom && nIncr == 1)
                        {
                            int iJump = v.sqlite3VdbeCurrentAddr() + nCol + 1;
                            for (i = 0; i < nCol; i++)
                            {
                                int iChild = aiCol[i] + 1 + regData;
                                int iParent = pIdx.aiColumn[i] + 1 + regData;
                                Debug.Assert(aiCol[i] != pTab.iPKey);
                                if (pIdx.aiColumn[i] == pTab.iPKey)
                                {
                                    /* The parent key is a composite key that includes the IPK column */
                                    iParent = regData;
                                }
                                v.sqlite3VdbeAddOp3(OP_Ne, iChild, iJump, iParent);
                                v.sqlite3VdbeChangeP5(SQLITE_JUMPIFNULL);
                            }
                            v.sqlite3VdbeAddOp2(OP_Goto, 0, iOk);
                        }
                        v.sqlite3VdbeAddOp3(OP_MakeRecord, regTemp, nCol, regRec);
                        v.sqlite3VdbeChangeP4(-1, v.sqlite3IndexAffinityStr(pIdx), P4_TRANSIENT);
                        v.sqlite3VdbeAddOp4Int(OP_Found, iCur, iOk, regRec, 0);
                        this.sqlite3ReleaseTempReg(regRec);
                        this.sqlite3ReleaseTempRange(regTemp, nCol);
                    }
                }
                if (0 == pFKey.isDeferred && null == this.pToplevel && 0 == this.isMultiWrite)
                {
                    /* Special case: If this is an INSERT statement that will insert exactly
        ** one row into the table, raise a constraint immediately instead of
        ** incrementing a counter. This is necessary as the VM code is being
        ** generated for will not open a statement transaction.  */
                    Debug.Assert(nIncr == 1);
                    sqlite3HaltConstraint(this, OE_Abort, "foreign key constraint failed", P4_STATIC);
                }
                else
                {
                    if (nIncr > 0 && pFKey.isDeferred == 0)
                    {
                        sqlite3ParseToplevel(this).mayAbort = 1;
                    }
                    v.sqlite3VdbeAddOp2(OP_FkCounter, pFKey.isDeferred, nIncr);
                }
                v.sqlite3VdbeResolveLabel(iOk);
                v.sqlite3VdbeAddOp1(OP_Close, iCur);
            }
            public///<summary>
                /// This function is called to generate code executed when a row is deleted
                /// from the parent table of foreign key constraint pFKey and, if pFKey is
                /// deferred, when a row is inserted into the same table. When generating
                /// code for an SQL UPDATE operation, this function may be called twice -
                /// once to "delete" the old row and once to "insert" the new row.
                ///
                /// The code generated by this function scans through the rows in the child
                /// table that correspond to the parent table row being deleted or inserted.
                /// For each child row found, one of the following actions is taken:
                ///
                ///   Operation | FK type   | Action taken
                ///   --------------------------------------------------------------------------
                ///   DELETE      immediate   Increment the "immediate constraint counter".
                ///                           Or, if the ON (UPDATE|DELETE) action is RESTRICT,
                ///                           throw a "foreign key constraint failed" exception.
                ///
                ///   INSERT      immediate   Decrement the "immediate constraint counter".
                ///
                ///   DELETE      deferred    Increment the "deferred constraint counter".
                ///                           Or, if the ON (UPDATE|DELETE) action is RESTRICT,
                ///                           throw a "foreign key constraint failed" exception.
                ///
                ///   INSERT      deferred    Decrement the "deferred constraint counter".
                ///
                /// These operations are identified in the comment at the top of this file
                /// (fkey.c) as "I.2" and "D.2".
                ///
                ///</summary>
            void fkScanChildren(/* Parse context */SrcList pSrc,/* SrcList containing the table to scan */Table pTab, Index pIdx,/* Foreign key index */FKey pFKey,/* Foreign key relationship */int[] aiCol,/* Map from pIdx cols to child table cols */int regData,/* Referenced table data starts here */int nIncr/* Amount to increment deferred counter by */)
            {
                sqlite3 db = this.db;
                /* Database handle */
                int i;
                /* Iterator variable */
                Expr pWhere = null;
                /* WHERE clause to scan with */
                NameContext sNameContext;
                /* Context used to resolve WHERE clause */
                WhereInfo pWInfo;
                /* Context used by sqlite3WhereXXX() */
                int iFkIfZero = 0;
                /* Address of OP_FkIfZero */
                Vdbe v = sqlite3GetVdbe(this);
                Debug.Assert(null == pIdx || pIdx.pTable == pTab);
                if (nIncr < 0)
                {
                    iFkIfZero = v.sqlite3VdbeAddOp2(OP_FkIfZero, pFKey.isDeferred, 0);
                }
                /* Create an Expr object representing an SQL expression like:
      **
      **   <parent-key1> = <child-key1> AND <parent-key2> = <child-key2> ...
      **
      ** The collation sequence used for the comparison should be that of
      ** the parent key columns. The affinity of the parent key column should
      ** be applied to each child key value before the comparison takes place.
      */
                for (i = 0; i < pFKey.nCol; i++)
                {
                    Expr pLeft;
                    /* Value from parent table row */
                    Expr pRight;
                    /* Column ref to child table */
                    Expr pEq;
                    /* Expression (pLeft = pRight) */
                    int iCol;
                    /* Index of column in child table */
                    string zCol;
                    /* Name of column in child table */
                    pLeft = sqlite3Expr(db, TK_REGISTER, null);
                    if (pLeft != null)
                    {
                        /* Set the collation sequence and affinity of the LHS of each TK_EQ
          ** expression to the parent key column defaults.  */
                        if (pIdx != null)
                        {
                            Column pCol;
                            iCol = pIdx.aiColumn[i];
                            pCol = pTab.aCol[iCol];
                            if (pTab.iPKey == iCol)
                                iCol = -1;
                            pLeft.iTable = regData + iCol + 1;
                            pLeft.affinity = pCol.affinity;
                            pLeft.pColl = sqlite3LocateCollSeq(this, pCol.zColl);
                        }
                        else
                        {
                            pLeft.iTable = regData;
                            pLeft.affinity = SQLITE_AFF_INTEGER;
                        }
                    }
                    iCol = aiCol != null ? aiCol[i] : pFKey.aCol[0].iFrom;
                    Debug.Assert(iCol >= 0);
                    zCol = pFKey.pFrom.aCol[iCol].zName;
                    pRight = sqlite3Expr(db, TK_ID, zCol);
                    pEq = this.sqlite3PExpr(TK_EQ, pLeft, pRight, 0);
                    pWhere = sqlite3ExprAnd(db, pWhere, pEq);
                }
                /* If the child table is the same as the parent table, and this scan
      ** is taking place as part of a DELETE operation (operation D.2), omit the
      ** row being deleted from the scan by adding ($rowid != rowid) to the WHERE 
      ** clause, where $rowid is the rowid of the row being deleted.  */
                if (pTab == pFKey.pFrom && nIncr > 0)
                {
                    Expr pEq;
                    /* Expression (pLeft = pRight) */
                    Expr pLeft;
                    /* Value from parent table row */
                    Expr pRight;
                    /* Column ref to child table */
                    pLeft = sqlite3Expr(db, TK_REGISTER, null);
                    pRight = sqlite3Expr(db, TK_COLUMN, null);
                    if (pLeft != null && pRight != null)
                    {
                        pLeft.iTable = regData;
                        pLeft.affinity = SQLITE_AFF_INTEGER;
                        pRight.iTable = pSrc.a[0].iCursor;
                        pRight.iColumn = -1;
                    }
                    pEq = this.sqlite3PExpr(TK_NE, pLeft, pRight, 0);
                    pWhere = sqlite3ExprAnd(db, pWhere, pEq);
                }
                /* Resolve the references in the WHERE clause. */
                sNameContext = new NameContext();
                // memset( &sNameContext, 0, sizeof( NameContext ) );
                sNameContext.pSrcList = pSrc;
                sNameContext.pParse = this;
                sqlite3ResolveExprNames(sNameContext, ref pWhere);
                /* Create VDBE to loop through the entries in pSrc that match the WHERE
      ** clause. If the constraint is not deferred, throw an exception for
      ** each row found. Otherwise, for deferred constraints, increment the
      ** deferred constraint counter by nIncr for each row selected.  */
                ExprList elDummy = null;
                pWInfo = this.sqlite3WhereBegin(pSrc, pWhere, ref elDummy, 0);
                if (nIncr > 0 && pFKey.isDeferred == 0)
                {
                    sqlite3ParseToplevel(this).mayAbort = 1;
                }
                v.sqlite3VdbeAddOp2(OP_FkCounter, pFKey.isDeferred, nIncr);
                if (pWInfo != null)
                {
                    pWInfo.sqlite3WhereEnd();
                }
                /* Clean up the WHERE clause constructed above. */
                sqlite3ExprDelete(db, ref pWhere);
                if (iFkIfZero != 0)
                {
                    v.sqlite3VdbeJumpHere(iFkIfZero);
                }
            }
            public///<summary>
                /// This function is called to generate code that runs when table pTab is
                /// being dropped from the database. The SrcList passed as the second argument
                /// to this function contains a single entry guaranteed to resolve to
                /// table pTab.
                ///
                /// Normally, no code is required. However, if either
                ///
                ///   (a) The table is the parent table of a FK constraint, or
                ///   (b) The table is the child table of a deferred FK constraint and it is
                ///       determined at runtime that there are outstanding deferred FK
                ///       constraint violations in the database,
                ///
                /// then the equivalent of "DELETE FROM <tbl>" is executed before dropping
                /// the table from the database. Triggers are disabled while running this
                /// DELETE, but foreign key actions are not.
                ///
                ///</summary>
            void sqlite3FkDropTable(SrcList pName, Table pTab)
            {
                sqlite3 db = this.db;
                if ((db.flags & SQLITE_ForeignKeys) != 0 && !IsVirtual(pTab) && null == pTab.pSelect)
                {
                    int iSkip = 0;
                    Vdbe v = sqlite3GetVdbe(this);
                    Debug.Assert(v != null);
                    /* VDBE has already been allocated */
                    if (sqlite3FkReferences(pTab) == null)
                    {
                        /* Search for a deferred foreign key constraint for which this table
          ** is the child table. If one cannot be found, return without 
          ** generating any VDBE code. If one can be found, then jump over
          ** the entire DELETE if there are no outstanding deferred constraints
          ** when this statement is run.  */
                        FKey p;
                        for (p = pTab.pFKey; p != null; p = p.pNextFrom)
                        {
                            if (p.isDeferred != 0)
                                break;
                        }
                        if (null == p)
                            return;
                        iSkip = v.sqlite3VdbeMakeLabel();
                        v.sqlite3VdbeAddOp2(OP_FkIfZero, 1, iSkip);
                    }
                    this.disableTriggers = 1;
                    this.sqlite3DeleteFrom(sqlite3SrcListDup(db, pName, 0), null);
                    this.disableTriggers = 0;
                    /* If the DELETE has generated immediate foreign key constraint 
        ** violations, halt the VDBE and return an error at this point, before
        ** any modifications to the schema are made. This is because statement
        ** transactions are not able to rollback schema changes.  */
                    v.sqlite3VdbeAddOp2(OP_FkIfZero, 0, v.sqlite3VdbeCurrentAddr() + 2);
                    sqlite3HaltConstraint(this, OE_Abort, "foreign key constraint failed", P4_STATIC);
                    if (iSkip != 0)
                    {
                        v.sqlite3VdbeResolveLabel(iSkip);
                    }
                }
            }
            public///<summary>
                /// This function is called when inserting, deleting or updating a row of
                /// table pTab to generate VDBE code to perform foreign key constraint
                /// processing for the operation.
                ///
                /// For a DELETE operation, parameter regOld is passed the index of the
                /// first register in an array of (pTab.nCol+1) registers containing the
                /// rowid of the row being deleted, followed by each of the column values
                /// of the row being deleted, from left to right. Parameter regNew is passed
                /// zero in this case.
                ///
                /// For an INSERT operation, regOld is passed zero and regNew is passed the
                /// first register of an array of (pTab.nCol+1) registers containing the new
                /// row data.
                ///
                /// For an UPDATE operation, this function is called twice. Once before
                /// the original record is deleted from the table using the calling convention
                /// described for DELETE. Then again after the original record is deleted
                /// but before the new record is inserted using the INSERT convention.
                ///
                ///</summary>
            void sqlite3FkCheck(/* Parse context */Table pTab,/* Row is being deleted from this table */int regOld,/* Previous row data is stored here */int regNew/* New row data is stored here */)
            {
                sqlite3 db = this.db;
                /* Database handle */
                FKey pFKey;
                /* Used to iterate through FKs */
                int iDb;
                /* Index of database containing pTab */
                string zDb;
                /* Name of database containing pTab */
                int isIgnoreErrors = this.disableTriggers;
                /* Exactly one of regOld and regNew should be non-zero. */
                Debug.Assert((regOld == 0) != (regNew == 0));
                /* If foreign-keys are disabled, this function is a no-op. */
                if ((db.flags & SQLITE_ForeignKeys) == 0)
                    return;
                iDb = sqlite3SchemaToIndex(db, pTab.pSchema);
                zDb = db.aDb[iDb].zName;
                /* Loop through all the foreign key constraints for which pTab is the
      ** child table (the table that the foreign key definition is part of).  */
                for (pFKey = pTab.pFKey; pFKey != null; pFKey = pFKey.pNextFrom)
                {
                    Table pTo;
                    /* Parent table of foreign key pFKey */
                    Index pIdx = null;
                    /* Index on key columns in pTo */
                    int[] aiFree = null;
                    int[] aiCol;
                    int iCol;
                    int i;
                    int isIgnore = 0;
                    /* Find the parent table of this foreign key. Also find a unique index 
        ** on the parent key columns in the parent table. If either of these 
        ** schema items cannot be located, set an error in pParse and return 
        ** early.  */
                    if (this.disableTriggers != 0)
                    {
                        pTo = sqlite3FindTable(db, pFKey.zTo, zDb);
                    }
                    else
                    {
                        pTo = sqlite3LocateTable(this, 0, pFKey.zTo, zDb);
                    }
                    if (null == pTo || this.locateFkeyIndex(pTo, pFKey, out pIdx, out aiFree) != 0)
                    {
                        if (0 == isIgnoreErrors/* || db.mallocFailed */)
                            return;
                        continue;
                    }
                    Debug.Assert(pFKey.nCol == 1 || (aiFree != null && pIdx != null));
                    if (aiFree != null)
                    {
                        aiCol = aiFree;
                    }
                    else
                    {
                        iCol = pFKey.aCol[0].iFrom;
                        aiCol = new int[1];
                        aiCol[0] = iCol;
                    }
                    for (i = 0; i < pFKey.nCol; i++)
                    {
                        if (aiCol[i] == pTab.iPKey)
                        {
                            aiCol[i] = -1;
                        }
#if !SQLITE_OMIT_AUTHORIZATION
																																																																																																																																																																			      /* Request permission to read the parent key columns. If the 
      ** authorization callback returns SQLITE_IGNORE, behave as if any
      ** values read from the parent table are NULL. */
      if( db.xAuth ){
        int rcauth;
        char *zCol = pTo.aCol[pIdx ? pIdx.aiColumn[i] : pTo.iPKey].zName;
        rcauth = sqlite3AuthReadCol(pParse, pTo.zName, zCol, iDb);
        isIgnore = (rcauth==SQLITE_IGNORE);
      }
#endif
                    }
                    /* Take a shared-cache advisory read-lock on the parent table. Allocate 
        ** a cursor to use to search the unique index on the parent key columns 
        ** in the parent table.  */
                    sqlite3TableLock(this, iDb, pTo.tnum, 0, pTo.zName);
                    this.nTab++;
                    if (regOld != 0)
                    {
                        /* A row is being removed from the child table. Search for the parent.
          ** If the parent does not exist, removing the child row resolves an 
          ** outstanding foreign key constraint violation. */
                        this.fkLookupParent(iDb, pTo, pIdx, pFKey, aiCol, regOld, -1, isIgnore);
                    }
                    if (regNew != 0)
                    {
                        /* A row is being added to the child table. If a parent row cannot
          ** be found, adding the child row has violated the FK constraint. */
                        this.fkLookupParent(iDb, pTo, pIdx, pFKey, aiCol, regNew, +1, isIgnore);
                    }
                    db.sqlite3DbFree(ref aiFree);
                }
                /* Loop through all the foreign key constraints that refer to this table */
                for (pFKey = sqlite3FkReferences(pTab); pFKey != null; pFKey = pFKey.pNextTo)
                {
                    Index pIdx = null;
                    /* Foreign key index for pFKey */
                    SrcList pSrc;
                    int[] aiCol = null;
                    if (0 == pFKey.isDeferred && null == this.pToplevel && 0 == this.isMultiWrite)
                    {
                        Debug.Assert(regOld == 0 && regNew != 0);
                        /* Inserting a single row into a parent table cannot cause an immediate
          ** foreign key violation. So do nothing in this case.  */
                        continue;
                    }
                    if (this.locateFkeyIndex(pTab, pFKey, out pIdx, out aiCol) != 0)
                    {
                        if (0 == isIgnoreErrors/*|| db.mallocFailed */)
                            return;
                        continue;
                    }
                    Debug.Assert(aiCol != null || pFKey.nCol == 1);
                    /* Create a SrcList structure containing a single table (the table 
        ** the foreign key that refers to this table is attached to). This
        ** is required for the sqlite3WhereXXX() interface.  */
                    pSrc = sqlite3SrcListAppend(db, 0, null, null);
                    if (pSrc != null)
                    {
                        SrcList_item pItem = pSrc.a[0];
                        pItem.pTab = pFKey.pFrom;
                        pItem.zName = pFKey.pFrom.zName;
                        pItem.pTab.nRef++;
                        pItem.iCursor = this.nTab++;
                        if (regNew != 0)
                        {
                            this.fkScanChildren(pSrc, pTab, pIdx, pFKey, aiCol, regNew, -1);
                        }
                        if (regOld != 0)
                        {
                            /* If there is a RESTRICT action configured for the current operation
            ** on the parent table of this FK, then throw an exception 
            ** immediately if the FK constraint is violated, even if this is a
            ** deferred trigger. That's what RESTRICT means. To defer checking
            ** the constraint, the FK should specify NO ACTION (represented
            ** using OE_None). NO ACTION is the default.  */
                            this.fkScanChildren(pSrc, pTab, pIdx, pFKey, aiCol, regOld, 1);
                        }
                        pItem.zName = null;
                        sqlite3SrcListDelete(db, ref pSrc);
                    }
                    db.sqlite3DbFree(ref aiCol);
                }
            }
            public///<summary>
                /// This function is called before generating code to update or delete a
                /// row contained in table pTab.
                ///
                ///</summary>
            u32 sqlite3FkOldmask(/* Parse context */Table pTab/* Table being modified */)
            {
                u32 mask = 0;
                if ((this.db.flags & SQLITE_ForeignKeys) != 0)
                {
                    FKey p;
                    int i;
                    for (p = pTab.pFKey; p != null; p = p.pNextFrom)
                    {
                        for (i = 0; i < p.nCol; i++)
                            mask |= COLUMN_MASK(p.aCol[i].iFrom);
                    }
                    for (p = sqlite3FkReferences(pTab); p != null; p = p.pNextTo)
                    {
                        Index pIdx;
                        int[] iDummy;
                        this.locateFkeyIndex(pTab, p, out pIdx, out iDummy);
                        if (pIdx != null)
                        {
                            for (i = 0; i < pIdx.nColumn; i++)
                                mask |= COLUMN_MASK(pIdx.aiColumn[i]);
                        }
                    }
                }
                return mask;
            }
            public///<summary>
                /// This function is called before generating code to update or delete a
                /// row contained in table pTab. If the operation is a DELETE, then
                /// parameter aChange is passed a NULL value. For an UPDATE, aChange points
                /// to an array of size N, where N is the number of columns in table pTab.
                /// If the i'th column is not modified by the UPDATE, then the corresponding
                /// entry in the aChange[] array is set to -1. If the column is modified,
                /// the value is 0 or greater. Parameter chngRowid is set to true if the
                /// UPDATE statement modifies the rowid fields of the table.
                ///
                /// If any foreign key processing will be required, this function returns
                /// true. If there is no foreign key related processing, this function
                /// returns false.
                ///
                ///</summary>
            int sqlite3FkRequired(/* Parse context */Table pTab,/* Table being modified */int[] aChange,/* Non-NULL for UPDATE operations */int chngRowid/* True for UPDATE that affects rowid */)
            {
                if ((this.db.flags & SQLITE_ForeignKeys) != 0)
                {
                    if (null == aChange)
                    {
                        /* A DELETE operation. Foreign key processing is required if the 
          ** table in question is either the child or parent table for any 
          ** foreign key constraint.  */
                        return (sqlite3FkReferences(pTab) != null || pTab.pFKey != null) ? 1 : 0;
                    }
                    else
                    {
                        /* This is an UPDATE. Foreign key processing is only required if the
          ** operation modifies one or more child or parent key columns. */
                        int i;
                        FKey p;
                        /* Check if any child key columns are being modified. */
                        for (p = pTab.pFKey; p != null; p = p.pNextFrom)
                        {
                            for (i = 0; i < p.nCol; i++)
                            {
                                int iChildKey = p.aCol[i].iFrom;
                                if (aChange[iChildKey] >= 0)
                                    return 1;
                                if (iChildKey == pTab.iPKey && chngRowid != 0)
                                    return 1;
                            }
                        }
                        /* Check if any parent key columns are being modified. */
                        for (p = sqlite3FkReferences(pTab); p != null; p = p.pNextTo)
                        {
                            for (i = 0; i < p.nCol; i++)
                            {
                                string zKey = p.aCol[i].zCol;
                                int iKey;
                                for (iKey = 0; iKey < pTab.nCol; iKey++)
                                {
                                    Column pCol = pTab.aCol[iKey];
                                    if ((!String.IsNullOrEmpty(zKey) ? pCol.zName.Equals(zKey, StringComparison.InvariantCultureIgnoreCase) : pCol.isPrimKey != 0))
                                    {
                                        if (aChange[iKey] >= 0)
                                            return 1;
                                        if (iKey == pTab.iPKey && chngRowid != 0)
                                            return 1;
                                    }
                                }
                            }
                        }
                    }
                }
                return 0;
            }
            public///<summary>
                /// This function is called when an UPDATE or DELETE operation is being
                /// compiled on table pTab, which is the parent table of foreign-key pFKey.
                /// If the current operation is an UPDATE, then the pChanges parameter is
                /// passed a pointer to the list of columns being modified. If it is a
                /// DELETE, pChanges is passed a NULL pointer.
                ///
                /// It returns a pointer to a Trigger structure containing a trigger
                /// equivalent to the ON UPDATE or ON DELETE action specified by pFKey.
                /// If the action is "NO ACTION" or "RESTRICT", then a NULL pointer is
                /// returned (these actions require no special handling by the triggers
                /// sub-system, code for them is created by fkScanChildren()).
                ///
                /// For example, if pFKey is the foreign key and pTab is table "p" in
                /// the following schema:
                ///
                ///   CREATE TABLE p(pk PRIMARY KEY);
                ///   CREATE TABLE c(ck REFERENCES p ON DELETE CASCADE);
                ///
                /// then the returned trigger structure is equivalent to:
                ///
                ///   CREATE TRIGGER ... DELETE ON p BEGIN
                ///     DELETE FROM c WHERE ck = old.pk;
                ///   END;
                ///
                /// The returned pointer is cached as part of the foreign key object. It
                /// is eventually freed along with the rest of the foreign key object by
                /// sqlite3FkDelete().
                ///
                ///</summary>
            Trigger fkActionTrigger(/* Parse context */Table pTab,/* Table being updated or deleted from */FKey pFKey,/* Foreign key to get action for */ExprList pChanges/* Change-list for UPDATE, NULL for DELETE */)
            {
                sqlite3 db = this.db;
                /* Database handle */
                int action;
                /* One of OE_None, OE_Cascade etc. */
                Trigger pTrigger;
                /* Trigger definition to return */
                int iAction = (pChanges != null) ? 1 : 0;
                /* 1 for UPDATE, 0 for DELETE */
                action = pFKey.aAction[iAction];
                pTrigger = pFKey.apTrigger[iAction];
                if (action != OE_None && null == pTrigger)
                {
                    u8 enableLookaside;
                    /* Copy of db.lookaside.bEnabled */
                    string zFrom;
                    /* Name of child table */
                    int nFrom;
                    /* Length in bytes of zFrom */
                    Index pIdx = null;
                    /* Parent key index for this FK */
                    int[] aiCol = null;
                    /* child table cols . parent key cols */
                    TriggerStep pStep = null;
                    /* First (only) step of trigger program */
                    Expr pWhere = null;
                    /* WHERE clause of trigger step */
                    ExprList pList = null;
                    /* Changes list if ON UPDATE CASCADE */
                    Select pSelect = null;
                    /* If RESTRICT, "SELECT RAISE(...)" */
                    int i;
                    /* Iterator variable */
                    Expr pWhen = null;
                    /* WHEN clause for the trigger */
                    if (this.locateFkeyIndex(pTab, pFKey, out pIdx, out aiCol) != 0)
                        return null;
                    Debug.Assert(aiCol != null || pFKey.nCol == 1);
                    for (i = 0; i < pFKey.nCol; i++)
                    {
                        Token tOld = new Token("old", 3);
                        /* Literal "old" token */
                        Token tNew = new Token("new", 3);
                        /* Literal "new" token */
                        Token tFromCol = new Token();
                        /* Name of column in child table */
                        Token tToCol = new Token();
                        /* Name of column in parent table */
                        int iFromCol;
                        /* Idx of column in child table */
                        Expr pEq;
                        /* tFromCol = OLD.tToCol */
                        iFromCol = aiCol != null ? aiCol[i] : pFKey.aCol[0].iFrom;
                        Debug.Assert(iFromCol >= 0);
                        tToCol.zRestSql = pIdx != null ? pTab.aCol[pIdx.aiColumn[i]].zName : "oid";
                        tFromCol.zRestSql = pFKey.pFrom.aCol[iFromCol].zName;
                        tToCol.Length = StringExtensions.sqlite3Strlen30(tToCol.zRestSql);
                        tFromCol.Length = StringExtensions.sqlite3Strlen30(tFromCol.zRestSql);
                        /* Create the expression "OLD.zToCol = zFromCol". It is important
          ** that the "OLD.zToCol" term is on the LHS of the = operator, so
          ** that the affinity and collation sequence associated with the
          ** parent table are used for the comparison. */
                        pEq = this.sqlite3PExpr(TK_EQ, this.sqlite3PExpr(TK_DOT, this.sqlite3PExpr(TK_ID, null, null, tOld), this.sqlite3PExpr(TK_ID, null, null, tToCol), 0), this.sqlite3PExpr(TK_ID, null, null, tFromCol), 0);
                        pWhere = sqlite3ExprAnd(db, pWhere, pEq);
                        /* For ON UPDATE, construct the next term of the WHEN clause.
          ** The final WHEN clause will be like this:
          **
          **    WHEN NOT(old.col1 IS new.col1 AND ... AND old.colN IS new.colN)
          */
                        if (pChanges != null)
                        {
                            pEq = this.sqlite3PExpr(TK_IS, this.sqlite3PExpr(TK_DOT, this.sqlite3PExpr(TK_ID, null, null, tOld), this.sqlite3PExpr(TK_ID, null, null, tToCol), 0), this.sqlite3PExpr(TK_DOT, this.sqlite3PExpr(TK_ID, null, null, tNew), this.sqlite3PExpr(TK_ID, null, null, tToCol), 0), 0);
                            pWhen = sqlite3ExprAnd(db, pWhen, pEq);
                        }
                        if (action != OE_Restrict && (action != OE_Cascade || pChanges != null))
                        {
                            Expr pNew;
                            if (action == OE_Cascade)
                            {
                                pNew = this.sqlite3PExpr(TK_DOT, this.sqlite3PExpr(TK_ID, null, null, tNew), this.sqlite3PExpr(TK_ID, null, null, tToCol), 0);
                            }
                            else
                                if (action == OE_SetDflt)
                                {
                                    Expr pDflt = pFKey.pFrom.aCol[iFromCol].pDflt;
                                    if (pDflt != null)
                                    {
                                        pNew = sqlite3ExprDup(db, pDflt, 0);
                                    }
                                    else
                                    {
                                        pNew = this.sqlite3PExpr(TK_NULL, 0, 0, 0);
                                    }
                                }
                                else
                                {
                                    pNew = this.sqlite3PExpr(TK_NULL, 0, 0, 0);
                                }
                            pList = this.sqlite3ExprListAppend(pList, pNew);
                            this.sqlite3ExprListSetName(pList, tFromCol, 0);
                        }
                    }
                    db.sqlite3DbFree(ref aiCol);
                    zFrom = pFKey.pFrom.zName;
                    nFrom = StringExtensions.sqlite3Strlen30(zFrom);
                    if (action == OE_Restrict)
                    {
                        Token tFrom = new Token();
                        Expr pRaise;
                        tFrom.zRestSql = zFrom;
                        tFrom.Length = nFrom;
                        pRaise = sqlite3Expr(db, TK_RAISE, "foreign key constraint failed");
                        if (pRaise != null)
                        {
                            pRaise.affinity = (char)OE_Abort;
                        }
                        pSelect = sqlite3SelectNew(this, this.sqlite3ExprListAppend(0, pRaise), sqlite3SrcListAppend(db, 0, tFrom, null), pWhere, null, null, null, 0, null, null);
                        pWhere = null;
                    }
                    /* Disable lookaside memory allocation */
                    enableLookaside = db.lookaside.bEnabled;
                    db.lookaside.bEnabled = 0;
                    pTrigger = new Trigger();
                    //(Trigger*)sqlite3DbMallocZero( db,
                    //     sizeof( Trigger ) +         /* struct Trigger */
                    //     sizeof( TriggerStep ) +     /* Single step in trigger program */
                    //     nFrom + 1                 /* Space for pStep.target.z */
                    // );
                    //if ( pTrigger )
                    {
                        pStep = pTrigger.step_list = new TriggerStep();
                        // = (TriggerStep)pTrigger[1];
                        //pStep.target.z = pStep[1];
                        pStep.target.Length = nFrom;
                        pStep.target.zRestSql = zFrom;
                        // memcpy( (char*)pStep.target.z, zFrom, nFrom );
                        pStep.pWhere = sqlite3ExprDup(db, pWhere, EXPRDUP_REDUCE);
                        pStep.pExprList = sqlite3ExprListDup(db, pList, EXPRDUP_REDUCE);
                        pStep.pSelect = sqlite3SelectDup(db, pSelect, EXPRDUP_REDUCE);
                        if (pWhen != null)
                        {
                            pWhen = this.sqlite3PExpr(TK_NOT, pWhen, 0, 0);
                            pTrigger.pWhen = sqlite3ExprDup(db, pWhen, EXPRDUP_REDUCE);
                        }
                        /* Re-enable the lookaside buffer, if it was disabled earlier. */
                        //if ( db.mallocFailed == 1 )
                        //{
                        //  fkTriggerDelete( db, pTrigger );
                        //  return 0;
                        //}
                    }
                    db.lookaside.bEnabled = enableLookaside;
                    sqlite3ExprDelete(db, ref pWhere);
                    sqlite3ExprDelete(db, ref pWhen);
                    sqlite3ExprListDelete(db, ref pList);
                    sqlite3SelectDelete(db, ref pSelect);
                    switch (action)
                    {
                        case OE_Restrict:
                            pStep.op = TK_SELECT;
                            break;
                        case OE_Cascade:
                            if (null == pChanges)
                            {
                                pStep.op = TK_DELETE;
                                break;
                            }
                            goto default;
                        default:
                            pStep.op = TK_UPDATE;
                            break;
                    }
                    pStep.pTrig = pTrigger;
                    pTrigger.pSchema = pTab.pSchema;
                    pTrigger.pTabSchema = pTab.pSchema;
                    pFKey.apTrigger[iAction] = pTrigger;
                    pTrigger.op = (byte)(pChanges != null ? TK_UPDATE : TK_DELETE);
                }
                return pTrigger;
            }
            public///<summary>
                /// This function is called when deleting or updating a row to implement
                /// any required CASCADE, SET NULL or SET DEFAULT actions.
                ///
                ///</summary>
            void sqlite3FkActions(/* Parse context */Table pTab,/* Table being updated or deleted from */ExprList pChanges,/* Change-list for UPDATE, NULL for DELETE */int regOld/* Address of array containing old row */)
            {
                /* If foreign-key support is enabled, iterate through all FKs that 
      ** refer to table pTab. If there is an action a6ssociated with the FK 
      ** for this operation (either update or delete), invoke the associated 
      ** trigger sub-program.  */
                if ((this.db.flags & SQLITE_ForeignKeys) != 0)
                {
                    FKey pFKey;
                    /* Iterator variable */
                    for (pFKey = sqlite3FkReferences(pTab); pFKey != null; pFKey = pFKey.pNextTo)
                    {
                        Trigger pAction = this.fkActionTrigger(pTab, pFKey, pChanges);
                        if (pAction != null)
                        {
                            sqlite3CodeRowTriggerDirect(this, pAction, pTab, regOld, OE_Abort, 0);
                        }
                    }
                }
            }
            public int sqlite3RunParser(string zSql, ref string pzErrMsg)
            {
                Log.WriteHeader("sqlite3RunParser:"+zSql);
                Log.Indent();
                int nErr = 0;
                /* Number of errors encountered */
                int i;
                /* Loop counter */
                yyParser pEngine;
                /* type of the next token */
                int lastTokenParsed = -1;
                /* type of the previous token */
                byte enableLookaside;
                /* Saved value of db->lookaside.bEnabled */
                sqlite3 db = this.db;
                /* The database connection */
                int mxSqlLen;
                /* Max length of an SQL string */
                mxSqlLen = db.aLimit[SQLITE_LIMIT_SQL_LENGTH];
                if (db.activeVdbeCnt == 0)
                {
                    db.u1.isInterrupted = false;
                }
                this.rc = SQLITE_OK;
                this.zTail = new StringBuilder(zSql);
                i = 0;
                Debug.Assert(pzErrMsg != null);
                pEngine = sqlite3ParserAlloc();
                //sqlite3ParserAlloc((void*(*)(size_t))sqlite3Malloc);
                //if ( pEngine == null )
                //{
                //  db.mallocFailed = 1;
                //  return SQLITE_NOMEM;
                //}
                Debug.Assert(this.pNewTable == null);
                Debug.Assert(this.pNewTrigger == null);
                Debug.Assert(this.nVar == 0);
                Debug.Assert(this.nzVar == 0);
                Debug.Assert(this.azVar == null);
                enableLookaside = db.lookaside.bEnabled;
                if (db.lookaside.pStart != 0)
                    db.lookaside.bEnabled = 1;
                while (/*  0 == db.mallocFailed && */i < zSql.Length)
                {
                    Debug.Assert(i >= 0);
                    //pParse->sLastToken.z = &zSql[i];
                    i += (this.sLastToken = Sqlite3.GetToken(zSql, i)).Length;
                    //Log.WriteLine("token :" + this.sLastToken);

                    if (i > mxSqlLen)
                    {
                        this.rc = SQLITE_TOOBIG;
                        break;
                    }
                    switch (this.sLastToken.TokenType)
                    {
                        case Operator.TK_SPACE:
                            {
                                if (db.u1.isInterrupted)
                                {
                                    sqlite3ErrorMsg(this, "interrupt");
                                    this.rc = SQLITE_INTERRUPT;
                                    goto abort_parse;
                                }
                                break;
                            }
                        case Operator.TK_ILLEGAL:
                            {
                                db.sqlite3DbFree(ref pzErrMsg);
                                pzErrMsg = sqlite3MPrintf(db, "unrecognized token: \"%T\"", (object)this.sLastToken);
                                nErr++;
                                goto abort_parse;
                            }
                        case Operator.TK_SEMI:
                            {
                                //pParse.zTail = new StringBuilder(zSql.Substring( i,zSql.Length-i ));
                                /* Fall thru into the default case */
                                goto default;
                            }
                        default:
                            {
                                pEngine.sqlite3Parser(this.sLastToken.TokenType, this.sLastToken, this);
                                if (this.rc != SQLITE_OK)
                                {
                                    goto abort_parse;
                                }
                                break;
                            }
                    }
                }
            abort_parse:
                this.zTail = new StringBuilder(zSql.Length <= i ? "" : zSql.Substring(i, zSql.Length - i));
                if (zSql.Length >= i && nErr == 0 && this.rc == SQLITE_OK)
                {
                    if (lastTokenParsed != TK_SEMI)
                    {
                        pEngine.sqlite3Parser(Operator.TK_SEMI, this.sLastToken, this);
                    }
                    pEngine.sqlite3Parser(0, this.sLastToken, this);
                }
#if YYTRACKMAXSTACKDEPTH
																																																																																										sqlite3StatusSet(SQLITE_STATUS_PARSER_STACK,
sqlite3ParserStackPeak(pEngine)
);
#endif
                pEngine.sqlite3ParserFree(null);
                //sqlite3_free );
                db.lookaside.bEnabled = enableLookaside;
                //if ( db.mallocFailed != 0 )
                //{
                //  pParse.rc = SQLITE_NOMEM;
                //}
                if (this.rc != SQLITE_OK && this.rc != SQLITE_DONE && this.zErrMsg == "")
                {
                    sqlite3SetString(ref this.zErrMsg, db, sqlite3ErrStr(this.rc));
                }
                //assert( pzErrMsg!=0 );
                if (this.zErrMsg != null)
                {
                    pzErrMsg = this.zErrMsg;
                    sqlite3_log(this.rc, "%s", pzErrMsg);
                    this.zErrMsg = "";
                    nErr++;
                }
                if (this.pVdbe != null && this.nErr > 0 && this.nested == 0)
                {
                    sqlite3VdbeDelete(ref this.pVdbe);
                    this.pVdbe = null;
                }
#if !SQLITE_OMIT_SHARED_CACHE
																																																																																										if ( pParse.nested == 0 )
{
sqlite3DbFree( db, ref pParse.aTableLock );
pParse.aTableLock = null;
pParse.nTableLock = 0;
}
#endif
#if !SQLITE_OMIT_VIRTUALTABLE
                this.apVtabLock = null;
                //sqlite3_free( pParse.apVtabLock );
#endif
                if (!IN_DECLARE_VTAB(this))
                {
                    /* If the pParse.declareVtab flag is set, do not delete any table
        ** structure built up in pParse.pNewTable. The calling code (see vtab.c)
        ** will take responsibility for freeing the Table structure.
        */
                    sqlite3DeleteTable(db, ref this.pNewTable);
                }
#if !SQLITE_OMIT_TRIGGER
                sqlite3DeleteTrigger(db, ref this.pNewTrigger);
#endif
                //for ( i = pParse.nzVar - 1; i >= 0; i-- )
                //  sqlite3DbFree( db, pParse.azVar[i] );
                db.sqlite3DbFree(ref this.azVar);
                db.sqlite3DbFree(ref this.aAlias);
                while (this.pAinc != null)
                {
                    AutoincInfo p = this.pAinc;
                    this.pAinc = p.pNext;
                    db.sqlite3DbFree(ref p);
                }
                while (this.pZombieTab != null)
                {
                    Table p = this.pZombieTab;
                    this.pZombieTab = p.pNextZombie;
                    sqlite3DeleteTable(db, ref p);
                }
                if (nErr > 0 && this.rc == SQLITE_OK)
                {
                    this.rc = SQLITE_ERROR;
                }
                Log.Unindent();
                return nErr;
            }
            public void sqlite3Insert(SrcList pTabList, int null_3, int null_4, IdList pColumn, int onError)
            {
                this.sqlite3Insert(pTabList, null, null, pColumn, onError);
            }
            public void sqlite3Insert(SrcList pTabList, int null_3, Select pSelect, IdList pColumn, int onError)
            {
                this.sqlite3Insert(pTabList, null, pSelect, pColumn, onError);
            }
            public void sqlite3Insert(SrcList pTabList, ExprList pList, int null_4, IdList pColumn, int onError)
            {
                this.sqlite3Insert(pTabList, pList, null, pColumn, onError);
            }
            public void sqlite3Insert(/* Parser context */SrcList pTabList,/* Name of table into which we are inserting */ExprList pList,/* List of values to be inserted */Select pSelect,/* A SELECT statement to use as the data source */IdList pColumn,/* Column names corresponding to IDLIST. */int onError/* How to handle constraint errors */)
            {
                sqlite3 db;
                /* The main database structure */
                Table pTab;
                /* The table to insert into.  aka TABLE */
                string zTab;
                /* Name of the table into which we are inserting */
                string zDb;
                /* Name of the database holding this table */
                int i = 0;
                int j = 0;
                int idx = 0;
                /* Loop counters */
                Vdbe v;
                /* Generate code into this virtual machine */
                Index pIdx;
                /* For looping over indices of the table */
                int nColumn;
                /* Number of columns in the data */
                int nHidden = 0;
                /* Number of hidden columns if TABLE is virtual */
                int baseCur = 0;
                /* VDBE VdbeCursor number for pTab */
                int keyColumn = -1;
                /* Column that is the INTEGER PRIMARY KEY */
                int endOfLoop = 0;
                /* Label for the end of the insertion loop */
                bool useTempTable = false;
                /* Store SELECT results in intermediate table */
                int srcTab = 0;
                /* Data comes from this temporary cursor if >=0 */
                int addrInsTop = 0;
                /* Jump to label "D" */
                int addrCont = 0;
                /* Top of insert loop. Label "C" in templates 3 and 4 */
                int addrSelect = 0;
                /* Address of coroutine that implements the SELECT */
                SelectDest dest;
                /* Destination for SELECT on rhs of INSERT */
                int iDb;
                /* Index of database holding TABLE */
                Db pDb;
                /* The database containing table being inserted into */
                bool appendFlag = false;
                /* True if the insert is likely to be an append */
                /* Register allocations */
                int regFromSelect = 0;
                /* Base register for data coming from SELECT */
                int regAutoinc = 0;
                /* Register holding the AUTOINCREMENT counter */
                int regRowCount = 0;
                /* Memory cell used for the row counter */
                int regIns;
                /* Block of regs holding rowid+data being inserted */
                int regRowid;
                /* registers holding insert rowid */
                int regData;
                /* register holding first column to insert */
                int regEof = 0;
                /* Register recording end of SELECT data */
                int[] aRegIdx = null;
                /* One register allocated to each index */
#if !SQLITE_OMIT_TRIGGER
                bool isView = false;
                /* True if attempting to insert into a view */
                Trigger pTrigger;
                /* List of triggers on pTab, if required */
                int tmask = 0;
                /* Mask of trigger times */
#endif
                db = this.db;
                dest = new SelectDest();
                // memset( &dest, 0, sizeof( dest ) );
                if (this.nErr != 0/*|| db.mallocFailed != 0 */)
                {
                    goto insert_cleanup;
                }
                /* Locate the table into which we will be inserting new information.
      */
                Debug.Assert(pTabList.nSrc == 1);
                zTab = pTabList.a[0].zName;
                if (NEVER(zTab == null))
                    goto insert_cleanup;
                pTab = this.sqlite3SrcListLookup(pTabList);
                if (pTab == null)
                {
                    goto insert_cleanup;
                }
                iDb = sqlite3SchemaToIndex(db, pTab.pSchema);
                Debug.Assert(iDb < db.nDb);
                pDb = db.aDb[iDb];
                zDb = pDb.zName;
#if !SQLITE_OMIT_AUTHORIZATION
																																																																																					if( sqlite3AuthCheck(pParse, SQLITE_INSERT, pTab.zName, 0, zDb) ){
goto insert_cleanup;
}
#endif
                /* Figure out if we have any triggers and if the table being
** inserted into is a view
*/
#if !SQLITE_OMIT_TRIGGER
                pTrigger = sqlite3TriggersExist(this, pTab, TK_INSERT, null, out tmask);
                isView = pTab.pSelect != null;
#else
																																																																																					      Trigger pTrigger = null;  // define pTrigger 0
      int tmask = 0;            // define tmask 0
      bool isView = false;
#endif
#if SQLITE_OMIT_VIEW
																																																																																					// undef isView
isView = false;
#endif
#if !SQLITE_OMIT_TRIGGER
                Debug.Assert((pTrigger != null && tmask != 0) || (pTrigger == null && tmask == 0));
#endif
#if !SQLITE_OMIT_VIEW
                /* If pTab is really a view, make sure it has been initialized.
      ** ViewGetColumnNames() is a no-op if pTab is not a view (or virtual
      ** module table).
      */
                if (sqlite3ViewGetColumnNames(this, pTab) != -0)
                {
                    goto insert_cleanup;
                }
#endif
                /* Ensure that:
      *  (a) the table is not read-only, 
      *  (b) that if it is a view then ON INSERT triggers exist
      */
                if (this.sqlite3IsReadOnly(pTab, tmask))
                {
                    goto insert_cleanup;
                }
                /* Allocate a VDBE
      */
                v = sqlite3GetVdbe(this);
                if (v == null)
                    goto insert_cleanup;
                if (this.nested == 0)
                    sqlite3VdbeCountChanges(v);
                sqlite3BeginWriteOperation(this, (pSelect != null || pTrigger != null) ? 1 : 0, iDb);
#if !SQLITE_OMIT_XFER_OPT
                /* If the statement is of the form
**
**       INSERT INTO <table1> SELECT * FROM <table2>;
**
** Then special optimizations can be applied that make the transfer
** very fast and which reduce fragmentation of indices.
**
** This is the 2nd template.
*/
                if (pColumn == null && xferOptimization(this, pTab, pSelect, onError, iDb) != 0)
                {
                    Debug.Assert(null == pTrigger);
                    Debug.Assert(pList == null);
                    goto insert_end;
                }
#endif
                /* If this is an AUTOINCREMENT table, look up the sequence number in the
** sqlite_sequence table and store it in memory cell regAutoinc.
*/
                regAutoinc = this.autoIncBegin(iDb, pTab);
                /* Figure out how many columns of data are supplied.  If the data
      ** is coming from a SELECT statement, then generate a co-routine that
      ** produces a single row of the SELECT on each invocation.  The
      ** co-routine is the common header to the 3rd and 4th templates.
      */
                if (pSelect != null)
                {
                    /* Data is coming from a SELECT.  Generate code to implement that SELECT
        ** as a co-routine.  The code is common to both the 3rd and 4th
        ** templates:
        **
        **         EOF <- 0
        **         X <- A
        **         goto B
        **      A: setup for the SELECT
        **         loop over the tables in the SELECT
        **           load value into register R..R+n
        **           yield X
        **         end loop
        **         cleanup after the SELECT
        **         EOF <- 1
        **         yield X
        **         halt-error
        **
        ** On each invocation of the co-routine, it puts a single row of the
        ** SELECT result into registers dest.iMem...dest.iMem+dest.nMem-1.
        ** (These output registers are allocated by sqlite3Select().)  When
        ** the SELECT completes, it sets the EOF flag stored in regEof.
        */
                    int rc = 0, j1;
                    regEof = ++this.nMem;
                    v.sqlite3VdbeAddOp2(OP_Integer, 0, regEof);
                    /* EOF <- 0 */
#if SQLITE_DEBUG
																																																																																																															        VdbeComment( v, "SELECT eof flag" );
#endif
                    sqlite3SelectDestInit(dest, SelectResultType.Coroutine, ++this.nMem);
                    addrSelect = v.sqlite3VdbeCurrentAddr() + 2;
                    v.sqlite3VdbeAddOp2(OP_Integer, addrSelect - 1, dest.iParm);
                    j1 = v.sqlite3VdbeAddOp2(OP_Goto, 0, 0);
#if SQLITE_DEBUG
																																																																																																															        VdbeComment( v, "Jump over SELECT coroutine" );
#endif
                    /* Resolve the expressions in the SELECT statement and execute it. */
                    rc = sqlite3Select(this, pSelect, ref dest);
                    Debug.Assert(this.nErr == 0 || rc != 0);
                    if (rc != 0 || NEVER(this.nErr != 0)/*|| db.mallocFailed != 0 */)
                    {
                        goto insert_cleanup;
                    }
                    v.sqlite3VdbeAddOp2(OP_Integer, 1, regEof);
                    /* EOF <- 1 */
                    v.sqlite3VdbeAddOp1(OP_Yield, dest.iParm);
                    /* yield X */
                    v.sqlite3VdbeAddOp2(OP_Halt, SQLITE_INTERNAL, OE_Abort);
#if SQLITE_DEBUG
																																																																																																															        VdbeComment( v, "End of SELECT coroutine" );
#endif
                    v.sqlite3VdbeJumpHere(j1);
                    /* label B: */
                    regFromSelect = dest.iMem;
                    Debug.Assert(pSelect.pEList != null);
                    nColumn = pSelect.pEList.nExpr;
                    Debug.Assert(dest.nMem == nColumn);
                    /* Set useTempTable to TRUE if the result of the SELECT statement
        ** should be written into a temporary table (template 4).  Set to
        ** FALSE if each* row of the SELECT can be written directly into
        ** the destination table (template 3).
        **
        ** A temp table must be used if the table being updated is also one
        ** of the tables being read by the SELECT statement.  Also use a
        ** temp table in the case of row triggers.
        */
                    if (pTrigger != null || this.readsTable(addrSelect, iDb, pTab))
                    {
                        useTempTable = true;
                    }
                    if (useTempTable)
                    {
                        /* Invoke the coroutine to extract information from the SELECT
          ** and add it to a transient table srcTab.  The code generated
          ** here is from the 4th template:
          **
          **      B: open temp table
          **      L: yield X
          **         if EOF goto M
          **         insert row from R..R+n into temp table
          **         goto L
          **      M: ...
          */
                        int regRec;
                        /* Register to hold packed record */
                        int regTempRowid;
                        /* Register to hold temp table ROWID */
                        int addrTop;
                        /* Label "L" */
                        int addrIf;
                        /* Address of jump to M */
                        srcTab = this.nTab++;
                        regRec = this.sqlite3GetTempReg();
                        regTempRowid = this.sqlite3GetTempReg();
                        v.sqlite3VdbeAddOp2(OP_OpenEphemeral, srcTab, nColumn);
                        addrTop = v.sqlite3VdbeAddOp1(OP_Yield, dest.iParm);
                        addrIf = v.sqlite3VdbeAddOp1(OP_If, regEof);
                        v.sqlite3VdbeAddOp3(OP_MakeRecord, regFromSelect, nColumn, regRec);
                        v.sqlite3VdbeAddOp2(OP_NewRowid, srcTab, regTempRowid);
                        v.sqlite3VdbeAddOp3(OP_Insert, srcTab, regRec, regTempRowid);
                        v.sqlite3VdbeAddOp2(OP_Goto, 0, addrTop);
                        v.sqlite3VdbeJumpHere(addrIf);
                        this.sqlite3ReleaseTempReg(regRec);
                        this.sqlite3ReleaseTempReg(regTempRowid);
                    }
                }
                else
                {
                    /* This is the case if the data for the INSERT is coming from a VALUES
        ** clause
        */
                    NameContext sNC;
                    sNC = new NameContext();
                    // memset( &sNC, 0, sNC ).Length;
                    sNC.pParse = this;
                    srcTab = -1;
                    Debug.Assert(!useTempTable);
                    nColumn = pList != null ? pList.nExpr : 0;
                    for (i = 0; i < nColumn; i++)
                    {
                        if (sqlite3ResolveExprNames(sNC, ref pList.a[i].pExpr) != 0)
                        {
                            goto insert_cleanup;
                        }
                    }
                }
                /* Make sure the number of columns in the source data matches the number
      ** of columns to be inserted into the table.
      */
                if (IsVirtual(pTab))
                {
                    for (i = 0; i < pTab.nCol; i++)
                    {
                        nHidden += (IsHiddenColumn(pTab.aCol[i]) ? 1 : 0);
                    }
                }
                if (pColumn == null && nColumn != 0 && nColumn != (pTab.nCol - nHidden))
                {
                    sqlite3ErrorMsg(this, "table %S has %d columns but %d values were supplied", pTabList, 0, pTab.nCol - nHidden, nColumn);
                    goto insert_cleanup;
                }
                if (pColumn != null && nColumn != pColumn.nId)
                {
                    sqlite3ErrorMsg(this, "%d values for %d columns", nColumn, pColumn.nId);
                    goto insert_cleanup;
                }
                /* If the INSERT statement included an IDLIST term, then make sure
      ** all elements of the IDLIST really are columns of the table and
      ** remember the column indices.
      **
      ** If the table has an INTEGER PRIMARY KEY column and that column
      ** is named in the IDLIST, then record in the keyColumn variable
      ** the index into IDLIST of the primary key column.  keyColumn is
      ** the index of the primary key as it appears in IDLIST, not as
      ** is appears in the original table.  (The index of the primary
      ** key in the original table is pTab.iPKey.)
      */
                if (pColumn != null)
                {
                    for (i = 0; i < pColumn.nId; i++)
                    {
                        pColumn.a[i].idx = -1;
                    }
                    for (i = 0; i < pColumn.nId; i++)
                    {
                        for (j = 0; j < pTab.nCol; j++)
                        {
                            if (pColumn.a[i].zName.Equals(pTab.aCol[j].zName, StringComparison.InvariantCultureIgnoreCase))
                            {
                                pColumn.a[i].idx = j;
                                if (j == pTab.iPKey)
                                {
                                    keyColumn = i;
                                }
                                break;
                            }
                        }
                        if (j >= pTab.nCol)
                        {
                            if (sqlite3IsRowid(pColumn.a[i].zName))
                            {
                                keyColumn = i;
                            }
                            else
                            {
                                sqlite3ErrorMsg(this, "table %S has no column named %s", pTabList, 0, pColumn.a[i].zName);
                                this.checkSchema = 1;
                                goto insert_cleanup;
                            }
                        }
                    }
                }
                /* If there is no IDLIST term but the table has an integer primary
      ** key, the set the keyColumn variable to the primary key column index
      ** in the original table definition.
      */
                if (pColumn == null && nColumn > 0)
                {
                    keyColumn = pTab.iPKey;
                }
                /* Initialize the count of rows to be inserted
      */
                if ((db.flags & SQLITE_CountRows) != 0)
                {
                    regRowCount = ++this.nMem;
                    v.sqlite3VdbeAddOp2(OP_Integer, 0, regRowCount);
                }
                /* If this is not a view, open the table and and all indices */
                if (!isView)
                {
                    int nIdx;
                    baseCur = this.nTab;
                    nIdx = this.sqlite3OpenTableAndIndices(pTab, baseCur, OP_OpenWrite);
                    aRegIdx = new int[nIdx + 1];
                    // sqlite3DbMallocRaw( db, sizeof( int ) * ( nIdx + 1 ) );
                    if (aRegIdx == null)
                    {
                        goto insert_cleanup;
                    }
                    for (i = 0; i < nIdx; i++)
                    {
                        aRegIdx[i] = ++this.nMem;
                    }
                }
                /* This is the top of the main insertion loop */
                if (useTempTable)
                {
                    /* This block codes the top of loop only.  The complete loop is the
        ** following pseudocode (template 4):
        **
        **         rewind temp table
        **      C: loop over rows of intermediate table
        **           transfer values form intermediate table into <table>
        **         end loop
        **      D: ...
        */
                    addrInsTop = v.sqlite3VdbeAddOp1(OP_Rewind, srcTab);
                    addrCont = v.sqlite3VdbeCurrentAddr();
                }
                else
                    if (pSelect != null)
                    {
                        /* This block codes the top of loop only.  The complete loop is the
        ** following pseudocode (template 3):
        **
        **      C: yield X
        **         if EOF goto D
        **         insert the select result into <table> from R..R+n
        **         goto C
        **      D: ...
        */
                        addrCont = v.sqlite3VdbeAddOp1(OP_Yield, dest.iParm);
                        addrInsTop = v.sqlite3VdbeAddOp1(OP_If, regEof);
                    }
                /* Allocate registers for holding the rowid of the new row,
      ** the content of the new row, and the assemblied row record.
      */
                regRowid = regIns = this.nMem + 1;
                this.nMem += pTab.nCol + 1;
                if (IsVirtual(pTab))
                {
                    regRowid++;
                    this.nMem++;
                }
                regData = regRowid + 1;
                /* Run the BEFORE and INSTEAD OF triggers, if there are any
      */
                endOfLoop = v.sqlite3VdbeMakeLabel();
#if !SQLITE_OMIT_TRIGGER
                if ((tmask & TRIGGER_BEFORE) != 0)
                {
                    int regCols = this.sqlite3GetTempRange(pTab.nCol + 1);
                    /* build the NEW.* reference row.  Note that if there is an INTEGER
        ** PRIMARY KEY into which a NULL is being inserted, that NULL will be
        ** translated into a unique ID for the row.  But on a BEFORE trigger,
        ** we do not know what the unique ID will be (because the insert has
        ** not happened yet) so we substitute a rowid of -1
        */
                    if (keyColumn < 0)
                    {
                        v.sqlite3VdbeAddOp2(OP_Integer, -1, regCols);
                    }
                    else
                    {
                        int j1;
                        if (useTempTable)
                        {
                            v.sqlite3VdbeAddOp3(OP_Column, srcTab, keyColumn, regCols);
                        }
                        else
                        {
                            Debug.Assert(pSelect == null);
                            /* Otherwise useTempTable is true */
                            this.sqlite3ExprCode(pList.a[keyColumn].pExpr, regCols);
                        }
                        j1 = v.sqlite3VdbeAddOp1(OP_NotNull, regCols);
                        v.sqlite3VdbeAddOp2(OP_Integer, -1, regCols);
                        v.sqlite3VdbeJumpHere(j1);
                        v.sqlite3VdbeAddOp1(OP_MustBeInt, regCols);
                    }
                    /* Cannot have triggers on a virtual table. If it were possible,
        ** this block would have to account for hidden column.
        */
                    Debug.Assert(!IsVirtual(pTab));
                    /* Create the new column data
        */
                    for (i = 0; i < pTab.nCol; i++)
                    {
                        if (pColumn == null)
                        {
                            j = i;
                        }
                        else
                        {
                            for (j = 0; j < pColumn.nId; j++)
                            {
                                if (pColumn.a[j].idx == i)
                                    break;
                            }
                        }
                        if ((!useTempTable && null == pList) || (pColumn != null && j >= pColumn.nId))
                        {
                            this.sqlite3ExprCode(pTab.aCol[i].pDflt, regCols + i + 1);
                        }
                        else
                            if (useTempTable)
                            {
                                v.sqlite3VdbeAddOp3(OP_Column, srcTab, j, regCols + i + 1);
                            }
                            else
                            {
                                Debug.Assert(pSelect == null);
                                /* Otherwise useTempTable is true */
                                this.sqlite3ExprCodeAndCache(pList.a[j].pExpr, regCols + i + 1);
                            }
                    }
                    /* If this is an INSERT on a view with an INSTEAD OF INSERT trigger,
        ** do not attempt any conversions before assembling the record.
        ** If this is a real table, attempt conversions as required by the
        ** table column affinities.
        */
                    if (!isView)
                    {
                        v.sqlite3VdbeAddOp2(OP_Affinity, regCols + 1, pTab.nCol);
                        v.sqlite3TableAffinityStr(pTab);
                    }
                    /* Fire BEFORE or INSTEAD OF triggers */
                    sqlite3CodeRowTrigger(this, pTrigger, TK_INSERT, null, TRIGGER_BEFORE, pTab, regCols - pTab.nCol - 1, onError, endOfLoop);
                    this.sqlite3ReleaseTempRange(regCols, pTab.nCol + 1);
                }
#endif
                /* Push the record number for the new entry onto the stack.  The
** record number is a randomly generate integer created by NewRowid
** except when the table has an INTEGER PRIMARY KEY column, in which
** case the record number is the same as that column.
*/
                if (!isView)
                {
                    if (IsVirtual(pTab))
                    {
                        /* The row that the VUpdate opcode will delete: none */
                        v.sqlite3VdbeAddOp2(OP_Null, 0, regIns);
                    }
                    if (keyColumn >= 0)
                    {
                        if (useTempTable)
                        {
                            v.sqlite3VdbeAddOp3(OP_Column, srcTab, keyColumn, regRowid);
                        }
                        else
                            if (pSelect != null)
                            {
                                v.sqlite3VdbeAddOp2(OP_SCopy, regFromSelect + keyColumn, regRowid);
                            }
                            else
                            {
                                VdbeOp pOp;
                                this.sqlite3ExprCode(pList.a[keyColumn].pExpr, regRowid);
                                pOp = v.sqlite3VdbeGetOp(-1);
                                if (ALWAYS(pOp != null) && pOp.opcode == OP_Null && !IsVirtual(pTab))
                                {
                                    appendFlag = true;
                                    pOp.opcode = OP_NewRowid;
                                    pOp.p1 = baseCur;
                                    pOp.p2 = regRowid;
                                    pOp.p3 = regAutoinc;
                                }
                            }
                        /* If the PRIMARY KEY expression is NULL, then use OP_NewRowid
          ** to generate a unique primary key value.
          */
                        if (!appendFlag)
                        {
                            int j1;
                            if (!IsVirtual(pTab))
                            {
                                j1 = v.sqlite3VdbeAddOp1(OP_NotNull, regRowid);
                                v.sqlite3VdbeAddOp3(OP_NewRowid, baseCur, regRowid, regAutoinc);
                                v.sqlite3VdbeJumpHere(j1);
                            }
                            else
                            {
                                j1 = v.sqlite3VdbeCurrentAddr();
                                v.sqlite3VdbeAddOp2(OP_IsNull, regRowid, j1 + 2);
                            }
                            v.sqlite3VdbeAddOp1(OP_MustBeInt, regRowid);
                        }
                    }
                    else
                        if (IsVirtual(pTab))
                        {
                            v.sqlite3VdbeAddOp2(OP_Null, 0, regRowid);
                        }
                        else
                        {
                            v.sqlite3VdbeAddOp3(OP_NewRowid, baseCur, regRowid, regAutoinc);
                            appendFlag = true;
                        }
                    this.autoIncStep(regAutoinc, regRowid);
                    /* Push onto the stack, data for all columns of the new entry, beginning
        ** with the first column.
        */
                    nHidden = 0;
                    for (i = 0; i < pTab.nCol; i++)
                    {
                        int iRegStore = regRowid + 1 + i;
                        if (i == pTab.iPKey)
                        {
                            /* The value of the INTEGER PRIMARY KEY column is always a NULL.
            ** Whenever this column is read, the record number will be substituted
            ** in its place.  So will fill this column with a NULL to avoid
            ** taking up data space with information that will never be used. */
                            v.sqlite3VdbeAddOp2(OP_Null, 0, iRegStore);
                            continue;
                        }
                        if (pColumn == null)
                        {
                            if (IsHiddenColumn(pTab.aCol[i]))
                            {
                                Debug.Assert(IsVirtual(pTab));
                                j = -1;
                                nHidden++;
                            }
                            else
                            {
                                j = i - nHidden;
                            }
                        }
                        else
                        {
                            for (j = 0; j < pColumn.nId; j++)
                            {
                                if (pColumn.a[j].idx == i)
                                    break;
                            }
                        }
                        if (j < 0 || nColumn == 0 || (pColumn != null && j >= pColumn.nId))
                        {
                            this.sqlite3ExprCode(pTab.aCol[i].pDflt, iRegStore);
                        }
                        else
                            if (useTempTable)
                            {
                                v.sqlite3VdbeAddOp3(OP_Column, srcTab, j, iRegStore);
                            }
                            else
                                if (pSelect != null)
                                {
                                    v.sqlite3VdbeAddOp2(OP_SCopy, regFromSelect + j, iRegStore);
                                }
                                else
                                {
                                    this.sqlite3ExprCode(pList.a[j].pExpr, iRegStore);
                                }
                    }
                    /* Generate code to check constraints and generate index keys and
        ** do the insertion.
        */
#if !SQLITE_OMIT_VIRTUALTABLE
                    if (IsVirtual(pTab))
                    {
                        VTable pVTab = sqlite3GetVTable(db, pTab);
                        this.sqlite3VtabMakeWritable(pTab);
                        v.sqlite3VdbeAddOp4(OP_VUpdate, 1, pTab.nCol + 2, regIns, pVTab, P4_VTAB);
                        v.sqlite3VdbeChangeP5((byte)(onError == OE_Default ? OE_Abort : onError));
                        sqlite3MayAbort(this);
                    }
                    else
#endif
                    {
                        int isReplace = 0;
                        /* Set to true if constraints may cause a replace */
                        this.sqlite3GenerateConstraintChecks(pTab, baseCur, regIns, aRegIdx, keyColumn >= 0 ? 1 : 0, false, onError, endOfLoop, out isReplace);
                        this.sqlite3FkCheck(pTab, 0, regIns);
                        this.sqlite3CompleteInsertion(pTab, baseCur, regIns, aRegIdx, false, appendFlag, isReplace == 0);
                    }
                }
                /* Update the count of rows that are inserted
      */
                if ((db.flags & SQLITE_CountRows) != 0)
                {
                    v.sqlite3VdbeAddOp2(OP_AddImm, regRowCount, 1);
                }
#if !SQLITE_OMIT_TRIGGER
                if (pTrigger != null)
                {
                    /* Code AFTER triggers */
                    sqlite3CodeRowTrigger(this, pTrigger, TK_INSERT, null, TRIGGER_AFTER, pTab, regData - 2 - pTab.nCol, onError, endOfLoop);
                }
#endif
                /* The bottom of the main insertion loop, if the data source
** is a SELECT statement.
*/
                v.sqlite3VdbeResolveLabel(endOfLoop);
                if (useTempTable)
                {
                    v.sqlite3VdbeAddOp2(OP_Next, srcTab, addrCont);
                    v.sqlite3VdbeJumpHere(addrInsTop);
                    v.sqlite3VdbeAddOp1(OP_Close, srcTab);
                }
                else
                    if (pSelect != null)
                    {
                        v.sqlite3VdbeAddOp2(OP_Goto, 0, addrCont);
                        v.sqlite3VdbeJumpHere(addrInsTop);
                    }
                if (!IsVirtual(pTab) && !isView)
                {
                    /* Close all tables opened */
                    v.sqlite3VdbeAddOp1(OP_Close, baseCur);
                    for (idx = 1, pIdx = pTab.pIndex; pIdx != null; pIdx = pIdx.pNext, idx++)
                    {
                        v.sqlite3VdbeAddOp1(OP_Close, idx + baseCur);
                    }
                }
            insert_end:
                /* Update the sqlite_sequence table by storing the content of the
      ** maximum rowid counter values recorded while inserting into
      ** autoincrement tables.
      */
                if (this.nested == 0 && this.pTriggerTab == null)
                {
                    this.sqlite3AutoincrementEnd();
                }
                /*
      ** Return the number of rows inserted. If this routine is
      ** generating code because of a call to sqlite3NestedParse(), do not
      ** invoke the callback function.
      */
                if ((db.flags & SQLITE_CountRows) != 0 && 0 == this.nested && null == this.pTriggerTab)
                {
                    v.sqlite3VdbeAddOp2(OP_ResultRow, regRowCount, 1);
                    sqlite3VdbeSetNumCols(v, 1);
                    sqlite3VdbeSetColName(v, 0, COLNAME_NAME, "rows inserted", SQLITE_STATIC);
                }
            insert_cleanup:
                sqlite3SrcListDelete(db, ref pTabList);
                sqlite3ExprListDelete(db, ref pList);
                sqlite3SelectDelete(db, ref pSelect);
                sqlite3IdListDelete(db, ref pColumn);
                db.sqlite3DbFree(ref aRegIdx);
            }
            public void sqlite3AutoincrementBegin()
            {
                AutoincInfo p;
                /* Information about an AUTOINCREMENT */
                sqlite3 db = this.db;
                /* The database connection */
                Db pDb;
                /* Database only autoinc table */
                int memId;
                /* Register holding max rowid */
                int addr;
                /* A VDBE address */
                Vdbe v = this.pVdbe;
                /* VDBE under construction */
                /* This routine is never called during trigger-generation.  It is
** only called from the top-level */
                Debug.Assert(this.pTriggerTab == null);
                Debug.Assert(this == sqlite3ParseToplevel(this));
                Debug.Assert(v != null);
                /* We failed long ago if this is not so */
                for (p = this.pAinc; p != null; p = p.pNext)
                {
                    pDb = db.aDb[p.iDb];
                    memId = p.regCtr;
                    Debug.Assert(sqlite3SchemaMutexHeld(db, 0, pDb.pSchema));
                    this.sqlite3OpenTable(0, p.iDb, pDb.pSchema.pSeqTab, OP_OpenRead);
                    addr = v.sqlite3VdbeCurrentAddr();
                    v.sqlite3VdbeAddOp4(OP_String8, 0, memId - 1, 0, p.pTab.zName, 0);
                    v.sqlite3VdbeAddOp2(OP_Rewind, 0, addr + 9);
                    v.sqlite3VdbeAddOp3(OP_Column, 0, 0, memId);
                    v.sqlite3VdbeAddOp3(OP_Ne, memId - 1, addr + 7, memId);
                    v.sqlite3VdbeChangeP5(SQLITE_JUMPIFNULL);
                    v.sqlite3VdbeAddOp2(OP_Rowid, 0, memId + 1);
                    v.sqlite3VdbeAddOp3(OP_Column, 0, 1, memId);
                    v.sqlite3VdbeAddOp2(OP_Goto, 0, addr + 9);
                    v.sqlite3VdbeAddOp2(OP_Next, 0, addr + 2);
                    v.sqlite3VdbeAddOp2(OP_Integer, 0, memId);
                    v.sqlite3VdbeAddOp0(OP_Close);
                }
            }
            public void sqlite3AutoincrementEnd()
            {
                AutoincInfo p;
                Vdbe v = this.pVdbe;
                sqlite3 db = this.db;
                Debug.Assert(v != null);
                for (p = this.pAinc; p != null; p = p.pNext)
                {
                    Db pDb = db.aDb[p.iDb];
                    int j1, j2, j3, j4, j5;
                    int iRec;
                    int memId = p.regCtr;
                    iRec = this.sqlite3GetTempReg();
                    Debug.Assert(sqlite3SchemaMutexHeld(db, 0, pDb.pSchema));
                    this.sqlite3OpenTable(0, p.iDb, pDb.pSchema.pSeqTab, OP_OpenWrite);
                    j1 = v.sqlite3VdbeAddOp1(OP_NotNull, memId + 1);
                    j2 = v.sqlite3VdbeAddOp0(OP_Rewind);
                    j3 = v.sqlite3VdbeAddOp3(OP_Column, 0, 0, iRec);
                    j4 = v.sqlite3VdbeAddOp3(OP_Eq, memId - 1, 0, iRec);
                    v.sqlite3VdbeAddOp2(OP_Next, 0, j3);
                    v.sqlite3VdbeJumpHere(j2);
                    v.sqlite3VdbeAddOp2(OP_NewRowid, 0, memId + 1);
                    j5 = v.sqlite3VdbeAddOp0(OP_Goto);
                    v.sqlite3VdbeJumpHere(j4);
                    v.sqlite3VdbeAddOp2(OP_Rowid, 0, memId + 1);
                    v.sqlite3VdbeJumpHere(j1);
                    v.sqlite3VdbeJumpHere(j5);
                    v.sqlite3VdbeAddOp3(OP_MakeRecord, memId - 1, 2, iRec);
                    v.sqlite3VdbeAddOp3(OP_Insert, 0, iRec, memId + 1);
                    v.sqlite3VdbeChangeP5(OPFLAG_APPEND);
                    v.sqlite3VdbeAddOp0(OP_Close);
                    this.sqlite3ReleaseTempReg(iRec);
                }
            }
            public void autoIncStep(int memId, int regRowid)
            {
                if (memId > 0)
                {
                    this.pVdbe.sqlite3VdbeAddOp2(OP_MemMax, memId, regRowid);
                }
            }
            public void sqlite3GenerateConstraintChecks(/* The parser context */Table pTab,/* the table into which we are inserting */int baseCur,/* Index of a read/write cursor pointing at pTab */int regRowid,/* Index of the range of input registers */int[] aRegIdx,/* Register used by each index.  0 for unused indices */int rowidChng,/* True if the rowid might collide with existing entry */bool isUpdate,/* True for UPDATE, False for INSERT */int overrideError,/* Override onError to this if not OE_Default */int ignoreDest,/* Jump to this label on an OE_Ignore resolution */out int pbMayReplace/* OUT: Set to true if constraint may cause a replace */)
            {
                int i;
                /* loop counter */
                Vdbe v;
                /* VDBE under constrution */
                int nCol;
                /* Number of columns */
                int onError;
                /* Conflict resolution strategy */
                int j1;
                /* Addresss of jump instruction */
                int j2 = 0, j3;
                /* Addresses of jump instructions */
                int regData;
                /* Register containing first data column */
                int iCur;
                /* Table cursor number */
                Index pIdx;
                /* Pointer to one of the indices */
                bool seenReplace = false;
                /* True if REPLACE is used to resolve INT PK conflict */
                int regOldRowid = (rowidChng != 0 && isUpdate) ? rowidChng : regRowid;
                v = sqlite3GetVdbe(this);
                Debug.Assert(v != null);
                Debug.Assert(pTab.pSelect == null);
                /* This table is not a VIEW */
                nCol = pTab.nCol;
                regData = regRowid + 1;
                /* Test all NOT NULL constraints.
      */
                for (i = 0; i < nCol; i++)
                {
                    if (i == pTab.iPKey)
                    {
                        continue;
                    }
                    onError = pTab.aCol[i].notNull;
                    if (onError == OE_None)
                        continue;
                    if (overrideError != OE_Default)
                    {
                        onError = overrideError;
                    }
                    else
                        if (onError == OE_Default)
                        {
                            onError = OE_Abort;
                        }
                    if (onError == OE_Replace && pTab.aCol[i].pDflt == null)
                    {
                        onError = OE_Abort;
                    }
                    Debug.Assert(onError == OE_Rollback || onError == OE_Abort || onError == OE_Fail || onError == OE_Ignore || onError == OE_Replace);
                    switch (onError)
                    {
                        case OE_Abort:
                            {
                                sqlite3MayAbort(this);
                                goto case OE_Fail;
                            }
                        case OE_Rollback:
                        case OE_Fail:
                            {
                                string zMsg;
                                v.sqlite3VdbeAddOp3(OP_HaltIfNull, SQLITE_CONSTRAINT, onError, regData + i);
                                zMsg = sqlite3MPrintf(this.db, "%s.%s may not be NULL", pTab.zName, pTab.aCol[i].zName);
                                v.sqlite3VdbeChangeP4(-1, zMsg, P4_DYNAMIC);
                                break;
                            }
                        case OE_Ignore:
                            {
                                v.sqlite3VdbeAddOp2(OP_IsNull, regData + i, ignoreDest);
                                break;
                            }
                        default:
                            {
                                Debug.Assert(onError == OE_Replace);
                                j1 = v.sqlite3VdbeAddOp1(OP_NotNull, regData + i);
                                this.sqlite3ExprCode(pTab.aCol[i].pDflt, regData + i);
                                v.sqlite3VdbeJumpHere(j1);
                                break;
                            }
                    }
                }
                /* Test all CHECK constraints
      */
#if !SQLITE_OMIT_CHECK
                if (pTab.pCheck != null && (this.db.flags & SQLITE_IgnoreChecks) == 0)
                {
                    int allOk = v.sqlite3VdbeMakeLabel();
                    this.ckBase = regData;
                    this.sqlite3ExprIfTrue(pTab.pCheck, allOk, SQLITE_JUMPIFNULL);
                    onError = overrideError != OE_Default ? overrideError : OE_Abort;
                    if (onError == OE_Ignore)
                    {
                        v.sqlite3VdbeAddOp2(OP_Goto, 0, ignoreDest);
                    }
                    else
                    {
                        if (onError == OE_Replace)
                            onError = OE_Abort;
                        /* IMP: R-15569-63625 */
                        sqlite3HaltConstraint(this, onError, (string)null, 0);
                    }
                    v.sqlite3VdbeResolveLabel(allOk);
                }
#endif
                /* If we have an INTEGER PRIMARY KEY, make sure the primary key
** of the new record does not previously exist.  Except, if this
** is an UPDATE and the primary key is not changing, that is OK.
*/
                if (rowidChng != 0)
                {
                    onError = pTab.keyConf;
                    if (overrideError != OE_Default)
                    {
                        onError = overrideError;
                    }
                    else
                        if (onError == OE_Default)
                        {
                            onError = OE_Abort;
                        }
                    if (isUpdate)
                    {
                        j2 = v.sqlite3VdbeAddOp3(OP_Eq, regRowid, 0, rowidChng);
                    }
                    j3 = v.sqlite3VdbeAddOp3(OP_NotExists, baseCur, 0, regRowid);
                    switch (onError)
                    {
                        default:
                            {
                                onError = OE_Abort;
                                /* Fall thru into the next case */
                            }
                            goto case OE_Rollback;
                        case OE_Rollback:
                        case OE_Abort:
                        case OE_Fail:
                            {
                                sqlite3HaltConstraint(this, onError, "PRIMARY KEY must be unique", P4_STATIC);
                                break;
                            }
                        case OE_Replace:
                            {
                                /* If there are DELETE triggers on this table and the
                      ** recursive-triggers flag is set, call GenerateRowDelete() to
                      ** remove the conflicting row from the the table. This will fire
                      ** the triggers and remove both the table and index b-tree entries.
                      **
                      ** Otherwise, if there are no triggers or the recursive-triggers
                      ** flag is not set, but the table has one or more indexes, call 
                      ** GenerateRowIndexDelete(). This removes the index b-tree entries 
                      ** only. The table b-tree entry will be replaced by the new entry 
                      ** when it is inserted.  
                      **
                      ** If either GenerateRowDelete() or GenerateRowIndexDelete() is called,
                      ** also invoke MultiWrite() to indicate that this VDBE may require
                      ** statement rollback (if the statement is aborted after the delete
                      ** takes place). Earlier versions called sqlite3MultiWrite() regardless,
                      ** but being more selective here allows statements like:
                      **
                      **   REPLACE INTO t(rowid) VALUES($newrowid)
                      **
                      ** to run without a statement journal if there are no indexes on the
                      ** table.
                      */
                                Trigger pTrigger = null;
                                if ((this.db.flags & SQLITE_RecTriggers) != 0)
                                {
                                    int iDummy;
                                    pTrigger = sqlite3TriggersExist(this, pTab, TK_DELETE, null, out iDummy);
                                }
                                if (pTrigger != null || this.sqlite3FkRequired(pTab, null, 0) != 0)
                                {
                                    sqlite3MultiWrite(this);
                                    this.sqlite3GenerateRowDelete(pTab, baseCur, regRowid, 0, pTrigger, OE_Replace);
                                }
                                else
                                    if (pTab.pIndex != null)
                                    {
                                        sqlite3MultiWrite(this);
                                        this.sqlite3GenerateRowIndexDelete(pTab, baseCur, 0);
                                    }
                                seenReplace = true;
                                break;
                            }
                        case OE_Ignore:
                            {
                                Debug.Assert(!seenReplace);
                                v.sqlite3VdbeAddOp2(OP_Goto, 0, ignoreDest);
                                break;
                            }
                    }
                    v.sqlite3VdbeJumpHere(j3);
                    if (isUpdate)
                    {
                        v.sqlite3VdbeJumpHere(j2);
                    }
                }
                /* Test all UNIQUE constraints by creating entries for each UNIQUE
      ** index and making sure that duplicate entries do not already exist.
      ** Add the new records to the indices as we go.
      */
                for (iCur = 0, pIdx = pTab.pIndex; pIdx != null; pIdx = pIdx.pNext, iCur++)
                {
                    int regIdx;
                    int regR;
                    if (aRegIdx[iCur] == 0)
                        continue;
                    /* Skip unused indices */
                    /* Create a key for accessing the index entry */
                    regIdx = this.sqlite3GetTempRange(pIdx.nColumn + 1);
                    for (i = 0; i < pIdx.nColumn; i++)
                    {
                        int idx = pIdx.aiColumn[i];
                        if (idx == pTab.iPKey)
                        {
                            v.sqlite3VdbeAddOp2(OP_SCopy, regRowid, regIdx + i);
                        }
                        else
                        {
                            v.sqlite3VdbeAddOp2(OP_SCopy, regData + idx, regIdx + i);
                        }
                    }
                    v.sqlite3VdbeAddOp2(OP_SCopy, regRowid, regIdx + i);
                    v.sqlite3VdbeAddOp3(OP_MakeRecord, regIdx, pIdx.nColumn + 1, aRegIdx[iCur]);
                    v.sqlite3VdbeChangeP4(-1, v.sqlite3IndexAffinityStr(pIdx), P4_TRANSIENT);
                    this.sqlite3ExprCacheAffinityChange(regIdx, pIdx.nColumn + 1);
                    /* Find out what action to take in case there is an indexing conflict */
                    onError = pIdx.onError;
                    if (onError == OE_None)
                    {
                        this.sqlite3ReleaseTempRange(regIdx, pIdx.nColumn + 1);
                        continue;
                        /* pIdx is not a UNIQUE index */
                    }
                    if (overrideError != OE_Default)
                    {
                        onError = overrideError;
                    }
                    else
                        if (onError == OE_Default)
                        {
                            onError = OE_Abort;
                        }
                    if (seenReplace)
                    {
                        if (onError == OE_Ignore)
                            onError = OE_Replace;
                        else
                            if (onError == OE_Fail)
                                onError = OE_Abort;
                    }
                    /* Check to see if the new index entry will be unique */
                    regR = this.sqlite3GetTempReg();
                    v.sqlite3VdbeAddOp2(OP_SCopy, regOldRowid, regR);
                    j3 = v.sqlite3VdbeAddOp4(OP_IsUnique, baseCur + iCur + 1, 0, regR, regIdx,//regR, SQLITE_INT_TO_PTR(regIdx),
                    P4_INT32);
                    this.sqlite3ReleaseTempRange(regIdx, pIdx.nColumn + 1);
                    /* Generate code that executes if the new index entry is not unique */
                    Debug.Assert(onError == OE_Rollback || onError == OE_Abort || onError == OE_Fail || onError == OE_Ignore || onError == OE_Replace);
                    switch (onError)
                    {
                        case OE_Rollback:
                        case OE_Abort:
                        case OE_Fail:
                            {
                                int j;
                                StrAccum errMsg = new StrAccum(200);
                                string zSep;
                                string zErr;
                                sqlite3StrAccumInit(errMsg, null, 0, 200);
                                errMsg.db = this.db;
                                zSep = pIdx.nColumn > 1 ? "columns " : "column ";
                                for (j = 0; j < pIdx.nColumn; j++)
                                {
                                    string zCol = pTab.aCol[pIdx.aiColumn[j]].zName;
                                    sqlite3StrAccumAppend(errMsg, zSep, -1);
                                    zSep = ", ";
                                    sqlite3StrAccumAppend(errMsg, zCol, -1);
                                }
                                sqlite3StrAccumAppend(errMsg, pIdx.nColumn > 1 ? " are not unique" : " is not unique", -1);
                                zErr = sqlite3StrAccumFinish(errMsg);
                                sqlite3HaltConstraint(this, onError, zErr, 0);
                                errMsg.db.sqlite3DbFree(ref zErr);
                                break;
                            }
                        case OE_Ignore:
                            {
                                Debug.Assert(!seenReplace);
                                v.sqlite3VdbeAddOp2(OP_Goto, 0, ignoreDest);
                                break;
                            }
                        default:
                            {
                                Trigger pTrigger = null;
                                Debug.Assert(onError == OE_Replace);
                                sqlite3MultiWrite(this);
                                if ((this.db.flags & SQLITE_RecTriggers) != 0)
                                {
                                    int iDummy;
                                    pTrigger = sqlite3TriggersExist(this, pTab, TK_DELETE, null, out iDummy);
                                }
                                this.sqlite3GenerateRowDelete(pTab, baseCur, regR, 0, pTrigger, OE_Replace);
                                seenReplace = true;
                                break;
                            }
                    }
                    v.sqlite3VdbeJumpHere(j3);
                    this.sqlite3ReleaseTempReg(regR);
                }
                //if ( pbMayReplace )
                {
                    pbMayReplace = seenReplace ? 1 : 0;
                }
            }
            public void sqlite3CompleteInsertion(/* The parser context */Table pTab,/* the table into which we are inserting */int baseCur,/* Index of a read/write cursor pointing at pTab */int regRowid,/* Range of content */int[] aRegIdx,/* Register used by each index.  0 for unused indices */bool isUpdate,/* True for UPDATE, False for INSERT */bool appendBias,/* True if this is likely to be an append */bool useSeekResult/* True to set the USESEEKRESULT flag on OP_[Idx]Insert */)
            {
                int i;
                Vdbe v;
                int nIdx;
                Index pIdx;
                u8 pik_flags;
                int regData;
                int regRec;
                v = sqlite3GetVdbe(this);
                Debug.Assert(v != null);
                Debug.Assert(pTab.pSelect == null);
                /* This table is not a VIEW */
                for (nIdx = 0, pIdx = pTab.pIndex; pIdx != null; pIdx = pIdx.pNext, nIdx++)
                {
                }
                for (i = nIdx - 1; i >= 0; i--)
                {
                    if (aRegIdx[i] == 0)
                        continue;
                    v.sqlite3VdbeAddOp2(OP_IdxInsert, baseCur + i + 1, aRegIdx[i]);
                    if (useSeekResult)
                    {
                        v.sqlite3VdbeChangeP5(OPFLAG_USESEEKRESULT);
                    }
                }
                regData = regRowid + 1;
                regRec = this.sqlite3GetTempReg();
                v.sqlite3VdbeAddOp3(OP_MakeRecord, regData, pTab.nCol, regRec);
                v.sqlite3TableAffinityStr(pTab);
                this.sqlite3ExprCacheAffinityChange(regData, pTab.nCol);
                if (this.nested != 0)
                {
                    pik_flags = 0;
                }
                else
                {
                    pik_flags = OPFLAG_NCHANGE;
                    pik_flags |= (isUpdate ? OPFLAG_ISUPDATE : OPFLAG_LASTROWID);
                }
                if (appendBias)
                {
                    pik_flags |= OPFLAG_APPEND;
                }
                if (useSeekResult)
                {
                    pik_flags |= OPFLAG_USESEEKRESULT;
                }
                v.sqlite3VdbeAddOp3(OP_Insert, baseCur, regRec, regRowid);
                if (this.nested == 0)
                {
                    v.sqlite3VdbeChangeP4(-1, pTab.zName, P4_TRANSIENT);
                }
                v.sqlite3VdbeChangeP5(pik_flags);
            }
            public int sqlite3OpenTableAndIndices(/* Parsing context */Table pTab,/* Table to be opened */int baseCur,/* VdbeCursor number assigned to the table */int op/* OP_OpenRead or OP_OpenWrite */)
            {
                int i;
                int iDb;
                Index pIdx;
                Vdbe v;
                if (IsVirtual(pTab))
                    return 0;
                iDb = sqlite3SchemaToIndex(this.db, pTab.pSchema);
                v = sqlite3GetVdbe(this);
                Debug.Assert(v != null);
                this.sqlite3OpenTable(baseCur, iDb, pTab, op);
                for (i = 1, pIdx = pTab.pIndex; pIdx != null; pIdx = pIdx.pNext, i++)
                {
                    KeyInfo pKey = sqlite3IndexKeyinfo(this, pIdx);
                    Debug.Assert(pIdx.pSchema == pTab.pSchema);
                    v.sqlite3VdbeAddOp4(op, i + baseCur, pIdx.tnum, iDb, pKey, P4_KEYINFO_HANDOFF);
#if SQLITE_DEBUG
																																																																																																															        VdbeComment( v, "%s", pIdx.zName );
#endif
                }
                if (this.nTab < baseCur + i)
                {
                    this.nTab = baseCur + i;
                }
                return i - 1;
            }
            public int autoIncBegin(/* Parsing context */int iDb,/* Index of the database holding pTab */Table pTab/* The table we are writing to */)
            {
                int memId = 0;
                /* Register holding maximum rowid */
                if ((pTab.tabFlags & TF_Autoincrement) != 0)
                {
                    Parse pToplevel = sqlite3ParseToplevel(this);
                    AutoincInfo pInfo;
                    pInfo = pToplevel.pAinc;
                    while (pInfo != null && pInfo.pTab != pTab)
                    {
                        pInfo = pInfo.pNext;
                    }
                    if (pInfo == null)
                    {
                        pInfo = new AutoincInfo();
                        //sqlite3DbMallocRaw(pParse.db, sizeof(*pInfo));
                        //if( pInfo==0 ) return 0;
                        pInfo.pNext = pToplevel.pAinc;
                        pToplevel.pAinc = pInfo;
                        pInfo.pTab = pTab;
                        pInfo.iDb = iDb;
                        pToplevel.nMem++;
                        /* Register to hold name of table */
                        pInfo.regCtr = ++pToplevel.nMem;
                        /* Max rowid register */
                        pToplevel.nMem++;
                        /* Rowid in sqlite_sequence */
                    }
                    memId = pInfo.regCtr;
                }
                return memId;
            }
            public bool readsTable(int iStartAddr, int iDb, Table pTab)
            {
                Vdbe v = sqlite3GetVdbe(this);
                int i;
                int iEnd = v.sqlite3VdbeCurrentAddr();
#if !SQLITE_OMIT_VIRTUALTABLE
                VTable pVTab = IsVirtual(pTab) ? sqlite3GetVTable(this.db, pTab) : null;
#endif
                for (i = iStartAddr; i < iEnd; i++)
                {
                    VdbeOp pOp = v.sqlite3VdbeGetOp(i);
                    Debug.Assert(pOp != null);
                    if (pOp.opcode == OP_OpenRead && pOp.p3 == iDb)
                    {
                        Index pIndex;
                        int tnum = pOp.p2;
                        if (tnum == pTab.tnum)
                        {
                            return true;
                        }
                        for (pIndex = pTab.pIndex; pIndex != null; pIndex = pIndex.pNext)
                        {
                            if (tnum == pIndex.tnum)
                            {
                                return true;
                            }
                        }
                    }
#if !SQLITE_OMIT_VIRTUALTABLE
                    if (pOp.opcode == OP_VOpen && pOp.p4.pVtab == pVTab)
                    {
                        Debug.Assert(pOp.p4.pVtab != null);
                        Debug.Assert(pOp.p4type == P4_VTAB);
                        return true;
                    }
#endif
                }
                return false;
            }
            public void sqlite3OpenTable(/* Generate code into this VDBE */int iCur,/* The cursor number of the table */int iDb,/* The database index in sqlite3.aDb[] */Table pTab,/* The table to be opened */int opcode/* OP_OpenRead or OP_OpenWrite */)
            {
                Vdbe v;
                if (IsVirtual(pTab))
                    return;
                v = sqlite3GetVdbe(this);
                Debug.Assert(opcode == OP_OpenWrite || opcode == OP_OpenRead);
                sqlite3TableLock(this, iDb, pTab.tnum, (opcode == OP_OpenWrite) ? (byte)1 : (byte)0, pTab.zName);
                v.sqlite3VdbeAddOp3(opcode, iCur, pTab.tnum, iDb);
                v.sqlite3VdbeChangeP4(-1, (pTab.nCol), P4_INT32);
                //SQLITE_INT_TO_PTR( pTab.nCol ), P4_INT32 );
                VdbeComment(v, "%s", pTab.zName);
            }
            public void sqlite3Update(/* The parser context */SrcList pTabList,/* The table in which we should change things */ExprList pChanges,/* Things to be changed */Expr pWhere,/* The WHERE clause.  May be null */int onError/* How to handle constraint errors */)
            {
                int i, j;
                /* Loop counters */
                Table pTab;
                /* The table to be updated */
                int addr = 0;
                /* VDBE instruction address of the start of the loop */
                WhereInfo pWInfo;
                /* Information about the WHERE clause */
                Vdbe v;
                /* The virtual database engine */
                Index pIdx;
                /* For looping over indices */
                int nIdx;
                /* Number of indices that need updating */
                int iCur;
                /* VDBE Cursor number of pTab */
                sqlite3 db;
                /* The database structure */
                int[] aRegIdx = null;
                /* One register assigned to each index to be updated */
                int[] aXRef = null;
                /* aXRef[i] is the index in pChanges.a[] of the
** an expression for the i-th column of the table.
** aXRef[i]==-1 if the i-th column is not changed. */
                bool chngRowid;
                /* True if the record number is being changed */
                Expr pRowidExpr = null;
                /* Expression defining the new record number */
                bool openAll = false;
                /* True if all indices need to be opened */
                AuthContext sContext;
                /* The authorization context */
                NameContext sNC;
                /* The name-context to resolve expressions in */
                int iDb;
                /* Database containing the table being updated */
                bool okOnePass;
                /* True for one-pass algorithm without the FIFO */
                bool hasFK;
                /* True if foreign key processing is required */
#if !SQLITE_OMIT_TRIGGER
                bool isView;
                /* True when updating a view (INSTEAD OF trigger) */
                Trigger pTrigger;
                /* List of triggers on pTab, if required */
                int tmask = 0;
                /* Mask of TRIGGER_BEFORE|TRIGGER_AFTER */
#endif
                int newmask;
                /* Mask of NEW.* columns accessed by BEFORE triggers */
                /* Register Allocations */
                int regRowCount = 0;
                /* A count of rows changed */
                int regOldRowid;
                /* The old rowid */
                int regNewRowid;
                /* The new rowid */
                int regNew;
                int regOld = 0;
                int regRowSet = 0;
                /* Rowset of rows to be updated */
                sContext = new AuthContext();
                //memset( &sContext, 0, sizeof( sContext ) );
                db = this.db;
                if (this.nErr != 0/*|| db.mallocFailed != 0 */)
                {
                    goto update_cleanup;
                }
                Debug.Assert(pTabList.nSrc == 1);
                /* Locate the table which we want to update.
      */
                pTab = this.sqlite3SrcListLookup(pTabList);
                if (pTab == null)
                    goto update_cleanup;
                iDb = sqlite3SchemaToIndex(this.db, pTab.pSchema);
                /* Figure out if we have any triggers and if the table being
      ** updated is a view.
      */
#if !SQLITE_OMIT_TRIGGER
                pTrigger = sqlite3TriggersExist(this, pTab, TK_UPDATE, pChanges, out tmask);
                isView = pTab.pSelect != null;
                Debug.Assert(pTrigger != null || tmask == 0);
#else
																																																																																					      const Trigger pTrigger = null;// define pTrigger 0
      const int tmask = 0;          // define tmask 0
#endif
#if SQLITE_OMIT_TRIGGER || SQLITE_OMIT_VIEW
																																																																																					//     undef isView
      const bool isView = false;    // define isView 0
#endif
                if (sqlite3ViewGetColumnNames(this, pTab) != 0)
                {
                    goto update_cleanup;
                }
                if (this.sqlite3IsReadOnly(pTab, tmask))
                {
                    goto update_cleanup;
                }
                aXRef = new int[pTab.nCol];
                // sqlite3DbMallocRaw(db, sizeof(int) * pTab.nCol);
                //if ( aXRef == null ) goto update_cleanup;
                for (i = 0; i < pTab.nCol; i++)
                    aXRef[i] = -1;
                /* Allocate a cursors for the main database table and for all indices.
      ** The index cursors might not be used, but if they are used they
      ** need to occur right after the database cursor.  So go ahead and
      ** allocate enough space, just in case.
      */
                pTabList.a[0].iCursor = iCur = this.nTab++;
                for (pIdx = pTab.pIndex; pIdx != null; pIdx = pIdx.pNext)
                {
                    this.nTab++;
                }
                /* Initialize the name-context */
                sNC = new NameContext();
                // memset(&sNC, 0, sNC).Length;
                sNC.pParse = this;
                sNC.pSrcList = pTabList;
                /* Resolve the column names in all the expressions of the
      ** of the UPDATE statement.  Also find the column index
      ** for each column to be updated in the pChanges array.  For each
      ** column to be updated, make sure we have authorization to change
      ** that column.
      */
                chngRowid = false;
                for (i = 0; i < pChanges.nExpr; i++)
                {
                    if (sqlite3ResolveExprNames(sNC, ref pChanges.a[i].pExpr) != 0)
                    {
                        goto update_cleanup;
                    }
                    for (j = 0; j < pTab.nCol; j++)
                    {
                        if (pTab.aCol[j].zName.Equals(pChanges.a[i].zName, StringComparison.InvariantCultureIgnoreCase))
                        {
                            if (j == pTab.iPKey)
                            {
                                chngRowid = true;
                                pRowidExpr = pChanges.a[i].pExpr;
                            }
                            aXRef[j] = i;
                            break;
                        }
                    }
                    if (j >= pTab.nCol)
                    {
                        if (sqlite3IsRowid(pChanges.a[i].zName))
                        {
                            chngRowid = true;
                            pRowidExpr = pChanges.a[i].pExpr;
                        }
                        else
                        {
                            sqlite3ErrorMsg(this, "no such column: %s", pChanges.a[i].zName);
                            this.checkSchema = 1;
                            goto update_cleanup;
                        }
                    }
#if !SQLITE_OMIT_AUTHORIZATION
																																																																																																															{
int rc;
rc = sqlite3AuthCheck(pParse, SQLITE_UPDATE, pTab.zName,
pTab.aCol[j].zName, db.aDb[iDb].zName);
if( rc==SQLITE_DENY ){
goto update_cleanup;
}else if( rc==SQLITE_IGNORE ){
aXRef[j] = -1;
}
}
#endif
                }
                hasFK = this.sqlite3FkRequired(pTab, aXRef, chngRowid ? 1 : 0) != 0;
                /* Allocate memory for the array aRegIdx[].  There is one entry in the
      ** array for each index associated with table being updated.  Fill in
      ** the value with a register number for indices that are to be used
      ** and with zero for unused indices.
      */
                for (nIdx = 0, pIdx = pTab.pIndex; pIdx != null; pIdx = pIdx.pNext, nIdx++)
                {
                }
                if (nIdx > 0)
                {
                    aRegIdx = new int[nIdx];
                    // sqlite3DbMallocRaw(db, Index*.Length * nIdx);
                    if (aRegIdx == null)
                        goto update_cleanup;
                }
                for (j = 0, pIdx = pTab.pIndex; pIdx != null; pIdx = pIdx.pNext, j++)
                {
                    int reg;
                    if (hasFK || chngRowid)
                    {
                        reg = ++this.nMem;
                    }
                    else
                    {
                        reg = 0;
                        for (i = 0; i < pIdx.nColumn; i++)
                        {
                            if (aXRef[pIdx.aiColumn[i]] >= 0)
                            {
                                reg = ++this.nMem;
                                break;
                            }
                        }
                    }
                    aRegIdx[j] = reg;
                }
                /* Begin generating code. */
                v = sqlite3GetVdbe(this);
                if (v == null)
                    goto update_cleanup;
                if (this.nested == 0)
                    sqlite3VdbeCountChanges(v);
                sqlite3BeginWriteOperation(this, 1, iDb);
#if !SQLITE_OMIT_VIRTUALTABLE
                /* Virtual tables must be handled separately */
                if (IsVirtual(pTab))
                {
                    this.updateVirtualTable(pTabList, pTab, pChanges, pRowidExpr, aXRef, pWhere, onError);
                    pWhere = null;
                    pTabList = null;
                    goto update_cleanup;
                }
#endif
                /* Allocate required registers. */
                regOldRowid = regNewRowid = ++this.nMem;
                if (pTrigger != null || hasFK)
                {
                    regOld = this.nMem + 1;
                    this.nMem += pTab.nCol;
                }
                if (chngRowid || pTrigger != null || hasFK)
                {
                    regNewRowid = ++this.nMem;
                }
                regNew = this.nMem + 1;
                this.nMem += pTab.nCol;
                /* Start the view context. */
                if (isView)
                {
                    sqlite3AuthContextPush(this, sContext, pTab.zName);
                }
                /* If we are trying to update a view, realize that view into
      ** a ephemeral table.
      */
#if !(SQLITE_OMIT_VIEW) && !(SQLITE_OMIT_TRIGGER)
                if (isView)
                {
                    this.sqlite3MaterializeView(pTab, pWhere, iCur);
                }
#endif
                /* Resolve the column names in all the expressions in the
** WHERE clause.
*/
                if (sqlite3ResolveExprNames(sNC, ref pWhere) != 0)
                {
                    goto update_cleanup;
                }
                /* Begin the database scan
      */
                v.sqlite3VdbeAddOp2(OP_Null, 0, regOldRowid);
                ExprList NullOrderby = null;
                pWInfo = this.sqlite3WhereBegin(pTabList, pWhere, ref NullOrderby, WHERE_ONEPASS_DESIRED);
                if (pWInfo == null)
                    goto update_cleanup;
                okOnePass = pWInfo.okOnePass != 0;
                /* Remember the rowid of every item to be updated.
      */
                v.sqlite3VdbeAddOp2(OP_Rowid, iCur, regOldRowid);
                if (!okOnePass)
                {
                    regRowSet = ++this.nMem;
                    v.sqlite3VdbeAddOp2(OP_RowSetAdd, regRowSet, regOldRowid);
                }
                /* End the database scan loop.
      */
                pWInfo.sqlite3WhereEnd();
                /* Initialize the count of updated rows
      */
                if ((db.flags & SQLITE_CountRows) != 0 && null == this.pTriggerTab)
                {
                    regRowCount = ++this.nMem;
                    v.sqlite3VdbeAddOp2(OP_Integer, 0, regRowCount);
                }
                if (!isView)
                {
                    /*
        ** Open every index that needs updating.  Note that if any
        ** index could potentially invoke a REPLACE conflict resolution
        ** action, then we need to open all indices because we might need
        ** to be deleting some records.
        */
                    if (!okOnePass)
                        this.sqlite3OpenTable(iCur, iDb, pTab, OP_OpenWrite);
                    if (onError == OE_Replace)
                    {
                        openAll = true;
                    }
                    else
                    {
                        openAll = false;
                        for (pIdx = pTab.pIndex; pIdx != null; pIdx = pIdx.pNext)
                        {
                            if (pIdx.onError == OE_Replace)
                            {
                                openAll = true;
                                break;
                            }
                        }
                    }
                    for (i = 0, pIdx = pTab.pIndex; pIdx != null; pIdx = pIdx.pNext, i++)
                    {
                        if (openAll || aRegIdx[i] > 0)
                        {
                            KeyInfo pKey = sqlite3IndexKeyinfo(this, pIdx);
                            v.sqlite3VdbeAddOp4(OP_OpenWrite, iCur + i + 1, pIdx.tnum, iDb, pKey, P4_KEYINFO_HANDOFF);
                            Debug.Assert(this.nTab > iCur + i + 1);
                        }
                    }
                }
                /* Top of the update loop */
                if (okOnePass)
                {
                    int a1 = v.sqlite3VdbeAddOp1(OP_NotNull, regOldRowid);
                    addr = v.sqlite3VdbeAddOp0(OP_Goto);
                    v.sqlite3VdbeJumpHere(a1);
                }
                else
                {
                    addr = v.sqlite3VdbeAddOp3(OP_RowSetRead, regRowSet, 0, regOldRowid);
                }
                /* Make cursor iCur point to the record that is being updated. If
      ** this record does not exist for some reason (deleted by a trigger,
      ** for example, then jump to the next iteration of the RowSet loop.  */
                v.sqlite3VdbeAddOp3(OP_NotExists, iCur, addr, regOldRowid);
                /* If the record number will change, set register regNewRowid to
      ** contain the new value. If the record number is not being modified,
      ** then regNewRowid is the same register as regOldRowid, which is
      ** already populated.  */
                Debug.Assert(chngRowid || pTrigger != null || hasFK || regOldRowid == regNewRowid);
                if (chngRowid)
                {
                    this.sqlite3ExprCode(pRowidExpr, regNewRowid);
                    v.sqlite3VdbeAddOp1(OP_MustBeInt, regNewRowid);
                }
                /* If there are triggers on this table, populate an array of registers 
      ** with the required old.* column data.  */
                if (hasFK || pTrigger != null)
                {
                    u32 oldmask = (hasFK ? this.sqlite3FkOldmask(pTab) : 0);
                    oldmask |= sqlite3TriggerColmask(this, pTrigger, pChanges, 0, TRIGGER_BEFORE | TRIGGER_AFTER, pTab, onError);
                    for (i = 0; i < pTab.nCol; i++)
                    {
                        if (aXRef[i] < 0 || oldmask == 0xffffffff || (i < 32 && 0 != (oldmask & (1 << i))))
                        {
                            v.sqlite3ExprCodeGetColumnOfTable(pTab, iCur, i, regOld + i);
                        }
                        else
                        {
                            v.sqlite3VdbeAddOp2(OP_Null, 0, regOld + i);
                        }
                    }
                    if (chngRowid == false)
                    {
                        v.sqlite3VdbeAddOp2(OP_Copy, regOldRowid, regNewRowid);
                    }
                }
                /* Populate the array of registers beginning at regNew with the new
      ** row data. This array is used to check constaints, create the new
      ** table and index records, and as the values for any new.* references
      ** made by triggers.
      **
      ** If there are one or more BEFORE triggers, then do not populate the
      ** registers associated with columns that are (a) not modified by
      ** this UPDATE statement and (b) not accessed by new.* references. The
      ** values for registers not modified by the UPDATE must be reloaded from 
      ** the database after the BEFORE triggers are fired anyway (as the trigger 
      ** may have modified them). So not loading those that are not going to
      ** be used eliminates some redundant opcodes.
      */
                newmask = (int)sqlite3TriggerColmask(this, pTrigger, pChanges, 1, TRIGGER_BEFORE, pTab, onError);
                for (i = 0; i < pTab.nCol; i++)
                {
                    if (i == pTab.iPKey)
                    {
                        v.sqlite3VdbeAddOp2(OP_Null, 0, regNew + i);
                    }
                    else
                    {
                        j = aXRef[i];
                        if (j >= 0)
                        {
                            this.sqlite3ExprCode(pChanges.a[j].pExpr, regNew + i);
                        }
                        else
                            if (0 == (tmask & TRIGGER_BEFORE) || i > 31 || (newmask & (1 << i)) != 0)
                            {
                                /* This branch loads the value of a column that will not be changed 
            ** into a register. This is done if there are no BEFORE triggers, or
            ** if there are one or more BEFORE triggers that use this value via
            ** a new.* reference in a trigger program.
            */
                                testcase(i == 31);
                                testcase(i == 32);
                                v.sqlite3VdbeAddOp3(OP_Column, iCur, i, regNew + i);
                                v.sqlite3ColumnDefault(pTab, i, regNew + i);
                            }
                    }
                }
                /* Fire any BEFORE UPDATE triggers. This happens before constraints are
      ** verified. One could argue that this is wrong.
      */
                if ((tmask & TRIGGER_BEFORE) != 0)
                {
                    v.sqlite3VdbeAddOp2(OP_Affinity, regNew, pTab.nCol);
                    v.sqlite3TableAffinityStr(pTab);
                    sqlite3CodeRowTrigger(this, pTrigger, TK_UPDATE, pChanges, TRIGGER_BEFORE, pTab, regOldRowid, onError, addr);
                    /* The row-trigger may have deleted the row being updated. In this
        ** case, jump to the next row. No updates or AFTER triggers are 
        ** required. This behaviour - what happens when the row being updated
        ** is deleted or renamed by a BEFORE trigger - is left undefined in the
        ** documentation.
        */
                    v.sqlite3VdbeAddOp3(OP_NotExists, iCur, addr, regOldRowid);
                    /* If it did not delete it, the row-trigger may still have modified 
        ** some of the columns of the row being updated. Load the values for 
        ** all columns not modified by the update statement into their 
        ** registers in case this has happened.
        */
                    for (i = 0; i < pTab.nCol; i++)
                    {
                        if (aXRef[i] < 0 && i != pTab.iPKey)
                        {
                            v.sqlite3VdbeAddOp3(OP_Column, iCur, i, regNew + i);
                            v.sqlite3ColumnDefault(pTab, i, regNew + i);
                        }
                    }
                }
                if (!isView)
                {
                    int j1;
                    /* Address of jump instruction */
                    /* Do constraint checks. */
                    int iDummy;
                    this.sqlite3GenerateConstraintChecks(pTab, iCur, regNewRowid, aRegIdx, (chngRowid ? regOldRowid : 0), true, onError, addr, out iDummy);
                    /* Do FK constraint checks. */
                    if (hasFK)
                    {
                        this.sqlite3FkCheck(pTab, regOldRowid, 0);
                    }
                    /* Delete the index entries associated with the current record.  */
                    j1 = v.sqlite3VdbeAddOp3(OP_NotExists, iCur, 0, regOldRowid);
                    this.sqlite3GenerateRowIndexDelete(pTab, iCur, aRegIdx);
                    /* If changing the record number, delete the old record.  */
                    if (hasFK || chngRowid)
                    {
                        v.sqlite3VdbeAddOp2(OP_Delete, iCur, 0);
                    }
                    v.sqlite3VdbeJumpHere(j1);
                    if (hasFK)
                    {
                        this.sqlite3FkCheck(pTab, 0, regNewRowid);
                    }
                    /* Insert the new index entries and the new record. */
                    this.sqlite3CompleteInsertion(pTab, iCur, regNewRowid, aRegIdx, true, false, false);
                    /* Do any ON CASCADE, SET NULL or SET DEFAULT operations required to
        ** handle rows (possibly in other tables) that refer via a foreign key
        ** to the row just updated. */
                    if (hasFK)
                    {
                        this.sqlite3FkActions(pTab, pChanges, regOldRowid);
                    }
                }
                /* Increment the row counter 
      */
                if ((db.flags & SQLITE_CountRows) != 0 && null == this.pTriggerTab)
                {
                    v.sqlite3VdbeAddOp2(OP_AddImm, regRowCount, 1);
                }
                sqlite3CodeRowTrigger(this, pTrigger, TK_UPDATE, pChanges, TRIGGER_AFTER, pTab, regOldRowid, onError, addr);
                /* Repeat the above with the next record to be updated, until
      ** all record selected by the WHERE clause have been updated.
      */
                v.sqlite3VdbeAddOp2(OP_Goto, 0, addr);
                v.sqlite3VdbeJumpHere(addr);
                /* Close all tables */
                for (i = 0, pIdx = pTab.pIndex; pIdx != null; pIdx = pIdx.pNext, i++)
                {
                    if (openAll || aRegIdx[i] > 0)
                    {
                        v.sqlite3VdbeAddOp2(OP_Close, iCur + i + 1, 0);
                    }
                }
                v.sqlite3VdbeAddOp2(OP_Close, iCur, 0);
                /* Update the sqlite_sequence table by storing the content of the
      ** maximum rowid counter values recorded while inserting into
      ** autoincrement tables.
      */
                if (this.nested == 0 && this.pTriggerTab == null)
                {
                    this.sqlite3AutoincrementEnd();
                }
                /*
      ** Return the number of rows that were changed. If this routine is 
      ** generating code because of a call to sqlite3NestedParse(), do not
      ** invoke the callback function.
      */
                if ((db.flags & SQLITE_CountRows) != 0 && null == this.pTriggerTab && 0 == this.nested)
                {
                    v.sqlite3VdbeAddOp2(OP_ResultRow, regRowCount, 1);
                    sqlite3VdbeSetNumCols(v, 1);
                    sqlite3VdbeSetColName(v, 0, COLNAME_NAME, "rows updated", SQLITE_STATIC);
                }
            update_cleanup:
#if !SQLITE_OMIT_AUTHORIZATION
																																																																																					sqlite3AuthContextPop(sContext);
#endif
                db.sqlite3DbFree(ref aRegIdx);
                db.sqlite3DbFree(ref aXRef);
                sqlite3SrcListDelete(db, ref pTabList);
                sqlite3ExprListDelete(db, ref pChanges);
                sqlite3ExprDelete(db, ref pWhere);
                return;
            }
            public void updateVirtualTable(/* The parsing context */SrcList pSrc,/* The virtual table to be modified */Table pTab,/* The virtual table */ExprList pChanges,/* The columns to change in the UPDATE statement */Expr pRowid,/* Expression used to recompute the rowid */int[] aXRef,/* Mapping from columns of pTab to entries in pChanges */Expr pWhere,/* WHERE clause of the UPDATE statement */int onError/* ON CONFLICT strategy */)
            {
                Vdbe v = this.pVdbe;
                /* Virtual machine under construction */
                ExprList pEList = null;
                /* The result set of the SELECT statement */
                Select pSelect = null;
                /* The SELECT statement */
                Expr pExpr;
                /* Temporary expression */
                int ephemTab;
                /* Table holding the result of the SELECT */
                int i;
                /* Loop counter */
                int addr;
                /* Address of top of loop */
                int iReg;
                /* First register in set passed to OP_VUpdate */
                sqlite3 db = this.db;
                /* Database connection */
                VTable pVTab = sqlite3GetVTable(db, pTab);
                SelectDest dest = new SelectDest();
                /* Construct the SELECT statement that will find the new values for
      ** all updated rows.
      */
                pEList = this.sqlite3ExprListAppend(0, sqlite3Expr(db, TK_ID, "_rowid_"));
                if (pRowid != null)
                {
                    pEList = this.sqlite3ExprListAppend(pEList, sqlite3ExprDup(db, pRowid, 0));
                }
                Debug.Assert(pTab.iPKey < 0);
                for (i = 0; i < pTab.nCol; i++)
                {
                    if (aXRef[i] >= 0)
                    {
                        pExpr = sqlite3ExprDup(db, pChanges.a[aXRef[i]].pExpr, 0);
                    }
                    else
                    {
                        pExpr = sqlite3Expr(db, TK_ID, pTab.aCol[i].zName);
                    }
                    pEList = this.sqlite3ExprListAppend(pEList, pExpr);
                }
                pSelect = sqlite3SelectNew(this, pEList, pSrc, pWhere, null, null, null, 0, null, null);
                /* Create the ephemeral table into which the update results will
      ** be stored.
      */
                Debug.Assert(v != null);
                ephemTab = this.nTab++;
                v.sqlite3VdbeAddOp2(OP_OpenEphemeral, ephemTab, pTab.nCol + 1 + ((pRowid != null) ? 1 : 0));
                v.sqlite3VdbeChangeP5(BTREE_UNORDERED);
                /* fill the ephemeral table
      */
                sqlite3SelectDestInit(dest, SelectResultType.Table, ephemTab);
                sqlite3Select(this, pSelect, ref dest);
                /* Generate code to scan the ephemeral table and call VUpdate. */
                iReg = ++this.nMem;
                this.nMem += pTab.nCol + 1;
                addr = v.sqlite3VdbeAddOp2(OP_Rewind, ephemTab, 0);
                v.sqlite3VdbeAddOp3(OP_Column, ephemTab, 0, iReg);
                v.sqlite3VdbeAddOp3(OP_Column, ephemTab, (pRowid != null ? 1 : 0), iReg + 1);
                for (i = 0; i < pTab.nCol; i++)
                {
                    v.sqlite3VdbeAddOp3(OP_Column, ephemTab, i + 1 + ((pRowid != null) ? 1 : 0), iReg + 2 + i);
                }
                this.sqlite3VtabMakeWritable(pTab);
                v.sqlite3VdbeAddOp4(OP_VUpdate, 0, pTab.nCol + 2, iReg, pVTab, P4_VTAB);
                v.sqlite3VdbeChangeP5((byte)(onError == OE_Default ? OE_Abort : onError));
                sqlite3MayAbort(this);
                v.sqlite3VdbeAddOp2(OP_Next, ephemTab, addr + 1);
                v.sqlite3VdbeJumpHere(addr);
                v.sqlite3VdbeAddOp2(OP_Close, ephemTab, 0);
                /* Cleanup */
                sqlite3SelectDelete(db, ref pSelect);
            }
            public Table sqlite3SrcListLookup(SrcList pSrc)
            {
                SrcList_item pItem = pSrc.a[0];
                Table pTab;
                Debug.Assert(pItem != null && pSrc.nSrc == 1);
                pTab = sqlite3LocateTable(this, 0, pItem.zName, pItem.zDatabase);
                sqlite3DeleteTable(this.db, ref pItem.pTab);
                pItem.pTab = pTab;
                if (pTab != null)
                {
                    pTab.nRef++;
                }
                if (sqlite3IndexedByLookup(this, pItem) != 0)
                {
                    pTab = null;
                }
                return pTab;
            }
            public bool sqlite3IsReadOnly(Table pTab, int viewOk)
            {
                /* A table is not writable under the following circumstances:
      **
      **   1) It is a virtual table and no implementation of the xUpdate method
      **      has been provided, or
      **   2) It is a system table (i.e. sqlite_master), this call is not
      **      part of a nested parse and writable_schema pragma has not
      **      been specified.
      **
      ** In either case leave an error message in pParse and return non-zero.
      */
                if ((IsVirtual(pTab) && sqlite3GetVTable(this.db, pTab).pMod.pModule.xUpdate == null) || ((pTab.tabFlags & TF_Readonly) != 0 && (this.db.flags & SQLITE_WriteSchema) == 0 && this.nested == 0))
                {
                    sqlite3ErrorMsg(this, "table %s may not be modified", pTab.zName);
                    return true;
                }
#if !SQLITE_OMIT_VIEW
                if (viewOk == 0 && pTab.pSelect != null)
                {
                    sqlite3ErrorMsg(this, "cannot modify %s because it is a view", pTab.zName);
                    return true;
                }
#endif
                return false;
            }
            public void sqlite3MaterializeView(/* Parsing context */Table pView,/* View definition */Expr pWhere,/* Optional WHERE clause to be added */int iCur/* VdbeCursor number for ephemerial table */)
            {
                SelectDest dest = new SelectDest();
                Select pDup;
                sqlite3 db = this.db;
                pDup = sqlite3SelectDup(db, pView.pSelect, 0);
                if (pWhere != null)
                {
                    SrcList pFrom;
                    pWhere = sqlite3ExprDup(db, pWhere, 0);
                    pFrom = sqlite3SrcListAppend(db, null, null, null);
                    //if ( pFrom != null )
                    //{
                    Debug.Assert(pFrom.nSrc == 1);
                    pFrom.a[0].zAlias = pView.zName;
                    // sqlite3DbStrDup( db, pView.zName );
                    pFrom.a[0].pSelect = pDup;
                    Debug.Assert(pFrom.a[0].pOn == null);
                    Debug.Assert(pFrom.a[0].pUsing == null);
                    //}
                    //else
                    //{
                    //  sqlite3SelectDelete( db, ref pDup );
                    //}
                    pDup = sqlite3SelectNew(this, null, pFrom, pWhere, null, null, null, 0, null, null);
                }
                sqlite3SelectDestInit(dest, SelectResultType.EphemTab, iCur);
                sqlite3Select(this, pDup, ref dest);
                sqlite3SelectDelete(db, ref pDup);
            }
            public void sqlite3DeleteFrom(/* The parser context */SrcList pTabList,/* The table from which we should delete things */Expr pWhere/* The WHERE clause.  May be null */)
            {
                Vdbe v;
                /* The virtual database engine */
                Table pTab;
                /* The table from which records will be deleted */
                string zDb;
                /* Name of database holding pTab */
                int end, addr = 0;
                /* A couple addresses of generated code */
                int i;
                /* Loop counter */
                WhereInfo pWInfo;
                /* Information about the WHERE clause */
                Index pIdx;
                /* For looping over indices of the table */
                int iCur;
                /* VDBE VdbeCursor number for pTab */
                sqlite3 db;
                /* Main database structure */
                AuthContext sContext;
                /* Authorization context */
                NameContext sNC;
                /* Name context to resolve expressions in */
                int iDb;
                /* Database number */
                int memCnt = -1;
                /* Memory cell used for change counting */
                int rcauth;
                /* Value returned by authorization callback */
#if !SQLITE_OMIT_TRIGGER
                bool isView;
                /* True if attempting to delete from a view */
                Trigger pTrigger;
                /* List of table triggers, if required */
#endif
                sContext = new AuthContext();
                //memset(&sContext, 0, sizeof(sContext));
                db = this.db;
                if (this.nErr != 0/*|| db.mallocFailed != 0 */)
                {
                    goto delete_from_cleanup;
                }
                Debug.Assert(pTabList.nSrc == 1);
                /* Locate the table which we want to delete.  This table has to be
      ** put in an SrcList structure because some of the subroutines we
      ** will be calling are designed to work with multiple tables and expect
      ** an SrcList* parameter instead of just a Table* parameter.
      */
                pTab = this.sqlite3SrcListLookup(pTabList);
                if (pTab == null)
                    goto delete_from_cleanup;
                /* Figure out if we have any triggers and if the table being
      ** deleted from is a view
      */
#if !SQLITE_OMIT_TRIGGER
                int iDummy;
                pTrigger = sqlite3TriggersExist(this, pTab, TK_DELETE, null, out iDummy);
                isView = pTab.pSelect != null;
#else
																																																																																					      const Trigger pTrigger = null;
      bool isView = false;
#endif
#if SQLITE_OMIT_VIEW
																																																																																					// undef isView
isView = false;
#endif
                /* If pTab is really a view, make sure it has been initialized.
*/
                if (sqlite3ViewGetColumnNames(this, pTab) != 0)
                {
                    goto delete_from_cleanup;
                }
                if (this.sqlite3IsReadOnly(pTab, (pTrigger != null ? 1 : 0)))
                {
                    goto delete_from_cleanup;
                }
                iDb = sqlite3SchemaToIndex(db, pTab.pSchema);
                Debug.Assert(iDb < db.nDb);
                zDb = db.aDb[iDb].zName;
#if !SQLITE_OMIT_AUTHORIZATION
																																																																																					rcauth = sqlite3AuthCheck(pParse, SQLITE_DELETE, pTab->zName, 0, zDb);
#else
                rcauth = SQLITE_OK;
#endif
                Debug.Assert(rcauth == SQLITE_OK || rcauth == SQLITE_DENY || rcauth == SQLITE_IGNORE);
                if (rcauth == SQLITE_DENY)
                {
                    goto delete_from_cleanup;
                }
                Debug.Assert(!isView || pTrigger != null);
                /* Assign  cursor number to the table and all its indices.
      */
                Debug.Assert(pTabList.nSrc == 1);
                iCur = pTabList.a[0].iCursor = this.nTab++;
                for (pIdx = pTab.pIndex; pIdx != null; pIdx = pIdx.pNext)
                {
                    this.nTab++;
                }
#if !SQLITE_OMIT_AUTHORIZATION
																																																																																					/* Start the view context
*/
if( isView ){
sqlite3AuthContextPush(pParse, sContext, pTab.zName);
}
#endif
                /* Begin generating code.
*/
                v = sqlite3GetVdbe(this);
                if (v == null)
                {
                    goto delete_from_cleanup;
                }
                if (this.nested == 0)
                    sqlite3VdbeCountChanges(v);
                sqlite3BeginWriteOperation(this, 1, iDb);
                /* If we are trying to delete from a view, realize that view into
      ** a ephemeral table.
      */
#if !(SQLITE_OMIT_VIEW) && !(SQLITE_OMIT_TRIGGER)
                if (isView)
                {
                    this.sqlite3MaterializeView(pTab, pWhere, iCur);
                }
#endif
                /* Resolve the column names in the WHERE clause.
      */
                sNC = new NameContext();
                // memset( &sNC, 0, sizeof( sNC ) );
                sNC.pParse = this;
                sNC.pSrcList = pTabList;
                if (sqlite3ResolveExprNames(sNC, ref pWhere) != 0)
                {
                    goto delete_from_cleanup;
                }
                /* Initialize the counter of the number of rows deleted, if
** we are counting rows.
*/
                if ((db.flags & SQLITE_CountRows) != 0)
                {
                    memCnt = ++this.nMem;
                    v.sqlite3VdbeAddOp2(OP_Integer, 0, memCnt);
                }
#if !SQLITE_OMIT_TRUNCATE_OPTIMIZATION
                /* Special case: A DELETE without a WHERE clause deletes everything.
  ** It is easier just to erase the whole table. Prior to version 3.6.5,
  ** this optimization caused the row change count (the value returned by 
  ** API function sqlite3_count_changes) to be set incorrectly.  */
                if (rcauth == SQLITE_OK && pWhere == null && null == pTrigger && !IsVirtual(pTab) && 0 == this.sqlite3FkRequired(pTab, null, 0))
                {
                    Debug.Assert(!isView);
                    v.sqlite3VdbeAddOp4(OP_Clear, pTab.tnum, iDb, memCnt, pTab.zName, P4_STATIC);
                    for (pIdx = pTab.pIndex; pIdx != null; pIdx = pIdx.pNext)
                    {
                        Debug.Assert(pIdx.pSchema == pTab.pSchema);
                        v.sqlite3VdbeAddOp2(OP_Clear, pIdx.tnum, iDb);
                    }
                }
                else
#endif
                /* The usual case: There is a WHERE clause so we have to scan through
** the table and pick which records to delete.
*/
                {
                    int iRowSet = ++this.nMem;
                    /* Register for rowset of rows to delete */
                    int iRowid = ++this.nMem;
                    /* Used for storing rowid values. */
                    int regRowid;
                    /* Actual register containing rowids */
                    /* Collect rowids of every row to be deleted.
*/
                    v.sqlite3VdbeAddOp2(OP_Null, 0, iRowSet);
                    ExprList elDummy = null;
                    pWInfo = this.sqlite3WhereBegin(pTabList, pWhere, ref elDummy, WHERE_DUPLICATES_OK);
                    if (pWInfo == null)
                        goto delete_from_cleanup;
                    regRowid = this.sqlite3ExprCodeGetColumn(pTab, -1, iCur, iRowid);
                    v.sqlite3VdbeAddOp2(OP_RowSetAdd, iRowSet, regRowid);
                    if ((db.flags & SQLITE_CountRows) != 0)
                    {
                        v.sqlite3VdbeAddOp2(OP_AddImm, memCnt, 1);
                    }
                    pWInfo.sqlite3WhereEnd();
                    /* Delete every item whose key was written to the list during the
        ** database scan.  We have to delete items after the scan is complete
        ** because deleting an item can change the scan order. */
                    end = v.sqlite3VdbeMakeLabel();
                    /* Unless this is a view, open cursors for the table we are 
        ** deleting from and all its indices. If this is a view, then the
        ** only effect this statement has is to fire the INSTEAD OF 
        ** triggers.  */
                    if (!isView)
                    {
                        this.sqlite3OpenTableAndIndices(pTab, iCur, OP_OpenWrite);
                    }
                    addr = v.sqlite3VdbeAddOp3(OP_RowSetRead, iRowSet, end, iRowid);
                    /* Delete the row */
#if !SQLITE_OMIT_VIRTUALTABLE
                    if (IsVirtual(pTab))
                    {
                        VTable pVTab = sqlite3GetVTable(db, pTab);
                        this.sqlite3VtabMakeWritable(pTab);
                        v.sqlite3VdbeAddOp4(OP_VUpdate, 0, 1, iRowid, pVTab, P4_VTAB);
                        v.sqlite3VdbeChangeP5(OE_Abort);
                        sqlite3MayAbort(this);
                    }
                    else
#endif
                    {
                        int count = (this.nested == 0) ? 1 : 0;
                        /* True to count changes */
                        this.sqlite3GenerateRowDelete(pTab, iCur, iRowid, count, pTrigger, OE_Default);
                    }
                    /* End of the delete loop */
                    v.sqlite3VdbeAddOp2(OP_Goto, 0, addr);
                    v.sqlite3VdbeResolveLabel(end);
                    /* Close the cursors open on the table and its indexes. */
                    if (!isView && !IsVirtual(pTab))
                    {
                        for (i = 1, pIdx = pTab.pIndex; pIdx != null; i++, pIdx = pIdx.pNext)
                        {
                            v.sqlite3VdbeAddOp2(OP_Close, iCur + i, pIdx.tnum);
                        }
                        v.sqlite3VdbeAddOp1(OP_Close, iCur);
                    }
                }
                /* Update the sqlite_sequence table by storing the content of the
      ** maximum rowid counter values recorded while inserting into
      ** autoincrement tables.
      */
                if (this.nested == 0 && this.pTriggerTab == null)
                {
                    this.sqlite3AutoincrementEnd();
                }
                /* Return the number of rows that were deleted. If this routine is 
      ** generating code because of a call to sqlite3NestedParse(), do not
      ** invoke the callback function.
      */
                if ((db.flags & SQLITE_CountRows) != 0 && 0 == this.nested && null == this.pTriggerTab)
                {
                    v.sqlite3VdbeAddOp2(OP_ResultRow, memCnt, 1);
                    sqlite3VdbeSetNumCols(v, 1);
                    sqlite3VdbeSetColName(v, 0, COLNAME_NAME, "rows deleted", SQLITE_STATIC);
                }
            delete_from_cleanup:
#if !SQLITE_OMIT_AUTHORIZATION
																																																																																					sqlite3AuthContextPop(sContext);
#endif
                sqlite3SrcListDelete(db, ref pTabList);
                sqlite3ExprDelete(db, ref pWhere);
                return;
            }
            public void sqlite3GenerateRowDelete(/* Parsing context */Table pTab,/* Table containing the row to be deleted */int iCur,/* VdbeCursor number for the table */int iRowid,/* Memory cell that contains the rowid to delete */int count,/* If non-zero, increment the row change counter */Trigger pTrigger,/* List of triggers to (potentially) fire */int onconf/* Default ON CONFLICT policy for triggers */)
            {
                Vdbe v = this.pVdbe;
                /* Vdbe */
                int iOld = 0;
                /* First register in OLD.* array */
                int iLabel;
                /* Label resolved to end of generated code */
                /* Vdbe is guaranteed to have been allocated by this stage. */
                Debug.Assert(v != null);
                /* Seek cursor iCur to the row to delete. If this row no longer exists 
      ** (this can happen if a trigger program has already deleted it), do
      ** not attempt to delete it or fire any DELETE triggers.  */
                iLabel = v.sqlite3VdbeMakeLabel();
                v.sqlite3VdbeAddOp3(OP_NotExists, iCur, iLabel, iRowid);
                /* If there are any triggers to fire, allocate a range of registers to
      ** use for the old.* references in the triggers.  */
                if (this.sqlite3FkRequired(pTab, null, 0) != 0 || pTrigger != null)
                {
                    u32 mask;
                    /* Mask of OLD.* columns in use */
                    int iCol;
                    /* Iterator used while populating OLD.* */
                    /* TODO: Could use temporary registers here. Also could attempt to
** avoid copying the contents of the rowid register.  */
                    mask = sqlite3TriggerColmask(this, pTrigger, null, 0, TRIGGER_BEFORE | TRIGGER_AFTER, pTab, onconf);
                    mask |= this.sqlite3FkOldmask(pTab);
                    iOld = this.nMem + 1;
                    this.nMem += (1 + pTab.nCol);
                    /* Populate the OLD.* pseudo-table register array. These values will be 
        ** used by any BEFORE and AFTER triggers that exist.  */
                    v.sqlite3VdbeAddOp2(OP_Copy, iRowid, iOld);
                    for (iCol = 0; iCol < pTab.nCol; iCol++)
                    {
                        if (mask == 0xffffffff || (mask & (1 << iCol)) != 0)
                        {
                            v.sqlite3ExprCodeGetColumnOfTable(pTab, iCur, iCol, iOld + iCol + 1);
                        }
                    }
                    /* Invoke BEFORE DELETE trigger programs. */
                    sqlite3CodeRowTrigger(this, pTrigger, TK_DELETE, null, TRIGGER_BEFORE, pTab, iOld, onconf, iLabel);
                    /* Seek the cursor to the row to be deleted again. It may be that
        ** the BEFORE triggers coded above have already removed the row
        ** being deleted. Do not attempt to delete the row a second time, and 
        ** do not fire AFTER triggers.  */
                    v.sqlite3VdbeAddOp3(OP_NotExists, iCur, iLabel, iRowid);
                    /* Do FK processing. This call checks that any FK constraints that
        ** refer to this table (i.e. constraints attached to other tables) 
        ** are not violated by deleting this row.  */
                    this.sqlite3FkCheck(pTab, iOld, 0);
                }
                /* Delete the index and table entries. Skip this step if pTab is really
      ** a view (in which case the only effect of the DELETE statement is to
      ** fire the INSTEAD OF triggers).  */
                if (pTab.pSelect == null)
                {
                    this.sqlite3GenerateRowIndexDelete(pTab, iCur, 0);
                    v.sqlite3VdbeAddOp2(OP_Delete, iCur, (count != 0 ? (int)OPFLAG_NCHANGE : 0));
                    if (count != 0)
                    {
                        v.sqlite3VdbeChangeP4(-1, pTab.zName, P4_TRANSIENT);
                    }
                }
                /* Do any ON CASCADE, SET NULL or SET DEFAULT operations required to
      ** handle rows (possibly in other tables) that refer via a foreign key
      ** to the row just deleted. */
                this.sqlite3FkActions(pTab, null, iOld);
                /* Invoke AFTER DELETE trigger programs. */
                sqlite3CodeRowTrigger(this, pTrigger, TK_DELETE, null, TRIGGER_AFTER, pTab, iOld, onconf, iLabel);
                /* Jump here if the row had already been deleted before any BEFORE
      ** trigger programs were invoked. Or if a trigger program throws a 
      ** RAISE(IGNORE) exception.  */
                v.sqlite3VdbeResolveLabel(iLabel);
            }
            public void sqlite3GenerateRowIndexDelete(/* Parsing and code generating context */Table pTab,/* Table containing the row to be deleted */int iCur,/* VdbeCursor number for the table */int nothing/* Only delete if aRegIdx!=0 && aRegIdx[i]>0 */)
            {
                int[] aRegIdx = null;
                this.sqlite3GenerateRowIndexDelete(pTab, iCur, aRegIdx);
            }
            public void sqlite3GenerateRowIndexDelete(/* Parsing and code generating context */Table pTab,/* Table containing the row to be deleted */int iCur,/* VdbeCursor number for the table */int[] aRegIdx/* Only delete if aRegIdx!=0 && aRegIdx[i]>0 */)
            {
                int i;
                Index pIdx;
                int r1;
                for (i = 1, pIdx = pTab.pIndex; pIdx != null; i++, pIdx = pIdx.pNext)
                {
                    if (aRegIdx != null && aRegIdx[i - 1] == 0)
                        continue;
                    r1 = this.sqlite3GenerateIndexKey(pIdx, iCur, 0, false);
                    this.pVdbe.sqlite3VdbeAddOp3(OP_IdxDelete, iCur + i, r1, pIdx.nColumn + 1);
                }
            }
            public int sqlite3GenerateIndexKey(/* Parsing context */Index pIdx,/* The index for which to generate a key */int iCur,/* VdbeCursor number for the pIdx.pTable table */int regOut,/* Write the new index key to this register */bool doMakeRec/* Run the OP_MakeRecord instruction if true */)
            {
                Vdbe v = this.pVdbe;
                int j;
                Table pTab = pIdx.pTable;
                int regBase;
                int nCol;
                nCol = pIdx.nColumn;
                regBase = this.sqlite3GetTempRange(nCol + 1);
                v.sqlite3VdbeAddOp2(OP_Rowid, iCur, regBase + nCol);
                for (j = 0; j < nCol; j++)
                {
                    int idx = pIdx.aiColumn[j];
                    if (idx == pTab.iPKey)
                    {
                        v.sqlite3VdbeAddOp2(OP_SCopy, regBase + nCol, regBase + j);
                    }
                    else
                    {
                        v.sqlite3VdbeAddOp3(OP_Column, iCur, idx, regBase + j);
                        v.sqlite3ColumnDefault(pTab, idx, -1);
                    }
                }
                if (doMakeRec)
                {
                    string zAff;
                    if (pTab.pSelect != null || (this.db.flags & SQLITE_IdxRealAsInt) != 0)
                    {
                        zAff = "";
                    }
                    else
                    {
                        zAff = v.sqlite3IndexAffinityStr(pIdx);
                    }
                    v.sqlite3VdbeAddOp3(OP_MakeRecord, regBase, nCol + 1, regOut);
                    v.sqlite3VdbeChangeP4(-1, zAff, P4_TRANSIENT);
                }
                this.sqlite3ReleaseTempRange(regBase, nCol + 1);
                return regBase;
            }
            public Expr sqlite3ExprSetCollByToken(Expr pExpr, Token pCollName)
            {
                string zColl;
                /* Dequoted name of collation sequence */
                CollSeq pColl;
                sqlite3 db = this.db;
                zColl = sqlite3NameFromToken(db, pCollName);
                pColl = sqlite3LocateCollSeq(this, zColl);
                pExpr.sqlite3ExprSetColl(pColl);
                db.sqlite3DbFree(ref zColl);
                return pExpr;
            }
            public CollSeq sqlite3ExprCollSeq(Expr pExpr)
            {
                CollSeq pColl = null;
                Expr p = pExpr;
                while (ALWAYS(p))
                {
                    Operator op;
                    pColl = pExpr.pColl;
                    if (pColl != null)
                        break;
                    op = p.Operator;
                    if (p.pTab != null && (op == Operator.TK_AGG_COLUMN || op == Operator.TK_COLUMN || op == Operator.TK_REGISTER || op == Operator.TK_TRIGGER))
                    {
                        /* op==TK_REGISTER && p->pTab!=0 happens when pExpr was originally
          ** a TK_COLUMN but was previously evaluated and cached in a register */
                        string zColl;
                        int j = p.iColumn;
                        if (j >= 0)
                        {
                            sqlite3 db = this.db;
                            zColl = p.pTab.aCol[j].zColl;
                            pColl = sqlite3FindCollSeq(db, ENC(db), zColl, 0);
                            pExpr.pColl = pColl;
                        }
                        break;
                    }
                    if (op != Operator.TK_CAST && op != Operator.TK_UPLUS)
                    {
                        break;
                    }
                    p = p.pLeft;
                }
                if (sqlite3CheckCollSeq(this, pColl) != 0)
                {
                    pColl = null;
                }
                return pColl;
            }
            public CollSeq sqlite3BinaryCompareCollSeq(Expr pLeft, Expr pRight)
            {
                CollSeq pColl;
                Debug.Assert(pLeft != null);
                if ((pLeft.flags & EP_ExpCollate) != 0)
                {
                    Debug.Assert(pLeft.pColl != null);
                    pColl = pLeft.pColl;
                }
                else
                    if (pRight != null && ((pRight.flags & EP_ExpCollate) != 0))
                    {
                        Debug.Assert(pRight.pColl != null);
                        pColl = pRight.pColl;
                    }
                    else
                    {
                        pColl = this.sqlite3ExprCollSeq(pLeft);
                        if (pColl == null)
                        {
                            pColl = this.sqlite3ExprCollSeq(pRight);
                        }
                    }
                return pColl;
            }
            public int codeCompare(/* The parsing (and code generating) context */Expr pLeft,/* The left operand */Expr pRight,/* The right operand */int opcode,/* The comparison opcode */int in1, int in2,/* Register holding operands */int dest,/* Jump here if true.  */int jumpIfNull/* If true, jump if either operand is NULL */)
            {
                int p5;
                int addr;
                CollSeq p4;
                p4 = this.sqlite3BinaryCompareCollSeq(pLeft, pRight);
                p5 = pLeft.binaryCompareP5(pRight, jumpIfNull);
                addr = this.pVdbe.sqlite3VdbeAddOp4(opcode, in2, dest, in1, p4, P4_COLLSEQ);
                this.pVdbe.sqlite3VdbeChangeP5((u8)p5);
                return addr;
            }
            public int sqlite3ExprCheckHeight(int nHeight)
            {
                int rc = SQLITE_OK;
                int mxHeight = this.db.aLimit[SQLITE_LIMIT_EXPR_DEPTH];
                if (nHeight > mxHeight)
                {
                    sqlite3ErrorMsg(this, "Expression tree is too large (maximum depth %d)", mxHeight);
                    rc = SQLITE_ERROR;
                }
                return rc;
            }
            public void sqlite3ExprSetHeight(Expr p)
            {
                p.exprSetHeight();
                this.sqlite3ExprCheckHeight(p.nHeight);
            }
            public Expr sqlite3PExpr(int op, int null_3, int null_4, int null_5)
            {
                return this.sqlite3PExpr(op, null, null, null);
            }
            public Expr sqlite3PExpr(int op, int null_3, int null_4, Token pToken)
            {
                return this.sqlite3PExpr(op, null, null, pToken);
            }
            public Expr sqlite3PExpr(int op, Expr pLeft, int null_4, int null_5)
            {
                return this.sqlite3PExpr(op, pLeft, null, null);
            }
            public Expr sqlite3PExpr(int op, Expr pLeft, int null_4, Token pToken)
            {
                return this.sqlite3PExpr(op, pLeft, null, pToken);
            }
            public Expr sqlite3PExpr(int op, Expr pLeft, Expr pRight, int null_5)
            {
                return this.sqlite3PExpr(op, pLeft, pRight, null);
            }
            public Expr sqlite3PExpr(/* Parsing context */int op,/* Expression opcode */Expr pLeft,/* Left operand */Expr pRight,/* Right operand */Token pToken/* Argument Token */)
            {
                Log.WriteHeader(@"sqlite3PExpr");
                Log.Indent();
                if (null != pToken)
                {
                    Log.WriteLine(pToken.Length);
                    Log.WriteLine(pToken.zRestSql);
                }

                Expr p = sqlite3ExprAlloc(this.db, op, pToken, 1);
                sqlite3ExprAttachSubtrees(this.db, p, pLeft, pRight);
                if (p != null)
                {
                    this.sqlite3ExprCheckHeight(p.nHeight);
                }
                Log.Unindent();
                return p;
            }
            public void sqlite3ExprCacheStore(int iTab, int iCol, int iReg)
            {
                int i;
                int minLru;
                int idxLru;
                yColCache p = new yColCache();
                Debug.Assert(iReg > 0);
                /* Register numbers are always positive */
                Debug.Assert(iCol >= -1 && iCol < 32768);
                /* Finite column numbers */
                /* The SQLITE_ColumnCache flag disables the column cache.  This is used
** for testing only - to verify that SQLite always gets the same answer
** with and without the column cache.
*/
                if ((this.db.flags & SQLITE_ColumnCache) != 0)
                    return;
                /* First replace any existing entry.
      **
      ** Actually, the way the column cache is currently used, we are guaranteed
      ** that the object will never already be in cache.  Verify this guarantee.
      */
#if !NDEBUG
																																																																																					      for ( i = 0; i < SQLITE_N_COLCACHE; i++ )//p=pParse.aColCache... p++)
      {
#if FALSE
																																																																																					p = pParse.aColCache[i];
if ( p.iReg != 0 && p.iTable == iTab && p.iColumn == iCol )
{
cacheEntryClear( pParse, p );
p.iLevel = pParse.iCacheLevel;
p.iReg = iReg;
p.lru = pParse.iCacheCnt++;
return;
}
#endif
																																																																																					        Debug.Assert( p.iReg == 0 || p.iTable != iTab || p.iColumn != iCol );
      }
#endif
                /* Find an empty slot and replace it */
                for (i = 0; i < SQLITE_N_COLCACHE; i++)//p=pParse.aColCache... p++)
                {
                    p = this.aColCache[i];
                    if (p.iReg == 0)
                    {
                        p.iLevel = this.iCacheLevel;
                        p.iTable = iTab;
                        p.iColumn = iCol;
                        p.iReg = iReg;
                        p.tempReg = 0;
                        p.lru = this.iCacheCnt++;
                        return;
                    }
                }
                /* Replace the last recently used */
                minLru = 0x7fffffff;
                idxLru = -1;
                for (i = 0; i < SQLITE_N_COLCACHE; i++)//p=pParse.aColCache..., p++)
                {
                    p = this.aColCache[i];
                    if (p.lru < minLru)
                    {
                        idxLru = i;
                        minLru = p.lru;
                    }
                }
                if (ALWAYS(idxLru >= 0))
                {
                    p = this.aColCache[idxLru];
                    p.iLevel = this.iCacheLevel;
                    p.iTable = iTab;
                    p.iColumn = iCol;
                    p.iReg = iReg;
                    p.tempReg = 0;
                    p.lru = this.iCacheCnt++;
                    return;
                }
            }
            public void sqlite3ExprCacheRemove(int iReg, int nReg)
            {
                int i;
                int iLast = iReg + nReg - 1;
                yColCache p;
                for (i = 0; i < SQLITE_N_COLCACHE; i++)//p=pParse.aColCache... p++)
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
            public void sqlite3ExprCachePush()
            {
                this.iCacheLevel++;
            }
            public void sqlite3ExprCachePop(int N)
            {
                int i;
                yColCache p;
                Debug.Assert(N > 0);
                Debug.Assert(this.iCacheLevel >= N);
                this.iCacheLevel -= N;
                for (i = 0; i < SQLITE_N_COLCACHE; i++)// p++)
                {
                    p = this.aColCache[i];
                    if (p.iReg != 0 && p.iLevel > this.iCacheLevel)
                    {
                        this.cacheEntryClear(p);
                        p.iReg = 0;
                    }
                }
            }
            public void sqlite3ExprCachePinRegister(int iReg)
            {
                int i;
                yColCache p;
                for (i = 0; i < SQLITE_N_COLCACHE; i++)//p=pParse->aColCache; i<SQLITE_N_COLCACHE; i++, p++)
                {
                    p = this.aColCache[i];
                    if (p.iReg == iReg)
                    {
                        p.tempReg = 0;
                    }
                }
            }
            public int sqlite3ExprCodeGetColumn(/* Parsing and code generating context */Table pTab,/* Description of the table we are reading from */int iColumn,/* Index of the table column */int iTable,/* The cursor pointing to the table */int iReg/* Store results here */)
            {
                Vdbe v = this.pVdbe;
                int i;
                yColCache p;
                for (i = 0; i < SQLITE_N_COLCACHE; i++)
                {
                    // p=pParse.aColCache, p++
                    p = this.aColCache[i];
                    if (p.iReg > 0 && p.iTable == iTable && p.iColumn == iColumn)
                    {
                        p.lru = this.iCacheCnt++;
                        this.sqlite3ExprCachePinRegister(p.iReg);
                        return p.iReg;
                    }
                }
                Debug.Assert(v != null);
                v.sqlite3ExprCodeGetColumnOfTable(pTab, iTable, iColumn, iReg);
                this.sqlite3ExprCacheStore(iTable, iColumn, iReg);
                return iReg;
            }
            public void sqlite3ExprCacheClear()
            {
                int i;
                yColCache p;
                for (i = 0; i < SQLITE_N_COLCACHE; i++)// p=pParse.aColCache... p++)
                {
                    p = this.aColCache[i];
                    if (p.iReg != 0)
                    {
                        this.cacheEntryClear(p);
                        p.iReg = 0;
                    }
                }
            }
            public void sqlite3ExprCacheAffinityChange(int iStart, int iCount)
            {
                this.sqlite3ExprCacheRemove(iStart, iCount);
            }
            public void sqlite3ExprCodeMove(int iFrom, int iTo, int nReg)
            {
                int i;
                yColCache p;
                if (NEVER(iFrom == iTo))
                    return;
                this.pVdbe.sqlite3VdbeAddOp3(OP_Move, iFrom, iTo, nReg);
                for (i = 0; i < SQLITE_N_COLCACHE; i++)// p=pParse.aColCache... p++)
                {
                    p = this.aColCache[i];
                    int x = p.iReg;
                    if (x >= iFrom && x < iFrom + nReg)
                    {
                        p.iReg += iTo - iFrom;
                    }
                }
            }
            public void sqlite3ExprCodeCopy(int iFrom, int iTo, int nReg)
            {
                int i;
                if (NEVER(iFrom == iTo))
                    return;
                for (i = 0; i < nReg; i++)
                {
                    this.pVdbe.sqlite3VdbeAddOp2(OP_Copy, iFrom + i, iTo + i);
                }
            }
            public int usedAsColumnCache(int iFrom, int iTo)
            {
                return 0;
            }
            public int sqlite3ExprCodeTarget(Expr pExpr, int target)
            {
                Vdbe v = this.pVdbe;
                /* The VM under construction */
                int op;
                /* The opcode being coded */
                int inReg = target;
                /* Results stored in register inReg */
                int regFree1 = 0;
                /* If non-zero free this temporary register */
                int regFree2 = 0;
                /* If non-zero free this temporary register */
                int r1 = 0, r2 = 0, r3 = 0, r4 = 0;
                /* Various register numbers */
                sqlite3 db = this.db;
                /* The database connection */
                Debug.Assert(target > 0 && target <= this.nMem);
                if (v == null)
                {
                    //Debug.Assert( pParse.db.mallocFailed != 0 );
                    return 0;
                }
                if (pExpr == null)
                {
                    op = TK_NULL;
                }
                else
                {
                    op = pExpr.op;
                }
                switch (op)
                {
                    case TK_AGG_COLUMN:
                        {
                            AggInfo pAggInfo = pExpr.pAggInfo;
                            AggInfo_col pCol = pAggInfo.aCol[pExpr.iAgg];
                            if (pAggInfo.directMode == 0)
                            {
                                Debug.Assert(pCol.iMem > 0);
                                inReg = pCol.iMem;
                                break;
                            }
                            else
                                if (pAggInfo.useSortingIdx != 0)
                                {
                                    v.sqlite3VdbeAddOp3(OP_Column, pAggInfo.sortingIdx, pCol.iSorterColumn, target);
                                    break;
                                }
                            /* Otherwise, fall thru into the TK_COLUMN case */
                        }
                        goto case TK_COLUMN;
                    case TK_COLUMN:
                        {
                            if (pExpr.iTable < 0)
                            {
                                /* This only happens when coding check constraints */
                                Debug.Assert(this.ckBase > 0);
                                inReg = pExpr.iColumn + this.ckBase;
                            }
                            else
                            {
                                inReg = this.sqlite3ExprCodeGetColumn(pExpr.pTab, pExpr.iColumn, pExpr.iTable, target);
                            }
                            break;
                        }
                    case TK_INTEGER:
                        {
                            this.codeInteger(pExpr, false, target);
                            break;
                        }
#if !SQLITE_OMIT_FLOATING_POINT
                    case TK_FLOAT:
                        {
                            Debug.Assert(!ExprHasProperty(pExpr, EP_IntValue));
                            codeReal(v, pExpr.u.zToken, false, target);
                            break;
                        }
#endif
                    case TK_STRING:
                        {
                            Debug.Assert(!ExprHasProperty(pExpr, EP_IntValue));
                            v.sqlite3VdbeAddOp4(OP_String8, 0, target, 0, pExpr.u.zToken, 0);
                            break;
                        }
                    case TK_NULL:
                        {
                            v.sqlite3VdbeAddOp2(OP_Null, 0, target);
                            break;
                        }
#if !SQLITE_OMIT_BLOB_LITERAL
                    case TK_BLOB:
                        {
                            int n;
                            string z;
                            byte[] zBlob;
                            Debug.Assert(!ExprHasProperty(pExpr, EP_IntValue));
                            Debug.Assert(pExpr.u.zToken[0] == 'x' || pExpr.u.zToken[0] == 'X');
                            Debug.Assert(pExpr.u.zToken[1] == '\'');
                            z = pExpr.u.zToken.Substring(2);
                            n = StringExtensions.sqlite3Strlen30(z) - 1;
                            Debug.Assert(z[n] == '\'');
                            zBlob = Converter.sqlite3HexToBlob(sqlite3VdbeDb(v), z, n);
                            v.sqlite3VdbeAddOp4(OP_Blob, n / 2, target, 0, zBlob, P4_DYNAMIC);
                            break;
                        }
#endif
                    case TK_VARIABLE:
                        {
                            Debug.Assert(!ExprHasProperty(pExpr, EP_IntValue));
                            Debug.Assert(pExpr.u.zToken != null);
                            Debug.Assert(pExpr.u.zToken.Length != 0);
                            v.sqlite3VdbeAddOp2(OP_Variable, pExpr.iColumn, target);
                            if (pExpr.u.zToken.Length > 1)
                            {
                                Debug.Assert(pExpr.u.zToken[0] == '?' || pExpr.u.zToken.CompareTo(this.azVar[pExpr.iColumn - 1]) == 0);
                                v.sqlite3VdbeChangeP4(-1, this.azVar[pExpr.iColumn - 1], P4_STATIC);
                            }
                            break;
                        }
                    case TK_REGISTER:
                        {
                            inReg = pExpr.iTable;
                            break;
                        }
                    case TK_AS:
                        {
                            inReg = this.sqlite3ExprCodeTarget(pExpr.pLeft, target);
                            break;
                        }
#if !SQLITE_OMIT_CAST
                    case TK_CAST:
                        {
                            /* Expressions of the form:   CAST(pLeft AS token) */
                            int aff, to_op;
                            inReg = this.sqlite3ExprCodeTarget(pExpr.pLeft, target);
                            Debug.Assert(!ExprHasProperty(pExpr, EP_IntValue));
                            aff = sqlite3AffinityType(pExpr.u.zToken);
                            to_op = aff - SQLITE_AFF_TEXT + OP_ToText;
                            Debug.Assert(to_op == OP_ToText || aff != SQLITE_AFF_TEXT);
                            Debug.Assert(to_op == OP_ToBlob || aff != SQLITE_AFF_NONE);
                            Debug.Assert(to_op == OP_ToNumeric || aff != SQLITE_AFF_NUMERIC);
                            Debug.Assert(to_op == OP_ToInt || aff != SQLITE_AFF_INTEGER);
                            Debug.Assert(to_op == OP_ToReal || aff != SQLITE_AFF_REAL);
                            testcase(to_op == OP_ToText);
                            testcase(to_op == OP_ToBlob);
                            testcase(to_op == OP_ToNumeric);
                            testcase(to_op == OP_ToInt);
                            testcase(to_op == OP_ToReal);
                            if (inReg != target)
                            {
                                v.sqlite3VdbeAddOp2(OP_SCopy, inReg, target);
                                inReg = target;
                            }
                            v.sqlite3VdbeAddOp1(to_op, inReg);
                            testcase(this.usedAsColumnCache(inReg, inReg) != 0);
                            this.sqlite3ExprCacheAffinityChange(inReg, 1);
                            break;
                        }
#endif
                    case TK_LT:
                    case TK_LE:
                    case TK_GT:
                    case TK_GE:
                    case TK_NE:
                    case TK_EQ:
                        {
                            Debug.Assert(TK_LT == OP_Lt);
                            Debug.Assert(TK_LE == OP_Le);
                            Debug.Assert(TK_GT == OP_Gt);
                            Debug.Assert(TK_GE == OP_Ge);
                            Debug.Assert(TK_EQ == OP_Eq);
                            Debug.Assert(TK_NE == OP_Ne);
                            testcase(op == TK_LT);
                            testcase(op == TK_LE);
                            testcase(op == TK_GT);
                            testcase(op == TK_GE);
                            testcase(op == TK_EQ);
                            testcase(op == TK_NE);
                            r1 = this.sqlite3ExprCodeTemp(pExpr.pLeft, ref regFree1);
                            r2 = this.sqlite3ExprCodeTemp(pExpr.pRight, ref regFree2);
                            this.codeCompare(pExpr.pLeft, pExpr.pRight, op, r1, r2, inReg, SQLITE_STOREP2);
                            testcase(regFree1 == 0);
                            testcase(regFree2 == 0);
                            break;
                        }
                    case TK_IS:
                    case TK_ISNOT:
                        {
                            testcase(op == TK_IS);
                            testcase(op == TK_ISNOT);
                            r1 = this.sqlite3ExprCodeTemp(pExpr.pLeft, ref regFree1);
                            r2 = this.sqlite3ExprCodeTemp(pExpr.pRight, ref regFree2);
                            op = (op == TK_IS) ? TK_EQ : TK_NE;
                            this.codeCompare(pExpr.pLeft, pExpr.pRight, op, r1, r2, inReg, SQLITE_STOREP2 | SQLITE_NULLEQ);
                            testcase(regFree1 == 0);
                            testcase(regFree2 == 0);
                            break;
                        }
                    case TK_AND:
                    case TK_OR:
                    case TK_PLUS:
                    case TK_STAR:
                    case TK_MINUS:
                    case TK_REM:
                    case TK_BITAND:
                    case TK_BITOR:
                    case TK_SLASH:
                    case TK_LSHIFT:
                    case TK_RSHIFT:
                    case TK_CONCAT:
                        {
                            Debug.Assert(TK_AND == OP_And);
                            Debug.Assert(TK_OR == OP_Or);
                            Debug.Assert(TK_PLUS == OP_Add);
                            Debug.Assert(TK_MINUS == OP_Subtract);
                            Debug.Assert(TK_REM == OP_Remainder);
                            Debug.Assert(TK_BITAND == OP_BitAnd);
                            Debug.Assert(TK_BITOR == OP_BitOr);
                            Debug.Assert(TK_SLASH == OP_Divide);
                            Debug.Assert(TK_LSHIFT == OP_ShiftLeft);
                            Debug.Assert(TK_RSHIFT == OP_ShiftRight);
                            Debug.Assert(TK_CONCAT == OP_Concat);
                            testcase(op == TK_AND);
                            testcase(op == TK_OR);
                            testcase(op == TK_PLUS);
                            testcase(op == TK_MINUS);
                            testcase(op == TK_REM);
                            testcase(op == TK_BITAND);
                            testcase(op == TK_BITOR);
                            testcase(op == TK_SLASH);
                            testcase(op == TK_LSHIFT);
                            testcase(op == TK_RSHIFT);
                            testcase(op == TK_CONCAT);
                            r1 = this.sqlite3ExprCodeTemp(pExpr.pLeft, ref regFree1);
                            r2 = this.sqlite3ExprCodeTemp(pExpr.pRight, ref regFree2);
                            v.sqlite3VdbeAddOp3(op, r2, r1, target);
                            testcase(regFree1 == 0);
                            testcase(regFree2 == 0);
                            break;
                        }
                    case TK_UMINUS:
                        {
                            Expr pLeft = pExpr.pLeft;
                            Debug.Assert(pLeft != null);
                            if (pLeft.Operator == Operator.TK_INTEGER)
                            {
                                this.codeInteger(pLeft, true, target);
#if !SQLITE_OMIT_FLOATING_POINT
                            }
                            else
                                if (pLeft.Operator == Operator.TK_FLOAT)
                                {
                                    Debug.Assert(!ExprHasProperty(pExpr, EP_IntValue));
                                    codeReal(v, pLeft.u.zToken, true, target);
#endif
                                }
                                else
                                {
                                    regFree1 = r1 = this.sqlite3GetTempReg();
                                    v.sqlite3VdbeAddOp2(OP_Integer, 0, r1);
                                    r2 = this.sqlite3ExprCodeTemp(pExpr.pLeft, ref regFree2);
                                    v.sqlite3VdbeAddOp3(OP_Subtract, r2, r1, target);
                                    testcase(regFree2 == 0);
                                }
                            inReg = target;
                            break;
                        }
                    case TK_BITNOT:
                    case TK_NOT:
                        {
                            Debug.Assert(TK_BITNOT == OP_BitNot);
                            Debug.Assert(TK_NOT == OP_Not);
                            testcase(op == TK_BITNOT);
                            testcase(op == TK_NOT);
                            r1 = this.sqlite3ExprCodeTemp(pExpr.pLeft, ref regFree1);
                            testcase(regFree1 == 0);
                            inReg = target;
                            v.sqlite3VdbeAddOp2(op, r1, inReg);
                            break;
                        }
                    case TK_ISNULL:
                    case TK_NOTNULL:
                        {
                            int addr;
                            Debug.Assert(TK_ISNULL == OP_IsNull);
                            Debug.Assert(TK_NOTNULL == OP_NotNull);
                            testcase(op == TK_ISNULL);
                            testcase(op == TK_NOTNULL);
                            v.sqlite3VdbeAddOp2(OP_Integer, 1, target);
                            r1 = this.sqlite3ExprCodeTemp(pExpr.pLeft, ref regFree1);
                            testcase(regFree1 == 0);
                            addr = v.sqlite3VdbeAddOp1(op, r1);
                            v.sqlite3VdbeAddOp2(OP_AddImm, target, -1);
                            v.sqlite3VdbeJumpHere(addr);
                            break;
                        }
                    case TK_AGG_FUNCTION:
                        {
                            AggInfo pInfo = pExpr.pAggInfo;
                            if (pInfo == null)
                            {
                                Debug.Assert(!ExprHasProperty(pExpr, EP_IntValue));
                                sqlite3ErrorMsg(this, "misuse of aggregate: %s()", pExpr.u.zToken);
                            }
                            else
                            {
                                inReg = pInfo.aFunc[pExpr.iAgg].iMem;
                            }
                            break;
                        }
                    case TK_CONST_FUNC:
                    case TK_FUNCTION:
                        {
                            ExprList pFarg;
                            /* List of function arguments */
                            int nFarg;
                            /* Number of function arguments */
                            FuncDef pDef;
                            /* The function definition object */
                            int nId;
                            /* Length of the function name in bytes */
                            string zId;
                            /* The function name */
                            int constMask = 0;
                            /* Mask of function arguments that are constant */
                            int i;
                            /* Loop counter */
                            SqliteEncoding enc = ENC(db);
                            /* The text encoding used by this database */
                            CollSeq pColl = null;
                            /* A collating sequence */
                            Debug.Assert(!ExprHasProperty(pExpr, EP_xIsSelect));
                            testcase(op == TK_CONST_FUNC);
                            testcase(op == TK_FUNCTION);
                            if (ExprHasAnyProperty(pExpr, EP_TokenOnly))
                            {
                                pFarg = null;
                            }
                            else
                            {
                                pFarg = pExpr.x.pList;
                            }
                            nFarg = pFarg != null ? pFarg.nExpr : 0;
                            Debug.Assert(!ExprHasProperty(pExpr, EP_IntValue));
                            zId = pExpr.u.zToken;
                            nId = StringExtensions.sqlite3Strlen30(zId);
                            pDef = sqlite3FindFunction(this.db, zId, nId, nFarg, enc, 0);
                            if (pDef == null)
                            {
                                sqlite3ErrorMsg(this, "unknown function: %.*s()", nId, zId);
                                break;
                            }
                            /* Attempt a direct implementation of the built-in COALESCE() and
                    ** IFNULL() functions.  This avoids unnecessary evalation of
                    ** arguments past the first non-NULL argument.
                    */
                            if ((pDef.flags & SQLITE_FUNC_COALESCE) != 0)
                            {
                                int endCoalesce = v.sqlite3VdbeMakeLabel();
                                Debug.Assert(nFarg >= 2);
                                this.sqlite3ExprCode(pFarg.a[0].pExpr, target);
                                for (i = 1; i < nFarg; i++)
                                {
                                    v.sqlite3VdbeAddOp2(OP_NotNull, target, endCoalesce);
                                    this.sqlite3ExprCacheRemove(target, 1);
                                    this.sqlite3ExprCachePush();
                                    this.sqlite3ExprCode(pFarg.a[i].pExpr, target);
                                    this.sqlite3ExprCachePop(1);
                                }
                                v.sqlite3VdbeResolveLabel(endCoalesce);
                                break;
                            }
                            if (pFarg != null)
                            {
                                r1 = this.sqlite3GetTempRange(nFarg);
                                this.sqlite3ExprCachePush();
                                /* Ticket 2ea2425d34be */
                                this.sqlite3ExprCodeExprList(pFarg, r1, true);
                                this.sqlite3ExprCachePop(1);
                                /* Ticket 2ea2425d34be */
                            }
                            else
                            {
                                r1 = 0;
                            }
#if !SQLITE_OMIT_VIRTUALTABLE
                            /* Possibly overload the function if the first argument is
** a virtual table column.
**
** For infix functions (LIKE, GLOB, REGEXP, and MATCH) use the
** second argument, not the first, as the argument to test to
** see if it is a column in a virtual table.  This is done because
** the left operand of infix functions (the operand we want to
** control overloading) ends up as the second argument to the
** function.  The expression "A glob B" is equivalent to
** "glob(B,A).  We want to use the A in "A glob B" to test
** for function overloading.  But we use the B term in "glob(B,A)".
*/
                            if (nFarg >= 2 && (pExpr.flags & EP_InfixFunc) != 0)
                            {
                                pDef = sqlite3VtabOverloadFunction(db, pDef, nFarg, pFarg.a[1].pExpr);
                            }
                            else
                                if (nFarg > 0)
                                {
                                    pDef = sqlite3VtabOverloadFunction(db, pDef, nFarg, pFarg.a[0].pExpr);
                                }
#endif
                            for (i = 0; i < nFarg; i++)
                            {
                                if (i < 32 && pFarg.a[i].pExpr.sqlite3ExprIsConstant() != 0)
                                {
                                    constMask |= (1 << i);
                                }
                                if ((pDef.flags & SQLITE_FUNC_NEEDCOLL) != 0 && null == pColl)
                                {
                                    pColl = this.sqlite3ExprCollSeq(pFarg.a[i].pExpr);
                                }
                            }
                            if ((pDef.flags & SQLITE_FUNC_NEEDCOLL) != 0)
                            {
                                if (null == pColl)
                                    pColl = db.pDfltColl;
                                v.sqlite3VdbeAddOp4(OP_CollSeq, 0, 0, 0, pColl, P4_COLLSEQ);
                            }
                            v.sqlite3VdbeAddOp4(OP_Function, constMask, r1, target, pDef, P4_FUNCDEF);
                            v.sqlite3VdbeChangeP5((u8)nFarg);
                            if (nFarg != 0)
                            {
                                this.sqlite3ReleaseTempRange(r1, nFarg);
                            }
                            break;
                        }
#if !SQLITE_OMIT_SUBQUERY
                    case TK_EXISTS:
                    case TK_SELECT:
                        {
                            testcase(op == TK_EXISTS);
                            testcase(op == TK_SELECT);
                            inReg = this.sqlite3CodeSubselect(pExpr, 0, false);
                            break;
                        }
                    case TK_IN:
                        {
                            int destIfFalse = v.sqlite3VdbeMakeLabel();
                            int destIfNull = v.sqlite3VdbeMakeLabel();
                            v.sqlite3VdbeAddOp2(OP_Null, 0, target);
                            this.sqlite3ExprCodeIN(pExpr, destIfFalse, destIfNull);
                            v.sqlite3VdbeAddOp2(OP_Integer, 1, target);
                            v.sqlite3VdbeResolveLabel(destIfFalse);
                            v.sqlite3VdbeAddOp2(OP_AddImm, target, 0);
                            v.sqlite3VdbeResolveLabel(destIfNull);
                            break;
                        }
#endif
                    /*
**    x BETWEEN y AND z
**
** This is equivalent to
**
**    x>=y AND x<=z
**
** X is stored in pExpr.pLeft.
** Y is stored in pExpr.x.pList.a[0].pExpr.
** Z is stored in pExpr.x.pList.a[1].pExpr.
*/
                    case TK_BETWEEN:
                        {
                            Expr pLeft = pExpr.pLeft;
                            ExprList_item pLItem = pExpr.x.pList.a[0];
                            Expr pRight = pLItem.pExpr;
                            r1 = this.sqlite3ExprCodeTemp(pLeft, ref regFree1);
                            r2 = this.sqlite3ExprCodeTemp(pRight, ref regFree2);
                            testcase(regFree1 == 0);
                            testcase(regFree2 == 0);
                            r3 = this.sqlite3GetTempReg();
                            r4 = this.sqlite3GetTempReg();
                            this.codeCompare(pLeft, pRight, OP_Ge, r1, r2, r3, SQLITE_STOREP2);
                            pLItem = pExpr.x.pList.a[1];
                            // pLItem++;
                            pRight = pLItem.pExpr;
                            this.sqlite3ReleaseTempReg(regFree2);
                            r2 = this.sqlite3ExprCodeTemp(pRight, ref regFree2);
                            testcase(regFree2 == 0);
                            this.codeCompare(pLeft, pRight, OP_Le, r1, r2, r4, SQLITE_STOREP2);
                            v.sqlite3VdbeAddOp3(OP_And, r3, r4, target);
                            this.sqlite3ReleaseTempReg(r3);
                            this.sqlite3ReleaseTempReg(r4);
                            break;
                        }
                    case TK_UPLUS:
                        {
                            inReg = this.sqlite3ExprCodeTarget(pExpr.pLeft, target);
                            break;
                        }
                    case TK_TRIGGER:
                        {
                            /* If the opcode is TK_TRIGGER, then the expression is a reference
                    ** to a column in the new.* or old.* pseudo-tables available to
                    ** trigger programs. In this case Expr.iTable is set to 1 for the
                    ** new.* pseudo-table, or 0 for the old.* pseudo-table. Expr.iColumn
                    ** is set to the column of the pseudo-table to read, or to -1 to
                    ** read the rowid field.
                    **
                    ** The expression is implemented using an OP_Param opcode. The p1
                    ** parameter is set to 0 for an old.rowid reference, or to (i+1)
                    ** to reference another column of the old.* pseudo-table, where 
                    ** i is the index of the column. For a new.rowid reference, p1 is
                    ** set to (n+1), where n is the number of columns in each pseudo-table.
                    ** For a reference to any other column in the new.* pseudo-table, p1
                    ** is set to (n+2+i), where n and i are as defined previously. For
                    ** example, if the table on which triggers are being fired is
                    ** declared as:
                    **
                    **   CREATE TABLE t1(a, b);
                    **
                    ** Then p1 is interpreted as follows:
                    **
                    **   p1==0   .    old.rowid     p1==3   .    new.rowid
                    **   p1==1   .    old.a         p1==4   .    new.a
                    **   p1==2   .    old.b         p1==5   .    new.b       
                    */
                            Table pTab = pExpr.pTab;
                            int p1 = pExpr.iTable * (pTab.nCol + 1) + 1 + pExpr.iColumn;
                            Debug.Assert(pExpr.iTable == 0 || pExpr.iTable == 1);
                            Debug.Assert(pExpr.iColumn >= -1 && pExpr.iColumn < pTab.nCol);
                            Debug.Assert(pTab.iPKey < 0 || pExpr.iColumn != pTab.iPKey);
                            Debug.Assert(p1 >= 0 && p1 < (pTab.nCol * 2 + 2));
                            v.sqlite3VdbeAddOp2(OP_Param, p1, target);
                            VdbeComment(v, "%s.%s -> $%d", (pExpr.iTable != 0 ? "new" : "old"), (pExpr.iColumn < 0 ? "rowid" : pExpr.pTab.aCol[pExpr.iColumn].zName), target);
                            /* If the column has REAL affinity, it may currently be stored as an
                    ** integer. Use OP_RealAffinity to make sure it is really real.  */
                            if (pExpr.iColumn >= 0 && pTab.aCol[pExpr.iColumn].affinity == SQLITE_AFF_REAL)
                            {
                                v.sqlite3VdbeAddOp1(OP_RealAffinity, target);
                            }
                            break;
                        }
                    /*
            ** Form A:
            **   CASE x WHEN e1 THEN r1 WHEN e2 THEN r2 ... WHEN eN THEN rN ELSE y END
            **
            ** Form B:
            **   CASE WHEN e1 THEN r1 WHEN e2 THEN r2 ... WHEN eN THEN rN ELSE y END
            **
            ** Form A is can be transformed into the equivalent form B as follows:
            **   CASE WHEN x=e1 THEN r1 WHEN x=e2 THEN r2 ...
            **        WHEN x=eN THEN rN ELSE y END
            **
            ** X (if it exists) is in pExpr.pLeft.
            ** Y is in pExpr.pRight.  The Y is also optional.  If there is no
            ** ELSE clause and no other term matches, then the result of the
            ** exprssion is NULL.
            ** Ei is in pExpr.x.pList.a[i*2] and Ri is pExpr.x.pList.a[i*2+1].
            **
            ** The result of the expression is the Ri for the first matching Ei,
            ** or if there is no matching Ei, the ELSE term Y, or if there is
            ** no ELSE term, NULL.
            */
                    default:
                        {
                            Debug.Assert(op == TK_CASE);
                            int endLabel;
                            /* GOTO label for end of CASE stmt */
                            int nextCase;
                            /* GOTO label for next WHEN clause */
                            int nExpr;
                            /* 2x number of WHEN terms */
                            int i;
                            /* Loop counter */
                            ExprList pEList;
                            /* List of WHEN terms */
                            ExprList_item[] aListelem;
                            /* Array of WHEN terms */
                            Expr opCompare = new Expr();
                            /* The X==Ei expression */
                            Expr cacheX;
                            /* Cached expression X */
                            Expr pX;
                            /* The X expression */
                            Expr pTest = null;
                            /* X==Ei (form A) or just Ei (form B) */
#if !NDEBUG
																																																																																																															            int iCacheLevel = pParse.iCacheLevel;
            //VVA_ONLY( int iCacheLevel = pParse.iCacheLevel; )
#endif
                            Debug.Assert(!ExprHasProperty(pExpr, EP_xIsSelect) && pExpr.x.pList != null);
                            Debug.Assert((pExpr.x.pList.nExpr % 2) == 0);
                            Debug.Assert(pExpr.x.pList.nExpr > 0);
                            pEList = pExpr.x.pList;
                            aListelem = pEList.a;
                            nExpr = pEList.nExpr;
                            endLabel = v.sqlite3VdbeMakeLabel();
                            if ((pX = pExpr.pLeft) != null)
                            {
                                cacheX = pX;
                                testcase(pX.Operator == Operator.TK_COLUMN);
                                testcase(pX.Operator == Operator.TK_REGISTER);
                                cacheX.iTable = this.sqlite3ExprCodeTemp(pX, ref regFree1);
                                testcase(regFree1 == 0);
                                cacheX.Operator = Operator.TK_REGISTER;
                                opCompare.Operator = Operator.TK_EQ;
                                opCompare.pLeft = cacheX;
                                pTest = opCompare;
                                /* Ticket b351d95f9cd5ef17e9d9dbae18f5ca8611190001:
                      ** The value in regFree1 might get SCopy-ed into the file result.
                      ** So make sure that the regFree1 register is not reused for other
                      ** purposes and possibly overwritten.  */
                                regFree1 = 0;
                            }
                            for (i = 0; i < nExpr; i = i + 2)
                            {
                                this.sqlite3ExprCachePush();
                                if (pX != null)
                                {
                                    Debug.Assert(pTest != null);
                                    opCompare.pRight = aListelem[i].pExpr;
                                }
                                else
                                {
                                    pTest = aListelem[i].pExpr;
                                }
                                nextCase = v.sqlite3VdbeMakeLabel();
                                testcase(pTest.op == TK_COLUMN);
                                this.sqlite3ExprIfFalse(pTest, nextCase, SQLITE_JUMPIFNULL);
                                testcase(aListelem[i + 1].pExpr.op == TK_COLUMN);
                                testcase(aListelem[i + 1].pExpr.op == TK_REGISTER);
                                this.sqlite3ExprCode(aListelem[i + 1].pExpr, target);
                                v.sqlite3VdbeAddOp2(OP_Goto, 0, endLabel);
                                this.sqlite3ExprCachePop(1);
                                v.sqlite3VdbeResolveLabel(nextCase);
                            }
                            if (pExpr.pRight != null)
                            {
                                this.sqlite3ExprCachePush();
                                this.sqlite3ExprCode(pExpr.pRight, target);
                                this.sqlite3ExprCachePop(1);
                            }
                            else
                            {
                                v.sqlite3VdbeAddOp2(OP_Null, 0, target);
                            }
#if !NDEBUG
																																																																																																															            Debug.Assert( /* db.mallocFailed != 0 || */ pParse.nErr > 0
            || pParse.iCacheLevel == iCacheLevel );
#endif
                            v.sqlite3VdbeResolveLabel(endLabel);
                            break;
                        }
#if !SQLITE_OMIT_TRIGGER
                    case TK_RAISE:
                        {
                            Debug.Assert(pExpr.affinity == OE_Rollback || pExpr.affinity == OE_Abort || pExpr.affinity == OE_Fail || pExpr.affinity == OE_Ignore);
                            if (null == this.pTriggerTab)
                            {
                                sqlite3ErrorMsg(this, "RAISE() may only be used within a trigger-program");
                                return 0;
                            }
                            if (pExpr.affinity == OE_Abort)
                            {
                                sqlite3MayAbort(this);
                            }
                            Debug.Assert(!ExprHasProperty(pExpr, EP_IntValue));
                            if (pExpr.affinity == OE_Ignore)
                            {
                                v.sqlite3VdbeAddOp4(OP_Halt, SQLITE_OK, OE_Ignore, 0, pExpr.u.zToken, 0);
                            }
                            else
                            {
                                sqlite3HaltConstraint(this, pExpr.affinity, pExpr.u.zToken, 0);
                            }
                            break;
                        }
#endif
                }
                this.sqlite3ReleaseTempReg(regFree1);
                this.sqlite3ReleaseTempReg(regFree2);
                return inReg;
            }
            public int sqlite3ExprCodeTemp(Expr pExpr, ref int pReg)
            {
                int r1 = this.sqlite3GetTempReg();
                int r2 = this.sqlite3ExprCodeTarget(pExpr, r1);
                if (r2 == r1)
                {
                    pReg = r1;
                }
                else
                {
                    this.sqlite3ReleaseTempReg(r1);
                    pReg = 0;
                }
                return r2;
            }
            public int sqlite3ExprCode(Expr pExpr, int target)
            {
                int inReg;
                Debug.Assert(target > 0 && target <= this.nMem);
                if (pExpr != null && pExpr.op == TK_REGISTER)
                {
                    this.pVdbe.sqlite3VdbeAddOp2(OP_Copy, pExpr.iTable, target);
                }
                else
                {
                    inReg = this.sqlite3ExprCodeTarget(pExpr, target);
                    Debug.Assert(this.pVdbe != null/* || pParse.db.mallocFailed != 0 */);
                    if (inReg != target && this.pVdbe != null)
                    {
                        this.pVdbe.sqlite3VdbeAddOp2(OP_SCopy, inReg, target);
                    }
                }
                return target;
            }
            public int sqlite3ExprCodeAndCache(Expr pExpr, int target)
            {
                Vdbe v = this.pVdbe;
                int inReg;
                inReg = this.sqlite3ExprCode(pExpr, target);
                Debug.Assert(target > 0);
                /* This routine is called for terms to INSERT or UPDATE.  And the only
      ** other place where expressions can be converted into TK_REGISTER is
      ** in WHERE clause processing.  So as currently implemented, there is
      ** no way for a TK_REGISTER to exist here.  But it seems prudent to
      ** keep the ALWAYS() in case the conditions above change with future
      ** modifications or enhancements. */
                if (ALWAYS(pExpr.op != TK_REGISTER))
                {
                    int iMem;
                    iMem = ++this.nMem;
                    v.sqlite3VdbeAddOp2(OP_Copy, inReg, iMem);
                    pExpr.iTable = iMem;
                    pExpr.op2 = pExpr.op;
                    pExpr.op = TK_REGISTER;
                }
                return inReg;
            }
            public void sqlite3ExprCodeConstants(Expr pExpr)
            {
                Walker w;
                if (this.cookieGoto != 0)
                    return;
                if ((this.db.flags & SQLITE_FactorOutConst) != 0)
                    return;
                w = new Walker();
                w.xExprCallback = (dxExprCallback)evalConstExpr;
                w.xSelectCallback = null;
                w.pParse = this;
                w.sqlite3WalkExpr(ref pExpr);
            }
            public int sqlite3ExprCodeExprList(/* Parsing context */ExprList pList,/* The expression list to be coded */int target,/* Where to write results */bool doHardCopy/* Make a hard copy of every element */)
            {
                ExprList_item pItem;
                int i, n;
                Debug.Assert(pList != null);
                Debug.Assert(target > 0);
                Debug.Assert(this.pVdbe != null);
                /* Never gets this far otherwise */
                n = pList.nExpr;
                for (i = 0; i < n; i++)// pItem++)
                {
                    pItem = pList.a[i];
                    Expr pExpr = pItem.pExpr;
                    int inReg = this.sqlite3ExprCodeTarget(pExpr, target + i);
                    if (inReg != target + i)
                    {
                        this.pVdbe.sqlite3VdbeAddOp2(doHardCopy ? OP_Copy : OP_SCopy, inReg, target + i);
                    }
                }
                return n;
            }
            public void exprCodeBetween(/* Parsing and code generating context */Expr pExpr,/* The BETWEEN expression */int dest,/* Jump here if the jump is taken */int jumpIfTrue,/* Take the jump if the BETWEEN is true */int jumpIfNull/* Take the jump if the BETWEEN is NULL */)
            {
                Expr exprAnd = new Expr();
                /* The AND operator in  x>=y AND x<=z  */
                Expr compLeft = new Expr();
                /* The  x>=y  term */
                Expr compRight = new Expr();
                /* The  x<=z  term */
                Expr exprX;
                /* The  x  subexpression */
                int regFree1 = 0;
                /* Temporary use register */
                Debug.Assert(!ExprHasProperty(pExpr, EP_xIsSelect));
                exprX = pExpr.pLeft.Copy();
                exprAnd.op = TK_AND;
                exprAnd.pLeft = compLeft;
                exprAnd.pRight = compRight;
                compLeft.op = TK_GE;
                compLeft.pLeft = exprX;
                compLeft.pRight = pExpr.x.pList.a[0].pExpr;
                compRight.op = TK_LE;
                compRight.pLeft = exprX;
                compRight.pRight = pExpr.x.pList.a[1].pExpr;
                exprX.iTable = this.sqlite3ExprCodeTemp(exprX, ref regFree1);
                exprX.op = TK_REGISTER;
                if (jumpIfTrue != 0)
                {
                    this.sqlite3ExprIfTrue(exprAnd, dest, jumpIfNull);
                }
                else
                {
                    this.sqlite3ExprIfFalse(exprAnd, dest, jumpIfNull);
                }
                this.sqlite3ReleaseTempReg(regFree1);
                /* Ensure adequate test coverage */
                testcase(jumpIfTrue == 0 && jumpIfNull == 0 && regFree1 == 0);
                testcase(jumpIfTrue == 0 && jumpIfNull == 0 && regFree1 != 0);
                testcase(jumpIfTrue == 0 && jumpIfNull != 0 && regFree1 == 0);
                testcase(jumpIfTrue == 0 && jumpIfNull != 0 && regFree1 != 0);
                testcase(jumpIfTrue != 0 && jumpIfNull == 0 && regFree1 == 0);
                testcase(jumpIfTrue != 0 && jumpIfNull == 0 && regFree1 != 0);
                testcase(jumpIfTrue != 0 && jumpIfNull != 0 && regFree1 == 0);
                testcase(jumpIfTrue != 0 && jumpIfNull != 0 && regFree1 != 0);
            }
            public void sqlite3ExprIfTrue(Expr pExpr, int dest, int jumpIfNull)
            {
                Vdbe v = this.pVdbe;
                int op = 0;
                int regFree1 = 0;
                int regFree2 = 0;
                int r1 = 0, r2 = 0;
                Debug.Assert(jumpIfNull == SQLITE_JUMPIFNULL || jumpIfNull == 0);
                if (NEVER(v == null))
                    return;
                /* Existance of VDBE checked by caller */
                if (NEVER(pExpr == null))
                    return;
                /* No way this can happen */
                op = pExpr.op;
                switch (op)
                {
                    case TK_AND:
                        {
                            int d2 = v.sqlite3VdbeMakeLabel();
                            testcase(jumpIfNull == 0);
                            this.sqlite3ExprCachePush();
                            this.sqlite3ExprIfFalse(pExpr.pLeft, d2, jumpIfNull ^ SQLITE_JUMPIFNULL);
                            this.sqlite3ExprIfTrue(pExpr.pRight, dest, jumpIfNull);
                            v.sqlite3VdbeResolveLabel(d2);
                            this.sqlite3ExprCachePop(1);
                            break;
                        }
                    case TK_OR:
                        {
                            testcase(jumpIfNull == 0);
                            this.sqlite3ExprIfTrue(pExpr.pLeft, dest, jumpIfNull);
                            this.sqlite3ExprIfTrue(pExpr.pRight, dest, jumpIfNull);
                            break;
                        }
                    case TK_NOT:
                        {
                            testcase(jumpIfNull == 0);
                            this.sqlite3ExprIfFalse(pExpr.pLeft, dest, jumpIfNull);
                            break;
                        }
                    case TK_LT:
                    case TK_LE:
                    case TK_GT:
                    case TK_GE:
                    case TK_NE:
                    case TK_EQ:
                        {
                            Debug.Assert(TK_LT == OP_Lt);
                            Debug.Assert(TK_LE == OP_Le);
                            Debug.Assert(TK_GT == OP_Gt);
                            Debug.Assert(TK_GE == OP_Ge);
                            Debug.Assert(TK_EQ == OP_Eq);
                            Debug.Assert(TK_NE == OP_Ne);
                            testcase(op == TK_LT);
                            testcase(op == TK_LE);
                            testcase(op == TK_GT);
                            testcase(op == TK_GE);
                            testcase(op == TK_EQ);
                            testcase(op == TK_NE);
                            testcase(jumpIfNull == 0);
                            r1 = this.sqlite3ExprCodeTemp(pExpr.pLeft, ref regFree1);
                            r2 = this.sqlite3ExprCodeTemp(pExpr.pRight, ref regFree2);
                            this.codeCompare(pExpr.pLeft, pExpr.pRight, op, r1, r2, dest, jumpIfNull);
                            testcase(regFree1 == 0);
                            testcase(regFree2 == 0);
                            break;
                        }
                    case TK_IS:
                    case TK_ISNOT:
                        {
                            testcase(op == TK_IS);
                            testcase(op == TK_ISNOT);
                            r1 = this.sqlite3ExprCodeTemp(pExpr.pLeft, ref regFree1);
                            r2 = this.sqlite3ExprCodeTemp(pExpr.pRight, ref regFree2);
                            op = (op == TK_IS) ? TK_EQ : TK_NE;
                            this.codeCompare(pExpr.pLeft, pExpr.pRight, op, r1, r2, dest, SQLITE_NULLEQ);
                            testcase(regFree1 == 0);
                            testcase(regFree2 == 0);
                            break;
                        }
                    case TK_ISNULL:
                    case TK_NOTNULL:
                        {
                            Debug.Assert(TK_ISNULL == OP_IsNull);
                            Debug.Assert(TK_NOTNULL == OP_NotNull);
                            testcase(op == TK_ISNULL);
                            testcase(op == TK_NOTNULL);
                            r1 = this.sqlite3ExprCodeTemp(pExpr.pLeft, ref regFree1);
                            v.sqlite3VdbeAddOp2(op, r1, dest);
                            testcase(regFree1 == 0);
                            break;
                        }
                    case TK_BETWEEN:
                        {
                            testcase(jumpIfNull == 0);
                            this.exprCodeBetween(pExpr, dest, 1, jumpIfNull);
                            break;
                        }
#if SQLITE_OMIT_SUBQUERY
																																																																																					        case TK_IN:
          {
            int destIfFalse = sqlite3VdbeMakeLabel( v );
            int destIfNull = jumpIfNull != 0 ? dest : destIfFalse;
            sqlite3ExprCodeIN( pParse, pExpr, destIfFalse, destIfNull );
            sqlite3VdbeAddOp2( v, OP_Goto, 0, dest );
            sqlite3VdbeResolveLabel( v, destIfFalse );
            break;
          }
#endif
                    default:
                        {
                            r1 = this.sqlite3ExprCodeTemp(pExpr, ref regFree1);
                            v.sqlite3VdbeAddOp3(OP_If, r1, dest, jumpIfNull != 0 ? 1 : 0);
                            testcase(regFree1 == 0);
                            testcase(jumpIfNull == 0);
                            break;
                        }
                }
                this.sqlite3ReleaseTempReg(regFree1);
                this.sqlite3ReleaseTempReg(regFree2);
            }
            public void sqlite3ExprIfFalse(Expr pExpr, int dest, int jumpIfNull)
            {
                Vdbe v = this.pVdbe;
                int op = 0;
                int regFree1 = 0;
                int regFree2 = 0;
                int r1 = 0, r2 = 0;
                Debug.Assert(jumpIfNull == SQLITE_JUMPIFNULL || jumpIfNull == 0);
                if (NEVER(v == null))
                    return;
                /* Existance of VDBE checked by caller */
                if (pExpr == null)
                    return;
                /* The value of pExpr.op and op are related as follows:
      **
      **       pExpr.op            op
      **       ---------          ----------
      **       TK_ISNULL          OP_NotNull
      **       TK_NOTNULL         OP_IsNull
      **       TK_NE              OP_Eq
      **       TK_EQ              OP_Ne
      **       TK_GT              OP_Le
      **       TK_LE              OP_Gt
      **       TK_GE              OP_Lt
      **       TK_LT              OP_Ge
      **
      ** For other values of pExpr.op, op is undefined and unused.
      ** The value of TK_ and OP_ constants are arranged such that we
      ** can compute the mapping above using the following expression.
      ** Assert()s verify that the computation is correct.
      */
                op = ((pExpr.op + (TK_ISNULL & 1)) ^ 1) - (TK_ISNULL & 1);
                /* Verify correct alignment of TK_ and OP_ constants
      */
                Debug.Assert(pExpr.op != TK_ISNULL || op == OP_NotNull);
                Debug.Assert(pExpr.op != TK_NOTNULL || op == OP_IsNull);
                Debug.Assert(pExpr.op != TK_NE || op == OP_Eq);
                Debug.Assert(pExpr.op != TK_EQ || op == OP_Ne);
                Debug.Assert(pExpr.op != TK_LT || op == OP_Ge);
                Debug.Assert(pExpr.op != TK_LE || op == OP_Gt);
                Debug.Assert(pExpr.op != TK_GT || op == OP_Le);
                Debug.Assert(pExpr.op != TK_GE || op == OP_Lt);
                switch (pExpr.op)
                {
                    case TK_AND:
                        {
                            testcase(jumpIfNull == 0);
                            this.sqlite3ExprIfFalse(pExpr.pLeft, dest, jumpIfNull);
                            this.sqlite3ExprIfFalse(pExpr.pRight, dest, jumpIfNull);
                            break;
                        }
                    case TK_OR:
                        {
                            int d2 = v.sqlite3VdbeMakeLabel();
                            testcase(jumpIfNull == 0);
                            this.sqlite3ExprCachePush();
                            this.sqlite3ExprIfTrue(pExpr.pLeft, d2, jumpIfNull ^ SQLITE_JUMPIFNULL);
                            this.sqlite3ExprIfFalse(pExpr.pRight, dest, jumpIfNull);
                            v.sqlite3VdbeResolveLabel(d2);
                            this.sqlite3ExprCachePop(1);
                            break;
                        }
                    case TK_NOT:
                        {
                            testcase(jumpIfNull == 0);
                            this.sqlite3ExprIfTrue(pExpr.pLeft, dest, jumpIfNull);
                            break;
                        }
                    case TK_LT:
                    case TK_LE:
                    case TK_GT:
                    case TK_GE:
                    case TK_NE:
                    case TK_EQ:
                        {
                            testcase(op == TK_LT);
                            testcase(op == TK_LE);
                            testcase(op == TK_GT);
                            testcase(op == TK_GE);
                            testcase(op == TK_EQ);
                            testcase(op == TK_NE);
                            testcase(jumpIfNull == 0);
                            r1 = this.sqlite3ExprCodeTemp(pExpr.pLeft, ref regFree1);
                            r2 = this.sqlite3ExprCodeTemp(pExpr.pRight, ref regFree2);
                            this.codeCompare(pExpr.pLeft, pExpr.pRight, op, r1, r2, dest, jumpIfNull);
                            testcase(regFree1 == 0);
                            testcase(regFree2 == 0);
                            break;
                        }
                    case TK_IS:
                    case TK_ISNOT:
                        {
                            testcase(pExpr.op == TK_IS);
                            testcase(pExpr.op == TK_ISNOT);
                            r1 = this.sqlite3ExprCodeTemp(pExpr.pLeft, ref regFree1);
                            r2 = this.sqlite3ExprCodeTemp(pExpr.pRight, ref regFree2);
                            op = (pExpr.op == TK_IS) ? TK_NE : TK_EQ;
                            this.codeCompare(pExpr.pLeft, pExpr.pRight, op, r1, r2, dest, SQLITE_NULLEQ);
                            testcase(regFree1 == 0);
                            testcase(regFree2 == 0);
                            break;
                        }
                    case TK_ISNULL:
                    case TK_NOTNULL:
                        {
                            testcase(op == TK_ISNULL);
                            testcase(op == TK_NOTNULL);
                            r1 = this.sqlite3ExprCodeTemp(pExpr.pLeft, ref regFree1);
                            v.sqlite3VdbeAddOp2(op, r1, dest);
                            testcase(regFree1 == 0);
                            break;
                        }
                    case TK_BETWEEN:
                        {
                            testcase(jumpIfNull == 0);
                            this.exprCodeBetween(pExpr, dest, 0, jumpIfNull);
                            break;
                        }
#if SQLITE_OMIT_SUBQUERY
																																																																																					        case TK_IN:
          {
            if ( jumpIfNull != 0 )
            {
              sqlite3ExprCodeIN( pParse, pExpr, dest, dest );
            }
            else
            {
              int destIfNull = sqlite3VdbeMakeLabel( v );
              sqlite3ExprCodeIN( pParse, pExpr, dest, destIfNull );
              sqlite3VdbeResolveLabel( v, destIfNull );
            }
          break;
          }
#endif
                    default:
                        {
                            r1 = this.sqlite3ExprCodeTemp(pExpr, ref regFree1);
                            v.sqlite3VdbeAddOp3(OP_IfNot, r1, dest, jumpIfNull != 0 ? 1 : 0);
                            testcase(regFree1 == 0);
                            testcase(jumpIfNull == 0);
                            break;
                        }
                }
                this.sqlite3ReleaseTempReg(regFree1);
                this.sqlite3ReleaseTempReg(regFree2);
            }
            public int sqlite3GetTempReg()
            {
                if (this.nTempReg == 0)
                {
                    return ++this.nMem;
                }
                return this.aTempReg[--this.nTempReg];
            }
            public void sqlite3ReleaseTempReg(int iReg)
            {
                if (iReg != 0 && this.nTempReg < ArraySize(this.aTempReg))
                {
                    int i;
                    yColCache p;
                    for (i = 0; i < SQLITE_N_COLCACHE; i++)//p=pParse.aColCache... p++)
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
            public int sqlite3GetTempRange(int nReg)
            {
                int i, n;
                i = this.iRangeReg;
                n = this.nRangeReg;
                if (nReg <= n)
                {
                    //Debug.Assert( 1 == usedAsColumnCache( pParse, i, i + n - 1 ) );
                    this.iRangeReg += nReg;
                    this.nRangeReg -= nReg;
                }
                else
                {
                    i = this.nMem + 1;
                    this.nMem += nReg;
                }
                return i;
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
            public ExprList sqlite3ExprListAppend(int null_2, Expr pExpr)
            {
                return this.sqlite3ExprListAppend(null, pExpr);
            }
            public ExprList sqlite3ExprListAppend(/* Parsing context */ExprList pList,/* List to which to append. Might be NULL */Expr pExpr/* Expression to be appended. Might be NULL */)
            {
                sqlite3 db = this.db;
                if (pList == null)
                {
                    pList = new ExprList();
                    //sqlite3DbMallocZero(db, ExprList).Length;
                    //if ( pList == null )
                    //{
                    //  goto no_mem;
                    //}
                    Debug.Assert(pList.nAlloc == 0);
                }
                if (pList.nAlloc <= pList.nExpr)
                {
                    ExprList_item a;
                    int n = pList.nAlloc * 2 + 4;
                    //a = sqlite3DbRealloc(db, pList.a, n*sizeof(pList.a[0]));
                    //if( a==0 ){
                    //  goto no_mem;
                    //}
                    Array.Resize(ref pList.a, n);
                    // = a;
                    pList.nAlloc = pList.a.Length;
                    // sqlite3DbMallocSize(db, a)/sizeof(a[0]);
                }
                Debug.Assert(pList.a != null);
                if (true)
                {
                    pList.a[pList.nExpr] = new ExprList_item();
                    //ExprList_item pItem = pList.a[pList.nExpr++];
                    //pItem = new ExprList_item();//memset(pItem, 0, sizeof(*pItem));
                    //pItem.pExpr = pExpr;
                    pList.a[pList.nExpr++].pExpr = pExpr;
                }
                return pList;
                //no_mem:
                //  /* Avoid leaking memory if malloc has failed. */
                //  sqlite3ExprDelete( db, ref pExpr );
                //  sqlite3ExprListDelete( db, ref pList );
                //  return null;
            }
            public void sqlite3ExprListSetSpan(/* Parsing context */ExprList pList,/* List to which to add the span. */ExprSpan pSpan/* The span to be added */)
            {
                sqlite3 db = this.db;
                Debug.Assert(pList != null/*|| db.mallocFailed != 0 */);
                if (pList != null)
                {
                    ExprList_item pItem = pList.a[pList.nExpr - 1];
                    Debug.Assert(pList.nExpr > 0);
                    Debug.Assert(/* db.mallocFailed != 0 || */pItem.pExpr == pSpan.pExpr);
                    db.sqlite3DbFree(ref pItem.zSpan);
                    pItem.zSpan = pSpan.zStart.Substring(0, pSpan.zStart.Length <= pSpan.zEnd.Length ? pSpan.zStart.Length : pSpan.zStart.Length - pSpan.zEnd.Length);
                    // sqlite3DbStrNDup( db, pSpan.zStart,
                    //(int)( pSpan.zEnd- pSpan.zStart) );
                }
            }
            public void sqlite3ExprListSetName(/* Parsing context */ExprList pList,/* List to which to add the span. */Token pName,/* Name to be added */int dequote/* True to cause the name to be dequoted */)
            {
                Debug.Assert(pList != null/* || pParse.db.mallocFailed != 0 */);
                if (pList != null)
                {
                    ExprList_item pItem;
                    Debug.Assert(pList.nExpr > 0);
                    pItem = pList.a[pList.nExpr - 1];
                    Debug.Assert(pItem.zName == null);
                    pItem.zName = pName.zRestSql.Substring(0, pName.Length);
                    //sqlite3DbStrNDup(pParse.db, pName.z, pName.n);
                    if (dequote != 0 && !String.IsNullOrEmpty(pItem.zName))
                        StringExtensions.sqlite3Dequote(ref pItem.zName);
                }
            }
            public void sqlite3ExprListCheckLength(ExprList pEList, string zObject)
            {
                int mx = this.db.aLimit[SQLITE_LIMIT_COLUMN];
                testcase(pEList != null && pEList.nExpr == mx);
                testcase(pEList != null && pEList.nExpr == mx + 1);
                if (pEList != null && pEList.nExpr > mx)
                {
                    sqlite3ErrorMsg(this, "too many columns in %s", zObject);
                }
            }
            public Expr sqlite3ExprFunction(int null_2, Token pToken)
            {
                return this.sqlite3ExprFunction(null, pToken);
            }
            public Expr sqlite3ExprFunction(ExprList pList, int null_3)
            {
                return this.sqlite3ExprFunction(pList, null);
            }
            public Expr sqlite3ExprFunction(ExprList pList, Token pToken)
            {
                Expr pNew;
                sqlite3 db = this.db;
                Debug.Assert(pToken != null);
                pNew = sqlite3ExprAlloc(db, TK_FUNCTION, pToken, 1);
                if (pNew == null)
                {
                    sqlite3ExprListDelete(db, ref pList);
                    /* Avoid memory leak when malloc fails */
                    return null;
                }
                pNew.x.pList = pList;
                Debug.Assert(!ExprHasProperty(pNew, EP_xIsSelect));
                this.sqlite3ExprSetHeight(pNew);
                return pNew;
            }
            public void sqlite3ExprAssignVarNumber(Expr pExpr)
            {
                sqlite3 db = this.db;
                string z;
                if (pExpr == null)
                    return;
                Debug.Assert(!ExprHasAnyProperty(pExpr, EP_IntValue | EP_Reduced | EP_TokenOnly));
                z = pExpr.u.zToken;
                Debug.Assert(z != null);
                Debug.Assert(z.Length != 0);
                if (z.Length == 1)
                {
                    /* Wildcard of the form "?".  Assign the next variable number */
                    Debug.Assert(z[0] == '?');
                    pExpr.iColumn = (ynVar)(++this.nVar);
                }
                else
                {
                    ynVar x = 0;
                    int n = StringExtensions.sqlite3Strlen30(z);
                    if (z[0] == '?')
                    {
                        /* Wildcard of the form "?nnn".  Convert "nnn" to an integer and
        ** use it as the variable number */
                        i64 i = 0;
                        bool bOk = 0 == Converter.sqlite3Atoi64(z.Substring(1), ref i, n - 1, SqliteEncoding.UTF8);
                        pExpr.iColumn = x = (ynVar)i;
                        testcase(i == 0);
                        testcase(i == 1);
                        testcase(i == db.aLimit[SQLITE_LIMIT_VARIABLE_NUMBER] - 1);
                        testcase(i == db.aLimit[SQLITE_LIMIT_VARIABLE_NUMBER]);
                        if (bOk == false || i < 1 || i > db.aLimit[SQLITE_LIMIT_VARIABLE_NUMBER])
                        {
                            sqlite3ErrorMsg(this, "variable number must be between ?1 and ?%d", db.aLimit[SQLITE_LIMIT_VARIABLE_NUMBER]);
                            x = 0;
                        }
                        if (i > this.nVar)
                        {
                            this.nVar = (int)i;
                        }
                    }
                    else
                    {
                        /* Wildcards like ":aaa", "$aaa" or "@aaa".  Reuse the same variable
        ** number as the prior appearance of the same name, or if the name
        ** has never appeared before, reuse the same variable number
        */
                        ynVar i;
                        for (i = 0; i < this.nzVar; i++)
                        {
                            if (this.azVar[i] != null && z.CompareTo(this.azVar[i]) == 0)//memcmp(pParse.azVar[i],z,n+1)==0 )
                            {
                                pExpr.iColumn = x = (ynVar)(i + 1);
                                break;
                            }
                        }
                        if (x == 0)
                            x = pExpr.iColumn = (ynVar)(++this.nVar);
                    }
                    if (x > 0)
                    {
                        if (x > this.nzVar)
                        {
                            //char **a;
                            //a = sqlite3DbRealloc(db, pParse.azVar, x*sizeof(a[0]));
                            //if( a==0 ) return;  /* Error reported through db.mallocFailed */
                            //pParse.azVar = a;
                            //memset(&a[pParse.nzVar], 0, (x-pParse.nzVar)*sizeof(a[0]));
                            Array.Resize(ref this.azVar, x);
                            this.nzVar = x;
                        }
                        if (z[0] != '?' || this.azVar[x - 1] == null)
                        {
                            //sqlite3DbFree(db, pParse.azVar[x-1]);
                            this.azVar[x - 1] = z.Substring(0, n);
                            //sqlite3DbStrNDup( db, z, n );
                        }
                    }
                }
                if (this.nErr == 0 && this.nVar > db.aLimit[SQLITE_LIMIT_VARIABLE_NUMBER])
                {
                    sqlite3ErrorMsg(this, "too many SQL variables");
                }
            }
            public void exprCommute(Expr pExpr)
            {
                u16 expRight = (u16)(pExpr.pRight.flags & EP_ExpCollate);
                u16 expLeft = (u16)(pExpr.pLeft.flags & EP_ExpCollate);
                Debug.Assert(allowedOp(pExpr.op) && pExpr.op != TK_IN);
                pExpr.pRight.pColl = this.sqlite3ExprCollSeq(pExpr.pRight);
                pExpr.pLeft.pColl = this.sqlite3ExprCollSeq(pExpr.pLeft);
                SWAP(ref pExpr.pRight.pColl, ref pExpr.pLeft.pColl);
                pExpr.pRight.flags = (u16)((pExpr.pRight.flags & ~EP_ExpCollate) | expLeft);
                pExpr.pLeft.flags = (u16)((pExpr.pLeft.flags & ~EP_ExpCollate) | expRight);
                SWAP(ref pExpr.pRight, ref pExpr.pLeft);
                if (pExpr.op >= TK_GT)
                {
                    Debug.Assert(TK_LT == TK_GT + 2);
                    Debug.Assert(TK_GE == TK_LE + 2);
                    Debug.Assert(TK_GT > TK_EQ);
                    Debug.Assert(TK_GT < TK_LE);
                    Debug.Assert(pExpr.op >= TK_GT && pExpr.op <= TK_GE);
                    pExpr.op = (u8)(((pExpr.op - TK_GT) ^ 2) + TK_GT);
                }
            }
            public int isLikeOrGlob(/* Parsing and code generating context */Expr pExpr,/* Test this expression */ref Expr ppPrefix,/* Pointer to TK_STRING expression with pattern prefix */ref bool pisComplete,/* True if the only wildcard is % in the last character */ref bool pnoCase/* True if uppercase is equivalent to lowercase */)
            {
                string z = null;
                /* String on RHS of LIKE operator */
                Expr pRight, pLeft;
                /* Right and left size of LIKE operator */
                ExprList pList;
                /* List of operands to the LIKE operator */
                int c = 0;
                /* One character in z[] */
                int cnt;
                /* Number of non-wildcard prefix characters */
                char[] wc = new char[3];
                /* Wildcard characters */
                sqlite3 db = this.db;
                /* Data_base connection */
                sqlite3_value pVal = null;
                int op;
                /* Opcode of pRight */
                if (!sqlite3IsLikeFunction(db, pExpr, ref pnoCase, wc))
                {
                    return 0;
                }
                //#if SQLITE_EBCDIC
                //if( pnoCase ) return 0;
                //#endif
                pList = pExpr.x.pList;
                pLeft = pList.a[1].pExpr;
                if (pLeft.op != TK_COLUMN || pLeft.sqlite3ExprAffinity() != SQLITE_AFF_TEXT)
                {
                    /* IMP: R-02065-49465 The left-hand side of the LIKE or GLOB operator must
        ** be the name of an indexed column with TEXT affinity. */
                    return 0;
                }
                Debug.Assert(pLeft.iColumn != (-1));
                /* Because IPK never has AFF_TEXT */
                pRight = pList.a[0].pExpr;
                op = pRight.op;
                if (op == TK_REGISTER)
                {
                    op = pRight.op2;
                }
                if (op == TK_VARIABLE)
                {
                    Vdbe pReprepare = this.pReprepare;
                    int iCol = pRight.iColumn;
                    pVal = sqlite3VdbeGetValue(pReprepare, iCol, (byte)SQLITE_AFF_NONE);
                    if (pVal != null && sqlite3_value_type(pVal) == SQLITE_TEXT)
                    {
                        z = sqlite3_value_text(pVal);
                    }
                    sqlite3VdbeSetVarmask(this.pVdbe, iCol);
                    /* IMP: R-23257-02778 */
                    Debug.Assert(pRight.op == TK_VARIABLE || pRight.op == TK_REGISTER);
                }
                else
                    if (op == TK_STRING)
                    {
                        z = pRight.u.zToken;
                    }
                if (!String.IsNullOrEmpty(z))
                {
                    cnt = 0;
                    while (cnt < z.Length && (c = z[cnt]) != 0 && c != wc[0] && c != wc[1] && c != wc[2])
                    {
                        cnt++;
                    }
                    if (cnt != 0 && 255 != (u8)z[cnt - 1])
                    {
                        Expr pPrefix;
                        pisComplete = c == wc[0] && cnt == z.Length - 1;
                        pPrefix = sqlite3Expr(db, TK_STRING, z);
                        if (pPrefix != null)
                            pPrefix.u.zToken = pPrefix.u.zToken.Substring(0, cnt);
                        ppPrefix = pPrefix;
                        if (op == TK_VARIABLE)
                        {
                            Vdbe v = this.pVdbe;
                            sqlite3VdbeSetVarmask(v, pRight.iColumn);
                            /* IMP: R-23257-02778 */
                            if (pisComplete && pRight.u.zToken.Length > 1)
                            {
                                /* If the rhs of the LIKE expression is a variable, and the current
              ** value of the variable means there is no need to invoke the LIKE
              ** function, then no OP_Variable will be added to the program.
              ** This causes problems for the sqlite3_bind_parameter_name()
              ** API. To workaround them, add a dummy OP_Variable here.
              */
                                int r1 = this.sqlite3GetTempReg();
                                this.sqlite3ExprCodeTarget(pRight, r1);
                                v.sqlite3VdbeChangeP3(v.sqlite3VdbeCurrentAddr() - 1, 0);
                                this.sqlite3ReleaseTempReg(r1);
                            }
                        }
                    }
                    else
                    {
                        z = null;
                    }
                }
                sqlite3ValueFree(ref pVal);
                return (z != null) ? 1 : 0;
            }
            public void bestOrClauseIndex(/* The parsing context */WhereClause pWC,/* The WHERE clause */SrcList_item pSrc,/* The FROM clause term to search */Bitmask notReady,/* Mask of cursors not available for indexing */Bitmask notValid,/* Cursors not available for any purpose */ExprList pOrderBy,/* The ORDER BY clause */WhereCost pCost/* Lowest cost query plan */)
            {
#if !SQLITE_OMIT_OR_OPTIMIZATION
                int iCur = pSrc.iCursor;
                /* The cursor of the table to be accessed */
                Bitmask maskSrc = pWC.pMaskSet.getMask(iCur);
                /* Bitmask for pSrc */
                WhereTerm pWCEnd = pWC.a[pWC.nTerm];
                /* End of pWC.a[] */
                WhereTerm pTerm;
                /* A single term of the WHERE clause */
                /* No OR-clause optimization allowed if the INDEXED BY or NOT INDEXED clauses
** are used */
                if (pSrc.notIndexed != 0 || pSrc.pIndex != null)
                {
                    return;
                }
                /* Search the WHERE clause terms for a usable WO_OR term. */
                for (int _pt = 0; _pt < pWC.nTerm; _pt++)//<pWCEnd; pTerm++)
                {
                    pTerm = pWC.a[_pt];
                    if (pTerm.eOperator == WO_OR && ((pTerm.prereqAll & ~maskSrc) & notReady) == 0 && (pTerm.u.pOrInfo.indexable & maskSrc) != 0)
                    {
                        WhereClause pOrWC = pTerm.u.pOrInfo.wc;
                        WhereTerm pOrWCEnd = pOrWC.a[pOrWC.nTerm];
                        WhereTerm pOrTerm;
                        int flags = WHERE_MULTI_OR;
                        double rTotal = 0;
                        double nRow = 0;
                        Bitmask used = 0;
                        for (int _pOrWC = 0; _pOrWC < pOrWC.nTerm; _pOrWC++)//pOrTerm = pOrWC.a ; pOrTerm < pOrWCEnd ; pOrTerm++ )
                        {
                            pOrTerm = pOrWC.a[_pOrWC];
                            WhereCost sTermCost = null;
#if (SQLITE_TEST) && (SQLITE_DEBUG)
																																																																																																																																																																		            WHERETRACE( "... Multi-index OR testing for term %d of %d....\n",
            _pOrWC, pOrWC.nTerm - _pOrWC//( pOrTerm - pOrWC.a ), ( pTerm - pWC.a )
            );
#endif
                            if (pOrTerm.eOperator == WO_AND)
                            {
                                WhereClause pAndWC = pOrTerm.u.pAndInfo.wc;
                                this.bestIndex(pAndWC, pSrc, notReady, notValid, null, ref sTermCost);
                            }
                            else
                                if (pOrTerm.leftCursor == iCur)
                                {
                                    WhereClause tempWC = new WhereClause();
                                    tempWC.pParse = pWC.pParse;
                                    tempWC.pMaskSet = pWC.pMaskSet;
                                    tempWC.op = TK_AND;
                                    tempWC.a = new WhereTerm[2];
                                    tempWC.a[0] = pOrTerm;
                                    tempWC.nTerm = 1;
                                    this.bestIndex(tempWC, pSrc, notReady, notValid, null, ref sTermCost);
                                }
                                else
                                {
                                    continue;
                                }
                            rTotal += sTermCost.rCost;
                            nRow += sTermCost.plan.nRow;
                            used |= sTermCost.used;
                            if (rTotal >= pCost.rCost)
                                break;
                        }
                        /* If there is an ORDER BY clause, increase the scan cost to account
          ** for the cost of the sort. */
                        if (pOrderBy != null)
                        {
#if (SQLITE_TEST) && (SQLITE_DEBUG)
																																																																																																																																																																		            WHERETRACE( "... sorting increases OR cost %.9g to %.9g\n",
            rTotal, rTotal + nRow * estLog( nRow ) );
#endif
                            rTotal += nRow * estLog(nRow);
                        }
                        /* If the cost of scanning using this OR term for optimization is
          ** less than the current cost stored in pCost, replace the contents
          ** of pCost. */
#if (SQLITE_TEST) && (SQLITE_DEBUG)
																																																																																																																																								          WHERETRACE( "... multi-index OR cost=%.9g nrow=%.9g\n", rTotal, nRow );
#endif
                        if (rTotal < pCost.rCost)
                        {
                            pCost.rCost = rTotal;
                            pCost.used = used;
                            pCost.plan.nRow = nRow;
                            pCost.plan.wsFlags = (uint)flags;
                            pCost.plan.u.pTerm = pTerm;
                        }
                    }
                }
#endif
            }
            public void bestAutomaticIndex(/* The parsing context */WhereClause pWC,/* The WHERE clause */SrcList_item pSrc,/* The FROM clause term to search */Bitmask notReady,/* Mask of cursors that are not available */WhereCost pCost/* Lowest cost query plan */)
            {
                double nTableRow;
                /* Rows in the input table */
                double logN;
                /* log(nTableRow) */
                double costTempIdx;
                /* per-query cost of the transient index */
                WhereTerm pTerm;
                /* A single term of the WHERE clause */
                WhereTerm pWCEnd;
                /* End of pWC.a[] */
                Table pTable;
                /* Table that might be indexed */
                if (this.nQueryLoop <= (double)1)
                {
                    /* There is no point in building an automatic index for a single scan */
                    return;
                }
                if ((this.db.flags & SQLITE_AutoIndex) == 0)
                {
                    /* Automatic indices are disabled at run-time */
                    return;
                }
                if ((pCost.plan.wsFlags & WHERE_NOT_FULLSCAN) != 0)
                {
                    /* We already have some kind of index in use for this query. */
                    return;
                }
                if (pSrc.notIndexed != 0)
                {
                    /* The NOT INDEXED clause appears in the SQL. */
                    return;
                }
                Debug.Assert(this.nQueryLoop >= (double)1);
                pTable = pSrc.pTab;
                nTableRow = pTable.nRowEst;
                logN = estLog(nTableRow);
                costTempIdx = 2 * logN * (nTableRow / this.nQueryLoop + 1);
                if (costTempIdx >= pCost.rCost)
                {
                    /* The cost of creating the transient table would be greater than
        ** doing the full table scan */
                    return;
                }
                /* Search for any equality comparison term */
                //pWCEnd = pWC.a[pWC.nTerm];
                for (int ipTerm = 0; ipTerm < pWC.nTerm; ipTerm++)//; pTerm<pWCEnd; pTerm++)
                {
                    pTerm = pWC.a[ipTerm];
                    if (pTerm.termCanDriveIndex(pSrc, notReady) != 0)
                    {
#if (SQLITE_TEST) && (SQLITE_DEBUG)
																																																																																																																																								          WHERETRACE( "auto-index reduces cost from %.2f to %.2f\n",
          pCost.rCost, costTempIdx );
#endif
                        pCost.rCost = costTempIdx;
                        pCost.plan.nRow = logN + 1;
                        pCost.plan.wsFlags = WHERE_TEMP_INDEX;
                        pCost.used = pTerm.prereqRight;
                        break;
                    }
                }
            }
            public void constructAutomaticIndex(/* The parsing context */WhereClause pWC,/* The WHERE clause */SrcList_item pSrc,/* The FROM clause term to get the next index */Bitmask notReady,/* Mask of cursors that are not available */WhereLevel pLevel/* Write new index here */)
            {
                int nColumn;
                /* Number of columns in the constructed index */
                WhereTerm pTerm;
                /* A single term of the WHERE clause */
                WhereTerm pWCEnd;
                /* End of pWC.a[] */
                int nByte;
                /* Byte of memory needed for pIdx */
                Index pIdx;
                /* Object describing the transient index */
                Vdbe v;
                /* Prepared statement under construction */
                int regIsInit;
                /* Register set by initialization */
                int addrInit;
                /* Address of the initialization bypass jump */
                Table pTable;
                /* The table being indexed */
                KeyInfo pKeyinfo;
                /* Key information for the index */
                int addrTop;
                /* Top of the index fill loop */
                int regRecord;
                /* Register holding an index record */
                int n;
                /* Column counter */
                int i;
                /* Loop counter */
                int mxBitCol;
                /* Maximum column in pSrc.colUsed */
                CollSeq pColl;
                /* Collating sequence to on a column */
                Bitmask idxCols;
                /* Bitmap of columns used for indexing */
                Bitmask extraCols;
                /* Bitmap of additional columns */
                /* Generate code to skip over the creation and initialization of the
** transient index on 2nd and subsequent iterations of the loop. */
                v = this.pVdbe;
                Debug.Assert(v != null);
                regIsInit = ++this.nMem;
                addrInit = v.sqlite3VdbeAddOp1(OP_If, regIsInit);
                v.sqlite3VdbeAddOp2(OP_Integer, 1, regIsInit);
                /* Count the number of columns that will be added to the index
      ** and used to match WHERE clause constraints */
                nColumn = 0;
                pTable = pSrc.pTab;
                //pWCEnd = pWC.a[pWC.nTerm];
                idxCols = 0;
                for (int ipTerm = 0; ipTerm < pWC.nTerm; ipTerm++)//; pTerm<pWCEnd; pTerm++)
                {
                    pTerm = pWC.a[ipTerm];
                    if (pTerm.termCanDriveIndex(pSrc, notReady) != 0)
                    {
                        int iCol = pTerm.u.leftColumn;
                        Bitmask cMask = iCol >= BMS ? ((Bitmask)1) << (BMS - 1) : ((Bitmask)1) << iCol;
                        testcase(iCol == BMS);
                        testcase(iCol == BMS - 1);
                        if ((idxCols & cMask) == 0)
                        {
                            nColumn++;
                            idxCols |= cMask;
                        }
                    }
                }
                Debug.Assert(nColumn > 0);
                pLevel.plan.nEq = (u32)nColumn;
                /* Count the number of additional columns needed to create a
      ** covering index.  A "covering index" is an index that contains all
      ** columns that are needed by the query.  With a covering index, the
      ** original table never needs to be accessed.  Automatic indices must
      ** be a covering index because the index will not be updated if the
      ** original table changes and the index and table cannot both be used
      ** if they go out of sync.
      */
                extraCols = pSrc.colUsed & (~idxCols | (((Bitmask)1) << (BMS - 1)));
                mxBitCol = (pTable.nCol >= BMS - 1) ? BMS - 1 : pTable.nCol;
                testcase(pTable.nCol == BMS - 1);
                testcase(pTable.nCol == BMS - 2);
                for (i = 0; i < mxBitCol; i++)
                {
                    if ((extraCols & (((Bitmask)1) << i)) != 0)
                        nColumn++;
                }
                if ((pSrc.colUsed & (((Bitmask)1) << (BMS - 1))) != 0)
                {
                    nColumn += pTable.nCol - BMS + 1;
                }
                pLevel.plan.wsFlags |= WHERE_COLUMN_EQ | WHERE_IDX_ONLY | WO_EQ;
                /* Construct the Index object to describe this index */
                //nByte = sizeof(Index);
                //nByte += nColumn*sizeof(int);     /* Index.aiColumn */
                //nByte += nColumn*sizeof(char);   /* Index.azColl */
                //nByte += nColumn;                 /* Index.aSortOrder */
                //pIdx = sqlite3DbMallocZero(pParse.db, nByte);
                //if( pIdx==null) return;
                pIdx = new Index();
                pLevel.plan.u.pIdx = pIdx;
                pIdx.azColl = new string[nColumn + 1];
                // pIdx[1];
                pIdx.aiColumn = new int[nColumn + 1];
                // pIdx.azColl[nColumn];
                pIdx.aSortOrder = new u8[nColumn + 1];
                // pIdx.aiColumn[nColumn];
                pIdx.zName = "auto-index";
                pIdx.nColumn = nColumn;
                pIdx.pTable = pTable;
                n = 0;
                idxCols = 0;
                //for(pTerm=pWC.a; pTerm<pWCEnd; pTerm++){
                for (int ipTerm = 0; ipTerm < pWC.nTerm; ipTerm++)
                {
                    pTerm = pWC.a[ipTerm];
                    if (pTerm.termCanDriveIndex(pSrc, notReady) != 0)
                    {
                        int iCol = pTerm.u.leftColumn;
                        Bitmask cMask = iCol >= BMS ? ((Bitmask)1) << (BMS - 1) : ((Bitmask)1) << iCol;
                        if ((idxCols & cMask) == 0)
                        {
                            Expr pX = pTerm.pExpr;
                            idxCols |= cMask;
                            pIdx.aiColumn[n] = pTerm.u.leftColumn;
                            pColl = this.sqlite3BinaryCompareCollSeq(pX.pLeft, pX.pRight);
                            pIdx.azColl[n] = ALWAYS(pColl != null) ? pColl.zName : "BINARY";
                            n++;
                        }
                    }
                }
                Debug.Assert((u32)n == pLevel.plan.nEq);
                /* Add additional columns needed to make the automatic index into
      ** a covering index */
                for (i = 0; i < mxBitCol; i++)
                {
                    if ((extraCols & (((Bitmask)1) << i)) != 0)
                    {
                        pIdx.aiColumn[n] = i;
                        pIdx.azColl[n] = "BINARY";
                        n++;
                    }
                }
                if ((pSrc.colUsed & (((Bitmask)1) << (BMS - 1))) != 0)
                {
                    for (i = BMS - 1; i < pTable.nCol; i++)
                    {
                        pIdx.aiColumn[n] = i;
                        pIdx.azColl[n] = "BINARY";
                        n++;
                    }
                }
                Debug.Assert(n == nColumn);
                /* Create the automatic index */
                pKeyinfo = sqlite3IndexKeyinfo(this, pIdx);
                Debug.Assert(pLevel.iIdxCur >= 0);
                v.sqlite3VdbeAddOp4(OP_OpenAutoindex, pLevel.iIdxCur, nColumn + 1, 0, pKeyinfo, P4_KEYINFO_HANDOFF);
                VdbeComment(v, "for %s", pTable.zName);
                /* Fill the automatic index with content */
                addrTop = v.sqlite3VdbeAddOp1(OP_Rewind, pLevel.iTabCur);
                regRecord = this.sqlite3GetTempReg();
                this.sqlite3GenerateIndexKey(pIdx, pLevel.iTabCur, regRecord, true);
                v.sqlite3VdbeAddOp2(OP_IdxInsert, pLevel.iIdxCur, regRecord);
                v.sqlite3VdbeChangeP5(OPFLAG_USESEEKRESULT);
                v.sqlite3VdbeAddOp2(OP_Next, pLevel.iTabCur, addrTop + 1);
                v.sqlite3VdbeChangeP5(SQLITE_STMTSTATUS_AUTOINDEX);
                v.sqlite3VdbeJumpHere(addrTop);
                this.sqlite3ReleaseTempReg(regRecord);
                /* Jump here when skipping the initialization */
                v.sqlite3VdbeJumpHere(addrInit);
            }
            public sqlite3_index_info allocateIndexInfo(WhereClause pWC, SrcList_item pSrc, ExprList pOrderBy)
            {
                int i, j;
                int nTerm;
                sqlite3_index_constraint[] pIdxCons;
                sqlite3_index_orderby[] pIdxOrderBy;
                sqlite3_index_constraint_usage[] pUsage;
                WhereTerm pTerm;
                int nOrderBy;
                sqlite3_index_info pIdxInfo;
#if (SQLITE_TEST) && (SQLITE_DEBUG)
																																																																																				      WHERETRACE( "Recomputing index info for %s...\n", pSrc.pTab.zName );
#endif
                /* Count the number of possible WHERE clause constraints referring
** to this virtual table */
                for (i = nTerm = 0; i < pWC.nTerm; i++)//, pTerm++ )
                {
                    pTerm = pWC.a[i];
                    if (pTerm.leftCursor != pSrc.iCursor)
                        continue;
                    Debug.Assert((pTerm.eOperator & (pTerm.eOperator - 1)) == 0);
                    testcase(pTerm.eOperator == WO_IN);
                    testcase(pTerm.eOperator == WO_ISNULL);
                    if ((pTerm.eOperator & (WO_IN | WO_ISNULL)) != 0)
                        continue;
                    nTerm++;
                }
                /* If the ORDER BY clause contains only columns in the current
      ** virtual table then allocate space for the aOrderBy part of
      ** the sqlite3_index_info structure.
      */
                nOrderBy = 0;
                if (pOrderBy != null)
                {
                    for (i = 0; i < pOrderBy.nExpr; i++)
                    {
                        Expr pExpr = pOrderBy.a[i].pExpr;
                        if (pExpr.op != TK_COLUMN || pExpr.iTable != pSrc.iCursor)
                            break;
                    }
                    if (i == pOrderBy.nExpr)
                    {
                        nOrderBy = pOrderBy.nExpr;
                    }
                }
                /* Allocate the sqlite3_index_info structure
      */
                pIdxInfo = new sqlite3_index_info();
                //sqlite3DbMallocZero(pParse.db, sizeof(*pIdxInfo)
                //+ (sizeof(*pIdxCons) + sizeof(*pUsage))*nTerm
                //+ sizeof(*pIdxOrderBy)*nOrderBy );
                //if ( pIdxInfo == null )
                //{
                //  sqlite3ErrorMsg( pParse, "out of memory" );
                //  /* (double)0 In case of SQLITE_OMIT_FLOATING_POINT... */
                //  return null;
                //}
                /* Initialize the structure.  The sqlite3_index_info structure contains
      ** many fields that are declared "const" to prevent xBestIndex from
      ** changing them.  We have to do some funky casting in order to
      ** initialize those fields.
      */
                pIdxCons = new sqlite3_index_constraint[nTerm];
                //(sqlite3_index_constraint)pIdxInfo[1];
                pIdxOrderBy = new sqlite3_index_orderby[nOrderBy];
                //(sqlite3_index_orderby)pIdxCons[nTerm];
                pUsage = new sqlite3_index_constraint_usage[nTerm];
                //(sqlite3_index_constraint_usage)pIdxOrderBy[nOrderBy];
                pIdxInfo.nConstraint = nTerm;
                pIdxInfo.nOrderBy = nOrderBy;
                pIdxInfo.aConstraint = pIdxCons;
                pIdxInfo.aOrderBy = pIdxOrderBy;
                pIdxInfo.aConstraintUsage = pUsage;
                for (i = j = 0; i < pWC.nTerm; i++)//, pTerm++ )
                {
                    pTerm = pWC.a[i];
                    if (pTerm.leftCursor != pSrc.iCursor)
                        continue;
                    Debug.Assert((pTerm.eOperator & (pTerm.eOperator - 1)) == 0);
                    testcase(pTerm.eOperator == WO_IN);
                    testcase(pTerm.eOperator == WO_ISNULL);
                    if ((pTerm.eOperator & (WO_IN | WO_ISNULL)) != 0)
                        continue;
                    if (pIdxCons[j] == null)
                        pIdxCons[j] = new sqlite3_index_constraint();
                    pIdxCons[j].iColumn = pTerm.u.leftColumn;
                    pIdxCons[j].iTermOffset = i;
                    pIdxCons[j].op = (u8)pTerm.eOperator;
                    /* The direct Debug.Assignment in the previous line is possible only because
        ** the WO_ and SQLITE_INDEX_CONSTRAINT_ codes are identical.  The
        ** following Debug.Asserts verify this fact. */
                    Debug.Assert(WO_EQ == SQLITE_INDEX_CONSTRAINT_EQ);
                    Debug.Assert(WO_LT == SQLITE_INDEX_CONSTRAINT_LT);
                    Debug.Assert(WO_LE == SQLITE_INDEX_CONSTRAINT_LE);
                    Debug.Assert(WO_GT == SQLITE_INDEX_CONSTRAINT_GT);
                    Debug.Assert(WO_GE == SQLITE_INDEX_CONSTRAINT_GE);
                    Debug.Assert(WO_MATCH == SQLITE_INDEX_CONSTRAINT_MATCH);
                    Debug.Assert((pTerm.eOperator & (WO_EQ | WO_LT | WO_LE | WO_GT | WO_GE | WO_MATCH)) != 0);
                    j++;
                }
                for (i = 0; i < nOrderBy; i++)
                {
                    Expr pExpr = pOrderBy.a[i].pExpr;
                    if (pIdxOrderBy[i] == null)
                        pIdxOrderBy[i] = new sqlite3_index_orderby();
                    pIdxOrderBy[i].iColumn = pExpr.iColumn;
                    pIdxOrderBy[i].desc = pOrderBy.a[i].sortOrder != 0;
                }
                return pIdxInfo;
            }
            public int vtabBestIndex(Table pTab, sqlite3_index_info p)
            {
                sqlite3_vtab pVtab = sqlite3GetVTable(this.db, pTab).pVtab;
                int i;
                int rc;
#if (SQLITE_TEST) && (SQLITE_DEBUG)
																																																																																				      WHERETRACE( "xBestIndex for %s\n", pTab.zName );
#endif
                TRACE_IDX_INPUTS(p);
                rc = pVtab.pModule.xBestIndex(pVtab, ref p);
                TRACE_IDX_OUTPUTS(p);
                if (rc != SQLITE_OK)
                {
                    //if ( rc == SQLITE_NOMEM )
                    //{
                    //  pParse.db.mallocFailed = 1;
                    //}
                    // else 
                    if (String.IsNullOrEmpty(pVtab.zErrMsg))
                    {
                        sqlite3ErrorMsg(this, "%s", sqlite3ErrStr(rc));
                    }
                    else
                    {
                        sqlite3ErrorMsg(this, "%s", pVtab.zErrMsg);
                    }
                }
                //sqlite3_free( pVtab.zErrMsg );
                pVtab.zErrMsg = null;
                for (i = 0; i < p.nConstraint; i++)
                {
                    if (!p.aConstraint[i].usable && p.aConstraintUsage[i].argvIndex > 0)
                    {
                        sqlite3ErrorMsg(this, "table %s: xBestIndex returned an invalid plan", pTab.zName);
                    }
                }
                return this.nErr;
            }
            public int whereRangeScanEst(/* Parsing & code generating context */Index p,/* The index containing the range-compared column; "x" */int nEq,/* index into p.aCol[] of the range-compared column */WhereTerm pLower,/* Lower bound on the range. ex: "x>123" Might be NULL */WhereTerm pUpper,/* Upper bound on the range. ex: "x<455" Might be NULL */out int piEst/* OUT: Return value */)
            {
                int rc = SQLITE_OK;
#if SQLITE_ENABLE_STAT2
																																																																																				
      if ( nEq == 0 && p.aSample != null )
      {
        sqlite3_value pLowerVal = null;
        sqlite3_value pUpperVal = null;
        int iEst;
        int iLower = 0;
        int iUpper = SQLITE_INDEX_SAMPLES;
        int roundUpUpper = 0;
        int roundUpLower = 0;
        char aff = p.pTable.aCol[p.aiColumn[0]].affinity;

        if ( pLower != null )
        {
          Expr pExpr = pLower.pExpr.pRight;
          rc = valueFromExpr( pParse, pExpr, aff, ref pLowerVal );
          Debug.Assert( pLower.eOperator == WO_GT || pLower.eOperator == WO_GE );
          roundUpLower = ( pLower.eOperator == WO_GT ) ? 1 : 0;
        }
        if ( rc == SQLITE_OK && pUpper != null )
        {
          Expr pExpr = pUpper.pExpr.pRight;
          rc = valueFromExpr( pParse, pExpr, aff, ref pUpperVal );
          Debug.Assert( pUpper.eOperator == WO_LT || pUpper.eOperator == WO_LE );
          roundUpUpper = ( pUpper.eOperator == WO_LE ) ? 1 : 0;
        }

        if ( rc != SQLITE_OK || ( pLowerVal == null && pUpperVal == null ) )
        {
          sqlite3ValueFree( ref pLowerVal );
          sqlite3ValueFree( ref pUpperVal );
          goto range_est_fallback;
        }
        else if ( pLowerVal == null )
        {
          rc = whereRangeRegion( pParse, p, pUpperVal, roundUpUpper, out iUpper );
          if ( pLower != null )
            iLower = iUpper / 2;
        }
        else if ( pUpperVal == null )
        {
          rc = whereRangeRegion( pParse, p, pLowerVal, roundUpLower, out iLower );
          if ( pUpper != null )
            iUpper = ( iLower + SQLITE_INDEX_SAMPLES + 1 ) / 2;
        }
        else
        {
          rc = whereRangeRegion( pParse, p, pUpperVal, roundUpUpper, out iUpper );
          if ( rc == SQLITE_OK )
          {
            rc = whereRangeRegion( pParse, p, pLowerVal, roundUpLower, out iLower );
          }
        }
        WHERETRACE( "range scan regions: %d..%d\n", iLower, iUpper );

        iEst = iUpper - iLower;
        testcase( iEst == SQLITE_INDEX_SAMPLES );
        Debug.Assert( iEst <= SQLITE_INDEX_SAMPLES );
        if ( iEst < 1 )
        {
          piEst = 50 / SQLITE_INDEX_SAMPLES;
        }
        else
        {
          piEst = ( iEst * 100 ) / SQLITE_INDEX_SAMPLES;
        }

        sqlite3ValueFree( ref pLowerVal );
        sqlite3ValueFree( ref pUpperVal );
        return rc;
      }
range_est_fallback:
#else
                UNUSED_PARAMETER(this);
                UNUSED_PARAMETER(p);
                UNUSED_PARAMETER(nEq);
#endif
                Debug.Assert(pLower != null || pUpper != null);
                piEst = 100;
                if (pLower != null && (pLower.wtFlags & TERM_VNULL) == 0)
                    piEst /= 4;
                if (pUpper != null)
                    piEst /= 4;
                return rc;
            }
            public void bestVirtualIndex(/* The parsing context */WhereClause pWC,/* The WHERE clause */SrcList_item pSrc,/* The FROM clause term to search */Bitmask notReady,/* Mask of cursors not available for index */Bitmask notValid,/* Cursors not valid for any purpose */ExprList pOrderBy,/* The order by clause */ref WhereCost pCost,/* Lowest cost query plan */ref sqlite3_index_info ppIdxInfo/* Index information passed to xBestIndex */)
            {
                Table pTab = pSrc.pTab;
                sqlite3_index_info pIdxInfo;
                sqlite3_index_constraint pIdxCons;
                sqlite3_index_constraint_usage[] pUsage = null;
                WhereTerm pTerm;
                int i, j;
                int nOrderBy;
                double rCost;
                /* Make sure wsFlags is initialized to some sane value. Otherwise, if the
      ** malloc in allocateIndexInfo() fails and this function returns leaving
      ** wsFlags in an uninitialized state, the caller may behave unpredictably.
      */
                pCost = new WhereCost();
                //memset(pCost, 0, sizeof(*pCost));
                pCost.plan.wsFlags = WHERE_VIRTUALTABLE;
                /* If the sqlite3_index_info structure has not been previously
      ** allocated and initialized, then allocate and initialize it now.
      */
                pIdxInfo = ppIdxInfo;
                if (pIdxInfo == null)
                {
                    ppIdxInfo = pIdxInfo = this.allocateIndexInfo(pWC, pSrc, pOrderBy);
                }
                if (pIdxInfo == null)
                {
                    return;
                }
                /* At this point, the sqlite3_index_info structure that pIdxInfo points
      ** to will have been initialized, either during the current invocation or
      ** during some prior invocation.  Now we just have to customize the
      ** details of pIdxInfo for the current invocation and pDebug.Ass it to
      ** xBestIndex.
      */
                /* The module name must be defined. Also, by this point there must
              ** be a pointer to an sqlite3_vtab structure. Otherwise
              ** sqlite3ViewGetColumnNames() would have picked up the error.
              */
                Debug.Assert(pTab.azModuleArg != null && pTab.azModuleArg[0] != null);
                Debug.Assert(sqlite3GetVTable(this.db, pTab) != null);
                /* Set the aConstraint[].usable fields and initialize all
      ** output variables to zero.
      **
      ** aConstraint[].usable is true for constraints where the right-hand
      ** side contains only references to tables to the left of the current
      ** table.  In other words, if the constraint is of the form:
      **
      **           column = expr
      **
      ** and we are evaluating a join, then the constraint on column is
      ** only valid if all tables referenced in expr occur to the left
      ** of the table containing column.
      **
      ** The aConstraints[] array contains entries for all constraints
      ** on the current table.  That way we only have to compute it once
      ** even though we might try to pick the best index multiple times.
      ** For each attempt at picking an index, the order of tables in the
      ** join might be different so we have to recompute the usable flag
      ** each time.
      */
                //pIdxCons = *(struct sqlite3_index_constraint**)&pIdxInfo->aConstraint;
                //pUsage = pIdxInfo->aConstraintUsage;
                for (i = 0; i < pIdxInfo.nConstraint; i++)
                {
                    pIdxCons = pIdxInfo.aConstraint[i];
                    pUsage = pIdxInfo.aConstraintUsage;
                    j = pIdxCons.iTermOffset;
                    pTerm = pWC.a[j];
                    pIdxCons.usable = (pTerm.prereqRight & notReady) == 0;
                    pUsage[i] = new sqlite3_index_constraint_usage();
                }
                // memset(pUsage, 0, sizeof(pUsage[0])*pIdxInfo.nConstraint);
                if (pIdxInfo.needToFreeIdxStr != 0)
                {
                    //sqlite3_free(ref pIdxInfo.idxStr);
                }
                pIdxInfo.idxStr = null;
                pIdxInfo.idxNum = 0;
                pIdxInfo.needToFreeIdxStr = 0;
                pIdxInfo.orderByConsumed = false;
                /* ((double)2) In case of SQLITE_OMIT_FLOATING_POINT... */
                pIdxInfo.estimatedCost = SQLITE_BIG_DBL / ((double)2);
                nOrderBy = pIdxInfo.nOrderBy;
                if (null == pOrderBy)
                {
                    pIdxInfo.nOrderBy = 0;
                }
                if (this.vtabBestIndex(pTab, pIdxInfo) != 0)
                {
                    return;
                }
                //pIdxCons = (sqlite3_index_constraint)pIdxInfo.aConstraint;
                for (i = 0; i < pIdxInfo.nConstraint; i++)
                {
                    if (pUsage[i].argvIndex > 0)
                    {
                        //pCost.used |= pWC.a[pIdxCons[i].iTermOffset].prereqRight;
                        pCost.used |= pWC.a[pIdxInfo.aConstraint[i].iTermOffset].prereqRight;
                    }
                }
                /* If there is an ORDER BY clause, and the selected virtual table index
      ** does not satisfy it, increase the cost of the scan accordingly. This
      ** matches the processing for non-virtual tables in bestBtreeIndex().
      */
                rCost = pIdxInfo.estimatedCost;
                if (pOrderBy != null && !pIdxInfo.orderByConsumed)
                {
                    rCost += estLog(rCost) * rCost;
                }
                /* The cost is not allowed to be larger than SQLITE_BIG_DBL (the
      ** inital value of lowestCost in this loop. If it is, then the
      ** (cost<lowestCost) test below will never be true.
      **
      ** Use "(double)2" instead of "2.0" in case OMIT_FLOATING_POINT
      ** is defined.
      */
                if ((SQLITE_BIG_DBL / ((double)2)) < rCost)
                {
                    pCost.rCost = (SQLITE_BIG_DBL / ((double)2));
                }
                else
                {
                    pCost.rCost = rCost;
                }
                pCost.plan.u.pVtabIdx = pIdxInfo;
                if (pIdxInfo.orderByConsumed)
                {
                    pCost.plan.wsFlags |= WHERE_ORDERBY;
                }
                pCost.plan.nEq = 0;
                pIdxInfo.nOrderBy = nOrderBy;
                /* Try to find a more efficient access pattern by using multiple indexes
      ** to optimize an OR expression within the WHERE clause.
      */
                this.bestOrClauseIndex(pWC, pSrc, notReady, notValid, pOrderBy, pCost);
            }
            public void bestBtreeIndex(/* The parsing context */WhereClause pWC,/* The WHERE clause */SrcList_item pSrc,/* The FROM clause term to search */Bitmask notReady,/* Mask of cursors not available for indexing */Bitmask notValid,/* Cursors not available for any purpose */ExprList pOrderBy,/* The ORDER BY clause */ref WhereCost pCost/* Lowest cost query plan */)
            {
                int iCur = pSrc.iCursor;
                /* The cursor of the table to be accessed */
                Index pProbe;
                /* An index we are evaluating */
                Index pIdx;
                /* Copy of pProbe, or zero for IPK index */
                u32 eqTermMask;
                /* Current mask of valid equality operators */
                u32 idxEqTermMask;
                /* Index mask of valid equality operators */
                Index sPk;
                /* A fake index object for the primary key */
                int[] aiRowEstPk = new int[2];
                /* The aiRowEst[] value for the sPk index */
                int aiColumnPk = -1;
                /* The aColumn[] value for the sPk index */
                int wsFlagMask;
                /* Allowed flags in pCost.plan.wsFlag */
                /* Initialize the cost to a worst-case value */
                if (pCost == null)
                    pCost = new WhereCost();
                else
                    pCost.Clear();
                //memset(pCost, 0, sizeof(*pCost));
                pCost.rCost = SQLITE_BIG_DBL;
                /* If the pSrc table is the right table of a LEFT JOIN then we may not
      ** use an index to satisfy IS NULL constraints on that table.  This is
      ** because columns might end up being NULL if the table does not match -
      ** a circumstance which the index cannot help us discover.  Ticket #2177.
      */
                if ((pSrc.jointype & JT_LEFT) != 0)
                {
                    idxEqTermMask = WO_EQ | WO_IN;
                }
                else
                {
                    idxEqTermMask = WO_EQ | WO_IN | WO_ISNULL;
                }
                if (pSrc.pIndex != null)
                {
                    /* An INDEXED BY clause specifies a particular index to use */
                    pIdx = pProbe = pSrc.pIndex;
                    wsFlagMask = ~(WHERE_ROWID_EQ | WHERE_ROWID_RANGE);
                    eqTermMask = idxEqTermMask;
                }
                else
                {
                    /* There is no INDEXED BY clause.  Create a fake Index object in local
        ** variable sPk to represent the rowid primary key index.  Make this
        ** fake index the first in a chain of Index objects with all of the real
        ** indices to follow */
                    Index pFirst;
                    /* First of real indices on the table */
                    sPk = new Index();
                    // memset( &sPk, 0, sizeof( Index ) );
                    sPk.aSortOrder = new byte[1];
                    sPk.azColl = new string[1];
                    sPk.azColl[0] = "";
                    sPk.nColumn = 1;
                    sPk.aiColumn = new int[1];
                    sPk.aiColumn[0] = aiColumnPk;
                    sPk.aiRowEst = aiRowEstPk;
                    sPk.onError = OE_Replace;
                    sPk.pTable = pSrc.pTab;
                    aiRowEstPk[0] = (int)pSrc.pTab.nRowEst;
                    aiRowEstPk[1] = 1;
                    pFirst = pSrc.pTab.pIndex;
                    if (pSrc.notIndexed == 0)
                    {
                        /* The real indices of the table are only considered if the
          ** NOT INDEXED qualifier is omitted from the FROM clause */
                        sPk.pNext = pFirst;
                    }
                    pProbe = sPk;
                    wsFlagMask = ~(WHERE_COLUMN_IN | WHERE_COLUMN_EQ | WHERE_COLUMN_NULL | WHERE_COLUMN_RANGE);
                    eqTermMask = WO_EQ | WO_IN;
                    pIdx = null;
                }
                /* Loop over all indices looking for the best one to use
      */
                for (; pProbe != null; pIdx = pProbe = pProbe.pNext)
                {
                    int[] aiRowEst = pProbe.aiRowEst;
                    double cost;
                    /* Cost of using pProbe */
                    double nRow;
                    /* Estimated number of rows in result set */
                    double log10N = 0;
                    /* base-10 logarithm of nRow (inexact) */
                    int rev = 0;
                    /* True to scan in reverse order */
                    int wsFlags = 0;
                    Bitmask used = 0;
                    /* The following variables are populated based on the properties of
        ** index being evaluated. They are then used to determine the expected
        ** cost and number of rows returned.
        **
        **  nEq: 
        **    Number of equality terms that can be implemented using the index.
        **    In other words, the number of initial fields in the index that
        **    are used in == or IN or NOT NULL constraints of the WHERE clause.
        **
        **  nInMul:  
        **    The "in-multiplier". This is an estimate of how many seek operations 
        **    SQLite must perform on the index in question. For example, if the 
        **    WHERE clause is:
        **
        **      WHERE a IN (1, 2, 3) AND b IN (4, 5, 6)
        **
        **    SQLite must perform 9 lookups on an index on (a, b), so nInMul is 
        **    set to 9. Given the same schema and either of the following WHERE 
        **    clauses:
        **
        **      WHERE a =  1
        **      WHERE a >= 2
        **
        **    nInMul is set to 1.
        **
        **    If there exists a WHERE term of the form "x IN (SELECT ...)", then 
        **    the sub-select is assumed to return 25 rows for the purposes of 
        **    determining nInMul.
        **
        **  bInEst:  
        **    Set to true if there was at least one "x IN (SELECT ...)" term used 
        **    in determining the value of nInMul.  Note that the RHS of the
        **    IN operator must be a SELECT, not a value list, for this variable
        **    to be true.
        **
        **  estBound:
        **    An estimate on the amount of the table that must be searched.  A
        **    value of 100 means the entire table is searched.  Range constraints
        **    might reduce this to a value less than 100 to indicate that only
        **    a fraction of the table needs searching.  In the absence of
        **    sqlite_stat2 ANALYZE data, a single inequality reduces the search
        **    space to 1/4rd its original size.  So an x>? constraint reduces
        **    estBound to 25.  Two constraints (x>? AND x<?) reduce estBound to 6.
        **
        **  bSort:   
        **    Boolean. True if there is an ORDER BY clause that will require an 
        **    external sort (i.e. scanning the index being evaluated will not 
        **    correctly order records).
        **
        **  bLookup: 
        **    Boolean. True if a table lookup is required for each index entry
        **    visited.  In other words, true if this is not a covering index.
        **    This is always false for the rowid primary key index of a table.
        **    For other indexes, it is true unless all the columns of the table
        **    used by the SELECT statement are present in the index (such an
        **    index is sometimes described as a covering index).
        **    For example, given the index on (a, b), the second of the following 
        **    two queries requires table b-tree lookups in order to find the value
        **    of column c, but the first does not because columns a and b are
        **    both available in the index.
        **
        **             SELECT a, b    FROM tbl WHERE a = 1;
        **             SELECT a, b, c FROM tbl WHERE a = 1;
        */
                    int nEq;
                    /* Number of == or IN terms matching index */
                    int bInEst = 0;
                    /* True if "x IN (SELECT...)" seen */
                    int nInMul = 1;
                    /* Number of distinct equalities to lookup */
                    int estBound = 100;
                    /* Estimated reduction in search space */
                    int nBound = 0;
                    /* Number of range constraints seen */
                    int bSort = 0;
                    /* True if external sort required */
                    int bLookup = 0;
                    /* True if not a covering index */
                    WhereTerm pTerm;
                    /* A single term of the WHERE clause */
#if SQLITE_ENABLE_STAT2
																																																																																																														        WhereTerm pFirstTerm = null;  /* First term matching the index */
#endif
                    /* Determine the values of nEq and nInMul */
                    for (nEq = 0; nEq < pProbe.nColumn; nEq++)
                    {
                        int j = pProbe.aiColumn[nEq];
                        pTerm = pWC.findTerm(iCur, j, notReady, eqTermMask, pIdx);
                        if (pTerm == null)
                            break;
                        wsFlags |= (WHERE_COLUMN_EQ | WHERE_ROWID_EQ);
                        if ((pTerm.eOperator & WO_IN) != 0)
                        {
                            Expr pExpr = pTerm.pExpr;
                            wsFlags |= WHERE_COLUMN_IN;
                            if (ExprHasProperty(pExpr, EP_xIsSelect))
                            {
                                /* "x IN (SELECT ...)":  Assume the SELECT returns 25 rows */
                                nInMul *= 25;
                                bInEst = 1;
                            }
                            else
                                if (ALWAYS(pExpr.x.pList != null) && pExpr.x.pList.nExpr != 0)
                                {
                                    /* "x IN (value, value, ...)" */
                                    nInMul *= pExpr.x.pList.nExpr;
                                }
                        }
                        else
                            if ((pTerm.eOperator & WO_ISNULL) != 0)
                            {
                                wsFlags |= WHERE_COLUMN_NULL;
                            }
#if SQLITE_ENABLE_STAT2
																																																																																																																																								          if ( nEq == 0 && pProbe.aSample != null )
            pFirstTerm = pTerm;
#endif
                        used |= pTerm.prereqRight;
                    }
                    /* Determine the value of estBound. */
                    if (nEq < pProbe.nColumn && pProbe.bUnordered == 0)
                    {
                        int j = pProbe.aiColumn[nEq];
                        if (pWC.findTerm(iCur, j, notReady, WO_LT | WO_LE | WO_GT | WO_GE, pIdx) != null)
                        {
                            WhereTerm pTop = pWC.findTerm(iCur, j, notReady, WO_LT | WO_LE, pIdx);
                            WhereTerm pBtm = pWC.findTerm(iCur, j, notReady, WO_GT | WO_GE, pIdx);
                            this.whereRangeScanEst(pProbe, nEq, pBtm, pTop, out estBound);
                            if (pTop != null)
                            {
                                nBound = 1;
                                wsFlags |= WHERE_TOP_LIMIT;
                                used |= pTop.prereqRight;
                            }
                            if (pBtm != null)
                            {
                                nBound++;
                                wsFlags |= WHERE_BTM_LIMIT;
                                used |= pBtm.prereqRight;
                            }
                            wsFlags |= (WHERE_COLUMN_RANGE | WHERE_ROWID_RANGE);
                        }
                    }
                    else
                        if (pProbe.onError != OE_None)
                        {
                            testcase(wsFlags & WHERE_COLUMN_IN);
                            testcase(wsFlags & WHERE_COLUMN_NULL);
                            if ((wsFlags & (WHERE_COLUMN_IN | WHERE_COLUMN_NULL)) == 0)
                            {
                                wsFlags |= WHERE_UNIQUE;
                            }
                        }
                    /* If there is an ORDER BY clause and the index being considered will
        ** naturally scan rows in the required order, set the appropriate flags
        ** in wsFlags. Otherwise, if there is an ORDER BY clause but the index
        ** will scan rows in a different order, set the bSort variable.  */
                    if (pOrderBy != null)
                    {
                        if ((wsFlags & WHERE_COLUMN_IN) == 0 && pProbe.bUnordered == 0 && this.isSortingIndex(pWC.pMaskSet, pProbe, iCur, pOrderBy, nEq, wsFlags, ref rev))
                        {
                            wsFlags |= WHERE_ROWID_RANGE | WHERE_COLUMN_RANGE | WHERE_ORDERBY;
                            wsFlags |= (rev != 0 ? WHERE_REVERSE : 0);
                        }
                        else
                        {
                            bSort = 1;
                        }
                    }
                    /* If currently calculating the cost of using an index (not the IPK
        ** index), determine if all required column data may be obtained without 
        ** using the main table (i.e. if the index is a covering
        ** index for this query). If it is, set the WHERE_IDX_ONLY flag in
        ** wsFlags. Otherwise, set the bLookup variable to true.  */
                    if (pIdx != null && wsFlags != 0)
                    {
                        Bitmask m = pSrc.colUsed;
                        int j;
                        for (j = 0; j < pIdx.nColumn; j++)
                        {
                            int x = pIdx.aiColumn[j];
                            if (x < BMS - 1)
                            {
                                m &= ~(((Bitmask)1) << x);
                            }
                        }
                        if (m == 0)
                        {
                            wsFlags |= WHERE_IDX_ONLY;
                        }
                        else
                        {
                            bLookup = 1;
                        }
                    }
                    /*
        ** Estimate the number of rows of output.  For an "x IN (SELECT...)"
        ** constraint, do not let the estimate exceed half the rows in the table.
        */
                    nRow = (double)(aiRowEst[nEq] * nInMul);
                    if (bInEst != 0 && nRow * 2 > aiRowEst[0])
                    {
                        nRow = aiRowEst[0] / 2;
                        nInMul = (int)(nRow / aiRowEst[nEq]);
                    }
#if SQLITE_ENABLE_STAT2
																																																																																																														        /* If the constraint is of the form x=VALUE and histogram
    ** data is available for column x, then it might be possible
    ** to get a better estimate on the number of rows based on
    ** VALUE and how common that value is according to the histogram.
    */
        if ( nRow > (double)1 && nEq == 1 && pFirstTerm != null )
        {
          if ( ( pFirstTerm.eOperator & ( WO_EQ | WO_ISNULL ) ) != 0 )
          {
            testcase( pFirstTerm.eOperator == WO_EQ );
            testcase( pFirstTerm.eOperator == WO_ISNULL );
            whereEqualScanEst( pParse, pProbe, pFirstTerm.pExpr.pRight, ref nRow );
          }
          else if ( pFirstTerm.eOperator == WO_IN && bInEst == 0 )
          {
            whereInScanEst( pParse, pProbe, pFirstTerm.pExpr.x.pList, ref nRow );
          }
        }
#endif
                    /* Adjust the number of output rows and downward to reflect rows
    ** that are excluded by range constraints.
    */
                    nRow = (nRow * (double)estBound) / (double)100;
                    if (nRow < 1)
                        nRow = 1;
                    /* Experiments run on real SQLite databases show that the time needed
        ** to do a binary search to locate a row in a table or index is roughly
        ** log10(N) times the time to move from one row to the next row within
        ** a table or index.  The actual times can vary, with the size of
        ** records being an important factor.  Both moves and searches are
        ** slower with larger records, presumably because fewer records fit
        ** on one page and hence more pages have to be fetched.
        **
        ** The ANALYZE command and the sqlite_stat1 and sqlite_stat2 tables do
        ** not give us data on the relative sizes of table and index records.
        ** So this computation assumes table records are about twice as big
        ** as index records
        */
                    if ((wsFlags & WHERE_NOT_FULLSCAN) == 0)
                    {
                        /* The cost of a full table scan is a number of move operations equal
          ** to the number of rows in the table.
          **
          ** We add an additional 4x penalty to full table scans.  This causes
          ** the cost function to err on the side of choosing an index over
          ** choosing a full scan.  This 4x full-scan penalty is an arguable
          ** decision and one which we expect to revisit in the future.  But
          ** it seems to be working well enough at the moment.
          */
                        cost = aiRowEst[0] * 4;
                    }
                    else
                    {
                        log10N = estLog(aiRowEst[0]);
                        cost = nRow;
                        if (pIdx != null)
                        {
                            if (bLookup != 0)
                            {
                                /* For an index lookup followed by a table lookup:
              **    nInMul index searches to find the start of each index range
              **  + nRow steps through the index
              **  + nRow table searches to lookup the table entry using the rowid
              */
                                cost += (nInMul + nRow) * log10N;
                            }
                            else
                            {
                                /* For a covering index:
              **     nInMul index searches to find the initial entry 
              **   + nRow steps through the index
              */
                                cost += nInMul * log10N;
                            }
                        }
                        else
                        {
                            /* For a rowid primary key lookup:
            **    nInMult table searches to find the initial entry for each range
            **  + nRow steps through the table
            */
                            cost += nInMul * log10N;
                        }
                    }
                    /* Add in the estimated cost of sorting the result.  Actual experimental
        ** measurements of sorting performance in SQLite show that sorting time
        ** adds C*N*log10(N) to the cost, where N is the number of rows to be 
        ** sorted and C is a factor between 1.95 and 4.3.  We will split the
        ** difference and select C of 3.0.
        */
                    if (bSort != 0)
                    {
                        cost += nRow * estLog(nRow) * 3;
                    }
                    /**** Cost of using this index has now been computed ****/
                    /* If there are additional constraints on this table that cannot
** be used with the current index, but which might lower the number
** of output rows, adjust the nRow value accordingly.  This only 
** matters if the current index is the least costly, so do not bother
** with this step if we already know this index will not be chosen.
** Also, never reduce the output row count below 2 using this step.
**
** It is critical that the notValid mask be used here instead of
** the notReady mask.  When computing an "optimal" index, the notReady
** mask will only have one bit set - the bit for the current table.
** The notValid mask, on the other hand, always has all bits set for
** tables that are not in outer loops.  If notReady is used here instead
** of notValid, then a optimal index that depends on inner joins loops
** might be selected even when there exists an optimal index that has
** no such dependency.
*/
                    if (nRow > 2 && cost <= pCost.rCost)
                    {
                        //int k;                       /* Loop counter */
                        int nSkipEq = nEq;
                        /* Number of == constraints to skip */
                        int nSkipRange = nBound;
                        /* Number of < constraints to skip */
                        Bitmask thisTab;
                        /* Bitmap for pSrc */
                        thisTab = pWC.pMaskSet.getMask(iCur);
                        for (int ipTerm = 0, k = pWC.nTerm; nRow > 2 && k != 0; k--, ipTerm++)//pTerm++)
                        {
                            pTerm = pWC.a[ipTerm];
                            if ((pTerm.wtFlags & TERM_VIRTUAL) != 0)
                                continue;
                            if ((pTerm.prereqAll & notValid) != thisTab)
                                continue;
                            if ((pTerm.eOperator & (WO_EQ | WO_IN | WO_ISNULL)) != 0)
                            {
                                if (nSkipEq != 0)
                                {
                                    /* Ignore the first nEq equality matches since the index
                ** has already accounted for these */
                                    nSkipEq--;
                                }
                                else
                                {
                                    /* Assume each additional equality match reduces the result
                ** set size by a factor of 10 */
                                    nRow /= 10;
                                }
                            }
                            else
                                if ((pTerm.eOperator & (WO_LT | WO_LE | WO_GT | WO_GE)) != 0)
                                {
                                    if (nSkipRange != 0)
                                    {
                                        /* Ignore the first nSkipRange range constraints since the index
                ** has already accounted for these */
                                        nSkipRange--;
                                    }
                                    else
                                    {
                                        /* Assume each additional range constraint reduces the result
                ** set size by a factor of 3.  Indexed range constraints reduce
                ** the search space by a larger factor: 4.  We make indexed range
                ** more selective intentionally because of the subjective 
                ** observation that indexed range constraints really are more
                ** selective in practice, on average. */
                                        nRow /= 3;
                                    }
                                }
                                else
                                    if (pTerm.eOperator != WO_NOOP)
                                    {
                                        /* Any other expression lowers the output row count by half */
                                        nRow /= 2;
                                    }
                        }
                        if (nRow < 2)
                            nRow = 2;
                    }
#if (SQLITE_TEST) && (SQLITE_DEBUG)
																																																																																																														        WHERETRACE(
        "%s(%s): nEq=%d nInMul=%d estBound=%d bSort=%d bLookup=%d wsFlags=0x%x\n" +
      "         notReady=0x%llx log10N=%.1f nRow=%.1f cost=%.1f used=0x%llx\n",
        pSrc.pTab.zName, ( pIdx != null ? pIdx.zName : "ipk" ),
        nEq, nInMul, estBound, bSort, bLookup, wsFlags,
        notReady, log10N, cost, used
        );
#endif
                    /* If this index is the best we have seen so far, then record this
** index and its cost in the pCost structure.
*/
                    if ((null == pIdx || wsFlags != 0) && (cost < pCost.rCost || (cost <= pCost.rCost && nRow < pCost.plan.nRow)))
                    {
                        pCost.rCost = cost;
                        pCost.used = used;
                        pCost.plan.nRow = nRow;
                        pCost.plan.wsFlags = (uint)(wsFlags & wsFlagMask);
                        pCost.plan.nEq = (uint)nEq;
                        pCost.plan.u.pIdx = pIdx;
                    }
                    /* If there was an INDEXED BY clause, then only that one index is
        ** considered. */
                    if (pSrc.pIndex != null)
                        break;
                    /* Reset masks for the next index in the loop */
                    wsFlagMask = ~(WHERE_ROWID_EQ | WHERE_ROWID_RANGE);
                    eqTermMask = idxEqTermMask;
                }
                /* If there is no ORDER BY clause and the SQLITE_ReverseOrder flag
      ** is set, then reverse the order that the index will be scanned
      ** in. This is used for application testing, to help find cases
      ** where application behaviour depends on the (undefined) order that
      ** SQLite outputs rows in in the absence of an ORDER BY clause.  */
                if (null == pOrderBy && (this.db.flags & SQLITE_ReverseOrder) != 0)
                {
                    pCost.plan.wsFlags |= WHERE_REVERSE;
                }
                Debug.Assert(pOrderBy != null || (pCost.plan.wsFlags & WHERE_ORDERBY) == 0);
                Debug.Assert(pCost.plan.u.pIdx == null || (pCost.plan.wsFlags & WHERE_ROWID_EQ) == 0);
                Debug.Assert(pSrc.pIndex == null || pCost.plan.u.pIdx == null || pCost.plan.u.pIdx == pSrc.pIndex);
#if (SQLITE_TEST) && (SQLITE_DEBUG)
																																																																																				      WHERETRACE( "best index is: %s\n",
      ( ( pCost.plan.wsFlags & WHERE_NOT_FULLSCAN ) == 0 ? "none" :
      pCost.plan.u.pIdx != null ? pCost.plan.u.pIdx.zName : "ipk" )
      );
#endif
                this.bestOrClauseIndex(pWC, pSrc, notReady, notValid, pOrderBy, pCost);
                this.bestAutomaticIndex(pWC, pSrc, notReady, pCost);
                pCost.plan.wsFlags |= (u32)eqTermMask;
            }
            public void bestIndex(/* The parsing context */WhereClause pWC,/* The WHERE clause */SrcList_item pSrc,/* The FROM clause term to search */Bitmask notReady,/* Mask of cursors not available for indexing */Bitmask notValid,/* Cursors not available for any purpose */ExprList pOrderBy,/* The ORDER BY clause */ref WhereCost pCost/* Lowest cost query plan */)
            {
#if !SQLITE_OMIT_VIRTUALTABLE
                if (IsVirtual(pSrc.pTab))
                {
                    sqlite3_index_info p = null;
                    this.bestVirtualIndex(pWC, pSrc, notReady, notValid, pOrderBy, ref pCost, ref p);
                    if (p.needToFreeIdxStr != 0)
                    {
                        //sqlite3_free(ref p.idxStr);
                    }
                    this.db.sqlite3DbFree(ref p);
                }
                else
#endif
                {
                    this.bestBtreeIndex(pWC, pSrc, notReady, notValid, pOrderBy, ref pCost);
                }
            }
            public void codeApplyAffinity(int _base, int n, string zAff)
            {
                Vdbe v = this.pVdbe;
                //if (zAff == 0)
                //{
                //  Debug.Assert(pParse.db.mallocFailed);
                //  return;
                //}
                Debug.Assert(v != null);
                /* Adjust base and n to skip over SQLITE_AFF_NONE entries at the beginning
      ** and end of the affinity string.
      */
                while (n > 0 && zAff[0] == SQLITE_AFF_NONE)
                {
                    n--;
                    _base++;
                    zAff = zAff.Substring(1);
                    // zAff++;
                }
                while (n > 1 && zAff[n - 1] == SQLITE_AFF_NONE)
                {
                    n--;
                }
                /* Code the OP_Affinity opcode if there is anything left to do. */
                if (n > 0)
                {
                    v.sqlite3VdbeAddOp2(OP_Affinity, _base, n);
                    v.sqlite3VdbeChangeP4(-1, zAff, n);
                    this.sqlite3ExprCacheAffinityChange(_base, n);
                }
            }
            public int codeEqualityTerm(/* The parsing context */WhereTerm pTerm,/* The term of the WHERE clause to be coded */WhereLevel pLevel,/* When level of the FROM clause we are working on */int iTarget/* Attempt to leave results in this register */)
            {
                Expr pX = pTerm.pExpr;
                Vdbe v = this.pVdbe;
                int iReg;
                /* Register holding results */
                Debug.Assert(iTarget > 0);
                if (pX.Operator == Operator.TK_EQ)
                {
                    iReg = this.sqlite3ExprCodeTarget(pX.pRight, iTarget);
                }
                else
                    if (pX.Operator == Operator.TK_ISNULL)
                    {
                        iReg = iTarget;
                        v.sqlite3VdbeAddOp2(OP_Null, 0, iReg);
#if !SQLITE_OMIT_SUBQUERY
                    }
                    else
                    {
                        int eType;
                        int iTab;
                        InLoop pIn;
                        Debug.Assert(pX.Operator == Operator.TK_IN);
                        iReg = iTarget;
                        int iDummy = -1;
                        eType = this.sqlite3FindInIndex(pX, ref iDummy);
                        iTab = pX.iTable;
                        v.sqlite3VdbeAddOp2(OP_Rewind, iTab, 0);
                        Debug.Assert((pLevel.plan.wsFlags & WHERE_IN_ABLE) != 0);
                        if (pLevel.u._in.nIn == 0)
                        {
                            pLevel.addrNxt = v.sqlite3VdbeMakeLabel();
                        }
                        pLevel.u._in.nIn++;
                        if (pLevel.u._in.aInLoop == null)
                            pLevel.u._in.aInLoop = new InLoop[pLevel.u._in.nIn];
                        else
                            Array.Resize(ref pLevel.u._in.aInLoop, pLevel.u._in.nIn);
                        //sqlite3DbReallocOrFree(pParse.db, pLevel.u._in.aInLoop,
                        //                       sizeof(pLevel.u._in.aInLoop[0])*pLevel.u._in.nIn);
                        //pIn = pLevel.u._in.aInLoop;
                        if (pLevel.u._in.aInLoop != null)//(pIn )
                        {
                            pLevel.u._in.aInLoop[pLevel.u._in.nIn - 1] = new InLoop();
                            pIn = pLevel.u._in.aInLoop[pLevel.u._in.nIn - 1];
                            //pIn++
                            pIn.iCur = iTab;
                            if (eType == IN_INDEX_ROWID)
                            {
                                pIn.addrInTop = v.sqlite3VdbeAddOp2(OP_Rowid, iTab, iReg);
                            }
                            else
                            {
                                pIn.addrInTop = v.sqlite3VdbeAddOp3(OP_Column, iTab, 0, iReg);
                            }
                            v.sqlite3VdbeAddOp1(OP_IsNull, iReg);
                        }
                        else
                        {
                            pLevel.u._in.nIn = 0;
                        }
#endif
                    }
                pLevel.disableTerm(pTerm);
                return iReg;
            }
            public int codeAllEqualityTerms(/* Parsing context */WhereLevel pLevel,/* Which nested loop of the FROM we are coding */WhereClause pWC,/* The WHERE clause */Bitmask notReady,/* Which parts of FROM have not yet been coded */int nExtraReg,/* Number of extra registers to allocate */out StringBuilder pzAff/* OUT: Set to point to affinity string */)
            {
                int nEq = (int)pLevel.plan.nEq;
                /* The number of == or IN constraints to code */
                Vdbe v = this.pVdbe;
                /* The vm under construction */
                Index pIdx;
                /* The index being used for this loop */
                int iCur = pLevel.iTabCur;
                /* The cursor of the table */
                WhereTerm pTerm;
                /* A single constraint term */
                int j;
                /* Loop counter */
                int regBase;
                /* Base register */
                int nReg;
                /* Number of registers to allocate */
                StringBuilder zAff;
                /* Affinity string to return */
                /* This module is only called on query plans that use an index. */
                Debug.Assert((pLevel.plan.wsFlags & WHERE_INDEXED) != 0);
                pIdx = pLevel.plan.u.pIdx;
                /* Figure out how many memory cells we will need then allocate them.
      */
                regBase = this.nMem + 1;
                nReg = (int)(pLevel.plan.nEq + nExtraReg);
                this.nMem += nReg;
                zAff = new StringBuilder(v.sqlite3IndexAffinityStr(pIdx));
                //sqlite3DbStrDup(pParse.db, sqlite3IndexAffinityStr(v, pIdx));
                //if( null==zAff ){
                //  pParse.db.mallocFailed = 1;
                //}
                /* Evaluate the equality constraints
      */
                Debug.Assert(pIdx.nColumn >= nEq);
                for (j = 0; j < nEq; j++)
                {
                    int r1;
                    int k = pIdx.aiColumn[j];
                    pTerm = pWC.findTerm(iCur, k, notReady, pLevel.plan.wsFlags, pIdx);
                    if (NEVER(pTerm == null))
                        break;
                    /* The following true for indices with redundant columns. 
        ** Ex: CREATE INDEX i1 ON t1(a,b,a); SELECT * FROM t1 WHERE a=0 AND b=0; */
                    testcase((pTerm.wtFlags & TERM_CODED) != 0);
                    testcase(pTerm.wtFlags & TERM_VIRTUAL);
                    /* EV: R-30575-11662 */
                    r1 = this.codeEqualityTerm(pTerm, pLevel, regBase + j);
                    if (r1 != regBase + j)
                    {
                        if (nReg == 1)
                        {
                            this.sqlite3ReleaseTempReg(regBase);
                            regBase = r1;
                        }
                        else
                        {
                            v.sqlite3VdbeAddOp2(OP_SCopy, r1, regBase + j);
                        }
                    }
                    testcase(pTerm.eOperator & WO_ISNULL);
                    testcase(pTerm.eOperator & WO_IN);
                    if ((pTerm.eOperator & (WO_ISNULL | WO_IN)) == 0)
                    {
                        Expr pRight = pTerm.pExpr.pRight;
                        sqlite3ExprCodeIsNullJump(v, pRight, regBase + j, pLevel.addrBrk);
                        if (zAff.Length > 0)
                        {
                            if (pRight.sqlite3CompareAffinity(zAff[j]) == SQLITE_AFF_NONE)
                            {
                                zAff[j] = SQLITE_AFF_NONE;
                            }
                            if ((sqlite3ExprNeedsNoAffinityChange(pRight, zAff[j])) != 0)
                            {
                                zAff[j] = SQLITE_AFF_NONE;
                            }
                        }
                    }
                }
                pzAff = zAff;
                return regBase;
            }
            public void explainOneScan(/* Parse context */SrcList pTabList,/* Table list this loop refers to */WhereLevel pLevel,/* Scan to write OP_Explain opcode for */int iLevel,/* Value for "level" column of output */int iFrom,/* Value for "from" column of output */u16 wctrlFlags/* Flags passed to sqlite3WhereBegin() */)
            {
                if (this.explain == 2)
                {
                    u32 flags = pLevel.plan.wsFlags;
                    SrcList_item pItem = pTabList.a[pLevel.iFrom];
                    Vdbe v = this.pVdbe;
                    /* VM being constructed */
                    sqlite3 db = this.db;
                    /* Database handle */
                    StringBuilder zMsg = new StringBuilder(1000);
                    /* Text to add to EQP output */
                    sqlite3_int64 nRow;
                    /* Expected number of rows visited by scan */
                    int iId = this.iSelectId;
                    /* Select id (left-most output column) */
                    bool isSearch;
                    /* True for a SEARCH. False for SCAN. */
                    if ((flags & WHERE_MULTI_OR) != 0 || (wctrlFlags & WHERE_ONETABLE_ONLY) != 0)
                        return;
                    isSearch = (pLevel.plan.nEq > 0) || (flags & (WHERE_BTM_LIMIT | WHERE_TOP_LIMIT)) != 0 || (wctrlFlags & (WHERE_ORDERBY_MIN | WHERE_ORDERBY_MAX)) != 0;
                    zMsg.Append(sqlite3MPrintf(db, "%s", isSearch ? "SEARCH" : "SCAN"));
                    if (pItem.pSelect != null)
                    {
                        zMsg.Append(sqlite3MAppendf(db, null, " SUBQUERY %d", pItem.iSelectId));
                    }
                    else
                    {
                        zMsg.Append(sqlite3MAppendf(db, null, " TABLE %s", pItem.zName));
                    }
                    if (pItem.zAlias != null)
                    {
                        zMsg.Append(sqlite3MAppendf(db, null, " AS %s", pItem.zAlias));
                    }
                    if ((flags & WHERE_INDEXED) != 0)
                    {
                        string zWhere = db.explainIndexRange(pLevel, pItem.pTab);
                        zMsg.Append(sqlite3MAppendf(db, null, " USING %s%sINDEX%s%s%s", ((flags & WHERE_TEMP_INDEX) != 0 ? "AUTOMATIC " : ""), ((flags & WHERE_IDX_ONLY) != 0 ? "COVERING " : ""), ((flags & WHERE_TEMP_INDEX) != 0 ? "" : " "), ((flags & WHERE_TEMP_INDEX) != 0 ? "" : pLevel.plan.u.pIdx.zName), zWhere != null ? zWhere : ""));
                        db.sqlite3DbFree(ref zWhere);
                    }
                    else
                        if ((flags & (WHERE_ROWID_EQ | WHERE_ROWID_RANGE)) != 0)
                        {
                            zMsg.Append(" USING INTEGER PRIMARY KEY");
                            if ((flags & WHERE_ROWID_EQ) != 0)
                            {
                                zMsg.Append(" (rowid=?)");
                            }
                            else
                                if ((flags & WHERE_BOTH_LIMIT) == WHERE_BOTH_LIMIT)
                                {
                                    zMsg.Append(" (rowid>? AND rowid<?)");
                                }
                                else
                                    if ((flags & WHERE_BTM_LIMIT) != 0)
                                    {
                                        zMsg.Append(" (rowid>?)");
                                    }
                                    else
                                        if ((flags & WHERE_TOP_LIMIT) != 0)
                                        {
                                            zMsg.Append(" (rowid<?)");
                                        }
                        }
#if !SQLITE_OMIT_VIRTUALTABLE
                        else
                            if ((flags & WHERE_VIRTUALTABLE) != 0)
                            {
                                sqlite3_index_info pVtabIdx = pLevel.plan.u.pVtabIdx;
                                zMsg.Append(sqlite3MAppendf(db, null, " VIRTUAL TABLE INDEX %d:%s", pVtabIdx.idxNum, pVtabIdx.idxStr));
                            }
#endif
                    if ((wctrlFlags & (WHERE_ORDERBY_MIN | WHERE_ORDERBY_MAX)) != 0)
                    {
                        testcase(wctrlFlags & WHERE_ORDERBY_MIN);
                        nRow = 1;
                    }
                    else
                    {
                        nRow = (sqlite3_int64)pLevel.plan.nRow;
                    }
                    zMsg.Append(sqlite3MAppendf(db, null, " (~%lld rows)", nRow));
                    v.sqlite3VdbeAddOp4(OP_Explain, iId, iLevel, iFrom, zMsg, P4_DYNAMIC);
                }
            }
            public WhereInfo sqlite3WhereBegin(/* The parser context */SrcList pTabList,/* A list of all tables to be scanned */Expr pWhere,/* The WHERE clause */ref ExprList ppOrderBy,/* An ORDER BY clause, or NULL */u16 wctrlFlags/* One of the WHERE_* flags defined in sqliteInt.h */)
            {
                int i;
                /* Loop counter */
                int nByteWInfo;
                /* Num. bytes allocated for WhereInfo struct */
                int nTabList;
                /* Number of elements in pTabList */
                WhereInfo pWInfo;
                /* Will become the return value of this function */
                Vdbe v = this.pVdbe;
                /* The virtual data_base engine */
                Bitmask notReady;
                /* Cursors that are not yet positioned */
                WhereMaskSet pMaskSet;
                /* The expression mask set */
                WhereClause pWC = new WhereClause();
                /* Decomposition of the WHERE clause */
                SrcList_item pTabItem;
                /* A single entry from pTabList */
                WhereLevel pLevel;
                /* A single level in the pWInfo list */
                int iFrom;
                /* First unused FROM clause element */
                int andFlags;
                /* AND-ed combination of all pWC.a[].wtFlags */
                sqlite3 db;
                /* Data_base connection */
                /* The number of tables in the FROM clause is limited by the number of
** bits in a Bitmask
*/
                testcase(pTabList.nSrc == BMS);
                if (pTabList.nSrc > BMS)
                {
                    sqlite3ErrorMsg(this, "at most %d tables in a join", BMS);
                    return null;
                }
                /* This function normally generates a nested loop for all tables in 
      ** pTabList.  But if the WHERE_ONETABLE_ONLY flag is set, then we should
      ** only generate code for the first table in pTabList and assume that
      ** any cursors associated with subsequent tables are uninitialized.
      */
                nTabList = ((wctrlFlags & WHERE_ONETABLE_ONLY) != 0) ? 1 : (int)pTabList.nSrc;
                /* Allocate and initialize the WhereInfo structure that will become the
      ** return value. A single allocation is used to store the WhereInfo
      ** struct, the contents of WhereInfo.a[], the WhereClause structure
      ** and the WhereMaskSet structure. Since WhereClause contains an 8-byte
      ** field (type Bitmask) it must be aligned on an 8-byte boundary on
      ** some architectures. Hence the ROUND8() below.
      */
                db = this.db;
                pWInfo = new WhereInfo();
                //nByteWInfo = ROUND8(sizeof(WhereInfo)+(nTabList-1)*sizeof(WhereLevel));
                //pWInfo = sqlite3DbMallocZero( db,
                //    nByteWInfo +
                //    sizeof( WhereClause ) +
                //    sizeof( WhereMaskSet )
                //);
                pWInfo.a = new WhereLevel[pTabList.nSrc];
                for (int ai = 0; ai < pWInfo.a.Length; ai++)
                {
                    pWInfo.a[ai] = new WhereLevel();
                }
                //if ( db.mallocFailed != 0 )
                //{
                //sqlite3DbFree(db, pWInfo);
                //pWInfo = 0;
                //  goto whereBeginError;
                //}
                pWInfo.nLevel = nTabList;
                pWInfo.pParse = this;
                pWInfo.pTabList = pTabList;
                pWInfo.iBreak = v.sqlite3VdbeMakeLabel();
                pWInfo.pWC = pWC = new WhereClause();
                // (WhereClause )((u8 )pWInfo)[nByteWInfo];
                pWInfo.wctrlFlags = wctrlFlags;
                pWInfo.savedNQueryLoop = this.nQueryLoop;
                //pMaskSet = (WhereMaskSet)pWC[1];
                /* Split the WHERE clause into separate subexpressions where each
      ** subexpression is separated by an AND operator.
      */
                pMaskSet = new WhereMaskSet();
                //initMaskSet(pMaskSet);
                pWC.whereClauseInit(this, pMaskSet);
                this.sqlite3ExprCodeConstants(pWhere);
                pWC.whereSplit(pWhere, Operator.TK_AND);
                /* IMP: R-15842-53296 */
                /* Special case: a WHERE clause that is constant.  Evaluate the
** expression and either jump over all of the code or fall thru.
*/
                if (pWhere != null && (nTabList == 0 || pWhere.sqlite3ExprIsConstantNotJoin() != 0))
                {
                    this.sqlite3ExprIfFalse(pWhere, pWInfo.iBreak, SQLITE_JUMPIFNULL);
                    pWhere = null;
                }
                /* Assign a bit from the bitmask to every term in the FROM clause.
      **
      ** When assigning bitmask values to FROM clause cursors, it must be
      ** the case that if X is the bitmask for the N-th FROM clause term then
      ** the bitmask for all FROM clause terms to the left of the N-th term
      ** is (X-1).   An expression from the ON clause of a LEFT JOIN can use
      ** its Expr.iRightJoinTable value to find the bitmask of the right table
      ** of the join.  Subtracting one from the right table bitmask gives a
      ** bitmask for all tables to the left of the join.  Knowing the bitmask
      ** for all tables to the left of a left join is important.  Ticket #3015.
      **
      ** Configure the WhereClause.vmask variable so that bits that correspond
      ** to virtual table cursors are set. This is used to selectively disable
      ** the OR-to-IN transformation in exprAnalyzeOrTerm(). It is not helpful
      ** with virtual tables.
      **
      ** Note that bitmasks are created for all pTabList.nSrc tables in
      ** pTabList, not just the first nTabList tables.  nTabList is normally
      ** equal to pTabList.nSrc but might be shortened to 1 if the
      ** WHERE_ONETABLE_ONLY flag is set.
      */
                Debug.Assert(pWC.vmask == 0 && pMaskSet.n == 0);
                for (i = 0; i < pTabList.nSrc; i++)
                {
                    pMaskSet.createMask(pTabList.a[i].iCursor);
#if !SQLITE_OMIT_VIRTUALTABLE
                    if (ALWAYS(pTabList.a[i].pTab) && IsVirtual(pTabList.a[i].pTab))
                    {
                        pWC.vmask |= ((Bitmask)1 << i);
                    }
#endif
                }
#if !NDEBUG
																																																																																				      {
        Bitmask toTheLeft = 0;
        for ( i = 0; i < pTabList.nSrc; i++ )
        {
          Bitmask m = getMask( pMaskSet, pTabList.a[i].iCursor );
          Debug.Assert( ( m - 1 ) == toTheLeft );
          toTheLeft |= m;
        }
      }
#endif
                /* Analyze all of the subexpressions.  Note that exprAnalyze() might
** add new virtual terms onto the end of the WHERE clause.  We do not
** want to analyze these virtual terms, so start analyzing at the end
** and work forward so that the added virtual terms are never processed.
*/
                pTabList.exprAnalyzeAll(pWC);
                //if ( db.mallocFailed != 0 )
                //{
                //  goto whereBeginError;
                //}
                /* Chose the best index to use for each table in the FROM clause.
      **
      ** This loop fills in the following fields:
      **
      **   pWInfo.a[].pIdx      The index to use for this level of the loop.
      **   pWInfo.a[].wsFlags   WHERE_xxx flags Debug.Associated with pIdx
      **   pWInfo.a[].nEq       The number of == and IN constraints
      **   pWInfo.a[].iFrom     Which term of the FROM clause is being coded
      **   pWInfo.a[].iTabCur   The VDBE cursor for the data_base table
      **   pWInfo.a[].iIdxCur   The VDBE cursor for the index
      **   pWInfo.a[].pTerm     When wsFlags==WO_OR, the OR-clause term
      **
      ** This loop also figures out the nesting order of tables in the FROM
      ** clause.
      */
                notReady = ~(Bitmask)0;
                andFlags = ~0;
#if (SQLITE_TEST) && (SQLITE_DEBUG)
																																																																																				      WHERETRACE( "*** Optimizer Start ***\n" );
#endif
                for (i = iFrom = 0; i < nTabList; i++)//, pLevel++ )
                {
                    pLevel = pWInfo.a[i];
                    WhereCost bestPlan;
                    /* Most efficient plan seen so far */
                    Index pIdx;
                    /* Index for FROM table at pTabItem */
                    int j;
                    /* For looping over FROM tables */
                    int bestJ = -1;
                    /* The value of j */
                    Bitmask m;
                    /* Bitmask value for j or bestJ */
                    int isOptimal;
                    /* Iterator for optimal/non-optimal search */
                    int nUnconstrained;
                    /* Number tables without INDEXED BY */
                    Bitmask notIndexed;
                    /* Mask of tables that cannot use an index */
                    bestPlan = new WhereCost();
                    // memset( &bestPlan, 0, sizeof( bestPlan ) );
                    bestPlan.rCost = SQLITE_BIG_DBL;
#if (SQLITE_TEST) && (SQLITE_DEBUG)
																																																																																																														        WHERETRACE( "*** Begin search for loop %d ***\n", i );
#endif
                    /* Loop through the remaining entries in the FROM clause to find the
** next nested loop. The loop tests all FROM clause entries
** either once or twice. 
**
** The first test is always performed if there are two or more entries
** remaining and never performed if there is only one FROM clause entry
** to choose from.  The first test looks for an "optimal" scan.  In
** this context an optimal scan is one that uses the same strategy
** for the given FROM clause entry as would be selected if the entry
** were used as the innermost nested loop.  In other words, a table
** is chosen such that the cost of running that table cannot be reduced
** by waiting for other tables to run first.  This "optimal" test works
** by first assuming that the FROM clause is on the inner loop and finding
** its query plan, then checking to see if that query plan uses any
** other FROM clause terms that are notReady.  If no notReady terms are
** used then the "optimal" query plan works.
**
** Note that the WhereCost.nRow parameter for an optimal scan might
** not be as small as it would be if the table really were the innermost
** join.  The nRow value can be reduced by WHERE clause constraints
** that do not use indices.  But this nRow reduction only happens if the
** table really is the innermost join.  
**
** The second loop iteration is only performed if no optimal scan
** strategies were found by the first iteration. This second iteration
** is used to search for the lowest cost scan overall.
**
** Previous versions of SQLite performed only the second iteration -
** the next outermost loop was always that with the lowest overall
** cost. However, this meant that SQLite could select the wrong plan
** for scripts such as the following:
**   
**   CREATE TABLE t1(a, b); 
**   CREATE TABLE t2(c, d);
**   SELECT * FROM t2, t1 WHERE t2.rowid = t1.a;
**
** The best strategy is to iterate through table t1 first. However it
** is not possible to determine this with a simple greedy algorithm.
** Since the cost of a linear scan through table t2 is the same 
** as the cost of a linear scan through table t1, a simple greedy 
** algorithm may choose to use t2 for the outer loop, which is a much
** costlier approach.
*/
                    nUnconstrained = 0;
                    notIndexed = 0;
                    for (isOptimal = (iFrom < nTabList - 1) ? 1 : 0; isOptimal >= 0 && bestJ < 0; isOptimal--)
                    {
                        Bitmask mask;
                        /* Mask of tables not yet ready */
                        for (j = iFrom; j < nTabList; j++)//, pTabItem++)
                        {
                            pTabItem = pTabList.a[j];
                            int doNotReorder;
                            /* True if this table should not be reordered */
                            WhereCost sCost = new WhereCost();
                            /* Cost information from best[Virtual]Index() */
                            ExprList pOrderBy;
                            /* ORDER BY clause for index to optimize */
                            doNotReorder = (pTabItem.jointype & (JT_LEFT | JT_CROSS)) != 0 ? 1 : 0;
                            if ((j != iFrom && doNotReorder != 0))
                                break;
                            m = pMaskSet.getMask(pTabItem.iCursor);
                            if ((m & notReady) == 0)
                            {
                                if (j == iFrom)
                                    iFrom++;
                                continue;
                            }
                            mask = (isOptimal != 0 ? m : notReady);
                            pOrderBy = ((i == 0 && ppOrderBy != null) ? ppOrderBy : null);
                            if (pTabItem.pIndex == null)
                                nUnconstrained++;
#if (SQLITE_TEST) && (SQLITE_DEBUG)
																																																																																																																																																																		            WHERETRACE( "=== trying table %d with isOptimal=%d ===\n",
            j, isOptimal );
#endif
                            Debug.Assert(pTabItem.pTab != null);
#if !SQLITE_OMIT_VIRTUALTABLE
                            if (IsVirtual(pTabItem.pTab))
                            {
                                sqlite3_index_info pp = pWInfo.a[j].pIdxInfo;
                                this.bestVirtualIndex(pWC, pTabItem, mask, notReady, pOrderBy, ref sCost, ref pp);
                            }
                            else
#endif
                            {
                                this.bestBtreeIndex(pWC, pTabItem, mask, notReady, pOrderBy, ref sCost);
                            }
                            Debug.Assert(isOptimal != 0 || (sCost.used & notReady) == 0);
                            /* If an INDEXED BY clause is present, then the plan must use that
            ** index if it uses any index at all */
                            Debug.Assert(pTabItem.pIndex == null || (sCost.plan.wsFlags & WHERE_NOT_FULLSCAN) == 0 || sCost.plan.u.pIdx == pTabItem.pIndex);
                            if (isOptimal != 0 && (sCost.plan.wsFlags & WHERE_NOT_FULLSCAN) == 0)
                            {
                                notIndexed |= m;
                            }
                            /* Conditions under which this table becomes the best so far:
            **
            **   (1) The table must not depend on other tables that have not
            **       yet run.
            **
            **   (2) A full-table-scan plan cannot supercede indexed plan unless
            **       the full-table-scan is an "optimal" plan as defined above.
            **
            **   (3) All tables have an INDEXED BY clause or this table lacks an
            **       INDEXED BY clause or this table uses the specific
            **       index specified by its INDEXED BY clause.  This rule ensures
            **       that a best-so-far is always selected even if an impossible
            **       combination of INDEXED BY clauses are given.  The error
            **       will be detected and relayed back to the application later.
            **       The NEVER() comes about because rule (2) above prevents
            **       An indexable full-table-scan from reaching rule (3).
            **
            **   (4) The plan cost must be lower than prior plans or else the
            **       cost must be the same and the number of rows must be lower.
            */
                            if ((sCost.used & notReady) == 0/* (1) */&& (bestJ < 0 || (notIndexed & m) != 0/* (2) */|| (bestPlan.plan.wsFlags & WHERE_NOT_FULLSCAN) == 0 || (sCost.plan.wsFlags & WHERE_NOT_FULLSCAN) != 0) && (nUnconstrained == 0 || pTabItem.pIndex == null/* (3) */|| NEVER((sCost.plan.wsFlags & WHERE_NOT_FULLSCAN) != 0)) && (bestJ < 0 || sCost.rCost < bestPlan.rCost/* (4) */|| (sCost.rCost <= bestPlan.rCost && sCost.plan.nRow < bestPlan.plan.nRow)))
                            {
#if (SQLITE_TEST) && (SQLITE_DEBUG)
																																																																																																																																																																																												              WHERETRACE( "=== table %d is best so far" +
              " with cost=%g and nRow=%g\n",
              j, sCost.rCost, sCost.plan.nRow );
#endif
                                bestPlan = sCost;
                                bestJ = j;
                            }
                            if (doNotReorder != 0)
                                break;
                        }
                    }
                    Debug.Assert(bestJ >= 0);
                    Debug.Assert((notReady & pMaskSet.getMask(pTabList.a[bestJ].iCursor)) != 0);
#if (SQLITE_TEST) && (SQLITE_DEBUG)
																																																																																																														        WHERETRACE( "*** Optimizer selects table %d for loop %d" +
        " with cost=%g and nRow=%g\n",
        bestJ, i,//pLevel-pWInfo.a,
        bestPlan.rCost, bestPlan.plan.nRow );
#endif
                    if ((bestPlan.plan.wsFlags & WHERE_ORDERBY) != 0)
                    {
                        ppOrderBy = null;
                    }
                    andFlags = (int)(andFlags & bestPlan.plan.wsFlags);
                    pLevel.plan = bestPlan.plan;
                    testcase(bestPlan.plan.wsFlags & WHERE_INDEXED);
                    testcase(bestPlan.plan.wsFlags & WHERE_TEMP_INDEX);
                    if ((bestPlan.plan.wsFlags & (WHERE_INDEXED | WHERE_TEMP_INDEX)) != 0)
                    {
                        pLevel.iIdxCur = this.nTab++;
                    }
                    else
                    {
                        pLevel.iIdxCur = -1;
                    }
                    notReady &= ~pMaskSet.getMask(pTabList.a[bestJ].iCursor);
                    pLevel.iFrom = (u8)bestJ;
                    if (bestPlan.plan.nRow >= (double)1)
                    {
                        this.nQueryLoop *= bestPlan.plan.nRow;
                    }
                    /* Check that if the table scanned by this loop iteration had an
        ** INDEXED BY clause attached to it, that the named index is being
        ** used for the scan. If not, then query compilation has failed.
        ** Return an error.
        */
                    pIdx = pTabList.a[bestJ].pIndex;
                    if (pIdx != null)
                    {
                        if ((bestPlan.plan.wsFlags & WHERE_INDEXED) == 0)
                        {
                            sqlite3ErrorMsg(this, "cannot use index: %s", pIdx.zName);
                            goto whereBeginError;
                        }
                        else
                        {
                            /* If an INDEXED BY clause is used, the bestIndex() function is
            ** guaranteed to find the index specified in the INDEXED BY clause
            ** if it find an index at all. */
                            Debug.Assert(bestPlan.plan.u.pIdx == pIdx);
                        }
                    }
                }
#if (SQLITE_TEST) && (SQLITE_DEBUG)
																																																																																				      WHERETRACE( "*** Optimizer Finished ***\n" );
#endif
                if (this.nErr != 0/*|| db.mallocFailed != 0 */)
                {
                    goto whereBeginError;
                }
                /* If the total query only selects a single row, then the ORDER BY
      ** clause is irrelevant.
      */
                if ((andFlags & WHERE_UNIQUE) != 0 && ppOrderBy != null)
                {
                    ppOrderBy = null;
                }
                /* If the caller is an UPDATE or DELETE statement that is requesting
      ** to use a one-pDebug.Ass algorithm, determine if this is appropriate.
      ** The one-pass algorithm only works if the WHERE clause constraints
      ** the statement to update a single row.
      */
                Debug.Assert((wctrlFlags & WHERE_ONEPASS_DESIRED) == 0 || pWInfo.nLevel == 1);
                if ((wctrlFlags & WHERE_ONEPASS_DESIRED) != 0 && (andFlags & WHERE_UNIQUE) != 0)
                {
                    pWInfo.okOnePass = 1;
                    pWInfo.a[0].plan.wsFlags = (u32)(pWInfo.a[0].plan.wsFlags & ~WHERE_IDX_ONLY);
                }
                /* Open all tables in the pTabList and any indices selected for
      ** searching those tables.
      */
                sqlite3CodeVerifySchema(this, -1);
                /* Insert the cookie verifier Goto */
                notReady = ~(Bitmask)0;
                pWInfo.nRowOut = (double)1;
                for (i = 0; i < nTabList; i++)//, pLevel++ )
                {
                    pLevel = pWInfo.a[i];
                    Table pTab;
                    /* Table to open */
                    int iDb;
                    /* Index of data_base containing table/index */
                    pTabItem = pTabList.a[pLevel.iFrom];
                    pTab = pTabItem.pTab;
                    pLevel.iTabCur = pTabItem.iCursor;
                    pWInfo.nRowOut *= pLevel.plan.nRow;
                    iDb = sqlite3SchemaToIndex(db, pTab.pSchema);
                    if ((pTab.tabFlags & TF_Ephemeral) != 0 || pTab.pSelect != null)
                    {
                        /* Do nothing */
                    }
                    else
#if !SQLITE_OMIT_VIRTUALTABLE
                        if ((pLevel.plan.wsFlags & WHERE_VIRTUALTABLE) != 0)
                        {
                            VTable pVTab = sqlite3GetVTable(db, pTab);
                            int iCur = pTabItem.iCursor;
                            v.sqlite3VdbeAddOp4(OP_VOpen, iCur, 0, 0, pVTab, P4_VTAB);
                        }
                        else
#endif
                            if ((pLevel.plan.wsFlags & WHERE_IDX_ONLY) == 0 && (wctrlFlags & WHERE_OMIT_OPEN) == 0)
                            {
                                int op = pWInfo.okOnePass != 0 ? OP_OpenWrite : OP_OpenRead;
                                this.sqlite3OpenTable(pTabItem.iCursor, iDb, pTab, op);
                                testcase(pTab.nCol == BMS - 1);
                                testcase(pTab.nCol == BMS);
                                if (0 == pWInfo.okOnePass && pTab.nCol < BMS)
                                {
                                    Bitmask b = pTabItem.colUsed;
                                    int n = 0;
                                    for (; b != 0; b = b >> 1, n++)
                                    {
                                    }
                                    v.sqlite3VdbeChangeP4(v.sqlite3VdbeCurrentAddr() - 1, n, P4_INT32);
                                    //SQLITE_INT_TO_PTR(n)
                                    Debug.Assert(n <= pTab.nCol);
                                }
                            }
                            else
                            {
                                sqlite3TableLock(this, iDb, pTab.tnum, 0, pTab.zName);
                            }
#if !SQLITE_OMIT_AUTOMATIC_INDEX
                    if ((pLevel.plan.wsFlags & WHERE_TEMP_INDEX) != 0)
                    {
                        this.constructAutomaticIndex(pWC, pTabItem, notReady, pLevel);
                    }
                    else
#endif
                        if ((pLevel.plan.wsFlags & WHERE_INDEXED) != 0)
                        {
                            Index pIx = pLevel.plan.u.pIdx;
                            KeyInfo pKey = sqlite3IndexKeyinfo(this, pIx);
                            int iIdxCur = pLevel.iIdxCur;
                            Debug.Assert(pIx.pSchema == pTab.pSchema);
                            Debug.Assert(iIdxCur >= 0);
                            v.sqlite3VdbeAddOp4(OP_OpenRead, iIdxCur, pIx.tnum, iDb, pKey, P4_KEYINFO_HANDOFF);
#if SQLITE_DEBUG
																																																																																																																																																																		            VdbeComment( v, "%s", pIx.zName );
#endif
                        }
                    sqlite3CodeVerifySchema(this, iDb);
                    notReady &= ~pWC.pMaskSet.getMask(pTabItem.iCursor);
                }
                pWInfo.iTop = v.sqlite3VdbeCurrentAddr();
                //if( db.mallocFailed ) goto whereBeginError;
                /* Generate the code to do the search.  Each iteration of the for
      ** loop below generates code for a single nested loop of the VM
      ** program.
      */
                notReady = ~(Bitmask)0;
                for (i = 0; i < nTabList; i++)
                {
                    pLevel = pWInfo.a[i];
                    this.explainOneScan(pTabList, pLevel, i, pLevel.iFrom, wctrlFlags);
                    notReady = pWInfo.codeOneLoopStart(i, wctrlFlags, notReady);
                    pWInfo.iContinue = pLevel.addrCont;
                }
#if SQLITE_TEST
																																																																																				      /* Record in the query plan information about the current table
** and the index used to access it (if any).  If the table itself
** is not used, its name is just '{}'.  If no index is used
** the index is listed as "{}".  If the primary key is used the
** index name is '*'.
*/
#if !TCLSH
																																																																																				      sqlite3_query_plan.Length = 0;
#else
																																																																																				      sqlite3_query_plan.sValue = "";
#endif
																																																																																				      for ( i = 0; i < nTabList; i++ )
      {
        string z;
        int n;
        pLevel = pWInfo.a[i];
        pTabItem = pTabList.a[pLevel.iFrom];
        z = pTabItem.zAlias;
        if ( z == null )
          z = pTabItem.pTab.zName;
        n = StringExtensions.sqlite3Strlen30( z );
        if ( true ) //n+nQPlan < sizeof(sqlite3_query_plan)-10 )
        {
          if ( ( pLevel.plan.wsFlags & WHERE_IDX_ONLY ) != 0 )
          {
            sqlite3_query_plan.Append( "{}" ); //memcpy( &sqlite3_query_plan[nQPlan], "{}", 2 );
            nQPlan += 2;
          }
          else
          {
            sqlite3_query_plan.Append( z ); //memcpy( &sqlite3_query_plan[nQPlan], z, n );
            nQPlan += n;
          }
          sqlite3_query_plan.Append( " " );
          nQPlan++; //sqlite3_query_plan[nQPlan++] = ' ';
        }
        testcase( pLevel.plan.wsFlags & WHERE_ROWID_EQ );
        testcase( pLevel.plan.wsFlags & WHERE_ROWID_RANGE );
        if ( ( pLevel.plan.wsFlags & ( WHERE_ROWID_EQ | WHERE_ROWID_RANGE ) ) != 0 )
        {
          sqlite3_query_plan.Append( "* " ); //memcpy(&sqlite3_query_plan[nQPlan], "* ", 2);
          nQPlan += 2;
        }
        else if ( ( pLevel.plan.wsFlags & WHERE_INDEXED ) != 0 )
        {
          n = StringExtensions.sqlite3Strlen30( pLevel.plan.u.pIdx.zName );
          if ( true ) //n+nQPlan < sizeof(sqlite3_query_plan)-2 )//if( n+nQPlan < sizeof(sqlite3_query_plan)-2 )
          {
            sqlite3_query_plan.Append( pLevel.plan.u.pIdx.zName ); //memcpy(&sqlite3_query_plan[nQPlan], pLevel.plan.u.pIdx.zName, n);
            nQPlan += n;
            sqlite3_query_plan.Append( " " ); //sqlite3_query_plan[nQPlan++] = ' ';
          }
        }
        else
        {
          sqlite3_query_plan.Append( "{} " ); //memcpy( &sqlite3_query_plan[nQPlan], "{} ", 3 );
          nQPlan += 3;
        }
      }
      //while( nQPlan>0 && sqlite3_query_plan[nQPlan-1]==' ' ){
      //  sqlite3_query_plan[--nQPlan] = 0;
      //}
      //sqlite3_query_plan[nQPlan] = 0;
#if !TCLSH
																																																																																				      sqlite3_query_plan = new StringBuilder( sqlite3_query_plan.ToString().Trim() );
#else
																																																																																				      sqlite3_query_plan.Trim();
#endif
																																																																																				      nQPlan = 0;
#endif
                /* Record the continuation address in the WhereInfo structure.  Then
** clean up and return.
*/
                return pWInfo;
            /* Jump here if malloc fails */whereBeginError:
                if (pWInfo != null)
                {
                    this.nQueryLoop = pWInfo.savedNQueryLoop;
                    db.whereInfoFree(pWInfo);
                }
                return null;
            }
            public bool isSortingIndex(/* Parsing context */WhereMaskSet pMaskSet,/* Mapping from table cursor numbers to bitmaps */Index pIdx,/* The index we are testing */int _base,/* Cursor number for the table to be sorted */ExprList pOrderBy,/* The ORDER BY clause */int nEqCol,/* Number of index columns with == constraints */int wsFlags,/* Index usages flags */ref int pbRev/* Set to 1 if ORDER BY is DESC */)
            {
                int i, j;
                /* Loop counters */
                int sortOrder = 0;
                /* XOR of index and ORDER BY sort direction */
                int nTerm;
                /* Number of ORDER BY terms */
                ExprList_item pTerm;
                /* A term of the ORDER BY clause */
                sqlite3 db = this.db;
                Debug.Assert(pOrderBy != null);
                nTerm = pOrderBy.nExpr;
                Debug.Assert(nTerm > 0);
                /* Argument pIdx must either point to a 'real' named index structure, 
      ** or an index structure allocated on the stack by bestBtreeIndex() to
      ** represent the rowid index that is part of every table.  */
                Debug.Assert(!String.IsNullOrEmpty(pIdx.zName) || (pIdx.nColumn == 1 && pIdx.aiColumn[0] == -1));
                /* Match terms of the ORDER BY clause against columns of
      ** the index.
      **
      ** Note that indices have pIdx.nColumn regular columns plus
      ** one additional column containing the rowid.  The rowid column
      ** of the index is also allowed to match against the ORDER BY
      ** clause.
      */
                for (i = j = 0; j < nTerm && i <= pIdx.nColumn; i++)
                {
                    pTerm = pOrderBy.a[j];
                    Expr pExpr;
                    /* The expression of the ORDER BY pTerm */
                    CollSeq pColl;
                    /* The collating sequence of pExpr */
                    int termSortOrder;
                    /* Sort order for this term */
                    int iColumn;
                    /* The i-th column of the index.  -1 for rowid */
                    int iSortOrder;
                    /* 1 for DESC, 0 for ASC on the i-th index term */
                    string zColl;
                    /* Name of the collating sequence for i-th index term */
                    pExpr = pTerm.pExpr;
                    if (pExpr.Operator != Operator.TK_COLUMN || pExpr.iTable != _base)
                    {
                        /* Can not use an index sort on anything that is not a column in the
          ** left-most table of the FROM clause */
                        break;
                    }
                    pColl = this.sqlite3ExprCollSeq(pExpr);
                    if (null == pColl)
                    {
                        pColl = db.pDfltColl;
                    }
                    if (!String.IsNullOrEmpty(pIdx.zName) && i < pIdx.nColumn)
                    {
                        iColumn = pIdx.aiColumn[i];
                        if (iColumn == pIdx.pTable.iPKey)
                        {
                            iColumn = -1;
                        }
                        iSortOrder = pIdx.aSortOrder[i];
                        zColl = pIdx.azColl[i];
                    }
                    else
                    {
                        iColumn = -1;
                        iSortOrder = 0;
                        zColl = pColl.zName;
                    }
                    if (pExpr.iColumn != iColumn || !pColl.zName.Equals(zColl, StringComparison.InvariantCultureIgnoreCase))
                    {
                        /* Term j of the ORDER BY clause does not match column i of the index */
                        if (i < nEqCol)
                        {
                            /* If an index column that is constrained by == fails to match an
            ** ORDER BY term, that is OK.  Just ignore that column of the index
            */
                            continue;
                        }
                        else
                            if (i == pIdx.nColumn)
                            {
                                /* Index column i is the rowid.  All other terms match. */
                                break;
                            }
                            else
                            {
                                /* If an index column fails to match and is not constrained by ==
            ** then the index cannot satisfy the ORDER BY constraint.
            */
                                return false;
                            }
                    }
                    Debug.Assert(pIdx.aSortOrder != null || iColumn == -1);
                    Debug.Assert(pTerm.sortOrder == 0 || pTerm.sortOrder == 1);
                    Debug.Assert(iSortOrder == 0 || iSortOrder == 1);
                    termSortOrder = iSortOrder ^ pTerm.sortOrder;
                    if (i > nEqCol)
                    {
                        if (termSortOrder != sortOrder)
                        {
                            /* Indices can only be used if all ORDER BY terms past the
            ** equality constraints are all either DESC or ASC. */
                            return false;
                        }
                    }
                    else
                    {
                        sortOrder = termSortOrder;
                    }
                    j++;
                    //pTerm++;
                    if (iColumn < 0 && !pOrderBy.referencesOtherTables(pMaskSet, j, _base))
                    {
                        /* If the indexed column is the primary key and everything matches
          ** so far and none of the ORDER BY terms to the right reference other
          ** tables in the join, then we are Debug.Assured that the index can be used
          ** to sort because the primary key is unique and so none of the other
          ** columns will make any difference
          */
                        j = nTerm;
                    }
                }
                pbRev = sortOrder != 0 ? 1 : 0;
                if (j >= nTerm)
                {
                    /* All terms of the ORDER BY clause are covered by this index so
        ** this index can be used for sorting. */
                    return true;
                }
                if (pIdx.onError != OE_None && i == pIdx.nColumn && (wsFlags & WHERE_COLUMN_NULL) == 0 && !pOrderBy.referencesOtherTables(pMaskSet, j, _base))
                {
                    /* All terms of this index match some prefix of the ORDER BY clause
        ** and the index is UNIQUE and no terms on the tail of the ORDER BY
        ** clause reference other tables in a join.  If this is all true then
        ** the order by clause is superfluous.  Not that if the matching
        ** condition is IS NULL then the result is not necessarily unique
        ** even on a UNIQUE index, so disallow those cases. */
                    return true;
                }
                return false;
            }
            public void binaryToUnaryIfNull(Expr pY, Expr pA, int op)
            {
                sqlite3 db = this.db;
                if (/*db.mallocFailed == null && */pY.Operator == Operator.TK_NULL)
                {
                    pA.Operator = (Operator)op;
                    sqlite3ExprDelete(db, ref pA.pRight);
                    pA.pRight = null;
                }
            }
            public int sqlite3FindInIndex(Expr pX, ref int prNotFound)
            {
                Select p;
                /* SELECT to the right of IN operator */
                int eType = 0;
                /* Type of RHS table. IN_INDEX_* */
                int iTab = this.nTab++;
                /* Cursor of the RHS table */
                bool mustBeUnique = (prNotFound != 0);
                /* True if RHS must be unique */
                Debug.Assert(pX.Operator == Operator.TK_IN);
                /* Check to see if an existing table or index can be used to
      ** satisfy the query.  This is preferable to generating a new
      ** ephemeral table.
      */
                p = (ExprHasProperty(pX, EP_xIsSelect) ? pX.x.pSelect : null);
                if (ALWAYS(this.nErr == 0) && isCandidateForInOpt(p) != 0)
                {
                    sqlite3 db = this.db;
                    /* Database connection */
                    Expr pExpr = p.pEList.a[0].pExpr;
                    /* Expression <column> */
                    int iCol = pExpr.iColumn;
                    /* Index of column <column> */
                    Vdbe v = sqlite3GetVdbe(this);
                    /* Virtual machine being coded */
                    Table pTab = p.pSrc.a[0].pTab;
                    /* Table <table>. */
                    int iDb;
                    /* Database idx for pTab */
                    /* Code an OP_VerifyCookie and OP_TableLock for <table>. */
                    iDb = sqlite3SchemaToIndex(db, pTab.pSchema);
                    sqlite3CodeVerifySchema(this, iDb);
                    sqlite3TableLock(this, iDb, pTab.tnum, 0, pTab.zName);
                    /* This function is only called from two places. In both cases the vdbe
        ** has already been allocated. So assume sqlite3GetVdbe() is always
        ** successful here.
        */
                    Debug.Assert(v != null);
                    if (iCol < 0)
                    {
                        int iMem = ++this.nMem;
                        int iAddr;
                        iAddr = v.sqlite3VdbeAddOp1(OP_If, iMem);
                        v.sqlite3VdbeAddOp2(OP_Integer, 1, iMem);
                        this.sqlite3OpenTable(iTab, iDb, pTab, OP_OpenRead);
                        eType = IN_INDEX_ROWID;
                        v.sqlite3VdbeJumpHere(iAddr);
                    }
                    else
                    {
                        Index pIdx;
                        /* Iterator variable */
                        /* The collation sequence used by the comparison. If an index is to
** be used in place of a temp.table, it must be ordered according
** to this collation sequence. */
                        CollSeq pReq = this.sqlite3BinaryCompareCollSeq(pX.pLeft, pExpr);
                        /* Check that the affinity that will be used to perform the
          ** comparison is the same as the affinity of the column. If
          ** it is not, it is not possible to use any index.
          */
                        char aff = pX.comparisonAffinity();
                        bool affinity_ok = (pTab.aCol[iCol].affinity == aff || aff == SQLITE_AFF_NONE);
                        for (pIdx = pTab.pIndex; pIdx != null && eType == 0 && affinity_ok; pIdx = pIdx.pNext)
                        {
                            if ((pIdx.aiColumn[0] == iCol) && (sqlite3FindCollSeq(db, ENC(db), pIdx.azColl[0], 0) == pReq) && (mustBeUnique == false || (pIdx.nColumn == 1 && pIdx.onError != OE_None)))
                            {
                                int iMem = ++this.nMem;
                                int iAddr;
                                KeyInfo pKey;
                                pKey = sqlite3IndexKeyinfo(this, pIdx);
                                iAddr = v.sqlite3VdbeAddOp1(OP_If, iMem);
                                v.sqlite3VdbeAddOp2(OP_Integer, 1, iMem);
                                v.sqlite3VdbeAddOp4(OP_OpenRead, iTab, pIdx.tnum, iDb, pKey, P4_KEYINFO_HANDOFF);
#if SQLITE_DEBUG
																																																																																																																																																																				              VdbeComment( v, "%s", pIdx.zName );
#endif
                                eType = IN_INDEX_INDEX;
                                v.sqlite3VdbeJumpHere(iAddr);
                                if (//prNotFound != null &&         -- always exists under C#
                                pTab.aCol[iCol].notNull == 0)
                                {
                                    prNotFound = ++this.nMem;
                                }
                            }
                        }
                    }
                }
                if (eType == 0)
                {
                    /* Could not found an existing table or index to use as the RHS b-tree.
        ** We will have to generate an ephemeral table to do the job.
        */
                    double savedNQueryLoop = this.nQueryLoop;
                    int rMayHaveNull = 0;
                    eType = IN_INDEX_EPH;
                    if (prNotFound != -1)// Klude to show prNotFound not available
                    {
                        prNotFound = rMayHaveNull = ++this.nMem;
                    }
                    else
                    {
                        testcase(this.nQueryLoop > (double)1);
                        this.nQueryLoop = (double)1;
                        if (pX.pLeft.iColumn < 0 && !ExprHasAnyProperty(pX, EP_xIsSelect))
                        {
                            eType = IN_INDEX_ROWID;
                        }
                    }
                    this.sqlite3CodeSubselect(pX, rMayHaveNull, eType == IN_INDEX_ROWID);
                    this.nQueryLoop = savedNQueryLoop;
                }
                else
                {
                    pX.iTable = iTab;
                }
                return eType;
            }
            public int sqlite3CodeSubselect(/* Parsing context */Expr pExpr,/* The IN, SELECT, or EXISTS operator */int rMayHaveNull,/* Register that records whether NULLs exist in RHS */bool isRowid/* If true, LHS of IN operator is a rowid */)
            {
                int testAddr = 0;
                /* One-time test address */
                int rReg = 0;
                /* Register storing resulting */
                Vdbe v = sqlite3GetVdbe(this);
                if (NEVER(v == null))
                    return 0;
                this.sqlite3ExprCachePush();
                /* This code must be run in its entirety every time it is encountered
      ** if any of the following is true:
      **
      **    *  The right-hand side is a correlated subquery
      **    *  The right-hand side is an expression list containing variables
      **    *  We are inside a trigger
      **
      ** If all of the above are false, then we can run this code just once
      ** save the results, and reuse the same result on subsequent invocations.
      */
                if (!ExprHasAnyProperty(pExpr, EP_VarSelect) && null == this.pTriggerTab)
                {
                    int mem = ++this.nMem;
                    v.sqlite3VdbeAddOp1(OP_If, mem);
                    testAddr = v.sqlite3VdbeAddOp2(OP_Integer, 1, mem);
                    Debug.Assert(testAddr > 0/* || pParse.db.mallocFailed != 0 */);
                }
#if !SQLITE_OMIT_EXPLAIN
                if (this.explain == 2)
                {
                    string zMsg = sqlite3MPrintf(this.db, "EXECUTE %s%s SUBQUERY %d", testAddr != 0 ? "" : "CORRELATED ", pExpr.Operator == Operator.TK_IN ? "LIST" : "SCALAR", this.iNextSelectId);
                    v.sqlite3VdbeAddOp4(OP_Explain, this.iSelectId, 0, 0, zMsg, P4_DYNAMIC);
                }
#endif
                switch (pExpr.Operator)
                {
                    case Operator.TK_IN:
                        {
                            char affinity;
                            /* Affinity of the LHS of the IN */
                            KeyInfo keyInfo;
                            /* Keyinfo for the generated table */
                            int addr;
                            /* Address of OP_OpenEphemeral instruction */
                            Expr pLeft = pExpr.pLeft;
                            /* the LHS of the IN operator */
                            if (rMayHaveNull != 0)
                            {
                                v.sqlite3VdbeAddOp2(OP_Null, 0, rMayHaveNull);
                            }
                            affinity = pLeft.sqlite3ExprAffinity();
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
                    */
                            pExpr.iTable = this.nTab++;
                            addr = v.sqlite3VdbeAddOp2(OP_OpenEphemeral, (int)pExpr.iTable, !isRowid);
                            if (rMayHaveNull == 0)
                                v.sqlite3VdbeChangeP5(BTREE_UNORDERED);
                            keyInfo = new KeyInfo();
                            // memset( &keyInfo, 0, sizeof(keyInfo ));
                            keyInfo.nField = 1;
                            if (ExprHasProperty(pExpr, EP_xIsSelect))
                            {
                                /* Case 1:     expr IN (SELECT ...)
                      **
                      ** Generate code to write the results of the select into the temporary
                      ** table allocated and opened above.
                      */
                                SelectDest dest = new SelectDest();
                                ExprList pEList;
                                Debug.Assert(!isRowid);
                                sqlite3SelectDestInit(dest, SelectResultType.Set, pExpr.iTable);
                                dest.affinity = (char)affinity;
                                Debug.Assert((pExpr.iTable & 0x0000FFFF) == pExpr.iTable);
                                pExpr.x.pSelect.iLimit = 0;
                                if (sqlite3Select(this, pExpr.x.pSelect, ref dest) != 0)
                                {
                                    return 0;
                                }
                                pEList = pExpr.x.pSelect.pEList;
                                if (ALWAYS(pEList != null) && pEList.nExpr > 0)
                                {
                                    keyInfo.aColl[0] = this.sqlite3BinaryCompareCollSeq(pExpr.pLeft, pEList.a[0].pExpr);
                                }
                            }
                            else
                                if (ALWAYS(pExpr.x.pList != null))
                                {
                                    /* Case 2:     expr IN (exprlist)
                      **
                      ** For each expression, build an index key from the evaluation and
                      ** store it in the temporary table. If <expr> is a column, then use
                      ** that columns affinity when building index keys. If <expr> is not
                      ** a column, use numeric affinity.
                      */
                                    int i;
                                    ExprList pList = pExpr.x.pList;
                                    ExprList_item pItem;
                                    int r1, r2, r3;
                                    if (affinity == '\0')
                                    {
                                        affinity = SQLITE_AFF_NONE;
                                    }
                                    keyInfo.aColl[0] = this.sqlite3ExprCollSeq(pExpr.pLeft);
                                    /* Loop through each expression in <exprlist>. */
                                    r1 = this.sqlite3GetTempReg();
                                    r2 = this.sqlite3GetTempReg();
                                    v.sqlite3VdbeAddOp2(OP_Null, 0, r2);
                                    for (i = 0; i < pList.nExpr; i++)
                                    {
                                        //, pItem++){
                                        pItem = pList.a[i];
                                        Expr pE2 = pItem.pExpr;
                                        int iValToIns = 0;
                                        /* If the expression is not constant then we will need to
                        ** disable the test that was generated above that makes sure
                        ** this code only executes once.  Because for a non-constant
                        ** expression we need to rerun this code each time.
                        */
                                        if (testAddr != 0 && pE2.sqlite3ExprIsConstant() == 0)
                                        {
                                            sqlite3VdbeChangeToNoop(v, testAddr - 1, 2);
                                            testAddr = 0;
                                        }
                                        /* Evaluate the expression and insert it into the temp table */
                                        if (isRowid && pE2.sqlite3ExprIsInteger(ref iValToIns) != 0)
                                        {
                                            v.sqlite3VdbeAddOp3(OP_InsertInt, pExpr.iTable, r2, iValToIns);
                                        }
                                        else
                                        {
                                            r3 = this.sqlite3ExprCodeTarget(pE2, r1);
                                            if (isRowid)
                                            {
                                                v.sqlite3VdbeAddOp2(OP_MustBeInt, r3, v.sqlite3VdbeCurrentAddr() + 2);
                                                v.sqlite3VdbeAddOp3(OP_Insert, pExpr.iTable, r2, r3);
                                            }
                                            else
                                            {
                                                v.sqlite3VdbeAddOp4(OP_MakeRecord, r3, 1, r2, affinity, 1);
                                                this.sqlite3ExprCacheAffinityChange(r3, 1);
                                                v.sqlite3VdbeAddOp2(OP_IdxInsert, pExpr.iTable, r2);
                                            }
                                        }
                                    }
                                    this.sqlite3ReleaseTempReg(r1);
                                    this.sqlite3ReleaseTempReg(r2);
                                }
                            if (!isRowid)
                            {
                                v.sqlite3VdbeChangeP4(addr, keyInfo, P4_KEYINFO);
                            }
                            break;
                        }
                    case Operator.TK_EXISTS:
                    case Operator.TK_SELECT:
                    default:
                        {
                            /* If this has to be a scalar SELECT.  Generate code to put the
                    ** value of this select in a memory cell and record the number
                    ** of the memory cell in iColumn.  If this is an EXISTS, write
                    ** an integer 0 (not exists) or 1 (exists) into a memory cell
                    ** and record that memory cell in iColumn.
                    */
                            Select pSel;
                            /* SELECT statement to encode */
                            SelectDest dest = new SelectDest();
                            /* How to deal with SELECt result */
                            testcase(pExpr.Operator == Operator.TK_EXISTS);
                            testcase(pExpr.Operator == Operator.TK_SELECT);
                            Debug.Assert(pExpr.Operator == Operator.TK_EXISTS || pExpr.Operator == Operator.TK_SELECT);
                            Debug.Assert(ExprHasProperty(pExpr, EP_xIsSelect));
                            pSel = pExpr.x.pSelect;
                            sqlite3SelectDestInit(dest, 0, ++this.nMem);
                            if (pExpr.Operator == Operator.TK_SELECT)
                            {
                                dest.eDest = SelectResultType.Mem;
                                v.sqlite3VdbeAddOp2(OP_Null, 0, dest.iParm);
#if SQLITE_DEBUG
																																																																																																																						              VdbeComment( v, "Init subquery result" );
#endif
                            }
                            else
                            {
                                dest.eDest = SelectResultType.Exists;
                                v.sqlite3VdbeAddOp2(OP_Integer, 0, dest.iParm);
#if SQLITE_DEBUG
																																																																																																																						              VdbeComment( v, "Init EXISTS result" );
#endif
                            }
                            sqlite3ExprDelete(this.db, ref pSel.pLimit);
                            pSel.pLimit = this.sqlite3PExpr(TK_INTEGER, null, null, sqlite3IntTokens[1]);
                            pSel.iLimit = 0;
                            if (sqlite3Select(this, pSel, ref dest) != 0)
                            {
                                return 0;
                            }
                            rReg = dest.iParm;
                            ExprSetIrreducible(pExpr);
                            break;
                        }
                }
                if (testAddr != 0)
                {
                    v.sqlite3VdbeJumpHere(testAddr - 1);
                }
                this.sqlite3ExprCachePop(1);
                return rReg;
            }
            public void sqlite3ExprCodeIN(/* Parsing and code generating context */Expr pExpr,/* The IN expression */int destIfFalse,/* Jump here if LHS is not contained in the RHS */int destIfNull/* Jump here if the results are unknown due to NULLs */)
            {
                int rRhsHasNull = 0;
                /* Register that is true if RHS contains NULL values */
                char affinity;
                /* Comparison affinity to use */
                int eType;
                /* Type of the RHS */
                int r1;
                /* Temporary use register */
                Vdbe v;
                /* Statement under construction */
                /* Compute the RHS.   After this step, the table with cursor
** pExpr.iTable will contains the values that make up the RHS.
*/
                v = this.pVdbe;
                Debug.Assert(v != null);
                /* OOM detected prior to this routine */
                VdbeNoopComment(v, "begin IN expr");
                eType = this.sqlite3FindInIndex(pExpr, ref rRhsHasNull);
                /* Figure out the affinity to use to create a key from the results
      ** of the expression. affinityStr stores a static string suitable for
      ** P4 of OP_MakeRecord.
      */
                affinity = pExpr.comparisonAffinity();
                /* Code the LHS, the <expr> from "<expr> IN (...)".
      */
                this.sqlite3ExprCachePush();
                r1 = this.sqlite3GetTempReg();
                this.sqlite3ExprCode(pExpr.pLeft, r1);
                /* If the LHS is NULL, then the result is either false or NULL depending
      ** on whether the RHS is empty or not, respectively.
      */
                if (destIfNull == destIfFalse)
                {
                    /* Shortcut for the common case where the false and NULL outcomes are
        ** the same. */
                    v.sqlite3VdbeAddOp2(OP_IsNull, r1, destIfNull);
                }
                else
                {
                    int addr1 = v.sqlite3VdbeAddOp1(OP_NotNull, r1);
                    v.sqlite3VdbeAddOp2(OP_Rewind, pExpr.iTable, destIfFalse);
                    v.sqlite3VdbeAddOp2(OP_Goto, 0, destIfNull);
                    v.sqlite3VdbeJumpHere(addr1);
                }
                if (eType == IN_INDEX_ROWID)
                {
                    /* In this case, the RHS is the ROWID of table b-tree
        */
                    v.sqlite3VdbeAddOp2(OP_MustBeInt, r1, destIfFalse);
                    v.sqlite3VdbeAddOp3(OP_NotExists, pExpr.iTable, destIfFalse, r1);
                }
                else
                {
                    /* In this case, the RHS is an index b-tree.
        */
                    v.sqlite3VdbeAddOp4(OP_Affinity, r1, 1, 0, affinity, 1);
                    /* If the set membership test fails, then the result of the 
        ** "x IN (...)" expression must be either 0 or NULL. If the set
        ** contains no NULL values, then the result is 0. If the set 
        ** contains one or more NULL values, then the result of the
        ** expression is also NULL.
        */
                    if (rRhsHasNull == 0 || destIfFalse == destIfNull)
                    {
                        /* This branch runs if it is known at compile time that the RHS
          ** cannot contain NULL values. This happens as the result
          ** of a "NOT NULL" constraint in the database schema.
          **
          ** Also run this branch if NULL is equivalent to FALSE
          ** for this particular IN operator.
          */
                        v.sqlite3VdbeAddOp4Int(OP_NotFound, pExpr.iTable, destIfFalse, r1, 1);
                    }
                    else
                    {
                        /* In this branch, the RHS of the IN might contain a NULL and
          ** the presence of a NULL on the RHS makes a difference in the
          ** outcome.
          */
                        int j1, j2, j3;
                        /* First check to see if the LHS is contained in the RHS.  If so,
          ** then the presence of NULLs in the RHS does not matter, so jump
          ** over all of the code that follows.
          */
                        j1 = v.sqlite3VdbeAddOp4Int(OP_Found, pExpr.iTable, 0, r1, 1);
                        /* Here we begin generating code that runs if the LHS is not
          ** contained within the RHS.  Generate additional code that
          ** tests the RHS for NULLs.  If the RHS contains a NULL then
          ** jump to destIfNull.  If there are no NULLs in the RHS then
          ** jump to destIfFalse.
          */
                        j2 = v.sqlite3VdbeAddOp1(OP_NotNull, rRhsHasNull);
                        j3 = v.sqlite3VdbeAddOp4Int(OP_Found, pExpr.iTable, 0, rRhsHasNull, 1);
                        v.sqlite3VdbeAddOp2(OP_Integer, -1, rRhsHasNull);
                        v.sqlite3VdbeJumpHere(j3);
                        v.sqlite3VdbeAddOp2(OP_AddImm, rRhsHasNull, 1);
                        v.sqlite3VdbeJumpHere(j2);
                        /* Jump to the appropriate target depending on whether or not
          ** the RHS contains a NULL
          */
                        v.sqlite3VdbeAddOp2(OP_If, rRhsHasNull, destIfNull);
                        v.sqlite3VdbeAddOp2(OP_Goto, 0, destIfFalse);
                        /* The OP_Found at the top of this branch jumps here when true, 
          ** causing the overall IN expression evaluation to fall through.
          */
                        v.sqlite3VdbeJumpHere(j1);
                    }
                }
                this.sqlite3ReleaseTempReg(r1);
                this.sqlite3ExprCachePop(1);
                VdbeComment(v, "end IN expr");
            }
            public void codeInteger(Expr pExpr, bool negFlag, int iMem)
            {
                Vdbe v = this.pVdbe;
                if ((pExpr.flags & EP_IntValue) != 0)
                {
                    int i = pExpr.u.iValue;
                    Debug.Assert(i >= 0);
                    if (negFlag)
                        i = -i;
                    v.sqlite3VdbeAddOp2(OP_Integer, i, iMem);
                }
                else
                {
                    int c;
                    i64 value = 0;
                    string z = pExpr.u.zToken;
                    Debug.Assert(!String.IsNullOrEmpty(z));
                    c = Converter.sqlite3Atoi64(z, ref value, StringExtensions.sqlite3Strlen30(z), SqliteEncoding.UTF8);
                    if (c == 0 || (c == 2 && negFlag))
                    {
                        //char* zV;
                        if (negFlag)
                        {
                            value = c == 2 ? IntegerExtensions.SMALLEST_INT64 : -value;
                        }
                        v.sqlite3VdbeAddOp4(OP_Int64, 0, iMem, 0, value, P4_INT64);
                    }
                    else
                    {
#if SQLITE_OMIT_FLOATING_POINT
																																																																																																																						sqlite3ErrorMsg(pParse, "oversized integer: %s%s", negFlag ? "-" : "", z);
#else
                        codeReal(v, z, negFlag, iMem);
#endif
                    }
                }
            }
            public void cacheEntryClear(yColCache p)
            {
                if (p.tempReg != 0)
                {
                    if (this.nTempReg < ArraySize(this.aTempReg))
                    {
                        this.aTempReg[this.nTempReg++] = p.iReg;
                    }
                    p.tempReg = 0;
                }
            }
            public void sqlite3VtabBeginParse(/* Parsing context */Token pName1,/* Name of new table, or database name */Token pName2,/* Name of new table or NULL */Token pModuleName/* Name of the module for the virtual table */)
            {
                int iDb;
                /* The database the table is being created in */
                Table pTable;
                /* The new virtual table */
                sqlite3 db;
                /* Database connection */
                sqlite3StartTable(this, pName1, pName2, 0, 0, 1, 0);
                pTable = this.pNewTable;
                if (pTable == null)
                    return;
                Debug.Assert(null == pTable.pIndex);
                db = this.db;
                iDb = sqlite3SchemaToIndex(db, pTable.pSchema);
                Debug.Assert(iDb >= 0);
                pTable.tabFlags |= TF_Virtual;
                pTable.nModuleArg = 0;
                addModuleArgument(db, pTable, sqlite3NameFromToken(db, pModuleName));
                addModuleArgument(db, pTable, db.aDb[iDb].zName);
                //sqlite3DbStrDup( db, db.aDb[iDb].zName ) );
                addModuleArgument(db, pTable, pTable.zName);
                //sqlite3DbStrDup( db, pTable.zName ) );
                this.sNameToken.Length = this.sNameToken.zRestSql.Length;
                //      (int)[pModuleName.n] - pName1.z );
#if !SQLITE_OMIT_AUTHORIZATION
																																																																			  /* Creating a virtual table invokes the authorization callback twice.
  ** The first invocation, to obtain permission to INSERT a row into the
  ** sqlite_master table, has already been made by sqlite3StartTable().
  ** The second call, to obtain permission to create the table, is made now.
  */
  if( pTable->azModuleArg ){
    sqlite3AuthCheck(pParse, SQLITE_CREATE_VTABLE, pTable->zName, 
            pTable->azModuleArg[0], pParse->db->aDb[iDb].zName);
  }
#endif
            }
            public void addArgumentToVtab()
            {
                if (this.sArg.zRestSql != null && ALWAYS(this.pNewTable))
                {
                    string z = this.sArg.zRestSql.Substring(0, this.sArg.Length);
                    int n = this.sArg.Length;
                    sqlite3 db = this.db;
                    addModuleArgument(db, this.pNewTable, z);
                    ///sqlite3DbStrNDup( db, z, n ) );
                }
            }
            public void sqlite3VtabFinishParse(Token pEnd)
            {
                Table pTab = this.pNewTable;
                /* The table being constructed */
                sqlite3 db = this.db;
                /* The database connection */
                if (pTab == null)
                    return;
                this.addArgumentToVtab();
                this.sArg.zRestSql = "";
                if (pTab.nModuleArg < 1)
                    return;
                /* If the CREATE VIRTUAL TABLE statement is being entered for the
      ** first time (in other words if the virtual table is actually being
      ** created now instead of just being read out of sqlite_master) then
      ** do additional initialization work and store the statement text
      ** in the sqlite_master table.
      */
                if (0 == db.init.busy)
                {
                    string zStmt;
                    string zWhere;
                    int iDb;
                    Vdbe v;
                    /* Compute the complete text of the CREATE VIRTUAL TABLE statement */
                    if (pEnd != null)
                    {
                        this.sNameToken.Length = this.sNameToken.zRestSql.Length;
                        //(int)( pEnd.z - pParse.sNameToken.z ) + pEnd.n;
                    }
                    zStmt = sqlite3MPrintf(db, "CREATE VIRTUAL TABLE %T", this.sNameToken.zRestSql.Substring(0, this.sNameToken.Length));
                    /* A slot for the record has already been allocated in the 
        ** SQLITE_MASTER table.  We just need to update that slot with all
        ** the information we've collected.  
        **
        ** The VM register number pParse.regRowid holds the rowid of an
        ** entry in the sqlite_master table tht was created for this vtab
        ** by sqlite3StartTable().
        */
                    iDb = sqlite3SchemaToIndex(db, pTab.pSchema);
                    sqlite3NestedParse(this, "UPDATE %Q.%s " + "SET type='table', name=%Q, tbl_name=%Q, rootpage=0, sql=%Q " + "WHERE rowid=#%d", db.aDb[iDb].zName, SCHEMA_TABLE(iDb), pTab.zName, pTab.zName, zStmt, this.regRowid);
                    db.sqlite3DbFree(ref zStmt);
                    v = sqlite3GetVdbe(this);
                    sqlite3ChangeCookie(this, iDb);
                    v.sqlite3VdbeAddOp2(OP_Expire, 0, 0);
                    zWhere = sqlite3MPrintf(db, "name='%q' AND type='table'", pTab.zName);
                    sqlite3VdbeAddParseSchemaOp(v, iDb, zWhere);
                    v.sqlite3VdbeAddOp4(OP_VCreate, iDb, 0, 0, pTab.zName, StringExtensions.sqlite3Strlen30(pTab.zName) + 1);
                }
                /* If we are rereading the sqlite_master table create the in-memory
      ** record of the table. The xConnect() method is not called until
      ** the first time the virtual table is used in an SQL statement. This
      ** allows a schema that contains virtual tables to be loaded before
      ** the required virtual table implementations are registered.  */else
                {
                    Table pOld;
                    Schema pSchema = pTab.pSchema;
                    string zName = pTab.zName;
                    int nName = StringExtensions.sqlite3Strlen30(zName);
                    Debug.Assert(sqlite3SchemaMutexHeld(db, 0, pSchema));
                    pOld = sqlite3HashInsert(ref pSchema.tblHash, zName, nName, pTab);
                    if (pOld != null)
                    {
                        //db.mallocFailed = 1;
                        Debug.Assert(pTab == pOld);
                        /* Malloc must have failed inside HashInsert() */
                        return;
                    }
                    this.pNewTable = null;
                }
            }
            public void sqlite3VtabArgInit()
            {
                this.addArgumentToVtab();
                this.sArg.zRestSql = null;
                this.sArg.Length = 0;
            }
            public void sqlite3VtabArgExtend(Token p)
            {
                Token pArg = this.sArg;
                if (pArg.zRestSql == null)
                {
                    pArg.zRestSql = p.zRestSql;
                    pArg.Length = p.Length;
                }
                else
                {
                    //Debug.Assert( pArg.z< p.z );
                    pArg.Length += p.Length + 1;
                    //(int)( p.z[p.n] - pArg.z );
                }
            }
            public int sqlite3VtabCallConnect(Table pTab)
            {
                sqlite3 db = this.db;
                string zMod;
                Module pMod;
                int rc;
                Debug.Assert(pTab != null);
                if ((pTab.tabFlags & TF_Virtual) == 0 || sqlite3GetVTable(db, pTab) != null)
                {
                    return SQLITE_OK;
                }
                /* Locate the required virtual table module */
                zMod = pTab.azModuleArg[0];
                pMod = (Module)sqlite3HashFind(db.aModule, zMod, StringExtensions.sqlite3Strlen30(zMod), (Module)null);
                if (null == pMod)
                {
                    string zModule = pTab.azModuleArg[0];
                    sqlite3ErrorMsg(this, "no such module: %s", zModule);
                    rc = SQLITE_ERROR;
                }
                else
                {
                    string zErr = null;
                    rc = vtabCallConstructor(db, pTab, pMod, pMod.pModule.xConnect, ref zErr);
                    if (rc != SQLITE_OK)
                    {
                        sqlite3ErrorMsg(this, "%s", zErr);
                    }
                    zErr = null;
                    //sqlite3DbFree( db, zErr );
                }
                return rc;
            }
            public void sqlite3VtabMakeWritable(Table pTab)
            {
                Parse pToplevel = sqlite3ParseToplevel(this);
                int i, n;
                //Table[] apVtabLock = null;
                Debug.Assert(IsVirtual(pTab));
                for (i = 0; i < pToplevel.nVtabLock; i++)
                {
                    if (pTab == pToplevel.apVtabLock[i])
                        return;
                }
                n = pToplevel.apVtabLock == null ? 1 : pToplevel.apVtabLock.Length + 1;
                //(pToplevel.nVtabLock+1)*sizeof(pToplevel.apVtabLock[0]);
                //sqlite3_realloc( pToplevel.apVtabLock, n );
                //if ( apVtabLock != null )
                {
                    Array.Resize(ref pToplevel.apVtabLock, n);
                    // pToplevel.apVtabLock= apVtabLock;
                    pToplevel.apVtabLock[pToplevel.nVtabLock++] = pTab;
                    //else
                    //{
                    //  pToplevel.db.mallocFailed = 1;
                    //}
                }
            }
        }
		
	}















}
