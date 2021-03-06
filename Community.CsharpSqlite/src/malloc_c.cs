using System.Diagnostics;
using System.Text;

namespace Community.CsharpSqlite
{
	using sqlite3_int64 = System.Int64;
	using i64 = System.Int64;
	using sqlite3_uint64 = System.UInt64;
	using u32 = System.UInt32;
	using System;
    using Community.CsharpSqlite.Os;
    using Community.CsharpSqlite.Engine;
    using Community.CsharpSqlite.Tree;
    using Community.CsharpSqlite.Utils;
   
        public class malloc_cs
        {
            ///<summary>
            /// 2001 September 15
            ///
            /// The author disclaims copyright to this source code.  In place of
            /// a legal notice, here is a blessing:
            ///
            ///    May you do good and not evil.
            ///    May you find forgiveness for yourself and forgive others.
            ///    May you share freely, never taking more than you give.
            ///
            ///
            ///
            /// Memory allocation functions used throughout sqlite.
            ///
            ///  Included in SQLite3 port to C#-SQLite;  2008 Noah B Hart
            ///  C#-SQLite is an independent reimplementation of the SQLite software library
            ///
            ///  SQLITE_SOURCE_ID: 2011-06-23 19:49:22 4374b7e83ea0a3fbc3691f9c0c936272862f32f2
            ///
            ///
            ///
            ///</summary>
            //#include "sqliteInt.h"
            //#include <stdarg.h>
            ///<summary>
            /// Attempt to release up to n bytes of non-essential memory currently
            /// held by SQLite. An example of non-essential memory is memory used to
            /// cache database pages that are not currently in use.
            ///
            ///</summary>
            static SqlResult sqlite3_release_memory(int n)
            {
#if SQLITE_ENABLE_MEMORY_MANAGEMENT
																																																												int nRet = 0;
nRet += sqlite3PcacheReleaseMemory(n-nRet);
return nRet;
#else
                sqliteinth.UNUSED_PARAMETER(n);
                return SqlResult.SQLITE_OK;
#endif
            }

            ///<summary>
            /// State information local to the memory allocation subsystem.
            ///
            ///</summary>
            //static SQLITE_WSD struct Mem0Global {
            public class Mem0Global
            {
                ///
                ///<summary>
                ///</summary>
                ///<param name="Number of free pages for scratch and page">cache memory </param>

                public int nScratchFree;

                public int nPageFree;

                public sqlite3_mutex mutex;

                ///
                ///<summary>
                ///Mutex to serialize access 
                ///</summary>

                ///
                ///<summary>
                ///The alarm callback and its arguments.  The mem0.mutex lock will
                ///be held while the callback is running.  Recursive calls into
                ///the memory subsystem are allowed, but no new callbacks will be
                ///issued.
                ///
                ///</summary>

                public sqlite3_int64 alarmThreshold;

                public dxalarmCallback alarmCallback;

                // (*alarmCallback)(void*, sqlite3_int64,int);
                public object alarmArg;

                ///
                ///<summary>
                ///Pointers to the end of sqliteinth.sqlite3GlobalConfig.pScratch and
                ///sqliteinth.sqlite3GlobalConfig.pPage to a block of memory that records
                ///which pages are available.
                ///
                ///</summary>

                //u32 *aScratchFree;
                ///<summary>
                /// True if heap is nearly "full" where "full" is defined by the
                /// sqlite3_soft_heap_limit() setting.
                ///
                ///</summary>
                public bool nearlyFull;

                public byte[][][] aByte;

                public int[] aByteSize;

                public int[] aByte_used;

                public int[][] aInt;

                public Mem[] aMem;

                public BtCursor[] aBtCursor;

                public struct memstat
                {
                    public int alloc;

                    // # of allocation requests
                    public int dealloc;

                    // # of deallocations
                    public int cached;

                    // # of cache hits
                    public int next;

                    // # Next slot to use
                    public int max;
                    // # Max slot used
                }

                public memstat msByte;

                public memstat msInt;

                public memstat msMem;

                public memstat msBtCursor;

                public Mem0Global()
                {
                }

                public Mem0Global(int nScratchFree, int nPageFree, sqlite3_mutex mutex, sqlite3_int64 alarmThreshold, dxalarmCallback alarmCallback, object alarmArg, int Byte_Allocation, int Int_Allocation, int Mem_Allocation, int BtCursor_Allocation)
                {
                    this.nScratchFree = nScratchFree;
                    this.nPageFree = nPageFree;
                    this.mutex = mutex;
                    this.alarmThreshold = alarmThreshold;
                    this.alarmCallback = alarmCallback;
                    this.alarmArg = alarmArg;
                    this.msByte.next = -1;
                    this.msInt.next = -1;
                    this.msMem.next = -1;
                    this.aByteSize = new int[] {
					32,
					256,
					1024,
					8192,
					0
				};
                    this.aByte_used = new int[] {
					-1,
					-1,
					-1,
					-1,
					-1
				};
                    this.aByte = new byte[this.aByteSize.Length][][];
                    for (int i = 0; i < this.aByteSize.Length; i++)
                        this.aByte[i] = new byte[Byte_Allocation][];
                    this.aInt = new int[Int_Allocation][];
                    this.aMem = new Mem[Mem_Allocation <= 4 ? 4 : Mem_Allocation];
                    this.aBtCursor = new BtCursor[BtCursor_Allocation <= 4 ? 4 : BtCursor_Allocation];
                    this.nearlyFull = false;
                }
            }

            //mem0 = { 0, 0, 0, 0, 0, 0, 0, 0 };
            //#define mem0 GLOBAL(struct Mem0Global, mem0)
            public static Mem0Global mem0 = new Mem0Global();

