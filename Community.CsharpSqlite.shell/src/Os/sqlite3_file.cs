using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using HANDLE = System.IntPtr;
using i16 = System.Int16;
using sqlite3_int64 = System.Int64;
using u32 = System.UInt32;

using System.Threading;
using DWORD = System.UInt64;
using System.IO;

namespace Community.CsharpSqlite
{
    public partial class Sqlite3
    {
        ///<summary>
        /// CAPI3REF: OS Interface Open File Handle
        ///
        /// An [sqlite3_file] object represents an open file in the
        /// [sqlite3_vfs | OS interface layer].  Individual OS interface
        /// implementations will
        /// want to subclass this object by appending additional fields
        /// for their own use.  The pMethods entry is a pointer to an
        /// [sqlite3_io_methods] object that defines methods for performing
        /// I/O operations on the open file.
        ///---------------------------------------------------
        ///  This subclass is a subclass of sqlite3_file.  Each open memory-journal
        /// is an instance of this class.
        ///------------------------------------------------------
        /// The winFile structure is a subclass of sqlite3_file* specific to the win32
        /// portability layer.
        /// 
        ///</summary>
        //typedef struct sqlite3_file sqlite3_file;
        //struct sqlite3_file {
        //  const struct sqlite3_io_methods *pMethods;  /* Methods for an open file */
        //};
        public partial class sqlite3_file
        {
            public sqlite3_vfs pVfs;

            ///
            ///<summary>
            ///The VFS used to open this file 
            ///</summary>

            public FileStream fs;

            ///
            ///<summary>
            ///Filestream access to this file
            ///</summary>

            // public HANDLE h;            /* Handle for accessing the file */
            public int locktype;

            ///
            ///<summary>
            ///Type of lock currently held on this file 
            ///</summary>

            public int sharedLockByte;

            ///
            ///<summary>
            ///Randomly chosen byte used as a shared lock 
            ///</summary>

            public DWORD lastErrno;

            ///
            ///<summary>
            ///The Windows errno from the last I/O error 
            ///</summary>

            public DWORD sectorSize;

            ///
            ///<summary>
            ///Sector size of the device file is on 
            ///</summary>

#if !SQLITE_OMIT_WAL
																																																									public winShm pShm;            /* Instance of shared memory on this file */
#else
            public object pShm;

            ///
            ///<summary>
            ///DUMMY Instance of shared memory on this file 
            ///</summary>

#endif
            public string zPath;

            ///
            ///<summary>
            ///Full pathname of this file 
            ///</summary>

            public int szChunk;

            ///<summary>
            ///Chunk size configured by FCNTL_CHUNK_SIZE
            ///</summary>
#if SQLITE_OS_WINCE
																																																									Wstring zDeleteOnClose;  /* Name of file to delete when closing */
HANDLE hMutex;          /* Mutex used to control access to shared lock */
HANDLE hShared;         /* Shared memory segment used for locking */
winceLock local;        /* Locks obtained by this instance of sqlite3_file */
winceLock *shared;      /* Global shared lock memory for the file  */
#endif
            public void Clear()
            {
                pMethods = null;
                fs = null;
                locktype = 0;
                sharedLockByte = 0;
                lastErrno = 0;
                sectorSize = 0;
            }

            public Community.CsharpSqlite.Sqlite3.sqlite3_io_methods pMethods;
            ///
            ///<summary>
            ///Must be first 
            ///</summary>
            ///

            ///<summary>
            /// The argument to this macro is a file descriptor (type sqlite3_file*).
            /// Return 0 if it is not open, or non-zero (but not 1) if it is.
            ///
            /// This is so that expressions can be written as:
            ///
            ///   if( isOpen(pPager.jfd) ){ ...
            ///
            /// instead of
            ///
            ///   if( pPager.jfd->pMethods ){ ...
            ///
            ///</summary>
            //#define isOpen(pFd) ((pFd)->pMethods)
            public bool isOpen
            {
                get { return this.pMethods != null; }
            }


            //public sqlite3_io_methods pMethods; /* Parent class. MUST BE FIRST */
            public FileChunk pFirst;

            ///
            ///<summary>
            ///</summary>
            ///<param name="Head of in">list </param>

            public FilePoint endpoint;

            ///
            ///<summary>
            ///Pointer to the end of the file 
            ///</summary>

            public FilePoint readpoint;

            ///
            ///<summary>
            ///Pointer to the end of the last xRead() 
            ///</summary>

            public///<summary>
                /// If pFile is currently larger than iSize bytes, then truncate it to
                /// exactly iSize bytes. If pFile is not larger than iSize bytes, then
                /// this function is a no-op.
                ///
                /// Return SQLITE_OK if everything is successful, or an SQLite error
                /// code if an error occurs.
                ///</summary>
            int backupTruncateFile(int iSize)
            {
                long iCurrent = 0;
                int rc = os.sqlite3OsFileSize(this, ref iCurrent);
                if (rc == SQLITE_OK && iCurrent > iSize)
                {
                    rc = os.sqlite3OsTruncate(this, iSize);
                }
                return rc;
            }
        }

