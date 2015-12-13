using System;
using System.Diagnostics;
using System.Text;
using i64=System.Int64;
using u8=System.Byte;
using u16=System.UInt16;
using u32=System.UInt32;
using Community.CsharpSqlite;
namespace Community.CsharpSqlite.Engine
{
    using sqlite3_value = Engine.Mem;
    using System.Globalization;
    using Community.CsharpSqlite.Ast;
    using Community.CsharpSqlite.Metadata;
    using Community.CsharpSqlite.Os;
    using Community.CsharpSqlite.Engine;
    using Community.CsharpSqlite.Utils;

   

   
    
        public class vdbemem_cs
        {

            ///
            ///<summary>
            ///2004 May 26
            ///
            ///The author disclaims copyright to this source code.  In place of
            ///a legal notice, here is a blessing:
            ///
            ///May you do good and not evil.
            ///May you find forgiveness for yourself and forgive others.
            ///May you share freely, never taking more than you give.
            ///
            ///
            ///
            ///This file contains code use to manipulate "Mem" structure.  A "Mem"
            ///stores a single value in the VDBE.  Mem is an opaque structure visible
            ///only within the VDBE.  Interface routines refer to a Mem using the
            ///name sqlite_value
            ///
            ///</summary>
            ///<param name="Included in SQLite3 port to C#">SQLite;  2008 Noah B Hart</param>
            ///<param name="C#">SQLite is an independent reimplementation of the SQLite software library</param>
            ///<param name=""></param>
            ///<param name="SQLITE_SOURCE_ID: 2011">19 13:26:54 ed1da510a239ea767a01dc332b667119fa3c908e</param>
            ///<param name=""></param>
            ///<param name=""></param>
            ///<param name=""></param>
            //#include "sqliteInt.h"
            //#include "vdbeInt.h"
            
            // TODO -- Convert to inline for speed
            ///<summary>
            /// If pMem is an object with a valid string representation, this routine
            /// ensures the internal encoding for the string representation is
            /// 'desiredEnc', one of SqliteEncoding.UTF8, SqliteEncoding.UTF16LE or SqliteEncoding.UTF16BE.
            ///
            /// If pMem is not a string object, or the encoding of the string
            /// representation is already stored using the requested encoding, then this
            /// routine is a no-op.
            ///
            /// SqlResult.SQLITE_OK is returned if the conversion is successful (or not required).
            /// SQLITE_NOMEM may be returned if a malloc() fails during conversion
            /// between formats.
            ///
            ///</summary>
            public static SqlResult sqlite3VdbeChangeEncoding(Mem pMem, SqliteEncoding desiredEnc)
            {
                SqlResult rc;
                Debug.Assert((pMem.flags & MemFlags.MEM_RowSet) == 0);
                Debug.Assert(desiredEnc == SqliteEncoding.UTF8 || desiredEnc == SqliteEncoding.UTF16LE || desiredEnc == SqliteEncoding.UTF16BE);
                if ((pMem.flags & MemFlags.MEM_Str) == 0 || pMem.enc == desiredEnc)
                {
                    if (String.IsNullOrEmpty(pMem.z) && pMem.zBLOB != null)
                        pMem.z = Encoding.UTF8.GetString(pMem.zBLOB, 0, pMem.zBLOB.Length);
                    return SqlResult.SQLITE_OK;
                }
                Debug.Assert(pMem.db == null || pMem.db.mutex.sqlite3_mutex_held());
#if SQLITE_OMIT_UTF16
                return SqlResult.SQLITE_ERROR;
#else
																																																																		
/* MemTranslate() may return SqlResult.SQLITE_OK or SQLITE_NOMEM. If NOMEM is returned,
** then the encoding of the value may not have changed.
*/
rc = sqlite3VdbeMemTranslate(pMem, (u8)desiredEnc);
Debug.Assert(rc==SqlResult.SQLITE_OK    || rc==SQLITE_NOMEM);
Debug.Assert(rc==SqlResult.SQLITE_OK    || pMem.enc!=desiredEnc);
Debug.Assert(rc==SQLITE_NOMEM || pMem.enc==desiredEnc);
return rc;
#endif
            }
            
