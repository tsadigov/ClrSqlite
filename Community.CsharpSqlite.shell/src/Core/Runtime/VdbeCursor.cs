using Community.CsharpSqlite.Metadata;
using Community.CsharpSqlite.tree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using i64 = System.Int64;
using u8 = System.Byte;
using u16 = System.UInt16;
using u32 = System.UInt32;
using u64 = System.UInt64;
using Pgno = System.UInt32;
using Community.CsharpSqlite.Utils;

namespace Community.CsharpSqlite.Engine.Core.Runtime
{
    public class VdbeCursor
    {
        public VdbeCursor()
        {
        }

        public BtCursor pCursor;

        ///
        ///<summary>
        ///The cursor structure of the backend 
        ///</summary>

        public Btree pBt;

        ///
        ///<summary>
        ///Separate file holding temporary table 
        ///</summary>

        public KeyInfo pKeyInfo;

        ///
        ///<summary>
        ///Info about index keys needed by index cursors 
        ///</summary>

        public int iDb;

        ///
        ///<summary>
        ///</summary>
        ///<param name="Index of cursor database in db">1) </param>

        public int pseudoTableReg;

        ///
        ///<summary>
        ///Register holding pseudotable content. 
        ///</summary>

        public int FieldCount;

        ///
        ///<summary>
        ///Number of fields in the header 
        ///</summary>

        public bool zeroed;

        ///
        ///<summary>
        ///True if zeroed out and ready for reuse 
        ///</summary>

        public bool rowidIsValid;

        ///
        ///<summary>
        ///True if lastRowid is valid 
        ///</summary>

        public bool atFirst;

        ///
        ///<summary>
        ///True if pointing to first entry 
        ///</summary>

        public bool useRandomRowid;

        ///
        ///<summary>
        ///</summary>
        ///<param name="Generate new record numbers semi">randomly </param>

        public bool nullRow;

        ///
        ///<summary>
        ///True if pointing to a row with no data 
        ///</summary>

        public bool deferredMoveto;

        ///
        ///<summary>
        ///A call to sqlite3BtreeMoveto() is needed 
        ///</summary>

        public bool isTable;

        ///
        ///<summary>
        ///True if a table requiring integer keys 
        ///</summary>

        public bool isIndex;

        ///
        ///<summary>
        ///</summary>
        ///<param name="True if an index containing keys only "> no data </param>

        public bool isOrdered;

        ///
        ///<summary>
        ///True if the underlying table is BTREE_UNORDERED 
        ///</summary>

#if !SQLITE_OMIT_VIRTUALTABLE
        public sqlite3_vtab_cursor pVtabCursor;

        ///
        ///<summary>
        ///The cursor for a virtual table 
        ///</summary>

        public sqlite3_module pModule;

        ///
        ///<summary>
        ///Module for cursor pVtabCursor 
        ///</summary>

#endif

        ///
        ///<summary>
        ///Sequence counter 
        ///</summary>
        public i64 seqCount;


        ///
        ///<summary>
        ///Argument to the deferred sqlite3BtreeMoveto() 
        ///</summary>
        public i64 movetoTarget;


        ///
        ///<summary>
        ///Last rowid from a Next or NextIdx operation 
        ///</summary>
        public i64 lastRowid;

        ///
        ///<summary>
        ///Result of last sqlite3BtreeMoveto() done by an OP_NotExists or
        ///OP_IsUnique opcode on this cursor. 
        ///</summary>

        public ThreeState seekResult;

        ///
        ///<summary>
        ///Cached information about the header for the data record that the
        ///cursor is currently pointing to.  Only valid if cacheStatus matches
        ///Vdbe.cacheCtr.  Vdbe.cacheCtr will never take on the value of
        ///CACHE_STALE and so setting cacheStatus=CACHE_STALE guarantees that
        ///the cache is out of date.
        ///
        ///aRow might point to (ephemeral) data for the current row, or it might
        ///be NULL.
        ///
        ///</summary>

        public u32 cacheStatus;

        ///
        ///<summary>
        ///Cache is valid if this matches Vdbe.cacheCtr 
        ///</summary>

        public Pgno payloadSize;

        ///
        ///<summary>
        ///Total number of bytes in the record 
        ///</summary>

        public u32[] aType;

        ///
        ///<summary>
        ///Type values for all entries in the record 
        ///</summary>

        public u32[] aOffset;

        ///<summary>
        ///Cached offsets to the start of each columns data
        ///</summary>
        public int aRow;

        ///
        ///<summary>
        ///Pointer to Data for the current row, if all on one page 
        ///</summary>

        public VdbeCursor Copy()
        {
            return (VdbeCursor)MemberwiseClone();
        }
    };

}
