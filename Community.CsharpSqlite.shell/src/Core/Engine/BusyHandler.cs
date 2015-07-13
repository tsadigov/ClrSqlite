using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Community.CsharpSqlite
{
    ///<summary>
    /// An instance of the following structure is used to store the busy-handler
    /// callback for a given sqlite handle.
    ///
    /// The sqlite.busyHandler member of the sqlite struct contains the busy
    /// callback for the database handle. Each pager opened via the sqlite
    /// handle is passed a pointer to sqlite.busyHandler. The busy-handler
    /// callback is currently invoked only from within pager.c.
    ///
    ///</summary>
    //typedef struct BusyHandler BusyHandler;
    public class BusyHandler
    {
        public dxBusy xFunc;
        //)(void *,int);  /* The busy callback */
        public object pArg;
        ///
        ///<summary>
        ///First arg to busy callback 
        ///</summary>
        public int nBusy;
        ///
        ///<summary>
        ///Incremented with each busy call 
        ///</summary>
    };

}
