using System;
using System.Diagnostics;
using System.Text;
using u8=System.Byte;
namespace Community.CsharpSqlite
{

    using Community.CsharpSqlite.builder;
    using Community.CsharpSqlite.Ast;
    using _Custom = Sqlite3._Custom;
    using Parse = Sqlite3.Parse;
    using Community.CsharpSqlite.Metadata;
    using Community.CsharpSqlite.Os;
    
        //#include "sqliteInt.h"
        ///<summary>
        /// Before a virtual table xCreate() or xConnect() method is invoked, the
        /// sqlite3.pVtabCtx member variable is set to point to an instance of
        /// this struct allocated on the stack. It is used by the implementation of
        /// the sqlite3_declare_vtab() and sqlite3_vtab_config() APIs, both of which
        /// are invoked only from within xCreate and xConnect methods.
        ///
        ///</summary>
        public class VtabCtx
        {
            public Table pTab;
            public VTable pVTable;
        };

        public class vtab
        {
            ///<summary>
            /// 2006 June 10
            ///
            /// The author disclaims copyright to this source code.  In place of
            /// a legal notice, here is a blessing:
            ///
            ///    May you do good and not evil.
            ///    May you find forgiveness for yourself and forgive others.
            ///    May you share freely, never taking more than you give.
            ///
            ///
            /// This file contains code used to help implement virtual tables.
            ///
            ///  Included in SQLite3 port to C#-SQLite;  2008 Noah B Hart
            ///  C#-SQLite is an independent reimplementation of the SQLite software library
            ///
            ///  SQLITE_SOURCE_ID: 2011-06-23 19:49:22 4374b7e83ea0a3fbc3691f9c0c936272862f32f2
            ///
            ///
            ///
            ///</summary>
#if !SQLITE_OMIT_VIRTUALTABLE
            

