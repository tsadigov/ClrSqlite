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
	using Op = Community.CsharpSqlite.Engine.VdbeOp;
	using System.Text;
    using sqlite3_value = Engine.Mem;
	using System.Collections.Generic;
    using Metadata;
    using Community.CsharpSqlite.tree;
    using Community.CsharpSqlite.Utils;
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
    namespace Engine
    {
        public class VdbeCursor
        {
            public VdbeCursor()
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

            public VdbeCursor Copy()
            {
                return (VdbeCursor)MemberwiseClone();
            }
        };
    


        ///
        ///<summary>
        ///The following are allowed values for Vdbe.magic
        ///
        ///</summary>

        //#define VdbeMagic.VDBE_MAGIC_INIT     0x26bceaa5    /* Building a VDBE program */
        //#define VdbeMagic.VDBE_MAGIC_RUN      0xbdf20da3    /* VDBE is ready to execute */
        //#define VdbeMagic.VDBE_MAGIC_HALT     0x519c2973    /* VDBE has completed execution */
        //#define VdbeMagic.VDBE_MAGIC_DEAD     0xb606c3c8    /* The VDBE has been deallocated */
        public enum VdbeMagic : uint
        {
            VDBE_MAGIC_INIT = 0x26bceaa5,

            ///
            ///<summary>
            ///Building a VDBE program 
            ///</summary>

            VDBE_MAGIC_RUN = 0xbdf20da3,

            ///
            ///<summary>
            ///VDBE is ready to execute 
            ///</summary>

            VDBE_MAGIC_HALT = 0x519c2973,

            ///
            ///<summary>
            ///VDBE has completed execution 
            ///</summary>

            VDBE_MAGIC_DEAD = 0xb606c3c8
        }

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

    }
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



        // TODO -- Convert back to inline for speed

#if SQLITE_DEBUG
#else

        		//#define VdbeFrameMem(p) ((Mem )&((u8 )p)[ROUND8(sizeof(VdbeFrame))])
		///<summary>
		/// A value for VdbeCursor.cacheValid that means the cache is always invalid.
		///
		///</summary>
		public const int CACHE_STALE = 0;

#endif




    
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
	//  #define sqlite3VdbeMemExpandBlob(x) SqlResult.SQLITE_OK
	#endif
	//#endif //* !_VDBEINT_H_) */
	}



   


    
}
