using System;
using System.Diagnostics;
using System.Text;
using i16 = System.Int16;
using u8 = System.Byte;
using u16 = System.UInt16;
using System.Linq;

namespace Community.CsharpSqlite.Metadata
{
    using sqlite3_value = Engine.Mem;
    using sqlite3_stmt = Engine.Vdbe;

    using Community.CsharpSqlite.Parsing;
    using Community.CsharpSqlite.builder;
    using Community.CsharpSqlite.Metadata;
    using Community.CsharpSqlite.Tree;
    using Community.CsharpSqlite.Utils;
    using sqlite3_int64 = System.Int64;
    using Engine;
    using Os;
    using Ast;
    public static partial class SchemaExtensions
    {
        ///<summary>
        /// This is the callback routine for the code that initializes the
        /// database.  See sqlite3Init() below for additional information.
        /// This routine is also called from the OP_ParseSchema opcode of the VDBE.
        ///
        /// Each callback contains the following information:
        ///
        ///     argv[0] = name of thing being created
        ///     argv[1] = root page number for table or index. 0 for trigger or view.
        ///     argv[2] = SQL text for the CREATE statement.
        ///
        ///
        ///</summary>
        public static int InitTableDefinitionCallback(InitData pData, sqlite3_int64 argc, object p2, object NotUsed)
        {
            string[] argv = p2 as string[];
            
            Connection db = pData.db;
            int iDb = pData.iDb;
            Debug.Assert(argc == 3);
            sqliteinth.UNUSED_PARAMETER2(NotUsed, argc);
            Debug.Assert(db.mutex.sqlite3_mutex_held());
            db.DbClearProperty(iDb, sqliteinth.DB_Empty);

            //if ( db.mallocFailed != 0 )
            //{
            //  corruptSchema( pData, argv[0], "" );
            //  return 1;
            //}
            Debug.Assert(iDb >= 0 && iDb < db.BackendCount);
            if (argv == null)
                return 0;
            ///Might happen if EMPTY_RESULT_CALLBACKS are on 
            if (argv[1] == null)
            {
                corruptSchema(pData, argv[0], "");
            }
            else
                if (!String.IsNullOrEmpty(argv[2]))
            {
                ///Call the parser to process a CREATE TABLE, INDEX or VIEW.
                ///But because db.init.busy is set to 1, no VDBE code is generated
                ///or executed.  All the parser does is build the internal data
                ///structures that describe the table, index, or view.
                SqlResult rc;
                sqlite3_stmt pStmt = null;
#if !NDEBUG || SQLITE_COVERAGE_TEST
																																																																																																																								        //TESTONLY(int rcp);            /* Return code from sqlite3_prepare() */
        int rcp;
#endif
                Debug.Assert(db.init.IsBusy);
                db.init.iDb = iDb;
                db.init.newTnum = Converter.sqlite3Atoi(argv[1]);
                db.init.orphanTrigger = false;
                //TESTONLY(rcp = ) sqlite3_prepare(db, argv[2], -1, &pStmt, 0);
#if !NDEBUG || SQLITE_COVERAGE_TEST
																																																																																																																								        rcp = sqlite3_prepare( db, argv[2], -1, ref pStmt, 0 );
#else
                prepare.sqlite3_prepare(db, argv[2], -1, ref pStmt, 0);
#endif
                rc = db.errCode;
#if !NDEBUG || SQLITE_COVERAGE_TEST
																																																																																																																								        Debug.Assert( ( rc & 0xFF ) == ( rcp & 0xFF ) );
#endif
                db.init.iDb = 0;
                if (SqlResult.SQLITE_OK != rc)
                {
                    if (db.init.orphanTrigger )
                    {
                        Debug.Assert(iDb == 1);
                    }
                    else {
                        pData.rc = rc;
                        //if ( rc == SQLITE_NOMEM )db.mallocFailed = 1;							
                        //else 
                        if (rc != SqlResult.SQLITE_INTERRUPT && (rc & (SqlResult)0xFF) != SqlResult.SQLITE_LOCKED)
                        {
                            corruptSchema(pData, argv[0], Sqlite3.sqlite3_errmsg(db));
                        }
                    }
                }
                vdbeapi.sqlite3_finalize(pStmt);
            }
            else
                    if (argv[0] == null || argv[0] == "")
            {
                corruptSchema(pData, null, null);
            }
            else {
                ///If the SQL column is blank it means this is an index that
                ///was created to be the PRIMARY KEY or to fulfill a UNIQUE
                ///constraint for a CREATE TABLE.  The index should have already
                ///been created when we processed the CREATE TABLE.  All we have
                ///to do here is record the root page number for that index.
                var pIndex = IndexBuilder.FindByName(db, db.Backends[iDb].Name, argv[0]);
                if (pIndex == null)
                {
                    ///This can occur if there exists an index on a TEMP table which
                    ///has the same name as another index on a permanent index.  Since
                    ///the permanent table is hidden by the TEMP table, we can also
                    ///safely ignore the index on the permanent table.

                    ///Do Nothing 
                    ;
                }
                else
                    if (Converter.sqlite3GetInt32(argv[1], ref pIndex.tnum) == false)
                {
                    corruptSchema(pData, argv[0], "invalid rootpage");
                }
            }
            return 0;
        }

