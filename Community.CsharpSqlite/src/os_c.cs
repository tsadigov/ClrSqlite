using System.Diagnostics;
using System.Text;
using HANDLE = System.IntPtr;
using i64 = System.Int64;
using u32 = System.UInt32;
using sqlite3_int64 = System.Int64;

namespace Community.CsharpSqlite
{
    public partial class Sqlite3
    {
        public class os
        {
            ///
            ///<summary>
            ///2005 November 29
            ///
            ///The author disclaims copyright to this source code.  In place of
            ///a legal notice, here is a blessing:
            ///
            ///May you do good and not evil.
            ///May you find forgiveness for yourself and forgive others.
            ///May you share freely, never taking more than you give.
            ///
            ///
            ///
            ///This file contains OS interface code that is common to all
            ///architectures.
            ///
            ///</summary>
            ///<param name="Included in SQLite3 port to C#">SQLite;  2008 Noah B Hart</param>
            ///<param name="C#">SQLite is an independent reimplementation of the SQLite software library</param>
            ///<param name=""></param>
            ///<param name="SQLITE_SOURCE_ID: 2010">07 20:14:09 a586a4deeb25330037a49df295b36aaf624d0f45</param>
            ///<param name=""></param>
            ///<param name=""></param>
            ///<param name=""></param>

            //#define _SQLITE_OS_C_ 1
            //#include "sqliteInt.h"
            //#undef _SQLITE_OS_C_
            ///<summary>
            /// The default SQLite sqlite3_vfs implementations do not allocate
            /// memory (actually, os_unix.c allocates a small amount of memory
            /// from within OsOpen()), but some third-party implementations may.
            /// So we test the effects of a malloc() failing and the sqlite3OsXXX()
            /// function returning SQLITE_IOERR_NOMEM using the DO_OS_MALLOC_TEST macro.
            ///
            /// The following functions are instrumented for malloc() failure
            /// testing:
            ///
            ///     sqlite3OsOpen()
            ///     sqlite3OsRead()
            ///     sqlite3OsWrite()
            ///     sqlite3OsSync()
            ///     sqlite3OsLock()
            ///
            ///
            ///</summary>
#if (SQLITE_TEST)
																																						    static int sqlite3_memdebug_vfs_oom_test = 1;

    //define DO_OS_MALLOC_TEST(x)                                       \
    //if (sqlite3_memdebug_vfs_oom_test && (!x || !sqlite3IsMemJournal(x))) {  \
    //  void *pTstAlloc = malloc_cs.sqlite3Malloc(10);                             \
    //  if (!pTstAlloc) return SQLITE_IOERR_NOMEM;                       \
    //  malloc_cs.sqlite3_free(pTstAlloc);                                         \
    //}
    static void DO_OS_MALLOC_TEST( sqlite3_file x )
    {
    }
#else
            //#define DO_OS_MALLOC_TEST(x)
            static void DO_OS_MALLOC_TEST(sqlite3_file x)
            {
            }

#endif
            ///<summary>
            /// The following routines are convenience wrappers around methods
            /// of the sqlite3_file object.  This is mostly just syntactic sugar. All
            /// of this would be completely automatic if SQLite were coded using
            /// C++ instead of plain old C.
            ///</summary>
            public static SqlResult sqlite3OsClose(sqlite3_file pId)
            {
                var rc = SqlResult.SQLITE_OK;
                if (pId.pMethods != null)
                {
                    rc = pId.pMethods.xClose(pId);
                    pId.pMethods = null;
                }
                return rc;
            }

            public static SqlResult  sqlite3OsRead(sqlite3_file id, byte[] pBuf, int amt, i64 offset)
            {
                DO_OS_MALLOC_TEST(id);
                if (pBuf == null)
                    pBuf = malloc_cs.sqlite3Malloc(amt);
                return id.pMethods.xRead(id, pBuf, amt, offset);
            }

            public static SqlResult sqlite3OsWrite(sqlite3_file id, byte[] pBuf, int amt, i64 offset)
            {
                DO_OS_MALLOC_TEST(id);
                return id.pMethods.xWrite(id, pBuf, amt, offset);
            }

            public static SqlResult sqlite3OsTruncate(sqlite3_file id, i64 size)
            {
                return id.pMethods.xTruncate(id, size);
            }

