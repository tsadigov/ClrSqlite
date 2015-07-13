using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Community.CsharpSqlite.Engine
{
    using DbPage = Paging.PgHdr;
    using sqlite3_pcache = Paging.PCache1;
    using sqlite3_stmt = Engine.Vdbe;
    using sqlite3_value = Engine.Mem;
    using codec_ctx = crypto.codec_ctx;
    using sqlite3_api_routines = Sqlite3.sqlite3_api_routines;
    using Community.CsharpSqlite.Ast;
    using Metadata;
    using Community.CsharpSqlite.Engine;
    using Community.CsharpSqlite.Paging;
    using HANDLE = System.IntPtr;
    using i16 = System.Int16;
    using sqlite3_int64 = System.Int64;
    using u32 = System.UInt32;


#if !SQLITE_OMIT_VIRTUALTABLE
    public delegate int dmxCreate(sqlite3 db, object pAux, int argc, string p4, object argv, sqlite3_vtab ppVTab, char p7);

    public delegate int dmxConnect(sqlite3 db, object pAux, int argc, string p4, object argv, sqlite3_vtab ppVTab, char p7);

    public delegate int dmxBestIndex(sqlite3_vtab pVTab, ref sqlite3_index_info pIndexInfo);

    public delegate int dmxDisconnect(sqlite3_vtab pVTab);

    public delegate int dmxDestroy(sqlite3_vtab pVTab);

    public delegate int dmxOpen(sqlite3_vtab pVTab, sqlite3_vtab_cursor ppCursor);

    public delegate int dmxClose(sqlite3_vtab_cursor pCursor);

    public delegate int dmxFilter(sqlite3_vtab_cursor pCursor, int idmxNum, string idmxStr, int argc, sqlite3_value argv);

    public delegate int dmxNext(sqlite3_vtab_cursor pCursor);

    public delegate int dmxEof(sqlite3_vtab_cursor pCursor);

    public delegate int dmxColumn(sqlite3_vtab_cursor pCursor, sqlite3_context ctx, int i3);

    public delegate int dmxRowid(sqlite3_vtab_cursor pCursor, sqlite3_int64 pRowid);

    public delegate int dmxUpdate(sqlite3_vtab pVTab, int i2, sqlite3_value sv3, sqlite3_int64 v4);

    public delegate int dmxBegin(sqlite3_vtab pVTab);

    public delegate int dmxSync(sqlite3_vtab pVTab);

    public delegate int dmxCommit(sqlite3_vtab pVTab);

    public delegate int dmxRollback(sqlite3_vtab pVTab);

    public delegate int dmxFindFunction(sqlite3_vtab pVtab, int nArg, string zName);

    public delegate int dmxRename(sqlite3_vtab pVtab, string zNew);

#endif
}
