using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using FILE=System.IO.TextWriter;
using i64=System.Int64;
using u8=System.Byte;
using u16=System.UInt16;
using u32=System.UInt32;
using u64=System.UInt64;
using unsigned=System.UIntPtr;
using Pgno=System.UInt32;
using i32=System.Int32;
#if !SQLITE_MAX_VARIABLE_NUMBER
using ynVar=System.Int16;
#else
using ynVar = System.Int32; 
#endif
/*
** The yDbMask datatype for the bitmask of all attached databases.
*/
#if SQLITE_MAX_ATTACHED
//  typedef sqlite3_uint64 yDbMask;
using yDbMask = System.Int64; 
#else
//  typedef unsigned int yDbMask;
using yDbMask=System.Int32;
#endif
namespace Community.CsharpSqlite
{
    using Op = VdbeOp;
    using System.Text;
    using sqlite3_value = Sqlite3.Mem;
    using System.Collections.Generic;
    public partial class Sqlite3
    {
        /*
    ** 2003 September 6
    **
    ** The author disclaims copyright to this source code.  In place of
    ** a legal notice, here is a blessing:
    **
    **    May you do good and not evil.
    **    May you find forgiveness for yourself and forgive others.
    **    May you share freely, never taking more than you give.
    **
    *************************************************************************
    ** This is the header file for information that is private to the
    ** VDBE.  This information used to all be at the top of the single
    ** source code file "vdbe.c".  When that file became too big (over
    ** 6000 lines long) it was split up into several smaller files and
    ** this header information was factored out.
    *************************************************************************
    **  Included in SQLite3 port to C#-SQLite;  2008 Noah B Hart
    **  C#-SQLite is an independent reimplementation of the SQLite software library
    **
    **  SQLITE_SOURCE_ID: 2011-06-23 19:49:22 4374b7e83ea0a3fbc3691f9c0c936272862f32f2
    **
    *************************************************************************
    */
        //#if !_VDBEINT_H_
        //#define _VDBEINT_H_
        /*
    ** SQL is translated into a sequence of instructions to be
    ** executed by a virtual machine.  Each instruction is an instance
    ** of the following structure.
    */
        //typedef struct VdbeOp Op;
        ///<summary>
        /// Boolean values
        ///
        ///</summary>
        //typedef unsigned char Bool;
        /*
    ** A cursor is a pointer into a single BTree within a database file.
    ** The cursor can seek to a BTree entry with a particular key, or
    ** loop over all entries of the Btree.  You can also insert new BTree
    ** entries or retrieve the key or data from the entry that the cursor
    ** is currently pointing to.
    **
    ** Every cursor that the virtual machine has open is represented by an
    ** instance of the following structure.
    */
        public class VdbeCursor
        {
            public VdbeCursor()
            {
            }
            public BtCursor pCursor;
            /* The cursor structure of the backend */
            public Btree pBt;
            /* Separate file holding temporary table */
            public KeyInfo pKeyInfo;
            /* Info about index keys needed by index cursors */
            public int iDb;
            /* Index of cursor database in db->aDb[] (or -1) */
            public int pseudoTableReg;
            /* Register holding pseudotable content. */
            public int nField;
            /* Number of fields in the header */
            public bool zeroed;
            /* True if zeroed out and ready for reuse */
            public bool rowidIsValid;
            /* True if lastRowid is valid */
            public bool atFirst;
            /* True if pointing to first entry */
            public bool useRandomRowid;
            /* Generate new record numbers semi-randomly */
            public bool nullRow;
            /* True if pointing to a row with no data */
            public bool deferredMoveto;
            /* A call to sqlite3BtreeMoveto() is needed */
            public bool isTable;
            /* True if a table requiring integer keys */
            public bool isIndex;
            /* True if an index containing keys only - no data */
            public bool isOrdered;
            /* True if the underlying table is BTREE_UNORDERED */
#if !SQLITE_OMIT_VIRTUALTABLE
            public sqlite3_vtab_cursor pVtabCursor;
            /* The cursor for a virtual table */
            public sqlite3_module pModule;
            /* Module for cursor pVtabCursor */
#endif
            public i64 seqCount;
            /* Sequence counter */
            public i64 movetoTarget;
            /* Argument to the deferred sqlite3BtreeMoveto() */
            public i64 lastRowid;
            /* Last rowid from a Next or NextIdx operation */
            /* Result of last sqlite3BtreeMoveto() done by an OP_NotExists or
** OP_IsUnique opcode on this cursor. */
            public int seekResult;
            /* Cached information about the header for the data record that the
      ** cursor is currently pointing to.  Only valid if cacheStatus matches
      ** Vdbe.cacheCtr.  Vdbe.cacheCtr will never take on the value of
      ** CACHE_STALE and so setting cacheStatus=CACHE_STALE guarantees that
      ** the cache is out of date.
      **
      ** aRow might point to (ephemeral) data for the current row, or it might
      ** be NULL.
      */
            public u32 cacheStatus;
            /* Cache is valid if this matches Vdbe.cacheCtr */
            public Pgno payloadSize;
            /* Total number of bytes in the record */
            public u32[] aType;
            /* Type values for all entries in the record */
            public u32[] aOffset;
            ///<summary>
            ///Cached offsets to the start of each columns data
            ///</summary>
            public int aRow;
            /* Pointer to Data for the current row, if all on one page */
            public VdbeCursor Copy()
            {
                return (VdbeCursor)MemberwiseClone();
            }
        };