            public static SqlResult sqlite3OsSync(sqlite3_file id, int flags)
            {
                DO_OS_MALLOC_TEST(id);
                return id.pMethods.xSync(id, flags);
            }

            public static SqlResult sqlite3OsFileSize(sqlite3_file id, ref long pSize)
            {
                return id.pMethods.xFileSize(id, ref pSize);
            }

            public static SqlResult sqlite3OsLock(sqlite3_file id, int lockType)
            {
                DO_OS_MALLOC_TEST(id);
                return id.pMethods.xLock(id, lockType);
            }

            public static SqlResult sqlite3OsUnlock(sqlite3_file id, int lockType)
            {
                return id.pMethods.xUnlock(id, lockType);
            }

            public static SqlResult sqlite3OsCheckReservedLock(sqlite3_file id, ref int pResOut)
            {
                DO_OS_MALLOC_TEST(id);
                return id.pMethods.xCheckReservedLock(id, ref pResOut);
            }

            public static SqlResult sqlite3OsFileControl(sqlite3_file id, u32 op, ref sqlite3_int64 pArg)
            {
                return id.pMethods.xFileControl(id, (int)op, ref pArg);
            }

            public static int sqlite3OsSectorSize(sqlite3_file id)
            {
                dxSectorSize xSectorSize = id.pMethods.xSectorSize;
                return (xSectorSize != null ? xSectorSize(id) : SQLITE_DEFAULT_SECTOR_SIZE);
            }

            public static int sqlite3OsDeviceCharacteristics(sqlite3_file id)
            {
                return id.pMethods.xDeviceCharacteristics(id);
            }

            static int sqlite3OsShmLock(sqlite3_file id, int offset, int n, int flags)
            {
                return id.pMethods.xShmLock(id, offset, n, flags);
            }

            static void sqlite3OsShmBarrier(sqlite3_file id)
            {
                id.pMethods.xShmBarrier(id);
            }

            static int sqlite3OsShmUnmap(sqlite3_file id, int deleteFlag)
            {
                return id.pMethods.xShmUnmap(id, deleteFlag);
            }

            static int sqlite3OsShmMap(sqlite3_file id, ///
                ///<summary>
                ///Database file handle 
                ///</summary>

            int iPage, int pgsz, int bExtend, ///
                ///<summary>
                ///True to extend file if necessary 
                ///</summary>

            out object pp///
                ///<summary>
                ///OUT: Pointer to mapping 
                ///</summary>

            )
            {
                return id.pMethods.xShmMap(id, iPage, pgsz, bExtend, out pp);
            }

            ///<summary>
            /// The next group of routines are convenience wrappers around the
            /// VFS methods.
            ///
            ///</summary>
            public static SqlResult sqlite3OsOpen(sqlite3_vfs pVfs, string zPath, sqlite3_file pFile, int flags, ref int pFlagsOut)
            {
                SqlResult rc;
                DO_OS_MALLOC_TEST(null);
                ///
                ///<summary>
                ///0x87f3f is a mask of SQLITE_OPEN_ flags that are valid to be passed
                ///down into the VFS layer.  Some SQLITE_OPEN_ flags (for example,
                ///SQLITE_OPEN_FULLMUTEX or SQLITE_OPEN_SHAREDCACHE) are blocked before
                ///reaching the VFS. 
                ///</summary>

                rc = pVfs.xOpen(pVfs, zPath, pFile, flags & 0x87f3f, out pFlagsOut);
                Debug.Assert(rc == SqlResult.SQLITE_OK || pFile.pMethods == null);
                return rc;
            }

            public static SqlResult sqlite3OsDelete(sqlite3_vfs pVfs, string zPath, int dirSync)
            {
                return pVfs.xDelete(pVfs, zPath, dirSync);
            }

            public static SqlResult sqlite3OsAccess(sqlite3_vfs pVfs, string zPath, int flags, ref int pResOut)
            {
                DO_OS_MALLOC_TEST(null);
                return pVfs.xAccess(pVfs, zPath, flags, out pResOut);
            }