            ///<summary>
            /// This routine runs when the memory allocator sees that the
            /// total memory allocation is about to exceed the soft heap
            /// limit.
            ///
            ///</summary>
            static void softHeapLimitEnforcer(object NotUsed, sqlite3_int64 NotUsed2, int allocSize)
            {
                sqliteinth.UNUSED_PARAMETER2(NotUsed, NotUsed2);
                sqlite3_release_memory(allocSize);
            }

#if !SQLITE_OMIT_DEPRECATED
																																								///<summary>
/// Deprecated external interface.  Internal/core SQLite code
/// should call sqlite3MemoryAlarm.
///</summary>
int sqlite3_memory_alarm(
void(*xCallback)(void *pArg, sqlite3_int64 used,int N),
void *pArg,
sqlite3_int64 iThreshold
){
return sqlite3MemoryAlarm(xCallback, pArg, iThreshold);
}
#endif
            ///<summary>
            /// Set the soft heap-size limit for the library. Passing a zero or
            /// negative value indicates no limit.
            ///</summary>
            static sqlite3_int64 sqlite3_soft_heap_limit64(sqlite3_int64 n)
            {
                sqlite3_int64 priorLimit;
                sqlite3_int64 excess;
#if !SQLITE_OMIT_AUTOINIT
                Sqlite3.sqlite3_initialize();
#endif
                using (mem0.mutex.scope())
                    priorLimit = mem0.alarmThreshold;
                
                if (n < 0)
                    return priorLimit;
                if (n > 0)
                {
                    sqlite3MemoryAlarm(softHeapLimitEnforcer, 0, n);
                }
                else
                {
                    sqlite3MemoryAlarm(null, 0, 0);
                }
                excess = sqlite3_memory_used() - n;
                if (excess > 0)
                    sqlite3_release_memory((int)(excess & 0x7fffffff));
                return priorLimit;
            }

            void sqlite3_soft_heap_limit(int n)
            {
                if (n < 0)
                    n = 0;
                sqlite3_soft_heap_limit64(n);
            }

            ///<summary>
            /// Initialize the memory allocation subsystem.
            ///
            ///</summary>
            public static SqlResult sqlite3MallocInit()
            {
                if (sqliteinth.sqlite3GlobalConfig.m.xMalloc == null)
                {
                    mempoolMethods.sqlite3MemSetDefault();
                }
                mem0 = new Mem0Global(0, 0, null, 0, null, null, 1, 1, 8, 8);
                //memset(&mem0, 0, sizeof(mem0));
                if (sqliteinth.sqlite3GlobalConfig.bCoreMutex)
                {
                    mem0.mutex = Sqlite3.sqlite3MutexAlloc(Sqlite3.SQLITE_MUTEX_STATIC_MEM);
                }
                if (sqliteinth.sqlite3GlobalConfig.pScratch != null && sqliteinth.sqlite3GlobalConfig.szScratch >= 100 && sqliteinth.sqlite3GlobalConfig.nScratch >= 0)
                {
                    int i;
                    sqliteinth.sqlite3GlobalConfig.szScratch = MathExtensions.ROUNDDOWN8(sqliteinth.sqlite3GlobalConfig.szScratch - 4);
                    //mem0.aScratchFree = (u32)&((char)sqliteinth.sqlite3GlobalConfig.pScratch)
                    //  [sqliteinth.sqlite3GlobalConfig.szScratch*sqliteinth.sqlite3GlobalConfig.nScratch];
                    //for(i=0; i<sqliteinth.sqlite3GlobalConfig.nScratch; i++){ mem0.aScratchFree[i] = i; }
                    //mem0.nScratchFree = sqliteinth.sqlite3GlobalConfig.nScratch;
                }
                else
                {
                    sqliteinth.sqlite3GlobalConfig.pScratch = null;
                    sqliteinth.sqlite3GlobalConfig.szScratch = 0;
                }
                if (sqliteinth.sqlite3GlobalConfig.pPage == null || sqliteinth.sqlite3GlobalConfig.szPage < 512 || sqliteinth.sqlite3GlobalConfig.nPage < 1)
                {
                    sqliteinth.sqlite3GlobalConfig.pPage = null;
                    sqliteinth.sqlite3GlobalConfig.szPage = 0;
                    sqliteinth.sqlite3GlobalConfig.nPage = 0;
                }
                return sqliteinth.sqlite3GlobalConfig.m.xInit(sqliteinth.sqlite3GlobalConfig.m.pAppData);
            }

            ///<summary>
            /// Return true if the heap is currently under memory pressure - in other
            /// words if the amount of heap used is close to the limit set by
            /// sqlite3_soft_heap_limit().
            ///
            ///</summary>
            public static bool sqlite3HeapNearlyFull()
            {
                return mem0.nearlyFull;
            }

            ///<summary>
            /// Deinitialize the memory allocation subsystem.
            ///
            ///</summary>
            public static void sqlite3MallocEnd()
            {
                if (sqliteinth.sqlite3GlobalConfig.m.xShutdown != null)
                {
                    sqliteinth.sqlite3GlobalConfig.m.xShutdown(sqliteinth.sqlite3GlobalConfig.m.pAppData);
                }
                mem0 = new Mem0Global();
                //memset(&mem0, 0, sizeof(mem0));
            }

            ///<summary>
            /// Return the amount of memory currently checked out.
            ///
            ///</summary>
            public static sqlite3_int64 sqlite3_memory_used()
            {
                int n = 0, mx = 0;
                sqlite3_int64 res;
                Sqlite3.sqlite3_status(SqliteStatus.SQLITE_STATUS_MEMORY_USED, ref n, ref mx, 0);
                res = (sqlite3_int64)n;
                ///
                ///<summary>
                ///Work around bug in Borland C. Ticket #3216 
                ///</summary>

                return res;
            }

