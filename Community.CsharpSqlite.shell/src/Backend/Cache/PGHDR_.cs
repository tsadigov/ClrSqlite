using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Community.CsharpSqlite.Cache
{
    public enum PGHDR
    {
        ///
        ///<summary>
        ///Bit values for PgHdr.flags 
        ///</summary>

        //#define DIRTY             0x002  /* Page has changed */
        //#define NEED_SYNC         0x004  /* Fsync the rollback journal before
        //                                       ** writing this page to the database */
        //#define NEED_READ         0x008  /* Content is unread */
        //#define REUSE_UNLIKELY    0x010  /* A hint that reuse is unlikely */
        //#define DONT_WRITE        0x020  /* Do not write content to disk */
        /*const*/ DIRTY = 0x002,

        ///
        ///<summary>
        ///Page has changed 
        ///</summary>

        /*const*/ NEED_SYNC = 0x004,

        ///
        ///<summary>
        ///Fsync the rollback journal before
        ///writing this page to the database 
        ///</summary>

        /*const*/ NEED_READ = 0x008,

        ///
        ///<summary>
        ///Content is unread 
        ///</summary>

        /*const*/ REUSE_UNLIKELY = 0x010,

        ///
        ///<summary>
        ///A hint that reuse is unlikely 
        ///</summary>

        /*const*/ DONT_WRITE = 0x020
        ///
        ///<summary>
        ///Do not write content to disk 
        ///</summary>
    }

}
