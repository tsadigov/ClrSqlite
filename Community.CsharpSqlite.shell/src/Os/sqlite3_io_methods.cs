using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Community.CsharpSqlite.Os
{
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
}