            ///<summary>
            /// Return the maximum amount of memory that has ever been
            /// checked out since either the beginning of this process
            /// or since the most recent reset.
            ///
            ///</summary>
            static sqlite3_int64 sqlite3_memory_highwater(int resetFlag)
            {
                int n = 0, mx = 0;
                sqlite3_int64 res;
                Sqlite3.sqlite3_status(SqliteStatus.SQLITE_STATUS_MEMORY_USED, ref n, ref mx, resetFlag);
                res = (sqlite3_int64)mx;
                ///
                ///<summary>
                ///Work around bug in Borland C. Ticket #3216 
                ///</summary>

                return res;
            }

        ///<summary>
        /// Change the alarm callback
        ///
        ///</summary>
        static SqlResult sqlite3MemoryAlarm(dxalarmCallback xCallback, //void(*xCallback)(object pArg, sqlite3_int64 used,int N),
        object pArg, sqlite3_int64 iThreshold)
        {
            int nUsed;
            using (mem0.mutex.scope())
            {
                mem0.alarmCallback = xCallback;
                mem0.alarmArg = pArg;
                mem0.alarmThreshold = iThreshold;
                nUsed = Sqlite3.sqlite3StatusValue(SqliteStatus.SQLITE_STATUS_MEMORY_USED);
                mem0.nearlyFull = (iThreshold > 0 && iThreshold <= nUsed);
            }
            return SqlResult.SQLITE_OK;
        }

            ///<summary>
            /// Trigger the alarm
            ///
            ///</summary>
            static void sqlite3MallocAlarm(int nByte)
            {
                dxalarmCallback xCallback;
                //void (*xCallback)(void*,sqlite3_int64,int);
                sqlite3_int64 nowUsed;
                object pArg;
                // void* pArg;
                if (mem0.alarmCallback == null)
                    return;
                xCallback = mem0.alarmCallback;
                nowUsed = Sqlite3.sqlite3StatusValue(SqliteStatus.SQLITE_STATUS_MEMORY_USED);
                pArg = mem0.alarmArg;
                mem0.alarmCallback = null;
                mem0.mutex.Exit();
                xCallback(pArg, nowUsed, nByte);
                mem0.mutex.Enter();
                mem0.alarmCallback = xCallback;
                mem0.alarmArg = pArg;
            }

            ///<summary>
            /// Do a memory allocation with statistics and alarms.  Assume the
            /// lock is already held.
            ///
            ///</summary>
            static int mallocWithAlarm(int n, ref int[] pp)
            {
                int nFull;
                int[] p;
                Debug.Assert(mem0.mutex.sqlite3_mutex_held());
                nFull = sqliteinth.sqlite3GlobalConfig.m.xRoundup(n);
                Sqlite3.sqlite3StatusSet(SqliteStatus.SQLITE_STATUS_MALLOC_SIZE, n);
                if (mem0.alarmCallback != null)
                {
                    int nUsed = Sqlite3.sqlite3StatusValue(SqliteStatus.SQLITE_STATUS_MEMORY_USED);
                    if (nUsed >= mem0.alarmThreshold - nFull)
                    {
                        mem0.nearlyFull = true;
                        malloc_cs.sqlite3MallocAlarm(nFull);
                    }
                    else
                    {
                        mem0.nearlyFull = false;
                    }
                }
                p = sqliteinth.sqlite3GlobalConfig.m.xMallocInt(nFull);
#if SQLITE_ENABLE_MEMORY_MANAGEMENT
																																																												if( p==null && mem0.alarmCallback!=null ){
malloc_cs.sqlite3MallocAlarm(nFull);
p = sqliteinth.sqlite3GlobalConfig.m.xMalloc(nFull);
}
#endif
                if (p != null)
                {
                    nFull = malloc_cs.sqlite3MallocSize(p);
                    Sqlite3.sqlite3StatusAdd(SqliteStatus.SQLITE_STATUS_MEMORY_USED, nFull);
                }
                pp = p;
                return nFull;
            }

            public static int mallocWithAlarm(int n, ref byte[] pp)
            {
                int nFull;
                byte[] p;
                Debug.Assert(mem0.mutex.sqlite3_mutex_held());
                nFull = sqliteinth.sqlite3GlobalConfig.m.xRoundup(n);
                Sqlite3.sqlite3StatusSet(SqliteStatus.SQLITE_STATUS_MALLOC_SIZE, n);
                if (mem0.alarmCallback != null)
                {
                    int nUsed = Sqlite3.sqlite3StatusValue(SqliteStatus.SQLITE_STATUS_MEMORY_USED);
                    if (nUsed + nFull >= mem0.alarmThreshold)
                    {
                        malloc_cs.sqlite3MallocAlarm(nFull);
                    }
                }
                p = sqliteinth.sqlite3GlobalConfig.m.xMalloc(nFull);
                if (p == null && mem0.alarmCallback != null)
                {
                    malloc_cs.sqlite3MallocAlarm(nFull);
                    p = sqliteinth.sqlite3GlobalConfig.m.xMalloc(nFull);
                }
                if (p != null)
                {
                    nFull = malloc_cs.sqlite3MallocSize(p);
                    Sqlite3.sqlite3StatusAdd(SqliteStatus.SQLITE_STATUS_MEMORY_USED, nFull);
                    Sqlite3.sqlite3StatusAdd(SqliteStatus.SQLITE_STATUS_MALLOC_COUNT, 1);
                }
                pp = p;
                return nFull;
            }

            ///<summary>
            /// Allocate memory.  This routine is like sqlite3_malloc() except that it
            /// assumes the memory subsystem has already been initialized.
            ///
            ///</summary>
            public static Mem sqlite3Malloc(Mem pMem)
            {
                return sqliteinth.sqlite3GlobalConfig.m.xMallocMem(pMem);
            }

            public static int[] sqlite3Malloc(int[] pInt, u32 n)
            {
                return malloc_cs.sqlite3Malloc(pInt, (int)n);
            }