        ///<summary>
        /// CAPI3REF: OS Interface File Virtual Methods Object
        ///
        /// Every file opened by the [sqlite3_vfs.xOpen] method populates an
        /// [sqlite3_file] object (or, more commonly, a subclass of the
        /// [sqlite3_file] object) with a pointer to an instance of this object.
        /// This object defines the methods used to perform various operations
        /// against the open file represented by the [sqlite3_file] object.
        ///
        /// If the [sqlite3_vfs.xOpen] method sets the sqlite3_file.pMethods element
        /// to a non-NULL pointer, then the sqlite3_io_methods.xClose method
        /// may be invoked even if the [sqlite3_vfs.xOpen] reported that it failed.  The
        /// only way to prevent a call to xClose following a failed [sqlite3_vfs.xOpen]
        /// is for the [sqlite3_vfs.xOpen] to set the sqlite3_file.pMethods element
        /// to NULL.
        ///
        /// The flags argument to xSync may be one of [SQLITE_SYNC_NORMAL] or
        /// [SQLITE_SYNC_FULL].  The first choice is the normal fsync().
        /// The second choice is a Mac OS X style fullsync.  The [SQLITE_SYNC_DATAONLY]
        /// flag may be ORed in to indicate that only the data of the file
        /// and not its inode needs to be synced.
        ///
        /// The integer values to xLock() and xUnlock() are one of
        /// <ul>
        /// <li> [SQLITE_LOCK_NONE],
        /// <li> [SQLITE_LOCK_SHARED],
        /// <li> [SQLITE_LOCK_RESERVED],
        /// <li> [SQLITE_LOCK_PENDING], or
        /// <li> [SQLITE_LOCK_EXCLUSIVE].
        /// </ul>
        /// xLock() increases the lock. xUnlock() decreases the lock.
        /// The xCheckReservedLock() method checks whether any database connection,
        /// either in this process or in some other process, is holding a RESERVED,
        /// PENDING, or EXCLUSIVE lock on the file.  It returns true
        /// if such a lock exists and false otherwise.
        ///
        /// The xFileControl() method is a generic interface that allows custom
        /// VFS implementations to directly control an open file using the
        /// [sqlite3_file_control()] interface.  The second "op" argument is an
        /// integer opcode.  The third argument is a generic pointer intended to
        /// point to a structure that may contain arguments or space in which to
        /// write return values.  Potential uses for xFileControl() might be
        /// functions to enable blocking locks with timeouts, to change the
        /// locking strategy (for example to use dot-file locks), to inquire
        /// about the status of a lock, or to break stale locks.  The SQLite
        /// core reserves all opcodes less than 100 for its own use.
        /// A [SQLITE_FCNTL_LOCKSTATE | list of opcodes] less than 100 is available.
        /// Applications that define a custom xFileControl method should use opcodes
        /// greater than 100 to avoid conflicts.  VFS implementations should
        /// return [SQLITE_NOTFOUND] for file control opcodes that they do not
        /// recognize.
        ///
        /// The xSectorSize() method returns the sector size of the
        /// device that underlies the file.  The sector size is the
        /// minimum write that can be performed without disturbing
        /// other bytes in the file.  The xDeviceCharacteristics()
        /// method returns a bit vector describing behaviors of the
        /// underlying device:
        ///
        /// <ul>
        /// <li> [SQLITE_IOCAP_ATOMIC]
        /// <li> [SQLITE_IOCAP_ATOMIC512]
        /// <li> [SQLITE_IOCAP_ATOMIC1K]
        /// <li> [SQLITE_IOCAP_ATOMIC2K]
        /// <li> [SQLITE_IOCAP_ATOMIC4K]
        /// <li> [SQLITE_IOCAP_ATOMIC8K]
        /// <li> [SQLITE_IOCAP_ATOMIC16K]
        /// <li> [SQLITE_IOCAP_ATOMIC32K]
        /// <li> [SQLITE_IOCAP_ATOMIC64K]
        /// <li> [SQLITE_IOCAP_SAFE_APPEND]
        /// <li> [SQLITE_IOCAP_SEQUENTIAL]
        /// </ul>
        ///
        /// The SQLITE_IOCAP_ATOMIC property means that all writes of
        /// any size are atomic.  The SQLITE_IOCAP_ATOMICnnn values
        /// mean that writes of blocks that are nnn bytes in size and
        /// are aligned to an address which is an integer multiple of
        /// nnn are atomic.  The SQLITE_IOCAP_SAFE_APPEND value means
        /// that when data is appended to a file, the data is appended
        /// first then the size of the file is extended, never the other
        /// way around.  The SQLITE_IOCAP_SEQUENTIAL property means that
        /// information is written to disk in the same order as calls
        /// to xWrite().
        ///
        /// If xRead() returns SQLITE_IOERR_SHORT_READ it must also fill
        /// in the unread portions of the buffer with zeros.  A VFS that
        /// fails to zero-fill short reads might seem to work.  However,
        /// failure to zero-fill short reads will eventually lead to
        /// database corruption.
        ///
        ///</summary>
        //typedef struct sqlite3_io_methods sqlite3_io_methods;
        //struct sqlite3_io_methods {
        //  int iVersion;
        //  int (*xClose)(sqlite3_file);
        //  int (*xRead)(sqlite3_file*, void*, int iAmt, sqlite3_int64 iOfst);
        //  int (*xWrite)(sqlite3_file*, const void*, int iAmt, sqlite3_int64 iOfst);
        //  int (*xTruncate)(sqlite3_file*, sqlite3_int64 size);
        //  int (*xSync)(sqlite3_file*, int flags);
        //  int (*xFileSize)(sqlite3_file*, sqlite3_int64 *pSize);
        //  int (*xLock)(sqlite3_file*, int);
        //  int (*xUnlock)(sqlite3_file*, int);
        //  int (*xCheckReservedLock)(sqlite3_file*, int *pResOut);
        //  int (*xFileControl)(sqlite3_file*, int op, object  *pArg);
        //  int (*xSectorSize)(sqlite3_file);
        //  int (*xDeviceCharacteristics)(sqlite3_file);
        //  /* Methods above are valid for version 1 */
        //  int (*xShmMap)(sqlite3_file*, int iPg, int pgsz, int, object  volatile*);
        //  int (*xShmLock)(sqlite3_file*, int offset, int n, int flags);
        //  void (*xShmBarrier)(sqlite3_file);
        //  int (*xShmUnmap)(sqlite3_file*, int deleteFlag);
        //  /* Methods above are valid for version 2 */
        //  /* Additional methods may be added in future releases */
        //};
        public class sqlite3_io_methods
        {
            public int iVersion;

