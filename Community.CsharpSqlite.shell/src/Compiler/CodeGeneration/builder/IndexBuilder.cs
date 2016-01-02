using Community.CsharpSqlite;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using i16 = System.Int16;
using u8 = System.Byte;
using u16 = System.UInt16;
using u32 = System.UInt32;
using Pgno = System.UInt32;
using Parse = Community.CsharpSqlite.Sqlite3.Parse;
using Community.CsharpSqlite.Ast;

namespace Community.CsharpSqlite.builder
{
    using Metadata;
    using Vdbe=Engine.Vdbe;
    using Community.CsharpSqlite.Os;
    using Community.CsharpSqlite.Engine;
    using Community.CsharpSqlite.Utils;
    public static class IndexBuilder
    {

        ///<summary>
        /// Locate the in-memory structure that describes
        /// a particular index given the name of that index
        /// and the name of the database that contains the index.
        /// Return NULL if not found.
        ///
        /// If zDatabase is 0, all databases are searched for the
        /// table and the first matching index is returned.  (No checking
        /// for duplicate index names is done.)  The search order is
        /// TEMP first, then MAIN, then any auxiliary databases added
        /// using the ATTACH command.
        ///
        ///</summary>
        public static Index sqlite3FindIndex(Connection db, string zName, string zDb)
        {
            Index p = null;
            int nName = StringExtensions.Strlen30(zName);
            ///All mutexes are required for schema access.  Make sure we hold them. 
            Debug.Assert(zDb != null || Sqlite3.sqlite3BtreeHoldsAllMutexes(db));
            for (int i = sqliteinth.OMIT_TEMPDB; i < db.BackendCount; i++)
            {
                int j = (i < 2) ? i ^ 1 : i;
                ///Search TEMP before MAIN 
                Schema pSchema = db.Backends[j].pSchema;
                Debug.Assert(pSchema != null);
                if (zDb != null && !zDb.Equals(db.Backends[j].Name, StringComparison.InvariantCultureIgnoreCase))
                    continue;
                Debug.Assert(Sqlite3.sqlite3SchemaMutexHeld(db, j, null));
                p = pSchema.idxHash.Find(zName, nName, (Index)null);
                if (p != null)
                    break;
            }
            return p;
        }
        ///<summary>
        /// Reclaim the memory used by an index
        ///
        ///</summary>
        public static void freeIndex(this Connection db, ref Index p)
        {
#if !SQLITE_OMIT_ANALYZE
            Sqlite3.sqlite3DeleteIndexSamples(db, p);
#endif
            db.DbFree(ref p.zColAff);
            db.DbFree(ref p);
        }
        ///<summary>
        /// For the index called zIdxName which is found in the database iDb,
        /// unlike that index from its Table then remove the index from
        /// the index hash table and free all memory structures associated
        /// with the index.
        ///
        ///</summary>
        public static void sqlite3UnlinkAndDeleteIndex(Connection db, int iDb, string zIdxName)//OPCODE:OP_DropIndex
        {
            Debug.Assert(Sqlite3.sqlite3SchemaMutexHeld(db, iDb, null));
            var pHash = db.Backends[iDb].pSchema.idxHash;
            var pIndex = HashExtensions.sqlite3HashInsert(ref pHash, zIdxName, zIdxName.Strlen30(), (Index)null);

            sqlite3UnlinkAndDeleteIndex(db, pIndex);
        }
        /// <summary>
        /// remove pIndex from linked-list
        /// starting from pIndex.pTable.pIndex
        /// moving by pNext
        /// </summary>
        public static void sqlite3UnlinkAndDeleteIndex(Connection db, Index pIndex)
        {
            if (Sqlite3.ALWAYS(pIndex))
            {
                pIndex.removeFromLinkedList(ref pIndex.pTable.pIndex, idx => idx.pNext, (node, next) => node.pNext = next);
                freeIndex(db, ref pIndex);
            }
            db.flags |= SqliteFlags.SQLITE_InternChanges;
        }

        ///<summary>
        ///Return a dynamicly allocated KeyInfo structure that can be used
        ///with  OpCode.OP_OpenRead or  OpCode.OP_OpenWrite to access database index pIdx.
        ///
        ///If successful, a pointer to the new structure is returned. In this case
        ///the caller is responsible for calling sqlite3DbFree(db, ) on the returned
        ///pointer. If an error occurs (out of memory or missing collation
        ///sequence), NULL is returned and the state of pParse updated to reflect
        ///the error.
        ///</summary>
        public static KeyInfo sqlite3IndexKeyinfo(this Index pIdx, Community.CsharpSqlite.Sqlite3.Parse pParse )
        {
            int i;
            int nCol = pIdx.nColumn;
            //int nBytes = KeyInfo.Length + (nCol - 1) * CollSeq*.Length + nCol;
            Connection db = pParse.db;
            KeyInfo pKey = new KeyInfo();
            // (KeyInfo)sqlite3DbMallocZero(db, nBytes);
            if (pKey != null)
            {
                pKey.db = pParse.db;
                pKey.aSortOrder = new SortOrder[nCol];
                pKey.aColl = new CollSeq[nCol];
                // (u8)&(pKey.aColl[nCol]);
                //        Debug.Assert(pKey.aSortOrder[nCol] == (((u8)pKey)[nBytes]));
                for (i = 0; i < nCol; i++)
                {
                    string zColl = pIdx.azColl[i];
                    Debug.Assert(zColl != null);
                    pKey.aColl[i] = build.sqlite3LocateCollSeq(pParse, zColl);
                    pKey.aSortOrder[i] = pIdx.aSortOrder[i];
                }
                pKey.nField = (u16)nCol;
            }
            if (pParse.nErr != 0)
            {
                pKey = null;
                db.DbFree(ref pKey);
            }
            return pKey;
        }