            public static int[] sqlite3Malloc(int[] pInt, int n)
            {
                int[] p = null;
                if (n < 0 || n >= 0x7fffff00)
                {
                    ///
                    ///<summary>
                    ///A memory allocation of a number of bytes which is near the maximum
                    ///signed integer value might cause an integer overflow inside of the
                    ///xMalloc().  Hence we limit the maximum size to 0x7fffff00, giving
                    ///255 bytes of overhead.  SQLite itself will never use anything near
                    ///this amount.  The only way to reach the limit is with sqlite3_malloc() 
                    ///</summary>

                    p = null;
                }
                else
                    if (sqliteinth.sqlite3GlobalConfig.bMemstat)
                    {
                        using (mem0.mutex.scope()) 
                            mallocWithAlarm(n, ref p);
                        
                    }
                    else
                    {
                        p = sqliteinth.sqlite3GlobalConfig.m.xMallocInt(n);
                    }
                return p;
            }

            public static byte[] sqlite3Malloc(u32 n)
            {
                return malloc_cs.sqlite3Malloc((int)n);
            }

            public static byte[] sqlite3Malloc(int n)
            {
                byte[] p = null;
                if (n < 0 || n >= 0x7fffff00)
                {
                    ///
                    ///<summary>
                    ///A memory allocation of a number of bytes which is near the maximum
                    ///signed integer value might cause an integer overflow inside of the
                    ///xMalloc().  Hence we limit the maximum size to 0x7fffff00, giving
                    ///255 bytes of overhead.  SQLite itself will never use anything near
                    ///this amount.  The only way to reach the limit is with sqlite3_malloc() 
                    ///</summary>

                    p = null;
                }
                else
                    if (sqliteinth.sqlite3GlobalConfig.bMemstat)
                    {
                using (mem0.mutex.scope())
                    mallocWithAlarm(n, ref p);
                
                    }
                    else
                    {
                        p = sqliteinth.sqlite3GlobalConfig.m.xMalloc(n);
                    }
                return p;
            }

            ///
            ///<summary>
            ///This version of the memory allocation is for use by the application.
            ///First make sure the memory subsystem is initialized, then do the
            ///allocation.
            ///
            ///</summary>

            public static byte[] sqlite3_malloc(int n)
            {
#if !SQLITE_OMIT_AUTOINIT
                if (Sqlite3.sqlite3_initialize() != 0)
                    return null;
#endif
                return malloc_cs.sqlite3Malloc(n);
            }

            ///<summary>
            /// Each thread may only have a single outstanding allocation from
            /// xScratchMalloc().  We verify this constraint in the single-threaded
            /// case by setting scratchAllocOut to 1 when an allocation
            /// is outstanding clearing it when the allocation is freed.
            ///
            ///</summary>
#if SQLITE_THREADSAFE && !(NDEBUG)
																																								    static int scratchAllocOut = 0;
#endif
            ///<summary>
            /// Allocate memory that is to be used and released right away.
            /// This routine is similar to alloca() in that it is not intended
            /// for situations where the memory might be held long-term.  This
            /// routine is intended to get memory to old large transient data
            /// structures that would not normally fit on the stack of an
            /// embedded processor.
            ///</summary>
            public static byte[][] sqlite3ScratchMalloc(byte[][] apCell, int n)
            {
                apCell = sqliteinth.sqlite3GlobalConfig.pScratch2;
                if (apCell == null)
                    apCell = new byte[n < 200 ? 200 : n][];
                else
                    if (apCell.Length < n)
                        Array.Resize(ref apCell, n);
                sqliteinth.sqlite3GlobalConfig.pScratch2 = null;
                return apCell;
            }

            public static byte[] sqlite3ScratchMalloc(int n)
            {
                byte[] p = null;
                Debug.Assert(n > 0);
#if SQLITE_THREADSAFE && !(NDEBUG)
																																																												      /* Verify that no more than two scratch allocation per thread
** is outstanding at one time.  (This is only checked in the
** single-threaded case since checking in the multi-threaded case
** would be much more complicated.) */
      Debug.Assert( scratchAllocOut <= 1 );
#endif
                if (sqliteinth.sqlite3GlobalConfig.szScratch < n)
                {
                    goto scratch_overflow;
                }
                else
                {
                using (mem0.mutex.scope())
                    if (mem0.nScratchFree == 0)
                    {                        
                        goto scratch_overflow;
                    }
                    else
                    {
                        int i;
                        //i = mem0.aScratchFree[--mem0.nScratchFree];
                        //i *= sqliteinth.sqlite3GlobalConfig.szScratch;
                        for (i = 0; i < sqliteinth.sqlite3GlobalConfig.pScratch.Length; i++)
                        {
                            if (sqliteinth.sqlite3GlobalConfig.pScratch[i] == null || sqliteinth.sqlite3GlobalConfig.pScratch[i].Length < n)
                                continue;
                            p = sqliteinth.sqlite3GlobalConfig.pScratch[i];
                            // (void)&((char)sqliteinth.sqlite3GlobalConfig.pScratch)[i];
                            sqliteinth.sqlite3GlobalConfig.pScratch[i] = null;
                            break;
                        }
                        
                        if (p == null)
                            goto scratch_overflow;
                        Sqlite3.sqlite3StatusAdd(SqliteStatus.SQLITE_STATUS_SCRATCH_USED, 1);
                        Sqlite3.sqlite3StatusSet(SqliteStatus.SQLITE_STATUS_SCRATCH_SIZE, n);
                        //Debug.Assert(  (((u8)p - (u8)0) & 7)==0 );
                    }
                }
#if SQLITE_THREADSAFE && !(NDEBUG)
																																																												      scratchAllocOut = ( p != null ? 1 : 0 );
#endif
                return p;
            scratch_overflow:
                if (sqliteinth.sqlite3GlobalConfig.bMemstat)                
                using (mem0.mutex.scope())
                {
                    Sqlite3.sqlite3StatusSet(SqliteStatus.SQLITE_STATUS_SCRATCH_SIZE, n);
                    n = mallocWithAlarm(n, ref p);
                    if (p != null)
                        Sqlite3.sqlite3StatusAdd(SqliteStatus.SQLITE_STATUS_SCRATCH_OVERFLOW, n);                
                }
                else
                {
                    p = sqliteinth.sqlite3GlobalConfig.m.xMalloc(n);
                }
                sqliteinth.sqlite3MemdebugSetType(p, MemType.SCRATCH);
#if SQLITE_THREADSAFE && !(NDEBUG)
																																																												      scratchAllocOut = ( p != null ) ? 1 : 0;
#endif
                return p;
            }