            public dxClose xClose;

            public dxRead xRead;

            public dxWrite xWrite;

            public dxTruncate xTruncate;

            public dxSync xSync;

            public dxFileSize xFileSize;

            public dxLock xLock;

            public dxUnlock xUnlock;

            public dxCheckReservedLock xCheckReservedLock;

            public dxFileControl xFileControl;

            public dxSectorSize xSectorSize;

            public dxDeviceCharacteristics xDeviceCharacteristics;

            public dxShmMap xShmMap;

            //int (*xShmMap)(sqlite3_file*, int iPg, int pgsz, int, object  volatile*);
            public dxShmLock xShmLock;

            //int (*xShmLock)(sqlite3_file*, int offset, int n, int flags);
            public dxShmBarrier xShmBarrier;

            //void (*xShmBarrier)(sqlite3_file);
            public dxShmUnmap xShmUnmap;

            //int (*xShmUnmap)(sqlite3_file*, int deleteFlag);
            ///
            ///<summary>
            ///Additional methods may be added in future releases 
            ///</summary>

            public sqlite3_io_methods(int iVersion, dxClose xClose, dxRead xRead, dxWrite xWrite, dxTruncate xTruncate, dxSync xSync, dxFileSize xFileSize, dxLock xLock, dxUnlock xUnlock, dxCheckReservedLock xCheckReservedLock, dxFileControl xFileControl, dxSectorSize xSectorSize, dxDeviceCharacteristics xDeviceCharacteristics, dxShmMap xShmMap, dxShmLock xShmLock, dxShmBarrier xShmBarrier, dxShmUnmap xShmUnmap)
            {
                this.iVersion = iVersion;
                this.xClose = xClose;
                this.xRead = xRead;
                this.xWrite = xWrite;
                this.xTruncate = xTruncate;
                this.xSync = xSync;
                this.xFileSize = xFileSize;
                this.xLock = xLock;
                this.xUnlock = xUnlock;
                this.xCheckReservedLock = xCheckReservedLock;
                this.xFileControl = xFileControl;
                this.xSectorSize = xSectorSize;
                this.xDeviceCharacteristics = xDeviceCharacteristics;
                this.xShmMap = xShmMap;
                this.xShmLock = xShmLock;
                this.xShmBarrier = xShmBarrier;
                this.xShmUnmap = xShmUnmap;
            }
        }

