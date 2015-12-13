using Community.CsharpSqlite.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Community.CsharpSqlite.Cache
{

    ///<summary>
    /// Free slots in the allocator used to divide up the buffer provided using
    /// the SQLITE_CONFIG_PAGECACHE mechanism.
    ///
    ///</summary>
    public class PgFreeslot:ILinkedListNode<PgFreeslot>
    {
        ///<summary>
        ///Next free slot 
        ///</summary>
        public PgFreeslot pNext { get; set; }

        ///<summary>
        ///Next Free Header 
        ///</summary>
        public PgHdr _PgHdr;
    };
}
