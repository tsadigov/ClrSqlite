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
namespace Community.CsharpSqlite {
	using Op=VdbeOp;
	using System.Text;
	using sqlite3_value=Sqlite3.Mem;
    using System.Collections.Generic;
	public partial class Sqlite3 {
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
    *///#if !_VDBEINT_H_
		//#define _VDBEINT_H_
		/*
    ** SQL is translated into a sequence of instructions to be
    ** executed by a virtual machine.  Each instruction is an instance
    ** of the following structure.
    *///typedef struct VdbeOp Op;
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
    */public class VdbeCursor {
          public VdbeCursor()
          {

          }
			public BtCursor pCursor;
			/* The cursor structure of the backend */public Btree pBt;
			/* Separate file holding temporary table */public KeyInfo pKeyInfo;
			/* Info about index keys needed by index cursors */public int iDb;
			/* Index of cursor database in db->aDb[] (or -1) */public int pseudoTableReg;
			/* Register holding pseudotable content. */public int nField;
			/* Number of fields in the header */public bool zeroed;
			/* True if zeroed out and ready for reuse */public bool rowidIsValid;
			/* True if lastRowid is valid */public bool atFirst;
			/* True if pointing to first entry */public bool useRandomRowid;
			/* Generate new record numbers semi-randomly */public bool nullRow;
			/* True if pointing to a row with no data */public bool deferredMoveto;
			/* A call to sqlite3BtreeMoveto() is needed */public bool isTable;
			/* True if a table requiring integer keys */public bool isIndex;
			/* True if an index containing keys only - no data */public bool isOrdered;
			/* True if the underlying table is BTREE_UNORDERED */
			#if !SQLITE_OMIT_VIRTUALTABLE
			public sqlite3_vtab_cursor pVtabCursor;
			/* The cursor for a virtual table */public sqlite3_module pModule;
			/* Module for cursor pVtabCursor */
			#endif
			public i64 seqCount;
			/* Sequence counter */public i64 movetoTarget;
			/* Argument to the deferred sqlite3BtreeMoveto() */public i64 lastRowid;
			/* Last rowid from a Next or NextIdx operation *//* Result of last sqlite3BtreeMoveto() done by an OP_NotExists or
      ** OP_IsUnique opcode on this cursor. */public int seekResult;
			/* Cached information about the header for the data record that the
      ** cursor is currently pointing to.  Only valid if cacheStatus matches
      ** Vdbe.cacheCtr.  Vdbe.cacheCtr will never take on the value of
      ** CACHE_STALE and so setting cacheStatus=CACHE_STALE guarantees that
      ** the cache is out of date.
      **
      ** aRow might point to (ephemeral) data for the current row, or it might
      ** be NULL.
      */public u32 cacheStatus;
			/* Cache is valid if this matches Vdbe.cacheCtr */public Pgno payloadSize;
			/* Total number of bytes in the record */public u32[] aType;
			/* Type values for all entries in the record */public u32[] aOffset;
			///<summary>
			///Cached offsets to the start of each columns data
			///</summary>
			public int aRow;
			/* Pointer to Data for the current row, if all on one page */public VdbeCursor Copy() {
				return (VdbeCursor)MemberwiseClone();
			}
		};

		//typedef struct VdbeCursor VdbeCursor;
		///<summary>
		/// When a sub-program is executed (OP_Program), a structure of this type
		/// is allocated to store the current value of the program counter, as
		/// well as the current memory cell array and various other frame specific
		/// values stored in the Vdbe struct. When the sub-program is finished,
		/// these values are copied back to the Vdbe from the VdbeFrame structure,
		/// restoring the state of the VM to as it was before the sub-program
		/// began executing.
		///
		/// The memory for a VdbeFrame object is allocated and managed by a memory
		/// cell in the parent (calling) frame. When the memory cell is deleted or
		/// overwritten, the VdbeFrame object is not freed immediately. Instead, it
		/// is linked into the Vdbe.pDelFrame list. The contents of the Vdbe.pDelFrame
		/// list is deleted when the VM is reset in VdbeHalt(). The reason for doing
		/// this instead of deleting the VdbeFrame immediately is to avoid recursive
		/// calls to sqlite3VdbeMemRelease() when the memory cells belonging to the
		/// child frame are released.
		///
		/// The currently executing frame is stored in Vdbe.pFrame. Vdbe.pFrame is
		/// set to NULL if the currently executing frame is the main program.
		///
		///</summary>
		//typedef struct VdbeFrame VdbeFrame;
		public class VdbeFrame {
            public VdbeFrame()
            {

            }
			public Vdbe v;
			/* VM this frame belongs to */public int pc;
			/* Program Counter in parent (calling) frame */public Op[] aOp;
			/* Program instructions for parent frame */public int nOp;
			/* Size of aOp array */public Mem[] aMem;
			/* Array of memory cells for parent frame */public int nMem;
			/* Number of entries in aMem */public VdbeCursor[] apCsr;
			/* Array of Vdbe cursors for parent frame */public u16 nCursor;
			/* Number of entries in apCsr */public int token;
			/* Copy of SubProgram.token */public int nChildMem;
			/* Number of memory cells for child frame */public int nChildCsr;
			/* Number of cursors for child frame */public i64 lastRowid;
			/* Last insert rowid (sqlite3.lastRowid) */public int nChange;
			/* Statement changes (Vdbe.nChanges)     */public VdbeFrame pParent;
			/* Parent of this frame, or NULL if parent is main *///
			// Needed for C# Implementation
			//
			public Mem[] aChildMem;
			/* Array of memory cells for child frame */public VdbeCursor[] aChildCsr;
		/* Array of cursors for child frame */};

