using Community.CsharpSqlite.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using i64 = System.Int64;
using u8 = System.Byte;
using u16 = System.UInt16;
using u32 = System.UInt32;
using u64 = System.UInt64;
using Community.CsharpSqlite.Metadata;

namespace Community.CsharpSqlite.Engine
{
    ///<summary>
    ///Internally, the vdbe manipulates nearly all SQL values as Mem
    ///structures. Each Mem struct may cache multiple representations (string,
    ///integer etc.) of the same value.
    ///
    ///</summary>

    public class Mem
    {
        ///<summary>
        ///The associated database connection 
        ///</summary>
        public Connection db;

        ///<summary>
        ///String value
        ///</summary>
        public string AsString;

        ///<summary>
        ///Real value 
        ///</summary>
        public double AsReal;
        

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
            ///<summary>
            ///Integer value used when MEM.MEM_Int is set in flags 
            ///</summary>
            public i64 AsInteger;



#endif
            ///<summary>
            ///Used when bit MEM.MEM_Zero is set in flags 
            ///</summary>
            public int nZero;



            ///<summary>
            ///Used only when flags==MEM.MEM_Agg 
            ///</summary>
            public FuncDef pDef;
            
            ///<summary>
            ///Used only when flags==MEM.MEM_RowSet 
            ///</summary>
            public RowSet pRowSet;

            

            ///<summary>
            ///Used when flags==MEM.MEM_Frame 
            ///</summary>
            public VdbeFrame pFrame;
            

        }

        public union_ip u;

        ///<summary>
        ///BLOB value 
        ///</summary>
        public byte[] zBLOB;


        ///<summary>
        ///Number of characters in string value, excluding '\0' 
        ///</summary>
        public int CharacterCount;



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
                return flags;
            }
            set
            {
                flags = value;
            }
        }

        ///
        ///<summary>
        ///Some combination of MEM.MEM_Null, MEM.MEM_Str, MEM.MEM_Dyn, etc. 
        ///</summary>

#endif
        public FoundationalType type;
        ///<summary>
        ///One of SQLITE_NULL, SQLITE_TEXT, SQLITE_INTEGER, etc 
        ///</summary>
        public FoundationalType ValType
        {
            get
            {
                return type;
            }
            set
            {
                type = value;
            }
        }

        
        ///<summary>
        ///SqliteEncoding.UTF8, SqliteEncoding.UTF16BE, SqliteEncoding.UTF16LE 
        ///</summary>
        public SqliteEncoding enc;
        

#if SQLITE_DEBUG
																																																																								      public Mem pScopyFrom;        /* This Mem is a shallow copy of pScopyFrom */
      public object pFiller;        /* So that sizeof(Mem) is a multiple of 8 */
#endif
        ///<summary>
        ///If not null, call this function to delete Mem.z 
        ///</summary>
        public dxDel xDel;

        

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

        public Mem(Connection db, string z, double AsReal, int AsInt, int n, MemFlags flags, FoundationalType type, SqliteEncoding enc
#if SQLITE_DEBUG
																																																																								         , Mem pScopyFrom, object pFiller  /* pScopyFrom, pFiller */
#endif
)
        {
            this.db = db;
            this.AsString = z;
            this.AsReal = AsReal;
            this.u.AsInteger = AsInt;
            this.CharacterCount = n;
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
            ct.AsReal = AsReal;
            ct.db = db;
            ct.AsString = AsString;
            if (zBLOB == null)
                ct.zBLOB = null;
            else
            {
                ct.zBLOB = malloc_cs.sqlite3Malloc(zBLOB.Length);
                Buffer.BlockCopy(zBLOB, 0, ct.zBLOB, 0, zBLOB.Length);
            }
            ct.CharacterCount = CharacterCount;
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
            pMem.AsString = null;
            pMem.type = FoundationalType.SQLITE_NULL;
        }
