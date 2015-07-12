using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Pgno = System.UInt32;
using i64 = System.Int64;
using u32 = System.UInt32;
using BITVEC_TELEM = System.Byte;
using System.Diagnostics;
using Community.CsharpSqlite.Utils;

namespace Community.CsharpSqlite.Utils
{
	public static partial class BitvecExtensions
	{
		


        ///
        ///<summary>
        ///Return the value of the iSize parameter specified when Bitvec *p
        ///was created.
        ///</summary>

        public static u32 sqlite3BitvecSize(this Bitvec p)
        {
            return p.iSize;
        }



        ///<summary>
        /// Set the i-th bit.  Return 0 on success and an error code if
        /// anything goes wrong.
        ///
        /// This routine might cause sub-bitmaps to be allocated.  Failing
        /// to get the memory needed to hold the sub-bitmap is the only
        /// that can go wrong with an insert, assuming p and i are valid.
        ///
        /// The calling function must ensure that p is a valid Bitvec object
        /// and that the value for "i" is within range of the Bitvec object.
        /// Otherwise the behavior is undefined.
        ///</summary>
        public static SqlResult sqlite3BitvecSet(this Bitvec p, u32 i)
        {
            u32 h;
            if (p == null)
                return SqlResult.SQLITE_OK;
            Debug.Assert(i > 0);
            Debug.Assert(i <= p.iSize);
            i--;
            while ((p.iSize > BITVEC_NBIT) && p.iDivisor != 0)
            {
                u32 bin = i / p.iDivisor;
                i = i % p.iDivisor;
                if (p.u.apSub[bin] == null)
                {
                    p.u.apSub[bin] = Bitvec.sqlite3BitvecCreate(p.iDivisor);
                    //if ( p.u.apSub[bin] == null )
                    //  return SQLITE_NOMEM;
                }
                p = p.u.apSub[bin];
            }
            if (p.iSize <= BITVEC_NBIT)
            {
                p.u.aBitmap[i / BITVEC_SZELEM] |= (byte)(1 << (int)(i & (BITVEC_SZELEM - 1)));
                return SqlResult.SQLITE_OK;
            }
            h = BITVEC_HASH(i++);
            ///
            ///<summary>
            ///if there wasn't a hash collision, and this doesn't 
            ///</summary>

            ///
            ///<summary>
            ///completely fill the hash, then just add it without 
            ///</summary>

            ///
            ///<summary>
            ///</summary>
            ///<param name="worring about sub">hashing. </param>

            if (0 == p.u.aHash[h])
            {
                if (p.nSet < (BITVEC_NINT - 1))
                {
                    goto bitvec_set_end;
                }
                else
                {
                    goto bitvec_set_rehash;
                }
            }
            ///
            ///<summary>
            ///there was a collision, check to see if it's already 
            ///</summary>

            ///
            ///<summary>
            ///in hash, if not, try to find a spot for it 
            ///</summary>

            do
            {
                if (p.u.aHash[h] == i)
                    return SqlResult.SQLITE_OK;
                h++;
                if (h >= BITVEC_NINT)
                    h = 0;
            }
            while (p.u.aHash[h] != 0);
            ///
            ///<summary>
            ///we didn't find it in the hash.  h points to the first 
            ///</summary>

            ///
            ///<summary>
            ///available free spot. check to see if this is going to 
            ///</summary>

            ///
            ///<summary>
            ///make our hash too "full".  
            ///</summary>

            bitvec_set_rehash:
            if (p.nSet >= BITVEC_MXHASH)
            {
                u32 j;
                SqlResult rc;
                u32[] aiValues = new u32[BITVEC_NINT];
                // = sqlite3StackAllocRaw(0, sizeof(p->u.aHash));
                //if ( aiValues == null )
                //{
                //  return SQLITE_NOMEM;
                //}
                //else
                {
                    Buffer.BlockCopy(p.u.aHash, 0, aiValues, 0, aiValues.Length * (sizeof(u32)));
                    // memcpy(aiValues, p->u.aHash, sizeof(p->u.aHash));
                    p.u.apSub = new Bitvec[BITVEC_NPTR];
                    //memset(p->u.apSub, 0, sizeof(p->u.apSub));
                    p.iDivisor = (u32)((p.iSize + BITVEC_NPTR - 1) / BITVEC_NPTR);
                    rc = sqlite3BitvecSet(p, i);
                    for (j = 0; j < BITVEC_NINT; j++)
                    {
                        if (aiValues[j] != 0)
                            rc |= sqlite3BitvecSet(p, aiValues[j]);
                    }
                    //sqlite3StackFree( null, aiValues );
                    return rc;
                }
            }
            bitvec_set_end:
            p.nSet++;
            p.u.aHash[h] = i;
            return SqlResult.SQLITE_OK;
        }

