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
		#if TRACE
																																						
static bool sqlite3PagerTrace = false;  /* True to enable tracing */
//define sqlite3DebugPrintf printf
//define PAGERTRACE(X)     if( sqlite3PagerTrace ){ sqlite3DebugPrintf X; }
static void PAGERTRACE( string T, params object[] ap ) { if ( sqlite3PagerTrace )sqlite3DebugPrintf( T, ap ); }
#else
		//#define PAGERTRACE(X)
		static void PAGERTRACE (string T, params object[] ap)
		{
		}

		#endif
		///<summary>
		/// The following two macros are used within the PAGERTRACE() macros above
		/// to print out file-descriptors.
		///
		/// PAGERID() takes a pointer to a Pager struct as its argument. The
		/// associated file-descriptor is returned. FILEHANDLEID() takes an sqlite3_file
		/// struct as its argument.
		///</summary>
		//#define PAGERID(p) ((int)(p.fd))
		static int PAGERID (Pager p)
		{
			return p.GetHashCode ();
		}

		//#define FILEHANDLEID(fd) ((int)fd)
		static int FILEHANDLEID (sqlite3_file fd)
		{
			return fd.GetHashCode ();
		}

		///
///<summary>
///The Pager.eState variable stores the current 'state' of a pager. A
///pager may be in any one of the seven states shown in the following
///state diagram.
///
///</summary>
///<param name="OPEN <">+</param>
///<param name="|         |      |">|         |      |</param>
///<param name="V         |      |">V         |      |</param>
///<param name="+">+      |</param>
///<param name="|              |                |">|              |                |</param>
///<param name="|              V                |">|              V                |</param>
///<param name="|<">> ERROR</param>
///<param name="|              |                ^  ">|              |                ^  </param>
///<param name="|              V                |">|              V                |</param>
///<param name="|<">>|</param>
///<param name="|              |                |">|              |                |</param>
///<param name="|              V                |">|              V                |</param>
///<param name="|<">>|</param>
///<param name="|              |                |">|              |                |</param>
///<param name="|              V                |">|              V                |</param>
///<param name="+<">>+</param>
///<param name=""></param>
///<param name=""></param>
///<param name="List of state transitions and the C [function] that performs each:">List of state transitions and the C [function] that performs each:</param>
///<param name=""></param>
///<param name="OPEN              ">> READER              [sqlite3PagerSharedLock]</param>
///<param name="READER            ">> OPEN                [pager_unlock]</param>
///<param name=""></param>
///<param name="READER            ">> WRITER_LOCKED       [sqlite3PagerBegin]</param>
///<param name="WRITER_LOCKED     ">> WRITER_CACHEMOD     [pager_open_journal]</param>
///<param name="WRITER_CACHEMOD   ">> WRITER_DBMOD        [syncJournal]</param>
///<param name="WRITER_DBMOD      ">> WRITER_FINISHED     [sqlite3PagerCommitPhaseOne]</param>
///<param name="WRITER_***        ">> READER              [pager_end_transaction]</param>
///<param name=""></param>
///<param name="WRITER_***        ">> ERROR               [pager_error]</param>
///<param name="ERROR             ">> OPEN                [pager_unlock]</param>
///<param name=""></param>
///<param name=""></param>
///<param name="OPEN:">OPEN:</param>
///<param name=""></param>
///<param name="The pager starts up in this state. Nothing is guaranteed in this">The pager starts up in this state. Nothing is guaranteed in this</param>
///<param name="state "> the file may or may not be locked and the database size is</param>
///<param name="unknown. The database may not be read or written.">unknown. The database may not be read or written.</param>
///<param name=""></param>
///<param name="No read or write transaction is active.">No read or write transaction is active.</param>
///<param name="Any lock, or no lock at all, may be held on the database file.">Any lock, or no lock at all, may be held on the database file.</param>
///<param name="The dbSize, dbOrigSize and dbFileSize variables may not be trusted.">The dbSize, dbOrigSize and dbFileSize variables may not be trusted.</param>
///<param name=""></param>
///<param name="READER:">READER:</param>
///<param name=""></param>
///<param name="In this state all the requirements for reading the database in ">In this state all the requirements for reading the database in </param>
///<param name="rollback (non">WAL) mode are met. Unless the pager is (or recently</param>
///<param name="was) in exclusive">level read transaction is </param>
///<param name="open. The database size is known in this state.">open. The database size is known in this state.</param>
///<param name=""></param>
///<param name="A connection running with locking_mode=normal enters this state when">A connection running with locking_mode=normal enters this state when</param>
///<param name="it opens a read">transaction on the database and returns to state</param>
///<param name="OPEN after the read">transaction is completed. However a connection</param>
///<param name="running in locking_mode=exclusive (including temp databases) remains in">running in locking_mode=exclusive (including temp databases) remains in</param>
///<param name="this state even after the read">transaction is closed. The only way</param>
///<param name="a locking_mode=exclusive connection can transition from READER to OPEN">a locking_mode=exclusive connection can transition from READER to OPEN</param>
///<param name="is via the ERROR state (see below).">is via the ERROR state (see below).</param>
///<param name=""></param>
///<param name="A read transaction may be active (but a write">transaction cannot).</param>
///<param name="A SHARED or greater lock is held on the database file.">A SHARED or greater lock is held on the database file.</param>
///<param name="The dbSize variable may be trusted (even if a user">level read </param>
///<param name="transaction is not active). The dbOrigSize and dbFileSize variables">transaction is not active). The dbOrigSize and dbFileSize variables</param>
///<param name="may not be trusted at this point.">may not be trusted at this point.</param>
///<param name="If the database is a WAL database, then the WAL connection is open.">If the database is a WAL database, then the WAL connection is open.</param>
///<param name="Even if a read">transaction is not open, it is guaranteed that </param>
///<param name="there is no hot">system.</param>
///<param name=""></param>
///<param name="WRITER_LOCKED:">WRITER_LOCKED:</param>
///<param name=""></param>
///<param name="The pager moves to this state from READER when a write">transaction</param>
///<param name="is first opened on the database. In WRITER_LOCKED state, all locks ">is first opened on the database. In WRITER_LOCKED state, all locks </param>
///<param name="required to start a write">transaction are held, but no actual </param>
///<param name="modifications to the cache or database have taken place.">modifications to the cache or database have taken place.</param>
///<param name=""></param>
///<param name="In rollback mode, a RESERVED or (if the transaction was opened with ">In rollback mode, a RESERVED or (if the transaction was opened with </param>
///<param name="BEGIN EXCLUSIVE) EXCLUSIVE lock is obtained on the database file when">BEGIN EXCLUSIVE) EXCLUSIVE lock is obtained on the database file when</param>
///<param name="moving to this state, but the journal file is not written to or opened ">moving to this state, but the journal file is not written to or opened </param>
///<param name="to in this state. If the transaction is committed or rolled back while ">to in this state. If the transaction is committed or rolled back while </param>
///<param name="in WRITER_LOCKED state, all that is required is to unlock the database ">in WRITER_LOCKED state, all that is required is to unlock the database </param>
///<param name="file.">file.</param>
///<param name=""></param>
///<param name="IN WAL mode, WalBeginWriteTransaction() is called to lock the log file.">IN WAL mode, WalBeginWriteTransaction() is called to lock the log file.</param>
///<param name="If the connection is running with locking_mode=exclusive, an attempt">If the connection is running with locking_mode=exclusive, an attempt</param>
///<param name="is made to obtain an EXCLUSIVE lock on the database file.">is made to obtain an EXCLUSIVE lock on the database file.</param>
///<param name=""></param>
///<param name="A write transaction is active.">A write transaction is active.</param>
///<param name="If the connection is open in rollback">mode, a RESERVED or greater </param>
///<param name="lock is held on the database file.">lock is held on the database file.</param>
///<param name="If the connection is open in WAL">mode, a WAL write transaction</param>
///<param name="is open (i.e. sqlite3WalBeginWriteTransaction() has been successfully">is open (i.e. sqlite3WalBeginWriteTransaction() has been successfully</param>
///<param name="called).">called).</param>
///<param name="The dbSize, dbOrigSize and dbFileSize variables are all valid.">The dbSize, dbOrigSize and dbFileSize variables are all valid.</param>
///<param name="The contents of the pager cache have not been modified.">The contents of the pager cache have not been modified.</param>
///<param name="The journal file may or may not be open.">The journal file may or may not be open.</param>
///<param name="Nothing (not even the first header) has been written to the journal.">Nothing (not even the first header) has been written to the journal.</param>
///<param name=""></param>
///<param name="WRITER_CACHEMOD:">WRITER_CACHEMOD:</param>
///<param name=""></param>
///<param name="A pager moves from WRITER_LOCKED state to this state when a page is">A pager moves from WRITER_LOCKED state to this state when a page is</param>
///<param name="first modified by the upper layer. In rollback mode the journal file">first modified by the upper layer. In rollback mode the journal file</param>
///<param name="is opened (if it is not already open) and a header written to the">is opened (if it is not already open) and a header written to the</param>
///<param name="start of it. The database file on disk has not been modified.">start of it. The database file on disk has not been modified.</param>
///<param name=""></param>
///<param name="A write transaction is active.">A write transaction is active.</param>
///<param name="A RESERVED or greater lock is held on the database file.">A RESERVED or greater lock is held on the database file.</param>
///<param name="The journal file is open and the first header has been written ">The journal file is open and the first header has been written </param>
///<param name="to it, but the header has not been synced to disk.">to it, but the header has not been synced to disk.</param>
///<param name="The contents of the page cache have been modified.">The contents of the page cache have been modified.</param>
///<param name=""></param>
///<param name="WRITER_DBMOD:">WRITER_DBMOD:</param>
///<param name=""></param>
///<param name="The pager transitions from WRITER_CACHEMOD into WRITER_DBMOD state">The pager transitions from WRITER_CACHEMOD into WRITER_DBMOD state</param>
///<param name="when it modifies the contents of the database file. WAL connections">when it modifies the contents of the database file. WAL connections</param>
///<param name="never enter this state (since they do not modify the database file,">never enter this state (since they do not modify the database file,</param>
///<param name="just the log file).">just the log file).</param>
///<param name=""></param>
///<param name="A write transaction is active.">A write transaction is active.</param>
///<param name="An EXCLUSIVE or greater lock is held on the database file.">An EXCLUSIVE or greater lock is held on the database file.</param>
///<param name="The journal file is open and the first header has been written ">The journal file is open and the first header has been written </param>
///<param name="and synced to disk.">and synced to disk.</param>
///<param name="The contents of the page cache have been modified (and possibly">The contents of the page cache have been modified (and possibly</param>
///<param name="written to disk).">written to disk).</param>
///<param name=""></param>
///<param name="WRITER_FINISHED:">WRITER_FINISHED:</param>
///<param name=""></param>
///<param name="It is not possible for a WAL connection to enter this state.">It is not possible for a WAL connection to enter this state.</param>
///<param name=""></param>
///<param name="A rollback">mode pager changes to WRITER_FINISHED state from WRITER_DBMOD</param>
///<param name="state after the entire transaction has been successfully written into the">state after the entire transaction has been successfully written into the</param>
///<param name="database file. In this state the transaction may be committed simply">database file. In this state the transaction may be committed simply</param>
///<param name="by finalizing the journal file. Once in WRITER_FINISHED state, it is ">by finalizing the journal file. Once in WRITER_FINISHED state, it is </param>
///<param name="not possible to modify the database further. At this point, the upper ">not possible to modify the database further. At this point, the upper </param>
///<param name="layer must either commit or rollback the transaction.">layer must either commit or rollback the transaction.</param>
///<param name=""></param>
///<param name="A write transaction is active.">A write transaction is active.</param>
///<param name="An EXCLUSIVE or greater lock is held on the database file.">An EXCLUSIVE or greater lock is held on the database file.</param>
///<param name="All writing and syncing of journal and database data has finished.">All writing and syncing of journal and database data has finished.</param>
///<param name="If no error occured, all that remains is to finalize the journal to">If no error occured, all that remains is to finalize the journal to</param>
///<param name="commit the transaction. If an error did occur, the caller will need">commit the transaction. If an error did occur, the caller will need</param>
///<param name="to rollback the transaction. ">to rollback the transaction. </param>
///<param name=""></param>
///<param name="ERROR:">ERROR:</param>
///<param name=""></param>
///<param name="The ERROR state is entered when an IO or disk">full error (including</param>
///<param name="SQLITE_IOERR_NOMEM) occurs at a point in the code that makes it ">SQLITE_IOERR_NOMEM) occurs at a point in the code that makes it </param>
///<param name="difficult to be sure that the in">memory pager state (cache contents, </param>
///<param name="db size etc.) are consistent with the contents of the file">system.</param>
///<param name=""></param>
///<param name="Temporary pager files may enter the ERROR state, but in">memory pagers</param>
///<param name="cannot.">cannot.</param>
///<param name=""></param>
///<param name="For example, if an IO error occurs while performing a rollback, ">For example, if an IO error occurs while performing a rollback, </param>
///<param name="the contents of the page">cache may be left in an inconsistent state.</param>
///<param name="At this point it would be dangerous to change back to READER state">At this point it would be dangerous to change back to READER state</param>
///<param name="(as usually happens after a rollback). Any subsequent readers might">(as usually happens after a rollback). Any subsequent readers might</param>
///<param name="report database corruption (due to the inconsistent cache), and if">report database corruption (due to the inconsistent cache), and if</param>
///<param name="they upgrade to writers, they may inadvertently corrupt the database">they upgrade to writers, they may inadvertently corrupt the database</param>
///<param name="file. To avoid this hazard, the pager switches into the ERROR state">file. To avoid this hazard, the pager switches into the ERROR state</param>
///<param name="instead of READER following such an error.">instead of READER following such an error.</param>
///<param name=""></param>
///<param name="Once it has entered the ERROR state, any attempt to use the pager">Once it has entered the ERROR state, any attempt to use the pager</param>
///<param name="to read or write data returns an error. Eventually, once all ">to read or write data returns an error. Eventually, once all </param>
///<param name="outstanding transactions have been abandoned, the pager is able to">outstanding transactions have been abandoned, the pager is able to</param>
///<param name="transition back to OPEN state, discarding the contents of the ">transition back to OPEN state, discarding the contents of the </param>
///<param name="page">memory state at the same time. Everything</param>
///<param name="is reloaded from disk (and, if necessary, hot">journal rollback peformed)</param>
///<param name="when a read">transaction is next opened on the pager (transitioning</param>
///<param name="the pager into READER state). At that point the system has recovered ">the pager into READER state). At that point the system has recovered </param>
///<param name="from the error.">from the error.</param>
///<param name=""></param>
///<param name="Specifically, the pager jumps into the ERROR state if:">Specifically, the pager jumps into the ERROR state if:</param>
///<param name=""></param>
///<param name="1. An error occurs while attempting a rollback. This happens in">1. An error occurs while attempting a rollback. This happens in</param>
///<param name="function sqlite3PagerRollback().">function sqlite3PagerRollback().</param>
///<param name=""></param>
///<param name="2. An error occurs while attempting to finalize a journal file">2. An error occurs while attempting to finalize a journal file</param>
///<param name="following a commit in function sqlite3PagerCommitPhaseTwo().">following a commit in function sqlite3PagerCommitPhaseTwo().</param>
///<param name=""></param>
///<param name="3. An error occurs while attempting to write to the journal or">3. An error occurs while attempting to write to the journal or</param>
///<param name="database file in function pagerStress() in order to free up">database file in function pagerStress() in order to free up</param>
///<param name="memory.">memory.</param>
///<param name=""></param>
///<param name="In other cases, the error is returned to the b">tree</param>
///<param name="layer then attempts a rollback operation. If the error condition ">layer then attempts a rollback operation. If the error condition </param>
///<param name="persists, the pager enters the ERROR state via condition (1) above.">persists, the pager enters the ERROR state via condition (1) above.</param>
///<param name=""></param>
///<param name="Condition (3) is necessary because it can be triggered by a read">only</param>
///<param name="statement executed within a transaction. In this case, if the error">statement executed within a transaction. In this case, if the error</param>
///<param name="code were simply returned to the user, the b">tree layer would not</param>
///<param name="automatically attempt a rollback, as it assumes that an error in a">automatically attempt a rollback, as it assumes that an error in a</param>
///<param name="read">only statement cannot leave the pager in an internally inconsistent </param>
///<param name="state.">state.</param>
///<param name=""></param>
///<param name="The Pager.errCode variable is set to something other than SQLITE_OK.">The Pager.errCode variable is set to something other than SQLITE_OK.</param>
///<param name="There are one or more outstanding references to pages (after the">There are one or more outstanding references to pages (after the</param>
///<param name="last reference is dropped the pager should move back to OPEN state).">last reference is dropped the pager should move back to OPEN state).</param>
///<param name="The pager is not an in">memory pager.</param>
///<param name=""></param>
///<param name=""></param>
///<param name="Notes:">Notes:</param>
///<param name=""></param>
///<param name="A pager is never in WRITER_DBMOD or WRITER_FINISHED state if the">A pager is never in WRITER_DBMOD or WRITER_FINISHED state if the</param>
///<param name="connection is open in WAL mode. A WAL connection is always in one">connection is open in WAL mode. A WAL connection is always in one</param>
///<param name="of the first four states.">of the first four states.</param>
///<param name=""></param>
///<param name="Normally, a connection open in exclusive mode is never in PAGER_OPEN">Normally, a connection open in exclusive mode is never in PAGER_OPEN</param>
///<param name="state. There are two exceptions: immediately after exclusive">mode has</param>
///<param name="been turned on (and before any read or write transactions are ">been turned on (and before any read or write transactions are </param>
///<param name="executed), and when the pager is leaving the "error state".">executed), and when the pager is leaving the "error state".</param>
///<param name=""></param>
///<param name="See also: assert_pager_state().">See also: assert_pager_state().</param>
///<param name=""></param>

		//#define PAGER_OPEN                  0
		//#define PAGER_READER                1
		//#define PAGER_WRITER_LOCKED         2
		//#define PAGER_WRITER_CACHEMOD       3
		//#define PAGER_WRITER_DBMOD          4
		//#define PAGER_WRITER_FINISHED       5
		//#define PAGER_ERROR                 6
		const int PAGER_OPEN = 0;

		const int PAGER_READER = 1;

		const int PAGER_WRITER_LOCKED = 2;

		const int PAGER_WRITER_CACHEMOD = 3;

		const int PAGER_WRITER_DBMOD = 4;

		const int PAGER_WRITER_FINISHED = 5;

		const int PAGER_ERROR = 6;

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
///<param name="PAGER_OPEN state.">PAGER_OPEN state.</param>
///<param name=""></param>

		//#define UNKNOWN_LOCK                (EXCLUSIVE_LOCK+1)
		const int UNKNOWN_LOCK = (EXCLUSIVE_LOCK + 1);

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
		static bool CODEC1 (Pager P, byte[] D, uint N///
///<summary>
///page number 
///</summary>

		, int X///
///<summary>
///E (moved to caller 
///</summary>

		)
		{
			return ((P.xCodec != null) && (P.xCodec (P.pCodec, D, N, X) == null));
		}

		// The E parameter is what executes when there is an error, 
		// cannot implement here, since this is not really a macro
		// calling code must be modified to call E when truen
		//# define CODEC2(P,D,N,X,E,O) \
		//if( P.xCodec==0 ){ O=(char*)D; }else \
		//if( (O=(char*)(P.xCodec(P.pCodec,D,N,X)))==0 ){ E; }
		static bool CODEC2 (Pager P, byte[] D, uint N, int X, ref byte[] O)
		{
			if (P.xCodec == null) {
				O = D;
				// do nothing
				return false;
			}
			else {
				return ((O = P.xCodec (P.pCodec, D, N, X)) == null);
			}
		}

		#else
																																						// define CODEC1(P,D,N,X,E)   /* NO-OP */
static bool CODEC1 (Pager P, byte[] D, uint N /* page number */, int X /* E (moved to caller */)  { return false; }
// define CODEC2(P,D,N,X,E,O) O=(char*)D
static bool CODEC2( Pager P, byte[] D, uint N, int X, ref byte[] O ) { O = D; return false; }
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

		///<summary>
		/// An instance of the following structure is allocated for each active
		/// savepoint and statement transaction in the system. All such structures
		/// are stored in the Pager.aSavepoint[] array, which is allocated and
		/// resized using sqlite3Realloc().
		///
		/// When a savepoint is created, the PagerSavepoint.iHdrOffset field is
		/// set to 0. If a journal-header is written into the main journal while
		/// the savepoint is active, then iHdrOffset is set to the byte offset
		/// immediately following the last journal record written into the main
		/// journal before the journal-header. This is required during savepoint
		/// rollback (see pagerPlaybackSavepoint()).
		///
		///</summary>
		//typedef struct PagerSavepoint PagerSavepoint;
		public class PagerSavepoint
		{
			public i64 iOffset;

			///
///<summary>
///Starting offset in main journal 
///</summary>

			public i64 iHdrOffset;

			///
///<summary>
///See above 
///</summary>

			public Bitvec pInSavepoint;

			///
///<summary>
///Set of pages in this savepoint 
///</summary>

			public Pgno nOrig;

			///
///<summary>
///Original number of pages in file 
///</summary>

			public Pgno iSubRec;

			///
///<summary>
///</summary>
///<param name="Index of first record in sub">journal </param>

			#if !SQLITE_OMIT_WAL
																																																									public u32 aWalData[WAL_SAVEPOINT_NDATA];        /* WAL savepoint context */
#else
			public object aWalData = null;

			///
///<summary>
///Used for C# convenience 
///</summary>

			#endif
			public static implicit operator bool (PagerSavepoint b) {
				return (b != null);
			}
		};


		///
