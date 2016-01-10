///
///<summary>
///
///</summary>
///<param name="Included in SQLite3 port to C#">SQLite;  2008 Noah B Hart</param>
///<param name="C#">SQLite is an independent reimplementation of the SQLite software library</param>
///<param name=""></param>
///<param name="SQLITE_SOURCE_ID: 2010">23 18:52:01 42537b60566f288167f1b5864a5435986838e3a3</param>
///<param name=""></param>
///<param name=""></param>
namespace Community.CsharpSqlite {
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
#if SQLITE_MAX_ATTACHED
				//  typedef sqlite3_uint64 yDbMask;
using yDbMask = System.Int64; 
#else
    //  typedef unsigned int yDbMask;
    using yDbMask = System.Int32;
    using System.Collections.Generic;
    using Community.CsharpSqlite.Engine;
    using Community.CsharpSqlite.Ast;
    using Community.CsharpSqlite.Parsing;
    using Community.CsharpSqlite.builder;
    using Community.CsharpSqlite.Metadata;
    using Community.CsharpSqlite.Os;
    using Community.CsharpSqlite.tree;
    using Community.CsharpSqlite.Utils;
    using Community.CsharpSqlite.Metadata.Traverse;
    using Compiler;
    using Compiler.CodeGeneration;
    using Compiler.Parser;
    using Compiler.CodeGen;
#endif
    public partial class Sqlite3 {
        public class Parse : VdbeFacade {

            public SqlResult rc;
            ///
            ///<summary>
            ///Return code from execution 
            ///</summary>
            public string zErrMsg;
            ///
            ///<summary>
            ///An error message 
            ///</summary>

            public u8 colNamesSet;
            ///
            ///<summary>
            ///TRUE after  OpCode.Op_ColumnName has been issued to pVdbe 
            ///</summary>
            public u8 nameClash;
            ///
            ///<summary>
            ///A permanent table name clashes with temp table name 
            ///</summary>
            public u8 checkSchema;
            ///
            ///<summary>
            ///Causes schema cookie check after an error 
            ///</summary>
            public u8 nested;
            ///
            ///<summary>
            ///Number of nested calls to the parser/code generator 
            ///</summary>
            public u8 parseError;
            ///
            ///<summary>
            ///True after a parsing error.  Ticket #1794 
            ///</summary>

            u8 _nTempInUse;
            public u8 nTempInUse {
                get {
                    return _nTempInUse;
                }
                set {
                    _nTempInUse = value;
                }
            }
            ///
            ///<summary>
            ///Number of aTempReg[] currently checked out 
            ///</summary>


            public int nErr;
            ///
            ///<summary>
            ///Number of errors seen 
            ///</summary>
            public int nTab;
            ///
            ///<summary>
            ///Number of previously allocated VDBE cursors 
            ///</summary>

            public int nSet;
            ///
            ///<summary>
            ///Number of sets used so far 
            ///</summary>
            public int ckBase;
            ///
            ///<summary>
            ///Base register of data during check constraints 
            ///</summary>

            public int iCacheCnt;
            ///
            ///<summary>
            ///Counter used to generate aColCache[].lru values 
            ///</summary>
            public u8 nColCache;
            ///
            ///<summary>
            ///Number of entries in the column cache 
            ///</summary>
            public u8 iColCache;
            ///
            ///<summary>
            ///Next entry of the cache to replace 
            ///</summary>

            public yDbMask writeMask;
            ///
            ///<summary>
            ///Start a write transaction on these databases 
            ///</summary>
            public yDbMask cookieMask;
            ///
            ///<summary>
            ///Bitmask of schema verified databases 
            ///</summary>
            public u8 isMultiWrite;
            ///
            ///<summary>
            ///True if statement may affect/insert multiple rows 
            ///</summary>
            public u8 mayAbort;
            ///
            ///<summary>
            ///True if statement may throw an ABORT exception 
            ///</summary>
            public int cookieGoto;
            ///
            ///<summary>
            ///Address of OpCode.OP_Goto to cookie verifier subroutine 
            ///</summary>
            public int[] cookieValue;
            ///
            ///<summary>
            ///Values of cookies to verify 
            ///</summary>
#if !SQLITE_OMIT_SHARED_CACHE
																																																																																																public int nTableLock;         /* Number of locks in aTableLock */
public TableLock[] aTableLock; /* Required table locks for shared-cache mode */
#endif
            ///<summary>
            ///Register holding rowid of CREATE TABLE entry 
            ///</summary>
            public int regRowid;
            ///
            ///<summary>
            ///Register holding root page number for new objects 
            ///</summary>
            public int regRoot;
            ///
            ///<summary>
            ///Information about AUTOINCREMENT counters 
            ///</summary>
            public AutoincInfo pAinc;
            ///<summary>
            ///</summary>
            ///<param name="Max args passed to user function by sub">program </param>
            ///
            ///<summary>
            ///Information used while coding trigger programs. 
            ///</summary>

            public int nMaxArg;
            ///
            ///<summary>
            ///Parse structure for main program (or NULL) 
            ///</summary>
            public Parse pToplevel;
            ///
            ///<summary>
            ///Table triggers are being coded for 
            ///</summary>
            public Table pTriggerTab;
            ///
            ///<summary>
            ///Mask of old.* columns referenced 
            ///</summary>
            public u32 oldmask;
            ///
            ///<summary>
            ///Mask of new.* columns referenced 
            ///</summary>
            public u32 newmask;
            ///<summary>
            ///TokenType.TK_UPDATE, TokenType.TK_INSERT or TokenType.TK_DELETE 
            ///</summary>

            public u8 eTriggerOp;
            public TokenType eTriggerOperator
            {
                get
                {
                    return (TokenType)eTriggerOp;
                }
            }

            ///
            ///<summary>
            ///Default ON CONFLICT policy for trigger steps 
            ///</summary>
            public OnConstraintError eOrconf;
            ///

            public u8 disableTriggers;
            ///
            ///<summary>
            ///True to disable triggers 
            ///</summary>
            public double nQueryLoop;
            ///
            ///<summary>
            ///Estimated number of iterations of a query 
            ///</summary>
            ///
            ///<summary>
            ///Above is constant between recursions.  Below is reset before and after
            ///each recursion 
            ///</summary>
            public int nVar;
            ///
            ///<summary>
            ///Number of '?' variables seen in the SQL so far 
            ///</summary>
            public int nzVar {
                get { return azVar.Count; }
            }
            ///<summary>
            ///Number of available slots in azVar[] 
            ///</summary>
            public MyCollection<string> azVar = new MyCollection<string>();
            ///
            ///<summary>
            ///Pointers to names of parameters 
            ///</summary>


            ///<summary>
            ///VM being reprepared (sqlite3Reprepare()) 
            ///</summary>
            public Vdbe pReprepare;
            ///<summary>
            ///Number of aliased result set columns 
            ///</summary>
            public int nAlias;
            ///<summary>
            ///Number of allocated slots for aAlias[] 
            ///</summary>
            public int nAliasAlloc;
            ///<summary>
            ///Register used to hold aliased result 
            ///</summary>
            public int[] aAlias;
            ///<summary>
            ///True if the EXPLAIN flag is found on the query 
            ///</summary>
            public u8 explain;
            Token _sNameToken;
            ///<summary>
            ///Token with unqualified schema object name 
            ///</summary>
            public Token sNameToken
            {
                get {
                    return _sNameToken;
                }
                set {
                    _sNameToken = value;
                    if (null != _sNameToken)
                        Log.WriteHeader("Parse name : " + _sNameToken.Text);
                }
            }
            ///<summary>
            ///The last token parsed 
            ///</summary>
            public Token sLastToken;
            ///<summary>
            ///All SQL text past the last semicolon parsed 
            ///</summary>
            StringBuilder m_zTail = new StringBuilder();
            public StringBuilder zTail
            {
                get { return m_zTail; }
                set { m_zTail = value; }
            }



            ///
            ///<summary>
            ///A table being constructed by CREATE TABLE 
            ///</summary>
            public Table pNewTable;


            ///<summary>
            ///Trigger under construct by a CREATE TRIGGER 
            ///</summary>
            public Trigger pNewTrigger;

            ///<summary>
            ///The 6th parameter to db.xAuth callbacks 
            ///</summary>
            public string zAuthContext;

#if !SQLITE_OMIT_VIRTUALTABLE
            ///
            ///<summary>
            ///Complete text of a module argument 
            ///</summary>
            public Token sArg;



            ///<summary>
            ///True if inside sqlite3_declare_vtab() 
            ///</summary>
            public u8 declareVtab;
            

            ///<summary>
            ///Number of virtual tables to lock 
            ///</summary>
            public int nVtabLock;
            


            ///<summary>
            ///Pointer to virtual tables needing locking 
            ///</summary>
            public Table[] apVtabLock;
            
#endif

            ///<param name="Expression tree height of current sub">select </param>
            public int nHeight;
            
            ///<summary>
            ///List of Table objects to delete after code gen 
            ///</summary>
            public Table pZombieTab;
            


            ///<summary>
            ///Linked list of coded triggers
            ///</summary>
            public TriggerPrg pTriggerPrg;
            
#if !SQLITE_OMIT_EXPLAIN
            public int iSelectId;
            public int iNextSelectId;
#endif
            // We need to create instances of the col cache
            public Parse() {
                aTempReg = new int[8];
                ///Holding area for temporary registers 
                aColCache = new sqliteinth.yColCache[sqliteinth.SQLITE_N_COLCACHE];
                ///One for each valid column cache entry 
                for (int i = 0; i < this.aColCache.Length; i++) {
                    this.aColCache[i] = new sqliteinth.yColCache();
                }
                cookieValue = new int[Limits.SQLITE_MAX_ATTACHED + 2];
                ///Values of cookies to verify 
                sLastToken = new Token();
                ///The last token parsed 
#if !SQLITE_OMIT_VIRTUALTABLE
                sArg = new Token();
#endif
            }
            public void ResetMembers()// Need to clear all the following variables during each recursion
            {
                nVar = 0;
                azVar = new MyCollection<string>();
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
                if (SaveBuf[nested] != null) {
                    nVar = SaveBuf[nested].nVar;
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
///<param name="is parsed (i.e. build.sqlite3AddColumn()) add the new Column data to">is parsed (i.e. build.sqlite3AddColumn()) add the new Column data to</param>
///<param name="the copy. The copy of the Table structure is deleted by tokenize.c">the copy. The copy of the Table structure is deleted by tokenize.c</param>
///<param name="after parsing is finished.">after parsing is finished.</param>
///<param name=""></param>
///<param name="Routine sqlite3AlterFinishAddColumn() will be called to complete">Routine sqlite3AlterFinishAddColumn() will be called to complete</param>
///<param name="coding the "ALTER TABLE ... ADD" statement.">coding the "ALTER TABLE ... ADD" statement.</param>
*/
            public void sqlite3AlterBeginAddColumn(SrcList pSrc) {
                Connection db = this.db;
                ///Look up the table being altered. 
                Debug.Assert(this.pNewTable == null);
                Debug.Assert(sqlite3BtreeHoldsAllMutexes(db));
                //      if ( db.mallocFailed != 0 ) goto exit_begin_add_column;
                var pTab = TableBuilder.sqlite3LocateTable(this, pSrc.a[0].zName, pSrc.a[0].zDatabase);
                if (pTab == null)
                    goto exit_begin_add_column;
                if (pTab.IsVirtual()) {
                    utilc.sqlite3ErrorMsg(this, "virtual tables may not be altered");
                    goto exit_begin_add_column;
                }
                ///Make sure this is not an attempt to ALTER a view. 
                if (pTab.IsView) {
                    utilc.sqlite3ErrorMsg(this, "Cannot add a column to a view");
                    goto exit_begin_add_column;
                }
                if (SqlResult.SQLITE_OK != this.isSystemTable(pTab.zName)) {
                    goto exit_begin_add_column;
                }
                Debug.Assert(pTab.addColOffset > 0);
                var iDb = db.indexOfBackendWithSchema(pTab.pSchema);
                ///Put a copy of the Table struct in Parse.pNewTable for the
                ///build.sqlite3AddColumn() function and friends to modify.  But modify
                ///the name by adding an "sqlite_altertab_" prefix.  By adding this
                ///prefix, we insure that the name will not collide with an existing
                ///table because user table are not allowed to have the "sqlite_"
                ///prefix on their name.
                var nAlloc = (((pTab.nCol - 1) / 8) * 8) + 8;
                Debug.Assert(nAlloc >= pTab.nCol && nAlloc % 8 == 0 && nAlloc - pTab.nCol < 8);

                var pNew = this.pNewTable = new Table()
                {
                    nRef = 1,
                    nCol = pTab.nCol,
                    zName = io.sqlite3MPrintf(db, "sqlite_altertab_%s", pTab.zName),
                    addColOffset = pTab.addColOffset,
                    pSchema = db.Backends[iDb].pSchema,
                    aCol = pTab.aCol.Select(x => {
                        var pCol = x.Copy();
                        pCol.zColl = null;
                        pCol.zType = null;
                        pCol.DefaultValue = null;
                        pCol.DefaultValueSource = null;
                        return pCol;
                    }).ToArray()
                };
                // (Table*)sqlite3DbMallocZero( db, sizeof(Table))
                //if(pNew==null)
                //	goto exit_begin_add_column;
                Debug.Assert(pNew.nCol > 0);
                // (Column*)sqlite3DbMallocZero( db, sizeof(Column) * nAlloc );
                if (pNew.aCol == null || pNew.zName == null) {
                    //        db.mallocFailed = 1;
                    goto exit_begin_add_column;
                }
                // memcpy( pNew.aCol, pTab.aCol, sizeof(Column) * pNew.nCol );
                ///Begin a transaction and increment the schema cookie.  
                build.sqlite3BeginWriteOperation(this, 0, iDb);

                if (null == this.sqlite3GetVdbe())
                    goto exit_begin_add_column;
                build.codegenChangeCookie(this, iDb);

                exit_begin_add_column://<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<
                build.sqlite3SrcListDelete(db, ref pSrc);
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
          void sqlite3AlterFinishAddColumn(Token pColDef) {
                ///Null-terminated column definitior
                ///The new column 
                ///Default value for the new column 				
                var db = this.db;///The database connection; 
				if (this.nErr != 0///
                                  ///|| db.mallocFailed != 0 
              )
                    return;
                var pNew = this.pNewTable;///Copy of pParse.pNewTable 
				Debug.Assert(pNew != null);
                Debug.Assert(sqlite3BtreeHoldsAllMutexes(db));
                var iDb = db.indexOfBackendWithSchema(pNew.pSchema);///Database number 
				var zDb = db.Backends[iDb].Name;///Database name 
				var zTab = pNew.zName.Substring(16);///Table name 
                // zTab = &pNew->zName[16]; /* Skip the "sqlite_altertab_" prefix on the name */
                var pCol = pNew.aCol[pNew.nCol - 1];
                var pDflt = pCol.DefaultValue;
                var pTab = TableBuilder.sqlite3FindTable(db, zDb, zTab);///Table being altered 
				Debug.Assert(pTab != null);
#if !SQLITE_OMIT_AUTHORIZATION
																																																																																																																																/* Invoke the authorization callback. */
if( sqlite3AuthCheck(pParse, SQLITE_ALTER_TABLE, zDb, pTab.zName, 0) ){
return;
}
#endif
                ///If the default value for the new column was specified with a
                ///literal NULL, then set pDflt to 0. This simplifies checking
                ///for an SQL NULL default below.
                if (pDflt != null && pDflt.Operator == TokenType.TK_NULL) {
                    pDflt = null;
                }
                ///Check that the new column is not specified as PRIMARY KEY or UNIQUE.
                ///If there is a NOT NULL constraint, then the default value for the
                ///column must not be NULL.
                if (pCol.isPrimKey != 0) {
                    utilc.sqlite3ErrorMsg(this, "Cannot add a PRIMARY KEY column");
                    return;
                }
                if (pNew.pIndex != null) {
                    utilc.sqlite3ErrorMsg(this, "Cannot add a UNIQUE column");
                    return;
                }
                if ((db.flags & SqliteFlags.SQLITE_ForeignKeys) != 0 && pNew.pFKey != null && pDflt != null)
                {
                    utilc.sqlite3ErrorMsg(this, "Cannot add a REFERENCES column with non-NULL default value");
                    return;
                }
                if (pCol.notNull != 0 && pDflt == null) {
                    utilc.sqlite3ErrorMsg(this, "Cannot add a NOT NULL column with default value NULL");
                    return;
                }
                ///Ensure the default expression is something that sqlite3ValueFromExpr()
                ///can handle (i.e. not CURRENT_TIME etc.)
                if (pDflt != null) {
                    sqlite3_value pVal = null;
                    if (vdbemem_cs.sqlite3ValueFromExpr(db, pDflt, SqliteEncoding.UTF8, sqliteinth.SQLITE_AFF_NONE, ref pVal) != 0)
                    {
                        //        db.mallocFailed = 1;
                        return;
                    }
                    if (pVal == null) {
                        utilc.sqlite3ErrorMsg(this, "Cannot add a column with non-constant default");
                        return;
                    }
                    vdbemem_cs.sqlite3ValueFree(ref pVal);
                }
                ///Modify the CREATE TABLE statement. 
                var zCol = pColDef.zRestSql.Substring(0, pColDef.Length).Replace(";", " ").Trim();
                //sqlite3DbStrNDup(db, (char*)pColDef.z, pColDef.n);
                if (zCol != null) {
                    //  char zEnd = zCol[pColDef.n-1];
                    SqliteFlags savedDbFlags = db.flags;
                    //      while( zEnd>zCol && (*zEnd==';' || CharExtensions.sqlite3Isspace(*zEnd)) ){
                    //    zEnd-- = '\0';
                    //  }
                    db.flags |= SqliteFlags.SQLITE_PreferBuiltin;
                    build.sqlite3NestedParse(this, "UPDATE \"%w\".%s SET " + "sql = substr(sql,1,%d) || ', ' || %Q || substr(sql,%d) " + "WHERE type = 'table' AND name = %Q", zDb, sqliteinth.SCHEMA_TABLE(iDb), pNew.addColOffset, zCol, pNew.addColOffset + 1, zTab);
                    db.DbFree(ref zCol);
                    db.flags = savedDbFlags;
                }
                ///If the default value of the new column is NULL, then set the file
                ///format to 2. If the default value of the new column is not NULL,
                ///the file format becomes 3.
                this.codegenMinimumFileFormat(iDb, pDflt != null ? 3 : 2);
                ///Reload the schema of the modified table. 
                this.reloadTableSchema(pTab, pTab.zName);
            }



            ///<summary>
            /// Parameter zName is the name of a table that is about to be altered
            /// (either with ALTER TABLE ... RENAME TO or ALTER TABLE ... ADD COLUMN).
            /// If the table is a system table, this function leaves an error message
            /// in pParse->zErr (system tables may not be altered) and returns non-zero.
            ///
            /// Or, if zName is not a system table, zero is returned.
            ///</summary>
            public SqlResult isSystemTable(string zName) {
                if (zName.StartsWith("sqlite_", System.StringComparison.InvariantCultureIgnoreCase)) {
                    utilc.sqlite3ErrorMsg(this, "table %s may not be altered", zName);
                    return (SqlResult)1;
                }
                return 0;
            }


            ///<summary>
            /// Generate code to implement the "ALTER TABLE xxx RENAME TO yyy"
            /// command.
            ///</summary>
            public void sqlite3AlterRenameTable(
                SrcList pSrc,///The table to rename. 
			    Token tknName///The new table name. 
			) {
                Connection db = this.db;///Database connection 
                String NewNameOfTheTable = String.Empty;
#if !SQLITE_OMIT_TRIGGER
                string zWhere = "";///Where clause to locate temp triggers 
#endif
                VTable pVTab = null;
                ///Non-tab with an xRename()                 
                var savedDbFlags = db.flags;///Saved value of db>flags
                //if ( NEVER( db.mallocFailed != 0 ) ) goto exit_rename_table;
                Debug.Assert(pSrc.Count == 1);
                Debug.Assert(sqlite3BtreeHoldsAllMutexes(this.db));
                var pTab = TableBuilder.sqlite3LocateTable(this, pSrc.a[0].zName, pSrc.a[0].zDatabase);///Table being renamed 
				if (pTab == null)
                    goto exit_rename_table;
                var iDb = this.db.indexOfBackendWithSchema(pTab.pSchema);///Database that contains the table 
				var zDb = db.Backends[iDb].Name;///Name of database iDb 
                db.flags |= SqliteFlags.SQLITE_PreferBuiltin;
                ///Get a NULL terminated version of the new table name. 
                //RULE
                
                NewNameOfTheTable = build.Token2Name(db, tknName);///<param name="NULL">terminated version of pName </param>

                if (NewNameOfTheTable == null)
                    goto exit_rename_table;
                ///Check that a table or index named 'zName' does not already exist
                ///in database iDb. If so, this is an error.
                //RULE
                if (TableBuilder.sqlite3FindTable(db, zDb, NewNameOfTheTable) != null || IndexBuilder.sqlite3FindIndex(db, NewNameOfTheTable, zDb) != null) {
                    utilc.sqlite3ErrorMsg(this, "there is already another table or index with this name: %s", NewNameOfTheTable);
                    goto exit_rename_table;
                }
                ///Make sure it is not a system table being altered, or a reserved name
                ///that the table is being renamed to.
                //RULE
                if (SqlResult.SQLITE_OK != this.isSystemTable(pTab.zName)) {
                    goto exit_rename_table;
                }
                //RULE
                if (SqlResult.SQLITE_OK != build.sqlite3CheckObjectName(this, NewNameOfTheTable)) {
                    goto exit_rename_table;
                }
                //RULE
#if !SQLITE_OMIT_VIEW
                if (pTab.pSelect != null) {
                    utilc.sqlite3ErrorMsg(this, "view %s may not be altered", pTab.zName);
                    goto exit_rename_table;
                }
#endif
#if !SQLITE_OMIT_AUTHORIZATION
																																																																																																																																/* Invoke the authorization callback. */
if( sqlite3AuthCheck(pParse, SQLITE_ALTER_TABLE, zDb, pTab.zName, 0) ){
goto exit_rename_table;
}
#endif
                //RULE
                if (build.sqlite3ViewGetColumnNames(this, pTab) != 0) {
                    goto exit_rename_table;
                }
#if !SQLITE_OMIT_VIRTUALTABLE
                if (pTab.IsVirtual()) {
                    pVTab = VTableMethodsExtensions.sqlite3GetVTable(db, pTab);
                    if (pVTab.pVtab.pModule.xRename == null) {
                        pVTab = null;
                    }
                }
#endif
                ///Begin a transaction and code the VerifyCookie for database iDb.
                ///Then modify the schema cookie (since the ALTER TABLE modifies the
                ///schema). Open a statement transaction if the table is a virtual
                ///table.
                //RULE
                var v = this.sqlite3GetVdbe();
                if (v == null) {
                    goto exit_rename_table;
                }
                build.sqlite3BeginWriteOperation(this, pVTab != null ? 1 : 0, iDb);
                build.codegenChangeCookie(this, iDb);
                ///If this is a virtual table, invoke the xRename() function if
                ///one is defined. The xRename() callback will modify the names
                ///<param name="of any resources used by the v">table implementation (including other</param>
                ///<param name="SQLite tables) that are identified by the name of the virtual table.">SQLite tables) that are identified by the name of the virtual table.</param>
                ///<param name=""></param>
#if !SQLITE_OMIT_VIRTUALTABLE
                if (pVTab != null) {
                    var cellRef = ++this.UsedCellCount;
                    v.sqlite3VdbeAddOp4(OpCode.OP_String8, 0, cellRef, 0, NewNameOfTheTable, 0);
                    v.sqlite3VdbeAddOp4(OpCode.OP_VRename, cellRef, 0, 0, pVTab, P4Usage.P4_VTAB);
                    build.sqlite3MayAbort(this);
                }
#endif
                ///<param name="figure out how many UTF">8 characters are in zName </param>
                var zTabName = pTab.zName;///Original name of the table zTabName=pTab.zName;
                var nTabName = sqlite3Utf8CharLen(zTabName, -1);///<param name="Number of UTF">8 characters in zTabName </param>
#if !(SQLITE_OMIT_FOREIGN_KEY) && !(SQLITE_OMIT_TRIGGER)
                if ((db.flags & SqliteFlags.SQLITE_ForeignKeys) != 0)
                {
                    ///<param name="If foreign">key support is enabled, rewrite the CREATE TABLE </param>
                    ///<param name="statements corresponding to all child tables of foreign key constraints">statements corresponding to all child tables of foreign key constraints</param>
                    ///<param name="for which the renamed table is the parent table.  ">for which the renamed table is the parent table.  </param>
                    if ((zWhere = this.whereForeignKeys(pTab)) != null) {
                        build.sqlite3NestedParse(this, "UPDATE \"%w\".%s SET " + "sql = sqlite_rename_parent(sql, %Q, %Q) " + "WHERE %s;", zDb, sqliteinth.SCHEMA_TABLE(iDb), zTabName, NewNameOfTheTable, zWhere);
                        db.DbFree(ref zWhere);
                    }
                }
#endif
                ///Modify the sqlite_master table to use the new table name. 
                build.sqlite3NestedParse(this, "UPDATE %Q.%s SET " +
#if SQLITE_OMIT_TRIGGER
																																																																																																																																 "sql = sqlite_rename_table(sql, %Q), " +
#else
                "sql = CASE " + "WHEN type = 'trigger' THEN sqlite_rename_trigger(sql, %Q)" + "ELSE sqlite_rename_table(sql, %Q) END, " +
#endif
 "tbl_name = %Q, " + "name = CASE " + "WHEN type='table' THEN %Q " + "WHEN name LIKE 'sqlite_autoindex%%' AND type='index' THEN " + "'sqlite_autoindex_' || %Q || substr(name,%d+18) " + "ELSE name END " + "WHERE tbl_name=%Q AND " + "(type='table' OR type='index' OR type='trigger');", zDb, sqliteinth.SCHEMA_TABLE(iDb), NewNameOfTheTable, NewNameOfTheTable, NewNameOfTheTable,
#if !SQLITE_OMIT_TRIGGER
                NewNameOfTheTable,
#endif
                NewNameOfTheTable, nTabName, zTabName);
#if !SQLITE_OMIT_AUTOINCREMENT
                ///If the sqlite_sequence table exists in this database, then update
                ///it with the new table name.
                if (TableBuilder.sqlite3FindTable(db, zDb, "sqlite_sequence") != null) {
                    build.sqlite3NestedParse(this, "UPDATE \"%w\".sqlite_sequence set name = %Q WHERE name = %Q", zDb, NewNameOfTheTable, pTab.zName);
                }
#endif
#if !SQLITE_OMIT_TRIGGER
                ///If there are TEMP triggers on this table, modify the sqlite_temp_master
                ///table. Don't do this if the table being ALTERed is itself located in
                ///the temp database.
                if ((zWhere = this.whereTempTriggers(pTab)) != "") {
                    build.sqlite3NestedParse(this, "UPDATE sqlite_temp_master SET " + "sql = sqlite_rename_trigger(sql, %Q), " + "tbl_name = %Q " + "WHERE %s;", NewNameOfTheTable, NewNameOfTheTable, zWhere);
                    db.DbFree(ref zWhere);
                }
#endif
#if !(SQLITE_OMIT_FOREIGN_KEY) && !(SQLITE_OMIT_TRIGGER)
                if ((db.flags & SqliteFlags.SQLITE_ForeignKeys) != 0)
                {
                    FKey p;
                    for (p = fkeyc.sqlite3FkReferences(pTab); p != null; p = p.pNextTo) {
                        Table pFrom = p.pFrom;
                        if (pFrom != pTab) {
                            this.reloadTableSchema(p.pFrom, pFrom.zName);
                        }
                    }
                }
#endif
                ///Drop and reload the internal table schema. 
                this.reloadTableSchema(pTab, NewNameOfTheTable);
                exit_rename_table:
                build.sqlite3SrcListDelete(db, ref pSrc);
                db.DbFree(ref NewNameOfTheTable);
                db.flags = savedDbFlags;
            }

            ///<summary>
			/// Generate code to drop and reload the internal representation of table
			/// pTab from the database, including triggers and temporary triggers.
			/// Argument zName is the name of the table in the database schema at
			/// the time the generated code is executed. This can be different from
			/// pTab.zName if this function is being called to code part of an
			/// "ALTER TABLE RENAME TO" statement.
			///</summary>
            public void reloadTableSchema(Table pTab, string zName)
            {
                ///Index of database containing pTab 
#if !SQLITE_OMIT_TRIGGER
                Trigger pTrig;
#endif
                var v = this.sqlite3GetVdbe();
                if (NEVER(v == null))
                    return;
                Debug.Assert(sqlite3BtreeHoldsAllMutexes(this.db));
                var iDb = this.db.indexOfBackendWithSchema(pTab.pSchema);
                Debug.Assert(iDb >= 0);
#if !SQLITE_OMIT_TRIGGER
                ///Drop any table triggers from the internal schema. 
                pTab.sqlite3TriggerList(this).linkedList().ForEach(
                    trg =>
                    {
                        Drop_Trigger(trg, v, iDb);
                    }
                );
#endif
                Drop_Table(pTab, v, iDb);
                ///Reload the table, index and permanent trigger schemas. 
                var zWhere = io.sqlite3MPrintf(this.db, "tbl_name=%Q", zName);
                if (zWhere == null)
                    return;

                v.codegenAddParseSchemaOp(iDb, zWhere);
#if !SQLITE_OMIT_TRIGGER
                ///Now, if the table is not stored in the temp database, reload any temp
                ///triggers. Don't use IN(...) in case SQLITE_OMIT_SUBQUERY is defined.
                if ((zWhere = this.whereTempTriggers(pTab)) != "")
                {
                    this.codegenAddParseSchemaOp(1, zWhere);
                }
#endif
            }

            



            ///<summary>
            /// Generate the text of a WHERE expression which can be used to select all
            /// temporary triggers on table pTab from the sqlite_temp_master table. If
            /// table pTab has no temporary triggers, or is itself stored in the
            /// temporary database, NULL is returned.
            ///</summary>
            public string whereTempTriggers(Table pTab) {
                string zWhere = "";
                Schema pTempSchema = this.db.Backends[1].pSchema;
                ///Temp db schema 
                ///
                ///If the table is not located in the temp.db (in which case NULL is
                ///returned, loop through the tables list of triggers. For each trigger
                ///that is not part of the temp.db schema, add a clause to the WHERE
                ///expression being built up in zWhere.
                if (pTab.pSchema != pTempSchema)
                    pTab.sqlite3TriggerList(this).linkedList()
                        .Where(pTrig => pTrig.pSchema == pTempSchema)
                        .ForEach(pTrig => zWhere = alter.whereOrName(db, zWhere, pTrig.zName));

                if (!String.IsNullOrEmpty(zWhere)) {
                    zWhere = io.sqlite3MPrintf(this.db, "type='trigger' AND (%s)", zWhere);
                    //sqlite3DbFree( pParse.db, ref zWhere );
                    //zWhere = zNew;
                }
                return zWhere;
            }

            ///<summary>
            /// Generate the text of a WHERE expression which can be used to select all
            /// tables that have foreign key constraints that refer to table pTab (i.e.
            /// constraints for which pTab is the parent table) from the sqlite_master
            /// table.
            ///</summary>
            public string whereForeignKeys(Table pTab) {
                string zWhere = "";
                fkeyc.sqlite3FkReferences(pTab).path(p => p.pNextTo)
                    .ForEach(p => zWhere = alter.whereOrName(this.db, zWhere, p.pFrom.zName));

                return zWhere;
            }
            public void openStatTable(///
                                      ///<summary>
                                      ///Parsing context 
                                      ///</summary>
          int iDb,///
                    ///<summary>
                    ///The database we are looking in 
                    ///</summary>
            int iStatCur,///
                         ///<summary>
                         ///Open the sqlite_stat1 table on this cursor 
                         ///</summary>
           string zWhere,///
                          ///<summary>
                          ///Delete entries for this table or index 
                          ///</summary>
          string zWhereType///
                             ///<summary>
                             ///Either "tbl" or "idx" 
                             ///</summary>
           ) {
                int[] aRoot = new int[] {
                    0,
                    0
                };
                u8[] aCreateTbl = new u8[] {
                    0,
                    0
                };
                int i;
                Connection db = this.db;
                DbBackend pDb;
                Vdbe v = this.sqlite3GetVdbe();
                if (v == null)
                    return;
                Debug.Assert(sqlite3BtreeHoldsAllMutexes(db));
                Debug.Assert(v.sqlite3VdbeDb() == db);
                pDb = db.Backends[iDb];
                for (i = 0; i < Sqlite3.ArraySize(aTable); i++) {
                    string zTab = aTable[i].zName;
                    Table pStat;
                    if ((pStat = TableBuilder.sqlite3FindTable(db, pDb.Name, zTab)) == null) {
                        ///
                        ///<summary>
                        ///The sqlite_stat[12] table does not exist. Create it. Note that a 
                        ///</summary>
                        ///<param name="side">effect of the CREATE TABLE statement is to leave the rootpage </param>
                        ///<param name="of the new table in register pParse.regRoot. This is important ">of the new table in register pParse.regRoot. This is important </param>
                        ///<param name="because the OpenWrite opcode below will be needing it. ">because the OpenWrite opcode below will be needing it. </param>
                        build.sqlite3NestedParse(this, "CREATE TABLE %Q.%s(%s)", pDb.Name, zTab, aTable[i].zCols);
                        aRoot[i] = this.regRoot;
                        aCreateTbl[i] = 1;
                    }
                    else {
                        ///
                        ///<summary>
                        ///The table already exists. If zWhere is not NULL, delete all entries 
                        ///associated with the table zWhere. If zWhere is NULL, delete the
                        ///entire contents of the table. 
                        ///</summary>
                        aRoot[i] = pStat.tnum;
                        sqliteinth.sqlite3TableLock(this, iDb, aRoot[i], 1, zTab);
                        if (!String.IsNullOrEmpty(zWhere)) {
                            build.sqlite3NestedParse(this, "DELETE FROM %Q.%s WHERE %s=%Q", pDb.Name, zTab, zWhereType, zWhere);
                        }
                        else {
                            ///
                            ///<summary>
                            ///The sqlite_stat[12] table already exists.  Delete all rows. 
                            ///</summary>
                            v.sqlite3VdbeAddOp2(OpCode.OP_Clear, aRoot[i], iDb);
                        }
                    }
                }
                ///
                ///<summary>
                ///Open the sqlite_stat[12] tables for writing. 
                ///</summary>
                for (i = 0; i < Sqlite3.ArraySize(aTable); i++) {
                    v.sqlite3VdbeAddOp3(OpCode.OP_OpenWrite, iStatCur + i, aRoot[i], iDb);
                    v.sqlite3VdbeChangeP4(-1, 3, P4Usage.P4_INT32);
                    v.sqlite3VdbeChangeP5(aCreateTbl[i]);
                }
            }
            public///<summary>
                  /// Generate code to do an analysis of all indices associated with
                  /// a single table.
                  ///</summary>
          void analyzeOneTable(///
                                 ///<summary>
                                 ///Parser context 
                                 ///</summary>
           Table pTab,///
                       ///<summary>
                       ///Table whose indices are to be analyzed 
                       ///</summary>
         Index pOnlyIdx,///
                           ///<summary>
                           ///If not NULL, only analyze this one index 
                           ///</summary>
         int iStatCur,///
                         ///<summary>
                         ///Index of VdbeCursor that writes the sqlite_stat1 table 
                         ///</summary>
           int iMem///
                    ///<summary>
                    ///Available memory locations begin here 
                    ///</summary>
            ) {
                Connection db = this.db;
                ///
                ///<summary>
                ///Database handle 
                ///</summary>
                Index pIdx;
                ///
                ///<summary>
                ///An index to being analyzed 
                ///</summary>
                int iIdxCur;
                ///
                ///<summary>
                ///Cursor open on index being analyzed 
                ///</summary>
                Vdbe v;
                ///
                ///<summary>
                ///The virtual machine being built up 
                ///</summary>
                int i;
                ///
                ///<summary>
                ///Loop counter 
                ///</summary>
                int topOfLoop;
                ///
                ///<summary>
                ///The top of the loop 
                ///</summary>
                int endOfLoop;
                ///
                ///<summary>
                ///The end of the loop 
                ///</summary>
                int jZeroRows = -1;
                ///
                ///<summary>
                ///Jump from here if number of rows is zero 
                ///</summary>
                int iDb;
                ///
                ///<summary>
                ///Index of database containing pTab 
                ///</summary>
                int regTabname = iMem++;
                ///
                ///<summary>
                ///Register containing table name 
                ///</summary>
                int regIdxname = iMem++;
                ///
                ///<summary>
                ///Register containing index name 
                ///</summary>
                int regSampleno = iMem++;
                ///
                ///<summary>
                ///Register containing next sample number 
                ///</summary>
                int regCol = iMem++;
                ///
                ///<summary>
                ///Content of a column analyzed table 
                ///</summary>
                int regRec = iMem++;
                ///
                ///<summary>
                ///Register holding completed record 
                ///</summary>
                int regTemp = iMem++;
                ///
                ///<summary>
                ///Temporary use register 
                ///</summary>
                int regRowid = iMem++;
                ///
                ///<summary>
                ///Rowid for the inserted record 
                ///</summary>
#if SQLITE_ENABLE_STAT2
																																																																																																																															  int addr = 0;                /* Instruction address */
  int regTemp2 = iMem++;       /* Temporary use register */
  int regSamplerecno = iMem++; /* Index of next sample to record */
  int regRecno = iMem++;       /* Current sample index */
  int regLast = iMem++;        /* Index of last sample to record */
  int regFirst = iMem++;       /* Index of first sample to record */
#endif
                v = this.sqlite3GetVdbe();
                if (v == null || NEVER(pTab == null)) {
                    return;
                }
                if (pTab.tnum == 0) {
                    ///
                    ///<summary>
                    ///Do not gather statistics on views or virtual tables 
                    ///</summary>
                    return;
                }
                if (pTab.zName.StartsWith("sqlite_", StringComparison.InvariantCultureIgnoreCase)) {
                    ///
                    ///<summary>
                    ///Do not gather statistics on system tables 
                    ///</summary>
                    return;
                }
                Debug.Assert(sqlite3BtreeHoldsAllMutexes(db));
                iDb = db.indexOfBackendWithSchema(pTab.pSchema);
                Debug.Assert(iDb >= 0);
                Debug.Assert(sqlite3SchemaMutexHeld(db, iDb, null));
#if !SQLITE_OMIT_AUTHORIZATION
																																																																																																																															if( sqlite3AuthCheck(pParse, SQLITE_ANALYZE, pTab.zName, 0,
db.aDb[iDb].zName ) ){
return;
}
#endif
                ///
                ///<summary>
                ///</summary>
                ///<param name="Establish a read">cache level. </param>
                sqliteinth.sqlite3TableLock(this, iDb, pTab.tnum, 0, pTab.zName);
                iIdxCur = this.nTab++;
                v.sqlite3VdbeAddOp4(OpCode.OP_String8, 0, regTabname, 0, pTab.zName, 0);
                for (pIdx = pTab.pIndex; pIdx != null; pIdx = pIdx.pNext) {
                    int nCol;
                    KeyInfo pKey;
                    if (pOnlyIdx != null && pOnlyIdx != pIdx)
                        continue;
                    nCol = pIdx.nColumn;
                    pKey = pIdx.sqlite3IndexKeyinfo(this);
                    if (iMem + 1 + (nCol * 2) > this.UsedCellCount) {
                        this.UsedCellCount = iMem + 1 + (nCol * 2);
                    }
                    ///
                    ///<summary>
                    ///Open a cursor to the index to be analyzed. 
                    ///</summary>
                    Debug.Assert(iDb == db.indexOfBackendWithSchema(pIdx.pSchema));
                    v.sqlite3VdbeAddOp4(OpCode.OP_OpenRead, iIdxCur, pIdx.tnum, iDb, pKey, P4Usage.P4_KEYINFO_HANDOFF);
                    v.VdbeComment("%s", pIdx.zName);
                    ///
                    ///<summary>
                    ///Populate the registers containing the index names. 
                    ///</summary>
                    v.sqlite3VdbeAddOp4(OpCode.OP_String8, 0, regIdxname, 0, pIdx.zName, 0);
#if SQLITE_ENABLE_STAT2
																																																																																																																																																															
    /* If this iteration of the loop is generating code to analyze the
** first index in the pTab.pIndex list, then register regLast has
** not been populated. In this case populate it now.  */
    if ( pTab.pIndex == pIdx )
    {
      sqlite3VdbeAddOp2( v, OpCode.OP_Integer, SQLITE_INDEX_SAMPLES, regSamplerecno );
      sqlite3VdbeAddOp2( v, OpCode.OP_Integer, SQLITE_INDEX_SAMPLES * 2 - 1, regTemp );
      sqlite3VdbeAddOp2( v, OpCode.OP_Integer, SQLITE_INDEX_SAMPLES * 2, regTemp2 );

      sqlite3VdbeAddOp2( v,  OpCode.OP_Count, iIdxCur, regLast );
      sqlite3VdbeAddOp2( v,  OpCode.OP_Null, 0, regFirst );
      addr = sqlite3VdbeAddOp3( v,  OpCode.OP_Lt, regSamplerecno, 0, regLast );
      sqlite3VdbeAddOp3( v,  OpCode.OP_Divide, regTemp2, regLast, regFirst );
      sqlite3VdbeAddOp3( v,  OpCode.OP_Multiply, regLast, regTemp, regLast );
      sqlite3VdbeAddOp2( v,  OpCode.OP_AddImm, regLast, SQLITE_INDEX_SAMPLES * 2 - 2 );
      sqlite3VdbeAddOp3( v,  OpCode.OP_Divide, regTemp2, regLast, regLast );
      sqlite3VdbeJumpHere( v, addr );
    }

    /* Zero the regSampleno and regRecno registers. */
    sqlite3VdbeAddOp2( v, OpCode.OP_Integer, 0, regSampleno );
    sqlite3VdbeAddOp2( v, OpCode.OP_Integer, 0, regRecno );
    sqlite3VdbeAddOp2( v,  OpCode.OP_Copy, regFirst, regSamplerecno );
#endif
                    ///
                    ///<summary>
                    ///The block of memory cells initialized here is used as follows.
                    ///
                    ///iMem:                
                    ///The total number of rows in the table.
                    ///
                    ///iMem+1 .. iMem+nCol: 
                    ///Number of distinct entries in index considering the 
                    ///</summary>
                    ///<param name="left">most N columns only, where N is between 1 and nCol, </param>
                    ///<param name="inclusive.">inclusive.</param>
                    ///<param name=""></param>
                    ///<param name="iMem+nCol+1 .. Mem+2*nCol:  ">iMem+nCol+1 .. Mem+2*nCol:  </param>
                    ///<param name="Previous value of indexed columns, from left to right.">Previous value of indexed columns, from left to right.</param>
                    ///<param name=""></param>
                    ///<param name="Cells iMem through iMem+nCol are initialized to 0. The others are ">Cells iMem through iMem+nCol are initialized to 0. The others are </param>
                    ///<param name="initialized to contain an SQL NULL.">initialized to contain an SQL NULL.</param>
                    for (i = 0; i <= nCol; i++) {
                        v.sqlite3VdbeAddOp2(OpCode.OP_Integer, 0, iMem + i);
                    }
                    for (i = 0; i < nCol; i++) {
                        v.sqlite3VdbeAddOp2(OpCode.OP_Null, 0, iMem + nCol + i + 1);
                    }
                    ///
                    ///<summary>
                    ///Start the analysis loop. This loop runs through all the entries in
                    ///</summary>
                    ///<param name="the index b">tree.  </param>
                    endOfLoop = v.sqlite3VdbeMakeLabel();
                    v.sqlite3VdbeAddOp2(OpCode.OP_Rewind, iIdxCur, endOfLoop);
                    topOfLoop = v.sqlite3VdbeCurrentAddr();
                    v.sqlite3VdbeAddOp2(OpCode.OP_AddImm, iMem, 1);
                    for (i = 0; i < nCol; i++) {
                        v.sqlite3VdbeAddOp3(OpCode.OP_Column, iIdxCur, i, regCol);
                        CollSeq pColl;
                        if (i == 0) {
#if SQLITE_ENABLE_STAT2
																																																																																																																																																																																																																															        /* Check if the record that cursor iIdxCur points to contains a
** value that should be stored in the sqlite_stat2 table. If so,
** store it.  */
        int ne = sqlite3VdbeAddOp3( v,  OpCode.OP_Ne, regRecno, 0, regSamplerecno );
        Debug.Assert( regTabname + 1 == regIdxname
        && regTabname + 2 == regSampleno
        && regTabname + 3 == regCol
        );
        sqlite3VdbeChangeP5( v, sqliteinth.SQLITE_JUMPIFNULL );
        sqlite3VdbeAddOp4( v,  OpCode.OP_MakeRecord, regTabname, 4, regRec, "aaab", 0 );
        sqlite3VdbeAddOp2( v,  OpCode.OP_NewRowid, iStatCur + 1, regRowid );
        sqlite3VdbeAddOp3( v,  OpCode.OP_Insert, iStatCur + 1, regRec, regRowid );

        /* Calculate new values for regSamplerecno and regSampleno.
        **
        **   sampleno = sampleno + 1
        **   samplerecno = samplerecno+(remaining records)/(remaining samples)
        */
        sqlite3VdbeAddOp2( v,  OpCode.OP_AddImm, regSampleno, 1 );
        sqlite3VdbeAddOp3( v,  OpCode.OP_Subtract, regRecno, regLast, regTemp );
        sqlite3VdbeAddOp2( v,  OpCode.OP_AddImm, regTemp, -1 );
        sqlite3VdbeAddOp2( v, OpCode.OP_Integer, SQLITE_INDEX_SAMPLES, regTemp2 );
        sqlite3VdbeAddOp3( v,  OpCode.OP_Subtract, regSampleno, regTemp2, regTemp2 );
        sqlite3VdbeAddOp3( v,  OpCode.OP_Divide, regTemp2, regTemp, regTemp );
        sqlite3VdbeAddOp3( v,  OpCode.OP_Add, regSamplerecno, regTemp, regSamplerecno );

        sqlite3VdbeJumpHere( v, ne );
        sqlite3VdbeAddOp2( v,  OpCode.OP_AddImm, regRecno, 1 );
#endif
                            ///
                            ///<summary>
                            ///Always record the very first row 
                            ///</summary>
                            v.sqlite3VdbeAddOp1(OpCode.OP_IfNot, iMem + 1);
                        }
                        Debug.Assert(pIdx.azColl != null);
                        Debug.Assert(pIdx.azColl[i] != null);
                        pColl = build.sqlite3LocateCollSeq(this, pIdx.azColl[i]);
                        v.sqlite3VdbeAddOp4(OpCode.OP_Ne, regCol, 0, iMem + nCol + i + 1, pColl, P4Usage.P4_COLLSEQ);
                        v.sqlite3VdbeChangeP5(sqliteinth.SQLITE_NULLEQ);
                    }
                    //if( db.mallocFailed ){
                    //  /* If a malloc failure has occurred, then the result of the expression 
                    //  ** passed as the second argument to the call to sqlite3VdbeJumpHere() 
                    //  ** below may be negative. Which causes an Debug.Assert() to fail (or an
                    //  ** out-of-bounds write if SQLITE_DEBUG is not defined).  */
                    //  return;
                    //}
                    v.sqlite3VdbeAddOp2(OpCode.OP_Goto, 0, endOfLoop);
                    for (i = 0; i < nCol; i++) {
                        int addr2 = v.sqlite3VdbeCurrentAddr() - (nCol * 2);
                        if (i == 0) {
                            v.sqlite3VdbeJumpHere(addr2 - 1);
                            ///
                            ///<summary>
                            ///Set jump dest for the  OpCode.OP_IfNot 
                            ///</summary>
                        }
                        v.sqlite3VdbeJumpHere(addr2);
                        ///
                        ///<summary>
                        ///Set jump dest for the  OpCode.OP_Ne 
                        ///</summary>
                        v.sqlite3VdbeAddOp2(OpCode.OP_AddImm, iMem + i + 1, 1);
                        v.sqlite3VdbeAddOp3(OpCode.OP_Column, iIdxCur, i, iMem + nCol + i + 1);
                    }
                    ///
                    ///<summary>
                    ///End of the analysis loop. 
                    ///</summary>
                    v.sqlite3VdbeResolveLabel(endOfLoop);
                    v.sqlite3VdbeAddOp2(OpCode.OP_Next, iIdxCur, topOfLoop);
                    v.sqlite3VdbeAddOp1(OpCode.OP_Close, iIdxCur);
                    ///
                    ///<summary>
                    ///Store the results in sqlite_stat1.
                    ///
                    ///The result is a single row of the sqlite_stat1 table.  The first
                    ///two columns are the names of the table and index.  The third column
                    ///is a string composed of a list of integer statistics about the
                    ///index.  The first integer in the list is the total number of entries
                    ///in the index.  There is one additional integer in the list for each
                    ///column of the table.  This additional integer is a guess of how many
                    ///rows of the table the index will select.  If D is the count of distinct
                    ///values and K is the total number of rows, then the integer is computed
                    ///as:
                    ///
                    ///</summary>
                    ///<param name="I = (K+D">1)/D</param>
                    ///<param name=""></param>
                    ///<param name="If K==0 then no entry is made into the sqlite_stat1 table.  ">If K==0 then no entry is made into the sqlite_stat1 table.  </param>
                    ///<param name="If K>0 then it is always the case the D>0 so division by zero">If K>0 then it is always the case the D>0 so division by zero</param>
                    ///<param name="is never possible.">is never possible.</param>
                    ///<param name=""></param>
                    v.sqlite3VdbeAddOp2(OpCode.OP_SCopy, iMem, regSampleno);
                    if (jZeroRows < 0) {
                        jZeroRows = v.sqlite3VdbeAddOp1(OpCode.OP_IfNot, iMem);
                    }
                    for (i = 0; i < nCol; i++) {
                        v.sqlite3VdbeAddOp4(OpCode.OP_String8, 0, regTemp, 0, " ", 0);
                        v.sqlite3VdbeAddOp3(OpCode.OP_Concat, regTemp, regSampleno, regSampleno);
                        v.sqlite3VdbeAddOp3(OpCode.OP_Add, iMem, iMem + i + 1, regTemp);
                        v.sqlite3VdbeAddOp2(OpCode.OP_AddImm, regTemp, -1);
                        v.sqlite3VdbeAddOp3(OpCode.OP_Divide, iMem + i + 1, regTemp, regTemp);
                        v.sqlite3VdbeAddOp1(OpCode.OP_ToInt, regTemp);
                        v.sqlite3VdbeAddOp3(OpCode.OP_Concat, regTemp, regSampleno, regSampleno);
                    }
                    v.sqlite3VdbeAddOp4(OpCode.OP_MakeRecord, regTabname, 3, regRec, "aaa", 0);
                    v.sqlite3VdbeAddOp2(OpCode.OP_NewRowid, iStatCur, regRowid);
                    v.sqlite3VdbeAddOp3(OpCode.OP_Insert, iStatCur, regRec, regRowid);
                    v.sqlite3VdbeChangeP5(OpFlag.OPFLAG_APPEND);
                }
                ///
                ///<summary>
                ///If the table has no indices, create a single sqlite_stat1 entry
                ///containing NULL as the index name and the row count as the content.
                ///
                ///</summary>
                if (pTab.pIndex == null) {
                    v.sqlite3VdbeAddOp3(OpCode.OP_OpenRead, iIdxCur, pTab.tnum, iDb);
                    v.VdbeComment("%s", pTab.zName);
                    v.sqlite3VdbeAddOp2(OpCode.OP_Count, iIdxCur, regSampleno);
                    v.sqlite3VdbeAddOp1(OpCode.OP_Close, iIdxCur);
                    jZeroRows = v.sqlite3VdbeAddOp1(OpCode.OP_IfNot, regSampleno);
                }
                else {
                    v.sqlite3VdbeJumpHere(jZeroRows);
                    jZeroRows = v.sqlite3VdbeAddOp0(OpCode.OP_Goto);
                }
                v.sqlite3VdbeAddOp2(OpCode.OP_Null, 0, regIdxname);
                v.sqlite3VdbeAddOp4(OpCode.OP_MakeRecord, regTabname, 3, regRec, "aaa", 0);
                v.sqlite3VdbeAddOp2(OpCode.OP_NewRowid, iStatCur, regRowid);
                v.sqlite3VdbeAddOp3(OpCode.OP_Insert, iStatCur, regRec, regRowid);
                v.sqlite3VdbeChangeP5(OpFlag.OPFLAG_APPEND);
                if (this.UsedCellCount < regRec)
                    this.UsedCellCount = regRec;
                v.sqlite3VdbeJumpHere(jZeroRows);
            }
            public///<summary>
                  /// Generate code that will cause the most recent index analysis to
                  /// be loaded into internal hash tables where is can be used.
                  ///</summary>
          void loadAnalysis(int iDb) {
                Vdbe v = this.sqlite3GetVdbe();
                if (v != null) {
                    v.sqlite3VdbeAddOp1(OpCode.OP_LoadAnalysis, iDb);
                }
            }
            public///<summary>
                  /// Generate code that will do an analysis of an entire database
                  ///</summary>
          void analyzeDatabase(int iDb) {
                Connection db = this.db;
                Schema pSchema = db.Backends[iDb].pSchema;
                ///
                ///<summary>
                ///Schema of database iDb 
                ///</summary>
                HashElem k;
                int iStatCur;
                int iMem;
                build.sqlite3BeginWriteOperation(this, 0, iDb);
                iStatCur = this.nTab;
                this.nTab += 2;
                this.openStatTable(iDb, iStatCur, null, null);
                iMem = this.UsedCellCount + 1;
                Debug.Assert(sqlite3SchemaMutexHeld(db, iDb, null));
                //for(k=sqliteHashFirst(pSchema.tblHash); k; k=sqliteHashNext(k)){
                for (k = pSchema.Tables.first; k != null; k = k.pNext) {
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
            public void analyzeTable(Table pTab, Index pOnlyIdx) {
                int iDb;
                int iStatCur;
                Debug.Assert(pTab != null);
                Debug.Assert(sqlite3BtreeHoldsAllMutexes(this.db));
                iDb = this.db.indexOfBackendWithSchema(pTab.pSchema);
                build.sqlite3BeginWriteOperation(this, 0, iDb);
                iStatCur = this.nTab;
                this.nTab += 2;
                if (pOnlyIdx != null) {
                    this.openStatTable(iDb, iStatCur, pOnlyIdx.zName, "idx");
                }
                else {
                    this.openStatTable(iDb, iStatCur, pTab.zName, "tbl");
                }
                this.analyzeOneTable(pTab, pOnlyIdx, iStatCur, this.UsedCellCount + 1);
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
            void sqlite3Analyze(int null_2, int null_3) {
                this.sqlite3Analyze(null, null);
            }
            public void sqlite3Analyze(Token pName1, Token pName2) {
                Connection db = this.db;
                Table pTab;
                Index pIdx;
                Token pTableName = null;
                ///Read the database schema. If an error occurs, leave an error message
                ///and code in pParse and return NULL. 
                Debug.Assert(sqlite3BtreeHoldsAllMutexes(this.db));
                if (SqlResult.SQLITE_OK != prepare.sqlite3ReadSchema(this)) {
                    return;
                }
                Debug.Assert(pName2 != null || pName1 == null);
                if (pName1 == null) {
                    ///Form 1:  Analyze everything 
                    for (var i = 0; i < db.BackendCount; i++) {
                        if (i == 1)
                            continue;
                        ///Do not analyze the TEMP database 
                        this.analyzeDatabase(i);
                    }
                }
                else
                    if (pName2.Length == 0) {
                    ///Form 2:  Analyze the database or table named 
                    var iDb = db.FindDbIdxByToken(pName1);
                    if (iDb >= 0) {
                        this.analyzeDatabase(iDb);
                    }
                    else {
                        var name = build.Token2Name(db, pName1);
                        if (name != null) {
                            if ((pIdx = IndexBuilder.sqlite3FindIndex(db, name, null)) != null) {
                                this.analyzeTable(pIdx.pTable, pIdx);
                            }
                            else
                                if ((pTab = TableBuilder.sqlite3LocateTable(this, name)) != null) {
                                this.analyzeTable(pTab, null);
                            }
                            name = null;
                            //sqlite3DbFree( db, z );
                        }
                    }
                }
                else {
                    ///Form 3: Analyze the fully qualified table name 
                    var iDb = build.sqlite3TwoPartName(this, pName1, pName2, ref pTableName);
                    if (iDb >= 0) {
                        var zDb = db.Backends[iDb].Name;
                        var z = build.Token2Name(db, pTableName);
                        if (z != null) {
                            if ((pIdx = IndexBuilder.sqlite3FindIndex(db, z, zDb)) != null) {
                                this.analyzeTable(pIdx.pTable, pIdx);
                            }
                            else
                                if ((pTab = TableBuilder.sqlite3LocateTable(this, z, zDb)) != null) {
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
            public void codeAttach(///
                                   ///<summary>
                                   ///The parser context 
                                   ///</summary>
            AuthTarget type,///
                            ///<summary>
                            ///Either SQLITE_ATTACH or SQLITE_DETACH 
                            ///</summary>
            FuncDef pFunc,///
                          ///<summary>
                          ///FuncDef wrapper for detachFunc() or attachFunc() 
                          ///</summary>
          Expr pAuthArg,///
                        ///<summary>
                        ///Expression to pass to authorization callback 
                        ///</summary>
            Expr pFilename,///
                           ///<summary>
                           ///Name of database file 
                           ///</summary>
         Expr pDbname,///
                      ///<summary>
                      ///Name of the database to use internally 
                      ///</summary>
          Expr pKey///
                   ///<summary>
                   ///Database key for encryption extension 
                   ///</summary>
         ) {
                SqlResult rc;
                NameContext sName;
                Vdbe v;
                Connection db = this.db;
                int regArgs;
                sName = new NameContext();
                // memset( &sName, 0, sizeof(NameContext));
                sName.pParse = this;
                if (SqlResult.SQLITE_OK != (rc = sName.resolveAttachExpr(pFilename)) || SqlResult.SQLITE_OK != (rc = sName.resolveAttachExpr(pDbname)) || SqlResult.SQLITE_OK != (rc = sName.resolveAttachExpr(pKey))) {
                    this.nErr++;
                    goto attach_end;
                }
#if !SQLITE_OMIT_AUTHORIZATION
																																																																																																																														if( pAuthArg ){
char *zAuthArg;
if( pAuthArg->op==TokenType.TK_STRING ){
  zAuthArg = pAuthArg->u.zToken;
}else{
  zAuthArg = 0;
}
rc = sqlite3AuthCheck(pParse, type, zAuthArg, 0, 0);
if(rc!=SqlResult.SQLITE_OK ){
goto attach_end;
}
}
#endif
                v = this.sqlite3GetVdbe();
                regArgs = this.sqlite3GetTempRange(4);
                this.sqlite3ExprCode(pFilename, regArgs);
                this.sqlite3ExprCode(pDbname, regArgs + 1);
                this.sqlite3ExprCode(pKey, regArgs + 2);
                Debug.Assert(v != null///
                                      ///<summary>
                                      ///|| db.mallocFailed != 0 
                                      ///</summary>
              );
                if (v != null) {
                    v.sqlite3VdbeAddOp3(OpCode.OP_Function, 0, regArgs + 3 - pFunc.nArg, regArgs + 3);
                    Debug.Assert(pFunc.nArg == -1 || (pFunc.nArg & 0xff) == pFunc.nArg);
                    v.sqlite3VdbeChangeP5((u8)(pFunc.nArg));
                    v.sqlite3VdbeChangeP4(-1, pFunc, P4Usage.P4_FUNCDEF);
                    ///
                    ///<summary>
                    ///Code an  OpCode.OP_Expire. For an ATTACH statement, set P1 to true (expire this
                    ///statement only). For DETACH, set it to false (expire all existing
                    ///statements).
                    ///
                    ///</summary>
                    v.sqlite3VdbeAddOp1(OpCode.OP_Expire, (type == AuthTarget.SQLITE_ATTACH) ? 1 : 0);
                }
                attach_end:
                exprc.Delete(db, ref pFilename);
                exprc.Delete(db, ref pDbname);
                exprc.Delete(db, ref pKey);
            }
            public void sqlite3Detach(Expr pDbname) {
                this.codeAttach(AuthTarget.SQLITE_DETACH, detach_func, pDbname, null, null, pDbname);
            }
            public void sqlite3Attach(Expr p, Expr pDbname, Expr pKey) {
                this.codeAttach(AuthTarget.SQLITE_ATTACH, attach_func, p, p, pDbname, pKey);
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
          int locateFkeyIndex(///
                                ///<summary>
                                ///Parse context to store any error in 
                                ///</summary>
            Table pParent,///
                          ///<summary>
                          ///Parent table of FK constraint pFKey 
                          ///</summary>
          FKey pFKey,///
                       ///<summary>
                       ///Foreign key to find index for 
                       ///</summary>
         out Index ppIdx,///
                            ///<summary>
                            ///OUT: Unique index on parent table 
                            ///</summary>
            out int[] paiCol///
                            ///<summary>
                            ///OUT: Map of index columns in pFKey 
                            ///</summary>
            ) {
                Index pIdx = null;
                ///
                ///<summary>
                ///Value to return via *ppIdx 
                ///</summary>
                ppIdx = null;
                int[] aiCol = null;
                ///
                ///<summary>
                ///Value to return via *paiCol 
                ///</summary>
                paiCol = null;
                int nCol = pFKey.nCol;
                ///
                ///<summary>
                ///Number of columns in parent key 
                ///</summary>
                string zKey = pFKey.aCol[0].zCol;
                ///
                ///<summary>
                ///</summary>
                ///<param name="Name of left">most parent key column </param>
                ///
                ///<summary>
                ///The caller is responsible for zeroing output parameters. 
                ///</summary>
                //assert( ppIdx && *ppIdx==0 );
                //assert( !paiCol || *paiCol==0 );
                Debug.Assert(this != null);
                ///
                ///<summary>
                ///</summary>
                ///<param name="If this is a non">composite (single column) foreign key, check if it </param>
                ///<param name="maps to the INTEGER PRIMARY KEY of table pParent. If so, leave *ppIdx ">maps to the INTEGER PRIMARY KEY of table pParent. If so, leave *ppIdx </param>
                ///<param name="and *paiCol set to zero and return early. ">and *paiCol set to zero and return early. </param>
                ///<param name=""></param>
                ///<param name="Otherwise, for a composite foreign key (more than one column), allocate">Otherwise, for a composite foreign key (more than one column), allocate</param>
                ///<param name="space for the aiCol array (returned via output parameter *paiCol).">space for the aiCol array (returned via output parameter *paiCol).</param>
                ///<param name="Non">composite foreign keys do not require the aiCol array.</param>
                ///<param name=""></param>
                if (nCol == 1) {
                    ///
                    ///<summary>
                    ///The FK maps to the IPK if any of the following are true:
                    ///
                    ///1) There is an INTEGER PRIMARY KEY column and the FK is implicitly 
                    ///mapped to the primary key of table pParent, or
                    ///2) The FK is explicitly mapped to a column declared as INTEGER
                    ///PRIMARY KEY.
                    ///
                    ///</summary>
                    if (pParent.iPKey >= 0) {
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
                for (pIdx = pParent.pIndex; pIdx != null; pIdx = pIdx.pNext) {
                    if (pIdx.nColumn == nCol && pIdx.onError != OnConstraintError.OE_None) {
                        ///
                        ///<summary>
                        ///pIdx is a UNIQUE index (or a PRIMARY KEY) and has the right number
                        ///of columns. If each indexed column corresponds to a foreign key
                        ///column of pFKey, then this index is a winner.  
                        ///</summary>
                        if (zKey == null) {
                            ///
                            ///<summary>
                            ///If zKey is NULL, then this foreign key is implicitly mapped to 
                            ///the PRIMARY KEY of table pParent. The PRIMARY KEY index may be 
                            ///identified by the test (Index.autoIndex==2).  
                            ///</summary>
                            if (pIdx.autoIndex == 2) {
                                if (aiCol != null) {
                                    int i;
                                    for (i = 0; i < nCol; i++)
                                        aiCol[i] = pFKey.aCol[i].iFrom;
                                }
                                break;
                            }
                        }
                        else {
                            ///
                            ///<summary>
                            ///</summary>
                            ///<param name="If zKey is non">NULL, then this foreign key was declared to</param>
                            ///<param name="map to an explicit list of columns in table pParent. Check if this">map to an explicit list of columns in table pParent. Check if this</param>
                            ///<param name="index matches those columns. Also, check that the index uses">index matches those columns. Also, check that the index uses</param>
                            ///<param name="the default collation sequences for each column. ">the default collation sequences for each column. </param>
                            int i, j;
                            for (i = 0; i < nCol; i++) {
                                int iCol = pIdx.aiColumn[i];
                                ///
                                ///<summary>
                                ///Index of column in parent tbl 
                                ///</summary>
                                string zDfltColl;
                                ///
                                ///<summary>
                                ///Def. collation for column 
                                ///</summary>
                                string zIdxCol;
                                ///
                                ///<summary>
                                ///Name of indexed column 
                                ///</summary>
                                ///
                                ///<summary>
                                ///If the index uses a collation sequence that is different from
                                ///the default collation sequence for the column, this index is
                                ///unusable. Bail out early in this case.  
                                ///</summary>
                                zDfltColl = pParent.aCol[iCol].zColl;
                                if (String.IsNullOrEmpty(zDfltColl)) {
                                    zDfltColl = "BINARY";
                                }
                                if (!pIdx.azColl[i].Equals(zDfltColl, StringComparison.InvariantCultureIgnoreCase))
                                    break;
                                zIdxCol = pParent.aCol[iCol].zName;
                                for (j = 0; j < nCol; j++) {
                                    if (pFKey.aCol[j].zCol.Equals(zIdxCol, StringComparison.InvariantCultureIgnoreCase)) {
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
                            ///
                            ///<summary>
                            ///pIdx is usable 
                            ///</summary>
                        }
                    }
                }
                if (null == pIdx) {
                    if (0 == this.disableTriggers) {
                        utilc.sqlite3ErrorMsg(this, "foreign key mismatch");
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
                  ///   Op | FK type   | Action taken
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
          void fkLookupParent(///
                                ///<summary>
                                ///Parse context 
                                ///</summary>
            int iDb,///
                    ///<summary>
                    ///Index of database housing pTab 
                    ///</summary>
            Table pTab,///
                       ///<summary>
                       ///Parent table of FK pFKey 
                       ///</summary>
         Index pIdx,///
                       ///<summary>
                       ///Unique index on parent key columns in pTab 
                       ///</summary>
         FKey pFKey,///
                       ///<summary>
                       ///Foreign key constraint 
                       ///</summary>
         int[] aiCol,///
                        ///<summary>
                        ///Map from parent key columns to child table columns 
                        ///</summary>
            int regData,///
                        ///<summary>
                        ///Address of array containing child table row 
                        ///</summary>
            int nIncr,///
                      ///<summary>
                      ///Increment constraint counter by this 
                      ///</summary>
          int isIgnore///
                        ///<summary>
                        ///If true, pretend pTab contains all NULL values 
                        ///</summary>
            ) {

                Vdbe v = this.sqlite3GetVdbe();
                ///Vdbe to add code to 				
                int iCur = this.nTab - 1;
                ///Cursor number to use 
                int iOk = v.sqlite3VdbeMakeLabel();
                ///jump here if parent key found 
                ///<summary>
                ///If nIncr is less than zero, then check at runtime if there are any
                ///outstanding constraints to resolve. If there are not, there is no need
                ///to check if deleting this row resolves any outstanding violations.
                ///
                ///Check if any of the key columns in the child table row are NULL. If 
                ///any are, then the constraint is considered satisfied. No need to 
                ///search for a matching row in the parent table.  
                ///</summary>
                if (nIncr < 0) {
                    v.sqlite3VdbeAddOp2(OpCode.OP_FkIfZero, pFKey.isDeferred, iOk);
                }
                for (var i = 0; i < pFKey.nCol; i++) {
                    int iReg = aiCol[i] + regData + 1;
                    v.sqlite3VdbeAddOp2(OpCode.OP_IsNull, iReg, iOk);
                }
                if (isIgnore == 0) {
                    if (pIdx == null) {
                        ///If pIdx is NULL, then the parent key is the INTEGER PRIMARY KEY
                        ///column of the parent table (table pTab).  

                        ///Address of MustBeInt instruction 
                        int regTemp = this.allocTempReg();
                        ///
                        ///<summary>
                        ///Invoke MustBeInt to coerce the child key value to an integer (i.e. 
                        ///apply the affinity of the parent key). If this fails, then there
                        ///is no matching parent key. Before using MustBeInt, make a copy of
                        ///the value. Otherwise, the value inserted into the child key column
                        ///will have INTEGER affinity applied to it, which may not be correct.  
                        ///</summary>
                        v.sqlite3VdbeAddOp2(OpCode.OP_SCopy, aiCol[0] + 1 + regData, regTemp);
                        var iMustBeInt = v.sqlite3VdbeAddOp2(OpCode.OP_MustBeInt, regTemp, 0);
                        ///If the parent table is the same as the child table, and we are about
                        ///<param name="to increment the constraint">counter (i.e. this is an INSERT operation),</param>
                        ///<param name="then check if the row being inserted matches itself. If so, do not">then check if the row being inserted matches itself. If so, do not</param>
                        ///<param name="increment the constraint">counter.  </param>
                        if (pTab == pFKey.pFrom && nIncr == 1) {
                            v.sqlite3VdbeAddOp3(OpCode.OP_Eq, regData, iOk, regTemp);
                        }
                        this.sqlite3OpenTable(iCur, iDb, pTab, OpCode.OP_OpenRead);
                        v.sqlite3VdbeAddOp3(OpCode.OP_NotExists, iCur, 0, regTemp);
                        v.sqlite3VdbeAddOp2(OpCode.OP_Goto, 0, iOk);
                        v.sqlite3VdbeJumpHere(v.sqlite3VdbeCurrentAddr() - 2);
                        v.sqlite3VdbeJumpHere(iMustBeInt);
                        this.deallocTempReg(regTemp);
                    }
                    else {
                        int nCol = pFKey.nCol;
                        int regTemp = this.sqlite3GetTempRange(nCol);
                        int regRec = this.allocTempReg();
                        KeyInfo pKey = pIdx.sqlite3IndexKeyinfo(this);
                        v.sqlite3VdbeAddOp3(OpCode.OP_OpenRead, iCur, pIdx.tnum, iDb);
                        v.sqlite3VdbeChangeP4(-1, pKey, P4Usage.P4_KEYINFO_HANDOFF);
                        for (var i = 0; i < nCol; i++) {
                            v.sqlite3VdbeAddOp2(OpCode.OP_Copy, aiCol[i] + 1 + regData, regTemp + i);
                        }
                        ///
                        ///<summary>
                        ///If the parent table is the same as the child table, and we are about
                        ///</summary>
                        ///<param name="to increment the constraint">counter (i.e. this is an INSERT operation),</param>
                        ///<param name="then check if the row being inserted matches itself. If so, do not">then check if the row being inserted matches itself. If so, do not</param>
                        ///<param name="increment the constraint">counter. </param>
                        ///<param name=""></param>
                        ///<param name="If any of the parent">key values are NULL, then the row cannot match </param>
                        ///<param name="itself. So set JUMPIFNULL to make sure we do the  OpCode.OP_Found if any">itself. So set JUMPIFNULL to make sure we do the  OpCode.OP_Found if any</param>
                        ///<param name="of the parent">key values are NULL (at this point it is known that</param>
                        ///<param name="none of the child key values are).">none of the child key values are).</param>
                        ///<param name=""></param>
                        if (pTab == pFKey.pFrom && nIncr == 1) {
                            int iJump = v.sqlite3VdbeCurrentAddr() + nCol + 1;
                            for (var i = 0; i < nCol; i++) {
                                int iChild = aiCol[i] + 1 + regData;
                                int iParent = pIdx.aiColumn[i] + 1 + regData;
                                Debug.Assert(aiCol[i] != pTab.iPKey);
                                if (pIdx.aiColumn[i] == pTab.iPKey) {
                                    ///
                                    ///<summary>
                                    ///The parent key is a composite key that includes the IPK column 
                                    ///</summary>
                                    iParent = regData;
                                }
                                v.sqlite3VdbeAddOp3(OpCode.OP_Ne, iChild, iJump, iParent);
                                v.sqlite3VdbeChangeP5(sqliteinth.SQLITE_JUMPIFNULL);
                            }
                            v.sqlite3VdbeAddOp2(OpCode.OP_Goto, 0, iOk);
                        }
                        v.sqlite3VdbeAddOp3(OpCode.OP_MakeRecord, regTemp, nCol, regRec);
                        v.sqlite3VdbeChangeP4(-1, v.sqlite3IndexAffinityStr(pIdx), P4Usage.P4_TRANSIENT);
                        v.sqlite3VdbeAddOp4Int(OpCode.OP_Found, iCur, iOk, regRec, 0);
                        this.deallocTempReg(regRec);
                        this.sqlite3ReleaseTempRange(regTemp, nCol);
                    }
                }
                if (0 == pFKey.isDeferred && null == this.pToplevel && 0 == this.isMultiWrite) {
                    ///
                    ///<summary>
                    ///Special case: If this is an INSERT statement that will insert exactly
                    ///one row into the table, raise a constraint immediately instead of
                    ///incrementing a counter. This is necessary as the VM code is being
                    ///generated for will not open a statement transaction.  
                    ///</summary>
                    Debug.Assert(nIncr == 1);
                    build.sqlite3HaltConstraint(this, OnConstraintError.OE_Abort, "foreign key constraint failed", P4Usage.P4_STATIC);
                }
                else {
                    if (nIncr > 0 && pFKey.isDeferred == 0) {
                        sqliteinth.sqlite3ParseToplevel(this).mayAbort = 1;
                    }
                    v.sqlite3VdbeAddOp2(OpCode.OP_FkCounter, pFKey.isDeferred, nIncr);
                }
                v.sqlite3VdbeResolveLabel(iOk);
                v.sqlite3VdbeAddOp1(OpCode.OP_Close, iCur);
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
                  ///   Op | FK type   | Action taken
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
          void fkScanChildren(///
                                ///<summary>
                                ///Parse context 
                                ///</summary>
            SrcList pSrc,///
                         ///<summary>
                         ///SrcList containing the table to scan 
                         ///</summary>
           Table pTab, Index pIdx,///
                                   ///<summary>
                                   ///Foreign key index 
                                   ///</summary>
         FKey pFKey,///
                       ///<summary>
                       ///Foreign key relationship 
                       ///</summary>
         int[] aiCol,///
                        ///<summary>
                        ///Map from pIdx cols to child table cols 
                        ///</summary>
            int regData,///
                        ///<summary>
                        ///Referenced table data starts here 
                        ///</summary>
            int nIncr///
                     ///<summary>
                     ///Amount to increment deferred counter by 
                     ///</summary>
           ) {
                Connection db = this.db;
                ///
                ///<summary>
                ///Database handle 
                ///</summary>
                int i;
                ///
                ///<summary>
                ///Iterator variable 
                ///</summary>
                Expr pWhere = null;
                ///
                ///<summary>
                ///WHERE clause to scan with 
                ///</summary>
                NameContext sNameContext;
                ///
                ///<summary>
                ///Context used to resolve WHERE clause 
                ///</summary>
                WhereInfo pWInfo;
                ///
                ///<summary>
                ///Context used by sqlite3WhereXXX() 
                ///</summary>
                int iFkIfZero = 0;
                ///
                ///<summary>
                ///Address of  OpCode.OP_FkIfZero 
                ///</summary>
                Vdbe v = this.sqlite3GetVdbe();
                Debug.Assert(null == pIdx || pIdx.pTable == pTab);
                if (nIncr < 0) {
                    iFkIfZero = v.sqlite3VdbeAddOp2(OpCode.OP_FkIfZero, pFKey.isDeferred, 0);
                }
                ///
                ///<summary>
                ///Create an Expr object representing an SQL expression like:
                ///
                ///</summary>
                ///<param name="<parent">key2> ...</param>
                ///<param name=""></param>
                ///<param name="The collation sequence used for the comparison should be that of">The collation sequence used for the comparison should be that of</param>
                ///<param name="the parent key columns. The affinity of the parent key column should">the parent key columns. The affinity of the parent key column should</param>
                ///<param name="be applied to each child key value before the comparison takes place.">be applied to each child key value before the comparison takes place.</param>
                ///<param name=""></param>
                for (i = 0; i < pFKey.nCol; i++) {
                    Expr pLeft;
                    ///
                    ///<summary>
                    ///Value from parent table row 
                    ///</summary>
                    Expr pRight;
                    ///
                    ///<summary>
                    ///Column ref to child table 
                    ///</summary>
                    Expr pEq;
                    ///
                    ///<summary>
                    ///Expression (pLeft = pRight) 
                    ///</summary>
                    int iCol;
                    ///
                    ///<summary>
                    ///Index of column in child table 
                    ///</summary>
                    string zCol;
                    ///
                    ///<summary>
                    ///Name of column in child table 
                    ///</summary>
                    pLeft = exprc.sqlite3Expr(db, TokenType.TK_REGISTER, null);
                    if (pLeft != null) {
                        ///
                        ///<summary>
                        ///Set the collation sequence and affinity of the LHS of each TokenType.TK_EQ
                        ///expression to the parent key column defaults.  
                        ///</summary>
                        if (pIdx != null) {
                            Column pCol;
                            iCol = pIdx.aiColumn[i];
                            pCol = pTab.aCol[iCol];
                            if (pTab.iPKey == iCol)
                                iCol = -1;
                            pLeft.iTable = regData + iCol + 1;
                            pLeft.affinity = pCol.affinity;
                            pLeft.CollatingSequence = build.sqlite3LocateCollSeq(this, pCol.zColl);
                        }
                        else {
                            pLeft.iTable = regData;
                            pLeft.affinity = sqliteinth.SQLITE_AFF_INTEGER;
                        }
                    }
                    iCol = aiCol != null ? aiCol[i] : pFKey.aCol[0].iFrom;
                    Debug.Assert(iCol >= 0);
                    zCol = pFKey.pFrom.aCol[iCol].zName;
                    pRight = exprc.sqlite3Expr(db, TokenType.TK_ID, zCol);
                    pEq = this.sqlite3PExpr(TokenType.TK_EQ, pLeft, pRight, 0);
                    pWhere = exprc.sqlite3ExprAnd(db, pWhere, pEq);
                }
                ///
                ///<summary>
                ///If the child table is the same as the parent table, and this scan
                ///is taking place as part of a DELETE operation (operation D.2), omit the
                ///row being deleted from the scan by adding ($rowid != rowid) to the WHERE 
                ///clause, where $rowid is the rowid of the row being deleted.  
                ///</summary>
                if (pTab == pFKey.pFrom && nIncr > 0) {
                    Expr pEq;
                    ///
                    ///<summary>
                    ///Expression (pLeft = pRight) 
                    ///</summary>
                    Expr pLeft;
                    ///
                    ///<summary>
                    ///Value from parent table row 
                    ///</summary>
                    Expr pRight;
                    ///
                    ///<summary>
                    ///Column ref to child table 
                    ///</summary>
                    pLeft = exprc.sqlite3Expr(db, TokenType.TK_REGISTER, null);
                    pRight = exprc.sqlite3Expr(db, TokenType.TK_COLUMN, null);
                    if (pLeft != null && pRight != null) {
                        pLeft.iTable = regData;
                        pLeft.affinity = sqliteinth.SQLITE_AFF_INTEGER;
                        pRight.iTable = pSrc.a[0].iCursor;
                        pRight.iColumn = -1;
                    }
                    pEq = this.sqlite3PExpr(TokenType.TK_NE, pLeft, pRight, 0);
                    pWhere = exprc.sqlite3ExprAnd(db, pWhere, pEq);
                }
                ///
                ///<summary>
                ///Resolve the references in the WHERE clause. 
                ///</summary>
                sNameContext = new NameContext();
                // memset( &sNameContext, 0, sizeof( NameContext ) );
                sNameContext.pSrcList = pSrc;
                sNameContext.pParse = this;
                ResolveExtensions.sqlite3ResolveExprNames(sNameContext, ref pWhere);
                ///
                ///<summary>
                ///Create VDBE to loop through the entries in pSrc that match the WHERE
                ///clause. If the constraint is not deferred, throw an exception for
                ///each row found. Otherwise, for deferred constraints, increment the
                ///deferred constraint counter by nIncr for each row selected.  
                ///</summary>
                ExprList elDummy = null;
                pWInfo = this.sqlite3WhereBegin(pSrc, pWhere, ref elDummy, 0);
                if (nIncr > 0 && pFKey.isDeferred == 0) {
                    sqliteinth.sqlite3ParseToplevel(this).mayAbort = 1;
                }
                v.sqlite3VdbeAddOp2(OpCode.OP_FkCounter, pFKey.isDeferred, nIncr);
                if (pWInfo != null) {
                    pWInfo.sqlite3WhereEnd();
                }
                ///
                ///<summary>
                ///Clean up the WHERE clause constructed above. 
                ///</summary>
                exprc.Delete(db, ref pWhere);
                if (iFkIfZero != 0) {
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
          void sqlite3FkDropTable(SrcList pName, Table pTab) {
                Connection db = this.db;
                if ((db.flags & SqliteFlags.SQLITE_ForeignKeys) != 0 && !pTab.IsVirtual() && null == pTab.pSelect)
                {
                    int iSkip = 0;
                    Vdbe v = this.sqlite3GetVdbe();
                    Debug.Assert(v != null);
                    ///
                    ///<summary>
                    ///VDBE has already been allocated 
                    ///</summary>
                    if (fkeyc.sqlite3FkReferences(pTab) == null) {
                        ///
                        ///<summary>
                        ///Search for a deferred foreign key constraint for which this table
                        ///is the child table. If one cannot be found, return without 
                        ///generating any VDBE code. If one can be found, then jump over
                        ///the entire DELETE if there are no outstanding deferred constraints
                        ///when this statement is run.  
                        ///</summary>
                        FKey p;
                        for (p = pTab.pFKey; p != null; p = p.pNextFrom) {
                            if (p.isDeferred != 0)
                                break;
                        }
                        if (null == p)
                            return;
                        iSkip = v.sqlite3VdbeMakeLabel();
                        v.sqlite3VdbeAddOp2(OpCode.OP_FkIfZero, 1, iSkip);
                    }
                    this.disableTriggers = 1;
                    this.sqlite3DeleteFrom(exprc.sqlite3SrcListDup(db, pName, 0), null);
                    this.disableTriggers = 0;
                    ///
                    ///<summary>
                    ///If the DELETE has generated immediate foreign key constraint 
                    ///violations, halt the VDBE and return an error at this point, before
                    ///any modifications to the schema are made. This is because statement
                    ///transactions are not able to rollback schema changes.  
                    ///</summary>
                    v.sqlite3VdbeAddOp2(OpCode.OP_FkIfZero, 0, v.sqlite3VdbeCurrentAddr() + 2);
                    build.sqlite3HaltConstraint(this, OnConstraintError.OE_Abort, "foreign key constraint failed", P4Usage.P4_STATIC);
                    if (iSkip != 0) {
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
          void sqlite3FkCheck(///
                                ///<summary>
                                ///Parse context 
                                ///</summary>
            Table pTab,///
                       ///<summary>
                       ///Row is being deleted from this table 
                       ///</summary>
         int regOld,///
                       ///<summary>
                       ///Previous row data is stored here 
                       ///</summary>
         int regNew///
                      ///<summary>
                      ///New row data is stored here 
                      ///</summary>
          ) {
                Connection db = this.db;
                ///
                ///<summary>
                ///Database handle 
                ///</summary>
                FKey pFKey;
                ///
                ///<summary>
                ///Used to iterate through FKs 
                ///</summary>
                int iDb;
                ///
                ///<summary>
                ///Index of database containing pTab 
                ///</summary>
                string zDb;
                ///
                ///<summary>
                ///Name of database containing pTab 
                ///</summary>
                int isIgnoreErrors = this.disableTriggers;
                ///
                ///<summary>
                ///</summary>
                ///<param name="Exactly one of regOld and regNew should be non">zero. </param>
                Debug.Assert((regOld == 0) != (regNew == 0));
                ///
                ///<summary>
                ///</summary>
                ///<param name="If foreign">op. </param>
                if ((db.flags & SqliteFlags.SQLITE_ForeignKeys) == 0)
                    return;
                iDb = db.indexOfBackendWithSchema(pTab.pSchema);
                zDb = db.Backends[iDb].Name;
                ///
                ///<summary>
                ///Loop through all the foreign key constraints for which pTab is the
                ///child table (the table that the foreign key definition is part of).  
                ///</summary>
                for (pFKey = pTab.pFKey; pFKey != null; pFKey = pFKey.pNextFrom) {
                    Table pTo;
                    ///
                    ///<summary>
                    ///Parent table of foreign key pFKey 
                    ///</summary>
                    Index pIdx = null;
                    ///
                    ///<summary>
                    ///Index on key columns in pTo 
                    ///</summary>
                    int[] aiFree = null;
                    int[] aiCol;
                    int iCol;
                    int i;
                    int isIgnore = 0;
                    ///
                    ///<summary>
                    ///Find the parent table of this foreign key. Also find a unique index 
                    ///on the parent key columns in the parent table. If either of these 
                    ///schema items cannot be located, set an error in pParse and return 
                    ///early.  
                    ///</summary>
                    if (this.disableTriggers != 0) {
                        pTo = TableBuilder.sqlite3FindTable(db, zDb, pFKey.zTo);
                    }
                    else {
                        pTo = TableBuilder.sqlite3LocateTable(this, pFKey.zTo, zDb);
                    }
                    if (null == pTo || this.locateFkeyIndex(pTo, pFKey, out pIdx, out aiFree) != 0) {
                        if (0 == isIgnoreErrors///
                                               ///<summary>
                                               ///|| db.mallocFailed 
                                               ///</summary>
                     )
                            return;
                        continue;
                    }
                    Debug.Assert(pFKey.nCol == 1 || (aiFree != null && pIdx != null));
                    if (aiFree != null) {
                        aiCol = aiFree;
                    }
                    else {
                        iCol = pFKey.aCol[0].iFrom;
                        aiCol = new int[1];
                        aiCol[0] = iCol;
                    }
                    for (i = 0; i < pFKey.nCol; i++) {
                        if (aiCol[i] == pTab.iPKey) {
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
                    ///
                    ///<summary>
                    ///</summary>
                    ///<param name="Take a shared">lock on the parent table. Allocate </param>
                    ///<param name="a cursor to use to search the unique index on the parent key columns ">a cursor to use to search the unique index on the parent key columns </param>
                    ///<param name="in the parent table.  ">in the parent table.  </param>
                    sqliteinth.sqlite3TableLock(this, iDb, pTo.tnum, 0, pTo.zName);
                    this.nTab++;
                    if (regOld != 0) {
                        ///
                        ///<summary>
                        ///A row is being removed from the child table. Search for the parent.
                        ///If the parent does not exist, removing the child row resolves an 
                        ///outstanding foreign key constraint violation. 
                        ///</summary>
                        this.fkLookupParent(iDb, pTo, pIdx, pFKey, aiCol, regOld, -1, isIgnore);
                    }
                    if (regNew != 0) {
                        ///
                        ///<summary>
                        ///A row is being added to the child table. If a parent row cannot
                        ///be found, adding the child row has violated the FK constraint. 
                        ///</summary>
                        this.fkLookupParent(iDb, pTo, pIdx, pFKey, aiCol, regNew, +1, isIgnore);
                    }
                    db.sqlite3DbFree(ref aiFree);
                }
                ///
                ///<summary>
                ///Loop through all the foreign key constraints that refer to this table 
                ///</summary>
                for (pFKey = fkeyc.sqlite3FkReferences(pTab); pFKey != null; pFKey = pFKey.pNextTo) {
                    Index pIdx = null;
                    ///
                    ///<summary>
                    ///Foreign key index for pFKey 
                    ///</summary>
                    SrcList pSrc;
                    int[] aiCol = null;
                    if (0 == pFKey.isDeferred && null == this.pToplevel && 0 == this.isMultiWrite) {
                        Debug.Assert(regOld == 0 && regNew != 0);
                        ///
                        ///<summary>
                        ///Inserting a single row into a parent table cannot cause an immediate
                        ///foreign key violation. So do nothing in this case.  
                        ///</summary>
                        continue;
                    }
                    if (this.locateFkeyIndex(pTab, pFKey, out pIdx, out aiCol) != 0) {
                        if (0 == isIgnoreErrors///
                                               ///<summary>
                                               ///|| db.mallocFailed 
                                               ///</summary>
                     )
                            return;
                        continue;
                    }
                    Debug.Assert(aiCol != null || pFKey.nCol == 1);
                    ///
                    ///<summary>
                    ///Create a SrcList structure containing a single table (the table 
                    ///the foreign key that refers to this table is attached to). This
                    ///is required for the sqlite3WhereXXX() interface.  
                    ///</summary>
                    pSrc = build.sqlite3SrcListAppend(db, 0, null, null);
                    if (pSrc != null) {
                        SrcList_item pItem = pSrc.a[0];
                        pItem.pTab = pFKey.pFrom;
                        pItem.zName = pFKey.pFrom.zName;
                        pItem.pTab.nRef++;
                        pItem.iCursor = this.nTab++;
                        if (regNew != 0) {
                            this.fkScanChildren(pSrc, pTab, pIdx, pFKey, aiCol, regNew, -1);
                        }
                        if (regOld != 0) {
                            ///
                            ///<summary>
                            ///If there is a RESTRICT action configured for the current operation
                            ///on the parent table of this FK, then throw an exception 
                            ///immediately if the FK constraint is violated, even if this is a
                            ///deferred trigger. That's what RESTRICT means. To defer checking
                            ///the constraint, the FK should specify NO ACTION (represented
                            ///using OnConstraintError.OE_None). NO ACTION is the default.  
                            ///</summary>
                            this.fkScanChildren(pSrc, pTab, pIdx, pFKey, aiCol, regOld, 1);
                        }
                        pItem.zName = null;
                        build.sqlite3SrcListDelete(db, ref pSrc);
                    }
                    db.sqlite3DbFree(ref aiCol);
                }
            }

            ///<summary>
            /// This function is called before generating code to update or delete a
            /// row contained in table pTab.
            ///</summary>
            public u32 sqlite3FkOldmask(///
                Table pTab///Table being modified 
			) {
                u32 mask = 0;
                if ((this.db.flags & SqliteFlags.SQLITE_ForeignKeys) != 0)
                {
                    FKey p;
                    int i;
                    for (p = pTab.pFKey; p != null; p = p.pNextFrom) {
                        for (i = 0; i < p.nCol; i++)
                            mask |= fkeyc.COLUMN_MASK(p.aCol[i].iFrom);
                    }
                    for (p = fkeyc.sqlite3FkReferences(pTab); p != null; p = p.pNextTo) {
                        Index pIdx;
                        int[] iDummy;
                        this.locateFkeyIndex(pTab, p, out pIdx, out iDummy);
                        if (pIdx != null) {
                            for (i = 0; i < pIdx.nColumn; i++)
                                mask |= fkeyc.COLUMN_MASK(pIdx.aiColumn[i]);
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
          bool sqlite3FkRequired(
            Table pTab,///
                       ///<summary>
                       ///Table being modified 
                       ///</summary>
         int[] aChange,///
                       ///<summary>
                       ///</summary>
                       ///<param name="Non">NULL for UPDATE operations </param>
          int chngRowid///
                       ///<summary>
                       ///True for UPDATE that affects rowid 
                       ///</summary>
           )
            {
                bool res = false;

                if ((this.db.flags & SqliteFlags.SQLITE_ForeignKeys) != 0)
                {
                    if (null == aChange)
                    {
                        ///A DELETE operation. Foreign key processing is required if the 
                        ///table in question is either the child or parent table for any 
                        ///foreign key constraint.  
                        return (fkeyc.sqlite3FkReferences(pTab) != null || pTab.pFKey != null);
                    }
                    else {
                        ///This is an UPDATE. Foreign key processing is only required if the
						///operation modifies one or more child or parent key columns. 
                        ///Check if any child key columns are being modified. 
                        pTab.pFKey.path(x => x.pNextFrom).ForEach(p =>
                        {
                            for (var i = 0; i < p.nCol; i++)
                            {
                                int iChildKey = p.aCol[i].iFrom;
                                if (aChange[iChildKey] >= 0)
                                    res = true;
                                if (iChildKey == pTab.iPKey && chngRowid != 0)
                                    res = true;
                            }
                        }
                        );
                        ///Check if any parent key columns are being modified. 
                        fkeyc.sqlite3FkReferences(pTab).path(x => x.pNextFrom).ForEach(p =>
                        {
                            for (var i = 0; i < p.nCol; i++)
                            {
                                string zKey = p.aCol[i].zCol;
                                for (var iKey = 0; iKey < pTab.nCol; iKey++)
                                {
                                    Column pCol = pTab.aCol[iKey];
                                    if ((!String.IsNullOrEmpty(zKey) ? pCol.zName.Equals(zKey, StringComparison.InvariantCultureIgnoreCase) : pCol.isPrimKey != 0))
                                    {
                                        if (aChange[iKey] >= 0)
                                            res = true;
                                        if (iKey == pTab.iPKey && chngRowid != 0)
                                            res = true;
                                    }
                                }
                            }
                        });
                    }
                }
                return res;
            }
            ///<summary>
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
            public Trigger fkActionTrigger(
			    Table pTab,///Table being updated or deleted from 
			    FKey pFKey,///Foreign key to get action for 			
			    ExprList pChanges///Change list for UPDATE, NULL for DELETE 
			) {
				Connection db=this.db;///Database handle 				
				int iAction=(pChanges!=null)?1:0;///1 for UPDATE, 0 for DELETE 
				var action=pFKey.aAction[iAction];///One of OnConstraintError.OE_None, OnConstraintError.OE_Cascade etc. 
				var pTrigger = pFKey.apTrigger[iAction];///Trigger definition to return 
				if (action!=OnConstraintError.OE_None&&null==pTrigger) {
					u8 enableLookaside;
					///Copy of db.lookaside.bEnabled 
					string zFrom;
					///Name of child table 
					int nFrom;
					///Length in bytes of zFrom 
					Index pIdx=null;
					///Parent key index for this FK 
					int[] aiCol=null;
					///child table cols . parent key cols 
					TriggerStep pStep=null;
					///First (only) step of trigger program 
					Expr pWhere=null;
					///WHERE clause of trigger step 
					ExprList pList=null;
					///Changes list if ON UPDATE CASCADE 
					Select pSelect=null;
					///If RESTRICT, "SELECT RAISE(...)" 
					Expr pWhen=null;
					///
					///<summary>
					///WHEN clause for the trigger 
					///</summary>
					if(this.locateFkeyIndex(pTab,pFKey,out pIdx,out aiCol)!=0)
						return null;
					Debug.Assert(aiCol!=null||pFKey.nCol==1);
					for(var i=0;i<pFKey.nCol;i++) {
						Token tOld=new Token("old",3);
						///Literal "old" token 
						Token tNew=new Token("new",3);
						///Literal "new" token 
						Token tFromCol=new Token();
						///Name of column in child table 
						Token tToCol=new Token();
						///Name of column in parent table 
						int iFromCol;
						///Idx of column in child table 
						Expr pEq;
						///tFromCol = OLD.tToCol 
						iFromCol=aiCol!=null?aiCol[i]:pFKey.aCol[0].iFrom;
						Debug.Assert(iFromCol>=0);
						tToCol.zRestSql=pIdx!=null?pTab.aCol[pIdx.aiColumn[i]].zName:"oid";
						tFromCol.zRestSql=pFKey.pFrom.aCol[iFromCol].zName;
						tToCol.Length=StringExtensions.Strlen30(tToCol.zRestSql);
						tFromCol.Length=StringExtensions.Strlen30(tFromCol.zRestSql);
						///Create the expression "OLD.zToCol = zFromCol". It is important
						///that the "OLD.zToCol" term is on the LHS of the = operator, so
						///that the affinity and collation sequence associated with the
						///parent table are used for the comparison. 
						pEq=this.sqlite3PExpr(TokenType.TK_EQ,this.sqlite3PExpr(TokenType.TK_DOT,this.sqlite3PExpr(TokenType.TK_ID,null,null,tOld),this.sqlite3PExpr(TokenType.TK_ID,null,null,tToCol),0),this.sqlite3PExpr(TokenType.TK_ID,null,null,tFromCol),0);
						pWhere=exprc.sqlite3ExprAnd(db,pWhere,pEq);
						///For ON UPDATE, construct the next term of the WHEN clause.
						///The final WHEN clause will be like this:
						///
						///WHEN NOT(old.col1 IS new.col1 AND ... AND old.colN IS new.colN)
						if(pChanges!=null) {
							pEq=this.sqlite3PExpr(TokenType.TK_IS,this.sqlite3PExpr(TokenType.TK_DOT,this.sqlite3PExpr(TokenType.TK_ID,null,null,tOld),this.sqlite3PExpr(TokenType.TK_ID,null,null,tToCol),0),this.sqlite3PExpr(TokenType.TK_DOT,this.sqlite3PExpr(TokenType.TK_ID,null,null,tNew),this.sqlite3PExpr(TokenType.TK_ID,null,null,tToCol),0),0);
							pWhen=exprc.sqlite3ExprAnd(db,pWhen,pEq);
						}
						if(action!=OnConstraintError.OE_Restrict&&(action!=OnConstraintError.OE_Cascade||pChanges!=null)) {
							Expr pNew;
							if(action==OnConstraintError.OE_Cascade) {
								pNew=this.sqlite3PExpr(TokenType.TK_DOT,this.sqlite3PExpr(TokenType.TK_ID,null,null,tNew),this.sqlite3PExpr(TokenType.TK_ID,null,null,tToCol),0);
							}
							else
								if(action==OnConstraintError.OE_SetDflt) {
									Expr pDflt=pFKey.pFrom.aCol[iFromCol].DefaultValue;
									if(pDflt!=null) {
										pNew=exprc.sqlite3ExprDup(db,pDflt,0);
									}
									else {
										pNew=this.sqlite3PExpr(TokenType.TK_NULL,0,0,0);
									}
								}
								else {
									pNew=this.sqlite3PExpr(TokenType.TK_NULL,0,0,0);
								}
							pList= pList.Append(pNew);
							this.sqlite3ExprListSetName(pList,tFromCol,0);
						}
					}
					db.sqlite3DbFree(ref aiCol);
					zFrom=pFKey.pFrom.zName;
					nFrom=StringExtensions.Strlen30(zFrom);
					if(action==OnConstraintError.OE_Restrict) {
						Token tFrom=new Token();
						Expr pRaise;
						tFrom.zRestSql=zFrom;
						tFrom.Length=nFrom;
						pRaise=exprc.sqlite3Expr(db,TokenType.TK_RAISE,"foreign key constraint failed");
						if(pRaise!=null) {
							pRaise.affinity=(char)OnConstraintError.OE_Abort;
						}
						pSelect=Select.Create(this,CollectionExtensions.Append(null,pRaise),build.sqlite3SrcListAppend(db,0,tFrom,null),pWhere,null,null,null,0,null,null);
						pWhere=null;
					}
					///
					///<summary>
					///Disable lookaside memory allocation 
					///</summary>
					enableLookaside=db.lookaside.bEnabled;
					db.lookaside.bEnabled=0;
					pTrigger=new Trigger();
					//(Trigger*)sqlite3DbMallocZero( db,
					//     sizeof( Trigger ) +         /* struct Trigger */
					//     sizeof( TriggerStep ) +     /* Single step in trigger program */
					//     nFrom + 1                 /* Space for pStep.target.z */
					// );
					//if ( pTrigger )
					{
						pStep=pTrigger.step_list=new TriggerStep();
						// = (TriggerStep)pTrigger[1];
						//pStep.target.z = pStep[1];
						pStep.target.Length=nFrom;
						pStep.target.zRestSql=zFrom;
						// memcpy( (char*)pStep.target.z, zFrom, nFrom );
						pStep.pWhere=exprc.sqlite3ExprDup(db,pWhere,EXPRDUP_REDUCE);
						pStep.pExprList=exprc.sqlite3ExprListDup(db,pList,EXPRDUP_REDUCE);
						pStep.pSelect=exprc.sqlite3SelectDup(db,pSelect,EXPRDUP_REDUCE);
						if(pWhen!=null) {
							pWhen=this.sqlite3PExpr(TokenType.TK_NOT,pWhen,0,0);
							pTrigger.pWhen=exprc.sqlite3ExprDup(db,pWhen,EXPRDUP_REDUCE);
						}
						///
						///<summary>
						///</summary>
						///<param name="Re">enable the lookaside buffer, if it was disabled earlier. </param>
						//if ( db.mallocFailed == 1 )
						//{
						//  fkTriggerDelete( db, pTrigger );
						//  return 0;
						//}
					}
					db.lookaside.bEnabled=enableLookaside;
					exprc.Delete(db,ref pWhere);
					exprc.Delete(db,ref pWhen);
					exprc.Delete(db,ref pList);
					SelectMethods.SelectDestructor(db,ref pSelect);
					switch(action) {
					case OnConstraintError.OE_Restrict:
					pStep.Operator = TokenType.TK_SELECT;
					break;
					case OnConstraintError.OE_Cascade:
					if(null==pChanges) {
						pStep.Operator= TokenType.TK_DELETE;
						break;
					}
					goto default;
					default:
					pStep.Operator = TokenType.TK_UPDATE;
					break;
					}
					pStep.pTrig=pTrigger;
					pTrigger.pSchema=pTab.pSchema;
					pTrigger.pTabSchema=pTab.pSchema;
					pFKey.apTrigger[iAction]=pTrigger;
					pTrigger.op=(byte)(pChanges!=null?TokenType.TK_UPDATE:TokenType.TK_DELETE);
				}
				return pTrigger;
			}
            ///<summary>
            /// This function is called when deleting or updating a row to implement
            /// any required CASCADE, SET NULL or SET DEFAULT actions.
            ///
            ///</summary>
            public void sqlite3FkActions(
                Table pTab,///Table being updated or deleted from 
			    ExprList pChanges,///<param name="Change">list for UPDATE, NULL for DELETE </param>
			    int regOld///Address of array containing old row 
			) {
				///<param name="If foreign">key support is enabled, iterate through all FKs that </param>
				///<param name="refer to table pTab. If there is an action a6ssociated with the FK ">refer to table pTab. If there is an action a6ssociated with the FK </param>
				///<param name="for this operation (either update or delete), invoke the associated ">for this operation (either update or delete), invoke the associated </param>
				///<param name="trigger sub">program.  </param>
                if ((this.db.flags & SqliteFlags.SQLITE_ForeignKeys) != 0)
                {
					FKey pFKey;
					///Iterator variable 
					for(pFKey=fkeyc.sqlite3FkReferences(pTab);pFKey!=null;pFKey=pFKey.pNextTo) {
						Trigger pAction=this.fkActionTrigger(pTab,pFKey,pChanges);
						if(pAction!=null) {
                            TriggerBuilder.sqlite3CodeRowTriggerDirect(this,pAction,pTab,regOld,OnConstraintError.OE_Abort,0);
						}
					}
				}
			}

            IEnumerable<Token> lex(String zSql)
            {
                int i = 0;///Loop counter 

                int mxSqlLen;///Max length of an SQL string 
                mxSqlLen = db.aLimit[Globals.SQLITE_LIMIT_SQL_LENGTH];

                while (
                    ///0 == db.mallocFailed && 
                i < zSql.Length)
                {
                    Debug.Assert(i >= 0);

                    //pParse->sLastToken.z = &zSql[i];
                    i += (this.sLastToken = Lexer.GetNextToken(zSql, i)).Length;
                    //Log.WriteLine("token :" + this.sLastToken);
                    if (i > mxSqlLen)
                    {
                        this.rc = SqlResult.SQLITE_TOOBIG;
                        break;
                    }

                    yield return this.sLastToken;
                }
            }



            static List<Tuple<List<TokenType>, ConsoleColor>> colors = new List<Tuple<List<TokenType>, ConsoleColor>>(){
        new Tuple<List<TokenType>,ConsoleColor>(
            new List<TokenType>{TokenType.TK_SELECT,TokenType.TK_FROM,TokenType.TK_WHERE,TokenType.TK_OR,TokenType.TK_ORDER,TokenType.TK_DISTINCT,TokenType.TK_CREATE,TokenType.TK_TABLE},
            ConsoleColor.Cyan
        )
        ,
        new Tuple<List<TokenType>,ConsoleColor>(
            new List<TokenType>{TokenType.TK_SEMI,TokenType.TK_UMINUS,TokenType.TK_COMMA,TokenType.TK_LP,TokenType.TK_RP},
            ConsoleColor.Red
        )
        ,
        new Tuple<List<TokenType>,ConsoleColor>(
            new List<TokenType>{TokenType.TK_INTEGER,TokenType.TK_STRING},
            ConsoleColor.White
        )
        ,
        new Tuple<List<TokenType>,ConsoleColor>(
            new List<TokenType>{TokenType.TK_ID,TokenType.TK_STAR},
            ConsoleColor.Yellow
        )
        ,
        new Tuple<List<TokenType>,ConsoleColor>(
            System.Enum.GetValues(typeof(TokenType)).Cast<TokenType>().ToList(),
            ConsoleColor.Gray
        )
    };
            

            

            static void print(Token token)
            {
                TokenType tk = token.TokenType;
                String tkn = token.Text;
                var tkString = (tk == TokenType.TK_SPACE ? "." : tk.ToString().Replace("TK_", String.Empty));
                int length = Math.Max(tkn.Length, tkString.Length + 2);
                if ((length + Console.CursorLeft) >= Console.BufferWidth) {
                    Console.CursorLeft = 0;
                    Console.CursorTop += 3;
                    Console.Write(">");
                }
                var clr = Console.ForegroundColor;
                Console.ForegroundColor = colors.First(x => x.Item1.Contains(tk)).Item2;
                var left = Console.CursorLeft;
                Console.Write(tkn.ToString().pad( length).ToLower());
                Console.CursorLeft = left;
                Console.CursorTop += 2;
                Console.CursorLeft = left;
                //Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.Write(tkString.pad( length).ToLower());
                Console.CursorTop -= 2;
                Console.ForegroundColor = clr;
            }

			public SqlResult sqlite3RunParser(string zSql,ref string pzErrMsg) {
				Log.WriteHeader("sqlite3RunParser:"+zSql);
				//Log.Indent();
                SqlResult nErr = (SqlResult)0;///Number of errors encountered 
				ParseMethods.yyParser pEngine;///type of the next token 
				var lastTokenParsed=-1;///type of the previous token 
				byte enableLookaside;///<param name="Saved value of db">>lookaside.bEnabled </param>
				Connection db=this.db;///The database connection 
								if(db.activeVdbeCnt==0) {
					db.u1.isInterrupted=false;
				}
				this.rc=SqlResult.SQLITE_OK;
				Debug.Assert(pzErrMsg!=null);
				pEngine=ParseMethods.sqlite3ParserAlloc();
				//sqlite3ParserAlloc((void*(*)(size_t))malloc_cs.sqlite3Malloc);
				//if ( pEngine == null ){ db.mallocFailed = 1;return SQLITE_NOMEM;}

				Debug.Assert(this.pNewTable==null);
				Debug.Assert(this.pNewTrigger==null);
				Debug.Assert(this.nVar==0);
				Debug.Assert(this.nzVar==0);
				Debug.Assert(this.azVar==null);
				enableLookaside=db.lookaside.bEnabled;
				if(db.lookaside.pStart!=0)
					db.lookaside.bEnabled=1;


                Console.Clear();
                var tokens = lex(zSql).ToArray();
                tokens.ForEach(t=>Shell.print(t.TokenType,t.Text));
                
                int i = 0;
				foreach(Token token in tokens){
                    i += token.Length;
					switch(token.TokenType) {
					case TokenType.TK_SPACE: {
						if(db.u1.isInterrupted) {
							utilc.sqlite3ErrorMsg(this,"interrupt");
							this.rc=SqlResult.SQLITE_INTERRUPT;
							goto abort_parse;
						}
						break;
					}
					case TokenType.TK_ILLEGAL: {
						db.DbFree(ref pzErrMsg);
						pzErrMsg=io.sqlite3MPrintf(db,"unrecognized token: \"%T\"",(object)token);
						nErr++;
						goto abort_parse;
					}
					case TokenType.TK_SEMI: {
						//pParse.zTail = new StringBuilder(zSql.Substring( i,zSql.Length-i ));
						///Fall thru into the default case 
						goto default;
					}
					default: {
						pEngine.sqlite3Parser(token.TokenType,token,this);
						if(this.rc!=SqlResult.SQLITE_OK) {
							goto abort_parse;
						}
						break;
					}
					}
				}
                this.zTail = new StringBuilder(zSql);

				abort_parse:
				this.zTail=new StringBuilder(zSql.Length<=i?"":zSql.Substring(i,zSql.Length-i));
				if(zSql.Length>=i&&nErr==0&&this.rc==SqlResult.SQLITE_OK) {
					if((TokenType)lastTokenParsed!=TokenType.TK_SEMI) {
						pEngine.sqlite3Parser(TokenType.TK_SEMI,this.sLastToken,this);
					}
					pEngine.sqlite3Parser(0,this.sLastToken,this);
				}
				#if YYTRACKMAXSTACKDEPTH
sqlite3StatusSet(SQLITE_STATUS_PARSER_STACK,
sqlite3ParserStackPeak(pEngine)
);
#endif
				pEngine.sqlite3ParserFree(null);
				//malloc_cs.sqlite3_free );
				db.lookaside.bEnabled=enableLookaside;
				//if ( db.mallocFailed != 0 )
				//{
				//  pParse.rc = SQLITE_NOMEM;
				//}
				if(this.rc!=SqlResult.SQLITE_OK&&this.rc!=SqlResult.SQLITE_DONE&&this.zErrMsg=="") {
					malloc_cs.sqlite3SetString(ref this.zErrMsg,db, this.rc.sqlite3ErrStr());
				}
				//assert( pzErrMsg!=0 );
				if(this.zErrMsg!=null) {
					pzErrMsg=this.zErrMsg;
					io.sqlite3_log(this.rc,"%s",pzErrMsg);
					this.zErrMsg="";
					nErr++;
				}
				if(this.pVdbe!=null&&this.nErr>0&&this.nested==0) {
                    vdbeaux.sqlite3VdbeDelete(ref this.pVdbe);
                    this.pVdbe = this.pVdbe;//trigger setter
					this.pVdbe=null;
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
				this.apVtabLock=null;
				//malloc_cs.sqlite3_free( pParse.apVtabLock );
				#endif
				if(!sqliteinth.IN_DECLARE_VTAB(this)) {
					///
					///<summary>
					///If the pParse.declareVtab flag is set, do not delete any table
					///structure built up in pParse.pNewTable. The calling code (see vtab.c)
					///will take responsibility for freeing the Table structure.
					///
					///</summary>
					TableBuilder.sqlite3DeleteTable(db,ref this.pNewTable);
				}
#if !SQLITE_OMIT_TRIGGER
                TriggerParser.sqlite3DeleteTrigger(db,ref this.pNewTrigger);
				#endif
				//for ( i = pParse.nzVar - 1; i >= 0; i-- )
				//  sqlite3DbFree( db, pParse.azVar[i] );
				db.DbFree(ref this.azVar);
				db.sqlite3DbFree(ref this.aAlias);
				while(this.pAinc!=null) {
					AutoincInfo p=this.pAinc;
					this.pAinc=p.pNext;
					db.DbFree(ref p);
				}
				while(this.pZombieTab!=null) {
					Table p=this.pZombieTab;
					this.pZombieTab=p.pNextZombie;
					TableBuilder.sqlite3DeleteTable(db,ref p);
				}
				if(nErr>0&&this.rc==SqlResult.SQLITE_OK) {
					this.rc=SqlResult.SQLITE_ERROR;
				}
				//Log.Unindent();
				return nErr;
			}
			public void sqlite3Insert(SrcList pTabList,int null_3,int null_4,IdList pColumn,OnConstraintError onError) {
				this.sqlite3Insert(pTabList,null,null,pColumn,onError);
			}
			public void sqlite3Insert(SrcList pTabList,int null_3,Select pSelect,IdList pColumn,OnConstraintError onError) {
				this.sqlite3Insert(pTabList,null,pSelect,pColumn,onError);
			}
			public void sqlite3Insert(SrcList pTabList,ExprList pList,int null_4,IdList pColumn,OnConstraintError onError) {
				this.sqlite3Insert(pTabList,pList,null,pColumn,onError);
			}
			public void sqlite3Insert(
			    SrcList pTabList,///Name of table into which we are inserting 
			    ExprList pList,///List of values to be inserted 
			    Select pSelect,///A SELECT statement to use as the data source 
			    IdList pColumn,///Column names corresponding to IDLIST. 
			    OnConstraintError onError///How to handle constraint errors 
			) {
				int i=0;
				int j=0;
				
				int nColumn;
				///Number of columns in the data 
				int nHidden=0;
				///Number of hidden columns if TABLE is virtual 
				int baseCur=0;
				///VDBE VdbeCursor number for pTab 
				int keyColumn=-1;
				///Column that is the INTEGER PRIMARY KEY 
				int endOfLoop=0;
				///Label for the end of the insertion loop 
				bool useTempTable=false;
				///Store SELECT results in intermediate table 
				int srcTab=0;
				///Data comes from this temporary cursor if >=0 
				int addrInsTop=0;
				///Jump to label "D" 
				int addrCont=0;
				///Top of insert loop. Label "C" in templates 3 and 4 
				int addrSelect=0;
				///Address of coroutine that implements the SELECT 
				SelectDest dest;
				///Destination for SELECT on rhs of INSERT 
				
				
				bool appendFlag=false;
				///True if the insert is likely to be an append 
				///
				///Register allocations 
				int regFromSelect=0;
				///Base register for data coming from SELECT 
				int regAutoinc=0;
				///Register holding the AUTOINCREMENT counter 
				int regRowCount=0;
				///Memory cell used for the row counter 
				int regIns;
				///Block of regs holding rowid+data being inserted 
				int regRowid;
				///registers holding insert rowid 
				int regData;
				///register holding first column to insert 
				int regEof=0;
				///Register recording end of SELECT data 
				int[] aRegIdx=null;
				///One register allocated to each index 
				#if !SQLITE_OMIT_TRIGGER
				
				
				TriggerType tmask=0;
				///Mask of trigger times 
				#endif
				var db=this.db;///The main database structure 
				dest = new SelectDest();
				// memset( &dest, 0, sizeof( dest ) );
				if(this.nErr!=0///
				///<summary>
				///|| db.mallocFailed != 0 
				///</summary>
				) {
					goto insert_cleanup;
				}
				///Locate the table into which we will be inserting new information.
				Debug.Assert(pTabList.Count==1);
				var tableName=pTabList.a[0].zName;///Name of the table into which we are inserting 
				if (NEVER(tableName==null))
					goto insert_cleanup;
				var refTable=this.GetFirstTableInTheList(pTabList);///The table to insert into.  aka TABLE 
				if (refTable==null) {
					goto insert_cleanup;
				}

				var iDb=db.indexOfBackendWithSchema(refTable.pSchema);///Index of database holding TABLE 				
                                                                      
                Debug.Assert(iDb<db.BackendCount);
				var refDb=db.Backends[iDb];///The database containing table being inserted into 
				var dbName =refDb.Name;///Name of the database holding this table 
#if !SQLITE_OMIT_AUTHORIZATION
																																																																																																					if( sqlite3AuthCheck(pParse, SQLITE_INSERT, pTab.zName, 0, zDb) ){
goto insert_cleanup;
}
#endif
                ///Figure out if we have any triggers and if the table being
                ///inserted into is a view
#if !SQLITE_OMIT_TRIGGER
                var pTrigger = TriggerParser.sqlite3TriggersExist(this,refTable,TokenType.TK_INSERT,null,out tmask);///True if attempting to insert into a view                 
                var isView =refTable.IsView;///List of triggers on pTab, if required 
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
                Debug.Assert((pTrigger!=null&&tmask!=0)||(pTrigger==null&&tmask==0));
				#endif
				#if !SQLITE_OMIT_VIEW
				///If pTab is really a view, make sure it has been initialized.
				///ViewGetColumnNames() is a no-op if pTab is not a view (or virtual
				///module table).">module table).
				if(build.sqlite3ViewGetColumnNames(this,refTable)!=-0) {
					goto insert_cleanup;
				}
				#endif
				///Ensure that:
				///(a) the table is not read-only, 
				///(b) that if it is a view then ON INSERT triggers exist">(b) that if it is a view then ON INSERT triggers exist
				if(this.sqlite3IsReadOnly(refTable,tmask)) {
					goto insert_cleanup;
				}
				///Allocate a VDBE
				var v=this.sqlite3GetVdbe();///Generate code into this virtual machine 
				if (v==null)
					goto insert_cleanup;
				if(this.nested==0)
					v.sqlite3VdbeCountChanges();
				build.sqlite3BeginWriteOperation(this,(null != pSelect||null != pTrigger)?1:0,iDb);
				#if !SQLITE_OMIT_XFER_OPT
				///If the statement is of the form
				///
				///INSERT INTO <table1> SELECT * FROM <table2>;
				///
				///Then special optimizations can be applied that make the transfer
				///very fast and which reduce fragmentation of indices.
				///
				///This is the 2nd template.
				if(pColumn==null&&xferOptimization(this,refTable,pSelect,onError,iDb)!=0) {
					Debug.Assert(null==pTrigger);
					Debug.Assert(pList==null);
					goto insert_end;
				}
				#endif
				///If this is an AUTOINCREMENT table, look up the sequence number in the
				///sqlite_sequence table and store it in memory cell regAutoinc.
				regAutoinc=this.autoIncBegin(iDb,refTable);
				///Figure out how many columns of data are supplied.  If the data
				///is coming from a SELECT statement, then generate a co-routine that
				///produces a single row of the SELECT on each invocation.  The-produces a single row of the SELECT on each invocation.  The
				///co">routine is the common header to the 3rd and 4th templates.
				if(pSelect!=null) {
					///Data is coming from a SELECT.  Generate code to implement that SELECT
					///as a co">routine.  The code is common to both the 3rd and 4th</param>
					///templates:">templates:</param>
					///"></param>
					///EOF <"> 0</param>
					///X <"> A</param>
					///goto B">goto B</param>
					///A: setup for the SELECT">A: setup for the SELECT</param>
					///loop over the tables in the SELECT">loop over the tables in the SELECT</param>
					///load value into register R..R+n">load value into register R..R+n</param>
					///yield X">yield X</param>
					///end loop">end loop</param>
					///cleanup after the SELECT">cleanup after the SELECT</param>
					///EOF <"> 1</param>
					///yield X">yield X</param>
					///halt">error</param>
					///"></param>
					///On each invocation of the co">routine, it puts a single row of the</param>
					///SELECT result into registers dest.iMem...dest.iMem+dest.nMem">1.</param>
					///(These output registers are allocated by sqlite3Select().)  When">(These output registers are allocated by sqlite3Select().)  When</param>
					///the SELECT completes, it sets the EOF flag stored in regEof.">the SELECT completes, it sets the EOF flag stored in regEof.</param>
					var rc=(SqlResult)0;
                    int j1;
					regEof=++this.UsedCellCount;
					v.sqlite3VdbeAddOp2(OpCode.OP_Integer,0,regEof);
					///<param name="EOF <"> 0 </param>
					#if SQLITE_DEBUG
																																																																																																																																			        VdbeComment( v, "SELECT eof flag" );
#endif
                    dest.Init(SelectResultType.Coroutine, ++this.UsedCellCount);
					addrSelect=v.sqlite3VdbeCurrentAddr()+2;
					v.sqlite3VdbeAddOp2(OpCode.OP_Integer,addrSelect-1,dest.iParm);
					j1=v.sqlite3VdbeAddOp2(OpCode.OP_Goto,0,0);
					#if SQLITE_DEBUG
																																																																																																																																			        VdbeComment( v, "Jump over SELECT coroutine" );
#endif
					///Resolve the expressions in the SELECT statement and execute it. 
					rc=Select.sqlite3Select(this,pSelect,ref dest);
					Debug.Assert(this.nErr==0||rc!=0);
					if(rc!=0||NEVER(this.nErr!=0)///
					///<summary>
					///|| db.mallocFailed != 0 
					///</summary>
					) {
						goto insert_cleanup;
					}
					v.sqlite3VdbeAddOp2(OpCode.OP_Integer,1,regEof);
					///<param name="EOF <"> 1 </param>
					v.sqlite3VdbeAddOp1(OpCode.OP_Yield,dest.iParm);
					///yield X 
                    v.sqlite3VdbeAddOp2(OpCode.OP_Halt, (int)SqlResult.SQLITE_INTERNAL, (int)OnConstraintError.OE_Abort);
					#if SQLITE_DEBUG
																																																																																																																																			        VdbeComment( v, "End of SELECT coroutine" );
#endif
					v.sqlite3VdbeJumpHere(j1);
					///label B: 
					regFromSelect=dest.iMem;
					Debug.Assert(pSelect.ResultingFieldList!=null);
					nColumn=pSelect.ResultingFieldList.Count;
					Debug.Assert(dest.nMem==nColumn);
					///
					///<summary>
					///Set useTempTable to TRUE if the result of the SELECT statement
					///should be written into a temporary table (template 4).  Set to
					///FALSE if each* row of the SELECT can be written directly into
					///the destination table (template 3).
					///
					///A temp table must be used if the table being updated is also one
					///of the tables being read by the SELECT statement.  Also use a
					///temp table in the case of row triggers.
					///
					///</summary>
					if(pTrigger!=null||this.readsTable(addrSelect,iDb,refTable)) {
						useTempTable=true;
					}
					if(useTempTable) {
						///Invoke the coroutine to extract information from the SELECT
						///and add it to a transient table srcTab.  The code generated
						///here is from the 4th template:
						///
						///B: open temp table
						///L: yield X
						///if EOF goto M
						///insert row from R..R+n into temp table
						///goto L
						///M: ...
						int regRec;
						///Register to hold packed record 
						int regTempRowid;
						///Register to hold temp table ROWID 
						int addrTop;
						///Label "L" 
						int addrIf;
						///Address of jump to M 
						srcTab=this.nTab++;
						regRec=this.allocTempReg();
						regTempRowid=this.allocTempReg();
                        v.sqlite3VdbeAddOp2(OpCode.OP_OpenEphemeral, srcTab, nColumn);
						addrTop=v.sqlite3VdbeAddOp1(OpCode.OP_Yield,dest.iParm);
						addrIf=v.sqlite3VdbeAddOp1(OpCode.OP_If,regEof);
						v.sqlite3VdbeAddOp3( OpCode.OP_MakeRecord,regFromSelect,nColumn,regRec);
						v.sqlite3VdbeAddOp2( OpCode.OP_NewRowid,srcTab,regTempRowid);
						v.sqlite3VdbeAddOp3( OpCode.OP_Insert,srcTab,regRec,regTempRowid);
						v.sqlite3VdbeAddOp2(OpCode.OP_Goto,0,addrTop);
						v.sqlite3VdbeJumpHere(addrIf);
						this.deallocTempReg(regRec);
						this.deallocTempReg(regTempRowid);
					}
				}
				else {
					///This is the case if the data for the INSERT is coming from a VALUES
					///clause
					var sNC=new NameContext();
					// memset( &sNC, 0, sNC ).Length;
					sNC.pParse=this;
					srcTab=-1;
					Debug.Assert(!useTempTable);
					nColumn=pList!=null?pList.Count:0;
					for(i=0;i<nColumn;i++) {
						if(ResolveExtensions.sqlite3ResolveExprNames(sNC,ref pList.a[i].pExpr)!=0) {
							goto insert_cleanup;
						}
					}
				}
				///Make sure the number of columns in the source data matches the number
				///of columns to be inserted into the table.
				if(refTable.IsVirtual()) {
					for(i=0;i<refTable.nCol;i++) {
						nHidden+=(refTable.aCol[i].IsHiddenColumn()?1:0);
					}
				}
				if(pColumn==null&&nColumn!=0&&nColumn!=(refTable.nCol-nHidden)) {
					utilc.sqlite3ErrorMsg(this,"table %S has %d columns but %d values were supplied",pTabList,0,refTable.nCol-nHidden,nColumn);
					goto insert_cleanup;
				}
				if(pColumn!=null&&nColumn!=pColumn.nId) {
					utilc.sqlite3ErrorMsg(this,"%d values for %d columns",nColumn,pColumn.nId);
					goto insert_cleanup;
				}
				///If the INSERT statement included an IDLIST term, then make sure
				///all elements of the IDLIST really are columns of the table and
				///remember the column indices.
				///
				///If the table has an INTEGER PRIMARY KEY column and that column
				///is named in the IDLIST, then record in the keyColumn variable
				///the index into IDLIST of the primary key column.  keyColumn is
				///the index of the primary key as it appears in IDLIST, not as
				///is appears in the original table.  (The index of the primary
				///key in the original table is pTab.iPKey.)
				if(pColumn!=null) {
					for(i=0;i<pColumn.nId;i++) {
						pColumn.a[i].idx=-1;
					}
					for(i=0;i<pColumn.nId;i++) {
						for(j=0;j<refTable.nCol;j++) {
							if(pColumn.a[i].zName.Equals(refTable.aCol[j].zName,StringComparison.InvariantCultureIgnoreCase)) {
								pColumn.a[i].idx=j;
								if(j==refTable.iPKey) {
									keyColumn=i;
								}
								break;
							}
						}
						if(j>=refTable.nCol) {
							if(exprc.sqlite3IsRowid(pColumn.a[i].zName)) {
								keyColumn=i;
							}
							else {
								utilc.sqlite3ErrorMsg(this,"table %S has no column named %s",pTabList,0,pColumn.a[i].zName);
								this.checkSchema=1;
								goto insert_cleanup;
							}
						}
					}
				}
				///If there is no IDLIST term but the table has an integer primary
				///key, the set the keyColumn variable to the primary key column index
				///in the original table definition.
				if(pColumn==null&&nColumn>0) {
					keyColumn=refTable.iPKey;
				}
				///Initialize the count of rows to be inserted
				if((db.flags&SqliteFlags.SQLITE_CountRows)!=0) {
					regRowCount=++this.UsedCellCount;
					v.sqlite3VdbeAddOp2(OpCode.OP_Integer,0,regRowCount);
				}
				///If this is not a view, open the table and and all indices 
				if(!isView) {
					int nIdx;
					baseCur=this.nTab;
                    nIdx = this.sqlite3OpenTableAndIndices(refTable, baseCur, OpCode.OP_OpenWrite);
					aRegIdx=new int[nIdx+1];
					// sqlite3DbMallocRaw( db, sizeof( int ) * ( nIdx + 1 ) );
					if(aRegIdx==null) {
						goto insert_cleanup;
					}
					for(i=0;i<nIdx;i++) {
						aRegIdx[i]=++this.UsedCellCount;
					}
				}
				///This is the top of the main insertion loop 
				if(useTempTable) {
					///This block codes the top of loop only.  The complete loop is the
					///following pseudocode (template 4):
					///
					///rewind temp table
					///C: loop over rows of intermediate table
					///transfer values form intermediate table into <table>
					///end loop
					///D: ...
					addrInsTop=v.sqlite3VdbeAddOp1(OpCode.OP_Rewind,srcTab);
					addrCont=v.sqlite3VdbeCurrentAddr();
				}
				else
					if(pSelect!=null) {
						///
						///<summary>
						///This block codes the top of loop only.  The complete loop is the
						///following pseudocode (template 3):
						///
						///C: yield X
						///if EOF goto D
						///insert the select result into <table> from R..R+n
						///goto C
						///D: ...
						///
						///</summary>
						addrCont=v.sqlite3VdbeAddOp1(OpCode.OP_Yield,dest.iParm);
						addrInsTop=v.sqlite3VdbeAddOp1(OpCode.OP_If,regEof);
					}
				///Allocate registers for holding the rowid of the new row,
				///the content of the new row, and the assemblied row record.
				regRowid=regIns=this.UsedCellCount+1;
				this.UsedCellCount+=refTable.nCol+1;
				if(refTable.IsVirtual()) {
					regRowid++;
					this.UsedCellCount++;
				}
				regData=regRowid+1;
				///Run the BEFORE and INSTEAD OF triggers, if there are any
				endOfLoop=v.sqlite3VdbeMakeLabel();
				#if !SQLITE_OMIT_TRIGGER
				if((tmask&TriggerType.TRIGGER_BEFORE)!=0) {
					int regCols=this.sqlite3GetTempRange(refTable.nCol+1);
					///build the NEW.* reference row.  Note that if there is an INTEGER
					///PRIMARY KEY into which a NULL is being inserted, that NULL will be
					///translated into a unique ID for the row.  But on a BEFORE trigger,
					///we do not know what the unique ID will be (because the insert has
					///<param name="not happened yet) so we substitute a rowid of ">1</param>
					if(keyColumn<0) {
						v.sqlite3VdbeAddOp2(OpCode.OP_Integer,-1,regCols);
					}
					else {
						int j1;
						if(useTempTable) {
							v.sqlite3VdbeAddOp3( OpCode.OP_Column,srcTab,keyColumn,regCols);
						}
						else {
							Debug.Assert(pSelect==null);
							///
							///<summary>
							///Otherwise useTempTable is true 
							///</summary>
							this.sqlite3ExprCode(pList.a[keyColumn].pExpr,regCols);
						}
						j1=v.sqlite3VdbeAddOp1(OpCode.OP_NotNull,regCols);
						v.sqlite3VdbeAddOp2(OpCode.OP_Integer,-1,regCols);
						v.sqlite3VdbeJumpHere(j1);
						v.sqlite3VdbeAddOp1(OpCode.OP_MustBeInt,regCols);
					}
					///
					///<summary>
					///Cannot have triggers on a virtual table. If it were possible,
					///this block would have to account for hidden column.
					///
					///</summary>
					Debug.Assert(!refTable.IsVirtual());
					///
					///<summary>
					///Create the new column data
					///
					///</summary>
					for(i=0;i<refTable.nCol;i++) {
						if(pColumn==null) {
							j=i;
						}
						else {
							for(j=0;j<pColumn.nId;j++) {
								if(pColumn.a[j].idx==i)
									break;
							}
						}
						if((!useTempTable&&null==pList)||(pColumn!=null&&j>=pColumn.nId)) {
							this.sqlite3ExprCode(refTable.aCol[i].DefaultValue,regCols+i+1);
						}
						else
							if(useTempTable) {
								v.sqlite3VdbeAddOp3( OpCode.OP_Column,srcTab,j,regCols+i+1);
							}
							else {
								Debug.Assert(pSelect==null);
								///Otherwise useTempTable is true 
								this.sqlite3ExprCodeAndCache(pList.a[j].pExpr,regCols+i+1);
							}
					}
					///If this is an INSERT on a view with an INSTEAD OF INSERT trigger,
					///do not attempt any conversions before assembling the record.
					///If this is a real table, attempt conversions as required by the
					///table column affinities.
					if(!isView) {
                        v.sqlite3VdbeAddOp2(OpCode.OP_Affinity, regCols + 1, refTable.nCol);
						v.sqlite3TableAffinityStr(refTable);
					}
                    ///Fire BEFORE or INSTEAD OF triggers 
                    TriggerBuilder.sqlite3CodeRowTrigger(this,pTrigger,TokenType.TK_INSERT,null,TriggerType.TRIGGER_BEFORE,refTable,regCols-refTable.nCol-1,onError,endOfLoop);
					this.sqlite3ReleaseTempRange(regCols,refTable.nCol+1);
				}
				#endif
				///
				///<summary>
				///Push the record number for the new entry onto the stack.  The
				///record number is a randomly generate integer created by NewRowid
				///except when the table has an INTEGER PRIMARY KEY column, in which
				///case the record number is the same as that column.
				///</summary>
				if(!isView) {
					if(refTable.IsVirtual()) {
						///
						///<summary>
						///The row that the VUpdate opcode will delete: none 
						///</summary>
                        v.sqlite3VdbeAddOp2(OpCode.OP_Null, 0, regIns);
					}
					if(keyColumn>=0) {
						if(useTempTable) {
							v.sqlite3VdbeAddOp3( OpCode.OP_Column,srcTab,keyColumn,regRowid);
						}
						else
							if(pSelect!=null) {
                                v.sqlite3VdbeAddOp2(OpCode.OP_SCopy, regFromSelect + keyColumn, regRowid);
							}
							else {
								VdbeOp pOp;
								this.sqlite3ExprCode(pList.a[keyColumn].pExpr,regRowid);
								pOp=v.sqlite3VdbeGetOp(-1);
								if(Sqlite3.ALWAYS(pOp!=null)&&pOp.OpCode==OpCode.OP_Null&&!refTable.IsVirtual()) {
									appendFlag=true;
									pOp.OpCode=OpCode.OP_NewRowid;
									pOp.p1=baseCur;
									pOp.p2=regRowid;
									pOp.p3=regAutoinc;
								}
							}
						///
						///<summary>
						///If the PRIMARY KEY expression is NULL, then use  OpCode.OP_NewRowid
						///to generate a unique primary key value.
						///
						///</summary>
						if(!appendFlag) {
							int j1;
							if(!refTable.IsVirtual()) {
								j1=v.sqlite3VdbeAddOp1(OpCode.OP_NotNull,regRowid);
								v.sqlite3VdbeAddOp3( OpCode.OP_NewRowid,baseCur,regRowid,regAutoinc);
								v.sqlite3VdbeJumpHere(j1);
							}
							else {
								j1=v.sqlite3VdbeCurrentAddr();
                                v.sqlite3VdbeAddOp2(OpCode.OP_IsNull, regRowid, j1 + 2);
							}
							v.sqlite3VdbeAddOp1(OpCode.OP_MustBeInt,regRowid);
						}
					}
					else
						if(refTable.IsVirtual()) {
                            v.sqlite3VdbeAddOp2(OpCode.OP_Null, 0, regRowid);
						}
						else {
							v.sqlite3VdbeAddOp3( OpCode.OP_NewRowid,baseCur,regRowid,regAutoinc);
							appendFlag=true;
						}
					this.autoIncStep(regAutoinc,regRowid);
					///
					///<summary>
					///Push onto the stack, data for all columns of the new entry, beginning
					///with the first column.
					///
					///</summary>
					nHidden=0;
					for(i=0;i<refTable.nCol;i++) {
						int iRegStore=regRowid+1+i;
						if(i==refTable.iPKey) {
							///
							///<summary>
							///The value of the INTEGER PRIMARY KEY column is always a NULL.
							///Whenever this column is read, the record number will be substituted
							///in its place.  So will fill this column with a NULL to avoid
							///taking up data space with information that will never be used. 
							///</summary>
                            v.sqlite3VdbeAddOp2(OpCode.OP_Null, 0, iRegStore);
							continue;
						}
						if(pColumn==null) {
                            if (refTable.aCol[i].IsHiddenColumn())
                            {
								Debug.Assert(refTable.IsVirtual());
								j=-1;
								nHidden++;
							}
							else {
								j=i-nHidden;
							}
						}
						else {
							for(j=0;j<pColumn.nId;j++) {
								if(pColumn.a[j].idx==i)
									break;
							}
						}
						if(j<0||nColumn==0||(pColumn!=null&&j>=pColumn.nId)) {
							this.sqlite3ExprCode(refTable.aCol[i].DefaultValue,iRegStore);
						}
						else
							if(useTempTable) {
								v.sqlite3VdbeAddOp3( OpCode.OP_Column,srcTab,j,iRegStore);
							}
							else
								if(pSelect!=null) {
                                    v.sqlite3VdbeAddOp2(OpCode.OP_SCopy, regFromSelect + j, iRegStore);
								}
								else {
									this.sqlite3ExprCode(pList.a[j].pExpr,iRegStore);
								}
					}
					///Generate code to check constraints and generate index keys and
					///do the insertion.
					#if !SQLITE_OMIT_VIRTUALTABLE
					if(refTable.IsVirtual()) {
                        VTable pVTab = VTableMethodsExtensions.sqlite3GetVTable(db, refTable);
						this.sqlite3VtabMakeWritable(refTable);
                        v.sqlite3VdbeAddOp4(OpCode.OP_VUpdate, 1, refTable.nCol + 2, regIns, pVTab,  P4Usage.P4_VTAB);
						v.sqlite3VdbeChangeP5((byte)(onError==OnConstraintError.OE_Default?OnConstraintError.OE_Abort:onError));
						build.sqlite3MayAbort(this);
					}
					else
					#endif
					 {
						int isReplace=0;
						///Set to true if constraints may cause a replace 
						this.sqlite3GenerateConstraintChecks(refTable,baseCur,regIns,aRegIdx,keyColumn>=0?1:0,false,onError,endOfLoop,out isReplace);
						this.sqlite3FkCheck(refTable,0,regIns);
						this.sqlite3CompleteInsertion(refTable,baseCur,regIns,aRegIdx,false,appendFlag,isReplace==0);
					}
				}
				///Update the count of rows that are inserted
                if ((db.flags & SqliteFlags.SQLITE_CountRows) != 0)
                {
					v.sqlite3VdbeAddOp2(OpCode.OP_AddImm,regRowCount,1);
				}
				#if !SQLITE_OMIT_TRIGGER
				if(pTrigger!=null) {
                    ///Code AFTER triggers 
                    TriggerBuilder.sqlite3CodeRowTrigger(this,pTrigger,TokenType.TK_INSERT,null,TriggerType.TRIGGER_AFTER,refTable,regData-2-refTable.nCol,onError,endOfLoop);
				}
				#endif
				///The bottom of the main insertion loop, if the data source
				///is a SELECT statement.
				v.sqlite3VdbeResolveLabel(endOfLoop);
				if(useTempTable) {
					v.sqlite3VdbeAddOp2( OpCode.OP_Next,srcTab,addrCont);
					v.sqlite3VdbeJumpHere(addrInsTop);
					v.sqlite3VdbeAddOp1(OpCode.OP_Close,srcTab);
				}
				else
					if(pSelect!=null) {
						v.sqlite3VdbeAddOp2(OpCode.OP_Goto,0,addrCont);
						v.sqlite3VdbeJumpHere(addrInsTop);
					}
                if (!refTable.IsVirtual() && !isView)
                {
                    ///Close all tables opened 
                    v.sqlite3VdbeAddOp1(OpCode.OP_Close, baseCur);
                    ///For looping over indices of the table 
                    refTable.pIndex.linkedList().ForEach(
                        (pIdx, ii) =>
                        {
                            v.sqlite3VdbeAddOp1(OpCode.OP_Close, ii + 1 + baseCur);
                        }
                    );
                }
				insert_end:
				///Update the sqlite_sequence table by storing the content of the
				///maximum rowid counter values recorded while inserting into
				///autoincrement tables.
				if(this.nested==0&&this.pTriggerTab==null) {
					this.sqlite3AutoincrementEnd();
				}
				///Return the number of rows inserted. If this routine is
				///generating code because of a call to build.sqlite3NestedParse(), do not
				///invoke the callback function.
                if ((db.flags & SqliteFlags.SQLITE_CountRows) != 0 && 0 == this.nested && null == this.pTriggerTab)
                {
					v.sqlite3VdbeAddOp2(OpCode.OP_ResultRow,regRowCount,1);
					v.sqlite3VdbeSetNumCols(1);
					v.sqlite3VdbeSetColName(0,ColName.NAME,"rows inserted",SQLITE_STATIC);
				}
				insert_cleanup:
				build.sqlite3SrcListDelete(db,ref pTabList);
				exprc.Delete(db,ref pList);
				SelectMethods.SelectDestructor(db,ref pSelect);
				build.sqlite3IdListDelete(db,ref pColumn);
				db.sqlite3DbFree(ref aRegIdx);
			}
			public void sqlite3AutoincrementBegin() {
				AutoincInfo p;
				///
				///<summary>
				///Information about an AUTOINCREMENT 
				///</summary>
				Connection db=this.db;
				///
				///<summary>
				///The database connection 
				///</summary>
				DbBackend pDb;
				///
				///<summary>
				///Database only autoinc table 
				///</summary>
				int memId;
				///
				///<summary>
				///Register holding max rowid 
				///</summary>
				int addr;
				///
				///<summary>
				///A VDBE address 
				///</summary>
				Vdbe v=this.pVdbe;
				///
				///<summary>
				///VDBE under construction 
				///</summary>
				///
				///<summary>
				///</summary>
				///<param name="This routine is never called during trigger">generation.  It is</param>
				///<param name="only called from the top">level </param>
				Debug.Assert(this.pTriggerTab==null);
				Debug.Assert(this==sqliteinth.sqlite3ParseToplevel(this));
				Debug.Assert(v!=null);
				///
				///<summary>
				///We failed long ago if this is not so 
				///</summary>
				for(p=this.pAinc;p!=null;p=p.pNext) {
					pDb=db.Backends[p.iDb];
					memId=p.regCtr;
					Debug.Assert(sqlite3SchemaMutexHeld(db,0,pDb.pSchema));
					this.sqlite3OpenTable(0,p.iDb,pDb.pSchema.pSeqTab,OpCode.OP_OpenRead);
					addr=v.sqlite3VdbeCurrentAddr();
					v.sqlite3VdbeAddOp4(OpCode.OP_String8,0,memId-1,0,p.pTab.zName,0);
                    v.sqlite3VdbeAddOp2(OpCode.OP_Rewind, 0, addr + 9);
                    v.sqlite3VdbeAddOp3(OpCode.OP_Column, 0, 0, memId);
                    v.sqlite3VdbeAddOp3(OpCode.OP_Ne, memId - 1, addr + 7, memId);
					v.sqlite3VdbeChangeP5(sqliteinth.SQLITE_JUMPIFNULL);
                    v.sqlite3VdbeAddOp2(OpCode.OP_Rowid, 0, memId + 1);
                    v.sqlite3VdbeAddOp3(OpCode.OP_Column, 0, 1, memId);
                    v.sqlite3VdbeAddOp2(OpCode.OP_Goto, 0, addr + 9);
                    v.sqlite3VdbeAddOp2(OpCode.OP_Next, 0, addr + 2);
					v.sqlite3VdbeAddOp2(OpCode.OP_Integer,0,memId);
                    v.sqlite3VdbeAddOp0(OpCode.OP_Close);
				}
			}
			public void sqlite3AutoincrementEnd() {
				AutoincInfo p;
				Vdbe v=this.pVdbe;
				Connection db=this.db;
				Debug.Assert(v!=null);
				for(p=this.pAinc;p!=null;p=p.pNext) {
					DbBackend pDb=db.Backends[p.iDb];
					int j1,j2,j3,j4,j5;
					int iRec;
					int memId=p.regCtr;
					iRec=this.allocTempReg();
					Debug.Assert(sqlite3SchemaMutexHeld(db,0,pDb.pSchema));
                    this.sqlite3OpenTable(0, p.iDb, pDb.pSchema.pSeqTab, OpCode.OP_OpenWrite);
					j1=v.sqlite3VdbeAddOp1(OpCode.OP_NotNull,memId+1);
                    j2 = v.sqlite3VdbeAddOp0(OpCode.OP_Rewind);
					j3=v.sqlite3VdbeAddOp3( OpCode.OP_Column,0,0,iRec);
					j4=v.sqlite3VdbeAddOp3( OpCode.OP_Eq,memId-1,0,iRec);
					v.sqlite3VdbeAddOp2( OpCode.OP_Next,0,j3);
					v.sqlite3VdbeJumpHere(j2);
					v.sqlite3VdbeAddOp2( OpCode.OP_NewRowid,0,memId+1);
					j5=v.sqlite3VdbeAddOp0(OpCode.OP_Goto);
					v.sqlite3VdbeJumpHere(j4);
                    v.sqlite3VdbeAddOp2(OpCode.OP_Rowid, 0, memId + 1);
					v.sqlite3VdbeJumpHere(j1);
					v.sqlite3VdbeJumpHere(j5);
					v.sqlite3VdbeAddOp3( OpCode.OP_MakeRecord,memId-1,2,iRec);
					v.sqlite3VdbeAddOp3( OpCode.OP_Insert,0,iRec,memId+1);
                    v.sqlite3VdbeChangeP5(OpFlag.OPFLAG_APPEND);
                    v.sqlite3VdbeAddOp0(OpCode.OP_Close);
					this.deallocTempReg(iRec);
				}
			}
			public void autoIncStep(int memId,int regRowid) {
				if(memId>0) {
					this.pVdbe.sqlite3VdbeAddOp2( OpCode.OP_MemMax,memId,regRowid);
				}
			}
			public void sqlite3GenerateConstraintChecks(///
			///<summary>
			///The parser context 
			///</summary>
			Table pTab,///
			///<summary>
			///the table into which we are inserting 
			///</summary>
			int baseCur,///
			///<summary>
			///Index of a read/write cursor pointing at pTab 
			///</summary>
			int regRowid,///
			///<summary>
			///Index of the range of input registers 
			///</summary>
			int[] aRegIdx,///
			///<summary>
			///Register used by each index.  0 for unused indices 
			///</summary>
			int rowidChng,///
			///<summary>
			///True if the rowid might collide with existing entry 
			///</summary>
			bool isUpdate,///
			///<summary>
			///True for UPDATE, False for INSERT 
			///</summary>
			OnConstraintError overrideError,///
			///<summary>
			///Override onError to this if not OnConstraintError.OE_Default 
			///</summary>
			int ignoreDest,///
			///<summary>
			///Jump to this label on an OnConstraintError.OE_Ignore resolution 
			///</summary>
			out int pbMayReplace///
			///<summary>
			///OUT: Set to true if constraint may cause a replace 
			///</summary>
			) {
				int i;
				///
				///<summary>
				///loop counter 
				///</summary>
				Vdbe v;
				///
				///<summary>
				///VDBE under constrution 
				///</summary>
				int nCol;
				///
				///<summary>
				///Number of columns 
				///</summary>
				OnConstraintError onError;
				///
				///<summary>
				///Conflict resolution strategy 
				///</summary>
				int j1;
				///
				///<summary>
				///Addresss of jump instruction 
				///</summary>
				int j2=0,j3;
				///
				///<summary>
				///Addresses of jump instructions 
				///</summary>
				int regData;
				///
				///<summary>
				///Register containing first data column 
				///</summary>
				int iCur;
				///
				///<summary>
				///Table cursor number 
				///</summary>
				Index pIdx;
				///
				///<summary>
				///Pointer to one of the indices 
				///</summary>
				bool seenReplace=false;
				///
				///<summary>
				///True if REPLACE is used to resolve INT PK conflict 
				///</summary>
				int regOldRowid=(rowidChng!=0&&isUpdate)?rowidChng:regRowid;
				v=this.sqlite3GetVdbe();
				Debug.Assert(v!=null);
				Debug.Assert(pTab.pSelect==null);
				///
				///<summary>
				///This table is not a VIEW 
				///</summary>
				nCol=pTab.nCol;
				regData=regRowid+1;
				///
				///<summary>
				///Test all NOT NULL constraints.
				///
				///</summary>
				for(i=0;i<nCol;i++) {
					if(i==pTab.iPKey) {
						continue;
					}
					onError=(OnConstraintError)pTab.aCol[i].notNull;
					if(onError==OnConstraintError.OE_None)
						continue;
					if(overrideError!=OnConstraintError.OE_Default) {
						onError=overrideError;
					}
					else
						if(onError==OnConstraintError.OE_Default) {
							onError=OnConstraintError.OE_Abort;
						}
					if(onError==OnConstraintError.OE_Replace&&pTab.aCol[i].DefaultValue==null) {
						onError=OnConstraintError.OE_Abort;
					}
					Debug.Assert(onError==OnConstraintError.OE_Rollback||onError==OnConstraintError.OE_Abort||onError==OnConstraintError.OE_Fail||onError==OnConstraintError.OE_Ignore||onError==OnConstraintError.OE_Replace);
					switch(onError) {
					    case OnConstraintError.OE_Abort: {
						    build.sqlite3MayAbort(this);
						    goto case OnConstraintError.OE_Fail;
					    }
                        case OnConstraintError.OE_Rollback:
					    case OnConstraintError.OE_Fail: {
						    string zMsg;
                            v.sqlite3VdbeAddOp3(OpCode.OP_HaltIfNull, (int)SqlResult.SQLITE_CONSTRAINT, (int)onError, regData + i);
						    zMsg=io.sqlite3MPrintf(this.db,"%s.%s may not be NULL",pTab.zName,pTab.aCol[i].zName);
						    v.sqlite3VdbeChangeP4(-1,zMsg, P4Usage.P4_DYNAMIC);
						    break;
					    }
					    case OnConstraintError.OE_Ignore: {
                            v.sqlite3VdbeAddOp2(OpCode.OP_IsNull, regData + i, ignoreDest);
						    break;
					    }
					    default: {
						    Debug.Assert(onError==OnConstraintError.OE_Replace);
						    j1=v.sqlite3VdbeAddOp1(OpCode.OP_NotNull,regData+i);
						    this.sqlite3ExprCode(pTab.aCol[i].DefaultValue,regData+i);
						    v.sqlite3VdbeJumpHere(j1);
						    break;
					}
					}
				}
				///
				///<summary>
				///Test all CHECK constraints
				///
				///</summary>
				#if !SQLITE_OMIT_CHECK
                if (pTab.pCheck != null && (this.db.flags & SqliteFlags.SQLITE_IgnoreChecks) == 0)
                {
					int allOk=v.sqlite3VdbeMakeLabel();
					this.ckBase=regData;
					this.sqlite3ExprIfTrue(pTab.pCheck,allOk,sqliteinth.SQLITE_JUMPIFNULL);
                    onError = overrideError != OnConstraintError.OE_Default ? overrideError : OnConstraintError.OE_Abort;
					if(onError==OnConstraintError.OE_Ignore) {
						v.sqlite3VdbeAddOp2(OpCode.OP_Goto,0,ignoreDest);
					}
					else {
						if(onError==OnConstraintError.OE_Replace)
							onError=OnConstraintError.OE_Abort;
						///
						///<summary>
						///</summary>
						///<param name="IMP: R">63625 </param>
						build.sqlite3HaltConstraint(this,onError,(string)null,0);
					}
					v.sqlite3VdbeResolveLabel(allOk);
				}
				#endif
				///
				///<summary>
				///If we have an INTEGER PRIMARY KEY, make sure the primary key
				///of the new record does not previously exist.  Except, if this
				///is an UPDATE and the primary key is not changing, that is OK.
				///</summary>
				if(rowidChng!=0) {
					onError=pTab.keyConf;
					if(overrideError!=OnConstraintError.OE_Default) {
						onError=overrideError;
					}
					else
						if(onError==OnConstraintError.OE_Default) {
							onError=OnConstraintError.OE_Abort;
						}
					if(isUpdate) {
						j2=v.sqlite3VdbeAddOp3( OpCode.OP_Eq,regRowid,0,rowidChng);
					}
					j3=v.sqlite3VdbeAddOp3( OpCode.OP_NotExists,baseCur,0,regRowid);
					switch(onError) {
					default:
					{
						onError=OnConstraintError.OE_Abort;
						///
						///<summary>
						///Fall thru into the next case 
						///</summary>
					}
					goto case OnConstraintError.OE_Rollback;
					case OnConstraintError.OE_Rollback:
					case OnConstraintError.OE_Abort:
					case OnConstraintError.OE_Fail: {
						build.sqlite3HaltConstraint(this,onError,"PRIMARY KEY must be unique", P4Usage.P4_STATIC);
						break;
					}
					case OnConstraintError.OE_Replace: {
						///
						///<summary>
						///If there are DELETE triggers on this table and the
						///</summary>
						///<param name="recursive">triggers flag is set, call GenerateRowDelete() to</param>
						///<param name="remove the conflicting row from the the table. This will fire">remove the conflicting row from the the table. This will fire</param>
						///<param name="the triggers and remove both the table and index b">tree entries.</param>
						///<param name=""></param>
						///<param name="Otherwise, if there are no triggers or the recursive">triggers</param>
						///<param name="flag is not set, but the table has one or more indexes, call ">flag is not set, but the table has one or more indexes, call </param>
						///<param name="GenerateRowIndexDelete(). This removes the index b">tree entries </param>
						///<param name="only. The table b">tree entry will be replaced by the new entry </param>
						///<param name="when it is inserted.  ">when it is inserted.  </param>
						///<param name=""></param>
						///<param name="If either GenerateRowDelete() or GenerateRowIndexDelete() is called,">If either GenerateRowDelete() or GenerateRowIndexDelete() is called,</param>
						///<param name="also invoke MultiWrite() to indicate that this VDBE may require">also invoke MultiWrite() to indicate that this VDBE may require</param>
						///<param name="statement rollback (if the statement is aborted after the delete">statement rollback (if the statement is aborted after the delete</param>
						///<param name="takes place). Earlier versions called build.sqlite3MultiWrite() regardless,">takes place). Earlier versions called build.sqlite3MultiWrite() regardless,</param>
						///<param name="but being more selective here allows statements like:">but being more selective here allows statements like:</param>
						///<param name=""></param>
						///<param name="REPLACE INTO t(rowid) VALUES($newrowid)">REPLACE INTO t(rowid) VALUES($newrowid)</param>
						///<param name=""></param>
						///<param name="to run without a statement journal if there are no indexes on the">to run without a statement journal if there are no indexes on the</param>
						///<param name="table.">table.</param>
						///<param name=""></param>
						Trigger pTrigger=null;
                        if ((this.db.flags & SqliteFlags.SQLITE_RecTriggers) != 0)
                        {
							TriggerType iDummy;
							pTrigger= TriggerParser.sqlite3TriggersExist(this,pTab,TokenType.TK_DELETE,null,out iDummy);
						}
						if(pTrigger!=null||this.sqlite3FkRequired(pTab,null,0)!=false) {
							build.sqlite3MultiWrite(this);
							this.codegenRowDelete(pTab,baseCur,regRowid,0,pTrigger,OnConstraintError.OE_Replace);
						}
						else
							if(pTab.pIndex!=null) {
								build.sqlite3MultiWrite(this);
								this.codegenRowIndexDelete(pTab,baseCur,0);
							}
						seenReplace=true;
						break;
					}
					case OnConstraintError.OE_Ignore: {
						Debug.Assert(!seenReplace);
						v.sqlite3VdbeAddOp2(OpCode.OP_Goto,0,ignoreDest);
						break;
					}
					}
					v.sqlite3VdbeJumpHere(j3);
					if(isUpdate) {
						v.sqlite3VdbeJumpHere(j2);
					}
				}
				///
				///<summary>
				///Test all UNIQUE constraints by creating entries for each UNIQUE
				///index and making sure that duplicate entries do not already exist.
				///Add the new records to the indices as we go.
				///
				///</summary>
				for(iCur=0,pIdx=pTab.pIndex;pIdx!=null;pIdx=pIdx.pNext,iCur++) {
					int regIdx;
					int regR;
					if(aRegIdx[iCur]==0)
						continue;
					///
					///<summary>
					///Skip unused indices 
					///</summary>
					///
					///<summary>
					///Create a key for accessing the index entry 
					///</summary>
					regIdx=this.sqlite3GetTempRange(pIdx.nColumn+1);
					for(i=0;i<pIdx.nColumn;i++) {
						int idx=pIdx.aiColumn[i];
						if(idx==pTab.iPKey) {
                            v.sqlite3VdbeAddOp2(OpCode.OP_SCopy, regRowid, regIdx + i);
						}
						else {
                            v.sqlite3VdbeAddOp2(OpCode.OP_SCopy, regData + idx, regIdx + i);
						}
					}
                    v.sqlite3VdbeAddOp2(OpCode.OP_SCopy, regRowid, regIdx + i);
					v.sqlite3VdbeAddOp3( OpCode.OP_MakeRecord,regIdx,pIdx.nColumn+1,aRegIdx[iCur]);
					v.sqlite3VdbeChangeP4(-1,v.sqlite3IndexAffinityStr(pIdx), P4Usage.P4_TRANSIENT);
					this.sqlite3ExprCacheAffinityChange(regIdx,pIdx.nColumn+1);
					///
					///<summary>
					///Find out what action to take in case there is an indexing conflict 
					///</summary>
					onError=pIdx.onError;
					if(onError==OnConstraintError.OE_None) {
						this.sqlite3ReleaseTempRange(regIdx,pIdx.nColumn+1);
						continue;
						///
						///<summary>
						///pIdx is not a UNIQUE index 
						///</summary>
					}
					if(overrideError!=OnConstraintError.OE_Default) {
						onError=overrideError;
					}
					else
						if(onError==OnConstraintError.OE_Default) {
							onError=OnConstraintError.OE_Abort;
						}
					if(seenReplace) {
						if(onError==OnConstraintError.OE_Ignore)
							onError=OnConstraintError.OE_Replace;
						else
							if(onError==OnConstraintError.OE_Fail)
								onError=OnConstraintError.OE_Abort;
					}
					///
					///<summary>
					///Check to see if the new index entry will be unique 
					///</summary>
					regR=this.allocTempReg();
                    v.sqlite3VdbeAddOp2(OpCode.OP_SCopy, regOldRowid, regR);
                    j3 = v.sqlite3VdbeAddOp4(OpCode.OP_IsUnique, baseCur + iCur + 1, 0, regR, regIdx,//regR, SQLITE_INT_TO_PTR(regIdx),
					 P4Usage.P4_INT32);
					this.sqlite3ReleaseTempRange(regIdx,pIdx.nColumn+1);
					///
					///<summary>
					///Generate code that executes if the new index entry is not unique 
					///</summary>
					Debug.Assert(onError==OnConstraintError.OE_Rollback||onError==OnConstraintError.OE_Abort||onError==OnConstraintError.OE_Fail||onError==OnConstraintError.OE_Ignore||onError==OnConstraintError.OE_Replace);
					switch(onError) {
					case OnConstraintError.OE_Rollback:
					case OnConstraintError.OE_Abort:
					case OnConstraintError.OE_Fail: {
						int j;
						StrAccum errMsg=new StrAccum(200);
						string zSep;
						string zErr;
						io.sqlite3StrAccumInit(errMsg,null,0,200);
						errMsg.db=this.db;
						zSep=pIdx.nColumn>1?"columns ":"column ";
						for(j=0;j<pIdx.nColumn;j++) {
							string zCol=pTab.aCol[pIdx.aiColumn[j]].zName;
                            errMsg.sqlite3StrAccumAppend(zSep, -1);
							zSep=", ";
                            errMsg.sqlite3StrAccumAppend(zCol, -1);
						}
                        errMsg.sqlite3StrAccumAppend(pIdx.nColumn > 1 ? " are not unique" : " is not unique", -1);
						zErr=io.sqlite3StrAccumFinish(errMsg);
						build.sqlite3HaltConstraint(this,onError,zErr,0);
						errMsg.db.DbFree(ref zErr);
						break;
					}
					case OnConstraintError.OE_Ignore: {
						Debug.Assert(!seenReplace);
						v.sqlite3VdbeAddOp2(OpCode.OP_Goto,0,ignoreDest);
						break;
					}
					default: {
						Trigger pTrigger=null;
						Debug.Assert(onError==OnConstraintError.OE_Replace);
						build.sqlite3MultiWrite(this);
                        if ((this.db.flags & SqliteFlags.SQLITE_RecTriggers) != 0)
                        {
							TriggerType iDummy;
							pTrigger= TriggerParser.sqlite3TriggersExist(this,pTab,TokenType.TK_DELETE,null,out iDummy);
						}
						this.codegenRowDelete(pTab,baseCur,regR,0,pTrigger,OnConstraintError.OE_Replace);
						seenReplace=true;
						break;
					}
					}
					v.sqlite3VdbeJumpHere(j3);
					this.deallocTempReg(regR);
				}
				//if ( pbMayReplace )
				{
					pbMayReplace=seenReplace?1:0;
				}
			}
			public void sqlite3CompleteInsertion(///
			///<summary>
			///The parser context 
			///</summary>
			Table pTab,///
			///<summary>
			///the table into which we are inserting 
			///</summary>
			int baseCur,///
			///<summary>
			///Index of a read/write cursor pointing at pTab 
			///</summary>
			int regRowid,///
			///<summary>
			///Range of content 
			///</summary>
			int[] aRegIdx,///
			///<summary>
			///Register used by each index.  0 for unused indices 
			///</summary>
			bool isUpdate,///
			///<summary>
			///True for UPDATE, False for INSERT 
			///</summary>
			bool appendBias,///
			///<summary>
			///True if this is likely to be an append 
			///</summary>
			bool useSeekResult///
			///<summary>
			///True to set the USESEEKRESULT flag on  OpCode.OP_[Idx]Insert 
			///</summary>
			) {
				int i;
				Vdbe v;
				int nIdx;
				Index pIdx;
                OpFlag pik_flags;
				int regData;
				int regRec;
				v=this.sqlite3GetVdbe();
				Debug.Assert(v!=null);
				Debug.Assert(pTab.pSelect==null);
				///
				///<summary>
				///This table is not a VIEW 
				///</summary>
				for(nIdx=0,pIdx=pTab.pIndex;pIdx!=null;pIdx=pIdx.pNext,nIdx++) {
				}
				for(i=nIdx-1;i>=0;i--) {
					if(aRegIdx[i]==0)
						continue;
					v.sqlite3VdbeAddOp2( OpCode.OP_IdxInsert,baseCur+i+1,aRegIdx[i]);
					if(useSeekResult) {
						v.sqlite3VdbeChangeP5(OpFlag.OPFLAG_USESEEKRESULT);
					}
				}
				regData=regRowid+1;
				regRec=this.allocTempReg();
				v.sqlite3VdbeAddOp3( OpCode.OP_MakeRecord,regData,pTab.nCol,regRec);
				v.sqlite3TableAffinityStr(pTab);
				this.sqlite3ExprCacheAffinityChange(regData,pTab.nCol);
				if(this.nested!=0) {
					pik_flags=0;
				}
				else {
                    pik_flags = OpFlag.OPFLAG_NCHANGE;
                    pik_flags |= (isUpdate ? OpFlag.OPFLAG_ISUPDATE : OpFlag.OPFLAG_LASTROWID);
				}
				if(appendBias) {
                    pik_flags |= OpFlag.OPFLAG_APPEND;
				}
				if(useSeekResult) {
                    pik_flags |= OpFlag.OPFLAG_USESEEKRESULT;
				}
				v.sqlite3VdbeAddOp3( OpCode.OP_Insert,baseCur,regRec,regRowid);
				if(this.nested==0) {
					v.sqlite3VdbeChangeP4(-1,pTab.zName, (int)P4Usage.P4_TRANSIENT);
				}
				v.sqlite3VdbeChangeP5(pik_flags);
			}
			public int sqlite3OpenTableAndIndices(///
			///<summary>
			///Parsing context 
			///</summary>
			Table pTab,///
			///<summary>
			///Table to be opened 
			///</summary>
			int baseCur,///
			///<summary>
			///VdbeCursor number assigned to the table 
			///</summary>
			OpCode op///
			///<summary>
			///OP_OpenRead or  OpCode.OP_OpenWrite 
			///</summary>
			) {
				int i;
				int iDb;
				Index pIdx;
				Vdbe v;
				if(pTab.IsVirtual())
					return 0;
				iDb= this.db.indexOfBackendWithSchema(pTab.pSchema);
				v=this.sqlite3GetVdbe();
				Debug.Assert(v!=null);
				this.sqlite3OpenTable(baseCur,iDb,pTab,op);
				for(i=1,pIdx=pTab.pIndex;pIdx!=null;pIdx=pIdx.pNext,i++) {
                    KeyInfo pKey = pIdx.sqlite3IndexKeyinfo(this);
					Debug.Assert(pIdx.pSchema==pTab.pSchema);
					v.sqlite3VdbeAddOp4(op,i+baseCur,pIdx.tnum,iDb,pKey, P4Usage.P4_KEYINFO_HANDOFF);
					#if SQLITE_DEBUG
																																																																																																																																			        VdbeComment( v, "%s", pIdx.zName );
#endif
				}
				if(this.nTab<baseCur+i) {
					this.nTab=baseCur+i;
				}
				return i-1;
			}

			public int autoIncBegin(
			    int iDb,///Index of the database holding pTab 
			    Table pTab///The table we are writing to 
			) {
				int memId=0;
				///Register holding maximum rowid 
				if((pTab.tabFlags&TableFlags.TF_Autoincrement)!=0) {
					Parse pToplevel=sqliteinth.sqlite3ParseToplevel(this);
					var pInfo=pToplevel.pAinc.linkedList()
                        .FirstOrDefault(p=> p.pTab != pTab);
					
					if(pInfo==null) {
                        pInfo = new AutoincInfo() {
                            pTab = pTab,
                            iDb = iDb
                        };
						//sqlite3DbMallocRaw(pParse.db, sizeof(*pInfo));
						//if( pInfo==0 ) return 0;
						pInfo.pNext=pToplevel.pAinc;
						pToplevel.pAinc=pInfo;
						
						pToplevel.UsedCellCount++;
						///Register to hold name of table 
						pInfo.regCtr=++pToplevel.UsedCellCount;
						///Max rowid register 
						pToplevel.UsedCellCount++;
						///Rowid in sqlite_sequence 
					}
					memId=pInfo.regCtr;
				}
				return memId;
			}

			public bool readsTable(int iStartAddr,int iDb,Table pTab) {
				Vdbe v=this.sqlite3GetVdbe();
				int i;
				int iEnd=v.sqlite3VdbeCurrentAddr();
				#if !SQLITE_OMIT_VIRTUALTABLE
                VTable pVTab = pTab.IsVirtual() ? VTableMethodsExtensions.sqlite3GetVTable(this.db, pTab) : null;
				#endif
				for(i=iStartAddr;i<iEnd;i++) {
					VdbeOp pOp=v.sqlite3VdbeGetOp(i);
					Debug.Assert(pOp!=null);
					if(pOp.OpCode==OpCode.OP_OpenRead&&pOp.p3==iDb) {
						Index pIndex;
						int tnum=pOp.p2;
						if(tnum==pTab.tnum) {
							return true;
						}
						for(pIndex=pTab.pIndex;pIndex!=null;pIndex=pIndex.pNext) {
							if(tnum==pIndex.tnum) {
								return true;
							}
						}
					}
					#if !SQLITE_OMIT_VIRTUALTABLE
					if(pOp.OpCode==OpCode.OP_VOpen&&pOp.p4.pVtab==pVTab) {
						Debug.Assert(pOp.p4.pVtab!=null);
						Debug.Assert(pOp.p4type== P4Usage.P4_VTAB);
						return true;
					}
					#endif
				}
				return false;
			}
			public void sqlite3OpenTable(///
			///<summary>
			///Generate code into this VDBE 
			///</summary>
			int iCur,///
			///<summary>
			///The cursor number of the table 
			///</summary>
			int iDb,///
			///<summary>
			///The database index in sqlite3.aDb[] 
			///</summary>
			Table pTab,///
			///<summary>
			///The table to be opened 
			///</summary>
			OpCode opcode///
			///<summary>
			///OP_OpenRead or  OpCode.OP_OpenWrite 
			///</summary>
			) {
				Vdbe v;
				if(pTab.IsVirtual())
					return;
				v=this.sqlite3GetVdbe();
				Debug.Assert(opcode== OpCode.OP_OpenWrite||opcode== OpCode.OP_OpenRead);
                sqliteinth.sqlite3TableLock(this, iDb, pTab.tnum, (opcode == OpCode.OP_OpenWrite) ? (byte)1 : (byte)0, pTab.zName);
				v.sqlite3VdbeAddOp3(opcode,iCur,pTab.tnum,iDb);
				v.sqlite3VdbeChangeP4(-1,(pTab.nCol), P4Usage.P4_INT32);
				//SQLITE_INT_TO_PTR( pTab.nCol ),  P4Usage.P4_INT32 );
				v.VdbeComment("%s",pTab.zName);
			}
			public void sqlite3Update(///
			///<summary>
			///The parser context 
			///</summary>
			SrcList pTabList,///
			///<summary>
			///The table in which we should change things 
			///</summary>
			ExprList pChanges,///
			///<summary>
			///Things to be changed 
			///</summary>
			Expr pWhere,///
			///<summary>
			///The WHERE clause.  May be null 
			///</summary>
			OnConstraintError onError///
			///<summary>
			///How to handle constraint errors 
			///</summary>
			) {
				int i,j;
				///
				///<summary>
				///Loop counters 
				///</summary>
				Table pTab;
				///
				///<summary>
				///The table to be updated 
				///</summary>
				int addr=0;
				///
				///<summary>
				///VDBE instruction address of the start of the loop 
				///</summary>
				WhereInfo pWInfo;
				///
				///<summary>
				///Information about the WHERE clause 
				///</summary>
				Vdbe v;
				///
				///<summary>
				///The virtual database engine 
				///</summary>
				Index pIdx;
				///
				///<summary>
				///For looping over indices 
				///</summary>
				int nIdx;
				///
				///<summary>
				///Number of indices that need updating 
				///</summary>
				int iCur;
				///
				///<summary>
				///VDBE Cursor number of pTab 
				///</summary>
				Connection db;
				///
				///<summary>
				///The database structure 
				///</summary>
				int[] aRegIdx=null;
				///
				///<summary>
				///One register assigned to each index to be updated 
				///</summary>
				int[] aXRef=null;
				///
				///<summary>
				///aXRef[i] is the index in pChanges.a[] of the
				///</summary>
				///<param name="an expression for the i">th column of the table.</param>
				///<param name="aXRef[i]==">th column is not changed. </param>
				bool chngRowid;
				///
				///<summary>
				///True if the record number is being changed 
				///</summary>
				Expr pRowidExpr=null;
				///
				///<summary>
				///Expression defining the new record number 
				///</summary>
				bool openAll=false;
				///
				///<summary>
				///True if all indices need to be opened 
				///</summary>
				AuthContext sContext;
				///
				///<summary>
				///The authorization context 
				///</summary>
				NameContext sNC;
				///
				///<summary>
				///</summary>
				///<param name="The name">context to resolve expressions in </param>
				int iDb;
				///
				///<summary>
				///Database containing the table being updated 
				///</summary>
				bool okOnePass;
				///
				///<summary>
				///</summary>
				///<param name="True for one">pass algorithm without the FIFO </param>
				bool hasFK;
				///
				///<summary>
				///True if foreign key processing is required 
				///</summary>
				#if !SQLITE_OMIT_TRIGGER
				bool isView;
				///
				///<summary>
				///True when updating a view (INSTEAD OF trigger) 
				///</summary>
				Trigger pTrigger;
				///
				///<summary>
				///List of triggers on pTab, if required 
				///</summary>
				TriggerType tmask=0;
				///
				///<summary>
				///Mask of TriggerType.TRIGGER_BEFORE|TriggerType.TRIGGER_AFTER 
				///</summary>
				#endif
				int newmask;
				///
				///<summary>
				///Mask of NEW.* columns accessed by BEFORE triggers 
				///</summary>
				///
				///<summary>
				///Register Allocations 
				///</summary>
				int regRowCount=0;
				///
				///<summary>
				///A count of rows changed 
				///</summary>
				int regOldRowid;
				///
				///<summary>
				///The old rowid 
				///</summary>
				int regNewRowid;
				///
				///<summary>
				///The new rowid 
				///</summary>
				int regNew;
				int regOld=0;
				int regRowSet=0;
				///
				///<summary>
				///Rowset of rows to be updated 
				///</summary>
				sContext=new AuthContext();
				//memset( &sContext, 0, sizeof( sContext ) );
				db=this.db;
				if(this.nErr!=0///
				///<summary>
				///|| db.mallocFailed != 0 
				///</summary>
				) {
					goto update_cleanup;
				}
				Debug.Assert(pTabList.Count==1);
				///Locate the table which we want to update.
				pTab=this.GetFirstTableInTheList(pTabList);
				if(pTab==null)
					goto update_cleanup;
				iDb= this.db.indexOfBackendWithSchema(pTab.pSchema);
				///
				///<summary>
				///Figure out if we have any triggers and if the table being
				///updated is a view.
				///
				///</summary>
				#if !SQLITE_OMIT_TRIGGER
				pTrigger= TriggerParser.sqlite3TriggersExist(this,pTab,TokenType.TK_UPDATE,pChanges,out tmask);
				isView=pTab.pSelect!=null;
				Debug.Assert(pTrigger!=null||tmask==0);
				#else
																																																																																																					      const Trigger pTrigger = null;// define pTrigger 0
      const int tmask = 0;          // define tmask 0
#endif
				#if SQLITE_OMIT_TRIGGER || SQLITE_OMIT_VIEW
																																																																																																					//     undef isView
      const bool isView = false;    // define isView 0
#endif
				if(build.sqlite3ViewGetColumnNames(this,pTab)!=0) {
					goto update_cleanup;
				}
				if(this.sqlite3IsReadOnly(pTab,tmask)) {
					goto update_cleanup;
				}
				aXRef=new int[pTab.nCol];
				// sqlite3DbMallocRaw(db, sizeof(int) * pTab.nCol);
				//if ( aXRef == null ) goto update_cleanup;
				for(i=0;i<pTab.nCol;i++)
					aXRef[i]=-1;
				///
				///<summary>
				///Allocate a cursors for the main database table and for all indices.
				///The index cursors might not be used, but if they are used they
				///need to occur right after the database cursor.  So go ahead and
				///allocate enough space, just in case.
				///
				///</summary>
				pTabList.a[0].iCursor=iCur=this.nTab++;
				for(pIdx=pTab.pIndex;pIdx!=null;pIdx=pIdx.pNext) {
					this.nTab++;
				}
				///
				///<summary>
				///</summary>
				///<param name="Initialize the name">context </param>
				sNC=new NameContext();
				// memset(&sNC, 0, sNC).Length;
				sNC.pParse=this;
				sNC.pSrcList=pTabList;
				///
				///<summary>
				///Resolve the column names in all the expressions of the
				///of the UPDATE statement.  Also find the column index
				///for each column to be updated in the pChanges array.  For each
				///column to be updated, make sure we have authorization to change
				///that column.
				///
				///</summary>
				chngRowid=false;
				for(i=0;i<pChanges.Count;i++) {
					if(ResolveExtensions.sqlite3ResolveExprNames(sNC,ref pChanges.a[i].pExpr)!=0) {
						goto update_cleanup;
					}
					for(j=0;j<pTab.nCol;j++) {
						if(pTab.aCol[j].zName.Equals(pChanges.a[i].zName,StringComparison.InvariantCultureIgnoreCase)) {
							if(j==pTab.iPKey) {
								chngRowid=true;
								pRowidExpr=pChanges.a[i].pExpr;
							}
							aXRef[j]=i;
							break;
						}
					}
					if(j>=pTab.nCol) {
						if(exprc.sqlite3IsRowid(pChanges.a[i].zName)) {
							chngRowid=true;
							pRowidExpr=pChanges.a[i].pExpr;
						}
						else {
							utilc.sqlite3ErrorMsg(this,"no such column: %s",pChanges.a[i].zName);
							this.checkSchema=1;
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
				hasFK=this.sqlite3FkRequired(pTab,aXRef,chngRowid?1:0)!=false;
				///
				///<summary>
				///Allocate memory for the array aRegIdx[].  There is one entry in the
				///array for each index associated with table being updated.  Fill in
				///the value with a register number for indices that are to be used
				///and with zero for unused indices.
				///
				///</summary>
				for(nIdx=0,pIdx=pTab.pIndex;pIdx!=null;pIdx=pIdx.pNext,nIdx++) {
				}
				if(nIdx>0) {
					aRegIdx=new int[nIdx];
					// sqlite3DbMallocRaw(db, Index*.Length * nIdx);
					if(aRegIdx==null)
						goto update_cleanup;
				}
				for(j=0,pIdx=pTab.pIndex;pIdx!=null;pIdx=pIdx.pNext,j++) {
					int reg;
					if(hasFK||chngRowid) {
						reg=++this.UsedCellCount;
					}
					else {
						reg=0;
						for(i=0;i<pIdx.nColumn;i++) {
							if(aXRef[pIdx.aiColumn[i]]>=0) {
								reg=++this.UsedCellCount;
								break;
							}
						}
					}
					aRegIdx[j]=reg;
				}
				///
				///<summary>
				///Begin generating code. 
				///</summary>
				v=this.sqlite3GetVdbe();
				if(v==null)
					goto update_cleanup;
				if(this.nested==0)
					v.sqlite3VdbeCountChanges();
				build.sqlite3BeginWriteOperation(this,1,iDb);
				#if !SQLITE_OMIT_VIRTUALTABLE
				///
				///<summary>
				///Virtual tables must be handled separately 
				///</summary>
				if(pTab.IsVirtual()) {
					this.updateVirtualTable(pTabList,pTab,pChanges,pRowidExpr,aXRef,pWhere,onError);
					pWhere=null;
					pTabList=null;
					goto update_cleanup;
				}
				#endif
				///
				///<summary>
				///Allocate required registers. 
				///</summary>
				regOldRowid=regNewRowid=++this.UsedCellCount;
				if(pTrigger!=null||hasFK) {
					regOld=this.UsedCellCount+1;
					this.UsedCellCount+=pTab.nCol;
				}
				if(chngRowid||pTrigger!=null||hasFK) {
					regNewRowid=++this.UsedCellCount;
				}
				regNew=this.UsedCellCount+1;
				this.UsedCellCount+=pTab.nCol;
				///
				///<summary>
				///Start the view context. 
				///</summary>
				if(isView) {
                    sqliteinth.sqlite3AuthContextPush(this, sContext, pTab.zName);
				}
				///
				///<summary>
				///If we are trying to update a view, realize that view into
				///a ephemeral table.
				///
				///</summary>
				#if !(SQLITE_OMIT_VIEW) && !(SQLITE_OMIT_TRIGGER)
				if(isView) {
					this.sqlite3MaterializeView(pTab,pWhere,iCur);
				}
				#endif
				///
				///<summary>
				///Resolve the column names in all the expressions in the
				///WHERE clause.
				///</summary>
				if(ResolveExtensions.sqlite3ResolveExprNames(sNC,ref pWhere)!=0) {
					goto update_cleanup;
				}
				///
				///<summary>
				///Begin the database scan
				///
				///</summary>
                v.sqlite3VdbeAddOp2(OpCode.OP_Null, 0, regOldRowid);
				ExprList NullOrderby=null;
				pWInfo=this.sqlite3WhereBegin(pTabList,pWhere,ref NullOrderby,wherec.WHERE_ONEPASS_DESIRED);
				if(pWInfo==null)
					goto update_cleanup;
				okOnePass=pWInfo.okOnePass!=0;
				///
				///<summary>
				///Remember the rowid of every item to be updated.
				///
				///</summary>
                v.sqlite3VdbeAddOp2(OpCode.OP_Rowid, iCur, regOldRowid);
				if(!okOnePass) {
					regRowSet=++this.UsedCellCount;
					v.sqlite3VdbeAddOp2( OpCode.OP_RowSetAdd,regRowSet,regOldRowid);
				}
				///
				///<summary>
				///End the database scan loop.
				///
				///</summary>
				pWInfo.sqlite3WhereEnd();
				///
				///<summary>
				///Initialize the count of updated rows
				///
				///</summary>
                if ((db.flags & SqliteFlags.SQLITE_CountRows) != 0 && null == this.pTriggerTab)
                {
					regRowCount=++this.UsedCellCount;
					v.sqlite3VdbeAddOp2(OpCode.OP_Integer,0,regRowCount);
				}
				if(!isView) {
					///
					///<summary>
					///Open every index that needs updating.  Note that if any
					///index could potentially invoke a REPLACE conflict resolution
					///action, then we need to open all indices because we might need
					///to be deleting some records.
					///
					///</summary>
					if(!okOnePass)
                        this.sqlite3OpenTable(iCur, iDb, pTab, OpCode.OP_OpenWrite);
					if(onError==OnConstraintError.OE_Replace) {
						openAll=true;
					}
					else {
						openAll=false;
						for(pIdx=pTab.pIndex;pIdx!=null;pIdx=pIdx.pNext) {
							if(pIdx.onError==OnConstraintError.OE_Replace) {
								openAll=true;
								break;
							}
						}
					}
					for(i=0,pIdx=pTab.pIndex;pIdx!=null;pIdx=pIdx.pNext,i++) {
						if(openAll||aRegIdx[i]>0) {
                            KeyInfo pKey = pIdx.sqlite3IndexKeyinfo(this);
                            v.sqlite3VdbeAddOp4(OpCode.OP_OpenWrite, iCur + i + 1, pIdx.tnum, iDb, pKey, P4Usage.P4_KEYINFO_HANDOFF);
							Debug.Assert(this.nTab>iCur+i+1);
						}
					}
				}
				///
				///<summary>
				///Top of the update loop 
				///</summary>
				if(okOnePass) {
					int a1=v.sqlite3VdbeAddOp1(OpCode.OP_NotNull,regOldRowid);
					addr=v.sqlite3VdbeAddOp0(OpCode.OP_Goto);
					v.sqlite3VdbeJumpHere(a1);
				}
				else {
					addr=v.sqlite3VdbeAddOp3( OpCode.OP_RowSetRead,regRowSet,0,regOldRowid);
				}
				///
				///<summary>
				///Make cursor iCur point to the record that is being updated. If
				///this record does not exist for some reason (deleted by a trigger,
				///for example, then jump to the next iteration of the RowSet loop.  
				///</summary>
                v.sqlite3VdbeAddOp3(OpCode.OP_NotExists, iCur, addr, regOldRowid);
				///
				///<summary>
				///If the record number will change, set register regNewRowid to
				///contain the new value. If the record number is not being modified,
				///then regNewRowid is the same register as regOldRowid, which is
				///already populated.  
				///</summary>
				Debug.Assert(chngRowid||pTrigger!=null||hasFK||regOldRowid==regNewRowid);
				if(chngRowid) {
					this.sqlite3ExprCode(pRowidExpr,regNewRowid);
					v.sqlite3VdbeAddOp1(OpCode.OP_MustBeInt,regNewRowid);
				}
				///
				///<summary>
				///If there are triggers on this table, populate an array of registers 
				///with the required old.* column data.  
				///</summary>
				if(hasFK||pTrigger!=null) {
					u32 oldmask=(hasFK?this.sqlite3FkOldmask(pTab):0);
					oldmask|= TriggerParser.sqlite3TriggerColmask(this,pTrigger,pChanges,0,TriggerType.TRIGGER_BEFORE|TriggerType.TRIGGER_AFTER,pTab,onError);
					for(i=0;i<pTab.nCol;i++) {
						if(aXRef[i]<0||oldmask==0xffffffff||(i<32&&0!=(oldmask&(1<<i)))) {
							v.codegenExprCodeGetColumnOfTable(pTab,iCur,i,regOld+i);
						}
						else {
                            v.sqlite3VdbeAddOp2(OpCode.OP_Null, 0, regOld + i);
						}
					}
					if(chngRowid==false) {
                        v.sqlite3VdbeAddOp2(OpCode.OP_Copy, regOldRowid, regNewRowid);
					}
				}
				///
				///<summary>
				///Populate the array of registers beginning at regNew with the new
				///row data. This array is used to check constaints, create the new
				///table and index records, and as the values for any new.* references
				///made by triggers.
				///
				///If there are one or more BEFORE triggers, then do not populate the
				///registers associated with columns that are (a) not modified by
				///this UPDATE statement and (b) not accessed by new.* references. The
				///values for registers not modified by the UPDATE must be reloaded from 
				///the database after the BEFORE triggers are fired anyway (as the trigger 
				///may have modified them). So not loading those that are not going to
				///be used eliminates some redundant opcodes.
				///
				///</summary>
				newmask=(int)TriggerParser.sqlite3TriggerColmask(this,pTrigger,pChanges,1,TriggerType.TRIGGER_BEFORE,pTab,onError);
				for(i=0;i<pTab.nCol;i++) {
					if(i==pTab.iPKey) {
                        v.sqlite3VdbeAddOp2(OpCode.OP_Null, 0, regNew + i);
					}
					else {
						j=aXRef[i];
						if(j>=0) {
							this.sqlite3ExprCode(pChanges.a[j].pExpr,regNew+i);
						}
						else
							if(0==(tmask&TriggerType.TRIGGER_BEFORE)||i>31||(newmask&(1<<i))!=0) {
								///
								///<summary>
								///This branch loads the value of a column that will not be changed 
								///into a register. This is done if there are no BEFORE triggers, or
								///if there are one or more BEFORE triggers that use this value via
								///a new.* reference in a trigger program.
								///
								///</summary>
								sqliteinth.testcase(i==31);
								sqliteinth.testcase(i==32);
								v.sqlite3VdbeAddOp3( OpCode.OP_Column,iCur,i,regNew+i);
								v.codegenColumnDefault(pTab,i,regNew+i);
							}
					}
				}
				///
				///<summary>
				///Fire any BEFORE UPDATE triggers. This happens before constraints are
				///verified. One could argue that this is wrong.
				///
				///</summary>
				if((tmask&TriggerType.TRIGGER_BEFORE)!=0) {
                    v.sqlite3VdbeAddOp2(OpCode.OP_Affinity, regNew, pTab.nCol);
					v.sqlite3TableAffinityStr(pTab);
                    TriggerBuilder.sqlite3CodeRowTrigger(this,pTrigger,TokenType.TK_UPDATE,pChanges,TriggerType.TRIGGER_BEFORE,pTab,regOldRowid,onError,addr);
					///
					///<summary>
					///</summary>
					///<param name="The row">trigger may have deleted the row being updated. In this</param>
					///<param name="case, jump to the next row. No updates or AFTER triggers are ">case, jump to the next row. No updates or AFTER triggers are </param>
					///<param name="required. This behaviour "> what happens when the row being updated</param>
					///<param name="is deleted or renamed by a BEFORE trigger "> is left undefined in the</param>
					///<param name="documentation.">documentation.</param>
					///<param name=""></param>
                    v.sqlite3VdbeAddOp3(OpCode.OP_NotExists, iCur, addr, regOldRowid);
					///<param name="If it did not delete it, the row">trigger may still have modified </param>
					///<param name="some of the columns of the row being updated. Load the values for ">some of the columns of the row being updated. Load the values for </param>
					///<param name="all columns not modified by the update statement into their ">all columns not modified by the update statement into their </param>
					///<param name="registers in case this has happened.">registers in case this has happened.</param>
					for(i=0;i<pTab.nCol;i++) {
						if(aXRef[i]<0&&i!=pTab.iPKey) {
							v.sqlite3VdbeAddOp3( OpCode.OP_Column,iCur,i,regNew+i);
							v.codegenColumnDefault(pTab,i,regNew+i);
						}
					}
				}
				if(!isView) {
					int j1;
					///Address of jump instruction 
					///
					///Do constraint checks. 
					int iDummy;
					this.sqlite3GenerateConstraintChecks(pTab,iCur,regNewRowid,aRegIdx,(chngRowid?regOldRowid:0),true,onError,addr,out iDummy);
					///Do FK constraint checks. 
					if(hasFK) {
						this.sqlite3FkCheck(pTab,regOldRowid,0);
					}
					///Delete the index entries associated with the current record.  
					j1=v.sqlite3VdbeAddOp3( OpCode.OP_NotExists,iCur,0,regOldRowid);
					this.codegenRowIndexDelete(pTab,iCur,aRegIdx);
					///If changing the record number, delete the old record.  
					if(hasFK||chngRowid) {
						v.sqlite3VdbeAddOp2( OpCode.OP_Delete,iCur,0);
					}
					v.sqlite3VdbeJumpHere(j1);
					if(hasFK) {
						this.sqlite3FkCheck(pTab,0,regNewRowid);
					}
					///Insert the new index entries and the new record. 
					this.sqlite3CompleteInsertion(pTab,iCur,regNewRowid,aRegIdx,true,false,false);
					///Do any ON CASCADE, SET NULL or SET DEFAULT operations required to
					///handle rows (possibly in other tables) that refer via a foreign key
					///to the row just updated. 
					if(hasFK) {
						this.sqlite3FkActions(pTab,pChanges,regOldRowid);
					}
				}
				///
				///<summary>
				///Increment the row counter 
				///
				///</summary>
                if ((db.flags & SqliteFlags.SQLITE_CountRows) != 0 && null == this.pTriggerTab)
                {
					v.sqlite3VdbeAddOp2(OpCode.OP_AddImm,regRowCount,1);
				}
                TriggerBuilder.sqlite3CodeRowTrigger(this,pTrigger,TokenType.TK_UPDATE,pChanges,TriggerType.TRIGGER_AFTER,pTab,regOldRowid,onError,addr);
				///
				///<summary>
				///Repeat the above with the next record to be updated, until
				///all record selected by the WHERE clause have been updated.
				///
				///</summary>
				v.sqlite3VdbeAddOp2(OpCode.OP_Goto,0,addr);
				v.sqlite3VdbeJumpHere(addr);
				///
				///<summary>
				///Close all tables 
				///</summary>
				for(i=0,pIdx=pTab.pIndex;pIdx!=null;pIdx=pIdx.pNext,i++) {
					if(openAll||aRegIdx[i]>0) {
                        v.sqlite3VdbeAddOp2(OpCode.OP_Close, iCur + i + 1, 0);
					}
				}
                v.sqlite3VdbeAddOp2(OpCode.OP_Close, iCur, 0);
				///
				///<summary>
				///Update the sqlite_sequence table by storing the content of the
				///maximum rowid counter values recorded while inserting into
				///autoincrement tables.
				///
				///</summary>
				if(this.nested==0&&this.pTriggerTab==null) {
					this.sqlite3AutoincrementEnd();
				}
				///
				///<summary>
				///Return the number of rows that were changed. If this routine is 
				///generating code because of a call to build.sqlite3NestedParse(), do not
				///invoke the callback function.
				///
				///</summary>
                if ((db.flags & SqliteFlags.SQLITE_CountRows) != 0 && null == this.pTriggerTab && 0 == this.nested)
                {
					v.sqlite3VdbeAddOp2(OpCode.OP_ResultRow,regRowCount,1);
					v.sqlite3VdbeSetNumCols(1);
                    v.sqlite3VdbeSetColName(0, ColName.NAME, "rows updated", SQLITE_STATIC);
				}
				update_cleanup:
				#if !SQLITE_OMIT_AUTHORIZATION
																																																																																																					sqlite3AuthContextPop(sContext);
#endif
				db.sqlite3DbFree(ref aRegIdx);
				db.sqlite3DbFree(ref aXRef);
				build.sqlite3SrcListDelete(db,ref pTabList);
				exprc.Delete(db,ref pChanges);
				exprc.Delete(db,ref pWhere);
				return;
			}
			public void updateVirtualTable(///
			///<summary>
			///The parsing context 
			///</summary>
			SrcList pSrc,///
			///<summary>
			///The virtual table to be modified 
			///</summary>
			Table pTab,///
			///<summary>
			///The virtual table 
			///</summary>
			ExprList pChanges,///
			///<summary>
			///The columns to change in the UPDATE statement 
			///</summary>
			Expr pRowid,///
			///<summary>
			///Expression used to recompute the rowid 
			///</summary>
			int[] aXRef,///
			///<summary>
			///Mapping from columns of pTab to entries in pChanges 
			///</summary>
			Expr pWhere,///
			///<summary>
			///WHERE clause of the UPDATE statement 
			///</summary>
			OnConstraintError onError///
			///<summary>
			///ON CONFLICT strategy 
			///</summary>
			) {
				Vdbe v=this.pVdbe;
				///
				///<summary>
				///Virtual machine under construction 
				///</summary>
				ExprList pEList=null;
				///
				///<summary>
				///The result set of the SELECT statement 
				///</summary>
				Select pSelect=null;
				///
				///<summary>
				///The SELECT statement 
				///</summary>
				Expr pExpr;
				///
				///<summary>
				///Temporary expression 
				///</summary>
				int ephemTab;
				///
				///<summary>
				///Table holding the result of the SELECT 
				///</summary>
				int i;
				///
				///<summary>
				///Loop counter 
				///</summary>
				int addr;
				///
				///<summary>
				///Address of top of loop 
				///</summary>
				int iReg;
				///
				///<summary>
				///First register in set passed to  OpCode.OP_VUpdate 
				///</summary>
				Connection db=this.db;
				///
				///<summary>
				///Database connection 
				///</summary>
                VTable pVTab = VTableMethodsExtensions.sqlite3GetVTable(db, pTab);
				SelectDest dest=new SelectDest();
				///
				///<summary>
				///Construct the SELECT statement that will find the new values for
				///all updated rows.
				///
				///</summary>
				pEList=CollectionExtensions.Append(null,exprc.sqlite3Expr(db,TokenType.TK_ID,"_rowid_"));
				if(pRowid!=null) {
					pEList=pEList.Append(exprc.sqlite3ExprDup(db,pRowid,0));
				}
				Debug.Assert(pTab.iPKey<0);
				for(i=0;i<pTab.nCol;i++) {
					if(aXRef[i]>=0) {
						pExpr=exprc.sqlite3ExprDup(db,pChanges.a[aXRef[i]].pExpr,0);
					}
					else {
						pExpr=exprc.sqlite3Expr(db,TokenType.TK_ID,pTab.aCol[i].zName);
					}
					pEList=pEList.Append(pExpr);
				}
				pSelect=Select.Create(this,pEList,pSrc,pWhere,null,null,null,0,null,null);
				///
				///<summary>
				///Create the ephemeral table into which the update results will
				///be stored.
				///
				///</summary>
				Debug.Assert(v!=null);
				ephemTab=this.nTab++;
                v.sqlite3VdbeAddOp2(OpCode.OP_OpenEphemeral, ephemTab, pTab.nCol + 1 + ((pRowid != null) ? 1 : 0));
				v.sqlite3VdbeChangeP5(BTREE_UNORDERED);
				///
				///<summary>
				///fill the ephemeral table
				///
				///</summary>
                dest.Init(SelectResultType.Table, ephemTab);
				Select.sqlite3Select(this,pSelect,ref dest);
				///
				///<summary>
				///Generate code to scan the ephemeral table and call VUpdate. 
				///</summary>
				iReg=++this.UsedCellCount;
				this.UsedCellCount+=pTab.nCol+1;
                addr = v.sqlite3VdbeAddOp2(OpCode.OP_Rewind, ephemTab, 0);
				v.sqlite3VdbeAddOp3( OpCode.OP_Column,ephemTab,0,iReg);
				v.sqlite3VdbeAddOp3( OpCode.OP_Column,ephemTab,(pRowid!=null?1:0),iReg+1);
				for(i=0;i<pTab.nCol;i++) {
					v.sqlite3VdbeAddOp3( OpCode.OP_Column,ephemTab,i+1+((pRowid!=null)?1:0),iReg+2+i);
				}
				this.sqlite3VtabMakeWritable(pTab);
                v.sqlite3VdbeAddOp4(OpCode.OP_VUpdate, 0, pTab.nCol + 2, iReg, pVTab,  P4Usage.P4_VTAB);
				v.sqlite3VdbeChangeP5((byte)(onError==OnConstraintError.OE_Default?OnConstraintError.OE_Abort:onError));
				build.sqlite3MayAbort(this);
				v.sqlite3VdbeAddOp2( OpCode.OP_Next,ephemTab,addr+1);
				v.sqlite3VdbeJumpHere(addr);
                v.sqlite3VdbeAddOp2(OpCode.OP_Close, ephemTab, 0);
				///Cleanup 
				SelectMethods.SelectDestructor(db,ref pSelect);
			}

            /// <summary>
            /// sqlite3SrcListLookup
            /// </summary>
            /// <param name="pSrc"></param>
            /// <returns></returns>
			public Table GetFirstTableInTheList(SrcList pSrc) {
				SrcList_item pItem=pSrc.a[0];				
				Debug.Assert(pItem!=null&&pSrc.Count==1);
				var pTab=TableBuilder.sqlite3LocateTable(this, pItem.zName, pItem.zDatabase);
				TableBuilder.sqlite3DeleteTable(this.db,ref pItem.pTab);
				pItem.pTab=pTab;
				if(pTab!=null) {
					pTab.nRef++;
				}
                if (SelectMethods.sqlite3IndexedByLookup(this, pItem) != 0)
                {
					pTab=null;
				}
				return pTab;
			}

			public bool sqlite3IsReadOnly(Table pTab,TriggerType viewOk) {
				///
				///<summary>
				///A table is not writable under the following circumstances:
				///
				///1) It is a virtual table and no implementation of the xUpdate method
				///has been provided, or
				///2) It is a system table (i.e. sqlite_master), this call is not
				///part of a nested parse and writable_schema pragma has not
				///been specified.
				///
				///</summary>
				///<param name="In either case leave an error message in pParse and return non">zero.</param>
				///<param name=""></param>
				if((pTab.IsVirtual()&&VTableMethodsExtensions.sqlite3GetVTable(this.db,pTab).pMod.pModule.xUpdate==null)
                    ||
                    ((pTab.tabFlags & TableFlags.TF_Readonly) != 0 && (this.db.flags & SqliteFlags.SQLITE_WriteSchema) == 0 && this.nested == 0))
                {

					utilc.sqlite3ErrorMsg(this,"table %s may not be modified",pTab.zName);
					return true;
				}
				#if !SQLITE_OMIT_VIEW
				if(viewOk==(TriggerType)0&&pTab.pSelect!=null) {
					utilc.sqlite3ErrorMsg(this,"cannot modify %s because it is a view",pTab.zName);
					return true;
				}
				#endif
				return false;
			}
			public void sqlite3MaterializeView(///
			///<summary>
			///Parsing context 
			///</summary>
			Table pView,///
			///<summary>
			///View definition 
			///</summary>
			Expr pWhere,///
			///<summary>
			///Optional WHERE clause to be added 
			///</summary>
			int iCur///
			///<summary>
			///VdbeCursor number for ephemerial table 
			///</summary>
			) {
				SelectDest dest=new SelectDest();
				Select pDup;
				Connection db=this.db;
				pDup=exprc.sqlite3SelectDup(db,pView.pSelect,0);
				if(pWhere!=null) {
					SrcList pFrom;
					pWhere=exprc.sqlite3ExprDup(db,pWhere,0);
					pFrom=build.sqlite3SrcListAppend(db,null,null,null);
					//if ( pFrom != null )
					//{
					Debug.Assert(pFrom.Count==1);
					pFrom.a[0].zAlias=pView.zName;
					// sqlite3DbStrDup( db, pView.zName );
					pFrom.a[0].pSelect=pDup;
					Debug.Assert(pFrom.a[0].pOn==null);
					Debug.Assert(pFrom.a[0].pUsing==null);
					//}
					//else
					//{
					//  SelectMethods.sqlite3SelectDelete( db, ref pDup );
					//}
					pDup=Select.Create(this,null,pFrom,pWhere,null,null,null,0,null,null);
				}
                dest.Init( SelectResultType.EphemTab, iCur);
				Select.sqlite3Select(this,pDup,ref dest);
				SelectMethods.SelectDestructor(db,ref pDup);
			}
			public void sqlite3DeleteFrom(///
			///<summary>
			///The parser context 
			///</summary>
			SrcList pTabList,///
			///<summary>
			///The table from which we should delete things 
			///</summary>
			Expr pWhere///
			///<summary>
			///The WHERE clause.  May be null 
			///</summary>
			) {
				Vdbe v;
				///
				///<summary>
				///The virtual database engine 
				///</summary>
				Table pTab;
				///
				///<summary>
				///The table from which records will be deleted 
				///</summary>
				string zDb;
				///
				///<summary>
				///Name of database holding pTab 
				///</summary>
				int end,addr=0;
				///
				///<summary>
				///A couple addresses of generated code 
				///</summary>
				int i;
				///
				///<summary>
				///Loop counter 
				///</summary>
				WhereInfo pWInfo;
				///
				///<summary>
				///Information about the WHERE clause 
				///</summary>
				Index pIdx;
				///
				///<summary>
				///For looping over indices of the table 
				///</summary>
				int iCur;
				///
				///<summary>
				///VDBE VdbeCursor number for pTab 
				///</summary>
				Connection db;
				///
				///<summary>
				///Main database structure 
				///</summary>
				AuthContext sContext;
				///
				///<summary>
				///Authorization context 
				///</summary>
				NameContext sNC;
				///
				///<summary>
				///Name context to resolve expressions in 
				///</summary>
				int iDb;
				///
				///<summary>
				///Database number 
				///</summary>
				int memCnt=-1;
				///
				///<summary>
				///Memory cell used for change counting 
				///</summary>
				AuthResult rcauth;
				///
				///<summary>
				///Value returned by authorization callback 
				///</summary>
				#if !SQLITE_OMIT_TRIGGER
				bool isView;
				///
				///<summary>
				///True if attempting to delete from a view 
				///</summary>
				Trigger pTrigger;
				///
				///<summary>
				///List of table triggers, if required 
				///</summary>
				#endif
				sContext=new AuthContext();
				//memset(&sContext, 0, sizeof(sContext));
				db=this.db;
				if(this.nErr!=0///
				///<summary>
				///|| db.mallocFailed != 0 
				///</summary>
				) {
					goto delete_from_cleanup;
				}
				Debug.Assert(pTabList.Count==1);
				///
				///<summary>
				///Locate the table which we want to delete.  This table has to be
				///put in an SrcList structure because some of the subroutines we
				///will be calling are designed to work with multiple tables and expect
				///an SrcList* parameter instead of just a Table* parameter.
				///
				///</summary>
				pTab=this.GetFirstTableInTheList(pTabList);
				if(pTab==null)
					goto delete_from_cleanup;
				///
				///<summary>
				///Figure out if we have any triggers and if the table being
				///deleted from is a view
				///
				///</summary>
				#if !SQLITE_OMIT_TRIGGER
				TriggerType iDummy;
				pTrigger= TriggerParser.sqlite3TriggersExist(this,pTab,TokenType.TK_DELETE,null,out iDummy);
				isView=pTab.pSelect!=null;
				#else
																																																																																																					      const Trigger pTrigger = null;
      bool isView = false;
#endif
				#if SQLITE_OMIT_VIEW
																																																																																																					// undef isView
isView = false;
#endif
				///
				///<summary>
				///If pTab is really a view, make sure it has been initialized.
				///</summary>
				if(build.sqlite3ViewGetColumnNames(this,pTab)!=0) {
					goto delete_from_cleanup;
				}
				if(this.sqlite3IsReadOnly(pTab,(TriggerType)(pTrigger!=null?1:0))) {
					goto delete_from_cleanup;
				}
				iDb=db.indexOfBackendWithSchema(pTab.pSchema);
				Debug.Assert(iDb<db.BackendCount);
				zDb=db.Backends[iDb].Name;
				#if !SQLITE_OMIT_AUTHORIZATION
																																																																																																					rcauth = sqlite3AuthCheck(pParse, SQLITE_DELETE, pTab->zName, 0, zDb);
#else
                rcauth = AuthResult.SQLITE_OK;
				#endif
				Debug.Assert(rcauth==AuthResult.SQLITE_OK||rcauth==AuthResult.SQLITE_DENY||rcauth==AuthResult.SQLITE_IGNORE);
				if(rcauth==AuthResult.SQLITE_DENY) {
					goto delete_from_cleanup;
				}
                
				Debug.Assert(!isView||pTrigger!=null);
				///
				///<summary>
				///Assign  cursor number to the table and all its indices.
				///
				///</summary>
				Debug.Assert(pTabList.Count==1);
				iCur=pTabList.a[0].iCursor=this.nTab++;
				for(pIdx=pTab.pIndex;pIdx!=null;pIdx=pIdx.pNext) {
					this.nTab++;
				}
				#if !SQLITE_OMIT_AUTHORIZATION
																																																																																																					/* Start the view context
*/
if( isView ){
sqlite3AuthContextPush(pParse, sContext, pTab.zName);
}
#endif
				///
				///<summary>
				///Begin generating code.
				///</summary>
				v=this.sqlite3GetVdbe();
				if(v==null) {
					goto delete_from_cleanup;
				}
				if(this.nested==0)
					v.sqlite3VdbeCountChanges();
				build.sqlite3BeginWriteOperation(this,1,iDb);
				///
				///<summary>
				///If we are trying to delete from a view, realize that view into
				///a ephemeral table.
				///
				///</summary>
				#if !(SQLITE_OMIT_VIEW) && !(SQLITE_OMIT_TRIGGER)
				if(isView) {
					this.sqlite3MaterializeView(pTab,pWhere,iCur);
				}
				#endif
				///
				///<summary>
				///Resolve the column names in the WHERE clause.
				///
				///</summary>
				sNC=new NameContext();
				// memset( &sNC, 0, sizeof( sNC ) );
				sNC.pParse=this;
				sNC.pSrcList=pTabList;
				if(ResolveExtensions.sqlite3ResolveExprNames(sNC,ref pWhere)!=0) {
					goto delete_from_cleanup;
				}
				///
				///<summary>
				///Initialize the counter of the number of rows deleted, if
				///we are counting rows.
				///</summary>
                if ((db.flags & SqliteFlags.SQLITE_CountRows) != 0)
                {
					memCnt=++this.UsedCellCount;
					v.sqlite3VdbeAddOp2(OpCode.OP_Integer,0,memCnt);
				}
				#if !SQLITE_OMIT_TRUNCATE_OPTIMIZATION
				///
				///<summary>
				///Special case: A DELETE without a WHERE clause deletes everything.
				///It is easier just to erase the whole table. Prior to version 3.6.5,
				///this optimization caused the row change count (the value returned by 
				///API function sqlite3_count_changes) to be set incorrectly.  
				///</summary>
				if(rcauth==AuthResult.SQLITE_OK&&pWhere==null&&null==pTrigger&&!pTab.IsVirtual()&&false==this.sqlite3FkRequired(pTab,null,0)) {
					Debug.Assert(!isView);
                    v.sqlite3VdbeAddOp4(OpCode.OP_Clear, pTab.tnum, iDb, memCnt, pTab.zName, P4Usage.P4_STATIC);
					for(pIdx=pTab.pIndex;pIdx!=null;pIdx=pIdx.pNext) {
						Debug.Assert(pIdx.pSchema==pTab.pSchema);
                        v.sqlite3VdbeAddOp2(OpCode.OP_Clear, pIdx.tnum, iDb);
					}
				}
				else
				#endif
				///
				///<summary>
				///The usual case: There is a WHERE clause so we have to scan through
				///the table and pick which records to delete.
				///</summary>
				 {
					int iRowSet=++this.UsedCellCount;
					///
					///<summary>
					///Register for rowset of rows to delete 
					///</summary>
					int iRowid=++this.UsedCellCount;
					///
					///<summary>
					///Used for storing rowid values. 
					///</summary>
					int regRowid;
					///
					///<summary>
					///Actual register containing rowids 
					///</summary>
					///
					///<summary>
					///Collect rowids of every row to be deleted.
					///</summary>
                    v.sqlite3VdbeAddOp2(OpCode.OP_Null, 0, iRowSet);
					ExprList elDummy=null;
					pWInfo=this.sqlite3WhereBegin(pTabList,pWhere,ref elDummy,wherec.WHERE_DUPLICATES_OK);
					if(pWInfo==null)
						goto delete_from_cleanup;
					regRowid=this.sqlite3ExprCodeGetColumn(pTab,-1,iCur,iRowid);
					v.sqlite3VdbeAddOp2( OpCode.OP_RowSetAdd,iRowSet,regRowid);
                    if ((db.flags & SqliteFlags.SQLITE_CountRows) != 0)
                    {
						v.sqlite3VdbeAddOp2(OpCode.OP_AddImm,memCnt,1);
					}
					pWInfo.sqlite3WhereEnd();
					///
					///<summary>
					///Delete every item whose key was written to the list during the
					///database scan.  We have to delete items after the scan is complete
					///because deleting an item can change the scan order. 
					///</summary>
					end=v.sqlite3VdbeMakeLabel();
					///
					///<summary>
					///Unless this is a view, open cursors for the table we are 
					///deleting from and all its indices. If this is a view, then the
					///only effect this statement has is to fire the INSTEAD OF 
					///triggers.  
					///</summary>
					if(!isView) {
                        this.sqlite3OpenTableAndIndices(pTab, iCur, OpCode.OP_OpenWrite);
					}
					addr=v.sqlite3VdbeAddOp3( OpCode.OP_RowSetRead,iRowSet,end,iRowid);
					///Delete the row 
					#if !SQLITE_OMIT_VIRTUALTABLE
					if(pTab.IsVirtual()) {
                        VTable pVTab = VTableMethodsExtensions.sqlite3GetVTable(db, pTab);
						this.sqlite3VtabMakeWritable(pTab);
                        v.sqlite3VdbeAddOp4(OpCode.OP_VUpdate, 0, 1, iRowid, pVTab,  P4Usage.P4_VTAB);
						v.sqlite3VdbeChangeP5((byte)OnConstraintError.OE_Abort);
						build.sqlite3MayAbort(this);
					}
					else
					#endif
					 {
						int count=(this.nested==0)?1:0;
						///
						///<summary>
						///True to count changes 
						///</summary>
						this.codegenRowDelete(pTab,iCur,iRowid,count,pTrigger,OnConstraintError.OE_Default);
					}
					///
					///<summary>
					///End of the delete loop 
					///</summary>
					v.sqlite3VdbeAddOp2(OpCode.OP_Goto,0,addr);
					v.sqlite3VdbeResolveLabel(end);
					///
					///<summary>
					///Close the cursors open on the table and its indexes. 
					///</summary>
					if(!isView&&!pTab.IsVirtual()) {
						for(i=1,pIdx=pTab.pIndex;pIdx!=null;i++,pIdx=pIdx.pNext) {
                            v.sqlite3VdbeAddOp2(OpCode.OP_Close, iCur + i, pIdx.tnum);
						}
						v.sqlite3VdbeAddOp1(OpCode.OP_Close,iCur);
					}
				}
				///
				///<summary>
				///Update the sqlite_sequence table by storing the content of the
				///maximum rowid counter values recorded while inserting into
				///autoincrement tables.
				///
				///</summary>
				if(this.nested==0&&this.pTriggerTab==null) {
					this.sqlite3AutoincrementEnd();
				}
				///
				///<summary>
				///Return the number of rows that were deleted. If this routine is 
				///generating code because of a call to build.sqlite3NestedParse(), do not
				///invoke the callback function.
				///
				///</summary>
                if ((db.flags & SqliteFlags.SQLITE_CountRows) != 0 && 0 == this.nested && null == this.pTriggerTab)
                {
					v.sqlite3VdbeAddOp2(OpCode.OP_ResultRow,memCnt,1);
					v.sqlite3VdbeSetNumCols(1);
                    v.sqlite3VdbeSetColName(0, ColName.NAME, "rows deleted", SQLITE_STATIC);
				}
				delete_from_cleanup:
				#if !SQLITE_OMIT_AUTHORIZATION
																																																																																																					sqlite3AuthContextPop(sContext);
#endif
				build.sqlite3SrcListDelete(db,ref pTabList);
				exprc.Delete(db,ref pWhere);
				return;
			}

			public void codegenRowDelete(///
			    Table pTab,///
			    ///Table containing the row to be deleted 
			    int iCur,///
			    ///VdbeCursor number for the table 
			    int iRowid,///
			    ///Memory cell that contains the rowid to delete 
			    int count,///
			    ///<param name="If non">zero, increment the row change counter </param>
			    Trigger pTrigger,///
			    ///List of triggers to (potentially) fire 
			    OnConstraintError onconf///
			    ///Default ON CONFLICT policy for triggers 
			) {
				Vdbe v=this.pVdbe;
				///Vdbe 
				int iOld=0;
				///First register in OLD.* array 
				///
				///Vdbe is guaranteed to have been allocated by this stage. 
				Debug.Assert(v!=null);
                ///Seek cursor iCur to the row to delete. If this row no longer exists 
                ///(this can happen if a trigger program has already deleted it), do
                ///not attempt to delete it or fire any DELETE triggers.
                ///
                ///Label resolved to end of generated code 

                var iLabel =v.sqlite3VdbeMakeLabel();
                v.sqlite3VdbeAddOp3(OpCode.OP_NotExists, iCur, iLabel, iRowid);
				///If there are any triggers to fire, allocate a range of registers to
				///use for the old.* references in the triggers.  
				if(this.sqlite3FkRequired(pTab,null,0)!=false||pTrigger!=null) {
                    ///Iterator used while populating OLD.* 
                    ///
                    ///TODO: Could use temporary registers here. Also could attempt to
                    ///avoid copying the contents of the rowid register.  
                    ///Mask of OLD.* columns in use 

                    var mask = TriggerParser.sqlite3TriggerColmask(this,pTrigger,null,0,TriggerType.TRIGGER_BEFORE|TriggerType.TRIGGER_AFTER,pTab,onconf);
					mask|=this.sqlite3FkOldmask(pTab);
					iOld=this.UsedCellCount+1;
					this.UsedCellCount+=(1+pTab.nCol);
					///<param name="Populate the OLD.* pseudo">table register array. These values will be </param>
					///<param name="used by any BEFORE and AFTER triggers that exist.  ">used by any BEFORE and AFTER triggers that exist.  </param>
                    v.sqlite3VdbeAddOp2(OpCode.OP_Copy, iRowid, iOld);
					for(var iCol=0;iCol<pTab.nCol;iCol++) {
						if(mask==0xffffffff||(mask&(1<<iCol))!=0) {
							v.codegenExprCodeGetColumnOfTable(pTab,iCur,iCol,iOld+iCol+1);
						}
					}
                    ///Invoke BEFORE DELETE trigger programs. 
                    TriggerBuilder.sqlite3CodeRowTrigger(this,pTrigger,TokenType.TK_DELETE,null,TriggerType.TRIGGER_BEFORE,pTab,iOld,onconf,iLabel);
					///Seek the cursor to the row to be deleted again. It may be that
					///the BEFORE triggers coded above have already removed the row
					///being deleted. Do not attempt to delete the row a second time, and 
					///do not fire AFTER triggers.  
                    v.sqlite3VdbeAddOp3(OpCode.OP_NotExists, iCur, iLabel, iRowid);
					///Do FK processing. This call checks that any FK constraints that
					///refer to this table (i.e. constraints attached to other tables) 
					///are not violated by deleting this row.  
					this.sqlite3FkCheck(pTab,iOld,0);
				}
				///Delete the index and table entries. Skip this step if pTab is really
				///a view (in which case the only effect of the DELETE statement is to
				///fire the INSTEAD OF triggers).  
				if(pTab.pSelect==null) {
					this.codegenRowIndexDelete(pTab,iCur,0);
                    v.sqlite3VdbeAddOp2( OpCode.OP_Delete, iCur, (count != 0 ? (int)OpFlag.OPFLAG_NCHANGE : 0));
					if(count!=0) {
						v.sqlite3VdbeChangeP4(-1,pTab.zName, P4Usage.P4_TRANSIENT);
					}
				}
				///Do any ON CASCADE, SET NULL or SET DEFAULT operations required to
				///handle rows (possibly in other tables) that refer via a foreign key
				///to the row just deleted. 
				this.sqlite3FkActions(pTab,null,iOld);
                ///Invoke AFTER DELETE trigger programs. 
                TriggerBuilder.sqlite3CodeRowTrigger(this,pTrigger,TokenType.TK_DELETE,null,TriggerType.TRIGGER_AFTER,pTab,iOld,onconf,iLabel);
				///Jump here if the row had already been deleted before any BEFORE
				///trigger programs were invoked. Or if a trigger program throws a 
				///RAISE(IGNORE) exception.  
				v.sqlite3VdbeResolveLabel(iLabel);
			}
			public void codegenRowIndexDelete(
			    Table pTab,///Table containing the row to be deleted 
			    int iCur,///VdbeCursor number for the table 
			    int nothing///Only delete if aRegIdx!=0 && aRegIdx[i]>0 
			) {
				int[] aRegIdx=null;
				this.codegenRowIndexDelete(pTab,iCur,aRegIdx);
			}
			public void codegenRowIndexDelete(
			    Table pTab,
                ///Table containing the row to be deleted 
			    int iCur,///
			    ///VdbeCursor number for the table 
			    int[] aRegIdx///
			    ///Only delete if aRegIdx!=0 && aRegIdx[i]>0 
			) {
                pTab.pIndex.path(x => x.pNext).ForEach(
(Action<Index, int>)((pIdx, idx) => {//start with 1
                        var i = idx + 1;
                        if (aRegIdx != null && aRegIdx[i - 1] == 0)
                            return;
                        var r1 = this.codegenGenerateIndexKey(pIdx, iCur, 0, false);
                        this.pVdbe.sqlite3VdbeAddOp3(OpCode.OP_IdxDelete, iCur + i, r1, pIdx.nColumn + 1);
                    })
                );
                
			}
			
			public Expr sqlite3ExprSetCollByToken(Expr pExpr,Token pCollName) {
				string zColl;
				///Dequoted name of collation sequence 
				CollSeq pColl;
				Connection db=this.db;
				zColl=build.Token2Name(db,pCollName);
				pColl=build.sqlite3LocateCollSeq(this,zColl);
				pExpr.sqlite3ExprSetColl(pColl);
				db.DbFree(ref zColl);
				return pExpr;
			}
			
			
			
			public SqlResult sqlite3ExprCheckHeight(int nHeight) {
				var rc=SqlResult.SQLITE_OK;
				int mxHeight=this.db.aLimit[Globals.SQLITE_LIMIT_EXPR_DEPTH];
				if(nHeight>mxHeight) {
					utilc.sqlite3ErrorMsg(this,"Expression tree is too large (maximum depth %d)",mxHeight);
					rc=SqlResult.SQLITE_ERROR;
				}
				return rc;
			}
			public void sqlite3ExprSetHeight(Expr p) {
				p.exprSetHeight();
				this.sqlite3ExprCheckHeight(p.nHeight);
			}
			public Expr sqlite3PExpr(TokenType op,int null_3,int null_4,int null_5) {
				return this.sqlite3PExpr(op,null,null,null);
			}
			public Expr sqlite3PExpr(TokenType op,int null_3,int null_4,Token pToken) {
				return this.sqlite3PExpr(op,null,null,pToken);
			}
			public Expr sqlite3PExpr(TokenType op,Expr pLeft,int null_4,int null_5) {
				return this.sqlite3PExpr(op,pLeft,null,null);
			}
			public Expr sqlite3PExpr(TokenType op,Expr pLeft,int null_4,Token pToken) {
				return this.sqlite3PExpr(op,pLeft,null,pToken);
			}
			public Expr sqlite3PExpr(TokenType op,Expr pLeft,Expr pRight,int null_5) {
				return this.sqlite3PExpr(op,pLeft,pRight,null);
			}
			public Expr sqlite3PExpr(
                TokenType op,///Expression opcode 
			    Expr pLeft,///Left operand 
			    Expr pRight,///Right operand 
			    Token pToken///Argument Token 
			) {
				//Log.WriteHeader(@"sqlite3PExpr");
				//Log.Indent();
				//if (null != pToken)
				//{
				//    Log.WriteLine(pToken.Length);
				//    Log.WriteLine(pToken.zRestSql);
				//}
				Expr p=exprc.CreateExpr(this.db,op,pToken,true);
				exprc.sqlite3ExprAttachSubtrees(this.db,p,pLeft,pRight);
				if(p!=null) {
					this.sqlite3ExprCheckHeight(p.nHeight);
				}
				//Log.Unindent();
				return p;
			}
			public void sqlite3ExprCacheStore(int iTab,int iCol,int iReg) {
				int i;
				int minLru;
				int idxLru;
				sqliteinth.yColCache p=new sqliteinth.yColCache();
				Debug.Assert(iReg>0);
				///
				///<summary>
				///Register numbers are always positive 
				///</summary>
				Debug.Assert(iCol>=-1&&iCol<32768);
				///
				///<summary>
				///Finite column numbers 
				///</summary>
				///
				///<summary>
				///The SQLITE_ColumnCache flag disables the column cache.  This is used
				///</summary>
				///<param name="for testing only "> to verify that SQLite always gets the same answer</param>
				///<param name="with and without the column cache.">with and without the column cache.</param>
                if ((this.db.flags & SqliteFlags.SQLITE_ColumnCache) != 0)
					return;
				///
				///<summary>
				///First replace any existing entry.
				///
				///Actually, the way the column cache is currently used, we are guaranteed
				///that the object will never already be in cache.  Verify this guarantee.
				///
				///</summary>
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
				///
				///<summary>
				///Find an empty slot and replace it 
				///</summary>
                for (i = 0; i < sqliteinth.SQLITE_N_COLCACHE; i++)//p=pParse.aColCache... p++)
				 {
					p=this.aColCache[i];
					if(p.iReg==0) {
						p.iLevel=this.iCacheLevel;
						p.iTable=iTab;
						p.iColumn=iCol;
						p.iReg=iReg;
						p.tempReg=0;
						p.lru=this.iCacheCnt++;
						return;
					}
				}
				///
				///<summary>
				///Replace the last recently used 
				///</summary>
				minLru=0x7fffffff;
				idxLru=-1;
                for (i = 0; i < sqliteinth.SQLITE_N_COLCACHE; i++)//p=pParse.aColCache..., p++)
				 {
					p=this.aColCache[i];
					if(p.lru<minLru) {
						idxLru=i;
						minLru=p.lru;
					}
				}
				if(Sqlite3.ALWAYS(idxLru>=0)) {
					p=this.aColCache[idxLru];
					p.iLevel=this.iCacheLevel;
					p.iTable=iTab;
					p.iColumn=iCol;
					p.iReg=iReg;
					p.tempReg=0;
					p.lru=this.iCacheCnt++;
					return;
				}
			}
			
			
			
			public void sqlite3ExprCachePinRegister(int iReg) {
				int i;
				sqliteinth.yColCache p;
                for (i = 0; i < sqliteinth.SQLITE_N_COLCACHE; i++)//p=pParse->aColCache; i<SQLITE_N_COLCACHE; i++, p++)
				 {
					p=this.aColCache[i];
					if(p.iReg==iReg) {
						p.tempReg=0;
					}
				}
			}
			public int sqlite3ExprCodeGetColumn(///
			///<summary>
			///Parsing and code generating context 
			///</summary>
			Table pTab,///
			///<summary>
			///Description of the table we are reading from 
			///</summary>
			int iColumn,///
			///<summary>
			///Index of the table column 
			///</summary>
			int iTable,///
			///<summary>
			///The cursor pointing to the table 
			///</summary>
			int iReg///
			///<summary>
			///Store results here 
			///</summary>
			) {
				Vdbe v=this.pVdbe;
				int i;
				sqliteinth.yColCache p;
                for (i = 0; i < sqliteinth.SQLITE_N_COLCACHE; i++)
                {
					// p=pParse.aColCache, p++
					p=this.aColCache[i];
					if(p.iReg>0&&p.iTable==iTable&&p.iColumn==iColumn) {
						p.lru=this.iCacheCnt++;
						this.sqlite3ExprCachePinRegister(p.iReg);
						return p.iReg;
					}
				}
				Debug.Assert(v!=null);
				v.codegenExprCodeGetColumnOfTable(pTab,iTable,iColumn,iReg);
				this.sqlite3ExprCacheStore(iTable,iColumn,iReg);
				return iReg;
			}
			public void sqlite3ExprCacheClear() {
				int i;
				sqliteinth.yColCache p;
                for (i = 0; i < sqliteinth.SQLITE_N_COLCACHE; i++)// p=pParse.aColCache... p++)
				 {
					p=this.aColCache[i];
					if(p.iReg!=0) {
						this.cacheEntryClear(p);
						p.iReg=0;
					}
				}
			}
			public void sqlite3ExprCacheAffinityChange(int iStart,int iCount) {
				this.sqlite3ExprCacheRemove(iStart,iCount);
			}
			public void sqlite3ExprCodeMove(int iFrom,int iTo,int nReg) {
				int i;
				sqliteinth.yColCache p;
				if(NEVER(iFrom==iTo))
					return;
                this.pVdbe.sqlite3VdbeAddOp3(OpCode.OP_Move, iFrom, iTo, nReg);
                for (i = 0; i < sqliteinth.SQLITE_N_COLCACHE; i++)// p=pParse.aColCache... p++)
				 {
					p=this.aColCache[i];
					int x=p.iReg;
					if(x>=iFrom&&x<iFrom+nReg) {
						p.iReg+=iTo-iFrom;
					}
				}
			}
			
			public int usedAsColumnCache(int iFrom,int iTo) {
				return 0;
			}
			public override int sqlite3ExprCodeTarget(Expr pExpr,int target) {
				Vdbe v=this.pVdbe;
				///The VM under construction 
				int inReg=target;
				///Results stored in register inReg 
				int regFree1=0;
				///<param name="If non">zero free this temporary register </param>
				int regFree2=0;
				///<param name="If non">zero free this temporary register </param>
				int r1=0,r2=0,r3=0,r4=0;
				///Various register numbers 
				Connection db=this.db;
				///The database connection 
				Debug.Assert(target>0&&target<=this.UsedCellCount);
				if(v==null) {
					//Debug.Assert( pParse.db.mallocFailed != 0 );
					return 0;
				}

				///The opcode being coded 
                var op= pExpr?.Operator??TokenType.TK_NULL;
                
				switch(op) {
				case TokenType.TK_AGG_COLUMN:
				{
					AggInfo pAggInfo=pExpr.pAggInfo;
					AggInfo_col pCol=pAggInfo.aCol[pExpr.iAgg];
					if(pAggInfo.directMode==0) {
						Debug.Assert(pCol.iMem>0);
						inReg=pCol.iMem;
						break;
					}
					else
						if(pAggInfo.useSortingIdx!=0) {
							v.sqlite3VdbeAddOp3( OpCode.OP_Column,pAggInfo.sortingIdx,pCol.iSorterColumn,target);
							break;
						}
					///Otherwise, fall thru into the TokenType.TK_COLUMN case 
				}
				goto case TokenType.TK_COLUMN;
				case TokenType.TK_COLUMN: {
					if(pExpr.iTable<0) {
						///This only happens when coding check constraints 
						Debug.Assert(this.ckBase>0);
						inReg=pExpr.iColumn+this.ckBase;
					}
					else {
						inReg=this.sqlite3ExprCodeGetColumn(pExpr.pTab,pExpr.iColumn,pExpr.iTable,target);
					}
					break;
				}
				case TokenType.TK_INTEGER: {
					codeInteger(pExpr,false,target);
					break;
				}
				#if !SQLITE_OMIT_FLOATING_POINT
				case TokenType.TK_FLOAT: {
					Debug.Assert(!pExpr.HasProperty(ExprFlags.EP_IntValue));
					codeReal(v,pExpr.u.zToken,false,target);
					break;
				}
				#endif
				case TokenType.TK_STRING: {
					Debug.Assert(!pExpr.HasProperty(ExprFlags.EP_IntValue));
					v.sqlite3VdbeAddOp4(OpCode.OP_String8,0,target,0,pExpr.u.zToken,0);
					break;
				}
				case TokenType.TK_NULL: {
                    v.sqlite3VdbeAddOp2(OpCode.OP_Null, 0, target);
					break;
				}
				#if !SQLITE_OMIT_BLOB_LITERAL
				case TokenType.TK_BLOB: {
					int n;
					string z;
					byte[] zBlob;
					Debug.Assert(!pExpr.HasProperty(ExprFlags.EP_IntValue));
					Debug.Assert(pExpr.u.zToken[0]=='x'||pExpr.u.zToken[0]=='X');
					Debug.Assert(pExpr.u.zToken[1]=='\'');
					z=pExpr.u.zToken.Substring(2);
					n=StringExtensions.Strlen30(z)-1;
					Debug.Assert(z[n]=='\'');
					zBlob=Converter.sqlite3HexToBlob(v.sqlite3VdbeDb(),z,n);
                    v.sqlite3VdbeAddOp4(OpCode.OP_Blob, n / 2, target, 0, zBlob,  P4Usage.P4_DYNAMIC);
					break;
				}
				#endif
				case TokenType.TK_VARIABLE: {
					Debug.Assert(!pExpr.HasProperty(ExprFlags.EP_IntValue));
					Debug.Assert(pExpr.u.zToken!=null);
					Debug.Assert(pExpr.u.zToken.Length!=0);
                    v.sqlite3VdbeAddOp2(OpCode.OP_Variable, pExpr.iColumn, target);
					if(pExpr.u.zToken.Length>1) {
						Debug.Assert(pExpr.u.zToken[0]=='?'||pExpr.u.zToken.CompareTo(this.azVar[pExpr.iColumn-1])==0);
						v.sqlite3VdbeChangeP4(-1,this.azVar[pExpr.iColumn-1], P4Usage.P4_STATIC);
					}
					break;
				}
				case TokenType.TK_REGISTER: {
					inReg=pExpr.iTable;
					break;
				}
				case TokenType.TK_AS: {
					inReg=this.sqlite3ExprCodeTarget(pExpr.pLeft,target);
					break;
				}
				#if !SQLITE_OMIT_CAST
				case TokenType.TK_CAST: {
					///Expressions of the form:   CAST(pLeft AS token) 
					int aff;
                    OpCode to_op;
					inReg=this.sqlite3ExprCodeTarget(pExpr.pLeft,target);
					Debug.Assert(!pExpr.HasProperty(ExprFlags.EP_IntValue));
					aff=build.sqlite3AffinityType(pExpr.u.zToken);
					to_op=(OpCode)(aff-sqliteinth.SQLITE_AFF_TEXT+(int)OpCode.OP_ToText);
					Debug.Assert(to_op==   OpCode.OP_ToText||aff!=sqliteinth.SQLITE_AFF_TEXT);
                    Debug.Assert(to_op ==  OpCode.OP_ToBlob || aff != sqliteinth.SQLITE_AFF_NONE);
                    Debug.Assert(to_op ==  OpCode.OP_ToNumeric || aff != sqliteinth.SQLITE_AFF_NUMERIC);
                    Debug.Assert(to_op ==  OpCode.OP_ToInt || aff != sqliteinth.SQLITE_AFF_INTEGER);
                    Debug.Assert(to_op ==  OpCode.OP_ToReal || aff != sqliteinth.SQLITE_AFF_REAL);
					sqliteinth.testcase(to_op== OpCode.OP_ToText);
					sqliteinth.testcase(to_op== OpCode.OP_ToBlob);
					sqliteinth.testcase(to_op== OpCode.OP_ToNumeric);
					sqliteinth.testcase(to_op== OpCode.OP_ToInt);
					sqliteinth.testcase(to_op== OpCode.OP_ToReal);
					if(inReg!=target) {
                        v.sqlite3VdbeAddOp2(OpCode.OP_SCopy, inReg, target);
						inReg=target;
					}
					v.sqlite3VdbeAddOp1((OpCode)to_op,inReg);
					sqliteinth.testcase(this.usedAsColumnCache(inReg,inReg)!=0);
					this.sqlite3ExprCacheAffinityChange(inReg,1);
					break;
				}
				#endif
				case TokenType.TK_LT:
				case TokenType.TK_LE:
				case TokenType.TK_GT:
				case TokenType.TK_GE:
				case TokenType.TK_NE:
				case TokenType.TK_EQ: {
					Debug.Assert(TokenType.TK_LT.Equals( OpCode.OP_Lt));
                    Debug.Assert(TokenType.TK_LE.Equals(OpCode.OP_Le));
                    Debug.Assert(TokenType.TK_GT.Equals(OpCode.OP_Gt));
                    Debug.Assert(TokenType.TK_GE.Equals(OpCode.OP_Ge));
                    Debug.Assert(TokenType.TK_EQ.Equals(OpCode.OP_Eq));
                    Debug.Assert(TokenType.TK_NE.Equals(OpCode.OP_Ne));
					sqliteinth.testcase(op==TokenType.TK_LT);
					sqliteinth.testcase(op==TokenType.TK_LE);
					sqliteinth.testcase(op==TokenType.TK_GT);
					sqliteinth.testcase(op==TokenType.TK_GE);
					sqliteinth.testcase(op==TokenType.TK_EQ);
					sqliteinth.testcase(op==TokenType.TK_NE);
					r1=this.sqlite3ExprCodeTemp(pExpr.pLeft,ref regFree1);
					r2=this.sqlite3ExprCodeTemp(pExpr.pRight,ref regFree2);
                    this.codeCompare(pExpr.pLeft, pExpr.pRight, (OpCode)op, r1, r2, inReg, sqliteinth.SQLITE_STOREP2);
					sqliteinth.testcase(regFree1==0);
					sqliteinth.testcase(regFree2==0);
					break;
				}
				case TokenType.TK_IS:
				case TokenType.TK_ISNOT: {
					sqliteinth.testcase(op==TokenType.TK_IS);
					sqliteinth.testcase(op==TokenType.TK_ISNOT);
					r1=this.sqlite3ExprCodeTemp(pExpr.pLeft,ref regFree1);
					r2=this.sqlite3ExprCodeTemp(pExpr.pRight,ref regFree2);
					op=(op==TokenType.TK_IS)?TokenType.TK_EQ:TokenType.TK_NE;
                    this.codeCompare(pExpr.pLeft, pExpr.pRight, (OpCode)op, r1, r2, inReg, sqliteinth.SQLITE_STOREP2 | sqliteinth.SQLITE_NULLEQ);
					sqliteinth.testcase(regFree1==0);
					sqliteinth.testcase(regFree2==0);
					break;
				}
				case TokenType.TK_AND:
				case TokenType.TK_OR:
				case TokenType.TK_PLUS:
				case TokenType.TK_STAR:
				case TokenType.TK_MINUS:
				case TokenType.TK_REM:
				case TokenType.TK_BITAND:
				case TokenType.TK_BITOR:
				case TokenType.TK_SLASH:
				case TokenType.TK_LSHIFT:
				case TokenType.TK_RSHIFT:
				case TokenType.TK_CONCAT: {
                    Debug.Assert(TokenType.TK_AND.Equals(OpCode.OP_And));
                    Debug.Assert(TokenType.TK_OR.Equals(OpCode.OP_Or));
                    Debug.Assert(TokenType.TK_PLUS.Equals(OpCode.OP_Add));
                    Debug.Assert(TokenType.TK_MINUS.Equals(OpCode.OP_Subtract));
                    Debug.Assert(TokenType.TK_REM.Equals(OpCode.OP_Remainder));
                    Debug.Assert(TokenType.TK_BITAND.Equals(OpCode.OP_BitAnd));
                    Debug.Assert(TokenType.TK_BITOR.Equals(OpCode.OP_BitOr));
                    Debug.Assert(TokenType.TK_SLASH.Equals(OpCode.OP_Divide));
                    Debug.Assert(TokenType.TK_LSHIFT.Equals(OpCode.OP_ShiftLeft));
                    Debug.Assert(TokenType.TK_RSHIFT.Equals(OpCode.OP_ShiftRight));
                    Debug.Assert(TokenType.TK_CONCAT.Equals(OpCode.OP_Concat));
					sqliteinth.testcase(op==TokenType.TK_AND);
					sqliteinth.testcase(op==TokenType.TK_OR);
					sqliteinth.testcase(op==TokenType.TK_PLUS);
					sqliteinth.testcase(op==TokenType.TK_MINUS);
					sqliteinth.testcase(op==TokenType.TK_REM);
					sqliteinth.testcase(op==TokenType.TK_BITAND);
					sqliteinth.testcase(op==TokenType.TK_BITOR);
					sqliteinth.testcase(op==TokenType.TK_SLASH);
					sqliteinth.testcase(op==TokenType.TK_LSHIFT);
					sqliteinth.testcase(op==TokenType.TK_RSHIFT);
					sqliteinth.testcase(op==TokenType.TK_CONCAT);
					r1=this.sqlite3ExprCodeTemp(pExpr.pLeft,ref regFree1);
					r2=this.sqlite3ExprCodeTemp(pExpr.pRight,ref regFree2);
					v.sqlite3VdbeAddOp3(op,r2,r1,target);
					sqliteinth.testcase(regFree1==0);
					sqliteinth.testcase(regFree2==0);
					break;
				}
				case TokenType.TK_UMINUS: {
					Expr pLeft=pExpr.pLeft;
					Debug.Assert(pLeft!=null);
					if(pLeft.Operator==TokenType.TK_INTEGER) {
						codeInteger(pLeft,true,target);
						#if !SQLITE_OMIT_FLOATING_POINT
					}
					else
						if(pLeft.Operator==TokenType.TK_FLOAT) {
							Debug.Assert(!pExpr.HasProperty(ExprFlags.EP_IntValue));
							codeReal(v,pLeft.u.zToken,true,target);
							#endif
						}
						else {
							regFree1=r1=this.allocTempReg();
							v.sqlite3VdbeAddOp2(OpCode.OP_Integer,0,r1);
							r2=this.sqlite3ExprCodeTemp(pExpr.pLeft,ref regFree2);
							v.sqlite3VdbeAddOp3(OpCode.OP_Subtract,r2,r1,target);
							sqliteinth.testcase(regFree2==0);
						}
					inReg=target;
					break;
				}
				case TokenType.TK_BITNOT:
				case TokenType.TK_NOT: {
                    Debug.Assert(TokenType.TK_BITNOT.Equals(OpCode.OP_BitNot));
					Debug.Assert(TokenType.TK_NOT.Equals(OpCode.OP_Not));
					sqliteinth.testcase(op==TokenType.TK_BITNOT);
					sqliteinth.testcase(op==TokenType.TK_NOT);
					r1=this.sqlite3ExprCodeTemp(pExpr.pLeft,ref regFree1);
					sqliteinth.testcase(regFree1==0);
					inReg=target;
					v.sqlite3VdbeAddOp2(op,r1,inReg);
					break;
				}
				case TokenType.TK_ISNULL:
				case TokenType.TK_NOTNULL: {
					int addr;
					Debug.Assert((int)TokenType.TK_ISNULL==(int)OpCode.OP_IsNull);
					Debug.Assert((int)TokenType.TK_NOTNULL==(int)OpCode.OP_NotNull);
					sqliteinth.testcase(op==TokenType.TK_ISNULL);
					sqliteinth.testcase(op==TokenType.TK_NOTNULL);
					v.sqlite3VdbeAddOp2(OpCode.OP_Integer,1,target);
					r1=this.sqlite3ExprCodeTemp(pExpr.pLeft,ref regFree1);
					sqliteinth.testcase(regFree1==0);
					addr=v.sqlite3VdbeAddOp1((OpCode)op,r1);
					v.sqlite3VdbeAddOp2(OpCode.OP_AddImm,target,-1);
					v.sqlite3VdbeJumpHere(addr);
					break;
				}
				case TokenType.TK_AGG_FUNCTION: {
					AggInfo pInfo=pExpr.pAggInfo;
					if(pInfo==null) {
						Debug.Assert(!pExpr.HasProperty(ExprFlags.EP_IntValue));
						utilc.sqlite3ErrorMsg(this,"misuse of aggregate: %s()",pExpr.u.zToken);
					}
					else {
						inReg=pInfo.aFunc[pExpr.iAgg].iMem;
					}
					break;
				}
				case TokenType.TK_CONST_FUNC:
				case TokenType.TK_FUNCTION: {
					
					
					int nId;
					///Length of the function name in bytes 
					string zId;
					///The function name 
					int constMask=0;
					///Mask of function arguments that are constant 
					SqliteEncoding enc=sqliteinth.ENC(db);
					///The text encoding used by this database 
					CollSeq pColl=null;
					///A collating sequence 
					Debug.Assert(!pExpr.HasProperty(ExprFlags.EP_xIsSelect));
					sqliteinth.testcase(op==TokenType.TK_CONST_FUNC);
					sqliteinth.testcase(op==TokenType.TK_FUNCTION);
                            ExprList pFarg;
                            ///List of function arguments 
                            if (pExpr.ExprHasAnyProperty(ExprFlags.EP_TokenOnly)) {
						pFarg=null;
					}
					else {
						pFarg=pExpr.x.pList;
					}
                            ///Number of function arguments 
                            var nFarg =pFarg!=null?pFarg.Count:0;
					Debug.Assert(!pExpr.HasProperty(ExprFlags.EP_IntValue));
					zId=pExpr.u.zToken;
					nId=StringExtensions.Strlen30(zId);
                    var pDef = FuncDefTraverse.sqlite3FindFunction(this.db, zId, nId, nFarg, enc, 0);
					if(pDef==null) {
						utilc.sqlite3ErrorMsg(this,"unknown function: %.*s()",nId,zId);
						break;
					}
					///<param name="Attempt a direct implementation of the built">in COALESCE() and</param>
					///<param name="IFNULL() functions.  This avoids unnecessary evalation of">IFNULL() functions.  This avoids unnecessary evalation of</param>
					///<param name="arguments past the first non">NULL argument.</param>
                    if ((pDef.flags & FuncFlags.SQLITE_FUNC_COALESCE) != 0)
                    {
						int endCoalesce=v.sqlite3VdbeMakeLabel();
						Debug.Assert(nFarg>=2);
						this.sqlite3ExprCode(pFarg.a[0].pExpr,target);
						for(var i=1;i<nFarg;i++) {
                            v.sqlite3VdbeAddOp2(OpCode.OP_NotNull, target, endCoalesce);
							this.sqlite3ExprCacheRemove(target,1);
							this.sqlite3ExprCachePush();
							this.sqlite3ExprCode(pFarg.a[i].pExpr,target);
							this.sqlite3ExprCachePop(1);
						}
						v.sqlite3VdbeResolveLabel(endCoalesce);
						break;
					}
					if(pFarg!=null) {
						r1=this.sqlite3GetTempRange(nFarg);
						this.sqlite3ExprCachePush();
						///Ticket 2ea2425d34be 
						this.sqlite3ExprCodeExprList(pFarg,r1,true);
						this.sqlite3ExprCachePop(1);
						///Ticket 2ea2425d34be 
					}
					else {
						r1=0;
					}
					#if !SQLITE_OMIT_VIRTUALTABLE
					///
					///<summary>
					///Possibly overload the function if the first argument is
					///a virtual table column.
					///
					///For infix functions (LIKE, GLOB, REGEXP, and MATCH) use the
					///second argument, not the first, as the argument to test to
					///see if it is a column in a virtual table.  This is done because
					///the left operand of infix functions (the operand we want to
					///control overloading) ends up as the second argument to the
					///function.  The expression "A glob B" is equivalent to
					///"glob(B,A).  We want to use the A in "A glob B" to test
					///for function overloading.  But we use the B term in "glob(B,A)".
					///</summary>
					if(nFarg>=2&&(pExpr.Flags&ExprFlags.EP_InfixFunc)!=0) {
                        pDef = VTableMethodsExtensions.sqlite3VtabOverloadFunction(db, pDef, nFarg, pFarg.a[1].pExpr);
					}
					else
						if(nFarg>0) {
                            pDef = VTableMethodsExtensions.sqlite3VtabOverloadFunction(db, pDef, nFarg, pFarg.a[0].pExpr);
						}
					#endif
					for(var i=0;i<nFarg;i++) {
						if(i<32&&pFarg.a[i].pExpr.sqlite3ExprIsConstant()!=0) {
							constMask|=(1<<i);
						}
						if((pDef.flags& FuncFlags.SQLITE_FUNC_NEEDCOLL)!=0&&null==pColl) {
							pColl=this.sqlite3ExprCollSeq(pFarg.a[i].pExpr);
						}
					}
                    if ((pDef.flags & FuncFlags.SQLITE_FUNC_NEEDCOLL) != 0)
                    {
						if(null==pColl)
							pColl=db.pDfltColl;
                        v.sqlite3VdbeAddOp4(OpCode.OP_CollSeq, 0, 0, 0, pColl, P4Usage.P4_COLLSEQ);
					}
					v.sqlite3VdbeAddOp4( OpCode.OP_Function,constMask,r1,target,pDef, P4Usage.P4_FUNCDEF);
					v.sqlite3VdbeChangeP5((u8)nFarg);
					if(nFarg!=0) {
						this.sqlite3ReleaseTempRange(r1,nFarg);
					}
					break;
				}
				#if !SQLITE_OMIT_SUBQUERY
				case TokenType.TK_EXISTS:
				case TokenType.TK_SELECT: {
					sqliteinth.testcase(op==TokenType.TK_EXISTS);
					sqliteinth.testcase(op==TokenType.TK_SELECT);
					inReg=this.sqlite3CodeSubselect(pExpr,0,false);
					break;
				}
				case TokenType.TK_IN: {
					int destIfFalse=v.sqlite3VdbeMakeLabel();
					int destIfNull=v.sqlite3VdbeMakeLabel();
                    v.sqlite3VdbeAddOp2(OpCode.OP_Null, 0, target);
					this.sqlite3ExprCodeIN(pExpr,destIfFalse,destIfNull);
					v.sqlite3VdbeAddOp2(OpCode.OP_Integer,1,target);
					v.sqlite3VdbeResolveLabel(destIfFalse);
					v.sqlite3VdbeAddOp2(OpCode.OP_AddImm,target,0);
					v.sqlite3VdbeResolveLabel(destIfNull);
					break;
				}
				#endif
				///
				///<summary>
				///x BETWEEN y AND z
				///
				///This is equivalent to
				///
				///x>=y AND x<=z
				///
				///X is stored in pExpr.pLeft.
				///Y is stored in pExpr.x.pList.a[0].pExpr.
				///Z is stored in pExpr.x.pList.a[1].pExpr.
				///</summary>
				case TokenType.TK_BETWEEN: {
					Expr pLeft=pExpr.pLeft;
					ExprList_item pLItem=pExpr.x.pList[0];
					Expr pRight=pLItem.pExpr;
					r1=this.sqlite3ExprCodeTemp(pLeft,ref regFree1);
					r2=this.sqlite3ExprCodeTemp(pRight,ref regFree2);
					sqliteinth.testcase(regFree1==0);
					sqliteinth.testcase(regFree2==0);
					r3=this.allocTempReg();
					r4=this.allocTempReg();
                    this.codeCompare(pLeft, pRight,  OpCode.OP_Ge, r1, r2, r3, sqliteinth.SQLITE_STOREP2);
					pLItem=pExpr.x.pList[1];
					// pLItem++;
					pRight=pLItem.pExpr;
					this.deallocTempReg(regFree2);
					r2=this.sqlite3ExprCodeTemp(pRight,ref regFree2);
					sqliteinth.testcase(regFree2==0);
                    this.codeCompare(pLeft, pRight,  OpCode.OP_Le, r1, r2, r4, sqliteinth.SQLITE_STOREP2);
                    v.sqlite3VdbeAddOp3(OpCode.OP_And, r3, r4, target);
					this.deallocTempReg(r3);
					this.deallocTempReg(r4);
					break;
				}
				case TokenType.TK_UPLUS: {
					inReg=this.sqlite3ExprCodeTarget(pExpr.pLeft,target);
					break;
				}
				case TokenType.TK_TRIGGER: {
					///
					///<summary>
					///If the opcode is TokenType.TK_TRIGGER, then the expression is a reference
					///</summary>
					///<param name="to a column in the new.* or old.* pseudo">tables available to</param>
					///<param name="trigger programs. In this case Expr.iTable is set to 1 for the">trigger programs. In this case Expr.iTable is set to 1 for the</param>
					///<param name="new.* pseudo">table. Expr.iColumn</param>
					///<param name="is set to the column of the pseudo">1 to</param>
					///<param name="read the rowid field.">read the rowid field.</param>
					///<param name=""></param>
					///<param name="The expression is implemented using an  OpCode.OP_Param opcode. The p1">The expression is implemented using an  OpCode.OP_Param opcode. The p1</param>
					///<param name="parameter is set to 0 for an old.rowid reference, or to (i+1)">parameter is set to 0 for an old.rowid reference, or to (i+1)</param>
					///<param name="to reference another column of the old.* pseudo">table, where </param>
					///<param name="i is the index of the column. For a new.rowid reference, p1 is">i is the index of the column. For a new.rowid reference, p1 is</param>
					///<param name="set to (n+1), where n is the number of columns in each pseudo">table.</param>
					///<param name="For a reference to any other column in the new.* pseudo">table, p1</param>
					///<param name="is set to (n+2+i), where n and i are as defined previously. For">is set to (n+2+i), where n and i are as defined previously. For</param>
					///<param name="example, if the table on which triggers are being fired is">example, if the table on which triggers are being fired is</param>
					///<param name="declared as:">declared as:</param>
					///<param name=""></param>
					///<param name="CREATE TABLE t1(a, b);">CREATE TABLE t1(a, b);</param>
					///<param name=""></param>
					///<param name="Then p1 is interpreted as follows:">Then p1 is interpreted as follows:</param>
					///<param name=""></param>
					///<param name="p1==0   .    old.rowid     p1==3   .    new.rowid">p1==0   .    old.rowid     p1==3   .    new.rowid</param>
					///<param name="p1==1   .    old.a         p1==4   .    new.a">p1==1   .    old.a         p1==4   .    new.a</param>
					///<param name="p1==2   .    old.b         p1==5   .    new.b       ">p1==2   .    old.b         p1==5   .    new.b       </param>
					///<param name=""></param>
					Table pTab=pExpr.pTab;
					int p1=pExpr.iTable*(pTab.nCol+1)+1+pExpr.iColumn;
					Debug.Assert(pExpr.iTable==0||pExpr.iTable==1);
					Debug.Assert(pExpr.iColumn>=-1&&pExpr.iColumn<pTab.nCol);
					Debug.Assert(pTab.iPKey<0||pExpr.iColumn!=pTab.iPKey);
					Debug.Assert(p1>=0&&p1<(pTab.nCol*2+2));
					v.sqlite3VdbeAddOp2( OpCode.OP_Param,p1,target);
					v.VdbeComment("%s.%s -> $%d",(pExpr.iTable!=0?"new":"old"),(pExpr.iColumn<0?"rowid":pExpr.pTab.aCol[pExpr.iColumn].zName),target);
					///
					///<summary>
					///If the column has REAL affinity, it may currently be stored as an
					///integer. Use OpCode.OP_RealAffinity to make sure it is really real.  
					///</summary>
                    if (pExpr.iColumn >= 0 && pTab.aCol[pExpr.iColumn].affinity == sqliteinth.SQLITE_AFF_REAL)
                    {
						v.sqlite3VdbeAddOp1(OpCode.OP_RealAffinity,target);
					}
					break;
				}
				///
				///<summary>
				///Form A:
				///CASE x WHEN e1 THEN r1 WHEN e2 THEN r2 ... WHEN eN THEN rN ELSE y END
				///
				///Form B:
				///CASE WHEN e1 THEN r1 WHEN e2 THEN r2 ... WHEN eN THEN rN ELSE y END
				///
				///Form A is can be transformed into the equivalent form B as follows:
				///CASE WHEN x=e1 THEN r1 WHEN x=e2 THEN r2 ...
				///WHEN x=eN THEN rN ELSE y END
				///
				///X (if it exists) is in pExpr.pLeft.
				///Y is in pExpr.pRight.  The Y is also optional.  If there is no
				///ELSE clause and no other term matches, then the result of the
				///exprssion is NULL.
				///Ei is in pExpr.x.pList.a[i*2] and Ri is pExpr.x.pList.a[i*2+1].
				///
				///The result of the expression is the Ri for the first matching Ei,
				///or if there is no matching Ei, the ELSE term Y, or if there is
				///no ELSE term, NULL.
				///
				///</summary>
				default: {
					Debug.Assert(op==TokenType.TK_CASE);
					int endLabel;
					///GOTO label for end of CASE stmt 
					int nextCase;
					///GOTO label for next WHEN clause 
					int nExpr;
					///2x number of WHEN terms 
					int i;
					///Loop counter 
					ExprList pEList;
					///List of WHEN terms 
					IList<ExprList_item> aListelem;
					///Array of WHEN terms 
					Expr opCompare=new Expr();
					///The X==Ei expression 
					Expr cacheX;
					///Cached expression X 
					Expr pX;
					///The X expression 
					Expr pTest=null;
					///X==Ei (form A) or just Ei (form B) 
					#if !NDEBUG
																																																																																																																																			            int iCacheLevel = pParse.iCacheLevel;
            //VVA_ONLY( int iCacheLevel = pParse.iCacheLevel; )
#endif
					Debug.Assert(!pExpr.HasProperty(ExprFlags.EP_xIsSelect)&&pExpr.x.pList!=null);
					Debug.Assert((pExpr.x.pList.Count%2)==0);
					Debug.Assert(pExpr.x.pList.Count>0);
					pEList=pExpr.x.pList;
					aListelem=pEList.a;
					nExpr=pEList.Count;
					endLabel=v.sqlite3VdbeMakeLabel();
					if((pX=pExpr.pLeft)!=null) {
						cacheX=pX;
						sqliteinth.testcase(pX.Operator==TokenType.TK_COLUMN);
						sqliteinth.testcase(pX.Operator==TokenType.TK_REGISTER);
						cacheX.iTable=this.sqlite3ExprCodeTemp(pX,ref regFree1);
						sqliteinth.testcase(regFree1==0);
						cacheX.Operator=TokenType.TK_REGISTER;
						opCompare.Operator=TokenType.TK_EQ;
						opCompare.pLeft=cacheX;
						pTest=opCompare;
						///Ticket b351d95f9cd5ef17e9d9dbae18f5ca8611190001:
						///<param name="The value in regFree1 might get SCopy">ed into the file result.</param>
						///<param name="So make sure that the regFree1 register is not reused for other">So make sure that the regFree1 register is not reused for other</param>
						///<param name="purposes and possibly overwritten.  ">purposes and possibly overwritten.  </param>
						regFree1=0;
					}
					for(i=0;i<nExpr;i=i+2) {
						this.sqlite3ExprCachePush();
						if(pX!=null) {
							Debug.Assert(pTest!=null);
							opCompare.pRight=aListelem[i].pExpr;
						}
						else {
							pTest=aListelem[i].pExpr;
						}
						nextCase=v.sqlite3VdbeMakeLabel();
						sqliteinth.testcase(pTest.Operator == TokenType.TK_COLUMN);
						this.sqlite3ExprIfFalse(pTest,nextCase,sqliteinth.SQLITE_JUMPIFNULL);
						sqliteinth.testcase(aListelem[i+1].pExpr.Operator == TokenType.TK_COLUMN);
						sqliteinth.testcase(aListelem[i+1].pExpr.Operator == TokenType.TK_REGISTER);
						this.sqlite3ExprCode(aListelem[i+1].pExpr,target);
						v.sqlite3VdbeAddOp2(OpCode.OP_Goto,0,endLabel);
						this.sqlite3ExprCachePop(1);
						v.sqlite3VdbeResolveLabel(nextCase);
					}
					if(pExpr.pRight!=null) {
						this.sqlite3ExprCachePush();
						this.sqlite3ExprCode(pExpr.pRight,target);
						this.sqlite3ExprCachePop(1);
					}
					else {
                        v.sqlite3VdbeAddOp2(OpCode.OP_Null, 0, target);
					}
					#if !NDEBUG
																																																																																																																																			            Debug.Assert( /* db.mallocFailed != 0 || */ pParse.nErr > 0
            || pParse.iCacheLevel == iCacheLevel );
#endif
					v.sqlite3VdbeResolveLabel(endLabel);
					break;
				}
				#if !SQLITE_OMIT_TRIGGER
				case TokenType.TK_RAISE: {
                    Debug.Assert(((OnConstraintError)pExpr.affinity)
                                        .In(OnConstraintError.OE_Rollback,
                                            OnConstraintError.OE_Abort,
                                            OnConstraintError.OE_Fail,
                                            OnConstraintError.OE_Ignore));

					if(null==this.pTriggerTab) {
						utilc.sqlite3ErrorMsg(this,"RAISE() may only be used within a trigger-program");
						return 0;
					}
                    if ((OnConstraintError)pExpr.affinity == OnConstraintError.OE_Abort)
                    {
						build.sqlite3MayAbort(this);
					}
					Debug.Assert(!pExpr.HasProperty(ExprFlags.EP_IntValue));
                    if ((OnConstraintError)pExpr.affinity == OnConstraintError.OE_Ignore)
                    {
						v.sqlite3VdbeAddOp4(OpCode.OP_Halt,(int)SqlResult.SQLITE_OK,(int)OnConstraintError.OE_Ignore,0,pExpr.u.zToken,0);
					}
					else {
						build.sqlite3HaltConstraint( this,(OnConstraintError)pExpr.affinity,pExpr.u.zToken,(P4Usage)0);
					}
					break;
				}
				#endif
				}
				this.deallocTempReg(regFree1);
				this.deallocTempReg(regFree2);
				return inReg;
			}
			
			public int sqlite3ExprCode(Expr pExpr,int target) {
				int inReg;
				Debug.Assert(target>0&&target<=this.UsedCellCount);
				if(pExpr!=null&&pExpr.Operator == TokenType.TK_REGISTER) {
                    this.pVdbe.sqlite3VdbeAddOp2(OpCode.OP_Copy, pExpr.iTable, target);
				}
				else {
					inReg=this.sqlite3ExprCodeTarget(pExpr,target);
					Debug.Assert(this.pVdbe!=null///
					///<summary>
					///|| pParse.db.mallocFailed != 0 
					///</summary>
					);
					if(inReg!=target&&this.pVdbe!=null) {
                        this.pVdbe.sqlite3VdbeAddOp2(OpCode.OP_SCopy, inReg, target);
					}
				}
				return target;
			}
			public int sqlite3ExprCodeAndCache(Expr pExpr,int target) {
				Vdbe v=this.pVdbe;
				int inReg;
				inReg=this.sqlite3ExprCode(pExpr,target);
				Debug.Assert(target>0);
				///
				///<summary>
				///This routine is called for terms to INSERT or UPDATE.  And the only
				///other place where expressions can be converted into TokenType.TK_REGISTER is
				///in WHERE clause processing.  So as currently implemented, there is
				///no way for a TokenType.TK_REGISTER to exist here.  But it seems prudent to
				///keep the Sqlite3.ALWAYS() in case the conditions above change with future
				///modifications or enhancements. 
				///</summary>
				if(Sqlite3.ALWAYS(pExpr.Operator != TokenType.TK_REGISTER)) {
					int iMem;
					iMem=++this.UsedCellCount;
                    v.sqlite3VdbeAddOp2(OpCode.OP_Copy, inReg, iMem);
					pExpr.iTable=iMem;
					pExpr.op2=pExpr.op;
					pExpr.Operator = TokenType.TK_REGISTER;
				}
				return inReg;
			}
			public void sqlite3ExprCodeConstants(Expr pExpr) {
				Walker w;
				if(this.cookieGoto!=0)
					return;
                if ((this.db.flags & SqliteFlags.SQLITE_FactorOutConst) != 0)
					return;
				w=new Walker();
				w.xExprCallback=(dxExprCallback)exprc.evalConstExpr;
				w.xSelectCallback=null;
				w.pParse=this;
				w.sqlite3WalkExpr(ref pExpr);
			}
			
			
			
			
			
			
			public void sqlite3ExprListSetSpan(
			    ExprList pList,///
			    ///List to which to add the span. 
			    ExprSpan pSpan///
			    ///The span to be added 
			) {
				Connection db=this.db;
				Debug.Assert(pList!=null///
				///<summary>
				///|| db.mallocFailed != 0 
				///</summary>
				);
				if(pList!=null) {
					ExprList_item pItem=pList.a[pList.Count-1];
					Debug.Assert(pList.Count>0);
					Debug.Assert(///
					///<summary>
					///db.mallocFailed != 0 || 
					///</summary>
					pItem.pExpr==pSpan.pExpr);
					db.DbFree(ref pItem.zSpan);
					pItem.zSpan=pSpan.zStart.Substring(0,pSpan.zStart.Length<=pSpan.zEnd.Length?pSpan.zStart.Length:pSpan.zStart.Length-pSpan.zEnd.Length);
					// sqlite3DbStrNDup( db, pSpan.zStart,
					//(int)( pSpan.zEnd- pSpan.zStart) );
				}
			}
			public void sqlite3ExprListSetName(///
			///<summary>
			///Parsing context 
			///</summary>
			ExprList pList,///
			///<summary>
			///List to which to add the span. 
			///</summary>
			Token pName,///
			///<summary>
			///Name to be added 
			///</summary>
			int dequote///
			///<summary>
			///True to cause the name to be dequoted 
			///</summary>
			) {
				Debug.Assert(pList!=null///
				///<summary>
				///|| pParse.db.mallocFailed != 0 
				///</summary>
				);
				if(pList!=null) {
					ExprList_item pItem;
					Debug.Assert(pList.Count>0);
					pItem=pList.a[pList.Count-1];
					Debug.Assert(pItem.zName==null);
					pItem.zName=pName.zRestSql.Substring(0,pName.Length);
					//sqlite3DbStrNDup(pParse.db, pName.z, pName.n);
					if(dequote!=0&&!String.IsNullOrEmpty(pItem.zName))
						StringExtensions.sqlite3Dequote(ref pItem.zName);
				}
			}
			public void sqlite3ExprListCheckLength(ExprList pEList,string zObject) {
				int mx=this.db.aLimit[Globals.SQLITE_LIMIT_COLUMN];
				sqliteinth.testcase(pEList!=null&&pEList.Count==mx);
				sqliteinth.testcase(pEList!=null&&pEList.Count==mx+1);
				if(pEList!=null&&pEList.Count>mx) {
					utilc.sqlite3ErrorMsg(this,"too many columns in %s",zObject);
				}
			}
			public Expr sqlite3ExprFunction(int null_2,Token pToken) {
				return this.sqlite3ExprFunction(null,pToken);
			}
			public Expr sqlite3ExprFunction(ExprList pList,int null_3) {
				return this.sqlite3ExprFunction(pList,null);
			}
			public Expr sqlite3ExprFunction(ExprList pList,Token pToken) {
				Connection db=this.db;
				Debug.Assert(pToken!=null);
				var pNew=exprc.CreateExpr(db,TokenType.TK_FUNCTION,pToken,true);
				if(pNew==null) {
					exprc.Delete(db,ref pList);
					///Avoid memory leak when malloc fails 
					return null;
				}
				pNew.x.pList=pList;
				Debug.Assert(!pNew.HasProperty(ExprFlags.EP_xIsSelect));
				this.sqlite3ExprSetHeight(pNew);
				return pNew;
			}
            /// <summary>
            /// expr ::= VARIABLE 
            /// </summary>
            /// <param name="pExpr"></param>
			public void sqlite3ExprAssignVarNumber(Expr pExpr) {
				Connection db=this.db;
				if(pExpr==null)
					return;
				Debug.Assert(!pExpr.ExprHasAnyProperty(ExprFlags.EP_IntValue|ExprFlags.EP_Reduced|ExprFlags.EP_TokenOnly));
				var z=pExpr.u.zToken;
				Debug.Assert(z!=null);
				Debug.Assert(z.Length!=0);
				if(z.Length==1) {
					///Wildcard of the form "?".  Assign the next variable number 
					Debug.Assert(z[0]=='?');
					pExpr.iColumn=(ynVar)(++this.nVar);
				}
				else {
					ynVar x=0;
					int n=StringExtensions.Strlen30(z);
					if(z[0]=='?') {
						///Wildcard of the form "?nnn".  Convert "nnn" to an integer and
						///use it as the variable number 
						i64 i=0;
						bool bOk=0==Converter.sqlite3Atoi64(z.Substring(1),ref i,n-1,SqliteEncoding.UTF8);
						pExpr.iColumn=x=(ynVar)i;
						sqliteinth.testcase(i==0);
						sqliteinth.testcase(i==1);
						sqliteinth.testcase(i==db.aLimit[Globals.SQLITE_LIMIT_VARIABLE_NUMBER]-1);
						sqliteinth.testcase(i==db.aLimit[Globals.SQLITE_LIMIT_VARIABLE_NUMBER]);
						if(bOk==false||i<1||i>db.aLimit[Globals.SQLITE_LIMIT_VARIABLE_NUMBER]) {
							utilc.sqlite3ErrorMsg(this,"variable number must be between ?1 and ?%d",db.aLimit[Globals.SQLITE_LIMIT_VARIABLE_NUMBER]);
							x=0;
						}
						if(i>this.nVar) {
							this.nVar=(int)i;
						}
					}
					else {
						///Wildcards like ":aaa", "$aaa" or "@aaa".  Reuse the same variable
						///number as the prior appearance of the same name, or if the name
						///has never appeared before, reuse the same variable number						
						for(var i=0;i<this.nzVar;i++) {
							if(this.azVar[i]!=null&&z.CompareTo(this.azVar[i])==0)//memcmp(pParse.azVar[i],z,n+1)==0 )
							 {
								pExpr.iColumn=x=(ynVar)(i+1);
								break;
							}
						}
						if(x==0)
							x=pExpr.iColumn=(ynVar)(++this.nVar);
					}
					if(x>0) {
                        azVar.Count = x;						
						if(z[0]!='?'||this.azVar[x-1]==null) {
							//sqlite3DbFree(db, pParse.azVar[x-1]);
							this.azVar[x-1]=z.Substring(0,n);
							//sqlite3DbStrNDup( db, z, n );
						}
					}
				}
				if(this.nErr==0&&this.nVar>db.aLimit[Globals.SQLITE_LIMIT_VARIABLE_NUMBER]) {
					utilc.sqlite3ErrorMsg(this,"too many SQL variables");
				}
			}
			public void exprCommute(Expr pExpr) {
				ExprFlags expRight=(pExpr.pRight.Flags&ExprFlags.EP_ExpCollate);
				ExprFlags expLeft=(pExpr.pLeft.Flags&ExprFlags.EP_ExpCollate);
				Debug.Assert(wherec.allowedOp(pExpr.Operator)&&pExpr.Operator!=TokenType.TK_IN);
				pExpr.pRight.CollatingSequence=this.sqlite3ExprCollSeq(pExpr.pRight);
				pExpr.pLeft.CollatingSequence=this.sqlite3ExprCollSeq(pExpr.pLeft);
				_Custom.SWAP(ref pExpr.pRight.CollatingSequence,ref pExpr.pLeft.CollatingSequence);
				pExpr.pRight.Flags=((pExpr.pRight.Flags&~ExprFlags.EP_ExpCollate)|expLeft);
				pExpr.pLeft.Flags=((pExpr.pLeft.Flags&~ExprFlags.EP_ExpCollate)|expRight);
                _Custom.SWAP(ref pExpr.pRight, ref pExpr.pLeft);
				if(pExpr.Operator>=TokenType.TK_GT) {
					Debug.Assert(TokenType.TK_LT==TokenType.TK_GT+2);
					Debug.Assert(TokenType.TK_GE==TokenType.TK_LE+2);
					Debug.Assert(TokenType.TK_GT>TokenType.TK_EQ);
					Debug.Assert(TokenType.TK_GT<TokenType.TK_LE);
					Debug.Assert(pExpr.Operator>=TokenType.TK_GT&&pExpr.Operator<=TokenType.TK_GE);
					pExpr.op=(u8)(((pExpr.op-(int)TokenType.TK_GT)^2)+(int)TokenType.TK_GT);
				}
			}
			public int isLikeOrGlob(///
			///<summary>
			///Parsing and code generating context 
			///</summary>
			Expr pExpr,///
			///<summary>
			///Test this expression 
			///</summary>
			ref Expr ppPrefix,///
			///<summary>
			///Pointer to TokenType.TK_STRING expression with pattern prefix 
			///</summary>
			ref bool pisComplete,///
			///<summary>
			///True if the only wildcard is % in the last character 
			///</summary>
			ref bool pnoCase///
			///<summary>
			///True if uppercase is equivalent to lowercase 
			///</summary>
			) {
				string z=null;
				///
				///<summary>
				///String on RHS of LIKE operator 
				///</summary>
				Expr pRight,pLeft;
				///
				///<summary>
				///Right and left size of LIKE operator 
				///</summary>
				ExprList pList;
				///
				///<summary>
				///List of operands to the LIKE operator 
				///</summary>
				int c=0;
				///
				///<summary>
				///One character in z[] 
				///</summary>
				int cnt;
				///
				///<summary>
				///</summary>
				///<param name="Number of non">wildcard prefix characters </param>
				char[] wc=new char[3];
				///
				///<summary>
				///Wildcard characters 
				///</summary>
				Connection db=this.db;
				///
				///<summary>
				///Data_base connection 
				///</summary>
				sqlite3_value pVal=null;
				
				
				if(!PredefinedFunctions.sqlite3IsLikeFunction(db,pExpr,ref pnoCase,wc)) {
					return 0;
				}
				//#if SQLITE_EBCDIC
				//if( pnoCase ) return 0;
				//#endif
				pList=pExpr.x.pList;
				pLeft=pList.a[1].pExpr;
				if(pLeft.Operator!=TokenType.TK_COLUMN||pLeft.sqlite3ExprAffinity()!=sqliteinth.SQLITE_AFF_TEXT) {
					///
					///<summary>
					///</summary>
					///<param name="IMP: R">hand side of the LIKE or GLOB operator must</param>
					///<param name="be the name of an indexed column with TEXT affinity. ">be the name of an indexed column with TEXT affinity. </param>
					return 0;
				}
				Debug.Assert(pLeft.iColumn!=(-1));
				///
				///<summary>
				///Because IPK never has AFF_TEXT 
				///</summary>
				pRight=pList.a[0].pExpr;

                ///
				///<summary>
				///Opcode of pRight 
				///</summary>
				var op =pRight.Operator;
				if(op==TokenType.TK_REGISTER) {
					op=pRight.Operator2;
				}
				if(op==TokenType.TK_VARIABLE) {
					Vdbe pReprepare=this.pReprepare;
					int iCol=pRight.iColumn;
                    pVal = pReprepare.sqlite3VdbeGetValue(iCol, (byte)sqliteinth.SQLITE_AFF_NONE);
                    if (pVal != null && vdbeapi.sqlite3_value_type(pVal) == FoundationalType.SQLITE_TEXT)
                    {
						z=vdbeapi.sqlite3_value_text(pVal);
					}
					this.pVdbe.sqlite3VdbeSetVarmask(iCol);
					///
					///<summary>
					///</summary>
					///<param name="IMP: R">02778 </param>
					Debug.Assert(pRight.Operator==TokenType.TK_VARIABLE||pRight.Operator==TokenType.TK_REGISTER);
				}
				else
					if(op==TokenType.TK_STRING) {
						z=pRight.u.zToken;
					}
				if(!String.IsNullOrEmpty(z)) {
					cnt=0;
					while(cnt<z.Length&&(c=z[cnt])!=0&&c!=wc[0]&&c!=wc[1]&&c!=wc[2]) {
						cnt++;
					}
					if(cnt!=0&&255!=(u8)z[cnt-1]) {
						Expr pPrefix;
						pisComplete=c==wc[0]&&cnt==z.Length-1;
						pPrefix=exprc.sqlite3Expr(db,TokenType.TK_STRING,z);
						if(pPrefix!=null)
							pPrefix.u.zToken=pPrefix.u.zToken.Substring(0,cnt);
						ppPrefix=pPrefix;
						if(op==TokenType.TK_VARIABLE) {
							Vdbe v=this.pVdbe;
							v.sqlite3VdbeSetVarmask(pRight.iColumn);
							///
							///<summary>
							///</summary>
							///<param name="IMP: R">02778 </param>
							if(pisComplete&&pRight.u.zToken.Length>1) {
								///
								///<summary>
								///If the rhs of the LIKE expression is a variable, and the current
								///value of the variable means there is no need to invoke the LIKE
								///function, then no  OpCode.OP_Variable will be added to the program.
								///This causes problems for the sqlite3_bind_parameter_name()
								///API. To workaround them, add a dummy  OpCode.OP_Variable here.
								///
								///</summary>
								int r1=this.allocTempReg();
								this.sqlite3ExprCodeTarget(pRight,r1);
								v.sqlite3VdbeChangeP3(v.sqlite3VdbeCurrentAddr()-1,0);
								this.deallocTempReg(r1);
							}
						}
					}
					else {
						z=null;
					}
				}
                vdbemem_cs.sqlite3ValueFree(ref pVal);
				return (z!=null)?1:0;
			}
			public void bestOrClauseIndex(///
			///<summary>
			///The parsing context 
			///</summary>
			WhereClause pWC,///
			///<summary>
			///The WHERE clause 
			///</summary>
			SrcList_item pSrc,///
			///<summary>
			///The FROM clause term to search 
			///</summary>
			Bitmask notReady,///
			///<summary>
			///Mask of cursors not available for indexing 
			///</summary>
			Bitmask notValid,///
			///<summary>
			///Cursors not available for any purpose 
			///</summary>
			ExprList pOrderBy,///
			///<summary>
			///The ORDER BY clause 
			///</summary>
			WhereCost pCost///
			///<summary>
			///Lowest cost query plan 
			///</summary>
			) {
				#if !SQLITE_OMIT_OR_OPTIMIZATION
				int iCur=pSrc.iCursor;
				///
				///<summary>
				///The cursor of the table to be accessed 
				///</summary>
				Bitmask maskSrc=pWC.pMaskSet.getMask(iCur);
				///
				///<summary>
				///Bitmask for pSrc 
				///</summary>
				WhereTerm pWCEnd=pWC.a[pWC.nTerm];
				///
				///<summary>
				///End of pWC.a[] 
				///</summary>
				WhereTerm pTerm;
				///
				///<summary>
				///A single term of the WHERE clause 
				///</summary>
				///
				///<summary>
				///</summary>
				///<param name="No OR">clause optimization allowed if the INDEXED BY or NOT INDEXED clauses</param>
				///<param name="are used ">are used </param>
				if(pSrc.notIndexed!=0||pSrc.pIndex!=null) {
					return;
				}
				///
				///<summary>
				///Search the WHERE clause terms for a usable wherec.WO_OR term. 
				///</summary>
				for(int _pt=0;_pt<pWC.nTerm;_pt++)//<pWCEnd; pTerm++)
				 {
					pTerm=pWC.a[_pt];
					if(pTerm.eOperator==wherec.WO_OR&&((pTerm.prereqAll&~maskSrc)&notReady)==0&&(pTerm.u.pOrInfo.indexable&maskSrc)!=0) {
						WhereClause pOrWC=pTerm.u.pOrInfo.wc;
						WhereTerm pOrWCEnd=pOrWC.a[pOrWC.nTerm];
						WhereTerm pOrTerm;
						int flags=wherec.WHERE_MULTI_OR;
						double rTotal=0;
						double nRow=0;
						Bitmask used=0;
						for(int _pOrWC=0;_pOrWC<pOrWC.nTerm;_pOrWC++)//pOrTerm = pOrWC.a ; pOrTerm < pOrWCEnd ; pOrTerm++ )
						 {
							pOrTerm=pOrWC.a[_pOrWC];
							WhereCost sTermCost=null;
							#if (SQLITE_TEST) && (SQLITE_DEBUG)
																																																																																																																																																																																														            WHERETRACE( "... Multi-index OR testing for term %d of %d....\n",
            _pOrWC, pOrWC.nTerm - _pOrWC//( pOrTerm - pOrWC.a ), ( pTerm - pWC.a )
            );
#endif
							if(pOrTerm.eOperator==wherec.WO_AND) {
								WhereClause pAndWC=pOrTerm.u.pAndInfo.wc;
								this.bestIndex(pAndWC,pSrc,notReady,notValid,null,ref sTermCost);
							}
							else
								if(pOrTerm.leftCursor==iCur) {
									WhereClause tempWC=new WhereClause();
									tempWC.pParse=pWC.pParse;
									tempWC.pMaskSet=pWC.pMaskSet;
									tempWC.Operator=TokenType.TK_AND;
									tempWC.a=new WhereTerm[2];
									tempWC.a[0]=pOrTerm;
									tempWC.nTerm=1;
									this.bestIndex(tempWC,pSrc,notReady,notValid,null,ref sTermCost);
								}
								else {
									continue;
								}
							rTotal+=sTermCost.rCost;
							nRow+=sTermCost.plan.nRow;
							used|=sTermCost.used;
							if(rTotal>=pCost.rCost)
								break;
						}
						///
						///<summary>
						///If there is an ORDER BY clause, increase the scan cost to account
						///for the cost of the sort. 
						///</summary>
						if(pOrderBy!=null) {
							#if (SQLITE_TEST) && (SQLITE_DEBUG)
																																																																																																																																																																																														            WHERETRACE( "... sorting increases OR cost %.9g to %.9g\n",
            rTotal, rTotal + nRow * estLog( nRow ) );
#endif
							rTotal+=nRow*MathExtensions.estLog(nRow);
						}
						///
						///<summary>
						///If the cost of scanning using this OR term for optimization is
						///less than the current cost stored in pCost, replace the contents
						///of pCost. 
						///</summary>
						#if (SQLITE_TEST) && (SQLITE_DEBUG)
																																																																																																																																																																          WHERETRACE( "... multi-index OR cost=%.9g nrow=%.9g\n", rTotal, nRow );
#endif
						if(rTotal<pCost.rCost) {
							pCost.rCost=rTotal;
							pCost.used=used;
							pCost.plan.nRow=nRow;
							pCost.plan.wsFlags=(uint)flags;
							pCost.plan.u.pTerm=pTerm;
						}
					}
				}
				#endif
			}
			public void bestAutomaticIndex(///
			///<summary>
			///The parsing context 
			///</summary>
			WhereClause pWC,///
			///<summary>
			///The WHERE clause 
			///</summary>
			SrcList_item pSrc,///
			///<summary>
			///The FROM clause term to search 
			///</summary>
			Bitmask notReady,///
			///<summary>
			///Mask of cursors that are not available 
			///</summary>
			WhereCost pCost///
			///<summary>
			///Lowest cost query plan 
			///</summary>
			) {
				double nTableRow;
				///
				///<summary>
				///Rows in the input table 
				///</summary>
				double logN;
				///
				///<summary>
				///log(nTableRow) 
				///</summary>
				double costTempIdx;
				///
				///<summary>
				///</summary>
				///<param name="per">query cost of the transient index </param>
				WhereTerm pTerm;
				///
				///<summary>
				///A single term of the WHERE clause 
				///</summary>
				WhereTerm pWCEnd;
				///
				///<summary>
				///End of pWC.a[] 
				///</summary>
				Table pTable;
				///
				///<summary>
				///Table that might be indexed 
				///</summary>
				if(this.nQueryLoop<=(double)1) {
					///
					///<summary>
					///There is no point in building an automatic index for a single scan 
					///</summary>
					return;
				}
                if ((this.db.flags & SqliteFlags.SQLITE_AutoIndex) == 0)
                {
					///
					///<summary>
					///</summary>
					///<param name="Automatic indices are disabled at run">time </param>
					return;
				}
				if((pCost.plan.wsFlags&wherec.WHERE_NOT_FULLSCAN)!=0) {
					///
					///<summary>
					///We already have some kind of index in use for this query. 
					///</summary>
					return;
				}
				if(pSrc.notIndexed!=0) {
					///
					///<summary>
					///The NOT INDEXED clause appears in the SQL. 
					///</summary>
					return;
				}
				Debug.Assert(this.nQueryLoop>=(double)1);
				pTable=pSrc.pTab;
				nTableRow=pTable.nRowEst;
				logN=MathExtensions.estLog(nTableRow);
				costTempIdx=2*logN*(nTableRow/this.nQueryLoop+1);
				if(costTempIdx>=pCost.rCost) {
					///
					///<summary>
					///The cost of creating the transient table would be greater than
					///doing the full table scan 
					///</summary>
					return;
				}
				///
				///<summary>
				///Search for any equality comparison term 
				///</summary>
				//pWCEnd = pWC.a[pWC.nTerm];
				for(int ipTerm=0;ipTerm<pWC.nTerm;ipTerm++)//; pTerm<pWCEnd; pTerm++)
				 {
					pTerm=pWC.a[ipTerm];
					if(pTerm.termCanDriveIndex(pSrc,notReady)!=0) {
						#if (SQLITE_TEST) && (SQLITE_DEBUG)
																																																																																																																																																																          WHERETRACE( "auto-index reduces cost from %.2f to %.2f\n",
          pCost.rCost, costTempIdx );
#endif
						pCost.rCost=costTempIdx;
						pCost.plan.nRow=logN+1;
						pCost.plan.wsFlags=wherec.WHERE_TEMP_INDEX;
						pCost.used=pTerm.prereqRight;
						break;
					}
				}
			}
			public void constructAutomaticIndex(///
			///<summary>
			///The parsing context 
			///</summary>
			WhereClause pWC,///
			///<summary>
			///The WHERE clause 
			///</summary>
			SrcList_item pSrc,///
			///<summary>
			///The FROM clause term to get the next index 
			///</summary>
			Bitmask notReady,///
			///<summary>
			///Mask of cursors that are not available 
			///</summary>
			WhereLevel pLevel///
			///<summary>
			///Write new index here 
			///</summary>
			) {
				int nColumn;
				///
				///<summary>
				///Number of columns in the constructed index 
				///</summary>
				WhereTerm pTerm;
				///
				///<summary>
				///A single term of the WHERE clause 
				///</summary>
				WhereTerm pWCEnd;
				///
				///<summary>
				///End of pWC.a[] 
				///</summary>
				int nByte;
				///
				///<summary>
				///Byte of memory needed for pIdx 
				///</summary>
				Index pIdx;
				///
				///<summary>
				///Object describing the transient index 
				///</summary>
				Vdbe v;
				///
				///<summary>
				///Prepared statement under construction 
				///</summary>
				int regIsInit;
				///
				///<summary>
				///Register set by initialization 
				///</summary>
				int addrInit;
				///
				///<summary>
				///Address of the initialization bypass jump 
				///</summary>
				Table pTable;
				///
				///<summary>
				///The table being indexed 
				///</summary>
				KeyInfo pKeyinfo;
				///
				///<summary>
				///Key information for the index 
				///</summary>
				int addrTop;
				///
				///<summary>
				///Top of the index fill loop 
				///</summary>
				int regRecord;
				///
				///<summary>
				///Register holding an index record 
				///</summary>
				int n;
				///
				///<summary>
				///Column counter 
				///</summary>
				int i;
				///
				///<summary>
				///Loop counter 
				///</summary>
				int mxBitCol;
				///
				///<summary>
				///Maximum column in pSrc.colUsed 
				///</summary>
				CollSeq pColl;
				///
				///<summary>
				///Collating sequence to on a column 
				///</summary>
				Bitmask idxCols;
				///
				///<summary>
				///Bitmap of columns used for indexing 
				///</summary>
				Bitmask extraCols;
				///
				///<summary>
				///Bitmap of additional columns 
				///</summary>
				///
				///<summary>
				///Generate code to skip over the creation and initialization of the
				///transient index on 2nd and subsequent iterations of the loop. 
				///</summary>
				v=this.pVdbe;
				Debug.Assert(v!=null);
				regIsInit=++this.UsedCellCount;
				addrInit=v.sqlite3VdbeAddOp1(OpCode.OP_If,regIsInit);
				v.sqlite3VdbeAddOp2(OpCode.OP_Integer,1,regIsInit);
				///
				///<summary>
				///Count the number of columns that will be added to the index
				///and used to match WHERE clause constraints 
				///</summary>
				nColumn=0;
				pTable=pSrc.pTab;
				//pWCEnd = pWC.a[pWC.nTerm];
				idxCols=0;
				for(int ipTerm=0;ipTerm<pWC.nTerm;ipTerm++)//; pTerm<pWCEnd; pTerm++)
				 {
					pTerm=pWC.a[ipTerm];
					if(pTerm.termCanDriveIndex(pSrc,notReady)!=0) {
						int iCol=pTerm.u.leftColumn;
                        Bitmask cMask = iCol >= Globals.BMS ? ((Bitmask)1) << (Globals.BMS - 1) : ((Bitmask)1) << iCol;
                        sqliteinth.testcase(iCol == Globals.BMS);
                        sqliteinth.testcase(iCol == Globals.BMS - 1);
						if((idxCols&cMask)==0) {
							nColumn++;
							idxCols|=cMask;
						}
					}
				}
				Debug.Assert(nColumn>0);
				pLevel.plan.nEq=(u32)nColumn;
				///
				///<summary>
				///Count the number of additional columns needed to create a
				///covering index.  A "covering index" is an index that contains all
				///columns that are needed by the query.  With a covering index, the
				///original table never needs to be accessed.  Automatic indices must
				///be a covering index because the index will not be updated if the
				///original table changes and the index and table cannot both be used
				///if they go out of sync.
				///
				///</summary>
                extraCols = pSrc.colUsed & (~idxCols | (((Bitmask)1) << (Globals.BMS - 1)));
                mxBitCol = (pTable.nCol >= Globals.BMS - 1) ? Globals.BMS - 1 : pTable.nCol;
                sqliteinth.testcase(pTable.nCol == Globals.BMS - 1);
                sqliteinth.testcase(pTable.nCol == Globals.BMS - 2);
				for(i=0;i<mxBitCol;i++) {
					if((extraCols&(((Bitmask)1)<<i))!=0)
						nColumn++;
				}
                if ((pSrc.colUsed & (((Bitmask)1) << (Globals.BMS - 1))) != 0)
                {
                    nColumn += pTable.nCol - Globals.BMS + 1;
				}
				pLevel.plan.wsFlags|=wherec.WHERE_COLUMN_EQ|wherec.WHERE_IDX_ONLY|wherec.WO_EQ;
				///
				///<summary>
				///Construct the Index object to describe this index 
				///</summary>
				//nByte = sizeof(Index);
				//nByte += nColumn*sizeof(int);     /* Index.aiColumn */
				//nByte += nColumn*sizeof(char);   /* Index.azColl */
				//nByte += nColumn;                 /* Index.aSortOrder */
				//pIdx = sqlite3DbMallocZero(pParse.db, nByte);
				//if( pIdx==null) return;
				pIdx=new Index();
				pLevel.plan.u.pIdx=pIdx;
				pIdx.azColl=new string[nColumn+1];
				// pIdx[1];
				pIdx.aiColumn=new int[nColumn+1];
				// pIdx.azColl[nColumn];
				pIdx.aSortOrder=new SortOrder[nColumn+1];
				// pIdx.aiColumn[nColumn];
				pIdx.zName="auto-index";
				pIdx.nColumn=nColumn;
				pIdx.pTable=pTable;
				n=0;
				idxCols=0;
				//for(pTerm=pWC.a; pTerm<pWCEnd; pTerm++){
				for(int ipTerm=0;ipTerm<pWC.nTerm;ipTerm++) {
					pTerm=pWC.a[ipTerm];
					if(pTerm.termCanDriveIndex(pSrc,notReady)!=0) {
						int iCol=pTerm.u.leftColumn;
                        Bitmask cMask = iCol >= Globals.BMS ? ((Bitmask)1) << (Globals.BMS - 1) : ((Bitmask)1) << iCol;
						if((idxCols&cMask)==0) {
							Expr pX=pTerm.pExpr;
							idxCols|=cMask;
							pIdx.aiColumn[n]=pTerm.u.leftColumn;
							pColl=this.sqlite3BinaryCompareCollSeq(pX.pLeft,pX.pRight);
							pIdx.azColl[n]=Sqlite3.ALWAYS(pColl!=null)?pColl.zName:"BINARY";
							n++;
						}
					}
				}
				Debug.Assert((u32)n==pLevel.plan.nEq);
				///
				///<summary>
				///Add additional columns needed to make the automatic index into
				///a covering index 
				///</summary>
				for(i=0;i<mxBitCol;i++) {
					if((extraCols&(((Bitmask)1)<<i))!=0) {
						pIdx.aiColumn[n]=i;
						pIdx.azColl[n]="BINARY";
						n++;
					}
				}
                if ((pSrc.colUsed & (((Bitmask)1) << (Globals.BMS - 1))) != 0)
                {
                    for (i = Globals.BMS - 1; i < pTable.nCol; i++)
                    {
						pIdx.aiColumn[n]=i;
						pIdx.azColl[n]="BINARY";
						n++;
					}
				}
				Debug.Assert(n==nColumn);
				///
				///<summary>
				///Create the automatic index 
				///</summary>
                pKeyinfo = pIdx.sqlite3IndexKeyinfo(this);
				Debug.Assert(pLevel.iIdxCur>=0);
				v.sqlite3VdbeAddOp4( OpCode.OP_OpenAutoindex,pLevel.iIdxCur,nColumn+1,0,pKeyinfo, P4Usage.P4_KEYINFO_HANDOFF);
				v.VdbeComment("for %s",pTable.zName);
				///
				///<summary>
				///Fill the automatic index with content 
				///</summary>
				addrTop=v.sqlite3VdbeAddOp1(OpCode.OP_Rewind,pLevel.iTabCur);
				regRecord=this.allocTempReg();
				this.codegenGenerateIndexKey(pIdx,pLevel.iTabCur,regRecord,true);
				v.sqlite3VdbeAddOp2( OpCode.OP_IdxInsert,pLevel.iIdxCur,regRecord);
				v.sqlite3VdbeChangeP5(OpFlag.OPFLAG_USESEEKRESULT);
                v.sqlite3VdbeAddOp2(OpCode.OP_Next, pLevel.iTabCur, addrTop + 1);
				v.sqlite3VdbeChangeP5(SQLITE_STMTSTATUS_AUTOINDEX);
				v.sqlite3VdbeJumpHere(addrTop);
				this.deallocTempReg(regRecord);
				///
				///<summary>
				///Jump here when skipping the initialization 
				///</summary>
				v.sqlite3VdbeJumpHere(addrInit);
			}
			public sqlite3_index_info allocateIndexInfo(WhereClause pWC,SrcList_item pSrc,ExprList pOrderBy) {
				int i,j;
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
				///
				///<summary>
				///Count the number of possible WHERE clause constraints referring
				///to this virtual table 
				///</summary>
				for(i=nTerm=0;i<pWC.nTerm;i++)//, pTerm++ )
				 {
					pTerm=pWC.a[i];
					if(pTerm.leftCursor!=pSrc.iCursor)
						continue;
					Debug.Assert((pTerm.eOperator&(pTerm.eOperator-1))==0);
					sqliteinth.testcase(pTerm.eOperator==wherec.WO_IN);
					sqliteinth.testcase(pTerm.eOperator==wherec.WO_ISNULL);
					if((pTerm.eOperator&(wherec.WO_IN|wherec.WO_ISNULL))!=0)
						continue;
					nTerm++;
				}
				///
				///<summary>
				///If the ORDER BY clause contains only columns in the current
				///virtual table then allocate space for the aOrderBy part of
				///the sqlite3_index_info structure.
				///
				///</summary>
				nOrderBy=0;
				if(pOrderBy!=null) {
					for(i=0;i<pOrderBy.Count;i++) {
						Expr pExpr=pOrderBy.a[i].pExpr;
						if(pExpr.Operator!=TokenType.TK_COLUMN||pExpr.iTable!=pSrc.iCursor)
							break;
					}
					if(i==pOrderBy.Count) {
						nOrderBy=pOrderBy.Count;
					}
				}
				///
				///<summary>
				///Allocate the sqlite3_index_info structure
				///
				///</summary>
				pIdxInfo=new sqlite3_index_info();
				//sqlite3DbMallocZero(pParse.db, sizeof(*pIdxInfo)
				//+ (sizeof(*pIdxCons) + sizeof(*pUsage))*nTerm
				//+ sizeof(*pIdxOrderBy)*nOrderBy );
				//if ( pIdxInfo == null )
				//{
				//  utilc.sqlite3ErrorMsg( pParse, "out of memory" );
				//  /* (double)0 In case of SQLITE_OMIT_FLOATING_POINT... */
				//  return null;
				//}
				///
				///<summary>
				///Initialize the structure.  The sqlite3_index_info structure contains
				///many fields that are declared "const" to prevent xBestIndex from
				///changing them.  We have to do some funky casting in order to
				///initialize those fields.
				///
				///</summary>
				pIdxCons=new sqlite3_index_constraint[nTerm];
				//(sqlite3_index_constraint)pIdxInfo[1];
				pIdxOrderBy=new sqlite3_index_orderby[nOrderBy];
				//(sqlite3_index_orderby)pIdxCons[nTerm];
				pUsage=new sqlite3_index_constraint_usage[nTerm];
				//(sqlite3_index_constraint_usage)pIdxOrderBy[nOrderBy];
				pIdxInfo.nConstraint=nTerm;
				pIdxInfo.nOrderBy=nOrderBy;
				pIdxInfo.aConstraint=pIdxCons;
				pIdxInfo.aOrderBy=pIdxOrderBy;
				pIdxInfo.aConstraintUsage=pUsage;
				for(i=j=0;i<pWC.nTerm;i++)//, pTerm++ )
				 {
					pTerm=pWC.a[i];
					if(pTerm.leftCursor!=pSrc.iCursor)
						continue;
					Debug.Assert((pTerm.eOperator&(pTerm.eOperator-1))==0);
					sqliteinth.testcase(pTerm.eOperator==wherec.WO_IN);
					sqliteinth.testcase(pTerm.eOperator==wherec.WO_ISNULL);
					if((pTerm.eOperator&(wherec.WO_IN|wherec.WO_ISNULL))!=0)
						continue;
					if(pIdxCons[j]==null)
						pIdxCons[j]=new sqlite3_index_constraint();
					pIdxCons[j].iColumn=pTerm.u.leftColumn;
					pIdxCons[j].iTermOffset=i;
					pIdxCons[j].op=(u8)pTerm.eOperator;
					///
					///<summary>
					///The direct Debug.Assignment in the previous line is possible only because
					///the wherec.WO_ and SQLITE_INDEX_CONSTRAINT_ codes are identical.  The
					///following Debug.Asserts verify this fact. 
					///</summary>
					Debug.Assert(wherec.WO_EQ==SQLITE_INDEX_CONSTRAINT_EQ);
					Debug.Assert(wherec.WO_LT==SQLITE_INDEX_CONSTRAINT_LT);
					Debug.Assert(wherec.WO_LE==SQLITE_INDEX_CONSTRAINT_LE);
					Debug.Assert(wherec.WO_GT==SQLITE_INDEX_CONSTRAINT_GT);
					Debug.Assert(wherec.WO_GE==SQLITE_INDEX_CONSTRAINT_GE);
					Debug.Assert(wherec.WO_MATCH==SQLITE_INDEX_CONSTRAINT_MATCH);
					Debug.Assert((pTerm.eOperator&(wherec.WO_EQ|wherec.WO_LT|wherec.WO_LE|wherec.WO_GT|wherec.WO_GE|wherec.WO_MATCH))!=0);
					j++;
				}
				for(i=0;i<nOrderBy;i++) {
					Expr pExpr=pOrderBy.a[i].pExpr;
					if(pIdxOrderBy[i]==null)
						pIdxOrderBy[i]=new sqlite3_index_orderby();
					pIdxOrderBy[i].iColumn=pExpr.iColumn;
					pIdxOrderBy[i].desc=pOrderBy.a[i].sortOrder!=0;
				}
				return pIdxInfo;
			}
			public int vtabBestIndex(Table pTab,sqlite3_index_info p) {
				sqlite3_vtab pVtab=VTableMethodsExtensions.sqlite3GetVTable(this.db,pTab).pVtab;
				int i;
				int rc;
				#if (SQLITE_TEST) && (SQLITE_DEBUG)
																																																																																																				      WHERETRACE( "xBestIndex for %s\n", pTab.zName );
#endif
				wherec.TRACE_IDX_INPUTS(p);
				rc=pVtab.pModule.xBestIndex(pVtab,ref p);
				wherec.TRACE_IDX_OUTPUTS(p);
				if(rc!=(int )SqlResult.SQLITE_OK) {
					//if ( rc == SQLITE_NOMEM )
					//{
					//  pParse.db.mallocFailed = 1;
					//}
					// else 
					if(String.IsNullOrEmpty(pVtab.zErrMsg)) {
						utilc.sqlite3ErrorMsg(this,"%s",sqlite3ErrStr(rc));
					}
					else {
						utilc.sqlite3ErrorMsg(this,"%s",pVtab.zErrMsg);
					}
				}
				//malloc_cs.sqlite3_free( pVtab.zErrMsg );
				pVtab.zErrMsg=null;
				for(i=0;i<p.nConstraint;i++) {
					if(!p.aConstraint[i].usable&&p.aConstraintUsage[i].argvIndex>0) {
						utilc.sqlite3ErrorMsg(this,"table %s: xBestIndex returned an invalid plan",pTab.zName);
					}
				}
				return this.nErr;
			}

            private string sqlite3ErrStr(int rc)
            {
                return rc.sqlite3ErrStr();
            }

            public SqlResult whereRangeScanEst(///
			///<summary>
			///Parsing & code generating context 
			///</summary>
			Index p,///
			///<summary>
			///</summary>
			///<param name="The index containing the range">compared column; "x" </param>
			int nEq,///
			///<summary>
			///</summary>
			///<param name="index into p.aCol[] of the range">compared column </param>
			WhereTerm pLower,///
			///<summary>
			///Lower bound on the range. ex: "x>123" Might be NULL 
			///</summary>
			WhereTerm pUpper,///
			///<summary>
			///Upper bound on the range. ex: "x<455" Might be NULL 
			///</summary>
			out int piEst///
			///<summary>
			///OUT: Return value 
			///</summary>
			) {
				var rc=SqlResult.SQLITE_OK;
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
          Debug.Assert( pLower.eOperator == wherec.WO_GT || pLower.eOperator == wherec.WO_GE );
          roundUpLower = ( pLower.eOperator == wherec.WO_GT ) ? 1 : 0;
        }
        if ( rc == SqlResult.SQLITE_OK && pUpper != null )
        {
          Expr pExpr = pUpper.pExpr.pRight;
          rc = valueFromExpr( pParse, pExpr, aff, ref pUpperVal );
          Debug.Assert( pUpper.eOperator == wherec.WO_LT || pUpper.eOperator == wherec.WO_LE );
          roundUpUpper = ( pUpper.eOperator == wherec.WO_LE ) ? 1 : 0;
        }

        if ( rc != SqlResult.SQLITE_OK || ( pLowerVal == null && pUpperVal == null ) )
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
          if ( rc == SqlResult.SQLITE_OK )
          {
            rc = whereRangeRegion( pParse, p, pLowerVal, roundUpLower, out iLower );
          }
        }
        WHERETRACE( "range scan regions: %d..%d\n", iLower, iUpper );

        iEst = iUpper - iLower;
        sqliteinth.testcase( iEst == SQLITE_INDEX_SAMPLES );
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
				sqliteinth.UNUSED_PARAMETER(this);
				sqliteinth.UNUSED_PARAMETER(p);
				sqliteinth.UNUSED_PARAMETER(nEq);
				#endif
				Debug.Assert(pLower!=null||pUpper!=null);
				piEst=100;
				if(pLower!=null&&(pLower.wtFlags&WhereTermFlags.TERM_VNULL)==0)
					piEst/=4;
				if(pUpper!=null)
					piEst/=4;
				return rc;
			}
			public void bestVirtualIndex(///
			///<summary>
			///The parsing context 
			///</summary>
			WhereClause pWC,///
			///<summary>
			///The WHERE clause 
			///</summary>
			SrcList_item pSrc,///
			///<summary>
			///The FROM clause term to search 
			///</summary>
			Bitmask notReady,///
			///<summary>
			///Mask of cursors not available for index 
			///</summary>
			Bitmask notValid,///
			///<summary>
			///Cursors not valid for any purpose 
			///</summary>
			ExprList pOrderBy,///
			///<summary>
			///The order by clause 
			///</summary>
			ref WhereCost pCost,///
			///<summary>
			///Lowest cost query plan 
			///</summary>
			ref sqlite3_index_info ppIdxInfo///
			///<summary>
			///Index information passed to xBestIndex 
			///</summary>
			) {
				Table pTab=pSrc.pTab;
				sqlite3_index_info pIdxInfo;
				sqlite3_index_constraint pIdxCons;
				sqlite3_index_constraint_usage[] pUsage=null;
				WhereTerm pTerm;
				int i,j;
				int nOrderBy;
				double rCost;
				///
				///<summary>
				///Make sure wsFlags is initialized to some sane value. Otherwise, if the
				///malloc in allocateIndexInfo() fails and this function returns leaving
				///wsFlags in an uninitialized state, the caller may behave unpredictably.
				///
				///</summary>
				pCost=new WhereCost();
				//memset(pCost, 0, sizeof(*pCost));
				pCost.plan.wsFlags=wherec.WHERE_VIRTUALTABLE;
				///
				///<summary>
				///If the sqlite3_index_info structure has not been previously
				///allocated and initialized, then allocate and initialize it now.
				///
				///</summary>
				pIdxInfo=ppIdxInfo;
				if(pIdxInfo==null) {
					ppIdxInfo=pIdxInfo=this.allocateIndexInfo(pWC,pSrc,pOrderBy);
				}
				if(pIdxInfo==null) {
					return;
				}
				///
				///<summary>
				///At this point, the sqlite3_index_info structure that pIdxInfo points
				///to will have been initialized, either during the current invocation or
				///during some prior invocation.  Now we just have to customize the
				///details of pIdxInfo for the current invocation and pDebug.Ass it to
				///xBestIndex.
				///
				///</summary>
				///
				///<summary>
				///The module name must be defined. Also, by this point there must
				///be a pointer to an sqlite3_vtab structure. Otherwise
				///build.sqlite3ViewGetColumnNames() would have picked up the error.
				///
				///</summary>
				Debug.Assert(pTab.azModuleArg!=null&&pTab.azModuleArg[0]!=null);
                Debug.Assert(VTableMethodsExtensions.sqlite3GetVTable(this.db, pTab) != null);
				///
				///<summary>
				///Set the aConstraint[].usable fields and initialize all
				///output variables to zero.
				///
				///</summary>
				///<param name="aConstraint[].usable is true for constraints where the right">hand</param>
				///<param name="side contains only references to tables to the left of the current">side contains only references to tables to the left of the current</param>
				///<param name="table.  In other words, if the constraint is of the form:">table.  In other words, if the constraint is of the form:</param>
				///<param name=""></param>
				///<param name="column = expr">column = expr</param>
				///<param name=""></param>
				///<param name="and we are evaluating a join, then the constraint on column is">and we are evaluating a join, then the constraint on column is</param>
				///<param name="only valid if all tables referenced in expr occur to the left">only valid if all tables referenced in expr occur to the left</param>
				///<param name="of the table containing column.">of the table containing column.</param>
				///<param name=""></param>
				///<param name="The aConstraints[] array contains entries for all constraints">The aConstraints[] array contains entries for all constraints</param>
				///<param name="on the current table.  That way we only have to compute it once">on the current table.  That way we only have to compute it once</param>
				///<param name="even though we might try to pick the best index multiple times.">even though we might try to pick the best index multiple times.</param>
				///<param name="For each attempt at picking an index, the order of tables in the">For each attempt at picking an index, the order of tables in the</param>
				///<param name="join might be different so we have to recompute the usable flag">join might be different so we have to recompute the usable flag</param>
				///<param name="each time.">each time.</param>
				///<param name=""></param>
				//pIdxCons = *(struct sqlite3_index_constraint**)&pIdxInfo->aConstraint;
				//pUsage = pIdxInfo->aConstraintUsage;
				for(i=0;i<pIdxInfo.nConstraint;i++) {
					pIdxCons=pIdxInfo.aConstraint[i];
					pUsage=pIdxInfo.aConstraintUsage;
					j=pIdxCons.iTermOffset;
					pTerm=pWC.a[j];
					pIdxCons.usable=(pTerm.prereqRight&notReady)==0;
					pUsage[i]=new sqlite3_index_constraint_usage();
				}
				// memset(pUsage, 0, sizeof(pUsage[0])*pIdxInfo.nConstraint);
				if(pIdxInfo.needToFreeIdxStr!=0) {
					//malloc_cs.sqlite3_free(ref pIdxInfo.idxStr);
				}
				pIdxInfo.idxStr=null;
				pIdxInfo.idxNum=0;
				pIdxInfo.needToFreeIdxStr=0;
				pIdxInfo.orderByConsumed=false;
				///
				///<summary>
				///((double)2) In case of SQLITE_OMIT_FLOATING_POINT... 
				///</summary>
                pIdxInfo.estimatedCost = sqliteinth.SQLITE_BIG_DBL / ((double)2);
				nOrderBy=pIdxInfo.nOrderBy;
				if(null==pOrderBy) {
					pIdxInfo.nOrderBy=0;
				}
				if(this.vtabBestIndex(pTab,pIdxInfo)!=0) {
					return;
				}
				//pIdxCons = (sqlite3_index_constraint)pIdxInfo.aConstraint;
				for(i=0;i<pIdxInfo.nConstraint;i++) {
					if(pUsage[i].argvIndex>0) {
						//pCost.used |= pWC.a[pIdxCons[i].iTermOffset].prereqRight;
						pCost.used|=pWC.a[pIdxInfo.aConstraint[i].iTermOffset].prereqRight;
					}
				}
				///
				///<summary>
				///If there is an ORDER BY clause, and the selected virtual table index
				///does not satisfy it, increase the cost of the scan accordingly. This
				///</summary>
				///<param name="matches the processing for non">virtual tables in bestBtreeIndex().</param>
				///<param name=""></param>
				rCost=pIdxInfo.estimatedCost;
				if(pOrderBy!=null&&!pIdxInfo.orderByConsumed) {
					rCost+=MathExtensions.estLog(rCost)*rCost;
				}
				///
				///<summary>
				///The cost is not allowed to be larger than SQLITE_BIG_DBL (the
				///inital value of lowestCost in this loop. If it is, then the
				///(cost<lowestCost) test below will never be true.
				///
				///Use "(double)2" instead of "2.0" in case OMIT_FLOATING_POINT
				///is defined.
				///
				///</summary>
                if ((sqliteinth.SQLITE_BIG_DBL / ((double)2)) < rCost)
                {
                    pCost.rCost = (sqliteinth.SQLITE_BIG_DBL / ((double)2));
				}
				else {
					pCost.rCost=rCost;
				}
				pCost.plan.u.pVtabIdx=pIdxInfo;
				if(pIdxInfo.orderByConsumed) {
					pCost.plan.wsFlags|=wherec.WHERE_ORDERBY;
				}
				pCost.plan.nEq=0;
				pIdxInfo.nOrderBy=nOrderBy;
				///
				///<summary>
				///Try to find a more efficient access pattern by using multiple indexes
				///to optimize an OR expression within the WHERE clause.
				///
				///</summary>
				this.bestOrClauseIndex(pWC,pSrc,notReady,notValid,pOrderBy,pCost);
			}
			public void bestBtreeIndex(///
			///<summary>
			///The parsing context 
			///</summary>
			WhereClause pWC,///
			///<summary>
			///The WHERE clause 
			///</summary>
			SrcList_item pSrc,///
			///<summary>
			///The FROM clause term to search 
			///</summary>
			Bitmask notReady,///
			///<summary>
			///Mask of cursors not available for indexing 
			///</summary>
			Bitmask notValid,///
			///<summary>
			///Cursors not available for any purpose 
			///</summary>
			ExprList pOrderBy,///
			///<summary>
			///The ORDER BY clause 
			///</summary>
			ref WhereCost pCost///
			///<summary>
			///Lowest cost query plan 
			///</summary>
			) {
				int iCur=pSrc.iCursor;
				///
				///<summary>
				///The cursor of the table to be accessed 
				///</summary>
				Index pProbe;
				///
				///<summary>
				///An index we are evaluating 
				///</summary>
				Index pIdx;
				///
				///<summary>
				///Copy of pProbe, or zero for IPK index 
				///</summary>
				u32 eqTermMask;
				///
				///<summary>
				///Current mask of valid equality operators 
				///</summary>
				u32 idxEqTermMask;
				///
				///<summary>
				///Index mask of valid equality operators 
				///</summary>
				Index sPk;
				///
				///<summary>
				///A fake index object for the primary key 
				///</summary>
				int[] aiRowEstPk=new int[2];
				///
				///<summary>
				///The aiRowEst[] value for the sPk index 
				///</summary>
				int aiColumnPk=-1;
				///
				///<summary>
				///The aColumn[] value for the sPk index 
				///</summary>
				int wsFlagMask;
				///
				///<summary>
				///Allowed flags in pCost.plan.wsFlag 
				///</summary>
				///
				///<summary>
				///</summary>
				///<param name="Initialize the cost to a worst">case value </param>
				if(pCost==null)
					pCost=new WhereCost();
				else
					pCost.Clear();
				//memset(pCost, 0, sizeof(*pCost));
                pCost.rCost = sqliteinth.SQLITE_BIG_DBL;
				///
				///<summary>
				///If the pSrc table is the right table of a LEFT JOIN then we may not
				///use an index to satisfy IS NULL constraints on that table.  This is
				///</summary>
				///<param name="because columns might end up being NULL if the table does not match "></param>
				///<param name="a circumstance which the index cannot help us discover.  Ticket #2177.">a circumstance which the index cannot help us discover.  Ticket #2177.</param>
				///<param name=""></param>
                if ((pSrc.jointype & JoinType.JT_LEFT) != 0)
                {
					idxEqTermMask=wherec.WO_EQ|wherec.WO_IN;
				}
				else {
					idxEqTermMask=wherec.WO_EQ|wherec.WO_IN|wherec.WO_ISNULL;
				}
				if(pSrc.pIndex!=null) {
					///
					///<summary>
					///An INDEXED BY clause specifies a particular index to use 
					///</summary>
					pIdx=pProbe=pSrc.pIndex;
					wsFlagMask=~(wherec.WHERE_ROWID_EQ|wherec.WHERE_ROWID_RANGE);
					eqTermMask=idxEqTermMask;
				}
				else {
					///
					///<summary>
					///There is no INDEXED BY clause.  Create a fake Index object in local
					///variable sPk to represent the rowid primary key index.  Make this
					///fake index the first in a chain of Index objects with all of the real
					///indices to follow 
					///</summary>
					Index pFirst;
					///
					///<summary>
					///First of real indices on the table 
					///</summary>
					sPk=new Index();
					// memset( &sPk, 0, sizeof( Index ) );
                    sPk.aSortOrder = new SortOrder[1];
					sPk.azColl=new string[1];
					sPk.azColl[0]="";
					sPk.nColumn=1;
					sPk.aiColumn=new int[1];
					sPk.aiColumn[0]=aiColumnPk;
					sPk.aiRowEst=aiRowEstPk;
					sPk.onError=OnConstraintError.OE_Replace;
					sPk.pTable=pSrc.pTab;
					aiRowEstPk[0]=(int)pSrc.pTab.nRowEst;
					aiRowEstPk[1]=1;
					pFirst=pSrc.pTab.pIndex;
					if(pSrc.notIndexed==0) {
						///
						///<summary>
						///The real indices of the table are only considered if the
						///NOT INDEXED qualifier is omitted from the FROM clause 
						///</summary>
						sPk.pNext=pFirst;
					}
					pProbe=sPk;
					wsFlagMask=~(wherec.WHERE_COLUMN_IN|wherec.WHERE_COLUMN_EQ|wherec.WHERE_COLUMN_NULL|wherec.WHERE_COLUMN_RANGE);
					eqTermMask=wherec.WO_EQ|wherec.WO_IN;
					pIdx=null;
				}
				///
				///<summary>
				///Loop over all indices looking for the best one to use
				///
				///</summary>
				for(;pProbe!=null;pIdx=pProbe=pProbe.pNext) {
					int[] aiRowEst=pProbe.aiRowEst;
					double cost;
					///
					///<summary>
					///Cost of using pProbe 
					///</summary>
					double nRow;
					///
					///<summary>
					///Estimated number of rows in result set 
					///</summary>
					double log10N=0;
					///
					///<summary>
					///</summary>
					///<param name="base">10 logarithm of nRow (inexact) </param>
					int rev=0;
					///
					///<summary>
					///True to scan in reverse order 
					///</summary>
					int wsFlags=0;
					Bitmask used=0;
					///
					///<summary>
					///The following variables are populated based on the properties of
					///index being evaluated. They are then used to determine the expected
					///cost and number of rows returned.
					///
					///nEq: 
					///Number of equality terms that can be implemented using the index.
					///In other words, the number of initial fields in the index that
					///are used in == or IN or NOT NULL constraints of the WHERE clause.
					///
					///nInMul:  
					///</summary>
					///<param name="The "in">multiplier". This is an estimate of how many seek operations </param>
					///<param name="SQLite must perform on the index in question. For example, if the ">SQLite must perform on the index in question. For example, if the </param>
					///<param name="WHERE clause is:">WHERE clause is:</param>
					///<param name=""></param>
					///<param name="WHERE a IN (1, 2, 3) AND b IN (4, 5, 6)">WHERE a IN (1, 2, 3) AND b IN (4, 5, 6)</param>
					///<param name=""></param>
					///<param name="SQLite must perform 9 lookups on an index on (a, b), so nInMul is ">SQLite must perform 9 lookups on an index on (a, b), so nInMul is </param>
					///<param name="set to 9. Given the same schema and either of the following WHERE ">set to 9. Given the same schema and either of the following WHERE </param>
					///<param name="clauses:">clauses:</param>
					///<param name=""></param>
					///<param name="WHERE a =  1">WHERE a =  1</param>
					///<param name="WHERE a >= 2">WHERE a >= 2</param>
					///<param name=""></param>
					///<param name="nInMul is set to 1.">nInMul is set to 1.</param>
					///<param name=""></param>
					///<param name="If there exists a WHERE term of the form "x IN (SELECT ...)", then ">If there exists a WHERE term of the form "x IN (SELECT ...)", then </param>
					///<param name="the sub">select is assumed to return 25 rows for the purposes of </param>
					///<param name="determining nInMul.">determining nInMul.</param>
					///<param name=""></param>
					///<param name="bInEst:  ">bInEst:  </param>
					///<param name="Set to true if there was at least one "x IN (SELECT ...)" term used ">Set to true if there was at least one "x IN (SELECT ...)" term used </param>
					///<param name="in determining the value of nInMul.  Note that the RHS of the">in determining the value of nInMul.  Note that the RHS of the</param>
					///<param name="IN operator must be a SELECT, not a value list, for this variable">IN operator must be a SELECT, not a value list, for this variable</param>
					///<param name="to be true.">to be true.</param>
					///<param name=""></param>
					///<param name="estBound:">estBound:</param>
					///<param name="An estimate on the amount of the table that must be searched.  A">An estimate on the amount of the table that must be searched.  A</param>
					///<param name="value of 100 means the entire table is searched.  Range constraints">value of 100 means the entire table is searched.  Range constraints</param>
					///<param name="might reduce this to a value less than 100 to indicate that only">might reduce this to a value less than 100 to indicate that only</param>
					///<param name="a fraction of the table needs searching.  In the absence of">a fraction of the table needs searching.  In the absence of</param>
					///<param name="sqlite_stat2 ANALYZE data, a single inequality reduces the search">sqlite_stat2 ANALYZE data, a single inequality reduces the search</param>
					///<param name="space to 1/4rd its original size.  So an x>? constraint reduces">space to 1/4rd its original size.  So an x>? constraint reduces</param>
					///<param name="estBound to 25.  Two constraints (x>? AND x<?) reduce estBound to 6.">estBound to 25.  Two constraints (x>? AND x<?) reduce estBound to 6.</param>
					///<param name=""></param>
					///<param name="bSort:   ">bSort:   </param>
					///<param name="Boolean. True if there is an ORDER BY clause that will require an ">Boolean. True if there is an ORDER BY clause that will require an </param>
					///<param name="external sort (i.e. scanning the index being evaluated will not ">external sort (i.e. scanning the index being evaluated will not </param>
					///<param name="correctly order records).">correctly order records).</param>
					///<param name=""></param>
					///<param name="bLookup: ">bLookup: </param>
					///<param name="Boolean. True if a table lookup is required for each index entry">Boolean. True if a table lookup is required for each index entry</param>
					///<param name="visited.  In other words, true if this is not a covering index.">visited.  In other words, true if this is not a covering index.</param>
					///<param name="This is always false for the rowid primary key index of a table.">This is always false for the rowid primary key index of a table.</param>
					///<param name="For other indexes, it is true unless all the columns of the table">For other indexes, it is true unless all the columns of the table</param>
					///<param name="used by the SELECT statement are present in the index (such an">used by the SELECT statement are present in the index (such an</param>
					///<param name="index is sometimes described as a covering index).">index is sometimes described as a covering index).</param>
					///<param name="For example, given the index on (a, b), the second of the following ">For example, given the index on (a, b), the second of the following </param>
					///<param name="two queries requires table b">tree lookups in order to find the value</param>
					///<param name="of column c, but the first does not because columns a and b are">of column c, but the first does not because columns a and b are</param>
					///<param name="both available in the index.">both available in the index.</param>
					///<param name=""></param>
					///<param name="SELECT a, b    FROM tbl WHERE a = 1;">SELECT a, b    FROM tbl WHERE a = 1;</param>
					///<param name="SELECT a, b, c FROM tbl WHERE a = 1;">SELECT a, b, c FROM tbl WHERE a = 1;</param>
					///<param name=""></param>
					int nEq;
					///
					///<summary>
					///Number of == or IN terms matching index 
					///</summary>
					int bInEst=0;
					///
					///<summary>
					///True if "x IN (SELECT...)" seen 
					///</summary>
					int nInMul=1;
					///
					///<summary>
					///Number of distinct equalities to lookup 
					///</summary>
					int estBound=100;
					///
					///<summary>
					///Estimated reduction in search space 
					///</summary>
					int nBound=0;
					///
					///<summary>
					///Number of range constraints seen 
					///</summary>
					int bSort=0;
					///
					///<summary>
					///True if external sort required 
					///</summary>
					int bLookup=0;
					///
					///<summary>
					///True if not a covering index 
					///</summary>
					WhereTerm pTerm;
					///
					///<summary>
					///A single term of the WHERE clause 
					///</summary>
					#if SQLITE_ENABLE_STAT2
																																																																																																																																		        WhereTerm pFirstTerm = null;  /* First term matching the index */
#endif
					///
					///<summary>
					///Determine the values of nEq and nInMul 
					///</summary>
					for(nEq=0;nEq<pProbe.nColumn;nEq++) {
						int j=pProbe.aiColumn[nEq];
						pTerm=pWC.findTerm(iCur,j,notReady,eqTermMask,pIdx);
						if(pTerm==null)
							break;
						wsFlags|=(wherec.WHERE_COLUMN_EQ|wherec.WHERE_ROWID_EQ);
						if((pTerm.eOperator&wherec.WO_IN)!=0) {
							Expr pExpr=pTerm.pExpr;
							wsFlags|=wherec.WHERE_COLUMN_IN;
							if(pExpr.HasProperty(ExprFlags.EP_xIsSelect)) {
								///
								///<summary>
								///"x IN (SELECT ...)":  Assume the SELECT returns 25 rows 
								///</summary>
								nInMul*=25;
								bInEst=1;
							}
							else
								if(Sqlite3.ALWAYS(pExpr.x.pList!=null)&&pExpr.x.pList.Count!=0) {
									///
									///<summary>
									///"x IN (value, value, ...)" 
									///</summary>
									nInMul*=pExpr.x.pList.Count;
								}
						}
						else
							if((pTerm.eOperator&wherec.WO_ISNULL)!=0) {
								wsFlags|=wherec.WHERE_COLUMN_NULL;
							}
						#if SQLITE_ENABLE_STAT2
																																																																																																																																																																          if ( nEq == 0 && pProbe.aSample != null )
            pFirstTerm = pTerm;
#endif
						used|=pTerm.prereqRight;
					}
					///
					///<summary>
					///Determine the value of estBound. 
					///</summary>
					if(nEq<pProbe.nColumn&&pProbe.bUnordered==0) {
						int j=pProbe.aiColumn[nEq];
						if(pWC.findTerm(iCur,j,notReady,wherec.WO_LT|wherec.WO_LE|wherec.WO_GT|wherec.WO_GE,pIdx)!=null) {
							WhereTerm pTop=pWC.findTerm(iCur,j,notReady,wherec.WO_LT|wherec.WO_LE,pIdx);
							WhereTerm pBtm=pWC.findTerm(iCur,j,notReady,wherec.WO_GT|wherec.WO_GE,pIdx);
							this.whereRangeScanEst(pProbe,nEq,pBtm,pTop,out estBound);
							if(pTop!=null) {
								nBound=1;
								wsFlags|=wherec.WHERE_TOP_LIMIT;
								used|=pTop.prereqRight;
							}
							if(pBtm!=null) {
								nBound++;
								wsFlags|=wherec.WHERE_BTM_LIMIT;
								used|=pBtm.prereqRight;
							}
							wsFlags|=(wherec.WHERE_COLUMN_RANGE|wherec.WHERE_ROWID_RANGE);
						}
					}
					else
						if(pProbe.onError!=OnConstraintError.OE_None) {
							sqliteinth.testcase(wsFlags&wherec.WHERE_COLUMN_IN);
							sqliteinth.testcase(wsFlags&wherec.WHERE_COLUMN_NULL);
							if((wsFlags&(wherec.WHERE_COLUMN_IN|wherec.WHERE_COLUMN_NULL))==0) {
								wsFlags|=wherec.WHERE_UNIQUE;
							}
						}
					///
					///<summary>
					///If there is an ORDER BY clause and the index being considered will
					///naturally scan rows in the required order, set the appropriate flags
					///in wsFlags. Otherwise, if there is an ORDER BY clause but the index
					///will scan rows in a different order, set the bSort variable.  
					///</summary>
					if(pOrderBy!=null) {
						if((wsFlags&wherec.WHERE_COLUMN_IN)==0&&pProbe.bUnordered==0&&this.isSortingIndex(pWC.pMaskSet,pProbe,iCur,pOrderBy,nEq,wsFlags,ref rev)) {
							wsFlags|=wherec.WHERE_ROWID_RANGE|wherec.WHERE_COLUMN_RANGE|wherec.WHERE_ORDERBY;
							wsFlags|=(rev!=0?wherec.WHERE_REVERSE:0);
						}
						else {
							bSort=1;
						}
					}
					///
					///<summary>
					///If currently calculating the cost of using an index (not the IPK
					///index), determine if all required column data may be obtained without 
					///using the main table (i.e. if the index is a covering
					///index for this query). If it is, set the wherec.WHERE_IDX_ONLY flag in
					///wsFlags. Otherwise, set the bLookup variable to true.  
					///</summary>
					if(pIdx!=null&&wsFlags!=0) {
						Bitmask m=pSrc.colUsed;
						int j;
						for(j=0;j<pIdx.nColumn;j++) {
							int x=pIdx.aiColumn[j];
                            if (x < Globals.BMS - 1)
                            {
								m&=~(((Bitmask)1)<<x);
							}
						}
						if(m==0) {
							wsFlags|=wherec.WHERE_IDX_ONLY;
						}
						else {
							bLookup=1;
						}
					}
					///
					///<summary>
					///Estimate the number of rows of output.  For an "x IN (SELECT...)"
					///constraint, do not let the estimate exceed half the rows in the table.
					///
					///</summary>
					nRow=(double)(aiRowEst[nEq]*nInMul);
					if(bInEst!=0&&nRow*2>aiRowEst[0]) {
						nRow=aiRowEst[0]/2;
						nInMul=(int)(nRow/aiRowEst[nEq]);
					}
					#if SQLITE_ENABLE_STAT2
																																																																																																																																		        /* If the constraint is of the form x=VALUE and histogram
    ** data is available for column x, then it might be possible
    ** to get a better estimate on the number of rows based on
    ** VALUE and how common that value is according to the histogram.
    */
        if ( nRow > (double)1 && nEq == 1 && pFirstTerm != null )
        {
          if ( ( pFirstTerm.eOperator & ( wherec.WO_EQ | wherec.WO_ISNULL ) ) != 0 )
          {
            sqliteinth.testcase( pFirstTerm.eOperator == wherec.WO_EQ );
            sqliteinth.testcase( pFirstTerm.eOperator == wherec.WO_ISNULL );
            whereEqualScanEst( pParse, pProbe, pFirstTerm.pExpr.pRight, ref nRow );
          }
          else if ( pFirstTerm.eOperator == wherec.WO_IN && bInEst == 0 )
          {
            whereInScanEst( pParse, pProbe, pFirstTerm.pExpr.x.pList, ref nRow );
          }
        }
#endif
					///
					///<summary>
					///Adjust the number of output rows and downward to reflect rows
					///that are excluded by range constraints.
					///
					///</summary>
					nRow=(nRow*(double)estBound)/(double)100;
					if(nRow<1)
						nRow=1;
					///
					///<summary>
					///Experiments run on real SQLite databases show that the time needed
					///to do a binary search to locate a row in a table or index is roughly
					///log10(N) times the time to move from one row to the next row within
					///a table or index.  The actual times can vary, with the size of
					///records being an important factor.  Both moves and searches are
					///slower with larger records, presumably because fewer records fit
					///on one page and hence more pages have to be fetched.
					///
					///The ANALYZE command and the sqlite_stat1 and sqlite_stat2 tables do
					///not give us data on the relative sizes of table and index records.
					///So this computation assumes table records are about twice as big
					///as index records
					///
					///</summary>
					if((wsFlags&wherec.WHERE_NOT_FULLSCAN)==0) {
						///
						///<summary>
						///The cost of a full table scan is a number of move operations equal
						///to the number of rows in the table.
						///
						///We add an additional 4x penalty to full table scans.  This causes
						///the cost function to err on the side of choosing an index over
						///</summary>
						///<param name="choosing a full scan.  This 4x full">scan penalty is an arguable</param>
						///<param name="decision and one which we expect to revisit in the future.  But">decision and one which we expect to revisit in the future.  But</param>
						///<param name="it seems to be working well enough at the moment.">it seems to be working well enough at the moment.</param>
						///<param name=""></param>
						cost=aiRowEst[0]*4;
					}
					else {
                        log10N = MathExtensions.estLog(aiRowEst[0]);
						cost=nRow;
						if(pIdx!=null) {
							if(bLookup!=0) {
								///
								///<summary>
								///For an index lookup followed by a table lookup:
								///nInMul index searches to find the start of each index range
								///+ nRow steps through the index
								///+ nRow table searches to lookup the table entry using the rowid
								///
								///</summary>
								cost+=(nInMul+nRow)*log10N;
							}
							else {
								///
								///<summary>
								///For a covering index:
								///nInMul index searches to find the initial entry 
								///+ nRow steps through the index
								///
								///</summary>
								cost+=nInMul*log10N;
							}
						}
						else {
							///
							///<summary>
							///For a rowid primary key lookup:
							///nInMult table searches to find the initial entry for each range
							///+ nRow steps through the table
							///
							///</summary>
							cost+=nInMul*log10N;
						}
					}
					///
					///<summary>
					///Add in the estimated cost of sorting the result.  Actual experimental
					///measurements of sorting performance in SQLite show that sorting time
					///adds C*N*log10(N) to the cost, where N is the number of rows to be 
					///sorted and C is a factor between 1.95 and 4.3.  We will split the
					///difference and select C of 3.0.
					///
					///</summary>
					if(bSort!=0) {
                        cost += nRow * MathExtensions.estLog(nRow) * 3;
					}
					///
					///<summary>
					///Cost of using this index has now been computed ***
					///</summary>
					///
					///<summary>
					///If there are additional constraints on this table that cannot
					///be used with the current index, but which might lower the number
					///of output rows, adjust the nRow value accordingly.  This only 
					///matters if the current index is the least costly, so do not bother
					///with this step if we already know this index will not be chosen.
					///Also, never reduce the output row count below 2 using this step.
					///
					///It is critical that the notValid mask be used here instead of
					///the notReady mask.  When computing an "optimal" index, the notReady
					///</summary>
					///<param name="mask will only have one bit set "> the bit for the current table.</param>
					///<param name="The notValid mask, on the other hand, always has all bits set for">The notValid mask, on the other hand, always has all bits set for</param>
					///<param name="tables that are not in outer loops.  If notReady is used here instead">tables that are not in outer loops.  If notReady is used here instead</param>
					///<param name="of notValid, then a optimal index that depends on inner joins loops">of notValid, then a optimal index that depends on inner joins loops</param>
					///<param name="might be selected even when there exists an optimal index that has">might be selected even when there exists an optimal index that has</param>
					///<param name="no such dependency.">no such dependency.</param>
					if(nRow>2&&cost<=pCost.rCost) {
						//int k;                       /* Loop counter */
						int nSkipEq=nEq;
						///
						///<summary>
						///Number of == constraints to skip 
						///</summary>
						int nSkipRange=nBound;
						///
						///<summary>
						///Number of < constraints to skip 
						///</summary>
						Bitmask thisTab;
						///
						///<summary>
						///Bitmap for pSrc 
						///</summary>
						thisTab=pWC.pMaskSet.getMask(iCur);
						for(int ipTerm=0,k=pWC.nTerm;nRow>2&&k!=0;k--,ipTerm++)//pTerm++)
						 {
							pTerm=pWC.a[ipTerm];
							if((pTerm.wtFlags&WhereTermFlags.TERM_VIRTUAL)!=0)
								continue;
							if((pTerm.prereqAll&notValid)!=thisTab)
								continue;
							if((pTerm.eOperator&(wherec.WO_EQ|wherec.WO_IN|wherec.WO_ISNULL))!=0) {
								if(nSkipEq!=0) {
									///
									///<summary>
									///Ignore the first nEq equality matches since the index
									///has already accounted for these 
									///</summary>
									nSkipEq--;
								}
								else {
									///
									///<summary>
									///Assume each additional equality match reduces the result
									///set size by a factor of 10 
									///</summary>
									nRow/=10;
								}
							}
							else
								if((pTerm.eOperator&(wherec.WO_LT|wherec.WO_LE|wherec.WO_GT|wherec.WO_GE))!=0) {
									if(nSkipRange!=0) {
										///
										///<summary>
										///Ignore the first nSkipRange range constraints since the index
										///has already accounted for these 
										///</summary>
										nSkipRange--;
									}
									else {
										///
										///<summary>
										///Assume each additional range constraint reduces the result
										///set size by a factor of 3.  Indexed range constraints reduce
										///the search space by a larger factor: 4.  We make indexed range
										///more selective intentionally because of the subjective 
										///observation that indexed range constraints really are more
										///selective in practice, on average. 
										///</summary>
										nRow/=3;
									}
								}
								else
									if(pTerm.eOperator!=wherec.WO_NOOP) {
										///
										///<summary>
										///Any other expression lowers the output row count by half 
										///</summary>
										nRow/=2;
									}
						}
						if(nRow<2)
							nRow=2;
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
					///
					///<summary>
					///If this index is the best we have seen so far, then record this
					///index and its cost in the pCost structure.
					///</summary>
					if((null==pIdx||wsFlags!=0)&&(cost<pCost.rCost||(cost<=pCost.rCost&&nRow<pCost.plan.nRow))) {
						pCost.rCost=cost;
						pCost.used=used;
						pCost.plan.nRow=nRow;
						pCost.plan.wsFlags=(uint)(wsFlags&wsFlagMask);
						pCost.plan.nEq=(uint)nEq;
						pCost.plan.u.pIdx=pIdx;
					}
					///
					///<summary>
					///If there was an INDEXED BY clause, then only that one index is
					///considered. 
					///</summary>
					if(pSrc.pIndex!=null)
						break;
					///
					///<summary>
					///Reset masks for the next index in the loop 
					///</summary>
					wsFlagMask=~(wherec.WHERE_ROWID_EQ|wherec.WHERE_ROWID_RANGE);
					eqTermMask=idxEqTermMask;
				}
				///
				///<summary>
				///If there is no ORDER BY clause and the SQLITE_ReverseOrder flag
				///is set, then reverse the order that the index will be scanned
				///in. This is used for application testing, to help find cases
				///where application behaviour depends on the (undefined) order that
				///SQLite outputs rows in in the absence of an ORDER BY clause.  
				///</summary>
                if (null == pOrderBy && (this.db.flags & SqliteFlags.SQLITE_ReverseOrder) != 0)
                {
					pCost.plan.wsFlags|=wherec.WHERE_REVERSE;
				}
				Debug.Assert(pOrderBy!=null||(pCost.plan.wsFlags&wherec.WHERE_ORDERBY)==0);
				Debug.Assert(pCost.plan.u.pIdx==null||(pCost.plan.wsFlags&wherec.WHERE_ROWID_EQ)==0);
				Debug.Assert(pSrc.pIndex==null||pCost.plan.u.pIdx==null||pCost.plan.u.pIdx==pSrc.pIndex);
				#if (SQLITE_TEST) && (SQLITE_DEBUG)
																																																																																																				      WHERETRACE( "best index is: %s\n",
      ( ( pCost.plan.wsFlags & wherec.WHERE_NOT_FULLSCAN ) == 0 ? "none" :
      pCost.plan.u.pIdx != null ? pCost.plan.u.pIdx.zName : "ipk" )
      );
#endif
				this.bestOrClauseIndex(pWC,pSrc,notReady,notValid,pOrderBy,pCost);
				this.bestAutomaticIndex(pWC,pSrc,notReady,pCost);
				pCost.plan.wsFlags|=(u32)eqTermMask;
			}
			public void bestIndex(///
			///<summary>
			///The parsing context 
			///</summary>
			WhereClause pWC,///
			///<summary>
			///The WHERE clause 
			///</summary>
			SrcList_item pSrc,///
			///<summary>
			///The FROM clause term to search 
			///</summary>
			Bitmask notReady,///
			///<summary>
			///Mask of cursors not available for indexing 
			///</summary>
			Bitmask notValid,///
			///<summary>
			///Cursors not available for any purpose 
			///</summary>
			ExprList pOrderBy,///
			///<summary>
			///The ORDER BY clause 
			///</summary>
			ref WhereCost pCost///
			///<summary>
			///Lowest cost query plan 
			///</summary>
			) {
				#if !SQLITE_OMIT_VIRTUALTABLE
                if (pSrc.pTab.IsVirtual())
                {
					sqlite3_index_info p=null;
					this.bestVirtualIndex(pWC,pSrc,notReady,notValid,pOrderBy,ref pCost,ref p);
					if(p.needToFreeIdxStr!=0) {
						//malloc_cs.sqlite3_free(ref p.idxStr);
					}
					this.db.DbFree(ref p);
				}
				else
				#endif
				 {
					this.bestBtreeIndex(pWC,pSrc,notReady,notValid,pOrderBy,ref pCost);
				}
			}
			public void codeApplyAffinity(int _base,int n,string zAff) {
				Vdbe v=this.pVdbe;
				//if (zAff == 0)
				//{
				//  Debug.Assert(pParse.db.mallocFailed);
				//  return;
				//}
				Debug.Assert(v!=null);
				///
				///<summary>
				///Adjust base and n to skip over SQLITE_AFF_NONE entries at the beginning
				///and end of the affinity string.
				///
				///</summary>
                while (n > 0 && zAff[0] == sqliteinth.SQLITE_AFF_NONE)
                {
					n--;
					_base++;
					zAff=zAff.Substring(1);
					// zAff++;
				}
                while (n > 1 && zAff[n - 1] == sqliteinth.SQLITE_AFF_NONE)
                {
					n--;
				}
				///
				///<summary>
				///Code the  OpCode.OP_Affinity opcode if there is anything left to do. 
				///</summary>
				if(n>0) {
                    v.sqlite3VdbeAddOp2(OpCode.OP_Affinity, _base, n);
					v.sqlite3VdbeChangeP4(-1,zAff,n);
					this.sqlite3ExprCacheAffinityChange(_base,n);
				}
			}
			public int codeEqualityTerm(///
			///<summary>
			///The parsing context 
			///</summary>
			WhereTerm pTerm,///
			///<summary>
			///The term of the WHERE clause to be coded 
			///</summary>
			WhereLevel pLevel,///
			///<summary>
			///When level of the FROM clause we are working on 
			///</summary>
			int iTarget///
			///<summary>
			///Attempt to leave results in this register 
			///</summary>
			) {
				Expr pX=pTerm.pExpr;
				Vdbe v=this.pVdbe;
				int iReg;
				///
				///<summary>
				///Register holding results 
				///</summary>
				Debug.Assert(iTarget>0);
				if(pX.Operator==TokenType.TK_EQ) {
					iReg=this.sqlite3ExprCodeTarget(pX.pRight,iTarget);
				}
				else
					if(pX.Operator==TokenType.TK_ISNULL) {
						iReg=iTarget;
                        v.sqlite3VdbeAddOp2(OpCode.OP_Null, 0, iReg);
						#if !SQLITE_OMIT_SUBQUERY
					}
					else {
						int eType;
						int iTab;
						InLoop pIn;
						Debug.Assert(pX.Operator==TokenType.TK_IN);
						iReg=iTarget;
						int iDummy=-1;
						eType=this.sqlite3FindInIndex(pX,ref iDummy);
						iTab=pX.iTable;
                        v.sqlite3VdbeAddOp2(OpCode.OP_Rewind, iTab, 0);
						Debug.Assert((pLevel.plan.wsFlags&wherec.WHERE_IN_ABLE)!=0);
						if(pLevel.u._in.nIn==0) {
							pLevel.addrNxt=v.sqlite3VdbeMakeLabel();
						}
						pLevel.u._in.nIn++;
						if(pLevel.u._in.aInLoop==null)
							pLevel.u._in.aInLoop=new InLoop[pLevel.u._in.nIn];
						else
							Array.Resize(ref pLevel.u._in.aInLoop,pLevel.u._in.nIn);
						//sqlite3DbReallocOrFree(pParse.db, pLevel.u._in.aInLoop,
						//                       sizeof(pLevel.u._in.aInLoop[0])*pLevel.u._in.nIn);
						//pIn = pLevel.u._in.aInLoop;
						if(pLevel.u._in.aInLoop!=null)//(pIn )
						 {
							pLevel.u._in.aInLoop[pLevel.u._in.nIn-1]=new InLoop();
							pIn=pLevel.u._in.aInLoop[pLevel.u._in.nIn-1];
							//pIn++
							pIn.iCur=iTab;
							if(eType==sqliteinth.IN_INDEX_ROWID) {
                                pIn.addrInTop = v.sqlite3VdbeAddOp2(OpCode.OP_Rowid, iTab, iReg);
							}
							else {
								pIn.addrInTop=v.sqlite3VdbeAddOp3( OpCode.OP_Column,iTab,0,iReg);
							}
							v.sqlite3VdbeAddOp1(OpCode.OP_IsNull,iReg);
						}
						else {
							pLevel.u._in.nIn=0;
						}
						#endif
					}
				pLevel.disableTerm(pTerm);
				return iReg;
			}
			public int codeAllEqualityTerms(///
			///<summary>
			///Parsing context 
			///</summary>
			WhereLevel pLevel,///
			///<summary>
			///Which nested loop of the FROM we are coding 
			///</summary>
			WhereClause pWC,///
			///<summary>
			///The WHERE clause 
			///</summary>
			Bitmask notReady,///
			///<summary>
			///Which parts of FROM have not yet been coded 
			///</summary>
			int nExtraReg,///
			///<summary>
			///Number of extra registers to allocate 
			///</summary>
			out StringBuilder pzAff///
			///<summary>
			///OUT: Set to point to affinity string 
			///</summary>
			) {
				int nEq=(int)pLevel.plan.nEq;
				///
				///<summary>
				///The number of == or IN constraints to code 
				///</summary>
				Vdbe v=this.pVdbe;
				///
				///<summary>
				///The vm under construction 
				///</summary>
				Index pIdx;
				///
				///<summary>
				///The index being used for this loop 
				///</summary>
				int iCur=pLevel.iTabCur;
				///
				///<summary>
				///The cursor of the table 
				///</summary>
				WhereTerm pTerm;
				///
				///<summary>
				///A single constraint term 
				///</summary>
				int j;
				///
				///<summary>
				///Loop counter 
				///</summary>
				int regBase;
				///
				///<summary>
				///Base register 
				///</summary>
				int nReg;
				///
				///<summary>
				///Number of registers to allocate 
				///</summary>
				StringBuilder zAff;
				///
				///<summary>
				///Affinity string to return 
				///</summary>
				///
				///<summary>
				///This module is only called on query plans that use an index. 
				///</summary>
				Debug.Assert((pLevel.plan.wsFlags&wherec.WHERE_INDEXED)!=0);
				pIdx=pLevel.plan.u.pIdx;
				///
				///<summary>
				///Figure out how many memory cells we will need then allocate them.
				///
				///</summary>
				regBase=this.UsedCellCount+1;
				nReg=(int)(pLevel.plan.nEq+nExtraReg);
				this.UsedCellCount+=nReg;
				zAff=new StringBuilder(v.sqlite3IndexAffinityStr(pIdx));
				//sqlite3DbStrDup(pParse.db, sqlite3IndexAffinityStr(v, pIdx));
				//if( null==zAff ){
				//  pParse.db.mallocFailed = 1;
				//}
				///
				///<summary>
				///Evaluate the equality constraints
				///
				///</summary>
				Debug.Assert(pIdx.nColumn>=nEq);
				for(j=0;j<nEq;j++) {
					int r1;
					int k=pIdx.aiColumn[j];
					pTerm=pWC.findTerm(iCur,k,notReady,pLevel.plan.wsFlags,pIdx);
					if(NEVER(pTerm==null))
						break;
					///
					///<summary>
					///The following true for indices with redundant columns. 
					///Ex: CREATE INDEX i1 ON t1(a,b,a); SELECT * FROM t1 WHERE a=0 AND b=0; 
					///</summary>
					sqliteinth.testcase((pTerm.wtFlags&WhereTermFlags.TERM_CODED)!=0);
					sqliteinth.testcase(pTerm.wtFlags&WhereTermFlags.TERM_VIRTUAL);
					///
					///<summary>
					///</summary>
					///<param name="EV: R">11662 </param>
					r1=this.codeEqualityTerm(pTerm,pLevel,regBase+j);
					if(r1!=regBase+j) {
						if(nReg==1) {
							this.deallocTempReg(regBase);
							regBase=r1;
						}
						else {
                            v.sqlite3VdbeAddOp2(OpCode.OP_SCopy, r1, regBase + j);
						}
					}
					sqliteinth.testcase(pTerm.eOperator&wherec.WO_ISNULL);
					sqliteinth.testcase(pTerm.eOperator&wherec.WO_IN);
					if((pTerm.eOperator&(wherec.WO_ISNULL|wherec.WO_IN))==0) {
						Expr pRight=pTerm.pExpr.pRight;
						exprc.sqlite3ExprCodeIsNullJump(v,pRight,regBase+j,pLevel.addrBrk);
						if(zAff.Length>0) {
                            if (pRight.sqlite3CompareAffinity(zAff[j]) == sqliteinth.SQLITE_AFF_NONE)
                            {
                                zAff[j] = sqliteinth.SQLITE_AFF_NONE;
							}
							if((exprc.sqlite3ExprNeedsNoAffinityChange(pRight,zAff[j]))!=0) {
                                zAff[j] = sqliteinth.SQLITE_AFF_NONE;
							}
						}
					}
				}
				pzAff=zAff;
				return regBase;
			}
			public void explainOneScan(///
			///<summary>
			///Parse context 
			///</summary>
			SrcList pTabList,///
			///<summary>
			///Table list this loop refers to 
			///</summary>
			WhereLevel pLevel,///
			///<summary>
			///Scan to write  OpCode.OP_Explain opcode for 
			///</summary>
			int iLevel,///
			///<summary>
			///Value for "level" column of output 
			///</summary>
			int iFrom,///
			///<summary>
			///Value for "from" column of output 
			///</summary>
			u16 wctrlFlags///
			///<summary>
			///Flags passed to sqlite3WhereBegin() 
			///</summary>
			) {
				if(this.explain==2) {
					u32 flags=pLevel.plan.wsFlags;
					SrcList_item pItem=pTabList.a[pLevel.iFrom];
					Vdbe v=this.pVdbe;
					///
					///<summary>
					///VM being constructed 
					///</summary>
					Connection db=this.db;
					///
					///<summary>
					///Database handle 
					///</summary>
					StringBuilder zMsg=new StringBuilder(1000);
					///
					///<summary>
					///Text to add to EQP output 
					///</summary>
					sqlite3_int64 nRow;
					///
					///<summary>
					///Expected number of rows visited by scan 
					///</summary>
					int iId=this.iSelectId;
					///
					///<summary>
					///</summary>
					///<param name="Select id (left">most output column) </param>
					bool isSearch;
					///
					///<summary>
					///True for a SEARCH. False for SCAN. 
					///</summary>
					if((flags&wherec.WHERE_MULTI_OR)!=0||(wctrlFlags&wherec.WHERE_ONETABLE_ONLY)!=0)
						return;
					isSearch=(pLevel.plan.nEq>0)||(flags&(wherec.WHERE_BTM_LIMIT|wherec.WHERE_TOP_LIMIT))!=0||(wctrlFlags&(wherec.WHERE_ORDERBY_MIN|wherec.WHERE_ORDERBY_MAX))!=0;
					zMsg.Append(io.sqlite3MPrintf(db,"%s",isSearch?"SEARCH":"SCAN"));
					if(pItem.pSelect!=null) {
						zMsg.Append(io.sqlite3MAppendf(db,null," SUBQUERY %d",pItem.iSelectId));
					}
					else {
						zMsg.Append(io.sqlite3MAppendf(db,null," TABLE %s",pItem.zName));
					}
					if(pItem.zAlias!=null) {
						zMsg.Append(io.sqlite3MAppendf(db,null," AS %s",pItem.zAlias));
					}
					if((flags&wherec.WHERE_INDEXED)!=0) {
						string zWhere=db.explainIndexRange(pLevel,pItem.pTab);
						zMsg.Append(io.sqlite3MAppendf(db,null," USING %s%sINDEX%s%s%s",((flags&wherec.WHERE_TEMP_INDEX)!=0?"AUTOMATIC ":""),((flags&wherec.WHERE_IDX_ONLY)!=0?"COVERING ":""),((flags&wherec.WHERE_TEMP_INDEX)!=0?"":" "),((flags&wherec.WHERE_TEMP_INDEX)!=0?"":pLevel.plan.u.pIdx.zName),zWhere!=null?zWhere:""));
						db.DbFree(ref zWhere);
					}
					else
						if((flags&(wherec.WHERE_ROWID_EQ|wherec.WHERE_ROWID_RANGE))!=0) {
							zMsg.Append(" USING INTEGER PRIMARY KEY");
							if((flags&wherec.WHERE_ROWID_EQ)!=0) {
								zMsg.Append(" (rowid=?)");
							}
							else
								if((flags&wherec.WHERE_BOTH_LIMIT)==wherec.WHERE_BOTH_LIMIT) {
									zMsg.Append(" (rowid>? AND rowid<?)");
								}
								else
									if((flags&wherec.WHERE_BTM_LIMIT)!=0) {
										zMsg.Append(" (rowid>?)");
									}
									else
										if((flags&wherec.WHERE_TOP_LIMIT)!=0) {
											zMsg.Append(" (rowid<?)");
										}
						}
						#if !SQLITE_OMIT_VIRTUALTABLE
						else
							if((flags&wherec.WHERE_VIRTUALTABLE)!=0) {
								sqlite3_index_info pVtabIdx=pLevel.plan.u.pVtabIdx;
								zMsg.Append(io.sqlite3MAppendf(db,null," VIRTUAL TABLE INDEX %d:%s",pVtabIdx.idxNum,pVtabIdx.idxStr));
							}
					#endif
					if((wctrlFlags&(wherec.WHERE_ORDERBY_MIN|wherec.WHERE_ORDERBY_MAX))!=0) {
						sqliteinth.testcase(wctrlFlags&wherec.WHERE_ORDERBY_MIN);
						nRow=1;
					}
					else {
						nRow=(sqlite3_int64)pLevel.plan.nRow;
					}
					zMsg.Append(io.sqlite3MAppendf(db,null," (~%lld rows)",nRow));
                    v.sqlite3VdbeAddOp4(OpCode.OP_Explain, iId, iLevel, iFrom, zMsg,  P4Usage.P4_DYNAMIC);
				}
			}
			public WhereInfo sqlite3WhereBegin(///
			///<summary>
			///The parser context 
			///</summary>
			SrcList pTabList,///
			///<summary>
			///A list of all tables to be scanned 
			///</summary>
			Expr pWhere,///
			///<summary>
			///The WHERE clause 
			///</summary>
			ref ExprList ppOrderBy,///
			///<summary>
			///An ORDER BY clause, or NULL 
			///</summary>
			u16 wctrlFlags///
			///<summary>
			///One of the wherec.WHERE_* flags defined in sqliteInt.h 
			///</summary>
			) {
				int i;
				///
				///<summary>
				///Loop counter 
				///</summary>
				int nByteWInfo;
				///
				///<summary>
				///Num. bytes allocated for WhereInfo struct 
				///</summary>
				int nTabList;
				///
				///<summary>
				///Number of elements in pTabList 
				///</summary>
				WhereInfo pWInfo;
				///
				///<summary>
				///Will become the return value of this function 
				///</summary>
				Vdbe v=this.pVdbe;
				///
				///<summary>
				///The virtual data_base engine 
				///</summary>
				Bitmask notReady;
				///
				///<summary>
				///Cursors that are not yet positioned 
				///</summary>
				WhereMaskSet pMaskSet;
				///
				///<summary>
				///The expression mask set 
				///</summary>
				WhereClause pWC=new WhereClause();
				///
				///<summary>
				///Decomposition of the WHERE clause 
				///</summary>
				SrcList_item pTabItem;
				///
				///<summary>
				///A single entry from pTabList 
				///</summary>
				WhereLevel pLevel;
				///
				///<summary>
				///A single level in the pWInfo list 
				///</summary>
				int iFrom;
				///
				///<summary>
				///First unused FROM clause element 
				///</summary>
				int andFlags;
				///
				///<summary>
				///</summary>
				///<param name="AND">ed combination of all pWC.a[].wtFlags </param>
				Connection db;
				///
				///<summary>
				///Data_base connection 
				///</summary>
				///
				///<summary>
				///The number of tables in the FROM clause is limited by the number of
				///bits in a Bitmask
				///</summary>
                sqliteinth.testcase(pTabList.Count == Globals.BMS);
                if (pTabList.Count > Globals.BMS)
                {
                    utilc.sqlite3ErrorMsg(this, "at most %d tables in a join", Globals.BMS);
					return null;
				}
				///
				///<summary>
				///This function normally generates a nested loop for all tables in 
				///pTabList.  But if the wherec.WHERE_ONETABLE_ONLY flag is set, then we should
				///only generate code for the first table in pTabList and assume that
				///any cursors associated with subsequent tables are uninitialized.
				///
				///</summary>
				nTabList=((wctrlFlags&wherec.WHERE_ONETABLE_ONLY)!=0)?1:(int)pTabList.Count;
				///
				///<summary>
				///Allocate and initialize the WhereInfo structure that will become the
				///return value. A single allocation is used to store the WhereInfo
				///struct, the contents of WhereInfo.a[], the WhereClause structure
				///</summary>
				///<param name="and the WhereMaskSet structure. Since WhereClause contains an 8">byte</param>
				///<param name="field (type Bitmask) it must be aligned on an 8">byte boundary on</param>
				///<param name="some architectures. Hence the ROUND8() below.">some architectures. Hence the ROUND8() below.</param>
				///<param name=""></param>
				db=this.db;
				pWInfo=new WhereInfo();
				//nByteWInfo = ROUND8(sizeof(WhereInfo)+(nTabList-1)*sizeof(WhereLevel));
				//pWInfo = sqlite3DbMallocZero( db,
				//    nByteWInfo +
				//    sizeof( WhereClause ) +
				//    sizeof( WhereMaskSet )
				//);
				pWInfo.a=new WhereLevel[pTabList.Count];
				for(int ai=0;ai<pWInfo.a.Length;ai++) {
					pWInfo.a[ai]=new WhereLevel();
				}
				//if ( db.mallocFailed != 0 )
				//{
				//sqlite3DbFree(db, pWInfo);
				//pWInfo = 0;
				//  goto whereBeginError;
				//}
				pWInfo.nLevel=nTabList;
				pWInfo.pParse=this;
				pWInfo.pTabList=pTabList;
				pWInfo.iBreak=v.sqlite3VdbeMakeLabel();
				pWInfo.pWC=pWC=new WhereClause();
				// (WhereClause )((u8 )pWInfo)[nByteWInfo];
				pWInfo.wctrlFlags=wctrlFlags;
				pWInfo.savedNQueryLoop=this.nQueryLoop;
				//pMaskSet = (WhereMaskSet)pWC[1];
				///
				///<summary>
				///Split the WHERE clause into separate subexpressions where each
				///subexpression is separated by an AND operator.
				///
				///</summary>
				pMaskSet=new WhereMaskSet();
				//initMaskSet(pMaskSet);
				pWC.whereClauseInit(this,pMaskSet);
				this.sqlite3ExprCodeConstants(pWhere);
				pWC.whereSplit(pWhere,TokenType.TK_AND);
				///
				///<summary>
				///</summary>
				///<param name="IMP: R">53296 </param>
				///
				///<summary>
				///Special case: a WHERE clause that is constant.  Evaluate the
				///expression and either jump over all of the code or fall thru.
				///</summary>
				if(pWhere!=null&&(nTabList==0||pWhere.sqlite3ExprIsConstantNotJoin()!=0)) {
					this.sqlite3ExprIfFalse(pWhere,pWInfo.iBreak,sqliteinth.SQLITE_JUMPIFNULL);
					pWhere=null;
				}
				///
				///<summary>
				///Assign a bit from the bitmask to every term in the FROM clause.
				///
				///When assigning bitmask values to FROM clause cursors, it must be
				///</summary>
				///<param name="the case that if X is the bitmask for the N">th FROM clause term then</param>
				///<param name="the bitmask for all FROM clause terms to the left of the N">th term</param>
				///<param name="is (X">1).   An expression from the ON clause of a LEFT JOIN can use</param>
				///<param name="its Expr.iRightJoinTable value to find the bitmask of the right table">its Expr.iRightJoinTable value to find the bitmask of the right table</param>
				///<param name="of the join.  Subtracting one from the right table bitmask gives a">of the join.  Subtracting one from the right table bitmask gives a</param>
				///<param name="bitmask for all tables to the left of the join.  Knowing the bitmask">bitmask for all tables to the left of the join.  Knowing the bitmask</param>
				///<param name="for all tables to the left of a left join is important.  Ticket #3015.">for all tables to the left of a left join is important.  Ticket #3015.</param>
				///<param name=""></param>
				///<param name="Configure the WhereClause.vmask variable so that bits that correspond">Configure the WhereClause.vmask variable so that bits that correspond</param>
				///<param name="to virtual table cursors are set. This is used to selectively disable">to virtual table cursors are set. This is used to selectively disable</param>
				///<param name="the OR">IN transformation in exprAnalyzeOrTerm(). It is not helpful</param>
				///<param name="with virtual tables.">with virtual tables.</param>
				///<param name=""></param>
				///<param name="Note that bitmasks are created for all pTabList.nSrc tables in">Note that bitmasks are created for all pTabList.nSrc tables in</param>
				///<param name="pTabList, not just the first nTabList tables.  nTabList is normally">pTabList, not just the first nTabList tables.  nTabList is normally</param>
				///<param name="equal to pTabList.nSrc but might be shortened to 1 if the">equal to pTabList.nSrc but might be shortened to 1 if the</param>
				///<param name="wherec.WHERE_ONETABLE_ONLY flag is set.">wherec.WHERE_ONETABLE_ONLY flag is set.</param>
				///<param name=""></param>
				Debug.Assert(pWC.vmask==0&&pMaskSet.n==0);
				for(i=0;i<pTabList.Count;i++) {
					pMaskSet.createMask(pTabList.a[i].iCursor);
					#if !SQLITE_OMIT_VIRTUALTABLE
                    if (Sqlite3.ALWAYS(pTabList.a[i].pTab) && pTabList.a[i].pTab.IsVirtual())
                    {
						pWC.vmask|=((Bitmask)1<<i);
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
				///
				///<summary>
				///Analyze all of the subexpressions.  Note that exprAnalyze() might
				///add new virtual terms onto the end of the WHERE clause.  We do not
				///want to analyze these virtual terms, so start analyzing at the end
				///and work forward so that the added virtual terms are never processed.
				///</summary>
				pTabList.exprAnalyzeAll(pWC);
				//if ( db.mallocFailed != 0 )
				//{
				//  goto whereBeginError;
				//}
				///
				///<summary>
				///Chose the best index to use for each table in the FROM clause.
				///
				///This loop fills in the following fields:
				///
				///pWInfo.a[].pIdx      The index to use for this level of the loop.
				///pWInfo.a[].wsFlags   wherec.WHERE_xxx flags Debug.Associated with pIdx
				///pWInfo.a[].nEq       The number of == and IN constraints
				///pWInfo.a[].iFrom     Which term of the FROM clause is being coded
				///pWInfo.a[].iTabCur   The VDBE cursor for the data_base table
				///pWInfo.a[].iIdxCur   The VDBE cursor for the index
				///</summary>
				///<param name="pWInfo.a[].pTerm     When wsFlags==wherec.WO_OR, the OR">clause term</param>
				///<param name=""></param>
				///<param name="This loop also figures out the nesting order of tables in the FROM">This loop also figures out the nesting order of tables in the FROM</param>
				///<param name="clause.">clause.</param>
				///<param name=""></param>
				notReady=~(Bitmask)0;
				andFlags=~0;
				#if (SQLITE_TEST) && (SQLITE_DEBUG)
																																																																																																				      WHERETRACE( "*** Optimizer Start ***\n" );
#endif
				for(i=iFrom=0;i<nTabList;i++)//, pLevel++ )
				 {
					pLevel=pWInfo.a[i];
					WhereCost bestPlan;
					///
					///<summary>
					///Most efficient plan seen so far 
					///</summary>
					Index pIdx;
					///
					///<summary>
					///Index for FROM table at pTabItem 
					///</summary>
					int j;
					///
					///<summary>
					///For looping over FROM tables 
					///</summary>
					int bestJ=-1;
					///
					///<summary>
					///The value of j 
					///</summary>
					Bitmask m;
					///
					///<summary>
					///Bitmask value for j or bestJ 
					///</summary>
					int isOptimal;
					///
					///<summary>
					///</summary>
					///<param name="Iterator for optimal/non">optimal search </param>
					int nUnconstrained;
					///
					///<summary>
					///Number tables without INDEXED BY 
					///</summary>
					Bitmask notIndexed;
					///
					///<summary>
					///Mask of tables that cannot use an index 
					///</summary>
					bestPlan=new WhereCost();
					// memset( &bestPlan, 0, sizeof( bestPlan ) );
                    bestPlan.rCost = sqliteinth.SQLITE_BIG_DBL;
					#if (SQLITE_TEST) && (SQLITE_DEBUG)
																																																																																																																																		        WHERETRACE( "*** Begin search for loop %d ***\n", i );
#endif
					///
					///<summary>
					///Loop through the remaining entries in the FROM clause to find the
					///next nested loop. The loop tests all FROM clause entries
					///either once or twice. 
					///
					///The first test is always performed if there are two or more entries
					///remaining and never performed if there is only one FROM clause entry
					///to choose from.  The first test looks for an "optimal" scan.  In
					///this context an optimal scan is one that uses the same strategy
					///for the given FROM clause entry as would be selected if the entry
					///were used as the innermost nested loop.  In other words, a table
					///is chosen such that the cost of running that table cannot be reduced
					///by waiting for other tables to run first.  This "optimal" test works
					///by first assuming that the FROM clause is on the inner loop and finding
					///its query plan, then checking to see if that query plan uses any
					///other FROM clause terms that are notReady.  If no notReady terms are
					///used then the "optimal" query plan works.
					///
					///Note that the WhereCost.nRow parameter for an optimal scan might
					///not be as small as it would be if the table really were the innermost
					///join.  The nRow value can be reduced by WHERE clause constraints
					///that do not use indices.  But this nRow reduction only happens if the
					///table really is the innermost join.  
					///
					///The second loop iteration is only performed if no optimal scan
					///strategies were found by the first iteration. This second iteration
					///is used to search for the lowest cost scan overall.
					///
					///</summary>
					///<param name="Previous versions of SQLite performed only the second iteration "></param>
					///<param name="the next outermost loop was always that with the lowest overall">the next outermost loop was always that with the lowest overall</param>
					///<param name="cost. However, this meant that SQLite could select the wrong plan">cost. However, this meant that SQLite could select the wrong plan</param>
					///<param name="for scripts such as the following:">for scripts such as the following:</param>
					///<param name=""></param>
					///<param name="CREATE TABLE t1(a, b); ">CREATE TABLE t1(a, b); </param>
					///<param name="CREATE TABLE t2(c, d);">CREATE TABLE t2(c, d);</param>
					///<param name="SELECT * FROM t2, t1 WHERE t2.rowid = t1.a;">SELECT * FROM t2, t1 WHERE t2.rowid = t1.a;</param>
					///<param name=""></param>
					///<param name="The best strategy is to iterate through table t1 first. However it">The best strategy is to iterate through table t1 first. However it</param>
					///<param name="is not possible to determine this with a simple greedy algorithm.">is not possible to determine this with a simple greedy algorithm.</param>
					///<param name="Since the cost of a linear scan through table t2 is the same ">Since the cost of a linear scan through table t2 is the same </param>
					///<param name="as the cost of a linear scan through table t1, a simple greedy ">as the cost of a linear scan through table t1, a simple greedy </param>
					///<param name="algorithm may choose to use t2 for the outer loop, which is a much">algorithm may choose to use t2 for the outer loop, which is a much</param>
					///<param name="costlier approach.">costlier approach.</param>
					nUnconstrained=0;
					notIndexed=0;
					for(isOptimal=(iFrom<nTabList-1)?1:0;isOptimal>=0&&bestJ<0;isOptimal--) {
						Bitmask mask;
						///
						///<summary>
						///Mask of tables not yet ready 
						///</summary>
						for(j=iFrom;j<nTabList;j++)//, pTabItem++)
						 {
							pTabItem=pTabList.a[j];
							int doNotReorder;
							///
							///<summary>
							///True if this table should not be reordered 
							///</summary>
							WhereCost sCost=new WhereCost();
							///
							///<summary>
							///Cost information from best[Virtual]Index() 
							///</summary>
							ExprList pOrderBy;
							///
							///<summary>
							///ORDER BY clause for index to optimize 
							///</summary>
                            doNotReorder = (pTabItem.jointype & (JoinType.JT_LEFT | JoinType.JT_CROSS)) != 0 ? 1 : 0;
							if((j!=iFrom&&doNotReorder!=0))
								break;
							m=pMaskSet.getMask(pTabItem.iCursor);
							if((m&notReady)==0) {
								if(j==iFrom)
									iFrom++;
								continue;
							}
							mask=(isOptimal!=0?m:notReady);
							pOrderBy=((i==0&&ppOrderBy!=null)?ppOrderBy:null);
							if(pTabItem.pIndex==null)
								nUnconstrained++;
							#if (SQLITE_TEST) && (SQLITE_DEBUG)
																																																																																																																																																																																														            WHERETRACE( "=== trying table %d with isOptimal=%d ===\n",
            j, isOptimal );
#endif
							Debug.Assert(pTabItem.pTab!=null);
							#if !SQLITE_OMIT_VIRTUALTABLE
							if(pTabItem.pTab.IsVirtual()) {
								sqlite3_index_info pp=pWInfo.a[j].pIdxInfo;
								this.bestVirtualIndex(pWC,pTabItem,mask,notReady,pOrderBy,ref sCost,ref pp);
							}
							else
							#endif
							 {
								this.bestBtreeIndex(pWC,pTabItem,mask,notReady,pOrderBy,ref sCost);
							}
							Debug.Assert(isOptimal!=0||(sCost.used&notReady)==0);
							///
							///<summary>
							///If an INDEXED BY clause is present, then the plan must use that
							///index if it uses any index at all 
							///</summary>
							Debug.Assert(pTabItem.pIndex==null||(sCost.plan.wsFlags&wherec.WHERE_NOT_FULLSCAN)==0||sCost.plan.u.pIdx==pTabItem.pIndex);
							if(isOptimal!=0&&(sCost.plan.wsFlags&wherec.WHERE_NOT_FULLSCAN)==0) {
								notIndexed|=m;
							}
							///
							///<summary>
							///Conditions under which this table becomes the best so far:
							///
							///(1) The table must not depend on other tables that have not
							///yet run.
							///
							///</summary>
							///<param name="(2) A full">scan plan cannot supercede indexed plan unless</param>
							///<param name="the full">scan is an "optimal" plan as defined above.</param>
							///<param name=""></param>
							///<param name="(3) All tables have an INDEXED BY clause or this table lacks an">(3) All tables have an INDEXED BY clause or this table lacks an</param>
							///<param name="INDEXED BY clause or this table uses the specific">INDEXED BY clause or this table uses the specific</param>
							///<param name="index specified by its INDEXED BY clause.  This rule ensures">index specified by its INDEXED BY clause.  This rule ensures</param>
							///<param name="that a best">far is always selected even if an impossible</param>
							///<param name="combination of INDEXED BY clauses are given.  The error">combination of INDEXED BY clauses are given.  The error</param>
							///<param name="will be detected and relayed back to the application later.">will be detected and relayed back to the application later.</param>
							///<param name="The NEVER() comes about because rule (2) above prevents">The NEVER() comes about because rule (2) above prevents</param>
							///<param name="An indexable full">scan from reaching rule (3).</param>
							///<param name=""></param>
							///<param name="(4) The plan cost must be lower than prior plans or else the">(4) The plan cost must be lower than prior plans or else the</param>
							///<param name="cost must be the same and the number of rows must be lower.">cost must be the same and the number of rows must be lower.</param>
							///<param name=""></param>
							if((sCost.used&notReady)==0///
							///<summary>
							///(1) 
							///</summary>
							&&(bestJ<0||(notIndexed&m)!=0///
							///<summary>
							///(2) 
							///</summary>
							||(bestPlan.plan.wsFlags&wherec.WHERE_NOT_FULLSCAN)==0||(sCost.plan.wsFlags&wherec.WHERE_NOT_FULLSCAN)!=0)&&(nUnconstrained==0||pTabItem.pIndex==null///
							///<summary>
							///(3) 
							///</summary>
							||NEVER((sCost.plan.wsFlags&wherec.WHERE_NOT_FULLSCAN)!=0))&&(bestJ<0||sCost.rCost<bestPlan.rCost///
							///<summary>
							///(4) 
							///</summary>
							||(sCost.rCost<=bestPlan.rCost&&sCost.plan.nRow<bestPlan.plan.nRow))) {
								#if (SQLITE_TEST) && (SQLITE_DEBUG)
																																																																																																																																																																																																																												              WHERETRACE( "=== table %d is best so far" +
              " with cost=%g and nRow=%g\n",
              j, sCost.rCost, sCost.plan.nRow );
#endif
								bestPlan=sCost;
								bestJ=j;
							}
							if(doNotReorder!=0)
								break;
						}
					}
					Debug.Assert(bestJ>=0);
					Debug.Assert((notReady&pMaskSet.getMask(pTabList.a[bestJ].iCursor))!=0);
					#if (SQLITE_TEST) && (SQLITE_DEBUG)
																																																																																																																																		        WHERETRACE( "*** Optimizer selects table %d for loop %d" +
        " with cost=%g and nRow=%g\n",
        bestJ, i,//pLevel-pWInfo.a,
        bestPlan.rCost, bestPlan.plan.nRow );
#endif
					if((bestPlan.plan.wsFlags&wherec.WHERE_ORDERBY)!=0) {
						ppOrderBy=null;
					}
					andFlags=(int)(andFlags&bestPlan.plan.wsFlags);
					pLevel.plan=bestPlan.plan;
					sqliteinth.testcase(bestPlan.plan.wsFlags&wherec.WHERE_INDEXED);
					sqliteinth.testcase(bestPlan.plan.wsFlags&wherec.WHERE_TEMP_INDEX);
					if((bestPlan.plan.wsFlags&(wherec.WHERE_INDEXED|wherec.WHERE_TEMP_INDEX))!=0) {
						pLevel.iIdxCur=this.nTab++;
					}
					else {
						pLevel.iIdxCur=-1;
					}
					notReady&=~pMaskSet.getMask(pTabList.a[bestJ].iCursor);
					pLevel.iFrom=(u8)bestJ;
					if(bestPlan.plan.nRow>=(double)1) {
						this.nQueryLoop*=bestPlan.plan.nRow;
					}
					///
					///<summary>
					///Check that if the table scanned by this loop iteration had an
					///INDEXED BY clause attached to it, that the named index is being
					///used for the scan. If not, then query compilation has failed.
					///Return an error.
					///
					///</summary>
					pIdx=pTabList.a[bestJ].pIndex;
					if(pIdx!=null) {
						if((bestPlan.plan.wsFlags&wherec.WHERE_INDEXED)==0) {
							utilc.sqlite3ErrorMsg(this,"cannot use index: %s",pIdx.zName);
							goto whereBeginError;
						}
						else {
							///
							///<summary>
							///If an INDEXED BY clause is used, the bestIndex() function is
							///guaranteed to find the index specified in the INDEXED BY clause
							///if it find an index at all. 
							///</summary>
							Debug.Assert(bestPlan.plan.u.pIdx==pIdx);
						}
					}
				}
				#if (SQLITE_TEST) && (SQLITE_DEBUG)
																																																																																																				      WHERETRACE( "*** Optimizer Finished ***\n" );
#endif
				if(this.nErr!=0///
				///<summary>
				///|| db.mallocFailed != 0 
				///</summary>
				) {
					goto whereBeginError;
				}
				///
				///<summary>
				///If the total query only selects a single row, then the ORDER BY
				///clause is irrelevant.
				///
				///</summary>
				if((andFlags&wherec.WHERE_UNIQUE)!=0&&ppOrderBy!=null) {
					ppOrderBy=null;
				}
				///
				///<summary>
				///If the caller is an UPDATE or DELETE statement that is requesting
				///</summary>
				///<param name="to use a one">pDebug.Ass algorithm, determine if this is appropriate.</param>
				///<param name="The one">pass algorithm only works if the WHERE clause constraints</param>
				///<param name="the statement to update a single row.">the statement to update a single row.</param>
				///<param name=""></param>
				Debug.Assert((wctrlFlags&wherec.WHERE_ONEPASS_DESIRED)==0||pWInfo.nLevel==1);
				if((wctrlFlags&wherec.WHERE_ONEPASS_DESIRED)!=0&&(andFlags&wherec.WHERE_UNIQUE)!=0) {
					pWInfo.okOnePass=1;
					pWInfo.a[0].plan.wsFlags=(u32)(pWInfo.a[0].plan.wsFlags&~wherec.WHERE_IDX_ONLY);
				}
				///
				///<summary>
				///Open all tables in the pTabList and any indices selected for
				///searching those tables.
				///
				///</summary>
				build.sqlite3CodeVerifySchema(this,-1);
				///
				///<summary>
				///Insert the cookie verifier Goto 
				///</summary>
				notReady=~(Bitmask)0;
				pWInfo.nRowOut=(double)1;
				for(i=0;i<nTabList;i++)//, pLevel++ )
				 {
					pLevel=pWInfo.a[i];
					Table pTab;
					///
					///<summary>
					///Table to open 
					///</summary>
					int iDb;
					///
					///<summary>
					///Index of data_base containing table/index 
					///</summary>
					pTabItem=pTabList.a[pLevel.iFrom];
					pTab=pTabItem.pTab;
					pLevel.iTabCur=pTabItem.iCursor;
					pWInfo.nRowOut*=pLevel.plan.nRow;
					iDb=db.indexOfBackendWithSchema(pTab.pSchema);
					if((pTab.tabFlags&TableFlags.TF_Ephemeral)!=0||pTab.pSelect!=null) {
						///
						///<summary>
						///Do nothing 
						///</summary>
					}
					else
						#if !SQLITE_OMIT_VIRTUALTABLE
						if((pLevel.plan.wsFlags&wherec.WHERE_VIRTUALTABLE)!=0) {
							VTable pVTab=VTableMethodsExtensions.sqlite3GetVTable(db,pTab);
							int iCur=pTabItem.iCursor;
                            v.sqlite3VdbeAddOp4(OpCode.OP_VOpen, iCur, 0, 0, pVTab,  P4Usage.P4_VTAB);
						}
						else
							#endif
							if((pLevel.plan.wsFlags&wherec.WHERE_IDX_ONLY)==0&&(wctrlFlags&wherec.WHERE_OMIT_OPEN)==0) {
                                OpCode op = pWInfo.okOnePass != 0 ? OpCode.OP_OpenWrite :  OpCode.OP_OpenRead;
								this.sqlite3OpenTable(pTabItem.iCursor,iDb,pTab,op);
								sqliteinth.testcase(pTab.nCol==Globals.BMS-1);
                                sqliteinth.testcase(pTab.nCol == Globals.BMS);
                                if (0 == pWInfo.okOnePass && pTab.nCol < Globals.BMS)
                                {
									Bitmask b=pTabItem.colUsed;
									int n=0;
									for(;b!=0;b=b>>1,n++) {
									}
									v.sqlite3VdbeChangeP4(v.sqlite3VdbeCurrentAddr()-1,n, P4Usage.P4_INT32);
									//SQLITE_INT_TO_PTR(n)
									Debug.Assert(n<=pTab.nCol);
								}
							}
							else {
								sqliteinth.sqlite3TableLock(this,iDb,pTab.tnum,0,pTab.zName);
							}
					#if !SQLITE_OMIT_AUTOMATIC_INDEX
					if((pLevel.plan.wsFlags&wherec.WHERE_TEMP_INDEX)!=0) {
						this.constructAutomaticIndex(pWC,pTabItem,notReady,pLevel);
					}
					else
						#endif
						if((pLevel.plan.wsFlags&wherec.WHERE_INDEXED)!=0) {
							Index pIx=pLevel.plan.u.pIdx;
                            KeyInfo pKey = pIx.sqlite3IndexKeyinfo(this);
							int iIdxCur=pLevel.iIdxCur;
							Debug.Assert(pIx.pSchema==pTab.pSchema);
							Debug.Assert(iIdxCur>=0);
							v.sqlite3VdbeAddOp4( OpCode.OP_OpenRead,iIdxCur,pIx.tnum,iDb,pKey, P4Usage.P4_KEYINFO_HANDOFF);
							#if SQLITE_DEBUG
																																																																																																																																																																																														            VdbeComment( v, "%s", pIx.zName );
#endif
						}
					build.sqlite3CodeVerifySchema(this,iDb);
					notReady&=~pWC.pMaskSet.getMask(pTabItem.iCursor);
				}
				pWInfo.iTop=v.sqlite3VdbeCurrentAddr();
				//if( db.mallocFailed ) goto whereBeginError;
				///
				///<summary>
				///Generate the code to do the search.  Each iteration of the for
				///loop below generates code for a single nested loop of the VM
				///program.
				///
				///</summary>
				notReady=~(Bitmask)0;
				for(i=0;i<nTabList;i++) {
					pLevel=pWInfo.a[i];
					this.explainOneScan(pTabList,pLevel,i,pLevel.iFrom,wctrlFlags);
					notReady=pWInfo.codeOneLoopStart(i,wctrlFlags,notReady);
					pWInfo.iContinue=pLevel.addrCont;
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
          if ( ( pLevel.plan.wsFlags & wherec.WHERE_IDX_ONLY ) != 0 )
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
        sqliteinth.testcase( pLevel.plan.wsFlags & wherec.WHERE_ROWID_EQ );
        sqliteinth.testcase( pLevel.plan.wsFlags & wherec.WHERE_ROWID_RANGE );
        if ( ( pLevel.plan.wsFlags & ( wherec.WHERE_ROWID_EQ | wherec.WHERE_ROWID_RANGE ) ) != 0 )
        {
          sqlite3_query_plan.Append( "* " ); //memcpy(&sqlite3_query_plan[nQPlan], "* ", 2);
          nQPlan += 2;
        }
        else if ( ( pLevel.plan.wsFlags & wherec.WHERE_INDEXED ) != 0 )
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
				///
				///<summary>
				///Record the continuation address in the WhereInfo structure.  Then
				///clean up and return.
				///</summary>
				return pWInfo;
				///
				///<summary>
				///Jump here if malloc fails 
				///</summary>
				whereBeginError:
				if(pWInfo!=null) {
					this.nQueryLoop=pWInfo.savedNQueryLoop;
					db.whereInfoFree(pWInfo);
				}
				return null;
			}
			public bool isSortingIndex(///
			///<summary>
			///Parsing context 
			///</summary>
			WhereMaskSet pMaskSet,///
			///<summary>
			///Mapping from table cursor numbers to bitmaps 
			///</summary>
			Index pIdx,///
			///<summary>
			///The index we are testing 
			///</summary>
			int _base,///
			///<summary>
			///Cursor number for the table to be sorted 
			///</summary>
			ExprList pOrderBy,///
			///<summary>
			///The ORDER BY clause 
			///</summary>
			int nEqCol,///
			///<summary>
			///Number of index columns with == constraints 
			///</summary>
			int wsFlags,///
			///<summary>
			///Index usages flags 
			///</summary>
			ref int pbRev///
			///<summary>
			///Set to 1 if ORDER BY is DESC 
			///</summary>
			) {
				int i,j;
				///
				///<summary>
				///Loop counters 
				///</summary>
                SortOrder sortOrder = (SortOrder)0;
				///
				///<summary>
				///XOR of index and ORDER BY sort direction 
				///</summary>
				int nTerm;
				///
				///<summary>
				///Number of ORDER BY terms 
				///</summary>
				ExprList_item pTerm;
				///
				///<summary>
				///A term of the ORDER BY clause 
				///</summary>
				Connection db=this.db;
				Debug.Assert(pOrderBy!=null);
				nTerm=pOrderBy.Count;
				Debug.Assert(nTerm>0);
				///
				///<summary>
				///Argument pIdx must either point to a 'real' named index structure, 
				///or an index structure allocated on the stack by bestBtreeIndex() to
				///represent the rowid index that is part of every table.  
				///</summary>
				Debug.Assert(!String.IsNullOrEmpty(pIdx.zName)||(pIdx.nColumn==1&&pIdx.aiColumn[0]==-1));
				///
				///<summary>
				///Match terms of the ORDER BY clause against columns of
				///the index.
				///
				///Note that indices have pIdx.nColumn regular columns plus
				///one additional column containing the rowid.  The rowid column
				///of the index is also allowed to match against the ORDER BY
				///clause.
				///
				///</summary>
				for(i=j=0;j<nTerm&&i<=pIdx.nColumn;i++) {
					pTerm=pOrderBy.a[j];
					Expr pExpr;
					///
					///<summary>
					///The expression of the ORDER BY pTerm 
					///</summary>
					CollSeq pColl;
					///
					///<summary>
					///The collating sequence of pExpr 
					///</summary>
					SortOrder termSortOrder;
					///
					///<summary>
					///Sort order for this term 
					///</summary>
					int iColumn;
					///
					///<summary>
					///</summary>
					///<param name="The i">1 for rowid </param>
					SortOrder iSortOrder;
					///
					///<summary>
					///</summary>
					///<param name="1 for DESC, 0 for ASC on the i">th index term </param>
					string zColl;
					///
					///<summary>
					///</summary>
					///<param name="Name of the collating sequence for i">th index term </param>
					pExpr=pTerm.pExpr;
					if(pExpr.Operator!=TokenType.TK_COLUMN||pExpr.iTable!=_base) {
						///
						///<summary>
						///Can not use an index sort on anything that is not a column in the
						///</summary>
						///<param name="left">most table of the FROM clause </param>
						break;
					}
					pColl=this.sqlite3ExprCollSeq(pExpr);
					if(null==pColl) {
						pColl=db.pDfltColl;
					}
					if(!String.IsNullOrEmpty(pIdx.zName)&&i<pIdx.nColumn) {
						iColumn=pIdx.aiColumn[i];
						if(iColumn==pIdx.pTable.iPKey) {
							iColumn=-1;
						}
						iSortOrder=pIdx.aSortOrder[i];
						zColl=pIdx.azColl[i];
					}
					else {
						iColumn=-1;
						iSortOrder=0;
						zColl=pColl.zName;
					}
					if(pExpr.iColumn!=iColumn||!pColl.zName.Equals(zColl,StringComparison.InvariantCultureIgnoreCase)) {
						///
						///<summary>
						///Term j of the ORDER BY clause does not match column i of the index 
						///</summary>
						if(i<nEqCol) {
							///
							///<summary>
							///If an index column that is constrained by == fails to match an
							///ORDER BY term, that is OK.  Just ignore that column of the index
							///
							///</summary>
							continue;
						}
						else
							if(i==pIdx.nColumn) {
								///
								///<summary>
								///Index column i is the rowid.  All other terms match. 
								///</summary>
								break;
							}
							else {
								///
								///<summary>
								///If an index column fails to match and is not constrained by ==
								///then the index cannot satisfy the ORDER BY constraint.
								///
								///</summary>
								return false;
							}
					}
					Debug.Assert(pIdx.aSortOrder!=null||iColumn==-1);
					Debug.Assert((int)pTerm.sortOrder==0||(int)pTerm.sortOrder==1);
					Debug.Assert((int)iSortOrder==0||(int)iSortOrder==1);
					termSortOrder=iSortOrder^pTerm.sortOrder;
					if(i>nEqCol) {
						if(termSortOrder!=sortOrder) {
							///
							///<summary>
							///Indices can only be used if all ORDER BY terms past the
							///equality constraints are all either DESC or ASC. 
							///</summary>
							return false;
						}
					}
					else {
						sortOrder=termSortOrder;
					}
					j++;
					//pTerm++;
					if(iColumn<0&&!pOrderBy.referencesOtherTables(pMaskSet,j,_base)) {
						///
						///<summary>
						///If the indexed column is the primary key and everything matches
						///so far and none of the ORDER BY terms to the right reference other
						///tables in the join, then we are Debug.Assured that the index can be used
						///to sort because the primary key is unique and so none of the other
						///columns will make any difference
						///
						///</summary>
						j=nTerm;
					}
				}
				pbRev=sortOrder!=0?1:0;
				if(j>=nTerm) {
					///
					///<summary>
					///All terms of the ORDER BY clause are covered by this index so
					///this index can be used for sorting. 
					///</summary>
					return true;
				}
				if(pIdx.onError!=OnConstraintError.OE_None&&i==pIdx.nColumn&&(wsFlags&wherec.WHERE_COLUMN_NULL)==0&&!pOrderBy.referencesOtherTables(pMaskSet,j,_base)) {
					///
					///<summary>
					///All terms of this index match some prefix of the ORDER BY clause
					///and the index is UNIQUE and no terms on the tail of the ORDER BY
					///clause reference other tables in a join.  If this is all true then
					///the order by clause is superfluous.  Not that if the matching
					///condition is IS NULL then the result is not necessarily unique
					///even on a UNIQUE index, so disallow those cases. 
					///</summary>
					return true;
				}
				return false;
			}
			public void binaryToUnaryIfNull(Expr pY,Expr pA,int op) {
				Connection db=this.db;
				if(///
				///<summary>
				///db.mallocFailed == null && 
				///</summary>
				pY.Operator==TokenType.TK_NULL) {
					pA.Operator=(TokenType)op;
					exprc.Delete(db,ref pA.pRight);
					pA.pRight=null;
				}
			}
			public int sqlite3FindInIndex(Expr pX,ref int prNotFound) {
				Select p;
				///
				///<summary>
				///SELECT to the right of IN operator 
				///</summary>
				int eType=0;
				///
				///<summary>
				///Type of RHS table. IN_INDEX_* 
				///</summary>
				int iTab=this.nTab++;
				///
				///<summary>
				///Cursor of the RHS table 
				///</summary>
				bool mustBeUnique=(prNotFound!=0);
				///
				///<summary>
				///True if RHS must be unique 
				///</summary>
				Debug.Assert(pX.Operator==TokenType.TK_IN);
				///
				///<summary>
				///Check to see if an existing table or index can be used to
				///satisfy the query.  This is preferable to generating a new
				///ephemeral table.
				///
				///</summary>
				p=(pX.HasProperty(ExprFlags.EP_xIsSelect)?pX.x.pSelect:null);
				if(Sqlite3.ALWAYS(this.nErr==0)&&exprc.isCandidateForInOpt(p)!=0) {
					Connection db=this.db;
					///
					///<summary>
					///Database connection 
					///</summary>
					Expr pExpr=p.ResultingFieldList.a[0].pExpr;
					///
					///<summary>
					///Expression <column> 
					///</summary>
					int iCol=pExpr.iColumn;
					///
					///<summary>
					///Index of column <column> 
					///</summary>
					Vdbe v=this.sqlite3GetVdbe();
					///
					///<summary>
					///Virtual machine being coded 
					///</summary>
					Table pTab=p.pSrc.a[0].pTab;
					///
					///<summary>
					///Table <table>. 
					///</summary>
					int iDb;
					///
					///<summary>
					///Database idx for pTab 
					///</summary>
					///
					///<summary>
					///Code an  OpCode.OP_VerifyCookie and  OpCode.OP_TableLock for <table>. 
					///</summary>
					iDb=db.indexOfBackendWithSchema(pTab.pSchema);
					build.sqlite3CodeVerifySchema(this,iDb);
					sqliteinth.sqlite3TableLock(this,iDb,pTab.tnum,0,pTab.zName);
					///
					///<summary>
					///This function is only called from two places. In both cases the vdbe
					///has already been allocated. So assume sqlite3GetVdbe() is always
					///successful here.
					///
					///</summary>
					Debug.Assert(v!=null);
					if(iCol<0) {
						int iMem=++this.UsedCellCount;
						int iAddr;
						iAddr=v.sqlite3VdbeAddOp1(OpCode.OP_If,iMem);
						v.sqlite3VdbeAddOp2(OpCode.OP_Integer,1,iMem);
						this.sqlite3OpenTable(iTab,iDb,pTab,OpCode.OP_OpenRead);
                        eType = sqliteinth.IN_INDEX_ROWID;
						v.sqlite3VdbeJumpHere(iAddr);
					}
					else {
						Index pIdx;
						///
						///<summary>
						///Iterator variable 
						///</summary>
						///
						///<summary>
						///The collation sequence used by the comparison. If an index is to
						///be used in place of a temp.table, it must be ordered according
						///to this collation sequence. 
						///</summary>
						CollSeq pReq=this.sqlite3BinaryCompareCollSeq(pX.pLeft,pExpr);
						///
						///<summary>
						///Check that the affinity that will be used to perform the
						///comparison is the same as the affinity of the column. If
						///it is not, it is not possible to use any index.
						///
						///</summary>
						char aff=pX.comparisonAffinity();
                        bool affinity_ok = (pTab.aCol[iCol].affinity == aff || aff == sqliteinth.SQLITE_AFF_NONE);
						for(pIdx=pTab.pIndex;pIdx!=null&&eType==0&&affinity_ok;pIdx=pIdx.pNext) {
							if((pIdx.aiColumn[0]==iCol)&&(db.sqlite3FindCollSeq(sqliteinth.ENC(db),pIdx.azColl[0],0)==pReq)&&(mustBeUnique==false||(pIdx.nColumn==1&&pIdx.onError!=OnConstraintError.OE_None))) {
								int iMem=++this.UsedCellCount;
								int iAddr;
								KeyInfo pKey;
                                pKey = pIdx.sqlite3IndexKeyinfo(this);
								iAddr=v.sqlite3VdbeAddOp1(OpCode.OP_If,iMem);
								v.sqlite3VdbeAddOp2(OpCode.OP_Integer,1,iMem);
								v.sqlite3VdbeAddOp4( OpCode.OP_OpenRead,iTab,pIdx.tnum,iDb,pKey, P4Usage.P4_KEYINFO_HANDOFF);
								#if SQLITE_DEBUG
																																																																																																																																																																																																				              VdbeComment( v, "%s", pIdx.zName );
#endif
                                eType = sqliteinth.IN_INDEX_INDEX;
								v.sqlite3VdbeJumpHere(iAddr);
								if(//prNotFound != null &&         -- always exists under C#
								pTab.aCol[iCol].notNull==0) {
									prNotFound=++this.UsedCellCount;
								}
							}
						}
					}
				}
				if(eType==0) {
					///
					///<summary>
					///</summary>
					///<param name="Could not found an existing table or index to use as the RHS b">tree.</param>
					///<param name="We will have to generate an ephemeral table to do the job.">We will have to generate an ephemeral table to do the job.</param>
					///<param name=""></param>
					double savedNQueryLoop=this.nQueryLoop;
					int rMayHaveNull=0;
					eType=sqliteinth.IN_INDEX_EPH;
					if(prNotFound!=-1)// Klude to show prNotFound not available
					 {
						prNotFound=rMayHaveNull=++this.UsedCellCount;
					}
					else {
						sqliteinth.testcase(this.nQueryLoop>(double)1);
						this.nQueryLoop=(double)1;
						if(pX.pLeft.iColumn<0&&!pX.ExprHasAnyProperty(ExprFlags.EP_xIsSelect)) {
                            eType = sqliteinth.IN_INDEX_ROWID;
						}
					}
                    this.sqlite3CodeSubselect(pX, rMayHaveNull, eType == sqliteinth.IN_INDEX_ROWID);
					this.nQueryLoop=savedNQueryLoop;
				}
				else {
					pX.iTable=iTab;
				}
				return eType;
			}
			public int sqlite3CodeSubselect(///
			///<summary>
			///Parsing context 
			///</summary>
			Expr pExpr,///
			///<summary>
			///The IN, SELECT, or EXISTS operator 
			///</summary>
			int rMayHaveNull,///
			///<summary>
			///Register that records whether NULLs exist in RHS 
			///</summary>
			bool isRowid///
			///<summary>
			///If true, LHS of IN operator is a rowid 
			///</summary>
			) {
				int testAddr=0;
				///
				///<summary>
				///</summary>
				///<param name="One">time test address </param>
				int rReg=0;
				///
				///<summary>
				///Register storing resulting 
				///</summary>
				Vdbe v=this.sqlite3GetVdbe();
				if(NEVER(v==null))
					return 0;
				this.sqlite3ExprCachePush();
				///
				///<summary>
				///This code must be run in its entirety every time it is encountered
				///if any of the following is true:
				///
				///</summary>
				///<param name="The right">hand side is a correlated subquery</param>
				///<param name="The right">hand side is an expression list containing variables</param>
				///<param name="We are inside a trigger">We are inside a trigger</param>
				///<param name=""></param>
				///<param name="If all of the above are false, then we can run this code just once">If all of the above are false, then we can run this code just once</param>
				///<param name="save the results, and reuse the same result on subsequent invocations.">save the results, and reuse the same result on subsequent invocations.</param>
				///<param name=""></param>
				if(!pExpr.ExprHasAnyProperty(ExprFlags.EP_VarSelect)&&null==this.pTriggerTab) {
					int mem=++this.UsedCellCount;
					v.sqlite3VdbeAddOp1(OpCode.OP_If,mem);
					testAddr=v.sqlite3VdbeAddOp2(OpCode.OP_Integer,1,mem);
					Debug.Assert(testAddr>0///
					///<summary>
					///|| pParse.db.mallocFailed != 0 
					///</summary>
					);
				}
				#if !SQLITE_OMIT_EXPLAIN
				if(this.explain==2) {
					string zMsg=io.sqlite3MPrintf(this.db,"EXECUTE %s%s SUBQUERY %d",testAddr!=0?"":"CORRELATED ",pExpr.Operator==TokenType.TK_IN?"LIST":"SCALAR",this.iNextSelectId);
                    v.sqlite3VdbeAddOp4(OpCode.OP_Explain, this.iSelectId, 0, 0, zMsg,  P4Usage.P4_DYNAMIC);
				}
				#endif
				switch(pExpr.Operator) {
				case TokenType.TK_IN: {
					char affinity;
					///
					///<summary>
					///Affinity of the LHS of the IN 
					///</summary>
					KeyInfo keyInfo;
					///
					///<summary>
					///Keyinfo for the generated table 
					///</summary>
					int addr;
					///
					///<summary>
					///Address of  OpCode.OP_OpenEphemeral instruction 
					///</summary>
					Expr pLeft=pExpr.pLeft;
					///
					///<summary>
					///the LHS of the IN operator 
					///</summary>
					if(rMayHaveNull!=0) {
                        v.sqlite3VdbeAddOp2(OpCode.OP_Null, 0, rMayHaveNull);
					}
					affinity=pLeft.sqlite3ExprAffinity();
					///
					///<summary>
					///Whether this is an 'x IN(SELECT...)' or an 'x IN(<exprlist>)'
					///expression it is handled the same way. An ephemeral table is
					///</summary>
					///<param name="filled with single">field index keys representing the results</param>
					///<param name="from the SELECT or the <exprlist>.">from the SELECT or the <exprlist>.</param>
					///<param name=""></param>
					///<param name="If the 'x' expression is a column value, or the SELECT...">If the 'x' expression is a column value, or the SELECT...</param>
					///<param name="statement returns a column value, then the affinity of that">statement returns a column value, then the affinity of that</param>
					///<param name="column is used to build the index keys. If both 'x' and the">column is used to build the index keys. If both 'x' and the</param>
					///<param name="SELECT... statement are columns, then numeric affinity is used">SELECT... statement are columns, then numeric affinity is used</param>
					///<param name="if either column has NUMERIC or INTEGER affinity. If neither">if either column has NUMERIC or INTEGER affinity. If neither</param>
					///<param name="'x' nor the SELECT... statement are columns, then numeric affinity">'x' nor the SELECT... statement are columns, then numeric affinity</param>
					///<param name="is used.">is used.</param>
					///<param name=""></param>
					pExpr.iTable=this.nTab++;
                    addr = v.sqlite3VdbeAddOp2(OpCode.OP_OpenEphemeral, (int)pExpr.iTable, !isRowid);
					if(rMayHaveNull==0)
						v.sqlite3VdbeChangeP5(BTREE_UNORDERED);
					keyInfo=new KeyInfo();
					// memset( &keyInfo, 0, sizeof(keyInfo ));
					keyInfo.nField=1;
					if(pExpr.HasProperty(ExprFlags.EP_xIsSelect)) {
						///
						///<summary>
						///Case 1:     expr IN (SELECT ...)
						///
						///Generate code to write the results of the select into the temporary
						///table allocated and opened above.
						///
						///</summary>
						SelectDest dest=new SelectDest();
						ExprList pEList;
						Debug.Assert(!isRowid);
						dest.Init(SelectResultType.Set,pExpr.iTable);
						dest.affinity=(char)affinity;
						Debug.Assert((pExpr.iTable&0x0000FFFF)==pExpr.iTable);
						pExpr.x.pSelect.iLimit=0;
						if(Select.sqlite3Select(this,pExpr.x.pSelect,ref dest)!=0) {
							return 0;
						}
						pEList=pExpr.x.pSelect.ResultingFieldList;
						if(Sqlite3.ALWAYS(pEList!=null)&&pEList.Count>0) {
							keyInfo.aColl[0]=this.sqlite3BinaryCompareCollSeq(pExpr.pLeft,pEList.a[0].pExpr);
						}
					}
					else
						if(Sqlite3.ALWAYS(pExpr.x.pList!=null)) {
							///
							///<summary>
							///Case 2:     expr IN (exprlist)
							///
							///For each expression, build an index key from the evaluation and
							///store it in the temporary table. If <expr> is a column, then use
							///that columns affinity when building index keys. If <expr> is not
							///a column, use numeric affinity.
							///
							///</summary>
							int i;
							ExprList pList=pExpr.x.pList;
							ExprList_item pItem;
							int r1,r2,r3;
							if(affinity=='\0') {
                                affinity = sqliteinth.SQLITE_AFF_NONE;
							}
							keyInfo.aColl[0]=this.sqlite3ExprCollSeq(pExpr.pLeft);
							///
							///<summary>
							///Loop through each expression in <exprlist>. 
							///</summary>
							r1=this.allocTempReg();
							r2=this.allocTempReg();
                            v.sqlite3VdbeAddOp2(OpCode.OP_Null, 0, r2);
							for(i=0;i<pList.Count;i++) {
								//, pItem++){
								pItem=pList.a[i];
								Expr pE2=pItem.pExpr;
								int iValToIns=0;
								///
								///<summary>
								///If the expression is not constant then we will need to
								///disable the test that was generated above that makes sure
								///</summary>
								///<param name="this code only executes once.  Because for a non">constant</param>
								///<param name="expression we need to rerun this code each time.">expression we need to rerun this code each time.</param>
								///<param name=""></param>
								if(testAddr!=0&&pE2.sqlite3ExprIsConstant()==0) {
                                    vdbeaux.sqlite3VdbeChangeToNoop(v, testAddr - 1, 2);
									testAddr=0;
								}
								///
								///<summary>
								///Evaluate the expression and insert it into the temp table 
								///</summary>
								if(isRowid&&pE2.sqlite3ExprIsInteger(ref iValToIns)) {
									v.sqlite3VdbeAddOp3( OpCode.OP_InsertInt,pExpr.iTable,r2,iValToIns);
								}
								else {
									r3=this.sqlite3ExprCodeTarget(pE2,r1);
									if(isRowid) {
										v.sqlite3VdbeAddOp2( OpCode.OP_MustBeInt,r3,v.sqlite3VdbeCurrentAddr()+2);
										v.sqlite3VdbeAddOp3( OpCode.OP_Insert,pExpr.iTable,r2,r3);
									}
									else {
										v.sqlite3VdbeAddOp4( OpCode.OP_MakeRecord,r3,1,r2,affinity,(P4Usage)1);
										this.sqlite3ExprCacheAffinityChange(r3,1);
										v.sqlite3VdbeAddOp2( OpCode.OP_IdxInsert,pExpr.iTable,r2);
									}
								}
							}
							this.deallocTempReg(r1);
							this.deallocTempReg(r2);
						}
					if(!isRowid) {
						v.sqlite3VdbeChangeP4(addr,keyInfo, P4Usage.P4_KEYINFO);
					}
					break;
				}
				case TokenType.TK_EXISTS:
				case TokenType.TK_SELECT:
				default: {
					///
					///<summary>
					///If this has to be a scalar SELECT.  Generate code to put the
					///value of this select in a memory cell and record the number
					///of the memory cell in iColumn.  If this is an EXISTS, write
					///an integer 0 (not exists) or 1 (exists) into a memory cell
					///and record that memory cell in iColumn.
					///
					///</summary>
					Select pSel;
					///
					///<summary>
					///SELECT statement to encode 
					///</summary>
					SelectDest dest=new SelectDest();
					///
					///<summary>
					///How to deal with SELECt result 
					///</summary>
					sqliteinth.testcase(pExpr.Operator==TokenType.TK_EXISTS);
					sqliteinth.testcase(pExpr.Operator==TokenType.TK_SELECT);
					Debug.Assert(pExpr.Operator==TokenType.TK_EXISTS||pExpr.Operator==TokenType.TK_SELECT);
					Debug.Assert(pExpr.HasProperty(ExprFlags.EP_xIsSelect));
					pSel=pExpr.x.pSelect;
					dest.Init(0,++this.UsedCellCount);
					if(pExpr.Operator==TokenType.TK_SELECT) {
						dest.eDest=SelectResultType.Mem;
                        v.sqlite3VdbeAddOp2(OpCode.OP_Null, 0, dest.iParm);
						#if SQLITE_DEBUG
																																																																																																																																														              VdbeComment( v, "Init subquery result" );
#endif
					}
					else {
						dest.eDest=SelectResultType.Exists;
						v.sqlite3VdbeAddOp2(OpCode.OP_Integer,0,dest.iParm);
						#if SQLITE_DEBUG
																																																																																																																																														              VdbeComment( v, "Init EXISTS result" );
#endif
					}
					exprc.Delete(this.db,ref pSel.pLimit);
					pSel.pLimit=this.sqlite3PExpr(TokenType.TK_INTEGER,null,null,ParseMethods.sqlite3IntTokens[1]);
					pSel.iLimit=0;
					if(Select.sqlite3Select(this,pSel,ref dest)!=0) {
						return 0;
					}
					rReg=dest.iParm;
					pExpr.ExprSetIrreducible();
					break;
				}
				}
				if(testAddr!=0) {
					v.sqlite3VdbeJumpHere(testAddr-1);
				}
				this.sqlite3ExprCachePop(1);
				return rReg;
			}
			public void sqlite3ExprCodeIN(///
			///<summary>
			///Parsing and code generating context 
			///</summary>
			Expr pExpr,///
			///<summary>
			///The IN expression 
			///</summary>
			int destIfFalse,///
			///<summary>
			///Jump here if LHS is not contained in the RHS 
			///</summary>
			int destIfNull///
			///<summary>
			///Jump here if the results are unknown due to NULLs 
			///</summary>
			) {
				int rRhsHasNull=0;
				///
				///<summary>
				///Register that is true if RHS contains NULL values 
				///</summary>
				char affinity;
				///
				///<summary>
				///Comparison affinity to use 
				///</summary>
				int eType;
				///
				///<summary>
				///Type of the RHS 
				///</summary>
				int r1;
				///
				///<summary>
				///Temporary use register 
				///</summary>
				Vdbe v;
				///
				///<summary>
				///Statement under construction 
				///</summary>
				///
				///<summary>
				///Compute the RHS.   After this step, the table with cursor
				///pExpr.iTable will contains the values that make up the RHS.
				///</summary>
				v=this.pVdbe;
				Debug.Assert(v!=null);
				///
				///<summary>
				///OOM detected prior to this routine 
				///</summary>
				v.VdbeNoopComment("begin IN expr");
				eType=this.sqlite3FindInIndex(pExpr,ref rRhsHasNull);
				///
				///<summary>
				///Figure out the affinity to use to create a key from the results
				///of the expression. affinityStr stores a static string suitable for
				///P4 of  OpCode.OP_MakeRecord.
				///
				///</summary>
				affinity=pExpr.comparisonAffinity();
				///
				///<summary>
				///Code the LHS, the <expr> from "<expr> IN (...)".
				///
				///</summary>
				this.sqlite3ExprCachePush();
				r1=this.allocTempReg();
				this.sqlite3ExprCode(pExpr.pLeft,r1);
				///
				///<summary>
				///If the LHS is NULL, then the result is either false or NULL depending
				///on whether the RHS is empty or not, respectively.
				///
				///</summary>
				if(destIfNull==destIfFalse) {
					///
					///<summary>
					///Shortcut for the common case where the false and NULL outcomes are
					///the same. 
					///</summary>
                    v.sqlite3VdbeAddOp2(OpCode.OP_IsNull, r1, destIfNull);
				}
				else {
					int addr1=v.sqlite3VdbeAddOp1(OpCode.OP_NotNull,r1);
                    v.sqlite3VdbeAddOp2(OpCode.OP_Rewind, pExpr.iTable, destIfFalse);
					v.sqlite3VdbeAddOp2(OpCode.OP_Goto,0,destIfNull);
					v.sqlite3VdbeJumpHere(addr1);
				}
                if (eType == sqliteinth.IN_INDEX_ROWID)
                {
					///
					///<summary>
					///</summary>
					///<param name="In this case, the RHS is the ROWID of table b">tree</param>
					///<param name=""></param>
					v.sqlite3VdbeAddOp2( OpCode.OP_MustBeInt,r1,destIfFalse);
                    v.sqlite3VdbeAddOp3(OpCode.OP_NotExists, pExpr.iTable, destIfFalse, r1);
				}
				else {
					///
					///<summary>
					///</summary>
					///<param name="In this case, the RHS is an index b">tree.</param>
					///<param name=""></param>
                    v.sqlite3VdbeAddOp4(OpCode.OP_Affinity, r1, 1, 0, affinity, (P4Usage)1);
					///
					///<summary>
					///If the set membership test fails, then the result of the 
					///"x IN (...)" expression must be either 0 or NULL. If the set
					///contains no NULL values, then the result is 0. If the set 
					///contains one or more NULL values, then the result of the
					///expression is also NULL.
					///
					///</summary>
					if(rRhsHasNull==0||destIfFalse==destIfNull) {
						///
						///<summary>
						///This branch runs if it is known at compile time that the RHS
						///cannot contain NULL values. This happens as the result
						///of a "NOT NULL" constraint in the database schema.
						///
						///Also run this branch if NULL is equivalent to FALSE
						///for this particular IN operator.
						///
						///</summary>
                        v.sqlite3VdbeAddOp4Int(OpCode.OP_NotFound, pExpr.iTable, destIfFalse, r1, 1);
					}
					else {
						///
						///<summary>
						///In this branch, the RHS of the IN might contain a NULL and
						///the presence of a NULL on the RHS makes a difference in the
						///outcome.
						///
						///</summary>
						int j1,j2,j3;
						///
						///<summary>
						///First check to see if the LHS is contained in the RHS.  If so,
						///then the presence of NULLs in the RHS does not matter, so jump
						///over all of the code that follows.
						///
						///</summary>
						j1=v.sqlite3VdbeAddOp4Int( OpCode.OP_Found,pExpr.iTable,0,r1,1);
						///
						///<summary>
						///Here we begin generating code that runs if the LHS is not
						///contained within the RHS.  Generate additional code that
						///tests the RHS for NULLs.  If the RHS contains a NULL then
						///jump to destIfNull.  If there are no NULLs in the RHS then
						///jump to destIfFalse.
						///
						///</summary>
						j2=v.sqlite3VdbeAddOp1(OpCode.OP_NotNull,rRhsHasNull);
						j3=v.sqlite3VdbeAddOp4Int( OpCode.OP_Found,pExpr.iTable,0,rRhsHasNull,1);
						v.sqlite3VdbeAddOp2(OpCode.OP_Integer,-1,rRhsHasNull);
						v.sqlite3VdbeJumpHere(j3);
						v.sqlite3VdbeAddOp2(OpCode.OP_AddImm,rRhsHasNull,1);
						v.sqlite3VdbeJumpHere(j2);
						///
						///<summary>
						///Jump to the appropriate target depending on whether or not
						///the RHS contains a NULL
						///
						///</summary>
						v.sqlite3VdbeAddOp2( OpCode.OP_If,rRhsHasNull,destIfNull);
						v.sqlite3VdbeAddOp2(OpCode.OP_Goto,0,destIfFalse);
						///
						///<summary>
						///The  OpCode.OP_Found at the top of this branch jumps here when true, 
						///causing the overall IN expression evaluation to fall through.
						///
						///</summary>
						v.sqlite3VdbeJumpHere(j1);
					}
				}
				this.deallocTempReg(r1);
				this.sqlite3ExprCachePop(1);
				v.VdbeComment("end IN expr");
			}
			
			public void sqlite3VtabBeginParse(///
			///<summary>
			///Parsing context 
			///</summary>
			Token pName1,///
			///<summary>
			///Name of new table, or database name 
			///</summary>
			Token pName2,///
			///<summary>
			///Name of new table or NULL 
			///</summary>
			Token pModuleName///
			///<summary>
			///Name of the module for the virtual table 
			///</summary>
			) {
				int iDb;
				///
				///<summary>
				///The database the table is being created in 
				///</summary>
				Table pTable;
				///
				///<summary>
				///The new virtual table 
				///</summary>
				Connection db;
				///
				///<summary>
				///Database connection 
				///</summary>
				TableBuilder.sqlite3StartTable(this,pName1,pName2,0,0,1,0);
				pTable=this.pNewTable;
				if(pTable==null)
					return;
				Debug.Assert(null==pTable.pIndex);
				db=this.db;
				iDb=db.indexOfBackendWithSchema(pTable.pSchema);
				Debug.Assert(iDb>=0);
				pTable.tabFlags|=TableFlags.TF_Virtual;
				pTable.nModuleArg=0;
                VTableMethodsExtensions.addModuleArgument(db, pTable, build.Token2Name(db, pModuleName));
                VTableMethodsExtensions.addModuleArgument(db, pTable, db.Backends[iDb].Name);
				//sqlite3DbStrDup( db, db.aDb[iDb].zName ) );
				VTableMethodsExtensions.addModuleArgument(db,pTable,pTable.zName);
				//sqlite3DbStrDup( db, pTable.zName ) );
				this.sNameToken.Length=this.sNameToken.zRestSql.Length;
				//      (int)[pModuleName.n] - pName1.z );
				#if !SQLITE_OMIT_AUTHORIZATION
																																																																																			  /* Creating a virtual table invokes the authorization callback twice.
  ** The first invocation, to obtain permission to INSERT a row into the
  ** sqlite_master table, has already been made by build.sqlite3StartTable().
  ** The second call, to obtain permission to create the table, is made now.
  */
  if( pTable->azModuleArg ){
    sqlite3AuthCheck(pParse, SQLITE_CREATE_VTABLE, pTable->zName, 
            pTable->azModuleArg[0], pParse->db->aDb[iDb].zName);
  }
#endif
			}
			public void addArgumentToVtab() {
				if(this.sArg.zRestSql!=null&&Sqlite3.ALWAYS(this.pNewTable)) {
					string z=this.sArg.zRestSql.Substring(0,this.sArg.Length);
					int n=this.sArg.Length;
					Connection db=this.db;
                    VTableMethodsExtensions.addModuleArgument(db, this.pNewTable, z);
					///sqlite3DbStrNDup( db, z, n ) );
				}
			}
			public void sqlite3VtabFinishParse(Token pEnd) {
				Table pTab=this.pNewTable;
				///The table being constructed 
				Connection db=this.db;
				///The database connection 
				if(pTab==null)
					return;
				this.addArgumentToVtab();
				this.sArg.zRestSql="";
				if(pTab.nModuleArg<1)
					return;
				///If the CREATE VIRTUAL TABLE statement is being entered for the
				///first time (in other words if the virtual table is actually being
				///created now instead of just being read out of sqlite_master) then
				///do additional initialization work and store the statement text
				///in the sqlite_master table.
				if(0==db.init.busy) {
					string zStmt;
					string zWhere;
					int iDb;
					Vdbe v;
					///Compute the complete text of the CREATE VIRTUAL TABLE statement 
					if(pEnd!=null) {
						this.sNameToken.Length=this.sNameToken.zRestSql.Length;
						//(int)( pEnd.z - pParse.sNameToken.z ) + pEnd.n;
					}
					zStmt=io.sqlite3MPrintf(db,"CREATE VIRTUAL TABLE %T",this.sNameToken.zRestSql.Substring(0,this.sNameToken.Length));
					///A slot for the record has already been allocated in the 
					///SQLITE_MASTER table.  We just need to update that slot with all
					///the information we've collected.  
					///The VM register number pParse.regRowid holds the rowid of an
					///entry in the sqlite_master table tht was created for this vtab
					///by build.sqlite3StartTable().
					iDb=db.indexOfBackendWithSchema(pTab.pSchema);
                    build.sqlite3NestedParse(this, "UPDATE %Q.%s " + "SET type='table', name=%Q, tbl_name=%Q, rootpage=0, sql=%Q " + "WHERE rowid=#%d", db.Backends[iDb].Name, sqliteinth.SCHEMA_TABLE(iDb), pTab.zName, pTab.zName, zStmt, this.regRowid);
					db.DbFree(ref zStmt);
					v=this.sqlite3GetVdbe();
					build.codegenChangeCookie(this,iDb);
					v.sqlite3VdbeAddOp2( OpCode.OP_Expire,0,0);
					zWhere=io.sqlite3MPrintf(db,"name='%q' AND type='table'",pTab.zName);
					v.codegenAddParseSchemaOp(iDb,zWhere);
                    v.sqlite3VdbeAddOp4(OpCode.OP_VCreate, iDb, 0, 0, pTab.zName, (P4Usage)(StringExtensions.Strlen30(pTab.zName) + 1));
				}
				///
				///<summary>
				///</summary>
				///<param name="If we are rereading the sqlite_master table create the in">memory</param>
				///<param name="record of the table. The xConnect() method is not called until">record of the table. The xConnect() method is not called until</param>
				///<param name="the first time the virtual table is used in an SQL statement. This">the first time the virtual table is used in an SQL statement. This</param>
				///<param name="allows a schema that contains virtual tables to be loaded before">allows a schema that contains virtual tables to be loaded before</param>
				///<param name="the required virtual table implementations are registered.  ">the required virtual table implementations are registered.  </param>
				else {
					Table pOld;
					Schema pSchema=pTab.pSchema;
					string zName=pTab.zName;
					int nName=StringExtensions.Strlen30(zName);
					Debug.Assert(sqlite3SchemaMutexHeld(db,0,pSchema));
					pOld=HashExtensions.Insert(pSchema.Tables,zName,nName,pTab);
					if(pOld!=null) {
						//db.mallocFailed = 1;
						Debug.Assert(pTab==pOld);
						///
						///<summary>
						///Malloc must have failed inside HashInsert() 
						///</summary>
						return;
					}
					this.pNewTable=null;
				}
			}
			public void sqlite3VtabArgInit() {
				this.addArgumentToVtab();
				this.sArg.zRestSql=null;
				this.sArg.Length=0;
			}
			public void sqlite3VtabArgExtend(Token p) {
				Token pArg=this.sArg;
				if(pArg.zRestSql==null) {
					pArg.zRestSql=p.zRestSql;
					pArg.Length=p.Length;
				}
				else {
					//Debug.Assert( pArg.z< p.z );
					pArg.Length+=p.Length+1;
					//(int)( p.z[p.n] - pArg.z );
				}
			}
			public SqlResult sqlite3VtabCallConnect(Table pTab) {
				Connection db=this.db;
				string zMod;
				Module pMod;
				SqlResult rc;
				Debug.Assert(pTab!=null);
				if((pTab.tabFlags&TableFlags.TF_Virtual)==0||VTableMethodsExtensions.sqlite3GetVTable(db,pTab)!=null) {
					return SqlResult.SQLITE_OK;
				}
				///Locate the required virtual table module 
				zMod=pTab.azModuleArg[0];
				pMod=db.aModule.Find(zMod,zMod.Strlen30());
				if(null==pMod) {
					string zModule=pTab.azModuleArg[0];
					utilc.sqlite3ErrorMsg(this,"no such module: %s",zModule);
					rc=SqlResult.SQLITE_ERROR;
				}
				else {
					string zErr=null;
					rc=VTableMethodsExtensions.vtabCallConstructor(db,pTab,pMod,pMod.pModule.xConnect,ref zErr);
					if(rc!=SqlResult.SQLITE_OK) {
						utilc.sqlite3ErrorMsg(this,"%s",zErr);
					}
					zErr=null;
					//sqlite3DbFree( db, zErr );
				}
				return rc;
			}
			public void sqlite3VtabMakeWritable(Table pTab) {
				Parse pToplevel=sqliteinth.sqlite3ParseToplevel(this);
				int i,n;
				//Table[] apVtabLock = null;
				Debug.Assert(pTab.IsVirtual());
				for(i=0;i<pToplevel.nVtabLock;i++) {
					if(pTab==pToplevel.apVtabLock[i])
						return;
				}
				n=pToplevel.apVtabLock==null?1:pToplevel.apVtabLock.Length+1;
				//(pToplevel.nVtabLock+1)*sizeof(pToplevel.apVtabLock[0]);
				//sqlite3_realloc( pToplevel.apVtabLock, n );
				//if ( apVtabLock != null )
				{
					Array.Resize(ref pToplevel.apVtabLock,n);
					// pToplevel.apVtabLock= apVtabLock;
					pToplevel.apVtabLock[pToplevel.nVtabLock++]=pTab;
					//else
					//{
					//  pToplevel.db.mallocFailed = 1;
					//}
				}
			}
		}
	}
}