        ///
        ///<summary>
        ///CAPI3REF: Standard File Control Opcodes
        ///
        ///These integer constants are opcodes for the xFileControl method
        ///of the [sqlite3_io_methods] object and for the [sqlite3_file_control()]
        ///interface.
        ///
        ///The [SQLITE_FCNTL_LOCKSTATE] opcode is used for debugging.  This
        ///opcode causes the xFileControl method to write the current state of
        ///the lock (one of [SQLITE_LOCK_NONE], [SQLITE_LOCK_SHARED],
        ///[SQLITE_LOCK_RESERVED], [SQLITE_LOCK_PENDING], or [SQLITE_LOCK_EXCLUSIVE])
        ///into an integer that the pArg argument points to. This capability
        ///is used during testing and only needs to be supported when SQLITE_TEST
        ///is defined.
        ///
        ///The [SQLITE_FCNTL_SIZE_HINT] opcode is used by SQLite to give the VFS
        ///layer a hint of how large the database file will grow to be during the
        ///current transaction.  This hint is not guaranteed to be accurate but it
        ///is often close.  The underlying VFS might choose to preallocate database
        ///file space based on this hint in order to help writes to the database
        ///file run faster.
        ///
        ///The [SQLITE_FCNTL_CHUNK_SIZE] opcode is used to request that the VFS
        ///extends and truncates the database file in chunks of a size specified
        ///by the user. The fourth argument to [sqlite3_file_control()] should 
        ///</summary>
        ///<param name="point to an integer (type int) containing the new chunk">size to use</param>
        ///<param name="for the nominated database. Allocating database file space in large">for the nominated database. Allocating database file space in large</param>
        ///<param name="chunks (say 1MB at a time), may reduce file">system fragmentation and</param>
        ///<param name="improve performance on some systems.">improve performance on some systems.</param>
        ///<param name=""></param>
        ///<param name="The [SQLITE_FCNTL_FILE_POINTER] opcode is used to obtain a pointer">The [SQLITE_FCNTL_FILE_POINTER] opcode is used to obtain a pointer</param>
        ///<param name="to the [sqlite3_file] object associated with a particular database">to the [sqlite3_file] object associated with a particular database</param>
        ///<param name="connection.  See the [sqlite3_file_control()] documentation for">connection.  See the [sqlite3_file_control()] documentation for</param>
        ///<param name="additional information.">additional information.</param>
        ///<param name=""></param>
        ///<param name="^(The [SQLITE_FCNTL_SYNC_OMITTED] opcode is generated internally by">^(The [SQLITE_FCNTL_SYNC_OMITTED] opcode is generated internally by</param>
        ///<param name="SQLite and sent to all VFSes in place of a call to the xSync method">SQLite and sent to all VFSes in place of a call to the xSync method</param>
        ///<param name="when the database connection has [PRAGMA synchronous] set to OFF.)^">when the database connection has [PRAGMA synchronous] set to OFF.)^</param>
        ///<param name="Some specialized VFSes need this signal in order to operate correctly">Some specialized VFSes need this signal in order to operate correctly</param>
        ///<param name="when [PRAGMA synchronous | PRAGMA synchronous=OFF] is set, but most ">when [PRAGMA synchronous | PRAGMA synchronous=OFF] is set, but most </param>
        ///<param name="VFSes do not need this signal and should silently ignore this opcode.">VFSes do not need this signal and should silently ignore this opcode.</param>
        ///<param name="Applications should not call [sqlite3_file_control()] with this">Applications should not call [sqlite3_file_control()] with this</param>
        ///<param name="opcode as doing so may disrupt the operation of the specialized VFSes">opcode as doing so may disrupt the operation of the specialized VFSes</param>
        ///<param name="that do require it.  ">that do require it.  </param>
        ///<param name=""></param>

        //#define SQLITE_FCNTL_LOCKSTATE        1
        //#define SQLITE_GET_LOCKPROXYFILE      2
        //#define SQLITE_SET_LOCKPROXYFILE      3
        //#define SQLITE_LAST_ERRNO             4
        //#define SQLITE_FCNTL_SIZE_HINT        5
        //#define SQLITE_FCNTL_CHUNK_SIZE       6
        //#define SQLITE_FCNTL_FILE_POINTER     7
        //#define SQLITE_FCNTL_SYNC_OMITTED     8
        private const int SQLITE_FCNTL_LOCKSTATE = 1;

        private const int SQLITE_GET_LOCKPROXYFILE = 2;

        private const int SQLITE_SET_LOCKPROXYFILE = 3;

        private const int SQLITE_LAST_ERRNO = 4;

        private const int SQLITE_FCNTL_SIZE_HINT = 5;

        private const int SQLITE_FCNTL_CHUNK_SIZE = 6;

        private const int SQLITE_FCNTL_FILE_POINTER = 7;

        private const int SQLITE_FCNTL_SYNC_OMITTED = 8;





















        ///
        ///<summary>
        ///VFS Delegates
        ///
        ///</summary>

        public delegate int dxClose(sqlite3_file File_ID);

        public delegate int dxCheckReservedLock(sqlite3_file File_ID, ref int pRes);

        public delegate int dxDeviceCharacteristics(sqlite3_file File_ID);

        public delegate int dxFileControl(sqlite3_file File_ID, int op, ref sqlite3_int64 pArgs);

        public delegate int dxFileSize(sqlite3_file File_ID, ref long size);

        public delegate int dxLock(sqlite3_file File_ID, int locktype);

        public delegate int dxRead(sqlite3_file File_ID, byte[] buffer, int amount, sqlite3_int64 offset);

        public delegate int dxSectorSize(sqlite3_file File_ID);

        public delegate int dxSync(sqlite3_file File_ID, int flags);

        public delegate int dxTruncate(sqlite3_file File_ID, sqlite3_int64 size);

        public delegate int dxUnlock(sqlite3_file File_ID, int locktype);

        public delegate int dxWrite(sqlite3_file File_ID, byte[] buffer, int amount, sqlite3_int64 offset);

        public delegate int dxShmMap(sqlite3_file File_ID, int iPg, int pgsz, int pInt, out object pvolatile);

        public delegate int dxShmLock(sqlite3_file File_ID, int offset, int n, int flags);

        public delegate void dxShmBarrier(sqlite3_file File_ID);

        public delegate int dxShmUnmap(sqlite3_file File_ID, int deleteFlag);

        ///
        ///<summary>
        ///sqlite_vfs Delegates
        ///
        ///</summary>

        public delegate int dxOpen(sqlite3_vfs vfs, string zName, sqlite3_file db, int flags, out int pOutFlags);

        public delegate int dxDelete(sqlite3_vfs vfs, string zName, int syncDir);

