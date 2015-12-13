using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HANDLE = System.IntPtr;
using i16 = System.Int16;
using sqlite3_int64 = System.Int64;
using u32 = System.UInt32;

namespace Community.CsharpSqlite.Paging
{
    using DbPage = Cache.PgHdr;
    using sqlite3_pcache = Cache.PCache1;
    using sqlite3_stmt = Engine.Vdbe;
    using sqlite3_value = Engine.Mem;
    using codec_ctx = crypto.codec_ctx;
    using sqlite3_api_routines = Sqlite3.sqlite3_api_routines;
    using Community.CsharpSqlite.Ast;
    using Metadata;
    using Community.CsharpSqlite.Engine;
    using Community.CsharpSqlite.Paging;

    // pcache Methods
    public delegate SqlResult dxPC_Init(object NotUsed);

    public delegate void dxPC_Shutdown(object NotUsed);

    public delegate sqlite3_pcache dxPC_Create(int szPage, bool bPurgeable);

    public delegate void dxPC_Cachesize(sqlite3_pcache pCache, int nCachesize);

    public delegate int dxPC_Pagecount(sqlite3_pcache pCache);

    public delegate Cache.PgHdr dxPC_Fetch(sqlite3_pcache pCache, u32 key, int createFlag);

    public delegate void dxPC_Unpin(sqlite3_pcache pCache, Cache.PgHdr p2, bool discard);

    public delegate void dxPC_Rekey(sqlite3_pcache pCache, Cache.PgHdr p2, u32 oldKey, u32 newKey);

    public delegate void dxPC_Truncate(sqlite3_pcache pCache, u32 iLimit);

    public delegate void dxPC_Destroy(ref sqlite3_pcache pCache);

    public delegate void dxIter(Cache.PgHdr p);
}
