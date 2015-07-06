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
    using sqlite3_value = Engine.Mem;
	using System.Collections.Generic;
    using Metadata;
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



    namespace Engine
    {
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
                ///Integer value used when MEM.MEM_Int is set in flags 
                ///</summary>

#endif
                public int nZero;

                ///
                ///<summary>
                ///Used when bit MEM.MEM_Zero is set in flags 
                ///</summary>

                public FuncDef pDef;

                ///
                ///<summary>
                ///Used only when flags==MEM.MEM_Agg 
                ///</summary>

                public RowSet pRowSet;

                ///
                ///<summary>
                ///Used only when flags==MEM.MEM_RowSet 
                ///</summary>

                public VdbeFrame pFrame;
                ///
                ///<summary>
                ///Used when flags==MEM.MEM_Frame 
                ///</summary>

            }

            public union_ip u;

            ///<summary>
            ///BLOB value 
            ///</summary>
            public byte[] zBLOB;


            ///<summary>
            ///Number of characters in string value, excluding '\0' 
            ///</summary>
            public int n;



#if DEBUG_CLASS_MEM || DEBUG_CLASS_ALL
																																																																								public u16 _flags;              /* First operand */
public u16 flags
{
get { return _flags; }
set { _flags = value; }
}
#else
            public MemFlags flags;//TODO:u16

            public MemFlags Flags
            {
                get
                {
                    return (MemFlags)flags;
                }
                set
                {
                    flags = (MemFlags)value;
                }
            }

            ///
            ///<summary>
            ///Some combination of MEM.MEM_Null, MEM.MEM_Str, MEM.MEM_Dyn, etc. 
            ///</summary>

