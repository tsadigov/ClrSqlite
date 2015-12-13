using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Community.CsharpSqlite.Cache
{
    ///
    ///<summary>
    ///Global data used by this cache.
    ///
    ///</summary>

    public class PCacheGlobal
    {
        public PGroup grp;

        ///
        ///<summary>
        ///The global PGroup for mode (2) 
        ///</summary>

        ///
        ///<summary>
        ///Variables related to SQLITE_CONFIG_PAGECACHE settings.  The
        ///szSlot, nSlot, pStart, pEnd, nReserve, and isInit values are all
        ///fixed at sqlite3_initialize() time and do not require mutex protection.
        ///The nFreeSlot and pFree values do require mutex protection.
        ///
        ///</summary>

        public bool isInit;

        ///
        ///<summary>
        ///True if initialized 
        ///</summary>

        public int szSlot;

        ///
        ///<summary>
        ///Size of each free slot 
        ///</summary>

        public int nSlot;

        ///
        ///<summary>
        ///The number of pcache slots 
        ///</summary>

        public int nReserve;

        ///
        ///<summary>
        ///Try to keep nFreeSlot above this 
        ///</summary>

        public object pStart, pEnd;

        ///
        ///<summary>
        ///Bounds of pagecache malloc range 
        ///</summary>

        ///
        ///<summary>
        ///Above requires no mutex.  Use mutex below for variable that follow. 
        ///</summary>

        public sqlite3_mutex mutex;

        ///
        ///<summary>
        ///Mutex for accessing the following: 
        ///</summary>

        public int nFreeSlot;

        ///
        ///<summary>
        ///Number of unused pcache slots 
        ///</summary>

        public PgFreeslot pFree;

        ///
        ///<summary>
        ///Free page blocks 
        ///</summary>

        ///
        ///<summary>
        ///The following value requires a mutex to change.  We skip the mutex on
        ///</summary>
        ///<param name="reading because (1) most platforms read a 32">bit integer atomically and</param>
        ///<param name="(2) even if an incorrect value is read, no great harm is done since this">(2) even if an incorrect value is read, no great harm is done since this</param>
        ///<param name="is really just an optimization. ">is really just an optimization. </param>

        public bool bUnderPressure;

        ///
        ///<summary>
        ///True if low on PAGECACHE memory 
        ///</summary>

        // C#
        public PCacheGlobal()
        {
            grp = new PGroup();
        }
    }

}