        ///<summary>
        /// Clear the i-th bit.
        ///
        /// pBuf must be a pointer to at least BITVEC_SZ bytes of temporary storage
        /// that BitvecClear can use to rebuilt its hash table.
        ///</summary>
        public static void sqlite3BitvecClear(this Bitvec p, u32 i, u32[] pBuf)
        {
            if (p == null)
                return;
            Debug.Assert(i > 0);
            i--;
            while (p.iDivisor != 0)
            {
                u32 bin = i / p.iDivisor;
                i = i % p.iDivisor;
                p = p.u.apSub[bin];
                if (null == p)
                {
                    return;
                }
            }
            if (p.iSize <= BITVEC_NBIT)
            {
                p.u.aBitmap[i / BITVEC_SZELEM] &= (byte)~((1 << (int)(i & (BITVEC_SZELEM - 1))));
            }
            else
            {
                u32 j;
                u32[] aiValues = pBuf;
                Array.Copy(p.u.aHash, aiValues, p.u.aHash.Length);
                //memcpy(aiValues, p->u.aHash, sizeof(p->u.aHash));
                p.u.aHash = new u32[aiValues.Length];
                // memset(p->u.aHash, 0, sizeof(p->u.aHash));
                p.nSet = 0;
                for (j = 0; j < BITVEC_NINT; j++)
                {
                    if (aiValues[j] != 0 && aiValues[j] != (i + 1))
                    {
                        u32 h = BITVEC_HASH(aiValues[j] - 1);
                        p.nSet++;
                        while (p.u.aHash[h] != 0)
                        {
                            h++;
                            if (h >= BITVEC_NINT)
                                h = 0;
                        }
                        p.u.aHash[h] = aiValues[j];
                    }
                }
            }
        }




























        ///<summary>
		/// Check to see if the i-th bit is set.  Return true or false.
		/// If p is NULL (if the bitmap has not been created) or if
		/// i is out of range, then return false.
		///</summary>
		public static int sqlite3BitvecTest(this Bitvec p, u32 i)
        {
            if (p == null || i == 0)
                return 0;
            if (i > p.iSize)
                return 0;
            i--;
            while (p.iDivisor != 0)
            {
                u32 bin = i / p.iDivisor;
                i = i % p.iDivisor;
                p = p.u.apSub[bin];
                if (null == p)
                {
                    return 0;
                }
            }
            if (p.iSize <= BITVEC_NBIT)
            {
                return ((p.u.aBitmap[i / BITVEC_SZELEM] & (1 << (int)(i & (BITVEC_SZELEM - 1)))) != 0) ? 1 : 0;
            }
            else
            {
                u32 h = BITVEC_HASH(i++);
                while (p.u.aHash[h] != 0)
                {
                    if (p.u.aHash[h] == i)
                        return 1;
                    h = (h + 1) % BITVEC_NINT;
                }
                return 0;
            }
        }


        ///<summary>
        /// Destroy a bitmap object.  Reclaim all memory used.
        ///</summary>
        public static void sqlite3BitvecDestroy(ref Bitvec p)
        {
            if (p == null)
                return;
            if (p.iDivisor != 0)
            {
                u32 i;
                for (i = 0; i < BITVEC_NPTR; i++)
                {
                    sqlite3BitvecDestroy(ref p.u.apSub[i]);
                }
            }
            //malloc_cs.sqlite3_free( ref p );
        }



#if !SQLITE_OMIT_BUILTIN_TEST
        ///<summary>
        /// Let V[] be an array of unsigned characters sufficient to hold
        /// up to N bits.  Let I be an integer between 0 and N.  0<=I<N.
        /// Then the following macros can be used to set, clear, or test
        /// individual bits within V.
        ///</summary>
        //#define SETBIT(V,I)      V[I>>3] |= (1<<(I&7))
        static void SETBIT(byte[] V, int I)
        {
            V[I >> 3] |= (byte)(1 << (I & 7));
        }

        //#define CLEARBIT(V,I)    V[I>>3] &= ~(1<<(I&7))
        static void CLEARBIT(byte[] V, int I)
        {
            V[I >> 3] &= (byte)~(1 << (I & 7));
        }

        //#define TESTBIT(V,I)     (V[I>>3]&(1<<(I&7)))!=0
        static int TESTBIT(byte[] V, int I)
        {
            return (V[I >> 3] & (1 << (I & 7))) != 0 ? 1 : 0;
        }