        //#define VdbeFrameMem(p) ((Mem )&((u8 )p)[ROUND8(sizeof(VdbeFrame))])
        ///<summary>
        /// A value for VdbeCursor.cacheValid that means the cache is always invalid.
        ///
        ///</summary>
        const int CACHE_STALE = 0;
        /*
    ** Internally, the vdbe manipulates nearly all SQL values as Mem
    ** structures. Each Mem struct may cache multiple representations (string,
    ** integer etc.) of the same value.
    */
        public class Mem
        {
            public sqlite3 db;
            /* The associated database connection */
            public string z;
            ///<summary>
            ///String value
            ///</summary>
            public double r;
            /* Real value */
            public struct union_ip
            {
#if DEBUG_CLASS_MEM || DEBUG_CLASS_ALL
																																																																																												public i64 _i;              /* First operand */
public i64 i
{
get { return _i; }
set { _i = value; }
}
#else
                public i64 i;
                /* Integer value used when MEM_Int is set in flags */
#endif
                public int nZero;
                /* Used when bit MEM_Zero is set in flags */
                public FuncDef pDef;
                /* Used only when flags==MEM_Agg */
                public RowSet pRowSet;
                /* Used only when flags==MEM_RowSet */
                public VdbeFrame pFrame;
                /* Used when flags==MEM_Frame */
            }
            public union_ip u;
            public byte[] zBLOB;
            /* BLOB value */
            public int n;
            /* Number of characters in string value, excluding '\0' */
#if DEBUG_CLASS_MEM || DEBUG_CLASS_ALL
																																																																					public u16 _flags;              /* First operand */
public u16 flags
{
get { return _flags; }
set { _flags = value; }
}
#else
            public u16 flags;
            public MemFlags Flags
            {
                get
                {
                    return (MemFlags)flags;
                }
                set
                {
                    flags = (u16)value;
                }
            }
            /* Some combination of MEM_Null, MEM_Str, MEM_Dyn, etc. */
#endif
            public u8 type;
            public ValType ValType
            {
                get
                {
                    return (ValType)type;
                }
                set
                {
                    flags = (u8)value;
                }
            }
            /* One of SQLITE_NULL, SQLITE_TEXT, SQLITE_INTEGER, etc */
            public SqliteEncoding enc;
            /* SqliteEncoding.UTF8, SqliteEncoding.UTF16BE, SqliteEncoding.UTF16LE */
#if SQLITE_DEBUG
																																																																					      public Mem pScopyFrom;        /* This Mem is a shallow copy of pScopyFrom */
      public object pFiller;        /* So that sizeof(Mem) is a multiple of 8 */
#endif
            public dxDel xDel;
            /* If not null, call this function to delete Mem.z */
            // Not used under c#
            //public string zMalloc;      /* Dynamic buffer allocated by sqlite3Malloc() */
            public Mem _Mem;
            /* Used when C# overload Z as MEM space */
            public SumCtx _SumCtx;
            /* Used when C# overload Z as Sum context */
            public SubProgram[] _SubProgram;
            /* Used when C# overload Z as SubProgram*/
            public StrAccum _StrAccum;
            /* Used when C# overload Z as STR context */
            public object _MD5Context;
            ///<summary>
            ///Used when C# overload Z as MD5 context
            ///</summary>
            public Mem()
            {
            }
            public Mem(sqlite3 db, string z, double r, int i, int n, u16 flags, u8 type, SqliteEncoding enc
#if SQLITE_DEBUG
																																																																					         , Mem pScopyFrom, object pFiller  /* pScopyFrom, pFiller */
#endif
)
            {
                this.db = db;
                this.z = z;
                this.r = r;
                this.u.i = i;
                this.n = n;
                this.flags = flags;
#if SQLITE_DEBUG
																																																																																												        this.pScopyFrom = pScopyFrom;
        this.pFiller = pFiller;
#endif
                this.type = type;
                this.enc = enc;
            }
            public void CopyTo(ref Mem ct)
            {
                if (ct == null)
                    ct = new Mem();
                ct.u = u;
                ct.r = r;
                ct.db = db;
                ct.z = z;
                if (zBLOB == null)
                    ct.zBLOB = null;
                else
                {
                    ct.zBLOB = sqlite3Malloc(zBLOB.Length);
                    Buffer.BlockCopy(zBLOB, 0, ct.zBLOB, 0, zBLOB.Length);
                }
                ct.n = n;
                ct.flags = flags;
                ct.type = type;
                ct.enc = enc;
                ct.xDel = xDel;
            }
            public///<summary>
                /// Return true if a memory cell is not marked as invalid.  This macro
                /// is for use inside Debug.Assert() statements only.
                ///
                ///</summary>
#if SQLITE_DEBUG
																																																													    //define memIsValid(M)  ((M)->flags & MEM_Invalid)==0
    static bool memIsValid( Mem M )
    {
      return ( ( M ).flags & MEM_Invalid ) == 0;
    }
#else
            bool memIsValid()
            {
                return true;
            }
            public int sqlite3VdbeMemExpandBlob()
            {
                return SQLITE_OK;
            }
            public///<summary>
                /// Clear any existing type flags from a Mem and replace them with f
                ///</summary>
                //#define MemSetTypeFlag(p, f) \
                //   ((p)->flags = ((p)->flags&~(MEM_TypeMask|MEM_Zero))|f)
            void MemSetTypeFlag(int f)
            {
                this.flags = (u16)(this.flags & ~(MEM_TypeMask | MEM_Zero) | f);
            }
        }
        // TODO -- Convert back to inline for speed
#endif
        ///<summary>
        ///A VdbeFunc is just a FuncDef (defined in sqliteInt.h) that contains
        /// additional information about auxiliary information bound to arguments
        /// of the function.  This is used to implement the sqlite3_get_auxdata()
        /// and sqlite3_set_auxdata() APIs.  The "auxdata" is some auxiliary data
        /// that can be associated with a constant argument to a function.  This
        /// allows functions such as "regexp" to compile their constant regular
        /// expression argument once and reused the compiled code for multiple
        /// invocations.
        ///</summary>
        public class AuxData
        {
            public object pAux;
            /* Aux data for the i-th argument */
            //(void );      /* Destructor for the aux data */
        };

