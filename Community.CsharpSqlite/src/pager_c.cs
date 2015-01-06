using System;
using System.Diagnostics;
using System.IO;
using i16 = System.Int16;
using i64 = System.Int64;
using u8 = System.Byte;
using u16 = System.UInt16;
using u32 = System.UInt32;
using Pgno = System.UInt32;
using sqlite3_int64 = System.Int64;

namespace Community.CsharpSqlite
{
    using System.Text;
    using DbPage = Sqlite3.PgHdr;

    public partial class Sqlite3
    {


        ///
            ///<summary>
            ///Journal files begin with the following magic string.  The data
            ///was obtained from /dev/random.  It is used only as a sanity check.
            ///
            ///Since version 2.8.0, the journal format contains additional sanity
            ///checking information.  If the power fails while the journal is being
            ///</summary>
            ///<param name="written, semi">random garbage data might appear in the journal</param>
            ///<param name="file after power is restored.  If an attempt is then made">file after power is restored.  If an attempt is then made</param>
            ///<param name="to roll the journal back, the database could be corrupted.  The additional">to roll the journal back, the database could be corrupted.  The additional</param>
            ///<param name="sanity checking data is an attempt to discover the garbage in the">sanity checking data is an attempt to discover the garbage in the</param>
            ///<param name="journal and ignore it.">journal and ignore it.</param>
            ///<param name=""></param>
            ///<param name="The sanity checking information for the new journal format consists">The sanity checking information for the new journal format consists</param>
            ///<param name="of a 32">bit checksum on each page of data.  The checksum covers both</param>
            ///<param name="the page number and the pPager.pageSize bytes of data for the page.">the page number and the pPager.pageSize bytes of data for the page.</param>
            ///<param name="This cksum is initialized to a 32">bit random value that appears in the</param>
            ///<param name="journal file right after the header.  The random initializer is important,">journal file right after the header.  The random initializer is important,</param>
            ///<param name="because garbage data that appears at the end of a journal is likely">because garbage data that appears at the end of a journal is likely</param>
            ///<param name="data that was once in other files that have now been deleted.  If the">data that was once in other files that have now been deleted.  If the</param>
            ///<param name="garbage data came from an obsolete journal file, the checksums might">garbage data came from an obsolete journal file, the checksums might</param>
            ///<param name="be correct.  But by initializing the checksum to random value which">be correct.  But by initializing the checksum to random value which</param>
            ///<param name="is different for every journal, we minimize that risk.">is different for every journal, we minimize that risk.</param>

        public static byte[] aJournalMagic = new byte[] {
			0xd9,
			0xd5,
			0x05,
			0xf9,
			0x20,
			0xa1,
			0x63,
			0xd7,
		};


        ///
        ///<summary>
        ///</summary>
        ///<param name="The macro MEMDB is true if we are dealing with an in">memory database.</param>
        ///<param name="We do this as a macro so that if the SQLITE_OMIT_MEMORYDB macro is set,">We do this as a macro so that if the SQLITE_OMIT_MEMORYDB macro is set,</param>
        ///<param name="the value of MEMDB will be a constant and the compiler will optimize">the value of MEMDB will be a constant and the compiler will optimize</param>
        ///<param name="out code that would never execute.">out code that would never execute.</param>
        ///<param name=""></param>

#if !SQLITE_OMIT_MEMORYDB
																																						// define MEMDB 0
const int MEMDB = 0;
#else
        //# define MEMDB pPager.memDb
#endif
            


        ///
        ///<summary>
        ///</summary>
        ///<param name="The maximum legal page number is (2^31 "> 1).</param>

        //#define PAGER_MAX_PGNO 2147483647
        const int PAGER_MAX_PGNO = 2147483647;

        ///
        ///<summary>
        ///The Pager.eLock variable is almost always set to one of the 
        ///</summary>
        ///<param name="following locking">states, according to the lock currently held on</param>
        ///<param name="the database file: NO_LOCK, SHARED_LOCK, RESERVED_LOCK or EXCLUSIVE_LOCK.">the database file: NO_LOCK, SHARED_LOCK, RESERVED_LOCK or EXCLUSIVE_LOCK.</param>
        ///<param name="This variable is kept up to date as locks are taken and released by">This variable is kept up to date as locks are taken and released by</param>
        ///<param name="the pagerLockDb() and pagerUnlockDb() wrappers.">the pagerLockDb() and pagerUnlockDb() wrappers.</param>
        ///<param name=""></param>
        ///<param name="If the VFS xLock() or xUnlock() returns an error other than SQLITE_BUSY">If the VFS xLock() or xUnlock() returns an error other than SQLITE_BUSY</param>
        ///<param name="(i.e. one of the SQLITE_IOERR subtypes), it is not clear whether or not">(i.e. one of the SQLITE_IOERR subtypes), it is not clear whether or not</param>
        ///<param name="the operation was successful. In these circumstances pagerLockDb() and">the operation was successful. In these circumstances pagerLockDb() and</param>
        ///<param name="pagerUnlockDb() take a conservative approach "> eLock is always updated</param>
        ///<param name="when unlocking the file, and only updated when locking the file if the">when unlocking the file, and only updated when locking the file if the</param>
        ///<param name="VFS call is successful. This way, the Pager.eLock variable may be set">VFS call is successful. This way, the Pager.eLock variable may be set</param>
        ///<param name="to a less exclusive (lower) value than the lock that is actually held">to a less exclusive (lower) value than the lock that is actually held</param>
        ///<param name="at the system level, but it is never set to a more exclusive value.">at the system level, but it is never set to a more exclusive value.</param>
        ///<param name=""></param>
        ///<param name="This is usually safe. If an xUnlock fails or appears to fail, there may ">This is usually safe. If an xUnlock fails or appears to fail, there may </param>
        ///<param name="be a few redundant xLock() calls or a lock may be held for longer than">be a few redundant xLock() calls or a lock may be held for longer than</param>
        ///<param name="required, but nothing really goes wrong.">required, but nothing really goes wrong.</param>
        ///<param name=""></param>
        ///<param name="The exception is when the database file is unlocked as the pager moves">The exception is when the database file is unlocked as the pager moves</param>
        ///<param name="from ERROR to OPEN state. At this point there may be a hot">journal file </param>
        ///<param name="in the file">>SHARED</param>
        ///<param name="transition, by the same pager or any other). If the call to xUnlock()">transition, by the same pager or any other). If the call to xUnlock()</param>
        ///<param name="fails at this point and the pager is left holding an EXCLUSIVE lock, this">fails at this point and the pager is left holding an EXCLUSIVE lock, this</param>
        ///<param name="can confuse the call to xCheckReservedLock() call made later as part">can confuse the call to xCheckReservedLock() call made later as part</param>
        ///<param name="of hot">journal detection.</param>
        ///<param name=""></param>
        ///<param name="xCheckReservedLock() is defined as returning true "if there is a RESERVED ">xCheckReservedLock() is defined as returning true "if there is a RESERVED </param>
        ///<param name="lock held by this process or any others". So xCheckReservedLock may ">lock held by this process or any others". So xCheckReservedLock may </param>
        ///<param name="return true because the caller itself is holding an EXCLUSIVE lock (but">return true because the caller itself is holding an EXCLUSIVE lock (but</param>
        ///<param name="doesn't know it because of a previous error in xUnlock). If this happens">doesn't know it because of a previous error in xUnlock). If this happens</param>
        ///<param name="a hot">journal may be mistaken for a journal being created by an active</param>
        ///<param name="transaction in another process, causing SQLite to read from the database">transaction in another process, causing SQLite to read from the database</param>
        ///<param name="without rolling it back.">without rolling it back.</param>
        ///<param name=""></param>
        ///<param name="To work around this, if a call to xUnlock() fails when unlocking the">To work around this, if a call to xUnlock() fails when unlocking the</param>
        ///<param name="database in the ERROR state, Pager.eLock is set to UNKNOWN_LOCK. It">database in the ERROR state, Pager.eLock is set to UNKNOWN_LOCK. It</param>
        ///<param name="is only changed back to a real locking state after a successful call">is only changed back to a real locking state after a successful call</param>
        ///<param name="to xLock(EXCLUSIVE). Also, the code to do the OPEN">>SHARED state transition</param>
        ///<param name="omits the check for a hot">journal if Pager.eLock is set to UNKNOWN_LOCK </param>
        ///<param name="lock. Instead, it assumes a hot">journal exists and obtains an EXCLUSIVE</param>
        ///<param name="lock on the database file before attempting to roll it back. See function">lock on the database file before attempting to roll it back. See function</param>
        ///<param name="PagerSharedLock() for more detail.">PagerSharedLock() for more detail.</param>
        ///<param name=""></param>
        ///<param name="Pager.eLock may only be set to UNKNOWN_LOCK when the pager is in ">Pager.eLock may only be set to UNKNOWN_LOCK when the pager is in </param>
        ///<param name="PagerState.PAGER_OPEN state.">PagerState.PAGER_OPEN state.</param>
        ///<param name=""></param>

        //#define UNKNOWN_LOCK                (EXCLUSIVE_LOCK+1)
        const int UNKNOWN_LOCK = (EXCLUSIVE_LOCK + 1);

#if TRACE
																																						
static bool sqlite3PagerTrace = false;  /* True to enable tracing */
//define sqlite3DebugPrintf printf
//define PAGERTRACE(X)     if( sqlite3PagerTrace ){ sqlite3DebugPrintf X; }
static void PAGERTRACE( string T, params object[] ap ) { if ( sqlite3PagerTrace )sqlite3DebugPrintf( T, ap ); }
#else
        //#define PAGERTRACE(X)
        static void PAGERTRACE(string T, params object[] ap)
        {
        }

#endif

        ///
        ///<summary>
        ///The maximum allowed sector size. 64KiB. If the xSectorsize() method 
        ///returns a value larger than this, then MAX_SECTOR_SIZE is used instead.
        ///This could conceivably cause corruption following a power failure on
        ///such a system. This is currently an undocumented limit.
        ///</summary>

        //#define MAX_SECTOR_SIZE 0x10000
        const int MAX_SECTOR_SIZE = 0x10000;




        public class PagerMethods
        {
            ///
            ///<summary>
            ///2001 September 15
            ///
            ///The author disclaims copyright to this source code.  In place of
            ///a legal notice, here is a blessing:
            ///
            ///May you do good and not evil.
            ///May you find forgiveness for yourself and forgive others.
            ///May you share freely, never taking more than you give.
            ///
            ///
            ///This is the implementation of the page cache subsystem or "pager".
            ///
            ///The pager is used to access a database disk file.  It implements
            ///atomic commit and rollback through the use of a journal file that
            ///is separate from the database file.  The pager also implements file
            ///locking to prevent two processes from writing the same database
            ///file simultaneously, or one process from reading the database while
            ///another is writing.
            ///
            ///</summary>
            ///<param name="Included in SQLite3 port to C#">SQLite;  2008 Noah B Hart</param>
            ///<param name="C#">SQLite is an independent reimplementation of the SQLite software library</param>
            ///<param name=""></param>
            ///<param name="SQLITE_SOURCE_ID: 2011">23 19:49:22 4374b7e83ea0a3fbc3691f9c0c936272862f32f2</param>
            ///<param name=""></param>
            ///<param name=""></param>
            ///<param name=""></param>

#if !SQLITE_OMIT_DISKIO
            //#include "sqliteInt.h"
            //#include "wal.h"
            ///
            ///<summary>
            ///NOTES ON THE DESIGN OF THE PAGER ************************
            ///
            ///This comment block describes invariants that hold when using a rollback
            ///journal.  These invariants do not apply for journal_mode=WAL,
            ///journal_mode=MEMORY, or journal_mode=OFF.
            ///
            ///Within this comment block, a page is deemed to have been synced
            ///automatically as soon as it is written when PRAGMA synchronous=OFF.
            ///Otherwise, the page is not synced until the xSync method of the VFS
            ///is called successfully on the file containing the page.
            ///
            ///Definition:  A page of the database file is said to be "overwriteable" if
            ///one or more of the following are true about the page:
            ///
            ///(a)  The original content of the page as it was at the beginning of
            ///the transaction has been written into the rollback journal and
            ///synced.
            ///
            ///(b)  The page was a freelist leaf page at the start of the transaction.
            ///
            ///(c)  The page number is greater than the largest page that existed in
            ///the database file at the start of the transaction.
            ///
            ///(1) A page of the database file is never overwritten unless one of the
            ///following are true:
            ///
            ///(a) The page and all other pages on the same sector are overwriteable.
            ///
            ///(b) The atomic page write optimization is enabled, and the entire
            ///transaction other than the update of the transaction sequence
            ///number consists of a single page change.
            ///
            ///(2) The content of a page written into the rollback journal exactly matches
            ///both the content in the database when the rollback journal was written
            ///and the content in the database at the beginning of the current
            ///transaction.
            ///
            ///(3) Writes to the database file are an integer multiple of the page size
            ///in length and are aligned on a page boundary.
            ///
            ///(4) Reads from the database file are either aligned on a page boundary and
            ///an integer multiple of the page size in length or are taken from the
            ///first 100 bytes of the database file.
            ///
            ///(5) All writes to the database file are synced prior to the rollback journal
            ///being deleted, truncated, or zeroed.
            ///
            ///(6) If a master journal file is used, then all writes to the database file
            ///are synced prior to the master journal being deleted.
            ///
            ///Definition: Two databases (or the same database at two points it time)
            ///are said to be "logically equivalent" if they give the same answer to
            ///all queries.  Note in particular the the content of freelist leaf
            ///pages can be changed arbitarily without effecting the logical equivalence
            ///of the database.
            ///
            ///(7) At any time, if any subset, including the empty set and the total set,
            ///of the unsynced changes to a rollback journal are removed and the 
            ///journal is rolled back, the resulting database file will be logical
            ///equivalent to the database file at the beginning of the transaction.
            ///
            ///(8) When a transaction is rolled back, the xTruncate method of the VFS
            ///is called to restore the database file to the same size it was at
            ///the beginning of the transaction.  (In some VFSes, the xTruncate
            ///</summary>
            ///<param name="method is a no">op, but that does not change the fact the SQLite will</param>
            ///<param name="invoke it.)">invoke it.)</param>
            ///<param name=""></param>
            ///<param name="(9) Whenever the database file is modified, at least one bit in the range">(9) Whenever the database file is modified, at least one bit in the range</param>
            ///<param name="of bytes from 24 through 39 inclusive will be changed prior to releasing">of bytes from 24 through 39 inclusive will be changed prior to releasing</param>
            ///<param name="the EXCLUSIVE lock, thus signaling other connections on the same">the EXCLUSIVE lock, thus signaling other connections on the same</param>
            ///<param name="database to flush their caches.">database to flush their caches.</param>
            ///<param name=""></param>
            ///<param name="(10) The pattern of bits in bytes 24 through 39 shall not repeat in less">(10) The pattern of bits in bytes 24 through 39 shall not repeat in less</param>
            ///<param name="than one billion transactions.">than one billion transactions.</param>
            ///<param name=""></param>
            ///<param name="(11) A database file is well">formed at the beginning and at the conclusion</param>
            ///<param name="of every transaction.">of every transaction.</param>
            ///<param name=""></param>
            ///<param name="(12) An EXCLUSIVE lock is held on the database file when writing to">(12) An EXCLUSIVE lock is held on the database file when writing to</param>
            ///<param name="the database file.">the database file.</param>
            ///<param name=""></param>
            ///<param name="(13) A SHARED lock is held on the database file while reading any">(13) A SHARED lock is held on the database file while reading any</param>
            ///<param name="content out of the database file.">content out of the database file.</param>
            ///<param name=""></param>
            ///<param name=""></param>

