using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Pgno = System.UInt32;
using i64 = System.Int64;
using u32 = System.UInt32;
using BITVEC_TELEM = System.Byte;

namespace Community.CsharpSqlite.Utils{

    public class Bitvec
    {
        ///<summary>
        ///Maximum bit index.  Max iSize is 4,294,967,296. 
        ///</summary>
        public u32 iSize;


        ///<param name="Number of bits that are set "> only valid for aHash</param>
        ///<param name="element.  Max is BITVEC_NINT.  For BITVEC_SZ of 512,">element.  Max is BITVEC_NINT.  For BITVEC_SZ of 512,</param>
        ///<param name="this would be 125. ">this would be 125. </param>
        public u32 nSet;

        
        public u32 iDivisor;

        ///
        ///<summary>
        ///Number of bits handled by each apSub[] entry. 
        ///</summary>

        ///
        ///<summary>
        ///Should >=0 for apSub element. 
        ///</summary>

        ///
        ///<summary>
        ///Max iDivisor is max(u32) / BITVEC_NPTR + 1.  
        ///</summary>

        ///
        ///<summary>
        ///For a BITVEC_SZ of 512, this would be 34,359,739. 
        ///</summary>

        public _u u = new _u();

        public static implicit operator bool(Bitvec b)
        {
            return (b != null);
        }







        ///<summary>
        /// Create a new bitmap object able to handle bits between 0 and iSize,
        /// inclusive.  Return a pointer to the new object.  Return NULL if
        /// malloc fails.
        ///</summary>
        public static Bitvec sqlite3BitvecCreate(u32 iSize)
        {
            Bitvec p;
            //Debug.Assert( sizeof(p)==BITVEC_SZ );
            p = new Bitvec();
            //malloc_cs.sqlite3MallocZero( sizeof(p) );
            if (p != null)
            {
                p.iSize = iSize;
            }
            return p;
        }
    };


    ///
    ///<summary>
    ///2008 February 16
    ///
    ///The author disclaims copyright to this source code.  In place of
    ///a legal notice, here is a blessing:
    ///
    ///May you do good and not evil.
    ///May you find forgiveness for yourself and forgive others.
    ///May you share freely, never taking more than you give.
    ///
    ///
    ///</summary>
    ///<param name="This file implements an object that represents a fixed">length</param>
    ///<param name="bitmap.  Bits are numbered starting with 1.">bitmap.  Bits are numbered starting with 1.</param>
    ///<param name=""></param>
    ///<param name="A bitmap is used to record which pages of a database file have been">A bitmap is used to record which pages of a database file have been</param>
    ///<param name="journalled during a transaction, or which pages have the "dont">write"</param>
    ///<param name="property.  Usually only a few pages are meet either condition.">property.  Usually only a few pages are meet either condition.</param>
    ///<param name="So the bitmap is usually sparse and has low cardinality.">So the bitmap is usually sparse and has low cardinality.</param>
    ///<param name="But sometimes (for example when during a DROP of a large table) most">But sometimes (for example when during a DROP of a large table) most</param>
    ///<param name="or all of the pages in a database can get journalled.  In those cases,">or all of the pages in a database can get journalled.  In those cases,</param>
    ///<param name="the bitmap becomes dense with high cardinality.  The algorithm needs">the bitmap becomes dense with high cardinality.  The algorithm needs</param>
    ///<param name="to handle both cases well.">to handle both cases well.</param>
    ///<param name=""></param>
    ///<param name="The size of the bitmap is fixed when the object is created.">The size of the bitmap is fixed when the object is created.</param>
    ///<param name=""></param>
    ///<param name="All bits are clear when the bitmap is created.  Individual bits">All bits are clear when the bitmap is created.  Individual bits</param>
    ///<param name="may be set or cleared one at a time.">may be set or cleared one at a time.</param>
    ///<param name=""></param>
    ///<param name="Test operations are about 100 times more common that set operations.">Test operations are about 100 times more common that set operations.</param>
    ///<param name="Clear operations are exceedingly rare.  There are usually between">Clear operations are exceedingly rare.  There are usually between</param>
    ///<param name="5 and 500 set operations per Bitvec object, though the number of sets can">5 and 500 set operations per Bitvec object, though the number of sets can</param>
    ///<param name="sometimes grow into tens of thousands or larger.  The size of the">sometimes grow into tens of thousands or larger.  The size of the</param>
    ///<param name="Bitvec object is the number of pages in the database file at the">Bitvec object is the number of pages in the database file at the</param>
    ///<param name="start of a transaction, and is thus usually less than a few thousand,">start of a transaction, and is thus usually less than a few thousand,</param>
    ///<param name="but can be as large as 2 billion for a really big database.">but can be as large as 2 billion for a really big database.</param>
    ///<param name=""></param>
    ///<param name="Included in SQLite3 port to C#">SQLite;  2008 Noah B Hart</param>
    ///<param name="C#">SQLite is an independent reimplementation of the SQLite software library</param>
    ///<param name=""></param>
    ///<param name="SQLITE_SOURCE_ID: 2010">23 18:52:01 42537b60566f288167f1b5864a5435986838e3a3</param>
    ///<param name=""></param>
    ///<param name=""></param>

