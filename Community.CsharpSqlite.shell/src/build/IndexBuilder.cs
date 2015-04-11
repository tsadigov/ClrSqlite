using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Community.CsharpSqlite
{
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
        public static Index sqlite3FindIndex(sqlite3 db, string zName, string zDb)
        {
            Index p = null;
            int nName = StringExtensions.sqlite3Strlen30(zName);
            ///All mutexes are required for schema access.  Make sure we hold them. 
            Debug.Assert(zDb != null || Sqlite3.sqlite3BtreeHoldsAllMutexes(db));
            for (int i = sqliteinth.OMIT_TEMPDB; i < db.nDb; i++)
            {
                int j = (i < 2) ? i ^ 1 : i;
                ///Search TEMP before MAIN 
                Schema pSchema = db.aDb[j].pSchema;
                Debug.Assert(pSchema != null);
                if (zDb != null && !zDb.Equals(db.aDb[j].zName, StringComparison.InvariantCultureIgnoreCase))
                    continue;
                Debug.Assert(Sqlite3.sqlite3SchemaMutexHeld(db, j, null));
                p = pSchema.idxHash.sqlite3HashFind(zName, nName, (Index)null);
                if (p != null)
                    break;
            }
            return p;
        }
        ///<summary>
        /// Reclaim the memory used by an index
        ///
        ///</summary>
        public static void freeIndex(this sqlite3 db, ref Index p)
        {
#if !SQLITE_OMIT_ANALYZE
            Sqlite3.sqlite3DeleteIndexSamples(db, p);
#endif
            db.sqlite3DbFree(ref p.zColAff);
            db.sqlite3DbFree(ref p);
        }
        ///<summary>
        /// For the index called zIdxName which is found in the database iDb,
        /// unlike that index from its Table then remove the index from
        /// the index hash table and free all memory structures associated
        /// with the index.
        ///
        ///</summary>
        public static void sqlite3UnlinkAndDeleteIndex(sqlite3 db, int iDb, string zIdxName)//OPCODE:OP_DropIndex
        {
            Debug.Assert(Sqlite3.sqlite3SchemaMutexHeld(db, iDb, null));
            var pHash = db.aDb[iDb].pSchema.idxHash;
            var pIndex = HashExtensions.sqlite3HashInsert(ref pHash, zIdxName, zIdxName.sqlite3Strlen30(), (Index)null);

            sqlite3UnlinkAndDeleteIndex(db, pIndex);
        }
        /// <summary>
        /// remove pIndex from linked-list
        /// starting from pIndex.pTable.pIndex
        /// moving by pNext
        /// </summary>
        public static void sqlite3UnlinkAndDeleteIndex(sqlite3 db, Index pIndex)
        {
            if (Sqlite3.ALWAYS(pIndex))
            {
                pIndex.removeFromLinkedList(ref pIndex.pTable.pIndex, idx => idx.pNext, (node, next) => node.pNext = next);
                freeIndex(db, ref pIndex);
            }
            db.flags |= SqliteFlags.SQLITE_InternChanges;
        }

    }
}