        public delegate int dxAccess(sqlite3_vfs vfs, string zName, int flags, out int pResOut);

        public delegate int dxFullPathname(sqlite3_vfs vfs, string zName, int nOut, StringBuilder zOut);

        public delegate HANDLE dxDlOpen(sqlite3_vfs vfs, string zFilename);

        public delegate int dxDlError(sqlite3_vfs vfs, int nByte, string zErrMsg);

        public delegate HANDLE dxDlSym(sqlite3_vfs vfs, HANDLE data, string zSymbol);

        public delegate int dxDlClose(sqlite3_vfs vfs, HANDLE data);

        public delegate int dxRandomness(sqlite3_vfs vfs, int nByte, byte[] buffer);

        public delegate int dxSleep(sqlite3_vfs vfs, int microseconds);

        public delegate int dxCurrentTime(sqlite3_vfs vfs, ref double currenttime);

        public delegate int dxGetLastError(sqlite3_vfs pVfs, int nBuf, ref string zBuf);

        public delegate int dxCurrentTimeInt64(sqlite3_vfs pVfs, ref sqlite3_int64 pTime);

        public delegate int dxSetSystemCall(sqlite3_vfs pVfs, string zName, sqlite3_int64 sqlite3_syscall_ptr);

        public delegate int dxGetSystemCall(sqlite3_vfs pVfs, string zName, sqlite3_int64 sqlite3_syscall_ptr);

        public delegate int dxNextSystemCall(sqlite3_vfs pVfs, string zName, sqlite3_int64 sqlite3_syscall_ptr);


