            ///<summary>
            /// Macros for troubleshooting.  Normally turned off
            ///
            ///</summary>

            ///<summary>
            /// The following two macros are used within the PAGERTRACE() macros above
            /// to print out file-descriptors.
            ///
            /// PagerMethods.PAGERID() takes a pointer to a Pager struct as its argument. The
            /// associated file-descriptor is returned. FILEHANDLEID() takes an sqlite3_file
            /// struct as its argument.
            ///</summary>
            //#define PagerMethods.PAGERID(p) ((int)(p.fd))
            public static int PAGERID(Pager p)
            {
                return p.GetHashCode();
            }

            //#define FILEHANDLEID(fd) ((int)fd)
            static int FILEHANDLEID(sqlite3_file fd)
            {
                return fd.GetHashCode();
            }

            


            ///<summary>
            /// A macro used for invoking the codec if there is one
            ///
            ///</summary>
            // The E parameter is what executes when there is an error, 
            // cannot implement here, since this is not really a macro
            // calling code must be modified to call E when truen
#if SQLITE_HAS_CODEC
            //# define CODEC1(P,D,N,X,E) \
            //if( P.xCodec && P.xCodec(P.pCodec,D,N,X)==0 ){ E; }
            public static bool CODEC1(Pager P, byte[] D, uint N///
                ///<summary>
                ///page number 
                ///</summary>

            , int X///
                ///<summary>
                ///E (moved to caller 
                ///</summary>

            )
            {
                return ((P.xCodec != null) && (P.xCodec(P.pCodec, D, N, X) == null));
            }

            // The E parameter is what executes when there is an error, 
            // cannot implement here, since this is not really a macro
            // calling code must be modified to call E when truen
            //# define CODEC2(P,D,N,X,E,O) \
            //if( P.xCodec==0 ){ O=(char*)D; }else \
            //if( (O=(char*)(P.xCodec(P.pCodec,D,N,X)))==0 ){ E; }
            public static bool CODEC2(Pager P, byte[] D, uint N, int X, ref byte[] O)
            {
                if (P.xCodec == null)
                {
                    O = D;
                    // do nothing
                    return false;
                }
                else
                {
                    return ((O = P.xCodec(P.pCodec, D, N, X)) == null);
                }
            }

#else
																																						// define CODEC1(P,D,N,X,E)   /* NO-OP */
static bool CODEC1 (Pager P, byte[] D, uint N /* page number */, int X /* E (moved to caller */)  { return false; }
// define CODEC2(P,D,N,X,E,O) O=(char*)D
static bool CODEC2( Pager P, byte[] D, uint N, int X, ref byte[] O ) { O = D; return false; }
#endif







            
            ///<summary>
            /// The following global variables hold counters used for
            /// testing purposes only.  These variables do not exist in
            /// a non-testing build.  These variables are not thread-safe.
            ///
            ///</summary>
#if SQLITE_TEST
#if !TCLSH
																																						    static int sqlite3_pager_readdb_count = 0;    /* Number of full pages read from DB */
    static int sqlite3_pager_writedb_count = 0;   /* Number of full pages written to DB */
    static int sqlite3_pager_writej_count = 0;    /* Number of pages written to journal */
#else
																																						    static tcl.lang.Var.SQLITE3_GETSET sqlite3_pager_readdb_count = new tcl.lang.Var.SQLITE3_GETSET( "sqlite3_pager_readdb_count" );
    static tcl.lang.Var.SQLITE3_GETSET sqlite3_pager_writedb_count = new tcl.lang.Var.SQLITE3_GETSET( "sqlite3_pager_writedb_count" );
    static tcl.lang.Var.SQLITE3_GETSET sqlite3_pager_writej_count = new tcl.lang.Var.SQLITE3_GETSET( "sqlite3_pager_writej_count" );
#endif
																																						    static void PAGER_INCR( ref int v )
    {
      v++;
    }
#else
            //# define PAGER_INCR(v)
            public static void PAGER_INCR(ref int v)
            {
            }

#endif
            

         

            

            //# define pagerRollbackWal(x) 0
            //# define pagerWalFrames(v,w,x,y,z) 0
            //# define pagerOpenWalIfPresent(z) SqlResult.SQLITE_OK
            //# define pagerBeginReadTransaction(z) SqlResult.SQLITE_OK
#endif
#if NDEBUG
            ///
            ///<summary>
            ///Usage:
            ///
            ///Debug.Assert( assert_pager_state(pPager) );
            ///
            ///This function runs many Debug.Asserts to try to find inconsistencies in
            ///the internal state of the Pager object.
            ///</summary>

#else
																																						    static bool assert_pager_state( Pager pPager )
    {
      return true;
    }
#endif
#if SQLITE_DEBUG
																																						    /*
** Return a pointer to a human readable string in a static buffer
** containing the state of the Pager object passed as an argument. This
** is intended to be used within debuggers. For example, as an alternative
** to "print *pPager" in gdb:
**
** (gdb) printf "%s", print_pager_state(pPager)
*/
    static string print_pager_state( Pager p )
    {
      StringBuilder zRet = new StringBuilder( 1024 );

      io.sqlite3_snprintf( 1024, zRet,
      "Filename:      %s\n" +
      "State:         %s errCode=%d\n" +
      "Lock:          %s\n" +
      "Locking mode:  locking_mode=%s\n" +
      "Journal mode:  journal_mode=%s\n" +
      "Backing store: tempFile=%d memDb=%d useJournal=%d\n" +
      "Journal:       journalOff=%lld journalHdr=%lld\n" +
      "Size:          dbsize=%d dbOrigSize=%d dbFileSize=%d\n"
      , p.zFilename
      , p.eState == PagerState.PAGER_OPEN ? "OPEN" :
      p.eState == PagerState.PAGER_READER ? "READER" :
      p.eState == PagerState.PAGER_WRITER_LOCKED ? "WRITER_LOCKED" :
      p.eState == PagerState.PAGER_WRITER_CACHEMOD ? "WRITER_CACHEMOD" :
      p.eState == PagerState.PAGER_WRITER_DBMOD ? "WRITER_DBMOD" :
      p.eState == PagerState.PAGER_WRITER_FINISHED ? "WRITER_FINISHED" :
      p.eState == PagerState.PAGER_ERROR ? "ERROR" : "?error?"
      , (int)p.errCode
      , p.eLock == NO_LOCK ? "NO_LOCK" :
      p.eLock == RESERVED_LOCK ? "RESERVED" :
      p.eLock == EXCLUSIVE_LOCK ? "EXCLUSIVE" :
      p.eLock == SHARED_LOCK ? "SHARED" :
      p.eLock == UNKNOWN_LOCK ? "UNKNOWN" : "?error?"
      , p.exclusiveMode ? "exclusive" : "normal"
      , p.journalMode == PAGER_JOURNALMODE_MEMORY ? "memory" :
      p.journalMode == PAGER_JOURNALMODE_OFF ? "off" :
      p.journalMode == PAGER_JOURNALMODE_DELETE ? "delete" :
      p.journalMode == PAGER_JOURNALMODE_PERSIST ? "persist" :
      p.journalMode == PAGER_JOURNALMODE_TRUNCATE ? "truncate" :
      p.journalMode == PAGER_JOURNALMODE_WAL ? "wal" : "?error?"
      , p.tempFile ? 1 : 0, (int)p.memDb, (int)p.useJournal
      , p.journalOff, p.journalHdr
      , (int)p.dbSize, (int)p.dbOrigSize, (int)p.dbFileSize
      );

      return zRet.ToString();
    }
#endif
            ///<summary>
            /// Read a 32-bit integer from the given file descriptor.  Store the integer
            /// that is read in pRes.  Return SqlResult.SQLITE_OK if everything worked, or an
            /// error code is something goes wrong.
            ///
            /// All values are stored on disk as big-endian.
            ///
            ///</summary>
            public static SqlResult read32bits(sqlite3_file fd, int offset, ref int pRes)
            {
                u32 u32_pRes = 0;
                var rc = PagerMethods.read32bits(fd, offset, ref u32_pRes);
                pRes = (int)u32_pRes;
                return rc;
            }

            public static SqlResult read32bits(sqlite3_file fd, i64 offset, ref u32 pRes)
            {
                var rc = read32bits(fd, (int)offset, ref pRes);
                return rc;
            }

            public static SqlResult read32bits(sqlite3_file fd, int offset, ref u32 pRes)
            {
                byte[] ac = new byte[4];
                SqlResult rc = os.sqlite3OsRead(fd, ac, ac.Length, offset);
                if (rc == SqlResult.SQLITE_OK)
                {
                    pRes = Converter.sqlite3Get4byte(ac);
                }
                else
                    pRes = 0;
                return rc;
            }

            
            ///<summary>
            /// Write a 32-bit integer into the given file descriptor.  Return SqlResult.SQLITE_OK
            /// on success or an error code is something goes wrong.
            ///
            ///</summary>
            public static SqlResult write32bits(sqlite3_file fd, i64 offset, u32 val)
            {
                byte[] ac = new byte[4];
                Converter.put32bits(ac, val);
                return os.sqlite3OsWrite(fd, ac, 4, offset);
            }

            ///
            ///<summary>
            ///Lock the database file to level eLock, which must be either SHARED_LOCK,
            ///RESERVED_LOCK or EXCLUSIVE_LOCK. If the caller is successful, set the
            ///Pager.eLock variable to the new locking state. 
            ///
            ///Except, if Pager.eLock is set to UNKNOWN_LOCK when this function is 
            ///called, do not modify it unless the new locking state is EXCLUSIVE_LOCK. 
            ///See the comment above the #define of UNKNOWN_LOCK for an explanation 
            ///of this.
            ///
            ///</summary>