    //#include "sqliteInt.h"






    public static partial class BitvecExtensions {
        #region defines
        ///
        ///<summary>
        ///Size of the Bitvec structure in bytes. 
        ///</summary>

        static int BITVEC_SZ = 512;

        ///
        ///<summary>
        ///Round the union size down to the nearest pointer boundary, since that's how
        ///it will be aligned within the Bitvec struct. 
        ///</summary>

        //#define BITVEC_USIZE     (((BITVEC_SZ-(3*sizeof(u32)))/sizeof(Bitvec*))*sizeof(Bitvec*))
        static int BITVEC_USIZE = (((BITVEC_SZ - (3 * sizeof(u32))) / 4) * 4);

        ///
        ///<summary>
        ///Type of the array "element" for the bitmap representation.
        ///Should be a power of 2, and ideally, evenly divide into BITVEC_USIZE.
        ///Setting this to the "natural word" size of your CPU may improve
        ///performance. 
        ///</summary>

        //#define BITVEC_TELEM     u8
        //using BITVEC_TELEM     = System.Byte;
        ///
        ///<summary>
        ///Size, in bits, of the bitmap element. 
        ///</summary>

        //#define BITVEC_SZELEM    8
        public const int BITVEC_SZELEM = 8;

        ///
        ///<summary>
        ///Number of elements in a bitmap array. 
        ///</summary>

        //#define BITVEC_NELEM     (BITVEC_USIZE/sizeof(BITVEC_TELEM))
        public static int BITVEC_NELEM = (int)(BITVEC_USIZE / sizeof(BITVEC_TELEM));

        ///
        ///<summary>
        ///Number of bits in the bitmap array. 
        ///</summary>

        //#define BITVEC_NBIT      (BITVEC_NELEM*BITVEC_SZELEM)
        public static int BITVEC_NBIT = (BITVEC_NELEM * BITVEC_SZELEM);

        ///
        ///<summary>
        ///Number of u32 values in hash table. 
        ///</summary>

        //#define BITVEC_NINT      (BITVEC_USIZE/sizeof(u32))
        public static u32 BITVEC_NINT = (u32)(BITVEC_USIZE / sizeof(u32));

        ///<summary>
        ///Maximum number of entries in hash table before
        /// sub-dividing and re-hashing.
        ///</summary>
        //#define BITVEC_MXHASH    (BITVEC_NINT/2)
        public static int BITVEC_MXHASH = (int)(BITVEC_NINT / 2);




        ///<summary>
        ///Hashing function for the aHash representation.
        /// Empirical testing showed that the *37 multiplier
        /// (an arbitrary prime)in the hash function provided
        /// no fewer collisions than the no-op *1.
        ///</summary>
        //#define BITVEC_HASH(X)   (((X)*1)%BITVEC_NINT)
        public static u32 BITVEC_HASH(u32 X)
        {
            return (u32)(((X) * 1) % BITVEC_NINT);
        }

        public static int BITVEC_NPTR = (int)(BITVEC_USIZE / 4);



        #endregion


    }

    //sizeof(Bitvec *));
    ///<summary>
    /// A bitmap is an instance of the following structure.
    ///
    /// This bitmap records the existence of zero or more bits
    /// with values between 1 and iSize, inclusive.
    ///
    /// There are three possible representations of the bitmap.
    /// If iSize<=BITVEC_NBIT, then Bitvec.u.aBitmap[] is a straight
    /// bitmap.  The least significant bit is bit 1.
    ///
    /// If iSize>BITVEC_NBIT and iDivisor==0 then Bitvec.u.aHash[] is
    /// a hash table that will hold up to BITVEC_MXHASH distinct values.
    ///
    /// Otherwise, the value i is redirected into one of BITVEC_NPTR
    /// sub-bitmaps pointed to by Bitvec.u.apSub[].  Each subbitmap
    /// handles up to iDivisor separate values of i.  apSub[0] holds
    /// values between 1 and iDivisor.  apSub[1] holds values between
    /// iDivisor+1 and 2*iDivisor.  apSub[N] holds values between
    /// N*iDivisor+1 and (N+1)*iDivisor.  Each subbitmap is normalized
    /// to hold deal with values between 1 and iDivisor.
    ///</summary>
    public class _u
		{
			public BITVEC_TELEM[] aBitmap = new byte[BitvecExtensions.BITVEC_NELEM];

			///
///<summary>
///Bitmap representation 
///</summary>

			public u32[] aHash = new u32[BitvecExtensions.BITVEC_NINT];

			///
///<summary>
///Hash table representation 
///</summary>

			public Bitvec[] apSub = new Bitvec[BitvecExtensions.BITVEC_NPTR];
		///
///<summary>
///Recursive representation 
///</summary>

		}
   
}