            ///<summary>
            /// If the given Mem* has a zero-filled tail, turn it into an ordinary
            /// blob stored in dynamically allocated space.
            ///
            ///</summary>
#if !SQLITE_OMIT_INCRBLOB
																																												static int sqlite3VdbeMemExpandBlob( Mem pMem )
{
if ( ( pMem.flags & MEM.MEM_Zero ) != 0 )
{
u32 nByte;
Debug.Assert( ( pMem.flags & MEM.MEM_Blob ) != 0 );
Debug.Assert( ( pMem.flags & MEM.MEM_RowSet ) == 0 );
Debug.Assert( pMem.db == null || Sqlite3.sqlite3_mutex_held( pMem.db.mutex ) );
/* Set nByte to the number of bytes required to store the expanded blob. */
nByte = (u32)( pMem.n + pMem.u.nZero );
if ( nByte <= 0 )
{
nByte = 1;
}
if ( sqlite3VdbeMemGrow( pMem, (int)nByte, 1 ) != 0 )
{
return SQLITE_NOMEM;
} /* Set nByte to the number of bytes required to store the expanded blob. */
nByte = (u32)( pMem.n + pMem.u.nZero );
if ( nByte <= 0 )
{
nByte = 1;
}
if ( sqlite3VdbeMemGrow( pMem, (int)nByte, 1 ) != 0 )
{
return SQLITE_NOMEM;
}
//memset(&pMem->z[pMem->n], 0, pMem->u.nZero);
pMem.zBLOB = Encoding.UTF8.GetBytes( pMem.z );
pMem.z = null;
pMem.n += (int)pMem.u.nZero;
pMem.u.i = 0;
pMem.flags = (u16)( pMem.flags & ~( MEM.MEM_Zero | MEM.MEM_Static | MEM.MEM_Ephem | MEM.MEM_Term ) );
pMem.flags |= MEM.MEM_Dyn;
}
return SqlResult.SQLITE_OK;
}
#endif
            ///<summary>
            /// Make sure the given Mem is \u0000 terminated.
            ///</summary>
            public static SqlResult sqlite3VdbeMemNulTerminate(Mem pMem)
            {
                Debug.Assert(pMem.db == null || pMem.db.mutex.sqlite3_mutex_held());
                if ((pMem.flags & MemFlags.MEM_Term) != 0 || (pMem.flags & MemFlags.MEM_Str) == 0)
                {
                    return SqlResult.SQLITE_OK;
                    ///
                    ///<summary>
                    ///Nothing to do 
                    ///</summary>
                }
                //if ( pMem.n != 0 && sqlite3VdbeMemGrow( pMem, pMem.n + 2, 1 ) != 0 )
                //{
                //  return SQLITE_NOMEM;
                //}
                //  pMem.z[pMem->n] = 0;
                //  pMem.z[pMem->n+1] = 0;
                if (pMem.z != null && pMem.n < pMem.z.Length)
                    pMem.z = pMem.z.Substring(0, pMem.n);
                pMem.flags |= MemFlags.MEM_Term;
                return SqlResult.SQLITE_OK;
            }
            ///<summary>
            /// Add MEM.MEM_Str to the set of representations for the given Mem.  Numbers
            /// are converted using io.sqlite3_snprintf().  Converting a BLOB to a string
            /// is a no-op.
            ///
            /// Existing representations MEM.MEM_Int and MEM.MEM_Real are *not* invalidated.
            ///
            /// A MEM.MEM_Null value will never be passed to this function. This function is
            /// used for converting values to text for returning to the user (i.e. via
            /// vdbeapi.sqlite3_value_text()), or for ensuring that values to be used as btree
            /// keys are strings. In the former case a NULL pointer is returned the
            /// user and the later is an internal programming error.
            ///
            ///</summary>
            public static SqlResult sqlite3VdbeMemStringify(Mem pMem, SqliteEncoding enc)
            {
                var rc = SqlResult.SQLITE_OK;
                var fg = pMem.flags;
                const int nByte = 32;
                Debug.Assert(pMem.db == null || pMem.db.mutex.sqlite3_mutex_held());
                Debug.Assert((fg & MemFlags.MEM_Zero) == 0);
                Debug.Assert((fg & (MemFlags.MEM_Str | MemFlags.MEM_Blob)) == 0);
                Debug.Assert((fg & (MemFlags.MEM_Int | MemFlags.MEM_Real)) != 0);
                Debug.Assert((pMem.flags & MemFlags.MEM_RowSet) == 0);
                //assert( EIGHT_BYTE_ALIGNMENT(pMem) );
                if (pMem.Grow(nByte, 0) != 0)
                {
                    return SqlResult.SQLITE_NOMEM;
                }
                ///
                ///<summary>
                ///</summary>
                ///<param name="For a Real or Integer, use io.sqlite3_snprintf() to produce the UTF">8</param>
                ///<param name="string representation of the value. Then, if the required encoding">string representation of the value. Then, if the required encoding</param>
                ///<param name="is UTF">16be do a translation.</param>
                ///<param name=""></param>
                ///<param name="FIX ME: It would be better if io.sqlite3_snprintf() could do UTF">16.</param>
                ///<param name=""></param>
                if ((fg & MemFlags.MEM_Int) != 0)
                {
                    pMem.z = pMem.u.i.ToString();
                    //sqlite3_snprintf(nByte, pMem.z, "%lld", pMem->u.i);
                }
                else
                {
                    Debug.Assert((fg & MemFlags.MEM_Real) != 0);
                    if (Double.IsNegativeInfinity(pMem.r))
                        pMem.z = "-Inf";
                    else
                        if (Double.IsInfinity(pMem.r))
                            pMem.z = "Inf";
                        else
                            if (Double.IsPositiveInfinity(pMem.r))
                                pMem.z = "+Inf";
                            else
                                if (pMem.r.ToString(CultureInfo.InvariantCulture).Contains("."))
                                    pMem.z = pMem.r.ToString(CultureInfo.InvariantCulture).ToLower();
                                //sqlite3_snprintf(nByte, pMem.z, "%!.15g", pMem->r);
                                else
                                    pMem.z = pMem.r.ToString(CultureInfo.InvariantCulture) + ".0";
                }
                pMem.n = StringExtensions.sqlite3Strlen30(pMem.z);
                pMem.enc = SqliteEncoding.UTF8;
                pMem.flags |= MemFlags.MEM_Str | MemFlags.MEM_Term;
                sqlite3VdbeChangeEncoding(pMem, enc);
                return rc;
            }
            ///<summary>
            /// Memory cell pMem contains the context of an aggregate function.
            /// This routine calls the finalize method for that function.  The
            /// result of the aggregate is stored back into pMem.
            ///
            /// Return SqlResult.SQLITE_ERROR if the finalizer reports an error.  SqlResult.SQLITE_OK
            /// otherwise.
            ///
            ///</summary>
            public static SqlResult sqlite3VdbeMemFinalize(Mem pMem, FuncDef pFunc)
            {
                var rc = SqlResult.SQLITE_OK;
                if (Sqlite3.ALWAYS(pFunc != null && pFunc.xFinalize != null))
                {
                    sqlite3_context ctx = new sqlite3_context();
                    Debug.Assert((pMem.flags & MemFlags.MEM_Null) != 0 || pFunc == pMem.u.pDef);
                    Debug.Assert(pMem.db == null || pMem.db.mutex.sqlite3_mutex_held());
                    //memset(&ctx, 0, sizeof(ctx));
                    ctx.s.flags = MemFlags.MEM_Null;
                    ctx.s.db = pMem.db;
                    ctx.pMem = pMem;
                    ctx.pFunc = pFunc;
                    pFunc.xFinalize(ctx);
                    ///
                    ///<summary>
                    ///</summary>
                    ///<param name="IMP: R">23230 </param>
                    Debug.Assert(0 == (pMem.flags & MemFlags.MEM_Dyn) && pMem.xDel == null);
                    malloc_cs.sqlite3DbFree(pMem.db, ref pMem.zBLOB);
                    //zMalloc );
                    ctx.s.CopyTo(ref pMem);
                    //memcpy(pMem, &ctx.s, sizeof(ctx.s));
                    rc = ctx.isError;
                }
                return rc;
            }