            ///
            ///<summary>
            ///</summary>
            ///<param name="This function determines whether or not the atomic">write optimization</param>
            ///<param name="can be used with this pager. The optimization can be used if:">can be used with this pager. The optimization can be used if:</param>
            ///<param name=""></param>
            ///<param name="(a) the value returned by OsDeviceCharacteristics() indicates that">(a) the value returned by OsDeviceCharacteristics() indicates that</param>
            ///<param name="a database page may be written atomically, and">a database page may be written atomically, and</param>
            ///<param name="(b) the value returned by OsSectorSize() is less than or equal">(b) the value returned by OsSectorSize() is less than or equal</param>
            ///<param name="to the page size.">to the page size.</param>
            ///<param name=""></param>
            ///<param name="The optimization is also always enabled for temporary files. It is">The optimization is also always enabled for temporary files. It is</param>
            ///<param name="an error to call this function if pPager is opened on an in">memory</param>
            ///<param name="database.">database.</param>
            ///<param name=""></param>
            ///<param name="If the optimization cannot be used, 0 is returned. If it can be used,">If the optimization cannot be used, 0 is returned. If it can be used,</param>
            ///<param name="then the value returned is the size of the journal file when it">then the value returned is the size of the journal file when it</param>
            ///<param name="contains rollback data for exactly one page.">contains rollback data for exactly one page.</param>
            ///<param name=""></param>

#if SQLITE_ENABLE_ATOMIC_WRITE
																																						static int jrnlBufferSize(Pager *pPager){
Debug.Assert( 0==MEMDB );
if( !pPager.tempFile ){
int dc;                           /* Device characteristics */
int nSector;                      /* Sector size */
int szPage;                       /* Page size */

Debug.Assert( pPager.fd.isOpen  );
dc = sqlite3OsDeviceCharacteristics(pPager.fd);
nSector = pPager.sectorSize;
szPage = pPager.pageSize;

Debug.Assert(SQLITE_IOCAP_ATOMIC512==(512>>8));
Debug.Assert(SQLITE_IOCAP_ATOMIC64K==(65536>>8));
if( 0==(dc&(SQLITE_IOCAP_ATOMIC|(szPage>>8)) || nSector>szPage) ){
return 0;
}
}

return JOURNAL_HDR_SZ(pPager) + JOURNAL_PG_SZ(pPager);
}
#endif
            ///<summary>
            /// If SQLITE_CHECK_PAGES is defined then we do some sanity checking
            /// on the cache using a hash function.  This is used for testing
            /// and debugging only.
            ///</summary>
#if SQLITE_CHECK_PAGES
																																						/*
** Return a 32-bit hash of the page data for pPage.
*/
static u32 pager_datahash(int nByte, unsigned char pData){
u32 hash = 0;
int i;
for(i=0; i<nByte; i++){
hash = (hash*1039) + pData[i];
}
return hash;
}
static void pager_pagehash(PgHdr pPage){
return pager_datahash(pPage.pPager.pageSize, (unsigned char *)pPage.pData);
}
static u32 pager_set_pagehash(PgHdr pPage){
pPage.pageHash = pager_pagehash(pPage);
}

/*
** The CHECK_PAGE macro takes a PgHdr* as an argument. If SQLITE_CHECK_PAGES
** is defined, and NDEBUG is not defined, an Debug.Assert() statement checks
** that the page is either dirty or still matches the calculated page-hash.
*/
//define CHECK_PAGE(x) checkPage(x)
static void checkPage(PgHdr pPg){
Pager pPager = pPg.pPager;
assert( pPager->eState!=PagerState.PAGER_ERROR );
assert( (pPg->flags&PGHDR_DIRTY) || pPg->pageHash==pager_pagehash(pPg) );
}

#else
            //#define pager_datahash(X,Y)  0
            public static int pager_datahash(int X, byte[] Y)
            {
                return 0;
            }

            //#define pager_pagehash(X)  0
            //#define pager_set_pagehash(X)
            //#define CHECK_PAGE(x)
#endif
            ///<summary>
            /// When this is called the journal file for pager pPager must be open.
            /// This function attempts to read a master journal file name from the
            /// end of the file and, if successful, copies it into memory supplied
            /// by the caller. See comments above writeMasterJournal() for the format
            /// used to store a master journal file name at the end of a journal file.
            ///
            /// zMaster must point to a buffer of at least nMaster bytes allocated by
            /// the caller. This should be sqlite3_vfs.mxPathname+1 (to ensure there is
            /// enough space to write the master journal name). If the master journal
            /// name in the journal is longer than nMaster bytes (including a
            /// nul-terminator), then this is handled as if no master journal name
            /// were present in the journal.
            ///
            /// If a master journal file name is present at the end of the journal
            /// file, then it is copied into the buffer pointed to by zMaster. A
            /// nul-terminator byte is appended to the buffer following the master
            /// journal file name.
            ///
            /// If it is determined that no master journal file name is present
            /// zMaster[0] is set to 0 and SqlResult.SQLITE_OK returned.
            ///
            /// If an error occurs while reading from the journal file, an SQLite
            /// error code is returned.
            ///</summary>
            public static SqlResult readMasterJournal(sqlite3_file pJrnl, byte[] zMaster, u32 nMaster)
            {
                SqlResult rc;
                ///
                ///<summary>
                ///Return code 
                ///</summary>

                int len = 0;
                ///
                ///<summary>
                ///Length in bytes of master journal name 
                ///</summary>

                i64 szJ = 0;
                ///
                ///<summary>
                ///Total size in bytes of journal file pJrnl 
                ///</summary>

                u32 cksum = 0;
                ///
                ///<summary>
                ///MJ checksum value read from journal 
                ///</summary>

                int u;
                ///
                ///<summary>
                ///Unsigned loop counter 
                ///</summary>

                byte[] aMagic = new byte[8];
                ///
                ///<summary>
                ///A buffer to hold the magic header 
                ///</summary>

                zMaster[0] = 0;
                if (SqlResult.SQLITE_OK != (rc = os.sqlite3OsFileSize(pJrnl, ref szJ)) || szJ < 16 || SqlResult.SQLITE_OK != (rc = PagerMethods.read32bits(pJrnl, (int)(szJ - 16), ref len)) || len >= nMaster || SqlResult.SQLITE_OK != (rc = PagerMethods.read32bits(pJrnl, szJ - 12, ref cksum)) || SqlResult.SQLITE_OK != (rc = os.sqlite3OsRead(pJrnl, aMagic, 8, szJ - 8)) || _Custom.memcmp(aMagic, aJournalMagic, 8) != 0 || SqlResult.SQLITE_OK != (rc = os.sqlite3OsRead(pJrnl, zMaster, len, (long)(szJ - 16 - len))))
                {
                    return rc;
                }
                ///
                ///<summary>
                ///See if the checksum matches the master journal name 
                ///</summary>

                for (u = 0; u < len; u++)
                {
                    cksum -= zMaster[u];
                }
                if (cksum != 0)
                {
                    ///
                    ///<summary>
                    ///If the checksum doesn't add up, then one or more of the disk sectors
                    ///containing the master journal filename is corrupted. This means
                    ///definitely roll back, so just return SqlResult.SQLITE_OK and report a (nul)
                    ///</summary>
                    ///<param name="master">journal filename.</param>
                    ///<param name=""></param>

                    len = 0;
                }
                if (len == 0)
                    zMaster[0] = 0;
                return SqlResult.SQLITE_OK;
            }