        public class VdbeFunc : FuncDef
        {
            public FuncDef pFunc;
            /* The definition of the function */
            public int nAux;
            /* Number of entries allocated for apAux[] */
            public AuxData[] apAux = new AuxData[2];
            /* One slot for each function argument */
        };

        ///<summary>
        /// The "context" argument for a installable function.  A pointer to an
        /// instance of this structure is the first argument to the routines used
        /// implement the SQL functions.
        ///
        /// There is a typedef for this structure in sqlite.h.  So all routines,
        /// even the public interface to SQLite, can use a pointer to this structure.
        /// But this file is the only place where the internal details of this
        /// structure are known.
        ///
        /// This structure is defined inside of vdbeInt.h because it uses substructures
        /// (Mem) which are only defined there.
        ///
        ///</summary>
        public class sqlite3_context
        {
            public FuncDef pFunc;
            /* Pointer to function information.  MUST BE FIRST */
            public VdbeFunc pVdbeFunc;
            /* Auxilary data, if created. */
            public Mem s = new Mem();
            /* The return value is stored here */
            public Mem pMem;
            /* Memory cell used to store aggregate context */
            public int isError;
            /* Error code returned by the function. */
            public CollSeq pColl;
            /* Collating sequence */
            public///<summary>
                /// sqlite3_result_  
                /// The following routines are used by user-defined functions to specify
                /// the function result.
                ///
                /// The setStrOrError() funtion calls sqlite3VdbeMemSetStr() to store the
                /// result as a string or blob but if the string or blob is too large, it
                /// then sets the error code to SQLITE_TOOBIG
                ///
                ///</summary>
                void setResultStrOrError(/* Function context */string z,/* String pointer */int o,/* offset into string */int n,/* Bytes in string, or negative */SqliteEncoding enc,/* Encoding of z.  0 for BLOBs */dxDel xDel//void (*xDel)(void)     /* Destructor function */
                )
            {
                if (sqlite3VdbeMemSetStr(this.s, z, o, n, enc, xDel) == SQLITE_TOOBIG)
                {
                    this.sqlite3_result_error_toobig();
                }
            }
            public void setResultStrOrError(/* Function context */string z,/* String pointer */int n,/* Bytes in string, or negative */SqliteEncoding enc,/* Encoding of z.  0 for BLOBs */dxDel xDel//void (*xDel)(void)     /* Destructor function */
            )
            {
                if (sqlite3VdbeMemSetStr(this.s, z, n, enc, xDel) == SQLITE_TOOBIG)
                {
                    this.sqlite3_result_error_toobig();
                }
            }
            public void sqlite3_result_blob(string z, int n, dxDel xDel)
            {
                Debug.Assert(n >= 0);
                Debug.Assert(sqlite3_mutex_held(this.s.db.mutex));
                this.setResultStrOrError(z, n, 0, xDel);
            }
            public void sqlite3_result_double(double rVal)
            {
                Debug.Assert(sqlite3_mutex_held(this.s.db.mutex));
                sqlite3VdbeMemSetDouble(this.s, rVal);
            }
            public void sqlite3_result_error(string z, int n)
            {
                Debug.Assert(sqlite3_mutex_held(this.s.db.mutex));
                this.setResultStrOrError(z, n, SqliteEncoding.UTF8, SQLITE_TRANSIENT);
                this.isError = SQLITE_ERROR;
            }
            public void sqlite3_result_int(int iVal)
            {
                Debug.Assert(sqlite3_mutex_held(this.s.db.mutex));
                sqlite3VdbeMemSetInt64(this.s, (i64)iVal);
            }
            public void sqlite3_result_int64(i64 iVal)
            {
                Debug.Assert(sqlite3_mutex_held(this.s.db.mutex));
                sqlite3VdbeMemSetInt64(this.s, iVal);
            }
            public void sqlite3_result_null()
            {
                Debug.Assert(sqlite3_mutex_held(this.s.db.mutex));
                sqlite3VdbeMemSetNull(this.s);
            }
            public void sqlite3_result_text(string z, int o,//Offset
            int n, dxDel xDel)
            {
                Debug.Assert(sqlite3_mutex_held(this.s.db.mutex));
                this.setResultStrOrError(z, o, n, SqliteEncoding.UTF8, xDel);
            }
            public void sqlite3_result_text(StringBuilder z, int n, dxDel xDel)
            {
                Debug.Assert(sqlite3_mutex_held(this.s.db.mutex));
                this.setResultStrOrError(z.ToString(), n, SqliteEncoding.UTF8, xDel);
            }
            public void sqlite3_result_text(string z, int n, dxDel xDel)
            {
                Debug.Assert(sqlite3_mutex_held(this.s.db.mutex));
                this.setResultStrOrError(z, n, SqliteEncoding.UTF8, xDel);
            }
            public void sqlite3_result_value(sqlite3_value pValue)
            {
                Debug.Assert(sqlite3_mutex_held(this.s.db.mutex));
                sqlite3VdbeMemCopy(this.s, pValue);
            }
            public void sqlite3_result_zeroblob(int n)
            {
                Debug.Assert(sqlite3_mutex_held(this.s.db.mutex));
                sqlite3VdbeMemSetZeroBlob(this.s, n);
            }
            public void sqlite3_result_error_code(int errCode)
            {
                this.isError = errCode;
                if ((this.s.flags & MEM_Null) != 0)
                {
                    this.setResultStrOrError(sqlite3ErrStr(errCode), -1, SqliteEncoding.UTF8, SQLITE_STATIC);
                }
            }
            ///<summary>
            ///Force an SQLITE_TOOBIG error.
            ///</summary>
            public void sqlite3_result_error_toobig()
            {
                Debug.Assert(sqlite3_mutex_held(this.s.db.mutex));
                this.isError = SQLITE_ERROR;
                this.setResultStrOrError("string or blob too big", -1, SqliteEncoding.UTF8, SQLITE_STATIC);
            }
            ///<summary>
            ///An SQLITE_NOMEM error.
            ///</summary>
            public void sqlite3_result_error_nomem()
            {
                Debug.Assert(sqlite3_mutex_held(this.s.db.mutex));
                sqlite3VdbeMemSetNull(this.s);
                this.isError = SQLITE_NOMEM;
                //pCtx.s.db.mallocFailed = 1;
            }
        }
       
        
        /*
    ** The following are allowed values for Vdbe.magic
    */
        //#define VDBE_MAGIC_INIT     0x26bceaa5    /* Building a VDBE program */
        //#define VDBE_MAGIC_RUN      0xbdf20da3    /* VDBE is ready to execute */
        //#define VDBE_MAGIC_HALT     0x519c2973    /* VDBE has completed execution */
        //#define VDBE_MAGIC_DEAD     0xb606c3c8    /* The VDBE has been deallocated */
        const u32 VDBE_MAGIC_INIT = 0x26bceaa5;
        /* Building a VDBE program */
        const u32 VDBE_MAGIC_RUN = 0xbdf20da3;
        /* VDBE is ready to execute */
        const u32 VDBE_MAGIC_HALT = 0x519c2973;
        /* VDBE has completed execution */
        const u32 VDBE_MAGIC_DEAD = 0xb606c3c8;
        /* The VDBE has been deallocated */
        //# define sqlite3VdbeLeave(X)
#if SQLITE_DEBUG
																															    //void sqlite3VdbeMemPrepareToChange(Vdbe*,Mem);
#endif
#if !SQLITE_OMIT_FOREIGN_KEY
        //int sqlite3VdbeCheckFk(Vdbe *, int);
#else
																															// define sqlite3VdbeCheckFk(p,i) 0
static int sqlite3VdbeCheckFk( Vdbe p, int i ) { return 0; }
#endif
        //int sqlite3VdbeMemTranslate(Mem*, u8);
        //#if SQLITE_DEBUG
        //  void sqlite3VdbePrintSql(Vdbe);
        //  void sqlite3VdbeMemPrettyPrint(Mem pMem, string zBuf);
        //#endif
        //int sqlite3VdbeMemHandleBom(Mem pMem);
#if !SQLITE_OMIT_INCRBLOB
																															//  int sqlite3VdbeMemExpandBlob(Mem );
#else
        //  #define sqlite3VdbeMemExpandBlob(x) SQLITE_OK
#endif
        //#endif //* !_VDBEINT_H_) */
    }
}
