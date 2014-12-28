using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using FILE = System.IO.TextWriter;
using i64 = System.Int64;
using u8 = System.Byte;
using u16 = System.UInt16;
using u32 = System.UInt32;
using u64 = System.UInt64;
using unsigned = System.UIntPtr;
using Pgno = System.UInt32;
using i32 = System.Int32;

#if !SQLITE_MAX_VARIABLE_NUMBER
using ynVar = System.Int16;

#else
using ynVar = System.Int32; 
#endif
///
///<summary>
///The yDbMask datatype for the bitmask of all attached databases.
///</summary>

#if SQLITE_MAX_ATTACHED
//  typedef sqlite3_uint64 yDbMask;
using yDbMask = System.Int64; 
#else
//  typedef unsigned int yDbMask;
using yDbMask = System.Int32;

#endif
namespace Community.CsharpSqlite
{
	using Op = VdbeOp;
	using System.Text;
	using sqlite3_value = Sqlite3.Mem;
	using System.Collections.Generic;

	public partial class Sqlite3
	{
		///
///<summary>
///2003 September 6
///
///The author disclaims copyright to this source code.  In place of
///a legal notice, here is a blessing:
///
///May you do good and not evil.
///May you find forgiveness for yourself and forgive others.
///May you share freely, never taking more than you give.
///
///
///This is the header file for information that is private to the
///VDBE.  This information used to all be at the top of the single
///source code file "vdbe.c".  When that file became too big (over
///6000 lines long) it was split up into several smaller files and
///this header information was factored out.
///
///</summary>
///<param name="Included in SQLite3 port to C#">SQLite;  2008 Noah B Hart</param>
///<param name="C#">SQLite is an independent reimplementation of the SQLite software library</param>
///<param name=""></param>
///<param name="SQLITE_SOURCE_ID: 2011">23 19:49:22 4374b7e83ea0a3fbc3691f9c0c936272862f32f2</param>
///<param name=""></param>
///<param name=""></param>
///<param name=""></param>

		//#if !_VDBEINT_H_
		//#define _VDBEINT_H_
		///
///<summary>
///SQL is translated into a sequence of instructions to be
///executed by a virtual machine.  Each instruction is an instance
///of the following structure.
///
///</summary>

		//typedef struct VdbeOp Op;
		///<summary>
		/// Boolean values
		///
		///</summary>
		//typedef unsigned char Bool;
		///
///<summary>
///A cursor is a pointer into a single BTree within a database file.
///The cursor can seek to a BTree entry with a particular key, or
///loop over all entries of the Btree.  You can also insert new BTree
///entries or retrieve the key or data from the entry that the cursor
///is currently pointing to.
///
///Every cursor that the virtual machine has open is represented by an
///instance of the following structure.
///
///</summary>

		public class VdbeCursor
		{
			public VdbeCursor ()
			{
			}

			public BtCursor pCursor;

			///
///<summary>
///The cursor structure of the backend 
///</summary>

			public Btree pBt;

			///
///<summary>
///Separate file holding temporary table 
///</summary>

			public KeyInfo pKeyInfo;

			///
///<summary>
///Info about index keys needed by index cursors 
///</summary>

			public int iDb;

			///
///<summary>
///</summary>
///<param name="Index of cursor database in db">1) </param>

			public int pseudoTableReg;

			///
///<summary>
///Register holding pseudotable content. 
///</summary>

			public int nField;

			///
///<summary>
///Number of fields in the header 
///</summary>

			public bool zeroed;

			///
///<summary>
///True if zeroed out and ready for reuse 
///</summary>

			public bool rowidIsValid;

			///
///<summary>
///True if lastRowid is valid 
///</summary>

			public bool atFirst;

			///
///<summary>
///True if pointing to first entry 
///</summary>

			public bool useRandomRowid;

			///
///<summary>
///</summary>
///<param name="Generate new record numbers semi">randomly </param>

			public bool nullRow;

			///
///<summary>
///True if pointing to a row with no data 
///</summary>

			public bool deferredMoveto;

			///
///<summary>
///A call to sqlite3BtreeMoveto() is needed 
///</summary>

			public bool isTable;

			///
///<summary>
///True if a table requiring integer keys 
///</summary>

			public bool isIndex;

			///
///<summary>
///</summary>
///<param name="True if an index containing keys only "> no data </param>

			public bool isOrdered;

			///
///<summary>
///True if the underlying table is BTREE_UNORDERED 
///</summary>

			#if !SQLITE_OMIT_VIRTUALTABLE
			public sqlite3_vtab_cursor pVtabCursor;