            ///
            ///<summary>
            ///Parameter aData must point to a buffer of pPager.pageSize bytes
            ///of data. Compute and return a checksum based ont the contents of the
            ///page of data and the current value of pPager.cksumInit.
            ///
            ///This is not a real checksum. It is really just the sum of the
            ///random initial value (pPager.cksumInit) and every 200th byte
            ///of the page data, starting with byte offset (pPager.pageSize%200).
            ///</summary>
            ///<param name="Each byte is interpreted as an 8">bit unsigned integer.</param>
            ///<param name=""></param>
            ///<param name="Changing the formula used to compute this checksum results in an">Changing the formula used to compute this checksum results in an</param>
            ///<param name="incompatible journal file format.">incompatible journal file format.</param>
            ///<param name=""></param>
            ///<param name="If journal corruption occurs due to a power failure, the most likely">If journal corruption occurs due to a power failure, the most likely</param>
            ///<param name="scenario is that one end or the other of the record will be changed.">scenario is that one end or the other of the record will be changed.</param>
            ///<param name="It is much less likely that the two ends of the journal record will be">It is much less likely that the two ends of the journal record will be</param>
            ///<param name="correct and the middle be corrupt.  Thus, this "checksum" scheme,">correct and the middle be corrupt.  Thus, this "checksum" scheme,</param>
            ///<param name="though fast and simple, catches the mostly likely kind of corruption.">though fast and simple, catches the mostly likely kind of corruption.</param>
            ///<param name=""></param>
#if SQLITE_DEBUG
#else
																																						// define pagerReportSize(X)     /* No-op if we do not support a codec */
static void pagerReportSize(Pager X){}
#endif
            ///<summary>
            /// Read the content for page pPg out of the database file and into
            /// pPg.pData. A shared lock or greater must be held on the database
            /// file before this function is called.
            ///
            /// If page 1 is read, then the value of Pager.dbFileVers[] is set to
            /// the value read from the database file.
            ///
            /// If an IO error occurs, then the IO error is returned to the caller.
            /// Otherwise, SqlResult.SQLITE_OK is returned.
            ///
            ///</summary>
            public static SqlResult  readDbPage(PgHdr pPg)
            {
                Pager pPager = pPg.pPager;
                ///
                ///<summary>
                ///Pager object associated with page pPg 
                ///</summary>

                Pgno pgno = pPg.pgno;
                ///
                ///<summary>
                ///Page number to read 
                ///</summary>

                var rc = SqlResult.SQLITE_OK;
                ///
                ///<summary>
                ///Return code 
                ///</summary>

                int isInWal = 0;
                ///
                ///<summary>
                ///True if page is in log file 
                ///</summary>

                int pgsz = pPager.pageSize;
                ///
                ///<summary>
                ///Number of bytes to read 
                ///</summary>

                Debug.Assert(pPager.eState >= PagerState.PAGER_READER &&
#if SQLITE_OMIT_MEMORYDB
																																																									0 == MEMDB 
#else
 0 == pPager.memDb
#endif
);
                Debug.Assert(pPager.fd.isOpen );
                if (NEVER(!pPager.fd.isOpen ))
                {
                    Debug.Assert(pPager.tempFile);
                    Array.Clear(pPg.pData, 0, pPager.pageSize);
                    // memset(pPg.pData, 0, pPager.pageSize);
                    return SqlResult.SQLITE_OK;
                }
                if (pPager.pagerUseWal())
                {
                    ///
                    ///<summary>
                    ///</summary>
                    ///<param name="Try to pull the page from the write">ahead log. </param>

                    rc = wal.sqlite3WalRead(pPager.pWal, pgno, ref isInWal, pgsz, pPg.pData);
                }
                if (rc == SqlResult.SQLITE_OK && 0 == isInWal)
                {
                    i64 iOffset = (pgno - 1) * (i64)pPager.pageSize;
                    rc = os.sqlite3OsRead(pPager.fd, pPg.pData, pgsz, iOffset);
                    if (rc == SqlResult.SQLITE_IOERR_SHORT_READ)
                    {
                        rc = SqlResult.SQLITE_OK;
                    }
                }
                if (pgno == 1)
                {
                    if (rc != 0)
                    {
                        ///
                        ///<summary>
                        ///If the read is unsuccessful, set the dbFileVers[] to something
                        ///that will never be a valid file version.  dbFileVers[] is a copy
                        ///of bytes 24..39 of the database.  Bytes 28..31 should always be
                        ///zero or the size of the database in page. Bytes 32..35 and 35..39
                        ///should be page numbers which are never 0xffffffff.  So filling
                        ///pPager.dbFileVers[] with all 0xff bytes should suffice.
                        ///
                        ///For an encrypted database, the situation is more complex:  bytes
                        ///24..39 of the database are white noise.  But the probability of
                        ///white noising equaling 16 bytes of 0xff is vanishingly small so
                        ///we should still be ok.
                        ///
                        ///</summary>

                        for (int i = 0; i < pPager.dbFileVers.Length; pPager.dbFileVers[i++] = 0xff)
                            ;
                        // memset(pPager.dbFileVers, 0xff, sizeof(pPager.dbFileVers));
                    }
                    else
                    {
                        //u8[] dbFileVers = pPg.pData[24];
                        Buffer.BlockCopy(pPg.pData, 24, pPager.dbFileVers, 0, pPager.dbFileVers.Length);
                        //memcpy(&pPager.dbFileVers, dbFileVers, sizeof(pPager.dbFileVers));
                    }
                }
                if (CODEC1(pPager, pPg.pData, pgno, crypto.SQLITE_DECRYPT))
                    rc = SqlResult.SQLITE_NOMEM;
                //CODEC1(pPager, pPg.pData, pgno, 3, rc = SQLITE_NOMEM);
#if SQLITE_TEST
																																																									      //  PAGER_INCR(ref sqlite3_pager_readdb_count);
#if !TCLSH
																																																									      PAGER_INCR( ref sqlite3_pager_readdb_count );
#else
																																																									      int iValue;
      iValue = sqlite3_pager_readdb_count.iValue;
      PAGER_INCR( ref iValue );
      sqlite3_pager_readdb_count.iValue = iValue;
#endif
																																																									
      PAGER_INCR( ref pPager.nRead );
#endif
                sqliteinth.IOTRACE("PGIN %p %d\n", pPager, pgno);
                PAGERTRACE("FETCH %d page %d hash(%08x)\n", PagerMethods.PAGERID(pPager), pgno, pPg.pager_pagehash());
                return rc;
            }

           

#if !SQLITE_OMIT_WAL
																																						///<summary>
/// This function is invoked once for each page that has already been
/// written into the log file when a WAL transaction is rolled back.
/// Parameter iPg is the page number of said page. The pCtx argument
/// is actually a pointer to the Pager structure.
///
/// If page iPg is present in the cache, and has no outstanding references,
/// it is discarded. Otherwise, if there are one or more outstanding
/// references, the page content is reloaded from the database. If the
/// attempt to reload content from the database is required and fails,
/// return an SQLite error code. Otherwise, SqlResult.SQLITE_OK.
///</summary>
static int pagerUndoCallback(void *pCtx, Pgno iPg){
var rc = SqlResult.SQLITE_OK;
Pager *pPager = (Pager *)pCtx;
PgHdr *pPg;

pPg = sqlite3PagerLookup(pPager, iPg);
if( pPg ){
if( PCacheMethods.sqlite3PcachePageRefcount(pPg)==1 ){
sqlite3PcacheDrop(pPg);
}else{
rc = readDbPage(pPg);
if( rc==SqlResult.SQLITE_OK ){
pPager.xReiniter(pPg);
}
PagerMethods.sqlite3PagerUnref(pPg);
}
}

/* Normally, if a transaction is rolled back, any backup processes are
** updated as data is copied out of the rollback journal and into the
** database. This is not generally possible with a WAL database, as
** rollback involves simply truncating the log file. Therefore, if one
** or more frames have already been written to the log (and therefore 
** also copied into the backup databases) as part of this transaction,
** the backups must be restarted.
*/
sqlite3BackupRestart(pPager.pBackup);

return rc;
}

///<summary>
/// This function is called to rollback a transaction on a WAL database.
///</summary>
static int pagerRollbackWal(Pager *pPager){
int rc;                         /* Return Code */
PgHdr *pList;                   /* List of dirty pages to revert */

/* For all pages in the cache that are currently dirty or have already
** been written (but not committed) to the log file, do one of the 
** following:
**
**   + Discard the cached page (if refcount==0), or
**   + Reload page content from the database (if refcount>0).
*/
pPager.dbSize = pPager.dbOrigSize;
rc = sqlite3WalUndo(pPager.pWal, pagerUndoCallback, (void *)pPager);
pList = PCacheMethods.sqlite3PcacheDirtyList(pPager.pPCache);
while( pList && rc==SqlResult.SQLITE_OK ){
PgHdr *pNext = pList->pDirty;
rc = pagerUndoCallback((void *)pPager, pList->pgno);
pList = pNext;
}

return rc;
}


///<summary>
/// This function is a wrapper around sqlite3WalFrames(). As well as logging
/// the contents of the list of pages headed by pList (connected by pDirty),
/// this function notifies any active backup processes that the pages have
/// changed.
///
/// The list of pages passed into this routine is always sorted by page number.
/// Hence, if page 1 appears anywhere on the list, it will be the first page.
///</summary> 
static int pagerWalFrames(
Pager *pPager,                  /* Pager object */
PgHdr *pList,                   /* List of frames to log */
Pgno nTruncate,                 /* Database size after this commit */
int isCommit,                   /* True if this is a commit */
int syncFlags                   /* Flags to pass to OsSync() (or 0) */
){
int rc;                         /* Return code */
#if (SQLITE_DEBUG) || (SQLITE_CHECK_PAGES)
																																						PgHdr *p;                       /* For looping over pages */
#endif
																																						
assert( pPager.pWal );
#if SQLITE_DEBUG
																																						/* Verify that the page list is in accending order */
for(p=pList; p && p->pDirty; p=p->pDirty){
assert( p->pgno < p->pDirty->pgno );
}
#endif
																																						
  if( isCommit ){
    /* If a WAL transaction is being committed, there is no point in writing
    ** any pages with page numbers greater than nTruncate into the WAL file.
    ** They will never be read by any client. So remove them from the pDirty
    ** list here. */
    PgHdr *p;
    PgHdr **ppNext = &pList;
    for(p=pList; (*ppNext = p); p=p->pDirty){
      if( p->pgno<=nTruncate ) ppNext = &p->pDirty;
    }
    assert( pList );
  }


if( pList->pgno==1 ) pager_write_changecounter(pList);
rc = sqlite3WalFrames(pPager.pWal, 
pPager.pageSize, pList, nTruncate, isCommit, syncFlags
);
if( rc==SqlResult.SQLITE_OK && pPager.pBackup ){
PgHdr *p;
for(p=pList; p; p=p->pDirty){
sqlite3BackupUpdate(pPager.pBackup, p->pgno, (u8 *)p->pData);
}
}

#if SQLITE_CHECK_PAGES
																																						pList = PCacheMethods.sqlite3PcacheDirtyList(pPager.pPCache);
for(p=pList; p; p=p->pDirty){
pager_set_pagehash(p);
}
#endif
																																						
return rc;
}

///<summary>
/// Begin a read transaction on the WAL.
///
/// This routine used to be called "pagerOpenSnapshot()" because it essentially
/// makes a snapshot of the database at the current point in time and preserves
/// that snapshot for use by the reader in spite of concurrently changes by
/// other writers or checkpointers.
///</summary>
static int pagerBeginReadTransaction(Pager *pPager){
int rc;                         /* Return code */
int changed = 0;                /* True if cache must be reset */

assert( pagerUseWal(pPager) );
assert( pPager.eState==PagerState.PAGER_OPEN || pPager.eState==PagerState.PAGER_READER );

/* sqlite3WalEndReadTransaction() was not called for the previous
** transaction in locking_mode=EXCLUSIVE.  So call it now.  If we
** are in locking_mode=NORMAL and EndRead() was previously called,
** the duplicate call is harmless.
*/
sqlite3WalEndReadTransaction(pPager.pWal);

rc = sqlite3WalBeginReadTransaction(pPager.pWal, &changed);
if( rc!=SqlResult.SQLITE_OK || changed ){
pager_reset(pPager);
}

return rc;
}
#endif
#if !SQLITE_OMIT_WAL
																																						///<summary>
/// Check if the *-wal file that corresponds to the database opened by pPager
/// exists if the database is not empy, or verify that the *-wal file does
/// not exist (by deleting it) if the database file is empty.
///
/// If the database is not empty and the *-wal file exists, open the pager
/// in WAL mode.  If the database is empty or if no *-wal file exists and
/// if no error occurs, make sure Pager.journalMode is not set to
/// PAGER_JOURNALMODE_WAL.
///
/// Return SqlResult.SQLITE_OK or an error code.
///
/// The caller must hold a SHARED lock on the database file to call this
/// function. Because an EXCLUSIVE lock on the db file is required to delete
/// a WAL on a none-empty database, this ensures there is no race condition
/// between the xAccess() below and an xDelete() being executed by some
/// other connection.
///</summary>
static int pagerOpenWalIfPresent(Pager *pPager){
var rc = SqlResult.SQLITE_OK;
Debug.Assert( pPager.eState==PagerState.PAGER_OPEN );
Debug.Assert( pPager.eLock>=SHARED_LOCK || pPager.noReadlock );

if( !pPager.tempFile ){
int isWal;                    /* True if WAL file exists */
Pgno nPage;                   /* Size of the database file */

rc = pagerPagecount(pPager, &nPage);
if( rc ) return rc;
if( nPage==0 ){
rc = sqlite3OsDelete(pPager.pVfs, pPager.zWal, 0);
isWal = 0;
}else{
rc = sqlite3OsAccess(
pPager.pVfs, pPager.zWal, SQLITE_ACCESS_EXISTS, &isWal
);
}
if( rc==SqlResult.SQLITE_OK ){
if( isWal ){
sqliteinth.testcase( PCacheMethods.sqlite3PcachePagecount(pPager.pPCache)==0 );
rc = sqlite3PagerOpenWal(pPager, 0);
}else if( pPager.journalMode==PAGER_JOURNALMODE_WAL ){
pPager.journalMode = PAGER_JOURNALMODE_DELETE;
}
}
}
return rc;
}
#endif
            ///
            ///<summary>
            ///</summary>
            ///<param name="Change the maximum number of in">memory pages that are allowed.</param>
            ///<param name=""></param>

            ///
            ///<summary>
            ///Attempt to set the maximum database page count if mxPage is positive.
            ///Make no changes if mxPage is zero or negative.  And never reduce the
            ///maximum page count below the current size of the database.
            ///
            ///Regardless of mxPage, return the current maximum page count.
            ///
            ///</summary>

            ///
            ///<summary>
            ///Try to obtain a lock of type locktype on the database file. If
            ///</summary>
            ///<param name="a similar or greater lock is already held, this function is a no">op</param>
            ///<param name="(returning SqlResult.SQLITE_OK immediately).">(returning SqlResult.SQLITE_OK immediately).</param>
            ///<param name=""></param>
            ///<param name="Otherwise, attempt to obtain the lock using sqlite3OsLock(). Invoke">Otherwise, attempt to obtain the lock using sqlite3OsLock(). Invoke</param>
            ///<param name="the busy callback if the lock is currently not available. Repeat">the busy callback if the lock is currently not available. Repeat</param>
            ///<param name="until the busy callback returns false or until the attempt to">until the busy callback returns false or until the attempt to</param>
            ///<param name="obtain the lock succeeds.">obtain the lock succeeds.</param>
            ///<param name=""></param>
            ///<param name="Return SqlResult.SQLITE_OK on success and an error code if we cannot obtain">Return SqlResult.SQLITE_OK on success and an error code if we cannot obtain</param>
            ///<param name="the lock. If the lock is obtained successfully, set the Pager.state">the lock. If the lock is obtained successfully, set the Pager.state</param>
            ///<param name="variable to locktype before returning.">variable to locktype before returning.</param>
            ///<param name=""></param>

            ///<summary>
            /// Function assertTruncateConstraint(pPager) checks that one of the
            /// following is true for all dirty pages currently in the page-cache:
            ///
            ///   a) The page number is less than or equal to the size of the
            ///      current database image, in pages, OR
            ///
            ///   b) if the page content were written at this time, it would not
            ///      be necessary to write the current content out to the sub-journal
            ///      (as determined by function subjRequiresPage()).
            ///
            /// If the condition asserted by this function were not true, and the
            /// dirty page were to be discarded from the cache via the pagerStress()
            /// routine, pagerStress() would not write the current page content to
            /// the database file. If a savepoint transaction were rolled back after
            /// this happened, the correct behaviour would be to restore the current
            /// content of the page. However, since this content is not present in either
            /// the database file or the portion of the rollback journal and
            /// sub-journal rolled back the content could not be restored and the
            /// database image would become corrupt. It is therefore fortunate that
            /// this circumstance cannot arise.
            ///
            ///</summary>
#if SQLITE_DEBUG
																																						    static void assertTruncateConstraintCb( PgHdr pPg )
    {
      Debug.Assert( ( pPg.flags & PGHDR_DIRTY ) != 0 );
      Debug.Assert( !subjRequiresPage( pPg ) || pPg.pgno <= pPg.pPager.dbSize );
    }
    static void assertTruncateConstraint( Pager pPager )
    {
      PCacheMethods.sqlite3PcacheIterateDirty( pPager.pPCache, assertTruncateConstraintCb );
    }
#else
            //# define assertTruncateConstraint(pPager)
            static void assertTruncateConstraintCb(PgHdr pPg)
            {
            }

#endif
#if !NDEBUG || SQLITE_TEST
																																						    ///<summary>
/// Return the page number for page pPg.
///</summary>
    static Pgno sqlite3PagerPagenumber( DbPage pPg )
    {
      return pPg.pgno;
    }
#else
            public static Pgno sqlite3PagerPagenumber(DbPage pPg)
            {
                return pPg.pgno;
            }

#endif
            ///<summary>
            /// Increment the reference count for page pPg.
            ///</summary>
            public static void sqlite3PagerRef(DbPage pPg)
            {
                pPg.sqlite3PcacheRef();
            }