            public static SqlResult sqlite3OsFullPathname(sqlite3_vfs pVfs, string zPath, int nPathOut, StringBuilder zPathOut)
            {
                zPathOut.Length = 0;
                //zPathOut[0] = 0;
                return pVfs.xFullPathname(pVfs, zPath, nPathOut, zPathOut);
            }

#if !SQLITE_OMIT_LOAD_EXTENSION
            public static HANDLE sqlite3OsDlOpen(sqlite3_vfs pVfs, string zPath)
            {
                return pVfs.xDlOpen(pVfs, zPath);
            }

            public static void sqlite3OsDlError(sqlite3_vfs pVfs, int nByte, string zBufOut)
            {
                pVfs.xDlError(pVfs, nByte, zBufOut);
            }

            public static object sqlite3OsDlSym(sqlite3_vfs pVfs, HANDLE pHdle, ref string zSym)
            {
                return pVfs.xDlSym(pVfs, pHdle, zSym);
            }

            public static void sqlite3OsDlClose(sqlite3_vfs pVfs, HANDLE pHandle)
            {
                pVfs.xDlClose(pVfs, pHandle);
            }

#endif
            public static int sqlite3OsRandomness(sqlite3_vfs pVfs, int nByte, byte[] zBufOut)
            {
                return pVfs.xRandomness(pVfs, nByte, zBufOut);
            }

            public static int sqlite3OsSleep(sqlite3_vfs pVfs, int nMicro)
            {
                return pVfs.xSleep(pVfs, nMicro);
            }

            public static int sqlite3OsCurrentTimeInt64(sqlite3_vfs pVfs, ref sqlite3_int64 pTimeOut)
            {
                int rc;
                ///
                ///<summary>
                ///</summary>
                ///<param name="IMPLEMENTATION">42493 SQLite will use the xCurrentTimeInt64()</param>
                ///<param name="method to get the current date and time if that method is available">method to get the current date and time if that method is available</param>
                ///<param name="(if iVersion is 2 or greater and the function pointer is not NULL) and">(if iVersion is 2 or greater and the function pointer is not NULL) and</param>
                ///<param name="will fall back to xCurrentTime() if xCurrentTimeInt64() is">will fall back to xCurrentTime() if xCurrentTimeInt64() is</param>
                ///<param name="unavailable.">unavailable.</param>
                ///<param name=""></param>

                if (pVfs.iVersion >= 2 && pVfs.xCurrentTimeInt64 != null)
                {
                    rc = pVfs.xCurrentTimeInt64(pVfs, ref pTimeOut);
                }
                else
                {
                    double r = 0;
                    rc = pVfs.xCurrentTime(pVfs, ref r);
                    pTimeOut = (sqlite3_int64)(r * 86400000.0);
                }
                return rc;
            }

            public static SqlResult sqlite3OsOpenMalloc(ref sqlite3_vfs pVfs, string zFile, ref sqlite3_file ppFile, int flags, ref int pOutFlags)
            {
                SqlResult rc = SqlResult.SQLITE_NOMEM;
                sqlite3_file pFile;
                pFile = new sqlite3_file();
                //malloc_cs.sqlite3Malloc(ref pVfs.szOsFile);
                if (pFile != null)
                {
                    rc = sqlite3OsOpen(pVfs, zFile, pFile, flags, ref pOutFlags);
                    if (rc != SqlResult.SQLITE_OK)
                    {
                        pFile = null;
                        // was  sqlite3DbFree(db,ref  pFile);
                    }
                    else
                    {
                        ppFile = pFile;
                    }
                }
                return rc;
            }

            public static SqlResult sqlite3OsCloseFree(sqlite3_file pFile)
            {
                SqlResult rc = SqlResult.SQLITE_OK;
                Debug.Assert(pFile != null);
                rc = sqlite3OsClose(pFile);
                //malloc_cs.sqlite3_free( ref pFile );
                return rc;
            }

            ///
            ///<summary>
            ///This function is a wrapper around the OS specific implementation of
            ///sqlite3_os_init(). The purpose of the wrapper is to provide the
            ///ability to simulate a malloc failure, so that the handling of an
            ///error in sqlite3_os_init() by the upper layers can be tested.
            ///
            ///</summary>

            static SqlResult sqlite3OsInit()
            {
                //void *p = sqlite3_malloc(10);
                //if( p==null ) return SQLITE_NOMEM;
                //malloc_cs.sqlite3_free(ref p);
                return sqlite3_os_init();
            }