            public static void sqlite3ScratchFree(byte[][] p)
            {
                if (p != null)
                {
                    if (sqliteinth.sqlite3GlobalConfig.pScratch2 == null || sqliteinth.sqlite3GlobalConfig.pScratch2.Length < p.Length)
                    {
                        Debug.Assert(sqliteinth.sqlite3MemdebugHasType(p, MemType.SCRATCH));
                        Debug.Assert(sqliteinth.sqlite3MemdebugNoType(p, ~MemType.SCRATCH));
                        sqliteinth.sqlite3MemdebugSetType(p, MemType.HEAP);
                        if (sqliteinth.sqlite3GlobalConfig.bMemstat)
                        {
                            int iSize = malloc_cs.sqlite3MallocSize(p);
                        using (mem0.mutex.scope())
                        {
                            Sqlite3.sqlite3StatusAdd(SqliteStatus.SQLITE_STATUS_SCRATCH_OVERFLOW, -iSize);
                            Sqlite3.sqlite3StatusAdd(SqliteStatus.SQLITE_STATUS_MEMORY_USED, -iSize);
                            Sqlite3.sqlite3StatusAdd(SqliteStatus.SQLITE_STATUS_MALLOC_COUNT, -1);
                            sqliteinth.sqlite3GlobalConfig.pScratch2 = p;
                            // sqliteinth.sqlite3GlobalConfig.m.xFree(ref p);
                        }
                        }
                        else
                        {
                            sqliteinth.sqlite3GlobalConfig.pScratch2 = p;
                            //sqliteinth.sqlite3GlobalConfig.m.xFree(ref p);
                        }
                    }
                    else// larger Scratch 2 already in use, let the C# GC handle
                    {
                        //int i;
                        //i = (int)((u8)p - (u8)sqliteinth.sqlite3GlobalConfig.pScratch);
                        //i /= sqliteinth.sqlite3GlobalConfig.szScratch;
                        //Debug.Assert(i >= 0 && i < sqliteinth.sqlite3GlobalConfig.nScratch);
                        //mem0.mutex.sqlite3_mutex_enter();
                        //Debug.Assert(mem0.nScratchFree < (u32)sqliteinth.sqlite3GlobalConfig.nScratch);
                        //mem0.aScratchFree[mem0.nScratchFree++] = i;
                        //sqlite3StatusAdd(SQLITE_STATUS_SCRATCH_USED, -1);
                        //mem0.mutex.sqlite3_mutex_leave();
#if SQLITE_THREADSAFE && !(NDEBUG)
																																																																																																				          /* Verify that no more than two scratch allocation per thread
** is outstanding at one time.  (This is only checked in the
** single-threaded case since checking in the multi-threaded case
** would be much more complicated.) */
          Debug.Assert( scratchAllocOut >= 1 && scratchAllocOut <= 2 );
          scratchAllocOut = 0;
#endif
                    }
                    //if( p>=sqliteinth.sqlite3GlobalConfig.pScratch && p<mem0.pScratchEnd ){
                    //  /* Release memory from the SQLITE_CONFIG_SCRATCH allocation */
                    //  ScratchFreeslot *pSlot;
                    //  pSlot = (ScratchFreeslot)p;
                    //  mem0.mutex.sqlite3_mutex_enter();
                    //  pSlot->pNext = mem0.pScratchFree;
                    //  mem0.pScratchFree = pSlot;
                    //  mem0.nScratchFree++;
                    //  Debug.Assert( mem0.nScratchFree <= (u32)sqliteinth.sqlite3GlobalConfig.nScratch );
                    //  sqlite3StatusAdd(SQLITE_STATUS_SCRATCH_USED, -1);
                    //  mem0.mutex.sqlite3_mutex_leave();
                    //}else{
                    //  /* Release memory back to the heap */
                    //  Debug.Assert( sqlite3MemdebugHasType(p, MemType.SCRATCH) );
                    //  Debug.Assert( sqliteinth.sqlite3MemdebugNoType(p, ~MemType.SCRATCH) );
                    //  sqlite3MemdebugSetType(p, MemType.HEAP);
                    //  if( sqliteinth.sqlite3GlobalConfig.bMemstat ){
                    //    int iSize = malloc_cs.sqlite3MallocSize(p);
                    //    mem0.mutex.sqlite3_mutex_enter();
                    //    sqlite3StatusAdd(SQLITE_STATUS_SCRATCH_OVERFLOW, -iSize);
                    //    sqlite3StatusAdd(Sqlite3.Sqlite3.SQLITE_STATUS_MEMORY_USED, -iSize);
                    //    sqlite3StatusAdd(SQLITE_STATUS_MALLOC_COUNT, -1);
                    //    sqliteinth.sqlite3GlobalConfig.m.xFree(p);
                    //    mem0.mutex.sqlite3_mutex_leave();
                    //  }else{
                    //    sqliteinth.sqlite3GlobalConfig.m.xFree(p);
                    //  }
                    p = null;
                }
            }