#endif



        public SqlResult sqlite3VdbeMemSetStr(
        string z,
            ///<summary>
            ///Bytes in string, or negative 
            ///</summary>
            int ByteCountOrNegative,
            ///<summary>
            ///Encoding of z.  0 for BLOBs 
            ///</summary>
            SqliteEncoding enc,
            ///<summary>
            ///Destructor function 
            ///</summary>         
            dxDel xDel                      
            )
        {
            return this.sqlite3VdbeMemSetStr(z, 0, ByteCountOrNegative, enc, xDel);
        }
        // Call w/o offset
        public SqlResult sqlite3VdbeMemSetStr(
            string z,
            ///<summary>
            ///offset into string 
            ///</summary>
            int offset,
                       
            int ByteCountOrNegative,
            SqliteEncoding enc,
            dxDel xDel//)(void*)/* Destructor function */
        )
        {
            ///Memory cell to set to string value 
            Mem pMem = this;
                            
            int nByte = ByteCountOrNegative;
            ///New value for pMem->n 
            int iLimit;
            ///Maximum allowed string or blob size 
            MemFlags flags = 0;
            ///New value for pMem->flags 
            Debug.Assert(pMem.db == null || pMem.db.mutex.sqlite3_mutex_held());
            Debug.Assert((pMem.flags & MemFlags.MEM_RowSet) == 0);
            
            ///If z is a NULL pointer, set pMem to contain an SQL NULL. 
            if (z == null || z.Length < offset)
            {
                pMem.sqlite3VdbeMemSetNull();
                return SqlResult.SQLITE_OK;
            }

            iLimit=(pMem.db != null)?pMem.db.aLimit[Globals.SQLITE_LIMIT_LENGTH]:Limits.SQLITE_MAX_LENGTH;

            flags = (enc == 0 ? MemFlags.MEM_Blob : MemFlags.MEM_Str);
            if (nByte < 0)
            {                
                nByte = CountBytes(z, offset, enc, iLimit);
                flags |= MemFlags.MEM_Term;
            }
            ///The following block sets the new values of Mem.z and Mem.xDel. It
            ///also sets a flag in local variable "flags" to indicate the memory
            ///management (one of MEM.MEM_Dyn or MEM.MEM_Static).
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
                if (pMem.Grow((int)nAlloc, 0) != 0)
                {
                    return SqlResult.SQLITE_NOMEM;
                }
                //if ( nAlloc < z.Length )
                //pMem.z = new byte[nAlloc]; Buffer.BlockCopy( z, 0, pMem.z, 0, (int)nAlloc ); }
                //else
                if (enc == 0)
                {
                    pMem.AsString = null;
                    pMem.zBLOB = malloc_cs.sqlite3Malloc(ByteCountOrNegative);
                    for (int i = 0; i < ByteCountOrNegative && i < z.Length - offset; i++)
                        pMem.zBLOB[i] = (byte)z[offset + i];
                }
                else
                {
                    pMem.AsString = ByteCountOrNegative > 0 && z.Length - offset > ByteCountOrNegative ? z.Substring(offset, ByteCountOrNegative) : z.Substring(offset);
                    //memcpy(pMem.z, z, nAlloc);
                    malloc_cs.sqlite3_free(ref pMem.zBLOB);
                }
            }
            else {
                pMem.sqlite3VdbeMemRelease();
                if (enc == 0)
                {
                    pMem.AsString = null;
                    if (pMem.zBLOB != null)
                        malloc_cs.sqlite3_free(ref pMem.zBLOB);
                    pMem.zBLOB = Encoding.UTF8.GetBytes(offset == 0 ? z : z.Length + offset < ByteCountOrNegative ? z.Substring(offset, ByteCountOrNegative) : z.Substring(offset));
                }
                else
                {
                    pMem.AsString = ByteCountOrNegative > 0 && z.Length - offset > ByteCountOrNegative ? z.Substring(offset, ByteCountOrNegative) : z.Substring(offset);
                    //memcpy(pMem.z, z, nAlloc);
                    malloc_cs.sqlite3_free(ref pMem.zBLOB);
                }

                if (xDel == sqliteinth.SQLITE_DYNAMIC)
                {
                    //pMem.zMalloc = pMem.z = (char*)z;                    
                    pMem.xDel = null;
                }
                else
                {                    
                    pMem.xDel = xDel;
                    flags |= ((xDel == Sqlite3.SQLITE_STATIC) ? MemFlags.MEM_Static : MemFlags.MEM_Dyn);
                }
            }
            pMem.CharacterCount = nByte;
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

        private static int CountBytes(string z, int offset, SqliteEncoding enc, int iLimit)
        {
            Debug.Assert(enc != 0);
            int nByte=0;
            if (enc == SqliteEncoding.UTF8)
                for (nByte = 0; nByte <= iLimit && nByte < z.Length - offset 
                    && z[offset + nByte] != 0; nByte++) ;                
            else
                for (nByte = 0; nByte <= iLimit 
                    && z[nByte + offset] != 0 || z[offset + nByte + 1] != 0; nByte += 2) ;

            return nByte;
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
            pMem.CharacterCount = 0;
            if (n < 0)
                n = 0;
            pMem.u.nZero = n;
            pMem.enc = SqliteEncoding.UTF8;
#if SQLITE_OMIT_INCRBLOB
            pMem.Grow( n, 0);
            //if( pMem.z!= null ){
            pMem.CharacterCount = n;
            pMem.AsString = null;
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
        public void Set(i64 val)
        {
            Mem pMem = this;
            pMem.sqlite3VdbeMemRelease();
            pMem.u.AsInteger = val;
            pMem.flags = MemFlags.MEM_Int;
            pMem.type = FoundationalType.SQLITE_INTEGER;
        }
        ///<summary>
        /// Delete any previous value and set the value stored in pMem to val,
        /// manifest type REAL.
        ///
        ///</summary>
        public void Set(double val)
        {
            Mem pMem = this;
            if (MathExtensions.sqlite3IsNaN(val))
            {
                pMem.sqlite3VdbeMemSetNull();
            }
            else
            {
                pMem.sqlite3VdbeMemRelease();
                pMem.AsReal = val;
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
            Connection db = pMem.db;
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
                    p.xDel(ref p.AsString);
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
            p.CharacterCount = 0;
            p.AsString = null;
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
            p.AsString = null;
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
            pMem.u.AsInteger = pMem.ToInt();
            pMem.MemSetTypeFlag(MemFlags.MEM_Int);
            return SqlResult.SQLITE_OK;
        }
        ///<summary>
        /// Convert pMem so that it is of type MEM.MEM_Real.
        /// Invalidate any prior representations.
        ///
        ///</summary>
        public SqlResult Realify()
        {
            Mem pMem = this;
            Debug.Assert(pMem.db == null || pMem.db.mutex.sqlite3_mutex_held());
            //assert( EIGHT_BYTE_ALIGNMENT(pMem) );
            pMem.AsReal = pMem.ToReal();
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
        public SqlResult Numerify()
        {
            Mem pMem = this;
            if ((pMem.flags & (MemFlags.MEM_Int | MemFlags.MEM_Real | MemFlags.MEM_Null)) == 0)
            {
                Debug.Assert((pMem.flags & (MemFlags.MEM_Blob | MemFlags.MEM_Str)) != 0);
                Debug.Assert(pMem.db == null || pMem.db.mutex.sqlite3_mutex_held());
                if ((pMem.flags & MemFlags.MEM_Blob) != 0 && pMem.AsString == null)
                {
                    if (0 == Converter.sqlite3Atoi64(Encoding.UTF8.GetString(pMem.zBLOB, 0, pMem.zBLOB.Length), ref pMem.u.AsInteger, pMem.CharacterCount, pMem.enc))
                        pMem.MemSetTypeFlag(MemFlags.MEM_Int);
                    else
                    {
                        pMem.AsReal = pMem.ToReal();
                        pMem.MemSetTypeFlag(MemFlags.MEM_Real);
                        pMem.sqlite3VdbeIntegerAffinity();
                    }
                }
                else
                    if (0 == Converter.sqlite3Atoi64(pMem.AsString, ref pMem.u.AsInteger, pMem.CharacterCount, pMem.enc))
                {
                    pMem.MemSetTypeFlag(MemFlags.MEM_Int);
                }
                else
                {
                    pMem.AsReal = pMem.ToReal();
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
        public i64 ToInt()
        {
            Mem pMem = this;
            MemFlags flags;
            Debug.Assert(pMem.db == null || pMem.db.mutex.sqlite3_mutex_held());
            // assert( EIGHT_BYTE_ALIGNMENT(pMem) );
            flags = pMem.flags;
            if ((flags & MemFlags.MEM_Int) != 0)
            {
                return pMem.u.AsInteger;
            }
            else
                if ((flags & MemFlags.MEM_Real) != 0)
            {
                return vdbemem_cs.doubleToInt64(pMem.AsReal);
            }
            else
                    if ((flags & (MemFlags.MEM_Str)) != 0)
            {
                i64 value = 0;
                Debug.Assert(pMem.AsString != null || pMem.CharacterCount == 0);
                sqliteinth.testcase(pMem.AsString == null);
                Converter.sqlite3Atoi64(pMem.AsString, ref value, pMem.CharacterCount, pMem.enc);
                return value;
            }
            else
                        if ((flags & (MemFlags.MEM_Blob)) != 0)
            {
                i64 value = 0;
                Debug.Assert(pMem.zBLOB != null || pMem.CharacterCount == 0);
                sqliteinth.testcase(pMem.zBLOB == null);
                Converter.sqlite3Atoi64(Encoding.UTF8.GetString(pMem.zBLOB, 0, pMem.CharacterCount), ref value, pMem.CharacterCount, pMem.enc);
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
        public double ToReal()
        {
            Mem pMem = this;
            Debug.Assert(pMem.db == null || pMem.db.mutex.sqlite3_mutex_held());
            //assert( EIGHT_BYTE_ALIGNMENT(pMem) );
            if ((pMem.flags & MemFlags.MEM_Real) != 0)
            {
                return pMem.AsReal;
            }
            else
                if ((pMem.flags & MemFlags.MEM_Int) != 0)
            {
                return (double)pMem.u.AsInteger;
            }
            else
                    if ((pMem.flags & (MemFlags.MEM_Str)) != 0)
            {
                ///
                ///<summary>
                ///(double)0 In case of SQLITE_OMIT_FLOATING_POINT... 
                ///</summary>
                double val = (double)0;
                Converter.sqlite3AtoF(pMem.AsString, ref val, pMem.CharacterCount, pMem.enc);
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
                Debug.Assert(pMem.zBLOB != null || pMem.CharacterCount == 0);
                Converter.sqlite3AtoF(Encoding.UTF8.GetString(pMem.zBLOB, 0, pMem.CharacterCount), ref val, pMem.CharacterCount, pMem.enc);
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
        ///</summary>
        public void sqlite3VdbeIntegerAffinity()
        {
            Mem pMem = this;
            Debug.Assert((pMem.flags & MemFlags.MEM_Real) != 0);
            Debug.Assert((pMem.flags & MemFlags.MEM_RowSet) == 0);
            Debug.Assert(pMem.db == null || pMem.db.mutex.sqlite3_mutex_held());
            //assert( EIGHT_BYTE_ALIGNMENT(pMem) );
            pMem.u.AsInteger = vdbemem_cs.doubleToInt64(pMem.AsReal);
            ///Only mark the value as an integer if
            ///<param name="(1) the round">op, and</param>
            ///<param name="(2) The integer is neither the largest nor the smallest">(2) The integer is neither the largest nor the smallest</param>
            ///<param name="possible integer (ticket #3922)">possible integer (ticket #3922)</param>
            ///<param name=""></param>
            ///<param name="The second and third terms in the following conditional enforces">The second and third terms in the following conditional enforces</param>
            ///<param name="the second condition under the assumption that addition overflow causes">the second condition under the assumption that addition overflow causes</param>
            ///<param name="values to wrap around.  On x86 hardware, the third term is always">values to wrap around.  On x86 hardware, the third term is always</param>
            ///<param name="true and could be omitted.  But we leave it in because other">true and could be omitted.  But we leave it in because other</param>
            ///<param name="architectures might behave differently.">architectures might behave differently.</param>
            if (pMem.AsReal == (double)pMem.u.AsInteger && pMem.u.AsInteger > IntegerExtensions.SMALLEST_INT64 && Sqlite3.ALWAYS(pMem.u.AsInteger < IntegerExtensions.LARGEST_INT64))
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
        public SqlResult MakeWriteable()
        {
            Mem pMem = this;
            MemFlags f;
            Debug.Assert(pMem.db == null || pMem.db.mutex.sqlite3_mutex_held());
            Debug.Assert((pMem.flags & MemFlags.MEM_RowSet) == 0);
            pMem.expandBlob();
            f = pMem.flags;
            if ((f & (MemFlags.MEM_Str | MemFlags.MEM_Blob)) != 0)// TODO -- && pMem.z != pMem.zMalloc )
            {
                if (pMem.Grow( pMem.CharacterCount + 2, 1) != 0)
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

        public override string ToString()
        {
            return String.Format("{0}\t\t{1}\t\t{2}]\t\t{3} ",this.type,this.u.pRowSet,this.AsString,this.zBLOB);
        }

    }

}
