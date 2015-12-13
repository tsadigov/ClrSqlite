using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Community.CsharpSqlite.Engine
{
    public static class MemExtensions
    {

        ///<summary>
        /// Make sure pMem.z points to a writable allocation of at least
        /// n bytes.
        ///
        /// If the memory cell currently contains string or blob data
        /// and the third argument passed to this function is true, the
        /// current content of the cell is preserved. Otherwise, it may
        /// be discarded.
        ///
        /// This function sets the MEM.MEM_Dyn flag and clears any xDel callback.
        /// It also clears MEM.MEM_Ephem and MEM.MEM_Static. If the preserve flag is
        /// not set, Mem.n is zeroed.
        ///
        ///</summary>
        public static SqlResult Grow(this Mem pMem, int n, int preserve)
        {
            // TODO -- What do we want to do about this routine?
            //Debug.Assert( 1 >=
            //  ((pMem.zMalloc !=null )? 1 : 0) + //&& pMem.zMalloc==pMem.z) ? 1 : 0) +
            //  (((pMem.flags & MEM.MEM_Dyn)!=0 && pMem.xDel!=null) ? 1 : 0) +
            //  ((pMem.flags & MEM.MEM_Ephem)!=0 ? 1 : 0) +
            //  ((pMem.flags & MEM.MEM_Static)!=0 ? 1 : 0)
            //);
            //assert( (pMem->flags&MEM.MEM_RowSet)==0 );
            //if( n<32 ) n = 32;
            //if( sqlite3DbMallocSize(pMem->db, pMem.zMalloc)<n ){
            if (preserve != 0)
            {
                //& pMem.z==pMem.zMalloc ){
                if (pMem.z == null)
                    pMem.z = "";
                //      sqlite3DbReallocOrFree( pMem.db, pMem.z, n );
                else
                    if (n < pMem.z.Length)
                    pMem.z = pMem.z.Substring(0, n);
                preserve = 0;
            }
            else
            {
                //  sqlite3DbFree(pMem->db,ref pMem.zMalloc);
                pMem.z = "";
                //   sqlite3DbMallocRaw( pMem.db, n );
            }
            //}
            //  if( pMem->z && preserve && pMem->zMalloc && pMem->z!=pMem->zMalloc ){
            // memcpy(pMem.zMalloc, pMem.z, pMem.n);
            //}
            if ((pMem.flags & MemFlags.MEM_Dyn) != 0 && pMem.xDel != null)
            {
                pMem.xDel(ref pMem.z);
            }
            // TODO --pMem.z = pMem.zMalloc;
            if (pMem.z == null)
            {
                pMem.flags = MemFlags.MEM_Null;
            }
            else
            {
                pMem.flags = (pMem.flags & ~(MemFlags.MEM_Ephem | MemFlags.MEM_Static));
            }
            pMem.xDel = null;
            return pMem.z != null ? SqlResult.SQLITE_OK : SqlResult.SQLITE_NOMEM;
        }
        ///


            
            ///<summary>
        ///Return true if the Mem object contains a TEXT or BLOB that is            
        ///too large - whose size exceeds p.db.aLimit[SQLITE_LIMIT_LENGTH].

        public static bool IsTooBig(this Mem p)
        {
            //Debug.Assert( p.db != null );
            if ((p.flags & (MemFlags.MEM_Str | MemFlags.MEM_Blob)) != 0)
            {
                int n = p.n;
                if ((p.flags & MemFlags.MEM_Zero) != 0)
                {
                    n += p.u.nZero;
                }
                return n > p.db.aLimit[Globals.SQLITE_LIMIT_LENGTH];
            }
            return false;
        }

        ///<summary>
        /// Change the value of a Mem to be a string or a BLOB.
        ///
        /// The memory management strategy depends on the value of the xDel
        /// parameter. If the value passed is SQLITE_TRANSIENT, then the
        /// string is copied into a (possibly existing) buffer managed by the
        /// Mem structure. Otherwise, any existing buffer is freed and the
        /// pointer copied.
        ///
        /// If the string is too large (if it exceeds the SQLITE_LIMIT_LENGTH
        /// size limit) then no memory allocation occurs.  If the string can be
        /// stored without allocating memory, then it is.  If a memory allocation
        /// is required to store the string, then value of pMem is unchanged.  In
        /// either case, SQLITE_TOOBIG is returned.
        ///
        ///</summary>
        public static SqlResult MakeBlob(
            this Mem pMem,///Memory cell to set to string value 
            byte[] zBlob,///Blob pointer 
            int n,///Bytes in Blob 
            SqliteEncoding enc,///0 for BLOBs 
            dxDel xDel///Destructor function 
        )
        {
            return pMem.MakeBlob(zBlob, 0, n >= 0 ? n : zBlob.Length, enc, xDel);
        }
        // Call w/o offset
        public static SqlResult MakeBlob(
            this Mem pMem,///Memory cell to set to string value 
            byte[] zBlob,///Blob pointer 
            int offset,///offset into string 
            int n,///Bytes in string, or negative 
            SqliteEncoding enc,///</summary>
            dxDel xDel//)(void*)/* Destructor function */
        )
        {
            int nByte = n;
            ///New value for pMem>n
            int iLimit;
            ///Maximum allowed string or blob size 
            Debug.Assert(pMem.db == null || pMem.db.mutex.sqlite3_mutex_held());
            Debug.Assert((pMem.flags & MemFlags.MEM_RowSet) == 0);
            ///If zBlob is a NULL pointer, set pMem to contain an SQL NULL. 
            if (zBlob == null || zBlob.Length < offset)
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
            if (nByte < 0)
            {
                Debug.Assert(enc != 0);
                if (enc == SqliteEncoding.UTF8)
                {
                    for (nByte = 0; nByte <= iLimit && nByte < zBlob.Length - offset && zBlob[offset + nByte] != 0; nByte++)
                    {
                    }
                }
                else
                {
                    for (nByte = 0; nByte <= iLimit && zBlob[nByte + offset] != 0 || zBlob[offset + nByte + 1] != 0; nByte += 2)
                    {
                    }
                }
            }
            ///
            ///<summary>
            ///The following block sets the new values of Mem.z and Mem.xDel. It
            ///also sets a flag in local variable "flags" to indicate the memory
            ///management (one of MEM.MEM_Dyn or MEM.MEM_Static).
            ///
            ///</summary>
            Debug.Assert(enc == 0);
            {
                pMem.z = null;
                pMem.zBLOB = malloc_cs.sqlite3Malloc(n);
                Buffer.BlockCopy(zBlob, offset, pMem.zBLOB, 0, n);
            }
            pMem.n = nByte;
            pMem.flags = MemFlags.MEM_Blob | MemFlags.MEM_Term;
            pMem.enc = (enc == 0 ? SqliteEncoding.UTF8 : enc);
            pMem.type = (enc == 0 ? FoundationalType.SQLITE_BLOB : FoundationalType.SQLITE_TEXT);
            if (nByte > iLimit)
            {
                return SqlResult.SQLITE_TOOBIG;
            }
            return SqlResult.SQLITE_OK;
        }


#if !SQLITE_OMIT_FLOATING_POINT
        ///<summary>
        /// Delete any previous value and set the value stored in pMem to NULL.
        ///</summary>
        public static void sqlite3VdbeMemSetNull(this Mem pMem)
        {
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
    }

}