///<summary>
///A open page cache is an instance of struct Pager. A description of
///some of the more important member variables follows:
///
///eState
///
///The current 'state' of the pager object. See the comment and state
///diagram above for a description of the pager state.
///
///eLock
///
///</summary>
///<param name="For a real on"></param>
///<param name="NO_LOCK, SHARED_LOCK, RESERVED_LOCK or EXCLUSIVE_LOCK.">NO_LOCK, SHARED_LOCK, RESERVED_LOCK or EXCLUSIVE_LOCK.</param>
///<param name=""></param>
///<param name="For a temporary or in">memory database (neither of which require any</param>
///<param name="locks), this variable is always set to EXCLUSIVE_LOCK. Since such">locks), this variable is always set to EXCLUSIVE_LOCK. Since such</param>
///<param name="databases always have Pager.exclusiveMode==1, this tricks the pager">databases always have Pager.exclusiveMode==1, this tricks the pager</param>
///<param name="logic into thinking that it already has all the locks it will ever">logic into thinking that it already has all the locks it will ever</param>
///<param name="need (and no reason to release them).">need (and no reason to release them).</param>
///<param name=""></param>
///<param name="In some (obscure) circumstances, this variable may also be set to">In some (obscure) circumstances, this variable may also be set to</param>
///<param name="UNKNOWN_LOCK. See the comment above the #define of UNKNOWN_LOCK for">UNKNOWN_LOCK. See the comment above the #define of UNKNOWN_LOCK for</param>
///<param name="details.">details.</param>
///<param name=""></param>
///<param name="changeCountDone">changeCountDone</param>
///<param name=""></param>
///<param name="This boolean variable is used to make sure that the change">counter </param>
///<param name="(the 4">byte header field at byte offset 24 of the database file) is </param>
///<param name="not updated more often than necessary. ">not updated more often than necessary. </param>
///<param name=""></param>
///<param name="It is set to true when the change">counter field is updated, which </param>
///<param name="can only happen if an exclusive lock is held on the database file.">can only happen if an exclusive lock is held on the database file.</param>
///<param name="It is cleared (set to false) whenever an exclusive lock is ">It is cleared (set to false) whenever an exclusive lock is </param>
///<param name="relinquished on the database file. Each time a transaction is committed,">relinquished on the database file. Each time a transaction is committed,</param>
///<param name="The changeCountDone flag is inspected. If it is true, the work of">The changeCountDone flag is inspected. If it is true, the work of</param>
///<param name="updating the change">counter is omitted for the current transaction.</param>
///<param name=""></param>
///<param name="This mechanism means that when running in exclusive mode, a connection ">This mechanism means that when running in exclusive mode, a connection </param>
///<param name="need only update the change">counter once, for the first transaction</param>
///<param name="committed.">committed.</param>
///<param name=""></param>
///<param name="setMaster">setMaster</param>
///<param name=""></param>
///<param name="When PagerCommitPhaseOne() is called to commit a transaction, it may">When PagerCommitPhaseOne() is called to commit a transaction, it may</param>
///<param name="(or may not) specify a master">journal name to be written into the </param>
///<param name="journal file before it is synced to disk.">journal file before it is synced to disk.</param>
///<param name=""></param>
///<param name="Whether or not a journal file contains a master">journal pointer affects </param>
///<param name="the way in which the journal file is finalized after the transaction is ">the way in which the journal file is finalized after the transaction is </param>
///<param name="committed or rolled back when running in "journal_mode=PERSIST" mode.">committed or rolled back when running in "journal_mode=PERSIST" mode.</param>
///<param name="If a journal file does not contain a master">journal pointer, it is</param>
///<param name="finalized by overwriting the first journal header with zeroes. If">finalized by overwriting the first journal header with zeroes. If</param>
///<param name="it does contain a master">journal pointer the journal file is finalized </param>
///<param name="by truncating it to zero bytes, just as if the connection were ">by truncating it to zero bytes, just as if the connection were </param>
///<param name="running in "journal_mode=truncate" mode.">running in "journal_mode=truncate" mode.</param>
///<param name=""></param>
///<param name="Journal files that contain master journal pointers cannot be finalized">Journal files that contain master journal pointers cannot be finalized</param>
///<param name="simply by overwriting the first journal">header with zeroes, as the</param>
///<param name="master journal pointer could interfere with hot">journal rollback of any</param>
///<param name="subsequently interrupted transaction that reuses the journal file.">subsequently interrupted transaction that reuses the journal file.</param>
///<param name=""></param>
///<param name="The flag is cleared as soon as the journal file is finalized (either">The flag is cleared as soon as the journal file is finalized (either</param>
///<param name="by PagerCommitPhaseTwo or PagerRollback). If an IO error prevents the">by PagerCommitPhaseTwo or PagerRollback). If an IO error prevents the</param>
///<param name="journal file from being successfully finalized, the setMaster flag">journal file from being successfully finalized, the setMaster flag</param>
///<param name="is cleared anyway (and the pager will move to ERROR state).">is cleared anyway (and the pager will move to ERROR state).</param>
///<param name=""></param>
///<param name="doNotSpill, doNotSyncSpill">doNotSpill, doNotSyncSpill</param>
///<param name=""></param>
///<param name="These two boolean variables control the behaviour of cache">spills</param>
///<param name="(calls made by the pcache module to the pagerStress() routine to">(calls made by the pcache module to the pagerStress() routine to</param>
///<param name="write cached data to the file">system in order to free up memory).</param>
///<param name=""></param>
///<param name="When doNotSpill is non">zero, writing to the database from pagerStress()</param>
///<param name="is disabled altogether. This is done in a very obscure case that">is disabled altogether. This is done in a very obscure case that</param>
///<param name="comes up during savepoint rollback that requires the pcache module">comes up during savepoint rollback that requires the pcache module</param>
///<param name="to allocate a new page to prevent the journal file from being written">to allocate a new page to prevent the journal file from being written</param>
///<param name="while it is being traversed by code in pager_playback().">while it is being traversed by code in pager_playback().</param>
///<param name=""></param>
///<param name="If doNotSyncSpill is non">zero, writing to the database from pagerStress()</param>
///<param name="is permitted, but syncing the journal file is not. This flag is set">is permitted, but syncing the journal file is not. This flag is set</param>
///<param name="by sqlite3PagerWrite() when the file">size is larger than</param>
///<param name="the database page">size in order to prevent a journal sync from happening </param>
///<param name="in between the journalling of two pages on the same sector. ">in between the journalling of two pages on the same sector. </param>
///<param name=""></param>
///<param name="subjInMemory">subjInMemory</param>
///<param name=""></param>
///<param name="This is a boolean variable. If true, then any required sub">journal</param>
///<param name="is opened as an in">memory</param>
///<param name="sub">memory pager files.</param>
///<param name=""></param>
///<param name="This variable is updated by the upper layer each time a new ">This variable is updated by the upper layer each time a new </param>
///<param name="write">transaction is opened.</param>
///<param name=""></param>
///<param name="dbSize, dbOrigSize, dbFileSize">dbSize, dbOrigSize, dbFileSize</param>
///<param name=""></param>
///<param name="Variable dbSize is set to the number of pages in the database file.">Variable dbSize is set to the number of pages in the database file.</param>
///<param name="It is valid in PAGER_READER and higher states (all states except for">It is valid in PAGER_READER and higher states (all states except for</param>
///<param name="OPEN and ERROR). ">OPEN and ERROR). </param>
///<param name=""></param>
///<param name="dbSize is set based on the size of the database file, which may be ">dbSize is set based on the size of the database file, which may be </param>
///<param name="larger than the size of the database (the value stored at offset">larger than the size of the database (the value stored at offset</param>
///<param name="28 of the database header by the btree). If the size of the file">28 of the database header by the btree). If the size of the file</param>
///<param name="is not an integer multiple of the page">size, the value stored in</param>
///<param name="dbSize is rounded down (i.e. a 5KB file with 2K page">size has dbSize==2).</param>
///<param name="Except, any file that is greater than 0 bytes in size is considered">Except, any file that is greater than 0 bytes in size is considered</param>
///<param name="to have at least one page. (i.e. a 1KB file with 2K page">size leads</param>
///<param name="to dbSize==1).">to dbSize==1).</param>
///<param name=""></param>
///<param name="During a write">numbers greater than</param>
///<param name="dbSize are modified in the cache, dbSize is updated accordingly.">dbSize are modified in the cache, dbSize is updated accordingly.</param>
///<param name="Similarly, if the database is truncated using PagerTruncateImage(), ">Similarly, if the database is truncated using PagerTruncateImage(), </param>
///<param name="dbSize is updated.">dbSize is updated.</param>
///<param name=""></param>
///<param name="Variables dbOrigSize and dbFileSize are valid in states ">Variables dbOrigSize and dbFileSize are valid in states </param>
///<param name="PAGER_WRITER_LOCKED and higher. dbOrigSize is a copy of the dbSize">PAGER_WRITER_LOCKED and higher. dbOrigSize is a copy of the dbSize</param>
///<param name="variable at the start of the transaction. It is used during rollback,">variable at the start of the transaction. It is used during rollback,</param>
///<param name="and to determine whether or not pages need to be journalled before">and to determine whether or not pages need to be journalled before</param>
///<param name="being modified.">being modified.</param>
///<param name=""></param>
///<param name="Throughout a write">transaction, dbFileSize contains the size of</param>
///<param name="the file on disk in pages. It is set to a copy of dbSize when the">the file on disk in pages. It is set to a copy of dbSize when the</param>
///<param name="write">transaction is first opened, and updated when VFS calls are made</param>
///<param name="to write or truncate the database file on disk. ">to write or truncate the database file on disk. </param>
///<param name=""></param>
///<param name="The only reason the dbFileSize variable is required is to suppress ">The only reason the dbFileSize variable is required is to suppress </param>
///<param name="unnecessary calls to xTruncate() after committing a transaction. If, ">unnecessary calls to xTruncate() after committing a transaction. If, </param>
///<param name="when a transaction is committed, the dbFileSize variable indicates ">when a transaction is committed, the dbFileSize variable indicates </param>
///<param name="that the database file is larger than the database image (Pager.dbSize), ">that the database file is larger than the database image (Pager.dbSize), </param>
///<param name="pager_truncate() is called. The pager_truncate() call uses xFilesize()">pager_truncate() is called. The pager_truncate() call uses xFilesize()</param>
///<param name="to measure the database file on disk, and then truncates it if required.">to measure the database file on disk, and then truncates it if required.</param>
///<param name="dbFileSize is not used when rolling back a transaction. In this case">dbFileSize is not used when rolling back a transaction. In this case</param>
///<param name="pager_truncate() is called unconditionally (which means there may be">pager_truncate() is called unconditionally (which means there may be</param>
///<param name="a call to xFilesize() that is not strictly required). In either case,">a call to xFilesize() that is not strictly required). In either case,</param>
///<param name="pager_truncate() may cause the file to become smaller or larger.">pager_truncate() may cause the file to become smaller or larger.</param>
///<param name=""></param>
///<param name="dbHintSize">dbHintSize</param>
///<param name=""></param>
///<param name="The dbHintSize variable is used to limit the number of calls made to">The dbHintSize variable is used to limit the number of calls made to</param>
///<param name="the VFS xFileControl(FCNTL_SIZE_HINT) method. ">the VFS xFileControl(FCNTL_SIZE_HINT) method. </param>
///<param name=""></param>
///<param name="dbHintSize is set to a copy of the dbSize variable when a">dbHintSize is set to a copy of the dbSize variable when a</param>
///<param name="write">transaction is opened (at the same time as dbFileSize and</param>
///<param name="dbOrigSize). If the xFileControl(FCNTL_SIZE_HINT) method is called,">dbOrigSize). If the xFileControl(FCNTL_SIZE_HINT) method is called,</param>
///<param name="dbHintSize is increased to the number of pages that correspond to the">dbHintSize is increased to the number of pages that correspond to the</param>
///<param name="size">hint passed to the method call. See pager_write_pagelist() for </param>
///<param name="details.">details.</param>
///<param name=""></param>
///<param name="errCode">errCode</param>
///<param name=""></param>
///<param name="The Pager.errCode variable is only ever used in PAGER_ERROR state. It">The Pager.errCode variable is only ever used in PAGER_ERROR state. It</param>
///<param name="is set to zero in all other states. In PAGER_ERROR state, Pager.errCode ">is set to zero in all other states. In PAGER_ERROR state, Pager.errCode </param>
///<param name="is always set to SQLITE_FULL, SQLITE_IOERR or one of the SQLITE_IOERR_XXX ">is always set to SQLITE_FULL, SQLITE_IOERR or one of the SQLITE_IOERR_XXX </param>
///<param name="sub">codes.</param>
///<param name=""></param>

		public class Pager
		{
			public sqlite3_vfs pVfs;

			///
///<summary>
///OS functions to use for IO 
///</summary>

			public bool exclusiveMode;

			///
///<summary>
///Boolean. True if locking_mode==EXCLUSIVE 
///</summary>

			public u8 journalMode;

			///
///<summary>
///One of the PAGER_JOURNALMODE_* values 
///</summary>

			public u8 useJournal;

			///
///<summary>
///Use a rollback journal on this file 
///</summary>

			public u8 noReadlock;

			///
///<summary>
///Do not bother to obtain readlocks 
///</summary>

			public bool noSync;

			///
///<summary>
///Do not sync the journal if true 
///</summary>

			public bool fullSync;

			///
///<summary>
///Do extra syncs of the journal for robustness 
///</summary>

			public u8 ckptSyncFlags;

			///
///<summary>
///SYNC_NORMAL or SYNC_FULL for checkpoint 
///</summary>

			public u8 syncFlags;

			///
///<summary>
///SYNC_NORMAL or SYNC_FULL otherwise 
///</summary>

			public bool tempFile;

			///
///<summary>
///zFilename is a temporary file 
///</summary>

			public bool readOnly;

			///
///<summary>
///</summary>
///<param name="True for a read">only database </param>

			public bool alwaysRollback;

			///
///<summary>
///Disable DontRollback() for all pages 
///</summary>

			public u8 memDb;

			///
///<summary>
///True to inhibit all file I/O 
///</summary>

			///
///<summary>
///
///The following block contains those class members that change during
///routine opertion.  Class members not in this block are either fixed
///when the pager is first created or else only change when there is a
///significant mode change (such as changing the page_size, locking_mode,
///or the journal_mode).  From another view, these class members describe
///the "state" of the pager, while other class members describe the
///"configuration" of the pager.
///
///</summary>

			public u8 eState;

			///
///<summary>
///Pager state (OPEN, READER, WRITER_LOCKED..) 
///</summary>

			public u8 eLock;

			///
///<summary>
///Current lock held on database file 
///</summary>

			public bool changeCountDone;

			///
///<summary>
///</summary>
///<param name="Set after incrementing the change">counter </param>

			public int setMaster;

			///
///<summary>
///</summary>
///<param name="True if a m">j name has been written to jrnl </param>

			public u8 doNotSpill;

			///
///<summary>
///</summary>
///<param name="Do not spill the cache when non">zero </param>

			public u8 doNotSyncSpill;

			///
///<summary>
///Do not do a spill that requires jrnl sync 
///</summary>

			public u8 subjInMemory;

			///
///<summary>
///</summary>
///<param name="True to use in">journals </param>

			public Pgno dbSize;

			///
///<summary>
///Number of pages in the database 
///</summary>

			public Pgno dbOrigSize;

			///
///<summary>
///dbSize before the current transaction 
///</summary>

			public Pgno dbFileSize;

			///
///<summary>
///Number of pages in the database file 
///</summary>

			public Pgno dbHintSize;

			///
///<summary>
///Value passed to FCNTL_SIZE_HINT call 
///</summary>

			public int errCode;

			///
///<summary>
///One of several kinds of errors 
///</summary>

			public int nRec;

			///
///<summary>
///</summary>
///<param name="Pages journalled since last j">header written </param>

			public u32 cksumInit;

			///
///<summary>
///</summary>
///<param name="Quasi">random value added to every checksum </param>

			public u32 nSubRec;

			///
///<summary>
///</summary>
///<param name="Number of records written to sub">journal </param>

			public Bitvec pInJournal;

			///
///<summary>
///One bit for each page in the database file 
///</summary>

			public sqlite3_file fd;

			///
///<summary>
///File descriptor for database 
///</summary>

			public sqlite3_file jfd;

			///
///<summary>
///File descriptor for main journal 
///</summary>

			public sqlite3_file sjfd;

			///
///<summary>
///</summary>
///<param name="File descriptor for sub">journal </param>

			public i64 journalOff;

			///
///<summary>
///Current write offset in the journal file 
///</summary>

			public i64 journalHdr;

			///
///<summary>
///Byte offset to previous journal header 
///</summary>

			sqlite3_backup _pBackup;

			///
///<summary>
///Pointer to list of ongoing backup processes 
///</summary>

			public sqlite3_backup pBackup {
				get {
					return _pBackup;
				}
				set {
					_pBackup = value;
				}
			}

			public PagerSavepoint[] aSavepoint;

			///
///<summary>
///Array of active savepoints 
///</summary>

			public int nSavepoint;

			///
///<summary>
///Number of elements in aSavepoint[] 
///</summary>

			public u8[] dbFileVers = new u8[16];

			///
///<summary>
///Changes whenever database file changes 
///</summary>

			///
///<summary>
///</summary>
///<param name="End of the routinely">changing class members</param>
///<param name=""></param>

			public u16 nExtra;

			///
///<summary>
///</summary>
///<param name="Add this many bytes to each in">memory page </param>

			public i16 nReserve;

			///
///<summary>
///Number of unused bytes at end of each page 
///</summary>

			public u32 vfsFlags;

			///
///<summary>
///Flags for sqlite3_vfs.xOpen() 
///</summary>

			public u32 sectorSize;

			///
///<summary>
///Assumed sector size during rollback 
///</summary>

			public int pageSize;

			///
///<summary>
///Number of bytes in a page 
///</summary>

			public Pgno mxPgno;

			///
///<summary>
///Maximum allowed size of the database 
///</summary>

			public i64 journalSizeLimit;

			///
///<summary>
///Size limit for persistent journal files 
///</summary>

			public string zFilename;

			///
///<summary>
///Name of the database file 
///</summary>

			public string zJournal;

			///
///<summary>
///Name of the journal file 
///</summary>

			public dxBusyHandler xBusyHandler;

			///
///<summary>
///Function to call when busy 
///</summary>

			public object pBusyHandlerArg;

			///
///<summary>
///Context argument for xBusyHandler 
///</summary>

			#if SQLITE_TEST || DEBUG
																																																									      public int nHit, nMiss;              /* Cache hits and missing */
      public int nRead, nWrite;            /* Database pages read/written */
#else
			public int nHit;

			#endif
			public dxReiniter xReiniter;

			//(DbPage*,int);/* Call this routine when reloading pages */
			#if SQLITE_HAS_CODEC
			//void *(*xCodec)(void*,void*,Pgno,int); 
			public dxCodec xCodec;

			///
///<summary>
///Routine for en/decoding data 
///</summary>

			//void (*xCodecSizeChng)(void*,int,int); 
			public dxCodecSizeChng xCodecSizeChng;

			///
///<summary>
///Notify of page size changes 
///</summary>

			//void (*xCodecFree)(void*);             
			public dxCodecFree xCodecFree;

			///
///<summary>
///Destructor for the codec 
///</summary>

			public codec_ctx pCodec;

			///
///<summary>
///First argument to xCodec... methods 
///</summary>

			#endif
			public byte[] pTmpSpace;

			///
///<summary>
///Pager.pageSize bytes of space for tmp use 
///</summary>

			public PCache pPCache;

			///
///<summary>
///Pointer to page cache object 
///</summary>

			#if !SQLITE_OMIT_WAL
																																																									public Wal pWal;                       /* Write-ahead log used by "journal_mode=wal" */
public string zWal;                    /* File name for write-ahead log */
#else
			public sqlite3_vfs pWal = null;

			///
///<summary>
///Having this dummy here makes C# easier 
///</summary>

			#endif
			public///<summary>
			/// The size of the of each page record in the journal is given by
			/// the following macro.
			///
			///</summary>
			//#define JOURNAL_PG_SZ(pPager)  ((pPager.pageSize) + 8)
			int JOURNAL_PG_SZ ()
			{
				return (this.pageSize + 8);
			}

			public///<summary>
			/// The journal header size for this pager. This is usually the same
			/// size as a single disk sector. See also setSectorSize().
			///
			///</summary>
			//#define JOURNAL_HDR_SZ(pPager) (pPager.sectorSize)
			u32 JOURNAL_HDR_SZ ()
			{
				return (this.sectorSize);
			}

			public///<summary>
			/// Return true if this pager uses a write-ahead log instead of the usual
			/// rollback journal. Otherwise false.
			///
			///</summary>
			#if !SQLITE_OMIT_WAL
																																																			static int pagerUseWal(Pager *pPager){
return (pPager->pWal!=0);
}
#else
			//# define pagerUseWal(x) 0
			bool pagerUseWal ()
			{
				return false;
			}

			public int pagerRollbackWal ()
			{
				return 0;
			}

			public int pagerWalFrames (PgHdr w, Pgno x, int y, int z)
			{
				return 0;
			}

			public int pagerOpenWalIfPresent ()
			{
				return SQLITE_OK;
			}

			public int pagerBeginReadTransaction ()
			{
				return SQLITE_OK;
			}

			public bool assert_pager_state ()
			{
				Pager pPager = this;
				///
///<summary>
///State must be valid. 
///</summary>

				Debug.Assert (this.eState == PAGER_OPEN || this.eState == PAGER_READER || this.eState == PAGER_WRITER_LOCKED || this.eState == PAGER_WRITER_CACHEMOD || this.eState == PAGER_WRITER_DBMOD || this.eState == PAGER_WRITER_FINISHED || this.eState == PAGER_ERROR);
				///
///<summary>
///</summary>
///<param name="Regardless of the current state, a temp">file connection always behaves</param>
///<param name="as if it has an exclusive lock on the database file. It never updates">as if it has an exclusive lock on the database file. It never updates</param>
///<param name="the change">counter field, so the changeCountDone flag is always set.</param>
///<param name=""></param>

				Debug.Assert (this.tempFile == false || this.eLock == EXCLUSIVE_LOCK);
				Debug.Assert (this.tempFile == false || pPager.changeCountDone);
				///
///<summary>
///</summary>
///<param name="If the useJournal flag is clear, the journal">mode must be "OFF". </param>
///<param name="And if the journal">mode is "OFF", the journal file must not be open.</param>
///<param name=""></param>

				Debug.Assert (this.journalMode == PAGER_JOURNALMODE_OFF || this.useJournal != 0);
				Debug.Assert (this.journalMode != PAGER_JOURNALMODE_OFF || !isOpen (this.jfd));
				///
///<summary>
///</summary>
///<param name="Check that MEMDB implies noSync. And an in">memory journal. Since </param>
///<param name="this means an in">memory pager performs no IO at all, it cannot encounter </param>
///<param name="either SQLITE_IOERR or SQLITE_FULL during rollback or while finalizing ">either SQLITE_IOERR or SQLITE_FULL during rollback or while finalizing </param>
///<param name="a journal file. (although the in">memory journal implementation may </param>
///<param name="return SQLITE_IOERR_NOMEM while the journal file is being written). It ">return SQLITE_IOERR_NOMEM while the journal file is being written). It </param>
///<param name="is therefore not possible for an in">memory pager to enter the ERROR </param>
///<param name="state.">state.</param>
///<param name=""></param>

				if (
				#if SQLITE_OMIT_MEMORYDB
																																																																						0!=MEMDB
#else
				0 != pPager.memDb
				#endif
				) {
					Debug.Assert (this.noSync);
					Debug.Assert (this.journalMode == PAGER_JOURNALMODE_OFF || this.journalMode == PAGER_JOURNALMODE_MEMORY);
					Debug.Assert (this.eState != PAGER_ERROR && this.eState != PAGER_OPEN);
					Debug.Assert (this.pagerUseWal () == false);
				}
				///
///<summary>
///If changeCountDone is set, a RESERVED lock or greater must be held
///on the file.
///
///</summary>

				Debug.Assert (pPager.changeCountDone == false || pPager.eLock >= RESERVED_LOCK);
				Debug.Assert (this.eLock != PENDING_LOCK);
				switch (this.eState) {
				case PAGER_OPEN:
					Debug.Assert (
					#if SQLITE_OMIT_MEMORYDB
																																																																							0==MEMDB
#else
					0 == pPager.memDb
					#endif
					);
					Debug.Assert (pPager.errCode == SQLITE_OK);
					Debug.Assert (sqlite3PcacheRefCount (pPager.pPCache) == 0 || pPager.tempFile);
					break;
				case PAGER_READER:
					Debug.Assert (pPager.errCode == SQLITE_OK);
					Debug.Assert (this.eLock != UNKNOWN_LOCK);
					Debug.Assert (this.eLock >= SHARED_LOCK || this.noReadlock != 0);
					break;
				case PAGER_WRITER_LOCKED:
					Debug.Assert (this.eLock != UNKNOWN_LOCK);
					Debug.Assert (pPager.errCode == SQLITE_OK);
					if (!pPager.pagerUseWal ()) {
						Debug.Assert (this.eLock >= RESERVED_LOCK);
					}
					Debug.Assert (pPager.dbSize == pPager.dbOrigSize);
					Debug.Assert (pPager.dbOrigSize == pPager.dbFileSize);
					Debug.Assert (pPager.dbOrigSize == pPager.dbHintSize);
					Debug.Assert (pPager.setMaster == 0);
					break;
				case PAGER_WRITER_CACHEMOD:
					Debug.Assert (this.eLock != UNKNOWN_LOCK);
					Debug.Assert (pPager.errCode == SQLITE_OK);
					if (!pPager.pagerUseWal ()) {
						///
///<summary>
///It is possible that if journal_mode=wal here that neither the
///journal file nor the WAL file are open. This happens during
///a rollback transaction that switches from journal_mode=off
///to journal_mode=wal.
///
///</summary>

						Debug.Assert (this.eLock >= RESERVED_LOCK);
						Debug.Assert (isOpen (this.jfd) || this.journalMode == PAGER_JOURNALMODE_OFF || this.journalMode == PAGER_JOURNALMODE_WAL);
					}
					Debug.Assert (pPager.dbOrigSize == pPager.dbFileSize);
					Debug.Assert (pPager.dbOrigSize == pPager.dbHintSize);
					break;
				case PAGER_WRITER_DBMOD:
					Debug.Assert (this.eLock == EXCLUSIVE_LOCK);
					Debug.Assert (pPager.errCode == SQLITE_OK);
					Debug.Assert (!pPager.pagerUseWal ());
					Debug.Assert (this.eLock >= EXCLUSIVE_LOCK);
					Debug.Assert (isOpen (this.jfd) || this.journalMode == PAGER_JOURNALMODE_OFF || this.journalMode == PAGER_JOURNALMODE_WAL);
					Debug.Assert (pPager.dbOrigSize <= pPager.dbHintSize);
					break;
				case PAGER_WRITER_FINISHED:
					Debug.Assert (this.eLock == EXCLUSIVE_LOCK);
					Debug.Assert (pPager.errCode == SQLITE_OK);
					Debug.Assert (!pPager.pagerUseWal ());
					Debug.Assert (isOpen (this.jfd) || this.journalMode == PAGER_JOURNALMODE_OFF || this.journalMode == PAGER_JOURNALMODE_WAL);
					break;
				case PAGER_ERROR:
					///
///<summary>
///There must be at least one outstanding reference to the pager if
///in ERROR state. Otherwise the pager should have already dropped
///back to OPEN state.
///
///</summary>

					Debug.Assert (pPager.errCode != SQLITE_OK);
					Debug.Assert (sqlite3PcacheRefCount (pPager.pPCache) > 0);
					break;
				}
				return true;
			}

			public///<summary>
			/// Unlock the database file to level eLock, which must be either NO_LOCK
			/// or SHARED_LOCK. Regardless of whether or not the call to xUnlock()
			/// succeeds, set the Pager.eLock variable to match the (attempted) new lock.
			///
			/// Except, if Pager.eLock is set to UNKNOWN_LOCK when this function is
			/// called, do not modify it. See the comment above the #define of
			/// UNKNOWN_LOCK for an explanation of this.
			///
			///</summary>
			int pagerUnlockDb (int eLock)
			{
				int rc = SQLITE_OK;
				Debug.Assert (!this.exclusiveMode || this.eLock == eLock);
				Debug.Assert (eLock == NO_LOCK || eLock == SHARED_LOCK);
				Debug.Assert (eLock != NO_LOCK || this.pagerUseWal () == false);
				if (isOpen (this.fd)) {
					Debug.Assert (this.eLock >= eLock);
					rc = sqlite3OsUnlock (this.fd, eLock);
					if (this.eLock != UNKNOWN_LOCK) {
						this.eLock = (u8)eLock;
					}
					IOTRACE ("UNLOCK %p %d\n", this, eLock);
				}
				return rc;
			}

			public int pagerLockDb (int eLock)
			{
				int rc = SQLITE_OK;
				Debug.Assert (eLock == SHARED_LOCK || eLock == RESERVED_LOCK || eLock == EXCLUSIVE_LOCK);
				if (this.eLock < eLock || this.eLock == UNKNOWN_LOCK) {
					rc = sqlite3OsLock (this.fd, eLock);
					if (rc == SQLITE_OK && (this.eLock != UNKNOWN_LOCK || eLock == EXCLUSIVE_LOCK)) {
						this.eLock = (u8)eLock;
						IOTRACE ("LOCK %p %d\n", this, eLock);
					}
				}
				return rc;
			}

			public///<summary>
			/// Return the offset of the sector boundary at or immediately
			/// following the value in pPager.journalOff, assuming a sector
			/// size of pPager.sectorSize bytes.
			///
			/// i.e for a sector size of 512:
			///
			///   Pager.journalOff          Return value
			///   ---------------------------------------
			///   0                         0
			///   512                       512
			///   100                       512
			///   2000                      2048
			///
			///
			///</summary>
			i64 journalHdrOffset ()
			{
				i64 offset = 0;
				i64 c = this.journalOff;
				if (c != 0) {
					offset = (int)(((c - 1) / this.sectorSize + 1) * this.sectorSize);
					//offset = ((c-1)/JOURNAL_HDR_SZ(pPager) + 1) * JOURNAL_HDR_SZ(pPager);
				}
				Debug.Assert (offset % this.sectorSize == 0);
				//Debug.Assert(offset % JOURNAL_HDR_SZ(pPager) == 0);
				Debug.Assert (offset >= c);
				Debug.Assert ((offset - c) < this.sectorSize);
				//Debug.Assert( (offset-c)<JOURNAL_HDR_SZ(pPager) );
				return offset;
			}

			public void seekJournalHdr ()
			{
				this.journalOff = this.journalHdrOffset ();
			}

			public///<summary>
			/// The journal file must be open when this function is called.
			///
			/// This function is a no-op if the journal file has not been written to
			/// within the current transaction (i.e. if Pager.journalOff==0).
			///
			/// If doTruncate is non-zero or the Pager.journalSizeLimit variable is
			/// set to 0, then truncate the journal file to zero bytes in size. Otherwise,
			/// zero the 28-byte header at the start of the journal file. In either case,
			/// if the pager is not in no-sync mode, sync the journal file immediately
			/// after writing or truncating it.
			///
			/// If Pager.journalSizeLimit is set to a positive, non-zero value, and
			/// following the truncation or zeroing described above the size of the
			/// journal file in bytes is larger than this value, then truncate the
			/// journal file to Pager.journalSizeLimit bytes. The journal file does
			/// not need to be synced following this operation.
			///
			/// If an IO error occurs, abandon processing and return the IO error code.
			/// Otherwise, return SQLITE_OK.
			///
			///</summary>
			int zeroJournalHdr (int doTruncate)
			{
				int rc = SQLITE_OK;
				///
///<summary>
///Return code 
///</summary>

				Debug.Assert (isOpen (this.jfd));
				if (this.journalOff != 0) {
					i64 iLimit = this.journalSizeLimit;
					///
///<summary>
///Local cache of jsl 
///</summary>

					IOTRACE ("JZEROHDR %p\n", this);
					if (doTruncate != 0 || iLimit == 0) {
						rc = sqlite3OsTruncate (this.jfd, 0);
					}
					else {
						byte[] zeroHdr = new byte[28];
						// = {0};
						rc = sqlite3OsWrite (this.jfd, zeroHdr, zeroHdr.Length, 0);
					}
					if (rc == SQLITE_OK && !this.noSync) {
						rc = sqlite3OsSync (this.jfd, SQLITE_SYNC_DATAONLY | this.syncFlags);
					}
					///
///<summary>
///At this point the transaction is committed but the write lock
///is still held on the file. If there is a size limit configured for
///the persistent journal and the journal file currently consumes more
///space than that limit allows for, truncate it now. There is no need
///to sync the file following this operation.
///
///</summary>

					if (rc == SQLITE_OK && iLimit > 0) {
						i64 sz = 0;
						rc = sqlite3OsFileSize (this.jfd, ref sz);
						if (rc == SQLITE_OK && sz > iLimit) {
							rc = sqlite3OsTruncate (this.jfd, iLimit);
						}
					}
				}
				return rc;
			}

			public///<summary>
			/// The journal file must be open when this routine is called. A journal
			/// header (JOURNAL_HDR_SZ bytes) is written into the journal file at the
			/// current location.
			///
			/// The format for the journal header is as follows:
			/// - 8 bytes: Magic identifying journal format.
			/// - 4 bytes: Number of records in journal, or -1 no-sync mode is on.
			/// - 4 bytes: Random number used for page hash.
			/// - 4 bytes: Initial database page count.
			/// - 4 bytes: Sector size used by the process that wrote this journal.
			/// - 4 bytes: Database page size.
			///
			/// Followed by (JOURNAL_HDR_SZ - 28) bytes of unused space.
			///
			///</summary>
			int writeJournalHdr ()
			{
				int rc = SQLITE_OK;
				///
///<summary>
///Return code 
///</summary>

				byte[] zHeader = this.pTmpSpace;
				///
///<summary>
///Temporary space used to build header 
///</summary>

				u32 nHeader = (u32)this.pageSize;
				///
///<summary>
///Size of buffer pointed to by zHeader 
///</summary>

				u32 nWrite;
				///
///<summary>
///Bytes of header sector written 
///</summary>

				int ii;
				///
///<summary>
///Loop counter 
///</summary>

				Debug.Assert (isOpen (this.jfd));
				///
///<summary>
///Journal file must be open. 
///</summary>

				if (nHeader > this.JOURNAL_HDR_SZ ()) {
					nHeader = this.JOURNAL_HDR_SZ ();
				}
				///
///<summary>
///If there are active savepoints and any of them were created
///since the most recent journal header was written, update the
///PagerSavepoint.iHdrOffset fields now.
///
///</summary>

				for (ii = 0; ii < this.nSavepoint; ii++) {
					if (this.aSavepoint [ii].iHdrOffset == 0) {
						this.aSavepoint [ii].iHdrOffset = this.journalOff;
					}
				}
				this.journalHdr = this.journalOff = this.journalHdrOffset ();
				///
///<summary>
///</summary>
///<param name="Write the nRec Field "> the number of page records that follow this</param>
///<param name="journal header. Normally, zero is written to this value at this time.">journal header. Normally, zero is written to this value at this time.</param>
///<param name="After the records are added to the journal (and the journal synced,">After the records are added to the journal (and the journal synced,</param>
///<param name="if in full">sync mode), the zero is overwritten with the true number</param>
///<param name="of records (see syncJournal()).">of records (see syncJournal()).</param>
///<param name=""></param>
///<param name="A faster alternative is to write 0xFFFFFFFF to the nRec field. When">A faster alternative is to write 0xFFFFFFFF to the nRec field. When</param>
///<param name="reading the journal this value tells SQLite to assume that the">reading the journal this value tells SQLite to assume that the</param>
///<param name="rest of the journal file contains valid page records. This assumption">rest of the journal file contains valid page records. This assumption</param>
///<param name="is dangerous, as if a failure occurred whilst writing to the journal">is dangerous, as if a failure occurred whilst writing to the journal</param>
///<param name="file it may contain some garbage data. There are two scenarios">file it may contain some garbage data. There are two scenarios</param>
///<param name="where this risk can be ignored:">where this risk can be ignored:</param>
///<param name=""></param>
///<param name="When the pager is in no">sync mode. Corruption can follow a</param>
///<param name="power failure in this case anyway.">power failure in this case anyway.</param>
///<param name=""></param>
///<param name="When the SQLITE_IOCAP_SAFE_APPEND flag is set. This guarantees">When the SQLITE_IOCAP_SAFE_APPEND flag is set. This guarantees</param>
///<param name="that garbage data is never appended to the journal file.">that garbage data is never appended to the journal file.</param>
///<param name=""></param>

				Debug.Assert (isOpen (this.fd) || this.noSync);
				if (this.noSync || (this.journalMode == PAGER_JOURNALMODE_MEMORY) || (sqlite3OsDeviceCharacteristics (this.fd) & SQLITE_IOCAP_SAFE_APPEND) != 0) {
					aJournalMagic.CopyTo (zHeader, 0);
					// memcpy(zHeader, aJournalMagic, sizeof(aJournalMagic));
					put32bits (zHeader, aJournalMagic.Length, 0xffffffff);
				}
				else {
					Array.Clear (zHeader, 0, aJournalMagic.Length + 4);
					//memset(zHeader, 0, sizeof(aJournalMagic)+4);
				}
				///
///<summary>
///</summary>
///<param name="The random check">hash initialiser </param>

				i64 i64Temp = 0;
				sqlite3_randomness (sizeof(i64), ref i64Temp);
				this.cksumInit = (u32)i64Temp;
				put32bits (zHeader, aJournalMagic.Length + 4, this.cksumInit);
				///
///<summary>
///The initial database size 
///</summary>

				put32bits (zHeader, aJournalMagic.Length + 8, this.dbOrigSize);
				///
///<summary>
///The assumed sector size for this process 
///</summary>

				put32bits (zHeader, aJournalMagic.Length + 12, this.sectorSize);
				///
///<summary>
///The page size 
///</summary>

				put32bits (zHeader, aJournalMagic.Length + 16, (u32)this.pageSize);
				///
///<summary>
///Initializing the tail of the buffer is not necessary.  Everything
///works find if the following memset() is omitted.  But initializing
///the memory prevents valgrind from complaining, so we are willing to
///take the performance hit.
///
///</summary>

				//  memset(&zHeader[sizeof(aJournalMagic)+20], 0,
				//  nHeader-(sizeof(aJournalMagic)+20));
				Array.Clear (zHeader, aJournalMagic.Length + 20, (int)nHeader - (aJournalMagic.Length + 20));
				///
///<summary>
///In theory, it is only necessary to write the 28 bytes that the
///journal header consumes to the journal file here. Then increment the
///Pager.journalOff variable by JOURNAL_HDR_SZ so that the next
///record is written to the following sector (leaving a gap in the file
///that will be implicitly filled in by the OS).
///
///However it has been discovered that on some systems this pattern can
///be significantly slower than contiguously writing data to the file,
///even if that means explicitly writing data to the block of
///</summary>
///<param name="(JOURNAL_HDR_SZ "> 28) bytes that will not be used. So that is what</param>
///<param name="is done.">is done.</param>
///<param name=""></param>
///<param name="The loop is required here in case the sector">size is larger than the</param>
///<param name="database page size. Since the zHeader buffer is only Pager.pageSize">database page size. Since the zHeader buffer is only Pager.pageSize</param>
///<param name="bytes in size, more than one call to sqlite3OsWrite() may be required">bytes in size, more than one call to sqlite3OsWrite() may be required</param>
///<param name="to populate the entire journal header sector.">to populate the entire journal header sector.</param>
///<param name=""></param>

				for (nWrite = 0; rc == SQLITE_OK && nWrite < this.JOURNAL_HDR_SZ (); nWrite += nHeader) {
					IOTRACE ("JHDR %p %lld %d\n", this, this.journalHdr, nHeader);
					rc = sqlite3OsWrite (this.jfd, zHeader, (int)nHeader, this.journalOff);
					Debug.Assert (this.journalHdr <= this.journalOff);
					this.journalOff += (int)nHeader;
				}
				return rc;
			}

			public///<summary>
			/// The journal file must be open when this is called. A journal header file
			/// (JOURNAL_HDR_SZ bytes) is read from the current location in the journal
			/// file. The current location in the journal file is given by
			/// pPager.journalOff. See comments above function writeJournalHdr() for
			/// a description of the journal header format.
			///
			/// If the header is read successfully, *pNRec is set to the number of
			/// page records following this header and *pDbSize is set to the size of the
			/// database before the transaction began, in pages. Also, pPager.cksumInit
			/// is set to the value read from the journal header. SQLITE_OK is returned
			/// in this case.
			///
			/// If the journal header file appears to be corrupted, SQLITE_DONE is
			/// returned and *pNRec and *PDbSize are undefined.  If JOURNAL_HDR_SZ bytes
			/// cannot be read from the journal file an error code is returned.
			///
			///</summary>
			int readJournalHdr (///
///<summary>
///Pager object 
///</summary>

			int isHot, i64 journalSize, ///
///<summary>
///Size of the open journal file in bytes 
///</summary>

			out u32 pNRec, ///
///<summary>
///OUT: Value read from the nRec field 
///</summary>

			out u32 pDbSize///
///<summary>
///OUT: Value of original database size field 
///</summary>

			)
			{
				int rc;
				///
///<summary>
///Return code 
///</summary>

				byte[] aMagic = new byte[8];
				///
///<summary>
///A buffer to hold the magic header 
///</summary>

				i64 iHdrOff;
				///
///<summary>
///Offset of journal header being read 
///</summary>

				Debug.Assert (isOpen (this.jfd));
				///
///<summary>
///Journal file must be open. 
///</summary>

				pNRec = 0;
				pDbSize = 0;
				///
///<summary>
///Advance Pager.journalOff to the start of the next sector. If the
///journal file is too small for there to be a header stored at this
///point, return SQLITE_DONE.
///
///</summary>

				this.journalOff = this.journalHdrOffset ();
				if (this.journalOff + this.JOURNAL_HDR_SZ () > journalSize) {
					return SQLITE_DONE;
				}
				iHdrOff = this.journalOff;
				///
///<summary>
///Read in the first 8 bytes of the journal header. If they do not match
///the  magic string found at the start of each journal header, return
///SQLITE_DONE. If an IO error occurs, return an error code. Otherwise,
///proceed.
///
///</summary>

				if (isHot != 0 || iHdrOff != this.journalHdr) {
					rc = sqlite3OsRead (this.jfd, aMagic, aMagic.Length, iHdrOff);
					if (rc != 0) {
						return rc;
					}
					if (memcmp (aMagic, aJournalMagic, aMagic.Length) != 0) {
						return SQLITE_DONE;
					}
				}
				///
///<summary>
///</summary>
///<param name="Read the first three 32">bit fields of the journal header: The nRec</param>
///<param name="field, the checksum">initializer and the database size at the start</param>
///<param name="of the transaction. Return an error code if anything goes wrong.">of the transaction. Return an error code if anything goes wrong.</param>
///<param name=""></param>

				if (SQLITE_OK != (rc = read32bits (this.jfd, iHdrOff + 8, ref pNRec)) || SQLITE_OK != (rc = read32bits (this.jfd, iHdrOff + 12, ref this.cksumInit)) || SQLITE_OK != (rc = read32bits (this.jfd, iHdrOff + 16, ref pDbSize))) {
					return rc;
				}
				if (this.journalOff == 0) {
					u32 iPageSize = 0;
					///
///<summary>
///</summary>
///<param name="Page">size field of journal header </param>

					u32 iSectorSize = 0;
					///
///<summary>
///</summary>
///<param name="Sector">size field of journal header </param>

					///
///<summary>
///</summary>
///<param name="Read the page">size journal header fields. </param>

					if (SQLITE_OK != (rc = read32bits (this.jfd, iHdrOff + 20, ref iSectorSize)) || SQLITE_OK != (rc = read32bits (this.jfd, iHdrOff + 24, ref iPageSize))) {
						return rc;
					}
					///
///<summary>
///</summary>
///<param name="Versions of SQLite prior to 3.5.8 set the page">size field of the</param>
///<param name="journal header to zero. In this case, assume that the Pager.pageSize">journal header to zero. In this case, assume that the Pager.pageSize</param>
///<param name="variable is already set to the correct page size.">variable is already set to the correct page size.</param>
///<param name=""></param>

					if (iPageSize == 0) {
						iPageSize = (u32)this.pageSize;
					}
					///
///<summary>
///</summary>
///<param name="Check that the values read from the page">size fields</param>
///<param name="are within range. To be 'in range', both values need to be a power">are within range. To be 'in range', both values need to be a power</param>
///<param name="of two greater than or equal to 512 or 32, and not greater than their ">of two greater than or equal to 512 or 32, and not greater than their </param>
///<param name="respective compile time maximum limits.">respective compile time maximum limits.</param>
///<param name=""></param>

					if (iPageSize < 512 || iSectorSize < 32 || iPageSize > SQLITE_MAX_PAGE_SIZE || iSectorSize > MAX_SECTOR_SIZE || ((iPageSize - 1) & iPageSize) != 0 || ((iSectorSize - 1) & iSectorSize) != 0) {
						///
///<summary>
///</summary>
///<param name="If the either the page">header is</param>
///<param name="invalid, then the process that wrote the journal">header must have</param>
///<param name="crashed before the header was synced. In this case stop reading">crashed before the header was synced. In this case stop reading</param>
///<param name="the journal file here.">the journal file here.</param>
///<param name=""></param>

						return SQLITE_DONE;
					}
					///
///<summary>
///</summary>
///<param name="Update the page">size to match the value read from the journal.</param>
///<param name="Use a testcase() macro to make sure that malloc failure within">Use a testcase() macro to make sure that malloc failure within</param>
///<param name="PagerSetPagesize() is tested.">PagerSetPagesize() is tested.</param>
///<param name=""></param>

					rc = this.sqlite3PagerSetPagesize (ref iPageSize, -1);
					testcase (rc != SQLITE_OK);
					///
///<summary>
///</summary>
///<param name="Update the assumed sector">size to match the value used by</param>
///<param name="the process that created this journal. If this journal was">the process that created this journal. If this journal was</param>
///<param name="created by a process other than this one, then this routine">created by a process other than this one, then this routine</param>
///<param name="is being called from within pager_playback(). The local value">is being called from within pager_playback(). The local value</param>
///<param name="of Pager.sectorSize is restored at the end of that routine.">of Pager.sectorSize is restored at the end of that routine.</param>
///<param name=""></param>

					this.sectorSize = iSectorSize;
				}
				this.journalOff += (int)this.JOURNAL_HDR_SZ ();
				return rc;
			}

			public///<summary>
			/// Write the supplied master journal name into the journal file for pager
			/// pPager at the current location. The master journal name must be the last
			/// thing written to a journal file. If the pager is in full-sync mode, the
			/// journal file descriptor is advanced to the next sector boundary before
			/// anything is written. The format is:
			///
			///   + 4 bytes: PAGER_MJ_PGNO.
			///   + N bytes: Master journal filename in utf-8.
			///   + 4 bytes: N (length of master journal name in bytes, no nul-terminator).
			///   + 4 bytes: Master journal name checksum.
			///   + 8 bytes: aJournalMagic[].
			///
			/// The master journal page checksum is the sum of the bytes in the master
			/// journal name, where each byte is interpreted as a signed 8-bit integer.
			///
			/// If zMaster is a NULL pointer (occurs for a single database transaction),
			/// this call is a no-op.
			///
			///</summary>
			int writeMasterJournal (string zMaster)
			{
				int rc;
				///
///<summary>
///Return code 
///</summary>

				int nMaster;
				///
///<summary>
///Length of string zMaster 
///</summary>

				i64 iHdrOff;
				///
///<summary>
///Offset of header in journal file 
///</summary>

				i64 jrnlSize = 0;
				///
///<summary>
///Size of journal file on disk 
///</summary>

				u32 cksum = 0;
				///
///<summary>
///Checksum of string zMaster 
///</summary>

				Debug.Assert (this.setMaster == 0);
				Debug.Assert (!this.pagerUseWal ());
				if (null == zMaster || this.journalMode == PAGER_JOURNALMODE_MEMORY || this.journalMode == PAGER_JOURNALMODE_OFF) {
					return SQLITE_OK;
				}
				this.setMaster = 1;
				Debug.Assert (isOpen (this.jfd));
				Debug.Assert (this.journalHdr <= this.journalOff);
				///
///<summary>
///Calculate the length in bytes and the checksum of zMaster 
///</summary>

				for (nMaster = 0; nMaster < zMaster.Length && zMaster [nMaster] != 0; nMaster++) {
					cksum += zMaster [nMaster];
				}
				///
///<summary>
///</summary>
///<param name="If in full">sync mode, advance to the next disk sector before writing</param>
///<param name="the master journal name. This is in case the previous page written to">the master journal name. This is in case the previous page written to</param>
///<param name="the journal has already been synced.">the journal has already been synced.</param>
///<param name=""></param>

				if (this.fullSync) {
					this.journalOff = this.journalHdrOffset ();
				}
				iHdrOff = this.journalOff;
				///
///<summary>
///Write the master journal data to the end of the journal file. If
///an error occurs, return the error code to the caller.
///
///</summary>

				if ((0 != (rc = write32bits (this.jfd, iHdrOff, (u32)PAGER_MJ_PGNO (this)))) || (0 != (rc = sqlite3OsWrite (this.jfd, Encoding.UTF8.GetBytes (zMaster), nMaster, iHdrOff + 4))) || (0 != (rc = write32bits (this.jfd, iHdrOff + 4 + nMaster, (u32)nMaster))) || (0 != (rc = write32bits (this.jfd, iHdrOff + 4 + nMaster + 4, cksum))) || (0 != (rc = sqlite3OsWrite (this.jfd, aJournalMagic, 8, iHdrOff + 4 + nMaster + 8)))) {
					return rc;
				}
				this.journalOff += (nMaster + 20);
				///
///<summary>
///</summary>
///<param name="If the pager is in peristent">journal mode, then the physical</param>
///<param name="journal">journal name</param>
///<param name="and 8 bytes of magic data just written to the file. This is">and 8 bytes of magic data just written to the file. This is</param>
///<param name="dangerous because the code to rollback a hot">journal file</param>
///<param name="will not be able to find the master">journal name to determine</param>
///<param name="whether or not the journal is hot.">whether or not the journal is hot.</param>
///<param name=""></param>
///<param name="Easiest thing to do in this scenario is to truncate the journal">Easiest thing to do in this scenario is to truncate the journal</param>
///<param name="file to the required size.">file to the required size.</param>
///<param name=""></param>

				if (SQLITE_OK == (rc = sqlite3OsFileSize (this.jfd, ref jrnlSize)) && jrnlSize > this.journalOff) {
					rc = sqlite3OsTruncate (this.jfd, this.journalOff);
				}
				return rc;
			}

			public///<summary>
			/// Find a page in the hash table given its page number. Return
			/// a pointer to the page or NULL if the requested page is not
			/// already in memory.
			///
			///</summary>
			PgHdr pager_lookup (u32 pgno)
			{
				PgHdr p = null;
				///
///<summary>
///Return value 
///</summary>

				///
///<summary>
///It is not possible for a call to PcacheFetch() with createFlag==0 to
///fail, since no attempt to allocate dynamic memory will be made.
///
///</summary>

				sqlite3PcacheFetch (this.pPCache, pgno, 0, ref p);
				return p;
			}

			public///<summary>
			/// Discard the entire contents of the in-memory page-cache.
			///
			///</summary>
			void pager_reset ()
			{
				if (null != this.pBackup)
					this.pBackup.sqlite3BackupRestart ();
				sqlite3PcacheClear (this.pPCache);
			}

			public///<summary>
			/// Set the bit number pgno in the PagerSavepoint.pInSavepoint
			/// bitvecs of all open savepoints. Return SQLITE_OK if successful
			/// or SQLITE_NOMEM if a malloc failure occurs.
			///
			///</summary>
			int addToSavepointBitvecs (u32 pgno)
			{
				int ii;
				///
///<summary>
///Loop counter 
///</summary>

				int rc = SQLITE_OK;
				///
///<summary>
///Result code 
///</summary>

				for (ii = 0; ii < this.nSavepoint; ii++) {
					PagerSavepoint p = this.aSavepoint [ii];
					if (pgno <= p.nOrig) {
						rc |= sqlite3BitvecSet (p.pInSavepoint, pgno);
						testcase (rc == SQLITE_NOMEM);
						Debug.Assert (rc == SQLITE_OK || rc == SQLITE_NOMEM);
					}
				}
				return rc;
			}

			public///<summary>
			/// Free all structures in the Pager.aSavepoint[] array and set both
			/// Pager.aSavepoint and Pager.nSavepoint to zero. Close the sub-journal
			/// if it is open and the pager is not in exclusive mode.
			///
			///</summary>
			void releaseAllSavepoints ()
			{
				int ii;
				///
///<summary>
///Iterator for looping through Pager.aSavepoint 
///</summary>

				for (ii = 0; ii < this.nSavepoint; ii++) {
					sqlite3BitvecDestroy (ref this.aSavepoint [ii].pInSavepoint);
				}
				if (!this.exclusiveMode || sqlite3IsMemJournal (this.sjfd)) {
					sqlite3OsClose (this.sjfd);
				}
				//sqlite3_free( ref pPager.aSavepoint );
				this.aSavepoint = null;
				this.nSavepoint = 0;
				this.nSubRec = 0;
			}

			public///<summary>
			/// This function is a no-op if the pager is in exclusive mode and not
			/// in the ERROR state. Otherwise, it switches the pager to PAGER_OPEN
			/// state.
			///
			/// If the pager is not in exclusive-access mode, the database file is
			/// completely unlocked. If the file is unlocked and the file-system does
			/// not exhibit the UNDELETABLE_WHEN_OPEN property, the journal file is
			/// closed (if it is open).
			///
			/// If the pager is in ERROR state when this function is called, the
			/// contents of the pager cache are discarded before switching back to
			/// the OPEN state. Regardless of whether the pager is in exclusive-mode
			/// or not, any journal file left in the file-system will be treated
			/// as a hot-journal and rolled back the next time a read-transaction
			/// is opened (by this or by any other connection).
			///
			///</summary>
			void pager_unlock ()
			{
				Debug.Assert (this.eState == PAGER_READER || this.eState == PAGER_OPEN || this.eState == PAGER_ERROR);
				sqlite3BitvecDestroy (ref this.pInJournal);
				this.pInJournal = null;
				this.releaseAllSavepoints ();
				if (this.pagerUseWal ()) {
					Debug.Assert (!isOpen (this.jfd));
					sqlite3WalEndReadTransaction (this.pWal);
					this.eState = PAGER_OPEN;
				}
				else
					if (!this.exclusiveMode) {
						int rc;
						///
///<summary>
///Error code returned by pagerUnlockDb() 
///</summary>

						int iDc = isOpen (this.fd) ? sqlite3OsDeviceCharacteristics (this.fd) : 0;
						///
///<summary>
///If the operating system support deletion of open files, then
///close the journal file when dropping the database lock.  Otherwise
///another connection with journal_mode=delete might delete the file
///out from under us.
///
///</summary>

						Debug.Assert ((PAGER_JOURNALMODE_MEMORY & 5) != 1);
						Debug.Assert ((PAGER_JOURNALMODE_OFF & 5) != 1);
						Debug.Assert ((PAGER_JOURNALMODE_WAL & 5) != 1);
						Debug.Assert ((PAGER_JOURNALMODE_DELETE & 5) != 1);
						Debug.Assert ((PAGER_JOURNALMODE_TRUNCATE & 5) == 1);
						Debug.Assert ((PAGER_JOURNALMODE_PERSIST & 5) == 1);
						if (0 == (iDc & SQLITE_IOCAP_UNDELETABLE_WHEN_OPEN) || 1 != (this.journalMode & 5)) {
							sqlite3OsClose (this.jfd);
						}
						///
///<summary>
///If the pager is in the ERROR state and the call to unlock the database
///file fails, set the current lock to UNKNOWN_LOCK. See the comment
///above the #define for UNKNOWN_LOCK for an explanation of why this
///is necessary.
///
///</summary>

						rc = this.pagerUnlockDb (NO_LOCK);
						if (rc != SQLITE_OK && this.eState == PAGER_ERROR) {
							this.eLock = UNKNOWN_LOCK;
						}
						///
///<summary>
///The pager state may be changed from PAGER_ERROR to PAGER_OPEN here
///</summary>
///<param name="without clearing the error code. This is intentional "> the error</param>
///<param name="code is cleared and the cache reset in the block below.">code is cleared and the cache reset in the block below.</param>
///<param name=""></param>

						Debug.Assert (this.errCode != 0 || this.eState != PAGER_ERROR);
						this.changeCountDone = false;
						this.eState = PAGER_OPEN;
					}
				///
///<summary>
///If Pager.errCode is set, the contents of the pager cache cannot be
///trusted. Now that there are no outstanding references to the pager,
///it can safely move back to PAGER_OPEN state. This happens in both
///</summary>
///<param name="normal and exclusive">locking mode.</param>
///<param name=""></param>

				if (this.errCode != 0) {
					Debug.Assert (
					#if SQLITE_OMIT_MEMORYDB
																																																																																									0==MEMDB
#else
					0 == this.memDb
					#endif
					);
					this.pager_reset ();
					this.changeCountDone = this.tempFile;
					this.eState = PAGER_OPEN;
					this.errCode = SQLITE_OK;
				}
				this.journalOff = 0;
				this.journalHdr = 0;
				this.setMaster = 0;
			}

			public///<summary>
			/// This function is called whenever an IOERR or FULL error that requires
			/// the pager to transition into the ERROR state may ahve occurred.
			/// The first argument is a pointer to the pager structure, the second
			/// the error-code about to be returned by a pager API function. The
			/// value returned is a copy of the second argument to this function.
			///
			/// If the second argument is SQLITE_FULL, SQLITE_IOERR or one of the
			/// IOERR sub-codes, the pager enters the ERROR state and the error code
			/// is stored in Pager.errCode. While the pager remains in the ERROR state,
			/// all major API calls on the Pager will immediately return Pager.errCode.
			///
			/// The ERROR state indicates that the contents of the pager-cache
			/// cannot be trusted. This state can be cleared by completely discarding
			/// the contents of the pager-cache. If a transaction was active when
			/// the persistent error occurred, then the rollback journal may need
			/// to be replayed to restore the contents of the database file (as if
			/// it were a hot-journal).
			///
			///</summary>
			int pager_error (int rc)
			{
				int rc2 = rc & 0xff;
				Debug.Assert (rc == SQLITE_OK || 
				#if SQLITE_OMIT_MEMORYDB
																																																																						0==MEMDB
#else
				0 == this.memDb
				#endif
				);
				Debug.Assert (this.errCode == SQLITE_FULL || this.errCode == SQLITE_OK || (this.errCode & 0xff) == SQLITE_IOERR);
				if (rc2 == SQLITE_FULL || rc2 == SQLITE_IOERR) {
					this.errCode = rc;
					this.eState = PAGER_ERROR;
				}
				return rc;
			}

			public///<summary>
			/// This routine ends a transaction. A transaction is usually ended by
			/// either a COMMIT or a ROLLBACK operation. This routine may be called
			/// after rollback of a hot-journal, or if an error occurs while opening
			/// the journal file or writing the very first journal-header of a
			/// database transaction.
			///
			/// This routine is never called in PAGER_ERROR state. If it is called
			/// in PAGER_NONE or PAGER_SHARED state and the lock held is less
			/// exclusive than a RESERVED lock, it is a no-op.
			///
			/// Otherwise, any active savepoints are released.
			///
			/// If the journal file is open, then it is "finalized". Once a journal
			/// file has been finalized it is not possible to use it to roll back a
			/// transaction. Nor will it be considered to be a hot-journal by this
			/// or any other database connection. Exactly how a journal is finalized
			/// depends on whether or not the pager is running in exclusive mode and
			/// the current journal-mode (Pager.journalMode value), as follows:
			///
			///   journalMode==MEMORY
			///     Journal file descriptor is simply closed. This destroys an
			///     in-memory journal.
			///
			///   journalMode==TRUNCATE
			///     Journal file is truncated to zero bytes in size.
			///
			///   journalMode==PERSIST
			///     The first 28 bytes of the journal file are zeroed. This invalidates
			///     the first journal header in the file, and hence the entire journal
			///     file. An invalid journal file cannot be rolled back.
			///
			///   journalMode==DELETE
			///     The journal file is closed and deleted using sqlite3OsDelete().
			///
			///     If the pager is running in exclusive mode, this method of finalizing
			///     the journal file is never used. Instead, if the journalMode is
			///     DELETE and the pager is in exclusive mode, the method described under
			///     journalMode==PERSIST is used instead.
			///
			/// After the journal is finalized, the pager moves to PAGER_READER state.
			/// If running in non-exclusive rollback mode, the lock on the file is
			/// downgraded to a SHARED_LOCK.
			///
			/// SQLITE_OK is returned if no error occurs. If an error occurs during
			/// any of the IO operations to finalize the journal file or unlock the
			/// database then the IO error code is returned to the user. If the
			/// operation to finalize the journal file fails, then the code still
			/// tries to unlock the database file if not in exclusive mode. If the
			/// unlock operation fails as well, then the first error code related
			/// to the first error encountered (the journal finalization one) is
			/// returned.
			///
			///</summary>
			int pager_end_transaction (int hasMaster)
			{
				int rc = SQLITE_OK;
				///
///<summary>
///Error code from journal finalization operation 
///</summary>

				int rc2 = SQLITE_OK;
				///
///<summary>
///Error code from db file unlock operation 
///</summary>

				///
///<summary>
///Do nothing if the pager does not have an open write transaction
///or at least a RESERVED lock. This function may be called when there
///</summary>
///<param name="is no write">transaction active but a RESERVED or greater lock is</param>
///<param name="held under two circumstances:">held under two circumstances:</param>
///<param name=""></param>
///<param name="1. After a successful hot">journal rollback, it is called with</param>
///<param name="eState==PAGER_NONE and eLock==EXCLUSIVE_LOCK.">eState==PAGER_NONE and eLock==EXCLUSIVE_LOCK.</param>
///<param name=""></param>
///<param name="2. If a connection with locking_mode=exclusive holding an EXCLUSIVE ">2. If a connection with locking_mode=exclusive holding an EXCLUSIVE </param>
///<param name="lock switches back to locking_mode=normal and then executes a">lock switches back to locking_mode=normal and then executes a</param>
///<param name="read">transaction, this function is called with eState==PAGER_READER </param>
///<param name="and eLock==EXCLUSIVE_LOCK when the read">transaction is closed.</param>
///<param name=""></param>

				Debug.Assert (this.assert_pager_state ());
				Debug.Assert (this.eState != PAGER_ERROR);
				if (this.eState < PAGER_WRITER_LOCKED && this.eLock < RESERVED_LOCK) {
					return SQLITE_OK;
				}
				this.releaseAllSavepoints ();
				Debug.Assert (isOpen (this.jfd) || this.pInJournal == null);
				if (isOpen (this.jfd)) {
					Debug.Assert (!this.pagerUseWal ());
					///
///<summary>
///Finalize the journal file. 
///</summary>

					if (sqlite3IsMemJournal (this.jfd)) {
						Debug.Assert (this.journalMode == PAGER_JOURNALMODE_MEMORY);
						sqlite3OsClose (this.jfd);
					}
					else
						if (this.journalMode == PAGER_JOURNALMODE_TRUNCATE) {
							if (this.journalOff == 0) {
								rc = SQLITE_OK;
							}
							else {
								rc = sqlite3OsTruncate (this.jfd, 0);
							}
							this.journalOff = 0;
						}
						else
							if (this.journalMode == PAGER_JOURNALMODE_PERSIST || (this.exclusiveMode && this.journalMode != PAGER_JOURNALMODE_WAL)) {
								rc = this.zeroJournalHdr (hasMaster);
								this.journalOff = 0;
							}
							else {
								///
///<summary>
///This branch may be executed with Pager.journalMode==MEMORY if
///</summary>
///<param name="a hot">journal was just rolled back. In this case the journal</param>
///<param name="file should be closed and deleted. If this connection writes to">file should be closed and deleted. If this connection writes to</param>
///<param name="the database file, it will do so using an in">memory journal. </param>
///<param name=""></param>

								Debug.Assert (this.journalMode == PAGER_JOURNALMODE_DELETE || this.journalMode == PAGER_JOURNALMODE_MEMORY || this.journalMode == PAGER_JOURNALMODE_WAL);
								sqlite3OsClose (this.jfd);
								if (!this.tempFile) {
									rc = sqlite3OsDelete (this.pVfs, this.zJournal, 0);
								}
							}
				}
				#if SQLITE_CHECK_PAGES
																																																																						sqlite3PcacheIterateDirty(pPager.pPCache, pager_set_pagehash);
if( pPager.dbSize==0 && sqlite3PcacheRefCount(pPager.pPCache)>0 ){
PgHdr p = pager_lookup(pPager, 1);
if( p != null ){
p.pageHash = null;
sqlite3PagerUnref(p);
}
}
#endif
				sqlite3BitvecDestroy (ref this.pInJournal);
				this.pInJournal = null;
				this.nRec = 0;
				sqlite3PcacheCleanAll (this.pPCache);
				sqlite3PcacheTruncate (this.pPCache, this.dbSize);
				if (this.pagerUseWal ()) {
					///
///<summary>
///</summary>
///<param name="Drop the WAL write">lock, if any. Also, if the connection was in </param>
///<param name="locking_mode=exclusive mode but is no longer, drop the EXCLUSIVE ">locking_mode=exclusive mode but is no longer, drop the EXCLUSIVE </param>
///<param name="lock held on the database file.">lock held on the database file.</param>
///<param name=""></param>

					rc2 = sqlite3WalEndWriteTransaction (this.pWal);
					Debug.Assert (rc2 == SQLITE_OK);
				}
				if (!this.exclusiveMode && (!this.pagerUseWal () || sqlite3WalExclusiveMode (this.pWal, 0))) {
					rc2 = this.pagerUnlockDb (SHARED_LOCK);
					this.changeCountDone = false;
				}
				this.eState = PAGER_READER;
				this.setMaster = 0;
				return (rc == SQLITE_OK ? rc2 : rc);
			}

			public///<summary>
			/// Execute a rollback if a transaction is active and unlock the
			/// database file.
			///
			/// If the pager has already entered the ERROR state, do not attempt
			/// the rollback at this time. Instead, pager_unlock() is called. The
			/// call to pager_unlock() will discard all in-memory pages, unlock
			/// the database file and move the pager back to OPEN state. If this
			/// means that there is a hot-journal left in the file-system, the next
			/// connection to obtain a shared lock on the pager (which may be this one)
			/// will roll it back.
			///
			/// If the pager has not already entered the ERROR state, but an IO or
			/// malloc error occurs during a rollback, then this will itself cause
			/// the pager to enter the ERROR state. Which will be cleared by the
			/// call to pager_unlock(), as described above.
			///
			///</summary>
			void pagerUnlockAndRollback ()
			{
				if (this.eState != PAGER_ERROR && this.eState != PAGER_OPEN) {
					Debug.Assert (this.assert_pager_state ());
					if (this.eState >= PAGER_WRITER_LOCKED) {
						sqlite3BeginBenignMalloc ();
						this.sqlite3PagerRollback ();
						sqlite3EndBenignMalloc ();
					}
					else
						if (!this.exclusiveMode) {
							Debug.Assert (this.eState == PAGER_READER);
							this.pager_end_transaction (0);
						}
				}
				this.pager_unlock ();
			}

			public u32 pager_cksum (byte[] aData)
			{
				u32 cksum = this.cksumInit;
				///
///<summary>
///Checksum value to return 
///</summary>

				int i = this.pageSize - 200;
				///
///<summary>
///Loop counter 
///</summary>

				while (i > 0) {
					cksum += aData [i];
					i -= 200;
				}
				return cksum;
			}

			public///<summary>
			/// Report the current page size and number of reserved bytes back
			/// to the codec.
			///
			///</summary>
			#if SQLITE_HAS_CODEC
			void pagerReportSize ()
			{
				if (this.xCodecSizeChng != null) {
					this.xCodecSizeChng (this.pCodec, this.pageSize, this.nReserve);
				}
			}

			public///<summary>
			/// Read a single page from either the journal file (if isMainJrnl==1) or
			/// from the sub-journal (if isMainJrnl==0) and playback that page.
			/// The page begins at offset *pOffset into the file. The *pOffset
			/// value is increased to the start of the next page in the journal.
			///
			/// The main rollback journal uses checksums - the statement journal does
			/// not.
			///
			/// If the page number of the page record read from the (sub-)journal file
			/// is greater than the current value of Pager.dbSize, then playback is
			/// skipped and SQLITE_OK is returned.
			///
			/// If pDone is not NULL, then it is a record of pages that have already
			/// been played back.  If the page at *pOffset has already been played back
			/// (if the corresponding pDone bit is set) then skip the playback.
			/// Make sure the pDone bit corresponding to the *pOffset page is set
			/// prior to returning.
			///
			/// If the page record is successfully read from the (sub-)journal file
			/// and played back, then SQLITE_OK is returned. If an IO error occurs
			/// while reading the record from the (sub-)journal file or while writing
			/// to the database file, then the IO error code is returned. If data
			/// is successfully read from the (sub-)journal file but appears to be
			/// corrupted, SQLITE_DONE is returned. Data is considered corrupted in
			/// two circumstances:
			///
			///   * If the record page-number is illegal (0 or PAGER_MJ_PGNO), or
			///   * If the record is being rolled back from the main journal file
			///     and the checksum field does not match the record content.
			///
			/// Neither of these two scenarios are possible during a savepoint rollback.
			///
			/// If this is a savepoint rollback, then memory may have to be dynamically
			/// allocated by this function. If this is the case and an allocation fails,
			/// SQLITE_NOMEM is returned.
			///</summary>
			int pager_playback_one_page (///
///<summary>
///The pager being played back 
///</summary>

			ref i64 pOffset, ///
///<summary>
///Offset of record to playback 
///</summary>

			Bitvec pDone, ///
///<summary>
///Bitvec of pages already played back 
///</summary>

			int isMainJrnl, ///
///<summary>
///True for main rollback journal. False for Stmt jrnl 
///</summary>

			int isSavepnt///
///<summary>
///True for a savepoint rollback 
///</summary>

			)
			{
				int rc;
				PgHdr pPg;
				///
///<summary>
///An existing page in the cache 
///</summary>

				Pgno pgno = 0;
				///
///<summary>
///The page number of a page in journal 
///</summary>

				u32 cksum = 0;
				///
///<summary>
///Checksum used for sanity checking 
///</summary>

				byte[] aData;
				///
///<summary>
///Temporary storage for the page 
///</summary>

				sqlite3_file jfd;
				///
///<summary>
///The file descriptor for the journal file 
///</summary>

				bool isSynced;
				///
///<summary>
///True if journal page is synced 
///</summary>

				Debug.Assert ((isMainJrnl & ~1) == 0);
				///
///<summary>
///isMainJrnl is 0 or 1 
///</summary>

				Debug.Assert ((isSavepnt & ~1) == 0);
				///
///<summary>
///isSavepnt is 0 or 1 
///</summary>

				Debug.Assert (isMainJrnl != 0 || pDone != null);
				///
///<summary>
///</summary>
///<param name="pDone always used on sub">journals </param>

				Debug.Assert (isSavepnt != 0 || pDone == null);
				///
///<summary>
///</summary>
///<param name="pDone never used on non">savepoint </param>

				aData = this.pTmpSpace;
				Debug.Assert (aData != null);
				///
///<summary>
///Temp storage must have already been allocated 
///</summary>

				Debug.Assert (this.pagerUseWal () == false || (0 == isMainJrnl && isSavepnt != 0));
				///
///<summary>
///Either the state is greater than PAGER_WRITER_CACHEMOD (a transaction 
///or savepoint rollback done at the request of the caller) or this is
///</summary>
///<param name="a hot">journal rollback, the pager</param>
///<param name="is in state OPEN and holds an EXCLUSIVE lock. Hot">journal rollback</param>
///<param name="only reads from the main journal, not the sub">journal.</param>
///<param name=""></param>

				Debug.Assert (this.eState >= PAGER_WRITER_CACHEMOD || (this.eState == PAGER_OPEN && this.eLock == EXCLUSIVE_LOCK));
				Debug.Assert (this.eState >= PAGER_WRITER_CACHEMOD || isMainJrnl != 0);
				///
///<summary>
///</summary>
///<param name="Read the page number and page data from the journal or sub">journal</param>
///<param name="file. Return an error code to the caller if an IO error occurs.">file. Return an error code to the caller if an IO error occurs.</param>
///<param name=""></param>

				jfd = isMainJrnl != 0 ? this.jfd : this.sjfd;
				rc = read32bits (jfd, pOffset, ref pgno);
				if (rc != SQLITE_OK)
					return rc;
				rc = sqlite3OsRead (jfd, aData, this.pageSize, (pOffset) + 4);
				if (rc != SQLITE_OK)
					return rc;
				pOffset += this.pageSize + 4 + isMainJrnl * 4;
				///
///<summary>
///Sanity checking on the page.  This is more important that I originally
///thought.  If a power failure occurs while the journal is being written,
///it could cause invalid data to be written into the journal.  We need to
///detect this invalid data (with high probability) and ignore it.
///
///</summary>

				if (pgno == 0 || pgno == PAGER_MJ_PGNO (this)) {
					Debug.Assert (0 == isSavepnt);
					return SQLITE_DONE;
				}
				if (pgno > this.dbSize || sqlite3BitvecTest (pDone, pgno) != 0) {
					return SQLITE_OK;
				}
				if (isMainJrnl != 0) {
					rc = read32bits (jfd, (pOffset) - 4, ref cksum);
					if (rc != 0)
						return rc;
					if (0 == isSavepnt && this.pager_cksum (aData) != cksum) {
						return SQLITE_DONE;
					}
				}
				///
///<summary>
///If this page has already been played by before during the current
///rollback, then don't bother to play it back again.
///
///</summary>

				if (pDone != null && (rc = sqlite3BitvecSet (pDone, pgno)) != SQLITE_OK) {
					return rc;
				}
				///
///<summary>
///When playing back page 1, restore the nReserve setting
///
///</summary>

				if (pgno == 1 && this.nReserve != (aData) [20]) {
					this.nReserve = (aData) [20];
					this.pagerReportSize ();
				}
				///
///<summary>
///If the pager is in CACHEMOD state, then there must be a copy of this
///page in the pager cache. In this case just update the pager cache,
///not the database file. The page is left marked dirty in this case.
///
///</summary>
///<param name="An exception to the above rule: If the database is in no">sync mode</param>
///<param name="and a page is moved during an incremental vacuum then the page may">and a page is moved during an incremental vacuum then the page may</param>
///<param name="not be in the pager cache. Later: if a malloc() or IO error occurs">not be in the pager cache. Later: if a malloc() or IO error occurs</param>
///<param name="during a Movepage() call, then the page may not be in the cache">during a Movepage() call, then the page may not be in the cache</param>
///<param name="either. So the condition described in the above paragraph is not">either. So the condition described in the above paragraph is not</param>
///<param name="assert()able.">assert()able.</param>
///<param name=""></param>
///<param name="If in WRITER_DBMOD, WRITER_FINISHED or OPEN state, then we update the">If in WRITER_DBMOD, WRITER_FINISHED or OPEN state, then we update the</param>
///<param name="pager cache if it exists and the main file. The page is then marked ">pager cache if it exists and the main file. The page is then marked </param>
///<param name="not dirty. Since this code is only executed in PAGER_OPEN state for">not dirty. Since this code is only executed in PAGER_OPEN state for</param>
///<param name="a hot">cache is empty</param>
///<param name="if the pager is in OPEN state.">if the pager is in OPEN state.</param>
///<param name=""></param>
///<param name="Ticket #1171:  The statement journal might contain page content that is">Ticket #1171:  The statement journal might contain page content that is</param>
///<param name="different from the page content at the start of the transaction.">different from the page content at the start of the transaction.</param>
///<param name="This occurs when a page is changed prior to the start of a statement">This occurs when a page is changed prior to the start of a statement</param>
///<param name="then changed again within the statement.  When rolling back such a">then changed again within the statement.  When rolling back such a</param>
///<param name="statement we must not write to the original database unless we know">statement we must not write to the original database unless we know</param>
///<param name="for certain that original page contents are synced into the main rollback">for certain that original page contents are synced into the main rollback</param>
///<param name="journal.  Otherwise, a power loss might leave modified data in the">journal.  Otherwise, a power loss might leave modified data in the</param>
///<param name="database file without an entry in the rollback journal that can">database file without an entry in the rollback journal that can</param>
///<param name="restore the database to its original form.  Two conditions must be">restore the database to its original form.  Two conditions must be</param>
///<param name="met before writing to the database files. (1) the database must be">met before writing to the database files. (1) the database must be</param>
///<param name="locked.  (2) we know that the original page content is fully synced">locked.  (2) we know that the original page content is fully synced</param>
///<param name="in the main journal either because the page is not in cache or else">in the main journal either because the page is not in cache or else</param>
///<param name="the page is marked as needSync==0.">the page is marked as needSync==0.</param>
///<param name=""></param>
///<param name="2008">14:  When attempting to vacuum a corrupt database file, it</param>
///<param name="is possible to fail a statement on a database that does not yet exist.">is possible to fail a statement on a database that does not yet exist.</param>
///<param name="Do not attempt to write if database file has never been opened.">Do not attempt to write if database file has never been opened.</param>
///<param name=""></param>

				if (this.pagerUseWal ()) {
					pPg = null;
				}
				else {
					pPg = this.pager_lookup (pgno);
				}
				Debug.Assert (pPg != null || 
				#if SQLITE_OMIT_MEMORYDB
																																																																						0==MEMDB
#else
				this.memDb == 0
				#endif
				);
				Debug.Assert (this.eState != PAGER_OPEN || pPg == null);
				PAGERTRACE ("PLAYBACK %d page %d hash(%08x) %s\n", PAGERID (this), pgno, pager_datahash (this.pageSize, aData), (isMainJrnl != 0 ? "main-journal" : "sub-journal"));
				if (isMainJrnl != 0) {
					isSynced = this.noSync || (pOffset <= this.journalHdr);
				}
				else {
					isSynced = (pPg == null || 0 == (pPg.flags & PGHDR_NEED_SYNC));
				}
				if (isOpen (this.fd) && (this.eState >= PAGER_WRITER_DBMOD || this.eState == PAGER_OPEN) && isSynced) {
					i64 ofst = (pgno - 1) * this.pageSize;
					testcase (0 == isSavepnt && pPg != null && (pPg.flags & PGHDR_NEED_SYNC) != 0);
					Debug.Assert (!this.pagerUseWal ());
					rc = sqlite3OsWrite (this.fd, aData, this.pageSize, ofst);
					if (pgno > this.dbFileSize) {
						this.dbFileSize = pgno;
					}
					if (this.pBackup != null) {
						if (CODEC1 (this, aData, pgno, SQLITE_DECRYPT))
							rc = SQLITE_NOMEM;
						// CODEC1( pPager, aData, pgno, 3, rc = SQLITE_NOMEM );
						this.pBackup.sqlite3BackupUpdate (pgno, (u8[])aData);
						if (CODEC2 (this, aData, pgno, SQLITE_ENCRYPT_READ_CTX, ref aData))
							rc = SQLITE_NOMEM;
						//CODEC2( pPager, aData, pgno, 7, rc = SQLITE_NOMEM, aData);
					}
				}
				else
					if (0 == isMainJrnl && pPg == null) {
						///
///<summary>
///If this is a rollback of a savepoint and data was not written to
///</summary>
///<param name="the database and the page is not in">memory, there is a potential</param>
///<param name="problem. When the page is next fetched by the b">tree layer, it</param>
///<param name="will be read from the database file, which may or may not be">will be read from the database file, which may or may not be</param>
///<param name="current.">current.</param>
///<param name=""></param>
///<param name="There are a couple of different ways this can happen. All are quite">There are a couple of different ways this can happen. All are quite</param>
///<param name="obscure. When running in synchronous mode, this can only happen">obscure. When running in synchronous mode, this can only happen</param>
///<param name="if the page is on the free">list at the start of the transaction, then</param>
///<param name="populated, then moved using sqlite3PagerMovepage().">populated, then moved using sqlite3PagerMovepage().</param>
///<param name=""></param>
///<param name="The solution is to add an in">memory page to the cache containing</param>
///<param name="the data just read from the sub">journal. Mark the page as dirty</param>
///<param name="and if the pager requires a journal">sync, then mark the page as</param>
///<param name="requiring a journal">sync before it is written.</param>
///<param name=""></param>

						Debug.Assert (isSavepnt != 0);
						Debug.Assert (this.doNotSpill == 0);
						this.doNotSpill++;
						rc = this.sqlite3PagerAcquire (pgno, ref pPg, 1);
						Debug.Assert (this.doNotSpill == 1);
						this.doNotSpill--;
						if (rc != SQLITE_OK)
							return rc;
						pPg.flags &= ~PGHDR_NEED_READ;
						sqlite3PcacheMakeDirty (pPg);
					}
				if (pPg != null) {
					///
///<summary>
///No page should ever be explicitly rolled back that is in use, except
///for page 1 which is held in use in order to keep the lock on the
///database active. However such a page may be rolled back as a result
///of an internal error resulting in an automatic call to
///sqlite3PagerRollback().
///
///</summary>

					byte[] pData = pPg.pData;
					Buffer.BlockCopy (aData, 0, pData, 0, this.pageSize);
					// memcpy(pData, (u8[])aData, pPager.pageSize);
					this.xReiniter (pPg);
					if (isMainJrnl != 0 && (0 == isSavepnt || pOffset <= this.journalHdr)) {
						///
///<summary>
///If the contents of this page were just restored from the main
///journal file, then its content must be as they were when the
///transaction was first opened. In this case we can mark the page
///as clean, since there will be no need to write it out to the
///database.
///
///There is one exception to this rule. If the page is being rolled
///back as part of a savepoint (or statement) rollback from an
///unsynced portion of the main journal file, then it is not safe
///to mark the page as clean. This is because marking the page as
///clean will clear the PGHDR_NEED_SYNC flag. Since the page is
///already in the journal file (recorded in Pager.pInJournal) and
///the PGHDR_NEED_SYNC flag is cleared, if the page is written to
///again within this transaction, it will be marked as dirty but
///the PGHDR_NEED_SYNC flag will not be set. It could then potentially
///be written out into the database file before its journal file
///segment is synced. If a crash occurs during or following this,
///database corruption may ensue.
///
///</summary>

						Debug.Assert (!this.pagerUseWal ());
						sqlite3PcacheMakeClean (pPg);
					}
					pPg.pager_set_pagehash ();
					///
///<summary>
///If this was page 1, then restore the value of Pager.dbFileVers.
///Do this before any decoding. 
///</summary>

					if (pgno == 1) {
						Buffer.BlockCopy (pData, 24, this.dbFileVers, 0, this.dbFileVers.Length);
						//memcpy(pPager.dbFileVers, ((u8*)pData)[24], sizeof(pPager.dbFileVers));
					}
					///
///<summary>
///Decode the page just read from disk 
///</summary>

					if (CODEC1 (this, pData, pPg.pgno, SQLITE_DECRYPT))
						rc = SQLITE_NOMEM;
					//CODEC1(pPager, pData, pPg.pgno, 3, rc=SQLITE_NOMEM);
					sqlite3PcacheRelease (pPg);
				}
				return rc;
			}

			public///<summary>
			/// Parameter zMaster is the name of a master journal file. A single journal
			/// file that referred to the master journal file has just been rolled back.
			/// This routine checks if it is possible to delete the master journal file,
			/// and does so if it is.
			///
			/// Argument zMaster may point to Pager.pTmpSpace. So that buffer is not
			/// available for use within this function.
			///
			/// When a master journal file is created, it is populated with the names
			/// of all of its child journals, one after another, formatted as utf-8
			/// encoded text. The end of each child journal file is marked with a
			/// nul-terminator byte (0x00). i.e. the entire contents of a master journal
			/// file for a transaction involving two databases might be:
			///
			///   "/home/bill/a.db-journal\x00/home/bill/b.db-journal\x00"
			///
			/// A master journal file may only be deleted once all of its child
			/// journals have been rolled back.
			///
			/// This function reads the contents of the master-journal file into
			/// memory and loops through each of the child journal names. For
			/// each child journal, it checks if:
			///
			///   * if the child journal exists, and if so
			///   * if the child journal contains a reference to master journal
			///     file zMaster
			///
			/// If a child journal can be found that matches both of the criteria
			/// above, this function returns without doing anything. Otherwise, if
			/// no such child journal can be found, file zMaster is deleted from
			/// the file-system using sqlite3OsDelete().
			///
			/// If an IO error within this function, an error code is returned. This
			/// function allocates memory by calling sqlite3Malloc(). If an allocation
			/// fails, SQLITE_NOMEM is returned. Otherwise, if no IO or malloc errors
			/// occur, SQLITE_OK is returned.
			///
			/// TODO: This function allocates a single block of memory to load
			/// the entire contents of the master journal file. This could be
			/// a couple of kilobytes or so - potentially larger than the page
			/// size.
			///
			///</summary>
			int pager_delmaster (string zMaster)
			{
				sqlite3_vfs pVfs = this.pVfs;
				int rc;
				///
///<summary>
///Return code 
///</summary>

				sqlite3_file pMaster;
				///
///<summary>
///</summary>
///<param name="Malloc'd master">journal file descriptor </param>

				sqlite3_file pJournal;
				///
///<summary>
///</summary>
///<param name="Malloc'd child">journal file descriptor </param>

				//string zMasterJournal = null; /* Contents of master journal file */
				i64 nMasterJournal;
				///
///<summary>
///Size of master journal file 
///</summary>

				string zJournal;
				///
///<summary>
///Pointer to one journal within MJ file 
///</summary>

				string zMasterPtr;
				///
///<summary>
///Space to hold MJ filename from a journal file 
///</summary>

				int nMasterPtr;
				///
///<summary>
///Amount of space allocated to zMasterPtr[] 
///</summary>

				///
///<summary>
///Allocate space for both the pJournal and pMaster file descriptors.
///If successful, open the master journal file for reading.
///
///</summary>

				pMaster = new sqlite3_file ();
				// (sqlite3_file*)sqlite3MallocZero( pVfs.szOsFile * 2 );
				pJournal = new sqlite3_file ();
				// (sqlite3_file*)( ( (u8*)pMaster ) + pVfs.szOsFile );
				//if ( null == pMaster )
				//{
				//  rc = SQLITE_NOMEM;
				//}
				//else
				{
					const int flags = (SQLITE_OPEN_READONLY | SQLITE_OPEN_MASTER_JOURNAL);
					int iDummy = 0;
					rc = sqlite3OsOpen (pVfs, zMaster, pMaster, flags, ref iDummy);
					//TODO --
					///
///<summary>
///Load the entire master journal file into space obtained from
///sqlite3_malloc() and pointed to by zMasterJournal.   Also obtain
///sufficient space (in zMasterPtr) to hold the names of master
///</summary>
///<param name="journal files extracted from regular rollback">journals.</param>
///<param name=""></param>

					//rc = sqlite3OsFileSize(pMaster, &nMasterJournal);
					//if (rc != SQLITE_OK) goto delmaster_out;
					//nMasterPtr = pVfs.mxPathname + 1;
					//  zMasterJournal = sqlite3Malloc((int)nMasterJournal + nMasterPtr + 1);
					//  if ( !zMasterJournal )
					//  {
					//    rc = SQLITE_NOMEM;
					//    goto delmaster_out;
					//  }
					//  zMasterPtr = &zMasterJournal[nMasterJournal+1];
					//  rc = sqlite3OsRead( pMaster, zMasterJournal, (int)nMasterJournal, 0 );
					//  if ( rc != SQLITE_OK ) goto delmaster_out;
					//  zMasterJournal[nMasterJournal] = 0;
					//  zJournal = zMasterJournal;
					//  while ( ( zJournal - zMasterJournal ) < nMasterJournal )
					//  {
					//    int exists;
					//    rc = sqlite3OsAccess( pVfs, zJournal, SQLITE_ACCESS_EXISTS, &exists );
					//    if ( rc != SQLITE_OK )
					//    {
					//      goto delmaster_out;
					//    }
					//    if ( exists )
					//    {
					//      /* One of the journals pointed to by the master journal exists.
					//      ** Open it and check if it points at the master journal. If
					//      ** so, return without deleting the master journal file.
					//      */
					//      int c;
					//      int flags = ( SQLITE_OPEN_READONLY | SQLITE_OPEN_MAIN_JOURNAL );
					//      rc = sqlite3OsOpen( pVfs, zJournal, pJournal, flags, 0 );
					//      if ( rc != SQLITE_OK )
					//      {
					//        goto delmaster_out;
					//      }
					//      rc = readMasterJournal( pJournal, zMasterPtr, nMasterPtr );
					//      sqlite3OsClose( pJournal );
					//      if ( rc != SQLITE_OK )
					//      {
					//        goto delmaster_out;
					//      }
					//      c = zMasterPtr[0] != 0 && strcmp( zMasterPtr, zMaster ) == 0;
					//      if ( c )
					//      {
					//        /* We have a match. Do not delete the master journal file. */
					//        goto delmaster_out;
					//      }
					//    }
					//    zJournal += ( StringExtensions.sqlite3Strlen30( zJournal ) + 1 );
					//   }
					//
					//sqlite3OsClose(pMaster);
					//rc = sqlite3OsDelete( pVfs, zMaster, 0 );
					//sqlite3_free( ref zMasterJournal );
				}
				if (rc != SQLITE_OK)
					goto delmaster_out;
				Debugger.Break ();
				goto delmaster_out;
				delmaster_out:
				if (pMaster != null) {
					sqlite3OsClose (pMaster);
					Debug.Assert (!isOpen (pJournal));
					//sqlite3_free( ref pMaster );
				}
				return rc;
			}

			public///<summary>
			/// This function is used to change the actual size of the database
			/// file in the file-system. This only happens when committing a transaction,
			/// or rolling back a transaction (including rolling back a hot-journal).
			///
			/// If the main database file is not open, or the pager is not in either
			/// DBMOD or OPEN state, this function is a no-op. Otherwise, the size
			/// of the file is changed to nPage pages (nPage*pPager.pageSize bytes).
			/// If the file on disk is currently larger than nPage pages, then use the VFS
			/// xTruncate() method to truncate it.
			///
			/// Or, it might might be the case that the file on disk is smaller than
			/// nPage pages. Some operating system implementations can get confused if
			/// you try to truncate a file to some size that is larger than it
			/// currently is, so detect this case and write a single zero byte to
			/// the end of the new file instead.
			///
			/// If successful, return SQLITE_OK. If an IO error occurs while modifying
			/// the database file, return the error code to the caller.
			///
			///</summary>
			int pager_truncate (u32 nPage)
			{
				int rc = SQLITE_OK;
				Debug.Assert (this.eState != PAGER_ERROR);
				Debug.Assert (this.eState != PAGER_READER);
				if (isOpen (this.fd) && (this.eState >= PAGER_WRITER_DBMOD || this.eState == PAGER_OPEN)) {
					i64 currentSize = 0, newSize;
					int szPage = this.pageSize;
					Debug.Assert (this.eLock == EXCLUSIVE_LOCK);
					///
///<summary>
///TODO: Is it safe to use Pager.dbFileSize here? 
///</summary>

					rc = sqlite3OsFileSize (this.fd, ref currentSize);
					newSize = szPage * nPage;
					if (rc == SQLITE_OK && currentSize != newSize) {
						if (currentSize > newSize) {
							rc = sqlite3OsTruncate (this.fd, newSize);
						}
						else {
							byte[] pTmp = this.pTmpSpace;
							Array.Clear (pTmp, 0, szPage);
							//memset( pTmp, 0, szPage );
							testcase ((newSize - szPage) < currentSize);
							testcase ((newSize - szPage) == currentSize);
							testcase ((newSize - szPage) > currentSize);
							rc = sqlite3OsWrite (this.fd, pTmp, szPage, newSize - szPage);
						}
						if (rc == SQLITE_OK) {
							this.dbSize = nPage;
						}
					}
				}
				return rc;
			}

			public///<summary>
			/// Set the value of the Pager.sectorSize variable for the given
			/// pager based on the value returned by the xSectorSize method
			/// of the open database file. The sector size will be used used
			/// to determine the size and alignment of journal header and
			/// master journal pointers within created journal files.
			///
			/// For temporary files the effective sector size is always 512 bytes.
			///
			/// Otherwise, for non-temporary files, the effective sector size is
			/// the value returned by the xSectorSize() method rounded up to 512 if
			/// it is less than 32, or rounded down to MAX_SECTOR_SIZE if it
			/// is greater than MAX_SECTOR_SIZE.
			///
			///</summary>
			void setSectorSize ()
			{
				Debug.Assert (isOpen (this.fd) || this.tempFile);
				if (!this.tempFile) {
					///
///<summary>
///Sector size doesn't matter for temporary files. Also, the file
///may not have been opened yet, in which case the OsSectorSize()
///call will segfault.
///
///</summary>

					this.sectorSize = (u32)sqlite3OsSectorSize (this.fd);
				}
				if (this.sectorSize < 32) {
					Debug.Assert (MAX_SECTOR_SIZE >= 512);
					this.sectorSize = 512;
				}
				if (this.sectorSize > MAX_SECTOR_SIZE) {
					this.sectorSize = MAX_SECTOR_SIZE;
				}
			}

			public///<summary>
			/// Playback the journal and thus restore the database file to
			/// the state it was in before we started making changes.
			///
			/// The journal file format is as follows:
			///
			///  (1)  8 byte prefix.  A copy of aJournalMagic[].
			///  (2)  4 byte big-endian integer which is the number of valid page records
			///       in the journal.  If this value is 0xffffffff, then compute the
			///       number of page records from the journal size.
			///  (3)  4 byte big-endian integer which is the initial value for the
			///       sanity checksum.
			///  (4)  4 byte integer which is the number of pages to truncate the
			///       database to during a rollback.
			///  (5)  4 byte big-endian integer which is the sector size.  The header
			///       is this many bytes in size.
			///  (6)  4 byte big-endian integer which is the page size.
			///  (7)  zero padding out to the next sector size.
			///  (8)  Zero or more pages instances, each as follows:
			///
			/// When we speak of the journal header, we mean the first 7 items above.
			/// Each entry in the journal is an instance of the 8th item.
			///
			/// Call the value from the second bullet "nRec".  nRec is the number of
			/// valid page entries in the journal.  In most cases, you can compute the
			/// value of nRec from the size of the journal file.  But if a power
			/// failure occurred while the journal was being written, it could be the
			/// case that the size of the journal file had already been increased but
			/// the extra entries had not yet made it safely to disk.  In such a case,
			/// the value of nRec computed from the file size would be too large.  For
			/// that reason, we always use the nRec value in the header.
			///
			/// If the nRec value is 0xffffffff it means that nRec should be computed
			/// from the file size.  This value is used when the user selects the
			/// no-sync option for the journal.  A power failure could lead to corruption
			/// in this case.  But for things like temporary table (which will be
			/// deleted when the power is restored) we don't care.
			///
			/// If the file opened as the journal file is not a well-formed
			/// journal file then all pages up to the first corrupted page are rolled
			/// back (or no pages if the journal header is corrupted). The journal file
			/// is then deleted and SQLITE_OK returned, just as if no corruption had
			/// been encountered.
			///
			/// If an I/O or malloc() error occurs, the journal-file is not deleted
			/// and an error code is returned.
			///
			/// The isHot parameter indicates that we are trying to rollback a journal
			/// that might be a hot journal.  Or, it could be that the journal is
			/// preserved because of JOURNALMODE_PERSIST or JOURNALMODE_TRUNCATE.
			/// If the journal really is hot, reset the pager cache prior rolling
			/// back any content.  If the journal is merely persistent, no reset is
			/// needed.
			///
			///</summary>
			int pager_playback (int isHot)
			{
				sqlite3_vfs pVfs = this.pVfs;
				i64 szJ = 0;
				///
///<summary>
///Size of the journal file in bytes 
///</summary>

				u32 nRec = 0;
				///
///<summary>
///Number of Records in the journal 
///</summary>

				u32 u;
				///
///<summary>
///Unsigned loop counter 
///</summary>

				u32 mxPg = 0;
				///
///<summary>
///Size of the original file in pages 
///</summary>

				int rc;
				///
///<summary>
///Result code of a subroutine 
///</summary>

				int res = 1;
				///
///<summary>
///Value returned by sqlite3OsAccess() 
///</summary>

				byte[] zMaster = null;
				///
///<summary>
///Name of master journal file if any 
///</summary>

				int needPagerReset;
				///
///<summary>
///True to reset page prior to first page rollback 
///</summary>

				///
///<summary>
///Figure out how many records are in the journal.  Abort early if
///the journal is empty.
///
///</summary>

				Debug.Assert (isOpen (this.jfd));
				rc = sqlite3OsFileSize (this.jfd, ref szJ);
				if (rc != SQLITE_OK) {
					goto end_playback;
				}
				///
///<summary>
///Read the master journal name from the journal, if it is present.
///If a master journal file name is specified, but the file is not
///present on disk, then the journal is not hot and does not need to be
///played back.
///
///TODO: Technically the following is an error because it assumes that
///buffer Pager.pTmpSpace is (mxPathname+1) bytes or larger. i.e. that
///(pPager.pageSize >= pPager.pVfs.mxPathname+1). Using os_unix.c,
///mxPathname is 512, which is the same as the minimum allowable value
///for pageSize.
///
///</summary>

				zMaster = new byte[this.pVfs.mxPathname + 1];
				// pPager.pTmpSpace );
				rc = readMasterJournal (this.jfd, zMaster, (u32)this.pVfs.mxPathname + 1);
				if (rc == SQLITE_OK && zMaster [0] != 0) {
					rc = sqlite3OsAccess (pVfs, Encoding.UTF8.GetString (zMaster, 0, zMaster.Length), SQLITE_ACCESS_EXISTS, ref res);
				}
				zMaster = null;
				if (rc != SQLITE_OK || res == 0) {
					goto end_playback;
				}
				this.journalOff = 0;
				needPagerReset = isHot;
				///
///<summary>
///This loop terminates either when a readJournalHdr() or
///pager_playback_one_page() call returns SQLITE_DONE or an IO error
///occurs.
///
///</summary>

				while (true) {
					///
///<summary>
///Read the next journal header from the journal file.  If there are
///not enough bytes left in the journal file for a complete header, or
///it is corrupted, then a process must have failed while writing it.
///This indicates nothing more needs to be rolled back.
///
///</summary>

					rc = this.readJournalHdr (isHot, szJ, out nRec, out mxPg);
					if (rc != SQLITE_OK) {
						if (rc == SQLITE_DONE) {
							rc = SQLITE_OK;
						}
						goto end_playback;
					}
					///
///<summary>
///If nRec is 0xffffffff, then this journal was created by a process
///</summary>
///<param name="working in no">sync mode. This means that the rest of the journal</param>
///<param name="file consists of pages, there are no more journal headers. Compute">file consists of pages, there are no more journal headers. Compute</param>
///<param name="the value of nRec based on this assumption.">the value of nRec based on this assumption.</param>
///<param name=""></param>

					if (nRec == 0xffffffff) {
						Debug.Assert (this.journalOff == this.JOURNAL_HDR_SZ ());
						nRec = (u32)((szJ - this.JOURNAL_HDR_SZ ()) / this.JOURNAL_PG_SZ ());
					}
					///
///<summary>
///If nRec is 0 and this rollback is of a transaction created by this
///process and if this is the final header in the journal, then it means
///that this part of the journal was being filled but has not yet been
///synced to disk.  Compute the number of pages based on the remaining
///size of the file.
///
///The third term of the test was added to fix ticket #2565.
///When rolling back a hot journal, nRec==0 always means that the next
///chunk of the journal contains zero pages to be rolled back.  But
///when doing a ROLLBACK and the nRec==0 chunk is the last chunk in
///the journal, it means that the journal might contain additional
///pages that need to be rolled back and that the number of pages
///should be computed based on the journal file size.
///
///</summary>

					if (nRec == 0 && 0 == isHot && this.journalHdr + this.JOURNAL_HDR_SZ () == this.journalOff) {
						nRec = (u32)((szJ - this.journalOff) / this.JOURNAL_PG_SZ ());
					}
					///
///<summary>
///If this is the first header read from the journal, truncate the
///database file back to its original size.
///
///</summary>

					if (this.journalOff == this.JOURNAL_HDR_SZ ()) {
						rc = this.pager_truncate (mxPg);
						if (rc != SQLITE_OK) {
							goto end_playback;
						}
						this.dbSize = mxPg;
					}
					///
///<summary>
///Copy original pages out of the journal and back into the
///database file and/or page cache.
///
///</summary>

					for (u = 0; u < nRec; u++) {
						if (needPagerReset != 0) {
							this.pager_reset ();
							needPagerReset = 0;
						}
						rc = this.pager_playback_one_page (ref this.journalOff, null, 1, 0);
						if (rc != SQLITE_OK) {
							if (rc == SQLITE_DONE) {
								rc = SQLITE_OK;
								this.journalOff = szJ;
								break;
							}
							else
								if (rc == SQLITE_IOERR_SHORT_READ) {
									///
///<summary>
///If the journal has been truncated, simply stop reading and
///processing the journal. This might happen if the journal was
///not completely written and synced prior to a crash.  In that
///case, the database should have never been written in the
///first place so it is OK to simply abandon the rollback. 
///</summary>

									rc = SQLITE_OK;
									goto end_playback;
								}
								else {
									///
///<summary>
///If we are unable to rollback, quit and return the error
///code.  This will cause the pager to enter the error state
///so that no further harm will be done.  Perhaps the next
///process to come along will be able to rollback the database.
///
///</summary>

									goto end_playback;
								}
						}
					}
				}
				///
///<summary>
///NOTREACHED
///</summary>

				end_playback:
				///
///<summary>
///Following a rollback, the database file should be back in its original
///state prior to the start of the transaction, so invoke the
///</summary>
///<param name="SQLITE_FCNTL_DB_UNCHANGED file">control method to disable the</param>
///<param name="assertion that the transaction counter was modified.">assertion that the transaction counter was modified.</param>
///<param name=""></param>

				sqlite3_int64 iDummy = 0;
				Debug.Assert (this.fd.pMethods == null || sqlite3OsFileControl (this.fd, SQLITE_FCNTL_DB_UNCHANGED, ref iDummy) >= SQLITE_OK);
				///
///<summary>
///If this playback is happening automatically as a result of an IO or
///</summary>
///<param name="malloc error that occurred after the change">counter was updated but</param>
///<param name="before the transaction was committed, then the change">counter</param>
///<param name="modification may just have been reverted. If this happens in exclusive">modification may just have been reverted. If this happens in exclusive</param>
///<param name="mode, then subsequent transactions performed by the connection will not">mode, then subsequent transactions performed by the connection will not</param>
///<param name="update the change">counter at all. This may lead to cache inconsistency</param>
///<param name="problems for other processes at some point in the future. So, just">problems for other processes at some point in the future. So, just</param>
///<param name="in case this has happened, clear the changeCountDone flag now.">in case this has happened, clear the changeCountDone flag now.</param>
///<param name=""></param>

				this.changeCountDone = this.tempFile;
				if (rc == SQLITE_OK) {
					zMaster = new byte[this.pVfs.mxPathname + 1];
					//pPager.pTmpSpace );
					rc = readMasterJournal (this.jfd, zMaster, (u32)this.pVfs.mxPathname + 1);
					testcase (rc != SQLITE_OK);
				}
				if (rc == SQLITE_OK && (this.eState >= PAGER_WRITER_DBMOD || this.eState == PAGER_OPEN)) {
					rc = this.sqlite3PagerSync ();
				}
				if (rc == SQLITE_OK) {
					rc = this.pager_end_transaction (zMaster [0] != '\0' ? 1 : 0);
					testcase (rc != SQLITE_OK);
				}
				if (rc == SQLITE_OK && zMaster [0] != '\0' && res != 0) {
					///
///<summary>
///If there was a master journal and this routine will return success,
///see if it is possible to delete the master journal.
///
///</summary>

					rc = this.pager_delmaster (Encoding.UTF8.GetString (zMaster, 0, zMaster.Length));
					testcase (rc != SQLITE_OK);
				}
				///
///<summary>
///The Pager.sectorSize variable may have been updated while rolling
///back a journal created by a process with a different sector size
///value. Reset it to the correct value for this process.
///
///</summary>

				this.setSectorSize ();
				return rc;
			}

			public///<summary>
			/// This function is called as part of the transition from PAGER_OPEN
			/// to PAGER_READER state to determine the size of the database file
			/// in pages (assuming the page size currently stored in Pager.pageSize).
			///
			/// If no error occurs, SQLITE_OK is returned and the size of the database
			/// in pages is stored in *pnPage. Otherwise, an error code (perhaps
			/// SQLITE_IOERR_FSTAT) is returned and *pnPage is left unmodified.
			///
			///</summary>
			int pagerPagecount (ref Pgno pnPage)
			{
				Pgno nPage;
				///
///<summary>
///Value to return via *pnPage 
///</summary>

				///
///<summary>
///</summary>
///<param name="Query the WAL sub">system for the database size. The WalDbsize()</param>
///<param name="function returns zero if the WAL is not open (i.e. Pager.pWal==0), or">function returns zero if the WAL is not open (i.e. Pager.pWal==0), or</param>
///<param name="if the database size is not available. The database size is not">if the database size is not available. The database size is not</param>
///<param name="available from the WAL sub">system if the log file is empty or</param>
///<param name="contains no valid committed transactions.">contains no valid committed transactions.</param>
///<param name=""></param>

				Debug.Assert (this.eState == PAGER_OPEN);
				Debug.Assert (this.eLock >= SHARED_LOCK || this.noReadlock != 0);
				nPage = sqlite3WalDbsize (this.pWal);
				///
///<summary>
///</summary>
///<param name="If the database size was not available from the WAL sub">system,</param>
///<param name="determine it based on the size of the database file. If the size">determine it based on the size of the database file. If the size</param>
///<param name="of the database file is not an integer multiple of the page">size,</param>
///<param name="round down to the nearest page. Except, any file larger than 0">round down to the nearest page. Except, any file larger than 0</param>
///<param name="bytes in size is considered to contain at least one page.">bytes in size is considered to contain at least one page.</param>
///<param name=""></param>

				if (nPage == 0) {
					i64 n = 0;
					///
///<summary>
///Size of db file in bytes 
///</summary>

					Debug.Assert (isOpen (this.fd) || this.tempFile);
					if (isOpen (this.fd)) {
						int rc = sqlite3OsFileSize (this.fd, ref n);
						if (rc != SQLITE_OK) {
							return rc;
						}
					}
					nPage = (Pgno)(n / this.pageSize);
					if (nPage == 0 && n > 0) {
						nPage = 1;
					}
				}
				///
///<summary>
///If the current number of pages in the file is greater than the
///configured maximum pager number, increase the allowed limit so
///that the file can be read.
///
///</summary>

				if (nPage > this.mxPgno) {
					this.mxPgno = (Pgno)nPage;
				}
				pnPage = nPage;
				return SQLITE_OK;
			}

			public///<summary>
			/// Playback savepoint pSavepoint. Or, if pSavepoint==NULL, then playback
			/// the entire master journal file. The case pSavepoint==NULL occurs when
			/// a ROLLBACK TO command is invoked on a SAVEPOINT that is a transaction
			/// savepoint.
			///
			/// When pSavepoint is not NULL (meaning a non-transaction savepoint is
			/// being rolled back), then the rollback consists of up to three stages,
			/// performed in the order specified:
			///
			///   * Pages are played back from the main journal starting at byte
			///     offset PagerSavepoint.iOffset and continuing to
			///     PagerSavepoint.iHdrOffset, or to the end of the main journal
			///     file if PagerSavepoint.iHdrOffset is zero.
			///
			///   * If PagerSavepoint.iHdrOffset is not zero, then pages are played
			///     back starting from the journal header immediately following
			///     PagerSavepoint.iHdrOffset to the end of the main journal file.
			///
			///   * Pages are then played back from the sub-journal file, starting
			///     with the PagerSavepoint.iSubRec and continuing to the end of
			///     the journal file.
			///
			/// Throughout the rollback process, each time a page is rolled back, the
			/// corresponding bit is set in a bitvec structure (variable pDone in the
			/// implementation below). This is used to ensure that a page is only
			/// rolled back the first time it is encountered in either journal.
			///
			/// If pSavepoint is NULL, then pages are only played back from the main
			/// journal file. There is no need for a bitvec in this case.
			///
			/// In either case, before playback commences the Pager.dbSize variable
			/// is reset to the value that it held at the start of the savepoint
			/// (or transaction). No page with a page-number greater than this value
			/// is played back. If one is encountered it is simply skipped.
			///</summary>
			int pagerPlaybackSavepoint (PagerSavepoint pSavepoint)
			{
				i64 szJ;
				///
///<summary>
///Effective size of the main journal 
///</summary>

				i64 iHdrOff;
				///
///<summary>
///</summary>
///<param name="End of first segment of main">journal records </param>

				int rc = SQLITE_OK;
				///
///<summary>
///Return code 
///</summary>

				Bitvec pDone = null;
				///
///<summary>
///Bitvec to ensure pages played back only once 
///</summary>

				Debug.Assert (this.eState != PAGER_ERROR);
				Debug.Assert (this.eState >= PAGER_WRITER_LOCKED);
				///
///<summary>
///Allocate a bitvec to use to store the set of pages rolled back 
///</summary>

				if (pSavepoint != null) {
					pDone = sqlite3BitvecCreate (pSavepoint.nOrig);
					//if ( null == pDone )
					//{
					//  return SQLITE_NOMEM;
					//}
				}
				///
///<summary>
///Set the database size back to the value it was before the savepoint
///being reverted was opened.
///
///</summary>

				this.dbSize = pSavepoint != null ? pSavepoint.nOrig : this.dbOrigSize;
				this.changeCountDone = this.tempFile;
				if (!pSavepoint && this.pagerUseWal ()) {
					return this.pagerRollbackWal ();
				}
				///
///<summary>
///Use pPager.journalOff as the effective size of the main rollback
///journal.  The actual file might be larger than this in
///PAGER_JOURNALMODE_TRUNCATE or PAGER_JOURNALMODE_PERSIST.  But anything
///</summary>
///<param name="past pPager.journalOff is off">limits to us.</param>
///<param name=""></param>

				szJ = this.journalOff;
				Debug.Assert (this.pagerUseWal () == false || szJ == 0);
				///
///<summary>
///Begin by rolling back records from the main journal starting at
///PagerSavepoint.iOffset and continuing to the next journal header.
///There might be records in the main journal that have a page number
///greater than the current database size (pPager.dbSize) but those
///will be skipped automatically.  Pages are added to pDone as they
///are played back.
///
///</summary>

				if (pSavepoint != null && !this.pagerUseWal ()) {
					iHdrOff = pSavepoint.iHdrOffset != 0 ? pSavepoint.iHdrOffset : szJ;
					this.journalOff = pSavepoint.iOffset;
					while (rc == SQLITE_OK && this.journalOff < iHdrOff) {
						rc = this.pager_playback_one_page (ref this.journalOff, pDone, 1, 1);
					}
					Debug.Assert (rc != SQLITE_DONE);
				}
				else {
					this.journalOff = 0;
				}
				///
///<summary>
///Continue rolling back records out of the main journal starting at
///the first journal header seen and continuing until the effective end
///</summary>
///<param name="of the main journal file.  Continue to skip out">range pages and</param>
///<param name="continue adding pages rolled back to pDone.">continue adding pages rolled back to pDone.</param>
///<param name=""></param>

				while (rc == SQLITE_OK && this.journalOff < szJ) {
					u32 ii;
					///
///<summary>
///Loop counter 
///</summary>

					u32 nJRec;
					///
///<summary>
///Number of Journal Records 
///</summary>

					u32 dummy;
					rc = this.readJournalHdr (0, (int)szJ, out nJRec, out dummy);
					Debug.Assert (rc != SQLITE_DONE);
					///
///<summary>
///The "pPager.journalHdr+JOURNAL_HDR_SZ(pPager)==pPager.journalOff"
///test is related to ticket #2565.  See the discussion in the
///pager_playback() function for additional information.
///
///</summary>

					if (nJRec == 0 && this.journalHdr + this.JOURNAL_HDR_SZ () >= this.journalOff) {
						nJRec = (u32)((szJ - this.journalOff) / this.JOURNAL_PG_SZ ());
					}
					for (ii = 0; rc == SQLITE_OK && ii < nJRec && this.journalOff < szJ; ii++) {
						rc = this.pager_playback_one_page (ref this.journalOff, pDone, 1, 1);
					}
					Debug.Assert (rc != SQLITE_DONE);
				}
				Debug.Assert (rc != SQLITE_OK || this.journalOff >= szJ);
				///
///<summary>
///</summary>
///<param name="Finally,  rollback pages from the sub">journal.  Page that were</param>
///<param name="previously rolled back out of the main journal (and are hence in pDone)">previously rolled back out of the main journal (and are hence in pDone)</param>
///<param name="will be skipped.  Out">range pages are also skipped.</param>
///<param name=""></param>

				if (pSavepoint != null) {
					u32 ii;
					///
///<summary>
///Loop counter 
///</summary>

					i64 offset = pSavepoint.iSubRec * (4 + this.pageSize);
					if (this.pagerUseWal ()) {
						rc = sqlite3WalSavepointUndo (this.pWal, pSavepoint.aWalData);
					}
					for (ii = pSavepoint.iSubRec; rc == SQLITE_OK && ii < this.nSubRec; ii++) {
						Debug.Assert (offset == ii * (4 + this.pageSize));
						rc = this.pager_playback_one_page (ref offset, pDone, 0, 1);
					}
					Debug.Assert (rc != SQLITE_DONE);
				}
				sqlite3BitvecDestroy (ref pDone);
				if (rc == SQLITE_OK) {
					this.journalOff = (int)szJ;
				}
				return rc;
			}

			public void sqlite3PagerSetCachesize (int mxPage)
			{
				sqlite3PcacheSetCachesize (this.pPCache, mxPage);
			}

			public///<summary>
			/// Adjust the robustness of the database to damage due to OS crashes
			/// or power failures by changing the number of syncs()s when writing
			/// the rollback journal.  There are three levels:
			///
			///    OFF       sqlite3OsSync() is never called.  This is the default
			///              for temporary and transient files.
			///
			///    NORMAL    The journal is synced once before writes begin on the
			///              database.  This is normally adequate protection, but
			///              it is theoretically possible, though very unlikely,
			///              that an inopertune power failure could leave the journal
			///              in a state which would cause damage to the database
			///              when it is rolled back.
			///
			///    FULL      The journal is synced twice before writes begin on the
			///              database (with some additional information - the nRec field
			///              of the journal header - being written in between the two
			///              syncs).  If we assume that writing a
			///              single disk sector is atomic, then this mode provides
			///              assurance that the journal will not be corrupted to the
			///              point of causing damage to the database during rollback.
			///
			/// The above is for a rollback-journal mode.  For WAL mode, OFF continues
			/// to mean that no syncs ever occur.  NORMAL means that the WAL is synced
			/// prior to the start of checkpoint and that the database file is synced
			/// at the conclusion of the checkpoint if the entire content of the WAL
			/// was written back into the database.  But no sync operations occur for
			/// an ordinary commit in NORMAL mode with WAL.  FULL means that the WAL
			/// file is synced following each commit operation, in addition to the
			/// syncs associated with NORMAL.
			///
			/// Do not confuse synchronous=FULL with SQLITE_SYNC_FULL.  The
			/// SQLITE_SYNC_FULL macro means to use the MacOSX-style full-fsync
			/// using fcntl(F_FULLFSYNC).  SQLITE_SYNC_NORMAL means to do an
			/// ordinary fsync() call.  There is no difference between SQLITE_SYNC_FULL
			/// and SQLITE_SYNC_NORMAL on platforms other than MacOSX.  But the
			/// synchronous=FULL versus synchronous=NORMAL setting determines when
			/// the xSync primitive is called and is relevant to all platforms.
			///
			/// Numeric values associated with these states are OFF==1, NORMAL=2,
			/// and FULL=3.
			///
			///</summary>
			#if !SQLITE_OMIT_PAGER_PRAGMAS
			void sqlite3PagerSetSafetyLevel (///
///<summary>
///The pager to set safety level for 
///</summary>

			int level, ///
///<summary>
///PRAGMA synchronous.  1=OFF, 2=NORMAL, 3=FULL 
///</summary>

			int bFullFsync, ///
///<summary>
///PRAGMA fullfsync 
///</summary>

			int bCkptFullFsync///
///<summary>
///PRAGMA checkpoint_fullfsync 
///</summary>

			)
			{
				Debug.Assert (level >= 1 && level <= 3);
				this.noSync = (level == 1 || this.tempFile);
				this.fullSync = (level == 3 && !this.tempFile);
				if (this.noSync) {
					this.syncFlags = 0;
					this.ckptSyncFlags = 0;
				}
				else
					if (bFullFsync != 0) {
						this.syncFlags = SQLITE_SYNC_FULL;
						this.ckptSyncFlags = SQLITE_SYNC_FULL;
					}
					else
						if (bCkptFullFsync != 0) {
							this.syncFlags = SQLITE_SYNC_NORMAL;
							this.ckptSyncFlags = SQLITE_SYNC_FULL;
						}
						else {
							this.syncFlags = SQLITE_SYNC_NORMAL;
							this.ckptSyncFlags = SQLITE_SYNC_NORMAL;
						}
			}

			public///<summary>
			/// The following global variable is incremented whenever the library
			/// attempts to open a temporary file.  This information is used for
			/// testing and analysis only.
			///</summary>
			#if SQLITE_TEST
																																																			#if !TCLSH
																																																			    static int sqlite3_opentemp_count = 0;
#else
																																																			    static tcl.lang.Var.SQLITE3_GETSET sqlite3_opentemp_count = new tcl.lang.Var.SQLITE3_GETSET( "sqlite3_opentemp_count" );
#endif
																																																			#endif
			///<summary>
			/// Open a temporary file.
			///
			/// Write the file descriptor into *pFile. Return SQLITE_OK on success
			/// or some other error code if we fail. The OS will automatically
			/// delete the temporary file when it is closed.
			///
			/// The flags passed to the VFS layer xOpen() call are those specified
			/// by parameter vfsFlags ORed with the following:
			///
			///     SQLITE_OPEN_READWRITE
			///     SQLITE_OPEN_CREATE
			///     SQLITE_OPEN_EXCLUSIVE
			///     SQLITE_OPEN_DELETEONCLOSE
			///</summary>
			int pagerOpentemp (///
///<summary>
///The pager object 
///</summary>

			ref sqlite3_file pFile, ///
///<summary>
///Write the file descriptor here 
///</summary>

			int vfsFlags///
///<summary>
///Flags passed through to the VFS 
///</summary>

			)
			{
				int rc;
				///
///<summary>
///Return code 
///</summary>

				#if SQLITE_TEST
																																																																						#if !TCLSH
																																																																						      sqlite3_opentemp_count++;  /* Used for testing and analysis only */
#else
																																																																						      sqlite3_opentemp_count.iValue++;  /* Used for testing and analysis only */
#endif
																																																																						#endif
				vfsFlags |= SQLITE_OPEN_READWRITE | SQLITE_OPEN_CREATE | SQLITE_OPEN_EXCLUSIVE | SQLITE_OPEN_DELETEONCLOSE;
				int dummy = 0;
				rc = sqlite3OsOpen (this.pVfs, null, pFile, vfsFlags, ref dummy);
				Debug.Assert (rc != SQLITE_OK || isOpen (pFile));
				return rc;
			}

			public///<summary>
			/// Set the busy handler function.
			///
			/// The pager invokes the busy-handler if sqlite3OsLock() returns
			/// SQLITE_BUSY when trying to upgrade from no-lock to a SHARED lock,
			/// or when trying to upgrade from a RESERVED lock to an EXCLUSIVE
			/// lock. It does *not* invoke the busy handler when upgrading from
			/// SHARED to RESERVED, or when upgrading from SHARED to EXCLUSIVE
			/// (which occurs during hot-journal rollback). Summary:
			///
			///   Transition                        | Invokes xBusyHandler
			///   --------------------------------------------------------
			///   NO_LOCK       . SHARED_LOCK      | Yes
			///   SHARED_LOCK   . RESERVED_LOCK    | No
			///   SHARED_LOCK   . EXCLUSIVE_LOCK   | No
			///   RESERVED_LOCK . EXCLUSIVE_LOCK   | Yes
			///
			/// If the busy-handler callback returns non-zero, the lock is
			/// retried. If it returns zero, then the SQLITE_BUSY error is
			/// returned to the caller of the pager API function.
			///
			///</summary>
			void sqlite3PagerSetBusyhandler (///
///<summary>
///Pager object 
///</summary>

			dxBusyHandler xBusyHandler, ///
///<summary>
///</summary>
///<param name="Pointer to busy">handler function </param>

			//int (*xBusyHandler)(void *),
			object pBusyHandlerArg///
///<summary>
///Argument to pass to xBusyHandler 
///</summary>

			)
			{
				this.xBusyHandler = xBusyHandler;
				this.pBusyHandlerArg = pBusyHandlerArg;
			}

			public///<summary>
			/// Change the page size used by the Pager object. The new page size
			/// is passed in *pPageSize.
			///
			/// If the pager is in the error state when this function is called, it
			/// is a no-op. The value returned is the error state error code (i.e.
			/// one of SQLITE_IOERR, an SQLITE_IOERR_xxx sub-code or SQLITE_FULL).
			///
			/// Otherwise, if all of the following are true:
			///
			///   * the new page size (value of *pPageSize) is valid (a power
			///     of two between 512 and SQLITE_MAX_PAGE_SIZE, inclusive), and
			///
			///   * there are no outstanding page references, and
			///
			///   * the database is either not an in-memory database or it is
			///     an in-memory database that currently consists of zero pages.
			///
			/// then the pager object page size is set to *pPageSize.
			///
			/// If the page size is changed, then this function uses sqlite3PagerMalloc()
			/// to obtain a new Pager.pTmpSpace buffer. If this allocation attempt
			/// fails, SQLITE_NOMEM is returned and the page size remains unchanged.
			/// In all other cases, SQLITE_OK is returned.
			///
			/// If the page size is not changed, either because one of the enumerated
			/// conditions above is not true, the pager was in error state when this
			/// function was called, or because the memory allocation attempt failed,
			/// then *pPageSize is set to the old, retained page size before returning.
			///
			///</summary>
			int sqlite3PagerSetPagesize (ref u32 pPageSize, int nReserve)
			{
				int rc = SQLITE_OK;
				///
///<summary>
///It is not possible to do a full assert_pager_state() here, as this
///function may be called from within PagerOpen(), before the state
///of the Pager object is internally consistent.
///
///At one point this function returned an error if the pager was in 
///PAGER_ERROR state. But since PAGER_ERROR state guarantees that
///there is at least one outstanding page reference, this function
///</summary>
///<param name="is a no">op for that case anyhow.</param>
///<param name=""></param>

				u32 pageSize = pPageSize;
				Debug.Assert (pageSize == 0 || (pageSize >= 512 && pageSize <= SQLITE_MAX_PAGE_SIZE));
				if ((this.memDb == 0 || this.dbSize == 0) && sqlite3PcacheRefCount (this.pPCache) == 0 && pageSize != 0 && pageSize != (u32)this.pageSize) {
					//char *pNew = NULL;             /* New temp space */
					i64 nByte = 0;
					if (this.eState > PAGER_OPEN && isOpen (this.fd)) {
						rc = sqlite3OsFileSize (this.fd, ref nByte);
					}
					//if ( rc == SQLITE_OK )
					//{
					//pNew = (char *)sqlite3PageMalloc(pageSize);
					//if( !pNew ) rc = SQLITE_NOMEM;
					//}
					if (rc == SQLITE_OK) {
						this.pager_reset ();
						this.dbSize = (Pgno)(nByte / pageSize);
						this.pageSize = (int)pageSize;
						sqlite3PageFree (ref this.pTmpSpace);
						this.pTmpSpace = sqlite3Malloc (pageSize);
						// pNew;
						sqlite3PcacheSetPageSize (this.pPCache, (int)pageSize);
					}
				}
				pPageSize = (u32)this.pageSize;
				if (rc == SQLITE_OK) {
					if (nReserve < 0)
						nReserve = this.nReserve;
					Debug.Assert (nReserve >= 0 && nReserve < 1000);
					this.nReserve = (i16)nReserve;
					this.pagerReportSize ();
				}
				return rc;
			}

			public///<summary>
			/// Return a pointer to the "temporary page" buffer held internally
			/// by the pager.  This is a buffer that is big enough to hold the
			/// entire content of a database page.  This buffer is used internally
			/// during rollback and will be overwritten whenever a rollback
			/// occurs.  But other modules are free to use it too, as long as
			/// no rollbacks are happening.
			///
			///</summary>
			byte[] sqlite3PagerTempSpace ()
			{
				return this.pTmpSpace;
			}

			public Pgno sqlite3PagerMaxPageCount (int mxPage)
			{
				if (mxPage > 0) {
					this.mxPgno = (Pgno)mxPage;
				}
				Debug.Assert (this.eState != PAGER_OPEN);
				///
///<summary>
///Called only by OP_MaxPgcnt 
///</summary>

				Debug.Assert (this.mxPgno >= this.dbSize);
				///
///<summary>
///OP_MaxPgcnt enforces this 
///</summary>

				return this.mxPgno;
			}

			public///<summary>
			/// The following set of routines are used to disable the simulated
			/// I/O error mechanism.  These routines are used to avoid simulated
			/// errors in places where we do not care about errors.
			///
			/// Unless -DSQLITE_TEST=1 is used, these routines are all no-ops
			/// and generate no code.
			///
			///</summary>
			#if SQLITE_TEST
																																																			    //extern int sqlite3_io_error_pending;
    //extern int sqlite3_io_error_hit;
    static int saved_cnt;
    static void disable_simulated_io_errors()
    {
#if !TCLSH
																																																			      saved_cnt = sqlite3_io_error_pending;
      sqlite3_io_error_pending = -1;
#else
																																																			      saved_cnt = sqlite3_io_error_pending.iValue;
      sqlite3_io_error_pending.iValue = -1;
#endif
																																																			    }

    static void enable_simulated_io_errors()
    {
#if !TCLSH
																																																			      sqlite3_io_error_pending = saved_cnt;
#else
																																																			      sqlite3_io_error_pending.iValue = saved_cnt;
#endif
																																																			    }
#else
			//# define disable_simulated_io_errors()
			//# define enable_simulated_io_errors()
			#endif
			///<summary>
			/// Read the first N bytes from the beginning of the file into memory
			/// that pDest points to.
			///
			/// If the pager was opened on a transient file (zFilename==""), or
			/// opened on a file less than N bytes in size, the output buffer is
			/// zeroed and SQLITE_OK returned. The rationale for this is that this
			/// function is used to read database headers, and a new transient or
			/// zero sized database has a header than consists entirely of zeroes.
			///
			/// If any IO error apart from SQLITE_IOERR_SHORT_READ is encountered,
			/// the error code is returned to the caller and the contents of the
			/// output buffer undefined.
			///</summary>
			int sqlite3PagerReadFileheader (int N, byte[] pDest)
			{
				int rc = SQLITE_OK;
				Array.Clear (pDest, 0, N);
				//memset(pDest, 0, N);
				Debug.Assert (isOpen (this.fd) || this.tempFile);
				///
///<summary>
///This routine is only called by btree immediately after creating
///the Pager object.  There has not been an opportunity to transition
///to WAL mode yet.
///
///</summary>

				Debug.Assert (!this.pagerUseWal ());
				if (isOpen (this.fd)) {
					IOTRACE ("DBHDR %p 0 %d\n", this, N);
					rc = sqlite3OsRead (this.fd, pDest, N, 0);
					if (rc == SQLITE_IOERR_SHORT_READ) {
						rc = SQLITE_OK;
					}
				}
				return rc;
			}

			public///<summary>
			/// This function may only be called when a read-transaction is open on
			/// the pager. It returns the total number of pages in the database.
			///
			/// However, if the file is between 1 and <page-size> bytes in size, then
			/// this is considered a 1 page file.
			///
			///</summary>
			void sqlite3PagerPagecount (out Pgno pnPage)
			{
				Debug.Assert (this.eState >= PAGER_READER);
				Debug.Assert (this.eState != PAGER_WRITER_FINISHED);
				pnPage = this.dbSize;
			}

			public int pager_wait_on_lock (int locktype)
			{
				int rc;
				///
///<summary>
///Return code 
///</summary>

				///
///<summary>
///</summary>
///<param name="Check that this is either a no">op (because the requested lock is</param>
///<param name="already held, or one of the transistions that the busy">handler</param>
///<param name="may be invoked during, according to the comment above">may be invoked during, according to the comment above</param>
///<param name="sqlite3PagerSetBusyhandler().">sqlite3PagerSetBusyhandler().</param>
///<param name=""></param>

				Debug.Assert ((this.eLock >= locktype) || (this.eLock == NO_LOCK && locktype == SHARED_LOCK) || (this.eLock == RESERVED_LOCK && locktype == EXCLUSIVE_LOCK));
				do {
					rc = this.pagerLockDb (locktype);
				}
				while (rc == SQLITE_BUSY && this.xBusyHandler (this.pBusyHandlerArg) != 0);
				return rc;
			}

			public void assertTruncateConstraint ()
			{
			}

			public///<summary>
			/// Truncate the in-memory database file image to nPage pages. This
			/// function does not actually modify the database file on disk. It
			/// just sets the internal state of the pager object so that the
			/// truncation will be done when the current transaction is committed.
			///</summary>
			void sqlite3PagerTruncateImage (u32 nPage)
			{
				Debug.Assert (this.dbSize >= nPage);
				Debug.Assert (this.eState >= PAGER_WRITER_CACHEMOD);
				this.dbSize = nPage;
				this.assertTruncateConstraint ();
			}

			public///<summary>
			/// This function is called before attempting a hot-journal rollback. It
			/// syncs the journal file to disk, then sets pPager.journalHdr to the
			/// size of the journal file so that the pager_playback() routine knows
			/// that the entire journal file has been synced.
			///
			/// Syncing a hot-journal to disk before attempting to roll it back ensures
			/// that if a power-failure occurs during the rollback, the process that
			/// attempts rollback following system recovery sees the same journal
			/// content as this process.
			///
			/// If everything goes as planned, SQLITE_OK is returned. Otherwise,
			/// an SQLite error code.
			///
			///</summary>
			int pagerSyncHotJournal ()
			{
				int rc = SQLITE_OK;
				if (!this.noSync) {
					rc = sqlite3OsSync (this.jfd, SQLITE_SYNC_NORMAL);
				}
				if (rc == SQLITE_OK) {
					rc = sqlite3OsFileSize (this.jfd, ref this.journalHdr);
				}
				return rc;
			}

			public///<summary>
			/// Shutdown the page cache.  Free all memory and close all files.
			///
			/// If a transaction was in progress when this routine is called, that
			/// transaction is rolled back.  All outstanding pages are invalidated
			/// and their memory is freed.  Any attempt to use a page associated
			/// with this page cache after this function returns will likely
			/// result in a coredump.
			///
			/// This function always succeeds. If a transaction is active an attempt
			/// is made to roll it back. If an error occurs during the rollback
			/// a hot journal may be left in the filesystem but no error is returned
			/// to the caller.
			///
			///</summary>
			int sqlite3PagerClose ()
			{
				u8[] pTmp = this.pTmpSpace;
				#if SQLITE_TEST
																																																																						      disable_simulated_io_errors();
#endif
				sqlite3BeginBenignMalloc ();
				///
///<summary>
///pPager.errCode = 0; 
///</summary>

				this.exclusiveMode = false;
				#if !SQLITE_OMIT_WAL
																																																																						sqlite3WalClose(pPager->pWal, pPager->ckptSyncFlags, pPager->pageSize, pTmp);
pPager.pWal = 0;
#endif
				this.pager_reset ();
				if (
				#if SQLITE_OMIT_MEMORYDB
																																																																						1==MEMDB
#else
				1 == this.memDb
				#endif
				) {
					this.pager_unlock ();
				}
				else {
					///
///<summary>
///If it is open, sync the journal file before calling UnlockAndRollback.
///If this is not done, then an unsynced portion of the open journal 
///file may be played back into the database. If a power failure occurs 
///while this is happening, the database could become corrupt.
///
///If an error occurs while trying to sync the journal, shift the pager
///into the ERROR state. This causes UnlockAndRollback to unlock the
///database and close the journal file without attempting to roll it
///</summary>
///<param name="back or finalize it. The next database user will have to do hot">journal</param>
///<param name="rollback before accessing the database file.">rollback before accessing the database file.</param>
///<param name=""></param>

					if (isOpen (this.jfd)) {
						this.pager_error (this.pagerSyncHotJournal ());
					}
					this.pagerUnlockAndRollback ();
				}
				sqlite3EndBenignMalloc ();
				#if SQLITE_TEST
																																																																						      enable_simulated_io_errors();
#endif
				PAGERTRACE ("CLOSE %d\n", PAGERID (this));
				IOTRACE ("CLOSE %p\n", this);
				sqlite3OsClose (this.jfd);
				sqlite3OsClose (this.fd);
				//sqlite3_free( ref pTmp );
				sqlite3PcacheClose (this.pPCache);
				#if SQLITE_HAS_CODEC
				if (this.xCodecFree != null)
					this.xCodecFree (ref this.pCodec);
				#endif
				Debug.Assert (null == this.aSavepoint && !this.pInJournal);
				Debug.Assert (!isOpen (this.jfd) && !isOpen (this.sjfd));
				//sqlite3_free( ref pPager );
				return SQLITE_OK;
			}

			public///<summary>
			/// Sync the journal. In other words, make sure all the pages that have
			/// been written to the journal have actually reached the surface of the
			/// disk and can be restored in the event of a hot-journal rollback.
			///
			/// If the Pager.noSync flag is set, then this function is a no-op.
			/// Otherwise, the actions required depend on the journal-mode and the
			/// device characteristics of the the file-system, as follows:
			///
			///   * If the journal file is an in-memory journal file, no action need
			///     be taken.
			///
			///   * Otherwise, if the device does not support the SAFE_APPEND property,
			///     then the nRec field of the most recently written journal header
			///     is updated to contain the number of journal records that have
			///     been written following it. If the pager is operating in full-sync
			///     mode, then the journal file is synced before this field is updated.
			///
			///   * If the device does not support the SEQUENTIAL property, then
			///     journal file is synced.
			///
			/// Or, in pseudo-code:
			///
			///   if( NOT <in-memory journal> ){
			///     if( NOT SAFE_APPEND ){
			///       if( <full-sync mode> ) xSync(<journal file>);
			///       <update nRec field>
			///     }
			///     if( NOT SEQUENTIAL ) xSync(<journal file>);
			///   }
			///
			/// If successful, this routine clears the PGHDR_NEED_SYNC flag of every
			/// page currently held in memory before returning SQLITE_OK. If an IO
			/// error is encountered, then the IO error code is returned to the caller.
			///
			///</summary>
			int syncJournal (int newHdr)
			{
				int rc = SQLITE_OK;
				Debug.Assert (this.eState == PAGER_WRITER_CACHEMOD || this.eState == PAGER_WRITER_DBMOD);
				Debug.Assert (this.assert_pager_state ());
				Debug.Assert (!this.pagerUseWal ());
				rc = this.sqlite3PagerExclusiveLock ();
				if (rc != SQLITE_OK)
					return rc;
				if (!this.noSync) {
					Debug.Assert (!this.tempFile);
					if (isOpen (this.jfd) && this.journalMode != PAGER_JOURNALMODE_MEMORY) {
						int iDc = sqlite3OsDeviceCharacteristics (this.fd);
						Debug.Assert (isOpen (this.jfd));
						if (0 == (iDc & SQLITE_IOCAP_SAFE_APPEND)) {
							///
///<summary>
///This block deals with an obscure problem. If the last connection
///</summary>
///<param name="that wrote to this database was operating in persistent">journal</param>
///<param name="mode, then the journal file may at this point actually be larger">mode, then the journal file may at this point actually be larger</param>
///<param name="than Pager.journalOff bytes. If the next thing in the journal">than Pager.journalOff bytes. If the next thing in the journal</param>
///<param name="file happens to be a journal">header (written as part of the</param>
///<param name="previous connection's transaction), and a crash or power">failure</param>
///<param name="occurs after nRec is updated but before this connection writes">occurs after nRec is updated but before this connection writes</param>
///<param name="anything else to the journal file (or commits/rolls back its">anything else to the journal file (or commits/rolls back its</param>
///<param name="transaction), then SQLite may become confused when doing the">transaction), then SQLite may become confused when doing the</param>
///<param name="hot">journal rollback following recovery. It may roll back all</param>
///<param name="of this connections data, then proceed to rolling back the old,">of this connections data, then proceed to rolling back the old,</param>
///<param name="out">date data that follows it. Database corruption.</param>
///<param name=""></param>
///<param name="To work around this, if the journal file does appear to contain">To work around this, if the journal file does appear to contain</param>
///<param name="a valid header following Pager.journalOff, then write a 0x00">a valid header following Pager.journalOff, then write a 0x00</param>
///<param name="byte to the start of it to prevent it from being recognized.">byte to the start of it to prevent it from being recognized.</param>
///<param name=""></param>
///<param name="Variable iNextHdrOffset is set to the offset at which this">Variable iNextHdrOffset is set to the offset at which this</param>
///<param name="problematic header will occur, if it exists. aMagic is used">problematic header will occur, if it exists. aMagic is used</param>
///<param name="as a temporary buffer to inspect the first couple of bytes of">as a temporary buffer to inspect the first couple of bytes of</param>
///<param name="the potential journal header.">the potential journal header.</param>
///<param name=""></param>

							i64 iNextHdrOffset;
							u8[] aMagic = new u8[8];
							u8[] zHeader = new u8[aJournalMagic.Length + 4];
							aJournalMagic.CopyTo (zHeader, 0);
							// memcpy(zHeader, aJournalMagic, sizeof(aJournalMagic));
							put32bits (zHeader, aJournalMagic.Length, this.nRec);
							iNextHdrOffset = this.journalHdrOffset ();
							rc = sqlite3OsRead (this.jfd, aMagic, 8, iNextHdrOffset);
							if (rc == SQLITE_OK && 0 == memcmp (aMagic, aJournalMagic, 8)) {
								u8[] zerobyte = new u8[1];
								rc = sqlite3OsWrite (this.jfd, zerobyte, 1, iNextHdrOffset);
							}
							if (rc != SQLITE_OK && rc != SQLITE_IOERR_SHORT_READ) {
								return rc;
							}
							///
///<summary>
///Write the nRec value into the journal file header. If in
///</summary>
///<param name="full">synchronous mode, sync the journal first. This ensures that</param>
///<param name="all data has really hit the disk before nRec is updated to mark">all data has really hit the disk before nRec is updated to mark</param>
///<param name="it as a candidate for rollback.">it as a candidate for rollback.</param>
///<param name=""></param>
///<param name="This is not required if the persistent media supports the">This is not required if the persistent media supports the</param>
///<param name="SAFE_APPEND property. Because in this case it is not possible">SAFE_APPEND property. Because in this case it is not possible</param>
///<param name="for garbage data to be appended to the file, the nRec field">for garbage data to be appended to the file, the nRec field</param>
///<param name="is populated with 0xFFFFFFFF when the journal header is written">is populated with 0xFFFFFFFF when the journal header is written</param>
///<param name="and never needs to be updated.">and never needs to be updated.</param>
///<param name=""></param>

							if (this.fullSync && 0 == (iDc & SQLITE_IOCAP_SEQUENTIAL)) {
								PAGERTRACE ("SYNC journal of %d\n", PAGERID (this));
								IOTRACE ("JSYNC %p\n", this);
								rc = sqlite3OsSync (this.jfd, this.syncFlags);
								if (rc != SQLITE_OK)
									return rc;
							}
							IOTRACE ("JHDR %p %lld\n", this, this.journalHdr);
							rc = sqlite3OsWrite (this.jfd, zHeader, zHeader.Length, this.journalHdr);
							if (rc != SQLITE_OK)
								return rc;
						}
						if (0 == (iDc & SQLITE_IOCAP_SEQUENTIAL)) {
							PAGERTRACE ("SYNC journal of %d\n", PAGERID (this));
							IOTRACE ("JSYNC %p\n", this);
							rc = sqlite3OsSync (this.jfd, this.syncFlags | (this.syncFlags == SQLITE_SYNC_FULL ? SQLITE_SYNC_DATAONLY : 0));
							if (rc != SQLITE_OK)
								return rc;
						}
						this.journalHdr = this.journalOff;
						if (newHdr != 0 && 0 == (iDc & SQLITE_IOCAP_SAFE_APPEND)) {
							this.nRec = 0;
							rc = this.writeJournalHdr ();
							if (rc != SQLITE_OK)
								return rc;
						}
					}
					else {
						this.journalHdr = this.journalOff;
					}
				}
				///
///<summary>
///Unless the pager is in noSync mode, the journal file was just 
///successfully synced. Either way, clear the PGHDR_NEED_SYNC flag on 
///all pages.
///
///</summary>

				sqlite3PcacheClearSyncFlags (this.pPCache);
				this.eState = PAGER_WRITER_DBMOD;
				Debug.Assert (this.assert_pager_state ());
				return SQLITE_OK;
			}

			public///<summary>
			/// The argument is the first in a linked list of dirty pages connected
			/// by the PgHdr.pDirty pointer. This function writes each one of the
			/// in-memory pages in the list to the database file. The argument may
			/// be NULL, representing an empty list. In this case this function is
			/// a no-op.
			///
			/// The pager must hold at least a RESERVED lock when this function
			/// is called. Before writing anything to the database file, this lock
			/// is upgraded to an EXCLUSIVE lock. If the lock cannot be obtained,
			/// SQLITE_BUSY is returned and no data is written to the database file.
			///
			/// If the pager is a temp-file pager and the actual file-system file
			/// is not yet open, it is created and opened before any data is
			/// written out.
			///
			/// Once the lock has been upgraded and, if necessary, the file opened,
			/// the pages are written out to the database file in list order. Writing
			/// a page is skipped if it meets either of the following criteria:
			///
			///   * The page number is greater than Pager.dbSize, or
			///   * The PGHDR_DONT_WRITE flag is set on the page.
			///
			/// If writing out a page causes the database file to grow, Pager.dbFileSize
			/// is updated accordingly. If page 1 is written out, then the value cached
			/// in Pager.dbFileVers[] is updated to match the new value stored in
			/// the database file.
			///
			/// If everything is successful, SQLITE_OK is returned. If an IO error
			/// occurs, an IO error code is returned. Or, if the EXCLUSIVE lock cannot
			/// be obtained, SQLITE_BUSY is returned.
			///
			///</summary>
			int pager_write_pagelist (PgHdr pList)
			{
				int rc = SQLITE_OK;
				///
///<summary>
///Return code 
///</summary>

				///
///<summary>
///This function is only called for rollback pagers in WRITER_DBMOD state. 
///</summary>

				Debug.Assert (!this.pagerUseWal ());
				Debug.Assert (this.eState == PAGER_WRITER_DBMOD);
				Debug.Assert (this.eLock == EXCLUSIVE_LOCK);
				///
///<summary>
///</summary>
///<param name="If the file is a temp">file has not yet been opened, open it now. It</param>
///<param name="is not possible for rc to be other than SQLITE_OK if this branch">is not possible for rc to be other than SQLITE_OK if this branch</param>
///<param name="is taken, as pager_wait_on_lock() is a no">files.</param>
///<param name=""></param>

				if (!isOpen (this.fd)) {
					Debug.Assert (this.tempFile && rc == SQLITE_OK);
					rc = this.pagerOpentemp (ref this.fd, (int)this.vfsFlags);
				}
				///
///<summary>
///Before the first write, give the VFS a hint of what the final
///file size will be.
///
///</summary>

				Debug.Assert (rc != SQLITE_OK || isOpen (this.fd));
				if (rc == SQLITE_OK && this.dbSize > this.dbHintSize) {
					sqlite3_int64 szFile = this.pageSize * (sqlite3_int64)this.dbSize;
					sqlite3OsFileControl (this.fd, SQLITE_FCNTL_SIZE_HINT, ref szFile);
					this.dbHintSize = this.dbSize;
				}
				while (rc == SQLITE_OK && pList) {
					Pgno pgno = pList.pgno;
					///
///<summary>
///If there are dirty pages in the page cache with page numbers greater
///than Pager.dbSize, this means sqlite3PagerTruncateImage() was called to
///</summary>
///<param name="make the file smaller (presumably by auto">vacuum code). Do not write</param>
///<param name="any such pages to the file.">any such pages to the file.</param>
///<param name=""></param>
///<param name="Also, do not write out any page that has the PGHDR_DONT_WRITE flag">Also, do not write out any page that has the PGHDR_DONT_WRITE flag</param>
///<param name="set (set by sqlite3PagerDontWrite()).">set (set by sqlite3PagerDontWrite()).</param>
///<param name=""></param>

					if (pList.pgno <= this.dbSize && 0 == (pList.flags & PGHDR_DONT_WRITE)) {
						i64 offset = (pList.pgno - 1) * (i64)this.pageSize;
						///
///<summary>
///Offset to write 
///</summary>

						byte[] pData = null;
						///
///<summary>
///Data to write 
///</summary>

						Debug.Assert ((pList.flags & PGHDR_NEED_SYNC) == 0);
						if (pList.pgno == 1)
							pager_write_changecounter (pList);
						///
///<summary>
///Encode the database 
///</summary>

						if (CODEC2 (this, pList.pData, pgno, SQLITE_ENCRYPT_WRITE_CTX, ref pData))
							return SQLITE_NOMEM;
						//     CODEC2(pPager, pList.pData, pgno, 6, return SQLITE_NOMEM, pData);
						///
///<summary>
///Write out the page data. 
///</summary>

						rc = sqlite3OsWrite (this.fd, pData, this.pageSize, offset);
						///
///<summary>
///If page 1 was just written, update Pager.dbFileVers to match
///the value now stored in the database file. If writing this
///page caused the database file to grow, update dbFileSize.
///
///</summary>

						if (pgno == 1) {
							Buffer.BlockCopy (pData, 24, this.dbFileVers, 0, this.dbFileVers.Length);
							// memcpy(pPager.dbFileVers, pData[24], pPager.dbFileVers).Length;
						}
						if (pgno > this.dbFileSize) {
							this.dbFileSize = pgno;
						}
						///
///<summary>
///Update any backup objects copying the contents of this pager. 
///</summary>

						this.pBackup.sqlite3BackupUpdate (pgno, pList.pData);
						PAGERTRACE ("STORE %d page %d hash(%08x)\n", PAGERID (this), pgno, pList.pager_pagehash ());
						IOTRACE ("PGOUT %p %d\n", this, pgno);
						#if SQLITE_TEST
																																																																																																												#if !TCLSH
																																																																																																												          PAGER_INCR( ref sqlite3_pager_writedb_count );
#else
																																																																																																												          int iValue;
          iValue = sqlite3_pager_writedb_count.iValue;
          PAGER_INCR( ref iValue );
          sqlite3_pager_writedb_count.iValue = iValue;
#endif
																																																																																																												
          PAGER_INCR( ref pPager.nWrite );
#endif
					}
					else {
						PAGERTRACE ("NOSTORE %d page %d\n", PAGERID (this), pgno);
					}
					pList.pager_set_pagehash ();
					pList = pList.pDirty;
				}
				return rc;
			}

			public///<summary>
			/// Ensure that the sub-journal file is open. If it is already open, this
			/// function is a no-op.
			///
			/// SQLITE_OK is returned if everything goes according to plan. An
			/// SQLITE_IOERR_XXX error code is returned if a call to sqlite3OsOpen()
			/// fails.
			///
			///</summary>
			int openSubJournal ()
			{
				int rc = SQLITE_OK;
				if (!isOpen (this.sjfd)) {
					if (this.journalMode == PAGER_JOURNALMODE_MEMORY || this.subjInMemory != 0) {
						sqlite3MemJournalOpen (this.sjfd);
					}
					else {
						rc = this.pagerOpentemp (ref this.sjfd, SQLITE_OPEN_SUBJOURNAL);
					}
				}
				return rc;
			}

			public///<summary>
			/// This function is called after transitioning from PAGER_UNLOCK to
			/// PAGER_SHARED state. It tests if there is a hot journal present in
			/// the file-system for the given pager. A hot journal is one that
			/// needs to be played back. According to this function, a hot-journal
			/// file exists if the following criteria are met:
			///
			///   * The journal file exists in the file system, and
			///   * No process holds a RESERVED or greater lock on the database file, and
			///   * The database file itself is greater than 0 bytes in size, and
			///   * The first byte of the journal file exists and is not 0x00.
			///
			/// If the current size of the database file is 0 but a journal file
			/// exists, that is probably an old journal left over from a prior
			/// database with the same name. In this case the journal file is
			/// just deleted using OsDelete, *pExists is set to 0 and SQLITE_OK
			/// is returned.
			///
			/// This routine does not check if there is a master journal filename
			/// at the end of the file. If there is, and that master journal file
			/// does not exist, then the journal file is not really hot. In this
			/// case this routine will return a false-positive. The pager_playback()
			/// routine will discover that the journal file is not really hot and
			/// will not roll it back.
			///
			/// If a hot-journal file is found to exist, *pExists is set to 1 and
			/// SQLITE_OK returned. If no hot-journal file is present, *pExists is
			/// set to 0 and SQLITE_OK returned. If an IO error occurs while trying
			/// to determine whether or not a hot-journal file exists, the IO error
			/// code is returned and the value of *pExists is undefined.
			///
			///</summary>
			int hasHotJournal (ref int pExists)
			{
				sqlite3_vfs pVfs = this.pVfs;
				int rc = SQLITE_OK;
				///
///<summary>
///Return code 
///</summary>

				int exists = 1;
				///
///<summary>
///True if a journal file is present 
///</summary>

				int jrnlOpen = isOpen (this.jfd) ? 1 : 0;
				Debug.Assert (this.useJournal != 0);
				Debug.Assert (isOpen (this.fd));
				Debug.Assert (this.eState == PAGER_OPEN);
				Debug.Assert (jrnlOpen == 0 || (sqlite3OsDeviceCharacteristics (this.jfd) & SQLITE_IOCAP_UNDELETABLE_WHEN_OPEN) != 0);
				pExists = 0;
				if (0 == jrnlOpen) {
					rc = sqlite3OsAccess (pVfs, this.zJournal, SQLITE_ACCESS_EXISTS, ref exists);
				}
				if (rc == SQLITE_OK && exists != 0) {
					int locked = 0;
					///
///<summary>
///True if some process holds a RESERVED lock 
///</summary>

					///
///<summary>
///Race condition here:  Another process might have been holding the
///the RESERVED lock and have a journal open at the sqlite3OsAccess()
///call above, but then delete the journal and drop the lock before
///we get to the following sqlite3OsCheckReservedLock() call.  If that
///is the case, this routine might think there is a hot journal when
///</summary>
///<param name="in fact there is none.  This results in a false">positive which will</param>
///<param name="be dealt with by the playback routine.  Ticket #3883.">be dealt with by the playback routine.  Ticket #3883.</param>
///<param name=""></param>

					rc = sqlite3OsCheckReservedLock (this.fd, ref locked);
					if (rc == SQLITE_OK && locked == 0) {
						Pgno nPage = 0;
						///
///<summary>
///Number of pages in database file 
///</summary>

						///
///<summary>
///Check the size of the database file. If it consists of 0 pages,
///then delete the journal file. See the header comment above for
///the reasoning here.  Delete the obsolete journal file under
///a RESERVED lock to avoid race conditions and to avoid violating
///[H33020].
///
///</summary>

						rc = this.pagerPagecount (ref nPage);
						if (rc == SQLITE_OK) {
							if (nPage == 0) {
								sqlite3BeginBenignMalloc ();
								if (this.pagerLockDb (RESERVED_LOCK) == SQLITE_OK) {
									sqlite3OsDelete (pVfs, this.zJournal, 0);
									if (!this.exclusiveMode)
										this.pagerUnlockDb (SHARED_LOCK);
								}
								sqlite3EndBenignMalloc ();
							}
							else {
								///
///<summary>
///The journal file exists and no other connection has a reserved
///or greater lock on the database file. Now check that there is
///</summary>
///<param name="at least one non">zero bytes at the start of the journal file.</param>
///<param name="If there is, then we consider this journal to be hot. If not,">If there is, then we consider this journal to be hot. If not,</param>
///<param name="it can be ignored.">it can be ignored.</param>
///<param name=""></param>

								if (0 == jrnlOpen) {
									int f = SQLITE_OPEN_READONLY | SQLITE_OPEN_MAIN_JOURNAL;
									rc = sqlite3OsOpen (pVfs, this.zJournal, this.jfd, f, ref f);
								}
								if (rc == SQLITE_OK) {
									u8[] first = new u8[1];
									rc = sqlite3OsRead (this.jfd, first, 1, 0);
									if (rc == SQLITE_IOERR_SHORT_READ) {
										rc = SQLITE_OK;
									}
									if (0 == jrnlOpen) {
										sqlite3OsClose (this.jfd);
									}
									pExists = (first [0] != 0) ? 1 : 0;
								}
								else
									if (rc == SQLITE_CANTOPEN) {
										///
///<summary>
///If we cannot open the rollback journal file in order to see if
///its has a zero header, that might be due to an I/O error, or
///it might be due to the race condition described above and in
///ticket #3883.  Either way, assume that the journal is hot.
///This might be a false positive.  But if it is, then the
///automatic journal playback and recovery mechanism will deal
///with it under an EXCLUSIVE lock where we do not need to
///worry so much with race conditions.
///
///</summary>

										pExists = 1;
										rc = SQLITE_OK;
									}
							}
						}
					}
				}
				return rc;
			}

			public///<summary>
			/// This function is called to obtain a shared lock on the database file.
			/// It is illegal to call sqlite3PagerAcquire() until after this function
			/// has been successfully called. If a shared-lock is already held when
			/// this function is called, it is a no-op.
			///
			/// The following operations are also performed by this function.
			///
			///   1) If the pager is currently in PAGER_OPEN state (no lock held
			///      on the database file), then an attempt is made to obtain a
			///      SHARED lock on the database file. Immediately after obtaining
			///      the SHARED lock, the file-system is checked for a hot-journal,
			///      which is played back if present. Following any hot-journal
			///      rollback, the contents of the cache are validated by checking
			///      the 'change-counter' field of the database file header and
			///      discarded if they are found to be invalid.
			///
			///   2) If the pager is running in exclusive-mode, and there are currently
			///      no outstanding references to any pages, and is in the error state,
			///      then an attempt is made to clear the error state by discarding
			///      the contents of the page cache and rolling back any open journal
			///      file.
			///
			/// If everything is successful, SQLITE_OK is returned. If an IO error
			/// occurs while locking the database, checking for a hot-journal file or
			/// rolling back a journal file, the IO error code is returned.
			///
			///</summary>
			int sqlite3PagerSharedLock ()
			{
				int rc = SQLITE_OK;
				///
///<summary>
///Return code 
///</summary>

				///
///<summary>
///</summary>
///<param name="This routine is only called from b">tree and only when there are no</param>
///<param name="outstanding pages. This implies that the pager state should either">outstanding pages. This implies that the pager state should either</param>
///<param name="be OPEN or READER. READER is only possible if the pager is or was in ">be OPEN or READER. READER is only possible if the pager is or was in </param>
///<param name="exclusive access mode.">exclusive access mode.</param>
///<param name=""></param>

				Debug.Assert (sqlite3PcacheRefCount (this.pPCache) == 0);
				Debug.Assert (this.assert_pager_state ());
				Debug.Assert (this.eState == PAGER_OPEN || this.eState == PAGER_READER);
				if (NEVER (
				#if SQLITE_OMIT_MEMORYDB
																																																																						0!=MEMDB
#else
				0 != this.memDb 
				#endif
				&& this.errCode != 0)) {
					return this.errCode;
				}
				if (!this.pagerUseWal () && this.eState == PAGER_OPEN) {
					int bHotJournal = 1;
					///
///<summary>
///</summary>
///<param name="True if there exists a hot journal">file </param>

					Debug.Assert (
					#if SQLITE_OMIT_MEMORYDB
																																																																																									0==MEMDB
#else
					0 == this.memDb
					#endif
					);
					Debug.Assert (this.noReadlock == 0 || this.readOnly);
					if (this.noReadlock == 0) {
						rc = this.pager_wait_on_lock (SHARED_LOCK);
						if (rc != SQLITE_OK) {
							Debug.Assert (this.eLock == NO_LOCK || this.eLock == UNKNOWN_LOCK);
							goto failed;
						}
					}
					///
///<summary>
///If a journal file exists, and there is no RESERVED lock on the
///database file, then it either needs to be played back or deleted.
///
///</summary>

					if (this.eLock <= SHARED_LOCK) {
						rc = this.hasHotJournal (ref bHotJournal);
					}
					if (rc != SQLITE_OK) {
						goto failed;
					}
					if (bHotJournal != 0) {
						///
///<summary>
///Get an EXCLUSIVE lock on the database file. At this point it is
///important that a RESERVED lock is not obtained on the way to the
///EXCLUSIVE lock. If it were, another process might open the
///database file, detect the RESERVED lock, and conclude that the
///database is safe to read while this process is still rolling the 
///</summary>
///<param name="hot">journal back.</param>
///<param name=""></param>
///<param name="Because the intermediate RESERVED lock is not requested, any">Because the intermediate RESERVED lock is not requested, any</param>
///<param name="other process attempting to access the database file will get to ">other process attempting to access the database file will get to </param>
///<param name="this point in the code and fail to obtain its own EXCLUSIVE lock ">this point in the code and fail to obtain its own EXCLUSIVE lock </param>
///<param name="on the database file.">on the database file.</param>
///<param name=""></param>
///<param name="Unless the pager is in locking_mode=exclusive mode, the lock is">Unless the pager is in locking_mode=exclusive mode, the lock is</param>
///<param name="downgraded to SHARED_LOCK before this function returns.">downgraded to SHARED_LOCK before this function returns.</param>
///<param name=""></param>

						rc = this.pagerLockDb (EXCLUSIVE_LOCK);
						if (rc != SQLITE_OK) {
							goto failed;
						}
						///
///<summary>
///If it is not already open and the file exists on disk, open the 
///journal for read/write access. Write access is required because 
///</summary>
///<param name="in exclusive">access mode the file descriptor will be kept open </param>
///<param name="and possibly used for a transaction later on. Also, write">access </param>
///<param name="is usually required to finalize the journal in journal_mode=persist ">is usually required to finalize the journal in journal_mode=persist </param>
///<param name="mode (and also for journal_mode=truncate on some systems).">mode (and also for journal_mode=truncate on some systems).</param>
///<param name=""></param>
///<param name="If the journal does not exist, it usually means that some ">If the journal does not exist, it usually means that some </param>
///<param name="other connection managed to get in and roll it back before ">other connection managed to get in and roll it back before </param>
///<param name="this connection obtained the exclusive lock above. Or, it ">this connection obtained the exclusive lock above. Or, it </param>
///<param name="may mean that the pager was in the error">state when this</param>
///<param name="function was called and the journal file does not exist.">function was called and the journal file does not exist.</param>
///<param name=""></param>

						if (!isOpen (this.jfd)) {
							sqlite3_vfs pVfs = this.pVfs;
							int bExists = 0;
							///
///<summary>
///True if journal file exists 
///</summary>

							rc = sqlite3OsAccess (pVfs, this.zJournal, SQLITE_ACCESS_EXISTS, ref bExists);
							if (rc == SQLITE_OK && bExists != 0) {
								int fout = 0;
								int f = SQLITE_OPEN_READWRITE | SQLITE_OPEN_MAIN_JOURNAL;
								Debug.Assert (!this.tempFile);
								rc = sqlite3OsOpen (pVfs, this.zJournal, this.jfd, f, ref fout);
								Debug.Assert (rc != SQLITE_OK || isOpen (this.jfd));
								if (rc == SQLITE_OK && (fout & SQLITE_OPEN_READONLY) != 0) {
									rc = SQLITE_CANTOPEN_BKPT ();
									sqlite3OsClose (this.jfd);
								}
							}
						}
						///
///<summary>
///Playback and delete the journal.  Drop the database write
///lock and reacquire the read lock. Purge the cache before
///</summary>
///<param name="playing back the hot">journal so that we don't end up with</param>
///<param name="an inconsistent cache.  Sync the hot journal before playing">an inconsistent cache.  Sync the hot journal before playing</param>
///<param name="it back since the process that crashed and left the hot journal">it back since the process that crashed and left the hot journal</param>
///<param name="probably did not sync it and we are required to always sync">probably did not sync it and we are required to always sync</param>
///<param name="the journal before playing it back.">the journal before playing it back.</param>
///<param name=""></param>

						if (isOpen (this.jfd)) {
							Debug.Assert (rc == SQLITE_OK);
							rc = this.pagerSyncHotJournal ();
							if (rc == SQLITE_OK) {
								rc = this.pager_playback (1);
								this.eState = PAGER_OPEN;
							}
						}
						else
							if (!this.exclusiveMode) {
								this.pagerUnlockDb (SHARED_LOCK);
							}
						if (rc != SQLITE_OK) {
							///
///<summary>
///This branch is taken if an error occurs while trying to open
///</summary>
///<param name="or roll back a hot">journal while holding an EXCLUSIVE lock. The</param>
///<param name="pager_unlock() routine will be called before returning to unlock">pager_unlock() routine will be called before returning to unlock</param>
///<param name="the file. If the unlock attempt fails, then Pager.eLock must be">the file. If the unlock attempt fails, then Pager.eLock must be</param>
///<param name="set to UNKNOWN_LOCK (see the comment above the #define for ">set to UNKNOWN_LOCK (see the comment above the #define for </param>
///<param name="UNKNOWN_LOCK above for an explanation). ">UNKNOWN_LOCK above for an explanation). </param>
///<param name=""></param>
///<param name="In order to get pager_unlock() to do this, set Pager.eState to">In order to get pager_unlock() to do this, set Pager.eState to</param>
///<param name="PAGER_ERROR now. This is not actually counted as a transition">PAGER_ERROR now. This is not actually counted as a transition</param>
///<param name="to ERROR state in the state diagram at the top of this file,">to ERROR state in the state diagram at the top of this file,</param>
///<param name="since we know that the same call to pager_unlock() will very">since we know that the same call to pager_unlock() will very</param>
///<param name="shortly transition the pager object to the OPEN state. Calling">shortly transition the pager object to the OPEN state. Calling</param>
///<param name="assert_pager_state() would fail now, as it should not be possible">assert_pager_state() would fail now, as it should not be possible</param>
///<param name="to be in ERROR state when there are zero outstanding page ">to be in ERROR state when there are zero outstanding page </param>
///<param name="references.">references.</param>
///<param name=""></param>

							this.pager_error (rc);
							goto failed;
						}
						Debug.Assert (this.eState == PAGER_OPEN);
						Debug.Assert ((this.eLock == SHARED_LOCK) || (this.exclusiveMode && this.eLock > SHARED_LOCK));
					}
					if (!this.tempFile && (this.pBackup != null || sqlite3PcachePagecount (this.pPCache) > 0)) {
						///
///<summary>
///</summary>
///<param name="The shared">lock has just been acquired on the database file</param>
///<param name="and there are already pages in the cache (from a previous">and there are already pages in the cache (from a previous</param>
///<param name="read or write transaction).  Check to see if the database">read or write transaction).  Check to see if the database</param>
///<param name="has been modified.  If the database has changed, flush the">has been modified.  If the database has changed, flush the</param>
///<param name="cache.">cache.</param>
///<param name=""></param>
///<param name="Database changes is detected by looking at 15 bytes beginning">Database changes is detected by looking at 15 bytes beginning</param>
///<param name="at offset 24 into the file.  The first 4 of these 16 bytes are">at offset 24 into the file.  The first 4 of these 16 bytes are</param>
///<param name="a 32">bit counter that is incremented with each change.  The</param>
///<param name="other bytes change randomly with each file change when">other bytes change randomly with each file change when</param>
///<param name="a codec is in use.">a codec is in use.</param>
///<param name=""></param>
///<param name="There is a vanishingly small chance that a change will not be ">There is a vanishingly small chance that a change will not be </param>
///<param name="detected.  The chance of an undetected change is so small that">detected.  The chance of an undetected change is so small that</param>
///<param name="it can be neglected.">it can be neglected.</param>
///<param name=""></param>

						Pgno nPage = 0;
						byte[] dbFileVers = new byte[this.dbFileVers.Length];
						rc = this.pagerPagecount (ref nPage);
						if (rc != 0)
							goto failed;
						if (nPage > 0) {
							IOTRACE ("CKVERS %p %d\n", this, dbFileVers.Length);
							rc = sqlite3OsRead (this.fd, dbFileVers, dbFileVers.Length, 24);
							if (rc != SQLITE_OK) {
								goto failed;
							}
						}
						else {
							Array.Clear (dbFileVers, 0, dbFileVers.Length);
							// memset( dbFileVers, 0, sizeof( dbFileVers ) );
						}
						if (memcmp (this.dbFileVers, dbFileVers, dbFileVers.Length) != 0) {
							this.pager_reset ();
						}
					}
					///
///<summary>
///</summary>
///<param name="If there is a WAL file in the file">system, open this database in WAL</param>
///<param name="mode. Otherwise, the following function call is a no">op.</param>
///<param name=""></param>

					rc = this.pagerOpenWalIfPresent ();
					#if !SQLITE_OMIT_WAL
																																																																																									Debug.Assert( pPager.pWal == null || rc == SQLITE_OK );
#endif
				}
				if (this.pagerUseWal ()) {
					Debug.Assert (rc == SQLITE_OK);
					rc = this.pagerBeginReadTransaction ();
				}
				if (this.eState == PAGER_OPEN && rc == SQLITE_OK) {
					rc = this.pagerPagecount (ref this.dbSize);
				}
				failed:
				if (rc != SQLITE_OK) {
					Debug.Assert (
					#if SQLITE_OMIT_MEMORYDB
																																																																																									0==MEMDB
#else
					0 == this.memDb
					#endif
					);
					this.pager_unlock ();
					Debug.Assert (this.eState == PAGER_OPEN);
				}
				else {
					this.eState = PAGER_READER;
				}
				return rc;
			}

			public void pagerUnlockIfUnused ()
			{
				if (sqlite3PcacheRefCount (this.pPCache) == 0) {
					this.pagerUnlockAndRollback ();
				}
			}

			public///<summary>
			/// Acquire a reference to page number pgno in pager pPager (a page
			/// reference has type DbPage*). If the requested reference is
			/// successfully obtained, it is copied to *ppPage and SQLITE_OK returned.
			///
			/// If the requested page is already in the cache, it is returned.
			/// Otherwise, a new page object is allocated and populated with data
			/// read from the database file. In some cases, the pcache module may
			/// choose not to allocate a new page object and may reuse an existing
			/// object with no outstanding references.
			///
			/// The extra data appended to a page is always initialized to zeros the
			/// first time a page is loaded into memory. If the page requested is
			/// already in the cache when this function is called, then the extra
			/// data is left as it was when the page object was last used.
			///
			/// If the database image is smaller than the requested page or if a
			/// non-zero value is passed as the noContent parameter and the
			/// requested page is not already stored in the cache, then no
			/// actual disk read occurs. In this case the memory image of the
			/// page is initialized to all zeros.
			///
			/// If noContent is true, it means that we do not care about the contents
			/// of the page. This occurs in two seperate scenarios:
			///
			///   a) When reading a free-list leaf page from the database, and
			///
			///   b) When a savepoint is being rolled back and we need to load
			///      a new page into the cache to be filled with the data read
			///      from the savepoint journal.
			///
			/// If noContent is true, then the data returned is zeroed instead of
			/// being read from the database. Additionally, the bits corresponding
			/// to pgno in Pager.pInJournal (bitvec of pages already written to the
			/// journal file) and the PagerSavepoint.pInSavepoint bitvecs of any open
			/// savepoints are set. This means if the page is made writable at any
			/// point in the future, using a call to sqlite3PagerWrite(), its contents
			/// will not be journaled. This saves IO.
			///
			/// The acquisition might fail for several reasons.  In all cases,
			/// an appropriate error code is returned and *ppPage is set to NULL.
			///
			/// See also sqlite3PagerLookup().  Both this routine and Lookup() attempt
			/// to find a page in the in-memory cache first.  If the page is not already
			/// in memory, this routine goes to disk to read it in whereas Lookup()
			/// just returns 0.  This routine acquires a read-lock the first time it
			/// has to go to disk, and could also playback an old journal if necessary.
			/// Since Lookup() never goes to disk, it never has to deal with locks
			/// or journal files.
			///
			///</summary>
			// Under C# from the header file
			//#define sqlite3PagerGet(A,B,C) sqlite3PagerAcquire(A,B,C,0)
			int sqlite3PagerGet (///
///<summary>
///The pager open on the database file 
///</summary>

			u32 pgno, ///
///<summary>
///Page number to fetch 
///</summary>

			ref DbPage ppPage///
///<summary>
///Write a pointer to the page here 
///</summary>

			)
			{
				return this.sqlite3PagerAcquire (pgno, ref ppPage, 0);
			}

			public int sqlite3PagerAcquire (///
///<summary>
///The pager open on the database file 
///</summary>

			u32 pgno, ///
///<summary>
///Page number to fetch 
///</summary>

			ref DbPage ppPage, ///
///<summary>
///Write a pointer to the page here 
///</summary>

			u8 noContent///
///<summary>
///Do not bother reading content from disk if true 
///</summary>

			)
			{
				int rc;
				PgHdr pPg = null;
				Debug.Assert (this.eState >= PAGER_READER);
				Debug.Assert (this.assert_pager_state ());
				if (pgno == 0) {
					return SQLITE_CORRUPT_BKPT ();
				}
				///
///<summary>
///If the pager is in the error state, return an error immediately. 
///Otherwise, request the page from the PCache layer. 
///</summary>

				if (this.errCode != SQLITE_OK) {
					rc = this.errCode;
				}
				else {
					rc = sqlite3PcacheFetch (this.pPCache, pgno, 1, ref ppPage);
				}
				if (rc != SQLITE_OK) {
					///
///<summary>
///Either the call to sqlite3PcacheFetch() returned an error or the
///</summary>
///<param name="pager was already in the error">state when this function was called.</param>
///<param name="Set pPg to 0 and jump to the exception handler.  ">Set pPg to 0 and jump to the exception handler.  </param>

					pPg = null;
					goto pager_acquire_err;
				}
				Debug.Assert ((ppPage).pgno == pgno);
				Debug.Assert ((ppPage).pPager == this || (ppPage).pPager == null);
				if ((ppPage).pPager != null && 0 == noContent) {
					///
///<summary>
///In this case the pcache already contains an initialized copy of
///the page. Return without further ado.  
///</summary>

					Debug.Assert (pgno <= PAGER_MAX_PGNO && pgno != PAGER_MJ_PGNO (this));
					PAGER_INCR (ref this.nHit);
					return SQLITE_OK;
				}
				else {
					///
///<summary>
///The pager cache has created a new page. Its content needs to 
///be initialized.  
///</summary>

					#if SQLITE_TEST
																																																																																									        PAGER_INCR( ref pPager.nMiss );
#endif
					pPg = ppPage;
					pPg.pPager = this;
					pPg.pExtra = new MemPage ();
					//memset(pPg.pExtra, 0, pPager.nExtra);
					///
///<summary>
///The maximum page number is 2^31. Return SQLITE_CORRUPT if a page
///</summary>
///<param name="number greater than this, or the unused locking">page, is requested. </param>

					if (pgno > PAGER_MAX_PGNO || pgno == PAGER_MJ_PGNO (this)) {
						rc = SQLITE_CORRUPT_BKPT ();
						goto pager_acquire_err;
					}
					if (
					#if SQLITE_OMIT_MEMORYDB
																																																																																									1==MEMDB
#else
					this.memDb != 0 
					#endif
					|| this.dbSize < pgno || noContent != 0 || !isOpen (this.fd)) {
						if (pgno > this.mxPgno) {
							rc = SQLITE_FULL;
							goto pager_acquire_err;
						}
						if (noContent != 0) {
							///
///<summary>
///</summary>
///<param name="Failure to set the bits in the InJournal bit">vectors is benign.</param>
///<param name="It merely means that we might do some extra work to journal a">It merely means that we might do some extra work to journal a</param>
///<param name="page that does not need to be journaled.  Nevertheless, be sure">page that does not need to be journaled.  Nevertheless, be sure</param>
///<param name="to test the case where a malloc error occurs while trying to set">to test the case where a malloc error occurs while trying to set</param>
///<param name="a bit in a bit vector.">a bit in a bit vector.</param>
///<param name=""></param>

							sqlite3BeginBenignMalloc ();
							if (pgno <= this.dbOrigSize) {
								#if !NDEBUG || SQLITE_COVERAGE_TEST
																																																																																																																																																		              rc = sqlite3BitvecSet( pPager.pInJournal, pgno );          //TESTONLY( rc = ) sqlite3BitvecSet(pPager.pInJournal, pgno);
#else
								sqlite3BitvecSet (this.pInJournal, pgno);
								#endif
								testcase (rc == SQLITE_NOMEM);
							}
							#if !NDEBUG || SQLITE_COVERAGE_TEST
																																																																																																																															            rc = addToSavepointBitvecs( pPager, pgno ); //TESTONLY( rc = ) addToSavepointBitvecs(pPager, pgno);
#else
							this.addToSavepointBitvecs (pgno);
							#endif
							testcase (rc == SQLITE_NOMEM);
							sqlite3EndBenignMalloc ();
						}
						//memset(pPg.pData, 0, pPager.pageSize);
						Array.Clear (pPg.pData, 0, this.pageSize);
						IOTRACE ("ZERO %p %d\n", this, pgno);
					}
					else {
						Debug.Assert (pPg.pPager == this);
						rc = readDbPage (pPg);
						if (rc != SQLITE_OK) {
							goto pager_acquire_err;
						}
					}
					pPg.pager_set_pagehash ();
				}
				return SQLITE_OK;
				pager_acquire_err:
				Debug.Assert (rc != SQLITE_OK);
				if (pPg != null) {
					sqlite3PcacheDrop (pPg);
				}
				this.pagerUnlockIfUnused ();
				ppPage = null;
				return rc;
			}

			public///<summary>
			/// Acquire a page if it is already in the in-memory cache.  Do
			/// not read the page from disk.  Return a pointer to the page,
			/// or 0 if the page is not in cache.
			///
			/// See also sqlite3PagerGet().  The difference between this routine
			/// and sqlite3PagerGet() is that _get() will go to the disk and read
			/// in the page if the page is not already in cache.  This routine
			/// returns NULL if the page is not in cache or if a disk I/O error
			/// has ever happened.
			///
			///</summary>
			DbPage sqlite3PagerLookup (u32 pgno)
			{
				PgHdr pPg = null;
				Debug.Assert (this != null);
				Debug.Assert (pgno != 0);
				Debug.Assert (this.pPCache != null);
				Debug.Assert (this.eState >= PAGER_READER && this.eState != PAGER_ERROR);
				sqlite3PcacheFetch (this.pPCache, pgno, 0, ref pPg);
				return pPg;
			}

			public///<summary>
			/// This function is called at the start of every write transaction.
			/// There must already be a RESERVED or EXCLUSIVE lock on the database
			/// file when this routine is called.
			///
			/// Open the journal file for pager pPager and write a journal header
			/// to the start of it. If there are active savepoints, open the sub-journal
			/// as well. This function is only used when the journal file is being
			/// opened to write a rollback log for a transaction. It is not used
			/// when opening a hot journal file to roll it back.
			///
			/// If the journal file is already open (as it may be in exclusive mode),
			/// then this function just writes a journal header to the start of the
			/// already open file.
			///
			/// Whether or not the journal file is opened by this function, the
			/// Pager.pInJournal bitvec structure is allocated.
			///
			/// Return SQLITE_OK if everything is successful. Otherwise, return
			/// SQLITE_NOMEM if the attempt to allocate Pager.pInJournal fails, or
			/// an IO error code if opening or writing the journal file fails.
			///
			///</summary>
			int pager_open_journal ()
			{
				int rc = SQLITE_OK;
				///
///<summary>
///Return code 
///</summary>

				sqlite3_vfs pVfs = this.pVfs;
				///
///<summary>
///Local cache of vfs pointer 
///</summary>

				Debug.Assert (this.eState == PAGER_WRITER_LOCKED);
				Debug.Assert (this.assert_pager_state ());
				Debug.Assert (this.pInJournal == null);
				///
///<summary>
///</summary>
///<param name="If already in the error state, this function is a no">op.  But on</param>
///<param name="the other hand, this routine is never called if we are already in">the other hand, this routine is never called if we are already in</param>
///<param name="an error state. ">an error state. </param>

				if (NEVER (this.errCode) != 0)
					return this.errCode;
				if (!this.pagerUseWal () && this.journalMode != PAGER_JOURNALMODE_OFF) {
					this.pInJournal = sqlite3BitvecCreate (this.dbSize);
					//if (pPager.pInJournal == null)
					//{
					//  return SQLITE_NOMEM;
					//}
					///
///<summary>
///Open the journal file if it is not already open. 
///</summary>

					if (!isOpen (this.jfd)) {
						if (this.journalMode == PAGER_JOURNALMODE_MEMORY) {
							sqlite3MemJournalOpen (this.jfd);
						}
						else {
							int flags = ///
///<summary>
///VFS flags to open journal file 
///</summary>

							SQLITE_OPEN_READWRITE | SQLITE_OPEN_CREATE | (this.tempFile ? (SQLITE_OPEN_DELETEONCLOSE | SQLITE_OPEN_TEMP_JOURNAL) : (SQLITE_OPEN_MAIN_JOURNAL));
							#if SQLITE_ENABLE_ATOMIC_WRITE
																																																																																																																															rc = sqlite3JournalOpen(
pVfs, pPager.zJournal, pPager.jfd, flags, jrnlBufferSize(pPager)
);
#else
							int int0 = 0;
							rc = sqlite3OsOpen (pVfs, this.zJournal, this.jfd, flags, ref int0);
							#endif
						}
						Debug.Assert (rc != SQLITE_OK || isOpen (this.jfd));
					}
					///
///<summary>
///Write the first journal header to the journal file and open
///</summary>
///<param name="the sub">journal if necessary.</param>
///<param name=""></param>

					if (rc == SQLITE_OK) {
						///
///<summary>
///TODO: Check if all of these are really required. 
///</summary>

						this.nRec = 0;
						this.journalOff = 0;
						this.setMaster = 0;
						this.journalHdr = 0;
						rc = this.writeJournalHdr ();
					}
				}
				if (rc != SQLITE_OK) {
					sqlite3BitvecDestroy (ref this.pInJournal);
					this.pInJournal = null;
				}
				else {
					Debug.Assert (this.eState == PAGER_WRITER_LOCKED);
					this.eState = PAGER_WRITER_CACHEMOD;
				}
				return rc;
			}

			public///<summary>
			/// Begin a write-transaction on the specified pager object. If a
			/// write-transaction has already been opened, this function is a no-op.
			///
			/// If the exFlag argument is false, then acquire at least a RESERVED
			/// lock on the database file. If exFlag is true, then acquire at least
			/// an EXCLUSIVE lock. If such a lock is already held, no locking
			/// functions need be called.
			///
			/// If the subjInMemory argument is non-zero, then any sub-journal opened
			/// within this transaction will be opened as an in-memory file. This
			/// has no effect if the sub-journal is already opened (as it may be when
			/// running in exclusive mode) or if the transaction does not require a
			/// sub-journal. If the subjInMemory argument is zero, then any required
			/// sub-journal is implemented in-memory if pPager is an in-memory database,
			/// or using a temporary file otherwise.
			///
			///</summary>
			int sqlite3PagerBegin (bool exFlag, int subjInMemory)
			{
				int rc = SQLITE_OK;
				if (this.errCode != 0)
					return this.errCode;
				Debug.Assert (this.eState >= PAGER_READER && this.eState < PAGER_ERROR);
				this.subjInMemory = (u8)subjInMemory;
				if (ALWAYS (this.eState == PAGER_READER)) {
					Debug.Assert (this.pInJournal == null);
					if (this.pagerUseWal ()) {
						///
///<summary>
///If the pager is configured to use locking_mode=exclusive, and an
///exclusive lock on the database is not already held, obtain it now.
///
///</summary>

						if (this.exclusiveMode && sqlite3WalExclusiveMode (this.pWal, -1)) {
							rc = this.pagerLockDb (EXCLUSIVE_LOCK);
							if (rc != SQLITE_OK) {
								return rc;
							}
							sqlite3WalExclusiveMode (this.pWal, 1);
						}
						///
///<summary>
///Grab the write lock on the log file. If successful, upgrade to
///PAGER_RESERVED state. Otherwise, return an error code to the caller.
///</summary>
///<param name="The busy">handler is not invoked if another connection already</param>
///<param name="holds the write">lock. If possible, the upper layer will call it.</param>
///<param name=""></param>

						rc = sqlite3WalBeginWriteTransaction (this.pWal);
					}
					else {
						///
///<summary>
///Obtain a RESERVED lock on the database file. If the exFlag parameter
///is true, then immediately upgrade this to an EXCLUSIVE lock. The
///</summary>
///<param name="busy">handler callback can be used when upgrading to the EXCLUSIVE</param>
///<param name="lock, but not when obtaining the RESERVED lock.">lock, but not when obtaining the RESERVED lock.</param>
///<param name=""></param>

						rc = this.pagerLockDb (RESERVED_LOCK);
						if (rc == SQLITE_OK && exFlag) {
							rc = this.pager_wait_on_lock (EXCLUSIVE_LOCK);
						}
					}
					if (rc == SQLITE_OK) {
						///
///<summary>
///Change to WRITER_LOCKED state.
///
///WAL mode sets Pager.eState to PAGER_WRITER_LOCKED or CACHEMOD
///when it has an open transaction, but never to DBMOD or FINISHED.
///This is because in those states the code to roll back savepoint 
///</summary>
///<param name="transactions may copy data from the sub">journal into the database </param>
///<param name="file as well as into the page cache. Which would be incorrect in ">file as well as into the page cache. Which would be incorrect in </param>
///<param name="WAL mode.">WAL mode.</param>
///<param name=""></param>

						this.eState = PAGER_WRITER_LOCKED;
						this.dbHintSize = this.dbSize;
						this.dbFileSize = this.dbSize;
						this.dbOrigSize = this.dbSize;
						this.journalOff = 0;
					}
					Debug.Assert (rc == SQLITE_OK || this.eState == PAGER_READER);
					Debug.Assert (rc != SQLITE_OK || this.eState == PAGER_WRITER_LOCKED);
					Debug.Assert (this.assert_pager_state ());
				}
				PAGERTRACE ("TRANSACTION %d\n", PAGERID (this));
				return rc;
			}

			public///<summary>
			/// This routine is called to increment the value of the database file
			/// change-counter, stored as a 4-byte big-endian integer starting at
			/// byte offset 24 of the pager file.  The secondary change counter at
			/// 92 is also updated, as is the SQLite version number at offset 96.
			///
			/// But this only happens if the pPager.changeCountDone flag is false.
			/// To avoid excess churning of page 1, the update only happens once.
			/// See also the pager_write_changecounter() routine that does an
			/// unconditional update of the change counters.
			///
			/// If the isDirectMode flag is zero, then this is done by calling
			/// sqlite3PagerWrite() on page 1, then modifying the contents of the
			/// page data. In this case the file will be updated when the current
			/// transaction is committed.
			///
			/// The isDirectMode flag may only be non-zero if the library was compiled
			/// with the SQLITE_ENABLE_ATOMIC_WRITE macro defined. In this case,
			/// if isDirect is non-zero, then the database file is updated directly
			/// by writing an updated version of page 1 using a call to the
			/// sqlite3OsWrite() function.
			///
			///</summary>
			int pager_incr_changecounter (bool isDirectMode)
			{
				int rc = SQLITE_OK;
				Debug.Assert (this.eState == PAGER_WRITER_CACHEMOD || this.eState == PAGER_WRITER_DBMOD);
				Debug.Assert (this.assert_pager_state ());
				///
///<summary>
///Declare and initialize constant integer 'isDirect'. If the
///</summary>
///<param name="atomic">write optimization is enabled in this build, then isDirect</param>
///<param name="is initialized to the value passed as the isDirectMode parameter">is initialized to the value passed as the isDirectMode parameter</param>
///<param name="to this function. Otherwise, it is always set to zero.">to this function. Otherwise, it is always set to zero.</param>
///<param name=""></param>
///<param name="The idea is that if the atomic">write optimization is not</param>
///<param name="enabled at compile time, the compiler can omit the tests of">enabled at compile time, the compiler can omit the tests of</param>
///<param name="'isDirect' below, as well as the block enclosed in the">'isDirect' below, as well as the block enclosed in the</param>
///<param name=""if( isDirect )" condition.">"if( isDirect )" condition.</param>
///<param name=""></param>

				#if !SQLITE_ENABLE_ATOMIC_WRITE
				//# define DIRECT_MODE 0
				bool DIRECT_MODE = false;
				Debug.Assert (isDirectMode == false);
				UNUSED_PARAMETER (isDirectMode);
				#else
																																																																						// define DIRECT_MODE isDirectMode
int DIRECT_MODE = isDirectMode;
#endif
				if (!this.changeCountDone && this.dbSize > 0) {
					PgHdr pPgHdr = null;
					///
///<summary>
///Reference to page 1 
///</summary>

					Debug.Assert (!this.tempFile && isOpen (this.fd));
					///
///<summary>
///Open page 1 of the file for writing. 
///</summary>

					rc = this.sqlite3PagerGet (1, ref pPgHdr);
					Debug.Assert (pPgHdr == null || rc == SQLITE_OK);
					///
///<summary>
///If page one was fetched successfully, and this function is not
///</summary>
///<param name="operating in direct">mode, make page 1 writable.  When not in </param>
///<param name="direct mode, page 1 is always held in cache and hence the PagerGet()">direct mode, page 1 is always held in cache and hence the PagerGet()</param>
///<param name="above is always successful "> hence the ALWAYS on rc==SQLITE_OK.</param>
///<param name=""></param>

					if (!DIRECT_MODE && ALWAYS (rc == SQLITE_OK)) {
						rc = sqlite3PagerWrite (pPgHdr);
					}
					if (rc == SQLITE_OK) {
						///
///<summary>
///Actually do the update of the change counter 
///</summary>

						pager_write_changecounter (pPgHdr);
						///
///<summary>
///If running in direct mode, write the contents of page 1 to the file. 
///</summary>

						if (DIRECT_MODE) {
							u8[] zBuf = null;
							Debug.Assert (this.dbFileSize > 0);
							if (CODEC2 (this, pPgHdr.pData, 1, SQLITE_ENCRYPT_WRITE_CTX, ref zBuf))
								return rc = SQLITE_NOMEM;
							//CODEC2(pPager, pPgHdr.pData, 1, 6, rc=SQLITE_NOMEM, zBuf);
							if (rc == SQLITE_OK) {
								rc = sqlite3OsWrite (this.fd, zBuf, this.pageSize, 0);
							}
							if (rc == SQLITE_OK) {
								this.changeCountDone = true;
							}
						}
						else {
							this.changeCountDone = true;
						}
					}
					///
///<summary>
///Release the page reference. 
///</summary>

					sqlite3PagerUnref (pPgHdr);
				}
				return rc;
			}

			public///<summary>
			/// Sync the database file to disk. This is a no-op for in-memory databases
			/// or pages with the Pager.noSync flag set.
			///
			/// If successful, or if called on a pager for which it is a no-op, this
			/// function returns SQLITE_OK. Otherwise, an IO error code is returned.
			///
			///</summary>
			int sqlite3PagerSync ()
			{
				long rc = SQLITE_OK;
				if (!this.noSync) {
					Debug.Assert (
					#if SQLITE_OMIT_MEMORYDB
																																																																																									0 == MEMDB
#else
					0 == this.memDb
					#endif
					);
					rc = sqlite3OsSync (this.fd, this.syncFlags);
				}
				else
					if (isOpen (this.fd)) {
						Debug.Assert (
						#if SQLITE_OMIT_MEMORYDB
																																																																																																												0 == MEMDB
#else
						0 == this.memDb
						#endif
						);
						sqlite3OsFileControl (this.fd, SQLITE_FCNTL_SYNC_OMITTED, ref rc);
					}
				return (int)rc;
			}

			public///<summary>
			/// This function may only be called while a write-transaction is active in
			/// rollback. If the connection is in WAL mode, this call is a no-op.
			/// Otherwise, if the connection does not already have an EXCLUSIVE lock on
			/// the database file, an attempt is made to obtain one.
			///
			/// If the EXCLUSIVE lock is already held or the attempt to obtain it is
			/// successful, or the connection is in WAL mode, SQLITE_OK is returned.
			/// Otherwise, either SQLITE_BUSY or an SQLITE_IOERR_XXX error code is
			/// returned.
			///
			///</summary>
			int sqlite3PagerExclusiveLock ()
			{
				int rc = SQLITE_OK;
				Debug.Assert (this.eState == PAGER_WRITER_CACHEMOD || this.eState == PAGER_WRITER_DBMOD || this.eState == PAGER_WRITER_LOCKED);
				Debug.Assert (this.assert_pager_state ());
				if (false == this.pagerUseWal ()) {
					rc = this.pager_wait_on_lock (EXCLUSIVE_LOCK);
				}
				return rc;
			}

			public///<summary>
			/// Sync the database file for the pager pPager. zMaster points to the name
			/// of a master journal file that should be written into the individual
			/// journal file. zMaster may be NULL, which is interpreted as no master
			/// journal (a single database transaction).
			///
			/// This routine ensures that:
			///
			///   * The database file change-counter is updated,
			///   * the journal is synced (unless the atomic-write optimization is used),
			///   * all dirty pages are written to the database file,
			///   * the database file is truncated (if required), and
			///   * the database file synced.
			///
			/// The only thing that remains to commit the transaction is to finalize
			/// (delete, truncate or zero the first part of) the journal file (or
			/// delete the master journal file if specified).
			///
			/// Note that if zMaster==NULL, this does not overwrite a previous value
			/// passed to an sqlite3PagerCommitPhaseOne() call.
			///
			/// If the final parameter - noSync - is true, then the database file itself
			/// is not synced. The caller must call sqlite3PagerSync() directly to
			/// sync the database file before calling CommitPhaseTwo() to delete the
			/// journal file in this case.
			///
			///</summary>
			int sqlite3PagerCommitPhaseOne (///
///<summary>
///Pager object 
///</summary>

			string zMaster, ///
///<summary>
///If not NULL, the master journal name 
///</summary>

			bool noSync///
///<summary>
///True to omit the xSync on the db file 
///</summary>

			)
			{
				int rc = SQLITE_OK;
				///
///<summary>
///Return code 
///</summary>

				Debug.Assert (this.eState == PAGER_WRITER_LOCKED || this.eState == PAGER_WRITER_CACHEMOD || this.eState == PAGER_WRITER_DBMOD || this.eState == PAGER_ERROR);
				Debug.Assert (this.assert_pager_state ());
				///
///<summary>
///If a prior error occurred, report that error again. 
///</summary>

				if (NEVER (this.errCode != 0))
					return this.errCode;
				PAGERTRACE ("DATABASE SYNC: File=%s zMaster=%s nSize=%d\n", this.zFilename, zMaster, this.dbSize);
				///
///<summary>
///If no database changes have been made, return early. 
///</summary>

				if (this.eState < PAGER_WRITER_CACHEMOD)
					return SQLITE_OK;
				if (
				#if SQLITE_OMIT_MEMORYDB
																																																																						0 != MEMDB
#else
				0 != this.memDb
				#endif
				) {
					///
///<summary>
///</summary>
///<param name="If this is an in">memory db, or no pages have been written to, or this</param>
///<param name="function has already been called, it is mostly a no">op.  However, any</param>
///<param name="backup in progress needs to be restarted.">backup in progress needs to be restarted.</param>
///<param name=""></param>

					this.pBackup.sqlite3BackupRestart ();
				}
				else {
					if (this.pagerUseWal ()) {
						PgHdr pList = sqlite3PcacheDirtyList (this.pPCache);
						PgHdr pPageOne = null;
						if (pList == null) {
							///
///<summary>
///Must have at least one page for the WAL commit flag.
///</summary>
///<param name="Ticket [2d1a5c67dfc2363e44f29d9bbd57f] 2null11">18 </param>

							rc = this.sqlite3PagerGet (1, ref pPageOne);
							pList = pPageOne;
							pList.pDirty = null;
						}
						Debug.Assert (rc == SQLITE_OK);
						if (ALWAYS (pList)) {
							rc = this.pagerWalFrames (pList, this.dbSize, 1, (this.fullSync ? this.syncFlags : (byte)0));
						}
						sqlite3PagerUnref (pPageOne);
						if (rc == SQLITE_OK) {
							sqlite3PcacheCleanAll (this.pPCache);
						}
					}
					else {
						///
///<summary>
///</summary>
///<param name="The following block updates the change">counter. Exactly how it</param>
///<param name="does this depends on whether or not the atomic">update optimization</param>
///<param name="was enabled at compile time, and if this transaction meets the">was enabled at compile time, and if this transaction meets the</param>
///<param name="runtime criteria to use the operation:">runtime criteria to use the operation:</param>
///<param name=""></param>
///<param name="The file">write property for</param>
///<param name="blocks of size page">size, and</param>
///<param name="This commit is not part of a multi">file transaction, and</param>
///<param name="Exactly one page has been modified and store in the journal file.">Exactly one page has been modified and store in the journal file.</param>
///<param name=""></param>
///<param name="If the optimization was not enabled at compile time, then the">If the optimization was not enabled at compile time, then the</param>
///<param name="pager_incr_changecounter() function is called to update the change">pager_incr_changecounter() function is called to update the change</param>
///<param name="counter in 'indirect">mode'. If the optimization is compiled in but</param>
///<param name="is not applicable to this transaction, call sqlite3JournalCreate()">is not applicable to this transaction, call sqlite3JournalCreate()</param>
///<param name="to make sure the journal file has actually been created, then call">to make sure the journal file has actually been created, then call</param>
///<param name="pager_incr_changecounter() to update the change">counter in indirect</param>
///<param name="mode.">mode.</param>
///<param name=""></param>
///<param name="Otherwise, if the optimization is both enabled and applicable,">Otherwise, if the optimization is both enabled and applicable,</param>
///<param name="then call pager_incr_changecounter() to update the change">counter</param>
///<param name="in 'direct' mode. In this case the journal file will never be">in 'direct' mode. In this case the journal file will never be</param>
///<param name="created for this transaction.">created for this transaction.</param>
///<param name=""></param>

						#if SQLITE_ENABLE_ATOMIC_WRITE
																																																																																																												PgHdr *pPg;
Debug.Assert( isOpen(pPager.jfd) 
|| pPager.journalMode==PAGER_JOURNALMODE_OFF 
|| pPager.journalMode==PAGER_JOURNALMODE_WAL 
);
if( !zMaster && isOpen(pPager.jfd)
&& pPager.journalOff==jrnlBufferSize(pPager)
&& pPager.dbSize>=pPager.dbOrigSize
&& (0==(pPg = sqlite3PcacheDirtyList(pPager.pPCache)) || 0==pPg.pDirty)
){
/* Update the db file change counter via the direct-write method. The
** following call will modify the in-memory representation of page 1
** to include the updated change counter and then write page 1
** directly to the database file. Because of the atomic-write
** property of the host file-system, this is safe.
*/
rc = pager_incr_changecounter(pPager, 1);
}else{
rc = sqlite3JournalCreate(pPager.jfd);
if( rc==SQLITE_OK ){
rc = pager_incr_changecounter(pPager, 0);
}
}
#else
						rc = this.pager_incr_changecounter (false);
						#endif
						if (rc != SQLITE_OK)
							goto commit_phase_one_exit;
						///
///<summary>
///If this transaction has made the database smaller, then all pages
///being discarded by the truncation must be written to the journal
///</summary>
///<param name="file. This can only happen in auto">vacuum mode.</param>
///<param name=""></param>
///<param name="Before reading the pages with page numbers larger than the">Before reading the pages with page numbers larger than the</param>
///<param name="current value of Pager.dbSize, set dbSize back to the value">current value of Pager.dbSize, set dbSize back to the value</param>
///<param name="that it took at the start of the transaction. Otherwise, the">that it took at the start of the transaction. Otherwise, the</param>
///<param name="calls to sqlite3PagerGet() return zeroed pages instead of">calls to sqlite3PagerGet() return zeroed pages instead of</param>
///<param name="reading data from the database file.">reading data from the database file.</param>
///<param name=""></param>

						#if !SQLITE_OMIT_AUTOVACUUM
						if (this.dbSize < this.dbOrigSize && this.journalMode != PAGER_JOURNALMODE_OFF) {
							Pgno i;
							///
///<summary>
///Iterator variable 
///</summary>

							Pgno iSkip = PAGER_MJ_PGNO (this);
							///
///<summary>
///Pending lock page 
///</summary>

							Pgno dbSize = this.dbSize;
							///
///<summary>
///Database image size 
///</summary>

							this.dbSize = this.dbOrigSize;
							for (i = dbSize + 1; i <= this.dbOrigSize; i++) {
								if (0 == sqlite3BitvecTest (this.pInJournal, i) && i != iSkip) {
									PgHdr pPage = null;
									///
///<summary>
///Page to journal 
///</summary>

									rc = this.sqlite3PagerGet (i, ref pPage);
									if (rc != SQLITE_OK)
										goto commit_phase_one_exit;
									rc = sqlite3PagerWrite (pPage);
									sqlite3PagerUnref (pPage);
									if (rc != SQLITE_OK)
										goto commit_phase_one_exit;
								}
							}
							this.dbSize = dbSize;
						}
						#endif
						///
///<summary>
///Write the master journal name into the journal file. If a master
///journal file name has already been written to the journal file,
///</summary>
///<param name="or if zMaster is NULL (no master journal), then this call is a no">op.</param>

						rc = this.writeMasterJournal (zMaster);
						if (rc != SQLITE_OK)
							goto commit_phase_one_exit;
						///
///<summary>
///Sync the journal file and write all dirty pages to the database.
///</summary>
///<param name="If the atomic">update optimization is being used, this sync will not </param>
///<param name="create the journal file or perform any real IO.">create the journal file or perform any real IO.</param>
///<param name=""></param>
///<param name="Because the change">counter page was just modified, unless the</param>
///<param name="atomic">update optimization is used it is almost certain that the</param>
///<param name="journal requires a sync here. However, in locking_mode=exclusive">journal requires a sync here. However, in locking_mode=exclusive</param>
///<param name="on a system under memory pressure it is just possible that this is ">on a system under memory pressure it is just possible that this is </param>
///<param name="not the case. In this case it is likely enough that the redundant">not the case. In this case it is likely enough that the redundant</param>
///<param name="xSync() call will be changed to a no">op by the OS anyhow. </param>
///<param name=""></param>

						rc = this.syncJournal (0);
						if (rc != SQLITE_OK)
							goto commit_phase_one_exit;
						rc = this.pager_write_pagelist (sqlite3PcacheDirtyList (this.pPCache));
						if (rc != SQLITE_OK) {
							Debug.Assert (rc != SQLITE_IOERR_BLOCKED);
							goto commit_phase_one_exit;
						}
						sqlite3PcacheCleanAll (this.pPCache);
						///
///<summary>
///If the file on disk is not the same size as the database image,
///then use pager_truncate to grow or shrink the file here.
///
///</summary>

						if (this.dbSize != this.dbFileSize) {
							Pgno nNew = (Pgno)(this.dbSize - (this.dbSize == PAGER_MJ_PGNO (this) ? 1 : 0));
							Debug.Assert (this.eState >= PAGER_WRITER_DBMOD);
							rc = this.pager_truncate (nNew);
							if (rc != SQLITE_OK)
								goto commit_phase_one_exit;
						}
						///
///<summary>
///Finally, sync the database file. 
///</summary>

						if (!noSync) {
							rc = this.sqlite3PagerSync ();
						}
						IOTRACE ("DBSYNC %p\n", this);
					}
				}
				commit_phase_one_exit:
				if (rc == SQLITE_OK && !this.pagerUseWal ()) {
					this.eState = PAGER_WRITER_FINISHED;
				}
				return rc;
			}

			public///<summary>
			/// When this function is called, the database file has been completely
			/// updated to reflect the changes made by the current transaction and
			/// synced to disk. The journal file still exists in the file-system
			/// though, and if a failure occurs at this point it will eventually
			/// be used as a hot-journal and the current transaction rolled back.
			///
			/// This function finalizes the journal file, either by deleting,
			/// truncating or partially zeroing it, so that it cannot be used
			/// for hot-journal rollback. Once this is done the transaction is
			/// irrevocably committed.
			///
			/// If an error occurs, an IO error code is returned and the pager
			/// moves into the error state. Otherwise, SQLITE_OK is returned.
			///
			///</summary>
			int sqlite3PagerCommitPhaseTwo ()
			{
				int rc = SQLITE_OK;
				///
///<summary>
///Return code 
///</summary>

				///
///<summary>
///This routine should not be called if a prior error has occurred.
///But if (due to a coding error elsewhere in the system) it does get
///called, just return the same error code without doing anything. 
///</summary>

				if (NEVER (this.errCode) != 0)
					return this.errCode;
				Debug.Assert (this.eState == PAGER_WRITER_LOCKED || this.eState == PAGER_WRITER_FINISHED || (this.pagerUseWal () && this.eState == PAGER_WRITER_CACHEMOD));
				Debug.Assert (this.assert_pager_state ());
				///
///<summary>
///An optimization. If the database was not actually modified during
///</summary>
///<param name="this transaction, the pager is running in exclusive">mode and is</param>
///<param name="using persistent journals, then this function is a no">op.</param>
///<param name=""></param>
///<param name="The start of the journal file currently contains a single journal">The start of the journal file currently contains a single journal</param>
///<param name="header with the nRec field set to 0. If such a journal is used as">header with the nRec field set to 0. If such a journal is used as</param>
///<param name="a hot">journal rollback, 0 changes will be made</param>
///<param name="to the database file. So there is no need to zero the journal">to the database file. So there is no need to zero the journal</param>
///<param name="header. Since the pager is in exclusive mode, there is no need">header. Since the pager is in exclusive mode, there is no need</param>
///<param name="to drop any locks either.">to drop any locks either.</param>
///<param name=""></param>

				if (this.eState == PAGER_WRITER_LOCKED && this.exclusiveMode && this.journalMode == PAGER_JOURNALMODE_PERSIST) {
					Debug.Assert (this.journalOff == this.JOURNAL_HDR_SZ () || 0 == this.journalOff);
					this.eState = PAGER_READER;
					return SQLITE_OK;
				}
				PAGERTRACE ("COMMIT %d\n", PAGERID (this));
				rc = this.pager_end_transaction (this.setMaster);
				return this.pager_error (rc);
			}

			public///<summary>
			/// If a write transaction is open, then all changes made within the
			/// transaction are reverted and the current write-transaction is closed.
			/// The pager falls back to PAGER_READER state if successful, or PAGER_ERROR
			/// state if an error occurs.
			///
			/// If the pager is already in PAGER_ERROR state when this function is called,
			/// it returns Pager.errCode immediately. No work is performed in this case.
			///
			/// Otherwise, in rollback mode, this function performs two functions:
			///
			///   1) It rolls back the journal file, restoring all database file and
			///      in-memory cache pages to the state they were in when the transaction
			///      was opened, and
			///
			///   2) It finalizes the journal file, so that it is not used for hot
			///      rollback at any point in the future.
			///
			/// Finalization of the journal file (task 2) is only performed if the
			/// rollback is successful.
			///
			/// In WAL mode, all cache-entries containing data modified within the
			/// current transaction are either expelled from the cache or reverted to
			/// their pre-transaction state by re-reading data from the database or
			/// WAL files. The WAL transaction is then closed.
			///
			///</summary>
			int sqlite3PagerRollback ()
			{
				int rc = SQLITE_OK;
				///
///<summary>
///Return code 
///</summary>

				PAGERTRACE ("ROLLBACK %d\n", PAGERID (this));
				///
///<summary>
///</summary>
///<param name="PagerRollback() is a no">op if called in READER or OPEN state. If</param>
///<param name="the pager is already in the ERROR state, the rollback is not ">the pager is already in the ERROR state, the rollback is not </param>
///<param name="attempted here. Instead, the error code is returned to the caller.">attempted here. Instead, the error code is returned to the caller.</param>
///<param name=""></param>

				Debug.Assert (this.assert_pager_state ());
				if (this.eState == PAGER_ERROR)
					return this.errCode;
				if (this.eState <= PAGER_READER)
					return SQLITE_OK;
				if (this.pagerUseWal ()) {
					int rc2;
					rc = this.sqlite3PagerSavepoint (SAVEPOINT_ROLLBACK, -1);
					rc2 = this.pager_end_transaction (this.setMaster);
					if (rc == SQLITE_OK)
						rc = rc2;
					rc = this.pager_error (rc);
				}
				else
					if (!isOpen (this.jfd) || this.eState == PAGER_WRITER_LOCKED) {
						int eState = this.eState;
						rc = this.pager_end_transaction (0);
						if (
						#if SQLITE_OMIT_MEMORYDB
																																																																																																												0==MEMDB
#else
						0 == this.memDb 
						#endif
						&& eState > PAGER_WRITER_LOCKED) {
							///
///<summary>
///This can happen using journal_mode=off. Move the pager to the error 
///state to indicate that the contents of the cache may not be trusted.
///Any active readers will get SQLITE_ABORT.
///
///</summary>

							this.errCode = SQLITE_ABORT;
							this.eState = PAGER_ERROR;
							return rc;
						}
					}
					else {
						rc = this.pager_playback (0);
					}
				Debug.Assert (this.eState == PAGER_READER || rc != SQLITE_OK);
				Debug.Assert (rc == SQLITE_OK || rc == SQLITE_FULL || (rc & 0xFF) == SQLITE_IOERR);
				///
///<summary>
///If an error occurs during a ROLLBACK, we can no longer trust the pager
///cache. So call pager_error() on the way out to make any error persistent.
///
///</summary>

				return this.pager_error (rc);
			}

			public///<summary>
			/// Return TRUE if the database file is opened read-only.  Return FALSE
			/// if the database is (in theory) writable.
			///
			///</summary>
			bool sqlite3PagerIsreadonly ()
			{
				return this.readOnly;
			}

			public///<summary>
			/// Return the number of references to the pager.
			///
			///</summary>
			int sqlite3PagerRefcount ()
			{
				return sqlite3PcacheRefCount (this.pPCache);
			}

			public///<summary>
			/// Return the approximate number of bytes of memory currently
			/// used by the pager and its associated cache.
			///
			///</summary>
			int sqlite3PagerMemUsed ()
			{
				int perPageSize = this.pageSize + this.nExtra + 20;
				//+ sizeof(PgHdr) + 5*sizeof(void*);
				return perPageSize * sqlite3PcachePagecount (this.pPCache) + 0 // Not readily available under C#// sqlite3MallocSize(pPager);
				+ this.pageSize;
			}

			public///<summary>
			/// Return true if this is an in-memory pager.
			///</summary>
			bool sqlite3PagerIsMemdb ()
			{
				#if SQLITE_OMIT_MEMORYDB
																																																																						return MEMDB != 0;
#else
				return this.memDb != 0;
				#endif
			}

			public///<summary>
			/// Check that there are at least nSavepoint savepoints open. If there are
			/// currently less than nSavepoints open, then open one or more savepoints
			/// to make up the difference. If the number of savepoints is already
			/// equal to nSavepoint, then this function is a no-op.
			///
			/// If a memory allocation fails, SQLITE_NOMEM is returned. If an error
			/// occurs while opening the sub-journal file, then an IO error code is
			/// returned. Otherwise, SQLITE_OK.
			///
			///</summary>
			int sqlite3PagerOpenSavepoint (int nSavepoint)
			{
				int rc = SQLITE_OK;
				///
///<summary>
///Return code 
///</summary>

				int nCurrent = this.nSavepoint;
				///
///<summary>
///Current number of savepoints 
///</summary>

				Debug.Assert (this.eState >= PAGER_WRITER_LOCKED);
				Debug.Assert (this.assert_pager_state ());
				if (nSavepoint > nCurrent && this.useJournal != 0) {
					int ii;
					///
///<summary>
///Iterator variable 
///</summary>

					PagerSavepoint[] aNew;
					///
///<summary>
///New Pager.aSavepoint array 
///</summary>

					///
///<summary>
///Grow the Pager.aSavepoint array using realloc(). Return SQLITE_NOMEM
///if the allocation fails. Otherwise, zero the new portion in case a 
///malloc failure occurs while populating it in the for(...) loop below.
///
///</summary>

					//aNew = (PagerSavepoint *)sqlite3Realloc(
					//    pPager.aSavepoint, sizeof(PagerSavepoint)*nSavepoint
					//);
					Array.Resize (ref this.aSavepoint, nSavepoint);
					aNew = this.aSavepoint;
					//if( null==aNew ){
					//  return SQLITE_NOMEM;
					//}
					// memset(&aNew[nCurrent], 0, (nSavepoint-nCurrent) * sizeof(PagerSavepoint));
					// pPager.aSavepoint = aNew;
					///
///<summary>
///Populate the PagerSavepoint structures just allocated. 
///</summary>

					for (ii = nCurrent; ii < nSavepoint; ii++) {
						aNew [ii] = new PagerSavepoint ();
						aNew [ii].nOrig = this.dbSize;
						if (isOpen (this.jfd) && this.journalOff > 0) {
							aNew [ii].iOffset = this.journalOff;
						}
						else {
							aNew [ii].iOffset = (int)this.JOURNAL_HDR_SZ ();
						}
						aNew [ii].iSubRec = this.nSubRec;
						aNew [ii].pInSavepoint = sqlite3BitvecCreate (this.dbSize);
						//if ( null == aNew[ii].pInSavepoint )
						//{
						//  return SQLITE_NOMEM;
						//}
						if (this.pagerUseWal ()) {
							sqlite3WalSavepoint (this.pWal, aNew [ii].aWalData);
						}
						this.nSavepoint = ii + 1;
					}
					Debug.Assert (this.nSavepoint == nSavepoint);
					this.assertTruncateConstraint ();
				}
				return rc;
			}

			public///<summary>
			/// This function is called to rollback or release (commit) a savepoint.
			/// The savepoint to release or rollback need not be the most recently
			/// created savepoint.
			///
			/// Parameter op is always either SAVEPOINT_ROLLBACK or SAVEPOINT_RELEASE.
			/// If it is SAVEPOINT_RELEASE, then release and destroy the savepoint with
			/// index iSavepoint. If it is SAVEPOINT_ROLLBACK, then rollback all changes
			/// that have occurred since the specified savepoint was created.
			///
			/// The savepoint to rollback or release is identified by parameter
			/// iSavepoint. A value of 0 means to operate on the outermost savepoint
			/// (the first created). A value of (Pager.nSavepoint-1) means operate
			/// on the most recently created savepoint. If iSavepoint is greater than
			/// (Pager.nSavepoint-1), then this function is a no-op.
			///
			/// If a negative value is passed to this function, then the current
			/// transaction is rolled back. This is different to calling
			/// sqlite3PagerRollback() because this function does not terminate
			/// the transaction or unlock the database, it just restores the
			/// contents of the database to its original state.
			///
			/// In any case, all savepoints with an index greater than iSavepoint
			/// are destroyed. If this is a release operation (op==SAVEPOINT_RELEASE),
			/// then savepoint iSavepoint is also destroyed.
			///
			/// This function may return SQLITE_NOMEM if a memory allocation fails,
			/// or an IO error code if an IO error occurs while rolling back a
			/// savepoint. If no errors occur, SQLITE_OK is returned.
			///
			///</summary>
			int sqlite3PagerSavepoint (int op, int iSavepoint)
			{
				int rc = this.errCode;
				///
///<summary>
///Return code 
///</summary>

				Debug.Assert (op == SAVEPOINT_RELEASE || op == SAVEPOINT_ROLLBACK);
				Debug.Assert (iSavepoint >= 0 || op == SAVEPOINT_ROLLBACK);
				if (rc == SQLITE_OK && iSavepoint < this.nSavepoint) {
					int ii;
					///
///<summary>
///Iterator variable 
///</summary>

					int nNew;
					///
///<summary>
///Number of remaining savepoints after this op. 
///</summary>

					///
///<summary>
///Figure out how many savepoints will still be active after this
///operation. Store this value in nNew. Then free resources associated
///with any savepoints that are destroyed by this operation.
///
///</summary>

					nNew = iSavepoint + ((op == SAVEPOINT_RELEASE) ? 0 : 1);
					for (ii = nNew; ii < this.nSavepoint; ii++) {
						sqlite3BitvecDestroy (ref this.aSavepoint [ii].pInSavepoint);
					}
					this.nSavepoint = nNew;
					///
///<summary>
///If this is a release of the outermost savepoint, truncate 
///</summary>
///<param name="the sub">journal to zero bytes in size. </param>

					if (op == SAVEPOINT_RELEASE) {
						if (nNew == 0 && isOpen (this.sjfd)) {
							///
///<summary>
///</summary>
///<param name="Only truncate if it is an in">journal. </param>

							if (sqlite3IsMemJournal (this.sjfd)) {
								rc = sqlite3OsTruncate (this.sjfd, 0);
								Debug.Assert (rc == SQLITE_OK);
							}
							this.nSubRec = 0;
						}
					}
					///
///<summary>
///Else this is a rollback operation, playback the specified savepoint.
///</summary>
///<param name="If this is a temp">file, it is possible that the journal file has</param>
///<param name="not yet been opened. In this case there have been no changes to">not yet been opened. In this case there have been no changes to</param>
///<param name="the database file, so the playback operation can be skipped.">the database file, so the playback operation can be skipped.</param>
///<param name=""></param>

					else
						if (this.pagerUseWal () || isOpen (this.jfd)) {
							PagerSavepoint pSavepoint = (nNew == 0) ? null : this.aSavepoint [nNew - 1];
							rc = this.pagerPlaybackSavepoint (pSavepoint);
							Debug.Assert (rc != SQLITE_DONE);
						}
				}
				return rc;
			}

			public///<summary>
			/// Return the VFS structure for the pager.
			///
			///</summary>
			sqlite3_vfs sqlite3PagerVfs ()
			{
				return this.pVfs;
			}

			public///<summary>
			/// Return the full pathname of the database file.
			///
			///</summary>
			string sqlite3PagerFilename ()
			{
				return this.zFilename;
			}

			public///<summary>
			/// Return the file handle for the database file associated
			/// with the pager.  This might return NULL if the file has
			/// not yet been opened.
			///
			///</summary>
			sqlite3_file sqlite3PagerFile ()
			{
				return this.fd;
			}

			public///<summary>
			/// Return true if fsync() calls are disabled for this pager.  Return FALSE
			/// if fsync()s are executed normally.
			///
			///</summary>
			bool sqlite3PagerNosync ()
			{
				return this.noSync;
			}

			public///<summary>
			/// Return the full pathname of the journal file.
			///
			///</summary>
			string sqlite3PagerJournalname ()
			{
				return this.zJournal;
			}

			public void sqlite3PagerSetCodec (dxCodec xCodec, //void *(*xCodec)(void*,void*,Pgno,int),
			dxCodecSizeChng xCodecSizeChng, //void (*xCodecSizeChng)(void*,int,int),
			dxCodecFree xCodecFree, //void (*xCodecFree)(void*),
			codec_ctx pCodec)
			{
				if (this.xCodecFree != null)
					this.xCodecFree (ref this.pCodec);
				this.xCodec = (this.memDb != 0) ? null : xCodec;
				this.xCodecSizeChng = xCodecSizeChng;
				this.xCodecFree = xCodecFree;
				this.pCodec = pCodec;
				this.pagerReportSize ();
			}

			public object sqlite3PagerGetCodec ()
			{
				return this.pCodec;
			}

			public///<summary>
			/// Move the page pPg to location pgno in the file.
			///
			/// There must be no references to the page previously located at
			/// pgno (which we call pPgOld) though that page is allowed to be
			/// in cache.  If the page previously located at pgno is not already
			/// in the rollback journal, it is not put there by by this routine.
			///
			/// References to the page pPg remain valid. Updating any
			/// meta-data associated with pPg (i.e. data stored in the nExtra bytes
			/// allocated along with the page) is the responsibility of the caller.
			///
			/// A transaction must be active when this routine is called. It used to be
			/// required that a statement transaction was not active, but this restriction
			/// has been removed (CREATE INDEX needs to move a page when a statement
			/// transaction is active).
			///
			/// If the fourth argument, isCommit, is non-zero, then this page is being
			/// moved as part of a database reorganization just before the transaction
			/// is being committed. In this case, it is guaranteed that the database page
			/// pPg refers to will not be written to again within this transaction.
			///
			/// This function may return SQLITE_NOMEM or an IO error code if an error
			/// occurs. Otherwise, it returns SQLITE_OK.
			///</summary>
			int sqlite3PagerMovepage (DbPage pPg, u32 pgno, int isCommit)
			{
				PgHdr pPgOld;
				///
///<summary>
///The page being overwritten. 
///</summary>

				u32 needSyncPgno = 0;
				///
///<summary>
///Old value of pPg.pgno, if sync is required 
///</summary>

				int rc;
				///
///<summary>
///Return code 
///</summary>

				Pgno origPgno;
				///
///<summary>
///The original page number 
///</summary>

				Debug.Assert (pPg.nRef > 0);
				Debug.Assert (this.eState == PAGER_WRITER_CACHEMOD || this.eState == PAGER_WRITER_DBMOD);
				Debug.Assert (this.assert_pager_state ());
				///
///<summary>
///</summary>
///<param name="In order to be able to rollback, an in">memory database must journal</param>
///<param name="the page we are moving from.">the page we are moving from.</param>
///<param name=""></param>

				if (
				#if SQLITE_OMIT_MEMORYDB
																																																																						1==MEMDB
#else
				this.memDb != 0
				#endif
				) {
					rc = sqlite3PagerWrite (pPg);
					if (rc != 0)
						return rc;
				}
				///
///<summary>
///If the page being moved is dirty and has not been saved by the latest
///savepoint, then save the current contents of the page into the
///</summary>
///<param name="sub">journal now. This is required to handle the following scenario:</param>
///<param name=""></param>
///<param name="BEGIN;">BEGIN;</param>
///<param name="<journal page X, then modify it in memory>"><journal page X, then modify it in memory></param>
///<param name="SAVEPOINT one;">SAVEPOINT one;</param>
///<param name="<Move page X to location Y>"><Move page X to location Y></param>
///<param name="ROLLBACK TO one;">ROLLBACK TO one;</param>
///<param name=""></param>
///<param name="If page X were not written to the sub">journal here, it would not</param>
///<param name="be possible to restore its contents when the "ROLLBACK TO one"">be possible to restore its contents when the "ROLLBACK TO one"</param>
///<param name="statement were is processed.">statement were is processed.</param>
///<param name=""></param>
///<param name="subjournalPage() may need to allocate space to store pPg.pgno into">subjournalPage() may need to allocate space to store pPg.pgno into</param>
///<param name="one or more savepoint bitvecs. This is the reason this function">one or more savepoint bitvecs. This is the reason this function</param>
///<param name="may return SQLITE_NOMEM.">may return SQLITE_NOMEM.</param>
///<param name=""></param>

				if ((pPg.flags & PGHDR_DIRTY) != 0 && pPg.subjRequiresPage () && SQLITE_OK != (rc = subjournalPage (pPg))) {
					return rc;
				}
				PAGERTRACE ("MOVE %d page %d (needSync=%d) moves to %d\n", PAGERID (this), pPg.pgno, (pPg.flags & PGHDR_NEED_SYNC) != 0 ? 1 : 0, pgno);
				IOTRACE ("MOVE %p %d %d\n", this, pPg.pgno, pgno);
				///
///<summary>
///If the journal needs to be sync()ed before page pPg.pgno can
///be written to, store pPg.pgno in local variable needSyncPgno.
///
///If the isCommit flag is set, there is no need to remember that
///the journal needs to be sync()ed before database page pPg.pgno
///can be written to. The caller has already promised not to write to it.
///
///</summary>

				if (((pPg.flags & PGHDR_NEED_SYNC) != 0) && 0 == isCommit) {
					needSyncPgno = pPg.pgno;
					Debug.Assert (pPg.pageInJournal () || pPg.pgno > this.dbOrigSize);
					Debug.Assert ((pPg.flags & PGHDR_DIRTY) != 0);
				}
				///
///<summary>
///</summary>
///<param name="If the cache contains a page with page">number pgno, remove it</param>
///<param name="from its hash chain. Also, if the PGHDR_NEED_SYNC was set for">from its hash chain. Also, if the PGHDR_NEED_SYNC was set for</param>
///<param name="page pgno before the 'move' operation, it needs to be retained">page pgno before the 'move' operation, it needs to be retained</param>
///<param name="for the page moved there.">for the page moved there.</param>
///<param name=""></param>

				pPg.flags &= ~PGHDR_NEED_SYNC;
				pPgOld = this.pager_lookup (pgno);
				Debug.Assert (null == pPgOld || pPgOld.nRef == 1);
				if (pPgOld != null) {
					pPg.flags |= (pPgOld.flags & PGHDR_NEED_SYNC);
					if (
					#if SQLITE_OMIT_MEMORYDB
																																																																																									1==MEMDB
#else
					this.memDb != 0
					#endif
					) {
						///
///<summary>
///</summary>
///<param name="Do not discard pages from an in">memory database since we might</param>
///<param name="need to rollback later.  Just move the page out of the way. ">need to rollback later.  Just move the page out of the way. </param>

						sqlite3PcacheMove (pPgOld, this.dbSize + 1);
					}
					else {
						sqlite3PcacheDrop (pPgOld);
					}
				}
				origPgno = pPg.pgno;
				sqlite3PcacheMove (pPg, pgno);
				sqlite3PcacheMakeDirty (pPg);
				///
///<summary>
///</summary>
///<param name="For an in">memory database, make sure the original page continues</param>
///<param name="to exist, in case the transaction needs to roll back.  Use pPgOld">to exist, in case the transaction needs to roll back.  Use pPgOld</param>
///<param name="as the original page since it has already been allocated.">as the original page since it has already been allocated.</param>
///<param name=""></param>

				if (
				#if SQLITE_OMIT_MEMORYDB
																																																																						0!=MEMDB
#else
				0 != this.memDb
				#endif
				) {
					Debug.Assert (pPgOld);
					sqlite3PcacheMove (pPgOld, origPgno);
					sqlite3PagerUnref (pPgOld);
				}
				if (needSyncPgno != 0) {
					///
///<summary>
///</summary>
///<param name="If needSyncPgno is non">zero, then the journal file needs to be</param>
///<param name="sync()ed before any data is written to database file page needSyncPgno.">sync()ed before any data is written to database file page needSyncPgno.</param>
///<param name="Currently, no such page exists in the page">cache and the</param>
///<param name=""is journaled" bitvec flag has been set. This needs to be remedied by">"is journaled" bitvec flag has been set. This needs to be remedied by</param>
///<param name="loading the page into the pager">cache and setting the PGHDR_NEED_SYNC</param>
///<param name="flag.">flag.</param>
///<param name=""></param>
///<param name="If the attempt to load the page into the page">cache fails, (due</param>
///<param name="to a malloc() or IO failure), clear the bit in the pInJournal[]">to a malloc() or IO failure), clear the bit in the pInJournal[]</param>
///<param name="array. Otherwise, if the page is loaded and written again in">array. Otherwise, if the page is loaded and written again in</param>
///<param name="this transaction, it may be written to the database file before">this transaction, it may be written to the database file before</param>
///<param name="it is synced into the journal file. This way, it may end up in">it is synced into the journal file. This way, it may end up in</param>
///<param name="the journal file twice, but that is not a problem.">the journal file twice, but that is not a problem.</param>
///<param name=""></param>

					PgHdr pPgHdr = null;
					rc = this.sqlite3PagerGet (needSyncPgno, ref pPgHdr);
					if (rc != SQLITE_OK) {
						if (needSyncPgno <= this.dbOrigSize) {
							Debug.Assert (this.pTmpSpace != null);
							u32[] pTemp = new u32[this.pTmpSpace.Length];
							sqlite3BitvecClear (this.pInJournal, needSyncPgno, pTemp);
							//pPager.pTmpSpace );
						}
						return rc;
					}
					pPgHdr.flags |= PGHDR_NEED_SYNC;
					sqlite3PcacheMakeDirty (pPgHdr);
					sqlite3PagerUnref (pPgHdr);
				}
				return SQLITE_OK;
			}

			public///<summary>
			/// Get/set the locking-mode for this pager. Parameter eMode must be one
			/// of PAGER_LOCKINGMODE_QUERY, PAGER_LOCKINGMODE_NORMAL or
			/// PAGER_LOCKINGMODE_EXCLUSIVE. If the parameter is not _QUERY, then
			/// the locking-mode is set to the value specified.
			///
			/// The returned value is either PAGER_LOCKINGMODE_NORMAL or
			/// PAGER_LOCKINGMODE_EXCLUSIVE, indicating the current (possibly updated)
			/// locking-mode.
			///
			///</summary>
			bool sqlite3PagerLockingMode (int eMode)
			{
				Debug.Assert (eMode == PAGER_LOCKINGMODE_QUERY || eMode == PAGER_LOCKINGMODE_NORMAL || eMode == PAGER_LOCKINGMODE_EXCLUSIVE);
				Debug.Assert (PAGER_LOCKINGMODE_QUERY < 0);
				Debug.Assert (PAGER_LOCKINGMODE_NORMAL >= 0 && PAGER_LOCKINGMODE_EXCLUSIVE >= 0);
				Debug.Assert (this.exclusiveMode || false == sqlite3WalHeapMemory (this.pWal));
				if (eMode >= 0 && !this.tempFile && !sqlite3WalHeapMemory (this.pWal)) {
					this.exclusiveMode = eMode != 0;
				}
				return this.exclusiveMode;
			}

			public///<summary>
			/// Set the journal-mode for this pager. Parameter eMode must be one of:
			///
			///    PAGER_JOURNALMODE_DELETE
			///    PAGER_JOURNALMODE_TRUNCATE
			///    PAGER_JOURNALMODE_PERSIST
			///    PAGER_JOURNALMODE_OFF
			///    PAGER_JOURNALMODE_MEMORY
			///    PAGER_JOURNALMODE_WAL
			///
			/// The journalmode is set to the value specified if the change is allowed.
			/// The change may be disallowed for the following reasons:
			///
			///   *  An in-memory database can only have its journal_mode set to _OFF
			///      or _MEMORY.
			///
			///   *  Temporary databases cannot have _WAL journalmode.
			///
			/// The returned indicate the current (possibly updated) journal-mode.
			///
			///</summary>
			int sqlite3PagerSetJournalMode (int eMode)
			{
				u8 eOld = this.journalMode;
				///
///<summary>
///Prior journalmode 
///</summary>

				#if SQLITE_DEBUG
																																																																						      /* The print_pager_state() routine is intended to be used by the debugger
** only.  We invoke it once here to suppress a compiler warning. */
      print_pager_state( pPager );
#endif
				///
///<summary>
///The eMode parameter is always valid 
///</summary>

				Debug.Assert (eMode == PAGER_JOURNALMODE_DELETE || eMode == PAGER_JOURNALMODE_TRUNCATE || eMode == PAGER_JOURNALMODE_PERSIST || eMode == PAGER_JOURNALMODE_OFF || eMode == PAGER_JOURNALMODE_WAL || eMode == PAGER_JOURNALMODE_MEMORY);
				///
///<summary>
///This routine is only called from the OP_JournalMode opcode, and
///the logic there will never allow a temporary file to be changed
///to WAL mode.
///
///</summary>

				Debug.Assert (this.tempFile == false || eMode != PAGER_JOURNALMODE_WAL);
				///
///<summary>
///</summary>
///<param name="Do allow the journalmode of an in">memory database to be set to</param>
///<param name="anything other than MEMORY or OFF">anything other than MEMORY or OFF</param>
///<param name=""></param>

				if (
				#if SQLITE_OMIT_MEMORYDB
																																																																						1==MEMDB
#else
				1 == this.memDb
				#endif
				) {
					Debug.Assert (eOld == PAGER_JOURNALMODE_MEMORY || eOld == PAGER_JOURNALMODE_OFF);
					if (eMode != PAGER_JOURNALMODE_MEMORY && eMode != PAGER_JOURNALMODE_OFF) {
						eMode = eOld;
					}
				}
				if (eMode != eOld) {
					///
///<summary>
///Change the journal mode. 
///</summary>

					Debug.Assert (this.eState != PAGER_ERROR);
					this.journalMode = (u8)eMode;
					///
///<summary>
///When transistioning from TRUNCATE or PERSIST to any other journal
///mode except WAL, unless the pager is in locking_mode=exclusive mode,
///delete the journal file.
///
///</summary>

					Debug.Assert ((PAGER_JOURNALMODE_TRUNCATE & 5) == 1);
					Debug.Assert ((PAGER_JOURNALMODE_PERSIST & 5) == 1);
					Debug.Assert ((PAGER_JOURNALMODE_DELETE & 5) == 0);
					Debug.Assert ((PAGER_JOURNALMODE_MEMORY & 5) == 4);
					Debug.Assert ((PAGER_JOURNALMODE_OFF & 5) == 0);
					Debug.Assert ((PAGER_JOURNALMODE_WAL & 5) == 5);
					Debug.Assert (isOpen (this.fd) || this.exclusiveMode);
					if (!this.exclusiveMode && (eOld & 5) == 1 && (eMode & 1) == 0) {
						///
///<summary>
///In this case we would like to delete the journal file. If it is
///not possible, then that is not a problem. Deleting the journal file
///here is an optimization only.
///
///Before deleting the journal file, obtain a RESERVED lock on the
///database file. This ensures that the journal file is not deleted
///while it is in use by some other client.
///
///</summary>

						sqlite3OsClose (this.jfd);
						if (this.eLock >= RESERVED_LOCK) {
							sqlite3OsDelete (this.pVfs, this.zJournal, 0);
						}
						else {
							int rc = SQLITE_OK;
							int state = this.eState;
							Debug.Assert (state == PAGER_OPEN || state == PAGER_READER);
							if (state == PAGER_OPEN) {
								rc = this.sqlite3PagerSharedLock ();
							}
							if (this.eState == PAGER_READER) {
								Debug.Assert (rc == SQLITE_OK);
								rc = this.pagerLockDb (RESERVED_LOCK);
							}
							if (rc == SQLITE_OK) {
								sqlite3OsDelete (this.pVfs, this.zJournal, 0);
							}
							if (rc == SQLITE_OK && state == PAGER_READER) {
								this.pagerUnlockDb (SHARED_LOCK);
							}
							else
								if (state == PAGER_OPEN) {
									this.pager_unlock ();
								}
							Debug.Assert (state == this.eState);
						}
					}
				}
				///
///<summary>
///Return the new journal mode 
///</summary>

				return (int)this.journalMode;
			}

			public///<summary>
			/// Return the current journal mode.
			///
			///</summary>
			int sqlite3PagerGetJournalMode ()
			{
				return (int)this.journalMode;
			}

			public///<summary>
			/// Return TRUE if the pager is in a state where it is OK to change the
			/// journalmode.  Journalmode changes can only happen when the database
			/// is unmodified.
			///
			///</summary>
			int sqlite3PagerOkToChangeJournalMode ()
			{
				Debug.Assert (this.assert_pager_state ());
				if (this.eState >= PAGER_WRITER_CACHEMOD)
					return 0;
				if (NEVER (isOpen (this.jfd) && this.journalOff > 0))
					return 0;
				return 1;
			}

			public///<summary>
			/// Get/set the size-limit used for persistent journal files.
			///
			/// Setting the size limit to -1 means no limit is enforced.
			/// An attempt to set a limit smaller than -1 is a no-op.
			///
			///</summary>
			i64 sqlite3PagerJournalSizeLimit (i64 iLimit)
			{
				if (iLimit >= -1) {
					this.journalSizeLimit = iLimit;
					sqlite3WalLimit (this.pWal, iLimit);
				}
				return this.journalSizeLimit;
			}

			public///<summary>
			/// Return a pointer to the pPager.pBackup variable. The backup module
			/// in backup.c maintains the content of this variable. This module
			/// uses it opaquely as an argument to sqlite3BackupRestart() and
			/// sqlite3BackupUpdate() only.
			///
			///</summary>
			sqlite3_backup sqlite3PagerBackupPtr ()
			{
				return this.pBackup;
			}

			public void sqlite3pager_get_codec (ref codec_ctx ctx)
			{
				ctx = this.pCodec;
			}

			public int sqlite3pager_is_mj_pgno (Pgno pgno)
			{
				return (PAGER_MJ_PGNO (this) == pgno) ? 1 : 0;
			}

			public sqlite3_file sqlite3Pager_get_fd ()
			{
				return (isOpen (this.fd)) ? this.fd : null;
			}

			public void sqlite3pager_sqlite3PagerSetCodec (dxCodec xCodec, dxCodecSizeChng xCodecSizeChng, dxCodecFree xCodecFree, codec_ctx pCodec)
			{
				this.sqlite3PagerSetCodec (xCodec, xCodecSizeChng, xCodecFree, pCodec);
			}
		}

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
		static void PAGER_INCR (ref int v)
		{
		}

		#endif
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

		static byte[] aJournalMagic = new byte[] {
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

		#if SQLITE_OMIT_MEMORYDB
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

		///<summary>
		/// The argument to this macro is a file descriptor (type sqlite3_file*).
		/// Return 0 if it is not open, or non-zero (but not 1) if it is.
		///
		/// This is so that expressions can be written as:
		///
		///   if( isOpen(pPager.jfd) ){ ...
		///
		/// instead of
		///
		///   if( pPager.jfd->pMethods ){ ...
		///
		///</summary>
		//#define isOpen(pFd) ((pFd)->pMethods)
		static bool isOpen (sqlite3_file pFd)
		{
			return pFd.pMethods != null;
		}

		//# define pagerRollbackWal(x) 0
		//# define pagerWalFrames(v,w,x,y,z) 0
		//# define pagerOpenWalIfPresent(z) SQLITE_OK
		//# define pagerBeginReadTransaction(z) SQLITE_OK
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

      sqlite3_snprintf( 1024, zRet,
      "Filename:      %s\n" +
      "State:         %s errCode=%d\n" +
      "Lock:          %s\n" +
      "Locking mode:  locking_mode=%s\n" +
      "Journal mode:  journal_mode=%s\n" +
      "Backing store: tempFile=%d memDb=%d useJournal=%d\n" +
      "Journal:       journalOff=%lld journalHdr=%lld\n" +
      "Size:          dbsize=%d dbOrigSize=%d dbFileSize=%d\n"
      , p.zFilename
      , p.eState == PAGER_OPEN ? "OPEN" :
      p.eState == PAGER_READER ? "READER" :
      p.eState == PAGER_WRITER_LOCKED ? "WRITER_LOCKED" :
      p.eState == PAGER_WRITER_CACHEMOD ? "WRITER_CACHEMOD" :
      p.eState == PAGER_WRITER_DBMOD ? "WRITER_DBMOD" :
      p.eState == PAGER_WRITER_FINISHED ? "WRITER_FINISHED" :
      p.eState == PAGER_ERROR ? "ERROR" : "?error?"
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
		/// that is read in pRes.  Return SQLITE_OK if everything worked, or an
		/// error code is something goes wrong.
		///
		/// All values are stored on disk as big-endian.
		///
		///</summary>
		static int read32bits (sqlite3_file fd, int offset, ref int pRes)
		{
			u32 u32_pRes = 0;
			int rc = read32bits (fd, offset, ref u32_pRes);
			pRes = (int)u32_pRes;
			return rc;
		}

		static int read32bits (sqlite3_file fd, i64 offset, ref u32 pRes)
		{
			int rc = read32bits (fd, (int)offset, ref pRes);
			return rc;
		}

		static int read32bits (sqlite3_file fd, int offset, ref u32 pRes)
		{
			byte[] ac = new byte[4];
			int rc = sqlite3OsRead (fd, ac, ac.Length, offset);
			if (rc == SQLITE_OK) {
				pRes = Converter.sqlite3Get4byte (ac);
			}
			else
				pRes = 0;
			return rc;
		}

		///<summary>
		/// Write a 32-bit integer into a string buffer in big-endian byte order.
		///
		///</summary>
		//#define put32bits(A,B)  sqlite3sqlite3Put4byte((u8*)A,B)
		static void put32bits (string ac, int offset, int val)
		{
			byte[] A = new byte[4];
			A [0] = (byte)ac [offset + 0];
			A [1] = (byte)ac [offset + 1];
			A [2] = (byte)ac [offset + 2];
			A [3] = (byte)ac [offset + 3];
			Converter.sqlite3Put4byte (A, 0, val);
		}

		static void put32bits (byte[] ac, int offset, int val)
		{
			Converter.sqlite3Put4byte (ac, offset, (u32)val);
		}

		static void put32bits (byte[] ac, u32 val)
		{
			Converter.sqlite3Put4byte (ac, 0U, val);
		}

		static void put32bits (byte[] ac, int offset, u32 val)
		{
			Converter.sqlite3Put4byte (ac, offset, val);
		}

		///<summary>
		/// Write a 32-bit integer into the given file descriptor.  Return SQLITE_OK
		/// on success or an error code is something goes wrong.
		///
		///</summary>
		static int write32bits (sqlite3_file fd, i64 offset, u32 val)
		{
			byte[] ac = new byte[4];
			put32bits (ac, val);
			return sqlite3OsWrite (fd, ac, 4, offset);
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

Debug.Assert( isOpen(pPager.fd) );
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
assert( pPager->eState!=PAGER_ERROR );
assert( (pPg->flags&PGHDR_DIRTY) || pPg->pageHash==pager_pagehash(pPg) );
}

#else
		//#define pager_datahash(X,Y)  0
		static int pager_datahash (int X, byte[] Y)
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
		/// zMaster[0] is set to 0 and SQLITE_OK returned.
		///
		/// If an error occurs while reading from the journal file, an SQLite
		/// error code is returned.
		///</summary>
		static int readMasterJournal (sqlite3_file pJrnl, byte[] zMaster, u32 nMaster)
		{
			int rc;
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

			zMaster [0] = 0;
			if (SQLITE_OK != (rc = sqlite3OsFileSize (pJrnl, ref szJ)) || szJ < 16 || SQLITE_OK != (rc = read32bits (pJrnl, (int)(szJ - 16), ref len)) || len >= nMaster || SQLITE_OK != (rc = read32bits (pJrnl, szJ - 12, ref cksum)) || SQLITE_OK != (rc = sqlite3OsRead (pJrnl, aMagic, 8, szJ - 8)) || memcmp (aMagic, aJournalMagic, 8) != 0 || SQLITE_OK != (rc = sqlite3OsRead (pJrnl, zMaster, len, (long)(szJ - 16 - len)))) {
				return rc;
			}
			///
///<summary>
///See if the checksum matches the master journal name 
///</summary>

			for (u = 0; u < len; u++) {
				cksum -= zMaster [u];
			}
			if (cksum != 0) {
				///
///<summary>
///If the checksum doesn't add up, then one or more of the disk sectors
///containing the master journal filename is corrupted. This means
///definitely roll back, so just return SQLITE_OK and report a (nul)
///</summary>
///<param name="master">journal filename.</param>
///<param name=""></param>

				len = 0;
			}
			if (len == 0)
				zMaster [0] = 0;
			return SQLITE_OK;
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
		/// Otherwise, SQLITE_OK is returned.
		///
		///</summary>
		static int readDbPage (PgHdr pPg)
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

			int rc = SQLITE_OK;
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

			Debug.Assert (pPager.eState >= PAGER_READER && 
			#if SQLITE_OMIT_MEMORYDB
																																																									0 == MEMDB 
#else
			0 == pPager.memDb
			#endif
			);
			Debug.Assert (isOpen (pPager.fd));
			if (NEVER (!isOpen (pPager.fd))) {
				Debug.Assert (pPager.tempFile);
				Array.Clear (pPg.pData, 0, pPager.pageSize);
				// memset(pPg.pData, 0, pPager.pageSize);
				return SQLITE_OK;
			}
			if (pPager.pagerUseWal ()) {
				///
///<summary>
///</summary>
///<param name="Try to pull the page from the write">ahead log. </param>

				rc = sqlite3WalRead (pPager.pWal, pgno, ref isInWal, pgsz, pPg.pData);
			}
			if (rc == SQLITE_OK && 0 == isInWal) {
				i64 iOffset = (pgno - 1) * (i64)pPager.pageSize;
				rc = sqlite3OsRead (pPager.fd, pPg.pData, pgsz, iOffset);
				if (rc == SQLITE_IOERR_SHORT_READ) {
					rc = SQLITE_OK;
				}
			}
			if (pgno == 1) {
				if (rc != 0) {
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

					for (int i = 0; i < pPager.dbFileVers.Length; pPager.dbFileVers [i++] = 0xff)
						;
					// memset(pPager.dbFileVers, 0xff, sizeof(pPager.dbFileVers));
				}
				else {
					//u8[] dbFileVers = pPg.pData[24];
					Buffer.BlockCopy (pPg.pData, 24, pPager.dbFileVers, 0, pPager.dbFileVers.Length);
					//memcpy(&pPager.dbFileVers, dbFileVers, sizeof(pPager.dbFileVers));
				}
			}
			if (CODEC1 (pPager, pPg.pData, pgno, SQLITE_DECRYPT))
				rc = SQLITE_NOMEM;
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
			IOTRACE ("PGIN %p %d\n", pPager, pgno);
			PAGERTRACE ("FETCH %d page %d hash(%08x)\n", PAGERID (pPager), pgno, pPg.pager_pagehash ());
			return rc;
		}

		///<summary>
		/// Update the value of the change-counter at offsets 24 and 92 in
		/// the header and the sqlite version number at offset 96.
		///
		/// This is an unconditional update.  See also the pager_incr_changecounter()
		/// routine which only updates the change-counter if the update is actually
		/// needed, as determined by the pPager.changeCountDone state variable.
		///
		///</summary>
		static void pager_write_changecounter (PgHdr pPg)
		{
			u32 change_counter;
			///
///<summary>
///Increment the value just read and write it back to byte 24. 
///</summary>

			change_counter = Converter.sqlite3Get4byte (pPg.pPager.dbFileVers, 0) + 1;
			put32bits (pPg.pData, 24, change_counter);
			///
///<summary>
///Also store the SQLite version number in bytes 96..99 and in
///bytes 92..95 store the change counter for which the version number
///is valid. 
///</summary>

			put32bits (pPg.pData, 92, change_counter);
			put32bits (pPg.pData, 96, SQLITE_VERSION_NUMBER);
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
/// return an SQLite error code. Otherwise, SQLITE_OK.
///</summary>
static int pagerUndoCallback(void *pCtx, Pgno iPg){
int rc = SQLITE_OK;
Pager *pPager = (Pager *)pCtx;
PgHdr *pPg;

pPg = sqlite3PagerLookup(pPager, iPg);
if( pPg ){
if( sqlite3PcachePageRefcount(pPg)==1 ){
sqlite3PcacheDrop(pPg);
}else{
rc = readDbPage(pPg);
if( rc==SQLITE_OK ){
pPager.xReiniter(pPg);
}
sqlite3PagerUnref(pPg);
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
pList = sqlite3PcacheDirtyList(pPager.pPCache);
while( pList && rc==SQLITE_OK ){
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
if( rc==SQLITE_OK && pPager.pBackup ){
PgHdr *p;
for(p=pList; p; p=p->pDirty){
sqlite3BackupUpdate(pPager.pBackup, p->pgno, (u8 *)p->pData);
}
}

#if SQLITE_CHECK_PAGES
																																						pList = sqlite3PcacheDirtyList(pPager.pPCache);
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
assert( pPager.eState==PAGER_OPEN || pPager.eState==PAGER_READER );

/* sqlite3WalEndReadTransaction() was not called for the previous
** transaction in locking_mode=EXCLUSIVE.  So call it now.  If we
** are in locking_mode=NORMAL and EndRead() was previously called,
** the duplicate call is harmless.
*/
sqlite3WalEndReadTransaction(pPager.pWal);

rc = sqlite3WalBeginReadTransaction(pPager.pWal, &changed);
if( rc!=SQLITE_OK || changed ){
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
/// Return SQLITE_OK or an error code.
///
/// The caller must hold a SHARED lock on the database file to call this
/// function. Because an EXCLUSIVE lock on the db file is required to delete
/// a WAL on a none-empty database, this ensures there is no race condition
/// between the xAccess() below and an xDelete() being executed by some
/// other connection.
///</summary>
static int pagerOpenWalIfPresent(Pager *pPager){
int rc = SQLITE_OK;
Debug.Assert( pPager.eState==PAGER_OPEN );
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
if( rc==SQLITE_OK ){
if( isWal ){
testcase( sqlite3PcachePagecount(pPager.pPCache)==0 );
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

		#endif
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
///<param name="(returning SQLITE_OK immediately).">(returning SQLITE_OK immediately).</param>
///<param name=""></param>
///<param name="Otherwise, attempt to obtain the lock using sqlite3OsLock(). Invoke">Otherwise, attempt to obtain the lock using sqlite3OsLock(). Invoke</param>
///<param name="the busy callback if the lock is currently not available. Repeat">the busy callback if the lock is currently not available. Repeat</param>
///<param name="until the busy callback returns false or until the attempt to">until the busy callback returns false or until the attempt to</param>
///<param name="obtain the lock succeeds.">obtain the lock succeeds.</param>
///<param name=""></param>
///<param name="Return SQLITE_OK on success and an error code if we cannot obtain">Return SQLITE_OK on success and an error code if we cannot obtain</param>
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
      sqlite3PcacheIterateDirty( pPager.pPCache, assertTruncateConstraintCb );
    }
#else
		//# define assertTruncateConstraint(pPager)
		static void assertTruncateConstraintCb (PgHdr pPg)
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
		static Pgno sqlite3PagerPagenumber (DbPage pPg)
		{
			return pPg.pgno;
		}

		#endif
		///<summary>
		/// Increment the reference count for page pPg.
		///</summary>
		static void sqlite3PagerRef (DbPage pPg)
		{
			sqlite3PcacheRef (pPg);
		}

		///<summary>
		/// Append a record of the current state of page pPg to the sub-journal.
		/// It is the callers responsibility to use subjRequiresPage() to check
		/// that it is really required before calling this function.
		///
		/// If successful, set the bit corresponding to pPg.pgno in the bitvecs
		/// for all open savepoints before returning.
		///
		/// This function returns SQLITE_OK if everything is successful, an IO
		/// error code if the attempt to write to the sub-journal fails, or
		/// SQLITE_NOMEM if a malloc fails while setting a bit in a savepoint
		/// bitvec.
		///
		///</summary>
		static int subjournalPage (PgHdr pPg)
		{
			int rc = SQLITE_OK;
			Pager pPager = pPg.pPager;
			if (pPager.journalMode != PAGER_JOURNALMODE_OFF) {
				///
///<summary>
///</summary>
///<param name="Open the sub">journal, if it has not already been opened </param>

				Debug.Assert (pPager.useJournal != 0);
				Debug.Assert (isOpen (pPager.jfd) || pPager.pagerUseWal ());
				Debug.Assert (isOpen (pPager.sjfd) || pPager.nSubRec == 0);
				Debug.Assert (pPager.pagerUseWal () || pPg.pageInJournal () || pPg.pgno > pPager.dbOrigSize);
				rc = pPager.openSubJournal ();
				///
///<summary>
///</summary>
///<param name="If the sub">journal was opened successfully (or was already open),</param>
///<param name="write the journal record into the file.  ">write the journal record into the file.  </param>

				if (rc == SQLITE_OK) {
					byte[] pData = pPg.pData;
					i64 offset = pPager.nSubRec * (4 + pPager.pageSize);
					byte[] pData2 = null;
					if (CODEC2 (pPager, pData, pPg.pgno, SQLITE_ENCRYPT_READ_CTX, ref pData2))
						return SQLITE_NOMEM;
					//CODEC2(pPager, pData, pPg.pgno, 7, return SQLITE_NOMEM, pData2);
					PAGERTRACE ("STMT-JOURNAL %d page %d\n", PAGERID (pPager), pPg.pgno);
					rc = write32bits (pPager.sjfd, offset, pPg.pgno);
					if (rc == SQLITE_OK) {
						rc = sqlite3OsWrite (pPager.sjfd, pData2, pPager.pageSize, offset + 4);
					}
				}
			}
			if (rc == SQLITE_OK) {
				pPager.nSubRec++;
				Debug.Assert (pPager.nSavepoint > 0);
				rc = pPager.addToSavepointBitvecs (pPg.pgno);
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
		/// If successful, sqlite3PcacheMakeClean() is called on the page and
		/// SQLITE_OK returned. If an IO error occurs while trying to make the
		/// page clean, the IO error code is returned. If the page cannot be
		/// made clean for some other reason, but no error occurs, then SQLITE_OK
		/// is returned by sqlite3PcacheMakeClean() is not called.
		///
		///</summary>
		static int pagerStress (object p, PgHdr pPg)
		{
			Pager pPager = (Pager)p;
			int rc = SQLITE_OK;
			Debug.Assert (pPg.pPager == pPager);
			Debug.Assert ((pPg.flags & PGHDR_DIRTY) != 0);
			///
///<summary>
///The doNotSyncSpill flag is set during times when doing a sync of
///journal (and adding a new header) is not allowed.  This occurs
///during calls to sqlite3PagerWrite() while trying to journal multiple
///pages belonging to the same sector.
///
///The doNotSpill flag inhibits all cache spilling regardless of whether
///or not a sync is required.  This is set during a rollback.
///
///Spilling is also prohibited when in an error state since that could
///lead to database corruption.   In the current implementaton it 
///is impossible for sqlite3PCacheFetch() to be called with createFlag==1
///while in the error state, hence it is impossible for this routine to
///be called in the error state.  Nevertheless, we include a NEVER()
///test for the error state as a safeguard against future changes.
///
///</summary>

			if (NEVER (pPager.errCode != 0))
				return SQLITE_OK;
			if (pPager.doNotSpill != 0)
				return SQLITE_OK;
			if (pPager.doNotSyncSpill != 0 && (pPg.flags & PGHDR_NEED_SYNC) != 0) {
				return SQLITE_OK;
			}
			pPg.pDirty = null;
			if (pPager.pagerUseWal ()) {
				///
///<summary>
///Write a single frame for this page to the log. 
///</summary>

				if (pPg.subjRequiresPage ()) {
					rc = subjournalPage (pPg);
				}
				if (rc == SQLITE_OK) {
					rc = pPager.pagerWalFrames (pPg, 0, 0, 0);
				}
			}
			else {
				///
///<summary>
///Sync the journal file if required. 
///</summary>

				if ((pPg.flags & PGHDR_NEED_SYNC) != 0 || pPager.eState == PAGER_WRITER_CACHEMOD) {
					rc = pPager.syncJournal (1);
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

				if (NEVER (rc == SQLITE_OK && pPg.pgno > pPager.dbSize && pPg.subjRequiresPage ())) {
					rc = subjournalPage (pPg);
				}
				///
///<summary>
///Write the contents of the page out to the database file. 
///</summary>

				if (rc == SQLITE_OK) {
					Debug.Assert ((pPg.flags & PGHDR_NEED_SYNC) == 0);
					rc = pPager.pager_write_pagelist (pPg);
				}
			}
			///
///<summary>
///Mark the page as clean. 
///</summary>

			if (rc == SQLITE_OK) {
				PAGERTRACE ("STRESS %d page %d\n", PAGERID (pPager), pPg.pgno);
				sqlite3PcacheMakeClean (pPg);
			}
			return pPager.pager_error (rc);
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
		/// via the sqlite3PagerGetExtra() API.
		///
		/// The flags argument is used to specify properties that affect the
		/// operation of the pager. It should be passed some bitwise combination
		/// of the PAGER_OMIT_JOURNAL and PAGER_NO_READLOCK flags.
		///
		/// The vfsFlags parameter is a bitmask to pass to the flags parameter
		/// of the xOpen() method of the supplied VFS when opening files.
		///
		/// If the pager object is allocated and the specified file opened
		/// successfully, SQLITE_OK is returned and *ppPager set to point to
		/// the new pager object. If an error occurs, *ppPager is set to NULL
		/// and error code returned. This function may return SQLITE_NOMEM
		/// (sqlite3Malloc() is used to allocate memory), SQLITE_CANTOPEN or
		/// various SQLITE_IO_XXX errors.
		///
		///</summary>
		static int sqlite3PagerOpen (sqlite3_vfs pVfs, ///
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

			int rc = SQLITE_OK;
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

			int pcacheSize = sqlite3PcacheSize ();
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

			if (sqlite3JournalSize (pVfs) > sqlite3MemJournalSize ()) {
				journalFileSize = ROUND8 (sqlite3JournalSize (pVfs));
			}
			else {
				journalFileSize = ROUND8 (sqlite3MemJournalSize ());
			}
			///
///<summary>
///Set the output variable to NULL in case an error occurs. 
///</summary>

			ppPager = null;
			#if !SQLITE_OMIT_MEMORYDB
			if ((flags & PAGER_MEMORY) != 0) {
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

			if (!String.IsNullOrEmpty (zFilename)) {
				string z;
				nPathname = pVfs.mxPathname + 1;
				zPathname = new StringBuilder (nPathname * 2);
				// sqlite3Malloc( nPathname * 2 );
				//if ( zPathname == null )
				//{
				//  return SQLITE_NOMEM;
				//}
				//zPathname[0] = 0; /* Make sure initialized even if FullPathname() fails */
				rc = sqlite3OsFullPathname (pVfs, zFilename, nPathname, zPathname);
				nPathname = StringExtensions.sqlite3Strlen30 (zPathname);
				z = zUri = zFilename;
				//.Substring(StringExtensions.sqlite3Strlen30( zFilename ) );
				//while ( *z )
				//{
				//  z += StringExtensions.sqlite3Strlen30( z ) + 1;
				//  z += StringExtensions.sqlite3Strlen30( z ) + 1;
				//}
				nUri = zUri.Length;
				//        &z[1] - zUri;
				if (rc == SQLITE_OK && nPathname + 8 > pVfs.mxPathname) {
					///
///<summary>
///This branch is taken when the journal path required by
///the database being opened will be more than pVfs.mxPathname
///bytes in length. This means the database cannot be opened,
///as it will not be possible to open the journal file or even
///</summary>
///<param name="check for a hot">journal before reading.</param>
///<param name=""></param>

					rc = SQLITE_CANTOPEN_BKPT ();
				}
				if (rc != SQLITE_OK) {
					//sqlite3_free( ref zPathname );
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

			//pPtr = (u8 *)sqlite3MallocZero(
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
			//  //sqlite3_free(zPathname);
			//  return SQLITE_NOMEM;
			//}
			pPager = new Pager ();
			//(Pager*)(pPtr);
			pPager.pPCache = new PCache ();
			//(PCache*)(pPtr += ROUND8(sizeof(*pPager)));
			pPager.fd = new sqlite3_file ();
			//(sqlite3_file*)(pPtr += ROUND8(pcacheSize));
			pPager.sjfd = new sqlite3_file ();
			//(sqlite3_file*)(pPtr += ROUND8(pVfs.szOsFile));
			pPager.jfd = new sqlite3_file ();
			//(sqlite3_file*)(pPtr += journalFileSize);
			//pPager.zFilename =    (char*)(pPtr += journalFileSize);
			//Debug.Assert( EIGHT_BYTE_ALIGNMENT(pPager.jfd) );
			///
///<summary>
///Fill in the Pager.zFilename and Pager.zJournal buffers, if required. 
///</summary>

			if (zPathname != null) {
				Debug.Assert (nPathname > 0);
				//pPager.zJournal =   (char*)(pPtr += nPathname + 1 + nUri);
				//memcpy(pPager.zFilename, zPathname, nPathname);
				pPager.zFilename = zPathname.ToString ();
				zUri = pPager.zFilename;
				//.Substring( nPathname + 1 );//memcpy( &pPager.zFilename[nPathname + 1], zUri, nUri );
				//memcpy(pPager.zJournal, zPathname, nPathname);
				//memcpy(&pPager.zJournal[nPathname], "-journal", 8);
				pPager.zJournal = pPager.zFilename + "-journal";
				sqlite3FileSuffix3 (pPager.zFilename, pPager.zJournal);
				#if !SQLITE_OMIT_WAL
																																																																												pPager.zWal = &pPager.zJournal[nPathname+8+1];
memcpy(pPager.zWal, zPathname, nPathname);
memcpy(&pPager.zWal[nPathname], "-wal", 4);
        sqlite3FileSuffix3(pPager.zFilename, pPager.zWal);
#endif
				//sqlite3_free( ref zPathname );
			}
			else {
				pPager.zFilename = "";
			}
			pPager.pVfs = pVfs;
			pPager.vfsFlags = (u32)vfsFlags;
			///
///<summary>
///Open the pager file.
///
///</summary>

			if (!String.IsNullOrEmpty (zFilename)) {
				int fout = 0;
				///
///<summary>
///VFS flags returned by xOpen() 
///</summary>

				rc = sqlite3OsOpen (pVfs, pPager.zFilename, pPager.fd, vfsFlags, ref fout);
				Debug.Assert (0 == memDb);
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

				if (rc == SQLITE_OK && !readOnly) {
					pPager.setSectorSize ();
					Debug.Assert (SQLITE_DEFAULT_PAGE_SIZE <= SQLITE_MAX_DEFAULT_PAGE_SIZE);
					if (szPageDflt < pPager.sectorSize) {
						if (pPager.sectorSize > SQLITE_MAX_DEFAULT_PAGE_SIZE) {
							szPageDflt = SQLITE_MAX_DEFAULT_PAGE_SIZE;
						}
						else {
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
			else {
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
				pPager.eState = PAGER_READER;
				pPager.eLock = EXCLUSIVE_LOCK;
				readOnly = (vfsFlags & SQLITE_OPEN_READONLY) != 0;
			}
			///
///<summary>
///The following call to PagerSetPagesize() serves to set the value of
///Pager.pageSize and to allocate the Pager.pTmpSpace buffer.
///
///</summary>

			if (rc == SQLITE_OK) {
				Debug.Assert (pPager.memDb == 0);
				rc = pPager.sqlite3PagerSetPagesize (ref szPageDflt, -1);
				testcase (rc != SQLITE_OK);
			}
			///
///<summary>
///If an error occurred in either of the blocks above, free the
///Pager structure and close the file.
///
///</summary>

			if (rc != SQLITE_OK) {
				Debug.Assert (null == pPager.pTmpSpace);
				sqlite3OsClose (pPager.fd);
				//sqlite3_free( ref pPager );
				return rc;
			}
			///
///<summary>
///Initialize the PCache object. 
///</summary>

			Debug.Assert (nExtra < 1000);
			nExtra = ROUND8 (nExtra);
			sqlite3PcacheOpen ((int)szPageDflt, nExtra, 0 == memDb, 0 == memDb ? (dxStress)pagerStress : null, pPager, pPager.pPCache);
			PAGERTRACE ("OPEN %d %s\n", FILEHANDLEID (pPager.fd), pPager.zFilename);
			IOTRACE ("OPEN %p %s\n", pPager, pPager.zFilename);
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
			Debug.Assert (tempFile == PAGER_LOCKINGMODE_NORMAL || tempFile == PAGER_LOCKINGMODE_EXCLUSIVE);
			Debug.Assert (PAGER_LOCKINGMODE_EXCLUSIVE == 1);
			pPager.exclusiveMode = tempFile != 0;
			pPager.changeCountDone = pPager.tempFile;
			pPager.memDb = memDb;
			pPager.readOnly = readOnly;
			Debug.Assert (useJournal || pPager.tempFile);
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
			Debug.Assert (isOpen (pPager.fd) || tempFile != 0);
			pPager.setSectorSize ();
			if (!useJournal) {
				pPager.journalMode = PAGER_JOURNALMODE_OFF;
			}
			else
				if (memDb != 0) {
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
			return SQLITE_OK;
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
		static void sqlite3PagerUnref (DbPage pPg)
		{
			if (pPg != null) {
				Pager pPager = pPg.pPager;
				sqlite3PcacheRelease (pPg);
				pPager.pagerUnlockIfUnused ();
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
		static int pager_write (PgHdr pPg)
		{
			byte[] pData = pPg.pData;
			Pager pPager = pPg.pPager;
			int rc = SQLITE_OK;
			///
///<summary>
///</summary>
///<param name="This routine is not called unless a write">transaction has already </param>
///<param name="been started. The journal file may or may not be open at this point.">been started. The journal file may or may not be open at this point.</param>
///<param name="It is never called in the ERROR state.">It is never called in the ERROR state.</param>
///<param name=""></param>

			Debug.Assert (pPager.eState == PAGER_WRITER_LOCKED || pPager.eState == PAGER_WRITER_CACHEMOD || pPager.eState == PAGER_WRITER_DBMOD);
			Debug.Assert (pPager.assert_pager_state ());
			///
///<summary>
///If an error has been previously detected, report the same error
///again. This should not happen, but the check provides robustness. 
///</summary>

			if (NEVER (pPager.errCode) != 0)
				return pPager.errCode;
			///
///<summary>
///</summary>
///<param name="Higher">level routines never call this function if database is not</param>
///<param name="writable.  But check anyway, just for robustness. ">writable.  But check anyway, just for robustness. </param>

			if (NEVER (pPager.readOnly))
				return SQLITE_PERM;
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
///<param name="This is done before calling sqlite3PcacheMakeDirty() on the page. ">This is done before calling sqlite3PcacheMakeDirty() on the page. </param>
///<param name="Otherwise, if it were done after calling sqlite3PcacheMakeDirty(), then">Otherwise, if it were done after calling sqlite3PcacheMakeDirty(), then</param>
///<param name="an error might occur and the pager would end up in WRITER_LOCKED state">an error might occur and the pager would end up in WRITER_LOCKED state</param>
///<param name="with pages marked as dirty in the cache.">with pages marked as dirty in the cache.</param>

			if (pPager.eState == PAGER_WRITER_LOCKED) {
				rc = pPager.pager_open_journal ();
				if (rc != SQLITE_OK)
					return rc;
			}
			Debug.Assert (pPager.eState >= PAGER_WRITER_CACHEMOD);
			Debug.Assert (pPager.assert_pager_state ());
			///
///<summary>
///Mark the page as dirty.  If the page has already been written
///to the journal then we can return right away.
///
///</summary>

			sqlite3PcacheMakeDirty (pPg);
			if (pPg.pageInJournal () && !pPg.subjRequiresPage ()) {
				Debug.Assert (!pPager.pagerUseWal ());
			}
			else {
				///
///<summary>
///The transaction journal now exists and we have a RESERVED or an
///EXCLUSIVE lock on the main database file.  Write the current page to
///the transaction journal if it is not there already.
///
///</summary>

				if (!pPg.pageInJournal () && !pPager.pagerUseWal ()) {
					Debug.Assert (pPager.pagerUseWal () == false);
					if (pPg.pgno <= pPager.dbOrigSize && isOpen (pPager.jfd)) {
						u32 cksum;
						byte[] pData2 = null;
						i64 iOff = pPager.journalOff;
						///
///<summary>
///We should never write to the journal file the page that
///contains the database locks.  The following Debug.Assert verifies
///that we do not. 
///</summary>

						Debug.Assert (pPg.pgno != ((PENDING_BYTE / (pPager.pageSize)) + 1));
						//PAGER_MJ_PGNO(pPager) );
						Debug.Assert (pPager.journalHdr <= pPager.journalOff);
						if (CODEC2 (pPager, pData, pPg.pgno, SQLITE_ENCRYPT_READ_CTX, ref pData2))
							return SQLITE_NOMEM;
						// CODEC2(pPager, pData, pPg.pgno, 7, return SQLITE_NOMEM, pData2);
						cksum = pPager.pager_cksum (pData2);
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
						rc = write32bits (pPager.jfd, iOff, pPg.pgno);
						if (rc != SQLITE_OK)
							return rc;
						rc = sqlite3OsWrite (pPager.jfd, pData2, pPager.pageSize, iOff + 4);
						if (rc != SQLITE_OK)
							return rc;
						rc = write32bits (pPager.jfd, iOff + pPager.pageSize + 4, cksum);
						if (rc != SQLITE_OK)
							return rc;
						IOTRACE ("JOUT %p %d %lld %d\n", pPager, pPg.pgno, pPager.journalOff, pPager.pageSize);
						#if SQLITE_TEST
																																																																																																																		#if !TCLSH
																																																																																																																		            PAGER_INCR( ref sqlite3_pager_writej_count );
#else
																																																																																																																		            int iValue = sqlite3_pager_writej_count.iValue;
            PAGER_INCR( ref iValue );
            sqlite3_pager_writej_count.iValue = iValue;
#endif
																																																																																																																		#endif
						PAGERTRACE ("JOURNAL %d page %d needSync=%d hash(%08x)\n", PAGERID (pPager), pPg.pgno, ((pPg.flags & PGHDR_NEED_SYNC) != 0 ? 1 : 0), pPg.pager_pagehash ());
						pPager.journalOff += 8 + pPager.pageSize;
						pPager.nRec++;
						Debug.Assert (pPager.pInJournal != null);
						rc = sqlite3BitvecSet (pPager.pInJournal, pPg.pgno);
						testcase (rc == SQLITE_NOMEM);
						Debug.Assert (rc == SQLITE_OK || rc == SQLITE_NOMEM);
						rc |= pPager.addToSavepointBitvecs (pPg.pgno);
						if (rc != SQLITE_OK) {
							Debug.Assert (rc == SQLITE_NOMEM);
							return rc;
						}
					}
					else {
						if (pPager.eState != PAGER_WRITER_DBMOD) {
							pPg.flags |= PGHDR_NEED_SYNC;
						}
						PAGERTRACE ("APPEND %d page %d needSync=%d\n", PAGERID (pPager), pPg.pgno, ((pPg.flags & PGHDR_NEED_SYNC) != 0 ? 1 : 0));
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

				if (pPg.subjRequiresPage ()) {
					rc = subjournalPage (pPg);
				}
			}
			///
///<summary>
///Update the database size and return.
///
///</summary>

			if (pPager.dbSize < pPg.pgno) {
				pPager.dbSize = pPg.pgno;
			}
			return rc;
		}

		///
///<summary>
///Mark a data page as writeable. This routine must be called before
///making changes to a page. The caller must check the return value
///of this function and be careful not to change any page data unless
///this routine returns SQLITE_OK.
///
///The difference between this function and pager_write() is that this
///function also deals with the special case where 2 or more pages
///</summary>
///<param name="fit on a single disk sector. In this case all co">resident pages</param>
///<param name="must have been written to the journal file before returning.">must have been written to the journal file before returning.</param>
///<param name=""></param>
///<param name="If an error occurs, SQLITE_NOMEM or an IO error code is returned">If an error occurs, SQLITE_NOMEM or an IO error code is returned</param>
///<param name="as appropriate. Otherwise, SQLITE_OK.">as appropriate. Otherwise, SQLITE_OK.</param>
///<param name=""></param>

		static int sqlite3PagerWrite (DbPage pDbPage)
		{
			int rc = SQLITE_OK;
			PgHdr pPg = pDbPage;
			Pager pPager = pPg.pPager;
			u32 nPagePerSector = (u32)(pPager.sectorSize / pPager.pageSize);
			Debug.Assert (pPager.eState >= PAGER_WRITER_LOCKED);
			Debug.Assert (pPager.eState != PAGER_ERROR);
			Debug.Assert (pPager.assert_pager_state ());
			if (nPagePerSector > 1) {
				Pgno nPageCount = 0;
				///
///<summary>
///Total number of pages in database file 
///</summary>

				Pgno pg1;
				///
///<summary>
///First page of the sector pPg is located on. 
///</summary>

				Pgno nPage = 0;
				///
///<summary>
///Number of pages starting at pg1 to journal 
///</summary>

				int ii;
				///
///<summary>
///Loop counter 
///</summary>

				bool needSync = false;
				///
///<summary>
///True if any page has PGHDR_NEED_SYNC 
///</summary>

				///
///<summary>
///Set the doNotSyncSpill flag to 1. This is because we cannot allow
///a journal header to be written between the pages journaled by
///this function.
///
///</summary>

				Debug.Assert (
				#if SQLITE_OMIT_MEMORYDB
																																																																												0==MEMDB
#else
				0 == pPager.memDb
				#endif
				);
				Debug.Assert (pPager.doNotSyncSpill == 0);
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
				if (pPg.pgno > nPageCount) {
					nPage = (pPg.pgno - pg1) + 1;
				}
				else
					if ((pg1 + nPagePerSector - 1) > nPageCount) {
						nPage = nPageCount + 1 - pg1;
					}
					else {
						nPage = nPagePerSector;
					}
				Debug.Assert (nPage > 0);
				Debug.Assert (pg1 <= pPg.pgno);
				Debug.Assert ((pg1 + nPage) > pPg.pgno);
				for (ii = 0; ii < nPage && rc == SQLITE_OK; ii++) {
					u32 pg = (u32)(pg1 + ii);
					PgHdr pPage = new PgHdr ();
					if (pg == pPg.pgno || sqlite3BitvecTest (pPager.pInJournal, pg) == 0) {
						if (pg != ((PENDING_BYTE / (pPager.pageSize)) + 1))//PAGER_MJ_PGNO(pPager))
						 {
							rc = pPager.sqlite3PagerGet (pg, ref pPage);
							if (rc == SQLITE_OK) {
								rc = pager_write (pPage);
								if ((pPage.flags & PGHDR_NEED_SYNC) != 0) {
									needSync = true;
								}
								sqlite3PagerUnref (pPage);
							}
						}
					}
					else
						if ((pPage = pPager.pager_lookup (pg)) != null) {
							if ((pPage.flags & PGHDR_NEED_SYNC) != 0) {
								needSync = true;
							}
							sqlite3PagerUnref (pPage);
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

				if (rc == SQLITE_OK && needSync) {
					Debug.Assert (
					#if SQLITE_OMIT_MEMORYDB
																																																																																															0==MEMDB
#else
					0 == pPager.memDb
					#endif
					);
					for (ii = 0; ii < nPage; ii++) {
						PgHdr pPage = pPager.pager_lookup ((u32)(pg1 + ii));
						if (pPage != null) {
							pPage.flags |= PGHDR_NEED_SYNC;
							sqlite3PagerUnref (pPage);
						}
					}
				}
				Debug.Assert (pPager.doNotSyncSpill == 1);
				pPager.doNotSyncSpill--;
			}
			else {
				rc = pager_write (pDbPage);
			}
			return rc;
		}

		///<summary>
		/// Return TRUE if the page given in the argument was previously passed
		/// to sqlite3PagerWrite().  In other words, return TRUE if it is ok
		/// to change the content of the page.
		///
		///</summary>
		#if !NDEBUG
																																						    static bool sqlite3PagerIswriteable( DbPage pPg )
    {
      return ( pPg.flags & PGHDR_DIRTY ) != 0;
    }
#else
		static bool sqlite3PagerIswriteable (DbPage pPg)
		{
			return true;
		}

		#endif
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
		static void sqlite3PagerDontWrite (PgHdr pPg)
		{
			Pager pPager = pPg.pPager;
			if ((pPg.flags & PGHDR_DIRTY) != 0 && pPager.nSavepoint == 0) {
				PAGERTRACE ("DONT_WRITE page %d of %d\n", pPg.pgno, PAGERID (pPager));
				IOTRACE ("CLEAN %p %d\n", pPager, pPg.pgno);
				pPg.flags |= PGHDR_DONT_WRITE;
				pPg.pager_set_pagehash ();
			}
		}

		///<summary>
		/// Return the number of references to the specified page.
		///
		///</summary>
		static int sqlite3PagerPageRefcount (DbPage pPage)
		{
			return sqlite3PcachePageRefcount (pPage);
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
      a[2] = sqlite3PcacheGetCachesize( pPager.pPCache );
      a[3] = pPager.eState == PAGER_OPEN ? -1 : (int)pPager.dbSize;
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
		/// Return a pointer to the data for the specified page.
		///</summary>
		static byte[] sqlite3PagerGetData (DbPage pPg)
		{
			Debug.Assert (pPg.nRef > 0 || pPg.pPager.memDb != 0);
			return pPg.pData;
		}

		///<summary>
		/// Return a pointer to the Pager.nExtra bytes of "extra" space
		/// allocated along with the specified page.
		///
		///</summary>
		static MemPage sqlite3PagerGetExtra (DbPage pPg)
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
  int rc = SQLITE_OK;
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
if( rc!=SQLITE_OK ){
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
int rc = SQLITE_OK;

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
if( rc==SQLITE_OK ){
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
** return SQLITE_OK. If an error occurs or the VFS used by the pager does 
** not support the xShmXXX() methods, return an error code. *pbOpen is
** not modified in either case.
**
** If the pager is open on a temp-file (or in-memory database), or if
** the WAL file is already open, set *pbOpen to 1 and return SQLITE_OK
** without doing anything.
*/
int sqlite3PagerOpenWal(
Pager *pPager,                  /* Pager object */
int *pbOpen                     /* OUT: Set to true if call is a no-op */
){
int rc = SQLITE_OK;             /* Return code */

assert( assert_pager_state(pPager) );
assert( pPager.eState==PAGER_OPEN   || pbOpen );
assert( pPager.eState==PAGER_READER || !pbOpen );
assert( pbOpen==0 || *pbOpen==0 );
assert( pbOpen!=0 || (!pPager.tempFile && !pPager.pWal) );

if( !pPager.tempFile && !pPager.pWal ){
if( !sqlite3PagerWalSupported(pPager) ) return SQLITE_CANTOPEN;

/* Close any rollback journal previously open */
sqlite3OsClose(pPager.jfd);

rc = pagerOpenWal(pPager);
if( rc==SQLITE_OK ){
pPager.journalMode = PAGER_JOURNALMODE_WAL;
pPager.eState = PAGER_OPEN;
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
int rc = SQLITE_OK;

assert( pPager.journalMode==PAGER_JOURNALMODE_WAL );

/* If the log file is not already open, but does exist in the file-system,
** it may need to be checkpointed before the connection can switch to
** rollback mode. Open it now so this can happen.
*/
if( !pPager.pWal ){
int logexists = 0;
rc = pagerLockDb(pPager, SHARED_LOCK);
if( rc==SQLITE_OK ){
rc = sqlite3OsAccess(
pPager.pVfs, pPager.zWal, SQLITE_ACCESS_EXISTS, &logexists
);
}
if( rc==SQLITE_OK && logexists ){
rc = pagerOpenWal(pPager);
}
}

/* Checkpoint and close the log. Because an EXCLUSIVE lock is held on
** the database file, the log and log-summary files will be deleted.
*/
if( rc==SQLITE_OK && pPager.pWal ){
rc = pagerExclusiveLock(pPager);
if( rc==SQLITE_OK ){
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
	#endif
	}
}