            ///<summary>
            /// Convert a 64-bit IEEE double into a 64-bit signed integer.
            /// If the double is too large, return 0x8000000000000000.
            ///
            /// Most systems appear to do this simply by assigning
            /// variables and without the extra range tests.  But
            /// there are reports that windows throws an expection
            /// if the floating point value is out of range. (See ticket #2880.)
            /// Because we do not completely understand the problem, we will
            /// take the conservative approach and always do range tests
            /// before attempting the conversion.
            ///
            ///</summary>
            public static i64 doubleToInt64(double r)
            {
#if SQLITE_OMIT_FLOATING_POINT
																																																																		/* When floating-point is omitted, double and int64 are the same thing */
return r;
#else
                ///
                ///<summary>
                ///Many compilers we encounter do not define constants for the
                ///</summary>
                ///<param name="minimum and maximum 64">bit integers, or they define them</param>
                ///<param name="inconsistently.  And many do not understand the "LL" notation.">inconsistently.  And many do not understand the "LL" notation.</param>
                ///<param name="So we define our own static constants here using nothing">So we define our own static constants here using nothing</param>
                ///<param name="larger than a 32">bit integer constant.</param>
                const i64 maxInt = IntegerExtensions.LARGEST_INT64;
                const i64 minInt = IntegerExtensions.SMALLEST_INT64;
                if (r < (double)minInt)
                {
                    return minInt;
                }
                else
                    if (r > (double)maxInt)
                    {
                        ///
                        ///<summary>
                        ///</summary>
                        ///<param name="minInt is correct here "> not maxInt.  It turns out that assigning</param>
                        ///<param name="a very large positive number to an integer results in a very large">a very large positive number to an integer results in a very large</param>
                        ///<param name="negative integer.  This makes no sense, but it is what x86 hardware">negative integer.  This makes no sense, but it is what x86 hardware</param>
                        ///<param name="does so for compatibility we will do the same in software. ">does so for compatibility we will do the same in software. </param>
                        return minInt;
                    }
                    else
                    {
                        return (i64)r;
                    }
#endif
            }
        
            
#if SQLITE_DEBUG
																																												    /*
** This routine prepares a memory cell for modication by breaking
** its link to a shallow copy and by marking any current shallow
** copies of this cell as invalid.
**
** This is used for testing and debugging only - to make sure shallow
** copies are not misused.
*/
    static void sqlite3VdbeMemPrepareToChange( Vdbe pVdbe, Mem pMem )
    {
      int i;
      Mem pX;
      for ( i = 1; i <= pVdbe.nMem; i++ )
      {
        pX = pVdbe.aMem[i];
        if ( pX.pScopyFrom == pMem )
        {
          pX.flags |= MEM.MEM_Invalid;
          pX.pScopyFrom = null;
        }
      }
      pMem.pScopyFrom = null;
    }
#endif
            ///<summary>
            /// Size of struct Mem not including the Mem.zMalloc member.
            ///</summary>
            //#define MEMCELLSIZE (size_t)(&(((Mem *)0).zMalloc))
            ///<summary>
            /// Make an shallow copy of pFrom into pTo.  Prior contents of
            /// pTo are freed.  The pFrom.z field is not duplicated.  If
            /// pFrom.z is used, then pTo.z points to the same thing as pFrom.z
            /// and flags gets srcType (either MEM.MEM_Ephem or MEM.MEM_Static).
            ///
            ///</summary>
            public static void sqlite3VdbeMemShallowCopy(Mem pTo, Mem pFrom, MemFlags srcType)
            {
                Debug.Assert((pFrom.flags & MemFlags.MEM_RowSet) == 0);
                pTo.sqlite3VdbeMemReleaseExternal();
                pFrom.CopyTo(ref pTo);
                //  memcpy(pTo, pFrom, MEMCELLSIZE);
                pTo.xDel = null;
                if ((pFrom.flags & MemFlags.MEM_Static) != 0)
                {
                    pTo.flags = (pFrom.flags & ~(MemFlags.MEM_Dyn | MemFlags.MEM_Static | MemFlags.MEM_Ephem));
                    Debug.Assert(srcType == MemFlags.MEM_Ephem || srcType == MemFlags.MEM_Static);
                    pTo.flags |= srcType;
                }
            }
            ///<summary>
            /// Make a full copy of pFrom into pTo.  Prior contents of pTo are
            /// freed before the copy is made.
            ///
            ///</summary>
            public static SqlResult sqlite3VdbeMemCopy(Mem pTo, Mem pFrom)
            {
                var rc = SqlResult.SQLITE_OK;
                Debug.Assert((pFrom.flags & MemFlags.MEM_RowSet) == 0);
                pTo.sqlite3VdbeMemReleaseExternal();
                pFrom.CopyTo(ref pTo);
                // memcpy(pTo, pFrom, MEMCELLSIZE);
                pTo.flags = (pTo.flags & ~MemFlags.MEM_Dyn);
                if ((pTo.flags & (MemFlags.MEM_Str | MemFlags.MEM_Blob)) != 0)
                {
                    if (0 == (pFrom.flags & MemFlags.MEM_Static))
                    {
                        pTo.flags |= MemFlags.MEM_Ephem;
                        rc = pTo.sqlite3VdbeMemMakeWriteable();
                    }
                }
                return rc;
            }
            ///<summary>
            /// Transfer the contents of pFrom to pTo. Any existing value in pTo is
            /// freed. If pFrom contains ephemeral data, a copy is made.
            ///
            /// pFrom contains an SQL NULL when this routine returns.
            ///
            ///</summary>
            public static void sqlite3VdbeMemMove(Mem pTo, Mem pFrom)
            {
                Debug.Assert(pFrom.db == null || pFrom.db.mutex.sqlite3_mutex_held());
                Debug.Assert(pTo.db == null || pTo.db.mutex.sqlite3_mutex_held());
                Debug.Assert(pFrom.db == null || pTo.db == null || pFrom.db == pTo.db);
                pTo.sqlite3VdbeMemRelease();
                pFrom.CopyTo(ref pTo);
                // memcpy(pTo, pFrom, Mem).Length;
                pFrom.flags = MemFlags.MEM_Null;
                pFrom.xDel = null;
                pFrom.z = null;
                malloc_cs.sqlite3_free(ref pFrom.zBLOB);
                //pFrom.zMalloc=0;
            }

            