        ///<summary>
        /// Fill the InitData structure with an error message that indicates
        /// that the database is corrupt.
        ///
        ///</summary>
        static void corruptSchema(
            InitData pData,///Initialization context                                                  
            string zObj,///Object being parsed at the point of error                    
            string zExtra////Error information                      
       )
        {
            Connection db = pData.db;
            if (///
                ///<summary>
                ///0 == db.mallocFailed && 
                ///</summary>
            (db.flags & SqliteFlags.SQLITE_RecoveryMode) == 0)
            {
                {
                    if (zObj == null)
                    {
                        zObj = "?";
#if SQLITE_OMIT_UTF16
                        if (sqliteinth.ENC(db) != SqliteEncoding.UTF8)
                            zObj = Sqlite3.encnames[((int)sqliteinth.ENC(db))].zName;
#endif
                    }
                    malloc_cs.sqlite3SetString(ref pData.pzErrMsg, db, "malformed database schema (%s)", zObj);
                    if (!String.IsNullOrEmpty(zExtra))
                    {
                        pData.pzErrMsg = io.sqlite3MAppendf(db, pData.pzErrMsg, "%s - %s", pData.pzErrMsg, zExtra);
                    }
                }
                pData.rc =//db.mallocFailed != 0 ? SQLITE_NOMEM :
                sqliteinth.SQLITE_CORRUPT_BKPT();
            }
        }

        ///<summary>
        /// Free all resources held by the schema structure. The void* argument points
        /// at a Schema struct. This function does not call sqlite3DbFree(db, ) on the
        /// pointer itself, it just cleans up subsidiary resources (i.e. the contents
        /// of the schema hash tables).
        ///
        /// The Schema.cache_size variable is not cleared.
        ///
        ///</summary>    
		public static void sqlite3SchemaClear(this Schema p)
        {   
            Schema pSchema = p;
            pSchema.Indexes.sqlite3HashClear();

            {
                var triggers = pSchema.Triggers;
                triggers.sqlite3HashInit();
                triggers.sqliteHashFirst().linkedList().ForEach(pElem =>
                    {
                        var pTrigger = pElem.sqliteHashData() as Trigger;
                        TriggerParser.sqlite3DeleteTrigger(null, ref pTrigger);
                    }
                );
                triggers.sqlite3HashClear();
                triggers.sqlite3HashInit();
            }

            {
                var tables = pSchema.Tables;
                tables.first.linkedList().ForEach(pElem =>
                    {
                        Table pTab = pElem.data as Table;
                    //sqliteHashData(pElem);
                    TableBuilder.sqlite3DeleteTable(null, ref pTab);
                    }
                );
                tables.sqlite3HashClear();
            }
           
            
            pSchema.ForeignKeys.sqlite3HashClear();


            pSchema.pSeqTab = null;
            if ((pSchema.flags & sqliteinth.DB_SchemaLoaded) != 0)
            {
                pSchema.iGeneration++;
                pSchema.flags = (u16)(pSchema.flags & (~sqliteinth.DB_SchemaLoaded));
            }
            p.Clear();
        }
        
        ///<summary>
        ///Find and return the schema associated with a BTree.  Create
        ///a new one if necessary.
        ///</summary>
        public static Schema GetSchema(this Btree btree, Connection connection)
        {
            Schema p=(null==btree)? new Schema(): btree.sqlite3BtreeSchema(-1, (dxFreeSchema)sqlite3SchemaClear); ;
            
            if (0 == p.file_format)
            {
                p.initHashes();
                p.enc = SqliteEncoding.UTF8;
            }
            return p;
        }



        public static T FindInBackendsSchemas<T>(this Connection db, string zName, string zDb, Func<Schema, Hash<T>> prop) where T : class
        {
            T result = null;
            int nName = StringExtensions.Strlen30(zName);
            ///All mutexes are required for schema access.  Make sure we hold them. 
            Debug.Assert(zDb != null || Sqlite3.sqlite3BtreeHoldsAllMutexes(db));

            var backend = db.Backends.Take(db.BackendCount)
                    .Swap(0, 1)///Search TEMP before MAIN //        int j = (i < 2) ? i ^ 1 : i;
                    .Where(
                        b => !(zDb != null && !zDb.Equals(b.Name, StringComparison.InvariantCultureIgnoreCase))//Debug.Assert(pSchema != null);
                    )
                    .Select(b => { Debug.Assert(Sqlite3.sqlite3SchemaMutexHeld(db, b, null)); return b; })//Debug.Assert(Sqlite3.sqlite3SchemaMutexHeld(db, j, null));)
                    .FirstOrDefault(//Debug.Assert(Sqlite3.sqlite3SchemaMutexHeld(db, j, null));
                        b => null != (result = prop(b.pSchema).Find(zName, nName))
                    );

            return result;
        }
    }
}
