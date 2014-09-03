using System;
using System.Diagnostics;
using i64 = System.Int64;
using u8 = System.Byte;
using u32 = System.UInt32;
using u64 = System.UInt64;

namespace Community.CsharpSqlite
{
	public partial class Sqlite3
	{
		///<summary>
		/// 2001 September 15
		///
		/// The author disclaims copyright to this source code.  In place of
		/// a legal notice, here is a blessing:
		///
		///    May you do good and not evil.
		///    May you find forgiveness for yourself and forgive others.
		///    May you share freely, never taking more than you give.
		///
		///
		/// This file contains code to implement a pseudo-random number
		/// generator (PRNG) for SQLite.
		///
		/// Random numbers are used by some of the database backends in order
		/// to generate random integer keys for tables or random filenames.
		///
		///  Included in SQLite3 port to C#-SQLite;  2008 Noah B Hart
		///  C#-SQLite is an independent reimplementation of the SQLite software library
		///
		///  SQLITE_SOURCE_ID: 2010-08-23 18:52:01 42537b60566f288167f1b5864a5435986838e3a3
		///
		///
		///
		///</summary>
		//#include "sqliteInt.h"
		///<summary>
		///All threads share a single random number generator.
		/// This structure is the current state of the generator.
		///
		///</summary>
		public class sqlite3PrngType
		{
			public bool isInit;

			///
///<summary>
///True if initialized 
///</summary>

			public int i;

			public int j;

			///<summary>
			///State variables
			///</summary>
			public u8[] s = new u8[256];

			///
///<summary>
///State variables 
///</summary>

			public sqlite3PrngType Copy ()
			{
				sqlite3PrngType cp = (sqlite3PrngType)MemberwiseClone ();
				cp.s = new u8[s.Length];
				Array.Copy (s, cp.s, s.Length);
				return cp;
			}
		}

		public static sqlite3PrngType sqlite3Prng = new sqlite3PrngType ();

		///<summary>
		/// Get a single 8-bit random value from the RC4 PRNG.  The Mutex
		/// must be held while executing this routine.
		///
		/// Why not just use a library random generator like lrand48() for this?
		/// Because the OP_NewRowid opcode in the VDBE depends on having a very
		/// good source of random numbers.  The lrand48() library function may
		/// well be good enough.  But maybe not.  Or maybe lrand48() has some
		/// subtle problems on some systems that could cause problems.  It is hard
		/// to know.  To minimize the risk of problems due to bad lrand48()
		/// implementations, SQLite uses this random number generator based
		/// on RC4, which we know works very well.
		///
		/// (Later):  Actually, OP_NewRowid does not depend on a good source of
		/// randomness any more.  But we will leave this code in all the same.
		///
		///</summary>
		static u8 randomu8 ()
		{
			u8 t;
			///
///<summary>
///</summary>
///<param name="The "wsdPrng" macro will resolve to the pseudo">random number generator</param>
///<param name="state vector.  If writable static data is unsupported on the target,">state vector.  If writable static data is unsupported on the target,</param>
///<param name="we have to locate the state vector at run">time.  In the more common</param>
///<param name="case where writable static data is supported, wsdPrng can refer directly">case where writable static data is supported, wsdPrng can refer directly</param>
///<param name="to the "sqlite3Prng" state vector declared above.">to the "sqlite3Prng" state vector declared above.</param>
///<param name=""></param>

			#if SQLITE_OMIT_WSD
																																																									struct sqlite3PrngType *p = &GLOBAL(struct sqlite3PrngType, sqlite3Prng);
// define wsdPrng p[0]
#else
			//# define wsdPrng sqlite3Prng
			sqlite3PrngType wsdPrng = sqlite3Prng;
			#endif
			///
///<summary>
///Initialize the state of the random number generator once,
///the first time this routine is called.  The seed value does
///not need to contain a lot of randomness since we are not
///trying to do secure encryption or anything like that...
///
///Nothing in this file or anywhere else in SQLite does any kind of
///</summary>
///<param name="encryption.  The RC4 algorithm is being used as a PRNG (pseudo">random</param>
///<param name="number generator) not as an encryption device.">number generator) not as an encryption device.</param>

			if (!wsdPrng.isInit) {
				int i;
				u8[] k = new u8[256];
				wsdPrng.j = 0;
				wsdPrng.i = 0;
				sqlite3OsRandomness (sqlite3_vfs_find (""), 256, k);
				for (i = 0; i < 255; i++) {
					wsdPrng.s [i] = (u8)i;
				}
				for (i = 0; i < 255; i++) {
					wsdPrng.j = (u8)(wsdPrng.j + wsdPrng.s [i] + k [i]);
					t = wsdPrng.s [wsdPrng.j];
					wsdPrng.s [wsdPrng.j] = wsdPrng.s [i];
					wsdPrng.s [i] = t;
				}
				wsdPrng.isInit = true;
			}
			///
///<summary>
///Generate and return single random u8
///
///</summary>

			wsdPrng.i++;
			t = wsdPrng.s [(u8)wsdPrng.i];
			wsdPrng.j = (u8)(wsdPrng.j + t);
			wsdPrng.s [(u8)wsdPrng.i] = wsdPrng.s [wsdPrng.j];
			wsdPrng.s [wsdPrng.j] = t;
			t += wsdPrng.s [(u8)wsdPrng.i];
			return wsdPrng.s [t];
		}