            ///<summary>
            /// Append a record of the current state of page pPg to the sub-journal.
            /// It is the callers responsibility to use subjRequiresPage() to check
            /// that it is really required before calling this function.
            ///
            /// If successful, set the bit corresponding to pPg.pgno in the bitvecs
            /// for all open savepoints before returning.
            ///
            /// This function returns SqlResult.SQLITE_OK if everything is successful, an IO
            /// error code if the attempt to write to the sub-journal fails, or
            /// SQLITE_NOMEM if a malloc fails while setting a bit in a savepoint
            /// bitvec.
            ///
            ///</summary>
            public static SqlResult subjournalPage(PgHdr pPg)
            {
                SqlResult rc = SqlResult.SQLITE_OK;
                Pager pPager = pPg.pPager;
                if (pPager.journalMode != PAGER_JOURNALMODE_OFF)
                {
                    ///
                    ///<summary>
                    ///</summary>
                    ///<param name="Open the sub">journal, if it has not already been opened </param>

                    Debug.Assert(pPager.useJournal != 0);
                    Debug.Assert(pPager.jfd.isOpen   || pPager.pagerUseWal());
                    Debug.Assert(pPager.sjfd.isOpen || pPager.nSubRec == 0);
                    Debug.Assert(pPager.pagerUseWal() || pPg.pageInJournal() || pPg.pgno > pPager.dbOrigSize);
                    rc = pPager.openSubJournal();
                    ///
                    ///<summary>
                    ///</summary>
                    ///<param name="If the sub">journal was opened successfully (or was already open),</param>
                    ///<param name="write the journal record into the file.  ">write the journal record into the file.  </param>

                    if (rc == SqlResult.SQLITE_OK)
                    {
                        byte[] pData = pPg.pData;
                        i64 offset = pPager.nSubRec * (4 + pPager.pageSize);
                        byte[] pData2 = null;
                        if (CODEC2(pPager, pData, pPg.pgno, crypto.SQLITE_ENCRYPT_READ_CTX, ref pData2))
                            return SqlResult.SQLITE_NOMEM;
                        //CODEC2(pPager, pData, pPg.pgno, 7, return SQLITE_NOMEM, pData2);
                        PAGERTRACE("STMT-JOURNAL %d page %d\n", PagerMethods.PAGERID(pPager), pPg.pgno);
                        rc = write32bits(pPager.sjfd, offset, pPg.pgno);
                        if (rc == SqlResult.SQLITE_OK)
                        {
                            rc = os.sqlite3OsWrite(pPager.sjfd, pData2, pPager.pageSize, offset + 4);
                        }
                    }
                }
                if (rc == SqlResult.SQLITE_OK)
                {
                    pPager.nSubRec++;
                    Debug.Assert(pPager.nSavepoint > 0);
                    rc = pPager.addToSavepointBitvecs(pPg.pgno);
                }
                return rc;
            }

            ///<summary>
            /// This function is called by the pcache layer when it has reached some
            /// soft memory limit. The first argument is a pointer to a Pager object
            /// (cast as a void*). The pager is always 'purgeable' (not an in-memory
            /// database). The second argument is a reference to a page that is
            /// currently dirty but has no outstanding references. The page
            /// is always associated with the Pager object passed as the first
            /// argument.
            ///
            /// The job of this function is to make pPg clean by writing its contents
            /// out to the database file, if possible. This may involve syncing the
            /// journal file.
            ///
            /// If successful, PCacheMethods.sqlite3PcacheMakeClean() is called on the page and
            /// SqlResult.SQLITE_OK returned. If an IO error occurs while trying to make the
            /// page clean, the IO error code is returned. If the page cannot be
            /// made clean for some other reason, but no error occurs, then SqlResult.SQLITE_OK
            /// is returned by PCacheMethods.sqlite3PcacheMakeClean() is not called.
            ///
            ///</summary>
            static SqlResult pagerStress(object p, PgHdr pPg)
            {
                Pager pPager = (Pager)p;
                var rc = SqlResult.SQLITE_OK;
                Debug.Assert(pPg.pPager == pPager);
                Debug.Assert((pPg.flags & PGHDR_DIRTY) != 0);
                ///
                ///<summary>
                ///The doNotSyncSpill flag is set during times when doing a sync of
                ///journal (and adding a new header) is not allowed.  This occurs
                ///during calls to PagerMethods.sqlite3PagerWrite() while trying to journal multiple
                ///pages belonging to the same sector.
                ///
                ///The doNotSpill flag inhibits all cache spilling regardless of whether
                ///or not a sync is required.  This is set during a rollback.
                ///
                ///Spilling is also prohibited when in an error state since that could
                ///lead to database corruption.   In the current implementaton it 
                ///is impossible for PCacheMethods.sqlite3PcacheFetch() to be called with createFlag==1
                ///while in the error state, hence it is impossible for this routine to
                ///be called in the error state.  Nevertheless, we include a NEVER()
                ///test for the error state as a safeguard against future changes.
                ///
                ///</summary>

                if (NEVER(pPager.errCode != 0))
                    return SqlResult.SQLITE_OK;
                if (pPager.doNotSpill != 0)
                    return SqlResult.SQLITE_OK;
                if (pPager.doNotSyncSpill != 0 && (pPg.flags & PGHDR_NEED_SYNC) != 0)
                {
                    return SqlResult.SQLITE_OK;
                }
                pPg.pDirty = null;
                if (pPager.pagerUseWal())
                {
                    ///
                    ///<summary>
                    ///Write a single frame for this page to the log. 
                    ///</summary>

                    if (pPg.subjRequiresPage())
                    {
                        rc = subjournalPage(pPg);
                    }
                    if (rc == SqlResult.SQLITE_OK)
                    {
                        rc = pPager.pagerWalFrames(pPg, 0, 0, 0);
                    }
                }
                else
                {
                    ///
                    ///<summary>
                    ///Sync the journal file if required. 
                    ///</summary>

                    if ((pPg.flags & PGHDR_NEED_SYNC) != 0 || pPager.eState == PagerState.PAGER_WRITER_CACHEMOD)
                    {
                        rc = pPager.syncJournal(1);
                    }
                    ///
                    ///<summary>
                    ///If the page number of this page is larger than the current size of
                    ///</summary>
                    ///<param name="the database image, it may need to be written to the sub">journal.</param>
                    ///<param name="This is because the call to pager_write_pagelist() below will not">This is because the call to pager_write_pagelist() below will not</param>
                    ///<param name="actually write data to the file in this case.">actually write data to the file in this case.</param>
                    ///<param name=""></param>
                    ///<param name="Consider the following sequence of events:">Consider the following sequence of events:</param>
                    ///<param name=""></param>
                    ///<param name="BEGIN;">BEGIN;</param>
                    ///<param name="<journal page X>"><journal page X></param>
                    ///<param name="<modify page X>"><modify page X></param>
                    ///<param name="SAVEPOINT sp;">SAVEPOINT sp;</param>
                    ///<param name="<shrink database file to Y pages>"><shrink database file to Y pages></param>
                    ///<param name="pagerStress(page X)">pagerStress(page X)</param>
                    ///<param name="ROLLBACK TO sp;">ROLLBACK TO sp;</param>
                    ///<param name=""></param>
                    ///<param name="If (X>Y), then when pagerStress is called page X will not be written">If (X>Y), then when pagerStress is called page X will not be written</param>
                    ///<param name="out to the database file, but will be dropped from the cache. Then,">out to the database file, but will be dropped from the cache. Then,</param>
                    ///<param name="following the "ROLLBACK TO sp" statement, reading page X will read">following the "ROLLBACK TO sp" statement, reading page X will read</param>
                    ///<param name="data from the database file. This will be the copy of page X as it">data from the database file. This will be the copy of page X as it</param>
                    ///<param name="was when the transaction started, not as it was when "SAVEPOINT sp"">was when the transaction started, not as it was when "SAVEPOINT sp"</param>
                    ///<param name="was executed.">was executed.</param>
                    ///<param name=""></param>
                    ///<param name="The solution is to write the current data for page X into the">The solution is to write the current data for page X into the</param>
                    ///<param name="sub">journal file now (if it is not already there), so that it will</param>
                    ///<param name="be restored to its current value when the "ROLLBACK TO sp" is">be restored to its current value when the "ROLLBACK TO sp" is</param>
                    ///<param name="executed.">executed.</param>
                    ///<param name=""></param>

                    if (NEVER(rc == SqlResult.SQLITE_OK && pPg.pgno > pPager.dbSize && pPg.subjRequiresPage()))
                    {
                        rc = subjournalPage(pPg);
                    }
                    ///
                    ///<summary>
                    ///Write the contents of the page out to the database file. 
                    ///</summary>

                    if (rc == SqlResult.SQLITE_OK)
                    {
                        Debug.Assert((pPg.flags & PGHDR_NEED_SYNC) == 0);
                        rc = pPager.pager_write_pagelist(pPg);
                    }
                }
                ///
                ///<summary>
                ///Mark the page as clean. 
                ///</summary>

                if (rc == SqlResult.SQLITE_OK)
                {
                    PAGERTRACE("STRESS %d page %d\n", PagerMethods.PAGERID(pPager), pPg.pgno);
                    PCacheMethods.sqlite3PcacheMakeClean(pPg);
                }
                return pPager.pager_error(rc);
            }

