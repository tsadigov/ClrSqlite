using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Community.CsharpSqlite.Os
{

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

    public enum SQLITE_SHM
    {
        UNLOCK = 1,
        LOCK = 2,
        SHARED = 4,
        EXCLUSIVE = 8
    }
}