        //typedef struct sqlite3_mutex sqlite3_mutex;
        ///<summary>
        /// CAPI3REF: OS Interface Object
        ///
        /// An instance of the sqlite3_vfs object defines the interface between
        /// the SQLite core and the underlying operating system.  The "vfs"
        /// in the name of the object stands for "virtual file system".  See
        /// the [VFS | VFS documentation] for further information.
        ///
        /// The value of the iVersion field is initially 1 but may be larger in
        /// future versions of SQLite.  Additional fields may be appended to this
        /// object when the iVersion value is increased.  Note that the structure
        /// of the sqlite3_vfs object changes in the transaction between
        /// SQLite version 3.5.9 and 3.6.0 and yet the iVersion field was not
        /// modified.
        ///
        /// The szOsFile field is the size of the subclassed [sqlite3_file]
        /// structure used by this VFS.  mxPathname is the maximum length of
        /// a pathname in this VFS.
        ///
        /// Registered sqlite3_vfs objects are kept on a linked list formed by
        /// the pNext pointer.  The [sqlite3_vfs_register()]
        /// and [sqlite3_vfs_unregister()] interfaces manage this list
        /// in a thread-safe way.  The [sqlite3_vfs_find()] interface
        /// searches the list.  Neither the application code nor the VFS
        /// implementation should use the pNext pointer.
        ///
        /// The pNext field is the only field in the sqlite3_vfs
        /// structure that SQLite will ever modify.  SQLite will only access
        /// or modify this field while holding a particular static mutex.
        /// The application should never modify anything within the sqlite3_vfs
        /// object once the object has been registered.
        ///
        /// The zName field holds the name of the VFS module.  The name must
        /// be unique across all VFS modules.
        ///
        /// [[sqlite3_vfs.xOpen]
        /// ^SQLite guarantees that the zFilename parameter to xOpen
        /// is either a NULL pointer or string obtained
        /// from xFullPathname() with an optional suffix added.
        /// ^If a suffix is added to the zFilename parameter, it will
        /// consist of a single "-" character followed by no more than
        /// 10 alphanumeric and/or "-" characters.
        /// ^SQLite further guarantees that
        /// the string will be valid and unchanged until xClose() is
        /// called. Because of the previous sentence,
        /// the [sqlite3_file] can safely store a pointer to the
        /// filename if it needs to remember the filename for some reason.
        /// If the zFilename parameter to xOpen is a NULL pointer then xOpen
        /// must invent its own temporary name for the file.  ^Whenever the
        /// xFilename parameter is NULL it will also be the case that the
        /// flags parameter will include [SQLITE_OPEN_DELETEONCLOSE].
        ///
        /// The flags argument to xOpen() includes all bits set in
        /// the flags argument to [sqlite3_open_v2()].  Or if [sqlite3_open()]
        /// or [sqlite3_open16()] is used, then flags includes at least
        /// [SQLITE_OPEN_READWRITE] | [SQLITE_OPEN_CREATE].
        /// If xOpen() opens a file read-only then it sets *pOutFlags to
        /// include [SQLITE_OPEN_READONLY].  Other bits in *pOutFlags may be set.
        ///
        /// ^(SQLite will also add one of the following flags to the xOpen()
        /// call, depending on the object being opened:
        ///
        /// <ul>
        /// <li>  [SQLITE_OPEN_MAIN_DB]
        /// <li>  [SQLITE_OPEN_MAIN_JOURNAL]
        /// <li>  [SQLITE_OPEN_TEMP_DB]
        /// <li>  [SQLITE_OPEN_TEMP_JOURNAL]
        /// <li>  [SQLITE_OPEN_TRANSIENT_DB]
        /// <li>  [SQLITE_OPEN_SUBJOURNAL]
        /// <li>  [SQLITE_OPEN_MASTER_JOURNAL]
        /// <li>  [SQLITE_OPEN_WAL]
        /// </ul>)^
        ///
        /// The file I/O implementation can use the object type flags to
        /// change the way it deals with files.  For example, an application
        /// that does not care about crash recovery or rollback might make
        /// the open of a journal file a no-op.  Writes to this journal would
        /// also be no-ops, and any attempt to read the journal would return
        /// SQLITE_IOERR.  Or the implementation might recognize that a database
        /// file will be doing page-aligned sector reads and writes in a random
        /// order and set up its I/O subsystem accordingly.
        ///
        /// SQLite might also add one of the following flags to the xOpen method:
        ///
        /// <ul>
        /// <li> [SQLITE_OPEN_DELETEONCLOSE]
        /// <li> [SQLITE_OPEN_EXCLUSIVE]
        /// </ul>
        ///
        /// The [SQLITE_OPEN_DELETEONCLOSE] flag means the file should be
        /// deleted when it is closed.  ^The [SQLITE_OPEN_DELETEONCLOSE]
        /// will be set for TEMP databases and their journals, transient
        /// databases, and subjournals.
        ///
        /// ^The [SQLITE_OPEN_EXCLUSIVE] flag is always used in conjunction
        /// with the [SQLITE_OPEN_CREATE] flag, which are both directly
        /// analogous to the O_EXCL and O_CREAT flags of the POSIX open()
        /// API.  The SQLITE_OPEN_EXCLUSIVE flag, when paired with the
        /// SQLITE_OPEN_CREATE, is used to indicate that file should always
        /// be created, and that it is an error if it already exists.
        /// It is <i>not</i> used to indicate the file should be opened
        /// for exclusive access.
        ///
        /// ^At least szOsFile bytes of memory are allocated by SQLite
        /// to hold the  [sqlite3_file] structure passed as the third
        /// argument to xOpen.  The xOpen method does not have to
        /// allocate the structure; it should just fill it in.  Note that
        /// the xOpen method must set the sqlite3_file.pMethods to either
        /// a valid [sqlite3_io_methods] object or to NULL.  xOpen must do
        /// this even if the open fails.  SQLite expects that the sqlite3_file.pMethods
        /// element will be valid after xOpen returns regardless of the success
        /// or failure of the xOpen call.
        ///
        /// [[sqlite3_vfs.xAccess]
        /// ^The flags argument to xAccess() may be [SQLITE_ACCESS_EXISTS]
        /// to test for the existence of a file, or [SQLITE_ACCESS_READWRITE] to
        /// test whether a file is readable and writable, or [SQLITE_ACCESS_READ]
        /// to test whether a file is at least readable.   The file can be a
        /// directory.
        ///
        /// ^SQLite will always allocate at least mxPathname+1 bytes for the
        /// output buffer xFullPathname.  The exact size of the output buffer
        /// is also passed as a parameter to both  methods. If the output buffer
        /// is not large enough, [SQLITE_CANTOPEN] should be returned. Since this is
        /// handled as a fatal error by SQLite, vfs implementations should endeavor
        /// to prevent this by setting mxPathname to a sufficiently large value.
        ///
        /// The xRandomness(), xSleep(), xCurrentTime(), and xCurrentTimeInt64()
        /// interfaces are not strictly a part of the filesystem, but they are
        /// included in the VFS structure for completeness.
        /// The xRandomness() function attempts to return nBytes bytes
        /// of good-quality randomness into zOut.  The return value is
        /// the actual number of bytes of randomness obtained.
        /// The xSleep() method causes the calling thread to sleep for at
        /// least the number of microseconds given.  ^The xCurrentTime()
        /// method returns a Julian Day Number for the current date and time as
        /// a floating point value.
        /// ^The xCurrentTimeInt64() method returns, as an integer, the Julian
        /// Day Number multiplied by 86400000 (the number of milliseconds in
        /// a 24-hour day).
        /// ^SQLite will use the xCurrentTimeInt64() method to get the current
        /// date and time if that method is available (if iVersion is 2 or
        /// greater and the function pointer is not NULL) and will fall back
        /// to xCurrentTime() if xCurrentTimeInt64() is unavailable.
        ///
        /// ^The xSetSystemCall(), xGetSystemCall(), and xNestSystemCall() interfaces
        /// are not used by the SQLite core.  These optional interfaces are provided
        /// by some VFSes to facilitate testing of the VFS code. By overriding
        /// system calls with functions under its control, a test program can
        /// simulate faults and error conditions that would otherwise be difficult
        /// or impossible to induce.  The set of system calls that can be overridden
        /// varies from one VFS to another, and from one version of the same VFS to the
        /// next.  Applications that use these interfaces must be prepared for any
        /// or all of these interfaces to be NULL or for their behavior to change
        /// from one release to the next.  Applications must not attempt to access
        /// any of these methods if the iVersion of the VFS is less than 3.
        ///
        ///</summary>
        //typedef struct sqlite3_vfs sqlite3_vfs;
        //typedef void (*sqlite3_syscall_ptr)(void);
        //struct sqlite3_vfs {
        //  int iVersion;            /* Structure version number (currently 3) */
        //  int szOsFile;            /* Size of subclassed sqlite3_file */
        //  int mxPathname;          /* Maximum file pathname length */
        //  sqlite3_vfs *pNext;      /* Next registered VFS */
        //  string zName;       /* Name of this virtual file system */
        //  void *pAppData;          /* Pointer to application-specific data */
        //  int (*xOpen)(sqlite3_vfs*, string zName, sqlite3_file*,
        //               int flags, int *pOutFlags);
        //  int (*xDelete)(sqlite3_vfs*, string zName, int syncDir);
        //  int (*xAccess)(sqlite3_vfs*, string zName, int flags, int *pResOut);
        //  int (*xFullPathname)(sqlite3_vfs*, string zName, int nOut, string zOut);
        //  void *(*xDlOpen)(sqlite3_vfs*, string zFilename);
        //  void (*xDlError)(sqlite3_vfs*, int nByte, string zErrMsg);
        //  void (*(*xDlSym)(sqlite3_vfs*,void*, string zSymbol))(void);
        //  void (*xDlClose)(sqlite3_vfs*, void);
        //  int (*xRandomness)(sqlite3_vfs*, int nByte, string zOut);
        //  int (*xSleep)(sqlite3_vfs*, int microseconds);
        //  int (*xCurrentTime)(sqlite3_vfs*, double);
        //  int (*xGetLastError)(sqlite3_vfs*, int, char );
        //  /*
        //  ** The methods above are in version 1 of the sqlite_vfs object
        //  ** definition.  Those that follow are added in version 2 or later
        //  */
        //  int (*xCurrentTimeInt64)(sqlite3_vfs*, sqlite3_int64);
        //  /*
        //  ** The methods above are in versions 1 and 2 of the sqlite_vfs object.
        //  ** New fields may be appended in figure versions.  The iVersion
        //  ** value will increment whenever this happens. 
        //  */
        //};
        public class sqlite3_vfs
        {