			///
///<summary>
///The cursor for a virtual table 
///</summary>

			public sqlite3_module pModule;

			///
///<summary>
///Module for cursor pVtabCursor 
///</summary>

			#endif
			public i64 seqCount;

			///
///<summary>
///Sequence counter 
///</summary>

			public i64 movetoTarget;

			///
///<summary>
///Argument to the deferred sqlite3BtreeMoveto() 
///</summary>

			public i64 lastRowid;

			///
///<summary>
///Last rowid from a Next or NextIdx operation 
///</summary>

			///
///<summary>
///Result of last sqlite3BtreeMoveto() done by an OP_NotExists or
///OP_IsUnique opcode on this cursor. 
///</summary>

			public int seekResult;

			///
///<summary>
///Cached information about the header for the data record that the
///cursor is currently pointing to.  Only valid if cacheStatus matches
///Vdbe.cacheCtr.  Vdbe.cacheCtr will never take on the value of
///CACHE_STALE and so setting cacheStatus=CACHE_STALE guarantees that
///the cache is out of date.
///
///aRow might point to (ephemeral) data for the current row, or it might
///be NULL.
///
///</summary>

			public u32 cacheStatus;

			///
///<summary>
///Cache is valid if this matches Vdbe.cacheCtr 
///</summary>

			public Pgno payloadSize;

			///
///<summary>
///Total number of bytes in the record 
///</summary>

			public u32[] aType;

			///
///<summary>
///Type values for all entries in the record 
///</summary>

			public u32[] aOffset;

			///<summary>
			///Cached offsets to the start of each columns data
			///</summary>
			public int aRow;

			///
///<summary>
///Pointer to Data for the current row, if all on one page 
///</summary>

			public VdbeCursor Copy ()
			{
				return (VdbeCursor)MemberwiseClone ();
			}
		};


		//#define VdbeFrameMem(p) ((Mem )&((u8 )p)[ROUND8(sizeof(VdbeFrame))])
		///<summary>
		/// A value for VdbeCursor.cacheValid that means the cache is always invalid.
		///
		///</summary>
		const int CACHE_STALE = 0;

		///
///<summary>
///Internally, the vdbe manipulates nearly all SQL values as Mem
///structures. Each Mem struct may cache multiple representations (string,
///integer etc.) of the same value.
///
///</summary>

		public class Mem
		{
			public sqlite3 db;

			///
///<summary>
///The associated database connection 
///</summary>

			public string z;

			///<summary>
			///String value
			///</summary>
			public double r;

			///
///<summary>
///Real value 
///</summary>

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

				///
///<summary>
///Integer value used when MEM_Int is set in flags 
///</summary>

				#endif
				public int nZero;

				///
///<summary>
///Used when bit MEM_Zero is set in flags 
///</summary>

				public FuncDef pDef;

				///
///<summary>
///Used only when flags==MEM_Agg 
///</summary>

				public RowSet pRowSet;

				///
///<summary>
///Used only when flags==MEM_RowSet 
///</summary>

				public VdbeFrame pFrame;
			///
///<summary>
///Used when flags==MEM_Frame 
///</summary>

			}

			public union_ip u;

			public byte[] zBLOB;

			///
///<summary>
///BLOB value 
///</summary>

			public int n;

			///
///<summary>
///Number of characters in string value, excluding '\0' 
///</summary>

			#if DEBUG_CLASS_MEM || DEBUG_CLASS_ALL
																																																																								public u16 _flags;              /* First operand */
public u16 flags
{
get { return _flags; }
set { _flags = value; }
}
#else
			public u16 flags;

			public MemFlags Flags {
				get {
					return (MemFlags)flags;
				}
				set {
					flags = (u16)value;
				}
			}

			///
///<summary>
///Some combination of MEM_Null, MEM_Str, MEM_Dyn, etc. 
///</summary>

			#endif
			public u8 type;

			public ValType ValType {
				get {
					return (ValType)type;
				}
				set {
					type = (u8)value;
				}
			}

			///
///<summary>
///One of SQLITE_NULL, SQLITE_TEXT, SQLITE_INTEGER, etc 
///</summary>

			public SqliteEncoding enc;

			///
///<summary>
///SqliteEncoding.UTF8, SqliteEncoding.UTF16BE, SqliteEncoding.UTF16LE 
///</summary>

			#if SQLITE_DEBUG
																																																																								      public Mem pScopyFrom;        /* This Mem is a shallow copy of pScopyFrom */
      public object pFiller;        /* So that sizeof(Mem) is a multiple of 8 */
#endif
			public dxDel xDel;