		//#define VdbeFrameMem(p) ((Mem )&((u8 )p)[ROUND8(sizeof(VdbeFrame))])
		///<summary>
		/// A value for VdbeCursor.cacheValid that means the cache is always invalid.
		///
		///</summary>
		const int CACHE_STALE=0;
		/*
    ** Internally, the vdbe manipulates nearly all SQL values as Mem
    ** structures. Each Mem struct may cache multiple representations (string,
    ** integer etc.) of the same value.
    */public class Mem {
			public sqlite3 db;
			/* The associated database connection */public string z;
			///<summary>
			///String value
			///</summary>
			public double r;
			/* Real value */public struct union_ip {
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
				/* Used when bit MEM_Zero is set in flags */public FuncDef pDef;
				/* Used only when flags==MEM_Agg */public RowSet pRowSet;
				/* Used only when flags==MEM_RowSet */public VdbeFrame pFrame;
			/* Used when flags==MEM_Frame */}
			public union_ip u;
			public byte[] zBLOB;
			/* BLOB value */public int n;
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
			/* One of SQLITE_NULL, SQLITE_TEXT, SQLITE_INTEGER, etc */public SqliteEncoding enc;
			/* SqliteEncoding.UTF8, SqliteEncoding.UTF16BE, SqliteEncoding.UTF16LE */
			#if SQLITE_DEBUG
																																																																		      public Mem pScopyFrom;        /* This Mem is a shallow copy of pScopyFrom */
      public object pFiller;        /* So that sizeof(Mem) is a multiple of 8 */
#endif
			public dxDel xDel;
			/* If not null, call this function to delete Mem.z */// Not used under c#
			//public string zMalloc;      /* Dynamic buffer allocated by sqlite3Malloc() */
			public Mem _Mem;
			/* Used when C# overload Z as MEM space */public SumCtx _SumCtx;
			/* Used when C# overload Z as Sum context */public SubProgram[] _SubProgram;
			/* Used when C# overload Z as SubProgram*/public StrAccum _StrAccum;
			/* Used when C# overload Z as STR context */public object _MD5Context;
			///<summary>
			///Used when C# overload Z as MD5 context
			///</summary>
			public Mem() {
			}
			public Mem(sqlite3 db,string z,double r,int i,int n,u16 flags,u8 type,SqliteEncoding enc
			#if SQLITE_DEBUG
																																																																		         , Mem pScopyFrom, object pFiller  /* pScopyFrom, pFiller */
#endif
			) {
				this.db=db;
				this.z=z;
				this.r=r;
				this.u.i=i;
				this.n=n;
				this.flags=flags;
				#if SQLITE_DEBUG
																																																																																								        this.pScopyFrom = pScopyFrom;
        this.pFiller = pFiller;
#endif
				this.type=type;
				this.enc=enc;
			}
			public void CopyTo(ref Mem ct) {
				if(ct==null)
					ct=new Mem();
				ct.u=u;
				ct.r=r;
				ct.db=db;
				ct.z=z;
				if(zBLOB==null)
					ct.zBLOB=null;
				else {
					ct.zBLOB=sqlite3Malloc(zBLOB.Length);
					Buffer.BlockCopy(zBLOB,0,ct.zBLOB,0,zBLOB.Length);
				}
				ct.n=n;
				ct.flags=flags;
				ct.type=type;
				ct.enc=enc;
				ct.xDel=xDel;
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
			bool memIsValid() {
				return true;
			}
			public int sqlite3VdbeMemExpandBlob() {
				return SQLITE_OK;
			}
			public///<summary>
			/// Clear any existing type flags from a Mem and replace them with f
			///</summary>
			//#define MemSetTypeFlag(p, f) \
			//   ((p)->flags = ((p)->flags&~(MEM_TypeMask|MEM_Zero))|f)
			void MemSetTypeFlag(int f) {
				this.flags=(u16)(this.flags&~(MEM_TypeMask|MEM_Zero)|f);
			}
		}
		/* One or more of the following flags are set to indicate the validOK
    ** representations of the value stored in the Mem struct.
    **
    ** If the MEM_Null flag is set, then the value is an SQL NULL value.
    ** No other flags may be set in this case.
    **
    ** If the MEM_Str flag is set then Mem.z points at a string representation.
    ** Usually this is encoded in the same unicode encoding as the main
    ** database (see below for exceptions). If the MEM_Term flag is also
    ** set, then the string is nul terminated. The MEM_Int and MEM_Real
    ** flags may coexist with the MEM_Str flag.
    *///#define MEM_Null      0x0001   /* Value is NULL */
		//#define MEM_Str       0x0002   /* Value is a string */
		//#define MEM_Int       0x0004   /* Value is an integer */
		//#define MEM_Real      0x0008   /* Value is a real number */
		//#define MEM_Blob      0x0010   /* Value is a BLOB */
		//#define MEM_RowSet    0x0020   /* Value is a RowSet object */
		//#define MEM_Frame     0x0040   /* Value is a VdbeFrame object */
		//#define MEM_Invalid   0x0080   /* Value is undefined */
		//#define MEM_TypeMask  0x00ff   /* Mask of type bits */
		const int MEM_Null=0x0001;
		const int MEM_Str=0x0002;
		const int MEM_Int=0x0004;
		const int MEM_Real=0x0008;
		const int MEM_Blob=0x0010;
		const int MEM_RowSet=0x0020;
		const int MEM_Frame=0x0040;
		const int MEM_Invalid=0x0080;
		const int MEM_TypeMask=0x00ff;
		/* Whenever Mem contains a valid string or blob representation, one of
    ** the following flags must be set to determine the memory management
    ** policy for Mem.z.  The MEM_Term flag tells us whether or not the
    ** string is \000 or \u0000 terminated
    //    *///#define MEM_Term      0x0200   /* String rep is nul terminated */
		//#define MEM_Dyn       0x0400   /* Need to call sqliteFree() on Mem.z */
		//#define MEM_Static    0x0800   /* Mem.z points to a static string */
		//#define MEM_Ephem     0x1000   /* Mem.z points to an ephemeral string */
		//#define MEM_Agg       0x2000   /* Mem.z points to an agg function context */
		//#define MEM_Zero      0x4000   /* Mem.i contains count of 0s appended to blob */
		//#if SQLITE_OMIT_INCRBLOB
		//  #undef MEM_Zero
		//  #define MEM_Zero 0x0000
		//#endif
		const int MEM_Term=0x0200;
		const int MEM_Dyn=0x0400;
		const int MEM_Static=0x0800;
		const int MEM_Ephem=0x1000;
		const int MEM_Agg=0x2000;
		#if !SQLITE_OMIT_INCRBLOB
																																												const int MEM_Zero = 0x4000;  
#else
		const int MEM_Zero=0x0000;
		#endif
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
		public class AuxData {
			public object pAux;
		/* Aux data for the i-th argument *///(void );      /* Destructor for the aux data */
		};

		public class VdbeFunc : FuncDef {
			public FuncDef pFunc;
			/* The definition of the function */public int nAux;
			/* Number of entries allocated for apAux[] */public AuxData[] apAux=new AuxData[2];
		/* One slot for each function argument */};

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
		public class sqlite3_context {
			public FuncDef pFunc;
			/* Pointer to function information.  MUST BE FIRST */public VdbeFunc pVdbeFunc;
			/* Auxilary data, if created. */public Mem s=new Mem();
			/* The return value is stored here */public Mem pMem;
			/* Memory cell used to store aggregate context */public int isError;
			/* Error code returned by the function. */public CollSeq pColl;
			/* Collating sequence */public///<summary>
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
			) {
				if(sqlite3VdbeMemSetStr(this.s,z,o,n,enc,xDel)==SQLITE_TOOBIG) {
					this.sqlite3_result_error_toobig();
				}
			}
			public void setResultStrOrError(/* Function context */string z,/* String pointer */int n,/* Bytes in string, or negative */SqliteEncoding enc,/* Encoding of z.  0 for BLOBs */dxDel xDel//void (*xDel)(void)     /* Destructor function */
			) {
				if(sqlite3VdbeMemSetStr(this.s,z,n,enc,xDel)==SQLITE_TOOBIG) {
					this.sqlite3_result_error_toobig();
				}
			}
			public void sqlite3_result_blob(string z,int n,dxDel xDel) {
				Debug.Assert(n>=0);
				Debug.Assert(sqlite3_mutex_held(this.s.db.mutex));
				this.setResultStrOrError(z,n,0,xDel);
			}
			public void sqlite3_result_double(double rVal) {
				Debug.Assert(sqlite3_mutex_held(this.s.db.mutex));
				sqlite3VdbeMemSetDouble(this.s,rVal);
			}
			public void sqlite3_result_error(string z,int n) {
				Debug.Assert(sqlite3_mutex_held(this.s.db.mutex));
				this.setResultStrOrError(z,n,SqliteEncoding.UTF8,SQLITE_TRANSIENT);
				this.isError=SQLITE_ERROR;
			}
			public void sqlite3_result_int(int iVal) {
				Debug.Assert(sqlite3_mutex_held(this.s.db.mutex));
				sqlite3VdbeMemSetInt64(this.s,(i64)iVal);
			}
			public void sqlite3_result_int64(i64 iVal) {
				Debug.Assert(sqlite3_mutex_held(this.s.db.mutex));
				sqlite3VdbeMemSetInt64(this.s,iVal);
			}
			public void sqlite3_result_null() {
				Debug.Assert(sqlite3_mutex_held(this.s.db.mutex));
				sqlite3VdbeMemSetNull(this.s);
			}
			public void sqlite3_result_text(string z,int o,//Offset
			int n,dxDel xDel) {
				Debug.Assert(sqlite3_mutex_held(this.s.db.mutex));
				this.setResultStrOrError(z,o,n,SqliteEncoding.UTF8,xDel);
			}
			public void sqlite3_result_text(StringBuilder z,int n,dxDel xDel) {
				Debug.Assert(sqlite3_mutex_held(this.s.db.mutex));
				this.setResultStrOrError(z.ToString(),n,SqliteEncoding.UTF8,xDel);
			}
			public void sqlite3_result_text(string z,int n,dxDel xDel) {
				Debug.Assert(sqlite3_mutex_held(this.s.db.mutex));
				this.setResultStrOrError(z,n,SqliteEncoding.UTF8,xDel);
			}
			public void sqlite3_result_value(sqlite3_value pValue) {
				Debug.Assert(sqlite3_mutex_held(this.s.db.mutex));
				sqlite3VdbeMemCopy(this.s,pValue);
			}
			public void sqlite3_result_zeroblob(int n) {
				Debug.Assert(sqlite3_mutex_held(this.s.db.mutex));
				sqlite3VdbeMemSetZeroBlob(this.s,n);
			}
			public void sqlite3_result_error_code(int errCode) {
				this.isError=errCode;
				if((this.s.flags&MEM_Null)!=0) {
					this.setResultStrOrError(sqlite3ErrStr(errCode),-1,SqliteEncoding.UTF8,SQLITE_STATIC);
				}
			}
			///<summary>
			///Force an SQLITE_TOOBIG error.
			///</summary>
			public void sqlite3_result_error_toobig() {
				Debug.Assert(sqlite3_mutex_held(this.s.db.mutex));
				this.isError=SQLITE_ERROR;
				this.setResultStrOrError("string or blob too big",-1,SqliteEncoding.UTF8,SQLITE_STATIC);
			}
			///<summary>
			///An SQLITE_NOMEM error.
			///</summary>
			public void sqlite3_result_error_nomem() {
				Debug.Assert(sqlite3_mutex_held(this.s.db.mutex));
				sqlite3VdbeMemSetNull(this.s);
				this.isError=SQLITE_NOMEM;
				//pCtx.s.db.mallocFailed = 1;
			}
		}
		/*
    ** An instance of the virtual machine.  This structure contains the complete
    ** state of the virtual machine.
    **
    ** The "sqlite3_stmt" structure pointer that is returned by sqlite3_prepare()
    ** is really a pointer to an instance of this structure.
    **
    ** The Vdbe.inVtabMethod variable is set to non-zero for the duration of
    ** any virtual table method invocations made by the vdbe program. It is
    ** set to 2 for xDestroy method calls and 1 for all other methods. This
    ** variable is used for two purposes: to allow xDestroy methods to execute
    ** "DROP TABLE" statements and to prevent some nasty side effects of
    ** malloc failure when SQLite is invoked recursively by a virtual table
    ** method function.
    */public class Vdbe {
          public Vdbe()
          {

          }
			public sqlite3 db;
			/* The database connection that owns this statement */
            /** Space to hold the virtual machine's program */
            public Op[] aOp;
            public List<Op> lOp = new List<Op>();
          /* The memory locations */
          public Mem[] aMem;
			public Mem[] apArg;
			/* Arguments to currently executing user function */public Mem[] aColName;
			/* Column names to return */public Mem[] pResultSet;
			/* Pointer to an array of results */public int nMem;
			/* Number of memory locations currently allocated */public int nOp;
			/* Number of instructions in the program */public int nOpAlloc;
			/* Number of slots allocated for aOp[] */public int nLabel;
			/* Number of labels used */public int nLabelAlloc;
			/* Number of slots allocated in aLabel[] */public int[] aLabel;
			/* Space to hold the labels */public u16 nResColumn;
			/* Number of columns in one row of the result set */public u16 nCursor;
			/* Number of slots in apCsr[] */public u32 magic;
			/* Magic number for sanity checking */public string zErrMsg;
			/* Error message written here */public Vdbe pPrev;
			/* Linked list of VDBEs with the same Vdbe.db */public Vdbe pNext;
			/* Linked list of VDBEs with the same Vdbe.db */public VdbeCursor[] apCsr;
			/* One element of this array for each open cursor */public Mem[] aVar;
			/* Values for the OP_Variable opcode. */public string[] azVar;
			/* Name of variables */public ynVar nVar;
			/* Number of entries in aVar[] */public ynVar nzVar;
			/* Number of entries in azVar[] */public u32 cacheCtr;
			/* VdbeCursor row cache generation counter */
          /// <summary>
            /// The program counter 
          /// </summary>
          public int currentOpCodeIndex;
       
          /// <summary>
          /// Value to return
          /// </summary>
			public int rc;
			public u8 errorAction;
			/* Recovery action to do in case of an error */public int explain;
			/* True if EXPLAIN present on SQL command */public bool changeCntOn;
			/* True to update the change-counter */public bool expired;
			/* True if the VM needs to be recompiled */public u8 runOnlyOnce;
			/* Automatically expire on reset */public int minWriteFileFormat;
			/* Minimum file format for writable database files */public int inVtabMethod;
			/* See comments above */public bool usesStmtJournal;
			/* True if uses a statement journal */public bool readOnly;
			/* True for read-only statements */public int nChange;
			/* Number of db changes made since last reset */public bool isPrepareV2;
			/* True if prepared with prepare_v2() */public yDbMask btreeMask;
			/* Bitmask of db.aDb[] entries referenced */public yDbMask lockMask;
			/* Subset of btreeMask that requires a lock */public int iStatement;
			/* Statement number (or 0 if has not opened stmt) */public int[] aCounter=new int[3];
			/* Counters used by sqlite3_stmt_status() */
			#if !SQLITE_OMIT_TRACE
			public i64 startTime;
			/* Time when query started - used for profiling */
			#endif
			public i64 nFkConstraint;
			/* Number of imm. FK constraints this VM */public i64 nStmtDefCons;
			/* Number of def. constraints when stmt started */public string zSql="";
			/* Text of the SQL statement that generated this */public object pFree;
			/* Free this when deleting the vdbe */
			#if SQLITE_DEBUG
																																																																		      public FILE trace;             /* Write an execution trace here, if not NULL */
#endif
			public VdbeFrame pFrame;
			/* Parent frame */public VdbeFrame pDelFrame;
			/* List of frame objects to free on VM reset */public int nFrame;
			/* Number of frames in pFrame list */public u32 expmask;
			///<summary>
			///Binding to these vars invalidates VM
			///</summary>
			public SubProgram pProgram;
			///<summary>
			///Linked list of all sub-programs used by VM
			///</summary>
			public Vdbe Copy() {
				Vdbe cp=(Vdbe)MemberwiseClone();
				return cp;
			}
			public void CopyTo(Vdbe ct) {
				ct.db=db;
				ct.pPrev=pPrev;
				ct.pNext=pNext;
				ct.nOp=nOp;
				ct.nOpAlloc=nOpAlloc;
				ct.lOp=lOp;
				ct.nLabel=nLabel;
				ct.nLabelAlloc=nLabelAlloc;
				ct.aLabel=aLabel;
				ct.apArg=apArg;
				ct.aColName=aColName;
				ct.nCursor=nCursor;
				ct.apCsr=apCsr;
				ct.aVar=aVar;
				ct.azVar=azVar;
				ct.nVar=nVar;
				ct.nzVar=nzVar;
				ct.magic=magic;
				ct.nMem=nMem;
				ct.aMem=aMem;
				ct.cacheCtr=cacheCtr;
				ct.currentOpCodeIndex=currentOpCodeIndex;
				ct.rc=rc;
				ct.errorAction=errorAction;
				ct.nResColumn=nResColumn;
				ct.zErrMsg=zErrMsg;
				ct.pResultSet=pResultSet;
				ct.explain=explain;
				ct.changeCntOn=changeCntOn;
				ct.expired=expired;
				ct.minWriteFileFormat=minWriteFileFormat;
				ct.inVtabMethod=inVtabMethod;
				ct.usesStmtJournal=usesStmtJournal;
				ct.readOnly=readOnly;
				ct.nChange=nChange;
				ct.isPrepareV2=isPrepareV2;
				#if !SQLITE_OMIT_TRACE
				ct.startTime=startTime;
				#endif
				ct.btreeMask=btreeMask;
				ct.lockMask=lockMask;
				aCounter.CopyTo(ct.aCounter,0);
				ct.zSql=zSql;
				ct.pFree=pFree;
				#if SQLITE_DEBUG
																																																																																								        ct.trace = trace;
#endif
				ct.nFkConstraint=nFkConstraint;
				ct.nStmtDefCons=nStmtDefCons;
				ct.iStatement=iStatement;
				ct.pFrame=pFrame;
				ct.nFrame=nFrame;
				ct.expmask=expmask;
				ct.pProgram=pProgram;
				#if SQLITE_SSE
																																																																																								ct.fetchId=fetchId;
ct.lru=lru;
#endif
				#if SQLITE_ENABLE_MEMORY_MANAGEMENT
																																																																																								ct.pLruPrev=pLruPrev;
ct.pLruNext=pLruNext;
#endif
			}
			public///<summary>
			/// Function prototypes
			///
			///</summary>
			//void sqlite3VdbeFreeCursor(Vdbe *, VdbeCursor);
			//void sqliteVdbePopStack(Vdbe*,int);
			//int sqlite3VdbeCursorMoveto(VdbeCursor);
			//#if (SQLITE_DEBUG) || defined(VDBE_PROFILE)
			//void sqlite3VdbePrintOp(FILE*, int, Op);
			//#endif
			//u32 sqlite3VdbeSerialTypeLen(u32);
			//u32 sqlite3VdbeSerialType(Mem*, int);
			//u32sqlite3VdbeSerialPut(unsigned char*, int, Mem*, int);
			//u32 sqlite3VdbeSerialGet(const unsigned char*, u32, Mem);
			//void sqlite3VdbeDeleteAuxData(VdbeFunc*, int);
			//int sqlite2BtreeKeyCompare(BtCursor *, const void *, int, int, int );
			//int sqlite3VdbeIdxKeyCompare(VdbeCursor*,UnpackedRecord*,int);
			//int sqlite3VdbeIdxRowid(sqlite3 *, i64 );
			//int sqlite3MemCompare(const Mem*, const Mem*, const CollSeq);
			//int sqlite3VdbeExec(Vdbe);
			//int sqlite3VdbeList(Vdbe);
			//int sqlite3VdbeHalt(Vdbe);
			//int sqlite3VdbeChangeEncoding(Mem *, int);
			//int sqlite3VdbeMemTooBig(Mem);
			//int sqlite3VdbeMemCopy(Mem*, const Mem);
			//void sqlite3VdbeMemShallowCopy(Mem*, const Mem*, int);
			//void sqlite3VdbeMemMove(Mem*, Mem);
			//int sqlite3VdbeMemNulTerminate(Mem);
			//int sqlite3VdbeMemSetStr(Mem*, const char*, int, u8, void()(void));
			//void sqlite3VdbeMemSetInt64(Mem*, i64);
			#if SQLITE_OMIT_FLOATING_POINT
																																																										// define sqlite3VdbeMemSetDouble sqlite3VdbeMemSetInt64
#else
			//void sqlite3VdbeMemSetDouble(Mem*, double);
			#endif
			//void sqlite3VdbeMemSetNull(Mem);
			//void sqlite3VdbeMemSetZeroBlob(Mem*,int);
			//void sqlite3VdbeMemSetRowSet(Mem);
			//int sqlite3VdbeMemMakeWriteable(Mem);
			//int sqlite3VdbeMemStringify(Mem*, int);
			//i64 sqlite3VdbeIntValue(Mem);
			//int sqlite3VdbeMemIntegerify(Mem);
			//double sqlite3VdbeRealValue(Mem);
			//void sqlite3VdbeIntegerAffinity(Mem);
			//int sqlite3VdbeMemRealify(Mem);
			//int sqlite3VdbeMemNumerify(Mem);
			//int sqlite3VdbeMemFromBtree(BtCursor*,int,int,int,Mem);
			//void sqlite3VdbeMemRelease(Mem p);
			//void sqlite3VdbeMemReleaseExternal(Mem p);
			//int sqlite3VdbeMemFinalize(Mem*, FuncDef);
			//string sqlite3OpcodeName(int);
			//int sqlite3VdbeMemGrow(Mem pMem, int n, int preserve);
			//int sqlite3VdbeCloseStatement(Vdbe *, int);
			//void sqlite3VdbeFrameDelete(VdbeFrame);
			//int sqlite3VdbeFrameRestore(VdbeFrame );
			//void sqlite3VdbeMemStoreType(Mem *pMem);  
			#if !(SQLITE_OMIT_SHARED_CACHE) && SQLITE_THREADSAFE
																																																										  //void sqlite3VdbeEnter(Vdbe);
  //void sqlite3VdbeLeave(Vdbe);
#else
			//# define sqlite3VdbeEnter(X)
			void sqlite3VdbeEnter() {
			}
			public void sqlite3VdbeLeave() {
			}
            public int sqlite3VdbeAddOp3(int op, int p1, int p2, int p3) {
                return sqlite3VdbeAddOp3((OpCode)op,p1,p2,p3);
            }
			public int sqlite3VdbeAddOp3(OpCode op,int p1,int p2,int p3) {
				int i;
				VdbeOp pOp;
				i=this.nOp;
				Debug.Assert(this.magic==VDBE_MAGIC_INIT);
				//Debug.Assert(op>0&&op<0xff);
				if(this.nOpAlloc<=i) {
					if(growOpArray(this)!=0) {
						return 1;
					}
				}
				this.nOp++;
				
                pOp = new VdbeOp();
				pOp.opcode=(u8)op;
				pOp.p5=0;
				pOp.p1=p1;
				pOp.p2=p2;
				pOp.p3=p3;
				pOp.p4.p=null;
				pOp.p4type=P4_NOTUSED;
                lOp.Add(pOp);
				#if SQLITE_DEBUG
																																																																									      pOp.zComment = null;
      if ( sqlite3VdbeAddopTrace )
        sqlite3VdbePrintOp( null, i, p.aOp[i] );
#endif
				#if VDBE_PROFILE
																																																																									pOp.cycles = 0;
pOp.cnt = 0;
#endif
				return i;
			}
			public int sqlite3VdbeAddOp0(int op) {
				return this.sqlite3VdbeAddOp3(op,0,0,0);
			}
			public int sqlite3VdbeAddOp1(int op,int p1) {
				return this.sqlite3VdbeAddOp3(op,p1,0,0);
			}
			public int sqlite3VdbeAddOp2(int op,int p1,bool b2) {
				return this.sqlite3VdbeAddOp2(op,p1,(int)(b2?1:0));
			}
			public int sqlite3VdbeAddOp2(int op,int p1,int p2) {
				return this.sqlite3VdbeAddOp3(op,p1,p2,0);
			}
			public int sqlite3VdbeAddOp4(int op,int p1,int p2,int p3,i32 pP4,int p4type) {
				union_p4 _p4=new union_p4();
				_p4.i=pP4;
				int addr=this.sqlite3VdbeAddOp3(op,p1,p2,p3);
				this.sqlite3VdbeChangeP4(addr,_p4,p4type);
				return addr;
			}
			public int sqlite3VdbeAddOp4(int op,int p1,int p2,int p3,char pP4,int p4type) {
				union_p4 _p4=new union_p4();
				_p4.z=pP4.ToString();
				int addr=this.sqlite3VdbeAddOp3(op,p1,p2,p3);
				this.sqlite3VdbeChangeP4(addr,_p4,p4type);
				return addr;
			}
			public int sqlite3VdbeAddOp4(int op,int p1,int p2,int p3,StringBuilder pP4,int p4type) {
				//      Debug.Assert( pP4 != null );
				union_p4 _p4=new union_p4();
				_p4.z=pP4.ToString();
				int addr=this.sqlite3VdbeAddOp3(op,p1,p2,p3);
				this.sqlite3VdbeChangeP4(addr,_p4,p4type);
				return addr;
			}
			public int sqlite3VdbeAddOp4(int op,int p1,int p2,int p3,string pP4,int p4type) {
				//      Debug.Assert( pP4 != null );
				union_p4 _p4=new union_p4();
				_p4.z=pP4;
				int addr=this.sqlite3VdbeAddOp3(op,p1,p2,p3);
				this.sqlite3VdbeChangeP4(addr,_p4,p4type);
				return addr;
			}
			public int sqlite3VdbeAddOp4(int op,int p1,int p2,int p3,byte[] pP4,int p4type) {
				Debug.Assert(op==OP_Null||pP4!=null);
				union_p4 _p4=new union_p4();
				_p4.z=Encoding.UTF8.GetString(pP4,0,pP4.Length);
				int addr=this.sqlite3VdbeAddOp3(op,p1,p2,p3);
				this.sqlite3VdbeChangeP4(addr,_p4,p4type);
				return addr;
			}
			public int sqlite3VdbeAddOp4(int op,int p1,int p2,int p3,int[] pP4,int p4type) {
				Debug.Assert(pP4!=null);
				union_p4 _p4=new union_p4();
				_p4.ai=pP4;
				int addr=this.sqlite3VdbeAddOp3(op,p1,p2,p3);
				this.sqlite3VdbeChangeP4(addr,_p4,p4type);
				return addr;
			}
			public int sqlite3VdbeAddOp4(int op,int p1,int p2,int p3,i64 pP4,int p4type) {
				union_p4 _p4=new union_p4();
				_p4.pI64=pP4;
				int addr=this.sqlite3VdbeAddOp3(op,p1,p2,p3);
				this.sqlite3VdbeChangeP4(addr,_p4,p4type);
				return addr;
			}
			public int sqlite3VdbeAddOp4(int op,int p1,int p2,int p3,double pP4,int p4type) {
				union_p4 _p4=new union_p4();
				_p4.pReal=pP4;
				int addr=this.sqlite3VdbeAddOp3(op,p1,p2,p3);
				this.sqlite3VdbeChangeP4(addr,_p4,p4type);
				return addr;
			}
			public int sqlite3VdbeAddOp4(int op,int p1,int p2,int p3,FuncDef pP4,int p4type) {
				union_p4 _p4=new union_p4();
				_p4.pFunc=pP4;
				int addr=this.sqlite3VdbeAddOp3(op,p1,p2,p3);
				this.sqlite3VdbeChangeP4(addr,_p4,p4type);
				return addr;
			}
			public int sqlite3VdbeAddOp4(int op,int p1,int p2,int p3,CollSeq pP4,int p4type) {
				union_p4 _p4=new union_p4();
				_p4.pColl=pP4;
				int addr=this.sqlite3VdbeAddOp3(op,p1,p2,p3);
				this.sqlite3VdbeChangeP4(addr,_p4,p4type);
				return addr;
			}
			public int sqlite3VdbeAddOp4(int op,int p1,int p2,int p3,KeyInfo pP4,int p4type) {
				union_p4 _p4=new union_p4();
				_p4.pKeyInfo=pP4;
				int addr=this.sqlite3VdbeAddOp3(op,p1,p2,p3);
				this.sqlite3VdbeChangeP4(addr,_p4,p4type);
				return addr;
			}
			public int sqlite3VdbeAddOp4(int op,int p1,int p2,int p3,VTable pP4,int p4type) {
				Debug.Assert(pP4!=null);
				union_p4 _p4=new union_p4();
				_p4.pVtab=pP4;
				int addr=this.sqlite3VdbeAddOp3(op,p1,p2,p3);
				this.sqlite3VdbeChangeP4(addr,_p4,p4type);
				return addr;
			}
			public int sqlite3VdbeAddOp4Int(/* Add the opcode to this VM */int op,/* The new opcode */int p1,/* The P1 operand */int p2,/* The P2 operand */int p3,/* The P3 operand */int p4/* The P4 operand as an integer */) {
				union_p4 _p4=new union_p4();
				_p4.i=p4;
				int addr=this.sqlite3VdbeAddOp3(op,p1,p2,p3);
				this.sqlite3VdbeChangeP4(addr,_p4,P4_INT32);
				return addr;
			}
			public int sqlite3VdbeMakeLabel() {
				int i;
				i=this.nLabel++;
				Debug.Assert(this.magic==VDBE_MAGIC_INIT);
				if(i>=this.nLabelAlloc) {
					int n=this.nLabelAlloc==0?15:this.nLabelAlloc*2+5;
					if(this.aLabel==null)
						this.aLabel=sqlite3Malloc(this.aLabel,n);
					else
						Array.Resize(ref this.aLabel,n);
					//p.aLabel = sqlite3DbReallocOrFree(p.db, p.aLabel,
					//                                       n*sizeof(p.aLabel[0]));
					this.nLabelAlloc=this.aLabel.Length;
					//sqlite3DbMallocSize(p.db, p.aLabel)/sizeof(p.aLabel[0]);
				}
				if(this.aLabel!=null) {
					this.aLabel[i]=-1;
				}
				return -1-i;
			}
			public void sqlite3VdbeResolveLabel(int x) {
				int j=-1-x;
				Debug.Assert(this.magic==VDBE_MAGIC_INIT);
				Debug.Assert(j>=0&&j<this.nLabel);
				if(this.aLabel!=null) {
					this.aLabel[j]=this.nOp;
				}
			}
			public void sqlite3VdbeRunOnlyOnce() {
				this.runOnlyOnce=1;
			}
			public void resolveP2Values(ref int pMaxFuncArgs) {
				int i;
				int nMaxArgs=pMaxFuncArgs;
				Op pOp;
				int[] aLabel=this.aLabel;
				this.readOnly=true;
				for(i=0;i<this.nOp;i++)//  for(pOp=p->aOp, i=p->nOp-1; i>=0; i--, pOp++)
				 {
					pOp=this.lOp[i];
					u8 opcode=pOp.opcode;
					pOp.opflags=(u8)sqlite3OpcodeProperty[opcode];
					if(opcode==OP_Function||opcode==OP_AggStep) {
						if(pOp.p5>nMaxArgs)
							nMaxArgs=pOp.p5;
					}
					else
						if((opcode==OP_Transaction&&pOp.p2!=0)||opcode==OP_Vacuum) {
							this.readOnly=false;
							#if !SQLITE_OMIT_VIRTUALTABLE
						}
						else
							if(opcode==OP_VUpdate) {
								if(pOp.p2>nMaxArgs)
									nMaxArgs=pOp.p2;
							}
							else
								if(opcode==OP_VFilter) {
									int n;
									Debug.Assert(this.nOp-i>=3);
									Debug.Assert(this.lOp[i-1].opcode==OP_Integer);
									//pOp[-1].opcode==OP_Integer );
									n=this.lOp[i-1].p1;
									//pOp[-1].p1;
									if(n>nMaxArgs)
										nMaxArgs=n;
									#endif
								}
					if((pOp.opflags&OPFLG_JUMP)!=0&&pOp.p2<0) {
						Debug.Assert(-1-pOp.p2<this.nLabel);
						pOp.p2=aLabel[-1-pOp.p2];
					}
				}
				this.db.sqlite3DbFree(ref this.aLabel);
				pMaxFuncArgs=nMaxArgs;
			}
			public int sqlite3VdbeCurrentAddr() {
				Debug.Assert(this.magic==VDBE_MAGIC_INIT);
				return this.nOp;
			}
			public VdbeOp[] sqlite3VdbeTakeOpArray(ref int pnOp,ref int pnMaxArg) {
				List<VdbeOp> lOp=this.lOp;
				Debug.Assert(aOp!=null);
				// && 0==p.db.mallocFailed );
				/* Check that sqlite3VdbeUsesBtree() was not called on this VM */Debug.Assert(this.btreeMask==0);
				this.resolveP2Values(ref pnMaxArg);
				pnOp=this.nOp;
				this.lOp=null;
				return lOp.ToArray();
			}
			public int sqlite3VdbeAddOpList(int nOp,VdbeOpList[] aOp) {
				int addr;
				Debug.Assert(this.magic==VDBE_MAGIC_INIT);
				if(this.nOp+nOp>this.nOpAlloc&&growOpArray(this)!=0) {
					return 0;
				}
				addr=this.nOp;
				if(ALWAYS(nOp>0)) {
					int i;
					VdbeOpList pIn;
					for(i=0;i<nOp;i++) {
						pIn=aOp[i];
						int p2=pIn.p2;
						if(this.lOp[i+addr]==null)
							this.lOp[i+addr]=new VdbeOp();
						VdbeOp pOut=this.lOp[i+addr];
						pOut.opcode=pIn.opcode;
						pOut.p1=pIn.p1;
						if(p2<0&&(sqlite3OpcodeProperty[pOut.opcode]&OPFLG_JUMP)!=0) {
							pOut.p2=addr+(-1-p2);
							// ADDR(p2);
						}
						else {
							pOut.p2=p2;
						}
						pOut.p3=pIn.p3;
						pOut.p4type=P4_NOTUSED;
						pOut.p4.p=null;
						pOut.p5=0;
						#if SQLITE_DEBUG
																																																																																																																							          pOut.zComment = null;
          if ( sqlite3VdbeAddopTrace )
          {
            sqlite3VdbePrintOp( null, i + addr, p.aOp[i + addr] );
          }
#endif
					}
					this.nOp+=nOp;
				}
				return addr;
			}
			public void sqlite3VdbeChangeP1(int addr,int val) {
				Debug.Assert(this!=null);
				Debug.Assert(addr>=0);
				if(this.nOp>addr) {
					this.lOp[addr].p1=val;
				}
			}
			public void sqlite3VdbeChangeP2(int addr,int val) {
				Debug.Assert(this!=null);
				Debug.Assert(addr>=0);
				if(this.nOp>addr) {
					this.lOp[addr].p2=val;
				}
			}
			public void sqlite3VdbeChangeP3(int addr,int val) {
				Debug.Assert(this!=null);
				Debug.Assert(addr>=0);
				if(this.nOp>addr) {
					this.lOp[addr].p3=val;
				}
			}
			public void sqlite3VdbeChangeP5(u8 val) {
				Debug.Assert(this!=null);
				if(this.lOp!=null) {
					Debug.Assert(this.nOp>0);
					this.lOp[this.nOp-1].p5=val;
				}
			}
			public void sqlite3VdbeJumpHere(int addr) {
				Debug.Assert(addr>=0);
				this.sqlite3VdbeChangeP2(addr,this.nOp);
			}
			public void sqlite3VdbeChangeP4(int addr,CollSeq pColl,int n) {
				union_p4 _p4=new union_p4();
				_p4.pColl=pColl;
				this.sqlite3VdbeChangeP4(addr,_p4,n);
			}
			public void sqlite3VdbeChangeP4(int addr,FuncDef pFunc,int n) {
				union_p4 _p4=new union_p4();
				_p4.pFunc=pFunc;
				this.sqlite3VdbeChangeP4(addr,_p4,n);
			}
			public void sqlite3VdbeChangeP4(int addr,KeyInfo pKeyInfo,int n) {
				union_p4 _p4=new union_p4();
				_p4.pKeyInfo=pKeyInfo;
				this.sqlite3VdbeChangeP4(addr,_p4,n);
			}
			public void sqlite3VdbeChangeP4(int addr,int i32n,int n) {
				union_p4 _p4=new union_p4();
				_p4.i=i32n;
				this.sqlite3VdbeChangeP4(addr,_p4,n);
			}
			public void sqlite3VdbeChangeP4(int addr,char c,int n) {
				union_p4 _p4=new union_p4();
				_p4.z=c.ToString();
				this.sqlite3VdbeChangeP4(addr,_p4,n);
			}
			public void sqlite3VdbeChangeP4(int addr,Mem m,int n) {
				union_p4 _p4=new union_p4();
				_p4.pMem=m;
				this.sqlite3VdbeChangeP4(addr,_p4,n);
			}
			public void sqlite3VdbeChangeP4(int addr,string z,dxDel P4_Type) {
				union_p4 _p4=new union_p4();
				_p4.z=z;
				this.sqlite3VdbeChangeP4(addr,_p4,P4_DYNAMIC);
			}
			public void sqlite3VdbeChangeP4(int addr,SubProgram pProgram,int n) {
				union_p4 _p4=new union_p4();
				_p4.pProgram=pProgram;
				this.sqlite3VdbeChangeP4(addr,_p4,n);
			}
			public void sqlite3VdbeChangeP4(int addr,string z,int n) {
				union_p4 _p4=new union_p4();
				if(n>0&&n<=z.Length)
					_p4.z=z.Substring(0,n);
				else
					_p4.z=z;
				this.sqlite3VdbeChangeP4(addr,_p4,n);
			}
			public void sqlite3VdbeChangeP4(int addr,union_p4 _p4,int n) {
				Op pOp;
				sqlite3 db;
				Debug.Assert(this!=null);
				db=this.db;
				Debug.Assert(this.magic==VDBE_MAGIC_INIT);
				if(this.lOp==null/*|| db.mallocFailed != 0 */) {
					if(n!=P4_KEYINFO&&n!=P4_VTAB) {
						freeP4(db,n,_p4);
					}
					return;
				}
				Debug.Assert(this.nOp>0);
				Debug.Assert(addr<this.nOp);
				if(addr<0) {
					addr=this.nOp-1;
				}
				pOp=this.lOp[addr];
				freeP4(db,pOp.p4type,pOp.p4.p);
				pOp.p4.p=null;
				if(n==P4_INT32) {
					/* Note: this cast is safe, because the origin data point was an int
        ** that was cast to a (string ). */pOp.p4.i=_p4.i;
					// SQLITE_PTR_TO_INT(zP4);
					pOp.p4type=P4_INT32;
				}
				else
					if(n==P4_INT64) {
						pOp.p4.pI64=_p4.pI64;
						pOp.p4type=n;
					}
					else
						if(n==P4_REAL) {
							pOp.p4.pReal=_p4.pReal;
							pOp.p4type=n;
						}
						else
							if(_p4==null) {
								pOp.p4.p=null;
								pOp.p4type=P4_NOTUSED;
							}
							else
								if(n==P4_KEYINFO) {
									KeyInfo pKeyInfo;
									int nField,nByte;
									nField=_p4.pKeyInfo.nField;
									//nByte = sizeof(*pKeyInfo) + (nField-1)*sizeof(pKeyInfo.aColl[0]) + nField;
									pKeyInfo=new KeyInfo();
									//sqlite3DbMallocRaw(0, nByte);
									pOp.p4.pKeyInfo=pKeyInfo;
									if(pKeyInfo!=null) {
										//u8 *aSortOrder;
										// memcpy((char)pKeyInfo, zP4, nByte - nField);
										//aSortOrder = pKeyInfo.aSortOrder;
										//if( aSortOrder ){
										//  pKeyInfo.aSortOrder = (unsigned char)&pKeyInfo.aColl[nField];
										//  memcpy(pKeyInfo.aSortOrder, aSortOrder, nField);
										//}
										pKeyInfo=_p4.pKeyInfo.Copy();
										pOp.p4type=P4_KEYINFO;
									}
									else {
										//p.db.mallocFailed = 1;
										pOp.p4type=P4_NOTUSED;
									}
									pOp.p4.pKeyInfo=_p4.pKeyInfo;
									pOp.p4type=P4_KEYINFO;
								}
								else
									if(n==P4_KEYINFO_HANDOFF||n==P4_KEYINFO_STATIC) {
										pOp.p4.pKeyInfo=_p4.pKeyInfo;
										pOp.p4type=P4_KEYINFO;
									}
									else
										if(n==P4_FUNCDEF) {
											pOp.p4.pFunc=_p4.pFunc;
											pOp.p4type=P4_FUNCDEF;
										}
										else
											if(n==P4_COLLSEQ) {
												pOp.p4.pColl=_p4.pColl;
												pOp.p4type=P4_COLLSEQ;
											}
											else
												if(n==P4_DYNAMIC||n==P4_STATIC||n==P4_MPRINTF) {
													pOp.p4.z=_p4.z;
													pOp.p4type=P4_DYNAMIC;
												}
												else
													if(n==P4_MEM) {
														pOp.p4.pMem=_p4.pMem;
														pOp.p4type=P4_MEM;
													}
													else
														if(n==P4_INTARRAY) {
															pOp.p4.ai=_p4.ai;
															pOp.p4type=P4_INTARRAY;
														}
														else
															if(n==P4_SUBPROGRAM) {
																pOp.p4.pProgram=_p4.pProgram;
																pOp.p4type=P4_SUBPROGRAM;
															}
															else
																if(n==P4_VTAB) {
																	pOp.p4.pVtab=_p4.pVtab;
																	pOp.p4type=P4_VTAB;
																	sqlite3VtabLock(_p4.pVtab);
																	Debug.Assert((_p4.pVtab).db==this.db);
																}
																else
																	if(n<0) {
																		pOp.p4.p=_p4.p;
																		pOp.p4type=n;
																	}
																	else {
																		//if (n == 0) n =  n = StringExtensions.sqlite3Strlen30(zP4);
																		pOp.p4.z=_p4.z;
																		// sqlite3DbStrNDup(p.db, zP4, n);
																		pOp.p4type=P4_DYNAMIC;
																	}
			}
			public VdbeOp sqlite3VdbeGetOp(int addr) {
				/* C89 specifies that the constant "dummy" will be initialized to all
      ** zeros, which is correct.  MSVC generates a warning, nevertheless. */Debug.Assert(this.magic==VDBE_MAGIC_INIT);
				if(addr<0) {
					#if SQLITE_OMIT_TRACE
																																																																																																if( p.nOp==0 ) return dummy;
#endif
					addr=this.nOp-1;
				}
				Debug.Assert((addr>=0&&addr<this.nOp)/* || p.db.mallocFailed != 0 */);
				//if ( p.db.mallocFailed != 0 )
				//{
				//  return dummy;
				//}
				//else
				{
					return this.lOp[addr];
				}
			}
			public string sqlite3IndexAffinityStr(Index pIdx) {
				if(pIdx.zColAff==null||pIdx.zColAff[0]=='\0') {
					/* The first time a column affinity string for a particular index is
        ** required, it is allocated and populated here. It is then stored as
        ** a member of the Index structure for subsequent use.
        **
        ** The column affinity string will eventually be deleted by
        ** sqliteDeleteIndex() when the Index structure itself is cleaned
        ** up.
        */int n;
					Table pTab=pIdx.pTable;
					sqlite3 db=sqlite3VdbeDb(this);
					StringBuilder pIdx_zColAff=new StringBuilder(pIdx.nColumn+2);
					// (char )sqlite3DbMallocRaw(0, pIdx->nColumn+2);
					//      if ( pIdx_zColAff == null )
					//      {
					//        db.mallocFailed = 1;
					//        return null;
					//      }
					for(n=0;n<pIdx.nColumn;n++) {
						pIdx_zColAff.Append(pTab.aCol[pIdx.aiColumn[n]].affinity);
					}
					pIdx_zColAff.Append(SQLITE_AFF_NONE);
					pIdx_zColAff.Append('\0');
					pIdx.zColAff=pIdx_zColAff.ToString();
				}
				return pIdx.zColAff;
			}
			public void sqlite3TableAffinityStr(Table pTab) {
				/* The first time a column affinity string for a particular table
      ** is required, it is allocated and populated here. It is then
      ** stored as a member of the Table structure for subsequent use.
      **
      ** The column affinity string will eventually be deleted by
      ** sqlite3DeleteTable() when the Table structure itself is cleaned up.
      */if(pTab.zColAff==null) {
					StringBuilder zColAff;
					int i;
					sqlite3 db=sqlite3VdbeDb(this);
					zColAff=new StringBuilder(pTab.nCol+1);
					// (char)sqlite3DbMallocRaw(0, pTab->nCol+1);
					if(zColAff==null) {
						////        db.mallocFailed = 1;
						return;
					}
					for(i=0;i<pTab.nCol;i++) {
						zColAff.Append(pTab.aCol[i].affinity);
					}
					//zColAff.Append( '\0' );
					pTab.zColAff=zColAff.ToString();
				}
				this.sqlite3VdbeChangeP4(-1,pTab.zColAff,P4_TRANSIENT);
			}
			public void sqlite3ColumnDefault(Table pTab,int i,int iReg) {
				Debug.Assert(pTab!=null);
				if(null==pTab.pSelect) {
					sqlite3_value pValue=new sqlite3_value();
					SqliteEncoding enc=ENC(sqlite3VdbeDb(this));
					Column pCol=pTab.aCol[i];
					#if SQLITE_DEBUG
																																																																																											        VdbeComment( v, "%s.%s", pTab.zName, pCol.zName );
#endif
					Debug.Assert(i<pTab.nCol);
					sqlite3ValueFromExpr(sqlite3VdbeDb(this),pCol.pDflt,enc,pCol.affinity,ref pValue);
					if(pValue!=null) {
						this.sqlite3VdbeChangeP4(-1,pValue,P4_MEM);
					}
					#if !SQLITE_OMIT_FLOATING_POINT
					if(iReg>=0&&pTab.aCol[i].affinity==SQLITE_AFF_REAL) {
						this.sqlite3VdbeAddOp1(OP_RealAffinity,iReg);
					}
					#endif
				}
			}
			public void sqlite3ExprCodeGetColumnOfTable(/* The VDBE under construction */Table pTab,/* The table containing the value */int iTabCur,/* The cursor for this table */int iCol,/* Index of the column to extract */int regOut/* Extract the value into this register */) {
				if(iCol<0||iCol==pTab.iPKey) {
					this.sqlite3VdbeAddOp2(OP_Rowid,iTabCur,regOut);
				}
				else {
					int op=IsVirtual(pTab)?OP_VColumn:OP_Column;
					this.sqlite3VdbeAddOp3(op,iTabCur,iCol,regOut);
				}
				if(iCol>=0) {
					this.sqlite3ColumnDefault(pTab,iCol,regOut);
				}
			}
			public bool vdbeSafety() {
				if(this.db==null) {
					sqlite3_log(SQLITE_MISUSE,"API called with finalized prepared statement");
					return true;
				}
				else {
					return false;
				}
			}
			public bool vdbeSafetyNotNull() {
				if(this==null) {
					sqlite3_log(SQLITE_MISUSE,"API called with NULL prepared statement");
					return true;
				}
				else {
					return this.vdbeSafety();
				}
			}
		}
		/*
    ** The following are allowed values for Vdbe.magic
    *///#define VDBE_MAGIC_INIT     0x26bceaa5    /* Building a VDBE program */
		//#define VDBE_MAGIC_RUN      0xbdf20da3    /* VDBE is ready to execute */
		//#define VDBE_MAGIC_HALT     0x519c2973    /* VDBE has completed execution */
		//#define VDBE_MAGIC_DEAD     0xb606c3c8    /* The VDBE has been deallocated */
		const u32 VDBE_MAGIC_INIT=0x26bceaa5;
		/* Building a VDBE program */const u32 VDBE_MAGIC_RUN=0xbdf20da3;
		/* VDBE is ready to execute */const u32 VDBE_MAGIC_HALT=0x519c2973;
		/* VDBE has completed execution */const u32 VDBE_MAGIC_DEAD=0xb606c3c8;
	/* The VDBE has been deallocated *///# define sqlite3VdbeLeave(X)
	#endif
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