            ///<summary>
            /// Allocate and initialize a new Pager object and put a pointer to it
            /// in *ppPager. The pager should eventually be freed by passing it
            /// to sqlite3PagerClose().
            ///
            /// The zFilename argument is the path to the database file to open.
            /// If zFilename is NULL then a randomly-named temporary file is created
            /// and used as the file to be cached. Temporary files are be deleted
            /// automatically when they are closed. If zFilename is ":memory:" then
            /// all information is held in cache. It is never written to disk.
            /// This can be used to implement an in-memory database.
            ///
            /// The nExtra parameter specifies the number of bytes of space allocated
            /// along with each page reference. This space is available to the user
            /// via the  PagerMethods.sqlite3PagerGetExtra () API.
            ///
            /// The flags argument is used to specify properties that affect the
            /// operation of the pager. It should be passed some bitwise combination
            /// of the PAGER_OMIT_JOURNAL and PAGER_NO_READLOCK flags.
            ///
            /// The vfsFlags parameter is a bitmask to pass to the flags parameter
            /// of the xOpen() method of the supplied VFS when opening files.
            ///
            /// If the pager object is allocated and the specified file opened
            /// successfully, SqlResult.SQLITE_OK is returned and *ppPager set to point to
            /// the new pager object. If an error occurs, *ppPager is set to NULL
            /// and error code returned. This function may return SQLITE_NOMEM
            /// (malloc_cs.sqlite3Malloc() is used to allocate memory), SQLITE_CANTOPEN or
            /// various SQLITE_IO_XXX errors.
            ///
            ///</summary>
            public static SqlResult sqlite3PagerOpen(sqlite3_vfs pVfs, ///
                ///<summary>
                ///The virtual file system to use 
                ///</summary>

            out Pager ppPager, ///
                ///<summary>
                ///OUT: Return the Pager structure here 
                ///</summary>

            string zFilename, ///
                ///<summary>
                ///Name of the database file to open 
                ///</summary>

            int nExtra, ///
                ///<summary>
                ///</summary>
                ///<param name="Extra bytes append to each in">memory page </param>

            int flags, ///
                ///<summary>
                ///flags controlling this file 
                ///</summary>

            int vfsFlags, ///
                ///<summary>
                ///flags passed through to sqlite3_vfs.xOpen() 
                ///</summary>

            dxReiniter xReinit///
                ///<summary>
                ///Function to reinitialize pages 
                ///</summary>

            )
            {
                u8 pPtr;
                Pager pPager = null;
                ///
                ///<summary>
                ///Pager object to allocate and return 
                ///</summary>

                var rc = SqlResult.SQLITE_OK;
                ///
                ///<summary>
                ///Return code 
                ///</summary>

                u8 tempFile = 0;
                ///
                ///<summary>
                ///</summary>
                ///<param name="True for temp files (incl. in">memory files) </param>

                // Needs to be u8 for later tests
                u8 memDb = 0;
                ///
                ///<summary>
                ///</summary>
                ///<param name="True if this is an in">memory file </param>

                bool readOnly = false;
                ///
                ///<summary>
                ///</summary>
                ///<param name="True if this is a read">only file </param>

                int journalFileSize;
                ///
                ///<summary>
                ///Bytes to allocate for each journal fd 
                ///</summary>

                StringBuilder zPathname = null;
                ///
                ///<summary>
                ///Full path to database file 
                ///</summary>

                int nPathname = 0;
                ///
                ///<summary>
                ///Number of bytes in zPathname 
                ///</summary>

                bool useJournal = (flags & PAGER_OMIT_JOURNAL) == 0;
                ///
                ///<summary>
                ///False to omit journal 
                ///</summary>

                bool noReadlock = (flags & PAGER_NO_READLOCK) != 0;
                ///
                ///<summary>
                ///</summary>
                ///<param name="True to omit read">lock </param>

                int pcacheSize = PCacheMethods.sqlite3PcacheSize();
                ///
                ///<summary>
                ///Bytes to allocate for PCache 
                ///</summary>

                u32 szPageDflt = SQLITE_DEFAULT_PAGE_SIZE;
                ///
                ///<summary>
                ///Default page size 
                ///</summary>

                string zUri = null;
                ///
                ///<summary>
                ///URI args to copy 
                ///</summary>

                int nUri = 0;
                ///
                ///<summary>
                ///Number of bytes of URI args at *zUri 
                ///</summary>

                ///
                ///<summary>
                ///</summary>
                ///<param name="Figure out how much space is required for each journal file">handle</param>
                ///<param name="(there are two of them, the main journal and the sub">journal). This</param>
                ///<param name="is the maximum space required for an in">memory journal file handle</param>
                ///<param name="and a regular journal file">handle"</param>
                ///<param name="may be a wrapper capable of caching the first portion of the journal">may be a wrapper capable of caching the first portion of the journal</param>
                ///<param name="file in memory to implement the atomic">write optimization (see</param>
                ///<param name="source file journal.c).">source file journal.c).</param>
                ///<param name=""></param>

                if (pVfs.sqlite3JournalSize() > memjrnl.sqlite3MemJournalSize())
                {
                    journalFileSize = MathExtensions.ROUND8(pVfs.sqlite3JournalSize());
                }
                else
                {
                    journalFileSize = MathExtensions.ROUND8(memjrnl.sqlite3MemJournalSize());
                }
                ///
                ///<summary>
                ///Set the output variable to NULL in case an error occurs. 
                ///</summary>

                ppPager = null;
#if !SQLITE_OMIT_MEMORYDB
                if ((flags & PAGER_MEMORY) != 0)
                {
                    memDb = 1;
                    zFilename = null;
                }
#endif
                ///
                ///<summary>
                ///Compute and store the full pathname in an allocated buffer pointed
                ///to by zPathname, length nPathname. Or, if this is a temporary file,
                ///leave both nPathname and zPathname set to 0.
                ///
                ///</summary>

                if (!String.IsNullOrEmpty(zFilename))
                {
                    string z;
                    nPathname = pVfs.mxPathname + 1;
                    zPathname = new StringBuilder(nPathname * 2);
                    // malloc_cs.sqlite3Malloc( nPathname * 2 );
                    //if ( zPathname == null )
                    //{
                    //  return SQLITE_NOMEM;
                    //}
                    //zPathname[0] = 0; /* Make sure initialized even if FullPathname() fails */
                    rc = os.sqlite3OsFullPathname(pVfs, zFilename, nPathname, zPathname);
                    nPathname = StringExtensions.sqlite3Strlen30(zPathname);
                    z = zUri = zFilename;
                    //.Substring(StringExtensions.sqlite3Strlen30( zFilename ) );
                    //while ( *z )
                    //{
                    //  z += StringExtensions.sqlite3Strlen30( z ) + 1;
                    //  z += StringExtensions.sqlite3Strlen30( z ) + 1;
                    //}
                    nUri = zUri.Length;
                    //        &z[1] - zUri;
                    if (rc == SqlResult.SQLITE_OK && nPathname + 8 > pVfs.mxPathname)
                    {
                        ///
                        ///<summary>
                        ///This branch is taken when the journal path required by
                        ///the database being opened will be more than pVfs.mxPathname
                        ///bytes in length. This means the database cannot be opened,
                        ///as it will not be possible to open the journal file or even
                        ///</summary>
                        ///<param name="check for a hot">journal before reading.</param>
                        ///<param name=""></param>

                        rc =  sqliteinth.SQLITE_CANTOPEN_BKPT();
                    }
                    if (rc != SqlResult.SQLITE_OK)
                    {
                        //malloc_cs.sqlite3_free( ref zPathname );
                        return rc;
                    }
                }
                ///
                ///<summary>
                ///Allocate memory for the Pager structure, PCache object, the
                ///three file descriptors, the database file name and the journal
                ///file name. The layout in memory is as follows:
                ///
                ///Pager object                    (sizeof(Pager) bytes)
                ///PCache object                   (sqlite3PcacheSize() bytes)
                ///Database file handle            (pVfs.szOsFile bytes)
                ///</summary>
                ///<param name="Sub">journal file handle         (journalFileSize bytes)</param>
                ///<param name="Main journal file handle        (journalFileSize bytes)">Main journal file handle        (journalFileSize bytes)</param>
                ///<param name="Database file name              (nPathname+1 bytes)">Database file name              (nPathname+1 bytes)</param>
                ///<param name="Journal file name               (nPathname+8+1 bytes)">Journal file name               (nPathname+8+1 bytes)</param>
                ///<param name=""></param>

                //pPtr = (u8 *)malloc_cs.sqlite3MallocZero(
                //  ROUND8(sizeof(*pPager)) +           /* Pager structure */
                //  ROUND8(pcacheSize)      +           /* PCache object */
                //  ROUND8(pVfs.szOsFile)   +           /* The main db file */
                //  journalFileSize * 2 +       /* The two journal files */
                //  nPathname + 1 + nUri +         /* zFilename */
                //  nPathname + 8 + 1           /* zJournal */
                //#if !SQLITE_OMIT_WAL
                //    + nPathname + 4 + 1              /* zWal */
                //#endif
                //);
                //  Debug.Assert( EIGHT_BYTE_ALIGNMENT(SQLITE_INT_TO_PTR(journalFileSize)));
                //if( !pPtr ){
                //  //malloc_cs.sqlite3_free(zPathname);
                //  return SQLITE_NOMEM;
                //}
                pPager = new Pager();
                //(Pager*)(pPtr);
                pPager.pPCache = new PCache();
                //(PCache*)(pPtr += ROUND8(sizeof(*pPager)));
                pPager.fd = new sqlite3_file();
                //(sqlite3_file*)(pPtr += ROUND8(pcacheSize));
                pPager.sjfd = new sqlite3_file();
                //(sqlite3_file*)(pPtr += ROUND8(pVfs.szOsFile));
                pPager.jfd = new sqlite3_file();
                //(sqlite3_file*)(pPtr += journalFileSize);
                //pPager.zFilename =    (char*)(pPtr += journalFileSize);
                //Debug.Assert( EIGHT_BYTE_ALIGNMENT(pPager.jfd) );
                ///
                ///<summary>
                ///Fill in the Pager.zFilename and Pager.zJournal buffers, if required. 
                ///</summary>

                if (zPathname != null)
                {
                    Debug.Assert(nPathname > 0);
                    //pPager.zJournal =   (char*)(pPtr += nPathname + 1 + nUri);
                    //memcpy(pPager.zFilename, zPathname, nPathname);
                    pPager.zFilename = zPathname.ToString();
                    zUri = pPager.zFilename;
                    //.Substring( nPathname + 1 );//memcpy( &pPager.zFilename[nPathname + 1], zUri, nUri );
                    //memcpy(pPager.zJournal, zPathname, nPathname);
                    //memcpy(&pPager.zJournal[nPathname], "-journal", 8);
                    pPager.zJournal = pPager.zFilename + "-journal";
                    sqliteinth.sqlite3FileSuffix3(pPager.zFilename, pPager.zJournal);
#if !SQLITE_OMIT_WAL
																																																																												pPager.zWal = &pPager.zJournal[nPathname+8+1];
memcpy(pPager.zWal, zPathname, nPathname);
memcpy(&pPager.zWal[nPathname], "-wal", 4);
        sqlite3FileSuffix3(pPager.zFilename, pPager.zWal);
#endif
                    //malloc_cs.sqlite3_free( ref zPathname );
                }
                else
                {
                    pPager.zFilename = "";
                }
                pPager.pVfs = pVfs;
                pPager.vfsFlags = (u32)vfsFlags;
                ///
                ///<summary>
                ///Open the pager file.
                ///
                ///</summary>

                if (!String.IsNullOrEmpty(zFilename))
                {
                    int fout = 0;
                    ///
                    ///<summary>
                    ///VFS flags returned by xOpen() 
                    ///</summary>

                    rc = os.sqlite3OsOpen(pVfs, pPager.zFilename, pPager.fd, vfsFlags, ref fout);
                    Debug.Assert(0 == memDb);
                    readOnly = (fout & SQLITE_OPEN_READONLY) != 0;
                    ///
                    ///<summary>
                    ///If the file was successfully opened for read/write access,
                    ///choose a default page size in case we have to create the
                    ///database file. The default page size is the maximum of:
                    ///
                    ///+ SQLITE_DEFAULT_PAGE_SIZE,
                    ///+ The value returned by sqlite3OsSectorSize()
                    ///+ The largest page size that can be written atomically.
                    ///
                    ///</summary>

                    if (rc == SqlResult.SQLITE_OK && !readOnly)
                    {
                        pPager.setSectorSize();
                        Debug.Assert(SQLITE_DEFAULT_PAGE_SIZE <= SQLITE_MAX_DEFAULT_PAGE_SIZE);
                        if (szPageDflt < pPager.sectorSize)
                        {
                            if (pPager.sectorSize > SQLITE_MAX_DEFAULT_PAGE_SIZE)
                            {
                                szPageDflt = SQLITE_MAX_DEFAULT_PAGE_SIZE;
                            }
                            else
                            {
                                szPageDflt = (u32)pPager.sectorSize;
                            }
                        }
#if SQLITE_ENABLE_ATOMIC_WRITE
																																																																																															{
int iDc = sqlite3OsDeviceCharacteristics(pPager.fd);
int ii;
Debug.Assert(SQLITE_IOCAP_ATOMIC512==(512>>8));
Debug.Assert(SQLITE_IOCAP_ATOMIC64K==(65536>>8));
Debug.Assert(SQLITE_MAX_DEFAULT_PAGE_SIZE<=65536);
for(ii=szPageDflt; ii<=SQLITE_MAX_DEFAULT_PAGE_SIZE; ii=ii*2){
if( iDc&(SQLITE_IOCAP_ATOMIC|(ii>>8)) ){
szPageDflt = ii;
}
}
}
#endif
                    }
                }
                else
                {
                    ///
                    ///<summary>
                    ///If a temporary file is requested, it is not opened immediately.
                    ///In this case we accept the default page size and delay actually
                    ///opening the file until the first call to OsWrite().
                    ///
                    ///</summary>
                    ///<param name="This branch is also run for an in">memory</param>
                    ///<param name="database is the same as a temp">file that is never written out to</param>
                    ///<param name="disk and uses an in">memory rollback journal.</param>
                    ///<param name=""></param>

                    tempFile = 1;
                    pPager.eState = PagerState.PAGER_READER;
                    pPager.eLock = EXCLUSIVE_LOCK;
                    readOnly = (vfsFlags & SQLITE_OPEN_READONLY) != 0;
                }
                ///
                ///<summary>
                ///The following call to PagerSetPagesize() serves to set the value of
                ///Pager.pageSize and to allocate the Pager.pTmpSpace buffer.
                ///
                ///</summary>

                if (rc == SqlResult.SQLITE_OK)
                {
                    Debug.Assert(pPager.memDb == 0);
                    rc = pPager.sqlite3PagerSetPagesize(ref szPageDflt, -1);
                    sqliteinth.testcase(rc != SqlResult.SQLITE_OK);
                }
                ///
                ///<summary>
                ///If an error occurred in either of the blocks above, free the
                ///Pager structure and close the file.
                ///
                ///</summary>

                if (rc != SqlResult.SQLITE_OK)
                {
                    Debug.Assert(null == pPager.pTmpSpace);
                    os.sqlite3OsClose(pPager.fd);
                    //malloc_cs.sqlite3_free( ref pPager );
                    return rc;
                }
                ///
                ///<summary>
                ///Initialize the PCache object. 
                ///</summary>

                Debug.Assert(nExtra < 1000);
                nExtra = nExtra.ROUND8();
                PCacheMethods.sqlite3PcacheOpen((int)szPageDflt, nExtra, 0 == memDb, 0 == memDb ? (dxStress)pagerStress : null, pPager, pPager.pPCache);
                PAGERTRACE("OPEN %d %s\n", FILEHANDLEID(pPager.fd), pPager.zFilename);
                sqliteinth.IOTRACE("OPEN %p %s\n", pPager, pPager.zFilename);
                pPager.useJournal = (u8)(useJournal ? 1 : 0);
                pPager.noReadlock = (u8)(noReadlock && readOnly ? 1 : 0);
                ///
                ///<summary>
                ///pPager.stmtOpen = 0; 
                ///</summary>

                ///
                ///<summary>
                ///pPager.stmtInUse = 0; 
                ///</summary>

                ///
                ///<summary>
                ///pPager.nRef = 0; 
                ///</summary>

                ///
                ///<summary>
                ///pPager.stmtSize = 0; 
                ///</summary>

                ///
                ///<summary>
                ///pPager.stmtJSize = 0; 
                ///</summary>

                ///
                ///<summary>
                ///pPager.nPage = 0; 
                ///</summary>

                pPager.mxPgno = SQLITE_MAX_PAGE_COUNT;
                ///
                ///<summary>
                ///pPager.state = PAGER_UNLOCK; 
                ///</summary>

#if FALSE
																																																									Debug.Assert(pPager.state == (tempFile != 0 ? PAGER_EXCLUSIVE : PAGER_UNLOCK));
#endif
                ///
                ///<summary>
                ///pPager.errMask = 0; 
                ///</summary>

                pPager.tempFile = tempFile != 0;
                Debug.Assert(tempFile == PAGER_LOCKINGMODE_NORMAL || tempFile == PAGER_LOCKINGMODE_EXCLUSIVE);
                Debug.Assert(PAGER_LOCKINGMODE_EXCLUSIVE == 1);
                pPager.exclusiveMode = tempFile != 0;
                pPager.changeCountDone = pPager.tempFile;
                pPager.memDb = memDb;
                pPager.readOnly = readOnly;
                Debug.Assert(useJournal || pPager.tempFile);
                pPager.noSync = pPager.tempFile;
                pPager.fullSync = pPager.noSync;
                pPager.syncFlags = (byte)(pPager.noSync ? 0 : SQLITE_SYNC_NORMAL);
                pPager.ckptSyncFlags = pPager.syncFlags;
                ///
                ///<summary>
                ///pPager.pFirst = 0; 
                ///</summary>

                ///
                ///<summary>
                ///pPager.pFirstSynced = 0; 
                ///</summary>

                ///
                ///<summary>
                ///pPager.pLast = 0; 
                ///</summary>

                pPager.nExtra = (u16)nExtra;
                pPager.journalSizeLimit = SQLITE_DEFAULT_JOURNAL_SIZE_LIMIT;
                Debug.Assert(pPager.fd.isOpen  || tempFile != 0);
                pPager.setSectorSize();
                if (!useJournal)
                {
                    pPager.journalMode = PAGER_JOURNALMODE_OFF;
                }
                else
                    if (memDb != 0)
                    {
                        pPager.journalMode = PAGER_JOURNALMODE_MEMORY;
                    }
                ///
                ///<summary>
                ///pPager.xBusyHandler = 0; 
                ///</summary>

                ///
                ///<summary>
                ///pPager.pBusyHandlerArg = 0; 
                ///</summary>

                pPager.xReiniter = xReinit;
                ///
                ///<summary>
                ///memset(pPager.aHash, 0, sizeof(pPager.aHash)); 
                ///</summary>

                ppPager = pPager;
                return SqlResult.SQLITE_OK;
            }

