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
    using Community.CsharpSqlite.Parsing;
    using Community.CsharpSqlite.builder;
    using Community.CsharpSqlite.Metadata;
    using Community.CsharpSqlite.tree;
    using Community.CsharpSqlite.Utils;

    public static partial class SchemaExtensions
    {
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
            Hash temp1;
            Hash temp2;
            HashElem pElem;
            Schema pSchema = p;
            temp1 = pSchema.tblHash;
            temp2 = pSchema.trigHash;
            pSchema.trigHash.sqlite3HashInit();
            pSchema.idxHash.sqlite3HashClear();
            for (pElem = temp2.sqliteHashFirst(); pElem != null; pElem = pElem.sqliteHashNext())
            {
                Trigger pTrigger = (Trigger)pElem.sqliteHashData();
                TriggerParser.sqlite3DeleteTrigger(null, ref pTrigger);
            }
            temp2.sqlite3HashClear();
            pSchema.trigHash.sqlite3HashInit();
            for (pElem = temp1.first; pElem != null; pElem = pElem.next)//sqliteHashFirst(&temp1); pElem; pElem = sqliteHashNext(pElem))
            {
                Table pTab = (Table)pElem.data;
                //sqliteHashData(pElem);
                TableBuilder.sqlite3DeleteTable(null, ref pTab);
            }
            temp1.sqlite3HashClear();
            pSchema.fkeyHash.sqlite3HashClear();
            pSchema.pSeqTab = null;
            if ((pSchema.flags & sqliteinth.DB_SchemaLoaded) != 0)
            {
                pSchema.iGeneration++;
                pSchema.flags = (u16)(pSchema.flags & (~sqliteinth.DB_SchemaLoaded));
            }
            p.Clear();
        }
        ///
        ///<summary>
        ///Find and return the schema associated with a BTree.  Create
        ///a new one if necessary.
        ///
        ///</summary>
        public static Schema sqlite3SchemaGet(this Btree pBt, Connection connection)
        {
            Schema p;
            if (pBt != null)
            {
                p = pBt.sqlite3BtreeSchema(-1, (dxFreeSchema)sqlite3SchemaClear);
                //Schema.Length, sqlite3SchemaFree);
            }
            else
            {
                p = new Schema();
                // (Schema *)sqlite3DbMallocZero(0, sizeof(Schema));
            }
            if (p == null)
            {
                ////        db.mallocFailed = 1;
            }
            else
                if (0 == p.file_format)
            {
                p.tblHash.sqlite3HashInit();
                p.idxHash.sqlite3HashInit();
                p.trigHash.sqlite3HashInit();
                p.fkeyHash.sqlite3HashInit();
                p.enc = SqliteEncoding.UTF8;
            }
            return p;
        }
    }
}