            public int iVersion;

            ///
            ///<summary>
            ///Structure version number (currently 3) 
            ///</summary>

            public int szOsFile;

            ///
            ///<summary>
            ///Size of subclassed sqlite3_file 
            ///</summary>

            public int mxPathname;

            ///
            ///<summary>
            ///Maximum file pathname length 
            ///</summary>

            public sqlite3_vfs pNext;

            ///
            ///<summary>
            ///Next registered VFS 
            ///</summary>

            public string zName;

            ///
            ///<summary>
            ///Name of this virtual file system 
            ///</summary>

            public object pAppData;

            ///
            ///<summary>
            ///</summary>
            ///<param name="Pointer to application">specific data </param>

            public dxOpen xOpen;

            public dxDelete xDelete;

            public dxAccess xAccess;

            public dxFullPathname xFullPathname;

            public dxDlOpen xDlOpen;

            public dxDlError xDlError;

            public dxDlSym xDlSym;

            public dxDlClose xDlClose;

            public dxRandomness xRandomness;

            public dxSleep xSleep;

            public dxCurrentTime xCurrentTime;

            public dxGetLastError xGetLastError;

            ///
            ///<summary>
            ///The methods above are in version 1 of the sqlite_vfs object
            ///definition.  Those that follow are added in version 2 or later
            ///
            ///</summary>

            public dxCurrentTimeInt64 xCurrentTimeInt64;

            ///
            ///<summary>
            ///The methods above are in versions 1 and 2 of the sqlite_vfs object.
            ///Those below are for version 3 and greater.
            ///
            ///</summary>

            //int (*xSetSystemCall)(sqlite3_vfs*, string zName, sqlite3_syscall_ptr);
            public dxSetSystemCall xSetSystemCall;

            //sqlite3_syscall_ptr (*xGetSystemCall)(sqlite3_vfs*, string zName);
            public dxGetSystemCall xGetSystemCall;

            //string (*xNextSystemCall)(sqlite3_vfs*, string zName);
            public dxNextSystemCall xNextSystemCall;

            ///
            ///<summary>
            ///The methods above are in versions 1 through 3 of the sqlite_vfs object.
            ///New fields may be appended in figure versions.  The iVersion
            ///value will increment whenever this happens. 
            ///
            ///</summary>

            ///<summary>
            ///New fields may be appended in figure versions.  The iVersion
            /// value will increment whenever this happens.
            ///</summary>
            public sqlite3_vfs()
            {
            }

            public sqlite3_vfs(int iVersion, int szOsFile, int mxPathname, sqlite3_vfs pNext, string zName, object pAppData, dxOpen xOpen, dxDelete xDelete, dxAccess xAccess, dxFullPathname xFullPathname, dxDlOpen xDlOpen, dxDlError xDlError, dxDlSym xDlSym, dxDlClose xDlClose, dxRandomness xRandomness, dxSleep xSleep, dxCurrentTime xCurrentTime, dxGetLastError xGetLastError, dxCurrentTimeInt64 xCurrentTimeInt64, dxSetSystemCall xSetSystemCall, dxGetSystemCall xGetSystemCall, dxNextSystemCall xNextSystemCall)
            {
                this.iVersion = iVersion;
                this.szOsFile = szOsFile;
                this.mxPathname = mxPathname;
                this.pNext = pNext;
                this.zName = zName;
                this.pAppData = pAppData;
                this.xOpen = xOpen;
                this.xDelete = xDelete;
                this.xAccess = xAccess;
                this.xFullPathname = xFullPathname;
                this.xDlOpen = xDlOpen;
                this.xDlError = xDlError;
                this.xDlSym = xDlSym;
                this.xDlClose = xDlClose;
                this.xRandomness = xRandomness;
                this.xSleep = xSleep;
                this.xCurrentTime = xCurrentTime;
                this.xGetLastError = xGetLastError;
                this.xCurrentTimeInt64 = xCurrentTimeInt64;
            }