        ///
        ///<summary>
        ///This routine runs an extensive test of the Bitvec code.
        ///
        ///The input is an array of integers that acts as a program
        ///to test the Bitvec.  The integers are opcodes followed
        ///by 0, 1, or 3 operands, depending on the opcode.  Another
        ///opcode follows immediately after the last operand.
        ///
        ///There are 6 opcodes numbered from 0 through 5.  0 is the
        ///"halt" opcode and causes the test to end.
        ///
        ///0          Halt and return the number of errors
        ///1 N S X    Set N bits beginning with S and incrementing by X
        ///2 N S X    Clear N bits beginning with S and incrementing by X
        ///3 N        Set N randomly chosen bits
        ///4 N        Clear N randomly chosen bits
        ///5 N S X    Set N bits from S increment X in array only, not in bitvec
        ///
        ///The opcodes 1 through 4 perform set and clear operations are performed
        ///on both a Bitvec object and on a linear array of bits obtained from malloc.
        ///Opcode 5 works on the linear array only, not on the Bitvec.
        ///Opcode 5 is used to deliberately induce a fault in order to
        ///confirm that error detection works.
        ///
        ///At the conclusion of the test the linear array is compared
        ///against the Bitvec object.  If there are any differences,
        ///an error is returned.  If they are the same, zero is returned.
        ///
        ///</summary>
        ///<param name="If a memory allocation error occurs, return ">1.</param>

        public static int sqlite3BitvecBuiltinTest(u32 sz, int[] aOp)
        {
            Bitvec pBitvec = null;
            byte[] pV = null;
            var rc = -1;
            int i, nx, pc, op;
            u32[] pTmpSpace;
            ///
            ///<summary>
            ///Allocate the Bitvec to be tested and a linear array of
            ///bits to act as the reference 
            ///</summary>

            pBitvec = Bitvec.sqlite3BitvecCreate(sz);
            pV = malloc_cs.sqlite3_malloc((int)(sz + 7) / 8 + 1);
            pTmpSpace = new u32[BITVEC_SZ];
            // sqlite3_malloc( BITVEC_SZ );
            if (pBitvec == null || pV == null || pTmpSpace == null)
                goto bitvec_end;
            Array.Clear(pV, 0, (int)(sz + 7) / 8 + 1);
            // memset( pV, 0, ( sz + 7 ) / 8 + 1 );
            ///
            ///<summary>
            ///NULL pBitvec tests 
            ///</summary>

            sqlite3BitvecSet(null, (u32)1);
            sqlite3BitvecClear(null, 1, pTmpSpace);
            ///
            ///<summary>
            ///Run the program 
            ///</summary>

            pc = 0;
            while ((op = aOp[pc]) != 0)
            {
                switch (op)
                {
                    case 1:
                    case 2:
                    case 5:
                        {
                            nx = 4;
                            i = aOp[pc + 2] - 1;
                            aOp[pc + 2] += aOp[pc + 3];
                            break;
                        }
                    case 3:
                    case 4:
                    default:
                        {
                            nx = 2;
                            i64 i64Temp = 0;
                            Sqlite3.sqlite3_randomness(sizeof(i64), ref i64Temp);
                            i = (int)i64Temp;
                            break;
                        }
                }
                if ((--aOp[pc + 1]) > 0)
                    nx = 0;
                pc += nx;
                i = (int)((i & 0x7fffffff) % sz);
                if ((op & 1) != 0)
                {
                    SETBIT(pV, (i + 1));
                    if (op != 5)
                    {
                        if (sqlite3BitvecSet(pBitvec, (u32)i + 1) != 0)
                            goto bitvec_end;
                    }
                }
                else
                {
                    CLEARBIT(pV, (i + 1));
                    sqlite3BitvecClear(pBitvec, (u32)i + 1, pTmpSpace);
                }
            }
            ///
            ///<summary>
            ///Test to make sure the linear array exactly matches the
            ///Bitvec object.  Start with the assumption that they do
            ///</summary>
            ///<param name="match (rc==0).  Change rc to non">zero if a discrepancy</param>
            ///<param name="is found.">is found.</param>
            ///<param name=""></param>

            rc = sqlite3BitvecTest(null, 0) + sqlite3BitvecTest(pBitvec, sz + 1) + sqlite3BitvecTest(pBitvec, 0) + (int)(sqlite3BitvecSize(pBitvec) - sz);
            for (i = 1; i <= sz; i++)
            {
                if ((TESTBIT(pV, i)) != sqlite3BitvecTest(pBitvec, (u32)i))
                {
                    rc = i;
                    break;
                }
            }
            ///
            ///<summary>
            ///Free allocated structure 
            ///</summary>

            bitvec_end:
            //malloc_cs.sqlite3_free( ref pTmpSpace );
            //malloc_cs.sqlite3_free( ref pV );
            sqlite3BitvecDestroy(ref pBitvec);
            return rc;
        }
#endif




    }
}