            ///<summary>
            /// TRUE if p is a lookaside memory allocation from db
            ///
            ///</summary>
#if !SQLITE_OMIT_LOOKASIDE
																																								static int isLookaside(sqlite3 db, object  *p){
return p && p>=db.lookaside.pStart && p<db.lookaside.pEnd;
}
#else
            //#define isLookaside(A,B) 0
            static bool isLookaside(Connection db, object p)
            {
                return false;
            }

#endif
            ///<summary>
            /// Return the size of a memory allocation previously obtained from
            /// malloc_cs.sqlite3Malloc() or sqlite3_malloc().
            ///</summary>
            //int malloc_cs.sqlite3MallocSize(void* p)
            //{
            //  Debug.Assert(sqlite3MemdebugHasType(p, MemType.HEAP));
            //  Debug.Assert( sqliteinth.sqlite3MemdebugNoType(p, MemType.DB) );
            //  return sqliteinth.sqlite3GlobalConfig.m.xSize(p);
            //}
            public static int sqlite3MallocSize(byte[][] p)
            {
                return p.Length * p[0].Length;
            }

            public static int sqlite3MallocSize(int[] p)
            {
                return p.Length;
            }

            public static int sqlite3MallocSize(byte[] p)
            {
                return sqliteinth.sqlite3GlobalConfig.m.xSize(p);
            }

            static int sqlite3DbMallocSize(Connection db, byte[] p)
            {
                Debug.Assert(db == null || db.mutex.sqlite3_mutex_held());
                if (db != null && isLookaside(db, p))
                {
                    return db.lookaside.sz;
                }
                else
                {
                    Debug.Assert(sqliteinth.sqlite3MemdebugHasType(p, MemType.DB));
                    Debug.Assert(sqliteinth.sqlite3MemdebugHasType(p, MemType.LOOKASIDE | MemType.HEAP));
                    Debug.Assert(db != null || sqliteinth.sqlite3MemdebugNoType(p, MemType.LOOKASIDE));
                    return sqliteinth.sqlite3GlobalConfig.m.xSize(p);
                }
            }

            ///<summary>
            /// Free memory previously obtained from malloc_cs.sqlite3Malloc().
            ///
            ///</summary>
            public static void sqlite3_free(ref byte[] p)
            {
                if (p == null)
                    return;
                Debug.Assert(sqliteinth.sqlite3MemdebugNoType(p, MemType.DB));
                Debug.Assert(sqliteinth.sqlite3MemdebugHasType(p, MemType.HEAP));
            if (sqliteinth.sqlite3GlobalConfig.bMemstat)
            {
                using (mem0.mutex.scope())
                {
                    Sqlite3.sqlite3StatusAdd(SqliteStatus.SQLITE_STATUS_MEMORY_USED, -malloc_cs.sqlite3MallocSize(p));
                    Sqlite3.sqlite3StatusAdd(SqliteStatus.SQLITE_STATUS_MALLOC_COUNT, -1);
                    sqliteinth.sqlite3GlobalConfig.m.xFree(ref p);
                }
            }
            else
            {
                sqliteinth.sqlite3GlobalConfig.m.xFree(ref p);
            }
                p = null;
            }

            public static void sqlite3_free(ref Mem p)
            {
                if (p == null)
                    return;
                if (sqliteinth.sqlite3GlobalConfig.bMemstat)
                {
                    using(mem0.mutex.scope())
                    //sqlite3StatusAdd( Sqlite3.Sqlite3.SQLITE_STATUS_MEMORY_USED, -malloc_cs.sqlite3MallocSize( p ) );
                        sqliteinth.sqlite3GlobalConfig.m.xFreeMem(ref p);
                    
                }
                else
                {
                    sqliteinth.sqlite3GlobalConfig.m.xFreeMem(ref p);
                }
                p = null;
            }

            ///<summary>
            /// Free memory that might be associated with a particular database
            /// connection.
            ///
            ///</summary>
            public static void sqlite3DbFree(Connection db, ref byte[] p)
            {
                Debug.Assert(db == null || db.mutex.sqlite3_mutex_held());
                if (db != null)
                {
                    //if ( db.pnBytesFreed != 0 )
                    //{
#if SQLITE_OMIT_LOOKASIDE
                    //db.pnBytesFreed += 1;
#else
																																																																																db.pnBytesFreed += sqlite3DbMallocSize( db, p );
#endif
                    return;
                    //}
#if !SQLITE_OMIT_LOOKASIDE
																																																																																if( isLookaside(db, p) ){
LookasideSlot *pBuf = (LookasideSlot)p;
pBuf.pNext = db.lookaside.pFree;
db.lookaside.pFree = pBuf;
db.lookaside.nOut--;
}else
#endif
                    //{
                    //  Debug.Assert( sqlite3MemdebugHasType( p, MemType.DB ) );
                    //  Debug.Assert( sqlite3MemdebugHasType( p, MemType.LOOKASIDE | MemType.HEAP ) );
                    //  Debug.Assert( db != null || sqliteinth.sqlite3MemdebugNoType( p, MemType.LOOKASIDE ) );
                    //  sqlite3MemdebugSetType( p, MemType.HEAP );
                    //  malloc_cs.sqlite3_free( ref p );
                    //}
                }
            }