		///<summary>
		/// Return N random u8s.
		///
		///</summary>
		static void sqlite3_randomness (int N, ref i64 pBuf)
		{
			u8[] zBuf = new u8[N];
			pBuf = 0;
			#if SQLITE_THREADSAFE
																																																									      sqlite3_mutex mutex = sqlite3MutexAlloc( SQLITE_MUTEX_STATIC_PRNG );
#endif
			sqlite3_mutex_enter (mutex);
			while (N-- > 0) {
				pBuf = (u32)((pBuf << 8) + randomu8 ());
				//  zBuf[N] = randomu8();
			}
			sqlite3_mutex_leave (mutex);
		}

		static void sqlite3_randomness (byte[] pBuf, int Offset, int N)
		{
			i64 iBuf = System.DateTime.Now.Ticks;
			#if SQLITE_THREADSAFE
																																																									  sqlite3_mutex mutex = sqlite3MutexAlloc(SQLITE_MUTEX_STATIC_PRNG);
#endif
			sqlite3_mutex_enter (mutex);
			while (N-- > 0) {
				iBuf = (u32)((iBuf << 8) + randomu8 ());
				//  zBuf[N] = randomu8();
				pBuf [Offset++] = (byte)iBuf;
			}
			sqlite3_mutex_leave (mutex);
		}

		#if !SQLITE_OMIT_BUILTIN_TEST
		///<summary>
		/// For testing purposes, we sometimes want to preserve the state of
		/// PRNG and restore the PRNG to its saved state at a later time, or
		/// to reset the PRNG to its initial state.  These routines accomplish
		/// those tasks.
		///
		/// The sqlite3_test_control() interface calls these routines to
		/// control the PRNG.
		///</summary>
		static sqlite3PrngType sqlite3SavedPrng = null;

		static void sqlite3PrngSaveState ()
		{
			sqlite3SavedPrng = sqlite3Prng.Copy ();
			//      memcpy(
			//  &GLOBAL(struct sqlite3PrngType, sqlite3SavedPrng),
			//  &GLOBAL(struct sqlite3PrngType, sqlite3Prng),
			//  sizeof(sqlite3Prng)
			//);
		}

		static void sqlite3PrngRestoreState ()
		{
			sqlite3Prng = sqlite3SavedPrng.Copy ();
			//memcpy(
			//  &GLOBAL(struct sqlite3PrngType, sqlite3Prng),
			//  &GLOBAL(struct sqlite3PrngType, sqlite3SavedPrng),
			//  sizeof(sqlite3Prng)
			//);
		}

		static void sqlite3PrngResetState ()
		{
			sqlite3Prng.isInit = false;
			//  GLOBAL(struct sqlite3PrngType, sqlite3Prng).isInit = 0;
		}
	#endif
	}
}
