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

#if !SQLITE_MAX_VARIABLE_NUMBER
using ynVar = System.Int16;

#else
using ynVar = System.Int32; 
#endif

namespace Community.CsharpSqlite
{
    using Metadata;
        //typedef struct AggInfo AggInfo;
        //typedef struct AuthContext AuthContext;
        //typedef struct AutoincInfo AutoincInfo;
        //typedef struct Bitvec Bitvec;
        //typedef struct CollSeq CollSeq;
        //typedef struct Column Column;
        //typedef struct Db Db;
        //typedef struct Schema Schema;
        //typedef struct Expr Expr;
        //typedef struct ExprList ExprList;
        //typedef struct ExprSpan ExprSpan;
        //typedef struct FKey FKey;
        //typedef struct FuncDestructor FuncDestructor;
        //typedef struct FuncDef FuncDef;
        //typedef struct IdList IdList;
        //typedef struct Index Index;
        //typedef struct IndexSample IndexSample;
        //typedef struct KeyClass KeyClass;
        //typedef struct KeyInfo KeyInfo;
        //typedef struct Lookaside Lookaside;
        //typedef struct LookasideSlot LookasideSlot;
        //typedef struct Module Module;
        //typedef struct NameContext NameContext;
        //typedef struct Parse Parse;
        //typedef struct RowSet RowSet;
        //typedef struct Savepoint Savepoint;
        //typedef struct Select Select;
        //typedef struct SrcList SrcList;
        //typedef struct StrAccum StrAccum;
        //typedef struct Table Table;
        //typedef struct TableLock TableLock;
        //typedef struct Token Token;
        //typedef struct Trigger Trigger;
        //typedef struct TriggerPrg TriggerPrg;
        //typedef struct TriggerStep TriggerStep;
        //typedef struct UnpackedRecord UnpackedRecord;
        //typedef struct VTable VTable;
        //typedef struct VtabCtx VtabCtx;
        //typedef struct Walker Walker;
        //typedef struct WherePlan WherePlan;
        //typedef struct WhereInfo WhereInfo;
        //typedef struct WhereLevel WhereLevel;
        ///<summary>
        /// Defer sourcing vdbe.h and btree.h until after the "u8" and
        /// "BusyHandler" typedefs. vdbe.h also requires a few of the opaque
        /// pointer types (i.e. FuncDef) defined above.
        ///
        ///</summary>
        //#include "btree.h"
        //#include "vdbe.h"
        //#include "pager.h"
        //#include "pcache_g.h"
        //#include "os.h"
        //#include "mutex.h"
        ///<summary>
        /// Each database file to be accessed by the system is an instance
        /// of the following structure.  There are normally two of these structures
        /// in the sqlite.aDb[] array.  aDb[0] is the main database file and
        /// aDb[1] is the database file used to hold temporary tables.  Additional
        /// databases may be attached.
        ///
        ///</summary>
        public class DbBackend
        {
        ///<summary>
        ///Name of this database  
        ///</summary>
        public string Name;

        ///
        

        ///<summary>
        ///The B Tree structure for this database file  
        ///</summary>
        public tree.Btree BTree;

        ///
        

        ///<summary>
        ///0: not writable.  1: Transaction.  2: Checkpoint  
        ///</summary>
        public u8 inTrans;

        ///
        
        ///<summary>
        ///How aggressive at syncing data to disk  
        ///</summary>

        public u8 safety_level;

        ///
        

        ///<summary>
        ///Pointer to database schema (possibly shared)  
        ///</summary>
        public Schema pSchema;
            

        };

    }