            ///<summary>
            /// Change the size of an existing memory allocation
            ///
            ///</summary>
            static byte[] sqlite3Realloc(byte[] pOld, int nBytes)
            {
                int nOld, nNew, nDiff;
                byte[] pNew;
                if (pOld == null)
                {
                    pOld = malloc_cs.sqlite3Malloc(nBytes);
                    return pOld;
                }
                if (nBytes < 0)
                {
                    malloc_cs.sqlite3_free(ref pOld);
                    return null;
                }
                if (nBytes >= 0x7fffff00)
                {
                    ///
                    ///<summary>
                    ///The 0x7ffff00 limit term is explained in comments on malloc_cs.sqlite3Malloc() 
                    ///</summary>

                    return null;
                }
                nOld = malloc_cs.sqlite3MallocSize(pOld);
                nNew = sqliteinth.sqlite3GlobalConfig.m.xRoundup(nBytes);
                if (nOld == nNew)
                {
                    pNew = pOld;
                }
                else
                    if (sqliteinth.sqlite3GlobalConfig.bMemstat)
                    {
                using (mem0.mutex.scope())
                {
                    Sqlite3.sqlite3StatusSet(SqliteStatus.SQLITE_STATUS_MALLOC_SIZE, nBytes);
                    nDiff = nNew - nOld;
                    if (Sqlite3.sqlite3StatusValue(SqliteStatus.SQLITE_STATUS_MEMORY_USED) >= mem0.alarmThreshold - nDiff)
                    {
                        malloc_cs.sqlite3MallocAlarm(nDiff);
                    }
                    Debug.Assert(sqliteinth.sqlite3MemdebugHasType(pOld, MemType.HEAP));
                    Debug.Assert(sqliteinth.sqlite3MemdebugNoType(pOld, ~MemType.HEAP));
                    pNew = sqliteinth.sqlite3GlobalConfig.m.xRealloc(pOld, nNew);
                    if (pNew == null && mem0.alarmCallback != null)
                    {
                        malloc_cs.sqlite3MallocAlarm(nBytes);
                        pNew = sqliteinth.sqlite3GlobalConfig.m.xRealloc(pOld, nNew);
                    }
                    if (pNew != null)
                    {
                        nNew = malloc_cs.sqlite3MallocSize(pNew);
                        Sqlite3.sqlite3StatusAdd(SqliteStatus.SQLITE_STATUS_MEMORY_USED, nNew - nOld);
                    }
                }
                    }
                    else
                    {
                        pNew = sqliteinth.sqlite3GlobalConfig.m.xRealloc(pOld, nNew);
                    }
                return pNew;
            }

            ///<summary>
            /// The public interface to sqlite3Realloc.  Make sure that the memory
            /// subsystem is initialized prior to invoking sqliteRealloc.
            ///
            ///</summary>
            static byte[] sqlite3_realloc(byte[] pOld, int n)
            {
#if !SQLITE_OMIT_AUTOINIT
                if (Sqlite3.sqlite3_initialize() != 0)
                    return null;
#endif
                return sqlite3Realloc(pOld, n);
            }

            ///<summary>
            /// Allocate and zero memory.
            ///
            ///</summary>
            static byte[] sqlite3MallocZero(int n)
            {
                byte[] p = malloc_cs.sqlite3Malloc(n);
                if (p != null)
                {
                    Array.Clear(p, 0, n);
                    // memset(p, 0, n);
                }
                return p;
            }

            ///<summary>
            /// Allocate and zero memory.  If the allocation fails, make
            /// the mallocFailed flag in the connection pointer.
            ///
            ///</summary>
            public static Mem sqlite3DbMallocZero(Connection db, Mem m)
            {
                return new Mem();
            }

            public static byte[] sqlite3DbMallocZero(Connection db, int n)
            {
                byte[] p = sqlite3DbMallocRaw(db, n);
                if (p != null)
                {
                    Array.Clear(p, 0, n);
                    // memset(p, 0, n);
                }
                return p;
            }

            ///<summary>
            /// Allocate and zero memory.  If the allocation fails, make
            /// the mallocFailed flag in the connection pointer.
            ///
            /// If db!=0 and db->mallocFailed is true (indicating a prior malloc
            /// failure on the same database connection) then always return 0.
            /// Hence for a particular database connection, once malloc starts
            /// failing, it fails consistently until mallocFailed is reset.
            /// This is an important assumption.  There are many places in the
            /// code that do things like this:
            ///
            ///         int *a = (int)sqlite3DbMallocRaw(db, 100);
            ///         int *b = (int)sqlite3DbMallocRaw(db, 200);
            ///         if( b ) a[10] = 9;
            ///
            /// In other words, if a subsequent malloc (ex: "b") worked, it is assumed
            /// that all prior mallocs (ex: "a") worked too.
            ///
            ///</summary>
            static byte[] sqlite3DbMallocRaw(Connection db, int n)
            {
                byte[] p;
                Debug.Assert(db == null || db.mutex.sqlite3_mutex_held());
                Debug.Assert(db == null || db.pnBytesFreed == 0);
#if !SQLITE_OMIT_LOOKASIDE
																																																												if( db ){
LookasideSlot *pBuf;
if( db->mallocFailed ){
return 0;
}
if( db->lookaside.bEnabled ){
if( n>db->lookaside.sz ){
db->lookaside.anStat[1]++;
}else if( (pBuf = db->lookaside.pFree)==0 ){
db->lookaside.anStat[2]++;
}else{
db->lookaside.pFree = pBuf->pNext;
db->lookaside.nOut++;
db->lookaside.anStat[0]++;
if( db->lookaside.nOut>db->lookaside.mxOut ){
db->lookaside.mxOut = db->lookaside.nOut;
}
return (void)pBuf;
}
}
}
#else
                //if( db && db->mallocFailed ){
                //  return 0;
                //}
#endif
                p = malloc_cs.sqlite3Malloc(n);
                //if( null==p && db ){
                //  db->mallocFailed = 1;
                //}
#if !SQLITE_OMIT_LOOKASIDE
																																																												sqlite3MemdebugSetType(p, MemType.DB |
((db !=null && db.lookaside.bEnabled) ? MemType.LOOKASIDE : MemType.HEAP));
#endif
                return p;
            }

            ///
            ///<summary>
            ///Resize the block of memory pointed to by p to n bytes. If the
            ///resize fails, set the mallocFailed flag in the connection object.
            ///
            ///</summary>