			///
///<summary>
///If not null, call this function to delete Mem.z 
///</summary>

			// Not used under c#
			//public string zMalloc;      /* Dynamic buffer allocated by sqlite3Malloc() */
			public Mem _Mem;

			///
///<summary>
///Used when C# overload Z as MEM space 
///</summary>

			public SumCtx _SumCtx;

			///
///<summary>
///Used when C# overload Z as Sum context 
///</summary>

			public SubProgram[] _SubProgram;

			///
///<summary>
///Used when C# overload Z as SubProgram
///</summary>

			public StrAccum _StrAccum;

			///
///<summary>
///Used when C# overload Z as STR context 
///</summary>

			public object _MD5Context;

			///<summary>
			///Used when C# overload Z as MD5 context
			///</summary>
			public Mem ()
			{
			}

			public Mem (sqlite3 db, string z, double r, int i, int n, u16 flags, u8 type, SqliteEncoding enc
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

			public void CopyTo (ref Mem ct)
			{
				if (ct == null)
					ct = new Mem ();
				ct.u = u;
				ct.r = r;
				ct.db = db;
				ct.z = z;
				if (zBLOB == null)
					ct.zBLOB = null;
				else {
					ct.zBLOB = sqlite3Malloc (zBLOB.Length);
					Buffer.BlockCopy (zBLOB, 0, ct.zBLOB, 0, zBLOB.Length);
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
			bool memIsValid ()
			{
				return true;
			}

			public int sqlite3VdbeMemExpandBlob ()
			{
				return SQLITE_OK;
			}

			public///<summary>
			/// Clear any existing type flags from a Mem and replace them with f
			///</summary>
			//#define MemSetTypeFlag(p, f) \
			//   ((p)->flags = ((p)->flags&~(MEM_TypeMask|MEM_Zero))|f)
			void MemSetTypeFlag (int f)
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
		///
///<summary>
///</summary>
///<param name="Aux data for the i">th argument </param>

		//(void );      /* Destructor for the aux data */
		};


		public class VdbeFunc : FuncDef
		{
			public FuncDef pFunc;

			///
///<summary>
///The definition of the function 
///</summary>

			public int nAux;

			///
///<summary>
///Number of entries allocated for apAux[] 
///</summary>

			public AuxData[] apAux = new AuxData[2];
		///
///<summary>
///One slot for each function argument 
///</summary>

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

			///
///<summary>
///Pointer to function information.  MUST BE FIRST 
///</summary>

			public VdbeFunc pVdbeFunc;

			///
///<summary>
///Auxilary data, if created. 
///</summary>

			public Mem s = new Mem ();

			///
///<summary>
///The return value is stored here 
///</summary>

			public Mem pMem;

			///
///<summary>
///Memory cell used to store aggregate context 
///</summary>

			public int isError;

			///
///<summary>
///Error code returned by the function. 
///</summary>

			public CollSeq pColl;

			///
///<summary>
///Collating sequence 
///</summary>

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
			void setResultStrOrError (///
///<summary>
///Function context 
///</summary>

			string z, ///
///<summary>
///String pointer 
///</summary>

			int o, ///
///<summary>
///offset into string 
///</summary>

			int n, ///
///<summary>
///Bytes in string, or negative 
///</summary>

			SqliteEncoding enc, ///
///<summary>
///Encoding of z.  0 for BLOBs 
///</summary>

			dxDel xDel//void (*xDel)(void)     /* Destructor function */
			)
			{
				if (sqlite3VdbeMemSetStr (this.s, z, o, n, enc, xDel) == SQLITE_TOOBIG) {
					this.sqlite3_result_error_toobig ();
				}
			}

			public void setResultStrOrError (///
///<summary>
///Function context 
///</summary>

			string z, ///
///<summary>
///String pointer 
///</summary>

			int n, ///
///<summary>
///Bytes in string, or negative 
///</summary>

			SqliteEncoding enc, ///
///<summary>
///Encoding of z.  0 for BLOBs 
///</summary>

			dxDel xDel//void (*xDel)(void)     /* Destructor function */
			)
			{
				if (sqlite3VdbeMemSetStr (this.s, z, n, enc, xDel) == SQLITE_TOOBIG) {
					this.sqlite3_result_error_toobig ();
				}
			}

			public void sqlite3_result_blob (string z, int n, dxDel xDel)
			{
				Debug.Assert (n >= 0);
				Debug.Assert (sqlite3_mutex_held (this.s.db.mutex));
				this.setResultStrOrError (z, n, 0, xDel);
			}

			public void sqlite3_result_double (double rVal)
			{
				Debug.Assert (sqlite3_mutex_held (this.s.db.mutex));
				sqlite3VdbeMemSetDouble (this.s, rVal);
			}

			public void sqlite3_result_error (string z, int n)
			{
				Debug.Assert (sqlite3_mutex_held (this.s.db.mutex));
				this.setResultStrOrError (z, n, SqliteEncoding.UTF8, SQLITE_TRANSIENT);
				this.isError = SQLITE_ERROR;
			}

			public void sqlite3_result_int (int iVal)
			{
				Debug.Assert (sqlite3_mutex_held (this.s.db.mutex));
				sqlite3VdbeMemSetInt64 (this.s, (i64)iVal);
			}

			public void sqlite3_result_int64 (i64 iVal)
			{
				Debug.Assert (sqlite3_mutex_held (this.s.db.mutex));
				sqlite3VdbeMemSetInt64 (this.s, iVal);
			}

			public void sqlite3_result_null ()
			{
				Debug.Assert (sqlite3_mutex_held (this.s.db.mutex));
				sqlite3VdbeMemSetNull (this.s);
			}

			public void sqlite3_result_text (string z, int o, //Offset
			int n, dxDel xDel)
			{
				Debug.Assert (sqlite3_mutex_held (this.s.db.mutex));
				this.setResultStrOrError (z, o, n, SqliteEncoding.UTF8, xDel);
			}

			public void sqlite3_result_text (StringBuilder z, int n, dxDel xDel)
			{
				Debug.Assert (sqlite3_mutex_held (this.s.db.mutex));
				this.setResultStrOrError (z.ToString (), n, SqliteEncoding.UTF8, xDel);
			}

			public void sqlite3_result_text (string z, int n, dxDel xDel)
			{
				Debug.Assert (sqlite3_mutex_held (this.s.db.mutex));
				this.setResultStrOrError (z, n, SqliteEncoding.UTF8, xDel);
			}

			public void sqlite3_result_value (sqlite3_value pValue)
			{
				Debug.Assert (sqlite3_mutex_held (this.s.db.mutex));
				sqlite3VdbeMemCopy (this.s, pValue);
			}

			public void sqlite3_result_zeroblob (int n)
			{
				Debug.Assert (sqlite3_mutex_held (this.s.db.mutex));
				sqlite3VdbeMemSetZeroBlob (this.s, n);
			}

			public void sqlite3_result_error_code (int errCode)
			{
				this.isError = errCode;
				if ((this.s.flags & MEM_Null) != 0) {
					this.setResultStrOrError (sqlite3ErrStr (errCode), -1, SqliteEncoding.UTF8, SQLITE_STATIC);
				}
			}

			///<summary>
			///Force an SQLITE_TOOBIG error.
			///</summary>
			public void sqlite3_result_error_toobig ()
			{
				Debug.Assert (sqlite3_mutex_held (this.s.db.mutex));
				this.isError = SQLITE_ERROR;
				this.setResultStrOrError ("string or blob too big", -1, SqliteEncoding.UTF8, SQLITE_STATIC);
			}

			///<summary>
			///An SQLITE_NOMEM error.
			///</summary>
			public void sqlite3_result_error_nomem ()
			{
				Debug.Assert (sqlite3_mutex_held (this.s.db.mutex));
				sqlite3VdbeMemSetNull (this.s);
				this.isError = SQLITE_NOMEM;
				//pCtx.s.db.mallocFailed = 1;
			}
		}

		///
///<summary>
///The following are allowed values for Vdbe.magic
///
///</summary>

		//#define VDBE_MAGIC_INIT     0x26bceaa5    /* Building a VDBE program */
		//#define VDBE_MAGIC_RUN      0xbdf20da3    /* VDBE is ready to execute */
		//#define VDBE_MAGIC_HALT     0x519c2973    /* VDBE has completed execution */
		//#define VDBE_MAGIC_DEAD     0xb606c3c8    /* The VDBE has been deallocated */
		const u32 VDBE_MAGIC_INIT = 0x26bceaa5;

		///
///<summary>
///Building a VDBE program 
///</summary>

		const u32 VDBE_MAGIC_RUN = 0xbdf20da3;

		///
///<summary>
///VDBE is ready to execute 
///</summary>

		const u32 VDBE_MAGIC_HALT = 0x519c2973;

		///
///<summary>
///VDBE has completed execution 
///</summary>

		const u32 VDBE_MAGIC_DEAD = 0xb606c3c8;
	///
///<summary>
///The VDBE has been deallocated 
///</summary>

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