            public void CopyTo(sqlite3_vfs ct)
            {
                ct.iVersion = this.iVersion;
                ct.szOsFile = this.szOsFile;
                ct.mxPathname = this.mxPathname;
                ct.pNext = this.pNext;
                ct.zName = this.zName;
                ct.pAppData = this.pAppData;
                ct.xOpen = this.xOpen;
                ct.xDelete = this.xDelete;
                ct.xAccess = this.xAccess;
                ct.xFullPathname = this.xFullPathname;
                ct.xDlOpen = this.xDlOpen;
                ct.xDlError = this.xDlError;
                ct.xDlSym = this.xDlSym;
                ct.xDlClose = this.xDlClose;
                ct.xRandomness = this.xRandomness;
                ct.xSleep = this.xSleep;
                ct.xCurrentTime = this.xCurrentTime;
                ct.xGetLastError = this.xGetLastError;
                ct.xCurrentTimeInt64 = this.xCurrentTimeInt64;
            }




#if SQLITE_ENABLE_ATOMIC_WRITE
																																																												//  int sqlite3JournalOpen(sqlite3_vfs *, string , sqlite3_file *, int, int);
//  int sqlite3JournalSize(sqlite3_vfs );
//  int sqlite3JournalCreate(sqlite3_file );
#else
            //#define sqlite3JournalSize(pVfs) ((pVfs)->szOsFile)
            public int sqlite3JournalSize()
            {
                return this.szOsFile;
            }
#endif
        }

        ///
        ///<summary>
        ///CAPI3REF: Flags for the xAccess VFS method
        ///
        ///These integer constants can be used as the third parameter to
        ///the xAccess method of an [sqlite3_vfs] object.  They determine
        ///what kind of permissions the xAccess method is looking for.
        ///With SQLITE_ACCESS_EXISTS, the xAccess method
        ///simply checks whether the file exists.
        ///With SQLITE_ACCESS_READWRITE, the xAccess method
        ///checks whether the named directory is both readable and writable
        ///(in other words, if files can be added, removed, and renamed within
        ///the directory).
        ///The SQLITE_ACCESS_READWRITE constant is currently used only by the
        ///[temp_store_directory pragma], though this could change in a future
        ///release of SQLite.
        ///With SQLITE_ACCESS_READ, the xAccess method
        ///checks whether the file is readable.  The SQLITE_ACCESS_READ constant is
        ///currently unused, though it might be used in a future release of
        ///SQLite.
        ///
        ///</summary>

        //#define SQLITE_ACCESS_EXISTS    0
        //#define SQLITE_ACCESS_READWRITE 1   /* Used by PRAGMA temp_store_directory */
        //#define SQLITE_ACCESS_READ      2   /* Unused */
        private const int SQLITE_ACCESS_EXISTS = 0;

        private const int SQLITE_ACCESS_READWRITE = 1;

        private const int SQLITE_ACCESS_READ = 2;

        ///
        ///<summary>
        ///CAPI3REF: Flags for the xShmLock VFS method
        ///
        ///These integer constants define the various locking operations
        ///allowed by the xShmLock method of [sqlite3_io_methods].  The
        ///following are the only legal combinations of flags to the
        ///xShmLock method:
        ///
        ///<ul>
        ///<li>  SQLITE_SHM_LOCK | SQLITE_SHM_SHARED
        ///<li>  SQLITE_SHM_LOCK | SQLITE_SHM_EXCLUSIVE
        ///<li>  SQLITE_SHM_UNLOCK | SQLITE_SHM_SHARED
        ///<li>  SQLITE_SHM_UNLOCK | SQLITE_SHM_EXCLUSIVE
        ///</ul>
        ///
        ///When unlocking, the same SHARED or EXCLUSIVE flag must be supplied as
        ///was given no the corresponding lock.  
        ///
        ///The xShmLock method can transition between unlocked and SHARED or
        ///between unlocked and EXCLUSIVE.  It cannot transition between SHARED
        ///and EXCLUSIVE.
        ///
        ///</summary>

        //#define SQLITE_SHM_UNLOCK       1
        //#define SQLITE_SHM_LOCK         2
        //#define SQLITE_SHM_SHARED       4
        //#define SQLITE_SHM_EXCLUSIVE    8
        private const int SQLITE_SHM_UNLOCK = 1;

        private const int SQLITE_SHM_LOCK = 2;

        private const int SQLITE_SHM_SHARED = 4;

        private const int SQLITE_SHM_EXCLUSIVE = 8;

    }






}