            ///<summary>
            /// Compare the values contained by the two memory cells, returning
            /// negative, zero or positive if pMem1 is less than, equal to, or greater
            /// than pMem2. Sorting order is NULL's first, followed by numbers (integers
            /// and reals) sorted numerically, followed by text ordered by the collating
            /// sequence pColl and finally blob's ordered by _Custom.memcmp().
            ///
            /// Two NULL values are considered equal by this function.
            ///
            ///</summary>
            public static int sqlite3MemCompare(Mem pMem1, Mem pMem2, CollSeq pColl)
            {
                int rc;
                MemFlags f1, f2;
                MemFlags combined_flags;
                f1 = pMem1.flags;
                f2 = pMem2.flags;
                combined_flags = f1 | f2;
                Debug.Assert((combined_flags & MemFlags.MEM_RowSet) == 0);
                ///
                ///<summary>
                ///If one value is NULL, it is less than the other. If both values
                ///are NULL, return 0.
                ///
                ///</summary>
                if ((combined_flags & MemFlags.MEM_Null) != 0)
                {
                    return (f2 & MemFlags.MEM_Null) - (f1 & MemFlags.MEM_Null);
                }
                ///
                ///<summary>
                ///If one value is a number and the other is not, the number is less.
                ///If both are numbers, compare as reals if one is a real, or as integers
                ///if both values are integers.
                ///
                ///</summary>
                if ((combined_flags & (MemFlags.MEM_Int | MemFlags.MEM_Real)) != 0)
                {
                    if ((f1 & (MemFlags.MEM_Int | MemFlags.MEM_Real)) == 0)
                    {
                        return 1;
                    }
                    if ((f2 & (MemFlags.MEM_Int | MemFlags.MEM_Real)) == 0)
                    {
                        return -1;
                    }
                    if ((f1 & f2 & MemFlags.MEM_Int) == 0)
                    {
                        double r1, r2;
                        if ((f1 & MemFlags.MEM_Real) == 0)
                        {
                            r1 = (double)pMem1.u.i;
                        }
                        else
                        {
                            r1 = pMem1.r;
                        }
                        if ((f2 & MemFlags.MEM_Real) == 0)
                        {
                            r2 = (double)pMem2.u.i;
                        }
                        else
                        {
                            r2 = pMem2.r;
                        }
                        if (r1 < r2)
                            return -1;
                        if (r1 > r2)
                            return 1;
                        return 0;
                    }
                    else
                    {
                        Debug.Assert((f1 & MemFlags.MEM_Int) != 0);
                        Debug.Assert((f2 & MemFlags.MEM_Int) != 0);
                        if (pMem1.u.i < pMem2.u.i)
                            return -1;
                        if (pMem1.u.i > pMem2.u.i)
                            return 1;
                        return 0;
                    }
                }
                ///
                ///<summary>
                ///If one value is a string and the other is a blob, the string is less.
                ///If both are strings, compare using the collating functions.
                ///
                ///</summary>
                if ((combined_flags & MemFlags.MEM_Str) != 0)
                {
                    if ((f1 & MemFlags.MEM_Str) == 0)
                    {
                        return 1;
                    }
                    if ((f2 & MemFlags.MEM_Str) == 0)
                    {
                        return -1;
                    }
                    Debug.Assert(pMem1.enc == pMem2.enc);
                    Debug.Assert(pMem1.enc == SqliteEncoding.UTF8 || pMem1.enc == SqliteEncoding.UTF16LE || pMem1.enc == SqliteEncoding.UTF16BE);
                    ///
                    ///<summary>
                    ///The collation sequence must be defined at this point, even if
                    ///the user deletes the collation sequence after the vdbe program is
                    ///compiled (this was not always the case).
                    ///
                    ///</summary>
                    Debug.Assert(pColl == null || pColl.xCmp != null);
                    if (pColl != null)
                    {
                        if (pMem1.enc == pColl.enc)
                        {
                            ///
                            ///<summary>
                            ///The strings are already in the correct encoding.  Call the
                            ///comparison function directly 
                            ///</summary>
                            return pColl.xCmp(pColl.pUser, pMem1.n, pMem1.z, pMem2.n, pMem2.z);
                        }
                        else
                        {
                            string v1, v2;
                            int n1, n2;
                            Mem c1 = null;
                            Mem c2 = null;
                            c1 = malloc_cs.sqlite3Malloc(c1);
                            // memset( &c1, 0, sizeof( c1 ) );
                            c2 = malloc_cs.sqlite3Malloc(c2);
                            // memset( &c2, 0, sizeof( c2 ) );
                            sqlite3VdbeMemShallowCopy(c1, pMem1, MemFlags.MEM_Ephem);
                            sqlite3VdbeMemShallowCopy(c2, pMem2, MemFlags.MEM_Ephem);
                            v1 = sqlite3ValueText((sqlite3_value)c1, pColl.enc);
                            n1 = v1 == null ? 0 : c1.n;
                            v2 = sqlite3ValueText((sqlite3_value)c2, pColl.enc);
                            n2 = v2 == null ? 0 : c2.n;
                            rc = pColl.xCmp(pColl.pUser, n1, v1, n2, v2);
                            c1.sqlite3VdbeMemRelease();
                            c2.sqlite3VdbeMemRelease();
                            return rc;
                        }
                    }
                    ///
                    ///<summary>
                    ///If a NULL pointer was passed as the collate function, fall through
                    ///to the blob case and use memcmp().  
                    ///</summary>
                }
                ///
                ///<summary>
                ///Both values must be blobs.  Compare using memcmp().  
                ///</summary>
                if ((pMem1.flags & MemFlags.MEM_Blob) != 0)
                    if (pMem1.zBLOB != null)
                        rc = _Custom.memcmp(pMem1.zBLOB, pMem2.zBLOB, (pMem1.n > pMem2.n) ? pMem2.n : pMem1.n);
                    else
                        rc = _Custom.memcmp(pMem1.z, pMem2.zBLOB, (pMem1.n > pMem2.n) ? pMem2.n : pMem1.n);
                else
                    rc = _Custom.memcmp(pMem1.z, pMem2.z, (pMem1.n > pMem2.n) ? pMem2.n : pMem1.n);
                if (rc == 0)
                {
                    rc = pMem1.n - pMem2.n;
                }
                return rc;
            }
            ///<summary>
            /// Move data out of a btree key or data field and into a Mem structure.
            /// The data or key is taken from the entry that pCur is currently pointing
            /// to.  offset and amt determine what portion of the data or key to retrieve.
            /// key is true to get the key or false to get data.  The result is written
            /// into the pMem element.
            ///
            /// The pMem structure is assumed to be uninitialized.  Any prior content
            /// is overwritten without being freed.
            ///
            /// If this routine fails for any reason (malloc returns NULL or unable
            /// to read from the disk) then the pMem is left in an inconsistent state.
            ///
            ///</summary>
            public static SqlResult sqlite3VdbeMemFromBtree(tree.BtCursor pCur,///
                ///<summary>
                ///Cursor pointing at record to retrieve. 
                ///</summary>
            int offset,///
                ///<summary>
                ///Offset from the start of data to return bytes from. 
                ///</summary>
            int amt,///
                ///<summary>
                ///Number of bytes to return. 
                ///</summary>
            bool key,///
                ///<summary>
                ///If true, retrieve from the btree key, not data. 
                ///</summary>
            Mem pMem///
                ///<summary>
                ///OUT: Return data in this Mem structure. 
                ///</summary>
            )
            {
                byte[] zData;
                ///
                ///<summary>
                ///Data from the btree layer 
                ///</summary>
                int available = 0;
                ///
                ///<summary>
                ///Number of bytes available on the local btree page 
                ///</summary>
                var rc = SqlResult.SQLITE_OK;
                ///
                ///<summary>
                ///Return code 
                ///</summary>
                Debug.Assert(pCur.sqlite3BtreeCursorIsValid());
                ///
                ///<summary>
                ///Note: the calls to BtreeKeyFetch() and DataFetch() below assert()
                ///that both the BtShared and database handle mutexes are held. 
                ///</summary>
                Debug.Assert((pMem.flags & MemFlags.MEM_RowSet) == 0);
                int outOffset = -1;
                if (key)
                {
                    zData = pCur.KeyFetch(ref available, ref outOffset);
                }
                else
                {
                    zData = pCur.DataFetch(ref available, ref outOffset);
                }
                Debug.Assert(zData != null);
                if (offset + amt <= available && (pMem.flags & MemFlags.MEM_Dyn) == 0)
                {
                    pMem.sqlite3VdbeMemRelease();
                    pMem.zBLOB = malloc_cs.sqlite3Malloc(amt);
                    Buffer.BlockCopy(zData, offset, pMem.zBLOB, 0, amt);
                    //pMem.z = &zData[offset];
                    pMem.flags = MemFlags.MEM_Blob | MemFlags.MEM_Ephem;
                }
                else
                    if (SqlResult.SQLITE_OK == (rc = pMem.Grow( amt + 2, 0)))
                    {
                        pMem.enc = 0;
                        pMem.type = FoundationalType.SQLITE_BLOB;
                        pMem.z = null;
                        pMem.zBLOB = malloc_cs.sqlite3Malloc(amt);
                        pMem.flags = MemFlags.MEM_Blob | MemFlags.MEM_Dyn | MemFlags.MEM_Term;
                        if (key)
                        {
                            rc = pCur.sqlite3BtreeKey((u32)offset, (u32)amt, pMem.zBLOB);
                        }
                        else
                        {
                            rc = pCur.sqlite3BtreeData((u32)offset, (u32)amt, pMem.zBLOB);
                            //pMem.z =  pMem_z ;
                        }
                        //pMem.z[amt] = 0;
                        //pMem.z[amt+1] = 0;
                        if (rc != SqlResult.SQLITE_OK)
                        {
                            pMem.sqlite3VdbeMemRelease();
                        }
                    }
                pMem.n = amt;
                malloc_cs.sqlite3_free(ref zData);
                return rc;
            }
            ///<summary>
            ///This function is only available internally, it is not part of the
            /// external API. It works in a similar way to vdbeapi.sqlite3_value_text(),
            /// except the data returned is in the encoding specified by the second
            /// parameter, which must be one of SqliteEncoding.UTF16BE, SqliteEncoding.UTF16LE or
            /// SqliteEncoding.UTF8.
            ///
            /// (2006-02-16:)  The enc value can be or-ed with SqliteEncoding.UTF16_ALIGNED.
            /// If that is the case, then the result must be aligned on an even byte
            /// boundary.
            ///
            ///</summary>
            public static string sqlite3ValueText(sqlite3_value pVal, SqliteEncoding enc)
            {
                if (pVal == null)
                    return null;
                Debug.Assert(pVal.db == null || pVal.db.mutex.sqlite3_mutex_held());
                Debug.Assert((enc & (SqliteEncoding)3) == (enc & ~SqliteEncoding.UTF16_ALIGNED));
                Debug.Assert((pVal.flags & MemFlags.MEM_RowSet) == 0);
                if ((pVal.flags & MemFlags.MEM_Null) != 0)
                {
                    return null;
                }
                Debug.Assert((MemFlags)((int)MemFlags.MEM_Blob >> 3) == MemFlags.MEM_Str);
                pVal.flags |= (MemFlags)((int)(pVal.flags & MemFlags.MEM_Blob) >> 3);
                if ((pVal.flags & MemFlags.MEM_Zero) != 0)
                    pVal.sqlite3VdbeMemExpandBlob();
                // expandBlob(pVal);
                if ((pVal.flags & MemFlags.MEM_Str) != 0)
                {
                    if (sqlite3VdbeChangeEncoding(pVal, enc & ~SqliteEncoding.UTF16_ALIGNED) != SqlResult.SQLITE_OK)
                    {
                        return null;
                        // Encoding Error
                    }
                    if ((enc & SqliteEncoding.UTF16_ALIGNED) != 0 && 1 == (1 & (pVal.z[0])))//1==(1&SQLITE_PTR_TO_INT(pVal.z))
                    {
                        Debug.Assert((pVal.flags & (MemFlags.MEM_Ephem | MemFlags.MEM_Static)) != 0);
                        if (pVal.sqlite3VdbeMemMakeWriteable() != SqlResult.SQLITE_OK)
                        {
                            return null;
                        }
                    }
                    sqlite3VdbeMemNulTerminate(pVal);
                    ///
                    ///<summary>
                    ///</summary>
                    ///<param name="IMP: R">45467 </param>
                }
                else
                {
                    Debug.Assert((pVal.flags & MemFlags.MEM_Blob) == 0);
                    vdbemem_cs.sqlite3VdbeMemStringify(pVal, enc);
                    //  assert( 0==(1&SQLITE_PTR_TO_INT(pVal->z)) );
                }
                Debug.Assert(pVal.enc == (enc & ~SqliteEncoding.UTF16_ALIGNED) || pVal.db == null//|| pVal.db.mallocFailed != 0
                );
                if (pVal.enc == (enc & ~SqliteEncoding.UTF16_ALIGNED))
                {
                    return pVal.z;
                }
                else
                {
                    return null;
                }
            }
            ///<summary>
            /// Create a new sqlite3_value object.
            ///
            ///</summary>
            public static sqlite3_value sqlite3ValueNew(Connection db)
            {
                Mem p = null;
                p = malloc_cs.sqlite3DbMallocZero(db, p);
                //if ( p != null )
                //{
                p.flags = MemFlags.MEM_Null;
                p.type = FoundationalType.SQLITE_NULL;
                p.db = db;
                //}
                return p;
            }
            ///<summary>
            /// Create a new sqlite3_value object, containing the value of pExpr.
            ///
            /// This only works for very simple expressions that consist of one constant
            /// token (i.e. "5", "5.1", "'a string'"). If the expression can
            /// be converted directly into a value, then the value is allocated and
            /// a pointer written to ppVal. The caller is responsible for deallocating
            /// the value by passing it to sqlite3ValueFree() later on. If the expression
            /// cannot be converted to a value, then ppVal is set to NULL.
            ///
            ///</summary>
            public static SqlResult sqlite3ValueFromExpr(Connection db,///
                ///<summary>
                ///The database connection 
                ///</summary>
            Expr pExpr,///
                ///<summary>
                ///The expression to evaluate 
                ///</summary>
            SqliteEncoding enc,///
                ///<summary>
                ///Encoding to use 
                ///</summary>
            char affinity,///
                ///<summary>
                ///Affinity to use 
                ///</summary>
            ref sqlite3_value ppVal///
                ///<summary>
                ///Write the new value here 
                ///</summary>
            )
            {
                int op;
                string zVal = "";
                sqlite3_value pVal = null;
                int negInt = 1;
                string zNeg = "";
                if (pExpr == null)
                {
                    ppVal = null;
                    return SqlResult.SQLITE_OK;
                }
                op = pExpr.op;
                ///
                ///<summary>
                ///op can only be Sqlite3.TK_REGISTER if we have compiled with SQLITE_ENABLE_STAT2.
                ///The ifdef here is to enable us to achieve 100% branch test coverage even
                ///when SQLITE_ENABLE_STAT2 is omitted.
                ///
                ///</summary>
#if SQLITE_ENABLE_STAT2
																																																																		      if ( op == Sqlite3.TK_REGISTER )
        op = pExpr.op2;
#else
                if (Sqlite3.NEVER(op == Sqlite3.TK_REGISTER))
                    op = pExpr.op2;
#endif
                ///
                ///<summary>
                ///Handle negative integers in a single step.  This is needed in the
                ///</summary>
                ///<param name="case when the value is ">9223372036854775808.</param>
                if (op == Sqlite3.TK_UMINUS && (pExpr.pLeft.op == Sqlite3.TK_INTEGER || pExpr.pLeft.op == Sqlite3.TK_FLOAT))
                {
                    pExpr = pExpr.pLeft;
                    op = pExpr.op;
                    negInt = -1;
                    zNeg = "-";
                }
                if (op == Sqlite3.TK_STRING || op == Sqlite3.TK_FLOAT || op == Sqlite3.TK_INTEGER)
                {
                    pVal = sqlite3ValueNew(db);
                    if (pVal == null)
                        goto no_mem;
                    if (pExpr.ExprHasProperty(ExprFlags.EP_IntValue))
                    {
                        pVal.sqlite3VdbeMemSetInt64((i64)pExpr.u.iValue * negInt);
                    }
                    else
                    {
                        zVal = io.sqlite3MPrintf(db, "%s%s", zNeg, pExpr.u.zToken);
                        //if ( zVal == null ) goto no_mem;
                        sqlite3ValueSetStr(pVal, -1, zVal, SqliteEncoding.UTF8, sqliteinth.SQLITE_DYNAMIC);
                        if (op == Sqlite3.TK_FLOAT)
                            pVal.type = FoundationalType.SQLITE_FLOAT;
                    }
                    if ((op == Sqlite3.TK_INTEGER || op == Sqlite3.TK_FLOAT) && affinity == sqliteinth.SQLITE_AFF_NONE)
                    {
                        Sqlite3.sqlite3ValueApplyAffinity(pVal, sqliteinth.SQLITE_AFF_NUMERIC, SqliteEncoding.UTF8);
                    }
                    else
                    {
                        Sqlite3.sqlite3ValueApplyAffinity(pVal, affinity, SqliteEncoding.UTF8);
                    }
                    if ((pVal.flags & (MemFlags.MEM_Int | MemFlags.MEM_Real)) != 0)
                        pVal.flags = (pVal.flags & ~MemFlags.MEM_Str);
                    if (enc != SqliteEncoding.UTF8)
                    {
                        sqlite3VdbeChangeEncoding(pVal, enc);
                    }
                }
                if (enc != SqliteEncoding.UTF8)
                {
                    sqlite3VdbeChangeEncoding(pVal, enc);
                }
                else
                    if (op == Sqlite3.TK_UMINUS)
                    {
                        ///
                        ///<summary>
                        ///</summary>
                        ///<param name="This branch happens for multiple negative signs.  Ex: ">5) </param>
                        if (SqlResult.SQLITE_OK == sqlite3ValueFromExpr(db, pExpr.pLeft, enc, affinity, ref pVal))
                        {
                            pVal.sqlite3VdbeMemNumerify();
                            if (pVal.u.i == IntegerExtensions.SMALLEST_INT64)
                            {
                                pVal.flags &= MemFlags.MEM_Int;
                                pVal.flags |= MemFlags.MEM_Real;
                                pVal.r = (double)IntegerExtensions.LARGEST_INT64;
                            }
                            else
                            {
                                pVal.u.i = -pVal.u.i;
                            }
                            pVal.r = -pVal.r;
                            Sqlite3.sqlite3ValueApplyAffinity(pVal, affinity, enc);
                        }
                    }
                    else
                        if (op == Sqlite3.TK_NULL)
                        {
                            pVal = sqlite3ValueNew(db);
                            if (pVal == null)
                                goto no_mem;
                        }
#if !SQLITE_OMIT_BLOB_LITERAL
                        else
                            if (op == Sqlite3.TK_BLOB)
                            {
                                int nVal;
                                Debug.Assert(pExpr.u.zToken[0] == 'x' || pExpr.u.zToken[0] == 'X');
                                Debug.Assert(pExpr.u.zToken[1] == '\'');
                                pVal = sqlite3ValueNew(db);
                                if (null == pVal)
                                    goto no_mem;
                                zVal = pExpr.u.zToken.Substring(2);
                                nVal = StringExtensions.sqlite3Strlen30(zVal) - 1;
                                Debug.Assert(zVal[nVal] == '\'');
                                byte[] blob = Converter.sqlite3HexToBlob(db, zVal, nVal);
                                pVal.sqlite3VdbeMemSetStr(Encoding.UTF8.GetString(blob, 0, blob.Length), nVal / 2, 0, sqliteinth.SQLITE_DYNAMIC);
                            }
#endif
                if (pVal != null)
                {
                    Sqlite3.sqlite3VdbeMemStoreType(pVal);
                }
                ppVal = pVal;
                return SqlResult.SQLITE_OK;
            no_mem:
                //db.mallocFailed = 1;
                db.sqlite3DbFree(ref zVal);
                pVal = null;
                // sqlite3ValueFree(pVal);
                ppVal = null;
                return SqlResult.SQLITE_NOMEM;
            }
            ///<summary>
            /// Change the string value of an sqlite3_value object
            ///
            ///</summary>
            public static void sqlite3ValueSetStr(sqlite3_value v,///
                ///<summary>
                ///Value to be set 
                ///</summary>
            int n,///
                ///<summary>
                ///Length of string z 
                ///</summary>
            string z,///
                ///<summary>
                ///Text of the new string 
                ///</summary>
            SqliteEncoding enc,///
                ///<summary>
                ///Encoding to use 
                ///</summary>
            dxDel xDel//)(void*) /* Destructor for the string */
            )
            {
                if (v != null)
                    v.sqlite3VdbeMemSetStr(z, n, enc, xDel);
            }
            ///<summary>
            /// Free an sqlite3_value object
            ///
            ///</summary>
            public static void sqlite3ValueFree(ref sqlite3_value v)
            {
                if (v == null)
                    return;
                v.sqlite3VdbeMemRelease();
                v.db.sqlite3DbFree(ref v);
            }
            ///
            ///<summary>
            ///Return the number of bytes in the sqlite3_value object assuming
            ///that it uses the encoding "enc"
            ///
            ///</summary>
            public static int sqlite3ValueBytes(sqlite3_value pVal, SqliteEncoding enc)
            {
                Mem p = (Mem)pVal;
                if ((p.flags & MemFlags.MEM_Blob) != 0 || sqlite3ValueText(pVal, enc) != null)
                {
                    if ((p.flags & MemFlags.MEM_Zero) != 0)
                    {
                        return p.n + p.u.nZero;
                    }
                    else
                    {
                        return p.z == null ? p.zBLOB.Length : p.n;
                    }
                }
                return 0;
            }
        }
    
}