#endif
            public FoundationalType type;

            public FoundationalType ValType
            {
                get
                {
                    return (FoundationalType)type;
                }
                set
                {
                    type = (FoundationalType)value;
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
            //public string zMalloc;      /* Dynamic buffer allocated by malloc_cs.sqlite3Malloc() */
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
            public Mem()
            {
            }

            public Mem(sqlite3 db, string z, double r, int i, int n, MemFlags flags, FoundationalType type, SqliteEncoding enc
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
                    ct.zBLOB = malloc_cs.sqlite3Malloc(zBLOB.Length);
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
																																																																    //define memIsValid(M)  ((M)->flags & MEM.MEM_Invalid)==0
    static bool memIsValid( Mem M )
    {
      return ( ( M ).flags & MEM.MEM_Invalid ) == 0;
    }
#else
            bool memIsValid()
            {
                return true;
            }

            public SqlResult sqlite3VdbeMemExpandBlob()
            {
                return SqlResult.SQLITE_OK;
            }

            ///<summary>
            /// Clear any existing type flags from a Mem and replace them with f
            ///</summary>
            //#define MemSetTypeFlag(p, f) \
            //   ((p)->flags = ((p)->flags&~(MEM.MEM_TypeMask|MEM.MEM_Zero))|f)
            public void MemSetTypeFlag(MemFlags f)
            {
                this.flags = (this.flags & ~(MemFlags.MEM_TypeMask | MemFlags.MEM_Zero) | f);
            }


#if !SQLITE_OMIT_FLOATING_POINT
            ///<summary>
            /// Delete any previous value and set the value stored in pMem to NULL.
            ///</summary>
            public void sqlite3VdbeMemSetNull()
            {
                Mem pMem = this;
                if ((pMem.flags & MemFlags.MEM_Frame) != 0)
                {
                    VdbeFrame pFrame = pMem.u.pFrame;
                    pFrame.pParent = pFrame.v.pDelFrame;
                    pFrame.v.pDelFrame = pFrame;
                }
                if ((pMem.flags & MemFlags.MEM_RowSet) != 0)
                {
                    pMem.u.pRowSet.sqlite3RowSetClear();
                }
                pMem.MemSetTypeFlag(MemFlags.MEM_Null);
                malloc_cs.sqlite3_free(ref pMem.zBLOB);
                pMem.z = null;
                pMem.type = FoundationalType.SQLITE_NULL;
            }
#endif



            public SqlResult sqlite3VdbeMemSetStr(
            string z,///
                ///<summary>
                ///String pointer 
                ///</summary>
            int n,///
                ///<summary>
                ///Bytes in string, or negative 
                ///</summary>
            SqliteEncoding enc,///
                ///<summary>
                ///Encoding of z.  0 for BLOBs 
                ///</summary>
            dxDel xDel///
                ///<summary>
                ///Destructor function 
                ///</summary>
            )
            {
                Mem pMem = this;///
                ///<summary>
                ///Memory cell to set to string value 
                ///</summary>
                return pMem.sqlite3VdbeMemSetStr(z, 0, n, enc, xDel);
            }
            // Call w/o offset
            public SqlResult sqlite3VdbeMemSetStr(
            string z,///
                ///<summary>
                ///String pointer 
                ///</summary>
            int offset,///
                ///<summary>
                ///offset into string 
                ///</summary>
            int n,///
                ///<summary>
                ///Bytes in string, or negative 
                ///</summary>
            SqliteEncoding enc,///
                ///<summary>
                ///Encoding of z.  0 for BLOBs 
                ///</summary>
            dxDel xDel//)(void*)/* Destructor function */
            )
            {
                Mem pMem = this;///
                ///<summary>
                ///Memory cell to set to string value 
                ///</summary>
                int nByte = n;
                ///
                ///<summary>
                ///</summary>
                ///<param name="New value for pMem">>n </param>
                int iLimit;
                ///
                ///<summary>
                ///Maximum allowed string or blob size 
                ///</summary>
                MemFlags flags = 0;
                ///
                ///<summary>
                ///</summary>
                ///<param name="New value for pMem">>flags </param>
                Debug.Assert(pMem.db == null || pMem.db.mutex.sqlite3_mutex_held());
                Debug.Assert((pMem.flags & MemFlags.MEM_RowSet) == 0);
                ///
                ///<summary>
                ///If z is a NULL pointer, set pMem to contain an SQL NULL. 
                ///</summary>
                if (z == null || z.Length < offset)
                {
                    pMem.sqlite3VdbeMemSetNull();
                    return SqlResult.SQLITE_OK;
                }
                if (pMem.db != null)
                {
                    iLimit = pMem.db.aLimit[Globals.SQLITE_LIMIT_LENGTH];
                }
                else
                {
                    iLimit = Limits.SQLITE_MAX_LENGTH;
                }
                flags = (enc == 0 ? MemFlags.MEM_Blob : MemFlags.MEM_Str);
                if (nByte < 0)
                {
                    Debug.Assert(enc != 0);
                    if (enc == SqliteEncoding.UTF8)
                    {
                        for (nByte = 0; nByte <= iLimit && nByte < z.Length - offset && z[offset + nByte] != 0; nByte++)
                        {
                        }
                    }
                    else
                    {
                        for (nByte = 0; nByte <= iLimit && z[nByte + offset] != 0 || z[offset + nByte + 1] != 0; nByte += 2)
                        {
                        }
                    }
                    flags |= MemFlags.MEM_Term;
                }
                ///
                ///<summary>
                ///The following block sets the new values of Mem.z and Mem.xDel. It
                ///also sets a flag in local variable "flags" to indicate the memory
                ///management (one of MEM.MEM_Dyn or MEM.MEM_Static).
                ///
                ///</summary>
                if (xDel == Sqlite3.SQLITE_TRANSIENT)
                {
                    u32 nAlloc = (u32)nByte;
                    if ((flags & MemFlags.MEM_Term) != 0)
                    {
                        nAlloc += (u32)(enc == SqliteEncoding.UTF8 ? 1 : 2);
                    }
                    if (nByte > iLimit)
                    {
                        return SqlResult.SQLITE_TOOBIG;
                    }
                    if (vdbemem_cs.sqlite3VdbeMemGrow(pMem, (int)nAlloc, 0) != 0)
                    {
                        return SqlResult.SQLITE_NOMEM;
                    }
                    //if ( nAlloc < z.Length )
                    //pMem.z = new byte[nAlloc]; Buffer.BlockCopy( z, 0, pMem.z, 0, (int)nAlloc ); }
                    //else
                    if (enc == 0)
                    {
                        pMem.z = null;
                        pMem.zBLOB = malloc_cs.sqlite3Malloc(n);
                        for (int i = 0; i < n && i < z.Length - offset; i++)
                            pMem.zBLOB[i] = (byte)z[offset + i];
                    }
                    else
                    {
                        pMem.z = n > 0 && z.Length - offset > n ? z.Substring(offset, n) : z.Substring(offset);
                        //memcpy(pMem.z, z, nAlloc);
                        malloc_cs.sqlite3_free(ref pMem.zBLOB);
                    }
                }
                else
                    if (xDel == sqliteinth.SQLITE_DYNAMIC)
                    {
                        pMem.sqlite3VdbeMemRelease();
                        //pMem.zMalloc = pMem.z = (char*)z;
                        if (enc == 0)
                        {
                            pMem.z = null;
                            if (pMem.zBLOB != null)
                                malloc_cs.sqlite3_free(ref pMem.zBLOB);
                            pMem.zBLOB = Encoding.UTF8.GetBytes(offset == 0 ? z : z.Length + offset < n ? z.Substring(offset, n) : z.Substring(offset));
                        }
                        else
                        {
                            pMem.z = n > 0 && z.Length - offset > n ? z.Substring(offset, n) : z.Substring(offset);
                            //memcpy(pMem.z, z, nAlloc);
                            malloc_cs.sqlite3_free(ref pMem.zBLOB);
                        }
                        pMem.xDel = null;
                    }
                    else
                    {
                        pMem.sqlite3VdbeMemRelease();
                        if (enc == 0)
                        {
                            pMem.z = null;
                            if (pMem.zBLOB != null)
                                malloc_cs.sqlite3_free(ref pMem.zBLOB);
                            pMem.zBLOB = Encoding.UTF8.GetBytes(offset == 0 ? z : z.Length + offset < n ? z.Substring(offset, n) : z.Substring(offset));
                        }
                        else
                        {
                            pMem.z = n > 0 && z.Length - offset > n ? z.Substring(offset, n) : z.Substring(offset);
                            //memcpy(pMem.z, z, nAlloc);
                            malloc_cs.sqlite3_free(ref pMem.zBLOB);
                        }
                        pMem.xDel = xDel;
                        flags |= ((xDel == Sqlite3.SQLITE_STATIC) ? MemFlags.MEM_Static : MemFlags.MEM_Dyn);
                    }
                pMem.n = nByte;
                pMem.flags = flags;
                pMem.enc = ((byte)enc == 0 ? SqliteEncoding.UTF8 : enc);
                pMem.type = (enc == 0 ? FoundationalType.SQLITE_BLOB : FoundationalType.SQLITE_TEXT);
#if !SQLITE_OMIT_UTF16
																																																																		if( pMem.enc!=SqliteEncoding.UTF8 && sqlite3VdbeMemHandleBom(pMem)!=0 ){
return SQLITE_NOMEM;
}
#endif
                if (nByte > iLimit)
                {
                    return SqlResult.SQLITE_TOOBIG;
                }
                return SqlResult.SQLITE_OK;
            }



            ///<summary>
            /// Delete any previous value and set the value to be a BLOB of length
            /// n containing all zeros.
            ///</summary>
            public void sqlite3VdbeMemSetZeroBlob(int n)
            {
                Mem pMem = this;
                pMem.sqlite3VdbeMemRelease();
                pMem.flags = MemFlags.MEM_Blob | MemFlags.MEM_Zero;
                pMem.type = FoundationalType.SQLITE_BLOB;
                pMem.n = 0;
                if (n < 0)
                    n = 0;
                pMem.u.nZero = n;
                pMem.enc = SqliteEncoding.UTF8;
#if SQLITE_OMIT_INCRBLOB
                vdbemem_cs.sqlite3VdbeMemGrow(pMem, n, 0);
                //if( pMem.z!= null ){
                pMem.n = n;
                pMem.z = null;
                //memset(pMem.z, 0, n);
                pMem.zBLOB = malloc_cs.sqlite3Malloc(n);
                //}
#endif
            }
            ///<summary>
            /// Delete any previous value and set the value stored in pMem to val,
            /// manifest type INTEGER.
            ///
            ///</summary>
            public void sqlite3VdbeMemSetInt64(i64 val)
            {
                Mem pMem = this;
                pMem.sqlite3VdbeMemRelease();
                pMem.u.i = val;
                pMem.flags = MemFlags.MEM_Int;
                pMem.type = FoundationalType.SQLITE_INTEGER;
            }
            ///<summary>
            /// Delete any previous value and set the value stored in pMem to val,
            /// manifest type REAL.
            ///
            ///</summary>
            public void sqlite3VdbeMemSetDouble(double val)
            {
                Mem pMem = this;
                if (MathExtensions.sqlite3IsNaN(val))
                {
                    pMem.sqlite3VdbeMemSetNull();
                }
                else
                {
                    pMem.sqlite3VdbeMemRelease();
                    pMem.r = val;
                    pMem.flags = MemFlags.MEM_Real;
                    pMem.type = FoundationalType.SQLITE_FLOAT;
                }
            }



            ///<summary>
            /// Delete any previous value and set the value of pMem to be an
            /// empty boolean index.
            ///
            ///</summary>
            public void sqlite3VdbeMemSetRowSet()
            {
                Mem pMem = this;
                sqlite3 db = pMem.db;
                Debug.Assert(db != null);
                Debug.Assert((pMem.flags & MemFlags.MEM_RowSet) == 0);
                pMem.sqlite3VdbeMemRelease();
                //pMem.zMalloc = sqlite3DbMallocRaw( db, 64 );
                //if ( db.mallocFailed != 0 )
                //{
                //  pMem.flags = MEM.MEM_Null;
                //}
                //else
                {
                    //Debug.Assert( pMem.zMalloc );
                    pMem.u.pRowSet = new RowSet(db, 5);
                    // sqlite3RowSetInit( db, pMem.zMalloc,
                    //     sqlite3DbMallocSize( db, pMem.zMalloc ) );
                    Debug.Assert(pMem.u.pRowSet != null);
                    pMem.flags = MemFlags.MEM_RowSet;
                }
            }


            ///<summary>
            /// If the memory cell contains a string value that must be freed by
            /// invoking an external callback, free it now. Calling this function
            /// does not free any Mem.zMalloc buffer.
            ///
            ///</summary>
            public void sqlite3VdbeMemReleaseExternal()
            {
                var p = this;
                Debug.Assert(p.db == null || p.db.mutex.sqlite3_mutex_held());
                sqliteinth.testcase(p.flags & MemFlags.MEM_Agg);
                sqliteinth.testcase(p.flags & MemFlags.MEM_Dyn);
                sqliteinth.testcase(p.flags & MemFlags.MEM_RowSet);
                sqliteinth.testcase(p.flags & MemFlags.MEM_Frame);
                if ((p.flags & (MemFlags.MEM_Agg | MemFlags.MEM_Dyn | MemFlags.MEM_RowSet | MemFlags.MEM_Frame)) != 0)
                {
                    if ((p.flags & MemFlags.MEM_Agg) != 0)
                    {
                        vdbemem_cs.sqlite3VdbeMemFinalize(p, p.u.pDef);
                        Debug.Assert((p.flags & MemFlags.MEM_Agg) == 0);
                        p.sqlite3VdbeMemRelease();
                    }
                    else
                        if ((p.flags & MemFlags.MEM_Dyn) != 0 && p.xDel != null)
                        {
                            Debug.Assert((p.flags & MemFlags.MEM_RowSet) == 0);
                            p.xDel(ref p.z);
                            p.xDel = null;
                        }
                        else
                            if ((p.flags & MemFlags.MEM_RowSet) != 0)
                            {
                                p.u.pRowSet.sqlite3RowSetClear();
                            }
                            else
                                if ((p.flags & MemFlags.MEM_Frame) != 0)
                                {
                                    p.sqlite3VdbeMemSetNull();
                                }
                }
                p.n = 0;
                p.z = null;
                p.zBLOB = null;
            }


            /*      ///<summary>
                  /// If the memory cell contains a string value that must be freed by
                  /// invoking an external callback, free it now. Calling this function
                  /// does not free any Mem.zMalloc buffer.
                  ///
                  ///</summary>
                  public void sqlite3VdbeMemReleaseExternal()
                  {
                      Mem p = this;
                      Debug.Assert(p.db == null || p.db.mutex.sqlite3_mutex_held());
                      sqliteinth.testcase(p.flags & MEM.MEM_Agg);
                      sqliteinth.testcase(p.flags & MEM.MEM_Dyn);
                      sqliteinth.testcase(p.flags & MEM.MEM_RowSet);
                      sqliteinth.testcase(p.flags & MEM.MEM_Frame);
                      if ((p.flags & (MEM.MEM_Agg | MEM.MEM_Dyn | MEM.MEM_RowSet | MEM.MEM_Frame)) != 0)
                      {
                          if ((p.flags & MEM.MEM_Agg) != 0)
                          {
                              sqlite3VdbeMemFinalize(p, p.u.pDef);
                              Debug.Assert((p.flags & MEM.MEM_Agg) == 0);
                              p.sqlite3VdbeMemRelease();
                          }
                          else
                              if ((p.flags & MEM.MEM_Dyn) != 0 && p.xDel != null)
                              {
                                  Debug.Assert((p.flags & MEM.MEM_RowSet) == 0);
                                  p.xDel(ref p.z);
                                  p.xDel = null;
                              }
                              else
                                  if ((p.flags & MEM.MEM_RowSet) != 0)
                                  {
                                      sqlite3RowSetClear(p.u.pRowSet);
                                  }
                                  else
                                      if ((p.flags & MEM.MEM_Frame) != 0)
                                      {
                                          sqlite3VdbeMemSetNull(p);
                                      }
                      }
                      p.n = 0;
                      p.z = null;
                      p.zBLOB = null;
                  }

                  */


            ///<summary>
            /// Release any memory held by the Mem. This may leave the Mem in an
            /// inconsistent state, for example with (Mem.z==0) and
            /// (Mem.type==SQLITE_TEXT).
            ///
            ///</summary>
            public void sqlite3VdbeMemRelease()
            {
                Mem p = this;
                p.sqlite3VdbeMemReleaseExternal();
                malloc_cs.sqlite3DbFree(p.db, ref p.zBLOB);
                //zMalloc );
                p.z = null;
                //p.zMalloc = 0;
                p.xDel = null;
            }


            ///<summary>
            /// Convert pMem to type integer.  Invalidate any prior representations.
            ///
            ///</summary>
            public SqlResult sqlite3VdbeMemIntegerify()
            {
                Mem pMem = this;
                Debug.Assert(pMem.db == null || pMem.db.mutex.sqlite3_mutex_held());
                Debug.Assert((pMem.flags & MemFlags.MEM_RowSet) == 0);
                //assert( EIGHT_BYTE_ALIGNMENT(pMem) );
                pMem.u.i = pMem.sqlite3VdbeIntValue();
                pMem.MemSetTypeFlag(MemFlags.MEM_Int);
                return SqlResult.SQLITE_OK;
            }
            ///<summary>
            /// Convert pMem so that it is of type MEM.MEM_Real.
            /// Invalidate any prior representations.
            ///
            ///</summary>
            public SqlResult sqlite3VdbeMemRealify()
            {
                Mem pMem = this;
                Debug.Assert(pMem.db == null || pMem.db.mutex.sqlite3_mutex_held());
                //assert( EIGHT_BYTE_ALIGNMENT(pMem) );
                pMem.r = pMem.sqlite3VdbeRealValue();
                pMem.MemSetTypeFlag(MemFlags.MEM_Real);
                return SqlResult.SQLITE_OK;
            }
            ///<summary>
            /// Convert pMem so that it has types MEM.MEM_Real or MEM.MEM_Int or both.
            /// Invalidate any prior representations.
            ///
            /// Every effort is made to force the conversion, even if the input
            /// is a string that does not look completely like a number.  Convert
            /// as much of the string as we can and ignore the rest.
            ///
            ///</summary>
            public SqlResult sqlite3VdbeMemNumerify()
            {
                Mem pMem = this;
                if ((pMem.flags & (MemFlags.MEM_Int | MemFlags.MEM_Real | MemFlags.MEM_Null)) == 0)
                {
                    Debug.Assert((pMem.flags & (MemFlags.MEM_Blob | MemFlags.MEM_Str)) != 0);
                    Debug.Assert(pMem.db == null || pMem.db.mutex.sqlite3_mutex_held());
                    if ((pMem.flags & MemFlags.MEM_Blob) != 0 && pMem.z == null)
                    {
                        if (0 == Converter.sqlite3Atoi64(Encoding.UTF8.GetString(pMem.zBLOB, 0, pMem.zBLOB.Length), ref pMem.u.i, pMem.n, pMem.enc))
                            pMem.MemSetTypeFlag(MemFlags.MEM_Int);
                        else
                        {
                            pMem.r = pMem.sqlite3VdbeRealValue();
                            pMem.MemSetTypeFlag(MemFlags.MEM_Real);
                            pMem.sqlite3VdbeIntegerAffinity();
                        }
                    }
                    else
                        if (0 == Converter.sqlite3Atoi64(pMem.z, ref pMem.u.i, pMem.n, pMem.enc))
                        {
                            pMem.MemSetTypeFlag(MemFlags.MEM_Int);
                        }
                        else
                        {
                            pMem.r = pMem.sqlite3VdbeRealValue();
                            pMem.MemSetTypeFlag(MemFlags.MEM_Real);
                            pMem.sqlite3VdbeIntegerAffinity();
                        }
                }
                Debug.Assert((pMem.flags & (MemFlags.MEM_Int | MemFlags.MEM_Real | MemFlags.MEM_Null)) != 0);
                pMem.flags = (pMem.flags & ~(MemFlags.MEM_Str | MemFlags.MEM_Blob));
                return SqlResult.SQLITE_OK;
            }


            ///<summary>
            /// Return some kind of integer value which is the best we can do
            /// at representing the value that *pMem describes as an integer.
            /// If pMem is an integer, then the value is exact.  If pMem is
            /// a floating-point then the value returned is the integer part.
            /// If pMem is a string or blob, then we make an attempt to convert
            /// it into a integer and return that.  If pMem represents an
            /// an SQL-NULL value, return 0.
            ///
            /// If pMem represents a string value, its encoding might be changed.
            ///
            ///</summary>
            public i64 sqlite3VdbeIntValue()
            {
                Mem pMem = this;
                MemFlags flags;
                Debug.Assert(pMem.db == null || pMem.db.mutex.sqlite3_mutex_held());
                // assert( EIGHT_BYTE_ALIGNMENT(pMem) );
                flags = pMem.flags;
                if ((flags & MemFlags.MEM_Int) != 0)
                {
                    return pMem.u.i;
                }
                else
                    if ((flags & MemFlags.MEM_Real) != 0)
                    {
                        return vdbemem_cs.doubleToInt64(pMem.r);
                    }
                    else
                        if ((flags & (MemFlags.MEM_Str)) != 0)
                        {
                            i64 value = 0;
                            Debug.Assert(pMem.z != null || pMem.n == 0);
                            sqliteinth.testcase(pMem.z == null);
                            Converter.sqlite3Atoi64(pMem.z, ref value, pMem.n, pMem.enc);
                            return value;
                        }
                        else
                            if ((flags & (MemFlags.MEM_Blob)) != 0)
                            {
                                i64 value = 0;
                                Debug.Assert(pMem.zBLOB != null || pMem.n == 0);
                                sqliteinth.testcase(pMem.zBLOB == null);
                                Converter.sqlite3Atoi64(Encoding.UTF8.GetString(pMem.zBLOB, 0, pMem.n), ref value, pMem.n, pMem.enc);
                                return value;
                            }
                            else
                            {
                                return 0;
                            }
            }
            ///<summary>
            /// Return the best representation of pMem that we can get into a
            /// double.  If pMem is already a double or an integer, return its
            /// value.  If it is a string or blob, try to convert it to a double.
            /// If it is a NULL, return 0.0.
            ///
            ///</summary>
            public double sqlite3VdbeRealValue()
            {
                Mem pMem = this;
                Debug.Assert(pMem.db == null || pMem.db.mutex.sqlite3_mutex_held());
                //assert( EIGHT_BYTE_ALIGNMENT(pMem) );
                if ((pMem.flags & MemFlags.MEM_Real) != 0)
                {
                    return pMem.r;
                }
                else
                    if ((pMem.flags & MemFlags.MEM_Int) != 0)
                    {
                        return (double)pMem.u.i;
                    }
                    else
                        if ((pMem.flags & (MemFlags.MEM_Str)) != 0)
                        {
                            ///
                            ///<summary>
                            ///(double)0 In case of SQLITE_OMIT_FLOATING_POINT... 
                            ///</summary>
                            double val = (double)0;
                            Converter.sqlite3AtoF(pMem.z, ref val, pMem.n, pMem.enc);
                            return val;
                        }
                        else
                            if ((pMem.flags & (MemFlags.MEM_Blob)) != 0)
                            {
                                ///
                                ///<summary>
                                ///(double)0 In case of SQLITE_OMIT_FLOATING_POINT... 
                                ///</summary>
                                double val = (double)0;
                                Debug.Assert(pMem.zBLOB != null || pMem.n == 0);
                                Converter.sqlite3AtoF(Encoding.UTF8.GetString(pMem.zBLOB, 0, pMem.n), ref val, pMem.n, pMem.enc);
                                return val;
                            }
                            else
                            {
                                ///
                                ///<summary>
                                ///(double)0 In case of SQLITE_OMIT_FLOATING_POINT... 
                                ///</summary>
                                return (double)0;
                            }
            }


            ///<summary>
            /// The MEM structure is already a MEM.MEM_Real.  Try to also make it a
            /// MEM.MEM_Int if we can.
            ///
            ///</summary>
            public void sqlite3VdbeIntegerAffinity()
            {
                Mem pMem = this;
                Debug.Assert((pMem.flags & MemFlags.MEM_Real) != 0);
                Debug.Assert((pMem.flags & MemFlags.MEM_RowSet) == 0);
                Debug.Assert(pMem.db == null || pMem.db.mutex.sqlite3_mutex_held());
                //assert( EIGHT_BYTE_ALIGNMENT(pMem) );
                pMem.u.i = vdbemem_cs.doubleToInt64(pMem.r);
                ///
                ///<summary>
                ///Only mark the value as an integer if
                ///
                ///</summary>
                ///<param name="(1) the round">op, and</param>
                ///<param name="(2) The integer is neither the largest nor the smallest">(2) The integer is neither the largest nor the smallest</param>
                ///<param name="possible integer (ticket #3922)">possible integer (ticket #3922)</param>
                ///<param name=""></param>
                ///<param name="The second and third terms in the following conditional enforces">The second and third terms in the following conditional enforces</param>
                ///<param name="the second condition under the assumption that addition overflow causes">the second condition under the assumption that addition overflow causes</param>
                ///<param name="values to wrap around.  On x86 hardware, the third term is always">values to wrap around.  On x86 hardware, the third term is always</param>
                ///<param name="true and could be omitted.  But we leave it in because other">true and could be omitted.  But we leave it in because other</param>
                ///<param name="architectures might behave differently.">architectures might behave differently.</param>
                ///<param name=""></param>
                if (pMem.r == (double)pMem.u.i && pMem.u.i > IntegerExtensions.SMALLEST_INT64 && Sqlite3.ALWAYS(pMem.u.i < IntegerExtensions.LARGEST_INT64))
                {
                    pMem.flags |= MemFlags.MEM_Int;
                }
            }


            ///<summary>
            ///Make the given Mem object MEM.MEM_Dyn.  In other words, make it so
            ///that any TEXT or BLOB content is stored in memory obtained from
            ///malloc().  In this way, we know that the memory is safe to be
            ///overwritten or altered.
            ///
            ///Return SqlResult.SQLITE_OK on success or SQLITE_NOMEM if malloc fails.
            ///
            ///</summary>
            public SqlResult sqlite3VdbeMemMakeWriteable()
            {
                Mem pMem = this;
                MemFlags f;
                Debug.Assert(pMem.db == null || pMem.db.mutex.sqlite3_mutex_held());
                Debug.Assert((pMem.flags & MemFlags.MEM_RowSet) == 0);
                pMem.expandBlob();
                f = pMem.flags;
                if ((f & (MemFlags.MEM_Str | MemFlags.MEM_Blob)) != 0)// TODO -- && pMem.z != pMem.zMalloc )
                {
                    if (vdbemem_cs.sqlite3VdbeMemGrow(pMem, pMem.n + 2, 1) != 0)
                        //{
                        //  return SQLITE_NOMEM;
                        //}
                        //pMem.z[pMem->n] = 0;
                        //pMem.z[pMem->n + 1] = 0;
                        pMem.flags |= MemFlags.MEM_Term;
#if SQLITE_DEBUG
																																																																																								        pMem.pScopyFrom = null;
#endif
                }
                return SqlResult.SQLITE_OK;
            }


            ///<summary>
            /// Call sqlite3VdbeMemExpandBlob() on the supplied value (type Mem*)
            /// P if required.
            ///
            ///</summary>
            //#define expandBlob(P) (((P)->flags&MEM.MEM_Zero)?sqlite3VdbeMemExpandBlob(P):0)
            public void expandBlob()
            {
                Mem P = this;
                if ((P.flags & MemFlags.MEM_Zero) != 0)
                    P.sqlite3VdbeMemExpandBlob();
            }

#endif

        }


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
}