        ///<summary>
        /// Check to see if pIndex uses the collating sequence pColl.  Return
        /// true if it does and false if it does not.
        ///
        ///</summary>
#if !SQLITE_OMIT_REINDEX
        static bool collationMatch(string zColl, Index pIndex)
        {
            int i;
            Debug.Assert(zColl != null);
            for (i = 0; i < pIndex.nColumn; i++)
            {
                string z = pIndex.azColl[i];
                Debug.Assert(z != null);
                if (z.Equals(zColl, StringComparison.InvariantCultureIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }
#endif



        ///<summary>
        /// Generate code for the REINDEX command.
        ///
        ///        REINDEX                            -- 1
        ///        REINDEX  <collation>               -- 2
        ///        REINDEX  ?<database>.?<tablename>  -- 3
        ///        REINDEX  ?<database>.?<indexname>  -- 4
        ///
        /// Form 1 causes all indices in all attached databases to be rebuilt.
        /// Form 2 rebuilds all indices in all databases that use the named
        /// collating function.  Forms 3 and 4 rebuild the named index or all
        /// indices associated with the named table.
        ///</summary>
#if !SQLITE_OMIT_REINDEX
        // OVERLOADS, so I don't need to rewrite parse.c
        public static void sqlite3Reindex(this Community.CsharpSqlite.Sqlite3.Parse pParse, int null_2, int null_3)
        {
            sqlite3Reindex(pParse, null, null);
        }
        public static void sqlite3Reindex(this Community.CsharpSqlite.Sqlite3.Parse pParse, Token pName1, Token pName2)
        {
            CollSeq pColl;
            ///Collating sequence to be reindexed, or NULL 
            string z;
            ///Name of a table or index 
            string zDb;
            ///Name of the database 
            Table pTab;
            ///A table in the database 
            Index pIndex;
            ///An index associated with pTab 
            int iDb;
            ///The database index number 
            Connection db = pParse.db;
            ///The database connection 
            Token pObjName = new Token();
            ///Name of the table or index to be reindexed 
            ///
            ///Read the database schema. If an error occurs, leave an error message
            ///and code in pParse and return NULL. 
            if (SqlResult.SQLITE_OK != pParse.sqlite3ReadSchema())
            {
                return;
            }
            if (pName1 == null)
            {
                reindexDatabases(pParse, null);
                return;
            }
            else
                if (Sqlite3.NEVER(pName2 == null) || pName2.zRestSql == null || pName2.zRestSql.Length == 0)
                {
                    string zColl;
                    Debug.Assert(pName1.zRestSql != null);
                    zColl = build.sqlite3NameFromToken(pParse.db, pName1);
                    if (zColl == null)
                        return;
                    pColl = db.sqlite3FindCollSeq(sqliteinth.ENC(db), zColl, 0);
                    if (pColl != null)
                    {
                        reindexDatabases(pParse, zColl);
                        db.DbFree(ref zColl);
                        return;
                    }
                    db.DbFree(ref zColl);
                }
            iDb = build.sqlite3TwoPartName(pParse, pName1, pName2, ref pObjName);
            if (iDb < 0)
                return;
            z = build.sqlite3NameFromToken(db, pObjName);
            if (z == null)
                return;
            zDb = db.Backends[iDb].Name;
            pTab = TableBuilder.sqlite3FindTable(db, zDb, z);
            if (pTab != null)
            {
                reindexTable(pParse, pTab, null);
                db.DbFree(ref z);
                return;
            }
            pIndex = IndexBuilder.sqlite3FindIndex(db, z, zDb);
            db.DbFree(ref z);
            if (pIndex != null)
            {
                Community.CsharpSqlite.build.sqlite3BeginWriteOperation(pParse, 0, iDb);
                //Community.CsharpSqlite.build.
                pParse.sqlite3RefillIndex(pIndex, -1);
                return;
            }
            utilc.sqlite3ErrorMsg(pParse, "unable to identify the object to be reindexed");
        }
#endif
        ///<summary>
        /// Recompute all indices of pTab that use the collating sequence pColl.
        /// If pColl == null then recompute all indices of pTab.
        ///</summary>
#if !SQLITE_OMIT_REINDEX
        static void reindexTable(Community.CsharpSqlite.Sqlite3.Parse pParse, Table pTab, string zColl)
        {
            ///An index associated with pTab 
            foreach (var pIndex in pTab.pIndex.path(pi => pi.pNext))
            {
                if (zColl == null || collationMatch(zColl, pIndex))
                {
                    int iDb = pParse.db.indexOf( pTab.pSchema);
                    Community.CsharpSqlite.build.sqlite3BeginWriteOperation(pParse, 0, iDb);
                    pParse.sqlite3RefillIndex(pIndex, -1);
                }
            }
        }
#endif
        ///<summary>
        /// Recompute all indices of all tables in all databases where the
        /// indices use the collating sequence pColl.  If pColl == null then recompute
        /// all indices everywhere.
        ///</summary>
#if !SQLITE_OMIT_REINDEX
        static void reindexDatabases(this Community.CsharpSqlite.Sqlite3.Parse pParse, string zColl)
        {
            Connection db = pParse.db;///The database connection 
            Table pTab;///A table in the database 
            Debug.Assert(Sqlite3.sqlite3BtreeHoldsAllMutexes(db));
            ///Needed for schema access 
            foreach (var pDb in db.Backends.Take(db.BackendCount))//, pDb++ )
            {
                Debug.Assert(pDb != null);
                //HashElem k;///For looping over tables in pDb 
                pDb.pSchema.tblHash.first
                    .path(h => h.next)
                    .ForEach(k=> reindexTable(pParse, (Table)k.data, zColl));
                //for ( k = sqliteHashFirst( pDb.pSchema.tblHash ) ; k != null ; k = sqliteHashNext( k ) )
            }
        }
#endif

        ///
        ///<summary>
        ///Generate code that will erase and refill index pIdx.  This is
        ///used to initialize a newly created index or to recompute the
        ///content of an index in response to a REINDEX command.
        ///
        ///if memRootPage is not negative, it means that the index is newly
        ///created.  The register specified by memRootPage contains the
        ///root page number of the index.  If memRootPage is negative, then
        ///the index already exists and must be cleared before being refilled and
        ///the root page number of the index is taken from pIndex.tnum.
        ///
        ///</summary>
        public static void sqlite3RefillIndex(this Parse pParse, Index pIndex, int memRootPage)
        {
            Table pTab = pIndex.pTable;
            ///The table that is indexed 
            int iTab = pParse.nTab++;
            ///Btree cursor used for pTab 
            int iIdx = pParse.nTab++;
            ///Btree cursor used for pIndex 
            int addr1;
            ///Address of top of loop 
            int tnum;
            ///Root page of index 
            Vdbe v;
            ///Generate code into this virtual machine 
            KeyInfo pKey;
            ///KeyInfo for index 
            int regIdxKey;
            ///Registers containing the index key 
            int regRecord;
            ///Register holding assemblied index record 
            Connection db = pParse.db;
            ///The database connection 
            int iDb = db.indexOf( pIndex.pSchema);
#if !SQLITE_OMIT_AUTHORIZATION
																																																																																	if( sqlite3AuthCheck(pParse, SQLITE_REINDEX, pIndex.zName, 0,
db.aDb[iDb].zName ) ){
return;
}
#endif
            ///<param name="Require a write">lock on the table to perform this operation </param>
            sqliteinth.sqlite3TableLock(pParse, iDb, pTab.tnum, 1, pTab.zName);
            v = pParse.sqlite3GetVdbe();
            if (v == null)
                return;
            if (memRootPage >= 0)
            {
                tnum = memRootPage;
            }
            else
            {
                tnum = pIndex.tnum;
                v.sqlite3VdbeAddOp2(OpCode.OP_Clear, tnum, iDb);
            }
            pKey = pIndex.sqlite3IndexKeyinfo(pParse);
            v.sqlite3VdbeAddOp4(OpCode.OP_OpenWrite, iIdx, tnum, iDb, pKey, P4Usage.P4_KEYINFO_HANDOFF);
            if (memRootPage >= 0)
            {
                v.sqlite3VdbeChangeP5(1);
            }
            pParse.sqlite3OpenTable(iTab, iDb, pTab, OpCode.OP_OpenRead);
            addr1 = v.sqlite3VdbeAddOp2(OpCode.OP_Rewind, iTab, 0);
            regRecord = pParse.sqlite3GetTempReg();
            regIdxKey = pParse.sqlite3GenerateIndexKey(pIndex, iTab, regRecord, true);
            if (pIndex.onError != OnConstraintError.OE_None)
            {
                int regRowid = regIdxKey + pIndex.nColumn;
                int j2 = v.sqlite3VdbeCurrentAddr() + 2;
                int pRegKey = regIdxKey;
                // SQLITE_INT_TO_PTR( regIdxKey );
                ///
                ///The registers accessed by the  OpCode.OP_IsUnique opcode were allocated
                ///using sqlite3GetTempRange() inside of the sqlite3GenerateIndexKey()
                ///call above. Just before that function was freed they were released
                ///(made available to the compiler for reuse) using
                ///sqlite3ReleaseTempRange(). So in some ways having the  OpCode.OP_IsUnique
                ///opcode use the values stored within seems dangerous. However, since
                ///we can be sure that no other temp registers have been allocated
                ///since sqlite3ReleaseTempRange() was called, it is safe to do so.
                v.sqlite3VdbeAddOp4(OpCode.OP_IsUnique, iIdx, j2, regRowid, pRegKey, P4Usage.P4_INT32);
                build.sqlite3HaltConstraint(pParse, OnConstraintError.OE_Abort, "indexed columns are not unique", P4Usage.P4_STATIC);
            }
            v.sqlite3VdbeAddOp2(OpCode.OP_IdxInsert, iIdx, regRecord);
            v.sqlite3VdbeChangeP5(OpFlag.OPFLAG_USESEEKRESULT);
            pParse.sqlite3ReleaseTempReg(regRecord);
            v.sqlite3VdbeAddOp2(OpCode.OP_Next, iTab, addr1 + 1);
            v.sqlite3VdbeJumpHere(addr1);
            v.sqlite3VdbeAddOp1(OpCode.OP_Close, iTab);
            v.sqlite3VdbeAddOp1(OpCode.OP_Close, iIdx);
        }



        ///<summary>
        /// Create a new index for an SQL table.  pName1.pName2 is the name of the index
        /// and pTblList is the name of the table that is to be indexed.  Both will
        /// be NULL for a primary key or an index that is created to satisfy a
        /// UNIQUE constraint.  If pTable and pIndex are NULL, use pParse.pNewTable
        /// as the table to be indexed.  pParse.pNewTable is a table that is
        /// currently being constructed by a CREATE TABLE statement.
        ///
        /// pList is a list of columns to be indexed.  pList will be NULL if this
        /// is a primary key or unique-constraint on the most recent column added
        /// to the table currently under construction.
        ///
        /// If the index is created successfully, return a pointer to the new Index
        /// structure. This is used by build.sqlite3AddPrimaryKey() to mark the index
        /// as the tables primary key (Index.autoIndex==2).
        ///
        ///</summary>
        // OVERLOADS, so I don't need to rewrite parse.c
        public static Index sqlite3CreateIndex(this Parse pParse, int null_2, int null_3, int null_4, int null_5, OnConstraintError onError, int null_7, int null_8, SortOrder sortOrder, int ifNotExist)
        {
            return pParse.sqlite3CreateIndex(null, null, null, null, onError, null, null, sortOrder, ifNotExist);
        }
        public static Index sqlite3CreateIndex(this Parse pParse, int null_2, int null_3, int null_4, ExprList pList, OnConstraintError onError, int null_7, int null_8, SortOrder sortOrder, int ifNotExist)
        {
            return sqlite3CreateIndex(pParse, null, null, null, pList, onError, null, null, sortOrder, ifNotExist);
        }
        public static Index sqlite3CreateIndex(this Parse pParse,///
            ///All information about this Parse 
        Token pName1,///
            ///First part of index name. May be NULL 
        Token pName2,///
            ///Second part of index name. May be NULL 
        SrcList pTblName,///
            ///Table to index. Use pParse.pNewTable if 0 
        ExprList pList,///
            ///A list of columns to be indexed 
        OnConstraintError onError,///
            ///OE_Abort, OnConstraintError.OE_Ignore, OnConstraintError.OE_Replace, or OnConstraintError.OE_None 
        Token pStart,///
            ///The CREATE token that begins this statement 
            ///</summary>
        Token pEnd,///
            ///The ")" that closes the CREATE INDEX statement 
        SortOrder sortOrder,///
            ///Sort order of primary key when pList==NULL 
        int ifNotExist///
            ///Omit error if index already exists 
        )
        {
            Index pRet = null;
            ///Pointer to return 
            Table pTab = null;
            ///Table to be indexed 
            Index pIndex = null;
            ///The index to be created 
            string zName = null;
            ///Name of the index 
            int nName;
            ///Number of characters in zName 
            int i, j;
            Token nullId = new Token();
            ///Fake token for an empty ID list 
            DbFixer sFix = new DbFixer();
            ///For assigning database names to pTable 
            SortOrder sortOrderMask;
            ///1 to honor DESC in index.  0 to ignore. 
            Connection db = pParse.db;
            DbBackend pDb;
            ///The specific table containing the indexed database 
            int iDb;
            ///Index of the database that is being written 
            Token pName = null;
            ///Unqualified name of the index to create 
            ExprList_item pListItem;
            ///For looping over pList 
            int nCol;
            int nExtra = 0;
            StringBuilder zExtra = new StringBuilder();
            Debug.Assert(pStart == null || pEnd != null);
            ///<param name="pEnd must be non">NULL if pStart is </param>
            Debug.Assert(pParse.nErr == 0);
            ///Never called with prior errors 
            if (///
                ///db.mallocFailed != 0  || 
            sqliteinth.IN_DECLARE_VTAB(pParse))
            {
                goto exit_create_index;
            }
            if (SqlResult.SQLITE_OK != pParse.sqlite3ReadSchema())
            {
                goto exit_create_index;
            }
            ///Find the table that is to be indexed.  Return early if not found.
            if (pTblName != null)
            {
                ///<param name="Use the two">part index name to determine the database</param>
                ///<param name="to search for the table. 'Fix' the table name to this db">to search for the table. 'Fix' the table name to this db</param>
                ///<param name="before looking up the table.">before looking up the table.</param>
                ///<param name=""></param>
                Debug.Assert(pName1 != null && pName2 != null);
                iDb = build.sqlite3TwoPartName(pParse, pName1, pName2, ref pName);
                if (iDb < 0)
                    goto exit_create_index;
#if !SQLITE_OMIT_TEMPDB
                ///If the index name was unqualified, check if the the table
                ///is a temp table. If so, set the database to 1. Do not do this
                ///if initialising a database schema.
                if (0 == db.init.busy)
                {
                    pTab = pParse.sqlite3SrcListLookup(pTblName);
                    if (pName2.Length == 0 && pTab != null && pTab.pSchema == db.Backends[1].pSchema)
                    {
                        iDb = 1;
                    }
                }
#endif
                if (sFix.sqlite3FixInit(pParse, iDb, "index", pName) != 0 && sFix.sqlite3FixSrcList(pTblName) != 0)
                {
                    ///Because the parser constructs pTblName from a single identifier,
                    ///sqlite3FixSrcList can never fail. 
                    Debugger.Break();
                }
                pTab = TableBuilder.sqlite3LocateTable(pParse, 0, pTblName.a[0].zName, pTblName.a[0].zDatabase);
                if (pTab == null///
                    ///<summary>
                    ///|| db.mallocFailed != 0 
                    ///</summary>
                )
                    goto exit_create_index;
                Debug.Assert(db.Backends[iDb].pSchema == pTab.pSchema);
            }
            else
            {
                Debug.Assert(pName == null);
                pTab = pParse.pNewTable;
                if (pTab == null)
                    goto exit_create_index;
                iDb = db.indexOf( pTab.pSchema);
            }
            pDb = db.Backends[iDb];
            Debug.Assert(pTab != null);
            Debug.Assert(pParse.nErr == 0);
            if (pTab.zName.StartsWith("sqlite_", System.StringComparison.InvariantCultureIgnoreCase) && !pTab.zName.StartsWith("sqlite_altertab_", System.StringComparison.InvariantCultureIgnoreCase))
            {
                utilc.sqlite3ErrorMsg(pParse, "table %s may not be indexed", pTab.zName);
                goto exit_create_index;
            }
#if !SQLITE_OMIT_VIEW
            if (pTab.pSelect != null)
            {
                utilc.sqlite3ErrorMsg(pParse, "views may not be indexed");
                goto exit_create_index;
            }
#endif
            if (pTab.IsVirtual())
            {
                utilc.sqlite3ErrorMsg(pParse, "virtual tables may not be indexed");
                goto exit_create_index;
            }
            ///
            ///<summary>
            ///Find the name of the index.  Make sure there is not already another
            ///index or table with the same name.
            ///
            ///Exception:  If we are reading the names of permanent indices from the
            ///sqlite_master table (because some other process changed the schema) and
            ///one of the index names collides with the name of a temporary table or
            ///index, then we will continue to process this index.
            ///
            ///If pName==0 it means that we are
            ///dealing with a primary key or UNIQUE constraint.  We have to invent our
            ///own name.
            ///
            ///</summary>
            if (pName != null)
            {
                zName = build.sqlite3NameFromToken(db, pName);
                if (zName == null)
                    goto exit_create_index;
                if (SqlResult.SQLITE_OK != build.sqlite3CheckObjectName(pParse, zName))
                {
                    goto exit_create_index;
                }
                if (0 == db.init.busy)
                {
                    if (TableBuilder.sqlite3FindTable(db, null, zName) != null)
                    {
                        utilc.sqlite3ErrorMsg(pParse, "there is already a table named %s", zName);
                        goto exit_create_index;
                    }
                }
                if (IndexBuilder.sqlite3FindIndex(db, zName, pDb.Name) != null)
                {
                    if (ifNotExist == 0)
                    {
                        utilc.sqlite3ErrorMsg(pParse, "index %s already exists", zName);
                    }
                    else
                    {
                        Debug.Assert(0 == db.init.busy);
                        build.sqlite3CodeVerifySchema(pParse, iDb);
                    }
                    goto exit_create_index;
                }
            }
            else
            {
                int n = 0;
                Index pLoop;
                for (pLoop = pTab.pIndex, n = 1; pLoop != null; pLoop = pLoop.pNext, n++)
                {
                }
                zName = io.sqlite3MPrintf(db, "sqlite_autoindex_%s_%d", pTab.zName, n);
                if (zName == null)
                {
                    goto exit_create_index;
                }
            }
            ///Check for authorization to create an index.
#if !SQLITE_OMIT_AUTHORIZATION
																																																																																	{
string zDb = pDb.zName;
if( sqlite3AuthCheck(pParse, SQLITE_INSERT, SCHEMA_TABLE(iDb), 0, zDb) ){
goto exit_create_index;
}
i = SQLITE_CREATE_INDEX;
if( OMIT_TEMPDB ==0&& iDb==1 ) i = SQLITE_CREATE_TEMP_INDEX;
if( sqlite3AuthCheck(pParse, i, zName, pTab.zName, zDb) ){
goto exit_create_index;
}
}
#endif
            ///If pList==0, it means this routine was called to make a primary
            ///key out of the last column added to the table under construction.
            ///So create a fake list to simulate this.
            if (pList == null)
            {
                nullId.zRestSql = pTab.aCol[pTab.nCol - 1].zName;
                nullId.Length = StringExtensions.Strlen30(nullId.zRestSql);
                pList = CollectionExtensions.Append(null, null);
                if (pList == null)
                    goto exit_create_index;
                pParse.sqlite3ExprListSetName(pList, nullId, 0);
                pList.a[0].sortOrder = sortOrder;
            }
            ///Figure out how many bytes of space are required to store explicitly
            ///specified collation sequence names.
            for (i = 0; i < pList.Count; i++)
            {
                Expr pExpr = pList.a[i].pExpr;
                if (pExpr != null)
                {
                    CollSeq pColl = pExpr.CollatingSequence;
                    ///Either pColl!=0 or there was an OOM failure.  But if an OOM
                    ///failure we have quit before reaching this point. 
                    if (Sqlite3.ALWAYS(pColl != null))
                    {
                        nExtra += (1 + StringExtensions.Strlen30(pColl.zName));
                    }
                }
            }
            ///Allocate the index structure.
            nName = StringExtensions.Strlen30(zName);
            nCol = pList.Count;
            pIndex = new Index();
            // sqlite3DbMallocZero( db,
            //    Index.Length +              /* Index structure  */
            //    sizeof( int ) * nCol +           /* Index.aiColumn   */
            //    sizeof( int ) * ( nCol + 1 ) +       /* Index.aiRowEst   */
            //    sizeof( char* ) * nCol +        /* Index.azColl     */
            //    u8.Length * nCol +            /* Index.aSortOrder */
            //    nName + 1 +                  /* Index.zName      */
            //    nExtra                       /* Collation sequence names */
            //);
            //if ( db.mallocFailed != 0 )
            //{
            //  goto exit_create_index;
            //}
            pIndex.azColl = new string[nCol + 1];
            //(char*)(pIndex[1]);
            pIndex.aiColumn = new int[nCol + 1];
            //(int )(pIndex->azColl[nCol]);
            pIndex.aiRowEst = new int[nCol + 1];
            //(unsigned )(pIndex->aiColumn[nCol]);
            pIndex.aSortOrder = new SortOrder[nCol + 1];
            //(u8 )(pIndex->aiRowEst[nCol+1]);
            //pIndex.zName = null;// (char)( &pIndex->aSortOrder[nCol] );
            zExtra = new StringBuilder(nName + 1);
            // (char)( &pIndex.zName[nName + 1] );
            if (zName.Length == nName)
                pIndex.zName = zName;
            else
            {
                pIndex.zName = zName.Substring(0, nName);
            }
            // memcpy( pIndex.zName, zName, nName + 1 );
            pIndex.pTable = pTab;
            pIndex.nColumn = pList.Count;
            pIndex.onError = onError;
            pIndex.autoIndex = (u8)(pName == null ? 1 : 0);
            pIndex.pSchema = db.Backends[iDb].pSchema;
            Debug.Assert(Sqlite3.sqlite3SchemaMutexHeld(db, iDb, null));
            ///Check to see if we should honor DESC requests on index columns
            if (pDb.pSchema.file_format >= 4)
            {
                ///Honor DESC 
                sortOrderMask = SortOrder.SQLITE_SO_DESC;
            }
            else
            {
                ///Ignore DESC 
                sortOrderMask = SortOrder.SQLITE_SO_ASC;
            }
            ///Scan the names of the columns of the table to be indexed and
            ///load the column indices into the Index structure.  Report an error
            ///if any column is not found.
            ///
            ///TODO:  Add a test to make sure that the same column is not named
            ///more than once within the same index.  Only the first instance of
            ///the column will ever be used by the optimizer.  Note that using the
            ///same column more than once cannot be an error because that would
            ///<param name="break backwards compatibility "> it needs to be a warning.</param>
            ///<param name=""></param>
            for (i = 0; i < pList.Count; i++)
            {
                //, pListItem++){
                pListItem = pList.a[i];
                string zColName = pListItem.zName;
                Column pTabCol;
                SortOrder requestedSortOrder;
                string zColl;
                ///Collation sequence name 
                for (j = 0; j < pTab.nCol; j++)
                {
                    //, pTabCol++){
                    pTabCol = pTab.aCol[j];
                    if (zColName.Equals(pTabCol.zName, StringComparison.InvariantCultureIgnoreCase))
                        break;
                }
                if (j >= pTab.nCol)
                {
                    utilc.sqlite3ErrorMsg(pParse, "table %s has no column named %s", pTab.zName, zColName);
                    pParse.checkSchema = 1;
                    goto exit_create_index;
                }
                pIndex.aiColumn[i] = j;
                ///
                ///<summary>
                ///</summary>
                ///<param name="Justification of the Sqlite3.ALWAYS(pListItem">>pColl):  Because of</param>
                ///<param name="the way the "idxlist" non">terminal is constructed by the parser,</param>
                ///<param name="if pListItem">>pColl</param>
                ///<param name="must exist or else there must have been an OOM error.  But if there">must exist or else there must have been an OOM error.  But if there</param>
                ///<param name="was an OOM error, we would never reach this point. ">was an OOM error, we would never reach this point. </param>
                if (pListItem.pExpr != null && Sqlite3.ALWAYS(pListItem.pExpr.CollatingSequence))
                {
                    int nColl;
                    zColl = pListItem.pExpr.CollatingSequence.zName;
                    nColl = StringExtensions.Strlen30(zColl);
                    Debug.Assert(nExtra >= nColl);
                    zExtra = new StringBuilder(zColl.Substring(0, nColl));
                    // memcpy( zExtra, zColl, nColl );
                    zColl = zExtra.ToString();
                    //zExtra += nColl;
                    nExtra -= nColl;
                }
                else
                {
                    zColl = pTab.aCol[j].zColl;
                    if (zColl == null)
                    {
                        zColl = db.pDfltColl.zName;
                    }
                }
                if (0 == db.init.busy && build.sqlite3LocateCollSeq(pParse, zColl) == null)
                {
                    goto exit_create_index;
                }
                pIndex.azColl[i] = zColl;
                requestedSortOrder = ((pListItem.sortOrder.Intersects(sortOrderMask)) ? SortOrder.SQLITE_SO_DESC : SortOrder.SQLITE_SO_ASC);
                pIndex.aSortOrder[i] = requestedSortOrder;
            }
            build.sqlite3DefaultRowEst(pIndex);
            if (pTab == pParse.pNewTable)
            {
                ///This routine has been called to create an automatic index as a
                ///result of a PRIMARY KEY or UNIQUE clause on a column definition, or
                ///a PRIMARY KEY or UNIQUE clause following the column definitions.
                ///i.e. one of:
                ///
                ///CREATE TABLE t(x PRIMARY KEY, y);
                ///CREATE TABLE t(x, y, UNIQUE(x, y));
                ///
                ///Either way, check to see if the table already has such an index. If
                ///so, don't bother creating this one. This only applies to
                ///automatically created indices. Users can do as they wish with
                ///explicit indices.
                ///
                ///Two UNIQUE or PRIMARY KEY constraints are considered equivalent
                ///(and thus suppressing the second one) even if they have different
                ///sort orders.
                ///
                ///If there are different collating sequences or if the columns of
                ///the constraint occur in different orders, then the constraints are
                ///considered distinct and both result in separate indices.
                foreach (var pIdx in pTab.pIndex.path(pid => pid.pNext))
                {
                    int k;
                    Debug.Assert(pIdx.onError != OnConstraintError.OE_None);
                    Debug.Assert(pIdx.autoIndex != 0);
                    Debug.Assert(pIndex.onError != OnConstraintError.OE_None);
                    if (pIdx.nColumn != pIndex.nColumn)
                        continue;
                    for (k = 0; k < pIdx.nColumn; k++)
                    {
                        string z1;
                        string z2;
                        if (pIdx.aiColumn[k] != pIndex.aiColumn[k])
                            break;
                        z1 = pIdx.azColl[k];
                        z2 = pIndex.azColl[k];
                        if (z1 != z2 && !z1.Equals(z2, StringComparison.InvariantCultureIgnoreCase))
                            break;
                    }
                    if (k == pIdx.nColumn)
                    {
                        if (pIdx.onError != pIndex.onError)
                        {
                            ///This constraint creates the same index as a previous
                            ///constraint specified somewhere in the CREATE TABLE statement.
                            ///However the ON CONFLICT clauses are different. If both this
                            ///constraint and the previous equivalent constraint have explicit
                            ///ON CONFLICT clauses this is an error. Otherwise, use the
                            ///explicitly specified behavior for the index.
                            if (!(pIdx.onError == OnConstraintError.OE_Default || pIndex.onError == OnConstraintError.OE_Default))
                            {
                                utilc.sqlite3ErrorMsg(pParse, "conflicting ON CONFLICT clauses specified", 0);
                            }
                            if (pIdx.onError == OnConstraintError.OE_Default)
                            {
                                pIdx.onError = pIndex.onError;
                            }
                        }
                        goto exit_create_index;
                    }
                }
            }
            ///Link the new Index structure to its table and to the other
            ///</summary>
            ///<param name="in">memory database structures.</param>
            ///<param name=""></param>
            if (db.init.busy != 0)
            {
                Index p;
                Debug.Assert(Sqlite3.sqlite3SchemaMutexHeld(db, 0, pIndex.pSchema));
                p = HashExtensions.sqlite3HashInsert(ref pIndex.pSchema.idxHash, pIndex.zName, StringExtensions.Strlen30(pIndex.zName), pIndex);
                if (p != null)
                {
                    Debug.Assert(p == pIndex);
                    ///Malloc must have failed 
                    //        db.mallocFailed = 1;
                    goto exit_create_index;
                }
                db.flags |= SqliteFlags.SQLITE_InternChanges;
                if (pTblName != null)
                {
                    pIndex.tnum = db.init.newTnum;
                }
            }
            ///If the db.init.busy is 0 then create the index on disk.  This
            ///involves writing the index into the master table and filling in the
            ///index with the current table contents.
            ///
            ///The db.init.busy is 0 when the user first enters a CREATE INDEX
            ///command.  db.init.busy is 1 when a database is opened and
            ///CREATE INDEX statements are read out of the master table.  In
            ///the latter case the index already exists on disk, which is why
            ///we don't want to recreate it.
            ///
            ///If pTblName==0 it means this index is generated as a primary key
            ///or UNIQUE constraint of a CREATE TABLE statement.  Since the table
            ///has just been created, it contains no data and the index initialization
            ///step can be skipped.
            else//if ( 0 == db.init.busy )
            {
                Vdbe v;
                string zStmt;
                int iMem = ++pParse.nMem;
                v = pParse.sqlite3GetVdbe();
                if (v == null)
                    goto exit_create_index;
                ///Create the rootpage for the index
                build.sqlite3BeginWriteOperation(pParse, 1, iDb);
                v.sqlite3VdbeAddOp2(OpCode.OP_CreateIndex, iDb, iMem);
                ///Gather the complete text of the CREATE INDEX statement into
                ///the zStmt variable
                if (pStart != null)
                {
                    Debug.Assert(pEnd != null);
                    ///A named index with an explicit CREATE INDEX statement 
                    zStmt = io.sqlite3MPrintf(db, "CREATE%s INDEX %.*s", onError == OnConstraintError.OE_None ? "" : " UNIQUE", (int)(pName.zRestSql.Length - pEnd.zRestSql.Length) + 1, pName.zRestSql);
                }
                else
                {
                    ///An automatic index created by a PRIMARY KEY or UNIQUE constraint 
                    ///zStmt = io.sqlite3MPrintf(""); 
                    zStmt = null;
                }
                ///Add an entry in sqlite_master for this index
                build.sqlite3NestedParse(pParse, "INSERT INTO %Q.%s VALUES('index',%Q,%Q,#%d,%Q);", db.Backends[iDb].Name, sqliteinth.SCHEMA_TABLE(iDb), pIndex.zName, pTab.zName, iMem, zStmt);
                db.DbFree(ref zStmt);
                ///Fill the index with data and reparse the schema. Code an  OpCode.OP_Expire
                ///<param name="to invalidate all pre">compiled statements.</param>
                ///<param name=""></param>
                if (pTblName != null)
                {
                    pParse.sqlite3RefillIndex(pIndex, iMem);
                    build.sqlite3ChangeCookie(pParse, iDb);
                    v.sqlite3VdbeAddParseSchemaOp(iDb, io.sqlite3MPrintf(db, "name='%q' AND type='index'", pIndex.zName));
                    v.sqlite3VdbeAddOp1(OpCode.OP_Expire, 0);
                }
            }
            ///When adding an index to the list of indices for a table, make
            ///sure all indices labeled OnConstraintError.OE_Replace come after all those labeled
            ///OE_Ignore.  This is necessary for the correct constraint check
            ///processing (in sqlite3GenerateConstraintChecks()) as part of
            ///UPDATE and INSERT statements.
            if (db.init.busy != 0 || pTblName == null)
            {
                if (onError != OnConstraintError.OE_Replace || pTab.pIndex == null || pTab.pIndex.onError == OnConstraintError.OE_Replace)
                {
                    pIndex.pNext = pTab.pIndex;
                    pTab.pIndex = pIndex;
                }
                else
                {
                    Index pOther = pTab.pIndex;
                    while (pOther.pNext != null && pOther.pNext.onError != OnConstraintError.OE_Replace)
                    {
                        pOther = pOther.pNext;
                    }
                    pIndex.pNext = pOther.pNext;
                    pOther.pNext = pIndex;
                }
                pRet = pIndex;
                pIndex = null;
            }
        ///Clean up before exiting 
        exit_create_index:
            if (pIndex != null)
            {
                //sqlite3DbFree(db, ref pIndex.zColAff );
                db.DbFree(ref pIndex);
            }
            exprc.Delete(db, ref pList);
            build.sqlite3SrcListDelete(db, ref pTblName);
            db.DbFree(ref zName);
            return pRet;
        }

            
 
    }
}