            ///
            ///<summary>
            ///The list of all registered VFS implementations.
            ///
            ///</summary>

            public static sqlite3_vfs s_vfsList;

            public static sqlite3_vfs vfsList
            {
                get { return s_vfsList; }
                set { s_vfsList = value; }
            }

            //#define vfsList GLOBAL(sqlite3_vfs *, vfsList)
            ///<summary>
            /// Locate a VFS by name.  If no name is given, simply return the
            /// first VFS on the list.
            ///
            ///</summary>
            static bool isInit = false;

            public static sqlite3_vfs sqlite3_vfs_find(string zVfs)
            {
                sqlite3_vfs pVfs = null;
#if SQLITE_THREADSAFE
																																																									      sqlite3_mutex mutex;
#endif
#if !SQLITE_OMIT_AUTOINIT
                SqlResult rc = sqlite3_initialize();
                if (rc != 0)
                    return null;
#endif
#if SQLITE_THREADSAFE
																																																									      mutex = sqlite3MutexAlloc( SQLITE_MUTEX_STATIC_MASTER );
#endif
                mutex.sqlite3_mutex_enter();
                for (pVfs = vfsList; pVfs != null; pVfs = pVfs.pNext)
                {
                    if (zVfs == null || zVfs == "")
                        break;
                    if (zVfs == pVfs.zName)
                        break;
                    //strcmp(zVfs, pVfs.zName) == null) break;
                }
                mutex.sqlite3_mutex_leave();
                return pVfs;
            }

            ///<summary>
            /// Unlink a VFS from the linked list
            ///
            ///</summary>
            static void vfsUnlink(sqlite3_vfs pVfs)
            {
                Debug.Assert(Sqlite3.sqlite3MutexAlloc(SQLITE_MUTEX_STATIC_MASTER).sqlite3_mutex_held());
                if (pVfs == null)
                {
                    ///
                    ///<summary>
                    ///</summary>
                    ///<param name="No">op </param>

                }
                else
                    if (vfsList == pVfs)
                    {
                        vfsList = pVfs.pNext;
                    }
                    else
                        if (vfsList != null)
                        {
                            sqlite3_vfs p = vfsList;
                            while (p.pNext != null && p.pNext != pVfs)
                            {
                                p = p.pNext;
                            }
                            if (p.pNext == pVfs)
                            {
                                p.pNext = pVfs.pNext;
                            }
                        }
            }

            ///<summary>
            /// Register a VFS with the system.  It is harmless to register the same
            /// VFS multiple times.  The new VFS becomes the default if makeDflt is
            /// true.
            ///
            ///</summary>
            public static SqlResult sqlite3_vfs_register(sqlite3_vfs pVfs, int makeDflt)
            {
                sqlite3_mutex mutex;
#if !SQLITE_OMIT_AUTOINIT
                SqlResult rc = sqlite3_initialize();
                if (rc != 0)
                    return rc;
#endif
                mutex = sqlite3MutexAlloc(SQLITE_MUTEX_STATIC_MASTER);
                mutex.sqlite3_mutex_enter();
                vfsUnlink(pVfs);
                if (makeDflt != 0 || vfsList == null)
                {
                    pVfs.pNext = vfsList;
                    vfsList = pVfs;
                }
                else
                {
                    pVfs.pNext = vfsList.pNext;
                    vfsList.pNext = pVfs;
                }
                Debug.Assert(vfsList != null);
                mutex.sqlite3_mutex_leave();
                return SqlResult.SQLITE_OK;
            }

            ///
            ///<summary>
            ///Unregister a VFS so that it is no longer accessible.
            ///
            ///</summary>

            static SqlResult sqlite3_vfs_unregister(sqlite3_vfs pVfs)
            {
#if SQLITE_THREADSAFE
																																																									      sqlite3_mutex mutex = sqlite3MutexAlloc( SQLITE_MUTEX_STATIC_MASTER );
#endif
                mutex.sqlite3_mutex_enter();
                vfsUnlink(pVfs);
                mutex.sqlite3_mutex_leave();
                return SqlResult.SQLITE_OK;
            }
        }
    }
}