            ///
            ///<summary>
            ///If the reference count has reached zero, rollback any active
            ///transaction and unlock the pager.
            ///
            ///Except, in locking_mode=EXCLUSIVE when there is nothing to in
            ///the rollback journal, the unlock is not performed and there is
            ///</summary>
            ///<param name="nothing to rollback, so this routine is a no">op.</param>
            ///<param name=""></param>

            ///<summary>
            /// Release a page reference.
            ///
            /// If the number of references to the page drop to zero, then the
            /// page is added to the LRU list.  When all references to all pages
            /// are released, a rollback occurs and the lock on the database is
            /// removed.
            ///
            ///</summary>
            public static void sqlite3PagerUnref(DbPage pPg)
            {
                if (pPg != null)
                {
                    Pager pPager = pPg.pPager;
                    PCacheMethods.sqlite3PcacheRelease(pPg);
                    pPager.pagerUnlockIfUnused();
                }
            }

            ///<summary>
            /// Mark a single data page as writeable. The page is written into the
            /// main journal or sub-journal as required. If the page is written into
            /// one of the journals, the corresponding bit is set in the
            /// Pager.pInJournal bitvec and the PagerSavepoint.pInSavepoint bitvecs
            /// of any open savepoints as appropriate.
            ///
            ///</summary>
            static SqlResult pager_write(PgHdr pPg)
            {
                byte[] pData = pPg.pData;
                Pager pPager = pPg.pPager;
                var rc = SqlResult.SQLITE_OK;
                ///
                ///<summary>
                ///</summary>
                ///<param name="This routine is not called unless a write">transaction has already </param>
                ///<param name="been started. The journal file may or may not be open at this point.">been started. The journal file may or may not be open at this point.</param>
                ///<param name="It is never called in the ERROR state.">It is never called in the ERROR state.</param>
                ///<param name=""></param>

                Debug.Assert(pPager.eState == PagerState.PAGER_WRITER_LOCKED || pPager.eState == PagerState.PAGER_WRITER_CACHEMOD || pPager.eState == PagerState.PAGER_WRITER_DBMOD);
                Debug.Assert(pPager.assert_pager_state());
                ///
                ///<summary>
                ///If an error has been previously detected, report the same error
                ///again. This should not happen, but the check provides robustness. 
                ///</summary>

                if (NEVER((int)pPager.errCode) != 0)
                    return pPager.errCode;
                ///
                ///<summary>
                ///</summary>
                ///<param name="Higher">level routines never call this function if database is not</param>
                ///<param name="writable.  But check anyway, just for robustness. ">writable.  But check anyway, just for robustness. </param>

                if (NEVER(pPager.readOnly))
                    return SqlResult.SQLITE_PERM;
#if SQLITE_CHECK_PAGES
																																																									CHECK_PAGE(pPg);
#endif
                ///
                ///<summary>
                ///The journal file needs to be opened. Higher level routines have already
                ///</summary>
                ///<param name="obtained the necessary locks to begin the write">transaction, but the</param>
                ///<param name="rollback journal might not yet be open. Open it now if this is the case.">rollback journal might not yet be open. Open it now if this is the case.</param>
                ///<param name=""></param>
                ///<param name="This is done before calling PCacheMethods.sqlite3PcacheMakeDirty() on the page. ">This is done before calling PCacheMethods.sqlite3PcacheMakeDirty() on the page. </param>
                ///<param name="Otherwise, if it were done after calling PCacheMethods.sqlite3PcacheMakeDirty(), then">Otherwise, if it were done after calling PCacheMethods.sqlite3PcacheMakeDirty(), then</param>
                ///<param name="an error might occur and the pager would end up in WRITER_LOCKED state">an error might occur and the pager would end up in WRITER_LOCKED state</param>
                ///<param name="with pages marked as dirty in the cache.">with pages marked as dirty in the cache.</param>

                if (pPager.eState == PagerState.PAGER_WRITER_LOCKED)
                {
                    rc = pPager.pager_open_journal();
                    if (rc != SqlResult.SQLITE_OK)
                        return rc;
                }
                Debug.Assert(pPager.eState >= PagerState.PAGER_WRITER_CACHEMOD);
                Debug.Assert(pPager.assert_pager_state());
                ///
                ///<summary>
                ///Mark the page as dirty.  If the page has already been written
                ///to the journal then we can return right away.
                ///
                ///</summary>

                PCacheMethods.sqlite3PcacheMakeDirty(pPg);
                if (pPg.pageInJournal() && !pPg.subjRequiresPage())
                {
                    Debug.Assert(!pPager.pagerUseWal());
                }
                else
                {
                    ///
                    ///<summary>
                    ///The transaction journal now exists and we have a RESERVED or an
                    ///EXCLUSIVE lock on the main database file.  Write the current page to
                    ///the transaction journal if it is not there already.
                    ///
                    ///</summary>

                    if (!pPg.pageInJournal() && !pPager.pagerUseWal())
                    {
                        Debug.Assert(pPager.pagerUseWal() == false);
                        if (pPg.pgno <= pPager.dbOrigSize && pPager.jfd.isOpen  )
                        {
                            u32 cksum;
                            byte[] pData2 = null;
                            i64 iOff = pPager.journalOff;
                            ///
                            ///<summary>
                            ///We should never write to the journal file the page that
                            ///contains the database locks.  The following Debug.Assert verifies
                            ///that we do not. 
                            ///</summary>

                            Debug.Assert(pPg.pgno != ((PENDING_BYTE / (pPager.pageSize)) + 1));
                            //PAGER_MJ_PGNO(pPager) );
                            Debug.Assert(pPager.journalHdr <= pPager.journalOff);
                            if (CODEC2(pPager, pData, pPg.pgno, crypto.SQLITE_ENCRYPT_READ_CTX, ref pData2))
                                return SqlResult.SQLITE_NOMEM;
                            // CODEC2(pPager, pData, pPg.pgno, 7, return SQLITE_NOMEM, pData2);
                            cksum = pPager.pager_cksum(pData2);
                            ///
                            ///<summary>
                            ///Even if an IO or diskfull error occurred while journalling the
                            ///</summary>
                            ///<param name="page in the block above, set the need">sync flag for the page.</param>
                            ///<param name="Otherwise, when the transaction is rolled back, the logic in">Otherwise, when the transaction is rolled back, the logic in</param>
                            ///<param name="playback_one_page() will think that the page needs to be restored">playback_one_page() will think that the page needs to be restored</param>
                            ///<param name="in the database file. And if an IO error occurs while doing so,">in the database file. And if an IO error occurs while doing so,</param>
                            ///<param name="then corruption may follow.">then corruption may follow.</param>
                            ///<param name=""></param>

                            pPg.flags |= PGHDR_NEED_SYNC;
                            rc = write32bits(pPager.jfd, iOff, pPg.pgno);
                            if (rc != SqlResult.SQLITE_OK)
                                return rc;
                            rc = os.sqlite3OsWrite(pPager.jfd, pData2, pPager.pageSize, iOff + 4);
                            if (rc != SqlResult.SQLITE_OK)
                                return rc;
                            rc = write32bits(pPager.jfd, iOff + pPager.pageSize + 4, cksum);
                            if (rc != SqlResult.SQLITE_OK)
                                return rc;
                            sqliteinth.IOTRACE("JOUT %p %d %lld %d\n", pPager, pPg.pgno, pPager.journalOff, pPager.pageSize);
#if SQLITE_TEST
#if !TCLSH
																																																																																																																		            PAGER_INCR( ref sqlite3_pager_writej_count );
#else
																																																																																																																		            int iValue = sqlite3_pager_writej_count.iValue;
            PAGER_INCR( ref iValue );
            sqlite3_pager_writej_count.iValue = iValue;
#endif
#endif
                            PAGERTRACE("JOURNAL %d page %d needSync=%d hash(%08x)\n", PagerMethods.PAGERID(pPager), pPg.pgno, ((pPg.flags & PGHDR_NEED_SYNC) != 0 ? 1 : 0), pPg.pager_pagehash());
                            pPager.journalOff += 8 + pPager.pageSize;
                            pPager.nRec++;
                            Debug.Assert(pPager.pInJournal != null);
                            rc = sqlite3BitvecSet(pPager.pInJournal, pPg.pgno);
                            sqliteinth.testcase(rc == SqlResult.SQLITE_NOMEM);
                            Debug.Assert(rc == SqlResult.SQLITE_OK || rc == SqlResult.SQLITE_NOMEM);
                            rc |= pPager.addToSavepointBitvecs(pPg.pgno);
                            if (rc != SqlResult.SQLITE_OK)
                            {
                                Debug.Assert(rc == SqlResult.SQLITE_NOMEM);
                                return rc;
                            }
                        }
                        else
                        {
                            if (pPager.eState != PagerState.PAGER_WRITER_DBMOD)
                            {
                                pPg.flags |= PGHDR_NEED_SYNC;
                            }
                            PAGERTRACE("APPEND %d page %d needSync=%d\n", PagerMethods.PAGERID(pPager), pPg.pgno, ((pPg.flags & PGHDR_NEED_SYNC) != 0 ? 1 : 0));
                        }
                    }
                    ///
                    ///<summary>
                    ///If the statement journal is open and the page is not in it,
                    ///then write the current page to the statement journal.  Note that
                    ///the statement journal format differs from the standard journal format
                    ///in that it omits the checksums and the header.
                    ///
                    ///</summary>

                    if (pPg.subjRequiresPage())
                    {
                        rc = subjournalPage(pPg);
                    }
                }
                ///
                ///<summary>
                ///Update the database size and return.
                ///
                ///</summary>

                if (pPager.dbSize < pPg.pgno)
                {
                    pPager.dbSize = pPg.pgno;
                }
                return rc;
            }

            ///
            ///<summary>
            ///Mark a data page as writeable. This routine must be called before
            ///making changes to a page. The caller must check the return value
            ///of this function and be careful not to change any page data unless
            ///this routine returns SqlResult.SQLITE_OK.
            ///
            ///The difference between this function and pager_write() is that this
            ///function also deals with the special case where 2 or more pages
            ///</summary>
            ///<param name="fit on a single disk sector. In this case all co">resident pages</param>
            ///<param name="must have been written to the journal file before returning.">must have been written to the journal file before returning.</param>
            ///<param name=""></param>
            ///<param name="If an error occurs, SQLITE_NOMEM or an IO error code is returned">If an error occurs, SQLITE_NOMEM or an IO error code is returned</param>
            ///<param name="as appropriate. Otherwise, SqlResult.SQLITE_OK.">as appropriate. Otherwise, SqlResult.SQLITE_OK.</param>
            ///<param name=""></param>

