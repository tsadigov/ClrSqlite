using System;
using System.Diagnostics;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Text;
using Bitmask = System.UInt64;
using i16 = System.Int16;
using i64 = System.Int64;
using sqlite3_int64 = System.Int64;
using u8 = System.Byte;
using u16 = System.UInt16;
using u32 = System.UInt32;
using u64 = System.UInt64;
using unsigned = System.UInt64;
using Pgno = System.UInt32;
using Community.CsharpSqlite.Engine;

namespace Community.CsharpSqlite

{
    namespace Metadata{
        ///<summary>
        /// An object of this type is created for each virtual table present in
        /// the database schema.
        ///
        /// If the database schema is shared, then there is one instance of this
        /// structure for each database connection (sqlite3) that uses the shared
        /// schema. This is because each database connection requires its own unique
        /// instance of the sqlite3_vtab* handle used to access the virtual table
        /// implementation. sqlite3_vtab* handles can not be shared between
        /// database connections, even when the rest of the in-memory database
        /// schema is shared, as the implementation often stores the database
        /// connection handle passed to it via the xConnect() or xCreate() method
        /// during initialization internally. This database connection handle may
        /// then be used by the virtual table implementation to access real tables
        /// within the database. So that they appear as part of the callers
        /// transaction, these accesses need to be made via the same database
        /// connection as that used to execute SQL operations on the virtual table.
        ///
        /// All VTable objects that correspond to a single table in a shared
        /// database schema are initially stored in a linked-list pointed to by
        /// the Table.pVTable member variable of the corresponding Table object.
        /// When an sqlite3_prepare() operation is required to access the virtual
        /// table, it searches the list for the VTable that corresponds to the
        /// database connection doing the preparing so as to use the correct
        /// sqlite3_vtab* handle in the compiled query.
        ///
        /// When an in-memory Table object is deleted (for example when the
        /// schema is being reloaded for some reason), the VTable objects are not
        /// deleted and the sqlite3_vtab* handles are not xDisconnect()ed
        /// immediately. Instead, they are moved from the Table.pVTable list to
        /// another linked list headed by the sqlite3.pDisconnect member of the
        /// corresponding sqlite3 structure. They are then deleted/xDisconnected
        /// next time a statement is prepared using said sqlite3*. This is done
        /// to avoid deadlock issues involving multiple sqlite3.mutex mutexes.
        /// Refer to comments above function sqlite3VtabUnlockList() for an
        /// explanation as to why it is safe to add an entry to an sqlite3.pDisconnect
        /// list without holding the corresponding sqlite3.mutex mutex.
        ///
        /// The memory for objects of this type is always allocated by
        /// sqlite3DbMalloc(), using the connection handle stored in VTable.db as
        /// the first argument.
        ///
        ///</summary>
        public class VTable:ILinkedListNode<VTable>
        {
            ///<summary>
            ///Database connection associated with this table 
            ///</summary>
            public Community.CsharpSqlite.Connection db;

            ///<summary>
            ///Pointer to module implementation 
            ///</summary>
            public Module pMod;

            ///<summary>
            ///Pointer to vtab instance 
            ///</summary>
            public sqlite3_vtab pVtab;

            ///<summary>
            ///Number of pointers to this structure 
            ///</summary>
            public int nRef;

            ///<summary>
            ///True if constraints are supported 
            ///</summary>
            public u8 bConstraint;

            ///<summary>
            ///Depth of the SAVEPOINT stack 
            ///</summary>
            public int iSavepoint;

            ///<summary>
            ///Next in linked list (see above) 
            ///</summary>
            public VTable pNext { get; set; }

        };





        
    }
namespace Engine{
    ///<summary>
    /// CAPI3REF: Virtual Table Instance Object
    /// KEYWORDS: sqlite3_vtab
    ///
    /// Every [virtual table module] implementation uses a subclass
    /// of this object to describe a particular instance
    /// of the [virtual table].  Each subclass will
    /// be tailored to the specific needs of the module implementation.
    /// The purpose of this superclass is to define certain fields that are
    /// common to all module implementations.
    ///
    /// ^Virtual tables methods can set an error message by assigning a
    /// string obtained from [io.sqlite3_mprintf()] to zErrMsg.  The method should
    /// take care that any prior string is freed by a call to [malloc_cs.sqlite3_free()]
    /// prior to assigning a new string to zErrMsg.  ^After the error message
    /// is delivered up to the client application, the string will be automatically
    /// freed by malloc_cs.sqlite3_free() and the zErrMsg field will be zeroed.
    ///
    ///</summary>
    //struct sqlite3_vtab {
    //  const sqlite3_module *pModule;  /* The module for this virtual table */
    //  int nRef;                       /* NO LONGER USED */
    //  string zErrMsg;                  /* Error message from io.sqlite3_mprintf() */
    //  /* Virtual table implementations will typically add additional fields */
    //};
    public class sqlite3_vtab
    {
        ///<summary>
        ///The module for this virtual table 
        ///</summary>
        public sqlite3_module pModule;


        ///<summary>
        ///Used internally 
        ///</summary>
        public int nRef;


        ///<summary>
        ///Error message from io.sqlite3_mprintf() 
        ///</summary>
        public string zErrMsg;
        

        ///<summary>
        ///Virtual table implementations will typically add additional fields 
        ///</summary>

    };

}

}