            ///<summary>
            /// The actual function that does the work of creating a new module.
            /// This function implements the sqlite3_create_module() and
            /// sqlite3_create_module_v2() interfaces.
            ///
            ///</summary>
            static SqlResult createModule(sqlite3 db,///Database in which module is registered 
                string zName,///Name assigned to this module 
                sqlite3_module pModule,///The definition of the module 
                object pAux,///Context pointer for xCreate/xConnect 
                smdxDestroy xDestroy///Module destructor function 
            )
            {
                SqlResult rc;
                Module pMod;
                db.mutex.sqlite3_mutex_enter();
                var nName = StringExtensions.sqlite3Strlen30(zName);
                pMod = new Module();
                //  (Module)sqlite3DbMallocRaw( db, sizeof( Module ) + nName + 1 );
                if (pMod != null)
                {
                    Module pDel;
                    string zCopy;
                    // = (char )(&pMod[1]);
                    zCopy = zName;
                    //memcpy(zCopy, zName, nName+1);
                    pMod.zName = zCopy;
                    pMod.pModule = pModule;
                    pMod.pAux = pAux;
                    pMod.xDestroy = xDestroy;
                    pDel = (Module)HashExtensions.sqlite3HashInsert(ref db.aModule, zCopy, nName, pMod);
                    if (pDel != null && pDel.xDestroy != null)
                    {
                        build.sqlite3ResetInternalSchema(db, -1);
                        pDel.xDestroy(ref pDel.pAux);
                    }
                    db.sqlite3DbFree(ref pDel);
                    //if( pDel==pMod ){
                    //  db.mallocFailed = 1;
                    //}
                }
                else
                    if (xDestroy != null)
                    {
                        xDestroy(ref pAux);
                    }
                rc = malloc_cs.sqlite3ApiExit(db, SqlResult.SQLITE_OK);
                db.mutex.sqlite3_mutex_leave();
                return rc;
            }
            ///<summary>
            /// External API function used to create a new virtual-table module.
            ///
            ///</summary>
            static SqlResult sqlite3_create_module(sqlite3 db,///Database in which module is registered 
                string zName,///Name assigned to this module 
                sqlite3_module pModule,///The definition of the module 
                object pAux///Context pointer for xCreate/xConnect 
            )
            {
                return createModule(db, zName, pModule, pAux, null);
            }
            ///<summary>
            /// External API function used to create a new virtual-table module.
            ///
            ///</summary>
            static SqlResult sqlite3_create_module_v2(sqlite3 db,///Database in which module is registered 
            string zName,///Name assigned to this module 
            sqlite3_module pModule,///The definition of the module 
            sqlite3_vtab pAux,///Context pointer for xCreate/xConnect 
            smdxDestroy xDestroy///Module destructor function 
            )
            {
                return createModule(db, zName, pModule, pAux, xDestroy);
            }
            ///<summary>
            /// Lock the virtual table so that it cannot be disconnected.
            /// Locks nest.  Every lock should have a corresponding unlock.
            /// If an unlock is omitted, resources leaks will occur.
            ///
            /// If a disconnect is attempted while a virtual table is locked,
            /// the disconnect is deferred until all locks have been removed.
            ///
            ///</summary>
            public static void sqlite3VtabLock(VTable pVTab)
            {
                pVTab.nRef++;
            }
            ///<summary>
            /// pTab is a pointer to a Table structure representing a virtual-table.
            /// Return a pointer to the VTable object used by connection db to access
            /// this virtual-table, if one has been created, or NULL otherwise.
            ///
            ///</summary>
            public static VTable sqlite3GetVTable(sqlite3 db, Table pTab)
            {
                VTable pVtab;
                Debug.Assert(pTab.IsVirtual());
                for (pVtab = pTab.pVTable; pVtab != null && pVtab.db != db; pVtab = pVtab.pNext)
                    ;
                return pVtab;
            }
            ///<summary>
            /// Decrement the ref-count on a virtual table object. When the ref-count
            /// reaches zero, call the xDisconnect() method to delete the object.
            ///
            ///</summary>
            public static void sqlite3VtabUnlock(VTable pVTab)
            {
                sqlite3 db = pVTab.db;
                Debug.Assert(db != null);
                Debug.Assert(pVTab.nRef > 0);
                Debug.Assert(utilc.sqlite3SafetyCheckOk(db));
                pVTab.nRef--;
                if (pVTab.nRef == 0)
                {
                    object p = pVTab.pVtab;
                    if (p != null)
                    {
                        ((sqlite3_vtab)p).pModule.xDisconnect(ref p);
                    }
                    db.sqlite3DbFree(ref pVTab);
                }
            }
            ///<summary>
            /// Table p is a virtual table. This function moves all elements in the
            /// p.pVTable list to the sqlite3.pDisconnect lists of their associated
            /// database connections to be disconnected at the next opportunity.
            /// Except, if argument db is not NULL, then the entry associated with
            /// connection db is left in the p.pVTable list.
            ///
            ///</summary>
            static VTable vtabDisconnectAll(sqlite3 db, Table p)
            {
                VTable pRet = null;
                VTable pVTable = p.pVTable;
                p.pVTable = null;
                ///
                ///<summary>
                ///Assert that the mutex (if any) associated with the BtShared database 
                ///that contains table p is held by the caller. See header comments 
                ///above function sqlite3VtabUnlockList() for an explanation of why
                ///this makes it safe to access the sqlite3.pDisconnect list of any
                ///database connection that may have an entry in the p.pVTable list.
                ///
                ///</summary>
                Debug.Assert(db == null || Sqlite3.sqlite3SchemaMutexHeld(db, 0, p.pSchema));
                while (pVTable != null)
                {
                    sqlite3 db2 = pVTable.db;
                    VTable pNext = pVTable.pNext;
                    Debug.Assert(db2 != null);
                    if (db2 == db)
                    {
                        pRet = pVTable;
                        p.pVTable = pRet;
                        pRet.pNext = null;
                    }
                    else
                    {
                        pVTable.pNext = db2.pDisconnect;
                        db2.pDisconnect = pVTable;
                    }
                    pVTable = pNext;
                }
                Debug.Assert(null == db || pRet != null);
                return pRet;
            }
            ///<summary>
            /// Disconnect all the virtual table objects in the sqlite3.pDisconnect list.
            ///
            /// This function may only be called when the mutexes associated with all
            /// shared b-tree databases opened using connection db are held by the
            /// caller. This is done to protect the sqlite3.pDisconnect list. The
            /// sqlite3.pDisconnect list is accessed only as follows:
            ///
            ///   1) By this function. In this case, all BtShared mutexes and the mutex
            ///      associated with the database handle itself must be held.
            ///
            ///   2) By function vtabDisconnectAll(), when it adds a VTable entry to
            ///      the sqlite3.pDisconnect list. In this case either the BtShared mutex
            ///      associated with the database the virtual table is stored in is held
            ///      or, if the virtual table is stored in a non-sharable database, then
            ///      the database handle mutex is held.
            ///
            /// As a result, a sqlite3.pDisconnect cannot be accessed simultaneously
            /// by multiple threads. It is thread-safe.
            ///
            ///</summary>
            public static void sqlite3VtabUnlockList(sqlite3 db)
            {
                VTable p = db.pDisconnect;
                db.pDisconnect = null;
                Debug.Assert(Sqlite3.sqlite3BtreeHoldsAllMutexes(db));
                Debug.Assert(db.mutex.sqlite3_mutex_held());
                if (p != null)
                {
                    Engine.vdbeaux.sqlite3ExpirePreparedStatements(db);
                    do
                    {
                        VTable pNext = p.pNext;
                        sqlite3VtabUnlock(p);
                        p = pNext;
                    }
                    while (p != null);
                }
            }
            ///<summary>
            /// Clear any and all virtual-table information from the Table record.
            /// This routine is called, for example, just before deleting the Table
            /// record.
            ///
            /// Since it is a virtual-table, the Table structure contains a pointer
            /// to the head of a linked list of VTable structures. Each VTable
            /// structure is associated with a single sqlite3* user of the schema.
            /// The reference count of the VTable structure associated with database
            /// connection db is decremented immediately (which may lead to the
            /// structure being xDisconnected and free). Any other VTable structures
            /// in the list are moved to the sqlite3.pDisconnect list of the associated
            /// database connection.
            ///
            ///</summary>
            public static void sqlite3VtabClear(sqlite3 db, Table p)
            {
                if (null == db || db.pnBytesFreed == 0)
                    vtabDisconnectAll(null, p);
                if (p.azModuleArg != null)
                {
                    int i;
                    for (i = 0; i < p.nModuleArg; i++)
                    {
                        db.sqlite3DbFree(ref p.azModuleArg[i]);
                    }
                    db.sqlite3DbFree(ref p.azModuleArg);
                }
            }
            ///<summary>
            /// Add a new module argument to pTable.azModuleArg[].
            /// The string is not copied - the pointer is stored.  The
            /// string will be freed automatically when the table is
            /// deleted.
            ///
            ///</summary>
            public static void addModuleArgument(sqlite3 db, Table pTable, string zArg)
            {
                int i = pTable.nModuleArg++;
                //int nBytes = sizeof(char )*(1+pTable.nModuleArg);
                //string[] azModuleArg;
                //sqlite3DbRealloc( db, pTable.azModuleArg, nBytes );
                if (pTable.azModuleArg == null || pTable.azModuleArg.Length < pTable.nModuleArg)
                    Array.Resize(ref pTable.azModuleArg, 3 + pTable.nModuleArg);
                //if ( azModuleArg == null )
                //{
                //  int j;
                //  for ( j = 0; j < i; j++ )
                //  {
                //    sqlite3DbFree( db, ref pTable.azModuleArg[j] );
                //  }
                //  sqlite3DbFree( db, ref zArg );
                //  sqlite3DbFree( db, ref pTable.azModuleArg );
                //  pTable.nModuleArg = 0;
                //}
                //else
                {
                    pTable.azModuleArg[i] = zArg;
                    //pTable.azModuleArg[i + 1] = null;
                    //azModuleArg[i+1] = 0;
                    //pTable.azModuleArg = azModuleArg;
                }
            }
            ///<summary>
            /// The parser calls this routine when it first sees a CREATE VIRTUAL TABLE
            /// statement.  The module name has been parsed, but the optional list
            /// of parameters that follow the module name are still pending.
            ///
            ///</summary>
            ///<summary>
            /// This routine takes the module argument that has been accumulating
            /// in pParse.zArg[] and appends it to the list of arguments on the
            /// virtual table currently under construction in pParse.pTable.
            ///
            ///</summary>
            ///<summary>
            /// The parser calls this routine after the CREATE VIRTUAL TABLE statement
            /// has been completely parsed.
            ///
            ///</summary>
            ///<summary>
            /// The parser calls this routine when it sees the first token
            /// of an argument to the module name in a CREATE VIRTUAL TABLE statement.
            ///
            ///</summary>
            ///<summary>
            /// The parser calls this routine for each token after the first token
            /// in an argument to the module name in a CREATE VIRTUAL TABLE statement.
            ///
            ///</summary>
            ///<summary>
            /// Invoke a virtual table constructor (either xCreate or xConnect). The
            /// pointer to the function to invoke is passed as the fourth parameter
            /// to this procedure.
            ///
            ///</summary>
            public static SqlResult vtabCallConstructor(sqlite3 db, Table pTab, Module pMod, smdxCreateConnect xConstruct, ref string pzErr)
            {
                VtabCtx sCtx = new VtabCtx();
                VTable pVTable;
                SqlResult rc;
                string[] azArg = pTab.azModuleArg;
                int nArg = pTab.nModuleArg;
                string zErr = null;
                string zModuleName = io.sqlite3MPrintf(db, "%s", pTab.zName);
                //if ( String.IsNullOrEmpty( zModuleName ) )
                //{
                //  return SQLITE_NOMEM;
                //}
                pVTable = new VTable();
                //sqlite3DbMallocZero( db, sizeof( VTable ) );
                //if ( null == pVTable )
                //{
                //  sqlite3DbFree( db, ref zModuleName );
                //  return SQLITE_NOMEM;
                //}
                pVTable.db = db;
                pVTable.pMod = pMod;
                ///Invoke the virtual table constructor 
                //assert( &db->pVtabCtx );
                Debug.Assert(xConstruct != null);
                sCtx.pTab = pTab;
                sCtx.pVTable = pVTable;
                db.pVtabCtx = sCtx;
                rc = xConstruct(db, pMod.pAux, nArg, azArg, out pVTable.pVtab, out zErr);
                db.pVtabCtx = null;
                //if ( rc == SQLITE_NOMEM )
                //  db.mallocFailed = 1;
                if (SqlResult.SQLITE_OK != rc)
                {
                    if (zErr == "")
                    {
                        pzErr = io.sqlite3MPrintf(db, "vtable constructor failed: %s", zModuleName);
                    }
                    else
                    {
                        pzErr = io.sqlite3MPrintf(db, "%s", zErr);
                        zErr = null;
                        //malloc_cs.sqlite3_free( zErr );
                    }
                    db.sqlite3DbFree(ref pVTable);
                }
                else
                    if (Sqlite3.ALWAYS(pVTable.pVtab))
                    {
                        ///Justification of Sqlite3.ALWAYS():  A correct vtab constructor must allocate
                        ///the sqlite3_vtab object if successful.  
                        pVTable.pVtab.pModule = pMod.pModule;
                        pVTable.nRef = 1;
                        if (sCtx.pTab != null)
                        {
                            string zFormat = "vtable constructor did not declare schema: %s";
                            pzErr = io.sqlite3MPrintf(db, zFormat, pTab.zName);
                            sqlite3VtabUnlock(pVTable);
                            rc = SqlResult.SQLITE_ERROR;
                        }
                        else
                        {
                            int iCol;
                            ///
                            ///<summary>
                            ///If everything went according to plan, link the new VTable structure
                            ///</summary>
                            ///<param name="into the linked list headed by pTab">>pVTable. Then loop through the </param>
                            ///<param name="columns of the table to see if any of them contain the token "hidden".">columns of the table to see if any of them contain the token "hidden".</param>
                            ///<param name="If so, set the Column.isHidden flag and remove the token from">If so, set the Column.isHidden flag and remove the token from</param>
                            ///<param name="the type string.  ">the type string.  </param>
                            pVTable.pNext = pTab.pVTable;
                            pTab.pVTable = pVTable;
                            for (iCol = 0; iCol < pTab.nCol; iCol++)
                            {
                                if (String.IsNullOrEmpty(pTab.aCol[iCol].zType))
                                    continue;
                                StringBuilder zType = new StringBuilder(pTab.aCol[iCol].zType);
                                int nType;
                                int i = 0;
                                //if ( zType )
                                //  continue;
                                nType = StringExtensions.sqlite3Strlen30(zType);
                                if (StringExtensions.sqlite3StrNICmp("hidden", 0, zType.ToString(), 6) != 0 || (zType.Length > 6 && zType[6] != ' '))
                                {
                                    for (i = 0; i < nType; i++)
                                    {
                                        if ((0 == StringExtensions.sqlite3StrNICmp(" hidden", zType.ToString().Substring(i), 7)) && (i + 7 == zType.Length || (zType[i + 7] == '\0' || zType[i + 7] == ' ')))
                                        {
                                            i++;
                                            break;
                                        }
                                    }
                                }
                                if (i < nType)
                                {
                                    int j;
                                    int nDel = 6 + (zType.Length > i + 6 ? 1 : 0);
                                    for (j = i; (j + nDel) < nType; j++)
                                    {
                                        zType[j] = zType[j + nDel];
                                    }
                                    if (zType[i] == '\0' && i > 0)
                                    {
                                        Debug.Assert(zType[i - 1] == ' ');
                                        zType.Length = i;
                                        //[i - 1] = '\0';
                                    }
                                    pTab.aCol[iCol].isHidden = 1;
                                    pTab.aCol[iCol].zType = zType.ToString().Substring(0, j);
                                }
                            }
                        }
                    }
                db.sqlite3DbFree(ref zModuleName);
                return rc;
            }
            ///<summary>
            /// This function is invoked by the parser to call the xConnect() method
            /// of the virtual table pTab. If an error occurs, an error code is returned
            /// and an error left in pParse.
            ///
            /// This call is a no-op if table pTab is not a virtual table.
            ///
            ///</summary>
            ///<summary>
            /// Grow the db.aVTrans[] array so that there is room for at least one
            /// more v-table. Return SQLITE_NOMEM if a malloc fails, or SqlResult.SQLITE_OK otherwise.
            ///
            ///</summary>
            static SqlResult growVTrans(sqlite3 db)
            {
                const int ARRAY_INCR = 5;
                ///
                ///<summary>
                ///Grow the sqlite3.aVTrans array if required 
                ///</summary>
                if ((db.nVTrans % ARRAY_INCR) == 0)
                {
                    //VTable** aVTrans;
                    //int nBytes = sizeof( sqlite3_vtab* ) * ( db.nVTrans + ARRAY_INCR );
                    //aVTrans = sqlite3DbRealloc( db, (void)db.aVTrans, nBytes );
                    //if ( !aVTrans )
                    //{
                    //  return SQLITE_NOMEM;
                    //}
                    //memset( &aVTrans[db.nVTrans], 0, sizeof( sqlite3_vtab* ) * ARRAY_INCR );
                    Array.Resize(ref db.aVTrans, db.nVTrans + ARRAY_INCR);
                }
                return SqlResult.SQLITE_OK;
            }
            ///<summary>
            /// Add the virtual table pVTab to the array sqlite3.aVTrans[]. Space should
            /// have already been reserved using growVTrans().
            ///
            ///</summary>
            static void addToVTrans(sqlite3 db, VTable pVTab)
            {
                ///
                ///<summary>
                ///Add pVtab to the end of sqlite3.aVTrans 
                ///</summary>
                db.aVTrans[db.nVTrans++] = pVTab;
                sqlite3VtabLock(pVTab);
            }
            ///<summary>
            /// This function is invoked by the vdbe to call the xCreate method
            /// of the virtual table named zTab in database iDb.
            ///
            /// If an error occurs, *pzErr is set to point an an English language
            /// description of the error and an SQLITE_XXX error code is returned.
            /// In this case the caller must call sqlite3DbFree(db, ) on *pzErr.
            ///
            ///</summary>
            public static SqlResult sqlite3VtabCallCreate(sqlite3 db, int iDb, string zTab, ref string pzErr)
            {
                var rc = SqlResult.SQLITE_OK;
                Table pTab;
                Module pMod;
                string zMod;
                pTab = TableBuilder.sqlite3FindTable(db, zTab, db.aDb[iDb].zName);
                Debug.Assert(pTab != null && (pTab.tabFlags & TableFlags.TF_Virtual) != 0 && null == pTab.pVTable);
                ///
                ///<summary>
                ///Locate the required virtual table module 
                ///</summary>
                zMod = pTab.azModuleArg[0];
                pMod = (Module)db.aModule.sqlite3HashFind(zMod, StringExtensions.sqlite3Strlen30(zMod), (Module)null);
                ///
                ///<summary>
                ///If the module has been registered and includes a Create method, 
                ///invoke it now. If the module has not been registered, return an 
                ///error. Otherwise, do nothing.
                ///
                ///</summary>
                if (null == pMod)
                {
                    pzErr = io.sqlite3MPrintf(db, "no such module: %s", zMod);
                    rc = SqlResult.SQLITE_ERROR;
                }
                else
                {
                    rc = vtabCallConstructor(db, pTab, pMod, pMod.pModule.xCreate, ref pzErr);
                }
                ///
                ///<summary>
                ///Justification of Sqlite3.ALWAYS():  The xConstructor method is required to
                ///create a valid sqlite3_vtab if it returns SqlResult.SQLITE_OK. 
                ///</summary>
                if (rc == SqlResult.SQLITE_OK && Sqlite3.ALWAYS(sqlite3GetVTable(db, pTab)))
                {
                    rc = growVTrans(db);
                    if (rc == SqlResult.SQLITE_OK)
                    {
                        addToVTrans(db, sqlite3GetVTable(db, pTab));
                    }
                }
                return rc;
            }
            ///<summary>
            /// This function is used to set the schema of a virtual table.  It is only
            /// valid to call this function from within the xCreate() or xConnect() of a
            /// virtual table module.
            ///
            ///</summary>
            static SqlResult sqlite3_declare_vtab(sqlite3 db, string zCreateTable)
            {
                Parse pParse;
                var rc = SqlResult.SQLITE_OK;
                Table pTab;
                string zErr = "";
                db.mutex.sqlite3_mutex_enter();
                if (null == db.pVtabCtx || null == (pTab = db.pVtabCtx.pTab))
                {
                    utilc.sqlite3Error(db, SqlResult.SQLITE_MISUSE, 0);
                    db.mutex.sqlite3_mutex_leave();
                    return sqliteinth.SQLITE_MISUSE_BKPT();
                }
                Debug.Assert((pTab.tabFlags & TableFlags.TF_Virtual) != 0);
                pParse = new Parse();
                //sqlite3StackAllocZero(db, sizeof(*pParse));
                //if ( pParse == null )
                //{
                //  rc = SQLITE_NOMEM;
                //}
                //else
                {
                    pParse.declareVtab = 1;
                    pParse.db = db;
                    pParse.nQueryLoop = 1;
                    if (SqlResult.SQLITE_OK == pParse.sqlite3RunParser(zCreateTable, ref zErr) && pParse.pNewTable != null//&& !db.mallocFailed
                    && null == pParse.pNewTable.pSelect && (pParse.pNewTable.tabFlags & TableFlags.TF_Virtual) == 0)
                    {
                        if (null == pTab.aCol)
                        {
                            pTab.aCol = pParse.pNewTable.aCol;
                            pTab.nCol = pParse.pNewTable.nCol;
                            pParse.pNewTable.nCol = 0;
                            pParse.pNewTable.aCol = null;
                        }
                        db.pVtabCtx.pTab = null;
                    }
                    else
                    {
                        utilc.sqlite3Error(db, SqlResult.SQLITE_ERROR, (zErr != null ? "%s" : null), zErr);
                        zErr = null;
                        //sqlite3DbFree( db, zErr );
                        rc = SqlResult.SQLITE_ERROR;
                    }
                    pParse.declareVtab = 0;
                    if (pParse.pVdbe != null)
                    {
                        Engine.vdbeaux.sqlite3VdbeFinalize(ref pParse.pVdbe);
                    }
                    TableBuilder.sqlite3DeleteTable(db, ref pParse.pNewTable);
                    //sqlite3StackFree( db, pParse );
                }
                Debug.Assert((rc & (SqlResult)0xff) == rc);
                rc = malloc_cs.sqlite3ApiExit(db, rc);
                db.mutex.sqlite3_mutex_leave();
                return rc;
            }
            ///<summary>
            /// This function is invoked by the vdbe to call the xDestroy method
            /// of the virtual table named zTab in database iDb. This occurs
            /// when a DROP TABLE is mentioned.
            ///
            /// This call is a no-op if zTab is not a virtual table.
            ///
            ///</summary>
            public static SqlResult sqlite3VtabCallDestroy(sqlite3 db, int iDb, string zTab)
            {
                var rc = SqlResult.SQLITE_OK;
                Table pTab;
                pTab = TableBuilder.sqlite3FindTable(db, zTab, db.aDb[iDb].zName);
                if (Sqlite3.ALWAYS(pTab != null && pTab.pVTable != null))
                {
                    VTable p = vtabDisconnectAll(db, pTab);
                    Debug.Assert(rc == SqlResult.SQLITE_OK);
                    object obj = p.pVtab;
                    rc = p.pMod.pModule.xDestroy(ref obj);
                    p.pVtab = null;
                    ///
                    ///<summary>
                    ///Remove the sqlite3_vtab* from the aVTrans[] array, if applicable 
                    ///</summary>
                    if (rc == SqlResult.SQLITE_OK)
                    {
                        Debug.Assert(pTab.pVTable == p && p.pNext == null);
                        p.pVtab = null;
                        pTab.pVTable = null;
                        sqlite3VtabUnlock(p);
                    }
                }
                return rc;
            }
            ///<summary>
            /// This function invokes either the xRollback or xCommit method
            /// of each of the virtual tables in the sqlite3.aVTrans array. The method
            /// called is identified by the second argument, "offset", which is
            /// the offset of the method to call in the sqlite3_module structure.
            ///
            /// The array is cleared after invoking the callbacks.
            ///
            ///</summary>
            static void callFinaliser(sqlite3 db, int offset)
            {
                int i;
                if (db.aVTrans != null)
                {
                    for (i = 0; i < db.nVTrans; i++)
                    {
                        VTable pVTab = db.aVTrans[i];
                        sqlite3_vtab p = pVTab.pVtab;
                        if (p != null)
                        {
                            //int (*x)(sqlite3_vtab );
                            //x = *(int (*)(sqlite3_vtab ))((char )p.pModule + offset);
                            //if( x ) x(p);
                            if (offset == 0)
                            {
                                if (p.pModule.xCommit != null)
                                    p.pModule.xCommit(p);
                            }
                            else
                            {
                                if (p.pModule.xRollback != null)
                                    p.pModule.xRollback(p);
                            }
                        }
                        pVTab.iSavepoint = 0;
                        sqlite3VtabUnlock(pVTab);
                    }
                    db.sqlite3DbFree(ref db.aVTrans);
                    db.nVTrans = 0;
                    db.aVTrans = null;
                }
            }
            ///<summary>
            /// Invoke the xSync method of all virtual tables in the sqlite3.aVTrans
            /// array. Return the error code for the first error that occurs, or
            /// SqlResult.SQLITE_OK if all xSync operations are successful.
            ///
            /// Set *pzErrmsg to point to a buffer that should be released using
            /// sqlite3DbFree() containing an error message, if one is available.
            ///
            ///</summary>
            public static SqlResult sqlite3VtabSync(sqlite3 db, ref string pzErrmsg)
            {
                int i;
                SqlResult rc = SqlResult.SQLITE_OK;
                VTable[] aVTrans = db.aVTrans;
                db.aVTrans = null;
                for (i = 0; rc == SqlResult.SQLITE_OK && i < db.nVTrans; i++)
                {
                    smdxFunction x;
                    //int (*x)(sqlite3_vtab );
                    sqlite3_vtab pVtab = aVTrans[i].pVtab;
                    if (pVtab != null && (x = pVtab.pModule.xSync) != null)
                    {
                        rc = x(pVtab);
                        //sqlite3DbFree(db, ref pzErrmsg);
                        pzErrmsg = pVtab.zErrMsg;
                        // sqlite3DbStrDup( db, pVtab.zErrMsg );
                        pVtab.zErrMsg = null;
                        //malloc_cs.sqlite3_free( ref pVtab.zErrMsg );
                    }
                }
                db.aVTrans = aVTrans;
                return rc;
            }
            ///<summary>
            /// Invoke the xRollback method of all virtual tables in the
            /// sqlite3.aVTrans array. Then clear the array itself.
            ///
            ///</summary>
            public static SqlResult sqlite3VtabRollback(sqlite3 db)
            {
                callFinaliser(db, 1);
                //offsetof( sqlite3_module, xRollback ) );
                return SqlResult.SQLITE_OK;
            }
            ///<summary>
            /// Invoke the xCommit method of all virtual tables in the
            /// sqlite3.aVTrans array. Then clear the array itself.
            ///
            ///</summary>
            public static SqlResult sqlite3VtabCommit(sqlite3 db)
            {
                callFinaliser(db, 0);
                //offsetof( sqlite3_module, xCommit ) );
                return SqlResult.SQLITE_OK;
            }
            ///<summary>
            /// If the virtual table pVtab supports the transaction interface
            /// (xBegin/xRollback/xCommit and optionally xSync) and a transaction is
            /// not currently open, invoke the xBegin method now.
            ///
            /// If the xBegin call is successful, place the sqlite3_vtab pointer
            /// in the sqlite3.aVTrans array.
            ///
            ///</summary>
            public static SqlResult sqlite3VtabBegin(sqlite3 db, VTable pVTab)
            {
                SqlResult rc = SqlResult.SQLITE_OK;
                sqlite3_module pModule;
                ///
                ///<summary>
                ///Special case: If db.aVTrans is NULL and db.nVTrans is greater
                ///than zero, then this function is being called from within a
                ///virtual module xSync() callback. It is illegal to write to 
                ///virtual module tables in this case, so return SQLITE_LOCKED.
                ///
                ///</summary>
                if (sqliteinth.sqlite3VtabInSync(db))
                {
                    return SqlResult.SQLITE_LOCKED;
                }
                if (null == pVTab)
                {
                    return SqlResult.SQLITE_OK;
                }
                pModule = pVTab.pVtab.pModule;
                if (pModule.xBegin != null)
                {
                    int i;
                    ///
                    ///<summary>
                    ///If pVtab is already in the aVTrans array, return early 
                    ///</summary>
                    for (i = 0; i < db.nVTrans; i++)
                    {
                        if (db.aVTrans[i] == pVTab)
                        {
                            return SqlResult.SQLITE_OK;
                        }
                    }
                    ///
                    ///<summary>
                    ///Invoke the xBegin method. If successful, add the vtab to the 
                    ///sqlite3.aVTrans[] array. 
                    ///</summary>
                    rc = growVTrans(db);
                    if (rc == SqlResult.SQLITE_OK)
                    {
                        rc = pModule.xBegin(pVTab.pVtab);
                        if (rc == SqlResult.SQLITE_OK)
                        {
                            addToVTrans(db, pVTab);
                        }
                    }
                }
                return rc;
            }
            ///<summary>
            /// Invoke either the xSavepoint, xRollbackTo or xRelease method of all
            /// virtual tables that currently have an open transaction. Pass iSavepoint
            /// as the second argument to the virtual table method invoked.
            ///
            /// If op is SAVEPOINT_BEGIN, the xSavepoint method is invoked. If it is
            /// sqliteinth.SAVEPOINT_ROLLBACK, the xRollbackTo method. Otherwise, if op is
            /// SAVEPOINT_RELEASE, then the xRelease method of each virtual table with
            /// an open transaction is invoked.
            ///
            /// If any virtual table method returns an error code other than SqlResult.SQLITE_OK,
            /// processing is abandoned and the error returned to the caller of this
            /// function immediately. If all calls to virtual table methods are successful,
            /// SqlResult.SQLITE_OK is returned.
            ///
            ///</summary>
            public static SqlResult sqlite3VtabSavepoint(sqlite3 db, int op, int iSavepoint)
            {
                SqlResult rc = SqlResult.SQLITE_OK;
                Debug.Assert(op == sqliteinth.SAVEPOINT_RELEASE || op == sqliteinth.SAVEPOINT_ROLLBACK || op == sqliteinth.SAVEPOINT_BEGIN);
                Debug.Assert(iSavepoint >= 0);
                if (db.aVTrans != null)
                {
                    int i;
                    for (i = 0; rc == SqlResult.SQLITE_OK && i < db.nVTrans; i++)
                    {
                        VTable pVTab = db.aVTrans[i];
                        sqlite3_module pMod = pVTab.pMod.pModule;
                        if (pMod.iVersion >= 2)
                        {
                            smdxFunctionArg xMethod = null;
                            //int (*xMethod)(sqlite3_vtab *, int);
                            switch (op)
                            {
                                case sqliteinth.SAVEPOINT_BEGIN:
                                    xMethod = pMod.xSavepoint;
                                    pVTab.iSavepoint = iSavepoint + 1;
                                    break;
                                case sqliteinth.SAVEPOINT_ROLLBACK:
                                    xMethod = pMod.xRollbackTo;
                                    break;
                                default:
                                    xMethod = pMod.xRelease;
                                    break;
                            }
                            if (xMethod != null && pVTab.iSavepoint > iSavepoint)
                            {
                                rc = xMethod(db.aVTrans[i].pVtab, iSavepoint);
                            }
                        }
                    }
                }
                return rc;
            }
            ///<summary>
            /// The first parameter (pDef) is a function implementation.  The
            /// second parameter (pExpr) is the first argument to this function.
            /// If pExpr is a column in a virtual table, then let the virtual
            /// table implementation have an opportunity to overload the function.
            ///
            /// This routine is used to allow virtual table implementations to
            /// overload MATCH, LIKE, GLOB, and REGEXP operators.
            ///
            /// Return either the pDef argument (indicating no change) or a
            /// new FuncDef structure that is marked as ephemeral using the
            /// SQLITE_FUNC_EPHEM flag.
            ///
            ///</summary>
            public static FuncDef sqlite3VtabOverloadFunction(sqlite3 db,///
                ///<summary>
                ///Database connection for reporting malloc problems 
                ///</summary>
            FuncDef pDef,///
                ///<summary>
                ///Function to possibly overload 
                ///</summary>
            int nArg,///
                ///<summary>
                ///Number of arguments to the function 
                ///</summary>
            Expr pExpr///
                ///<summary>
                ///First argument to the function 
                ///</summary>
            )
            {
                Table pTab;
                sqlite3_vtab pVtab;
                sqlite3_module pMod;
                dxFunc xFunc = null;
                //void (*xFunc)(sqlite3_context*,int,sqlite3_value*) = 0;
                object pArg = null;
                FuncDef pNew;
                var rc = 0;
                string zLowerName;
                string z;
                ///
                ///<summary>
                ///Check to see the left operand is a column in a virtual table 
                ///</summary>
                if (Sqlite3.NEVER(pExpr == null))
                    return pDef;
                if (pExpr.op != Sqlite3.TK_COLUMN)
                    return pDef;
                pTab = pExpr.pTab;
                if (Sqlite3.NEVER(pTab == null))
                    return pDef;
                if ((pTab.tabFlags & TableFlags.TF_Virtual) == 0)
                    return pDef;
                pVtab = sqlite3GetVTable(db, pTab).pVtab;
                Debug.Assert(pVtab != null);
                Debug.Assert(pVtab.pModule != null);
                pMod = (sqlite3_module)pVtab.pModule;
                if (pMod.xFindFunction == null)
                    return pDef;
                ///
                ///<summary>
                ///Call the xFindFunction method on the virtual table implementation
                ///to see if the implementation wants to overload this function 
                ///
                ///</summary>
                zLowerName = pDef.zName;
                //sqlite3DbStrDup(db, pDef.zName);
                if (zLowerName != null)
                {
                    //for(z=(unsigned char)zLowerName; *z; z++){
                    //  *z = _Custom.sqlite3UpperToLower[*z];
                    //}
                    rc = pMod.xFindFunction(pVtab, nArg, zLowerName.ToLowerInvariant(), ref xFunc, ref pArg);
                    db.sqlite3DbFree(ref zLowerName);
                }
                if (rc == 0)
                {
                    return pDef;
                }
                ///
                ///<summary>
                ///Create a new ephemeral function definition for the overloaded
                ///function 
                ///</summary>
                //sqlite3DbMallocZero(db, sizeof(*pNew)
                //      + StringExtensions.sqlite3Strlen30(pDef.zName) + 1);
                //if ( pNew == null )
                //{
                //  return pDef;
                //}
                pNew = pDef.Copy();
                pNew.zName = pDef.zName;
                //pNew.zName = (char )&pNew[1];
                //memcpy(pNew.zName, pDef.zName, StringExtensions.sqlite3Strlen30(pDef.zName)+1);
                pNew.xFunc = xFunc;
                pNew.pUserData = pArg;
                pNew.flags |= FuncFlags.SQLITE_FUNC_EPHEM;
                return pNew;
            }
            ///<summary>
            /// Make sure virtual table pTab is contained in the pParse.apVirtualLock[]
            /// array so that an OP_VBegin will get generated for it.  Add pTab to the
            /// array if it is missing.  If pTab is already in the array, this routine
            /// is a no-op.
            ///
            ///</summary>
            static VTabConflictPolicy[] aMap = new VTabConflictPolicy[] {
			VTabConflictPolicy.SQLITE_ROLLBACK,
			VTabConflictPolicy.SQLITE_ABORT,
			VTabConflictPolicy.SQLITE_FAIL,
			VTabConflictPolicy.SQLITE_IGNORE,
			VTabConflictPolicy.SQLITE_REPLACE
		};
            ///<summary>
            /// Return the ON CONFLICT resolution mode in effect for the virtual
            /// table update operation currently in progress.
            ///
            /// The results of this routine are undefined unless it is called from
            /// within an xUpdate method.
            ///
            ///</summary>
            static int sqlite3_vtab_on_conflict(sqlite3 db)
            {
                //static const unsigned char aMap[] = { 
                //  SQLITE_ROLLBACK, SQLITE_ABORT, SQLITE_FAIL, SQLITE_IGNORE, SQLITE_REPLACE 
                //};
                Debug.Assert(
                    (int)OnConstraintError.OE_Rollback == 1
                    && (int)OnConstraintError.OE_Abort == 2
                    && (int)OnConstraintError.OE_Fail == 3);

                Debug.Assert((int)OnConstraintError.OE_Ignore == 4
                    && (int)OnConstraintError.OE_Replace == 5);

                Debug.Assert(db.vtabOnConflict >= 1 && db.vtabOnConflict <= 5);
                return (int)aMap[db.vtabOnConflict - 1];
            }
            ///
            ///<summary>
            ///Call from within the xCreate() or xConnect() methods to provide 
            ///the SQLite core with additional information about the behavior
            ///of the virtual table being implemented.
            ///
            ///</summary>
            static SqlResult sqlite3_vtab_config(sqlite3 db, int op, params object[] ap)
            {
                // TODO ...){
                //va_list ap;
                var rc = SqlResult.SQLITE_OK;
                db.mutex.sqlite3_mutex_enter();
                _Custom.va_start(ap, "op");
                switch (op)
                {
                    case Sqlite3.SQLITE_VTAB_CONSTRAINT_SUPPORT:
                        {
                            VtabCtx p = db.pVtabCtx;
                            if (null == p)
                            {
                                rc = sqliteinth.SQLITE_MISUSE_BKPT();
                            }
                            else
                            {
                                Debug.Assert(p.pTab == null || (p.pTab.tabFlags.HasAnyProperty(TableFlags.TF_Virtual)));
                                p.pVTable.bConstraint = (Byte)_Custom.va_arg(ap, (Int32)0);
                            }
                            break;
                        }
                    default:
                        rc = sqliteinth.SQLITE_MISUSE_BKPT();
                        break;
                }
                _Custom.va_end(ref ap);
                if (rc != SqlResult.SQLITE_OK)
                    utilc.sqlite3Error(db, rc, 0);
                db.mutex.sqlite3_mutex_leave();
                return rc;
            }
#endif
        }
}