            static byte[] sqlite3DbRealloc(Connection db, byte[] p, int n)
            {
                byte[] pNew = null;
                Debug.Assert(db != null);
                Debug.Assert(db.mutex.sqlite3_mutex_held());
                //if( db->mallocFailed==0 ){
                if (p == null)
                {
                    return sqlite3DbMallocRaw(db, n);
                }
#if !SQLITE_OMIT_LOOKASIDE
																																																												if( isLookaside(db, p) ){
if( n<=db->lookaside.sz ){
return p;
}
pNew = sqlite3DbMallocRaw(db, n);
if( pNew ){
memcpy(pNew, p, db->lookaside.sz);
sqlite3DbFree(db, ref p);
}
}else
#else
                {
                    {
#endif
                        Debug.Assert(sqliteinth.sqlite3MemdebugHasType(p, MemType.DB));
                        Debug.Assert(sqliteinth.sqlite3MemdebugHasType(p, MemType.LOOKASIDE | MemType.HEAP));
                        sqliteinth.sqlite3MemdebugSetType(p, MemType.HEAP);
                        pNew = sqlite3_realloc(p, n);
                        //if( null==pNew ){
                        //sqlite3MemdebugSetType(p, MemType.DB|MemType.HEAP);
                        //  db->mallocFailed = 1;
                        //}
#if !SQLITE_OMIT_LOOKASIDE
																																																																																																				sqlite3MemdebugSetType(pNew, MemType.DB | 
(db.lookaside.bEnabled ? MemType.LOOKASIDE : MemType.HEAP));
#endif
                    }
                }
                return pNew;
            }

            ///
            ///<summary>
            ///Attempt to reallocate p.  If the reallocation fails, then free p
            ///and set the mallocFailed flag in the database connection.
            ///
            ///</summary>

            static byte[] sqlite3DbReallocOrFree(Connection db, byte[] p, int n)
            {
                byte[] pNew;
                pNew = sqlite3DbRealloc(db, p, n);
                if (null == pNew)
                {
                    sqlite3DbFree(db, ref p);
                }
                return pNew;
            }

            ///
            ///<summary>
            ///Make a copy of a string in memory obtained from sqliteMalloc(). These 
            ///functions call malloc_cs.sqlite3MallocRaw() directly instead of sqliteMalloc(). This
            ///is because when memory debugging is turned on, these two functions are 
            ///called via macros that record the current file and line number in the
            ///ThreadData structure.
            ///
            ///</summary>

            //char *sqlite3DbStrDup(sqlite3 db, string z){
            //  string zNew;
            //  size_t n;
            //  if( z==0 ){
            //    return 0;
            //  }
            //  n = StringExtensions.sqlite3Strlen30(z) + 1;
            //  Debug.Assert( (n&0x7fffffff)==n );
            //  zNew = sqlite3DbMallocRaw(db, (int)n);
            //  if( zNew ){
            //    memcpy(zNew, z, n);
            //  }
            //  return zNew;
            //}
            //char *sqlite3DbStrNDup(sqlite3 db, string z, int n){
            //  string zNew;
            //  if( z==0 ){
            //    return 0;
            //  }
            //  Debug.Assert( (n&0x7fffffff)==n );
            //  zNew = sqlite3DbMallocRaw(db, n+1);
            //  if( zNew ){
            //    memcpy(zNew, z, n);
            //    zNew[n] = 0;
            //  }
            //  return zNew;
            //}
            ///
            ///<summary>
            ///Create a string from the zFromat argument and the va_list that follows.
            ///Store the string in memory obtained from sqliteMalloc() and make pz
            ///point to that string.
            ///
            ///</summary>

            public static void sqlite3SetString(ref string pz, Connection db, string zFormat, params string[] ap)
            {
                //va_list ap;
                lock (_Custom.lock_va_list)
                {
                    string z;
                    _Custom.va_start(ap, zFormat);
                    z = io.sqlite3VMPrintf(db, zFormat, ap);
                    _Custom.va_end(ref ap);
                    db.DbFree(ref pz);
                    pz = z;
                }
            }

            ///
            ///<summary>
            ///This function must be called before exiting any API function (i.e.
            ///returning control to the user) that has called sqlite3_malloc or
            ///sqlite3_realloc.
            ///
            ///The returned value is normally a copy of the second argument to this
            ///function. However, if a malloc() failure has occurred since the previous
            ///invocation SQLITE_NOMEM is returned instead.
            ///
            ///If the first argument, db, is not NULL and a malloc() error has occurred,
            ///</summary>
            ///<param name="then the connection error">code (the value returned by sqlite3_errcode())</param>
            ///<param name="is set to SQLITE_NOMEM.">is set to SQLITE_NOMEM.</param>
            ///<param name=""></param>

            public static SqlResult sqlite3ApiExit(int zero, SqlResult rc)
            {
                Connection db = null;
                return malloc_cs.sqlite3ApiExit(db, rc);
            }

            public static SqlResult sqlite3ApiExit(Connection db, SqlResult rc)
            {
                ///
                ///<summary>
                ///If the db handle is not NULL, then we must hold the connection handle
                ///mutex here. Otherwise the read (and possible write) of db.mallocFailed
                ///is unsafe, as is the call to utilc.sqlite3Error().
                ///
                ///</summary>

                Debug.Assert(db == null || db.mutex.sqlite3_mutex_held());
                if (///
                    ///<summary>
                    ///db != null && db.mallocFailed != 0 || 
                    ///</summary>

                rc == SqlResult.SQLITE_IOERR_NOMEM)
                {
                    utilc.sqlite3Error(db, SqlResult.SQLITE_NOMEM, "");
                    //db.mallocFailed = 0;
                    rc = SqlResult.SQLITE_NOMEM;
                }
                return rc & (db != null ? db.errMask : (SqlResult)0xff);
            }
        }
    }