            public static SqlResult sqlite3PagerWrite(DbPage pDbPage)
            {
                SqlResult rc = SqlResult.SQLITE_OK;
                PgHdr pPg = pDbPage;
                Pager pPager = pPg.pPager;
                u32 nPagePerSector = (u32)(pPager.sectorSize / pPager.pageSize);
                Debug.Assert(pPager.eState >= PagerState.PAGER_WRITER_LOCKED);
                Debug.Assert(pPager.eState != PagerState.PAGER_ERROR);
                Debug.Assert(pPager.assert_pager_state());
                if (nPagePerSector > 1)
                {
                    Pgno nPageCount = 0;
                    ///Total number of pages in database file 

                    Pgno pg1;
                    ///First page of the sector pPg is located on. 

                    Pgno nPage = 0;
                    ///Number of pages starting at pg1 to journal 

                    int ii;
                    ///Loop counter 

                    bool needSync = false;
                    ///True if any page has PGHDR_NEED_SYNC 

                    ///Set the doNotSyncSpill flag to 1. This is because we cannot allow
                    ///a journal header to be written between the pages journaled by
                    ///this function.

                    Debug.Assert(
#if SQLITE_OMIT_MEMORYDB
																																																																												0==MEMDB
#else
0 == pPager.memDb
#endif
);
                    Debug.Assert(pPager.doNotSyncSpill == 0);
                    pPager.doNotSyncSpill++;
                    ///
                    ///<summary>
                    ///</summary>
                    ///<param name="This trick assumes that both the page">size are</param>
                    ///<param name="an integer power of 2. It sets variable pg1 to the identifier">an integer power of 2. It sets variable pg1 to the identifier</param>
                    ///<param name="of the first page of the sector pPg is located on.">of the first page of the sector pPg is located on.</param>
                    ///<param name=""></param>

                    pg1 = (u32)((pPg.pgno - 1) & ~(nPagePerSector - 1)) + 1;
                    nPageCount = pPager.dbSize;
                    if (pPg.pgno > nPageCount)
                    {
                        nPage = (pPg.pgno - pg1) + 1;
                    }
                    else
                        if ((pg1 + nPagePerSector - 1) > nPageCount)
                        {
                            nPage = nPageCount + 1 - pg1;
                        }
                        else
                        {
                            nPage = nPagePerSector;
                        }
                    Debug.Assert(nPage > 0);
                    Debug.Assert(pg1 <= pPg.pgno);
                    Debug.Assert((pg1 + nPage) > pPg.pgno);
                    for (ii = 0; ii < nPage && rc == SqlResult.SQLITE_OK; ii++)
                    {
                        u32 pg = (u32)(pg1 + ii);
                        PgHdr pPage = new PgHdr();
                        if (pg == pPg.pgno || sqlite3BitvecTest(pPager.pInJournal, pg) == 0)
                        {
                            if (pg != ((PENDING_BYTE / (pPager.pageSize)) + 1))//PAGER_MJ_PGNO(pPager))
                            {
                                rc = pPager.sqlite3PagerGet(pg, ref pPage);
                                if (rc == SqlResult.SQLITE_OK)
                                {
                                    rc = pager_write(pPage);
                                    if ((pPage.flags & PGHDR_NEED_SYNC) != 0)
                                    {
                                        needSync = true;
                                    }
                                    PagerMethods.sqlite3PagerUnref(pPage);
                                }
                            }
                        }
                        else
                            if ((pPage = pPager.pager_lookup(pg)) != null)
                            {
                                if ((pPage.flags & PGHDR_NEED_SYNC) != 0)
                                {
                                    needSync = true;
                                }
                                PagerMethods.sqlite3PagerUnref(pPage);
                            }
                    }
                    ///
                    ///<summary>
                    ///If the PGHDR_NEED_SYNC flag is set for any of the nPage pages
                    ///starting at pg1, then it needs to be set for all of them. Because
                    ///writing to any of these nPage pages may damage the others, the
                    ///journal file must contain sync()ed copies of all of them
                    ///before any of them can be written out to the database file.
                    ///
                    ///</summary>

                    if (rc == SqlResult.SQLITE_OK && needSync)
                    {
                        Debug.Assert(
#if SQLITE_OMIT_MEMORYDB
																																																																																															0==MEMDB
#else
0 == pPager.memDb
#endif
);
                        for (ii = 0; ii < nPage; ii++)
                        {
                            PgHdr pPage = pPager.pager_lookup((u32)(pg1 + ii));
                            if (pPage != null)
                            {
                                pPage.flags |= PGHDR_NEED_SYNC;
                                PagerMethods.sqlite3PagerUnref(pPage);
                            }
                        }
                    }
                    Debug.Assert(pPager.doNotSyncSpill == 1);
                    pPager.doNotSyncSpill--;
                }
                else
                {
                    rc = pager_write(pDbPage);
                }
                return rc;
            }

     
            ///<summary>
            /// A call to this routine tells the pager that it is not necessary to
            /// write the information on page pPg back to the disk, even though
            /// that page might be marked as dirty.  This happens, for example, when
            /// the page has been added as a leaf of the freelist and so its
            /// content no longer matters.
            ///
            /// The overlying software layer calls this routine when all of the data
            /// on the given page is unused. The pager marks the page as clean so
            /// that it does not get written to disk.
            ///
            /// Tests show that this optimization can quadruple the speed of large
            /// DELETE operations.
            ///</summary>
            public static void sqlite3PagerDontWrite(PgHdr pPg)
            {
                Pager pPager = pPg.pPager;
                if ((pPg.flags & PGHDR_DIRTY) != 0 && pPager.nSavepoint == 0)
                {
                    PAGERTRACE("DONT_WRITE page %d of %d\n", pPg.pgno, PagerMethods.PAGERID(pPager));
                    sqliteinth.IOTRACE("CLEAN %p %d\n", pPager, pPg.pgno);
                    pPg.flags |= PGHDR_DONT_WRITE;
                    pPg.pager_set_pagehash();
                }
            }

            

#if SQLITE_TEST
																																						    /*
** This routine is used for testing and analysis only.
*/
    static int[] sqlite3PagerStats( Pager pPager )
    {
      int[] a = new int[11];
      a[0] = sqlite3PcacheRefCount( pPager.pPCache );
      a[1] = sqlite3PcachePagecount( pPager.pPCache );
      a[2] = PCacheMethods.sqlite3PcacheGetCachesize( pPager.pPCache );
      a[3] = pPager.eState == PagerState.PAGER_OPEN ? -1 : (int)pPager.dbSize;
      a[4] = pPager.eState;
      a[5] = pPager.errCode;
      a[6] = pPager.nHit;
      a[7] = pPager.nMiss;
      a[8] = 0;  /* Used to be pPager.nOvfl */
      a[9] = pPager.nRead;
      a[10] = pPager.nWrite;
      return a;
    }
#endif
#if SQLITE_HAS_CODEC
            ///
            ///<summary>
            ///Set or retrieve the codec for this pager
            ///</summary>

#endif
#if !SQLITE_OMIT_AUTOVACUUM
#endif
            

            ///<summary>
            /// Return a pointer to the Pager.nExtra bytes of "extra" space
            /// allocated along with the specified page.
            ///
            ///</summary>
            public static MemPage  sqlite3PagerGetExtra (DbPage pPg)
            {
                return pPg.pExtra;
            }
#if !SQLITE_OMIT_WAL
																			///<summary>
/// This function is called when the user invokes "PRAGMA wal_checkpoint",
/// "PRAGMA wal_blocking_checkpoint" or calls the sqlite3_wal_checkpoint()
/// or wal_blocking_checkpoint() API functions.
///
/// Parameter eMode is one of SQLITE_CHECKPOINT_PASSIVE, FULL or RESTART.
///</summary>
int sqlite3PagerCheckpoint(Pager *pPager, int eMode, int *pnLog, int *pnCkpt){
  var rc = SqlResult.SQLITE_OK;
  if( pPager.pWal ){
    rc = sqlite3WalCheckpoint(pPager.pWal, eMode,
        pPager.xBusyHandler, pPager.pBusyHandlerArg,
        pPager.ckptSyncFlags, pPager.pageSize, (u8 *)pPager.pTmpSpace,
        pnLog, pnCkpt
    );
  }
  return rc;
}

    int sqlite3PagerWalCallback(Pager *pPager){
return sqlite3WalCallback(pPager.pWal);
}

///<summary>
/// Return true if the underlying VFS for the given pager supports the
/// primitives necessary for write-ahead logging.
///</summary>
int sqlite3PagerWalSupported(Pager *pPager){
const sqlite3_io_methods *pMethods = pPager.fd->pMethods;
return pPager.exclusiveMode || (pMethods->iVersion>=2 && pMethods->xShmMap);
}

///<summary>
/// Attempt to take an exclusive lock on the database file. If a PENDING lock
/// is obtained instead, immediately release it.
///</summary>
static int pagerExclusiveLock(Pager *pPager){
int rc;                         /* Return code */

assert( pPager.eLock==SHARED_LOCK || pPager.eLock==EXCLUSIVE_LOCK );
rc = pagerLockDb(pPager, EXCLUSIVE_LOCK);
if( rc!=SqlResult.SQLITE_OK ){
/* If the attempt to grab the exclusive lock failed, release the
** pending lock that may have been obtained instead.  */
pagerUnlockDb(pPager, SHARED_LOCK);
}

return rc;
}

/*
** Call sqlite3WalOpen() to open the WAL handle. If the pager is in 
** exclusive-locking mode when this function is called, take an EXCLUSIVE
** lock on the database file and use heap-memory to store the wal-index
** in. Otherwise, use the normal shared-memory.
*/
static int pagerOpenWal(Pager *pPager){
var rc = SqlResult.SQLITE_OK;

assert( pPager.pWal==0 && pPager.tempFile==0 );
assert( pPager.eLock==SHARED_LOCK || pPager.eLock==EXCLUSIVE_LOCK || pPager.noReadlock);

/* If the pager is already in exclusive-mode, the WAL module will use 
** heap-memory for the wal-index instead of the VFS shared-memory 
** implementation. Take the exclusive lock now, before opening the WAL
** file, to make sure this is safe.
*/
if( pPager.exclusiveMode ){
rc = pagerExclusiveLock(pPager);
}

/* Open the connection to the log file. If this operation fails, 
** (e.g. due to malloc() failure), return an error code.
*/
if( rc==SqlResult.SQLITE_OK ){
rc = sqlite3WalOpen(pPager.pVfs, 
pPager.fd, pPager.zWal, pPager.exclusiveMode, &pPager.pWal
        pPager.journalSizeLimit, &pPager.pWal
);
}

return rc;
}


/*
** The caller must be holding a SHARED lock on the database file to call
** this function.
**
** If the pager passed as the first argument is open on a real database
** file (not a temp file or an in-memory database), and the WAL file
** is not already open, make an attempt to open it now. If successful,
** return SqlResult.SQLITE_OK. If an error occurs or the VFS used by the pager does 
** not support the xShmXXX() methods, return an error code. *pbOpen is
** not modified in either case.
**
** If the pager is open on a temp-file (or in-memory database), or if
** the WAL file is already open, set *pbOpen to 1 and return SqlResult.SQLITE_OK
** without doing anything.
*/
int sqlite3PagerOpenWal(
Pager *pPager,                  /* Pager object */
int *pbOpen                     /* OUT: Set to true if call is a no-op */
){
var rc = SqlResult.SQLITE_OK;             /* Return code */

assert( assert_pager_state(pPager) );
assert( pPager.eState==PagerState.PAGER_OPEN   || pbOpen );
assert( pPager.eState==PagerState.PAGER_READER || !pbOpen );
assert( pbOpen==0 || *pbOpen==0 );
assert( pbOpen!=0 || (!pPager.tempFile && !pPager.pWal) );

if( !pPager.tempFile && !pPager.pWal ){
if( !sqlite3PagerWalSupported(pPager) ) return SQLITE_CANTOPEN;

/* Close any rollback journal previously open */
sqlite3OsClose(pPager.jfd);

rc = pagerOpenWal(pPager);
if( rc==SqlResult.SQLITE_OK ){
pPager.journalMode = PAGER_JOURNALMODE_WAL;
pPager.eState = PagerState.PAGER_OPEN;
}
}else{
*pbOpen = 1;
}

return rc;
}

/*
** This function is called to close the connection to the log file prior
** to switching from WAL to rollback mode.
**
** Before closing the log file, this function attempts to take an 
** EXCLUSIVE lock on the database file. If this cannot be obtained, an
** error (SQLITE_BUSY) is returned and the log connection is not closed.
** If successful, the EXCLUSIVE lock is not released before returning.
*/
int sqlite3PagerCloseWal(Pager *pPager){
var rc = SqlResult.SQLITE_OK;

assert( pPager.journalMode==PAGER_JOURNALMODE_WAL );

/* If the log file is not already open, but does exist in the file-system,
** it may need to be checkpointed before the connection can switch to
** rollback mode. Open it now so this can happen.
*/
if( !pPager.pWal ){
int logexists = 0;
rc = pagerLockDb(pPager, SHARED_LOCK);
if( rc==SqlResult.SQLITE_OK ){
rc = sqlite3OsAccess(
pPager.pVfs, pPager.zWal, SQLITE_ACCESS_EXISTS, &logexists
);
}
if( rc==SqlResult.SQLITE_OK && logexists ){
rc = pagerOpenWal(pPager);
}
}

/* Checkpoint and close the log. Because an EXCLUSIVE lock is held on
** the database file, the log and log-summary files will be deleted.
*/
if( rc==SqlResult.SQLITE_OK && pPager.pWal ){
rc = pagerExclusiveLock(pPager);
if( rc==SqlResult.SQLITE_OK ){
rc = sqlite3WalClose(pPager.pWal, pPager.ckptSyncFlags,
           pPager.pageSize, (u8*)pPager.pTmpSpace);
pPager.pWal = 0;
}
}
return rc;
}

#if SQLITE_HAS_CODEC
																			/*
** This function is called by the wal module when writing page content
** into the log file.
**
** This function returns a pointer to a buffer containing the encrypted
** page content. If a malloc fails, this function may return NULL.
*/
void sqlite3PagerCodec(PgHdr *pPg){
voidaData = 0;
CODEC2(pPg->pPager, pPg->pData, pPg->pgno, 6, return 0, aData);
return aData;
}
#endif
																			
#endif

        }
    }

}